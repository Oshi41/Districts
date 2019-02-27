using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Districts.New.Interfaces;
using Newtonsoft.Json;

namespace Districts.Singleton.Implementation
{
    class WebWork : IWebWorker
    {
        private readonly string _base = "http://www.dom.mos.ru/Lookups/GetSearchAutoComplete?term=";

        public async Task<IList<string>> Hints(string street)
        {
            var uri = _base + HttpUtility.UrlEncode(street);

            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Encoding = Encoding.UTF8;
                var response = await client.DownloadStringTaskAsync(uri);
                return JsonConvert.DeserializeObject<List<home_info>>(response)
                    .Select(x => x.label)
                    .ToList();
            }
        }

        #region Nested

        private class home_info
        {
            public string url { get; set; }
            public string value { get; set; }
            public string label { get; set; }
            public string section { get; set; }
        }

        #endregion
    }

}
