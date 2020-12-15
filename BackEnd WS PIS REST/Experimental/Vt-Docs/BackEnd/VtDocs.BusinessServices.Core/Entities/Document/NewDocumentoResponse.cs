using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DocsPaVO.documento;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class NewDocumentoResponse : Response
    {
        /// <summary>
        /// Dati del nuovo documento
        /// </summary>
        public DocsPaVO.documento.SchedaDocumento Documento
        {
            get;
            set;
        }

        /// <summary>
        /// Lista delle tipologie documentali disponibili
        /// </summary>
        public DocsPaVO.documento.TipologiaAtto[] TipiDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// Lista dei modelli trasmissione disponibili
        /// </summary>
        public DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione[] ModelliTrasmissione
        {
            get;
            set;
        }

        /// <summary>
        /// Lista dei registri di protocollo visibili per l'utente
        /// </summary>
        /// <remarks>
        /// Dato restituito solo se nella request del servizio è richiesto
        /// </remarks>
        public DocsPaVO.utente.Registro[] RegistriProtocollo
        {
            get;
            set;
        }

        /// <summary>
        /// True se i mittenti multipli sono abilitati, false in caso contrario
        /// </summary>
        public bool MittentiMultipliEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Lista mezzi di spedizione visibili per l'utente
        /// </summary>
        public DocsPaVO.amministrazione.MezzoSpedizione[] MezziSpedizione
        {
            get;
            set;
        }
    }
}
