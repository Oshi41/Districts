using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Legacy.JsonClasses;

namespace DistrictsLib.Interfaces
{
    public interface ICardGenerator
    {
        /// <summary>
        /// Генерирую карточки по данным, которые лежат по адресу
        /// </summary>
        /// <param name="homes"></param>
        /// <param name="rules"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        List<Card> Generate(string homes, string rules, string infos);

        /// <summary>
        /// Генерирую карточки по переданным данным
        /// </summary>
        /// <param name="homes"></param>
        /// <param name="rules"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        List<Card> Generate(IList<Building> homes, IList<Rule> rules, IList<HomeInfo> infos);
    }
}
