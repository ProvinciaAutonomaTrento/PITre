using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Pubblicazione
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    /// 
    [Serializable()]
    public class FiltroDocumenti
    {
        /// <summary>
        /// 
        /// </summary>
        public TipiDocumentiEnum TipoDocumento = TipiDocumentiEnum.Tutti;

        /// <summary>
        /// 
        /// </summary>
        public enum TipiDocumentiEnum
        {
            Tutti,
            Arrivo,
            Partenza,
            Interni,
            NonProtocollati,
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class FiltroPubblicazioneDocumenti
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID_TIPO_DOCUMENTO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ESITO_PUBBLICAZIONE { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class PubblicazioneDocumenti
    {
        public string SYSTEM_ID { get; set; }
        public string ID_TIPO_DOCUMENTO { get; set; }
        public string ID_PROFILE { get; set; }
        public string ID_USER { get; set; }
        public string ID_RUOLO { get; set; }
        public string DATA_DOC_PUBBLICATO { get; set; }
        public string DATA_PUBBLICAZIONE_DOCUMENTO { get; set; }
        public string ESITO_PUBBLICAZIONE { get; set; }
        public string ERRORE_PUBBLICAZIONE { get; set; }
    }
}
