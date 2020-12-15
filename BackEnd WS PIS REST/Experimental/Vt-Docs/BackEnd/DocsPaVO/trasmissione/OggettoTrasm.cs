using System;
using System.Xml.Serialization;
using System.Collections;
using DocsPaVO.utente;

namespace DocsPaVO.trasmissione
{
	/// <summary>
	/// </summary>
	[XmlType("TrasmissioneOggettoTrasm")]
	public class OggettoTrasm 
	{
		public DocsPaVO.documento.InfoDocumento infoDocumento;
		public DocsPaVO.fascicolazione.InfoFascicolo infoFascicolo;
	}
}