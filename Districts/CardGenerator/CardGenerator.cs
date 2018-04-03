using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.Printing;
using Districts.Settings;
using Districts.ViewModel.TabsVM;
using Newtonsoft.Json;

namespace Districts.CardGenerator
{
    /// <summary>
    ///     Генератор карточек
    /// </summary>
    internal class CardGenerator
    {
        /// <summary>
        ///     настройки приложения
        /// </summary>
        private readonly ApplicationSettings settings = ApplicationSettings.ReadOrCreate();

        #region Public

        /// <summary>
        ///     сгенерировать карточки согласно домам, правилам доступа и кодам.
        /// </summary>
        public void Generate()
        {
            var allHomes = LoadingWork.LoadSortedHomes().Values.SelectMany(x => x).ToList();
            var allRules = LoadingWork.LoadRules().Values.ToList().SelectMany(x => x).ToList();
            var allCodes = LoadingWork.LoadCodes().Values.ToList().SelectMany(x => x).ToList();

            var allDoors = GetAllDoors(allHomes, allRules, allCodes);
            var cards = GenerateCards(allHomes, allRules, allCodes, allDoors);
            Write(cards);
        }

        public void GenerateNew(bool useBestDistribution = false)
        {
            var allHomes = LoadingWork.LoadSortedHomes().Values.SelectMany(x => x).ToList();
            var allRules = LoadingWork.LoadRules().Values.ToList().SelectMany(x => x).ToList();
            var allCodes = LoadingWork.LoadCodes().Values.ToList().SelectMany(x => x).ToList();

            var map = new HomeMap(allHomes, allCodes, allRules);
            var cards = GenerateCardsNew(map, useBestDistribution);
            Write(cards);
            //var doors = GetAllDoorsNew(map);
        }

        /// <summary>
        ///     Печатаем участки
        /// </summary>
        public void PrintVisual()
        {
            var cards = LoadingWork.LoadCards().Select(x => x.Value).ToList();
            //var codes = Helper.Helper.LoadCodes().Values.SelectMany(x => x).ToList();

            PrintVisual(cards);
        }

        #endregion

        #region Private

        /// <summary>
        ///     Получает все доступные квартиры квартиры
        /// </summary>
        /// <param name="allHomes">Список домов</param>
        /// <param name="allRules">Список правил посещения</param>
        /// <param name="allcodes">Список кодов</param>
        /// <returns></returns>
        private List<Door> GetAllDoors(List<Building> allHomes,
            List<ForbiddenElement> allRules,
            List<HomeInfo> allcodes)
        {
            var result = new List<Door>();

            foreach (var home in allHomes)
            {
                var rule = allRules.FirstOrDefault(x => home.IsTheSameObject(x))
                           ?? new ForbiddenElement(home);
                var code = allcodes.FirstOrDefault(x => home.IsTheSameObject(x))
                           ?? new HomeInfo(home);
                result.AddRange(GetHomeDoors(home, rule, code));
            }

            return result;
        }

        /// <summary>
        ///     Все незапрещенные квартиры в доме
        /// </summary>
        /// <param name="home">Дом</param>
        /// <param name="rule">Правила доступо</param>
        /// <param name="homeInfo">Код</param>
        /// <returns></returns>
        private IEnumerable<Door> GetHomeDoors(Building home,
            ForbiddenElement rule, HomeInfo homeInfo)
        {
            var all = new HashSet<int>(Enumerable.Range(1, home.Doors + 1));

            var forbidden = new HashSet<int>();
            forbidden.UnionWith(rule.Aggressive);
            forbidden.UnionWith(rule.NoVisit);
            forbidden.UnionWith(rule.NoWorried);

            all.ExceptWith(forbidden);

            foreach (var i in all)
            {
                var temp = new Door(home);
                temp.Number = i;
                temp.Entrance = GetEntrance(i, home.Floors, home.Entrances);
                var contains = homeInfo.AllCodes.ContainsKey(temp.Entrance);
                if (contains) temp.Codes.AddRange(homeInfo.AllCodes[temp.Entrance]);
                yield return temp;
            }
        }

