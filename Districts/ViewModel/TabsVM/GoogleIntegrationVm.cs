using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Districts.Goggle;
using Districts.Helper;
using Districts.MVVM;
using Districts.Parser;

namespace Districts.ViewModel.TabsVM
{
    class GoogleIntegrationVm : ObservableObject
    {
        private readonly IGoogleDriveApi _api;
        private string _login;
        private bool _connected;

        public string Login
        {
            get => _login;
            set => Set(ref _login, value);
        }

        public bool Connected
        {
            get => _connected;
            set => Set(ref _connected, value);
        }

        public ICommand ConnectCommand { get; }
        public ICommand SyncCommand { get; }
        public ICommand DownloadCommand { get; }

        public GoogleIntegrationVm()
        {
            _api = new GoogleDriveApi(new Parser.Parser());

            ConnectCommand = new CommandAsync(OnConnect, () => !string.IsNullOrWhiteSpace(Login) && !_connected);
            SyncCommand = new CommandAsync(OnUpload, () => _connected);
            DownloadCommand = new CommandAsync(OnDownload, () => _connected);
        }

        private async Task OnDownload()
        {
            try
            {
                await _api.DownloadAndUpdate();
                MessageHelper.ShowDoneBubble();
            }
            catch (Exception e)
            {
                Tracer.WriteError(e);
                MessageHelper.ShowMessage(e.Message, "Ошибка");
            }
        }

        private async Task OnConnect()
        {
            try
            {
                await _api.Connect(Login);
                Connected = true;
            }
            catch (Exception e)
            {
                Tracer.WriteError(e);
                MessageHelper.ShowMessage(e.Message, "Ошибка");
            }
        }

        private async Task OnUpload()
        {
            try
            {
                await _api.Upload();
                MessageHelper.ShowDoneBubble();
            }
            catch (Exception e)
            {
                Tracer.WriteError(e);
                MessageHelper.ShowMessage(e.Message, "Ошибка");
            }
        }
    }
}
