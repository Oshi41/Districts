using System;
using Districts.New.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Districts.New.Implementation.Classes
{
    public class Code : iCode
    {
        [JsonConstructor]
        public Code(string text, CodeStatus status)
        {
            Text = text;
            Status = status;
        }

        public string Text { get; }

        [JsonConverter(typeof(StringEnumConverter))]
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
