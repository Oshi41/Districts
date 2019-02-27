using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Districts.JsonClasses;
using Mvvm;

namespace Districts.ViewModel.PrintVM
{
    public class OneCardSizeViewModel : BindableBase
    {
        public OneCardSizeViewModel(int number, List<Door> doors)
        {
            Number = number.ToString();

            foreach (var door in doors)
            {
                var record = new Record();
                record.Street = door.Street;
                record.Floor = door.Number.ToString();
                record.Number = door.HouseNumber;
                record.Codes = door.Codes.Aggregate(string.Empty, (s, s1) => s += s1);
                Records.Add(record);
            }
        }

        public string Number
        {
            get => _number;
            set
            {
                if (value == _number) return;
                _number = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Record> Records
        {
            get => _records;
            set
            {
                if (Equals(value, _records)) return;
                _records = value;
                OnPropertyChanged();
            }
        }

        #region Fields

        private string _number;
        private ObservableCollection<Record> _records = new ObservableCollection<Record>();

        #endregion
    }
}