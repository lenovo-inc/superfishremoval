using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SuperFishRemovalTool
{
    /// <summary>
    /// Serializes XML
    /// </summary>
    internal static class Serializer
    {
        /// <summary>
        /// Converts XML into an object (that implements DataContract / IXmlSerializable)
        /// </summary>
        /// <typeparam name="T">Type of object to serialize</typeparam>
        /// <param name="xml">XML that represents the object</param>
        /// <returns>Instantiated instance</returns>
        public static T Deserialize<T>(string xml)
        {
            T obj = default(T);
            var readerSettings = new XmlReaderSettings()
            {
                CheckCharacters = false,
                IgnoreWhitespace = true,
            };

            using (var stringReader = new StringReader(xml))
            {
                using (var xmlReader = XmlReader.Create(stringReader, readerSettings))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    obj = (T)serializer.Deserialize(xmlReader);
                }
            }

            return obj;
        }


        /// <summary>
        /// Converts an object (that implements DataContract / IXmlSerializable) into XML
        /// </summary>
        /// <typeparam name="T">Type of object to create</typeparam>
        /// <param name="instance">Instance of type T</param>
        /// <returns>The serialized XML</returns>
        internal static string Serialize<T>(T instance)
        {
            string xml = null;
            var xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = false,
                NewLineHandling = NewLineHandling.Entitize,
                Encoding = new UTF8Encoding(false, false), // Prevent BOM! Very important
                // Encoding = new UnicodeEncoding(false, false), // Prevent BOM! Very important
            };

            using (var stringWriter = new StringWriter())
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                using (var xmlWriter = System.Xml.XmlWriter.Create(stringWriter, xmlWriterSettings))
                {
                    serializer.Serialize(xmlWriter, instance);
                }
                xml = stringWriter.ToString();
            }
            return xml;
        }
    }
}
