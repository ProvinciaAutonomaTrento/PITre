using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class HSM_Signature : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializeLanguage();
                this.InitializePage();

            }
        }

        private void InitializePage()
        {
            this.popolaCampiMemento();

            DocsPaWR.FileRequest fileReq = null;

            if (FileManager.GetSelectedAttachment() == null)
                fileReq = UIManager.FileManager.getSelectedFile();
            else
            {
                fileReq = FileManager.GetSelectedAttachment();
            }

            if (UIManager.UserManager.IsAuthorizedFunctions("TO_GET_OTP"))
            {
                this.BtnRequestOTP.Visible = true;
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString()).Equals("1"))
            {
                bool isPdf = (FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToUpper() == "PDF");

                if (!isPdf && !fileReq.firmato.Equals("1"))
                {
                    this.HsmCheckConvert.Checked = true;

                    if (UIManager.FileManager.IsEnabledSupportedFileTypes())
                    {
                        this.FileTypes = UIManager.FileManager.GetSupportedFileTypes(Int32.Parse(UIManager.UserManager.GetInfoUser().idAmministrazione));

                        bool retVal = true;

                        int count = this.FileTypes.Count(e => e.FileExtension.ToLowerInvariant() == FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToLowerInvariant() &&
                                                                e.FileTypeUsed && e.FileTypeSignature);
                        retVal = (count > 0);
                        //INC000000778865 - PROBLEMA CON MASCHERA DI FIRMA HSM
                        //this.HsmCheckConvert.Checked = retVal;
                    }
                }

                this.HsmCheckConvert.Enabled = false;
                
                setCofirma(fileReq.firmato == "1" ? true : false);

                //ABBATANGELI - nuova gestione sign/cosign
                //MEV LIBRO FIRMA
                //if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("1"))
                //{
                //    this.PnlTypeSign.Visible = false;
                //}
            }

            //se il documento è in libro firma, forzo la selezione al radio button relativo al tipo di firma richiesto
            if (fileReq.inLibroFirma)
            {
                this.HsmLitPades.Enabled = false;
                this.HsmLitP7M.Enabled = false;
                string typeSignature = LibroFirmaManager.GetTypeSignatureToBeEntered(fileReq);
                if (typeSignature.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES))
                {
                    this.HsmLitPades.Checked = true;
                    this.HsmLitP7M.Checked = false;
                }
                else
                {
                    this.HsmLitPades.Checked = false;
                    this.HsmLitP7M.Checked = true;
                }
            }
            //ABBATANGELI - Nuova gestione firma/cofirma
            //if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.LOCK_COFIRMA.ToString())) || Utils.InitConfigurationKeys.GetValue("0", DBKeys.LOCK_COFIRMA.ToString()).Equals("0"))
            //Valore della Chiave FE_SET_TIPO_FIRMA
            //  0: Annidata
            //  1: Parallela
            //  2: Annidata non modificabile
            //  3: Parallela non modificabile
            string setTipoFirma = string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SET_TIPO_FIRMA.ToString())) ? "0" : Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SET_TIPO_FIRMA.ToString());
            if (setTipoFirma.Equals("0") || setTipoFirma.Equals("2"))
            {
                this.optFirma.Checked = true;
                if (fileReq.firmato.Equals("1"))
                {
                    bool enabledRadio = setTipoFirma.Equals("0");
                    this.optCofirma.Enabled = enabledRadio;
                    this.optFirma.Enabled = enabledRadio;
                }
                else
                {
                    this.optCofirma.Enabled = false;
                    this.optFirma.Enabled = false;
                }
            }
            else
            {
                if (fileReq.firmato.Equals("1") && !fileReq.tipoFirma.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA))
                {
                    this.optCofirma.Checked = true;

                    bool enabledRadio = setTipoFirma.Equals("1");
                    this.optCofirma.Enabled = enabledRadio;
                    this.optFirma.Enabled = enabledRadio;
                }
                else
                {
                    this.optFirma.Checked = true;
                    this.optCofirma.Enabled = false;
                    this.optFirma.Enabled = false;
                }
            }

            this.TxtHsmPin.Focus();
        }

        //La cofirma si deve attivare SOLO se il file è già firmato CADES
        // il pdf (pades) non prevede la cofirma
        
        protected void setCofirma(bool isSigned)
        {
            //this.HsmRadioCoSign.Enabled = true;
            this.optCofirma.Enabled = true;
            
            if (!isSigned)
            {
                //this.HsmRadioCoSign.Enabled = false;
                this.optCofirma.Enabled = false;
            }

        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('HSMSignature', '');", true);
        }


        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnRequestOTP.Text = Utils.Languages.GetLabelFromCode("BtnRequestOTP", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("HSM_SignatureBtnClose", language);
            this.BtnSign.Text = Utils.Languages.GetLabelFromCode("HSM_SignatureBtnSign", language);
            this.HsmLitAlias.Text = Utils.Languages.GetLabelFromCode("HsmLitAlias", language);
            this.HsmLitDomain.Text = Utils.Languages.GetLabelFromCode("HsmLitDomain", language);
            this.HsmLitPin.Text = Utils.Languages.GetLabelFromCode("HsmLitPin", language);
            this.HsmLitOtp.Text = Utils.Languages.GetLabelFromCode("HsmLitOtp", language);
            this.HsmLitPades.Text = Utils.Languages.GetLabelFromCode("HsmLitPades", language);
            this.HsmLitP7M.Text = Utils.Languages.GetLabelFromCode("HsmLitP7M", language);
            this.HsmCheckMarkTemporal.Text = Utils.Languages.GetLabelFromCode("HsmCheckMarkTemporal", language);
            //this.HsmRadioSign.Text = Utils.Languages.GetLabelFromCode("HsmRadioSign", language);
            //this.HsmRadioCoSign.Text = Utils.Languages.GetLabelFromCode("HsmRadioCoSign", language);
            this.HsmCheckConvert.Text = Utils.Languages.GetLabelFromCode("HsmCheckConvert", language);

            this.optFirma.Text = Languages.GetLabelFromCode("DigitalSignSelector_OptFirma", language);
            this.optFirma.ToolTip = Languages.GetLabelFromCode("DigitalSignSelector_OptFirmaTo", language);
            this.optCofirma.Text = Languages.GetLabelFromCode("DigitalSignSelector_OptCofirma", language);
            this.optCofirma.ToolTip = Languages.GetLabelFromCode("DigitalSignSelector_OptCofirmaTo", language);
        }


        //andrebbe chiamato all'open della maschera se presenti i dati alias e dominio essi verrano PRE popolati
        private void popolaCampiMemento()
        {
            DigitalSignature.RemoteDigitalSignManager dsm = new DigitalSignature.RemoteDigitalSignManager();
            try
            {
                DigitalSignature.RemoteDigitalSignManager.Memento m = dsm.HSM_GetMementoForUser();
                if (m != null)
                {
                    this.TxtHsmAlias.Text = m.Alias;
                    this.TxtHsmDomain.Text = m.Dominio;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        //richiedo OTP 
        protected void BtnRequestOTP_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            DigitalSignature.RemoteDigitalSignManager dsm = new DigitalSignature.RemoteDigitalSignManager();
            if (!string.IsNullOrEmpty(this.TxtHsmAlias.Text) && !string.IsNullOrEmpty(this.TxtHsmDomain.Text))
            {
                string alias = this.TxtHsmAlias.Text;
                string dominio = this.TxtHsmDomain.Text;

                try
                {
                    DigitalSignature.RemoteDigitalSignManager.Memento m = new DigitalSignature.RemoteDigitalSignManager.Memento { Alias = alias, Dominio = dominio };
                    //Intanto salvo il memento
                    dsm.HSM_SetMementoForUser(m);
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return;
                }

                bool retval = false;
                try
                {
                    retval = dsm.HSM_RequestOTP(alias, dominio);
                }
                catch (System.Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorHsmSignOtp', 'error');} else {parent.ajaxDialogModal('ErrorHsmSignOtp', 'error');}", true);
                    return;
                }


                if (retval)
                {
                    //mandiamo in avviso che informa del successo dell'operazione e che tra pochi secondi riceverà l'opt sul telefonio
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('SuccessHsmRequestOTP', 'check');} else {parent.ajaxDialogModal('SuccessHsmRequestOTP', 'check');}", true);
                }
                else
                {
                    // è accaduto un inconveniente.. l'operazione è fallita.
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorHsmSignOtp', 'error');} else {parent.ajaxDialogModal('ErrorHsmSignOtp', 'error');}", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningHsmRequestOTPReq', 'warning');} else {parent.ajaxDialogModal('WarningHsmRequestOTPReq', 'warning');}", true);
            }

        }

        protected void BtnSign_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            if (!string.IsNullOrEmpty(this.TxtHsmAlias.Text) && !string.IsNullOrEmpty(this.TxtHsmDomain.Text) && !string.IsNullOrEmpty(this.TxtHsmPin.Text) && !string.IsNullOrEmpty(this.TxtHsmLitOtp.Text) && (this.HsmLitPades.Checked || this.HsmLitP7M.Checked))
            {

                string alias = this.TxtHsmAlias.Text;
                string dominio = this.TxtHsmDomain.Text;
                string pin = this.TxtHsmPin.Text;
                string otp = this.TxtHsmLitOtp.Text;

                DocsPaWR.FileRequest fileReq = null;

                if (FileManager.GetSelectedAttachment() == null)
                    fileReq = UIManager.FileManager.getSelectedFile();
                else
                {
                    fileReq = FileManager.GetSelectedAttachment();
                }

                bool isPdf = (FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToUpper() == "PDF");

                //if (!idPdf && this.HsmLitPades.Checked && !this.HsmCheckConvert.Checked)
                //{
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningHsmSignPDF', 'warning');} else {parent.ajaxDialogModal('WarningHsmSignPDF', 'warning');}", true);
                //}
                #region VERIFICA DIMENSIONE MASSIMA FILE
                int maxDimFileSign = 0;
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString())) &&
                   !Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString()).Equals("0"))
                    maxDimFileSign = Convert.ToInt32(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString()));
                if (maxDimFileSign > 0 && Convert.ToInt32(fileReq.fileSize) > maxDimFileSign)
                {
                    string maxSize = Convert.ToString(Math.Round((double)maxDimFileSign / 1048576, 3));
                    string msgDesc = "WarningStartProcessSignatureMaxDimFile";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + maxSize + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + maxSize + "');}", true);
                    return;
                }
                #endregion

                string msgError = CheckSign(fileReq, isPdf);
                if (!string.IsNullOrEmpty(msgError))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgError + "', 'warning');} else {parent.ajaxDialogModal('" + msgError + "', 'warning');}", true);
                }
                else
                {

                    DigitalSignature.RemoteDigitalSignManager dsm = new DigitalSignature.RemoteDigitalSignManager();

                    // attenzione mancano questi dati in maschera, vanno popolati come quelli sopra, prendendo i valori corretti.

                    //ABBATANGELI - Nuova gestione sign/cosign
                    //bool cofirma = this.HsmRadioCoSign.Checked; //prendere dalla checkbox cofirma
                    bool cofirma = this.optCofirma.Checked;
                    bool timestamp = this.HsmCheckMarkTemporal.Checked; //prendere dalla checkbox timestamp
                    DigitalSignature.RemoteDigitalSignManager.tipoFirma tipoFirma = new DigitalSignature.RemoteDigitalSignManager.tipoFirma();
                    if (this.HsmLitPades.Checked)
                    {
                        tipoFirma = DigitalSignature.RemoteDigitalSignManager.tipoFirma.PADES;
                    }
                    else
                    {
                        tipoFirma = DigitalSignature.RemoteDigitalSignManager.tipoFirma.CADES;
                        //Non posso firmare parallelemente un file firmato PADES con CADES, quindi impongo firma annidata
                        if (fileReq.firmato.Equals("1") && (fileReq.tipoFirma == TipoFirma.PADES || fileReq.tipoFirma == TipoFirma.PADES_ELETTORNICA))
                            cofirma = false;
                    }

                    try
                    {
                        DigitalSignature.RemoteDigitalSignManager.Memento m = new DigitalSignature.RemoteDigitalSignManager.Memento { Alias = alias, Dominio = dominio };
                        //Intanto salvo il memento
                        dsm.HSM_SetMementoForUser(m);
                    }
                    catch (System.Exception ex)
                    {
                        UIManager.AdministrationManager.DiagnosticError(ex);
                        return;
                    }

                    bool retval = false;
                    bool convert = !isPdf && this.HsmCheckConvert.Checked;
                    DocsPaWR.FirmaResult firmaResult = new FirmaResult();
                    try
                    {
                        //ABBATANGELI - Rischiesta sempre "cofirma" nel caso sia stata già apposta la prima firma
                        //if (fileReq.firmato == "1")
                        //    cofirma = true;

                        retval = dsm.HSM_Sign(fileReq, cofirma, timestamp, tipoFirma, alias, dominio, otp, pin, convert, out firmaResult);
                    }
                    catch (System.Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorHsmSignOtp', 'error');} else {parent.ajaxDialogModal('ErrorHsmSignOtp', 'error');}", true);
                        return;
                    }

                    if (retval)
                    {
                        //che famo se retval è true tocca refreschare la lista come nell amascera firma tradizionale
                        this.SetNewFile(fileReq);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('HSMSignature', 'up');", true);
                    }
                    else
                    {
                        // è accaduto un inconveniente.. la firma non è andata a buon fine...
                        string warningEsitoFirma = string.Empty;
                        string errorText = string.Empty;
                        if (firmaResult.esito != null && !string.IsNullOrEmpty(firmaResult.esito.Codice))
                            warningEsitoFirma = firmaResult.esito.Codice;

                        if(string.IsNullOrEmpty(warningEsitoFirma))
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorHsmSign', 'error', '', '" + errorText + "');} else {parent.ajaxDialogModal('ErrorHsmSign', 'error', '', '" + errorText + "');}", true);
                        else
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + warningEsitoFirma + "', 'warning');} else {parent.ajaxDialogModal('" + warningEsitoFirma + "', 'warning');}", true);

                    }
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningHsmSign', 'warning');} else {parent.ajaxDialogModal('WarningHsmSign', 'warning');}", true);
            }
        }

        protected void HsmLitPades_Change(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            DocsPaWR.FileRequest fileReq = null;

            if (FileManager.GetSelectedAttachment() == null)
                fileReq = UIManager.FileManager.getSelectedFile();
            else
            {
                fileReq = FileManager.GetSelectedAttachment();
            }

            bool isPdf = (FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToUpper() == "PDF");


            if (!this.HsmCheckConvert.Enabled && isPdf && fileReq.firmato.Equals("1"))
            {
                this.HsmCheckConvert.Checked = false;
            }
            else
            {


                if (UIManager.FileManager.IsEnabledSupportedFileTypes())
                {
                    this.FileTypes = UIManager.FileManager.GetSupportedFileTypes(Int32.Parse(UIManager.UserManager.GetInfoUser().idAmministrazione));

                    bool retVal = true;

                    int count = this.FileTypes.Count(f => f.FileExtension.ToLowerInvariant() == FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToLowerInvariant() &&
                                                            f.FileTypeUsed && f.FileTypeSignature);
                    retVal = (count > 0);

                    //INC000000778865 - PROBLEMA CON MASCHERA DI FIRMA HSM
                    //this.HsmCheckConvert.Checked = retVal;
                }

            }

            this.UpPnlSign.Update();
        }

        protected void HsmLitP7M_Change(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            DocsPaWR.FileRequest fileReq = null;

            if (FileManager.GetSelectedAttachment() == null)
                fileReq = UIManager.FileManager.getSelectedFile();
            else
            {
                fileReq = FileManager.GetSelectedAttachment();
            }

            bool isPdf = (FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToUpper() == "PDF");

            if (!this.HsmCheckConvert.Enabled && !isPdf)
            {
                if (UIManager.FileManager.IsEnabledSupportedFileTypes())
                {
                    this.FileTypes = UIManager.FileManager.GetSupportedFileTypes(Int32.Parse(UIManager.UserManager.GetInfoUser().idAmministrazione));

                    bool retVal = true;

                    int count = this.FileTypes.Count(f => f.FileExtension.ToLowerInvariant() == FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToLowerInvariant() &&
                                                            f.FileTypeUsed && f.FileTypeSignature);
                    retVal = (count > 0);

                    //INC000000778865 - PROBLEMA CON MASCHERA DI FIRMA HSM
                    //this.HsmCheckConvert.Checked = retVal;
                }
            }

            this.UpPnlSign.Update();
        }

        private void SetNewFile(FileRequest fileReq)
        {
            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            DocumentManager.setSelectedNumberVersion("0");
            DocumentManager.ListDocVersions = DocumentManager.getDocumentListVersions(this.Page, fileReq.docNumber, fileReq.docNumber).documenti;
            if (DocumentManager.ListDocVersions != null && DocumentManager.ListDocVersions.Length > 0)
            {
                if (DocumentManager.getSelectedNumberVersion().Equals("0"))
                    DocumentManager.setSelectedNumberVersion((from docs in DocumentManager.ListDocVersions select Convert.ToInt32(docs.version)).Max().ToString());
            }
            if (UIManager.DocumentManager.getSelectedAttachId() != null)
            {
                FileRequest filRequestNew = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
                FileManager.aggiornaFileRequest(this.Page, filRequestNew, false);
            }
            else
            {
                doc.documenti = DocumentManager.ListDocVersions;// = DocumentManager.getDocumentListVersions(this.Page, fileReq.docNumber, fileReq.docNumber).documenti;
                UIManager.DocumentManager.setSelectedRecord(doc);
            }

            //doc.documenti = DocumentManager.ListDocVersions = DocumentManager.getDocumentListVersions(this.Page, fileReq.docNumber, fileReq.docNumber).documenti;
            //UIManager.DocumentManager.setSelectedRecord(doc);

            ///
            //DocsPaWR.InfoUtente infoUser = UserManager.GetInfoUser();



            //FileRequest fileDoc = ((from v in DocumentManager.ListDocVersions select Convert.ToInt32(v.version)).Max().ToString());

            //if (DocumentManager.getSelectedAttachId() != null) //nel caso di allegato 
            //{
            //    DocumentManager.setSelectedAttachId(fileReq.versionId);
            //}

            FileRequest fileDoc = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
            UIManager.FileManager.setSelectedFile(fileDoc);
            if (DocumentManager.getSelectedAttachId() != null) //nel caso di allegato 
            {
                DocumentManager.setSelectedAttachId(fileDoc.versionId);
            }
        }

        //protected string GetFileExtension()
        //{
        //    NttDataWA.DocsPaWR.FileRequest fileRequest = NttDataWA.UIManager.FileManager.getSelectedFile(this);

        //    if (fileRequest != null)
        //    {
        //        System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileRequest.fileName);
        //        return fileInfo.Extension.ToLower();
        //    }
        //    else
        //        return string.Empty;
        //}

        /// <summary>
        /// 
        /// </summary>
        protected bool IsEnabledConvPDFSincrona
        {
            get
            {
                return new NttDataWA.DocsPaWR.DocsPaWebService().IsEnabledConversionePdfLatoServerSincrona();
                //return true;
            }
        }

        /// <summary>
        /// Tipi di file
        /// </summary>
        private SupportedFileType[] FileTypes
        {
            get
            {
                if (this.ViewState["SupportedFileType"] == null)
                    return null;
                else
                    return this.ViewState["SupportedFileType"] as SupportedFileType[];
            }
            set
            {
                if (this.ViewState["SupportedFileType"] == null)
                    this.ViewState.Add("SupportedFileType", value);
                else
                    this.ViewState["SupportedFileType"] = value;
            }
        }

        private string CheckSign(FileRequest fileReq, bool isPdf)
        {
            string msgError = string.Empty;

            #region FIRMA PADES
            
            //Controllo che il file che stò firmando è un PDF
            //if(this.HsmRadioSign.Checked)
            if (this.optFirma.Checked)
            {
                if (!isPdf && this.HsmLitPades.Checked && !this.HsmCheckConvert.Checked)
                {
                    msgError = "WarningHsmSignPDF";
                    return msgError;
                }
            }

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileReq.fileName);
            bool isCadesSign = (fileInfo != null && !String.IsNullOrEmpty(fileInfo.Extension) && fileInfo.Extension.ToLower().Equals(".p7m"));
            //bool isCadesSign = (!String.IsNullOrEmpty(fileReq.tipoFirma) && fileReq.tipoFirma.Equals(TipoFirma.CADES)) || (fileInfo != null && !String.IsNullOrEmpty(fileInfo.Extension) && fileInfo.Extension.ToLower().Equals(".p7m"));

            if (this.HsmLitPades.Checked && isCadesSign)
            {
                msgError = "WarningHsmSignPades";
                return msgError;
            }
            #endregion

            #region COFIRMA
            //Non posso firmare parallelemente un file firmato PADES con CADES, quindi impongo firma annidata
            //La cofirma può essere solo di tipo CADES e applicabile so su file firmato CADES

            //if (this.HsmRadioCoSign.Checked)
            //if (this.optCofirma.Checked)
            //{
            //    //Non posso cofirmare PADES
            //    if (this.HsmLitPades.Checked)
            //    {
            //        msgError = "WarningHsmCoSignPades";
            //        return msgError;
            //    }
            //    else if(this.HsmLitP7M.Checked && (System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf")))
            //    {
            //        //Non posso firmare CADES un file firmato PADES :se l'estensione del file è PDF, il file è stato firmato PADES
            //        msgError = "WarningHsmCoSignCades";
            //        return msgError;
            //    }
            //}

            #endregion

            #region FORMATO AMMESSO ALLA FIRMA

            if(!this.HsmCheckConvert.Checked && UIManager.FileManager.IsEnabledSupportedFileTypes())
            {
                string extensionFile = (fileReq.fileName.Split('.').Length > 1) ? (fileReq.fileName.Split('.'))[fileReq.fileName.Split('.').Length - 1] : string.Empty;
                this.FileTypes = UIManager.FileManager.GetSupportedFileTypes(Int32.Parse(UIManager.UserManager.GetInfoUser().idAmministrazione));

                bool retVal = true;

                int count = FileTypes.Count(e => e.FileExtension.ToLowerInvariant() == extensionFile.ToLowerInvariant() &&
                                                            e.FileTypeUsed && e.FileTypeSignature);

                retVal = (count > 0);
                if(!retVal)
                {
                    msgError = "WarningFormatoNonAmmessoAllaFirma";
                }
            }


            #endregion



            return msgError;
        }
    }
}