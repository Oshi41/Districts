using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DistrictsLib.Interfaces;
using DistrictsLib.Interfaces.ActionArbiter;
using DistrictsNew.Models;
using DistrictsNew.Models.Interfaces;
using DistrictsNew.ViewModel.Base;
using DistrictsNew.ViewModel.Dialogs.Base;
using DistrictsNew.ViewModel.HostDialogs;
using MaterialDesignThemes.Wpf;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel.Dialogs
{
    class GoogleSyncViewModel : DialogBaseViewModelBase
    {

        private readonly string _backupFolder;

        private bool _isExecuting;
        //private string _author;
        //private bool _isConnected;

        public GoogleConnectViewModel HostViewModel { get; }

        public bool IsExecuting
        {
            get => _isExecuting;
            set => SetProperty(ref _isExecuting, value);
        }

        //public string Author
        //{
        //    get => _author;
        //    set => SetProperty(ref _author, value);
        //}

        //public bool IsConnected
        //{
        //    get => _isConnected;
        //    set => SetProperty(ref _isConnected, value);
        //}

        public ICommand ConnectCommand { get; }
        public ObservableCommand UploadCommand { get; }
        public ObservableCommand DownloadCommand { get; }
        public IReadOnlyCollection<SavingItem> Entries { get; }

        public GoogleSyncViewModel(IChangeNotifier changeNotifier,
                                   IGoogleApiModel model)
            : base(changeNotifier)
        {
            var settings = Properties.Settings.Default;

            var list = new List<SavingItem>
            {
                new SavingItem(true, settings.CardsFolder),
                new SavingItem(true, settings.ManageFolder),
                new SavingItem(true, settings.RestrictionsFolder),
                new SavingItem(true, settings.HomeInfoFolder),
            };

            list.ForEach(x => x.PropertyChanged += NotifyChanges);

            Entries = list;

            _backupFolder = settings.BackupFolder;

            DownloadCommand = ObservableCommand.FromAsyncHandler(OnDownload, OnCanExecuteCommands);
            UploadCommand = ObservableCommand.FromAsyncHandler(OnUpload, OnCanExecuteCommands);

            ConnectCommand = new DelegateCommand(OnOpenConnect, () => !IsExecuting);

            HostViewModel = new GoogleConnectViewModel(model, settings.Login);
        }

        private async void OnOpenConnect()
        {
            await DialogHost.Show(HostViewModel, HostName);
        }

        private void NotifyChanges(object sender, PropertyChangedEventArgs e)
        {
            if (Entries.All(x => !x.IsChecked)
                && sender is SavingItem item)
            {
                item.IsChecked = true;
            }
        }

        private async Task OnUpload()
        {
            IsExecuting = true;

            try
            {
                await HostViewModel.Model.ArchiveAndUpload(Entries, _backupFolder);
                ShowInfo(Properties.Resources.GoogleApi_SyncData);
                ChangeNotifier.SetChange();
            }
            catch (Exception e)
            {
                // красивый текст уже сформирован
                ShowInfo(e.ToString());
            }
            finally
            {
                IsExecuting = false;
            }
        }

        private async Task OnDownload()
        {
            IsExecuting = true;

            try
            {
                await HostViewModel.Model.DownloadAndReplace(_backupFolder);
                ShowInfo(Properties.Resources.GoogleApi_SyncData);
                ChangeNotifier.SetChange();
            }
            catch (Exception e)
            {
                // красивый текст уже сформирован
                ShowInfo(e.ToString());
            }
            finally
            {
                IsExecuting = false;
            }
        }

        private bool OnCanExecuteCommands()
        {
            return HostViewModel.Model.IsConnected()
                && !IsExecuting;
        }
    }
}
