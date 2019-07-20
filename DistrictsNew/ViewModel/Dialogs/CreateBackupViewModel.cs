using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DistrictsLib.Extentions;
using DistrictsLib.Interfaces;
using DistrictsLib.Interfaces.ActionArbiter;
using DistrictsLib.Interfaces.IArchiver;
using DistrictsNew.Models.Interfaces;
using DistrictsNew.ViewModel.Base;
using DistrictsNew.ViewModel.HostDialogs;
using MaterialDesignThemes.Wpf;
using Mvvm;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel.Dialogs
{
    public class CreateBackupViewModel : ChangesViewModel
    {
        private readonly ICreateArchiveModel _model;
        private string _backupFolder;
        private readonly IActionArbiter _arbiter;
        private bool? _isAllChecked;

        public static string HostName { get; } = nameof(CreateBackupViewModel);
        public List<SavingItem> Items { get; } = new List<SavingItem>();

        public string BackupFolder
        {
            get => _backupFolder;
            set => SetProperty(ref _backupFolder, value);
        }

        public bool? IsAllChecked
        {
            get => _isAllChecked;
            set
            {
                if (SetProperty(ref _isAllChecked, value)
                    && IsAllChecked.HasValue)
                {
                    _arbiter.Do(() =>
                    {
                        Items.ForEach(x => x.IsChecked = IsAllChecked.Value);
                    });
                }
            }
        }

        public ICommand CreateZipCommand { get; }
        public ICommand ChooseFolderCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeNotifier"></param>
        /// <param name="model"></param>
        /// <param name="baseFolder"></param>
        /// <param name="backupFolder"></param>
        /// <param name="arbiter"></param>
        /// <param name="selected">Полный путь к файлу/папке, которые нужно выделить</param>
        public CreateBackupViewModel(IChangeNotifier changeNotifier,
            ICreateArchiveModel model,
            string baseFolder,
            string backupFolder,
            IActionArbiter arbiter,
            // Full path
            IEnumerable<string> selected)
            : base(changeNotifier)
        {
            _model = model;
            _backupFolder = backupFolder;
            _arbiter = arbiter;

            if (!Directory.Exists(baseFolder))
                return;

            var contentDirs = Directory.GetDirectories(baseFolder);
            Items.AddRange(contentDirs.Select(x => new SavingItem(true, x)));

            var files = Directory.GetFiles(baseFolder);
            Items.AddRange(files.Select(x => new SavingItem(false, x)));

            foreach (var item in Items)
            {
                item.IsChecked = selected.Contains(item.FullName);
            }

            CreateZipCommand = new DelegateCommand(CreateZip, OnCanCreateZip);
            ChooseFolderCommand = new DelegateCommand(OnChooseFolder);

            Items.ForEach(x => x.PropertyChanged += NotifyChanges);
        }

        #region Command handlers

        private async void CreateZip()
        {
            try
            {
                string file;

                using (new AwaitingMessageVm(Properties.Resources.AS_Awaiting, HostName))
                {
                    file = _model.CreateZipPath(BackupFolder, Items, null);
                    ChangeNotifier.SetChange();
                }

                await ShowWarning(string.Format(Properties.Resources.CreateArchive_StrFormat_ArchiveCreated, 
                    file), 
                    false);
            }
            catch (Exception e)
            {
                await ShowWarning(e.ToString());
            }
        }

        private bool OnCanCreateZip()
        {
            return Items.Any(x => x.IsChecked);
        }

        private void OnChooseFolder()
        {
            var dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                BackupFolder = dlg.SelectedPath;
            }
        }

        #endregion

        private void NotifyChanges(object sender, PropertyChangedEventArgs e)
        {
            _arbiter.Do(() =>
            {
                var allTrue = Items.All(x => x.IsChecked);
                var allFalse = Items.All(x => !x.IsChecked);

                if (!allFalse && !allTrue)
                {
                    IsAllChecked = null;
                }
                else
                {
                    IsAllChecked = allTrue;
                }
            });
        }

        private async Task ShowWarning(string text, bool isError = true)
        {
            Trace.WriteLine(text);

            var vm = new DialogMessage(text, isError, cancelCaption: Properties.Resources.OK);
            await DialogHost.Show(vm, HostName);
        }

        #region Overrides of ErrorViewModel

        protected override string ValidateError(string column)
        {
            if (string.Equals(column, nameof(SavingItem.IsChecked)))
            {
                if (!Items.Any(x => x.IsChecked))
                {
                    return Properties.Resources.CreateBackup_ChooseFiles;
                }
            }

            return base.ValidateError(column);
        }

        #endregion
    }

    public class SavingItem : BindableBase
    {
        private bool _isChecked;

        public SavingItem(bool isFolder, string fullName, bool isChecked = true)
        {
            _isChecked = isChecked;
            IsFolder = isFolder;

            FullName = fullName;

            Name = Path.GetFileName(FullName);
        }

        public bool IsFolder { get; }

        public string Name { get; }
        public string FullName { get; }

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
    }
}
