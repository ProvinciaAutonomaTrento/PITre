using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.trasmissione
{
    /// <summary>
    /// Gestione dei metadati di una trasmissione a documento o fascicolo
    /// </summary>
    [Serializable()]
    public class InfoTrasmissione
    {
        /// <summary>
        /// 
        /// </summary>
        public InfoTrasmissione()
        { }

        public string IdTrasmissione = string.Empty;
        public string IdRuolo = string.Empty;
        public string Ruolo = string.Empty;
        public string IdUtente = string.Empty;
        public string Utente = string.Empty;
        public string Tipo = string.Empty;
        public string IdDocumento = string.Empty;
        public string IdFascicolo = string.Empty;
        public string DataInvio = string.Empty;
        public string NoteGenerali = string.Empty;
        public string IdRagioneTrasmissione = string.Empty;
        public string RagioneTrasmissione = string.Empty;
        public string DataScadenza = string.Empty;
        public string SalvataConCessione = string.Empty;
        public string ruoloMittente = string.Empty;
        public string UserId = string.Empty;
        //proprietà video 
        public string videoMittRuolo = string.Empty;
        public string videoMittUtente = string.Empty;
        public string UtenteDelegato = string.Empty;
    }
}