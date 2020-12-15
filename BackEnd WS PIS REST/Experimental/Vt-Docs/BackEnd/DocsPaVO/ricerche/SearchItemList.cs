using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.ricerche
{
	/// <summary>
	/// Classe utilizzata per scambiare la lista delle ricerche salvate
	/// </summary>
	[XmlType("SearchItemList")]
    [Serializable()]
	public class SearchItemList
	{
		/// <summary>
		/// Array di SearchItem contenente la lista dei criteri di ricerca disponibili
		/// per la pagina indicata
		/// </summary>
		[XmlArray()]
		public SearchItem[] lista = null;
	}
}
