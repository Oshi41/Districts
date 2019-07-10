using System.Collections.Generic;
using DistrictsLib.Interfaces.Json;
using DistrictsLib.Legacy.JsonClasses;
using DistrictsLib.Legacy.JsonClasses.Manage;

namespace DistrictsLib.Interfaces
{
    public interface ISerializer
    {
        void SaveManage(IReadOnlyCollection<ICardManagement> manage);
        void SaveCards(List<Card> cards);
        void SaveRules(List<ForbiddenElement> rules);
        void SaveCodes(List<HomeInfo> codes);
        void SaveSortedHomes(List<Building> homes);
    }
}
