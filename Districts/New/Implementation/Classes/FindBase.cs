using System;
using System.Linq;
using Districts.New.Interfaces;
using Newtonsoft.Json;

namespace Districts.New.Implementation.Classes
{
    public abstract class FindBase : iFind
    {
        [JsonConstructor]
        protected FindBase(string street, int homeNumber, int housing, int afterSlash)
        {
            Street = street;
            HomeNumber = homeNumber;
            Housing = housing;
            AfterSlash = afterSlash;
        }

        protected FindBase(string street, string number)
             :this(street, -1,-1,-1)
        {
            number = number.Replace(" ", "");

            // parse nuber
            if (int.TryParse(new string(number.TakeWhile(char.IsDigit).ToArray()), out var parsedNumber))
            {
                HomeNumber = parsedNumber;
            }

            // parse housing
            var index = number.IndexOf(number.FirstOrDefault(char.IsLetter));
            if (index >= 0
                && int.TryParse(new string(number
                    .Skip(index + 1)
                    .TakeWhile(char.IsDigit)
                    .ToArray()), out var parsedHousing))
            {
                Housing = parsedHousing;
            }

            // parse slash
            index = number.IndexOfAny(new[] {'/', '\\'});
            if (index >= 0
                && int.TryParse(new string(
                    number
                        .Skip(index + 1)
                        .TakeWhile(char.IsDigit)
                        .ToArray()), out var parsedSlash))
            {
                AfterSlash = parsedSlash;
            }
        }

        public string Street { get; }
        public int HomeNumber { get; }
        public int Housing { get; }
        public int AfterSlash { get; }

        public bool SameObject(iFind obj, ReturnConditions conditions = ReturnConditions.WithSlash)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj == null)
                return false;

            if (!string.Equals(Street, obj.Street, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var sameNumber = HomeNumber == obj.HomeNumber;
            var sameHousing = Housing == obj.Housing;
            var sameSlash = AfterSlash == obj.AfterSlash;

            if ((conditions & ReturnConditions.SameHouse) != 0
                && sameNumber)
            {
                return true;
            }

            if ((conditions & ReturnConditions.SameHousing) != ReturnConditions.SameHousing
                && sameHousing)
            {
                return true;
            }

            if ((conditions & ReturnConditions.WithSlash) != ReturnConditions.WithSlash
                && sameSlash)
            {
                return true;
            }
            
            //todo add more/less

            return false;
        }

        public string GetPrettyHouseName()
        {
            var result = HomeNumber.ToString();

            if (Housing >= 0)
            {
                result += "k " + Housing;
            }

            if (AfterSlash >= 0)
            {
                result += "/ " + AfterSlash;
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is iFind find)
            {
                return Equals(find);
            }

            return false;
        }

        protected bool Equals(iFind other)
        {
            return SameObject(other, ReturnConditions.WithSlash);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = HomeNumber;
                hashCode = (hashCode * 397) ^ Housing;
                hashCode = (hashCode * 397) ^ AfterSlash;
                return hashCode;
            }
        }
    }
}
