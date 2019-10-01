using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace StampaRegistri.DocsPaWR305
{
	[GeneratedCode("System.Xml", "2.0.50727.42"), XmlType(Namespace = "http://localhost")]
	[Serializable]
	public enum FiltriTrasmissione
	{
		NOTE_GENERALI,
		NOTE_INDIVIDUALI,
		ACCETTATA_RIFIUTATA_IL,
		ACCETTATA_RIFIUTATA_SUCCESSIVA_AL,
		ACCETTATA_RIFIUTATA_PRECEDENTE_IL,
		SCADENZA_IL,
		SCADENZA_SUCCESSIVA_AL,
		SCADENZA_PRECEDENTE_IL,
		RISPOSTA_IL,
		RISPOSTA_SUCCESSIVA_AL,
		RISPOSTA_PRECEDENTE_IL
	}
}
