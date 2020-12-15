using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Globalization;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Data;

namespace NttDataWA.UIManager
{
    public class UserManager
    {        
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static void SetUserInSession(DocsPaWR.Utente user)
        {
            try
            {
                user.urlWA = Utils.Common.GetHttpFullPath();
                HttpContext.Current.Session["userData"] = user;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DocsPaWR.Utente GetUserInSession()
        {
            try
            {
                return HttpContext.Current.Session["userData"] as DocsPaWR.Utente;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SetUserLanguage(string language)
        {
            try
            {
                HttpContext.Current.Session["language"] = language;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static string GetUserLanguage()
        {
            try
            {
                return HttpContext.Current.Session["language"] as string;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string GetLanguageData()
        {
            try
            {
                string language = GetUserLanguage();
                if (string.IsNullOrEmpty(language) || language.Equals("Italian"))
                {
                    return "it";
                }
                else
                {
                    return "en";
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static InfoUtente GetInfoUser()
        {
            try
            {
                Utente user = HttpContext.Current.Session["userData"] as Utente;
                Ruolo role = HttpContext.Current.Session["userRole"] as Ruolo;
                InfoUtente infoUser = new InfoUtente();

                if (user != null && role != null)
                {
                    infoUser.idPeople = user.idPeople;
                    infoUser.dst = user.dst;
                    infoUser.idAmministrazione = user.idAmministrazione;
                    infoUser.userId = user.userId;
                    infoUser.sede = user.sede;
                    if (user.urlWA != null)
                        infoUser.urlWA = user.urlWA;
                    infoUser.delegato = getDelegato();

                    infoUser.idCorrGlobali = role.systemId;
                    infoUser.idGruppo = role.idGruppo;
                    //Laura 19 Marzo
                    if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]))
                        infoUser.codWorkingApplication = System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"].ToString();
                }

                return infoUser;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ElementoRubrica[] getGerarchiaElemento(DocsPaWR.InfoUtente infute, string codice, DocsPaWR.AddressbookTipoUtente tipoIE, Page page, DocsPaWR.SmistamentoRubrica smistamentoRubrica)
        {
            try
            {
                return docsPaWS.rubricaGetGerarchiaElemento(codice, tipoIE, infute, smistamentoRubrica);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ElementoRubrica[] getChildrenElemento(string elementID, string childrensType, DocsPaWR.InfoUtente u)
        {
            try
            {
                return docsPaWS.rubricaGetChildrenElement(elementID, childrensType, u);
            }

            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ElementoRubrica[] getRootItems(DocsPaWR.AddressbookTipoUtente tipoIE, DocsPaWR.InfoUtente infute, DocsPaWR.SmistamentoRubrica smistamentoRubrica)
        {
            try
            {
                return docsPaWS.rubricaGetRootItems(tipoIE, infute, smistamentoRubrica);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void check_children_existence(DocsPaWR.InfoUtente infute, ref ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti)
        {
            try
            {
                docsPaWS.rubricaCheckChildrenExistenceEx(ref ers, checkUo, checkRuoli, checkUtenti, infute);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static Ruolo GetSelectedRole()
        {
            try
            {
                return HttpContext.Current.Session["userRole"] as Ruolo;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Corrispondente getCorrispondentBySystemID(string systemID)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteBySystemId(systemID);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Canale GetCanalePreferenzialeByIdCorr(string systemID)
        {
            try
            {
                return docsPaWS.GetCanalePreferenzialeByIdCorr(systemID);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Corrispondente getCorrispondenteBySystemIDDisabled(string systemID)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteBySystemIdDisabled(systemID);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Return a list of corrispondent
        /// </summary>
        /// <param name="qco"></param>
        /// <returns></returns>
        public static Corrispondente[] getListCorrispondent(AddressbookQueryCorrispondente qco)
        {
            Corrispondente[] result;
            try
            {
                result = docsPaWS.AddressbookGetListaCorrispondenti(qco);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static Registro[] GetRegistersList()
        {
            Registro[] result = null;
            try
            {
                result = docsPaWS.UtenteGetRegistri(GetInfoUser().idCorrGlobali);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        /// <summary>
        /// Dato un idRuolo ritorna Registri e RF o solo RF.
        /// Se all="" allora anche idAooColl deve essere = "" e il metodo ritorna sia Reg che RF
        /// Se all="0" (e idAooColl deve essere = "" ) ritorna tutti gli RF assocaiati a un determinato ruolo
        /// Se all = "1" (idAooColl deve essere popolato con una systemId di un registro) e in
        /// tal caso il metoto ritorna tutti gli Rf associati a quel registro e visibili dal ruolo
        /// corrente
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static Registro[] getListaRegistriWithRF(Page page, string all, string idAooColl)
        {
            Registro[] result = null;
            try
            {
                result = docsPaWS.UtenteGetRegistriWithRf(GetInfoUser().idCorrGlobali, all, idAooColl);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        /// <summary>
        /// Dato un idRuolo ritorna Registri e RF o solo RF.
        /// Se all="" allora anche idAooColl deve essere = "" e il metodo ritorna sia Reg che RF
        /// Se all="0" (e idAooColl deve essere = "" ) ritorna tutti gli RF assocaiati a un determinato ruolo
        /// Se all = "1" (idAooColl deve essere popolato con una systemId di un registro) e in
        /// tal caso il metoto ritorna tutti gli Rf associati a quel registro e visibili dal ruolo
        /// corrente
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <param name="all"></param>
        /// <param name="idAooColl"></param>
        /// <returns></returns>
        public static Registro[] getListaRegistriWithRF(string idRuolo, string all, string idAooColl)
        {
            Registro[] result = null;
            try
            {
                result = docsPaWS.UtenteGetRegistriWithRf(idRuolo, all, idAooColl);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static Registro getRegisterSelected()
        {
            try
            {
                return (Registro)GetSessionValue("userRegistro");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Registro getRegistroBySistemId(Page page, string idRegistro)
        {
            DocsPaWR.Registro result = null;
            try
            {
                result = docsPaWS.GetRegistroBySistemId(idRegistro);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }

        public static void setRegistroSelezionato(Page page, Registro registro)
        {
            try
            {
                //setto la lista degli idRegistri selezionati per le ricerche dei corrispondenti...
                setListaIdRegistri(page, registro.systemId);

                //page.Session["userRegistro"] = registro;
                SetSessionValue("userRegistro", registro);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void setListaIdRegistri(Page page, string idRegistro)
        {
            try
            {
                string[] idRegistri = { idRegistro };
                setListaIdRegistri(page, idRegistri);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void setListaIdRegistri(Page page, string[] idRegistri)
        {
            try
            {
                page.Session["rubrica.listaIdRegistri"] = idRegistri;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        //Laura 25 Marzo
        public static string[] getListaIdRegistri(Page page)
        {
            return (string[])page.Session["rubrica.listaIdRegistri"];
        }

        public static bool IsAuthorizedFunctions(string tipologia)
        {
            try
            {
                String tipologiaUpper = tipologia.ToUpper();
                DocsPaWR.Ruolo userRuolo;
                if ((userRuolo = GetSelectedRole()) == null)
                    return false;
                return (from function in userRuolo.funzioni where function.codice.ToUpper().Equals(tipologiaUpper) select function.systemId).Count() > 0 ? true : false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool IsRoleAuthorizedFunctions(string tipologia, DocsPaWR.Ruolo ruolo)
        {
            try
            {
                if (ruolo == null)
                    return false;
                String tipologiaUpper = tipologia.ToUpper();
                return (from function in ruolo.funzioni where function.codice.ToUpper().Equals(tipologiaUpper) select function.systemId).Count() > 0 ? true : false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }


        public static bool isFiltroAooEnabled()
        {
            bool result = false;
            try
            {
                result = docsPaWS.isFiltroAooEnabled();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;

        }

        /// <summary>
        /// Return a list of corresponding
        /// </summary>
        /// <param name="page"></param>
        /// <param name="myCorr"></param>
        /// <returns></returns>
        public static string GetCorrespondingDescription(Page page, Corrispondente myCorr)
        {
            try
            {
                string desc = "";
                if (myCorr == null)
                    return "";

                if (myCorr.GetType() == typeof(Ruolo))
                {
                    UnitaOrganizzativa corrUO;
                    string descrUO = "";

                    Ruolo corrRuolo = (Ruolo)myCorr;
                    descrUO = "";
                    corrUO = corrRuolo.uo;

                    while (corrUO != null)
                    {
                        descrUO = descrUO + " - " + corrUO.descrizione;
                        corrUO = corrUO.parent;
                    }
                    desc = corrRuolo.descrizione + descrUO;
                }
                else
                {
                    desc = myCorr.descrizione;
                }

                return desc;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool verifyRegNoAOO(DocsPaWR.SchedaDocumento schedaCorrente, DocsPaWR.Registro[] userRegistri)
        {
            try
            {
                bool result = false;
                if (schedaCorrente != null && schedaCorrente.registro != null & schedaCorrente.protocollo != null)
                {
                    if (userRegistri != null && userRegistri.Length > 0)
                    {
                        if (schedaCorrente.registro != null)
                        {
                            foreach (DocsPaWR.Registro rep in userRegistri)
                            {
                                if (rep.systemId.Equals(schedaCorrente.registro.systemId))
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
                if (schedaCorrente != null && schedaCorrente.registro == null)
                {
                    result = true;
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static Utente GetUtenteByIdPeople(string idPeople)
        {
            try
            {
                return docsPaWS.getUtenteById(idPeople);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string[] GetUserInternalAOO(string idPeople, string idRegister)
        {
            try
            {
                return docsPaWS.getUtenteInternoAOO(idPeople, idRegister, UIManager.UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// ritorna un oggetto null, se il corrispondente non osiste o è disabilitato
        /// </summary>
        /// <param name="page"></param>
        /// <param name="codice"></param>
        /// <param name="tipoIE"></param>
        /// <returns></returns>
        public static DocsPaWR.Corrispondente getCorrispondenteByCodRubricaIENotdisabled(Page page, string codice, DocsPaWR.AddressbookTipoUtente tipoIE)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteByCodRubricaIENotDisabled(codice, tipoIE, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Corrispondente getCorrispondenteByIdPeople(Page page, string idPeople, DocsPaWR.AddressbookTipoUtente tipoIE)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteByIdPeople(idPeople, tipoIE, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Corrispondente getCorrispondenteCompletoBySystemId(Page page, string systemId, DocsPaWR.AddressbookTipoUtente tipoIE)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteCompletoBySystemId(systemId, tipoIE, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getRuoloRespUoFromUo(Page page, string id_Uo, string tipoRuolo, string idCorr)
        {
            try
            {
                return docsPaWS.AddressbookGetRuoloRespUoFromUo(id_Uo, tipoRuolo, idCorr);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return "-1";
            }
        }

        public static Corrispondente[] getListaCorrispondenti(Page page, AddressbookQueryCorrispondente qco)
        {
            try
            {
                Corrispondente[] result = docsPaWS.AddressbookGetListaCorrispondenti(qco);

                //if (result == null)
                //{
                //    throw new Exception();
                //}
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Ruolo[] getRuoliRiferimentoAutorizzati(Page page, DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca, DocsPaWR.UnitaOrganizzativa uo)
        {
            try
            {
                return docsPaWS.AddressbookGetRuoliRiferimentoAutorizzati(qca, uo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.Corrispondente getCorrispondenteByCodRubricaIE(Page page, string codice, DocsPaWR.AddressbookTipoUtente tipoIE)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteByCodRubricaIE(codice, tipoIE, UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void setQueryRubricaCaller(ref DocsPaWR.ParametriRicercaRubrica qco)
        {
            try
            {
                qco.caller = new DocsPaWR.RubricaCallerIdentity();
                Ruolo role = UserManager.GetSelectedRole();
                Utente user = UserManager.GetUserInSession();
                Registro registry = UserManager.getRegisterSelected();

                if (role != null)
                {
                    qco.caller.IdRuolo = role.systemId;
                }

                if (user != null)
                {
                    //qco.caller.IdUtente = user.systemId;
                    qco.caller.IdUtente = user.idPeople; // serve esclusivamente nella gestione delle liste che fa riferimento all'idPeople invece del systemId
                }

                if (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_MANAGE
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO
                    || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE)
                {
                    qco.caller.IdRegistro = null;
                }
                else
                {
                    if (registry != null)
                    {
                        qco.caller.IdRegistro = registry.systemId;
                    }

                }

                if ((qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_TRASM_ALL) ||
                (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_TRASM_INF) ||
                (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_TRASM_SUP) ||
                (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_TRASM_PARILIVELLO))
                {
                    if (HttpContext.Current.Session["AddressBook.from"] != null && HttpContext.Current.Session["AddressBook.from"].ToString().Equals("T_N_R_S"))
                    {
                        if (HttpContext.Current.Session["AddressBook.type"] != null)
                        {
                            if (HttpContext.Current.Session["AddressBook.type"].Equals("D"))
                            {
                                SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();
                                if (doc != null && !string.IsNullOrEmpty(doc.systemId))
                                {
                                    qco.ObjectType = doc.tipoProto;
                                }
                                else
                                {
                                    qco.ObjectType = "";
                                }
                            }
                            else
                            {
                                if (HttpContext.Current.Session["AddressBook.type"].Equals("F"))
                                {
                                    Fascicolo prj = UIManager.ProjectManager.getProjectInSession();
                                    if (prj != null && !string.IsNullOrEmpty(prj.systemID))
                                    {
                                        qco.ObjectType = "F:" + prj.idClassificazione;
                                    }
                                    else
                                    {
                                        qco.ObjectType = "F:";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static ElementoRubrica[] getElementiRubrica(Page page, ParametriRicercaRubrica qco)
        {
            try
            {
                return getElementiRubrica(page, qco, null);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ElementoRubrica[] getElementiRubrica(Page page, ParametriRicercaRubrica qco, DocsPaWR.SmistamentoRubrica smistamentoRubrica)
        {
            try
            {
                return docsPaWS.rubricaGetElementiRubrica(qco, UserManager.GetInfoUser(), smistamentoRubrica);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ElementoRubrica[] getElementiRubrica(Page page, ParametriRicercaRubrica qco, int rowStart, int maxRowForPage, out int totale)
        {
            DocsPaWR.SmistamentoRubrica smistamentoRubrica = null;
            totale = 0;
            try
            {
                return docsPaWS.rubricaGetElementiRubricaPaging(qco, UserManager.GetInfoUser(), smistamentoRubrica, rowStart, maxRowForPage, out totale);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ElementoRubrica getElementoRubrica(Page page, string cod)
        {
            try
            {
                return getElementoRubrica(page, cod, null, null);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ElementoRubrica getElementoRubrica(Page page, string cod, string condRegistri)
        {
            try
            {
                return getElementoRubrica(page, cod, null, condRegistri);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ElementoRubrica getElementoRubrica(Page page, string cod, DocsPaWR.SmistamentoRubrica smistaRubrica, string condregistri)
        {
            try
            {
                return docsPaWS.rubricaGetElementoRubrica(cod, UserManager.GetInfoUser(), smistaRubrica, condregistri);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ArrayList getCorrispondentiByCodLista(Page page, string codiceLista, string idAmm)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                return new ArrayList(docsPaWS.getCorrispondentiByCodLista(codiceLista, idAmm, infoUtente));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ArrayList getCorrispondentiByCodRF(Page page, string codiceRF)
        {
            try
            {
                return new ArrayList(docsPaWS.getCorrispondentiByCodRF(codiceRF));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool isUserDisabled(string codiceRubrica, string idAmm)
        {
            try
            {
                return docsPaWS.isUserDisabled(codiceRubrica, idAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static Ruolo[] GetRuoliUtenteByIdCorr(string idCorr)
        {
            try
            {
                return docsPaWS.GetListaRuoliUtenteByIdCorr(idCorr);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string[] GetUoInterneAoo()
        {
            try
            {
                return docsPaWS.GetUoInterneAoo(UIManager.RegistryManager.GetRegistryInSession().systemId, UIManager.UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string[] getUtenteInternoAOO(string idPeople, string systemIdRegistro)
        {
            try
            {
                return docsPaWS.getUtenteInternoAOO(idPeople, systemIdRegistro, UIManager.UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Restituisce la lista dei 'destinatari' / 'destinatari in cc' associati ad un documento con il relativo canale preferenziale
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="typeDest">Può assumere D(estrai le info sui destinatari), C(estrati le info sui destinatari in CC)</param>
        /// <returns>List</returns>
        public static List<Corrispondente> GetPrefChannelAllDest(string idProfile, string typeDest)
        {
            try
            {
                return new List<Corrispondente>(docsPaWS.GetPrefChannelAllDest(idProfile, typeDest));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool esisteCorrispondente(Corrispondente[] lista, Corrispondente corr)
        {
            try
            {
                if (corr.systemId != null)
                {
                    if (lista != null)
                    {
                        for (int i = 0; i < lista.Length; i++)
                        {
                            if (lista[i] != null && lista[i].systemId != null)
                                if (lista[i].systemId.Equals(corr.systemId))
                                    return true;
                        }
                    }
                }
                else
                {
                    if (lista != null)
                    {
                        for (int i = 0; i < lista.Length; i++)
                        {

                            if (lista[i].descrizione.ToUpper().Equals(corr.descrizione.ToUpper()))
                                return true;
                        }
                    }
                }
                return false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static Corrispondente[] addCorrispondente(Corrispondente[] lista, Corrispondente corr)
        {
            try
            {
                Corrispondente[] nuovaLista;
                if (lista != null)
                {
                    //Per le liste di ditribuzione
                    if (corr.tipoCorrispondente == "L")
                    {
                        string idAmm = UserManager.GetInfoUser().idAmministrazione;
                        ArrayList lsCorr = UserManager.getCorrispondentiByCodLista(new Page(), corr.codiceRubrica, idAmm);
                        System.Object[] l_objects = new System.Object[0];


                        for (int i = 0; i < lsCorr.Count; i++)
                        {
                            Corrispondente c = (Corrispondente)lsCorr[i];

                            if (!UserManager.esisteCorrispondente(lista, c))
                            {
                                l_objects = utils.addToArray(l_objects, c);
                            }
                        }

                        nuovaLista = new Corrispondente[l_objects.Length + lista.Length];
                        lista.CopyTo(nuovaLista, 0);
                        l_objects.CopyTo(nuovaLista, lista.Length);

                    }
                    else
                    {
                        int len = lista.Length;
                        nuovaLista = new Corrispondente[len + 1];
                        lista.CopyTo(nuovaLista, 0);
                        nuovaLista[len] = corr;
                    }
                }
                else
                {
                    //Per le liste di ditribuzione
                    if (corr.tipoCorrispondente == "L")
                    {
                        string idAmm = UserManager.GetInfoUser().idAmministrazione;
                        ArrayList lsCorr = UserManager.getCorrispondentiByCodLista(new Page(), corr.codiceRubrica, idAmm);

                        System.Object[] l_objects = new System.Object[0];

                        for (int i = 0; i < lsCorr.Count; i++)
                        {
                            Corrispondente c = (Corrispondente)lsCorr[i];

                            if (!UserManager.esisteCorrispondente(lista, c))
                            {
                                l_objects = utils.addToArray(l_objects, c);
                            }
                        }

                        nuovaLista = new Corrispondente[lsCorr.Count];
                        l_objects.CopyTo(nuovaLista, 0);

                    }
                    else
                    {
                        nuovaLista = new Corrispondente[1];
                        nuovaLista[0] = corr;
                    }

                }
                return nuovaLista;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Corrispondente[] removeCorrispondente(Corrispondente[] lista, int index)
        {
            try
            {
                if (lista == null || lista.Length < index)
                    return lista;

                if (lista.Length == 1)
                    return null;

                Corrispondente[] nuovaLista = null;
                if (lista != null && lista.Length > 0)
                {
                    for (int i = 0; i < lista.Length; i++)
                    {
                        if (i != index)
                            nuovaLista = addCorrispondente(nuovaLista, lista[i]);
                    }
                }
                return nuovaLista;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        #region gestione OGGETTI SESSIONE
        /// Impostazione valore in sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionValue"></param>
        private static void SetSessionValue(string sessionKey, object sessionValue)
        {
            try
            {
                System.Web.HttpContext.Current.Session[sessionKey] = sessionValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Reperimento valore da sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private static Object GetSessionValue(string sessionKey)
        {
            try
            {
                return System.Web.HttpContext.Current.Session[sessionKey];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Rimozione chiave di sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        private static void RemoveSessionValue(string sessionKey)
        {
            try
            {
                System.Web.HttpContext.Current.Session.Remove(sessionKey);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        #endregion gestione OGGETTI SESSIONE

        public static Registro[] GetRegistriByRuolo(Page page, string idRuolo)
        {
            try
            {
                return docsPaWS.GetListaRegistriByRuolo(idRuolo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Utente getUtente(System.Web.SessionState.HttpSessionState session)
        {
            try
            {
                //Laura 9 Aprile modificato perchè nel caso in cui la chiave non è presente nella ricerca rischio di non avere risultati
                //ABBATANGELI GIANLUIGI - Aggiunta l'informazione che indica l'applicazione su cui sta lavorando l'utente
                Utente utente = session["userData"] as Utente;
                if (utente != null)
                    if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]))
                        utente.codWorkingApplication = System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"].ToString();
                return utente;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Registro getRegistroSelezionato(Page page)
        {
            try
            {
                return (Registro)GetSessionValue("userRegistro");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool disabilitaButtHMDiritti(string accessRights)
        {
            try
            {
                return (Convert.ToInt32(accessRights) < Convert.ToInt32(Utils.HMdiritti.HMdiritti_Write) || Convert.ToInt32(accessRights) < Convert.ToInt32(Utils.HMdiritti.HMdiritti_Eredita));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool disabilitaButtHMDirittiTrasmInAccettazione(string accessRights)
        {
            try
            {
                return (Convert.ToInt32(accessRights) < Convert.ToInt32(Utils.HMdiritti.HMdiritti_Read));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static string getNomeRF(Page page, string codiceRF)
        {
            try
            {
                return docsPaWS.getNomeRF(codiceRF);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getNomeLista(Page page, string codiceLista, string idAmm)
        {
            try
            {
                return docsPaWS.getNomeLista(codiceLista, idAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// True se l'utente non ha i diritti di scrittura o ereditarietà sul documento in sessione
        /// </summary>
        /// <returns></returns>
        public static bool IsRightsWritingInherits(SchedaDocumento schDoc = null)
        {
            bool result = false;
            SchedaDocumento documentTab = null;
            try
            {
                documentTab = (schDoc != null) ? schDoc : DocumentManager.getSelectedRecord();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            //verifica delle autorizzazioni di HM sul documento
            if (documentTab != null && 
                !string.IsNullOrEmpty(documentTab.accessRights) && 
                !((Convert.ToInt32(documentTab.accessRights) < (int)NttDataWA.Utils.HMdiritti.HMdiritti_Write) ||
                (Convert.ToInt32(documentTab.accessRights) < (int)NttDataWA.Utils.HMdiritti.HMdiritti_Eredita))
            )
                result = true;
            return result;
        }

        public static bool EnableNewTransmission(SchedaDocumento schDoc = null)
        {
            bool result = false;
            SchedaDocumento documentTab = null;
            try
            {
                documentTab = (schDoc != null) ? schDoc : DocumentManager.getSelectedRecord();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            //verifica delle autorizzazioni di HM sul documento
            if (documentTab != null && 
                !string.IsNullOrEmpty(documentTab.accessRights) && 
                !((Convert.ToInt32(documentTab.accessRights) < (int)NttDataWA.Utils.HMdiritti.HMdiritti_Read))
            )
                result = true;
            return result;
        }

        public static bool EnableNewTransmissionProject()
        {
            bool result = false;
            Fascicolo prj = null;
            try
            {
                prj = ProjectManager.getProjectInSession();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            //verifica delle autorizzazioni di HM sul fascicolo
            if (prj != null &&
                !string.IsNullOrEmpty(prj.accessRights) &&
                !((Convert.ToInt32(prj.accessRights) < (int)NttDataWA.Utils.HMdiritti.HMdiritti_Read))
            )
                result = true;
            return result;
        }

        public static NttDataWA.DocsPaWR.Amministrazione[] getListaAmministrazioni(Page page, out string returnMsg)
        {
            returnMsg = string.Empty;

            try
            {
                return docsPaWS.amministrazioneGetAmministrazioni(out returnMsg);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Restituisce il nome della stampante delle etichette utilizzata dall'utente.
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static string getDispositivoStampaUtente(string idPeople)
        {
            string retval = null;
            try
            {
                DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                retval = ws.GetDispositivoStampaUtente(idPeople);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return retval;
        }

        public static NttDataWA.DocsPaWR.Configurazione getParametroConfigurazione(Page page)
        {
            try
            {
                return docsPaWS.amministrazioneGetParametroConfigurazione();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Metodo per recuperare quale tipo di componenti utilizza l'utente
        /// </summary>
        /// <param name="brObject">Il browser utilizzato dall'utente. Se non è Internet Explorer ritorna 3 (APPLET)</param>
        /// <returns>Il tipo di componenti utilizzati: 
        /// 0 - non configurati
        /// 1 - activeX
        /// 2 - Smart Client
        /// 3- Applet
        /// </returns>
        public static string getComponentType(string userAgent) // HttpBrowserCapabilities brObject)           
        {
            try
            { 
                string retval = Constans.TYPE_ACTIVEX;

                DocsPaWR.DocsPaWebService ws = NttDataWA.Utils.ProxyManager.GetWS();
                DocsPaWR.SmartClientConfigurations smcConf = ws.GetSmartClientConfigurationsPerUser(UserManager.GetInfoUser());
                retval = smcConf.ComponentsType;
                // Controllo se sono attivi smartclient o activex su browser diversi da IE e imposto SOCKET nel caso      
                // Necessario cercare "Trident" nella stringa per IE11+
                if (!(userAgent.Contains("MSIE") || userAgent.Contains("Trident")) && !(retval.Equals(Constans.TYPE_APPLET) || retval.Equals(Constans.TYPE_SOCKET)))
                {
                    retval = Constans.TYPE_APPLET;
                }

                return retval;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string getStatoRegistro(Registro reg)
        {
            // R = Rosso -  CHIUSO
            // V = Verde -  APERTO
            // G = Giallo - APERTO IN GIALLO
            try
            {
                string dataApertura = reg.dataApertura;

                if (!dataApertura.Equals(""))
                {

                    DateTime dt_cor = DateTime.Now;

                    CultureInfo ci = new CultureInfo("it-IT");

                    string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };

                    DateTime d_ap = DateTime.ParseExact(dataApertura, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                    //aggiungo un giorno per fare il confronto con now (che comprende anche minuti e secondi)
                    d_ap = d_ap.AddDays(1);

                    string mydate = dt_cor.ToString(ci);

                    //DateTime dt = DateTime.ParseExact(mydate,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);
                   
                    string StatoAperto = System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.STATO_REG_APERTO.ToString()];
                    if (reg.stato.Equals(StatoAperto))
                    {
                        if (dt_cor.CompareTo(d_ap) > 0)
                        {
                            //data odierna maggiore della data di apertura del registro
                            return "G";
                        }
                        else
                            return "V";
                    }
                }
                return "R";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ElementoRubrica[] GetRuoliUtente(Page page, string id_amm, string cod_rubrica)
        {
            try
            {
                return docsPaWS.AddressbookGetRuoliUtente(id_amm, cod_rubrica);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool DeleteModifyCorrispondenteEsterno(Page page, DatiModificaCorr datiModifica, int flagListe, string action, out string message)
        {
            message = String.Empty;
            try
            {
                return docsPaWS.CorrispondentiDeleteModifyCorrispondenteEsterno(UserManager.GetInfoUser(), datiModifica, flagListe, action, out message);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool DeleteModifyCorrispondenteEsterno(Page page, DatiModificaCorr datiModifica, int flagListe, string action, out string newIdCorr, out string message)
        {
            message = String.Empty;
            newIdCorr = String.Empty;
            try
            {
                return docsPaWS.CorrispondentiDeleteModifyCorrispondenteEsternoWithId(UserManager.GetInfoUser(), datiModifica, flagListe, action, out newIdCorr, out message);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static DocsPaWR.Corrispondente addressbookInsertCorrispondente(Page page, DocsPaWR.Corrispondente newCorr, DocsPaWR.Corrispondente parentCorr)
        {
            try
            {
                InfoUtente iu = GetInfoUser();
                DocsPaWR.Corrispondente result = docsPaWS.AddressbookInsertCorrispondente(newCorr, parentCorr, iu);
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Controlla che la sessione corrisponda ad un utente connesso
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static DocsPaWR.ValidationResult ValidateLogin(string userName, string idAmm, string sessionId)
        {
            DocsPaWR.ValidationResult result = DocsPaWR.ValidationResult.APPLICATION_ERROR;

            try
            {
                result = docsPaWS.ValidateLogin(userName, idAmm, sessionId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                result = DocsPaWR.ValidationResult.APPLICATION_ERROR;
            }

            return result;
        }

        public static Registro[] getListaRegistriNoFiltroAOO(out bool filtroAOO)
        {
            filtroAOO = false;
            Registro[] result = null;
            try
            {
                result = docsPaWS.UtenteGetRegistriNoFiltroAOO(GetInfoUser().idAmministrazione, out filtroAOO);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return result;
        }
        public static void removeListaIdRegistri(Page page)
        {
            page.Session.Remove("rubrica.listaIdRegistri");
        }
        public static string[] getListaIdRegistriUtente(Page page)
        {
            return getListaIdRegistri(page, getListaRegistri(page));
        }

 public static DocsPaWR.Corrispondente[] getAllRuoli(DocsPaWR.Corrispondente[] corrispondenti, bool cameFromRubrica)
        {
            string l_oldSystemId = "";
            System.Object[] l_objects = new System.Object[0];
            System.Object[] l_objects_ruoli = new System.Object[0];
            DocsPaWR.Ruolo[] lruolo = new DocsPaWR.Ruolo[0];
            int i = 0;
            foreach (DocsPaWR.Corrispondente t_corrispondente in corrispondenti)
            {
                string t_systemId = t_corrispondente.systemId;
                if (t_systemId != l_oldSystemId)
                {
                    l_objects = utils.addToArray(l_objects, t_corrispondente);
                    l_oldSystemId = t_systemId;
                    i = i + 1;
                    continue;
                }
                else
                {
                    /* il corrispondente non viene aggiunto, in quanto sarebbe un duplicato 
                     * ma viene aggiunto solamente il ruolo */

                    if (t_corrispondente.GetType().Equals(typeof(DocsPaWR.Utente)))
                    {
                        if (!cameFromRubrica)
                        {
                            if ((l_objects[i - 1]).GetType().Equals(typeof(DocsPaWR.Utente)))
                            {
                                l_objects_ruoli = ((utils.addToArray(((DocsPaWR.Utente)(l_objects[i - 1])).ruoli, ((DocsPaWR.Utente)t_corrispondente).ruoli[0])));
                                DocsPaWR.Ruolo[] l_ruolo = new DocsPaWR.Ruolo[l_objects_ruoli.Length];
                                ((DocsPaWR.Utente)(l_objects[i - 1])).ruoli = l_ruolo;
                                l_objects_ruoli.CopyTo(((DocsPaWR.Utente)(l_objects[i - 1])).ruoli, 0);

                            }
                        }

                    }
                }

            }

            DocsPaWR.Corrispondente[] l_corrSearch = new DocsPaWR.Corrispondente[l_objects.Length];
            l_objects.CopyTo(l_corrSearch, 0);

            return l_corrSearch;
        }

        private static string[] getListaIdRegistri(Page page, Registro[] registri)
        {
            int numRegistri = registri.Length;
            string[] id = new string[numRegistri];
            for (int i = 0; i < numRegistri; i++)
                id[i] = registri[i].systemId;
            return id;
        }
        public static Registro[] getListaRegistri(Page page)
        {
            Registro[] result = null;
            try
            {
                result = docsPaWS.UtenteGetRegistri(UserManager.GetInfoUser().idCorrGlobali);

                if (result == null)
                {
                    throw new Exception();
                }
            }           
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            return result;
        }

        // Modifica Tatiana per la ricerca Visibilità
        public static int getIdProfileByData(DocsPaWR.InfoUtente infoUtente, string numProto, string AnnoProto, string idRegistro, out string inArchivio)
        {
            inArchivio = "-1";
            try
            {
                return docsPaWS.DO_getIdProfileByData(numProto, AnnoProto, idRegistro, infoUtente, out inArchivio);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return -1;
            }

        }
        // end Modifica Tatiana per la ricerca Visibilità

        public static bool isRFEnabled()
        {
            bool result = false;
            try
            {
                result = docsPaWS.isRFEnabled();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            return result;

        }

        //Emilio Modelli Trasmissione

        public static OrgRegistro[] getRegistriByCodAmm(string codAmm, string codChaRF)
        {
            OrgRegistro[] result = null;
            try
            {
                result = docsPaWS.AmmGetRegistri(codAmm, codChaRF);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }

            return result;
        }

        //Emilio Modelli Trasmissione
        public static ArrayList getModelliUtente(Page page, NttDataWA.DocsPaWR.Utente utente, NttDataWA.DocsPaWR.InfoUtente infoUt, DocsPaWR.FiltroRicerca[] filtriRicerca)
        {
            try
            {
                //Veronica
                return new ArrayList(docsPaWS.getModelliUtente(utente, infoUt, filtriRicerca));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        //Emilio Modelli Trasmissione
        public static NttDataWA.DocsPaWR.ModelloTrasmissione getModelloByID(Page page, string idAmm, string idModello)
        {
            try
            {
                return docsPaWS.getModelloByID(idAmm, idModello);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        # region Necessario alla firma con SmartClient;
        public static string getBoolDocSalva(Page page)
        {
            return (string)page.Session["docsalva.bool"];
        }

        public static void setBoolDocSalva(Page page, string salvato)
        {
            page.Session["docsalva.bool"] = salvato;
        }
        public static void removeBoolDocSalva(Page page)
        {
            page.Session.Remove("docsalva.bool");
        }
        
        # endregion
        
        /// <summary>
        /// Normalizzazione valore per una proprietà stringa
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLenght"></param>
        /// <returns></returns>
        public static string normalizeStringPropertyValue(string value)
        {

            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();
                value = value.Replace("/", string.Empty);
                value = value.Replace(System.Environment.NewLine, string.Empty);
                value = value.Replace("\n", string.Empty);
                value = value.Replace("|", string.Empty);
            }

            return value;
        }

        public static void setDelegato(InfoUtente infoUtente)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["userDelegato"] = infoUtente;
        }

        public static void setUtenteDelegato(Page page, Utente utente)
        {
            if (utente!=null)
                utente.urlWA = utils.getHttpFullPath();
            page.Session["userDataDelegato"] = utente;
        }

        public static Utente getUtenteDelegato()
        {
            return (Utente)HttpContext.Current.Session["userDataDelegato"];
        }

        public static InfoUtente getDelegato()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session!=null && HttpContext.Current.Session["userDelegato"] != null)
            {
                return HttpContext.Current.Session["userDelegato"] as InfoUtente;
            }
            else
            {
                return null;
            }
          
        }

        public static ElementoRubrica[] filtra_trasmissioniPerListe(Page page, ParametriRicercaRubrica qco, DocsPaWR.ElementoRubrica[] ers)
        {
            try
            {
                return docsPaWS.filtra_trasmissioniPerListe(qco, UserManager.GetInfoUser(), ers);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        //Emilio modelli Trasmissione
        public static void cancellaModello(Page page, string idAmministrazione, string idModello)
        {
            try
            {
                docsPaWS.CancellaModello(idAmministrazione, idModello);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static string getCodiceLista(string idLista)
        {
            try
            {
                return docsPaWS.getCodiceLista(idLista);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return string.Empty;
            }
        }

        public static DataSet getCorrispondentiLista(string codiceLista)
        {
            try
            {
                return docsPaWS.getCorrispondentiLista(codiceLista);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void deleteListaDistribuzione(string codiceLista)
        {
            try
            {
                docsPaWS.deleteListaDistribuzione(codiceLista);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        //Inizio Laura 9 Maggio
        public static DataSet getListePerRuoloUt()
        {
            try
            {
                return docsPaWS.getListePerRuoloUt(UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool isUniqueCodLista(string codLista, string idAmm)
        {
            try
            {
                return docsPaWS.isUniqueCodLista(codLista, idAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool isUniqueCod(string codLista, string idAmm)
        {
            try
            {
                return docsPaWS.isUniqueCod(codLista, idAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }


        public static bool isUniqueNomeLista(string nomeLista)
        {
            try
            {
                return docsPaWS.isUniqueNomeLista(nomeLista, UserManager.GetInfoUser().idAmministrazione);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static void salvaLista (DataSet dsCorrLista, string nomeLista, string codiceLista, string idUtente, string idAmm, string gruppo)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                docsPaWS.salvaListaGruppo(dsCorrLista, nomeLista, codiceLista, idUtente, idAmm, gruppo, infoUtente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void modificaLista(DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista)
        {
            try
            {
                docsPaWS.modificaLista(dsCorrLista, idLista, nomeLista, codiceLista);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void modificaLista(DataSet dsCorrLista, string idLista)
        {
            try
            {
                docsPaWS.modificaListaCorr(dsCorrLista, idLista);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static void modificaListaGruppo(DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista, string idGruppo)
        {
            try
            {
                docsPaWS.modificaListaGruppo(dsCorrLista, idLista, nomeLista, codiceLista, idGruppo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static string getRuoloOrUserLista(string idLista)
        {
            try
            {
                return docsPaWS.getRuoloOrUserLista(idLista);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return string.Empty;
            }
        }

        public static void modificaListaUser(DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista, string idUtente)
        {
            try
            {
                docsPaWS.modificaListaUser(dsCorrLista, idLista, nomeLista, codiceLista, idUtente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        //Tatiana 16/05/2013
        #region Report Corrispondenti
        public static DocsPaWR.FileDocumento reportCorrispondenti(Page page, string tipo, DocsPaWR.Registro registro)
        {
            try
            {
                //costruzione oggetto queryCorrispondente
                AddressbookQueryCorrispondente qco = new AddressbookQueryCorrispondente();
                qco.fineValidita = true;
                InfoUtente infoUtente = UserManager.GetInfoUser();
                switch (tipo)
                {
                    case "I": qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO; break;
                    case "E": qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.ESTERNO; break;
                    default: qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.GLOBALE; break;
                }
                qco.idAmministrazione = infoUtente.idAmministrazione;
                if (registro != null)
                {
                    qco.idRegistri = new String[1];
                    qco.idRegistri[0] = registro.systemId;
                }
                else
                    qco.idRegistri = getListaIdRegistri(page);

                DocsPaWR.FileDocumento result = docsPaWS.ReportCorrispondenti(qco, infoUtente);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return null;
        }
        #endregion Report Corrispondenti

        //Emilio modelli trasmissione
        public static bool ruoloIsAutorized(Page page, string tipologia)
        {
            DocsPaWR.Ruolo userRuolo = null;
            if (page == null)
                userRuolo = RoleManager.GetRoleInSession();
            else
                userRuolo = RoleManager.GetRoleInSession();
            if (userRuolo == null || userRuolo.funzioni == null)
                return false;
            /*-*/
            bool trovato = false;
            for (int i = 0; i < userRuolo.funzioni.Length; i++)
            {
                if (userRuolo.funzioni[i].codice.Equals(tipologia))
                {
                    trovato = true;
                    break;
                }
            }
            return trovato;
        }

        // MEV Utente Multi Amministrazione
        /// <summary>
        /// Lista Amministrazione pewr Utente
        /// </summary>
        public static DocsPaWR.Amministrazione[] getListaAmministrazioniByUser(Page page, string userId, bool controllo, out string returnMsg)
        {
            returnMsg = string.Empty;

            try
            {
                return docsPaWS.amministrazioneGetAmministrazioniByUser(userId,controllo, out returnMsg);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return null;
        }

        /// <summary>
        /// Dato l'id dell'amministrazione, il metodo restituisce le informazioni relative all'Amministrazione 
        /// </summary>
        /// <param name="idAmm">id amministrazione</param>
        /// <returns></returns>
        public static InfoAmministrazione getInfoAmmCorrente(string idAmm)
        {
            InfoAmministrazione result = null;

            try
            {
                result = docsPaWS.AmmGetInfoAmmCorrente(idAmm);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return result;
        }

        public static bool FunzioneEsistente(string codiceFunzione)
        {
            bool retValue = false;
            try
            {
                retValue = docsPaWS.FunzioneEsistente(codiceFunzione);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return retValue;
        }

        public static Ruolo getRuolo(System.Web.SessionState.HttpSessionState session)
        {
            return session["userRuolo"] as Ruolo;
        }

        public static List<Utente> getUserInRoleByIdCorrGlobali(string idRole)
        {
            return docsPaWS.getUserInRoleByIdCorrGlobali(idRole).ToList<Utente>();
        }

        public static List<Utente> getUserInRoleByIdGruppo(string idRole)
        {
            return docsPaWS.getUserInRoleByIdGruppo(idRole).ToList<Utente>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static InfoUtente getInfoUtente(System.Web.SessionState.HttpSessionState session)
        {
            Utente utente = getUtente(session);
            Ruolo ruolo = getRuolo(session);

            InfoUtente infoUtente = new InfoUtente();

            try
            {
                if (utente != null)
                {
                    infoUtente.idPeople = utente.idPeople;
                    infoUtente.dst = utente.dst;
                    infoUtente.idAmministrazione = utente.idAmministrazione;
                    infoUtente.userId = utente.userId;
                    infoUtente.sede = utente.sede;
                    if (utente.urlWA != null)
                        infoUtente.urlWA = utente.urlWA;
                    infoUtente.delegato = getDelegato();

                    //ABBATANGELI GIANLUIGI - Gestione delle applicazioni esterne
                    infoUtente.extApplications = utente.extApplications;
                    //ABBATANGELI GIANLUIGI - Aggiunta l'informazione che indica l'applicazione su cui sta lavorando l'utente
                    infoUtente.codWorkingApplication = (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]) ? "___" : System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]);
                }

                if (ruolo != null)
                {
                    infoUtente.idCorrGlobali = ruolo.systemId;
                    infoUtente.idGruppo = ruolo.idGruppo;
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("Impossibile accedere alle informazioni dell'utente" + exception.ToString());
                infoUtente = null;
            }

            return infoUtente;
        }

        /// <summary>
        /// Get the CHA_TIPO_FIRMA in PEOPLE ROW TABLE
        /// </summary>
        /// <param name="idPeople)">logged user system id </param>
        /// <returns>cha_tipo_firma</returns>
        public static string GetSignTypePreference(string idPeople)
        {
            return docsPaWS.GetSignTypePreference(idPeople);
        }

        /// <summary>
        /// Get the CHA_TIPO_FIRMA in PEOPLE ROW TABLE
        /// </summary>
        /// <param name="idPeople)">logged user system id </param>
        /// <returns>cha_tipo_firma</returns>
        public static void SetSignTypePreference(string idPeople, string chaPreference)
        {
            docsPaWS.SetSignTypePreference(idPeople, chaPreference);
        }

        /// <summary>
        /// INTEGRAZIONE PITRE-PARER
        /// MEV Policy e responsabile conservazione
        /// Recupera l'id ruolo del responsabile conservazione
        /// </summary>
        /// <returns></returns>
        public static string GetIdRuoloRespConservazione()
        {
            return docsPaWS.GetIdRuoloRespConservazione(UserManager.GetInfoUser().idAmministrazione);
        }

        /// <summary>
        ///LIBRO FIRMA
        ///Utente automatico ulizzato per i passi automatici di Libro Firma
        /// </summary>
        /// <returns></returns>
        public static DocsPaWR.Utente GetUtenteAutomatico()
        {
            try
            {
                return docsPaWS.GetUtenteAutomatico(UserManager.GetInfoUser().idAmministrazione);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
    }
}
