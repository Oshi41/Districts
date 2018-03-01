using Districts.MVVM;

namespace Districts.ViewModel.PrintVM
{
    public class Record : ObservableObject
    {
        private string _street;
        private string _number;
        private string _codes;
        private string _floor;


        public string Street
        {
            get { return _street; }
            set
            {
                if (value == _street) return;
                _street = value;
                OnPropertyChanged();
            }
        }
        public string Number
        {
            get { return _number; }
            set
            {
                if (value == _number) return;
                _number = value;
                OnPropertyChanged();
            }
        }
        public string Floor
        {
            get { return _floor; }
            set
            {
                if (value == _floor) return;
                _floor = value;
                OnPropertyChanged();
            }
        }
        public string Codes
        {
            get { return _codes; }
            set
            {
                if (value == _codes) return;
                _codes = value;
                OnPropertyChanged();
            }
        }
    }
}
