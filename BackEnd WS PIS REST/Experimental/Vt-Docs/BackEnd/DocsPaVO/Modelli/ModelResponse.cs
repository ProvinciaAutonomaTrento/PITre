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
    /// Mantiene le informazioni relative al componente clientside 
    /// utilizzato per la creazione ed elaborazione del documento a partire dal modello
    /// </summary>
    [Serializable()]
    public class ModelProcessorInfo
    {
        /// <summary>
        /// Identificativo univoco del componente clientside
        /// </summary>
        [XmlAttribute(AttributeName = "id")]
        public int Id = 0;

        /// <summary>
        /// Nome leggibile nel componente clientside
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name = string.Empty;

        /// <summary>
        /// ClassId del componente client side 
        /// </summary>
        [XmlAttribute(AttributeName = "classId")]
        public string ClassId = string.Empty;

        /// <summary>
        /// Estensioni di file supportate dal word processor
        /// </summary>
        /// <remarks>
        /// Lista di estensioni separate da carattere |
        /// </remarks>
        [XmlAttribute(AttributeName = "supportedExtensions")]
        public string SupportedExtensions = null;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class Model
    {
        /// <summary>
        /// Tipo di modello documento richiesto
        /// </summary>
        [XmlAttribute(AttributeName = "modelType")]
        public string ModelType = string.Empty;

        /// <summary>
        /// Contenuto del modello
        /// </summary>
        [XmlElement(ElementName = "file")]
        public FileContent File = new FileContent();

        /// <summary>
        /// Lista della chiavi o placeholders da sostituire con i corrispondenti
        /// valori nel documento
        /// </summary>
        [XmlArray(ElementName = "keyValuePairs")]
        [XmlArrayItem(typeof(ModelKeyValuePair), ElementName = "pair")]
        public ModelKeyValuePair[] KeyValuePairs = new ModelKeyValuePair[0];
    }

    /// <summary>
    /// Entità che rappresenta una sezione da includere nel documento (compresa tra due bookmark)
    /// </summary>
    [Serializable()]
    public class IncludeSection
    {
        /// <summary>
        /// Nome del bookmark a partire da cui estrarre il contenuto dal file
        /// </summary>
        [XmlAttribute(AttributeName = "begin")]
        public string Begin = string.Empty;

        /// <summary>
        /// Nome del bookmark fino a cui estrarre il contenuto dal file
        /// </summary>
        [XmlAttribute(AttributeName = "end")]
        public string End = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName = "file")]
        public FileContent File = new FileContent();
    }

    /// <summary>
    /// Rappresenta il contenuto del file
    /// </summary>
    [Serializable()]
    public class FileContent
    {
        /// <summary>
        /// Nome del file nel documentale
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string FileName = string.Empty;

        /// <summary>
        /// Contenuto del file in formato bytearray
        /// </summary>
        [XmlAttribute(AttributeName = "content")]
        public byte[] Content = null;
    }

    /// <summary>
    /// Rappresenta un valore da sostituire nel modello del documento
    /// </summary>
    [Serializable()]
    public class ModelKeyValuePair
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "key")]
        public string Key = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName = "value")]
        public string Value = string.Empty;
    }

    /// <summary>
    /// Classe utilizzata per restituire le informazioni sul modello
    /// </summary>
    [Serializable()]
    [XmlRoot(ElementName = "modelResponse")]
    public class ModelResponse
    {
        /// <summary>
        /// Id del documento da elaborare
        /// </summary>
        [XmlAttribute(AttributeName = "documentId")]
        public string DocumentId = string.Empty;

        /// <summary>
        /// Contiene i dettagli dell'eventuale eccezione verificatasi
        /// nell'elaborazione dei dati del modello 
        /// </summary>
        [XmlAttribute(AttributeName = "exception")]
        public string Exception = string.Empty;

        /// <summary>
        /// Contiene le informazioni relative al componente clientside
        /// necessario per elaborare il documento
        /// </summary>
        [XmlElement(ElementName = "processorInfo")]
        public ModelProcessorInfo ProcessorInfo = new ModelProcessorInfo();

        /// <summary>
        /// Contiene i dati e il file del modello da elaborare
        /// </summary>
        [XmlElement(ElementName = "model")]
        public Model DocumentModel = new Model();

        /// <summary>
        /// Contenuto da inserire tra i due bookmark "Start_Book" e "End_Book" appositamente
        /// predisposti nel modello del documento da elaborare 
        /// </summary>
        [XmlArray(ElementName = "includeSections")]
        [XmlArrayItem(typeof(IncludeSection), ElementName = "includeSection")]
        public IncludeSection[] IncludeSections = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string ToXml(ModelResponse instance)
        {
            XmlSerializer serializer = new XmlSerializer(instance.GetType());

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, instance);
                stream.Seek(0, SeekOrigin.Begin);
                return new StreamReader(stream).ReadToEnd();
            }
        }
    }
}
