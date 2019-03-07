using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.JsonClasses.Manage;
using Google.Apis.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class EqualsTests
    {
        [TestMethod]
        public void TestCard()
        {
            var card1 = new Card
            {
                Number = 1,
                Doors = new List<Door>
                {
                    new Door
                    {
                        Number = 1,
                        Codes = new List<string> {"1234"},
                        Street = "1",
                        HouseNumber = "1k1",
                        Entrance = 1
                    }
                }
            };

            var card2 = new Card
            {
                Number = 1,
                Doors = new List<Door>
                {
                    new Door
                    {
                        Number = 1,
                        Codes = new List<string> {"1234"},
                        Street = "1",
                        HouseNumber = "1k1",
                        Entrance = 1
                    }
                }
            };

            Assert.AreEqual(card1, card2);
            Assert.AreEqual(card1.GetHashCode(), card2.GetHashCode());

            card2.Number++;

            Assert.AreNotEqual(card1, card2);
        }

        [TestMethod]
        public void TestCodes()
        {
            var code1 = new HomeInfo
            {
                Street = "1",
                HouseNumber = "1k1",
                AllCodes = new Dictionary<int, List<string>>
                {
                    {
                        1, new List<string>
                        {
                            "1234"
                        }
                    }
                },
                Begin = 1
            };

            var code2 = new HomeInfo
            {
                Street = "1",
                HouseNumber = "1k1",
                AllCodes = new Dictionary<int, List<string>>
                {
                    {
                        1, new List<string>
                        {
                            "1234"
                        }
                    }
                },
                Begin = 1
            };

            Assert.AreEqual(code1, code2);
            Assert.AreEqual(code1.GetHashCode(), code2.GetHashCode());

            code2.Street += " ";

            Assert.AreNotEqual(code1, code2);
        }

        [TestMethod]
        public void TestDoors()
        {
            var door1 = new Door
            {
                Number = 1,
                Codes = new List<string> {"1234"},
                Street = "1",
                HouseNumber = "1k1",
                Entrance = 1
            };

            var door2 = new Door
            {
                Number = 1,
                Codes = new List<string> { "1234" },
                Street = "1",
                HouseNumber = "1k1",
                Entrance = 1
            };

            Assert.AreEqual(door2, door1);
            Assert.AreEqual(door2.GetHashCode(), door1.GetHashCode());

            door1.Number++;

            Assert.AreNotEqual(door1, door2);
        }

        [TestMethod]
        public void TestCardManage()
        {
            var manage = new CardManagement
            {
                Number = 1,
                Actions = new List<ManageRecord>
                {
                    new ManageRecord
                    {
                        ActionType = ActionTypes.Taken,
                        Date = DateTime.Now,
                        Subject = "test"
                    }
                }
            };

            var manage2 = new CardManagement
            {
                Number = 1,
                Actions = new List<ManageRecord>
                {
                    new ManageRecord
                    {
                        ActionType = ActionTypes.Taken,
                        Date = DateTime.Now,
                        Subject = "test"
                    }
                }
            };

            Assert.AreEqual(manage, manage2);
            Assert.AreEqual(manage.GetHashCode(), manage2.GetHashCode());

            manage2.Number++;
            Assert.AreNotEqual(manage, manage2);
        }

        [TestMethod]
        public void TestRules()
        {
            var f = new ForbiddenElement
            {
                Street = "1",
                HouseNumber = "1k1",
                Aggressive = new List<int> {1, 2},
                NoVisit = new List<int> {1, 2},
                NoWorried = new List<int> {1, 2},
                Comments = "1234",
            };

            var s = new ForbiddenElement
            {
                Street = "1",
                HouseNumber = "1k1",
                Aggressive = new List<int> {1, 2},
                NoVisit = new List<int> {1, 2},
                NoWorried = new List<int> {1, 2},
                Comments = "1234",
            };

            Assert.AreEqual(f,s);
            Assert.AreEqual(f.GetHashCode(),s.GetHashCode());

            s.Street += " ";

            Assert.AreNotEqual(f,s);
        }

        [TestMethod]
        public void IsTermwiseEqualsTest()
        {
            var card1 = new Card
            {
                Number = 1,
                Doors = new List<Door>
                {
                    new Door
                    {
                        Number = 1,
                        Codes = new List<string> {"1234"},
                        Street = "1",
                        HouseNumber = "1k1",
                        Entrance = 1
                    }
                }
            };

            var card2 = new Card
            {
                Number = 1,
                Doors = new List<Door>
                {
                    new Door
                    {
                        Number = 1,
                        Codes = new List<string> {"1234"},
                        Street = "1",
                        HouseNumber = "1k1",
                        Entrance = 1
                    }
                }
            };

            var list1 = new List<Card> {card1};
            var list2 = new List<Card> {card2};

            Assert.IsTrue(list2.IsTermwiseEquals(list1));
        }
    }
}
