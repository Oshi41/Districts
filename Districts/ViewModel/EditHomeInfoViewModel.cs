using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Districts.Comparers;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.MVVM;

namespace Districts.ViewModel
{
    public class EditHomeInfoViewModel : ObservableObject
    {
        #region Fields

        #region To save check

        private readonly CountingTypes? _sourceAlgorythm;
        private readonly int _sourceNumber;

        #endregion

        private ObservableCollection<CodesViewModel> _codes1 = new ObservableCollection<CodesViewModel>();
        private string _aggresive;
        private string _noWorried;
        private string _noVisit;
        private string _comments;
        private CountingTypes? _countingAlgorythm;
        private int _begin;
        private bool _inheritSettings;

        #endregion

        #region Props

        public Building Home { get; private set; }
        public ForbiddenElement ForbiddenElement { get; private set; }
        public HomeInfo HomeInfo { get; private set; }

        public ObservableCollection<CodesViewModel> Codes
        {
            get { return _codes1; }
            set
            {
                if (Equals(value, _codes1)) return;
                _codes1 = value;
                OnPropertyChanged();
            }
        }

        public string Aggresive
        {
            get { return _aggresive; }
            set
            {
                if (value == _aggresive) return;
                _aggresive = value;
                OnPropertyChanged();
            }
        }

        public string NoWorried
        {
            get { return _noWorried; }
            set
            {
                if (Equals(value, _noWorried)) return;
                _noWorried = value;
                OnPropertyChanged();
            }
        }

        public string NoVisit
        {
            get { return _noVisit; }
            set
            {
                if (value == _noVisit) return;
                _noVisit = value;
                OnPropertyChanged();
            }
        }

        public string Comments
        {
            get { return _comments; }
            set
            {
                if (value == _comments) return;
                _comments = value;
                OnPropertyChanged();
            }
        }

        public CountingTypes? CountingAlgorythm
        {
            get { return _countingAlgorythm; }
            set
            {
                if (value == _countingAlgorythm) return;
                _countingAlgorythm = value;
                OnPropertyChanged();
            }
        }

        public int Begin
        {
            get { return _begin; }
            set
            {
                if (value == _begin) return;
                _begin = value;
                OnPropertyChanged();
            }
        }

