using System.Collections.Generic;

namespace Districts.New.Interfaces
{
    public interface iHome : iFind
    {
        IList<iDoor> Doors { get; }

        bool HasConcierge { get; }
        int FirstDoor { get; }
        int Floors { get; }
        int Entrances { get; }
        string Comments { get; set; }
    }
}
