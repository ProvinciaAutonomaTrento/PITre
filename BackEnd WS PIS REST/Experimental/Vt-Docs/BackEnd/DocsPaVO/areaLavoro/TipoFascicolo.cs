using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.areaLavoro
{
	/// <summary>
	/// </summary>
	[XmlType("AreaLavoroTipoFascicolo")]
	public enum TipoFascicolo
	{
		GENERALE,
		PROCEDIMENTALE,
		TUTTI
	}
}