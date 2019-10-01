using System;
using System.Xml.Serialization;

namespace DocsPaVO.filtri.trasmissione
{
    /// <summary>
    /// </summary>
    [XmlType("FiltriTrasmissioneNascosti")]
    public enum listaArgomentiNascosti
    {
        TIPO_OGGETTO, //D:documento, F:fascicolo
        NO_CERCA_INFERIORI,
        ASSEGNAZIONI_PENDENTI,
        TODO_LIST,
        IN_RISPOSTA,
        ID_TRASMISSIONE,
        ATTIVITA_NON_CONCLUSE,
        TRASMISSIONE_DOC_PROTOCOLLATI,
        TRASMISSIONE_DOC_PROT_ARRIVO,
        TRASMISSIONE_DOC_PROT_PARTENZA,
        TRASMISSIONE_DOC_NON_PROTOCOLLATI,
        ID_UO, //relativo alle trasmissioni di una UO ed utilizzato per i report
        COD_RUBR_DEST_UTENTE,
        COD_RUBR_DEST_RUOLO,
        ID_UO_DEST,
        COD_RUBR_MITT_UTENTE,
        COD_RUBR_MITT_RUOLO,
        ID_UO_MITT,
        DESTINATARIO_UTENTE,
        DESTINATARIO_RUOLO,
        DESTINATARIO_UO,
        MITTENTE_UTENTE,
        MITTENTE_RUOLO,
        MITTENTE_UO,
        TRASMISSIONE_IL,
        TRASMISSIONE_SUCCESSIVA_AL,
        TRASMISSIONE_PRECEDENTE_IL,
        TRASMISSIONE_SC,
        TRASMISSIONE_MC,
        TRASMISSIONE_TODAY,
        TRASMISSIONE_IERI,
        TRASMISSIONE_ULTIMI_SETTE_GIORNI,
        TRASMISSIONE_ULTMI_TRENTUNO_GIORNI,
        RAGIONE,
        STATO,
        ACCETTATA_RIFIUTATA,
        DATA_ACCETTAZIONE,
        DATA_ACCETTAZIONE_DA,
        DATA_ACCETTAZIONE_A,
        DATA_ACCETTAZIONE_SC,
        DATA_ACCETTAZIONE_MC,
        DATA_ACCETTAZIONE_TODAY,
        DATA_RIFIUTO,
        DATA_RIFIUTO_DA,
        DATA_RIFIUTO_A,
        DATA_RIFIUTO_SC,
        DATA_RIFIUTO_MC,
        DATA_RIFIUTO_TODAY,
        DATA_ACC_RIF,
        DATA_ACC_RIF_DA,
        DATA_ACC_RIF_A,//A:accettata, R:rifiutata
        DATA_ACC_RIF_SC,
        DATA_ACC_RIF_MC,
        DATA_ACC_RIF_TODAY,
        PENDENTI,
        TRASMISSIONE_DOC_TUTTI,
        OGGETTO_DOCUMENTO_TRASMESSO,
        OGGETTO_FASCICOLO_TRASMESSO,
        ELEMENTI_NON_VISTI,
        DOCUMENTI_ACQUISITI,
        DOCUMENTI_FIRMATI,
        TRASMISSIONI_ACCETTATE,
        VISTE,
        MY_RECEIVED_TRANSMISSIONS,
        EFFETTUATE_RUOLI_IN_RF,
        DTA_INVIO,

        //per ordinamento todolist
        DTA_INVIO_DESC,
        DTA_SCAD_DESC,
        DTA_SCAD_ASC,
        DTA_INVIO_ASC,
        MANCANZA_IMMAGINE,
        MANCANZA_FASCICOLAZIONE,
        DA_PROTOCOLLARE,

        RICEVUTE_EFFETTUATE,

        AL_MIO_RUOLO,
        A_ME_STESSO,
        DAL_MIO_RUOLO, // Trasmissioni effettuate dal mio ruolo
        RUOLO_SOTTOPOSTO,
        PERSONA_SOTTOPOSTA,
        TRASMISSIONE_DOC_PROT_INTERNO,
        DIAGRAMMA_STATO_DOC,
        TIPO_ATTO,
        PROFILAZIONE_DINAMICA,
        TIPOLOGIA_FASCICOLO,
        DIAGRAMMA_STATO_FASC,
        CON_IMMAGINE,
        FIRMATO,
        TAB_TRASMISSIONI,
        TIPO_FILE_ACQUISITO,
        /// <summary>
        /// Nella pagina ricerca trasmissioni ricevute, è stato selezionato un
        /// ruolo ed è stato flaggato "Estendi a storicizzati"
        /// </summary>
        RUOLO_EXTEND_TO_HISTORICIZED,
        /// <summary>
        /// Nella pagina di ricerca trasmissioni ricevute, è stato selezionato un
        /// ruolo mittente ed è stato flaggato "Estendi a storicizzati"
        /// </summary>
        MITT_DEST_EXTEND_TO_HISTORICIZED,

        /// <summary>
        /// Trasmissione non lavorata dall'utente notificato selezionato
        /// </summary>
        NON_LAVORATE_DA_UTENTE_NOTIFICATO
    }
}