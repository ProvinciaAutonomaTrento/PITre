using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.AdminTool.Gestione_RF
{
    public partial class GestioneRuoliInRF : System.Web.UI.Page
    {
        #region variabili e webcontrols

        
        protected System.Web.UI.WebControls.DataGrid dg_ruoli;
        protected DataSet dsRuoli;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoRicerca;
        protected DataSet dsUo;

		#endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            if (!IsPostBack)
            {
                this.lbl_percorso.Text = "&nbsp;Gestione ruoli in RF: " + Server.UrlDecode(Request.QueryString["codRF"].ToString());
                this.lbl_percorso.Text = this.lbl_percorso.Text + "   &gt; AOO collegata: " + Server.UrlDecode(Request.QueryString["codAooCol"].ToString());
                
                //se l'RF è disabilitato non si possono associare nuovi ruoli all'RF
                if (Request.QueryString["chaDisabled"].ToString()=="1")
                    gestioneCampiRFDisabilitato();

                this.Inizialize();
            }
        }

        protected void gestioneCampiRFDisabilitato()
        {
            this.txt_ricerca.Enabled = false;
            this.ddl_ricerca.Enabled = false;
            this.btn_find.Enabled = false;
            this.lbl_avviso.Text = "RF disabilitato, pertanto non è possibile associare nuovi ruoli";
            this.lbl_avviso.Visible = true;
        }

        private void InitializeDataSetRuoli()
        {
            dsRuoli = new DataSet();
            DataColumn dc;
            dsRuoli.Tables.Add("RUOLI");
            dc = new DataColumn("IDRuolo");
            dsRuoli.Tables["RUOLI"].Columns.Add(dc);
            dc = new DataColumn("codice");
            dsRuoli.Tables["RUOLI"].Columns.Add(dc);
            dc = new DataColumn("descrizione");
            dsRuoli.Tables["RUOLI"].Columns.Add(dc);
            dc = new DataColumn("IDAmministrazione");
            dsRuoli.Tables["RUOLI"].Columns.Add(dc);
            dc = new DataColumn("IDGruppo");
            dsRuoli.Tables["RUOLI"].Columns.Add(dc);
        }


        /// <summary>
        /// carica il DataGrid relativo ai ruoli che attualmente sono associati alla Aoo
        /// collegata all'RF corrente
        /// </summary>
        private void Inizialize()
        {
            this.LoadRuoliAttualiRF();
            //this.LoadUoAttualiRF();
        }

        #region UO
        private void InitializeDataSetUO()
        {
            dsUo = new DataSet();
            DataColumn dc;
            dsUo.Tables.Add("UO");
            dc = new DataColumn("codice");
            dsUo.Tables["UO"].Columns.Add(dc);
            dc = new DataColumn("IDUO");
            dsUo.Tables["UO"].Columns.Add(dc);
            dc = new DataColumn("descrizione");
            dsUo.Tables["UO"].Columns.Add(dc);
        }

        private void LoadUoAttualiRF(string tipoRicerca, string ricerca)
        {
            try
            {
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();

                //Cerco solo UO della AOO COLLEGATA: idAooColl 
                //theManager.ListaUOInReg(Request.QueryString["idAooColl"].ToString(), tipoRicerca, ricerca);

                //Cerco tutte le UO dell'amministrazione
                theManager.ListaUO("", "", Request.QueryString["idAmm"].ToString());
                                
                //if (theManager.getListaUOInReg() != null && theManager.getListaUOInReg().Count > 0)
                if (theManager.getListaUO() != null && theManager.getListaUO().Count > 0)
                {
                    this.dg_ruoli.Visible = true;
                    this.dg_ruoliTrovatiInRF.Visible = false;
                    this.dg_UOTrovatiInRF.Visible = true;
                    this.lbl_risultatoRuoliRF.Visible = false;

                    this.InitializeDataSetUO();

                    DataRow row;

                    //foreach (DocsPAWA.DocsPaWR.OrgUO uo in theManager.getListaUOInReg())
                    foreach (DocsPAWA.DocsPaWR.OrgUO uo in theManager.getListaUO())
                    {
                        row = dsUo.Tables[0].NewRow();
                        row["codice"] = uo.Codice;
                        row["IDUO"] = uo.IDCorrGlobale;
                        row["descrizione"] = uo.Descrizione;

                        dsUo.Tables["UO"].Rows.Add(row);
                    }

                    this.SetSessionDataSetUo(dsUo);
                    
                    DataView dv = dsUo.Tables["UO"].DefaultView;
                    dv.Sort = "descrizione ASC";
                    this.dg_UOTrovatiInRF.DataSource = dv;
                    this.dg_UOTrovatiInRF.DataBind();
                }                
            }
            catch
            {
                this.gestErrori();
            }
        }

        #endregion

        #region caricamento Ruoli Attuali in RF

        private void LoadRuoliAttualiRF()
        {
           try
             {
                 Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();

                 //Cerco solo i ruoli della AOO COLLEGATA: idReg 
                 theManager.GetListaRuoliAOO(Request.QueryString["idReg"].ToString());

                 if (theManager.getListaRuoliAOO() != null && theManager.getListaRuoliAOO().Count >= 0)
                 {
                     this.dg_ruoli.Visible = true;
                     //this.dg_UOTrovatiInRF.Visible = false;
                     this.lbl_risultatoRuoliRF.Visible = false;

                     this.InitializeDataSetRuoli();

                     DataRow row;

                     foreach (DocsPAWA.DocsPaWR.OrgRuolo ruolo in theManager.getListaRuoliAOO())
                     {
                         row = dsRuoli.Tables[0].NewRow();
                         row["IDRuolo"] = ruolo.IDCorrGlobale;
                         row["codice"] = ruolo.CodiceRubrica;
                         row["descrizione"] = ruolo.Descrizione;
                         row["IDAmministrazione"] = ruolo.IDAmministrazione;
                         row["IDGruppo"] = ruolo.IDGruppo;

                         dsRuoli.Tables["RUOLI"].Rows.Add(row);
                     }

                     DataView dv = dsRuoli.Tables["RUOLI"].DefaultView;
                     dv.Sort = "descrizione ASC";
                     dg_ruoli.DataSource = dv;
                     dg_ruoli.DataBind();
                 }
                 else
                 {
                     this.dg_ruoli.Visible = false;
                     this.lbl_risultatoRuoliRF.Visible = true;
                 }
             }
             catch
             {
                 this.gestErrori();
             }                 
        }

        protected void dg_UOTrovatiInRF_PreRender(object sender, System.EventArgs e)
        {
            for (int i = 0; i < this.dg_UOTrovatiInRF.Items.Count; i++)
            {

                if (this.dg_UOTrovatiInRF.Items[i].ItemIndex >= 0)
                {
                    switch (this.dg_UOTrovatiInRF.Items[i].ItemType.ToString().Trim())
                    {
                        case "Item":
                            this.dg_UOTrovatiInRF.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                            this.dg_UOTrovatiInRF.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioN'");
                            break;

                        case "AlternatingItem":
                            this.dg_UOTrovatiInRF.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                            this.dg_UOTrovatiInRF.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioA'");
                            break;
                    }
                }
            }
        }

        protected void dg_ruoliTrovatiInRF_PreRender(object sender, System.EventArgs e)
        {
            for (int i = 0; i < this.dg_ruoliTrovatiInRF.Items.Count; i++)
            {

                if (this.dg_ruoliTrovatiInRF.Items[i].ItemIndex >= 0)
                {
                    switch (this.dg_ruoliTrovatiInRF.Items[i].ItemType.ToString().Trim())
                    {
                        case "Item":
                            this.dg_ruoliTrovatiInRF.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                            this.dg_ruoliTrovatiInRF.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioN'");
                            break;

                        case "AlternatingItem":
                            this.dg_ruoliTrovatiInRF.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                            this.dg_ruoliTrovatiInRF.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioA'");
                            break;
                    }
                }
            }
        }

        protected void btn_find_Click(object sender, System.EventArgs e)
        {
            if (this.ddl_tipoRicerca.SelectedValue.Equals("UO"))
            {
                this.RicercaUOInRF();
                this.pnl_ruoli.Visible = false;
                this.pnl_uo.Visible = true;
            }
            else
            {
                this.RicercaRuoliInRF();
                this.pnl_uo.Visible = false;
                this.pnl_ruoli.Visible = true;
            }
        }

        protected void RicercaUOInRF()
        {
            try
            {
                this.dg_ruoliTrovatiInRF.Visible = false;
                string ricerca = this.txt_ricerca.Text.Trim();
                //if ((ricerca != null && ricerca != string.Empty) || (this.ddl_ricerca.SelectedValue.ToString().Equals("*")))
                //{
                    LoadUoAttualiRF(this.ddl_ricerca.SelectedValue.ToString(), ricerca);
                //}
            }
            catch (Exception e)
            {
                this.gestErrori();
            }
        }

        protected void RicercaRuoliInRF()
        {
            try
            {
                this.dg_UOTrovatiInRF.Visible = false;
                string ricerca = this.txt_ricerca.Text.Trim();
                if ((ricerca != null && ricerca != string.Empty && ricerca != "") || (this.ddl_ricerca.SelectedValue.ToString().Equals("*")))
                {
                    string listaDaEscludere = string.Empty;
                    // prende le IDRuolo da escludere nella ricerca
                    if (this.dg_ruoli.Items.Count > 0)
                    {
                        foreach (DataGridItem item in this.dg_ruoli.Items)
                        {
                            listaDaEscludere += "," + item.Cells[0].Text;
                        }
                        listaDaEscludere = "(" + listaDaEscludere.Substring(1, listaDaEscludere.Length - 1) + ")";
                    }

                    Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                    
                    //Cerco solo i ruoli che sono associati all'RF selezionato (idReg)
                    //theManager.ListaRuoli(Request.QueryString["idAmm"].ToString(), this.ddl_ricerca.SelectedValue.ToString(), ricerca, Request.QueryString["idAooColl"].ToString(), listaDaEscludere);
                    
                    //Cerco tutti i ruoli dell'amministrazione
                    theManager.ListaRuoli(Request.QueryString["idAmm"].ToString(), this.ddl_ricerca.SelectedValue.ToString(), ricerca, null, listaDaEscludere);

                    if (theManager.getListaRuoli() != null && theManager.getListaRuoli().Count > 0)
                    {
                        this.dg_ruoliTrovatiInRF.Visible = true;
                        this.lbl_avviso.Text = "";

                        this.InitializeDataSetRuoli();

                        DataRow row;

                        foreach (DocsPAWA.DocsPaWR.OrgRuolo ruolo in theManager.getListaRuoli())
                        {
                            row = dsRuoli.Tables[0].NewRow();
                            row["IDRuolo"] = ruolo.IDCorrGlobale;
                            row["codice"] = ruolo.CodiceRubrica;
                            row["descrizione"] = ruolo.Descrizione;
                            row["IDAmministrazione"] = ruolo.IDAmministrazione;
                            row["IDGruppo"] = ruolo.IDGruppo;

                            dsRuoli.Tables["RUOLI"].Rows.Add(row);
                        }

                        // Impostazione dataset in sessione
                        this.SetSessionDataSetRuoli(dsRuoli);
                        dg_ruoliTrovatiInRF.CurrentPageIndex = 0;
                        DataView dv = dsRuoli.Tables["RUOLI"].DefaultView;
                        dv.Sort = "descrizione ASC";
                        this.dg_ruoliTrovatiInRF.DataSource = dv;
                        this.dg_ruoliTrovatiInRF.DataBind();
                    }
                    else
                    {
                        this.dg_ruoliTrovatiInRF.Visible = false;
                        this.lbl_avviso.Text = "Nessun dato trovato.";
                    }
                }
            }
            catch
            {
                this.gestErrori();
            }
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            try
            {
                if (this.dg_ruoliTrovatiInRF.Items.Count > 0)
                {
                    foreach (DataGridItem item in this.dg_ruoliTrovatiInRF.Items)
                    {
                        item.Cells[5].Attributes.Add("onclick", "window.document.body.style.cursor='wait'; void(0);");
                    }
                }
            }
            catch
            {
                this.gestErrori();
            }
        }

        #region Gestione del risultato della ricerca ruoli in sessione

        private void SetSessionDataSetRuoli(DataSet dsRuoli)
        {
            Session["RUOLIDATASET"] = dsRuoli;
        }

        private DataSet GetSessionDataSetRuoli()
        {
            return (DataSet)Session["RUOLIDATASET"];
        }

        private void RemoveSessionDataSetRuoli()
        {
            this.GetSessionDataSetRuoli().Dispose();
            Session.Remove("RUOLIDATASET");
        }

        #endregion	

        #region Gestione del risultato della ricerca UO in Registro
        private void SetSessionDataSetUo(DataSet dsUO)
        {
            Session["UODATASET"] = dsUO;
        }

        private DataSet GetSessionDataSetUO()
        {
            return (DataSet)Session["UODATASET"];
        }

        private void RemoveSessionDataSetUO()
        {
            this.GetSessionDataSetUO().Dispose();
            Session.Remove("UODATASET");
        }
        #endregion

        #region Gestione errori
        /// <summary>
        /// gestore degli errori
        /// </summary>
        private void gestErrori()
        {
            this.dg_ruoliTrovatiInRF.Visible = false;
            this.lbl_avviso.Text = "Errore di sistema!";
        }
        #endregion


        private bool AssociazioneRuoloRF(string idRuolo, string idRF)
        {
            bool result = false;

            try
            {
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.AssociazioneRFRuolo(idRF, idRuolo);

                DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                esito = theManager.getEsitoOperazione();

                if (esito.Codice.Equals(0))
                {
                    //inseriamo i diritti dei ruoli per tutte le caselle dell'RF(visibilità 1 su consulta,notifica,spedisci)
                    DocsPAWA.DocsPaWR.DocsPaWebService ws =  new DocsPaWR.DocsPaWebService();
                    DocsPAWA.DocsPaWR.CasellaRegistro[] caselle = ws.AmmGetMailRegistro(idRF);
                    System.Collections.Generic.List<DocsPAWA.DocsPaWR.RightRuoloMailRegistro> rightRuoloMailRegistro = new System.Collections.Generic.List<DocsPaWR.RightRuoloMailRegistro>();
                    if (caselle != null && caselle.Length > 0)
                    {
                        foreach (DocsPAWA.DocsPaWR.CasellaRegistro c in caselle)
                        {
                            //di default imposto la visibilità del ruolo sulla mail a 0(nessuna diritto)
                            // l'utente modifica la visibilità da organigramma
                            DocsPAWA.DocsPaWR.RightRuoloMailRegistro right = new DocsPaWR.RightRuoloMailRegistro
                            {
                                IdRegistro = idRF,
                                idRuolo = idRuolo,
                                EmailRegistro = c.EmailRegistro,
                                cha_consulta = "false",
                                cha_notifica = "false",
                                cha_spedisci = "false"
                            };
                            rightRuoloMailRegistro.Add(right);
                        }
                        DocsPAWA.DocsPaWR.ValidationResultInfo res = ws.AmmInsRightMailRegistro(rightRuoloMailRegistro.ToArray());
                        if (res.Value)
                            result = true;
                        else result = false;
                    }
                }
                else
                {
                    if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                    {
                        string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                        this.ClientScript.RegisterStartupScript(this.GetType(), "alertJavaScript", scriptString);
                    }
                }

                esito = null;
            }
            catch
            {
                this.gestErrori();
            }

            return result;
        }

        #endregion

        protected void dg_ruoliTrovatiInRF_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (this.AssociazioneRuoloRF(e.Item.Cells[0].Text, Request.QueryString["idReg"]))
            {
                //si inizializza il DataSet dei Ruoli
                this.InitializeDataSetRuoli();

                //prendo dalla sessione il dataSet dei Ruoli
                dsRuoli = GetSessionDataSetRuoli();

                //Ricarica la ricerca dei ruoli nel lato sinistro della pagina
                LoadRicercaRuoliDopoIns(dsRuoli, e.Item.Cells[0].Text);

                //Ricarico la parte destra della pagina
                LoadRuoliAttualiRF();
            }
        }

        protected void dg_UOTrovatiInRF_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.Item.Cells.Count > 1)
            {
                string idUo = e.Item.Cells[1].Text;

                // metodo che mi restituisce tutti i ruoli di un dato UO
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.ListaRuoliUORic(idUo, true);

                ArrayList ii = theManager.getListaRuoliUO();
                if (theManager.getListaRuoliUO() != null)
                {
                    this.InitializeDataSetUO();
                    foreach (DocsPaWR.OrgRuolo ruo in theManager.getListaRuoliUO())
                    {
                        if (this.AssociazioneRuoloRF(ruo.IDCorrGlobale, Request.QueryString["idReg"]))
                        {
                            LoadRuoliAttualiRF();
                        }
                    }
                    dsUo = GetSessionDataSetUO();

                    LoadRicercaUODopoIns(dsUo, idUo);
                }
            }
        }

        private void LoadRicercaRuoliDopoIns(DataSet dsRuoli, string IDeliminato)
        {
            try
            {
                foreach (DataRow row in dsRuoli.Tables["RUOLI"].Rows)
                {
                    if (row["IDRuolo"].Equals(IDeliminato))
                    {
                        row.Delete();
                        break;
                    }
                }

                if (dsRuoli.Tables["RUOLI"].Rows.Count > 0)
                {
                    if (this.dg_ruoliTrovatiInRF.Items.Count == 1)
                        this.dg_ruoliTrovatiInRF.CurrentPageIndex -= 1;

                    DataView dv = dsRuoli.Tables["RUOLI"].DefaultView;
                    dv.Sort = "descrizione ASC";
                    this.dg_ruoliTrovatiInRF.DataSource = dv;
                    this.dg_ruoliTrovatiInRF.DataBind();

                    this.SetSessionDataSetRuoli(dsRuoli);
                }
                else
                {
                    this.dg_ruoliTrovatiInRF.Visible = false;
                }
            }
            catch
            {
                this.gestErrori();
            }
        }

        private void LoadRicercaUODopoIns(DataSet dsUo, string IDeliminato)
        {
            try
            {
                foreach (DataRow row in dsUo.Tables["UO"].Rows)
                {
                    if (row["IDUO"].Equals(IDeliminato))
                    {
                        row.Delete();
                        break;
                    }
                }

                if (dsUo.Tables["UO"].Rows.Count > 0)
                {
                    if (this.dg_UOTrovatiInRF.Items.Count == 1)
                        this.dg_UOTrovatiInRF.CurrentPageIndex -= 1;

                    DataView dv = dsUo.Tables["UO"].DefaultView;
                    dv.Sort = "descrizione ASC";
                    this.dg_UOTrovatiInRF.DataSource = dv;
                    this.dg_UOTrovatiInRF.DataBind();

                    this.SetSessionDataSetUo(dsUo);
                }
                else
                {
                    this.dg_UOTrovatiInRF.Visible = false;
                }
            }
            catch
            {
                this.gestErrori();
            }
        }

        private bool EliminaAssociazioneRFRuolo(string idRuolo, string idRf)
        {
            bool result = false;

            try
            {
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.DeleteAssociazioneRFRuolo(idRf, idRuolo);

                DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
                esito = theManager.getEsitoOperazione();

                if (esito.Codice.Equals(0))
                {
                    DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                    DocsPAWA.DocsPaWR.ValidationResultInfo res = ws.AmmDelRightMailRegistro(idRf, idRuolo, string.Empty);
                    if(res.Value)
                        result = true;
                    else
                        result = false;
                }
                else
                {
                    if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
                    {
                        string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione.Replace("'", "''") + "');</SCRIPT>";
                        this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                    }
                }
                esito = null;
            }
            catch
            {
                this.gestErrori();
            }
            return result;
        }

        protected void ddl_ricerca_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.ddl_ricerca.SelectedValue.ToString().Equals("*"))
            {
                this.txt_ricerca.Enabled = false;
                this.txt_ricerca.Text = "";
                this.txt_ricerca.BackColor = System.Drawing.Color.LightGray;
            }
            else
            {
                this.txt_ricerca.Enabled = true;
                this.txt_ricerca.BackColor = System.Drawing.Color.White;
            }
        }


        protected void dg_ruoli_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("Eliminazione"))
                {
                    if (this.EliminaAssociazioneRFRuolo(e.Item.Cells[0].Text, Request.QueryString["idReg"]))
                    {



                        if (dg_ruoli.Items.Count > 1)
                        {
                            this.LoadRuoliAttualiRF();
                        }
                        else
                        {
                            // ripulisce il datagrid
                            this.InitializeDataSetRuoli();

                            DataView dv = dsRuoli.Tables["RUOLI"].DefaultView;
                            dv.Sort = "descrizione ASC";
                            dg_ruoli.DataSource = dv;
                            dg_ruoli.DataBind();

                            this.dg_ruoli.Visible = false;
                            this.lbl_risultatoRuoliRF.Visible = true;
                        
                        }

                        //ricarica la ricerca
                        this.RicercaRuoliInRF();
                    }
                }
            }
            catch
            {
                this.gestErrori();
            }

        }

        

        protected void dg_ruoli_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            e.Item.Cells[5].Attributes.Add("onclick", "if (!window.confirm('Sei sicuro di voler disassociare questo ruolo dall\\'RF?')) {return false};");			
        }

        protected void dg_ruoliTrovatiInRF_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            this.dg_ruoliTrovatiInRF.CurrentPageIndex = e.NewPageIndex;
            dsRuoli = this.GetSessionDataSetRuoli();
            DataView dv = dsRuoli.Tables["RUOLI"].DefaultView;
            dv.Sort = "descrizione ASC";
            this.dg_ruoliTrovatiInRF.DataSource = dv;
            this.dg_ruoliTrovatiInRF.DataBind();
        }

        protected void dg_UOTrovatiInRF_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            this.dg_UOTrovatiInRF.CurrentPageIndex = e.NewPageIndex;
            //dsUo = this.GetSessionDataSetRuoli();
            dsUo = this.GetSessionDataSetUO();
            DataView dv = dsUo.Tables["UO"].DefaultView;
            dv.Sort = "descrizione ASC";
            this.dg_UOTrovatiInRF.DataSource = dv;
            this.dg_UOTrovatiInRF.DataBind();
        }
    }
}
