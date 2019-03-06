using System;
using System.Collections.Generic;
using System.IO;
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
using Google.Apis.Services;
using Google.Apis.Util.Store;
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
            using (var memory =
                new MemoryStream(
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(
                            new google_data(
                                new Parser.Parser())))))
            {
                var folder = await GetFolder();

                var meta = new File
                {
                    Name = _fileName,
                    Parents = new List<string>
                    {
                        folder.Id
                    }
                };

                var req = _driveService.Files.Create(
                    meta, memory, "text/x-json");
                req.Fields = "id";

                var resp = await req.UploadAsync(_cancellation.Token);

                if (resp.Exception != null)
                {
                    Tracer.WriteError(resp.Exception);
                }
            }
        }

        public async Task Download()
        {
            var folder = await GetFolder();
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

            await CheckFolder();
        }

        public void Cancel()
        {
            _cancellation.Cancel();
        }

        #endregion

        /// <summary>
        /// Создает папку, если это необходимо
        /// </summary>
        /// <returns></returns>
        private async Task CheckFolder()
        {// если нашли папку, выходим
            if (await GetFolder() != null)
                return;

            var meta = new File
            {
                Name = AppFolder,
                MimeType = "application/vnd.google-apps.folder"
            };

            var createRequest = _driveService.Files.Create(meta);
            createRequest.Fields = "id";
            var file = await createRequest.ExecuteAsync(_cancellation.Token);

            Tracer.Write($"Created folder - {file.Id}");
        }

        private async Task<File> GetFolder()
        {
            // создал запрос
            var request = _driveService
                .Files
                .List();

            // заполнил запрос
            request.PageSize = 5;
            request.Q = $"name='{AppFolder}'";
            request.Fields = "files(name, id)";

            // выполнил запрос
            var response = await request.ExecuteAsync(_cancellation.Token);

            return response.Files.FirstOrDefault();
        }

        #region Nested

        class google_data
        {
            private readonly IParser _parser;

            public google_data(IParser parser)
            {
                _parser = parser;

                Cards = _parser.LoadCards();
                Codes = _parser.LoadCodes();
                Managements = _parser.LoadManage();
                Restrictions = _parser.LoadRules();
            }

            public List<Card> Cards { get; }
            public List<HomeInfo> Codes { get; }
            public List<CardManagement> Managements { get; }
            public List<ForbiddenElement> Restrictions { get; }
        }

        #endregion
    }


}
