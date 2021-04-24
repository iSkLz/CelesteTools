using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace BinaryXML
{
    [Serializable]
    public partial class Element
    {
        public string Name;

        public List<Attribute> Attributes = new List<Attribute>();
        public List<Element> Children = new List<Element>();

        public Element()
        {
        }
    }
}
