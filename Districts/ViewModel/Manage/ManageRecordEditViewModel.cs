using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Districts.JsonClasses.Manage;
using Mvvm;
using Mvvm.Commands;

namespace Districts.ViewModel.Manage
{
    internal class ManageRecordEditViewModel : BindableBase
    {
        public ManageRecordEditViewModel(CardManagement card, List<string> names)
        {
            if (card == null)
                return;

            Names = new ObservableCollection<string>(names);
            Number = card.Number;
            Actions = new ObservableCollection<ActionViewModel>(card.Actions.Select(x => new ActionViewModel(x)));
            IsTaken = card.HasOwner();
            DropCardCommand = new DelegateCommand<object>(OnDropCommand);
            AddAction = new DelegateCommand<object>(OnAddAction, OnCanAddCommand);
            DropCardCommand = new DelegateCommand<object>(OnDropCommand);
            RemoveAction = new DelegateCommand<object>(OnRemoveAction, OnCanRemoveAction);
        }

        private bool OnCanRemoveAction(object obj)
        {
            return SelectedAction != null;
        }

        public CardManagement ToCard()
        {
            return new CardManagement
            {
                Actions = Actions.Select(x => x.ToRecord()).ToList(),
                Number = Number
            };
        }


        private void OnRemoveAction(object obj)
        {
            if (obj is ActionViewModel model) Actions.Remove(model);
        }

        private bool OnCanAddCommand(object obj)
        {
            var noname = string.IsNullOrEmpty(Subject);
            var nodate = !Date.HasValue;
            var noaction = !ActionType.HasValue;

            var error = noname || nodate || noaction;

            return !error;
        }

        private void OnAddAction(object obj)
        {
            if (!OnCanAddCommand(obj))
                return;

            var record = new ManageRecord
            {
                ActionType = ActionType.Value,
                Date = Date.Value,
                Subject = Subject
            };

            Actions.Add(new ActionViewModel(record));

            // добавили имя для подсказки
            if (!Names.Contains(record.Subject))
                Names.Add(record.Subject);
        }

        private void OnDropCommand(object obj)
        {
            var lastOwner = Actions.OrderBy(x => x.Date).LastOrDefault(x => x.ActionType == ActionTypes.Taken)?.Subject;
            var record = new ManageRecord
            {
                ActionType = ActionTypes.Dropped,
                Date = DateTime.Today,
                Subject = lastOwner
            };

            Actions.Add(new ActionViewModel(record));
            IsTaken = false;
        }

        #region Fields

        private ObservableCollection<ActionViewModel> _actions;
        private DateTime? _date;
        private ActionTypes? _actionType;
        private string _subject;
        private int _number;
        private bool _isTaken;
        private ObservableCollection<string> _names;
        private SortingType _sortingType;
        private ActionViewModel _selectedAction;

        #endregion

        #region Properties

        public bool IsTaken
        {
            get => _isTaken;
            set => SetProperty(ref _isTaken , value);
        }

        public int Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        public DelegateCommandBase DropCardCommand { get; set; }
        public DelegateCommandBase AddAction { get; set; }
        public DelegateCommandBase RemoveAction { get; set; }

        public ObservableCollection<ActionViewModel> Actions
        {
            get => _actions;
            set => SetProperty(ref _actions, value);
        }

        public ActionViewModel SelectedAction
        {
            get => _selectedAction;
            set => SetProperty(ref _selectedAction, value);
        }


        public DateTime? Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public ActionTypes? ActionType
        {
            get => _actionType;
            set => SetProperty(ref _actionType, value);
        }

        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        public ObservableCollection<string> Names
        {
            get => _names;
            set => SetProperty(ref _names, value);
        }

        #endregion

        protected override bool SetProperty<T>(ref T storage, T value, string propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);

            DropCardCommand.RaiseCanExecuteChanged();
            AddAction.RaiseCanExecuteChanged();
            RemoveAction.RaiseCanExecuteChanged();

            return result;
        }
    }


    [Flags]
    internal enum SortingType
    {
        [Description("Не на руках")] Droppd = 1,
        [Description("На руках")] InUse = 2,

        Diff3M = 4 | Diff5M,
        Diff5M = 8 | Diff1Y,
        Diff1Y = 16,

        [Description("На руках более 3 месяцев")]
        InUse3M = InUse | Diff3M,

        [Description("На руках более 5 месяцев")]
        InUse5M = InUse | Diff5M,
        [Description("На руках более года")] InUse1Y = InUse | Diff1Y,

        [Description("Брали 3 месяца назад")] Droppd3M = Droppd | Diff3M,
        [Description("Брали 5 месяцев назад")] Droppd5M = Droppd | Diff5M,

        [Description("Брали более года назад")]
        Droppd1Y = Droppd | Diff1Y,

        [Description("Все")] All = Droppd | InUse
    }
}