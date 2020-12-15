using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace DocsPaDocumentale.FullTextSearch
{
    public class SolrData
    {
        public class response
        {
            [XmlElement(ElementName = "lst")]
            public lista[] lst;
            public resultValue result;
        }
        public class lista
        {
            [XmlElement(ElementName = "int")]
            public dataValue[] integer;

            [XmlElement(ElementName = "str")]
            public dataValue[] str;

            [XmlElement(ElementName = "lst")]
            public lista[] lst;
        }


        public class dataValue
        {
            [XmlAttribute(AttributeName ="name")]
            public string name ;

            [XmlText  ()]
            public string Value;
        }

        public class resultValue
        {
            [XmlAttribute(AttributeName = "name")]
            public string name;
            
            [XmlAttribute(AttributeName = "numFound")]
            public string numFound;
            
            [XmlAttribute(AttributeName = "start")]
            public string start;

            [XmlAttribute(AttributeName = "maxScore")]
            public string maxScore;

            [XmlElement(ElementName = "doc")]
            public doc[] docs;


        }


        public class doc
        {
            [XmlElement(ElementName = "str")]
            public docValue[] str;

            [XmlElement(ElementName = "float")]
            public docValue[] floats;
        }


        public class docValue
        {
            [XmlAttribute(AttributeName = "name")]
            public string name;

            [XmlText()]
            public string Value;
        }

        public response deserialize(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(response));
            return  (response)DeserializeObject<response>(xml);
        }


        static public t DeserializeObject<t>(String pXmlizedString)
        {
            XmlSerializer xs = new XmlSerializer(typeof(t));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

            try
            {
                return (t)xs.Deserialize(memoryStream);
            }
            catch (Exception e) { System.Console.WriteLine(e); return default(t); }

        }

        static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

    }
}
