using System;
using System.Xml.Serialization;
using System.Collections;
using DocsPaVO.utente;

namespace DocsPaVO.trasmissione
{
	/// <summary>
	/// </summary>
	[XmlType("TrasmissioneDiritti")]
	public class Diritti 
	{
		public string accessRights;
		public string idAmministrazione;
	}
}