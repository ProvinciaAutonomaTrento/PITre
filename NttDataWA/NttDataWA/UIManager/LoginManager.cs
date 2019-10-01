using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Web.UI;

namespace NttDataWA.UIManager
{
    public class LoginManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        /// <summary>
        /// Initialize languages
        /// </summary>
        public static void IniInitializesLanguages()
        {
            try
            {
                Utils.Languages.InitializesLanguages();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Get available languages
        /// </summary>
        /// <returns>List<string></returns>
        public static List<string> GetAvailableLanguages()
        {
            try
            {
                return Utils.Languages.GetAvailableLanguages();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Get Language direction from language code
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string GetLanguageDirectionFromCode(string language)
        {
            try
            {
                return Utils.Languages.GetLanguageDirection(language);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Get a label in a selected language
        /// </summary>
        /// <param name="idControl">idControl</param>
        /// <param name="language">language</param>
        /// <returns>string</returns>
        public static string GetLabelFromCode(string idControl, string language)
        {
            try
            {
                return Utils.Languages.GetLabelFromCode(idControl, language);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Get a message in a selected language
        /// </summary>
        /// <param name="idControl">idControl</param>
        /// <param name="language">language</param>
        /// <returns>string</returns>
        public static string GetMessageFromCode(string idControl, string language)
        {
            try
            {
                return Utils.Languages.GetMessageFromCode(idControl, language);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// If multilanguage is active return true
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsEnableMultiLanguages()
        {
            try
            {
                return Utils.Languages.IsEnableMultiLanguages();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Get language description
        /// </summary>
        /// <param name="idControl">idControl</param>
        /// <param name="language">language</param>
        /// <returns>string</returns>
        public static string GetDescriptionFromCode(string language)
        {
            try
            {
                return Utils.Languages.GetDescriptionFromCode(language);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="user">UserLogin</param>
        /// <param name="oldPassword">string</param>
        /// <returns>ValidationResultInfo</returns>
        public static DocsPaWR.ValidationResultInfo UserChangePassword(DocsPaWR.UserLogin user, string oldPassword)
        {
            try
            {
                return docsPaWS.UserChangePassword(user, oldPassword);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Verify if integration CAS is Active (SSO)
        /// </summary>
        public static bool IsActiveCAS()
        {
            try
            {
                return !string.IsNullOrEmpty(GetCASServiceUrl());
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Get CAS services Services URL
        /// </summary>
        /// <returns></returns>
        public static string GetCASServiceUrl()
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.SSOURL.ToString()];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Forced User Login
        /// </summary>
        /// <param name="login">UserLogin</param>
        /// <param name="loginResult">LoginResult</param>
        /// <returns>Utente</returns>
        public static Utente ForcedLogin(Page page, DocsPaWR.UserLogin login, out DocsPaWR.LoginResult loginResult)
        {
            Utente user = null;
            loginResult = DocsPaWR.LoginResult.OK;
            string ipaddress;
            try
            {
                loginResult = docsPaWS.Login(login, true, page.Session.SessionID, out user, out ipaddress);
            }
            catch (Exception exception)
            {
                //logger.Debug("Login Error", exception);
                loginResult = DocsPaWR.LoginResult.APPLICATION_ERROR;
                user = null;
            }
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return null;
            //}
            return user;
        }

        /// <summary>
        /// User Login
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="login">UserLogin</param>
        /// <param name="loginResult">LoginResult</param>
        /// <param name="ipaddress">string</param>
        /// <returns>Utente</returns>
        public static Utente Login(Page page, DocsPaWR.UserLogin login,
            out DocsPaWR.LoginResult loginResult, out string ipaddress)
        {
            Utente utente = null;
            loginResult = DocsPaWR.LoginResult.OK;
            ipaddress = string.Empty;
            try
            {
                loginResult = docsPaWS.Login(login, false, page.Session.SessionID, out utente, out ipaddress);
            }
            catch (Exception exception)
            {
                //logger.Debug("Login Error", exception);
                loginResult = DocsPaWR.LoginResult.APPLICATION_ERROR;
                utente = null;
            }
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return null;
            //}

            return utente;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public static void LogOut(string sessionId)
        {
            try
            {
                InfoUtente infoUt = UserManager.GetInfoUser();
                if (infoUt!=null)
                    docsPaWS.Logoff(infoUt.userId, infoUt.idAmministrazione, sessionId, infoUt.dst);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void LogOut(string userId, string idAmministrazione, string sessionId)
        {
            try
            {
                docsPaWS.LogoffOtherSessions(userId, idAmministrazione, sessionId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DocsPaWR.UserLogin CreateUserLoginCurrentUser(string password)
            {
            return CreateUserLoginCurrentUser(password, false);
        }

        /// <summary>
        /// Creazione oggetto "UserLogin" a partire dai metadati dalla sessione utente corrente
        /// </summary>
        /// <param name="password">La password deve essere fornita dall'utente, in quanto non è mantenuta nella sessione</param>
        /// <returns></returns>
        public static DocsPaWR.UserLogin CreateUserLoginCurrentUser(string password, bool changePwd)
        {
            DocsPaWR.UserLogin userLogin = null;

            // Reperimento oggetto infoutente corrente
            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

            if (infoUtente != null)
            {
                userLogin = new DocsPaWR.UserLogin();
                userLogin.SystemID = infoUtente.idPeople;
                userLogin.UserName = infoUtente.userId;
                userLogin.Password = password;
                if (changePwd)
                userLogin.IdAmministrazione = "0";
                else
                    userLogin.IdAmministrazione = infoUtente.idAmministrazione;
                userLogin.DST = infoUtente.dst;
                userLogin.IPAddress = HttpContext.Current.Request.UserHostAddress;
            }

            return userLogin;
        }

        public static bool ModificaPasswordUtenteMultiAmm(Page page, string userId, string idAmm)
        {
            bool resultValue = false;

            try
            {
                return docsPaWS.ModificaPasswordUtenteMultiAmm(userId, idAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return resultValue;
            }

            return resultValue;
        }

        public static void LogOut2(string userId, string idAmministrazione, string sessionId, string dst)
        {
            try
            {
                docsPaWS.Logoff(userId, idAmministrazione, sessionId, dst);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
    }
}