using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Districts.JsonClasses;

namespace Districts.WebRequest
{
    public class HomeDownloader
    {
        #region NodeNames

        private string _floorsKey = "Количество этажей";
        private string _doorsKey = "Количество жилых помещений";
        private string _elevatorKey = "Количество лифтов";
        private string _enteranceKey = "Количество подъездов";
        private string _childrenTag = "наибольшее";


        #endregion

        #region Fields

        /// <summary>
        /// Начало запроса
        /// </summary>
        private static readonly string HomeBaseUri = "http://www.dom.mos.ru/Building/Passport?pk=";
        /// <summary>
        /// То, что заменяем
        /// </summary>
        private string ReplaceblePrefix = "&section=Buildings";


        #endregion

        #region WebRequest

        /// <summary>
        /// Заполняет информаци о домах
        /// </summary>
        /// <param name="source">Дом, в который положим всю инфу</param>
        /// <param name="partOfUri">Часть url, которая приходит с запросом на улицу</param>
        /// <returns></returns>
        public async Task<Building> ParseProperties(Building source, string partOfUri)
        {
            var uri = GetCorrectUri(partOfUri);
            var html = await SendRequest(uri);

            source.Uri = uri;
            int.TryParse(FindValueTag(html, _elevatorKey), out var elevators);
            int.TryParse(FindValueTag(html, _enteranceKey), out var enterances);
            int.TryParse(FindValueTag(html, _doorsKey), out var doors);
            int.TryParse(FindValueTag(html, _floorsKey, _childrenTag), out var floors);

            source.Doors = doors;
            source.Floors = floors;
            source.Elevators = elevators;
            source.Entrances = enterances;

            return source;
        }

        private string GetCorrectUri(string partOfUri)
        {
            var result = partOfUri.Replace("/Building/Details/", "");
            return HomeBaseUri + result;
        }

        /// <summary>
        /// Возвраащет строку в виде HTML
        /// </summary>
        /// <param name="uri">URI информации о доме</param>
        /// <returns></returns>
        private async Task<string> SendRequest(string uri)
        {
            string result;
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "text/html");
                client.Encoding = Encoding.UTF8;
                result = await client.DownloadStringTaskAsync(new Uri(uri));
            }

            return result;
        }


        #endregion

        #region Parse work

        /// <summary>
        /// Находит значене по имени ноды
        /// </summary>
        /// <param name="document">HTML документ</param>
        /// <param name="nodeName">Название ноды</param>
        /// <returns></returns>
        private string FindValueTag(string document, string nodeName)
        {
            int index = document.IndexOf(nodeName, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return string.Empty;

            index += nodeName.Length;

            return GetValueNode(document, index);
        }

        /// <summary>
        /// Находит нужную ноду, после чего ищет дочернюю и выцепляет её значение
        /// </summary>
        /// <param name="document">HTML документ</param>
        /// <param name="baseNode">Основная нода - родитель</param>
        /// <param name="childNode">Дочерняя нода</param>
        /// <returns></returns>
        private string FindValueTag(string document, string baseNode, string childNode)
        {
            int index = document.IndexOf(baseNode, StringComparison.Ordinal);
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
        /// Находит значение ближайшей ноды к индексу
        /// </summary>
        /// <param name="document">HTML документ</param>
        /// <param name="findIndex">Индекс ноды</param>
        /// <returns></returns>
        private string GetValueNode(string document, int findIndex)
        {
            // Нашли закрывающийся таг
            int valueTagIndex = document.IndexOf("<", findIndex, StringComparison.Ordinal);

            // Нашли открывающийся таг значения
            // Больше на 1, так как ищем следующее попадание
            valueTagIndex = document.IndexOf("<", valueTagIndex + 1, StringComparison.Ordinal);

            // индекс закрывающейся кавыки прямо перед значением
            int beforeValue = document.IndexOf(">", valueTagIndex, StringComparison.Ordinal);
            // индекс открывающейся кавычки сразу после значения
            int afterValue = document.IndexOf("<", beforeValue, StringComparison.Ordinal);

            // Обрезаем, тка как попадаем на скобку
            int start = beforeValue + 1;
            int count = afterValue - start;
            var value = document.Substring(start, count).Replace(" ", "");

            return value;
        }
        #endregion
    }
}
