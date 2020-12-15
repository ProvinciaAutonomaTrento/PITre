using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
	[XmlType("DocumentoParolaChiave")]
    [Serializable()]
	public class ParolaChiave 
	{
		public string systemId;
		public string descrizione;
		public string idAmministrazione;
	    public string idRegistro;
	}
}