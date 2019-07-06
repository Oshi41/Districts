﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Legacy.JsonClasses;

namespace DistrictsLib.Interfaces
{
    public interface ICardRepare
    {
        /// <summary>
        /// Чинит переданные карточки
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        bool? Repare(IList<Card> cards);
    }
}
