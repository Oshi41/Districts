using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsNew.ViewModel.Dialogs;

namespace DistrictsNew.Models.Interfaces
{
    public interface IGoogleApiModel
    {
        /// <summary>
        /// Создаем копию и загржаем её в гугл
        /// </summary>
        /// <param name="entries">Список вхождений папок/файлов</param>
        /// <param name="backupFolder">Откуда берём список папок/фалов - базовая папка откуда начинается иерархия папок</param>
        /// <returns></returns>
        Task ArchiveAndUpload(IReadOnlyCollection<SavingItem> entries, string backupFolder);

        /// <summary>
        /// Скачиваю архив с гугла и записываю
        /// </summary>
        /// <param name="backupFolder"></param>
        /// <returns></returns>
        Task DownloadAndReplace(string backupFolder);

        /// <summary>
        /// Подключение
        /// </summary>
        /// <param name="autor"></param>
        /// <returns></returns>
        Task Connect(string autor);

        bool IsConnected();

        /// <summary>
        /// Отмена асинхронной операции
        /// </summary>
        void Cancel();
    }
}
