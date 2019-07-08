using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DistrictsLib.Implementation.ActionArbiter;
using DistrictsLib.Interfaces.ActionArbiter;
using DistrictsNew.ViewModel.DialogHostsVms;
using MaterialDesignThemes.Wpf;
using Mvvm;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel
{
    public class SettingsViewModel : BindableBase
    {
        private int _roomsCount;
        private ObservableCollection<string> _streets;

        public int RoomsCount
        {
            get => _roomsCount;
            set => SetProperty(ref _roomsCount, value);
        }

        public ObservableCollection<string> Streets
        {
            get => _streets;
            set => SetProperty(ref _streets,
 value);
        }

        public ICommand DeleteStreetCommand { get; }

        public ICommand AddStreetCommand { get; }

        public SettingsViewModel()
        {
            DeleteStreetCommand = new DelegateCommand<string>(OnDeleteStreet);
            AddStreetCommand = DelegateCommand.FromAsyncHandler(OnAddStreet);
        }

        private async Task OnAddStreet()
        {
            var vm = new EditStreetViewModel(Streets, new TimedAction(new ActionArbiter()));
            var result = await DialogHost.Show(vm, "Settings");

            if (Equals(true, result))
            {
                Streets.Add(vm.CurrentText);
            }
        }

        private void OnDeleteStreet(string obj)
        {
            if (Streets.Contains(obj))
                Streets.Remove(obj);
        }
    }
}
