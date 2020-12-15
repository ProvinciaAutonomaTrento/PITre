using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DocsPaDocumentale.Interfaces;
using DocsPaDocumentale_DOCUMENTUM.Documentale;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
using DocsPaUtils.Data;

namespace DocsPaDocumentale_PITRE.Migrazione
{
    /// <summary>
    /// Servizi di connessione al sistema DocsPa
    /// </summary>
    internal class LoginServices
    {
        private LoginServices()
        { }

        /// <summary>
        /// Creazione oggetto UserLogin necessario per la connessione al sistema DocsPa
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static UserLogin GetUserLogin(string userName, string password)
        {
            UserLogin loginData = new UserLogin();
            loginData.UserName = userName;
            loginData.Password = password;
            return loginData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static UserLogin GetUserLogin(string userName, string password, string idAmministrazione)
        {
            UserLogin loginData = new UserLogin();
            loginData.UserName = userName;
            loginData.Password = password;
            loginData.IdAmministrazione = idAmministrazione;
            return loginData;
        }

        /// <summary>
        /// Connessione dell'utente amministratore al sistema DocsPa
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public static InfoUtenteAmministratore LoginAdmin(string userName, string password, out UserLogin.LoginResult loginResult)
        {
            InfoUtenteAmministratore infoUtente;

            DocsPaDocumentale.Interfaces.IUserManager userManager = new DocsPaDocumentale_PITRE.Documentale.UserManager();
            userManager.LoginAdminUser(GetUserLogin(userName, password), true, out infoUtente, out loginResult);

            return infoUtente;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public static string LoginUserDctm(string userName, string password, string idAmministrazione, out UserLogin.LoginResult loginResult)
        {
            DocsPaDocumentale.Interfaces.IUserManager userManager = new DocsPaDocumentale_DOCUMENTUM.Documentale.UserManager();
            
            Utente utente;
            userManager.LoginUser(GetUserLogin(userName, password, idAmministrazione), out utente, out loginResult);

            if (loginResult == UserLogin.LoginResult.OK)
                return utente.dst;
            else
                return string.Empty;
        }
    }
}
