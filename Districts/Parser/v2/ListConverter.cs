using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Districts.Parser.v2.Converters
{
    /// <summary>
    /// Конвертер для списков.
    /// Нужно указать конкретный тип и тип интерфейса!
    /// </summary>
    class ListConverter<TConcrete, TInterface> : JsonConverter
        where TConcrete : class, TInterface
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var list = serializer
                .Deserialize<List<TConcrete>>(reader)
                .Cast<TInterface>()
                .ToList();

            return list;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
