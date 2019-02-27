using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Districts.New.Interfaces
{
    public enum ActionType
    {
        Taken,
        Dropped,
        Deleted
    }

    interface iRecord
    {
        ActionType Action { get; }
        string Subject { get; }
        DateTime Date { get;  }
    }
}
