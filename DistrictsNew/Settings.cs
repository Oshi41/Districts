using System;
using System.Configuration;
using System.IO;

namespace DistrictsNew.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    public sealed partial class Settings {
        
        public Settings() {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //

            this.SettingsLoaded += SubscribeOnce;
        }

        private void SubscribeOnce(object sender, SettingsLoadedEventArgs e)
        {
            this.SettingsLoaded -= SubscribeOnce;

            // Сделать более точную проверку
            if (string.IsNullOrWhiteSpace(BuildingFolder))
            {
                Init();
            }
        }

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }

        #region Initialization

        private void Init()
        {
            var baseFolder = Path
                .Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Districts");

            Propagate(baseFolder);
        }

        private void CheckFile(string file)
        {
            if (!File.Exists(file))
                File.Create(file).Close();
        }

        private void CheckFolder(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        #endregion

        /// <summary>
        /// Устанавливаем базовую папку, в которой будет соблюдаться иерархия
        /// </summary>
        /// <param name="folder"></param>
        public void Propagate(string folder)
        {
            CheckFolder(folder);

            BuildingFolder = Path.Combine(folder, "Buildings");
            CheckFolder(BuildingFolder);

            CardsFolder = Path.Combine(folder, "Cards");
            CheckFolder(CardsFolder);

            StreetsFile = Path.Combine(folder, "Streets.txt");
            CheckFile(StreetsFile);

            HomeInfoFolder = Path.Combine(folder, "HomeInfo");
            CheckFolder(HomeInfoFolder);

            RestrictionsFolder = Path.Combine(folder, "Restrictions");
            CheckFolder(RestrictionsFolder);

            ManageFolder = Path.Combine(folder, "ManageRecords");
            CheckFolder(ManageFolder);

            BackupFolder = Path.Combine(folder, "Backups");
            CheckFolder(BackupFolder);

            TokensFolder = Path.Combine(folder, "tokens");
            CheckFolder(TokensFolder);

            LogFolder = Path.Combine(folder, "Logs");
            CheckFolder(LogFolder);

            Save();
        }
    }
}
