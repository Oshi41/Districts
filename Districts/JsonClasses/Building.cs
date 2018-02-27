using System;
using System.Collections.Generic;
using Districts.JsonClasses.Base;

namespace Districts.JsonClasses
{
    /// <summary>
    /// Информация о здании
    /// </summary>
    public class Building : BaseFindableObject
    {
        /// <summary>
        /// Кол-во квартир
        /// </summary>
        public int Doors { get; set; }

        /// <summary>
        /// Кол-во этажей
        /// </summary>
        public int Floors { get; set; }

        /// <summary>
        /// Кол-во лифтов
        /// </summary>
        public int Elevators { get; set; }

        /// <summary>
        /// Кол-во подъездов
        /// </summary>
        public int Entrances { get; set; }

        /// <summary>
        /// Есть ли консьерж
        /// </summary>
        public bool HasConcierge { get; set; }

        /// <summary>
        /// ссылка на дом
        /// </summary>
        public string Uri { get; set; }

        public Building(string street, string houseNumber) : base(street, houseNumber)
        {
        }
    }


    public class HouseNumberComparer : IComparer<Building>
    {
        public int Compare(Building x, Building y)
        {
            var first = new HouseNumber(x.HouseNumber);
            var second = new HouseNumber(y.HouseNumber);

            var numberDiff = first.Number.CompareTo(second.Number);
            if (numberDiff != 0) return numberDiff;

            var housingDiff = first.Housing.CompareTo(second.Housing);
            if (housingDiff != 0) return housingDiff;

            return first.AfterSlash.CompareTo(second.AfterSlash);
        }

        #region Nested class

        private struct HouseNumber
        {
            public int Number { get; set; }
            public int Housing { get; set; }
            public int AfterSlash { get; set; }

            public HouseNumber(string text)
            {
                Number = -1;
                Housing = -1;
                AfterSlash = -1;

                if (string.IsNullOrWhiteSpace(text))
                    return;

                Number = ParseHouseNumber(text);
                Housing = ParseHousing(text);
                AfterSlash = ParseSlash(text);
            }

            private int ParseSlash(string text)
            {
                var index = text.IndexOf("\\", StringComparison.Ordinal);
                if (index < 0)
                    index = text.IndexOf("/", StringComparison.Ordinal);

                if (index >= 0)
                {
                    String temp = "";

                    foreach (var c in text.Remove(0, index + 1))
                    {
                        if (char.IsDigit(c))
                            temp += c;
                        else break;
                    }

                    if (int.TryParse(temp, out int res))
                    {
                        return res;
                    }
                }
                return -1;
            }

            private int ParseHouseNumber(string text)
            {
                string main = string.Empty;

                foreach (var c in text)
                {
                    if (char.IsDigit(c))
                        main += c;
                    else
                    {
                        break;
                    }
                }

                if (int.TryParse(main, out int temp))
                {
                    return temp;
                }

                return -1;
            }

            private int ParseHousing(string text)
            {
                int index = text.IndexOf("к", StringComparison.Ordinal);
                if (index >= 0)
                {
                    var tempS = string.Empty;
                    foreach (var c in text.Remove(0, index + 1))
                    {
                        if (char.IsDigit(c))
                            tempS += c;
                        else
                        {
                            break;
                        }
                    }


                    if (int.TryParse(tempS, out int temp))
                    {
                        return temp;
                    }

                    return 1;
                }

                return -1;
            }
        }

        #endregion
    }
}
