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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using DocsPAWA.DocsPaWR;


namespace DocsPAWA.popup
{
    /// <summary>
    ///
    /// </summary>
    public class RicercaDocNonProtocollati : DocsPAWA.CssPage
    {
        protected System.Web.UI.WebControls.Label lbl_message;
        protected System.Web.UI.WebControls.DataGrid DataGrid1;
        protected System.Web.UI.WebControls.Label lblIdDocumento;
        protected System.Web.UI.WebControls.Label lblDataCrezione;
        protected System.Web.UI.WebControls.Label lblEndDataCreazione;
        protected DocsPAWA.UserControls.Calendar txtInitDtaCreazione;
        protected System.Web.UI.HtmlControls.HtmlTable tblNumeroProtocollo;
        //protected DocsPaWebCtrlLibrary.DateMask txtEndDataProtocollo;
        protected DocsPAWA.UserControls.Calendar txtEndDataCreazione;
        protected System.Web.UI.WebControls.Label lblInitDocumento;
        protected System.Web.UI.WebControls.TextBox txtInitDocumento;
        protected System.Web.UI.WebControls.Label lblEndDocumento;
        protected System.Web.UI.WebControls.TextBox txtEndDocumento;
        protected System.Web.UI.WebControls.DropDownList ddl_dtaCreazione;
        protected System.Web.UI.WebControls.Label lblInitDtaCreazione;
        //protected DocsPaWebCtrlLibrary.DateMask txtInitDtaProto;

        protected System.Web.UI.WebControls.DropDownList ddl_numDoc;

        protected System.Web.UI.WebControls.Button btn_chiudi;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        protected static int currentPage;
        protected DocsPAWA.DocsPaWR.InfoDocumento[] infoDoc;
        protected ArrayList Dg_elem;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] ListaFiltri;
        protected int numTotPage;
        protected System.Web.UI.WebControls.CheckBox chk_ADL;
        protected System.Web.UI.WebControls.Label lbl_countRecord;
        protected System.Web.UI.WebControls.DataGrid dg_lista_corr;
        protected System.Web.UI.WebControls.ImageButton btn_chiudi_risultato;

        protected System.Web.UI.WebControls.ImageButton btn_clearFlag;
        protected int nRec;
        protected DataSet ds;
        protected System.Web.UI.WebControls.Button btn_ok;
        protected System.Web.UI.WebControls.Button btn_find;

        protected DataTable dt;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;
        protected Hashtable ht_destinatariTO_CC;
        protected string str_indexSel;

        protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDocInLavorazione;
        protected DocsPAWA.DocsPaWR.InfoDocumento infoDocSel = null;
        protected DocsPAWA.UserControls.AppTitleProvider appTitleProvider;
        protected string tipoProto;


        private void Page_Load(object sender, System.EventArgs e)
        {
            this.Response.Expires = -1;

            DocsPAWA.Utils.DefaultButton(this, ref this.GetCalendarControl("txtInitDtaCreazione").txt_Data, ref btn_find);
            DocsPAWA.Utils.DefaultButton(this, ref this.GetCalendarControl("txtEndDataCreazione").txt_Data, ref btn_find);


            if (!IsPostBack)
            {
                RicercaNonProtocollatiSessionMng.SetAsLoaded(this);
            }
            else
            {
                // gestione del valore di ritorno della modal Dialog 
                if (this.hd_returnValueModal.Value != null && this.hd_returnValueModal.Value != string.Empty && this.hd_returnValueModal.Value != "undefined")
                {

                    string retValue = this.GestioneAvvisoModale(hd_returnValueModal.Value);

                    if (retValue != "C")
                    {
                        //rimuovo le cose appoggiate in sessione
                        RicercaNonProtocollatiSessionMng.ClearSessionData(this);
                    }
                }
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
            this.chk_ADL.CheckedChanged += new System.EventHandler(this.chk_ADL_CheckedChanged);
            this.ddl_numDoc.SelectedIndexChanged += new System.EventHandler(this.ddl_numDoc_SelectedIndexChanged);
            this.ddl_dtaCreazione.SelectedIndexChanged += new System.EventHandler(this.ddl_dtaProto_SelectedIndexChanged);

            this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChange);
            this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        private void ddl_numDoc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.txtEndDocumento.Text = "";

            if (this.ddl_numDoc.SelectedIndex == 0)
            {
                this.txtEndDocumento.Visible = false;
                this.lblEndDocumento.Visible = false;
                this.lblInitDocumento.Visible = false;
            }
            else
            {
                this.txtEndDocumento.Visible = true;
                this.lblEndDocumento.Visible = true;
                this.lblInitDocumento.Visible = true;
            }
        }


