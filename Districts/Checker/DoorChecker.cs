using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.JsonClasses;

namespace Districts.Checker
{
    class DoorComparer : IEqualityComparer<Door>
    {
        public bool Equals(Door x, Door y)
        {
            return x.IsTheSameObject(y) && y.Number == x.Number;
        }

        public int GetHashCode(Door obj)
        {
            return obj.Street.GetHashCode() ^ obj.HouseNumber.GetHashCode() ^ obj.Number.GetHashCode();
        }
    }
    class DoorChecker
    {

        public List<Door> FindRepeated()
        {
            var doors = Helper.Helper.LoadCards().SelectMany(x => x.Doors).ToList();

            var distinct = doors.Distinct(new DoorComparer());

            return doors.Except(distinct).ToList();
        }
    }
}
