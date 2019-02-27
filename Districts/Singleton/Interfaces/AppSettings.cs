using System;
using System.IO;
using Newtonsoft.Json;

namespace Districts.Singleton.Implementation
{
    class AppSettings : IAppSettings
    {
        public AppSettings()
        {
            Read();
        }

        private static string _localPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Districts");

        private app_settings _inner = new app_settings(_localPath, 25);

        public string BaseFolder
        {
            get=> _inner.BaseFolder;
            set => _inner.BaseFolder = value;
        }

        public int MaxDoors
        {
            get => _inner.MaxDoors;
            set => _inner.MaxDoors = value;
        }
        public string GetBuildingPath()
        {
            return _inner.BuildingPath;
        }

        public string GetCardsPath()
        {
            return _inner.CardsPath;
        }

        public string GetStreetsFile()
        {
            return _inner.StreetsPath;
        }

        public string GetHomeInfoPath()
        {
            return _inner.HomeInfoPath;
        }

        public string GetRestrictionsPath()
        {
            return _inner.RestrictionsPath;
        }

        public string GetLogsPath()
        {
            return _inner.LogPath;
        }

        public string GetManagePath()
        {
            return _inner.ManageRecordsPath;
        }

        public string GetBackupPath()
        {
            return _inner.BackupFolder;
        }

        public string GetConfigFile()
        {
            return Path.Combine(_localPath, "config");
        }

        public void Read()
        {
            CheckFolderExistance(_localPath);

            if (File.Exists(GetConfigFile()))
            {
                _inner = JsonConvert.DeserializeObject<app_settings>(File.ReadAllText(GetConfigFile()));
            }
            else
            {
                _inner = new app_settings(_localPath, 25);
                Save();
            }

            CheckFolderExistance(
                BaseFolder,
                GetBuildingPath(),
                GetCardsPath(),
                GetStreetsFile(),
                GetHomeInfoPath(),
                GetRestrictionsPath(),
                GetLogsPath(),
                GetManagePath(),
                GetBackupPath());
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(_inner, Formatting.Indented);
            File.WriteAllText(GetConfigFile(), json);
        }

        private static void CheckFolderExistance(params string[] folders)
        {
            foreach (var folder in folders)
            {
                if (string.IsNullOrEmpty(folder)) continue;
                try
                {
                    var fullpath = Path.GetFullPath(folder);

                    if (!Directory.Exists(fullpath))
                        Directory.CreateDirectory(fullpath);
                }
                catch
                {
                    //igonred
                }
            }
        }

        private static void CheckFileExistance(params string[] files)
        {
            foreach (var file in files)
            {
                // файл уже есть
                if (File.Exists(file))
                    continue;

                // корневая папка
                var dict = Path.GetDirectoryName(file);
                // проверяем ее наличие или создаем
                CheckFolderExistance(dict);
                // создаем файл
                var stream = File.CreateText(file);
                // закрываем поток создания файла
                stream.Close();
            }
        }

        #region Nested

        private class app_settings
        {
            public app_settings(string baseFolder, 
                int maxDoors)
            {
                BaseFolder = baseFolder;
                MaxDoors = maxDoors;
                BuildingPath = Path.Combine(BaseFolder, "Buildings");
                CardsPath = Path.Combine(BaseFolder, "Cards");
                StreetsPath = Path.Combine(BaseFolder, "Streets.txt");
                HomeInfoPath = Path.Combine(BaseFolder, "HomeInfo");
                RestrictionsPath = Path.Combine(BaseFolder, "Restrictions");
                LogPath = Path.Combine(BaseFolder, "Logs");
                ManageRecordsPath = Path.Combine(BaseFolder, "ManageRecords");
                BackupFolder = Path.Combine(BaseFolder, "Backups");
            }

            /// <summary>
            ///     Будет меняться только базовая папка, остальные создаются внутри этой
            /// </summary>
            public string BaseFolder { get; set; }

            /// <summary>
            ///     Макс кол-во квартир
            /// </summary>
            public int MaxDoors { get; set; }

            /// <summary>
            ///     Путь к домам
            /// </summary>
            public string BuildingPath { get; set; }

            /// <summary>
            ///     Путь к карточкам участков
            /// </summary>
            public string CardsPath { get; set; }

            /// <summary>
            ///     Путь к улицам
            /// </summary>
            public string StreetsPath { get; set; }

            /// <summary>
            ///     Путь к кодам
            /// </summary>
            public string HomeInfoPath { get; set; }

            /// <summary>
            ///     Путь к правилам доступа
            /// </summary>
            public string RestrictionsPath { get; set; }

            /// <summary>
            ///     Путь для логирования
            /// </summary>
            public string LogPath { get; set; }

            /// <summary>
            ///     Путь для записей о карточке
            /// </summary>
            public string ManageRecordsPath { get; set; }

            /// <summary>
            ///     Папка с бэкапами
            /// </summary>
            public string BackupFolder { get; set; }
        }

        #endregion
    }
}
