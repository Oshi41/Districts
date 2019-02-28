using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Districts.New.Interfaces;
using Mvvm;
using Mvvm.Commands;

namespace Districts.New.ViewModel
{
    class HomesViewModel : BindableBase
    {
        private readonly IList<string> _streets;
        private readonly IWebWorker _worker;

        public HomesViewModel(IList<string> streets, IWebWorker worker)
        {
            _streets = streets;
            _worker = worker;
            Download = new DelegateCommand(OnDownload, CanDownload);
        }

        private bool CanDownload()
        {
            return _streets.Any();
        }

        private void OnDownload()
        {
            
        }

        public ICommand Download { get; }
    }
}
