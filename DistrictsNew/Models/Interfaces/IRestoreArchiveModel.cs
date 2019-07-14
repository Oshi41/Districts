using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Interfaces.IArchiver;

namespace DistrictsNew.Models
{
    public interface IRestoreArchiveModel
    {
        /// <summary>
        /// Восатанавливает архив в указанный путь.
        /// <para>Возвращает непустую строку, если были опциональные предупреждения</para>
        /// </summary>
        /// <param name="info">Информация о zip</param>
        /// <param name="destination">Куда извлекаем</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        Task<string> RestoreWithWarnings(IZipInfo info, string destination);

        /// <summary>
        /// Возраащет список архивов и список ошибок, если они произошшли
        /// </summary>
        /// <param name="folder">Папка с архивами</param>
        /// <param name="warnings">список ошибок</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        IList<IZipInfo> ReadZips(string folder, out string warnings);
    }
}
