using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class ReportSpedizioni : System.Web.UI.Page
    {
        private DocsPaWR.SchedaDocumento schedaDocumento;
        private string messError = "";

        protected Boolean closeAll = false;
        

        public string IdDocumento
        {
            get
            {
                if (Session["ReportSpedizioni_IdDocumento"] != null) return Session["ReportSpedizioni_IdDocumento"].ToString();
                return string.Empty;
            }
            set
            {
                Session["ReportSpedizioni_IdDocumento"] = value;
            }
        }

        public string urlImgEsito
        {
            get
            {
                if (Session["ReportSpedizioni_urlImgEsito"] != null) return Session["ReportSpedizioni_urlImgEsito"].ToString();
                return string.Empty;
            }
            set
            {
                Session["ReportSpedizioni_urlImgEsito"] = value;
            }
        }

        public bool IsMonitoring
        {
            get
            {
                return Request.QueryString.Count > 0 && (Request.QueryString["caller"] != null && Request.QueryString["caller"].Equals("monitoring"));
            } 
        }

        private List<string> IdDocumentiSelezionatiMonitoring
        {
            get
            {
                return (List<string>)HttpContext.Current.Session["IdDocumentiSelezionatiMonitoring"];
            }
        }
        #region Events Methods

        protected void ddlTipoFiltro_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectFiltroData(this.ddlTipoFiltro.SelectedValue);
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            int MaxCharLenghtOggettoDoc = 128; //max numero di caratteri da visualizzare per la descrizione oggetto doc
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //bind oggetto documento 
                Label lblOggettoDoc = e.Row.FindControl("lblOggettoDocumento") as Label;
                if (lblOggettoDoc.Text.Length > 128)
                {
                    lblOggettoDoc.CssClass = "repSpedLongTxt";
                    lblOggettoDoc.ToolTip = lblOggettoDoc.Text;
                    lblOggettoDoc.Text = lblOggettoDoc.Text.Substring(0, MaxCharLenghtOggettoDoc) + "...";
                }
                ImageButton cib = e.Row.FindControl("IndexImgDetailsDocument") as ImageButton;
                if (!string.IsNullOrEmpty(IdDocumento))
                    cib.Visible = false;
                //bind icon alert 
                Image imgAlert = e.Row.FindControl("imgAlert") as Image;
                if (string.IsNullOrEmpty(urlImgEsito))
                {
                    switch ((e.Row.DataItem as DocsPaWR.InfoDocumentoSpedito).InfoSpedizione)
                    {
                        case DocsPaWR.TipoInfoSpedizione.Effettuato:
                            imgAlert.ImageUrl = @"~/Images/Common/messager_check.png";
                            break;
                        case DocsPaWR.TipoInfoSpedizione.Errore:
                            imgAlert.ImageUrl = @"~/Images/Common/messager_error.png";
                            break;
                        case DocsPaWR.TipoInfoSpedizione.Warning:
                            imgAlert.ImageUrl = @"~/Images/Common/messager_warning.png";
                            break;
                    }
                }
                else
                {
                    imgAlert.ImageUrl = urlImgEsito;
                }

                if (!string.IsNullOrEmpty(IdDocumento))
                {
                    urlImgEsito = imgAlert.ImageUrl;
                }

                // bind griglia spedizioni
                GridView gvSpedizioni = e.Row.FindControl("gvSpedizioni") as GridView;
                gvSpedizioni.DataSource = (e.Row.DataItem as DocsPaWR.InfoDocumentoSpedito).Spedizioni;
                gvSpedizioni.DataBind();
            }
        }

        protected void gvSpedizioni_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //bind icon tipo ricevuta - Accettazione
                Image imgAccettazione = e.Row.FindControl("imgAccettazione") as Image;
                imgAccettazione.ImageUrl = getImageUrlTipoRicevuta((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Accettazione);
                imgAccettazione.ToolTip = getToolTipsTipoRicevuta((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Accettazione);
                
                //bind icon tipo ricevuta - Consegna
                Image imgConsegna = e.Row.FindControl("imgConsegna") as Image;
                imgConsegna.ImageUrl = getImageUrlTipoRicevuta((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Consegna);
                imgConsegna.ToolTip = getToolTipsTipoRicevuta((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Consegna);
                if ((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Consegna == DocsPaWR.TipologiaStatoRicevuta.KO)
                {
                    imgConsegna.ToolTip = (!string.IsNullOrEmpty((e.Row.DataItem as DocsPaWR.InfoSpedizione).MotivoAnnullEccezione)?(e.Row.DataItem as DocsPaWR.InfoSpedizione).MotivoAnnullEccezione:"Mancata Consegna") ;
                }

                // Modifica Lembo 11-09-2013: Tooltip nuovi...
                if ((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Consegna == DocsPaWR.TipologiaStatoRicevuta.AttendereCausaMezzo &&
                    (e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Accettazione == DocsPaWR.TipologiaStatoRicevuta.Attendere &&
                    (e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Conferma == DocsPaWR.TipologiaStatoRicevuta.AttendereCausaMezzo)
                {
                    imgConsegna.ToolTip = "In attesa della ricevuta di accettazione.";
                }

                //bind icon tipo ricevuta - Conferma
                Image imgConferma = e.Row.FindControl("imgConferma") as Image;
                imgConferma.ImageUrl = getImageUrlTipoRicevuta((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Conferma);
                imgConferma.ToolTip = getToolTipsTipoRicevuta((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Conferma);
                if ((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Conferma == DocsPaWR.TipologiaStatoRicevuta.OK)
                {
                    if (!string.IsNullOrEmpty((e.Row.DataItem as DocsPaWR.InfoSpedizione).ProtocolloDestinatario) && (e.Row.DataItem as DocsPaWR.InfoSpedizione).DataProtDest != null)
                    {
                        imgConferma.ToolTip = (e.Row.DataItem as DocsPaWR.InfoSpedizione).ProtocolloDestinatario + ", protocollato il " + (e.Row.DataItem as DocsPaWR.InfoSpedizione).DataProtDest.Substring(0, 11);
                    }
                    else
                    {
                        imgConferma.ImageUrl = getImageUrlTipoRicevuta(DocsPaWR.TipologiaStatoRicevuta.Attendere);
                        imgConferma.ToolTip = getToolTipsTipoRicevuta(DocsPaWR.TipologiaStatoRicevuta.Attendere);

                
                    }
                }
                //bind icon tipo ricevuta - Annullamento
                Image imgAnnullamento = e.Row.FindControl("imgAnnullamento") as Image;
                imgAnnullamento.ImageUrl = getImageUrlTipoRicevuta((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Annullamento);
                imgAnnullamento.ToolTip = getToolTipsTipoRicevuta((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Annullamento);
                if ((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Annullamento == DocsPaWR.TipologiaStatoRicevuta.OK)
                {
                    imgAnnullamento.ToolTip = (e.Row.DataItem as DocsPaWR.InfoSpedizione).MotivoAnnullEccezione;
                }
                if ((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Annullamento == DocsPaWR.TipologiaStatoRicevuta.Attendere)
                {
                    imgAnnullamento.ToolTip = "Possibile ricevuta di annullamento";
                }
                //bind icon tipo ricevuta - Eccezione
                Image imgEccezione = e.Row.FindControl("imgEccezione") as Image;
                imgEccezione.ImageUrl = getImageUrlTipoRicevuta((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Eccezione);
                imgEccezione.ToolTip = getToolTipsTipoRicevuta((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Eccezione);
                if ((e.Row.DataItem as DocsPaWR.InfoSpedizione).TipoRicevuta_Eccezione == DocsPaWR.TipologiaStatoRicevuta.KO)
                {
                    imgEccezione.ToolTip = (e.Row.DataItem as DocsPaWR.InfoSpedizione).MotivoAnnullEccezione;
                }
                Label lblazioneinfo = e.Row.FindControl("labelAzioneInfoSped") as Label;
                lblazioneinfo.Text = ((e.Row.DataItem as DocsPaWR.InfoSpedizione).Azione_Info == DocsPaWR.TipologiaAzione.Rispedire ? "Verificare e Rispedire" : (e.Row.DataItem as DocsPaWR.InfoSpedizione).Azione_Info.ToString()); 
            }
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    urlImgEsito = "";
                    InitializeLanguage();
                    if (!this.IsMonitoring)
                    {
                        FetchRegistri();
                        SetDefaultDataFilters();
                        if (!string.IsNullOrEmpty(IdDocumento))
                        {
                            ReadListaReport();
                        }
                    }
                    else
                    {
                        SetFiltriMonitoraggio();
                        ReadListaReportMonitoring();
                    }
                }
                RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void SetFiltriMonitoraggio()
        {
            this.rl_visibilita.SelectedIndex = 1;
            ckbEsitoOK.Checked = true;
            ckbEsitoAttesa.Checked = true;
            ckbEsitoKO.Checked = true;
            this.tblEspandiChiudi.Visible = false;
            this.pnlAltriFiltri.Visible = false;
            this.pnlRegistroCasella.Visible = false;
        }

        private void InitializeLanguage()
        {
            Utils.Languages.InitializesLanguages();
            string currentLanguage = (string.IsNullOrEmpty(UIManager.UserManager.GetUserLanguage()) ? "Italian" : UIManager.UserManager.GetUserLanguage());
            this.SenderLblRegistriRF.Text = UIManager.LoginManager.GetLabelFromCode("SenderLblRegistriRF", currentLanguage);
            this.SenderLblCaselle.Text = UIManager.LoginManager.GetLabelFromCode("SenderLblCaselle", currentLanguage);
            

        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: 'Nessun risultato trovato' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: 'Nessun risultato trovato' });", true);
        
            
        }

        protected void cboRegistriRF_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddl_caselle.Items.Clear();
                if (!string.IsNullOrEmpty(this.cboRegistriRF.SelectedValue))
                {
                    //aggiorno le caselle associate al registro/rf appena selezionato
                    SetCaselleRegistro("1");
                    ddl_caselle.Enabled = true;
                    
                }
                else
                {
                    ddl_caselle.Enabled = false;
                    
                }
                // Impostazione abilitazione controlli
                
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Aggiorna la drop down list delle caselle abilitate alla spedizione associate al registro selezionato
        /// </summary>
        private void SetCaselleRegistro(string selectMain)
        {
            //se è stato selezionato un registro/rf nella ddl dei registri mittente, allora setto la ddl delle caselle associate al registro
            if (!string.IsNullOrEmpty(cboRegistriRF.SelectedValue))
            {
                List<DocsPaWR.CasellaRegistro> listCaselle = new List<DocsPaWR.CasellaRegistro>();
                listCaselle = MultiBoxManager.GetComboRegisterSend(cboRegistriRF.SelectedValue);
                foreach (DocsPaWR.CasellaRegistro c in listCaselle)
                {
                    System.Text.StringBuilder formatMail = new System.Text.StringBuilder();
                    if (c.Principale.Equals("1"))
                        formatMail.Append("* ");
                    formatMail.Append(c.EmailRegistro);
                    if (!string.IsNullOrEmpty(c.Note))
                    {
                        formatMail.Append(" - ");
                        formatMail.Append(c.Note);
                    }
                    ddl_caselle.Items.Add(new ListItem(formatMail.ToString(), c.EmailRegistro));
                }

                if (listCaselle.Count == 0)
                {
                    ddl_caselle.Enabled = false;
                    ddl_caselle.Width = new Unit(200);
                    return;
                }
                //se ho appena settato un nuovo registro/rf allora seleziono la casella principale
                if (selectMain.Equals("1"))
                {
                    //imposto la casella principale come selezionata
                    foreach (ListItem i in ddl_caselle.Items)
                    {
                        if (i.Text.Split(new string[] { "*" }, 2, System.StringSplitOptions.None).Length > 1)
                        {
                            ddl_caselle.SelectedValue = i.Value;
                            break;
                        }
                    }
                }

            }
        }

        protected void FetchRegistri()
        {
            // Inserimento elemento vuoto
            if (RegistryManager.GetRegistryInSession() == null)
            {
                RegistryManager.SetRegistryInSession(RegistryManager.GetRegistriesByRole(UserManager.GetSelectedRole().systemId).FirstOrDefault());
            }

            this.cboRegistriRF.Items.Add(new ListItem(string.Empty, string.Empty));
            DocsPaWR.Registro[] registriRF = MultiBoxManager.GetRegisterEnabledSend().ToArray();
            this.cboRegistriRF.Items.AddRange(
                (from reg in registriRF
                 select new ListItem(
                 string.Format("{0} - {1}", reg.codRegistro, reg.descrizione), reg.systemId)).ToArray());
            int countRF = registriRF.Count(e => e.chaRF == "1");
            this.cboRegistriRF.SelectedValue = string.Empty;
            if (!string.IsNullOrEmpty(this.cboRegistriRF.SelectedValue))
            {
                SetCaselleRegistro("1");
                ddl_caselle.Enabled = true;
            }
        }


        protected void btnApplicaFiltri_OnClick(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            if (IsMonitoring)
                ReadListaReportMonitoring();
            else
                ReadListaReport();
           
        }

        protected void btnChiudi_Click(object sender, EventArgs e)
        {
            try
            {
                 ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
                 if (!string.IsNullOrEmpty(IdDocumento))
                 {
                     Session["ReportSpedizioni_IdDocumento"] = null;

                     ScriptManager.RegisterClientScriptBlock(this.UpSend, this.UpSend.GetType(), "closeAJM", "parent.closeAjaxModal('SendingReportDocument','');", true);
                 }
                 else if(IsMonitoring)
                {
                    ScriptManager.RegisterClientScriptBlock(this.UpSend, this.UpSend.GetType(), "closeAJM", "parent.closeAjaxModal('SendingReportMonitoring','');", true);
                }
                 else
                 {
                     ScriptManager.RegisterClientScriptBlock(this.UpSend, this.UpSend.GetType(), "closeAJM", "parent.closeAjaxModal('SendingReport','');", true);
                 
                 }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {            
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            ExportReportSpedizioni();
            UpSend.Update();
        }

        #endregion


        #region Private Methods

        private void SelectFiltroData(string SelectedTipoData)
        {
            switch (SelectedTipoData)
            {
                case "ValoreSingolo":
                    lblDataDa.Visible = false;
                    txtDataDa.Visible = true;
                    lblDataA.Visible = false;
                    txtDataA.Visible = false;
                    txtDataDa.Text = string.Empty;
                    break;
                case "Intervallo":
                    lblDataDa.Visible = true;
                    txtDataDa.Visible = true;
                    lblDataA.Visible = true;
                    txtDataA.Visible = true;
                    txtDataDa.Text = string.Empty;
                    txtDataA.Text = string.Empty;
                    break;
                case "Oggi":
                    lblDataDa.Visible = false;
                    txtDataDa.Visible = true;
                    lblDataA.Visible = false;
                    txtDataA.Visible = false;
                    txtDataDa.Text = DateTime.Today.ToString("dd/MM/yyyy");
                    break;

                case "SettimanaCorrente":
                    lblDataDa.Visible = true;
                    txtDataDa.Visible = true;
                    lblDataA.Visible = true;
                    txtDataA.Visible = true;
                    txtDataDa.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                    txtDataA.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                    break;

                case "MeseCorrente":
                    lblDataDa.Visible = true;
                    txtDataDa.Visible = true;
                    lblDataA.Visible = true;
                    txtDataA.Visible = true;
                    txtDataDa.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                    txtDataA.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                    break;
            }
        }

        private string getImageUrlTipoRicevuta(DocsPaWR.TipologiaStatoRicevuta statoRicevuta)
        {
            switch (statoRicevuta)
            {
                case DocsPaWR.TipologiaStatoRicevuta.KO:
                    return @"~/Images/Common/messager_error.png";
                case DocsPaWR.TipologiaStatoRicevuta.OK:
                    return @"~/Images/Common/messager_check.png";
                case DocsPaWR.TipologiaStatoRicevuta.Attendere:
                    return @"~/Images/Common/messager_hourglass.png";
                case DocsPaWR.TipologiaStatoRicevuta.AttendereCausaMezzo:
                    return @"~/Images/Common/messager_cancel_gray.png";
                default:
                    return @"~/Images/Common/messager_cancel_gray.png";
            }
        }

        private string getToolTipsTipoRicevuta(DocsPaWR.TipologiaStatoRicevuta statoRicevuta)
        {
            switch (statoRicevuta)
            {
                case DocsPaWR.TipologiaStatoRicevuta.KO:
                    return "Ricevuta KO";
                case DocsPaWR.TipologiaStatoRicevuta.OK:
                    return "Ricevuta OK";
                case DocsPaWR.TipologiaStatoRicevuta.Attendere:
                    return "In attesa di ricevuta";
                case DocsPaWR.TipologiaStatoRicevuta.AttendereCausaMezzo:
                    return "Ricevuta non prevista";
                default:
                    return string.Empty;
            }
        }

        private void ReadListaReportMonitoring()
        {
            DocsPaWR.FiltriReportSpedizioni filters = new FiltriReportSpedizioni();
            filters = getFiltersFromGui();
            try
            {
                List<DocsPaWR.InfoDocumentoSpedito> listSpedizioni = SenderManager.GetReportSpedizioniDocumenti(filters, this.IdDocumentiSelezionatiMonitoring);
                if (listSpedizioni.Count > 0 && string.IsNullOrEmpty(IdDocumento))
                {
                    this.tblEspandiChiudi.Visible = true;
                    this.lblNumDocTrovati.Text = listSpedizioni.Count.ToString();
                }
                else
                {
                    this.tblEspandiChiudi.Visible = false;
                }

                gvlistaDocumenti.DataSource = listSpedizioni;
                gvlistaDocumenti.DataBind();
                UpPnlDest.Update();
                gvlistaDocumenti.Visible = true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        

        private void ReadListaReport()
        {
            DocsPaWR.DocsPaWebService docsPaSvr = new DocsPaWR.DocsPaWebService();
            //recupero filtri da UI
            DocsPaWR.FiltriReportSpedizioni filters = new FiltriReportSpedizioni();
                
                filters = getFiltersFromGui();

            // filtro per singolo documento
            if (!string.IsNullOrEmpty(IdDocumento))
            {
                filters.IdDocumento = IdDocumento;
                // nasconde altri filtri
                pnlAltriFiltri.Visible = false;
                // nasconde pannello esito complessivo spedizione
                filtriEsito.Visible = false;

                pnlRegistroCasella.Visible = false;
            }
            else
            {
                // visualizza filtro altri filtri
                pnlAltriFiltri.Visible = true;
                // visualizza pannello esito complessivo spedizione
                filtriEsito.Visible = true;

                pnlRegistroCasella.Visible = true;
            }
            try
            {
                List<DocsPaWR.InfoDocumentoSpedito> listSpedizioni = docsPaSvr.GetReportSpedizioni(filters,UserManager.GetInfoUser()).ToList();
                if (listSpedizioni.Count > 0 && string.IsNullOrEmpty(IdDocumento))
                {
                    this.tblEspandiChiudi.Visible = true;
                    this.lblNumDocTrovati.Text = listSpedizioni.Count.ToString();
                }
                else
                {
                    this.tblEspandiChiudi.Visible = false;
                }

                gvlistaDocumenti.DataSource = listSpedizioni;
                gvlistaDocumenti.DataBind();
                UpPnlDest.Update();
                gvlistaDocumenti.Visible = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void SetDefaultDataFilters()
        {
            ddlTipoFiltro.SelectedIndex = 4;
            SelectFiltroData(ddlTipoFiltro.SelectedValue);
            tblEspandiChiudi.Visible = false;
            if (string.IsNullOrWhiteSpace(IdDocumento))
            {
                rl_visibilita.SelectedIndex = 0;
                ckbEsitoOK.Checked = true;
                ckbEsitoAttesa.Checked = true;
                ckbEsitoKO.Checked = true;
                //cboRegistriRF.Enabled = false;
            }
        }


        /// <summary>
        /// Recupera i filtri dalla Gui
        /// </summary>
        /// <returns></returns>
        private DocsPaWR.FiltriReportSpedizioni getFiltersFromGui()
        {
            DocsPaWR.FiltriReportSpedizioni filters = new DocsPaWR.FiltriReportSpedizioni();
            // Tipo Ricevuta
            filters.TipoRicevuta_Accettazione = ckbTipoRicevuta.Items[0].Selected; // Accettazione
            filters.TipoRicevuta_AvvenutaConsegna = ckbTipoRicevuta.Items[1].Selected; // Avvenuta Consegna
            filters.TipoRicevuta_MancataAccettazione = ckbTipoRicevuta.Items[2].Selected; // Mancata accettazione
            filters.TipoRicevuta_MancataConsegna = ckbTipoRicevuta.Items[3].Selected; // Mancata Consegna
            filters.TipoRicevuta_ConfermaRicezione = ckbTipoRicevuta.Items[4].Selected; // Conferma Ricezione
            filters.TipoRicevuta_AnnullamentoProtocollazione = ckbTipoRicevuta.Items[5].Selected; // Annulla Protocollazione
            filters.TipoRicevuta_Eccezione = ckbTipoRicevuta.Items[6].Selected; // Eccezioni
            filters.TipoRicevuta_ConErrori = ckbTipoRicevuta.Items[7].Selected; // Con Errori
            filters.TipoRicevuta_EsitoOK = ckbEsitoOK.Checked;
            filters.TipoRicevuta_EsitoAttesa = ckbEsitoAttesa.Checked;
            filters.TipoRicevuta_EsitoKO = ckbEsitoKO.Checked;
            if (!IsMonitoring)
            {
                // Tipo Filtro Data
                switch (ddlTipoFiltro.SelectedValue)
                {
                    case "Intervallo":
                        filters.FiltroData = DocsPaWR.TipoFiltroData.Intervallo;
                        break;
                    case "MeseCorrente":
                        filters.FiltroData = DocsPaWR.TipoFiltroData.MeseCorrente;
                        break;
                    case "Oggi":
                        filters.FiltroData = DocsPaWR.TipoFiltroData.Oggi;
                        break;
                    case "SettimanaCorrente":
                        filters.FiltroData = DocsPaWR.TipoFiltroData.SettimanaCorrente;
                        break;
                    case "ValoreSingolo":
                        filters.FiltroData = DocsPaWR.TipoFiltroData.ValoreSingolo;
                        break;
                }

                //Data Da
                if (!string.IsNullOrEmpty(txtDataA.Text))
                    filters.DataA = Convert.ToDateTime(txtDataA.Text);
                //Data A
                if (!string.IsNullOrEmpty(txtDataDa.Text))
                    filters.DataDa = Convert.ToDateTime(txtDataDa.Text);
            }
            else
            {
                filters.FiltroData = DocsPaWR.TipoFiltroData.Oggi;
            }
            //Visibilita
            if (rl_visibilita.Items[0].Selected)
                // AllDocByRuolo
                filters.VisibilitaDoc = DocsPaWR.TipoVisibilitaDocumenti.AllDocByRuolo;
            else
                //Visibilita AllDoc
                filters.VisibilitaDoc = DocsPaWR.TipoVisibilitaDocumenti.AllDoc;
            if(cboRegistriRF.SelectedItem!=null && !string.IsNullOrEmpty(cboRegistriRF.SelectedValue))
            filters.IdRegMailMittente = cboRegistriRF.SelectedValue;
            if (ddl_caselle.SelectedItem != null && !string.IsNullOrEmpty(ddl_caselle.SelectedValue))
                filters.MailMittente = ddl_caselle.SelectedValue;
            return filters;
        }

        #endregion


        #region Export Report Spedizioni

        /// <summary>
        /// Metodo di configurazione ed entry-point per la reportistica delle spedizioni
        /// </summary>
        private void ExportReportSpedizioni()
        {
            try
            {
                // Recupero delle informazioni sull'utente / amministratore loggato
                InfoUtente userInfo = UserManager.GetInfoUser();

                // Salvataggio della request nel call context
                PrintReportRequestDataset request =
                    new PrintReportRequestDataset()
                    {
                        ContextName = "ReportSpedizioni",
                        SearchFilters = GetGenericFilters(getFiltersFromGui()),
                        UserInfo = userInfo,
                        Title = "Report Spedizioni",
                    };

                Session["ActiveExportFormatPDF"] = false;
                Session["requestPrintReport"] = request;
                Session["readOnlySubtitle"] = false;
                Session["visibleGrdFields"] = false;
                ReportingUtils.PrintRequest = request;

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ReportGenerator", "ajaxModalPopupReportGenerator();", true);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Mapping dei filtri da passare al mototore di reporting
        /// </summary>
        /// <param name="filters">oggetto contenente i filtri presenti nella gui</param>
        /// <returns></returns>
        private FiltroRicerca[] GetGenericFilters(DocsPaWR.FiltriReportSpedizioni filters)
        {
            if (!string.IsNullOrEmpty(IdDocumento))
            {
                filters.IdDocumento = IdDocumento;
            }
            FiltroRicerca[] gloabalFilters = new FiltroRicerca[1];
            FiltroRicerca globalFilter = new FiltroRicerca();
            globalFilter.listaFiltriSpedizioni = filters;
            gloabalFilters[0]  = globalFilter;
            return gloabalFilters;
        }

        #endregion
        

        protected void ckbTipoRicevutaTutti_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox check = (sender as CheckBox);
                if (check.Checked)
                {
                    foreach (ListItem item in  ckbTipoRicevuta.Items)
                        item.Selected = true;
                }
                else
                {
                    foreach (ListItem item in ckbTipoRicevuta.Items)
                        item.Selected = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void gvlistaDocumenti_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "ViewDetailsDocument")
                {


                    string idDocument = e.CommandArgument.ToString();
                    SchedaDocumento schedaDoc = UIManager.DocumentManager.getDocumentDetails(this.Page, idDocument, idDocument);
                    DocumentManager.setSelectedRecord(schedaDoc);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('SendingReport', 'redirect');", true);
        
                    //Response.Redirect("../Document/Document.aspx",false);
                    //Response.End();
                    return;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void rl_visibilita_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (rl_visibilita.Items[0].Selected)
                {
                    cboRegistriRF.Enabled = true;

                }
                else
                {
                    cboRegistriRF.SelectedValue = string.Empty;
                    this.cboRegistriRF_SelectedIndexChanged(sender, e);
                    cboRegistriRF.Enabled = false;

                }
                this.upPnlRegistroCasella.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }



    }
}