using System.Collections.Generic;
using Districts.Helper;
using Districts.New.Interfaces;
using Districts.Parser.v2.Converters;
using Newtonsoft.Json;

namespace Districts.New.Implementation.Classes
{
    class Home : FindBase, iHome
    {
        [JsonConstructor]
        public Home(string street, int homeNumber, int housing, int afterSlash, IList<iDoor> doors) 
            : base(street, homeNumber, housing, afterSlash)
        {
            Doors = doors;
        }

        [JsonConverter(typeof(ListConverter<Door, iDoor>))]
        public IList<iDoor> Doors { get; }

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
