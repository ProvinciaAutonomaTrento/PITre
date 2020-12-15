using System; 
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using log4net;

namespace DocsPAWA.documento
{

	/// <summary>
	/// Summary description for p_File.
	/// </summary>
    public class tabDoc : System.Web.UI.Page, System.Web.UI.ICallbackEventHandler
	{
        private ILog logger = LogManager.GetLogger(typeof(tabDoc));
		protected DocsPaWebCtrlLibrary.ImageButton btn_visualizza;
		protected DocsPaWebCtrlLibrary.ImageButton btn_acquisisci;
		protected DocsPAWA.DocsPaWR.SchedaDocumento schedaCorrente;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hdn_doc;
		protected DocsPaWebCtrlLibrary.ImageButton btn_firma;
		protected DocsPaWebCtrlLibrary.ImageButton btn_Zoom;
        private string returnValue = string.Empty;
		protected DocsPaWebCtrlLibrary.ImageButton btn_cofirma;
		protected System.Web.UI.WebControls.Label lbl_size;
        protected System.Web.UI.WebControls.Image img_cartaceo;
        protected System.Web.UI.WebControls.ImageButton img_firmato;
		protected System.Web.UI.WebControls.Image img_tipo;
		protected System.Web.UI.HtmlControls.HtmlTableCell tipo;
		protected System.Web.UI.HtmlControls.HtmlImage btn_DettagliFirma;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hdn_size_max;
		protected DocsPaWebCtrlLibrary.ImageButton btn_visualizza_con_segn;
        
		private string fileDaVisualizzare;
		protected DocsPaWebCtrlLibrary.ImageButton btn_posiziona_segn;
		protected System.Web.UI.WebControls.Panel pnl_visual_segnatura;
		protected System.Web.UI.HtmlControls.HtmlTable tb_visual_segnatura;
		protected System.Web.UI.HtmlControls.HtmlTableCell prova;
		protected System.Web.UI.HtmlControls.HtmlTableCell prova2;
		protected System.Web.UI.HtmlControls.HtmlTableCell td_contiene_visual;
		protected System.Web.UI.HtmlControls.HtmlTableCell td_visualizza;
		protected System.Web.UI.HtmlControls.HtmlTableCell td_segnatura;
		//protected System.Web.UI.HtmlControls.HtmlInputHidden hd_conferma;

        // Firma Automatica e segnatura elettronica
        protected DocsPaWebCtrlLibrary.ImageButton btn_segn_firma_automatica;
        private bool docSizeIsZero = false;
        private bool docFirmato = false;
        private bool docIsPdf = false;
        private bool docIsProtocollo = false;
        //protected System.Web.UI.WebControls.HiddenField hf_segn_firma_automatica;
        // END Firma Automatica e segnatura elettronica

        //
        // Firma HSM
        protected DocsPaWebCtrlLibrary.ImageButton imgbtn_firmaHSM;
        // END Firma HSM
        //
		
		private bool _fileFirmato=false;
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrameDoc;
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrameSignedDoc;
		protected System.Web.UI.HtmlControls.HtmlTableCell dimensione;
        protected System.Web.UI.HtmlControls.HtmlTableCell cartaceo;
		protected System.Web.UI.HtmlControls.HtmlTableCell firmatoLabel;
		protected DocsPaWebCtrlLibrary.ImageButton btn_modello;
		protected System.Web.UI.HtmlControls.HtmlTableCell td_label_modello;
		protected System.Web.UI.HtmlControls.HtmlTableCell Td1;
		protected System.Web.UI.WebControls.ImageButton imgCheckOutDocument;
		protected System.Web.UI.HtmlControls.HtmlTableCell checkOutDocument;
		private string valore = String.Empty;
        //il tipo lo metto a false di default quindi significa che prende la segnatura invece che il timbro
        private string valTipo = "false";
        private string valRotaz = String.Empty;
        private string valCarat = String.Empty;
        private string valColor = String.Empty;
        private string valOrienta = String.Empty;
        private string estensione = String.Empty;
        protected System.Web.UI.WebControls.ImageButton img_converti;
        protected System.Web.UI.WebControls.Label lbl_converti;
        protected System.Web.UI.WebControls.Panel pnl_converti;

        protected DocsPAWA.FormatiDocumento.SupportedFilesController supportedFileTypeController;

        protected DocsPAWA.ActivexWrappers.ClientModelProcessor clientModelProcessor;

        protected System.Web.UI.HtmlControls.HtmlLink linkCss;

        protected DocsPaWebCtrlLibrary.ImageButton btn_timestamp;

        private bool _SignedDocumentOnSession = false;
		
		private void ShowLabels(bool visible)
		{
			this.dimensione.Visible = visible;
            this.cartaceo.Visible = visible;
			this.firmatoLabel.Visible = visible;
			this.tipo.Visible = visible;
		}
	
