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

        public MainViewModel()
        {
            var worker = IoC.Instance.Get<IWebWorker>();
            var parser = IoC.Instance.Get<IParser>();
            var provider = IoC.Instance.Get<IDialogProvider>();

            StreetsViewModel = new StreetsViewModel(worker, parser, provider);
        }
    }
}
