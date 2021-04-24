using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace BinaryXML
{
    public partial class Attribute
    {
        static readonly Dictionary<AttributeType, Func<BinaryReader, string[], object>> BinaryReaders
            = new Dictionary<AttributeType, Func<BinaryReader, string[], object>>()
        {
            {
                AttributeType.Boolean, (reader, strings) => reader.ReadBoolean()
            },
            {
                AttributeType.Byte, (reader, strings) => reader.ReadByte()
            },
            {
                AttributeType.Short, (reader, strings) => reader.ReadInt16()
            },
            {
                AttributeType.Integer, (reader, strings) => reader.ReadUInt32()
            },
            {
                AttributeType.Float, (reader, strings) => reader.ReadSingle()
            },
            {
                AttributeType.LookupString, (reader, strings) => strings[reader.ReadInt16()]
            },
            {
                AttributeType.String, (reader, strings) => reader.ReadString()
            },
            {
                AttributeType.LengthEncodedString, (reader, strings) =>
                {
                    byte[] bytes = reader.ReadBytes(reader.ReadInt16());
                    StringBuilder builder = new StringBuilder(bytes.Length / 2);

                    for (int i = 0; i < bytes.Length; i += 2)
                    {
                        var chr = (char)bytes[i + 1];
                        builder.Append(chr, bytes[i]);
                    }

                    var str = builder.ToString();
                    return str;
                }
            },
        };

        static readonly Dictionary<AttributeType, Action<Attribute, BinaryWriter, Dictionary<string, short>>> BinaryWriters
            = new Dictionary<AttributeType, Action<Attribute, BinaryWriter, Dictionary<string, short>>>()
        {
            {
                AttributeType.Boolean, (attribute, writer, strings) => writer.Write(attribute.Boolean)
            },
            {
                AttributeType.Byte, (attribute, writer, strings) => writer.Write(attribute.Byte)
            },
            {
                AttributeType.Short, (attribute, writer, strings) => writer.Write(attribute.Short)
            },
            {
                AttributeType.Integer, (attribute, writer, strings) => writer.Write(attribute.Integer)
            },
            {
                AttributeType.Float, (attribute, writer, strings) => writer.Write(attribute.Float)
            },
            {
                AttributeType.LookupString, (attribute, writer, strings) => writer.Write(strings[attribute.String])
            },
            {
                AttributeType.String, (attribute, writer, strings) => writer.Write(attribute.String)
            },
            {
                AttributeType.LengthEncodedString, (attribute, writer, strings) =>
                {
                    List<byte> bytes = new List<byte>();

                    var str = attribute.String;
                    byte count = 0;
                    for (int i = 0; i < str.Length; i++)
                    {
                        char cur = str[i];

                        // Keep track of how many times the current characters appeared
                        count++;

                        // If the next character is different or we simply hit the end of the string
                        if (i == str.Length - 1 || cur != str[i + 1])
                        {
                            // Write down the count and the character
                            bytes.Add(count);
                            bytes.Add((byte)cur);
                            // Reset
                            count = 0;
                        }
                    }

                    writer.Write((short)bytes.Count);
                    writer.Write(bytes.ToArray());
                }
            },
        };

        public Attribute(BinaryReader reader, string[] strings)
        {
            var index = reader.ReadInt16();
            Name = strings[index];
            Value = BinaryReaders[Type = (AttributeType)reader.ReadByte()](reader, strings);
        }

        public void Serialize(BinaryWriter writer, Dictionary<string, short> strings)
        {
            writer.Write(strings[Name]);
            writer.Write((byte)Type);
            BinaryWriters[Type](this, writer, strings);
        }

        public void AddLookupStrings(Dictionary<string, short> strings)
        {
            strings.OptionalAdd(Name);
            if (Type == AttributeType.LookupString)
                strings.OptionalAdd(String);
        }
    }
}
