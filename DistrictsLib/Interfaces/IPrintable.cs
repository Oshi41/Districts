using System.Collections.Generic;
using DistrictsLib.Legacy.JsonClasses;

namespace DistrictsLib.Interfaces
{
    public interface IPrintable
    {
        /// <summary>
        /// Печать всех карточек
        /// </summary>
        /// <param name="cards"></param>
        void Print(IList<Card> cards);
    }
}
