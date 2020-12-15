using System;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
	[XmlType("DocumentoLogDocumento")]
    [Serializable()]
	public class LogDocumento
	{
		public string systemId;
		public string dataAzione;
        public string userIdOperatore;
        public string idPeopleOPeratore;
        public string idGruppoOperatore;
        public string idAmm;
        public string descrOggetto;
        public string codAzione;
        public string chaEsito;
	}
}
