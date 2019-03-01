using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Districts.Comparers;
using Districts.JsonClasses;
using Districts.JsonClasses.Base;
using Districts.JsonClasses.Manage;
using Districts.Settings.v1;
using Newtonsoft.Json;

namespace Districts.Helper
{
    public static class LoadingWork
    {
        private static Parser.v1.Parser _parser = new Parser.v1.Parser();

        public static Dictionary<string, List<Building>> LoadSortedHomes()
        {
            return _parser.LoadSortedHomes();
        }

        public static Dictionary<string, List<ForbiddenElement>> LoadRules()
        {
            return _parser.LoadRules();
        }

        public static Dictionary<string, List<HomeInfo>> LoadCodes()
        {
            return _parser.LoadCodes();
        }

        public static Dictionary<string, Card> LoadCards()
        {
            return _parser.LoadCards();
        }

        public static Dictionary<string, CardManagement> LoadManageElements()
        {
            return _parser.LoadManageElements();
        }
    }
}