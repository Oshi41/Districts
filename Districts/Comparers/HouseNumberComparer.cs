using System;
using System.Collections.Generic;
using Districts.JsonClasses;

namespace Districts.Comparers
{
    /// <summary>
    ///     Сравнивает дома по номеру
    /// </summary>
    public class HouseNumberComparer : IComparer<Building>
    {
        private readonly HouseNumberComparerFromString _comparer = new HouseNumberComparerFromString();

        /// <summary>
        ///     Сравнивает два дома
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(Building x, Building y)
        {
            return _comparer.Compare(x?.HouseNumber, y?.HouseNumber);
        }
    }

    public class HouseNumberComparerFromString : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(x, null)) return -1;
            if (ReferenceEquals(y, null)) return 1;

            var first = new HouseNumber(x);
            var second = new HouseNumber(y);

            var numberDiff = first.Number.CompareTo(second.Number);
            if (numberDiff != 0) return numberDiff;

            var housingDiff = first.Housing.CompareTo(second.Housing);
            if (housingDiff != 0) return housingDiff;

            return first.AfterSlash.CompareTo(second.AfterSlash);
        }
    }

    public class HouseNumberComparerFromDoor : IComparer<Door>
    {
        private readonly IComparer<string> _comparer;

        public HouseNumberComparerFromDoor(IComparer<string> comparer)
        {
            _comparer = comparer;
        }

        public int Compare(Door x, Door y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(x, null)) return -1;
            if (ReferenceEquals(y, null)) return 1;

            var streetCompare = string.Compare(x.Street, y.Street, StringComparison.Ordinal);
            if (streetCompare != 0)
                return streetCompare;

            return _comparer.Compare(x.HouseNumber, y.HouseNumber);
        }
    }

    /// <summary>
    ///     Класс представляющий номер дома
    /// </summary>
    public struct HouseNumber
    {
        /// <summary>
        ///     Номер дома
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        ///     Подъезд
        /// </summary>
        public int Housing { get; set; }

        /// <summary>
        ///     То, что иде т после слеша
        /// </summary>
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
                var temp = "";

                foreach (var c in text.Remove(0, index + 1))
                    if (char.IsDigit(c))
                        temp += c;
                    else
                        break;

                if (int.TryParse(temp, out var res)) return res;
            }

            return -1;
        }

        private int ParseHouseNumber(string text)
        {
            var main = string.Empty;

            foreach (var c in text)
                if (char.IsDigit(c))
                    main += c;
                else
                    break;

            if (int.TryParse(main, out var temp)) return temp;

            return -1;
        }

        private int ParseHousing(string text)
        {
            var index = text.IndexOf("к", StringComparison.Ordinal);
            if (index >= 0)
            {
                var tempS = string.Empty;
                foreach (var c in text.Remove(0, index + 1))
                    if (char.IsDigit(c))
                        tempS += c;
                    else
                        break;


                if (int.TryParse(tempS, out var temp)) return temp;

                return 1;
            }

            return -1;
        }
    }
}