        #region METODI VALIDAZIONE DATI IN INPUT
        /// <summary>
        /// Validazione valore numerico
        /// </summary>
        /// <param name="numberText"></param>
        /// <returns></returns>
        private bool IsValidNumber(TextBox numberText)
        {
            bool retValue = true;

            try
            {
                int.Parse(numberText.Text);
            }
            catch
            {
                retValue = false;
            }

            return retValue;
        }

        public bool IsValidYear(string strYear)
        {
            Regex onlyNumberPattern = new Regex(@"\d{4}");
            return onlyNumberPattern.IsMatch(strYear);
        }
        #endregion

        protected bool RicercaDocumenti()
        {
            try
            {

                //array contenitore degli array filtro di ricerca
                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                #region filtro Grigi
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
                fV1.valore = "true";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                //solo doc grigi
                //fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                //fV1.valore = "true";
                //fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                #region Filtro per REGISTRO
                if (!UserManager.isFiltroAooEnabled(this))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRI_UTENTE_IN_CONDITION_CON_NULL.ToString();
                    fV1.valore = (String)Session["inRegCondition"];
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion


                #region filtro DOCNUMBER
                if (this.ddl_numDoc.SelectedIndex == 0)
                {

                    if (this.txtInitDocumento.Text != null && !this.txtInitDocumento.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                        fV1.valore = this.txtInitDocumento.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {
                    if (!this.txtInitDocumento.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                        fV1.valore = this.txtInitDocumento.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txtEndDocumento.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                        fV1.valore = this.txtEndDocumento.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion


                #region filtro DATA CREAZIONE
                if (this.ddl_dtaCreazione.SelectedIndex == 0)
                {//valore singolo specificato per DATA_creazione
                    if (!this.GetCalendarControl("txtInitDtaCreazione").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txtInitDtaCreazione").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaCreazione").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txtInitDtaCreazione").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {
                    if (!this.GetCalendarControl("txtInitDtaCreazione").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txtInitDtaCreazione").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaCreazione").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txtInitDtaCreazione").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtEndDataCreazione").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion



                qV[0] = fVList;
                return true;

            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
            }
        }

        private void LoadData(bool updateGrid)
        {
            SearchResultInfo[] idProfileList;
            DocsPaWR.InfoUtente infoUt = new DocsPAWA.DocsPaWR.InfoUtente();
            infoUt = UserManager.getInfoUtente(this);
            ListaFiltri = DocumentManager.getFiltroRicDoc(this);
            if (!this.chk_ADL.Checked == true)
            {
                infoDoc = DocumentManager.getQueryInfoDocumentoPaging(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, currentPage, out numTotPage, out nRec, true, false, true, false, out idProfileList);

                //Controllo inserito per non far visualizzare se stesso
                DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumentoLav = DocumentManager.getDocumentoInLavorazione();
                if (schedaDocumentoLav != null)
                {
                    List<DocsPaWR.InfoDocumento> tempDoc = new List<DocsPaWR.InfoDocumento>();
                    tempDoc = infoDoc.ToList<DocsPaWR.InfoDocumento>();
                    foreach (DocsPaWR.InfoDocumento infDocTemp in infoDoc)
                    {
                        if (infDocTemp.docNumber == schedaDocumentoLav.docNumber)
                        {
                            tempDoc.Remove(infDocTemp);
                            break;
                        }
                    }
                    infoDoc = tempDoc.ToArray();
                }
                //FINE CONTROLLO INSERIMENTO PER NON FAR VISUALIZZARE SE STESSO
            }
            else
            {
                DocsPaWR.AreaLavoro areaLavoro = DocumentManager.getListaAreaLavoro(this, "G", "1", currentPage, out numTotPage, out nRec, UserManager.getRegistroSelezionato(this).systemId);

                infoDoc = new DocsPAWA.DocsPaWR.InfoDocumento[areaLavoro.lista.Length];

                for (int i = 0; i < areaLavoro.lista.Length; i++)
                    infoDoc[i] = (DocsPAWA.DocsPaWR.InfoDocumento)areaLavoro.lista[i];
            }


            this.lbl_countRecord.Visible = true;
            this.lbl_countRecord.Text = "Documenti tot:  " + nRec;

            this.DataGrid1.VirtualItemCount = nRec;
            this.DataGrid1.CurrentPageIndex = currentPage - 1;

            //appoggio il risultato in sessione
            //Session["RicercaProtocolliUscita.ListaInfoDoc"] =infoDoc;
            RicercaNonProtocollatiSessionMng.SetListaInfoDocumenti(this, infoDoc);

            if (infoDoc != null && infoDoc.Length > 0)
            {
                this.BindGrid(infoDoc);
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
                int numDoc = new Int32();
                string tipoProto = string.Empty;
                for (int i = 0; i < infos.Length; i++)
                {
                    currentDoc = ((DocsPAWA.DocsPaWR.InfoDocumento)infos[i]);

                    string dataApertura = "";
                    if (currentDoc.dataApertura != null && currentDoc.dataApertura.Length > 0)
                        dataApertura = currentDoc.dataApertura.Substring(0, 10);

                    if (currentDoc.numProt != null && !currentDoc.numProt.Equals(""))
                    {
                        numDoc = Int32.Parse(currentDoc.numProt);
                        descrDoc = numDoc.ToString();
                    }
                    else
                    {
                        descrDoc = currentDoc.docNumber;
                    }

                    if (currentDoc.tipoProto != string.Empty)
                    {
                        tipoProto = currentDoc.tipoProto;
                        if (tipoProto.Equals("G"))
                            tipoProto = "NP";
                    }
                    descrDoc = descrDoc + "\n" + dataApertura;
                    string MittDest = "";

                    if (currentDoc.mittDest != null && currentDoc.mittDest.Length > 0)
                    {
                        for (int g = 0; g < currentDoc.mittDest.Length; g++)
                            MittDest += currentDoc.mittDest[g] + " - ";
                        if (currentDoc.mittDest.Length > 0)
                            MittDest = MittDest.Substring(0, MittDest.Length - 3);
                    }

                    Dg_elem.Add(new ProtocolloUscitaDataGridItem(descrDoc, currentDoc.oggetto, MittDest, currentDoc.codRegistro, i, tipoProto, numDoc));
                }

                this.DataGrid1.SelectedIndex = -1;
                this.DataGrid1.DataSource = Dg_elem;
                this.DataGrid1.DataBind();
                this.DataGrid1.Visible = true;
            }
            else
            {
                this.DataGrid1.Visible = false;
                this.lbl_message.Visible = true;
            }
        }

        public class ProtocolloUscitaDataGridItem
        {
            private string data;
            private string oggetto;
            private string mittDest;
            private string tipoProto;
            private string registro;
            private int chiave;
            private int idDoc;

            public ProtocolloUscitaDataGridItem(string data, string oggetto, string mittDest, string registro, int chiave, string tipoProto, int idDoc)
            {
                this.data = data;
                this.oggetto = oggetto;
                this.registro = registro;
                this.mittDest = mittDest;
                this.tipoProto = tipoProto;
                this.chiave = chiave;
                this.idDoc = idDoc;
            }

            public string Data { get { return data; } }
            public string Oggetto { get { return oggetto; } }
            public string Registro { get { return registro; } }
            public string MittDest { get { return mittDest; } }
            public int Chiave { get { return chiave; } }
            public string TipoProto { get { return tipoProto; } }
            public int IdDoc { get { return idDoc; } }

        }

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }

        private void ddl_dtaProto_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text = "";
            if (this.ddl_dtaCreazione.SelectedIndex == 0)
            {
                this.GetCalendarControl("txtEndDataProtocollo").Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").btn_Cal.Visible = false;
                this.lblEndDataCreazione.Visible = false;
                this.lblInitDtaCreazione.Visible = false;

            }
            else
            {
                this.GetCalendarControl("txtEndDataCreazione").Visible = true;
                this.GetCalendarControl("txtEndDataCreazione").btn_Cal.Visible = true;
                this.GetCalendarControl("txtEndDataCreazione").txt_Data.Visible = true;
                this.lblEndDataCreazione.Visible = true;
                this.lblInitDtaCreazione.Visible = true;
            }
        }

        private void DataGrid1_PageIndexChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            currentPage = e.NewPageIndex + 1;

            // Cricamento del DataGrid
            this.LoadData(true);
        }

        private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            DocsPaWR.Corrispondente[] listaCorrTo = null;
            DocsPaWR.Corrispondente[] listaCorrCC = null;

            if (this.DataGrid1.SelectedIndex >= 0)
            {
                str_indexSel = ((Label)this.DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[5].Controls[1]).Text;
                int indexSel = Int32.Parse(str_indexSel);
                //this.infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento[]) Session["RicercaProtocolliUscita.ListaInfoDoc"];
                this.infoDoc = RicercaNonProtocollatiSessionMng.GetListaInfoDocumenti(this);

                if (indexSel > -1)
                    infoDocSel = (DocsPAWA.DocsPaWR.InfoDocumento)this.infoDoc[indexSel];

                if (infoDocSel != null)
                {
                    //prendo il dettaglio del documento e estraggo i destinatari del protocollo
                    DocsPaWR.SchedaDocumento schedaDocUscita = new DocsPAWA.DocsPaWR.SchedaDocumento();
                    schedaDocUscita = DocumentManager.getDettaglioDocumento(this, infoDocSel.idProfile, infoDocSel.docNumber);
                    //prendo i destinatari in To
                    listaCorrTo = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari;
                    //prendo i destinatari in CC
                    listaCorrCC = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatariConoscenza;

                    FillDataGrid(listaCorrTo, listaCorrCC);
                    DocumentManager.setInfoDocumento(this, infoDocSel);
                }

            }
        }

