using System;

namespace DocsPaVO.documento
{
	/// <summary>
	/// Possibili risultati di una protocollazione.
	/// </summary>
	public enum ResultProtocollazione
	{
        /// <summary>
        /// successo
        /// </summary>
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
		APPLICATION_ERROR,
		HM_UPDATE_ERROR,
		DOCUMENTO_GIA_PROTOCOLLATO,
        FORMATO_SEGNATURA_MANCANTE,
        FASCICOLO_NON_TROVATO,
        ERRORE_DURANTE_LA_FASCICOLAZIONE,
        ERRORE_WSPIA_PROTOCOLLO_ENTRATA_MITTENTE,
        ERRORE_WSPIA_CLASSIFICA_NODO_FOGLIA,
        ERRORE_WSPIA_PROTOCOLLAZIONE_SEMPLICE
	}
}
