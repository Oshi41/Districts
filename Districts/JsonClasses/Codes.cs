using System.Collections.Generic;
using System.Linq;
using Districts.JsonClasses.Base;

namespace Districts.JsonClasses
{
    /// <summary>
    /// Коды к подъездам
    /// </summary>
    public class Codes : BaseFindableObject
    {
        /// <summary>
        /// Список кодов по подъездам
        /// </summary>
        public Dictionary<int, List<string>> AllCodes { get; set; } = new Dictionary<int, List<string>>();

        /// <summary>
        /// Возвращает коды по номеру подъезда
        /// </summary>
        /// <param name="entrance"></param>
        /// <returns></returns>
        public IList<string> GetCodesByEntrance(int entrance)
        {
            if (AllCodes.ContainsKey(entrance))
                return AllCodes[entrance];


            return new List<string>();
        }

        public Codes(BaseFindableObject obj) : base(obj)
        {
        }

        public Codes(string street, string houseNumber) : base(street, houseNumber)
        {
        }

        #region Override

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            if (obj is Codes x)
            {
                if (x.AllCodes.Count != AllCodes.Count)
                    return false;

                var keyes = AllCodes.Keys;
                foreach (var key in keyes)
                {
                    if (!x.AllCodes.ContainsKey(key))
                        return false;

                    if (!AllCodes[key].SequenceEqual(x.AllCodes[key]))
                        return false;
                }

                return true;
            }

            return false;
        }

        public static bool operator ==(Codes x, Codes y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(Codes x, Codes y)
        {
            return !(x == y);
        }


        #endregion


    }
}
