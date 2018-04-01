using System;
using Districts.Comparers;

namespace Districts.JsonClasses.Base
{
    /// <summary>
    /// Базовый класс-родитель для квартир и домов
    /// </summary>
    public class BaseFindableObject
    {
        #region Properties

        /// <summary>
        /// Имя улицы
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Имя дома
        /// </summary>
        public string HouseNumber { get; set; }

        #endregion

        #region Constructors

        public BaseFindableObject(BaseFindableObject obj)
        {
            if (obj == null)
                return;

            Street = obj.Street;
            HouseNumber = obj.HouseNumber;
        }

        /// <summary>
        /// Нужен открытый конструктор для сериализации
        /// </summary>
        /// <param name="street"></param>
        /// <param name="houseNumber"></param>
        public BaseFindableObject(string street = "", string houseNumber = "")
        {
            Street = street;
            HouseNumber = houseNumber;
        }

        #endregion

        /// <summary>
        /// Ищет совпадение по объектам
        /// </summary>
        /// <param name="obj">Объект, с которым сверяем</param>
        /// <returns></returns>
        public bool IsTheSameObject(BaseFindableObject obj)
        {
            if (obj == null)
                return false;

            if (!TheSameStreet(obj.Street))
                return false;

            // они дожны быть точными, инчае можем найти совпадения.
            // например, 1 и 1к2
            return HouseNumber == obj.HouseNumber;
        }

        /// <summary>
        /// Возвращает тот же дом, только не учитывает корпус
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="findCondition">Условия поиска</param>
        /// <returns></returns>
        public bool TheSameHouse(BaseFindableObject obj,
            ReturnConditions findCondition)
        {
            // нет объекта, ошибка
            if (obj == null)
                return false;

            // улицы разные
            if (!TheSameStreet(obj.Street))
                return false;

            // нашли номера домов
            var number = new HouseNumber(HouseNumber);
            var objNumber = new HouseNumber(obj.HouseNumber);

            // разные номера, выходим
            if (number.Number != objNumber.Number)
                return false;

            // нужно сравнить слеши
            if ((findCondition & ReturnConditions.CompareSlash) != 0
                && number.AfterSlash != objNumber.AfterSlash)
            {
                return false;
            }

            // возвращаем что угодно
            if ((findCondition & ReturnConditions.All) == ReturnConditions.All)
                return true;

            // сравниваем значения
            var compare = number.Housing.CompareTo(objNumber.Housing);

            // возвращаем, если стоит включать себя
            if (compare == 0 && (findCondition & ReturnConditions.Self) != 0)
            {
                return true;
            }

            // нашли меньше
            if (compare < 0 && (findCondition & ReturnConditions.LessThen) != 0)
            {
                return true;
            }

            // нашли больше
            if (compare > 0 && (findCondition & ReturnConditions.MoreThen) != 0)
            {
                return true;
            }

            return false;
        }

        private bool TheSameStreet(string street)
        {
            if (string.IsNullOrWhiteSpace(street))
                return false;

            bool sameStreet = Street.Contains(street) || street.Contains(Street);
            return sameStreet;
        }


        public override bool Equals(object obj)
        {
            if (obj is BaseFindableObject x)
            {
                return x.Street == Street && x.HouseNumber == HouseNumber;
            }

            return false;
        }


        #region Nested

        [Flags]
        public enum ReturnConditions
        {
            LessThen = 1,
            MoreThen = 2,
            Self = 4,
            CompareSlash = 8,

            LessThenInclude = LessThen | Self,
            MoreThenInclude = MoreThen | Self,

            All = LessThen | LessThenInclude | MoreThen | MoreThenInclude,
            AllWithSlashCheck = All | CompareSlash,
        }

        #endregion
    }
}
