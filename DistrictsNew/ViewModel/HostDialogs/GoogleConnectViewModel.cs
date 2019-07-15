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
        private string _author;
        private bool _isExecuting;
        public IGoogleApiModel Model { get; }

        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        public ICommand ConnectCommand { get; }

        public GoogleConnectViewModel(IGoogleApiModel model,
            string author)
        {
            Model = model;
            Author = author;

            var command = new CompositeCommand();

            command.RegisterCommand(DelegateCommand.FromAsyncHandler(OnConnect, CanConnect));
            command.RegisterCommand(DialogHost.CloseDialogCommand);

            ConnectCommand = command;
        }

        private async Task OnConnect()
        {
            _isExecuting = true;

            await Model.Connect(Author);

            // Нужно для обработки валидации
            OnPropertyChanged(nameof(Author));

            _isExecuting = false;
        }

        private bool CanConnect()
        {
            return !string.IsNullOrWhiteSpace(Author)
                   && !_isExecuting;
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
