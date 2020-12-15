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

namespace DocsPAWA.popup
{
    /// <summary>
    /// Summary description for oggettario.
    /// </summary>
    public class oggettario : DocsPAWA.CssPage
    {
        //protected System.Web.UI.WebControls.TextBox TextOggetto;
        //protected System.Web.UI.WebControls.ListBox ListOggetti;
        protected System.Web.UI.WebControls.Label LabelOggetto;
        protected System.Web.UI.WebControls.Label lblCombo;
        protected System.Web.UI.WebControls.Button btn_chiudi;
        protected System.Web.UI.WebControls.Button btn_ok;
        protected System.Web.UI.WebControls.Button btn_cerca;
        protected System.Web.UI.WebControls.ImageButton ImageButton1;
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.Button btn_aggiungi;
        protected System.Web.UI.WebControls.DataGrid dg_Oggetti;
        protected System.Web.UI.WebControls.DropDownList ddlRegRf;
        //protected System.Web.UI.WebControls.Panel pnlCombo;
        protected System.Web.UI.WebControls.Label lbl_risultatoRicOgg;
        protected System.Web.UI.WebControls.Button btn_modifica;

        //protected System.Web.UI.HtmlControls.HtmlInputHidden hMOD_Authorized;
        //protected System.Web.UI.HtmlControls.HtmlInputHidden hINS_Authorized;
        //protected System.Web.UI.HtmlControls.HtmlInputHidden hCANC_Authorized;

        protected System.Web.UI.HtmlControls.HtmlInputText hMOD_Authorized;
        protected System.Web.UI.HtmlControls.HtmlInputText hINS_Authorized;
        protected System.Web.UI.HtmlControls.HtmlInputText hCANC_Authorized;
        protected DocsPAWA.documento.Oggetto ctrl_oggetto;
        protected System.Web.UI.WebControls.ImageButton btn_clear;
        
        protected bool codOgg_enable;

        //my var
        string wnd;
        protected ArrayList Dt_elem;
        protected DocsPAWA.DocsPaWR.Registro[] listaRF;

        //end my var

        protected bool _isMOD_Authorized;
        protected bool _isINS_Authorized;
        protected bool _isCANC_Authorized;

        private void Page_PreRender(object sender, System.EventArgs e)
        {

        }

