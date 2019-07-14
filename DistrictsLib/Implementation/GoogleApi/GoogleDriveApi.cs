using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DistrictsLib.Interfaces;
using DistrictsLib.Interfaces.GoogleApi;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using File = Google.Apis.Drive.v3.Data.File;

namespace DistrictsLib.Implementation.GoogleApi
{
    //class GoogleDriveApi : IGoogleDriveApi
    //{
    //    private readonly string _tokenPath;
    //    private readonly IParser _parser;
    //    private readonly ISerializer _serializer;
    //    private readonly IGoogleData _googleData;

    //    /// <summary>
    //    /// Имя файла, хранящегосся в гугл
    //    /// </summary>
    //    private const string _newFileName = "districts.config";

    //    /// <summary>
    //    /// Сервис Google Drive
    //    /// </summary>
    //    private DriveService _driveService;

    //    private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

    //    public GoogleDriveApi(string tokenPath,
    //        IParser parser, 
    //        ISerializer serializer,
    //        IGoogleData googleData)
    //    {
    //        _tokenPath = tokenPath;
    //        _parser = parser;
    //        _serializer = serializer;
    //        _googleData = googleData;
    //    }

    //    #region Implementation of IGoogleDriveApi

    //    public async Task<bool> TryConnect(string author)
    //    {
    //        try
    //        {
    //            var googleCredentials = GoogleClientSecrets
    //                .Load(
    //                    new MemoryStream(
    //                        Properties.Resources.credentials));
    //            var scope = new[] { DriveService.Scope.DriveAppdata };

    //            var fileStore = new FileDataStore(_tokenPath, true);

    //            var credential = await GoogleWebAuthorizationBroker
    //                .AuthorizeAsync(
    //                    googleCredentials.Secrets,
    //                    scope,
    //                    author,
    //                    _cancellation.Token,
    //                    fileStore);

    //            Trace.WriteLine($"Google API: Connected to Google Drive as {author}");

    //            var initializer = new BaseClientService.Initializer
    //            {
    //                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name,
    //                HttpClientInitializer = credential
    //            };

    //            _driveService = new DriveService(initializer);
    //            return true;
    //        }
    //        catch (Exception e)
    //        {
    //            Trace.WriteLine(e);
    //            return false;
    //        }
    //    }

    //    public async Task<bool> TryUpload(string fileSource)
    //    {
    //        try
    //        {
    //            var data = new GoogleDataJson
    //            {
    //                Cards = _parser.LoadCards(),
    //                Codes = _parser.LoadCodes(),
    //                Managements = _parser.LoadManage(),
    //                Restrictions = _parser.LoadRules(),
    //            };

    //            var file = await GetFile();

    //            using (var stream = _googleData.GetStreamFromData(data))
    //            {
    //                var request = _driveService
    //                    .Files
    //                    .Update(new File(), file.Id, stream, file.MimeType);

    //                request.Fields = "id";

    //                var response = await request.UploadAsync(_cancellation.Token);

    //                if (response.Exception != null)
    //                {
    //                    throw response.Exception;
    //                }
    //            }

    //            Trace.WriteLine($"Google API: Successfully loaded data");
    //            return true;
    //        }
    //        catch (Exception e)
    //        {
    //            Trace.WriteLine(e);
    //            return false;
    //        }

    //    }

    //    public async Task<bool> TryDownload(string destination)
    //    {
    //        string tempPath = string.Empty;

    //        try
    //        {
    //            var file = await GetFile();
    //            var request = _driveService.Files.Get(file.Id);

    //            tempPath = Path.GetTempPath();

    //            using (var raw = new MemoryStream())
    //            {
    //                var result = await request.DownloadAsync(raw);

    //                if (result.Exception != null)
    //                {
    //                    throw result.Exception;
    //                }

    //                var data = _googleData.GetDataFromStream(
    //                    new FileStream(tempPath, 
    //                        FileMode.OpenOrCreate,
    //                        FileAccess.Write));

    //                if (data.Cards.Any())
    //                {
    //                    Trace.WriteLine($"Google API: saved cards");
    //                    _serializer.SaveCards(data.Cards);
    //                }

    //                if (data.Codes.Any())
    //                {
    //                    Trace.WriteLine($"Google API: saved codes");
    //                    _serializer.SaveCodes(data.Codes);
    //                }

    //                if (data.Managements.Any())
    //                {
    //                    Trace.WriteLine($"Google API: saved card managements");
    //                    _serializer.SaveManage(data.Managements);
    //                }

    //                if (data.Restrictions.Any())
    //                {
    //                    Trace.WriteLine($"Google API: saved visiting rules");
    //                    _serializer.SaveRules(data.Restrictions);
    //                }
    //            }

    //            return true;
    //        }
    //        catch (Exception e)
    //        {
    //            Trace.WriteLine(e);
    //            return false;

    //        }
    //        finally
    //        {
    //            if (System.IO.File.Exists(tempPath))
    //            {
    //                System.IO.File.Delete(tempPath);
    //            }
    //        }

    //    }

    //    public void Cancel()
    //    {
    //        _cancellation.Cancel();
    //    }

    //    #endregion

    //    private async Task<Google.Apis.Drive.v3.Data.File> GetFile()
    //    {
    //        // создал запрос
    //        var request = _driveService
    //            .Files
    //            .List();

    //        var fields = "id, name, lastModifyingUser, modifiedTime, mimeType";

    //        request.Spaces = "appDataFolder";
    //        request.Fields = $"files({fields})";

    //        var response = await request.ExecuteAsync(_cancellation.Token);

    //        var file = response.Files.FirstOrDefault();
    //        if (file != null)
    //            return file;


    //        file = new File
    //        {
    //            Name = _newFileName,
    //            Parents = new List<string>
    //            {
    //                "appDataFolder"
    //            }
    //        };

    //        var createReq = _driveService.Files.Create(file);
    //        createReq.Fields = fields;
    //        file = await createReq.ExecuteAsync(_cancellation.Token);

    //        Trace.WriteLine($"Created file - {file?.Name}");

    //        return file;

    //    }
    //}
}
