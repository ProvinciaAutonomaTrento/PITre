using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DocsPaDB;
using DocsPaUtils.Data;
using Security.Core.CryptingAlgorithm;
using Security.Core.Exceptions;
using Security.Core.Helper;

namespace Security.Core.AuthenticationTokenManager.SSO
{
    /// <summary>
    /// Classe per la gestione del token di autenticazione per il SingleSignOn
    /// </summary>
    public sealed class SSOAuthTokenManager : AuthTokenManager
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
        /// {3} = Tipo di token
        /// {4} = Data di scadenza del token.
        /// </remarks>
        private const string ENCRYPTED_VALUE_FORMAT = "UID={0};SESSIONID={1};DATETIME={2};TOKENTYPE={3};TOKENEXPIRATIONDATE={4};";
        private const string UID = "UID";
        private const string SESSIONID = "SESSIONID";
        private const string DATETIME = "DATETIME";
        private const string TOKENEXPIRATIONDATE = "TOKENEXPIRATIONDATE";
        private const string TOKENTYPE = "TOKENTYPE";

        // Nomi delle store per salvataggio e lettura dei dati sul token
        private const string SAVE_TOKEN = "SP_SAVE_TOKEN";
        private const string LOAD_TOKEN = "SP_LOAD_TOKEN";
        private const string REMOVE_TOKEN = "SP_REMOVE_TOKEN";

        /// <summary>
        /// Funzione per l'interpretazione del numero di caratteri di cui deve essere composta la chiave
        /// </summary>
        /// <param name="keyLength">Lunghezza della chiave da interpretare</param>
        /// <returns>Numero di caratteri decodificato</returns>
        private int DecodeKeyLength(AuthTokenManager.KeyLengthEnum keyLength)
        {
            int length;
            switch (keyLength)
            {
                case AuthTokenManager.KeyLengthEnum.Sixteen:
                    length = 16;
                    break;
                case AuthTokenManager.KeyLengthEnum.ThirtyTwo:
                    length = 32;
                    break;
                default:
                    length = 16;
                    break;
            }

            return length;
 
        }

