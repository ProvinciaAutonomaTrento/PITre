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
using System.Configuration;
using System.Collections.Generic;


namespace DocsPAWA.popup
{
    /// <summary>
    /// Summary description for visibilitaDocumento.
    /// </summary>
    public class visibilitaFascicolo : DocsPAWA.CssPage
    {
        protected System.Web.UI.WebControls.DataGrid DataGrid1;
        protected DocsPAWA.DocsPaWR.InfoFascicolo InfoFasc;
        protected DocsPAWA.DocsPaWR.Fascicolo fasc;
        protected DocsPAWA.DocsPaWR.FascicoloDiritto[] ListaFascDir;
        protected System.Web.UI.WebControls.Button btn_ok;
        protected System.Web.UI.WebControls.Label lb_dettagli;
        protected System.Web.UI.WebControls.Label Label2;
        protected DocsPAWA.dataSet.DS_Visibilit dS_Visibilit1;
        protected string dirittiUser;
        protected ArrayList Dt_elem;
        protected Utilities.MessageBox Msg_Rimuovi;

        protected System.Web.UI.WebControls.Panel pnl_descRimuovi;
        protected System.Web.UI.WebControls.TextBox txt_note;
        protected System.Web.UI.WebControls.Button btn_okDesc;
        protected System.Web.UI.WebControls.Button btn_annulla;

        protected string _idVisFasc = String.Empty;
        protected bool _isACLauthorized;
        protected DocsPaWebCtrlLibrary.ImageButton btn_storia_visibilita;
        protected System.Web.UI.WebControls.Label lbl_atipicita;

        //
        // Mev Editing ACL
        protected string check_all = string.Empty;
        protected System.Web.UI.WebControls.CheckBox CheckBoxSelezionaTutti;
        protected DocsPaWebCtrlLibrary.ImageButton CestinoMassiva;
        protected DocsPaWebCtrlLibrary.ImageButton RipristinoMassiva_;
        //protected DocsPAWA.DocsPaWR.DocumentoDiritto[] listaElementiDocDir;
        protected string rimozioneMassiva = string.Empty;
        protected Utilities.MessageBox Msg_Ripristina;
        protected string ripristinoMassiva = string.Empty;
        // End MEV
        //

        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                lbl_atipicita.Text = String.Empty;
                lbl_atipicita.Visible = false;
            }

            bool cercaRimossi = true;
            Response.Expires = -1;
            if ( ((!IsPostBack) && (_idVisFasc == string.Empty)) || (_idVisFasc == "reload"))
            {
                
                fasc = (DocsPAWA.DocsPaWR.Fascicolo)FascicoliManager.getFascicoloSelezionato(this);
                string rootFolder = FascicoliManager.GetRootFolderFasc(this.Page, fasc);

                if (fasc == null)
                {
                    this.lb_dettagli.Text = "Errore nel reperimento dei dati del fascicolo";
                    this.lb_dettagli.Visible = true;
                    return;
                }

                InfoFasc = FascicoliManager.getInfoFascicoloDaFascicolo(fasc, this);
                
                if (InfoFasc == null)
                {
                    this.lb_dettagli.Text = "Errore nel reperimento dei dati del fascicolo";
                    this.lb_dettagli.Visible = true;
                    return;
                }

                dirittiUser = fasc.dirittoUtente;

               // ListaFascDir = FascicoliManager.getListaVisibilita(this, InfoFasc, cercaRimossi, rootFolder);
                ListaFascDir = FascicoliManager.getListaVisibilitaSemplificata(this, InfoFasc, cercaRimossi, rootFolder);
                Dt_elem = new ArrayList();

                //qui	
                if (ListaFascDir != null && ListaFascDir.Length <= 0)
                {
                    this.lb_dettagli.Text = "Nessun valore trovato";
                    this.lb_dettagli.Visible = true;
                    return;
                }
                string rimosso = "";
                for (int i = 0; i < ListaFascDir.Length; i++)
                {

                    string descrSoggetto = UserManager.getDecrizioneCorrispondente(this, ListaFascDir[i].soggetto);
                    //Dt_elem.Add(new Cols(descrSoggetto, setTipoDiritto(ListaDocDir[i]), ListaDocDir[i].soggetto.codiceRubrica));
                    string Corr = GetTipoCorr(ListaFascDir[i]);
                    if (ListaFascDir[i].deleted)
                        rimosso = "1";
                    else rimosso = "0";
                    this.dS_Visibilit1.element1.Addelement1Row(setTipoDiritto(ListaFascDir[i]), descrSoggetto, ListaFascDir[i].soggetto.codiceRubrica, Corr, ListaFascDir[i].soggetto.dta_fine, rimosso, ListaFascDir[i].note, ListaFascDir[i].personorgroup, ListaFascDir[i].dtaInsSecurity, ListaFascDir[i].noteSecurity, ListaFascDir[i].soggetto.tipoCorrispondente == "R" ? !String.IsNullOrEmpty(((DocsPAWA.DocsPaWR.Ruolo)ListaFascDir[i].soggetto).ShowHistory) && ((DocsPAWA.DocsPaWR.Ruolo)ListaFascDir[i].soggetto).ShowHistory != "0" : false, ListaFascDir[i].soggetto.systemId, this.GetRightDescription(ListaFascDir[i].accessRights));
                }
                Session["ListaFascDir"] = this.ListaFascDir;
                Session["Dg_visibilita"] = this.dS_Visibilit1.Tables[0];
                this.DataGrid1.DataSource = this.dS_Visibilit1.Tables[0];
                this.DataGrid1.DataBind();
            }

            //Verifica atipicita documento
            DocsPaWR.InfoAtipicita infoAtipicita = null;
            if (fasc != null && !string.IsNullOrEmpty(fasc.systemID))
                infoAtipicita = Utils.GetInfoAtipicita(DocsPaWR.TipoOggettoAtipico.FASCICOLO, fasc.systemID);
            if (infoAtipicita != null && infoAtipicita.CodiceAtipicita != "T" && !string.IsNullOrEmpty(infoAtipicita.DescrizioneAtipicita))
            {
                lbl_atipicita.Text = infoAtipicita.DescrizioneAtipicita;
                lbl_atipicita.Visible = true;
            }

