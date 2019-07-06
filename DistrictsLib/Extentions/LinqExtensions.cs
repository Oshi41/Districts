using System;
using System.Collections.Generic;
using System.Linq;

namespace DistrictsLib.Extentions
{
    public static class LinqExtensions
    {
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

        public static bool IsTermwiseEquals<T>(this IEnumerable<T> source, IEnumerable<T> list, IEqualityComparer<T> comparer = null)
        {
            if (source.IsNullOrEmpty() && list.IsNullOrEmpty())
                return true;

            if (source.IsNullOrEmpty() || list.IsNullOrEmpty())
                return false;

            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            if (source.Count() != list.Count())
                return false;

            var except = source.Except(list, comparer);

            return !except.Any();
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
    }
}
