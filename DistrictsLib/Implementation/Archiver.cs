using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Ionic.Zip;
using DistrictsLib.Extentions;
using DistrictsLib.Interfaces;

namespace DistrictsLib.Implementation
{
    public class Archiver : IArchiver
    {
        #region Implementation of IArchiver

        public bool TryToZip(string zip, params string[] entries)
        {
            if (File.Exists(zip) || entries.IsNullOrEmpty())
            {
                Trace.WriteLine($"Zip file already existing {zip} or entries are empty");
                return false;
            }

            try
            {
                using (var zipFile = new ZipFile())
                {
                    //
                    // нужно для русского языка
                    //
                    zipFile.AlternateEncodingUsage = ZipOption.Always;
                    zipFile.AlternateEncoding = Encoding.GetEncoding(866);

                    foreach (var entry in entries)
                    {
                        if (Directory.Exists(entry))
                        {
                            zipFile.AddDirectory(entry, entry);
                        }
                        else if (File.Exists(entry))
                        {
                            zipFile.AddFile(entry, entry);
                        }
                        else
                        {
                            Trace.WriteLine($"Cannot detect entry {entry}");
                        }
                    }

                    var now = DateTime.Now;
                    zipFile.Comment = $"Created at {now.ToShortDateString()} {now.ToShortTimeString()}";

                    zipFile.Save(zip);
                    Trace.WriteLine($"Saved zip by path {zip}");
                }

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
                using (var archive = new ZipFile(zip))
                {
                    archive.ExtractAll(destination);
                    Trace.WriteLine($"Successfully extracted ZIP {zip} to destination folder {destination}");
                }

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
