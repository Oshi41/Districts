using System.Collections.Generic;
using Districts.New.Implementation.Classes;
using Districts.New.Interfaces;

namespace Districts.Parser.v2
{
    public interface IParser
    {
        iCard LoadCard(int number);
        IList<string> LoadStreets();
        void SaveStreets(IList<string> streets);

        //IList<iCard> LoadCards();
        //IList<iHome> LoadHomes();
        //IList<iManage> LoadManagement();
        //void SaveCards(IList<Card> cards);
        //void SaveHomes(IList<Home> homes);
        //void SaveManagements(IList<Manage> history);

    }
}