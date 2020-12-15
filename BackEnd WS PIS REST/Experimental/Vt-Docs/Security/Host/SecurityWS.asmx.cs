using System;
using System.Web.Services;
using DocsPaUtils.Exceptions;
using System.Configuration;
using Security.Core.AuthenticationTokenManager;

namespace Security.Core.Host
{
    /// <summary>
    /// Questo web service fornisce un punto di accesso web al motore del convertitore
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/VTDocs/Security")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    class SecurityWS : System.Web.Services.WebService
    {
        /// <summary>
        /// Istanza statica del manager dei token di autenticazione
        /// </summary>
        private static AuthTokenManager _authTokenManager;

        /// <summary>
        /// Funzione per l'inizializzazione del manager dei token di autenticazione
        /// </summary>
        private void InitializeSecurityToken()
        {
            if (_authTokenManager == null)
                _authTokenManager = AuthTokenManager.GetInstance(ConfigurationManager.AppSettings["AuthManagerFullName"]);
        }

        /// <summary>
        /// Metodo web per la verifica della validità di un token di autenticazione
        /// </summary>
        /// <param name="token">Token da verificare</param>
        /// <returns>True se il token è valido</returns>
        [WebMethod(Description="Metodo web per la verifica della validità di un token di autenticazione.")]
        public bool IsAuthToken(string token)
        {
            this.InitializeSecurityToken();
            return _authTokenManager.IsAuthToken(token);
            
        }

        /// <summary>
        /// Metodo web per la generazione di un token di autenticazione one time.
        /// </summary>
        /// <param name="userId">User id dell'utente per cui generare un token one time</param>
        /// <param name="keyLength">Lunghezza della chiave espressa in numero di caratteri</param>
        /// <param name="initializationVectorLength">Lunghezza del vettore di inizializzazione espressa in numero di caratteri</param>
        /// <returns>Token di autenticazione one time</returns>
        [WebMethod(Description="Metodo web per la generazione di un token di autenticazione one time.")]
        public string GenerateOneTimeToken(string userId, AuthTokenManager.KeyLengthEnum keyLength, AuthTokenManager.KeyLengthEnum initializationVectorLength)
        {
            this.InitializeSecurityToken();

            try
            {
                return _authTokenManager.Generate(
                    userId,
                    AuthTokenManager.TokenTypeEnum.OneTime,
                    TimeSpan.Zero.TotalMilliseconds,
                    keyLength,
                    initializationVectorLength);
            }
            catch (Exception e)
            {
                throw SoapExceptionFactory.Create(e);
            }
 
        }

        /// <summary>
        /// Metodo web per la generazione di un token di autenticazione temporale.
        /// </summary>
        /// <param name="userId">User id dell'utente per cui generare un token one time</param>
        /// <param name="milliseconds">Durata del token espressa in millisecondi</param>
        /// <param name="keyLength">Lunghezza della chiave espressa in numero di caratteri</param>
        /// <param name="initializationVectorLength">Lunghezza del vettore di inizializzazione espressa in numero di caratteri</param>
        /// <returns>Token di autenticazione temporal</returns>
        [WebMethod(Description="Metodo web per la generazione di un token di autenticazione temporale.")]
        public string GenerateTemporalToken(String userId, double milliseconds, AuthTokenManager.KeyLengthEnum keyLength, AuthTokenManager.KeyLengthEnum initializationVectorLength)
        {
            this.InitializeSecurityToken();

            try
            {
                return _authTokenManager.Generate(
                    userId,
                    AuthTokenManager.TokenTypeEnum.Temporal,
                    milliseconds,
                    keyLength,
                    initializationVectorLength);
            }
            catch (Exception e)
            {
                throw SoapExceptionFactory.Create(e);
            }
            
        }

        /// <summary>
        /// Metodo web per la generazione di un token di autenticazione a invalidazione esplicita.
        /// </summary>
        /// <param name="userId">User id dell'utente per cui generare un token explicit</param>
        /// <param name="keyLength">Lunghezza della chiave espressa in numero di caratteri</param>
        /// <param name="initializationVectorLength">Lunghezza del vettore di inizializzazione espressa in numero di caratteri</param>
        /// <returns>Token di autenticazione explicit</returns>
        [WebMethod(Description="Metodo web per la generazione di un token di autenticazione a invalidazione esplicita.")]
        public string GenerateExplicitToken(String userId, AuthTokenManager.KeyLengthEnum keyLength, AuthTokenManager.KeyLengthEnum initializationVectorLength)
        {
            this.InitializeSecurityToken();

            try
            {
                return _authTokenManager.Generate(
                    userId,
                    AuthTokenManager.TokenTypeEnum.Explicit,
                    TimeSpan.MaxValue.TotalMilliseconds,
                    keyLength,
                    initializationVectorLength);
            }
            catch (Exception e)
            {
                throw SoapExceptionFactory.Create(e);
            }
        }

        /// <summary>
        /// Metodo web per l'annullamento di un token di autenticazione.
        /// </summary>
        /// <param name="userId">User id dell'utente proprietario del token da invalidare</param>
        /// <param name="token">Token da rimuovere</param>
        [WebMethod(Description="Metodo web per l'annullamento di un token di autenticazione.")]
        public void RemoveToken(String userId, String token)
        {
            this.InitializeSecurityToken();

            try
            {
                _authTokenManager.RemoveToken(
                    userId,
                    token);
            }
            catch (Exception e)
            {
                throw SoapExceptionFactory.Create(e);
            }
        }

        /// <summary>
        /// Metodo web per il restore di una sessione.
        /// </summary>
        /// <param name="userId">User id dell'utente proprietario del token da ripristinare</param>
        /// <param name="token">Token da ripristinare</param>
        /// <returns>Token di autenticazione descriptato</returns>
        [WebMethod(Description="Metodo web per il restore di una sessione.")]
        public string RestoreToken(string userId, string token)
        {
            this.InitializeSecurityToken();

            try
            {
                return _authTokenManager.Restore(
                    userId,
                    token);
            }
            catch (Exception e)
            {
                throw SoapExceptionFactory.Create(e);
            }
 
        }
        
    }

}
