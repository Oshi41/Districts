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
        private readonly IDialogProvider _provider;
        private ObservableCollection<StreetVm> streets;

        private object _selectedItem;

        public ObservableCollection<StreetVm> StreetHomes
        {
            get => streets;
            set => SetProperty(ref streets, value);
        }

        public EditHomesViewModel(IParser parser, IDialogProvider provider)
        {
            _parser = parser;
            _provider = provider;

            Delete = new DelegateCommand(OnDelete, OnCanDelete);
            SetSelection = new DelegateCommand<object>(OnSetSelection);
            EditHomeCommand = new DelegateCommand(OnEditHome, OnCanEdit);

            Refresh();
        }

        private bool OnCanEdit()
        {
            return _selectedItem is HomeVm;
        }

        private void OnEditHome()
        {
            if (_selectedItem is HomeVm vm)
            {
                var copy = new HomeVm(vm.ToHome());

                if (_provider.ShowDialog(copy, 350))
                {
                    var street = StreetHomes
                        .FirstOrDefault(x => x.Homes.Any(
                            c => c.SameObject(vm)));

                    if (street != null)
                    {
                        street.Homes.Remove(vm);
                        street.Homes.Add(new HomeVm(copy.ToHome()));
                    }
                }
            }

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

        public ICommand EditHomeCommand { get; }

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
        private ObservableCollection<HomeVm> _homes;

        public ObservableCollection<HomeVm> Homes
        {
            get => _homes;
            set => SetProperty(ref _homes, value);
        }

        public string StreetName { get; set; }

        public StreetVm(IEnumerable<iHome> homes)
        {
            Homes = new ObservableCollection<HomeVm>(homes.Select(x => new HomeVm(x)));
            StreetName = homes.FirstOrDefault()?.Street;
        }
    }

    class HomeVm : BindableBase
    {
        private iHome _vm;

        private readonly IDoorStatusParser _parser;
        private string _aggresiveRange;
        private string _noWorryRange;
        private string _restrictedRange;
        private bool _hasConcierge;
        private int _firstDoor;
        private ObservableCollection<DoorVm> _doors;

        public HomeVm(iHome vm)
        {
            _vm = vm;
            _parser = new DoorStatusParser();

            HasConcierge = _vm.HasConcierge;
            FirstDoor = _vm.FirstDoor;
            Doors = new ObservableCollection<DoorVm>(vm.Doors.Select(x => new DoorVm(x)));
        }

        public iHome ToHome()
        {
            _parser.Populate(AggresiveRange, Doors, DoorStatus.Aggressive);
            _parser.Populate(RestrictedRange, Doors, DoorStatus.Restricted);
            _parser.Populate(NoWorryRange, Doors, DoorStatus.NoWorry);

            _vm = new Home(_vm,
                Doors.OfType<iDoor>().ToList(),
                HasConcierge,
                FirstDoor,
                _vm.Floors,
                _vm.Entrances);

            return _vm;
        }

        public string AggresiveRange
        {
            get => _aggresiveRange;
            set => SetProperty(ref _aggresiveRange, value);
        }

        public string NoWorryRange
        {
            get => _noWorryRange;
            set => SetProperty(ref _noWorryRange, value);
        }

        public string RestrictedRange
        {
            get => _restrictedRange;
            set => SetProperty(ref _restrictedRange, value);
        }

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
        public ObservableCollection<DoorVm> Doors
        {
            get => _doors;
            set => SetProperty(ref _doors, value);
        }

        public bool SameObject(HomeVm obj)
        {
            return _vm.SameObject(obj?._vm);
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
