using System.Collections.Generic;
using Districts.JsonClasses.Base;

namespace Districts.JsonClasses
{
    public class ForbiddenElement : BaseFindableObject
    {
        /// <summary>
        /// Агрессиии
        /// </summary>
        public List<int> Aggressive { get; set; } = new List<int>();
        /// <summary>
        /// Просили не беспокоить
        /// </summary>
        public List<int> NoWorried { get; set; } = new List<int>();
        /// <summary>
        /// Не посещать
        /// </summary>
        public List<int> NoVisit { get; set; } = new List<int>();


        public ForbiddenElement(string street, string houseNumber) : base(street, houseNumber)
        {
        }
    }
}
