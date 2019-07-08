using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrictsLib.Interfaces.Json
{
    public interface ICardManagement
    {
        string Number { get; set; }
        IList<IManageRecord> Actions { get; set; }
    }
}
