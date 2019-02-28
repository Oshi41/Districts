using System.Collections.Generic;
using Districts.Helper;
using Districts.New.Interfaces;

namespace Districts.New.Implementation
{
    class Door : iDoor
    {
        private iFind _inner;
        public string Street => _inner.Street;

        public int HomeNumber => _inner.HomeNumber;

        public int Housing => _inner.Housing;

        public int AfterSlash => _inner.AfterSlash;

        public bool SameObject(iFind obj, ReturnConditions conditions = ReturnConditions.WithSlash)
        {
            return _inner.SameObject(obj, conditions);
        }

        public Door(iFind find, int doorNumber, int entrance, DoorStatus status, IList<iCode> codes)
        {
            _inner = find;
            DoorNumber = doorNumber;
            Entrance = entrance;
            Status = status;
            Codes = codes;
        }

        public int DoorNumber { get; }
        public int Entrance { get; }
        public DoorStatus Status { get; }
        public IList<iCode> Codes { get; }

        protected bool Equals(Door other)
        {
            return Equals(_inner, other._inner) 
                   && DoorNumber == other.DoorNumber 
                   && Entrance == other.Entrance &&
                   Status == other.Status 
                   && Codes.IsTermwiseEquals(other.Codes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Door) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_inner != null ? _inner.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ DoorNumber;
                hashCode = (hashCode * 397) ^ Entrance;
                hashCode = (hashCode * 397) ^ (int) Status;
                return hashCode;
            }
        }
    }

//    private iFind _inner;
//    public string Street => _inner.Street;

//    public int HomeNumber => _inner.HomeNumber;

//    public int Housing => _inner.Housing;

//    public int AfterSlash => _inner.AfterSlash;

//    public bool SameObject(iFind obj, ReturnConditions conditions = ReturnConditions.WithSlash)
//    {
//    return _inner.SameObject(obj, conditions);
//}

//public Door(iFind find)
//{
//_inner = find;
//}
}
