using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class ServerPosta 
	{
		public string systemId;
		public string descrizione;
		public string serverPOP;
		public string portaPOP;
		public string serverSMTP;
		public string portaSMTP;
		public string dominio;
	}
}