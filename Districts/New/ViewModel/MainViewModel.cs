using Districts.New.Implementation;
using Districts.New.Interfaces;
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
            var settings = IoC.Instance.Get<IAppSettings>();
            var provider = IoC.Instance.Get<IDialogProvider>();

            StreetsViewModel = new StreetsViewModel(worker, settings, provider);
        }
    }
}
