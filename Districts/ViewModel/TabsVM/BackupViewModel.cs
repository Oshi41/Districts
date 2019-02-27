using System;
using System.IO;
using System.Text;
using Districts.Helper;
using Districts.Settings;
using Ionic.Zip;
using Microsoft.Expression.Interactivity.Core;
using Mvvm;
using Mvvm.Commands;

namespace Districts.ViewModel.TabsVM
{
    public class BackupViewModel : BindableBase
    {
        private bool _saveCards;
        private bool _saveCodes;
        private bool _saveHomes;
        private bool _saveLogs;
        private bool _saveManagement;
        private bool _saveRestrictions;


        public BackupViewModel()
        {
            BackupActionCommand = new DelegateCommand<object>(OnBackUp, OnCanBackup);
        }


        public DelegateCommandBase BackupActionCommand { get; set; }

        public bool SaveHomes
        {
            get => _saveHomes;
            set => SetProperty(ref _saveHomes, value);
        }

        public bool SaveCards
        {
            get => _saveCards;
            set => SetProperty(ref _saveCards, value);
        }

        public bool SaveCodes
        {
            get => _saveCodes;
            set => SetProperty(ref _saveCodes, value);
        }

        public bool SaveRestrictions
        {
            get => _saveRestrictions;
            set => SetProperty(ref _saveRestrictions, value);
        }

        public bool SaveManagement
        {
            get => _saveManagement;
            set => SetProperty(ref _saveManagement, value);
        }

        public bool SaveLogs
        {
            get => _saveLogs;
            set => SetProperty(ref _saveLogs, value);
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

        protected override bool SetProperty<T>(ref T storage, T value, string propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);

            BackupActionCommand?.RaiseCanExecuteChanged();

            return result;
        }
    }
}