using System.Collections.Generic;

namespace Districts.New.Interfaces
{
    interface iManage
    {
        iCard Card { get; }
        IList<iRecord> Records { get; } 
    }
}
