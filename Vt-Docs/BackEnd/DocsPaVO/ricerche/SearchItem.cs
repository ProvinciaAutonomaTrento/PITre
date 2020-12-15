using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.ricerche
{
	/// <summary>
	/// Classe utilizzata per impostare e 
	/// salvare i criteri di ricerca
	/// </summary>
	[XmlType("SearchItem")]
    [Serializable()]
	public class SearchItem
	{
		/// <summary>
		/// Identificativo dell'elemento salvato
		/// </summary>
		public int system_id=0;

		/// <summary>
		/// Nome dell'elemento salvato
		/// </summary>
		public string descrizione = null;

        /// <summary>
        /// Tipo della ricerca
        /// </summary>
        public string tipo = null;

        /// <summary>
		/// Identificazione dell'utente proprietario
		/// </summary>
		public int owner_idPeople=0;
		
		/// <summary>
		/// Identificazione del ruolo proprietario
		/// </summary>
		public int owner_idGruppo=0;

		/// <summary>
		/// Nome della pagina di ricerca su cui ha validità l'elemento salvato
		/// </summary>
		public string pagina = null;

		/// <summary>
		/// Criteri di ricerca
		/// </summary>
		public string filtri = null;

		public override string ToString()
		{
			string f = "null";
			if (filtri!=null && filtri!="")
				f = "not null";
			string myself = "system_id: <" + system_id + ">,\n" +
				"descrizione: <" + descrizione + ">,\n" +
				"idPeople: <" + owner_idPeople + ">,\n" +
				"idGruppo: <" + owner_idGruppo + ">,\n" +
				"pagina: <" + pagina + ">,\n" +
				"filtri: <" + f + ">";
			return myself;
		}

        public string gridId;
	}
}
