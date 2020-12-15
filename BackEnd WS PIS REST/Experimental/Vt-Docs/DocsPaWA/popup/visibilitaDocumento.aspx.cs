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
    public class visibilitaDocumento : DocsPAWA.CssPage 
    {
        protected System.Web.UI.WebControls.DataGrid DataGrid1;
        protected DocsPAWA.DocsPaWR.InfoDocumento InfoDoc;
        protected DocsPAWA.DocsPaWR.SchedaDocumento SchedaDoc;
        protected DocsPAWA.DocsPaWR.DocumentoDiritto[] ListaDocDir;
        protected System.Web.UI.WebControls.Button btn_ok;
        protected System.Web.UI.WebControls.Label lb_dettagli;
        protected System.Web.UI.WebControls.Label Label2;
        protected System.Web.UI.WebControls.Label lbl_InArchivio;
        protected DocsPAWA.dataSet.DS_Visibilit dS_Visibilit1;
        protected Utilities.MessageBox Msg_Rimuovi;
        protected DocsPaWebCtrlLibrary.ImageButton btn_storia_visibilita;
        protected DocsPaWebCtrlLibrary.ImageButton CestinoMassiva;
        protected DocsPaWebCtrlLibrary.ImageButton RipristinoMass;
        
        protected System.Web.UI.WebControls.Panel pnl_descRimuovi;
        protected System.Web.UI.WebControls.TextBox txt_note;
        protected System.Web.UI.WebControls.Button btn_okDesc;
        protected System.Web.UI.WebControls.Button btn_annulla;
        
        protected ArrayList Dt_elem;
        protected string _idVisDoc = String.Empty;
        protected bool _isACLauthorized;
        protected System.Web.UI.WebControls.Label lbl_atipicita;
        //
        // Mev Editing ACL
        protected string check_all = string.Empty;
        protected System.Web.UI.WebControls.CheckBox CheckBoxSelezionaTutti;
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

           bool cercaRimossi;
            
            Response.Expires = -1;

            if ((!IsPostBack) && (Request.QueryString["VisFrame"] != null)
                && (Request.QueryString["VisFrame"]) != "")
            {
               cercaRimossi = false;
                
                //this.btn_storia_visibilita.Visible = this._isACLauthorized;
            
                //arriva da ricerca visibilità
               if ( (Request.QueryString["From"] != null && Request.QueryString["From"].ToString() == "ricerca") || !this._isACLauthorized)
               {
                   if (Request.QueryString["From"] != null && Request.QueryString["From"].ToString() == "ricerca" && Request.QueryString["inArchivio"] != null && Request.QueryString["inArchivio"].ToString() == "1" && !(UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA")))
                   {
                       lbl_InArchivio.Visible = true;
                   }
                   else
                       lbl_InArchivio.Visible = false;
                   this.btn_storia_visibilita.Visible = false;
                   this.CheckBoxSelezionaTutti.Visible = false;
                   this.RipristinoMass.Visible = false;
                   this.CestinoMassiva.Visible = false;
               }
               else
                   this.btn_storia_visibilita.Visible = true;

               // btn_storia_visibilita.Visible = false;
                _idVisDoc = Request.QueryString["VisFrame"].ToString();
                //ListaDocDir = DocumentManager.getListaVisibilita(this, _idVisDoc, cercaRimossi);
                ListaDocDir = DocumentManager.getListaVisibilitaSemplificata(this, _idVisDoc, cercaRimossi);
                if (ListaDocDir != null && ListaDocDir.Length <= 0)
                {
                    if (!lbl_InArchivio.Visible)
                    {
                        this.lb_dettagli.Text = "Nessun valore trovato";
                        this.lb_dettagli.Visible = true;
                    }
                    return;
                }

                for (int i = 0; i < ListaDocDir.Length; i++)
                {
                    string descrSoggetto = UserManager.getDecrizioneCorrispondente(this, ListaDocDir[i].soggetto);
                    string Corr = GetTipoCorr(ListaDocDir[i]);
                    //Mev Editing ACL
                    this.dS_Visibilit1.element1.Addelement1Row(setTipoDiritto(ListaDocDir[i]), descrSoggetto, ListaDocDir[i].soggetto.codiceRubrica, Corr, ListaDocDir[i].soggetto.dta_fine, "0", "", ListaDocDir[i].personorgroup, ListaDocDir[i].dtaInsSecurity, ListaDocDir[i].noteSecurity, ListaDocDir[i].soggetto.tipoCorrispondente == "R" ? !String.IsNullOrEmpty(((DocsPAWA.DocsPaWR.Ruolo)ListaDocDir[i].soggetto).ShowHistory) && ((DocsPAWA.DocsPaWR.Ruolo)ListaDocDir[i].soggetto).ShowHistory != "0" : false, ListaDocDir[i].soggetto.systemId, this.GetRightDescription(ListaDocDir[i].accessRights), ListaDocDir[i].Checked);
                }
                
                Session["Dg_visibilita"] = this.dS_Visibilit1.Tables[0];
                this.DataGrid1.DataSource = this.dS_Visibilit1.Tables[0];
                if (Session["ProtoExist"] == null)
                {
                    this.Label2.Visible = false;
                    this.btn_ok.Visible = false;
                }
                else //arrivo da Avviso Protocollo Esistente
                {
                    this.Label2.Visible = true;
                    this.btn_ok.Visible = true;
                    Session.Remove("ProtoExist");
                }
                this.lb_dettagli.Visible = true;
                this.DataGrid1.PageSize = 13;
                this.DataGrid1.DataBind();
           }
           if (((!IsPostBack) && (_idVisDoc == string.Empty)) || _idVisDoc == "reload")
           {
                ////Verifica se l'utente è abilitato alla funzione di editing ACL
                //this._isACLauthorized = UserManager.ruoloIsAutorized(this, "ACL_RIMUOVI");                

                SchedaDoc = (DocsPAWA.DocsPaWR.SchedaDocumento)DocumentManager.getDocumentoSelezionato(this);

                if (SchedaDoc == null)
                {
                    this.lb_dettagli.Text = "Errore nel reperimento dei dati del documento";
                    this.lb_dettagli.Visible = true;
                    return;
                }

                InfoDoc = DocumentManager.getInfoDocumento(SchedaDoc);
                if (InfoDoc == null)
                {
                    this.lb_dettagli.Text = "Errore nel reperimento dei dati del documento";
                    this.lb_dettagli.Visible = true;
                    return;
                }

                //
                cercaRimossi = true;
                //ListaDocDir = DocumentManager.getListaVisibilita(this, InfoDoc.idProfile, cercaRimossi);

                ListaDocDir = DocumentManager.getListaVisibilitaSemplificata(this, InfoDoc.idProfile, cercaRimossi);
                
                if (ListaDocDir != null && ListaDocDir.Length <= 0)
                {
                    this.lb_dettagli.Text = "Nessun valore trovato";
                    this.lb_dettagli.Visible = true;
                    return;
                }

                string rimosso = "";
                for (int i = 0; i < ListaDocDir.Length; i++)
                {
                    string descrSoggetto = UserManager.getDecrizioneCorrispondente(this, ListaDocDir[i].soggetto);
                    string Corr = GetTipoCorr(ListaDocDir[i]);
                    if (ListaDocDir[i].deleted)
                        rimosso = "1";
                    else rimosso = "0";
                    string note = ListaDocDir[i].note;
                    //Mev editing ACL
                    this.dS_Visibilit1.element1.Addelement1Row(setTipoDiritto(ListaDocDir[i]), descrSoggetto, ListaDocDir[i].soggetto.codiceRubrica, Corr, ListaDocDir[i].soggetto.dta_fine, rimosso, note, ListaDocDir[i].personorgroup, ListaDocDir[i].dtaInsSecurity, ListaDocDir[i].noteSecurity, ListaDocDir[i].soggetto.tipoCorrispondente == "R" ? !String.IsNullOrEmpty(((DocsPAWA.DocsPaWR.Ruolo)ListaDocDir[i].soggetto).ShowHistory) && ((DocsPAWA.DocsPaWR.Ruolo)ListaDocDir[i].soggetto).ShowHistory != "0" : false, ListaDocDir[i].soggetto.systemId, this.GetRightDescription(ListaDocDir[i].accessRights), ListaDocDir[i].Checked);
                }
                Session["ListaDocDir"] = this.ListaDocDir;
                Session["Dg_visibilita"] = this.dS_Visibilit1.Tables[0];
                this.DataGrid1.DataSource = this.dS_Visibilit1.Tables[0];
                this.DataGrid1.DataBind();

            }
            Utils.DefaultButton(this, ref txt_note, ref btn_okDesc);

            //Verifica atipicita documento
            DocsPaWR.InfoAtipicita infoAtipicita = null;
            if (SchedaDoc != null && !string.IsNullOrEmpty(SchedaDoc.docNumber))
                infoAtipicita = Utils.GetInfoAtipicita(DocsPaWR.TipoOggettoAtipico.DOCUMENTO, SchedaDoc.docNumber);
            if (infoAtipicita != null && infoAtipicita.CodiceAtipicita != "T" && !string.IsNullOrEmpty(infoAtipicita.DescrizioneAtipicita))
            {
                lbl_atipicita.Text = infoAtipicita.DescrizioneAtipicita;
                lbl_atipicita.Visible = true;
            }

            if (infoAtipicita != null && infoAtipicita.CodiceAtipicita == "T" && e != null && e is ModifyACL)
                this.lbl_atipicita.Visible = false;
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

        protected string GetTipoCorr(DocsPAWA.DocsPaWR.DocumentoDiritto docDirit)
        {
            try
            {
                string rtn = "";
                if (docDirit.soggetto.GetType() == typeof(DocsPAWA.DocsPaWR.Utente))
                    rtn = "UTENTE";
                else if (docDirit.soggetto.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
                    rtn = "RUOLO";
                else if (docDirit.soggetto.GetType() == typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))
                    rtn = "U.O.";
                return rtn;
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
                return "";
            }
        }

        protected void DataGrid1_PreRender(object sender, System.EventArgs e)
        {

           //Verifica se l'utente è abilitato alla funzione di editing ACL
           this._isACLauthorized = UserManager.ruoloIsAutorized(this, "ACL_RIMUOVI");  
            bool isUtente = false;
            int posPeople = -1;
            DataGrid dg = (DataGrid)sender;
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();

            for (int i = 0; i < dg.Items.Count; i++)
            {
                if (dg.Items[i].ItemIndex >= 0)
                {

                    //
                    // Poichè è stata aggiunta la colonna Checkbox, occorre incrementare gli indici delle celle/colonne
                    //

                    //Label lbl = (Label)dg.Items[i].Cells[5].Controls[1];
                    Label lbl = (Label)dg.Items[i].Cells[6].Controls[1];
                    if (lbl.Text.Equals("UTENTE"))
                    {
                        //((ImageButton)dg.Items[i].Cells[3].Controls[1]).Visible = false;
                        ((ImageButton)dg.Items[i].Cells[4].Controls[1]).Visible = false;
                    }

                    //Label dataFine = (Label)dg.Items[i].Cells[6].Controls[1];
                    Label dataFine = (Label)dg.Items[i].Cells[7].Controls[1];
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
                        ((ImageButton)dg.Items[i].Cells[4].Controls[1]).Visible = false;
                        //((ImageButton)dg.Items[i].Cells[7].Controls[1]).ImageUrl = "../images/proto/ico_risposta.gif";
                        ((ImageButton)dg.Items[i].Cells[8].Controls[1]).ImageUrl = "../images/proto/ico_risposta.gif";
                        //((ImageButton)dg.Items[i].Cells[7].Controls[1]).CommandName = "Ripristina";
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
                        //((ImageButton)dg.Items[i].Cells[7].Controls[1]).Visible = this._isACLauthorized && string.IsNullOrEmpty(_idVisDoc);
                        ((ImageButton)dg.Items[i].Cells[8].Controls[1]).Visible = this._isACLauthorized && string.IsNullOrEmpty(_idVisDoc);
                        // Mev Acl
                        ((CheckBox)dg.Items[i].Cells[0].Controls[1]).Visible = this._isACLauthorized && string.IsNullOrEmpty(_idVisDoc);
                        if (_isACLauthorized)
                        {
                           if (lbl.Text.Equals("UTENTE"))
                           {
                               if ((infoUtente.idPeople != idCorr.Text && !ripristina) || _idVisDoc == "reload")
                               {
                                   //((ImageButton)dg.Items[i].Cells[7].Controls[1]).Visible = true;
                                   ((ImageButton)dg.Items[i].Cells[8].Controls[1]).Visible = true;
                                   
                               }
                           }
                           else
                           {
                               if ((infoUtente.idGruppo != idCorr.Text && !ripristina) || _idVisDoc == "reload")
                               {
                                    //((ImageButton)dg.Items[i].Cells[7].Controls[1]).Visible = true;
                                    ((ImageButton)dg.Items[i].Cells[8].Controls[1]).Visible = true;

                               }
                           }

                           //
                           // Mev Editing Acl
                           ((CheckBox)dg.Items[i].Cells[0].Controls[1]).Visible = true;
                           // End MEV
                           //
                        }
                        if (Request.QueryString["From"]!=null && Request.QueryString["From"].ToString() == "ricerca")
                        {
                           //((ImageButton)dg.Items[i].Cells[7].Controls[1]).Visible = false;
                           ((ImageButton)dg.Items[i].Cells[8].Controls[1]).Visible = false;

                           //
                           // Mev Editing Acl
                           ((CheckBox)dg.Items[i].Cells[0].Controls[1]).Visible = false;
                           // End MEV
                           //
                        }
                    }
 
                 }

                

            }//End For

            //i clienti senza acl non devono vedere la colonna
            //dg.Columns[6].Visible = this._isACLauthorized;
            dg.Columns[7].Visible = this._isACLauthorized;

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
            //this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.Datagrid1_pager);
            //this.DataGrid1.PreRender += new System.EventHandler(this.DataGrid1_PreRender);
            //this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.Datagrid1_SelectedIndex);
            this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
            this.Msg_Rimuovi.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.Msg_Rimuovi_GetMessageBoxResponse);
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            this.btn_okDesc.Click += new System.EventHandler(this.btn_okDesc_Click);
            this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
            this.btn_storia_visibilita.Click += new System.Web.UI.ImageClickEventHandler(this.btn_storia_visibilita_Click);
            // Mev editing ACL
            this.Msg_Ripristina.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.Msg_Ripristina_GetMessageBoxResponse);
            //
            // Inserito per risolvere il problema del check seleziona/deseleziona tutti quando si apre e poi richiude la finestra 
            Session["check_all"] = String.Empty;
            // End editing ACL
            
            // 
            // dS_Visibilit1
            // 
            this.dS_Visibilit1.DataSetName = "DS_Visibilit";
            this.dS_Visibilit1.Locale = new System.Globalization.CultureInfo("en-US");
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);
            ((System.ComponentModel.ISupportInitialize)(this.dS_Visibilit1)).EndInit();

        }
        #endregion
      
        #region datagrid

        protected void Datagrid1_SelectedIndex(object sender, System.EventArgs e)
        {
           if(Request.QueryString["VisFrame"] != null && Request.QueryString["VisFrame"] != "")
              _idVisDoc = "";
           else
             _idVisDoc = "reload";

            //ricerca gli utenti appartenenti al ruolo specificato
            
            //
            // Mev editing ACL
            // a seguito dell'aggiunta di una colonna, occorre spostare di una posizione l'indice
            //getListaUtenti(((Label)this.DataGrid1.SelectedItem.Cells[4].Controls[1]).Text);
            getListaUtenti(((Label)this.DataGrid1.SelectedItem.Cells[5].Controls[1]).Text);
            // End Mev
            //
            pnl_descRimuovi.Visible = false;

        }

        protected void Datagrid1_pager(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
           if (Request.QueryString["VisFrame"] != null && Request.QueryString["VisFrame"] != "")
              _idVisDoc = "";
           else
              _idVisDoc = "reload";

            this.DataGrid1.CurrentPageIndex = e.NewPageIndex;
            DataTable TDNew = (DataTable)Session["Dg_visibilita"];
            DataGrid1.DataSource = TDNew;
            DataGrid1.DataBind();
        }

        private void DT_created(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {

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

        #endregion datagrid

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            this._isACLauthorized = UserManager.ruoloIsAutorized(this, "ACL_RIMUOVI");
           // this.btn_storia_visibilita.Visible = this._isACLauthorized;

            if (!this._isACLauthorized)
            {
                this.CestinoMassiva.Visible = false;
                this.CheckBoxSelezionaTutti.Visible = false;
                this.RipristinoMass.Visible = false;
            }

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

        private string setTipoDiritto(DocsPAWA.DocsPaWR.DocumentoDiritto docDir)
        {
            string star = (docDir.hideDocVersions ? Environment.NewLine + "*" : "");

            if (docDir.tipoDiritto.Equals(DocsPAWA.DocsPaWR.DocumentoTipoDiritto.TIPO_ACQUISITO))
            {
                DocsPaVO.HMDiritti.HMdiritti diritti = new DocsPaVO.HMDiritti.HMdiritti();
                if (docDir.accessRights == diritti.HDdiritti_Waiting)
                    return "IN ATTESA DI ACCETTAZIONE" + star;
                else
                    return "ACQUISITO" + star;
            }
            else
                if (docDir.tipoDiritto.Equals(DocsPAWA.DocsPaWR.DocumentoTipoDiritto.TIPO_PROPRIETARIO))
                    return "PROPRIETARIO" + star;
                else
                    if (docDir.tipoDiritto.Equals(DocsPAWA.DocsPaWR.DocumentoTipoDiritto.TIPO_TRASMISSIONE))
                        return "TRASMISSIONE" + star;
                    else
                        if (docDir.tipoDiritto.Equals(DocsPAWA.DocsPaWR.DocumentoTipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO))
                            return "INSERIMENTO IN FASC." + star;
                        else
                            if (docDir.tipoDiritto.Equals(DocsPAWA.DocsPaWR.DocumentoTipoDiritto.TIPO_SOSPESO))
                                return "SOSPESO" + star;
                            else
                                if (docDir.tipoDiritto.Equals(DocsPAWA.DocsPaWR.DocumentoTipoDiritto.TIPO_DELEGATO))
                                    return "PROPRIETARIO" + star;
            return "";
        }

        private void getListaUtenti(string codiceRubrica)
        {

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

        private void btn_ok_Click(object sender, System.EventArgs e)
        {
            Session.Remove("ListaDocDir");
            //DataGrid1.Dispose();
            //Response.Write("<script>var k=window.opener.document.forms[0].submit(); window.close();</script>");
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

            //
            // Mev Editing ACL
            // Se ho cliccato il cestino per la rimozione massiva con almeno un elemento gestisco tale rimozione 
            if (!string.IsNullOrEmpty(rimozioneMassiva) && rimozioneMassiva.Equals("true"))
            {
                if (ContaElementiCheckedNotDeleted(TDNew) > 0)
                {
                    messaggio = "Questa operazione rimuove tutti i diritti di visibilità del documento per i seguenti ruoli: \\n";
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
                //if (((Label)DataGrid1.SelectedItem.Cells[2].Controls[1]).Text.Equals("PROPRIETARIO"))
                if (((Label)DataGrid1.SelectedItem.Cells[3].Controls[1]).Text.Equals("PROPRIETARIO"))
                    messaggio = InitMessageXml.getInstance().getMessage("RIMUOVI_DOCUMENTO_ACL_PROPR");
                else
                    messaggio = InitMessageXml.getInstance().getMessage("RIMUOVI_DOCUMENTO_ACL");
            }
            Msg_Rimuovi.Confirm(messaggio);

        }

        private void btn_annulla_Click(object sender, System.EventArgs e)
        {
            this.pnl_descRimuovi.Visible = false;
        }

        private void btn_storia_visibilita_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
           _idVisDoc = "reload";
            //string scriptString = "<SCRIPT>ApriFinestraStoriaVisibilita('D');</SCRIPT>";
            //this.RegisterStartupScript("apriModalDialogStoriaVisibilita", scriptString);
            ClientScript.RegisterStartupScript(this.GetType(), "storia", "ApriFinestraStoriaVisibilita('D');", true);
            //this.btn_storia_visibilita.Attributes.Add("onclick", "ApriFinestraStoriaVisibilita('D');return false;");

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

        private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            this.lb_dettagli.Visible = false;
            this.DataGrid1.SelectedIndex = e.Item.ItemIndex;
            if (e.CommandName.Equals("Rimuovi"))
            {
                //string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_note.ID + "').focus() </SCRIPT>";
                //RegisterStartupScript("focus", s);
                ClientScript.RegisterStartupScript(this.GetType(), "focus", "document.getElementById('" + txt_note.ID + "').focus();", true);
                pnl_descRimuovi.Visible = true;
                txt_note.Text = "";
            }
            if (e.CommandName.Equals("Ripristina"))
            {
                RipristinaACL();
                pnl_descRimuovi.Visible = false;
            }
        }

        private void RimuoviACL()
        {
            //Verifica se si vuole rimuovere acl a un ruolo o un utente
            string personOrGroup = "R";
            //
            // Mev Editing ACL
            // Poichè è stata aggiunta la colonna Checkbox, occorre incrementare gli indici delle celle/colonne
            //
            //if (((Label)DataGrid1.SelectedItem.Cells[5].Controls[1]).Text.Equals("UTENTE"))
            if (((Label)DataGrid1.SelectedItem.Cells[6].Controls[1]).Text.Equals("UTENTE"))
                personOrGroup = "U";

            ListaDocDir = (DocsPAWA.DocsPaWR.DocumentoDiritto[])Session["ListaDocDir"];
            //DocsPAWA.DocsPaWR.DocumentoDiritto docDiritti = ListaDocDir[DataGrid1.SelectedIndex];
            DocsPAWA.DocsPaWR.DocumentoDiritto docDiritti = ListaDocDir[DataGrid1.SelectedItem.DataSetIndex];
            docDiritti.personorgroup = personOrGroup;
            docDiritti.note = this.txt_note.Text;
                    
            bool result = DocumentManager.editingACL(docDiritti, personOrGroup, UserManager.getInfoUtente(this));
            if (result)
            {
                _idVisDoc = "reload";
                Page_Load(null, new ModifyACL());
                DataGrid1.SelectedIndex = -1;
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
            //
            // Mev Editing ACL
            // Poichè è stata aggiunta la colonna Checkbox, occorre incrementare gli indici delle celle/colonne
            //
            //if (((Label)DataGrid1.SelectedItem.Cells[5].Controls[1]).Text.Equals("UTENTE"))
            if (((Label)DataGrid1.SelectedItem.Cells[6].Controls[1]).Text.Equals("UTENTE"))
                personOrGroup = "U";

            //chiamata al WebService per la rimozione dalla deleted_security
            //inserimento del record nella security
            ListaDocDir = (DocsPAWA.DocsPaWR.DocumentoDiritto[])Session["ListaDocDir"];
            DocsPAWA.DocsPaWR.DocumentoDiritto docDiritti = ListaDocDir[DataGrid1.SelectedItem.DataSetIndex];
            bool result = DocumentManager.ripristinaACL(docDiritti, personOrGroup, UserManager.getInfoUtente(this));
            if (result)
            {
                _idVisDoc = "reload";
                Page_Load(null, new ModifyACL());
                DataGrid1.SelectedIndex = -1;
            }
            else
            {
                //Response.Write("<script>alert('Attenzione operazione non riuscita');</script>");
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Attenzione operazione non riuscita');", true);
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

                if(!string.IsNullOrEmpty(rimozioneMassiva) && rimozioneMassiva.Equals("true"))
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

        //
        // MEV Editing ACL
        
        //
        // Funzione per cliccare tutte le checkBox
        // Viene richiamata sul click della checkbox Seleziona/deseleziona Tutti
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
        // End Funzione
        //

        //
        // Metodo per la rimozione massiva
        // Invocato all onClick del cestino
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

            // Inserisco in sessione il valore del flag per la rimozione Massiva
            Session["rimozioneMassiva"] = rimozioneMassiva;
            Session["pnl_descRimuovi.Visible"] = pnl_descRimuovi.Visible;
        }

        
        //
        // Metodo per il ripristino massivo
        // invocato al clic del ripristino massivo
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
                    messaggio = "Questa operazione ripristina tutti i diritti di visibilità del documento per i seguenti ruoli: \\n";
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
        // End Metodo rimozione massiva
        //

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
            
            for(int i =0; i<TDNew.Rows.Count; i++)
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
        /// Metodo per la rimozione massiva degli elementi in ACL
        /// </summary>
        private void RimuoviACLMassiva(DataTable dt)
        {
            //prendo dalla sessione ListaDocDir
            ListaDocDir = (DocsPAWA.DocsPaWR.DocumentoDiritto[])Session["ListaDocDir"];

            // Lista elementi Non rimossi
            List<string> listNotRemoved = new List<string>();

            // Lista elementi rimossi
            List<string> listRemoved = new List<string>();
          
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
                        
                        for (int j =0;j<ListaDocDir.Length; j++)
                        {
                            //if(ListaDocDir[j].personorgroup.Equals(idCorr) && !ListaDocDir[j].removed.Equals("1"))
                            if (ListaDocDir[j].personorgroup.Equals(idCorr)
                                && setTipoDiritto(ListaDocDir[j]).ToString().Equals(diritti)
                                && this.GetRightDescription(ListaDocDir[j].accessRights).ToString().Equals(tipodiritto)
                                && !ListaDocDir[j].deleted
                                )
                                indice = j;
                        }

                        DocsPAWA.DocsPaWR.DocumentoDiritto docDiritti = ListaDocDir[indice];
                        docDiritti.personorgroup = personOrGroup;
                        docDiritti.note = this.txt_note.Text;

                        bool result = DocumentManager.editingACL(docDiritti, personOrGroup, UserManager.getInfoUtente(this));

                        if (result)
                        {
                            // OK
                            // Attualmente non utilizzata
                            // Utilizzabile nell'eventualità di un reoprt
                            listRemoved.Add(dt.Rows[i]["ruolo"].ToString());
                        }
                        else
                        {
                            //Popolo una lista di stringhe con gli elementi che non sono stati rimossi dalla ACL
                            listNotRemoved.Add(dt.Rows[i]["ruolo"] + "\\n");
                        }
                    }

                }
            }

            //Session["Dg_visibilita"] = dt;
            pnl_descRimuovi.Visible = false;

            
            // Resoconto degli elemneti non andati a buon fine
            if (listNotRemoved != null && listNotRemoved.Count>0) 
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
            _idVisDoc = "reload";
            Page_Load(null, new ModifyACL());
            DataGrid1.SelectedIndex = -1;
        }


        /// <summary>
        /// Metodo per il ripristino massiva degli elementi in ACL
        /// </summary>
        private void RipristinaACLMassiva(DataTable dt)
        {
            //prendo dalla sessione ListaDocDir
            ListaDocDir = (DocsPAWA.DocsPaWR.DocumentoDiritto[])Session["ListaDocDir"];

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

                        for (int j = 0; j < ListaDocDir.Length; j++)
                        {
                            //if (ListaDocDir[j].personorgroup.Equals(idCorr) && !ListaDocDir[j].rimosso)
                            //if (ListaDocDir[j].personorgroup.Equals(idCorr) && ListaDocDir[j].removed.Equals("1"))
                            if (ListaDocDir[j].personorgroup.Equals(idCorr)
                                && setTipoDiritto(ListaDocDir[j]).ToString().Equals(diritti)
                                && this.GetRightDescription(ListaDocDir[j].accessRights).ToString().Equals(tipodiritto)
                                && ListaDocDir[j].deleted
                                )
                                indice = j;
                        }

                        DocsPAWA.DocsPaWR.DocumentoDiritto docDiritti = ListaDocDir[indice];
                        
                        //Chiamata per il ripristino
                        bool result = DocumentManager.ripristinaACL(docDiritti, personOrGroup, UserManager.getInfoUtente(this));
                        
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

            //Session["Dg_visibilita"] = dt;
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
            _idVisDoc = "reload";
            Page_Load(null, new ModifyACL());
            DataGrid1.SelectedIndex = -1;
        }

        // End MEV
        //

    }

    class ModifyACL : EventArgs
    { 
    }
}
