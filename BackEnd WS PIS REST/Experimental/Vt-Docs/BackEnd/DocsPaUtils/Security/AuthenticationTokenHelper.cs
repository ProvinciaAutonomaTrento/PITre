using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaUtils.Security
{
    /// <summary>
    /// Classe per la gestione del token di autenticazione per il SingleSignOn
    /// </summary>
    public sealed class SSOAuthTokenHelper
    {
        #region Private Members

        /// <summary>
        /// Prefisso del token di autenticazione
        /// </summary>
        private const string TOKEN_PREFIX = "SSO=";

        /// <summary>
        /// Formato valore encrypted
        /// </summary>
        /// <remarks>
        /// {0} = UserId
        /// {1} = SessionId
        /// {2} = Data erogazione token
        /// </remarks>
        private const string ENCRYPTED_VALUE_FORMAT = "UID={0};SESSIONID={1};DATETIME={2}";
        private const string UID = "UID";
        private const string SESSIONID = "SESSIONID";
        private const string DATETIME = "DATETIME";

        /// <summary>
        /// 
        /// </summary>
        private SSOAuthTokenHelper()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetSessionId()
        {
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
                return System.Web.HttpContext.Current.Session.SessionID;
            else
                return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="decryptedValue"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetTokenKeyValuePairs(string decryptedValue)
        {
            if (!decryptedValue.Contains(UID) || !decryptedValue.Contains(SESSIONID) || !decryptedValue.Contains(DATETIME))
            {
                throw new ApplicationException("Formato token di autenticazione non valido: parametri obbligatori non presenti");
            }
            else
            {
                string[] values = decryptedValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, string> pairs = new Dictionary<string, string>();

                foreach (string pair in values)
                {
                    string[] keyValuePair = pair.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                    if (keyValuePair.Length == 2)
                        pairs[keyValuePair[0]] = keyValuePair[1];
                    else
                        throw new ApplicationException("Formato token di autenticazione non valido");
                }

                return pairs;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsAuthToken(string token)
        {
            return (token.IndexOf(TOKEN_PREFIX) > -1);
        }

        /// <summary>
        /// Autenticazione Sistemi Esterni R.2
        /// </summary>
        /// <param name="token"></param>
        /// <param name="username"></param>
        /// <param name="tknDuration"></param>
        /// <returns></returns>
        public static string CtrlAuthToken(string token, string username, string tknDuration)
        {
            string retval = "OK";
            CryptoString crypto = new CryptoString(username.ToUpper());
            string encodedValue = token.Substring(TOKEN_PREFIX.Length);
            try
            {
                string decodedValue = crypto.Decrypt(encodedValue);
                string[] valori = decodedValue.Split(';');
                if (valori[0].Substring(4).Equals(username.ToUpper()))
                {
                    string tct = valori[2].Replace("\0", "").Substring(DATETIME.Length + 1);
                    DateTime TknCreationTime = DateTime.Parse(tct);
                    System.TimeSpan timespan = DateTime.Now - TknCreationTime;

                    if (timespan.TotalMinutes > Int32.Parse(tknDuration))
                        retval = "TKN_EXPIRED";

                }
                else
                {
                    retval = "WRONG_USER";
                }
            }
            catch (Exception e)
            {
                retval = "WRONG_USER";
            }
            return retval;
        }

        /// <summary>
        /// Generazione del token di autenticazione per l'utente fornito
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string Generate(string userId)
        {
            try
            {
                CryptoString crypto = new CryptoString(userId);
                string encodedValue = crypto.Encrypt(string.Format(ENCRYPTED_VALUE_FORMAT, userId, GetSessionId(), DateTime.Now));

                return string.Format("{0}{1}", TOKEN_PREFIX, encodedValue);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Si è verificato un errore nella cifratura del token di autenticazione", ex);
            }
        }

        /// <summary>
        /// Ripristino del token di autenticazione per l'utente fornito
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string Restore(string userId, string token)
        {
            if (IsAuthToken(token))
            {
                CryptoString crypto = new CryptoString(userId);
                string decryptedValue = string.Empty;

                try
                {
                    decryptedValue = crypto.Decrypt(token.Replace(TOKEN_PREFIX, string.Empty));
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Si è verificato un errore nella decifratura del token di autenticazione", ex);
                }

                Dictionary<string, string> keyValuePairs = GetTokenKeyValuePairs(decryptedValue);

                if (string.Compare(userId, keyValuePairs[UID], true) == 0)
                {
                    string sessionId = keyValuePairs[SESSIONID];

                    // GESTIONE SCADENZA TOKEN: PROBABILE EVOLUZIONE, DA ATTIVARE
                    //DateTime date;
                    //if (DateTime.TryParse(values[2], out date))
                    //{
                    //    TimeSpan ts = DateTime.Now.Subtract(date);
                    //    if (ts.Minutes > 10)
                    //        throw new ApplicationException("Il token di autenticazione risulta scaduto");
                    //}

                    return sessionId;
                }
                else
                    throw new ApplicationException(string.Format("Il token di autenticazione non risulta associato all'utente {0}", userId));
            }
            else
                throw new ApplicationException("Formato token di autenticazione non valido");
        }

        #endregion
    }
}
