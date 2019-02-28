using System.Collections.Generic;
using Districts.Helper;
using Districts.New.Interfaces;

namespace Districts.New.Implementation
{
    class Home : iHome
    {
        private iFind _inner;

        public Home(iFind inner, IList<iDoor> doors)
        {
            _inner = inner;
            Doors = doors;
        }

        public string Street => _inner.Street;

        public int HomeNumber => _inner.HomeNumber;

        public int Housing => _inner.Housing;

        public int AfterSlash => _inner.AfterSlash;

        public bool SameObject(iFind obj, ReturnConditions conditions = ReturnConditions.WithSlash)
        {
            return _inner.SameObject(obj, conditions);
        }

        public IList<iDoor> Doors { get; }

        protected bool Equals(Home other)
        {
            return Equals(_inner, other._inner) 
                   && Doors.IsTermwiseEquals(other.Doors);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Home) obj);
        }

        public override int GetHashCode()
        {
            return _inner.GetHashCode();
        }
    }
}
