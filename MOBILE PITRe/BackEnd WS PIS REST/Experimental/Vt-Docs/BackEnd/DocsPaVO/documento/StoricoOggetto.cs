using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
	[XmlType("DocumentoStoricoOggetto")]
    [Serializable()]
	public class StoricoOggetto: Storico 
	{
		public string occasionale;
		public string descrizione;
	}
}