        /// <summary>
        ///     Высчитывает подъезд, в котором находится
        /// </summary>
        /// <param name="floor">Номер квартиры</param>
        /// <param name="total">Всего квартир</param>
        /// <param name="totalEntrances">Всего подъездов</param>
        /// <returns></returns>
        private int GetEntrance(int floor, int total, int totalEntrances)
        {
            // квартиры в подъезде
            var onEntrance = Math.Ceiling((double) total / totalEntrances);

            // прохожу по всем подъездам
            for (var i = 0; i < totalEntrances; i++)
            {
                // номер подъезда
                var tempEntrance = i + 1;
                // если находится в пределах, то нашли наш
                if (floor >= onEntrance * i && floor <= onEntrance * tempEntrance) return tempEntrance;
            }

            return 1;
        }

        /// <summary>
        ///     Сгенерили карточки
        /// </summary>
        /// <param name="allHomes">Все дома</param>
        /// <param name="allRules">Все правила</param>
        /// <param name="allCodes">Все коды</param>
        /// <param name="doors">Полученныедвери</param>
        /// <returns></returns>
        private List<Card> GenerateCards(List<Building> allHomes,
            List<ForbiddenElement> allRules, List<HomeInfo> allCodes,
            List<Door> doors)
        {
            // итеративная переменная для циклической пробежки по всем карточкам
            var cardindex = 0;
            // макс. кол-во кв. в карточках
            var max = settings.MaxDoors;
            var amount = (int) Math.Ceiling(doors.Count / (double) max);

            // создаю опр. кол-во карточке
            var cards = new List<Card>(amount);
            for (var i = 0; i < amount; i++) cards.Add(new Card {Number = i + 1});

            // группирую квартиры по улицам
            var streets = doors.GroupBy(x => x.Street)
                .ToDictionary(x => x.Key, x => x.GetEnumerator()
                    .ToList());

            var homesRaw = new List<IList<Door>>();
            foreach (var street in streets)
            {
                // группирую по домам
                var streetHomes = street.Value.GroupBy(x => x.HouseNumber)
                    // выбираю все дома в список
                    .Select(x => x.GetEnumerator().ToList());
                // записываю его в рузельтат
                homesRaw.AddRange(streetHomes);
            }

            //
            // нашел на StackOverFlow отличное решение случайного перемешивания списка
            // https://stackoverflow.com/questions/273313/randomize-a-listt
            //
            var homes = homesRaw.OrderBy(x => Guid.NewGuid()).ToList();

            foreach (var home in homes)
                // пока есть свободные дверки
                while (home.Any())
                {
                    // получаю первую дступную карточку
                    var card = GetFreeCard(cards, cardindex);
                    // берем прям первый элемент
                    var chosen = home.First();

                    // todo сделать проверку на подъезд

                    card.Doors.Add(chosen);
                    home.Remove(chosen);

                    // перемещаем индекс
                    cardindex++;
                    if (cardindex >= cards.Count)
                        cardindex = 0;
                }

            return cards;
        }

        private Card GetFreeCard(List<Card> cards, int index)
        {
            if (index >= cards.Count)
                index = 0;

            var card = cards[index];

            if (card.Doors.Count >= settings.MaxDoors)
            {
                var find = cards.FindIndex(index, x => x.Doors.Count < settings.MaxDoors);

                return find > 0
                    ? cards[find]
                    : cards.FirstOrDefault(x => x.Doors.Count < settings.MaxDoors);
            }

            return card;
        }

        private void Write(List<Card> cards)
        {
            for (var i = 0; i < cards.Count; i++)
            {
                var toWrite = JsonConvert.SerializeObject(cards[i], Formatting.Indented);
                var filepath = Path.Combine(settings.CardsPath, "Карточка № " + cards[i].Number);
                File.WriteAllText(filepath, toWrite);
            }
        }

        private void PrintVisual(List<Card> cards)
        {
            var printDialog = new PrintDialog();
            var paginator = new CustomDocumentPaginator(cards, printDialog);
            // TODO
            ShowPreview(paginator);

            printDialog.PrintDocument(paginator, "Карточки");
        }

        private void ShowPreview(DocumentPaginator paginator)
        {
        }

        #endregion

        #region PrivateNew

