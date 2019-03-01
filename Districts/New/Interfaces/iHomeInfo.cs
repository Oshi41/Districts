using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Districts.New.Interfaces
{
    interface iHomeInfo : iFind
    {
        IDictionary<int, IList<iCode>> Codes { get; }
        bool HasConcierge { get; }
        int FirstDoor { get; }
    }
}
