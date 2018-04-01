using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Districts.Comparers;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.JsonClasses.Base;
using Districts.MVVM;
using Districts.Settings;
using Districts.Views;
using iTextSharp.text.xml;
using Newtonsoft.Json;

namespace Districts.ViewModel.TabsVM
{
    class TreeViewModel : ObservableObject
    {
        #region Fields

        //public List<Building> Homes { get; } = new List<Building>();
        //public List<ForbiddenElement> Rules { get; } = new List<ForbiddenElement>();
        //public List<HomeInfo> Codes { get; } = new List<HomeInfo>();

        private readonly HomeMap _map = new HomeMap();
        //private readonly Dictionary<Building, ForbiddenElement> _mappedRules = new Dictionary<Building, ForbiddenElement>();
        //private readonly Dictionary<Building, HomeInfo> _mappedHomeInfo = new Dictionary<Building, HomeInfo>();

        private ForbiddenElement _selectedForbiddenElement;
        private HomeInfo _selectedHomeInfo;
        private Building _selectedBuilding;
        private bool _canSave;

        #endregion

        #region Properties

        public ObservableCollection<StreetViewModel> Streets { get; set; } = new ObservableCollection<StreetViewModel>();

        public Command SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SetSelectedItemCommand { get; set; }

        public Building SelectedBuilding
        {
            get { return _selectedBuilding; }
            set
            {
                if (Equals(value, _selectedBuilding)) return;
                _selectedBuilding = value;
                OnPropertyChanged(nameof(SelectedBuilding));
            }
        }

        public HomeInfo SelectedHomeInfo
        {
            get { return _selectedHomeInfo; }
            set
            {
                if (Equals(value, _selectedHomeInfo)) return;
                _selectedHomeInfo = value;
                OnPropertyChanged(nameof(SelectedHomeInfo));
            }
        }

        public ForbiddenElement SelectedForbiddenElement
        {
            get { return _selectedForbiddenElement; }
            set
            {
                if (Equals(value, _selectedForbiddenElement)) return;
                _selectedForbiddenElement = value;
                OnPropertyChanged(nameof(SelectedForbiddenElement));
            }
        }

        private bool CanSave
        {
            get { return _canSave; }
            set
            {
                if (Equals(value, _canSave)) return;
                _canSave = value;

                OnPropertyChanged(nameof(CanSave));
                SaveCommand.OnCanExecuteChanged();
            }
        }

        #endregion

        public TreeViewModel()
        {
            LoadCommand = new Command(OnLoad);
            SaveCommand = new Command(SaveNew, () => CanSave);
            DeleteCommand = new Command(OnDelete);
            EditCommand = new Command(OnEdit);
            SetSelectedItemCommand = new Command(SetSelectedItem);
        }

        #region Methods

        private void OnEdit(object obj)
        {
            var vm = new EditHomeInfoViewModel(SelectedBuilding, SelectedForbiddenElement, SelectedHomeInfo);
            var window = new EditHomeInfo { DataContext = vm };
            window.ShowDialog();

            if (window.DialogResult == true)
            {
                CanSave = true;

                if (vm.CountingChanged)
                    CheckCounting(vm);
            }
        }

        private void OnDelete(object obj)
        {
            var isDeleted = _map.Remove(SelectedBuilding);
            //_map.Remove(SelectedBuilding);

            foreach (var street in Streets)
            {
                var find = street.Homes.FirstOrDefault(x => SelectedBuilding.IsTheSameObject(x));
                if (find != null)
                {
                    street.Homes.Remove(find);
                    isDeleted = true;
                    break;
                }
            }

            CanSave = isDeleted;
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
                _map.Clear();
                //_mappedHomeInfo.Clear();
                //_mappedRules.Clear();
                //Homes.Clear();
                //Codes.Clear();
                //Rules.Clear();
            }
        }

        private void SetSelectedItem(object obj)
        {
            SelectedBuilding = null;
            SelectedForbiddenElement = null;
            SelectedHomeInfo = null;

            if (obj is Building home)
            {
                SelectedBuilding = home;

                //if (_map.ContainsKey(home))
                SelectedHomeInfo = _map.GetHomeInfo(home);

                //if (_map.ContainsKey(home))
                SelectedForbiddenElement = _map.GetRule(home);

            }
        }


