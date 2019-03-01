using System.Collections.Generic;
using Districts.New.Interfaces;
using Newtonsoft.Json;

namespace Districts.New.Implementation.Classes
{
    class CommonHomeInfo : FindBase, iHomeInfo
    {
        [JsonConstructor]
        public CommonHomeInfo(string street, int homeNumber, int housing, int afterSlash, 
            IDictionary<int, IList<iCode>> codes, bool hasConcierge, int firstDoor) 
            : base(street, homeNumber, housing, afterSlash)
        {
            Init(codes, hasConcierge, firstDoor);
        }

        public CommonHomeInfo(string street, string number, IDictionary<int, IList<iCode>> codes, bool hasConcierge, 
            int firstDoor) 
            : base(street, number)
        {
            Init(codes, hasConcierge, firstDoor);
        }

        private void Init(IDictionary<int, IList<iCode>> codes, bool hasConcierge, int firstDoor)
        {
            Codes = codes;
            HasConcierge = hasConcierge;
            FirstDoor = firstDoor;
        }


        public IDictionary<int, IList<iCode>> Codes { get; private set; }
        public bool HasConcierge { get; private set; }
        public int FirstDoor { get; private set; }
    }
}
