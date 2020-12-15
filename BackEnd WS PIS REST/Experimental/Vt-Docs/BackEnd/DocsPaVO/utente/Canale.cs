using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class Canale 
	{
		public string systemId;
		public string descrizione;
		public string typeId;
		public string tipoCanale;
	}
}