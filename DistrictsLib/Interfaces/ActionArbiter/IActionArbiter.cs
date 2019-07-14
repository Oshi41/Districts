using System;
using System.Threading.Tasks;

namespace DistrictsLib.Interfaces.ActionArbiter
{
    public interface IActionArbiter
    {
        void Do(Action action);

        void Do(Func<Task> action);

        bool IsExecuting();
    }
}
