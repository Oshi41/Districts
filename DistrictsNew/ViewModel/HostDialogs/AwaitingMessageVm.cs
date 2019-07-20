using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using Mvvm;

namespace DistrictsNew.ViewModel.HostDialogs
{
    class AwaitingMessageVm : BindableBase, IDisposable
    {
        private DialogSession _session;

        public AwaitingMessageVm(string title, string hostName)
        {
            Title = title;

            DialogHost.Show(this, hostName, OnOpen);
        }

        //public void UpdateContent(object vm)
        //{
        //    _session?.UpdateContent(vm);
        //}

        //public void ShowWarning(string text, bool isError = true)
        //{
        //    var vm = new DialogMessage(text, isError, cancelCaption:Properties.Resources.OK);
        //    UpdateContent(vm);
        //}

        private void OnOpen(object sender, DialogOpenedEventArgs e)
        {
            _session = e.Session;
        }

        public string Title { get; }

        #region IDisposable

        public void Dispose()
        {
            _session?.Close(true);
        }

        #endregion
    }
}
