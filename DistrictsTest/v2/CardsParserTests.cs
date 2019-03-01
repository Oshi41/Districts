using System.Collections.Generic;
using Districts.Helper;
using Districts.New.Implementation.Classes;
using Districts.New.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DistrictsTest.v2
{
    [TestClass]
    public class CardsParserTests
    {
        [TestMethod]
        public void OneCard_ToJson_Equals()
        {
            var doors = new List<iDoor>
            {
                new Door("1", 1, 1, -1, 1, 1, DoorStatus.Good, new List<iCode>
                {
                    new Code("1234", CodeStatus.Good)
                }),

                new Door("2", 2, 2, 4, 1, 1, DoorStatus.NotWarning, new List<iCode>
                {
                    new Code("1234k4578", CodeStatus.NotWorking)
                }),
            };

            var card = new Card(1, doors);
            var json = JsonConvert.SerializeObject(card, Formatting.Indented);
            var copy = JsonConvert.DeserializeObject<Card>(json);

            Assert.AreEqual(card, copy);
        }

        [TestMethod]
        public void Cards_ToJson_Equals()
        {
            var cards = new List<iCard>
            {
                new Card(1,
                    new List<iDoor>
                    {
                        new Door("1", 1, 1, 1, 1, 1, DoorStatus.Good, new List<iCode>
                        {
                            new Code("1", CodeStatus.Good)
                        }),

                        new Door("2", 2, 2, 2, 2, 2, DoorStatus.NotWarning, new List<iCode>
                        {
                            new Code("1", CodeStatus.NotWorking),
                            new Code("2", CodeStatus.Good),
                        }),
                    }),

                new Card(2,
                    new List<iDoor>
                    {
                        new Door("2", 2, 2, 2, 2, 2, DoorStatus.Good, new List<iCode>
                        {
                            new Code("1", CodeStatus.Good)
                        }),

                        new Door("3", 3, 3, 4, 4, 3, DoorStatus.Restricted, new List<iCode>
                        {
                            new Code("1sdfsdf", CodeStatus.NotWorking),
                            new Code("zxcvzfv2", CodeStatus.Restricted),
                            new Code("xkdfjbhvxifhoishigsxghs", CodeStatus.Restricted),
                        }),
                    }),
            };

            var json = JsonConvert.SerializeObject(cards, Formatting.Indented);
            var copy = JsonConvert.DeserializeObject<List<Card>>(json);

            Assert.IsTrue(cards.IsTermwiseEquals(copy));
        }
    }
}
