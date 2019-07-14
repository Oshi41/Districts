using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.API.Archiver
{
    [TestClass]
    public class ArchiveTests
    {
        [TestMethod]
        public void ExtractFiles()
        {
            var entry = @"C:\Users\oshi4\AppData\Local\Districts\Backups\14.07.2019 09.57.56.zip";
            var archiver = new DistrictsLib.Implementation.Archiver.Archiver();

            var info = archiver.GetInfo(entry);
        }
    }
}
