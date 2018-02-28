using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Xps;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.Printing;
using Districts.Settings;
using Districts.ViewModel;
using Districts.ViewModel.PrintVM;
using Districts.Views;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Image = System.Drawing.Image;

namespace Districts.CardGenerator
{
    /// <summary>
    /// Генератор карточек
    /// </summary>
    class CardGenerator
    {
        /// <summary>
        /// настройки приложения
        /// </summary>
        ApplicationSettings settings = ApplicationSettings.ReadOrCreate();

        #region Public

        /// <summary>
        /// сгенерировать карточки согласно домам, правилам доступа и кодам.
        /// </summary>
        public void Generate()
        {
            List<Building> allHomes = LoadingWork.LoadSortedHomes().Values.SelectMany(x => x).ToList();
            List<ForbiddenElement> allRules = LoadingWork.LoadRules().Values.ToList().SelectMany(x => x).ToList();
            List<Codes> allCodes = LoadingWork.LoadCodes().Values.ToList().SelectMany(x => x).ToList();

            List<Door> allDoors = GetAllDoors(allHomes, allRules, allCodes);
            List<Card> cards = GenerateCards(allHomes, allRules, allCodes, allDoors);
            Write(cards);
        }

        /// <summary>
        /// Печатаем участки
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
        /// Получает все доступные квартиры квартиры
        /// </summary>
        /// <param name="allHomes">Список домов</param>
        /// <param name="allRules">Список правил посещения</param>
        /// <param name="allcodes">Список кодов</param>
        /// <returns></returns>
        private List<Door> GetAllDoors(List<Building> allHomes,
            List<ForbiddenElement> allRules,
            List<Codes> allcodes)
        {
            List<Door> result = new List<Door>();

            foreach (var home in allHomes)
            {
                var rule = allRules.FirstOrDefault(x => home.IsTheSameObject(x))
                           ?? new ForbiddenElement(home.Street, home.HouseNumber);
                var code = allcodes.FirstOrDefault(x => home.IsTheSameObject(x))
                           ?? new Codes(home.Street, home.HouseNumber);
                result.AddRange(GetHomeDoors(home, rule, code));
            }

            return result;
        }

        /// <summary>
        /// Все незапрещенные квартиры в доме
        /// </summary>
        /// <param name="home">Дом</param>
        /// <param name="rule">Правила доступо</param>
        /// <param name="code">Код</param>
        /// <returns></returns>
        private IEnumerable<Door> GetHomeDoors(Building home,
            ForbiddenElement rule, Codes code)
        {
            var all = new HashSet<int>(Enumerable.Range(1, home.Doors + 1));

            var forbidden = new HashSet<int>();
            forbidden.UnionWith(rule.Aggressive);
            forbidden.UnionWith(rule.NoVisit);
            forbidden.UnionWith(rule.NoWorried);

            all.ExceptWith(forbidden);

            foreach (var i in all)
            {
                var temp = new Door(home.Street, home.HouseNumber);
                temp.Number = i;
                temp.Entrance = GetEntrance(i, home.Floors, home.Entrances);
                if (code.AllCodes.ContainsKey(temp.Entrance))
                {
                    temp.Codes.AddRange(code.AllCodes[temp.Entrance]);
                }
                yield return temp;
            }
        }

        /// <summary>
        /// Высчитывает подъезд, в котором находится 
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="total"></param>
        /// <param name="totalEntrances"></param>
        /// <returns></returns>
        private int GetEntrance(int floor, int total, int totalEntrances)
        {
            // квартиры в подъезде
            var onEntrance = total / totalEntrances;

            // прохожу по всем подъездам
            for (int i = 0; i < totalEntrances; i++)
            {
                // номер подъезда
                int tempEntrance = i + 1;
                // если находится в пределах, то нашли наш
                if (floor >= onEntrance * i && floor <= onEntrance * tempEntrance)
                {
                    return tempEntrance;
                }
            }

            return 1;
        }

        /// <summary>
        /// Сгенерили карточки
        /// </summary>
        /// <param name="allHomes">Все дома</param>
        /// <param name="allRules">Все правила</param>
        /// <param name="allCodes">Все коды</param>
        /// <param name="doors">Полученныедвери</param>
        /// <returns></returns>
        private List<Card> GenerateCards(List<Building> allHomes,
            List<ForbiddenElement> allRules, List<Codes> allCodes,
            List<Door> doors)
        {
            int cardindex = 0;
            int max = settings.MaxDoors;
            var amount = (int)Math.Ceiling((doors.Count / (double)max));

            var cards = new List<Card>(amount);
            for (int i = 0; i < amount; i++)
            {
                cards.Add(new Card { Number = i + 1 });
            }

            var homes = doors.GroupBy(x => x.HouseNumber).ToDictionary(x => x.Key,
                x => x.GetEnumerator().ToIEnumerable().ToList());

            foreach (var home in homes)
            {
                // пока есть свободные дверки
                while (home.Value.Any())
                {
                    var card = GetFreeCard(cards, cardindex);

                    var chosen = home.Value.First();

                    card.Doors.Add(chosen);
                    home.Value.Remove(chosen);

                    cardindex++;
                    if (cardindex >= cards.Count)
                        cardindex = 0;
                }
            }

            return cards;
        }

        private Card GetFreeCard(List<Card> cards, int index)
        {
            if (index >= cards.Count)
                index = 0;

            Card card = cards[index];

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
            for (int i = 0; i < cards.Count; i++)
            {
                var toWrite = JsonConvert.SerializeObject(cards[i], Formatting.Indented);
                var filepath = Path.Combine(settings.CardsPath, "Карточка № " + i);
                File.WriteAllText(filepath, toWrite);
            }
        }
        private void PrintVisual(List<Card> cards)
        {
            PrintDialog printDialog = new PrintDialog();

            var paginator = new CustomDocumentPaginator(cards, printDialog);

            printDialog.PrintDocument(paginator, "Карточки");
        }


        #endregion

        //private void PrintVisualNew(List<Card> cards, List<Codes> codes)
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
}
