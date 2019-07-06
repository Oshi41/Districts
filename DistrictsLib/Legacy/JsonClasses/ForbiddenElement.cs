using System.Collections.Generic;
using DistrictsLib.Legacy.JsonClasses.Base;

namespace DistrictsLib.Legacy.JsonClasses
{
    public class ForbiddenElement : BaseFindableObject
    {
        public ForbiddenElement(BaseFindableObject obj) : base(obj)
        {
        }

        public ForbiddenElement()
        {
        }

        /// <summary>
        ///     Агрессиии
        /// </summary>
        public List<int> Aggressive { get; set; } = new List<int>();

        /// <summary>
        ///     Просили не беспокоить
        /// </summary>
        public List<int> NoWorried { get; set; } = new List<int>();

        /// <summary>
        ///     Не посещать
        /// </summary>
        public List<int> NoVisit { get; set; } = new List<int>();

        /// <summary>
        ///     Комментарии
        /// </summary>
        public string Comments { get; set; }

        #region Equality members

        protected bool Equals(ForbiddenElement other)
        {
            return base.Equals(other) 
                   && Helper.Helper.IsTermwiseEquals(Aggressive, other.Aggressive) 
                   && Helper.Helper.IsTermwiseEquals(NoWorried, other.NoWorried) 
                   && Helper.Helper.IsTermwiseEquals(NoVisit, other.NoVisit) 
                   && string.Equals(Comments, other.Comments);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ForbiddenElement) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        #endregion
    }
}