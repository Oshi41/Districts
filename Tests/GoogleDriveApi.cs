using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Districts.Goggle;
using Districts.Helper;
using Districts.JsonClasses;
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
        [TestMethod]
        public void Connect_Test()
        {
            var parser = new Parser();
            var oldCards = new List<Card>
            {
                new Card
                {
                    Number = 15,
                    Doors = new List<Door>
                    {
                        new Door()
                    },
                },

                new Card
                {
                    Number = 244,
                    Doors = new List<Door>
                    {
                        new Door
                        {
                            Number = 1,
                            Entrance = 2
                        },

                        new Door
                        {
                            Number = 14,
                            Entrance = 254
                        },
                    },
                },
            };
            parser.SaveCards(oldCards);

            var login = "user";
            var api = new GoggleDriveApi(Districts.Properties.Resources.credentials, "tokens");
            Task.WaitAll(api.ConnectAsync(login));
            Task.WaitAll(api.Upload());

            var newCards = new List<Card>
            {
                new Card
                {
                    Number = 1,
                    Doors = new List<Door>
                    {
                        new Door()
                    },
                },

                new Card
                {
                    Number = 2,
                    Doors = new List<Door>
                    {
                        new Door
                        {
                            Number = 1,
                            Entrance = 2
                        }
                    },
                },
            };

            parser.SaveCards(newCards);

            Task.WaitAll(api.Download());

            var loaded = parser.LoadCards();

            Assert.IsTrue(oldCards.IsTermwiseEquals(loaded));
        }
    }
}
