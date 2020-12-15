using System;
using System.Collections.Generic;
using System.Web;

namespace DocsPAWA.Spedizione
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICtrlListaDestinatari
    {
        void FetchData(DocsPaWR.SpedizioneDocumento spedizione, TipoDestinatarioEnum tipoDestinatario);
        void SaveData(DocsPaWR.SpedizioneDocumento spedizione);
        void SetStatoSpedizione(bool spedito);
        int Items { get; }
        bool AlmostOneChecked { get; }
    }

    public enum TipoDestinatarioEnum
    {
        Interno,
        Esterno,
        EsternoNonInteroperante,
        /// <summary>
        /// Interoperabilità semplficata
        /// </summary>
        SimplifiedInteroperability
    }
}
