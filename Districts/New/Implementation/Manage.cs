using System.Collections.Generic;
using Districts.Helper;
using Districts.New.Interfaces;

namespace Districts.New.Implementation
{
    class Manage : iManage
    {
        public Manage(IList<iRecord> records, iCard card)
        {
            Records = records;
            Card = card;
        }

        public iCard Card { get; }
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
