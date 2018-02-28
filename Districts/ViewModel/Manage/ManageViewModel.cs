using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Districts.Helper;
using Districts.JsonClasses.Manage;
using Districts.MVVM;

namespace Districts.ViewModel.Manage
{
    class ManageViewModel : ObservableObject
    {
        private ObservableCollection<CardManagement> _cards = new ObservableCollection<CardManagement>();
        private Dictionary<string, string> _people = new Dictionary<string, string>();

        public ObservableCollection<CardManagement> Cards
        {
            get { return _cards; }
            set
            {
                if (Equals(value, _cards)) return;
                _cards = value;
                OnPropertyChanged();
            }
        }
        public Dictionary<string, string> People
        {
            get { return _people; }
            set
            {
                if (Equals(value, _people)) return;
                _people = value;
                OnPropertyChanged();
            }
        }

        public ManageViewModel()
        {
            var all = LoadingWork.LoadManageElements().Select(x => x.Value).ToList();
            Cards = new ObservableCollection<CardManagement>(all);

            // сгруппировал по именам и взял только те, что на руках
            var groupped = all.GroupBy(x => x.GetLastOwner())
                .ToDictionary(x => x.Key, x => x.GetEnumerator().ToIEnumerable().Where(y => y.HasOwner()).ToList());

            //отсортировал по возрастанию
            groupped.OrderBy(x => x.Value.Count);

            foreach (var person in groupped)
            {
                if (!string.IsNullOrWhiteSpace(person.Key) && person.Value.Any())
                {
                    // добавил все занятые карточки
                    var takenCards = person.Value.Select(x => x.Number + " ").Aggregate("", (s, s1) => s += s1);
                    People.Add(person.Key, takenCards);
                }
            }
        }

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
    }
}
