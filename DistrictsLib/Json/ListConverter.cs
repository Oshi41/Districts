using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DistrictsLib.Json
{
    class ListConverter<TInt,T> : JsonConverter
        where T : TInt
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = serializer.Deserialize<List<T>>(reader);

            return result.OfType<TInt>().ToList();
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
