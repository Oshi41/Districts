using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using DistrictsNew.Properties;
using DistrictsNew.ViewModel.Dialogs;
using Mvvm;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel
{
    class MainViewModel : BindableBase
    {
        private readonly IParser _parser;
        private readonly ISerializer _serializer;
        private readonly GoogleApiModel _googleModel;
        private readonly GoogleSyncViewModel _googleSyncVm;
        private bool _isBusy;

        public ICommand OpenManagementcommand { get; }

        public ICommand OpenManageFolder { get; }

        public ICommand OpenSettingsCommand { get; }

        public ICommand OpenCreateArchiveCommand { get; }

        public ICommand OpenBackupFolder { get; }

        public ICommand OpenRestoreArchiveCommand { get; }

        public ICommand OpenGoogleSyncCommand { get; }
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public MainViewModel(IParser parser, ISerializer serializer)
        {
            _isBusy = true;

            _parser = parser;
            _serializer = serializer;
            OpenManagementcommand = new DelegateCommand(OnOpenManage);
            OpenSettingsCommand = new DelegateCommand(OnOpenSettings);
            OpenCreateArchiveCommand = new DelegateCommand(OnOpenCreateArchive);
            OpenRestoreArchiveCommand = new DelegateCommand(OnOpenRestoreArchive);
            OpenGoogleSyncCommand = new DelegateCommand(OnOpenGoogleSync);


            var settings = Settings.Default;

            OpenManageFolder = new DelegateCommand(() => OpenLink(settings.ManageFolder));
            OpenBackupFolder = new DelegateCommand(() => OpenLink(settings.BackupFolder));

            var model = new ArchiveModel(new Archiver());
            _googleModel = new GoogleApiModel(model, model,
                new GoogleDriveApi2(settings.TokensFolder));
            _googleSyncVm = new GoogleSyncViewModel(new SimpleNotifier(), _googleModel);

            TryConnect(settings);
        }

        private async Task TryConnect(Settings settings)
        {
            try
            {
                if (!settings.GoogleSync)
                    return;

                await _googleModel.Connect(settings.Login);
                await _googleModel.DownloadAndReplace(settings.BackupFolder);

                App.Current.MainWindow.Closing += UploadToGoogle;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void UploadToGoogle(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            IsBusy = true;

            if (WindowExtensions
                    .AskUser(Resources.AS_GoogleSync_UploadConfirm,
                        Resources.GoogleApi_Title) == true)
            {
                await _googleSyncVm
                    .HostViewModel
                    .Model
                    .ArchiveAndUpload(_googleSyncVm.Entries, Settings.Default.BackupFolder);
            }
            
            IsBusy = false;

            App.Current.Shutdown();
        }

        private void OnOpenGoogleSync()
        {
            _googleSyncVm.ShowDialog(Properties.Resources.GoogleApi_Title, 380);
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
                settings.Login,
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
