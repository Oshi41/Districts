using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            ChangeNotifier.PropertyChanged += NofifyChanges;
        }

        private void NofifyChanges(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsChanged));
        }

        /// <summary>
        /// Были и произведены изменения
        /// </summary>
        public bool IsChanged => ChangeNotifier.IsChanged();

        protected bool SetAndRemember<T>(ref T source, T value, [CallerMemberName] string member = null)
        {
            if (!Equals(source, value))
            {
                ChangeNotifier.Notify(source, value);
                SetProperty(ref source, value, member);

                return true;
            }

            return false;
        }

    }
}
