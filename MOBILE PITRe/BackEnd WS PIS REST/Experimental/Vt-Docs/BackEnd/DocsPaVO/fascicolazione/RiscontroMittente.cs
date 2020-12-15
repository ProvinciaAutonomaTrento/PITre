using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.fascicolazione
{
    [XmlType("RiscontroMittente")]
    [Serializable()]
    public class RiscontroMittente
    {
        public string systemId = string.Empty;
        public string riferimentoMittente = string.Empty;
        public string idCorrGlobaliMittente = string.Empty;
        public string idRegistroDestinatario = string.Empty;
        public string idTitolarioDestinatario = string.Empty;
        public string codClassificaDestinatario = string.Empty;
        public string codFascicoloDestinatario = string.Empty;
        public string protocolloTitolarioDestinatario = string.Empty;
        public string dtaRiscontro = string.Empty;
    }
}
