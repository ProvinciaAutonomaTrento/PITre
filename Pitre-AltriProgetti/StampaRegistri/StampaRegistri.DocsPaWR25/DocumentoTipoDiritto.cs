using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), XmlType(Namespace = "http://localhost")]
	[Serializable]
	public enum DocumentoTipoDiritto
	{
		TIPO_PROPRIETARIO,
		TIPO_TRASMISSIONE,
		TIPO_TRASMISSIONE_IN_FASCICOLO,
		TIPO_SOSPESO,
		TIPO_ACQUISITO
	}
}
