using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DistrictsLib.Interfaces.ActionArbiter;
using Mvvm;

namespace DistrictsNew.ViewModel.DialogHostsVms
{
    public class EditStreetViewModel : ExtendedBindable
    {
        private readonly List<string> _exclude;
        private readonly ITimedAction _action;

        private string _currentText;
        private ObservableCollection<string> _hints;
        private bool _isOpen;

        public string CurrentText
        {
            get => _currentText;
            set
            {
                if (SetProperty(ref _currentText, value))
                {

                }
            }
        }

        public ObservableCollection<string> Hints
        {
            get => _hints;
            set => SetProperty(ref _hints, value);
        }

        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }

        public EditStreetViewModel(IList<string> exclude, ITimedAction action)
        {
            _action = action;
            _exclude = exclude.ToList();
        }
    }
}
