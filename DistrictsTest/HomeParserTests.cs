using System.Linq;
using Districts.New.Implementation;
using Districts.New.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DistrictsTest
{
    class FakeRawHome : IRawHome
    {
        public string UriPart()
        {
            return "/Building/Details/8f01d056-23f3-42f0-93d3-b210519e450c";
        }

        public string Street()
        {
            return "1";
        }

        public string HouseNumber()
        {
            return "";
        }
    }

    [TestClass]
    public class HomeParserTests
    {
        [TestMethod]
        public void Download()
        {
            var parser = new HomeParser();
            var r = parser.DownloadAndParse(new FakeRawHome()).Result;

            Assert.IsNotNull(r);
        }
    }
}
