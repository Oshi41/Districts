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
        /// <param name="entries"></param>
        /// <param name="baseFolder"></param>
        /// <returns></returns>
        Task ArchiveAndUpload(IReadOnlyCollection<SavingItem> entries, string baseFolder);

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
    }
}
