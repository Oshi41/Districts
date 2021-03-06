﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DistrictsLib.Interfaces;
using DistrictsLib.Interfaces.IArchiver;
using DistrictsNew.Models;
using DistrictsNew.ViewModel.Base;
using DistrictsNew.ViewModel.HostDialogs;
using MaterialDesignThemes.Wpf;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel.Dialogs
{
    class RestoreBackupViewModel : ChangesViewModel
    {
        private readonly IRestoreArchiveModel _model;
        private string _backupFolder;
        public List<IZipInfo> Archives { get; } = new List<IZipInfo>();

        public string BackupFolder
        {
            get => _backupFolder;
            set
            {
                if (SetProperty(ref _backupFolder, value))
                {
                    Archives.Clear();

                    try
                    {
                        var toAdd = _model.ReadZips(value, out var warnings);
                        Archives.AddRange(toAdd);
                        if (!string.IsNullOrWhiteSpace(warnings))
                        {
                            ShowWarning(warnings);
                        }
                    }
                    catch (Exception e)
                    {
                        ShowWarning(e.ToString());
                    }
                }
            }
        }

        public static string HostName { get; } = nameof(RestoreBackupViewModel);

        public ICommand RestoreArchiveCommand { get; }

        public RestoreBackupViewModel(IChangeNotifier changeNotifier,
            IRestoreArchiveModel model,
            string backupFolder)
            : base(changeNotifier)
        {
            _model = model;
            BackupFolder = backupFolder;

            RestoreArchiveCommand = DelegateCommand<IZipInfo>.FromAsyncHandler(OnRestore, info => info != null);
        }

        private async Task OnRestore(IZipInfo obj)
        {
            var vm = new DialogMessage(Properties.Resources.RestoreBackup_Archivating_ReplaceWarning,
                false,
                Properties.Resources.AS_Confirm,
                Properties.Resources.Cancel);

            var result = await DialogHost.Show(vm, HostName);
            if (!Equals(true, result))
                return;


            try
            {
                string warnings;

                using (new AwaitingMessageVm(Properties.Resources.AS_Awaiting, HostName))
                {
                    warnings = await _model.RestoreWithWarnings(obj, Path.GetDirectoryName(BackupFolder));
                    ChangeNotifier.SetChange();
                }

                
                if (!string.IsNullOrWhiteSpace(warnings))
                {
                    await ShowWarning(warnings, false);
                }

                await DialogHost.Show(
                    new DialogMessage(Properties.Resources.RestoreBackup_ArchiveRestored,
                        false,
                        cancelCaption: Properties.Resources.OK), 
                    HostName);
            }

            catch (Exception e)
            {
                await ShowWarning(e.ToString());
            }
        }

        private async Task ShowWarning(string text, bool isError = true)
        {
            Trace.WriteLine(text);

            text = Properties.Resources.RestoreBackup_Archivating_Error + "\n\n" + text;

            var vm = new DialogMessage(text, isError, cancelCaption: Properties.Resources.OK);
            await DialogHost.Show(vm, HostName);
        }
    }
}
