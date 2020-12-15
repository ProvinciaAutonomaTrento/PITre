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

namespace DocsPAWA.popup
{
    public partial class DocInCestino : DocsPAWA.CssPage
    {
        protected ArrayList Dt_elem;
        protected DocsPAWA.DocsPaWR.InfoDocumento[] ListaDoc;
        protected System.Web.UI.WebControls.DataGrid DGDoc;
        protected System.Web.UI.WebControls.Label titolo;
        protected System.Web.UI.WebControls.ImageButton btn_stampa;
        protected System.Web.UI.WebControls.ImageButton btn_svuota;
        protected DocsPAWA.DocsPaWR.InfoDocumento InfoDoc;
        protected DocsPaWebCtrlLibrary.ImageButton btnFilterDocs;
        protected DocsPaWebCtrlLibrary.ImageButton btnShowAllDocs;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtFilterDocumentsRetValue;
        protected int numeroDoc = 0;
        protected Utilities.MessageBox Msg_Ripristina;
        protected Utilities.MessageBox Msg_SvuotaCestino;
        protected Utilities.MessageBox Msg_EliminaDoc;

        protected DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);

            getLettereProtocolli();

            if (!Page.IsPostBack)
            {
                this.btn_stampa.Attributes.Add("onclick", "StampaRisultatoRicerca();");
                //this.AttachWaitingControl();
                this.btnFilterDocs.Attributes.Add("onClick", "ShowDialogSearchDocuments();");
                this.btnShowAllDocs.Attributes.Add("onClick", "ShowWaitCursor();");
                ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.RemoveCurrentFilter();
                BindGrid();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            this.btnFilterDocs.Enabled = true;

            //ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.SetCurrentFilter(
            //this.btnShowAllDocs.Enabled = (ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.GetCurrentFilter() != null));
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnFilterDocs.Click += new System.Web.UI.ImageClickEventHandler(this.btnFilterDocs_Click);
            this.btnShowAllDocs.Click += new System.Web.UI.ImageClickEventHandler(this.btnShowAllDocs_Click);
            this.DGDoc.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DGDoc_ItemCommand);
            this.DGDoc.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DGDoc_ItemDataBound);
			
            this.Msg_Ripristina.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.Msg_Ripristina_GetMessageBoxResponse);
            this.Msg_SvuotaCestino.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.Msg_SvuotaCestino_GetMessageBoxResponse);
            this.Msg_EliminaDoc.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.Msg_EliminaDoc_GetMessageBoxResponse);
            this.btn_svuota.Click += new System.Web.UI.ImageClickEventHandler(this.btn_svuota_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(this.Page_PreRender);
        }
        #endregion


        #region datagrid
        public void BindGrid()
        {
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();

            DocsPaWR.FiltroRicerca[][] filtriRicercaDocumenti = GetFiltriRicercaDocumenti();


            if (filtriRicercaDocumenti == null)
                ListaDoc = DocumentManager.getDocInCestino(this, infoUtente, null);
            else
            {
                ListaDoc = DocumentManager.getDocInCestino(this, infoUtente, filtriRicercaDocumenti);
                if (ListaDoc == null)
                {
                    this.btnFilterDocs.Enabled = false;
                    this.btnShowAllDocs.Enabled = false;
                }
            }
            Session["ListaDocCestino"] = ListaDoc;
               


            if (ListaDoc == null || ListaDoc.Length == 0)
            {
                // this.titolo.Text = "Nessun valore trovato";
                this.DGDoc.Visible = false;
                this.btn_stampa.Visible = false;
                this.btn_svuota.Visible = false;
              
                numeroDoc = 0;

                //return;
            }
            else
            {
                InfoDoc = new DocsPAWA.DocsPaWR.InfoDocumento();
                Dt_elem = new ArrayList();
                string docData;
                for (int i = 0; i < ListaDoc.Length; i++)
                {

                    InfoDoc = (DocsPAWA.DocsPaWR.InfoDocumento)ListaDoc[i];
                    //il campo mittDest è un array list di possibili mitt/dest lo scorro tutto e concat in una singola string con separatore ="[spazio]-[spazio]"
                    string MittDest = "";
                    int numProt = new Int32();
                    if (InfoDoc.mittDest != null && InfoDoc.mittDest.Length > 0)
                    {
                        for (int g = 0; g < InfoDoc.mittDest.Length; g++)
                            MittDest += InfoDoc.mittDest[g] + " - ";
                        if (InfoDoc.mittDest.Length > 0)
                            MittDest = MittDest.Substring(0, MittDest.Length - 3);
                    }
                    else
                        MittDest += "";
                    docData = InfoDoc.docNumber + "\n" + ListaDoc[i].dataApertura;
                    string nuova_etichetta = getEtichetta(InfoDoc.tipoProto);
                    Dt_elem.Add(new Cols(docData, InfoDoc.codRegistro, InfoDoc.oggetto, nuova_etichetta, MittDest, InfoDoc.noteCestino, InfoDoc.autore, InfoDoc.acquisitaImmagine));
                }
                if (ListaDoc.Length > 0)
                {
                    //DocumentManager.setDataGridAllegati(this,Dt_elem);					
                    this.DGDoc.DataSource = Dt_elem;
                    this.DGDoc.DataBind();
                    
                }
                this.btn_stampa.Visible = true;
                this.btnFilterDocs.Visible = true;
                this.btnShowAllDocs.Enabled = false;
                this.DGDoc.Visible = true;
                numeroDoc = DGDoc.Items.Count;

                DGDoc.SelectedIndex = -1;
            }
            
            RefreshCountDocumenti();
        }

        public class Cols
        {
            private string docData;
            private string registro;
            private string oggetto;
            private string tipo;
            private string mittDest;
            private string motivo;
            private string autore;
            private string acquisitaImmagine;
           
            public Cols(string docData, string registro, string oggetto,  string tipo, string mittDest, string motivo, string autore,string acquisitaImmagine)
            {
                this.docData = docData;
                this.registro = registro;
                this.oggetto = oggetto;
                this.tipo = tipo;
                this.mittDest = mittDest;
                this.motivo = motivo;
                this.autore = autore;
                this.acquisitaImmagine = acquisitaImmagine;
            }

            public string DocData { get { return docData; } }
            public string Registro { get { return registro; } }
            public string Oggetto { get { return oggetto; } }
            public string Tipo { get { return tipo; } }
            public string MittDest { get { return mittDest; } }
            public string Motivo { get { return motivo; } }
            public string Autore { get { return autore; } }
            public string AcquisitaImmagine { get { return acquisitaImmagine; } }
        }

        private void DGDoc_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName.Equals("Select"))
            {
                int key = e.Item.ItemIndex;
                ListaDoc = ((DocsPAWA.DocsPaWR.InfoDocumento[])Session["ListaDocCestino"]);
                DocsPAWA.DocsPaWR.InfoDocumento infoDoc = new DocsPAWA.DocsPaWR.InfoDocumento();
                
                infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento)ListaDoc[e.Item.ItemIndex];
                DocumentManager.setRisultatoRicerca(this, infoDoc);

                Response.Write("<script>window.open('../documento/gestionedoc.aspx?tab=profilo&daCestino=1','principale');</script>");

                Response.Write("<script>window.close();</script>");
                //Response.Write("<script language='javascript'>top.principale.document.location = '../documento/gestionedoc.aspx?tab=profilo' ;</script>");
		
            }
            if (e.CommandName.Equals("Ripristina"))
            { 
                //Il documento ritorna attivo
                this.DGDoc.SelectedIndex = e.Item.ItemIndex;
                
                //Conferma utente
                string messaggio = InitMessageXml.getInstance().getMessage("RIATTIVA_DOCUMENTO");
                Msg_Ripristina.Confirm(messaggio);
                
            }
            if (e.CommandName.Equals("Elimina"))
            {
                DGDoc.SelectedIndex = e.Item.ItemIndex;
                //La rimozione dei documenti è un privilegio di solo alcuni utenti... 
                //Verifica che l'utente connesso sia abilitato a svuotare i cestini
                if (!UserManager.ruoloIsAutorized(this, "SVUOTA_CESTINO"))
                {
                    Response.Write("<script>alert('Utente non abilitato a questa operazione.')</script>");
                }
                else
                {
                    //Conferma utente
                    string messaggio = InitMessageXml.getInstance().getMessage("ELIMINA_DOCUMENTO");
                    Msg_EliminaDoc.Confirm(messaggio);
                   
                }
            }
            if (e.CommandName.Equals("VisDoc"))
            {
                int key = e.Item.ItemIndex;
                ListaDoc = ((DocsPAWA.DocsPaWR.InfoDocumento[])Session["ListaDocCestino"]);
                DocsPAWA.DocsPaWR.SchedaDocumento schedaSel = new DocsPAWA.DocsPaWR.SchedaDocumento();
                schedaSel = DocumentManager.getDettaglioDocumento(this, this.ListaDoc[key].idProfile, this.ListaDoc[key].docNumber);
                DocumentManager.setDocumentoSelezionato(this, schedaSel);
                FileManager.setSelectedFile(this, schedaSel.documenti[0], false);
                ClientScript.RegisterStartupScript(this.GetType(), "lanciaVIs", "loadvisualizzaDoc('" + Session.SessionID + "','" + this.ListaDoc[key].docNumber + "','" + this.ListaDoc[key].idProfile + "');", true);
            }
        }

        private void DGDoc_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            ImageButton imgbtn = new ImageButton();
            if (e.Item.ItemType == ListItemType.Item
                || e.Item.ItemType == ListItemType.AlternatingItem
                || e.Item.ItemType == ListItemType.SelectedItem)
            {
                e.Item.Cells[0].Font.Bold = true;
                e.Item.Cells[0].ForeColor = System.Drawing.Color.Gray;

                string acquisitaImmagine = ((TableCell)e.Item.Cells[10]).Text;
                imgbtn = (ImageButton)e.Item.Cells[8].Controls[1];
                if (!string.IsNullOrEmpty(acquisitaImmagine) && !acquisitaImmagine.Equals("0"))
                    imgbtn.Visible = true;
                else
                    imgbtn.Visible = false;

                //if (((DocsPAWA.popup.DocInCestino.Cols)(e.Item.DataItem)).IsRimovibile != null)
                //{
                //    string isRimovibile = ((DocsPAWA.popup.DocInCestino.Cols)(e.Item.DataItem)).IsRimovibile.ToString();
                //    if (isRimovibile != null && isRimovibile.Equals("0"))
                //    {
                //        //ImageButton imgbtn = new ImageButton();
                //        if (e.Item.Cells[11].Controls[1].GetType().Equals(typeof(ImageButton)))
                //        {
                //            imgbtn = (ImageButton)e.Item.Cells[11].Controls[1];
                //            imgbtn.Visible = true;
                //        }
                //    }
                //}
            }
        }


        private void RefreshCountDocumenti()
        {
            string msg = "Elenco documenti";
            if (this.ListaDoc != null)
                msg += " - Trovati " + this.numeroDoc.ToString() + " elementi.";
            this.PrintMsg(msg);
        }

        protected void PrintMsg(string msg)
        {
            this.titolo.Text = msg;
        }

        #endregion

        #region Message-Box
        private void Msg_Ripristina_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                RiattivaDoc();
            }
        }

        private void Msg_SvuotaCestino_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                SvuotaCestino();
            }
        }

        private void Msg_EliminaDoc_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                EliminaDoc();
            }
        }
        #endregion

        #region Operazioni di rimozione/ripristino
        private void RiattivaDoc()
        {
            ListaDoc = ((DocsPAWA.DocsPaWR.InfoDocumento[])Session["ListaDocCestino"]);
            DocsPAWA.DocsPaWR.InfoDocumento infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento)ListaDoc[DGDoc.SelectedIndex];

            // string DocData = ((Label)this.DGDoc.SelectedItem.Cells[0].Controls[1]).Text;
            // DocData = DocData.Substring(0, DocData.Length - 11);
            bool result = DocumentManager.riattivaDocumento(this, UserManager.getInfoUtente(this), infoDoc);
            if (result)
            {
                BindGrid();
            }
            else
            {
                Response.Write("<script>alert('Attenzione operazione non riuscita');</script>");
            }
        }

        private void btn_svuota_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //La rimozione dei documenti è un privilegio di solo alcuni utenti... 
            //Verifica che l'utente connesso sia abilitato a svuotare i cestini
            if (!UserManager.ruoloIsAutorized(this, "SVUOTA_CESTINO"))
            {
                Response.Write("<script>alert('Utente non abilitato a questa operazione.')</script>");
            }
            else
            {
                //Conferma utente
                string messaggio = InitMessageXml.getInstance().getMessage("SVUOTA_CESTINO");
                Msg_SvuotaCestino.Confirm(messaggio);
            }
        }
        public DocsPAWA.DocsPaWR.InfoDocumento[] addListaNew(DocsPAWA.DocsPaWR.InfoDocumento[] lista, DocsPAWA.DocsPaWR.InfoDocumento infoDoc)
        {
           DocsPAWA.DocsPaWR.InfoDocumento[] nuovaLista;
           if (lista != null)
           {
              int len = lista.Length;
              nuovaLista = new DocsPAWA.DocsPaWR.InfoDocumento[len + 1];
              lista.CopyTo(nuovaLista, 0);
              nuovaLista[len] = infoDoc;
           }
           else
           {
              nuovaLista = new DocsPAWA.DocsPaWR.InfoDocumento[1];
              nuovaLista[0] = infoDoc;
           }
           return nuovaLista;
        }

        private void SvuotaCestino()
        {

         
            ListaDoc = ((DocsPAWA.DocsPaWR.InfoDocumento[])Session["ListaDocCestino"]);
            
           DocsPAWA.DocsPaWR.InfoDocumento[] nuovaLista = null;
           for (int i = 0; i < ListaDoc.Length; i++)
           {
              //if (ListaDoc[i].isRimovibile == "0")
                 nuovaLista = addListaNew(nuovaLista, ListaDoc[i]);
           }
           


            bool docInCestino = false;
           bool result = false;
            if(nuovaLista!=null && nuovaLista.Length>0)
               result = DocumentManager.svuotaCestino(out docInCestino, this, UserManager.getInfoUtente(this), nuovaLista);
            else
               Response.Write("<script>alert('Non si possiedono i diritti necessari per rimuovere i documenti');</script>");
           
            if(result && (ListaDoc.Length > nuovaLista.Length))
               docInCestino=true;

            if (result)
            {
               DGDoc.Visible = false;
               
                if (docInCestino)
                {
                    //Ci sono ancora documenti nel cestino
                    btnFilterDocs.Visible = true;
                    btnShowAllDocs.Enabled = false;
                    titolo.Text = "";
                    BindGrid();
                    DGDoc.Visible = true;
                }
                else
                {
                    btnFilterDocs.Enabled = false;
                    btnShowAllDocs.Enabled = false;
                    btn_stampa.Visible = false;
                    btn_svuota.Visible = false;
                    titolo.Text = "Nessun documento in cestino.";
                }
            }
            else
            {
                Response.Write("<script>alert('Attenzione operazione non riuscita');</script>");
            }
        }

        private void EliminaDoc()
        {

            ListaDoc = ((DocsPAWA.DocsPaWR.InfoDocumento[])Session["ListaDocCestino"]);
            DocsPAWA.DocsPaWR.InfoDocumento infoDoc = new DocsPAWA.DocsPaWR.InfoDocumento();
            infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento) ListaDoc[DGDoc.SelectedIndex];

            bool result = DocumentManager.EliminaDoc(this, UserManager.getInfoUtente(this), infoDoc);

            if (result)
                BindGrid();
        }
        #endregion

        #region gestione filtri di ricerca documenti

        /// <summary>
        /// Reperimento filtri di ricerca correntemente impostati sui documenti
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.FiltroRicerca[][] GetFiltriRicercaDocumenti()
        {
            return ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.GetCurrentFilter();
        }

        private void btnFilterDocs_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (this.txtFilterDocumentsRetValue.Value == "true")
            {
                BindGrid();
                btnShowAllDocs.Enabled = true;
                btnFilterDocs.Enabled = true;
            }
        }

        private void btnShowAllDocs_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            // Rimozione del filtro correntemente impostato sui documenti
            ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.RemoveCurrentFilter();

            // Impostazione dell'indice della pagina corrente del datagrid a 0
            //this.dt_Prot.CurrentPageIndex = 0;

            //FascicoliManager.SetProtoDocsGridPaging(this, this.dt_Prot.CurrentPageIndex);

            // Caricamento dati senza filtri impostati
            //this.FillData(this.dt_Prot.CurrentPageIndex + 1);
            BindGrid();
            btnShowAllDocs.Enabled = false;
            btnFilterDocs.Enabled = true;
        }

        #endregion

        protected void btnChiudi_Click(object sender, ImageClickEventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chiudi", "<script language=javascript>window.close();</script>");
                   
        }

        //INSERITA DA FABIO PRENDE LE ETICHETTE DEI PROTOCOLLI
        private void getLettereProtocolli()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = session.getUserAmmSession();
            String idAmm = null;
            if (UserManager.getInfoUtente() != null && !string.IsNullOrEmpty(UserManager.getInfoUtente().idAmministrazione))
                idAmm = UserManager.getInfoUtente().idAmministrazione;
            this.etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
        }

        //CALCOLA ETICHETTA PROTOCOLLI
        private string getEtichetta(String etichetta)
        {
            if (etichetta.Equals("A"))
            {
                return this.etichette[0].Descrizione;
            }
            else
            {
                if (etichetta.Equals("P"))
                {
                    return this.etichette[1].Descrizione;
                }
                else
                {
                    if (etichetta.Equals("I"))
                    {
                        return this.etichette[2].Descrizione;
                    }
                    else
                    {
                        if (etichetta.Equals("ALL"))
                        {
                            return this.etichette[4].Descrizione;
                        }
                        else
                        {
                            return this.etichette[3].Descrizione;
                        }
                    }
                }
            }
        }
    }
}
