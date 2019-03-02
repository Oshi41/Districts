using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Districts.New.Interfaces
{
    public interface IHomeParser
    {
        /// <summary>
        /// Скачивает и парсит информацию о доме
        /// </summary>
        /// <param name="partOfUri">Relative url для отдельного дома. Приходит из API</param>
        /// <returns></returns>
        Task<iHome> DownloadAndParse(IRawHome raw);

        /// <summary>
        /// Высчитывает подъезд, в котором находится
        /// </summary>
        /// <param name="door">Номер квартиры</param>
        /// <param name="total">Всего квартир</param>
        /// <param name="totalEntrances">Всего подъездов</param>
        /// <returns></returns>
        int GetEntrance(int door, int total, int totalEntrances);
    }
}
