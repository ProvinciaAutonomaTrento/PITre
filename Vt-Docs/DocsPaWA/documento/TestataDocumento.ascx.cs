namespace DocsPAWA.documento
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for TestataDocumento.
	/// </summary>
	public class TestataDocumento : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label lblTitleSegnatura;
		protected System.Web.UI.WebControls.TextBox txtOggetto;
		protected System.Web.UI.WebControls.Label lblOggetto;
		protected System.Web.UI.WebControls.Label lblTitleNumProtocollo;
		protected System.Web.UI.WebControls.Label lblNumProtocollo;
		protected System.Web.UI.WebControls.Label lblTitleIdDocumento;
		protected System.Web.UI.WebControls.Label lblIdDocumento;
		protected System.Web.UI.WebControls.Label lblTitleData;
		protected System.Web.UI.WebControls.Label lblData;
        protected System.Web.UI.WebControls.Label lblAnnullamento;
        protected System.Web.UI.WebControls.Label lblPrivato;
		protected System.Web.UI.WebControls.Label lblDataAnnullamento;
		protected System.Web.UI.WebControls.TextBox txtSegnatura;
        protected System.Web.UI.WebControls.Label lblVisibilita;
        protected DocsPaWebCtrlLibrary.ImageButton btnVisibilita;
        protected DocsPaWebCtrlLibrary.ImageButton btn_log;
        protected System.Web.UI.WebControls.CheckBox chkPrivato;
        protected System.Web.UI.WebControls.Panel pnl_protoTitolario;
        protected System.Web.UI.WebControls.Label lbl_titleProtoTitolario;
        protected System.Web.UI.WebControls.Label lbl_protoTitolario;
        protected System.Web.UI.WebControls.Label lblStatoConsolidamento;
        protected System.Web.UI.HtmlControls.HtmlTableRow trStatoConsolidamento;

        private DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();

		private void Page_Load(object sender, System.EventArgs e)
		{
			DocsPaWR.SchedaDocumento schedaDocumento=(DocsPAWA.DocsPaWR.SchedaDocumento) DocumentManager.getDocumentoSelezionato();

            if (schedaDocumento != null)
            {
                this.SetVisibleDataAnnullamentoProtocollo(schedaDocumento);

                this.SetVisibleStoriaDocumento(schedaDocumento);

                this.SetVisibleVisibilitaDocumento(schedaDocumento);

                if (this.btnVisibilita.Visible)
                    this.btnVisibilita.Attributes.Add("onClick", "ApriFinestraVisibilita();");

                this.BindData(schedaDocumento);
            }
		}

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato();

            //Protocollo Titolario
            string protoTit = wws.isEnableProtocolloTitolario();
            string protoTitolario = String.Empty;
            if (!String.IsNullOrEmpty(schedaDocumento.systemId))
                protoTitolario = wws.getProtocolloTitolario(DocumentManager.getDocumentoSelezionato(this.Page));

            if (!string.IsNullOrEmpty(protoTitolario))
            {
                pnl_protoTitolario.Visible = true;
                lbl_titleProtoTitolario.Text = protoTit + " : ";
                lbl_protoTitolario.Text = protoTitolario;
            }
            else
            {
                pnl_protoTitolario.Visible = false;
                lbl_protoTitolario.Text = "";
            }

            this.trStatoConsolidamento.Visible = (schedaDocumento != null && 
                                schedaDocumento.ConsolidationState != null && 
                                schedaDocumento.ConsolidationState.State > DocsPaWR.DocumentConsolidationStateEnum.None);
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(this.Page_PreRender);
            this.btn_log.Click += new System.Web.UI.ImageClickEventHandler(this.btn_log_Click);
        }
		#endregion


        private void btn_log_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string scriptString = "<SCRIPT>ApriFinestraLog('D');</SCRIPT>";
            this.Page.RegisterStartupScript("apriModalDialogLog", scriptString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
		private void BindData(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento)
		{
            //if (!string.IsNullOrEmpty(schedaDocumento.systemId))
            if (schedaDocumento.repositoryContext == null)
            {
                bool documentoGrigio = (schedaDocumento.protocollo == null);
                bool documentoPredisposto = false;

                if (schedaDocumento != null && schedaDocumento.protocollo != null && schedaDocumento.protocollo.daProtocollare == "1")
                {
                    documentoPredisposto = true;
                }
                
                if (documentoGrigio || documentoPredisposto)
                {
                    this.lblData.Text = Utils.dateTimeLength(schedaDocumento.dataCreazione);
                }
                else
                {
                    // Verifica lo stato del registro (solo per protocollo)
                    bool isStatoRegistroGiallo = this.IsStatoRegistroGiallo(schedaDocumento.registro);

                    this.lblNumProtocollo.Text = schedaDocumento.protocollo.numero;
                    this.txtSegnatura.Text = schedaDocumento.protocollo.segnatura;
                    this.txtSegnatura.ToolTip = this.txtSegnatura.Text;

                    if (schedaDocumento.oraCreazione != null && schedaDocumento.oraCreazione != "")
                    {
                        ////*** ORA DI PROTOCOLLAZIONE NELLA SEGNATURA ---
                        //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("it-IT");
                        //string[] formati ={ "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };
                        //DateTime dataProtocollo = DateTime.ParseExact(schedaDocumento.protocollo.dataProtocollazione, formati, ci.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                        ////***
                        DateTime dataProtocollo = Convert.ToDateTime(schedaDocumento.protocollo.dataProtocollazione);
                        this.lblData.Text = dataProtocollo.ToShortDateString();

                        // Se lo stato del registro è giallo,
                        // non deve essere visualizzata l'ora
                        if (!isStatoRegistroGiallo)
                            this.lblData.Text += " " + Utils.timeLength(schedaDocumento.oraCreazione);
                    }
                    else
                    {
                        DateTime dataProtocollo = Convert.ToDateTime(schedaDocumento.protocollo.dataProtocollazione);
                        this.lblData.Text = dataProtocollo.ToShortDateString();
                    }
                    if (this.lblAnnullamento.Visible && this.lblDataAnnullamento.Visible)
                        this.lblDataAnnullamento.Text = schedaDocumento.protocollo.protocolloAnnullato.dataAnnullamento;
                }

                //utente è autorizzato a rimuovere le acl per il documento 
                //verifica che ci siano ACL rimosse 
                if (schedaDocumento != null && schedaDocumento.systemId != null)
                {
                    int result = DocumentManager.verificaDeletedACL(schedaDocumento.systemId, UserManager.getInfoUtente());
                    if (result == 1)
                    {
                        btnVisibilita.BorderWidth = 1;
                        this.btnVisibilita.BorderColor = Color.Red;
                    }
                    else
                        btnVisibilita.BorderWidth = 0;
                }

                this.lblIdDocumento.Text = schedaDocumento.docNumber;
                this.txtOggetto.Text = schedaDocumento.oggetto.descrizione;


                //CESTINO
                if (schedaDocumento != null && schedaDocumento.inCestino != null &&
                    schedaDocumento.inCestino == "1")
                {
                    this.btnVisibilita.Enabled = false;
                }
                #region privato
                if (!UserManager.ruoloIsAutorized(this.Page, "DO_PROTO_PRIVATO"))
                {
                    this.chkPrivato.Visible = false;
                    this.lblPrivato.Visible = false;
                }
                else
                {
                    this.chkPrivato.Visible = true;
                    this.lblPrivato.Visible = true;
                }

                //Veronica: gestione check privato
                if (schedaDocumento != null && (schedaDocumento.privato != null || schedaDocumento.personale != null))
                {
                    this.chkPrivato.Enabled = false;

                    //in questo caso in cui privato=1 forzo la visibilità anche se 
                    //il ruolo non è autorizzato, altrimenti non è visibile 
                    //l'informazione che il documento è privato
                    if (schedaDocumento.privato != null && schedaDocumento.privato == "1")
                    {
                        this.chkPrivato.Checked = (schedaDocumento.privato == "1");
                        this.chkPrivato.Visible = true;
                        this.lblPrivato.Visible = true;
                        this.lblPrivato.Text = "Privato";
                    }
                    if (schedaDocumento.personale != null && schedaDocumento.personale == "1")
                    {
                        this.chkPrivato.Visible = true;
                        this.lblPrivato.Visible = true;
                        this.chkPrivato.Checked = (schedaDocumento.personale == "1");
                        if (System.Configuration.ConfigurationManager.AppSettings["Label_Personale"] != null)
                        {
                            this.lblPrivato.Text = System.Configuration.ConfigurationManager.AppSettings["Label_Personale"].ToString();
                        }
                        else
                            this.lblPrivato.Text = "Utente";

                    }

                }
                #endregion

                string consolidationDescription = string.Empty;

                if (schedaDocumento != null && schedaDocumento.ConsolidationState != null)
                {
                    if (schedaDocumento.ConsolidationState.State == DocsPaWR.DocumentConsolidationStateEnum.Step1)
                        consolidationDescription = "CONSOLIDATO CONTENUTO";
                    else if (schedaDocumento.ConsolidationState.State == DocsPaWR.DocumentConsolidationStateEnum.Step2)
                        consolidationDescription = "CONSOLIDATI CONTENUTO E METADATI";
                }

                this.lblStatoConsolidamento.Text = consolidationDescription;

                //Verifica atipicita documento
                Utils.verificaAtipicita(schedaDocumento, DocsPaWR.TipoOggettoAtipico.DOCUMENTO, ref btnVisibilita);

                // Verifica del vero creatore del documento
                Utils.CheckCreatorRole(schedaDocumento, ref this.btn_log);

            }
        }

		/// <summary>
		/// Impostazione visibilità campo data annullamento del protocollo
		/// </summary>
		/// <param name="schedaDocumento"></param>
		private void SetVisibleDataAnnullamentoProtocollo(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento)
		{
			bool isVisible=(schedaDocumento.repositoryContext == null &&
                            schedaDocumento.protocollo!=null && 
							schedaDocumento.protocollo.protocolloAnnullato!=null);

			this.lblAnnullamento.Visible=isVisible;
			this.lblDataAnnullamento.Visible=isVisible;		
		}

        /// <summary>
        /// Impostazione visibilità campi per la visibilità del documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        private void SetVisibleVisibilitaDocumento(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento)
        {
            this.btnVisibilita.Visible = (schedaDocumento.repositoryContext == null);
            this.btnVisibilita.Visible = (this.btnVisibilita.Visible && schedaDocumento.documentoPrincipale == null);
            this.lblVisibilita.Visible = this.btnVisibilita.Visible;
        }

        /// <summary>
        /// Impostazione visibilità controlli relativi alla storia del documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        private void SetVisibleStoriaDocumento(DocsPaWR.SchedaDocumento schedaDocumento)
        {
            this.btn_log.Visible = (schedaDocumento.repositoryContext == null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
		private bool IsStatoRegistroGiallo(DocsPAWA.DocsPaWR.Registro registro)
		{
			return (DocsPAWA.UserManager.getStatoRegistro(registro)=="G");
		}
	}
}
