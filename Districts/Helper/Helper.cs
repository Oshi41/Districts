using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Districts.JsonClasses;
using Districts.JsonClasses.Manage;
using Districts.Settings;
using Newtonsoft.Json;

namespace Districts.Helper
{
    public static class Helper
    {
        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        /// <summary>
        /// Загружаю дома по названиям файла. Сортированы!
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<Building>> LoadHomes()
        {
            var settings = ApplicationSettings.ReadOrCreate();
            var result = new Dictionary<string, List<Building>>();

            var logger = new Logger();
            // загрузил дома
            var streets = Directory.GetFiles(settings.BuildingPath);
            foreach (var street in streets)
            {
                var json = File.ReadAllText(street);
                try
                {
                    var temp = JsonConvert.DeserializeObject<List<Building>>(json);
                    temp.Sort(new HouseNumberComparer());
                    result.Add(Path.GetFileName(street), temp);

                }
                catch (Exception e)
                {
                    logger.AddMessage(e);
                }
            }
            logger.WriteToFile();
            return result;
        }
        /// <summary>
        /// Загружаю правила по названию фала
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<ForbiddenElement>> LoadRules()
        {
            var settings = ApplicationSettings.ReadOrCreate();
            var result = new Dictionary<string, List<ForbiddenElement>>();

            var logger = new Logger();
            // загрузил запретные правилп
            var streets = Directory.GetFiles(settings.RestrictionsPath);
            foreach (var street in streets)
            {
                var json = File.ReadAllText(street);
                try
                {
                    var temp = JsonConvert.DeserializeObject<List<ForbiddenElement>>(json);
                    result.Add(Path.GetFileName(street), temp);
                }
                catch (Exception e)
                {
                    logger.AddMessage(e);
                }
            }

            logger.WriteToFile();
            return result;
        }
        public static Dictionary<string, List<Codes>> LoadCodes()
        {
            var settings = ApplicationSettings.ReadOrCreate();
            var result = new Dictionary<string, List<Codes>>();

            var logger = new Logger();
            // загузил коды
            var streets = Directory.GetFiles(settings.CodesPath);
            foreach (var street in streets)
            {
                var json = File.ReadAllText(street);
                try
                {
                    var temp = JsonConvert.DeserializeObject<List<Codes>>(json);
                    result.Add(Path.GetFileName(street), temp);
                }
                catch (Exception e)
                {
                    logger.AddMessage(e);
                }

            }

            logger.WriteToFile();

            return result;
        }
        /// <summary>
        /// Щагружаю карточки
        /// </summary>
        /// <returns></returns>
        public static List<Card> LoadCards()
        {
            var settings = ApplicationSettings.ReadOrCreate();
            var result = new List<Card>();

            var logger = new Logger();
            // загузил карточки
            var cards = Directory.GetFiles(settings.CardsPath);
            foreach (var card in cards)
            {
                var json = File.ReadAllText(card);
                try
                {
                    var temp = JsonConvert.DeserializeObject<Card>(json);
                    result.Add(temp);
                }
                catch (Exception e)
                {
                    logger.AddMessage(e);
                }
            }
            logger.WriteToFile();

            return result;
        }

        /// <summary>
        /// Щагружаю записи о карточках
        /// </summary>
        /// <returns></returns>
        public static List<CardManagement> LoadCardManagements()
        {
            var settings = ApplicationSettings.ReadOrCreate();
            var result = new List<CardManagement>();

            var logger = new Logger();
            // загузил карточки
            var cards = Directory.GetFiles(settings.CardsPath);
            foreach (var card in cards)
            {
                var json = File.ReadAllText(card);
                try
                {
                    var temp = JsonConvert.DeserializeObject<CardManagement>(json);
                    result.Add(temp);
                }
                catch (Exception e)
                {
                    logger.AddMessage(e);
                }
            }
            logger.WriteToFile();

            return result;
        }
        /// <summary>
        /// Удаляю пустые строки
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveEmptyLines(this string text)
        {
            var result = Regex.Replace(text, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            return result;
        }

        public static void ClearFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return;
            }

            var logger = new Logger();
            foreach (var file in Directory.GetFiles(path))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    logger.AddMessage("Can't delete " + file, e);
                }
            }

            logger.WriteToFile();
        }
    }
}
