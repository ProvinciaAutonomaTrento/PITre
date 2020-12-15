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

namespace DocsPAWA.Archivio
{
    public partial class tabRisultatiRicArchivio : DocsPAWA.CssPage
    {
        protected DocsPAWA.DocsPaWR.InfoDocumento[] ListaDocInSerie;
        protected System.Web.UI.WebControls.DataGrid dgDoc;
        protected ArrayList dataTableProt;
        protected Hashtable hashListaDocumenti;
        protected System.Web.UI.HtmlControls.HtmlTableRow trHeader;
        protected System.Web.UI.WebControls.Label lbl_messaggio;

        protected int nRec;
        protected int numTotPage;

        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    ViewState.Add("scelta", "");

                    Utils.startUp(this);
                    this.AttachWaitingControl();
                    if (Request.QueryString["tipoOp"] != null)
                    {

                        this.dgDoc.CurrentPageIndex = this.GetCurrentPageIndex();
                        if (Request.QueryString["tipoOp"] == "S")
                        {
                            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                            if (template != null)
                            {
                                ViewState["scelta"] = "S";
                                this.FillData(this.dgDoc.CurrentPageIndex + 1);
                            }
                        }
                        if (Request.QueryString["tipoOp"] == "F")
                        {
                            DocsPAWA.DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
                            if (fascicolo != null)
                            {
                                ViewState["scelta"] = "F";
                                this.FillData(this.dgDoc.CurrentPageIndex + 1);
                            }
                        }

                        //Se l'utente è un archivista cambiano l'immagine del punsante btn_archivia e la tooltip
                        if (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA"))
                        {
                            btn_archivia.Visible = true;
                            btn_archivia.ImageUrl = "../images/proto/btn_corrente.gif";
                            btn_archivia.AlternateText = "Inserisci tutti i documenti in archivio corrente";
                        }
                    }
                    else
                    {
                        this.dgDoc.Visible = false;
                        this.trHeader.Visible = false;
                        this.lbl_messaggio.Visible = false;
                    }
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
            this.PreRender += new System.EventHandler(this.Page_PreRender);
            this.dgDoc.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgDoc_PageIndexChanged);
            this.dgDoc.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgDoc_ItemDataBound);
            this.dgDoc.SelectedIndexChanged += new System.EventHandler(this.dgDoc_SelectedIndexChanged);
            this.dgDoc.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgDoc_ItemCommand);
	    }
        #endregion

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            this.RefreshCountDocumenti();
        }

        
        #region gestione datagrid
        /// <summary>
        /// Reperimento indice pagina corrente
        /// </summary>
        /// <returns></returns>
        private int GetCurrentPageIndex()
        {
            int retValue = 0;
            retValue = FascicoliManager.GetProtoDocsGridPaging(this);
            if (retValue == 0)
            {
                retValue = this.dgDoc.CurrentPageIndex;
            }
            return retValue;
        }

        private void dgDoc_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.dgDoc.SelectedIndex = -1;
            this.dgDoc.CurrentPageIndex = e.NewPageIndex;
            int pageNumber = e.NewPageIndex + 1;
            this.FillData(pageNumber);
        }

        private void dgDoc_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
           if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                e.Item.Cells[0].Font.Bold = true;
                e.Item.Cells[0].ForeColor = System.Drawing.Color.Gray;
                //Documenti protocollati in rosso
                Label lbl = e.Item.Cells[9].Controls[1] as Label;
                if (lbl != null)
                {
                    e.Item.Cells[1].Font.Bold = true;
                    if (lbl.Text.Equals(""))
                        e.Item.Cells[1].ForeColor = System.Drawing.Color.Black;
                    else
                        e.Item.Cells[1].ForeColor = System.Drawing.Color.Red;
                    lbl = null;
                }		
                //Documenti annullati in rosso
                string dataAnnull = ((TableCell)e.Item.Cells[8]).Text;
                try
                {
                    DateTime dt = Convert.ToDateTime(dataAnnull);
                    e.Item.ForeColor = System.Drawing.Color.Red;
                    e.Item.Font.Bold = true;
                    e.Item.Font.Strikeout = true;
                }
                catch { }
            }
            // gestione della icona dei dettagli -- tolgo icona se non c'è doc acquisito
            Label lblImg = e.Item.Cells[13].FindControl("lbl_chaImg") as Label;
            if (lblImg != null && lblImg.Text == "1")
            {
                ImageButton imgbtn = new ImageButton();
                if (e.Item.Cells[10].Controls[1].GetType().Equals(typeof(ImageButton)))
                {
                    imgbtn = (ImageButton)e.Item.Cells[10].Controls[1];
                    imgbtn.Visible = true;
                }
            }
        }

        //Non dovrebbe serivre più... perchè per ora sono stati commentati i pulsanti dal datagrid
        private void dgDoc_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            // visualizza doc in popup
            if (e.CommandName.Equals("VisDoc"))
            {
                //vis unificata
                string docNumber = ((Label)this.dgDoc.Items[e.Item.ItemIndex].Cells[12].Controls[1]).Text;
                string idProfile = ((Label)this.dgDoc.Items[e.Item.ItemIndex].Cells[11].Controls[1]).Text;

                DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumento(this, idProfile, docNumber);
                if (schedaDoc.documenti[0].fileSize != "" && schedaDoc.documenti[0].fileSize != "0")
                {
                    DocumentManager.setDocumentoSelezionato(this, schedaDoc);
                    FileManager.setSelectedFile(this, schedaDoc.documenti[0], false);
                    ClientScript.RegisterStartupScript(this.GetType(), "lanciaVIs", "loadvisualizzaDoc('" + Session.SessionID + "','" + docNumber + "','" + idProfile + "');", true);
                }
            }
            if (e.CommandName.Equals("InArchivio"))
            {
                string script = "<script>if(window.parent.parent.document.getElementById ('please_wait_archivio')!=null)";
                script += "{";
                script += "		window.parent.parent.document.getElementById ('please_wait_archivio').style.display = 'none'";
                script += "} </script>";
                Response.Write(script);

                string tipoOperazione = "";
                if (UserManager.ruoloIsAutorized(this, "DO_ARCHIVIA"))
                {
                    tipoOperazione = "IN_DEPOSITO";
                }
                else
                {
                    tipoOperazione = "IN_CORRENTE";
                }


                //Fascicoli generali
                string idProfile = ((Label)this.dgDoc.Items[e.Item.ItemIndex].Cells[11].Controls[1]).Text;
                if (ViewState["scelta"].ToString() == "F")
                {
                    if (DocumentManager.trasfInDepositoDocumento(this, idProfile, UserManager.getInfoUtente(), false, tipoOperazione) != "-1")
                    {
                        this.dgDoc.CurrentPageIndex = 0;
                        this.FillData(this.dgDoc.CurrentPageIndex + 1);
                    }
                }
                //caso serie di documenti repertoriati
                else if (ViewState["scelta"].ToString() == "S")
                {
                    if (DocumentManager.trasfInDepositoDocumento(this, idProfile, UserManager.getInfoUtente(), true, tipoOperazione) != "-1")
                    {
                        this.dgDoc.CurrentPageIndex = 0;
                        this.FillData(this.dgDoc.CurrentPageIndex + 1);
                    }
                }
            }
        }

        private void dgDoc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string newUrl = "";
            string docNumber = ((Label)this.dgDoc.Items[this.dgDoc.SelectedIndex].Cells[12].Controls[1]).Text;
            string idProfile = ((Label)this.dgDoc.Items[this.dgDoc.SelectedIndex].Cells[11].Controls[1]).Text;
            string tipoP = ((Label)this.dgDoc.Items[this.dgDoc.SelectedIndex].Cells[4].Controls[1]).Text;
            if (!tipoP.Equals("NP"))
            {
                newUrl = "../documento/gestioneDoc.aspx?tab=protocollo";
            }
            else
            {
                newUrl = "../documento/gestioneDoc.aspx?tab=profilo";
            }
            DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumento(this, idProfile, docNumber);
            if (schedaDoc != null)
            {
                DocumentManager.setDocumentoSelezionato(this, schedaDoc);
                Response.Write("<script>window.open('" + newUrl + "','principale');</script>");
            }
        }

        private void FillData(int requestedPage)
        {
            try
            {
                dataTableProt = new ArrayList();
                //Caso in cui è stato scelto di archiviare i documenti di un fascicolo generale
                if (ViewState["scelta"].ToString() == "F")
                {
                    DocsPAWA.DocsPaWR.Fascicolo fascicolo;
                    fascicolo = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
                    if (fascicolo != null)
                    {
                        caricaDataTablesFascicoli(fascicolo, requestedPage, out nRec, out numTotPage);
                    }
                }
                //caso in cui è stato scelto di archiviare i documenti in serie
                else if (ViewState["scelta"].ToString() == "S")
                {
                    caricaDataTablesSerie(requestedPage, out nRec, out numTotPage);
                }

                if (ViewState["scelta"].ToString() == "F")
                    this.dgDoc.Columns[0].Visible = false;
                else
                {
                    DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                    string valOggetto = Session["valOggetto"].ToString();
                    foreach (DocsPAWA.DocsPaWR.OggettoCustom oggetto in template.ELENCO_OGGETTI)
                    {
                        if (valOggetto.Equals(oggetto.SYSTEM_ID.ToString()))
                            this.dgDoc.Columns[0].HeaderText = oggetto.DESCRIZIONE;
                    }
                    this.dgDoc.Columns[0].Visible = true;
                }

                this.dgDoc.DataSource = dataTableProt;
                this.dgDoc.DataBind();
                //PER ora rendo invisibili le colonne 
                //per visualizzare il dettaglio del documento, 
                //per visualizzare il documento principale,
                //trasferire in deposito un singolo documento
                this.dgDoc.Columns[7].Visible = false;
                this.dgDoc.Columns[10].Visible = false; 
                this.dgDoc.Columns[14].Visible = false;

                this.dgDoc.Visible = (this.TotalRecordCount > 0);
                this.trHeader.Visible = (this.TotalRecordCount > 0);
                this.lbl_messaggio.Visible = false;
                if (TotalRecordCount == 0)
                {
                    this.lbl_messaggio.Text = "Nessun documento presente.";
                    this.lbl_messaggio.Visible = true;
                }
            }
            catch (Exception es)
            {
                string error = es.ToString();
            }
        }

        //Recupera la lista di documenti classificati in un dato fascicolo generale
        private void caricaDataTablesFascicoli(DocsPAWA.DocsPaWR.Fascicolo fascicolo, int numPage, out int nRec, out int numTotPage)
        {
            nRec = 0;
            numTotPage = 0;
            try
            {
                dataTableProt = new ArrayList();
                hashListaDocumenti = new Hashtable();
                DocsPaWR.InfoDocumento[] listaDoc = null;
                string anno = "";
                if (Session["anno"] != null)
                    anno = Session["anno"].ToString();
                listaDoc = FascicoliManager.getListaDocumentiDaArchiviare(this, fascicolo, numPage, out numTotPage, out nRec, anno);
                this.TotalRecordCount = nRec;
                this.dgDoc.VirtualItemCount = this.TotalRecordCount;
                if (listaDoc != null)
                {
                    if (listaDoc.Length > 0)
                    {
                        for (int i = 0; i < listaDoc.Length; i++)
                        {
                            hashListaDocumenti.Add(i, listaDoc[i]);
                            DocsPaWR.InfoDocumento infoDoc = listaDoc[i];
                            CostruisciDataTable(infoDoc);
                        }
                   }
                }
                FascicoliManager.setHashDocProtENonProt(this, hashListaDocumenti);
                //impostazione datatable in sessione
                FascicoliManager.setDataTableDocDaArchiv(this, dataTableProt);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        //Recupera la lista di documenti appartenenti ad una data serie
        private void caricaDataTablesSerie(int numPage, out int nRec, out int numTotPage)
        {
            nRec = 0;
            numTotPage = 0;
            try
            {
                dataTableProt = new ArrayList();
                DocsPaWR.InfoDocumento[] ListaDoc = null;
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                string valOggetto = Session["valOggetto"].ToString();
                string anno = Session["anno"].ToString();
                string rf_AOO = Session["aoo_rf"].ToString();
                //Restituisce la lista di tutti i documenti per la serie selezionata
                ListaDoc = DocumentManager.getDocInSerie(UserManager.getInfoUtente(this).idGruppo, UserManager.getInfoUtente(this).idPeople, this, template, numPage, out numTotPage, out nRec, valOggetto, anno, rf_AOO);
                this.TotalRecordCount = ListaDoc.Length;
                this.TotalRecordCount = nRec;
                this.dgDoc.VirtualItemCount = this.TotalRecordCount;
                if (ListaDoc != null)
                {
                    if (ListaDoc.Length > 0)
                    {
                        for (int i = 0; i < ListaDoc.Length; i++)
                        {
                            DocsPaWR.InfoDocumento infoDoc = ListaDoc[i];
                            CostruisciDataTable(infoDoc);
                        }
                    }
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void CostruisciDataTable(DocsPaWR.InfoDocumento infoDoc)
        {
            string descDoc = string.Empty;
                        
            //il campo mittDest è un array list di possibili 
            //mitt/dest lo scorro tutto e concat in una singola string 
            //con separatore ="[spazio]-[spazio]"
            string MittDest = "";

            if (infoDoc != null && infoDoc.mittDest != null)
            {
                if (infoDoc.mittDest.Length > 0)
                {
                    for (int g = 0; g < infoDoc.mittDest.Length; g++)
                    {
                        MittDest += infoDoc.mittDest[g] + " - ";
                    }
                    MittDest = MittDest.Substring(0, MittDest.Length - 3);
                }
            }
            //data apertura formattata
            string dataApertura = "";
            int numProt = new Int32();
            if (infoDoc.dataApertura != null && !infoDoc.dataApertura.Equals(""))
                dataApertura = infoDoc.dataApertura.Substring(0, 10);
            //setto il registro (prova)
            //infoDoc.codRegistro = schedaDoc.registro.codice;
            string tipoProto = string.Empty;

            if (infoDoc.tipoProto.Equals("A") || infoDoc.tipoProto.Equals("P") || infoDoc.tipoProto.Equals("I"))
            {
                //aggiunge al datatable dei protocollati
                if (infoDoc.numProt != null && !infoDoc.numProt.Equals(""))
                {
                    numProt = Int32.Parse(infoDoc.numProt);
                    descDoc = numProt + "\n" + dataApertura;
                    dataTableProt.Add(new Cols(descDoc, numProt, dataApertura, infoDoc.segnatura, infoDoc.codRegistro, infoDoc.tipoProto, infoDoc.oggetto, MittDest, infoDoc.dataAnnullamento, infoDoc.idProfile, infoDoc.docNumber, infoDoc.acquisitaImmagine, infoDoc.numSerie));
                }
                else
                {
                    // se documento grigio/pred.
                    descDoc = infoDoc.docNumber + "\n" + dataApertura;
                    if (infoDoc.tipoProto != null && infoDoc.tipoProto == "G")
                        tipoProto = "NP";
                    else
                        tipoProto = infoDoc.tipoProto;
                    dataTableProt.Add(new Cols(descDoc, new Int32(), dataApertura, "", infoDoc.codRegistro, tipoProto, infoDoc.oggetto, MittDest, infoDoc.dataAnnullamento, infoDoc.idProfile, infoDoc.docNumber, infoDoc.acquisitaImmagine, infoDoc.numSerie));
                }
            }
            else
            {
                // se documento grigio
                descDoc = infoDoc.docNumber + "\n" + dataApertura;

                if (infoDoc.tipoProto != null && infoDoc.tipoProto == "G")
                    tipoProto = "NP";
                else
                    tipoProto = infoDoc.tipoProto;
                dataTableProt.Add(new Cols(descDoc, Int32.Parse(infoDoc.idProfile), dataApertura, "", "", tipoProto, infoDoc.oggetto, "", "", infoDoc.idProfile, infoDoc.docNumber, infoDoc.acquisitaImmagine, infoDoc.numSerie));
            }
        }

        private void dettaglioDocumento(string idProfile, string docNumber, string newUrl)
        {
            DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumento(this, idProfile, docNumber);
            if (schedaDoc != null)
            {
                DocumentManager.setDocumentoSelezionato(this, schedaDoc);
                Response.Write("<script>window.open('" + newUrl + "','principale');</script>");
            }
        }

        public class Cols
        {
            private string descrDoc;
            private int numProt;
            private string dataApertura;
            private string segnatura;
            private string codRegistro;
            private string tipoProto;
            private string oggetto;
            private string mittDest;
            private string dataAnnullamento;
            private string idProfile;
            private string docNumber;
            private string acquisitaImmagine;
            private string numSerie;

            public Cols(string descrDoc, int numProt, string dataApertura, string segnatura, string codRegistro, string tipoProto, string oggetto, string mittDest, string dataAnnullamento, string idProfile, string docNumber, string acquisitaImmagine, string numSerie)
            {
                this.descrDoc = descrDoc;
                this.numProt = numProt;
                this.dataApertura = dataApertura;
                this.segnatura = segnatura;
                this.codRegistro = codRegistro;
                this.tipoProto = tipoProto;
                this.oggetto = oggetto;
                this.mittDest = mittDest;
                this.dataAnnullamento = dataAnnullamento;
                this.idProfile = idProfile;
                this.docNumber = docNumber;
                this.acquisitaImmagine = acquisitaImmagine;
                this.numSerie = numSerie;
            }

            public string DescrDoc { get { return descrDoc; } }
            public int NumProt { get { return numProt; } }
            public string DataApertura { get { return dataApertura; } }
            public string Segnatura { get { return segnatura; } }
            public string CodRegistro { get { return codRegistro; } }
            public string TipoProto { get { return tipoProto; } }
            public string Oggetto { get { return oggetto; } }
            public string MittDest { get { return mittDest; } }
            public string DataAnnullamento { get { return dataAnnullamento; } }
            public string IdProfile { get { return idProfile; } }
            public string DocNumber { get { return docNumber; } }
            public string AcquisitaImmagine { get { return acquisitaImmagine; } }
            public string NumSerie { get { return numSerie; } }
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
            this.titolo.Text = "Elenco documenti - Trovati " + this.TotalRecordCount.ToString() + " elementi.";
            if (this.TotalRecordCount == 0)
            {
                this.btn_archivia.Visible = false;
            }
        }

        public string getSegnatura(string segnatura)
        {
            string rtn = " ";
            try
            {
                if (segnatura == null)
                    return rtn = "";
                else
                    return rtn = segnatura;
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
                return rtn;
            }
        }
        #endregion

        #region Gestione DataGridPagingWait

        /// <summary>
        /// Attatch del controllo "DataGridPagingWait" al datagrid
        /// </summary>
        private void AttachWaitingControl()
        {
            this.WaitingControl.DataGridID = this.dgDoc.ClientID;
            this.WaitingControl.WaitScriptCallback = "WaitGridPagingAction();";
        }

        /// <summary>
        /// Reperimento controllo "DataGridPagingWait"
        /// </summary>
        private waiting.DataGridPagingWait WaitingControl
        {
            get
            {
                return this.FindControl("dgDoc_pagingWait") as waiting.DataGridPagingWait;
            }
        }

        #endregion

        protected string WaitingPanelTitle
        {
            get
            {
                if (UserManager.ruoloIsAutorized(this, "DO_ARCHIVIA"))
                    // se archivista
                    return "Trasferimento in archivio di deposito in corso...";
                else
                    return "Trasferimento in archivio corrente in corso...";
            }
        }

        protected void btn_archivia_Click(object sender, ImageClickEventArgs e)
        {
            string anno = "";
            int numDocsInArchivio2 = 0;
            string msg = "";

            string tipoOperazione = "";
            if (UserManager.ruoloIsAutorized(this, "DO_ARCHIVIA"))
            {
                tipoOperazione = "IN_DEPOSITO";
            }
            else
            {
                tipoOperazione = "IN_CORRENTE";
            }

            //Caso fascicoli generali
            if (ViewState["scelta"].ToString() == "F")
            {
                DocsPAWA.DocsPaWR.Fascicolo fascicolo;
                fascicolo = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
                if (fascicolo != null)
                {
                    if (Session["anno"] != null)
                        anno = Session["anno"].ToString();
                    numDocsInArchivio2 = FascicoliManager.trasfInDepositoALLDocsFascicoloGen(this, fascicolo, anno, UserManager.getInfoUtente(), tipoOperazione);
                    if (numDocsInArchivio2 > -1)
                    {
                        trHeader.Visible = false;
                        dgDoc.Visible = false;
                        this.lbl_messaggio.Visible = true;
                        if (UserManager.ruoloIsAutorized(this, "DO_ARCHIVIA"))
                        {
                            msg = "Trasferimento in archivio di deposito avvenuto con successo.";
                        }
                        else
                            msg = "Trasferimento in archivio corrente avvenuto con successo.";
                        if (numDocsInArchivio2 > 0)
                            msg += "\n" + numDocsInArchivio2 + " documenti ancora visibili in entrambi gli ambienti.";
                        this.lbl_messaggio.Text = msg;
                    }
                }
            }
            //caso serie di documenti repertoriati
            else if (ViewState["scelta"].ToString() == "S")
            {
                string valOggetto = Session["valOggetto"].ToString();
                anno = Session["anno"].ToString();
                string rfAOO = Session["aoo_rf"].ToString();
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                numDocsInArchivio2 = DocumentManager.trasfInDepositoSerie(UserManager.getInfoUtente(this), this, template, anno, valOggetto, tipoOperazione, rfAOO);
                if (numDocsInArchivio2 > -1)
                {
                    trHeader.Visible = false;
                    dgDoc.Visible = false;
                    this.lbl_messaggio.Visible = true;
                    if (UserManager.ruoloIsAutorized(this, "DO_ARCHIVIA"))
                    {
                        msg = "Trasferimento in archivio di deposito avvenuto con successo.";
                    }
                    else
                    {
                        msg = "Trasferimento in archivio corrente avvenuto con successo.";
                    }
                    if (numDocsInArchivio2 > 0)
                        msg += "\n" + numDocsInArchivio2 + " documenti ancora visibili in entrambi gli ambienti.";
                    this.lbl_messaggio.Text = msg;
                }
            }
        }
  
    }
}
