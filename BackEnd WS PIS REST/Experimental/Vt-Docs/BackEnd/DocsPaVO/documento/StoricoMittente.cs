using System;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
	[XmlType("DocumentoStoricoMittente")]
    [Serializable()]
	public class StoricoMittente: Storico 
	{
		public string cod_rubrica;
		public string descrizione;
	}
}
