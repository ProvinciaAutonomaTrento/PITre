using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.ricerche
{
	/// <summary>
	/// Classe utilizzata per impostare e 
	/// reperire informazioni sulla ricerca fulltext
	/// </summary>
	[XmlType("FullTextSearchContext")]
    [Serializable()]
	public class FullTextSearchContext
	{
		/// <summary>
		/// Testo da ricercare
		/// </summary>
		public string TextToSearch=string.Empty;

		/// <summary>
		/// Gestione paginazione, indice della pagina richiesta
		/// </summary>
		public int RequestedPageNumber=0;
		
		/// <summary>
		/// Gestione paginazione, numero totale delle pagine
		/// </summary>
		public int TotalPageNumber=0;
		
		/// <summary>
		/// Numero totale di record restituiti, indipendentemente dalla paginazione
		/// </summary>
 		public int TotalRecordCount=0;

		/// <summary>
		/// Array di stringhe contenente tutti gli id dei documenti
		/// estratti dalla ricerca fulltext
		/// </summary>
		[XmlArray()]
		public string[] SearchResultList=null;
	}
}