        /// <summary>
        /// Funzione per il reperimento dell'ID di sessione
        /// </summary>
        /// <returns>Id di sessione</returns>
        private string GetSessionId()
        {
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
                return System.Web.HttpContext.Current.Session.SessionID;
            else
                return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Questa funzione interpreta il token descriptato
        /// </summary>
        /// <param name="decryptedValue">Tocken decriptato</param>
        /// <returns>Dizionario chiave / valore con i parametri del token</returns>
        private Dictionary<string, string> GetTokenKeyValuePairs(string decryptedValue)
        {
            if (!decryptedValue.Contains(UID) || !decryptedValue.Contains(SESSIONID) || !decryptedValue.Contains(DATETIME) || !decryptedValue.Contains(TOKENEXPIRATIONDATE))
                throw new AuthenticationTokenNotValidException();
            else
            {
                string[] values = decryptedValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, string> pairs = new Dictionary<string, string>();

                foreach (string pair in values)
                {
                    string[] keyValuePair = pair.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                    if (keyValuePair.Length == 2)
                        pairs[keyValuePair[0]] = keyValuePair[1];
                    
                }

                return pairs;
            }
        }

        /// <summary>
        /// Funzione per la decodifica del tipo di token
        /// </summary>
        /// <param name="tokenType">Tipo token da decodificare</param>
        /// <returns>Tipo token decodificato</returns>
        private AuthTokenManager.TokenTypeEnum DecodeTokenType(String tokenType)
        {
            AuthTokenManager.TokenTypeEnum token = AuthTokenManager.TokenTypeEnum.OneTime;
            if (tokenType == AuthTokenManager.TokenTypeEnum.Temporal.ToString())
                token = AuthTokenManager.TokenTypeEnum.Temporal;
            else
                if (tokenType == AuthTokenManager.TokenTypeEnum.Explicit.ToString())
                    token = AuthTokenManager.TokenTypeEnum.Explicit;

            return token;
        }

        /// <summary>
        /// Funzione per la creazione di un parametro da passare ad una store
        /// </summary>
        /// <param name="parameterName">Nome del parametro</param>
        /// <param name="initializationValue">Valore da utilizzare per inizializzare il parametro</param>
        /// <param name="parameterDirection">Direzione del parametro</param>
        /// <param name="parameterLength">Lunghezza del campo</param>
        /// <param name="parameterType">Tipo di parametro</param>
        /// <returns>Parametro da inviare alla store</returns>
        private ParameterSP CreateParameter(String parameterName, Object initializationValue, DirectionParameter parameterDirection, int parameterLength, DbType parameterType)
        {
            // Creazione e restituzione del parametro
            ParameterSP parameter;

            parameter = new ParameterSP(parameterName, initializationValue, parameterDirection);
            parameter.Size = parameterLength;
            parameter.Tipo = parameterType;
            
            return parameter;
        }

        /// <summary>
        /// Funzione per il recupero del valore restituito da una store
        /// </summary>
        /// <param name="parameterName">Nome del parametro da recuperare</param>
        /// <param name="parameters">Lista dei parametri</param>
        /// <returns>Valore estratto</returns>
        private Object GetValueReturnedFromStore(String parameterName, ParameterSP[] parameters)
        {
            return parameters.Where(e => e.Nome == parameterName).FirstOrDefault().Valore;
        }


        /// <summary>
        /// Funzione per il salvataggio delle informazioni sul token
        /// </summary>
        /// <param name="key">Chiave utilizzata per criptare il token</param>
        /// <param name="token">Token criptato</param>
        /// <param name="initializationVector">Vettore di inizializzazione utilizzato per criptare il token</param>
        /// <param name="userId">User id dell'utente che ha richiesto il token</param>
        private void SaveToken(String key, String token, String initializationVector, String userId)
        {
            ArrayList parameters = new ArrayList();
            Object errorDescription = String.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                parameters.Add(this.CreateParameter("p_key", key, DirectionParameter.ParamInput, 2000, DbType.String));
                parameters.Add(this.CreateParameter("p_token", token, DirectionParameter.ParamInput, 2000, DbType.String));
                parameters.Add(this.CreateParameter("p_initialization_vector", initializationVector, DirectionParameter.ParamInput, 2000, DbType.String));
                parameters.Add(this.CreateParameter("p_error_description", String.Empty, DirectionParameter.ParamOutput, 2000, DbType.String));
                parameters.Add(this.CreateParameter("p_user_id", userId, DirectionParameter.ParamInput, 2000, DbType.String));

                try
                {
                    int result = dbProvider.ExecuteStoredProcedure(SAVE_TOKEN, parameters, null);

                    // Prelevamento messaggio restituito dalla store
                    errorDescription = this.GetValueReturnedFromStore("p_error_description", (ParameterSP[])parameters.ToArray(typeof(ParameterSP)));

                }
                catch (Exception e)
                {
                    throw new SaveException(e);
                }

                // Se errorDescription è una stringa valida e non vuota viene lanciata un'eccezione
                if (errorDescription != null && !String.IsNullOrEmpty(errorDescription.ToString()))
                    this.ThrowSaveException(errorDescription.ToString(), userId);

            }

        }

        /// <summary>
        /// Funzione utilizzata per sollevare un'eccezione adeguata al tipo di errore che si è verificato
        /// durante il salvataggio del token su DB
        /// </summary>
        /// <param name="message">Messaggio restituito dal token</param>
        /// <param name="userId">User id dell'utente per cui è stata richiesta la generazione di un token</param>
        private void ThrowSaveException(String message, String userId)
        {
            switch (message)
            {
                case "UnknowUserId":
                    throw new UnknowUserException(userId);
                case "UserAlreadyHaveToken":
                    throw new TokenAlreadyExistException(userId);
                default:
                    throw new GenericException(message);
            }
        }

        /// <summary>
        /// Funzione per la rimozione del record con le informazioni sul token
        /// </summary>
        /// <param name="encryptedToken">Token da eliminare</param>
        private void RemoveTokenFromDB(String encryptedToken)
        {
            ArrayList parameters = new ArrayList();

            using (DBProvider dbProvider = new DBProvider())
            {
                parameters.Add(this.CreateParameter("p_token", encryptedToken, DirectionParameter.ParamInput, 2000, DbType.String));

                try
                {
                    int result = dbProvider.ExecuteStoreProcedure(REMOVE_TOKEN, parameters);

                }
                catch (Exception e)
                {
                    throw new RemoveException(e);
                }

            }
            
        }

