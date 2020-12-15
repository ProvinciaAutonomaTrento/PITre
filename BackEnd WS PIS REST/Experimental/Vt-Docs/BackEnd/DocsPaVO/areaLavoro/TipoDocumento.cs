using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.areaLavoro
{
	/// <summary>
	/// </summary>
	[XmlType("AreaLavoroTipoDocumento")]
	public enum TipoDocumento
	{
		ARRIVO,
		PARTENZA,
		INTERNO,
		GRIGIO,
		TUTTI
	}
}