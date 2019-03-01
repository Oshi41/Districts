using System.ComponentModel;
using Districts.New.Implementation;
using Districts.New.Interfaces;
using Districts.Parser.v2;
using Districts.Singleton;
using Mvvm;

namespace Districts.New.ViewModel
{
    class MainViewModel : BindableBase
    {
        public StreetsViewModel StreetsViewModel { get; }
        public HomesViewModel HomesViewModel { get; }

        public MainViewModel()
        {
            var worker = IoC.Instance.Get<IWebWorker>();
            var parser = IoC.Instance.Get<IParser>();
            var provider = IoC.Instance.Get<IDialogProvider>();
            var messageHelper = IoC.Instance.Get<IMessageHelper>();
            var messageService = IoC.Instance.Get<IMessage>();

            StreetsViewModel = new StreetsViewModel(worker, parser, provider, messageService);
            HomesViewModel = new HomesViewModel(parser.LoadStreets(), worker, parser, messageHelper, messageService);
        }
    }
}
