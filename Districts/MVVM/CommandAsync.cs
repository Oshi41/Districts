using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Districts.MVVM
{
    class CommandAsync : ICommand
    {
        private bool _working;

        private readonly Func<object, Task> _action;
        private readonly Predicate<object> _canExecute;

        public CommandAsync(Func<Task> action, Func<bool> canExecute = null)
            :this(o => action(), o => canExecute == null || canExecute())
        {
        }

        public CommandAsync(Func<object, Task> action, Predicate<object> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        #region Implementation of ICommand

        public bool CanExecute(object parameter)
        {
            return !_working
                   && _canExecute == null
                   || _canExecute(parameter);
        }

        public async void Execute(object parameter)
        {
            _working = true;

            await _action(parameter);

            _working = false;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion
    }
}
