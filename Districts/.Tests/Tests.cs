using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Districts.JsonClasses;
using Districts.ViewModel;
using Districts.WebRequest;
using Newtonsoft.Json;

namespace Districts.Tests
{
    /// <summary>
    /// Вспомогательный класс для тестов
    /// </summary>
    class Tests
    {
        public static void StartTests()
        {
            var test = new Tests();
            //test.TestCardJson();
            //test.TestCodesJson();
            //test.TestDownloadStreet();
            //test.TestCompleteDownload();
            //test.LoadBuildings();

            //test.BackUp();
            //test.PrinVisual();
            //test.TestCards();
            test.TestRegex();
        }

        private void TestRegex()
        {
            var regex = new Regex(@"[0-9,\-]");
            string contains = "123456789-----,,,62162132165";
            string notContains = "384351dsafgdfg684s6df5vs6dt8jh4d9tu8kif6hu";
            string totallyNotAontains = "sdfgxgfhxfgh";

            var res1 = regex.Match(contains);
            var res2 = regex.Match(notContains);
            var res3 = regex.Match(totallyNotAontains);

            var mathes1 = regex.Matches(contains);
            var mathes2 = regex.Matches(notContains);
            var mathes3 = regex.Matches(totallyNotAontains);

            if (int.Parse(res1.Value) != 1)
                throw new Exception("Must be only one math");

            if (int.Parse(res2.Value) < 2)
                throw new Exception("Must be only multiple mathes");

            if (!string.IsNullOrEmpty(res3.Value))
                throw new Exception("Must be only no one math");
        }

        public void TestCardJson()
        {
            var card = new Card();
            for (int i = 0; i < 15; i++)
            {
                var door = new Door();
                card.Doors.Add(door);
            }

            string json = JsonConvert.SerializeObject(card, Formatting.Indented);

            Card newCard = JsonConvert.DeserializeObject<Card>(json);

            bool equals = newCard == card;

            if (!equals)
                throw new Exception();
        }

        public void TestCodesJson()
        {
            var codes = new Codes();

            for (int i = 0; i < 7; i++)
            {
                List<string> tempCodes = new List<string>();
                for (int j = 0; j < 3; j++)
                {
                    tempCodes.Add((j * i * 357) + "");
                }
                codes.AllCodes.Add(i, tempCodes);
            }

            string json = JsonConvert.SerializeObject(codes, Formatting.Indented);

            Codes newCodes = JsonConvert.DeserializeObject<Codes>(json);
            bool equals = newCodes == codes;

            if (!equals)
                throw new Exception();
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
            var tree = new EditTreeViewModel();
        }

        public void BackUp()
        {
            var backup = new Backup.BackupManager();
            backup.MakeBackup();
        }

        public void PrinVisual()
        {
            var print = new CardGenerator.CardGenerator();
            print.PrintVisual();
        }

        public void TestCards()
        {
            var checker = new Checker.DoorChecker();
            var theSame = checker.FindRepeated();

            if (theSame.Any())
                throw new Exception();
        }
    }
}
