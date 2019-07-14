using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DistrictsLib.Interfaces.ActionArbiter;

namespace DistrictsLib.Implementation.ActionArbiter
{
    public class SafeThreadActionArbiter : IActionArbiter
    {
        private readonly IActionArbiter _arbiter;
        private readonly SynchronizationContext _context;

        /// <summary>
        /// Должен создаваться только в GUI потоке!
        /// </summary>
        /// <param name="arbiter"></param>
        public SafeThreadActionArbiter(IActionArbiter arbiter)
        {
            _arbiter = arbiter;
            _context = SynchronizationContext.Current;
        }

        public void Do(Action action)
        {
            _arbiter.Do(action);
        }

        public void Do(Func<Task> action)
        {
            _context.Post(state =>
            {
                _arbiter.Do(action);
            }, null);
        }

        public bool IsExecuting()
        {
            return _arbiter.IsExecuting();
        }
    }
}
