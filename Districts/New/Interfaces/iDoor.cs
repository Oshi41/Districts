using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
