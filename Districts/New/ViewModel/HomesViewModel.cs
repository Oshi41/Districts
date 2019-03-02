using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Districts.New.Implementation;
using Districts.New.Implementation.Args;
using Districts.New.Interfaces;
using Districts.Parser.v2;
using Mvvm;
using Mvvm.Commands;

namespace Districts.New.ViewModel
{
    class HomesViewModel : BindableBase
    {
        private IList<string> _streets;

        private readonly IWebWorker _worker;
        private readonly IParser _parser;
        private readonly IMessageHelper _messageHelper;
        private readonly IMessage _messageService;

        private bool _isWorking;

        public HomesViewModel(IList<string> streets,
            IWebWorker worker,
            IParser parser, 
            IMessageHelper messageHelper,
            IMessage messageService)
        {
            _streets = streets;
            _worker = worker;
            _parser = parser;
            _messageHelper = messageHelper;
            _messageService = messageService;
            _messageService.Subscribe<MessageArgs>(OnStreetsChanged);

            Download = DelegateCommand.FromAsyncHandler(OnDownload, CanDownload);
        }

        private void OnStreetsChanged(object sender, MessageArgs e)
        {
            _streets = e.Streets.ToList();
            Download.RaiseCanExecuteChanged();
        }

        private bool CanDownload()
        {
            return _streets.Any() && !_isWorking;
        }

        private async Task OnDownload()
        {
            _isWorking = true;

            var homes = await _worker.DownloadHomes(_streets);

            _parser.SaveHomes(homes);

            _isWorking = true;
            _messageHelper.ShowDone();
        }

        public DelegateCommandBase Download { get; }
    }
}
