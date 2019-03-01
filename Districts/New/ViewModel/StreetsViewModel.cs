using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Districts.New.Implementation;
using Districts.New.Implementation.Args;
using Districts.New.Interfaces;
using Districts.Parser.v2;
using Microsoft.Expression.Interactivity.Core;
using Mvvm;

namespace Districts.New.ViewModel
{
    class StreetsViewModel : BindableBase
    {
        private readonly IWebWorker _web;
        private readonly IParser _parser;
        private readonly IDialogProvider _provider;
        private readonly IMessage _message;

        private bool _checkedStreets;
        /// <summary>
        /// список улиц
        /// </summary>
        private IList<string> _streets;

        private string _streetsShort;

        public StreetsViewModel(IWebWorker web,
            IParser parser,
            IDialogProvider provider,
            IMessage message)
        {
            _web = web;
            _parser = parser;
            _provider = provider;
            _message = message;

            SetStreetCommand = new ActionCommand(CustomizeStreets);
            SetStreets(_parser.LoadStreets());
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
                _parser.SaveStreets(vm.Streets);

                SetStreets(vm.Streets);
            }
        }

        private void SetStreets(IList<string> newStreets)
        {
            _streets = newStreets.ToList();
            CheckedStreets = _streets.Any();
            StreetsShort = string.Join("; ", _streets);

            _message.OnSendMessage(new MessageArgs(newStreets.ToList()));
        }
    }
}