		private void Page_Load(object sender, System.EventArgs e) 
		{
            logger.Info("BEGIN");
            try
            {
                //callback per pulsante di zoom
                String cbReference = Page.ClientScript.GetCallbackEventReference(this, "arg", "ReceiveServerUrlData", "context");
                String callbackScript;
                callbackScript = "function CallServerZoom(arg, context)" +
                    "{ " + cbReference + ";}";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(),
                    "CallServerZoom", callbackScript, true);

                //MODIFICA
                DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                string idAmm = UserManager.getInfoUtente().idAmministrazione;
                //modifica

                // GESTIONE PULSANTI DELLA SEGNATURA
                if (System.Configuration.ConfigurationManager.AppSettings["ENABLE_LABEL_PDF"] != null
                    && System.Configuration.ConfigurationManager.AppSettings["ENABLE_LABEL_PDF"].ToLower().Equals("true"))
                {
                    // contenitore
                    //this.td_contiene_visual.Width="135";
                    //this.td_contiene_visual.ColSpan=2;							
                    // TD sotto al contenitore con il pulsante visualizza
                    //this.td_visualizza.Width="45";
                    // pulsante visualizza
                    this.btn_visualizza.ImageUrl = "../images/tabDoc/visualizza_attivo_p.gif";
                    this.btn_visualizza.DisabledUrl = "../images/tabDoc/visualizza_noattivo_p.gif";
                    //this.btn_visualizza.Width=29;
                    // TD contenente la tabella con i pulsanti di segnatura
                    this.td_segnatura.Visible = true;
                    btn_posiziona_segn.Attributes.Add("onClick", "ApriFinestra()");
                    btn_posiziona_segn.PostBackUrl = "javascript:void(0);";
                }
                else
                {
                    // contenitore
                    //this.td_contiene_visual.Width="75";
                    //this.td_contiene_visual.ColSpan=0;							
                    // TD sotto al contenitore con il pulsante visualizza
                    //this.td_visualizza.Width="75";
                    // pulsante visualizza
                    this.btn_visualizza.ImageUrl = "../images/tabDocImages/visualizza_attivo.gif";
                    this.btn_visualizza.DisabledUrl = "../images/tabDocImages/visualizza_nonattivo.jpg";
                    //this.btn_visualizza.Width=59;
                    // TD contenente la tabella con i pulsanti di segnatura
                    this.td_segnatura.Visible = false;
                }


                ShowLabels(false);
                Utils.startUp(this);
                this.Response.Expires = -1;
                this.btn_firma.Enabled = false;
                this.btn_cofirma.Enabled = false;
                this.btn_visualizza.Enabled = false;
                this.btn_visualizza_con_segn.Enabled = false;
                this.btn_posiziona_segn.Enabled = false;
                this.btn_acquisisci.Enabled = false;
                System.Diagnostics.Trace.WriteLine("false");
                this.btn_Zoom.Enabled = false;

                //
                // Firma HSM
                this.imgbtn_firmaHSM.Visible = false;
                this.imgbtn_firmaHSM.Enabled = false;
                // End Firma HSM
                //

                //if (!this.IsPostBack)
                //{
                    // Registrazione script client
                    this.RegisterAcquireDocumentScript();
                    this.RegisterScriptFirmaDigitale();

                    //
                    // Firma HSM
                    this.RegisterScriptFirmaHSM();
                    // End Firma HSM
                    //
                //}

                schedaCorrente = DocumentManager.getDocumentoSelezionato(this);

                DocsPaWR.FileRequest documento = FileManager.getSelectedFile(this);

                if (ConfigSettings.getKey(ConfigSettings.KeysENUM.FILE_ACQ_SIZE_MAX) != null)
                {
                    this.hdn_size_max.Value = ConfigSettings.getKey(ConfigSettings.KeysENUM.FILE_ACQ_SIZE_MAX).ToLower();
                }

                //				if (SmartClient.Configurations.IsActive())
                //					this.btn_acquisisci.Attributes.Add("onclick","ShowAcquireSmartClient();");
                //				else
                //					this.btn_acquisisci.Attributes.Add("onclick","ApriAcquisisciDocumento();");

                System.Diagnostics.Trace.WriteLine("schedaCorrente is null: -" + ((bool)(schedaCorrente == null)).ToString());

                if (schedaCorrente != null)
                {
                    this.InitializeCheckInOutPanel();

                    //controllo per l'abilitazione tasto firma
                    if (CtrlIfProt() == true)
                    {
                        this.btn_firma.Enabled = false;
                        //
                        // Firma HSM
                        this.imgbtn_firmaHSM.Enabled = false;
                        this.imgbtn_firmaHSM.Visible = false;
                        // End Firma HSM
                        //
                    }
                    else
                    {
                        // Pulsante firma abilitato solo se il documento non è in stato checkout
                        this.btn_firma.Enabled = (this.schedaCorrente.checkOutStatus == null);

                        //
                        // Firma HSM
                        this.imgbtn_firmaHSM.Visible = (this.schedaCorrente.checkOutStatus == null) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                        this.imgbtn_firmaHSM.Enabled = (this.schedaCorrente.checkOutStatus == null) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                        // End Firma HSM
                        //

                        if (DocsPAWA.UserManager.isFiltroAooEnabled(this.Page))
                        {
                            if (btn_firma.Enabled)
                            {
                                DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistri(this);
                                btn_firma.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri);
                            }

                            //
                            // Firma HSM
                            if (imgbtn_firmaHSM.Enabled && imgbtn_firmaHSM.Visible)
                            {
                                DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistri(this);
                                imgbtn_firmaHSM.Visible = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                                imgbtn_firmaHSM.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                            }
                            // End Firma HSM
                            //
                        }
                    }

                    //size
                    string size = "0";

                    //string estensione = "";

                    if (documento != null)
                    {
                        try
                        {
                           //modifica
                                    DocsPAWA.DocsPaWR.InfoFileCaching infoFile = ws.getFileInCache(documento.docNumber, documento.versionId, idAmm);
                                    if (infoFile != null)
                                    {
                                        documento.fileSize = infoFile.file_size.ToString();
                                        documento.fileName = infoFile.CacheFilePath;
                                    }
                           //fine modifica
                            size = Convert.ToString((double)Int32.Parse(documento.fileSize) / 1024);

                            //Icona  Tipo
                            if (size.Equals("0"))
                            {
                                this.img_tipo.Visible = false;

                                #region Stampa Segnatura Firma Automatica External Service
                                // Stampa Segnatura Firma Automatica External Service
                                docSizeIsZero = true;
                                // End Stampa Segnatura Firma Automatica External Service
                                #endregion
                            }
                            else
                            {
                                this.lbl_size.Text = Convert.ToString(Math.Round((double)Int32.Parse(documento.fileSize) / 1024, 3)) + " Kb";
                                ShowLabels(true);

                                FileManager fileMan = new FileManager();

                                DocsPaWR.FileDocumento fileDocum = fileMan.getInfoFile(Page);

                                string fullName = fileDocum.fullName;
                                if (fullName != null && fullName.Length > 0)

                                    estensione = fullName.Substring(fullName.LastIndexOf(".") + 1);


                                //}	
							    //NUOVA GESTIONE DOCUMENTI FIRMATI
                                //this._fileFirmato = fileDocum.name.ToUpper().EndsWith("P7M");
                                if (string.IsNullOrEmpty(documento.firmato))
                                    this._fileFirmato = fileDocum.name.ToUpper().EndsWith("P7M");
                                else
                                    if (documento.firmato == "1")
                                        this._fileFirmato = true; ;
                                
                                estensione = FileManager.getEstensioneIntoSignedFile(this, fullName);
                                


                                String nomeOriginale = "\r\n" +fileDocum.nomeOriginale;
                                if (nomeOriginale == null)
                                    nomeOriginale = "";

                                this.img_tipo.ImageUrl = FileManager.getFileIcon(this, estensione);
                                this.img_tipo.AlternateText = "Tipo file: " + estensione + nomeOriginale;
                                this.img_tipo.Visible = true;

                                this.img_cartaceo.Visible = documento.cartaceo;


                                //if (this.btn_firma.Enabled)
                                //{
                                //    this.btn_firma.Enabled = this.supportedFileTypeController.IsSignEnabledPerExtension(estensione);
                                //}

                                //this._fileFirmato=(fileDocum.signatureResult!=null &&
                                //					fileDocum.signatureResult.PKCS7Documents.Length>0);

                                #region Stampa Segnatura Firma Automatica External Service
                                // Stampa Segnatura Firma Automatica External Service
                                if (this._fileFirmato)
                                    docFirmato = true;

                                //if ((!string.IsNullOrEmpty(estensione) && estensione.ToLower().Contains("pdf"))
                                //    || (!string.IsNullOrEmpty(nomeOriginale) && nomeOriginale.ToLower().Contains("pdf"))
                                //    )
                                if ((
                                    (!string.IsNullOrEmpty(estensione) && estensione.ToLower().Equals("pdf"))
                                    || (!string.IsNullOrEmpty(estensione) && estensione.ToLower().EndsWith("pdf"))
                                    )
                                    && System.IO.Path.GetExtension(fullName).ToLower().Equals(".pdf")
                                    )
                                    docIsPdf = true;
                                // End Stampa Segnatura Firma Automatica External Service
                                #endregion
                            }
                        }
                        catch
                        {
                        }

                    }

                    if (size.Equals("0"))
                    {
                        fileDaVisualizzare = "statoDocDaAcquisire.aspx";
                        //if (schedaCorrente.systemId != null)
                        //    if (schedaCorrente.systemId != null && !schedaCorrente.systemId.Equals(""))
                        //    {
                        //        this.btn_acquisisci.Enabled = true;
                        //    }

                        if (string.IsNullOrEmpty(this.schedaCorrente.systemId) && this.schedaCorrente.repositoryContext == null)
                            this.btn_acquisisci.Enabled = false;
                        else
                            this.btn_acquisisci.Enabled = true;

                        this.btn_visualizza.Enabled = false;
                        this.btn_visualizza_con_segn.Enabled = false;
                        this.btn_posiziona_segn.Enabled = false;
                        this.btn_firma.Enabled = false;
                        this.btn_Zoom.Enabled = false;

                        //
                        // Firma HSM
                        this.imgbtn_firmaHSM.Visible = false;
                        this.imgbtn_firmaHSM.Enabled = false;
                        // End Firma HSM
                        //

                        #region Stampa segnatura Firma Automatica External Service
                        // Stampa Segnatura Firma Automatica External Service
                        docSizeIsZero = true;
                        // End Stampa Segnatura Firma Automatica External Service
                        #endregion
                    }
                    else
                    {
                        string autoPreview = "false";
                        try
                        {
                            autoPreview = ConfigSettings.getKey(ConfigSettings.KeysENUM.DOCUMENT_AUTOPREVIEW).ToLower();
                        }
                        catch (Exception) { }
                        if (autoPreview.Equals("false"))
                            fileDaVisualizzare = "statoDocAcquisito.aspx";
                        else
                        {
                            fileDaVisualizzare = "docVisualizzaFrame.aspx?id=" + Session.SessionID;
                        }

                        if (Session["refreshDxPageVisualizzatore"] != null && Convert.ToBoolean(Session["refreshDxPageVisualizzatore"]) == true)
                        {
                            fileDaVisualizzare = "docVisualizzaFrame.aspx?id=" + Session.SessionID;
                        }
                        
                        this.btn_acquisisci.Enabled = false;

                        this.btn_visualizza.Enabled = true;


                        //abilitazione del bottone "visualizza con Segnatura solo nel caso di protocolli e pdf
                        string extension = this.GetCurrentFileExtension();

                        bool supportedType = (extension.Equals("PDF") || PdfConverterInfo.CanConvertFileToPdf(new FileManager().getInfoFile(this).fullName));

                        #region Stampa Segnatura Firma Automatica External Service
                        // Stampa Segnatura Firma Automatica External Service
                        if (!string.IsNullOrEmpty(extension) && extension.Equals("PDF") && System.IO.Path.GetExtension(new FileManager().getInfoFile(this).fullName).ToLower().Equals(".pdf"))
                            docIsPdf = true;
                        // End Stampa Segnatura Firma Automatica External Service
                        #endregion

                        if (schedaCorrente != null &&
                            
                            (
                           //protocolli
                            (schedaCorrente.protocollo != null
                            && schedaCorrente.protocollo.segnatura != null
                            && schedaCorrente.protocollo.segnatura != "")

                            || 
                            
                            //predisposti
                            (schedaCorrente.protocollo != null 
                            && string.IsNullOrEmpty(schedaCorrente.protocollo.segnatura) 
                            && !string.IsNullOrEmpty(schedaCorrente.systemId))

                            ||
                            
                            //grigi
                            (schedaCorrente.protocollo==null 
                            && !string.IsNullOrEmpty(schedaCorrente.systemId))
                            

                            )


                            && supportedType
                            && Session["allegato"] == null)
                        {
                            this.btn_visualizza_con_segn.Enabled = true;
                            if (Session["VisSegn"] != null)
                            {
                                this.btn_posiziona_segn.Enabled = true;
                            }
                        }

                        #region Stampa Segnatura Firma Automatica External Service
                        // Stampa Segnatura Firma Automatica External Service
                        if (schedaCorrente != null &&
                            //protocolli
                            (schedaCorrente.protocollo != null
                            && !string.IsNullOrEmpty(schedaCorrente.protocollo.segnatura))
                            )
                            docIsProtocollo = true;
                        #endregion

                        //Mev 109 INPS
                        if (utils.InitConfigurationKeys.GetValue("0", "FE_PERMANENT_DISPLAYS_SEGNATURE").Equals("1"))
                        {
                            //I pulsanti 'Visualizza con segnatura' e 'Posiziona segnatura' vengono abilitati solo per la versione corrente e solo nel caso in cui per tale versione non  
                            //sia già stato acquisito un documento con segnatura impressa
                            if (documento != null)
                            {
                                if ((documento.versionId.Equals(schedaCorrente.documenti[0].versionId) && DocumentManager.IsVersionWithSegnature(documento.versionId)) ||
                                    (!documento.versionId.Equals(schedaCorrente.documenti[0].versionId)))
                                {
                                    btn_visualizza_con_segn.Enabled = false;
                                    btn_posiziona_segn.Enabled = false;
                                }
                                //se la versione corrente non ha impressa segnatura, è di tipo pdf ed il protocollo è stato creato allora abilita il pulsante 'Posizionamento della Segnatura'
                                else if (GetCurrentFileExtension().ToUpper().Equals("PDF") && schedaCorrente.protocollo != null && !string.IsNullOrEmpty(schedaCorrente.protocollo.segnatura))
                                {
                                    btn_posiziona_segn.Enabled = true;
                                }
                            }
                        }

                        if (autoPreview == "false")
                        {
                            this.btn_firma.Enabled = false;
                            //
                            // Firma HSM
                            this.imgbtn_firmaHSM.Visible = false;
                            this.imgbtn_firmaHSM.Enabled = false;
                            // End Firma HSM
                            //
                        }
                        else
                            this.DoShowDocument();

                        this.btn_Zoom.Enabled = true;
                    }

                    if (this._fileFirmato)
                    {
                        this.img_firmato.Visible = true;
                        this.img_firmato.Style.Add("display", "static");

                        ShowLabels(true);
                        if (size.Equals("0"))
                        {
                            this.img_firmato.Visible = false;
                            ShowLabels(false);
                        }
                    }
                    else
                    {
                        this.img_firmato.Style.Add("display", "none");
                        this.img_firmato.Visible = true;
                    }

                    if ((schedaCorrente.inCestino != null && schedaCorrente.inCestino == "1") || (schedaCorrente.inArchivio != null && schedaCorrente.inArchivio == "1"))
                        DisabilitaTutto();
                }

                this.EnableButtonDettagliFirma();

