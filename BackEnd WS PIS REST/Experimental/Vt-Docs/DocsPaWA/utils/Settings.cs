using System;
using System.Configuration;
using System.Net;
using log4net;

using NS_Utils=DocsPAWA.Utils;

namespace DocsPAWA 
{
	/// <summary>
	/// Summary description for ConfigParameters.
	/// </summary>
	public class ConfigSettings
	{
		private class KeysManager
		{
            private static ILog logger = LogManager.GetLogger(typeof(KeysManager));
    
			public static string getKey(string keyName)
			{
				return getKey(keyName,null);
			}
			
			public static string getKey(string keyName,string defaultValue)
			{
				string retValue=null;
				try
				{
					if (System.Configuration.ConfigurationManager.AppSettings[keyName]!=null)
					{
						retValue=System.Configuration.ConfigurationManager.AppSettings[keyName];
					}
					else
					{
						//string l_defaultValue=null;
						//setKey(keyName,l_defaultValue);
						//retValue=l_defaultValue;
						string l_messageToLog="La chiave di configurazione '"+keyName+"' non esiste. ";
                        if (defaultValue != null)
                        {
                            l_messageToLog += "Creare la chiave ed impostarne il valore di default a '" + defaultValue + "'";
                            logger.Info(l_messageToLog);
                        }
                        retValue = defaultValue;
					}
				}
				catch(System.Exception)
				{
					string l_messageToLog="Errore nella lettura dei dati dalla chiave di configurazione '"+keyName+"'.";
					logger.Error (new System.Exception(l_messageToLog));					
				}
				
				return retValue;
			}

			public static void setKey(string keyName,string keyValue)
			{
				try
				{
					string l_keyValue=System.Configuration.ConfigurationManager.AppSettings.Get(keyName);

					if (l_keyValue==null)
					{
						//la chiave non esiste
						string l_messageToLog="La chiave di configurazione '"+keyName+"' non esiste."+"\n";
						l_messageToLog+="Creare la chiave ed impostarne il valore a '"+keyValue+"'";
						//Logger.log(l_messageToLog);
					}
					else
					{
						//la chiave esiste
						string l_messageToLog="Impostare il valore '"+keyValue+"' per la chiave di configurazione '"+keyName+"'.";
						//Logger.log(l_messageToLog);					
					}
				}
				catch(System.Exception)
				{
					string l_messageToLog="Tentativo di impostazione del valore '"+keyValue+"' per la chiave di configurazione '"+keyName+"'.";
					//Logger.logException(new System.Exception(l_messageToLog,exc));					
				}
			}
		}

		
		public enum KeysENUM
		{
			STATO_REG_APERTO,
			VISUALIZZA_OGGETTO_COMM_REF,
			DISPOSITIVO_STAMPA,
			URL_INIFILE_DISPOSITIVO_STAMPA,
            URL_INIFILE_DISPOSITIVO_STAMPA_FASC,
			CERCA_DUPLICATI_PROTOCOLLO,
            CERCA_DUPLICATI_PROTOCOLLO_2,
            DOCUMENT_AUTOPREVIEW,
			FULLSCREEN,
			ID_RUOLO_PROTOCOLLO_SEMPLICE,
			DOCUMENT_PDF_CONVERT,
			DOCUMENT_PDF_CONVERT_ENABLED, //riportato dalla 2.5 by massimo digregorio  
			VISUALIZZA_ALERT_PROTOCOLLO_CREATO, //riportato dalla 2.5 by massimo digregorio 
			VISUALIZZA_ALERT_PROFILO_CREATO, //riportato dalla 2.5 by massimo digregorio 
			VISUALIZZA_UO_ROOT,
			ID_AMMINISTRAZIONE,
			VISUALIZZA_ID_LEG,
			DEBUG,
			LABEL_ADL,
			LOG_PATH,
			LOG_LEVEL,
			VISUALIZZA_SPEDISCI_VIA_FAX,
			EXECUTIONTIMEOUT,
			PROXY,
			PROXYURL,
			PROXYCREDENTIALS,
			PROXYUSERNAME,
			PROXYPASSWORD,
			PROXYDOMAIN,
			VIEW_MITT_INTERMEDI,
			ADMINISTERED_LOGIN_MODE,
			AUTO_TO_DO_LIST,
			FULL_TEXT_SEARCH,
			ENABLE_UFFICIO_REF,
			RUBRICA_SEMPLIFICATA,
			DISABLE_LOGOUT_CLOSE_BUTTON,
			RUBRICA_V2,
			MITTENTE_DEFAULT, //aggiunto elisa per ANAS
			PROTO_SEMPLIFICATO_ENABLED,
			PROTOINGRESSO_ACQ_DOC_OBBLIGATORIA,
			FILE_ACQ_SIZE_MAX,
			ADOBE_ACROBAT_INTEGRATION,
			T_DO_SMISTA,
			VISUALIZZA_NOTE_PROTOCOLLO,
			AMM_SPOSTA_NODI_TITOLARIO,
			PROTOINGRESSO_STAMPA_ETICHETTA_AUTO,
			STAMPA_BUSTE,
			FULL_TEXT_ALERT_MESSAGE_ENABLED,
			ENABLE_LABEL_PDF,
            FULL_TEXT_MIN_TEXT_LENGTH,
            FASC_RAPIDA_REQUIRED,
            MEZZO_SPEDIZIONE,
            VISUAL_TIPOLOGIA_DOC_PROT_SEMPL,
            VISUAL_NOTE_PROT_SEMPL,
            CERCA_SOTTOFASCICOLI,
            TITLE,
            CONSERVAZIONE,
            COPYRIGHT,
            PRED_IN_TO_DO_LIST,
            LENGTH_CAMPI_PROFILATI,
            ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE,
            VISUALIZZA_NOTIFICHE_PEC,
            ENABLE_TIMESTAMP_DOC,
            POLICY_AGENT_ENABLED,
            CHIAVE_TOKEN,
            RUBRICA_VELOCE,
            MODELLO_DISPOSITIVO_STAMPA,
            AUTENTICAZIONE_WINDOWS
        }

		public static string getKey(KeysENUM key)
		{									 
			return getKey(key.ToString(),null);
		}

		public static string getKey(string key)
		{									 
			return getKey(key,null);
		}

		public static string getKey(KeysENUM key,string defaultValue)
		{									 
			return getKey(key.ToString(),defaultValue);
		}

		public static string getKey(string key,string defaultValue)
		{									 
			return KeysManager.getKey(key,defaultValue);
		}
		
		public static void setKey(string key,string newValue)
		{									 
			KeysManager.setKey(key,newValue);
		}

		public static void setKey(KeysENUM key,string newValue)
		{									 
			setKey(key.ToString(),newValue);
		}

		public static string IsEnabledFullTextSearch()
		{			
			string param=getKey(ConfigSettings.KeysENUM.FULL_TEXT_SEARCH);
			return (param!=null && param=="1"?"1":"0");
		}
	}
}
