using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.Text.RegularExpressions;
using NttDatalLibrary;
using System.Collections;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
	public partial class Signature : System.Web.UI.Page
    {
        #region Properties

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

            set
            {
                HttpContext.Current.Session["character"] = value;
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
            set
            {
                HttpContext.Current.Session["color"] = value;
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
            set
            {
                HttpContext.Current.Session["orientation"] = value;
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
            set
            {
                HttpContext.Current.Session["position"] = value;
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
            set
            {
                HttpContext.Current.Session["typeLabel"] = value;
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
            set
            {
                HttpContext.Current.Session["rotation"] = value;
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
            set
            {
                HttpContext.Current.Session["NoTimbro"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool NoTimbroTemp
        {
            get
            {
                if (HttpContext.Current.Session["NoTimbroTemp"] != null)
                    return (bool)HttpContext.Current.Session["NoTimbroTemp"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["NoTimbroTemp"] = value;
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
            set
            {
                HttpContext.Current.Session["printOnFirstPage"] = value;
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
            set
            {
                HttpContext.Current.Session["printOnLastPage"] = value;
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

        /// <summary>
        /// 
        /// </summary>
        private DocsPaWR.Documento[] ListDocVersions
        {
            get
            {
                DocsPaWR.Documento[] result = null;
                if (HttpContext.Current.Session["listDocVersions"] != null)
                {
                    result = HttpContext.Current.Session["listDocVersions"] as DocsPaWR.Documento[];
                }
                return result;
            }
        }

        private FileDocumento FileDoc
        {
            get
            {
                if (HttpContext.Current.Session["fileDoc"] != null)
                    return HttpContext.Current.Session["fileDoc"] as FileDocumento;
                else return null;
            }
            set
            {
                HttpContext.Current.Session["fileDoc"] = value;
            }
        }

        private string SchedaDocSystemId
        {
            set
            {
                HttpContext.Current.Session["schedaDocSystemId"] = value;
            }
        }

        private bool SavePermanentDisplaySignature
        {
            get
            {
                if (HttpContext.Current.Session["savePermanentDisplaySignature"] != null)
                    return (bool)HttpContext.Current.Session["savePermanentDisplaySignature"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["savePermanentDisplaySignature"] = value;
            }
        }

        private static bool PermanentDisplaySignature
        {
            get
            {
                if (HttpContext.Current.Session["permanentDisplaySignature"] != null)
                    return (bool)HttpContext.Current.Session["permanentDisplaySignature"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["permanentDisplaySignature"] = value;
            }
        }

        /// <summary>
        /// Indentifica se siamo nella situazione di apertura della popup(inserita per evitare che alla chiusura della popup riesegua il tutto il page_load)
        /// </summary>
        private bool OpenSignaturePopup
        {
            get
            {
                if (HttpContext.Current.Session["OpenSignaturePopup"] != null)
                    return (bool)HttpContext.Current.Session["OpenSignaturePopup"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["OpenSignaturePopup"] = value;
            }
        }

        private bool ChangeSignature
        {
            get
            {
                if (HttpContext.Current.Session["ChangeSignature"] != null)
                    return (bool)HttpContext.Current.Session["ChangeSignature"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["ChangeSignature"] = value;
            }
        }
        #endregion


        private string firma = "";
        private string positionSignature;
        private string characterSignature;
        private string colorSignature;
        private string orientationSignature;
        private bool typeLabel;
		private string estensione = String.Empty;
        private string urlImageBtnPosSegnature = "../Images/Documents/document_position_segnature.png";
        private string urlImageBtnPosSegnatureSelected = "../Images/Documents/document_position_segnature_selected.png";
        private FileRequest fileReq;
        private const string PDF = "pdf";
        private const string TSD = "tsd";
        private const string M7M = "m7m";

        #region Const

        private const string NoSignature = "noSignatureStamp";

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (this.Request.QueryString["printSignatureA4"] == null)
                {
                    if (FileManager.GetSelectedAttachment() == null)
                        fileReq = UIManager.FileManager.getSelectedFile();
                    else
                    {
                        //if (this.ListDocVersions != null)
                        //    fileReq = (from versionDoc in this.ListDocVersions where versionDoc.version.Equals(DocumentManager.getSelectedNumberVersion()) && versionDoc.versionId.Equals(FileManager.GetSelectedAttachment().versionId)  select versionDoc).FirstOrDefault();

                        //if(this.ListDocVersions == null || fileReq == null)
                            fileReq = FileManager.GetSelectedAttachment();
                    }
                }
                else
                    fileReq = FileManager.GetFileRequest();
                //if (tb_hidden.Value != null)
                //{
                //    if (tb_hidden.Value == "TxtX")
                //    {
                //        SetFocus(TxtX);
                //    }
                //    if (tb_hidden.Value == "TxtY")
                //    {
                //        SetFocus(TxtY);
                //    }
                //}
                if (!Page.IsPostBack && OpenSignaturePopup)
                {
                    InitializeComponent();
                    InitializeLanguage();
                    InitializesPage();

                }
                else
                {
                    RefreshScript();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
        }


        /// <summary>
        /// Reperimento filerequest a partire dal versionid
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        private DocsPaWR.FileRequest GetFileRequest(string versionId)
        {
            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            DocsPaWR.FileRequest fileRequest = doc.documenti.Where(e => e.versionId == versionId).FirstOrDefault();

            if (fileRequest == null)
            {
                // Ricerca la versione negli allegati al documento
                fileRequest = doc.allegati.Where(e => e.versionId == versionId).FirstOrDefault();
            }

            return fileRequest;
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.SignatureLblPositionInformation.Text = Utils.Languages.GetLabelFromCode("SignatureLblPositionInformation", language);
            this.SignatureLblDocumentDim.Text = Utils.Languages.GetLabelFromCode("SignatureLblDocumentDim", language);
            this.SignatureLblDocumentDimAltezza.Text = Utils.Languages.GetLabelFromCode("SignatureLblDocumentDimAltezza", language);
            this.SignatureLblDocumentDimLarghezza.Text = Utils.Languages.GetLabelFromCode("SignatureLblDocumentDimLarghezza", language);
            this.SignatureLblDocumentCustomLocation.Text = Utils.Languages.GetLabelFromCode("SignatureLblDocumentCustomLocation", language);
            this.SignatureLblCaracteristic.Text = Utils.Languages.GetLabelFromCode("SignatureLblCaracteristic", language);
            this.SignatureLblCharacter.Text = Utils.Languages.GetLabelFromCode("SignatureLblCharacter", language);
            this.SignatureLblColor.Text = Utils.Languages.GetLabelFromCode("SignatureLblColor", language);
            this.SignatureRdlStampVertical.Text = Utils.Languages.GetLabelFromCode("SignatureRdlStampVertical", language);
            this.SignatureRdlStampHorizontal.Text = Utils.Languages.GetLabelFromCode("SignatureRdlStampHorizontal", language);
            this.SignatureRdlSignature.Text = Utils.Languages.GetLabelFromCode("SignatureRdlSignature", language);
            this.ChkDigitalSignatureInfo.Text = Utils.Languages.GetLabelFromCode("ChkDigitalSignatureInfo", language);
            this.SignatureRblPrintOnFirstPage.Text = Utils.Languages.GetLabelFromCode("SignatureRblPrintOnFirstPage", language);
            this.SignatureRblPrintOnLastPage.Text = Utils.Languages.GetLabelFromCode("SignatureRblPrintOnLastPage", language);
            this.SignatureBtnConfirm.Text = Utils.Languages.GetLabelFromCode("SignatureBtnConfirm", language);
            this.SignatureBtnClose.Text = Utils.Languages.GetLabelFromCode("SignatureBtnClose", language);
            this.SignatureColorRed.Text = Utils.Languages.GetLabelFromCode("SignatureColorRed", language);
            this.SignatureColorBlack.Text = Utils.Languages.GetLabelFromCode("SignatureColorBlack", language);
            this.NoSignatureStamp.Text = Utils.Languages.GetLabelFromCode("SignatureNoSignatureStamp", language);
            this.SignatureRblDigitalCompleted.Text = Utils.Languages.GetLabelFromCode("SignatureRblDigitalCompleted", language);
            this.SignatureRblDigitalSynthesis.Text = Utils.Languages.GetLabelFromCode("SignatureRblDigitalSynthesis", language);
        }

        private void InitializeComponent()
        {
            //this.TxtX.TextChanged += new System.EventHandler(this.tbxPos_TextChanged);
            //this.TxtY.TextChanged += new System.EventHandler(this.tbxPos_TextChanged);
            //this.DdlCharacter.SelectedIndexChanged += new System.EventHandler(this.CharacterList_SelectedIndexChanged);
            //this.DdlColour.SelectedIndexChanged += new System.EventHandler(this.ColorList_SelectedIndexChanged);
            //this.RblOrientation.SelectedIndexChanged += new System.EventHandler(this.RblOrientation_SelectedIndexChanged);
            //this.ChkDigitalSignatureInfo.CheckedChanged += new System.EventHandler(this.ChkSegnatureInfo_CheckedChanged);
        }

        private void SetImageBtnPositionSegnature(CustomImageButton btn, string urlImage)
        {
            btn.ImageUrl = urlImage;
            btn.OnMouseOutImage = urlImage;
            btn.OnMouseOverImage = urlImage;
        }

        private FileDocumento DocumentWithSignature(FileRequest fileReq)
        {
            Response.Expires = -1;
            DocsPaWR.InfoUtente infoUser = UserManager.GetInfoUser();
            DocsPaWR.labelPdf label = new DocsPaWR.labelPdf();
            SchedaDocumento sch = null;
            if (FileManager.GetSelectedAttachment() == null)
                sch = DocumentManager.getSelectedRecord();
            else if (DocumentManager.getSelectedRecord().documentoPrincipale == null)
            {
                sch = DocumentManager.getDocumentListVersions(this.Page, fileReq.docNumber, fileReq.docNumber);
            }
            FileDocumento theDoc;
            if (FileDoc!=null && FileDoc.LabelPdf!=null)
            {
                //load data in the object Label
                label.position = FileDoc.LabelPdf.default_position;
                label.tipoLabel = FileDoc.LabelPdf.tipoLabel;
                label.label_rotation = FileDoc.LabelPdf.label_rotation;
                label.sel_font = FileDoc.LabelPdf.sel_font;
                label.sel_color = FileDoc.LabelPdf.sel_color;
                label.orientamento = FileDoc.LabelPdf.orientamento;
                if (fileReq.firmato == "1" || NoTimbroTemp)
                    label.notimbro = NoTimbroTemp;
                if (FileDoc.LabelPdf.digitalSignInfo != null)
                {
                    label.digitalSignInfo = new labelPdfDigitalSignInfo();
                    label.digitalSignInfo.printOnFirstPage = FileDoc.LabelPdf.digitalSignInfo.printOnFirstPage;
                    label.digitalSignInfo.printOnLastPage = FileDoc.LabelPdf.digitalSignInfo.printOnLastPage;
                    if (fileReq.firmato == "1" || NoTimbroTemp)
                        label.digitalSignInfo.printFormatSign = FileDoc.LabelPdf.digitalSignInfo.printFormatSign;
                }
                if (label.label_rotation == null || label.orientamento == null || label.position == null || label.sel_color == null || label.sel_font == null)
                {
                    label.position = PositionSignature;
                    label.label_rotation = RotationSignature;
                    label.sel_font = CharacterSignature;
                    label.sel_color = ColorSignature;
                    label.orientamento = OrientationSignature;
                }
            }
            else
            {
                
                label.position = PositionSignature;
                label.tipoLabel = TypeLabel;
                label.label_rotation = RotationSignature;
                label.sel_font = CharacterSignature;
                label.sel_color = ColorSignature;
                label.orientamento = OrientationSignature;
                if (fileReq.firmato == "1")
                    label.notimbro = NoTimbro;
                
            }
            if (this.Request.QueryString["printSignatureA4"] == null)
            {
                if (SavePermanentDisplaySignature)
                {
                    theDoc = FileManager.getInstance(sch.systemId).saveFileConSegnatura(this.Page, sch, label, fileReq);
                    HttpContext.Current.Session.Remove("savePermanentDisplaySignature");
                }
                else if (!string.IsNullOrEmpty(NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_RENDER_PDF")) &&
                         NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_RENDER_PDF").Equals("1") && !IsPdf(fileReq))
                        {
                            try
                            {
                                theDoc = FileManager.getInstance(sch.systemId).DocumentoGetFileConSegnaturaUsingLC(this.Page, sch, label, fileReq);
                            }
                            catch (System.Exception ex)
                            {
                                return null;
                            }
                        }
                        else
                        {
                            theDoc = FileManager.getInstance(sch.systemId).getFileConSegnatura(this.Page, sch, label, fileReq);
                        }
            }
            else
                theDoc = FileManager.getInstance(sch.systemId).getVoidFileConSegnatura(fileReq, sch, label, this.Page);

            if (theDoc!=null && theDoc.LabelPdf.default_position.Equals("pos_pers"))
                theDoc.LabelPdf.default_position = theDoc.LabelPdf.positions[4].PosX + "-" + theDoc.LabelPdf.positions[4].PosY;
            return theDoc;
        }

        private void InitializesPage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            NoTimbroTemp = NoTimbro;
            FileDoc = DocumentWithSignature(fileReq);
            if (FileDoc == null)
            {
                SetImageBtnPositionSegnature(BtnPosLeft, urlImageBtnPosSegnature);
                SetImageBtnPositionSegnature(BtnPosRight, urlImageBtnPosSegnature);
                SetImageBtnPositionSegnature(BtnDownSx, urlImageBtnPosSegnature);
                SetImageBtnPositionSegnature(BtnDownDx, urlImageBtnPosSegnature);
                HttpContext.Current.Session.Remove("OpenSignaturePopup");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "$(function() {ajaxDialogModal('ErrorConverPdf', 'error', null, null, null, null, 'parent.closeAjaxModal(\\'Signature\\',\\'\\');');});", true);
                return;
            }
            if(fileReq.firmato == "1")
                FileDoc.LabelPdf.notimbro = NoTimbro;
            this.frameSignature.Attributes["src"] = "../Document/AttachmentViewer.aspx";

            double a = Convert.ToDouble(FileDoc.LabelPdf.pdfHeight);
            double b = Convert.ToDouble(FileDoc.LabelPdf.pdfWidth);

            docHeight.Text = Convert.ToString(Convert.ToInt32(a));
            docWidth.Text = Convert.ToString(Convert.ToInt32(b));

            //Controllo chiave che regola la visualizzazione dei pulsanti per la gestione del Nascondi timbro/Segnatuta
            if (NttDataWA.Utils.InitConfigurationKeys.GetValue( UserManager.GetUserInSession().idAmministrazione, "FE_DETTAGLI_FIRMA").Equals("0"))
            {
                this.RblFormatSign.Visible = false;
                this.RblOrientation.Items.Remove(RblOrientation.Items.FindByValue("noSignatureStamp"));
                this.RblOrientation.RepeatColumns = 0;
            }
            //se la pagina è richiesta per il posizionamento della segnatura nel documento acquisito ed è attiva la chiave permanent_displays_signature
            if (NttDataWA.Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, "FE_PERMANENT_DISPLAYS_SEGNATURE").Equals("1") && this.Request.QueryString["printSignatureA4"] == null)
            {
                DocsPaWR.SchedaDocumento schedaCorrente = DocumentManager.getSelectedRecord();
                string diritti = "0";
                if (schedaCorrente != null)
                    diritti = schedaCorrente.accessRights;
                // Abilito il pulsante 'Salva Versione con segnatura' se l'utente ha i permessi di
                if (diritti.Equals("0") || diritti.Equals("255") || diritti.Equals("63"))
                {
                    this.SignatureBtnSave.Visible = true;
                    this.SignatureBtnSave.Text = Utils.Languages.GetLabelFromCode("SignatureBtnSave", language);
                    this.SignatureBtnSave.ToolTip = Utils.Languages.GetLabelFromCode("SignatureBtnSaveToolTip", language);
                }
            }
            if (this.Request.QueryString["printSignatureA4"] != null && this.Request.QueryString["printSignatureA4"] == "true")
            {
                this.SignatureBtnConfirm.Visible = false;
                this.ChkDigitalSignatureInfo.Enabled = false;
                this.ChkDigitalSignatureInfo.Checked = false;
                this.ChkDigitalSignatureInfo.Enabled = false;
                this.RblDigitalSignaturePos.Enabled = false;
            }

            //if (IsPermanentDisplaysSegnature)
            //{
            //    TxtX.AutoPostBack = false;
            //    TxtY.AutoPostBack = false;
            //}

            // VALORIZZO LE DROPDOWNLIST DEL CARATTERE E DEL COLORE
            // prende tutte le amm.ni disponibili
            NttDataWA.UIManager.AdministrationManager amm = new NttDataWA.UIManager.AdministrationManager();
            amm.ListaAmministrazioni();
            string idAmm = NttDataWA.UIManager.UserManager.GetInfoUser().idAmministrazione;
            DocsPaWR.InfoAmministrazione currAmm = new DocsPaWR.InfoAmministrazione();
            foreach (DocsPaWR.InfoAmministrazione infoAmm in amm.AmmGetListAmministrazioni())
            {
                if (infoAmm.IDAmm.Equals(idAmm))
                {
                    currAmm = infoAmm;
                    break;
                }
            }
            //foreach(ListItem l in DdlColour)

            DdlColour.SelectedIndex = System.Convert.ToInt32(FileDoc.LabelPdf.sel_color);

            DocsPaWR.carattere[] carat = currAmm.Timbro.carattere;
            //se non ho caricato i caratteri ho solo l'item blank per questo deve essere < 2.
            if (DdlCharacter.Items.Count < 2)
            {
                for (int j = 0; j < carat.Length; j++)
                {
                    DdlCharacter.Items.Add(carat[j].caratName + " - " + carat[j].dimensione);
                }
            }

            DdlCharacter.SelectedIndex = System.Convert.ToInt32(FileDoc.LabelPdf.sel_font);

            firma = fileReq.firmato;

            if (firma.Equals("1") && this.Request.QueryString["printSignatureA4"] == null)
            {
                // Caricamento dati stampa firma digitale
                this.ChkDigitalSignatureInfo.Checked = (FileDoc.LabelPdf.digitalSignInfo != null);

                if (this.ChkDigitalSignatureInfo.Checked)
                {
                    if (FileDoc.LabelPdf.digitalSignInfo.printOnFirstPage)
                        this.RblDigitalSignaturePos.SelectedValue = "printOnFirstPage";
                    else
                        this.RblDigitalSignaturePos.SelectedValue = "printOnLastPage";
                    if(FormatSignature.Equals(TypePrintFormatSign.Sign_Extended))
                    {
                        this.RblFormatSign.SelectedValue = "digitalCompleted";
                    }
                    else
                    {
                        this.RblFormatSign.SelectedValue = "digitalSynthesis";
                    }
                }

                this.RblDigitalSignaturePos.Enabled = this.ChkDigitalSignatureInfo.Checked;
                this.RblFormatSign.Enabled = this.ChkDigitalSignatureInfo.Checked;
            }
            else
            {
                //this.NoSignatureStamp.Enabled = false;
                if (!UIManager.UserManager.IsAuthorizedFunctions("DO_ENABLE_SIGNATURE_INFO"))
                    this.ChkDigitalSignatureInfo.Enabled = false;
                this.RblDigitalSignaturePos.Enabled = false;
                this.RblFormatSign.Enabled = false;
            }

             switch (FileDoc.LabelPdf.default_position)
                    {
                        case "pos_upSx": //è in posizione 0
                            SetImageBtnPositionSegnature(BtnPosLeft, urlImageBtnPosSegnatureSelected);
                            SetImageBtnPositionSegnature(BtnPosRight, urlImageBtnPosSegnature);
                            SetImageBtnPositionSegnature(BtnDownSx, urlImageBtnPosSegnature);
                            SetImageBtnPositionSegnature(BtnDownDx, urlImageBtnPosSegnature);
                            TxtX.Text = FileDoc.LabelPdf.positions[0].PosX;
                            TxtY.Text = FileDoc.LabelPdf.positions[0].PosY;
                            break;

                        case "pos_upDx": //è in posizione 1
                            SetImageBtnPositionSegnature(BtnPosLeft, urlImageBtnPosSegnature);
                            SetImageBtnPositionSegnature(BtnPosRight, urlImageBtnPosSegnatureSelected);
                            SetImageBtnPositionSegnature(BtnDownSx, urlImageBtnPosSegnature);
                            SetImageBtnPositionSegnature(BtnDownDx, urlImageBtnPosSegnature);
                            if (Convert.ToInt32(FileDoc.LabelPdf.positions[1].PosX) > Convert.ToInt32(docWidth.Text))
                            {
                                TxtX.Text = docWidth.Text;
                            }
                            else
                            {
                                TxtX.Text = FileDoc.LabelPdf.positions[1].PosX;
                            }
                            if (Convert.ToInt32(FileDoc.LabelPdf.positions[1].PosY) > Convert.ToInt32(docHeight.Text))
                            {
                                TxtY.Text = docHeight.Text;
                            }
                            else
                            {
                                TxtY.Text = FileDoc.LabelPdf.positions[1].PosY;
                            }
                            break;
                        case "pos_downSx": //è in posizione 2
                            SetImageBtnPositionSegnature(BtnPosLeft, urlImageBtnPosSegnature);
                            SetImageBtnPositionSegnature(BtnPosRight, urlImageBtnPosSegnature);
                            SetImageBtnPositionSegnature(BtnDownSx, urlImageBtnPosSegnatureSelected);
                            SetImageBtnPositionSegnature(BtnDownDx, urlImageBtnPosSegnature);
                            if (Convert.ToInt32(FileDoc.LabelPdf.positions[2].PosX) > Convert.ToInt32(docWidth.Text))
                            {
                                TxtX.Text = docWidth.Text;
                            }
                            else
                            {
                                TxtX.Text = FileDoc.LabelPdf.positions[2].PosX;
                            }
                            if (Convert.ToInt32(FileDoc.LabelPdf.positions[2].PosY) > Convert.ToInt32(docHeight.Text))
                            {
                                TxtY.Text = docHeight.Text;
                            }
                            else
                            {
                                TxtY.Text = FileDoc.LabelPdf.positions[2].PosY;
                            }
                            break;
                        case "pos_downDx": //è in posizione 3
                            SetImageBtnPositionSegnature(BtnPosLeft, urlImageBtnPosSegnature);
                            SetImageBtnPositionSegnature(BtnPosRight, urlImageBtnPosSegnature);
                            SetImageBtnPositionSegnature(BtnDownSx, urlImageBtnPosSegnature);
                            SetImageBtnPositionSegnature(BtnDownDx, urlImageBtnPosSegnatureSelected);
                            if (Convert.ToInt32(FileDoc.LabelPdf.positions[3].PosX) > Convert.ToInt32(docWidth.Text))
                            {
                                TxtX.Text = docWidth.Text;
                            }
                            else
                            {
                                TxtX.Text = FileDoc.LabelPdf.positions[3].PosX;
                            }
                            if (Convert.ToInt32(FileDoc.LabelPdf.positions[3].PosY) > Convert.ToInt32(docHeight.Text))
                            {
                                TxtY.Text = docHeight.Text;
                            }
                            else
                            {
                                TxtY.Text = FileDoc.LabelPdf.positions[3].PosY;
                            }
                            break;
                        default: // posizione personalizzata
                            SetImageBtnPositionSegnature(BtnPosLeft, urlImageBtnPosSegnature);
                            SetImageBtnPositionSegnature(BtnPosRight, urlImageBtnPosSegnature);
                            SetImageBtnPositionSegnature(BtnDownSx, urlImageBtnPosSegnature);
                            SetImageBtnPositionSegnature(BtnDownDx, urlImageBtnPosSegnature);
                            TxtX.Text = (from x in FileDoc.LabelPdf.positions where x.posName=="pos_pers" select x.PosX).FirstOrDefault();
                            TxtY.Text = (from x in FileDoc.LabelPdf.positions where x.posName=="pos_pers" select x.PosY).FirstOrDefault();;
                            break;
                    }

             if (FileDoc.LabelPdf.notimbro)
             {
                 this.NoSignatureStamp.Selected = true;
             }
             else
             {
                 if (!FileDoc.LabelPdf.tipoLabel)
                     this.SignatureRdlSignature.Selected = true;
                 else
                 {
                     if (FileDoc.LabelPdf.orientamento.Equals("verticale"))
                         this.SignatureRdlStampVertical.Selected = true;
                     else
                         this.SignatureRdlStampHorizontal.Selected = true;
                 }
             }

            }

        //public bool IsPermanentDisplaysSegnature
        //{
        //    get
        //    {
        //         return (NttDataWA.Utils.InitConfigurationKeys.GetValue("0", "FE_PERMANENT_DISPLAYS_SEGNATURE").Equals("1")) ? true : false;
        //    }
        //}

        protected void BtnSignaturePosition_Click(object sender, EventArgs e)
        {
            try {
                ImageButton buttonClick = (ImageButton)sender;
                string nameButtonClick = buttonClick.ID;
                switch(nameButtonClick)
                {
                    case "BtnPosLeft":
                        SetImageBtnPositionSegnature(BtnPosLeft, urlImageBtnPosSegnatureSelected);
                        SetImageBtnPositionSegnature(BtnPosRight, urlImageBtnPosSegnature);
                        SetImageBtnPositionSegnature(BtnDownSx, urlImageBtnPosSegnature);
                        SetImageBtnPositionSegnature(BtnDownDx, urlImageBtnPosSegnature);
                        positionSignature = "pos_upSx";
                        TxtX.Text = FileDoc.LabelPdf.positions[0].PosX;
                        TxtY.Text = FileDoc.LabelPdf.positions[0].PosY;
                        break;
                    case "BtnPosRight":
                        SetImageBtnPositionSegnature(BtnPosLeft, urlImageBtnPosSegnature);
                        SetImageBtnPositionSegnature(BtnPosRight, urlImageBtnPosSegnatureSelected);
                        SetImageBtnPositionSegnature(BtnDownSx, urlImageBtnPosSegnature);
                        SetImageBtnPositionSegnature(BtnDownDx, urlImageBtnPosSegnature);
                        positionSignature = "pos_upDx";
                        if (Convert.ToInt32(FileDoc.LabelPdf.positions[1].PosX) > Convert.ToInt32(docWidth.Text))
                             TxtX.Text = docWidth.Text;
                        else
                            TxtX.Text = FileDoc.LabelPdf.positions[1].PosX;
                        if (Convert.ToInt32(FileDoc.LabelPdf.positions[1].PosY) > Convert.ToInt32(docHeight.Text))
                            TxtY.Text = docHeight.Text;
                        else
                            TxtY.Text = FileDoc.LabelPdf.positions[1].PosY;
                        break;
                    case "BtnDownSx":
                        SetImageBtnPositionSegnature(BtnPosLeft, urlImageBtnPosSegnature);
                        SetImageBtnPositionSegnature(BtnPosRight, urlImageBtnPosSegnature);
                        SetImageBtnPositionSegnature(BtnDownSx, urlImageBtnPosSegnatureSelected);
                        SetImageBtnPositionSegnature(BtnDownDx, urlImageBtnPosSegnature);
                        positionSignature = "pos_downSx";
                        if (Convert.ToInt32(FileDoc.LabelPdf.positions[2].PosX) > Convert.ToInt32(docWidth.Text))
                            TxtX.Text = docWidth.Text;
                        else
                            TxtX.Text = FileDoc.LabelPdf.positions[2].PosX;
                            if (Convert.ToInt32(FileDoc.LabelPdf.positions[2].PosY) > Convert.ToInt32(docHeight.Text))
                                TxtY.Text = docHeight.Text;
                            else
                                TxtY.Text = FileDoc.LabelPdf.positions[2].PosY;
                        break;
                    case "BtnDownDx":
                        SetImageBtnPositionSegnature(BtnPosLeft, urlImageBtnPosSegnature);
                        SetImageBtnPositionSegnature(BtnPosRight, urlImageBtnPosSegnature);
                        SetImageBtnPositionSegnature(BtnDownSx, urlImageBtnPosSegnature);
                        SetImageBtnPositionSegnature(BtnDownDx, urlImageBtnPosSegnatureSelected);
                        positionSignature = "pos_downDx";
                        if (Convert.ToInt32(FileDoc.LabelPdf.positions[3].PosX) > Convert.ToInt32(docWidth.Text))
                            TxtX.Text = docWidth.Text;
                        else
                            TxtX.Text = FileDoc.LabelPdf.positions[3].PosX;
                        if (Convert.ToInt32(FileDoc.LabelPdf.positions[3].PosY) > Convert.ToInt32(docHeight.Text))
                            TxtY.Text = docHeight.Text;
                        else
                            TxtY.Text = FileDoc.LabelPdf.positions[3].PosY;
                        break;
                }
                FileDoc.LabelPdf.default_position = positionSignature;
                FileDoc = DocumentWithSignature(fileReq);
                this.ChangeSignature = true;
                UpdatePanelSignaturePosizionePesonalizzata.Update();
                UpdatePanelFrameSignature.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ColorList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try {
                colorSignature = DdlColour.SelectedIndex.ToString();
                FileDoc.LabelPdf.sel_color = colorSignature;
                FileDoc = DocumentWithSignature(fileReq);
                this.ChangeSignature = true;
                UpdatePanelFrameSignature.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CharacterList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try {
                characterSignature = DdlCharacter.SelectedIndex.ToString();
                FileDoc.LabelPdf.sel_font = characterSignature;
                FileDoc = DocumentWithSignature(fileReq);
                this.ChangeSignature = true;
                UpdatePanelFrameSignature.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void tbxPos_TextChanged(object sender, System.EventArgs e)
        {
            try {
                string msgDesc = string.Empty;
                if ((TxtX.Text != "" && isNan(TxtX)) && (TxtY.Text != "" && isNan(TxtY)))
                {
                    if ((Convert.ToInt32(TxtX.Text) < Convert.ToInt32(docWidth.Text)) && (Convert.ToInt32(TxtY.Text) < Convert.ToInt32(docHeight.Text)))
                    {
                        positionSignature = TxtX.Text + "-" + TxtY.Text;
                        FileDoc.LabelPdf.default_position = positionSignature;
                        FileDoc = DocumentWithSignature(fileReq);
                        SetImageBtnPositionSegnature(BtnPosLeft, urlImageBtnPosSegnature);
                        SetImageBtnPositionSegnature(BtnPosRight, urlImageBtnPosSegnature);
                        SetImageBtnPositionSegnature(BtnDownSx, urlImageBtnPosSegnature);
                        SetImageBtnPositionSegnature(BtnDownDx, urlImageBtnPosSegnature);
                        UpdatePanelcontentSegnatureBtn.Update();
                        this.ChangeSignature = true;
                        UpdatePanelFrameSignature.Update();
                    }
                    else
                    {
                        if ((Convert.ToInt32(TxtX.Text) > Convert.ToInt32(docWidth.Text)))
                        {

                           // msgDesc = "ErrorSignatureTxtX";
                            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorSignatureTxtX", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc + "', 'warning', '');}", true);
                            msgDesc = UIManager.LoginManager.GetMessageFromCode("ErrorSignatureTxtX", string.Empty);
                            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "<script>alert('" + msgDesc + "');</script>", false);
                            return;
                        }
                        if ((Convert.ToInt32(TxtY.Text) > Convert.ToInt32(docHeight.Text)))
                    
                        {
                            //msgDesc = "ErrorSignatureTxtY";
                            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorSignatureTxtY", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            msgDesc = UIManager.LoginManager.GetMessageFromCode("ErrorSignatureTxtY", string.Empty);
                            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "<script>alert('" + msgDesc + "');</script>", false);
                            return;
                        }
                    }
                }
                else
                {              
                    if (TxtX.Text == "" || TxtY.Text == "")
                    {
                        //msgDesc = "ErrorSignatureTxt";
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorSignatureTxt", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc + "', 'warning', '');}", true);
                        msgDesc = UIManager.LoginManager.GetMessageFromCode("ErrorSignatureTxt", string.Empty);
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "<script>alert('" + msgDesc + "');</script>", false);
                        return;
                    }
                    else
                        if (!isNan(TxtX) || !isNan(TxtY))
                        {
                            //msgDesc = "ErrorSignatureTxtValidate";
                            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorSignatureTxtValidate", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc + "', 'warning', '');}", true);
                            msgDesc = UIManager.LoginManager.GetMessageFromCode("ErrorSignatureTxtValidate", string.Empty);
                            ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "<script>alert('" + msgDesc + "');</script>", false);
                            return;
                        }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private static bool isNan(System.Web.UI.WebControls.TextBox tb)
        {
            bool result = false;

            string pattern = "^[0-9]*$";
            Regex NumberPattern = new Regex(pattern);
            result = NumberPattern.IsMatch(tb.Text);
            return result;

        }

        protected void RblOrientation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                typeLabel = false;
                orientationSignature = "false";
                string orientation = RblOrientation.SelectedValue.ToString();
                if (!orientation.Equals("signature") && !orientation.Equals(NoSignature))
                {
                    typeLabel = true;
                    orientationSignature = orientation;
                }
                else
                    if (orientation.Equals(NoSignature))
                    {
                        if (!UIManager.UserManager.IsAuthorizedFunctions("DO_ENABLE_SIGNATURE_INFO"))
                        {
                            this.ChkDigitalSignatureInfo.Checked = true;
                            this.RblDigitalSignaturePos.Enabled = true;
                            this.RblFormatSign.Enabled = true;
                            FileDoc.LabelPdf.digitalSignInfo = new labelPdfDigitalSignInfo();
                            FileDoc.LabelPdf.digitalSignInfo.printOnFirstPage = this.SignatureRblPrintOnFirstPage.Selected;
                            FileDoc.LabelPdf.digitalSignInfo.printOnLastPage = this.SignatureRblPrintOnLastPage.Selected;
                            FileDoc.LabelPdf.digitalSignInfo.printFormatSign = this.RblFormatSign.SelectedValue.Equals("digitalCompleted") ? DocsPaWR.TypePrintFormatSign.Sign_Extended : DocsPaWR.TypePrintFormatSign.Sign_Short;
                        }
                        NoTimbroTemp = true;
                        FileDoc.LabelPdf.notimbro = true;
                        this.UpdatePanelRblDigitalSignaturePos.Update();
                        FileDoc = DocumentWithSignature(fileReq);
                        this.ChangeSignature = true;
                        UpdatePanelFrameSignature.Update();
                        return;
                    }
                NoTimbroTemp = false;
                FileDoc.LabelPdf.tipoLabel = typeLabel;
                FileDoc.LabelPdf.orientamento = orientationSignature;
                FileDoc = DocumentWithSignature(fileReq);
                this.ChangeSignature = true;
                UpdatePanelFrameSignature.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void RblDigitalSignaturePos_Click(object sender, EventArgs e)
        {
            try {
                if (FileDoc.LabelPdf.digitalSignInfo != null)
                {
                    FileDoc.LabelPdf.digitalSignInfo.printOnFirstPage = this.SignatureRblPrintOnFirstPage.Selected;
                    FileDoc.LabelPdf.digitalSignInfo.printOnLastPage = this.SignatureRblPrintOnLastPage.Selected;
                    FileDoc.LabelPdf.digitalSignInfo.printFormatSign = this.RblFormatSign.SelectedValue.Equals("digitalCompleted") ? DocsPaWR.TypePrintFormatSign.Sign_Extended : DocsPaWR.TypePrintFormatSign.Sign_Short;
                }
                FileDoc = DocumentWithSignature(fileReq);
                this.ChangeSignature = true;
                UpdatePanelFrameSignature.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ChkSegnatureInfo_CheckedChanged(object sender, EventArgs e)
        {
            try {
                this.RblDigitalSignaturePos.Enabled = this.ChkDigitalSignatureInfo.Checked;
                this.RblFormatSign.Enabled = this.ChkDigitalSignatureInfo.Checked;
                if (ChkDigitalSignatureInfo.Checked)
                {
                    FileDoc.LabelPdf.digitalSignInfo = new labelPdfDigitalSignInfo();
                    FileDoc.LabelPdf.digitalSignInfo.printOnFirstPage = this.SignatureRblPrintOnFirstPage.Selected;
                    FileDoc.LabelPdf.digitalSignInfo.printOnLastPage = this.SignatureRblPrintOnLastPage.Selected;
                    FileDoc.LabelPdf.digitalSignInfo.printFormatSign = this.RblFormatSign.SelectedValue.Equals("digitalCompleted") ? DocsPaWR.TypePrintFormatSign.Sign_Extended : DocsPaWR.TypePrintFormatSign.Sign_Short;
                }
                else
                {
                    FileDoc.LabelPdf.digitalSignInfo = null;
                    if (!UIManager.UserManager.IsAuthorizedFunctions("DO_ENABLE_SIGNATURE_INFO") && RblOrientation.SelectedValue.Equals("noSignatureStamp"))
                    {
                        this.RblOrientation.SelectedValue = "signature";
                        NoTimbroTemp = false;
                    }
                    this.RblFormatSign.SelectedValue = "digitalCompleted";
                    this.RblDigitalSignaturePos.SelectedValue = "printOnFirstPage";
                    this.UpdatePanelRblOrientation.Update();
                }
                FileDoc = DocumentWithSignature(fileReq);
                this.ChangeSignature = true;
                UpdatePanelRblDigitalSignaturePos.Update();
                UpdatePanelFrameSignature.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SenderBtnClose_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session.Remove("OpenSignaturePopup");
            FileDoc.LabelPdf.default_position = PositionSignature;
            FileDoc.LabelPdf.sel_color = ColorSignature;
            FileDoc.LabelPdf.sel_font = CharacterSignature;
            FileDoc.LabelPdf.orientamento = OrientationSignature;
            FileDoc.LabelPdf.label_rotation = RotationSignature;
            FileDoc.LabelPdf.tipoLabel = TypeLabel;
            FileDoc.LabelPdf.notimbro = NoTimbro;
            if (FileDoc.LabelPdf.digitalSignInfo != null)
            {
                if (!PrintOnFirstPage && !PrintOnLastPage)
                {
                    this.ChkDigitalSignatureInfo.Checked = false;
                    FileDoc.LabelPdf.digitalSignInfo = null;
                }
                else
                {
                    FileDoc.LabelPdf.digitalSignInfo.printOnFirstPage = PrintOnFirstPage;
                    FileDoc.LabelPdf.digitalSignInfo.printOnLastPage = PrintOnLastPage;
                    FileDoc.LabelPdf.digitalSignInfo.printFormatSign = FormatSignature;
                }
            }
            else
            {
                if (PrintOnLastPage || PrintOnFirstPage)
                {
                    FileDoc.LabelPdf.digitalSignInfo = new labelPdfDigitalSignInfo();
                    FileDoc.LabelPdf.digitalSignInfo.printOnFirstPage = this.SignatureRblPrintOnFirstPage.Selected;
                    FileDoc.LabelPdf.digitalSignInfo.printOnLastPage = this.SignatureRblPrintOnLastPage.Selected;
                    FileDoc.LabelPdf.digitalSignInfo.printFormatSign = this.RblFormatSign.SelectedValue.Equals("digitalCompleted") ? DocsPaWR.TypePrintFormatSign.Sign_Extended : DocsPaWR.TypePrintFormatSign.Sign_Short;
                }
            }
            if (this.Request.QueryString["printSignatureA4"] == null)
                    Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('Signature','');</script></body></html>");
            else
                Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('SignatureA4','');</script></body></html>");
            HttpContext.Current.Session.Remove("NoTimbroTemp");
            Response.End();
        }


        protected void BtnConfirm_Click(object sender, EventArgs e)
        {
            string msgDesc = string.Empty;

            //Controllo se la coordinate per la posizione personalizzata sono inserite correttamente
            if (TxtX.Text == "" || TxtY.Text == "")
            {
                //msgDesc = "ErrorSignatureTxt";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorSignatureTxt", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc + "', 'warning', '');}", true);
                msgDesc = UIManager.LoginManager.GetMessageFromCode("ErrorSignatureTxt", string.Empty);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "<script>alert('" + msgDesc + "');</script>", false);
                return;
            }

            if (!isNan(TxtX) || !isNan(TxtY))
            {
                //msgDesc = "ErrorSignatureTxtValidate";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorSignatureTxtValidate", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc + "', 'warning', '');}", true);
                msgDesc = UIManager.LoginManager.GetMessageFromCode("ErrorSignatureTxtValidate", string.Empty);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "<script>alert('" + msgDesc + "');</script>", false);
                return;
            }

            if ((Convert.ToInt32(TxtX.Text) > Convert.ToInt32(docWidth.Text)))
            {

                // msgDesc = "ErrorSignatureTxtX";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorSignatureTxtX", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc + "', 'warning', '');}", true);
                msgDesc = UIManager.LoginManager.GetMessageFromCode("ErrorSignatureTxtX", string.Empty);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "<script>alert('" + msgDesc + "');</script>", false);
                return;
            }
            if ((Convert.ToInt32(TxtY.Text) > Convert.ToInt32(docHeight.Text)))
            {
                //msgDesc = "ErrorSignatureTxtY";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorSignatureTxtY", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                msgDesc = UIManager.LoginManager.GetMessageFromCode("ErrorSignatureTxtY", string.Empty);
                ScriptManager.RegisterStartupScript(this, typeof(Page), "Alert", "<script>alert('" + msgDesc + "');</script>", false);
                return;
            }

            PositionSignature = FileDoc.LabelPdf.default_position;
            ColorSignature = FileDoc.LabelPdf.sel_color;
            CharacterSignature = FileDoc.LabelPdf.sel_font;
            OrientationSignature = FileDoc.LabelPdf.orientamento;
            RotationSignature = FileDoc.LabelPdf.label_rotation;
            TypeLabel = FileDoc.LabelPdf.tipoLabel;
            NoTimbro = NoTimbroTemp;
            if (FileDoc.LabelPdf.digitalSignInfo != null)
            {
                PrintOnFirstPage = FileDoc.LabelPdf.digitalSignInfo.printOnFirstPage;
                PrintOnLastPage = FileDoc.LabelPdf.digitalSignInfo.printOnLastPage;
                if (fileReq.firmato == "1" ||  NoTimbroTemp)
                {
                    FormatSignature = FileDoc.LabelPdf.digitalSignInfo.printFormatSign;
                }                
            }
            else
                if (!this.ChkDigitalSignatureInfo.Checked)
                {
                    PrintOnFirstPage = false;
                    PrintOnLastPage = false;
                }
            //se ho richiesto di salvare il documento con impressa la segnatura o semplicemente di stampare il doc con segnatura ne tengo traccia nella sessione
            if (((Button)sender).ID.Equals("SignatureBtnSave"))
            {
                PermanentDisplaySignature = true;
                SavePermanentDisplaySignature = true;
            }
            HttpContext.Current.Session.Remove("NoTimbroTemp");
            SchedaDocSystemId = DocumentManager.getSelectedRecord().systemId;
            HttpContext.Current.Session.Remove("OpenSignaturePopup");
            this.ChangeSignature = true;
            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('Signature', 'up');</script></body></html>");
            Response.End();
        }

        private bool IsPdf(FileRequest file)
        {
            return (
                FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(PDF) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(TSD) ||
                FileManager.getEstensioneIntoSignedFile(file.fileName).Equals(M7M) ||
                (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.SHOW_DOCUMENT_AS_PDF_FORMAT.ToString()])
                && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.SHOW_DOCUMENT_AS_PDF_FORMAT.ToString()].Equals("1")
                && PdfConverterInfo.CanConvertFileToPdf(file.fileName)));

        }

	}
}