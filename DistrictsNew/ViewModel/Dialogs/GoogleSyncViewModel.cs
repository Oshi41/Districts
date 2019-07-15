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
        private readonly string _baseFolder;
        //private string _author;
        //private bool _isConnected;

        public GoogleConnectViewModel HostViewModel { get; }

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
        public ICommand UploadCommand { get; }
        public ICommand DownloadCommand { get; }
        public IReadOnlyCollection<SavingItem> Entries { get; }

        static GoogleSyncViewModel()
        {
            HostName = nameof(GoogleSyncViewModel);
        }

        public GoogleSyncViewModel(IChangeNotifier changeNotifier,
                                   IGoogleApiModel model,
                                   string baseFolder)
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

            _baseFolder = baseFolder;

            DownloadCommand = new DelegateCommand(OnDownload, OnIsConnected);
            UploadCommand = new DelegateCommand(OnUpload, OnIsConnected);
            ConnectCommand = new DelegateCommand(OnOpenConnect, () => !OnIsConnected());

            HostViewModel = new GoogleConnectViewModel(model, settings.Login);
        }

        private async void OnOpenConnect()
        {
            await DialogHost.Show(HostViewModel, HostName, RestrictClosing);
        }

        private void RestrictClosing(object sender, DialogClosingEventArgs e)
        {
            if (!OnIsConnected())
            {
                e.Cancel();
            }
        }

        private void NotifyChanges(object sender, PropertyChangedEventArgs e)
        {
            if (Entries.All(x => !x.IsChecked)
                && sender is SavingItem item)
            {
                item.IsChecked = true;
            }
        }

        private void OnUpload()
        {
            try
            {
                HostViewModel.Model.ArchiveAndUpload(Entries, _baseFolder);
                ShowInfo(Properties.Resources.GoogleApi_SyncData);
                this.ChangeNotifier.SetChange();
            }
            catch (Exception e)
            {
                ShowInfo(e.ToString());
            }
        }

        private void OnDownload()
        {
            try
            {
                HostViewModel.Model.DownloadAndReplace(_baseFolder);
                ShowInfo(Properties.Resources.GoogleApi_SyncData);
                this.ChangeNotifier.SetChange();
            }
            catch (Exception e)
            {
                ShowInfo(e.ToString());
            }
        }

        private bool OnIsConnected()
        {
            return HostViewModel.Model.IsConnected();
        }
    }
}
