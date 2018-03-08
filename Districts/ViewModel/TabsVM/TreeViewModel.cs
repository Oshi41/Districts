using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.JsonClasses.Base;
using Districts.MVVM;
using Districts.Settings;
using Districts.Views;
using Microsoft.Expression.Interactivity.Core;
using Newtonsoft.Json;

namespace Districts.ViewModel.TabsVM
{
    class TreeViewModel : ObservableObject
    {
        private bool _canSave;
        public List<Building> Homes { get; private set; } = new List<Building>();
        public List<ForbiddenElement> Rules { get; private set; } = new List<ForbiddenElement>();
        public List<Codes> Codes { get; private set; } = new List<Codes>();

        private Dictionary<Building, ForbiddenElement> _mappedRules = new Dictionary<Building, ForbiddenElement>();
        private Dictionary<Building, Codes> _mappedCodes = new Dictionary<Building, Codes>();

        public ObservableCollection<StreetViewModel> Streets { get; set; } = new ObservableCollection<StreetViewModel>();

        public Command SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand EditCommand { get; set; }


        public TreeViewModel()
        {
            LoadCommand = new Command(OnLoad);
            SaveCommand = new Command(Save, () => _canSave);
            DeleteCommand = new Command(OnDelete);
            EditCommand = new Command(OnEdit);
        }

        private void OnEdit(object obj)
        {
            if (obj is Building home)
            {
                var rule = _mappedRules[home];
                var code = _mappedCodes[home];

                var vm = new EditHomeInfoViewModel(home, rule, code);
                var window = new EditHomeInfo { DataContext = vm };
                window.ShowDialog();

                if (window.DialogResult == true)
                {
                    OnPropertyChanged(nameof(_canSave));
                }
            }
        }

        private void OnDelete(object obj)
        {
            if (obj is Building home)
            {
                _mappedCodes.Remove(home);
                _mappedRules.Remove(home);

                foreach (var street in Streets)
                {
                    var find = street.Homes.FirstOrDefault(x => home.IsTheSameObject(x));
                    if (find != null)
                    {
                        street.Homes.Remove(find);
                        break;
                    }
                }
            }
        }

        private void OnLoad(object param)
        {
            Streets.Clear();

            // загружаем с нуля
            if (param.Equals(true))
            {
                Load();
            }
            // очищаем все
            else
            {
                _mappedCodes.Clear();
                _mappedRules.Clear();
                Homes.Clear();
                Codes.Clear();
                Rules.Clear();
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(_canSave))
            {
                _canSave = true;
                SaveCommand.OnCanExecuteChanged();
            }
        }

        #region Load / Save

        private void Load()
        {
            // загрузил все дома
            var allHomes = LoadingWork.LoadSortedHomes();
            foreach (var street in allHomes)
            {
                Homes.AddRange(street.Value);
                AddStreet(street.Value);

            }
            // загрузил правила
            var allRules = LoadingWork.LoadRules();
            foreach (var street in allRules)
            {
                Rules.AddRange(street.Value);
            }

            // загрузил коды
            var allCodes = LoadingWork.LoadCodes();
            foreach (var street in allCodes)
            {
                Codes.AddRange(street.Value);
            }

            // смапировал
            foreach (var home in Homes)
            {
                var rule = Rules.FirstOrDefault(x => home.IsTheSameObject(x))
                           ?? new ForbiddenElement(home);
                var code = Codes.FirstOrDefault(x => home.IsTheSameObject(x))
                           ?? new Codes(home);

                _mappedCodes.Add(home, code);
                _mappedRules.Add(home, rule);
            }
        }

        private void Save()
        {
            var settings = ApplicationSettings.ReadOrCreate();

            Dictionary<string, List<Codes>> codes = _mappedCodes.Values
                .GroupBy(x => x.Street)
                .ToDictionary(x => x.Key, x => x.GetEnumerator().ToIEnumerable().ToList());

            // очистил предыдущие значения
            Helper.Helper.ClearFolder(settings.CodesPath);

            foreach (var street in codes.Keys)
            {
                var writableObj = codes[street];
                var writableStr = JsonConvert.SerializeObject(writableObj, Formatting.Indented);
                var filepath = Path.Combine(settings.CodesPath, street);
                File.WriteAllText(filepath, writableStr);
            }


            var rules = _mappedRules.Values.GroupBy(x => x.Street)
                .ToDictionary(x => x.Key, x => x.GetEnumerator().ToIEnumerable().ToList());

            // очистил предыдущие значения
            Helper.Helper.ClearFolder(settings.RestrictionsPath);

            foreach (var street in rules.Keys)
            {
                var writableObj = rules[street];
                var writableStr = JsonConvert.SerializeObject(writableObj, Formatting.Indented);
                var filepath = Path.Combine(settings.RestrictionsPath, street);
                File.WriteAllText(filepath, writableStr);
            }

            Dictionary<string, List<Building>> homes = _mappedCodes.Keys
                .GroupBy(x => x.Street)
                .ToDictionary(x => x.Key, x => x.GetEnumerator().ToIEnumerable().ToList());

            // очистил предыдущие значения
            Helper.Helper.ClearFolder(settings.BuildingPath);

            foreach (var street in homes.Keys)
            {
                var writableObj = homes[street];
                var writableStr = JsonConvert.SerializeObject(writableObj, Formatting.Indented);
                var filepath = Path.Combine(settings.BuildingPath, street);
                File.WriteAllText(filepath, writableStr);
            }

            OnLoad(false);

            _canSave = false;
        }

        private void AddStreet(List<Building> homes)
        {
            var street = new StreetViewModel(homes);
            Streets.Add(street);
        }
        #endregion
    }
}
