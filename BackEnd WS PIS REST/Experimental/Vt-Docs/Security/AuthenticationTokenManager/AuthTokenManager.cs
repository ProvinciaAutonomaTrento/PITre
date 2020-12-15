using System;
using Security.Core.Exceptions;

namespace Security.Core.AuthenticationTokenManager
{
    /// <summary>
    /// Interfaccia che deve essere implementata per realizzare un manager dei token di autenticazione
    /// </summary>
    public abstract class AuthTokenManager
    {
        /// <summary>
        /// Tipologia di token
        /// </summary>
        public enum TokenTypeEnum
        {
            /// <summary>
            /// Token monouso
            /// </summary>
            OneTime,
            /// <summary>
            /// Token temporale
            /// </summary>
            Temporal,
            /// <summary>
            /// Token esplicito
            /// </summary>
            Explicit
        }

        /// <summary>
        /// Lunghezza della chiave espressa in caratteri
        /// </summary>
        public enum KeyLengthEnum
        {
            Sixteen,
            ThirtyTwo
        }

        /// <summary>
        /// Funzione per l'instanziazione del manager di autenticazione da utilizzare a partire dal suo nome
        /// </summary>
        /// <param name="instanceFullName">Nome completo dell'instanza da attivare</param>
        /// <returns>Instanza del manager da utilizzare</returns>
        public static AuthTokenManager GetInstance(String instanceFullName)
        {
            AuthTokenManager tokenManager = null;

            try
            {
                // Instanziazione del tipo
                Type type = Type.GetType(String.Format("Security.Core.AuthenticationTokenManager.{0}", instanceFullName));

                // Attivazione dell'instanza
                tokenManager = Activator.CreateInstance(type) as AuthTokenManager;
            }
            catch
            {
                throw new AuthManagerNotFound(instanceFullName);
            }

            return tokenManager;
        }

        /// <summary>
        /// Funzione per verificare se una data stringa è un token di autenticazione
        /// </summary>
        /// <param name="token">Token da verificare</param>
        /// <returns>True se la stringa è un token di autenticazione</returns>
        public abstract bool IsAuthToken(String token);

        /// <summary>
        /// Funzione per la generazione di un token di autenticazione.
        /// </summary>
        /// <param name="userId">User id dell'utente per cui generare il token di autenticazione</param>
        /// <param name="tokenType">Tipo di token da generare</param>
        /// <param name="milliseconds">Durata del token espressa in millisecondi</param>
        /// <param name="keyLength">Lunghezza della chiave di crypting espressa in numero di caratteri</param>
        /// <param name="initializationVectorLength">Lunghezza del vettore di inizializzazione espresso in numero di caratteri</param>
        /// <returns>Token generato</returns>
        public abstract string Generate(String userId, TokenTypeEnum tokenType, Double milliseconds, KeyLengthEnum keyLength, KeyLengthEnum initializationVectorLength);

        /// <summary>
        /// Funzione per il ripristino di un token di autenticazione
        /// </summary>
        /// <param name="userId">User id dell'utente per cui restorare il token</param>
        /// <param name="token">Token da ripristinare</param>
        /// <returns>Token decriptato</returns>
        public abstract string Restore(String userId, String token);

        /// <summary>
        /// Funzione per l'invalidazione di un token di autenticazione
        /// </summary>
        /// <param name="userId">User id dell'utente di cui rimuovere il token di autenticazione</param>
        /// <param name="token">Token da rimuovere</param>
        public abstract  void RemoveToken(String userId, String token);

    }
}
