using System.Collections.Generic;
using DistrictsLib.Legacy.JsonClasses.Base;

namespace DistrictsLib.Legacy.JsonClasses
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
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HomeInfo) obj);
        }

        #region Equality members

        protected bool Equals(HomeInfo other)
        {
            return base.Equals(other)
                   && SafeEqualsCodes(AllCodes, other.AllCodes)
                   && Begin == other.Begin;
        }

        private bool SafeEqualsCodes(Dictionary<int, List<string>> allCodes, Dictionary<int, List<string>> otherAllCodes)
        {
            if (allCodes == otherAllCodes)
                return true;

            if (allCodes.Count != otherAllCodes.Count)
                return false;

            if (!allCodes.Keys.IsTermwiseEquals(otherAllCodes.Keys))
                return false;

            foreach (var code in allCodes)
            {
                if (!code.Value.IsTermwiseEquals(otherAllCodes[code.Key]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        #endregion

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