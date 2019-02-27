using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Districts.Singleton.Implementation
{
    interface IAppSettings
    {
        string BaseFolder { get; set; }

        int MaxDoors { get; set; }

        string GetBuildingPath();

        string GetCardsPath();

        string GetStreetsFile();

        string GetHomeInfoPath();

        string GetRestrictionsPath();

        string GetLogsPath();

        string GetManagePath();

        string GetBackupPath();

        string GetConfigFile();

        void Read();

        void Save();
    }
}
