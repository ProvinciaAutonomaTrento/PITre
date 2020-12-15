using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Definizione oggetto Tipo Funzione 
	/// relativo alla funzionalità Organigramma in Amministrazione.
	/// </summary>
	public class OrgTipoFunzione
	{
		public string IDTipoFunzione = string.Empty;

		public string Codice = string.Empty;

		public string Descrizione = string.Empty;

		public string IDAmministrazione = string.Empty;

		public string Associato = string.Empty;

		/// <summary>
		/// Singole funzioni associate al tipo funzione
		/// </summary>
		public OrgFunzione[] Funzioni=null;
	}
}
