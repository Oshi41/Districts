using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Districts.JsonClasses;
using Newtonsoft.Json;

namespace Districts.WebRequest
{
    public class StreetDownloader
    {
        private static readonly string _uriStrFormat = "http://www.dom.mos.ru/Lookups/GetSearchAutoComplete?term={0}&section=Buildings";

        private static readonly int _apiMax = 50;
        private static readonly int _maxBuilding = 1000;

        #region Nested class

        public class HomeJson
        {
            public string url { get; set; }
            public string value { get; set; }
            public string label { get; set; }
            public string section { get; set; }

            public string GetStreetName()
            {
                if (string.IsNullOrWhiteSpace(label))
                    return string.Empty;

                var start = label.IndexOf(",", StringComparison.Ordinal);
                var result = label.Substring(0, start);
                return Compress(result);
            }

            private string Compress(string text)
            {
                var result = text;

                result = Regex.Replace(result, @"улиц(\w*)", "ул.", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"просп(\w*)", "пр-т", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"бульвар(\w*)", "б-р", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"пос[её]л(\w*)[кн](\w*)", "пос.", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"шоссе(\w*)", "ш.", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"проезд(\w*)", "пр.", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"переул(\w*)к(\w*)", "пер.", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"набережн(\w*)", "пер.", RegexOptions.IgnoreCase);

                // оставил, но это все равно не будет использоваться
                //result = Regex.Replace(result, @"квартир(\w*)", "кв.", RegexOptions.IgnoreCase);
                //result = Regex.Replace(result, @"корпус(\w*)", "кор.", RegexOptions.IgnoreCase);
                //result = Regex.Replace(result, @"дом(\w*)", "д.", RegexOptions.IgnoreCase);
                //result = Regex.Replace(result, @"строение(\w*)", "стр.", RegexOptions.IgnoreCase);

                return result;
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

            public override bool Equals(object obj)
            {
                return obj is HomeJson json
                       && string.Equals(label, json.label);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        #endregion

        #region Web Request

        public async Task<List<Building>> DownloadStreet(string street)
        {
            var result = new List<Building>();
            var value = await DownloadWholeStreet(street);

            foreach (var oneHome in value)
            {
                var homeDownloader = new HomeDownloader();
                var home = new Building(oneHome.GetStreetName(), oneHome.GetHouseNumber());
                home = await homeDownloader.ParseProperties(home, oneHome.url);
                result.Add(home);
            }

            return result;
        }

        /// <summary>
        /// Скачивает уникальный дома
        /// <para>Необходимо, т.к API возвращает 50 результатов.</para>
        /// </summary>
        /// <param name="street"></param>
        /// <returns>Делает 20 запросов на каждую улицу, мб переделать, но в скорости потеряем</returns>
        private async Task<List<HomeJson>> DownloadWholeStreet(string street)
        {
            var quarry = string.Format(_uriStrFormat, street);
            var result = await GetParsedResponse(quarry);

            if (result.Count <= _apiMax)
                return result;

            // list of all homes
            var dict = new ConcurrentDictionary<string, HomeJson>();

            result.RemoveAt(_apiMax);
            foreach (var json in result)
            {
                dict[json.label] = json;
            }

            var tasks = new List<Task>();
            for (int i = 1; i < _maxBuilding / _apiMax; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var parsed = await GetParsedResponse(string.Format(_uriStrFormat, $"{street} {i}"));

                    foreach (var json in parsed.Take(_apiMax).ToList())
                    {
                        dict.TryAdd(json.label, json);
                    }
                }));
            }

            await Task.WhenAll(tasks);

            return dict.Values.ToList();
        }

        private async Task<List<HomeJson>> GetParsedResponse(string uri)
        {
            string result;
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Encoding = Encoding.UTF8;
                result = await client.DownloadStringTaskAsync(uri);
            }

            return JsonConvert.DeserializeObject<List<HomeJson>>(result) ?? new List<HomeJson>();
        }

        #endregion
    }
}