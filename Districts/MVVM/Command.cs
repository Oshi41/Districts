using System;
using System.Windows.Input;

namespace Districts.MVVM
{
    public class Command : ICommand
    {
        private readonly Action<object> _action;
        private readonly Predicate<object> _condition;
        EventHandler _canExecuteChanged;

        public Command(Action<object> action, Predicate<object> condition = null)
        {
            _action = action;
            _condition = condition;
        }

        public Command(Action action, Func<bool> condition = null)
            : this(o => action?.Invoke(), o => condition?.Invoke() ?? true)
        {
        }

        public bool CanExecute(object parameter)
        {
            return _condition == null || _condition(parameter);
        }

        public void Execute(object parameter)
        {
            _action?.Invoke(parameter);
        }


        public event EventHandler CanExecuteChanged
        {
            add
            {
                _canExecuteChanged += value;
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                _canExecuteChanged -= value;
                CommandManager.RequerySuggested -= value;
            }
        }

        public void OnCanExecuteChanged()
        {
            _canExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}