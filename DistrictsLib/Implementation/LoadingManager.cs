using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ClearFolder(_managementFolder);

            foreach (var manage in manages)
            {
                var name = Path.Combine(_managementFolder,$"{manage.Number}.json");
                var json = JsonConvert.SerializeObject(manage);

                File.WriteAllText(name, json);
            }
        }

        public void SaveCards(List<Card> cards)
        {
            ClearFolder(_cardsFolder);

            foreach (var card in cards)
            {
                var name = Path.Combine(_cardsFolder, $"{card.Number}.json");
                var json = JsonConvert.SerializeObject(card);

                File.WriteAllText(name, json);
            }
        }

        public void SaveRules(List<ForbiddenElement> rules)
        {
            throw new NotImplementedException();
        }

        public void SaveCodes(List<HomeInfo> codes)
        {
            throw new NotImplementedException();
        }

        public void SaveSortedHomes(List<Building> homes)
        {
            throw new NotImplementedException();
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
            }

            Directory.CreateDirectory(folder);
        }
    }
}
