using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DistrictsLib.Extentions;
using DistrictsLib.Implementation.ChangesNotifier;
using DistrictsLib.Interfaces;
using DistrictsLib.Interfaces.ActionArbiter;
using DistrictsLib.Interfaces.Json;
using DistrictsLib.Legacy.JsonClasses.Manage;
using DistrictsNew.ViewModel.Base;
using DistrictsNew.ViewModel.HostDialogs;
using MaterialDesignThemes.Wpf;
using Mvvm;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel.Manage
{
    public class CardManagementViewModel : ChangesViewModel, ICardManagement
    {
        #region Firlds

        private readonly IActionArbiter _arbiter;
        private readonly Func<IList<string>> _getAllNames;

        private string _number;
        private IList<IManageRecord> _actions = new ObservableCollection<IManageRecord>();
        private DateTime? _lastTake;
        private DateTime? _lastPass;
        private string _lastOwner;

        #endregion

        public CardManagementViewModel(ICardManagement card,
            IActionArbiter arbiter,
            Func<IList<string>> getAllNames,
            IChangeNotifier changeNotifier)
            : base(changeNotifier)
        {
            _arbiter = arbiter;
            _getAllNames = getAllNames;
            DeleteActionCommand = DelegateCommand<IManageRecord>.FromAsyncHandler(OnDeleteCommand, OnCanDeleteCommand);
            AddActionCommand = DelegateCommand.FromAsyncHandler(OnAdd);

            if (card != null)
            {
                _number = card.Number;
                _actions = new ObservableCollection<IManageRecord>(card.Actions.Select(x => new ManageRecordViewModel(x)));
            }

            Refresh();
        }

        private async Task OnAdd()
        {
            // Вытаскиваю те, которые мог добавить, но ещё не сохранил
            var subjects = Actions.Select(x => x.Subject)
                // а тут вытаскиваю все имена "снаружи" этой карточки
                .Union(_getAllNames?.Invoke() ?? new List<string>())
                // все имеющие смысл значения
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var vm = new ManageRecordViewModel(new ManageRecord(), subjects)
            {
                Date = DateTime.Today
            };

            if (await Show(vm))
            {
                Actions.Add(vm);
                ChangeNotifier.SetChange(nameof(Actions));
            }
        }

        private bool OnCanDeleteCommand(IManageRecord manageRecord)
        {
            return manageRecord != null
                   && Actions.Contains(manageRecord);
        }

        private async Task OnDeleteCommand(IManageRecord manageRecord)
        {
            if (!OnCanDeleteCommand(manageRecord))
                return;

            if (!await Show(new WarningMessage(Properties.Resources.AS_DeleteRecordConfirmation, true)))
                return;

            Actions.Remove(manageRecord);
            ChangeNotifier.SetChange(nameof(Actions));
        }

        #region Implementation of ICardManagement

        public string Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        public IList<IManageRecord> Actions
        {
            get => _actions;
            set => SetProperty(ref _actions, value);
        }

        #region Overrides of BindableBase

        protected override bool SetProperty<T>(ref T storage, T value, string propertyName = null)
        {
            if (base.SetProperty(ref storage, value, propertyName))
            {
                Refresh();

                return true;
            }

            return false;
        }

        #endregion

        #endregion

        #region Proeprties
        public DateTime? LastTake
        {
            get => _lastTake;
            set => SetProperty(ref _lastTake, value);
        }

        public string LastTakeStr => LastTake.ToPrettyString();

        public DateTime? LastPass
        {
            get => _lastPass;
            set => SetProperty(ref _lastPass, value);
        }

        public string LastPassStr => LastPass.ToPrettyString();

        public string LastOwner
        {
            get => _lastOwner;
            set => SetProperty(ref _lastOwner, value);
        }

        public ICommand AddActionCommand { get; }

        public ICommand DeleteActionCommand { get; }

        public static string HostName { get; } = nameof(CardManagementViewModel);
        #endregion

        public bool Contains(string text)
        {
            if (text == null)
                return false;

            return Number?.Contains(text) == true
                   || LastTakeStr?.Contains(text) == true
                   || LastOwner?.Contains(text) == true
                   || LastPassStr?.Contains(text) == true;
        }

        private void Refresh()
        {
            _arbiter.Do(() =>
            {
                LastOwner = this.LastOwner();
                LastPass = this.LastPassed();
                LastTake = this.LastTaking();
            });
        }

        private async Task<bool> Show(BindableBase vm)
        {
            var result = await DialogHost.Show(vm, HostName);
            return Equals(true, result);
        }
    }
}
