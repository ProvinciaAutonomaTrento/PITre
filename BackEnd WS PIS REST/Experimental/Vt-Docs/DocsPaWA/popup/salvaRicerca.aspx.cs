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
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{
    /// <summary>
    /// Summary description for salvaRicerca.
    /// </summary>
    public class salvaRicerca : basePopup
    {
        protected System.Web.UI.WebControls.Button btn_annulla;
        protected System.Web.UI.WebControls.Button btn_salva;
        protected System.Web.UI.WebControls.TextBox txt_titolo;
        protected System.Web.UI.WebControls.Panel pnl_uff_ref;
        protected System.Web.UI.WebControls.Label lblTitle;
        protected Utilities.MessageBox MessageBox1;
        protected System.Web.UI.WebControls.RadioButtonList rbl_share;
        protected System.Web.UI.WebControls.DropDownList ddl_ric_griglie;

        private DocsPAWA.ricercaDoc.SchedaRicerca schedaRicerca = null;

        protected System.Web.UI.WebControls.Panel pnl_griglie_custom;

        protected bool showGridPersonalization;

        public salvaRicerca()
        {
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            Utils.startUp(this);

            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente(this);
                object obj = null;
                if ((obj = Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY]) != null)
                {
                    schedaRicerca = (DocsPAWA.ricercaDoc.SchedaRicerca)obj;
                    if (!IsPostBack)
                    {
                        //adl
                        if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                        {
                            lblTitle.Text = "Salvataggio della ricerca in Area di Lavoro";
                            rbl_share.Items[0].Text = rbl_share.Items[0].Text.Replace("@usr@", schedaRicerca.Utente.descrizione);
                            rbl_share.Items[1].Text = rbl_share.Items[1].Text.Replace("@grp@", schedaRicerca.Ruolo.descrizione);
                            rbl_share.Items[1].Enabled = false;
                        }
                        else
                        {
                            rbl_share.Items[0].Text = rbl_share.Items[0].Text.Replace("@usr@", schedaRicerca.Utente.descrizione);
                            rbl_share.Items[1].Text = rbl_share.Items[1].Text.Replace("@grp@", schedaRicerca.Ruolo.descrizione);
                        }
                    }
                }
                //Pannello associazione griglie custom
                DocsPAWA.DocsPaWR.Funzione[] functions;
                functions = UserManager.getRuolo(this.Page).funzioni;
                this.showGridPersonalization = functions.Where(g => g.codice.Equals("GRID_PERSONALIZATION")).Count() > 0;
                this.pnl_griglie_custom.Visible = this.showGridPersonalization;
                if (!IsPostBack && this.showGridPersonalization)
                {
                    string visibility = rbl_share.SelectedValue;

                    bool allGrids = true;
                    
                    //Vuol dire c'è una griglia temporanea
                    if (GridManager.SelectedGrid != null && string.IsNullOrEmpty(GridManager.SelectedGrid.GridId))
                    {
                        ListItem it = new ListItem("Griglia temporanea", "-2");
                        this.ddl_ric_griglie.Items.Add(it);
                    }

                    if (visibility.Equals("grp"))
                    {
                        allGrids = false;
                    }

                    GridBaseInfo[] listGrid = GridManager.GetGridsBaseInfo(infoUtente, GridManager.SelectedGrid.GridType, allGrids);

                    if (listGrid != null && listGrid.Length > 0)
                    {
                        foreach (GridBaseInfo gb in listGrid)
                        {
                            ListItem it = new ListItem(gb.GridName, gb.GridId);
                            this.ddl_ric_griglie.Items.Add(it);
                        }
                    }
                }

            }
            catch (Exception) { }
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
            this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
            this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
            this.rbl_share.SelectedIndexChanged += new System.EventHandler(this.rbl_share_SelectedIndexChanged);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        private void btn_salva_Click(object sender, System.EventArgs e)
        {
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = new DocsPAWA.DocsPaWR.InfoUtente();
            infoUtente = UserManager.getInfoUtente(this.Page);

            if (txt_titolo.Text == null || txt_titolo.Text == "")
            {
                MessageBox1.Alert("Indicare un nome per il salvataggio del criterio di ricerca");
            }
            else
            {
                schedaRicerca.Tipo = Request.QueryString["tipo"].ToString();

                if (schedaRicerca != null && schedaRicerca.ProprietaNuovaRicerca != null)
                {
                    schedaRicerca.ProprietaNuovaRicerca.Titolo = txt_titolo.Text;
                    if (rbl_share.Items[0].Selected)
                        schedaRicerca.ProprietaNuovaRicerca.Condivisione = DocsPAWA.ricercaDoc.SchedaRicerca.NuovaRicerca.ModoCondivisione.Utente;
                    else if (rbl_share.Items[1].Selected)
                        schedaRicerca.ProprietaNuovaRicerca.Condivisione = DocsPAWA.ricercaDoc.SchedaRicerca.NuovaRicerca.ModoCondivisione.Ruolo;

                    try
                    {
                        //Verifico se salvare anche la griglia
                        string idGriglia = string.Empty;
                        Grid tempGrid = new Grid();
                        if (ddl_ric_griglie.Visible)
                        {
                            idGriglia = ddl_ric_griglie.SelectedValue;
                            if (!string.IsNullOrEmpty(idGriglia) && idGriglia.Equals("-2"))
                            {
                                tempGrid = GridManager.CloneGrid(GridManager.SelectedGrid);
                            }
                        }

                        string msg = string.Empty;
                        string pagina = string.Empty;
                        string _searchKey = schedaRicerca.getSearchKey();
                        System.Web.UI.Page currentPg = schedaRicerca.Pagina;
                        string adl = "0";
                        if (string.IsNullOrEmpty(_searchKey))
                            pagina = currentPg.ToString();
                        else
                            pagina = _searchKey;

                        if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                        {
                            adl = "1";
                        }

                        if (!schedaRicerca.verificaNome(txt_titolo.Text, infoUtente, pagina, adl))
                        {
                            if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                            {
                                schedaRicerca.Salva("ricADL", this.showGridPersonalization, idGriglia, tempGrid, GridManager.SelectedGrid.GridType);
                            }
                            else
                            {
                                schedaRicerca.Salva(null, this.showGridPersonalization, idGriglia, tempGrid, GridManager.SelectedGrid.GridType);
                            }

                            msg = "Il criterio di ricerca è stato salvato con nome '" + txt_titolo.Text + "'";
                            msg = msg.Replace("\"", "\\\"");
                            DropDownList ddl = (DropDownList)schedaRicerca.Pagina.FindControl("ddl_Ric_Salvate");
                           // ddl.SelectedValue = schedaRicerca.ProprietaNuovaRicerca.Id.ToString();
                            Session["idRicSalvata"]=schedaRicerca.ProprietaNuovaRicerca.Id.ToString();
                            schedaRicerca.ProprietaNuovaRicerca = null;
                        }
                        else
                        {
                            msg = "Esiste già una ricerca salvata con questo nome!";
                        }

                        CloseAndPostback(msg);
                    }
                    catch (Exception ex)
                    {
                        MessageBox1.Alert("Errore nel salvataggio della ricerca. Verificare i dati inseriti.");
                    }
                }
            }
        }

        private void btn_annulla_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        protected void rbl_share_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.pnl_griglie_custom.Visible)
            {
                string idGrigliaOld = this.ddl_ric_griglie.SelectedValue;
                this.ddl_ric_griglie.Items.Clear();
                string visibility = rbl_share.SelectedValue;
                InfoUtente infoUtente = UserManager.getInfoUtente(this);

                bool allGrids = true;

                //Vuol dire c'è una griglia temporanea
                if (GridManager.SelectedGrid != null && string.IsNullOrEmpty(GridManager.SelectedGrid.GridId))
                {
                    ListItem it = new ListItem("Griglia temporanea", "-2");
                    this.ddl_ric_griglie.Items.Add(it);
                }

                if (visibility.Equals("grp"))
                {
                    allGrids = false;
                }

                GridBaseInfo[] listGrid = GridManager.GetGridsBaseInfo(infoUtente, GridManager.SelectedGrid.GridType, allGrids);

                bool present = false;
                if (listGrid != null && listGrid.Length > 0)
                {
                    foreach (GridBaseInfo gb in listGrid)
                    {
                        ListItem it = new ListItem(gb.GridName, gb.GridId);
                        this.ddl_ric_griglie.Items.Add(it);
                        if (gb.GridId.Equals(idGrigliaOld))
                        {
                            present = true;
                        }
                    }
                    if (present)
                    {
                        this.ddl_ric_griglie.SelectedValue = idGrigliaOld;
                    }
                }
            }
        }
    }
}
