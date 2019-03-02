using System;
using System.Collections.Generic;
using System.Linq;
using Districts.Comparers;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.New.Implementation;
using Districts.New.Implementation.Classes;
using Districts.New.Interfaces;
using Districts.ViewModel.TabsVM;
using Door = Districts.JsonClasses.Door;

namespace Districts.Parser
{
    public class Migrate_v1_v2
    {
        private readonly IHomeParser _homeParser;
        private readonly v1.Parser _oldParser;
        private readonly v2.Parser _newParser;

        public Migrate_v1_v2(IAppSettings settings, IHomeParser homeParser)
        {
            _homeParser = homeParser;
            _oldParser = new v1.Parser();
            _newParser = new v2.Parser(settings);
        }

        /// <summary>
        /// Переносит карточки на новый формат
        /// </summary>
        public void MigrateCards()
        {
            var cards = _oldParser.LoadCards().Select(x => x.Value).ToList();
            var allForbidden = _oldParser.LoadRules().SelectMany(x => x.Value).ToList();

            var newCards = new List<New.Implementation.Classes.Card>();

            foreach (var oldCard in cards)
            {
                var doors = new List<iDoor>();

                foreach (var oldDoor in oldCard.Doors)
                {
                    var fb = allForbidden
                        .FirstOrDefault(x => x.IsTheSameObject(oldDoor));

                    doors.Add(ToDoor(oldDoor, fb ?? new ForbiddenElement(oldDoor)));
                }

                newCards.Add(new New.Implementation.Classes.Card(oldCard.Number, doors));
            }

            _newParser.SaveCards(newCards);
        }

        /// <summary>
        /// Переносит старую инфу. Не создает дома!!!
        /// </summary>
        public void MigrateHomes()
        {
            var allHomes = _oldParser.LoadSortedHomes().Values.SelectMany(x => x).ToList();
            var allRules = _oldParser.LoadRules().Values.ToList().SelectMany(x => x).ToList();
            var allCodes = _oldParser.LoadCodes().Values.ToList().SelectMany(x => x).ToList();
            
            // смапировали
            var map = new HomeMap(allHomes, allCodes, allRules);

            var homes = _newParser.LoadHomes();

            var comparer = new HouseNumberComparerFromString();

            foreach (var full in map)
            {
                var home = homes.FirstOrDefault(x => string.Equals(x.Street, full.Building.Street)
                                                     && 0 == comparer.Compare(x.ToString(), full.Building.HouseNumber));
                if (home == null)
                    continue;

                homes.Remove(home);

                var doors = home
                    .Doors
                    .Select(x => ToDoor(x, full));

                var copy = new Home(home, doors.ToList(),
                    full.Building.HasConcierge,
                    full.HomeInfo.Begin,
                    full.Building.Floors,
                    full.Building.Entrances);

                homes.Add(home);
            }

            _newParser.SaveHomes(homes);
        }
        
        private IList<iCode> ToCodes(IEnumerable<string> texts)
        {
            return texts.Select(x => (iCode) new Code(x, CodeStatus.Good)).ToList();
        }

        private iDoor ToDoor(Door door, ForbiddenElement element)
        {
            return new New.Implementation.Classes.Door(door.Street,
                door.HouseNumber,
                door.Number,
                door.Entrance,
                GetStatus(element, door.Number),
                ToCodes(door.Codes));
        }

        private iDoor ToDoor(iDoor door, FullHomeInfo full)
        {
            var number = full.HomeInfo.Begin - 1 + door.DoorNumber;
            var entrance = _homeParser.GetEntrance(number, 
                full.Building.Doors, 
                full.Building.Entrances);

            return new New.Implementation.Classes.Door(door,
                number,
                entrance,
                GetStatus(full.ForbiddenElement, number),
                ToCodes(full.HomeInfo.GetCodesByEntrance(entrance)));
        }

        private DoorStatus GetStatus(ForbiddenElement element, int number)
        {
            var status = DoorStatus.Good;

            if (element.Aggressive.Contains(number))
                status |= DoorStatus.Aggressive;
            if (element.NoWorried.Contains(number))
                status |= DoorStatus.NoWorry;
            if (element.NoVisit.Contains(number))
                status |= DoorStatus.Restricted;

            return status;
        }
    }

}
