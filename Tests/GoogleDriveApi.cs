using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Districts.Goggle;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.JsonClasses.Manage;
using Districts.Parser;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class GoogleDriveApi
    {
        private IGoogleDriveApi Api => new Districts.Goggle.GoogleDriveApi(new Parser());

        [TestMethod]
        public void ConnectTest()
        {
            Task.WaitAll(Api.Connect("test_name"));
        }

        [TestMethod]
        public void ParserSaveLoad_GApiSaveAndLoadTest()
        {
            var parser = new Parser();

            var codes = new List<HomeInfo>
            {
                new HomeInfo
                {
                    Street = "1",
                    HouseNumber = "1k1",
                    AllCodes = new Dictionary<int, List<string>>
                    {
                        {1, new List<string>
                            {
                                "1234"
                            }
                        }
                    },
                    Begin = 1
                }
            };

            var rules = new List<ForbiddenElement>
            {
                new ForbiddenElement
                {
                    Street = "1",
                    HouseNumber = "1k1",
                    Aggressive = new List<int>{1,2},
                    NoVisit = new List<int>{1,2},
                    NoWorried = new List<int>{1,2},
                    Comments = "1234",
                }
            };

            var cards = new List<Card>
            {
                new Card
                {
                    Number = 1,
                    Doors = new List<Door>
                    {
                        new Door
                        {
                            Number = 1,
                            Codes = new List<string>{"1234"},
                            Street = "1",
                            HouseNumber = "1k1",
                            Entrance = 1
                        }
                    }
                }
            };

            var manage = new List<CardManagement>
            {
                new CardManagement
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
                }
            };


            parser.SaveCodes(codes);
            parser.SaveRules(rules);
            parser.SaveCards(cards);
            parser.SaveManage(manage);

            // Дополнительно проверяем парсер
            Assert.IsTrue(codes.IsTermwiseEquals(parser.LoadCodes()));
            Assert.IsTrue(rules.IsTermwiseEquals(parser.LoadRules()));
            Assert.IsTrue(cards.IsTermwiseEquals(parser.LoadCards()));
            Assert.IsTrue(manage.IsTermwiseEquals(parser.LoadManage()));

            var api = Api;

            Task.WaitAll(api.Connect("test_name"));
            Task.WaitAll(api.Upload());
            Task.WaitAll(api.DownloadAndUpdate());

            // Дополнительно проверяем парсер
            Assert.IsTrue(codes.IsTermwiseEquals(parser.LoadCodes()));
            Assert.IsTrue(rules.IsTermwiseEquals(parser.LoadRules()));
            Assert.IsTrue(cards.IsTermwiseEquals(parser.LoadCards()));
            Assert.IsTrue(manage.IsTermwiseEquals(parser.LoadManage()));
        }
    }
}