        private void AppendDataRow(DataTable dt, string tipoCorr, string systemId, string descCorr)
        {
            DataRow row = dt.NewRow();
            row["SYSTEM_ID"] = systemId;
            row["TIPO_CORR"] = tipoCorr;
            row["DESC_CORR"] = descCorr;
            dt.Rows.Add(row);
            row = null;
        }

        /// <summary>
        /// Caricamento griglia destinatari del protocollo in uscita selezionato
        /// </summary>
        /// <param name="uoApp"></param>
        private void FillDataGrid(DocsPAWA.DocsPaWR.Corrispondente[] listaCorrTo, DocsPAWA.DocsPaWR.Corrispondente[] listaCorrCC)
        {
            //ds=this.CreateGridDataSetDestinatari();
            //this.CaricaGridDataSetDestinatari(ds, listaCorrTo, listaCorrCC);
            //this.dg_lista_corr.DataSource=ds;
            //DocumentManager.setDataGridProtocolliUscita(this,dt);
            //this.dg_lista_corr.DataBind();

            //// Impostazione corrispondente predefinito
            //this.SelectDefaultCorrispondente();
        }

        private DataSet CreateGridDataSetDestinatari()
        {
            DataSet retValue = new DataSet();

            dt = new DataTable("GRID_TABLE_DESTINATARI");
            dt.Columns.Add("SYSTEM_ID", typeof(string));
            dt.Columns.Add("TIPO_CORR", typeof(string));
            dt.Columns.Add("DESC_CORR", typeof(string));
            retValue.Tables.Add(dt);

            return retValue;
        }

