using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DistrictsLib.Extentions;
using DistrictsLib.Interfaces;
using DistrictsLib.Interfaces.Json;
using DistrictsLib.Json;
using DistrictsLib.Legacy.Comparers;
using DistrictsLib.Legacy.JsonClasses;
using DistrictsLib.Legacy.JsonClasses.Manage;
using Newtonsoft.Json;

namespace DistrictsLib.Implementation
{
    public class LoadingManager : IParser, ISerializer
    {
        private readonly string _homesPath;
        private readonly string _infosFolder;
        private readonly string _forbiddenFolder;
        private readonly string _cardsFolder;
        private readonly string _managementFolder;

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new InterfaceContractResolver()
        };

        public LoadingManager(string homesPath,
            string infosFolder,
            string forbiddenFolder,
            string cardsFolder,
            string managementFolder)
        {
            _homesPath = homesPath;
            _infosFolder = infosFolder;
            _forbiddenFolder = forbiddenFolder;
            _cardsFolder = cardsFolder;
            _managementFolder = managementFolder;
        }

        #region Implementation of IParser

        public List<ICardManagement> LoadManage()
        {
            return ReadFromFolderFiles<CardManagement>(_managementFolder)
                .OfType<ICardManagement>()
                .ToList();
        }

        public List<Card> LoadCards()
        {
            return ReadFromFolderFiles<Card>(_cardsFolder);
        }

        public List<ForbiddenElement> LoadRules()
        {
            var result = ReadFromFolderFiles<List<ForbiddenElement>>(_forbiddenFolder)
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

        public void SaveManage(IReadOnlyCollection<ICardManagement> manages)
        {
            SaveInFolder(_managementFolder, manages, m => $"{m.Number}.json");
        }

        public void SaveCards(List<Card> cards)
        {
            SaveInFolder(_cardsFolder, cards, c => $"{c.Number}.json");
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
                var json = JsonConvert.SerializeObject(item, _settings);
                Trace.WriteLine($"Saving {name}");

                File.WriteAllText(name, json);
            }
        }
    }
}