        /// <summary>
        /// Funzione per il recupero delle informazioni relative ad un token
        /// </summary>
        /// <param name="encryptedToken">Tocken criptato</param>
        /// <returns>Oggetto per la decodifica del token</returns>
        private CryptoString LoadToken(String encryptedToken)
        {
            // Valori restituiti dalla store procedure
            string key = String.Empty, initializationVector = String.Empty;
            DateTime actualDate = DateTime.Now;

            // Oggetto utilizzato per inizializzare un parametro da passare alla store
            ParameterSP parameter;

            // Inizializzazione del provider e della lista dei parametri
            DBProvider provider = new DBProvider();
            ArrayList parameters = new ArrayList();

            // Creazione lista parametri
            parameter = this.CreateParameter("p_token", encryptedToken, DirectionParameter.ParamInput, 2000, DbType.String);
            parameters.Add(parameter);
            parameter = this.CreateParameter("p_key", String.Empty, DirectionParameter.ParamOutput, 2000, DbType.String);
            parameters.Add(parameter);
            parameter = this.CreateParameter("p_initialization_vector", String.Empty, DirectionParameter.ParamOutput, 2000, DbType.String);
            parameters.Add(parameter);
            parameter = this.CreateParameter("p_actual_date", DateTime.Now, DirectionParameter.ParamOutput, 0, DbType.DateTime);
            parameters.Add(parameter);
            
            try
            {
                // Esecuzione store
                int result = provider.ExecuteStoredProcedure(LOAD_TOKEN, parameters, null);

                if (result != 1)
                    throw new LoadException();

            }
            catch (Exception e)
            {
                throw new LoadException(e);
            }

            try
            {
                // Prelevamento valori
                ParameterSP[] convertedList = (ParameterSP[])parameters.ToArray(typeof(ParameterSP));
                key = this.GetValueReturnedFromStore("p_key", convertedList).ToString();
                initializationVector = this.GetValueReturnedFromStore("p_initialization_vector", convertedList).ToString();
                actualDate = DateTime.Parse(this.GetValueReturnedFromStore("p_actual_date", convertedList).ToString());
            }
            catch (Exception)
            {
                throw new LoadException();
            }

            if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(encryptedToken))
                throw new LoadException();