        private void CheckCounting(EditHomeInfoViewModel vm)
        {
            var algorythm = vm.CountingAlgorythm;

            if (SelectedBuilding == null
                || algorythm == null
                /* настроили для одного здания, изменения УЖЕ сохранены
                // и не нуждаются в дальнейших действиях
                || algorythm == CountingTypes.Custom*/)
            {
                return;
            }

            // смещаем номера 
            if (algorythm.Value == CountingTypes.Custom && vm.InheritSettings)
            {
                var findCondition = BaseFindableObject.ReturnConditions.MoreThen;
                var affectedList = _map.FindSameHouseNumber(SelectedBuilding, findCondition);
                var step = vm.CustomChangedStep;

                foreach (var full in affectedList)
                {
                    full.HomeInfo.Begin += step;
                }
            }

            // умный счетчик номеров для всех корпусов
            if (algorythm.Value == CountingTypes.AutomaticIncrement)
            {
                var findCondition = BaseFindableObject.ReturnConditions.AllWithSlashCheck;

                var affectedList = _map.FindSameHouseNumber(SelectedBuilding, findCondition);
                var current = 1;
                foreach (var full in affectedList)
                {
                    // Меняю для всех корпусов, только если это разрешено
                    if (vm.InheritSettings || Equals(SelectedBuilding, full.Building))
                        full.HomeInfo.Begin = current;
                    current += full.Building.Doors;
                }
            }

            // нумеруем с единицы
            if (algorythm.Value == CountingTypes.Regular)
            {
                var findCondition = vm.InheritSettings
                    ? BaseFindableObject.ReturnConditions.AllWithSlashCheck
                    : BaseFindableObject.ReturnConditions.Self;

                var affectedList = _map.FindSameHouseNumber(SelectedBuilding, findCondition);
                foreach (var full in affectedList)
                {
                    full.HomeInfo.Begin = 1;
                }
            }
        }

        #endregion

        #region Load / Save

        private void Load()
        {
            // загрузил все дома
            var allHomes = LoadingWork.LoadSortedHomes();
            var homes = new List<Building>();
            foreach (var street in allHomes)
            {
                homes.AddRange(street.Value);
                AddStreet(street.Value);

            }
            // загрузил правила
            var allRules = LoadingWork.LoadRules();
            var rules = new List<ForbiddenElement>();
            foreach (var street in allRules)
            {
                rules.AddRange(street.Value);
            }

            // загрузил коды
            var allCodes = LoadingWork.LoadCodes();
            var codes = new List<HomeInfo>();
            foreach (var street in allCodes)
            {
                codes.AddRange(street.Value);
            }

            // смапировал
            foreach (var home in homes)
            {
                var rule = rules.FirstOrDefault(x => home.IsTheSameObject(x))
                           ?? new ForbiddenElement(home);
                var code = codes.FirstOrDefault(x => home.IsTheSameObject(x))
                           ?? new HomeInfo(home);

                _map.Add(home, rule, code);
            }
        }

        private void SaveNew()
        {
            var settings = ApplicationSettings.ReadOrCreate();

            // очистил предыдущие значения
            Helper.Helper.ClearFolder(settings.BuildingPath);
            Helper.Helper.ClearFolder(settings.HomeInfoPath);
            Helper.Helper.ClearFolder(settings.RestrictionsPath);

            // прохожу по улицам
            foreach (var street in _map.GetSortedByStreets)
            {
                if (!street.Any())
                    return;

                // вытащил значения на одной улице
                var homes = street.GetBuildingValues;
                var rules = street.GetRulesValues;
                var homeInfos = street.GetHomeInfoValues;

                // дома всегда есть, см условие выше
                var streetName = homes.FirstOrDefault().Street;


                // сохраняем дома
                SaveObj(homes, Path.Combine(settings.BuildingPath, streetName));
                // сохраняем правила доступа
                SaveObj(rules, Path.Combine(settings.RestrictionsPath, streetName));
                // сохраняем инф. о доме
                SaveObj(homeInfos, Path.Combine(settings.HomeInfoPath, streetName));
            }

            OnLoad(false);
            CanSave = false;
        }

