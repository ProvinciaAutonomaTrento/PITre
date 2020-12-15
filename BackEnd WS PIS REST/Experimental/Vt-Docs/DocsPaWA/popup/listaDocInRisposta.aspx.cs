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
using System.Collections.Generic;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{
    /// <summary>
    /// Summary description for listaDocInRisposta.
    /// </summary>
    public class listaDocInRisposta : DocsPAWA.CssPage
    {
        protected DocsPAWA.UserControls.AppTitleProvider appTitleProvider;
        protected System.Web.UI.WebControls.Label lbl_text;
        protected System.Web.UI.WebControls.Label lbl_message;
        protected System.Web.UI.WebControls.Button btn_chiudi;
        protected DocsPAWA.DocsPaWR.InfoDocumento[] infoDoc;
        protected System.Web.UI.WebControls.DataGrid DataGrid1;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        protected ArrayList Dg_elem;
        protected static int currentPage;
        protected int numTotPage;
        protected int nRec;
        protected string sys = "";
        protected string reg = "";
        protected string segn = "";
        protected string tipoProto = "";
        protected System.Web.UI.WebControls.Label lbl_titolo;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] ListaFiltri;
        protected string tempTipo = "";

        protected DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;
        protected string arrivo;
        protected string partenza;
        protected string interno;
        protected string grigio;

        protected System.Web.UI.WebControls.HiddenField hiddenType;

        private void Page_Load(object sender, System.EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                getParametersFromQueryString();
                currentPage = 1;
                //MODIFICA PER CONCATENAMENTO TRASVERSALE
                if (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
                                && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1")
                {
                    if (tipoProto.ToString().Equals("G"))
                    {
                        tempTipo = "G";
                    }
                    tipoProto = "T";
                }
                //FINE MODIFICA CONCATENAMENTO TRASVERSALE

                if (tipoProto.ToString().Equals("G"))
                {
                    appTitleProvider.PageName = "Documenti in risposta al documento";
                    lbl_text.Text = "Documenti in risposta al documento";
                    lbl_titolo.Text = lbl_titolo.Text + " " + sys;
                    if (GetFiltroDocGrigi(sys))
                    {
                        DocumentManager.setFiltroRicDoc(this, qV);
                        LoadData(true);
                        if (infoDoc == null || infoDoc.Length <= 0)
                        {
                            this.lbl_message.Text = "Documenti non trovati";
                            this.lbl_message.Visible = true;
                        }
                    }
                }
                else
                {
                    //MODIFICA INSERITA PER CONCATENAMENTO TRASVERSALE
                    if (tempTipo.Equals("G"))
                    {
                        appTitleProvider.PageName = "Documenti in risposta al documento";
                        lbl_text.Text = "Documenti in risposta al documento";
                        lbl_titolo.Text = lbl_titolo.Text + " " + sys;
                        if (GetFiltroDocGrigi(sys))
                        {
                            DocumentManager.setFiltroRicDoc(this, qV);
                            getLettereProtocolli();
                            LoadData(true);
                            if (infoDoc == null || infoDoc.Length <= 0)
                            {
                                this.lbl_message.Text = "Documenti non trovati";
                                this.lbl_message.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        appTitleProvider.PageName = "Documenti in risposta al protocollo";
                        lbl_text.Text = "Documenti in risposta al protocollo";
                        lbl_titolo.Text = lbl_titolo.Text + " " + segn;
                        if ((sys != null && !sys.ToString().Equals("")) && (reg != null && !reg.ToString().Equals("")) && (tipoProto != null && tipoProto.ToString() != String.Empty))
                        {
                            if (GetFiltroDocInRisposta(sys, reg, tipoProto))
                            {
                                DocumentManager.setFiltroRicDoc(this, qV);
                                getLettereProtocolli();
                                LoadData(true);
                                if (infoDoc == null || infoDoc.Length <= 0)
                                {
                                    this.lbl_message.Text = "Documenti non trovati";
                                    this.lbl_message.Visible = true;
                                }
                            }
                        }
                    }
                }
            } 
        }


        private void getParametersFromQueryString()
        {
            //sys = system_id del protocollo correntemente visualizzato
            if (Request.QueryString["sys"] != null)
                sys = Request.QueryString["sys"].ToString();
            //reg = registro del protocollo correntemente visualizzato
            if (Request.QueryString["regis"] != null)
                reg = Request.QueryString["regis"].ToString();
            //segn= segnatura del protocollo in arrivo
            if (Request.QueryString["seg"] != null)
                segn = this.Server.UrlDecode(Request.QueryString["seg"].ToString());
            //tipo del protocollo di cui si cercano le risposte (POSSIBILI VALORI "A" E "P")
            tipoProto = Request.QueryString["tipo"].ToString();
        }

        #region creazione Datagrid
        public class Cols
        {
            private string data;

            private string oggetto;
            private string mittDest;
            private string tipo;
            private string registro;
            private int chiave;
            private string segnatura;

            public Cols(string data, string oggetto, string mittDest, string tipo, string registro, int chiave, string segnatura)
            {
                this.data = data;
                this.oggetto = oggetto;
                this.tipo = tipo;
                this.registro = registro;
                this.mittDest = mittDest;
                this.chiave = chiave;
                this.segnatura = segnatura;
            }

            public string Data { get { return data; } }
            public string Oggetto { get { return oggetto; } }
            public string Tipo { get { return tipo; } }
            public string Registro { get { return registro; } }
            public string MittDest { get { return mittDest; } }
            public int Chiave { get { return chiave; } }
            public string Segnatura{ get { return segnatura;} }


        }
        #endregion

        #region caricamento Datagrid
        private void LoadData(bool updateGrid)
        {
            bool grigi = false;
            if (tipoProto.Equals("G"))
            {
                grigi = true;
            }
            DocsPaWR.InfoUtente infoUt = new DocsPAWA.DocsPaWR.InfoUtente();
            infoUt = UserManager.getInfoUtente(this);
            ListaFiltri = DocumentManager.getFiltroRicDoc(this);
            SearchResultInfo[] idProfiles;
            if (!UserManager.isFiltroAooEnabled(this))
                infoDoc = DocumentManager.getQueryInfoDocumentoPaging(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, currentPage, out numTotPage, out nRec, true, grigi, true, false, out idProfiles);
            else
                infoDoc = DocumentManager.getQueryInfoDocumentoPaging(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, currentPage, out numTotPage, out nRec, true, grigi, false, false, out idProfiles);
     
            this.DataGrid1.VirtualItemCount = nRec;
            this.DataGrid1.CurrentPageIndex = currentPage - 1;
            //appoggio il risultato in sessione
            Session["listaDocInRisp.infoDoc"] = infoDoc;

            this.BindGrid(infoDoc);
        }


        public void BindGrid(DocsPAWA.DocsPaWR.InfoDocumento[] infos)
        {
            DocsPaWR.InfoDocumento currentDoc;
            if (infos != null && infos.Length > 0)
            {
                this.SetMittDestColumnHeader();

                //Costruisco il datagrid
                Dg_elem = new ArrayList();
                string descrDoc = string.Empty;
                int numProt = new Int32();

                for (int i = 0; i < infos.Length; i++)
                {
                    currentDoc = ((DocsPAWA.DocsPaWR.InfoDocumento)infos[i]);
                    string dataApertura = "";



                    if (currentDoc.dataApertura != null && currentDoc.dataApertura.Length > 0)
                        dataApertura = currentDoc.dataApertura.Substring(0, 10);
                    if (currentDoc.numProt != null)
                        numProt = Int32.Parse(currentDoc.numProt);
                    else
                        numProt = Int32.Parse(currentDoc.docNumber);

                    descrDoc = numProt.ToString();

                    descrDoc = descrDoc + "\n" + dataApertura;

                    string MittDest = "";

                    if (currentDoc.mittDest != null && currentDoc.mittDest.Length > 0)
                    {
                        for (int g = 0; g < currentDoc.mittDest.Length; g++)
                            MittDest += currentDoc.mittDest[g] + " - ";
                        if (currentDoc.mittDest.Length > 0)
                            MittDest = MittDest.Substring(0, MittDest.Length - 3);
                    }

                    string newEtichetta = getEtichetta(currentDoc.tipoProto);

                    Dg_elem.Add(new Cols(descrDoc, currentDoc.oggetto, MittDest, newEtichetta, currentDoc.codRegistro, i, currentDoc.segnatura));
                }



                this.DataGrid1.DataSource = Dg_elem;
                this.DataGrid1.DataBind();

                string message = "";
                int retValue;

                DocsPaWR.SchedaDocumento doc = DocumentManager.getDettaglioDocumento(this.Page, sys, sys);

                DocsPaWR.DiagrammaStato dg = null;

                // PALUMBO: se sono documenti tipizzati devo verificare se hanno diagrammi associati
                if (doc != null && doc.tipologiaAtto != null)
                {
                    dg = DocsPAWA.DiagrammiManager.getDgByIdTipoDoc(doc.tipologiaAtto.systemId, (UserManager.getInfoUtente(this)).idAmministrazione, this);

                    //PALUMBO: se hanno diagrammi associati verifico lo stato attuale
                    if (dg != null && doc.docNumber != null && doc.docNumber != "")
                    {
                        DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(doc.docNumber, this);
                        //PALUMBO: se lo stato è finale non consento la cancellazione della concatenazione presente
                        if ((stato != null) && (stato.STATO_FINALE))
                        {
                            for (int i = 0; i < infos.Length; i++)
                            {
                                currentDoc = ((DocsPAWA.DocsPaWR.InfoDocumento)infos[i]);
                                ((ImageButton)this.DataGrid1.Items[i].Cells[7].Controls[1]).Visible = false;
                            }
                        }
                        //PALUMBO: se lo stato NON è finale consento la cancellazione della concatenazione presente se verificaACL da OK (logica pre-esistente)
                        else
                        {
                            for (int i = 0; i < infos.Length; i++)
                            {
                                currentDoc = ((DocsPAWA.DocsPaWR.InfoDocumento)infos[i]);

                                retValue = DocumentManager.verificaACL("D", currentDoc.docNumber, UserManager.getInfoUtente(), out message);

                                if (retValue == 0)
                                    ((ImageButton)this.DataGrid1.Items[i].Cells[7].Controls[1]).Visible = false;
                            }
                        }
                    }
                }
                // PALUMBO: se sono Documenti non tipizzati (logica pre-esistente)
                else if (doc != null && doc.tipologiaAtto == null)
                {
                    for (int i = 0; i < infos.Length; i++)
                    {
                        currentDoc = ((DocsPAWA.DocsPaWR.InfoDocumento)infos[i]);

                        retValue = DocumentManager.verificaACL("D", currentDoc.docNumber, UserManager.getInfoUtente(), out message);

                        if (retValue == 0)
                            ((ImageButton)this.DataGrid1.Items[i].Cells[7].Controls[1]).Visible = false;
                    }

                }
                else
                {
                    this.lbl_message.Text = "Documenti non trovati";
                    this.DataGrid1.Visible = false;
                    this.lbl_message.Visible = true;
                }
            }
        }
            
            


        private const int COL_MITT_DEST_INDEX = 3;

        private void SetMittDestColumnHeader()
        {

            string headerText = "Destinatario";
            if (tipoProto == "P")
                headerText = "Mittente";
            this.DataGrid1.Columns[COL_MITT_DEST_INDEX].HeaderText = headerText;
        }

        #endregion

        #region filtri di ricerca
        public bool GetFiltroDocInRisposta(string docSys, string idRegistro, string tipoProto)
        {
            try
            {

                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                //Filtro per protocolli in PARTENZA
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                //se il protocollo corrente è in partenza allora devo cercare le sue risposte
                //tra i protocolli in ingresso, viceversa per i protocolli in arrivo
                if (tipoProto.Equals("P"))
                {
                    fV1.valore = "A";
                }
                else
                {
                    fV1.valore = "P";
                }

                //MODIFICA PER CONCATENAMENTO TRASVERSALE
                if (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
                                && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1")
                {
                    fV1.valore = "T";
                }
                //FINE MODIFICA CONCATENAMENTO TRASVERSALE

                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                if (!UserManager.isFiltroAooEnabled(this))
                {
                    if (tipoProto.Equals("A"))
                    {
                        //Filtro per REGISTRI VISIBILI ALL'UTENTE
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRI_UTENTE_IN_CONDITION.ToString();
                        fV1.valore = (String)Session["inRegCondition"];
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        //Filtro per REGISTRO DEL DOCUMENTO PROTOCOLLATO
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                        fV1.valore = reg;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                //				//Filtro per REGISTRO
                //				fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //				fV1.argomento=DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                //				fV1.valore = idRegistro;
                //				fVList = Utils.addToArrayFiltroRicerca(fVList,fV1);

                //Filtro per ID_PARENT
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ID_PARENT.ToString();
                fV1.valore = docSys;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                if (tipoProto != "G" && tipoProto != "T")
                {
                    //Filtro per SOLI DOCUMENTI PROTOCOLLATI
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
                    fV1.valore = "0";  //corrisponde a 'false'
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                //FILTRO PER PREDISPOSTI
                if (tipoProto == "T")
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                    fV1.valore = "true";  
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                qV[0] = fVList;
                return true;

            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
            }
        }
        public bool GetFiltroDocGrigi(string docSys)
        {
            try
            {

                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                fV1.valore = "G";

                //MODIFICA PER CONCATENAMENTO TRASVERSALE
                if (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
                                && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1")
                {
                    fV1.valore = "T";
                }
                //FINE MODIFICA CONCATENAMENTO TRASVERSALE

                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);


                //Filtro per REGISTRI VISIBILI ALL'UTENTE
                if (!UserManager.isFiltroAooEnabled(this))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRI_UTENTE_IN_CONDITION_CON_NULL.ToString();
                    fV1.valore = (String)Session["inRegCondition"];
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                //Filtro per ID_PARENT
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ID_PARENT.ToString();
                fV1.valore = docSys;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //MODIFICA PER CONCATENAMENTO TRASVERSALE
                if (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
                                && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1")
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                qV[0] = fVList;
                return true;

            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
            }
        }
        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
            this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChange);
            this.DataGrid1.PreRender += new System.EventHandler(this.Datagrid1_PreRender);
            this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
            this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        #region gestione eventi datagrid
        private void Datagrid1_PreRender(object sender, System.EventArgs e)
        {
            try
            {
                for (int i = 0; i < this.DataGrid1.Items.Count; i++)
                {
                    if (this.DataGrid1.Items[i].ItemIndex >= 0)
                    {
                        switch (this.DataGrid1.Items[i].ItemType.ToString().Trim())
                        {
                            case "Item":
                                {
                                    this.DataGrid1.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                                    this.DataGrid1.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioN'");
                                    break;
                                }
                            case "AlternatingItem":
                                {
                                    this.DataGrid1.Items[i].Attributes.Add("onmouseover", "this.className='bg_grigioS'");
                                    this.DataGrid1.Items[i].Attributes.Add("onmouseout", "this.className='bg_grigioA'");
                                    break;
                                }

                        }
                        string message = string.Empty;

                        this.SetForeColorColumnDescrDocumento(this.DataGrid1.Items[i], this.DataGrid1.SelectedIndex);
                    }


                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }


        private void DataGrid1_PageIndexChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            getParametersFromQueryString();
            currentPage = e.NewPageIndex + 1;
            // Cricamento del DataGrid
            this.LoadData(true);
        }


        private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            DocsPaWR.InfoDocumento newinfoDoc;
            if (this.DataGrid1.SelectedIndex >= 0)
            {
                string str_indexSel = ((Label)this.DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[6].Controls[1]).Text;
                int indexSel = Int32.Parse(str_indexSel);
                this.infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento[])Session["listaDocInRisp.infoDoc"];

                if (indexSel > -1)
                    newinfoDoc = (DocsPAWA.DocsPaWR.InfoDocumento)this.infoDoc[indexSel];
                else
                {
                    newinfoDoc = null;
                    return;
                }

                if (newinfoDoc != null)
                {
                    string message = string.Empty;
                    int retValue = DocumentManager.verificaACL("D", newinfoDoc.docNumber, UserManager.getInfoUtente(), out message);
                    if (retValue == 0)
                        Response.Write("<script>window.alert('Non si posseggono i diritti necessari alla visualizzazione del documento richiesto.')</script>");
                    else
                    {
                        DocumentManager.setRisultatoRicerca(this, newinfoDoc);

                        Session.Remove("listaDocInRisp.infoDoc");
                        Response.Write("<script>window.returnValue='true'; window.close();</script>");
                    }
                }
            }
        }
        #endregion

        private void btn_chiudi_Click(object sender, System.EventArgs e)
        {
            Response.Write("<script>window.close();</script>");
        }

        private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                getParametersFromQueryString();
                DocsPaWR.InfoDocumento infoDocSel;
                if (e.Item.ItemIndex >= 0)
                {
                    string str_indexSel = ((Label)this.DataGrid1.Items[e.Item.ItemIndex].Cells[6].Controls[1]).Text;
                    int indexSel = Int32.Parse(str_indexSel);
                    this.infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento[])Session["listaDocInRisp.infoDoc"];

                    if (indexSel > -1)
                    {
                        infoDocSel = (DocsPAWA.DocsPaWR.InfoDocumento)this.infoDoc[indexSel];
                    }
                    else
                    {
                        infoDocSel = null;
                        return;
                    }

                    //Scollego i documenti
                    bool retValue = DocumentManager.scollegaDocumento(this, infoDocSel.idProfile);

                    if (retValue)
                    {
                        if (currentPage > 1 && this.DataGrid1.Items.Count == 1)
                        {
                            currentPage = currentPage - 1;
                        }

                        DocsPaWR.InfoDocumento[] newListInfoDocumenti = new DocsPAWA.DocsPaWR.InfoDocumento[this.infoDoc.Length - 1];

                        if (newListInfoDocumenti.Length == 0)
                        {
                            this.LoadData(true);
                        }
                        else
                        {
                            int index = 0;

                            foreach (DocsPAWA.DocsPaWR.InfoDocumento infoDocumento in this.infoDoc)
                            {
                                if (infoDocumento.idProfile != infoDocSel.idProfile)
                                {
                                    newListInfoDocumenti[index] = infoDocumento;
                                    index++;
                                }
                            }

                            this.infoDoc = newListInfoDocumenti;
                            Session["listaDocInRisp.infoDoc"] = this.infoDoc;

                            this.BindGrid(this.infoDoc);
                        }

                        this.DataGrid1.SelectedIndex = -1;
                    }
                }
                else
                {
                    Response.Write("<script>alert('Attenzione: selezionare un documento!');</script>");
                }
            }
        }

        /// <summary>
        /// Prende i dati esistenti per le etichette dei protocolli (Inserita da Fabio)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void getLettereProtocolli()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.InfoUtente infoUtente = new DocsPAWA.DocsPaWR.InfoUtente();
            infoUtente = UserManager.getInfoUtente(this);
            String idAmm = UserManager.getInfoUtente().idAmministrazione;

            this.etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
            this.arrivo = etichette[0].Descrizione; //Valore A
            this.partenza = etichette[1].Descrizione; //Valore P
            this.interno = etichette[2].Descrizione; //Valore I
            this.grigio = etichette[3].Descrizione; //Valore G
        }

        private string getEtichetta(string value)
        {
            if ((value == "G") || (value == "R"))
            {
                return this.grigio;
            }
            else
            {
                if (value == "A")
                {
                    return this.arrivo;
                }
                else
                {
                    if (value == "I")
                    {
                        return this.interno;
                    }
                    else
                    {
                        return this.partenza;
                    }
                }
            }
        }

        /// <summary>
        /// Impostazione del colore del carattere per la prima colonna della griglia:
        /// rosso se doc protocollato, altrimenti grigio 
        /// </summary>
        /// <param name="item"></param>
        private void SetForeColorColumnDescrDocumento(DataGridItem item, int index)
        {
            Label lbl = item.Cells[8].Controls[1] as Label;

            if (lbl != null)
            {
                item.Cells[0].Font.Bold = true;

                if (lbl.Text.Equals(""))
                {
                    if (item.ItemIndex != index)
                        ((Label)item.Cells[0].Controls[1]).ForeColor = Color.Black;
                    else
                        ((Label)item.Cells[0].Controls[1]).ForeColor = Color.White;
                }
                else
                {
                    //string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    string idAmm = string.Empty;
                    if ((string)Session["AMMDATASET"] != null)
                        idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                    else
                        if (UserManager.getInfoUtente() != null && !string.IsNullOrEmpty(UserManager.getInfoUtente().idAmministrazione))
                            idAmm = UserManager.getInfoUtente().idAmministrazione;

                    DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    string segnAmm = "0";
                    if (!string.IsNullOrEmpty(idAmm))
                        segnAmm = ws.getSegnAmm(idAmm);
                    switch (segnAmm)
                    {
                        case "0":
                            ((Label)item.Cells[0].Controls[1]).ForeColor = Color.Black;
                            break;

                        case "1":
                            ((Label)item.Cells[0].Controls[1]).ForeColor = Color.Blue;
                            break;

                        case "2":
                        default:
                            ((Label)item.Cells[0].Controls[1]).ForeColor = Color.Red;
                            break;
                    }
                }
                lbl = null;
            }
        }
    }
}
