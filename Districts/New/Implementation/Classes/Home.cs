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
        public Home(string street, int homeNumber, int housing, int afterSlash, IList<iDoor> doors, bool hasConcierge,
            int firstDoor, int floors, int entrances)
            : this(street, $"{homeNumber}k{housing}/{afterSlash}", doors, hasConcierge, firstDoor, floors, entrances)
        {
            Doors = doors;
        }

        public Home(string street, string number, IList<iDoor> doors, bool hasConcierge, int firstDoor, int floors,
            int entrances)
            : base(street, number)
        {
            Doors = doors;
            HasConcierge = hasConcierge;
            FirstDoor = firstDoor;
            Floors = floors;
            Entrances = entrances;
        }

        public Home(iFind find, IList<iDoor> doors, bool hasConcierge, int firstDoor, int floors,
            int entrances)
            : this(find.Street, find.HomeNumber, find.Housing,
                find.AfterSlash, doors, hasConcierge, firstDoor,
                floors, entrances)
        {

        }

        [JsonConverter(typeof(ListConverter<Door, iDoor>))]
        public IList<iDoor> Doors { get; }

        public bool HasConcierge { get; }
        public int FirstDoor { get; }
        public int Floors { get; }
        public int Entrances { get; }

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
