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
using NttDataWA.CheckInOut;


namespace NttDataWA.Document
{
    public partial class Attachments : System.Web.UI.Page
    {
        #region const
        /// <summary>
        /// Costanti tipo documento
        /// </summary>
        private const string PROTOCOL = "P";
        private const string TYPE_PROTO_A = "A";
        private const string TYPE_PROTO_P = "P";
        private const string TYPE_PROTO_I = "I";
        private const string TYPE_GREY = "G";
        private const string SWAP = "SWAP";
        private const string CONFIRM_REMOVE_ATTACHMENTS = "CONFIRM_REMOVE_ATTACHMENTS";
        private const string PANEL_VERSION = "UpBottomButtons";
        private const string MODIFY_VERSION = "MODIFY_VERSION";
        private const string CONFIRM_REMOVE_VERSION = "CONFIRM_REMOVE_VERSION";
        private const string SIGNATURE_PROCESS_CONCLUTED = "SignatureProcessConcluted";
        private const string PANEL_BUTTONS = "panelButtons";
        private const string POPUP_DRAG_AND_DROP = "POPUP_DRAG_AND_DROP";
        private const string UP_DOCUMENT_BUTTONS = "upPnlButtons";
        /// <summary>
        /// Path delle icone visualizzate nell'imagebutton
        /// per la navigazione verso il dettaglio dell'allegato
        /// </summary>
        private const string NAVIGATE_ICON_PATH = "../Images/Documents/dett_lente.gif";
        private const string NAVIGATE_ICON_PATH_ACQUIRED = "../Images/Documents/dett_lente_doc.gif";

        private const string PANEL_ATTACHMENTS = "panelAllegati";

        /// <summary>
        /// Tipo filtro per allegato
        /// </summary>
        private const string TYPE_PEC = "pec";
        private const string TYPE_IS = "SIMPLIFIEDINTEROPERABILITY";
        private const string TYPE_USER = "user";
        private const string TYPE_EXT = "esterni";
        private const string TYPE_ALL = "all";
        private const string TYPE_DERIVATI = "derivati";

        private const int TYPE_ATTACH_USER = 1;
        public int maxLength = 2000;

        private const string CLOSE_POPUP_ADDRESS_BOOK = "closePopupAddressBook";
        //public string scriptMaxLength = "$(function () { function charsLeft() { var maxLength = <%=maxLength%>; var actLength = $('#AttachmentDescription').val().length; if (actLength>maxLength) { $('#AttachmentDescription').val($('#AttachmentDescription').val().substring(0, maxLength-1)); actLength = maxLength; } $('#AttachmentDescription_chars').text(maxLength - actLength); }  $('#AttachmentDescription').keyup(charsLeft); $('#AttachmentDescription').change(charsLeft); charsLeft(); $('#AttachmentPagesCount').keydown(function (event) {  if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||  (event.keyCode == 65 && event.ctrlKey === true) ||  (event.keyCode >= 35 && event.keyCode <= 39)) {  return; } else {  if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) { event.preventDefault(); } } }); $('#AttachmentsBtnModify').click(function () { $('#AttachmentDescription_err').remove(); if ($('#AttachmentDescription').val().length == 0) { $('#AttachmentDescription').focus(); $('#AttachmentDescription').before('<span id='AttachmentDescription_err' class='red'>Campo obbligatorio</span>'); return false; } }); $('#AttachmentDescription').focus(); });";

        #endregion

        #region properties

