using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Districts.New.Interfaces
{
    interface iCard
    {
        int Number { get; }
        IList<iDoor> Doors { get; }
    }
}
