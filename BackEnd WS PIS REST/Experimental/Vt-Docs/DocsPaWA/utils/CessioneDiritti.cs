using System;
using System.Data;
using System.Configuration;
using System.Collections;


namespace DocsPAWA.utils
{
    /// <summary>
    /// Classe per la gestione della cessione di diritti su DOC. e FASC.
    /// </summary>
    public class CessioneDiritti
    {
        /// <summary>
        /// Verifica se l'utente è il PROPRIETARIO del DOC.
        /// </summary>
        /// <returns>True, False</returns>
        public bool UtenteProprietario(string docNumber)
        {
            return DocumentManager.dirittoProprietario(docNumber, UserManager.getInfoUtente());
        }

        /// <summary>
        /// Apre una pop-up per selezionare il ruolo a cui cedere i diritti di proprietà sull'oggetto
        /// </summary>
        public string AprePopUpSceltaNuovoProprietario(string tipo)
        {
            string javascript = string.Empty;
            javascript = "<script>";
            javascript += "apriSceltaNuovoPropietario('"+tipo+"');";            
            javascript += "</script>";
            return javascript;
        }

        /// <summary>
        /// Invia un messaggio a video che avvisa l'utente che tra i destinatari della trasmissioni
        /// non ci sono ruoli
        /// </summary>
        public string InviaMsgNoRuoli()
        {
            return "<script>alert('Per questa trasmissione è attiva l\\'opzione \\'Cedi i diritti\\'.\\n\\nPoiché il mittente è proprietario del documento, selezionare\\nalmeno un ruolo destinatario.');</script>";
        }
    }
}
