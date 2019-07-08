using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DistrictsLib.Implementation.ChangesNotifier;
using DistrictsLib.Interfaces;
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

        public MainViewModel(IParser parser, ISerializer serializer)
        {
            _parser = parser;
            _serializer = serializer;
            OpenManagementcommand = new DelegateCommand(OnOpenManage);
        }

        private void OnOpenManage()
        {
            var loaded = _parser.LoadManage();
            var vm = new ManageViewModel(loaded, new SimpleNotifier());


        }
    }
}
