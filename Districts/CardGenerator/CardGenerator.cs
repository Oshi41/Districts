using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.Printing;
using Districts.ViewModel.TabsVM;
using Newtonsoft.Json;
using Districts.Comparers;
using Districts.Settings.v1;

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
        private readonly ApplicationSettings _settings = ApplicationSettings.ReadOrCreate();

        #region Public

        ///// <summary>
        /////     сгенерировать карточки согласно домам, правилам доступа и кодам.
        ///// </summary>
        //public void Generate()
        //{
        //    var allHomes = LoadingWork.LoadSortedHomes().Values.SelectMany(x => x).ToList();
        //    var allRules = LoadingWork.LoadRules().Values.ToList().SelectMany(x => x).ToList();
        //    var allCodes = LoadingWork.LoadCodes().Values.ToList().SelectMany(x => x).ToList();

        //    var allDoors = GetAllDoors(allHomes, allRules, allCodes);
        //    var cards = GenerateCards(allHomes, allRules, allCodes, allDoors);
        //    Write(cards);
        //}

        /// <summary>
        /// Генерирует карточки
        /// </summary>
        /// <param name="uniqueEntry"></param>
        /// <param name="isSorted"></param>
        public void Generate(bool uniqueEntry, bool isSorted)
        {
            var allHomes = LoadingWork.LoadSortedHomes().Values.SelectMany(x => x).ToList();
            var allRules = LoadingWork.LoadRules().Values.ToList().SelectMany(x => x).ToList();
            var allCodes = LoadingWork.LoadCodes().Values.ToList().SelectMany(x => x).ToList();

            var map = new HomeMap(allHomes, allCodes, allRules);
            var cards = GenerateCards(map, uniqueEntry, isSorted);
            Write(cards);
            //var doors = GetAllDoors(map);
        }

        public void Repair(bool isSorted)
        {
            //
            // Загрузили из файлов всю инфу
            // 
            var allHomes = LoadingWork.LoadSortedHomes().Values.SelectMany(x => x).ToList();
            var allRules = LoadingWork.LoadRules().Values.ToList().SelectMany(x => x).ToList();
            var allCodes = LoadingWork.LoadCodes().Values.ToList().SelectMany(x => x).ToList();
            var allCards = LoadingWork.LoadCards();
            // смапировали
            var map = new HomeMap(allHomes, allCodes, allRules);
            // сгенерили все двери
            var allDoors = GetAllDoors(map);

            //
            // Пока что не планируется поддержка заполнения новых квартир,
            // сопряжено с трудностями сортировки.
            // Пока что просто логгируем то, что есть ещё квартиры для заполнения
            //

            // список квартир, которых нет в домах, информация о которых
            // была скачана
            var notExisting = new List<Door>();

            foreach (var pair in allCards)
            {
                Card card = pair.Value;

                for (int i = 0; i < card.Doors.Count; i++)
                {
                    var find = allDoors.FirstOrDefault(x => x.IsTheSameObject(card.Doors[i]));
                    if (find == null)
                    {
                        notExisting.Add(card.Doors[i]);
                        continue;
                    }

                    // заменяем на новую
                    card.Doors[i] = find;
                }

                if (isSorted)
                {
                    card.Doors = card
                        .Doors
                        .OrderBy(x => x.Street)
                        .ThenBy(x => x.HouseNumber, new HouseNumberComparerFromString())
                        .ToList();
                }
                else
                {
                    card.Doors = card.Doors.Shuffle().ToList();
                }
            }

            if (notExisting.Any())
            {
                var all = notExisting.Aggregate("", (main, door) => main += door.ToString() + "\n");
                Tracer.Tracer.Write("Следующих квартир не существует в выбранных домах: \n\n" + all);
            }

            var cardDoors = allCards.SelectMany(x => x.Value.Doors).ToList();
            var needToAdd = allDoors.Except(cardDoors, new BaseFindableObjectComparer()).ToList();
            if (needToAdd.Any())
            {
                //var all = needToAdd.Aggregate("", (main, door) => main += door.ToString() + "\n");
                Tracer.Tracer.Write("Данных квартир нет в участках: \n\n" + string.Join("\n", needToAdd));
            }

            foreach (var pair in allCards)
            {
                // записали карточку
                var toWrite = JsonConvert.SerializeObject(pair.Value, Formatting.Indented);
                File.WriteAllText(pair.Key, toWrite);
            }
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
        ///     Высчитывает подъезд, в котором находится
        /// </summary>
        /// <param name="floor">Номер квартиры</param>
        /// <param name="total">Всего квартир</param>
        /// <param name="totalEntrances">Всего подъездов</param>
        /// <returns></returns>
        private int GetEntrance(int floor, int total, int totalEntrances)
        {
            // квартиры в подъезде
            var onEntrance = Math.Ceiling((double)total / totalEntrances);

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

        private void Write(List<Card> cards)
        {
            for (var i = 0; i < cards.Count; i++)
            {
                var toWrite = JsonConvert.SerializeObject(cards[i], Formatting.Indented);
                var filepath = Path.Combine(_settings.CardsPath, "Карточка № " + cards[i].Number);
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

        private List<Door> GetAllDoors(HomeMap map)
        {
            var result = new List<Door>();

            if (!map.Any())
                return result;

            foreach (var full in map) result.AddRange(GetHomeDoors(full, map));

            return result;
        }

        private IEnumerable<Door> GetHomeDoors(FullHomeInfo fullInfo,
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

        /// <summary>
        /// Генерирует список карточек
        /// </summary>
        /// <param name="map">На карточке встречается только </param>
        /// <param name="uniqueEntry">Все квартиры на карточке из разных домов</param>
        /// <param name="isFlatsSorted">Сортируем квартиры на карточке</param>
        /// <returns></returns>
        private CardWorker GenerateCards(HomeMap map, bool uniqueEntry, bool isFlatsSorted)
        {
            // смапили дома и доступные квартиры
            var mappedDoors = new DoorsMap();
            foreach (var full in map) mappedDoors.Add(full, GetHomeDoors(full, map).ToList());

            // ограничиваем кол-во карточек
            CardWorker cards;
            if (uniqueEntry)
                cards = new CardWorker(mappedDoors.BiggestDoorsContainer);
            else
                cards = new CardWorker(GetMaxCardsCount(mappedDoors.DoorsCount, _settings.MaxDoors));

            cards.SetCardCapacity(_settings.MaxDoors);

            // перемешиваю дома для лучшего распределения
            mappedDoors.ShuffleHomes();

            var random = new Random();

            while (!mappedDoors.IsEmpty)
            {
                var pair = mappedDoors.GetRandom(random);
                var full = pair.Key;
                var doors = pair.Value;

                while (doors.Any())
                {
                    var door = doors.GetRandom(random);
                    cards.Add(door);
                    doors.Remove(door);
                }

                mappedDoors.Remove(full);
            }

            if (isFlatsSorted)
            {
                // рассортировал квартиры в карточках
                cards.SortCardRecords();
            }
            else
            {
                // перемешал записи в каждой карточке
                cards.ShuffleCardRecords();
            }

            return cards;
        }

        private int GetMaxCardsCount(int doors, int capacity)
        {
            var result = (double)doors / capacity;
            return (int)Math.Ceiling(result);
        }

        #endregion
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
            foreach (var pair in temp)
                Add(pair.Key, pair.Value);
        }

        //public void SortByStreet()
        //{
        //    var temp = this.OrderBy(x => x.Key.HomeInfo.Street)
        //        .ThenBy(x => x.Key.HomeInfo.HouseNumber)
        //        .ToDictionary(x => x.Key, x => x.Value);

        //    Clear();
        //    foreach (var pair in temp)
        //        Add(pair.Key, pair.Value);
        //}
    }

    internal class CardWorker : List<Card>
    {
        private int _capacity;
        private int _innerIndex;

        public CardWorker(int count = int.MaxValue)
            : base(count)
        {
            for (var i = 0; i < count; i++) Add(new Card { Number = i + 1 });
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
            {
                // пытаемся добавить ещё одну карточку
                if (Count >= _capacity)
                    return;

                Add(new Card { Number = Count + 1 });
                _innerIndex = Count - 1;
            }


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

        public void SortCardRecords()
        {
            var comparer = new HouseNumberComparerFromDoor(new HouseNumberComparerFromString());

            foreach (var card in this)
            {
                card.Doors = card.Doors.OrderBy(door => door, comparer).ToList();
            }
        }
    }
}