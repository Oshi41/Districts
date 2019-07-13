using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrictsLib.Extentions
{
    public static class DateTimeExtenstions
    {
        private const string _dateTimeFormat = "dd/MM/yyyy";
        private const string _timeTimeFormat = "hh.mm.ss";

        public static string ToPrettyString(this DateTime? time)
        {
            return time.HasValue
                ? time.Value.ToPrettyString()
                : string.Empty;
        }

        public static string ToPrettyString(this DateTime time)
        {
            return time.ToString(_dateTimeFormat);
        }

        public static string ToFullPrettyDateString(this DateTime time)
        {
            return $"{time.ToString(_dateTimeFormat)} {time.ToString(_timeTimeFormat)}";
        }
    }
}