            if (infoAtipicita != null && infoAtipicita.CodiceAtipicita == "T" && e != null && e is ModifyACL)
                this.lbl_atipicita.Visible = false;
            
            /*
            //Verifica atipicita fascicolo
            string descrizioneAtipicita = Utils.getDescrizioneAtipicita(fasc, DocsPaWR.TipoOggettoAtipico.FASCICOLO);
            if (!string.IsNullOrEmpty(descrizioneAtipicita))
            {
                lbl_atipicita.Text += descrizioneAtipicita;
                lbl_atipicita.Visible = true;
            }
            */
        }

        /// <summary>
        /// Metodo per la generazione di una descrizione estesa del tipo diritto
        /// </summary>
        /// <param name="accessRight">Diritto di accesso</param>
        /// <returns>Descrizione del tipo di diritto</returns>
        private String GetRightDescription(int accessRight)
        {
            String retVal = String.Empty;

            switch (accessRight)
            {
                case 0:
                case 255:
                case 63:
                    retVal = "Lettura / Scrittura";
                    break;
                case 45:
                case 20:
                    retVal = "Lettura";
                    break;
                default:
                    break;

            }

            return retVal;

        }

        protected string GetTipoCorr(DocsPAWA.DocsPaWR.FascicoloDiritto fascDirit)
        {
            try
            {
                string rtn = "";
                if (fascDirit.soggetto.GetType() == typeof(DocsPAWA.DocsPaWR.Utente))
                    rtn = "UTENTE";
                else if (fascDirit.soggetto.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
                    rtn = "RUOLO";
                else if (fascDirit.soggetto.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                    rtn = "U.O.";
                return rtn;
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
                return "";
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
            this.dS_Visibilit1 = new DocsPAWA.dataSet.DS_Visibilit();
            ((System.ComponentModel.ISupportInitialize)(this.dS_Visibilit1)).BeginInit();
            this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
            this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.Datagrid1_pager);
            this.DataGrid1.PreRender += new System.EventHandler(this.DataGrid1_PreRender);
            this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.Datagrid1_SelectedIndex);
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            this.Msg_Rimuovi.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.Msg_Rimuovi_GetMessageBoxResponse);
            // 
            // dS_Visibilit1
            // 
            this.dS_Visibilit1.DataSetName = "DS_Visibilit";
            this.dS_Visibilit1.Locale = new System.Globalization.CultureInfo("en-US");
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);
           ((System.ComponentModel.ISupportInitialize)(this.dS_Visibilit1)).EndInit();
            this.btn_storia_visibilita.Click += new System.Web.UI.ImageClickEventHandler(this.btn_storia_visibilita_Click);

            // Mev editing ACL
            this.Msg_Ripristina.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.Msg_Ripristina_GetMessageBoxResponse);
            //
            // Inserito per risolvere il problema del check seleziona/deseleziona tutti quando si apre e poi richiude la finestra 
            Session["check_all"] = String.Empty;
            // End editing ACL

            this.btn_okDesc.Click += new System.EventHandler(this.btn_okDesc_Click);
            this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
        }
        #endregion
        
        #region datagrid

        private void DataGrid1_PreRender(object sender, System.EventArgs e)
        {
            //
            // Mev Editing ACL
            // Poichè è stata aggiunta la colonna Checkbox, occorre incrementare gli indici delle celle/colonne
            //

            bool isUtente = false;
            int posPeople = -1;
            DataGrid dg = (DataGrid)sender;
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();

            //Verifica se l'utente è abilitato alla funzione di editing ACL
            this._isACLauthorized = UserManager.ruoloIsAutorized(this, "ACL_RIMUOVI");  

            for (int i = 0; i < dg.Items.Count; i++)
            {
                if (dg.Items[i].ItemIndex >= 0)
                {
                    //Label lbl = (Label)dg.Items[i].Cells[5].Controls[1];
                    Label lbl = (Label)dg.Items[i].Cells[6].Controls[1];

                    //Label dataFine = (Label)dg.Items[i].Cells[11].Controls[1];
                    Label dataFine = (Label)dg.Items[i].Cells[12].Controls[1];
                    if (dataFine.Text != null && dataFine.Text != String.Empty)
                    {
                        dg.Items[i].ForeColor = Color.Gray;
                        dg.Items[i].Font.Bold = false;
                        dg.Items[i].Font.Strikeout = false;
                        //((ImageButton)dg.Items[i].Cells[3].Controls[1]).Visible = false;
                        ((ImageButton)dg.Items[i].Cells[4].Controls[1]).Visible = false;
                    }

                    //Se Acl è revocata allora la riga è di colore rosso e barrata
                    //Label rimosso = (Label)dg.Items[i].Cells[8].Controls[1];
                    Label rimosso = (Label)dg.Items[i].Cells[9].Controls[1];
                    bool ripristina = false;
                    if (rimosso.Text != null && rimosso.Text != String.Empty && rimosso.Text.Equals("1"))
                    {
                        dg.Items[i].ForeColor = Color.Red;
                        dg.Items[i].Font.Bold = true;
                        dg.Items[i].Font.Strikeout = true;
                        
                        //((ImageButton)dg.Items[i].Cells[3].Controls[1]).Visible = false;
                        //((ImageButton)dg.Items[i].Cells[7].Controls[1]).ImageUrl = "../images/proto/ico_risposta.gif";
                        //((ImageButton)dg.Items[i].Cells[7].Controls[1]).CommandName = "Ripristina";
                        ((ImageButton)dg.Items[i].Cells[4].Controls[1]).Visible = false;
                        ((ImageButton)dg.Items[i].Cells[8].Controls[1]).ImageUrl = "../images/proto/ico_risposta.gif";
                        ((ImageButton)dg.Items[i].Cells[8].Controls[1]).CommandName = "Ripristina";
                        ripristina = true;
                    }

                    //Se l'utente è proprietario del documento, non è MAI possibile rimuovere i diritti.
                    //Se l'utente ha la funzione di "editing ACL" può rimuovere i diritti anche ad altri ruoli/utenti.
                    
                    //string diritto = ((Label)dg.Items[i].Cells[2].Controls[1]).Text;
                    string diritto = ((Label)dg.Items[i].Cells[3].Controls[1]).Text;
                    //Label idCorr = (Label)dg.Items[i].Cells[10].Controls[1];
                    Label idCorr = (Label)dg.Items[i].Cells[11].Controls[1];

                    if (diritto.Equals("PROPRIETARIO"))
                    {
                        //((ImageButton)dg.Items[i].Cells[7].Controls[1]).Visible = false;
                        ((ImageButton)dg.Items[i].Cells[8].Controls[1]).Visible = false;

                        //
                        // Mev Editing Acl
                        ((CheckBox)dg.Items[i].Cells[0].Controls[1]).Visible = false;
                        ((CheckBox)dg.Items[i].Cells[0].Controls[1]).Checked = false;
                        // End MEV
                        //
                    }
                    else
                    {
                        //((ImageButton)dg.Items[i].Cells[7].Controls[1]).Visible = this._isACLauthorized;
                        ((ImageButton)dg.Items[i].Cells[8].Controls[1]).Visible = this._isACLauthorized;
                        // Mev Acl
                        ((CheckBox)dg.Items[i].Cells[0].Controls[1]).Visible = this._isACLauthorized;
                        if (_isACLauthorized)
                        {
                           if (lbl.Text.Equals("UTENTE"))
                           {
                              if ((infoUtente.idPeople == idCorr.Text && !ripristina) || _idVisFasc == "reload")
                                  //((ImageButton)dg.Items[i].Cells[7].Controls[1]).Visible = true;
                                  ((ImageButton)dg.Items[i].Cells[8].Controls[1]).Visible = true;
                           }
                           else
                              if ((infoUtente.idGruppo == idCorr.Text && !ripristina) || _idVisFasc == "reload")
                                  //((ImageButton)dg.Items[i].Cells[7].Controls[1]).Visible = true;
                                  ((ImageButton)dg.Items[i].Cells[8].Controls[1]).Visible = true;

                           //
                           // Mev Editing Acl
                           ((CheckBox)dg.Items[i].Cells[0].Controls[1]).Visible = true;
                           // End MEV
                           //
                        }
                    }

                    
                }

               
            


                #region sospendi
                /*
                //si visualizza il tasto sospendi/riattiva, se è il caso
                bool visible = false;
                ((ImageButton)dg.Items[i].Cells[5].Controls[1]).Visible = false;
                //DocsPaWR.Corrispondente corr = ListaFascDir[i].soggetto;
                if (dirittiUser != null)
                {
                    if (dirittiUser.Equals("P"))
                    {
                        DocsPaWR.FascicoloTipoDiritto tipoDiritto = ListaFascDir[i].tipoDiritto;
                        if ((tipoDiritto == DocsPAWA.DocsPaWR.FascicoloTipoDiritto.TIPO_SOSPESO || tipoDiritto == DocsPAWA.DocsPaWR.FascicoloTipoDiritto.TIPO_TRASMISSIONE))
                            visible = true;
                    }
                }
                ((ImageButton)dg.Items[i].Cells[5].Controls[1]).Visible = visible;
                */
                #endregion
            }

            //i clienti senza acl non devono vedere la colonna
            this.btn_storia_visibilita.Visible = this._isACLauthorized;
            //dg.Columns[6].Visible = this._isACLauthorized;
            dg.Columns[7].Visible = this._isACLauthorized;
        }

        private void Datagrid1_SelectedIndex(object sender, System.EventArgs e)
        {
            //
            // Mev Editing ACL
            // Poichè è stata aggiunta la colonna Checkbox, occorre incrementare gli indici delle celle/colonne
            //

            //ricerca gli utenti appartenenti al ruolo specificato
           if (Request.QueryString["VisFrame"] != null && Request.QueryString["VisFrame"] != "")
              _idVisFasc = "";
           else
              _idVisFasc= "reload";

           //if (((Label)this.DataGrid1.SelectedItem.Cells[5].Controls[1]).Text.Equals("UTENTE"))
            if (((Label)this.DataGrid1.SelectedItem.Cells[6].Controls[1]).Text.Equals("UTENTE"))
            {
               getDettagliUtente(this.DataGrid1.SelectedItem);
            }
            else
            {
               getListaUtenti(this.DataGrid1.SelectedItem);
            }

            pnl_descRimuovi.Visible = false;
        }

        private void Datagrid1_pager(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
           if (Request.QueryString["VisFrame"] != null && Request.QueryString["VisFrame"] != "")
              _idVisFasc = "";
           else
              _idVisFasc = "reload";

            this.DataGrid1.CurrentPageIndex = e.NewPageIndex;
            DataTable TDNew = (DataTable)Session["Dg_visibilita"];
            DataGrid1.DataSource = TDNew;
            DataGrid1.DataBind();
        }

        public class Cols
        {
            private string diritti;
            private string ruolo;
            private string codiceRub;

            public Cols(string ruolo, string diritti, string codiceRub)
            {
                this.ruolo = ruolo;
                this.diritti = diritti;
                this.codiceRub = codiceRub;
            }
            public string Diritti { get { return diritti; } }
            public string Ruolo { get { return ruolo; } }
            public string CodiceRub { get { return codiceRub; } }
        }

        private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            //
            // Mev Editing ACL
            // Poichè è stata aggiunta la colonna Checkbox, occorre incrementare gli indici delle celle/colonne
            //
            this.lb_dettagli.Visible = false;

            if (e.CommandName.Equals("SospendiRiattiva"))
            {
                //string codRubrica = ((Label)e.Item.Cells[4].Controls[1]).Text;
                string codRubrica = ((Label)e.Item.Cells[5].Controls[1]).Text;
                sospendiRiattiva(codRubrica);
            }
            if (e.CommandName.Equals("Rimuovi"))
            {
                this.DataGrid1.SelectedIndex = e.Item.ItemIndex;
                ClientScript.RegisterStartupScript(this.GetType(), "focus", "document.getElementById('" + txt_note.ID + "').focus();", true);
                pnl_descRimuovi.Visible = true;
                txt_note.Text = "";
                //this.DataGrid1.SelectedIndex = e.Item.ItemIndex;
                //string messaggio = InitMessageXml.getInstance().getMessage("RIMUOVI_FASCICOLO_ACL");
                //Msg_Rimuovi.Confirm(messaggio);
            }
            if (e.CommandName.Equals("Ripristina"))
            {
                this.DataGrid1.SelectedIndex = e.Item.ItemIndex;
                RipristinaACL();
                pnl_descRimuovi.Visible = false;
            }
        }

        #endregion datagrid

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            this._isACLauthorized = UserManager.ruoloIsAutorized(this, "ACL_RIMUOVI");
            this.btn_storia_visibilita.Visible = this._isACLauthorized;

            if (!this._isACLauthorized)
            {
                this.CestinoMassiva.Visible = false;
                this.CheckBoxSelezionaTutti.Visible = false;
                this.RipristinoMassiva_.Visible = false;
            }

            //
            // Mev Editing ACL

            // Mantiene la posizione dello scroll
            if (!pnl_descRimuovi.Visible)
                this.Page.MaintainScrollPositionOnPostBack = true;

            // Recupero dalla sessione il valore della checkbox
            // true: Selezionata
            // false: Deselezionata
            // string.IsNullOrEmpty: deselezionata a causa della deselezione di un elemento 
            if (Session["check_all"] != null)
                check_all = (string)Session["check_all"];

            // metto in sessione il valore della variabile
            Session["check_all"] = check_all;

            // Recupero dalla sessione la dataTable del datagrid
            DataTable TDNew = (DataTable)Session["Dg_visibilita"];
            for (int i = 0; i < TDNew.Rows.Count; i++)
            {
                if (check_all.Equals("true"))
                    TDNew.Rows[i]["Checked"] = true;
                if (check_all.Equals("false"))
                    TDNew.Rows[i]["Checked"] = false;

                if (TDNew.Rows[i]["diritti"].Equals("PROPRIETARIO"))
                {
                    TDNew.Rows[i]["Checked"] = false;
                }
                else
                {
                    if (Request.QueryString["From"] != null && Request.QueryString["From"].ToString() == "ricerca")
                    {
                        TDNew.Rows[i]["Checked"] = false;
                    }
                }
            }

            // Inserisco in sessione del dataTable            
            Session["Dg_visibilita"] = TDNew;
            this.DataGrid1.DataSource = TDNew;
            this.DataGrid1.DataBind();
            // End MEV
        }

        private string setTipoDiritto(DocsPAWA.DocsPaWR.FascicoloDiritto fascDir)
        {
            if (fascDir.tipoDiritto.Equals(DocsPAWA.DocsPaWR.FascicoloTipoDiritto.TIPO_ACQUISITO))
            {                
                DocsPaVO.HMDiritti.HMdiritti diritti = new DocsPaVO.HMDiritti.HMdiritti();
                if (fascDir.accessRights == diritti.HDdiritti_Waiting)
                    return "IN ATTESA DI ACCETTAZIONE";
                else
                    return "ACQUISITO";
            }
            else
                if (fascDir.tipoDiritto.Equals(DocsPAWA.DocsPaWR.FascicoloTipoDiritto.TIPO_PROPRIETARIO))
                    return "PROPRIETARIO";
                else
                    if (fascDir.tipoDiritto.Equals(DocsPAWA.DocsPaWR.FascicoloTipoDiritto.TIPO_TRASMISSIONE))
                        return "TRASMISSIONE";
                    else
                        if (fascDir.tipoDiritto.Equals(DocsPAWA.DocsPaWR.FascicoloTipoDiritto.TIPO_SOSPESO))
                            return "SOSPESO";
                        else
                            if (fascDir.tipoDiritto.Equals(DocsPAWA.DocsPaWR.FascicoloTipoDiritto.TIPO_DELEGATO))
                                return "PROPRIETARIO";
            return "";
        }

        private void getListaUtenti(System.Web.UI.WebControls.DataGridItem selectedItem)
        {

            //string codiceRubrica = ((Label)selectedItem.Cells[4].Controls[1]).Text;
            string codiceRubrica = ((Label)selectedItem.Cells[5].Controls[1]).Text;
            //costruzione oggetto queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();

            qco.codiceRubrica = codiceRubrica;
            qco.getChildren = true;

            qco.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;// ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];
            qco.fineValidita = true;

            //corrispondenti interni
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;
            DocsPaWR.Corrispondente[] listaCorr = UserManager.getListaCorrispondenti(this.Page, qco);

            //visualizzo le informazioni sugli utenti
            string st_listaCorr = "";
            DocsPAWA.DocsPaWR.Corrispondente cor;

            for (int i = 0; i < listaCorr.Length; i++)
            {
                cor = (DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i];
                if (cor.dta_fine != string.Empty)
                    st_listaCorr += "<font color='Gray'>" + ((DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i]).descrizione + "</font><br>";
                else
                    st_listaCorr += ((DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i]).descrizione + "<br>";
            }
            this.lb_dettagli.Text = st_listaCorr;
            this.lb_dettagli.Visible = true;

        }

        private void getDettagliUtente(System.Web.UI.WebControls.DataGridItem selectedItem)
        {
            //string st_dettUtente = ((Label)selectedItem.Cells[0].Controls[1]).Text;
            string st_dettUtente = ((Label)selectedItem.Cells[1].Controls[1]).Text;
            //string codRubrica = ((Label)selectedItem.Cells[4].Controls[1]).Text;
            string codRubrica = ((Label)selectedItem.Cells[5].Controls[1]).Text;
            this.lb_dettagli.Text = st_dettUtente;
            this.lb_dettagli.Visible = true;
        }

        private void sospendiRiattiva(string codiceRubrica)
        {
            //si recuperano le informazioni sull'utente
            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);

            //si recuperano le informazioni sul corrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
            qco.codiceRubrica = codiceRubrica;
            qco.idAmministrazione = infoUtente.idAmministrazione;
            qco.getChildren = false;
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;
            qco.fineValidita = true;
            DocsPaWR.Corrispondente corr = UserManager.getListaCorrispondenti(this.Page, qco)[0];

            FascicoliManager.sospendiRiattiva(this, infoUtente.idPeople, corr, FascicoliManager.getFascicoloSelezionato(this));
            Response.Write("<script>window.location='visibilitaFascicolo.aspx'</script>");
            //Page.RegisterClientScriptBlock("submit","<script>window.document.visibilitaFascicolo.submit();</script>");

        }

        private void btn_ok_Click(object sender, System.EventArgs e)
        {
            Session.Remove("ListaFascDir");
            Response.Write("<script>window.close();</script>");
        }

        private void btn_okDesc_Click(object sender, System.EventArgs e)
        {
            //
            // Mev Editing ACL
            // Poichè è stata aggiunta la colonna Checkbox, occorre incrementare gli indici delle celle/colonne

            // Recupero dalla sessione gli elementi per la rimozione massiva
            rimozioneMassiva = (String)Session["rimozioneMassiva"];
            DataTable TDNew = (DataTable)Session["Dg_visibilita"];

            string messaggio = string.Empty;

            this.rimuoviMassivoMethod(TDNew, rimozioneMassiva);
        
        }

        private void btn_annulla_Click(object sender, System.EventArgs e)
        {
            this.pnl_descRimuovi.Visible = false;
        }

        protected void DataGrid1_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        private void Msg_Rimuovi_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                //
                // Mev Editing ACL
                // Aggiunta del controllo if-then-else per rimozione massiva ACL

                // Recupero dalla sessione del flag per la rimozione massiva e dataTable
                rimozioneMassiva = (String)Session["rimozioneMassiva"];
                DataTable TDNew = (DataTable)Session["Dg_visibilita"];

                if (!string.IsNullOrEmpty(rimozioneMassiva) && rimozioneMassiva.Equals("true"))
                {
                    // Metodo per la rimozione massiva ACL
                    if (TDNew != null)
                        RimuoviACLMassiva(TDNew);
                }
                else
                    //End Mev Editing ACL
                RimuoviACL();
            }
        }

        //
        // Mev Editing ACL
        // Message Box per il ripristina massivo
        private void Msg_Ripristina_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                // Recupero dalla sessione del flag per il ripristino massivo e dataTable
                ripristinoMassiva = (String)Session["ripristinoMassiva"];
                DataTable TDNew = (DataTable)Session["Dg_visibilita"];

                if (!string.IsNullOrEmpty(ripristinoMassiva) && ripristinoMassiva.Equals("true"))
                {
                    // Metodo per la rimozione massiva ACL
                    if (TDNew != null)
                        RipristinaACLMassiva(TDNew);
                }
            }
        }
        // End Mev Editing ACL
        //

        private void RimuoviACL()
        {
            //Verifica se è un ruolo o un utente
            string personOrGroup = "R";
            //if (((Label)DataGrid1.SelectedItem.Cells[5].Controls[1]).Text.Equals("UTENTE"))
            if (((Label)DataGrid1.SelectedItem.Cells[6].Controls[1]).Text.Equals("UTENTE"))
                personOrGroup = "U";


            ListaFascDir = (DocsPAWA.DocsPaWR.FascicoloDiritto[])Session["ListaFascDir"];
            DocsPAWA.DocsPaWR.FascicoloDiritto fascDiritti = ListaFascDir[DataGrid1.SelectedItem.DataSetIndex];
            bool result = FascicoliManager.editingFascACL(fascDiritti, personOrGroup, UserManager.getInfoUtente(this));
            if (result)
            {
              
               _idVisFasc = "reload";
               Page_Load(null, new ModifyACL()  );
               DataGrid1.SelectedIndex = -1;
                //riga segnata in rosso
                //if (fascDiritti.tipoDiritto.Equals(DocsPAWA.DocsPaWR.FascicoloTipoDiritto.TIPO_PROPRIETARIO))
                //{
                //    Response.Write("<script>alert('Questa operazione comporterà l\\'acquisizione\\ndei diritti di proprietà');</script>");
                //    _idVisFasc = "reload";
                //    Page_Load(null, null);
                //}
                //else
                //{
                //    DataGrid1.SelectedItem.ForeColor = Color.Red;
                //    DataGrid1.SelectedItem.Font.Bold = true;
                //    DataGrid1.SelectedItem.Font.Strikeout = true;
                //    ((ImageButton)DataGrid1.SelectedItem.Cells[2].Controls[1]).Visible = false;
                //    ((ImageButton)DataGrid1.SelectedItem.Cells[6].Controls[1]).ImageUrl = "../images/proto/ico_risposta.gif";
                //    ((ImageButton)DataGrid1.SelectedItem.Cells[6].Controls[1]).CommandName = "Ripristina";
                //}
                //DataGrid1.SelectedIndex = -1;
            }
            else
            {
                Response.Write("<script>alert('Attenzione operazione non riuscita');</script>");
            }
            pnl_descRimuovi.Visible = false;
        }

        private void RipristinaACL()
        {
            //Verifica se è un ruolo o un utente
            string personOrGroup = "R";
            //if (((Label)DataGrid1.SelectedItem.Cells[5].Controls[1]).Text.Equals("UTENTE"))
            if (((Label)DataGrid1.SelectedItem.Cells[6].Controls[1]).Text.Equals("UTENTE"))
                personOrGroup = "U";

            //chiamata al WebService per la rimozione dalla deleted_security
            //inserimento del record nella security
            ListaFascDir = (DocsPAWA.DocsPaWR.FascicoloDiritto[])Session["ListaFascDir"];
            DocsPAWA.DocsPaWR.FascicoloDiritto fascDiritti = ListaFascDir[DataGrid1.SelectedItem.DataSetIndex];
            
            bool result = FascicoliManager.ripristinaFascACL(fascDiritti, personOrGroup, UserManager.getInfoUtente(this));
            if (result)
            {

               _idVisFasc = "reload";
               Page_Load(null, new ModifyACL());
               DataGrid1.SelectedIndex = -1;
                //if (fascDiritti.tipoDiritto.Equals(DocsPAWA.DocsPaWR.FascicoloTipoDiritto.TIPO_PROPRIETARIO))
                //{
                //    _idVisFasc = "reload";
                //    Page_Load(null, null);
                //}
                //else
                //{
                //    //riga segnata in rosso
                //    DataGrid1.SelectedItem.ForeColor = Color.Black;
                //    DataGrid1.SelectedItem.Font.Bold = false;
                //    DataGrid1.SelectedItem.Font.Strikeout = false;
                //    ((ImageButton)DataGrid1.SelectedItem.Cells[2].Controls[1]).Visible = true;
                //    ((ImageButton)DataGrid1.SelectedItem.Cells[6].Controls[1]).ImageUrl = "../images/proto/b_elimina.gif";
                //    ((ImageButton)DataGrid1.SelectedItem.Cells[6].Controls[1]).CommandName = "Rimuovi";
                //}
                //DataGrid1.SelectedIndex = -1;
            }
            else
            {
                Response.Write("<script>alert('Attenzione operazione non riuscita');</script>");
            }
            pnl_descRimuovi.Visible = false;
        }

        protected void btn_storia_visibilita_Click(object sender, ImageClickEventArgs e)
        {
 
           
            string scriptString = "<SCRIPT>ApriFinestraStoriaVisibilita('F');</SCRIPT>";
            this.RegisterStartupScript("apriModalDialogStoriaVisibilita", scriptString);
            //this.btn_storia_visibilita.Attributes.Add("onclick", "ApriFinestraStoriaVisibilita('D');return false;");

       
        }

        //
        // Mev editing ACL

        /// <summary>
        /// Metodo per la rimozione massiva
        /// Invocato all onClick del cestino
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void RimuoviElementiSelezionati(Object sender, EventArgs args) 
        {
            // Pulisco il valore dell'operazione contrastante
            ripristinoMassiva = string.Empty;
            Session["ripristinoMassiva"] = ripristinoMassiva;
            this.CheckBoxSelezionaTutti.Checked = false;
            this.lb_dettagli.Visible = false;

            int countElementiChecked = 0;

            //Recupero dalla sessione il DataTable
            DataTable TDNew = (DataTable)Session["Dg_visibilita"];

            countElementiChecked = ContaElementiChecked(TDNew);

            if (countElementiChecked > 0)
            {
                // Flag per la rimozione massiva Acl
                rimozioneMassiva = "true";

                pnl_descRimuovi.Visible = true;
                txt_note.Text = "";

                //imposto il focus sul pannelo di rimozione elementi
                pnl_descRimuovi.Focus();
            }

            // Commentato per l'aggiunta del pnl_descRimuovi
            //this.rimuoviMassivoMethod(TDNew, rimozioneMassiva);

            // Inserisco in sessione il valore del flag per la rimozione Massiva
            Session["rimozioneMassiva"] = rimozioneMassiva;
            Session["pnl_descRimuovi.Visible"] = pnl_descRimuovi.Visible;
        }

        /// <summary>
        /// Metodo per il ripristino massivo
        /// invocato al clic del ripristino massivo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void RipristinaElementiSelezionati(Object sender, EventArgs args) 
        {
            // Pulisco il valore dell'operazione contrastante
            rimozioneMassiva = string.Empty;
            Session["rimozioneMassiva"] = rimozioneMassiva;
            this.CheckBoxSelezionaTutti.Checked = false;
            this.lb_dettagli.Visible = false;

            int countElementiChecked = 0;
            //Recupero dalla sessione il DataTable
            DataTable TDNew = (DataTable)Session["Dg_visibilita"];

            countElementiChecked = ContaElementiChecked(TDNew);

            if (countElementiChecked > 0)
            {
                // Flag per la rimozione massiva Acl
                ripristinoMassiva = "true";
            }

            this.ripristinaMassivoMethod(TDNew, ripristinoMassiva);

            Session["ripristinoMassiva"] = ripristinoMassiva;
        }

        /// <summary>
        /// Funzione per cliccare tutte le checkBox
        /// Viene richiamata sul click della checkbox Seleziona/deseleziona Tutti
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void SelectAll(Object sender, EventArgs args) 
        {
            CheckBox linkedItem = sender as CheckBox;
            Boolean itemchecked = linkedItem.Checked;
            check_all = "false";
            if (itemchecked)
            {
                //imposto a checked tutti gli item visible del datagrid
                check_all = "true";
            }
            Session["check_all"] = check_all;
        }

        /// <summary>
        /// Funzione invocata all'atto della selezione/deselezione di una checkbox
        /// Se la check è selezionata/deselezionata si clicca la checkbox, 
        /// viene impostata la variabile check_all a sting.Empty per preservare il valore delle checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void verificaCheckSelectAll(Object sender, EventArgs args) 
        {
            CheckBox cb = (CheckBox)sender;
            if (this.CheckBoxSelezionaTutti.Checked)
            {
                CheckBoxSelezionaTutti.Checked = false;
            }
            // La stringa vuota indica che devo deselezionare solo la checkbox seleziona/deseleziona tutti
            check_all = string.Empty;
            Session["check_all"] = check_all;

            //Imposto la check selezionata/deselezionata nel dataTable
            DataTable TDNew = (DataTable)Session["Dg_visibilita"];

            for (int i = 0; i < TDNew.Rows.Count; i++)
            {
                //
                // Per individuare l'elemento lo confronto con la sua classe CSS composta come segue:
                // idCorr_diritti_TipoDiritto
                if ((TDNew.Rows[i]["idCorr"].ToString() + "_" + TDNew.Rows[i]["diritti"].ToString() + "_" + TDNew.Rows[i]["TipoDiritto"].ToString()).Equals(cb.CssClass))
                    TDNew.Rows[i]["Checked"] = cb.Checked;
            }

            Session["Dg_visibilita"] = TDNew;
        }

        /// <summary>
        /// Metodo di utility ContaElementiChecked
        /// </summary>
        /// <param name="dt">DataTable dt</param>
        /// <returns></returns>
        private int ContaElementiChecked(DataTable dt)
        {
            int count = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((bool)dt.Rows[i]["Checked"])
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Metodo di utility per il ripristino massivo che compone i messaggi per l'utente
        /// </summary>
        /// <param name="TDNew">DataTable con i dati</param>
        /// <param name="ripristinoMassiva">True/False</param>
        private void ripristinaMassivoMethod(DataTable TDNew, string ripristinoMassiva)
        {
            // Recupero dalla sessione gli elementi per la rimozione massiva

            string messaggio = string.Empty;

            //
            // Mev Editing ACL
            // Se ho cliccato il ripristino per il ripristino massivo con almeno un elemento gestisco tale ripristino 
            if (!string.IsNullOrEmpty(ripristinoMassiva) && ripristinoMassiva.Equals("true"))
            {
                if (ContaElementiCheckedDeleted(TDNew) > 0)
                {
                    messaggio = "Questa operazione ripristina tutti i diritti di visibilità del fascicolo per i seguenti ruoli: \\n";
                }
                string mess = string.Empty;

                for (int i = 0; i < TDNew.Rows.Count; i++)
                {
                    if ((bool)TDNew.Rows[i]["Checked"])
                    {
                        if (TDNew.Rows[i]["rimosso"].Equals("1"))
                        {
                            string elementoDaRipristinare = string.Empty;
                            elementoDaRipristinare = "- " + TDNew.Rows[i]["ruolo"].ToString();
                            messaggio = messaggio + elementoDaRipristinare + "\\n";
                        }
                        else
                        {
                            //elementi che non è possibile ripristinare
                            string elementoNonRipristinabile = string.Empty;
                            elementoNonRipristinabile = "- " + TDNew.Rows[i]["ruolo"].ToString();
                            mess = mess + elementoNonRipristinabile + "\\n";
                        }
                    }
                }

                // composizione messaggio per elementi non ripristinabili
                if (!string.IsNullOrEmpty(mess))
                {
                    string testoElementiNonRipristinabili = "Attenzione non è possibile procedere con il ripristino dei seguenti elementi, in quanto non rimossi:" + "\\n";
                    messaggio = messaggio + "\\n" + testoElementiNonRipristinabili + mess;
                }

                Msg_Ripristina.Confirm(messaggio);
            }

        }

        private void rimuoviMassivoMethod(DataTable TDNew, string rimozioneMassiva)
        {
            //
            // Mev Editing ACL
            // Poichè è stata aggiunta la colonna Checkbox, occorre incrementare gli indici delle celle/colonne

            string messaggio = string.Empty;

            //
            // Mev Editing ACL
            // Se ho cliccato il cestino per la rimozione massiva con almeno un elemento gestisco tale rimozione 
            if (!string.IsNullOrEmpty(rimozioneMassiva) && rimozioneMassiva.Equals("true"))
            {
                if (ContaElementiCheckedNotDeleted(TDNew) > 0)
                {
                    messaggio = "Questa operazione rimuove tutti i diritti di visibilità del fascicolo per i seguenti ruoli: \\n";
                }
                string mess = string.Empty;

                for (int i = 0; i < TDNew.Rows.Count; i++)
                {
                    if ((bool)TDNew.Rows[i]["Checked"])
                    {
                        if (TDNew.Rows[i]["rimosso"].Equals("0"))
                        {
                            string elementoDaRimuovere = string.Empty;
                            elementoDaRimuovere = "- " + TDNew.Rows[i]["ruolo"].ToString();
                            messaggio = messaggio + elementoDaRimuovere + "\\n";
                        }
                        else
                        {
                            //elementi che non è possibile rimuovere
                            string elementoNonRemovibile = string.Empty;
                            elementoNonRemovibile = "- " + TDNew.Rows[i]["ruolo"].ToString();
                            mess = mess + elementoNonRemovibile + "\\n";
                        }
                    }
                }

                // composizione messaggio per elementi non removibili
                if (!string.IsNullOrEmpty(mess))
                {
                    string testoElementiNonRemovibili = "Attenzione non è possibile procedere con la rimozione dei seguenti elementi, in quanto già rimossi:" + "\\n";
                    messaggio = messaggio + "\\n" + testoElementiNonRemovibili + mess;
                }
            }
            else
            // End MEV Editing ACL
            //
            {
                messaggio = InitMessageXml.getInstance().getMessage("RIMUOVI_FASCICOLO_ACL");    
            }
            
            Msg_Rimuovi.Confirm(messaggio);

        }

        /// <summary>
        /// Metodo di utility ContaElementiCheckedNotDeleted
        /// </summary>
        /// <param name="dt">DataTable dt</param>
        /// <returns></returns>
        private int ContaElementiCheckedNotDeleted(DataTable dt)
        {
            int count = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((bool)dt.Rows[i]["Checked"] && dt.Rows[i]["rimosso"].Equals("0"))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Metodo di utility ContaElementiCheckedDeleted
        /// </summary>
        /// <param name="dt">DataTable dt</param>
        /// <returns></returns>
        private int ContaElementiCheckedDeleted(DataTable dt)
        {
            int count = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((bool)dt.Rows[i]["Checked"] && dt.Rows[i]["rimosso"].Equals("1"))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Metodo per il ripristino massiva degli elementi in ACL
        /// </summary>
        private void RipristinaACLMassiva(DataTable dt)
        {
            //prendo dalla sessione ListaFascDir
            ListaFascDir = (DocsPAWA.DocsPaWR.FascicoloDiritto[])Session["ListaFascDir"];

            // Lista elementi Non ripristinati
            List<string> listNotRestored = new List<string>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((bool)dt.Rows[i]["Checked"])
                {
                    if (dt.Rows[i]["rimosso"].Equals("1"))
                    {
                        //Verifica se si vuole rimuovere acl a un ruolo o un utente
                        string personOrGroup = "R";

                        if (dt.Rows[i]["tipo"].Equals("UTENTE"))
                            personOrGroup = "U";

                        //Identifico un elemento in dt che ritrovo nella listaDocDir
                        string idCorr = (string)dt.Rows[i]["idCorr"];
                        string diritti = (string)dt.Rows[i]["diritti"];
                        string tipodiritto = (string)dt.Rows[i]["TipoDiritto"];
                        int indice = 0;

                        for (int j = 0; j < ListaFascDir.Length; j++)
                        {
                            //if (ListaFascDir[j].personorgroup.Equals(idCorr) && !ListaFascDir[j].rimosso)
                            //if (ListaFascDir[j].personorgroup.Equals(idCorr) && ListaFascDir[j].rimosso)
                            if (ListaFascDir[j].personorgroup.Equals(idCorr)
                                && setTipoDiritto(ListaFascDir[j]).ToString().Equals(diritti)
                                && this.GetRightDescription(ListaFascDir[j].accessRights).ToString().Equals(tipodiritto)
                                && ListaFascDir[j].deleted)
                                indice = j;
                        }

                        DocsPAWA.DocsPaWR.FascicoloDiritto fascDiritti = ListaFascDir[indice];
                        //Chiamata per il ripristino
                        bool result = FascicoliManager.ripristinaFascACL(fascDiritti, personOrGroup, UserManager.getInfoUtente(this));

                        if (result)
                        {
                            //OK
                        }
                        else
                        {
                            //Popolo una lista di stringhe con gli elementi che non sono stati rimossi dalla ACL
                            listNotRestored.Add(dt.Rows[i]["ruolo"] + "\\n");
                        }
                    }
                }
            }

            pnl_descRimuovi.Visible = false;

            // Resoconto degli elemneti non andati a buon fine
            if (listNotRestored != null && listNotRestored.Count > 0)
            {
                string resultConcat = string.Empty;
                foreach (string tempMess in listNotRestored)
                {
                    resultConcat += tempMess.Replace("'", "\\'");
                }
                // Report degli elementi che non sono stati rimossi
                Response.Write("<script>alert('Attenzione operazione non riuscita per i seguenti elementi:\\n" + resultConcat + "');</script>");
            }

            //Al termine tolgo dalla sessione RipristinaMassivo
            ripristinoMassiva = string.Empty;
            Session["ripristinoMassiva"] = ripristinoMassiva;

            // Al termine occorre ricaricare la pagina
            _idVisFasc = "reload";
            Page_Load(null, new ModifyACL());
            DataGrid1.SelectedIndex = -1;
        }

        /// <summary>
        /// Metodo per la rimozione massiva degli elementi in ACL
        /// </summary>
        private void RimuoviACLMassiva(DataTable dt)
        {
            //prendo dalla sessione ListaFascDir
            ListaFascDir = (DocsPAWA.DocsPaWR.FascicoloDiritto[])Session["ListaFascDir"];

            // Lista elementi Non rimossi
            List<string> listNotRemoved = new List<string>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((bool)dt.Rows[i]["Checked"])
                {
                    if (dt.Rows[i]["rimosso"].Equals("0"))
                    {
                        //Verifica se si vuole rimuovere acl a un ruolo o un utente
                        string personOrGroup = "R";

                        if (dt.Rows[i]["tipo"].Equals("UTENTE"))
                            personOrGroup = "U";

                        //Identifico un elemento in dt che ritrovo nella listaDocDir
                        string idCorr = (string)dt.Rows[i]["idCorr"];
                        string diritti = (string)dt.Rows[i]["diritti"];
                        string tipodiritto = (string)dt.Rows[i]["TipoDiritto"];
                        int indice = 0;

                        for (int j = 0; j < ListaFascDir.Length; j++)
                        {
                            //if (ListaFascDir[j].personorgroup.Equals(idCorr) && !ListaFascDir[j].rimosso)
                            if (ListaFascDir[j].personorgroup.Equals(idCorr)
                                && setTipoDiritto(ListaFascDir[j]).ToString().Equals(diritti)
                                && this.GetRightDescription(ListaFascDir[j].accessRights).ToString().Equals(tipodiritto)
                                && !ListaFascDir[j].deleted)
                                indice = j;
                        }

                        DocsPAWA.DocsPaWR.FascicoloDiritto fascDiritti = ListaFascDir[indice];
                        bool result = FascicoliManager.editingFascACL(fascDiritti, personOrGroup, UserManager.getInfoUtente(this));

                        if (result)
                        {
                            //OK
                        }
                        else
                        {
                            //Popolo una lista di stringhe con gli elementi che non sono stati rimossi dalla ACL
                            listNotRemoved.Add(dt.Rows[i]["ruolo"] + "\\n");
                        }
                    }

                }
            }

            pnl_descRimuovi.Visible = false;

            // Resoconto degli elemneti non andati a buon fine
            if (listNotRemoved != null && listNotRemoved.Count > 0)
            {
                string resultConcat = string.Empty;
                foreach (string tempMess in listNotRemoved)
                {
                    resultConcat += tempMess.Replace("'", "\\'");
                }
                // Report degli elementi che non sono stati rimossi
                Response.Write("<script>alert('Attenzione operazione non riuscita per i seguenti elementi:\\n" + resultConcat + "');</script>");
            }

            //Al termine tolgo dalla sessione RipristinaMassivo
            rimozioneMassiva = string.Empty;
            Session["rimozioneMassiva"] = rimozioneMassiva;

            // Al termine occorre ricaricare la pagina
            _idVisFasc = "reload";
            Page_Load(null, new ModifyACL());
            DataGrid1.SelectedIndex = -1;
        }
    }
}
