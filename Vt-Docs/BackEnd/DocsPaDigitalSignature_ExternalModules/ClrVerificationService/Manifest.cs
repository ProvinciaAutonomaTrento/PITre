using System;
using System.Collections.Generic;
using System.Web;

namespace ClrVerificationService
{
    public class Manifest
    {

        public enum SignType
        {
            CADES,
            PADES
        }

        public class MainfestFileInformation
        {
            public string OriginalFullName;
            public string hash;
            public string SignedFullName;
        }

        public class ManifestFile
        {
            private static System.Xml.Serialization.XmlSerializer serializer;
            public string Token;
            public SignType SignatureType;
            public List<MainfestFileInformation> FileInformation = new List<MainfestFileInformation>();
            public bool cosign;
            public bool timestamp; 

            private static System.Xml.Serialization.XmlSerializer Serializer
            {
                get
                {
                    if ((serializer == null))
                    {
                        serializer = new System.Xml.Serialization.XmlSerializer(typeof(ManifestFile));
                    }
                    return serializer;
                }
            }

            public virtual string Serialize()
            {
                System.IO.StreamReader streamReader = null;
                System.IO.MemoryStream memoryStream = null;
                try
                {
                    memoryStream = new System.IO.MemoryStream();
                    Serializer.Serialize(memoryStream, this);
                    memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                    streamReader = new System.IO.StreamReader(memoryStream);
                    return streamReader.ReadToEnd();
                }
                finally
                {
                    if ((streamReader != null))
                    {
                        streamReader.Dispose();
                    }
                    if ((memoryStream != null))
                    {
                        memoryStream.Dispose();
                    }
                }
            }

            public static ManifestFile Deserialize(string xml)
            {
                System.IO.StringReader stringReader = null;
                try
                {
                    stringReader = new System.IO.StringReader(xml);
                    return ((ManifestFile)(Serializer.Deserialize(System.Xml.XmlReader.Create(stringReader))));
                }
                finally
                {
                    if ((stringReader != null))
                    {
                        stringReader.Dispose();
                    }
                }
            }
        }





    }
}