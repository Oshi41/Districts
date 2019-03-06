using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.JsonClasses.Manage;
using Districts.Parser;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Json;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Ionic.Zip;
using Newtonsoft.Json;
using File = Google.Apis.Drive.v3.Data.File;

namespace Districts.Goggle
{
    public interface IGoggleDriveApi
    {
        /// <summary>
        /// Асинхронно подключаемся
        /// </summary>
        /// <param name="login">Имя пользователя</param>
        /// <returns></returns>
        Task ConnectAsync(string login);

        /// <summary>
        /// Отмена подключения
        /// </summary>
        void Cancel();

        /// <summary>
        /// Имя папки в GoggleDrive
        /// </summary>
        string AppFolder { get; }

        /// <summary>
        /// Загружает инф-у на Goggle
        /// </summary>
        Task Upload();

        /// <summary>
        /// Скачивает информацию с Goggle и сохраняет в локальных файлах
        /// </summary>
        Task Download();
    }

    public class GoggleDriveApi : IGoggleDriveApi
    {
        #region Fields

        private readonly string _fileName = "1.json";

        /// <summary>
        /// Данные для авторизации
        /// </summary>
        private readonly GoogleClientSecrets _secrets;

        /// <summary>
        /// Отмена подключения
        /// </summary>
        private readonly CancellationTokenSource _cancellation;

        /// <summary>
        /// Разрешения приложения
        /// </summary>
        private readonly string[] _scopes = { DriveService.Scope.DriveAppdata };

        /// <summary>
        /// Папка где хранится кэш
        /// </summary>
        private readonly string _tokensPath;

        /// <summary>
        /// Подключение к API Goggle Drive
        /// </summary>
        private DriveService _driveService;

        #endregion

        public GoggleDriveApi(byte[] credentials,
            string tokensPath)
        {
            _tokensPath = tokensPath;
            _cancellation = new CancellationTokenSource();

            // Создал секреты приложения
            _secrets = GoogleClientSecrets.Load(new MemoryStream(credentials));

            // вот такая будет папка
            AppFolder = "Districts Data";
        }

        #region Implementation of IGoggleDriveApi

        public string AppFolder { get; }

        public async Task Upload()
        {
            var file = await GetFile();

            var compressed = new ZipFile();
            compressed.AddEntry(_fileName, 
                JsonConvert.SerializeObject(new google_data()), Encoding.UTF8);

            using (var stream = new MemoryStream())
            {
                compressed.Save(stream);

                var request = _driveService.Files.Update(new File(), file.Id, stream, file.MimeType);
                request.AddParents = file.Parents.FirstOrDefault();

                var result = await request.UploadAsync(_cancellation.Token);

                if (result.Exception != null)
                {
                    throw result.Exception;
                }
            }
        }

        public async Task Download()
        {
            var file = await GetFile();
            var request = _driveService.Files.Get(file.Id);
            var compressed = new MemoryStream();
            var result = await request.DownloadAsync(compressed);

            if (result.Exception != null)
            {
                Tracer.WriteError(result.Exception);
            }

            using (var zip = new ZipArchive(compressed))
            {
                var info = NewtonsoftJsonSerializer
                    .Instance
                    .Deserialize<google_data>(
                        zip.GetEntry(_fileName).Open());

                info.Save();
            }
        }

        public async Task ConnectAsync(string login)
        {
            var credential = await GoogleWebAuthorizationBroker
                .AuthorizeAsync(
                    _secrets.Secrets,
                    _scopes,
                    login,
                    _cancellation.Token,
                    new FileDataStore(_tokensPath, true));

            var initializer = new BaseClientService.Initializer
            {
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name,
                HttpClientInitializer = credential
            };

            _driveService = new DriveService(initializer);

            Tracer.Write($"Connected to Google Drive as {login}");
        }

        public void Cancel()
        {
            _cancellation.Cancel();
        }

        #endregion

        private async Task<File> GetFile()
        {
            string mimeType = "application/vnd.google-apps.folder";
            string fields = "id, name";

            // создал запрос
            var request = _driveService
                .Files
                .List();

            // заполнил запрос
            request.PageSize = 5;
            request.Q = $"name='{AppFolder}'";
            request.Fields = $"files({fields})";

            // выполнил запрос
            var response = await request.ExecuteAsync(_cancellation.Token);

            var folder = response.Files.FirstOrDefault();

            if (folder == null)
            {
                var meta = new File
                {
                    Name = AppFolder,
                    MimeType = mimeType
                };

                var createRequest = _driveService.Files.Create(meta);
                createRequest.Fields = "id, name";
                folder = await createRequest.ExecuteAsync(_cancellation.Token);

                Tracer.Write($"Created folder - {folder.Name}");
            }

            fields += ", mimeType, parents";

            var fileReq = _driveService.Files.List();
            fileReq.Q = $"name='{_fileName}' and '{folder.Id}' in parents";
            fileReq.Fields = $"files({fields})";

            response = await fileReq.ExecuteAsync(_cancellation.Token);
            var file = response.Files.FirstOrDefault();

            if (file == null)
            {
                var meta = new File
                {
                    Name = _fileName,
                    Parents = new List<string>
                    {
                        folder.Id
                    }
                };

                var createRequest = _driveService.Files.Create(meta);
                createRequest.Fields = fields;
                file = await createRequest.ExecuteAsync(_cancellation.Token);

                Tracer.Write($"Created file - {file.Name}");
            }

            return file;
        }

        #region Nested

        class google_data
        {
            private readonly IParser _parser;

            public google_data()
            {
                _parser = new Parser.Parser();

                Cards = _parser.LoadCards();
                Codes = _parser.LoadCodes();
                Managements = _parser.LoadManage();
                Restrictions = _parser.LoadRules();
            }

            public List<Card> Cards { get; }
            public List<HomeInfo> Codes { get; }
            public List<CardManagement> Managements { get; }
            public List<ForbiddenElement> Restrictions { get; }

            public void Save()
            {
                _parser.SaveCodes(Codes);
                _parser.SaveCards(Cards);
                _parser.SaveManage(Managements);
                _parser.SaveRules(Restrictions);
            }
        }

        #endregion
    }


}
