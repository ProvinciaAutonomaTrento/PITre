using System;

namespace StampaRegistri.Oggetti
{
	public class Costanti
	{
		public enum ListaParametriConfig
		{
			Docspa_Versione,
			Docspa_UserName,
			Docspa_PWD,
			Docspa_LoginForzata,
			Docspa_IDAmm,
			Docspa_Ruolo_IDCorr,
			HistoryLog_PathFolder,
			HistoryLog_FileNamePrefix,
			Log_PathFolder,
			Log_FileName,
			Log_LevelTrace,
			Log_Device,
			Docspa_IDRegistro,
			Work_ForzaChiusuraReg,
			Work_ApriRegDopoProcesso,
			Work_ConfermaChiusuraDopoProcesso,
			Docspa_TimeoutRichiesta_DocsPaWebService_InMinuti
		}

		public enum LivelliLog
		{
			NonTracciare,
			Errore,
			FlussoOperativo,
			Debug
		}

		public enum DispositiviDiLog
		{
			EventViewer = 3,
			File = 2,
			Console = 1,
			Nessuno = 0
		}

		public enum TipoMessaggio
		{
			ERRORE = -99,
			INFORMAZIONE = 1,
			DEBUG
		}

		public enum Errori
		{
			NessunErrore,
			Config_PathHistoryLog_NonValido = 11,
			Config_IDAmm_NonValido,
			Config_UserName_NonValido,
			Config_Password_NonValido,
			Config_IdRuolo_NonValido,
			Config_VersioneNonValida,
			Config_DispositivoLogNonValido,
			Config_FlagForzaChiusuraRegNonValido,
			Config_FlagAperturaRegDopoProcNonValido,
			Config_Registro_Vuoto,
			Config_TimeoutRichiestaWS_NonValido,
			Config_ConfermaChiusuraDopoProcesso,
			Docspa_WSNonRaggiungibile = 30,
			Docspa_LoginNonRiuscita,
			Docspa_CaricamentoUtenteFallito,
			Docspa_LogoutNonRiuscita,
			Docspa_RuoloUtenteNonValido,
			Docspa_UtenteNoHaRuoliAssociati,
			Docspa_RuoloNoHaRegistriAssociati,
			Docspa_Registro_NonValido,
			Docspa_CaricamentoRegistroFallito,
			Docspa_StatoRegistroNonValido,
			Docspa_CambioStatoRegistroFallito,
			Docspa_StampaRegistroNonRiuscita
		}

		public enum Informazioni
		{
			Work_CaricamentoAppConfigInCorso = 90,
			Work_EsecuzioneStoricLogInCorso,
			Work_StampaRegInCorso,
			Config_ParametriNonValidi = 100,
			Work_StoricLog_Fallito,
			Docspa_ConnessioneNonRiuscita,
			Docspa_ConnessioneRiuscita,
			Work_InizioProcesso,
			Work_FineProcesso,
			Operazione_Terminata,
			Docspa_LoginRiuscita,
			Docspa_LoginNonRiuscita,
			Docspa_CaricamentoRegistroNonRiuscito,
			Docspa_CaricamentoRegistroRiuscito,
			Docspa_ChiusuraRegistroNonRiuscita,
			Docspa_ChiusuraRegistroRiuscita,
			Docspa_CaricamentoStatoRegistroNonRiuscito,
			Docspa_CaricamentoStatoRegistroRiuscito,
			Docspa_AperturaRegistroNonRiuscita,
			Docspa_AperturaRegistroRiuscita,
			Docspa_ChiusuraRegistroNonRichiesta,
			Docspa_AperturaRegistroNonRichiesta,
			Docspa_StampaRegistroNonRiuscita,
			Docspa_StampaRegistroRiuscita,
			Docspa_RisultatoStampa
		}

		public enum Debug
		{
			DBG_Inizio = 207,
			DBG_Fine,
			DBG_Step
		}

