using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using DocsPaVO.utente;
using DocsPaVO.RubricaComune;

namespace BusinessLogic.RubricaComune
{
    /// <summary>
    /// Classe per il reperimento delle configurazioni DocsPa
    /// per l'utilizzo del sistema Rubrica Comune
    /// </summary>
    public sealed class Configurazioni
    {
        #region Public members

        /// <summary>
        /// Reperimento dei dati di configurazione per l'utente
        /// </summary>
        /// <param name="infoUtente">
        /// </param>
        /// <returns></returns>
        public static ConfigurazioniRubricaComune GetConfigurazioni(InfoUtente infoUtente)
        {
            ConfigurazioniRubricaComune config = new ConfigurazioniRubricaComune();

            StringDictionary items = ParseConnectionString(GetValue<string>(RUBRICA_COMUNE_CONNECTION_STRING, string.Empty));

            // Reperimento root path dei servizi rubrica comune 
            config.ServiceRoot = items["serviceRoot"];

            // Reperimento credenziali per l'accesso ai servizi rubrica comune
            config.SuperUserId = items["userId"];
            config.SuperUserPwd = items["password"];

            // Verifica se la gestione è abilitata
            config.GestioneAbilitata = (!string.IsNullOrEmpty(config.ServiceRoot) &&
                                         !string.IsNullOrEmpty(config.SuperUserId));

            // Verifica se la gestione della rubrica comune è abilitata o meno da tool di amministrazione
            config.GestioneAmministrazioneAbilitata = (config.GestioneAbilitata && GetValue<bool>(RUBRICA_COMUNE_AMMINISTRAZIONE, false));

            return config;
        }

        #endregion

        #region Private members

        /// <summary>
        /// Rappresenta la stringa di connessione per l'accesso ai servizi della rubrica comune
        /// </summary>
        private const string RUBRICA_COMUNE_CONNECTION_STRING = "RubricaComuneConnectionString";

        /// <summary>
        /// 
        /// </summary>
        private const string RUBRICA_COMUNE_AMMINISTRAZIONE = "RubricaComuneAmministrazione";

        /// <summary>
        /// Parsing della stringa di connessione verso i servizi della rubrica comune
        /// <remarks>
        /// La stringa di connessione è composta con il formato seguente:
        /// "serviceRoot=http://localhost/Rubrica/; userId=sa; password="
        /// </remarks>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static StringDictionary ParseConnectionString(string connectionString)
        {
            StringDictionary retValue = new StringDictionary();

            foreach (string item in connectionString.Split(';'))
            {
                string[] keyValuePair = item.Split('=');
                
                if (keyValuePair.Length == 2)
                    retValue.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento del valore di una configurazione
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static T GetValue<T>(string configKey, T defaultValue)
        {   
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[configKey]))
                return (T) Convert.ChangeType(ConfigurationManager.AppSettings[configKey], typeof(T));
            else
                return defaultValue;
        }

        #endregion
    }
}
