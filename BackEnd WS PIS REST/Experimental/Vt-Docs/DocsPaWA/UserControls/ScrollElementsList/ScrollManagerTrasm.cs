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

namespace DocsPAWA.UserControls.ScrollElementsList
{
    public class ScrollManagerTrasm : ScrollManager
    {
        public override string move(ObjScrollElementsList objScrollElementsList, Page page, ScrollManager.ScrollDirection scrollDirection)
        {
            //N.B. I casi di primo e ultimo elemento non vengono gestiti perchè
            //il pulsante next è disabilitato in caso ci si trova all'ultimo elemento ed
            //il pulsante prev è disabilitato in caso ci si trova al primo elemento

            //Mi muovo in avanti
            if (scrollDirection == ScrollManager.ScrollDirection.NEXT)
            {
                //Non siamo a fine pagina, vanno aggiornati i dati
                if (objScrollElementsList.selectedElement + 1 != objScrollElementsList.pageSize)
                {
                    switch(objScrollElementsList.searchContext)
                    {
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST:
                            return this.moveToDoListInPage(objScrollElementsList, page, scrollDirection);
                            break;
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST:
                            return this.moveToDoListInPage(objScrollElementsList, page, scrollDirection);
                            break;
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC:
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC:
                            return this.moveTrasmInPage(objScrollElementsList, page, scrollDirection);
                            break;
                    }
                }

                //Siamo a fine pagina, va rifatta la ricerca e aggiornati i dati
                if (objScrollElementsList.selectedElement + 1 == objScrollElementsList.pageSize)
                {
                    switch (objScrollElementsList.searchContext)
                    {
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST:
                            return this.moveToDoListNotInPage(objScrollElementsList, page, scrollDirection);
                            break;
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST:
                            return this.moveToDoListNotInPage(objScrollElementsList, page, scrollDirection);
                            break;
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC:
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC:
                            return this.moveTrasmNotInPage(objScrollElementsList, page, scrollDirection);
                            break;
                    }
                }
            }

            //Mi muovo indietro
            if (scrollDirection == ScrollManager.ScrollDirection.PREV)
            {
                //Non siamo a inizio pagina, vanno aggiornati i dati
                if (objScrollElementsList.selectedElement != 0)
                {
                    switch (objScrollElementsList.searchContext)
                    {
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST:
                            return this.moveToDoListInPage(objScrollElementsList, page, scrollDirection);
                            break;
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST:
                            return this.moveToDoListInPage(objScrollElementsList, page, scrollDirection);
                            break;
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC:
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC:
                            return this.moveTrasmInPage(objScrollElementsList, page, scrollDirection);
                            break;
                    }
                }

                //Siamo a inizio pagina, va rifatta la ricerca e aggiornati i dati
                if (objScrollElementsList.selectedElement == 0)
                {
                    switch (objScrollElementsList.searchContext)
                    {
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST:
                             return this.moveToDoListNotInPage(objScrollElementsList, page, scrollDirection);
                            break;
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST:
                            return this.moveToDoListNotInPage(objScrollElementsList, page, scrollDirection);
                            break;
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC:
                        case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC:
                            return this.moveTrasmNotInPage(objScrollElementsList, page, scrollDirection);
                            break;
                    }
                }
            }

            return string.Empty;
        }

