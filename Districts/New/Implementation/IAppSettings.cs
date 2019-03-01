namespace Districts.New.Implementation
{
    public interface IAppSettings
    {
        /// <summary>
        /// Бащовая папка
        /// </summary>
        string BaseFolder { get; set; }

        /// <summary>
        /// Кол-во квартир на участке
        /// </summary>
        int MaxDoors { get; set; }

        /// <summary>
        /// Статичный путь к конфигу
        /// </summary>
        string ConfigFilePath { get; }
        
        /// <summary>
        /// Путь к списку улиц
        /// </summary>
        string StreetsPath { get; }

        /// <summary>
        /// Путь к учетности ведения участков
        /// </summary>
        string ManagementPath { get; }

        /// <summary>
        /// Путь к скачанным домам
        /// </summary>
        string HomesPath { get; }

        /// <summary>
        /// Путь к сэкапу
        /// </summary>
        string BackupPath { get; }

        /// <summary>
        /// Путь к логам
        /// </summary>
        string LogsPath { get; }

        string CardsPath { get; }

        /// <summary>
        /// Путь к пользовательской информации о доме
        /// </summary>
        string CommonHomeInfoPath { get; }

        /// <summary>
        /// Читаем настройки
        /// </summary>
        void Read();

        /// <summary>
        /// Сохраняем
        /// </summary>
        void Save();
    }
}