            return new CryptoString(key, initializationVector);
        }

        /// <summary>
        /// Funzione per il reperimento della data attuale da DB
        /// </summary>
        /// <returns>La data attuale calcolata dal DB</returns>
        private DateTime GetActualDateFromDB()
        {
            // Data attuale calcolata da DB
            DateTime actualDate = DateTime.Now;
            using (DBProvider dbProvider = new DBProvider())
            {
                DateTime.TryParse(dbProvider.GetSysdate(), out actualDate);

            }

            return actualDate;

        }

        /// <summary>
        /// Funzione per calcolo della data di scadenza del token
        /// </summary>
        /// <param name="tokenType">Tipo di token</param>
        /// <param name="milliseconds">Durata in millisecondi del token</param>
        /// <returns>Data di scadenza del token</returns>
        private String GetExpirationDate(AuthTokenManager.TokenTypeEnum tokenType, double milliseconds)
        {
            // Di default la data di scadenza è oggi stesso
            DateTime expirationDate = DateTime.Now;

            switch (tokenType)
            {
                case AuthTokenManager.TokenTypeEnum.OneTime:     // Oggi
                    expirationDate = this.GetActualDateFromDB();
                    break;
                case AuthTokenManager.TokenTypeEnum.Temporal:    // Fra 'hours' ore
                    expirationDate = this.GetActualDateFromDB().AddMilliseconds(milliseconds);
                    break;
                case AuthTokenManager.TokenTypeEnum.Explicit:    // Esplicito
                    expirationDate = DateTime.MaxValue;
                    break;
                
            }

            return expirationDate.ToString();

        }

        /// <summary>
        /// Funzione per la verifica di validità del token
        /// </summary>
        /// <param name="tokenExpirationDate">Data di scadenza del token</param>
        /// <returns>True se il token è scaduto</returns>
        private bool IsExpiredToken(String tokenExpirationDate)
        {
            // Data attuale prelevata dal DB
            DateTime actualDate = this.GetActualDateFromDB();

            // Risultato del controllo
            bool tokenExpired = true;

            // Il token è scaduto se è valorizzato e la data di scadenza interpretata è minore
            // della data attuale
            if (!String.IsNullOrEmpty(tokenExpirationDate))
            {
                DateTime expirationDate = DateTime.Now;
                DateTime.TryParse(tokenExpirationDate, out expirationDate);

                tokenExpired = actualDate.CompareTo(expirationDate) >= 0;
            }

            return tokenExpired;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Funzione per la verifica di validità del token
        /// </summary>
        /// <param name="token">Token da verificare</param>
        /// <returns>True se il token è valido</returns>
        public override bool IsAuthToken(string token)
        {
            return (token.IndexOf(TOKEN_PREFIX) > -1);
        }

        /// <summary>
        /// Generazione del token di autenticazione per l'utente specificato
        /// </summary>
        /// <param name="userId">User id dell'utente proprietario del token</param>
        /// <param name="tokenType">Tipo di token da generare</param>
        /// <param name="milliseconds">Durata del token espressa in millisecondi</param>
        /// <param name="keyLength">Lunghezza della chiave espressa in numero di caratteri</param>
        /// <param name="initializationVectorLength">Lunghezza dell'initialization vector espressa in numero di caratteri</param>
        /// <returns>Token criptato</returns>
        public override string Generate(string userId, AuthTokenManager.TokenTypeEnum tokenType, double milliseconds, AuthTokenManager.KeyLengthEnum keyLength, AuthTokenManager.KeyLengthEnum initializationVectorLength)
        {
            // Chiave privata, vettore di inizializzazione e la userid pulita
            String key, initializationVector, cleanedUserId;

            // Pulizia dello user id
            cleanedUserId = userId.Trim().ToUpper();

            // Decifratura lunghezza della chiave e dell'initialization vector
            int kLength = this.DecodeKeyLength(keyLength);
            int ivLength = this.DecodeKeyLength(initializationVectorLength);

            // Generazione della chiavi
            key = KeyGeneratorHelper.BetterRandomString(kLength);
            initializationVector = KeyGeneratorHelper.BetterRandomString(ivLength);
            
            try
            {
                CryptoString crypto = new CryptoString(key, initializationVector);
                string encodedValue = crypto.Encrypt(String.Format(ENCRYPTED_VALUE_FORMAT, cleanedUserId, GetSessionId(), this.GetActualDateFromDB(), tokenType, this.GetExpirationDate(tokenType, milliseconds)));

                // Salvataggio token su db
                this.SaveToken(key, encodedValue, initializationVector, cleanedUserId);

                return string.Format("{0}{1}", TOKEN_PREFIX, encodedValue);
            }
            catch (Exception ex)
            {
                throw new EncryptionException(ex);
            }
        }

        /// <summary>
        /// Ripristino del token di autenticazione per l'utente specificato
        /// </summary>
        /// <param name="userId">Identificativo dell'utente proprietario del token</param>
        /// <param name="token">Token criptato</param>
        /// <returns>Token decriptato</returns>
        public override string Restore(string userId, string token)
        {
            if (IsAuthToken(token))
            {
                // Pulizia del token
                String cleanedToken = token.Replace(TOKEN_PREFIX, String.Empty);

                // Pulizia e della user id
                String cleanedUserId = userId.Trim().ToUpper();

                // Recupero delle informazioni di encryption dal DB
                CryptoString crypto= this.LoadToken(cleanedToken);

                String decryptedValue = String.Empty;
                try
                {
                    decryptedValue = crypto.Decrypt(cleanedToken);
                }
                catch (Exception ex)
                {
                    throw new DecryptionException(ex);
                }

                // Spacchettamento del token
                Dictionary<String, String> keyValuePairs = GetTokenKeyValuePairs(decryptedValue);

                // Controllo della validità del token
                bool expired = this.IsExpiredToken(keyValuePairs[TOKENEXPIRATIONDATE]);

                // Se il token è scaduto, viene cancellato il record dalla tabella
                if(expired)
                    this.RemoveTokenFromDB(cleanedToken);

                if (String.Compare(cleanedUserId, keyValuePairs[UID], true) == 0 && (!expired || this.DecodeTokenType(keyValuePairs[TOKENTYPE]) == AuthTokenManager.TokenTypeEnum.OneTime))
                {
                    return keyValuePairs[SESSIONID];
                }
                else
                    throw new NoAuthenticationTokenForUserException(userId);
            }
            else
                throw new AuthenticationTokenNotValidException();
        }

        /// <summary>
        /// Funzione per la rimozione del token specificato
        /// </summary>
        /// <param name="userId">Id dell'utente proprietario del token</param>
        /// <param name="token">Token da eliminare</param>
        public override void RemoveToken(String userId, String token)
        {
            if (IsAuthToken(token))
            {
                // Pulizia del token
                String cleanedToken = token.Replace(TOKEN_PREFIX, String.Empty);

                // Recupero delle informazioni di encryption dal DB
                CryptoString crypto = this.LoadToken(cleanedToken);

                string decryptedValue = string.Empty;

                try
                {
                    decryptedValue = crypto.Decrypt(cleanedToken);
                }
                catch (Exception ex)
                {
                    throw new DecryptionException(ex);
                }

                // Spacchettamento delle informazioni
                Dictionary<string, string> keyValuePairs = GetTokenKeyValuePairs(decryptedValue);

                // Se user id passato per parametro e user id presente nel token non corrispondono, non si può procedere
                if (!userId.Trim().ToUpper().Equals(keyValuePairs[UID]))
                    throw new NoAuthenticationTokenForUserException(userId);

                // Rimozione del token dal DB
                this.RemoveTokenFromDB(cleanedToken);

            }
            else
                throw new AuthenticationTokenNotValidException();
 
        }

        #endregion

    }
}
