using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DistrictsLib.Implementation.ChangesNotifier;
using DistrictsLib.Interfaces.ActionArbiter;
using DistrictsNew.Models.Interfaces;
using DistrictsNew.ViewModel.Base;
using DistrictsNew.ViewModel.Dialogs.Base;
using MaterialDesignThemes.Wpf;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel.HostDialogs
{
    class GoogleConnectViewModel : DialogBaseViewModelBase
    {
        private string _author;

        private bool _isConnected;
        private bool _isExecuting;
        public IGoogleApiModel Model { get; }

        public string Author
        {
            get => _author;
            set => SetProperty(ref _author, value);
        }

        // Условие видимости для кнопок
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        // Условие видимости прогрессбара в кнопке
        public bool IsExecuting
        {
            get => _isExecuting;
            private set => SetProperty(ref _isExecuting, value);
        }

        public ICommand ConnectCommand { get; }

        public ICommand CancelCommand { get; }


        public GoogleConnectViewModel(IGoogleApiModel model,
            string author)
            : base(null)
        {
            Model = model;
            Author = author;

            ConnectCommand = DelegateCommand.FromAsyncHandler(OnConnect, CanConnect);
            CancelCommand = new DelegateCommand(() => Model.Cancel(), () => IsExecuting);
        }

        private async Task OnConnect()
        {
            try
            {
                IsExecuting = true;

                // пытаемся подключиться
                await Model.Connect(Author);
            }
            catch (Exception e)
            {
                // Трассируем ошибку и показываем её
                Trace.WriteLine($"{nameof(GoogleConnectViewModel)}.{nameof(OnConnect)}: {e}");
                ShowInfo($"{Properties.Resources.GoogleConnect_Error}\n\n{e}");
            }
            finally
            {
                IsExecuting = false;
                IsConnected = Model.IsConnected();

                // Нужно для обработки валидации
                OnPropertyChanged(nameof(Author));

                // Сохраняю логин для гугла
                if (IsConnected)
                {
                    var settings = Properties.Settings.Default;
                    settings.Login = Author;
                    settings.Save();
                }
            }
        }

        private bool CanConnect()
        {
            return !string.IsNullOrWhiteSpace(Author)
                   && !IsExecuting;
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
