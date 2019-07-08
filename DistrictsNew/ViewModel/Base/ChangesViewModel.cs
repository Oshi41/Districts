using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Interfaces;

namespace DistrictsNew.ViewModel.Base
{
    public class ChangesViewModel : ErrorViewModel
    {
        protected readonly IChangeNotifier ChangeNotifier;

        public ChangesViewModel(IChangeNotifier changeNotifier)
        {
            ChangeNotifier = changeNotifier;
        }

        /// <summary>
        /// Были и произведены изменения
        /// </summary>
        public bool IsChanged => ChangeNotifier.IsChanged();

        protected virtual bool Set<T>(ref T source, T value, Action<T, T> callback, [CallerMemberName] string member = null)
        {
            if (!Equals(source, value))
            {
                callback(source, value);
            }

            return SetProperty(ref source, value, member);
        }
    }
}
