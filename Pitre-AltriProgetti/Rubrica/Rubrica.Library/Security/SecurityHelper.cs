using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Rubrica.Library.Security
{
    /// <summary>
    /// Classe di utilità per la gestione della security
    /// </summary>
    internal sealed class SecurityHelper
    {
        /// <summary>
        /// Validazione credenziali utente
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public static SecurityCredentialsResult ValidateCredentials(SecurityCredentials credentials)
        {
            SecurityCredentialsResult result = null;

            const string SP_GET = "GetUserCredentials";

            Dpa.DataAccess.Database db = RubricaDatabase.CreateDatabase();

            using (Dpa.DataAccess.DBCommandWrapper cw = db.GetStoredProcCommandWrapper(RubricaDatabase.GetSpNameForPackage(SP_GET)))
            {
                cw.AddInParameter("pNomeUtente", DbType.String, credentials.UserName);
                cw.AddInParameter("pPassword", DbType.String, SecurityCredentials.GetPasswordHash(credentials.UserName, credentials.Password));

                using (IDataReader reader = db.ExecuteReader(cw))
                {
                    if (reader.Read())
                    {
                        result = new SecurityCredentialsResult
                        {
                             UserName = credentials.UserName,
                             Amministratore = (Dpa.DataAccess.Helper.DataReaderHelper.GetValue<string>(reader, "Amministratore", false) == "1")
                        };
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Security.RubricaPrincipal AuthenticatedPrincipal
        {
            get
            {
                return Security.RubricaPrincipal.Current;
            }
        }

        /// <summary>
        /// Verifica oggetto Principal autenticato
        /// </summary>
        public static void CheckAuthenticatedPrincipal()
        {
            if (Security.RubricaPrincipal.Current == null ||
                (Security.RubricaPrincipal.Current != null && !Security.RubricaPrincipal.Current.Identity.IsAuthenticated))
                throw new System.Security.Authentication.AuthenticationException(Properties.Resources.InvalidCredentialsException);
        }

        /// <summary>
        /// Verifica che l'oggetto Principal autenticato sia del ruolo amministratore
        /// </summary>
        public static void CheckAdminAuthenticatedPrincipal()
        {
            CheckAuthenticatedPrincipal();
            
            if (!((Security.RubricaIdentity) Security.RubricaPrincipal.Current.Identity).IsAdminRole)
                throw new System.Security.Authentication.AuthenticationException(Properties.Resources.InvalidCredentialsException);
        }
    }
}