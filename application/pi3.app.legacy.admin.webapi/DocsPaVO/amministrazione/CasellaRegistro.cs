// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.amministrazione
{
    [DataContract]
    public class CasellaRegistro
    {
        [DataMember]
        public string System_id { get; set; } = string.Empty;
        [DataMember]
        public string IdRegistro { get; set; } = string.Empty;
        [DataMember]
        public string EmailRegistro { get; set; } = string.Empty;
        [DataMember]
        public string Principale { get; set; } = string.Empty;
        [DataMember]
        public string UserMail { get; set; } = string.Empty;
        [DataMember]
        public string PwdMail { get; set; } = string.Empty;
        [DataMember]
        public string ServerSMTP { get; set; } = string.Empty;
        [DataMember]
        public string SmtpSSL { get; set; } = string.Empty;
        [DataMember]
        public string PopSSL { get; set; } = string.Empty;
        [DataMember]
        public int PortaSMTP { get; set; } = 0;
        [DataMember]
        public string SmtpSta { get; set; } = string.Empty;
        [DataMember]
        public string ServerPOP { get; set; } = string.Empty;
        [DataMember]
        public int PortaPOP { get; set; } = 0;
        [DataMember]
        public string UserSMTP { get; set; } = string.Empty;
        [DataMember]
        public string PwdSMTP { get; set; } = string.Empty;
        [DataMember]
        public string IboxIMAP { get; set; } = string.Empty;
        [DataMember]
        public string ServerIMAP { get; set; } = string.Empty;
        [DataMember]
        public int PortaIMAP { get; set; } = 0;
        [DataMember]
        public string TipoConnessione { get; set; } = string.Empty;
        [DataMember]
        public string BoxMailElaborate { get; set; } = string.Empty;
        [DataMember]
        public string MailNonElaborate { get; set; } = string.Empty;
        [DataMember]
        public string ImapSSL { get; set; } = string.Empty;
        [DataMember]
        public string SoloMailPEC { get; set; } = string.Empty;
        [DataMember]
        public string RicevutaPEC { get; set; } = string.Empty;
        [DataMember]
        public string Note { get; set; } = string.Empty;

        // Per gestione pendenti tramite PEC
        [DataMember]
        public string MailRicevutePendenti { get; set; } = string.Empty;

        //Note da inserire nel testo della mail in spedizione documento
        [DataMember]
        public string MessageSendMail { get; set; } = string.Empty;
        [DataMember]
        public bool OverwriteMessageAmm { get; set; } = false;

        public string Provider = string.Empty;

        //Nuova gestione per MS Exchange
        public string TenantId = string.Empty;
        public string ClientId = string.Empty;
        public string ClientSecret = string.Empty;
    }
}
