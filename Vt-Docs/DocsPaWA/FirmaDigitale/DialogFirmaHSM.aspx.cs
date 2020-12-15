using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DocsPAWA.utils;

namespace DocsPAWA.FirmaDigitale
{
    public partial class DialogFirmaHSM : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
            }
        }

        private void InitializePage()
        {
            // Popolamento dei campi Memento
            this.popolaCampiMemento();

            DocsPaWR.FileRequest fileReq = null;

            fileReq = FileManager.getSelectedFile();

            #region CONVERSIONE PDF LATO SERVER SINCRONA
            // Verfico la presenza della conversione lato server sincrona
            bool enabledConvertPdfServerSinc = false;
            try
            {
                if (!string.IsNullOrEmpty(InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "CONVERSIONE_PDF_SINCRONA_LC")))
                {
                    // verifico la presenza della chiave di DB 'CONVERSIONE_PDF_SINCRONA_LC'
                    enabledConvertPdfServerSinc = (InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "CONVERSIONE_PDF_SINCRONA_LC").Equals("1") ? true : false);
                }
                else
                {
                    // Verifico la presenza della chiave di web.config
                    enabledConvertPdfServerSinc = (!string.IsNullOrEmpty(Utils.IsEbabledConversionePdfLatoServerSincrona()) &&
                    Utils.IsEbabledConversionePdfLatoServerSincrona().Equals("true") ? true : false);
                }
            }
            catch (Exception exc) 
            {
                enabledConvertPdfServerSinc = false;
            }

            if (enabledConvertPdfServerSinc) 
            {
                // Gestione checkbox
                this.chkConverti.Visible = true;
                this.chkConverti.Enabled = true;
                this.upChConverti.Update();
            }
            #endregion


            if (enabledConvertPdfServerSinc)
            {
                // Gestione flag di conversione
                if (!string.IsNullOrEmpty(InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_REQ_CONV_PDF")) &&
                    InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_REQ_CONV_PDF").Equals("1"))
                {
                    //DocsPaWR.FileRequest fileReq = null;

                    //fileReq = FileManager.getSelectedFile();

                    bool isPdf = (FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToUpper() == "PDF");

                    if (!isPdf)
                    {
                        this.chkConverti.Checked = true;

                        if (Utils.IsEnabledSupportedFileTypes())
                        {
                            this.FileTypes = Utils.GetSupportedFileTypes(Int32.Parse(UserManager.getInfoUtente().idAmministrazione));

                            bool retVal = true;

                            int count = this.FileTypes.Count(e => e.FileExtension.ToLowerInvariant() == FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToLowerInvariant() &&
                                                                    e.FileTypeUsed && e.FileTypeSignature);
                            retVal = (count > 0);

                            this.chkConverti.Checked = !retVal;
                        }
                    }

                    this.chkConverti.Enabled = false;
                    this.upChConverti.Update();
                }
                else
                {
                    // Piccola correzione - da riportare su Sviluppo
                    //// Conversione obbligatoria non abilitata
                    if (!System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"))
                    {
                        this.chkConverti.Enabled = true;
                        this.upChConverti.Update();
                    }
                    else
                    {
                        this.chkConverti.Enabled = false;
                        this.upChConverti.Update();
                    }
                    //this.chkConverti.Enabled = false;
                    //this.upChConverti.Update();
                }
            }
            else 
            {
                // Conversione non abilitata
                this.chkConverti.Visible = false;
                this.chkConverti.Enabled = false;
                this.upChConverti.Update();
            }

            // Gestione Cofirma
            setCofirma(fileReq.firmato == "1" ? true : false, fileReq);

            // Gestione PADES
            // OLD CODE
            /*if (!System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"))
            {
                this.rbPades.Enabled = false;
                this.upTipoFirma.Update();
            }*/

            // NEW CODE
            // Solo se la conversione pdf non è possibile allora la firma Pades deve essere inibita
            if (!System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf") && !enabledConvertPdfServerSinc)
            {
                this.rbPades.Enabled = false;
                this.upTipoFirma.Update();
            }

            // 2. Gestione Flag Pades in caso di PDF (default)
            if (System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"))
            {
                this.rbPades.Checked = true;
                this.rbPades.Enabled = true;
                this.rbCades.Checked = false;
                this.upTipoFirma.Update();
            }

            // Intervento richiesta ENAC
            // 30-06-2014
            #region FE_DISABLE_CADES_HSM - Diabilitazione CADES
            // Gestione tramite chiave di DB disabilitazione CADES
            // In Presenza di questa chiave alcuni vecchi comportamenti sono inibiti
            if (!string.IsNullOrEmpty(InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_DISABLE_CADES_HSM")) &&
                    InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_DISABLE_CADES_HSM").Equals("1"))
            {
                this.lblUserMess.Visible = false;
                // 1. DEFAULT per chiave accesa
                this.rbCades.Checked = false;
                this.rbCades.Enabled = false;
                this.upTipoFirma.Update();

                // 2. Gestione Flag Pades in caso di PDF (default)
                if (System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"))
                {
                    this.rbPades.Checked = true;
                    this.rbPades.Enabled = true;
                    this.rbCades.Checked = false;
                    this.upTipoFirma.Update();
                }

                // 3. Gestione Conversione PDF - Forzo la conversione
                if (enabledConvertPdfServerSinc)
                {
                    #region Old Code
                    /*
                    // Conversione PDF Lato Server Sincrona Attiva
                    if (!string.IsNullOrEmpty(InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_REQ_CONV_PDF")) &&
                    InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_REQ_CONV_PDF").Equals("1"))
                    {
                        // Conversione PDF Obbligatoria
                        bool isPdf = (FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToUpper() == "PDF" || System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"));

                        if (!isPdf)
                        {
                            this.chkConverti.Checked = true;

                            if (Utils.IsEnabledSupportedFileTypes())
                            {
                                this.FileTypes = Utils.GetSupportedFileTypes(Int32.Parse(UserManager.getInfoUtente().idAmministrazione));

                                bool retVal = true;

                                int count = this.FileTypes.Count(e => e.FileExtension.ToLowerInvariant() == FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToLowerInvariant() &&
                                                                        e.FileTypeUsed && e.FileTypeSignature);
                                retVal = (count > 0);

                                this.chkConverti.Checked = !retVal;
                            }
                        }

                        this.chkConverti.Enabled = false;
                        this.upChConverti.Update();
                      
                        // Se flag conversione pdf acceso --> unica firma possibile è PADES
                        if (this.chkConverti.Checked)
                        {
                            this.rbPades.Checked = true;
                            this.upTipoFirma.Update();
                        }
                    }
                    else 
                    {
                        // Conversione PDF Non obbligatoria
                        // Cades Spento ---> obbligo l'utente a Convertire
                        bool isPdf = (FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToUpper() == "PDF" || System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"));

                        if (!isPdf)
                        {
                            this.chkConverti.Checked = true;

                            if (Utils.IsEnabledSupportedFileTypes())
                            {
                                this.FileTypes = Utils.GetSupportedFileTypes(Int32.Parse(UserManager.getInfoUtente().idAmministrazione));

                                bool retVal = true;

                                int count = this.FileTypes.Count(e => e.FileExtension.ToLowerInvariant() == FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToLowerInvariant() &&
                                                                        e.FileTypeUsed && e.FileTypeSignature);
                                retVal = (count > 0);

                                this.chkConverti.Checked = !retVal;
                                
                            }
                            this.chkConverti.Enabled = false;
                            this.upChConverti.Update();
                            
                            // Se flag conversione pdf acceso --> unica firma possibile è PADES
                            if (this.chkConverti.Checked)
                            {
                                this.rbPades.Checked = true;
                                this.upTipoFirma.Update();
                            }
                        }
                        
                    }
                    */
                    #endregion

                    // Conversione PDF Lato Server Sincrona Attiva
                    // Conversione PDF Diventa Obbligatoria
                    bool isPdf = (FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToUpper() == "PDF" || System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"));

                    if (!isPdf)
                    {
                        this.chkConverti.Checked = true;

                        if (Utils.IsEnabledSupportedFileTypes())
                        {
                            this.FileTypes = Utils.GetSupportedFileTypes(Int32.Parse(UserManager.getInfoUtente().idAmministrazione));

                            bool retVal = true;

                            int count = this.FileTypes.Count(e => e.FileExtension.ToLowerInvariant() == FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToLowerInvariant() &&
                                                                    e.FileTypeUsed && e.FileTypeSignature);
                            retVal = (count > 0);

                            this.chkConverti.Checked = !retVal;
                        }
                    }

                    this.chkConverti.Enabled = false;
                    this.upChConverti.Update();

                    // Se flag conversione pdf acceso --> unica firma possibile è PADES
                    if (this.chkConverti.Checked)
                    {
                        this.rbPades.Checked = true;
                        this.rbPades.Enabled = true;
                        this.upTipoFirma.Update();
                    }
                }
                else
                {
                    // Conversione PDF Lato Server Sincrona Non Attiva - Non è possibile convertire PDF
                    if (!System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf"))
                    {
                        // Cades disattivato, Conversione Inibita --> Impossibile Firmare
                        btnFirma.Enabled = false;

                        // Radio button Pades
                        this.rbPades.Checked = true;
                        this.rbPades.Enabled = false;
                        this.upTipoFirma.Update();

                        // Messaggio Utente:
                        // Formato non compatibile con la firma PDF(PADES)
                        this.lblUserMess.Text = "Formato file non compatibile con la firma PDF(PADES)";
                        this.lblUserMess.Visible = true;
                        this.upUserMess.Update();
                    }
                }

            }
            #endregion


        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        /// <summary>
        /// Metodo per imostare il RadioButton di Cofirma
        /// </summary>
        /// <param name="isSigned"></param>
        protected void setCofirma(bool isSigned, DocsPaWR.FileRequest fr)
        {
            this.rbCofirma.Enabled = true;

            // Cofirma: Deve essere Firmato
            if (!isSigned)
                this.rbCofirma.Enabled = false;

            // Cofirma: Non Abilitata se è un PADES
            if (isSigned && System.IO.Path.GetExtension(fr.fileName).ToLower().Equals(".pdf"))
                this.rbCofirma.Enabled = false;

            this.uprbFirmaCoF.Update();
        }

        //andrebbe chiamato all'open della maschera se presenti i dati alias e dominio essi verrano PRE popolati
        private void popolaCampiMemento()
        {
            RemoteDigitalSignManager dsm = new RemoteDigitalSignManager();
            try
            {
                RemoteDigitalSignManager.Memento m = dsm.HSM_GetMementoForUser();
                if (m != null)
                {
                    this.txtUseerName.Text = m.Alias;
                    this.txtDominio.Text = m.Dominio;
                }
            }
            catch (System.Exception ex)
            {
                return;
            }
        }

        protected string NomeServizioFirmaHSM
        {
            get
            {
                string nomeServizio = Request.QueryString["NomeServizioFirmaHSM"];

                if (nomeServizio == null || nomeServizio == string.Empty)
                    nomeServizio = "Remota";

                return nomeServizio;
            }
        }

        //protected string docNumber
        //{
        //    get
        //    {
        //        string docnumber = Request.QueryString["docnumber"];

        //        return docnumber;
        //    }
        //}

        //protected string systemId
        //{
        //    get
        //    {
        //        string systemId = Request.QueryString["systemId"];

        //        return systemId;
        //    }
        //}

        public string GetMaskTitle()
        {
            string retValue = string.Empty;

            retValue = "Firma " + NomeServizioFirmaHSM + " HSM";

            return retValue;
        }

        protected void btnFirma_OnClick(object sender, EventArgs e)
        {

            // Da valutare i campi Obbligatori
            if (!string.IsNullOrEmpty(this.txtUseerName.Text)
                //&& !string.IsNullOrEmpty(this.txtDominio.Text) 
                && !string.IsNullOrEmpty(this.txtPassword.Text)
                && !string.IsNullOrEmpty(this.txtOTP.Text)
                && (this.rbPades.Checked || this.rbCades.Checked))
            {
                string alias = this.txtUseerName.Text;
                string dominio = this.txtDominio.Text;
                string pin = this.txtPassword.Text;
                string otp = this.txtOTP.Text;

                DocsPaWR.FileRequest fileReq = null;

                fileReq = FileManager.getSelectedFile();

                bool isPdf = (FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToUpper() == "PDF");

                string msgError = CheckSign(fileReq, isPdf);
                if (!string.IsNullOrEmpty(msgError))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "alert('" + msgError + "');", true);
                }
                else
                {

                    RemoteDigitalSignManager dsm = new RemoteDigitalSignManager();

                    // attenzione mancano questi dati in maschera, vanno popolati come quelli sopra, prendendo i valori corretti.
                    bool cofirma = this.rbCofirma.Checked; //prendere dalla checkbox cofirma
                    //bool timestamp = this.HsmCheckMarkTemporal.Checked; //prendere dalla checkbox timestamp
                    bool timestamp = false;
                    RemoteDigitalSignManager.tipoFirma tipoFirma = new RemoteDigitalSignManager.tipoFirma();
                    if (this.rbPades.Checked)
                    {
                        tipoFirma = RemoteDigitalSignManager.tipoFirma.PADES;
                    }
                    else
                    {
                        tipoFirma = RemoteDigitalSignManager.tipoFirma.CADES;
                    }


                    try
                    {
                        RemoteDigitalSignManager.Memento m = new RemoteDigitalSignManager.Memento { Alias = alias, Dominio = dominio };
                        //Intanto salvo il memento
                        dsm.HSM_SetMementoForUser(m);
                    }
                    catch (System.Exception ex)
                    {
                        // Non è stato possibile salvare il mememnto
                        return;
                    }

                    bool retval = false;
                    bool convert = !isPdf && this.chkConverti.Checked;
                    try
                    {
                        //System.Threading.Thread.Sleep(3000);
                        //retval = true;
                        retval = dsm.HSM_Sign(fileReq, cofirma, timestamp, tipoFirma, alias, dominio, otp, pin, convert);

                    }
                    catch (System.Exception ex)
                    {
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "alertFirmaKO", "alert('Attenzione, errore durante l'operazione di firma HSM');", true);

                    }

                    if (retval)
                    {
                        // Ricarico il dettaglio della scheda documento
                        DocsPaWR.SchedaDocumento sd = DocumentManager.getDocumentoSelezionato(this);
                        DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDettaglioDocumento(this, sd.systemId, sd.docNumber);
                        DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                        
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "alertFirmaOK", "alert(Operazione di firma HSM avvenuta con successo');", true);
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "ajaxDialogModal", "alert('Operazione di firma HSM avvenuta con successo');", true);
                    }
                    else
                    {
                        // è accaduto un inconveniente.. la firma non è andata a buon fine...
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "alertFirmaKO", "alert('Attenzione, errore durante l'operazione di firma HSM');", true);
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "ajaxDialogModal", "alert('Attenzione, errore durante la firma HSM');", true);
                    }

                    if(retval)
                        // Chiude la finestra
                        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeWindows", "window.close();", true);
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeWindows", "CloseWindow();", true);
                }
            }
            else
            {
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "alert('Attenzione, inserire alias utente, password e otp');", true);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "ajaxDialogModal", "alert('Attenzione, inserire alias utente, password e otp');", true);
            }

            //// Chiude la finestra
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeWindows", "window.close();", true);


        }

        protected void HsmrbPades_Change(object sender, EventArgs e)
        {
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            DocsPaWR.FileRequest fileReq = null;

            fileReq = FileManager.getSelectedFile();

            bool isPdf = (FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToUpper() == "PDF");


            if (!this.chkConverti.Enabled && isPdf)
            {
                this.chkConverti.Checked = false;
            }
            else
            {

                if (Utils.IsEnabledSupportedFileTypes())
                {
                    this.FileTypes = Utils.GetSupportedFileTypes(Int32.Parse(UserManager.getInfoUtente().idAmministrazione));

                    bool retVal = true;

                    int count = this.FileTypes.Count(f => f.FileExtension.ToLowerInvariant() == FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToLowerInvariant() &&
                                                            f.FileTypeUsed && f.FileTypeSignature);
                    retVal = (count > 0);

                    this.chkConverti.Checked = !retVal;
                }

            }

            // Update Panel
            this.upTipoFirma.Update();
            this.upChConverti.Update();
            this.uprbFirmaCoF.Update();
        }

        protected void HsmrbP7M_Change(object sender, EventArgs e)
        {
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            DocsPaWR.FileRequest fileReq = null;

            fileReq = FileManager.getSelectedFile();

            bool isPdf = (FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToUpper() == "PDF");

            if (!this.chkConverti.Enabled && !isPdf)
            {
                if (Utils.IsEnabledSupportedFileTypes())
                {
                    this.FileTypes = Utils.GetSupportedFileTypes(Int32.Parse(UserManager.getInfoUtente().idAmministrazione));

                    bool retVal = true;

                    int count = this.FileTypes.Count(f => f.FileExtension.ToLowerInvariant() == FileManager.getEstensioneIntoSignedFile(this.Page, fileReq.fileName).ToLowerInvariant() &&
                                                            f.FileTypeUsed && f.FileTypeSignature);
                    retVal = (count > 0);

                    this.chkConverti.Checked = !retVal;
                }
            }

            // Update Panel
            this.upTipoFirma.Update();
            this.upChConverti.Update();
            this.uprbFirmaCoF.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsEnabledConvPDFSincrona
        {
            get
            {
                return new DocsPAWA.DocsPaWR.DocsPaWebService().IsEnabledConversionePdfLatoServerSincrona();
            }
        }



        /// <summary>
        /// Tipi di file
        /// </summary>
        private DocsPAWA.DocsPaWR.SupportedFileType[] FileTypes
        {
            get
            {
                if (this.ViewState["SupportedFileType"] == null)
                    return null;
                else
                    return this.ViewState["SupportedFileType"] as DocsPAWA.DocsPaWR.SupportedFileType[];
            }
            set
            {
                if (this.ViewState["SupportedFileType"] == null)
                    this.ViewState.Add("SupportedFileType", value);
                else
                    this.ViewState["SupportedFileType"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileReq"></param>
        /// <param name="isPdf"></param>
        /// <returns></returns>
        private string CheckSign(DocsPAWA.DocsPaWR.FileRequest fileReq, bool isPdf)
        {
            string msgError = string.Empty;

            #region FIRMA PADES

            //Controllo che il file che stò firmando è un PDF
            if (this.rbFirma.Checked)
            {
                if (!isPdf && this.rbPades.Checked && !this.chkConverti.Checked)
                {
                    // Vedere messaggio di errore
                    msgError = "Attenzione, non è possibile firmare pades un documento non pdf";
                    return msgError;
                }
            }
            #endregion

            #region COFIRMA

            //La cofirma può essere solo di tipo CADES e applicabile so su file firmato CADES

            if (this.rbCofirma.Checked)
            {
                //Non posso cofirmare PADES
                if (this.rbPades.Checked)
                {
                    // // Vedere messaggio di errore
                    msgError = "Non è possibile applicare la firma PADES su file firmato";
                    return msgError;
                }
                else if (this.rbCades.Checked && (System.IO.Path.GetExtension(fileReq.fileName).ToLower().Equals(".pdf")))
                {
                    //Non posso firmare CADES un file firmato PADES :se l'estensione del file è PDF, il file è stato firmato PADES
                    // Vedere messaggio di errore
                    msgError = "Non è possibile applicare la firma CADES su file firmato PADES";
                    return msgError;
                }
            }

            #endregion

            return msgError;
        }
    }
}