﻿using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Districts.MVVM;
using Districts.Settings;

namespace Districts.ViewModel.TabsVM
{
    internal class OpenFolderViewModel : ObservableObject
    {
        private ApplicationSettings _settings = ApplicationSettings.ReadOrCreate();

        public OpenFolderViewModel()
        {
            OpenCommand = new Command(OpenFolder);
        }

        public ICommand OpenCommand { get; set; }

        public ApplicationSettings Settings
        {
            get => _settings;
            set
            {
                if (Equals(value, _settings)) return;
                _settings = value;
                OnPropertyChanged();
            }
        }

        private void OpenFolder(object obj)
        {
            if (obj is string path)
            {
                if (Directory.Exists(path)) Directory.CreateDirectory(path);

                Process.Start(path);
            }
        }
    }
}