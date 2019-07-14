using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DistrictsLib.Interfaces;
using DistrictsLib.Interfaces.ActionArbiter;
using DistrictsNew.ViewModel.Base;
using DistrictsNew.ViewModel.HostDialogs;
using MaterialDesignThemes.Wpf;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel.Dialogs
{
    public class SettingsViewModel : ChangesViewModel
    {
        #region Fields

        private string _baseFolder;
        private IList<string> _streets;
        private int _doorCount;
        private bool _googleSync;

        #endregion

        #region Props

        public string BaseFolder
        {
            get => _baseFolder;
            set => SetAndRemember(ref _baseFolder, value);
        }

        public ICommand ChooseFolder { get; }

        public IList<string> Streets
        {
            get => _streets;
            set => SetAndRemember(ref _streets, value);
        }

        public ICommand AddStreetCommand { get; }
        public ICommand RemoveStreetCommand { get; }

        public int DoorCount
        {
            get => _doorCount;
            set => SetAndRemember(ref _doorCount, value);
        }

        public static string HostName { get; } = nameof(SettingsViewModel);

        public ChooseStreetViewModel ChooseStreetVm { get; }

        public bool GoogleSync
        {
            get => _googleSync;
            set => SetAndRemember(ref _googleSync, value);
        }

        #endregion

        public SettingsViewModel(IChangeNotifier changeNotifier,
            string baseFolder,
            int doors,
            bool googleSync,
            IList<string> streets,
            ITimedAction timed,
            IStreetDownload download)
            : base(changeNotifier)
        {
            _baseFolder = baseFolder;
            _doorCount = doors;
            _googleSync = googleSync;
            _streets = new List<string>(streets);

            ChooseStreetVm = new ChooseStreetViewModel(download,
                timed,
                () => Streets);
            ChooseFolder = new DelegateCommand(OnChooseFolder);
            AddStreetCommand = DelegateCommand.FromAsyncHandler(OnAddStreet);
            RemoveStreetCommand = new DelegateCommand<string>(OnDeleteStreet, OnCanDelete);
        }

        private async Task OnAddStreet()
        {
            var result = await DialogHost.Show(ChooseStreetVm, HostName);
            if (Equals(true, result))
            {
                var copy = Streets.ToList();
                copy.Add(ChooseStreetVm.Street);
                Streets = copy;

                ChooseStreetVm.SelectCurrentStreet();
            }
        }

        private bool OnCanDelete(string arg)
        {
            return !string.IsNullOrWhiteSpace(arg)
                   && Streets.Contains(arg);
        }

        private void OnDeleteStreet(string obj)
        {
            var copy = Streets.ToList();
            copy.Remove(obj);

            Streets = new List<string>(copy);
        }

        private void OnChooseFolder()
        {
            var dlg = new FolderBrowserDialog
            {
                SelectedPath = BaseFolder
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                BaseFolder = dlg.SelectedPath;
            }
        }
    }
}
