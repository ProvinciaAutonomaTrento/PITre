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
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using DocsPAWA.SiteNavigation;
using DocsPAWA.ricercaDoc;

namespace DocsPAWA.UserControls.ScrollElementsList
{
    public class ScrollManagerDoc : ScrollManager
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
                    return this.moveDocInPage(objScrollElementsList, page, scrollDirection);
                }

                //Siamo a fine pagina, va rifatta la ricerca e aggiornati i dati
                if (objScrollElementsList.selectedElement + 1 == objScrollElementsList.pageSize)
                {
                    return this.moveDocNotInPage(objScrollElementsList, page, scrollDirection);
                }
            }

            //Mi muovo indietro
            if (scrollDirection == ScrollManager.ScrollDirection.PREV)
            {
                //Non siamo a inizio pagina, vanno aggiornati i dati
                if (objScrollElementsList.selectedElement != 0)
                {
                    return this.moveDocInPage(objScrollElementsList, page, scrollDirection);
                }

                //Siamo a inizio pagina, va rifatta la ricerca e aggiornati i dati
                if (objScrollElementsList.selectedElement == 0)
                {
                    return this.moveDocNotInPage(objScrollElementsList, page, scrollDirection);
                }
            }           

            return string.Empty;
        }

        private string moveDocInPage(ObjScrollElementsList objScrollElementsList, Page page, ScrollManager.ScrollDirection scrollDirection)
        {
            string script = string.Empty;

            if (SiteNavigation.CallContextStack.CallerContext != null)
            {
                //Verifico in che direzione muovermi
                if (scrollDirection == ScrollManager.ScrollDirection.NEXT)
                {
                    objScrollElementsList.selectedElement++;
                    SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["docIndex"] = objScrollElementsList.selectedElement.ToString();
                }
                if (scrollDirection == ScrollManager.ScrollDirection.PREV)
                {
                    objScrollElementsList.selectedElement--;
                    SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["docIndex"] = objScrollElementsList.selectedElement.ToString();
                }
            }

            if (objScrollElementsList.objList != null)
            {
                InfoDocumento newDoc = null;
                string tipo = objScrollElementsList.objList[objScrollElementsList.selectedElement].GetType().Name;

                if (tipo.Equals("SearchObject"))
                {
                    DocsPaWR.SearchObject doc = (DocsPaWR.SearchObject)objScrollElementsList.objList[objScrollElementsList.selectedElement];
                    newDoc = DocumentManager.GetInfoDocumento(doc.SearchObjectID, doc.SearchObjectID, page);
                    if (doc != null)
                    {
                        script = ScrollManager.refreshPage(newDoc, page);
                    }
                }
                else
                {
                    DocsPaWR.InfoDocumento doc = (DocsPaWR.InfoDocumento)objScrollElementsList.objList[objScrollElementsList.selectedElement];
                    newDoc = DocumentManager.GetInfoDocumento(doc.idProfile, doc.idProfile, page);
                    if (doc != null)
                    {
                        script = ScrollManager.refreshPage(newDoc, page);
                    }
                }
                



            }

            return script;
        }

        private string moveDocNotInPage(ObjScrollElementsList objScrollElementsList, Page page, ScrollManager.ScrollDirection scrollDirection)
        {
            string script = string.Empty;

            //Recupero le informazioni per effettuare una nuova ricerca
            DocsPAWA.DocsPaWR.SearchObject[] listaDoc = null;
            DocsPaWR.InfoDocumento doc = null;

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

                    //Effettuo la nuova ricerca
                    listaDoc = searchDoc(objScrollElementsList, page);
                    if (listaDoc != null)
                    {
                        objScrollElementsList.objList = new ArrayList(listaDoc);
                        string tipo = objScrollElementsList.objList[0].GetType().Name;

                        if (tipo.Equals("SearchObject"))
                        {
                            DocsPaWR.SearchObject Newdoc = (DocsPaWR.SearchObject)objScrollElementsList.objList[0];
                            doc = DocumentManager.GetInfoDocumento(Newdoc.SearchObjectID, Newdoc.SearchObjectID, page);
                        }
                        else
                            doc = (DocsPaWR.InfoDocumento)objScrollElementsList.objList[0];
                    }
                }

                if (scrollDirection == ScrollManager.ScrollDirection.PREV)
                {
                    objScrollElementsList.selectedElement = objScrollElementsList.pageSize - 1;
                    objScrollElementsList.selectedPage--;
                    SiteNavigation.CallContextStack.CallerContext.QueryStringParameters["docIndex"] = (objScrollElementsList.pageSize - 1).ToString();
                    SiteNavigation.CallContextStack.CallerContext.PageNumber = objScrollElementsList.selectedPage + 1;

                    //Effettuo la nuova ricerca
                    listaDoc = searchDoc(objScrollElementsList, page);
                    if (listaDoc != null)
                    {
                        objScrollElementsList.objList = new ArrayList(listaDoc);
                        string tipo = objScrollElementsList.objList[0].GetType().Name;

                        if (tipo.Equals("SearchObject"))
                        {
                            DocsPaWR.SearchObject Newdoc = (DocsPaWR.SearchObject)objScrollElementsList.objList[objScrollElementsList.pageSize - 1];
                            doc = DocumentManager.GetInfoDocumento(Newdoc.SearchObjectID, Newdoc.SearchObjectID, page);
                        }
                        else
                            doc = (DocsPaWR.InfoDocumento)objScrollElementsList.objList[objScrollElementsList.pageSize - 1];
                    }
                }
            }

            if (doc != null)
            {
                script = ScrollManager.refreshPage(doc, page);                
            }
        
            return script;
        }

        private DocsPaWR.SearchObject[] searchDoc(ObjScrollElementsList objScrollElementsList, Page page)
        {
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
            DocsPAWA.DocsPaWR.FiltroRicerca[][] listaFiltri = null;
            DocsPAWA.DocsPaWR.SearchObject[] listaDoc = null;
            SearchResultInfo[] idProfilesList;

            switch (objScrollElementsList.searchContext)
            {
                case ObjScrollElementsList.EmunSearchContext.RICERCA_DOCUMENTI:
                    listaFiltri = (DocsPAWA.DocsPaWR.FiltroRicerca[][])SiteNavigation.CallContextStack.CallerContext.SessionState["ricDoc.listaFiltri"];

                    // Se non ci sono filtri vengono caricati quelli per la nuova ricerca
                    if (listaFiltri == null)
                        listaFiltri = CallContextStack.CallerContext.ContextState["SearchFilters"] as FiltroRicerca[][];

                    
                  // listaDoc = DocumentManager.getQueryInfoDocumentoPaging(infoUtente.idGruppo, infoUtente.idPeople, page, listaFiltri, objScrollElementsList.selectedPage + 1, out objScrollElementsList.totalNumberOfPage, out objScrollElementsList.totalNumberOfElements, false, false, true, false, out idProfilesList);
                    listaDoc = DocumentManager.getQueryInfoDocumentoPagingCustom(infoUtente, page, listaFiltri, objScrollElementsList.selectedPage + 1, out objScrollElementsList.totalNumberOfPage, out objScrollElementsList.totalNumberOfElements, true, false, GridManager.IsRoleEnabledToUseGrids(), objScrollElementsList.pageSize, false, null, null, out idProfilesList);
                   
                   break;

                case ObjScrollElementsList.EmunSearchContext.RICERCA_DOC_IN_FASC:
                    listaFiltri = ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.GetCurrentFilter();

                    // Se non ci sono filtri vengono caricati quelli per la nuova ricerca
                    if(listaFiltri == null)
                        listaFiltri = CallContextStack.CallerContext.ContextState["SearchFilters"] as FiltroRicerca[][];

                    DocsPaWR.Folder folder;

                    // Se nel call context è salvato un fascicolo selezionato viene caricato il folder relativo altrimenti viene prelevato
                    // dal call context il folder salvato dalla pagina di ricerca documenti in fascicolo
                    if (SiteNavigation.CallContextStack.CallerContext.SessionState.ContainsKey("FascicoloSelezionato"))
                        folder = FascicoliManager.getFolder(
                            page,
                            (DocsPaWR.Fascicolo)SiteNavigation.CallContextStack.CallerContext.SessionState["FascicoloSelezionato"]);
                    else
                        folder = CallContextStack.CallerContext.ContextState["Folder"] as Folder;

                    FascicoliManager.SetProtoDocsGridPaging(page, objScrollElementsList.selectedPage);

                    FascicolazioneClassificazione a = new FascicolazioneClassificazione();
                    SearchResultInfo[] idProfiles;
                    if (folder != null)
                    {
                        if (listaFiltri == null)
                        {
                           // listaDoc = FascicoliManager.getListaDocumentiPaging(page, folder, objScrollElementsList.selectedPage + 1, out objScrollElementsList.totalNumberOfPage, out objScrollElementsList.totalNumberOfElements, false, out idProfiles);
                            //listaDoc = FascicoliManager.getListaFascicoliPagingCustom(page, a, UserManager.getRegistroSelezionato(page), listaFiltri, false, objScrollElementsList.selectedPage + 1, out objScrollElementsList.totalNumberOfPage, out objScrollElementsList.totalNumberOfElements, objScrollElementsList.pageSize, false, out idProfilesList, null, GridManager.IsRoleEnabledToUseGrids(), false, null, null);
                            listaDoc = FascicoliManager.getListaDocumentiPagingCustom(page, folder, listaFiltri, objScrollElementsList.selectedPage + 1, out objScrollElementsList.totalNumberOfPage, out objScrollElementsList.totalNumberOfElements, false, GridManager.IsRoleEnabledToUseGrids(), false, null, null, objScrollElementsList.pageSize, FiltriRicercaDocumenti.CurrentFilterSessionOrderFilter.GetCurrentFilter(), out idProfiles);
                        }
                        else
                        {
                           // listaDoc = FascicoliManager.getListaDocumentiPaging(page, folder, listaFiltri, objScrollElementsList.selectedPage + 1, out objScrollElementsList.totalNumberOfPage, out objScrollElementsList.totalNumberOfElements, false, out idProfiles);
                            listaDoc = FascicoliManager.getListaDocumentiPagingCustom(page, folder, listaFiltri, objScrollElementsList.selectedPage + 1, out objScrollElementsList.totalNumberOfPage, out objScrollElementsList.totalNumberOfElements, false, GridManager.IsRoleEnabledToUseGrids(), false, null, null, objScrollElementsList.pageSize, FiltriRicercaDocumenti.CurrentFilterSessionOrderFilter.GetCurrentFilter(), out idProfiles);
                        }
                    }
                    break;
            }

            return listaDoc;
        }
    }
}
