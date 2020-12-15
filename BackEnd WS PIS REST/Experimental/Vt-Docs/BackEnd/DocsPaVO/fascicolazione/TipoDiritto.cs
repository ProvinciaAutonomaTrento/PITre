using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.fascicolazione
{	
	/// <summary>
	/// </summary>
	[XmlType("FascicoloTipoDiritto")]
	public enum TipoDiritto
	{
		TIPO_PROPRIETARIO,
		TIPO_TRASMISSIONE,
		TIPO_ACQUISITO,
		TIPO_SOSPESO,
        TIPO_DELEGATO
	}
}
