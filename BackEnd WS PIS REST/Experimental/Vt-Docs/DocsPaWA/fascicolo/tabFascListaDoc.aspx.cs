// #define Debug

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
using DocsPAWA.UserControls;
using System.Collections.Generic;
using DocsPAWA.DocsPaWR;
using log4net;

namespace DocsPAWA.fascicolo
{
    /// <summary>
    /// Summary description for tabFascListaDoc.
    /// </summary>
    public class tabFascListaDoc : DocsPAWA.CssPage
    {
        #region WebControls / variabili / costanti
        protected System.Web.UI.WebControls.DataGrid dt_Prot;
        //	protected System.Web.UI.WebControls.DataGrid dt_NonProt;					
        protected DataTable dataTableProt;
        //	protected DataTable dataTableNonProt;
        protected Hashtable hashListaDocumenti;
        protected DocsPAWA.dataSet.DataSetRFascDoc dataSetRFascDoc1;
        protected System.Web.UI.HtmlControls.HtmlTableRow trHeader;
        protected System.Web.UI.HtmlControls.HtmlTableCell TD1;
        protected System.Web.UI.HtmlControls.HtmlTableCell TD3;
        //protected System.Web.UI.WebControls.Label lbl_docProtocollati;
        protected System.Web.UI.WebControls.Label titolo;
        //	protected System.Web.UI.WebControls.Label lbl_docNonProtocollati;
        protected System.Web.UI.HtmlControls.HtmlTableRow tr_gridDocProt;

        System.Web.UI.WebControls.Label lbl_idprofile;
        System.Web.UI.WebControls.Label lbl_chaImg;
        protected System.Web.UI.WebControls.ImageButton btn_stampa;
        //-------------------------------------------------------------------------------------
        protected DocsPAWA.DocsPaWR.Folder folder;
        protected int nRec;
        protected int numTotPage;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_gridDocProt;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtFilterDocumentsRetValue;
        protected DocsPaWebCtrlLibrary.ImageButton btnFilterDocs;
        protected DocsPaWebCtrlLibrary.ImageButton btnShowAllDocs;
        protected System.Web.UI.HtmlControls.HtmlTableRow tr_filterButtons;
        protected System.Web.UI.HtmlControls.HtmlGenericControl pnlContainer;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_docNonTrovati;
        //-------------------------------------------------------------------------------------

        protected DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;

        private ILog logger = LogManager.GetLogger(typeof(tabFascListaDoc));
       

        #endregion

        #region Page Load
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                logger.Debug("tabfasclistadoc.aspx_pg");
                Utils.startUp(this);

                getLettereProtocolli();
                logger.Debug("tabfasclistadoc.aspx_pg2");
               
                if (!Page.IsPostBack)
                {
                    this.btn_stampa.Attributes.Add("onclick", "StampaRisultatoRicerca();");

                    this.AttachWaitingControl();

                    this.dt_Prot.CurrentPageIndex = this.GetCurrentPageIndex();
                    logger.Debug("tabfasclistadoc.aspx_pg3");
               
                    FascicoliManager.SetProtoDocsGridPaging(this, 0);
                    logger.Debug("tabfasclistadoc.aspx_pg4");
               
                    this.FillData(this.dt_Prot.CurrentPageIndex + 1);
                    logger.Debug("tabfasclistadoc.aspx_pg5");
               
                }
                //else
                //{
                //    bool filtra = ((Request.QueryString["FilterDocumentsRetValue"] != null && Request.QueryString["FilterDocumentsRetValue"].ToString() == "true") || ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.GetCurrentFilter() != null);

                //    if (filtra)
                //        this.FiltraDocs();
                //    else
                //        this.RimuoviFiltriDocs();
                //}
            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {

            logger.Debug("  tabfasclistadoc.aspx_pg.Page_PreRender");


            this.RefreshControls();
            this.OnGridPreRender(this.dt_Prot);
            this.RefreshCountDocumenti();

            this.SetDocumentIndexFromQueryString(this.dt_Prot);
            logger.Debug("  tabfasclistadoc.aspx_pg.Page_PreRender2");
        }

        /// <summary>
        /// Aggiornamento visibilità / abilitazione controlli 
        /// </summary>
        private void RefreshControls()
        {
            this.dt_Prot.Visible = (this.TotalRecordCount > 0);
            this.btn_stampa.Visible = this.dt_Prot.Visible;

            this.EnableFilterButtons();
        }

