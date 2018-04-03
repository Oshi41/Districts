using System;
using System.IO;
using System.Text;
using Districts.Helper;
using Districts.MVVM;
using Districts.Settings;
using Ionic.Zip;

namespace Districts.ViewModel.TabsVM
{
    public class BackupViewModel : ObservableObject
    {
        private bool _saveCards;
        private bool _saveCodes;
        private bool _saveHomes;
        private bool _saveLogs;
        private bool _saveManagement;
        private bool _saveRestrictions;


        public BackupViewModel()
        {
            BackupCommand = new Command(OnBackUp, OnCanBackup);
        }


        public Command BackupCommand { get; set; }

        public bool SaveHomes
        {
            get => _saveHomes;
            set
            {
                if (value == _saveHomes) return;
                _saveHomes = value;
                OnPropertyChanged();
            }
        }

        public bool SaveCards
        {
            get => _saveCards;
            set
            {
                if (value == _saveCards) return;
                _saveCards = value;
                OnPropertyChanged();
            }
        }

        public bool SaveCodes
        {
            get => _saveCodes;
            set
            {
                if (value == _saveCodes) return;
                _saveCodes = value;
                OnPropertyChanged();
            }
        }

        public bool SaveRestrictions
        {
            get => _saveRestrictions;
            set
            {
                if (value == _saveRestrictions) return;
                _saveRestrictions = value;
                OnPropertyChanged();
            }
        }

        public bool SaveManagement
        {
            get => _saveManagement;
            set
            {
                if (value == _saveManagement) return;
                _saveManagement = value;
                OnPropertyChanged();
            }
        }

        public bool SaveLogs
        {
            get => _saveLogs;
            set
            {
                if (value == _saveLogs) return;
                _saveLogs = value;
                OnPropertyChanged();
            }
        }


        private bool OnCanBackup(object obj)
        {
            return SaveHomes
                   || SaveCards
                   || SaveCodes
                   || SaveRestrictions
                   || SaveManagement
                   || SaveLogs;
        }

        private void OnBackUp(object obj)
        {
            var settings = ApplicationSettings.ReadOrCreate();

            // создали папку
            var currentTime = DateTime.Now;
            var currentFolder = settings.BackupFolder + "\\" + currentTime.ToString("F").Replace(":", "-");
            Directory.CreateDirectory(currentFolder);

            if (SaveHomes) CompressAndSaveFolder(settings.BuildingPath, currentFolder + "\\Homes.zip");

            if (SaveCards) CompressAndSaveFolder(settings.CardsPath, currentFolder + "\\Cards.zip");

            if (SaveCodes) CompressAndSaveFolder(settings.HomeInfoPath, currentFolder + "\\HomeInfo.zip");

            if (SaveManagement)
                CompressAndSaveFolder(settings.ManageRecordsPath, currentFolder + "\\Management Recods.zip");

            if (SaveRestrictions)
                CompressAndSaveFolder(settings.RestrictionsPath, currentFolder + "\\Restrictions.zip");

            if (SaveLogs) CompressAndSaveFolder(settings.LogPath, currentFolder + "\\Logs.zip");

            MessageHelper.ShowDoneBubble();
        }

        private void CompressAndSaveFolder(string toCopy, string archiveName)
        {
            if (!Directory.Exists(toCopy))
            {
                Tracer.Write("Нет папки для сохранения");
                return;
            }

            using (var zip = new ZipFile())
            {
                //
                // нужно для русского языка
                //
                zip.AlternateEncodingUsage = ZipOption.Always;
                zip.AlternateEncoding = Encoding.GetEncoding(866);

                foreach (var file in Directory.GetFiles(toCopy)) zip.AddFile(file, "");
                zip.Save(archiveName);
            }
        }


        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            BackupCommand?.OnCanExecuteChanged();
        }
    }
}