using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.Deleghe;
using DocsPaVO.ricerche;
using System.Collections;
using System.Linq;
using log4net;
using DocsPaVO.utente;

namespace BusinessLogic.Deleghe
{
    public class DelegheManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DelegheManager));
        //Creazione nuova delega
        public static bool creaNuovaDelega(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Deleghe.InfoDelega infoDelega)
        {
            bool result = false;
            
            DocsPaDB.Query_DocsPAWS.Deleghe delega = new DocsPaDB.Query_DocsPAWS.Deleghe();
            result = delega.creaNuovaDelega(infoDelega);
            return result;
        }

        public static bool modificaDelega(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Deleghe.InfoDelega infoDelega, string tipoDelega, string idRuoloOld, string idUtenteOld, string dataScadenzaOld, string dataDecorrenzaOld, string idRuoloDeleganteOld)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Deleghe delega = new DocsPaDB.Query_DocsPAWS.Deleghe();
            result = delega.modificaDelega(infoUtente, infoDelega, tipoDelega, idRuoloOld, idUtenteOld, dataScadenzaOld, dataDecorrenzaOld, idRuoloDeleganteOld);
            return result;
        }

        public static ArrayList getListaDeleghe(DocsPaVO.utente.InfoUtente infoUtente, string tipoDelega, string statoDelega, string idAmm, out int numDeleghe)
        {
            ArrayList deleghe = new ArrayList();
            using (DocsPaDB.TransactionContext transactionalContext = new DocsPaDB.TransactionContext())
            {

                DocsPaDB.Query_DocsPAWS.Deleghe d = new DocsPaDB.Query_DocsPAWS.Deleghe();

                //Recupero la lista delle deleghe assegnate da un dato utente
                deleghe = d.getListaDeleghe(infoUtente, tipoDelega, statoDelega, idAmm, out numDeleghe);

            }
            return deleghe;
        }

        public static List<InfoDelega> searchDeleghe(DocsPaVO.utente.InfoUtente infoUtente, SearchDelegaInfo searchInfo, SearchPagingContext pagingContext)
        {
             DocsPaDB.Query_DocsPAWS.Deleghe d = new DocsPaDB.Query_DocsPAWS.Deleghe();
             if (string.IsNullOrEmpty(searchInfo.NomeDelegato) && string.IsNullOrEmpty(searchInfo.NomeDelegante))
             {
                 return d.searchDeleghe(infoUtente, searchInfo, pagingContext);
             }
             else
             {
                 InfoDelegaMatcher matcher=new InfoDelegaMatcher(searchInfo);
                 List<InfoDelega> temp = d.searchDeleghe(infoUtente, searchInfo).FindAll(matcher.Match);
                 pagingContext.RecordCount = temp.Count;
                 List<InfoDelega> result = new List<InfoDelega>(temp.Skip(pagingContext.StartRow - 1).Take(pagingContext.PageSize));
                 return result;
             }
        }

        public static DocsPaVO.utente.Utente esercitaDelega(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.UserLogin objLogin, string webSessionId, string id_delega, string idRuoloDelegante, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            DocsPaVO.utente.Utente utente = null;
            
            loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
            try
            {
                //INC000001112977 PAT gestione deleghe
                //Dismetto le deleghe rimaste in esercizio
                DocsPaDB.Query_DocsPAWS.Deleghe deleghe = new DocsPaDB.Query_DocsPAWS.Deleghe();
                deleghe.DismettiDelegheInEsercizio(infoUtente);
                 
                // Get User

                // ricostruzione informazioni utente delegante
                logger.DebugFormat("username {0} idAmm {1}", objLogin.UserName, objLogin.IdAmministrazione);
                utente = BusinessLogic.Utenti.UserManager.getUtente(objLogin.UserName, objLogin.IdAmministrazione);
                if (utente == null)
                    logger.Error("utente è null");


                // impostazione token di autenticazione
                DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();
                utente.dst = userManager.GetSuperUserAuthenticationToken();

                // Impostazione id sessione utente
                utente.sessionID = webSessionId;

                //Verifico che l'utente delegante non sia connesso  
                //si verifica la tabella DPA_LOGIN per unicità della connessione
                //la funzione torna True se l'utente è già collegato
                DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();

                if (!gestioneUtenti.CheckUserLogin(utente.userId, utente.idAmministrazione))
                {
                    if (idRuoloDelegante == "0")
                    {
                        //L'utente delegato eredita tutti i ruoli del delegante
                        utente.ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople);
                    }
                    else
                    {
                        ArrayList ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople);
                        ArrayList listRuolo = new ArrayList();
                        foreach (DocsPaVO.utente.Ruolo ruolo in ruoli)
                        {
                            if (idRuoloDelegante.Equals(ruolo.systemId))
                            {
                                listRuolo.Add(ruolo);
                            }
                        }
                        utente.ruoli = listRuolo;

                    }
                    utente.dominio = getDominio(utente.idPeople);

                    if (utente.ruoli.Count == 0)
                    {
                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_RUOLI;
                        utente = null;
                    }
                    else
                    {
                        gestioneUtenti.LockUserLoginDelegato(utente.userId, utente.idAmministrazione, webSessionId, objLogin.IPAddress, utente.dst, infoUtente.userId, id_delega);
                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
                    }
                }
                else
                {
                    //Utente delegante loggato: viene rimosso dalla dpa_login (cosidetta porcata)
                    DocsPaDB.Query_DocsPAWS.Utenti dbUserManager = new DocsPaDB.Query_DocsPAWS.Utenti();
                    if (gestioneUtenti.UnlockUserLogin(utente.userId, utente.idAmministrazione, utente.sessionID))
                    {
                        if (idRuoloDelegante == "0")
                        {
                            //L'utente delegato eredita tutti i ruoli del delegante
                            utente.ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople);
                        }
                        else
                        {
                            ArrayList ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople);
                            ArrayList listRuolo = new ArrayList();
                            foreach (DocsPaVO.utente.Ruolo ruolo in ruoli)
                            {
                                if (idRuoloDelegante.Equals(ruolo.systemId))
                                {
                                    listRuolo.Add(ruolo);
                                }
                            }
                            utente.ruoli = listRuolo;

                        }
                        utente.dominio = getDominio(utente.idPeople);

                        if (utente.ruoli.Count == 0)
                        {
                            loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_RUOLI;
                            utente = null;
                        }
                        else
                        {
                            gestioneUtenti.LockUserLoginDelegato(utente.userId, utente.idAmministrazione, webSessionId, objLogin.IPAddress, utente.dst, infoUtente.userId, id_delega);
                            loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
                        }
                    }
                    else
                    {
                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN;
                        utente = null;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat ( "msg {0}  stk {1} ",e.Message,e.StackTrace);
                loginResult = DocsPaVO.utente.UserLogin.LoginResult.APPLICATION_ERROR;
                logger.Debug("Errore nella gestione degli utenti (loginMethod)");
                utente = null;
            }

            return utente;
        }

        public static bool dismettiDelega(DocsPaVO.utente.InfoUtente infoUtente, string userIdDelegante)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Deleghe deleghe = new DocsPaDB.Query_DocsPAWS.Deleghe();
            result = deleghe.dismettiDelega(infoUtente, userIdDelegante);
            return result;
        }



        public static bool revocaDelega(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Deleghe.InfoDelega[] listaDeleghe, out string msg)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Deleghe deleghe = new DocsPaDB.Query_DocsPAWS.Deleghe();
            result = deleghe.revocaDelega(infoUtente, listaDeleghe, out msg);
            return result;
        }

        public static string getDominio(string idPeople)
        {
            string dominio = null;
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            utenti.GetDominio(out dominio, idPeople);
            return dominio;
        }

        public static InfoDelega getDelegaById(string id)
        {
            DocsPaDB.Query_DocsPAWS.Deleghe delega = new DocsPaDB.Query_DocsPAWS.Deleghe();
            InfoDelega result = delega.getDelegaById(id);
            return result;
        }

        public static bool verificaUnicaDelega(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Deleghe.InfoDelega infoDelega)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.Deleghe delega = new DocsPaDB.Query_DocsPAWS.Deleghe();
            result = delega.verificaUnicaDelega(infoUtente, infoDelega);
            return result;
        }

       
        public static int checkDelegaAttiva(DocsPaVO.utente.InfoUtente infoUtente)
        {
            int result = 0;
            DocsPaDB.Query_DocsPAWS.Deleghe delega = new DocsPaDB.Query_DocsPAWS.Deleghe();
            result = delega.checkDelegaAttiva(infoUtente);
            return result;
        }


        public static bool verificaUnicaDelegaAmm(DocsPaVO.Deleghe.InfoDelega infoDelega)
        {
            bool result = false;

            DocsPaDB.Query_DocsPAWS.Deleghe delega = new DocsPaDB.Query_DocsPAWS.Deleghe();
            result = delega.verificaUnicaDelegaAmm(infoDelega);
            return result;
        }

        public static bool modificaDelegaAmm(DocsPaVO.Deleghe.InfoDelega delegaOld, DocsPaVO.Deleghe.InfoDelega delegaNew, string tipoDelega, string dataScadenzaOld, string dataDecorrenzaOld)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Deleghe delega = new DocsPaDB.Query_DocsPAWS.Deleghe();
            result = delega.modificaDelegaAmm(delegaOld, delegaNew, tipoDelega, dataScadenzaOld, dataDecorrenzaOld);
            return result;
        }

        private class InfoDelegaMatcher
        {
            private delegate bool MatchDel(InfoDelega infoDelega,SearchDelegaInfo searchInfo);
            private static Dictionary<string, MatchDel> _matchDels;
            private SearchDelegaInfo _searchInfo;

            static InfoDelegaMatcher()
            {
                _matchDels = new Dictionary<string, MatchDel>();
                _matchDels.Add("assegnate", AssegnateMatchDel);
                _matchDels.Add("ricevute", RicevuteMatchDel);
                _matchDels.Add("esercizio", RicevuteMatchDel);
                _matchDels.Add("tutte", TutteMatchDel);
            }

            public InfoDelegaMatcher(SearchDelegaInfo searchInfo)
            {
                this._searchInfo = searchInfo;
            }

            public bool Match(InfoDelega infoDelega)
            {
                return _matchDels[_searchInfo.TipoDelega](infoDelega, _searchInfo);
            }

            private static bool AssegnateMatchDel(InfoDelega infoDelega, SearchDelegaInfo searchInfo)
            {
                return ContainString(infoDelega.cod_utente_delegato, searchInfo.NomeDelegato);
            }

            private static bool RicevuteMatchDel(InfoDelega infoDelega, SearchDelegaInfo searchInfo)
            {
                return ContainString(infoDelega.cod_utente_delegante, searchInfo.NomeDelegante);
            }

            private static bool TutteMatchDel(InfoDelega infoDelega, SearchDelegaInfo searchInfo)
            {
                return ContainString(infoDelega.cod_utente_delegato, searchInfo.NomeDelegato) && ContainString(infoDelega.cod_utente_delegante, searchInfo.NomeDelegante);
            }

            private static bool ContainString(string input, string value)
            {
                if(string.IsNullOrEmpty(value)) return true;
                if(string.IsNullOrEmpty(input)) return false;
                return input.ToUpper().Contains(value.ToUpper());
            }
        }
    }
}
