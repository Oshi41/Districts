using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.Helper;
using Districts.New.Implementation.Classes;
using Districts.New.Interfaces;
using Districts.Parser.v2;
using Districts.Singleton;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DistrictsTest.v2
{
    class FakeParser : IParser
    {
        private readonly iCard _card;

        public FakeParser(iCard card)
        {
            _card = card;
        }

        public iCard LoadCard(int number)
        {
            return _card;
        }

        public IList<string> LoadStreets()
        {
            throw new NotImplementedException();
        }

        public void SaveStreets(IList<string> streets)
        {
            throw new NotImplementedException();
        }

        public void UpdateRelationships()
        {
            throw new NotImplementedException();
        }

        public IList<iCard> LoadCards()
        {
            throw new NotImplementedException();
        }

        public IList<iHome> LoadHomes()
        {
            throw new NotImplementedException();
        }

        public void SaveHomes(IList<iHome> homes)
        {
            throw new NotImplementedException();
        }

        public IList<iManage> LoadManagement()
        {
            throw new NotImplementedException();
        }

        public void SaveCards(IList<Card> cards)
        {
            throw new NotImplementedException();
        }

        public void SaveHomes(IList<Home> homes)
        {
            throw new NotImplementedException();
        }

        public void SaveManagements(IList<Manage> history)
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class ManageParserTests
    {
        [TestMethod]
        public void Managements_ToJson_Equals()
        {
            var card = new Card(1, new List<iDoor>
            {
                new Door("1", 1, 1, 1, 1, 1, DoorStatus.Aggressive, new List<iCode>())
            });

            var managements = new List<iManage>
            {
                new Manage(new List<iRecord>
                {
                    new Record(ActionType.Deleted, "1", DateTime.Now),
                    new Record(ActionType.Taken, "2", DateTime.Now),
                }, card),

                new Manage(new List<iRecord>
                {
                    new Record(ActionType.Deleted, "1wedfs", DateTime.Now),
                }, card),
            };

            // инжектим фейковую реализацию
            IoC.Instance.Register<IParser>(new FakeParser(card));

            var json = JsonConvert.SerializeObject(managements);
            var copy = JsonConvert.DeserializeObject<List<Manage>>(json);

            Assert.IsTrue(managements.IsTermwiseEquals(copy));
        }
    }
}
