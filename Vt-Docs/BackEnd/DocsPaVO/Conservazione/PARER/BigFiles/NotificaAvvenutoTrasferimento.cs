using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER.BigFiles
{
    [XmlRoot(ElementName = "notificaAvvenutoTrasferimentoFile")]
    public class NotificaAvvenutoTrasferimento
    {
        [XmlElement(ElementName = "nmAmbiente")]
        public string Ambiente { get; set; }

        [XmlElement(ElementName = "nmVersatore")]
        public string Versatore { get; set; }

        [XmlElement(ElementName = "cdKeyObject")]
        public string Chiave { get; set; }

        [XmlArray(ElementName = "listaFileDepositati")]
        [XmlArrayItem(ElementName = "fileDepositato")]
        public List<FileDepositato> ListaFiles { get; set; }
    }

    
    public class FileDepositato
    {
        [XmlElement(ElementName = "cdEncoding")]
        public string Encoding { get; set; }

        [XmlElement(ElementName = "dsHashFile")]
        public string Hash { get; set; }

        [XmlElement(ElementName = "nmNomeFile")]
        public string NomeFile { get; set; }

        [XmlElement(ElementName = "nmTipoFile")]
        public string TipoFile { get; set; }
    }
}
