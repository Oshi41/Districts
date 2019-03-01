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

        void UpdateRelationships();

        IList<iHome> LoadHomes();
        void SaveHomes(IList<iHome> homes);

        //IList<iCard> LoadCards();
        //
        //IList<iManage> LoadManagement();
        //void SaveCards(IList<Card> cards);
        //
        //void SaveManagements(IList<Manage> history);

    }
}