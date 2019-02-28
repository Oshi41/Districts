using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Districts.Comparers;
using Districts.JsonClasses;
using Districts.JsonClasses.Base;
using Districts.New.Interfaces;
using Newtonsoft.Json;

namespace Districts.New.Implementation
{
    class WebWork : IWebWorker
    {
        private readonly string _autoCompliteFormatString = "http://www.dom.mos.ru/Lookups/GetSearchAutoComplete?term={0}&section=Buildings";

        public async Task<IList<string>> StreetHints(string street)
        {
            return (await DownloadHomesInner(street))
                .Take(50)
                .Select(x => x.GetStreetName())
                .Where(x => x.IndexOf(street, StringComparison.InvariantCultureIgnoreCase) >= 0)
                .Distinct()
                .ToList();
        }

        public async Task<IList<HomeInfo>> DownloadHomes(IList<string> streets)
        {
            var result = new List<Task<List<HomeInfo>>>();

            foreach (var street in streets)
            {
                result.Add(
                    Task.Run(async ()  =>
                    {
                        var taskResult = new List<HomeInfo>();
                        var innerQuery = street;
                        var comparer = new HouseNumberComparerFromString();
                        var homes = await DownloadHomesInner(innerQuery);

                        while (homes.Any())
                        {
                            taskResult.AddRange(homes
                                .Take(50)
                                .Select(x => new HomeInfo(
                                    new BaseFindableObject(street, x.GetHouseNumber()))));

                            taskResult = taskResult.OrderBy(x => x.HouseNumber, comparer).ToList();

                            
                        }


                        return taskResult;
                    }));
            }

            await Task.WhenAll(result);

            return result.SelectMany(x => x.Result).ToList();
        }

        #region private

        private async Task<IList<home_info>> DownloadHomesInner(string quarry)
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
                Tracer.Tracer.WriteError(e);
                return new List<home_info>();
            }
        }

        #endregion

        #region Nested

        private class home_info
        {
            public string url { get; set; }
            public string value { get; set; }
            public string label { get; set; }
            public string section { get; set; }

            public string GetStreetName()
            {
                var index = label.IndexOf(",");
                return label.Substring(0, index);
            }

            public string GetHouseNumber()
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
        }

        #endregion
    }

}
