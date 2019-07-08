using System;
using System.Collections.Generic;
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
            : this(record, new List<string>())
        {
        }

        public ManageRecordViewModel(IManageRecord record, 
            IEnumerable<string> subjects)
        {
            if (record != null)
            {
                _date = record.Date;
                _subject = record.Subject;
                _actionType = record.ActionType;
            }

            AvailableSubjects = new List<string>(subjects);
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

        public IList<string> AvailableSubjects { get; }
    }
}
