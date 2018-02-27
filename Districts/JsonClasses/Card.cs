using System.Collections.Generic;
using System.Linq;

namespace Districts.JsonClasses
{
    /// <summary>
    /// Информацияч о карточке
    /// </summary>
    public class Card 
    {
        /// <summary>
        /// Hомер кварточки
        /// </summary>
        public int  Number { get; set; }
        /// <summary>
        /// Список всеХ квартир в участке
        /// </summary>
        public List<Door> Doors { get; set; } = new List<Door>();

        public override bool Equals(object obj)
        {
            if (obj is Card x)
            {
                return x.Number == Number
                       && Doors.SequenceEqual(x.Doors);
            }

            return false;
        }

        public static bool operator == (Card x, Card y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(Card x, Card y)
        {
            return !(x == y);
        }
    }
}
