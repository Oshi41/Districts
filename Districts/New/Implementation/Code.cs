using System;
using Districts.New.Interfaces;

namespace Districts.New.Implementation
{
    class Code : iCode
    {
        public Code(string text, CodeStatus status)
        {
            Text = text;
            Status = status;
        }

        public string Text { get; }
        public CodeStatus Status { get; }

        protected bool Equals(Code other)
        {
            return string.Equals(Text, other.Text, StringComparison.InvariantCultureIgnoreCase) 
                   && Status == other.Status;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Code) obj);
        }

        public override int GetHashCode()
        {
            return (int) Status;
        }
    }
}
