namespace Districts.JsonClasses.Base
{
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

        public BaseFindableObject(string street, string houseNumber)
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
            var street = obj.Street;
            var house = obj.HouseNumber;

            if (string.IsNullOrWhiteSpace(street)
                || string.IsNullOrWhiteSpace(house))
            {
                return false;
            }

            bool sameStreet = Street.Contains(street) || street.Contains(Street);
            if (!sameStreet) return false;

            // они дожны быть точными, инчае можем найти совпадения.
            // например, 1 и 1к2
            return HouseNumber == obj.HouseNumber;
        }

        public override bool Equals(object obj)
        {
            if (obj is BaseFindableObject x)
            {
                return x.Street == Street && x.HouseNumber == HouseNumber;
            }

            return false;
        }
    }
}
