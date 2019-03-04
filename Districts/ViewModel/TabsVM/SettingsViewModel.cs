using System;
using System.IO;
using System.Windows.Input;
using Districts.Helper;
using Districts.MVVM;
using Districts.Settings;
using Districts.WebRequest;

namespace Districts.ViewModel.TabsVM
{
    internal class SettingsViewModel : ObservableObject
    {
        private string _baseFolderPath;
        // оставил, моэет пригодится
        //private string _manageRecordsPath;
        //private string _logPath;
        //private string _restrictionsPath;
        //private string _codesPath;
        //private string _cardsPath;
        //private string _buildingPath;

        private double _maxDoors;
        private string _streets;
        private string _currentText;
        private EditStreetViewModel _editStreetViewModel;


        public SettingsViewModel()
        {
            ToggleSettingsLoad = new Command(SettingsLoadSave);
            SaveSettings = new Command(OnSaveSettings);
            SaveDefault = new Command(OnSaveDefault);
        }

        public ICommand ToggleSettingsLoad { get; set; }
        public ICommand SaveSettings { get; set; }
        public ICommand SaveDefault { get; set; }

        public EditStreetViewModel EditStreetViewModel
        {
            get => _editStreetViewModel;
            set => Set(ref _editStreetViewModel, value);
        }


        // Оставил, может пригодится
        //public string BuildingPath
        //{
        //    get { return _buildingPath; }
        //    set
        //    {
        //        if (value == _buildingPath) return;
        //        _buildingPath = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public string CardsPath
        //{
        //    get { return _cardsPath; }
        //    set
        //    {
        //        if (value == _cardsPath) return;
        //        _cardsPath = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public string HomeInfoPath
        //{
        //    get { return _codesPath; }
        //    set
        //    {
        //        if (value == _codesPath) return;
        //        _codesPath = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public string RestrictionsPath
        //{
        //    get { return _restrictionsPath; }
        //    set
        //    {
        //        if (value == _restrictionsPath) return;
        //        _restrictionsPath = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public string LogPath
        //{
        //    get { return _logPath; }
        //    set
        //    {
        //        if (value == _logPath) return;
        //        _logPath = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public string ManageRecordsPath
        //{
        //    get { return _manageRecordsPath; }
        //    set
        //    {
        //        if (value == _manageRecordsPath) return;
        //        _manageRecordsPath = value;
        //        OnPropertyChanged();
        //    }
        //}

        public double MaxDoors
        {
            get => _maxDoors;
            set
            {
                if (value == _maxDoors) return;
                _maxDoors = value;
                OnPropertyChanged();
            }
        }

        public string BaseFolderPath
        {
            get => _baseFolderPath;
            set
            {
                if (value == _baseFolderPath) return;
                _baseFolderPath = value;
                OnPropertyChanged();
            }
        }

        public string CurrentText
        {
            get => _currentText;
            set => Set(ref _currentText, value);
        }

        private void OnSaveDefault()
        {
            var settings = ApplicationSettings.GetDefault();
            settings.Write();

            SettingsLoadSave(false);
        }

        private void OnSaveSettings()
        {
            var settings = ApplicationSettings.GetDefault();

            settings.BaseFolder = BaseFolderPath;
            settings.MaxDoors = (int)MaxDoors;
            //settings.BuildingPath = BuildingPath;
            //settings.CardsPath = CardsPath;
            //settings.HomeInfoPath = HomeInfoPath;
            //settings.RestrictionsPath = RestrictionsPath;
            //settings.LogPath = LogPath;
            //settings.ManageRecordsPath = ManageRecordsPath;

            settings.Write();

            var text = string.Join("\n", EditStreetViewModel.Streets);
            try
            {
                File.WriteAllText(settings.StreetsPath, text);
            }
            catch (Exception e)
            {
                Tracer.WriteError(e);
            }

            SettingsLoadSave(false);
        }

        private void SettingsLoadSave(object obj)
        {
            if (true.Equals(obj))
            {
                var settings = ApplicationSettings.ReadOrCreate();

                BaseFolderPath = settings.BaseFolder;
                MaxDoors = settings.MaxDoors;
                //BuildingPath = settings.BuildingPath;
                //CardsPath = settings.CardsPath;
                //HomeInfoPath = settings.HomeInfoPath;
                //RestrictionsPath = settings.RestrictionsPath;
                //LogPath = settings.LogPath;
                //ManageRecordsPath = settings.ManageRecordsPath;

                var temp = File
                        .ReadAllText(settings.StreetsPath)
                        .RemoveEmptyLines()
                        .Split('\n');

                EditStreetViewModel = new EditStreetViewModel(temp, new StreetDownloader());
            }
            else
            {
                BaseFolderPath = null;
                MaxDoors = 0;
                //BuildingPath = null;
                //CardsPath = null;
                //HomeInfoPath = null;
                //RestrictionsPath = null;
                //LogPath = null;
                //ManageRecordsPath = null;
                EditStreetViewModel = null;
            }
        }
    }
}