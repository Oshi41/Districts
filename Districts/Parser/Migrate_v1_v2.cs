using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.New.Implementation;
using Districts.New.Implementation.Classes;
using Districts.New.Interfaces;
using Districts.Singleton;
using Districts.ViewModel.TabsVM;
using Door = Districts.JsonClasses.Door;

namespace Districts.Parser
{
    public class Migrate_v1_v2
    {
        private readonly v1.Parser _oldParser;
        private readonly v2.Parser _newParser;

        public Migrate_v1_v2(IAppSettings settings)
        {
            _oldParser = new v1.Parser();
            _newParser = new v2.Parser(settings);
        }

        public void Migrate()
        {
            MigrateCards();
        }

        private void MigrateCards()
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

        private iDoor ToDoor(Door door, ForbiddenElement element)
        {
            var status = DoorStatus.Good;

            if (element.Aggressive.Contains(door.Number))
                status |= DoorStatus.Aggressive;
            if (element.NoWorried.Contains(door.Number))
                status |= DoorStatus.NoWorry;
            if (element.NoVisit.Contains(door.Number))
                status |= DoorStatus.Restricted;



            return new New.Implementation.Classes.Door(door.Street,
                door.HouseNumber,
                door.Number,
                door.Entrance,
                status,
                door
                    .Codes
                    .Select(x => (iCode) new Code(x, CodeStatus.Good))
                    .ToList());
        }
    }

}
