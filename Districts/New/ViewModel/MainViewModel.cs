using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Districts.New.Implementation;
using Districts.New.Interfaces;
using Districts.Parser.v2;
using Districts.Singleton;
using Mvvm;
using Mvvm.Commands;

namespace Districts.New.ViewModel
{
    class MainViewModel : BindableBase
    {
        private readonly IDialogProvider _provider;
        private readonly IParser _parser;

        public StreetsViewModel StreetsViewModel { get; }
        public HomesViewModel HomesViewModel { get; }
        
        public ICommand OpenEditHomes { get; }

        public MainViewModel()
        {
            var worker = IoC.Instance.Get<IWebWorker>();
            _parser = IoC.Instance.Get<IParser>();
            _provider = IoC.Instance.Get<IDialogProvider>();
            var messageHelper = IoC.Instance.Get<IMessageHelper>();
            var messageService = IoC.Instance.Get<IMessage>();

            StreetsViewModel = new StreetsViewModel(worker, _parser, _provider, messageService);
            HomesViewModel = new HomesViewModel(_parser.LoadStreets(), worker, _parser, messageHelper, messageService);
            //EditHomesViewModel = new EditHomesViewModel(parser);

            OpenEditHomes = new DelegateCommand(OnEditHomes);
        }

        private void OnEditHomes()
        {
            var vm = new EditHomesViewModel(_parser, _provider);

            if (_provider.ShowDialog(vm, 400))
            {
                _parser.SaveHomes(vm.StreetHomes.SelectMany(x => x.Homes).ToList());
            }
        }
    }
}
