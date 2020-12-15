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
        public string descProduttore;
	}

    public enum LogAmministrazione
    {
        INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN,
        INTERROTTO_PROCESSO_ALLEGATO_DA_ADMIN,
        DOC_CAMBIO_STATO_ADMIN
    }
}