        private List<Door> GetAllDoorsNew(HomeMap map)
        {
            var result = new List<Door>();

            if (!map.Any())
                return result;

            foreach (var full in map) result.AddRange(GetHomeDoorsNew(full, map));

            return result;
        }

        private IEnumerable<Door> GetHomeDoorsNew(FullHomeInfo fullInfo,
            HomeMap source)
        {
            // объявляем переменные для краткости
            var home = fullInfo.Building;
            var homeInfo = fullInfo.HomeInfo;
            var rule = fullInfo.ForbiddenElement;
            var start = homeInfo.Begin;

            var all = new HashSet<int>(Enumerable.Range(start, home.Doors));

            var forbidden = new HashSet<int>();
            forbidden.UnionWith(rule.Aggressive);
            forbidden.UnionWith(rule.NoVisit);
            forbidden.UnionWith(rule.NoWorried);

            all.ExceptWith(forbidden);

            foreach (var i in all)
            {
                var temp = new Door(home)
                {
                    Number = i,
                    Entrance = GetEntrance(i - start, home.Doors, home.Entrances)
                };

                var contains = homeInfo.AllCodes.ContainsKey(temp.Entrance);
                if (contains) temp.Codes.AddRange(homeInfo.AllCodes[temp.Entrance]);

                yield return temp;
            }
        }

        private CardWorker GenerateCardsNew(HomeMap map, bool bestDestribution)
        {
            // смапили дома и доступные квартиры
            var mappedDoors = new DoorsMap();
            foreach (var full in map) mappedDoors.Add(full, GetHomeDoorsNew(full, map).ToList());

            // ограничиваем кол-во карточек
            CardWorker cards;
            if (bestDestribution)
                cards = new CardWorker(mappedDoors.BiggestDoorsContainer);
            else
                cards = new CardWorker(GetMaxCardsCount(mappedDoors.DoorsCount, settings.MaxDoors));

            cards.SetCardCapacity(settings.MaxDoors);

            // перемешал дома в случайном порядке
            mappedDoors.ShuffleHomes();

            while (!mappedDoors.IsEmpty)
            {
                var pair = mappedDoors.FirstOrDefault();
                var full = pair.Key;
                var doors = pair.Value;

                while (doors.Any())
                {
                    var door = doors.FirstOrDefault();
                    cards.Add(door);
                    doors.Remove(door);
                }

                mappedDoors.Remove(full);
            }

            // перемешал записи в каждой карточке
            cards.ShuffleCardRecords();

            return cards;
        }

        private int GetMaxCardsCount(int doors, int capacity)
        {
            var result = (double) doors / capacity;
            return (int) Math.Ceiling(result);
        }

        #endregion

        //private void PrintVisualNew(List<Card> cards, List<HomeInfo> codes)
        //{
        //    PrintDialog printDlg = new PrintDialog();

        //    for (int i = 0; i < codes.Count; i += 2)
        //    {
        //        var fist = cards[i];
        //        var second = cards[i + 1];
        //        var vm = new PrintableViewMode(fist, second);
        //        var control = new PrintableCard { DataContext = vm };
        //        var filename = Path.Combine(settings.CardsPath, "Карточки " + i + " " + i + 1);

        //        SaveView(control, @"C:\log1.jpg");
        //        SaveUsingEncoder(control, @"C:\log1.jpg", new JpegBitmapEncoder());
        //        //printDlg.PrintVisual(control, "Card + " + i + " " + i + 1);
        //    }
        //}
        //private static void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        //{


        //    //Size visualSize = new Size(3005, 2800);
        //    //RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visualSize.Width, (int)visualSize.Height, 96, 96, PixelFormats.Default);

        //    //visual.Measure(visualSize);
        //    //visual.Arrange(new Rect(visualSize));
        //    //Thread.Sleep(2000);
        //    //bitmap.Render(visual);
        //    //BitmapFrame frame = BitmapFrame.Create(bitmap);
        //    //encoder.Frames.Add(frame);

        //    //using (var stream = File.Create(fileName))
        //    //{
        //    //    encoder.Save(stream);
        //    //}
        //}


        //private void StartPrint(Visual visual, string printerName, string fileName)
        //{
        //    PrintDialog dlg = new PrintDialog();
        //    dlg.PrintQueue = new PrintQueue(new PrintServer(), printerName);
        //    dlg.PrintTicket = new System.Printing.PrintTicket();

