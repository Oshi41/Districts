using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Districts.JsonClasses;
using Districts.MVVM;

namespace Districts.ViewModel
{
    public class StreetViewModel : ObservableObject
    {
        private ObservableCollection<Building> _homes;

        public ObservableCollection<Building> Homes
        {
            get { return _homes; }
            set
            {
                if (Equals(value, _homes)) return;
                _homes = value;
                OnPropertyChanged();
            }
        }

        public string Street => _homes.FirstOrDefault()?.Street ?? string.Empty;


        public StreetViewModel(IList<Building> homes)
        {
            Homes = new ObservableCollection<Building>(homes);

        }
    }
}
