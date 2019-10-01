using System;
using System.Web.UI;
using ConservazioneWA.DocsPaWR;
using ConservazioneWA.Utils;

namespace ConservazioneWA
{
    public partial class HSM_Signature : System.Web.UI.Page
    {
        protected WSConservazioneLocale.DocsPaConservazioneWS wss;
        protected WSConservazioneLocale.InfoUtente infoUtente;
        protected DocsPaWR.InfoUtente infoUtenteWR;
        protected string idIstanza;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                this.infoUtenteWR = GetInfoUtenteWR(infoUtente);// ((DocsPaWR.InfoUtente)Session["infoutCons"]);

                this.idIstanza = Request.QueryString["idIstanza"];
                this.wss = new ProxyManager().getProxy();

                this.InitializeLanguage();
                this.InitializePage();

            }
        }

        private DocsPaWR.InfoUtente GetInfoUtenteWR(WSConservazioneLocale.InfoUtente infoUt)
        {
            DocsPaWR.InfoUtente newInfoUt = new InfoUtente();

            newInfoUt.codWorkingApplication = infoUt.codWorkingApplication;
            //newInfoUt.delegato = infoUt.delegato;
            newInfoUt.diSistema = infoUt.diSistema;
            newInfoUt.dst = infoUt.dst;
            newInfoUt.extApplications = infoUt.extApplications;
            newInfoUt.idAmministrazione = infoUt.idAmministrazione;
            newInfoUt.idCorrGlobali = infoUt.idCorrGlobali;
            newInfoUt.idGruppo = infoUt.idGruppo;
            newInfoUt.idPeople = infoUt.idPeople;
            newInfoUt.matricola = infoUt.matricola;
            newInfoUt.sede = infoUt.sede;
            newInfoUt.urlWA = infoUt.urlWA;
            newInfoUt.userId = infoUt.userId;
            
            return newInfoUt;
        }

        private void InitializePage()
        {
            this.popolaCampiMemento();
            if (Utils.ConservazioneManager.isRoleAuthorized(this.infoUtente.idCorrGlobali, "TO_GET_OTP"))
            {
                this.BtnRequestOTP.Visible = true;
            }
            
            this.TxtHsmPin.Focus();

            this.gestisciClassiPulsanti();
        }


        private void gestisciClassiPulsanti()
        {

            if (BtnSign.Enabled)
            {
                BtnSign.Attributes.Remove("class");
                BtnSign.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                BtnSign.Attributes.Add("onmouseout", "this.className='cbtn';");
                BtnSign.Attributes.Add("class", "cbtn");
            }
            else
            {
                BtnSign.Attributes.Remove("onmouseover");
                BtnSign.Attributes.Remove("onmouseout");
                BtnSign.Attributes.Remove("class");
                BtnSign.Attributes.Add("class", "cbtnDisabled");
            }
            if (BtnClose.Enabled)
            {
                BtnClose.Attributes.Remove("class");
                BtnClose.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                BtnClose.Attributes.Add("onmouseout", "this.className='cbtn';");
                BtnClose.Attributes.Add("class", "cbtn");
            }
            else
            {
                BtnClose.Attributes.Remove("onmouseover");
                BtnClose.Attributes.Remove("onmouseout");
                BtnClose.Attributes.Remove("class");
                BtnClose.Attributes.Add("class", "cbtnDisabled");
            }
            if (BtnRequestOTP.Enabled)
            {
                BtnRequestOTP.Attributes.Remove("class");
                BtnRequestOTP.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                BtnRequestOTP.Attributes.Add("onmouseout", "this.className='cbtn';");
                BtnRequestOTP.Attributes.Add("class", "cbtn");
            }
            else
            {
                BtnRequestOTP.Attributes.Remove("onmouseover");
                BtnRequestOTP.Attributes.Remove("onmouseout");
                BtnRequestOTP.Attributes.Remove("class");
                BtnRequestOTP.Attributes.Add("class", "cbtnDisabled");
            }
        }


        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('HSMSignature', '');", true);
        }


        private void InitializeLanguage()
        {
            //string language = Utils.UserManager.GetUserLanguage();
            this.BtnRequestOTP.Text = "Richiedi OTP";// Utils.Languages.GetLabelFromCode("BtnRequestOTP", language);
            this.BtnClose.Text = "Chiudi"; // Utils.Languages.GetLabelFromCode("HSM_SignatureBtnClose", language);
            this.BtnSign.Text = "Firma"; // Utils.Languages.GetLabelFromCode("HSM_SignatureBtnSign", language);
            this.HsmLitAlias.Text = "Alias Utente*"; //Utils.Languages.GetLabelFromCode("HsmLitAlias", language);
            this.HsmLitDomain.Text = "Dominio Certificato*"; // Utils.Languages.GetLabelFromCode("HsmLitDomain", language);
            this.HsmLitPin.Text = "PIN*"; //Utils.Languages.GetLabelFromCode("HsmLitPin", language);
            this.HsmLitOtp.Text = "OTP*"; // Utils.Languages.GetLabelFromCode("HsmLitOtp", language);
        }


        //andrebbe chiamato all'open della maschera se presenti i dati alias e dominio essi verrano PRE popolati
        private void popolaCampiMemento()
        {
            DigitalSignature.RemoteDigitalSignManager dsm = new DigitalSignature.RemoteDigitalSignManager();
            try
            {
                DigitalSignature.RemoteDigitalSignManager.Memento m = dsm.HSM_GetMementoForUser(this.infoUtenteWR);
                if (m != null)
                {
                    this.TxtHsmAlias.Text = m.Alias;
                    this.TxtHsmDomain.Text = m.Dominio;
                }
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
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
                    dsm.HSM_SetMementoForUser(m, this.infoUtenteWR);
                }
                catch (System.Exception ex)
                {
                    //UIManager.AdministrationManager.DiagnosticError(ex);
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
            bool cofirma = false;
            bool timestamp = false; 
            bool convert = false;

            DigitalSignature.RemoteDigitalSignManager dsm = null;
            DigitalSignature.RemoteDigitalSignManager.tipoFirma tipoFirma = DigitalSignature.RemoteDigitalSignManager.tipoFirma.CADES;
            DigitalSignature.RemoteDigitalSignManager.Memento mem = null;

            String alias, dominio, pin, otp;

            byte[] fileToSign = null;

            if (!string.IsNullOrEmpty(this.TxtHsmAlias.Text) && !string.IsNullOrEmpty(this.TxtHsmDomain.Text) && !string.IsNullOrEmpty(this.TxtHsmPin.Text) && !string.IsNullOrEmpty(this.TxtHsmLitOtp.Text))
            {
                alias = this.TxtHsmAlias.Text;
                dominio = this.TxtHsmDomain.Text;
                pin = this.TxtHsmPin.Text;
                otp = this.TxtHsmLitOtp.Text;

                dsm = new DigitalSignature.RemoteDigitalSignManager();
                tipoFirma = new DigitalSignature.RemoteDigitalSignManager.tipoFirma();
                mem = new DigitalSignature.RemoteDigitalSignManager.Memento {Alias = alias, Dominio = dominio};

                try
                {
                    // D.O. 16/11/2016 le seguenti proprità non sono inizializzate in Page_Load (IsPostBack = true)
                    //
                    this.wss = new ProxyManager().getProxy(); 
                    this.idIstanza = Request.QueryString["idIstanza"]; 
                    this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                    this.infoUtenteWR = GetInfoUtenteWR(infoUtente);

                    dsm.HSM_SetMementoForUser(mem, this.infoUtenteWR);
                    //

                    fileToSign = wss.downloadSignedXml(idIstanza, this.infoUtente);

                    dsm.HSM_SignContent(idIstanza, fileToSign, cofirma, timestamp, tipoFirma, alias, dominio, otp, pin, convert, this.infoUtenteWR);
                }
                catch (System.Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + ex.Message + "', 'warning');} else {parent.ajaxDialogModal('" + ex.Message + "', 'warning');}", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "alert('Inserire tutti i campi obbligatori.';", true);
            }
        }


        private void SetNewFile(FileRequest fileReq)
        {
            byte[] bt = new byte[1];

            string result = wss.uploadSignedXml(idIstanza, bt, (WSConservazioneLocale.InfoUtente)Session["infoutCons"], true);
            Session["resultFirma"] = result;
        }


        /// <summary>
        /// 
        /// </summary>
        protected bool IsEnabledConvPDFSincrona
        {
            get
            {
                return new ConservazioneWA.DocsPaWR.DocsPaWebService().IsEnabledConversionePdfLatoServerSincrona();
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

    }
}