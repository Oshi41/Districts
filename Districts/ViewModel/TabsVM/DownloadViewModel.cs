using System;
using System.IO;
using System.Windows.Input;
using Districts.Helper;
using Districts.MVVM;
using Districts.Settings;
using Districts.WebRequest;

namespace Districts.ViewModel.TabsVM
{
    internal class DownloadViewModel : ObservableObject
    {
        private bool _isLoading;
        private bool _isPropChanged1;
        private string _streets;

        public DownloadViewModel()
        {
            DownloadCommand = new Command(OnDownload, o => !_isLoading);
            LoadStreetCommand = new Command(OnLoadStreet);
            SaveCommand = new Command(OnSave, () => IsPropChanged);
        }


        private bool IsPropChanged
        {
            get => _isPropChanged1;
            set
            {
                if (_isPropChanged1 != value)
                {
                    _isPropChanged1 = value;
                    SaveCommand.OnCanExecuteChanged();
                }
            }
        }


        public ICommand DownloadCommand { get; set; }
        public ICommand LoadStreetCommand { get; set; }
        public Command SaveCommand { get; set; }

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
                Tracer.WriteError(e);
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
                MessageHelper.ShowAwait();
                await downloader.DownloadInfo();
            }
            catch (Exception e)
            {
                Tracer.WriteError(e);
            }
            finally
            {
                _isLoading = false;
                MessageHelper.ShowDone();
            }
        }
    }
}