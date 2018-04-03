using System.Collections.Generic;
using System.Linq;
using Districts.JsonClasses.Base;

namespace Districts.JsonClasses
{
    /// <summary>
    ///     Коды к подъездам
    /// </summary>
    public class HomeInfo : BaseFindableObject
    {
        public HomeInfo(BaseFindableObject obj) : base(obj)
        {
        }

        public HomeInfo()
        {
        }

        /// <summary>
        ///     Список кодов по подъездам
        /// </summary>
        public Dictionary<int, List<string>> AllCodes { get; set; } = new Dictionary<int, List<string>>();

        /// <summary>
        ///     Используем последовательную нумерацию
        /// </summary>
        public int Begin { get; set; } = 1;

        /// <summary>
        ///     Возвращает коды по номеру подъезда
        /// </summary>
        /// <param name="entrance"></param>
        /// <remarks>Не участвует в сравнении!</remarks>
        /// >
        public IList<string> GetCodesByEntrance(int entrance)
        {
            if (AllCodes.ContainsKey(entrance))
                return AllCodes[entrance];


            return new List<string>();
        }

        #region Override

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            if (obj is HomeInfo x)
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

        public static bool operator ==(HomeInfo x, HomeInfo y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.Equals(y);
        }

        public static bool operator !=(HomeInfo x, HomeInfo y)
        {
            return !(x == y);
        }

        #endregion
    }
}