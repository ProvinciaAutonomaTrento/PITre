using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Pubblicazione
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class DocumentoDaPubblicare
    {
        /// <summary>
        /// 
        /// </summary>
        public string IdProfile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DocNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UtenteCreatore { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RuoloCreatore { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class FiltroDocumentiDaPubblicare
    {
        /// <summary>
        /// 
        /// </summary>
        public string IdDocumento { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdDocumentoFinale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DataPubblicazione { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DataPubblicazioneFinale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OraPubblicazione { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdTipoDocumento { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdTipoOggettoDataPubblicazione { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdTipoOggettoOraPubblicazione { get; set; }
    }
}
