using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Districts.JsonClasses.Manage;
using Districts.Views.Management;
using Microsoft.Expression.Interactivity.Core;
using Mvvm;

namespace Districts.ViewModel.Manage
{
    public class ManageRecordViewModel : BindableBase
    {
        public ManageRecordViewModel(CardManagement card)
        {
            if (card == null)
                return;

            _card = card;
            Number = _card.Number;
            UpdateByLastRecord(_card.Actions.LastOrDefault());

            EditCommand = new ActionCommand(OnEdit);
        }

        #region Fields

        // Сохраняем ссылку на объект, чтобы его же потом и менять
        private readonly CardManagement _card;
        private static readonly DateTime _innerTime = DateTime.Now;

        private DateTime? _taskenDate;
        private string _lastOwner;
        private DateTime? _droppedTime;
        private int _number;

        #endregion

        #region Properties

        public int Number
        {
            get => _number;
            set
            {
                if (value == _number) return;
                _number = value;
                OnPropertyChanged();
            }
        }

        public DateTime? TaskenDate
        {
            get => _taskenDate;
            set
            {
                if (value.Equals(_taskenDate)) return;
                _taskenDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime? DroppedTime
        {
            get => _droppedTime;
            set
            {
                if (value.Equals(_droppedTime)) return;
                _droppedTime = value;
                OnPropertyChanged();
            }
        }

        public string LastOwner
        {
            get => _lastOwner;
            set
            {
                if (value == _lastOwner) return;
                _lastOwner = value;
                OnPropertyChanged();
            }
        }

        public ICommand EditCommand { get; set; }

        #endregion

        #region Methods

        private void UpdateByLastRecord(ManageRecord record)
        {
            if (record == null)
            {
                LastOwner = null;
                TaskenDate = null;
                DroppedTime = null;
                return;
            }

            LastOwner = record.Subject;
            if (record.ActionType == ActionTypes.Dropped)
            {
                TaskenDate = _card.GetLastTakenTime(LastOwner);
                DroppedTime = record.Date;
            }
            else
            {
                TaskenDate = record.Date;
                DroppedTime = null;
            }
        }


        #region Public

        /// <summary>
        ///     Нужно для редактирования
        /// </summary>
        /// <returns></returns>
        public CardManagement CopyManagementCard()
        {
            return _card.Clone();
        }

        public void OnEdit(object param)
        {
            var names = param as List<string> ?? new List<string>();
            // показываю окно
            var vm = new ManageRecordEditViewModel(_card, names);
            var window = new ManageView {DataContext = vm};

            if (window.ShowDialog() == true)
            {
                _card.Actions = vm.Actions.Select(x => x.ToRecord()).ToList();
                UpdateByLastRecord(_card.Actions.LastOrDefault());

                // Добавляем новые имена
                //foreach (var action in vm.Actions)
                //{
                //    if (!_names.Contains(action.Subject))
                //        _names.Add(action.Subject);
                //}
            }

            // номер все равно изменить не можем
        }

        public bool HasOwner()
        {
            return _card.HasOwner();
        }

        public bool HasTimeDifference(TimeSpan span, bool nullValue)
        {
            if (!TaskenDate.HasValue)
                return nullValue;

            var droppedTime = DroppedTime ?? _innerTime;
            return span <= droppedTime - TaskenDate;
        }

        #endregion

        #endregion
    }

    //public class ManageRecordViewModel : BindableBase
    //{
    //    private readonly CardManagement _card;
    //    private int _number;
    //    private List<ManageRecord> _actions;
    //    private ActionTypes? _actionType;
    //    private DateTime? _date;
    //    private string _subject;

    //    public int Number
    //    {
    //        get { return _number; }
    //        set
    //        {
    //            if (value == _number) return;
    //            _number = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //    public ActionTypes? ActionType
    //    {
    //        get { return _actionType; }
    //        set
    //        {
    //            if (value == _actionType) return;
    //            _actionType = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //    public string Subject
    //    {
    //        get { return _subject; }
    //        set
    //        {
    //            if (value == _subject) return;
    //            _subject = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //    public DateTime? Date
    //    {
    //        get { return _date; }
    //        set
    //        {
    //            if (value.Equals(_date)) return;
    //            _date = value;
    //            OnPropertyChanged();
    //        }
    //    }


    //    public ICommand EditCommand { get; set; }
    //    public ICommand SaveActionCommand { get; set; }


    //    public ManageRecordViewModel(CardManagement card)
    //    {
    //        _card = card;
    //        if (card == null)
    //            return;

    //        Number = card.Number;
    //        _actions = card.Actions;
    //        UpdateByLastAction(_actions.LastOrDefault());

    //        EditCommand = new ActionCommand(OnEdit);
    //        SaveActionCommand = new ActionCommand(OnSave);
    //    }

    //    private void OnSave()
    //    {

    //    }

    //    private void OnEdit()
    //    {
    //        // call windoow
    //    }

    //    public CardManagement ToCardrecord()
    //    {
    //        var temp = new CardManagement
    //        {
    //            Number = Number,
    //            Actions = _actions
    //        };
    //        return temp;
    //    }

    //    private void UpdateByLastAction(ManageRecord last)
    //    {
    //        ActionType = last?.ActionType;
    //        Date = last?.Date;
    //        Subject = last?.Subject;

    //    }

    //}
    //class ManagementRecord : BindableBase
    //{
    //private int _number;
    //private DateTime? _first;
    //private DateTime? _last;
    //private string _owner;
    //private bool _isTaken;

    //public int Number
    //{
    //    get { return _number; }
    //    set
    //    {
    //        if (value == _number) return;
    //        _number = value;
    //        OnPropertyChanged();
    //    }
    //}

    //public DateTime? First
    //{
    //    get { return _first; }
    //    set
    //    {
    //        if (value.Equals(_first)) return;
    //        _first = value;
    //        OnPropertyChanged();
    //    }
    //}

    //public DateTime? Last
    //{
    //    get { return _last; }
    //    set
    //    {
    //        if (value.Equals(_last)) return;
    //        _last = value;
    //        OnPropertyChanged();
    //    }
    //}

    //public string Owner
    //{
    //    get { return _owner; }
    //    set
    //    {
    //        if (value == _owner) return;
    //        _owner = value;
    //        OnPropertyChanged();
    //    }
    //}

    //public bool IsTaken
    //{
    //    get { return _isTaken; }
    //    set
    //    {
    //        if (value == _isTaken) return;
    //        _isTaken = value;
    //        OnPropertyChanged();
    //    }
    //}

    //public ManagementRecord(CardManagement card)
    //    {
    //        Number = card.Number;
    //        IsTaken = card.HasOwner();
    //        Owner = card.GetLastOwner();

    //        card.FindLastDates(out var begin, out var end);

    //        if (begin != DateTime.MaxValue || begin != DateTime.MinValue)
    //            First = begin;
    //        if (end != DateTime.MaxValue || end != DateTime.MinValue)
    //            Last = end;
    //    }
    //}
}