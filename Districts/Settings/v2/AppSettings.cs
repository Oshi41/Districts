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

                    CheckFolders(LocalFolder,
                        BaseFolder,
                        ManagementPath, 
                        HomesPath,
                        BackupPath,
                        LogsPath,
                        CardsPath,
                        CommonHomeInfoPath);

                    ChechFiles(StreetsPath,
                        ConfigFilePath);
                }
            }
        }
        public int MaxDoors { get; set; }
        public string ConfigFilePath => _configFilePath;
        public string StreetsPath => Path.Combine(BaseFolder, "streets.json");
        public string ManagementPath => Path.Combine(BaseFolder, "Management");
        public string HomesPath => Path.Combine(BaseFolder, "Homes");
        public string BackupPath => Path.Combine(BaseFolder, "Backup");
        public string LogsPath => Path.Combine(BaseFolder, "logs");
        public string CardsPath => Path.Combine(BaseFolder, "Cards");
        public string CommonHomeInfoPath => Path.Combine(BaseFolder, "CommonInfo");


        public AppSettings()
        {
            Read();
        }

        public void Read()
        {
            CheckFolders(Path.GetDirectoryName(ConfigFilePath));

            if (File.Exists(ConfigFilePath) 
                && JsonConvert.DeserializeObject<settings>(File.ReadAllText(ConfigFilePath)) is settings set)
            {
                Update(set);
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

            File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(ToSettings()));
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
                    Tracer.Tracer.Instance.Write(e);
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
                    Tracer.Tracer.Instance.Write(e);
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
