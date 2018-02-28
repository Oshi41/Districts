﻿using System;

namespace Districts.JsonClasses.Manage
{
    public enum ActionTypes
    {
        /// <summary>
        /// Участок взят
        /// </summary>
        Taken,
        /// <summary>
        /// Участок возвращён
        /// </summary>
        Dropped,
    }

    public class ManageRecord
    {
        /// <summary>
        /// Время записи
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Автор записи
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Тип события
        /// </summary>
        public ActionTypes ActionType { get; set; }
    }
}
