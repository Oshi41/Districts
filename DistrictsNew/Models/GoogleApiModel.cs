using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Extentions;
using DistrictsLib.Interfaces.GoogleApi;
using DistrictsLib.Interfaces.IArchiver;
using DistrictsNew.Models.Interfaces;
using DistrictsNew.ViewModel.Dialogs;

namespace DistrictsNew.Models
{
    class GoogleApiModel : IGoogleApiModel
    {
        private readonly ICreateArchiveModel _createModel;
        private readonly IRestoreArchiveModel _restoreModel;
        private readonly IGoogleDriveApi _googleApi;

        public GoogleApiModel(ICreateArchiveModel createModel,
            IRestoreArchiveModel restoreModel,
            IGoogleDriveApi googleApi)
        {
            _createModel = createModel;
            _restoreModel = restoreModel;
            _googleApi = googleApi;
        }

        #region Implementation of IGoogleApiModel

        public async Task ArchiveAndUpload(IReadOnlyCollection<SavingItem> entries, string baseFolder)
        {
            var zip = _createModel.CreateZipPath(baseFolder, 
                entries,
                file => $"Created for Google {DateTime.Now.ToFullPrettyDateString()}");

            await _googleApi.Upload(zip);
        }

        public async Task DownloadAndReplace(string backupFolder)
        {
            var path = Path.Combine(Path.GetDirectoryName(backupFolder), $"google_{DateTime.Now.ToFullPrettyDateString()}.zip");
            await _googleApi.Download(path);

            var latestZip = _restoreModel
                .ReadZips(backupFolder, out var warnings)
                .FirstOrDefault(x => string.Equals(path, x.FullPath));

            await _restoreModel.RestoreWithWarnings(latestZip, Path.GetDirectoryName(backupFolder));
        }

        public async Task Connect(string autor)
        {
            await _googleApi.Connect(autor);
        }

        public bool IsConnected()
        {
            return _googleApi.IsConnected();
        }

        #endregion
    }
}