        /// <summary>
        /// In presenza di un solo corrispondente in griglia,
        /// lo seleziona per default
        /// </summary>
        private void SelectDefaultCorrispondente()
        {
            if (this.dg_lista_corr.Items.Count == 1)
            {
                DataGridItem dgItem = this.dg_lista_corr.Items[0];

                RadioButton optCorr = dgItem.Cells[3].FindControl("optCorr") as RadioButton;
                if (optCorr != null)
                    optCorr.Checked = true;
            }
        }





        private void btn_ok_Click(object sender, System.EventArgs e)
        {

            int itemIndex;
            bool oggettoOK = false;
            string oggettoDocSel = "";
            bool avanzaDoc = verificaSelezioneDocumento(out itemIndex);
            if (avanzaDoc)
            {

                this.infoDoc = RicercaNonProtocollatiSessionMng.GetListaInfoDocumenti(this);

                if (itemIndex > -1)
                {
                    infoDocSel = (DocsPAWA.DocsPaWR.InfoDocumento)this.infoDoc[itemIndex];
                    oggettoDocSel = ((Label)DataGrid1.Items[itemIndex].Cells[3].Controls[1]).Text;
                    DocumentManager.setInfoDocumento(this, infoDocSel);
                }

                schedaDocInLavorazione = DocumentManager.getDocumentoInLavorazione(this);
                if (schedaDocInLavorazione != null)
                {
                    //if (schedaDocInLavorazione.docNumber != null)
                    //{
                    //CASO DOC GRIGIO CREATO

                    //inizio verifica congruenza campo oggetto
                    if (schedaDocInLavorazione.oggetto != null && schedaDocInLavorazione.oggetto.descrizione != null && schedaDocInLavorazione.oggetto.descrizione != String.Empty)
                    {
                        if (oggettoDocSel != null && oggettoDocSel != String.Empty)
                        {
                            if (schedaDocInLavorazione.oggetto.descrizione.ToUpper().Equals(oggettoDocSel.ToUpper()))
                            {
                                oggettoOK = true;
                            }
                        }
                    }
                    else
                    {

                        oggettoOK = true;
                        if (oggettoDocSel != null && oggettoDocSel != String.Empty)
                        {
                            DocsPaWR.Oggetto ogg = new DocsPAWA.DocsPaWR.Oggetto();
                            ogg.descrizione = oggettoDocSel.ToString();
                            schedaDocInLavorazione.oggetto = ogg;
                        }
                    }

                    if (!oggettoOK)
                    {
                        //se oggetto non coincide si lancia un avviso	all'utente 
                        if (!this.Page.IsStartupScriptRegistered("avvisoModale"))
                        {
                            string scriptString = "";
                            if (schedaDocInLavorazione.docNumber != null)
                            {
                                scriptString = "<SCRIPT>OpenAvvisoModale( '" + oggettoOK + "' , 'True');</SCRIPT>";
                            }
                            else
                            {
                                scriptString = "<SCRIPT>OpenAvvisoModale( '" + oggettoOK + "' , 'False');</SCRIPT>";
                            }
                            this.Page.RegisterStartupScript("avvisoModale", scriptString);
                        }
                    }
                    else
                    {

                        schedaDocInLavorazione.rispostaDocumento = infoDocSel;
                        schedaDocInLavorazione.modificaRispostaDocumento = true;

                        DocumentManager.setDocumentoSelezionato(this, schedaDocInLavorazione);
                        DocumentManager.setDocumentoInLavorazione(this, schedaDocInLavorazione);

                        RicercaNonProtocollatiSessionMng.SetDialogReturnValue(this, true);

                        Page.RegisterStartupScript("", "<script>window.close();</script>");

                    }

                }
                //}

            }
        }

