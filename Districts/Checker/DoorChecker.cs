using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.Helper;
using Districts.JsonClasses;

namespace Districts.Checker
{
    /// <summary>
    /// Компаратор для сравнения дверей
    /// </summary>
    class DoorComparer : IEqualityComparer<Door>
    {
        public bool Equals(Door x, Door y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.IsTheSameObject(y) && y.Number == x.Number;
        }

        public int GetHashCode(Door obj)
        {
            return obj.Street.GetHashCode() ^ obj.HouseNumber.GetHashCode() ^ obj.Number.GetHashCode();
        }
    }

    /// <summary>
    /// Класс для проверки созданных карточек на повторения квартир
    /// </summary>
    class DoorChecker
    {
        /// <summary>
        /// Возвращает список повторяющихся дверей
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