        public bool InheritSettings
        {
            get { return _inheritSettings; }
            set
            {
                if (value == _inheritSettings) return;
                _inheritSettings = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; set; }


        public bool CountingChanged => _sourceAlgorythm != CountingAlgorythm
                                       || _sourceNumber != Begin
                                       || InheritSettings;

        public int CustomChangedStep => Begin - _sourceNumber;

        #endregion

        public EditHomeInfoViewModel(Building home, ForbiddenElement rule, HomeInfo homeInfo)
        {
            Home = home;
            ForbiddenElement = rule;
            HomeInfo = homeInfo;

            SetTabCount(home, homeInfo);

            Aggresive = CompressArray(ForbiddenElement.Aggressive);
            NoVisit = CompressArray(ForbiddenElement.NoVisit);
            NoWorried = CompressArray(ForbiddenElement.NoWorried);
            Comments = ForbiddenElement.Comments;
            Begin = homeInfo.Begin;

            SaveCommand = new Command(OnSave);

            var number = new HouseNumber(home.HouseNumber);
            if (number.Housing > 1)
            {
                CountingAlgorythm = Begin == 1
                    ? CountingTypes.Regular
                    : CountingTypes.Custom;

            }

            // Для сравнения
            _sourceAlgorythm = CountingAlgorythm;
            _sourceNumber = Begin;
        }

        #region Methods

        private void OnSave()
        {
            ForbiddenElement.Aggressive = ParseSequence(Aggresive);
            ForbiddenElement.NoVisit = ParseSequence(NoVisit);
            ForbiddenElement.NoWorried = ParseSequence(NoWorried);
            ForbiddenElement.Comments = Comments.RemoveEmptyLines();

            Dictionary<int, List<string>> temp = new Dictionary<int, List<string>>();
            foreach (var code in Codes)
            {
                var val = code.ToKeyPairvalue();
                temp.Add(val.Key, val.Value);
            }

            HomeInfo.AllCodes = temp;
            if (CountingAlgorythm == CountingTypes.Custom)
                HomeInfo.Begin = Begin;
        }

        private void SetTabCount(Building home, HomeInfo homeInfo)
        {
            for (int i = 1; i <= home.Entrances; i++)
            {
                if (homeInfo.AllCodes.ContainsKey(i))
                    Codes.Add(new CodesViewModel(new KeyValuePair<int, List<string>>(i, homeInfo.AllCodes[i])));
                else
                {
                    Codes.Add(new CodesViewModel(new KeyValuePair<int, List<string>>(i, new List<string>())));
                }
            }
        }



        #region Parse Sequense

        public static string CompressArray(List<int> ints)
        {
            if (ints.IsNullOrEmpty())
                return "";

            // сортирую по возрастанию
            ints.Sort();
            var values = new List<string>();

            for (var i = 0; i < ints.Count; i++)
            {
                // индекс группы значений
                var groupStart = ints[i];
                var groupEnd = groupStart;
                while (i < ints.Count - 1 && ints[i] - ints[i + 1] == -1)
                {
                    groupEnd = ints[i + 1];
                    i++;
                }
                // если группа меньше двух, добавляю через запятую
                if (groupEnd - groupStart < 2)
                {
                    for (var j = groupStart; j <= groupEnd; j++)
                    {
                        values.Add(j.ToString());
                    }
                }
                else
                {   // добавляю список
                    values.Add(groupStart + "-" + groupEnd);
                }

            }

            var result = values.Aggregate(string.Empty, (s, s1) => s += s1 + ",");
            return result.Substring(0, result.Length - 1);
        }

        private List<int> ParseSequence(string text)
        {
            HashSet<int> result = new HashSet<int>();
            if (string.IsNullOrWhiteSpace(text))
                return result.ToList();

            string temp = text.Replace(" ", "");
            string[] splitted = temp.Split(',');
            for (var i = 0; i < splitted.Length; i++)
            {
                string number = splitted[i].Replace("\u0000", "");

                if (number.Contains("-"))
                {
                    result.UnionWith(getRange(number));
                }
                else
                {
                    if (int.TryParse(number, out var parsed))
                    {
                        result.Add(parsed);
                    }
                }
            }

            //Пишу только уникальные хаты по возрастанию
            result.OrderBy(x => x);
            return result.ToList();
        }

        private List<int> getRange(string element)
        {
            element = element.Trim();
            string[] splitted = element.Split('-');

            int.TryParse(splitted[0], out var first);
            int.TryParse(splitted[splitted.Length - 1], out var second);

            if (second < first || second < 1)
            {
                return new List<int>();
            }

            //Включаем значение последнего, поэтому плюс один
            return Enumerable.Range(first, second - first + 1).ToList();
        }

        #endregion

        #endregion

        #region Nested

        public class CodesViewModel : ObservableObject
        {
            private string _codes;
            private int _number;

            public string HomeCodes
            {
                get { return _codes; }
                set
                {
                    if (Equals(value, _codes)) return;
                    _codes = value;
                    OnPropertyChanged();
                }
            }

            public int Number
            {
                get { return _number; }
                set
                {
                    if (value == _number) return;
                    _number = value;
                    OnPropertyChanged();
                }
            }


            public CodesViewModel(KeyValuePair<int, List<string>> codes)
            {
                HomeCodes = codes.Value.Aggregate(string.Empty, (x, y) => x += Environment.NewLine + y)
                    .RemoveEmptyLines();
                Number = codes.Key;
            }

            public KeyValuePair<int, List<string>> ToKeyPairvalue()
            {
                var lines = HomeCodes.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries
                );

                var result = new KeyValuePair<int, List<string>>(Number, new List<string>(lines));
                return result;
            }
        }

        #endregion
    }

    public enum CountingTypes
    {
        [Description("Обычная нумерация")]
        Regular,
        [Description("Продолжать по корпусам")]
        AutomaticIncrement,
        [Description("Настроить")]
        Custom
    }
}
