using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
	[XmlType("DocumentoTipoDiritto")]
	public enum TipoDiritto
	{
	   TIPO_PROPRIETARIO,
	   TIPO_TRASMISSIONE,
	   TIPO_TRASMISSIONE_IN_FASCICOLO,
	   TIPO_SOSPESO,
	   TIPO_ACQUISITO,
       TIPO_DELEGATO,
       TIPO_CONSERVAZIONE
	}
}