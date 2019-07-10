using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Interfaces;
using DistrictsLib.Interfaces.ActionArbiter;
using DistrictsNew.ViewModel.Base;
using Mvvm;

namespace DistrictsNew.ViewModel.HostDialogs
{
    public class ChooseStreetViewModel : ErrorViewModel
    {
        private readonly IStreetDownload _download;
        private readonly ITimedAction _timed;
        private readonly Func<IEnumerable<string>> _existingStreets;

        private string _street;
        private IList<string> _hints;

        public string Street
        {
            get => _street;
            set
            {
                if (SetProperty(ref _street, value))
                {
                    _timed.ScheduleAction(PerformSearch);
                }
            }
        }

        public IList<string> Hints
        {
            get => _hints;
            private set => SetProperty(ref _hints, value);
        }

        public ChooseStreetViewModel(IStreetDownload download,
            ITimedAction timed,
            Func<IEnumerable<string>> existingStreets)
        {
            _download = download;
            _timed = timed;
            _existingStreets = existingStreets;
            _hints = new ObservableCollection<string>();
        }

        private bool _isClearing;
        private async Task PerformSearch()
        {
            // Не ищем, потому что выбрали из предложенного
            if (Hints.Contains(Street))
                return;

            if (_isClearing)
            {
                _isClearing = false;
                return;
            }

            var hints = await _download.GetHints(Street);
            Hints = new ObservableCollection<string>(hints);
        }

        public void SelectCurrentStreet()
        {
            _isClearing = true;
            Hints.Remove(Street);
        }

        #region Overrides of ErrorViewModel

        protected override string ValidateError(string column)
        {
            if (string.Equals(column, nameof(Street)))
            {
                if (string.IsNullOrWhiteSpace(Street))
                {
                    return Properties.Resources.Settings_EnterStreetName;
                }

                if (_existingStreets != null
                    && _existingStreets().Contains(Street))
                {
                    return Properties.Resources.AddStreet_AlreadyExisting;
                }
            }

            return base.ValidateError(column);
        }

        #endregion
    }
}
