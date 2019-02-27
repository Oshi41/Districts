using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.Helper;
using Districts.New.Interfaces;

namespace Districts.New.Implementation
{
    class Card : iCard
    {
        public Card(int number, IList<iDoor> doors)
        {
            Number = number;
            Doors = doors;
        }

        public int Number { get; }
        public IList<iDoor> Doors { get; }

        protected bool Equals(Card other)
        {
            return Number == other.Number 
                   && Doors.IsTermwiseEquals(other.Doors);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Card) obj);
        }

        public override int GetHashCode()
        {
            return Number;
        }
    }
}
