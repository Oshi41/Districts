﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.MVVM;
using Districts.Settings;
using Districts.Views;
using Newtonsoft.Json;

namespace Districts.ViewModel
{
    //public class EditTreeViewModel : ObservableObject
    //{
    //    public EditTreeViewModel()
    //    {
    //        Load();
    //        InitView();
    //    }

    //    #region Model stuff

    //    public List<Building> Homes { get; private set; } = new List<Building>();
    //    public List<ForbiddenElement> Rules { get; private set; } = new List<ForbiddenElement>();
    //    public List<HomeInfo> HomeInfo { get; private set; } = new List<HomeInfo>();

    //    private Dictionary<Building, ForbiddenElement> _mappedRules = new Dictionary<Building, ForbiddenElement>();
    //    private Dictionary<Building, HomeInfo> _mappedHomeInfo = new Dictionary<Building, HomeInfo>();

    //    #region load/save

    //    private void Load()
    //    {
    //        // загрузил все дома
    //        var allHomes = LoadingWork.LoadSortedHomes();
    //        foreach (var street in allHomes)
    //        {
    //            Homes.AddRange(street.Value);
    //            //AddStreet(street.Value);
                
    //        }
    //        // загрузил правила
    //        var allRules = LoadingWork.LoadRules();
    //        foreach (var street in allRules)
    //        {
    //            Rules.AddRange(street.Value);
    //        }

    //        // загрузил коды
    //        var allHomeInfo = LoadingWork.LoadCodes();
    //        foreach (var street in allHomeInfo)
    //        {
    //            HomeInfo.AddRange(street.Value);
    //        }

    //        // смапировал
    //        foreach (var home in Homes)
    //        {
    //            var rule = Rules.FirstOrDefault(x => home.IsTheSameObject(x))
    //                       ?? new ForbiddenElement(home);
    //            var code = HomeInfo.FirstOrDefault(x => home.IsTheSameObject(x))
    //                       ?? new HomeInfo(home);

    //            _mappedHomeInfo.Add(home, code);
    //            _mappedRules.Add(home, rule);
    //        }
    //    }

    //    private void Save()
    //    {
    //        var settings = ApplicationSettings.ReadOrCreate();

    //        Dictionary<string, List<HomeInfo>> HomeInfo = _mappedHomeInfo.Values
    //            .GroupBy(x => x.Street)
    //            .ToDictionary(x => x.Key, x => x.GetEnumerator().ToIEnumerable().ToList());
            
    //        // очистил предыдущие значения
    //        Helper.Helper.ClearFolder(settings.HomeInfoPath);

    //        foreach (var street in HomeInfo.Keys)
    //        {
    //            var writableObj = HomeInfo[street];
    //            var writableStr = JsonConvert.SerializeObject(writableObj, Formatting.Indented);
    //            var filepath = Path.Combine(settings.HomeInfoPath, street);
    //            File.WriteAllText(filepath, writableStr);
    //        }


    //        var rules = _mappedRules.Values.GroupBy(x => x.Street)
    //            .ToDictionary(x => x.Key, x => x.GetEnumerator().ToIEnumerable().ToList());

    //        // очистил предыдущие значения
    //        Helper.Helper.ClearFolder(settings.RestrictionsPath);

    //        foreach (var street in rules.Keys)
    //        {
    //            var writableObj = rules[street];
    //            var writableStr = JsonConvert.SerializeObject(writableObj, Formatting.Indented);
    //            var filepath = Path.Combine(settings.RestrictionsPath, street);
    //            File.WriteAllText(filepath, writableStr);
    //        }
    //    }

    //    private void SaveHome()
    //    {
    //        var settings = ApplicationSettings.ReadOrCreate();

            
    //    }

    //    #endregion

    //    #endregion

    //    #region View stuff

    //    #region Props


    //    /// <summary>
    //    /// Все наши улицы
    //    /// </summary>
    //    public ObservableCollection<StreetViewModel> Streets
    //    {
    //        get { return _streets; }
    //        set
    //        {
    //            if (Equals(value, _streets)) return;
    //            _streets = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //    public Command DeleteHome { get; set; }
    //    public ICommand EditCommand { get; set; }
    //    public ICommand UpdateSelectedItem { get; set; }

    //    public Building CurrentHome { get; set; }

    //    public ICommand SaveCommand { get; set; }


    //    private ObservableCollection<StreetViewModel> _streets = new ObservableCollection<StreetViewModel>();


    //    #endregion

    //    private void InitView()
    //    {
    //        UpdateSelectedItem = new Command(UpdateCurrentItem);
    //        DeleteHome = new Command(OnDeleteCommand, OnCanDeleteCommand);
    //        SaveCommand = new Command(OnSave);
    //        EditCommand = new Command(LunchEdit);
    //    }

    //    private void LunchEdit(object o)
    //    {
    //        if (CurrentHome == null)
    //            return;
            
    //        var home = CurrentHome;
    //        var rule = _mappedRules[home];
    //        var code = _mappedHomeInfo[home];

    //        var vm= new EditHomeInfoViewModel(home, rule, code);
    //        var window = new EditHomeInfo() {DataContext = vm};
    //        window.ShowDialog();
    //    }

    //    #region Command handlers

        

    //    private void OnDeleteCommand(object obj)
    //    {
    //        if (!OnCanDeleteCommand(obj))
    //            return;

    //        _mappedHomeInfo.Remove(CurrentHome);
    //        _mappedRules.Remove(CurrentHome);

    //        foreach (var street in Streets)
    //        {
    //            var find = street.Homes.FirstOrDefault(x => CurrentHome.IsTheSameObject(x));
    //            if (find != null)
    //            {
    //                street.Homes.Remove(find);
    //                break;
    //            }
    //        }
    //    }

    //    private bool OnCanDeleteCommand(object obj)
    //    {
    //        if (CurrentHome != null)
    //            return true;

    //        var temp = obj as Building;
    //        if (temp != null)
    //        {
    //            CurrentHome = temp;
    //            return true;
    //        }

    //        return false;
    //    }


    //    private void UpdateCurrentItem(object obj)
    //    {
    //        CurrentHome = obj as Building;

    //        DeleteHome.OnCanExecuteChanged();
    //    }

    //    private void OnSave()
    //    {
    //        Save();
    //        SaveHome();
    //    }
    //    #endregion

    //    #endregion
    //}
}
