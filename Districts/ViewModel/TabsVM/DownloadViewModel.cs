using System;
using System.IO;
using System.Windows.Input;
using Districts.Helper;
using Districts.New.Implementation;
using Districts.Settings;
using Districts.Settings.v1;
using Districts.Singleton;
using Districts.WebRequest;
using Microsoft.Expression.Interactivity.Core;
using Mvvm;
using Mvvm.Commands;

namespace Districts.ViewModel.TabsVM
{
    internal class DownloadViewModel : BindableBase
    {
        private bool _isLoading;
        private bool _isPropChanged1;
        private string _streets;
        private IMessageHelper _messageHelper = IoC.Instance.Get<IMessageHelper>();

        public DownloadViewModel()
        {
            DownloadCommand = new DelegateCommand<object>(OnDownload, o => !_isLoading);
            LoadStreetCommand = new ActionCommand(OnLoadStreet);
            SaveActionCommand = new DelegateCommand(OnSave, () => IsPropChanged);
        }


        private bool IsPropChanged
        {
            get => _isPropChanged1;
            set
            {
                if (_isPropChanged1 != value)
                {
                    _isPropChanged1 = value;
                    SaveActionCommand.RaiseCanExecuteChanged();
                }
            }
        }


        public ICommand DownloadCommand { get; set; }
        public ICommand LoadStreetCommand { get; set; }
        public DelegateCommandBase SaveActionCommand { get; set; }

        public string Streets
        {
            get => _streets;
            set
            {
                if (value == _streets) return;
                _streets = value;
                OnPropertyChanged();

                IsPropChanged = true;
            }
        }


        private void OnSave()
        {
            var file = ApplicationSettings.ReadOrCreate().StreetsPath;
            try
            {
                File.WriteAllText(file, Streets.RemoveEmptyLines());
            }
            catch (Exception e)
            {
                Tracer.Tracer.WriteError(e);
            }

            IsPropChanged = false;
        }

        private void OnLoadStreet(object param)
        {
            // загружаю
            if (true.Equals(param))
            {
                var file = ApplicationSettings.ReadOrCreate().StreetsPath;
                var text = File.ReadAllText(file);
                Streets = text.RemoveEmptyLines();

                IsPropChanged = false;
            }
            // очищаю
            else
            {
                Streets = string.Empty;
            }
        }

        private async void OnDownload(object obj)
        {
            try
            {
                _isLoading = true;
                var downloader = new MainDownloader();
                _messageHelper.ShowAwait();
                await downloader.DownloadInfo();
            }
            catch (Exception e)
            {
                Tracer.Tracer.WriteError(e);
            }
            finally
            {
                _isLoading = false;
                _messageHelper.ShowDone();
            }
        }
    }
}