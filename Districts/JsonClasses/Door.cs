using System.Collections.Generic;
using Districts.JsonClasses.Base;

namespace Districts.JsonClasses
{
    /// <summary>
    /// Класс описывающий квартиру
    /// </summary>
    public class Door : BaseFindableObject
    {
        /// <summary>
        /// Номер квартиры
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Примерный подъезд
        /// </summary>
        public int Entrance { get; set; }
        /// <summary>
        /// Коды
        /// </summary>
        public List<string> Codes { get; set; } = new List<string>();

        public Door(string street, string houseNumber) : base(street, houseNumber)
        {
        }

        #region Overrided

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            if (obj is Door x)
            {
                return x.Number == Number;
            }

            return false;
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
