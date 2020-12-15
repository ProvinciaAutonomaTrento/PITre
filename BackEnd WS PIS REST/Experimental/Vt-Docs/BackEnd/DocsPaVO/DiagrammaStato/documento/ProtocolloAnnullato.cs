using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
    [Serializable()]
	public class ProtocolloAnnullato 
	{
		public string autorizzazione;
		public string dataAnnullamento;
	}
}