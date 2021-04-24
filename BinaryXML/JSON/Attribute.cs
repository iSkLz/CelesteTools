using Newtonsoft.Json;

namespace BinaryXML
{
    public partial class Attribute
    {
        public const string _NAMEPROP = "name";
        public const string _TYPEPROP = "type";
        public const string _VALPROP = "value";

        public Attribute(JsonReader reader)
        {
            Name = reader.ReadStringProperty();
            Type = (AttributeType)reader.ReadIntProperty();
            Value = TextParsers[Type](reader.ReadStringProperty());
            reader.ReadUntilPast(JsonToken.EndObject);
        }

        public void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(_NAMEPROP);
            writer.WriteValue(Name);

            writer.WritePropertyName(_TYPEPROP);
            writer.WriteValue((int)Type);

            writer.WritePropertyName(_VALPROP);
            writer.WriteValue(Value.ToString());

            writer.WriteEndObject();
        }
    }
}