        /// <summary>
        /// Caricamento dati
        /// </summary>
        private void FillData(int requestedPage)
        {
            Hashtable hashFolder;
            int idFolder;
            DocsPaWR.Folder folderToView;

            try
            {
                hashFolder = FascicoliManager.getHashFolder(this);

                dataTableProt = new DataTable();

                if (hashFolder != null)
                {
                    idFolder = this.GetIDFolderParameter();

                    folderToView = (DocsPAWA.DocsPaWR.Folder)hashFolder[idFolder];

                    if (folderToView != null)
                    {
                        FascicoliManager.setFolderSelezionato(this, folderToView);

                        // Caricamento dati paginati
                        caricaDataTablesPaging(folderToView,
                                                requestedPage,
                                                out nRec,
                                                out numTotPage);
                    }
                }

                this.dt_Prot.VirtualItemCount = nRec;
                this.dt_Prot.DataSource = dataTableProt;
                this.dt_Prot.DataBind();
            }
            catch (Exception es)
            {
                string error = es.ToString();
            }

            FascicoliManager.SetFolderViewTracing(this, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numprot"></param>
        /// <returns></returns>
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

        #region Caricamento dei dati e visibilità delle griglie

        /// <summary>
        /// Restituzione dell'IDFolder fornito in ingresso.
        /// Se l'ID non è presente nella chiave di sessione
        /// temporanea "tabFascListaDoc.idFolder", il valore
        /// viene reperito dalla querystring.
        /// </summary>
        /// <returns></returns>
        private int GetIDFolderParameter()
        {
            int retValue = 0;

            // verifica presenza chiave di sessione temporanea
            if (Session["tabFascListaDoc.idFolder"] != null)
            {
                retValue = Convert.ToInt32(Session["tabFascListaDoc.idFolder"]);
                // rimozione chiave di sessione temporanea
                //Session.Remove("tabFascListaDoc.idFolder");
            }
            else
            {
                if (string.IsNullOrEmpty( Request.QueryString["idFolder"]))
                    retValue = 0;
                else
                    retValue = Convert.ToInt32(Request.QueryString["idFolder"]);
            }

            return retValue;
        }

        /// <summary>
        /// Bind dei datagrid
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="dataGrid"></param>
        private void bindDataTableToDataGrid(DataTable dataTable, DataGrid dataGrid)
        {
            dataGrid.DataSource = dataTable;
            dataGrid.DataBind();
        }

        #endregion

        #region Paginazione dei dati

        /// <summary>
        /// Paging
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="numPage"></param>
        /// <param name="nRec"></param>
        /// <param name="numTotPage"></param>
        private void caricaDataTablesPaging(DocsPAWA.DocsPaWR.Folder folder,
                                            int numPage,
                                            out int nRec,
                                            out int numTotPage)
        {
            logger.Debug("tabfasclistadoc.aspx_pg.caricaDataTablesPaging");
               
            nRec = 0;
            numTotPage = 0;
            try
            {
                dataTableProt = new DataTable();
                //		dataTableNonProt=new DataTable();
                hashListaDocumenti = new Hashtable();

                DocsPaWR.FiltroRicerca[][] filtriRicercaDocumenti = GetFiltriRicercaDocumenti();

                DocsPaWR.InfoDocumento[] listaDoc = null;
                SearchResultInfo[] idProfileList;
                if (filtriRicercaDocumenti == null)
                    listaDoc = FascicoliManager.getListaDocumentiPaging(this, folder, numPage, out numTotPage, out nRec, false, out idProfileList);
                else
                    listaDoc = FascicoliManager.getListaDocumentiPaging(this, folder, filtriRicercaDocumenti, numPage, out numTotPage, out nRec, false, out idProfileList);

                this.TotalRecordCount = nRec;
                this.dt_Prot.VirtualItemCount = this.TotalRecordCount;

                if (listaDoc != null)
                {
                    if (listaDoc.Length > 0)
                    {
                        //descrizione documento (prot + data)
                        string descDoc = string.Empty;
                        this.dataSetRFascDoc1.element1.Rows.Clear();
                        for (int i = 0; i < listaDoc.Length; i++)
                        {


                            hashListaDocumenti.Add(i, listaDoc[i]);
                            DocsPaWR.InfoDocumento infoDoc = listaDoc[i];

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


                            // MODIFICATO DA FABIO PER ETICHETTE LETTERE PROTOCOLLI
                            if (infoDoc.tipoProto.Equals("A") || infoDoc.tipoProto.Equals("P") || infoDoc.tipoProto.Equals("I"))
                            {
                                //aggiunge al datatable dei protocollati
                                if (infoDoc.numProt != null && !infoDoc.numProt.Equals(""))
                                {
                                    numProt = Int32.Parse(infoDoc.numProt);
                                    descDoc = numProt + "\n" + dataApertura;
                                    //Protocollo titolario
                                    if (!string.IsNullOrEmpty(infoDoc.protocolloTitolario))
                                        descDoc += "<br>---------<br>" + infoDoc.protocolloTitolario.ToString();

                                    string nuova_etichetta = getEtichetta(infoDoc.tipoProto);
                                    this.dataSetRFascDoc1.element1.Addelement1Row(descDoc, numProt, dataApertura, infoDoc.segnatura, infoDoc.codRegistro, nuova_etichetta, infoDoc.oggetto, i, MittDest, infoDoc.dataAnnullamento, infoDoc.dataArchiviazione, infoDoc.idProfile, infoDoc.docNumber, infoDoc.acquisitaImmagine, infoDoc.inConservazione, infoDoc.cha_firmato, infoDoc.inADL);
                                }
                                else
                                {
                                    // se documento grigio/pred.
                                    descDoc = infoDoc.docNumber + "\n" + dataApertura;
                                    if (infoDoc.tipoProto != null && infoDoc.tipoProto == "G")
                                        tipoProto = "NP";
                                    else
                                        tipoProto = infoDoc.tipoProto;
                                    //Protocollo titolario
                                    if (!string.IsNullOrEmpty(infoDoc.protocolloTitolario))
                                        descDoc += "<br>---------<br>" + infoDoc.protocolloTitolario.ToString();

                                    string nuova_etichetta = getEtichetta(infoDoc.tipoProto);
                                    this.dataSetRFascDoc1.element1.Addelement1Row(descDoc, new Int32(), dataApertura, "", infoDoc.codRegistro, nuova_etichetta, infoDoc.oggetto, i, MittDest, infoDoc.dataAnnullamento, infoDoc.dataArchiviazione, infoDoc.idProfile, infoDoc.docNumber, infoDoc.acquisitaImmagine, infoDoc.inConservazione, infoDoc.cha_firmato, infoDoc.inADL);
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
                                //Protocollo titolario
                                if (!string.IsNullOrEmpty(infoDoc.protocolloTitolario))
                                    descDoc += "<br>---------<br>" + infoDoc.protocolloTitolario.ToString();

                                //aggiunge al datatable dei non protocollati
                                string nuova_etichetta = getEtichetta(infoDoc.tipoProto);
                                this.dataSetRFascDoc1.element1.Addelement1Row(descDoc, Int32.Parse(infoDoc.idProfile), dataApertura, "", "", nuova_etichetta, infoDoc.oggetto, i, "", "", infoDoc.dataArchiviazione, infoDoc.idProfile, infoDoc.docNumber, infoDoc.acquisitaImmagine, infoDoc.inConservazione, infoDoc.cha_firmato, infoDoc.inADL);
                            }
                        }
                    }

                }

                FascicoliManager.setHashDocProtENonProt(this, hashListaDocumenti);

                dataTableProt = dataSetRFascDoc1.Tables[0];

                // impostazione datatable in sessione
                FascicoliManager.setDataTableDocProt(this, dataTableProt);

                //Header protocollo titolario
                DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                string protocolloTitolario = wws.isEnableProtocolloTitolario();
                if (!string.IsNullOrEmpty(protocolloTitolario) && !IsPostBack)
                {
                    this.dt_Prot.Columns[0].HeaderText += "<br>---------<br>" + protocolloTitolario;
                }
            }
            catch (Exception es)
            {
                logger.Debug(es.Message+"  tabfasclistadoc.aspx_pg.caricaDataTablesPaging");
                ErrorManager.redirect(this, es);
            }
        }

        #endregion

        #region codice commentato

        /*		 
		private string getInactiveColorGridSeparator(DataGrid dataGrid)
		{
			return "#ffffff";
			//getHeaderGridBackColor(dataGrid);
		}

		private string getHeaderGridBackColor(DataGrid dataGrid)
		{
			return dataGrid.HeaderStyle.BackColor.ToArgb().ToString();
		}		
		*/

        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            //if (!this.DesignMode)
            //{
            //    if (Context != null && Context.Session != null && Session != null)
            //    {
                    InitializeComponent();
                    base.OnInit(e);
            //    }
            //}
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataSetRFascDoc1 = new DocsPAWA.dataSet.DataSetRFascDoc();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetRFascDoc1)).BeginInit();
            //this.dt_Prot.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.newGrid_ItemCommand);

            //this.dt_Prot.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dt_Prot_PageIndexChanged);
            //this.dt_Prot.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dt_Prot_ItemDataBound);
            //this.dt_Prot.SelectedIndexChanged += new System.EventHandler(this.dt_Prot_SelectedIndexChanged);
            //this.dt_Prot.PreRender += new EventHandler(dt_Prot_PreRender);
            //this.btnFilterDocs.Click += new System.Web.UI.ImageClickEventHandler(this.btnFilterDocs_Click);
            //this.btnShowAllDocs.Click += new System.Web.UI.ImageClickEventHandler(this.btnShowAllDocs_Click);
            // 
            // dataSetRFascDoc1
            // 
            this.dataSetRFascDoc1.DataSetName = "DataSetRFascDoc";
            this.dataSetRFascDoc1.Locale = new System.Globalization.CultureInfo("en-US");
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);
            ((System.ComponentModel.ISupportInitialize)(this.dataSetRFascDoc1)).EndInit();

        }
        #endregion

