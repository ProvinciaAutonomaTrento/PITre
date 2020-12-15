using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class InfoSupporto
    {
        public string SystemID;
        public string numCopia;
        public string dataProduzione;
        public string collocazioneFisica;
        public string dataUltimaVerifica;
        public string dataEliminazione;
        public string esitoVerifica;
        public string numVerifiche;
        public string dataProxVerifica;
        public string dataInsTSR;
        public string dataScadenzaMarca;
        public string marcaTemporale;
        public string idConservazione;
        public string statoSupporto;
        public string Note;
        public string idTipoSupporto;
        public string TipoSupporto;
        public string Capacita;
        //durata del supporto ai fini della leggibilità
        public string periodoVerifica;
        public string percVerifica;
        public string progressivoMarca = string.Empty;
        public string idProfileTrasmissione;

        /// <summary>
        /// Url http in cui risiede il file istanza di conservazione
        /// </summary>
        public string istanzaDownloadUrl = string.Empty;

        /// <summary>
        /// Url http in cui è presente la pagina di browse dell'istanza di conservazione
        /// </summary>
        public string istanzaBrowseUrl = string.Empty;

        // MEV CS 1.5
        // campi specifici per verifiche di leggibilità
        public string dataUltimaVerificaLeggibilita;
        public string dataProxVerificaLeggibilita;
        public string esitoVerificaLeggibilita;
        public string numVerificheLeggibilita;
        public string percVerificheLeggibilita;
    }
}