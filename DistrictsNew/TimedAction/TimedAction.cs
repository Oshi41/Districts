using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DistrictsNew.Interfaces;

namespace DistrictsNew.TimedAction
{
    class TimedAction : ITimedAction
    {
        private readonly Timer _timer;
        private readonly TimeSpan _diff;
        private Func<Task> _action;
        private DateTime _last;
        private bool _isWorking;

        public TimedAction(int mls = 200)
        {
            _diff = TimeSpan.FromMilliseconds(mls);
            _timer = new Timer(40);
            _timer.Elapsed += OnTick;
        }

        private void OnTick(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;

            if (now - _last < _diff)
                return;

            _last = now;
            Do(_action);
        }

        #region Implementation of ITimedAction

        public void Schedule(Func<Task> action)
        {
            _action = action;
            _last = DateTime.Now;
            _timer.Start();
        }

        #endregion

        #region Implementation of IActionArbiter

        public async void Do(Func<Task> action)
        {
            if (_isWorking || action == null)
                return;

            try
            {
                _isWorking = true;
                await action();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            finally
            {
                _isWorking = false;
                _timer.Stop();
            }
        }

        #endregion
    }
}
