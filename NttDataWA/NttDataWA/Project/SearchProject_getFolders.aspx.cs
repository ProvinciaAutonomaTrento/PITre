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
    public partial class SearchProject_getFolders : System.Web.UI.Page
    {

        private DocsPaWR.Fascicolo Project
        {
            get
            {
                Fascicolo result = null;
                if (HttpContext.Current.Session["searchproject"] != null)
                {
                    result = HttpContext.Current.Session["searchproject"] as Fascicolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["searchproject"] = value;
            }
        }

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

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
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
                        case "create_node":
                            // id=node_12595771
                            // position=1
                            // title=gggg
                            // type=default
                            Session["dataentry_node_folder"] = folder;
                            this.html_data.Text = "<" + "script type=\"text/javascript\">ajaxModalPopupCreateNode();<" + "/script>";
                            break;

                        case "rename_node":
                            // operation=rename_node
                            // title=ffffff
                            // id=rootnode_12595773
                            Session["dataentry_node_folder"] = folder;
                            this.html_data.Text = "<" + "script type=\"text/javascript\">ajaxModalPopupModifyNode();<" + "/script>";
                            break;

                        case "move_node":
                            // id=node_34342324
                            // parent=node_35366466
                            string parentId = Request.Form["parent"].Replace("node_", "").Replace("root_", "");
                            this.MoveFolder(folder, parentId);
                            break;

                        case "remove_node":
                            // id=node_12595771
                            Session["remove_node_folder"] = folder;
                            this.html_data.Text = "<" + "script type=\"text/javascript\">ajaxConfirmModal('ConfirmProjectDeleteSubset', 'HiddenRemoveNode', '');<" + "/script>";
                            break;

                        case "drag_documents":
                            folderId = Request.Form["ref"].Replace("node_", "").Replace("root_", "");
                            folder = new DocsPaWR.Folder();
                            if (!string.IsNullOrEmpty(folderId) && folderId.IndexOf("doc") < 0)
                            {
                                folder = ProjectManager.getFolder(this, folderId);
                                folder = ProjectManager.getFolder(this, folder);
                            }

                            string docsId = Request.Form["ids"];
                            this.MoveDocuments(docsId, this.Project.folderSelezionato, folder);
                            break;
                    }
                }
                else
                {
                    //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
                    Fascicolo Prj = this.Project;
                    if ((Prj.systemID != null && !string.IsNullOrEmpty(Prj.systemID)) && ProjectManager.CheckRevocationAcl(Prj))
                    {
                        this.html_data.Text = "<" + "script type=\"text/javascript\">function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}<" + "/script>\n";
                        return;
                    }

                    if (Request.QueryString["id"] == "0")
                    {
                        Fascicolo Fasc = this.Project;
                        DocsPaWR.Folder folder = ProjectManager.getFolder(this, Fasc);
                        this.caricaFoldersFascicolo(folder);
                    }
                    else if (Request.QueryString["q"] != null && Request.QueryString["q"].Length > 0)
                    {
                        string descrizione = Request.QueryString["q"].Trim();
                        Fascicolo fascSelezionato = this.Project;
                        string idFascicolo = fascSelezionato.systemID;
                        if (descrizione != string.Empty)
                        {
                            Folder[] risultatiFolder = ProjectManager.getFolderByDescrizione(this, idFascicolo, descrizione);

                            if (risultatiFolder != null)
                            {
                                for (int i = 0; i < risultatiFolder.Length; i++)
                                {
                                    if (this.html_data.Text.Length > 0) this.html_data.Text += ", ";
                                    this.html_data.Text += "	{ \"id\": \"node_" + ((Folder)risultatiFolder[i]).systemID + "\" , \"isResult\": \"true\" }";
                                }

                                this.html_data.Text = "[\n"
                                                    + html_data.Text
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

        private void MoveDocuments(string docsId, Folder fromFolder, Folder toFolder)
        {
            try
            {
                string messaggio = string.Empty;
                string[] docId = new string[1] { docsId };
                if (docsId.IndexOf(",") > 0) docId = docsId.Split(',');
                InfoUtente infoUtente = UserManager.GetInfoUser();

                foreach (string docNumber in docId)
                {
                    string idProfile = docNumber.Replace("doc", "");
                    SchedaDocumento doc = DocumentManager.getDocumentDetails(this, idProfile, idProfile);

                    if (fromFolder.systemID != toFolder.systemID)
                    {
                        if (doc.protocollo != null && doc.protocollo.protocolloAnnullato != null)
                        {
                            // alert documento annullato
                            this.html_data.Text += "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('AlertProjectMoveDocNoAllowedProtCanceled', 'warning', '');} else {parent.ajaxDialogModal('AlertProjectMoveDocNoAllowedProtCanceled', 'warning', '');}\n";
                        }
                        //else if (Convert.ToInt32(doc.accessRights) < Convert.ToInt32(HMdiritti.HMdiritti_Write))
                        //{
                        //    // alert no diritti in scrittura
                        //    this.html_data.Text += "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('AlertProjectMoveDocNoAllowedNoWrite', 'warning', '');} else {parent.ajaxDialogModal('AlertProjectMoveDocNoAllowedNoWrite', 'warning', '');}\n";
                        //}
                        else
                        {
                            // add new
                            UIManager.AddDocInProjectManager.addDocumentoInFolder(idProfile, toFolder.systemID, infoUtente);

                            // delete old
                            ValidationResultInfo risultato = UIManager.ProjectManager.deleteDocFromFolder(fromFolder, infoUtente, idProfile, RapidClassificationRequired.ToString(), out messaggio);
                            if (risultato != null && risultato.BrokenRules.Length > 0)
                                this.html_data.Text += "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('AlertProjectRemoveDoc', 'error', '');} else {parent.ajaxDialogModal('AlertProjectRemoveDoc', 'error', '');}\n";
                            else
                                if (!string.IsNullOrEmpty(messaggio) && messaggio != "")
                                    this.html_data.Text += "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorProjectRemoveDoc', 'error', '');} else {parent.ajaxDialogModal('ErrorProjectRemoveDoc', 'error', '');}\n";
                        }
                    }
                }

                this.html_data.Text = "<" + "script type=\"\">\n"
                                    + this.html_data.Text
                                    + "$('#BtnRebindGrid').click();\n"
                                    + "<" + "/script>\n";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void MoveFolder(Folder folder, string parentId)
        {
            try
            {
                // verifica se num doc visibili = tot doc in folder
                bool docsCountVisibleIsEgualNotVisible = ProjectManager.GetIfDocumentiCountVisibleIsEgualNotVisible(this, folder);

                if (docsCountVisibleIsEgualNotVisible)
                {
                    ProjectManager.MoveFolder(this, folder.systemID, parentId);
                }
                else
                {
                    string msg = "WarningProjectImpossibleMoveHasChildren";
                    this.html_data.Text = "<" + "script type=\"text/javascript\">ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning'); JsTree();<" + "/script>";
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
            try
            {
                if (folder != null)
                {
                    string html = this.GetSubFolders(folder, true);
                    this.html_data.Text = "<ul>\n"
                                          + html
                                          + "</ul>";
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
            bool isRoot = false;
            return GetSubFolders(folder, isRoot);
        }

        private string GetSubFolders(DocsPaWR.Folder folder, bool isRoot)
        {
            try
            {
                Fascicolo proj = this.Project;
                bool FASC_INS_DOC = UserManager.IsAuthorizedFunctions("FASC_INS_DOC");
                bool DO_DEL_DOC_FASC = UserManager.IsAuthorizedFunctions("DO_DEL_DOC_FASC");
                bool FASC_NEW_FOLDER = UserManager.IsAuthorizedFunctions("FASC_NEW_FOLDER");
                bool FASC_MOD_FOLDER = UserManager.IsAuthorizedFunctions("FASC_MOD_FOLDER");
                bool FASC_DEL_FOLDER = UserManager.IsAuthorizedFunctions("FASC_DEL_FOLDER");

                string type = isRoot ? "root" : "node";
                bool isUnamovable = !(
                    UserManager.IsAuthorizedFunctions("FASC_INS_DOC")
                    && UserManager.IsAuthorizedFunctions("DO_DEL_DOC_FASC")
                    && UserManager.IsAuthorizedFunctions("FASC_NEW_FOLDER")
                    && UserManager.IsAuthorizedFunctions("FASC_MOD_FOLDER")
                    && UserManager.IsAuthorizedFunctions("FASC_DEL_FOLDER")
                    && Convert.ToInt32(this.Project.accessRights) >= Convert.ToInt32(HMdiritti.HMdiritti_Write)
                    ) ? true : false;
                string cssClass = (isRoot || isUnamovable) ? " class=\"jstree-unamovable\"" : "";

                string html = "   <li id=\"" + type + "_" + folder.systemID + "\"" + cssClass + " data-title=\"" + utils.FormatJs(utils.FormatHtml(folder.descrizione)) + "\">\n"
                            + "       <a href=\"#\" class=\"document_ellipsis clickableRight\" title=\"" + utils.FormatJs(utils.FormatHtml(folder.descrizione)) + "\">" + utils.FormatHtml(folder.descrizione) + "</a>\n";
                if (folder.childs.Length > 0)
                {
                    html += "<ul>\n";
                    for (int k = 0; k < folder.childs.Length; k++)
                        html += this.GetSubFolders(folder.childs[k]);
                    html += "</ul>\n";
                }

                return html;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        private bool CreateFolder(DocsPaWR.Folder folder, string parentId)
        {
            try
            {
                // INSERIMENTO
                DocsPaWR.ResultCreazioneFolder result;
                if (!this.CreateNewFolder(folder, parentId, out result))
                {
                    // Visualizzazione messaggio di errore
                    string errorMessage = string.Empty;
                    if (result == DocsPaWR.ResultCreazioneFolder.FOLDER_EXIST)
                    {
                        string msg = "WarningProjectDuplicateSubset";
                        this.html_data.Text = "<" + "script type=\"text/javascript\">ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning');<" + "/script>";
                    }
                    else
                    {
                        string msg = "WarningProjectCreateError";
                        this.html_data.Text = "<" + "script type=\"text/javascript\">ajaxDialogModal('" + utils.FormatJs(msg) + "', 'warning');<" + "/script>";
                    }

                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        private bool CreateNewFolder(DocsPaWR.Folder folderSelected, string parentId, out DocsPaWR.ResultCreazioneFolder result)
        {
            bool retValue = false;
            result = DocsPaWR.ResultCreazioneFolder.GENERIC_ERROR;

            try
            {
                if (folderSelected != null)
                {
                    DocsPaWR.Folder newFolder = new DocsPaWR.Folder();

                    newFolder.idFascicolo = this.Project.systemID;
                    newFolder.idParent = parentId;
                    newFolder.descrizione = folderSelected.descrizione;

                    ProjectManager.newFolder(this, ref newFolder, UserManager.GetInfoUser(), RoleManager.GetRoleInSession(), out result);
                    retValue = (result == DocsPaWR.ResultCreazioneFolder.OK);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }

            return retValue;
        }

    }
}