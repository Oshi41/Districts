using Districts.MVVM;

namespace Districts.ViewModel.PrintVM
{
    public class Record : ObservableObject
    {
        private string _codes;
        private string _floor;
        private string _number;
        private string _street;


        public string Street
        {
            get => _street;
            set
            {
                if (value == _street) return;
                _street = value;
                OnPropertyChanged();
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

        public string Floor
        {
            get => _floor;
            set
            {
                if (value == _floor) return;
                _floor = value;
                OnPropertyChanged();
            }
        }

        public string Codes
        {
            get => _codes;
            set
            {
                if (value == _codes) return;
                _codes = value;
                OnPropertyChanged();
            }
        }
    }
}