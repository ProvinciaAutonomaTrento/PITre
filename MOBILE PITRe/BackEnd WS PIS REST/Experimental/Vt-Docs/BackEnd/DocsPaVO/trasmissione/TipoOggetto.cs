using System;
using System.Xml.Serialization;
using System.Collections;
using DocsPaVO.utente;

namespace DocsPaVO.trasmissione
{
	/// <summary>
	/// </summary>
	[XmlType("TrasmissioneTipoOggetto")]
	public enum TipoOggetto
	{
		DOCUMENTO,
		FASCICOLO
	}
}