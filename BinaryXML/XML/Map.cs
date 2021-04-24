using System.Xml;

namespace BinaryXML
{
    public partial class Map
    {
        public const string _ROOTELEMENT = "celeste-map";
        public const string _PACKAGENAME = "package-name";

        public Map(XmlDocument document)
        {
            PackageName = document.DocumentElement.GetAttribute(_PACKAGENAME);

            foreach (var node in document.DocumentElement.ChildNodes)
            {
                if (node is XmlElement element)
                    Root = new Element(element);
            }
        }

        public void Serialize(XmlWriter writer)
        {
            writer.WriteStartElement(_ROOTELEMENT);
            writer.WriteAttributeString(_PACKAGENAME, PackageName);
            Root.Serialize(writer);
            writer.Close();
        }
    }
}
