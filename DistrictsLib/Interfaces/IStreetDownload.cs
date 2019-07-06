using System.Collections.Generic;
using System.Threading.Tasks;
using DistrictsLib.Legacy.JsonClasses;

namespace DistrictsLib.Interfaces
{
    public interface IStreetDownload
    {
        /// <summary>
        /// Возвращает список подсказок похожих улиц
        /// </summary>
        /// <param name="text">Текст введенный пользователем</param>
        Task<IList<string>> GetHints(string text);

        /// <summary>
        /// Скачивает инф-у по всем домам на улице
        /// </summary>
        /// <param name="street"></param>
        /// <returns></returns>
        Task<IList<Building>> DownloadStreet(string street);

        /// <summary>
        /// Макс. кол-во результатов в ответе
        /// </summary>
        /// <returns></returns>
        int MaxApi();
    }
}
