using System;

namespace DocsPaVO.Logger
{
	/// <summary>
	/// Enum per il campo del database "VAR_COD_AZIONE", CHA_ESITO, VAR_DESC_OGGETTO.
	/// </summary>
	public class CodAzione
	{		
		public struct infoOggetto
		{
			public string Codice;
			public string Descrizione;
			public string Oggetto;
			public int Attivo;
            public string Notify;
			public infoOggetto(string codice,string descrizione,string oggetto,int attivo)
			{
				this.Codice=codice;
				this.Descrizione=descrizione;
				this.Oggetto=oggetto;
				this.Attivo=attivo;
                this.Notify = string.Empty;
			}		
		}

		public enum VAR_COD_AZIONE
		{				
			
			AGGIUNTO_DOCUMENTO_AL_FOLDER,
			AGGIUNTO_DOCUMENTO_AL_FASCICOLO,
			ALLEGATO_AGGIUNTO,
			APERTO_DETTAGLIO_DOCUMENTO,
			VISUALIZZA_FILE,
			CREATO_NUOVO_FOLDER_IN_FASCICOLO,
			DETTAGLIO_DOCUMENTO_APERTO,
			DOCUMENTO_GRIGIO_AGGIUNTO,
			DOCUMENTO_MODIFICATO,
			DOCUMENTO_PROTOCOLLATO_ESISTENTE,
			DOCUMENTO_RIMOSSO,
			DOCUMENTO_TRASMESSO,
			FASCIOLO_CREATO,		
			LOGIN,
			LOGOFF,
			MODIFICA_OGGETTO_DOCUMENTO,
			PROTOCOLLO_AGGIUNTO,
			PROTOCOLLO_ANNULLATO,
			REGISTRO_STAMPATO,
			STATO_REGISTRO_CAMBIATO,
			VERSIONE_AGGIUNTA,
		}

		public enum Esito
		{
			KO,
			OK
		}

		public enum VAR_COD_OGGETTO
		{
			UTENTE,
			DOCUMENTO,
			FASCICOLO,
			FOLDER,
			REGISTRO,
			TRASMISSIONE
		}

		public enum DISABLED_USERID
		{
			A,
			N,
			Y
		}
	}
}
