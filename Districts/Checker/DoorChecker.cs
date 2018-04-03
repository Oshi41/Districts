using System.Collections.Generic;
using System.Linq;
using Districts.Comparers;
using Districts.Helper;
using Districts.JsonClasses;

namespace Districts.Checker
{
    /// <summary>
    ///     Класс для проверки созданных карточек на повторения квартир
    /// </summary>
    internal class DoorChecker
    {
        /// <summary>
        ///     Возвращает список повторяющихся дверей
        /// </summary>
        /// <returns></returns>
        public List<Door> FindRepeated()
        {
            var doors = LoadingWork.LoadCards().SelectMany(x => x.Value.Doors).ToList();

            var distinct = doors.Distinct(new DoorComparer());

            return doors.Except(distinct).ToList();
        }
    }
}