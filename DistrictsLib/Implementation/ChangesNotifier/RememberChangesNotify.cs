using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Interfaces;

namespace DistrictsLib.Implementation.ChangesNotifier
{
    public class RememberChangesNotify : IChangeNotifier
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();
        private readonly Dictionary<string, bool> _changedValues = new Dictionary<string, bool>();

        public void Notify<T>(T old, T val, [CallerMemberName]string member = null)
        {
            if (member == null)
            {
                // Значения совпадают, все ок
                if (Equals(old, val))
                {
                    return;
                }

                // Неизвестный член, создаем GUID
                member = Guid.NewGuid().ToString();
            }

            //if (old is IList oldList)
            //{
            //    old = (T)(IList)oldList.OfType<object>().ToList();
            //}

            //if (val is IList addList)
            //{
            //    val = (T)(IList)addList.OfType<object>().ToList();
            //}

            // Записываем старое значение
            if (!_values.ContainsKey(member))
            {
                _values[member] = old;
            }

            // Сравниваем с предыдущим
            _changedValues[member] = Equals(_values[member], val);
            OnPropertyChanged(nameof(IsChanged));
        }

        public void SetChange([CallerMemberName]string member = null)
        {
            Notify<object>(null, null, member);
        }

        public bool IsChanged()
        {
            return _changedValues.Any(x => !x.Value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
