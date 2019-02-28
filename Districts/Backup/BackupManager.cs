using System;
using System.IO;
using Districts.Settings;
using Districts.Settings.v1;
using Ionic.Zip;
using Microsoft.Win32;

namespace Districts.Backup
{
    public class BackupManager
    {
        public void MakeBackup()
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "Aрхив 7-Zip (*.zip)|";

            if (dlg.ShowDialog() != true)
                return;

            using (var zip = new ZipFile())
            {
                var settings = ApplicationSettings.ReadOrCreate();

                zip.AddDirectory(settings.BuildingPath, Path.GetDirectoryName(settings.BuildingPath));
                zip.AddDirectory(settings.CardsPath, Path.GetDirectoryName(settings.CardsPath));
                zip.AddDirectory(settings.HomeInfoPath, Path.GetDirectoryName(settings.HomeInfoPath));
                zip.AddDirectory(settings.RestrictionsPath, Path.GetDirectoryName(settings.RestrictionsPath));
                zip.AddFile(settings.StreetsPath, Path.GetFileName(settings.StreetsPath));

                zip.Comment = "Был создан в " + DateTime.Now.ToShortTimeString();
                zip.Save(dlg.FileName);
            }
        }
    }
}
