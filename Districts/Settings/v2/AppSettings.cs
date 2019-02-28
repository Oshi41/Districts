using System;
using System.IO;
using Districts.New.Implementation;
using Newtonsoft.Json;

namespace Districts.Settings.v2
{
    class AppSettings : IAppSettings
    {
        private string _baseFolder;

        // Путь к конфигу статичен!
        private static string _configFilePath = Path.Combine(LocalFolder, "config.json");

        // новая папка - след версия
        private static string LocalFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Districts v2");

        public string BaseFolder
        {
            get => _baseFolder;
            set
            {
                if (_baseFolder != value)
                {
                    _baseFolder = value;
                    CheckFolders(BaseFolder, ManagementPath, LocalFolder, BackupPath);
                }
            }
        }
        public int MaxDoors { get; set; }
        public string ConfigFilePath => _configFilePath;
        public string StreetsPath => Path.Combine(BaseFolder, "streets.txt");
        public string ManagementPath => Path.Combine(BackupPath, "Management");
        public string HomesPath => Path.Combine(BackupPath, "Homes");
        public string BackupPath => Path.Combine(BaseFolder, "Backup");
        public string LogsPath => Path.Combine(BaseFolder, "logs");


        public AppSettings()
        {
            Read();
        }

        public void Read()
        {
            CheckFolders(Path.GetDirectoryName(ConfigFilePath));

            if (File.Exists(ConfigFilePath))
            {
                Update(JsonConvert.DeserializeObject<settings>(File.ReadAllText(ConfigFilePath)));
            }
            else
            {
                var defaults = settings.Default;
                Update(defaults);
                Save();
            }
        }

        public void Save()
        {
            ChechFiles(ConfigFilePath);

            File.ReadAllText(JsonConvert.SerializeObject(ToSettings()));
        }

        private void Update(settings settings)
        {
            MaxDoors = settings.doors;
            BaseFolder = settings.path;
        }

        private settings ToSettings()
        {
            return new settings
            {
                path = BaseFolder,
                doors = MaxDoors
            };
        }

        private static void CheckFolders(params string[] folders)
        {
            foreach (var folder in folders)
            {
                try
                {
                    var fullpath = Path.GetFullPath(folder);

                    if (!Directory.Exists(fullpath))
                        Directory.CreateDirectory(fullpath);
                }
                catch (Exception e)
                {
                    Tracer.Tracer.WriteError(e);
                }
            }
        }

        private static void ChechFiles(params string[] files)
        {
            foreach (var file in files)
            {
                try
                {
                    // файл уже есть
                    if (File.Exists(file))
                        continue;

                    // корневая папка
                    var dict = Path.GetDirectoryName(file);
                    // проверяем ее наличие или создаем
                    CheckFolders(dict);
                    // создаем файл
                    File.CreateText(file).Close();
                }
                catch (Exception e)
                {
                    Tracer.Tracer.WriteError(e);
                }
            }
        }

        #region Nested

        private class settings
        {
            public string path { get; set; }
            public int doors { get; set; }

            public static settings Default => new settings
            {
                doors = 25,
                path = LocalFolder
            };
        }

        #endregion
    }
}
