using System.Collections.Generic;

namespace Districts.New.Interfaces
{
    public interface iCard
    {
        int Number { get; }
        IList<iDoor> Doors { get; }
    }
}
