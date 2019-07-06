using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Extentions;
using DistrictsLib.Legacy.Comparers;
using DistrictsLib.Legacy.JsonClasses;

namespace DistrictsLib.Legacy.Helping
{
    internal class CardWorker : List<Card>
    {
        private int _capacity;
        private int _innerIndex;

        public CardWorker(int count = int.MaxValue)
            : base(count)
        {
            for (var i = 0; i < count; i++)
            {
                Add(new Card { Number = (i + 1).ToString() });
            }
        }

        private void AddCounter()
        {
            // ограничиваем верхние пределы
            if (_innerIndex >= Count)
                _innerIndex = 0;
            else
                _innerIndex++;

            // нашли ближайшую доступную карточку
            _innerIndex = FindIndex(_innerIndex, x => x.Doors.Count < _capacity);

            // если не нашли выше, ищем по всему массиву
            if (_innerIndex < 0)
                _innerIndex = FindIndex(x => x.Doors.Count < _capacity);
        }

        public CardWorker SetCardCapacity(int capacity)
        {
            _capacity = capacity;
            return this;
        }

        public void Add(Door item)
        {
            if (_innerIndex < 0)
            {
                // пытаемся добавить ещё одну карточку
                if (Count >= _capacity)
                    return;

                Add(new Card { Number = (Count + 1).ToString() });
                _innerIndex = Count - 1;
            }


            this[_innerIndex].Doors.Add(item);
            AddCounter();
        }

        public void ShuffleDoors()
        {
            foreach (var card in this)
            {
                card.Doors = card.Doors.Shuffle().ToList();
            }
        }

        public void SortDoors()
        {
            var comparer = new HouseNumberComparerFromDoor(new HouseNumberComparerFromString());

            foreach (var card in this)
            {
                card.Doors = card.Doors.OrderBy(door => door, comparer).ToList();
            }
        }
    }
}