        //    dlg.PrintVisual(visual, "dfg");

        //    StandardPrintController controller = new StandardPrintController();


        //}

        //public static void SaveView(FrameworkElement viewName, string fileName, int dpi = 96)
        //{
        //    string destFolder = Path.GetDirectoryName(fileName);
        //    fileName = Path.GetFileName(fileName);

        //    Rect bounds = VisualTreeHelper.GetDescendantBounds(viewName);

        //    int width = (int)viewName.RenderSize.Width;
        //    int height = (int)viewName.RenderSize.Height;

        //    if (width == 0 && height == 0)
        //    {
        //        height = 1920;
        //        width = 1080;
        //    }

        //    viewName.BeginInit();
        //    viewName.EndInit();

        //    viewName.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
        //    viewName.Arrange(new Rect(viewName.DesiredSize));
        //    viewName.UpdateLayout();

        //    var size = new Size(width, height);

        //    viewName.Measure(size);
        //    viewName.Arrange(new Rect(size));


        //    RenderTargetBitmap rtb = new RenderTargetBitmap(
        //        (int) viewName.ActualWidth,
        //        (int) viewName.ActualHeight,
        //            dpi,
        //            dpi,
        //            PixelFormats.Default
        //        );

        //    rtb.Render(viewName);

        //    DrawingVisual dv = new DrawingVisual();

        //    using (DrawingContext ctx = dv.RenderOpen())
        //    {
        //        VisualBrush vb = new VisualBrush(viewName);
        //        ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
        //    }


        //    try
        //    {
        //        var jpgEncoder = new JpegBitmapEncoder();
        //        jpgEncoder.Frames.Add(BitmapFrame.Create(rtb));

        //        Byte[] _imageArray;

        //        using (MemoryStream outputStream = new MemoryStream())
        //        {
        //            jpgEncoder.Save(outputStream);
        //            _imageArray = outputStream.ToArray();
        //        }

        //        //Try Find Save Path, if doesn't exists, create it.
        //        if (Directory.Exists(destFolder) == false)
        //            Directory.CreateDirectory(destFolder);

        //        FileStream fileStream = new FileStream(Path.Combine(destFolder, fileName), FileMode.Create, System.IO.FileAccess.Write);

        //        fileStream.Write(_imageArray, 0, _imageArray.Length);
        //        fileStream.Close();
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}
    }

    internal class DoorsMap : Dictionary<FullHomeInfo, List<Door>>
    {
        public int DoorsCount => this.Aggregate(0, (i, pair) => i + pair.Value.Count);
        public int BiggestDoorsContainer => this.Max(x => x.Value.Count);

        public bool IsEmpty
        {
            get { return !this.Any(x => x.Value.Any()); }
        }

        public void ShuffleHomes()
        {
            var temp = this.Shuffle().ToDictionary(x => x.Key, x => x.Value);
            Clear();
            foreach (var pair in temp) Add(pair.Key, pair.Value);
        }
    }

    internal class CardWorker : List<Card>
    {
        private int _capacity;
        private int _innerIndex;

        public CardWorker(int count = int.MaxValue)
            : base(count)
        {
            for (var i = 0; i < count; i++) Add(new Card {Number = i + 1});
        }

        private void AddCounter()
        {
            // ограничиваем верхние пределы
            if (_innerIndex >= Count)
                _innerIndex = 0;
            else
                _innerIndex++;

            // нашли ближайшую доступную карточку
            _innerIndex = FindIndex(_innerIndex, x => x.Doors.Count < _capacity);

            // если не нашли выше, ищем по всему массиву
            if (_innerIndex < 0)
                _innerIndex = FindIndex(x => x.Doors.Count < _capacity);
        }

        public CardWorker SetCardCapacity(int capacity)
        {
            _capacity = capacity;
            return this;
        }

        public void Add(Door item)
        {
            if (_innerIndex < 0)
                return;

            this[_innerIndex].Doors.Add(item);
            AddCounter();
        }

        public void ShuffleCardRecords()
        {
            foreach (var card in this)
            {
                card.Doors = card.Doors.Shuffle().ToList();
            }
        }
    }
}