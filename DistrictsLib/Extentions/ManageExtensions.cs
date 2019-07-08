using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Interfaces.Json;
using DistrictsLib.Legacy.JsonClasses.Manage;

namespace DistrictsLib.Extentions
{
    public static class ManageExtensions
    {
        public static DateTime? LastTaking(this ICardManagement management)
        {
            if (management?.Actions.IsNullOrEmpty() != false)
                return null;

            var result = management
                .Actions
                .OrderByDescending(x => x.Date)
                .FirstOrDefault(x => x.ActionType == ActionTypes.Taken);

            return result?.Date;
        }

        public static DateTime? LastPassed(this ICardManagement management)
        {
            if (management?.Actions.IsNullOrEmpty() != false)
                return null;

            var result = management
                .Actions
                .OrderByDescending(x => x.Date)
                .FirstOrDefault(x => x.ActionType == ActionTypes.Dropped);

            return result?.Date;
        }

        public static bool IsActive(this ICardManagement management)
        {
            if (management?.Actions.IsNullOrEmpty() != false)
                return false;

            return management
                   .Actions
                   .OrderByDescending(x => x.Date)
                   .First().ActionType == ActionTypes.Taken;
        }

        public static string LastOwner(this ICardManagement management)
        {
            if (management?.Actions.IsNullOrEmpty() != false)
                return null;

            return management
                       .Actions
                       .OrderByDescending(x => x.Date)
                       .First().Subject;
        }
    }
}
