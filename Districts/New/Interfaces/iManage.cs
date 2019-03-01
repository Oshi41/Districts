using System.Collections.Generic;

namespace Districts.New.Interfaces
{
    public interface iManage
    {
        iCard Card { get; }
        IList<iRecord> Records { get; } 
    }
}
