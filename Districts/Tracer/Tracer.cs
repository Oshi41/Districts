using System;
using System.IO;
using System.Runtime.CompilerServices;
using Districts.New.Implementation;
using Districts.Singleton;

namespace Districts.Tracer
{
    /// <summary>
    ///     Логирование
    /// </summary>
    public class Tracer : ITrace
    {
        private readonly object _obj = new object();
        public static ITrace Instance => _instance ?? (_instance = new Tracer(IoC.Instance.Get<IAppSettings>().LogsPath));
        private static ITrace _instance;


        private readonly string _filename;
        private Tracer(string folder)
        {
            _filename = Path.Combine(folder, DateTime.Now.ToShortDateString() + ".log");
            var stream = File.CreateText(_filename);
            stream.Close();
        }

        /// <summary>
        ///     Возвращает время в данный момент
        /// </summary>
        /// <returns></returns>
        private string GetTime()
        {
            var time = DateTime.Now;
            var timeToWrite = $"{time.Hour}:{time.Minute}:{time.Second}  ->  ";
            return timeToWrite;
        }

        /// <summary>
        ///     Записывает сообщение
        /// </summary>
        /// <param name="message"></param>
        public void Write(string message)
        {
            lock (_obj)
            {
                File.AppendAllText(_filename, $"{GetTime()}{message}\n\n");
            }
        }

        /// <summary>
        ///     Пишет ошибку
        /// </summary>
        /// <param name="e">Исключение</param>
        /// <param name="message">Сообщение пользователя</param>
        /// <param name="file"></param>
        /// <param name="lineNumber"></param>
        public void Write(Exception e, string message = null,
            [CallerMemberName] string file = null,
            [CallerLineNumber] int lineNumber = -1)
        {
            var result = $"{GetTime()} File - \'{file}\',line\'{lineNumber}\' ОШИБКА\n" +
                         (string.IsNullOrEmpty(message)
                             ? string.Empty
                             : message + "\n") +
                         $"{e.Message}\n\n";
            Write(result);
        }
    }
}