using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DistrictsNew.Properties;
using Mvvm.Commands;

namespace DistrictsNew.Models
{
    /// <summary>
    ///     An <see cref="T:System.Windows.Input.ICommand" /> whose delegates do not take any parameters for <see cref="M:Mvvm.Commands.DelegateCommand.Execute" /> and
    ///     <see cref="M:Mvvm.Commands.DelegateCommand.CanExecute" />.
    /// </summary>
    /// <see cref="T:Mvvm.Commands.DelegateCommandBase" />
    /// <see cref="T:Mvvm.Commands.DelegateCommand`1" />
    public class ObservableCommand : DelegateCommandBase, INotifyPropertyChanged
    {
        private bool _isExecuting;

        /// <summary>
        ///     Creates a new instance of <see cref="T:Mvvm.Commands.DelegateCommand" /> with the <see cref="T:System.Action" /> to invoke on execution.
        /// </summary>
        /// <param name="executeMethod">The <see cref="T:System.Action" /> to invoke when <see cref="M:System.Windows.Input.ICommand.Execute(System.Object)" /> is called.</param>
        public ObservableCommand(Action executeMethod)
          : this(executeMethod, (Func<bool>)(() => true), false)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="T:Mvvm.Commands.DelegateCommand" /> with the <see cref="T:System.Action" /> to invoke on execution
        ///     and a <see langword="Func" /> to query for determining if the command can execute.
        /// </summary>
        /// <param name="executeMethod">The <see cref="T:System.Action" /> to invoke when <see cref="M:System.Windows.Input.ICommand.Execute(System.Object)" /> is called.</param>
        /// <param name="canExecuteMethod">
        ///     The <see cref="T:System.Func`1" /> to invoke when <see cref="M:System.Windows.Input.ICommand.CanExecute(System.Object)" /> is
        ///     called
        /// </param>
        /// <param name="isAutomaticRequeryDisabled"></param>
        public ObservableCommand(
          Action executeMethod,
          Func<bool> canExecuteMethod,
          bool isAutomaticRequeryDisabled = false)
          : base((Action<object>)(o => executeMethod()), (Func<object, bool>)(o => canExecuteMethod()), isAutomaticRequeryDisabled)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));
        }

        private ObservableCommand(Func<Task> executeMethod)
          : this(executeMethod, (Func<bool>)(() => true), false)
        {
        }

        private ObservableCommand(
          Func<Task> executeMethod,
          Func<bool> canExecuteMethod,
          bool isAutomaticRequeryDisabled = false)
          : base((Func<object, Task>)(o => executeMethod()), (Func<object, bool>)(o => canExecuteMethod()), isAutomaticRequeryDisabled)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));
        }

        /// <summary>
        ///     Factory method to create a new instance of <see cref="T:Mvvm.Commands.DelegateCommand" /> from an awaitable handler method.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command.</param>
        /// <returns>Constructed instance of <see cref="T:Mvvm.Commands.DelegateCommand" /></returns>
        public static ObservableCommand FromAsyncHandler(Func<Task> executeMethod)
        {
            return new ObservableCommand(executeMethod);
        }

        /// <summary>
        ///     Factory method to create a new instance of <see cref="T:Mvvm.Commands.DelegateCommand" /> from an awaitable handler method.
        /// </summary>
        /// <param name="executeMethod">
        ///     Delegate to execute when Execute is called on the command. This can be null to just hook up
        ///     a CanExecute delegate.
        /// </param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command. This can be null.</param>
        /// <param name="isAutomaticRequeryDisabled"></param>
        /// <returns>Constructed instance of <see cref="T:Mvvm.Commands.DelegateCommand" /></returns>
        public static ObservableCommand FromAsyncHandler(
          Func<Task> executeMethod,
          Func<bool> canExecuteMethod,
          bool isAutomaticRequeryDisabled = false)
        {
            return new ObservableCommand(executeMethod, canExecuteMethod, isAutomaticRequeryDisabled);
        }

        /// <summary>Executes the command.</summary>
        public virtual async Task Execute()
        {
            try
            {
                IsExecuting = true;
                await this.Execute((object)null);
            }
            finally
            {
                IsExecuting = true;
            }
        }

        /// <summary>Determines if the command can be executed.</summary>
        /// <returns>Returns <see langword="true" /> if the command can execute, otherwise returns <see langword="false" />.</returns>
        public virtual bool CanExecute()
        {
            return !IsExecuting && this.CanExecute((object)null);
        }

        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                if (!Equals(value, _isExecuting))
                {
                    _isExecuting = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
