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
    }
}
