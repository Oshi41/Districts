using System.Collections.Generic;

namespace DistrictsLib.Interfaces.IArchiver
{
    public interface IArchiver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool TryToZip(string zip, params string[] entries);

        /// <summary>
        /// Разархивирует файл в указанное место
        /// </summary>
        /// <param name="zip"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        bool TryUnzip(string zip, string destination);

        /// <summary>
        /// Вытаскивает список вхождений в архив
        /// </summary>
        /// <param name="zip"></param>
        /// <returns></returns>
        IZipInfo GetInfo(string zip);
    }
}
