// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.RubricaComune;
using DocsPaVO.utente;
using System.Collections.Specialized;

namespace BusinessLogic.RubricaComune
{
    public class Configurazioni
    {
        /// <summary>
        /// Rappresenta la stringa di connessione per l'accesso ai servizi della rubrica comune
        /// </summary>
        private const string RUBRICA_COMUNE_CONNECTION_STRING = "RubricaComuneConnectionString";

        /// <summary>
        ///
        /// </summary>
        private const string RUBRICA_COMUNE_AMMINISTRAZIONE = "RubricaComuneAmministrazione";

        public static ConfigurazioniRubricaComune GetConfigurazioni(InfoUtente infoUtente)
        {
            ConfigurazioniRubricaComune config = new ConfigurazioniRubricaComune();

            //StringDictionary items = ParseConnectionString(GetValue<string>(RUBRICA_COMUNE_CONNECTION_STRING, string.Empty));
            StringDictionary items = ParseConnectionString(DocsPaVO.Settings.AppSettings.Instance.RubricaComuneConnectionString);

            // Reperimento root path dei servizi rubrica comune
            config.ServiceRoot = items["serviceRoot"];

            // Reperimento credenziali per l'accesso ai servizi rubrica comune
            config.SuperUserId = items["userId"];
            config.SuperUserPwd = items["password"];

            // Verifica se la gestione è abilitata
            config.GestioneAbilitata = (!string.IsNullOrEmpty(config.ServiceRoot) &&
                                         !string.IsNullOrEmpty(config.SuperUserId));

            // Verifica se la gestione della rubrica comune è abilitata o meno da tool di amministrazione
            if (Boolean.TryParse(DocsPaVO.Settings.AppSettings.Instance.RubricaComuneAmministrazione, out var abilitazione))
                config.GestioneAmministrazioneAbilitata = config.GestioneAbilitata && abilitazione;
            //config.GestioneAmministrazioneAbilitata = (config.GestioneAbilitata && GetValue<bool>(RUBRICA_COMUNE_AMMINISTRAZIONE, false));

            return config;
        }

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

        //private static T GetValue<T>(string configKey, T defaultValue)
        //{
        //    if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[configKey]))
        //        return (T)Convert.ChangeType(System.Configuration.ConfigurationManager.AppSettings[configKey], typeof(T));
        //    else
        //        return defaultValue;
        //}

    }
}
