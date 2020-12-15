using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR25
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), XmlType(Namespace = "http://localhost")]
	[Serializable]
	public enum FiltriFascicolazione
	{
		APERTURA_IL,
		APERTURA_SUCCESSIVA_AL,
		APERTURA_PRECEDENTE_IL,
		CHIUSURA_IL,
		CHIUSURA_SUCCESSIVA_AL,
		CHIUSURA_PRECEDENTE_IL,
		STATO,
		TITOLO,
		TIPO_FASCICOLO,
		CODICE_LEGISLATURA
	}
}
