using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class Storico 
	{
		public string systemId;
		public string dataModifica;
		public DocsPaVO.utente.Utente utente;
		public DocsPaVO.utente.Ruolo ruolo;
	}
}