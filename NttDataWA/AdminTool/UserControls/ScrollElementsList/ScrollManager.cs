using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using SAAdminTool.utils;

namespace SAAdminTool.UserControls.ScrollElementsList
{
    public class ScrollManager
    {
        public enum ScrollDirection
        {
            NEXT,
            PREV,
            NO_DIRECTION
        }

        public static bool enableScrollElementsList()
        {
            //Verifico se è abilitata la funzionalità
            if (System.Configuration.ConfigurationManager.AppSettings["ENABLE_SCROLL_ELEMENTS_LIST"] != null && System.Configuration.ConfigurationManager.AppSettings["ENABLE_SCROLL_ELEMENTS_LIST"] == "1")
            {
                //Verifico se provengo da una ricerca 
                if (SiteNavigation.CallContextStack.CallerContext != null)
                {
                    string stringContext = SiteNavigation.CallContextStack.CallerContext.ContextName;
                    
                    //Vengo da una ricerca documenti
                    if (stringContext == SiteNavigation.NavigationKeys.RICERCA_DOCUMENTI && 
                        getFromContextObjScrollElementsList() != null &&
                        getFromContextObjScrollElementsList().searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_DOCUMENTI &&
                        DocumentManager.getRisultatoRicerca(null) != null)
                    {
                        // Reperimento dell'indice occupato dal documento selezionato
                        int index = SearchUtils.GetIndexOfSelectedDocument(getFromContextObjScrollElementsList());
                        // Salvataggio dell'indice nel contesto del navigatore
                        getFromContextObjScrollElementsList().selectedElement = index;

                        return true;
                    }


                    /* Anomalia INPS - MCaropreso
                     * Viene mostrato lo scroll anche se i documenti trovati sono 0.
                     * Aggiunto controllo sul numero risultati
                     */

                    //Vengo da una ricerca documenti in area di lavoro
                    if (stringContext == SiteNavigation.NavigationKeys.RICERCA_DOCUMENTI_ADL &&
                        getFromContextObjScrollElementsList() != null &&
                        getFromContextObjScrollElementsList().searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_DOCUMENTI &&
                        DocumentManager.getRisultatoRicerca(null) != null)
                    {
                        return true;
                    }

                    //Vengo da una ricerca documenti in fascicolo
                    if (stringContext == SiteNavigation.NavigationKeys.FASCICOLO &&
                        getFromContextObjScrollElementsList() != null &&
                        getFromContextObjScrollElementsList().searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_DOC_IN_FASC &&
                        DocumentManager.getRisultatoRicerca(null) != null)
                    {
                        // Reperimento dell'indice occupato dal documento selezionato
                        int index = SearchUtils.GetIndexOfSelectedDocument(getFromContextObjScrollElementsList());
                        // Salvataggio dell'indice nel contesto del navigatore
                        getFromContextObjScrollElementsList().selectedElement = index;

                        return true;
                    }

                    //Vengo da una ricerca trasmissioni (TO_DO_LIST)
                    if (stringContext == SiteNavigation.NavigationKeys.PAGINA_INIZIALE &&
                        getFromContextObjScrollElementsList() != null &&
                        (getFromContextObjScrollElementsList().searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST
                        ||
                        getFromContextObjScrollElementsList().searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST))
                    {
                        if (!string.IsNullOrEmpty(SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["idTrasm"].ToString()))
                        {
                            // Reperimento dell'indice occupato dal documento selezionato
                            int index = getFromContextObjScrollElementsList().selectedElement;
                            // Salvataggio dell'indice nel contesto del navigatore
                            getFromContextObjScrollElementsList().selectedElement = index;
                            
                               
                            return true;
                        }
                        
                        //if (SiteNavigation.CallContextStack.CallerContext.QueryStringParameters.Count >= 2)
                        //{
                        //    //Ricerca trasmissioni to_do_list documenti
                        //    if (SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["tipoRic"].ToString().ToUpper() == "D".ToUpper())
                        //    {
                        //        if (!string.IsNullOrEmpty(SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["idTrasm"].ToString()))
                        //        {
                        //            return true;
                        //        }
                        //    }

                        //    //Ricerca trasmissioni to_do_list fascicoli
                        //    if (SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["tipoRic"].ToString().ToUpper() == "F".ToUpper())
                        //    {
                        //        if (!string.IsNullOrEmpty(SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["idTrasm"].ToString()))
                        //        {
                        //            return true;
                        //        }
                        //    }
                        //}
                    }

                    //Vengo da una ricerca trasmissioni
                    if (stringContext == SiteNavigation.NavigationKeys.RICERCA_TRASMISSIONI &&
                        getFromContextObjScrollElementsList() != null &&
                        (getFromContextObjScrollElementsList().searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC
                        ||
                        getFromContextObjScrollElementsList().searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC))
                    {
                        if (SiteNavigation.CallContextStack.CallerContext.SessionState.Count >= 2)
                        {
                            if (SiteNavigation.CallContextStack.CallerContext.SessionState["MemoriaFiltriRicTrasm"] != null)
                            {
                                foreach (DocsPaWR.FiltroRicerca filtro in (DocsPaWR.FiltroRicerca[]) SiteNavigation.CallContextStack.CallerContext.SessionState["MemoriaFiltriRicTrasm"])
                                {
                                    //Ricerca trasmissioni documenti
                                    if (filtro.argomento == "TIPO_OGGETTO" && filtro.valore == "D")
                                    {
                                        return true;
                                    }

                                    //Ricerca trasmissioni fascicoli
                                    if (filtro.argomento == "TIPO_OGGETTO" && filtro.valore == "F")
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }

                    //Vengo da una ricerca fascicoli
                    if (stringContext == SiteNavigation.NavigationKeys.RICERCA_FASCICOLI &&
                        getFromContextObjScrollElementsList() != null &&
                        getFromContextObjScrollElementsList().searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_FASCICOLI &&
                        FascicoliManager.getFascicoloSelezionato() != null)
                    {
                        // Reperimento dell'indice occupato dal fascicolo selezionato
                        int index = SearchUtils.GetIndexOfSelectedProject(getFromContextObjScrollElementsList());
                        // Salvataggio dell'indice nel contesto del navigatore
                        getFromContextObjScrollElementsList().selectedElement = index;
                        
                        return true;
                    }

                    /* Anomalia INPS - MCaropreso
                    * Viene mostrato lo scroll anche se i fascicoli trovati sono 0.
                    * Aggiunto controllo sul numero risultati
                    */

                    //Vengo da una ricerca fascicoli in area di lavoro
                    if (stringContext == SiteNavigation.NavigationKeys.RICERCA_FASCICOLI_ADL &&
                        getFromContextObjScrollElementsList() != null &&
                        getFromContextObjScrollElementsList().searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_FASCICOLI &&
                        FascicoliManager.getFascicoloSelezionato() != null)
                    {
                        return true;
                    }

                }

                //UserControls.ScrollElementsList.ScrollManager.clearSessionObjScrollElementsList();
                UserControls.ScrollElementsList.ScrollManager.clearContextObjScrollElementsList();
                return false;
            }
            else
            {
                return false;
            }
        }

        #region Codice che gestisce lo scroll di elementi nel CONTEXT (tasto back)
        public static void setInContextNewObjScrollElementsList(int totalNumberOfElements, int totalNumberOfPage, int pageSize, int selectedElement, int selectedPage, ArrayList objList, ObjScrollElementsList.EmunSearchContext searchContext)
        {
            ObjScrollElementsList objScrollElementsList = new ObjScrollElementsList();
            objScrollElementsList.totalNumberOfElements = totalNumberOfElements;
            objScrollElementsList.totalNumberOfPage = totalNumberOfPage;
            objScrollElementsList.pageSize = pageSize;
            objScrollElementsList.selectedElement = selectedElement;
            objScrollElementsList.selectedPage = selectedPage;
            objScrollElementsList.objList = objList;
            objScrollElementsList.searchContext = searchContext;

            SiteNavigation.CallContextStack.CurrentContext.objScrollElementsList = objScrollElementsList;
        }

        public static ObjScrollElementsList getFromContextObjScrollElementsList()
        {
            if (SiteNavigation.CallContextStack.CallerContext != null && SiteNavigation.CallContextStack.CallerContext.objScrollElementsList != null)
            {
                return SiteNavigation.CallContextStack.CallerContext.objScrollElementsList;
            }

            return null;
        }

        public static void clearContextObjScrollElementsList()
        {
            if (SiteNavigation.CallContextStack.CurrentContext != null && SiteNavigation.CallContextStack.CurrentContext.objScrollElementsList != null)
            {
                SiteNavigation.CallContextStack.CurrentContext.objScrollElementsList = null;
            }
        }
        #endregion

        #region Codice che gestisce lo scroll di elementi in sessione
        //public static void setInSessionObjScrollElementsList(ObjScrollElementsList objScrollElementsList)
        //{
        //    System.Web.HttpContext.Current.Session.Remove("ObjScrollElementsList");
        //    System.Web.HttpContext.Current.Session.Add("ObjScrollElementsList", objScrollElementsList);
        //}

        //public static void setInSessionNewObjScrollElementsList(int totalNumberOfElements, int totalNumberOfPage, int pageSize, int selectedElement, int selectedPage, ArrayList objList, ObjScrollElementsList.EmunSearchContext searchContext)
        //{
        //    ObjScrollElementsList objScrollElementsList = new ObjScrollElementsList();
        //    objScrollElementsList.totalNumberOfElements = totalNumberOfElements;
        //    objScrollElementsList.totalNumberOfPage = totalNumberOfPage;
        //    objScrollElementsList.pageSize = pageSize;
        //    objScrollElementsList.selectedElement = selectedElement;
        //    objScrollElementsList.selectedPage = selectedPage;
        //    objScrollElementsList.objList = objList;
        //    objScrollElementsList.searchContext = searchContext;            

        //    System.Web.HttpContext.Current.Session.Add("ObjScrollElementsList", objScrollElementsList);
        //}

        //public static ObjScrollElementsList getFromSessionObjScrollElementsList()
        //{
        //    if (System.Web.HttpContext.Current.Session["ObjScrollElementsList"] != null)
        //    {
        //        ObjScrollElementsList objScrollElementsList = (ObjScrollElementsList)System.Web.HttpContext.Current.Session["ObjScrollElementsList"];
        //        return objScrollElementsList;
        //    }

        //    return null;
        //}

        //public static void clearSessionObjScrollElementsList()
        //{
        //    System.Web.HttpContext.Current.Session.Remove("ObjScrollElementsList");
        //}
        #endregion        

        public virtual string move(ObjScrollElementsList objScrollElementsList, Page page, ScrollManager.ScrollDirection scrollDirection)
        {
            return string.Empty;
        }

        public static string refreshPage(DocsPaWR.Fascicolo fasc, Page page)
        {
            string script = string.Empty;

            FascicoliManager.setFascicoloSelezionato(page, fasc);
            SiteNavigation.CallContextStack.CurrentContext.ContextState["systemId"] = fasc.systemID;
            if (fasc.idRegistroNodoTit != null && fasc.idRegistroNodoTit != string.Empty)
            {
                DocsPaWR.Registro registroFascicolo = UserManager.getRegistroBySistemId(page, fasc.idRegistroNodoTit);
                if (registroFascicolo != null)
                {
                    UserManager.setRegistroSelezionato(page, registroFascicolo);
                }
                else
                {
                    UserManager.removeRegistroSelezionato(page);
                }
            }
            script = "top.principale.document.location = 'fascicolo/gestioneFasc.aspx?tab=documenti';";
            return script;
        }

        public static string refreshPage2(DocsPaWR.SearchObject fasc2, Page page)
        {
            string script = string.Empty;
            DocsPaWR.Fascicolo fasc = new DocsPaWR.Fascicolo();
            fasc = FascicoliManager.getFascicoloById(page, fasc2.SearchObjectID);
            FascicoliManager.setFascicoloSelezionato(page, fasc);
            SiteNavigation.CallContextStack.CurrentContext.ContextState["systemId"] = fasc.systemID;
            if (fasc.idRegistroNodoTit != null && fasc.idRegistroNodoTit != string.Empty)
            {
                DocsPaWR.Registro registroFascicolo = UserManager.getRegistroBySistemId(page, fasc.idRegistroNodoTit);
                if (registroFascicolo != null)
                {
                    UserManager.setRegistroSelezionato(page, registroFascicolo);
                }
                else
                {
                    UserManager.removeRegistroSelezionato(page);
                }
            }
            script = "top.principale.document.location = 'fascicolo/gestioneFasc.aspx?tab=documenti';";
            return script;
        }

        public static string refreshPage(SAAdminTool.DocsPaWR.InfoDocumento doc, Page page)
        {
            // Verifica se l'utente ha i diritti per accedere al documento
            string errorMessage = string.Empty;
            string script = string.Empty;

            int retValue = DocumentManager.verificaACL("D", doc.idProfile, UserManager.getInfoUtente(), out errorMessage);
            if (retValue == 0 || retValue == 1)
            {
                script = "alert('" + errorMessage + "');";
            }
            else
            {
                DocumentManager.setRisultatoRicerca(page, doc);
                script = "top.principale.document.location = 'documento/gestionedoc.aspx?tab=protocollo';";
            }

            return script;
        }

        public static string refreshPage(DocsPaWR.InfoFascicolo infoFasc, Page page)
        {
            string script = string.Empty;
            DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloById(page, infoFasc.idFascicolo);

            if (fasc != null)
            {
                FascicoliManager.setFascicoloSelezionato(page, fasc);
                SiteNavigation.CallContextStack.CurrentContext.ContextState["systemId"] = fasc.systemID;
                if (fasc.idRegistroNodoTit != null && fasc.idRegistroNodoTit != string.Empty)
                {
                    DocsPaWR.Registro registroFascicolo = UserManager.getRegistroBySistemId(page, fasc.idRegistroNodoTit);
                    if (registroFascicolo != null)
                    {
                        UserManager.setRegistroSelezionato(page, registroFascicolo);
                    }
                    else
                    {
                        UserManager.removeRegistroSelezionato(page);
                    }
                }
            }
            script = "top.principale.document.location = 'fascicolo/gestioneFasc.aspx?tab=documenti';";
            return script;            
        }
    }    
}
