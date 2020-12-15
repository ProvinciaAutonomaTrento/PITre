using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class Amministrazione 
	{
		public string systemId;
		public string descrizione;
		public string codice;
		public string libreria;
        public string email = string.Empty;
	}
}