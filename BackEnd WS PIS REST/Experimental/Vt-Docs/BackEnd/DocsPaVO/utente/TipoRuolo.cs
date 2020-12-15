using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class TipoRuolo 
	{
		public string systemId;
		public string id_Amm;
		public string codice;
		public string descrizione;
		public string livello;
		public bool abilitato;
		public TipoRuolo Parent;
	}
}