        #region Gestione dei datagrid

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dg"></param>
        private void OnGridPreRender(DataGrid dg)
        {
            int indCell;
            if (dg.ID == "dt_Prot")
                indCell = 9;
            else
                indCell = 4;


            //nascondo la colonna che cancella la classificazione:

            //1. se l'utente ha i diritti di sola lettura
            //DocsPaWR.Fascicolo Fasc=FascicoliManager.getFascicoloSelezionato(this);
            //if(Fasc !=null && Fasc.accessRights!=null && Fasc.accessRights!="")
            //{
            //    if(UserManager.disabilitaButtHMDiritti(Fasc.accessRights))
            //    {
            //        dg.Columns[10].Visible = false;
            //    }
            //}

            //2. se il fascicolo è chiuso 
            //if(Fasc !=null && Fasc.stato=="C")
            //{	
            //    dg.Columns[10].Visible = false;
            //}


            //			for(int i=0;i<dg.Items.Count;i++)
            //			{
            //				if(dg.Items[i].ItemIndex>=0)
            //				{	
            //					switch(dg.Items[i].ItemType.ToString().Trim())
            //					{
            //						case "Item":
            //						{
            //							dg.Items[i].Attributes.Add("onclick","this.className='bg_grigioS'");
            //							dg.Items[i].Cells[indCell].Attributes.Add ("onclick","if (!window.confirm('Il documento verrà rimosso dal fascicolo. Continuare?')) {return false};");
            //							break;
            //						}
            //						case "AlternatingItem":
            //						{
            //							dg.Items[i].Attributes.Add("onclick","this.className='bg_grigioS'");
            //							dg.Items[i].Cells[indCell].Attributes.Add ("onclick","if (!window.confirm('Il documento verrà rimosso dal fascicolo. Continuare?')) {return false};");
            //							break;
            //						}
            //					}
            //				}
            //			}
        }

