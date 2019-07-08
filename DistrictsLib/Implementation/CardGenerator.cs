using System;
using System.Collections.Generic;
using System.Linq;
using DistrictsLib.Extentions;
using DistrictsLib.Interfaces;
using DistrictsLib.Legacy.Helping;
using DistrictsLib.Legacy.JsonClasses;

namespace DistrictsLib.Implementation
{
    class CardGenerator : ICardGenerator
    {
        private readonly int _roomsCount;
        private readonly bool _isSorted;
        private readonly bool _oneRoomPerBuilding;
        private readonly bool _shuffleCards;

        public CardGenerator(int roomsCount = 25, 
            bool isSorted = true, 
            bool oneRoomPerBuilding = false,
            bool _shuffleCards = true)
        {
            _roomsCount = roomsCount;
            _isSorted = isSorted;
            _oneRoomPerBuilding = oneRoomPerBuilding;
            this._shuffleCards = _shuffleCards;
        }

        #region Implementation of ICardGenerator

        public List<Card> Generate(IList<Building> homes, IList<ForbiddenElement> rules, IList<HomeInfo> infos)
        {
            var result = new List<Card>();

            var allDoors = homes
                .Select(home =>
                {
                    var rule = rules.FirstOrDefault(x => x.IsTheSameObject(home)) ?? new ForbiddenElement(home);
                    var info = infos.FirstOrDefault(x => x.IsTheSameObject(home)) ?? new HomeInfo(home);

                    return GetDoors(home, rule, info);
                })
                .AsParallel()
                .ToList();

            var cardIterator = _oneRoomPerBuilding
                // Макс. кол-во на карточке зависит от самого большого дома
                ? new CardWorker(allDoors.Max(x => x.GetEnumerator().ToIEnumerable().Count()))
                : new CardWorker(_roomsCount);

            foreach (var homeDoors in allDoors)
            {
                var doors = homeDoors.GetEnumerator().ToIEnumerable().ToList();

                while (doors.Any())
                {
                    cardIterator.Add(doors.FirstOrDefault());
                    doors.RemoveAt(0);
                }
            }

            if (_isSorted)
            {
                cardIterator.SortDoors();
            }
            else
            {
                cardIterator.ShuffleDoors();
            }

            result.AddRange(cardIterator);

            if (_shuffleCards)
            {
                result = result.Shuffle().ToList();
            }

            return result;
        }

        #endregion

        private IEnumerable<Door> GetDoors(Building home, ForbiddenElement rule, HomeInfo homeInfo)
        {
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

        private int GetEntrance(int floor, int total, int totalEntrances)
        {
            // Кол-во квартир пв подъезде
            var entranceCount = (int) Math.Ceiling((double) total / totalEntrances);

            for (int i = 1; i <= totalEntrances; i++)
            {
                var firstRoom = entranceCount * (i - 1) + 1;
                var lastRoom = entranceCount * i;

                if (firstRoom <= floor && floor <= lastRoom)
                    return i;
            }

            return 1;
        } 
    }
}