                if (documento == null)
                {
                    this.btn_acquisisci.Enabled = false;
                    System.Diagnostics.Trace.WriteLine("false");
                }

                // Inizializzazione controllo verifica acl
                //if ((schedaCorrente != null) && (schedaCorrente.systemId != null))
                //    this.InitializeControlAclDocumento();

                //Controllo se la pagina deve visualizzare i campi profilati
                if (Request.QueryString["profilazione"] != null && Request.QueryString["profilazione"] == "1")
                {
                    Session.Add("focus", false);
                    if (Request.QueryString["pulsanti"] != null && Request.QueryString["pulsanti"] != "")
                    {
                        fileDaVisualizzare = "AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamica.aspx?pulsanti=" + Request.QueryString["pulsanti"];
                    }
                    else
                    {
                        fileDaVisualizzare = "AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinamica.aspx";
                    }
                }

                // Se nella request ho fra i parametri opt 
                // con valore visualizzaInterno...
                if (!String.IsNullOrEmpty(Request["opt"]) &&
                    Request["opt"].Equals("visualizzaInterno"))
                    // ...lancio l'evento click del bottone btn_visualizza
                    this.btn_visualizza_Click(sender, null);

                //Contrllo se il documento arriva per trasmissione extra aoo e non si ha visibilità sul registro
                if (UserManager.isFiltroAooEnabled(this) && btn_acquisisci.Enabled)
                {
                    DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);
                    this.btn_acquisisci.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri);
                }

                #region Stampa Segnatura Firma Automatica External Service
                // Abilitazione Pulsante Firma Automatica e Segnatura Elettronica, External Service
                bool stampa_segn_auto_enabled = DocsPAWA.UserManager.ruoloIsAutorized(this.Page, this.btn_segn_firma_automatica.Tipologia);

                // Controllo: Micro Abilitata; 
                // dimensione del documento; 
                // documento è PDF PADES (PDF + Firmato)
                // documento protocollo;
                // Documento non in checkout
                // documento non in cestino
                //if (stampa_segn_auto_enabled && !docSizeIsZero && docIsPdf && docIsProtocollo && !this.IsDocumentCheckedOut() && !this.IsDocumentoInCestino)
                if (stampa_segn_auto_enabled && !docSizeIsZero && docIsPdf && docIsProtocollo && !this.IsDocumentCheckedOut() && !this.IsDocumentoInCestino && docFirmato && schedaCorrente != null && !string.IsNullOrEmpty(schedaCorrente.systemId))
                    this.btn_segn_firma_automatica.Visible = true;
                else
                    this.btn_segn_firma_automatica.Visible = false;

                // End Abilitazione Pulsante Firma Automatica e Segnatura Elettronica, External System

                // Controllo HiddenField segnatura Automatica
                if (Session["ResultRemotePdfStamp"] != null)
                {
                    string resultRemotePdfStamp = string.Empty;
                    resultRemotePdfStamp = Session["ResultRemotePdfStamp"].ToString();

                    if (resultRemotePdfStamp.ToLower().Equals("true"))
                    {
                        // Operazione di firma avvenuta con successo
                        Session.Remove("ResultRemotePdfStamp");
                        this.RefreshDocument();
                    }
                    else
                    {
                        Session.Remove("ResultRemotePdfStamp");
                        this.RefreshDocument();
                    }
                }

                //if (this.hf_segn_firma_automatica != null)
                //{
                //    if (!string.IsNullOrEmpty(this.hf_segn_firma_automatica.Value) && !this.hf_segn_firma_automatica.Value.ToLower().Equals("null"))
                //    {
                //        if (!string.IsNullOrEmpty(this.hf_segn_firma_automatica.Value) && this.hf_segn_firma_automatica.Value.ToLower().Equals("true"))
                //        {
                //            // Operazione di firma avvenuta con successo
                //            this.hf_segn_firma_automatica.Value = "Null";
                //            this.RefreshDocument();
                //        }
                //        else
                //        {
                //            this.hf_segn_firma_automatica.Value = "Null";
                //            this.RefreshDocument();
                //        }
                //    }
                //}
                #endregion
            }
            catch (System.Exception es)
            {
                ErrorManager.OpenErrorPage(this, es, " ");
            }
            logger.Info("END");
		}

        /// <summary>
        /// Reperimento estensione del file da visualizzare
        /// </summary>
        /// <returns></returns>
        protected string GetCurrentFileExtension()
        {
            DocsPaWR.FileDocumento fileDocument = new FileManager().getInfoFile(this);

            if (fileDocument != null)
            {
                return FileManager.getEstensioneIntoSignedFile(this, fileDocument.fullName).ToUpper();
            }
            else
            {
                return string.Empty;
            }
        }

        protected string HasSignedDocumentOnSession()
        {
            return string.Format("?SIGNED_DOCUMENT_ON_SESSION={0}", _SignedDocumentOnSession);
        }

        /// <summary>
        /// Reperimento dimensione del file da visualizzare
        /// </summary>
        /// <returns></returns>
        protected double GetCurrentFileSize()
        {
            DocsPaWR.FileRequest documento = FileManager.getSelectedFile(this);
            
            double fileSize;
            if (documento != null && double.TryParse(documento.fileSize, out fileSize))
                return fileSize;
            else
                return 0;
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
			this.btn_acquisisci.Click += new System.Web.UI.ImageClickEventHandler(this.btn_acquisisci_Click);
			this.btn_visualizza.Click += new System.Web.UI.ImageClickEventHandler(this.btn_visualizza_Click);
			//this.btn_Zoom.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Zoom_Click);
			this.btn_visualizza_con_segn.Click += new System.Web.UI.ImageClickEventHandler(this.btn_visualizza_con_segn_Click);
			this.btn_firma.Click += new System.Web.UI.ImageClickEventHandler(this.btn_firma_Click);
			this.btn_cofirma.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cofirma_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.tabDoc_PreRender);

            //
            // Firma HSM
            this.imgbtn_firmaHSM.Click += new System.Web.UI.ImageClickEventHandler(this.imgbtn_firmaHSM_Click);
            // End Firma HSM
            //

            #region Stampa Segnatura Firma Automatica External Service
            //this.btn_segn_firma_automatica.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampa_segn_automatica_Click);
            this.btn_segn_firma_automatica.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampa_segn_automatica_Click_ModalDialog);
            #endregion
		}

		#endregion

		private bool CtrlIfProt()
		{
           bool  rtn = false;
			try
			{
				if (schedaCorrente.protocollo != null)
				{
                    if (schedaCorrente.protocollo.GetType() == typeof(DocsPaWR.ProtocolloUscita))
                        rtn= false;
                    else if (schedaCorrente.repositoryContext != null &&
                            !string.IsNullOrEmpty(schedaCorrente.protocollo.numero))
                        rtn = true;
				}
				else
				{
                    rtn = false;
				}

                if (schedaCorrente.inCestino != null 
                    && schedaCorrente.inCestino == "1")
                    rtn = true;

				return rtn;
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		
            return true;
		}

		private void btn_visualizza_Click(object sender, System.Web.UI.ImageClickEventArgs e) 
		{
            if (Session["VisSegn"] != null)
                Session.Remove("VisSegn");
            DoShowDocument();
		}

        private void DoShowDocument()
        {
            logger.Info("BEGIN");
            //if (!this.GetControlAclDocumento().AclRevocata)
            //{
            if (FileManager.getSelectedFile(this) == null)
                return;

            DocsPaWR.FileRequest fr = FileManager.getSelectedFile(this);

            if (this.CtrlIfProt() == true)
            {
                this.btn_firma.Enabled = false; //deve stare a false

                //
                // Firma HSM
                this.imgbtn_firmaHSM.Visible = false;
                this.imgbtn_firmaHSM.Enabled = false;
                // End Firma HSM
                //
            }
            else
            {
                if (fr is DocsPaWR.Allegato && fr.repositoryContext != null)
                {
                    this.btn_firma.Enabled = false;

                    //
                    // Firma HSM
                    this.imgbtn_firmaHSM.Visible = false;
                    this.imgbtn_firmaHSM.Enabled = false;
                    // End Firma HSM
                    //
                }
                else
                {
                    // Pulsante di firma abilitato solo se il documento non è in stato checkout
                    this.btn_firma.Enabled = (this.schedaCorrente.checkOutStatus == null);

                    //
                    // Firma HSM
                    this.imgbtn_firmaHSM.Visible = (this.schedaCorrente.checkOutStatus == null) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                    this.imgbtn_firmaHSM.Enabled = (this.schedaCorrente.checkOutStatus == null) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                    // End Firma HSM
                    //
                }

                // verifico che il formato è ammesso per la firma
                if (!this.supportedFileTypeController.IsSignEnabledPerExtension(estensione))
                {
                    this.btn_firma.Enabled = false;

                    //
                    // Firma HSM
                    this.imgbtn_firmaHSM.Visible = false;
                    this.imgbtn_firmaHSM.Enabled = false;
                    // End Firma HSM
                    //
                }

                this.btn_cofirma.Enabled = this._fileFirmato && this.btn_firma.Enabled;

                if (DocsPAWA.UserManager.isFiltroAooEnabled(this.Page))
                {
                    DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);
                    if (btn_firma.Enabled)
                    {
                        btn_firma.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri);
                    }

                    if (btn_cofirma.Enabled)
                    {
                        btn_cofirma.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri);
                    }

                    //
                    // Firma HSM
                    if (imgbtn_firmaHSM.Enabled && imgbtn_firmaHSM.Visible)
                    {
                        imgbtn_firmaHSM.Visible = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                        imgbtn_firmaHSM.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                    }
                    // End Firma HSM
                    //
                }
            }

            fileDaVisualizzare = "docVisualizzaFrame.aspx?id=" + Session.SessionID;


            UserManager.removeBoolDocSalva(this);

            this.SetDocumentoVisualizzato(true);
            //}
                Session.Add("refreshDxPageVisualizzatore", true);

                logger.Info("END");
		}

		private void SetDocumentoVisualizzato(bool value)
		{
			if (this.ViewState["IS_DOCUMENTO_VISUALIZZATO"]==null)
				this.ViewState.Add("IS_DOCUMENTO_VISUALIZZATO",value);
			else
				this.ViewState["IS_DOCUMENTO_VISUALIZZATO"]=value;
		}

		private bool IsDocumentoVisualizzato()
		{	
			if (this.ViewState["IS_DOCUMENTO_VISUALIZZATO"]!=null)
				return (bool) (this.ViewState["IS_DOCUMENTO_VISUALIZZATO"]);
			else
				return false;			
		}

		/// <summary>
		/// Gestione abilitazione / disabilitazione pulsante
		/// visualizzazione dettagli firma.
		/// Abilitato se:
		/// - in inserimento solo se il documento firmato è stato acquisito e visualizzato
		/// - in modifica sempre se il documento firmato 
		/// </summary>
		private void EnableButtonDettagliFirma()
		{	
			string pathImage="../images/tabDocImages/";
			string imageName="firma_dettagli_nonattivo.gif";
			string cursorStyle="DEFAULT";

			DocsPaWR.FileRequest fileRequest=FileManager.getSelectedFile(this);
			if (fileRequest!=null)
			{
				//bool signedFile=(fileRequest.firmatari!=null && fileRequest.firmatari.Length > 0);

				if (this._fileFirmato)
				{
					cursorStyle="HAND";
					imageName="firma_dettagli_attivo.gif";

					//string eventHandler="function btn_DettagliFirma_onclick() { ShowMaskDettagliFirma(); }";
					//Response.Write("<script>" + eventHandler + "</script>");
                    ClientScript.RegisterStartupScript(this.GetType(), "OpenFirma", "function btn_DettagliFirma_onclick() { ShowMaskDettagliFirma(); }", true) ;

				}
			}

			fileRequest=null;

			// Impostazione immagine controllo
			System.Web.UI.HtmlControls.HtmlImage img=
				(System.Web.UI.HtmlControls.HtmlImage) this.FindControl("btn_DettagliFirma");
			img.Src=pathImage + imageName;
			img.Style.Add("CURSOR",cursorStyle);
			img=null;
		}

        private string GetCssAmministrazione()
        {
            string Tema = string.Empty;
            if (UserManager.getInfoUtente() != null)
            {
                string idAmm = UserManager.getInfoUtente().idAmministrazione;
                DocsPAWA.UserManager userM = new UserManager();
                Tema = userM.getCssAmministrazione(idAmm);
            }
            return Tema;
        }

		private void tabDoc_PreRender(object sender, System.EventArgs e)
		{
            logger.Info("BEGIN");
            // ~
            string css = string.Empty;
            string Tema = GetCssAmministrazione();
            if (Tema != null && !Tema.Equals(""))
            {
                string[] realTema = Tema.Split('^');
                css = realTema[0];
            }
            else
                css = "TemaRosso";

            string pathCss = "../App_Themes/" +  css + "/" + css + ".css";
           // pathCss = "../App_Themes/TemaBlu/TemaBlu.css";
            this.linkCss.Attributes.Add("href", pathCss);
            this.linkCss.Attributes.Add("rel", "stylesheet");
            this.linkCss.Attributes.Add("type", "text/css");

			//arrivo dalla popup di posizione segnatura		
            if (Session["refreshDocWithSegnature"] != null && Session["refreshDocWithSegnature"].Equals("true"))
            {
                if (Session["personalize"] != null)
                {
                    valore = Session["personalize"].ToString();
                }
                    
                if (Session["tipoLabel"] != null)
                {
                    valTipo = Session["tipoLabel"].ToString();
                }
                if (Session["rotazione"] != null)
                {
                    valRotaz = Session["rotazione"].ToString();
                }
                if (Session["carattere"] != null)
                {
                    valCarat = Session["carattere"].ToString();
                }
                if (Session["colore"] != null)
                {
                    valColor = Session["colore"].ToString();
                }
                if (Session["orientamento"] != null)
                {
                    valOrienta = Session["orientamento"].ToString();
                }
                //se viene richiesto di salvare il documento con la segnatura permanente
                if (Session["permanent_displays_segnature"] != null && Session["permanent_displays_segnature"].Equals("true"))
                {
                    // se il doc è in checkout non creo la nuova versione con la segnatura impressa sul doc
                    string checkOutMessage = string.Empty;
                    if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(out checkOutMessage))
                    {
                        this.RegisterClientScript("checkOutMessage", "alert('" + checkOutMessage + "');");
                        this.iFrameDoc.NavigateTo = "docVisualizzaFrame.aspx?id=" + Session.SessionID + "&plusEtic=1&pos=" + valore + "&tipo=" + valTipo + "&rotazione=" + valRotaz + "&carattere=" + valCarat + "&colore=" + valColor + "&orientamento=" + valOrienta;
                    }
                    else
                    {
                        //disabilito i pulsanti 'visualizza con segnatura', 'posiziona segnatura'
                        btn_visualizza_con_segn.Enabled = false;
                        btn_posiziona_segn.Enabled = false;
                        this.iFrameDoc.NavigateTo = "docVisualizzaFrame.aspx?id=" + Session.SessionID + "&plusEtic=1&pos=" + valore + "&tipo=" + valTipo + "&rotazione=" + valRotaz + "&carattere=" + valCarat + "&colore=" + valColor + "&orientamento=" + valOrienta + "&save=true";
                        RefreshLeftPanelTabVersion();
                    }
                    Page.Session.Remove("permanent_displays_segnature");
                }
                else
                    this.iFrameDoc.NavigateTo = "docVisualizzaFrame.aspx?id=" + Session.SessionID + "&plusEtic=1&pos=" + valore + "&tipo=" + valTipo + "&rotazione=" + valRotaz + "&carattere=" + valCarat + "&colore=" + valColor + "&orientamento=" + valOrienta;
                Session.Remove("refreshDocWithSegnature");
            }

			else
			{
				this.iFrameDoc.NavigateTo = fileDaVisualizzare;
			}

			if (schedaCorrente != null && schedaCorrente.protocollo!=null)
			{
				DocsPaWR.ProtocolloAnnullato protAnnull;
				protAnnull = schedaCorrente.protocollo.protocolloAnnullato;
				if (protAnnull != null && protAnnull.dataAnnullamento != null && !protAnnull.Equals(""))
				{
					this.btn_acquisisci.Enabled = false;
					this.btn_firma.Enabled = false;
					this.btn_cofirma.Enabled = false;

                    //
                    // Firma HSM
                    this.imgbtn_firmaHSM.Visible = false;
                    //this.imgbtn_firmaHSM.Enabled = false;
                    // End Firma HSM
                    //
				}
			}
			
			
			verificaHMdiritti();

			//abilitazione delle funzioni in base al ruolo
			UserManager.disabilitaFunzNonAutorizzate(this);

			//	Gestione Modello documento:
			//	viene abilitato il tasto per generare un documento da modello solo se
			//	la chiave sul webconfig della WA denominata "MODELLO_DOCUMENTO" è diversa da 0 (ZERO)	
			//  ed è stata attivata la profilazione dinamica
            this.SetVisibilityModelloDocumento();

            //Gestione label e pulsante di conversione pdf lato server
            impostazioniPdfLatoServer();

            //Abilitazione timestamp
            impostazioneTimestamp();

            if (schedaCorrente != null && schedaCorrente.tipoProto != null && schedaCorrente.tipoProto.Equals("C"))
            {
                this.btn_visualizza_con_segn.Enabled = false;
                this.btn_posiziona_segn.Enabled = false;
            }

            // Controllo su stato documento consolidato
            if (this.schedaCorrente != null && 
                this.schedaCorrente.ConsolidationState != null &&
                this.schedaCorrente.ConsolidationState.State >= DocsPaWR.DocumentConsolidationStateEnum.Step1)
            {
                this.DisabilitaTutto();
            }

            // HSM
            // se il pulsante è visibile, allora deve essre abilitato.
            if (this.imgbtn_firmaHSM.Visible)
            {
                if (!this.imgbtn_firmaHSM.Enabled)
                    this.imgbtn_firmaHSM.Visible = false;

                try
                {
                    // il pulsante è abilitato solo se il formato è ammesso per la firma
                    if (!Utils.IsFormatSupportedForSign(FileManager.getSelectedFile(this)))
                        this.imgbtn_firmaHSM.Enabled = false;
                }
                catch (Exception except) { }
            }
            // End HSM

            logger.Info("END");
		}

        /// <summary>
        /// Impostazione visibilità per il pulsante di generazione del documento a partire dal modello in amministrazione
        /// </summary>
        /// <summary>
        /// Impostazione visibilità per il pulsante di generazione del documento a partire dal modello in amministrazione
        /// </summary>
        private void SetVisibilityModelloDocumento()
        {
            //	Gestione Modello documento:
            //	viene abilitato il tasto per generare un documento da modello solo se
            //	la chiave sul webconfig della WA denominata "MODELLO_DOCUMENTO" è diversa da 0 (ZERO)	
            //  ed è stata attivata la profilazione dinamica
            if (
                (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
                &&
                (System.Configuration.ConfigurationManager.AppSettings["MODELLO_DOCUMENTO"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["MODELLO_DOCUMENTO"] != "0")
                )
            {
                DocsPaWR.SchedaDocumento schedaDocumentoSelezionata = null;

                this.btn_modello.Enabled = false;

                if (this.IsSelectedTabAllegati)
                {
                    // Selezione allegato da tab allegati nella scheda documento principale

                    if (FileManager.getSelectedFile() == null)
                    {
                        // Nessun allegato selezionato, pulsante disabilitato
                        this.btn_modello.Enabled = false;

                        return;
                    }
                    else
                    {
                        schedaDocumentoSelezionata = CheckInOut.CheckInOutServices.CurrentSchedaDocumento;

                        //DOCUMENTO
                        // Il pulsante è abilitato solamente se:
                        // - l'utente ha i diritti per l'utilizzo delle funzionalità di checkout
                        // - il documento principale sia tipizzato
                        // - l'allegato non è in stato checkout
                        // - il documento principale non è in stato checkOut
                        // - in caso il documento principale sia un protocollo, questo non sia annullato
                        // - il documento principale non sia nel cestino
                        this.btn_modello.Enabled = CheckInOut.CheckInOutServices.UserEnabled &&
                                                        schedaDocumentoSelezionata != null &&
                                                        this.schedaCorrente.template != null &&
                                                        this.schedaCorrente.checkOutStatus == null &&
                                                        schedaDocumentoSelezionata.checkOutStatus == null &&
                                                        !this.IsDocumentoAnnullato &&
                                                        !this.IsDocumentoInCestino
                                                    &&
                                                    ((schedaDocumentoSelezionata.documenti != null &&
                                                        schedaDocumentoSelezionata.documenti.Length == 1 &&
                                                        schedaDocumentoSelezionata.template != null &&
                                                        !string.IsNullOrEmpty(schedaDocumentoSelezionata.template.PATH_MODELLO_1)
                                                        ) ||
                                                        (
                                                        schedaDocumentoSelezionata.documenti != null &&
                                                        schedaDocumentoSelezionata.documenti.Length > 1 &&
                                                        schedaDocumentoSelezionata.template != null &&
                                                        !string.IsNullOrEmpty(schedaDocumentoSelezionata.template.PATH_MODELLO_2)
                                                        )

                                                        );
                    }
                }
                else
                {
                    this.td_label_modello.InnerText = "Modello";
                    this.btn_modello.Visible = true;

                    schedaDocumentoSelezionata = this.schedaCorrente;

                    //DOCUMENTO
                    // Il pulsante è abilitato solamente se:
                    // - l'utente ha i diritti per l'utilizzo delle funzionalità di checkout
                    // - il documento non è in stato checkOut
                    // - in caso di protocollo, non sia annullato				
                    this.btn_modello.Enabled = CheckInOut.CheckInOutServices.UserEnabled &&
                                                    schedaDocumentoSelezionata != null &&
                                                    schedaDocumentoSelezionata.template != null &&
                                                    !this.IsDocumentoAnnullato &&
                                                    !this.IsDocumentCheckedOut() &&
                                                    !this.IsDocumentoInCestino
                        &&
                        ((schedaDocumentoSelezionata.documenti != null &&
                            schedaDocumentoSelezionata.documenti.Length == 1 &&
                            schedaDocumentoSelezionata.template != null &&
                            !string.IsNullOrEmpty(schedaDocumentoSelezionata.template.PATH_MODELLO_1)
                            ) ||
                            (
                            schedaDocumentoSelezionata.documenti != null &&
                            schedaDocumentoSelezionata.documenti.Length > 1 &&
                            !string.IsNullOrEmpty(schedaDocumentoSelezionata.template.PATH_MODELLO_2)
                            )

                                                        );
                }

                //ALLEGATO
                if (schedaDocumentoSelezionata != null && !this.IsDocumentoInCestino)
                {
                    if (schedaDocumentoSelezionata.documentoPrincipale != null)
                    {
                        //Abilito il pulsante modello se la scheda doc corrente è quella di un allegato
                        //ed il suo documento principale è di una certa tipologia, non è in checkout e non è annullato
                        DocsPaWR.SchedaDocumento schedaDocumentoPrincipale = DocumentManager.getDettaglioDocumento(this, schedaDocumentoSelezionata.documentoPrincipale.idProfile, schedaDocumentoSelezionata.documentoPrincipale.docNumber);


                        if (schedaDocumentoPrincipale.tipologiaAtto != null && !this.IsDocumentCheckedOut() && !this.IsDocumentoAnnullato && 
                            (schedaDocumentoPrincipale.template != null &&
                            !string.IsNullOrEmpty(schedaDocumentoPrincipale.template.PATH_ALLEGATO_1) &&
                            schedaDocumentoPrincipale.allegati != null && schedaDocumentoPrincipale.allegati.Length > 0)
                            )
                        {
                            this.btn_modello.Enabled = true;
                            if (DocsPAWA.UserManager.isFiltroAooEnabled(this.Page))
                            {
                                DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);
                                btn_modello.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri);
                            }
                        }
                        else
                        {
                            this.btn_modello.Enabled = false;
                        }
                    }
                }

                if (schedaDocumentoSelezionata != null &&
                    schedaDocumentoSelezionata.ConsolidationState != null &&
                    schedaDocumentoSelezionata.ConsolidationState.State > DocsPaWR.DocumentConsolidationStateEnum.None)
                {
                    // Documento consolidato, impossibile  generare il modello
                    this.btn_modello.Enabled = false;
                }

                if (this.btn_modello.Enabled)
                {
                    /* Integrazione M/TEXT
                     * Se il modello associato è di tipo M/TEXT, associa la logica M/TEXT
                     */
                    if (schedaDocumentoSelezionata.documentoPrincipale != null && string.IsNullOrEmpty(schedaDocumentoSelezionata.documentoPrincipale.idProfile))
                        schedaDocumentoSelezionata.template = DocumentManager.getDettaglioDocumento(this.Page, schedaDocumentoSelezionata.documentoPrincipale.idProfile, schedaDocumentoSelezionata.documentoPrincipale.idProfile).template;

                    if (schedaDocumentoSelezionata.template != null && schedaDocumentoSelezionata.template.PATH_MODELLO_1_EXT.ToLower().Equals("mtxt"))          // INSERIRE CONDIZIONE PER DISCRIMINARE M/TEXT
                        this.btn_modello.Attributes.Add("onClick", "ProcessMTEXTModel('" + schedaDocumentoSelezionata.systemId + "','" + schedaDocumentoSelezionata.docNumber + "');");
                    else
                        this.btn_modello.Attributes.Add("onClick", "ProcessDocumentModel('" + schedaDocumentoSelezionata.systemId + "','" + schedaDocumentoSelezionata.docNumber + "');");
                }
            }
            else
            {
                this.td_label_modello.InnerHtml = "&nbsp;";
                this.btn_modello.Visible = false;
            }
        }

        /// <summary>
        /// Verifica se il documento corrente è cestinato
        /// </summary>
        private bool IsDocumentoInCestino
        {
            get
            {
                return (schedaCorrente != null &&
                            schedaCorrente.inCestino != null &&
                            schedaCorrente.inCestino == "1");
            }
        }

		/// <summary>
		/// Verifica se il documento corrente è annullato
		/// </summary>
		private bool IsDocumentoAnnullato
		{
			get
			{
				return (schedaCorrente!=null && 
					schedaCorrente.protocollo!=null &&
					schedaCorrente.protocollo.protocolloAnnullato!=null);
			}
		}

		private void verificaHMdiritti()
		{
			//disabilitazione dei bottoni in base all'autorizzazione di HM 
			//sul documento
			if(schedaCorrente!=null && schedaCorrente.accessRights!=null && schedaCorrente.accessRights!="")
			{
				if(UserManager.disabilitaButtHMDiritti(schedaCorrente.accessRights))
				{
					//bottoni che devono essere disabilitati in caso
					//di diritti di sola lettura
                    DisabilitaTutto();
				}
			}
		}

        private void DisabilitaTutto()
        {
            this.btn_acquisisci.Enabled = false;
            this.btn_firma.Enabled = false;
            this.btn_cofirma.Enabled = false;

            //
            // Firma HSM
            this.imgbtn_firmaHSM.Visible = false;
            this.imgbtn_firmaHSM.Enabled = false;
            // End Firma HSM
            //
        }

		private void iFrameDoc_Navigate(object sender, System.EventArgs e)
		{
		
		}

		/// <summary>
		/// Registrazione script client per la firma digitale
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterScriptFirmaDigitale()
		{
            string checkOutMessage;

            // Registrazione degli script per consentire la firma digitale,
            // solamente se il documento / allegato non è in stato checkout

            if (!CheckInOut.CheckInOutServices.IsCheckedOutDocument(out checkOutMessage))
            {
                this.btn_firma.Attributes.Add("onClick", "return ShowDialogFirmaDigitale('sign');");
                this.btn_cofirma.Attributes.Add("onClick", "return ShowDialogFirmaDigitale('cosign');");
            }
            else
            {
                this.btn_firma.Attributes.Add("onclick", string.Format("alert('{0}'); return false;", checkOutMessage));
                this.btn_firma.Attributes.Add("onclick", string.Format("alert('{0}'); return false;", checkOutMessage));
            }
		}

        private void RegisterScriptFirmaHSM()
        {
            string checkOutMessage;

            string serviceNameSignHSM = string.Empty;

            //
            // Reperimento Servizio Firma HSM
            // Serve per definire i campi visibili nella popup HSM
            if (System.Configuration.ConfigurationManager.AppSettings["HSM_SERVICE_NAME"] != null)
                serviceNameSignHSM = System.Configuration.ConfigurationManager.AppSettings["HSM_SERVICE_NAME"];

            // Registrazione degli script per consentire la firma digitale,
            // solamente se il documento / allegato non è in stato checkout
            if (!CheckInOut.CheckInOutServices.IsCheckedOutDocument(out checkOutMessage))
            {
                this.imgbtn_firmaHSM.Attributes.Add("onClick", "return ShowDialogFirmaHSM('" + serviceNameSignHSM + "');");
                //this.imgbtn_firmaHSM.Attributes.Add("onClick", "return ShowDialogFirmaHSM('" + serviceNameSignHSM + ","+ schedaCorrente.systemId + "," + schedaCorrente.docNumber + "');");
            }
            else
            {
                this.imgbtn_firmaHSM.Attributes.Add("onClick", string.Format("alert('{0}'); return false;", checkOutMessage));
            }
        }

		/// <summary>
		/// Azione firma digitale del documento
		/// </summary>
		private void SignDocument()
		{
			string checkOutMessage;
			
			if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(out checkOutMessage))
			{
				// Se il documento è in stato checkOut, viene visualizzato un messaggio di notifica
				this.RegisterClientScript("checkOutMessage","alert('" + checkOutMessage + "');");
			}
		}

		/// <summary>
		/// Azione di cofirma digitale del documento
		/// </summary>
		private void CosignDocument()
		{
			string checkOutMessage;
			
			if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(out checkOutMessage))
			{
				// Se il documento è in stato checkOut, viene visualizzato un messaggio di notifica
				this.RegisterClientScript("checkOutMessage","alert('" + checkOutMessage + "');");
			}
		}

		private void btn_firma_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            this.SignDocument();
            this.RefreshLeftPanelIfAllegato();

            //Abbatangeli Gianluigi - Refresh del documento
            this.RefreshLeftPanelIfLiveCycle();
            this.RefreshDocument();
		}

		private void btn_cofirma_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            this.CosignDocument();
            this.RefreshLeftPanelIfAllegato();

            //Abbatangeli Gianluigi - Refresh del documento
            this.RefreshLeftPanelIfLiveCycle();
            this.RefreshDocument();
		}

		/*private void btn_Zoom_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            //if (!this.GetControlAclDocumento().AclRevocata)
            //{
            try
            {
                string pageUrl = "docVisualizzaFrame.aspx?id=" + Session.SessionID;
                if (Session["personalize"] != null)
                {
                    valore = Session["personalize"].ToString();
                }
                if (Session["tipoLabel"] != null)
                {
                    valTipo = Session["tipoLabel"].ToString();
                }
                if (Session["rotazione"] != null)
                {
                    valRotaz = Session["rotazione"].ToString();
                }
                if (Session["carattere"] != null)
                {
                    valCarat = Session["carattere"].ToString();
                }
                if (Session["colore"] != null)
                {
                    valColor = Session["colore"].ToString();
                }
                if (Session["orientamento"] != null)
                {
                    valOrienta = Session["orientamento"].ToString();
                }
                if (
                    schedaCorrente != null && schedaCorrente.protocollo != null
                    && schedaCorrente.protocollo.segnatura != null
                    && schedaCorrente.protocollo.segnatura != ""
                    && (Session["VisSegn"] != null)
                    )
                {

                    pageUrl = "docVisualizzaFrame.aspx?id=" + Session.SessionID + "&plusEtic=1&pos=" + valore + "&tipo=" + valTipo + "&rotazione=" + valRotaz + "&carattere=" + valCarat + "&colore=" + valColor + "&orientamento=" + valOrienta;
                    Session.Remove("VisSegn");
                }

                string targetName = "zoom";
                string script = "<script language=JavaScript>%1%</script>";
                string scriptBody = null;
                scriptBody = "ApriFinestraMassimizzata('" + pageUrl + "','" + targetName + "')";
                script = script.Replace("%1%", scriptBody);
                this.RegisterStartupScript("ZoomDoc", script);
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
            //}
		}*/

		private void btn_visualizza_con_segn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            if (Session["personalize"] != null)
                Session.Remove("personalize");
            if (Session["colore"] != null)
                Session.Remove("colore");
            if (Session["carattere"] != null)
                Session.Remove("carattere");

            //Mev Firma1 < inizializza  
            if (Session["printFormatSign"] != null)
                Session.Remove("printFormatSign");
            if (Session["notimbro"] != null)
                Session.Remove("notimbro");
            //>

            // Garantisce che nella selezione del comando “Visualizza dati con segnatura” siano sempre e 
            // solo mostrati i dati identificativi del documento. 
            // Non vengono mantenute in sessione eventuali selezioni operate nella popup di posizionamento dati.
            Session.Add("SHOWDOCWITHSEGNATURE", "TRUE");
            //>


            //if (!this.GetControlAclDocumento().AclRevocata)
            //{
                btn_posiziona_segn.Enabled = true;
                if (FileManager.getSelectedFile(this) == null)
                    return;

                if (this.CtrlIfProt() == true)
                {
                    this.btn_firma.Enabled = false; //deve stare a false

                    //
                    // Firma HSM
                    this.imgbtn_firmaHSM.Visible = false;
                    //this.imgbtn_firmaHSM.Enabled = false;
                    // End Firma HSM
                    //
                }
                else
                {
                    this.btn_firma.Enabled = (this.schedaCorrente.checkOutStatus == null);
                    this.btn_cofirma.Enabled = this._fileFirmato && this.btn_firma.Enabled;

                    //
                    // Firma HSM
                    this.imgbtn_firmaHSM.Visible = (this.schedaCorrente.checkOutStatus == null) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                    this.imgbtn_firmaHSM.Enabled = (this.schedaCorrente.checkOutStatus == null) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                    // End Firma HSM
                    //

                    if (DocsPAWA.UserManager.isFiltroAooEnabled(this.Page))
                    {
                        DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);

                        if (btn_firma.Enabled)
                        {
                            btn_firma.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri);
                        }

                        if (btn_cofirma.Enabled)
                        {
                            btn_cofirma.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri);
                        }

                        //
                        // Firma HSM
                        if (imgbtn_firmaHSM.Enabled && imgbtn_firmaHSM.Visible)
                        {
                            imgbtn_firmaHSM.Visible = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                            imgbtn_firmaHSM.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                        }
                        // End Firma HSM
                        //
                    }
                }

                DocsPaWR.FileRequest fr = FileManager.getSelectedFile(this);

                FileManager fileMan = new FileManager();

                DocsPaWR.FileDocumento fileDocum = fileMan.getInfoFile(Page);

                // Estensione del file da convertire
                string ext = FileManager.getEstensioneIntoSignedFile(this, fileDocum.name).ToUpper();

                bool supportedType = (ext.Equals("PDF") || PdfConverterInfo.CanConvertFileToPdf(fileDocum.name));

                    if (schedaCorrente != null &&

                            (
                        //protocolli
                            (schedaCorrente.protocollo != null
                            && schedaCorrente.protocollo.segnatura != null
                            && schedaCorrente.protocollo.segnatura != "")

                            ||

                            //predisposti
                            (schedaCorrente.protocollo != null
                            && string.IsNullOrEmpty(schedaCorrente.protocollo.segnatura)
                            && !string.IsNullOrEmpty(schedaCorrente.systemId))

                            ||

                            //grigi
                            (schedaCorrente.protocollo == null
                            && !string.IsNullOrEmpty(schedaCorrente.systemId))

                            )

                            && supportedType)
                {

                    fileDaVisualizzare = "docVisualizzaFrame.aspx?id=" + Session.SessionID + "&plusEtic=1&pos=" + valore + "&tipo=" + valTipo + "&rotazione=" + valRotaz + "&carattere=" + valCarat + "&colore=" + valColor + "&orientamento=" + valOrienta;
                    Session.Add("VisSegn", "1");
                }
                UserManager.removeBoolDocSalva(this);

                this.SetDocumentoVisualizzato(true);
            //}

		}

		#region Gestione CheckIn / CheckOut

		/// <summary>
		/// Verifica se il documento corrente è in stato checkout
		/// </summary>
		/// <returns></returns>
		private bool IsDocumentCheckedOut()
		{
			bool retValue=false;

			if (schedaCorrente!=null && schedaCorrente.systemId!=null &&
				CheckInOut.CheckOutContext.Current!=null && CheckInOut.CheckOutContext.Current.Status!=null)
			{
				DocsPaWR.CheckOutStatus status=CheckInOut.CheckOutContext.Current.Status;

				retValue=(schedaCorrente.systemId.Equals(status.IDDocument));
			}

			return retValue;
		}

		/// <summary>
		/// Handler dell'evento "OnCheckIn"
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCheckInDocument(object sender,EventArgs e)
		{	
			// Aggiornamento documento corrente
			this.RefreshDocument();
		}
	
		/// <summary>
		/// Aggiornamento documento corrente
		/// </summary>
		private void RefreshDocument()
		{
			// Reperimento scheda documento corrente
			DocsPaWR.SchedaDocumento schedaDocumento=DocumentManager.getDettaglioDocumento(this,schedaCorrente.systemId,schedaCorrente.docNumber);
			DocumentManager.setDocumentoSelezionato(this,schedaDocumento);

			// Aggiornamento pagina visualizzata
			string function="window.top.principale.iFrame_sx.document.location=window.top.principale.iFrame_sx.document.location.href;";
			Response.Write("<script>" + function + "</script>");
		}

		/// <summary>
		/// Inizializzazione pannello pulsanti di CheckIn - Checkout
		/// </summary>
		private void InitializeCheckInOutPanel()
		{
			CheckInOut.CheckInOutPanel ctl=this.FindControl("checkInOutPanel") as CheckInOut.CheckInOutPanel;
			ctl.OnCheckIn+=new EventHandler(this.OnCheckInDocument);

			if (ctl!=null && !this.IsPostBack)
			{
				ctl.Initialize(schedaCorrente.systemId,schedaCorrente.docNumber);
			}           
		}

		#endregion

		/// <summary>
		/// Azione di acquisizione documenti
		/// </summary>
		private void RegisterAcquireDocumentScript()
		{
			// Se il documento corrente è un allegato, è possibile
			// acquisire il file anche se il documento è in stato "CheckOut"
			DocsPaWR.FileRequest fileRequest=FileManager.getSelectedFile(this);
			bool allegato=(fileRequest!=null && fileRequest.GetType() == typeof(DocsPAWA.DocsPaWR.Allegato));

            string checkOutMessage;

            if (!CheckInOut.CheckInOutServices.IsCheckedOutDocument(out checkOutMessage) || allegato)
                this.btn_acquisisci.Attributes.Add("onclick", "ShowAcquire();");
            else
                this.btn_acquisisci.Attributes.Add("onclick", string.Format("alert('{0}'); return false;", checkOutMessage));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btn_acquisisci_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            if (!CheckInOut.CheckInOutServices.IsCheckedOutDocument())
            {
                this.RefreshLeftPanelIfAllegato();
                this.RefreshLeftPanelIfLiveCycle();
            }
		}

        /// <summary>
        /// Aggiornamento dettaglio del documento, 
        /// solamente in caso di visualizzazione degli allegati
        /// </summary>
        protected void RefreshLeftPanelIfAllegato()
        {
            if (this.IsEnabledProfilazioneAllegati)
            {
                // Solamente se la nuova gestione degli allegati risulta abilitata

                SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

                if (currentContext.ContextName == SiteNavigation.NavigationKeys.DOCUMENTO)
                {
                    if (currentContext.QueryStringParameters.ContainsKey("tab") &&
                        currentContext.QueryStringParameters["tab"].ToString().ToLower() == "allegati")
                    {
                        // Nel caso in cui si è nel contesto di visualizzazione degli allegati
                        // di un documento è necessario, successivamente l'acquisizione,
                        // aggiornare il dettaglio del documento

                        // Dal contesto corrente, vengono rimossi i parametri querystring
                        // "isNew" e "forceNewContext", in quanto non è necessario creare
                        // un nuovo contesto
                        if (currentContext.QueryStringParameters.ContainsKey("isNew"))
                            currentContext.QueryStringParameters.Remove("isNew");

                        if (currentContext.QueryStringParameters.ContainsKey("forceNewContext"))
                            currentContext.QueryStringParameters["forceNewContext"] = false;

                        this.RegisterClientScript("RefreshLeftPanel", string.Format("top.principale.iFrame_sx.document.location = '{0}'", currentContext.Url));
                    }
                }
            }
        }

        protected void RefreshLeftPanelIfLiveCycle()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null && currentContext.ContextName == SiteNavigation.NavigationKeys.DOCUMENTO)
            {
                if (currentContext.ContextState.ContainsKey("ProcessPdfLiveCycle"))
                {
                    this.RegisterClientScript("RefreshLeftPanel", string.Format("top.principale.iFrame_sx.document.location = '{0}'", currentContext.Url));                    
                }
            }
        }
        /// <summary>
        /// Permette il refresh del tab versioni quando si crea una nuova versione con segnatura impressa nel file associato alla versione.
        /// </summary>
        protected void RefreshLeftPanelTabVersion()
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext.QueryStringParameters["tab"] != null && currentContext.QueryStringParameters["tab"].Equals("versioni"))
                this.RegisterClientScript("RefreshLeftPanel", string.Format("top.principale.iFrame_sx.document.location = '{0}'", currentContext.Url));
        }

        /// <summary>
        /// Determina se è attiva la gestione della profilazione degli allegati
        /// </summary>
        protected bool IsEnabledProfilazioneAllegati
        {
            get
            {
                const string key = "IsEnabledProfilazioneAllegati";

                if (this.ViewState[key] == null)
                {
                    DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    this.ViewState.Add(key, ws.IsEnabledProfilazioneAllegati());
                }

                return Convert.ToBoolean(this.ViewState[key]);
            }
        }

        /// <summary>
        /// Verifica, in base al contesto corrente, se il tab corrente della scheda documento è quella degli allegati
        /// </summary>
        /// <returns></returns>
        protected bool IsSelectedTabAllegati
        {
            get
            {
                SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;

                string currentTab = context.QueryStringParameters["tab"] as string;

                if (!string.IsNullOrEmpty(currentTab))
                    return (currentTab.ToLower() == "allegati");
                else
                    return false;
            }
        }

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}
 
        #region Gestione controllo acl documento
        /*
        /// <summary>
        /// Inizializzazione controllo verifica acl
        /// </summary>
        protected virtual void InitializeControlAclDocumento()
        {
            AclDocumento ctl = this.GetControlAclDocumento();
            ctl.IdDocumento = DocumentManager.getDocumentoSelezionato().systemId;
            ctl.OnAclRevocata += new EventHandler(this.OnAclDocumentoRevocata);
        }

        /// <summary>
        /// Reperimento controllo acldocumento
        /// </summary>
        /// <returns></returns>
        protected AclDocumento GetControlAclDocumento()
        {
            return (AclDocumento)this.FindControl("aclDocumento");
        }

        /// <summary>
        /// Listener evento OnAclDocumentoRevocata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAclDocumentoRevocata(object sender, EventArgs e)
        {
            // Redirect alla homepage di docspa
            SiteNavigation.CallContextStack.Clear();
            SiteNavigation.NavigationContext.RefreshNavigation();
            string script = "<script>window.open('../sceltaRuoloNew.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
            Response.Write(script);
        }
        */
        #endregion

        #region Gest conversione lato server

        private void impostazioniPdfLatoServer()
        {
            //Reset
            img_converti.Visible = false;
            lbl_converti.Text = "";
            lbl_converti.Visible = false;
            pnl_converti.Visible = false;

            // Reperimento della scheda documento correntemente selezionata nel contesto del checkout
            DocsPaWR.SchedaDocumento documentoSelezionato = CheckInOut.CheckInOutServices.CurrentSchedaDocumento;

            if (DocsPAWA.Utils.isEnableConversionePdfLatoServer() == "true")
            {
                if (documentoSelezionato != null &&
                    documentoSelezionato.ConsolidationState != null &&
                    documentoSelezionato.ConsolidationState.State >= DocsPaWR.DocumentConsolidationStateEnum.Step1)
                {
                    // Il documento consolidato non può essere convertito
                    pnl_converti.Visible = false;
                    return;
                }

                DocsPaWR.FileRequest fileRequest = FileManager.getSelectedFile();

                //DISABILITO : se non esiste un file selezionato
                if (fileRequest == null)
                {
                    pnl_converti.Visible = false;
                    return;
                }

                //DISABILITO : se il documento è nuovo
                if (documentoSelezionato == null || documentoSelezionato.docNumber == "")
                {
                    pnl_converti.Visible = false;
                    return;
                }

                //DISABILITO : se non esiste un file acquisito
                if (string.IsNullOrEmpty(fileRequest.fileName))
                {
                    pnl_converti.Visible = false;
                    return;
                }

                int fileSize;
                Int32.TryParse(fileRequest.fileSize, out fileSize);

                if (fileSize == 0)
                {
                    pnl_converti.Visible = false;
                    return;
                }

                bool isAllegato = (fileRequest.GetType() == typeof(DocsPaWR.Allegato));








                if (!isAllegato)
                {
                    //documentoSelezionato = this.schedaCorrente;

                    //DISABILITO : se non è selezionata l'ultima versione del doc
                    // Non se si è in selezione di un allegato: da tab allegati viene sempre visualizzata l'ultima versione dell'allegato
                    if (documentoSelezionato.documenti != null &&
                        documentoSelezionato.documenti.Length > 1 &&
                        !fileRequest.version.Equals(documentoSelezionato.documenti[0].version))
                    {
                        pnl_converti.Visible = false;
                        return;
                    }
                }
                //else
                //    documentoSelezionato = CheckInOut.CheckInOutServices.CurrentSchedaDocumento;


                //DISABILITO : se esiste un file acquisito firmato
                if (fileRequest.fileName.ToUpper().EndsWith("P7M"))
                {
                    pnl_converti.Visible = false;
                    return;
                }

                //DISABILITO : se esiste un file acquisito ed è già un PDF
                if (fileRequest.fileName.ToUpper().EndsWith("PDF"))
                {
                    pnl_converti.Visible = false;
                    this.img_tipo.ImageUrl = FileManager.getFileIcon(this, "PDF");
                    return;
                }

                //DISABILITO : se il documento è bloccato ma non per una conversione 
                if (//CheckInOut.CheckInOutServices.IsCheckedOutDocument() &&
                    documentoSelezionato.checkOutStatus != null && !documentoSelezionato.checkOutStatus.InConversionePdf)
                {

                    pnl_converti.Visible = false;

                    return;
                }

                //Controllo Pacchetto Zanotti
                if (UserManager.disabilitaButtHMDiritti(documentoSelezionato.accessRights))
                {
                    pnl_converti.Visible = false;
                    return;
                }

                if (UserManager.isFiltroAooEnabled(this))
                {
                    DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);
                    this.pnl_converti.Visible = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri);
                    return;
                }

                if (documentoSelezionato != null && !string.IsNullOrEmpty(documentoSelezionato.systemId))
                {
                    //Verifico se il documento è già in conversione

                    if (documentoSelezionato.checkOutStatus != null && documentoSelezionato.checkOutStatus.InConversionePdf)
                    {
                        img_converti.Visible = false;
                        lbl_converti.Text = "in conversione ...";
                        lbl_converti.Visible = true;
                        pnl_converti.Visible = true;
                        ClientScript.RegisterStartupScript(this.GetType(), "disableDdlStati", "disableDdlStati();", true);
                        return;
                    }

                    //Documento che può essere mandato in conversione
                    ClientScript.RegisterStartupScript(this.GetType(), "enableDdlStati", "enableDdlStati();", true);
                    img_converti.Visible = true;
                    lbl_converti.Text = "";
                    lbl_converti.Visible = false;
                    //modifica
                    if (schedaCorrente != null)
                    {
                        DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                      //  HttpServerUtility server = HttpContext.Current.Server;
                        string idAmm = UserManager.getInfoUtente().idAmministrazione;
                        DocsPAWA.DocsPaWR.CacheConfig info = ws.getConfigurazioneCache(idAmm);
                        bool inCache = false;
                        string pathComponents = ws.recuperaPathComponents(schedaCorrente.documenti[0].docNumber, schedaCorrente.documenti[0].versionId);
                        DocsPaWR.FileRequest fr = FileManager.getSelectedFile(this);
                        if (fr == null)
                        {
                            if (!string.IsNullOrEmpty(schedaCorrente.docNumber))
                                inCache = ws.inCache(schedaCorrente.docNumber, schedaCorrente.documenti[0].versionId,idAmm);
                        }
                        else
                            inCache = ws.inCache(fr.docNumber, fr.versionId,idAmm);

                        if (info != null
                        && !info.caching
                        && string.IsNullOrEmpty(pathComponents)
                        && inCache)
                            pnl_converti.Visible = false;
                        else
                    pnl_converti.Visible = true;
                    }
                    else
                        //modifica
                        pnl_converti.Visible = true;

                    return;
                }
            }
        }

        protected void img_firmato_Click(object sender, ImageClickEventArgs e)
        {
            DocsPaWR.FileDocumento signedDocument = null;
            DocsPaWR.FileRequest fileRequest = FileManager.getSelectedFile(this);

            if (fileRequest != null && fileRequest.fileName != null && fileRequest.fileName != "")
            {
                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);

                DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
                signedDocument = docsPaWS.VerificaValiditaFirma(fileRequest, infoUtente);
                DocumentManager.SetSignedDocument(signedDocument);
                _SignedDocumentOnSession = true;
                docsPaWS = null;

                if (signedDocument != null)
                {
                    if (signedDocument.signatureResult != null)
                    {
                        if (signedDocument.signatureResult.StatusCode == 0)
                        {
                            img_firmato.BackColor = Color.LawnGreen;
                            img_firmato.ToolTip = "Controllo CRL OK";
                        }
                        else
                        {
                            img_firmato.BackColor = Color.Red;
                            img_firmato.ToolTip = "Controllo CRL Fallito! Click su dettagli firma per maggiori informazioni";
                        }
                    }
                }

            }

        }

        protected void img_converti_Click(object sender, ImageClickEventArgs e)
        {
            if (schedaCorrente != null)
            {
                FileManager fileManager = new FileManager();
                DocsPaWR.FileDocumento fileDocumento = fileManager.getFile(this);

                if (fileDocumento != null &&
                        fileDocumento.content != null &&
                        !string.IsNullOrEmpty(fileDocumento.name))
                {
                    FileManager.EnqueueServerPdfConversion(this,
                                    UserManager.getInfoUtente(this),
                                    fileDocumento.content,
                                    fileDocumento.name,
                                    CheckInOut.CheckInOutServices.CurrentSchedaDocumento);

                    //Aggiornamento contesto checkInOut
                    CheckInOut.CheckInOutServices.RefreshCheckOutStatus();


                    ClientScript.RegisterStartupScript(this.GetType(), "disableDdlStati", "disableDdlStati();", true);
                }
            }
        }

        #endregion

        #region Gestione elaborazione modelli con word processor client side

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool HasModelProcessorSelected()
        {
            return this.clientModelProcessor.HasModelProcessorSelected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetModelProcessorSupportedExtensions()
        {
            return this.clientModelProcessor.SupportedExtensions;
        }

        /// <summary>
        /// Calcolo del modello per la stampa del documento correntemente in lavorazione
        /// </summary>
        protected string ModelloDocumentoCorrente
        {
            get
            {
                string modello = string.Empty;

                if (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId))
                {
                    if (this.schedaCorrente.documentoPrincipale != null)
                    {
                        //Modello di allegato
                        modello = "A1";
                    }
                    else
                    {
                        //if (this.IsEnabledProfilazioneAllegati && this.IsSelectedTabAllegati)
                        if (this.IsSelectedTabAllegati)
                        {
                            // Modello allegato da tab allegati della scheda documento principale
                            modello = "A1";
                        }
                        else
                        {
                            // Reperimento versione corrente del documento
                            DocsPaWR.FileRequest fileRequest = this.schedaCorrente.documenti[0];

                            if (fileRequest != null &&
                                Convert.ToInt32(fileRequest.version) >= 1 &&
                                Convert.ToInt32(fileRequest.fileSize) > 0)
                            {
                                if (System.Configuration.ConfigurationManager.AppSettings["MODELLO_DOCUMENTO"] != null &&
                                    System.Configuration.ConfigurationManager.AppSettings["MODELLO_DOCUMENTO"] == "2")
                                    //Modello Due
                                    modello = "2";
                                else
                                    // Modello Uno
                                    modello = "1";
                            }
                            else
                            {
                                // Modello Uno
                                modello = "1";
                            }
                        }
                    }
                }

                return modello;
            }
        }

        #endregion

        protected DocsPaWR.InfoUtente InfoUtente
        {
            get
            {
                return UserManager.getInfoUtente();
            }
        }

        protected void img_tipo_Click(object sender, ImageClickEventArgs e)
        {
            // Se il ruolo è abilitato...
            if (UserManager.ruoloIsAutorized(this, "DO_VIS_ORIG"))
            {
                // ...allora bisogna visualizzare il file nel suo formato
                // originale
                if (FileManager.getSelectedFile(this) == null)
                    return;

                if (this.CtrlIfProt() == true)
                {
                    this.btn_firma.Enabled = false; //deve stare a false

                    //
                    // Firma HSM
                    this.imgbtn_firmaHSM.Visible = false;
                    //this.imgbtn_firmaHSM.Enabled = false;
                    // End Firma HSM
                    //

                }
                else
                {
                    this.btn_firma.Enabled = (this.schedaCorrente.checkOutStatus == null);
                    this.btn_cofirma.Enabled = (this.btn_firma.Enabled && this._fileFirmato);

                    //
                    // Firma HSM
                    this.imgbtn_firmaHSM.Visible = (this.schedaCorrente.checkOutStatus == null) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                    this.imgbtn_firmaHSM.Enabled = (this.schedaCorrente.checkOutStatus == null) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                    // End Firma HSM
                    //

                    if (DocsPAWA.UserManager.isFiltroAooEnabled(this.Page))
                    {
                        DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);
                        if (btn_firma.Enabled)
                        {
                            btn_firma.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri);
                        }

                        if (btn_cofirma.Enabled)
                        {
                            btn_cofirma.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri);
                        }

                        if (imgbtn_firmaHSM.Enabled && imgbtn_firmaHSM.Visible)
                        {
                            imgbtn_firmaHSM.Visible = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                            imgbtn_firmaHSM.Enabled = UserManager.verifyRegNoAOO(schedaCorrente, userRegistri) && (this.schedaCorrente != null && !string.IsNullOrEmpty(this.schedaCorrente.systemId));
                        }
                    }
                }

                DocsPaWR.FileRequest fr = FileManager.getSelectedFile(this);

                fileDaVisualizzare = "docVisualizzaFrame.aspx?visInOrig=1&id=" + Session.SessionID;


                UserManager.removeBoolDocSalva(this);

                this.SetDocumentoVisualizzato(true);
            }
            else
                // ...altrimenti simulo il click sul pulsante visualizza
                this.btn_visualizza_Click(sender, e);
        }

        private void impostazioneTimestamp()
        {
            bool enableTimestamp = false;
            Boolean.TryParse(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_TIMESTAMP_DOC), out enableTimestamp);
            if (enableTimestamp)
            {
                // Reperimento della scheda documento correntemente selezionata nel contesto del checkout
                DocsPaWR.SchedaDocumento documentoSelezionato = CheckInOut.CheckInOutServices.CurrentSchedaDocumento;
                if (documentoSelezionato != null && !string.IsNullOrEmpty(documentoSelezionato.systemId))
                {
                    btn_timestamp.Visible = true;
                    btn_timestamp.ImageUrl = "../App_Themes/ImgComuni/timestamp.jpg";
                    DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
                    DocsPaWR.FileRequest fileRequest = FileManager.getSelectedFile(this);
                    ArrayList timestampsDoc = DocumentManager.getTimestampsDoc(infoUtente, fileRequest);

                    if (timestampsDoc != null && timestampsDoc.Count > 0)
                    {
                        DocsPaWR.TimestampDoc timestampDoc = (DocsPaWR.TimestampDoc) timestampsDoc[0];
                        //Timestamp valido
                        if(Convert.ToDateTime(timestampDoc.DTA_SCADENZA) > System.DateTime.Now)
                            btn_timestamp.ImageUrl = "../App_Themes/ImgComuni/timestamp_valido.jpg";                            
                        //Timestamp scaduto
                        if (Convert.ToDateTime(timestampDoc.DTA_SCADENZA) < System.DateTime.Now)
                            btn_timestamp.ImageUrl = "../App_Themes/ImgComuni/timestamp_scaduto.jpg";     
                    }
                }
            }
        }

        //invocata in risposta al click sul pulsante zoom
        public void RaiseCallbackEvent(String eventArgument)
        {
            try
            {
                string pageUrl = "docVisualizzaFrame.aspx?id=" + Session.SessionID;
                if (Session["personalize"] != null)
                {
                    valore = Session["personalize"].ToString();
                }
                if (Session["tipoLabel"] != null)
                {
                    valTipo = Session["tipoLabel"].ToString();
                }
                if (Session["rotazione"] != null)
                {
                    valRotaz = Session["rotazione"].ToString();
                }
                if (Session["carattere"] != null)
                {
                    valCarat = Session["carattere"].ToString();
                }
                if (Session["colore"] != null)
                {
                    valColor = Session["colore"].ToString();
                }
                if (Session["orientamento"] != null)
                {
                    valOrienta = Session["orientamento"].ToString();
                }
                if (
                    schedaCorrente != null && schedaCorrente.protocollo != null
                    && schedaCorrente.protocollo.segnatura != null
                    && schedaCorrente.protocollo.segnatura != ""
                    && (Session["VisSegn"] != null)
                    )
                {
                    pageUrl = "docVisualizzaFrame.aspx?id=" + Session.SessionID + "&plusEtic=1&pos=" + valore + "&tipo=" + valTipo + "&rotazione=" + valRotaz + "&carattere=" + valCarat + "&colore=" + valColor + "&orientamento=" + valOrienta;
                    //Session.Remove("VisSegn");
                }
                returnValue = pageUrl;
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        public String GetCallbackResult()
        {
            return returnValue;
        }

        #region Stampa Segnatura Firma Automatica External Service
        /// <summary>
        /// Metodo per la Stampa Segnatura Firma Automatica External Service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_stampa_segn_automatica_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Stampa Segnatura Firma Automatica External Service
            this.RemotePdfStamp();
            //this.RefreshLeftPanelIfAllegato();

            //Refresh del documento
            //this.RefreshLeftPanelIfLiveCycle();
            this.RefreshDocument();
        }

        /// <summary>
        /// Metodo 2 per la Stampa Segnatura Firma Automatica External Service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_stampa_segn_automatica_Click_ModalDialog(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DocsPaWR.FileRequest frDoc = FileManager.getSelectedFile(this);
            DocsPaWR.InfoUtente infoUt = UserManager.getInfoUtente();
            DocsPaWR.SchedaDocumento sd = DocumentManager.getDocumentoSelezionato(this);

            Session["frDoc_RemotePdfStamp"] = frDoc;
            Session["infoUt_RemotePdfStamp"] = infoUt;
            Session["sd_RemotePdfStamp"] = sd;

            string script = "<script>" +
                            "var retval;" +
                            "retval = window.showModalDialog('../popup/RemotePdfStamp.aspx',this,'dialogWidth:520px;dialogHeight:50px;fullscreen:no;toolbar:no;status:no;resizable:no;scroll:auto;" +
                //"retval = window.open('../popup/RemotePdfStamp.aspx','','Width:520px;Height:240px;fullscreen:no;toolbar:no;status:no;resizable:no;scroll:auto;" +
                            "center:yes;help:no;close:no');" +
                //"document.getElementById('hf_segn_firma_automatica').value = retval.value;" +
                            "</script>";
            ClientScript.RegisterStartupScript(this.GetType(), "remotePdfStamp", script);


        }

        /// <summary>
        /// Metodo RemotePdfStamp per Stampa Segnatura Firma Automatica External Service
        /// </summary>
        private void RemotePdfStamp()
        {
            bool retVal = false;
            // Recupero parametri da passare al metodo RemotePdfStamp
            DocsPaWR.FileRequest frDoc = FileManager.getSelectedFile(this);
            DocsPaWR.InfoUtente infoUt = UserManager.getInfoUtente();
            DocsPaWR.SchedaDocumento sd = DocumentManager.getDocumentoSelezionato(this);

            // Creazione dell'oggetto per l'invocazione del webService
            DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
            try
            {
                // invocazione metodo per la Stampa Segnatura Firma Automatica External Service
                retVal = docsPaWS.RemotePdfStamp(infoUt, frDoc, sd.protocollo.segnatura);
            }
            catch (Exception e)
            {
                retVal = false;
            }

        }

        #endregion

        #region Firma HSM
        /// <summary>
        /// Metodo associato al click della firma HSM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgbtn_firmaHSM_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        #endregion
    }
}
