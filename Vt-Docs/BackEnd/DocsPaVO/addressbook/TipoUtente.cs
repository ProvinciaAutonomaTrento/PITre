using System;
using System.Xml.Serialization;

namespace DocsPaVO.addressbook
{
	/// <summary>
	/// </summary>
	[XmlType("AddressbookTipoUtente")]
	public enum TipoUtente
	{
	   INTERNO,
	   ESTERNO,
	   GLOBALE
	}
}
