using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Districts.Goggle;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class GoogleDriveApi
    {
        [TestMethod]
        public void Connect_Test()
        {
            var login = "user";
            var api = new GoggleDriveApi(Districts.Properties.Resources.credentials, "tokens");
            Task.WaitAll(api.ConnectAsync(login));
        }
    }
}
