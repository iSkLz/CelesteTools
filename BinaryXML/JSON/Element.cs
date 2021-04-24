using Newtonsoft.Json;

namespace BinaryXML
{
    public partial class Element
    {
        public const string _NAMEPROP = "name";
        public const string _ATTRPROP = "attributes";
        public const string _CHILDPROP = "children";

        public Element(JsonReader reader)
        {
            Name = reader.ReadStringProperty();
            Attributes = reader.ReadArrayProperty((r) => new Attribute(r));
            Children = reader.ReadArrayProperty((r) => new Element(r));
            reader.ReadUntilPast(JsonToken.EndObject);
        }

        public void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(_NAMEPROP);
            writer.WriteValue(Name);

            writer.WritePropertyName(_ATTRPROP);
            writer.WriteStartArray();
            foreach (var attribute in Attributes)
            {
                attribute.Serialize(writer);
            }
            writer.WriteEndArray();

            writer.WritePropertyName(_CHILDPROP);
            writer.WriteStartArray();
            foreach (var child in Children)
            {
                child.Serialize(writer);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
