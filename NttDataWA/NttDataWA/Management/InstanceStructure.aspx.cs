using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;

namespace NttDataWA.Management
{
    public partial class InstanceStructure : System.Web.UI.Page
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

        private bool EsisteDichiarazioneConformita
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["EsisteDichiarazioneConformita"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["EsisteDichiarazioneConformita"].ToString());
                }
                return result;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
                this.InstanceTabs.PageCaller = "STRUCTURE";
                this.InstanceTabs.RefreshTabs();
            }
            else
            {
                if (!string.IsNullOrEmpty(this.treenode_clickDel.Value))
                {
                    this.BtnContextMenu_Click();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('treenode_clickDel','');", true);
                }
                if (!string.IsNullOrEmpty(this.treenode_clickDelAll.Value))
                {
                    this.BtnCheckedDocuments_Click();
                    this.treenode_clickDelAll.Value = string.Empty;
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('treenode_clickDelAll','');", true);
                }
            }
        }

        protected void InitializePage()
        {
            this.InitializeLabel();
            this.InitInitiallyOpenTree();
            DocsPaWR.InstanceAccess instance = InstanceAccessManager.getInstanceAccessInSession();
            if (instance != null)
            {
                this.projectLblCodiceGenerato.Text = instance.ID_INSTANCE_ACCESS;
            }
            if (this.EsisteDichiarazioneConformita)
            {
                this.InstanceDetailsDelete.Enabled = false;
            }
        }

        private void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.litTreeExpandAll.Text = Utils.Languages.GetLabelFromCode("projectTreeExpandAll", language);
            this.litTreeCollapseAll.Text = Utils.Languages.GetLabelFromCode("projectTreeCollapseAll", language);
            //this.LnkViewDocument.Text = Utils.Languages.GetLabelFromCode("projectLnkViewDocument", language);
            this.InstanceDetailsDelete.Text = Utils.Languages.GetLabelFromCode("InstanceDetailsDelete", language);
        }

        private void InitInitiallyOpenTree()
        {
            //Fascicolo Fasc = ProjectManager.getProjectInSession();
            //DocsPaWR.Folder folder = ProjectManager.getFolder(this, Fasc);
            //this.jstree_initially_open.Text = "\"root_" + folder.systemID + "\"";
            //this.treenode_sel.Value = "root_" + folder.systemID;
        }

        protected void LnkViewDocument_Click(object sender, EventArgs e)
        {
            Session["IsInvokingFromProjectStructure"] = null;
            SchedaDocumento schedaDocumento = UIManager.DocumentManager.getSelectedRecord();
            //Fascicolo fascicolo = UIManager.ProjectManager.getProjectInSession();
            //Session["isZoom"] = null;
            List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
            Navigation.NavigationObject obj = navigationList.Last();
            Navigation.NavigationObject newObj = Navigation.NavigationUtils.CloneObject(obj);

            newObj.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_STRUCTURE.ToString(), string.Empty);
            newObj.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_STRUCTURE.ToString(), true,this.Page);
            newObj.CodePage = Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_STRUCTURE.ToString();
            newObj.Page = "INSTANCESTRUCTURE.ASPX";
            newObj.IdObject = schedaDocumento.systemId;
            newObj.OriginalObjectId = schedaDocumento.systemId;
            //newObj.folder = fascicolo.folderSelezionato;
            //newObj.idProject = fascicolo.systemID;
            //int indexElement = ((rowIndex + 1) / 2) + this.PageSize * (this.SelectedPage - 1);
            //newObj.DxPositionElement = indexElement.ToString();

            if (obj.NamePage.Equals(Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.MANAGEMENT_INSTANCE_STRUCTURE.ToString(), string.Empty)) && !string.IsNullOrEmpty(obj.IdObject) && obj.IdObject.Equals(newObj.idProject))
            {
                navigationList.Remove(obj);
            }
            navigationList.Add(newObj);
            Navigation.NavigationUtils.SetNavigationList(navigationList);

            Response.Redirect("../Document/document.aspx");
        }

        protected void BtnInstanceDetailsDelete_Click(object sender, EventArgs e)
        {
            string value = this.treenode_checked.Value;
        }

        protected void BtnChangeSelectedDocument_Click(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(this.treenode_sel.Value))
            //{
            //    InstanceAccessDocument acc = new InstanceAccessDocument();
            //    acc = (this.InstanceDocuments.Where(x => x.ID_INSTANCE_ACCESS_DOCUMENT == this.treenode_sel.Value.Split('_')[1])).FirstOrDefault();

            //    Session["IsInvokingFromProjectStructure"] = true;
            //    string value = this.treenode_sel.Value;
            //    string[] words = this.treenode_sel.Value.Split('_');
            //    if (words != null && words.Length > 1)
            //    {
            //        value = words[words.Length - 1];
            //    }

            //    if (acc != null)
            //    {
            //        SchedaDocumento doc = DocumentManager.getDocumentDetails(this, value, value);
            //        DocumentManager.setSelectedRecord(doc);
            //        FileManager.setSelectedFile(doc.documenti[0]);
            //        this.ViewDocument.Page_Load(null, null);
            //        this.unPnlVD.Visible = true;
            //        this.unPnlViewDocument.Update();
            //    }            
            //}
            //else
            //{
            //    DocumentManager.setSelectedRecord(null);
            //    this.unPnlVD.Visible = false;
            //    this.unPnlViewDocument.Update();
            //}

            List<InstanceAccessDocument> listDoc = new List<InstanceAccessDocument>();
            List<InstanceAccessAttachments> listAtt = new List<InstanceAccessAttachments>();
            InstanceAccessDocument acc = new InstanceAccessDocument();
            InstanceAccessAttachments att = new InstanceAccessAttachments();
            string idObject = string.Empty;
            if (!string.IsNullOrEmpty(this.treenode_sel.Value))
            {
                string documentID = string.Empty;
                //Session["IsInvokingFromProjectStructure"] = true;
                string value = this.treenode_sel.Value;
                string[] words = this.treenode_sel.Value.Split('_');
                if (words != null && words.Length > 1)
                {
                    value = words[words.Length - 1];
                }

                switch (this.treenode_sel.Value.Split('_')[0])
                {

                    case "doc":
                        acc = (this.InstanceDocuments.Where(x => x.ID_INSTANCE_ACCESS_DOCUMENT == this.treenode_sel.Value.Split('_')[1])).FirstOrDefault();
                        documentID = acc.DOCNUMBER;
                        break;
                    case "attach":
                        foreach (InstanceAccessDocument instanceDoc in this.InstanceDocuments)
                        {
                            if (instanceDoc.ATTACHMENTS != null && instanceDoc.ATTACHMENTS.Length > 0)
                            {
                                att = instanceDoc.ATTACHMENTS.Where(p => p.SYSTEM_ID.Equals(this.treenode_sel.Value.Split('_')[1])).FirstOrDefault();
                                instanceDoc.ATTACHMENTS.ToList<InstanceAccessAttachments>().RemoveAll(p => p.SYSTEM_ID.Equals(this.treenode_sel.Value.Split('_')[1]));
                                if (att != null)
                                {
                                    documentID = att.ID_ATTACH;
                                    break;
                                }
                            }

                        }
                        break;
                }

                if (!string.IsNullOrEmpty(documentID))
                {

                    SchedaDocumento doc = DocumentManager.getDocumentDetails(this, documentID, documentID);
                    DocumentManager.setSelectedRecord(doc);
                    FileManager.setSelectedFile(doc.documenti[0]);
                    this.ViewDocument.Page_Load(null, null);
                    this.unPnlVD.Visible = true;
                    this.unPnlViewDocument.Update();
                }
                else
                {
                    DocumentManager.setSelectedRecord(null);
                    this.unPnlVD.Visible = false;
                    this.unPnlViewDocument.Update();
                }
            }
            else
            {
                DocumentManager.setSelectedRecord(null);
                this.unPnlVD.Visible = false;
                this.unPnlViewDocument.Update();
            }
        }

        protected void BtnCheckedDocuments_Click()
        {
            string value = this.treenode_checked.Value;
            string idObject = string.Empty;
            List<InstanceAccessDocument> listDoc = new List<InstanceAccessDocument>();
            List<InstanceAccessDocument> listDocApp = new List<InstanceAccessDocument>();
            List<InstanceAccessAttachments> listAtt = new List<InstanceAccessAttachments>();
            List<InstanceAccessAttachments> listAttApp = new List<InstanceAccessAttachments>();
            //listDoc = (from d in this.InstanceDocuments select new{instance=d,}).up
            InstanceAccessDocument acc = new InstanceAccessDocument();

            //Inizialmente imposto tutti gli ENABLED dei documenti, e relativi allegati, a false
            for (int i = 0; i < this.InstanceDocuments.Count(); i++)
            {
                this.InstanceDocuments[i].ENABLE = false;
                listDoc.Add(this.InstanceDocuments[i]);
                if (InstanceDocuments[i].ATTACHMENTS != null && InstanceDocuments[i].ATTACHMENTS.Length > 0)
                {
                    for (int j = 0; j < InstanceDocuments[i].ATTACHMENTS.Count(); j++)
                    {
                        InstanceDocuments[i].ATTACHMENTS[j].ENABLE = false;
                        listAtt.Add(InstanceDocuments[i].ATTACHMENTS[j]);
                    }
                }
            }
            if (!string.IsNullOrEmpty(value))
            {
                string[] values = value.Split(',');
                if (values != null && values.Length > 0)
                {


                    foreach (string val in values)
                    {

                        acc = (this.InstanceDocuments.Where(x => x.ID_INSTANCE_ACCESS_DOCUMENT == val.Split('_')[1])).FirstOrDefault();
                        switch (val.Split('_')[0])
                        {
                                /*
                            case "root":
                                if (acc != null)
                                {
                                    idObject = acc.INFO_PROJECT.ID_PROJECT;
                                    listDocApp = this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject)).ToList<InstanceAccessDocument>();
                                    if (listDocApp != null && listDocApp.Count() > 0)
                                    {
                                        foreach (InstanceAccessDocument d in listDocApp)
                                        {
                                            int index = listDoc.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.ID_INSTANCE_ACCESS_DOCUMENT.Equals(d.ID_INSTANCE_ACCESS_DOCUMENT)).index;
                                            listDoc[index].ENABLE = true;
                                        }
                                    }
                                    this.InstanceDocuments.Clear();
                                    this.InstanceDocuments.AddRange(listDoc);
                                }
                                break;
                            case "node":
                                if (acc != null)
                                {
                                    idObject = acc.INFO_PROJECT.ID_PROJECT;
                                    listDocApp = this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject)).ToList<InstanceAccessDocument>();
                                    if (listDocApp != null && listDocApp.Count() > 0)
                                    {
                                        foreach (InstanceAccessDocument d in listDocApp)
                                        {
                                            int index = listDoc.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.ID_INSTANCE_ACCESS_DOCUMENT.Equals(d.ID_INSTANCE_ACCESS_DOCUMENT)).index;
                                            listDoc[index].ENABLE = true;
                                        }
                                    }
                                    this.InstanceDocuments.Clear();
                                    this.InstanceDocuments.AddRange(listDoc);
                                }
                                break;
                                 * */
                            case "doc":
                                if (acc != null)
                                {
                                    idObject = acc.ID_INSTANCE_ACCESS_DOCUMENT;
                                    List<InstanceAccessDocument> listInstanceAccessDocument = new List<InstanceAccessDocument>();
                                    listDocApp = this.InstanceDocuments.Where(x => x.ID_INSTANCE_ACCESS_DOCUMENT == idObject).ToList<InstanceAccessDocument>();
                                    if (listDocApp != null && listDocApp.Count() > 0)
                                    {
                                        foreach (InstanceAccessDocument d in listDocApp)
                                        {
                                            int index = listDoc.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.ID_INSTANCE_ACCESS_DOCUMENT.Equals(d.ID_INSTANCE_ACCESS_DOCUMENT)).index;
                                            listDoc[index].ENABLE = true;
                                        }
                                    }
                                    this.InstanceDocuments.Clear();
                                    this.InstanceDocuments.AddRange(listDoc);
                                }
                                break;
                            case "attach":

                                foreach (InstanceAccessDocument instanceDoc in this.InstanceDocuments)
                                {
                                    if (instanceDoc.ATTACHMENTS != null && instanceDoc.ATTACHMENTS.Length > 0)
                                    {
                                        listAttApp = instanceDoc.ATTACHMENTS.ToList<InstanceAccessAttachments>().Where(p => p.SYSTEM_ID.Equals(val.Split('_')[1])).ToList<InstanceAccessAttachments>();
                                        if (listAttApp != null && listAttApp.Count() > 0)
                                        {
                                            foreach (InstanceAccessAttachments a in listAttApp)
                                            {
                                                int index = listAtt.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.SYSTEM_ID.Equals(a.SYSTEM_ID)).index;
                                                listAtt[index].ENABLE = true;

                                                int indexListAttInSession = instanceDoc.ATTACHMENTS.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.SYSTEM_ID.Equals(a.SYSTEM_ID)).index;
                                                instanceDoc.ATTACHMENTS[indexListAttInSession].ENABLE = true;
                                            }
                                            break;
                                        }
                                    }

                                }

                                break;
                            case "rootNo":
                                idObject = this.treenode_deleted.Value.Split('_')[1];
                                listDoc = this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject)).ToList<InstanceAccessDocument>();
                                UIManager.InstanceAccessManager.RemoveInstanceAccessDocuments(listDoc);
                                this.InstanceDocuments.RemoveAll(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject));
                                break;
                            case "nodeNo":
                                idObject = this.treenode_deleted.Value.Split('_')[1];
                                listDoc = this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject)).ToList<InstanceAccessDocument>();
                                UIManager.InstanceAccessManager.RemoveInstanceAccessDocuments(listDoc);
                                this.InstanceDocuments.RemoveAll(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject));
                                break;
                        }
                    }               
                }
            }
            UIManager.InstanceAccessManager.UpdateInstanceAccessDocumentEnable(listDoc);
            if (listAtt != null && listAtt.Count() > 0)
            {
                UIManager.InstanceAccessManager.UpdateInstanceAccessAttachmentsEnable(listAtt);
            }
            DocsPaWR.InstanceAccess inst = UIManager.InstanceAccessManager.getInstanceAccessInSession();
            inst.DOCUMENTS = this.InstanceDocuments.ToArray<InstanceAccessDocument>();
            UIManager.InstanceAccessManager.setInstanceAccessInSession(inst);

            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshJsTree", "JsTree();", true);

            this.treenode_deleted.Value = string.Empty;
            this.upPnlButtons.Update();
        }

        /*
         * protected void BtnCheckedDocuments_Click()
        {
            string value = this.treenode_checked.Value;
            string idObject = string.Empty;
            List<InstanceAccessDocument> listDoc = new List<InstanceAccessDocument>();

            List<InstanceAccessAttachments> listAtt = new List<InstanceAccessAttachments>();
            InstanceAccessDocument acc = new InstanceAccessDocument();
            if (!string.IsNullOrEmpty(value))
            {
                string[] values = value.Split(',');
                if (values != null && values.Length > 0)
                {


                    foreach (string val in values)
                    {

                        acc = (this.InstanceDocuments.Where(x => x.ID_INSTANCE_ACCESS_DOCUMENT == val.Split('_')[1])).FirstOrDefault();
                        switch (val.Split('_')[0])
                        {

                            case "root":
                                if (acc != null)
                                {
                                    idObject = acc.INFO_PROJECT.ID_PROJECT;
                                    listDoc = this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject)).ToList<InstanceAccessDocument>();
                                    UIManager.InstanceAccessManager.RemoveInstanceAccessDocuments(listDoc);
                                    this.InstanceDocuments.RemoveAll(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject));
                                }
                                break;
                            case "node":
                                if (acc != null)
                                {
                                    idObject = acc.INFO_PROJECT.ID_PROJECT;
                                    listDoc = this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject)).ToList<InstanceAccessDocument>();
                                    UIManager.InstanceAccessManager.RemoveInstanceAccessDocuments(listDoc);
                                    this.InstanceDocuments.RemoveAll(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject));
                                }
                                break;

                            case "doc":
                                if (acc != null)
                                {
                                    idObject = acc.ID_INSTANCE_ACCESS_DOCUMENT;
                                    List<InstanceAccessDocument> listInstanceAccessDocument = new List<InstanceAccessDocument>();
                                    listDoc = this.InstanceDocuments.Where(x => x.ID_INSTANCE_ACCESS_DOCUMENT == idObject).ToList<InstanceAccessDocument>();
                                    //se ci sono allegati li prendo e li inserisco come documenti singoli
                                    foreach (InstanceAccessDocument instanceDoc in listDoc)
                                    {
                                        if (instanceDoc.ATTACHMENTS != null && instanceDoc.ATTACHMENTS.Length > 0)
                                        {

                                            foreach (InstanceAccessAttachments att in instanceDoc.ATTACHMENTS)
                                            {
                                                InstanceAccessDocument instInset = new InstanceAccessDocument();
                                                instInset.ID_INSTANCE_ACCESS = InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS;
                                                instInset.DOCNUMBER = att.ID_ATTACH;
                                                instInset.ATTACHMENTS = new InstanceAccessAttachments[0];
                                                instInset.INFO_DOCUMENT = new InfoDocument() { DOCNUMBER = att.ID_ATTACH };
                                                listInstanceAccessDocument.Add(instInset);
                                            }



                                        }
                                    }

                                    if (listInstanceAccessDocument != null && listInstanceAccessDocument.Count > 0 && InstanceAccessManager.InsertInstanceAccessDocuments(listInstanceAccessDocument))
                                    {
                                        DocsPaWR.InstanceAccess instanceAccess = InstanceAccessManager.GetInstanceAccessById(InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS);
                                        InstanceAccessManager.setInstanceAccessInSession(instanceAccess);
                                        this.InstanceDocuments = instanceAccess.DOCUMENTS.ToList<InstanceAccessDocument>();
                                    }
                                    UIManager.InstanceAccessManager.RemoveInstanceAccessDocuments(listDoc);
                                    this.InstanceDocuments.RemoveAll(x => x.ID_INSTANCE_ACCESS_DOCUMENT == idObject);
                                }
                                break;
                            case "attach":

                                foreach (InstanceAccessDocument instanceDoc in this.InstanceDocuments)
                                {
                                    if (instanceDoc.ATTACHMENTS != null && instanceDoc.ATTACHMENTS.Length > 0)
                                    {
                                        //listAtt = instanceDoc.ATTACHMENTS.ToList<InstanceAccessAttachments>().Where(p => p.ID_ATTACH.Equals(idObject)).ToList<InstanceAccessAttachments>();
                                        //instanceDoc.ATTACHMENTS.ToList<InstanceAccessAttachments>().RemoveAll(p => p.ID_ATTACH.Equals(idObject));

                                        listAtt = instanceDoc.ATTACHMENTS.ToList<InstanceAccessAttachments>().Where(p => p.SYSTEM_ID.Equals(val.Split('_')[1])).ToList<InstanceAccessAttachments>();
                                        instanceDoc.ATTACHMENTS.ToList<InstanceAccessAttachments>().RemoveAll(p => p.SYSTEM_ID.Equals(val.Split('_')[1]));



                                        if (listAtt != null && listAtt.Count > 0)
                                        {

                                            break;
                                        }
                                    }

                                }

                                UIManager.InstanceAccessManager.RemoveInstanceAccessAttachments(listAtt);

                                break;
                            case "rootNo":
                                idObject = this.treenode_deleted.Value.Split('_')[1];
                                listDoc = this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject)).ToList<InstanceAccessDocument>();
                                UIManager.InstanceAccessManager.RemoveInstanceAccessDocuments(listDoc);
                                this.InstanceDocuments.RemoveAll(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject));
                                break;
                            case "nodeNo":
                                idObject = this.treenode_deleted.Value.Split('_')[1];
                                listDoc = this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject)).ToList<InstanceAccessDocument>();
                                UIManager.InstanceAccessManager.RemoveInstanceAccessDocuments(listDoc);
                                this.InstanceDocuments.RemoveAll(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject));
                                break;
                        }
                    }
                    DocsPaWR.InstanceAccess inst = UIManager.InstanceAccessManager.getInstanceAccessInSession();
                    inst.DOCUMENTS = this.InstanceDocuments.ToArray<InstanceAccessDocument>();
                    UIManager.InstanceAccessManager.setInstanceAccessInSession(inst);

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshJsTree", "JsTree();", true);

                    this.treenode_deleted.Value = string.Empty;
                    this.upPnlButtons.Update();
                }
            }
        }
         * 
         * */

        protected void BtnContextMenu_Click()
        {
            List<InstanceAccessDocument> listDoc = new List<InstanceAccessDocument>();
            List<InstanceAccessAttachments> listAtt = new List<InstanceAccessAttachments>();
            InstanceAccessDocument acc = new InstanceAccessDocument();
            string idObject = string.Empty;
            if (!string.IsNullOrEmpty(this.treenode_deleted.Value))
            {

                acc = (this.InstanceDocuments.Where(x => x.ID_INSTANCE_ACCESS_DOCUMENT == this.treenode_deleted.Value.Split('_')[1])).FirstOrDefault();
                switch (this.treenode_deleted.Value.Split('_')[0])
                {
                    case "root":
                        idObject = acc.INFO_PROJECT.ID_PROJECT;
                        listDoc = this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject)).ToList<InstanceAccessDocument>();
                        UIManager.InstanceAccessManager.RemoveInstanceAccessDocuments(listDoc);
                        this.InstanceDocuments.RemoveAll(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject));
                        break;
                    case "node":
                        idObject = acc.INFO_PROJECT.ID_PROJECT;
                        listDoc = this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject)).ToList<InstanceAccessDocument>();
                        UIManager.InstanceAccessManager.RemoveInstanceAccessDocuments(listDoc);
                        this.InstanceDocuments.RemoveAll(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject));
                        break;
                    case "doc":
                        idObject = acc.ID_INSTANCE_ACCESS_DOCUMENT;
                        List<InstanceAccessDocument> listInstanceAccessDocument = new List<InstanceAccessDocument>();
                        listDoc = this.InstanceDocuments.Where(x => x.ID_INSTANCE_ACCESS_DOCUMENT == idObject).ToList<InstanceAccessDocument>();
                        //se ci sono allegati li prendo e li inserisco come documenti singoli
                        foreach (InstanceAccessDocument instanceDoc in listDoc)
                        {
                            if (instanceDoc.ATTACHMENTS != null && instanceDoc.ATTACHMENTS.Length > 0)
                            {

                                foreach (InstanceAccessAttachments att in instanceDoc.ATTACHMENTS)
                                {
                                    InstanceAccessDocument instInset = new InstanceAccessDocument();
                                    instInset.ID_INSTANCE_ACCESS = InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS;
                                    instInset.DOCNUMBER = att.ID_ATTACH;
                                    instInset.ATTACHMENTS = new InstanceAccessAttachments[0];
                                    instInset.INFO_DOCUMENT = new InfoDocument() { DOCNUMBER = att.ID_ATTACH };
                                    listInstanceAccessDocument.Add(instInset);
                                }



                            }
                        }

                        if (listInstanceAccessDocument != null && listInstanceAccessDocument.Count > 0 && InstanceAccessManager.InsertInstanceAccessDocuments(listInstanceAccessDocument))
                        {
                            DocsPaWR.InstanceAccess instanceAccess = InstanceAccessManager.GetInstanceAccessById(InstanceAccessManager.getInstanceAccessInSession().ID_INSTANCE_ACCESS);
                            InstanceAccessManager.setInstanceAccessInSession(instanceAccess);
                            this.InstanceDocuments = instanceAccess.DOCUMENTS.ToList<InstanceAccessDocument>();
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshJsTree", "JsTree();", true);
                        }
                        UIManager.InstanceAccessManager.RemoveInstanceAccessDocuments(listDoc);
                        this.InstanceDocuments.RemoveAll(x => x.ID_INSTANCE_ACCESS_DOCUMENT == idObject);
                        break;
                    case "attach":
                        foreach (InstanceAccessDocument instanceDoc in this.InstanceDocuments)
                        {
                            if (instanceDoc.ATTACHMENTS != null && instanceDoc.ATTACHMENTS.Length > 0)
                            {
                                //listAtt = instanceDoc.ATTACHMENTS.ToList<InstanceAccessAttachments>().Where(p => p.ID_ATTACH.Equals(idObject)).ToList<InstanceAccessAttachments>();
                                //instanceDoc.ATTACHMENTS.ToList<InstanceAccessAttachments>().RemoveAll(p => p.ID_ATTACH.Equals(idObject));

                                listAtt = instanceDoc.ATTACHMENTS.ToList<InstanceAccessAttachments>().Where(p => p.SYSTEM_ID.Equals(this.treenode_deleted.Value.Split('_')[1])).ToList<InstanceAccessAttachments>();
                                instanceDoc.ATTACHMENTS.ToList<InstanceAccessAttachments>().RemoveAll(p => p.SYSTEM_ID.Equals(this.treenode_deleted.Value.Split('_')[1]));


                                if (listAtt != null && listAtt.Count > 0)
                                {
                                    break;
                                }
                            }
                            // instanceDoc.ATTACHMENTS.ToList().RemoveAll(p => p.ID_ATTACH.Equals(this.treenode_deleted.Value.Split('_')[1]));
                        }

                        UIManager.InstanceAccessManager.RemoveInstanceAccessAttachments(listAtt);
                        /*
                                            InstanceAccessDocument doc = (from d in listInstanceAccessDocumentTemp
                                                                          where ((from a in d.ATTACHMENTS where a.ID_ATTACH.Equals("id") select a).Count() > 0)
                                                                          select d).FirstOrDefault();
                                            (listInstanceAccessDocumentTemp.Find(d => d.ID_INSTANCE_ACCESS_DOCUMENT.Equals(doc.ID_INSTANCE_ACCESS_DOCUMENT))).ATTACHMENTS.ToList().RemoveAll(a => a.ID_ATTACH.Equals("id"));
                        */
                        break;
                    case "rootNo":
                        idObject = this.treenode_deleted.Value.Split('_')[1];
                        listDoc = this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject)).ToList<InstanceAccessDocument>();
                        UIManager.InstanceAccessManager.RemoveInstanceAccessDocuments(listDoc);
                        this.InstanceDocuments.RemoveAll(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject));
                        break;
                    case "nodeNo":
                        idObject = this.treenode_deleted.Value.Split('_')[1];
                        listDoc = this.InstanceDocuments.Where(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject)).ToList<InstanceAccessDocument>();
                        UIManager.InstanceAccessManager.RemoveInstanceAccessDocuments(listDoc);
                        this.InstanceDocuments.RemoveAll(x => x.INFO_PROJECT != null && (x.INFO_PROJECT.ID_PROJECT == idObject || x.INFO_PROJECT.ID_FASCICOLO == idObject));
                        break;
                }

            }
            DocsPaWR.InstanceAccess inst = UIManager.InstanceAccessManager.getInstanceAccessInSession();
            inst.DOCUMENTS = this.InstanceDocuments.ToArray<InstanceAccessDocument>();
            UIManager.InstanceAccessManager.setInstanceAccessInSession(inst);

            //ScriptManager.RegisterStartupScript(this, this.GetType(), "jsTree", "JsTree();", true);
            //this.upnlStruttura.Update();

            this.treenode_deleted.Value = string.Empty;
            this.upPnlButtons.Update();
        }


    }
}