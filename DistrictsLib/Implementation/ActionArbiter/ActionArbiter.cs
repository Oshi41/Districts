using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Interfaces.ActionArbiter;

namespace DistrictsLib.Implementation.ActionArbiter
{
    public class ActionArbiter : IActionArbiter
    {
        private bool _isExecuting;

        #region Implementation of IActionArbiter

        public virtual void Do(Action action)
        {
            Do(() => Task.Run(action));
        }

        public async void Do(Func<Task> action)
        {
            if (action == null
                || _isExecuting)
            {
                return;
            }

            try
            {
                _isExecuting = true;
                await action();
            }
            finally
            {
                _isExecuting = false;
            }
        }

        #endregion
    }
}