        private bool IsForwarded
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsForwarded"] != null) result = (bool)HttpContext.Current.Session["IsForwarded"];
                return result;

            }
            set
            {
                HttpContext.Current.Session["IsForwarded"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 0;
                if (HttpContext.Current.Session["SelectedPage"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedPage"].ToString(), out toReturn);
                if (toReturn < 0) toReturn = 0;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedPage"] = value;
            }

        }

        private int SelectedItem
        {
            get
            {
                int toReturn = -1;
                if (HttpContext.Current.Session["SelectedItem"] != null)
                    Int32.TryParse(HttpContext.Current.Session["SelectedItem"].ToString(), out toReturn);

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedItem"] = value;
            }
        }

        private string FilterTipoAttach
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterTipoAttach"] != null) toReturn = HttpContext.Current.Session["FilterTipoAttach"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterTipoAttach"] = value;
            }
        }

        private SchedaDocumento Doc
        {
            get
            {
                return DocumentManager.getSelectedRecord();
            }

            set
            {
                DocumentManager.setSelectedRecord(value);
            }
        }

        private string VersionIdAttachSelected
        {
            get
            {
                if (HttpContext.Current.Session["versionIdAttachSelected"] != null)
                    return HttpContext.Current.Session["versionIdAttachSelected"].ToString();
                else return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["versionIdAttachSelected"] = value;
            }
        }

        /// <summary>
        /// Se true viene aggiornato il visualizzatore a seguito dell'eliminazione dell'allegato
        /// </summary>
        private bool UpdateByRemoveAttachment
        {
            set
            {
                HttpContext.Current.Session["updateByRemoveAttachment"] = value;
            }
        }

        private bool EnabledFilterExternal
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["EnabledFilterExternal"]);
            }
            set
            {
                HttpContext.Current.Session["EnabledFilterExternal"] = value;
            }
        }

        public bool EnabledManagingSwapAttachments
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["EnabledManagingSwapAttachments"]);
            }
            set
            {
                HttpContext.Current.Session["EnabledManagingSwapAttachments"] = value;
            }
        }

        public bool ModifyAttachmentInGrid
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["ModifyAttachmentInGrid_X"]);
            }
            set
            {
                HttpContext.Current.Session["ModifyAttachmentInGrid_X"] = value;
            }
        }

        public bool SwapAttachmentInGrid
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["SwapAttachmentInGrid_X"]);
            }
            set
            {
                HttpContext.Current.Session["SwapAttachmentInGrid_X"] = value;
            }
        }

        public bool RicaricaGrigliaAllegati
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["RicaricaGrigliaAllegati_X"]);
            }
            set
            {
                HttpContext.Current.Session["RicaricaGrigliaAllegati_X"] = value;
            }
        }

        public bool isExternalSystemInvoice
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["invoice"]);
            }
            set
            {
                HttpContext.Current.Session["invoice"] = value;
            }
        }

        public bool disableBtnRemoveDocRep
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["DisableBtnRemoveDocRep"]);
            }
            set
            {
                HttpContext.Current.Session["DisableBtnRemoveDocRep"] = value;
            }
        }

        public bool disableBtnSwapDocRep
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["DisableBtnSwapDocRep"]);
            }
            set
            {
                HttpContext.Current.Session["DisableBtnSwapDocRep"] = value;
            }
        }

        #endregion

        #region standard method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    
                    //la prima volta che entro nella pagina se non si tratta di un nuovo documento aggiorno 
                    //con una chiamata al backend doc principale e allegati
                    SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
                    if (!DocumentManager.IsNewDocument())
                    {
                        this.AttachmentsBtnUploadAllegati.Enabled = true;
                        documentTab.checkOutStatus = DocumentManager.GetCheckOutDocumentStatus(documentTab.systemId);
                        /* ABBA 10092013
                        CheckOutStatus allegato_checkOutStatus = null;

                        if (DocumentManager.getSelectedAttachId()!=null)
                            allegato_checkOutStatus = DocumentManager.GetCheckOutDocumentStatus(DocumentManager.GetSelectedAttachment().docNumber);
                        */

                        documentTab.documenti = DocumentManager.GetVersionsMainDocument(UserManager.GetInfoUser(), DocumentManager.getSelectedRecord().docNumber);
                        documentTab.allegati = DocumentManager.getAttachments(documentTab, TYPE_ALL);
                        DocumentManager.setSelectedRecord(documentTab);

                        if (DocumentManager.IsDocumentCheckedOut() || CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.getSelectedRecord().docNumber, DocumentManager.getSelectedRecord().docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord()))
                        {
                            this.AttachmentsBtnAdd.Enabled = false;
                            this.AttachmentsBtnUploadAllegati.Enabled = false;
                        }
                    }
                    // Gabriele Melini 15-01-2014
                    // INC000000523961 
                    // Gestione ritorno a pagina allegati da dettaglio allegato
                    // back
                    if (this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1"))
                    {
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject obj = navigationList.Last();
                        if (!obj.CodePage.Equals(Navigation.NavigationUtils.NamePage.DOCUMENT_ATTACHMENT.ToString()))
                        {
                            obj = new Navigation.NavigationObject();
                            obj = navigationList.ElementAt(navigationList.Count - 2);
                        }
                        if (!string.IsNullOrEmpty(obj.DxPositionElement))
                        {
                            int index = 0;
                            if (!Int32.TryParse(obj.DxPositionElement, out index))
                                this.SelectedItem = -1;
                            else
                            {
                                this.SelectedItem = index;
                                this.SelectedPage = Convert.ToInt32(obj.DxTotalPageNumber);
                                this.rblFilter.SelectedValue = obj.Type;
                            }
                        }
                    }
                    else
                    {
                        this.SelectedItem = -1;
                    }
                    // ---------------------------------
                    this.ModifyAttachmentInGrid = false;
                    this.SwapAttachmentInGrid = false;
                    this.RicaricaGrigliaAllegati = false;
                    this.RowModifyAttach1.Visible = false;
                    this.LoadKeys();
                    this.InitializeLanguage();
                    this.InitializePage();
                    RefreshScript();
                    grdAllegati_Bind();
                    EnableButtons();
                    panelButtons.Update();
                }
                else
                {
                    this.ReadRetValueFromPopup();
                    if (this.ModifyAttachmentInGrid)
                    {
                        ButtonsModifyInGrid(false);
                        //grdAllegati.SelectedRow.Cells[2].Focus();
                        this.RowModifyAttach1.Visible = true;
                        this.RowAttachmentsStd1.Visible = false;
                        this.panelAllegati.Visible = false;
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "scriptMaxLength", scriptMaxLength, true);
                        RefreshScript();
                        this.UpContainer.Update();
                        panelButtons.Update();

                    }
                    else if (this.SwapAttachmentInGrid)
                    {
                        ButtonsModifyInGrid(true);
                        //grdAllegati.SelectedRow.Cells[2].Focus();
                        this.RowModifyAttach1.Visible = true;
                        this.RowAttachmentsStd1.Visible = false;
                        this.panelAllegati.Visible = false;
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "scriptMaxLength", scriptMaxLength, true);
                        RefreshScript();
                        this.UpContainer.Update();
                        panelButtons.Update();
                    }
                    //else if (!AttachmentsBtnAdd.Enabled && (AttachmentsBtnModify.Enabled || AttachmentsBtnSwap.Enabled))
                    else if (RicaricaGrigliaAllegati)
                    {
                        this.RowModifyAttach1.Visible = false;
                        this.RowAttachmentsStd1.Visible = true;
                        this.panelAllegati.Visible = true;
                        RicaricaGrigliaAllegati = false;
                        grdAllegati_Bind();
                        EnableButtons();
                        this.UpContainer.Update();
                        panelButtons.Update();
                    }
                    //Emanuela 15-09-2015: commentato perchè dava eccezione nei casi di firma/e crea tsd
                    //else
                    //{
                    //    EnableButtons();
                    //    panelButtons.Update();
                    //}

                }

                HttpContext.Current.Session["isAttachment"] = true;
                this.InitDragAndDropReport();
                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitDragAndDropReport()
        {
            this.UploadLiveuploads.Visible = false;
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_ENABLE_DRAG_AND_DROP.ToString())) && Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_ENABLE_DRAG_AND_DROP.ToString()).Equals("1") && DocumentManager.getSelectedRecord() != null && !String.IsNullOrEmpty(DocumentManager.getSelectedRecord().docNumber))
            {
                this.UploadLiveuploads.Visible = true;
                if (DragAndDropManager.Report != null)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "MassiveReportDragAndDrop", " ajaxModalPopupMassiveReportDragAndDrop();", true);
            }
        }
        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "DragAndDrop", "require(['" + Page.ResolveClientUrl("~/Scripts/AttachmentsDragAndDrop.js") + "']);", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        /// <summary>
        /// Metodo per la gestione del ret value da popup
        /// </summary>
        protected void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.RepositoryView.ReturnValue))
            {
                if (this.RepositoryView.ReturnValue == "selected")
                {
                    if (DocumentManager.getSelectedAttachId() != null)
                    {
                        grdAllegati_BindLight();
                    }
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
            if (!String.IsNullOrWhiteSpace(this.AttachmentsUpload.ReturnValue))
            {
                if(this.AttachmentsUpload.ReturnValue == "ok")
                {
                    Response.Redirect("Attachments.aspx", false);
                }
            }


            if ((!string.IsNullOrEmpty(this.UplodadFile.ReturnValue)) || (!string.IsNullOrEmpty(this.ActiveXScann.ReturnValue)))
            {
                if (this.UplodadFile.ReturnValue != "repository")
                {
                    if (DocumentManager.getSelectedAttachId() != null)
                    {
                        grdAllegati_BindLight();
                    }
                    this.ViewDocument.RefreshAcquiredDocument();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('UplodadFile','')", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ActiveXScann','')", true);
                    return;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('UplodadFile','');", true);
                    HttpContext.Current.Session["UploadFileAlreadyOpened" + Session.SessionID] = null;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "RepView", "ajaxModalPopupRepositoryView();", true);
                }
            }

            if (!string.IsNullOrEmpty(this.DigitalSignSelector.ReturnValue))
            {
                this.ViewDocument.UpdateSignedFile();
                this.ViewDocument.ShowDocumentAcquired(false);
                FileRequest fileReq;
                if (UIManager.DocumentManager.getSelectedAttachId() != null)
                {
                    fileReq = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
                }
                else
                {
                    fileReq = FileManager.GetFileRequest();
                }
                if (fileReq != null && fileReq.inLibroFirma)
                {
                    //LibroFirmaManager.AggiornaDataEsecuzioneElementoInLibroFirma(fileReq.docNumber);
                    SchedaDocumento temp = DocumentManager.getDocumentListVersions(this.Page, fileReq.docNumber, fileReq.docNumber);
                    FileManager.aggiornaFileRequest(this.Page, temp.documenti.Where(e => e.versionId == fileReq.versionId).FirstOrDefault(), false);
                    this.ViewDocument.UpdateProcessLFInAction();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DigitalSignSelector','');", true);
                HttpContext.Current.Session["CommandType"] = null;
                return;
            }

            if (!string.IsNullOrEmpty(this.HSMSignature.ReturnValue))
            {
                this.ViewDocument.UpdateSignedFile();
                this.ViewDocument.ShowDocumentAcquired(false);
                FileRequest fileReq;
                if (UIManager.DocumentManager.getSelectedAttachId() != null)
                {
                    fileReq = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
                }
                else
                {
                    fileReq = FileManager.GetFileRequest();
                }
                if (fileReq !=null && fileReq.inLibroFirma)
                {
                    //LibroFirmaManager.AggiornaDataEsecuzioneElementoInLibroFirma(fileReq.docNumber);
                    SchedaDocumento temp = DocumentManager.getDocumentListVersions(this.Page, fileReq.docNumber, fileReq.docNumber);
                    FileManager.aggiornaFileRequest(this.Page, temp.documenti.Where(e => e.versionId == fileReq.versionId).FirstOrDefault(), false);
                    this.ViewDocument.UpdateProcessLFInAction();
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('HSMSignature','');", true);
            }

            if (!string.IsNullOrEmpty(this.DigitalCosignSelector.ReturnValue))
            {
                this.ViewDocument.UpdateSignedFile();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DigitalCosignSelector','');", true);
                HttpContext.Current.Session["CommandType"] = null;
            }

            if (!string.IsNullOrEmpty(this.DigitalVisureSelector.ReturnValue))
            {
                FileRequest approvingFile = (FileRequest)HttpContext.Current.Session["fileInAccettazione"];

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DigitalVisureSelector','');", true);
                HttpContext.Current.Session["fileInAccettazione"] = null;
                bool isAdvancementProcess = false;
                string message = string.Empty;
                if (!LibroFirmaManager.PutElectronicSignature(approvingFile, isAdvancementProcess, out message))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorPutElectronicSignature', 'warning', '', '" + message.Replace("'", @"\'") + "');} else {parent.ajaxDialogModal('ErrorPutElectronicSignature', 'warning', '', '" + message.Replace("'", @"\'") + "');}", true);
                    return;
                }

                SchedaDocumento temp = DocumentManager.getDocumentListVersions(this.Page, approvingFile.docNumber, approvingFile.docNumber);
                FileManager.aggiornaFileRequest(this.Page, temp.documenti.Where(e => e.versionId == approvingFile.versionId).FirstOrDefault(), false);

                this.ViewDocument.UpdateProcessLFInAction();
            }

            if (!string.IsNullOrEmpty(this.Signature.ReturnValue))
            {
                this.ViewDocument.ShowDocumentAcquired(true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('Signature','')", true);
                return;
            }

            if (!string.IsNullOrEmpty(this.DocumentViewer.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DocumentViewer','')", true);
            }

            if (!string.IsNullOrEmpty(this.AttachmentsAdd.ReturnValue))
            {
                this.rblFilter.SelectedValue = "user";
                grdAllegati_Bind();
                EnableButtons();
                panelButtons.Update();
                panelAllegati.Update();
                this.DocumentTabs.RefreshLayoutTab();
                this.UpContainerDocumentTab.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AttachmentsAdd','')", true);
                return;
            }

            if (!string.IsNullOrEmpty(this.AttachmentsModify.ReturnValue))
            {
                grdAllegati_Bind();
                EnableButtons();
                panelButtons.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AttachmentsModify','')", true);
                return;
            }
            if (!string.IsNullOrEmpty(this.AttachmentsSwap.ReturnValue))
            {
                grdAllegati_Bind();
                EnableButtons();
                panelButtons.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AttachmentsSwap','')", true);
                this.SwapAttachment();
                return;
            }
            if (!string.IsNullOrEmpty(this.VersionAdd.ReturnValue))
            {
                if (DocumentManager.getSelectedAttachId() != null)
                {
                    grdAllegati_BindLight();
                }
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

            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(PANEL_ATTACHMENTS))
            {
                if (this.Request.Form["__EVENTARGUMENT"] != null && this.Request.Form["__EVENTARGUMENT"].Equals(SWAP))
                {
                    SwapAttachment();
                    return;
                }
                //popup confirm eliminazione allegato
                /*if (this.Request.Form["__EVENTARGUMENT"] != null && this.Request.Form["__EVENTARGUMENT"].Equals(CONFIRM_REMOVE_ATTACHMENTS))
                {
                    string versionLabelAttachment = FileManager.GetFileRequest(VersionIdAttachSelected).versionLabel;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('ConfirmRemoveAttachment', 'HiddenRemoveAttachment', '','" + versionLabelAttachment + "');} else {parent.ajaxConfirmModal('ConfirmRemoveAttachment', 'HiddenRemoveAttachment', '','" + versionLabelAttachment + "');}", true);
                    return;
                }*/
                grdAllegati_Bind();
                EnableButtons();
                panelButtons.Update();
                return;
            }
            //eliminazione allegato
            if (!string.IsNullOrEmpty(this.HiddenRemoveAttachment.Value))
            {
                //rimuovo l'allegato
                if (!this.RemoveAttachment())
                {
                    string versionLabelAttachment = FileManager.GetFileRequest(VersionIdAttachSelected).versionLabel;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorRemoveAttachment', 'error', '','" + versionLabelAttachment + "');} else {parent.ajaxDialogModal('ErrorRemoveAttachment', 'error', '','" + versionLabelAttachment + "');}", true);
                    return;
                }

                //aggiorniamo la griglia
                //check that after the remove there are other element in the grid
                if ((this.grdAllegati.Rows.Count - 1) > 0)
                {
                    this.grdAllegati.SelectedIndex = 0;
                    this.grid_rowindex.Value = "0";
                }
                else
                {
                    this.grdAllegati.SelectedIndex = -1;
                    this.grid_rowindex.Value = "-1";
                }

                grdAllegati_Bind();
                EnableButtons();
                panelButtons.Update();
                this.HiddenRemoveAttachment.Value = string.Empty;
                this.UpdateByRemoveAttachment = true;
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

            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(PANEL_BUTTONS))
            {

                if (this.Request.Form["__EVENTARGUMENT"] != null && this.Request.Form["__EVENTARGUMENT"].Equals(SIGNATURE_PROCESS_CONCLUTED))
                {
                    this.ViewDocument.UpdateProcessLFInAction();
                }
            }
            //rimuovi versione
            if (!string.IsNullOrEmpty((this.ViewDocument.FindControl("HiddenRemoveVersion") as HiddenField).Value))
            {
                (this.ViewDocument.FindControl("HiddenRemoveVersion") as HiddenField).Value = string.Empty;
                bool res = this.ViewDocument.RemoveVersion();
                if (!res)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorRemoveVersion', 'error', '');} else {parent.ajaxDialogModal('ErrorRemoveVersion', 'error', '');}", true);

                    return;
                }
                if (DocumentManager.getSelectedAttachId() != null)
                {
                    grdAllegati_BindLight();
                }
                this.ViewDocument.ShowChangeVersions();
                return;
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
                this.panelButtons.Update();
            }

            if (this.CheckInOutUpdatedDoc())
            {
                if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.getSelectedRecord().systemId, DocumentManager.getSelectedRecord().docNumber, UIManager.UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord()))
                {
                    this.AttachmentsBtnAdd.Enabled = false;
                    this.AttachmentsBtnModify.Enabled = false;
                    this.AttachmentsBtnRemove.Enabled = false;
                    this.AttachmentsBtnSwap.Enabled = false;
                    this.AttachmentsBtnUploadAllegati.Enabled = false;

                }
                else
                {
                    this.AttachmentsBtnAdd.Enabled = true;
                    this.AttachmentsBtnModify.Enabled = true;
                    this.AttachmentsBtnRemove.Enabled = true;
                    this.AttachmentsBtnSwap.Enabled = true;
                    this.AttachmentsBtnUploadAllegati.Enabled = true;
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

                this.panelButtons.Update();
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

            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(UP_DOCUMENT_BUTTONS))
            {
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(POPUP_DRAG_AND_DROP)))
                {
                    DragAndDropManager.ClearReport();
                    return;
                }
            }
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals(PANEL_BUTTONS))
            {
                if (this.Request.Form["__EVENTARGUMENT"] != null && (this.Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_ADDRESS_BOOK)))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('AddressBookFromPopup','');", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popupObject", "document.getElementById('ifrm_StartProcessSignature').contentWindow.closeAddressBookPopup();", true);
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

        protected void InitializePage()
        {
            SchedaDocumento documentTab = DocumentManager.getSelectedRecord();

            switch (documentTab.tipoProto)
            {
                case TYPE_PROTO_A:
                    this.container.Attributes.Add("class", "borderOrange");
                    this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabOrangeDxBorder");
                    this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabOrange");
                    this.HeaderDocument.TypeDocument = PROTOCOL;
                    this.HeaderDocument.TypeRecord = TYPE_PROTO_A;
                    break;
                case TYPE_PROTO_P:
                    this.container.Attributes.Add("class", "borderGreen");
                    this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabGreenDxBorder");
                    this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabGreen");
                    this.HeaderDocument.TypeDocument = PROTOCOL;
                    this.HeaderDocument.TypeRecord = TYPE_PROTO_P;
                    break;

                case TYPE_PROTO_I:
                    this.container.Attributes.Add("class", "borderBlue");
                    this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabBlueDxBorder");
                    this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabBlue");
                    this.HeaderDocument.TypeDocument = PROTOCOL;
                    this.HeaderDocument.TypeRecord = TYPE_PROTO_I;
                    break;
                case TYPE_GREY:
                    this.container.Attributes.Add("class", "borderGrey");
                    this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabGreyDxBorder");
                    this.HeaderDocument.TypeDocument = TYPE_GREY;
                    this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabGrey");
                    break;
            }

            this.rblFilter.SelectedIndexChanged += new EventHandler(rblFilter_SelectedIndexChanged);
            //arrivo dalla pagina profilo a seguito del click su un ragruppamento allegati non utente
            if (Page.Request["attNoUser"] != null)
                this.rblFilter.SelectedValue = Page.Request["attNoUser"];

            litObject.Text = Server.HtmlEncode(Doc.oggetto.descrizione);
            // Gabriele Melini 15-01-2014
            // INC000000523961 
            // Gestione ritorno a pagina allegati da dettaglio allegato
            if (!(this.Request.QueryString["back"] != null && this.Request.QueryString["back"].Equals("1")))
                this.SelectedPage = 0;
            // enable controls
            if (!EnabledFilterExternal)
            {
                ListItem itemExternal = (from item in rblFilter.Items.Cast<ListItem>() where item.Value.Equals(TYPE_EXT) select item).FirstOrDefault();
                rblFilter.Items.Remove(itemExternal);
            }

            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.INTEROP_SERVICE_ACTIVE.ToString())) ||
               !Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.INTEROP_SERVICE_ACTIVE.ToString()).Equals("1"))
            {
                ListItem itemPitre = (from item in rblFilter.Items.Cast<ListItem>() where item.Value.Equals(TYPE_IS) select item).FirstOrDefault();
                rblFilter.Items.Remove(itemPitre);
            }

            string labelDerivati = System.Configuration.ConfigurationManager.AppSettings["LABEL_ALLEGATI_DERIVATI"];
            if (!string.IsNullOrEmpty(labelDerivati))
            {
                labelDerivati = Utils.Languages.GetLabelFromCode("lblAttachmentDerivati", UserManager.GetUserLanguage());
                this.rblFilter.Items.Add(
                    new ListItem(
                                  labelDerivati,
                                  TYPE_DERIVATI));
            }

            this.HeaderDocument.RefreshDataDocument();
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.Signature.Title = Utils.Languages.GetLabelFromCode("TitleSignaturePopup", language);
            this.SignatureA4.Title = Utils.Languages.GetLabelFromCode("PopupSignatureA4", language);
            this.DocumentViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
            this.ModifyVersion.Title = Utils.Languages.GetLabelFromCode("TitleModifyVersion", language);
            this.VersionAdd.Title = Utils.Languages.GetLabelFromCode("TitleVersionAdd", language);
            this.AttachmentsObject.InnerText = Utils.Languages.GetLabelFromCode("AttachmentsObject", language);
            this.lblFilter.Text = Utils.Languages.GetLabelFromCode("AttachmentsLblFilter", language);
            this.AttachmentsAll.Text = Utils.Languages.GetLabelFromCode("AttachmentsAll", language);
            this.AttachmentsUser.Text = Utils.Languages.GetLabelFromCode("AttachmentsUser", language);
            this.AttachmentsPec.Text = Utils.Languages.GetLabelFromCode("AttachmentsPec", language);
            this.AttachmentsPitre.Text = SimplifiedInteroperabilityManager.SearchItemDescriprion;
            this.AttachmentsExternalSystem.Text = (this.isExternalSystemInvoice ? Utils.Languages.GetLabelFromCode("AttachmentsExternalSystemInvoice", language) : Utils.Languages.GetLabelFromCode("AttachmentsExternalSystem", language));
            this.AttachmentsBtnAdd.Text = Utils.Languages.GetLabelFromCode("AttachmentsBtnAdd", language);
            this.AttachmentsBtnModify.Text = Utils.Languages.GetLabelFromCode("AttachmentsBtnModify", language);
            this.AttachmentsBtnSwap.Text = Utils.Languages.GetLabelFromCode("AttachmentsBtnSwap", language);
            this.AttachmentsBtnRemove.Text = Utils.Languages.GetLabelFromCode("AttachmentsBtnRemove", language);
            this.AttachmentsBtnAnnulla.Text = Utils.Languages.GetLabelFromCode("GenericBtnCancel", language);
            this.UplodadFile.Title = Utils.Languages.GetLabelFromCode("UplodadFile", language);
            this.DigitalSignDetails.Title = Utils.Languages.GetLabelFromCode("DigitalSignDetailsTitle", language);
            this.HSMSignature.Title = Utils.Languages.GetLabelFromCode("HSMSignature", language);
            this.StartProcessSignature.Title = Utils.Languages.GetLabelFromCode("StartProcessSignature", language);
            this.DetailsLFAutomaticMode.Title = Utils.Languages.GetLabelFromCode("DetailsLFAutomaticModeTitle", language);
            this.PrintLabel.Title = Utils.Languages.GetLabelFromCode("PrintLabelPopUpTitle", language);
            this.AttachmentLblModify.Text = Utils.Languages.GetLabelFromCode("AttachmentsLblModify", language);
            this.AttachmentLblModifyDescription.Text = Utils.Languages.GetLabelFromCode("AttachmentsLblModifyDescr", language);
            this.AttachmentsLblModifyPagesNumber.Text = Utils.Languages.GetLabelFromCode("AttachmentsLblPages", language);
            this.AttachmentsAdd.Title = Utils.Languages.GetLabelFromCode("AttachmentsTitle", language);
            this.AddressBookFromPopup.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);

        }

        protected void LoadKeys()
        {
            InfoUtente userInfo = UserManager.GetInfoUser();
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_FILTRO_ALLEGATI_ESTERNI.ToString())) &&
                Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_FILTRO_ALLEGATI_ESTERNI.ToString()).Equals("1"))
            {
                this.EnabledFilterExternal = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_GESTIONE_SCAMBIA_ALLEGATI.ToString())) &&
                Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_GESTIONE_SCAMBIA_ALLEGATI.ToString()).Equals("1"))
            {
                this.EnabledManagingSwapAttachments = true;
                //this.AttachmentsBtnSwap.OnClientClick = "return ajaxModalPopupAttachmentsSwap();";
                //this.AttachmentsBtnSwap.Click += new EventHandler(AttachmentsBtnSwap_Click);
            }
            else
            {
                this.EnabledManagingSwapAttachments = false;
                //this.AttachmentsBtnSwap.OnClientClick = "__doPostBack('panelAllegati', 'SWAP'); return false;";
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_INVOICE.ToString())) &&
                Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_INVOICE.ToString()).Equals("1"))
            {
                this.isExternalSystemInvoice = true;
            }
            else
            {
                this.isExternalSystemInvoice = false;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_DISABLE_BTN_REMOVE_DOC_REP.ToString())) &&
                Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_DISABLE_BTN_REMOVE_DOC_REP.ToString()).Equals("1"))
            {
                this.disableBtnRemoveDocRep = true;
            }
            else
            {
                this.disableBtnRemoveDocRep = false;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_DISABLE_BTN_SWAP_DOC_REP.ToString())) &&
                Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_DISABLE_BTN_SWAP_DOC_REP.ToString()).Equals("1"))
            {
                this.disableBtnSwapDocRep = true;
            }
            else
            {
                this.disableBtnSwapDocRep = false;
            }
        }

        #endregion

        #region Grid Attachments

        /// <summary>
        /// Verifica se si tratta di una nuova ricerca
        /// </summary>
        /// <returns></returns>
        protected bool checkIfNewSearch()
        {
            bool res = this.FilterTipoAttach != this.rblFilter.SelectedValue ? true : (!this.Page.IsPostBack) ? true : false;
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        private void grdAllegati_Bind()
        {
            Allegato[] at = null;
            //if you set the filter to all user or should I consider the attached user
            switch (this.rblFilter.SelectedValue)
            {
                case TYPE_USER:
                    at = (from att in Doc.allegati where att.TypeAttachment == 1 select att).ToArray();
                    break;
                case TYPE_PEC:
                    at = (from att in Doc.allegati where att.TypeAttachment == 2 select att).ToArray();
                    break;
                case TYPE_IS:
                    at = (from att in Doc.allegati where att.TypeAttachment == 3 select att).ToArray();
                    break;
                case TYPE_EXT:
                    at = (from att in Doc.allegati where att.TypeAttachment == 4 select att).ToArray();
                    break;
                case TYPE_ALL:
                    if (this.EnabledFilterExternal)
                    {
                        at = Doc.allegati;
                    }
                    else
                    {
                        at = (from att in Doc.allegati where att.TypeAttachment != 4 select att).ToArray();
                    }
                    break;
                case TYPE_DERIVATI:
                    at = (from att in Doc.allegati where att.TypeAttachment == 5 select att).ToArray();
                    break;
            }

            if (this.IsForwarded)
            {
                at = Doc.allegati;
            }

            this.grdAllegati.DataSource = at;
            bool isNewSearch = checkIfNewSearch();
            if (isNewSearch)
            {
                // Gabriele Melini 15-01-2014
                // INC000000523961 
                // Gestione ritorno a pagina allegati da dettaglio allegato
                if (this.SelectedItem >= 0)
                {
                    int index = (this.SelectedItem - 1) - this.SelectedPage * this.grdAllegati.PageSize;
                    if (index > this.grdAllegati.PageSize)
                        this.grdAllegati.SelectedIndex = (this.grdAllegati.DataSource as Allegato[]) != null && (this.grdAllegati.DataSource as Allegato[]).Length > 0 ? 0 : -1;
                    else
                        this.grdAllegati.SelectedIndex = index;

                    this.SelectedItem = -1;
                }
                else
                    this.grdAllegati.SelectedIndex = (this.grdAllegati.DataSource as Allegato[]) != null && (this.grdAllegati.DataSource as Allegato[]).Length > 0 ? 0 : -1;
                this.grid_rowindex.Value = this.grdAllegati.SelectedIndex.ToString();
                this.FilterTipoAttach = this.rblFilter.SelectedValue;
            }
            else
            {
                if (string.IsNullOrEmpty(this.grid_rowindex.Value) && (this.grdAllegati.DataSource as Allegato[]).Length > 0)
                    this.grid_rowindex.Value = "0";
                else if (string.IsNullOrEmpty(this.grid_rowindex.Value) && (this.grdAllegati.DataSource as Allegato[]).Length == 0)
                    this.grid_rowindex.Value = "-1";
                else
                    //quando viene aggiunto un nuovo allegato, seleziona quest'ultimo
                    if (this.grid_rowindex.Value.Equals("-2") && (this.grdAllegati.DataSource as Allegato[]).Length > 0)
                        this.grid_rowindex.Value = (((this.grdAllegati.DataSource as Allegato[]).Length - 1) % 10).ToString();
                if (!this.grid_rowindex.Value.Equals(this.grdAllegati.SelectedIndex.ToString()))
                    System.Web.HttpContext.Current.Session.Remove("selectedNumberVersion");
                if (int.Parse(this.grid_rowindex.Value) >= -1)
                    this.grdAllegati.SelectedIndex = int.Parse(this.grid_rowindex.Value);
            }
            this.grdAllegati.PageIndex = this.SelectedPage;
            this.grdAllegati.DataBind();
            this.ModifyAttachmentInGrid = false;
            this.SwapAttachmentInGrid = false;
            HighlightSelectedRow();

            if (this.grdAllegati.SelectedIndex >= 0 && (this.grdAllegati.DataSource as Allegato[]) != null && (this.grdAllegati.DataSource as Allegato[]).Length > 0)
            {
                DocumentManager.setSelectedAttachId((this.grdAllegati.DataSource as Allegato[])[this.grdAllegati.SelectedIndex + this.grdAllegati.PageIndex * this.grdAllegati.PageSize].versionId);
                VersionIdAttachSelected = DocumentManager.getSelectedAttachId();
            }
        }

        /// <summary>
        /// Versione light del bind della griglia allegati.
        /// Utilizzata per aggiornare l'immagine della url di redirect al profilo
        /// </summary>
        private void grdAllegati_BindLight()
        {
            try
            {
                Allegato[] at = null;

                //if you set the filter to all user or should I consider the attached user
                switch (this.rblFilter.SelectedValue)
                {
                    case TYPE_USER:
                        at = (from att in Doc.allegati where att.TypeAttachment == 1 select att).ToArray();
                        break;
                    case TYPE_PEC:
                        at = (from att in Doc.allegati where att.TypeAttachment == 2 select att).ToArray();
                        break;
                    case TYPE_IS:
                        at = (from att in Doc.allegati where att.TypeAttachment == 3 select att).ToArray();
                        break;
                    case TYPE_EXT:
                        at = (from att in Doc.allegati where att.TypeAttachment == 4 select att).ToArray();
                        break;
                    case TYPE_ALL:
                        at = Doc.allegati;
                        break;
                    case TYPE_DERIVATI:
                        at = (from att in Doc.allegati where att.TypeAttachment == 4 select att).ToArray();
                        break;
                }
                this.grdAllegati.DataSource = at;
                this.grdAllegati.PageIndex = this.SelectedPage;
                this.grdAllegati.SelectedIndex = Convert.ToInt32(this.grid_rowindex.Value);
                this.grdAllegati.DataBind();
                HighlightSelectedRow();
                panelAllegati.Update();
            }
            catch (Exception e)
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void HighlightSelectedRow()
        {

            if (this.grdAllegati.Rows.Count > 0 && this.grdAllegati.SelectedRow != null)
            {
                GridViewRow gvRow = this.grdAllegati.SelectedRow;
                foreach (GridViewRow GVR in this.grdAllegati.Rows)
                {
                    if (GVR == gvRow)
                    {
                        GVR.CssClass += " selectedrow";
                        //DocumentManager.setSelectedAttachId(((HiddenField)GVR.Cells[4].FindControl("attachId")).Value);
                        //VersionIdAttachSelected = DocumentManager.getSelectedAttachId();
                    }
                    else
                    {
                        GVR.CssClass = GVR.CssClass.Replace(" selectedrow", "");
                    }
                }
            }
        }

        /// <summary>
        /// Reperimento path dell'icona per la navigazione verso l'allegato
        /// </summary>
        /// <param name="allegato"></param>
        protected string ImageNavigateDocument(DocsPaWR.Allegato allegato)
        {
            int result;
            string path = string.Empty;
            if (allegato != null && allegato.fileSize != null && Int32.TryParse(allegato.fileSize, out result))
            {
                if (result > 0)
                    path = NAVIGATE_ICON_PATH_ACQUIRED;
                else
                    path = NAVIGATE_ICON_PATH;
            }
            else
                path = NAVIGATE_ICON_PATH;

            return path;
        }

        private bool RemoveAttachment()
        {
            DocsPaWR.Allegato allegato = FileManager.GetFileRequest(VersionIdAttachSelected) as Allegato;
            return DocumentManager.RemoveAttachment(SelectedPage, grdAllegati.PageSize, allegato);

        }
        #endregion

        #region Buttons
        /// <summary>
        /// Disabilitazione per consolidazione
        /// </summary>
        private void DisableButtonsConsolidation()
        {
            AttachmentsBtnAdd.Enabled = false;
            AttachmentsBtnModify.Enabled = false;
            AttachmentsBtnSwap.Enabled = false;
            AttachmentsBtnRemove.Enabled = false;
            this.AttachmentsBtnUploadAllegati.Enabled = false;
        }

        /// <summary>
        /// Disabilitazione di tutti i pulsanti
        /// </summary>
        private void DisableAllButtons()
        {
            AttachmentsBtnAdd.Enabled = false;
            AttachmentsBtnModify.Enabled = false;
            AttachmentsBtnRemove.Enabled = false;
            AttachmentsBtnSwap.Enabled = false;
            this.AttachmentsBtnUploadAllegati.Enabled = false;
            AttachmentsBtnAnnulla.Visible = false;
        }

        private void ButtonsModifyInGrid(bool swap)
        {
            AttachmentsBtnAdd.Enabled = false;
            AttachmentsBtnModify.Enabled = !swap;
            AttachmentsBtnRemove.Enabled = false;
            AttachmentsBtnSwap.Enabled = swap;
            AttachmentsBtnAnnulla.Visible = true;
            this.AttachmentsBtnUploadAllegati.Enabled = false;
        }

        /// <summary>
        /// Gestione abilitazione / disabilitazione pulsanti
        /// </summary>
        protected virtual void EnableButtons()
        {
            this.DisableAllButtons();
            bool checkOut = false;

            //se il documento non si trova nello stato IN CESTINO, ANNULLATO e IN ARCHIVIO
            if (!DocumentManager.IsDocumentInBasket() && !DocumentManager.IsDocumentAnnul() && !DocumentManager.IsDocumentInArchive())
            {
                AttachmentsBtnAdd.Enabled = true;
                this.AttachmentsBtnUploadAllegati.Enabled = true;
                if (DocumentManager.IsDocumentCheckedOut() || CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.getSelectedRecord().docNumber, DocumentManager.getSelectedRecord().docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord()))
                {
                    checkOut = true;
                    AttachmentsBtnAdd.Enabled = !checkOut;
                    this.AttachmentsBtnUploadAllegati.Enabled = !checkOut;
                }
                //se è presente almeno un allegato
                if (!string.IsNullOrEmpty(DocumentManager.getSelectedAttachId()) && this.grdAllegati.Rows.Count > 0 && this.grdAllegati.SelectedRow != null)
                {
                    Allegato FRAllegato = FileManager.GetFileRequest(VersionIdAttachSelected) as Allegato;
                    bool isTypeAttachUser = FRAllegato.TypeAttachment == TYPE_ATTACH_USER;
                    bool isTypeAttachExt = FRAllegato.TypeAttachment == 4;
                    if (isTypeAttachUser)
                        AttachmentsBtnModify.Enabled = !checkOut;

                    //rimuovi disabilitato nel caso di protocollo o documento repertoriato
                    AttachmentsBtnRemove.Enabled = !((Doc.protocollo != null && !string.IsNullOrEmpty(Doc.protocollo.segnatura)) || checkOut);

                    if (AttachmentsBtnRemove.Enabled)
                    {
                        if (isTypeAttachUser || isTypeAttachExt)
                            AttachmentsBtnSwap.Enabled = !checkOut;

                        //AttachmentsBtnRemove.Enabled = !DocumentManager.IsDocumentoRepertoriato(Doc.template); 
                    }

                    // INC000000574346
                    // in caso di documenti repertoriati l'abilitazione dei tasti rimuovi e scambia è governata rispettivamente dalle chiavi
                    // FE_DISABLE_BTN_REMOVE_DOC_REP e FE_DISABLE_BTN_SWAP_DOC_REP
                    bool isRepertoriato = DocumentManager.IsDocumentoRepertoriato(Doc.template);
                    if (isRepertoriato)
                    {
                        if(AttachmentsBtnRemove.Enabled)
                            AttachmentsBtnRemove.Enabled = !this.disableBtnRemoveDocRep;
                        AttachmentsBtnSwap.Enabled = !this.disableBtnSwapDocRep;
                    }

                    //Se l'allegato o il documento principale è in libro firma disabilito il bottone scambia e rimuovi allegato
                    if (Doc.documenti[0].inLibroFirma || FRAllegato.inLibroFirma)
                    {
                        AttachmentsBtnSwap.Enabled = false;
                        if (FRAllegato.inLibroFirma)
                            AttachmentsBtnRemove.Enabled = false;
                    }
                    /*
                    if (DocumentManager.IsFatturaElettronica() && isRepertoriato)
                    {
                        AttachmentsBtnSwap.Enabled = false;
                        AttachmentsBtnRemove.Enabled = false;
                    }
                     * */
                    //INC000000821588 : Nel caso di nuovo documento(documento inoltrato) disabilito il tasto scambia
                    if (DocumentManager.IsNewDocument())
                        AttachmentsBtnSwap.Enabled = false;
                }
            }

            // controllo su doc in conversione pdf
            if (!string.IsNullOrEmpty(DocumentManager.getSelectedAttachId()) && (DocumentManager.IsDocumentCheckedOut() && CheckInOut.CheckInOutServices.IsCheckedOutConversionePdf()))
            {
                this.AttachmentsBtnModify.Enabled = false;
            }

            //disabilito tutti i pulsanti se l'utente non ha i diritti di scrittura sul documento e non è un predisposto alla proto
            if (!UserManager.IsRightsWritingInherits() && !Doc.predisponiProtocollazione && !(DocumentManager.IsNewDocument() && Doc.repositoryContext != null))
            {
                this.DisableAllButtons();
            }

            //se attivo il filtro AOO  e il doc non è un predisposto alla proto
            if (UserManager.isFiltroAooEnabled() && !Doc.predisponiProtocollazione)
            {
                if (Doc != null && Doc.protocollo != null)
                {
                    DocsPaWR.Registro[] userRegistri = UserManager.GetRegistersList();
                    if (AttachmentsBtnAdd.Enabled) AttachmentsBtnAdd.Enabled = UserManager.verifyRegNoAOO(Doc, userRegistri);
                    if (AttachmentsBtnUploadAllegati.Enabled) AttachmentsBtnUploadAllegati.Enabled = UserManager.verifyRegNoAOO(Doc, userRegistri);
                    if (AttachmentsBtnModify.Enabled) AttachmentsBtnModify.Enabled = UserManager.verifyRegNoAOO(Doc, userRegistri);
                    if (AttachmentsBtnRemove.Enabled) AttachmentsBtnRemove.Enabled = UserManager.verifyRegNoAOO(Doc, userRegistri);
                    if (AttachmentsBtnSwap.Enabled) AttachmentsBtnSwap.Enabled = UserManager.verifyRegNoAOO(Doc, userRegistri);
                }
            }

            // Controllo su stato documento consolidato
            if (DocumentManager.IsDocumentConsolidate())
            {
                this.DisableButtonsConsolidation();
            }
            //se il documento è in stato finale ed al ruolo corrente non è stato attivato lo sblocco
            //sullo stato finale, allora disabilito tutti i pulsanti del tab allegati;
            if (DiagrammiManager.IsDocumentInFinalState() &&
                Convert.ToInt32(DocumentManager.getAccessRightDocBySystemID(DocumentManager.getSelectedRecord().docNumber,
                UserManager.GetInfoUser())) == 45)
            {
                this.DisableAllButtons();
            }
            //controllo funzioni attive sul ruolo
            VisibilityRoleFunction();

            panelButtons.Update();
        }

        /// <summary>
        /// Verifica micro funzioni attive
        /// </summary>
        private void VisibilityRoleFunction()
        {
            if (UserManager.IsAuthorizedFunctions("DO_ALL_AGGIUNGI"))
            {
                AttachmentsBtnAdd.Visible = true;
                this.AttachmentsBtnUploadAllegati.Visible = true;
            }
            else
            {
                AttachmentsBtnAdd.Visible = false;
                this.AttachmentsBtnUploadAllegati.Visible = false;
            }
            if (UserManager.IsAuthorizedFunctions("DO_ALL_MODIFICA"))
            {
                AttachmentsBtnModify.Visible = true;
            }
            else
            {
                AttachmentsBtnModify.Visible = false;
            }
            if (UserManager.IsAuthorizedFunctions("DO_ALL_RIMUOVI"))
            {
                AttachmentsBtnRemove.Visible = true;
            }
            else
            {
                AttachmentsBtnRemove.Visible = false;
            }
            if (UserManager.IsAuthorizedFunctions("DO_ALL_SOSTITUISCI"))
            {
                AttachmentsBtnSwap.Visible = true;
            }
            else
            {
                AttachmentsBtnSwap.Visible = false;
            }
        }
        #endregion

        #region Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SwapAttachment()
        {
            if (!DocumentManager.CheckRevocationAcl() && !DocumentManager.IsDocumentCheckedOut())
            {
                //DocsPaWR.Allegato attach = DocumentManager.GetSelectedAttachment();
                DocsPaWR.Allegato attach = (FileManager.GetFileRequest(VersionIdAttachSelected) as Allegato);

                if (attach != null)
                {
                    bool result = DocumentManager.swapAttachment(this, Doc.documenti[0], attach);
                    if (result)
                    {
                        SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
                        documentTab.allegati = DocumentManager.getAttachments(DocumentManager.getSelectedRecord(), TYPE_ALL, "");
                        documentTab.documenti = DocumentManager.GetVersionsMainDocument(UserManager.GetInfoUser(), documentTab.docNumber);
                        DocumentManager.setSelectedRecord(documentTab);
                        FileManager.setSelectedFile(documentTab.documenti[0]);
                    }
                    DocumentManager.setSelectedAttachId(VersionIdAttachSelected);
                    this.EnableButtons();
                    if (!result)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorSwapAttachment', 'error');} else {parent.parent.ajaxDialogModal('ErrorSwapAttachment', 'error');}", true);

                        return;
                    }
                }
            }

            if (DocumentManager.IsDocumentCheckedOut())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningSwapAttachment', 'warning');} else {parent.parent.ajaxDialogModal('WarningSwapAttachment', 'warning');}", true);

            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('CompleteSwapAttachment', 'check');} else {parent.parent.ajaxDialogModal('CompleteSwapAttachment', 'check');}", true);

            }
        }

        protected void rblFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.SelectedPage = 0;
                this.grdAllegati_Bind();
                this.panelAllegati.Update();
                EnableButtons();
                panelButtons.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdAllegati_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                ModifyAttachmentInGrid = false;
                SwapAttachmentInGrid = false;
                this.SelectedPage = e.NewPageIndex;
                this.SelectedItem = -1;
                this.grid_rowindex.Value = "0";
                grdAllegati_Bind();
                this.ViewDocument.ShowDocumentAcquired(false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdAllegati_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    for (int i = 0; i < (e.Row.Cells.Count - 1); i++)
                    {

                        e.Row.Cells[i].Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "');__doPostBack('panelAllegati');return false;";
                    }
                    e.Row.Cells[e.Row.Cells.Count - 1].Attributes["onClick"] = "$('#grid_rowindex').val('" + e.Row.RowIndex.ToString() + "');";
                    if (DocumentManager.IsNewDocument()) //nel caso di nuovo documento disabilito il redirect al dettaglio allegato
                    {
                        ((CustomImageButton)e.Row.FindControl("btnNavigateDocument")).Visible = false;
                        return;
                    }
                    ((CustomImageButton)e.Row.FindControl("btnNavigateDocument")).CssClass = "attach" + ((DocsPaWR.Allegato)e.Row.DataItem).docNumber;
                    ((CustomImageButton)e.Row.FindControl("btnNavigateDocument")).ImageUrl = this.ImageNavigateDocument((DocsPaWR.Allegato)e.Row.DataItem);
                    ((CustomImageButton)e.Row.FindControl("btnNavigateDocument")).OnMouseOverImage = this.ImageNavigateDocument((DocsPaWR.Allegato)e.Row.DataItem);
                    ((CustomImageButton)e.Row.FindControl("btnNavigateDocument")).OnMouseOutImage = this.ImageNavigateDocument((DocsPaWR.Allegato)e.Row.DataItem);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdAllegati_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                //Redirect al grigio contenente l'allegato selezionato
                case "NavigateDocument":
                    string versionId = (((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).FindControl("attachId") as HiddenField).Value;
                    if (!string.IsNullOrEmpty(versionId))
                    {
                        ModifyAttachmentInGrid = false;
                        SwapAttachmentInGrid = false;
                        SchedaDocumento newDoc = DocumentManager.getSelectedRecord();
                        List<Navigation.NavigationObject> navigationList = Navigation.NavigationUtils.GetNavigationList();
                        Navigation.NavigationObject actualPage = new Navigation.NavigationObject();
                        actualPage.IdObject = newDoc.systemId;
                        actualPage.NamePage = Navigation.NavigationUtils.GetNamePage(Navigation.NavigationUtils.NamePage.DOCUMENT_ATTACHMENT.ToString(), string.Empty);
                        actualPage.Link = Navigation.NavigationUtils.GetLink(Navigation.NavigationUtils.NamePage.DOCUMENT_ATTACHMENT.ToString(), true, this.Page);
                        actualPage.CodePage = Navigation.NavigationUtils.NamePage.DOCUMENT_ATTACHMENT.ToString();
                        actualPage.Page = "ATTACHMENTS.ASPX";
                        // Gabriele Melini 15-01-2014
                        // INC000000523961 
                        // Gestione ritorno a pagina allegati da dettaglio allegato
                        int rowIndex = ((e.CommandSource as CustomImageButton).Parent.Parent as GridViewRow).RowIndex;
                        int selectedIndex = (rowIndex + 1) + this.grdAllegati.PageSize * this.grdAllegati.PageIndex;
                        actualPage.DxTotalPageNumber = this.grdAllegati.PageIndex.ToString();
                        actualPage.DxPositionElement = selectedIndex.ToString();
                        actualPage.Type = this.rblFilter.SelectedValue;
                        // -------------------------------

                        navigationList.Add(actualPage);
                        Navigation.NavigationUtils.SetNavigationList(navigationList);


                        Allegato attachment = FileManager.GetFileRequest(versionId) as Allegato;
                        SchedaDocumento documentTab = DocumentManager.getDocumentDetails(this.Page, attachment.docNumber, attachment.docNumber);
                        DocumentManager.setSelectedRecord(documentTab);
                        Response.Redirect("Document.aspx?typeAttachment=" + attachment.TypeAttachment);
                        //Response.End();
                    }
                    return;
            }
        }

        #endregion

        protected void AttachmentsBtnModify_Click(object sender, EventArgs e)
        {
            if (!ModifyAttachmentInGrid)
            {
                Allegato attachment = DocumentManager.GetSelectedAttachment();
                this.AttachmentDescription.Text = attachment.descrizione;
                this.AttachmentPagesCount.Text = attachment.numeroPagine.ToString();
                ModifyAttachmentInGrid = true;
                Page_Load(sender, e);

            }
            else
            {
                int numPag = 0;
                if (Int32.TryParse(this.AttachmentPagesCount.Text, out numPag))
                {
                    if (!string.IsNullOrWhiteSpace(this.AttachmentDescription.Text))
                    {
                        Allegato attachment = DocumentManager.GetSelectedAttachment();
                        SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
                        List<DocsPaWR.Allegato> attachments = new List<DocsPaWR.Allegato>(documentTab.allegati);
                        attachment.descrizione = this.AttachmentDescription.Text;

                        attachment.numeroPagine = numPag;
                        documentTab.allegati = attachments.ToArray();
                        ProxyManager.GetWS().DocumentoModificaAllegato(UserManager.GetInfoUser(), attachment, documentTab.docNumber);
                        if (documentTab != null && documentTab.systemId != null && !(documentTab.systemId.Equals(documentTab.systemId))) FileManager.removeSelectedFile();
                        ModifyAttachmentInGrid = false;
                        SwapAttachmentInGrid = false;
                        RicaricaGrigliaAllegati = true;
                        Page_Load(sender, e);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningModAttachInGridDesc', 'warning');} else {parent.parent.ajaxDialogModal('WarningSwapAttachment', 'warning');}", true);

                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningModAttachInGridPageNum', 'warning');} else {parent.parent.ajaxDialogModal('WarningSwapAttachment', 'warning');}", true);

                }
            }
        }

        protected void AttachmentsBtnAnnulla_Click(object sender, EventArgs e)
        {
            ModifyAttachmentInGrid = false;
            SwapAttachmentInGrid = false;
            RicaricaGrigliaAllegati = true;
            Page_Load(sender, e);
        }

        protected void AttachmentsAdd_Click(object sender, EventArgs e)
        {
            SchedaDocumento schedaDoc = DocumentManager.getSelectedRecord();
            if (DocumentManager.IsDocumentoInLibroFirma(schedaDoc) && LibroFirmaManager.IsAttivoBloccoModificheDocumentoInLibroFirma())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningBloccoModificheDocumentoInLf', 'warning');} else {parent.parent.ajaxDialogModal('WarningBloccoModificheDocumentoInLf', 'warning');}", true);
                return;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AttachmentsAdd", "ajaxModalPopupAttachmentsAdd();", true);
        }

        protected void AttachmentsBtnRemove_Click(object sender, EventArgs e)
        {
            SchedaDocumento schedaDoc = DocumentManager.getSelectedRecord();
            if (DocumentManager.IsDocumentoInLibroFirma(schedaDoc) && LibroFirmaManager.IsAttivoBloccoModificheDocumentoInLibroFirma())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningBloccoModificheDocumentoInLf', 'warning');} else {parent.parent.ajaxDialogModal('WarningBloccoModificheDocumentoInLf', 'warning');}", true);
                return;
            }
            string versionLabelAttachment = FileManager.GetFileRequest(VersionIdAttachSelected).versionLabel;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('ConfirmRemoveAttachment', 'HiddenRemoveAttachment', '','" + versionLabelAttachment + "');} else {parent.ajaxConfirmModal('ConfirmRemoveAttachment', 'HiddenRemoveAttachment', '','" + versionLabelAttachment + "');}", true);
            return;
        }

        protected void AttachmentsBtnSwap_Click(object sender, EventArgs e)
        {
            if (this.EnabledManagingSwapAttachments)
            {
                if (!SwapAttachmentInGrid)
                {
                    Allegato attachment = DocumentManager.GetSelectedAttachment();
                    this.AttachmentDescription.Text = attachment.descrizione;
                    this.AttachmentPagesCount.Text = attachment.numeroPagine.ToString();
                    SwapAttachmentInGrid = true;
                    Page_Load(sender, e);
                }
                else
                {
                    int numPag = 0;
                    if (Int32.TryParse(this.AttachmentPagesCount.Text, out numPag))
                    {
                        if (!string.IsNullOrWhiteSpace(this.AttachmentDescription.Text))
                        {
                            Allegato attachment = DocumentManager.GetSelectedAttachment();
                            SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
                            List<DocsPaWR.Allegato> attachments = new List<DocsPaWR.Allegato>(documentTab.allegati);
                            attachment.descrizione = this.AttachmentDescription.Text;

                            attachment.numeroPagine = numPag;
                            documentTab.allegati = attachments.ToArray();
                            ProxyManager.GetWS().DocumentoModificaAllegato(UserManager.GetInfoUser(), attachment, documentTab.docNumber);
                            if (documentTab != null && documentTab.systemId != null && !(documentTab.systemId.Equals(documentTab.systemId))) FileManager.removeSelectedFile();
                            this.SwapAttachment();
                            this.ViewDocument.ShowDocumentAcquired(false);
                            ModifyAttachmentInGrid = false;
                            SwapAttachmentInGrid = false;
                            RicaricaGrigliaAllegati = true;
                            Page_Load(sender, e);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningModAttachInGridDesc', 'warning');} else {parent.parent.ajaxDialogModal('WarningSwapAttachment', 'warning');}", true);

                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningModAttachInGridPageNum', 'warning');} else {parent.parent.ajaxDialogModal('WarningSwapAttachment', 'warning');}", true);

                    }
                }
            }
            else
            {
                SwapAttachment();
                this.grdAllegati_Bind();
                this.panelAllegati.Update();
                EnableButtons();
                panelButtons.Update();
                this.ViewDocument.ShowDocumentAcquired(false);
                Page_Load(sender, e);
            }
        }

        protected string GetIdDocumento()
        {
            return (DocumentManager.GetSelectedAttachment() != null ? (DocumentManager.GetSelectedAttachment().docNumber != null ? DocumentManager.GetSelectedAttachment().docNumber : "0") : "0");
        }

        protected void AttachmentsBtnUploadAllegati_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "AttachmentsAdd", "ajaxModalPopupAttachmentsUpload();", true);
        }
    }

}
