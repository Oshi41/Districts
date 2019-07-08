using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DistrictsLib.Interfaces.ActionArbiter;

namespace DistrictsLib.Implementation.ActionArbiter
{
    public class TimedAction : ITimedAction
    {
        private readonly IActionArbiter _arbiter;
        private readonly Timer _timer;
        private Func<Task> _action;

        public TimedAction(IActionArbiter arbiter, int mls = 200)
        {
            _arbiter = arbiter;
            _timer = new Timer(mls);
            _timer.Elapsed += PerformAction;
        }

        private void PerformAction(object sender, ElapsedEventArgs e)
        {
            _arbiter.Do(_action);
        }

        #region Implementation of ITimedAction

        public void ScheduleAction(Action action)
        {
            ScheduleAction(() => Task.Run(action));
        }

        public void ScheduleAction(Func<Task> action)
        {
            _timer.Stop();
            _action = action;
            _timer.Start();
        }

        #endregion
    }
}
