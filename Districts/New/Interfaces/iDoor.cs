using System;
using System.Collections.Generic;

namespace Districts.New.Interfaces
{
    [Flags]
    public enum DoorStatus
    {
        Good = 0,

        NoWorry = 1,
        Aggressive = 2,
        Restricted = 4,
    }

    public interface iDoor : iFind
    {
        int DoorNumber { get; }
        int Entrance { get; }
        DoorStatus Status { get; }
        IList<iCode> Codes { get; }
    }
}
