using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Districts.Comparers;
using Districts.JsonClasses;
using Districts.JsonClasses.Base;
using Districts.New.Implementation.Classes;
using Districts.New.Interfaces;
using Newtonsoft.Json;

namespace Districts.New.Implementation
{
    class WebWork : IWebWorker
    {
        /// <summary>
        /// Макс сколько приходит по API
        /// </summary>
        private const int _maxAPI = 50;

        /// <summary>
        /// Самый большой номер дома
        /// </summary>
        private const int _maxHouseNumber = 1000;

        private readonly IHomeParser _homeParser;
        private readonly string _autoCompliteFormatString = "http://www.dom.mos.ru/Lookups/GetSearchAutoComplete?term={0}&section=Buildings";

        public WebWork(IHomeParser homeParser)
        {
            _homeParser = homeParser;
        }

        public async Task<IList<string>> StreetHints(string street)
        {
            return (await DownloadHomesInner(street))
                .Take(50)
                .Select(x => x.Street())
                .Where(x => x.IndexOf(street, StringComparison.InvariantCultureIgnoreCase) >= 0)
                .Distinct()
                .ToList();
        }

        public async Task<IList<iHome>> DownloadHomes(IList<string> streets)
        {
            var tasks = new List<Task<List<iHome>>>();

            foreach (var street in streets)
            {
                tasks.Add(Task.Run(async () =>
                {
                    // собрали вссе дома по улиуам
                    var streetHomes = await AggregateWholeStreet(street);

                    Tracer.Tracer.Instance.Write($"Collected {streetHomes.Count} homes on {street}");

                    // скачали инфу о всех домах
                    var parsed = streetHomes
                        .Select(async x => await _homeParser.DownloadAndParse(x))
                        .ToList();

                    // подожжём пока все скачается
                    await Task.WhenAll(parsed);

                    // вырезали те, у которых нет жилых квартир
                    var homes = parsed.Select(x => x.Result);
                    var except = homes.Where(x => !x.Doors.Any());

                    if (except.Any())
                    {
                        Tracer.Tracer.Instance.Write("No living flats here: "
                                            + string.Join(", ", except));

                        homes = homes.Except(except);
                    }

                    return homes.ToList();
                }));
            }

            await Task.WhenAll(tasks);

            return tasks.SelectMany(x => x.Result).ToList();
        }

        #region private

        private async Task<IList<home_info>> AggregateWholeStreet(string street)
        {
            var streetHomes = await DownloadHomesInner(street);

            if (streetHomes.Count > _maxAPI)
            {
                streetHomes.RemoveAt(_maxAPI);

                for (int i = 1; i < _maxHouseNumber / _maxAPI; i++)
                {
                    var quarry = street + " " + i;
                    var toAdd = (await DownloadHomesInner(quarry))
                        .Take(_maxAPI);

                    // Номера домов закончились
                    if (!toAdd.Any())
                        break;

                    streetHomes.AddRange(toAdd.Except(streetHomes));
                }
            }

            return streetHomes;
        }

        private async Task<List<home_info>> DownloadHomesInner(string quarry)
        {
            try
            {
                var uri = string.Format(_autoCompliteFormatString, HttpUtility.UrlEncode(quarry));

                using (var client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Encoding = Encoding.UTF8;
                    var response = await client.DownloadStringTaskAsync(uri);

                    return JsonConvert.DeserializeObject<List<home_info>>(response);
                }
            }
            catch (Exception e)
            {
                Tracer.Tracer.Instance.Write(e);
                return new List<home_info>();
            }
        }

        #endregion

        #region Nested

        private class home_info : IRawHome
        {
            public string url { get; set; }
            public string value { get; set; }
            public string label { get; set; }
            public string section { get; set; }

            public string UriPart()
            {
                return url;
            }

            public string Street()
            {
                var index = label.IndexOf(",");
                return label.Substring(0, index);
            }

            public string HouseNumber()
            {
                var text = label.Replace(",", "");
                var spitted = text.Split(' ');
                var result = string.Empty;

                foreach (var element in spitted)
                {
                    var i = TryParse(element);
                    // ошибка парсинга
                    if (i == null) continue;

                    if (string.IsNullOrWhiteSpace(result))
                        result += i;
                    else
                        result += "к" + i;
                }

                return result;
            }

            // Не выбрасывает исключение, понятно, что прошло неудачно, если значение нулевое
            private string TryParse(string text)
            {
                // может прийти значение типа "14/89"
                foreach (var c in text)
                    if (!char.IsDigit(c)
                        && c != '\\'
                        && c != '/')
                        return null;

                return text;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            public override bool Equals(object obj)
            {
                return obj is home_info info && string.Equals(label, info.label);
            }
        }

        #endregion
    }

}
