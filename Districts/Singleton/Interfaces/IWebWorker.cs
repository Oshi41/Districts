using System.Collections.Generic;
using System.Threading.Tasks;

namespace Districts.New.Interfaces
{
    interface IWebWorker
    {
        /// <summary>
        /// Возвращает список подсказок
        /// </summary>
        /// <returns></returns>
        Task<IList<string>> StreetHints(string street);
    }
}
