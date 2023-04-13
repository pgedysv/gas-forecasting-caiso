using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Pge.GasOps.EGen.Cmri.Core.Utilities
{
    public static class Utilities
    {
        public static string SerializeObjectToXml(object obj)
        {
            string response;
            var xmlWriterSettings = new XmlWriterSettings() { Indent = true };
            var serializer = new XmlSerializer(obj.GetType());
            using (var stringWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
                {
                    serializer.Serialize(xmlWriter, obj);
                }

                response = stringWriter.ToString();
            }
            return response;
        }
    }
}
