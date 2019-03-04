using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Districts.Helper;
using Districts.MVVM;
using Districts.WebRequest;

namespace Districts.ViewModel.TabsVM
{
    class EditStreetViewModel : ObservableObject
    {
        private readonly IStreetDownloader _downloader;

        private readonly ActionArbiter _actionArbiter;

        private string _currentText;
        private bool _isOpen;
        private ObservableCollection<string> _hints = new ObservableCollection<string>();
        private string _selectedStreet;
        private ObservableCollection<string> _streets;

        public EditStreetViewModel(IList<string> streets, 
            IStreetDownloader downloader)
        {
            _downloader = downloader;
            _streets = new ObservableCollection<string>(streets);

            AddCommand = new Command(OnAdd, CanAdd);
            RemoveCommand = new Command(OnRemove, OnCanRemove);

            _actionArbiter = new ActionArbiter();
        }

        public string CurrentText
        {
            get => _currentText;
            set
            {
                if (Set(ref _currentText, value))
                {
                    RequestHints();
                }
            }
        }

        public string SelectedStreet
        {
            get => _selectedStreet;
            set => Set(ref _selectedStreet, value);
        }

        public bool IsOpen
        {
            get => _isOpen;
            set => Set(ref _isOpen, value);
        }

        public ObservableCollection<string> Hints
        {
            get => _hints;
            set => Set(ref _hints, value);
        }

        public ObservableCollection<string> Streets
        {
            get => _streets;
            set => Set(ref _streets, value);
        }

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }

        private bool OnCanRemove()
        {
            return !string.IsNullOrWhiteSpace(SelectedStreet);
        }

        private void OnRemove()
        {
            Streets.Remove(SelectedStreet);
        }

        private bool CanAdd()
        {
            return !string.IsNullOrEmpty(CurrentText)
                   && Hints.Contains(CurrentText);
        }

        private void OnAdd()
        {
            Streets.Add(CurrentText);
            Hints.Remove(CurrentText);
            IsOpen = false;
        }

        private void RequestHints()
        {
            Func<Task> func = async () =>
            {
                if (string.IsNullOrWhiteSpace(CurrentText)
                    // Выбрали то, что уже было в списке
                    || Hints.Contains(CurrentText))
                {
                    SafeExecute(() => IsOpen = false);
                    return;
                }

                var homes = await _downloader
                    .RequestHomes(CurrentText);

                var hints = homes
                    .Take(_downloader.MaxAPI)
                    .Select(x => x.GetFullStreetName())
                    .Distinct()
                    .ToList();

                SafeExecute(() =>
                {
                    Hints = new ObservableCollection<string>(hints);
                    IsOpen = true;
                });
            };

            _actionArbiter.Request(func);
        }
    }

    interface IActionArbiter
    {
        void Request(Func<Task> action);

        // Task Do(Func<Task> action);
    }

    class ActionArbiter : IActionArbiter
    {
        private readonly Timer _timer;
        private readonly TimeSpan _diff;

        private Func<Task> _action;
        private DateTime _last;
        private bool _isWorking;
        

        public ActionArbiter(int mls = 200)
        {
            _diff = TimeSpan.FromMilliseconds(mls);
            _timer = new Timer(40);
            _timer.Elapsed += OnTick;
        }

        private void OnTick(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;

            if (now - _last < _diff)
                return;

            _last = now;
            Do(_action);
        }

        #region Implementation of IActionArbiter

        public void Request(Func<Task> action)
        {
            _action = action;
            _last = DateTime.Now;
            _timer.Start();
        }

        public async Task Do(Func<Task> action)
        {
            if (_isWorking || action == null)
                return;

            try
            {
                _isWorking = true;
                await action();
            }
            catch (Exception e)
            {
                Tracer.WriteError(e);
            }
            finally
            {
                _isWorking = false;
                _timer.Stop();
            }
        }

        #endregion
    }
}
