using System;
using System.Xml;
using System.Data;
using System.Collections;
using log4net;

namespace BusinessLogic.RDE
{
    public class Rde
    {
        private static ILog logger = LogManager.GetLogger(typeof(Rde));

        public static string EsportaUtenti()
        {
            string xmlDoc = string.Empty;

            string commandText = "select * from RDE_UTENTE_2";
            DataSet ds;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                dbProvider.ExecuteQuery(out ds, commandText);
                if (ds!=null && ds.Tables[0].Rows.Count > 0)
                {
                    xmlDoc = ds.GetXml();
                }
            }
            
            return xmlDoc;
        }

        public static string EsportaRegistri()
        {
            string xmlDoc = string.Empty;

            string commandText = "select * from RDE_REGISTRO_2";
            DataSet ds;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                dbProvider.ExecuteQuery(out ds, commandText);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    xmlDoc = ds.GetXml();
                }
            }

            return xmlDoc;
        }

        public static string EsportaAutorizzazioni()
        {
            string xmlDoc = string.Empty;

            string commandText = "select * from RDE_AUTORIZZAZIONE_2";
            DataSet ds;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                dbProvider.ExecuteQuery(out ds, commandText);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    xmlDoc = ds.GetXml();
                }
            }

            return xmlDoc;
        }

        public static DocsPaVO.utente.Utente loginMethod(DocsPaVO.utente.UserLogin objLogin,
                                                        out DocsPaVO.utente.UserLogin.LoginResult loginResult, 
                                                        bool forcedLogin,
                                                        string webSessionId, 
                                                        out string ipaddress)
        {
            DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
			loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
			ipaddress = string.Empty;

			try
			{
				// Ricerca dell'utente in amministrazione
                if (string.IsNullOrEmpty(objLogin.IdAmministrazione))
				{
                    ArrayList listaAmmin = BusinessLogic.Utenti.UserManager.getListaIdAmmUtente(objLogin);
                    
                    if (listaAmmin != null && listaAmmin.Count > 0)
                    {
                        if (listaAmmin.Count == 1)
                            objLogin.IdAmministrazione = listaAmmin[0].ToString();
                        else
                            loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_AMMIN;
                    }
                    if (listaAmmin == null) 
                        logger.Debug("Attenzione, la query S_People in GetIdAmmUtente non ha dato alcun risultato.");
                }

                // Se l'amministrazione è stata impostata
                if (!string.IsNullOrEmpty(objLogin.IdAmministrazione))
                {
                    utente = BusinessLogic.Utenti.UserManager.getUtente(objLogin.UserName, objLogin.IdAmministrazione);
                 
                    utente.ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople);
                    utente.dominio = BusinessLogic.Utenti.Login.getDominio(utente.idPeople);

                    DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();
                    utente.dst = userManager.GetSuperUserAuthenticationToken();

                    if (utente.ruoli.Count == 0)
                    {
                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_RUOLI;
                        utente = null;
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
    }
}