        private string GestioneAvvisoModale(string valore)
        {
            string retValue = string.Empty;

            try
            {
                //prendo il documento in sessione
                schedaDocInLavorazione = DocumentManager.getDocumentoInLavorazione(this);

                //prendo il protocollo in uscita selezionato dal datagrid
                infoDocSel = DocumentManager.getInfoDocumento(this);

                retValue = valore;

                switch (valore)
                {
                    case "Y": //Gestione pulsante SI, CONTINUA

                        /* Alla pressione del pulsante CONTINUA l'utente vuole proseguire il 
                         * collegamento nonostante oggetto sia diverso */

                        if (schedaDocInLavorazione != null)
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocInLavorazione.rispostaDocumento = infoDocSel;
                                schedaDocInLavorazione.modificaRispostaDocumento = true;
                            }

                            this.hd_returnValueModal.Value = "";

                            //if (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente == null)
                            //{
                            //   if (RicercaProtocolliUscitaSessionMng.getCorrispondenteRisposta(this) != null)
                            //   {
                            //      ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = RicercaProtocolliUscitaSessionMng.getCorrispondenteRisposta(this);
                            //   }
                            //}
                            DocumentManager.setDocumentoSelezionato(this, schedaDocInLavorazione);
                            DocumentManager.setDocumentoInLavorazione(this, schedaDocInLavorazione);
                            //metto in sessione la scheda documento con le informazioni del protocollo a cui risponde

                            Page.RegisterStartupScript("", "<script>window.close();</script>");
                        }

                        break;

                    case "N":

                        /* Alla pressione del pulsante NO, RESETTA l'utente vuole proseguire il collegamento 
                         * con i dati che ha digitato sulla pagina di protocollo */


                        if (schedaDocInLavorazione != null)
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocInLavorazione.rispostaDocumento = infoDocSel;
                                schedaDocInLavorazione.modificaRispostaDocumento = true;
                            }

