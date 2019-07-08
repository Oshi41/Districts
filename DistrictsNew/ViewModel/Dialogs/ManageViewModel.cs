using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DistrictsLib.Extentions;
using DistrictsLib.Implementation.ActionArbiter;
using DistrictsLib.Implementation.ChangesNotifier;
using DistrictsLib.Interfaces;
using DistrictsLib.Interfaces.ActionArbiter;
using DistrictsLib.Interfaces.Json;
using DistrictsLib.Legacy.JsonClasses.Manage;
using DistrictsNew.ViewModel.Base;
using DistrictsNew.ViewModel.HostDialogs;
using DistrictsNew.ViewModel.Manage;
using MaterialDesignThemes.Wpf;
using Mvvm;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel.Dialogs
{
    class ManageViewModel : ChangesViewModel
    {
        #region Fields

        private readonly IActionArbiter _arbiter;
        private readonly DateTime _today = DateTime.Today;

        private string _searchText;
        private SortPresets _selectedSort;
        private ObservableCollection<CardManagementViewModel> _cards;
        private readonly ICollection<ICardManagement> _origin;

        #endregion

        #region PRops

        /// <summary>
        /// Типы сортировок
        /// </summary>
        public List<SortPresets> SortTypes => new List<SortPresets>
        {
            new SortPresets(x => true, Properties.Resources.Manage_All),

            new SortPresets(x => x.IsActive(), Properties.Resources.Manage_OnHands),
            new SortPresets(x => !x.IsActive(), Properties.Resources.Manage_NotOnHands),


            new SortPresets(x => !x.IsActive()
                                 && _today - x.LastPassed() > TimeSpan.FromDays(30 * 3) , Properties.Resources.Manage_NotOnHands3),

            new SortPresets(x => !x.IsActive()                        // полгода
                                 && _today - x.LastPassed() > TimeSpan.FromDays(182) , Properties.Resources.Manage_NotOnHands6),

            new SortPresets(x => !x.IsActive()
                                 && _today - x.LastPassed() > TimeSpan.FromDays(365) , Properties.Resources.Manage_NotOnHandsYear),



            new SortPresets(x => x.IsActive()
                                 && _today - x.LastTaking() > TimeSpan.FromDays(30 * 3) , Properties.Resources.Manage_NotOnHands3),

            new SortPresets(x => x.IsActive()                       // полгода
                                 && _today - x.LastTaking() > TimeSpan.FromDays(182) , Properties.Resources.Manage_NotOnHands6),

            new SortPresets(x => x.IsActive()
                                 && _today - x.LastTaking() > TimeSpan.FromDays(365) , Properties.Resources.Manage_NotOnHandsYear),
        };

        /// <summary>
        /// Имя хоста
        /// </summary>
        public string HostName { get; } = nameof(ManageExtensions);

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    PerformSearch();
                }
            }
        }

        public ObservableCollection<CardManagementViewModel> Cards
        {
            get => _cards;
            set => SetProperty(ref _cards, value);
        }

        public SortPresets SelectedSort
        {
            get => _selectedSort;
            set
            {
                if (SetProperty(ref _selectedSort, value)
                    && value == null)
                {
                    SetProperty(ref _selectedSort, SortTypes.First());
                }
            }
        }

        /// <summary>
        /// оригинальная копия
        /// </summary>
        public IReadOnlyCollection<ICardManagement> Origin => (IReadOnlyCollection<ICardManagement>)_origin;

        public ICommand DeleteRowCommand { get; }

        public ICommand EditCardCommand { get; }

        public ICommand AddCardCommand { get; }

        #endregion

        public ManageViewModel(IList<ICardManagement> cards,
            IChangeNotifier notifier)
            : base(notifier)
        {
            DeleteRowCommand = new DelegateCommand<CardManagementViewModel>(OnDeleteRow, OnCanDeleteRow);
            AddCardCommand = DelegateCommand.FromAsyncHandler(OnAddCard);
            EditCardCommand = new DelegateCommand<ICardManagement>(OnEditCard, OnCanEditCard);

            _origin = cards.ToList();
            PerformSearch();
        }

        #region Command handlers

        private bool OnCanDeleteRow(CardManagementViewModel arg)
        {
            return arg != null && Cards.Contains(arg);
        }

        private void OnDeleteRow(CardManagementViewModel obj)
        {
            Cards.Remove(obj);
            ChangeNotifier.SetChange(nameof(Cards));
        }

        private async Task OnAddCard()
        {
            var addVm = new AddCardViewModel(new SimpleNotifier(), Origin.Select(x => x.Number).ToList());
            if (await Show(addVm))
            {
                _origin.Add(new CardManagement { Number = HostName });
            }
        }

        private void OnEditCard(ICardManagement obj)
        {

        }

        private bool OnCanEditCard(ICardManagement arg)
        {
            return arg != null;
        }

        #endregion

        private void PerformSearch()
        {
            var all = Origin
                .Select(x => new CardManagementViewModel(x,
                                                         new SafeThreadActionArbiter(new ActionArbiter()), 
                                                         GetAllSubjects,
                                                         new SimpleNotifier()))
                .ToList();

            IEnumerable<CardManagementViewModel> find = all.Select(x => x);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                find = find.Where(x => x.Contains(SearchText));
            }

            find = find.Where(x => SelectedSort.Predicate(x));

            Cards = new ObservableCollection<CardManagementViewModel>(find);
        }

        private async Task<bool> Show(BindableBase vm)
        {
            var result = await DialogHost.Show(vm, HostName);
            return Equals(true, result);
        }

        private IList<string> GetAllSubjects()
        {
            return Origin
                .SelectMany(x => x.Actions)
                .Select(x => x.Subject)
                .Distinct()
                .ToList();
        }
    }

    public class SortPresets
    {
        public SortPresets(Predicate<ICardManagement> predicate, string name)
        {
            Predicate = predicate;
            Name = name;
        }

        public string Name { get; }

        public Predicate<ICardManagement> Predicate { get; }
    }
}
