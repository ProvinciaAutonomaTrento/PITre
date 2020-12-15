using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
	[XmlType("DocumentoQueryOggetto")]
    [Serializable()]
	public class QueryOggetto 
	{
		public ArrayList idRegistri;
		public string idAmministrazione;
		public string queryDescrizione;
        public string queryCodice;
	}
}