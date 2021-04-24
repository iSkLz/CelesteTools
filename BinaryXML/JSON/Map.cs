using System;
using Newtonsoft.Json;

namespace BinaryXML
{
    public partial class Map
    {
        public const string _ROOTPROP = "root";
        public const string _PACKAGEPROP = "packageName";

        public class JsonParser : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Map);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return new Map(reader);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                ((Map)value).Serialize(writer);
            }
        }

        public Map(JsonReader reader)
        {
            PackageName = reader.ReadStringProperty();
            reader.ReadUntilPast(JsonToken.PropertyName);
            Root = new Element(reader);
            reader.ReadUntilPast(JsonToken.EndObject);
        }

        public void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(_PACKAGEPROP);
            writer.WriteValue(PackageName);

            writer.WritePropertyName(_ROOTPROP);
            Root.Serialize(writer);

            writer.WriteEndObject();
        }
    }
}
