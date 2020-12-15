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
using SAAdminTool.DocsPaWR;
using SAAdminTool.SiteNavigation;
using SAAdminTool.utils;

namespace SAAdminTool.UserControls.ScrollElementsList
{
    public class ScrollManagerFasc : ScrollManager
    {
        public override string move(ObjScrollElementsList objScrollElementsList, Page page, ScrollManager.ScrollDirection scrollDirection)
        {
            //N.B. I casi di primo e ultimo elemento non vengono gestiti perchè
            //il pulsante next è disabilitato in caso ci si trova all'ultimo elemento ed
            //il pulsante prev è disabilitato in caso ci si trova al primo elemento
            ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.RemoveCurrentFilter();
            //Mi muovo in avanti
            if (scrollDirection == ScrollManager.ScrollDirection.NEXT)
            {

                
                //Non siamo a fine pagina, vanno aggiornati i dati
                if (objScrollElementsList.selectedElement + 1 != objScrollElementsList.pageSize)
                {
                    return this.moveFascInPage(objScrollElementsList, page, scrollDirection);
                }

                //Siamo a fine pagina, va rifatta la ricerca e aggiornati i dati
                if (objScrollElementsList.selectedElement + 1 == objScrollElementsList.pageSize)
                {
                    return this.moveFascNotInPage(objScrollElementsList, page, scrollDirection);
                }
            }

            //Mi muovo indietro
            if (scrollDirection == ScrollManager.ScrollDirection.PREV)
            {
                //Non siamo a inizio pagina, vanno aggiornati i dati
                if (objScrollElementsList.selectedElement != 0)
                {
                    return this.moveFascInPage(objScrollElementsList, page, scrollDirection);
                }

                //Siamo a inizio pagina, va rifatta la ricerca e aggiornati i dati
                if (objScrollElementsList.selectedElement == 0)
                {
                    return this.moveFascNotInPage(objScrollElementsList, page, scrollDirection);
                }
            }

            return string.Empty;
        }

        private string moveFascInPage(ObjScrollElementsList objScrollElementsList, Page page, ScrollManager.ScrollDirection scrollDirection)
        {
            string script = string.Empty;

            if (SiteNavigation.CallContextStack.CallerContext != null)
            {
                //Verifico in che direzione muovermi
                if (scrollDirection == ScrollManager.ScrollDirection.NEXT)
                {
                    objScrollElementsList.selectedElement++;
                    SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["fascIndex"] = objScrollElementsList.selectedElement.ToString();
                }
                if (scrollDirection == ScrollManager.ScrollDirection.PREV)
                {
                    objScrollElementsList.selectedElement--;
                    SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["fascIndex"] = objScrollElementsList.selectedElement.ToString();
                }
            }

            if (objScrollElementsList.objList != null)
            {
                DocsPaWR.SearchObject fasc = (DocsPaWR.SearchObject)objScrollElementsList.objList[objScrollElementsList.selectedElement];

                //DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)objScrollElementsList.objList[objScrollElementsList.selectedElement];

                DocsPaWR.Fascicolo newfasc = FascicoliManager.getFascicoloById(page, fasc.SearchObjectID);

                if (fasc != null)
                {
                    script = ScrollManager.refreshPage(newfasc, page);
                }
            }

            return script;
        }

