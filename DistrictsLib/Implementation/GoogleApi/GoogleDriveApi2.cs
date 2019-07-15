using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DistrictsLib.Interfaces.GoogleApi;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using File = Google.Apis.Drive.v3.Data.File;

namespace DistrictsLib.Implementation.GoogleApi
{
    public class GoogleDriveApi2 : IGoogleDriveApi
    {
        /// <summary>
        /// Имя файла, хранящегосся в гугл
        /// </summary>
        private const string _newFileName = "districts.zip";

        /// <summary>
        /// Сервис Google Drive
        /// </summary>
        private DriveService _driveService;

        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private readonly string _tokenPath;

        public GoogleDriveApi2(string tokenPath)
        {
            _tokenPath = tokenPath;
        }

        #region Implementation of IGoogleDriveApi

        public async Task Connect(string author)
        {
            var googleCredentials = GoogleClientSecrets
                .Load(
                    new MemoryStream(
                        Properties.Resources.credentials));
            var scope = new[] { DriveService.Scope.DriveAppdata };

            var fileStore = new FileDataStore(_tokenPath, true);

            var credential = await GoogleWebAuthorizationBroker
                .AuthorizeAsync(
                    googleCredentials.Secrets,
                    scope,
                    author,
                    _cancellation.Token,
                    fileStore);

            Trace.WriteLine($"Google API: Connected to Google Drive as {author}");

            var initializer = new BaseClientService.Initializer
            {
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name,
                HttpClientInitializer = credential
            };

            _driveService = new DriveService(initializer);
        }

        public async Task Upload(string fileSource)
        {
            var file = await GetFile();

            using (var stream = new FileStream(fileSource, FileMode.Open))
            {
                var request = _driveService
                    .Files
                    .Update(new File(), file.Id, stream, file.MimeType);

                request.Fields = "id";

                var response = await request.UploadAsync(_cancellation.Token);

                if (response.Exception != null)
                {
                    throw response.Exception;
                }
            }

            Trace.WriteLine($"Google API: Successfully loaded data");
        }

        public async Task Download(string destination)
        {
            var file = await GetFile();
            var request = _driveService.Files.Get(file.Id);

            using (var raw = new FileStream(destination, FileMode.CreateNew))
            {
                var result = await request.DownloadAsync(raw);

                if (result.Exception != null)
                {
                    throw result.Exception;
                }
            }
        }

        public void Cancel()
        {
            _cancellation.Cancel();
        }

        public bool IsConnected()
        {
            return _driveService != null;
        }

        #endregion

        private async Task<Google.Apis.Drive.v3.Data.File> GetFile()
        {
            // создал запрос
            var request = _driveService
                .Files
                .List();

            var fields = "id, name, lastModifyingUser, modifiedTime, mimeType";

            request.Spaces = "appDataFolder";
            request.Fields = $"files({fields})";

            var response = await request.ExecuteAsync(_cancellation.Token);

            var file = response.Files.FirstOrDefault();
            if (file != null)
            {
                Trace.WriteLine($"Google API: File founded - {file.Name}");
                return file;
            }


            file = new File
            {
                Name = _newFileName,
                Parents = new List<string>
                {
                    "appDataFolder"
                }
            };

            var createReq = _driveService.Files.Create(file);
            createReq.Fields = fields;
            file = await createReq.ExecuteAsync(_cancellation.Token);

            Trace.WriteLine($"Google API: Created file - {file?.Name}");

            return file;

        }
    }
}
