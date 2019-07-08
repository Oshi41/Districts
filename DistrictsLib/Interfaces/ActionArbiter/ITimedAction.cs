using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrictsLib.Interfaces.ActionArbiter
{
    public interface ITimedAction
    {
        void ScheduleAction(Action action);
        void ScheduleAction(Func<Task> action);
    }
}
