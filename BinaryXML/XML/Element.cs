using System.Xml;

namespace BinaryXML
{
    public partial class Element
    {
        public Element(XmlElement element)
        {
            // Ew dots, put slashes
            Name = element.Name.Replace('.', '/');

            for (int i = 0; i < element.Attributes.Count; i++)
            {
                Attributes.Add(new Attribute(element.Attributes[i]));
            }

            for (int i = 0; i < element.ChildNodes.Count; i++)
            {
                var child = element.ChildNodes[i];
                if (child is XmlElement childElement)
                {
                    Children.Add(new Element(childElement));
                }
                else if (child is XmlText text)
                {
                    Attributes.Add(new Attribute(Attribute._INNERTEXT,
                        (AttributeType)int.Parse(text.Value[0].ToString()),
                        text.Value.Substring(1)
                    ));
                }
            }
        }

        public void Serialize(XmlWriter writer)
        {
            // Slashes are XML suicide
            writer.WriteStartElement(Name.Replace('/', '.'));

            for (int i = 0; i < Attributes.Count; i++)
            {
                Attributes[i].Serialize(writer);
            }

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Serialize(writer);
            }

            writer.WriteEndElement();
        }
    }
}
