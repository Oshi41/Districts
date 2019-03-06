using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.JsonClasses.Manage;
using Districts.Settings;
using Newtonsoft.Json;

namespace Districts.Parser
{
    public class Parser : IParser
    {
        #region Implementation of IParser

        public List<CardManagement> LoadManage()
        {
            return LoadFiles<CardManagement>(ApplicationSettings.ReadOrCreate().ManageRecordsPath);
        }

        public void SaveManage(List<CardManagement> manage)
        {
            SaveFiles(ApplicationSettings.ReadOrCreate().ManageRecordsPath,
                manage.Select(x => new KeyValuePair<string, CardManagement>($"{x.Number}.json", x))
                    .ToList());
        }

        public List<Card> LoadCards()
        {
            return LoadFiles<Card>(ApplicationSettings.ReadOrCreate().CardsPath);
        }

        public void SaveCards(List<Card> cards)
        {
            SaveFiles(ApplicationSettings.ReadOrCreate().CardsPath, 
                cards
                    .Select(x => new KeyValuePair<string, Card>($"{x.Number}.json", x))
                    .ToList());
        }

        public List<ForbiddenElement> LoadRules()
        {
            return LoadFiles<List<ForbiddenElement>>(ApplicationSettings.ReadOrCreate().RestrictionsPath)
                .SelectMany(x => x)
                .ToList();
        }

        public void SaveRules(List<ForbiddenElement> rules)
        {
            SaveFiles(ApplicationSettings.ReadOrCreate().RestrictionsPath, 
                rules.GroupBy(x => $"{x.Street}.json"));
        }

        public List<HomeInfo> LoadCodes()
        {
            return LoadFiles<List<HomeInfo>>(ApplicationSettings.ReadOrCreate().HomeInfoPath)
                .SelectMany(x => x)
                .ToList();
        }

        public void SaveCodes(List<HomeInfo> codes)
        {
            SaveFiles(ApplicationSettings.ReadOrCreate().HomeInfoPath, 
                codes.GroupBy(x => $"{x.Street}.json"));
        }

        #endregion

        #region private

        private List<T> LoadFiles<T>(string folder)
        {
            return Directory
                .GetFiles(folder)
                .Select(Load<T>)
                .ToList();
        }

        private T Load<T>(string file)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
        }

        private void SaveFiles<T>(string folder, IList<KeyValuePair<string, T>> grouped)
        {
            if (!grouped.Any())
                return;

            if (Directory.Exists(folder))
                Directory.Delete(folder, true);

            Directory.CreateDirectory(folder);

            foreach (var file in grouped)
            {
                Save(Path.Combine(folder, file.Key), file.Value);
            }
        }

        private void SaveFiles<T>(string folder, IEnumerable<IGrouping<string, T>> grouped)
        {
            SaveFiles(folder, grouped
                .ToDictionary(x => x.Key,
                    x => x.GetEnumerator().ToList())
                .ToList());
        }

        private void Save(string file, object value)
        {
            if (!Directory.Exists(Path.GetDirectoryName(file)))
                Directory.CreateDirectory(Path.GetDirectoryName(file));

            if (!File.Exists(file))
            {
                File.Create(file).Close();
            }

            File.WriteAllText(file, JsonConvert.SerializeObject(value, Formatting.Indented));
        }

        #endregion
    }
}
