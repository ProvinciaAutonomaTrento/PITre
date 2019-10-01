using NttDatalLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace NttDataWA.Utils
{
    public static class FatturaPARepository
    {
        public static XmlDocument GenerateXMLFromFattura(FatturaElettronicaType fattura)
        {
            XmlDocument XmlDoc = new XmlDocument();
            var stream = new System.IO.MemoryStream();
            string xml = "";
            try
            {
                var serializer = new XmlSerializer(typeof(NttDatalLibrary.FatturaElettronicaType));
                var writer = new NttDatalLibrary.CloserTagXMLWriter(stream);
                serializer.Serialize(writer, fattura);
                xml = Encoding.UTF8.GetString(stream.ToArray());
                int _indexFirstElement = xml.IndexOf("<");
                xml = xml.Substring(_indexFirstElement);
                
                XmlDoc.LoadXml(xml);
            }
            catch(Exception)
            {
                return null;
            }
            finally
            {
                stream.Dispose();
            }
            return XmlDoc;
        }

        public static FatturaElettronicaType GenerateFatturaFromXML(XmlDocument xmlDoc)
        {
            XmlReader reader = null;
            FatturaElettronicaType fattura;
            try
            {
                reader = XmlReader.Create((new System.IO.StringReader(xmlDoc.InnerXml)));
                var serializer = new XmlSerializer(typeof(FatturaElettronicaType));
                fattura = (FatturaElettronicaType)serializer.Deserialize(reader);
            }
            catch(Exception)
            {
                return null;
            }
            return fattura;
        }
    }
}