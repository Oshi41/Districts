using System.Collections.ObjectModel;
using Districts.MVVM;

namespace Districts.ViewModel.Manage
{
    class ManageViewModel : ObservableObject
    {
        private ObservableCollection<ManageRecordViewModel> _managements;

        
        public ObservableCollection<ManageRecordViewModel> Managements
        {
            get { return _managements; }
            set
            {
                if (Equals(value, _managements)) return;
                _managements = value;
                OnPropertyChanged();
            }
        }

    }
}
//private ObservableCollection<CardManagement> _cards = new ObservableCollection<CardManagement>();
        //private Dictionary<string, string> _people = new Dictionary<string, string>();
        //private bool _isSorted;

        //public ObservableCollection<CardManagement> Cards
        //{
        //    get { return _cards; }
        //    set
        //    {
        //        if (Equals(value, _cards)) return;
        //        _cards = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public Dictionary<string, string> People
        //{
        //    get { return _people; }
        //    set
        //    {
        //        if (Equals(value, _people)) return;
        //        _people = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public bool IsSorted
        //{
        //    get { return _isSorted; }
        //    set
        //    {
        //        if (value == _isSorted) return;
        //        _isSorted = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public ICommand SaveCommand { get; set; }

        //public ManageViewModel()
        //{
        //    var all = LoadingWork.LoadManageElements().Select(x => x.Value).ToList();
        //    Cards = new ObservableCollection<CardManagement>(all);

        //    // сгруппировал по именам и взял только те, что на руках
        //    //var groupped = all.GroupBy(x => x.GetLastOwner())
        //    //    .ToDictionary(x => x.Key, x => x.GetEnumerator().ToIEnumerable().Where(y => y.HasOwner()).ToList());

        //    ////отсортировал по возрастанию
        //    //groupped.OrderBy(x => x.Value.Count);

        //    //foreach (var person in groupped)
        //    //{
        //    //    if (!string.IsNullOrWhiteSpace(person.Key) && person.Value.Any())
        //    //    {
        //    //        // добавил все занятые карточки
        //    //        var takenCards = person.Value.Select(x => x.Number + " ").Aggregate("", (s, s1) => s += s1);
        //    //        People.Add(person.Key, takenCards);
        //    //    }
        //    //}
        //}

        //private ObservableCollection<ManageViewModel> _cards = new ObservableCollection<ManageViewModel>();
        //private Dictionary<string, string> _people = new Dictionary<string, string>();

        //public ObservableCollection<Manage> Cards
        //{
        //    get { return _cards; }
        //    set
        //    {
        //        if (Equals(value, _cards)) return;
        //        _cards = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public Dictionary<string, string> People
        //{
        //    get { return _people; }
        //    set
        //    {
        //        if (Equals(value, _people)) return;
        //        _people = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public ICommand SaveCommand { get; set; }


        //public ManageViewModel()
        //{
        //    var all = Helper.Helper.LoadCardManagements();
        //    Cards = new ObservableCollection<ManageViewModel>(all.Select(x => new ManageViewModel(x)));
        //}