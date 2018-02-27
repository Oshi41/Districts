using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.MVVM;

namespace Districts.ViewModel
{
    public class EditHomeInfoViewModel : ObservableObject
    {
        private Building _home;
        private readonly ForbiddenElement _rule;
        private readonly Codes _code;
        private ObservableCollection<CodesViewModel> _codes1 = new ObservableCollection<CodesViewModel>();
        private string _aggresive;
        private string _noWorried;
        private string _noVisit;

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

        public ICommand SaveCommand { get; set; }


        public EditHomeInfoViewModel(Building home, ForbiddenElement rule, Codes code)
        {
            _home = home;
            _rule = rule;
            _code = code;

            SetTabCount(home, code);

            Aggresive = CompressArray(_rule.Aggressive);
            NoVisit = CompressArray(_rule.NoVisit);
            NoWorried = CompressArray(_rule.NoWorried);

            SaveCommand = new Command(OnSave);
        }

        private void OnSave()
        {
            _rule.Aggressive = ParseSequence(Aggresive);
            _rule.NoVisit = ParseSequence(NoVisit);
            _rule.NoWorried = ParseSequence(NoWorried);

            Dictionary<int, List<string>> temp = new Dictionary<int, List<string>>();
            foreach (var code in Codes)
            {
                var val = code.ToKeyPairvalue();
                temp.Add(val.Key, val.Value);
            }

            _code.AllCodes = temp;
        }

        private void SetTabCount(Building home, Codes code)
        {
            for (int i = 1; i <= home.Entrances; i++)
            {
                if (code.AllCodes.ContainsKey(i))
                    Codes.Add(new CodesViewModel(new KeyValuePair<int, List<string>>(i, code.AllCodes[i])));
                else
                {
                    Codes.Add(new CodesViewModel(new KeyValuePair<int, List<string>>(i, new List<string>())));
                }
            }
        }

        #region Parse Sequense

        public static string CompressArray(List<int> ints)
        {
            ints.Sort();

            var values = new List<string>();

            for (var i = 0; i < ints.Count; i++)
            {
                var groupStart = ints[i];
                var groupEnd = groupStart;
                while (i < ints.Count - 1 && ints[i] - ints[i + 1] == -1)
                {
                    groupEnd = ints[i + 1];
                    i++;
                }

                if (groupEnd - groupStart < 2)
                {
                    for (var j = groupStart; j <= groupEnd; j++)
                    {
                        values.Add(j.ToString());
                    }
                }
                else
                {
                    values.Add(groupStart + "-" + groupEnd);
                }

            }

            var result = values.Aggregate(string.Empty, (s, s1) => s += s1)
                .Replace("[", "").Replace("]", "");
            return result;
        }

        private List<int> ParseSequence(string text)
        {
            List<int> result = new List<int>();

            string temp = text.Replace(" ", "");
            string[] splitted = temp.Split(',');
            for (var i = 0; i < splitted.Length; i++)
            {
                string number = splitted[i].Replace("\u0000", "");

                if (number.Contains("-"))
                {
                    result.AddRange(getRange(number));
                }
                else
                {
                    if (int.TryParse(number, out var parsed))
                    {
                        result.Add(parsed);
                    }
                }
            }

            return result;
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
                    new[] {Environment.NewLine},
                    StringSplitOptions.RemoveEmptyEntries
                );

                var result = new KeyValuePair<int, List<string>>(Number, new List<string>(lines));
                return result;
            }
        }
    }
}
