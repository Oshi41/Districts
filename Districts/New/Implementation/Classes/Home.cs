using System.Collections.Generic;
using Districts.Helper;
using Districts.New.Interfaces;
using Districts.Parser.v2.Converters;
using Newtonsoft.Json;

namespace Districts.New.Implementation.Classes
{
    public class Home : FindBase, iHome
    {
        [JsonConstructor]
        public Home(string street, int homeNumber, int housing, int afterSlash, IList<iDoor> doors) 
            : this(street, $"{homeNumber}k{housing}/{afterSlash}", doors)
        {
            Doors = doors;
        }

        public Home(string street, string number, IList<iDoor> doors)
            : base(street, number)
        {
            Doors = doors;
        }

        [JsonConverter(typeof(ListConverter<Door, iDoor>))]
        public IList<iDoor> Doors { get; }

        public string Comments { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Home home &&
                   base.Equals(obj) &&
                   Doors.IsTermwiseEquals(home.Doors);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
