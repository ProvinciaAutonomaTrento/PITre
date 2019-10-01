using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDatalLibrary;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDataWA.UserControls;


namespace NttDataWA.Project
{
    public partial class Structure_getFolders : System.Web.UI.Page
    {
        
        #region Properties

        private bool RapidClassificationRequired
        {
            get
            {
                return (bool)HttpContext.Current.Session["RapidClassificationRequired"];
            }
            set
            {
                HttpContext.Current.Session["RapidClassificationRequired"] = value;
            }
        }

        private FiltroRicerca[][] FiltroRicerca
        {
            get
            {
                return (FiltroRicerca[][])HttpContext.Current.Session["FiltroRicerca"];
            }
            set
            {
                HttpContext.Current.Session["FiltroRicerca"] = value;
            }
        }

        private SearchObject[] Result
        {
            get
            {
                return HttpContext.Current.Session["Result"] as SearchObject[];
            }
            set
            {
                HttpContext.Current.Session["Result"] = value;
            }
        }

        private int PageSizeStructure
        {
            get
            {
                if (HttpContext.Current.Session["PageSizeStructure"] != null)
                    return (int)HttpContext.Current.Session["PageSizeStructure"];
                else
                    return 0;
            }
            set
            {
                HttpContext.Current.Session["PageSizeStructure"] = value;
            }
        }

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                string qs = Request.QueryString.ToString();
                string f = Request.Form.ToString();

