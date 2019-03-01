using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.JsonClasses.Manage;
using Districts.Settings.v1;
using Districts.ViewModel.Manage;
using Microsoft.Expression.Interactivity.Core;
using Mvvm;
using Newtonsoft.Json;

namespace Districts.ViewModel.TabsVM
{
    internal class ManageViewModel : BindableBase
    {
        public ManageViewModel()
        {
            LoadCommand = new ActionCommand(OnLoadCommand);
            SearchCommand = new ActionCommand(OnSearch);
            CancelSearchCommand = new ActionCommand(OnClearSearch);
            SaveCommand = new ActionCommand(OnSave);
            EditRecord = new ActionCommand(OnEdit);
        }

        /// <summary>
        ///     Нужно для пробрасывания вглубь модели
        /// </summary>
        /// <param name="obj"></param>
        private void OnEdit(object obj)
        {
            if (obj is ManageRecordViewModel record)
            {
                record.EditCommand?.Execute(_savedNames);
                // могли добавить имена, переписываем их
                RewriteNames();
            }
        }

        /// <summary>
        ///     Содержит ли одно из полей строку поиска
        /// </summary>
        /// <param name="search">Что ищем</param>
        /// <param name="fields">Где ищем</param>
        /// <returns></returns>
        private bool CanFind(string search, params string[] fields)
        {
            // Подходит все, строка пустая
            if (string.IsNullOrEmpty(search))
                return true;


            foreach (var field in fields)
            {
                if (string.IsNullOrEmpty(field)
                    || field.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) < 0)
                    continue;

                // что то нашли, выводим результат
                return true;
            }

            // ничего не нашли, выходим
            return false;
        }

        private void SortByDate(CardManagement record)
        {
            if (record == null || !record.Actions.Any())
                return;

            record.Actions = record.Actions.OrderBy(x => x.Date).ToList();
        }

        private void SortByCondition(IEnumerable<ManageRecordViewModel> source)
        {
            var result = source;

            // если не стоит флаг "все"
            if ((SortingType & SortingType.All) != SortingType.All)
            {
                // сортируем только несипользуемые
                if ((SortingType & SortingType.Droppd) == SortingType.Droppd) result = result.Where(x => !x.HasOwner());

                // только те, что на руках
                if ((SortingType & SortingType.InUse) == SortingType.InUse) result = result.Where(x => x.HasOwner());


                // берем ограничение по времени
                if ((SortingType & SortingType.Diff1Y) == SortingType.Diff1Y)
                {
                    var span = TimeSpan.FromDays(365);

                    // пытаемся сделать промежуток максимально мелким
                    if ((SortingType & SortingType.Diff5M) == SortingType.Diff5M)
                        span = TimeSpan.FromDays(30 * 5);

                    if ((SortingType & SortingType.Diff3M) == SortingType.Diff3M)
                        span = TimeSpan.FromDays(30 * 3);

                    result = result.Where(x => x.HasTimeDifference(span, (SortingType & SortingType.Droppd) != 0));
                }
            }

            Cards = new ObservableCollection<ManageRecordViewModel>(result);
        }

        /// <summary>
        ///     Заполняем имена
        /// </summary>
        private void RewriteNames()
        {
            // заполнили именами
            var hasSet = new HashSet<string>();
            foreach (var card in _mappedCards.Values)
            foreach (var action in card.Actions)
            {
                var name = action.Subject;
                if (string.IsNullOrEmpty(name) || hasSet.Contains(name))
                    continue;

                hasSet.Add(name);
            }

            _savedNames = new List<string>(hasSet);
        }

        #region Fields

        private List<string> _savedNames = new List<string>();
        private string _searchText;
        private readonly Dictionary<Card, CardManagement> _mappedCards = new Dictionary<Card, CardManagement>();

        private ObservableCollection<ManageRecordViewModel> _cards = new ObservableCollection<ManageRecordViewModel>();
        private SortingType _sortingType = SortingType.All;

        #endregion

        #region Commands

        public ICommand LoadCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand CancelSearchCommand { get; set; }
        public ICommand EditRecord { get; set; }

        #endregion

        #region Properties

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value == _searchText) return;
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ManageRecordViewModel> Cards
        {
            get => _cards;
            set
            {
                if (Equals(value, _cards)) return;
                _cards = value;
                OnPropertyChanged();
            }
        }

        public SortingType SortingType
        {
            get => _sortingType;
            set
            {
                if (value == _sortingType) return;
                _sortingType = value;
                OnPropertyChanged();

                OnSearch();
            }
        }

        #endregion

        #region ActionCommand Handlers

        private void OnClearSearch()
        {
            SearchText = null;
            OnSearch();
        }

        private void OnSave()
        {
            var dictPath = ApplicationSettings.ReadOrCreate().ManageRecordsPath;

            foreach (var record in _mappedCards.Values)
            {
                SortByDate(record);
                var file = Path.Combine(dictPath, record.Number.ToString());
                var json = JsonConvert.SerializeObject(record, Formatting.Indented);
                try
                {
                    File.WriteAllText(file, json);
                }
                catch (Exception e)
                {
                    Tracer.Tracer.WriteError(e);
                }
            }

            OnLoadCommand(false);
        }

        private void OnSearch()
        {
            // строки поиска нет, выводим всё
            if (string.IsNullOrEmpty(SearchText))
            {
                SortByCondition(_mappedCards.Select(x => new ManageRecordViewModel(x.Value)));
            }
            else
            {
                var result = new List<ManageRecordViewModel>();
                foreach (var card in Cards)
                {
                    var taken = card.TaskenDate?.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var dropped = card.DroppedTime?.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                    if (CanFind(SearchText, taken, dropped, card.Number.ToString(), card.LastOwner))
                        result.Add(card);
                }

                SortByCondition(result);
            }
        }

        private void OnLoadCommand(object obj)
        {
            _mappedCards.Clear();
            Cards.Clear();
            SearchText = null;
            _savedNames.Clear();

            if (true.Equals(obj))
            {
                var cards = LoadingWork.LoadCards().Select(x => x.Value);
                var records = LoadingWork.LoadManageElements().Select(x => x.Value);

                foreach (var card in cards)
                {
                    var find = records.FirstOrDefault(x => x.Number == card.Number)
                               ?? new CardManagement {Number = card.Number};
                    SortByDate(find);
                    _mappedCards.Add(card, find);
                }

                //_allcards = records.ToList();
                Cards = new ObservableCollection<ManageRecordViewModel>(
                    _mappedCards
                        .Select(x => new ManageRecordViewModel(x.Value)));

                RewriteNames();
            }
        }

        #endregion
    }
}