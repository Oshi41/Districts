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
    class StreetsViewModel : BindableBase
    {
        private readonly IWebWorker _web;
        private readonly IAppSettings _settings;
        private readonly IDialogProvider _provider;

        private bool _checkedStreets;
        /// <summary>
        /// список улиц
        /// </summary>
        private IList<string> _streets;

        private string _streetsShort;

        public StreetsViewModel(IWebWorker web,
            IAppSettings settings,
            IDialogProvider provider)
        {
            _web = web;
            _settings = settings;
            _provider = provider;

            SetStreetCommand = new ActionCommand(CustomizeStreets);
            SetStreets(File.ReadAllText(_settings.StreetsPath).Split('\n'));
        }

        public ICommand SetStreetCommand { get; }

        public bool CheckedStreets
        {
            get => _checkedStreets;
            set => SetProperty(ref _checkedStreets, value);
        }

        public string StreetsShort
        {
            get => _streetsShort;
            set => SetProperty(ref _streetsShort, value);
        }

        private void CustomizeStreets()
        {
            var vm = new EditStreetViewModel(_web, _streets);

            if (_provider.ShowDialog(vm, 400))
            {
                File.WriteAllText(_settings.StreetsPath,
                    string.Join("\n", vm.Streets));

                SetStreets(vm.Streets);
            }
        }

        private void SetStreets(IList<string> newStreets)
        {
            _streets = newStreets.ToList();
            CheckedStreets = _streets.Any();
            StreetsShort = string.Join("; ", _streets);
        }
    }
}
