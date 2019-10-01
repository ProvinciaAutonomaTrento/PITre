using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using System.Collections;
using NttDatalLibrary;
using System.Text;
using System.Globalization;
using System.Data;
using log4net;

namespace NttDataWA.Document
{
    public partial class Classification : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(Classification));
        private const string PANEL_VERSION = "UpBottomButtons";
        private const string MODIFY_VERSION = "MODIFY_VERSION";
        private const string CONFIRM_REMOVE_VERSION = "CONFIRM_REMOVE_VERSION";

        private void ClearSessionProperties()
        {
            this.Project = null;
            //Laura 24 Marzo
            ProjectManager.removeFascicoloSelezionatoFascRapida();
            ProjectManager.setProjectInSessionForRicFasc(String.Empty);
            //da verificare
            DocumentManager.RemoveSelectedAttachId();
        }

        private string ReturnValue
        {
            get
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["ReturnValuePopup"].ToString()))
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Se ACL rimossa, allora visualizzo un messaggio di warning all'utente per poi reindirizzarlo alla HOME.
                if (!DocumentManager.IsNewDocument() && DocumentManager.CheckRevocationAcl())
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "function RedirectHome(){$(location).attr('href','" + this.ResolveUrl("~/Index.aspx") + "');} if (parent.fra_main) {parent.fra_main.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()')} else {parent.parent.ajaxDialogModal('RevocationAcl', 'warning', '','',null,null,'RedirectHome()');}", true);
                    return;
                }

                if (!IsPostBack)
                {
                    Page.Form.DefaultFocus = this.ClassificationBtnSave.ClientID;

                    this.InitializeLanguage();
                    this.InitializePage();
                    this.ClearSessionProperties();
                    this.SetAjaxDescriptionProject();
                    //la prima volta che entro nella pagina aggiorno con una chiamata al backend doc principale e allegati
                    SchedaDocumento doc = DocumentManager.getSelectedRecord();
                    doc.checkOutStatus = DocumentManager.GetCheckOutDocumentStatus(doc.systemId);
                    doc.documenti = DocumentManager.GetVersionsMainDocument(UserManager.GetInfoUser(), DocumentManager.getSelectedRecord().docNumber);
                    doc.allegati = DocumentManager.getAttachments(doc, "all");
                    DocumentManager.setSelectedRecord(doc);

                    this.litObject.Text = Server.HtmlEncode(doc.oggetto.descrizione); //.Replace("\n", "<br />");

                    if (DocumentManager.IsDocumentCheckedOut())
                    {
                        this.DisabilitaTutto();
                    }

                    if (doc != null && !string.IsNullOrEmpty(doc.systemId))
                    {
                        if (!string.IsNullOrEmpty(doc.accessRights) && Convert.ToInt32(doc.accessRights) <= Convert.ToInt32(HMdiritti.HDdiritti_Waiting))
                            this.DisabilitaTutto();
                    }

                    // Se il documento è bloccato, viene disabilitato il pulsante "Salva"
                    //this.ClassificationBtnSave.Enabled = !DocumentManager.IsDocumentCheckedOut();
                    //this.ClassificationBntCreaFasc.Enabled = !DocumentManager.IsDocumentCheckedOut();

                    // Inizializzazione controllo verifica ACL
                    if ((doc != null) && (doc.inCestino != "1") && (doc.systemId != null))
                    {
                        String errorMessage = String.Empty;
                        InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
                        UIManager.DocumentManager.verifyDocumentACL("D", doc.systemId, infoUtente, out errorMessage);
                        if (errorMessage != string.Empty)
                        {
                            string msg = "ErrorClassificationsACLVerify";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                        }
                    }

                    //Verifico privilegi ruolo (in questo caso se può creare un fascicolo)
                    this.impostaVisibilitaBtnFascTit();

                    if ((doc.inCestino != null && doc.inCestino == "1") || (doc.inArchivio != null && doc.inArchivio == "1"))
                    {
                        this.DisabilitaTutto();
                    }

                    this.ControlAbortDocument();

                    //Caricamento della Gridview Fascicoli
                    this.creazioneDataTableFascicoli();

                    this.VisibiltyRoleFunctions();
                }
                else
                {
                    ReadRetValueFromPopup();
                }

                RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Metodo per la gestione del ret value da popup
        /// </summary>
        protected void ReadRetValueFromPopup()
        {
            if (this.deleteDoc.Value == "true")
            {
                string[] valori = Convert.ToString(Session["DeleteValues"]).Split(';');
                deleteDocument(Convert.ToUInt16(valori[0]), valori[1], valori[2], valori[3], valori[4]);
                Session["DeleteValues"] = string.Empty;
                this.deleteDoc.Value = string.Empty;
            }

            if (!string.IsNullOrEmpty(this.Signature.ReturnValue))
            {
                this.ViewDocument.ShowDocumentAcquired(true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('Signature','')", true);
                return;
            }

            if (!string.IsNullOrEmpty(this.HSMSignature.ReturnValue))
            {
                this.ViewDocument.UpdateSignedFile();
                this.ViewDocument.ShowDocumentAcquired(false);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('HSMSignature','');", true);
            }

            if (!string.IsNullOrEmpty(this.DocumentViewer.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DocumentViewer','')", true);
            }

            if (!string.IsNullOrEmpty(this.OpenTitolario.ReturnValue))
            {
                if (this.ReturnValue.Split('#').Length > 1)
                {
                    this.TxtCodeProject.Text = this.ReturnValue.Split('#').First();
                    this.TxtDescriptionProject.Text = this.ReturnValue.Split('#').Last();
                    this.UpPnlProject.Update();
                    TxtCodeProject_OnTextChanged(new object(), new EventArgs());
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenTitolario','')", true);
            }

            if (!string.IsNullOrEmpty(this.HiddenControlPrivateClass.Value))
            {
                this.HiddenControlPrivateClass.Value = string.Empty;
                this.UpHiddenConfirm.Update();
                this.inserisciDocumentoInFascicolo();
            }

            if (!string.IsNullOrEmpty(this.HiddenPublicFolder.Value))
            {
                this.HiddenPublicFolder.Value = string.Empty;
                this.UpHiddenConfirm.Update();
                this.inserisciDocumentoInFascicolo();
            }

            if (!string.IsNullOrEmpty(this.RepositoryView.ReturnValue))
            {
                if (this.RepositoryView.ReturnValue == "selected")
                {
                    this.ViewDocument.RefreshAcquiredDocument();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('RepositoryView','');", true);
                    HttpContext.Current.Session["UploadFileAlreadyOpened" + Session.SessionID] = null;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('RepositoryView','');", true);
                    HttpContext.Current.Session["UploadFileAlreadyOpened" + Session.SessionID] = null;
                }
            }

            if ((!string.IsNullOrEmpty(this.UplodadFile.ReturnValue)) || (!string.IsNullOrEmpty(this.ActiveXScann.ReturnValue)))
            {
                if (this.UplodadFile.ReturnValue != "repository")
                {
                    this.ViewDocument.RefreshAcquiredDocument();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('UplodadFile','')", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ActiveXScann','')", true);
                    return;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "RepView", "ajaxModalPopupRepositoryView();", true);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(this.DigitalSignSelector.ReturnValue))
            {
                this.ViewDocument.UpdateSignedFile();
                this.ViewDocument.ShowDocumentAcquired(false);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DigitalSignSelector','')", true);
                HttpContext.Current.Session["CommandType"] = null;
                return;
            }

            if (!string.IsNullOrEmpty(this.DigitalCosignSelector.ReturnValue))
            {
                this.ViewDocument.UpdateSignedFile();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DigitalCosignSelector','');", true);
                HttpContext.Current.Session["CommandType"] = null;
            }

            if (!string.IsNullOrEmpty(this.VersionAdd.ReturnValue))
            {
                this.ViewDocument.ShowChangeVersions();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('VersionAdd','')", true);
                return;
            }

            if (!string.IsNullOrEmpty(this.ModifyVersion.ReturnValue))
            {
                this.ViewDocument.UpdateDescriptionVersion();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ModifyVersion','');", true);
                return;
            }

            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(PANEL_VERSION))
            {

                //popup confirm eliminazione versione
                if (this.Request.Form["__EVENTARGUMENT"] != null && this.Request.Form["__EVENTARGUMENT"].Equals(CONFIRM_REMOVE_VERSION))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('ConfirmRemoveVersion', 'HiddenRemoveVersion', '');} else {parent.ajaxConfirmModal('ConfirmRemoveVersion', 'HiddenRemoveVersion', '');}", true);
                    return;
                }
            }
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals("UpDocumentButtons"))
            {

                if (this.Request.Form["__EVENTARGUMENT"] != null && this.Request.Form["__EVENTARGUMENT"].Equals("SignatureProcessConcluted"))
                {

                    this.ViewDocument.UpdateProcessLFInAction();
                }
            }
            //rimuovi versione
            if (!string.IsNullOrEmpty((this.ViewDocument.FindControl("HiddenRemoveVersion") as HiddenField).Value))
            {
                bool res = this.ViewDocument.RemoveVersion();
                if (!res)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorRemoveVersion', 'error', '');} else {parent.ajaxDialogModal('ErrorRemoveVersion', 'error', '');}", true);
                    return;
                }
                this.ViewDocument.ShowChangeVersions();
                (this.ViewDocument.FindControl("HiddenRemoveVersion") as HiddenField).Value = string.Empty;
                return;
            }

            //Laura 13 Marzo
            if (!string.IsNullOrEmpty(this.SearchProject.ReturnValue))
            {


                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProject','');", true);

                this.TxtCodeProject.Text = string.Empty;
                this.TxtDescriptionProject.Text = string.Empty;
                this.creazioneDataTableFascicoli();
                this.UpNFascicoli.Update();
                this.UpGrid.Update();
                this.UpPnlProject.Update();
                ////}
                ////else {
                //string msg = "ResultFascicolazione";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg + "', 'info', '', '" + HttpContext.Current.Session["ReturnValuePopup"].ToString() + "');} else {parent.ajaxDialogModal('" + msg + "', 'info', '', '" + HttpContext.Current.Session["ReturnValuePopup"].ToString() + "');}", true);
                ////}

                this.DocumentTabs.RefreshLayoutTab();
                this.UpContainerDocumentTab.Update();


                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('SearchProject','');", true);
            }

            if (!string.IsNullOrEmpty(this.OpenLocalCheckOutFile.ReturnValue))
            {
                System.Web.HttpContext.Current.Session["IsAlreadyDownloaded"] = null;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('OpenLocalCheckOutFile','');", true);
            }

            if (!string.IsNullOrEmpty(this.CheckInDocument.ReturnValue))
            {
                //this.CheckOutDisabled();
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('CheckInDocument','');", true);
            }

            if (!string.IsNullOrEmpty(this.UndoCheckOut.ReturnValue))
            {
                System.Web.HttpContext.Current.Session["isCheckinOrOut"] = null;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('UndoCheckOut','');", true);

                this.ViewDocument.RefreshCheckInDocument();

                this.UpContainer.Update();
                this.UpDocumentButtons.Update();
            }

            if (!string.IsNullOrEmpty(this.StartProcessSignature.ReturnValue))
            {
                if (UIManager.DocumentManager.getSelectedAttachId() != null)
                {
                    FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId()).inLibroFirma = true;
                }
                else
                {
                    FileManager.GetFileRequest().inLibroFirma = true;
                }
                this.ViewDocument.UpdateProcessLFInAction();
                this.DocumentTabs.RefreshLayoutTab();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('StartProcessSignature','');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('SuccessStartProcessSignature', 'check', '');} else {parent.ajaxDialogModal('SuccessStartProcessSignature', 'check', '');}", true);
            }

            if (this.CheckInOutUpdatedDoc())
            {
                if (DocumentManager.getSelectedRecord() != null && !string.IsNullOrEmpty(DocumentManager.getSelectedRecord().systemId))
                {
                    SchedaDocumento doc = DocumentManager.getDocumentDetails(this.Page, DocumentManager.getSelectedRecord().systemId, DocumentManager.getSelectedRecord().docNumber);
                    DocumentManager.setSelectedRecord(doc);
                    if (DocumentManager.IsDocumentCheckedOut() || CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.getSelectedRecord().docNumber, DocumentManager.getSelectedRecord().docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord()))
                    {
                        this.DisabilitaTutto();
                    }
                    else
                    {
                        this.ClassificationBtnSave.Enabled = true;
                        this.ClassificationBntCreaFasc.Enabled = true;
                        this.TxtCodeProject.ReadOnly = false;
                        this.TxtDescriptionProject.ReadOnly = false;
                        this.RapidSender.Enabled = true;
                        this.DdlRegistries.Enabled = true;
                        this.btnclassificationschema.Enabled = true;
                        this.ClassificationSchemaSearchProject.Enabled = true;
                    }
                }

                if (HttpContext.Current.Session["isCheckOutModel"] == null)
                {
                    HttpContext.Current.Session["isCheckOutModel"] = null;
                    this.ViewDocument.RefreshCheckInDocument();

                    this.UpContainer.Update();
                }
                else
                {
                    HttpContext.Current.Session["isCheckOutModel"] = null;
                    this.ViewDocument.RefreshCheckInDocumentNoUpdateDocument();
                }

                this.UpDocumentButtons.Update();
            }

            if (!string.IsNullOrEmpty(this.DigitalVisureSelector.ReturnValue))
            {
                FileRequest approvingFile = (FileRequest)HttpContext.Current.Session["fileInAccettazione"];
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DigitalVisureSelector','');", true);
                HttpContext.Current.Session["fileInAccettazione"] = null;
                bool isAdvancementProcess = false;
                string message = string.Empty;
                if (LibroFirmaManager.PutElectronicSignature(approvingFile, isAdvancementProcess, out message))
                {
                    SchedaDocumento temp = DocumentManager.getDocumentListVersions(this.Page, approvingFile.docNumber, approvingFile.docNumber);
                    FileManager.aggiornaFileRequest(this.Page, temp.documenti.Where(e => e.versionId == approvingFile.versionId).FirstOrDefault(), false);
                    this.ViewDocument.UpdateProcessLFInAction();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorPutElectronicSignature', 'warning', '', '" + message + "');} else {parent.ajaxDialogModal('ErrorPutElectronicSignature', 'warning', '', '" + message + "');}", true);
                    return;
                }
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeFrame", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
        }

        private bool CheckInOutUpdatedDoc()
        {
            bool retVal = false;

            if (System.Web.HttpContext.Current.Session["isCheckinOrOut"] != null)
            {
                retVal = bool.Parse(System.Web.HttpContext.Current.Session["isCheckinOrOut"].ToString());
                System.Web.HttpContext.Current.Session["isCheckinOrOut"] = null;
            }

            return retVal;
        }

        private void VisibiltyRoleFunctions()
        {
            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_CLA_TITOLARIO"))
            {
                this.btnclassificationschema.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_CLA_VIS_PROC"))
            {
                this.ClassificationSchemaSearchProject.Visible = false;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("DO_CLA_INSERISCI"))
            {
                this.ClassificationBtnSave.Visible = false;
                //this.PnlProject.Visible = true;
            }

            if (!UIManager.UserManager.IsAuthorizedFunctions("FASC_NUOVO"))
            {
                this.ClassificationBntCreaFasc.Visible = false;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
            {
                this.ClassificationSchemaSearchProject.Visible = false;
            }
        }

        private void ControlAbortDocument()
        {
            this.AbortDocument = false;
            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            if (doc != null && !string.IsNullOrEmpty(doc.tipoProto) && doc.tipoProto.ToUpper().Equals("A") || doc.tipoProto.ToUpper().Equals("P") || doc.tipoProto.ToUpper().Equals("I"))
            {
                if (doc != null && doc.tipoProto != null && doc.protocollo.protocolloAnnullato != null)
                {
                    this.AbortDocument = true;
                    this.DisabilitaTutto();
                }

            }
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private DocsPaWR.Ruolo Role
        {
            get
            {
                DocsPaWR.Ruolo result = null;
                if (HttpContext.Current.Session["role"] != null)
                {
                    result = HttpContext.Current.Session["role"] as DocsPaWR.Ruolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["role"] = value;
            }
        }

        private DocsPaWR.Registro Registry
        {
            get
            {
                DocsPaWR.Registro result = null;
                if (HttpContext.Current.Session["registry"] != null)
                {
                    result = HttpContext.Current.Session["registry"] as DocsPaWR.Registro;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["registry"] = value;
            }
        }
        private DocsPaWR.Utente UserLog
        {
            get
            {
                DocsPaWR.Utente result = null;
                if (HttpContext.Current.Session["user"] != null)
                {
                    result = HttpContext.Current.Session["user"] as DocsPaWR.Utente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["user"] = value;
            }
        }
        protected void InitializePage()
        {
            Session["filtered_data"] = string.Empty;
            this.Registry = UIManager.RegistryManager.GetRegistryInSession();
            SchedaDocumento resultDoc = DocumentManager.getSelectedRecord();
            this.Role = UIManager.RoleManager.GetRoleInSession();
            this.UserLog = UIManager.UserManager.GetUserInSession();

            if (resultDoc != null && !string.IsNullOrEmpty(resultDoc.systemId))
            {
                if (resultDoc.protocollo != null)
                {
                    // Assegno il registro senza combo di selezione 
                    this.Registry = resultDoc.registro;
                    RegistryManager.SetRegistryInSession(this.Registry);
                }
                else
                {

                    // Mostro la combo di selezione
                    //this.Registry = RegistryManager.GetRegistryInSession();
                    //this.PnlRegistries.Visible = true;
                    this.PopulateDDLRegistry(this.Role);
                    //this.DdlRegistries.SelectedValue = UIManager.RegistryManager.GetRegistryInSession().systemId;

                }
            }

            //this.rblFilter.SelectedIndexChanged += new EventHandler(rblFilter_SelectedIndexChanged);

            if (resultDoc.tipoProto.Equals("A"))
            {
                this.container.Attributes.Add("class", "borderOrange");
                this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabOrangeDxBorder");
                this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabOrange");
                this.HeaderDocument.TypeRecord = "A";
            }
            else
            {
                if (resultDoc.tipoProto.Equals("P"))
                {
                    this.container.Attributes.Add("class", "borderGreen");
                    this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabGreenDxBorder");
                    this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabGreen");
                    this.HeaderDocument.TypeRecord = "P";
                }
                else
                {
                    if (resultDoc.tipoProto.Equals("I"))
                    {
                        this.container.Attributes.Add("class", "borderBlue");
                        this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabBlueDxBorder");
                        this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabBlue");
                        this.HeaderDocument.TypeRecord = "I";
                    }
                    else
                    {
                        this.container.Attributes.Add("class", "borderGrey");
                        this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabGreyDxBorder");
                        this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabGrey");
                        this.HeaderDocument.TypeRecord = "N";
                    }
                }
            }
        }


        //private void setParametriPaginaInSession() // *** DA FARE ***
        //{
        //    //memorizzo i parametri che dovranno essere
        //    //disponibili alla pagina tra le varie 
        //    //sequenze di postback
        //    try
        //    {
        //        DocumentManager.setDataGridFascicoliContenitori(this, m_dataTableFascicoli);
        //        //DocumentManager.setDataFascicoliNonVisibili(this, this.listaFascNonVisibili);
        //    }
        //    catch (System.Exception es)
        //    {
        //        ErrorManager.redirect(this, es);
        //    }
        //}

        //protected virtual void InitializeControlAclDocumento() // *** DA FARE ***
        //{
        //    AclDocumento ctl = GetControlAclDocumento();
        //    ctl.IdDocumento = DocumentManager.getSelectedRecord().systemId;            
        //    ctl.OnAclRevocata += new EventHandler(this.OnAclDocumentoRevocata);
        //}

        //protected AclDocumento GetControlAclDocumento()
        //{
        //    return (AclDocumento)this.FindControl("aclDocumento");
        //}        

        protected void rbSel_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow item = null;
                string idProject = string.Empty;
                for (int i = 0; i < GridViewClassifications.Rows.Count; i++)
                {
                    RadioButton rbSelection = GridViewClassifications.Rows[i].Cells[4].FindControl("rbSel") as RadioButton;
                    if (rbSelection.Checked)
                    {
                        item = GridViewClassifications.Rows[i];
                        idProject = ((Label)GridViewClassifications.Rows[i].Cells[10].FindControl("lblsystemId")).Text.ToString();
                    }
                }

                if (item != null)
                {
                    DocumentManager.cambiaFascPrimaria(this, idProject, DocumentManager.getSelectedRecord().systemId);

                    creazioneDataTableFascicoli();

                    // *** DA FARE ***
                    //if (m_dataTableFascicoli != null && m_dataTableFascicoli.DefaultView.Count != 0)
                    //{
                    //    if (this.Datagrid2.Items.Count == 1 && Datagrid2.CurrentPageIndex > 0)
                    //    {
                    //        Datagrid2.CurrentPageIndex = Datagrid2.CurrentPageIndex - 1;
                    //    }
                    //    this.bindDataGrid();
                    //}                
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void DisabilitaTutto()
        {
            this.ClassificationBtnSave.Enabled = false;
            this.ClassificationBntCreaFasc.Enabled = false;
            this.TxtCodeProject.ReadOnly = true;
            this.TxtDescriptionProject.ReadOnly = true;
            this.RapidSender.Enabled = false;
            this.DdlRegistries.Enabled = false;
            this.btnclassificationschema.Enabled = false;
            this.ClassificationSchemaSearchProject.Enabled = false;
        }

        private void impostaVisibilitaBtnFascTit()
        {
            if (UserManager.IsAuthorizedFunctions("FASC_NUOVO"))
            {
                this.ClassificationBntCreaFasc.Visible = true;
            }
            else
            {
                this.ClassificationBntCreaFasc.Visible = false;
            }
        }


        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.DocumentViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
            this.Signature.Title = Utils.Languages.GetLabelFromCode("TitleSignaturePopup", language);
            this.SignatureA4.Title = Utils.Languages.GetLabelFromCode("PopupSignatureA4", language);
            this.ModifyVersion.Title = Utils.Languages.GetLabelFromCode("TitleModifyVersion", language);
            this.VersionAdd.Title = Utils.Languages.GetLabelFromCode("TitleVersionAdd", language);
            this.LblObject.Text = Utils.Languages.GetLabelFromCode("ClassificationsLitObject", language);
            this.LblTitle.Text = Utils.Languages.GetLabelFromCode("ClassificationsTitleGrid", language);
            this.LblTitleClassification.Text = Utils.Languages.GetLabelFromCode("ClassificationsTitleClassification", language);
            this.ClassificationBtnSave.Text = Utils.Languages.GetLabelFromCode("ClassificationsBtnSave", language);
            this.ClassificationBntCreaFasc.Text = Utils.Languages.GetLabelFromCode("ClassificationsBtnCreaFasc", language);
            this.btnclassificationschema.AlternateText = Utils.Languages.GetLabelFromCode("ClassificationsBtnClassificationSchema", language);
            this.btnclassificationschema.ToolTip = Utils.Languages.GetLabelFromCode("ClassificationsBtnClassificationSchema", language);
            this.ClassificationsLabRegistries.Text = Utils.Languages.GetLabelFromCode("ClassificationsLabRegistries", language);
            this.OpenTitolario.Title = Utils.Languages.GetLabelFromCode("TitleClassificationScheme", language);
            this.ClassificationSchemaSearchProject.AlternateText = Utils.Languages.GetLabelFromCode("ClassificationSchemaSearchProject", language);
            this.ClassificationSchemaSearchProject.ToolTip = Utils.Languages.GetLabelFromCode("ClassificationSchemaSearchProject", language);
            this.UplodadFile.Title = Utils.Languages.GetLabelFromCode("UplodadFile", language);
            this.DigitalSignDetails.Title = Utils.Languages.GetLabelFromCode("DigitalSignDetailsTitle", language);
            this.SearchProject.Title = Utils.Languages.GetLabelFromCode("SearchProjectTitle", language);
            this.HSMSignature.Title = Utils.Languages.GetLabelFromCode("HSMSignature", language);
            this.PrintLabel.Title = Utils.Languages.GetLabelFromCode("PrintLabelPopUpTitle", language);
            this.StartProcessSignature.Title = Utils.Languages.GetLabelFromCode("StartProcessSignature", language);
            this.DetailsLFAutomaticMode.Title = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeTitle", language);
        }
        #region Gridview
        private void creazioneDataTableFascicoli()
        {
            try
            {
                string codiceRegistro = string.Empty;
                string idRegistro = string.Empty;

                SchedaDocumento documentoSelezionato = UIManager.DocumentManager.getSelectedRecord();
                if (documentoSelezionato != null)
                {

                    //RECUPERARE VALORE PER infoDocumento
                    DocsPaWR.InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(documentoSelezionato);

                    DocsPaWR.Fascicolo[] listaFascicoli;

                    if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_TAB_TRASM_ALL.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_TAB_TRASM_ALL.ToString()).Equals("1"))
                    {
                        this.ProjectNoVisibility = true;
                        //Nuovo metodo per prendere la lista dei fascicoli senza security
                        listaFascicoli = DocumentManager.GetFascicoliDaDocNoSecurity(this, infoDocumento.idProfile);
                    }
                    else
                    {
                        this.ProjectNoVisibility = false;
                        listaFascicoli = DocumentManager.GetFascicoliDaDoc(this, infoDocumento.idProfile);
                    }

                    lblNFascicoli.Text = string.Empty;

                    if (listaFascicoli != null)
                    {
                        if (listaFascicoli.Length > 0)
                        {
                            if (string.IsNullOrEmpty(documentoSelezionato.fascicolato) || documentoSelezionato.fascicolato.Equals("0"))
                            {
                                documentoSelezionato.fascicolato = "1";
                                DocumentManager.setSelectedRecord(documentoSelezionato);
                            }
                            lblNFascicoli.Text = "(" + listaFascicoli.Length.ToString() + ")";

                            // Reperimento sottocartelle contenenti il doc corrente
                            DocsPaWR.Folder[] folders = ProjectManager.GetFoldersDocument(infoDocumento.idProfile);

                            string descrizioneFasc = string.Empty;

                            for (int i = 0; i < listaFascicoli.Length; i++)
                            {

                                DocsPaWR.Fascicolo fasc = listaFascicoli[i];
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, fasc.idClassificazione, UserManager.GetInfoUser().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;

                                idRegistro = fasc.idRegistroNodoTit; // systemId del registro
                                codiceRegistro = fasc.codiceRegistroNodoTit; //codice del Registro
                                if (idRegistro != null && idRegistro == String.Empty)//se il fascicolo è associato a un TITOLARIO con idREGISTRO = NULL
                                    codiceRegistro = "<B>TUTTI</B>";




                            }

                            this.ListProject = new List<string>();
                            string language = UIManager.UserManager.GetUserLanguage();
                            var filtered_data = (from f in listaFascicoli
                                                 select new
                                                 {
                                                     Codice = GetCodice(f.codice),
                                                     Descrizione = GetDescrizione(f),
                                                     Registro = GetRegistro(f.codiceRegistroNodoTit),
                                                     Stato = f.stato,
                                                     sysReg = f.idRegistroNodoTit,
                                                     idTitolario = f.idTitolario,
                                                     systemId = f.systemID,
                                                     fascPrimaria = f.isFascPrimaria,
                                                     tipoFasc = f.tipo,
                                                     sicurezzaUtente = f.sicurezzaUtente,
                                                     Tooltip0 = GetDescriptionFoldersDocument(folders, f.systemID),
                                                     Tooltip1 = Utils.Languages.GetLabelFromCode("ClassificationsTooltipCell1", language),
                                                     Tooltip2 = Utils.Languages.GetLabelFromCode("ClassificationsTooltipCell2", language),
                                                     Tooltip3 = Utils.Languages.GetLabelFromCode("ClassificationsTooltipCell3", language),
                                                     Tooltip4 = Utils.Languages.GetLabelFromCode("ClassificationsTooltipCell4", language)
                                                 }).ToArray();


                            this.GridViewClassifications.DataSource = filtered_data;
                            Session["filtered_data"] = filtered_data;
                            this.GridViewClassifications.DataBind();

                            //Usato per eliminare il link dai fascicoli 
                            if (UserManager.IsAuthorizedFunctions("DO_DEL_DOC_FASC") && !this.AbortDocument)
                            {
                                for (int i = 0; i < this.GridViewClassifications.Rows.Count; i++)
                                {
                                    GridViewRow dgItem = GridViewClassifications.Rows[i];
                                    Label stato = (Label)dgItem.FindControl("lblstato");
                                    Label sicurezzaUtente = (Label)dgItem.FindControl("lblSicurezzaUtente");
                                    if (stato.Text == "C" || sicurezzaUtente.Text.Equals("0"))
                                    {
                                        dgItem.FindControl("ibDelete").Visible = false;
                                    }
                                    else
                                    {
                                        dgItem.FindControl("ibDelete").Visible = true;
                                    }
                                }
                            }

                            //Se tipo "G" disabilita il link al fascicolo    
                            for (int i = 0; i < this.GridViewClassifications.Rows.Count; i++)
                            {
                                GridViewRow dgItem = this.GridViewClassifications.Rows[i];
                                Label lnkSelection = dgItem.Cells[11].FindControl("lblTipoFasc") as Label;
                                LinkButton te = dgItem.FindControl("lnkCodice") as LinkButton;
                                Label idProject = dgItem.FindControl("lblsystemId") as Label;
                                if (lnkSelection.Text.Equals("G") || (this.ProjectNoVisibility && this.ListProject != null && this.ListProject.Count > 0 && this.ListProject.Contains(idProject.Text)))
                                {
                                    te.Enabled = false;
                                    te.Attributes.Remove("class");
                                    te.ToolTip = Utils.Languages.GetLabelFromCode("ClassificationsTooltipCell0NoLink", UIManager.UserManager.GetUserLanguage());
                                }
                            }

                            this.GridViewClassifications.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("ClassificationsHeaderGrid0", language);
                            this.GridViewClassifications.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("ClassificationsHeaderGrid1", language);
                            this.GridViewClassifications.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("ClassificationsHeaderGrid2", language);
                            this.GridViewClassifications.HeaderRow.Cells[4].Text = Utils.Languages.GetLabelFromCode("ClassificationsHeaderGrid4", language);
                            this.GridViewClassifications.Visible = true;
                        }
                        else
                            this.GridViewClassifications.Visible = false;
                    }
                }

                //Verifico se abilitare il radio button
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_FASC_PRIMARIA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_FASC_PRIMARIA.ToString()).Equals("1"))
                {
                    this.GridViewClassifications.Columns[4].Visible = true;

                    for (int i = 0; i < this.GridViewClassifications.Rows.Count; i++)
                    {
                        GridViewRow dgItem = this.GridViewClassifications.Rows[i];
                        RadioButton rbSelection = dgItem.Cells[4].FindControl("rbSel") as RadioButton;
                        string fascPrimaria = ((Label)dgItem.Cells[5].Controls[1]).Text.ToString();

                        //disabilita il radio per la gestione della fascicolazione primaria 
                        if (((Label)dgItem.Cells[1].Controls[1]).Text.ToString().Equals("Descrizione non visualizzabile"))
                            rbSelection.Enabled = false;

                        if (fascPrimaria == "1")
                            rbSelection.Checked = true;
                        else
                            rbSelection.Checked = false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        #endregion


        private string GetDescriptionFoldersDocument(DocsPaWR.Folder[] folders, string systemIdFascicolo)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            string retValue = string.Empty;
            bool view = false;
            foreach (DocsPaWR.Folder folder in folders)
            {
                if (!folder.idFascicolo.Equals(systemIdFascicolo))
                    continue;

                view = true;

                string valorechiave = InitConfigurationKeys.GetValue("0", "FE_PROJECT_LEVEL");
                if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1") && !string.IsNullOrEmpty(folder.codicelivello))
                {
                    string temp = "";
                    for (int i = 1; i < folder.codicelivello.Length / 4; i++)
                        temp += string.Format("{0}.", Convert.ToInt32(folder.codicelivello.Substring(i*4,4)));

                    retValue += string.Format("<li>{0} - {1}</li>",
                        temp.Substring(0, temp.Length - 1), folder.descrizione);
                }
                else
                {
                    retValue += string.Format("<li>{0}</li>", 
                        folder.descrizione);
                }
            }

            if (!retValue.Equals(string.Empty) && view)
                retValue = string.Format("<div align='left'>{0}</div><div align='left'><ul>{1}</div></ul>",
                    Languages.GetLabelFromCode("ClassificationsTooltipCell0bis", language),
                    retValue);
            else retValue = Languages.GetLabelFromCode("ClassificationsTooltipCell0", language);

            return retValue;
        }

        protected string GetCodice(string valCodice)
        {
            string result = valCodice;
            return result;
        }

        protected string GetDescrizione(Fascicolo prj)
        {
            string result = prj.descrizione;
            if (this.ProjectNoVisibility)
            {
                if (prj.sicurezzaUtente.Equals("0"))
                {
                    result = "Descrizione non visualizzabile";
                    this.ListProject.Add(prj.systemID);
                }
            }
            return result;
        }

        protected string GetRegistro(string valRegistro)
        {
            string result = valRegistro;
            return result;
        }

        protected void GridViewClassifications_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                //msgConfirm = @"Il documento verrà rimosso dal fascicolo. Continuare?";

                GridViewRow row = (GridViewRow)GridViewClassifications.Rows[e.RowIndex];
                Label stato = (Label)row.FindControl("lblstato");

                if (stato.Text == "C")
                {
                    //string msg = @"Non è possibile eliminare un documento da un fascicolo chiuso.";
                    string msg = "ErrorClassificationsDocumentNoDeleteFileClose";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);

                }
                else
                {
                    SchedaDocumento docSelezionato = UIManager.DocumentManager.getSelectedRecord();
                    if (docSelezionato != null
                        && (docSelezionato.inCestino != null && docSelezionato.inCestino == "1"))
                    {
                        //string msg = @"Impossibile eliminare un documento rimosso da un fascicolo.";                        
                        string msg = "ErrorClassificationsDocumentNoDeleteFileRemove";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);

                    }
                    else
                        if (Convert.ToInt32(docSelezionato.accessRights) <= 45)
                        {
                            //string msg = @"Impossibile eliminare un documento da un fascicolo in sola lettura.";
                            string msg = "ErrorClassificationsDocumentNoDeleteFileNoEdit";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                        }
                        else
                        {
                            string msgConfirm = "ConfirmClassificationsDeleteDocument";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'deleteDoc');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'deleteDoc');}", true);

                            Label codFasc = (Label)row.FindControl("lblcodice");
                            Label sysReg = (Label)row.FindControl("lblsysReg");
                            Label idTitolario = (Label)row.FindControl("lblidTitolario");
                            Label systemId = (Label)row.FindControl("lblsystemId");

                            int key = e.RowIndex;
                            // Metto in sessione i valori per la cancellazione nel postback
                            Session["DeleteValues"] = key + ";" + codFasc.Text + ";" + sysReg.Text + ";" + idTitolario.Text + ";" + systemId.Text;
                        }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void GridViewClassifications_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("aperturaProject"))
            {
                string index = e.CommandArgument.ToString();
                Fascicolo fascicolo = UIManager.ProjectManager.getFascicoloById(index, UIManager.UserManager.GetInfoUser());
                fascicolo.template = ProfilerProjectManager.getTemplateFascDettagli(fascicolo.systemID);

                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                DocsPaWR.Folder[] folders = ProjectManager.GetFoldersDocument(doc.docNumber);
                if (folders != null && folders.Count() > 0)
                {
                    fascicolo.folderSelezionato = (from f in folders where f.idFascicolo.Equals(fascicolo.systemID) select f).FirstOrDefault();
                }

                UIManager.ProjectManager.setProjectInSession(fascicolo);


                List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                Navigation.NavigationObject obj = new Navigation.NavigationObject();
                //navigationList.Remove(obj);
                obj.IdObject = doc.systemId;
                obj.OriginalObjectId = doc.systemId;
                obj.NumPage = string.Empty;
                obj.DxTotalPageNumber = string.Empty;
                obj.DxTotalNumberElement = string.Empty;
                obj.ViewResult = false;
                obj.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT_CLASSIFICATION.ToString(), string.Empty);
                obj.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT_CLASSIFICATION.ToString(), true, this.Page);
                obj.CodePage = Navigation.NavigationUtils.NamePage.DOCUMENT_CLASSIFICATION.ToString();
                obj.Page = "CLASSIFICATIONS.ASPX";
                obj.DxPositionElement = string.Empty;
                navigationList.Add(obj);

                Response.Redirect("~/project/project.aspx");
            }
        }

        private void deleteDocument(int key, string codFasc, string sysReg, string idTitolario, string systemId)
        {
            SchedaDocumento documentoSelezionato = UIManager.DocumentManager.getSelectedRecord();
            DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(documentoSelezionato);

            //In questa operazione adesso è necessario tener conto dei titolari multipli
            DocsPaWR.Fascicolo fascicoloSelezionato = ProjectManager.getFascicoloInClassifica(this, codFasc, sysReg, idTitolario, systemId);
            if (fascicoloSelezionato != null)
            {
                DocsPaWR.Folder folder = ProjectManager.getFolder(this, fascicoloSelezionato);
                string msg = string.Empty;

                string valoreChiaveFasc = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");

                if (string.IsNullOrEmpty(valoreChiaveFasc))
                    valoreChiaveFasc = "false";
              
                //Aggiunto controllo per classificazione obbligatoria per tipo doucmento
                bool classificationRequiredByTypeDoc = false;
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_FASC_REQUIRED_TIPI_DOC.ToString())))
                {
                    classificationRequiredByTypeDoc = Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_FASC_REQUIRED_TIPI_DOC.ToString()).Equals("1");
                }
                if (classificationRequiredByTypeDoc)
                {
                    valoreChiaveFasc = DocumentManager.IsClassificationRqueredByTypeDoc(documentoSelezionato.tipoProto).ToString();
                }

                DocsPaWR.ValidationResultInfo result = ProjectManager.deleteDocFromProject(this, folder, infoDoc.idProfile, valoreChiaveFasc, fascicoloSelezionato, out msg);
                if (result != null && result.BrokenRules.Length > 0)
                {
                    DocsPaWR.BrokenRule br = (DocsPaWR.BrokenRule)result.BrokenRules[0];

                    string msgDinamic = @"" + br.Description + "";
                    string msgDesc = "WarningDocumentCustom";
                    string errFormt = Server.UrlEncode(msgDinamic);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'error', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'error', '', '" + utils.FormatJs(errFormt) + "');}; ", true);

                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDinamic.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msgDinamic.Replace("'", @"\'") + "', 'error', '');}", true);
                    return;
                }

                if (msg == string.Empty)
                {
                    //si ricarica il datagrid senza il dato eliminato
                    this.creazioneDataTableFascicoli();
                    this.UpNFascicoli.Update();
                    this.UpGrid.Update();
                }
                else
                {
                    string msgDesc = "WarningDocumentCustom";
                    string errFormt = Server.UrlEncode(msg);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'error', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'error', '', '" + utils.FormatJs(errFormt) + "');}; ", true);
                }

            }
        }

        #region Properties
        private bool ProjectNoVisibility
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["projectNoVisibility"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["projectNoVisibility"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["projectNoVisibility"] = value;
            }
        }

        private List<string> ListProject
        {
            get
            {
                List<string> result = new List<string>();
                if (HttpContext.Current.Session["listProject"] != null)
                {
                    result = HttpContext.Current.Session["listProject"] as List<string>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listProject"] = value;
            }
        }

        #endregion
        #region Fascicolazione Rapida


        protected void TxtCodeProject_OnTextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                SchedaDocumento documentoSelezionato = UIManager.DocumentManager.getSelectedRecord();
                if (documentoSelezionato.registro == null)
                {
                    this.SearchProjectNoRegistro();
                }
                else
                {
                    this.SearchProjectRegistro();
                }

                this.cercaClassificazioneDaCodice(this.TxtCodeProject.Text);
            }
            else
            {
                this.TxtCodeProject.Text = string.Empty;
                this.TxtDescriptionProject.Text = string.Empty;
                this.IdProject.Value = string.Empty;
                this.Project = null;
                HttpContext.Current.Session["classification"] = null;
                //Laura 25 Marzo
                ProjectManager.removeFascicoloSelezionatoFascRapida();
                ProjectManager.setProjectInSessionForRicFasc(String.Empty);
            }


            this.UpPnlProject.Update();
        }

        private bool cercaClassificazioneDaCodice(string codClassificazione)
        {
            bool res = false;
            DocsPaWR.Fascicolo[] listaFasc;
            if (!string.IsNullOrEmpty(codClassificazione))
            {
                listaFasc = this.getFascicolo(this.Registry, codClassificazione);

                DocsPaWR.FascicolazioneClassificazione[] FascClass = ProjectManager.fascicolazioneGetTitolario2(this, codClassificazione, false, this.getIdTitolario(codClassificazione, listaFasc));
                if (FascClass != null && FascClass.Length != 0)
                {
                    HttpContext.Current.Session["classification"] = FascClass[0];
                }
                else
                {
                    HttpContext.Current.Session["classification"] = null;
                }
            }

            return res;
        }

        private string getIdTitolario(string codClassificazione, DocsPaWR.Fascicolo[] listaFasc)
        {
            if (listaFasc != null && listaFasc.Length > 0)
            {
                DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)listaFasc[0];
                return fasc.idTitolario;
            }
            else
            {
                return null;
            }
        }

        private DocsPaWR.Fascicolo[] getFascicolo(DocsPaWR.Registro registro, string codClassificazione)
        {
            DocsPaWR.Fascicolo[] listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codClassificazione, registro, "I");
            return listaFasc;
        }

        protected void SearchProjectNoRegistro()
        {

            this.TxtDescriptionProject.Text = string.Empty;
            //
            if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                this.TxtDescriptionProject.Text = string.Empty;
                return;
            }

            if (this.TxtCodeProject.Text.IndexOf("//") > -1)
            {

                string codice = string.Empty;
                string descrizione = string.Empty;

                DocsPaWR.Fascicolo SottoFascicolo = getFolder(null, ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {
                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        this.TxtDescriptionProject.Text = descrizione;
                        this.TxtCodeProject.Text = codice;
                        //Laura 25 Marzo
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, SottoFascicolo.idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                        // DocsPaWR.Fascicolo fascForRicFasc = ProjectManager.getFascicoloById(gerClassifica[gerClassifica.Length - 1].systemId);
                        ProjectManager.setProjectInSessionForRicFasc(gerClassifica[gerClassifica.Length - 1].codice);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);
                        this.Project = SottoFascicolo;
                    }
                    else
                    {
                        //string msg = @"Attenzione, sottofascicolo non presente.";
                        string msg = "WarningClassificationsSubFileNoFound";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        this.Project = null;
                        this.TxtDescriptionProject.Text = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;

                    }
                }
                else
                {
                    string msg = "WarningClassificationsSubFileNoFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.TxtCodeProject.Text = string.Empty;
                }
            }
            else
            {
                DocsPaWR.Fascicolo[] listaFasc = getFascicolo(this.Registry);
                string codClassifica = string.Empty;
                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            this.TxtDescriptionProject.Text = listaFasc[0].descrizione;
                            this.Project = listaFasc[0];
                            //metto il fascicolo in sessione
                            //FascicoliManager.setFascicoloSelezionato(this,fasc);
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                codClassifica = listaFasc[0].codice;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);

                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            //Laura 25 Marzo
                            this.Project = listaFasc[0];
                            ProjectManager.setProjectInSessionForRicFasc(codClassifica);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                            //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'N');");
                        }
                        else
                        {
                            //Hashtable hashRegistriNodi = getRegistriNodi(listaFasc);
                            //caso 2: al codice digitato corrispondono piu fascicoli
                            codClassifica = this.TxtCodeProject.Text;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                //codClassifica = codClassifica;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            //Page.RegisterStartupScript("openListaFasc","<SCRIPT>ApriSceltaFascicolo();</SCRIPT>");
                            //Session.Add("hasRegistriNodi",hasRegistriNodi);

                            //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'Y');");

                            //Da Fare
                            //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli2('" + codClassifica + "', 'Y')</script>");                            

                            return;
                        }
                    }
                    else
                    {
                        //caso 0: al codice digitato non corrisponde alcun fascicolo
                        if (listaFasc.Length == 0)
                        {
                            //Provo il caso in cui il fascicolo è chiuso
                            Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.TxtCodeProject.Text);
                            if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                            {
                                //string msg = @"Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato.";
                                string msg = "WarningClassificationsFileNoOpen";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                //string msg = @"Attenzione, codice fascicolo non presente.";
                                string msg = "WarningClassificationsCodFileNoFound";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                            }

                            this.TxtDescriptionProject.Text = string.Empty;
                            this.TxtCodeProject.Text = string.Empty;
                        }
                        //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                        //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', '');");
                    }
                }
            }
        }

        protected void SearchProjectRegistro()
        {
            this.TxtDescriptionProject.Text = string.Empty;
            string codClassifica = string.Empty;

            if (string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                this.TxtDescriptionProject.Text = string.Empty;
                this.Project = null;
                //Laura 25 Marzo
                ProjectManager.removeFascicoloSelezionatoFascRapida();
                ProjectManager.setProjectInSessionForRicFasc(String.Empty);
                return;
            }

            //FASCICOLAZIONE IN SOTTOFASCICOLI

            if (this.TxtCodeProject.Text.IndexOf("//") > -1)
            {
                #region FASCICOLAZIONE IN SOTTOFASCICOLI
                string codice = string.Empty;
                string descrizione = string.Empty;
                DocsPaWR.Fascicolo SottoFascicolo = getFolder(this.Registry, ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {

                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        TxtDescriptionProject.Text = descrizione;
                        TxtCodeProject.Text = codice;
                        this.Project = SottoFascicolo;
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, SottoFascicolo.idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                        ProjectManager.setProjectInSessionForRicFasc(gerClassifica[gerClassifica.Length - 1].codice);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);
                    }
                    else
                    {

                        //string msg = @"Attenzione, sottofascicolo non presente.";
                        string msg = "WarningDocumentSubFileNoFound";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        this.TxtDescriptionProject.Text = string.Empty;
                        this.TxtCodeProject.Text = string.Empty;
                        this.Project = null;
                        ProjectManager.setProjectInSessionForRicFasc(null);
                        ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                    }
                }
                else
                {
                    Session["validCodeFasc"] = "false";

                    //string msg = @"Attenzione, sottofascicolo non presente.";
                    string msg = "WarningDocumentSubFileNoFound";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.TxtCodeProject.Text = string.Empty;
                    this.Project = null;
                    ProjectManager.setProjectInSessionForRicFasc(null);
                    ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                }

                #endregion
            }
            else
            {
                DocsPaWR.Fascicolo[] listaFasc = getFascicoli(this.Registry);


                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            this.Project = listaFasc[0];
                            this.TxtDescriptionProject.Text = listaFasc[0].descrizione;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                codClassifica = listaFasc[0].codice;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            //Laura 25 Marzo
                            ProjectManager.setProjectInSessionForRicFasc(codClassifica);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                        }
                        else
                        {
                            codClassifica = this.TxtCodeProject.Text;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                //codClassifica = codClassifica;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = ProjectManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.GetUserInSession().idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }

                            ////Da Fare
                            //RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codClassifica + "', 'Y')</script>");
                            return;
                        }
                    }
                    else
                    {
                        //caso 0: al codice digitato non corrisponde alcun fascicolo
                        if (listaFasc.Length == 0)
                        {
                            //Provo il caso in cui il fascicolo è chiuso
                            Fascicolo chiusoFasc = ProjectManager.getFascicoloDaCodice(this.Page, this.TxtCodeProject.Text);
                            if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                            {
                                //string msg = @"Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato.";
                                string msg = "WarningClassificationsFileNoOpen";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                //string msg = @"Attenzione, codice fascicolo non presente.";
                                string msg = "WarningClassificationsCodFileNoFound";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            this.TxtDescriptionProject.Text = string.Empty;
                            this.TxtCodeProject.Text = string.Empty;
                            this.Project = null;
                            //Laura 25 Marzo
                            ProjectManager.setProjectInSessionForRicFasc(null);
                            ProjectManager.setFascicoloSelezionatoFascRapida(this, null);
                        }
                    }
                }
            }
        }


        private DocsPaWR.Fascicolo[] getFascicoli(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!this.TxtCodeProject.Text.Equals(""))
            {
                string codiceFascicolo = TxtCodeProject.Text;
                listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        private DocsPaWR.Fascicolo[] getFascicolo(DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
            {
                string codiceFascicolo = TxtCodeProject.Text;
                listaFasc = ProjectManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }


        private DocsPaWR.Fascicolo getFolder(DocsPaWR.Registro registro, ref string codice, ref string descrizione)
        {
            DocsPaWR.Folder[] listaFolder = null;
            DocsPaWR.Fascicolo fasc = null;
            string separatore = "//";
            int posSep = this.TxtCodeProject.Text.IndexOf("//");
            if (this.TxtCodeProject.Text != string.Empty && posSep > -1)
            {

                string codiceFascicolo = TxtCodeProject.Text.Substring(0, posSep);
                string descrFolder = TxtCodeProject.Text.Substring(posSep + separatore.Length);

                listaFolder = ProjectManager.getListaFolderDaCodiceFascicolo(this, codiceFascicolo, descrFolder, registro);
                if (listaFolder != null && listaFolder.Length > 0)
                {
                    //calcolo fascicolazionerapida
                    InfoUtente infoUtente = UserManager.GetInfoUser();
                    fasc = ProjectManager.getFascicoloById(listaFolder[0].idFascicolo, infoUtente);

                    if (fasc != null)
                    {
                        //folder selezionato è l'ultimo
                        fasc.folderSelezionato = listaFolder[listaFolder.Length - 1];
                    }
                    codice = fasc.codice + separatore;
                    descrizione = fasc.descrizione + separatore;
                    for (int i = 0; i < listaFolder.Length; i++)
                    {
                        codice += listaFolder[i].descrizione + "/";
                        descrizione += listaFolder[i].descrizione + "/";
                    }
                    codice = codice.Substring(0, codice.Length - 1);
                    descrizione = descrizione.Substring(0, descrizione.Length - 1);

                }
            }
            if (fasc != null)
            {

                return fasc;

            }
            else
            {
                return null;
            }
        }


        #endregion

        protected void ClassificationBtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Crea fascicolazione del documento
                logger.Info("BEGIN");
                // Se il documento risulta bloccato non può essere classificato
                if (DocumentManager.IsDocumentCheckedOut())
                {
                    //string msg = @"Non è possibile effettuare la classificazione in quanto il documento principale oppure almeno un suo allegato risulta bloccato.";
                    string msg = "ErrorClassificationsNoClassificDocAttachBlocked";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                }
                else
                {
                    this.ClassificaDocumento();
                    //Laura 25 Marzo
                    ProjectManager.removeFascicoloSelezionatoFascRapida();
                    ProjectManager.setProjectInSessionForRicFasc(String.Empty);
                    this.DocumentTabs.RefreshLayoutTab();
                    this.UpContainerDocumentTab.Update();
                    logger.Info("END");
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ClassificaDocumento()
        {
            logger.Info("BEGIN");
            try
            {
                //if (!this.GetControlAclDocumento().AclRevocata)
                //{
                //if ((txt_codClass.Text != "" || txt_codFasc.Text != "" || h_codFasc.Value != ""))
                if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
                {

                    //if (Session["res"] == null || Session["res"].Equals("True"))
                    //{

                    SchedaDocumento documentoSelezionato = UIManager.DocumentManager.getSelectedRecord();
                    if ((documentoSelezionato.privato == "1" || documentoSelezionato.personale == "1") && this.Project != null && this.Project.tipo != "G")
                    {
                        string msg = string.Empty;
                        if (documentoSelezionato.privato == "1")
                        {
                            msg = "WarningDocumentPrivateClassification";
                        }
                        else
                        {
                            msg = "WarningDocumentUserClassification";
                        }

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'HiddenControlPrivateClass', '');} else {parent.ajaxConfirmModal('" + msg.Replace("'", @"\'") + "', 'HiddenControlPrivateClass', '');}", true);
                        this.UpHiddenConfirm.Update();
                    }
                    if (this.Project != null && this.Project.pubblico && string.IsNullOrEmpty(this.HiddenPublicFolder.Value))
                    {
                        string msgConfirm = "WarningDocumentConfirmPublicFolder";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'HiddenPublicFolder', '');", true);
                        this.UpHiddenConfirm.Update();
                    }
                    else
                    {
                        this.inserisciDocumentoInFascicolo();
                    }
                    //}
                    //else
                    //{

                    //    if (!ClientScript.IsStartupScriptRegistered("cod_class"))
                    //    {
                    //        //string msg = @"Attenzione, codice classifica non presente.";
                    //        string msg = "WarningClassificationsCodClassNoFound";

                    //        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                    //    }
                    //    //string s = "document.getElementById('" + txt_codClass.ID + "').focus();";
                    //    //ClientScript.RegisterStartupScript(this.GetType(), "focus", s, true);
                    //    //this.txt_codFasc.Text = string.Empty;
                    //    //this.h_codFasc.Value = string.Empty;
                    //    //ddl_titolari.SelectedIndex = 0;
                    //    TxtDescriptionProject.Text = string.Empty;
                    //}
                }
                else
                {
                    string msg = "WarningClassificationsCodFileCodClassNoFound";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
                //}

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        private void inserisciDocumentoInFascicolo()
        {
            logger.Info("BEGIN");
            //DocsPaWR.Folder selectedFolder;
            //bool outValue = false;
            SchedaDocumento documentoSelezionato = UIManager.DocumentManager.getSelectedRecord();

            if (documentoSelezionato != null)
            {
                Fascicolo m_fascicoloSelezionato = this.Project;


                string codice = string.Empty;
                string descrizione = string.Empty;
                string ver_descrizione = string.Empty;
                string ver_codice = string.Empty;

                DocsPaWR.Fascicolo SottoFascicolo = getFolder(null, ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {
                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        ver_descrizione = descrizione;
                        ver_codice = codice;
                    }
                }
                else
                {
                    ver_descrizione = Project.descrizione;
                    ver_codice = Project.codice;
                }

                //Controllo se la descrizione coincide con il codice
                if (this.Project != null && !string.IsNullOrEmpty(this.TxtCodeProject.Text) && !string.IsNullOrEmpty(this.TxtDescriptionProject.Text) && this.TxtDescriptionProject.Text != ver_descrizione)
                {
                    //string msg = "La descrizione non coincide con quella del fascicolo.";
                    string msgDesc = "WarningDocumentDescrFasc";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);

                    return;
                }


                if (m_fascicoloSelezionato != null)
                {
                    //se lo stato del fascicolo è chiuso non si deve inserire il documento
                    if (m_fascicoloSelezionato.stato == "C")
                    {
                        //string msg = @"Attenzione, il fascicolo è chiuso!";
                        string msg = "WarningClassificationsFileClosed";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                        return;
                    }
                    if (m_fascicoloSelezionato.accessRights != null && Convert.ToInt32(m_fascicoloSelezionato.accessRights) <= 45)
                    {
                        //string msg = @"Attenzione, il fascicolo è in lettura";
                        string msg = "WarningClassificationsFileNoEdit";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }

                    string valoreChiaveConsentiClass = string.Empty;

                    valoreChiaveConsentiClass = Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_BLOCCA_CLASS.ToString());

                    //valoreChiaveConsentiClass = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
                    if (((m_fascicoloSelezionato.tipo.Equals("G") && m_fascicoloSelezionato.isFascConsentita != null && m_fascicoloSelezionato.isFascConsentita == "0") || (m_fascicoloSelezionato.tipo.Equals("P") && !m_fascicoloSelezionato.isFascicolazioneConsentita)) && !string.IsNullOrEmpty(valoreChiaveConsentiClass) && valoreChiaveConsentiClass.Equals("1"))
                    {
                        //string msg = @"Non è possibile inserire documenti nel fascicolo. Selezionare un nodo foglia";
                        string msg = "ErrorClassificationsNoInsertDoc";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);

                        return;
                    }

                    //String message = String.Empty;                    
                    int risultato = DocumentManager.fascicolaRapida(documentoSelezionato.systemId, documentoSelezionato.docNumber, string.Empty, m_fascicoloSelezionato, true);

                    switch (risultato)
                    {
                        case 0:
                            documentoSelezionato.fascicolato = "1";

                            break;
                        case 1:
                            //string msg = @"Attenzione, il documento è già classificato nel fascicolo indicato.";
                            string msg = "WarningClassificationsDocumentFound";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);

                            break;
                        case 2:
                            //string msg = @"Attenzione, il documento è già classificato nel fascicolo indicato.";
                            string msg2 = "WarningClassificationsDocumentFound";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg2.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg2.Replace("'", @"\'") + "', 'error', '');}", true);

                            break;
                        case 3:
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningBloccoModificheDocumentoInLf', 'warning');} else {parent.parent.ajaxDialogModal('WarningBloccoModificheDocumentoInLf', 'warning');}", true);
                            break;
                    }

                    if (risultato == 0)
                    {
                        //si ricarica il datagrid con il nuovo dato
                        TxtCodeProject.Text = string.Empty;
                        TxtDescriptionProject.Text = string.Empty;
                        creazioneDataTableFascicoli();
                        UpNFascicoli.Update();
                        UpGrid.Update();
                        UpPnlProject.Update();
                    }
                    //else
                    //{
                    //    string msgDinamic = @"" + message + "";

                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDinamic.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msgDinamic.Replace("'", @"\'") + "', 'error', '');}", true);
                    //}
                    //}
                    //dopo la classifica rimuovo la Folder Selezionata
                    ProjectManager.removeFolderSelezionato(this);
                    //if (outValue) // SE il doc è già nella folder indicata
                    //{
                    //    //string msg = @"Attenzione, il documento è già classificato nel fascicolo indicato.";
                    //    string msg = "ErrorClassificationsDocumentFound";

                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '',popupWidth=300, popupHeight=300);} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);

                    //    return;
                    //}
                    //string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1].ToString();
                }
                else
                {
                    //this.txt_codFasc.Text = this.txt_codClass.Text;
                    //this.h_codFasc.Value = this.txt_codClass.Text;

                    //// Page.RegisterStartupScript("alert", "<script>alert('Attenzione! codice fascicolo non presente!')</script>");
                    ////string s = "<SCRIPT language='javascript'>try{document.getElementById('" + txt_codFasc.ID + "').focus();} catch(e){} </SCRIPT>";
                    //string s = "try{document.getElementById('" + txt_codFasc.ID + "').focus();} catch(e){}";
                    //ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Attenzione! codice fascicolo non presente!');", true);
                    //ClientScript.RegisterStartupScript(this.GetType(), "refresh", s, true);
                    //RegisterStartupScript("focus", s);
                    //return;
                }
            }
            logger.Info("END");
        }

        public static FiltroRicerca[] addToArrayFiltroRicerca(FiltroRicerca[] array, FiltroRicerca nuovoElemento)
        {
            FiltroRicerca[] nuovaLista;
            if (array != null)
            {
                int len = array.Length;
                nuovaLista = new FiltroRicerca[len + 1];
                array.CopyTo(nuovaLista, 0);
                nuovaLista[len] = nuovoElemento;
                return nuovaLista;
            }
            else
            {
                nuovaLista = new FiltroRicerca[1];
                nuovaLista[0] = nuovoElemento;
                return nuovaLista;
            }
        }

        protected void SetAjaxDescriptionProject()
        {
            if (this.Role == null)
            {
                this.Role = RoleManager.GetRoleInSession();
            }
            string dataUser = this.Role.idGruppo;
            if (this.Registry == null)
            {
                this.Registry = RoleManager.GetRoleInSession().registri[0];
            }
            dataUser = dataUser + "-" + this.Registry.systemId;
            RapidSender.ContextKey = dataUser + "-" + this.UserLog.idAmministrazione + "-" + UIManager.ClassificationSchemeManager.getTitolarioAttivo(UIManager.UserManager.GetInfoUser().idAmministrazione).ID + "-" + UserLog.idPeople + "-" + UIManager.UserManager.GetUserInSession().systemId;
        }

        protected void ClassificationBntCreaFasc_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_CREA_FASC_CON_COD_TIT_DEL_DOC.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_CREA_FASC_CON_COD_TIT_DEL_DOC.ToString()).Equals("1"))
            {
                if (string.IsNullOrEmpty(this.TxtCodeProject.Text) && GridViewClassifications != null && GridViewClassifications.Rows != null && GridViewClassifications.Rows.Count == 1)
                {
                    GridViewRow dgItem = this.GridViewClassifications.Rows[0];
                    LinkButton te = dgItem.FindControl("lnkCodice") as LinkButton;
                    DocsPaWR.Fascicolo[] listaFasc;
                    if (!string.IsNullOrEmpty(te.Text))
                    {
                        listaFasc = this.getFascicolo(this.Registry, te.Text);

                        DocsPaWR.FascicolazioneClassificazione[] FascClass = ProjectManager.fascicolazioneGetTitolario2(this, te.Text, false, this.getIdTitolario(te.Text, listaFasc));
                        if (FascClass != null && FascClass.Length != 0)
                        {
                            HttpContext.Current.Session["classification"] = FascClass[0];
                        }
                    }
                }
            }
            Response.Redirect("../Project/Project.aspx?t=n");
        }

        private DocsPaWR.Fascicolo Project
        {
            get
            {
                Fascicolo result = null;
                if (HttpContext.Current.Session["project"] != null)
                {
                    result = HttpContext.Current.Session["project"] as Fascicolo;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["project"] = value;
            }
        }

        protected void DdlRegistries_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListItem itemSelect = (sender as DropDownList).SelectedItem;

                Registro RegSel = (from reg in this.Role.registri
                                   where reg.systemId.Equals(itemSelect.Value) &&
                                       reg.codRegistro.Equals(itemSelect.Text.Trim())
                                   select reg).FirstOrDefault();

                this.Registry = RegSel;
                UIManager.RegistryManager.SetRegistryInSession(this.Registry);
                TxtDescriptionProject.Text = string.Empty;
                TxtCodeProject.Text = string.Empty;
                this.Project = null;
                //Laura 25 Marzo
                ProjectManager.removeFascicoloSelezionatoFascRapida();
                ProjectManager.setProjectInSessionForRicFasc(String.Empty);
                TxtDescriptionProject.Focus();

                SetAjaxDescriptionProject();
                this.UpDdlRegistries.Update();
                this.UpPnlProject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void PopulateDDLRegistry(DocsPaWR.Ruolo role)
        {
            int count = 0;
            foreach (DocsPaWR.Registro reg in role.registri)
            {
                if (!reg.flag_pregresso)
                {
                    count++;
                    ListItem item = new ListItem();
                    item.Text = reg.codRegistro;
                    item.Value = reg.systemId;
                    this.DdlRegistries.Items.Add(item);
                }
            }
            if (count == 1)
            {
                // Assegno il registro senza combo di selezione
                this.Registry = UIManager.RegistryManager.GetRegistryInSession();
            }
            else
            {
                // Mostro la combo di selezione
                this.Registry = RegistryManager.GetRegistryInSession();
                this.PnlRegistries.Visible = true;
                this.DdlRegistries.SelectedValue = UIManager.RegistryManager.GetRegistryInSession().systemId;
            }

        }

        private bool AbortDocument
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["abortDocument"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["abortDocument"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["abortDocument"] = value;
            }
        }

        protected void GridViewClassifications_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                string language = UIManager.UserManager.GetUserLanguage();

                GridViewClassifications.PageIndex = e.NewPageIndex;
                this.GridViewClassifications.DataSource = Session["filtered_data"];
                this.GridViewClassifications.DataBind();
                this.GridViewClassifications.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("ClassificationsHeaderGrid0", language);
                this.GridViewClassifications.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("ClassificationsHeaderGrid1", language);
                this.GridViewClassifications.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("ClassificationsHeaderGrid2", language);
                this.GridViewClassifications.HeaderRow.Cells[4].Text = Utils.Languages.GetLabelFromCode("ClassificationsHeaderGrid4", language);
                //Usato per eliminare il link dai fascicoli 
                if (UserManager.IsAuthorizedFunctions("DO_DEL_DOC_FASC") && !this.AbortDocument)
                {
                    for (int i = 0; i < this.GridViewClassifications.Rows.Count; i++)
                    {
                        GridViewRow dgItem = GridViewClassifications.Rows[i];
                        //GridViewRow row = (GridViewRow)GridViewClassifications.Rows[e.RowIndex];
                        Label stato = (Label)dgItem.FindControl("lblstato");
                        Label sicurezzaUtente = (Label)dgItem.FindControl("lblSicurezzaUtente");
                        if (stato.Text == "C" || sicurezzaUtente.Text.Equals("0"))
                        {
                            dgItem.FindControl("ibDelete").Visible = false;
                        }
                        else
                        {
                            dgItem.FindControl("ibDelete").Visible = true;
                        }
                    }
                }
                this.UpGrid.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected string GetIdDocumento()
        {
            return (DocumentManager.getSelectedRecord() != null ? (DocumentManager.getSelectedRecord().docNumber != null ? DocumentManager.getSelectedRecord().docNumber : "0") : "0");
        }
    }
}
