using System;
using Districts.New.Interfaces;
using Districts.Singleton;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Districts.Parser.v2.Converters
{
    class CardConverter : JsonConverter<iCard>
    {
        private readonly IParser _parser;
        private readonly string _propName = "CardNumber";


        public CardConverter()
        {
            _parser = IoC.Instance.Get<IParser>();
        }

        public override void WriteJson(JsonWriter writer, iCard value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(_propName);
            writer.WriteValue(value.Number);
            writer.WriteEnd();
        }

        public override iCard ReadJson(JsonReader reader, Type objectType, iCard existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var json = JObject.Load(reader);

            var number = json[_propName].Value<int>();
            return _parser.LoadCard(number);
        }
    }
}
