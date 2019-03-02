using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Districts.Helper;
using Districts.New.Implementation;
using Districts.New.Implementation.Classes;
using Districts.New.Interfaces;
using Newtonsoft.Json;

namespace Districts.Parser.v2
{
    class Parser : IParser
    {
        private readonly IAppSettings _settings;

        public Parser(IAppSettings settings)
        {
            _settings = settings;
        }

        public void UpdateRelationships()
        {
            var infos = LoadHomeInfos();
            var homes = LoadHomes();

            foreach (var info in infos)
            {
                var home = homes.FirstOrDefault(x => x.SameObject(info));
                if (home == null)
                    continue;

                homes.Remove(home);

                var doors = home
                    .Doors
                    .Select(x =>
                    {
                        var number = -1 + x.DoorNumber + info.FirstDoor;
                        info.Codes.TryGetValue(number, out var codes);
                        return (iDoor)new Door(x, number, x.Entrance, x.Status, codes);
                    });

                home = new Home(home.Street, 
                    home.HomeNumber, 
                    home.Housing,
                    home.AfterSlash,
                    doors.ToList());

                homes.Add(home);
            }


            SaveHomes(homes);
        }

        #region Save

        /// <summary>
        /// Сохраняю улицы. Хранятся как json array 
        /// </summary>
        /// <param name="streets"></param>
        public void SaveStreets(IList<string> streets)
        {
            File.WriteAllText(_settings.StreetsPath, JsonConvert.SerializeObject(streets, Formatting.Indented));
        }

        /// <summary>
        /// Сохраняю историю
        /// </summary>
        /// <param name="history"></param>
        public void SaveManagements(IList<Manage> history)
        {
            Save(history, _settings.ManagementPath, m => $"{m.Card.Number}.json");
        }

        public void SaveHomesInfo(IList<iHomeInfo> infos)
        {
            SaveFolders(infos.GroupBy(x => x.Street), _settings.CommonHomeInfoPath, i => $"{i}.json");

            UpdateRelationships();
        }

        public void SaveCards(IList<Card> cards)
        {
            Save(cards, _settings.CardsPath, c => $"{c.Number}.json");
        }

        #endregion

        #region Load
        /// <summary>
        /// Загруждаю список домов
        /// Хранится по улицам!
        /// </summary>
        /// <returns></returns>
        public IList<iHome> LoadHomes()
        {
            return LoadFolders<Home>(_settings.HomesPath)
                .Cast<iHome>()
                .ToList();
        }

        public void SaveHomes(IList<iHome> homes)
        {
            SaveFolders(homes.GroupBy(x => x.Street), _settings.HomesPath, h => $"{h}.json");
        }

        /// <summary>
        /// Загружаю улицы. Хранятся как json array
        /// </summary>
        /// <returns></returns>
        public IList<string> LoadStreets()
        {
            return JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_settings.StreetsPath)) 
                   ?? new List<string>();
        }

        /// <summary>
        /// Загружаю список карт
        /// </summary>
        /// <returns></returns>
        public IList<iCard> LoadCards()
        {
            return Load<Card>(_settings.CardsPath)
                .Cast<iCard>()
                .ToList();
        }

        /// <summary>
        /// Сохраняю карты
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public iCard LoadCard(int number)
        {
            var path = Path.Combine(_settings.CardsPath, $"{number}.json");

            if (File.Exists(path))
                return JsonConvert.DeserializeObject<Card>(File.ReadAllText(path));

            throw new Exception("Can't find number of card");
        }

        /// <summary>
        /// Загружаю исторю управления карточками
        /// </summary>
        /// <returns></returns>
        public IList<iManage> LoadManagement()
        {
            return Load<Manage>(_settings.ManagementPath)
                .Cast<iManage>()
                .ToList();
        }

        public IList<iHomeInfo> LoadHomeInfos()
        {
            return LoadFolders<Home>(_settings.CommonHomeInfoPath)
                .Cast<iHomeInfo>()
                .ToList();
        }

        #endregion

        #region private

        private IEnumerable<T> LoadFolders<T>(string folder)
        {
            var folders = Directory.GetDirectories(folder);

            return folders
                .SelectMany(Load<T>)
                .ToList();
        }

        private void SaveFolders<T>(IEnumerable<IGrouping<string, T>> items, string folder, Func<T, string> fileName)
        {
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);

            Directory.CreateDirectory(folder);

            foreach (var item in items)
            {
                Save(item.GetEnumerator().ToList<T>(), Path.Combine(folder, item.Key), fileName);
            }
        }


        private IEnumerable<T> Load<T>(string folder)
        {
            var files = Directory.GetFiles(folder);

            return files
                .Select(x => JsonConvert
                    .DeserializeObject<T>(File.ReadAllText(Path.Combine(folder, x))));
        }

        private void Save<T>(IList<T> items, string folder, Func<T, string> fileName)
        {
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);

            Directory.CreateDirectory(folder);

            foreach (var item in items)
            {
                var path = Path.Combine(folder, fileName(item));
                File.WriteAllText(path, JsonConvert.SerializeObject(item, Formatting.Indented));
            }
        }

        #endregion
    }
}
