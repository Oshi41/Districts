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

            CheckFolder(baseFolder);

            BuildingFolder = Path.Combine(baseFolder, "Buildings");
            CheckFolder(BuildingFolder);

            CardsFolder = Path.Combine(baseFolder, "Cards");
            CheckFolder(CardsFolder);

            StreetsFile = Path.Combine(baseFolder, "Streets.txt");
            CheckFile(StreetsFile);

            HomeInfoFolder = Path.Combine(baseFolder, "HomeInfo");
            CheckFolder(HomeInfoFolder);

            RestrictionsFolder = Path.Combine(baseFolder, "Restrictions");
            CheckFolder(RestrictionsFolder);

            ManageFolder = Path.Combine(baseFolder, "ManageRecords");
            CheckFolder(ManageFolder);

            BackupFolder = Path.Combine(baseFolder, "Backups");
            CheckFolder(BackupFolder);

            TokensFolder = Path.Combine(baseFolder, "tokens");
            CheckFolder(TokensFolder);

            LogFolder = Path.Combine(baseFolder, "Logs");
            CheckFolder(LogFolder);

            Save();
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
    }
}
