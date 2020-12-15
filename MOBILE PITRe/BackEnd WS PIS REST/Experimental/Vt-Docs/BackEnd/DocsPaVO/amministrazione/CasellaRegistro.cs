using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.amministrazione
{
    public class CasellaRegistro
    {
        public string System_id = string.Empty;
        public string IdRegistro = string.Empty;
        public string EmailRegistro = string.Empty;
        public string Principale = string.Empty;
        public string UserMail = string.Empty;
        public string PwdMail = string.Empty;
        public string ServerSMTP = string.Empty;
        public string SmtpSSL = string.Empty;
        public string PopSSL = string.Empty;
        public int PortaSMTP = 0;
        public string SmtpSta = string.Empty;
        public string ServerPOP = string.Empty;
        public int PortaPOP = 0;
        public string UserSMTP = string.Empty;
        public string PwdSMTP = string.Empty;
        public string IboxIMAP = string.Empty;
        public string ServerIMAP = string.Empty;
        public int PortaIMAP = 0;
        public string TipoConnessione = string.Empty;
        public string BoxMailElaborate = string.Empty;
        public string MailNonElaborate = string.Empty;
        public string ImapSSL = string.Empty;
        public string SoloMailPEC = string.Empty;
        public string RicevutaPEC = string.Empty;
        public string Note = string.Empty;

        // Per gestione pendenti tramite PEC
        public string MailRicevutePendenti = string.Empty;
    }
}
