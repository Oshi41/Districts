using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Districts.Helper
{
    /// <summary>
    /// Generic clone
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface ICloneable<T>
    {
        T Clone();
    }


    /// <summary>
    /// Вспомогательные методы
    /// </summary>
    public static class Helper
    {
        #region List work

        /// <summary>
        /// Перечислитель в простую коллекцию
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        /// <summary>
        /// Перечислитель в список
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static IList<T> ToList<T>(this IEnumerator<T> enumerator)
        {
            var list = new List<T>();

            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }

            return list;
        }
        /// <summary>
        /// Записывает в список все элементы этого типа
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static IList<T> ToList<T>(this IEnumerator enumerator)
        {
            var list = new List<T>();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current is T temp)
                    list.Add(temp);
            }

            return list;
        }
        /// <summary>
        /// Проверяет на пустой список
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerator)
        {
            return enumerator == null || !enumerator.Any();
        }

        #endregion

        #region Loading work

        /// <summary>
        /// Загружаю дома по названиям файла. Сортированы!
        /// </summary>
        /// <returns></returns>
        //[Obsolete("Исползуй LoadinWork.LoadSortedHomes.")]
        //public static Dictionary<string, List<Building>> LoadHomes()
        //{
        //    var settings = ApplicationSettings.ReadOrCreate();
        //    var result = new Dictionary<string, List<Building>>();

        //    var logger = new Logger();
        //    // загрузил дома
        //    var streets = Directory.GetFiles(settings.BuildingPath);
        //    foreach (var street in streets)
        //    {
        //        var json = File.ReadAllText(street);
        //        try
        //        {
        //            var temp = JsonConvert.DeserializeObject<List<Building>>(json);
        //            temp.Sort(new HouseNumberComparer());
        //            result.Add(Path.GetFileName(street), temp);

        //        }
        //        catch (Exception e)
        //        {
        //            logger.AddMessage(e);
        //        }
        //    }
        //    logger.WriteToFile();
        //    return result;
        //}
        ///// <summary>
        ///// Загружаю правила по названию фала
        ///// </summary>
        ///// <returns></returns>
        //[Obsolete("Используй LoadinWork.")]
        //public static Dictionary<string, List<ForbiddenElement>> LoadRules()
        //{
        //    var settings = ApplicationSettings.ReadOrCreate();
        //    var result = new Dictionary<string, List<ForbiddenElement>>();

        //    var logger = new Logger();
        //    // загрузил запретные правилп
        //    var streets = Directory.GetFiles(settings.RestrictionsPath);
        //    foreach (var street in streets)
        //    {
        //        var json = File.ReadAllText(street);
        //        try
        //        {
        //            var temp = JsonConvert.DeserializeObject<List<ForbiddenElement>>(json);
        //            result.Add(Path.GetFileName(street), temp);
        //        }
        //        catch (Exception e)
        //        {
        //            logger.AddMessage(e);
        //        }
        //    }

        //    logger.WriteToFile();
        //    return result;
        //}
        //[Obsolete("Используй LoadinWork.")]
        //public static Dictionary<string, List<Codes>> LoadCodes()
        //{
        //    var settings = ApplicationSettings.ReadOrCreate();
        //    var result = new Dictionary<string, List<Codes>>();

        //    var logger = new Logger();
        //    // загузил коды
        //    var streets = Directory.GetFiles(settings.CodesPath);
        //    foreach (var street in streets)
        //    {
        //        var json = File.ReadAllText(street);
        //        try
        //        {
        //            var temp = JsonConvert.DeserializeObject<List<Codes>>(json);
        //            result.Add(Path.GetFileName(street), temp);
        //        }
        //        catch (Exception e)
        //        {
        //            logger.AddMessage(e);
        //        }

        //    }

        //    logger.WriteToFile();

        //    return result;
        //}
        ///// <summary>
        ///// Щагружаю карточки
        ///// </summary>
        ///// <returns></returns>
        //[Obsolete("Используй LoadinWork.")]
        //public static List<Card> LoadCards()
        //{
        //    var settings = ApplicationSettings.ReadOrCreate();
        //    var result = new List<Card>();

        //    var logger = new Logger();
        //    // загузил карточки
        //    var cards = Directory.GetFiles(settings.CardsPath);
        //    foreach (var card in cards)
        //    {
        //        var json = File.ReadAllText(card);
        //        try
        //        {
        //            var temp = JsonConvert.DeserializeObject<Card>(json);
        //            result.Add(temp);
        //        }
        //        catch (Exception e)
        //        {
        //            logger.AddMessage(e);
        //        }
        //    }
        //    logger.WriteToFile();

        //    return result;
        //}
        ///// <summary>
        ///// Щагружаю записи о карточках
        ///// </summary>
        ///// <returns></returns>
        //[Obsolete("Используй LoadinWork.")]
        //public static List<CardManagement> LoadCardManagements()
        //{
        //    var settings = ApplicationSettings.ReadOrCreate();
        //    var result = new List<CardManagement>();

        //    var logger = new Logger();
        //    // загузил карточки
        //    var cards = Directory.GetFiles(settings.CardsPath);
        //    foreach (var card in cards)
        //    {
        //        var json = File.ReadAllText(card);
        //        try
        //        {
        //            var temp = JsonConvert.DeserializeObject<CardManagement>(json);
        //            result.Add(temp);
        //        }
        //        catch (Exception e)
        //        {
        //            logger.AddMessage(e);
        //        }
        //    }
        //    logger.WriteToFile();

        //    return result;
        //}

        #endregion

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
        /// <summary>
        /// Очищаю папку
        /// </summary>
        /// <param name="path"></param>
        public static void ClearFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return;
            }

            foreach (var file in Directory.GetFiles(path))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    Tracer.WriteError(e, "Can't delete " + file);
                }
            }
        }
    }
}
