using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Interfaces.IArchiver;

namespace DistrictsLib.Implementation.Archiver
{
    class ZipInfo : IZipInfo
    {
        public ZipInfo(DateTime createdDate,
            List<string> rootedPaths,
            string comment, string fullPath)
        {
            CreatedDate = createdDate;
            RootedPaths = rootedPaths;
            Comment = comment;
            FullPath = fullPath;
        }

        #region Implementation of IZipInfo

        public List<string> RootedPaths { get; }

        public string Comment { get; }

        public DateTime CreatedDate { get; }

        public string FullPath { get; }

        #endregion
    }
}
