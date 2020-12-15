using System;
using System.Xml.Serialization;

namespace DocsPaVO.filtri.filtriConservazione
{
    [Serializable()]
    [XmlType("FiltriConservazione")]
    public enum listaArgomenti  

    { 

        DATA_INVIO_IL,
        DATA_INVIO_DAL,
        DATA_INVIO_AL,
        DATA_INVIO_TODAY,
        DATA_INVIO_SC,
        DATA_INVIO_MC,
        DATA_CHIUSURA_IL,
        DATA_CHIUSURA_DAL,
        DATA_CHIUSURA_AL,
        DATA_CHIUSURA_TODAY,
        DATA_CHIUSURA_SC,
        DATA_CHIUSURA_MC,
        DATA_RIFIUTO_IL,
        DATA_RIFIUTO_DAL,
        DATA_RIFIUTO_AL,
        DATA_RIFIUTO_TODAY,
        DATA_RIFIUTO_SC,
        DATA_RIFIUTO_MC,
        ID_ISTANZA,
        ID_ISTANZA_DAL,
        ID_ISTANZA_AL,
        ID_SUPPORTO,
        ID_SUPPORTO_DAL,
        ID_SUPPORTO_AL,
        DATA_GENERAZIONE_IL,
        DATA_GENERAZIONE_DAL,
        DATA_GENERAZIONE_AL,
        DATA_GENERAZIONE_TODAY,
        DATA_GENERAZIONE_SC,
        DATA_GENERAZIONE_MC,
        DATA_ES_VERIFICA_IL,
        DATA_ES_VERIFICA_DAL,
        DATA_ES_VERIFICA_AL,
        DATA_ES_VERIFICA_TODAY,
        DATA_ES_VERIFICA_SC,
        DATA_ES_VERIFICA_MC,
        TIPO_DI_VERIFICA,
        ESITO_VERIFICA,
        PROTOCOLLO,
        PROTOCOLLO_DAL,
        PROTOCOLLO_AL,
        DOCUMENTO,
        DOCUMENTO_DAL,
        DOCUMENTO_AL,
        DATA_STAMPA_DA,
        DATA_STAMPA_A,
        DATA_STAMPA_IL,
        DATA_STAMPA_TODAY,
        DATA_STAMPA_SC,
        DATA_STAMPA_MC,
        FIRMATE

    }
}
