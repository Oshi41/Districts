using System;
using System.ComponentModel;
using DistrictsLib.Interfaces.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DistrictsLib.Legacy.JsonClasses.Manage
{
    public enum ActionTypes
    {
        /// <summary>
        ///     Участок взят
        /// </summary>
        Taken,

        /// <summary>
        ///     Участок возвращён
        /// </summary>
        Dropped,

        /// <summary>
        /// Утерян
        /// </summary>
        Lost,
    }

    public class ManageRecord : IManageRecord
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
        [JsonConverter(typeof(StringEnumConverter))]
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