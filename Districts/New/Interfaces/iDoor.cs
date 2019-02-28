using System.Collections.Generic;

namespace Districts.New.Interfaces
{
    public enum DoorStatus
    {
        Good,
        NotWarning,
        Aggressive,
        Restricted
    }

    interface iDoor : iFind
    {
        int DoorNumber { get; }
        int Entrance { get; }
        DoorStatus Status { get; }
        IList<iCode> Codes { get; }
    }
}
