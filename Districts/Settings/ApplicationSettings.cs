using System;
using System.IO;
using Newtonsoft.Json;

namespace Districts.Settings
{
    internal class ApplicationSettings
    {
        #region ctor

        private ApplicationSettings()
        {
        }

        #endregion

        /// <summary>
        ///     Будет меняться только базовая папка, остальные создаются внутри этой
        /// </summary>
        public string BaseFolder { get; set; } = GetLocalFolder();

        /// <summary>
        ///     Макс кол-во квартир
        /// </summary>
        public int MaxDoors { get; set; } = 25;

        /// <summary>
        ///     Путь к домам
        /// </summary>
        public string BuildingPath => BaseFolder + "\\Buildings";

        /// <summary>
        ///     Путь к карточкам участков
        /// </summary>
        public string CardsPath => BaseFolder + "\\Cards";

        /// <summary>
        ///     Путь к улицам
        /// </summary>
        public string StreetsPath => BaseFolder + "\\Streets.txt";

        /// <summary>
        ///     Путь к кодам
        /// </summary>
        public string HomeInfoPath => BaseFolder + "\\HomeInfo";

        /// <summary>
        ///     Путь к правилам доступа
        /// </summary>
        public string RestrictionsPath => BaseFolder + "\\Restrictions";

        /// <summary>
        ///     Путь для логирования
        /// </summary>
        public string LogPath => BaseFolder + "\\Logs";

        /// <summary>
        ///     Путь для записей о карточке
        /// </summary>
        public string ManageRecordsPath => BaseFolder + "\\ManageRecords";

        /// <summary>
        ///     Папка с бэкапами
        /// </summary>
        public string BackupFolder => BaseFolder + "\\Backups";

        /// <summary>
        /// Путь к токенам
        /// </summary>
        public string TokensPath => Path.Combine(BaseFolder, "tokens");

        /// <summary>
        ///     Путь к конфигу всегда один и тот же!!!!
        /// </summary>
        public static string ConfigPath => GetLocalFolder() + "\\config";

        

        #region Helping

        private static string GetLocalFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Districts");
        }

        #endregion

        public static ApplicationSettings ReadOrCreate()
        {
            var localPath = GetLocalFolder();
            if (!Directory.Exists(localPath))
                Directory.CreateDirectory(localPath);

            ApplicationSettings settings;

            var path = ConfigPath;

            if (File.Exists(path))
            {
                settings = Read(path);
            }
            else
            {
                settings = new ApplicationSettings();
                settings.Write();
            }

            //Создаю папки, если их нет
            CheckExistance(settings.BuildingPath,
                settings.CardsPath,
                settings.HomeInfoPath,
                settings.RestrictionsPath,
                settings.LogPath,
                settings.ManageRecordsPath,
                settings.BackupFolder);

            // создаю файлы, если их нет
            CheckFileExistance(settings.StreetsPath);

            return settings;
        }

        private static ApplicationSettings Read(string path)
        {
            var json = File.ReadAllText(path);
            var settings = JsonConvert.DeserializeObject<ApplicationSettings>(json);
            return settings;
        }

        public static ApplicationSettings GetDefault()
        {
            return new ApplicationSettings();
        }

        public void Write()
        {
            var path = ConfigPath;
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        private static void CheckExistance(params string[] folders)
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
                CheckExistance(dict);
                // создаем файл
                var stream = File.CreateText(file);
                // закрываем поток создания файла
                stream.Close();
            }
        }
    }
}