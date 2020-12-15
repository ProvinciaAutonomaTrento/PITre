using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using DocsPAWA.ricercaDoc;

namespace DocsPAWA.popup
{
    public partial class modificaRicerca : System.Web.UI.Page
    {

        private string idRicercaSalvata;
        private SchedaRicerca schedaRicerca = null;
        protected bool showGridPersonalization;

        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);

            object obj = null;
            if ((obj = Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY]) != null)
            {
                schedaRicerca = (DocsPAWA.ricercaDoc.SchedaRicerca)obj;
            }
            if ((Request.QueryString["idRicerca"] != null))
            {
                this.idRicercaSalvata = Request.QueryString["idRicerca"];
            }

            //Pannello associazione griglie custom
            DocsPAWA.DocsPaWR.Funzione[] functions;
            functions = UserManager.getRuolo(this.Page).funzioni;
            this.showGridPersonalization = functions.Where(g => g.codice.Equals("GRID_PERSONALIZATION")).Count() > 0;

            if (!IsPostBack)
            {
                loadFields();
            }


        }

        private void InitializeComponent()
        {
            this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
            this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.rbl_share.SelectedIndexChanged += new System.EventHandler(this.rbl_share_SelectedIndexChanged);
        }

        protected void loadFields()
        {
            InfoUtente infoUtente = UserManager.getInfoUtente(this);
            DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
            DocsPAWA.DocsPaWR.SearchItem itemOld = docspaws.RecuperaRicerca(Int32.Parse(idRicercaSalvata));
            txt_titolo.Text = itemOld.descrizione;
            schedaRicerca.Tipo = itemOld.tipo;
            if (!string.IsNullOrEmpty(itemOld.owner_idPeople.ToString()))
            {
                this.rbl_share.SelectedValue = "usr";
            }
            else
            {
                this.rbl_share.SelectedValue = "grp";
            }

            DocsPAWA.DocsPaWR.Utente userHome = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
            DocsPAWA.DocsPaWR.Ruolo userRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];

            rbl_share.Items[0].Text = rbl_share.Items[0].Text.Replace("@usr@", userHome.descrizione);
            rbl_share.Items[1].Text = rbl_share.Items[1].Text.Replace("@grp@", userRuolo.descrizione);

            if (schedaRicerca.ProprietaNuovaRicerca.Condivisione == SchedaRicerca.NuovaRicerca.ModoCondivisione.Utente)
            {
                rbl_share.Items[0].Selected = true;
                rbl_share.Items[1].Selected = false;
            }
            else
            {
                rbl_share.Items[0].Selected = false;
                rbl_share.Items[1].Selected = true;
            }

            this.pnl_griglie_custom.Visible = this.showGridPersonalization;
            if (!IsPostBack && this.showGridPersonalization)
            {
                this.ddl_ric_griglie.Items.Clear();
                //Vuol dire c'è una griglia temporanea
                if (GridManager.SelectedGrid != null && string.IsNullOrEmpty(GridManager.SelectedGrid.GridId))
                {
                    ListItem it = new ListItem("Griglia temporanea", "-2");
                    this.ddl_ric_griglie.Items.Add(it);
                }

                string visibility = rbl_share.SelectedValue;
                bool allGrids = true;

                if (visibility.Equals("grp"))
                {
                    allGrids = false;
                }

                GridBaseInfo[] listGrid = GridManager.GetGridsBaseInfo(infoUtente, GridManager.SelectedGrid.GridType, allGrids);

                Dictionary<string, GridBaseInfo> tempIdGrid = new Dictionary<string, GridBaseInfo>();

                if (listGrid != null && listGrid.Length > 0)
                {
                    foreach (GridBaseInfo gb in listGrid)
                    {
                        ListItem it = new ListItem(gb.GridName, gb.GridId);
                        this.ddl_ric_griglie.Items.Add(it);
                        tempIdGrid.Add(gb.GridId, gb);
                    }
                    if (!string.IsNullOrEmpty(schedaRicerca.gridId) && tempIdGrid != null && tempIdGrid.Count > 0)
                    {
                        if (tempIdGrid.ContainsKey(schedaRicerca.gridId))
                        {
                            this.ddl_ric_griglie.SelectedValue = schedaRicerca.gridId;
                        }
                    }
                }
            }
        }

        private void btn_annulla_Click(object sender, System.EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chiudi", "window.self.close();", true);
        }

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
                if (schedaRicerca != null && schedaRicerca.ProprietaNuovaRicerca != null)
                {
                    schedaRicerca.ProprietaNuovaRicerca.Id = Int32.Parse(this.idRicercaSalvata);

                    schedaRicerca.ProprietaNuovaRicerca.Titolo = txt_titolo.Text;
                    if (rbl_share.Items[0].Selected)
                        schedaRicerca.ProprietaNuovaRicerca.Condivisione = DocsPAWA.ricercaDoc.SchedaRicerca.NuovaRicerca.ModoCondivisione.Utente;
                    else if (rbl_share.Items[1].Selected)
                        schedaRicerca.ProprietaNuovaRicerca.Condivisione = DocsPAWA.ricercaDoc.SchedaRicerca.NuovaRicerca.ModoCondivisione.Ruolo;

                    try
                    {
                        string msg = string.Empty;
                        string pagina = string.Empty;
                        string _searchKey = schedaRicerca.getSearchKey();
                        System.Web.UI.Page currentPg = schedaRicerca.Pagina;

                        if (string.IsNullOrEmpty(_searchKey))
                            pagina = currentPg.ToString();
                        else
                            pagina = _searchKey;

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

                        if (!schedaRicerca.verificaNomeModifica(txt_titolo.Text, infoUtente, pagina, this.idRicercaSalvata))
                        {
                            schedaRicerca.Modifica(null, this.showGridPersonalization, idGriglia, tempGrid, GridManager.SelectedGrid.GridType);
                            msg = "Il criterio di ricerca è stato modificato con nome '" + txt_titolo.Text + "'";
                            msg = msg.Replace("\"", "\\\"");
                            DropDownList ddl = (DropDownList)schedaRicerca.Pagina.FindControl("ddl_Ric_Salvate");
                            Session["idRicSalvata"] = schedaRicerca.ProprietaNuovaRicerca.Id.ToString();
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

        protected void CloseAndPostback(string message)
        {
            string alertScript = "";
            string refreshScript = "window.dialogArguments.location.href = window.dialogArguments.location.href;";

            if (message != null)
                alertScript = "alert(\"" + message + "\");";

            string script = alertScript + refreshScript + "window.self.close();";
            ClientScript.RegisterStartupScript(this.GetType(), "alertMessage", script, true);
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