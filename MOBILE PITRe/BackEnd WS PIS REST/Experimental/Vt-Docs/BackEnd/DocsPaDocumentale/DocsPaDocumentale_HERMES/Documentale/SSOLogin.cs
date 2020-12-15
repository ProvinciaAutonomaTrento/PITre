using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace DocsPaDocumentale_HERMES.Documentale
{
    public class SSOLogin
    {
        private static ILog logger = LogManager.GetLogger(typeof(SSOLogin));
        #region autenticazione con TOKEN
        /// <summary>
        /// Autenticazione con TOKEN. Metodo Valido solo per ETDOC
        /// </summary>
        /// <param name="token">user_id&codice&cod_ruolo&cod_reg&impronta</param>
        /// <param name="message">messaggio di ritorno se qualcosa và male</param>
        /// <returns>true o false</returns>
        public static bool loginWithToken(string token, out string message)
        {
            bool result = true;
            string codice = getCodice();
            if (codice == null || codice.Equals(""))
            {
                message = "codice non valido";
                return false;
            }
            try
            {
                message = "";
                char[] sep = { '&' };
                string[] dati = token.Split(sep);
                if (dati.Length != 4)
                {
                    message = "Formato del token non riconosciuto";
                    logger.Debug("Errore di autenticazione con token (token: " + token + ")");
                    throw new System.Exception(message);
                }
                if (dati[3] == null || dati[3].Trim().Equals(""))
                {
                    message = "Formato del token non riconosciuto. Impronta non valida";
                    logger.Debug("Errore di autenticazione con token (token: " + token + ")");
                    throw new System.Exception(message);
                }
                //                 user_id + & + codice + & + cod_ruolo + & + cod_reg 
                string datiInput = dati[0] + "&" + codice + "&" + dati[1] + "&" + dati[2];
                byte[] bt_datiInput = ASCIIEncoding.ASCII.GetBytes(datiInput);
                string tokenOut = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(bt_datiInput);
                string tokenIn = dati[3].Replace("-", ""); //??? per uniformarci a come noi calcoliamo l'impronta
                if (tokenOut.Equals(tokenIn))
                    return true;
                else
                {
                    message = "L'impronta non corrisponde.";
                    return false;
                }
                return result;
            }
            catch (Exception e)
            {
                message = "Errore: " + e.Message.ToString();
                return false;
            }
            return result;

        }

        internal static string getCodice()
        {
            string codice;
            codice = System.Configuration.ConfigurationSettings.AppSettings["CHIAVE_TOKEN"];
            return codice;
        }

        #endregion

    }
}