        private void SaveObj(object toSave, string filepath)
        {
            var toWrite = JsonConvert.SerializeObject(toSave, Formatting.Indented);
            File.WriteAllText(filepath, toWrite);
        }

        private void AddStreet(List<Building> homes)
        {
            var street = new StreetViewModel(homes);
            Streets.Add(street);
        }
        #endregion
    }

    class HomeMap : List<FullHomeInfo>
    {
        public ForbiddenElement GetRule(Building home)
        {
            var find = this.FirstOrDefault(x => Equals(x.Building, home));
            return find?.ForbiddenElement;
        }

        public HomeInfo GetHomeInfo(Building home)
        {
            var find = this.FirstOrDefault(x => Equals(x.Building, home));
            return find?.HomeInfo;
        }

        public bool Remove(Building home)
        {
            var find = this.FirstOrDefault(x => Equals(x.Building, home));
            return Remove(find);
        }

        public void Add(Building home, ForbiddenElement element = null, HomeInfo info = null)
        {
            var full = new FullHomeInfo
            {
                Building = home,
                HomeInfo = info,
                ForbiddenElement = element
            };

            Add(full);
        }

        public List<HomeInfo> GetHomeInfoValues
        {
            get
            {
                var temp = new List<HomeInfo>();
                foreach (var full in this)
                {
                    var info = full.HomeInfo;

                    if (info != null)
                        temp.Add(info);
                }

                return temp;
            }
        }

        public List<ForbiddenElement> GetRulesValues
        {
            get
            {
                var temp = new List<ForbiddenElement>();
                foreach (var full in this)
                {
                    var info = full.ForbiddenElement;

                    if (info != null)
                        temp.Add(info);
                }

                return temp;
            }
        }

        public List<Building> GetBuildingValues
        {
            get
            {
                var temp = new List<Building>();
                foreach (var full in this)
                {
                    var info = full.Building;

                    if (info != null)
                        temp.Add(info);
                }

                return temp;
            }
        }

        public List<Building> GetSortedBuildingValues
        {
            get
            {
                var temp = GetBuildingValues;
                temp.Sort(new HouseNumberComparer());
                return temp;
            }
        }

        public List<HomeMap> GetSortedByStreets
        {
            get
            {
                var temp = new List<HomeMap>();

                var mapped = this.GroupBy(x => x.Building.Street);

                foreach (var streetVals in mapped)
                {
                    var mapInner = new HomeMap();
                    mapInner.AddRange(streetVals.GetEnumerator().ToIEnumerable());
                    temp.Add(mapInner);
                }

                return temp;
            }
        }

        public int DoorsCount => this.Aggregate(0, (i, info) => i + info.Building.Doors);

        /// <summary>
        /// Returns sorted!!!!
        /// </summary>
        /// <param name="home"></param>
        /// <param name="all"></param>
        /// <param name="ignoreAfterSlash"></param>
        /// <returns></returns>
        public HomeMap FindSameHouseNumber(Building home,
            BaseFindableObject.ReturnConditions all = BaseFindableObject.ReturnConditions.AllWithSlashCheck)
        {
            if (home == null)
                return new HomeMap();

            var temp = this.Where(x => x.Building.TheSameHouse(home, all));

            var homes = temp.Select(x => x.Building).ToList();
            var infos = temp.Select(x => x.HomeInfo);
            var rules = temp.Select(x => x.ForbiddenElement);

            // отсортировали по возрастанию
            homes.Sort(new HouseNumberComparer());

            var result = new HomeMap(homes, infos, rules);
            return result;
        }

        public HomeMap()
        {
            
        }

        public HomeMap(IEnumerable<Building> homes,
            IEnumerable<HomeInfo> homeInfos,
            IEnumerable<ForbiddenElement> rules)
        {
            foreach (var home in homes)
            {
                var rule = rules.FirstOrDefault(x => x.IsTheSameObject(home)) ?? new ForbiddenElement(home);
                var homeInfo = homeInfos.FirstOrDefault(x => x.IsTheSameObject(home)) ?? new HomeInfo(home);

                Add(home, rule, homeInfo);
            }
        }
    }

    class FullHomeInfo
    {
        public Building Building { get; set; }
        public ForbiddenElement ForbiddenElement { get; set; }
        public HomeInfo HomeInfo { get; set; }
    }
}
