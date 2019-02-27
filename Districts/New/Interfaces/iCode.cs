using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Districts.New.Interfaces
{
    public enum CodeStatus
    {
        Good,
        NotWorking,
        Restricted,
    }

    interface iCode
    {
        string Text { get; }
        CodeStatus Status { get; }
    }
}
