using System.Collections.Generic;
using System.Linq;
using DistrictsLib.Extentions;
using DistrictsLib.Interfaces;
using DistrictsLib.Legacy.Comparers;
using DistrictsLib.Legacy.JsonClasses;

namespace DistrictsLib.Implementation
{
    class SortedRepare : ICardRepare
    {
        private readonly bool _isSorted;

        public SortedRepare(bool isSorted)
        {
            _isSorted = isSorted;
        }

        #region Implementation of ICardRepare

        public bool? Repare(IList<Card> cards)
        {
            var comparer = new HouseNumberComparerFromDoor(new HouseNumberComparerFromString());

            foreach (var card in cards)
            {
                if (_isSorted)
                {
                    card.Doors.Sort(comparer);
                }
                else
                {
                    card.Doors = card.Doors.Shuffle().ToList();
                }
            }

            return true;
        }

        #endregion
    }
}
