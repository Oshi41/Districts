using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DistrictsLib.Extentions;
using DistrictsLib.Implementation.ActionArbiter;
using DistrictsLib.Interfaces.ActionArbiter;
using DistrictsLib.Interfaces.Json;
using DistrictsNew.ViewModel.Manage;

namespace DistrictsNew.ViewModel.Dialogs
{
    class ManageViewModel : ExtendedBindable
    {
        private readonly IActionArbiter _arbiter;
        private readonly DateTime _today = DateTime.Today;

        private string _searchText;
        private SortPresets _selectedSort;

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

        public ObservableCollection<CardManagementViewModel> Cards { get; set; }

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

        public ICommand AddCardCommand { get; }

        /// <summary>
        /// оригинальная копия
        /// </summary>
        public IReadOnlyCollection<ICardManagement> Origin { get; }

        public ManageViewModel(IList<ICardManagement> cards)
        {
            Origin = cards.ToList();
            PerformSearch();
        }

        private void PerformSearch()
        {
            var all = Origin.Select(x => new CardManagementViewModel(x, new ActionArbiter())).ToList();
            IEnumerable<CardManagementViewModel> find = all.Select(x => x);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                find = find.Where(x => x.Contains(SearchText));
            }

            find = find.Where(x => SelectedSort.Predicate(x));

            Cards = new ObservableCollection<CardManagementViewModel>(find);
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
