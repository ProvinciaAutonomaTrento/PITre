using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.areaLavoro
{
	/// <summary>
	/// </summary>
	[XmlType("AreaLavoroTipoOggetto")]
	public enum TipoOggetto
	{
	   DOCUMENTO,
	   FASCICOLO
	}
}