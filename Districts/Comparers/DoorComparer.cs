using System.Collections.Generic;
using Districts.JsonClasses;

namespace Districts.Comparers
{
    /// <summary>
    /// Компаратор для сравнения дверей
    /// </summary>
    public class DoorComparer : IEqualityComparer<Door>
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
}