        private void Page_Load(object sender, System.EventArgs e)
        {



            //Inizializzazione user control Oggetto
            ctrl_oggetto = this.GetControlOggetto();
            //disattivo la ricerca sul codice oggetto perchè qui c'è il bottone cerca!!!
            ctrl_oggetto.cod_oggetto_postback = false;
            //questa è l'abilitazione da Web.config del codice oggetto
            codOgg_enable = ctrl_oggetto.UseCodiceOggetto;
            //Imposto l'aspetto del controllo oggetto
            ctrl_oggetto.DimensioneOggetto("estesa", "oggettario");


            //Verifica se l'utente è abilitato alla funzione di editing ACL

            this.hMOD_Authorized.Value = UserManager.ruoloIsAutorized(this, "DO_MODIFICA_OGGETTARIO").ToString();
            this.hINS_Authorized.Value = UserManager.ruoloIsAutorized(this, "DO_INSERISCI_OGGETTARIO").ToString();
            this.hCANC_Authorized.Value = UserManager.ruoloIsAutorized(this, "DO_CANCELLA_OGGETTARIO").ToString();
            //richiamare la giusta funzione java script che valorizza il campo oggetto della pagina chiamante
            try
            {
                wnd = Request.QueryString["wnd"];

                if (!Page.IsPostBack)
                {
                    //this.pnlCombo.Visible = false;
                    this.lblCombo.Visible = false;
                    this.ddlRegRf.Visible = false;
                    caricaRegistriRFDisponibili();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.OpenErrorPage(this, ex, "Ricerca/Inserimento in oggettario");
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
            this.btn_cerca.Click += new System.EventHandler(this.btn_cerca_Click);
            this.btn_aggiungi.Click += new System.EventHandler(this.btn_aggiungi_Click);
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            this.btn_modifica.Click += new System.EventHandler(this.btn_modifica_Click);
            this.btn_clear.Click += new System.Web.UI.ImageClickEventHandler(this.btn_clear_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(this.Page_PreRender);
        }
        #endregion

        private void btn_ok_Click(object sender, System.EventArgs e)
        {
            string desc;
            Session.Add("oggettario.retValue", true);
            DocsPaWR.SchedaDocumento schedaDocumento;
            DocsPaWR.Oggetto oggettoDoc = new DocsPAWA.DocsPaWR.Oggetto();
            int selIndex = dg_Oggetti.SelectedIndex;

            if (selIndex > -1 && selIndex < this.dg_Oggetti.Items.Count)
            {
                oggettoDoc.systemId = ((Label)this.dg_Oggetti.Items[selIndex].Cells[2].Controls[1]).Text;
                oggettoDoc.descrizione = ((Label)this.dg_Oggetti.Items[selIndex].Cells[1].Controls[1]).Text;
                oggettoDoc.daAggiornare = true;

                string codOgg = ((Label)this.dg_Oggetti.Items[selIndex].Cells[0].Controls[1]).Text;
                if (codOgg != null && codOgg != string.Empty)
                {
                    oggettoDoc.codOggetto = codOgg;
                }
            }

            //in questo caso può leggere anche dalla textBox ed eventualmente modificare l'oggetto!!!
            string msg;

            //controllo sull'inserimento dell'oggetto
            if ((this.ctrl_oggetto.oggetto_text.Equals("") || this.ctrl_oggetto.oggetto_text == null) && (oggettoDoc.descrizione == null || oggettoDoc.descrizione == ""))
            {
                msg = "Selezionare un oggetto";
                Response.Write("<script>alert('" + msg + "');</script>");
                return;
            }

            //controllo sulla lunghezza dell'oggetto (max 2000 car.)
            if (this.ctrl_oggetto.oggetto_text.Length > 2000)
            {
                msg = "La lunghezza massima del campo oggetto non deve superare i 2000 caratteri.";
                ctrl_oggetto.oggetto_SetControlFocus();
                return;
            }

            if (ctrl_oggetto.oggetto_text != null && ctrl_oggetto.oggetto_text != "")
            {
                //replace dell'apice singolo
                oggettoDoc.descrizione = this.ctrl_oggetto.oggetto_text;
            }

            //Aggiunta del codice oggetto se esiste!!!
            if (ctrl_oggetto.cod_oggetto_text != null && ctrl_oggetto.cod_oggetto_text != string.Empty)
            {
                oggettoDoc.codOggetto = ctrl_oggetto.cod_oggetto_text;
            }
            else
            {
                //se è null lo pongo uguale alla stringa vuota per svuotare il campo codice oggetto!!!
                if (oggettoDoc.codOggetto == null && codOgg_enable)
                {
                    oggettoDoc.codOggetto = "";
                }
            }

            desc = oggettoDoc.descrizione;
            if (desc != null)
            {
                desc = desc.Replace("'", @"\'").Replace("\"", "\\\"").Replace("\n", @"\n");
                desc = desc.Replace("\r", @"\r").Replace("\t", @"\t");
            }
            else
                desc = "";
            if (wnd != null && (wnd.Equals("proto") || wnd.Equals("doc_Prof") || wnd.Equals("protoSempl")))
                if (!wnd.Equals("protoSempl"))
                {
                    schedaDocumento = (DocsPAWA.DocsPaWR.SchedaDocumento)Session["tabDoc.schedaDocumento"];
                    schedaDocumento.oggetto = oggettoDoc;
                    Session["tabDoc.schedaDocumento"] = schedaDocumento;
                    Session.Remove("saveButtonEnabled");
                    Session.Add("saveButtonEnabled", true);

                    DocumentManager.setDocumentoInLavorazione(schedaDocumento);
                    Response.Write("<script>var k=window.opener.document.forms[0].submit(); window.close();</script>");

                    //string fld_oggetto = (wnd == "doc_Prof") ? "txt_oggetto" : "txt_oggetto_P";
                    //Response.Write("<script>window.opener.document.getElementById('" + fld_oggetto + "').value='" + desc + "'; var k=window.opener.document.forms[0].submit(); window.close();</script>");
                }
                else
                {
                    ProtocollazioneIngresso.Protocollo.ProtocolloMng protoMng = new ProtocollazioneIngresso.Protocollo.ProtocolloMng(this);
                    DocsPAWA.DocsPaWR.SchedaDocumento currentDocument = protoMng.GetDocumentoCorrente();
                    currentDocument.oggetto = oggettoDoc;
                    Response.Write("<script>window.close();</script>");
                }
            if (wnd == "ric_E" || wnd == "ric_C" || wnd == "ric_CT" || wnd == "ric_G")
            {
                string nomeFunc = "setOggetto('" + wnd + "','" + desc + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "setOggetto", nomeFunc.ToString(), true);
            }
            if (wnd == "ric_Classifica")
            {
                Response.Write("<script>window.returnValue = '" + desc + "'; window.close();</script>");
            }

        }

        private void btn_clear_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.ctrl_oggetto.oggetto_text = "";
            this.ctrl_oggetto.cod_oggetto_text = "";
            this.ddlRegRf.SelectedIndex = 0;
        }

        private void btn_cerca_Click(object sender, System.EventArgs e)
        {
            ricerca();
        }
        private void ricerca()
        {
            DocsPaWR.Ruolo userRuolo = UserManager.getRuolo(this);
            string[] listaReg = null;

            if (wnd.StartsWith("proto")) //se vengo da PROTOCOLLO O PROTOCOLLO SEMPLIFICATO
            {
                if (ddlRegRf.SelectedValue == "")
                {
                    if (wnd == "proto")
                    {
                        //registro in sessione
                        listaReg = UserManager.getListaIdRegistri(this);
                    }
                    else
                    {
                        //se vengo dal protocollo Semplificato
                        if (wnd == "protoSempl")
                        {
                            ProtocollazioneIngresso.Registro.RegistroMng regMng = new ProtocollazioneIngresso.Registro.RegistroMng(this);
                            listaReg = new String[1];
                            listaReg[0] = regMng.GetRegistroCorrente().systemId;
                        }
                    }
                    //se cerco nel registro, cerco anche negli RF associati
                    caricaListaRF(ref listaReg, ddlRegRf, "");
                }
                else
                {
                    //ho specificato un RF, quindi cerco solo su quello
                    char[] sep = { '_' };
                    string[] datiSelezione = ddlRegRf.SelectedValue.Split(sep);

                    listaReg = Utils.addToArrayString(listaReg, datiSelezione[0]);

                    //listaReg = Utils.addToArrayString(listaReg, ddlRegRf.SelectedValue.Substring(0, ddlRegRf.SelectedValue.LastIndexOf("_")));
                }
            }
            else
            {
                if (ddlRegRf.SelectedValue == "")
                {
                    /* 
                       L'UTENTE HA SELEZIONATO LA VOCE "Tutti". In tal caso la ricerca dell'oggetto
                       verrà effettuata tra:
                       - tutti gli oggetti associati a tutti gli RF visibili al ruolo;
                       - tutti gli oggetti associati a tutti i registri visibili al ruolo; 
                       - tra tutti gli oggetti con registro NULL 
                    */
                    //for (int i = 0; i < userRuolo.registri.Length; i++)
                    //{
                    //    listaReg = Utils.addToArrayString(listaReg, userRuolo.registri[i].systemId);
                    //}

                    caricaListaRF(ref listaReg, ddlRegRf, "");
                    //manca il filtroo su TUTTI
                    listaReg = Utils.addToArrayString(listaReg, "");

                }
                else
                {
                    /* 
                       L'UTENTE HA SELEZIONATO UNA VOCE DIVERSA DA "Tutti". 
                     
                     Se è stato selezionato:
                     
                     - un registro: si ricerca nel registro e negli RF associati al esso
                     - un RF: si ricerca nell'RF corrente
                     
                    */

                    char[] sep = { '_' };
                    string[] datiSelezione = ddlRegRf.SelectedValue.Split(sep);

                    //aggiungo la selezione corrente
                    listaReg = Utils.addToArrayString(listaReg, datiSelezione[0]);

                    //se è un registro, devo aggiingere anche gli RF associati al registro
                    if (datiSelezione[1] == string.Empty && datiSelezione[2] == string.Empty)
                    {
                        caricaListaRF(ref listaReg, ddlRegRf, datiSelezione[0]);
                    }

                }

            }

            DocsPaWR.Oggetto[] listaObj;
            if (ctrl_oggetto.cod_oggetto_text != null && ctrl_oggetto.cod_oggetto_text != string.Empty && codOgg_enable)
            {
                //abilito la ricerca con il LIKE aggiungendo i seguenti caratteri speciali $@
                listaObj = DocumentManager.getListaOggettiByCod(this, listaReg, this.ctrl_oggetto.oggetto_text, ("$@" + this.ctrl_oggetto.cod_oggetto_text));
            }
            else
            {
                listaObj = DocumentManager.getListaOggetti(this, listaReg, this.ctrl_oggetto.oggetto_text);
            }

            this.lbl_risultatoRicOgg.Visible = false;

            //riempo il datagrid
            if (listaObj != null)
            {
                //Costruisco il datagrid
                Dt_elem = new ArrayList();
                for (int i = 0; i < listaObj.Length; i++)
                    Dt_elem.Add(new Cols(listaObj[i].codOggetto, listaObj[i].descrizione, listaObj[i].systemId, i, listaObj[i].codRegistro));

                if (codOgg_enable)
                {
                    this.dg_Oggetti.Columns[0].Visible = true;
                }

                if (listaObj.Length > 0)
                {
                    this.dg_Oggetti.DataSource = Dt_elem;
                    this.dg_Oggetti.DataBind();
                    this.dg_Oggetti.Visible = true;
                }
                else
                {

                    this.dg_Oggetti.Visible = false;
                    this.lbl_risultatoRicOgg.Visible = true; // nessun oggetto presente
                }
            }
            else
            {
                this.dg_Oggetti.Visible = false;
                this.lbl_risultatoRicOgg.Visible = true; // nessun oggetto presente

            }

        }


        /// <summary>
        /// carica gli RF o i registri o entrambi (anche gli RF disabled)
        /// </summary>
        /// <param name="listaReg">Array di stringhe contenente i registri/rf nel quale cercare l'oggetto</param>
        /// <param name="ddlReg">combo dalla quale si caricano i registri</param>
        /// <param name="idRegistro">se popolato, si caricano solamente gli RF che hanno come AOO collegata quella systemId</param>
        /// <returns></returns>
        private string[] caricaListaRF(ref string[] listaReg, DropDownList ddlReg, string idRegistro)
        {

            foreach (ListItem item in ddlReg.Items)
            {
                if (item.Value != "")
                {
                    char[] sep = { '_' };
                    string[] currentSelection = item.Value.Split(sep);

                    if (idRegistro != "")
                    {
                        //devo cercare gli RF associati al registro in input allora
                        //aggiungo alla lista di registri solamente quegli RF la cui AooCollegata coicide con
                        //la systemId del registro

                        if (idRegistro == currentSelection[2])
                        {
                            listaReg = Utils.addToArrayString(listaReg, currentSelection[0]);
                        }
                    }
                    else
                    {
                        listaReg = Utils.addToArrayString(listaReg, currentSelection[0]);
                    }
                }
            }
            return listaReg;

        }

        private void btn_aggiungi_Click(object sender, System.EventArgs e)
        {
            try
            {
                DocsPaWR.Oggetto oggetto = new DocsPAWA.DocsPaWR.Oggetto();
                DocsPaWR.Registro registro = new DocsPAWA.DocsPaWR.Registro();


                string msg;

                //controllo sull'inserimento dell'oggetto
                if (this.ctrl_oggetto.oggetto_text.Equals("") || this.ctrl_oggetto.oggetto_text == null)
                {
                    msg = "Inserire il valore: oggetto";
                    Response.Write("<script>alert('" + msg + "');</script>");
                    return;
                }

                //controllo sulla lunghezza dell'oggetto (max 2000 car.)
                if (this.ctrl_oggetto.oggetto_text.Length > 2000)
                {
                    msg = "La lunghezza massima del campo oggetto non deve superare i 2000 caratteri.";
                    ctrl_oggetto.oggetto_SetControlFocus();
                    return;
                }

                //replace dell'apice singolo
                oggetto.descrizione = this.ctrl_oggetto.oggetto_text;

                //Aggiunta del codice oggetto se esiste!!!
                if (ctrl_oggetto.cod_oggetto_text != null && ctrl_oggetto.cod_oggetto_text != string.Empty)
                {
                    oggetto.codOggetto = ctrl_oggetto.cod_oggetto_text;
                }
                else
                {
                    oggetto.codOggetto = "";
                }

                //if (wnd == "proto" || wnd == "protoSempl")
                //{
                if (ddlRegRf.SelectedItem.Value == "")
                {
                    if (wnd == "proto") // protocollo
                    {
                        registro = UserManager.getRegistroSelezionato(this);
                    }
                    else
                    {
                        if (wnd == "protoSempl") // protocollo semplificato
                        {
                            ProtocollazioneIngresso.Registro.RegistroMng regMng = new ProtocollazioneIngresso.Registro.RegistroMng(this);
                            registro = regMng.GetRegistroCorrente();
                        }
                        else
                        {
                            registro = null; // ricerche e profilo
                        }

                    }
                }
                else
                {
                    char[] sep = { '_' };
                    string[] datiSelezione = ddlRegRf.SelectedValue.Split(sep);
                    registro.systemId = datiSelezione[0];
                    registro.codice = ddlRegRf.SelectedItem.Text;
                }

                //DocsPaWR.DocsPaWebService wws = new DocsPaWR.DocsPaWebService();
                //DocsPaWR.Registro reg = wws.GetRegistroBySistemId(registro.systemId);
                if (registro != null && registro.Sospeso)
                {
                    RegisterClientScriptBlock("alertRegistroSospeso", "alert('Il registro selezionato è sospeso!');");
                    return;
                }

                string errMsg = "";
                oggetto = DocumentManager.addOggetto(this, oggetto, registro, ref errMsg);
                if (oggetto != null)
                {
                    ricerca();
                }
                else
                {
                    if (codOgg_enable && ctrl_oggetto.cod_oggetto_text != null && !ctrl_oggetto.cod_oggetto_text.Equals(""))
                    {
                        Response.Write("<script>alert('Oggetto o codice oggetto già presenti');</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Oggetto già presente');</script>");
                    }
                }

            }
            catch (Exception es)
            {
                ErrorManager.redirectToErrorPage(this, es);
            }
        }

        protected void dg_Oggetti_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            DocsPaWR.Oggetto oggettoDoc = new DocsPAWA.DocsPaWR.Oggetto();
            this.dg_Oggetti.SelectedIndex = e.Item.ItemIndex;
            int selIndex = this.dg_Oggetti.SelectedIndex;

            if (selIndex > -1)
            {
                oggettoDoc.systemId = ((Label)this.dg_Oggetti.Items[selIndex].Cells[2].Controls[1]).Text;
                oggettoDoc.descrizione = ((Label)this.dg_Oggetti.Items[selIndex].Cells[1].Controls[1]).Text;
                oggettoDoc.daAggiornare = true;

                string codOgg = ((Label)this.dg_Oggetti.Items[selIndex].Cells[0].Controls[1]).Text;
                if (codOgg != null && codOgg != string.Empty)
                {
                    oggettoDoc.codOggetto = codOgg;
                }
            }

            try
            {
                if (e.CommandName.ToLower() == "delete")
                {
                    bool result = DocumentManager.cancellaOggetto(this, oggettoDoc);
                    if (result)
                    {
                        ricerca();
                    }
                    else
                    {
                        Response.Write("<script>alert('Non è stato possibile effettuare la rimozione oggetto');</script>");
                    }
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirectToErrorPage(this, es);
            }
        }

        protected void dg_Oggetti_ItemSelected(object sender, System.EventArgs e)
        {
            DocsPaWR.Oggetto oggettoDoc = new DocsPAWA.DocsPaWR.Oggetto();
            //this.dg_Oggetti.SelectedIndex = e.Item.ItemIndex;
            int selIndex = this.dg_Oggetti.SelectedIndex;

            if (selIndex > -1)
            {
                oggettoDoc.systemId = ((Label)this.dg_Oggetti.Items[selIndex].Cells[2].Controls[1]).Text;
                oggettoDoc.descrizione = ((Label)this.dg_Oggetti.Items[selIndex].Cells[1].Controls[1]).Text;
                oggettoDoc.daAggiornare = true;

                string codOgg = ((Label)this.dg_Oggetti.Items[selIndex].Cells[0].Controls[1]).Text;
                if (codOgg != null && codOgg != string.Empty)
                {
                    oggettoDoc.codOggetto = codOgg;
                }
            }

            if (oggettoDoc.codOggetto != null && oggettoDoc.codOggetto != string.Empty)
            {
                ctrl_oggetto.cod_oggetto_text = oggettoDoc.codOggetto;
            }
            else
            {
                if (codOgg_enable)
                {
                    ctrl_oggetto.cod_oggetto_text = "";
                }
            }
            ctrl_oggetto.oggetto_text = oggettoDoc.descrizione;

            string regSel = ((Label)this.dg_Oggetti.Items[selIndex].Cells[3].Controls[1]).Text;
            //rf, nella ddl dei registri hanno degli spazi prima del nome, quindi se
            //non si aggiungono al rf da cercare il FindByText non trova nulla
            if (ddlRegRf.Items.FindByText(regSel) == null)
            {
                string strText = InserisciSpazi();
                regSel = strText + regSel;
            }
            ddlRegRf.SelectedIndex = ddlRegRf.Items.IndexOf(ddlRegRf.Items.FindByText(regSel));
        }

        public class Cols
        {
            private string codice;
            private string descrizione;
            private int page;
            private int chiave;
            private string codRegistro;
            private string codOggetto;

            public Cols(string codOggetto, string descrizione, string codice, int chiave, string codRegistro)
            {
                this.codOggetto = codOggetto;
                this.descrizione = descrizione;
                this.codice = codice;
                this.chiave = chiave;
                this.codRegistro = codRegistro;
            }

            public string CodOggetto { get { return codOggetto; } }
            public string Descrizione { get { return descrizione; } }
            public string Codice { get { return codice; } }
            public int Chiave { get { return chiave; } }
            public string CodRegistro { get { return codRegistro; } }

        }

        /// <summary>
        /// Ritorna tutti i registri e gli rf associati a un ruolo, o solamente gli RF o solamente i registri
        /// in base al valore dello string all
        /// se all="" ritorna tutto (sia registri che rf)
        /// se all="0" ritorna solo i registri
        /// se all="1" ritorna solo gli RF
        /// </summary>
        /// <param name="all"></param>
        //private void caricaRegistriRFDisponibili(string all, string idAooColl)
        private void caricaRegistriRFDisponibili()
        {
            //il pannello è visibile solo se il ruolo vede almeno un RF

            DocsPaWR.Registro reg = null;

            //prendo tutti i registri
            DocsPaWR.Registro[] listaRegistri = UserManager.getListaRegistriWithRF(this, "0", "");

            string labelCombo;
            if (listaRegistri != null && listaRegistri.Length > 0)
            {

                //importante. I value della combo dei registri sono formati da terne separate dal carattere "_":
                //nella prima posizione viene specificata la systemId del registro o dell'RF
                //nella seconda posizione viene specificato un valore popolato solo per gli RF:
                //  - "" per i registri
                //  - "0" per gli RF abilitati
                //  - "1" per gli RF non abilitati
                //nella terza posizione viene specificato l'IdAooCollegata (solo per gli RF)

                #region commento
                //for (int i = 0; i < listaRF.Length; i++)
                //{
                //    this.ddlRegRf.Items.Add((listaRF[i]).codRegistro);

                //    this.ddlRegRf.Items[i].Value = (listaRF[i]).systemId + "_" + listaRF[i].rfDisabled + "_" + listaRF[i].idAOOCollegata;         

                //}
                #endregion

                //this.pnlCombo.Visible = true;
                this.lblCombo.Visible = true;
                this.ddlRegRf.Visible = true;

                if (wnd == "proto" || wnd == "protoSempl") // caso di protocollo e protocollo semplificato
                {
                    if (wnd == "proto")
                        reg = UserManager.getRegistroSelezionato(this);
                    else
                    {
                        ProtocollazioneIngresso.Registro.RegistroMng regMng = new ProtocollazioneIngresso.Registro.RegistroMng(this);

                        reg = regMng.GetRegistroCorrente();
                    }


                    int l = -1;
                    for (int i = 0; i < listaRegistri.Length; i++)
                    {
                        if (listaRegistri[i].systemId == reg.systemId)//aggiungo il registro solo se coincide con quello di protocollo
                        {
                            l++;
                            this.ddlRegRf.Items.Add((listaRegistri[i]).codRegistro);

                            //this.ddlRegRf.Items[l].Value = (listaRegistri[i]).systemId + "_" + listaRegistri[i].rfDisabled + "_" + listaRegistri[i].idAOOCollegata;
                            this.ddlRegRf.Items[l].Value = "";
                            this.ddlRegRf.Items[l].Selected = true;

                            //prendo gli RF di ciascun registro
                            listaRF = UserManager.getListaRegistriWithRF(this, "1", (listaRegistri[i]).systemId);

                            if (listaRF != null && listaRF.Length > 0)
                            {
                                foreach (DocsPaWR.Registro currReg in listaRF)
                                {

                                    string strText = InserisciSpazi();
                                    this.ddlRegRf.Items.Add(strText + currReg.codRegistro);
                                    l++;
                                    //this.ddlRegRf.Items[l].Text = strText + currReg.codRegistro;
                                    this.ddlRegRf.Items[l].Value = currReg.systemId + "_" + currReg.rfDisabled + "_" + currReg.idAOOCollegata;
                                    //this.ddlRegRf.Items[l].Attributes.Add("style", " color:Gray");

                                }
                            }
                            else
                            {
                                //se non ci sono RF associati al registro di protocollo rendo invisibile il pannello
                                //this.pnlCombo.Visible = false;
                                this.lblCombo.Visible = false;
                                this.ddlRegRf.Visible = false;
                            }
                        }
                    }

                }
                else // caso di documenti grigi e ricerche
                {
                    ListItem item = new ListItem();
                    item.Text = "TUTTI";
                    item.Value = "";
                    item.Selected = true;
                    this.ddlRegRf.Items.Add(item);

                    int l = 0;
                    for (int i = 0; i < listaRegistri.Length; i++)
                    {
                        //carico nella combo tutti i registri che il ruolo può vedere
                        l++;
                        this.ddlRegRf.Items.Add((listaRegistri[i]).codRegistro);
                        this.ddlRegRf.Items[l].Value = (listaRegistri[i]).systemId + "_" + listaRegistri[i].rfDisabled + "_" + listaRegistri[i].idAOOCollegata;


                        //prendo gli RF di ciascun registro
                        listaRF = UserManager.getListaRegistriWithRF(this, "1", (listaRegistri[i]).systemId);

                        if (listaRF != null && listaRF.Length > 0)
                        {
                            foreach (DocsPaWR.Registro currReg in listaRF)
                            {
                                string strText = InserisciSpazi();
                                this.ddlRegRf.Items.Add(strText + currReg.codRegistro);
                                l++;
                                this.ddlRegRf.Items[l].Value = currReg.systemId + "_" + currReg.rfDisabled + "_" + currReg.idAOOCollegata;
                                //this.ddlRegRf.Items[l].Attributes.Add("style", " color:Gray");
                            }
                        }
                    }
                }




            }
            else
            {
                throw new Exception();
            }

        }

        protected Hashtable ordinaCombo(DocsPaWR.Registro[] listaReg)
        {
            Hashtable hashRF = new Hashtable();
            System.Collections.ArrayList al = new ArrayList();

            foreach (DocsPaWR.Registro registro in listaReg)
            {
                //se è un registro
                //if (registro.chaRF == "0")
                //{
                //    if (!hashRegistri.ContainsKey(registro.systemId))
                //    {
                //        al = new System.Collections.ArrayList();
                //        al.Add(registro);

                //        hashRegistri.Add(registro.systemId, al);
                //    }
                //}
                //else
                //{
                if (registro.chaRF == "1")
                {
                    if (!hashRF.ContainsKey(registro.idAOOCollegata))
                    {
                        al = new System.Collections.ArrayList();
                        al.Add(registro);

                        hashRF.Add(registro.idAOOCollegata, al);
                    }
                    else
                    {
                        ((System.Collections.ArrayList)hashRF[registro.idAOOCollegata]).Add(registro);
                    }
                }
                //}   
            }

            return hashRF;

        }

        protected void dg_Oggetti_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                if (UserManager.ruoloIsAutorized(this, "DO_CANCELLA_OGGETTARIO"))
                {
                    dg_Oggetti.Columns[5].Visible = true;
                }
                else
                {
                    dg_Oggetti.Columns[5].Visible = false;
                }
            }
            if (e.Item.ItemType == ListItemType.Header)
            {

                if (e.Item.Cells.Count > 0)
                {
                    if (wnd == "proto" || wnd == "protoSempl")
                    {
                        if (ddlRegRf.Items.Count > 1)
                        {
                            e.Item.Cells[2].Text = "Registro/Rf";
                        }
                        else
                        {
                            e.Item.Cells[2].Text = "Registro";
                        }
                    }
                    else
                    {
                        if (ddlRegRf.Items.Count > 2)
                        {
                            e.Item.Cells[2].Text = "Registro/Rf";
                        }
                        else
                        {
                            e.Item.Cells[2].Text = "Registro";
                        }
                    }


                }

            }
        }

