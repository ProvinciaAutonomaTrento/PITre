using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
	[XmlType("DocumentoDatiEmergenza")]
    [Serializable()]
	public class DatiEmergenza 
	{
		public string protocolloEmergenza;
		public string dataProtocollazioneEmergenza;
		public string nomeProtocollatoreEmergenza;
		public string cognomeProtocollatoreEmergenza;
	}
}