using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;
using NttDataWA.Popup;


namespace NttDataWA.UserControls
{
    public partial class ViewDocument : System.Web.UI.UserControl
    {

        #region Property
        /// <summary>
        ///  document file to be show
        /// </summary>
        private FileDocumento FileDoc
        {
            get
            {
                return HttpContext.Current.Session["fileDoc"] as FileDocumento;
            }
            set
            {
                HttpContext.Current.Session["fileDoc"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string PositionSignature
        {
            get
            {
                if (HttpContext.Current.Session["position"] != null)
                    return HttpContext.Current.Session["position"].ToString();
                else return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool TypeLabel
        {
            get
            {
                if (HttpContext.Current.Session["typeLabel"] != null)
                    return (bool)HttpContext.Current.Session["typeLabel"];
                else return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string RotationSignature
        {
            get
            {
                if (HttpContext.Current.Session["rotation"] != null)
                    return HttpContext.Current.Session["rotation"].ToString();
                else return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string CharacterSignature
        {
            get
            {
                if (HttpContext.Current.Session["character"] != null)
                    return HttpContext.Current.Session["character"].ToString();
                else return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string ColorSignature
        {
            get
            {
                if (HttpContext.Current.Session["color"] != null)
                    return HttpContext.Current.Session["color"].ToString();
                else return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string OrientationSignature
        {
            get
            {
                if (HttpContext.Current.Session["orientation"] != null)
                    return HttpContext.Current.Session["orientation"].ToString();
                else return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool NoTimbro
        {
            get
            {
                if (HttpContext.Current.Session["NoTimbro"] != null)
                    return (bool)HttpContext.Current.Session["NoTimbro"];
                else return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool PrintOnFirstPage
        {
            get
            {
                if (HttpContext.Current.Session["printOnFirstPage"] != null)
                    return (bool)HttpContext.Current.Session["printOnFirstPage"];
                else return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool PrintOnLastPage
        {
            get
            {
                if (HttpContext.Current.Session["printOnLastPage"] != null)
                    return (bool)HttpContext.Current.Session["printOnLastPage"];
                else return false;
            }
        }

        /// <summary>
        ///  
        /// </summary>
        private int FromPagePreview
        {
            get
            {
                return HttpContext.Current.Session["fromPagePreview"] != null ? (int)HttpContext.Current.Session["fromPagePreview"] : 0;
            }
            set
            {
                HttpContext.Current.Session["fromPagePreview"] = value;
            }
        }

        /// <summary>
        ///  
        /// </summary>
        private int LastPagePreview
        {
            get
            {
                return HttpContext.Current.Session["lastPagePreview"] != null ? (int)HttpContext.Current.Session["lastPagePreview"] : 0;
            }
            set
            {
                HttpContext.Current.Session["lastPagePreview"] = value;
            }
        }

        /// <summary>
        ///  
        /// </summary>
        private int TotalPagePreview
        {
            get
            {
                return HttpContext.Current.Session["totalPagePreview"] != null ? (int)HttpContext.Current.Session["totalPagePreview"] : 0;
            }
            set
            {
                HttpContext.Current.Session["totalPagePreview"] = value;
            }
        }

        private TypePrintFormatSign FormatSignature
        {
            get
            {
                if (HttpContext.Current.Session["FormatSignature"] != null)
                    return (TypePrintFormatSign)HttpContext.Current.Session["FormatSignature"];
                else return TypePrintFormatSign.Sign_Extended;
            }
            set
            {
                HttpContext.Current.Session["FormatSignature"] = value;
            }
        }

        private bool VerifyCRL
        {
            get
            {
                if (HttpContext.Current.Session["VerifyCRL"] != null)
                    return (bool)HttpContext.Current.Session["VerifyCRL"];
                else return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool EnableUnifiedView
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableUnifiedView"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableUnifiedView"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableUnifiedView"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool ShowDocumentAsPdfFormat
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["SHOW_DOCUMENT_AS_PDF_FORMAT"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["SHOW_DOCUMENT_AS_PDF_FORMAT"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SHOW_DOCUMENT_AS_PDF_FORMAT"] = value;
            }
        }

        private string SchedaDocSystemId
        {
            get
            {
                if (HttpContext.Current.Session["schedaDocSystemId"] != null)
                    return HttpContext.Current.Session["schedaDocSystemId"].ToString();
                else return string.Empty;
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
        /// True se è stato aggiornato il filtro sul pannello degli allegati
        /// </summary>
        private bool UpdateFilterPanelAttached
        {
            get
            {
                if (PageCaller.Equals(CALLER_ATTACHMENT))
                {
                    string rblFilterAttachments = string.Empty;

                    if ((Parent.FindControl("rblFilter") as RadioButtonList) != null)
                        rblFilterAttachments = (Parent.FindControl("rblFilter") as RadioButtonList).SelectedValue;
                    else
                        rblFilterAttachments = FilterTipoAttach;

                    if (HttpContext.Current.Session["UpdateFilterPanelAttached"] == null)
                    {
                        HttpContext.Current.Session["UpdateFilterPanelAttached"] = rblFilterAttachments;
                        return false;
                    }
                    else if (HttpContext.Current.Session["UpdateFilterPanelAttached"].ToString().Equals(rblFilterAttachments))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    HttpContext.Current.Session.Remove("UpdateFilterPanelAttached");
                    return false;
                }
            }

            set
            {
                string rblFilterAttachments = string.Empty;

                if ((Parent.FindControl("rblFilter") as RadioButtonList) != null)
                    rblFilterAttachments = (Parent.FindControl("rblFilter") as RadioButtonList).SelectedValue;
                else
                    rblFilterAttachments = FilterTipoAttach;

                HttpContext.Current.Session["UpdateFilterPanelAttached"] = rblFilterAttachments;
            }
        }

        [Browsable(true)]
        public string PageCaller
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["pageCaller"] != null)
                {
                    result = HttpContext.Current.Session["pageCaller"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["pageCaller"] = value;
            }
        }

        private bool DisallowOpZoom
        {
            get
            {
                if (HttpContext.Current.Session["disallowOpZoom"] != null)
                    return (bool)HttpContext.Current.Session["disallowOpZoom"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["disallowOpZoom"] = value;
            }
        }

        private bool IsZoom
        {
            get
            {
                if (HttpContext.Current.Session["isZoom"] != null)
                    return (bool)HttpContext.Current.Session["isZoom"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["isZoom"] = value;
            }
        }

        private bool CallFromSignDetails
        {
            get
            {
                if (HttpContext.Current.Session["CallFromSignDetails"] != null)
                    return (bool)HttpContext.Current.Session["CallFromSignDetails"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["CallFromSignDetails"] = value;
            }
        }

        private bool IsInvokingFromProjectStructure
        {
            get
            {
                if (HttpContext.Current.Session["IsInvokingFromProjectStructure"] != null)
                    return (bool)HttpContext.Current.Session["IsInvokingFromProjectStructure"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["IsInvokingFromProjectStructure"] = value;
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
        }

        /// <summary>
        /// Restituisce il file firmato
        /// </summary>
        private FileDocumento DocToSign
        {
            get
            {
                FileDocumento fileDoc = null;
                if (HttpContext.Current.Session["docToSign"] != null)
                    fileDoc = HttpContext.Current.Session["docToSign"] as FileDocumento;
                return fileDoc;
            }
        }

        /// <summary>
        /// Se true viene aggiornato il visualizzatore a seguito dell'eliminazione dell'allegato
        /// </summary>
        private bool UpdateByRemoveAttachment
        {
            get
            {
                if (HttpContext.Current.Session["updateByRemoveAttachment"] != null)
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["updateByRemoveAttachment"]);
                }
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session.Remove("updateByRemoveAttachment");
            }
        }

        /// <summary>
        /// Se true, la chiave di visualizzazione filtro allegati esterni è attiva
        /// </summary>
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

        private bool disableBtnRemoveDocRep
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

        private bool EnabledLibroFirma
        {
            get
            {
                return Convert.ToBoolean(HttpContext.Current.Session["EnabledLibroFirma"]);
            }
            set
            {
                HttpContext.Current.Session["EnabledLibroFirma"] = value;
            }
        }


        /// <summary>
        /// Contiene il nuovo valore per la nota di versione
        /// </summary>
        private string DescriptionVersion
        {
            get
            {
                if (HttpContext.Current.Session["descriptionVersion"] != null)
                    return HttpContext.Current.Session["descriptionVersion"].ToString();
                else return string.Empty;
            }

            set
            {
                HttpContext.Current.Session["descriptionVersion"] = value;
            }
        }

        //E' true se occore aggiornare il bottone del timestamp in seguito ad un timestamp assegnato
        private bool UpdateImageTimestamp
        {
            get
            {
                if (HttpContext.Current.Session["UpdateImageTimestamp"] != null)
                    return Convert.ToBoolean(HttpContext.Current.Session["UpdateImageTimestamp"]);
                else return false;
            }
            set
            {
                HttpContext.Current.Session["UpdateImageTimestamp"] = value;
            }
        }

        //E' true se si deve caricare una nuova versione se è stato creato un TSD dalla maschera di timestamp
        private bool UpdateNewVersionTSD
        {
            get
            {
                if (HttpContext.Current.Session["UpdateNewVersionTSD"] != null)
                    return Convert.ToBoolean(HttpContext.Current.Session["UpdateNewVersionTSD"]);
                else return false;
            }
            set
            {
                HttpContext.Current.Session["UpdateNewVersionTSD"] = value;
            }
        }

        private bool OpenZoomFromDetailsSender
        {
            get
            {
                if (HttpContext.Current.Session["OpenZoomFromDetailsSender"] != null)
                    return (bool)HttpContext.Current.Session["OpenZoomFromDetailsSender"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["OpenZoomFromDetailsSender"] = value;
            }
        }

        #endregion

        #region Remove property

        private void RemoveIsZoom()
        {
            HttpContext.Current.Session.Remove("isZoom");
        }

        private void RemoveVerifyCRL()
        {
            HttpContext.Current.Session.Remove("VerifyCRL");
        }

        #endregion

        #region const
        private const string TYPE_PEC = "pec";
        private const string TYPE_IS = "SIMPLIFIEDINTEROPERABILITY";
        private const string TYPE_USER = "user";
        private const string TYPE_EXT = "esterni";
        private const string TYPE_ALL = "all";
        private const string CALLER_ATTACHMENT = "ATTACHMENTS";
        private const string CALLER_DOCUMENT = "DOCUMENT";
        private const string SEARCH_DOCUMENT_ADVANCED = "ADVANCED";
        private const string SEARCH_DOCUMENT_SIMPLE = "SIMPLE";
        private const string CALLER_CLASSIFICATIONS = "CLASSIFICATIONS";
        private const string CALLER_NOTIFICATION_CENTER = "NOTIFICATION_CENTER";
        private const string PROJECT = "PROJECT";
        private const int CODE_ATTACH_USER = 1;
        private const int CODE_ATTACH_PEC = 2;
        private const int CODE_ATTACH_IS = 3;
        private const int CODE_ATTACH_EXT = 4;
        private const string PDF = "pdf";
        private const string TSD = "tsd";
        private const string M7M = "m7m";
        private const string EML = "eml";
        private const string PANEL_ALLEGATI = "panelAllegati";
        private const string CONFIRM_REMOVE_ATTACHMENTS = "CONFIRM_REMOVE_ATTACHMENTS";
        private const string UP_BOTTOM_BUTTONS = "UpBottomButtons";
        private const string UP_DOCUMENT_BUTTONS = "UpDocumentButtons";
        private const string POPUP_MODIFY_VERSION = "POPUP_MODIFY_VERSION";
        private const string PANEL_BUTTONS = "panelButtons";
        private const string CLOSE_POPUP_ZOOM = "closeZoom";
        private const string UP_PANEL_OBJECT = "UpdPnlObject";
        private const string CALLER_SMISTAMENTO = "SMISTAMENTO";
        private const string STRUCTURE_INSTANCE = "STRUCTURE_INSTANCE";
        private const string ESAMINA_UNO_A_UNO = "ESAMINA_UNO_A_UNO";
        #endregion

        #region global variable
        private static string signed;
        private static string str_locked;
        private static string lock_InLibroFirma;
        private static string yes;
        private static string no;
        private static string elettronicSignature;
        private static string digitalSignature;
        #endregion

        #region Standard Method

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack || sender == null)
            {
                LoadKeys();
                InitializeLanguage();
                InitializePage();
                if (Parent.FindControl("DocumentButtons") != null)
                    VisibilityRoleFunction();

                VisibilityRoleFunctionVersion();
                ClearSession();
                if (IsInvokingFromProjectStructure)
                    IsZoom = true;

                if (IsZoom || PageCaller == CALLER_SMISTAMENTO || CallFromSignDetails)
                {
                    if ((PageCaller != CALLER_SMISTAMENTO || CallFromSignDetails) && PageCaller != ESAMINA_UNO_A_UNO && FileManager.GetFileRequest() != null && !string.IsNullOrEmpty(FileManager.GetFileRequest().fileName) || PageCaller.ToUpper().Equals(CALLER_ATTACHMENT))
                    {
                        DisallowOpZoom = true;
                        EventHandler eventLinkViewFileDocument = new EventHandler(LinkViewFileDocument);
                        eventLinkViewFileDocument.Invoke(sender, e);
                        BottomButtons.Visible = false;
                    }
                    else if (sender == null)
                    {
                        PageCaller = CALLER_DOCUMENT;
                        InitializeContent();
                    }

                    if (PageCaller == CALLER_SMISTAMENTO && !IsZoom && !CallFromSignDetails)
                    {
                        PlcVersions.Visible = false;
                        string valoreChiave = Utils.InitConfigurationKeys.GetValue("0", "FE_VISUAL_DOC_SMISTAMENTO");
                        FileRequest fileDoc = UIManager.FileManager.getSelectedFile();
                        SchedaDocumento selectedRecord = DocumentManager.getSelectedRecord();
                        if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1") && fileDoc != null && selectedRecord != null && selectedRecord.documenti != null && selectedRecord.documenti.Length > 0 && fileDoc != null && !string.IsNullOrEmpty(fileDoc.fileSize) && Convert.ToUInt32(fileDoc.fileSize) > 0)
                        {
                            AutopreViewSmistamento(sender, e);
                        }
                        else
                        {
                            InitializeContent();
                        }

                    }
                    else
                    {
                        if (PageCaller == CALLER_SMISTAMENTO && IsZoom)
                        {
                            DisallowOpZoom = true;
                            EventHandler eventLinkViewFileDocument = new EventHandler(LinkViewFileDocument);
                            eventLinkViewFileDocument.Invoke(sender, e);
                            BottomButtons.Visible = false;
                        }
                        else if (CallFromSignDetails)
                        {
                            DisallowOpZoom = true;
                            EventHandler eventLinkViewFileDocument = new EventHandler(LinkViewFileDocument);
                            eventLinkViewFileDocument.Invoke(sender, e);
                            BottomButtons.Visible = false;
                            PlcVersions.Visible = false;
                        }
                    }
                }
                //else if (IsInvokingFromProjectStructure)
                //{
                //    divNumberVersion.Visible = false;
                //    UpPnlDocumentData.Update();

                //    UpPnlContentDxDx.Visible = false;
                //    UpPnlContentDxDx.Update();

                //    LinkViewFileDocument(null, null);
                //}
                else
                {
                    InitializeContent();
                }

                //CheckInOut.CheckInOutServices.InitializeContext();
                return;
            }
            else if (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].Equals("UpdPnlObject"))
            {
                if (Session["fileDoc"] != null && Session["personalFileSelected"] != null)
                {
                    //ABBATANGELI - Metodo alternativo per verificare la dimensione del file attuale.
                    //string myfileDoc = UIManager.FileManager.getSelectedFile().fileSize;
                    //int repositoryFileSize = (string.IsNullOrEmpty(myfileDoc) ? 0 : int.Parse(myfileDoc));

                    int repositoryFileSize = ((NttDataWA.DocsPaWR.FileDocumento) Session["fileDoc"]).length;

                    InitializeContent(repositoryFileSize);
                    Session["personalFileSelected"] = null;
                    return;
                }
            }


            if (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].Equals(UP_BOTTOM_BUTTONS))
            {
                if (Request.Form["__EVENTARGUMENT"] != null && (Request.Form["__EVENTARGUMENT"].Equals(POPUP_MODIFY_VERSION)))
                {
                    DescriptionVersion = litDocumentVersion.Text;

                    return;
                }
            }

            if (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].Equals(PANEL_ALLEGATI))
            {
                if (Request.Form["__EVENTARGUMENT"] == null || (!Request.Form["__EVENTARGUMENT"].Equals(CONFIRM_REMOVE_ATTACHMENTS)))
                {
                    InitializeContent();

                    return;
                }
            }
            //rimuovo IsZoom alla chiusura della popup di zoom(da x o pulsante chiudi) quando richiamata da profilo, allegato, classifica
            if (Request.Form["__EVENTTARGET"] != null &&
                (Request.Form["__EVENTTARGET"].Equals(PANEL_BUTTONS) || Request.Form["__EVENTTARGET"].Equals(UP_DOCUMENT_BUTTONS)))
            {
                if (Request.Form["__EVENTARGUMENT"] != null && (Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_ZOOM)))
                {
                    RemoveIsZoom();
                    ClearSession();
                    if (!OpenZoomFromDetailsSender)
                    {
                        ShowDocumentAcquired(true, true);
                    }
                    else
                    {
                        HttpContext.Current.Session.Remove("OpenZoomFromDetailsSender");
                    }

                    return;
                }
            }

            //rimuovo IsZoom alla chiusura della popup di zoom(da x o pulsante chiudi) quando richiamata dalla popup verifica esistenza protocollo
            if (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].Equals(UP_PANEL_OBJECT))
            {
                if (Request.Form["__EVENTARGUMENT"] != null && (Request.Form["__EVENTARGUMENT"].Equals(CLOSE_POPUP_ZOOM)))
                {
                    RemoveIsZoom();
                }
            }

            if (UpdateFilterPanelAttached)
            {
                InitializeContent();

                return;
            }

            if (UpdateByRemoveAttachment)
            {
                UpdateByRemoveAttachment = false;
                InitializeContent();

                return;
            }
            try
            {
                //se il documento è in stato finale e il ruolo non ha lo sblocco dello stato finale
                if (DiagrammiManager.IsDocumentInFinalState() &&
                    Convert.ToInt32(DocumentManager.GetAccessRightDocByDocument(DocumentManager.getSelectedRecord(), UserManager.GetInfoUser())) == 45)
                {
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ZOOM);
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_VIEWFILE);
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_POSITIONSIGNATURE);
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_TIMESTAMP);
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_SAVELOCALFILE);
                    btnAddVersion.Enabled = false;
                    btnModifyVersion.Enabled = false;
                    btnRemoveVersion.Enabled = false;
                }
            }
            catch (Exception)
            { 
            }

            if (DocumentManager.getSelectedAttachId() != null || DocumentManager.getSelectedRecord() != null)
            {
                if (DocumentManager.IsDocumentCheckedOut() || CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.getSelectedRecord().docNumber, DocumentManager.getSelectedRecord().docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord()))
                {
                    btnAddVersion.Enabled = false;
                    btnModifyVersion.Enabled = false;
                    btnRemoveVersion.Enabled = false;
                }
            }
            if (UpdateImageTimestamp || UpdateNewVersionTSD)
            {
                if (UpdateImageTimestamp)
                {
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                    HttpContext.Current.Session.Remove("UpdateImageTimestamp");
                }
                if (UpdateNewVersionTSD)
                {
                    ShowChangeVersions();
                    HttpContext.Current.Session.Remove("UpdateNewVersionTSD");
                }
            }

            if (VerifyCRL)
            {
                RemoveVerifyCRL();
                FileRequest FileReqForCRL;
                if (UIManager.DocumentManager.getSelectedAttachId() != null)
                {
                    FileReqForCRL = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
                }
                else
                {
                    FileReqForCRL = FileManager.GetFileRequest();
                }
                if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                {
                    FileReqForCRL = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
                }
                if (FileReqForCRL != null)
                {
                    IsVisibleButtonIdentityCard(FileReqForCRL);

                    UpBottomButtons.Update();
                    if ((DocumentButtons)Parent.FindControl("DocumentButtons") != null)
                        (((DocumentButtons)Parent.FindControl("DocumentButtons")).FindControl("UpDocumentButtons") as UpdatePanel).Update();
                }
            }
        }

        protected void LoadKeys()
        {
            InfoUtente userInfo = UserManager.GetInfoUser();
            //if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VIS_UNIFICATA.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.VIS_UNIFICATA.ToString()].Equals("1"))
            //{
            EnableUnifiedView = true;
            //}
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.SHOW_DOCUMENT_AS_PDF_FORMAT.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.SHOW_DOCUMENT_AS_PDF_FORMAT.ToString()].Equals("1"))
            {
                ShowDocumentAsPdfFormat = true;
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_FILTRO_ALLEGATI_ESTERNI.ToString())) &&
                Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_FILTRO_ALLEGATI_ESTERNI.ToString()).Equals("1"))
            {
                EnabledFilterExternal = true;
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_DISABLE_BTN_REMOVE_DOC_REP.ToString())) &&
                Utils.InitConfigurationKeys.GetValue(userInfo.idAmministrazione, DBKeys.FE_DISABLE_BTN_REMOVE_DOC_REP.ToString()).Equals("1"))
            {
                disableBtnRemoveDocRep = true;
            }
            else
            {
                disableBtnRemoveDocRep = false;
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("1"))
            {
                EnabledLibroFirma = true;
            }
        }

        public void InitializePage()
        {
            btnAddVersion.ImageUrl = ResolveUrl("~/Images/Icons/add_version.png");
            btnAddVersion.OnMouseOverImage = ResolveUrl("~/Images/Icons/add_version_hover.png");
            btnAddVersion.OnMouseOutImage = ResolveUrl("~/Images/Icons/add_version.png");
            btnAddVersion.ImageUrlDisabled = ResolveUrl("~/Images/Icons/add_version_disabled.png");
            btnRemoveVersion.ImageUrl = ResolveUrl("~/Images/Icons/delete.png");
            btnRemoveVersion.OnMouseOverImage = ResolveUrl("~/Images/Icons/delete_hover.png");
            btnRemoveVersion.OnMouseOutImage = ResolveUrl("~/Images/Icons/delete.png");
            btnRemoveVersion.ImageUrlDisabled = ResolveUrl("~/Images/Icons/delete_disabled.png");
            btnModifyVersion.ImageUrl = ResolveUrl("~/Images/Icons/edit_verion.png");
            btnModifyVersion.OnMouseOverImage = ResolveUrl("~/Images/Icons/edit_verion_hover.png");
            btnModifyVersion.OnMouseOutImage = ResolveUrl("~/Images/Icons/edit_verion.png");
            btnModifyVersion.ImageUrlDisabled = ResolveUrl("~/Images/Icons/edit_verion_disabled.png");

            if (PageCaller == CALLER_SMISTAMENTO && !IsZoom)
            {
                contentDxSx.Attributes.Remove("class");
                contentDxSx.Attributes.Add("class", "contentDxSxSmista");
            }
            if (PageCaller == ESAMINA_UNO_A_UNO)
            {
                contentDxSx.Attributes.Remove("class");
                contentDxSx.Attributes.Add("class", "contentDxSxEsamina");
            }
        }

        public void AutopreViewSmistamento(object sender, EventArgs e)
        {
            EventHandler eventLinkViewFileDocument = new EventHandler(LinkViewFileDocument);
            eventLinkViewFileDocument.Invoke(sender, e);
            BottomButtons.Visible = false;
        }

        public void InitializeContent(int alternativeFileSize = 0)
        {
            if (!string.IsNullOrEmpty(PageCaller))
            {
                FileRequest fileDoc = UIManager.FileManager.getSelectedFile();
                if (alternativeFileSize > 0) fileDoc.fileSize = alternativeFileSize.ToString();

                SchedaDocumento selectedRecord = DocumentManager.getSelectedRecord();

                if (!string.IsNullOrEmpty(SchedaDocSystemId) && (DocumentManager.IsNewDocument() || (selectedRecord != null && !selectedRecord.systemId.Equals(SchedaDocSystemId))))
                    DeleteLabelPdfProperty();
                //bool bool_locked = (selectedRecord != null ? (selectedRecord.checkOutStatus != null && !string.IsNullOrEmpty(selectedRecord.checkOutStatus.ID)) : false);
                bool bool_locked = false;
                if (DocumentManager.getSelectedRecord() != null)
                {
                    bool_locked = (CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.getSelectedRecord().docNumber, DocumentManager.getSelectedRecord().docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord()));
                }

                switch (PageCaller.ToUpper())
                {
                    case CALLER_DOCUMENT:
                    case CALLER_CLASSIFICATIONS:
                    case SEARCH_DOCUMENT_SIMPLE:
                    case SEARCH_DOCUMENT_ADVANCED:
                    case CALLER_NOTIFICATION_CENTER:
                    case PROJECT:
                        if (fileDoc != null && !string.IsNullOrEmpty(fileDoc.fileSize) && Convert.ToUInt32(fileDoc.fileSize) > 0)
                        {
                            UpPnlDocumentNotAcquired.Visible = false;
                            UpPnlDocumentAcquired.Visible = true;
                            if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                            {
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ALL);
                                DocumentImgIdentityCard.Enabled = true;
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UPLOADFILE);
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();

                                if (DisableSignature(fileDoc))
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_POSITIONSIGNATURE);
                                if (IsVisibleButtonIdentityCard(fileDoc))
                                {
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.V_IDENTITYCARD);
                                    DocumentImgIdentityCard.Visible = true;
                                }
                                else
                                {
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                                    DocumentImgIdentityCard.Visible = false;
                                }
                                if (fileDoc.firmato == "1")
                                {
                                    string tipoFirma = signed;
                                    if (!string.IsNullOrEmpty(fileDoc.tipoFirma))
                                        tipoFirma = fileDoc.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                                    LitDocumentSignature.Text = tipoFirma;
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                                    if (System.IO.Path.GetExtension(fileDoc.fileName).ToLower().Equals(".pdf"))
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                                }
                                else
                                {
                                    LitDocumentSignature.Text = "";
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                                }

                                if (bool_locked)
                                {
                                    LitDocumentBlocked.Text = str_locked;
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_CHECKOUT);
                                }
                                else
                                {
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UNLOCKED);
                                }
                                //disabilito i pulsanti firma. //ABBA - PERCHE' DISABILITATI ???
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).DisableButtonImageSign();

                                ////Libro Firma
                                //if (fileDoc.inLibroFirma)
                                //{
                                //    if (LibroFirmaManager.IsTitolare(fileDoc.docNumber, UserManager.GetInfoUser()))
                                //        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_LIBROFIRMA_UNLOCK);
                                //    else
                                //        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_LIBROFIRMA_LOCK);

                                //    LitDocumentSignature.Text = lock_InLibroFirma;
                                //}
                            }
                            ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile", UIManager.UserManager.GetUserLanguage()) + " V" + fileDoc.version;
                            ViewDocumentImageDocumentAcquired.Src = GetVersionImage(fileDoc, true);
                            LitDocumentSize.Text = Convert.ToString(Math.Round((double)Int32.Parse(fileDoc.fileSize) / 1024, 3)) + " Kb";
                            UpdateTooltipInfoVersionLight(fileDoc);

                            //Aggiungo questa parte di codice per visualizzare sempre il pannello delle versioni in basso
                            UpPnlDocumentData.Visible = true;
                            //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                            FindControl("divFrame").Visible = false;
                            DocumentManager.setSelectedNumberVersion(fileDoc.version);
                            BindVersions(fileDoc.versionId);
                        }
                        else if (fileDoc != null && selectedRecord != null && selectedRecord.documenti != null && selectedRecord.documenti.Length > 1)
                        {
                            UpPnlDocumentNotAcquired.Visible = true;
                            UpPnlDocumentData.Visible = true;
                            UpPnlDocumentAcquired.Visible = false;
                            GrdDocumentAttached.Visible = true;
                            GridAttachNoUser.Visible = true;

                            //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                            FindControl("divFrame").Visible = false;
                            ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframe();", true);
                            ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
                            DocumentManager.setSelectedNumberVersion(fileDoc.version);
                            BindVersions(fileDoc.versionId);
                            BindDocumentAttached();
                            if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                            {
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                                DocumentImgIdentityCard.Visible = false;
                                //se il documento non è in uno stato finale oppure è in stato finale
                                //ma è stato effetuato lo sblocco stato per il ruolo corrente 
                                //allora disabilito il pulsante di acquisiz.
                                if (!DiagrammiManager.IsDocumentInFinalState() ||
                                    Convert.ToInt32(DocumentManager.GetAccessRightDocByDocument(
                                    DocumentManager.getSelectedRecord(), UserManager.GetInfoUser())) > 45)
                                {
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_UPLOADFILE);
                                }
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                                LitDocumentSignature.Text = "";
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                                LitDocumentSize.Text = string.Empty;
                            }
                        }
                        else
                        {
                            LitDocumentSize.Text = string.Empty;
                            UpPnlDocumentNotAcquired.Visible = true;
                            UpPnlDocumentAcquired.Visible = false;
                            UpPnlDocumentData.Visible = false;
                            GrdDocumentAttached.Visible = false;
                            string language = UIManager.UserManager.GetUserLanguage();
                            ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile", language);
                            if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                            {
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                                DocumentImgIdentityCard.Visible = false;
                                //se il documento non è in uno stato finale oppure è in stato finale
                                //ma è stato effetuato lo sblocco stato per il ruolo corrente 
                                //allora disabilito il pulsante di acquisiz.
                                if (!DiagrammiManager.IsDocumentInFinalState() ||
                                    Convert.ToInt32(DocumentManager.GetAccessRightDocByDocument(
                                    DocumentManager.getSelectedRecord(), UserManager.GetInfoUser())) > 45)
                                {
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_UPLOADFILE);
                                }
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                                LitDocumentSignature.Text = "";
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                            }
                            ResetBottomButtons(bool_locked);
                        }
                        break;

