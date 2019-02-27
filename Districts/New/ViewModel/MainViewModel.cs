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
