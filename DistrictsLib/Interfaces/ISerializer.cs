using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Legacy.JsonClasses;
using DistrictsLib.Legacy.JsonClasses.Manage;

namespace DistrictsLib.Interfaces
{
    public interface ISerializer
    {
        void SaveManage(List<CardManagement> manage);
        void SaveCards(List<Card> cards);
        void SaveRules(List<ForbiddenElement> rules);
        void SaveCodes(List<HomeInfo> codes);
        void SaveSortedHomes(List<Building> homes);
    }
}
