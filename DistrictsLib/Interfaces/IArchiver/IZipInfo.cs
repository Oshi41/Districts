using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrictsLib.Interfaces.IArchiver
{
    public interface IZipInfo
    {
        List<string> RootedPaths { get; }
        string Comment { get; }
        DateTime CreatedDate { get; }
        string FullPath { get; }
    }
}
