using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace BusinessLogic.InstanceAccess
{
    class XmlInstance
    {
        DocsPaVO.InstanceAccess.Metadata.Instance istanza;

        public string  XmlFile 
        {
            get
            {
               
                return   SerializeObject<DocsPaVO.InstanceAccess.Metadata.Instance>(istanza);
            }
        }

        public  DocsPaVO.InstanceAccess.Metadata.Instance  Istanza
        {
            get
            {
                return istanza;
            }
            set
            {
                value = istanza;
            }

        }

        public XmlInstance(DocsPaVO.InstanceAccess.InstanceAccess instance, DocsPaVO.utente.InfoUtente infoUtente)
        {
            if (this.istanza == null)
                istanza = new DocsPaVO.InstanceAccess.Metadata.Instance();

            istanza.IdIstanza = instance.ID_INSTANCE_ACCESS;
            istanza.Descrizione = instance.DESCRIPTION;
            if (instance.RICHIEDENTE != null)
            {
                istanza.RichiedenteIstanza = new DocsPaVO.InstanceAccess.Metadata.Richiedente
                {
                    CodiceRichiedente = instance.RICHIEDENTE.codiceRubrica,
                    DescrizioneRichiedente = instance.RICHIEDENTE.descrizione
                };
            }
            if (!instance.CREATION_DATE.Equals(DateTime.MinValue))
            {
                istanza.DataRichiesta = instance.CREATION_DATE.ToShortDateString();
            }
            if (!instance.CLOSE_DATE.Equals(DateTime.MinValue))
                istanza.DataChiusura = instance.CLOSE_DATE.ToShortDateString();
            if (!string.IsNullOrEmpty(instance.ID_DOCUMENT_REQUEST))
            {
                DocsPaVO.documento.SchedaDocumento schDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, instance.ID_DOCUMENT_REQUEST, instance.ID_DOCUMENT_REQUEST);
                istanza.ProtocolloRichiesto = new DocsPaVO.InstanceAccess.Metadata.ProtocolloRichiesto()
                {
                     Oggetto = schDoc.oggetto.descrizione,
                     Segnatura = (schDoc.protocollo != null && !string.IsNullOrEmpty(schDoc.protocollo.segnatura)) ? schDoc.protocollo.segnatura : string.Empty,
                     DataCreazione = schDoc.dataCreazione,
                     IdDocumento = schDoc.docNumber                      
                };

            }
            istanza.Note = instance.NOTE;

        }

        public static String SerializeObject<t>(Object pObject)
        {
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(t));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xmlTextWriter.Formatting = Formatting.Indented;
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                xs.Serialize(xmlTextWriter, pObject, ns);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                memoryStream.Position = 0;
                //XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                StreamReader sr = new StreamReader(memoryStream);
                XmlizedString = sr.ReadToEnd();
                return XmlizedString;
            }
            catch (Exception e) { System.Console.WriteLine(e); return null; }
        }
    }
}
