using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Districts.New.Interfaces
{
    interface iHome : iFind
    {
        IList<iDoor> Doors { get; }
    }
}
