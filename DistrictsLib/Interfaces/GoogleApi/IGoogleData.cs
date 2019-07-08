using System.IO;
using DistrictsLib.Implementation.GoogleApi;

namespace DistrictsLib.Interfaces.GoogleApi
{
    interface IGoogleData
    {
        Stream GetStreamFromData(GoogleDataJson json);

        GoogleDataJson GetDataFromStream(Stream stream);
    }
}
