using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.Smistamento
{
	/// <summary>
	/// Definizione oggetto Utente 
	/// relativo alla funzionalità di smistamento documenti.
	/// </summary>
	public class UtenteSmistamento
	{
		public string ID=string.Empty;
		public string IDCorrGlobali=string.Empty;
		public string UserID=string.Empty;
		public string Denominazione=string.Empty;
		// tipologia di notifica smistamento (default=NoMail)
		public TipoNotificaSmistamentoEnum TipoNotificaSmistamento=TipoNotificaSmistamentoEnum.NoMail;
		public string EMail=string.Empty;
		public bool FlagCompetenza=false;
		public bool FlagConoscenza=false;
        //public string NoteIndividuali = string.Empty;
        public DocsPaVO.Smistamento.datiAggiuntiviSmistamento datiAggiuntiviSmistamento=new datiAggiuntiviSmistamento();
        public string ragioneTrasmRapida = string.Empty;
	}

	/// <summary>
	/// Enumeration per definire le tipologie di
	/// notifiche di smistamento documenti all'utente
	/// </summary>
	[XmlType("TipoNotificaSmistamento")]
	public enum TipoNotificaSmistamentoEnum
	{
		NoMail,
		Mail,		
		MailConAllegati,
        SoloAllegati

	}
}
