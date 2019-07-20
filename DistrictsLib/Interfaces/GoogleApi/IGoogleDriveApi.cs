using System;
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
        /// <exception cref="Exception"></exception>
        Task Connect(string author);

        /// <summary>
        /// Пытаемся загрузить на гугл диск наши файоы
        /// </summary>
        /// <param name="fileSource"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        Task Upload(string fileSource);

        /// <summary>
        /// Пытаемся скачать файлы в указанное место
        /// </summary>
        /// <param name="destination">Куда скачиваем</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        Task Download(string destination);

        /// <summary>
        /// Возвращает имя файла, типа google_ДАТА
        /// </summary>
        /// <returns></returns>
        Task<string> GetFileName();

        /// <summary>
        /// Отмена асинхронныйх действий
        /// </summary>
        void Cancel();

        /// <summary>
        /// Произведено ли подключение
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
    }
}
