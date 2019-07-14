using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Interfaces.ActionArbiter;

namespace DistrictsLib.Implementation.ActionArbiter
{
    class BlockedAction : IBlockedAction, IActionArbiter
    {
        private readonly IActionArbiter _action;
        private bool _isBlocked;

        public BlockedAction(IActionArbiter action)
        {
            _action = action;
        }

        #region Implementation of IBlockedAction

        public void Block()
        {
            _isBlocked = true;
        }

        public void Release()
        {
            _isBlocked = false;
        }

        #endregion

        #region Implementation of IActionArbiter

        public void Do(Action action)
        {
            Do(() => Task.Run(action));
        }

        public void Do(Func<Task> action)
        {
            if (_isBlocked)
                return;

            _action.Do(action);
        }

        public bool IsExecuting()
        {
            return _action.IsExecuting();
        }

        #endregion
    }
}
