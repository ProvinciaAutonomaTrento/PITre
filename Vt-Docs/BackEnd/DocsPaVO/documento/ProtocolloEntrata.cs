using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class ProtocolloEntrata : Protocollo 
	{
		public DocsPaVO.utente.Corrispondente mittente;
		public DocsPaVO.utente.Corrispondente mittenteIntermedio;
		public DocsPaVO.utente.Corrispondente ufficioReferente;
		public string descrizioneProtocolloMittente;
		public string dataProtocolloMittente;
		public bool daAggiornareMittente = false;
		public bool daAggiornareMittenteIntermedio = false;
        public string emailMittente = string.Empty;

        [XmlArray()]
        [XmlArrayItem(typeof(DocsPaVO.utente.Corrispondente))]
        public System.Collections.ArrayList mittenti;
        public bool daAggiornareMittentiMultipli = false;
	}
}