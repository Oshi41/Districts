using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Districts.JsonClasses.Manage;
using Districts.MVVM;

namespace Districts.ViewModel.Manage
{
    class ManageRecordEditViewModel : ObservableObject
    {
        #region Fields

        private ObservableCollection<ActionViewModel> _actions;
        private DateTime? _date;
        private ActionTypes? _actionType;
        private string _subject;
        private int _number;
        private bool _isTaken;
        private ObservableCollection<string> _names;
        private SortingType _sortingType;

        #endregion

        #region Properties

        public bool IsTaken
        {
            get { return _isTaken; }
            set
            {
                if (value == _isTaken) return;
                _isTaken = value;
                OnPropertyChanged();
            }
        }
        public int Number
        {
            get { return _number; }
            set
            {
                if (value == _number) return;
                _number = value;
                OnPropertyChanged();
            }
        }
        public ICommand DropCardCommand { get; set; }
        public Command AddAction { get; set; }
        public ICommand RemoveAction { get; set; }
        public ObservableCollection<ActionViewModel> Actions
        {
            get { return _actions; }
            set
            {
                if (Equals(value, _actions)) return;
                _actions = value;
                OnPropertyChanged();
            }
        }


        public DateTime? Date
        {
            get { return _date; }
            set
            {
                if (value.Equals(_date)) return;
                _date = value;
                OnPropertyChanged();
            }
        }
        public ActionTypes? ActionType
        {
            get { return _actionType; }
            set
            {
                if (value == _actionType) return;
                _actionType = value;
                OnPropertyChanged();
            }
        }
        public string Subject
        {
            get { return _subject; }
            set
            {
                if (value == _subject) return;
                _subject = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Names
        {
            get { return _names; }
            set
            {
                if (Equals(value, _names)) return;
                _names = value;
                OnPropertyChanged(nameof(Names));
            }
        }

        #endregion

        public ManageRecordEditViewModel(CardManagement card, List<string> names)
        {
            if (card == null)
                return;

            Names = new ObservableCollection<string>(names);
            Number = card.Number;
            Actions = new ObservableCollection<ActionViewModel>(card.Actions.Select(x => new ActionViewModel(x)));
            IsTaken = card.HasOwner();
            DropCardCommand = new Command(OnDropCommand);
            AddAction = new Command(OnAddAction, OnCanAddCommand);
            DropCardCommand = new Command(OnDropCommand);
            RemoveAction = new Command(OnRemoveAction);
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
            if (obj is ActionViewModel model)
            {
                Actions.Remove(model);
            }
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


        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            InvalidateCommands();
        }
    }

    
    [Flags]
    enum SortingType
    {
        [Description("Не на руках")]
        Droppd = 1,
        [Description("На руках")]
        InUse = 2,

        Diff3M = 4 | Diff5M,
        Diff5M = 8 | Diff1Y,
        Diff1Y = 16,

        [Description("На руках более 3 месяцев")]
        InUse3M = InUse | Diff3M,
        [Description("На руках более 5 месяцев")]
        InUse5M = InUse | Diff5M,
        [Description("На руках более года")]
        InUse1Y = InUse | Diff1Y,

        [Description("Брали 3 месяца назад")]
        Droppd3M = Droppd | Diff3M,
        [Description("Брали 5 месяцев назад")]
        Droppd5M = Droppd | Diff5M,
        [Description("Брали более года назад")]
        Droppd1Y = Droppd | Diff1Y,

        [Description("Все")]
        All = Droppd | InUse
    }
}
