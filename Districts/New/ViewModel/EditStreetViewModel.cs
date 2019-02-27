using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Districts.New.Interfaces;
using Microsoft.Expression.Interactivity.Core;
using Mvvm;
using Mvvm.Commands;

namespace Districts.New.ViewModel
{
    class EditStreetViewModel : BindableBase
    {
        private readonly IWebWorker _webWorker;

        private string _inputText;
        private ICommand _addPath;
        private ICommand _deletePath;
        private string _currentPath;
        private ObservableCollection<string> _hints;

        public EditStreetViewModel(IWebWorker webWorker)
        {
            _webWorker = webWorker;
            Streets = new ObservableCollection<string>();
            AddPath = new DelegateCommand(OnAddStreet, OnCanAddStreet);
            DeletePath = new DelegateCommand(OnDeletePath, OnCanDeletePath);
        }

        private bool OnCanDeletePath()
        {
            return CurrentPath != null;
        }

        private void OnDeletePath()
        {
            Streets.Remove(CurrentPath);
        }

        public string InputText
        {
            get => _inputText;
            set
            {
                if (SetProperty(ref _inputText, value))
                {
                    UpdateHints();
                }
            }
        }

        public ICommand AddPath
        {
            get => _addPath;
            set => SetProperty(ref _addPath, value);
        }

        public ICommand DeletePath
        {
            get => _deletePath;
            set => SetProperty(ref _deletePath, value);
        }

        public string CurrentPath
        {
            get => _currentPath;
            set => SetProperty(ref _currentPath, value);
        }

        public ObservableCollection<string> Streets { get; }

        public ObservableCollection<string> Hints
        {
            get => _hints;
            set => SetProperty(ref _hints, value);
        }


        private async void UpdateHints()
        {
            if (string.IsNullOrWhiteSpace(InputText))
            {
                Hints.Clear();
                return;
            }

            var result = await _webWorker.Hints(InputText);
            Hints = new ObservableCollection<string>(result);
        }

        private bool OnCanAddStreet()
        {
            return !string.IsNullOrWhiteSpace(InputText) && !Streets.Contains(InputText);
        }

        private void OnAddStreet()
        {
            Streets.Add(InputText);
            InputText = string.Empty;
        }
    }
}
