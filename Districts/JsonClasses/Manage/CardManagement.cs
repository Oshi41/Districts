using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Districts.Helper;

namespace Districts.JsonClasses.Manage
{
    /// <summary>
    /// Записи использования карточки
    /// </summary>
    public class CardManagement
    {
        /// <summary>
        /// Номер карточки
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// События
        /// </summary>
        public List<ManageRecord> Actions { get; set; } = new List<ManageRecord>();

        /// <summary>
        /// Есть ли на руках у кого нибудь
        /// </summary>
        /// <returns></returns>
        public bool HasOwner()
        {
            return Actions.Any() && Actions.LastOrDefault()?.ActionType == ActionTypes.Taken;
        }

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
    }
}
