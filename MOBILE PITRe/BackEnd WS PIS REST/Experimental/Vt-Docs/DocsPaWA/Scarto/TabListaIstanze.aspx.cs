using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace DocsPAWA.Scarto
{
    public partial class TabListaIstanze : DocsPAWA.CssPage
    {
        protected ArrayList dataTableProt;
        protected int nRec;
        protected int numTotPage;
        DocsPAWA.DocsPaWR.InfoScarto infoScarto;
        protected System.Web.UI.HtmlControls.HtmlTableRow regAut;
        protected System.Web.UI.HtmlControls.HtmlTableRow ric_aut_lbl;
        protected System.Web.UI.WebControls.Label lbl_estr_richiesta;
        protected System.Web.UI.WebControls.Label lbl_note;
        protected System.Web.UI.HtmlControls.HtmlTableRow autor_lbl;
        protected System.Web.UI.WebControls.Label lbl_estr_autorizzazione;
        protected System.Web.UI.WebControls.TextBox txt_autor;
        protected Utilities.MessageBox MsgBox_Scarta;
        protected System.Web.UI.WebControls.DropDownList ddl_operazioni;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    Utils.startUp(this);
                    //this.AttatchGridPagingWaitControl();
                    nRec = 0;
                    //this.AttachWaitingControl();
                    infoScarto = new DocsPAWA.DocsPaWR.InfoScarto();
                    infoScarto = FascicoliManager.getIstanzaScarto(this);
                    if (infoScarto != null)
                        this.FillData(this.dg_fasc.CurrentPageIndex + 1, infoScarto);
                    campiDescNote();
                }
            }
            catch (Exception es)
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
            this.Load += new System.EventHandler(this.Page_Load);
            this.dg_fasc.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_fasc_ItemCommand);
            this.dg_fasc.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dg_fasc_ItemCreated);
            this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
            this.btn_esegui.Click += new System.EventHandler(btn_esegui_Click);
            this.btn_report.Click += new System.Web.UI.ImageClickEventHandler(this.btn_report_Click);
            this.MsgBox_Scarta.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.MsgBox_Scarta_GetMessageBoxResponse);
            this.ddl_operazioni.SelectedIndexChanged += new System.EventHandler(this.ddl_operazioni_SelectedIndexChanged);
        }
        #endregion


        #region DATAGRID

        private void dg_fasc_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName.Equals("EliminaAreaScarto"))
            {
                DocsPaWR.Fascicolo fascicoloSelezionato;
                DocsPaWR.Fascicolo[] listaFasc = null;
                listaFasc = FascicoliManager.getListaFascicoliInGriglia(this);
                fascicoloSelezionato = listaFasc[e.Item.ItemIndex];

                FascicoliManager.eliminaDaAreaScarto(Page, fascicoloSelezionato, null, false, "");
                infoScarto = new DocsPAWA.DocsPaWR.InfoScarto();
                infoScarto = FascicoliManager.getIstanzaScarto(this);
                if (infoScarto != null)
                    this.FillData(this.dg_fasc.CurrentPageIndex + 1, infoScarto);
                //((Label)e.Item.Cells[9].Controls[1]).Text = "0";
            }
        }

        private void dg_fasc_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }
            }
        }

        private void FillData(int requestedPage, DocsPAWA.DocsPaWR.InfoScarto infoScarto)
        {
            try
            {
                dataTableProt = new ArrayList();
                caricaDataTablesFascicoli(infoScarto, requestedPage, out nRec, out numTotPage);
                this.dg_fasc.DataSource = dataTableProt;
                this.dg_fasc.DataBind();
                this.dg_fasc.Visible = (this.TotalRecordCount > 0);
                this.trHeader.Visible = (this.TotalRecordCount > 0);
                this.lbl_messaggio.Visible = false;
                if (TotalRecordCount == 0)
                {
                    this.lbl_messaggio.Text = "Nessun fascicolo presente.";
                    this.lbl_messaggio.Visible = true;
                }
            }
            catch (Exception es)
            {
                string error = es.ToString();
            }
        }

        private void caricaDataTablesFascicoli(DocsPAWA.DocsPaWR.InfoScarto infoScarto, int numPage, out int nRec, out int numTotPage)
        {
            nRec = 0;
            numTotPage = 0;
            try
            {
                dataTableProt = new ArrayList();
                DocsPaWR.Fascicolo[] listaFasc = null;
                listaFasc = FascicoliManager.getListaFascicoliInScarto(this, infoScarto, numPage, out numTotPage, out nRec);
                this.TotalRecordCount = nRec;
                this.dg_fasc.VirtualItemCount = this.TotalRecordCount;
                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        for (int i = 0; i < listaFasc.Length; i++)
                        {
                            DocsPaWR.Fascicolo fasc = listaFasc[i];

                            //calcolo mesi: oggi - chiusura = mesi dalla chiusura
                            int numMesi = 0;
                            DateTime today = DateTime.Today;
                            DateTime chiusura = Convert.ToDateTime(fasc.chiusura);

                            if (today.Year == chiusura.Year)
                                numMesi = today.Month - chiusura.Month;
                            if (today.Year > chiusura.Year)
                            {
                                int intervallo = today.Year - chiusura.Year;
                                numMesi = today.Month - chiusura.Month + (12 * intervallo);
                            }
                            string numMesiChiusura = numMesi.ToString();
                            dataTableProt.Add(new Cols(fasc.stato, fasc.tipo, fasc.codiceGerarchia, fasc.codice, fasc.descrizione, fasc.chiusura, fasc.numMesiConservazione, numMesiChiusura, fasc.inScarto, i));
                        }
                    }
                }
                FascicoliManager.setListaFascicoliInGriglia(this, listaFasc);
                FascicoliManager.setDataTableDocDaArchiv(this, dataTableProt);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private int TotalRecordCount
        {
            get
            {
                int count = 0;

                if (this.ViewState["TotalRecordCount"] != null)
                    Int32.TryParse(this.ViewState["TotalRecordCount"].ToString(), out count);

                return count;
            }
            set
            {
                this.ViewState["TotalRecordCount"] = value;
            }
        }

        private void RefreshCountDocumenti()
        {
            this.titolo.Text = "Elenco fascicoli - Trovati " + this.TotalRecordCount.ToString() + " elementi.";
            //if (this.TotalRecordCount == 0)
            //{
            //    this.btn_archivia.Visible = false;
            //}
        }


        #endregion

        #region operazioni

        private void campiDescNote()
        {
            dg_fasc.Columns[9].Visible = true;
            if (string.IsNullOrEmpty(infoScarto.descrizione) && infoScarto.stato == "N")
            {
                txt_descrizione.Text = "";
                txt_descrizione.Enabled = true;
                lbl_descrizione.Text = "Inserisci descrizione *";
                txt_note.Text = "";
                this.btn_salva.Text = "Salva";
                pnl_statiOp.Visible = false;
            }
            else
            {
                txt_descrizione.Text = infoScarto.descrizione;
                lbl_descrizione.Text = "Descrizione";
                txt_descrizione.Enabled = false;
                txt_note.Text = infoScarto.note;
                this.btn_salva.Text = "Modifica";
                pnl_statiOp.Visible = true;
                impostaStatoOp();
            }

        }

        private void impostaStatoOp()
        {
            int valore;
            lbl_stato.Text = "";
            ddl_operazioni.Visible = true;
            ddl_operazioni.Items.Clear();
            switch (infoScarto.stato)
            {
                case "A": lbl_stato.Text = "Aperta";
                    this.ddl_operazioni.Items.AddRange(this.GetListItemsOperazioni("A"));
                    //ddl_operazioni.Items.Add(new ListItem("Chiudi"));
                   
                    //Valore pulsante esegui
                    valore = 60;
                    btn_esegui.Width = Convert.ToUInt16(valore);
                    btn_esegui.Text = "Chiudi";
                    //pulsanti o etichetti necessari per l'operazione in corso
                    
                    break;
                case "C": dg_fasc.Columns[9].Visible = false;
                    lbl_stato.Text = "Chiusa";
                    this.ddl_operazioni.Items.AddRange(this.GetListItemsOperazioni("C"));
                    //ddl_operazioni.Items.Add(new ListItem("Registra estremi richiesta autorizzazione"));
                    //ddl_operazioni.Items.Add(new ListItem("Riapri"));
                    //valore pulsante esegui
                    valore = 160;
                    btn_esegui.Width = Convert.ToUInt16(valore);
                    btn_esegui.Text = "Registra estremi";
                    //pulsanti o etichetti necessari per l'operazione in corso
                    richAut.Visible = true;
                    report.Visible = true;
                    break;
                case "RI": dg_fasc.Columns[9].Visible = false;
                    lbl_stato.Text = "Richiesta inoltrata";
                    this.ddl_operazioni.Items.AddRange(this.GetListItemsOperazioni("RI"));

                    //ddl_operazioni.Items.Add(new ListItem("Registra estremi provvedimento autorizzazione"));
                    //ddl_operazioni.Items.Add(new ListItem("Autorizzazione negata"));
                    //Nuovi valori dell'istanza da rendere visibili dato lo stato 
                    ric_aut_lbl.Visible = true;
                    lbl_estr_richiesta.Text = infoScarto.estremi_richiesta;
                    //valore pulsante esegui
                    valore = 160;
                    btn_esegui.Width = Convert.ToUInt16(valore);
                    btn_esegui.Text = "Registra estremi";
                    //pulsanti o etichetti necessari per l'operazione in corso
                    regAut.Visible = true;
                    break;
                case "RA": dg_fasc.Columns[9].Visible = false;
                    lbl_stato.Text = "Richiesta autorizzata";
                    this.ddl_operazioni.Items.AddRange(this.GetListItemsOperazioni("RA"));
                    //ddl_operazioni.Items.Add(new ListItem("Scarta"));
                    //Nuovi valori dell'istanza da rendere visibili dato lo stato 
                    ric_aut_lbl.Visible = true;
                    lbl_estr_richiesta.Text = infoScarto.estremi_richiesta;
                    autor_lbl.Visible = true;
                    lbl_estr_autorizzazione.Text = infoScarto.estremi_autorizzazione;
                    //valore pulsante esegui
                    valore = 60;
                    btn_esegui.Width = Convert.ToUInt16(valore);
                    btn_esegui.Text = "Scarta";
                    break;
                case "S": dg_fasc.Columns[9].Visible = false;
                    lbl_stato.Text = "Scartata";
                    op.Visible = false;
                    ddl_operazioni.Visible = false;
                    btn_esegui.Visible = false;
                    btn_salva.Visible = false;
                    txt_note.Enabled = false;
                    lbl_note.Text = "Note";
                    ric_aut_lbl.Visible = true;
                    lbl_estr_richiesta.Text = infoScarto.estremi_richiesta;
                    autor_lbl.Visible = true;
                    lbl_estr_autorizzazione.Text = infoScarto.estremi_autorizzazione;
                    dataScarto.Visible = true;
                    lbl_data_scarto.Text = infoScarto.data_Scarto;
                    lbl_messaggio.Visible = false;
                    break;
            }
        }

        private void ddl_operazioni_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string value = ddl_operazioni.SelectedValue;
            if (value == "Riapri")
            {
                btn_esegui.Text = "Riapri";
                richAut.Visible = false;
                report.Visible = false;
            }
            if (value == "Registra estremi richiesta autorizzazione")
            {
                btn_esegui.Text = "Registra estremi";
                richAut.Visible = true;
                report.Visible = true;
            }
            if (value == "Autorizzazione negata")
            {
                regAut.Visible = false;
                btn_esegui.Text = "Richiedi autorizzazione";
            }
            if (value == "Registra estremi provvedimento autorizzazione")
            {
                regAut.Visible = true;
                btn_esegui.Text = "Registra estremi";
            }
        }

        private void btn_esegui_Click(object sender, System.EventArgs e)
        {
            int valore;
            ddl_operazioni.Visible = true;
            if (ddl_operazioni.SelectedIndex == -1)
                ddl_operazioni.SelectedIndex = 0;
            infoScarto = FascicoliManager.getIstanzaScarto(this);
            if (lbl_stato.Text == "Aperta" && ddl_operazioni.SelectedItem.Text == "Chiudi" && btn_esegui.Text == "Chiudi")
            {
                infoScarto.stato = "C";
                //webmethod per cambiare lo stato dell'istanza di scarto
                if (FascicoliManager.cambiaStatoScarto(this, infoScarto, UserManager.getInfoUtente(), ""))
                {
                    FascicoliManager.setIstanzaScarto(this, infoScarto);
                    dg_fasc.Columns[9].Visible = false;
                   
                    //Reimposta lo stato e l'operazione successiva
                    lbl_stato.Text = "Chiusa";
                    ddl_operazioni.Items.Clear();
                    this.ddl_operazioni.Items.AddRange(this.GetListItemsOperazioni("C"));
                    //ddl_operazioni.Items.Add(new ListItem("Registra estremi richiesta autorizzazione"));
                    richAut.Visible = true;
                    report.Visible = true;
                    valore = 160;
                    btn_esegui.Width = Convert.ToUInt16(valore);
                    btn_esegui.Text = "Registra estremi";
                    caricaDGFrameSX();
                }
                return;
            }
            if (lbl_stato.Text == "Chiusa" && ddl_operazioni.SelectedItem.Text == "Registra estremi richiesta autorizzazione")
            {
                infoScarto.stato = "RI";
                if (!string.IsNullOrEmpty(txt_ricAut.Text))
                {
                    infoScarto.estremi_richiesta = txt_ricAut.Text;
                    report.Visible = false;
                    //webmethod per cambiare lo stato dell'istanza di scarto
                    if (FascicoliManager.cambiaStatoScarto(this, infoScarto, UserManager.getInfoUtente(), "estremi_richiesta"))
                    {
                        FascicoliManager.setIstanzaScarto(this, infoScarto);
                        dg_fasc.Columns[9].Visible = false;
                        //Reimposta lo stato e l'operazione successiva
                        lbl_stato.Text = "Richiesta inoltrata";
                        ddl_operazioni.Items.Clear();
                        this.ddl_operazioni.Items.AddRange(this.GetListItemsOperazioni("RI"));

                        
                        //ddl_operazioni.Items.Add(new ListItem("Registra estremi provvedimento autorizzazione"));
                        report.Visible = false;
                        richAut.Visible = false;
                        regAut.Visible = true;
                        ric_aut_lbl.Visible = true;
                        lbl_estr_richiesta.Text = infoScarto.estremi_richiesta;
                        valore = 160;
                        btn_esegui.Width = Convert.ToUInt16(valore);
                        btn_esegui.Text = "Registra estremi";
                        caricaDGFrameSX();
                    }
                    return;
                }
                else
                {
                    Response.Write("<script>alert('Attenzione campo richiesta autorizzazione obbligatorio.')</script>");
                    return;
                }
            }
            if (lbl_stato.Text == "Chiusa" && ddl_operazioni.SelectedItem.Text == "Riapri")
            {
                infoScarto.stato = "A";
                if (FascicoliManager.cambiaStatoScarto(this, infoScarto, UserManager.getInfoUtente(), ""))
                {
                    FascicoliManager.setIstanzaScarto(this, infoScarto);
                    dg_fasc.Columns[9].Visible = true;
                    lbl_stato.Text = "Aperta";
                    ddl_operazioni.Items.Clear();
                    //ddl_operazioni.Items.Add(new ListItem("Chiudi"));
                    this.ddl_operazioni.Items.AddRange(this.GetListItemsOperazioni("A"));
                    //Valore pulsante esegui
                    valore = 60;
                    btn_esegui.Width = Convert.ToUInt16(valore);
                    btn_esegui.Text = "Chiudi";
                    //pulsanti o etichetti necessari per l'operazione in corso
                    report.Visible = false;
                    caricaDGFrameSX();
                }
                return;
            }
            if (lbl_stato.Text == "Richiesta inoltrata" && ddl_operazioni.SelectedItem.Text == "Autorizzazione negata")
            {
                infoScarto.stato = "C";

                //webmethod per cambiare lo stato dell'istanza di scarto
                if (FascicoliManager.cambiaStatoScarto(this, infoScarto, UserManager.getInfoUtente(), ""))
                {
                    FascicoliManager.setIstanzaScarto(this, infoScarto);
                    dg_fasc.Columns[9].Visible = false;
                    report.Visible = true;
                    lbl_stato.Text = "Chiusa";
                    ddl_operazioni.Items.Clear();
                    this.ddl_operazioni.Items.AddRange(this.GetListItemsOperazioni("C"));
                    //ddl_operazioni.Items.Add(new ListItem("Registra estremi richiesta autorizzazione"));
                    //ddl_operazioni.Items.Add(new ListItem("Riapri"));
                    //valore pulsante esegui
                    valore = 160;
                    btn_esegui.Width = Convert.ToUInt16(valore);
                    btn_esegui.Text = "Registra autorizzazione";
                    //pulsanti o etichetti necessari per l'operazione in corso
                    richAut.Visible = true;
                    caricaDGFrameSX();
                }
                return;

            }
            if (lbl_stato.Text == "Richiesta inoltrata" && ddl_operazioni.SelectedItem.Text == "Registra estremi provvedimento autorizzazione")
            {
                infoScarto.stato = "RA";
                if (!string.IsNullOrEmpty(txt_autor.Text))
                {
                    infoScarto.estremi_autorizzazione = txt_autor.Text;
                    //webmethod per cambiare lo stato dell'istanza di scarto
                    if (FascicoliManager.cambiaStatoScarto(this, infoScarto, UserManager.getInfoUtente(), "estremi_autorizzazione"))
                    {
                        FascicoliManager.setIstanzaScarto(this, infoScarto);
                        dg_fasc.Columns[9].Visible = false;
                        //Reimposta lo stato e l'operazione successiva
                        lbl_stato.Text = "Richiesta autorizzata";
                        ddl_operazioni.Items.Clear();
                        this.ddl_operazioni.Items.AddRange(this.GetListItemsOperazioni("RA"));

                        
                        //ddl_operazioni.Items.Add(new ListItem("Scarta"));
                        regAut.Visible = false;
                        report.Visible = false;
                        ric_aut_lbl.Visible = true;
                        lbl_estr_richiesta.Text = infoScarto.estremi_richiesta;
                        autor_lbl.Visible = true;
                        lbl_estr_autorizzazione.Text = infoScarto.estremi_autorizzazione;
                        valore = 60;
                        btn_esegui.Width = Convert.ToUInt16(valore);
                        btn_esegui.Text = "Scarta";
                        caricaDGFrameSX();
                    }
                    return;
                }
                else
                {
                    Response.Write("<script>alert('Attenzione campo Estremi del provvedimento di autorizzazione.')</script>");
                    return;
                }
            }
            if (lbl_stato.Text == "Richiesta autorizzata" && ddl_operazioni.SelectedItem.Text == "Scarta")
            {
                string messaggio = InitMessageXml.getInstance().getMessage("MSG_SCARTO");
                if (messaggio != "")
                {
                    MsgBox_Scarta.Confirm(messaggio);
                }
            }
        }

        private ListItem[] GetListItemsOperazioni(string stato)
        {
            ArrayList items = new ArrayList();
            switch (stato)
            {
                case "A":
                    items.Add(new ListItem("Chiudi"));
                    break;
                case "C":
                    items.Add(new ListItem("Registra estremi richiesta autorizzazione"));
                    items.Add(new ListItem("Riapri"));
                    break;
                case "RI":
                    items.Add(new ListItem("Registra estremi provvedimento autorizzazione"));
                    items.Add(new ListItem("Autorizzazione negata"));
                    break;
                case "RA":
                    items.Add(new ListItem("Scarta"));
                    break;
            }
            ListItem[] retValue = new ListItem[items.Count];
            items.CopyTo(retValue);
            return retValue;
        }

        private void MsgBox_Scarta_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                scarta();
            }
        }

        private void scarta() 
        {
            infoScarto = FascicoliManager.getIstanzaScarto(this);
            //effettua scarto
            infoScarto.stato = "S";
            if (FascicoliManager.cambiaStatoScarto(this, infoScarto, UserManager.getInfoUtente(), ""))
            {
                FascicoliManager.setIstanzaScarto(this, infoScarto);
                //disabilito tutto
                dg_fasc.Columns[9].Visible = false;
                lbl_stato.Text = "Scartata";
                dataScarto.Visible = true;
                lbl_data_scarto.Text = System.DateTime.Today.Date.ToShortDateString();
                op.Visible = false;
                ddl_operazioni.Visible = false;
                btn_esegui.Visible = false;
                btn_salva.Visible = false;
                txt_note.Enabled = false;
                caricaDGFrameSX();
            }
            return;
        }

        private void caricaDGFrameSX()
        {
            Session["operazione"] = "istanza";
            string url = "OpzioniScarto.aspx";
            string funct_dx2 = "top.principale.frames[0].location='" + url + "'";
            this.Page.Response.Write("<script> " + funct_dx2 + "</script>");
        }
        #endregion

        #region pulsanti
        private void btn_salva_Click(object sender, System.EventArgs e)
        {
            infoScarto = new DocsPAWA.DocsPaWR.InfoScarto();
            infoScarto = FascicoliManager.getIstanzaScarto(this);
            if (this.btn_salva.Text == "Salva")
            {
                if (!string.IsNullOrEmpty(txt_descrizione.Text))
                {
                    infoScarto.descrizione = txt_descrizione.Text;
                    infoScarto.note = txt_note.Text;

                    if (FascicoliManager.updateAreaScarto(Page, infoScarto))
                    {
                        infoScarto.stato = "A";
                        FascicoliManager.setIstanzaScarto(this, infoScarto);
                        btn_salva.Text = "Modifica";
                        lbl_descrizione.Text = "Descrizione";
                        txt_descrizione.Enabled = false;
                        pnl_statiOp.Visible = true;
                        impostaStatoOp();
                    }
                }
                else
                {
                    Response.Write("<script>alert('Attenzione campo descrizione obbligatorio.')</script>");
                    return;
                }
            }
            else
            {
                infoScarto.note = txt_note.Text;
                FascicoliManager.setIstanzaScarto(this, infoScarto);
                FascicoliManager.updateAreaScarto(Page, infoScarto);
            }
        }

        private void btn_report_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //this.btn_report.Attributes.Add("onclick", "StampaRisultatoRicerca();");
            RegisterStartupScript("generaReport", "<script>StampaRisultatoRicerca();</script>");
        }
        #endregion

        //private void AttatchGridPagingWaitControl()
        //{
        //    DataGridPagingWaitControl.DataGridID = this.dg_fasc.ClientID;
        //    DataGridPagingWaitControl.WaitScriptCallback = "WaitDataGridCallback(eventTarget,eventArgument);";
        //}

        //private waiting.DataGridPagingWait DataGridPagingWaitControl
        //{
        //    get
        //    {
        //        return this.FindControl("DataGridPagingWait1") as waiting.DataGridPagingWait;
        //    }
        //}

        

        public class Cols
        {
            private string stato;
            private string tipo;
            private string codClass;
            private string codice;
            private string descrizione;
            //private string apertura;
            private string mesiCons;
            private string mesiChiusura;
            private string chiusura;
            private string inScarto;
            private int chiave;

            public Cols(string stato, string tipo, string codClass, string codice, string descrizione, string chiusura, string mesiCons, string mesiChiusura, string inScarto, int chiave)
            {
                this.stato = stato;
                this.tipo = tipo;
                this.codClass = codClass;
                this.codice = codice;
                this.descrizione = descrizione;
                this.mesiCons = mesiCons;
                this.mesiChiusura = mesiChiusura;
                this.chiusura = chiusura;
                this.inScarto = inScarto;
                this.chiave = chiave;
            }

            public string Stato { get { return stato; } }
            public string Tipo { get { return tipo; } }
            public string CodClass { get { return codClass; } }
            public string Codice { get { return codice; } }
            public string Descrizione { get { return descrizione; } }
            //public string Apertura { get { return apertura; } }
            public string MesiCons { get { return mesiCons; } }
            public string MesiChiusura { get { return mesiChiusura; } }
            public string Chiusura { get { return chiusura; } }
            public string InScarto { get { return inScarto; } }
            public int Chiave { get { return chiave; } }
        }
    }
}
