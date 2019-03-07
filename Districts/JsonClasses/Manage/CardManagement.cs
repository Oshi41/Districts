using System;
using System.Collections.Generic;
using System.Linq;
using Districts.Helper;

namespace Districts.JsonClasses.Manage
{
    /// <summary>
    ///     Записи использования карточки
    /// </summary>
    public class CardManagement : ICloneable<CardManagement>
    {
        /// <summary>
        ///     Номер карточки
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        ///     События
        /// </summary>
        public List<ManageRecord> Actions { get; set; } = new List<ManageRecord>();

        ///// <summary>
        ///// Владелец в данный момент
        ///// </summary>
        ///// <returns></returns>
        //public string GetCurrentOwner()
        //{
        //    if (!Actions.Any() || !HasOwner())
        //        return string.Empty;

        //    return GetLastOwner();
        //}
        ///// <summary>
        ///// Последний у кого была на рукаъ
        ///// </summary>
        ///// <returns></returns>
        //public string GetLastOwner()
        //{
        //    return Actions?.LastOrDefault()?.Subject;
        //}

        ///// <summary>
        ///// Находим последние даты использования
        ///// </summary>
        ///// <param name="begin"></param>
        ///// <param name="end"></param>
        //public void FindLastDates(out DateTime begin, out DateTime end)
        //{
        //    begin = DateTime.MinValue;
        //    end = DateTime.MinValue;

        //    var find = Actions.LastOrDefault();
        //    if (find == null)
        //        return;

        //    if (find.ActionType == ActionTypes.Taken)
        //    {
        //        begin = find.Date;
        //        end = DateTime.MaxValue;
        //        return;
        //    }

        //    end = find.Date;
        //    var name = find.Subject;
        //    var reversedList = Actions.ToList();
        //    reversedList.Reverse();

        //    var prevTaken = reversedList.FirstOrDefault(x => x.Subject == name && x.ActionType == ActionTypes.Taken);
        //    if (prevTaken == null)
        //    {
        //        // ошибка, не занесли запись того, когда взяли
        //        var logger = new Logger();
        //        logger.AddMessage("Oшибка, не занесли запись того, когда взяли участок!");
        //        logger.WriteToFile();
        //    }

        //    begin = prevTaken.Date;
        //}

        public CardManagement Clone()
        {
            var copied = Actions.Select(x => new ManageRecord
            {
                ActionType = x.ActionType,
                Date = x.Date,
                Subject = x.Subject.ToString()
            });

            var copy = new CardManagement
            {
                Actions = copied.ToList(),
                Number = Number
            };

            return copy;
        }

        /// <summary>
        ///     Есть ли на руках у кого нибудь
        /// </summary>
        /// <returns></returns>
        public bool HasOwner()
        {
            return Actions.Any() && Actions.LastOrDefault()?.ActionType == ActionTypes.Taken;
        }

        public DateTime? GetLastDroppedTime()
        {
            var find = Actions.FindLast(x => x.ActionType == ActionTypes.Dropped);
            return find?.Date;
        }

        public DateTime? GetLastTakenTime(string name = "")
        {
            Predicate<ManageRecord> nameCondition = record =>
                record.Subject.Equals(name, StringComparison.InvariantCultureIgnoreCase);

            Predicate<ManageRecord> mainCondition = record =>
                record.ActionType == ActionTypes.Taken;

            // если пришло пустое имя, ищем без этого условия
            var find = Actions.FindLast(x => mainCondition(x) &&
                                             (string.IsNullOrWhiteSpace(name)
                                              || nameCondition(x)));

            return find?.Date;
        }

        /// <summary>
        ///     Возвращает имя владельца, null, если никто не держит карточку на руках
        /// </summary>
        /// <returns></returns>
        public string GetOwner()
        {
            return HasOwner()
                ? Actions.LastOrDefault().Subject
                : null;
        }

        #region Equality members

        protected bool Equals(CardManagement other)
        {
            return Number == other.Number
                   && Actions.IsTermwiseEquals(other.Actions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CardManagement) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        #endregion
    }
}