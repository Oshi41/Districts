using System;
using Districts.New.Interfaces;
using Newtonsoft.Json;

namespace Districts.New.Implementation.Classes
{
    class Record : iRecord
    {
        [JsonConstructor]
        public Record(ActionType action, string subject, DateTime date)
        {
            Action = action;
            Subject = subject;
            Date = date;
        }

        public ActionType Action { get; }
        public string Subject { get; }
        public DateTime Date { get; }

        protected bool Equals(Record other)
        {
            return Action == other.Action 
                   && string.Equals(Subject, other.Subject, StringComparison.InvariantCultureIgnoreCase) 
                   && Date.Equals(other.Date);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Record) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Action;
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                return hashCode;
            }
        }
    }
}