                if (Request.Form != null && Request.Form.Count > 0)
                {
                    string folderId = Request.Form["id"].Replace("node_", "").Replace("root_", "");
                    DocsPaWR.Folder folder = new DocsPaWR.Folder();
                    if (!string.IsNullOrEmpty(folderId) && folderId.IndexOf("doc") < 0)
                    {
                        folder = ProjectManager.getFolder(this, folderId);
                        folder = ProjectManager.getFolder(this, folder);
                    }

                    switch (Request.Form["operation"])
                    {
                        case "move_node":
                            // id=node_34342324
                            // parent=node_35366466
                            if (folderId.IndexOf("doc") >= 0)
                            {
                                string docId = folderId.Replace("doc_", "");
                                string parentId = Request.Form["parent"].Replace("node_", "").Replace("root_", "");
                                string oldParentId = Request.Form["oparent"].Replace("node_", "").Replace("root_", "");
                                this.MoveDocument(docId, oldParentId, parentId);
                            }
                            else
                            {
                                string parentId = Request.Form["parent"].Replace("node_", "").Replace("root_", "");
                                this.MoveFolder(folder, parentId);
                            }
                            break;
                    }
                }
                else
                {
                    //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
                    Fascicolo Prj = UIManager.ProjectManager.getProjectInSession();
                    if ((Prj.systemID != null && !string.IsNullOrEmpty(Prj.systemID)) && ProjectManager.CheckRevocationAcl())
                    {
                        this.html_data.Text = "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}";
                        return;
                    }

                    if (Request.QueryString["id"] == "0")
                    {
                        Fascicolo Fasc = ProjectManager.getProjectInSession();
                        DocsPaWR.Folder folder = ProjectManager.getFolder(this, Fasc);
                        this.caricaFoldersFascicolo(folder);
                    }
                    else if (Request.QueryString["id"] != null && Request.QueryString["id"] != "0")
                    {
                        string folderId = Request.QueryString["id"].Replace("node_", "").Replace("root_", "");
                        DocsPaWR.Folder folder = new DocsPaWR.Folder();
                        if (!string.IsNullOrEmpty(folderId) && folderId.IndexOf("doc") < 0)
                        {
                            folder = ProjectManager.getFolder(this, folderId);
                            folder = ProjectManager.getFolder(this, folder);
                            //this.html_data.Text = "<ul>\n";
                            this.html_data.Text += this.GetSubFolders(folder, false, true, false);
                            //this.html_data.Text = "</ul>\n";
                        }
                    }
                    else if (Request.QueryString["q"] != null && Request.QueryString["q"].Length > 0)
                    {
                        string descrizione = Request.QueryString["q"].Trim();
                        Fascicolo fascSelezionato = ProjectManager.getProjectInSession();
                        string idFascicolo = fascSelezionato.systemID;
                        if (descrizione != string.Empty)
                        {
                            string idParent = fascSelezionato.folderSelezionato.idParent;

                            Folder[] risultatiFolder = ProjectManager.getFolderByDescrizione(this, idFascicolo, descrizione);

                            if (risultatiFolder != null)
                            {
                                string html = string.Empty;

                                for (int i = 0; i < risultatiFolder.Length; i++)
                                {
                                    if (fascSelezionato.folderSelezionato.idParent != risultatiFolder[i].systemID)
                                    {
                                        Folder tempFolder = risultatiFolder[i];
                                        string html2 = string.Empty;
                                        while (fascSelezionato.folderSelezionato.idParent != tempFolder.idParent)
                                        {
                                            if (html.IndexOf("{ \"id\": \"node_" + tempFolder.systemID + "\" , \"isResult\": \"false\" }") < 0)
                                            {
                                                string isResult = "false";
                                                if (tempFolder.systemID == risultatiFolder[i].systemID) isResult = "true";

                                                if (html2.Length > 0)
                                                {
                                                    html2 = "	{ \"id\": \"node_" + tempFolder.systemID + "\" , \"isResult\": \"" + isResult + "\" }, " + html2;
                                                }
                                                else
                                                {
                                                    html2 = "	{ \"id\": \"node_" + tempFolder.systemID + "\" , \"isResult\": \"" + isResult + "\" } " + html2;
                                                }
                                            }
                                            tempFolder = ProjectManager.getFolder(this, tempFolder.idParent);
                                        }
                                        if (!string.IsNullOrEmpty(html))
                                            html += ", ";
                                        html += html2;
                                    }
                                }

                                this.html_data.Text = "[\n"
                                                    + html
                                                    + "]\n";
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void caricaFoldersFascicolo(DocsPaWR.Folder folder)
        {
            try {
                if (folder != null)
                {
                    string html = this.GetSubFolders(folder, true, true, true);
                    this.html_data.Text = html;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private string GetSubFolders(DocsPaWR.Folder folder)
        {
            return this.GetSubFolders(folder, false, false, true);
        }

        private string GetSubFolders(DocsPaWR.Folder folder, bool isRoot, bool showDocs, bool showItself)
        {
            try {
                string type = isRoot ? "root" : "node";
                bool isUnamovable = !(
                    UserManager.IsAuthorizedFunctions("FASC_INS_DOC")
                    && UserManager.IsAuthorizedFunctions("DO_DEL_DOC_FASC")
                    && UserManager.IsAuthorizedFunctions("FASC_NEW_FOLDER")
                    && UserManager.IsAuthorizedFunctions("FASC_MOD_FOLDER")
                    && UserManager.IsAuthorizedFunctions("FASC_DEL_FOLDER")
                    && Convert.ToInt32(ProjectManager.getProjectInSession().accessRights) >= Convert.ToInt32(HMdiritti.HMdiritti_Write)
                    ) ? true : false;

                int numDocs = this.GetDocuments(FiltroRicerca, folder);
                this.PageSizeStructure += numDocs;
                SearchObject[] result = this.Result;

                string cssClass = "";
                if (isRoot)
                    cssClass = " class=\"jstree-unamovable\"";
                else if (isUnamovable)
                    cssClass = " class=\"jstree-unamovable jstree-closed\"";
                else if (folder.childs.Length > 0 || numDocs>0)
                    cssClass = " class=\"jstree-closed\"";
            
                string html = string.Empty;

                if (showItself)
                {
                    html = "    <li id=\"" + type + "_" + folder.systemID + "\"" + cssClass + " data-title=\"" + utils.FormatJs(utils.FormatHtml(folder.descrizione)) + "\">\n"
                        + "        <a href=\"#\" class=\"document_ellipsis clickableRight-no-ie\" title=\"" + utils.FormatJs(utils.FormatHtml(folder.descrizione)) + "\">" + utils.FormatHtml(folder.descrizione) + "</a>\n";
                }

                if (showDocs)
                {
                    cssClass = "nosons";
                    if (!(
                        UserManager.IsAuthorizedFunctions("FASC_INS_DOC") 
                        && UserManager.IsAuthorizedFunctions("DO_DEL_DOC_FASC")
                        && Convert.ToInt32(ProjectManager.getProjectInSession().accessRights) >= Convert.ToInt32(HMdiritti.HMdiritti_Write)
                        )) cssClass += " jstree-unamovable";

                    if (folder.childs.Length > 0)
                    {
                        html += "<ul>\n";
                        for (int k = 0; k < folder.childs.Length; k++)
                            html += this.GetSubFolders(folder.childs[k]);
                        if (numDocs==0) html += "</ul>\n";
                    }

                    if (numDocs > 0)
                    {
                        if (folder.childs.Length==0) html += "<ul>\n";
                        foreach (SearchObject doc in result)
                        {
                            string numeroDocumento = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                            string numeroProtocollo = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D12")).FirstOrDefault().SearchObjectFieldValue;
                            string data = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
                            string oggetto = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D4")).FirstOrDefault().SearchObjectFieldValue;
                            string immagineAcquisita = doc.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D23")).FirstOrDefault().SearchObjectFieldValue;
                            string extension = !String.IsNullOrEmpty(immagineAcquisita) && immagineAcquisita != "0" ? immagineAcquisita : String.Empty;
                            if (string.IsNullOrEmpty(extension))
                            {
                                extension = "no_file";
                            }
                            else
                            {
                                extension = FileManager.getEstensioneIntoSignedFile(extension.ToLower());
                            }

                            if (!String.IsNullOrEmpty(numeroProtocollo))
                                oggetto = numeroProtocollo + " - " + data + " - " + oggetto;
                            else
                                oggetto = numeroDocumento + " - " + data + " - " + oggetto;

                            html += "   <li id=\"doc_" + numeroDocumento + "\" rel=\"" + extension + "\" class=\"" + cssClass + " doc_" + numeroDocumento + "\" data-title=\"" + utils.FormatJs(utils.FormatHtml(oggetto)) + "\">\n"
                                  + "       <a href=\"#\" class=\"document_ellipsis clickableRight-no-ie\" title=\"" + utils.FormatJs(utils.FormatHtml(oggetto)) + "\">" + utils.FormatHtml(oggetto) + "</a>\n"
                                  + "   </li>\n";
                        }
                        html += "</ul>\n";
                    }
                }

                if (showItself)
                {
                    html += "   </li>\n";
                }

                return html;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private int GetDocuments(FiltroRicerca[][] searchFilters, Folder folder)
        {
            try {
                bool showGridPersonalization = UIManager.GridManager.EnableCustomGrid(UIManager.RoleManager.GetRoleInSession());

                int recordNumber = 0;
                int selectedPage = 1;
                int pageNumbers = 0;

                Grid selectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Document, UserManager.GetInfoUser());
                FiltroRicerca[][] orderFilters = UIManager.GridManager.GetFiltriOrderRicerca(selectedGrid);
                this.Result = this.SearchDocument(searchFilters, selectedPage, out pageNumbers, out recordNumber, folder, orderFilters, showGridPersonalization, selectedGrid);
                return recordNumber;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return 0;
            }
        }

        private SearchObject[] SearchDocument(FiltroRicerca[][] searchFilters,
            int selectedPage, out int pageNumbers, out  int recordNumber, Folder folder,
            FiltroRicerca[][] orderFilters, bool showGridPersonalization, Grid SelectedGrid)
        {
            try {
                // Documenti individuati dalla ricerca
                SearchObject[] toReturn;
                // Lista dei system id dei documenti restituiti dalla ricerca
                SearchResultInfo[] idProfiles = null;

                // Recupero dei campi della griglia impostati come visibili
                Field[] visibleArray = null;
                List<Field> visibleFieldsTemplate;

                visibleFieldsTemplate = SelectedGrid.Fields.Where(e => e.Visible && e.GetType().Equals(typeof(Field)) && e.CustomObjectId != 0).ToList();
                if (visibleFieldsTemplate != null && visibleFieldsTemplate.Count > 0)
                    visibleArray = visibleFieldsTemplate.ToArray();

                toReturn = UIManager.ProjectManager.getListaDocumentiPagingCustom(
                     folder,
                     searchFilters,
                     selectedPage,
                     out pageNumbers,
                     out recordNumber,
                     !IsPostBack,
                     showGridPersonalization,
                     true,
                     visibleArray,
                     null,
                     10,
                     orderFilters,
                     out idProfiles);

                // Restituzione della lista di documenti da visualizzare
                return toReturn;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                pageNumbers = 0;
                recordNumber = 0;
                return new SearchObject[1];
            }
        }

        private void MoveFolder(Folder folder, string parentId)
        {
            try {
                // verifica se num doc visibili = tot doc in folder
                bool docsCountVisibleIsEgualNotVisible = ProjectManager.GetIfDocumentiCountVisibleIsEgualNotVisible(this, folder);

                if (docsCountVisibleIsEgualNotVisible)
                {
                    ProjectManager.MoveFolder(this, folder.systemID, parentId);
                }
                else
                {
                    string msg = "WarningProjectImpossibleMoveHasChildren";
                    this.html_data.Text = "<"+"script type=\"text/javascript\">ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning');<"+"/script>";
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void MoveDocument(string docsId, string oldParentId, string parentId)
        {
            try {
                string messaggio = string.Empty;
                string[] docId = new string[1] { docsId };
                if (docsId.IndexOf(",") > 0) docId = docsId.Split(',');
                InfoUtente infoUtente = UserManager.GetInfoUser();

                DocsPaWR.Folder fromFolder = ProjectManager.getFolder(this, oldParentId);
                fromFolder = ProjectManager.getFolder(this, fromFolder);

                DocsPaWR.Folder toFolder = ProjectManager.getFolder(this, parentId);
                toFolder = ProjectManager.getFolder(this, toFolder);

                foreach (string docNumber in docId)
                {
                    string idProfile = docNumber.Replace("doc", "");
                    SchedaDocumento doc = DocumentManager.getDocumentDetails(this, idProfile, idProfile);

                    if (doc.protocollo != null && doc.protocollo.protocolloAnnullato != null)
                    {
                        // alert documento annullato
                        this.html_data.Text += "<" + "script type=\"text/javascript\">ajaxDialogModal('AlertProjectMoveDocNoAllowedProtCanceled', 'warning', '');<" + "/script>\n";
                    }
                    //else if (Convert.ToInt32(doc.accessRights) < Convert.ToInt32(HMdiritti.HMdiritti_Write))
                    //{
                    //    // alert no diritti in scrittura
                    //    this.html_data.Text += "<" + "script type=\"text/javascript\">ajaxDialogModal('AlertProjectMoveDocNoAllowedNoWrite', 'warning', '');<" + "/script>\n";
                    //}
                    else
                    {
                        // add new
                        UIManager.AddDocInProjectManager.addDocumentoInFolder(idProfile, toFolder.systemID, infoUtente);

                        // delete old
                        ValidationResultInfo risultato = UIManager.ProjectManager.deleteDocFromFolder(fromFolder, infoUtente, idProfile, RapidClassificationRequired.ToString(), out messaggio);
                        if (risultato != null && risultato.BrokenRules.Length > 0)
                            this.html_data.Text += "<" + "script type=\"text/javascript\">ajaxDialogModal('AlertProjectRemoveDoc', 'error', '');<" + "/script>\n";
                        else
                            if (!string.IsNullOrEmpty(messaggio))
                                this.html_data.Text += "<" + "script type=\"text/javascript\">ajaxDialogModal('ErrorProjectRemoveDoc', 'error', '');<" + "/script>\n";
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        // verifica permessi drag n drop documento
        /*
         * 1. presenza microfunzione ("FASC_INS_DOC")
         * 2. presenza microfunzione ("DO_DEL_DOC_FASC")
         * */

        // verifica permessi drag n drop fascicolo
        /*
         * 1. presenza microfunzione ("FASC_INS_DOC")
         * 2. presenza microfunzione ("DO_DEL_DOC_FASC")
         * 3. presenza microfunzione AGG_SOTTOFASC
         * 4. presenza microfunzione DEL_SOTTOFASC
         * 5. presenza microfunzione MOD_SOTTOFASC
         * */

    }
}