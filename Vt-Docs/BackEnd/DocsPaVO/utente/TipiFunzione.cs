using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class TipiFunzione 
	{
		public string systemId;
		public string codice;
		public string descrizione;
		public bool daVisualizzare;
	}
}