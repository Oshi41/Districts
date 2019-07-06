using System.Collections.Generic;
using DistrictsLib.Legacy.JsonClasses;
using DistrictsLib.Legacy.JsonClasses.Manage;

namespace DistrictsLib.Interfaces
{
    public interface IParser
    {
        List<CardManagement> LoadManage();

        List<Card> LoadCards();

        List<ForbiddenElement> LoadRules();

        List<HomeInfo> LoadCodes();

        List<Building> LoadSortedHomes();
        
    }
}
