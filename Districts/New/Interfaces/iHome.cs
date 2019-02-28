using System.Collections.Generic;

namespace Districts.New.Interfaces
{
    interface iHome : iFind
    {
        IList<iDoor> Doors { get; }
    }
}
