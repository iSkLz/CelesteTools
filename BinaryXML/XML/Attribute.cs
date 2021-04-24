using System.Xml;

namespace BinaryXML
{
    public partial class Attribute
    {
        public const string _INNERTEXT = "innerText";

        public Attribute(string name, AttributeType type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
        public Attribute(XmlAttribute data)
        {
            // Ew dots, put slashes
            Name = data.Name.Replace('.', '/');
            Type = (AttributeType)int.Parse(data.Value[0].ToString());
            // Inner text attributes have an extra line
            Value = TextParsers[Type](data.Value.Substring(Name == _INNERTEXT ? 2 : 1));
        }

        public void Serialize(XmlWriter writer)
        {
            if (Name == _INNERTEXT)
                // Add a line if the attribute is inner text
                writer.WriteString(((int)Type) + "\n" + Value.ToString());
            else
                // Even here, slashes are suicide
                writer.WriteAttributeString(Name.Replace('/', '.'), ((int)Type) + Value.ToString());
        }
    }
}
