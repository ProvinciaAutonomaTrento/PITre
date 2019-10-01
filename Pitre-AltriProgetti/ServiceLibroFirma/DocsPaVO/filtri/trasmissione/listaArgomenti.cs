using System;
using System.Xml.Serialization;

namespace DocsPaVO.filtri.trasmissione 
{
	/// <summary>
	/// Summary description for filtriTrasmissione.
	/// </summary>
	[XmlType("FiltriTrasmissione")]
	public enum listaArgomenti 
	{
		
		NOTE_GENERALI,
		NOTE_INDIVIDUALI,
		SCADENZA_IL,
		SCADENZA_SUCCESSIVA_AL,
		SCADENZA_PRECEDENTE_IL,
		RISPOSTA_IL,
		RISPOSTA_SUCCESSIVA_AL,
		RISPOSTA_PRECEDENTE_IL,
        ORACLE_FIELD_FOR_ORDER,
        SQL_FIELD_FOR_ORDER,
        PROFILATION_FIELD_FOR_ORDER,
        ORDER_DIRECTION
	}

    [XmlType("FiltriModelliTrasmissione")]
    public enum listaArgomentiModelliTrasmissione
    {
        CODICE_MODELLO,
        DESCRIZIONE_MODELLO,
        RUOLI_DISABLED_RIC_TRASM,
        NOTE,
        TIPO_TRASMISSIONE,
        ID_REGISTRO,
        ID_RAGIONE_TRASMISSIONE,
        CODICE_CORR_PER_VISIBILITA,
        CODICE_CORR_PER_DESTINATARIO,
        RUOLI_DEST_DISABLED,
        MODELLI_CREATI_DA_UTENTE,
        MODELLI_CREATI_DA_AMMINISTRATORE
    }
}