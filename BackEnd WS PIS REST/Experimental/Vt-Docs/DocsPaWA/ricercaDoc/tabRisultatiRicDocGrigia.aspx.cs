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
using System.Collections.Generic;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.ricercaDoc
{
	/// <summary>
	/// Summary description for tabRisultatiRicDocGrigia.
	/// </summary>
	public class tabRisultatiRicDocGrigia : DocsPAWA.CssPage
	{
        protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd1;
		protected System.Web.UI.WebControls.ImageButton insertAllADL;
		protected System.Web.UI.WebControls.Label titolo;		
		protected System.Web.UI.HtmlControls.HtmlTableRow trHeader;
		protected System.Web.UI.HtmlControls.HtmlTableRow tr1;
		protected System.Web.UI.HtmlControls.HtmlTableCell Td2;
        protected System.Web.UI.HtmlControls.HtmlTableRow trBody;
        protected System.Web.UI.WebControls.Label msgADL;
        protected System.Web.UI.WebControls.ImageButton btn_stampa;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{	
				Utils.startUp(this);
				
                DocumentManager.removeDocumentoSelezionato(this);

                if (!Page.IsPostBack)
                {
                    this.btn_stampa.Attributes.Add("onclick", "StampaRisultatoRicerca();");

                    this.AttatchGridPagingWaitControl();
                 
                    if (this.Filtri != null)
                    {
                        // Impostazione filtri nel contesto corrente
                        this.SetCurrentFiltersOnContext(this.Filtri);

                        // nuova ADL
                        if (this.RicercaAdl)
                        {
                            //setting delle prop. video
                            insertAllADL.ToolTip = "Elimina tutti i documenti trovati in 'Area di lavoro'";
                            insertAllADL.ImageUrl = "../images/proto/cancella.gif";
                        }

                        // Ripristino pagina selezionata in precedenza
                        this.RestoreSelectedPage();

                        // Caricamento dati
                        this.Fetch();

                        // Ripristino eventuale elemento selezionato in precedenza
                        this.RestoreSelectedDocument();
                    }
                    // gestione colonna visualizza unificata
                    if ((System.Configuration.ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] != null) && (System.Configuration.ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] == "0"))
                    {
                        this.DataGrid1.Columns[6].Visible = false;
                    }
                }
			}
			catch(System.Web.Services.Protocols.SoapException es) 
			{
				ErrorManager.redirect(this, es);
			} 
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
			this.insertAllADL.Click += new System.Web.UI.ImageClickEventHandler(this.insertAllADL_Click);
			this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
			this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_OnPageIndexChanged);
			this.DataGrid1.PreRender += new System.EventHandler(this.Datagrid1_PreRender);
			this.DataGrid1.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid1_ItemDataBound);
			this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
			this.hd1.ServerChange += new System.EventHandler(this.hd1_ServerChange);
			// 
			// dataSeRDocGrigia1
			// 
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

        #region Gestione dati

        /// <summary>
        /// Caricamento dati pagina corrente
        /// </summary>
        private void Fetch()
        {
            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);

            int currentPage = this.DataGrid1.CurrentPageIndex + 1;

            int pageCount;
            int recordCount;
            SearchResultInfo[] idProfile;

            DocsPaWR.InfoDocumento[] documenti = DocumentManager.getQueryInfoDocumentoPaging(infoUtente.idGruppo, 
                                                    infoUtente.idPeople, 
                                                    this,
                                                    this.Filtri,
                                                    currentPage,
                                                    out pageCount,
                                                    out recordCount, 
                                                    false, 
                                                    true,
                                                    true,
                                                    !IsPostBack,
                                                    out idProfile);

            DocumentManager.setListaDocNonProt(this, documenti);

            this.DataGrid1.VirtualItemCount = recordCount;
            this.DataGrid1.DataSource = documenti;
            this.DataGrid1.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshCountDocumenti()
        {
            string msg = "Elenco documenti";

            if (this.DataGrid1.Items.Count > 0)
                msg += " - Trovati " + this.DataGrid1.VirtualItemCount.ToString() + " elementi.";

            this.PrintMsg(msg);
        }

        /// <summary>
        /// Reperimento IdProfile corrente
        /// </summary>
        private string IdProfile
        {
            get
            {
                if (this.DataGrid1.SelectedItem != null)
                    return this.DataGrid1.SelectedItem.Cells[0].Text;
                else
                    return null;
            }
        }

        /// <summary>
        /// Reperimento DocNumber corrente
        /// </summary>
        private string DocNumber
        {
            get
            {
                if (this.DataGrid1.SelectedItem != null)
                    return this.DataGrid1.SelectedItem.Cells[1].Text;
                else
                    return null;
            }
        }

        /// <summary>
        /// Verifica se si è in ambiente di ricerca in ADL
        /// </summary>
        private bool RicercaAdl
        {
            get
            {
                return (!string.IsNullOrEmpty(Request.QueryString["ricADL"]));
            }
        }

        /// <summary>
        /// Reperimento filtri impostati
        /// </summary>
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] Filtri
        {
            get
            {
                return DocumentManager.getFiltroRicDoc(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DocsPaWR.InfoDocumento GetDocumentoSelezionato()
        {
            if (!string.IsNullOrEmpty(this.IdProfile) && 
                !string.IsNullOrEmpty(this.DocNumber))
            {
                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

                return ws.GetInfoDocumento(UserManager.getInfoUtente(), this.IdProfile, this.DocNumber);
            }
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        protected void PrintMsg(string msg)
        {
            this.titolo.Text = msg;
        }

        /// <summary>
        /// Impostazione visibilità controlli
        /// </summary>
        private void SetControlsVisibility()
        {
            this.trHeader.Visible = true;

            this.btn_stampa.Visible = (this.DataGrid1.Items.Count > 0);
            this.insertAllADL.Visible = this.btn_stampa.Visible;
            this.trBody.Visible = this.btn_stampa.Visible;

            if (this.RicercaAdl)
                this.msgADL.Visible = true;
        }

        #endregion

        #region Gestione datagrid

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DataGrid1_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
                if (e.Item.Cells.Count > 0)
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string errorMessage = string.Empty;

            DocsPaWR.InfoDocumento infoDocumento = this.GetDocumentoSelezionato();

            int retValue = DocumentManager.verificaACL("D", infoDocumento.idProfile, UserManager.getInfoUtente(), out errorMessage);

            if (retValue == 0 || retValue == 1)
            {
                string script = ("<script>alert('" + errorMessage + "');</script>");
                Response.Write(script);
            }
            else
            {
                DocumentManager.setRisultatoRicerca(this, infoDocumento);
                DocumentManager.removeListaNonDocProt(this);
                //rimuovo l'eventuale fascicolo selezionato per la ricerca, altrimenti
                //si vede nel campo fasc rapida di profilo/protocollo
                FascicoliManager.removeFascicoloSelezionatoFascRapida();
                // Impostazione documento selezionato
                this.SetSelectedDocument();

                Response.Write(string.Format("<script language='javascript'>top.principale.document.location = '../documento/gestionedoc.aspx?tab=profilo&allegato={0}';</script>", infoDocumento.allegato.ToString().ToLower()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void DataGrid1_OnPageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.DataGrid1.SelectedIndex = -1;

            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            this.Fetch();

            // Impostazione numero pagina nel contesto corrente
            this.SetSelectedPage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("Area"))
                {
                    this.DataGrid1.SelectedIndex = e.Item.ItemIndex;

                    DocsPaWR.InfoDocumento infoDocumento = this.GetDocumentoSelezionato();

                    string h = Request.Form["hd1"];

                    if (this.hd1.Value.Equals("Yes"))
                    {
                        DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDettaglioDocumento(this, this.IdProfile, this.DocNumber);

                        //se ho attiva la nuova ADL devo invertire la funzionalità
                        if (this.RicercaAdl)
                        {
                            DocumentManager.eliminaDaAreaLavoro(this, schedaDocumento.systemId, null);
                            //riavvio la ricerca
                            string fromPage = Request.QueryString["from"].ToString();
                            ClientScript.RegisterStartupScript(this.GetType(), "LanciaRic", "top.principale.document.location = 'gestioneRicDoc.aspx?tab=" + fromPage + "&ricADL=1&from=" + fromPage + "';", true);
                        }
                        else //normale comportamento
                        {
                            DocumentManager.addAreaLavoro(this, schedaDocumento);

                            //setto le proprietà video
                            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[7].Controls[1]).ToolTip = "Elimina questo documento da 'Area di lavoro'";
                            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[7].Controls[1]).ImageUrl = "../images/proto/cancella.gif";
                            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[7].Controls[1]).Attributes.Add("OnClick", "ApriModalDialogNewADL();");
                            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[7].Controls[1]).CommandName = "EliminaADL";
                            e.Item.Cells[8].Text = "1";

                            //if (e.Item.Cells[6].Controls[1].GetType().Equals(typeof(DocsPaWebCtrlLibrary.ImageButton)))
                            //{
                            //    if (Session["listInArea"] != null)
                            //    {
                            //        listInArea = (Hashtable)Session["listInArea"];

                            //        if (listInArea.ContainsKey(this.DocNumber) == false)
                            //            listInArea.Add(this.DocNumber, infoDocumento);

                            //        Session["listInArea"] = listInArea;
                            //    }
                            //    else
                            //    {
                            //        listInArea = new Hashtable();
                            //        listInArea.Add(string.Concat(infoDocumento.numProt, infoDocumento.dataApertura), infoDocumento);
                            //        Session["listInArea"] = listInArea;
                            //    }
                            //}
                        }
                    }
                }
                if (e.CommandName.Equals("EliminaADL"))
                {
                    this.DataGrid1.SelectedIndex = e.Item.ItemIndex;

                    DocsPaWR.InfoDocumento infoDocumento = this.GetDocumentoSelezionato();

                    string h = Request.Form["hd1"];

                    if (this.hd1.Value.Equals("Yes"))
                    {
                        DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDettaglioDocumento(this, infoDocumento.idProfile, infoDocumento.docNumber);

                        DocumentManager.eliminaDaAreaLavoro(this, schedaDocumento.systemId, null);

                        //se ho attiva la nuova ADL devo invertire la funzionalità
                        if (this.RicercaAdl)
                        {
                            string fromPage = Request.QueryString["from"].ToString();
                            ClientScript.RegisterStartupScript(this.GetType(), "LanciaRic", "top.principale.document.location = 'gestioneRicDoc.aspx?tab=" + fromPage + "&ricADL=1&from=" + fromPage + "';", true);

                        }
                        else
                        {
                            //setto le proprietà video
                            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[7].Controls[1]).ToolTip = "Inserisci questo documento da 'Area di lavoro'";
                            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[7].Controls[1]).ImageUrl = "../images/proto/ins_area.gif";
                            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[7].Controls[1]).Attributes.Add("OnClick", "ApriModalDialogNew();");
                            ((DocsPaWebCtrlLibrary.ImageButton)e.Item.Cells[7].Controls[1]).CommandName = "Area";
                        }
                        e.Item.Cells[8].Text = "0";

                        //if (e.Item.Cells[6].Controls[1].GetType().Equals(typeof(DocsPaWebCtrlLibrary.ImageButton)))
                        //{
                        //    if (Session["listInArea"] != null)
                        //    {
                        //        listInArea = (Hashtable)Session["listInArea"];

                        //        if (!listInArea.ContainsKey(infoDocumento.docNumber))
                        //            listInArea.Add(infoDocumento.docNumber, infoDocumento);

                        //        Session["listInArea"] = listInArea;
                        //    }
                        //    else
                        //    {
                        //        listInArea = new Hashtable();
                        //        listInArea.Add(string.Concat(infoDocumento.numProt, infoDocumento.dataApertura), infoDocumento);
                        //        Session["listInArea"] = listInArea;
                        //    }
                        //}
                    }
                }
                // visualizza doc in popup
                if (e.CommandName.Equals("Vis"))
                {
                      this.DataGrid1.SelectedIndex = e.Item.ItemIndex;
                        //vis unificata
                        DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDettaglioDocumento(this, this.IdProfile, this.DocNumber);
                        DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                        FileManager.setSelectedFile(this, schedaDocumento.documenti[0], false);
                        ClientScript.RegisterStartupScript(this.GetType(), "lanciaVIs", "loadvisualizzaDoc('" + Session.SessionID + "','" + schedaDocumento.docNumber + "','" + schedaDocumento.systemId + "');", true);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid1_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                e.Item.Cells[0].Font.Bold = true;
                e.Item.Cells[0].ForeColor = Color.Black;
            }         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Datagrid1_PreRender(object sender, System.EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    // Impostazione visibilità controlli
                    this.SetControlsVisibility();

                    this.RefreshCountDocumenti();
                }

                foreach (DataGridItem item in this.DataGrid1.Items)
                {
                    if (item.ItemIndex >= 0)
                    {
                        // Impostazione colore di sfondo per la colonna descrizione
                        this.SetForeColorColumnDescrizioneDocumento(item);

                        ((DocsPaWebCtrlLibrary.ImageButton) item.Cells[7].Controls[1]).Attributes.Add("OnClick", "ApriModalDialog();");

                        if (item.Cells[7].Text == "1")
                        {
                            //setto le proprietà video
                            ((DocsPaWebCtrlLibrary.ImageButton)item.Cells[7].Controls[1]).ToolTip = "Elimina questo documento da 'Area di lavoro'";
                            ((DocsPaWebCtrlLibrary.ImageButton)item.Cells[7].Controls[1]).ImageUrl = "../images/proto/cancella.gif";
                            ((DocsPaWebCtrlLibrary.ImageButton)item.Cells[7].Controls[1]).Attributes.Add("OnClick", "ApriModalDialogNewADL();");
                            ((DocsPaWebCtrlLibrary.ImageButton)item.Cells[7].Controls[1]).CommandName = "EliminaADL";
                        }
                        //imposto icona vis
                        // gestione colonna visualizza unificata
                        if ((System.Configuration.ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] != null) && (System.Configuration.ConfigurationManager.AppSettings["GRD_VIS_UNIFICATA"] == "1"))
                        {
                            if (item.Cells[9].Text == "0")
                            {
                                ImageButton imgbtn = new ImageButton();
                                if (item.Cells[6].Controls[1].GetType().Equals(typeof(ImageButton)))
                                {
                                    imgbtn = (ImageButton)item.Cells[6].Controls[1];
                                    imgbtn.Visible = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        protected string GetDescrizione(DocsPaWR.InfoDocumento infoDocumento)
        {
            return string.Format("{0}<BR />{1}", infoDocumento.docNumber, infoDocumento.dataApertura);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        protected string GetTipo(DocsPaWR.InfoDocumento infoDocumento)
        {
            DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = session.getUserAmmSession();
            String idAmm = null;
            if (UserManager.getInfoUtente() != null && !string.IsNullOrEmpty(UserManager.getInfoUtente().idAmministrazione))
                idAmm = UserManager.getInfoUtente().idAmministrazione;
            etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
            string grigio = etichette[3].Descrizione; //Valore G
            string allegato = etichette[4].Descrizione; //Valore ALL

            if (infoDocumento.allegato)
                // return "ALL";
                return allegato;
            else
                // return "NP";
                return grigio;
        }

        /// <summary>
        /// Reperimento path icona dettaglio
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        protected string GetPathIconaDettaglio(DocsPaWR.InfoDocumento infoDocumento)
        {
            if (!string.IsNullOrEmpty(infoDocumento.acquisitaImmagine) &&
                infoDocumento.acquisitaImmagine.Equals("1"))
                return "../images/proto/dett_lente_doc.gif";
            else return String.Empty;
        }

        /// <summary>
        /// Impostazione del colore del carattere per la prima colonna della griglia:
        /// rosso se doc protocollato, altrimenti grigio 
        /// </summary>
        /// <param name="item"></param>
        private void SetForeColorColumnDescrizioneDocumento(DataGridItem item)
        {
            Label lblDescrizione = item.FindControl("lblDescrizione") as Label;

            if (lblDescrizione != null)
            {
                lblDescrizione.ForeColor = Color.Black;
                lblDescrizione.Font.Bold = true;
            }
        }

        #endregion

		private void hd1_ServerChange(object sender, System.EventArgs e)
		{
		
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void insertAllADL_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			try
			{
			    //verifica per nuova ADL-- viene sostituita la funzionalità di inserisci con elimina
                if (this.RicercaAdl)
                {
                    foreach (DocsPaWR.InfoDocumento item in DocumentManager.getListaDocNonProt(this))
                        DocumentManager.eliminaDaAreaLavoro(this, item.idProfile, null);

                    if (!this.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "ADLInserted"))
                    {
                        string script = "<script>alert('Eliminazione dei documenti in ADL avvenuta correttamente.');</script>";
                        this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ADLInserted", script);
                    }
                    //riavvio la ricerca
                    string fromPage = Request.QueryString["from"].ToString();
                    
                    this.ClientScript.RegisterStartupScript(this.GetType(), "LanciaRic", "top.principale.document.location = 'gestioneRicDoc.aspx?tab=" + fromPage + "&ricADL=1&from=" + fromPage + "';", true);
                }
                else
                {
                    //normale gestione
                    foreach (DocsPaWR.InfoDocumento item in DocumentManager.getListaDocNonProt(this))
                        DocumentManager.addAreaLavoro(this, item);

                    if (!this.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "ADLInserted"))
                    {
                        string script = "<script>alert('Inserimento dei documenti in ADL avvenuto correttamente.');</script>";
                        this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ADLInserted", script);
                    }
                }

                this.Fetch();
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}
		}

		#region Gestione call context

		/// <summary>
		/// Impostazione filtri correnti nella sessione del contesto corrente di ricerca
		/// </summary>
		/// <param name="filters"></param>
		private void SetCurrentFiltersOnContext(DocsPAWA.DocsPaWR.FiltroRicerca[][] filters)
		{
			SiteNavigation.CallContext currentContext=SiteNavigation.CallContextStack.CurrentContext;

			if (currentContext != null)
			{
				// Impostazione, nell'oggetto CallContext corrente,
				// della lista dei filtri di ricerca
				currentContext.SessionState[SchedaRicerca.SESSION_KEY]=Session[SchedaRicerca.SESSION_KEY];
                currentContext.SessionState["ricDoc.listaFiltri"] = filters;
			}
		}

		/// <summary>
		/// Reperimento numero pagina corrente dal contesto di ricerca
		/// </summary>
		/// <returns></returns>
		private void RestoreSelectedPage()
		{
			SiteNavigation.CallContext currentContext=SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null && currentContext.PageNumber > 0)
            {
                this.DataGrid1.CurrentPageIndex = (currentContext.PageNumber - 1);

            }
		}

		/// <summary>
		/// Impostazione numero pagina corrente del contesto di ricerca
		/// </summary>
		private void SetSelectedPage()
		{
			SiteNavigation.CallContext currentContext=SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null)
            {
                currentContext.PageNumber = (this.DataGrid1.CurrentPageIndex + 1);
            }
		}

		/// <summary>
		/// Impostazione dell'indice del fascicolo
		/// selezionato nel contesto di ricerca
		/// </summary>
		private void SetSelectedDocument()
		{
            if (!string.IsNullOrEmpty(this.IdProfile))
            {
                SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

                currentContext.QueryStringParameters["docIndex"] = this.IdProfile;
            }
		}

		/// <summary>
		/// Ripristino del documento selezionato precedentemente
		/// </summary>
		private void RestoreSelectedDocument()
		{
            string docIndex = this.Request.QueryString["docIndex"];

            if (!string.IsNullOrEmpty(docIndex))
            {
                foreach (DataGridItem item in this.DataGrid1.Items)
                {
                    if (item.Cells[0].Text == docIndex)
                    {
                        this.DataGrid1.SelectedIndex = item.ItemIndex;
                        break;
                    }
                }
            }
		}

		#endregion

        #region Gestione paging wait control

        private void AttatchGridPagingWaitControl()
        {
            DataGridPagingWaitControl.DataGridID = this.DataGrid1.ClientID;
            DataGridPagingWaitControl.WaitScriptCallback = "WaitDataGridCallback(eventTarget,eventArgument);";
        }

        private waiting.DataGridPagingWait DataGridPagingWaitControl
        {
            get
            {
                return this.FindControl("DataGridPagingWait1") as waiting.DataGridPagingWait;
            }
        }

        #endregion
    }
}