using System;
using System.Xml.Serialization;
using System.Collections;
using DocsPaVO.utente;

namespace DocsPaVO.trasmissione
{
	/// <summary>
	/// </summary>
	[XmlType("TrasmissioneTipoDestinatario")]
	public enum TipoDestinatario
	{
		GRUPPO,
		UTENTE,
		RUOLO
	}
}