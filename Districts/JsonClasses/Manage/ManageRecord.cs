using System;
using System.ComponentModel;

namespace Districts.JsonClasses.Manage
{
    public enum ActionTypes
    {
        /// <summary>
        ///     Участок взят
        /// </summary>
        [Description("взят")] Taken,

        /// <summary>
        ///     Участок возвращён
        /// </summary>
        [Description("сдан")] Dropped
    }

    public class ManageRecord
    {
        /// <summary>
        ///     Время записи
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Автор записи
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        ///     Тип события
        /// </summary>
        public ActionTypes ActionType { get; set; }

        #region Equality members

        protected bool Equals(ManageRecord other)
        {
            return Date.Equals(other.Date)
                   && string.Equals(Subject, other.Subject)
                   && ActionType == other.ActionType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ManageRecord) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        #endregion
    }
}