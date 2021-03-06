﻿using System.Collections.Generic;
using Districts.Helper;
using Districts.JsonClasses.Base;

namespace Districts.JsonClasses
{
    /// <summary>
    ///     Класс описывающий квартиру
    /// </summary>
    public class Door : BaseFindableObject
    {
        public Door(BaseFindableObject obj) : base(obj)
        {
        }

        public Door()
        {
        }

        /// <summary>
        ///     Номер квартиры
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        ///     Примерный подъезд
        /// </summary>
        public int Entrance { get; set; }

        /// <summary>
        ///     Коды
        /// </summary>
        public List<string> Codes { get; set; } = new List<string>();

        #region Overrided

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            if (obj is Door x)
                return Equals(x);

            return false;
        }

        protected bool Equals(Door other)
        {
            return base.Equals(other)
                   && Number == other.Number
                   && Entrance == other.Entrance
                   && Codes.IsTermwiseEquals(other.Codes);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Number;
                hashCode = (hashCode * 397) ^ Entrance;
                return hashCode;
            }
        }

        public static bool operator ==(Door x, Door y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(Door x, Door y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return Street + " " + HouseNumber + " " + Number;
        }

        #endregion
    }
}