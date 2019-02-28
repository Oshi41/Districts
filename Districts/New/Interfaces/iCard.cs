using System.Collections.Generic;

namespace Districts.New.Interfaces
{
    interface iCard
    {
        int Number { get; }
        IList<iDoor> Doors { get; }
    }
}
