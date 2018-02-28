using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.Settings;
using Newtonsoft.Json;

namespace Districts.WebRequest
{
    /// <summary>
    /// Скачивает всю информаию об улицах
    /// </summary>
    public class MainDownloader
    {
        public async Task DownloadInfo()
        {
            ApplicationSettings settings = ApplicationSettings.ReadOrCreate();
            
             if (!CheckIfStreetFileExist(settings))
            {
                Tracer.Write("Улицы не были заполнены");
                return;
            }
            

            var streets = File.ReadAllLines(settings.StreetsPath);
            List<Task<List<Building>>> allTasks = new List<Task<List<Building>>>();
            foreach (var street in streets)
            {
                var downloader = new StreetDownloader();
                allTasks.Add(downloader.DownloadStreet(street));
            }

            await Task.WhenAll(allTasks);

            foreach (var task in allTasks)
            {
                // сортирую те, у которых есть квартиры
                var witableObj = GetLivingBuilding(task.Result);
                if (witableObj.Any())
                {
                    WriteHomes(witableObj);
                    WriteRestrictions(witableObj);
                    WriteCodes(witableObj);
                }
            }
        }

        #region Write

        private void WriteHomes(List<Building> buildings)
        {
            var settings = ApplicationSettings.ReadOrCreate();

            var writableStr = JsonConvert.SerializeObject(buildings, Formatting.Indented);
            var filePath = Path.Combine(settings.BuildingPath, buildings.First().Street);
            File.WriteAllText(filePath, writableStr);
        }

        private void WriteRestrictions(List<Building> buildings)
        {
            var settings = ApplicationSettings.ReadOrCreate();

            var writableObj = buildings
                .Select(x => new ForbiddenElement(x))
                .ToList();

            var writtableStr = JsonConvert.SerializeObject(writableObj, Formatting.Indented);
            var filePath = Path.Combine(settings.RestrictionsPath, writableObj.First().Street);
            File.WriteAllText(filePath, writtableStr);
        }

        private void WriteCodes(List<Building> buildings)
        {
            var settings = ApplicationSettings.ReadOrCreate();
            var writableObj = buildings
                .Select(x => new Codes(x))
                .ToList();
            var writtableStr = JsonConvert.SerializeObject(writableObj, Formatting.Indented);
            var filePath = Path.Combine(settings.CodesPath, writableObj.First().Street);
            File.WriteAllText(filePath, writtableStr);
        }

        #endregion

        /// <summary>
        /// Проверяет налчие файла улиц
        /// </summary>
        /// <param name="settings"></param>
        private bool CheckIfStreetFileExist(ApplicationSettings settings)
        {
            if (!File.Exists(settings.StreetsPath))
            {
                var stream = File.CreateText(settings.StreetsPath);
                stream.Close();
                return false;
            }
            return true;
        }

        private List<Building> GetLivingBuilding(List<Building> buildings)
        {
            var temp = buildings.Where(x => x.Floors > 0)
                .ToList();
            temp.Sort(new HouseNumberComparer());
            return temp;
        }
    }
}
