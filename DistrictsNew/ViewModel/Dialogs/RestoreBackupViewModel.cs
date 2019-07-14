using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DistrictsLib.Interfaces;
using DistrictsLib.Interfaces.IArchiver;
using DistrictsNew.ViewModel.Base;
using DistrictsNew.ViewModel.HostDialogs;
using MaterialDesignThemes.Wpf;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel.Dialogs
{
    class RestoreBackupViewModel : ChangesViewModel
    {
        private readonly IArchiver _archiver;
        private string _backupFolder;
        public List<IZipInfo> Archives { get; } = new List<IZipInfo>();

        public string BackupFolder
        {
            get => _backupFolder;
            set
            {
                if (SetProperty(ref _backupFolder, value))
                {
                    Archives.Clear();
                    Archives.AddRange(ReadZips(value));
                }
            }
        }

        public static string HostName { get; } = nameof(RestoreBackupViewModel);

        public ICommand RestoreArchiveCommand { get; }

        public RestoreBackupViewModel(IChangeNotifier changeNotifier,
            IArchiver archiver,
            string backupFolder)
            : base(changeNotifier)
        {
            _archiver = archiver;
            BackupFolder = backupFolder;

            RestoreArchiveCommand = DelegateCommand<IZipInfo>.FromAsyncHandler(OnRestore, info => info != null);
        }

        private async Task OnRestore(IZipInfo obj)
        {
            var vm = new DialogMessage(Properties.Resources.RestoreBackup_Archivating_ReplaceWarning, 
                false, 
                Properties.Resources.AS_Confirm, 
                Properties.Resources.Cancel);

            var handler = new DialogClosingEventHandler(async (sender, args) =>
            {
                if (Equals(true, args.Parameter))
                {
                    var based = Path.GetDirectoryName(BackupFolder);

                    var errors = string.Empty;
                    foreach (var path in obj.RootedPaths)
                    {
                        try
                        {
                            var local = Path.Combine(based, path);

                            if (File.Exists(local))
                            {
                                File.Delete(local);
                                Trace.WriteLine($"Deleted file:\n{local}");
                            }

                            if (Directory.Exists(local))
                            {
                                Directory.Delete(local, true);
                                Trace.WriteLine($"Deleted folder:\n{local}");
                            }
                        }
                        catch (Exception e)
                        {
                            errors += "\n" + e;
                        }
                    }

                    if (!string.IsNullOrEmpty(errors))
                    {
                        ShowWarning(errors);
                    }

                    if (!await Task.Run(() => _archiver.TryUnzip(obj.FullPath, based)))
                    {
                        ShowWarning($"Zip placed here\n{obj.FullPath}\nShould extracted here\n{based}\n");
                        return;
                    }

                    ChangeNotifier.SetChange();
                    await DialogHost.Show(
                        new DialogMessage(Properties.Resources.RestoreBackup_ArchiveRestored,
                            false,
                            cancelCaption: Properties.Resources.OK));
                }
            });

            await DialogHost.Show(vm, HostName, handler);
        }

        private IList<IZipInfo> ReadZips(string folder)
        {
            var result = new List<IZipInfo>();

            if (!Directory.Exists(folder))
            {
                ShowWarning($"Can't find directory:\n{folder}");
                return result;
            }

            var warnings = string.Empty;
            foreach (var file in Directory.GetFiles(folder))
            {
                var info = _archiver.GetInfo(file);

                if (info == null)
                {
                    warnings += $"Can't read zip from:\n{file}";
                }
                else
                {
                    result.Add(info);
                }
            }

            if (!string.IsNullOrWhiteSpace(warnings))
            {
                ShowWarning(warnings);
            }

            return result;
        }

        private async void ShowWarning(string text, bool isError = true)
        {
            Trace.WriteLine(text);

            text = Properties.Resources.RestoreBackup_Archivating_Error + "\n\n" + text;

            var vm = new DialogMessage(text, isError, cancelCaption: Properties.Resources.OK);
            await DialogHost.Show(vm, HostName);
        }
    }
}
