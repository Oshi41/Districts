using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.JsonClasses;
using Districts.JsonClasses.Manage;
using Districts.Settings;
using Newtonsoft.Json;

namespace Districts.Helper
{
    public static class LoadingWork
    {
        /// <summary>
        /// Загружаю все файлы из папки и сериализую их из json формата
        /// </summary>
        /// <typeparam name="T">Тип элементов</typeparam>
        /// <param name="folder">Путь к папке</param>
        /// <returns></returns>
        private static Dictionary<string, T> LoadSmth<T>(string folder)
        {
            var result = new Dictionary<string, T>();

            if (!Directory.Exists(folder))
            {
                Tracer.WriteError(new FileNotFoundException("Не найдена папка " + folder));
                return new Dictionary<string, T>();
            }

            var fileNames = Directory.GetFiles(folder);
            foreach (var fileName in fileNames)
            {
                var fileContent = File.ReadAllText(fileName);
                try
                {
                    var temp = JsonConvert.DeserializeObject<T>(fileContent);
                    result.Add(Path.GetFileName(fileName), temp);
                }
                catch (Exception e)
                {
                    Tracer.WriteError(e);
                }
            }

            return result;
        }


        public static Dictionary<string, List<Building>> LoadSortedHomes()
        {
            var result = LoadSmth<List<Building>>(ApplicationSettings.ReadOrCreate().BuildingPath);
            foreach (var keyValuePair in result)
            {
                keyValuePair.Value.Sort(new HouseNumberComparer());
            }

            return result;
        }
        public static Dictionary<string, List<ForbiddenElement>> LoadRules()
        {
            return LoadSmth<List<ForbiddenElement>>(ApplicationSettings.ReadOrCreate().RestrictionsPath);
        }
        public static Dictionary<string, List<Codes>> LoadCodes()
        {
            return LoadSmth<List<Codes>>(ApplicationSettings.ReadOrCreate().CodesPath);
        }
        public static Dictionary<string, Card> LoadCards()
        {
            return LoadSmth<Card>(ApplicationSettings.ReadOrCreate().CardsPath);
        }
        public static Dictionary<string, CardManagement> LoadManageElements()
        {
            return LoadSmth<CardManagement>(ApplicationSettings.ReadOrCreate().ManageRecordsPath);
        }
    }
}
