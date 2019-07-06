using System.Threading.Tasks;

namespace DistrictsLib.Interfaces.GoogleApi
{
    public interface IGoogleDriveApi
    {
        /// <summary>
        /// Пытаемся подключиться к Google API
        /// </summary>
        /// <param name="author">Автор соединения</param>
        /// <returns></returns>
        Task<bool> TryConnect(string author);

        /// <summary>
        /// Пытаемся загрузить на гугл диск наши файоы
        /// </summary>
        /// <param name="fileSource"></param>
        /// <returns></returns>
        Task<bool> TryUpload(string fileSource);
        
        /// <summary>
        /// Пытаемся скачать файлы в указанное место
        /// </summary>
        /// <param name="destination">Куда скачиваем</param>
        /// <returns></returns>
        Task<bool> TryDownload(string destination);

        /// <summary>
        /// Отмена асинхронныйх действий
        /// </summary>
        void Cancel();
    }
}
