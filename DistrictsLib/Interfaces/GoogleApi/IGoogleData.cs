using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Implementation.GoogleApi;

namespace DistrictsLib.Interfaces.GoogleApi
{
    interface IGoogleData
    {
        Stream GetStreamFromData(GoogleDataJson json);

        GoogleDataJson GetDataFromStream(Stream stream);
    }
}
