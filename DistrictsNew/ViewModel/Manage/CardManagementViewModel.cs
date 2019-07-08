using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Extentions;
using DistrictsLib.Interfaces.ActionArbiter;
using DistrictsLib.Interfaces.Json;
using Mvvm;

namespace DistrictsNew.ViewModel.Manage
{
    class CardManagementViewModel : BindableBase, ICardManagement
    {
        

        private readonly IActionArbiter _arbiter;

        private string _number;
        private IList<IManageRecord> _actions = new ObservableCollection<IManageRecord>();
        private DateTime? _lastTake;
        private DateTime? _lastPass;
        private string _lastOwner;

        public CardManagementViewModel(ICardManagement card, IActionArbiter arbiter)
        {
            _arbiter = arbiter;
            if (card != null)
            {
                _number = card.Number;
                _actions = new ObservableCollection<IManageRecord>(card.Actions);
            }

            Refresh();
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
    }
}
