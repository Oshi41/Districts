using System.ComponentModel;
using System.Runtime.CompilerServices;
using Districts.Annotations;

namespace Districts.MVVM
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
    }
}