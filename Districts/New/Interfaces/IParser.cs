using System.Collections.Generic;
using Districts.New.Implementation.Classes;
using Districts.New.Interfaces;

namespace Districts.Parser.v2
{
    interface IParser
    {
        iCard LoadCard(int number);
        IList<iCard> LoadCards();
        IList<iHome> LoadHomes();
        IList<iManage> LoadManagement();
        IList<string> LoadStreets();
        void SaveCards(IList<Card> cards);
        void SaveHomes(IList<Home> homes);
        void SaveManagements(IList<Manage> history);
        void SaveStreets(IList<string> streets);
    }
}