        private string moveFascNotInPage(ObjScrollElementsList objScrollElementsList, Page page, ScrollManager.ScrollDirection scrollDirection)
        {
            string script = string.Empty;

            //Recupero le informazioni per effettuare una nuova ricerca
            SAAdminTool.DocsPaWR.SearchObject[] listaFasc = null;
            SAAdminTool.DocsPaWR.SearchObject fasc = null;

            //Recupero i filtri di ricerca ed effettuo la nuova ricerca
            if (SiteNavigation.CallContextStack.CallerContext != null)
            {
                //Verifico in che direzione muovermi
                if (scrollDirection == ScrollManager.ScrollDirection.NEXT)
                {
                    objScrollElementsList.selectedElement = 0;
                    objScrollElementsList.selectedPage++;
                    SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["fascIndex"] = "0";
                    SiteNavigation.CallContextStack.CallerContext.PageNumber = objScrollElementsList.selectedPage + 1;

                    //Effettuo la nuova ricerca
                    listaFasc = searchFasc(objScrollElementsList, page);
                    if (listaFasc != null)
                    {
                        objScrollElementsList.objList = new ArrayList(listaFasc);
                        fasc = (DocsPaWR.SearchObject)objScrollElementsList.objList[0];
                    }
                }

                if (scrollDirection == ScrollManager.ScrollDirection.PREV)
                {
                    objScrollElementsList.selectedElement = objScrollElementsList.pageSize - 1;
                    objScrollElementsList.selectedPage--;
                    SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["fascIndex"] = (objScrollElementsList.pageSize - 1).ToString();
                    SiteNavigation.CallContextStack.CallerContext.PageNumber = objScrollElementsList.selectedPage + 1;

                    //Effettuo la nuova ricerca
                    listaFasc = searchFasc(objScrollElementsList, page);
                    if (listaFasc != null)
                    {
                        objScrollElementsList.objList = new ArrayList(listaFasc);
                        fasc = (DocsPaWR.SearchObject)objScrollElementsList.objList[objScrollElementsList.pageSize - 1];
                    }
                }
            }

            if (fasc != null)
            {
                script = ScrollManager.refreshPage2(fasc, page);
            }
            
            return script;
        }

        private DocsPaWR.SearchObject[] searchFasc(ObjScrollElementsList objScrollElementsList, Page page)
        {
            SAAdminTool.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
            DocsPaWR.Registro userReg = UserManager.getRegistroSelezionato(page);
            DocsPaWR.FascicolazioneClassificazione classificazione; //= FascicoliManager.getClassificazioneSelezionata(page);
            
            // Caricamento della classifiazione dal livello di call context inferiore (se esiste)
            if (CallContextStack.CallerContext.ContextState.ContainsKey("Classification"))
                classificazione = CallContextStack.CallerContext.ContextState["Classification"] as DocsPaWR.FascicolazioneClassificazione;
            else
                classificazione = FascicoliManager.getClassificazioneSelezionata(page);

            bool allClass = FascicoliManager.getAllClassValue(page);
            SAAdminTool.DocsPaWR.FiltroRicerca[][] listaFiltri = null;
            SAAdminTool.DocsPaWR.SearchObject[] listaFasc = null;

            // Non utilizzato
            SearchResultInfo[] idProjects = null;

           switch (objScrollElementsList.searchContext)
            {
                case ObjScrollElementsList.EmunSearchContext.RICERCA_FASCICOLI:
                    listaFiltri = FascicoliManager.getMemoriaFiltriRicFasc(page);

                    // Se non ci sono filtri vengono caricati quelli per la nuova ricerca
                    if (listaFiltri == null)
                        listaFiltri = CallContextStack.CallerContext.ContextState["SearchFilters"] as FiltroRicerca[][];

                   // listaFasc = FascicoliManager.getListaFascicoliPaging(page, classificazione, userReg, listaFiltri[0], allClass, objScrollElementsList.selectedPage + 1, out objScrollElementsList.totalNumberOfPage, out objScrollElementsList.totalNumberOfElements, objScrollElementsList.pageSize, false, out idProjects, null);
                    listaFasc = FascicoliManager.getListaFascicoliPagingCustom(page, classificazione, userReg, listaFiltri[0], allClass, objScrollElementsList.selectedPage + 1, out objScrollElementsList.totalNumberOfPage, out objScrollElementsList.totalNumberOfElements, objScrollElementsList.pageSize, false, out idProjects, null, GridManager.IsRoleEnabledToUseGrids(), false, null,null,true);
                   break;
            }
            
            return listaFasc;
        }       
    }
}
