namespace DistrictsLib.Legacy.WebRequest
{
    ///// <summary>
    /////     Скачивает всю информаию об улицах
    ///// </summary>
    //public class MainDownloader
    //{
    //    private readonly IParser _parser = new Parser.Parser();
    //    public async Task DownloadInfo()
    //    {
    //        var settings = ApplicationSettings.ReadOrCreate();

    //        if (!CheckIfStreetFileExist(settings))
    //        {
    //            Tracer.Write("Улицы не были заполнены");
    //            return;
    //        }


    //        var streets = File.ReadAllLines(settings.StreetsPath);
    //        var allTasks = new List<Task<List<Building>>>();
    //        foreach (var street in streets)
    //        {
    //            var downloader = new StreetDownloader();
    //            allTasks.Add(downloader.DownloadStreet(street));
    //        }

    //        await Task.WhenAll(allTasks);

    //        foreach (var task in allTasks)
    //        {
    //            // сортирую те, у которых есть квартиры
    //            var witableObj = GetLivingBuilding(task.Result);
    //            // записываю в лог пропущенные
    //            TraceEmptyBuilding(task.Result, witableObj);

    //            if (witableObj.Any())
    //            {
    //                WriteHomes(witableObj);
    //                WriteRestrictions(witableObj);
    //                WriteCodes(witableObj);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    ///     Проверяет налчие файла улиц
    //    /// </summary>
    //    /// <param name="settings"></param>
    //    private bool CheckIfStreetFileExist(ApplicationSettings settings)
    //    {
    //        if (!File.Exists(settings.StreetsPath))
    //        {
    //            var stream = File.CreateText(settings.StreetsPath);
    //            stream.Close();
    //            return false;
    //        }

    //        return true;
    //    }

    //    private List<Building> GetLivingBuilding(List<Building> buildings)
    //    {
    //        var temp = buildings.Where(x => x.Floors > 0)
    //            .ToList();
    //        temp.Sort(new HouseNumberComparer());
    //        return temp;
    //    }

    //    private void TraceEmptyBuilding(List<Building> source, List<Building> added)
    //    {
    //        var except = source.Except(added);
    //        if (!except.Any())
    //            return;


    //        var json = JsonConvert.SerializeObject(except, Formatting.Indented);
    //        Tracer.Write("Следующие дома не попали в общий список:\n" + json);
    //    }

    //    #region Write

    //    private void WriteHomes(List<Building> buildings)
    //    {
    //        var settings = ApplicationSettings.ReadOrCreate();

    //        var writableStr = JsonConvert.SerializeObject(buildings, Formatting.Indented);
    //        var filePath = Path.Combine(settings.BuildingPath, buildings.First().Street);
    //        File.WriteAllText(filePath, writableStr);
    //    }

    //    private void WriteRestrictions(List<Building> buildings)
    //    {
    //        _parser.SaveRules(buildings
    //            .Select(x => new ForbiddenElement(x))
    //            .ToList());
    //    }

    //    private void WriteCodes(List<Building> buildings)
    //    {
    //        _parser.SaveCodes(buildings
    //            .Select(x => new HomeInfo(x))
    //            .ToList());
    //    }

    //    #endregion
    //}
}