using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Marvin.Commons.Utilities
{
    /// <summary>
    /// Class utility to serialize / deserialize objects.
    /// </summary>
    public class ClassToXML
    {
        /// <summary>
        /// Deserialize a XML in object.
        /// </summary>
        /// <param name="xml">XML string.</param>
        /// <param name="type">Object type</param>
        /// <returns>Object</returns>
        public static object DeserializeXML(string xml, Type type)
        {
            //TODO: Tratar exceções
            XmlSerializer serializer = new XmlSerializer(type);
            StringReader sr = new StringReader(xml);
            return serializer.Deserialize(sr);
        }

        /// <summary>
        /// Serialize object to XML.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="rootName">string</param>
        /// <returns>XML result.</returns>
        public static string SerializeObject(object obj, string rootName = null)
        {
            //TODO: Tratar exceções
            if (rootName == null)
                rootName = obj.GetType().Name;
            XmlSerializer serializer = new XmlSerializer(obj.GetType(), new XmlRootAttribute { ElementName = rootName });
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, obj);

            UTF8Encoding encoding = new UTF8Encoding();
            string xml = encoding.GetString(ms.ToArray());

            xml = xml.Replace("<?xml version=\"1.0\"?>", "");
            xml = xml.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            xml = xml.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

            return xml;
        }

        /// <summary>
        /// Serialize an byte array to XML
        /// </summary>
        /// <param name="bytes">Byte array</param>
        /// <returns>XML result.</returns>
        public static string SerealizeBytes(Byte[] bytes)
        {
            //TODO: Tratar exceções
            XmlSerializer serializer = new XmlSerializer(bytes.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, bytes);

            UTF8Encoding encoding = new UTF8Encoding();
            string xml = encoding.GetString(ms.ToArray());

            xml = xml.Replace("<?xml version=\"1.0\"?>", "");
            xml = xml.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            xml = xml.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

            XmlDocument docXml = new XmlDocument();
            docXml.InnerXml = xml;
            xml = docXml.FirstChild.InnerText;

            return xml;
        }

    }
}
