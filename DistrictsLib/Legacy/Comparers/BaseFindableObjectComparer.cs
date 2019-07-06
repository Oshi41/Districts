using System.Collections.Generic;
using DistrictsLib.Legacy.JsonClasses.Base;

namespace DistrictsLib.Legacy.Comparers
{
    internal class BaseFindableObjectComparer : IEqualityComparer<BaseFindableObject>
    {
        public bool Equals(BaseFindableObject x, BaseFindableObject y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(null, y)) return false;

            return x.IsTheSameObject(y);
        }

        public int GetHashCode(BaseFindableObject obj)
        {
            return obj.HouseNumber.GetHashCode() ^ obj.Street.GetHashCode();
        }
    }
}