using System.Collections.Generic;
using Districts.Helper;
using Districts.New.Interfaces;
using Districts.Parser.v2.Converters;
using Newtonsoft.Json;

namespace Districts.New.Implementation.Classes
{
    class Card : iCard
    {
        [JsonConstructor]
        public Card(int number, IList<iDoor> doors)
        {
            Number = number;
            Doors = doors;
        }

        public int Number { get; }

        [JsonConverter(typeof(ListConverter<Door, iDoor>))]
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
