using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DistrictsLib.Interfaces.ActionArbiter;
using DistrictsNew.Models.Interfaces;
using DistrictsNew.ViewModel.Base;
using MaterialDesignThemes.Wpf;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel.HostDialogs
{
    class GoogleConnectViewModel : ErrorViewModel
    {
        private readonly IActionArbiter _connectArbiter;
        private string _author;
        public IGoogleApiModel Model { get; }

        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        public ICommand ConnectCommand { get; }

        public GoogleConnectViewModel(IGoogleApiModel model,
            IActionArbiter connectArbiter,
            string author)
        {
            _connectArbiter = connectArbiter;
            Model = model;
            Author = author;

            var command = new CompositeCommand();

            command.RegisterCommand(new DelegateCommand(OnConnect, CanConnect));
            command.RegisterCommand(DialogHost.CloseDialogCommand);

            ConnectCommand = command;
        }

        private void OnConnect()
        {
            Model.Connect(Author);
            // Нужно для обработки валидации
            OnPropertyChanged(nameof(Author));
        }

        private bool CanConnect()
        {
            return !string.IsNullOrWhiteSpace(Author)
                   && !_connectArbiter.IsExecuting();
        }

        #region Overrides of ErrorViewModel

        protected override string ValidateError(string column)
        {
            if (string.Equals(column, nameof(Author)))
            {
                if (string.IsNullOrWhiteSpace(Author))
                {
                    return Properties.Resources.GoogleApi_AuthorHint;
                }

                if (!Model.IsConnected())
                {
                    return Properties.Resources.GoogleApi_ErrorConnect;
                }
            }

            return base.ValidateError(column);
        }

        #endregion
    }
}
