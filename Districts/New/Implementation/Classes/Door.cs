using System.Collections.Generic;
using Districts.Helper;
using Districts.New.Interfaces;
using Districts.Parser.v2.Converters;
using Newtonsoft.Json;

namespace Districts.New.Implementation.Classes
{
    public class Door : FindBase, iDoor
    {
        [JsonConstructor]
        public Door(string street, int homeNumber, int housing, int afterSlash, int doorNumber, 
            int entrance, DoorStatus status, IList<iCode> codes) 
            : base(street, homeNumber, housing, afterSlash)
        {
            DoorNumber = doorNumber;
            Entrance = entrance;
            Status = status;
            Codes = codes;
        }

        public Door(iFind find, int doorNumber, int entrance, DoorStatus status, IList<iCode> codes)
            : this(find.Street, find.HomeNumber, find.Housing, find.AfterSlash, doorNumber, entrance, status, codes)
        {
        }

        public int DoorNumber { get; }
        public int Entrance { get; }
        public DoorStatus Status { get; }

        [JsonConverter(typeof(ListConverter<Code, iCode>))]
        public IList<iCode> Codes { get; }

        public override bool Equals(object obj)
        {
            return obj is Door door &&
                   base.Equals(obj) 
                   && DoorNumber == door.DoorNumber 
                   && Entrance == door.Entrance 
                   && Status == door.Status 
                   && Codes.IsTermwiseEquals(door.Codes);
        }

        public override int GetHashCode()
        {
            var hashCode = -1864148151;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + DoorNumber.GetHashCode();
            hashCode = hashCode * -1521134295 + Entrance.GetHashCode();
            hashCode = hashCode * -1521134295 + Status.GetHashCode();
            return hashCode;
        }
    }
}
