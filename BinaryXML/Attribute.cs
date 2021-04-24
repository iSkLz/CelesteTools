using System;
using System.Collections.Generic;

namespace BinaryXML
{
    // WARNING:
    // When changing this enum pay close attention to the number of elements in it
    // XML attributes are serialized as one digit for the type then the value
    // If the number of elements here changes to 2+ digits you need to account for that in the XML methods
    public enum AttributeType
    {
        Boolean, Byte, Short, Integer, Float, LookupString, String, LengthEncodedString
    }

    public partial class Attribute
    {
        // Shared between xML and JSON
        static readonly Dictionary<AttributeType, Func<string, object>> TextParsers
            = new Dictionary<AttributeType, Func<string, object>>()
        {
            {
                AttributeType.Boolean, (str) => bool.Parse(str)
            },
            {
                AttributeType.Byte, (str) => byte.Parse(str)
            },
            {
                AttributeType.Short, (str) => short.Parse(str)
            },
            {
                AttributeType.Integer, (str) => int.Parse(str)
            },
            {
                AttributeType.Float, (str) => float.Parse(str)
            },
            {
                AttributeType.LookupString, (str) => str
            },
            {
                AttributeType.String, (str) => str
            },
            {
                AttributeType.LengthEncodedString, (str) => str
            },
        };

        public string Name;
        public AttributeType Type;
        public object Value;

        public string String => (string)Value;
        public short Short => (short)Value;
        public byte Byte => (byte)Value;
        public bool Boolean => (bool)Value;
        public int Integer => (int)Value;
        public float Float => (float)Value;
    }
}
