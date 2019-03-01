using System.Collections.Generic;

namespace Districts.New.Interfaces
{
    public interface iHome : iFind
    {
        IList<iDoor> Doors { get; }

        string Comments { get; set; }
    }
}
