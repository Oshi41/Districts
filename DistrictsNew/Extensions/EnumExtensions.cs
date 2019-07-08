using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Legacy.JsonClasses.Manage;

namespace DistrictsNew.Extensions
{
    public static class EnumExtensions
    {
        public static string Description(this ActionTypes types)
        {
            switch (types)
            {
                case ActionTypes.Dropped:
                    return Properties.Resources.Manage_Passed;

                case ActionTypes.Taken:
                    return Properties.Resources.Manage_Taken;

                case ActionTypes.Lost:
                    return Properties.Resources.Manage_Lost;

                default:
                    throw new Exception("Unknown type");
            }
        }
    }
}