        //Ricerca dello user control Oggetto lato server
        protected DocsPAWA.documento.Oggetto GetControlOggetto()
        {
            return (DocsPAWA.documento.Oggetto)this.FindControl("ctrl_oggetto");
        }

        protected void btn_modifica_Click(object sender, EventArgs e)
        {
            try
            {
                DocsPaWR.Oggetto oggetto = new DocsPAWA.DocsPaWR.Oggetto();
                DocsPaWR.Registro registro = new DocsPAWA.DocsPaWR.Registro();
                DocsPaWR.Registro registroDoc = new DocsPAWA.DocsPaWR.Registro();

                string msg;

                //controllo sull'inserimento dell'oggetto
                if (this.ctrl_oggetto.oggetto_text.Equals("") || this.ctrl_oggetto.oggetto_text == null)
                {
                    msg = "Inserire il valore: oggetto";
                    Response.Write("<script>alert('" + msg + "');</script>");
                    return;
                }

                //controllo sulla lunghezza dell'oggetto (max 2000 car.)
                if (this.ctrl_oggetto.oggetto_text.Length > 2000)
                {
                    msg = "La lunghezza massima del campo oggetto non deve superare i 2000 caratteri.";
                    ctrl_oggetto.oggetto_SetControlFocus();
                    return;
                }

                //replace dell'apice singolo
                oggetto.descrizione = this.ctrl_oggetto.oggetto_text;

                //Aggiunta del codice oggetto se esiste!!!
                if (ctrl_oggetto.cod_oggetto_text != null && ctrl_oggetto.cod_oggetto_text != string.Empty)
                {
                    oggetto.codOggetto = ctrl_oggetto.cod_oggetto_text;
                }
                else
                {
                    oggetto.codOggetto = "";
                }


                #region old selezione registro
                ////if (wnd == "proto" || wnd == "protoSempl")
                ////{
                //if (ddlRegRf.SelectedItem.Value == "")
                //{
                //    if (wnd == "proto") // protocollo
                //    {

                //        registro = UserManager.getRegistroSelezionato(this);
                //    }
                //    else
                //    {
                //        if (wnd == "protoSempl") // protocollo semplificato
                //        {
                //            ProtocollazioneIngresso.Registro.RegistroMng regMng = new ProtocollazioneIngresso.Registro.RegistroMng(this);

                //            registro = regMng.GetRegistroCorrente();
                //        }
                //        else
                //        {
                //            registro = null; // ricerche e profilo
                //        }

                //    }
                //}
                //else
                //{
                //    char[] sep = { '_' };
                //    string[] datiSelezione = ddlRegRf.SelectedValue.Split(sep);
                //    registro.systemId = datiSelezione[0];
                //    registro.codice = ddlRegRf.SelectedItem.Text;
                //}
                #endregion

                DocsPaWR.Registro[] listaRegistri = UserManager.getListaRegistriWithRF(this, "", "");
                DocsPaWR.Oggetto oggettoDoc = new DocsPAWA.DocsPaWR.Oggetto();
                int selIndex = this.dg_Oggetti.SelectedIndex;

                string SelectedReg;
                if (selIndex > -1 && selIndex < this.dg_Oggetti.Items.Count)
                {
                    oggettoDoc.systemId = ((Label)this.dg_Oggetti.Items[selIndex].Cells[2].Controls[1]).Text;
                    oggettoDoc.descrizione = ((Label)this.dg_Oggetti.Items[selIndex].Cells[1].Controls[1]).Text;
                    oggettoDoc.daAggiornare = true;
                    //devo prendere il registro dell'oggetto selezionato per la modifica!!!

                    registroDoc.codice = ((Label)this.dg_Oggetti.Items[selIndex].Cells[3].Controls[1]).Text;
                    for (int regNum = 0; regNum < listaRegistri.Length; regNum++)
                    {
                        if (listaRegistri[regNum].codRegistro.ToLower() == registroDoc.codice.ToLower())
                        {
                            registroDoc.systemId = listaRegistri[regNum].systemId;

                            break;
                        }
                    }

                    //SelectedReg = ((Label)this.dg_Oggetti.Items[selIndex].Cells[3].Controls[1]).Text;
                    SelectedReg = this.ddlRegRf.SelectedItem.Text;
                    // if(SelectedReg.ToUpper()!= "TUTTI")

                    registro.codice = SelectedReg;
                    //recupero la system_id del registro da modificare
                    for (int regNum = 0; regNum < listaRegistri.Length; regNum++)
                    {
                        if (listaRegistri[regNum].codRegistro.Trim().ToLower() == SelectedReg.Trim().ToLower())
                        {
                            registro.systemId = listaRegistri[regNum].systemId;
                            //if (ddlRegRf != null)
                            //{
                            //    ddlRegRf.SelectedIndex = System.Convert.ToInt32(listaRegistri[regNum].systemId);
                            //}
                            break;
                        }
                    }

                    if (registro.systemId == null || registro.systemId.Equals(""))
                    {
                        registro = null;
                    }

                    string codOgg = ((Label)this.dg_Oggetti.Items[selIndex].Cells[0].Controls[1]).Text;
                    if (codOgg != null && codOgg != string.Empty)
                    {
                        oggettoDoc.codOggetto = codOgg;
                    }
                }
                else
                {
                    msg = "Selezionare un oggetto da modificare";
                    Response.Write("<script>alert('" + msg + "');</script>");
                    return;
                }

                // bool result = DocumentManager.ModificaOggetto(this, oggettoDoc, oggetto, registro,ref errMsg);

                string errMsg = "";
                bool result = DocumentManager.cancellaOggetto(this, oggettoDoc);
                oggetto = DocumentManager.addOggetto(this, oggetto, registro, ref errMsg);
                //Nel caso di mancato inserimento recupero dalla cancellazione


                if (errMsg != string.Empty)
                {
                    Response.Write("<script>alert('" + errMsg + "');</script>");
                    if (oggetto == null)
                    {
                        oggetto = DocumentManager.addOggetto(this, oggettoDoc, registroDoc, ref errMsg);
                    }
                    return;
                }
                if (oggetto == null)
                {
                    oggetto = DocumentManager.addOggetto(this, oggettoDoc, registroDoc, ref errMsg);
                }

                if (result && oggetto != null)
                {
                    ricerca();
                }
                else
                {
                    Response.Write("<script>alert('Non è stato possibile effettuare la modifica oggetto: verifica il codice e la descrizione oggetto');</script>");
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirectToErrorPage(this, es);
            }
        }


        //public bool isVisibleComboRegistri(DocsPaWR.Registro[] listaRegistriCombo)
        //{
        //    bool retValue = false; // false come se ci fosse un solo
        //    for (int i = 0; i < listaRegistriCombo.Length; i++)
        //    {
        //        if (listaRegistriCombo[i].chaRF == "1")
        //        { 

        //        }
        //    }

        //}

        private string InserisciSpazi()
        {
            string strText = "";
            for (short iff = 0; iff < 3; iff++)
            {
                strText += " ";
            }
            return strText;
        }
    }
}
