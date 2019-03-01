using Districts.New.Interfaces;
using Districts.Singleton;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DistrictsTest
{
    [TestClass]
    public class WebWorkerTest
    {
        [TestMethod]
        public void DownloadStreet()
        {
            var worker = IoC.Instance.Get<IWebWorker>();
            var streets = new[] { "Тимуровская улица" };

            var homes = worker.DownloadHomes(streets).Result;

            Assert.IsTrue(homes.Count > 2);
        }
    }
}
