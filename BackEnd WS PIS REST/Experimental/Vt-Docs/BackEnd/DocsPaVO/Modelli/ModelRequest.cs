using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using DocsPaVO.utente;

namespace DocsPaVO.Modelli
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class ModelRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "documentId")]
        public string DocumentId = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "modelType")]
        public string ModelType = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName = "userInfo")]
        public InfoUtente UserInfo = null;

        /// <summary>
        /// Parsing del contenuto in formato xml dell'oggetto "ModelRequest"
        /// </summary>
        /// <param name="requestXml"></param>
        /// <returns></returns>
        public static ModelRequest Parse(string requestXml)
        {
            ModelRequest request = new ModelRequest();

            using (MemoryStream stream = new MemoryStream(System.Text.Encoding.Default.GetBytes(requestXml)))
            {
                XmlReader reader = XmlTextReader.Create(stream);

                while (reader.Read())
                {
                    if (reader.IsStartElement("modelRequest"))
                    {
                        request.DocumentId = MoveToAndGetAttributeValue(reader, "documentId");
                        request.ModelType = MoveToAndGetAttributeValue(reader, "modelType");
                    }
                    else if (reader.IsStartElement("userInfo"))
                    {
                        request.UserInfo = new InfoUtente();
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
