using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Legacy.JsonClasses.Manage;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DistrictsLib.Interfaces.Json
{
    public interface IManageRecord
    {
        /// <summary>
        ///     Время записи
        /// </summary>
        DateTime Date { get; set; }

        /// <summary>
        ///     Автор записи
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        ///     Тип события
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        ActionTypes ActionType { get; set; }
    }
}
