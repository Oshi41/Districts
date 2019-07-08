using System;
using DistrictsLib.Extentions;
using DistrictsLib.Interfaces.Json;
using DistrictsLib.Legacy.JsonClasses.Manage;
using Mvvm;

namespace DistrictsNew.ViewModel.Manage
{
    class ManageRecordViewModel : BindableBase, IManageRecord
    {
        private DateTime _date;
        private string _subject;
        private ActionTypes _actionType;

        public ManageRecordViewModel(IManageRecord record)
        {
            if (record != null)
            {
                _date = record.Date;
                _subject = record.Subject;
                _actionType = record.ActionType;
            }
        }

        /// <summary>
        ///     Время записи
        /// </summary>
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public string DateStr => Date.ToPrettyString();

        /// <summary>
        ///     Автор записи
        /// </summary>
        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        /// <summary>
        ///     Тип события
        /// </summary>
        public ActionTypes ActionType
        {
            get => _actionType;
            set => SetProperty(ref _actionType, value);
        }
    }
}
