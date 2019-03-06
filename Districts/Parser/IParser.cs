using System.Collections.Generic;
using Districts.JsonClasses;
using Districts.JsonClasses.Manage;

namespace Districts.Parser
{
    public interface IParser
    {
        List<CardManagement> LoadManage();
        void SaveManage(List<CardManagement> manage);


        List<Card> LoadCards();
        void SaveCards(List<Card> cards);


        List<ForbiddenElement> LoadRules();
        void SaveRules(List<ForbiddenElement> rules);


        List<HomeInfo> LoadCodes();
        void SaveCodes(List<HomeInfo> codes);
    }
}
