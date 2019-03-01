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

        #region Save

        /// <summary>
        /// Сохраняю список домов
        /// Хранится по улицам! 
        /// </summary>
        /// <param name="homes"></param>
        public void SaveHomes(IList<Home> homes)
        {
            Directory.Delete(_settings.HomesPath, true);

            Directory.CreateDirectory(_settings.HomesPath);

            var byStreets = homes.GroupBy(x => x.Street);

            foreach (var street in byStreets)
            {
                Save(street.GetEnumerator().ToList(), street.Key, h => $"{h.GetPrettyHouseName()}.json");
            }
        }

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
            // Вытаскиваю из всех директорий
            return Directory
                .GetDirectories(_settings.HomesPath)
                .SelectMany(x => Load<Home>(x)
                    .Cast<iHome>())
                .ToList();
        }

        /// <summary>
        /// Загружаю улицы. Хранятся как json array
        /// </summary>
        /// <returns></returns>
        public IList<string> LoadStreets()
        {
            return JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_settings.StreetsPath));
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

        #endregion

        #region private

        private IEnumerable<T> Load<T>(string folder)
        {
            var files = Directory.GetFiles(folder);

            return files
                .Select(x => JsonConvert
                    .DeserializeObject<T>(File.ReadAllText(x)));
        }

        private void Save<T>(IList<T> items, string folder, Func<T, string> fileName)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            foreach (var item in items)
            {
                File.WriteAllText(fileName(item), JsonConvert.SerializeObject(item, Formatting.Indented));
            }
        }

        #endregion
    }
}
