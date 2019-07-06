using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsNew.TimedAction;

namespace DistrictsNew.Interfaces
{
    public interface ITimedAction : IActionArbiter
    {
        /// <summary>
        /// Ставим в очередь действие
        /// </summary>
        /// <param name="action"></param>
        void Schedule(Func<Task> action);
    }
}
