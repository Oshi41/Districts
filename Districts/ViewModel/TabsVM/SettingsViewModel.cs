using System;
using System.IO;
using System.Windows.Input;
using Districts.Helper;
using Districts.Settings;
using Microsoft.Expression.Interactivity.Core;
using Mvvm;

namespace Districts.ViewModel.TabsVM
{
    internal class SettingsViewModel : BindableBase
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


        public SettingsViewModel()
        {
            ToggleSettingsLoad = new ActionCommand(SettingsLoadSave);
            SaveSettings = new ActionCommand(OnSaveSettings);
            SaveDefault = new ActionCommand(OnSaveDefault);
        }

        public ICommand ToggleSettingsLoad { get; set; }
        public ICommand SaveSettings { get; set; }
        public ICommand SaveDefault { get; set; }


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

        public string Streets
        {
            get => _streets;
            set
            {
                if (value == _streets) return;
                _streets = value;
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
            settings.MaxDoors = (int) MaxDoors;
            //settings.BuildingPath = BuildingPath;
            //settings.CardsPath = CardsPath;
            //settings.HomeInfoPath = HomeInfoPath;
            //settings.RestrictionsPath = RestrictionsPath;
            //settings.LogPath = LogPath;
            //settings.ManageRecordsPath = ManageRecordsPath;

            settings.Write();

            var text = Streets.RemoveEmptyLines();
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

                var temp = File.ReadAllText(settings.StreetsPath);
                Streets = temp.RemoveEmptyLines();
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
                Streets = null;
            }
        }
    }
}