        private string moveToDoListInPage(ObjScrollElementsList objScrollElementsList, Page page, ScrollManager.ScrollDirection scrollDirection)
        {
            string script = string.Empty;
            
            DocsPAWA.DocsPaWR.Trasmissione[] dettTrasm = null;
            
            if (SiteNavigation.CallContextStack.CallerContext != null)
            {
                //Verifico in che direzione muovermi
                if (scrollDirection == ScrollManager.ScrollDirection.NEXT)
                {
                    objScrollElementsList.selectedElement++;
                    
                    //Recupero il dettaglio della trasissione
                    dettTrasm = TrasmManager.trasmGetDettaglioTrasmissione(UserManager.getUtente(page), UserManager.getRuolo(page), (DocsPAWA.DocsPaWR.infoToDoList)objScrollElementsList.objList[objScrollElementsList.selectedElement]);
                    if (dettTrasm != null)
                        SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["idTrasm"] = dettTrasm[0].systemId; 
                }
                if (scrollDirection == ScrollManager.ScrollDirection.PREV)
                {
                    objScrollElementsList.selectedElement--;
                    
                    //Recupero il dettaglio della trasissione
                    dettTrasm = TrasmManager.trasmGetDettaglioTrasmissione(UserManager.getUtente(page), UserManager.getRuolo(page), (DocsPAWA.DocsPaWR.infoToDoList)objScrollElementsList.objList[objScrollElementsList.selectedElement]);
                    if (dettTrasm != null)
                        SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["idTrasm"] = dettTrasm[0].systemId; 
                }
            }

            if (objScrollElementsList.objList != null)
            {
                //Recupero l'infoDocumento o l'infoFascicolo dal dettaglio della trasmissione
                if (dettTrasm != null && dettTrasm[0].infoDocumento != null && !string.IsNullOrEmpty(dettTrasm[0].infoDocumento.idProfile))
                {
                    DocsPaWR.InfoDocumento doc = dettTrasm[0].infoDocumento;

                    if (doc != null)
                    {
                        script = ScrollManager.refreshPage(doc, page);
                    }
                }

                if (dettTrasm != null && dettTrasm[0].infoFascicolo != null && !string.IsNullOrEmpty(dettTrasm[0].infoFascicolo.idFascicolo))
                {
                    DocsPaWR.InfoFascicolo fasc = dettTrasm[0].infoFascicolo;

                    if (fasc != null)
                    {
                        script = ScrollManager.refreshPage(fasc, page);
                    }
                }                
            }

            return script;
        }

