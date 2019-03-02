using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Districts.Helper;
using Districts.New.Implementation.Classes;
using Districts.New.Interfaces;

namespace Districts.New.Implementation
{
    public class HomeParser : IHomeParser
    {
        #region fields

        private const string _floorsKey = "Количество этажей";
        private const string _doorsKey = "Количество жилых помещений, шт";
        private const string _elevatorKey = "Количество лифтов";
        private const string _enteranceKey = "Количество подъездов";
        private const string _maxKey = "наибольшее";

        private const string _uri = "http://www.dom.mos.ru/Building/Passport?pk=";

        #endregion

        public async Task<iHome> DownloadAndParse(IRawHome raw)
        {
            var street = raw.Street();
            var houseNumber = raw.HouseNumber();
            var uri = _uri + raw.UriPart().Replace("/Building/Details/", "");
            var html = "";

            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "text/html");
                client.Encoding = Encoding.UTF8;
                html = await client.DownloadStringTaskAsync(new Uri(uri));
            }

            int.TryParse(FindValueTag(html, _elevatorKey), out var elevators);
            int.TryParse(FindValueTag(html, _enteranceKey), out var entrances);
            int.TryParse(FindValueTag(html, _doorsKey), out var doors);
            int.TryParse(FindValueTag(html, _floorsKey, _maxKey), out var floors);

            

            IList<iDoor> result = new List<iDoor>();

            for (int i = 1; i <= doors; i++)
            {
                result.Add(new Door(street,
                    houseNumber,
                    i,
                    GetEntrance(i, doors, entrances),
                    DoorStatus.Good,
                    new List<iCode>()));
            }

            Tracer.Tracer.Instance.Write($"Parsed home - {street} {houseNumber}");

            return new Home(street, houseNumber, result, false, 1, floors, entrances);
        }

        /// <summary>
        /// Высчитывает подъезд, в котором находится
        /// </summary>
        /// <param name="door">Номер квартиры</param>
        /// <param name="total">Всего квартир</param>
        /// <param name="totalEntrances">Всего подъездов</param>
        /// <returns></returns>
        public int GetEntrance(int door, int total, int totalEntrances)
        {
            // квартиры в подъезде
            var onEntrance = Math.Ceiling((double)total / totalEntrances);

            // прохожу по всем подъездам
            for (var i = 0; i < totalEntrances; i++)
            {
                // номер подъезда
                var tempEntrance = i + 1;
                // если находится в пределах, то нашли наш
                if (door >= onEntrance * i && door <= onEntrance * tempEntrance) return tempEntrance;
            }

            return 1;
        }

        #region helping

        /// <summary>
        ///     Находит значене по имени ноды
        /// </summary>
        /// <param name="document">HTML документ</param>
        /// <param name="nodeName">Название ноды</param>
        /// <returns></returns>
        private string FindValueTag(string document, string nodeName)
        {
            var index = document.IndexOf(nodeName, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return string.Empty;

            index += nodeName.Length;

            return GetValueNode(document, index);
        }

        /// <summary>
        ///     Находит нужную ноду, после чего ищет дочернюю и выцепляет её значение
        /// </summary>
        /// <param name="document">HTML документ</param>
        /// <param name="baseNode">Основная нода - родитель</param>
        /// <param name="childNode">Дочерняя нода</param>
        /// <returns></returns>
        private string FindValueTag(string document, string baseNode, string childNode)
        {
            var index = document.IndexOf(baseNode, StringComparison.Ordinal);
            if (index < 0)
                return "";
            index += baseNode.Length;

            // ищем следующее
            index = document.IndexOf(childNode, index, StringComparison.Ordinal);
            if (index < 0)
                return "";
            index += childNode.Length;


            return GetValueNode(document, index);
        }

        /// <summary>
        ///     Находит значение ближайшей ноды к индексу
        /// </summary>
        /// <param name="document">HTML документ</param>
        /// <param name="findIndex">Индекс ноды</param>
        /// <returns></returns>
        private string GetValueNode(string document, int findIndex)
        {
            // Нашли закрывающийся таг
            var valueTagIndex = document.IndexOf("<", findIndex, StringComparison.Ordinal);

            // Нашли открывающийся таг значения
            // Больше на 1, так как ищем следующее попадание
            valueTagIndex = document.IndexOf("<", valueTagIndex + 1, StringComparison.Ordinal);

            // индекс закрывающейся кавыки прямо перед значением
            var beforeValue = document.IndexOf(">", valueTagIndex, StringComparison.Ordinal);
            // индекс открывающейся кавычки сразу после значения
            var afterValue = document.IndexOf("<", beforeValue, StringComparison.Ordinal);

            // Обрезаем, тка как попадаем на скобку
            var start = beforeValue + 1;
            var count = afterValue - start;
            var value = document.Substring(start, count).Replace(" ", "");

            return value;
        }

        #endregion
    }
}
