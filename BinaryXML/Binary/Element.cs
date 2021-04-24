using System.Collections.Generic;
using System.IO;

namespace BinaryXML
{
    public partial class Element
    {
        public Element(BinaryReader reader, string[] strings)
        {
            Name = strings[reader.ReadInt16()];

            var attributes = reader.ReadByte();
            for (byte i = 0; i < attributes; i++)
            {
                var attribute = new Attribute(reader, strings);
                Attributes.Add(attribute);
            }

            var children = reader.ReadUInt16();
            for (short i = 0; i < children; i++)
            {
                Children.Add(new Element(reader, strings));
            }
        }

        public void Serialize(BinaryWriter writer, Dictionary<string, short> strings)
        {
            writer.Write(strings[Name]);

            writer.Write((byte)Attributes.Count);
            foreach (var attribute in Attributes)
            {
                attribute.Serialize(writer, strings);
            }

            writer.Write((short)Children.Count);
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Serialize(writer, strings);
            }
        }

        public void AddLookupStrings(Dictionary<string, short> strings)
        {
            strings.OptionalAdd(Name);

            foreach (var attribute in Attributes)
            {
                attribute.AddLookupStrings(strings);
            }

            foreach (var child in Children)
            {
                child.AddLookupStrings(strings);
            }
        }
    }
}
