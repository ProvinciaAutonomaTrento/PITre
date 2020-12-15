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
using SAAdminTool;

namespace SAAdminTool.AdminTool.Gestione_Utenti
{
    public partial class VisualizzaMenuUtente : System.Web.UI.Page
    {
        //protected System.Web.UI.WebControls.DataGrid dg_menu;
        //protected System.Web.UI.WebControls.Button btn_mod_menu;
        //protected System.Web.UI.WebControls.Label titolo;
        //protected System.Web.UI.WebControls.Button btn_chiudi;
        string idAmm;
        string idCorrGlob;
        protected DataSet dsMenu;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.MaintainScrollPositionOnPostBack = true;

            Response.Expires = -1;

            try
            {
                idCorrGlob = this.Request.QueryString["idCorrGlob"];
                idAmm = this.Request.QueryString["idAmm"];
                //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
                if (Session.IsNewSession)
                {
                    Response.Redirect("../Exit.aspx?FROM=EXPIRED");
                }

                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                if (!ws.CheckSession(Session.SessionID))
                {
                    Response.Redirect("../Exit.aspx?FROM=ABORT");
                }
                // ---------------------------------------------------------------			

                if (!IsPostBack)
                {
                    this.btn_chiudi.Attributes.Add("onclick", "window.close();");
                    this.Inizialize();
                }
            }
            catch
            {
                this.GUI("Errore");
            }
        }


        private void GUI(string key)
        {
            switch (key)
            {
                case "Errore":
                    this.titolo.Text = "Si è verificato un errore!";
                    this.dg_menu.Visible = false;
                    this.btn_mod_menu.Visible = false;
                    this.btn_chiudi.Visible = true;
                    break;
                case "NoDataFound":
                    this.titolo.Text = "Nessuna voce di menù disponibile!";
                    this.dg_menu.Visible = false;
                    this.btn_mod_menu.Visible = false;
                    this.btn_chiudi.Visible = true;
                    break;
                case "Ok":
                    this.titolo.Text = "Lista delle voci di menù:";
                    this.dg_menu.Visible = true;
                    this.btn_mod_menu.Visible = true;
                    this.btn_chiudi.Visible = true;
                    break;
            }
        }

        private void Inizialize()
        {
           

            this.GetListaMenu(idAmm, idCorrGlob);
        }

        private bool isMenuVisibile(string voceMenu)
        {
            bool visibile = false;
            if (voceMenu == null || voceMenu.Equals(""))
                return true;
            if (System.Configuration.ConfigurationManager.AppSettings[voceMenu] != null && System.Configuration.ConfigurationManager.AppSettings[voceMenu] == "1" )
                return true;
            return visibile;
        }

        private void GetListaMenu(string idAmm, string idCorrGlob)
        {
            try
            {
                Amministrazione.Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                theManager.MenuUtente(idCorrGlob, idAmm);
                if (theManager.getMenuUtente() != null && theManager.getMenuUtente().Count > 0)
                {
                    /* Andrea De Marco - Recupero utente per verificare i diritti di Amministratore:
                     * 1: System Administrator
                     * 0: No Administrator
                     * 2: Super Administrator
                     * 3: User Administrator
                     */
                    DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                    SAAdminTool.DocsPaWR.OrgUtente utente = ws.AmmGetDatiUtente(idCorrGlob);
                    //End Andrea De Marco
                    InitializeDataSetMenu();
                    DataRow row;
                    int i=1;
                    foreach (SAAdminTool.DocsPaWR.Menu VoceMenu in theManager.getMenuUtente())
                    {
                        //Andrea De Marco - Solo in caso di System Administrator deve essere visibile la Gestione Chiavi di Configurazione.
                        if (utente != null && !string.IsNullOrEmpty(utente.Amministratore) && !utente.Amministratore.Equals("1"))
                        {
                            if ((!string.IsNullOrEmpty(VoceMenu.Codice) && VoceMenu.Codice.Equals("Gestione Chiavi Config")) ||
                                (!string.IsNullOrEmpty(VoceMenu.IDMenu) && VoceMenu.IDMenu.Equals("24")))
                                continue;
                        }
                        //End Andrea De Marco
                        //controllo il valore delle voci di menù che non devono essere viste
                        if (isMenuVisibile(VoceMenu.Visibilita))
                        {
                            row = dsMenu.Tables[0].NewRow();
                            row["IDMenu"] = VoceMenu.IDMenu;
                            row["Codice"] = VoceMenu.Codice;
                            row["Descrizione"] = VoceMenu.Descrizione;
                            row["IDAmministrazione"] = idAmm;
                            row["IDCorrGlob"] = idCorrGlob;
                            if (VoceMenu.Associato != null && VoceMenu.Associato != String.Empty)
                            {
                                row["Sel"] = "true";
                            }
                            else
                            {
                                row["Sel"] = "false";
                            }
                            dsMenu.Tables["MENU"].Rows.Add(row);
                            i++;
                        }
                    }

                    DataView dv = dsMenu.Tables["MENU"].DefaultView;
                    //dv.Sort = "Descrizione ASC";
                    this.dg_menu.DataSource = dv;
                    this.dg_menu.DataBind();

                    this.GUI("Ok");
                }
                else
                {
                    this.GUI("NoDataFound");
                }
            }
            catch
            {
                this.GUI("Errore");
            }
        }

