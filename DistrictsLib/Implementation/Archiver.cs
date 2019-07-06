using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Interfaces;

namespace DistrictsLib.Implementation
{
    public class Archiver : IArchiver
    {
        #region Implementation of IArchiver

        public bool TryToZip(string source, string zip)
        {
            if (!Directory.Exists(source) || File.Exists(zip))
            {
                Trace.WriteLine($"Folder is not existing {source} or zip file already existing {zip}");
                return false;
            }

            try
            {
                ZipFile.CreateFromDirectory(source, zip);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return false;
            }
        }

        public bool TryUnzip(string zip, string destination)
        {
            if (Directory.Exists(destination) || !File.Exists(zip))
            {
                Trace.WriteLine($"Zip file is not existing {zip} or folder already existing {destination}");
                return false;
            }

            try
            {
                ZipFile.ExtractToDirectory(zip, destination);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return false;
            }
        }

        #endregion
    }
}
