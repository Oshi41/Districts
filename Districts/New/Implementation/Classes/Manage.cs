using System.Collections.Generic;
using Districts.Helper;
using Districts.New.Interfaces;
using Districts.Parser.v2.Converters;
using Newtonsoft.Json;

namespace Districts.New.Implementation.Classes
{
    class Manage : iManage
    {
        [JsonConstructor]
        public Manage(IList<iRecord> records, iCard card)
        {
            Records = records;
            Card = card;
        }

        [JsonConverter(typeof(CardConverter))]
        public iCard Card { get; }

        [JsonConverter(typeof(ListConverter<Record, iRecord>))]
        public IList<iRecord> Records { get; }

        protected bool Equals(Manage other)
        {
            return Equals(Card, other.Card) 
                   && Records.IsTermwiseEquals(other.Records);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Manage) obj);
        }

        public override int GetHashCode()
        {
            return Card.GetHashCode();
        }
    }
}
