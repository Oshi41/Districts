using System.Collections.Generic;
using System.Threading.Tasks;
using Districts.JsonClasses;

namespace Districts.New.Interfaces
{
    interface IWebWorker
    {
        /// <summary>
        /// Возвращает список подсказок
        /// </summary>
        /// <returns></returns>
        Task<IList<string>> StreetHints(string street);

        /// <summary>
        /// Загружает список домов
        /// </summary>
        /// <param name="streets">Улицы</param>
        /// <returns></returns>
        Task<IList<HomeInfo>> DownloadHomes(IList<string> streets);
    }
}
