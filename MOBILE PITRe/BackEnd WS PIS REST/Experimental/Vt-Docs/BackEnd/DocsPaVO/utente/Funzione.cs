using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.utente 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class Funzione
	{
		public string systemId;
		public string codice;
		public string descrizione;
		public string idTipoFunzione;
		public string codTipoFunzione;
		public string descTipoFunzione;
		public string idParent;

		/// <summary>
		/// </summary>
		public Funzione()
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="systemId"></param>
		/// <param name="descrizione"></param>
		/// <param name="codice"></param>
		/// <param name="idTipoFunzione"></param>
		/// <param name="codTipoFunzione"></param>
		/// <param name="descTipoFunzione"></param>
		public Funzione(string systemId,
						string descrizione,
						string codice,
						string idTipoFunzione,
						string codTipoFunzione,
						string descTipoFunzione) 
		{
			this.systemId         = systemId;
			this.descrizione	  = descrizione;
			this.codice			  = codice;
			this.idTipoFunzione   = idTipoFunzione;
			this.codTipoFunzione  = codTipoFunzione;
            this.descTipoFunzione = descTipoFunzione;
		}
	}
}