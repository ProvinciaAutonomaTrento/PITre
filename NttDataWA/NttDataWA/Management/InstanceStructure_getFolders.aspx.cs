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

namespace NttDataWA.Management
{
    public partial class InstanceStructure_getFolders : System.Web.UI.Page
    {

        private List<DocsPaWR.InstanceAccessDocument> InstanceDocuments
        {
            get
            {
                List<DocsPaWR.InstanceAccessDocument> result = null;
                if (HttpContext.Current.Session["InstanceDocuments"] != null)
                {
                    result = HttpContext.Current.Session["InstanceDocuments"] as List<DocsPaWR.InstanceAccessDocument>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["InstanceDocuments"] = value;
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

        private Templates TipologiaAttoIstanza
        {
            get
            {
                return HttpContext.Current.Session["TipologiaAttoIstanza"] as Templates;
            }
            set
            {
                HttpContext.Current.Session["TipologiaAttoIstanza"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string qs = Request.QueryString.ToString();
                string f = Request.Form.ToString();

                if (!IsPostBack)
                {
                    this.InitDocuments();
                }

                if (this.InstanceDocuments != null)
                {
                    if (Request.QueryString["id"] == "0")
                    {
                        // visualizzazione fascicoli primo livello
                        this.caricaFoldersFascicoli();

                        // visualizzazione documenti "sciolti"
                        this.caricaDocumenti(null);
                    }
                    else if (Request.QueryString["id"] != null && Request.QueryString["id"] != "0")
                    {
                        string folderId = Request.QueryString["id"].Replace("node_", "").Replace("root_", "");
                        DocsPaWR.Folder folder = new DocsPaWR.Folder();
                        if (!string.IsNullOrEmpty(folderId) && folderId.IndexOf("doc") < 0)
                        {
                            folder = ProjectManager.getFolder(this, folderId);
                            folder = ProjectManager.getFolder(this, folder);
                            //this.html_data.Text += this.GetSubFolders(folder, false, true, false);
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

        private void InitDocuments()
        {

            if (UIManager.InstanceAccessManager.getInstanceAccessInSession() != null && UIManager.InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS != null && UIManager.InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS.Length > 0)
            {
                this.InstanceDocuments = UIManager.InstanceAccessManager.getInstanceAccessInSession().DOCUMENTS.ToList<InstanceAccessDocument>();
            }
            else
            {
                this.InstanceDocuments = new List<InstanceAccessDocument>();
            }

        }

        private string caricaDocumenti(DocsPaWR.Folder folder)
        {
            try
            {
                string type = (folder == null) ? "root" : "node";

                // identificazione documenti
                List<DocsPaWR.InstanceAccessDocument> docs = (folder == null) ? this.InstanceDocuments.Where(e => e.INFO_PROJECT == null && (this.TipologiaAttoIstanza == null || !e.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(this.TipologiaAttoIstanza.DESCRIZIONE))).ToList() : this.InstanceDocuments.Where(e => e.INFO_PROJECT != null && (e.INFO_PROJECT.ID_PROJECT.Equals(folder.systemID) || (e.INFO_PROJECT.ID_PROJECT.Equals(folder.idFascicolo) && e.INFO_PROJECT.ID_FASCICOLO == "") && folder.idFascicolo.Equals(folder.idParent))).ToList<DocsPaWR.InstanceAccessDocument>();

                string html = string.Empty;
                foreach (InstanceAccessDocument doc in docs)
                {
                    string cssClass = "nosons";
                    if (doc.ATTACHMENTS != null && doc.ATTACHMENTS.Length > 0)
                        cssClass = string.Empty;

                    string numeroDocumento = doc.DOCNUMBER;
                    string numeroProtocollo = doc.INFO_DOCUMENT.NUMBER_PROTO;
                    string data = utils.formatDataDocsPa(doc.INFO_DOCUMENT.DATE_CREATION);
                    string oggetto = doc.INFO_DOCUMENT.OBJECT;
                    string extension = !String.IsNullOrEmpty(doc.INFO_DOCUMENT.EXTENSION) && doc.INFO_DOCUMENT.EXTENSION != "0" ? doc.INFO_DOCUMENT.EXTENSION : String.Empty;
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

                    string check = string.Empty;
                    if (doc.ENABLE)
                    {
                        check = " jstree-checked";
                    }

                    html += "   <li id=\"doc_" + doc.ID_INSTANCE_ACCESS_DOCUMENT +"\" rel=\"" + extension + "\" class=\"" + cssClass + " doc_" + numeroDocumento + check+"\" data-title=\"" + utils.FormatJs(utils.FormatHtml(oggetto)) + "\">\n"
                          + "       <a href=\"#\" class=\"document_ellipsis clickableRight-no-ie\" title=\"" + utils.FormatJs(utils.FormatHtml(oggetto)) + "\">" + utils.FormatHtml(oggetto) + "</a>\n";
                    if (doc.ATTACHMENTS != null && doc.ATTACHMENTS.Length > 0)
                    {
                        html += "<ul>\n";
                        //for (int k = 0; k < doc.ATTACHMENTS.Length; k++)
                        if (doc.ATTACHMENTS != null && doc.ATTACHMENTS.Length > 0)
                        {
                            html += this.GetAttachments(doc.ATTACHMENTS);
                        }
                        html += "</ul>\n";
                    }
                    html += "   </li>\n";
                }

                return html;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return string.Empty;
            }
        }

        private string GetAttachments(InstanceAccessAttachments[] attachments)
        {
            string html = string.Empty;
            int i = 1;

            foreach (InstanceAccessAttachments attach in attachments)
            {
                string extension = !String.IsNullOrEmpty(attach.EXTENSION) && attach.EXTENSION != "0" ? attach.EXTENSION : String.Empty;
                if (string.IsNullOrEmpty(extension))
                {
                    extension = "no_file";
                }
                else
                {
                    extension = FileManager.getEstensioneIntoSignedFile(extension.ToLower());
                }

                //string title = "Allegato: " + attach.FILE_NAME;
                string title = attach.FILE_NAME;
                string check = string.Empty;
                if (attach.ENABLE)
                {
                    check = " jstree-checked";
                }
                html += "   <li id=\"attach_" + attach.SYSTEM_ID + "\" rel=\"" + extension + "\" class=\"nosons attach_" + attach.SYSTEM_ID + check+"\" data-title=\"" + utils.FormatJs(utils.FormatHtml(title)) + "\">\n"
                      + "       <a href=\"#\" class=\"document_ellipsis clickableRight-no-ie\" title=\"" + utils.FormatJs(utils.FormatHtml(title)) + "\">" + utils.FormatHtml(title) + "</a>\n";
                i++;
            }

            return html;
        }

        private void caricaFoldersFascicoli()
        {
            try
            {
                string html = string.Empty;

                List<string> prjs = (from d in this.InstanceDocuments where d.INFO_PROJECT != null && string.IsNullOrEmpty(d.INFO_PROJECT.ID_FASCICOLO) select d.INFO_PROJECT.ID_PROJECT).ToList().Distinct().ToList();

                List<string> idListProjectIns = new List<string>();

                foreach (string prj in prjs)
                {
                    idListProjectIns.Add(prj);
                    DocsPaWR.Fascicolo fasc = ProjectManager.getFascicoloById(this, prj);
                    DocsPaWR.Folder folder = ProjectManager.getFolder(this, fasc);

                    html += this.GetSubFolders(folder, true, ref idListProjectIns);
                }

                List<string> prjNoProject = (from d in this.InstanceDocuments where d.INFO_PROJECT != null && !string.IsNullOrEmpty(d.INFO_PROJECT.ID_FASCICOLO) && (!idListProjectIns.Contains(d.INFO_PROJECT.ID_PROJECT)) select d.INFO_PROJECT.ID_FASCICOLO).ToList().Distinct().ToList();

                List<string> prjAlreadyInsert = new List<string>();
                foreach (string prjNo in prjNoProject)
                {
                    DocsPaWR.Fascicolo fasc = null;
                    DocsPaWR.Folder folder = null;
                    if (!prjAlreadyInsert.Contains(prjNo))
                    {
                        fasc = ProjectManager.getFascicoloById(this, prjNo);
                        folder = ProjectManager.getFolder(this, fasc);
                        prjAlreadyInsert.Add(prjNo);

                        html += this.GetSubFolders(folder, true, ref idListProjectIns);
                    }
                }

                html += this.caricaDocumenti(null);

                this.html_data.Text = "<ul>\n"
                                      + html
                                      + "</ul>";
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private string GetSubFolders(DocsPaWR.Folder folder, ref List<string> idListProjectIns)
        {
            bool isRoot = false;
            return GetSubFolders(folder, isRoot, ref idListProjectIns);
        }

        private string GetSubFolders(DocsPaWR.Folder folder, bool isRoot, ref List<string> idListProjectIns)
        {
            try
            {
                string type = isRoot ? "root" : "node";
                string htmlDoc = this.caricaDocumenti(folder);

                InstanceAccessDocument acc = new InstanceAccessDocument();
                string idObject = string.Empty;
                string html = string.Empty;
                if (isRoot)
                {
                    acc = (this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && x.INFO_PROJECT.ID_PROJECT == folder.idFascicolo)).FirstOrDefault();
                }
                else
                {
                    acc = (this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && x.INFO_PROJECT.ID_PROJECT == folder.systemID)).FirstOrDefault();
                }

                if (acc != null)
                {
                    idListProjectIns.Add(acc.INFO_PROJECT.ID_PROJECT);
                    string idNode = acc.ID_INSTANCE_ACCESS_DOCUMENT;
                    string check = string.Empty;
                    //if (acc.ENABLE)
                    //{
                    //    check = "class=\"jstree-checked\"";
                    //}
                    check = "class=\"jstree-unchecked\"";
                    html = "   <li " + check + "rel=\"directory\" id=\"" + type + "_" + idNode + "\" data-title=\"" + utils.FormatJs(utils.FormatHtml(folder.descrizione)) + "\">\n"
                                + "       <a href=\"#\" class=\"document_ellipsis clickableRight\" title=\"" + utils.FormatJs(utils.FormatHtml(folder.descrizione)) + "\">" + utils.FormatHtml(folder.descrizione) + "</a>\n";
                    if (folder.childs.Length > 0 || !string.IsNullOrEmpty(htmlDoc))
                    {
                        html += "<ul>\n";
                        if (folder.childs.Length > 0)
                            for (int k = 0; k < folder.childs.Length; k++)
                                html += this.GetSubFolders(folder.childs[k], ref idListProjectIns);

                        if (!string.IsNullOrEmpty(htmlDoc))
                            html += htmlDoc;

                        html += "</ul>\n";
                    }
                    html += "   </li>\n";
                }
                else
                {
                    string typeNode = string.Empty;
                    if (folder.idFascicolo.Equals(folder.idParent))
                    {
                        idObject = folder.idFascicolo;
                        typeNode = "rootNo";
                    }
                    else
                    {
                        typeNode = "nodeNo";
                        idObject = folder.systemID;
                    }

                    string check = string.Empty;
                    if (acc!=null && acc.ENABLE)
                    {
                        check = "class=\"jstree-checked\"";
                    }

                    html = "   <li " + check + " id=\"" + typeNode + "_" + folder.idFascicolo + "\" data-title=\"" + utils.FormatJs(utils.FormatHtml(folder.descrizione)) + "\">\n"
                                + "       <a href=\"#\" class=\"document_ellipsis clickableRight\" title=\"" + utils.FormatJs(utils.FormatHtml(folder.descrizione)) + "\">" + utils.FormatHtml(folder.descrizione) + "</a>\n";
                    if (folder.childs.Length > 0 || !string.IsNullOrEmpty(htmlDoc))
                    {
                        html += "<ul>\n";
                        if (folder.childs.Length > 0)
                            for (int k = 0; k < folder.childs.Length; k++)
                                html += this.GetSubFolders(folder.childs[k], ref idListProjectIns);

                        if (!string.IsNullOrEmpty(htmlDoc))
                            html += htmlDoc;

                        html += "</ul>\n";
                    }
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

    }
}