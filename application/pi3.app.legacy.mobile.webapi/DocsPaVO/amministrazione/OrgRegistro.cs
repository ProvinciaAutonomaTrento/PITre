// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Runtime.Serialization;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Definizione oggetto Registro 
    /// relativo alla funzionalit� Organigramma in Amministrazione.
    /// </summary>
    [DataContract]
	public class OrgRegistro
	{
        [DataMember]
		public string IDRegistro { get; set; } = string.Empty;
        [DataMember]
        public string Codice { get; set; } = string.Empty;
        [DataMember]
        public string Descrizione { get; set; } = string.Empty;
        [DataMember]
        public string IDAmministrazione { get; set; } = string.Empty;
        [DataMember]
        public string CodiceAmministrazione { get; set; } = string.Empty;
        [DataMember]
        public string Associato { get; set; } = string.Empty;
        [DataMember]
        public OrgRegistro.MailRegistro Mail { get; set; } = new OrgRegistro.MailRegistro();
        [DataMember]
        public bool AperturaAutomatica { get; set; } = false;
        [DataMember]
        public string data_inizio { get; set; } = string.Empty;
        [DataMember]
        public string data_ass_visibilita { get; set; } = string.Empty;
        [DataMember]
        public string ID_PEOPLE_AOO { get; set; } = string.Empty;
        [DataMember]
        public string ID_RUOLO_AOO { get; set; } = string.Empty;
        [DataMember]
        public string DESCR_PEOPLE_AOO { get; set; } = string.Empty;
        [DataMember]
        public string DESCR_RUOLO_AOO { get; set; } = string.Empty;
        [DataMember]
        public string autoInterop { get; set; } = string.Empty;
        //se chaRF = 0 � un registro, 1 = RF
        [DataMember]
        public string chaRF { get; set; } = string.Empty;
        //systemId della AOO collegata all'RF, � popolata solo per gli RF
        [DataMember]
        public string idAOOCollegata { get; set; } = string.Empty;

        //rfDisabled indica se un RF � abilitato (rfDisabled = 0) o meno (rfDisabled = 1)
        [DataMember]
        public string rfDisabled { get; set; } = string.Empty;
        //diritto che acquisisce il ruolo responsabile del registro
        //45 lettura; 63 scrittura
        [DataMember]
        public string Diritto_Ruolo_AOO { get; set; } = string.Empty;
        [DataMember]
        public string Stato { get; set; } = string.Empty;
        [DataMember]
        public bool Sospeso { get; set; } = false;
        [DataMember]
        public string idRuoloResp { get; set; }
        [DataMember]
        public string invioRicevutaManuale { get; set; }
        [DataMember]
        public string idUtenteResp { get; set; }

        //Andrea De Marco
        //Booleano per verificare se la checkbox Import Pregressi � selezionata
        [DataMember]
        public bool flag_pregresso { get; set; }
        //Stringa per il campo anno dei pregressi
        [DataMember]
        public string anno_pregresso { get; set; }
        //End Andrea De Marco

        [DataMember]
        public string codiceIpa { get; set; } = string.Empty;

        /// <summary>
        /// Specifiche mail per il registro
        /// </summary>
        /// 
        [DataContract]
        public class MailRegistro
		{
            [DataMember]
			public string UserID { get; set; } = string.Empty;
            [DataMember]
            public string Password { get; set; } = string.Empty;
            [DataMember]
            public string Email { get; set; } = string.Empty;
            [DataMember]
            public string ServerSMTP { get; set; } = string.Empty;
            [DataMember]
            public int PortaSMTP { get; set; } = 0;
            [DataMember]
            public string UserSMTP { get; set; } = string.Empty;
            [DataMember]
            public string PasswordSMTP { get; set; } = string.Empty;
            [DataMember]
            public string ServerPOP { get; set; } = string.Empty;
            [DataMember]
            public int PortaPOP { get; set; } = 0;
            [DataMember]
            public string POPssl { get; set; } = string.Empty;
            [DataMember]
            public string SMTPssl { get; set; } = string.Empty;
            [DataMember]
            public string SMTPsslSTA { get; set; } = string.Empty;
            
            //modifica
            [DataMember]
            public string IMAPssl { get; set; } = string.Empty;
            [DataMember]
            public string serverImap { get; set; } = string.Empty;
            [DataMember]
            public string inbox { get; set; } = string.Empty;
            [DataMember]
            public string mailElaborate { get; set; } = string.Empty;
            [DataMember]
            public int portaIMAP { get; set; } = 0;
            [DataMember]
            public string tipoPosta { get; set; } = string.Empty;
            [DataMember]
            public string mailNonElaborate { get; set; } = string.Empty;
            // fine modifica
            //modifica del 10/07/2009
            [DataMember]
            public string soloMailPec { get; set; } = string.Empty;
            //fien modifica
            //modifica 6/6/11
            [DataMember]
            public string pecTipoRicevuta { get; set; } = string.Empty;

            // Per gestione pendenti tramite PEC
            [DataMember]
            public string MailRicevutePendenti { get; set; } = string.Empty;

            //Nuova gestione MS Exchange
            public string Provider { get; set; } = string.Empty;
            public string TenantId { get; set; } = string.Empty;
            public string ClientId { get; set; } = string.Empty;
            public string ClientSecret { get; set; } = string.Empty;
        }
    }
}