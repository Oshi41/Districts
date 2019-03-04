using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Districts.Properties;

namespace Districts.MVVM
{
    public class ObservableObject : INotifyPropertyChanged
    {
        protected SynchronizationContext _sync;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableObject()
        {
            _sync = SynchronizationContext.Current;
        }

        /// <summary>
        ///     Применять с осторожностью, Reflection
        /// </summary>
        protected void InvalidateCommands()
        {
            var properties = GetType().GetProperties();
            foreach (var property in properties)
                if (property.PropertyType == typeof(Command))
                {
                    var value = (Command) property.GetValue(this);
                    value?.OnCanExecuteChanged();
                }
        }

        protected bool Set<T>(ref T source, T value, [CallerMemberName]string caller = null)
        {
            if (Equals(source, value))
                return false;

            source = value;
            OnPropertyChanged(caller);
            return true;
        }

        protected void SafeExecute(Action action)
        {
            _sync.Post(state => action(), null);
        }
    }
}