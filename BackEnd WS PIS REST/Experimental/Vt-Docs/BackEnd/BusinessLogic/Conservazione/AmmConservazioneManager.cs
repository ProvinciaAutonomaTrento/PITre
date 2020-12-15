using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.amministrazione;

namespace BusinessLogic.Conservazione
{
    public class AmmConservazioneManager
    {
        /// <summary>
        /// metodo per prelevare le informazioni sulla configurazione della Stampa Registro
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static string GetStampaRegistroValues(string idAmm)
        {
            //
            // I valori ritornati dalla stringa sono concatenati nel seguente modo: 
            // valoreFrequenzaStampa_valoreDisabled
            string result = string.Empty;

            DocsPaDB.Query_DocsPAWS.Amministrazione ammConservazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            result = ammConservazione.getStampaRegistroValues(idAmm);
            
            return result;
        }

        /// <summary>
        /// Metodo per prelevare le informazioni sull'orario di stampa.
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static string GetStampaRegistroOraStampa(string idAmm)
        {
            string result = string.Empty;

            DocsPaDB.Query_DocsPAWS.Amministrazione ammConservazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            result = ammConservazione.getStampaRegistroOraStampa(idAmm);

            return result;

        }

        /// <summary>
        /// Metodo per il salvataggio delle informazioni sulla configurazione della Stampa Registro
        /// retValue = true/false
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="disabled"></param>
        /// <param name="printFreq"></param>
        /// <returns></returns>
        public static bool SaveStampaRegistroValues(string idAmm, string disabled, string printFreq, string printHour)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.Amministrazione ammConservazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            //
            // Prima di operare il salvataggio, verifico se è già presente una configurazione per quell'amministrazione
            if (string.IsNullOrEmpty(ammConservazione.getStampaRegistroValues(idAmm)))
            {
                //
                // Se non ho una precedente configurazione per l'ente, procedo con l'inserimento, altrimenti aggiorno la configurazione esistente
                result = ammConservazione.saveStampaRegistroValues(idAmm, disabled, printFreq, printHour);
            }
            else 
            {
                //
                // Aggiornamento configurazione stampa registro
                result = ammConservazione.updateStampaRegistroValues(idAmm, disabled, printFreq, printHour);
            }
            return result;
        }

        #region MEV CONS 1.5 - Gestione Alert Conservazione

        /// <summary>
        /// Verifica se il log specificato è attivo o meno per l'amministrazione selezionata
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="codice"></param>
        /// <returns></returns>
        public static bool IsLogAttivato(string idAmm, string codice)
        {            
            DocsPaDB.Query_DocsPAWS.Amministrazione ammConservazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return ammConservazione.IsLogConservazioneAttivato(idAmm, codice);
        }

        public static bool SaveGestioneAlert(string idAmm, AlertConservazione param)
        {

            bool retVal = false;

            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            //verifico se esiste già una configurazione per l'amministrazione corrente

            AlertConservazione alert = GetGestioneAlert(idAmm);
            
            if (alert == null)
            {
                //creo una nuova configurazione
                retVal = amm.SaveGestioneAlert(param);
            }
            else
            {
                //aggiorno la configurazione esistente
                retVal = amm.UpdateGestioneAlert(param);
            }

            return retVal;

        }

        public static AlertConservazione GetGestioneAlert(string idAmm)
        {
            AlertConservazione result = new AlertConservazione();

            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            result = amm.GetGestioneAlert(idAmm);



            return result;
        }

        #endregion
    }
}
