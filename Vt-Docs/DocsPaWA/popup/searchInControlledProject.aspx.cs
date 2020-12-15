using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using System.Collections;
using DocsPAWA.SiteNavigation;
using System.Drawing;

namespace DocsPAWA.popup
{
    public partial class searchInControlledProject : DocsPAWA.CssPage
    {
        protected InfoDocumento[] Result;
        protected EtichettaInfo[] etichette;
        protected InfoUtente infoUtente;
        protected DocsPaWebService wws;
        protected Corrispondente cr;
        protected string idAmm;
        protected int RecordCount;
        protected int PageCount;
        protected Hashtable hash_checked;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] ListaFiltri;
        protected static int currentPage;
        protected DocsPAWA.DocsPaWR.InfoDocumento[] infoDoc;
        protected int numTotPage;
        protected int nRec;
        protected ArrayList Dg_elem;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;

        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
            this.chkSelectDeselectAll.CheckedChanged += new System.EventHandler(this.chkSelectDeselectAll_CheckedChanged);
            this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
            this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChange);
            this.DataGrid1.PreRender += new System.EventHandler(this.DataGrid1_PreRender);
            this.DataGrid1.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid1_ItemDataBound);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.infoUtente = new InfoUtente();
            this.wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            this.cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
            this.idAmm = cr.idAmministrazione;
            this.infoUtente = UserManager.getInfoUtente(this);
            getLettereProtocolli();

            if (!this.IsPostBack)
            {
                this.InitializePage();
            }

            if (DataGrid1.Visible)
            {
                //prendo la hasc in sessione
                hash_checked = DocumentManager.getHash(this);
                if (hash_checked == null)
                {
                    hash_checked = new Hashtable();
                }

                //salvo i check spuntati alla pagina cliccata in precedenza
                foreach (DataGridItem dgItem in DataGrid1.Items)
                {
                    CheckBox checkBox = dgItem.Cells[0].Controls[0].FindControl("chkFascicola") as CheckBox;
                    Label lbl_key = (Label)dgItem.Cells[8].Controls[1];

                    if (lbl_key != null && checkBox != null)
                    {
                        if (checkBox.Checked)//se è spuntato lo inserisco
                        {
                            if (!hash_checked.ContainsKey(lbl_key.Text))
                            {
                                hash_checked.Add(lbl_key.Text, lbl_key.Text);
                            }
                        }
                        else //se non è selezionato vedo se è in hashtable, in caso lo rimuovo
                        {
                            if (hash_checked.ContainsKey(lbl_key.Text))
                            {
                                hash_checked.Remove(lbl_key.Text);
                            }
                        }
                    }
                }
                //setto il sessione la HASH che contiene gli item selezionati
                DocumentManager.setHash(this, hash_checked);
                if (!this.IsPostBack)
                {
                    SelectAllCheck(true);
                    chkSelectDeselectAll.Checked = true;
                }
            }
            else
            {
                this.chkSelectDeselectAll.Visible = false;
            }

        }

        private void InitializePage()
        {
            // Startup della pagina
            Utils.startUp(this);
            ViewState["SelectDeselectAllChecked"] = true;
            this.Result = null;
            this.Folder = this.GetFolder();
            currentPage = 1;
            LoadData(true);

        }


        private void getLettereProtocolli()
        {
            this.etichette = wws.getEtichetteDocumenti(infoUtente, this.idAmm);
        }

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

        public string getNumProt(string numprot)
        {
            string rtn = " ";
            try
            {
                if (numprot == null)
                    return rtn = "";
                else
                    if (numprot == "0" || numprot.Trim() == "")
                        return rtn = "";
                    else
                        return rtn = numprot;

            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
                return rtn;
            }

        }

        protected void Grid_OnItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        private void btn_ok_Click(object sender, System.EventArgs e)
        {
            setDocInTrasm();
        }

        private void setDocInTrasm()
        {
            Session["Controllati"] = null;
            string[] idDocControllati = new string[hash_checked.Count];

            hash_checked = DocumentManager.getHash(this);

            if (hash_checked != null && hash_checked.Count > 0)
            {
                infoDoc = RicercaDocumentiControllatiSessionMng.GetListaInfoDocumenti(this);

                int i = 0;
                if (this.Folder != null && this.Folder.idFascicolo != "")
                {
                    foreach (DictionaryEntry de in hash_checked)
                    {
                        idDocControllati[i] = de.Value.ToString();
                        i++;
                    }
                }
                RicercaDocumentiControllatiSessionMng.ClearSessionData(this);

                Session["Controllati"] = idDocControllati;

                Response.Write("<script>window.close();</script>");
            }

            else
            {
                Response.Write("<script>alert('Selezionare almeno un documento da trasmettere');</script>");
            }


        }

        private void btn_chiudi_Click(object sender, System.EventArgs e)
        {
            Session["Controllati"] = null;
            Response.Write("<script>window.close();</script>");
        }

        private void LoadData(bool updateGrid)
        {

            ListaFiltri = DocumentManager.getFiltroRicDoc(this);
            DocsPaWR.Registro regSel = UserManager.getRegistroSelezionato(this);
            SearchResultInfo[] idProfileList;
            //AL POSTO DELL'UNO IL NUMERO DELLA PAGINA
            infoDoc = FascicoliManager.getListaDocumentiPaging(this, this.Folder, currentPage, out numTotPage, out nRec, true, out idProfileList);

            this.DataGrid1.VirtualItemCount = nRec;
            this.DataGrid1.CurrentPageIndex = currentPage - 1;

            string[] idProfs = new string[idProfileList.Length];
            for (int i = 0; i < idProfileList.Length; i++)
            {
                idProfs[i] = idProfileList[i].Id;
            }
            RicercaDocumentiControllatiSessionMng.SetListaIdProfile(this, idProfs);
            RicercaDocumentiControllatiSessionMng.SetListaInfoDocumenti(this, infoDoc);

            if (infoDoc != null && infoDoc.Length > 0)
            {
                this.BindGrid(infoDoc);
            }
            else
            {
                //rendo invisibile il check per la selezione di tutti i checkbox
                this.chkSelectDeselectAll.Visible = false;
            }
        }

        public void BindGrid(DocsPAWA.DocsPaWR.InfoDocumento[] infos)
        {
            DocsPaWR.InfoDocumento currentDoc;

            if (infos != null && infos.Length > 0)
            {
                //Costruisco il datagrid
                Dg_elem = new ArrayList();
                string descrDoc = string.Empty;
                int numProt = new Int32();

                for (int i = 0; i < infos.Length; i++)
                {
                    currentDoc = ((DocsPAWA.DocsPaWR.InfoDocumento)infos[i]);

                    string data = "";
                    if (currentDoc.dataApertura != null && currentDoc.dataApertura.Length > 0)
                        data = currentDoc.dataApertura.Substring(0, 10);

                    if (currentDoc.numProt != null && !currentDoc.numProt.Equals(""))
                    {
                        numProt = Int32.Parse(currentDoc.numProt);
                        descrDoc = numProt.ToString();
                    }
                    else //se il doc è grigio
                    {
                        descrDoc = currentDoc.docNumber;
                    }

                    descrDoc = descrDoc + "\n" + data;

                    bool fascicola = true;
                    string nuova_etichetta = string.Empty;
                    if (currentDoc.dataAnnullamento != null && currentDoc.dataAnnullamento != "")
                    {
                        fascicola = false;
                    }
                    string tipoProto = string.Empty;
                    if (currentDoc.tipoProto != null && currentDoc.tipoProto == "G")
                        tipoProto = "NP";
                    else
                        tipoProto = currentDoc.tipoProto;

                    nuova_etichetta = getEtichetta(tipoProto);
                    Dg_elem.Add(new RicercaDocumentiPerControllatiDataGridItem(descrDoc, currentDoc.idProfile, currentDoc.numProt, currentDoc.codRegistro, currentDoc.oggetto, nuova_etichetta, currentDoc.dataAnnullamento, i, fascicola));
                }

                this.DataGrid1.SelectedIndex = -1;
                this.DataGrid1.DataSource = Dg_elem;
                this.DataGrid1.DataBind();
                this.DataGrid1.Visible = true;
                this.chkSelectDeselectAll.Enabled = true;
                this.chkSelectDeselectAll.Visible = true;
            }
            else
            {
                this.DataGrid1.Visible = false;
                this.chkSelectDeselectAll.Visible = false;
            }
        }

        private Folder Folder
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["Folder"] as Folder;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["Folder"] = value;
            }
        }

        private Folder GetFolder()
        {
            // Folder da restituire
            Folder toReturn;

            Hashtable hashFolder = FascicoliManager.getHashFolder(this);

            int idFolder = 0;
            if (!String.IsNullOrEmpty(Request["idFolder"]))
                Int32.TryParse(Request["idFolder"], out idFolder);

            // Reperimento del folder dall'hashFolder
            toReturn = hashFolder[idFolder] as Folder;

            // Restituzione del folder
            return toReturn;

        }

        private void chkSelectDeselectAll_CheckedChanged(object sender, System.EventArgs e)
        {
            this.SelectAllCheck(((CheckBox)sender).Checked);
        }

        /// <summary>
        /// Gestione selezione / deselezione di tutti i checkbox colonna associa
        /// </summary>
        /// <param name="value"></param>
        private void SelectAllCheck(bool value)
        {


            DataGridItemCollection gridItems = this.DataGrid1.Items;
            string[] IdProfiles = RicercaDocumentiControllatiSessionMng.GetListaIdProfile(this);
            if (IdProfiles != null)
            {
                ViewState["SelectDeselectAllChecked"] = value;

                // foreach (DataGridItem gridItem in gridItems)
                foreach (string infoD in IdProfiles)
                {



                    if (value)
                    {
                        if (!hash_checked.ContainsKey(infoD))
                            hash_checked.Add(infoD, infoD);
                    }
                    else
                    {
                        if (hash_checked.Contains(infoD))
                            hash_checked.Remove(infoD);
                    }

                }
                foreach (DataGridItem gridItem in gridItems)
                {
                    string dataAnnull = ((Label)gridItem.Cells[5].Controls[1]).Text;
                    CheckBox checkBox =
                        gridItem.Cells[0].Controls[0].FindControl("chkFascicola") as CheckBox;

                    //salto il settaggio del valore per i checkbox relativi a documenti annullati
                    if (checkBox != null && string.IsNullOrEmpty(dataAnnull))
                        checkBox.Checked = value;
                }
            }
        }

        /// <summary>
        /// Impostazione del colore del carattere per la prima colonna della griglia:
        /// rosso se doc protocollato, altrimenti grigio, sbarrato se il protocollo è annullato
        /// </summary>
        /// <param name="item"></param>
        private void DataGrid1_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                e.Item.Cells[1].Font.Bold = true;
                CheckBox checkBox = e.Item.Cells[0].Controls[0].FindControl("chkFascicola") as CheckBox;

                Label lbl = (Label)e.Item.Cells[9].Controls[1];
                Label key = (Label)e.Item.Cells[8].Controls[1];

                string dataAnnull = ((Label)e.Item.Cells[5].Controls[1]).Text;

                if (lbl.Text == "") //doc grigi/pred
                {
                    e.Item.Cells[1].ForeColor = Color.Black;
                }
                else //doc protocollati
                {
                    e.Item.Cells[1].ForeColor = Color.Red;

                    //disabilito l'immagine per selezionare il documento protocollato
                    //poichè se il protocollo è annullato non deve poter essere classificato

                    if (dataAnnull != null)
                    {
                        if (dataAnnull == String.Empty)//se il protocollo non è annullato
                        {
                            checkBox.Enabled = true;
                        }
                        else
                        {
                            checkBox.Enabled = false;
                            checkBox.ToolTip = "Il documento è annullato, non è possibile \ninserirlo in un fascicolo";
                        }

                        try
                        {
                            DateTime dt = Convert.ToDateTime(dataAnnull);
                            e.Item.ForeColor = Color.Red;
                            e.Item.Font.Bold = true;
                            e.Item.Font.Strikeout = true;
                        }
                        catch { }
                    }
                }
                //prendo la hashTable dei checkbox ceccati
                hash_checked = DocumentManager.getHash(this);
                if (hash_checked != null)
                {
                    if (hash_checked.ContainsKey(key.Text))
                    {
                        checkBox.Checked = true;
                    }
                }
            }
        }

        private void DataGrid1_PreRender(object sender, System.EventArgs e)
        {

            bool SelectDeselectAllChecked = (bool)ViewState["SelectDeselectAllChecked"];

            chkSelectDeselectAll.Checked = SelectDeselectAllChecked;
            DataGrid dg = ((DataGrid)sender);
            for (int i = 0; i < dg.Items.Count; i++)
            {
                string dataAnnull = ((Label)this.DataGrid1.Items[i].Cells[5].Controls[1]).Text;

                if (dataAnnull != null)
                {
                    if (dataAnnull != String.Empty)//se il protocollo è annullato
                    {
                        dg.Items[i].ToolTip = "Il documento è annullato: non è possibile inserirlo in un fascicolo.";
                    }
                }
            }
            //se il datagrid ha un solo item allora lo rendo spuntato di default
            if (dg != null && dg.Items != null && dg.Items.Count == 1 && dg.PageCount == 1)
            {
                CheckBox checkBox = dg.Items[0].Cells[0].Controls[0].FindControl("chkFascicola") as CheckBox;
                checkBox.Checked = true;
                this.chkSelectDeselectAll.Enabled = false;
            }
        }

        private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName == "ShowInfo")
            {

                DocsPaWR.InfoDocumento newinfoDoc = null;
                if (e.Item.ItemIndex >= 0)
                {
                    string str_indexSel = ((Label)this.DataGrid1.Items[e.Item.ItemIndex].Cells[7].Controls[1]).Text;
                    int indexSel = Int32.Parse(str_indexSel);

                    this.infoDoc = RicercaDocumentiControllatiSessionMng.GetListaInfoDocumenti(this);

                    if (indexSel > -1)
                        newinfoDoc = (DocsPAWA.DocsPaWR.InfoDocumento)this.infoDoc[indexSel];

                    if (newinfoDoc != null)
                    {
                        DocumentManager.setRisultatoRicerca(this, newinfoDoc);
                        FascicoliManager.removeFascicoloSelezionato(this);
                        FascicoliManager.removeFolderSelezionato(this);
                        RicercaDocumentiControllatiSessionMng.ClearSessionData(this);
                        Response.Write("<script>window.open('../documento/gestionedoc.aspx?tab=protocollo','principale');</script>");
                        Response.Write("<script>window.close();</script>");

                    }
                }
            }
        }

        private void DataGrid1_PageIndexChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            currentPage = e.NewPageIndex + 1;
            this.LoadData(true);
        }

        #region classe per la gestione della sessione
		/// <summary>
		/// Classe per la gestione dei dati in sessione relativamente
		/// alla dialog "RicercaDocumentiPerClassifica"
		/// </summary>
        public sealed class RicercaDocumentiControllatiSessionMng
        {
            private RicercaDocumentiControllatiSessionMng()
            {
            }
            /// <summary>
            /// Gestione rimozione dati in sessione
            /// </summary>
            /// <param name="page"></param>
            public static void ClearSessionData(Page page)
            {
                page.Session.Remove("DocumentiPerControllatiSessionMng.dialogReturnValue");
            }

            public static void SetListaIdProfile(Page page, string[] listaIdprofile)
            {
                page.Session["DocumentiPerControllati.ListaIdprofile"] = listaIdprofile;
            }

            public static string[] GetListaIdProfile(Page page)
            {
                return page.Session["DocumentiPerControllati.ListaIdprofile"] as string[];
            }

            public static void SetListaInfoDocumenti(Page page, DocsPaWR.InfoDocumento[] listaDocumenti)
            {
                page.Session["DocumentiPerControllati.ListaInfoDoc"] = listaDocumenti;
            }

            public static DocsPAWA.DocsPaWR.InfoDocumento[] GetListaInfoDocumenti(Page page)
            {
                return page.Session["DocumentiPerControllati.ListaInfoDoc"] as DocsPAWA.DocsPaWR.InfoDocumento[];
            }

            public static void RemoveListaInfoDocumenti(Page page)
            {
                page.Session.Remove("DocumentiPerControllati.ListaInfoDoc");
            }

            /// <summary>
            /// Impostazione flag booleano, se true, la dialog è stata caricata almeno una volta
            /// </summary>
            /// <param name="page"></param>
            public static void SetAsLoaded(Page page)
            {
                page.Session["DocumentiPerControllatiSessionMng.isLoaded"] = true;
            }

            /// <summary>
            /// Impostazione flag relativo al caricamento della dialog
            /// </summary>
            /// <param name="page"></param>
            public static void SetAsNotLoaded(Page page)
            {
                page.Session.Remove("DocumentiPerControllatiSessionMng.isLoaded");
            }

            /// <summary>
            /// Verifica se la dialog è stata caricata almeno una volta
            /// </summary>
            /// <param name="page"></param>
            /// <returns></returns>
            public static bool IsLoaded(Page page)
            {
                return (page.Session["DocumentiPerControllatiSessionMng.isLoaded"] != null);
            }

            /// <summary>
            /// Impostazione valore di ritorno
            /// </summary>
            /// <param name="page"></param>
            /// <param name="dialogReturnValue"></param>
            public static void SetDialogReturnValue(Page page, bool dialogReturnValue)
            {
                page.Session["DocumentiPerControllatiSessionMng.dialogReturnValue"] = dialogReturnValue;
            }

            /// <summary>
            /// Reperimento valore di ritorno
            /// </summary>
            /// <param name="page"></param>
            /// <returns></returns>
            public static bool GetDialogReturnValue(Page page)
            {
                bool retValue = false;

                if (IsLoaded(page))
                    retValue = Convert.ToBoolean(page.Session["DocumentiPerControllatiSessionMng.dialogReturnValue"]);

                page.Session.Remove("DocumentiPerControllatiSessionMng.isLoaded");

                return retValue;
            }

        #endregion

        }
            #region classe per la creazione del datagrid

		public class RicercaDocumentiPerControllatiDataGridItem 
		{		
			private string descDoc;
            private string idProfile;
            private string numProt;
			private string registro;
			private string oggetto;
			private string tipo;
			private string dataAnnullamento;
			private int chiave;
			private bool fascicola;
		

			public RicercaDocumentiPerControllatiDataGridItem(string descDoc,string idProfile,string numProt, string registro, string oggetto, string tipo, string dataAnnullamento, int chiave, bool fascicola)
			{
				this.descDoc = descDoc;
				this.idProfile = idProfile;
                this.numProt = numProt;
				this.registro = registro;
				this.oggetto = oggetto;
				this.tipo = tipo;
				this.dataAnnullamento = dataAnnullamento;
				this.chiave = chiave;
				this.fascicola=fascicola;
			}
					
			public string DescDoc{get{return descDoc;}}
            public string IdProfile { get { return idProfile; } }
            public string NumProt { get { return numProt; } }
			public string Registro{get{return registro;}}
			public string Oggetto{get{return oggetto;}}
			public string Tipo{get{return tipo;}}
			public string DataAnnullamento{get{return dataAnnullamento;}}
			public int    Chiave{get{return chiave;}}
			public bool   Fascicola{get{return fascicola;}}
		}

		#endregion

    }
}