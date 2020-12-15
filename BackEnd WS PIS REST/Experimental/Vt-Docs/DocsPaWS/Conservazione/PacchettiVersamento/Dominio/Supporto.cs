using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Dominio
{
    /// <summary>
    /// Dati del supporto di un'istanza di conservazione
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/Supporto")]
    public class Supporto
    {
        /// <summary>
        /// Identificativo univoco del supporto
        /// </summary>
        /// <remarks>
        /// Dato autoincrement generato dal sistema
        /// </remarks>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia del supporto creato
        /// </summary>
        public TipiSupportoEnum Tipo
        {
            get;
            set;
        }

        /// <summary>
        /// Numero di copia del supporto
        /// </summary>
        public string NumeroCopia
        {
            get;
            set;
        }

        /// <summary>
        /// Data di produzione del supporto
        /// </summary>
        public string DataProduzione
        {
            get;
            set;
        }

        /// <summary>
        /// Indirizzo http da cui è possibile scaricare il supporto
        /// </summary>
        public string Url
        {
            get;
            set;
        }
        
        //public string collocazioneFisica;
        //public string dataUltimaVerifica;
        //public string dataEliminazione;
        //public string esitoVerifica;
        //public string numVerifiche;
        //public string dataProxVerifica;
        //public string dataInsTSR;
        //public string dataScadenzaMarca;
        //public string marcaTemporale;
        //public string idConservazione;
        //public string statoSupporto;
        //public string Note;
        //public string idTipoSupporto;
        //public string TipoSupporto;
        //public string Capacita;
        ////durata del supporto ai fini della leggibilità
        //public string periodoVerifica;
        //public string percVerifica;
        //public string progressivoMarca = string.Empty;
        //public string idProfileTrasmissione;

        ///// <summary>
        ///// Url http in cui risiede il file istanza di conservazione
        ///// </summary>
        //public string istanzaDownloadUrl = string.Empty;
    }
}