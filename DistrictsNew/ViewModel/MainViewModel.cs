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
using DistrictsLib.Implementation.ChangesNotifier;
using DistrictsLib.Interfaces;
using DistrictsNew.Extensions;
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

        public MainViewModel(IParser parser, ISerializer serializer)
        {
            _parser = parser;
            _serializer = serializer;
            OpenManagementcommand = new DelegateCommand(OnOpenManage);
            OpenSettingsCommand = new DelegateCommand(OnOpenSettings);
            OpenCreateArchiveCommand = new DelegateCommand(OnOpenCreateArchive);

            OpenManageFolder = new DelegateCommand(() => OpenLink(Properties.Settings.Default.ManageFolder));
        }

        private void OnOpenCreateArchive()
        {
            var vm = new CreateBackupViewModel(new SimpleNotifier(),
                new Archiver(),
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
                _parser.LoadStreets(),
                new TimedAction(new SafeThreadActionArbiter(new ActionArbiter())),
                new StreetDownload());

            if (vm.ShowDialog(Properties.Resources.Settings_Title,
                    400) == true)
            {
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
            Process.Start(uri);
        }
    }
}