                            if (schedaDocInLavorazione.oggetto != null)
                            {
                                schedaDocInLavorazione.oggetto.descrizione = infoDocSel.oggetto.ToString();
                            }
                            else
                            {
                                DocsPaWR.Oggetto ogg = new DocsPAWA.DocsPaWR.Oggetto();
                                ogg.descrizione = infoDocSel.oggetto.ToString();
                                schedaDocInLavorazione.oggetto = ogg;
                            }

                            //if (RicercaProtocolliUscitaSessionMng.getCorrispondenteRisposta(this) != null)
                            //{
                            //   ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = RicercaProtocolliUscitaSessionMng.getCorrispondenteRisposta(this);
                            //}

                            //	popolo il campo mittente con il destinatario selezionato dal protocollo a cui ri risponde 
                            //((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = destSelected;

                            //metto in sessione la scheda documento con le informazioni del protocollo a cui risponde
                            DocumentManager.setDocumentoSelezionato(this, schedaDocInLavorazione);
                            DocumentManager.setDocumentoInLavorazione(this, schedaDocInLavorazione);
                            Page.RegisterStartupScript("", "<script>window.close();</script>");

                            this.hd_returnValueModal.Value = "";
                        }

                        break;

                    case "S":
                        //non posso modificare il mittente o oggetto, quindi il pulsante continua
                        //si limiterà a popolare il campo risposta al protocollo con l'infoDoc corrente
                        if (schedaDocInLavorazione != null)
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocInLavorazione.rispostaDocumento = infoDocSel;
                                schedaDocInLavorazione.modificaRispostaDocumento = true;
                            }
                        }
                        DocumentManager.setDocumentoSelezionato(this, schedaDocInLavorazione);
                        DocumentManager.setDocumentoInLavorazione(this, schedaDocInLavorazione);
                        Page.RegisterStartupScript("", "<script>window.close();</script>");

                        break;
                }
            }
            catch
            {

            }

            return retValue;
        }

        private string GetImage(string rowType)
        {
            string retValue = string.Empty;
            string spaceIndent = string.Empty;

            switch (rowType)
            {
                case "U":
                    retValue = "uo_noexp";
                    spaceIndent = "&nbsp;";
                    break;

                case "R":
                    retValue = "ruolo_noexp";
                    spaceIndent = "&nbsp;";
                    break;

                case "P":
                    retValue = "utente_noexp";
                    spaceIndent = "&nbsp;";
                    break;
            }

            retValue = spaceIndent + "<img src='../images/smistamento/" + retValue + ".gif' border='0'>";

            return retValue;
        }

        #region METODI DI VERIFICA

        //VERIFICA SE è STATO SELEZIONATO ALMENO UNA OPZIONE (CORRISPONDENTE) NEL PANNELLO pnl_corr
        private bool verificaSelezione(out int itemIndex)
        {
            bool verificaSelezione = false;
            itemIndex = -1;
            foreach (DataGridItem dgItem in this.dg_lista_corr.Items)
            {
                RadioButton optCorr = dgItem.Cells[3].FindControl("optCorr") as RadioButton;
                if ((optCorr != null) && optCorr.Checked == true)
                {
                    itemIndex = dgItem.ItemIndex;
                    verificaSelezione = true;
                    break;
                }
            }
            return verificaSelezione;
        }

        //VERIFICA SE è STATO SELEZIONATO un documento
        private bool verificaSelezioneDocumento(out int itemIndex)
        {
            bool verificaSelezione = false;
            itemIndex = -1;
            foreach (DataGridItem dgItem in this.DataGrid1.Items)
            {
                RadioButton optCorr = dgItem.Cells[5].FindControl("optCorr") as RadioButton;
                if ((optCorr != null) && optCorr.Checked == true)
                {
                    itemIndex = dgItem.ItemIndex;
                    verificaSelezione = true;
                    break;
                }
            }
            return verificaSelezione;
        }
        //VERIFICA se il mittente del protocollo in ingresso coincide con il destinatario selezionato
        //del protocollo in uscita a cui si sta rispondendo
        private bool verificaUguaglianzacorrispondenti(DocsPAWA.DocsPaWR.Corrispondente destSelected, DocsPAWA.DocsPaWR.Corrispondente mittCorrente)
        {
            bool verificaUguaglianza = false;
            if (destSelected.systemId == mittCorrente.systemId)
            {
                verificaUguaglianza = true;
            }
            return verificaUguaglianza;
        }

        private void btn_chiudi_Click(object sender, System.EventArgs e)
        {
            Response.Write("<SCRIPT>window.close();</SCRIPT>");
        }

        private void chk_ADL_CheckedChanged(object sender, System.EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked)
            {
                //se il checkBox è spuntato disablito i filtri di ricerca
                this.ddl_numDoc.SelectedIndex = 0;
                this.ddl_dtaCreazione.SelectedIndex = 0;
                this.ddl_numDoc.Enabled = false;
                this.ddl_dtaCreazione.Enabled = false;

                this.lblInitDtaCreazione.Visible = false;
                this.lblInitDocumento.Visible = false;
                this.lblEndDataCreazione.Visible = false;
                this.lblEndDocumento.Visible = false;

                this.GetCalendarControl("txtInitDtaCreazione").txt_Data.Enabled = false;
                this.GetCalendarControl("txtInitDtaCreazione").btn_Cal.Visible = false;
                this.GetCalendarControl("txtInitDtaCreazione").txt_Data.Text = String.Empty;

                this.GetCalendarControl("txtEndDataCreazione").Visible = false;
                this.GetCalendarControl("txtEndDataCreazione").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataCreazione").btn_Cal.Visible = false;
                this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text = String.Empty;

                this.txtInitDocumento.Enabled = false;
                this.txtInitDocumento.Text = String.Empty;

                this.txtEndDocumento.Text = String.Empty;
                this.txtEndDocumento.Visible = false;
            }
            else
            {
                //se il checkBox non è spuntato

                //riabilito i filtri di ricerca
                this.ddl_numDoc.Enabled = true;
                this.ddl_dtaCreazione.Enabled = true;

                this.GetCalendarControl("txtInitDtaCreazione").txt_Data.Enabled = true;
                this.GetCalendarControl("txtInitDtaCreazione").btn_Cal.Visible = true;
                this.GetCalendarControl("txtInitDtaCreazione").txt_Data.Text = String.Empty;
                this.txtInitDocumento.Enabled = true;
                string s = "<SCRIPT language='javascript'>try{document.getElementById('" + txtInitDocumento.ID + "').focus();} catch(e){}</SCRIPT>";
                RegisterStartupScript("focus", s);

            }
        }

        private void resetOption()
        {
            foreach (DataGridItem dgItem in this.dg_lista_corr.Items)
            {
                RadioButton optCorr = dgItem.Cells[3].FindControl("optCorr") as RadioButton;
                optCorr.Checked = false;
            }
        }




        #region classe per la gestione della sessione
        /// <summary>
        /// Classe per la gestione dei dati in sessione relativamente
        /// alla dialog "RicercaDocumentiNonProtocollati"
        /// </summary>
        public sealed class RicercaNonProtocollatiSessionMng
        {
            private RicercaNonProtocollatiSessionMng()
            {
            }

            /// <summary>
            /// Gestione rimozione dati in sessione
            /// </summary>
            /// <param name="page"></param>
            public static void ClearSessionData(Page page)
            {
                DocumentManager.removeFiltroRicDoc(page);
                DocumentManager.removeDataGridProtocolliUscita(page);

                DocumentManager.removeInfoDocumento(page);
                DocumentManager.removeHash(page);

                RemoveListaInfoDocumenti(page);
                page.Session.Remove("RicercaNonProtocollatiSessionMng.dialogReturnValue");
                removeCorrispondenteRisposta(page);

            }

            public static void SetListaInfoDocumenti(Page page, DocsPaWR.InfoDocumento[] listaDocumenti)
            {
                page.Session["RicercaNonProtocollati.ListaInfoDoc"] = listaDocumenti;
            }

            public static DocsPAWA.DocsPaWR.InfoDocumento[] GetListaInfoDocumenti(Page page)
            {
                return page.Session["RicercaNonProtocollati.ListaInfoDoc"] as DocsPAWA.DocsPaWR.InfoDocumento[];
            }

            public static void RemoveListaInfoDocumenti(Page page)
            {
                page.Session.Remove("RicercaNonProtocollati.ListaInfoDoc");
            }

            /// <summary>
            /// Impostazione flag booleano, se true, la dialog è stata caricata almeno una volta
            /// </summary>
            /// <param name="page"></param>
            public static void SetAsLoaded(Page page)
            {
                page.Session["RicercaNonProtocollatiSessionMng.isLoaded"] = true;
            }

            /// <summary>
            /// Impostazione flag relativo al caricamento della dialog
            /// </summary>
            /// <param name="page"></param>
            public static void SetAsNotLoaded(Page page)
            {
                page.Session.Remove("RicercaNonProtocollatiSessionMng.isLoaded");
            }

            /// <summary>
            /// Verifica se la dialog è stata caricata almeno una volta
            /// </summary>
            /// <param name="page"></param>
            /// <returns></returns>
            public static bool IsLoaded(Page page)
            {
                return (page.Session["RicercaNonProtocollatiSessionMng.isLoaded"] != null);
            }

            /// <summary>
            /// Impostazione valore di ritorno
            /// </summary>
            /// <param name="page"></param>
            /// <param name="dialogReturnValue"></param>
            public static void SetDialogReturnValue(Page page, bool dialogReturnValue)
            {
                page.Session["RicercaNonProtocollatiSessionMng.dialogReturnValue"] = dialogReturnValue;
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
                    retValue = Convert.ToBoolean(page.Session["RicercaNonProtocollatiSessionMng.dialogReturnValue"]);

                page.Session.Remove("RicercaNonProtocollatiSessionMng.isLoaded");

                return retValue;
            }

            /// <summary>
            /// Metodo per il settaggio in sessione del corrispondente selezionato per il protocollo di risposta
            /// </summary>
            /// <param name="page"></param>
            /// <param name="corrispondente"></param>
            public static void setCorrispondenteRisposta(Page page, DocsPAWA.DocsPaWR.Corrispondente corrispondente)
            {
                page.Session["RicercaNonProtocollati.corrispondenteRisposta"] = corrispondente;
            }

            public static DocsPAWA.DocsPaWR.Corrispondente getCorrispondenteRisposta(Page page)
            {
                return (DocsPAWA.DocsPaWR.Corrispondente)page.Session["RicercaPNonProtocollati.corrispondenteRisposta"];
            }

            public static void removeCorrispondenteRisposta(Page page)
            {
                page.Session.Remove("RicercaNonProtocollati.corrispondenteRisposta");
            }
        #endregion
        }

        protected void btn_find_Click(object sender, EventArgs e)
        {
            try
            {
                //impostazioni iniziali

                this.DataGrid1.Visible = false;
                this.lbl_countRecord.Visible = false;


                RicercaNonProtocollatiSessionMng.ClearSessionData(this);

                //fine impostazioni

                //INIZIO VALIDAZIONE DATI INPUT ALLA RICERCA				
                if (txtInitDocumento.Text != "")
                {
                    if (IsValidNumber(txtInitDocumento) == false)
                    {
                        Response.Write("<script>alert('Il numero di documento deve essere numerico!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txtInitDocumento.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return;
                    }
                }
                if (txtEndDocumento.Text != "")
                {
                    if (IsValidNumber(txtEndDocumento) == false)
                    {
                        Response.Write("<script>alert('Il numero di protocollo deve essere numerico!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txtEndDocumento.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return;
                    }
                }


                if (this.ddl_dtaCreazione.SelectedIndex == 1)
                {
                    if (Utils.isDate(this.GetCalendarControl("txtInitDtaCreazione").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txtInitDtaCreazione").txt_Data.Text, this.GetCalendarControl("txtEndDataCreazione").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date !');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaCreazione").txt_Data.ID + "').focus();</SCRIPT>";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                        // Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }

                // FINE VALIDAZIONE

                currentPage = 1;

                if (RicercaDocumenti())
                {
                    DocumentManager.setFiltroRicDoc(this, qV);
                    LoadData(true);
                }
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }


    }

        #endregion




}
