using System.Collections.Generic;
using DistrictsLib.Interfaces.Json;
using DistrictsLib.Legacy.JsonClasses;
using DistrictsLib.Legacy.JsonClasses.Manage;

namespace DistrictsLib.Implementation.GoogleApi
{
    public class GoogleDataJson
    {
        public List<Card> Cards { get; set; } = new List<Card>();
        public List<HomeInfo> Codes { get; set; } = new List<HomeInfo>();
        public List<ICardManagement> Managements { get; set; } = new List<ICardManagement>();
        public List<ForbiddenElement> Restrictions { get; set; } = new List<ForbiddenElement>();
    }
}
