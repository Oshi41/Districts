using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Interfaces.IArchiver;
using DistrictsNew.ViewModel.Dialogs;
using Ionic.Zip;

namespace DistrictsNew.Models.Interfaces
{
    public interface ICreateArchiveModel
    {
        /// <summary>
        /// Создаём архив в указанной папке из переданных списков файлов/папок
        /// </summary>
        /// <param name="destination">Где создаю архив</param>
        /// <param name="entries">Список вхождений в zip</param>
        /// <param name="nameFunc"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        string CreateZipPath(string destination, IReadOnlyCollection<SavingItem> entries, Func<ZipFile,string> nameFunc);
    }
}
