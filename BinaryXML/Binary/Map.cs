using System.Collections.Generic;
using System.IO;

namespace BinaryXML
{
    public partial class Map
    {
        public const string _SIGNATURE = "CELESTE MAP";

        public Map(BinaryReader reader)
        {
            if (reader.ReadString() != _SIGNATURE)
                throw new InvalidDataException("Missing map signature at the start");

            PackageName = reader.ReadString();

            // Read strings lookup array
            var strings = new string[reader.ReadInt16()];
            for (int i = 0; i < strings.Length; i++)
            {
                strings[i] = reader.ReadString();
            }

            Root = new Element(reader, strings);
        }

        public void Serialize(BinaryWriter writer)
        {
            // Write signature and package name
            writer.Write(_SIGNATURE);
            writer.Write(PackageName);

            // Generate lookup array
            var strings = new Dictionary<string, short>
            {
                { PackageName, 0 }
            };
            Root.AddLookupStrings(strings);

            // Write lookup array
            writer.Write((short)strings.Count);
            foreach (string str in strings.Keys)
            {
                writer.Write(str);
            }

            // Write elements
            Root.Serialize(writer, strings);
        }
    }
}
