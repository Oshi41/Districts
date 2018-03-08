using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.JsonClasses.Manage;
using Districts.MVVM;

namespace Districts.ViewModel.Manage
{
    class ActionViewModel : ObservableObject
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
            get { return _subject; }
            set
            {
                if (value == _subject) return;
                _subject = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (value.Equals(_date)) return;
                _date = value;
                OnPropertyChanged();
            }
        }

        public ActionTypes ActionType
        {
            get { return _actionType; }
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
