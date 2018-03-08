using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Districts.MVVM;
using Districts.Settings;

namespace Districts.ViewModel.TabsVM
{
    class OpenFolderViewModel : ObservableObject
    {
        private ApplicationSettings _settings = ApplicationSettings.ReadOrCreate();
        public ICommand OpenCommand { get; set; }

        public ApplicationSettings Settings
        {
            get { return _settings; }
            set
            {
                if (Equals(value, _settings)) return;
                _settings = value;
                OnPropertyChanged();
            }
        }

        public OpenFolderViewModel()
        {
            OpenCommand = new Command(OpenFolder);
        }

        private void OpenFolder(object obj)
        {
            if (obj is string path)
            {
                if (Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                Process.Start(path);
            }
        }
    }
}
