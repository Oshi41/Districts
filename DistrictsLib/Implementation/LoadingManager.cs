using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DistrictsLib.Extentions;
using DistrictsLib.Interfaces;
using DistrictsLib.Legacy.Comparers;
using DistrictsLib.Legacy.JsonClasses;
using DistrictsLib.Legacy.JsonClasses.Manage;
using Newtonsoft.Json;

namespace DistrictsLib.Implementation
{
    class LoadingManager : IParser, ISerializer
    {
        private readonly string _homesPath;
        private readonly string _rulesPath;
        private readonly string _infosFolder;
        private readonly string _forbiddenFolder;
        private readonly string _cardsFolder;
        private readonly string _managementFolder;

        public LoadingManager(string homesPath,
            string rulesPath,
            string infosFolder,
            string forbiddenFolder,
            string cardsFolder,
            string managementFolder)
        {
            _homesPath = homesPath;
            _rulesPath = rulesPath;
            _infosFolder = infosFolder;
            _forbiddenFolder = forbiddenFolder;
            _cardsFolder = cardsFolder;
            _managementFolder = managementFolder;
        }

        #region Implementation of IParser

        public List<CardManagement> LoadManage()
        {
            return ReadFromFolderFiles<CardManagement>(_managementFolder);
        }

        public List<Card> LoadCards()
        {
            return ReadFromFolderFiles<Card>(_cardsFolder);
        }

        public List<ForbiddenElement> LoadRules()
        {
            var result = ReadFromFolderFiles<List<ForbiddenElement>>(_rulesPath)
                .SelectMany(x => x)
                .ToList();

            return result;
        }

        public List<HomeInfo> LoadCodes()
        {
            var result = ReadFromFolderFiles<List<HomeInfo>>(_infosFolder)
                .SelectMany(x => x)
                .ToList();

            return result;
        }

        public List<Building> LoadSortedHomes()
        {
            var result = ReadFromFolderFiles<List<Building>>(_homesPath)
                .SelectMany(x => x)
                .ToList();

            result.Sort(new HouseNumberComparer());

            return result;
        }

        #endregion

        #region Implementation of ISerializer

        public void SaveManage(List<CardManagement> manages)
        {
            SaveInFolder(_managementFolder, manages, m => $"{m.Number}.json");
        }

        public void SaveCards(List<Card> cards)
        {
            SaveInFolder(_managementFolder, cards, c => $"{c.Number}.json");
        }

        public void SaveRules(List<ForbiddenElement> rules)
        {
            var groupped = rules
                .GroupBy(x => x.Street)
                .Select(x => x.GetEnumerator().ToIEnumerable());

            SaveInFolder(_forbiddenFolder, groupped, elements => $"{elements.FirstOrDefault()?.Street}.json");
        }

        public void SaveCodes(List<HomeInfo> codes)
        {
            var groupped = codes
                .GroupBy(x => x.Street)
                .Select(x => x.GetEnumerator().ToIEnumerable());

            SaveInFolder(_infosFolder, groupped, elements => $"{elements.FirstOrDefault()?.Street}.json");
        }

        public void SaveSortedHomes(List<Building> homes)
        {
            var groupped = homes
                .GroupBy(x => x.Street)
                .Select(x =>
                {
                    var result = x.GetEnumerator().ToIEnumerable().ToList();
                    result.Sort(new HouseNumberComparer());
                    return result;
                });

            SaveInFolder(_homesPath, groupped, list => $"{homes.FirstOrDefault()?.Street}.json");
        }

        #endregion

        private List<T> ReadFromFolderFiles<T>(string folder)
        {
            if (!Directory.Exists(folder))
            {
                throw new FileNotFoundException($"Cannot find folder by path {folder}");
            }

            return Directory
                .GetFiles(folder)
                .Select(x => JsonConvert.DeserializeObject<T>(File.ReadAllText(x)))
                .ToList();
        }

        private void ClearFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
                Trace.WriteLine($"Folder deleted {folder}");
            }

            Directory.CreateDirectory(folder);
        }

        private void SaveInFolder<T>(string folder, IEnumerable<T> source, Func<T, string> getName)
        {
            ClearFolder(folder);

            foreach (var item in source)
            {
                var name = Path.Combine(folder, getName(item));
                var json = JsonConvert.SerializeObject(item);
                Trace.WriteLine($"Saving {name}");

                File.WriteAllText(name, json);
            }
        }
    }
}
