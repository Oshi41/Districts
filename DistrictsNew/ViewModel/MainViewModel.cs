using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DistrictsLib.Implementation;
using DistrictsLib.Implementation.ActionArbiter;
using DistrictsLib.Implementation.Archiver;
using DistrictsLib.Implementation.ChangesNotifier;
using DistrictsLib.Implementation.GoogleApi;
using DistrictsLib.Interfaces;
using DistrictsNew.Extensions;
using DistrictsNew.Models;
using DistrictsNew.ViewModel.Dialogs;
using Mvvm;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel
{
    class MainViewModel : BindableBase
    {
        private readonly IParser _parser;
        private readonly ISerializer _serializer;

        public ICommand OpenManagementcommand { get; }

        public ICommand OpenManageFolder { get; }

        public ICommand OpenSettingsCommand { get; }

        public ICommand OpenCreateArchiveCommand { get; }

        public ICommand OpenBackupFolder { get; }

        public ICommand OpenRestoreArchiveCommand { get; }

        public ICommand OpenGoogleSyncCommand { get; }

        public MainViewModel(IParser parser, ISerializer serializer)
        {
            _parser = parser;
            _serializer = serializer;
            OpenManagementcommand = new DelegateCommand(OnOpenManage);
            OpenSettingsCommand = new DelegateCommand(OnOpenSettings);
            OpenCreateArchiveCommand = new DelegateCommand(OnOpenCreateArchive);
            OpenRestoreArchiveCommand = new DelegateCommand(OnOpenRestoreArchive);
            OpenGoogleSyncCommand = new DelegateCommand(OnOpenGoogleSync);


            OpenManageFolder = new DelegateCommand(() => OpenLink(Properties.Settings.Default.ManageFolder));
            OpenBackupFolder = new DelegateCommand(() => OpenLink(Properties.Settings.Default.BackupFolder));
        }

        private void OnOpenGoogleSync()
        {
            var model = new ArchiveModel(new Archiver());

            var vm = new GoogleSyncViewModel(new SimpleNotifier(), 
                new GoogleApiModel(model, model, 
                    new GoogleDriveApi2(Properties.Settings.Default.TokensFolder)));

            vm.ShowDialog(Properties.Resources.GoogleApi_Title, 380);
        }

        private void OnOpenRestoreArchive()
        {
            var vm = new RestoreBackupViewModel(new SimpleNotifier(), 
                new ArchiveModel(new Archiver()), 
                Properties.Settings.Default.BackupFolder);

            vm.ShowDialog(Properties.Resources.RestoreBackup_Title, 720, 440);
        }

        private void OnOpenCreateArchive()
        {
            var vm = new CreateBackupViewModel(new SimpleNotifier(),
                new ArchiveModel(new Archiver()), 
                Path.GetDirectoryName(Properties.Settings.Default.BackupFolder),
                Properties.Settings.Default.BackupFolder,
                new ActionArbiter());

            vm.ShowDialog(Properties.Resources.CreateBackup_Title, 400);
        }

        private void OnOpenSettings()
        {
            var settings = Properties.Settings.Default;

            var vm = new SettingsViewModel(new RememberChangesNotify(),
                Path.GetDirectoryName(settings.StreetsFile),
                settings.MaxDoors,
                settings.GoogleSync,
                _parser.LoadStreets(),
                new TimedAction(new SafeThreadActionArbiter(new ActionArbiter())),
                new StreetDownload());

            if (vm.ShowDialog(Properties.Resources.Settings_Title,
                    400) == true)
            {
                settings.GoogleSync = vm.GoogleSync;
                settings.MaxDoors = vm.DoorCount;
                settings.Propagate(vm.BaseFolder);

                _serializer.SaveStreets(vm.Streets);
            }
        }

        private void OnOpenManage()
        {
            var loaded = _parser.LoadManage();
            var vm = new ManageViewModel(loaded,
                new SimpleNotifier(),
                new TimedAction(new SafeThreadActionArbiter(new ActionArbiter())));

            if (vm.ShowDialog(Properties.Resources.Manage_Title, 600, 600 / 1.5) == true
                && vm.IsChanged)
            {
                _serializer.SaveManage(vm.Origin);
            }
        }

        private void OpenLink(string uri)
        {
            try
            {
                Process.Start(uri);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }
    }
}