        private string moveToDoListNotInPage(ObjScrollElementsList objScrollElementsList, Page page, ScrollManager.ScrollDirection scrollDirection)
        {
            string script = string.Empty;

            DocsPAWA.DocsPaWR.Trasmissione[] dettTrasm = null;
                
            //Recupero i filtri di ricerca ed effettuo la nuova ricerca
            if (SiteNavigation.CallContextStack.CallerContext != null)
            {
                //Verifico in che direzione muovermi
                if (scrollDirection == ScrollManager.ScrollDirection.NEXT)
                {
                    objScrollElementsList.selectedElement = 0;
                    objScrollElementsList.selectedPage++;

                    //Effettuo una nuova ricerca
                    DocsPAWA.DocsPaWR.infoToDoList[] listInfoToDoList = null;
                    int totalTrasmNonViste = 0;
                    if (objScrollElementsList.searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST)
                        listInfoToDoList =
                            TrasmManager.getMyNewTodolist(getListaRegistri(UserManager.getRuolo().registri),
                                                          DocumentManager.getFiltroRicTrasm(page),
                                                          objScrollElementsList.selectedPage + 1,
                                                          objScrollElementsList.pageSize,
                                                          out objScrollElementsList.totalNumberOfElements,
                                                          out totalTrasmNonViste);
                        //listInfoToDoList = TrasmManager.getMyTodolist("D", getListaRegistri(UserManager.getRuolo().registri), DocumentManager.getFiltroRicTrasm(page), objScrollElementsList.selectedPage + 1, objScrollElementsList.pageSize, out objScrollElementsList.totalNumberOfElements);))
                    if(objScrollElementsList.searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST)
                        TrasmManager.getMyNewTodolist(getListaRegistri(UserManager.getRuolo().registri),
                                                          DocumentManager.getFiltroRicTrasm(page),
                                                          objScrollElementsList.selectedPage + 1,
                                                          objScrollElementsList.pageSize,
                                                          out objScrollElementsList.totalNumberOfElements,
                                                          out totalTrasmNonViste);
                        //listInfoToDoList = TrasmManager.getMyTodolist("F", getListaRegistri(UserManager.getRuolo().registri), DocumentManager.getFiltroRicTrasm(page), objScrollElementsList.selectedPage + 1, objScrollElementsList.pageSize, out objScrollElementsList.totalNumberOfElements);

                    //Il risultato della ricerca potrebbe essere vuoto in quanto le trasmissioni che si stanno scorrendo vengono contestualmente rimosse dalla toDoList
                    if (listInfoToDoList != null && listInfoToDoList.Length != 0)
                    {
                        objScrollElementsList.objList = new ArrayList(listInfoToDoList);

                        //Recupero il dettaglio della trasmissione
                        dettTrasm = TrasmManager.trasmGetDettaglioTrasmissione(UserManager.getUtente(page), UserManager.getRuolo(page), (DocsPAWA.DocsPaWR.infoToDoList)objScrollElementsList.objList[objScrollElementsList.selectedElement]);
                        if (dettTrasm != null)
                        {
                            SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["idTrasm"] = dettTrasm[0].systemId;
                            SiteNavigation.CallContextStack.CallerContext.PageNumber = objScrollElementsList.selectedPage + 1;
                        }
                    }
                    else
                    {
                        if (SiteNavigation.CallContextStack.CallerContext != null)
                            SiteNavigation.CallContextStack.CallerContext.objScrollElementsList = null;
                        objScrollElementsList.objList = null;
                    }
                }

                if (scrollDirection == ScrollManager.ScrollDirection.PREV)
                {
                    objScrollElementsList.selectedElement = objScrollElementsList.pageSize - 1;
                    objScrollElementsList.selectedPage--;
                    int totalTrasmNonViste = 0;
                    //Effettuo una nuova ricerca
                    DocsPAWA.DocsPaWR.infoToDoList[] listInfoToDoList = null;
                    if (objScrollElementsList.searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC_TO_DO_LIST)
                        listInfoToDoList =
                            TrasmManager.getMyNewTodolist(getListaRegistri(UserManager.getRuolo().registri),
                                                          DocumentManager.getFiltroRicTrasm(page),
                                                          objScrollElementsList.selectedPage + 1,
                                                          objScrollElementsList.pageSize,
                                                          out objScrollElementsList.totalNumberOfElements,
                                                          out totalTrasmNonViste);
                        //listInfoToDoList = TrasmManager.getMyTodolist("D", getListaRegistri(UserManager.getRuolo().registri), DocumentManager.getFiltroRicTrasm(page), objScrollElementsList.selectedPage + 1, objScrollElementsList.pageSize, out objScrollElementsList.totalNumberOfElements);
                    if (objScrollElementsList.searchContext == ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC_TO_DO_LIST)
                        listInfoToDoList =
                            TrasmManager.getMyNewTodolist(getListaRegistri(UserManager.getRuolo().registri),
                                                          DocumentManager.getFiltroRicTrasm(page),
                                                          objScrollElementsList.selectedPage + 1,
                                                          objScrollElementsList.pageSize,
                                                          out objScrollElementsList.totalNumberOfElements,
                                                          out totalTrasmNonViste);
                        //listInfoToDoList = TrasmManager.getMyTodolist("F", getListaRegistri(UserManager.getRuolo().registri), DocumentManager.getFiltroRicTrasm(page), objScrollElementsList.selectedPage + 1, objScrollElementsList.pageSize, out objScrollElementsList.totalNumberOfElements);

                    //Il risultato della ricerca potrebbe essere vuoto in quanto le trasmissioni che si stanno scorrendo vengono contestualmente rimosse dalla toDoList
                    if (listInfoToDoList != null && listInfoToDoList.Length != 0)
                    {
                        objScrollElementsList.objList = new ArrayList(listInfoToDoList);

                        //Recupero il dettaglio della trasmissione
                        dettTrasm = TrasmManager.trasmGetDettaglioTrasmissione(UserManager.getUtente(page), UserManager.getRuolo(page), (DocsPAWA.DocsPaWR.infoToDoList)objScrollElementsList.objList[objScrollElementsList.selectedElement]);
                        if (dettTrasm != null)
                        {
                            SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["idTrasm"] = dettTrasm[0].systemId;
                            SiteNavigation.CallContextStack.CallerContext.PageNumber = objScrollElementsList.selectedPage + 1;
                        }
                    }
                    else
                    {
                        if (SiteNavigation.CallContextStack.CallerContext != null)
                            SiteNavigation.CallContextStack.CallerContext.objScrollElementsList = null;
                        objScrollElementsList.objList = null;
                    }
                }
            }

            if (objScrollElementsList.objList != null)
            {
                //Recupero l'infoDocumento o l'infoFascicolo dal dettaglio della trasmissione
                if (dettTrasm != null && dettTrasm[0].infoDocumento != null && !string.IsNullOrEmpty(dettTrasm[0].infoDocumento.idProfile))
                {
                    DocsPaWR.InfoDocumento doc = dettTrasm[0].infoDocumento;

                    if (doc != null)
                    {
                        script = ScrollManager.refreshPage(doc, page);
                    }
                }

                if (dettTrasm != null && dettTrasm[0].infoFascicolo != null && !string.IsNullOrEmpty(dettTrasm[0].infoFascicolo.idFascicolo))
                {
                    DocsPaWR.InfoFascicolo fasc = dettTrasm[0].infoFascicolo;

                    if (fasc != null)
                    {
                        script = ScrollManager.refreshPage(fasc, page);
                    }
                }          
            }

            return script;
        }

