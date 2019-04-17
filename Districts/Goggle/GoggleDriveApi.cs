using System;
using System.CodeDom;
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
using Districts.Settings;
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
    public interface IGoogleDriveApi
    {
        /// <summary>
        /// Получает файл и статус был ли он создан
        /// </summary>
        /// <returns></returns>
        Task Connect(string name);

        /// <summary>
        /// Загружаем сохранённую информацию
        /// </summary>
        /// <returns></returns>
        Task Upload();

        /// <summary>
        /// Скачивает и обновляет локальную информацию 
        /// </summary>
        /// <returns></returns>
        Task DownloadAndUpdate();
    }

    public class GoogleDriveApi : IGoogleDriveApi
    {
        #region Fields

        private readonly IParser _parser;

        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

        /// <summary>
        /// Имя файла для настроек
        /// </summary>
        private readonly string _fileName = "districts.config";

        /// <summary>
        /// Сервис Google Drive
        /// </summary>
        private DriveService _driveService;


        #endregion

        public GoogleDriveApi(IParser parser)
        {
            _parser = parser;
        }

        #region Implementation of IGoogleDriveApi

        public async Task Connect(string name)
        {
            var googleCredentials = GoogleClientSecrets
                .Load(
                    new MemoryStream(
                        Properties.Resources.credentials));
            var scope = new[] {DriveService.Scope.DriveAppdata};

            var fileStore = new FileDataStore(ApplicationSettings.ReadOrCreate().TokensPath, true);

            var credential = await GoogleWebAuthorizationBroker
                .AuthorizeAsync(
                    googleCredentials.Secrets,
                    scope,
                    name,
                    _cancellation.Token,
                    fileStore);

            Tracer.Write($"Connected to Google Drive as {name}");


            var initializer = new BaseClientService.Initializer
            {
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name,
                HttpClientInitializer = credential
            };

            _driveService = new DriveService(initializer);
        }

        public async Task Upload()
        {
            var data = new google_data(_parser);
            Stream stream = new MemoryStream(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(data)));

            // stream = Compress(stream);

            var existingFile = await GetFile();

            var request = _driveService
                .Files
                .Update(new File(), existingFile.Id, stream, existingFile.MimeType);
            request.Fields = "id";

            var response = await request.UploadAsync(_cancellation.Token);

            if (response.Exception != null)
            {
                throw response.Exception;
            }
        }

        public async Task DownloadAndUpdate()
        {
            var file = await GetFile();
            var request = _driveService.Files.Get(file.Id);

            var temp = Path.GetTempFileName();

            using (var raw = new MemoryStream())
            {
                var result = await request.DownloadAsync(raw);

                if (result.Exception != null)
                {
                    throw result.Exception;
                }

                using (var fileStream = new FileStream(temp, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    raw.WriteTo(fileStream);
                    fileStream.Close();
                }

                var data = JsonConvert.DeserializeObject<google_data>(System.IO.File.ReadAllText(temp));
                data?.Save(_parser);
            }

            if (System.IO.File.Exists(temp))
                System.IO.File.Delete(temp);
        }


        #endregion

        private async Task<File> GetFile()
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
                return file;


            file = new File
            {
                Name = _fileName,
                Parents = new List<string>
                {
                    "appDataFolder"
                }
            };

            var createReq = _driveService.Files.Create(file);
            createReq.Fields = fields;
            file = await createReq.ExecuteAsync(_cancellation.Token);

            Tracer.Write($"Created file - {file?.Name}");

            return file;
            
        }

        private Stream Compress(Stream origin)
        {
            var result = new MemoryStream();

            var compressed = new ZipFile();
            compressed.AddEntry(_fileName, origin);
            compressed.Save(result);

            origin.Close();

            return result;
        }

        private Stream Decompress(Stream origin)
        {
            using (var zip = new ZipArchive(origin))
            {
                var result = new MemoryStream();
                zip.GetEntry(_fileName)?.Open().CopyTo(result);
                return result;
            }
        }

        #region Nested

        class google_data
        {
            [JsonConstructor]
            public google_data(List<Card> cards, 
                List<HomeInfo> codes,
                List<CardManagement> managements,
                List<ForbiddenElement> restrictions)
            {
                Cards = cards;
                Codes = codes;
                Managements = managements;
                Restrictions = restrictions;
            }

            public google_data(IParser parser)
            {
                Cards = parser.LoadCards();
                Codes = parser.LoadCodes();
                Managements = parser.LoadManage();
                Restrictions = parser.LoadRules();
            }

            public List<Card> Cards { get; private set; }
            public List<HomeInfo> Codes { get; private set; }
            public List<CardManagement> Managements { get; private set; }
            public List<ForbiddenElement> Restrictions { get; private set; }

            public void Save(IParser parser)
            {
                parser.SaveCodes(Codes);
                parser.SaveCards(Cards);
                parser.SaveManage(Managements);
                parser.SaveRules(Restrictions);
            }
        }

        #endregion
        
    }
}