                    case CALLER_ATTACHMENT:
                        Allegato attSel = null;
                        int size = 0;
                        if (DocumentManager.getSelectedAttachId() == null)
                            DocumentManager.setSelectedAttachId(VersionIdAttachSelected);
                        string valueRblFilterAttachments = string.Empty;
                        if ((Parent.FindControl("rblFilter") as RadioButtonList) != null)
                            valueRblFilterAttachments = (Parent.FindControl("rblFilter") as RadioButtonList).SelectedValue;
                        else
                            valueRblFilterAttachments = FilterTipoAttach;
                        switch (valueRblFilterAttachments)
                        {
                            case TYPE_PEC:
                                size = (from att in selectedRecord.allegati where att.TypeAttachment == CODE_ATTACH_PEC select att).ToArray().Length;
                                break;
                            case TYPE_IS:
                                size = (from att in selectedRecord.allegati where att.TypeAttachment == CODE_ATTACH_IS select att).ToArray().Length;
                                break;
                            case TYPE_EXT:
                                size = (from att in selectedRecord.allegati where att.TypeAttachment == CODE_ATTACH_EXT select att).ToArray().Length;
                                break;
                            case TYPE_USER:
                                size = (from att in selectedRecord.allegati where att.TypeAttachment == CODE_ATTACH_USER select att).ToArray().Length;
                                break;
                            case TYPE_ALL:
                                size = selectedRecord.allegati != null ? selectedRecord.allegati.Length : 0;
                                break;
                        }
                        if (size > 0)
                        {
                            if (UpdateFilterPanelAttached) //poichè non è ancora stato aggiornato il versionId dal pannello a sinistra, prendo il primo allegato della lista
                            {
                                if (valueRblFilterAttachments.Equals(TYPE_USER))
                                    attSel = (from att in selectedRecord.allegati where att.TypeAttachment == CODE_ATTACH_USER select att).FirstOrDefault();
                                else if (valueRblFilterAttachments.Equals(TYPE_PEC))
                                    attSel = (from att in selectedRecord.allegati where att.TypeAttachment == CODE_ATTACH_PEC select att).FirstOrDefault();
                                else if (valueRblFilterAttachments.Equals(TYPE_IS))
                                    attSel = (from att in selectedRecord.allegati where att.TypeAttachment == CODE_ATTACH_IS select att).FirstOrDefault();
                                else if (valueRblFilterAttachments.Equals(TYPE_EXT))
                                    attSel = (from att in selectedRecord.allegati where att.TypeAttachment == CODE_ATTACH_EXT select att).FirstOrDefault();
                                else
                                    attSel = (from att in selectedRecord.allegati select att).FirstOrDefault();
                            }
                            else
                                attSel = (from att in selectedRecord.allegati where att.versionId.Equals(DocumentManager.getSelectedAttachId()) select att).FirstOrDefault();
                        }
                        if (size > 0 && attSel != null && !string.IsNullOrEmpty(attSel.fileSize) && Convert.ToInt32(attSel.fileSize) > 0)
                        {
                            UpPnlDocumentNotAcquired.Visible = false;
                            UpPnlDocumentAcquired.Visible = true;
                            //Aggiungo questa parte di codice per visualizzare sempre il pannello delle versioni in basso
                            UpPnlDocumentData.Visible = true;
                            //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                            FindControl("divFrame").Visible = false;
                            DocumentManager.setSelectedNumberVersion(attSel.version);
                            //**Emanuela : aggiunto 17/09/2013 per errore caricamento allegati non utente da filtro (caso di un documento senza allegati utente)
                            DocumentManager.setSelectedAttachId(attSel.versionId);
                            //**Fine Emanuela
                            BindVersions(attSel.versionId);
                            //UpPnlDocumentData.Visible = false;
                            //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                            GrdDocumentAttached.Visible = false;
                            GridAttachNoUser.Visible = false;
                            if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                            {
                                if (attSel.TypeAttachment != CODE_ATTACH_USER && attSel.TypeAttachment != CODE_ATTACH_EXT)
                                {
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_SAVELOCALFILE);
                                    if (IsAcquired(FileManager.GetFileRequest(VersionIdAttachSelected)))
                                    {
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ZOOM);
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_VIEWFILE);
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_TIMESTAMP);
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                                    }
                                    if (IsVisibleButtonIdentityCard(attSel))
                                    {
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.V_IDENTITYCARD);
                                        DocumentImgIdentityCard.Visible = true;
                                    }
                                    else
                                    {
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                                        DocumentImgIdentityCard.Visible = false;
                                    }

                                }
                                else
                                {
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ALL);
                                    DocumentImgIdentityCard.Enabled = true;
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UPLOADFILE);
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                                    if (DisableSignature(attSel) || DocumentManager.IsNewDocument())
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_POSITIONSIGNATURE);
                                    if (IsVisibleButtonIdentityCard(attSel))
                                    {
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.V_IDENTITYCARD);
                                        DocumentImgIdentityCard.Visible = true;
                                    }
                                    else
                                    {
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                                        DocumentImgIdentityCard.Visible = false;
                                    }

                                    ////Libro Firma
                                    //if (attSel.inLibroFirma)
                                    //{
                                    //    if (LibroFirmaManager.IsTitolare(fileDoc.docNumber, UserManager.GetInfoUser()))
                                    //        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_LIBROFIRMA_UNLOCK);
                                    //    else
                                    //        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_LIBROFIRMA_LOCK);

                                    //    LitDocumentSignature.Text = lock_InLibroFirma;
                                    //}
                                }
                            }
                            ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile",
                                UIManager.UserManager.GetUserLanguage()) + " V" + attSel.version;
                            ViewDocumentImageDocumentAcquired.Src = GetVersionImage(attSel, true);
                            LitDocumentSize.Text = Convert.ToString(Math.Round((double)Int32.Parse(attSel.fileSize) / 1024, 3)) + " Kb";
                            //SIGNED

                            if (!DocumentManager.IsNewDocument())
                            {
                                if (attSel != null && !string.IsNullOrEmpty(attSel.firmato))
                                {
                                    if (attSel.firmato.Equals("1"))
                                    {
                                        string tipoFirma = signed;
                                        if (!string.IsNullOrEmpty(attSel.tipoFirma))
                                            tipoFirma = attSel.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                                        LitDocumentSignature.Text = tipoFirma;
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                                        if(System.IO.Path.GetExtension(attSel.fileName).ToLower().Equals(".pdf"))
                                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                                    }
                                    else
                                    {
                                        LitDocumentSignature.Text = string.Empty;
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                                    }
                                }
                                else
                                {
                                    DocumentManager.ListDocVersions = DocumentManager.getDocumentListVersions(Page, attSel.docNumber, attSel.docNumber).documenti;
                                    FileRequest fileReq = (from v in DocumentManager.ListDocVersions where v.version.Equals(attSel.version) select v).FirstOrDefault();
                                    // Prova di ottimizzazione non valida. Capire se si può sostituire il prelievo delle versioni ed utilizzare attSel
                                    // precedentemente prelevato.
                                    //FileRequest fileReq = FileManager.GetFileRequest(fileDoc.versionId);

                                    if (!string.IsNullOrEmpty(fileReq.firmato) && fileReq.firmato.Equals("1"))
                                    {
                                        string tipoFirma = signed;
                                        if (!string.IsNullOrEmpty(fileReq.tipoFirma))
                                            tipoFirma = fileReq.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                                        LitDocumentSignature.Text = tipoFirma;
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                                        if (System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"))
                                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                                    }
                                    else
                                    {
                                        LitDocumentSignature.Text = string.Empty;
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                                    }
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(attSel.firmato) && attSel.firmato.Equals("1"))
                                {
                                    string tipoFirma = signed;
                                    if (!string.IsNullOrEmpty(attSel.tipoFirma))
                                        tipoFirma = attSel.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                                    LitDocumentSignature.Text = tipoFirma;
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                                    if (System.IO.Path.GetExtension(attSel.fileName).ToLower().Equals(".pdf"))
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                                }
                                else
                                {
                                    LitDocumentSignature.Text = string.Empty;
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                                }
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).DisableButtonImageSign();
                            }
                            ////CHECKOUT
                            string documentId = attSel.docNumber;
                            string docNum = attSel.docNumber;
                            /* ABBA 10092013
                            if (DocumentManager.getSelectedAttachId() != null)
                                docNum = documentId = UIManager.DocumentManager.GetSelectedAttachment().docNumber;
                            */

                            bool b_locked = CheckInOut.CheckInOutServices.IsCheckedOutDocument(docNum, documentId, UserManager.GetInfoUser(), false, DocumentManager.getSelectedRecord());
                            if (b_locked)
                            {
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_CHECKOUT);
                                LitDocumentBlocked.Text = str_locked;
                            }
                            else
                            {
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UNLOCKED);
                                LitDocumentBlocked.Text = "";
                            }

                            UpdateTooltipInfoVersionLight(attSel);

                            //disabilito i pulsanti firma.
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).DisableButtonImageSign();

                        }
                        else if (size > 0 && attSel != null && attSel.docNumber != null && (DocumentManager.getDocumentListVersions(Page, attSel.docNumber, attSel.docNumber).documenti.Length > 1))
                        {
                            UpPnlDocumentNotAcquired.Visible = true;
                            UpPnlDocumentData.Visible = true;
                            UpPnlDocumentAcquired.Visible = false;
                            GrdDocumentAttached.Visible = true;
                            GridAttachNoUser.Visible = true;

                            //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                            FindControl("divFrame").Visible = false;
                            ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframe();", true);
                            ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
                            DocumentManager.setSelectedNumberVersion(attSel.version);
                            BindVersions(attSel.versionId);
                            BindDocumentAttached();
                            if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                            {
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                                LitDocumentSignature.Text = string.Empty;
                                //se il documento non è in uno stato finale oppure è in stato finale
                                //ma è stato effetuato lo sblocco stato per il ruolo corrente 
                                //allora abilito il pulsante di acquisiz.
                                if (!DiagrammiManager.IsDocumentInFinalState() ||
                                    Convert.ToInt32(DocumentManager.GetAccessRightDocByDocument(
                                    DocumentManager.getSelectedRecord(), UserManager.GetInfoUser())) > 45)
                                {
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_UPLOADFILE);
                                }
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                                DocumentImgIdentityCard.Visible = false;
                            }
                            LitDocumentSize.Text = string.Empty;
                        }
                        else
                        {
                            UpPnlDocumentAcquired.Visible = false;
                            UpPnlDocumentNotAcquired.Visible = true;

                            UpPnlDocumentData.Visible = false;
                            GrdDocumentAttached.Visible = false;
                            GridAttachNoUser.Visible = false;
                            if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                            {
                                //non ci sono allegati
                                if (size == 0)
                                {
                                    imgInfoVersionSelected.Visible = false;
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                                    DocumentImgIdentityCard.Visible = false;
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                                    LitDocumentBlocked.Text = (bool_locked ? str_locked : string.Empty);

                                }
                                else // non è stato ancora acquisito un file per l'unica versione presente dell'allegato
                                {
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                                    DocumentImgIdentityCard.Visible = false;
                                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);

                                    //se il documento non è in uno stato finale oppure è in stato finale
                                    //ma è stato effetuato lo sblocco stato per il ruolo corrente 
                                    //allora abilito il pulsante di acquisiz.

                                    if (attSel != null)
                                    {
                                        DocumentManager.setSelectedNumberVersion(attSel.version);
                                        if (!DocumentManager.IsNewDocument())
                                        {
                                            DocumentManager.ListDocVersions = DocumentManager.getDocumentListVersions(Page, attSel.docNumber, attSel.docNumber).documenti;
                                        }
                                    }

                                    if (!DiagrammiManager.IsDocumentInFinalState() ||
                                        Convert.ToInt32(DocumentManager.GetAccessRightDocByDocument(
                                        DocumentManager.getSelectedRecord(), UserManager.GetInfoUser())) > 45)
                                    {
                                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_UPLOADFILE);
                                    }
                                    ResetBottomButtons(bool_locked);
                                }
                            }
                            LitDocumentSize.Text = string.Empty;
                        }
                        UpdateFilterPanelAttached = false;
                        UpPnlContentDxSx.Update();
                        UpPnlContentDxDx.Update();
                        break;
                    case CALLER_SMISTAMENTO:
                        if (!IsZoom)
                        {
                            if (fileDoc != null && !string.IsNullOrEmpty(fileDoc.fileSize) && Convert.ToUInt32(fileDoc.fileSize) > 0)
                            {
                                UpPnlDocumentNotAcquired.Visible = false;
                                UpPnlDocumentAcquired.Visible = true;
                                ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile", UIManager.UserManager.GetUserLanguage()) + " V" + fileDoc.version;
                                //ViewDocumentImageDocumentAcquired.Src = GetVersionImage(fileDoc, true);
                                LitDocumentSize.Visible = false;
                                imgInfoVersionSelected.Visible = false;
                                BottomButtons.Visible = false;
                                ViewDocumentLinkFile.Attributes.Add("onchange", "disallowOp('Content2');");
                                if (PageCaller.Equals(CALLER_SMISTAMENTO) && !CallFromSignDetails)
                                    ((SmistamentoDocumenti)Page).RefreshZoom2();


                            }
                            else if (fileDoc != null && selectedRecord != null && selectedRecord.documenti != null && selectedRecord.documenti.Length > 1)
                            {
                                UpPnlDocumentNotAcquired.Visible = true;
                                UpPnlDocumentData.Visible = true;
                                UpPnlDocumentAcquired.Visible = false;
                                GrdDocumentAttached.Visible = true;
                                GridAttachNoUser.Visible = true;
                                BottomButtons.Visible = false;

                                //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                                FindControl("divFrame").Visible = false;
                                ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframeViewerSmistamento();", true);
                                ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
                                DocumentManager.setSelectedNumberVersion(fileDoc.version);
                                BindDocumentAttached();
                                LitDocumentSize.Visible = false;
                                imgInfoVersionSelected.Visible = false;
                                if (PageCaller.Equals(CALLER_SMISTAMENTO) && !CallFromSignDetails)
                                    ((SmistamentoDocumenti)Page).RefreshZoom2();

                                if (PageCaller == ESAMINA_UNO_A_UNO)
                                {
                                    (FindControl("UpPnlContentDxDx") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                                    (FindControl("PlcVersions") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                                }
                            }
                            else
                            {
                                LitDocumentSize.Text = string.Empty;
                                UpPnlDocumentNotAcquired.Visible = true;
                                UpPnlDocumentAcquired.Visible = false;
                                UpPnlDocumentData.Visible = false;
                                GrdDocumentAttached.Visible = false;
                                string language = UIManager.UserManager.GetUserLanguage();
                                ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile", language);
                                LitDocumentSize.Visible = false;
                                imgInfoVersionSelected.Visible = false;
                                BottomButtons.Visible = false;
                                if (PageCaller.Equals(CALLER_SMISTAMENTO) && !CallFromSignDetails)
                                    ((SmistamentoDocumenti)Page).RefreshZoom2();

                            }
                        }

                        break;
                    case ESAMINA_UNO_A_UNO:
                        if (fileDoc != null && !string.IsNullOrEmpty(fileDoc.fileSize) && Convert.ToUInt32(fileDoc.fileSize) > 0)
                        {
                            UpPnlDocumentNotAcquired.Visible = false;
                            UpPnlDocumentAcquired.Visible = true;
                            ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile", UIManager.UserManager.GetUserLanguage()) + " V" + fileDoc.version;
                            //ViewDocumentImageDocumentAcquired.Src = GetVersionImage(fileDoc, true);
                            LitDocumentSize.Visible = false;
                            imgInfoVersionSelected.Visible = false;
                            ViewDocumentLinkFile.Attributes.Add("onchange", "disallowOp('Content2');");
                        }
                        else if (fileDoc != null && selectedRecord != null && selectedRecord.documenti != null && selectedRecord.documenti.Length > 1)
                        {
                            UpPnlDocumentNotAcquired.Visible = true;
                            UpPnlDocumentData.Visible = true;
                            UpPnlDocumentAcquired.Visible = false;
                            GrdDocumentAttached.Visible = true;
                            GridAttachNoUser.Visible = true;
                            this.BindDocumentAttached();
                            //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                            FindControl("divFrame").Visible = false;
                            ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
                            DocumentManager.setSelectedNumberVersion(fileDoc.version);
                            LitDocumentSize.Visible = false;
                            imgInfoVersionSelected.Visible = false;

                        }
                        else
                        {
                            LitDocumentSize.Text = string.Empty;
                            UpPnlDocumentNotAcquired.Visible = true;
                            UpPnlDocumentAcquired.Visible = false;
                            UpPnlDocumentData.Visible = false;
                            GrdDocumentAttached.Visible = false;
                            GridAttachNoUser.Visible = false;
                            string language = UIManager.UserManager.GetUserLanguage();
                            ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile", language);
                            LitDocumentSize.Visible = false;
                            imgInfoVersionSelected.Visible = false;
                        }
                        ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframeViewerSmistamento();", true);
                        PlcVersions.Visible = false;
                        BottomButtons.Visible = false;
                        //GrdDocumentAttached.Visible = false;
                        break;
                    case STRUCTURE_INSTANCE:
                        if (!string.IsNullOrEmpty(fileDoc.fileSize) && Convert.ToInt32(fileDoc.fileSize) > 0)
                        {
                            EventHandler eventLinkViewFileDocument = new EventHandler(LinkViewFileDocument);
                            eventLinkViewFileDocument.Invoke(null, null);
                        }
                        else
                        {
                            UpPnlDocumentNotAcquired.Visible = true;
                            FindControl("divFrame").Visible = false;
                        }
                        BottomButtons.Visible = false;
                        (FindControl("divNumberVersion") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                        GridAttachNoUser.Visible = false;
                        btnAddVersion.Visible = false;
                        btnModifyVersion.Visible = false;
                        btnRemoveVersion.Visible = false;
                        PlcVersions.Visible = false;
                        UpBottomButtons.Update();
                        UpPnlContentDxSx.Update();
                        UpPnlContentDxDx.Update();
                        break;
                }

                if (fileDoc != null && !string.IsNullOrEmpty(fileDoc.fileSize) && Convert.ToUInt32(fileDoc.fileSize) > 0)
                {
                    string extensionFile = (fileDoc.fileName.Split('.').Length > 1) ? (fileDoc.fileName.Split('.'))[fileDoc.fileName.Split('.').Length - 1] : string.Empty;
                    if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null && !verifyExtensionForSign(extensionFile))
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).DisableButtonImageSign();
                }

                if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                {
                    ButtonsManager();

                    if (EnabledLibroFirma)
                        ButtonsManagerLibroFirma();
                }
                UpBottomButtons.Update();
            }
        }
        /// <summary>
        /// Costruisce le stringhe relative al creatore della versione e utente che ha acquisito il file.
        /// Se i due utenti concidono le stringhe conincidono (Creato/Acquisito da), altrimenti sono separate.
        /// </summary>
        /// <param name="fileReq"></param>
        private void BuildStringAuthorFile(FileRequest fileReq)
        {
            litCreatore.Text = fileReq.autore;
            if (fileReq != null && !string.IsNullOrEmpty(fileReq.fileSize) && Convert.ToUInt32(fileReq.fileSize) > 0
                && !string.IsNullOrEmpty(fileReq.autoreFile) && !string.IsNullOrEmpty(fileReq.dataAcquisizione))
            {
                UpdateInfoFileAquired.Visible = true;
                litAuthorFile.Text = fileReq.autoreFile;
                litDocumentDateAcqured.Text = fileReq.dataAcquisizione;
            }
            else
            {
                UpdateInfoFileAquired.Visible = false;
            }
            UpdateInfoFileAquired.Update();
        }

        private Boolean verifyExtensionForSign(string extension)
        {
            Boolean retVal = true;

            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString())) || !Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString()).Equals("1"))
            {
                DocsPaWR.DocsPaWebService ws = NttDataWA.Utils.ProxyManager.GetWS();
                if (ws.IsEnabledSupportedFileTypes())
                {
                    DocsPaWR.SupportedFileType[] fileTypes = ws.GetSupportedFileTypes(Convert.ToInt32(UIManager.UserManager.GetInfoUser().idAmministrazione));
                    int count = fileTypes.Count(e => e.FileExtension.ToLowerInvariant() == extension.ToLowerInvariant() &&
                                                            e.FileTypeUsed && e.FileTypeSignature);
                    retVal = (count > 0);
                }
                else
                    retVal = true;
            }

            return retVal;
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            //this.ViewFullDocPreview.Text = Utils.Languages.GetLabelFromCode("ViewFullDocPreview", language);
            
            //this.imgNextPreview.ToolTip = Utils.Languages.GetLabelFromCode("ViewNextPreview", language);
            //this.imgPrevPreview.ToolTip = Utils.Languages.GetLabelFromCode("ViewPrevPreview", language);
            this.ViewDocumentLblNoAcquired.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLblNoAcquired", language);
            this.ViewDocumentLblAcquired.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLblAcquired", language);
            this.ViewDocumentTxtVersions.InnerText = Utils.Languages.GetLabelFromCode("ViewDocumentTxtVersions", language);
            this.ViewDocumentTxtDate.InnerText = Utils.Languages.GetLabelFromCode("ViewDocumentTxtDate", language);
            this.ViewDocumentTxtDateAcquired.InnerText = Utils.Languages.GetLabelFromCode("ViewDocumentTxtDate", language);
            this.ViewDocumentTxtPapery.InnerText = Utils.Languages.GetLabelFromCode("ViewDocumentTxtPapery", language);
            this.ViewDocumentAuthorFile.InnerText = Utils.Languages.GetLabelFromCode("ViewDocumentAuthorFile", language);
            this.ViewDocumentTxtNoteVersion.InnerText = Utils.Languages.GetLabelFromCode("ViewDocumentTxtNoteVersion", language);
            this.btnRemoveVersion.ToolTip = Utils.Languages.GetLabelFromCode("btnRemoveVersion", language);
            this.btnRemoveVersion.AlternateText = Utils.Languages.GetLabelFromCode("btnRemoveVersion", language);
            this.btnAddVersion.ToolTip = Utils.Languages.GetLabelFromCode("btnAddVersion", language);
            this.btnAddVersion.AlternateText = Utils.Languages.GetLabelFromCode("btnAddVersion", language);
            this.btnModifyVersion.ToolTip = Utils.Languages.GetLabelFromCode("btnModifyVersion", language);
            this.btnModifyVersion.AlternateText = Utils.Languages.GetLabelFromCode("btnModifyVersion", language);
            this.ViewDocumentCreatore.InnerHtml = Utils.Languages.GetLabelFromCode("ViewDocumentCreatore", language);
            yes = Utils.Languages.GetLabelFromCode("yes", language);
            no = Utils.Languages.GetLabelFromCode("no", language);
            signed = Utils.Languages.GetLabelFromCode("signed", language);
            str_locked = Utils.Languages.GetLabelFromCode("locked", language);
            lock_InLibroFirma = Utils.Languages.GetLabelFromCode("lock_InLibroFirma", language);
            elettronicSignature = Utils.Languages.GetLabelFromCode("ElettronicSignature", language);
            digitalSignature = Utils.Languages.GetLabelFromCode("DigitalSignature", language);
            this.DocumentImgIdentityCard.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgIdentityCardTooltip", language);

            
        }

        #endregion

        public void DeleteLabelPdfProperty()
        {
            HttpContext.Current.Session.Remove("character");
            HttpContext.Current.Session.Remove("color");
            HttpContext.Current.Session.Remove("orientation");
            HttpContext.Current.Session.Remove("position");
            HttpContext.Current.Session.Remove("typeLabel");
            HttpContext.Current.Session.Remove("rotation");
            HttpContext.Current.Session.Remove("printOnFirstPage");
            HttpContext.Current.Session.Remove("printOnLastPage");
            HttpContext.Current.Session.Remove("rotation");
            HttpContext.Current.Session.Remove("schedaDocSystemId");
            HttpContext.Current.Session.Remove("NoTimbro");
            HttpContext.Current.Session.Remove("FormatSignature");

        }

        private void ClearSession()
        {
            HttpContext.Current.Session.Remove("disallowOpZoom");
        }

        /// <summary>
        /// Verifica se il file è stato acquisito per la versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected bool IsAcquired(DocsPaWR.FileRequest fileRequest)
        {
            int fileSize = 0;
            if (fileRequest != null)
                Int32.TryParse(fileRequest.fileSize, out fileSize);
            return (fileSize > 0);
        }

        protected bool IsElementoSelezionatoInLf(DocsPaWR.FileRequest fileRequest)
        {
            bool result = false;

            if(this.PageCaller == ESAMINA_UNO_A_UNO)
            {
                result = fileRequest.docNumber.Equals(LibroFirmaManager.DocumentoSelezionatoPerFirma());
            }

            return result;
        }

        #region Buttons

        /// <summary>
        /// Visualizza/nasconde  i pulsanti del visualizzatore in base alle opportune micro funzioni associate al ruolo
        /// </summary>
        private void VisibilityRoleFunction()
        {
            if (UserManager.IsAuthorizedFunctions("DO_DOC_VISUALIZZA"))
            {
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).VisibilityButton(NttDataWA.UserControls.DocumentButtons.TypeVisibilityButton.V_VIEWFILE);
            }
            else
            {
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).VisibilityButton(NttDataWA.UserControls.DocumentButtons.TypeVisibilityButton.U_VIEWFILE);
            }
            if (UserManager.IsAuthorizedFunctions("DO_DOC_VISUALIZZAZOOM"))
            {
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).VisibilityButton(NttDataWA.UserControls.DocumentButtons.TypeVisibilityButton.V_ZOOMFILE);
            }
            else
            {
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).VisibilityButton(NttDataWA.UserControls.DocumentButtons.TypeVisibilityButton.U_ZOOMFILE);
            }
        }

        /// <summary>
        /// Visualizza/nasconde  i pulsanti sulle versione in base alle opportune micro funzioni associate al ruolo
        /// </summary>
        private void VisibilityRoleFunctionVersion()
        {
            if (UserManager.IsAuthorizedFunctions("DO_VER_NUOVA"))
            {
                btnAddVersion.Visible = true;
            }
            else
            {
                btnAddVersion.Visible = false;
            }
            if (UserManager.IsAuthorizedFunctions("DO_VER_MODIFICA"))
            {
                btnModifyVersion.Visible = true;
            }
            else
            {
                btnModifyVersion.Visible = false;
            }
            if (UserManager.IsAuthorizedFunctions("DO_VER_RIMUOVI"))
            {
                btnRemoveVersion.Visible = true;
            }
            else
            {
                btnRemoveVersion.Visible = false;
            }
        }

        /// <summary>
        /// Gestione dei pulsanti in seguito alle azioni di consolidamento, annullamento, 
        /// documento non salvato e diritti dell'utente sul documento
        /// </summary>
        private void ButtonsManager()
        {
            bool disableAll = false;
            SchedaDocumento documentTab = DocumentManager.getSelectedRecord();

            #region check per documento non salvato, utente che non possiede i diritti di scrittura o ereditarietà sul documento

            if (DocumentManager.IsNewDocument() && documentTab.repositoryContext == null && !PageCaller.ToUpper().Equals(CALLER_ATTACHMENT))
            {
                disableAll = true;
            }
            if (!DocumentManager.IsNewDocument() && !UserManager.IsRightsWritingInherits())
            {
                disableAll = true;
            }

            FileRequest fileReq = null;
            double size = 0;

            if (DocumentManager.getSelectedAttachId() != null) // ho aggiunto il file ad un allegato
            {
                fileReq = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
            }
            else // ho aggiunto il file al documento principale
            {
                fileReq = FileManager.GetFileRequest();
            }

            if (disableAll)
            {
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);



                if (fileReq != null && !string.IsNullOrEmpty(fileReq.fileSize))
                    size = (double)int.Parse(fileReq.fileSize) / 1024;

                if ((documentTab.accessRights == "20" || documentTab.accessRights == "45") && size > 0)
                {
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ZOOM);
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_VIEWFILE);
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_POSITIONSIGNATURE);
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_TIMESTAMP);
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_SAVELOCALFILE);
                }
            }
            #endregion

            //Il documento è annullato o se il documento è in cestino
            if (DocumentManager.IsDocumentAnnul() || (documentTab.inCestino != null && documentTab.inCestino == "1"))
            {
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ABORT);
            }

            //Il Documento è stato consolidato
            if (DocumentManager.IsDocumentConsolidate())
            {
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_CONSOLIDATION);
            }
        }


        private void ButtonsManagerLibroFirma()
        {
            SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
            FileRequest fileReq = null;
            if (DocumentManager.getSelectedAttachId() != null)
            {
                if (string.IsNullOrEmpty(DocumentManager.getSelectedAttachId()))
                    fileReq = null;
                else
                    fileReq = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());               
            }
            else
            {
                fileReq = FileManager.GetFileRequest();
            }
            //estraggo la versione  selezionata
            if (DocumentManager.ListDocVersions != null && DocumentManager.ListDocVersions.Count() > 0 && !string.IsNullOrEmpty(DocumentManager.getSelectedNumberVersion()) && !DocumentManager.getSelectedNumberVersion().Equals("0"))
            {
                string versionId = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v.versionId).FirstOrDefault();
                fileReq = FileManager.GetFileRequest(versionId);
            }
            if (fileReq != null && fileReq.inLibroFirma)
            {
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_START_SIGNATURE_PROCESS);
                if (fileReq.firmato.Equals("1"))
                {
                    string tipoFirma = signed;
                    if (!string.IsNullOrEmpty(fileReq.tipoFirma))
                        tipoFirma = fileReq.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                    LitDocumentSignature.Text = tipoFirma + " - " + lock_InLibroFirma;
                }
                else
                {
                    LitDocumentSignature.Text = lock_InLibroFirma;
                }
                if (!LibroFirmaManager.IsTitolare(fileReq.docNumber, UserManager.GetInfoUser()))
                {
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_LIBROFIRMA_LOCK);
                }
                else
                {
                    string typeSignature = LibroFirmaManager.GetTypeSignatureToBeEntered(fileReq);
                    if (typeSignature.Equals(LibroFirmaManager.TypeEvent.VERIFIED))
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_DIGITAL_SIGN);
                    }
                    else if (typeSignature.Equals(LibroFirmaManager.TypeEvent.ADVANCEMENT_PROCESS))
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_DIGITAL_SIGN);
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ELECTRONIC_SIGN);
                    }
                    else
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ELECTRONIC_SIGN);
                    }
                }
            }

            else if (fileReq != null && !string.IsNullOrEmpty(fileReq.fileName) && Convert.ToUInt32(fileReq.fileSize) > 0
                && Convert.ToInt32(documentTab.accessRights) > Convert.ToInt32(HMdiritti.HMdiritti_Read ))
               // per demo emea && FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToLowerInvariant().Equals(PDF))
            {
                bool isLastVersion = true;
                if (DocumentManager.ListDocVersions != null && DocumentManager.ListDocVersions.Count() > 0)
                {
                    string version = (from document in DocumentManager.ListDocVersions select Convert.ToInt32(document.version)).Max().ToString();
                    if (!string.IsNullOrEmpty(version) && !string.IsNullOrEmpty(DocumentManager.getSelectedNumberVersion()) && !DocumentManager.getSelectedNumberVersion().Equals("0"))
                        isLastVersion = DocumentManager.getSelectedNumberVersion().Equals(version);
                }
                if (isLastVersion)
                {
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_START_SIGNATURE_PROCESS);
                }
                else
                {
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_START_SIGNATURE_PROCESS);
                }
            }
            else
            {
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_START_SIGNATURE_PROCESS);
            }
        }

        /// <summary>
        /// Gestione dei pulsanti delle versioni, in seguito alle azioni di consolidamento, annullamento, 
        /// documento non salvato e diritti dell'utente sul documento
        /// </summary>
        private void ButtonsVersionManager(FileRequest fileReq)
        {
            #region check per documento annullato, consolidato, senza diritti di scrittura o ereditarietà sul documento
            bool disableAll = false;
            SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
            //Il documento è annullato
            if (DocumentManager.IsDocumentAnnul())
            {
                disableAll = true;
            }
            // l'utente non possiede i diritti di scrittura o ereditarietà sul documento
            if (!UserManager.IsRightsWritingInherits())
            {
                disableAll = true;
            }
            //Il Documento è stato consolidato
            if (DocumentManager.IsDocumentConsolidate())
            {
                disableAll = true;
            }
            if (documentTab.inCestino != null && documentTab.inCestino == "1")
            {
                disableAll = true;
            }
            if (disableAll)
            {
                btnAddVersion.Enabled = false;
                btnModifyVersion.Enabled = false;
                btnRemoveVersion.Enabled = false;
                return;
            }
            #endregion

            #region check per allegati non utente, dettaglio allegato, allegato utente, allegato contenuto in doc con stato finale
            // nel caso di allegato non utente disabilito i pulsanti sulle versioni
            if (fileReq.GetType() == typeof(Allegato) && ((fileReq as Allegato).TypeAttachment != CODE_ATTACH_USER))
            {
                disableAll = true;
            }
            //nel caso di dettaglio allegato non utente disabilito i pulsanti sulle versioni
            else if (fileReq.GetType() == typeof(Documento) && Page.Request["typeAttachment"] != null && Convert.ToInt32(Page.Request["typeAttachment"]) != CODE_ATTACH_USER)
            {
                disableAll = true;

            }
            // nel caso di un allegato contenuto in documento:
            //- tipizzato con diagramma di stato e stato corrente  finale;
            //- con ruolo che non ha lo sblocco dello stato finale;
            //allora disabilito i pulsanti sulle versioni
            else if (DiagrammiManager.IsDocumentInFinalState() &&
                Convert.ToInt32(DocumentManager.GetAccessRightDocByDocument(DocumentManager.getSelectedRecord(), UserManager.GetInfoUser())) == 45)
            {
                disableAll = true;
            }
            else if (!IsZoom || (PageCaller != CALLER_SMISTAMENTO) || PageCaller != ESAMINA_UNO_A_UNO) // nel caso di allegato utente abilito i pulsanti
            {
                disableAll = false;
            }

            if (DocumentManager.IsDocumentCheckedOut() || CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.getSelectedRecord().docNumber, DocumentManager.getSelectedRecord().docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord()))
            {
                disableAll = true;
            }


            if (disableAll)
            {
                btnAddVersion.Enabled = false;
                btnModifyVersion.Enabled = false;
                btnRemoveVersion.Enabled = false;
            }
            else
            {
                btnAddVersion.Enabled = true;
                btnModifyVersion.Enabled = true;
                btnRemoveVersion.Enabled = true;
            }
            #endregion

            #region check per enable/disable del pulsante rimuovi
            //se vale una delle seguenti asserzioni:
            //- è presente una sola versione;
            //- il documento è in ckeck out;
            //- il documento si trova in uno stato finale ed al ruolo corrente non è stato abilitato lo sblocco stato; 
            //allora disabilito il pulsante rimuovi.
            if ((DocumentManager.ListDocVersions != null && DocumentManager.ListDocVersions.Length == 1) ||
                DocumentManager.IsDocumentCheckedOut() ||
                (DiagrammiManager.IsDocumentInFinalState() &&
                Convert.ToInt32(DocumentManager.GetAccessRightDocByDocument(DocumentManager.getSelectedRecord(),
                UserManager.GetInfoUser())) == 45) ||
                (DocumentManager.IsDocumentoRepertoriato(DocumentManager.getSelectedRecord().template) && disableBtnRemoveDocRep)
                )
            {
                btnRemoveVersion.Enabled = false;
            }
            else
            {
                btnRemoveVersion.Enabled = true;
            }

            if (fileReq.GetType() != typeof(Allegato) && (DocumentManager.getSelectedRecord().protocollo != null && !string.IsNullOrEmpty(DocumentManager.getSelectedRecord().protocollo.segnatura)))
            {
                btnModifyVersion.Enabled = false;
                btnRemoveVersion.Enabled = false;
            }

            #endregion

            //MEV LIBRO FIRMA
            bool isInLibroFirma;
            if (FileManager.GetSelectedAttachment() == null)
            {
                isInLibroFirma = UIManager.FileManager.getSelectedFile().inLibroFirma;
            }
            else
            {
                isInLibroFirma = FileManager.GetSelectedAttachment().inLibroFirma;
            }
            if (isInLibroFirma)
            {
                btnAddVersion.Enabled = false;
                btnModifyVersion.Enabled = false;
                btnRemoveVersion.Enabled = false;
            }
        }

        #endregion

        #region Update Viewer

        /// <summary>
        /// Metodo per il refresh del visualizzatore dopo l'upload di un file
        /// </summary>
        public virtual void RefreshAcquiredDocument()
        {
            SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
            DocumentButtons documentButtons = (DocumentButtons)Parent.FindControl("DocumentButtons");
            FileRequest fileReq = null;
            double size = 0;
            bool bool_locked = (documentTab.checkOutStatus != null && !string.IsNullOrEmpty(documentTab.checkOutStatus.ID));

            if (DocumentManager.getSelectedAttachId() != null) // ho aggiunto il file ad un allegato
            {
                fileReq = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
            }
            else // ho aggiunto il file al documento principale
            {
                fileReq = FileManager.GetFileRequest();
            }
            if (!string.IsNullOrEmpty(fileReq.fileSize))
                size = (double)int.Parse(fileReq.fileSize) / 1024;
            if (size > 0)
            {
                DocumentManager.setSelectedNumberVersion("0");
                BindVersions(fileReq.versionId);
                if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                {
                    if (!bool_locked)
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).ResetCheckInOutState();

                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ALL);
                        DocumentImgIdentityCard.Enabled = true;
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UPLOADFILE);
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                        if (DisableSignature(fileReq))
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_POSITIONSIGNATURE);
                        if (IsVisibleButtonIdentityCard(fileReq))
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.V_IDENTITYCARD);
                            DocumentImgIdentityCard.Visible = true;
                        }
                        else
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                            DocumentImgIdentityCard.Visible = false;
                        }
                        if (fileReq.firmato == "1")
                        {
                            string tipoFirma = signed;
                            if (!string.IsNullOrEmpty(fileReq.tipoFirma))
                                tipoFirma = fileReq.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                            LitDocumentSignature.Text = tipoFirma;
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                            if (System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"))
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                        }
                        else
                        {
                            LitDocumentSignature.Text = string.Empty;
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);

                        }

                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UNLOCKED);

                        if (!FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToLowerInvariant().Equals(PDF))
                        {
                            // per DEMO EMEA-- ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ELECTRONIC_SIGN);
                          // per DEMO EMEA-- vogliono attivare processi firma anche su doc non pdf  ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_START_SIGNATURE_PROCESS);
                        }
                    }
                    else
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_CHECKOUT);
                    }
                }
                //Il seguente il lo sostituisco con quello seguente perchè ora il pannelo UpPnlDocumentData è sempre visibile
                //if (!UpPnlDocumentData.Visible)
                if ((UpPnlDocumentNotAcquired.Visible && !GrdDocumentAttached.Visible) || UpPnlDocumentAcquired.Visible)
                {
                    UpPnlDocumentAcquired.Visible = true;
                    ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile", UIManager.UserManager.GetUserLanguage()) + " V" + fileReq.version;
                    ViewDocumentImageDocumentAcquired.Src = GetVersionImage(fileReq, true);
                    //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                    //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                    FindControl("divFrame").Visible = false;
                    frame.Attributes["src"] = "";
                    UpPnlDocumentData.Visible = true;
                    //disabilito i pulsanti firma.
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).DisableButtonImageSign();
                }
                else
                {
                    //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                    FindControl("divFrame").Visible = true;
                    SaveFileDocument(fileReq);
                    frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";
                    if ((PageCaller.Equals(CALLER_ATTACHMENT) && GrdDocumentAttached.Rows.Count > 0) || !PageCaller.Equals(CALLER_ATTACHMENT))
                    {
                        UpPnlDocumentAcquired.Visible = false;
                        ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).Enabled = true;
                        ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).ImageUrl = GetVersionImage(fileReq);
                        ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).OnMouseOverImage = GetVersionImage(fileReq);
                        ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).OnMouseOutImage = GetVersionImage(fileReq);
                        ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).ImageUrlDisabled = GetVersionImage(fileReq);
                        ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVersioneStampabile") as HyperLink).Visible = true;
                        //string attachVersionId = ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("attachVersionId") as HiddenField).Value;
                        //string linkVersion = ResolveUrl("~/Document/AttachmentViewer.aspx?versionDownload=") + attachVersionId;
                        //((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVersioneStampabile") as HyperLink).NavigateUrl = linkVersion;
                    }
                    UpPnlContentDxDx.Update();
                    UpBottomButtons.Update();
                }

                UpPnlDocumentNotAcquired.Visible = false;
            }
            UpPnlContentDxSx.Update();
            imgInfoVersionSelected.Visible = true;

            if (size == 0)
                ResetBottomButtons(bool_locked);
            ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
        }

        public virtual void RefreshCheckInDocument()
        {
            FileRequest fileReq = null;

            if (DocumentManager.getSelectedAttachId() != null)
            {
                SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
                documentTab.documenti = DocumentManager.GetVersionsMainDocument(UserManager.GetInfoUser(), documentTab.systemId);
                documentTab.allegati = DocumentManager.getAttachments(documentTab, "all");
                DocumentManager.setSelectedRecord(documentTab);
                VersionIdAttachSelected = documentTab.allegati[0].versionId;
                DocumentManager.setSelectedAttachId(VersionIdAttachSelected);
                fileReq = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
            }
            else
            {
                fileReq = FileManager.GetFileRequest();
            }
            //SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
            //DocumentButtons documentButtons = (DocumentButtons)Parent.FindControl("DocumentButtons");

            //documentTab.checkOutStatus = DocumentManager.GetCheckOutDocumentStatus(documentTab.systemId);
            //documentTab.documenti = DocumentManager.GetVersionsMainDocument(UserManager.GetInfoUser(), documentTab.systemId);
            //documentTab.allegati = DocumentManager.getAttachments(documentTab, "all");
            //DocumentManager.setSelectedRecord(documentTab);

            //FileManager.setSelectedFile(documentTab.documenti[0]);
            //FileRequest fileReq = documentTab.documenti[0];
            //FileRequest fileReq = (DocumentManager.getSelectedAttachId() != null) ?
            //      FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) :
            //        FileManager.GetFileRequest();

            if (fileReq != null)
            {
                FileManager.setSelectedFile(fileReq);
                DocumentManager.setSelectedNumberVersion("0");
                BindVersions(fileReq.versionId);
                if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                {
                    //SchedaDocumento scd = DocumentManager.getSelectedRecord();
                    //string docId = scd.docNumber;
                    string docId = fileReq.docNumber;

                    bool bool_locked = CheckInOut.CheckInOutServices.IsCheckedOutDocument(docId, fileReq.docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord());

                    if (!bool_locked)
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ALL);
                        DocumentImgIdentityCard.Enabled = true;
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UPLOADFILE);
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                        if (DisableSignature(fileReq))
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_POSITIONSIGNATURE);
                        if (IsVisibleButtonIdentityCard(fileReq))
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.V_IDENTITYCARD);
                            DocumentImgIdentityCard.Visible = true;
                        }
                        else
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                            DocumentImgIdentityCard.Visible = false;
                        }
                        if (fileReq.firmato == "1")
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                            if (System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"))
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                        }

                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UNLOCKED);
                    }
                    else
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_CHECKOUT);
                    }
                }
                //Questo if non è più valido perchè ora il pannello UpPnlDocumentData è sempre visibile
                //if (!UpPnlDocumentData.Visible)
                if (fileReq.fileSize != "0")
                {
                    if ((UpPnlDocumentNotAcquired.Visible && !GrdDocumentAttached.Visible) || (UpPnlDocumentAcquired.Visible))
                    {
                        UpPnlDocumentAcquired.Visible = true;
                        ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile", UIManager.UserManager.GetUserLanguage()) + " V" + fileReq.version;
                        ViewDocumentImageDocumentAcquired.Src = GetVersionImage(fileReq, true);
                        //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                        //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                        FindControl("divFrame").Visible = false;
                        UpPnlDocumentData.Visible = true;
                    }
                    else
                    {
                        //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                        FindControl("divFrame").Visible = true;
                        SaveFileDocument(fileReq);
                        frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";
                        if ((PageCaller.Equals(CALLER_ATTACHMENT) && GrdDocumentAttached.Rows.Count > 0) || !PageCaller.Equals(CALLER_ATTACHMENT))
                        {
                            BindDocumentAttached();
                            GrdDocumentAttached.Visible = true;
                            UpPnlDocumentAcquired.Visible = false;
                            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).Enabled = true;
                            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).ImageUrl = GetVersionImage(fileReq);
                            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).OnMouseOverImage = GetVersionImage(fileReq);
                            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).OnMouseOutImage = GetVersionImage(fileReq);
                            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).ImageUrlDisabled = GetVersionImage(fileReq);
                            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVersioneStampabile") as HyperLink).Visible = true;
                            //string attachVersionId = ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("attachVersionId") as HiddenField).Value;
                            //string linkVersion = ResolveUrl("~/Document/AttachmentViewer.aspx?versionDownload=") + attachVersionId;
                            //((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVersioneStampabile") as HyperLink).NavigateUrl = linkVersion;
                        }
                        UpPnlContentDxDx.Update();
                        UpBottomButtons.Update();
                    }
                    UpPnlDocumentNotAcquired.Visible = false;
                }
            }
            UpPnlContentDxSx.Update();

            ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
        }

        public virtual void RefreshCheckInDocumentNoUpdateDocument()
        {
            FileRequest fileReq = null;

            if (DocumentManager.getSelectedAttachId() != null)
            {
                SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
                documentTab.documenti = DocumentManager.GetVersionsMainDocument(UserManager.GetInfoUser(), documentTab.systemId);
                documentTab.allegati = DocumentManager.getAttachments(documentTab, "all");
                DocumentManager.setSelectedRecord(documentTab);
                VersionIdAttachSelected = documentTab.allegati[0].versionId;
                DocumentManager.setSelectedAttachId(VersionIdAttachSelected);
                fileReq = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
            }
            else
            {
                fileReq = FileManager.GetFileRequest();
            }
            //SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
            //DocumentButtons documentButtons = (DocumentButtons)Parent.FindControl("DocumentButtons");

            //documentTab.checkOutStatus = DocumentManager.GetCheckOutDocumentStatus(documentTab.systemId);
            //documentTab.documenti = DocumentManager.GetVersionsMainDocument(UserManager.GetInfoUser(), documentTab.systemId);
            //documentTab.allegati = DocumentManager.getAttachments(documentTab, "all");
            //DocumentManager.setSelectedRecord(documentTab);

            //FileManager.setSelectedFile(documentTab.documenti[0]);
            //FileRequest fileReq = documentTab.documenti[0];
            //FileRequest fileReq = (DocumentManager.getSelectedAttachId() != null) ?
            //      FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) :
            //        FileManager.GetFileRequest();

            if (fileReq != null)
            {
                FileManager.setSelectedFile(fileReq);
                DocumentManager.setSelectedNumberVersion("0");
                //BindVersions(fileReq.versionId);
                if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                {
                    //SchedaDocumento scd = DocumentManager.getSelectedRecord();
                    //string docId = scd.docNumber;
                    string docId = fileReq.docNumber;

                    bool bool_locked = CheckInOut.CheckInOutServices.IsCheckedOutDocument(docId, fileReq.docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord());

                    if (!bool_locked)
                    {

                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ALL);
                        DocumentImgIdentityCard.Enabled = true;
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UPLOADFILE);
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                        if (DisableSignature(fileReq))
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_POSITIONSIGNATURE);
                        if (IsVisibleButtonIdentityCard(fileReq))
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.V_IDENTITYCARD);
                            DocumentImgIdentityCard.Visible = true;
                        }
                        else
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                            DocumentImgIdentityCard.Visible = false;
                        }
                        if (fileReq.firmato == "1")
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                            if (System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"))
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                        }

                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UNLOCKED);
                    }
                    else
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_CHECKOUT);
                    }
                }
                //Questo if non è più valido perchè ora il pannello UpPnlDocumentData è sempre visibile
                //if (!UpPnlDocumentData.Visible)
                //if (fileReq.fileSize != "0")
                //{
                //    if ((UpPnlDocumentNotAcquired.Visible && !GrdDocumentAttached.Visible) || (UpPnlDocumentAcquired.Visible))
                //    {
                //        UpPnlDocumentAcquired.Visible = true;
                //        ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile", UIManager.UserManager.GetUserLanguage()) + " V" + fileReq.version;
                //        ViewDocumentImageDocumentAcquired.Src = GetVersionImage(fileReq, true);
                //        (FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                //        (FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                //        FindControl("divFrame").Visible = false;
                //        UpPnlDocumentData.Visible = true;
                //    }
                //    else
                //    {
                //        (FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                //        FindControl("divFrame").Visible = true;
                //        SaveFileDocument(fileReq);
                //        frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";
                //        if ((PageCaller.Equals(CALLER_ATTACHMENT) && GrdDocumentAttached.Rows.Count > 0) || !PageCaller.Equals(CALLER_ATTACHMENT))
                //        {
                //            BindDocumentAttached();
                //            GrdDocumentAttached.Visible = true;
                //            UpPnlDocumentAcquired.Visible = false;
                //            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).Enabled = true;
                //            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).ImageUrl = GetVersionImage(fileReq);
                //            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).OnMouseOverImage = GetVersionImage(fileReq);
                //            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).OnMouseOutImage = GetVersionImage(fileReq);
                //            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).ImageUrlDisabled = GetVersionImage(fileReq);
                //            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVersioneStampabile") as CustomImageButton).Visible = true;
                //            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVersioneStampabile") as CustomImageButton).ImageUrl = ResolveUrl("~/Images/Icons/print_label.png");
                //            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVersioneStampabile") as CustomImageButton).OnMouseOverImage = ResolveUrl("~/Images/Icons/print_label_hover.png");
                //            ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVersioneStampabile") as CustomImageButton).OnMouseOutImage = ResolveUrl("~/Images/Icons/print_label.png");
                //        }
                //        UpPnlContentDxDx.Update();
                //        UpBottomButtons.Update();
                //    }
                //    UpPnlDocumentNotAcquired.Visible = false;
                //}
                LitDocumentBlocked.Text = str_locked;
                UpBottomButtons.Update();
            }

            // UpPnlContentDxSx.Update();

            ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
        }

        /// <summary>
        /// Aggiorna il visualizzatore, la griglia degli allegati e le versioni associate all'allegato o documento principale selezionato
        /// </summary>
        /// <param name="id"></param>
        public void ShowDocumentAcquired(bool autoPreview, bool closeZoom = false)
        {
            SchedaDocumento tabDocument = DocumentManager.getSelectedRecord();
            UpPnlDocumentNotAcquired.Visible = true;
            UpPnlDocumentAcquired.Visible = false;
            UpPnlDocumentData.Visible = false;
            GrdDocumentAttached.Visible = false;
            GridAttachNoUser.Visible = false;
            string idAmm = UserManager.GetInfoUser().idAmministrazione;
            FileRequest attach = null;
            if (!closeZoom || DocumentManager.IsNewDocument())
            {
                attach = (DocumentManager.getSelectedAttachId() != null) ?
                    FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) :
                        FileManager.GetFileRequest();
            }
            else
            {
                attach = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
            }
            if (attach != null)
            {
                bool isLastVersion = false;
                double size = 0;
                if (!string.IsNullOrEmpty(attach.fileSize))
                    size = (double)int.Parse(attach.fileSize) / 1024;

                if (size > 0)
                {
                    FileRequest fileReq = new FileRequest();
                    FindControl("divFrame").Visible = true;
                    UpPnlDocumentNotAcquired.Visible = false;
                    if (!DocumentManager.IsNewDocument())
                    {
                        DocumentManager.ListDocVersions = DocumentManager.getDocumentListVersions(Page, attach.docNumber, attach.docNumber).documenti;
                        fileReq = (from v in DocumentManager.ListDocVersions where v.version.Equals(attach.version) select v).FirstOrDefault();
                        //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                        /* se attiva autopreview
                            * 1-  aggiorna la grid view con gli allegati;
                            * 2- aggiorna le versioni associate all'allegato/documento principale correntemente selezionato
                            * 3- visualizza l'ultima versione associata al doc principale/ allegato selezionato
                            * */
                        if (autoPreview)
                        {
                            UpPnlDocumentData.Visible = true;
                            GrdDocumentAttached.Visible = true;
                            if (!IsZoom || (PageCaller != CALLER_SMISTAMENTO))
                                GridAttachNoUser.Visible = true;
                            BottomButtons.Visible = true;
                            if (!closeZoom)
                            {
                                DocumentManager.setSelectedNumberVersion("0");
                                BindVersions(attach.versionId);
                            }
                            else
                            {
                                string versionId = string.Empty;
                                if (DocumentManager.getSelectedAttachId() != null) //nel caso di allegato prendo il versionid dell'ultima versione
                                {
                                    versionId = DocumentManager.getSelectedAttachId();
                                }
                                else
                                {
                                    versionId = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v.versionId).FirstOrDefault();
                                }
                                BindVersions(versionId);
                            }
                            BindDocumentAttached();
                            SchedaDocumento doc = DocumentManager.getSelectedRecord();
                            //La segnatura è visibile solo se siamo nel caso di ultima versione di documento
                            string version = (from document in DocumentManager.ListDocVersions select document.version).Max();
                            isLastVersion = DocumentManager.getSelectedNumberVersion().Equals(version);
                            SaveFileDocument(attach);
                            frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";
                        }
                        else
                        {
                            UpPnlDocumentAcquired.Visible = true;
                            ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile", UIManager.UserManager.GetUserLanguage()) + " V" + attach.version;
                            ViewDocumentImageDocumentAcquired.Src = GetVersionImage(attach, true);
                            //Aggiungo questa parte di codice per visualizzare sempre il pannello delle versioni in basso
                            UpPnlDocumentData.Visible = true;
                            //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                            FindControl("divFrame").Visible = false;
                            DocumentManager.setSelectedNumberVersion(attach.version);
                            //BindVersions(attach.version);
                            //UpPnlDocumentData.Visible = false;
                            //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                            GrdDocumentAttached.Visible = false;
                            GridAttachNoUser.Visible = false;

                            string version = (from document in DocumentManager.ListDocVersions select document.version).Max();
                            isLastVersion = DocumentManager.getSelectedNumberVersion().Equals(version);
                        }
                    }
                    else
                    {
                        if (autoPreview)
                        {
                            UpPnlDocumentData.Visible = true;
                            GrdDocumentAttached.Visible = true;
                            if (!IsZoom || (PageCaller != CALLER_SMISTAMENTO))
                                GridAttachNoUser.Visible = true;
                            BottomButtons.Visible = true;
                            if (!closeZoom)
                            {
                                DocumentManager.setSelectedNumberVersion("0");
                                BindVersions(attach.versionId);
                            }
                            else
                            {
                                string versionId = string.Empty;
                                if (DocumentManager.getSelectedAttachId() != null) //nel caso di allegato prendo il versionid dell'ultima versione
                                {
                                    versionId = DocumentManager.getSelectedAttachId();
                                }
                                else
                                {
                                    versionId = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v.versionId).FirstOrDefault();
                                }
                                BindVersions(versionId);
                            }
                            BindDocumentAttached();
                            SchedaDocumento doc = DocumentManager.getSelectedRecord();
                            SaveFileDocument(attach);
                            frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";
                        }
                        else
                        {
                            UpPnlDocumentAcquired.Visible = true;
                            ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile", UIManager.UserManager.GetUserLanguage()) + " V" + attach.version;
                            ViewDocumentImageDocumentAcquired.Src = GetVersionImage(attach, true);
                            //Aggiungo questa parte di codice per visualizzare sempre il pannello delle versioni in basso
                            UpPnlDocumentData.Visible = true;
                            //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                            FindControl("divFrame").Visible = false;
                            DocumentManager.setSelectedNumberVersion(attach.version);
                            //BindVersions(attach.version);
                            //UpPnlDocumentData.Visible = false;
                            //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                            GrdDocumentAttached.Visible = false;
                            GridAttachNoUser.Visible = false;
                        }
                    }
                    if ((DocumentButtons)Parent.FindControl("DocumentButtons") != null)
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ALL);
                        DocumentImgIdentityCard.Enabled = true;
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UPLOADFILE);
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                        if ((PageCaller.Equals(CALLER_ATTACHMENT) && !IsAcquired(FileManager.GetFileRequest(VersionIdAttachSelected))) || ((PageCaller.Equals(CALLER_DOCUMENT) || PageCaller.Equals(CALLER_CLASSIFICATIONS)) && !IsAcquired(FileManager.GetFileRequest())))
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ZOOM);
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_VIEWFILE);
                        }
                        if (DisableSignature(attach) || !isLastVersion)
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_POSITIONSIGNATURE);
                        if (IsVisibleButtonIdentityCard(fileReq))
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.V_IDENTITYCARD);
                            DocumentImgIdentityCard.Visible = true;
                        }
                        else
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                            DocumentImgIdentityCard.Visible = false;
                        }
                        if (fileReq.firmato == "1")
                        {
                            string tipoFirma = signed;
                            if (!string.IsNullOrEmpty(fileReq.tipoFirma))
                                tipoFirma = fileReq.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                            LitDocumentSignature.Text = tipoFirma;
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                            if (System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"))
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                        }
                        else
                        {
                            LitDocumentSignature.Text = "";
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                        }
                        if (DocumentManager.IsNewDocument())
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).DisableButtonImageSign();

                    
                        // per demo emea
                        //  if (!FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToLowerInvariant().Equals(PDF))
                        //    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ELECTRONIC_SIGN);

                        //disabilito i pulsanti firma.
                        if (!autoPreview)
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).DisableButtonImageSign();
                    }
                }
                else
                {
                    //Abbatangeli - In test...
                    BottomButtons.Visible = false;
                    //Fine

                    if (!DocumentManager.IsNewDocument() && DocumentManager.getDocumentListVersions(Page, attach.docNumber, attach.docNumber).documenti.Length > 1)
                    {
                        UpPnlDocumentData.Visible = true;
                        UpPnlDocumentAcquired.Visible = false;
                        GrdDocumentAttached.Visible = true;
                        GridAttachNoUser.Visible = true;
                        BindVersions(attach.versionId);
                        BindDocumentAttached();
                        //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                        FindControl("divFrame").Visible = false;
                    }
                    else
                    {
                        UpPnlDocumentNotAcquired.Visible = true;
                        UpPnlDocumentAcquired.Visible = false;
                    }
                    if ((DocumentButtons)Parent.FindControl("DocumentButtons") != null)
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                        //se il documento non è in uno stato finale oppure è in stato finale
                        //ma è stato effetuato lo sblocco stato per il ruolo corrente 
                        //allora abilito il pulsante di acquisiz.
                        if (!DiagrammiManager.IsDocumentInFinalState() ||
                            Convert.ToInt32(DocumentManager.GetAccessRightDocByDocument(
                            DocumentManager.getSelectedRecord(), UserManager.GetInfoUser())) > 45)
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_UPLOADFILE);
                        }
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                        LitDocumentSignature.Text = string.Empty;
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                        DocumentImgIdentityCard.Visible = false;
                    }
                }
            }
            UpPnlContentDxSx.Update();
            UpPnlContentDxDx.Update();
            UpBottomButtons.Update();
            BuildStringAuthorFile(attach);
            UpdateInfoFileAquired.Update();
            if (!IsZoom && PageCaller != CALLER_SMISTAMENTO && this.PageCaller != ESAMINA_UNO_A_UNO)
                ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframe();", true);
            else
            {
                if ((PageCaller == CALLER_SMISTAMENTO || this.PageCaller == ESAMINA_UNO_A_UNO) && !IsZoom && !CallFromSignDetails)
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframeViewerSmistamento();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframeViewer();", true);
                }
            }
            ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
            if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
            {
                ButtonsManager();

                if (EnabledLibroFirma)
                    ButtonsManagerLibroFirma();
            }

            if (this.PageCaller == ESAMINA_UNO_A_UNO)
            {
                EsaminaLibroFirma page = this.Page as EsaminaLibroFirma;
                page.EnabledButtonEsamina(IsElementoSelezionatoInLf(attach));
                this.PlcVersions.Visible = false;
            }
        }

        public void LockDocumentInLF(bool isTitolare)
        {
            if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
            {
                if (isTitolare)
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_LIBROFIRMA_UNLOCK);
                else
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_LIBROFIRMA_LOCK);

                UpBottomButtons.Update();
            }
        }

        public void UpdateProcessLFInAction()
        {
            FileRequest fileDoc = new FileRequest();
            if (DocumentManager.IsNewDocument())
            {
                fileDoc = (DocumentManager.getSelectedAttachId() != null) ?
                    FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) :
                        FileManager.GetFileRequest();
            }
            else
            {
                //fileDoc = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
                if (UIManager.DocumentManager.getSelectedAttachId() != null)
                {
                    fileDoc = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
                }
                else
                {
                    fileDoc = FileManager.GetFileRequest();
                }
                if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                {
                    if (fileDoc.version != DocumentManager.getSelectedNumberVersion())
                        fileDoc = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
                }
            }
            if (fileDoc != null)
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframe();", true);

                //Nel caso in cui il documento non è più i libro firma, riabilito tutti i bottoni di firma
                if (!fileDoc.inLibroFirma)
                {
                    if (fileDoc.firmato == "1")
                    {
                        string tipoFirma = signed;
                        if (!string.IsNullOrEmpty(fileDoc.tipoFirma))
                            tipoFirma = fileDoc.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                        LitDocumentSignature.Text = tipoFirma;
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                        if (System.IO.Path.GetExtension(fileDoc.fileName).ToLower().Equals(".pdf"))
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                    }
                    else
                    {
                        LitDocumentSignature.Text = string.Empty;
                    }
                    //Se il documento è visualizzato abilito i bottoni di firma
                    if (FindControl("divFrame").Visible)
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_LIBROFIRMA_UNLOCK);

                        if (!string.IsNullOrEmpty(fileDoc.fileSize) && Convert.ToUInt32(fileDoc.fileSize) > 0)
                        {
                            string extensionFile = (fileDoc.fileName.Split('.').Length > 1) ? (fileDoc.fileName.Split('.'))[fileDoc.fileName.Split('.').Length - 1] : string.Empty;
                            if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null && !verifyExtensionForSign(extensionFile))
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).DisableButtonImageSign();
                           //per demo emea
                            //if (!FileManager.getEstensioneIntoSignedFile(fileDoc.fileName).ToLowerInvariant().Equals(PDF))
                            //    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ELECTRONIC_SIGN);

                        }
                    }
                    else
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).DisableButtonImageSign();
                    }
                }
                else
                {
                    if(fileDoc.firmato.Equals("1"))
                    {
                        string tipoFirma = signed;
                        if (!string.IsNullOrEmpty(fileDoc.tipoFirma))
                            tipoFirma = fileDoc.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                        LitDocumentSignature.Text = tipoFirma + " - " + lock_InLibroFirma;
                    }
                    else
                    {
                        LitDocumentSignature.Text = lock_InLibroFirma;
                    }
                }
                ButtonsManager();

                if (EnabledLibroFirma)
                    ButtonsManagerLibroFirma();

                ButtonsVersionManager(fileDoc);
                UpPnlContentDxSx.Update();
                UpBottomButtons.Update();
            }
        }

        public void UpdateSignedFile()
        {
            FileRequest fileDoc = new FileRequest();
            if (DocumentManager.IsNewDocument())
            {
                fileDoc = (DocumentManager.getSelectedAttachId() != null) ?
                    FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) :
                        FileManager.GetFileRequest();
            }
            else
            {
                //fileDoc = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
                if (UIManager.DocumentManager.getSelectedAttachId() != null)
                {
                    fileDoc = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
                }
                else
                {
                    fileDoc = FileManager.GetFileRequest();
                }
                if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                {
                    if (fileDoc.version != DocumentManager.getSelectedNumberVersion())
                        fileDoc = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
                }
            }

            if (fileDoc != null && !string.IsNullOrEmpty(fileDoc.fileSize) && Convert.ToUInt32(fileDoc.fileSize) > 0)
            {
                VersionIdAttachSelected = fileDoc.versionId;

                UpPnlDocumentNotAcquired.Visible = false;
                UpPnlDocumentAcquired.Visible = true;
                if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                {
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ALL);
                    DocumentImgIdentityCard.Enabled = true;
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UPLOADFILE);
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                    if (DisableSignature(fileDoc))
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_POSITIONSIGNATURE);
                    if (IsVisibleButtonIdentityCard(fileDoc))
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.V_IDENTITYCARD);
                        DocumentImgIdentityCard.Visible = true;
                    }
                    else
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                        DocumentImgIdentityCard.Visible = false;
                    }
                    if (fileDoc.firmato == "1")
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                        if (System.IO.Path.GetExtension(fileDoc.fileName).ToLower().Equals(".pdf"))
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                        string tipoFirma = signed;
                        if (!string.IsNullOrEmpty(fileDoc.tipoFirma))
                            tipoFirma = fileDoc.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                        LitDocumentSignature.Text = tipoFirma;
                    }
                }
                ViewDocumentLinkFile.Text = Utils.Languages.GetLabelFromCode("ViewDocumentLinkFile", UIManager.UserManager.GetUserLanguage()) + " V" + fileDoc.version;
                ViewDocumentImageDocumentAcquired.Src = GetVersionImage(fileDoc, true);
                LitDocumentSize.Text = Convert.ToString(Math.Round((double)Int32.Parse(fileDoc.fileSize) / 1024, 3)) + " Kb";
                DocumentManager.setSelectedNumberVersion(fileDoc.version);
                BindVersions(fileDoc.versionId);
                UpdateTooltipInfoVersionLight(fileDoc);
                UpBottomButtons.Update();
                UpPnlContentDxSx.Update();
            }
        }

        public void ShowChangeVersions()
        {
            double fileSize = 0;
            FileRequest fileReq = (DocumentManager.getSelectedAttachId() != null) ?
                FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) :
                FileManager.GetFileRequest();
            if (fileReq.GetType() == typeof(Allegato))
            {
                if ((from att in DocumentManager.getSelectedRecord().allegati where att.versionId.Equals(VersionIdAttachSelected) select att).Count() == 0)
                    VersionIdAttachSelected = fileReq.versionId;
            }
            if (!string.IsNullOrEmpty(fileReq.fileSize))
                fileSize = (double)int.Parse(fileReq.fileSize) / 1024;
            BindVersions(fileReq.versionId);
            if (fileSize == 0)
            {
                UpPnlDocumentNotAcquired.Visible = true;
                UpPnlDocumentAcquired.Visible = false;
                GrdDocumentAttached.Visible = true;
                GridAttachNoUser.Visible = true;
                UpPnlDocumentData.Visible = true;
                //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                FindControl("divFrame").Visible = false;
                ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframe();", true);
                ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
                if (GrdDocumentAttached != null && GrdDocumentAttached.Rows.Count > 0)
                {
                    ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("lblDescrizione") as Label).Text = GetLabel(fileReq);
                    ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).Enabled = false;
                    ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).ImageUrl = GetVersionImage(fileReq);
                    ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).OnMouseOverImage = GetVersionImage(fileReq);
                    ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).OnMouseOutImage = GetVersionImage(fileReq);
                    ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).ImageUrlDisabled = GetVersionImage(fileReq);
                    ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVersioneStampabile") as HyperLink).Visible = false;
                }
                else
                {
                    GrdDocumentAttached.Visible = true;
                    GridAttachNoUser.Visible = true;
                    BindDocumentAttached();
                }
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                LitDocumentSignature.Text = string.Empty;
                //se il documento non è in uno stato finale oppure è in stato finale
                //ma è stato effetuato lo sblocco stato per il ruolo corrente 
                //allora abilito il pulsante di acquisiz.
                if (!DiagrammiManager.IsDocumentInFinalState() ||
                    Convert.ToInt32(DocumentManager.GetAccessRightDocByDocument(
                    DocumentManager.getSelectedRecord(), UserManager.GetInfoUser())) > 45)
                {
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_UPLOADFILE);
                }
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                DocumentImgIdentityCard.Visible = false;
            }
            else
            {
                UpPnlDocumentNotAcquired.Visible = false;
                UpPnlDocumentAcquired.Visible = false;
                UpPnlDocumentData.Visible = true;
                //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                FindControl("divFrame").Visible = true;
                ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframe();", true);
                SaveFileDocument(fileReq);
                ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
                if (!GrdDocumentAttached.Visible || (GrdDocumentAttached != null && GrdDocumentAttached.Rows.Count > 0 && GrdDocumentAttached.SelectedIndex == -1))
                {
                    BindDocumentAttached();
                }

                GrdDocumentAttached.Visible = true;
                GridAttachNoUser.Visible = true;

                ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("lblDescrizione") as Label).Text = GetLabel(fileReq);
                ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).Enabled = true;
                ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).ImageUrl = GetVersionImage(fileReq);
                ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).OnMouseOverImage = GetVersionImage(fileReq);
                ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).OnMouseOutImage = GetVersionImage(fileReq);
                ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).ImageUrlDisabled = GetVersionImage(fileReq);
                ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVersioneStampabile") as HyperLink).Visible = true;
                //string attachVersionId = ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("attachVersionId") as HiddenField).Value;
                //string linkVersion = ResolveUrl("~/Document/AttachmentViewer.aspx?versionDownload=") + attachVersionId;
                //((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVersioneStampabile") as HyperLink).NavigateUrl = linkVersion;
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ALL);
                DocumentImgIdentityCard.Enabled = true;
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UPLOADFILE);
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_POSITIONSIGNATURE);
                if (IsVisibleButtonIdentityCard(fileReq))
                {
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.V_IDENTITYCARD);
                    DocumentImgIdentityCard.Visible = true;
                }
                else
                {
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                    DocumentImgIdentityCard.Visible = false;
                }
                ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();

                if (fileReq.firmato == "1")
                {
                    string tipoFirma = signed;
                    if (!string.IsNullOrEmpty(fileReq.tipoFirma))
                        tipoFirma = fileReq.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                    LitDocumentSignature.Text = tipoFirma;
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                    if (System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"))
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                }
                else
                {
                    LitDocumentSignature.Text = string.Empty;
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);

                }

            }
            ButtonsManager();
            if (EnabledLibroFirma)
                ButtonsManagerLibroFirma();
            UpPnlContentDxSx.Update();
            UpPnlContentDxDx.Update();
            UpBottomButtons.Update();
            ScriptManager.RegisterStartupScript(Page, GetType(), "function", "<script>reallowOp();</script>", false);

        }

        /// <summary>
        /// handles the click on document/attachment to display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LinkViewFileDocument(object sender, EventArgs e)
        {
            //PDZ 
            //Estratto il metodo perchè lo devo richiamare per l'anteprima dei PDF
            this.ViewFileDoc();
        }
        //PDZ
        //fromPageForPreview parametro che indica nel caso di previewPDF la pagina da cui deve partire la prossima anteprima
        //lastPageForPreview parametro che indica nel caso di previewPDF la pagina da cui deve partire la precedente anteprima
        private void ViewFileDoc(bool isPreview = true, int fromPageForPreview = 0, int lastPageForPreview = 0)
        {
            FileRequest file = null;
            SchedaDocumento sch = DocumentManager.getSelectedRecord();
                if (!string.IsNullOrEmpty(this.PageCaller) && this.PageCaller == CALLER_SMISTAMENTO && !this.IsZoom && !CallFromSignDetails)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                }
                if (this.PageCaller.ToUpper().Equals(CALLER_ATTACHMENT) || UIManager.DocumentManager.getSelectedAttachId() != null)
                {
                    //Quando sono nel tab allegati e seleziono Visualizza file reimposto il SelectedAttachId uguale a quello selezionato nella griglia di destra.
                    //if(IsZoom)
                    DocumentManager.setSelectedAttachId(VersionIdAttachSelected);
                    file = FileManager.GetSelectedAttachment();
                }

                if (file == null)
                {
                    //Aggiungo la riga seguente per far si che nel tab documenti, quando clicco sul bottone visualizza
                    //quando ho selezionato un allegato, mi ritorna al documento principale  
                    if(this.PageCaller != ESAMINA_UNO_A_UNO)
                        DocumentManager.RemoveSelectedAttachId();
                    file = UIManager.FileManager.getSelectedFile();
                }
                //(this.FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                this.FindControl("divFrame").Visible = true;
                SaveFileDocument(file, false, isPreview, fromPageForPreview, lastPageForPreview);

                this.frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";
                this.UpPnlDocumentData.Visible = true;
                //Se lo user control è richiamato tramite il bottone "Zoom" disabilito
                if (IsZoom || this.PageCaller == CALLER_SMISTAMENTO)
                {
                    (this.FindControl("divNumberVersion") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                }

                if ((this.PageCaller == CALLER_SMISTAMENTO && !IsZoom && !CallFromSignDetails))
                {
                    ((SmistamentoDocumenti)this.Page).RefreshZoom();
                }
                this.GrdDocumentAttached.Visible = true;
                if (!IsZoom || this.PageCaller != CALLER_SMISTAMENTO)
                    this.GridAttachNoUser.Visible = true;
                else
                    this.GridAttachNoUser.Visible = false;
                this.UpPnlDocumentAcquired.Visible = false;
                this.UpPnlDocumentNotAcquired.Visible = false;
                this.BottomButtons.Visible = true;
                if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                {
                    bool bool_locked = false;

                    if (UIManager.DocumentManager.getSelectedAttachId() == null)
                    {
                        SchedaDocumento selectedRecord = DocumentManager.getSelectedRecord();
                        bool_locked = (selectedRecord != null ? (selectedRecord.checkOutStatus != null && !string.IsNullOrEmpty(selectedRecord.checkOutStatus.ID)) : false);
                    }
                    else
                    {
                        bool_locked = (DocumentManager.GetSelectedAttachment() != null ? (DocumentManager.GetCheckOutDocumentStatus(DocumentManager.GetSelectedAttachment().docNumber) != null) : false);
                    }

                    if (!bool_locked)
                    {
                        DocumentButtons documentButtons = (DocumentButtons)Parent.FindControl("DocumentButtons");
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ALL);
                        if (file.firmato == "1")
                        {
                            string tipoFirma = signed;
                            if (!string.IsNullOrEmpty(file.tipoFirma))
                                tipoFirma = file.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                            LitDocumentSignature.Text = tipoFirma;
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                            if (System.IO.Path.GetExtension(file.fileName).ToLower().Equals(".pdf"))
                                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                        }
                        else
                        {
                            this.LitDocumentSignature.Text = "";
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                        }
                        this.DocumentImgIdentityCard.Enabled = true;
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UPLOADFILE);
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                        if (DisableSignature(file))
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_POSITIONSIGNATURE);
                        if (IsVisibleButtonIdentityCard(file))
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.V_IDENTITYCARD);
                            this.DocumentImgIdentityCard.Visible = true;
                        }
                        else
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                            this.DocumentImgIdentityCard.Visible = false;
                        }
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UNLOCKED);
                    }
                    else
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_CHECKOUT);
                    }
                }
                this.UpdatePanelPreview.Update();
                this.UpBottomButtons.Update();
                this.UpPnlContentDxSx.Update();
                this.UpPnlContentDxDx.Update();
                if (!IsZoom && this.PageCaller != CALLER_SMISTAMENTO && this.PageCaller != ESAMINA_UNO_A_UNO)
                    if (!this.CallFromSignDetails)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeFrame", "resizeIframe();", true);
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeFrame", "resizeIframeViewerSign();", true);
                else
                {
                    if (this.PageCaller == CALLER_SMISTAMENTO && !IsZoom)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeFrame", "resizeIframeViewerSmistamento();", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeFrame", "resizeIframeViewer();", true);
                    }
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "tipsy", "tooltipTipsy();", true);
                DocumentManager.setSelectedNumberVersion("0");
                this.BindVersions(file.versionId);
                this.BindDocumentAttached();
                if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                {
                    ButtonsManager();
                    if (this.EnabledLibroFirma)
                        ButtonsManagerLibroFirma();
                }

                //skizzo
                //documento visualizzato, abilito i pulsanti per la firma.
                if (file != null && !string.IsNullOrEmpty(file.fileSize) && Convert.ToUInt32(file.fileSize) > 0)
                {
                    string extensionFile = (file.fileName.Split('.').Length > 1) ? (file.fileName.Split('.'))[file.fileName.Split('.').Length - 1] : string.Empty;
                    if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null && !verifyExtensionForSign(extensionFile))
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).DisableButtonImageSign();
                    //if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null && !FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(PDF))
                    //    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ELECTRONIC_SIGN);
                }
            if (this.PageCaller == ESAMINA_UNO_A_UNO)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                //this.GridAttachNoUser.Visible = false;
                this.PlcVersions.Visible = false;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeFrame", "resizeIframeViewerSmistamento();", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "tipsy", "tooltipTipsy();", true);
                this.UpdatePanelPreview.Update();
                this.UpBottomButtons.Update();
                this.UpPnlContentDxSx.Update();
                this.UpPnlContentDxDx.Update();
            }

            //Se Anteprima abilitata e pdf, nascondo il pulsante visualizza documento
            if (IsPreview(file) && isPreview)
            {                      
                ((GrdDocumentAttached.Rows[GrdDocumentAttached.SelectedIndex] as GridViewRow).FindControl("btnVisualizza") as CustomImageButton).Visible = false;
            }
        }

        /// <summary>
        /// handles the click on document/attachment to display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public void LinkViewFileDocument(object sender, EventArgs e)
        //{
        //    FileRequest file = null;
        //    SchedaDocumento sch = DocumentManager.getSelectedRecord();
        //    if (PageCaller != ESAMINA_UNO_A_UNO)
        //    {
        //        if (!string.IsNullOrEmpty(PageCaller) && PageCaller == CALLER_SMISTAMENTO && !IsZoom && !CallFromSignDetails)
        //        {
        //            ScriptManager.RegisterStartupScript(Page, GetType(), "function", "reallowOp();", true);
        //        }
        //        if (PageCaller.ToUpper().Equals(CALLER_ATTACHMENT))
        //        {
        //            //Quando sono nel tab allegati e seleziono Visualizza file reimposto il SelectedAttachId uguale a quello selezionato nella griglia di destra.
        //            //if(IsZoom)
        //            DocumentManager.setSelectedAttachId(VersionIdAttachSelected);
        //            file = FileManager.GetSelectedAttachment();
        //        }

        //        if (file == null)
        //        {
        //            //Aggiungo la riga seguente per far si che nel tab documenti, quando clicco sul bottone visualizza
        //            //quando ho selezionato un allegato, mi ritorna al documento principale  
        //            DocumentManager.RemoveSelectedAttachId();
        //            file = UIManager.FileManager.getSelectedFile();
        //        }
        //        //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
        //        FindControl("divFrame").Visible = true;
        //        SaveFileDocument(file);
        //        frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";
        //        UpPnlDocumentData.Visible = true;
        //        //Se lo user control è richiamato tramite il bottone "Zoom" disabilito
        //        if (IsZoom || PageCaller == CALLER_SMISTAMENTO)
        //        {
        //            (FindControl("divNumberVersion") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
        //        }

        //        if ((PageCaller == CALLER_SMISTAMENTO && !IsZoom && !CallFromSignDetails))
        //        {
        //            ((SmistamentoDocumenti)Page).RefreshZoom();
        //        }
        //        GrdDocumentAttached.Visible = true;
        //        if (!IsZoom || PageCaller != CALLER_SMISTAMENTO)
        //            GridAttachNoUser.Visible = true;
        //        else
        //            GridAttachNoUser.Visible = false;
        //        UpPnlDocumentAcquired.Visible = false;
        //        UpPnlDocumentNotAcquired.Visible = false;
        //        BottomButtons.Visible = true;
        //        if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
        //        {
        //            bool bool_locked = false;

        //            if (UIManager.DocumentManager.getSelectedAttachId() == null)
        //            {
        //                SchedaDocumento selectedRecord = DocumentManager.getSelectedRecord();
        //                bool_locked = (selectedRecord != null ? (selectedRecord.checkOutStatus != null && !string.IsNullOrEmpty(selectedRecord.checkOutStatus.ID)) : false);
        //            }
        //            else
        //            {
        //                bool_locked = (DocumentManager.GetSelectedAttachment() != null ? (DocumentManager.GetCheckOutDocumentStatus(DocumentManager.GetSelectedAttachment().docNumber) != null) : false);
        //            }

        //            if (!bool_locked)
        //            {
        //                DocumentButtons documentButtons = (DocumentButtons)Parent.FindControl("DocumentButtons");
        //                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ALL);
        //                if (file.firmato == "1")
        //                {
        //                    string tipoFirma = signed;
        //                    if (!string.IsNullOrEmpty(file.tipoFirma))
        //                        tipoFirma = file.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
        //                    LitDocumentSignature.Text = tipoFirma;
        //                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
        //                    if (System.IO.Path.GetExtension(file.fileName).ToLower().Equals(".pdf"))
        //                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
        //                }
        //                else
        //                {
        //                    LitDocumentSignature.Text = "";
        //                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
        //                }
        //                DocumentImgIdentityCard.Enabled = true;
        //                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UPLOADFILE);
        //                ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
        //                if (DisableSignature(file))
        //                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_POSITIONSIGNATURE);
        //                if (IsVisibleButtonIdentityCard(file))
        //                {
        //                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.V_IDENTITYCARD);
        //                    DocumentImgIdentityCard.Visible = true;
        //                }
        //                else
        //                {
        //                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
        //                    DocumentImgIdentityCard.Visible = false;
        //                }
        //                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UNLOCKED);
        //            }
        //            else
        //            {
        //                ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_CHECKOUT);
        //            }
        //        }
        //        UpBottomButtons.Update();
        //        UpPnlContentDxSx.Update();
        //        UpPnlContentDxDx.Update();
        //        if (!IsZoom && PageCaller != CALLER_SMISTAMENTO)
        //            if (!CallFromSignDetails)
        //                ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframe();", true);
        //            else
        //                ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframeViewerSign();", true);
        //        else
        //        {
        //            if (PageCaller == CALLER_SMISTAMENTO && !IsZoom && !CallFromSignDetails)
        //            {
        //                ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframeViewerSmistamento();", true);
        //            }
        //            else
        //            {
        //                ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframeViewer();", true);
        //            }
        //        }
        //        ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
        //        DocumentManager.setSelectedNumberVersion("0");
        //        BindVersions(file.versionId);
        //        BindDocumentAttached();
        //        if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
        //        {
        //            ButtonsManager();
        //            if (EnabledLibroFirma)
        //                ButtonsManagerLibroFirma();
        //        }

        //        //skizzo
        //        //documento visualizzato, abilito i pulsanti per la firma.
        //        if (file != null && !string.IsNullOrEmpty(file.fileSize) && Convert.ToUInt32(file.fileSize) > 0)
        //        {
        //            string extensionFile = (file.fileName.Split('.').Length > 1) ? (file.fileName.Split('.'))[file.fileName.Split('.').Length - 1] : string.Empty;
        //            if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null && !verifyExtensionForSign(extensionFile))
        //                ((DocumentButtons)Parent.FindControl("DocumentButtons")).DisableButtonImageSign();
        //        }
        //    }
        //    else
        //    {
        //        file = sch.documenti[0];
        //        if (file != null)
        //        {
        //            UpPnlContentDxDx.Attributes.Add("style", "display:none");
        //            UpPnlDocumentData.Visible = true;
        //            UpPnlDocumentAcquired.Visible = false;
        //            UpPnlDocumentNotAcquired.Visible = false;
        //            BottomButtons.Visible = false;
        //            GrdDocumentAttached.Attributes.Add("style", "display:none"); ;
        //            GridAttachNoUser.Attributes.Add("style", "display:none");
        //            PlcVersions.Visible = false;
        //            (FindControl("divNumberVersion") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
        //            FindControl("divFrame").Visible = true;
        //            SaveFileDocument(file);
        //            frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";
        //            ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframeViewerSmistamento();", true);
        //            ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
        //            UpBottomButtons.Update();
        //            UpPnlContentDxSx.Update();
        //            UpPnlContentDxDx.Update();
        //        }
        //    }
        //}

        /// <summary>
        /// Resetta e nasconde le informazioni sulla pulsantiera BottomButtons
        /// </summary>
        private void ResetBottomButtons(bool locked = false)
        {
            imgInfoVersionSelected.ToolTip = string.Empty;
            imgInfoVersionSelected.AlternateText = string.Empty;
            imgInfoVersionSelected.Visible = false;
            LitDocumentSize.Text = string.Empty;
            LitDocumentSignature.Text = string.Empty;
            LitDocumentBlocked.Text = (locked ? str_locked : string.Empty);
            //BottomButtons.Visible = false;
            UpBottomButtons.Update();
        }
        #endregion

        #region File Document Manager
        /// <summary>
        /// Salva nella property FileDoc il FileDocumento da visualizzare
        /// </summary>
        /// <param name="fileReq"></param>
        /// <param name="isShowPrintableVersion"></param>
        /// <returns></returns>
        private void SaveFileDocument(FileRequest fileReq, bool isShowPrintableVersion = false, bool isPreview = true, int fromPage = 0, int lastPage = 0)
        {
            SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
            if (DocToSign == null)
            {
                //if required viewing with signature
                if (DocumentManager.getSelectedRecord() != null && !DocumentManager.IsNewDocument() && !DisableSignature(fileReq) && !isShowPrintableVersion)
                {
                    if (IsPdf(fileReq) || IsVisibleSignature())
                    {
                        
                        if (IsPreview(fileReq) && isPreview)
                        {
                            FileDoc = DocumentAnteprima(fileReq, fromPage, lastPage);
                        }
                        else
                        {
                            PlaceHolderPreview.Visible = false;
                            this.UpdatePanelPreview.Update();
                            //Abbatangeli - Segnatura permanente
                            FileDoc = DocumentWithSignature(fileReq);
                            //FileDoc = FileManager.getInstance(documentTab.systemId).GetFile(this.Page, fileReq, ShowDocumentAsPdfFormat);
                        }
                    }
                    else
                    {
                        if (fileReq.fileName.EndsWith("." + EML))
                        {
                            FileDoc = FileManager.getInstance(documentTab.systemId).GetFileAsEML(this.Page, fileReq);
                        }
                        else
                        {
                            FileDoc = FileManager.getInstance(documentTab.systemId).GetFile(this.Page, fileReq, ShowDocumentAsPdfFormat);
                        }
                    }
                }
                else
                {
                    if (fileReq.fileName.EndsWith("." + EML))
                    {
                        FileDoc = FileManager.getInstance(documentTab.systemId).GetFileAsEML(this.Page, fileReq);
                    }
                    else
                    {
                        FileDoc = FileManager.getInstance(documentTab.systemId).GetFile(this.Page, fileReq, ShowDocumentAsPdfFormat);
                    }
                }
            }
            else
            {
                FileDoc = DocToSign;
            }

            //SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
            //if (DocToSign == null)
            //{
            //    //if required viewing with signature
            //    if (DocumentManager.getSelectedRecord() != null && !DocumentManager.IsNewDocument() && !DisableSignature(fileReq) && !isShowPrintableVersion)
            //    {
            //        if (IsPdf(fileReq) || IsVisibleSignature())
            //        {
            //            FileDoc = DocumentWithSignature(fileReq);
            //        }
            //        else
            //        {
            //            if (fileReq.fileName.EndsWith("." + EML))
            //            {
            //                FileDoc = FileManager.getInstance(documentTab.systemId).GetFileAsEML(Page, fileReq);
            //            }
            //            else
            //            {
            //                FileDoc = FileManager.getInstance(documentTab.systemId).GetFile(Page, fileReq, ShowDocumentAsPdfFormat);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (fileReq.fileName.EndsWith("." + EML))
            //        {
            //            FileDoc = FileManager.getInstance(documentTab.systemId).GetFileAsEML(Page, fileReq);
            //        }
            //        else
            //        {
            //            FileDoc = FileManager.getInstance(documentTab.systemId).GetFile(Page, fileReq, ShowDocumentAsPdfFormat);
            //        }
            //    }
            //}
            //else
            //{
            //    FileDoc = DocToSign;
            //}
        }

        /// <summary>
        /// Returns the document / attachment with signature in the case of pdf file
        /// </summary>
        /// <param name="fileReq"></param>
        /// <returns></returns>
        private FileDocumento DocumentWithSignature(FileRequest fileReq)
        {
            SchedaDocumento sch = null;
            if (FileManager.GetSelectedAttachment() == null)
                sch = DocumentManager.getSelectedRecord();
            else if (DocumentManager.getSelectedRecord().documentoPrincipale == null)
            {
                sch = DocumentManager.getDocumentListVersions(Page, fileReq.docNumber, fileReq.docNumber);
            }
            Response.Expires = -1;
            DocsPaWR.InfoUtente infoUser = UserManager.GetInfoUser();
            DocsPaWR.labelPdf label = new DocsPaWR.labelPdf();
            //load data in the object Label
            label.position = PositionSignature;
            label.tipoLabel = TypeLabel;
            label.label_rotation = RotationSignature;
            label.sel_font = CharacterSignature;
            label.sel_color = ColorSignature;
            label.orientamento = OrientationSignature;
            if (fileReq.firmato == "1" || NoTimbro)
                label.notimbro = NoTimbro;
            if (PrintOnFirstPage || PrintOnLastPage)
            {
                label.digitalSignInfo = new labelPdfDigitalSignInfo();
                label.digitalSignInfo.printOnFirstPage = PrintOnFirstPage;
                label.digitalSignInfo.printOnLastPage = PrintOnLastPage;
                if (fileReq.firmato == "1" || NoTimbro)
                    label.digitalSignInfo.printFormatSign = FormatSignature;
            }
            DocsPaWR.SchedaDocumento schedaCorrente = NttDataWA.UIManager.DocumentManager.getSelectedRecord();
            FileDocumento theDoc;
            if (!string.IsNullOrEmpty(NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_RENDER_PDF")) &&
                NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_RENDER_PDF").Equals("1") && !IsPdf(fileReq))
            {
                try
                {
                    theDoc = FileManager.getInstance(sch.systemId).DocumentoGetFileConSegnaturaUsingLC(Page, sch, label, fileReq);
                    if (theDoc == null)
                    {
                        if (fileReq.fileName.EndsWith("." + EML))
                        {
                            theDoc = FileManager.getInstance(sch.systemId).GetFileAsEML(Page, fileReq);
                        }
                        else
                        {
                            theDoc = FileManager.getInstance(sch.systemId).GetFile(Page, fileReq, ShowDocumentAsPdfFormat);
                            return theDoc;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    if (fileReq.fileName.EndsWith("." + EML))
                    {
                        theDoc = FileManager.getInstance(sch.systemId).GetFileAsEML(Page, fileReq);
                    }
                    else
                    {
                        theDoc = FileManager.getInstance(sch.systemId).GetFile(Page, fileReq, ShowDocumentAsPdfFormat);
                    }
                }
            }
            else
            {
                theDoc = FileManager.getInstance(sch.systemId).getFileConSegnatura(Page, sch, label, fileReq);
            }
            if (theDoc != null && theDoc.LabelPdf.default_position.Equals("pos_pers"))
                theDoc.LabelPdf.default_position = theDoc.LabelPdf.positions[4].PosX + "-" + theDoc.LabelPdf.positions[4].PosY;

            return theDoc;
        }

        /// <summary>
        /// Returns the document / attachment with signature in the case of pdf file
        /// </summary>
        /// <param name="fileReq"></param>
        /// <returns></returns>
        private FileDocumentoAnteprima DocumentAnteprima(FileRequest fileReq, int fromPage, int lastPage)
        {
            SchedaDocumento sch = null;
            if (FileManager.GetSelectedAttachment() == null)
                sch = DocumentManager.getSelectedRecord();
            else if (DocumentManager.getSelectedRecord().documentoPrincipale == null)
            {
                sch = DocumentManager.getDocumentListVersions(this.Page, fileReq.docNumber, fileReq.docNumber);
            }
            Response.Expires = -1;
            DocsPaWR.InfoUtente infoUser = UserManager.GetInfoUser();
            DocsPaWR.labelPdf label = new DocsPaWR.labelPdf();
            //load data in the object Label
            label.position = PositionSignature;
            label.tipoLabel = TypeLabel;
            label.label_rotation = RotationSignature;
            label.sel_font = CharacterSignature;
            label.sel_color = ColorSignature;
            label.orientamento = OrientationSignature;
            if (fileReq.firmato == "1" || NoTimbro)
                label.notimbro = NoTimbro;
            if (PrintOnFirstPage || PrintOnLastPage)
            {
                label.digitalSignInfo = new labelPdfDigitalSignInfo();
                label.digitalSignInfo.printOnFirstPage = PrintOnFirstPage;
                label.digitalSignInfo.printOnLastPage = PrintOnLastPage;
                if (fileReq.firmato == "1" || NoTimbro)
                    label.digitalSignInfo.printFormatSign = FormatSignature;
            }
            DocsPaWR.SchedaDocumento schedaCorrente = NttDataWA.UIManager.DocumentManager.getSelectedRecord();
            FileDocumentoAnteprima theDocA = null;
            if (IsPdf(fileReq))
            {
                try
                {
                    theDocA = FileManager.getInstance(sch.systemId).getPdfPreviewFile(this.Page, fileReq, fromPage, lastPage, sch, label);
                    if (theDocA != null)
                    {

                        if (!string.IsNullOrEmpty(NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_PREVIEW_PDF_UP_DOWN")) &&
                            NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_PREVIEW_PDF_UP_DOWN").Equals("1"))
                        {
                            //this.ViewNextPreview.Enabled = (theDocA.lastPg != theDocA.totalPg);
                            //this.ViewPrevPreview.Enabled = (theDocA.firstPg > 1);
                            //this.ViewNextPreview.Visible = true;
                            //this.ViewPrevPreview.Visible = true;
                            this.imgNextPreview.Enabled = (theDocA.lastPg != theDocA.totalPg);
                            this.imgPrevPreview.Enabled = (theDocA.firstPg > 1);
                            this.imgNextPreview.Visible = true;
                            this.imgPrevPreview.Visible = true;
                        }

                        //this.ViewFullDocPreview.Visible = UserManager.IsAuthorizedFunctions("PREVIEW_FULL_DOWNLOAD");

                        string language = UIManager.UserManager.GetUserLanguage();
                        this.ViewDocumentPreview.InnerText = Utils.Languages.GetLabelFromCode("ViewDocumentPreview", language) + " " + Utils.Languages.GetLabelFromCode("AttachmentsPage", language) + " " + theDocA.firstPg + "/" + theDocA.lastPg + " " +
                                         Utils.Languages.GetLabelFromCode("LblPaginationOfDx", language) + " " + theDocA.totalPg;

                        FromPagePreview = theDocA.firstPg;
                        LastPagePreview = theDocA.lastPg;
                        this.PlaceHolderPreview.Visible = true;
                        this.UpdatePanelPreview.Update();
                    }
                }
                catch (Exception ex) { }
            }
            //if (theDoc != null && theDoc.LabelPdf.default_position.Equals("pos_pers"))
            //    theDoc.LabelPdf.default_position = theDoc.LabelPdf.positions[4].PosX + "-" + theDoc.LabelPdf.positions[4].PosY;

            return theDocA;
        }
        #endregion

        #region management version
        /// <summary>
        /// 
        /// </summary>
        private void BindVersions(string versionId)
        {
            if (!IsForwarded)
            {
                FileRequest fileReq = FileManager.GetFileRequest(versionId);
                DocsPaWR.InfoUtente infoUser = UserManager.GetInfoUser();
                if (!DocumentManager.IsNewDocument())
                {

                    DocumentManager.ListDocVersions = DocumentManager.getDocumentListVersions(Page, fileReq.docNumber, fileReq.docNumber).documenti;
                    if (DocumentManager.ListDocVersions != null && DocumentManager.ListDocVersions.Length > 0)
                    {
                        if (DocumentManager.getSelectedNumberVersion().Equals("0"))
                            DocumentManager.setSelectedNumberVersion((from doc in DocumentManager.ListDocVersions select Convert.ToInt32(doc.version)).Max().ToString());
                    }

                    ButtonsVersionManager(fileReq);
                }
                else
                {
                    btnAddVersion.Enabled = false;
                    btnModifyVersion.Enabled = false;
                    btnRemoveVersion.Enabled = false;
                }
                repDocumentVersions.DataSource = DocumentManager.ListDocVersions;
                repDocumentVersions.DataBind();
            }
            else
            {
                PlcVersions.Visible = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFullDoc_Click(object sender, EventArgs e)
        {
            FileDoc = null;
            ViewFileDoc(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPrev_Click(object sender, EventArgs e)
        {
            int fromPage = FromPagePreview;
            int lastPage = LastPagePreview;
            ViewFileDoc(true, 0, fromPage - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSucc_Click(object sender, EventArgs e)
        {
            int fromPage = FromPagePreview;
            int lastPage = LastPagePreview;
            ViewFileDoc(true, lastPage + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVersion_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "function", "<script>reallowOp();</script>", false);
            SchedaDocumento tabDocument = DocumentManager.getSelectedRecord();
            DocumentManager.setSelectedNumberVersion(((Button)sender).ToolTip);
            string versionId = string.Empty, version = string.Empty;
            bool isLastVersion = false;

            //ABBATANGELI - nel caso non ci siano file, evita errore Versione precedentemente fossilizzata in memoria
            FileDoc = null;

            if (DocumentManager.getSelectedAttachId() != null) //nel caso di allegato prendo il versionid dell'ultima versione
            {
                versionId = DocumentManager.getSelectedAttachId();
            }
            else
            {
                versionId = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v.versionId).FirstOrDefault();
            }
            BindVersions(versionId);
            UpPnlDocumentAcquired.Visible = false;
            UpPnlDocumentData.Visible = true;

            //I check if there is a file associated with the version: if there is otherwise not see that I display the msg doc to acquire
            FileRequest fileReq = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();

            if (fileReq != null && (!string.IsNullOrEmpty(fileReq.fileSize)) && Convert.ToInt32(fileReq.fileSize) > 0)
            {
                //La segnatura è visibile solo se siamo nel caso di ultima versione di documento e se la versione non ha impressa la segnatura
                version = (from document in DocumentManager.ListDocVersions select document.version).Max();
                isLastVersion = DocumentManager.getSelectedNumberVersion().Equals(version);

                if (DocumentManager.getSelectedAttachId() != null) //nel caso di allegato 
                {
                    DocumentManager.setSelectedAttachId(fileReq.versionId);
                }
                SaveFileDocument(fileReq);


                frame.Attributes["src"] = "../Document/AttachmentViewer.aspx";
                //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:block";
                FindControl("divFrame").Visible = true;
                UpPnlDocumentNotAcquired.Visible = false;
                if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                {
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshCheckInOutPanel();
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_ALL);
                    DocumentImgIdentityCard.Enabled = true;
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_UPLOADFILE);
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                    if ((PageCaller.Equals(CALLER_ATTACHMENT) && !IsAcquired(FileManager.GetFileRequest(VersionIdAttachSelected))) || ((PageCaller.Equals(CALLER_DOCUMENT) || PageCaller.Equals(CALLER_CLASSIFICATIONS)) && !IsAcquired(fileReq)))
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ZOOM);
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_VIEWFILE);
                    }
                    //if (DisableSignature(fileReq) || !isLastVersion)
                    if (DisableSignature(fileReq))
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_POSITIONSIGNATURE);
                    if (IsVisibleButtonIdentityCard(fileReq))
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.V_IDENTITYCARD);
                        DocumentImgIdentityCard.Visible = true;
                    }
                    else
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                        DocumentImgIdentityCard.Visible = false;
                    }
                    if (!isLastVersion)
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_CONVERTPDF);

                    //Controllo firma
                    if (fileReq.firmato == "1")
                    {
                        string tipoFirma = signed;
                        if (!string.IsNullOrEmpty(fileReq.tipoFirma))
                            tipoFirma = fileReq.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) ? elettronicSignature : digitalSignature;
                        LitDocumentSignature.Text = tipoFirma;
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.SIGNED);
                        if (System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"))
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_COSIGNED);
                    }
                    else
                    {
                        LitDocumentSignature.Text = string.Empty;
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                    }

                    string extensionFile = (fileReq.fileName.Split('.').Length > 1) ? (fileReq.fileName.Split('.'))[fileReq.fileName.Split('.').Length - 1] : string.Empty;
                    if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null && !verifyExtensionForSign(extensionFile))
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).DisableButtonImageSign();
                }
            }
            else
            {
                isLastVersion = DocumentManager.ListDocVersions[0].version.Equals(fileReq.version) ? true : false;
                UpPnlDocumentNotAcquired.Visible = true;
                //(FindControl("divFrame") as System.Web.UI.HtmlControls.HtmlGenericControl).Attributes["style"] = "display:none";
                FindControl("divFrame").Visible = false;
                if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                {
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).SetButtonImageTimestamp();
                    //Il documento è stato annullato o non è ancora stato salvato.
                    if (DocumentManager.IsDocumentAnnul() || (DocumentManager.IsNewDocument() && tabDocument.repositoryContext == null))
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                    }
                    // l'utente non possiede i diritti di scrittura o ereditarietà sul documento
                    else if (!UserManager.IsRightsWritingInherits())
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                    }
                    else if (!isLastVersion)// è stata selezionata una versione precedente del documento principale/allegato
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                    }
                    else
                    {
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_ALL);
                        //se il documento non è in uno stato finale oppure è in stato finale
                        //ma è stato effetuato lo sblocco stato per il ruolo corrente 
                        //allora abilito il pulsante di acquisiz.
                        if (!DiagrammiManager.IsDocumentInFinalState() ||
                            Convert.ToInt32(DocumentManager.GetAccessRightDocByDocument(
                            DocumentManager.getSelectedRecord(), UserManager.GetInfoUser())) > 45)
                        {
                            ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.E_UPLOADFILE);
                        }
                        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.NOTV_IDENTITYCARD);
                        DocumentImgIdentityCard.Visible = false;
                    }

                    //Disabilito bottoni fimrato
                    LitDocumentSignature.Text = string.Empty;
                    ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);
                }
            }
            if (!GrdDocumentAttached.Visible)
            {
                BindDocumentAttached();
            }
            GrdDocumentAttached.Visible = true;
            if (!IsZoom || PageCaller != CALLER_SMISTAMENTO || PageCaller != ESAMINA_UNO_A_UNO)
                GridAttachNoUser.Visible = true;

            if (IsPreview(fileReq))
                this.UpdatePanelPreview.Update();

            UpPnlContentDxSx.Update();
            UpPnlContentDxDx.Update();
            UpBottomButtons.Update();
            
            if (!IsZoom && PageCaller != CALLER_SMISTAMENTO && PageCaller != ESAMINA_UNO_A_UNO)
                ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframe();", true);
            else
            {
                if ((PageCaller == CALLER_SMISTAMENTO && !IsZoom && !CallFromSignDetails) || PageCaller == ESAMINA_UNO_A_UNO)
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframeViewerSmistamento();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframeViewer();", true);
                }
            }
            ScriptManager.RegisterStartupScript(Page, GetType(), "tipsy", "tooltipTipsy();", true);
            if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
            {
                ButtonsManager();

                if (EnabledLibroFirma)
                    ButtonsManagerLibroFirma();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void repDocumentVersions_Binding(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string id = ((DocsPaWR.Documento)e.Item.DataItem).version;
                // register click event on linkbutton on scriptmanager for asyncpostback
                ScriptManager scriptMan = ScriptManager.GetCurrent(Parent.Page);
                Button btn = e.Item.FindControl("btnVersion") as Button;
                if (btn != null)
                {
                    btn.Click += btnVersion_Click;
                    scriptMan.RegisterAsyncPostBackControl(btn);
                    if (DisallowOpZoom)
                        btn.OnClientClick = "disallowOp('ContentPlaceHolderHeader');";

                }
                ((Button)e.Item.FindControl("btnVersion")).Text = "V" + id;
                ((Button)e.Item.FindControl("btnVersion")).ToolTip = id;
                if (id == DocumentManager.getSelectedNumberVersion())
                {

                    /*bool isFileSigned = false;
                    if (string.IsNullOrEmpty(DocumentManager.ListDocVersions[e.Item.ItemIndex].firmato))
                    {
                        isFileSigned = DocumentManager.ListDocVersions[e.Item.ItemIndex].fileName.ToUpper().EndsWith("P7M");
                    }
                    else
                    {
                        if (DocumentManager.ListDocVersions[e.Item.ItemIndex].firmato.Equals("1")) isFileSigned = true;
                    }
                    */
                    litDocumentDate.Text = DocumentManager.ListDocVersions[e.Item.ItemIndex].dataInserimento;
                    //litCreatore.Text = DocumentManager.ListDocVersions[e.Item.ItemIndex].autore;
                    BuildStringAuthorFile(DocumentManager.ListDocVersions[e.Item.ItemIndex]);
                    if (Math.Round((double)Int32.Parse(DocumentManager.ListDocVersions[e.Item.ItemIndex].fileSize) / 1024, 3) > 0)
                    {
                        LitDocumentSize.Text = Convert.ToString(Math.Round((double)Int32.Parse(DocumentManager.ListDocVersions[e.Item.ItemIndex].fileSize) / 1024, 3)) + " Kb";
                    }
                    else
                    {
                        LitDocumentSize.Text = string.Empty;
                    }
                    litDocumentPapery.Text = DocumentManager.ListDocVersions[e.Item.ItemIndex].cartaceo ? yes : no;
                    litDocumentVersion.Text = DocumentManager.ListDocVersions[e.Item.ItemIndex].descrizione;

                    //aggiorno il tooltip di info sulla versione correntemente visualizzata
                    UpdateTooltipInfoVersion(DocumentManager.ListDocVersions[e.Item.ItemIndex]);

                    if ((UpPnlDocumentNotAcquired.Visible && !GrdDocumentAttached.Visible) || UpPnlDocumentAcquired.Visible)
                    {
                        frame.Attributes["src"] = "";
                    }
                    else
                    {
                        frame.Attributes["src"] = "../Document/AttachmentViewer.aspx?idattach=" + DocumentManager.getSelectedAttachId() + "&idversion=" + id;
                    }
                    ((Button)e.Item.FindControl("btnVersion")).Style.Add("background-color", "#00487A");
                    ((Button)e.Item.FindControl("btnVersion")).Style.Add("color", "#ffffff");
                }

                if (e.Item.ItemIndex == DocumentManager.ListDocVersions.Length - 1) ((Literal)e.Item.FindControl("repDocumentVersions_sep")).Visible = false;

                //SIGNED
                //if (!string.IsNullOrEmpty(DocumentManager.ListDocVersions[e.Item.ItemIndex].firmato) && DocumentManager.ListDocVersions[e.Item.ItemIndex].firmato.Equals("1"))
                //{
                //    LitDocumentSignature.Text = signed;
                //}
                //else
                //{
                //    LitDocumentSignature.Text = string.Empty;
                //    if (((DocumentButtons)Parent.FindControl("DocumentButtons")) != null)
                //        ((DocumentButtons)Parent.FindControl("DocumentButtons")).RefreshButtons(NttDataWA.UserControls.DocumentButtons.TypeRefresh.D_SIGNED);             
                //}

                ////CHECKOUT

                bool bool_locked = CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.ListDocVersions[e.Item.ItemIndex].docNumber, DocumentManager.ListDocVersions[e.Item.ItemIndex].docNumber, UserManager.GetInfoUser(), false, DocumentManager.getSelectedRecord());
                LitDocumentBlocked.Text = (bool_locked ? str_locked : "");
                //if (DocumentManager.ListDocVersions[e.Item.ItemIndex].inLibroFirma)
                //{
                //    LitDocumentSignature.Text = lock_InLibroFirma;
                //}

            }
        }

        /// <summary>
        /// Aggiorna la label contenente la descrizione della versione
        /// </summary>
        /// <returns></returns>
        public void UpdateDescriptionVersion()
        {
            litDocumentVersion.Text = DescriptionVersion;
            HttpContext.Current.Session.Remove("descriptionVersion");
            ScriptManager.RegisterStartupScript(Page, GetType(), "resizeFrame", "resizeIframe();", true);
            UpdateLitDocumentVersion.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool RemoveVersion()
        {
            const string attachment = "A";
            const string document = "D";
            string type = string.Empty;
            SchedaDocumento documentTab;
            Documento[] listDoc;
            Documento docRemove;
            if (!string.IsNullOrEmpty(DocumentManager.getSelectedAttachId()))
            {
                string docNumber = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()).docNumber;
                documentTab = DocumentManager.getDocumentListVersions(Page, docNumber, docNumber);
                listDoc = documentTab.documenti;
                type = attachment;
            }
            else
            {
                documentTab = DocumentManager.getSelectedRecord();
                listDoc = documentTab.documenti;
                type = document;
            }

            docRemove = (from d in listDoc where d.version.Equals(DocumentManager.getSelectedNumberVersion()) select d).FirstOrDefault();
            try
            {
                return DocumentManager.RemoveVersion(docRemove, documentTab, type);
            }
            catch (NttDataWAException ex)
            {
                return false;
            }
        }

        private void UpdateTooltipInfoVersion(FileRequest fr)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            if (!IsZoom && fr != null && PageCaller != CALLER_SMISTAMENTO && PageCaller != ESAMINA_UNO_A_UNO)
            {
                System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
                strBuilder.Append(Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgA", language) + " " + fr.version + " ");
                if (fr.docNumber.Equals(DocumentManager.getSelectedRecord().docNumber))
                {
                    strBuilder.Append(Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgB", language));
                }
                else
                {
                    if (!string.IsNullOrEmpty(DocumentManager.getSelectedAttachId()))
                    {
                        if (DocumentManager.getSelectedRecord().allegati != null)
                        {
                            if ((from att in DocumentManager.getSelectedRecord().allegati where att.versionId.Equals(DocumentManager.getSelectedAttachId()) select att).FirstOrDefault() != null)
                            {
                                strBuilder.Append(Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgC", language) + " " +
                               (from att in DocumentManager.getSelectedRecord().allegati where att.versionId.Equals(DocumentManager.getSelectedAttachId()) select att).FirstOrDefault().versionLabel);
                            }
                        }
                    }
                }
                string extensionFile = (fr.fileName.Split('.').Length > 1) ? (fr.fileName.Split('.'))[fr.fileName.Split('.').Length - 1] : string.Empty;
                if (!string.IsNullOrEmpty(extensionFile))
                    strBuilder.Append("<br />" + Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgD", language) + extensionFile);
                if (string.IsNullOrEmpty(fr.fileSize) || Convert.ToUInt32(fr.fileSize) == 0)
                {
                    strBuilder.Append("<p style=\"color:red;\">" + Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgE", language) +
                        "</p>");
                }


                SchedaDocumento selectedRecord = DocumentManager.getSelectedRecord();
                if (selectedRecord != null)
                {
                    FileDocumento doc = FileManager.getInstance(selectedRecord.systemId).getInfoFile(Page, fr);
                    if (doc != null && !string.IsNullOrEmpty(doc.nomeOriginale))
                    {
                        strBuilder.Append("<br />" + Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipOriginalFileName", language) + " " + doc.nomeOriginale);
                    }
                }

                imgInfoVersionSelected.ToolTip = strBuilder.ToString();
                imgInfoVersionSelected.AlternateText = strBuilder.ToString();
                BottomButtons.Visible = true;
                UpBottomButtons.Update();
            }
            else if (IsZoom || PageCaller == CALLER_SMISTAMENTO || PageCaller == ESAMINA_UNO_A_UNO)
            {
                //Abbatangeli - In test
                BottomButtons.Visible = false;
                //Fine

                imgInfoVersionSelected.Visible = false;
                UpBottomButtons.Update();
            }
        }

        private void UpdateTooltipInfoVersionLight(Allegato al)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            if (!IsZoom && al != null && PageCaller != CALLER_SMISTAMENTO && PageCaller != ESAMINA_UNO_A_UNO)
            {
                System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
                strBuilder.Append(Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgA", language) + " " + al.version + " ");
                if (!DocumentManager.IsNewDocument() && al.docNumber.Equals(DocumentManager.getSelectedRecord().docNumber))
                {
                    strBuilder.Append(Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgB", language));
                }
                else
                {
                    if (!string.IsNullOrEmpty(DocumentManager.getSelectedAttachId()))
                    {
                        if (((from att in DocumentManager.getSelectedRecord().allegati where att.versionId.Equals(DocumentManager.getSelectedAttachId()) select att).ToList().Count) > 0)
                            strBuilder.Append(Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgC", language) + " " +
                                (from att in DocumentManager.getSelectedRecord().allegati where att.versionId.Equals(DocumentManager.getSelectedAttachId()) select att).FirstOrDefault().versionLabel);
                    }
                }
                string extensionFile = (al.fileName.Split('.').Length > 1) ? (al.fileName.Split('.'))[al.fileName.Split('.').Length - 1] : string.Empty;
                if (!string.IsNullOrEmpty(extensionFile))
                    strBuilder.Append("<br />" + Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgD", language) + extensionFile);
                if (string.IsNullOrEmpty(al.fileSize) || Convert.ToUInt32(al.fileSize) == 0)
                {
                    strBuilder.Append("<p style=\"color:red;\">" + Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgE", language) +
                        "</p>");
                }

                SchedaDocumento selectedRecord = DocumentManager.getSelectedRecord();
                if (!DocumentManager.IsNewDocument() && selectedRecord != null)
                {
                    FileDocumento doc = FileManager.getInstance(selectedRecord.systemId).getInfoFile(Page, al);
                    if (doc != null && !string.IsNullOrEmpty(doc.nomeOriginale))
                    {
                        strBuilder.Append("<br />" + Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipOriginalFileName", language) + " " + doc.nomeOriginale);
                    }
                }

                imgInfoVersionSelected.ToolTip = strBuilder.ToString();
                imgInfoVersionSelected.AlternateText = strBuilder.ToString();
            }
        }

        private void UpdateTooltipInfoVersionLight(FileRequest al)
        {
            string language = UIManager.UserManager.GetUserLanguage();

            if (!IsZoom && al != null && PageCaller != CALLER_SMISTAMENTO && PageCaller != ESAMINA_UNO_A_UNO)
            {
                System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
                strBuilder.Append(Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgA", language) + " " + al.version + " ");
                if (al.docNumber.Equals(DocumentManager.getSelectedRecord().docNumber))
                {
                    strBuilder.Append(Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgB", language));
                }
                else
                {
                    FileRequest at = (from att in DocumentManager.getSelectedRecord().allegati where att.versionId.Equals(DocumentManager.getSelectedAttachId()) select att).FirstOrDefault();
                    string version = string.Empty;
                    if (at != null)
                    {
                        version = at.versionLabel;
                    }
                    strBuilder.Append(Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgC", language) + " " + version);
                }
                string extensionFile = (al.fileName.Split('.').Length > 1) ? (al.fileName.Split('.'))[al.fileName.Split('.').Length - 1] : string.Empty;
                if (!string.IsNullOrEmpty(extensionFile))
                    strBuilder.Append("<br />" + Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgD", language) + extensionFile);
                if (string.IsNullOrEmpty(al.fileSize) || Convert.ToUInt32(al.fileSize) == 0)
                {
                    strBuilder.Append("<p style=\"color:red;\">" + Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipMsgE", language) +
                        "</p>");
                }

                SchedaDocumento selectedRecord = DocumentManager.getSelectedRecord();
                if (selectedRecord != null)
                {
                    FileDocumento doc = FileManager.getInstance(selectedRecord.systemId).getInfoFile(Page, al);
                    if (doc != null && !string.IsNullOrEmpty(doc.nomeOriginale))
                    {
                        strBuilder.Append("<br />" + Utils.Languages.GetLabelFromCode("imgInfoVersionSelectedTooltipOriginalFileName", language) + " " + doc.nomeOriginale);
                    }
                }


                imgInfoVersionSelected.ToolTip = strBuilder.ToString();
                imgInfoVersionSelected.AlternateText = strBuilder.ToString();
            }
        }

        #endregion

        #region management GrdDocumentAttached

        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void GrdDocumentAttached_RowCommand(object source, GridViewCommandEventArgs e)
        {
            string versionId = (((e.CommandSource as ImageButton).Parent.Parent as GridViewRow).FindControl("attachVersionId") as HiddenField).Value;
            FileRequest file = FileManager.GetFileRequest(versionId);
            switch (e.CommandName)
            {
                case "ShowVersion":

                    if (file.GetType() == typeof(Allegato))
                    {
                        DocumentManager.setSelectedAttachId(versionId);
                    }
                    else
                    {
                        DocumentManager.RemoveSelectedAttachId();
                    }
                    ShowDocumentAcquired(true);
                    break;

                case "ShowPrintableVersion":
                    SaveFileDocument(file, true);
                    //frame.Attributes["src"] = "../Document/AttachmentViewer.aspx?download=1";
                    UpPnlContentDxSx.Update();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected string GetLinkVersionID(DocsPaWR.FileRequest fileRequest)
        {
            string link = string.Empty;
            if (fileRequest.versionId != null)
            {
                int maxDimFileSign = 0;
                string msgDesc = "WarningStartProcessSignatureMaxDimFile";
                if (UserManager.IsAuthorizedFunctions("DOWNLOAD_BIG_FILE") || CheckMaxFileSize(out maxDimFileSign))
                {
                    link = ResolveUrl("~/Document/AttachmentViewer.aspx?versionDownload=") + fileRequest.versionId;
                }
                else
                {
                    string maxSiz = Convert.ToString(Math.Round((double)maxDimFileSign / 1048576, 3));
                    link = "javascript:parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + maxSiz + "');";
                }
            }
            return link;
        }


        /// <summary>
        /// 
        /// </summary>
        protected void BindDocumentAttached()
        {
            System.Object[] items = GetAttachments();
            GrdDocumentAttached.DataSource = null;
            GrdDocumentAttached.DataSource = items;
            GrdDocumentAttached.DataBind();
            string versionIdSelected = DocumentManager.getSelectedAttachId();
            if (versionIdSelected == null)
                versionIdSelected = DocumentManager.getSelectedRecord().documenti.FirstOrDefault().versionId;


            for (int i = 0; i < items.Length; i++)
            {
                //abbiamo selezionato un allegato non utente
                if (!PageCaller.ToUpper().Equals(CALLER_SMISTAMENTO))
                {
                    if (FileManager.GetFileRequest(versionIdSelected).GetType() == typeof(Allegato) &&
                        ((FileManager.GetFileRequest(versionIdSelected) as Allegato).TypeAttachment != CODE_ATTACH_USER))
                        break;
                }
                if ((items[i] as FileRequest).versionId == versionIdSelected)
                {
                    GrdDocumentAttached.SelectedIndex = i;
                    if (GridAttachNoUser != null && GridAttachNoUser.SelectedIndex > -1)
                        GridAttachNoUser.SelectedIndex = -1;
                    break;
                }
            }

            items = GetAttachmentsNoUser();
            //abbiamo selezionato un allegato non utente
            if (FileManager.GetFileRequest(versionIdSelected).GetType() == typeof(Allegato) &&
                ((FileManager.GetFileRequest(versionIdSelected) as Allegato).TypeAttachment != CODE_ATTACH_USER) &&
                items != null && items.Length > 0)
            {
                for (int i = 0; i < (items as AttachNouser[]).Length; i++)
                {
                    if ((items[i] as AttachNouser).TypeAttachment == (FileManager.GetFileRequest(versionIdSelected) as Allegato).TypeAttachment)
                    {
                        GridAttachNoUser.SelectedIndex = i;
                        if (GrdDocumentAttached != null && GrdDocumentAttached.SelectedIndex > -1)
                            GrdDocumentAttached.SelectedIndex = -1;

                        break;
                    }
                }
            }

            GridAttachNoUser.DataSource = null;
            GridAttachNoUser.DataSource = items;
            GridAttachNoUser.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected DocsPaWR.FileRequest[] GetAttachments()
        {
            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            List<DocsPaWR.FileRequest> list = new List<DocsPaWR.FileRequest>();
            if (PageCaller.Equals(CALLER_DOCUMENT) || PageCaller.Equals(CALLER_CLASSIFICATIONS) || IsZoom || PageCaller.Equals(CALLER_SMISTAMENTO) || PageCaller == ESAMINA_UNO_A_UNO)
            {
                if (EnableUnifiedView)
                {
                    // Se attiva la visualizzazione unificata, vengono visualizzati tutti gli allegati e il documento principale
                    list.Add(FileManager.GetFileRequest());
                    list.AddRange((from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 1 select att).ToArray());

                    // MEV Visualizzazione allegati sistemi esterni su smistamento
                    if (PageCaller.Equals(CALLER_SMISTAMENTO) && !CallFromSignDetails)
                    {
                        list.AddRange((from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 4 select att).ToArray());
                    }
                }
                else
                {
                    // Visualizzazione del solo documento selezionato
                    list.Add(FileManager.GetFileRequest());
                }
            }
            else //quando ci troviamo in Attachments
            {
                // Se ho acquisito il file per il doc principale allora lo aggiungo alla lista.
                list.Add(FileManager.GetFileRequest());

                string valueRblFilterAttachments = string.Empty;
                if ((Parent.FindControl("rblFilter") as RadioButtonList) != null)
                    valueRblFilterAttachments = (Parent.FindControl("rblFilter") as RadioButtonList).SelectedValue;
                else
                    valueRblFilterAttachments = FilterTipoAttach;

                switch (valueRblFilterAttachments)
                {
                    case TYPE_USER:
                        list.AddRange((from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 1 select att).ToArray());
                        break;
                    case TYPE_ALL:
                        list.AddRange((from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 1 select att).ToArray());
                        break;
                }

            }
            return list.ToArray();
        }

        /// <summary>
        /// Reperimento dell'url dell'immagine per la versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected string GetVersionImage(DocsPaWR.FileRequest fileRequest, bool isInitializePage = false)
        {
            bool isAcquired = IsAcquired(fileRequest);
            if (isInitializePage)
            {
                return FileManager.getFileIconBig(Parent.Page, FileManager.getEstensioneIntoSignedFile(fileRequest.fileName));
            }
            else
            {
                if (fileRequest.GetType() == typeof(DocsPaWR.Allegato))
                {
                    if (isAcquired)
                    {
                        if (((Allegato)fileRequest) != null && ((Allegato)fileRequest).TypeAttachment == 4)
                            return "../Images/Icons/ico_external_attachment.png";
                        else
                            return "../Images/Icons/ico_user_attachment.png";
                    }
                    else
                    {
                        if (((Allegato)fileRequest) != null && ((Allegato)fileRequest).TypeAttachment == 4)
                            return "../Images/Icons/ico_external_attachment_disabled.png";
                        else
                            return "../Images/Icons/ico_user_attachment_disabled.png";
                    }
                }
                else
                {
                    return ResolveUrl(FileManager.getFileIcon(Parent.Page, FileManager.getEstensioneIntoSignedFile(FileManager.getSelectedFile().fileName)));
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected string GetVersionID(DocsPaWR.FileRequest fileRequest)
        {
            string versionId = string.Empty;
            if (fileRequest.versionId != null)
            {
                versionId = fileRequest.versionId;
                string scriptVersionId = "$('#versionId').val('" + versionId + "');";
            }
            return versionId;
        }

        /// <summary>
        /// Reperimento del tooltip della versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected string GetLabelTooltip(DocsPaWR.FileRequest fileRequest)
        {

            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            string tooltip = string.Empty;

            if (fileRequest.GetType() == typeof(DocsPaWR.Allegato))
                tooltip = fileRequest.descrizione + " " + fileRequest.versionId;
            else
                tooltip = doc.oggetto.descrizione;

            if (tooltip != null)
            {
                if (tooltip.Length > 128)
                    return tooltip.Substring(0, 125) + "...";
                else
                    return tooltip.ToString();
            }
            else
                return string.Empty;

        }

        /// <summary>
        /// Reperimento della descrizione della versione
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        protected string GetLabel(DocsPaWR.FileRequest fileRequest)
        {
            if (fileRequest != null)
            {
                if (fileRequest.GetType() == typeof(DocsPaWR.Allegato))
                    return fileRequest.versionLabel;
                else
                {
                    if (Request.QueryString["save"] != null && Request.QueryString["save"].Equals("true"))
                    {
                        int newId = Convert.ToInt32(fileRequest.version) + 1;
                        return string.Format("V.{0}", newId);
                    }
                    else
                        return string.Format("V.{0}", fileRequest.version.ToString());
                }
            }
            return "";
        }

        #endregion

        #region management GridAttachNoUser

        public class AttachNouser
        {
            public const int CODE_ATTACH_PEC = 2;
            public const int CODE_ATTACH_IS = 3;
            public const int CODE_ATTACH_EXTERNAL = 4;
            public int TypeAttachment;
            private int _countAttachIS;
            private int _countAttachPEC;
            private int _countAttachExternal;

            public int CountAttachIS
            {
                get
                {
                    return _countAttachIS;
                }
                set
                {
                    _countAttachIS = value;
                }
            }
            public int CountAttachPEC
            {
                get
                {
                    return _countAttachPEC;
                }
                set
                {
                    _countAttachPEC = value;
                }
            }
            public int CountAttachExternal
            {
                get
                {
                    return _countAttachExternal;
                }
                set
                {
                    _countAttachExternal = value;
                }
            }
            public int TypeAttachNoUser
            {
                get
                {
                    return TypeAttachment;
                }

                set
                {
                    TypeAttachment = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private System.Object[] GetAttachmentsNoUser()
        {
            List<AttachNouser> ListAttNoUser = new List<AttachNouser>();
            AttachNouser attNoUser = new AttachNouser();
            //se ci troviamo nel profilo ed è attiva la vista unificata, allora visualizzo tutti gli allegati non utente
            if (PageCaller.Equals(CALLER_DOCUMENT) || PageCaller.Equals(CALLER_CLASSIFICATIONS))
            {
                if (EnableUnifiedView)
                {
                    attNoUser.TypeAttachNoUser = AttachNouser.CODE_ATTACH_PEC;
                    attNoUser.CountAttachPEC = (from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 2 select att).Count();
                    if (attNoUser.CountAttachPEC > 0)
                        ListAttNoUser.Add(attNoUser);
                    attNoUser = new AttachNouser();
                    attNoUser.TypeAttachNoUser = AttachNouser.CODE_ATTACH_IS;
                    attNoUser.CountAttachIS = (from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 3 select att).Count();
                    if (attNoUser.CountAttachIS > 0)
                        ListAttNoUser.Add(attNoUser);
                    if (EnabledFilterExternal)
                    {
                        attNoUser = new AttachNouser();
                        attNoUser.TypeAttachNoUser = AttachNouser.CODE_ATTACH_EXTERNAL;
                        attNoUser.CountAttachExternal = (from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 4 select att).Count();
                        if (attNoUser.CountAttachExternal > 0)
                            ListAttNoUser.Add(attNoUser);
                    }
                }
            }
            if (PageCaller.Equals(CALLER_ATTACHMENT))
            {
                string valueRblFilterAttachments = string.Empty;
                if ((Parent.FindControl("rblFilter") as RadioButtonList) != null)
                    valueRblFilterAttachments = (Parent.FindControl("rblFilter") as RadioButtonList).SelectedValue;
                else
                    valueRblFilterAttachments = FilterTipoAttach;

                switch (valueRblFilterAttachments)
                {
                    case TYPE_ALL:
                        attNoUser.TypeAttachNoUser = AttachNouser.CODE_ATTACH_PEC;
                        attNoUser.CountAttachPEC = (from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 2 select att).Count();
                        if (attNoUser.CountAttachPEC > 0)
                            ListAttNoUser.Add(attNoUser);
                        attNoUser = new AttachNouser();
                        attNoUser.TypeAttachNoUser = AttachNouser.CODE_ATTACH_IS;
                        attNoUser.CountAttachIS = (from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 3 select att).Count();
                        if (attNoUser.CountAttachIS > 0)
                            ListAttNoUser.Add(attNoUser);
                        if (EnabledFilterExternal)
                        {
                            attNoUser = new AttachNouser();
                            attNoUser.TypeAttachNoUser = AttachNouser.CODE_ATTACH_EXTERNAL;
                            attNoUser.CountAttachExternal = (from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 4 select att).Count();
                            if (attNoUser.CountAttachExternal > 0)
                                ListAttNoUser.Add(attNoUser);
                        }
                        break;
                    case TYPE_PEC:
                        attNoUser.TypeAttachNoUser = AttachNouser.CODE_ATTACH_PEC;
                        attNoUser.CountAttachPEC = (from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 2 select att).Count();
                        ListAttNoUser.Add(attNoUser);
                        break;
                    case TYPE_IS:
                        attNoUser.TypeAttachNoUser = AttachNouser.CODE_ATTACH_IS;
                        attNoUser.CountAttachIS = (from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 3 select att).Count();
                        ListAttNoUser.Add(attNoUser);
                        break;
                    case TYPE_EXT:
                        attNoUser.TypeAttachNoUser = AttachNouser.CODE_ATTACH_EXTERNAL;
                        attNoUser.CountAttachExternal = (from att in DocumentManager.getSelectedRecord().allegati where att.TypeAttachment == 4 select att).Count();
                        ListAttNoUser.Add(attNoUser);
                        break;
                }
            }
            return (ListAttNoUser.Count > 0 ? ListAttNoUser.ToArray() : null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attachmentNoUser"></param>
        /// <returns></returns>
        protected string GetAttachNoUserImage(AttachNouser attachmentNoUser)
        {
            string imageUrl = string.Empty;
            switch (attachmentNoUser.TypeAttachNoUser)
            {
                case AttachNouser.CODE_ATTACH_PEC:
                    imageUrl = "../Images/Icons/ico_pec_attachment.png";
                    break;
                case AttachNouser.CODE_ATTACH_IS:
                    imageUrl = "../Images/Icons/ico_pitre_attachment.png";
                    break;
                case AttachNouser.CODE_ATTACH_EXTERNAL:
                    imageUrl = "../Images/Icons/ico_external_attachment.png";
                    break;
            }
            return imageUrl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attachmentNoUser"></param>
        /// <returns></returns>
        protected string GetAttachNoUserTooltip(AttachNouser attachmentNoUser)
        {
            string tooltip = string.Empty;
            string language = UIManager.UserManager.GetUserLanguage();
            switch (attachmentNoUser.TypeAttachNoUser)
            {
                case AttachNouser.CODE_ATTACH_PEC:
                    tooltip = Utils.Languages.GetLabelFromCode("AttachmentPEC_Tooltip", language) + ": " + attachmentNoUser.CountAttachPEC;
                    break;
                case AttachNouser.CODE_ATTACH_IS:
                    tooltip = Utils.Languages.GetLabelFromCode("AttachmentIS_Tooltip", language) + " " + SimplifiedInteroperabilityManager.SearchItemDescriprion + ": " + attachmentNoUser.CountAttachIS;
                    break;
                case AttachNouser.CODE_ATTACH_EXTERNAL:
                    tooltip = Utils.Languages.GetLabelFromCode("AttachmentExternal_Tooltip", language) + ": " + attachmentNoUser.CountAttachExternal;
                    break;
            }
            return tooltip;
        }



        /// <summary>
        /// Restituisce il tooltip da associare alla versione stampabile
        /// </summary>
        /// <param name="attachmentNoUser"></param>
        /// <returns></returns>
        protected string GetTooltipPrintableVersion()
        {
            return Utils.Languages.GetLabelFromCode("ViewDocumentGrdAttachedBtnPrintableVersion", UIManager.UserManager.GetUserLanguage());
        }

        /// <summary>
        /// Restituisce il codice del tipo allegato associato al link 
        /// </summary>
        /// <param name="attachmentNoUser"></param>
        /// <returns></returns>
        protected string ScriptClickAttachNoUser(AttachNouser attachmentNoUser)
        {
            string script = string.Empty;
            if (!PageCaller.Equals(CALLER_DOCUMENT) && !PageCaller.Equals(CALLER_CLASSIFICATIONS))
            {
                switch (attachmentNoUser.TypeAttachNoUser)
                {
                    case CODE_ATTACH_PEC:
                        script = "$(\"input[value='" + TYPE_PEC + "']:radio\").attr('checked', 'checked');__doPostBack('panelAllegati','');return false;";
                        break;
                    case CODE_ATTACH_IS:
                        script = "$(\"input[value='" + TYPE_IS + "']:radio\").attr('checked', 'checked');__doPostBack('panelAllegati','');return false;";
                        break;
                    case CODE_ATTACH_EXT:
                        script = "$(\"input[value='" + TYPE_EXT + "']:radio\").attr('checked', 'checked');__doPostBack('panelAllegati','');return false;";
                        break;
                }
            }
            return script;
        }

        protected void BtnAttachExt_Click(object sender, EventArgs e)
        {
            SchedaDocumento schedaDoc = DocumentManager.getSelectedRecord();
            if (schedaDoc.allegati != null)
            {
                Allegato all = (from a in schedaDoc.allegati where a.TypeAttachment.Equals(CODE_ATTACH_EXT) select a).FirstOrDefault();
                if (all != null)
                {
                    FileRequest file = FileManager.GetFileRequest(all.versionId);

                    DocumentManager.setSelectedAttachId(all.versionId);

                    ShowDocumentAcquired(true);
                }
            }
        }

        protected void AddVersion_Click(object sender, EventArgs e)
        {
            SchedaDocumento schedaDoc = DocumentManager.getSelectedRecord();
            if (DocumentManager.IsDocumentoInLibroFirma(schedaDoc) && LibroFirmaManager.IsAttivoBloccoModificheDocumentoInLibroFirma())
            {
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningBloccoModificheDocumentoInLf', 'warning');} else {parent.parent.ajaxDialogModal('WarningBloccoModificheDocumentoInLf', 'warning');}", true);
                return;
            }
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "AddVersion", "ajaxModalPopupVersionAdd();", true);
        }

        /// <summary>
        /// Restituisce il numero di allegati della tipologia selezionata(is,pec,esterni)
        /// </summary>
        /// <param name="attachmentNoUser"></param>
        /// <returns></returns>
        protected string GetCountAttachNoUser(AttachNouser attachmentNoUser)
        {
            int count = 0;
            switch (attachmentNoUser.TypeAttachNoUser)
            {
                case AttachNouser.CODE_ATTACH_PEC:
                    count = attachmentNoUser.CountAttachPEC;
                    break;
                case AttachNouser.CODE_ATTACH_IS:
                    count = attachmentNoUser.CountAttachIS;
                    break;
                case AttachNouser.CODE_ATTACH_EXTERNAL:
                    count = attachmentNoUser.CountAttachExternal;
                    break;
            }
            return count.ToString();
        }

        /// <summary>
        /// Restituisce true se sarà visualizzato il file in anteprime
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        private bool IsPreview(FileRequest file)
        {
            bool retVal = false;
            bool isPdf = FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(PDF);

            if (isPdf)
            {
                int fileSizeLimit = 5 * 1024 * 1024;
                int fileSize = int.Parse(file.fileSize);

                if (!string.IsNullOrEmpty(NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_PREVIEW_MB_LIMIT")) && Int32.Parse(NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_PREVIEW_MB_LIMIT")) > 0)
                {
                    fileSizeLimit = 1024 * 1024 * Int32.Parse(NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_PREVIEW_MB_LIMIT"));

                    if (fileSize > fileSizeLimit)
                        retVal = true;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Disabilita la segnatura se il file non è di tipo pdf, ha impressa la segnatura e se è un allegato non di tipo utente
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        private bool DisableSignature(FileRequest file)
        {
            //Verifico se la versione del documento ha impressa la segnatura
            bool withSignature = DocumentManager.IsVersionWithSegnature(file.versionId);

            //Verifico se la versione è un pdf o un pdf con firma digitale, oppure se è un file convertibile in pdf
            // Gabriele Melini 24-02-2014
            // le stringhe con le estensioni sono definite minuscole
            // il ToLower evita schianti in caso di file con estensioni maiuscole (e.g. INC000000309701)
            bool isPdf = (
                //FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(PDF) ||
                //FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(TSD) ||
                //FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(M7M) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(PDF) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(TSD) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(M7M) ||
                (ShowDocumentAsPdfFormat && PdfConverterInfo.CanConvertFileToPdf(file.fileName)) ||
                 (!string.IsNullOrEmpty(NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_RENDER_PDF")) && NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_RENDER_PDF").Equals("1")));

            bool isAllegatoUser = true;
            //Verifico se il file request è un allegato di tipo utente
            if (file.GetType() == typeof(Allegato))
                isAllegatoUser = (file as Allegato).TypeAttachment == CODE_ATTACH_USER;
            else
                //nel caso di dettaglio allegato verifico che sia di tipo utente
                if (file.GetType() == typeof(Documento) && Page.Request["typeAttachment"] != null)
                    isAllegatoUser = Convert.ToInt32(Page.Request["typeAttachment"]) == CODE_ATTACH_USER;

            // Controllo se l'allegato è esterno

            bool isAllegatoExt = true;
            //Verifico se il file request è un allegato di tipo utente
            if (file.GetType() == typeof(Allegato))
                isAllegatoExt = (file as Allegato).TypeAttachment == CODE_ATTACH_EXT;
            else
                //nel caso di dettaglio allegato verifico che sia di tipo utente
                if (file.GetType() == typeof(Documento) && Page.Request["typeAttachment"] != null)
                    isAllegatoExt = Convert.ToInt32(Page.Request["typeAttachment"]) == CODE_ATTACH_EXT;


            return !(!withSignature && isPdf && (isAllegatoUser || isAllegatoExt));
        }

        /// <summary>
        /// Controlla se è stata impostata la segnatura
        /// </summary>
        /// <returns></returns>
        private bool IsVisibleSignature()
        {
            return (!string.IsNullOrEmpty(PositionSignature) || !string.IsNullOrEmpty(CharacterSignature) || !string.IsNullOrEmpty(ColorSignature) ||
                    !string.IsNullOrEmpty(OrientationSignature) || TypeLabel || !string.IsNullOrEmpty(RotationSignature) || NoTimbro || PrintOnFirstPage ||
                    PrintOnLastPage);
        }

        private bool IsPdf(FileRequest file)
        {
            // Gabriele Melini 24-02-2014
            // le stringhe con le estensioni sono definite minuscole
            // il ToLower evita schianti in caso di file con estensioni maiuscole
            return (
                //FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(PDF) ||
                //FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(TSD) ||
                //FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(M7M) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(PDF) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(TSD) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).ToLowerInvariant().Equals(M7M) ||
                (ShowDocumentAsPdfFormat && PdfConverterInfo.CanConvertFileToPdf(file.fileName)));

        }


        #endregion

        #region Management button identity card

        /// <summary>
        /// Gestisce la visibilità del bottone Carta di identità
        /// </summary>
        /// <param name="fileReq"></param>
        /// <returns></returns>
        private bool IsVisibleButtonIdentityCard(FileRequest fileReq)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_IDENTITY_CARD")) &&
                NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_IDENTITY_CARD").Equals("1"))
            {
                FileInformation fileInformation = DocumentManager.GetFileInformation(fileReq, UserManager.GetInfoUser());
                if (fileInformation != null)
                {
                    if ((fileInformation.Status.Equals(VerifyStatus.InProgress) ||
                       fileInformation.Status.Equals(VerifyStatus.Invalid) ||
                       fileInformation.Status.Equals(VerifyStatus.Valid)))
                    {
                        result = true;
                        SetImageIdentityCard(fileInformation.Status);
                    }
                }
            }

            return result;
        }

        public void SetImageIdentityCard(VerifyStatus status)
        {
            if (status.Equals(VerifyStatus.InProgress))
            {
                DocumentImgIdentityCard.ImageUrl = "../Images/Icons/ico_verificata_progress.png";
                DocumentImgIdentityCard.ImageUrlDisabled = "../Images/Icons/ico_verificata_progress_disabled.png";
                DocumentImgIdentityCard.OnMouseOverImage = "../Images/Icons/ico_verificata_progress_hover.png";
                DocumentImgIdentityCard.OnMouseOutImage = "../Images/Icons/ico_verificata_progress.png";
            }
            else
            {
                DocumentImgIdentityCard.ImageUrl = "../Images/Icons/ico_verificata.png";
                DocumentImgIdentityCard.ImageUrlDisabled = "../Images/Icons/ico_spenta.png";
                DocumentImgIdentityCard.OnMouseOverImage = "../Images/Icons/ico_verificata_hover.png";
                DocumentImgIdentityCard.OnMouseOutImage = "../Images/Icons/ico_verificata.png";
            }
        }
        #endregion

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

        protected void GridAttachNoUser_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "ShowAttachedSelected")
                return;

            int iTypeAttachment = Convert.ToInt32(e.CommandArgument);

            if (PageCaller.Equals(CALLER_DOCUMENT) || PageCaller.Equals(CALLER_CLASSIFICATIONS))
            {
                switch (iTypeAttachment)
                {
                    case CODE_ATTACH_PEC:
                        Response.Redirect("~/Document/Attachments.aspx?attNoUser=" + TYPE_PEC);
                        break;
                    case CODE_ATTACH_IS:
                        Response.Redirect("~/Document/Attachments.aspx?attNoUser=" + TYPE_IS);
                        break;
                    case CODE_ATTACH_EXT:
                        SchedaDocumento schedaDoc = DocumentManager.getSelectedRecord();
                        if (schedaDoc.allegati != null && schedaDoc.allegati.Count(a => a.TypeAttachment.Equals(CODE_ATTACH_EXT)) == 1)
                            BtnAttachExt_Click(null, null);
                        else
                            Response.Redirect("~/Document/Attachments.aspx?attNoUser=" + TYPE_EXT);
                        
                        break;
                }
            }
            else
            {
                switch (iTypeAttachment)
                {
                    case CODE_ATTACH_PEC:
                        //script = "$(\"input[value='" + TYPE_PEC + "']:radio\").attr('checked', 'checked');__doPostBack('panelAllegati','');return false;";
                        break;
                    case CODE_ATTACH_IS:
                        //script = "$(\"input[value='" + TYPE_IS + "']:radio\").attr('checked', 'checked');__doPostBack('panelAllegati','');return false;";
                        break;
                    case CODE_ATTACH_EXT:
                        //script = "$(\"input[value='" + TYPE_EXT + "']:radio\").attr('checked', 'checked');__doPostBack('panelAllegati','');return false;";
                        break;

                }
            }
        }

        private bool CheckMaxFileSize(out int maxDimFileSign)
        {
            DocsPaWR.FileRequest fileReq = null;
            if (FileManager.GetSelectedAttachment() == null)
            {
                fileReq = UIManager.FileManager.getSelectedFile();
            }
            else
            {
                fileReq = FileManager.GetSelectedAttachment();
            }
            maxDimFileSign = 0;
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString())) &&
               !Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString()).Equals("0"))
                maxDimFileSign = Convert.ToInt32(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString()));
            if (maxDimFileSign > 0 && Convert.ToInt32(fileReq.fileSize) > maxDimFileSign)
            {
                return false;
            }
            else
                return true;
        }

        protected void btnVersioneStampabile_Click(object sender, EventArgs e)
        {
            int maxDimFileSign = 0;
            
            if (!UserManager.IsAuthorizedFunctions("DOWNLOAD_BIG_FILE"))
            {
                if (CheckMaxFileSize(out maxDimFileSign))
                {
                    string maxSize = Convert.ToString(Math.Round((double)maxDimFileSign / 1048576, 3));
                    string msgDesc = "WarningStartProcessSignatureMaxDimFile";
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + maxSize + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + maxSize + "');}", true);
                }
            }
        }
    }
}
