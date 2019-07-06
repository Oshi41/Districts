using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Legacy.JsonClasses;
using DistrictsLib.Legacy.JsonClasses.Manage;

namespace DistrictsLib.Implementation.GoogleApi
{
    public class GoogleDataJson
    {
        public List<Card> Cards { get; set; } = new List<Card>();
        public List<HomeInfo> Codes { get; set; } = new List<HomeInfo>();
        public List<CardManagement> Managements { get; set; } = new List<CardManagement>();
        public List<ForbiddenElement> Restrictions { get; set; } = new List<ForbiddenElement>();
    }
}
