using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DistrictsLib.Extentions;
using DistrictsLib.Interfaces.IArchiver;
using Ionic.Zip;

namespace DistrictsLib.Implementation.Archiver
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
                            zipFile.AddDirectory(entry, Path.GetFileName(entry));
                        }
                        else if (File.Exists(entry))
                        {
                            zipFile.AddFile(entry, "");
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
            if (!File.Exists(zip))
            {
                Trace.WriteLine($"Zip file is not existing {zip}");
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

        public IZipInfo GetInfo(string zip)
        {
            var rooted = new List<string>();

            if (File.Exists(zip))
            {
                using (var file = new ZipFile(zip))
                {
                    rooted.AddRange(file
                        .EntryFileNames
                        .Select(x => x.TryGetRootedPath())
                        .Where(x => !string.IsNullOrEmpty(x)));

                    var comment = file.Comment;

                    var date = file?.Entries?.FirstOrDefault()?.LastModified ?? DateTime.MinValue;

                    return new ZipInfo(date, rooted, comment, zip);
                }
            }

            return null;
        }

        #endregion
    }
}
