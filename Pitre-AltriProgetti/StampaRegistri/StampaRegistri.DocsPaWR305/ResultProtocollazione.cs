using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), XmlType(Namespace = "http://localhost")]
	[Serializable]
	public enum ResultProtocollazione
	{
		OK,
		REGISTRO_CHIUSO,
		REGISTRO_MANCANTE,
		STATO_REGISTRO_ERRATO,
		AMMINISTRAZIONE_MANCANTE,
		MITTENTE_MANCANTE,
		DESTINATARIO_MANCANTE,
		OGGETTO_MANCANTE,
		DATA_SUCCESSIVA_ATTUALE,
		DATA_ERRATA,
		APPLICATION_ERROR
	}
}
