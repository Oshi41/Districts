using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Districts.JsonClasses;
using Mvvm;

namespace Districts.ViewModel
{
    public class StreetViewModel : BindableBase
    {
        private ObservableCollection<Building> _homes;


        public StreetViewModel(IList<Building> homes)
        {
            Homes = new ObservableCollection<Building>(homes);
        }

        public ObservableCollection<Building> Homes
        {
            get => _homes;
            set
            {
                if (Equals(value, _homes)) return;
                _homes = value;
                OnPropertyChanged();
            }
        }

        public string Street => _homes.FirstOrDefault()?.Street ?? string.Empty;
    }
}