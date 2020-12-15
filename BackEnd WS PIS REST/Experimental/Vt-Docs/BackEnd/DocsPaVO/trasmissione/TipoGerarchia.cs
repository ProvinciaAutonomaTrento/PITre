using System;
using System.Xml.Serialization;
using System.Collections;
using DocsPaVO.utente;

namespace DocsPaVO.trasmissione
{
	/// <summary>
	/// </summary>
	[XmlType("TramissioneTipoGerarchia")]
	public enum TipoGerarchia
    {
        INFERIORE,
		PARILIVELLO,
		SUPERIORE,
		TUTTI
    }
}