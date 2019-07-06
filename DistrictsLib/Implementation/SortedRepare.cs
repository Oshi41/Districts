using System.Collections.Generic;
using DistrictsLib.Interfaces;
using DistrictsLib.Legacy.Comparers;
using DistrictsLib.Legacy.JsonClasses;

namespace DistrictsLib.Implementation
{
    class SortedRepare : ICardRepare
    {
        #region Implementation of ICardRepare

        public bool? Repare(IList<Card> cards)
        {
            var comparer = new HouseNumberComparerFromDoor(new HouseNumberComparerFromString());

            foreach (var card in cards)
            {
                card.Doors.Sort(comparer);
            }

            return true;
        }

        #endregion
    }
}
