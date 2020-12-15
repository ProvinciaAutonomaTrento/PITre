using System;
using System.Collections.Generic;
using System.Web;

namespace NttDataWA.Utils
{
    public interface CtrlCorrespondentsList
    {
        void FetchData(DocsPaWR.SpedizioneDocumento spedizione, CorrespondentTypeEnum tipoDestinatario);
        void SaveData(DocsPaWR.SpedizioneDocumento spedizione);
        void SetStatoSpedizione(bool spedito);
        int Items { get; }
        bool AlmostOneChecked { get; }
    }

    public enum CorrespondentTypeEnum
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