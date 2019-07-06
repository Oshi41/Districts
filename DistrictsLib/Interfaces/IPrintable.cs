using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
