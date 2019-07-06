using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Interfaces.GoogleApi;
using Newtonsoft.Json;

namespace DistrictsLib.Implementation.GoogleApi
{
    class RegularGoogleData : IGoogleData
    {
        #region Implementation of IGoogleData

        public Stream GetStreamFromData(GoogleDataJson json)
        {
            return new MemoryStream(
                Encoding
                    .UTF8
                    .GetBytes(
                        JsonConvert.SerializeObject(json)));
        }

        public GoogleDataJson GetDataFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                try
                {
                    return JsonConvert.DeserializeObject<GoogleDataJson>(json);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    return new GoogleDataJson();
                }
            }
        }

        #endregion
    }
}