		public enum MessaggiDaVisualizzare
		{
			Nessun_errore__,
			Storicizzazione_del_log_fallita__Verificare_le_proprietà_del_folder_impostato_nel_file_di_configurazione__ = 11,
			L____amministrazione_di_docspa_non_è_corretta__Verificare_nel_file_di_configurazione__,
			Lo_UserName_di_docspa_non_è_corretto__Verificare_nel_file_di_configurazione__,
			Non_è_stata_inserita_alcuna_password_per_l____utente_scelto__Verificare_nel_file_di_configurazione__,
			Non_è_stato_scelto_alcun_ruolo__Verrà_utilizzato_quello_di_riferimento__Verificare_nel_file_di_configurazione__,
			Lo_versione_di_docspa_non_è_corretta__Verificare_nel_file_di_configurazione__,
			Il_dispositivo_di_Log_non_è_corretto__Verificare_nel_file_di_configurazione__,
			Il_Flag_ForzaChiusuraReg_non_è_corretto__Verificare_nel_file_di_configurazione__,
			Il_Flag_AperturaReg_non_è_corretto__Verificare_nel_file_di_configurazione__,
			Non_è_stato_scelto_alcun_Registro__Verrà_utilizzato_quello_di_riferimento_del_Ruolo__Verificare_nel_file_di_configurazione__,
			Il_timeout_di_richiesta_dei_WS__scelto_non_è_corretto__Verificare_nel_file_di_configurazione__,
			La_richiesta_di_conferma_chiusura_applicazione_dopo_processo_non_è_corretto__Verificare_nel_file_di_configurazione__,
			Docspa_scelto_non_è_raggiungibile__verificare_l____url_relativo__ = 30,
			Login_in_Docspa_fallita__,
			Caricamento_Utente_fallito__,
			Logout_in_Docspa_fallita__,
			Ruolo_utente_non_valido__,
			L____Utente_non_ha_ruoli_associati__,
			Il_Ruolo_non_ha_registri_associati__,
			Il_Registro_non_è_corretto__Verificare_nel_file_di_configurazione__,
			Caricamento_registro_fallito__,
			Stato_registro_non_valido___,
			Cambiamento_dello_stato_del_registro_fallito,
			Stampa_del_registro_fallita__,
			Caricamento_parametri_di_configurazione_in_atto______ = 90,
			Storicizzazione_dei_log_in_atto______,
			Stampa_del_registro_in_corso______,
			Parametri_di_configurazione_non_validi__Impossibile_continuare__Consultare_il_log_per_dettagli__ = 100,
			Storicizzazione_del_log_fallita__,
			Docspa_non_è_raggiugibile__Consultare_il_log_per_dettagli__,
			Aperta_connessione_a_Docspa_,
			Inizio_Processo_di_stampa_registro__,
			Fine_Processo_di_stampa_registro__,
			Premere_INVIO_per_terminare_l____applicazione__,
			Autenticazione_su_Docspa_eseguita_correttamente__,
			Autenticazione_su_Docspa_Docspa_fallita,
			Caricamento_registro_fallito__Impossibile_continuare__Consultare_il_log_per_dettagli__,
			Caricamento_del_registro_eseguito__,
			Chiusura_registro_fallita__Impossibile_continuare__Consultare_il_log_per_dettagli__,
			Chiusura_del_registro_eseguita__,
			Caricamento_Stato_Registro_fallito__Impossibile_continuare__Consultare_il_log_per_dettagli__,
			Caricamento_Stato_Registro_eseguito__,
			Apertura_registro_fallita__Consultare_il_log_per_dettagli__,
			Apertura_del_registro_eseguita__,
			Chiusura_automatica_registro_non_richiesta__,
			Apertura_automatica_registro_non_richiesta__,
			Stampa_del_registro_fallita__Consultare_il_log_per_dettagli__,
			Stampa_del_registro_eseguita__,
			Risultato_della_Stampa___,
			Inizio__ = 207,
			Fine__,
			Operazione__
		}
	}
}