        private string moveTrasmInPage(ObjScrollElementsList objScrollElementsList, Page page, ScrollManager.ScrollDirection scrollDirection)
        {
            string script = string.Empty;

            DocsPAWA.DocsPaWR.Trasmissione trasm = null;

            if (SiteNavigation.CallContextStack.CallerContext != null)
            {
                //Verifico in che direzione muovermi
                if (scrollDirection == ScrollManager.ScrollDirection.NEXT)
                {
                    objScrollElementsList.selectedElement++;

                    //Recupero la trasmissione
                    trasm = (DocsPAWA.DocsPaWR.Trasmissione)objScrollElementsList.objList[objScrollElementsList.selectedElement];
                    if (trasm != null)
                        SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["docIndex"] = objScrollElementsList.selectedElement.ToString();
                }
                if (scrollDirection == ScrollManager.ScrollDirection.PREV)
                {
                    objScrollElementsList.selectedElement--;

                    //Recupero la trasmissione
                    trasm = (DocsPAWA.DocsPaWR.Trasmissione)objScrollElementsList.objList[objScrollElementsList.selectedElement];
                    if (trasm != null)
                        SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["docIndex"] = objScrollElementsList.selectedElement.ToString();
                }
            }

            if (objScrollElementsList.objList != null)
            {
                //Recupero l'infoDocumento o l'infoFascicolo dal dettaglio della trasmissione
                if (trasm != null && trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile))
                {
                    DocsPaWR.InfoDocumento doc = trasm.infoDocumento;

                    if (doc != null)
                    {
                        script = ScrollManager.refreshPage(doc, page);
                    }
                }

                if (trasm != null && trasm.infoFascicolo != null && !string.IsNullOrEmpty(trasm.infoFascicolo.idFascicolo))
                {
                    DocsPaWR.InfoFascicolo fasc = trasm.infoFascicolo;

                    if (fasc != null)
                    {
                        script = ScrollManager.refreshPage(fasc, page);
                    }
                }
            }

            return script;
        }

