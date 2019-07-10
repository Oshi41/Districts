using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DistrictsLib.Implementation.ActionArbiter;
using DistrictsLib.Implementation.ChangesNotifier;
using DistrictsLib.Interfaces;
using DistrictsNew.Extensions;
using DistrictsNew.ViewModel.Dialogs;
using Mvvm;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel
{
    class MainViewModel : BindableBase
    {
        private readonly IParser _parser;
        private readonly ISerializer _serializer;

        public ICommand OpenManagementcommand { get; }

        public ICommand OpenManageFolder { get; }

        public MainViewModel(IParser parser, ISerializer serializer)
        {
            _parser = parser;
            _serializer = serializer;
            OpenManagementcommand = new DelegateCommand(OnOpenManage);

            OpenManageFolder = new DelegateCommand(() => OpenLink(Properties.Settings.Default.ManageFolder));
        }

        private void OnOpenManage()
        {
            var loaded = _parser.LoadManage();
            var vm = new ManageViewModel(loaded,
                new SimpleNotifier(),
                new TimedAction(new SafeThreadActionArbiter(new ActionArbiter())));

            if (vm.ShowDialog(Properties.Resources.Manage_Title, 600, 600 / 1.5) == true
                && vm.IsChanged)
            {
                _serializer.SaveManage(vm.Origin);
            }
        }

        private void OpenLink(string uri)
        {
            Process.Start(uri);
        }
    }
}
