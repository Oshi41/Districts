using System;
using Districts.JsonClasses.Manage;
using Districts.MVVM;

namespace Districts.ViewModel.Manage
{
    class ManageRecordViewModel : ObservableObject
    {
        // Сохраняем ссылку на объект, чтобы его же потом и менять
        private readonly CardManagement _card;
        private DateTime _taskenDate;
        private string _lastOwner;
        private DateTime _droppedTime;
        private int _number;

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
        public DateTime TaskenDate
        {
            get { return _taskenDate; }
            set
            {
                if (value.Equals(_taskenDate)) return;
                _taskenDate = value;
                OnPropertyChanged();
            }
        }
        public DateTime DroppedTime
        {
            get { return _droppedTime; }
            set
            {
                if (value.Equals(_droppedTime)) return;
                _droppedTime = value;
                OnPropertyChanged();
            }
        }
        public string LastOwner
        {
            get { return _lastOwner; }
            set
            {
                if (value == _lastOwner) return;
                _lastOwner = value;
                OnPropertyChanged();
            }
        }


        public ManageRecordViewModel(CardManagement card)
        {
            if (_card == null)
                return;

            _card = card;
            Number = _card.Number;
        }

        private void UpdateByLastRecord()
        {

        }

        public void OnEdit()
        {
            var copy = _card.Clone();

            // показываю окно

            // if (result == true)
            _card.Actions = copy.Actions;
            // номер все равно изменить не можем

        }


    }
    //public class ManageRecordViewModel : ObservableObject
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
    //    public ICommand SaveCommand { get; set; }


    //    public ManageRecordViewModel(CardManagement card)
    //    {
    //        _card = card;
    //        if (card == null)
    //            return;

    //        Number = card.Number;
    //        _actions = card.Actions;
    //        UpdateByLastAction(_actions.LastOrDefault());

    //        EditCommand = new Command(OnEdit);
    //        SaveCommand = new Command(OnSave);
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
    //class ManagementRecord : ObservableObject
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
