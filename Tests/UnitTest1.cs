using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Districts.Backup;
using Districts.CardGenerator;
using Districts.Checker;
using Districts.JsonClasses;
using Districts.WebRequest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DoorsNumberArray_Regex_ForbiddenSymbols()
        {
            var regex = new Regex(@"[0-9,\-]");
            var contains = "123456789-----,,,62162132165";
            var notContains = "384351dsafgdfg684s6df5vs6dt8jh4d9tu8kif6hu";
            var totallyNotAontains = "sdfgxgfhxfgh";

            var res1 = regex.Match(contains);
            var res2 = regex.Match(notContains);
            var res3 = regex.Match(totallyNotAontains);

            Assert.IsTrue(int.Parse(res1.Value) == 1, "Must be only one math");
            Assert.IsTrue(int.Parse(res2.Value) > 1, "Must be only multiple mathes");
            Assert.IsTrue(string.IsNullOrEmpty(res3.Value), "Must be only no one math");
        }

        [TestMethod]
        public void Card_ToJson_Equals()
        {
            var card = new Card();
            for (var i = 0; i < 15; i++)
            {
                var door = new Door();
                card.Doors.Add(door);
            }

            var json = JsonConvert.SerializeObject(card, Formatting.Indented);

            var newCard = JsonConvert.DeserializeObject<Card>(json);

            Assert.IsTrue(newCard == card);
        }

        [TestMethod]
        public void HomeInfo_toJson_Equals()
        {
            var codes = new HomeInfo();

            for (var i = 0; i < 7; i++)
            {
                var tempCodes = new List<string>();
                for (var j = 0; j < 3; j++) tempCodes.Add(j * i * 357 + "");
                codes.AllCodes.Add(i, tempCodes);
            }

            var json = JsonConvert.SerializeObject(codes, Formatting.Indented);

            var newHomeInfo = JsonConvert.DeserializeObject<HomeInfo>(json);

            Assert.IsTrue(newHomeInfo == codes);
        }

        public async Task TestDownloadStreet()
        {
            var streetDownloadr = new StreetDownloader();
            await streetDownloadr.DownloadStreet("пролетарский");
        }

        public async Task TestCompleteDownload()
        {
            var download = new MainDownloader();
            download.DownloadInfo();
        }

        public void LoadBuildings()
        {
            //var tree = new EditTreeViewModel();
        }

        public void BackUp()
        {
            var backup = new BackupManager();
            backup.MakeBackup();
        }

        public void PrinVisual()
        {
            var print = new CardGenerator();
            print.PrintVisual();
        }

        public void TestCards()
        {
            var checker = new DoorChecker();
            var theSame = checker.FindRepeated();

            if (theSame.Any())
                throw new Exception();
        }

        [TestMethod]
        public void TestDirectorySearch()
        {
            var dir = @"D:\VsProjects\Oshi41\Districts";

            var childrenDirs = Directory.GetDirectories(dir);
            var childrenFiies = Directory.GetFiles(dir);
        }
    }
}
