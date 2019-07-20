using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Extentions;
using DistrictsLib.Interfaces.IArchiver;
using DistrictsNew.Models.Interfaces;
using DistrictsNew.ViewModel.Dialogs;
using Ionic.Zip;

namespace DistrictsNew.Models
{
    class ArchiveModel : ICreateArchiveModel, IRestoreArchiveModel
    {
        private readonly IArchiver _archiver;

        public ArchiveModel(IArchiver archiver)
        {
            _archiver = archiver;
        }

        #region Implementation of ICreateArchiveModel
        public string CreateZipPath(string destination, IReadOnlyCollection<SavingItem> zipEntries, Func<ZipFile,string> nameFunc)
        {
            if (!Directory.Exists(destination))
            {
                throw new Exception($"Can't find backups folder - {destination}");
            }

            var entries = zipEntries
                .Where(x => x.IsChecked)
                .Select(x => x.FullName)
                .ToArray();

            if (!entries.Any())
            {
                throw new Exception($"Can't find backup entries");
            }

            var path = Path.Combine(destination, $"{DateTime.Now.ToFullPrettyDateString()}.zip");

            if (!_archiver.TryToZip(path, nameFunc, entries))
            {
                throw new Exception($"Error during creating backup with entries {string.Join(", ", entries)}" +
                                    $" in backup folder {destination}" +
                                    $" and planning zip name {path}");
            }

            return path;
        }

        #endregion

        #region Implementation of IRestoreArchiveModel

        public async Task<string> RestoreWithWarnings(IZipInfo info, string destination)
        {
            var warnings = string.Empty;

            if (!await Task.Run(() => _archiver.TryUnzip(info.FullPath, destination, out warnings)))
            {
                throw new Exception($"Zip placed here\n{info.FullPath}\nShould extracted here\n{destination}\n");
            }

            return warnings;
        }

        public IList<IZipInfo> ReadZips(string folder, out string warnings)
        {
            warnings = string.Empty;

            var result = new List<IZipInfo>();

            if (!Directory.Exists(folder))
            {
                throw new Exception($"Can't find directory:\n{folder}");
            }
            
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

            return result;
        }

        #endregion
    }
}
