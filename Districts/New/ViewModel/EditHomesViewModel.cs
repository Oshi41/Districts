using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Districts.Helper;
using Districts.New.Implementation.Classes;
using Districts.New.Interfaces;
using Districts.Parser.v2;
using Mvvm;
using Mvvm.Commands;

namespace Districts.New.ViewModel
{
    class EditHomesViewModel : BindableBase
    {
        private readonly IParser _parser;
        private ObservableCollection<StreetVm> streets;

        private object _selectedItem;

        public ObservableCollection<StreetVm> StreetHomes
        {
            get => streets;
            set => SetProperty(ref streets, value);
        }

        public EditHomesViewModel(IParser parser)
        {
            _parser = parser;

            Delete = new DelegateCommand(OnDelete, OnCanDelete);
            SetSelection = new DelegateCommand<object>(OnSetSelection);

            Refresh();
        }

        private bool OnCanDelete()
        {
            return _selectedItem != null;
        }

        private void OnSetSelection(object obj)
        {
            _selectedItem = obj;
        }

        private void OnDelete()
        {
            if (_selectedItem is StreetVm street
                && StreetHomes.Contains(street))
            {
                StreetHomes.Remove(street);
            }

            if (_selectedItem is HomeVm vm)
            {
                var first = StreetHomes.FirstOrDefault(x => x.Homes.Contains(vm));

                if (first != null)
                {
                    first.Homes.Remove(vm);

                    if (!first.Homes.Any())
                    {
                        StreetHomes.Remove(first);
                    }
                }
            }
        }

        public ICommand Delete { get; }
        public ICommand SetSelection { get; }

        private void Refresh()
        {
            var homes = _parser.LoadHomes().GroupBy(x => x.Street);

            StreetHomes =
                new ObservableCollection<StreetVm>(homes
                    .Select(x => new StreetVm(x.GetEnumerator()
                        .ToList())));
        }
    }

    class StreetVm : BindableBase
    {
        private ObservableCollection<iHome> _homes;

        public ObservableCollection<iHome> Homes
        {
            get => _homes;
            set => SetProperty(ref _homes, value);
        }

        public string StreetName { get; set; }

        public StreetVm(IEnumerable<iHome> homes)
        {
            Homes = new ObservableCollection<iHome>(homes.Select(x => new HomeVm(x)));
            StreetName = Homes.FirstOrDefault()?.Street;
        }
    }

    class HomeVm : BindableBase, iHome
    {
        private iHome _home;
        private string _comments;
        private bool _hasConcierge;
        private int _firstDoor;
        private int _floors;
        private int _entrances;

        public HomeVm(iHome home)
        {
            _home = home;

            Doors = home.Doors.Select(x => (iDoor)new DoorVm(x)).ToList();
            Comments = home.Comments;
            HasConcierge = home.HasConcierge;
            FirstDoor = home.FirstDoor;
            Floors = home.Floors;
            Entrances = home.Entrances;
        }

        public string Street => _home.Street;

        public int HomeNumber => _home.HomeNumber;

        public int Housing => _home.Housing;

        public int AfterSlash => _home.AfterSlash;

        public bool SameObject(iFind obj, ReturnConditions conditions = ReturnConditions.WithSlash)
        {
            return _home.SameObject(obj, conditions);
        }

        public IList<iDoor> Doors { get; }

        public bool HasConcierge
        {
            get => _hasConcierge;
            set => SetProperty(ref _hasConcierge, value);
        }

        public int FirstDoor
        {
            get => _firstDoor;
            set => SetProperty(ref _firstDoor, value);
        }

        public int Floors
        {
            get => _floors;
            set => SetProperty(ref _floors, value);
        }

        public int Entrances
        {
            get => _entrances;
            set => SetProperty(ref _entrances, value);
        }

        public string Comments
        {
            get => _comments;
            set => SetProperty(ref _comments, value);
        }

        public override string ToString()
        {
            return _home?.ToString();
        }
    }

    class DoorVm : BindableBase, iDoor
    {
        private iDoor _door;
        private int _entrance;
        private int _doorNumber;
        private DoorStatus _status;
        private IList<iCode> _codes;

        public DoorVm(iDoor door)
        {
            _door = door;

            DoorNumber = door.DoorNumber;
            Entrance = door.Entrance;
            Status = door.Status;
            Codes = door.Codes.Select(x => (iCode)new CodeVm(x)).ToList();
        }

        public string Street => _door.Street;

        public int HomeNumber => _door.HomeNumber;

        public int Housing => _door.Housing;

        public int AfterSlash => _door.AfterSlash;

        public bool SameObject(iFind obj, ReturnConditions conditions = ReturnConditions.WithSlash)
        {
            return _door.SameObject(obj, conditions);
        }

        public int DoorNumber
        {
            get => _doorNumber;
            set => SetProperty(ref _doorNumber, value);
        }

        public int Entrance
        {
            get => _entrance;
            set => SetProperty(ref _entrance, value);
        }

        public DoorStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public IList<iCode> Codes
        {
            get => _codes;
            set => SetProperty(ref _codes, value);
        }
    }

    class CodeVm : BindableBase, iCode
    {
        private CodeStatus _status;
        private string _text;

        public CodeVm(iCode code)
        {
            _status = code.Status;
            _text = code.Text;
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public CodeStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
    }
}
