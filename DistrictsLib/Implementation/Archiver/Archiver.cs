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
        private readonly Encoding _encoding = Encoding.GetEncoding(866);

        #region Implementation of IArchiver

        public bool TryToZip(string zip, Func<ZipFile, string> commentFunc = null, params string[] entries)
        {
            if (File.Exists(zip) || entries.IsNullOrEmpty())
            {
                Trace.WriteLine($"Zip file already existing {zip} or entries are empty");
                return false;
            }

            if (commentFunc == null)
            {
                commentFunc = file => $"Created at {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}";
            }

            try
            {
                using (var zipFile = new ZipFile(_encoding))
                {
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

                    zipFile.Comment = commentFunc(zipFile);

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

        public bool TryUnzip(string zip, string destination, out string warnings)
        {
            warnings = string.Empty;

            if (!File.Exists(zip))
            {
                Trace.WriteLine($"Zip file is not existing {zip}");
                return false;
            }

            string temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            try
            {
                Directory.CreateDirectory(temp);

                using (var archive = new ZipFile(zip, _encoding))
                {
                    archive.ExtractAll(temp);
                    Trace.WriteLine($"Successfully extracted ZIP {zip} to temporary folder {temp}");

                    SafeMoveDir(temp, destination, out warnings);

                    Trace.WriteLine($"Successfully moved from {temp} to destination folder {destination}");
                }

                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return false;
            }
            finally
            {
                if (Directory.Exists(temp))
                    Directory.Delete(temp, true);
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

        private void SafeMoveDir(string source, string destination, out string warnings)
        {
            warnings = string.Empty;

            var folders = Directory
                .GetDirectories(source)
                .SelectRecursive(Directory.GetDirectories)
                .ToList();

            // создаем папки, если их нет
            foreach (var folder in folders)
            {
                var newFodler = folder.Replace(source, destination);
                if (!Directory.Exists(newFodler))
                    Directory.CreateDirectory(newFodler);
            }

            folders.Add(source);

            var all = folders.SelectMany(Directory.GetFiles);

            foreach (var item in all)
            {
                var newName = item.Replace(source, destination);

                try
                {
                    if (File.Exists(newName))
                    {
                        File.Delete(newName);
                    }

                    File.Move(item, newName);
                }
                catch (Exception e)
                {
                    warnings += e.Message + "\n";

                    Trace.WriteLine($"{nameof(Archiver)}.{nameof(SafeMoveDir)}: {e}");
                }
            }
        }
    }
}
