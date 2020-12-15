using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class ProtocolloUscita : Protocollo 
	{
		public bool daAggiornareDestinatari = false;
		public bool daAggiornareDestinatariConoscenza = false; 
		public bool daAggiornareMittente = false;
		public DocsPaVO.utente.Corrispondente mittente;
		public DocsPaVO.utente.Corrispondente ufficioReferente;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.utente.Corrispondente))]
		public System.Collections.ArrayList destinatari;
		
		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.utente.Corrispondente))]
		public System.Collections.ArrayList destinatariConoscenza;
	}
}