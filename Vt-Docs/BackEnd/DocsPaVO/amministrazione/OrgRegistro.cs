using System;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Definizione oggetto Registro 
	/// relativo alla funzionalità Organigramma in Amministrazione.
	/// </summary>
	public class OrgRegistro
	{
		public string IDRegistro=string.Empty;
		public string Codice=string.Empty;
		public string Descrizione=string.Empty;
		public string IDAmministrazione=string.Empty;
		public string CodiceAmministrazione=string.Empty;
		public string Associato=string.Empty;
		public OrgRegistro.MailRegistro Mail=new OrgRegistro.MailRegistro();
		public bool AperturaAutomatica=false;
		public string data_inizio=string.Empty;
		public string data_ass_visibilita=string.Empty;
        public string ID_PEOPLE_AOO = string.Empty;
        public string ID_RUOLO_AOO = string.Empty;
        public string DESCR_PEOPLE_AOO = string.Empty;
        public string DESCR_RUOLO_AOO = string.Empty;
        public string autoInterop = string.Empty;
        //se chaRF = 0 è un registro, 1 = RF
        public string chaRF = string.Empty;
        //systemId della AOO collegata all'RF, è popolata solo per gli RF
        public string idAOOCollegata = string.Empty;
        
        //rfDisabled indica se un RF è abilitato (rfDisabled = 0) o meno (rfDisabled = 1)
        public string rfDisabled = string.Empty;
         //diritto che acquisisce il ruolo responsabile del registro
         //45 lettura; 63 scrittura
        public string Diritto_Ruolo_AOO = string.Empty;
        public string Stato = string.Empty;
        public bool Sospeso = false;
        public string idRuoloResp;
        public string invioRicevutaManuale;
        public string idUtenteResp;

        //Andrea De Marco
        //Booleano per verificare se la checkbox Import Pregressi è selezionata
        public bool flag_pregresso;
        //Stringa per il campo anno dei pregressi
        public string anno_pregresso;
        //End Andrea De Marco

		/// <summary>
		/// Specifiche mail per il registro
		/// </summary>
		public class MailRegistro
		{
			public string UserID=string.Empty;
			public string Password=string.Empty;
			public string Email=string.Empty;
			public string ServerSMTP=string.Empty;
			public int PortaSMTP=0;
			public string UserSMTP=string.Empty;
			public string PasswordSMTP=string.Empty;
			public string ServerPOP=string.Empty;
			public int PortaPOP=0;
            public string POPssl=string.Empty;
            public string SMTPssl = string.Empty;
            public string SMTPsslSTA = string.Empty;
            //modifica
            public string IMAPssl = string.Empty;
            public string serverImap = string.Empty;
            public string inbox = string.Empty;
            public string mailElaborate = string.Empty;
            public int portaIMAP = 0;
            public string tipoPosta = string.Empty;
            public string mailNonElaborate = string.Empty;
            // fine modifica
            //modifica del 10/07/2009
            public string soloMailPec = string.Empty;
            //fien modifica
            //modifica 6/6/11
            public string pecTipoRicevuta = string.Empty;

            // Per gestione pendenti tramite PEC
            public string MailRicevutePendenti = string.Empty;

		}
	}
}