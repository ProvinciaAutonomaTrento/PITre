using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
	[XmlType("DocumentoStatoDocumento")]
    [Serializable()]
	public class StatoDocumento 
	{
		public string systemId;
		public string descrizione;
	}
}