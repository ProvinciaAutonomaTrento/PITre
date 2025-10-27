// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Xml;

namespace BusinessLogic.Utenti;

public class Login
{
    private static ILogger logger = Log.ForContext(typeof(Login));

    public static void logoff(string userId, string idAmm, string dst)
    {
        DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        gestioneUtenti.UnlockUserLogin(userId, idAmm);
        DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();
        userManager.LogoutUser(dst);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="objLogin"></param>
    /// <param name="loginResult"></param>
    /// <param name="forcedLogin">
    /// Imposta se la connessione deve essere forzata, ossia
    /// una eventuale connessione esistente viene annullata).
    /// </param>
    /// <returns></returns>
    public static DocsPaVO.utente.Utente loginMethod(DocsPaVO.utente.UserLogin objLogin,
        out DocsPaVO.utente.UserLogin.LoginResult loginResult, bool forcedLogin,
        string webSessionId, out string ipaddress)
    {
        DocsPaVO.utente.Utente utente = null;
        loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
        ipaddress = string.Empty;
        bool multiAmm = false;

        try
        {
            if (!string.IsNullOrWhiteSpace(objLogin.Password) && DocsPaUtils.Security.SSOAuthTokenHelper.IsAuthToken(objLogin.Password))
            {

                string ssoTok = DocsPaUtils.Security.SSOAuthTokenHelper.Restore(objLogin.UserName, objLogin.Password);
                objLogin.SSOLogin = true;

            }
        }
        catch { }

        try
        {
            // Ricerca dell'utente in amministrazione
            if (string.IsNullOrEmpty(objLogin.IdAmministrazione))
            {
                try
                {
                    ArrayList listaAmmin = UserManager.getListaIdAmmUtente(objLogin);

                    if (listaAmmin != null && listaAmmin.Count > 0)
                    {
                        if (listaAmmin.Count == 1)
                            objLogin.IdAmministrazione = listaAmmin[0].ToString();
                        else
                        {
                            //loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_AMMIN;
                            multiAmm = true;
                            objLogin.IdAmministrazione = listaAmmin[0].ToString();
                        }
                    }
                    if (listaAmmin == null) logger.Debug("Attenzione, la query S_People in GetIdAmmUtente non ha dato alcun risultato.");
                }
                catch (Exception ex)
                {
                    logger.Debug("Errore di connessione al DB durante la procedura di login");
                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.DB_ERROR;
                }
            }

            // Se l'amministrazione è stata impostata
            if (!string.IsNullOrEmpty(objLogin.IdAmministrazione))
            {
                // Get User
                DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

                if (userManager.LoginUser(objLogin, out utente, out loginResult))
                {
                    // Impostazione id sessione utente
                    utente.sessionID = webSessionId;

                    if (!forcedLogin) // Gestione delle connessioni esistenti da amministrazione
                    {
                        //login concesso all'utente
                        //si verifica la tabella DPA_LOGIN per unicità della connessione
                        //la funzione torna True se l'utente è già collegato
                        DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();

                        if (!gestioneUtenti.CheckUserLogin(utente.userId, utente.idAmministrazione))
                        {
                            utente.ruoli = UserManager.getRuoliUtente(utente.idPeople);
                            utente.dominio = getDominio(utente.idPeople);

                            if (utente.ruoli.Count == 0 && DocsPaUtils.Moduli.ModuliAuthManager.RolesRequired(objLogin.Modulo))
                            {
                                loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_RUOLI;
                                utente = null;
                            }
                            else
                            {
                                gestioneUtenti.LockUserLogin(utente.userId, utente.idAmministrazione, webSessionId, objLogin.IPAddress, utente.dst);
                                if (!multiAmm)
                                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
                                else
                                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_AMMIN;
                            }
                        }
                        else
                        {
                            if (!multiAmm)
                                loginResult = DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN;
                            else
                                loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_AMMIN;
                            //loginResult = DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN;
                            ipaddress = gestioneUtenti.GetUserIPAddress(utente.userId, utente.idAmministrazione);
                            utente = null;
                        }
                    }
                    else
                    {
                        // Gestione utente delle connessioni esistenti
                        DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();

                        // Cancella eventuali connessioni esistenti
                        gestioneUtenti.UnlockUserLogin(utente.userId, utente.idAmministrazione);

                        // Assegna connessione
                        gestioneUtenti.LockUserLogin(utente.userId, utente.idAmministrazione,
                            webSessionId, objLogin.IPAddress, utente.dst);
                        utente.ruoli = UserManager.getRuoliUtente(utente.idPeople);
                        utente.dominio = getDominio(utente.idPeople);
                        if (utente.ruoli.Count == 0 && DocsPaUtils.Moduli.ModuliAuthManager.RolesRequired(objLogin.Modulo))
                        {
                            loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_RUOLI;
                            utente = null;
                        }

                        // Qualifiche Utente
                        string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "GESTIONE_QUALIFICHE");
                        if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                        {
                            utente.qualifiche = new ArrayList(Utenti.QualificheManager.GetPeopleGroupsQualificheByIdPeople(utente.idPeople));
                        }

                        // Dato che il nuovo mobile vuole il controllo multiamministrazione, inserisco il controllo.
                        if (multiAmm)
                        {
                            loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_AMMIN;
                            utente = null;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            logger.Debug(e.Message);
            loginResult = DocsPaVO.utente.UserLogin.LoginResult.APPLICATION_ERROR;
            logger.Debug("Errore nella gestione degli utenti (loginMethod)");
            utente = null;
        }

        return utente;
    }

    public static string getDominio(string idPeople)
    {
        #region Codice Commentato
        /*
			logger.Debug("getLibrary");
			string queryString =
				" SELECT NETWORK_ID FROM NETWORK_ALIASES WHERE PERSONORGROUP= " + idPeople;

			logger.Debug (queryString);
			string dominio = null;
			try
			{
				dominio = db.executeScalar(queryString).ToString();
			}
			catch (Exception) {}
			*/
        #endregion

        string dominio = null;
        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        utenti.GetDominio(out dominio, idPeople);
        return dominio;
    }

    public static string ElencoUtentiConnessi(string codiceAmm)
    {
        string result = null;
        DataSet elencoUtenti = null;
        DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        elencoUtenti = gestioneUtenti.GetUtentiConnessi(codiceAmm);
        if (elencoUtenti != null)
        {
            XmlDocument xmlUtenti = new XmlDocument();

            XmlNode utenti = xmlUtenti.AppendChild(xmlUtenti.CreateElement("UTENTI"));

            foreach (DataRow utente in elencoUtenti.Tables[0].Rows)
            {
                XmlNode nodoUtente = utenti.AppendChild(xmlUtenti.CreateElement("UTENTE"));
                string userId = utente["USER_ID"].ToString().ToUpper();
                nodoUtente.AppendChild(xmlUtenti.CreateElement("USERID")).InnerText = userId;
                string nomeUtente = gestioneUtenti.GetNomeUtente(userId);
                nodoUtente.AppendChild(xmlUtenti.CreateElement("DESCRIZIONE")).InnerText = nomeUtente;
                nodoUtente.AppendChild(xmlUtenti.CreateElement("DATAORA")).InnerText = (utente["DTA_CONNESSIONE"]as DateTime?).Value.ToString(new CultureInfo("it-IT"));
            }
            result = xmlUtenti.OuterXml;
        }
        return result;
    }

    public static int NumUtentiConnessi(string codiceAmm)
    {
        int result = 0;
        DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        result = gestioneUtenti.GetNumUtentiConnessi(codiceAmm);
        if (result == null)
            result = 0;
        return result;
    }

    public static void disconnettiUtente(string userId, string codiceAmm)
    {
        DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazione = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
        string idAmm = amministrazione.GetAdminByName(codiceAmm);
        if (idAmm != null)
        {
            gestioneUtenti.UnlockUserLogin(userId, idAmm);
        }
        return;
    }

    public static DocsPaVO.Validations.ValidationResultInfo ChangePassword(DocsPaVO.utente.UserLogin user, string oldPassword)
    {
        DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

        DocsPaVO.Validations.ValidationResultInfo result = userManager.ChangeUserPwd(user, oldPassword);

        // Se l'esito è positivo...
        if (result.Value == true)
            UserLog.UserLog.WriteLog(user.UserName, "0", "0",
                user.IdAmministrazione, "MODUSER", "0",
                string.Format("Password dell'utente {0} modificata dall'utente stesso",
                    user.UserName), DocsPaVO.Logger.CodAzione.Esito.OK, null);

        return result;
    }

    public static int NumUtentiAttivi(string codiceAmm)
    {
        int result = 0;
        DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        result = gestioneUtenti.GetNumUtentiAttivi(codiceAmm);
        if (result == null)
            result = 0;
        return result;
    }

}