        private string moveTrasmNotInPage(ObjScrollElementsList objScrollElementsList, Page page, ScrollManager.ScrollDirection scrollDirection)
        {
            string script = string.Empty;

            DocsPAWA.DocsPaWR.Trasmissione[] listaTram = null;
            DocsPAWA.DocsPaWR.Trasmissione trasm = null;
            
            //Recupero i filtri di ricerca ed effettuo la nuova ricerca
            if (SiteNavigation.CallContextStack.CallerContext != null)
            {
                //Verifico in che direzione muovermi
                if (scrollDirection == ScrollManager.ScrollDirection.NEXT)
                {
                    objScrollElementsList.selectedElement = 0;
                    objScrollElementsList.selectedPage++;
                    SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["docIndex"] = "0";
                    SiteNavigation.CallContextStack.CallerContext.PageNumber = objScrollElementsList.selectedPage + 1;

                    //Effettuo una nuova ricerca
                    listaTram = searchTrasm(objScrollElementsList, page);
                    if (listaTram != null)
                    {
                        objScrollElementsList.objList = new ArrayList(listaTram);
                        trasm = (DocsPaWR.Trasmissione)objScrollElementsList.objList[0];
                    }                    
                }

                if (scrollDirection == ScrollManager.ScrollDirection.PREV)
                {
                    objScrollElementsList.selectedElement = objScrollElementsList.pageSize - 1;
                    objScrollElementsList.selectedPage--;
                    SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["docIndex"] = (objScrollElementsList.pageSize - 1).ToString();
                    SiteNavigation.CallContextStack.CallerContext.PageNumber = objScrollElementsList.selectedPage + 1;

                    //Effettuo una nuova ricerca
                    listaTram = searchTrasm(objScrollElementsList, page);
                    if (listaTram != null)
                    {
                        objScrollElementsList.objList = new ArrayList(listaTram);
                        trasm = (DocsPaWR.Trasmissione)objScrollElementsList.objList[objScrollElementsList.pageSize - 1];
                    }
                }
            }

            if (objScrollElementsList.objList != null)
            {
                //Recupero l'infoDocumento o l'infoFascicolo dal dettaglio della trasmissione
                if (trasm != null && trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile))
                {
                    DocsPaWR.InfoDocumento doc = trasm.infoDocumento;

                    if (doc != null)
                    {
                        script = ScrollManager.refreshPage(doc, page);
                    }
                }

                if (trasm != null && trasm.infoFascicolo != null && !string.IsNullOrEmpty(trasm.infoFascicolo.idFascicolo))
                {
                    DocsPaWR.InfoFascicolo fasc = trasm.infoFascicolo;

                    if (fasc != null)
                    {
                        script = ScrollManager.refreshPage(fasc, page);
                    }
                }
            }

            return script;
        }

        private DocsPaWR.Trasmissione[] searchTrasm(ObjScrollElementsList objScrollElementsList, Page page)
        {
            DocsPAWA.DocsPaWR.FiltroRicerca[] filtri = null;
            DocsPAWA.DocsPaWR.Trasmissione[] listaTram = null;
            DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();

            switch (objScrollElementsList.searchContext)
            {
                case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC:
                case ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC:
                    filtri = DocsPAWA.DocumentManager.getFiltroRicTrasm(page);
                    if (SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["verso"].ToString().ToUpper() == "R".ToUpper())
                    {
                        listaTram = DocsPAWA.TrasmManager.getQueryRicevutePaging(page, oggettoTrasm, UserManager.getUtente(page), UserManager.getRuolo(page), filtri, objScrollElementsList.selectedPage + 1, out objScrollElementsList.totalNumberOfPage, out objScrollElementsList.totalNumberOfElements);
                    }
                    if (SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["verso"].ToString().ToUpper() == "E".ToUpper())
                    {
                        listaTram = DocsPAWA.TrasmManager.getQueryEffettuatePaging(page, oggettoTrasm, UserManager.getUtente(page), UserManager.getRuolo(page), filtri, objScrollElementsList.selectedPage + 1, out objScrollElementsList.totalNumberOfPage, out objScrollElementsList.totalNumberOfElements);
                    }
                    break;
            }

            return listaTram;
        }

        private string getListaRegistri(DocsPAWA.DocsPaWR.Registro[] reg)
        {
            string registri = "0";

            if (reg.Length > 0)
            {
                foreach (DocsPAWA.DocsPaWR.Registro registro in reg)
                {
                    registri = registri + "," + registro.systemId;
                }
                return registri;
            }
            else return registri;
        }
    }
}                   
