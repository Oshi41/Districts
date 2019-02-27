using System;
using Districts.JsonClasses.Manage;
using Mvvm;

namespace Districts.ViewModel.Manage
{
    internal class ActionViewModel : BindableBase
    {
        private ActionTypes _actionType;
        private DateTime _date;
        private string _subject;

        public ActionViewModel(ManageRecord record)
        {
            if (record == null)
                return;

            ActionType = record.ActionType;
            Date = record.Date;
            Subject = record.Subject;
        }

        public ManageRecord ToRecord()
        {
            return new ManageRecord
            {
                ActionType = ActionType,
                Subject = Subject,
                Date = Date
            };
        }

        #region Properties

        public string Subject
        {
            get => _subject;
            set
            {
                if (value == _subject) return;
                _subject = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                if (value.Equals(_date)) return;
                _date = value;
                OnPropertyChanged();
            }
        }

        public ActionTypes ActionType
        {
            get => _actionType;
            set
            {
                if (value == _actionType) return;
                _actionType = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}