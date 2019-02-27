using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Districts.Helper;
using Districts.New.Interfaces;
using Newtonsoft.Json;

namespace Districts.Singleton.Implementation
{
    class WebWork : IWebWorker
    {
        private readonly string _autoCompliteFormatString = "http://www.dom.mos.ru/Lookups/GetSearchAutoComplete?term={0}&section=Buildings";

        public async Task<IList<string>> StreetHints(string street)
        {
            try
            {
                var uri = string.Format(_autoCompliteFormatString, HttpUtility.UrlEncode(street));

                using (var client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Encoding = Encoding.UTF8;
                    var response = await client.DownloadStringTaskAsync(uri);

                    return await Task.Run(() =>
                        JsonConvert
                            .DeserializeObject<List<home_info>>(response)
                            // API возвращает 50 результатов
                            .Take(50)
                            .Select(x => x.GetStreetName())
                            .Where(x => x.IndexOf(street, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            .Distinct()
                            .ToList());
                }
            }
            catch (Exception e)
            {
                Tracer.WriteError(e);
                return new List<string>();
            }
        }

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
        }

        #endregion
    }

}
