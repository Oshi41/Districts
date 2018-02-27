using System;
using System.IO;
using Newtonsoft.Json;

namespace Districts.Settings
{
    class ApplicationSettings
    {
        #region Helping

        private static string GetLocalFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Districts");
        }

        #endregion

        /// <summary>
        /// Макс кол-во квартир
        /// </summary>
        public int MaxDoors { get; set; } = 25;
        public string BuildingPath { get; set; } = GetLocalFolder() + "\\Buildings";
        public string CardsPath { get; set; } = GetLocalFolder() + "\\Cards";
        public string StreetsPath { get; set; } = GetLocalFolder() + "\\Streets.txt";
        public string CodesPath { get; set; } = GetLocalFolder() + "\\Codes";
        public string RestrictionsPath { get; set; } = GetLocalFolder() + "\\Restrictions";
        public string LogPath { get; set; } = GetLocalFolder() + "\\Logs";
        public string ManageRecordsPath { get; set; } = GetLocalFolder() + "\\ManageRecords";

        #region ctor

        private ApplicationSettings()
        {
            
        }

        #endregion

        public static ApplicationSettings ReadOrCreate()
        {
            string localPath = GetLocalFolder();

            if (!Directory.Exists(localPath))
                Directory.CreateDirectory(localPath);

            ApplicationSettings settings;

            var path = localPath + "\\config";
            if (File.Exists(path))
            {
                settings = Read(path);
            }
            else
            {
                settings = new ApplicationSettings();
                Write(path, settings);
            }

            //Создаю папки, если их нет
            CheckExistance(settings.BuildingPath, settings.CardsPath, settings.CodesPath, settings.RestrictionsPath);
            return settings;
        }

        private static void CheckExistance(params string[] folders)
        {
            foreach (var folder in folders)
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
        }
        private static void Write(string path, ApplicationSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        private static ApplicationSettings Read(string path)
        {
            var json = File.ReadAllText(path);
            var settings = JsonConvert.DeserializeObject<ApplicationSettings>(json);
            return settings;
        }
    }
}
