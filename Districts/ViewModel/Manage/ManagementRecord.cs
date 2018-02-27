using System;
using Districts.JsonClasses.Manage;
using Districts.MVVM;

namespace Districts.ViewModel.Manage
{
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
