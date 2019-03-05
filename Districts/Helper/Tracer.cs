using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using Districts.Settings;

namespace Districts.Helper
{
    /// <summary>
    ///     Логирование
    /// </summary>
    public class Tracer
    {
        #region Static

        private static readonly object _lock = new object();
        private static Tracer _instance;

        public static Tracer Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ?? (_instance = new Tracer());
                }
            }
        }
        

        #endregion

        private readonly object _writerLock = new object();
        private readonly ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();
        private readonly Timer _timer;
        private readonly string _filename;

        private Tracer()
        {
            _filename = Path.Combine(ApplicationSettings.ReadOrCreate().LogPath, DateTime.Now.ToShortDateString() + ".log");

            if (!File.Exists(_filename))
                File.CreateText(_filename).Close();

            _timer = new Timer(500);
            _timer.Elapsed += TryWriteToFile;
        }

        private void TryWriteToFile(object sender, ElapsedEventArgs e)
        {
            try
            {
                lock (_writerLock)
                {
                    while (!_messages.IsEmpty)
                    {
                        if (_messages.TryDequeue(out var msg))
                        {
                            File.AppendAllText(_filename, msg);
                        }
                    }

                    _timer.Stop();
                }
            }
            catch
            {
                // ignored
            }
        }

        private void EnqueueMessage(string msg)
        {
            _messages.Enqueue(msg);

            lock (_writerLock)
            {
                _timer.Start();
            }
        }

        /// <summary>
        /// Записывает сообщение
        /// </summary>
        /// <param name="message"></param>
        /// <param name="file">Имя файла</param>
        /// <param name="lineNumer">Номер строчки</param>
        public static void Write(string message, [CallerMemberName] string file = null, [CallerLineNumber] int lineNumer = -1)
        {
            var now = DateTime.Now;
            var msg = $"{now:hh:mm:ss.FFF} [{file}] [{lineNumer}] -> {message}\n";

            Instance.EnqueueMessage(msg);
        }

        /// <summary>
        ///     Пишет ошибку
        /// </summary>
        /// <param name="e">Исключение</param>
        /// <param name="file"></param>
        /// <param name="lineNumber"></param>
        public static void WriteError(Exception e,
            [CallerMemberName] string file = null,
            [CallerLineNumber] int lineNumber = -1)
        {
            Write($"ОШИБКА: {e.Message}", file, lineNumber);
        }
    }
}