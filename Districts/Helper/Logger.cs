using System;
using System.IO;
using System.Linq;
using Districts.Settings;

namespace Districts.Helper
{
    public class Logger
    {
        private string message;

        public void AddMessage(string s, Exception e = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
                message += s;

            if (e != null)
                message += "\n" + e.Message;

            message += "\n\n";
        }

        public void AddMessage(Exception e)
        {
            AddMessage(null, e);
        }

        public void Separate()
        {
            message += Enumerable.Repeat("--", 10).Aggregate("", (s, s1) => s += s1);
        }

        public void WriteToFile()
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var path = Path.Combine(ApplicationSettings.ReadOrCreate().LogPath, new Guid().ToString());
            File.WriteAllText(path, message);
        }
    }
}