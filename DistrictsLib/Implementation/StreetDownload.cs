using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistrictsLib.Interfaces;
using DistrictsLib.Legacy.JsonClasses;
using DistrictsLib.Legacy.WebRequest;

namespace DistrictsLib.Implementation
{
    public class StreetDownload : IStreetDownload
    {
        private readonly StreetDownloader _legacyDownloader;

        public StreetDownload()
        {
            _legacyDownloader = new StreetDownloader();
        }

        #region Implementation of IStreetDownload

        public async Task<IList<string>> GetHints(string text)
        {
            var rawResults = await _legacyDownloader.RequestHomes(text);

            return rawResults
                .Take(MaxApi())
                .Select(x => x.GetFullStreetName()).Distinct().ToList();
        }

        public async Task<IList<Building>> DownloadStreet(string street)
        {
            return await _legacyDownloader.DownloadStreet(street);
        }

        public int MaxApi()
        {
            return 50;
        }

        #endregion
    }
}
