using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Districts.New.Interfaces;
using Districts.Singleton;
using Districts.Singleton.Implementation;
using Microsoft.Expression.Interactivity.Core;
using Mvvm;

namespace Districts.New.ViewModel
{
    class MainViewModel : BindableBase
    {
        #region Fields
        
        private readonly IDialogProvider _provider;
        private readonly IWebWorker _web;
        private readonly IAppSettings _settings;
        private bool _checkedStreets;

        /// <summary>
        /// список улиц
        /// </summary>
        private IList<string> _streets;

        #endregion

        public MainViewModel()
        {
            SetStreetCommand = new ActionCommand(CustomizeStreets);

            _provider = IoC.Instance.Get<IDialogProvider>();
            _web = IoC.Instance.Get<IWebWorker>();
            _settings = IoC.Instance.Get<IAppSettings>();

            SetStreets(File.ReadAllText(_settings.GetStreetsFile()).Split('\n'));
        }

        public ICommand SetStreetCommand { get; }

        public bool CheckedStreets
        {
            get => _checkedStreets;
            set => SetProperty(ref _checkedStreets, value);
        }

        private void CustomizeStreets()
        {
            var vm = new EditStreetViewModel(_web, _streets);

            if (_provider.ShowDialog(vm, 400))
            {
                File.WriteAllText(_settings.GetStreetsFile(),
                    string.Join("\n", vm.Streets));

                SetStreets(vm.Streets);
            }
        }

        private void SetStreets(IList<string> newStreets)
        {
            _streets = newStreets.ToList();
            CheckedStreets = _streets.Any();
        }
    }
}