        /// <summary>
        /// 
        /// </summary>
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
        }

        private void dettaglioDocumento(int idDocumento, string newUrl)
        {
            Hashtable hashDoc = FascicoliManager.getHashDocProtENonProt(this);

            if (hashDoc != null)
            {
                DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumento(this, ((DocsPAWA.DocsPaWR.InfoDocumento)hashDoc[idDocumento]).idProfile, ((DocsPAWA.DocsPaWR.InfoDocumento)hashDoc[idDocumento]).docNumber);
                if (((DocsPAWA.DocsPaWR.InfoDocumento)hashDoc[idDocumento]).inArchivio != "0")
                    schedaDoc.inArchivio = ((DocsPAWA.DocsPaWR.InfoDocumento)hashDoc[idDocumento]).inArchivio;
                if (schedaDoc != null)
                {
                    DocumentManager.setDocumentoSelezionato(this, schedaDoc);
                    Response.Write("<script>window.open('" + newUrl + "','principale');</script>");
                }
            }

        }

        /// <summary>
        /// Method used to send debug alerts to the client
        /// </summary>
        /// <param name="message"></param>
        private void SendDebugAlert(string message)
        {
#if Debug
			Response.Write("<script>alert('" + message + "');</script>");
#endif
        }

        protected void dt_Prot_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName == "eliminaDocInFasc")
            {
                this.SetDocumentIndexOnContext(-1);
                this.dt_Prot.SelectedIndex = -1;

                //string key=((Label)this.dt_Prot.Items[e.Item.ItemIndex].Cells[6].Controls[1]).Text;

                //eliminaDocDaFasciolo(Int32.Parse(key));

                this.dt_Prot.CurrentPageIndex = 0;
                this.FillData(this.dt_Prot.CurrentPageIndex + 1);
            }
            else
            {
                if (e.CommandName != "Page")
                {
                    string key = ((Label)this.dt_Prot.Items[e.Item.ItemIndex].Cells[6].Controls[1]).Text;
                    this.SetDocumentIndexOnContext(Convert.ToInt32(key));
                    this.dt_Prot.SelectedIndex = Convert.ToInt32(key);
                }
            }
            // visualizza doc in popup
            // if (e.CommandName.Equals("VisDoc"))
            // {
            //     //vis unificata
            //     string docNumber = ((Label)this.dt_Prot.Items[e.Item.ItemIndex].Cells[15].Controls[1]).Text;

            //     string idProfile = ((Label)this.dt_Prot.Items[e.Item.ItemIndex].Cells[14].Controls[1]).Text;

            //     DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumento(this, idProfile, docNumber);
            //     if (schedaDoc.documenti[0].fileSize != "" && schedaDoc.documenti[0].fileSize != "0")
            //     {
            //         DocumentManager.setDocumentoSelezionato(this, schedaDoc);
            //         FileManager.setSelectedFile(this, schedaDoc.documenti[0], false);
            //         ClientScript.RegisterStartupScript(this.GetType(), "lanciaVIs", "loadvisualizzaDoc('" + Session.SessionID + "','" + docNumber + "','" + idProfile + "');", true);
            //     }

            // }

            //if (e.CommandName.Equals("AreaConservazione"))
            // {

            //     string key_appo = ((Label)this.dt_Prot.Items[e.Item.ItemIndex].Cells[6].Controls[1]).Text;
            //     int key = Int32.Parse(key_appo);
            //     Hashtable hashDoc = FascicoliManager.getHashDocProtENonProt(this);

            //     DocsPaWR.InfoDocumento infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento)hashDoc[key];

            //     DocsPAWA.DocsPaWR.SchedaDocumento schedaSel = DocumentManager.getDettaglioDocumento(this, infoDoc.idProfile, infoDoc.docNumber);

            //     if (Convert.ToInt32(schedaSel.documenti[0].fileSize) > 0)
            //     {
            //         int isPrimaIstanza = DocsPAWA.DocumentManager.isPrimaConservazione(Page, (UserManager.getInfoUtente(this)).idPeople, (UserManager.getInfoUtente(this)).idGruppo);
            //         if (isPrimaIstanza == 1)
            //         {
            //             string popup = "<script> alert('Si sta per creare una nuova istanza di conservazione')</script>";
            //             Page.RegisterClientScriptBlock("popUp", popup);
            //         }
            //         DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato();
            //         if (fasc != null)
            //         {
            //             string sysId = DocumentManager.addAreaConservazione(Page, infoDoc.idProfile, fasc.systemID, infoDoc.docNumber, UserManager.getInfoUtente(this), "D");

            //             int size_xml = DocumentManager.getItemSize(Page, schedaSel, sysId);
            //             int doc_size = Convert.ToInt32(schedaSel.documenti[0].fileSize);
            //             int size_allegati = 0;
            //             for (int i = 0; i < schedaSel.allegati.Length; i++)
            //             {
            //                 size_allegati = size_allegati + Convert.ToInt32(schedaSel.allegati[i].fileSize);
            //             }
            //             int total_size = size_allegati + doc_size + size_xml;

            //             int numeroAllegati = schedaSel.allegati.Length;
            //             string fileName = schedaSel.documenti[0].fileName;
            //             string tipoFile = System.IO.Path.GetExtension(fileName);

            //             DocumentManager.insertSizeInItemCons(Page, sysId, total_size);

            //             DocumentManager.updateItemsConservazione(Page, tipoFile, Convert.ToString(numeroAllegati), sysId);
            //         }
            //         ((Label)e.Item.Cells[18].Controls[1]).Text = "1";
            //     }
            //     else
            //     {
            //         string popup = "<script> alert('Il documento principale non ha alcun file associato, impossibile inserirlo in area conservazione')</script>";
            //         Page.RegisterClientScriptBlock("popUp", popup);
            //     }
            // }
            // if(e.CommandName.Equals("EliminaAreaCons"))
            // {

            //     string key_appo = ((Label)this.dt_Prot.Items[e.Item.ItemIndex].Cells[6].Controls[1]).Text;
            //     int key = Int32.Parse(key_appo);
            //Hashtable hashDoc = FascicoliManager.getHashDocProtENonProt(this);

            //         DocsPaWR.InfoDocumento infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento)hashDoc[key];

            //     if (DocumentManager.canDeleteAreaConservazione(Page, infoDoc.idProfile, (UserManager.getInfoUtente(this)).idPeople, (UserManager.getInfoUtente(this)).idGruppo) == 0)
            //     {
            //         string popup = "<script> alert('Impossibile eliminare il documento da Area di conservazione')</script>";
            //         Page.RegisterClientScriptBlock("alert", popup);
            //     }
            //     else
            //     {
            //         DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato();
            //         if (fasc != null)
            //         {
            //             DocumentManager.eliminaDaAreaConservazione(Page, infoDoc.idProfile, fasc, null, false, "");
            //         }
            //         else
            //         {
            //             DocumentManager.eliminaDaAreaConservazione(Page, infoDoc.idProfile, null, null, false, "");
            //         }
            //         ((System.Web.UI.WebControls.ImageButton)e.Item.Cells[17].Controls[1]).ToolTip = "Inserisci questo documento in 'Area di conservazione'";
            //         ((System.Web.UI.WebControls.ImageButton)e.Item.Cells[17].Controls[1]).ImageUrl = "../images/proto/conservazione_d.gif";
            //         ((System.Web.UI.WebControls.ImageButton)e.Item.Cells[17].Controls[1]).CommandName = "AreaConservazione";
            //         ((Label)e.Item.Cells[18].Controls[1]).Text = "0";
            //     }
            // }
            //if (e.CommandName.Equals("DocInFascicolo"))
            //{
            //    string script1 = "<script> alert('Il documento appartiene a un fascicolo, per rimuoverlo eliminare intero fascicolo da Area di Conservazione')</script>";
            //    Response.Write(script1);
            //    ((Label)e.Item.Cells[18].Controls[1]).Text = "2";
            //}

        }

        #endregion

        #region Elimina documenti

        /// <summary>
        /// Elimina un documento dal fascicolo
        /// </summary>
        /// <param name="key">Chiave del record da elimnare</param>
        private void deleteDocument(int key)
        {
            Hashtable hashDoc = FascicoliManager.getHashDocProtENonProt(this);

            if (hashDoc != null)
            {
                DocsPaWR.InfoDocumento infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento)hashDoc[key];
                if (infoDoc != null)
                {
                    string msg = string.Empty;
                    DocsPaWR.Folder fold = FascicoliManager.getFolderSelezionato(this);
                    string valoreChiaveFasc = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
                    if (string.IsNullOrEmpty(valoreChiaveFasc))
                        valoreChiaveFasc = "false";
                    DocsPAWA.DocsPaWR.ValidationResultInfo result = FascicoliManager.deleteDocFromFolder(this, fold, infoDoc.idProfile, valoreChiaveFasc, out msg);
                    if (result != null && result.BrokenRules.Length > 0)
                    {
                        DocsPAWA.DocsPaWR.BrokenRule br = (DocsPAWA.DocsPaWR.BrokenRule)result.BrokenRules[0];
                        ClientScript.RegisterStartupScript(this.GetType(), "sottonumeroSuccessivo", "alert('" + br.Description + "');", true);
                        return;
                    }
                    if (msg != string.Empty)
                    {
                        Response.Write("<script>alert('" + msg + "')</script>");
                        return;
                    }
                }
            }

            // rimuove la sessione della chiave che specifica il record da eliminare
            Page.Session.Remove("key");
        }

        #endregion

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
        private void eliminaDocDaFasciolo(int key)
        {
            try
            {
                if (((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_confirmDel")).Value == "si")
                {
                    deleteDocument(key);
                    ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_confirmDel")).Value = "";
                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }
     

        #region Gestione filtri sui documenti

        /// <summary>
        /// Reperimento filtri di ricerca correntemente impostati sui documenti
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.FiltroRicerca[][] GetFiltriRicercaDocumenti()
        {
            return ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.GetCurrentFilter();
        }

        /// <summary>
        /// Abilitazione / disabilitazione pulsanti di filtro documenti
        /// </summary>
        private void EnableFilterButtons()
        {
            bool isEnabled = (ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.GetCurrentFilter() != null);
            //this.btnShowAllDocs.Enabled=isEnabled;
        }

        private void btnFilterDocs_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            FiltraDocs();
        }



        private void FiltraDocs()
        {
            //if (bool.Parse(Request.QueryString["FilterDocumentsRetValue"].ToString()) == true)
            //{
                // Impostazione dell'indice della pagina corrente del datagrid a 0
                FascicoliManager.SetProtoDocsGridPaging(this, 0);

                this.dt_Prot.CurrentPageIndex = 0;

                // Caricamento dati applicando il filtro correntemente impostato
                this.FillData(this.dt_Prot.CurrentPageIndex + 1);
            //}
        }

        private void RimuoviFiltriDocs()
        {
            // Rimozione del filtro correntemente impostato sui documenti
            ricercaDoc.FiltriRicercaDocumenti.CurrentFilterSessionStorage.RemoveCurrentFilter();

             //Impostazione dell'indice della pagina corrente del datagrid a 0
            this.dt_Prot.CurrentPageIndex=0;

            FascicoliManager.SetProtoDocsGridPaging(this, this.dt_Prot.CurrentPageIndex);

            // Caricamento dati senza filtri impostati
            this.FillData(this.dt_Prot.CurrentPageIndex + 1);
        }

        private void btnShowAllDocs_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

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
                retValue = this.dt_Prot.CurrentPageIndex;
            }

            return retValue;
        }

        #endregion

        #region Gestione DataGridPagingWait

        /// <summary>
        /// Attatch del controllo "DataGridPagingWait" al datagrid
        /// </summary>
        private void AttachWaitingControl()
        {
            this.WaitingControl.DataGridID = this.dt_Prot.ClientID;

            this.WaitingControl.WaitScriptCallback = "WaitGridPagingAction();";
        }

        /// <summary>
        /// Reperimento controllo "DataGridPagingWait"
        /// </summary>
        private waiting.DataGridPagingWait WaitingControl
        {
            get
            {
                return this.FindControl("dt_Prot_pagingWait") as waiting.DataGridPagingWait;
            }
        }

        #endregion

        #region Gestione CallContext

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docIndex"></param>
        private void SetDocumentIndexOnContext(int docIndex)
        {
            logger.Debug( "  tabfasclistadoc.aspx_pg.SetDocumentIndexOnContext");
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext != null && currentContext.ContextName == SiteNavigation.NavigationKeys.FASCICOLO)
            {
                if (docIndex == -1)
                    currentContext.QueryStringParameters.Remove("docIndex");
                else
                    currentContext.QueryStringParameters["docIndex"] = docIndex.ToString();
            }
            logger.Debug("  tabfasclistadoc.aspx_pg.SetDocumentIndexOnContext_fine");
        }

        /// <summary>
        /// Ripristino del documento selezionato precedentemente
        /// </summary>
        /// <param name="dataGrid"></param>
        private void SetDocumentIndexFromQueryString(DataGrid dataGrid)
        {
            if (!this.IsPostBack)
            {
                string param = this.Request.QueryString["docIndex"];

                if (param != null && param != string.Empty)
                {
                    int documentIndex = -1;
                    try
                    {
                        documentIndex = Int32.Parse(param);
                    }
                    catch
                    {
                    }

                    dataGrid.SelectedIndex = documentIndex;
                }
            }
        }

        /// <summary>
        /// Aggiornamento contesto chiamante
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="docIndex"></param>
        private void RefreshCallerContext(int currentPage, int docIndex)
        {
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

            if (currentContext != null && currentContext.ContextName == SiteNavigation.NavigationKeys.FASCICOLO)
            {
                currentContext.QueryStringParameters["tab"] = "documenti";
                currentContext.QueryStringParameters["back"] = "Y";
                this.SetDocumentIndexOnContext(docIndex);

                currentContext.PageNumber = currentPage;

                // Nello stato di sessione del contesto viene memorizzato il fascicolo correntemente selezionato
                currentContext.SessionState["FascicoloSelezionato"] = FascicoliManager.getFascicoloSelezionato();
            }
        }

        #endregion

        protected void dt_Prot_PreRender(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dt_Prot.Items.Count; i++)
            {
                //if (((Label)this.dt_Prot.Items[i].Cells[18].Controls[1]).Text == "1")
                //{
                //    ((System.Web.UI.WebControls.ImageButton)this.dt_Prot.Items[i].Cells[17].Controls[1]).ToolTip = "Elimina questo documento da 'Area di conservazione'";
                //    ((System.Web.UI.WebControls.ImageButton)this.dt_Prot.Items[i].Cells[17].Controls[1]).ImageUrl = "../images/proto/cancella.gif";
                //    ((System.Web.UI.WebControls.ImageButton)this.dt_Prot.Items[i].Cells[17].Controls[1]).CommandName = "EliminaAreaCons";
                //}
                //else
                //{
                //    // se il documento appartiene a un fascicolo 
                //    if (((Label)this.dt_Prot.Items[i].Cells[18].Controls[1]).Text == "2")
                //    {
                //        ((System.Web.UI.WebControls.ImageButton)this.dt_Prot.Items[i].Cells[17].Controls[1]).ToolTip = "Elimina questo documento da 'Area di conservazione'";
                //        ((System.Web.UI.WebControls.ImageButton)this.dt_Prot.Items[i].Cells[17].Controls[1]).ImageUrl = "../images/proto/cancella.gif";
                //        ((System.Web.UI.WebControls.ImageButton)this.dt_Prot.Items[i].Cells[17].Controls[1]).CommandName = "DocInFascicolo";
                //    }
                //}
                dt_Prot.Items[i].Cells[0].Font.Bold = true;

                //11 è la colonna che utilizza il metodo getnumprot con il quale
                //si estrae se è un protocollo RED oppure grigio o pred. BLACK

                int index = this.dt_Prot.SelectedIndex;
                Label lbl = (Label)dt_Prot.Items[i].Cells[10].Controls[1];

                if (lbl.Text == "")
                {
                    if (i != index)
                    {
                        ((LinkButton)dt_Prot.Items[i].Cells[0].Controls[1]).ForeColor = Color.Black;
                    }
                    else
                    {
                        ((LinkButton)dt_Prot.Items[i].Cells[0].Controls[1]).ForeColor = Color.White;
                    }
                }
                else
                    ((LinkButton)dt_Prot.Items[i].Cells[0].Controls[1]).ForeColor = Color.Red;
            }
        }

        //PRENDI ETICHETTE PROTOCOLLI
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

        protected void dt_Prot_SelectedIndexChanged(object sender, EventArgs e)
        {
            string newUrl = "";

            FascicoliManager.SetProtoDocsGridPaging(this, this.dt_Prot.CurrentPageIndex);
            FascicoliManager.SetFolderViewTracing(this, true);

            /* Debug */
            this.SendDebugAlert("Prot=" + this.dt_Prot.CurrentPageIndex);

            string key = ((Label)this.dt_Prot.Items[this.dt_Prot.SelectedIndex].Cells[6].Controls[1]).Text;

            string tipoP = ((Label)this.dt_Prot.Items[this.dt_Prot.SelectedIndex].Cells[4].Controls[1]).Text;
            if (!tipoP.Equals("NP"))
            {
                newUrl = "../documento/gestioneDoc.aspx?tab=protocollo";
            }
            else
            {
                newUrl = "../documento/gestioneDoc.aspx?tab=profilo";
            }

            int idDocumento = Int32.Parse(key);

            // Aggiornamento contesto chiamante
            this.RefreshCallerContext(this.dt_Prot.CurrentPageIndex, idDocumento);

            dettaglioDocumento(idDocumento, newUrl);

            FascicoliManager.setMemoriaFascicoloSelezionato(this, FascicoliManager.getFascicoloSelezionato());
            FascicoliManager.setMemoriaFolderSelezionata(this, FascicoliManager.getFolderSelezionato(this));

            FascicoliManager.removeFascicoloSelezionato(this);
            FascicoliManager.removeFolderSelezionato(this);

            //Inserisco in sessione l'oggetto per lo scorrimento continuo della lista
            ArrayList listaDoc = new ArrayList(FascicoliManager.getHashDocProtENonProt(this).Values);
            listaDoc.Reverse();
            //UserControls.ScrollElementsList.ScrollManager.setInSessionNewObjScrollElementsList(dt_Prot.VirtualItemCount, dt_Prot.PageCount, dt_Prot.PageSize, idDocumento, dt_Prot.CurrentPageIndex, listaDoc, UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_DOC_IN_FASC);
            UserControls.ScrollElementsList.ScrollManager.setInContextNewObjScrollElementsList(dt_Prot.VirtualItemCount, dt_Prot.PageCount, dt_Prot.PageSize, idDocumento, dt_Prot.CurrentPageIndex, listaDoc, UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_DOC_IN_FASC);
        }

        protected void dt_Prot_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            //if (UserManager.ruoloIsAutorized(this, "DO_DEL_DOC_FASC"))
            //{
            //   dt_Prot.Columns[10].Visible = true;
            //}
            //else
            //{
            //   dt_Prot.Columns[10].Visible = false;
            //}
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {


                e.Item.Cells[0].Font.Bold = true;


                //11 è la colonna che utilizza il metodo getnumprot con il quale
                //si estrae se è un protocollo RED oppure grigio o pred. BLACK
                //Label lbl=(Label)e.Item.Cells[10].Controls[1];

                //if (lbl.Text=="")
                //    e.Item.Cells[0].ForeColor=Color.Black;
                //else
                //    e.Item.Cells[0].ForeColor=Color.Red;

                string dataAnnull = ((TableCell)e.Item.Cells[9]).Text;

                try
                {
                    DateTime dt = Convert.ToDateTime(dataAnnull);
                    e.Item.ForeColor = Color.Red;
                    e.Item.Font.Bold = true;
                    e.Item.Font.Strikeout = true;
                }
                catch { }


                // gestione della icona dei dettagli -- tolgo icona se non c'è doc acquisito
                //Label lblImg = e.Item.Cells[16].FindControl("lbl_chaImg") as Label;
                //ImageButton imgbtn = new ImageButton();

                //if (lblImg != null && lblImg.Text == "0")
                //{
                //    if (e.Item.Cells[13].Controls[1].GetType().Equals(typeof(ImageButton)))
                //    {
                //        imgbtn = (ImageButton)e.Item.Cells[13].Controls[1];
                //        imgbtn.Visible = false;
                //    }
                //}
                //else
                //{
                //    if (lblImg != null)
                //    {
                //        string image = FileManager.getFileIcon(Page, lblImg.Text.Trim().ToLower());

                //        if (e.Item.Cells[13].Controls[1].GetType().Equals(typeof(ImageButton)))
                //        {
                //            imgbtn = (ImageButton)e.Item.Cells[13].Controls[1];
                //            imgbtn.ImageUrl = image;
                //        }
                //    }
                //}
            }
        }

        protected void dt_Prot_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            this.dt_Prot.SelectedIndex = -1;

            this.dt_Prot.CurrentPageIndex = e.NewPageIndex;

            int pageNumber = e.NewPageIndex + 1;

            this.FillData(pageNumber);
        }


    }
}

