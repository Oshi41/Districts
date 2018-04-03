using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Districts.Helper
{
    /// <summary>
    ///     Generic clone
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface ICloneable<T>
    {
        T Clone();
    }


    /// <summary>
    ///     Вспомогательные методы
    /// </summary>
    public static class Helper
    {
        /// <summary>
        ///     Загружаю дома по названиям файла. Сортированы!
        /// </summary>
        /// <returns></returns>
        /// <summary>
        ///     Удаляю пустые строки
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveEmptyLines(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var result = Regex.Replace(text, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            return result;
        }

        /// <summary>
        ///     Очищаю папку
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
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    Tracer.WriteError(e, "Can't delete " + file);
                }
        }

        #region List work

        /// <summary>
        ///     Перечислитель в простую коллекцию
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }

        /// <summary>
        ///     Перечислитель в список
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static IList<T> ToList<T>(this IEnumerator<T> enumerator)
        {
            var list = new List<T>();

            while (enumerator.MoveNext()) list.Add(enumerator.Current);

            return list;
        }

        /// <summary>
        ///     Записывает в список все элементы этого типа
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static IList<T> ToList<T>(this IEnumerator enumerator)
        {
            var list = new List<T>();

            while (enumerator.MoveNext())
                if (enumerator.Current is T temp)
                    list.Add(temp);

            return list;
        }

        /// <summary>
        ///     Проверяет на пустой список
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerator)
        {
            return enumerator == null || !enumerator.Any();
        }

        /// <summary>
        /// Случайным образом перемешиваю элементы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> elements)
        {
            //
            // нашел на StackOverFlow отличное решение случайного перемешивания списка
            // https://stackoverflow.com/questions/273313/randomize-a-listt
            //

            var temp = elements.OrderBy(x => Guid.NewGuid());
            return temp;
        }

        #endregion
    }
}