        private void InitializeDataSetMenu()
        {
            dsMenu = new DataSet();
            DataColumn dc;
            dsMenu.Tables.Add("MENU");
            dc = new DataColumn("IDMenu");
            dsMenu.Tables["MENU"].Columns.Add(dc);
            dc = new DataColumn("Codice");
            dsMenu.Tables["MENU"].Columns.Add(dc);
            dc = new DataColumn("Descrizione");
            dsMenu.Tables["MENU"].Columns.Add(dc);
            dc = new DataColumn("IDAmministrazione");
            dsMenu.Tables["MENU"].Columns.Add(dc);
            dc = new DataColumn("IDCorrGlob");
            dsMenu.Tables["MENU"].Columns.Add(dc);
            dc = new DataColumn("Btn");
            dsMenu.Tables["MENU"].Columns.Add(dc);
            dc = new DataColumn("Sel");
            dsMenu.Tables["MENU"].Columns.Add(dc);
        }


        private void InserimentoMenu()
        {
            string idCorrGlob = string.Empty;
            try
            {
                if (this.dg_menu.Items.Count > 0)
                {
                    CheckBox spunta;
                    SAAdminTool.DocsPaWR.Menu menu = null;
                    ArrayList listaMenuSelezionati = new ArrayList();
                    string idAmm = this.Request.QueryString["idAmm"];
                    idCorrGlob = this.Request.QueryString["idCorrGlob"];

                    for (int i = 0; i < this.dg_menu.Items.Count; i++)
                    {
                        spunta = (CheckBox)dg_menu.Items[i].Cells[6].FindControl("Chk_menu");

                        if (spunta.Checked)
                        {
                            menu = new SAAdminTool.DocsPaWR.Menu();
                            menu.IDMenu = dg_menu.Items[i].Cells[0].Text;
                            menu.Codice = dg_menu.Items[i].Cells[1].Text;
                            menu.Descrizione = dg_menu.Items[i].Cells[2].Text;

                            if (menu.Codice.ToUpper().Equals("TITOLARIO"))
                            {
                                Amministrazione.Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                                if (!theManager.EsistonoRegistriAssociati(idCorrGlob))
                                {
                                    string scriptString = "<SCRIPT>alert('Attenzione, è necessario associare almeno un registro alla voce di menù Titolario');</SCRIPT>";
                                    this.Page.RegisterStartupScript("alertJavaScript", scriptString);
                                    
                                    spunta.Checked=false;
                                    ImageButton img_reg = dg_menu.Items[i].Cells[5].FindControl("dg_btn_registri") as ImageButton;
                                    img_reg.Visible = false;
                                    return;
                                }
                            }

                            listaMenuSelezionati.Add(menu);
                           
                            menu = null;
                            idCorrGlob = dg_menu.Items[i].Cells[4].Text;
                        }
                    }

                    if (listaMenuSelezionati != null && listaMenuSelezionati.Count > 0)
                    {
                        SAAdminTool.DocsPaWR.Menu[] vociMenu = new SAAdminTool.DocsPaWR.Menu[listaMenuSelezionati.Count];
                        listaMenuSelezionati.CopyTo(vociMenu);
                        listaMenuSelezionati = null;

                        Amministrazione.Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                        theManager.InsMenuUtente(vociMenu, idCorrGlob, idAmm);  //attenzione id_amm

                        SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
                        esito = theManager.getEsitoOperazione();

                        if (esito.Codice.Equals(0))
                        {
                            if (!this.Page.IsStartupScriptRegistered("closeJavaScript"))
                            {
                                string scriptString = "<SCRIPT>window.close();</SCRIPT>";
                                this.Page.RegisterStartupScript("closeJavaScript", scriptString);
                            }
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
                    else
                    {
                        //gestione cancellazione dati

                        Amministrazione.Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                        theManager.EliminaMenuUtente(idCorrGlob);
                        
                        SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
                        esito = theManager.getEsitoOperazione();

                        if (esito.Codice.Equals(0))
                        {
                            theManager.EliminaRegistriUtente(idCorrGlob);
                            if (!this.Page.IsStartupScriptRegistered("execJavaScript"))
                            {
                                string scriptString = "<SCRIPT>window.returnValue = 'noAmmTitolario'; window.close();</SCRIPT>";
                                this.Page.RegisterStartupScript("execJavaScript", scriptString);
                            }
                        }
                        else
                        {
                            if (!this.Page.IsStartupScriptRegistered("execJavaScript"))
                            {
                                string scriptString = "<SCRIPT>window.returnValue = 'noAmmTitolario'; window.close();</SCRIPT>";
                                this.Page.RegisterStartupScript("execJavaScript", scriptString);
                            }
                        }
                    }
                }
            }
            catch
            {
                this.GUI("Errore");
            }
        }

 

        protected void btn_mod_menu_Click(object sender, EventArgs e)
        {
            //check presenza dei registri quando è attivata la voce di menù 'titolario'
            this.InserimentoMenu();
        }

        protected void Chk_menu_CheckedChanged(object sender, EventArgs e)
        {
           string voceCheched = ((DataGridItem)(((CheckBox)sender).NamingContainer)).Cells[1].Text;
           ImageButton imgReg = (((DataGridItem)(((CheckBox)sender).NamingContainer)).Cells[5].FindControl("dg_btn_registri")) as ImageButton;
           if (voceCheched.ToUpper() == "TITOLARIO")
           { 
               
               //METTI QUI IL TUO CODICE
               if (((CheckBox)sender).Checked) //CASO 1: il check relativo al Titolario è selezionato
               {
                   imgReg.Visible = true;
                   this.Page.RegisterStartupScript("aprifinistra","<script>ApriFinestraRegistri('" + idCorrGlob + "','" + idAmm + "');</script>");

               }
               else  //CASO 2: il check relativo al Titolario NON è selezionato
               {
                   imgReg.Visible = false;
                   
               }
           }
        }

        protected void dg_menu_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void dg_menu_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            for (int i = 0; i < this.dg_menu.Items.Count; i++)
            {
                if (dg_menu.Items[i].Cells[1].Text.ToUpper().Equals("TITOLARIO"))
                {
                    CheckBox spunta = (CheckBox)dg_menu.Items[i].Cells[6].FindControl("Chk_menu");
                    if (spunta.Checked)
                    {
                        ImageButton img_reg = dg_menu.Items[i].Cells[5].FindControl("dg_btn_registri") as ImageButton;
                        img_reg.Visible = true;
                        img_reg.Attributes.Add("onClick", "ApriFinestraRegistri('" + idCorrGlob + "','" + idAmm + "');");
                    }
                }
            }
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            Amministrazione.Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
            theManager.MenuUtente(idCorrGlob,idAmm);
            bool isTitolario=false;
            foreach (SAAdminTool.DocsPaWR.Menu VoceMenu in theManager.getMenuUtente())
            {
                if (VoceMenu.Codice.ToUpper().Equals("TITOLARIO"))
                {
                    if (VoceMenu.Associato != null && VoceMenu.Associato != String.Empty)
                        isTitolario = true;
                }
            }
            if (!isTitolario)
                theManager.EliminaRegistriUtente(idCorrGlob);
        }

       
    }
}
