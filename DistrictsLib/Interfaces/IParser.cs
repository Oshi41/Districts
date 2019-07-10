using System.Collections.Generic;
using DistrictsLib.Interfaces.Json;
using DistrictsLib.Legacy.JsonClasses;
using DistrictsLib.Legacy.JsonClasses.Manage;

namespace DistrictsLib.Interfaces
{
    public interface IParser
    {
        List<ICardManagement> LoadManage();

        List<Card> LoadCards();

        List<ForbiddenElement> LoadRules();

        List<HomeInfo> LoadCodes();

        List<Building> LoadSortedHomes();

        IList<string> LoadStreets();
    }
}
