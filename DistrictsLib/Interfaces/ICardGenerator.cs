using System.Collections.Generic;
using DistrictsLib.Legacy.JsonClasses;

namespace DistrictsLib.Interfaces
{
    public interface ICardGenerator
    {
        /// <summary>
        /// Генерирую карточки по переданным данным
        /// </summary>
        /// <param name="homes"></param>
        /// <param name="rules"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        List<Card> Generate(IList<Building> homes, IList<ForbiddenElement> rules, IList<HomeInfo> infos);
    }
}
