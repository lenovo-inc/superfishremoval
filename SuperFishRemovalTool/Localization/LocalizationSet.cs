using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SuperFishRemovalTool.Localization
{
    [XmlRoot(ElementName = "LocalizationSet", Namespace = null)]
    public class LocalizationSet
    {
        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
        
        [XmlArray("ItemList")]
        [XmlArrayItem("Item")]
        public Item[] ItemList { get; set; }
    }

    public class Item
    {
        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }
        [XmlText]
        public string Value { get; set; }

    }
}
