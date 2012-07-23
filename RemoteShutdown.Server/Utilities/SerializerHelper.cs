using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace RemoteShutdown.Utilities
{
    public class SerializerHelper
    {
        public static string SerializerObject<T>(T t)
        {
            using (var sw = new StringWriter())
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(sw, t);
                return sw.ToString();
            }
        }

        public static T DeserializeObject<T>(string xml)
        {
            using (var sr = new StringReader(xml))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(sr); 
            }
        }
    }
}
