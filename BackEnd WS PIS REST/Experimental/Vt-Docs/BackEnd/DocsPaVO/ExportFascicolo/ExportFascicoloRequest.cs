using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace DocsPaVO.ExportFascicolo
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class ExportFascicoloRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName = "userInfo")]
        public DocsPaVO.utente.InfoUtente UserInfo = null;

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "idFascicolo")]
        public string IdFascicolo = string.Empty;

         /// <summary>
        /// Parsing del contenuto in formato xml dell'oggetto "ExportFascicoloRequest"
        /// </summary>
        /// <param name="requestXml"></param>
        /// <returns></returns>
        public static ExportFascicoloRequest Parse(string requestXml)
        {
            ExportFascicoloRequest request = new ExportFascicoloRequest();

            using (MemoryStream stream = new MemoryStream(System.Text.Encoding.Default.GetBytes(requestXml)))
            {
                XmlReader reader = XmlTextReader.Create(stream);

                while (reader.Read())
                {
                    if (reader.IsStartElement("fascicolo"))
                    {
                        request.IdFascicolo = MoveToAndGetAttributeValue(reader, "id");
                    }
                    else if (reader.IsStartElement("userInfo"))
                    {
                        request.UserInfo = new DocsPaVO.utente.InfoUtente();
                        request.UserInfo.userId = MoveToAndGetAttributeValue(reader, "userId");
                        request.UserInfo.idCorrGlobali = MoveToAndGetAttributeValue(reader, "idCorrGlobali");
                        request.UserInfo.idPeople = MoveToAndGetAttributeValue(reader, "idPeople");
                        request.UserInfo.idGruppo = MoveToAndGetAttributeValue(reader, "idGruppo");
                        request.UserInfo.idAmministrazione = MoveToAndGetAttributeValue(reader, "idAmministrazione");
                        request.UserInfo.sede = MoveToAndGetAttributeValue(reader, "sede");
                        request.UserInfo.urlWA = MoveToAndGetAttributeValue(reader, "urlWA");
                        request.UserInfo.dst = MoveToAndGetAttributeValue(reader, "dst");
                    }
                }
            }

            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string ToXml(DocsPaVO.ExportFascicolo.MetaInfoFascicolo instance)
        {
            XmlSerializer serializer = new XmlSerializer(instance.GetType());
            
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, instance);
                stream.Seek(0, SeekOrigin.Begin);
                return new StreamReader(stream).ReadToEnd();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string MoveToAndGetAttributeValue(XmlReader reader, string name)
        {
            if (reader.MoveToAttribute(name))
                return reader.GetAttribute(name);
            else
                return string.Empty;
        }
    }
}