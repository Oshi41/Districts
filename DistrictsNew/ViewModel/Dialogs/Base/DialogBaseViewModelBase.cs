using System.Diagnostics;
using DistrictsLib.Interfaces;
using DistrictsNew.ViewModel.Base;
using DistrictsNew.ViewModel.HostDialogs;
using MaterialDesignThemes.Wpf;

namespace DistrictsNew.ViewModel.Dialogs.Base
{
    class DialogBaseViewModelBase : ChangesViewModel
    {
        public static string HostName { get; protected set; }

        public DialogBaseViewModelBase(IChangeNotifier changeNotifier) 
            : base(changeNotifier)
        {
        }

        protected async void ShowInfo(string text, bool isError = true)
        {
            Trace.WriteLine(text);

            var vm = new DialogMessage(text, isError, cancelCaption: Properties.Resources.OK);
            await DialogHost.Show(vm, HostName);
        }
    }
}
