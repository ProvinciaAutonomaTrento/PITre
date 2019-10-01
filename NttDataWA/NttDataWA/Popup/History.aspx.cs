using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using System.Data;

namespace NttDataWA.Popup
{
    public partial class History : System.Web.UI.Page
    {
        protected DocsPaWR.SchedaDocumento schedaDocumento;
        protected DocsPaWR.Fascicolo fascicolo;
        protected DocsPaWR.DocumentoStoricoOggetto[] ListaStoriaOggetto;
        protected DocsPaWR.DocumentoStoricoMittente[] ListaStoria;
        protected DocsPaWR.DocumentoStoricoDataArrivo[] ListaStoriaData;
        protected DocsPaWR.StoricoProfilati[] ListaTipologia;

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializeLanguage();
                    this.InitializePage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.HistoryBtnChiudi.Text = Utils.Languages.GetLabelFromCode("HistoryBtnChiudi", language);
        }

        protected void InitializePage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            if (this.TypeHistory != null && !string.IsNullOrEmpty(this.TypeHistory))
            {
                switch (this.TypeHistory.ToLower())
                {
                    case "documentimgobjecthistory":
                        this.HistroyLblCaption.Text = Utils.Languages.GetLabelFromCode("HistoryLblObject", language);
                        CreateDataGridHistory("oggetto");
                        break;

                    case "documentimghistorysender":
                        this.HistroyLblCaption.Text = Utils.Languages.GetLabelFromCode("HistoryLblSender", language);
                        CreateDataGridHistory("M");
                        break;

                    case "documentimghistoryrecipient":
                        this.HistroyLblCaption.Text = Utils.Languages.GetLabelFromCode("HistoryLblRecipient", language);
                        CreateDataGridHistory("D");
                        break;

                    case "documentimghistorymultiplesender":
                        this.HistroyLblCaption.Text = Utils.Languages.GetLabelFromCode("HistoryLblMultipleSender", language);
                        CreateDataGridHistory("MD");
                        break;

                    case "documentimghistorydate":
                        this.HistroyLblCaption.Text = Utils.Languages.GetLabelFromCode("HistoryLblDate", language);
                        CreateDataGridHistory("data");
                        break;

                    case "documentimghistorytipology":
                        this.HistroyLblCaption.Text = Utils.Languages.GetLabelFromCode("HistoryLblTipology", language);
                        CreateDataGridHistory("campiProfilati");
                        break;

                    case "documentimghistorystate":
                        //this.HistroyLblCaptionStato.Text = Utils.Languages.GetLabelFromCode("HistoryLblState", language);
                        this.HistroyLblCaption.Text = Utils.Languages.GetLabelFromCode("HistoryLblState", language);
                        CreateDataGridHistoryStato("stato");
                        break;
                }
            }
        }
        protected void CreateDataGridHistoryStato(string tipoOggetto)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            if (DocumentManager.getSelectedRecord().docNumber != null)
            {

                DataSet ds = DiagrammiManager.getDiagrammaStoricoDoc(DocumentManager.getSelectedRecord().docNumber);

                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    this.GridViewStoricoStati.DataSource = ds;
                    this.DatagridStoricoStati.Visible = true;
                    this.GridViewDocHistoryPnl.Visible = false;
                    //this.lblDettagliStato.Visible = false;
                    this.lblDettagli.Visible = false;
                    this.GridViewStoricoStati.DataBind();

                    this.GridViewStoricoStati.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid1", language);
                    this.GridViewStoricoStati.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid2", language);
                    this.GridViewStoricoStati.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid0", language);
                    this.GridViewStoricoStati.HeaderRow.Cells[3].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid3", language);
                }
                else
                {
                    //this.lblDettagliStato.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                    //this.lblDettagliStato.Visible = true;
                    this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                    this.lblDettagli.Visible = true;
                    return;
                }
            }
            else
            {
                //this.lblDettagliStato.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                //this.lblDettagliStato.Visible = true;
                this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                this.lblDettagli.Visible = true;
                return;
            }

        }

        protected void CreateDataGridHistory(string tipoOggetto)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            schedaDocumento = (DocsPaWR.SchedaDocumento)DocumentManager.getSelectedRecord();

            //fascicolo = FascicoliManager.getFascicoloSelezionato(this);
            ////se si tratta dello storico profilati fascicoli e le informazioni sul fascicolo non sono state correttamente reperite termino con un msg di errore
            //if ((fascicolo == null && tipoOggetto.Equals("campiProfilatiFasc")) || (fascicolo != null && fascicolo.systemID == null))
            //{

            //    this.lb_dettagli.Text = "Errore nel reperimento dei dati del fascicolo";
            //    this.lb_dettagli.Visible = true;
            //    return;
            //}

            if ((schedaDocumento == null && (tipoOggetto.Equals("campiProfilati")) || (schedaDocumento != null && schedaDocumento.docNumber == null)))
            {
                //Non sono state effettuate modifiche
                this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                this.lblDettagli.Visible = true;
                return;
            }

            if (tipoOggetto != null && tipoOggetto.Equals("oggetto"))
            {
                // Oggetto

                if (schedaDocumento.modOggetto == null || schedaDocumento.modOggetto.Equals("0"))
                {
                    //Non sono state effettuate modifiche
                    this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                    this.lblDettagli.Visible = true;
                }

                ListaStoriaOggetto = DocumentManager.getStoriaModifiche(this, schedaDocumento, tipoOggetto);
            }

            if (tipoOggetto != null && (tipoOggetto.Equals("M") || tipoOggetto.Equals("D") || tipoOggetto.Equals("MD")))
            {
                //Mittente / Destinatario / Destinatari multipli 
                ListaStoria = DocumentManager.getStoriaModifiche(schedaDocumento, tipoOggetto);
                if (ListaStoria.Length == 0)
                {
                    this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                    this.lblDettagli.Visible = true;
                }
            }

            if (tipoOggetto != null && tipoOggetto.Equals("data"))
            {
                //Data e ora
                ListaStoriaData = DocumentManager.getStoriaModifiche(schedaDocumento.docNumber);
                if (ListaStoriaData.Length == 0)
                {
                    this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                    this.lblDettagli.Visible = true;
                }
            }

            if (tipoOggetto != null && tipoOggetto.Equals("campiProfilati"))
            {
                //Tipologia
                ListaTipologia = DocumentManager.getStoriaProfilatiAtto(this, schedaDocumento.template, schedaDocumento.docNumber, RoleManager.GetRoleInSession().idGruppo);
            }

            if (ListaStoriaOggetto != null || ListaStoria != null || ListaStoriaData != null || ListaTipologia != null)
            {
                if (ListaStoriaOggetto != null)
                {
                    if (ListaStoriaOggetto.Length > 0)
                    {
                        var filtered_data = (from f in ListaStoriaOggetto
                                             select new
                                             {
                                                 Data = GetDate(f.dataModifica),
                                                 Ruolo = GetRule(f.ruolo.descrizione),
                                                 Utente = GetUser(f.utente.descrizione),
                                                 Modifica = GetModify(f.descrizione),
                                                 Tooltip0 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell0", language),
                                                 Tooltip1 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell1", language),
                                                 Tooltip2 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell2", language),
                                                 Tooltip3 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell3", language)
                                             }).ToArray();

                        this.GridViewHistory.DataSource = filtered_data;
                        this.GridViewHistory.DataBind();

                        this.GridViewHistory.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid0", language);
                        this.GridViewHistory.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid1", language);
                        this.GridViewHistory.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid2", language);
                        this.GridViewHistory.HeaderRow.Cells[3].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid3", language);
                    }
                    else
                    {
                        this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                        this.lblDettagli.Visible = true;
                    }
                }

                if (ListaStoria != null)
                {
                    if (ListaStoria.Length > 0)
                    {
                        if (tipoOggetto == "D" || tipoOggetto == "MD")
                        {
                            ListaStoria = DO_AggregaCorrSto(ListaStoria.ToArray());
                            this.GridViewDestinatari.DataSource = ListaStoria;
                            this.GridViewDestinatari.DataBind();

                            this.GridViewDestinatari.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid0", language);
                            this.GridViewDestinatari.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid1", language);
                            this.GridViewDestinatari.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid2", language);
                            this.GridViewDestinatari.HeaderRow.Cells[3].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid3", language);
                        }
                        else
                        {
                            var filtered_data = (from f in ListaStoria
                                                 select new
                                                 {
                                                     Data = GetDate(f.dataModifica),
                                                     Ruolo = GetRule(f.ruolo.descrizione),
                                                     Utente = GetUser(f.utente.descrizione),
                                                     Modifica = GetModify(f.descrizione),
                                                     Tooltip0 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell0", language),
                                                     Tooltip1 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell1", language),
                                                     Tooltip2 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell2", language),
                                                     Tooltip3 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell3", language)
                                                 }).ToArray();

                            this.GridViewHistory.DataSource = filtered_data;
                            this.GridViewHistory.DataBind();

                            this.GridViewHistory.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid0", language);
                            this.GridViewHistory.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid1", language);
                            this.GridViewHistory.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid2", language);
                            this.GridViewHistory.HeaderRow.Cells[3].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid3", language);
                        }
                    }
                    else
                    {
                        this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                        this.lblDettagli.Visible = true;
                    }
                }
                if (ListaStoriaData != null)
                {
                    if (ListaStoriaData.Length > 0)
                    {
                        var filtered_data = (from f in ListaStoriaData
                                             select new
                                             {
                                                 Data = GetDate(f.dataModifica),
                                                 Ruolo = GetRule(f.ruolo.descrizione),
                                                 Utente = GetUser(f.utente.descrizione),
                                                 Modifica = GetModify(f.dta_arrivo),
                                                 Tooltip0 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell0", language),
                                                 Tooltip1 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell1", language),
                                                 Tooltip2 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell2", language),
                                                 Tooltip3 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell3", language)
                                             }).ToArray();

                        this.GridViewHistory.DataSource = filtered_data;
                        this.GridViewHistory.DataBind();

                        this.GridViewHistory.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid0", language);
                        this.GridViewHistory.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid1", language);
                        this.GridViewHistory.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid2", language);
                        this.GridViewHistory.HeaderRow.Cells[3].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid3", language);
                    }
                    else
                    {
                        this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                        this.lblDettagli.Visible = true;
                    }
                }

                if (ListaTipologia != null)
                {
                    if (ListaTipologia.Length > 0)
                    {
                        var filtered_data = (from f in ListaTipologia
                                             select new
                                             {
                                                 Data = GetDate(f.dta_modifica),
                                                 Ruolo = GetRule(f.ruolo.descrizione),
                                                 Utente = GetUser(f.utente.descrizione),
                                                 //Modifica = GetModify(f.var_desc_modifica),
                                                 Modifica = GetModify(f.oggetto.DESCRIZIONE + ": " +  f.var_desc_modifica),
                                                 Tooltip0 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell0", language),
                                                 Tooltip1 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell1", language),
                                                 Tooltip2 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell2", language),
                                                 Tooltip3 = Utils.Languages.GetLabelFromCode("HistoryTooltipCell3", language)
                                             }).ToArray();

                        this.GridViewHistory.DataSource = filtered_data;
                        this.GridViewHistory.DataBind();

                        this.GridViewHistory.HeaderRow.Cells[0].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid0", language);
                        this.GridViewHistory.HeaderRow.Cells[1].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid1", language);
                        this.GridViewHistory.HeaderRow.Cells[2].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid2", language);
                        this.GridViewHistory.HeaderRow.Cells[3].Text = Utils.Languages.GetLabelFromCode("HistoryHeaderGrid3", language);
                    }
                    else
                    {
                        this.lblDettagli.Text = Utils.Languages.GetLabelFromCode("HistoryLblDettagliNoMod", language);
                        this.lblDettagli.Visible = true;
                    }
                }
            }
            else
            {
                this.GridViewHistory.Visible = false;
            }
        }

        private DocsPaWR.DocumentoStoricoMittente[] DO_AggregaCorrSto(DocsPaWR.DocumentoStoricoMittente[] list)
        {
            DocsPaWR.DocumentoStoricoMittente[] newList = new DocsPaWR.DocumentoStoricoMittente[list.Length];
            int counter = 0;
            counter = list.Length;
            for (int i = 0; i < list.Length; i++)
            {
                DocsPaWR.DocumentoStoricoMittente sm = new DocsPaWR.DocumentoStoricoMittente();
                sm = (DocsPaWR.DocumentoStoricoMittente)list[i];
                if (sm != null)
                {
                    for (int j = 0; j < list.Length; j++)
                    {
                        if (list[j] != null)
                        {

                            if (sm.dataModifica == ((DocsPaWR.DocumentoStoricoMittente)list[j]).dataModifica && sm.descrizione != ((DocsPaWR.DocumentoStoricoMittente)list[j]).descrizione)
                            {
                                sm.descrizione += "</li><li>" + ((DocsPaWR.DocumentoStoricoMittente)list[j]).descrizione;
                                list[j] = null;
                                counter--;
                            }
                        }
                    }
                }
            }

            // Normalizzo l'array eliminanto dalla lista gli elementi vuoti 
            int t = 0;
            newList = new DocsPaWR.DocumentoStoricoMittente[counter];
            for (int x = 0; x < list.Length; x++)
            {
                if (list[x] != null)
                {
                    newList[t] = list[x];
                    t++;
                }
            }
            return newList;
        }

        #region Formattazione dei valori sul gridview

        protected string GetDate(string valDate)
        {
            string result = valDate;
            return result;
        }

        protected string GetRule(string valRule)
        {
            string result = valRule;
            return result;
        }

        protected string GetUser(string valUser)
        {
            string result = valUser;
            return result;
        }

        protected string GetModify(string valModify)
        {
            string result = valModify;
            return result;
        }
        #endregion

        public string TypeHistory
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["typeHistory"] != null)
                {
                    result = HttpContext.Current.Session["typeHistory"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["typeHistory"] = value;
            }
        }

        protected void HistoryBtnChiudi_Click(object sender, EventArgs e)
        {
            try {
                this.TypeHistory = string.Empty;
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeHistory", "parent.closeAjaxModal('History','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

    }
}