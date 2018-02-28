using System;
using System.IO;
using System.Runtime.CompilerServices;
using Districts.Settings;

namespace Districts.Helper
{
    /// <summary>
    /// Логирование
    /// </summary>
    public class Tracer
    {
        private readonly string _filename;
        protected static Tracer Instance = new Tracer();

        private Tracer()
        {
            _filename = Path.Combine(ApplicationSettings.ReadOrCreate().LogPath, Guid.NewGuid().ToString());
            var stream = File.CreateText(_filename);
            stream.Close();
        }

        /// <summary>
        /// Возвращает время в данный момент
        /// </summary>
        /// <returns></returns>
        private static string GetTime()
        {
            var time = DateTime.Now;
            var timeToWrite = $"{time.Hour}:{time.Minute}:{time.Second}  ->  ";
            return timeToWrite;
        }

        /// <summary>
        /// Записывает сообщение
        /// </summary>
        /// <param name="message"></param>
        public static void Write(string message)
        {
            File.AppendAllText(Instance._filename, $"{GetTime()}{message}\n\n");
        }

        /// <summary>
        /// Пишет ошибку
        /// </summary>
        /// <param name="e">Исключение</param>
        /// <param name="message">Сообщение пользователя</param>
        /// <param name="file"></param>
        /// <param name="lineNumer"></param>
        public static void WriteError(Exception e, string message = null, 
            [CallerMemberName] string file = null,
            [CallerLineNumber] int lineNumer = -1)
        {
            var result = $"{GetTime()} File - \'{file}\',line\'{lineNumer}\' ОШИБКА\n" +
                         (string.IsNullOrEmpty(message) 
                             ? string.Empty 
                             : message + "\n") +
                         $"{e.Message}\n\n";
            Write(result);
        }
    }
}
