using System;
using System.Collections.Generic;
using Ionic.Zip;

namespace DistrictsLib.Interfaces.IArchiver
{
    public interface IArchiver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool TryToZip(string zip, Func<ZipFile, string> commentFunc = null, params string[] entries);

        /// <summary>
        /// Разархивирует файл в указанное место
        /// </summary>
        /// <param name="zip"></param>
        /// <param name="destination"></param>
        /// <param name="warnings">Предупреждения в ходе распаковки</param>
        /// <returns></returns>
        bool TryUnzip(string zip, string destination, out string warnings);

        /// <summary>
        /// Вытаскивает список вхождений в архив
        /// </summary>
        /// <param name="zip"></param>
        /// <returns></returns>
        IZipInfo GetInfo(string zip);
    }
}
