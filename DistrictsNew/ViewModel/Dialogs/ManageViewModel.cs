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

        private readonly ITimedAction _timed;
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
        public static string HostName { get; } = nameof(ManageExtensions);

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _timed.ScheduleAction(PerformSearch);
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
                if (SetProperty(ref _selectedSort, value))
                {
                    _timed.ScheduleAction(PerformSearch);
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
            IChangeNotifier notifier,
            ITimedAction timed)
            : base(notifier)
        {
            _timed = timed;
            DeleteRowCommand = new DelegateCommand<CardManagementViewModel>(OnDeleteRow, OnCanDeleteRow);
            AddCardCommand = DelegateCommand.FromAsyncHandler(OnAddCard);
            EditCardCommand = DelegateCommand<CardManagementViewModel>.FromAsyncHandler(OnEditCard, OnCanEditCard);

            _origin = cards.ToList();
            PerformSearch();
        }

        #region Command handlers

        private bool OnCanDeleteRow(CardManagementViewModel arg)
        {
            if (arg == null)
                return false;

            var find = _origin.FirstOrDefault(x => string.Equals(arg.Number, x.Number));
            return find != null;
        }

        private void OnDeleteRow(CardManagementViewModel obj)
        {
            Replace( obj, null);
        }

        private async Task OnAddCard()
        {
            var addVm = new AddCardViewModel(new SimpleNotifier(), Origin.Select(x => x.Number).ToList());
            if (await Show(addVm))
            {
                Replace(null, new CardManagement { Number = addVm.CardName });
            }
        }

        private async Task OnEditCard(CardManagementViewModel obj)
        {
            var copy = CreateNew(obj);

            if (await Show(copy))
            {
                Replace(obj, copy);
            }
        }

        private bool OnCanEditCard(CardManagementViewModel arg)
        {
            return arg != null;
        }

        #endregion

        #region Private

        private void PerformSearch()
        {
            var find = Origin.Select(CreateNew);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                find = find.Where(x => x.Contains(SearchText));
            }

            if (SelectedSort?.Predicate != null)
            {
                find = find.Where(x => SelectedSort.Predicate(x));
            }

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

        private CardManagementViewModel CreateNew(ICardManagement source)
        {
            return new CardManagementViewModel(
                source,
                new SafeThreadActionArbiter(new ActionArbiter()), 
                GetAllSubjects,
                new SimpleNotifier());
        }

        private void Replace(ICardManagement old, ICardManagement add)
        {
            var changed = false;

            if (old != null)
            {
                var find = _origin.FirstOrDefault(x => string.Equals(x.Number, old.Number));
                if (find != null)
                {
                    _origin.Remove(find);
                    changed = true;
                }
            }

            if (add != null)
            {
                var find = _origin.FirstOrDefault(x => string.Equals(x.Number, add.Number));
                if (find != null)
                {
                    _origin.Remove(find);
                }

                _origin.Add(add);
                changed = true;
            }

            if (changed)
            {
                ChangeNotifier.SetChange(nameof(Origin));
                PerformSearch();
            }
        }

        #endregion
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
