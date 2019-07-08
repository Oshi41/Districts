using System.Collections.Generic;
using System.Linq;
using DistrictsLib.Extentions;
using DistrictsLib.Legacy.Comparers;
using DistrictsLib.Legacy.JsonClasses;
using DistrictsLib.Legacy.JsonClasses.Base;

namespace DistrictsLib.Legacy.Helping
{
    internal class HomeMap : List<HomeMap.FullHomeInfo>
    {
        public HomeMap()
        {
        }

        public HomeMap(IEnumerable<Building> homes,
            IEnumerable<HomeInfo> homeInfos,
            IEnumerable<ForbiddenElement> rules)
        {
            foreach (var home in homes)
            {
                var rule = rules.FirstOrDefault(x => x.IsTheSameObject(home)) ?? new ForbiddenElement(home);
                var homeInfo = homeInfos.FirstOrDefault(x => x.IsTheSameObject(home)) ?? new HomeInfo(home);

                Add(home, rule, homeInfo);
            }
        }

        public List<HomeInfo> GetHomeInfoValues
        {
            get
            {
                var temp = new List<HomeInfo>();
                foreach (var full in this)
                {
                    var info = full.HomeInfo;

                    if (info != null)
                        temp.Add(info);
                }

                return temp;
            }
        }

        public List<ForbiddenElement> GetRulesValues
        {
            get
            {
                var temp = new List<ForbiddenElement>();
                foreach (var full in this)
                {
                    var info = full.ForbiddenElement;

                    if (info != null)
                        temp.Add(info);
                }

                return temp;
            }
        }

        public List<Building> GetBuildingValues
        {
            get
            {
                var temp = new List<Building>();
                foreach (var full in this)
                {
                    var info = full.Building;

                    if (info != null)
                        temp.Add(info);
                }

                return temp;
            }
        }

        public List<Building> GetSortedBuildingValues
        {
            get
            {
                var temp = GetBuildingValues;
                temp.Sort(new HouseNumberComparer());
                return temp;
            }
        }

        public List<HomeMap> GetSortedByStreets
        {
            get
            {
                var temp = new List<HomeMap>();

                var mapped = this.GroupBy(x => x.Building.Street);

                foreach (var streetVals in mapped)
                {
                    var mapInner = new HomeMap();
                    mapInner.AddRange(streetVals.GetEnumerator().ToIEnumerable());
                    temp.Add(mapInner);
                }

                return temp;
            }
        }

        public int DoorsCount => this.Aggregate(0, (i, info) => i + info.Building.Doors);

        public ForbiddenElement GetRule(Building home)
        {
            var find = this.FirstOrDefault(x => Equals(x.Building, home));
            return find?.ForbiddenElement;
        }

        public HomeInfo GetHomeInfo(Building home)
        {
            var find = this.FirstOrDefault(x => Equals(x.Building, home));
            return find?.HomeInfo;
        }

        public bool Remove(Building home)
        {
            var find = this.FirstOrDefault(x => Equals(x.Building, home));
            return Remove(find);
        }

        public void Add(Building home, ForbiddenElement element = null, HomeInfo info = null)
        {
            var full = new FullHomeInfo
            {
                Building = home,
                HomeInfo = info,
                ForbiddenElement = element
            };

            Add(full);
        }

        /// <summary>
        ///     Returns sorted!!!!
        /// </summary>
        /// <param name="home"></param>
        /// <param name="all"></param>
        /// <param name="ignoreAfterSlash"></param>
        /// <returns></returns>
        public HomeMap FindSameHouseNumber(Building home,
            BaseFindableObject.ReturnConditions all = BaseFindableObject.ReturnConditions.AllWithSlashCheck)
        {
            if (home == null)
                return new HomeMap();

            var temp = this.Where(x => x.Building.TheSameHouse(home, all));

            var homes = temp.Select(x => x.Building).ToList();
            var infos = temp.Select(x => x.HomeInfo);
            var rules = temp.Select(x => x.ForbiddenElement);

            // отсортировали по возрастанию
            homes.Sort(new HouseNumberComparer());

            var result = new HomeMap(homes, infos, rules);
            return result;
        }

        internal class FullHomeInfo
        {
            public Building Building { get; set; }
            public ForbiddenElement ForbiddenElement { get; set; }
            public HomeInfo HomeInfo { get; set; }
        }
    }
}
