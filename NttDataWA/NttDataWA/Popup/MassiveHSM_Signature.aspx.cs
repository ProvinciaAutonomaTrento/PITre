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
    public partial class MassiveHSM_Signature : System.Web.UI.Page
    {

        #region Properties

        public static string CommandType = "";

        protected Dictionary<String, String> ListCheck
        {
            get
            {
                Dictionary<String, String> result = null;
                if (HttpContext.Current.Session["listCheck"] != null)
                {
                    result = HttpContext.Current.Session["listCheck"] as Dictionary<String, String>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listCheck"] = value;
            }
        }


        /// <summary>
        /// Lista delle informazioni sui documenti da firmare
        /// </summary>
        private List<SchedaDocumento> SchedaDocumentList
        {
            get
            {
                return HttpContext.Current.Session["SchedaDocumentList"] as List<SchedaDocumento>;
            }
            set
            {
                HttpContext.Current.Session["SchedaDocumentList"] = value;
            }
        }

        private bool IsEnabledSupportedFileTypes
        {
            get
            {
                if (HttpContext.Current.Session["IsEnabledSupportedFileTypes"] != null)
                    return (bool)HttpContext.Current.Session["IsEnabledSupportedFileTypes"];
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["IsEnabledSupportedFileTypes"] = value;
            }
        }

        private int MaxDimFileSign
        {
            get
            {
                if (HttpContext.Current.Session["MaxDimFileSign"] != null)
                    return (int)HttpContext.Current.Session["MaxDimFileSign"];
                else
                    return 0;
            }
            set
            {
                HttpContext.Current.Session["MaxDimFileSign"] = value;
            }
        }

        private bool IsLF
        {
            get
            {
                string daLibroFirma = Request.QueryString["LF"];
                if (string.IsNullOrEmpty(daLibroFirma))
                    return false;
                else
                    return Request.QueryString["LF"].Equals("1") ? true : false;
            }
        }

        private bool FirmaAnnidata
        {
            get
            {
                if (HttpContext.Current.Session["MassiveHSM_SignatureFirmaAnnidata"] != null)
                    return (bool)HttpContext.Current.Session["MassiveHSM_SignatureFirmaAnnidata"];
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["MassiveHSM_SignatureFirmaAnnidata"] = value;
            }
        }

        private bool Continue
        {
            get
            {
                if (HttpContext.Current.Session["MassiveHSM_SignatureContinue"] != null)
                    return (bool)HttpContext.Current.Session["MassiveHSM_SignatureContinue"];
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["MassiveHSM_SignatureContinue"] = value;
            }
        }

        protected Dictionary<String, FileToSign> ListToSign
        {
            get
            {
                Dictionary<String, FileToSign> result = null;
                if (HttpContext.Current.Session["listToSign"] != null)
                {
                    result = HttpContext.Current.Session["listToSign"] as Dictionary<String, FileToSign>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listToSign"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializeLanguage();

                if (HttpContext.Current.Session["CommandType"] != null)
                {
                    CommandType = HttpContext.Current.Session["CommandType"].ToString();
                }
                else
                    CommandType = "";

                this.LoadKeys();
                this.InitializeList();
                this.InitializePage();

            }
        }

        private void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString())) &&
                !Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString()).Equals("0"))
                this.MaxDimFileSign = Convert.ToInt32(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString()));
        }

        public void InitializeList()
        {
            Dictionary<String, MassiveOperationTarget> temp = new Dictionary<string, MassiveOperationTarget>();

            // Inizializzazione della mappa con i system id degli oggetti e lo stato
            // di checking (in fase di inizializzazione tutti gli item sono deselezionati)
            if (CommandType != "close")
            {
                foreach (KeyValuePair<string, string> item in this.ListCheck)
                    if (!temp.Keys.Contains(item.Key))
                        temp.Add(item.Key, new MassiveOperationTarget(item.Key, item.Value));

                // Salvataggio del dizionario
                MassiveOperationUtils.ItemsStatus = temp;
            }
        }


        private void InitializePage()
        {
            if (CommandType != "close")
            {
                this.FirmaAnnidata = false;
                this.Continue = false;
                this.popolaCampiMemento();
                if (UIManager.UserManager.IsAuthorizedFunctions("TO_GET_OTP"))
                {
                    this.BtnRequestOTP.Visible = true;
                }
               
                //MEV LIBRO FIRMA
                //if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("1"))
                //{
                //    this.HsmRadioCoSign.Visible = false;
                //}
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
                    this.HsmRadioSign.Checked = true;
                    this.HsmRadioCoSign.Enabled = true;
                    this.HsmRadioSign.Enabled = true;
                }
                else
                {
                    //forzaCofirma = true;
                    this.HsmRadioCoSign.Checked = true;
                    this.HsmRadioCoSign.Enabled = false;
                    this.HsmRadioSign.Enabled = false;
                }
                bool enableChangeRadio = setTipoFirma.Equals("0") || setTipoFirma.Equals("1");
                this.HsmRadioCoSign.Enabled = enableChangeRadio;
                this.HsmRadioSign.Enabled = enableChangeRadio;
                if (IsLF)
                {
                    //se la popup è aperta dal libro firma, posso firmare cades e pades.
                    //Qual'ora presenti entrambi firmo prima CADES e poi PADES. Conclusa CADES, la maschera non viene chiusa, ma verrà chiesto
                    //un nuovo OTP per procedere con firma PADES. Al termine di quest'ultima, se ci sono stati errori verrà mostrato
                    //il report altrimenti verrà chiusa la popup.
                    this.HsmLitP7M.Enabled = false;
                    this.HsmLitPades.Enabled = false;
                    List<MassiveOperationTarget> selectedItemSystemIdList = MassiveOperationUtils.GetSelectedItems();
                    if (selectedItemSystemIdList != null && selectedItemSystemIdList.Count > 0)
                    {
                        List<string> idDocumentList = (from temp in selectedItemSystemIdList select temp.Id).ToList<string>();
                        bool toSignCades = (from id in idDocumentList where id.Contains("C") select id).FirstOrDefault() != null;
                        if (toSignCades)
                        {
                            this.HsmLitP7M.Checked = true;
                            this.HsmLitPades.Checked = false;
                        }
                        else
                        {
                            this.HsmLitP7M.Checked = false;
                            this.HsmLitPades.Checked = true;
                        }
                    }
                }
                this.TxtHsmPin.Focus();
            }
        }


        protected void BtnClose_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CommandType"] = "close";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            CloseMask(false);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";

            if (IsLF)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "parent.closeAjaxModal('MassiveSignatureHSM', '" + retValue + "', parent);", true);
            // ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveSignatureHSM', '');", true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveSignatureHSM', '" + retValue + "');", true);
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
            this.HsmRadioSign.Text = Utils.Languages.GetLabelFromCode("HsmRadioSign", language);
            this.HsmRadioCoSign.Text = Utils.Languages.GetLabelFromCode("HsmRadioCoSign", language);
            this.BtnReport.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
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

            if (PutHSMSign() && IsLF)
            {
                HttpContext.Current.Session["CommandType"] = "close";
                this.CloseMask(true);
            }

        }

        private bool PutHSMSign()
        {
            bool retVal = false;
            string errorMsg = string.Empty;
            if (!string.IsNullOrEmpty(this.TxtHsmAlias.Text) && !string.IsNullOrEmpty(this.TxtHsmDomain.Text) && !string.IsNullOrEmpty(this.TxtHsmPin.Text) && !string.IsNullOrEmpty(this.TxtHsmLitOtp.Text) && (this.HsmLitPades.Checked || this.HsmLitP7M.Checked))
            {

                string alias = this.TxtHsmAlias.Text;
                string dominio = this.TxtHsmDomain.Text;
                string pin = this.TxtHsmPin.Text;
                string otp = this.TxtHsmLitOtp.Text;

                try
                {
                    DigitalSignature.RemoteDigitalSignManager dsm = new DigitalSignature.RemoteDigitalSignManager();
                    DigitalSignature.RemoteDigitalSignManager.Memento m = new DigitalSignature.RemoteDigitalSignManager.Memento { Alias = alias, Dominio = dominio };
                    //Intanto salvo il memento
                    dsm.HSM_SetMementoForUser(m);
                }
                catch (System.Exception ex)
                {
                    return true;
                }

                MassiveOperationReport.MassiveOperationResultEnum result;
                string codice = string.Empty;

                // Il dettaglio dell'elaborazione per un documento
                string details;

                MassiveOperationReport report = new MassiveOperationReport();
                if ((IsLF || Continue) && HttpContext.Current.Session["massiveSignReport"] != null)
                {
                    report = HttpContext.Current.Session["massiveSignReport"] as MassiveOperationReport;
                }

                //Lista dei fileReequest da passare in input al servizio di firma massiva
                List<FileRequest> fileRequestList = new List<FileRequest>();
                //List<FileRequest> listFileReqLF = new List<FileRequest>();
                this.IsEnabledSupportedFileTypes = UIManager.FileManager.IsEnabledSupportedFileTypes();
                if(this.IsEnabledSupportedFileTypes)
                    this.FileTypes = UIManager.FileManager.GetSupportedFileTypes(Int32.Parse(UIManager.UserManager.GetInfoUser().idAmministrazione));

                string objType = Request.QueryString["objType"];
                if (objType.Equals("D"))
                    this.SchedaDocumentList = this.LoadSchedaDocumentsList(
                        MassiveOperationUtils.GetSelectedItems());

                // Invio della trasmissione per ogni documento da inviare
                foreach (SchedaDocumento schedaDoc in this.SchedaDocumentList)
                {
                    result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                    details = String.Empty;

                    // Verifica della possibilità di firmare il documento
                    result = this.CanSignDocument(schedaDoc, out details);

                    if (result != MassiveOperationReport.MassiveOperationResultEnum.KO)
                    {
                        fileRequestList.Add(schedaDoc.documenti[0]);
                    }

                    // Aggiunta di una riga al report
                    if (result == MassiveOperationReport.MassiveOperationResultEnum.KO)
                    {
                        codice = MassiveOperationUtils.getItem(schedaDoc.docNumber).Codice;
                        report.AddReportRow(
                            codice,
                            result,
                            details);
                    }
                }
                //PER TUTTI I DOCUMENTI CHE HANNO SUPERATO I CONTROLLI VADO AD APPLICARE LA FIRMA
                if (fileRequestList.Count > 0)
                {
                    
                    DigitalSignature.RemoteDigitalSignManager dsm = new DigitalSignature.RemoteDigitalSignManager();

                    bool cofirma = this.HsmRadioCoSign.Checked; //prendere dalla checkbox cofirma

                    //ABBATANGELI - Hanno cambiato idea nuovamente e non è sempre cosign ma prende il valore dal checkbox cofirma
                    ////ABBATANGELI - PITre richiede sempre la cofirma per hsm
                    ////cofirma = true;


                    bool timestamp = this.HsmCheckMarkTemporal.Checked; //prendere dalla checkbox timestamp
                    DigitalSignature.RemoteDigitalSignManager.tipoFirma tipoFirma = new DigitalSignature.RemoteDigitalSignManager.tipoFirma();
                    if (this.HsmLitPades.Checked)
                    {
                        tipoFirma = DigitalSignature.RemoteDigitalSignManager.tipoFirma.PADES;
                    }
                    else
                    {
                        tipoFirma = DigitalSignature.RemoteDigitalSignManager.tipoFirma.CADES;
                        cofirma = this.HsmRadioCoSign.Checked && !FirmaAnnidata;
                    }
                    List<FirmaResult> firmaResult = null;
                    try
                    {
                        firmaResult = dsm.HSM_SignMultiSign(fileRequestList.ToArray(), cofirma, timestamp, tipoFirma, alias, dominio, otp, pin).ToList<FirmaResult>();
                        /*Commento perchè la distinzione tra documenti che devo firmare cades e quelli da firmare pades per il libro firma, la faccio prima, quando vado a selezionare SchedaDocumentList
                         * if (!IsLF)
                        {
                            firmaResult = dsm.HSM_SignMultiSign(fileRequestList.ToArray(), cofirma, timestamp, tipoFirma, alias, dominio, otp, pin).ToList<FirmaResult>();
                        }
                        else
                        {
                            List<MassiveOperationTarget> selectedItemSystemIdList = MassiveOperationUtils.GetSelectedItems();
                            string typeSign = tipoFirma.Equals(DigitalSignature.RemoteDigitalSignManager.tipoFirma.CADES) ? "C" : "P";
                            List<string> idDocumentList = (from temp in selectedItemSystemIdList where temp.Id.Contains(typeSign) select temp.Id.Replace(typeSign, "")).ToList<string>();                            
                            listFileReqLF = (from f in fileRequestList where idDocumentList.Contains(f.docNumber) select f).ToList();
                            if (listFileReqLF != null && listFileReqLF.Count > 0)
                            {
                                firmaResult = dsm.HSM_SignMultiSign(listFileReqLF.ToArray(), cofirma, timestamp, tipoFirma, alias, dominio, otp, pin).ToList<FirmaResult>();
                            }
                        }
                        */
                        if (firmaResult != null && ((firmaResult.Count > 1) || (firmaResult.Count == 1 && firmaResult[0].fileRequest != null)))
                        {
                            foreach (FirmaResult r in firmaResult)
                            {
                                string[] splitMsg = r.errore.Split(':');
                                if (splitMsg[0].Equals("true"))
                                {
                                    result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                                    details = "Firma HSM del documento avvenuta con successo";
                                    codice = MassiveOperationUtils.getItem(r.fileRequest.docNumber).Codice;
                                    report.AddReportRow(
                                        codice,
                                        result,
                                        details);
                                }
                                else
                                {
                                    if (r.esito == null || string.IsNullOrEmpty(r.esito.Codice))
                                        errorMsg = String.Format(
                                        "Si sono verificati degli errori durante la firma del documento. Dettagli: {0}",
                                        splitMsg[1]);
                                    else
                                        errorMsg = Utils.Languages.GetMessageFromCode(r.esito.Codice, UserManager.GetLanguageData());

                                    result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                                    codice = MassiveOperationUtils.getItem(r.fileRequest.docNumber).Codice;
                                    details = errorMsg;
                                    report.AddReportRow(
                                        codice,
                                        result,
                                        details);
                                }

                            }
                        }
                        else
                        {
                            //List<FileRequest> list = IsLF ? listFileReqLF : fileRequestList;
                            string error = "Si sono verificati degli errori durante la firma del documento";
                            if (firmaResult != null && firmaResult.Count > 0)
                                error += ": " + firmaResult[0].errore;
                            if(firmaResult[0].esito != null && !string.IsNullOrEmpty(firmaResult[0].esito.Codice))
                                error = Utils.Languages.GetMessageFromCode(firmaResult[0].esito.Codice, UserManager.GetUserLanguage());
                            foreach (FileRequest fr in fileRequestList)
                            {
                                result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                                codice = MassiveOperationUtils.getItem(fr.docNumber).Codice;
                                details = error;
                                report.AddReportRow(
                                    codice,
                                    result,
                                    details);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //List<FileRequest> list = IsLF ? listFileReqLF : fileRequestList;
                        foreach (FileRequest fr in fileRequestList)
                        {
                            result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                            codice = MassiveOperationUtils.getItem(fr.docNumber).Codice;
                            details = "Si sono verificati degli errori durante la firma HSM del documento.";
                            report.AddReportRow(
                                codice,
                                result,
                                details);
                        }
                    }
                }
                if(Continue)
                {
                    HttpContext.Current.Session["massiveSignReport"] = report;

                    //Se ho firmato CADES e ci sono documenti da firmare PADES non chiudo la maschera ma visualizzo un worning che
                    //informa l'utente di inserire un nuovo otp per procedere con la firma
                    this.TxtHsmLitOtp.Text = string.Empty;
                    this.HsmLitP7M.Enabled = false;
                    this.HsmLitPades.Enabled = false;
                    this.HsmRadioCoSign.Enabled = false;
                    this.UpOTP.Update();
                    this.UpPnlSign.Update();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningRequestNewOTPFirmaAnnidata', 'warning');} else {parent.ajaxDialogModal('WarningRequestNewOTPFirmaAnnidata', 'warning');}", true);
                    return false;
                }

                if (!IsLF)
                {
                    // Introduzione della riga di summary
                    string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
                    report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
                    this.generateReport(report, "Firma HSM massiva");
                }
                else
                {
                    HttpContext.Current.Session["massiveSignReport"] = report;

                    //Se ho firmato CADES e ci sono documenti da firmare PADES non chiudo la maschera ma visualizzo un worning che
                    //informa l'utente di inserire un nuovo otp per procedere con la firma
                    List<MassiveOperationTarget> selectedItemSystemIdList = MassiveOperationUtils.GetSelectedItems();
                    bool toSignPades = (from s in selectedItemSystemIdList where s.Id.Contains("P") select s).FirstOrDefault() != null;
                    if (this.HsmLitP7M.Checked && toSignPades)
                    {
                        this.HsmLitP7M.Checked = false;
                        this.HsmLitPades.Checked = true;
                        this.TxtHsmLitOtp.Text = string.Empty;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningRequestNewOTP', 'warning');} else {parent.ajaxDialogModal('WarningRequestNewOTP', 'warning');}", true);
                        this.UpPnlSign.Update();
                        this.UpOTP.Update();
                        return false;
                    }
                }

                retVal = true;
            }
            else
            {
                retVal = false;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningHsmSign', 'warning');} else {parent.ajaxDialogModal('WarningHsmSign', 'warning');}", true);
            }

            return retVal;
        }

        /// <summary>
        /// Funzione per verificare se è possibile applicare la firma HSM sul documento
        /// </summary>
        /// <param name="docInfo"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        private MassiveOperationReport.MassiveOperationResultEnum CanSignDocument(SchedaDocumento schedaDoc, out string details)
        {
            // Risultato della verifica
            MassiveOperationReport.MassiveOperationResultEnum retValue = MassiveOperationReport.MassiveOperationResultEnum.OK;
            System.Text.StringBuilder detailsBS = new System.Text.StringBuilder();

            string msgError = CheckSign(schedaDoc);

            if (!string.IsNullOrEmpty(msgError))
            {
                retValue = MassiveOperationReport.MassiveOperationResultEnum.KO;
                detailsBS.Append(msgError);
            }

            details = detailsBS.ToString();
            return retValue;

        }

        private void generateReport(MassiveOperationReport report, string titolo)
        {
            this.grdReport.DataSource = report.GetDataSet();
            this.grdReport.DataBind();
            this.pnlReport.Visible = true;
            this.upReport.Update();

            string template = "../xml/massiveOp_formatPdfExport.xml";
            report.GenerateDataSetForExport(Server.MapPath(template), titolo);

            this.BtnSign.Enabled = false;
            this.BtnRequestOTP.Enabled = false;
            this.BtnReport.Visible = true;
            this.UpPnlButtons.Update();
        }

        protected void BtnReport_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "reallowOp", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "visualReport", "parent.ajaxModalPopupMassiveReport();", true);
        }


        /// <summary>
        /// Funzione per il caricamento delle informazioni sui documenti
        /// </summary>
        /// <param name="selectedItemSystemIdList">La lista dei system id degli elementi selezionati</param>
        /// <returns>La lista degli id dei documenti selezionati</returns>
        private List<SchedaDocumento> LoadSchedaDocumentsList(List<MassiveOperationTarget> selectedItemSystemIdList)
        {
            // La lista da restituire
            List<SchedaDocumento> toReturn = new List<SchedaDocumento>();

            if (selectedItemSystemIdList != null && selectedItemSystemIdList.Count > 0)
            {
                //Se è il Libro firma seleziono solo gli id documenti che sono da firmare del tipo specificato nel radio
                DigitalSignature.RemoteDigitalSignManager.tipoFirma tipoFirma = new DigitalSignature.RemoteDigitalSignManager.tipoFirma();
                tipoFirma = this.HsmLitPades.Checked ? DigitalSignature.RemoteDigitalSignManager.tipoFirma.PADES : DigitalSignature.RemoteDigitalSignManager.tipoFirma.CADES;
                List<string> idDocumentList = new List<string>();

                //Firma PARALLELA: non posso firmare parallelamente CADES file non firmati o file PADES, quindi la firma verrà applicati in blocchi saparati
                //e varrà richiesto di inserire un nuovo OTP.
                if (this.CheckDominioFirmaForzaAnnidata() && this.HsmRadioCoSign.Checked && tipoFirma.Equals(DigitalSignature.RemoteDigitalSignManager.tipoFirma.CADES))
                {
                    List<MassiveOperationTarget> selectedItemSystemIdListTemp = null;
                    if (!this.Continue)
                    {
                        //Estraggo per la firma file firmati cades su cui posso applicare firma parallela(non posso fare parallela su CADES)
                        selectedItemSystemIdListTemp = (from item in selectedItemSystemIdList where IsDocFirmatoCADES(item.Id) select item).ToList();
                        if (selectedItemSystemIdListTemp != null && selectedItemSystemIdListTemp.Count > 0)
                        {
                            //Controllo se sono presenti anche file non firmati o firmati PADES, in caso l'utente dovrà reinserire l'otp per poi procedere di nuovo con la firma
                            this.Continue = (from item in selectedItemSystemIdList where !IsDocFirmatoCADES(item.Id) select item.Id).FirstOrDefault() != null;
                            selectedItemSystemIdList = selectedItemSystemIdListTemp;
                        }
                        else //se sono presenti solo file non firmati o PADES forzo subito Annidata
                        {
                            this.FirmaAnnidata = true;
                        }
                    }
                    else
                    {
                        //Estraggo per la firma i file mai firmati O firmati CADES
                        selectedItemSystemIdListTemp = (from item in selectedItemSystemIdList where !IsDocFirmatoCADES(item.Id) select item).ToList();
                        selectedItemSystemIdList = selectedItemSystemIdListTemp;
                        this.FirmaAnnidata = true;
                        this.Continue = false;
                    }
                }

                if (!IsLF)
                {
                    idDocumentList = (from temp in selectedItemSystemIdList select temp.Id.Replace("C", "").Replace("P", "")).ToList<string>();
                }
                else
                {
                    string typeSign = tipoFirma.Equals(DigitalSignature.RemoteDigitalSignManager.tipoFirma.CADES) ? "C" : "P";
                    idDocumentList = (from temp in selectedItemSystemIdList where temp.Id.Contains(typeSign) select temp.Id.Replace(typeSign, "")).ToList<string>();
                } 

                toReturn = DocumentManager.GetSchedaDocuments(idDocumentList, this);
            }

            // Restituzione della lista di info documento
            return toReturn;

        }

        /// <summary>
        /// Restituisce true se per il doiminio indicato è necessario forzare la firma annidata
        /// </summary>
        /// <returns></returns>
        public bool CheckDominioFirmaForzaAnnidata()
        {
            bool result = false;
            string domini = Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_DOMINI_HSM_FORZA_ANNIDATA.ToString());
            if (!string.IsNullOrEmpty(domini))
            {
                string[] split = domini.Split(';');
                foreach (string s in split)
                    if (s.ToLower().Equals(this.TxtHsmDomain.Text.ToLower()))
                        return true;
            }
            return result;
        }

        public bool IsDocFirmatoCADES(string idDocumento)
        {
            bool result = true;
            FileToSign file = null;
            try
            {
                if (!String.IsNullOrEmpty(idDocumento))
                {
                    if (this.ListToSign != null && this.ListToSign.ContainsKey(idDocumento))
                    {
                        file = this.ListToSign[idDocumento];
                        result = file.signed.Equals("1") && !file.signType.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA) 
                            && !file.signType.Equals(NttDataWA.Utils.TipoFirma.PADES) && !file.signType.Equals(NttDataWA.Utils.TipoFirma.PADES_ELETTORNICA);
                    }
                }
            }
            catch(Exception e)
            {

            }
            return result;
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

        private string CheckSign(SchedaDocumento schedaDoc)
        {
            string msgError = string.Empty;

            FileRequest fileReq = schedaDoc.documenti[0];
            bool isPdf = (FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToUpper() == "PDF");
            string estenxione = FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToLowerInvariant();
            #region FILE NON ACQUISITO
            if (string.IsNullOrEmpty(fileReq.fileSize) || int.Parse(fileReq.fileSize) == 0)
            {
                msgError = "Il file non risulta acquisito, quindi non è possibile applicare la firma HSM.";
                return msgError;
            }
            #endregion

            #region VERIFICA ESTENSIONE DEL FILE

            if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString())) || !Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString()).Equals("1"))
            {
                if (IsEnabledSupportedFileTypes)
                {
                    int count = this.FileTypes.Count(e => e.FileExtension.ToLowerInvariant() == estenxione && e.FileTypeUsed && e.FileTypeSignature);
                    bool retVal = (count > 0);
                    if (!retVal)
                    {
                        msgError = "Non è stato possibile firmare il documento. Effettuare la conversione in PDF del file.";
                        return msgError;
                    }
                }
            }

            #endregion

            #region VERIFICA DIMENSIONE FILE
            if (this.MaxDimFileSign > 0 && Convert.ToInt32(schedaDoc.documenti[0].fileSize) > this.MaxDimFileSign)
            {
                string maxSize = Convert.ToString(Math.Round((double)this.MaxDimFileSign / 1048576, 3));
                msgError = "La dimensione del file supera il limite massimo consentito per la firma. Il limite massimo consentito è: " + maxSize + " Mb.";
                return msgError;
            }
            #endregion

            #region DOCUMENTO ANNULLATO

            if (schedaDoc.protocollo != null && schedaDoc.protocollo.protocolloAnnullato != null && !string.IsNullOrEmpty(schedaDoc.protocollo.protocolloAnnullato.dataAnnullamento))
            {
                msgError = "Il file non è stato firmato in quanto il protocollo risulta annullato.";
                return msgError;
            }

            #endregion

            #region DOCUMENTO IN CESTINO

            if (schedaDoc.inCestino != null && schedaDoc.inCestino == "1")
            {
                msgError = "Il file non è stato firmato in quanto il documento è stato rimosso.";
                return msgError;
            }

            #endregion

            #region DOCUMENTO CONSOLIDATO

            if (schedaDoc.ConsolidationState != null && schedaDoc.ConsolidationState.State != DocsPaWR.DocumentConsolidationStateEnum.None)
            {
                msgError = "Il file non è stato firmato in quanto il documento è consolidato.";
                return msgError;
            }

            #endregion

            #region DOCUMENTO IN CHECKOUT

            if (schedaDoc.checkOutStatus != null && !string.IsNullOrEmpty(schedaDoc.checkOutStatus.ID))
            {
                msgError = "Il file non è stato firmato in quanto il documento è bloccato.";
                return msgError;
            }

            #endregion

            #region DOCUMENTO IN SOLA LETTURA

            if (Convert.ToInt32(schedaDoc.accessRights) == Convert.ToInt32(HMdiritti.HMdiritti_Read))
            {
                msgError = "Il file non è stato firmato in quanto il documento è in sola lettura.";
                return msgError;
            }

            #endregion

            #region DOCUMENTO IN ATTESA DI ACCETTAZIONE

            if (Convert.ToInt32(schedaDoc.accessRights) == Convert.ToInt32(HMdiritti.HDdiritti_Waiting))
            {
                msgError = "Il file non è stato firmato in quanto il documento è in attesa di accettazione.";
                return msgError;
            }

            #endregion

            #region FIRMA PADES

            //Controllo che il file che stò firmando è un PDF
            if (this.HsmRadioSign.Checked)
            {
                if (!isPdf && this.HsmLitPades.Checked)
                {
                    msgError = "Non è possibile firmare pades un documento non pdf. Effettuare la conversione in PDF del file";
                    return msgError;
                }
            }

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileReq.fileName);
            bool isCadesSign = (fileInfo != null && !String.IsNullOrEmpty(fileInfo.Extension) && fileInfo.Extension.ToLower().Equals(".p7m"));
            //bool isCadesSign = (!String.IsNullOrEmpty(fileReq.tipoFirma) && fileReq.tipoFirma.Equals(TipoFirma.CADES)) || (fileInfo != null && !String.IsNullOrEmpty(fileInfo.Extension) && fileInfo.Extension.ToLower().Equals(".p7m"));

            if (this.HsmLitPades.Checked && isCadesSign)
            {
                msgError = "Non è possibile applicare una firma PADES su di un file firmato CADES";
                return msgError;
            }

            #endregion

            #region COFIRMA

            //if (this.HsmRadioCoSign.Checked)
            //{
                ////Non posso cofirmare PADES
                //if (this.HsmLitPades.Checked)
                //{
                //    msgError = "Non è possibile applicare la cofirma PADES su file firmato";
                //    return msgError;
                //}
               // if (this.HsmLitP7M.Checked && (fileReq.tipoFirma == TipoFirma.PADES || fileReq.tipoFirma == TipoFirma.PADES_ELETTORNICA))
                //{
                    //Non posso firmare CADES un file firmato PADES :se l'estensione del file è PDF, il file è stato firmato PADES
                  //  msgError = "Non è possibile applicare la cofirma CADES su file firmato PADES";
                //    return msgError;
                //}
           // }

            #endregion

            #region VERFICA SE è OBBLIGATORIA LA CONVERSIONE IN PDF DEL FILE
            if (!isPdf)
            {
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString()).Equals("1"))
                {
                    if (IsEnabledSupportedFileTypes)
                    {
                        bool retVal = true;

                        int count = this.FileTypes.Count(f => f.FileExtension.ToLowerInvariant() == estenxione && f.FileTypeUsed && f.FileTypeSignature);
                        retVal = (count > 0);

                        if (!retVal)
                        {
                            msgError = "Non è stato possibile firmare il documento. Effettuare la conversione in PDF del file.";
                            return msgError;
                        }
                    }
                }

            }

            #endregion

            //MEV LIBRO FIRMA
            #region TIPO FIRMA DA APPORRE

            if (!IsLF && fileReq.inLibroFirma)
            {
                string typeSignature = LibroFirmaManager.GetTypeSignatureToBeEntered(fileReq);
                if (this.HsmLitP7M.Checked && !typeSignature.Equals(LibroFirmaManager.TypeEvent.SIGN_CADES))
                {
                    msgError = "Documento in Libro firma. La tipologia di firma selezionata non corrisponde a quella richiesta";
                    return msgError;
                }
                else if (this.HsmLitPades.Checked && !typeSignature.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES))
                {
                    msgError = "Documento in Libro firma. La tipologia di firma selezionata non corrisponde a quella richiesta";
                    return msgError;
                }
            }

            #endregion

            return msgError;
        }
    }
}