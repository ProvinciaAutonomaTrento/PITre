using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{
    public partial class RicercaProtocolli : DocsPAWA.CssPage
    {
        protected System.Web.UI.WebControls.Label lbl_message;
        protected System.Web.UI.WebControls.DataGrid DataGrid1;
        protected System.Web.UI.WebControls.Label lblNumeroProtocollo;
        protected System.Web.UI.WebControls.Label lblDataProtocollo;
        protected System.Web.UI.WebControls.Label lblEndDataProtocollo;
        protected System.Web.UI.HtmlControls.HtmlTable tblNumeroProtocollo;
        //protected DocsPaWebCtrlLibrary.DateMask txtEndDataProtocollo;
        protected DocsPAWA.UserControls.Calendar txtEndDataProtocollo;
        protected System.Web.UI.WebControls.Label lblInitNumProto;
        protected System.Web.UI.WebControls.TextBox txtInitNumProto;
        protected System.Web.UI.WebControls.Label lblEndNumProto;
        protected System.Web.UI.WebControls.TextBox txtEndNumProto;
        protected System.Web.UI.WebControls.DropDownList ddl_dtaProto;
        protected System.Web.UI.WebControls.Label lblInitDtaProto;
        //protected DocsPaWebCtrlLibrary.DateMask txtInitDtaProto;
        protected DocsPAWA.UserControls.Calendar txtInitDtaProto;
        protected System.Web.UI.WebControls.DropDownList ddl_numProto;
        protected System.Web.UI.WebControls.Button btn_find;
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
        protected System.Web.UI.WebControls.Label lbl_annoProto;
        protected System.Web.UI.WebControls.TextBox txt_annoProto;
        protected System.Web.UI.WebControls.Label lbl_countRecord;
        protected System.Web.UI.WebControls.DataGrid dg_lista_corr;
        protected System.Web.UI.WebControls.ImageButton btn_chiudi_risultato;
        protected System.Web.UI.WebControls.Panel pnl_corr;
        protected System.Web.UI.WebControls.ImageButton btn_clearFlag;
        protected int nRec;
        protected DataSet ds;
        protected System.Web.UI.WebControls.Button btn_ok;
        protected DataTable dt;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;
        protected Hashtable ht_destinatariTO_CC;
        protected string str_indexSel;
        protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDocUscita;
        protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDocIngresso;
        protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc;
        protected DocsPAWA.DocsPaWR.InfoDocumento infoDocSel = null;
        protected DocsPAWA.UserControls.AppTitleProvider appTitleProvider;
        protected string tipoProto;
        protected string callPage;

        protected System.Web.UI.WebControls.ListItem opArr;
        protected System.Web.UI.WebControls.ListItem opPart;
        protected System.Web.UI.WebControls.ListItem opInt;
        protected DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;
        protected System.Web.UI.WebControls.ListItem nnprot;
        protected System.Web.UI.WebControls.ListItem pred;

        private DocsPAWA.DocsPaWR.Registro[] userRegistri;

        private void Page_Load(object sender, System.EventArgs e)
        {
            this.Response.Expires = -1;
            DocsPAWA.Utils.DefaultButton(this, ref this.GetCalendarControl("txtInitDtaProto").txt_Data, ref btn_find);
            DocsPAWA.Utils.DefaultButton(this, ref this.GetCalendarControl("txtEndDataProtocollo").txt_Data, ref btn_find);
            
            this.callPage = "";

            if (!IsPostBack)
            {
                pnl_doc.Visible = false;
                pnl_proto.Visible = false;
                RicercaDocumentiSessionMng.SetAsLoaded(this);
                pnl_proto.Visible = false;
                pnl_doc.Visible = false;
                string tipo = this.Request.QueryString["tipo"];
                ViewState["callpage"] = null;
                ViewState["callpage"] = this.Request.QueryString["page"];
                if (ViewState["callpage"] != null)
                {
                    this.callPage = ViewState["callpage"] as String;
                }
                switch (tipo)
                {
                    case "In": this.rbl_TipoDoc.Items.FindByValue("Partenza").Selected = true;
                        pnl_proto.Visible = true;
                        break;
                    case "Out": this.rbl_TipoDoc.Items.FindByValue("Arrivo").Selected = true;
                        pnl_proto.Visible = true;
                        break;
                    case "NP": this.rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected = true;
                        pnl_doc.Visible = true;
                        break;
                    case "Pred": this.rbl_TipoDoc.Items.FindByValue("Predisposti").Selected = true;
                        pnl_proto.Visible = true;
                        break;
                    case "Own": this.rbl_TipoDoc.Items.FindByValue("Interno").Selected = true;
                        pnl_proto.Visible = true;
                        break;
                }

                this.GetCalendarControl("txtEndDataProtocollo").Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").btn_Cal.Visible = false;
                this.GetCalendarControl("txtEndDataDoc").Visible = false;
                this.GetCalendarControl("txtEndDataDoc").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataDoc").btn_Cal.Visible = false;

                //GESTIONE CATENE EXTRA AOO
                if (UserManager.isFiltroAooEnabled(this))
                {
                    pnl_catene_extra_aoo.Visible = true;
                    CaricaComboRegistri(ddl_reg);
                }
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
                        RicercaDocumentiSessionMng.ClearSessionData(this);
                    }
                }
                if (ViewState["callpage"] != null)
                {
                    this.callPage = ViewState["callpage"] as String;
                }
            }

            #region ABILITAZIONE PROTOCOLLAZIONE INTERNA
            DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            if (!ws.IsInternalProtocolEnabled(cr.idAmministrazione))
                this.rbl_TipoDoc.Items.Remove(this.rbl_TipoDoc.Items.FindByValue("Interno"));
            #endregion

            getLettereProtocolli();
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
            this.ddl_numProto.SelectedIndexChanged += new System.EventHandler(this.ddl_numProto_SelectedIndexChanged);
            this.ddl_dtaProto.SelectedIndexChanged += new System.EventHandler(this.ddl_dtaProto_SelectedIndexChanged);
            this.btn_find.Click += new System.EventHandler(this.btn_find_Click);
            this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChange);
            this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
            this.btn_chiudi_risultato.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudi_risultato_Click);
            this.dg_lista_corr.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_lista_corr_PageIndexChanged);
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.ddl_numDoc.SelectedIndexChanged += new System.EventHandler(this.ddl_numDoc_SelectedIndexChanged);
            this.ddl_dtaDoc.SelectedIndexChanged += new System.EventHandler(this.ddl_dtaDoc_SelectedIndexChanged);
        }
        #endregion

        private void ddl_numProto_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.txtEndNumProto.Text = "";

            if (this.ddl_numProto.SelectedIndex == 0)
            {
                this.txtEndNumProto.Visible = false;
                this.lblEndNumProto.Visible = false;
                this.lblInitNumProto.Visible = false;
            }
            else
            {
                this.txtEndNumProto.Visible = true;
                this.lblEndNumProto.Visible = true;
                this.lblInitNumProto.Visible = true;
            }
        }

        private void ddl_numDoc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.txtEndNumDoc.Text = "";

            if (this.ddl_numDoc.SelectedIndex == 0)
            {
                this.txtEndNumDoc.Visible = false;
                this.lblEndNumDoc.Visible = false;
                this.lblInitNumDoc.Visible = false;
            }
            else
            {
                this.txtEndNumDoc.Visible = true;
                this.lblEndNumDoc.Visible = true;
                this.lblInitNumDoc.Visible = true;
            }
        }

        private void ddl_dtaDoc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.txtEndDataDoc.Text = "";

            if (this.ddl_dtaDoc.SelectedIndex == 0)
            {
                this.txtEndDataDoc.Visible = false;
                this.lblEndDataDoc.Visible = false;
                this.lblInitDtaDoc.Visible = false;
                this.GetCalendarControl("txtEndDataDoc").Visible = false;
                this.GetCalendarControl("txtEndDataDoc").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataDoc").btn_Cal.Visible = false;
            }
            else
            {
                this.txtEndDataDoc.Visible = true;
                this.lblEndDataDoc.Visible = true;
                this.lblInitDtaDoc.Visible = true;
                this.GetCalendarControl("txtEndDataDoc").Visible = true;
                this.GetCalendarControl("txtEndDataDoc").txt_Data.Visible = true;
                this.GetCalendarControl("txtEndDataDoc").btn_Cal.Visible = true;
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

                #region Filtro per REGISTRO
                if (!this.rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected)
                {
                    if (!UserManager.isFiltroAooEnabled(this))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                        fV1.valore = UserManager.getRegistroSelezionato(this).systemId;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                        if (ddl_reg.Items.Count <= 1)
                        {
                            fV1.valore = UserManager.getRegistroSelezionato(this).systemId;
                        }
                        else
                        {
                            fV1.valore = this.ddl_reg.SelectedItem.Value;
                        }
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                #endregion

                #region filtro NUMERO DI PROTOCOLLO
                if (this.ddl_numProto.SelectedIndex == 0)
                {
                    if (this.txtInitNumProto.Text != null && !this.txtInitNumProto.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                        fV1.valore = this.txtInitNumProto.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                    if (!this.txtInitNumProto.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                        fV1.valore = this.txtInitNumProto.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txtEndNumProto.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                        fV1.valore = this.txtEndNumProto.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro ANNO DI PROTOCOLLO

                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                fV1.valore = this.txt_annoProto.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                #region filtro DATA PROTOCOLLO
                if (this.ddl_dtaProto.SelectedIndex == 0)
                {//valore singolo specificato per DATA_PROTOCOLLO
                    if (!this.GetCalendarControl("txtInitDtaProto").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txtInitDtaProto").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaProto").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txtInitDtaProto").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                    if (!this.GetCalendarControl("txtInitDtaProto").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txtInitDtaProto").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaProto").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txtInitDtaProto").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtEndDataProtocollo").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro per ricerca protocollo in PARTENZA
                if (this.rbl_TipoDoc.Items.FindByValue("Partenza").Selected)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                    fV1.valore = "P";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro per ricerca protocollo in ARRIVO
                if (this.rbl_TipoDoc.Items.FindByValue("Arrivo").Selected)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                    fV1.valore = "A";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro per ricerca protocollo INTERNI
                if (this.rbl_TipoDoc.Items.FindByValue("Interno") != null)
                {
                    if (this.rbl_TipoDoc.Items.FindByValue("Interno").Selected)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                        fV1.valore = "true";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                        fV1.valore = "I";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro per ricerca documenti non protocollati
                if (this.rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                    fV1.valore = "G";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro per ricerca documenti predisposti
                if (this.rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region NUOVO SVILUPPO CONCATENAMENTO TRASVERSALE PER TUTTI I TIPI DI DOCUMENTI
                #region filtro NUMERO DI DOCUMENTO
                if (this.ddl_numDoc.SelectedIndex == 0)
                {
                    if (this.txtInitDoc.Text != null && !this.txtInitDoc.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                        fV1.valore = this.txtInitDoc.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {//valore singolo carico DOCNUMBER_DAL - DOCNUMBER_AL
                    if (!this.txtInitDoc.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                        fV1.valore = this.txtInitDoc.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txtEndNumDoc.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                        fV1.valore = this.txtEndNumDoc.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro DATA CREAZIONE
                if (this.ddl_dtaDoc.SelectedIndex == 0)
                {//valore singolo specificato per DATA_CREAZIONE
                    if (!this.GetCalendarControl("txtInitDtaDoc").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txtInitDtaDoc").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaDoc").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txtInitDtaDoc").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {//valore singolo carico DATA_CREAZIONE_DAL - DATA_CREAZIONE_AL
                    if (!this.GetCalendarControl("txtInitDtaDoc").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txtInitDtaDoc").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaDoc").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txtInitDtaDoc").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.GetCalendarControl("txtEndDataDoc").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txtEndDataDoc").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtEndDataDoc").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return false;
                        }
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txtEndDataDoc").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

 
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
            DocsPaWR.InfoUtente infoUt = new DocsPAWA.DocsPaWR.InfoUtente();
            infoUt = UserManager.getInfoUtente(this);
            ListaFiltri = DocumentManager.getFiltroRicDoc(this);
            SearchResultInfo[] idProfileList;
            if (!this.chk_ADL.Checked == true)
            {
                bool grigi = false;
                if (rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected) // || rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
                    grigi = true;
                infoDoc = DocumentManager.getQueryInfoDocumentoPaging(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, currentPage, out numTotPage, out nRec, true, grigi, true, false, out idProfileList);

                //Controllo inserito per non far visualizzare se stesso
                DocsPaWR.InfoDocumento infoDocumentoLav = DocumentManager.getInfoDocumento(DocumentManager.getDocumentoSelezionato(this));
                if (infoDocumentoLav != null)
                {
                    List<DocsPaWR.InfoDocumento> tempDoc = new List<DocsPaWR.InfoDocumento>();
                    tempDoc = infoDoc.ToList<DocsPaWR.InfoDocumento>();
                    foreach (DocsPaWR.InfoDocumento infDocTemp in infoDoc)
                    {
                        if (infDocTemp.docNumber == infoDocumentoLav.docNumber)
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
                DocsPaWR.AreaLavoro areaLavoro = DocumentManager.getListaAreaLavoro(this, "P", "0", currentPage, out numTotPage, out nRec, UserManager.getRegistroSelezionato(this).systemId);

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
            RicercaDocumentiSessionMng.SetListaInfoDocumenti(this, infoDoc);

            if (infoDoc != null && infoDoc.Length > 0)
            {
                this.BindGrid(infoDoc);
            }

        }

        public void BindGrid(DocsPAWA.DocsPaWR.InfoDocumento[] infos)
        {
            DocsPaWR.InfoDocumento currentDoc;
            DocsPaWR.InfoDocumento infoDocumentoLav = DocumentManager.getInfoDocumento(DocumentManager.getDocumentoSelezionato(this));

            if (infos != null && infos.Length > 0)
            {
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


                    if (currentDoc.numProt != null && !currentDoc.numProt.Equals(""))
                    {
                        numProt = Int32.Parse(currentDoc.numProt);
                        descrDoc = numProt.ToString();
                    }
                    else
                    {
                        descrDoc = currentDoc.docNumber;
                    }
                    //numProt=Int32.Parse(currentDoc.numProt);
                    //descrDoc=numProt.ToString();

                    descrDoc = descrDoc + "\n" + dataApertura;
                    string MittDest = "";

                    if (currentDoc.mittDest != null && currentDoc.mittDest.Length > 0)
                    {
                        for (int g = 0; g < currentDoc.mittDest.Length; g++)
                            MittDest += currentDoc.mittDest[g] + " - ";
                        if (currentDoc.mittDest.Length > 0)
                            MittDest = MittDest.Substring(0, MittDest.Length - 3);
                    }

                    Dg_elem.Add(new ProtocolloDataGridItem(descrDoc, currentDoc.oggetto, MittDest, currentDoc.codRegistro, i));
                }

                this.DataGrid1.SelectedIndex = -1;
                this.DataGrid1.DataSource = Dg_elem;
                this.DataGrid1.DataBind();
                this.DataGrid1.Visible = true;

                for (int i = 0; i < DataGrid1.Items.Count; i++)
                {
                    Label lbl_numDoc = DataGrid1.Items[i].Cells[0].Controls[1] as Label;
                    lbl_numDoc.Font.Bold = true;
                    if (rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected || rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
                    {
                        lbl_numDoc.ForeColor = Color.Black;
                    }
                    else
                    {
                        lbl_numDoc.ForeColor = Color.Red;
                    }
                }

                if (rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected) // || rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
                {
                    this.DataGrid1.Columns[1].Visible = false;
                    this.DataGrid1.Columns[3].Visible = false;
                    this.DataGrid1.Columns[4].Visible = false;
                    this.DataGrid1.Columns[5].Visible = false;
                    this.DataGrid1.Columns[6].Visible = true;
                }
                if (rbl_TipoDoc.Items.FindByValue("Arrivo").Selected)
                {
                    if(this.callPage.Equals("profilo")){
                        this.DataGrid1.Columns[1].Visible = true;
                        this.DataGrid1.Columns[3].Visible = false;
                        this.DataGrid1.Columns[4].Visible = false;
                        this.DataGrid1.Columns[5].Visible = false;
                        this.DataGrid1.Columns[6].Visible = true;
                    }
                    else
                    {
                        this.DataGrid1.Columns[1].Visible = true;
                        this.DataGrid1.Columns[3].Visible = false;
                        this.DataGrid1.Columns[4].Visible = false;
                        this.DataGrid1.Columns[5].Visible = true;
                        this.DataGrid1.Columns[6].Visible = true;
                    }
                }
                if (rbl_TipoDoc.Items.FindByValue("Partenza").Selected)
                {
                    if (this.callPage.Equals("profilo"))
                    {
                        this.DataGrid1.Columns[1].Visible = true;
                        this.DataGrid1.Columns[3].Visible = false;
                        this.DataGrid1.Columns[4].Visible = false;
                        this.DataGrid1.Columns[5].Visible = false;
                        this.DataGrid1.Columns[6].Visible = true;
                    }
                    else
                    {
                        if (infoDocumentoLav.tipoProto.Equals("P"))
                        {
                            this.DataGrid1.Columns[1].Visible = true;
                            this.DataGrid1.Columns[3].Visible = false;
                            this.DataGrid1.Columns[4].Visible = false;
                            this.DataGrid1.Columns[5].Visible = false;
                            this.DataGrid1.Columns[6].Visible = true;
                        }
                        else
                        {
                            if((infoDocumentoLav.tipoProto.Equals("A") || infoDocumentoLav.tipoProto.Equals("I"))&& infoDocumentoLav.docNumber!=null && infoDocumentoLav.segnatura != null)
                            {
                                if(infoDocumentoLav.segnatura.Equals("")){
                                     this.DataGrid1.Columns[1].Visible = true;
                                     this.DataGrid1.Columns[3].Visible = false;
                                     this.DataGrid1.Columns[4].Visible = false;
                                     this.DataGrid1.Columns[5].Visible = false;
                                     this.DataGrid1.Columns[6].Visible = true;
                                }
                                else{
                                    this.DataGrid1.Columns[1].Visible = true;
                                    this.DataGrid1.Columns[3].Visible = false;
                                    this.DataGrid1.Columns[4].Visible = true;
                                    this.DataGrid1.Columns[5].Visible = false;
                                    this.DataGrid1.Columns[6].Visible = false;
                                }
                            }
                            else{
                                this.DataGrid1.Columns[1].Visible = true;
                                this.DataGrid1.Columns[3].Visible = false;
                                this.DataGrid1.Columns[4].Visible = true;
                                this.DataGrid1.Columns[5].Visible = false;
                                this.DataGrid1.Columns[6].Visible = false;
                            }
                        }
                    }
                }
                if (rbl_TipoDoc.Items.FindByValue("Interno") != null)
                {
                    if (rbl_TipoDoc.Items.FindByValue("Interno").Selected)
                    {
                        if (this.callPage.Equals("profilo"))
                        {
                            this.DataGrid1.Columns[1].Visible = true;
                            this.DataGrid1.Columns[3].Visible = true;
                            this.DataGrid1.Columns[4].Visible = false;
                            this.DataGrid1.Columns[5].Visible = false;
                            this.DataGrid1.Columns[6].Visible = true;
                        }
                        else{
                            if (infoDocumentoLav.tipoProto.Equals("I") || ((infoDocumentoLav.tipoProto.Equals("P") || infoDocumentoLav.tipoProto.Equals("A")) && infoDocumentoLav.docNumber != null))
                            { 
                                this.DataGrid1.Columns[1].Visible = true;
                                this.DataGrid1.Columns[3].Visible = true;
                                this.DataGrid1.Columns[4].Visible = false;
                                this.DataGrid1.Columns[5].Visible = false;
                                this.DataGrid1.Columns[6].Visible = true;
                            }
                            else
                            {
                            /*    this.DataGrid1.Columns[1].Visible = true;
                                this.DataGrid1.Columns[3].Visible = true;
                                this.DataGrid1.Columns[4].Visible = false;
                                this.DataGrid1.Columns[5].Visible = false;
                                this.DataGrid1.Columns[6].Visible = true;*/
                                this.DataGrid1.Columns[1].Visible = true;
                                this.DataGrid1.Columns[3].Visible = false;
                                this.DataGrid1.Columns[4].Visible = true;
                                this.DataGrid1.Columns[5].Visible = false;
                                this.DataGrid1.Columns[6].Visible = false;
                            }
                        }
                    }
                }

               if (rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
                {
                   this.DataGrid1.Columns[1].Visible = true;
                   this.DataGrid1.Columns[3].Visible = true;
                   this.DataGrid1.Columns[4].Visible = false;
                   this.DataGrid1.Columns[5].Visible = false;
                   this.DataGrid1.Columns[6].Visible = true;
                }

                


            }
            else
            {
                this.DataGrid1.Visible = false;
                this.lbl_message.Visible = true;
            }
        }

        public class ProtocolloDataGridItem
        {
            private string data;
            private string oggetto;
            private string mittDest;
            private string registro;
            private int chiave;

            public ProtocolloDataGridItem(string data, string oggetto, string mittDest, string registro, int chiave)
            {
                this.data = data;
                this.oggetto = oggetto;
                this.registro = registro;
                this.mittDest = mittDest;
                this.chiave = chiave;
            }

            public string Data { get { return data; } }
            public string Oggetto { get { return oggetto; } }
            public string Registro { get { return registro; } }
            public string MittDest { get { return mittDest; } }
            public int Chiave { get { return chiave; } }
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
            this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text = "";
            if (this.ddl_dtaProto.SelectedIndex == 0)
            {
                this.GetCalendarControl("txtEndDataProtocollo").Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").btn_Cal.Visible = false;
                this.lblEndDataProtocollo.Visible = false;
                this.lblInitDtaProto.Visible = false;

            }
            else
            {
                this.GetCalendarControl("txtEndDataProtocollo").Visible = true;
                this.GetCalendarControl("txtEndDataProtocollo").btn_Cal.Visible = true;
                this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Visible = true;
                this.lblEndDataProtocollo.Visible = true;
                this.lblInitDtaProto.Visible = true;
            }
        }

        private void DataGrid1_PageIndexChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            currentPage = e.NewPageIndex + 1;
            pnl_corr.Visible = false;
            // Cricamento del DataGrid
            this.LoadData(true);
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
            ds = this.CreateGridDataSetDestinatari();
            this.CaricaGridDataSetDestinatari(ds, listaCorrTo, listaCorrCC);
            this.dg_lista_corr.DataSource = ds;
            DocumentManager.setDataGridProtocolliUscita(this, dt);
            this.dg_lista_corr.DataBind();

            // Impostazione corrispondente predefinito
            this.SelectDefaultCorrispondente();
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
        /// Caricamento dataset utilizzato per le griglie
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="uo"></param>
        private void CaricaGridDataSetDestinatari(DataSet ds, DocsPAWA.DocsPaWR.Corrispondente[] listaCorrTo, DocsPAWA.DocsPaWR.Corrispondente[] listaCorrCC)
        {
            DataTable dt = ds.Tables["GRID_TABLE_DESTINATARI"];
            ht_destinatariTO_CC = new Hashtable();
            string tipoURP = "";

            if (listaCorrTo != null && listaCorrTo.Length > 0)
            {
                for (int i = 0; i < listaCorrTo.Length; i++)
                {
                    if (listaCorrTo[i].tipoCorrispondente != null && listaCorrTo[i].tipoCorrispondente.Equals("O"))
                    {
                        this.AppendDataRow(dt, listaCorrTo[i].tipoCorrispondente, listaCorrTo[i].systemId, "&nbsp;" + listaCorrTo[i].descrizione);
                    }
                    else
                    {
                        if (listaCorrTo[i].GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)))
                        {
                            tipoURP = "U";
                        }
                        if (listaCorrTo[i].GetType().Equals(typeof(DocsPAWA.DocsPaWR.Ruolo)))
                        {
                            tipoURP = "R";
                        }
                        if (listaCorrTo[i].GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
                        {
                            tipoURP = "P";
                        }
                        this.AppendDataRow(dt, listaCorrTo[i].tipoIE, listaCorrTo[i].systemId, GetImage(tipoURP) + " - " + listaCorrTo[i].descrizione);
                    }
                    ht_destinatariTO_CC.Add(listaCorrTo[i].systemId, listaCorrTo[i]);
                }
            }
            if (listaCorrCC != null && listaCorrCC.Length > 0)
            {
                for (int i = 0; i < listaCorrCC.Length; i++)
                {
                    if (listaCorrCC[i].tipoCorrispondente != null && listaCorrCC[i].tipoCorrispondente.Equals("O"))
                    {
                        this.AppendDataRow(dt, listaCorrCC[i].tipoCorrispondente, listaCorrCC[i].systemId, "&nbsp;" + listaCorrCC[i].descrizione + " (CC)");
                    }
                    else
                    {
                        if (listaCorrCC[i].GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)))
                        {
                            tipoURP = "U";
                        }
                        if (listaCorrCC[i].GetType().Equals(typeof(DocsPAWA.DocsPaWR.Ruolo)))
                        {
                            tipoURP = "R";
                        }
                        if (listaCorrCC[i].GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
                        {
                            tipoURP = "P";
                        }
                        this.AppendDataRow(dt, listaCorrCC[i].tipoIE, listaCorrCC[i].systemId, GetImage(tipoURP) + " - " + listaCorrCC[i].descrizione + " (CC)");
                    }
                    ht_destinatariTO_CC.Add(listaCorrCC[i].systemId, listaCorrCC[i]);
                }
            }
            if ((listaCorrTo != null && listaCorrTo.Length > 0) || (listaCorrCC != null && listaCorrCC.Length > 0))
            {
                this.pnl_corr.Visible = true;
            }
            DocumentManager.setHash(this, ht_destinatariTO_CC);

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

        private void btn_chiudi_risultato_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            pnl_corr.Visible = false;
        }

        private void dg_lista_corr_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.dg_lista_corr.SelectedIndex = -1;
            this.dg_lista_corr.CurrentPageIndex = e.NewPageIndex;
            dg_lista_corr.DataSource = DocumentManager.getDataGridProtocolliUscita(this);
            dg_lista_corr.DataBind();

        }

        private void btn_ok_Click(object sender, System.EventArgs e)
        {
            DocsPaWR.Corrispondente destSelected = null;
            bool avanzaDoc;
            bool diventaOccasionale = true;
            //itemIndex: indice item del datagrid che contiene radio selezionato
            int itemIndex;
            #region PROTOCOLLO PARTENZA
            if (rbl_TipoDoc.Items.FindByValue("Partenza").Selected)
            {
                /*	bool avanza:
                 *	- true: si può procedere perchè un corrispondente è stato selezionato;		
                 *  - false : non si procede e si avvisa l'utente che deve selezionarne uno */
                //bool avanza = verificaSelezione(out itemIndex);
                avanzaDoc = verificaSelezioneDocumento();
                if (avanzaDoc)
                {
                    bool avanzaCor = verificaSelezione(out itemIndex);
                    DocsPaWR.InfoDocumento infoDocumentoLav = DocumentManager.getInfoDocumento(DocumentManager.getDocumentoSelezionato(this));

                    if (avanzaCor || this.callPage.Equals("profilo") || infoDocumentoLav.tipoProto.Equals("P") || (infoDocumentoLav.tipoProto.Equals("A") || infoDocumentoLav.tipoProto.Equals("I") && infoDocumentoLav.docNumber != null && infoDocumentoLav.segnatura.Equals("")))
                    {
                        bool mittenteOK = false;
                        bool oggettoOK = false;
                        string oggettoProtoSel = "";

                        schedaDocUscita = DocumentManager.getDocumentoInLavorazione(this);
                        //NEL CASO IN CUI PARTO DA UN DOCUMENTO GRIGIO O RISPONDO A UNO IN PARTENZA CON UNO IN PARTENZA
                        if (this.callPage.Equals("profilo") || infoDocumentoLav.tipoProto.Equals("P") || ((infoDocumentoLav.tipoProto.Equals("A") || infoDocumentoLav.tipoProto.Equals("I")) && infoDocumentoLav.docNumber!=null && infoDocumentoLav.segnatura.Equals("")))
                        {
                            verificaSelezioneDocumento(out itemIndex);

                            this.infoDoc = RicercaDocumentiSessionMng.GetListaInfoDocumenti(this);

                            if (itemIndex > -1)
                            {
                                infoDocSel = (DocsPAWA.DocsPaWR.InfoDocumento)this.infoDoc[itemIndex];
                                DocumentManager.setInfoDocumento(this, infoDocSel);
                            }

                            
                            if (schedaDocUscita != null)
                            {
                                if (this.DataGrid1.SelectedIndex >= 0)
                                {
                                    oggettoProtoSel = ((Label)DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[2].Controls[1]).Text;
                                }
                                //inizio verifica congruenza campo oggetto
                                if (schedaDocUscita.docNumber != null)
                                {
                                    if (schedaDocUscita.oggetto != null && schedaDocUscita.oggetto.descrizione != null && schedaDocUscita.oggetto.descrizione != String.Empty)
                                    {
                                        if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                        {
                                            if (schedaDocUscita.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                            {
                                                oggettoOK = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    oggettoOK = true;
                                    schedaDocUscita.oggetto.descrizione = oggettoProtoSel;

                                }
                                if (infoDocumentoLav.tipoProto != null)
                                {
                                    if (infoDocumentoLav.tipoProto.Equals("P") && schedaDocUscita.protocollo != null && !oggettoOK)
                                    {
                                        if (!schedaDocUscita.protocollo.segnatura.Equals(""))
                                        {
                                            //se i corrisp non coincidono si lancia un avviso	all'utente 
                                            if (!this.Page.IsStartupScriptRegistered("avvisoModale"))
                                            {
                                                string scriptString = "<SCRIPT>OpenAvvisoModale('" + mittenteOK + "' , '" + oggettoOK + "' , 'True' , '" + diventaOccasionale + "');</SCRIPT>";
                                                this.Page.RegisterStartupScript("avvisoModale", scriptString);
                                            }
                                        }
                                    }
                                    bool first_step = false;
                                    if (infoDocumentoLav.tipoProto.Equals("G") || infoDocumentoLav.tipoProto.Equals("R") || infoDocumentoLav.tipoProto.Equals("C") || ((infoDocumentoLav.tipoProto.Equals("A") || infoDocumentoLav.tipoProto.Equals("I")) && infoDocumentoLav.docNumber != null && infoDocumentoLav.segnatura.Equals("")))
                                    {
                                        first_step = true;
                                        if (!oggettoOK)
                                        {
                                            if (!this.Page.IsStartupScriptRegistered("avvisoModale"))
                                            {
                                                string scriptString = "";
                                                if (!infoDocSel.segnatura.Equals(""))
                                                {
                                                    scriptString = "<SCRIPT>OpenAvvisoModale('" + null + "' , '" + oggettoOK + "' , 'False' , '" + diventaOccasionale + "');</SCRIPT>";
                                                }
                                                else
                                                {
                                                    scriptString = "<SCRIPT>OpenAvvisoModale('" + null + "' , '" + oggettoOK + "' , 'True' , '" + diventaOccasionale + "');</SCRIPT>";
                                                }
                                                this.Page.RegisterStartupScript("avvisoModale", scriptString);
                                            }

                                        }
                                        else
                                        {
                                            infoDocSel = DocumentManager.getInfoDocumento(this);
                                            if (infoDocSel != null)
                                            {
                                                schedaDocUscita.rispostaDocumento = infoDocSel;
                                                schedaDocUscita.modificaRispostaDocumento = true;
                                            }


                                            DocumentManager.setDocumentoSelezionato(this, schedaDocUscita);
                                            Page.RegisterStartupScript("", "<script>window.close();</script>");
                                        }
                                    }
                                    if (first_step == false)
                                    {
                                        if (infoDocumentoLav.segnatura != null)
                                        {
                                            if (infoDocumentoLav.tipoProto.Equals("P") && infoDocumentoLav.segnatura.Equals(""))
                                            {
                                                if (!oggettoOK)
                                                {
                                                    if (!this.Page.IsStartupScriptRegistered("avvisoModale"))
                                                    {
                                                        string scriptString = "";
                                                        if (!infoDocSel.segnatura.Equals(""))
                                                        {
                                                            scriptString = "<SCRIPT>OpenAvvisoModale('" + null + "' , '" + oggettoOK + "' , 'False' , '" + diventaOccasionale + "');</SCRIPT>";
                                                        }
                                                        else
                                                        {
                                                            scriptString = "<SCRIPT>OpenAvvisoModale('" + null + "' , '" + oggettoOK + "' , 'True' , '" + diventaOccasionale + "');</SCRIPT>";
                                                        }
                                                        this.Page.RegisterStartupScript("avvisoModale", scriptString);
                                                    }

                                                }
                                                else
                                                {
                                                    infoDocSel = DocumentManager.getInfoDocumento(this);
                                                    if (infoDocSel != null)
                                                    {
                                                        schedaDocUscita.rispostaDocumento = infoDocSel;
                                                        schedaDocUscita.modificaRispostaDocumento = true;
                                                    }


                                                    DocumentManager.setDocumentoSelezionato(this, schedaDocUscita);
                                                    Page.RegisterStartupScript("", "<script>window.close();</script>");
                                                }

                                            }
                                            else
                                            {
                                                if (!oggettoOK)
                                                {
                                                    if (!this.Page.IsStartupScriptRegistered("avvisoModale"))
                                                    {
                                                        string scriptString = "";
                                                        if (!infoDocSel.segnatura.Equals(""))
                                                        {
                                                            scriptString = "<SCRIPT>OpenAvvisoModale('" + null + "' , '" + oggettoOK + "' , 'False' , '" + diventaOccasionale + "');</SCRIPT>";
                                                        }
                                                        else
                                                        {
                                                            scriptString = "<SCRIPT>OpenAvvisoModale('" + null + "' , '" + oggettoOK + "' , 'True' , '" + diventaOccasionale + "');</SCRIPT>";
                                                        }
                                                        this.Page.RegisterStartupScript("avvisoModale", scriptString);
                                                    }

                                                }
                                                else
                                                {
                                                    infoDocSel = DocumentManager.getInfoDocumento(this);
                                                    if (infoDocSel != null)
                                                    {
                                                        schedaDocUscita.rispostaDocumento = infoDocSel;
                                                        schedaDocUscita.modificaRispostaDocumento = true;
                                                    }


                                                    DocumentManager.setDocumentoSelezionato(this, schedaDocUscita);
                                                    Page.RegisterStartupScript("", "<script>window.close();</script>");
                                                }

                                            }

                                        }
                                        else
                                        {

                                            infoDocSel = DocumentManager.getInfoDocumento(this);
                                            if (infoDocSel != null)
                                            {
                                                schedaDocUscita.rispostaDocumento = infoDocSel;
                                                schedaDocUscita.modificaRispostaDocumento = true;
                                            }


                                            DocumentManager.setDocumentoSelezionato(this, schedaDocUscita);
                                            Page.RegisterStartupScript("", "<script>window.close();</script>");


                                        }
                                    }
                                }
                                else
                                {
                                    if (!oggettoOK)
                                    {
                                        //se i corrisp non coincidono si lancia un avviso	all'utente 
                                        if (!this.Page.IsStartupScriptRegistered("avvisoModale"))
                                        {
                                            string scriptString = "";
                                            if (!infoDocSel.segnatura.Equals(""))
                                            {
                                                scriptString = "<SCRIPT>OpenAvvisoModale('" + null + "' , '" + oggettoOK + "' , 'False' , '" + diventaOccasionale + "');</SCRIPT>";
                                            }
                                            else
                                            {
                                                scriptString = "<SCRIPT>OpenAvvisoModale('" + null + "' , '" + oggettoOK + "' , 'True' , '" + diventaOccasionale + "');</SCRIPT>";
                                            }
                                            this.Page.RegisterStartupScript("avvisoModale", scriptString);
                                        }
                                    }
                                    else
                                    {
                                        infoDocSel = DocumentManager.getInfoDocumento(this);
                                        if (infoDocSel != null)
                                        {
                                            schedaDocUscita.rispostaDocumento = infoDocSel;
                                            schedaDocUscita.modificaRispostaDocumento = true;
                                        }


                                        DocumentManager.setDocumentoSelezionato(this, schedaDocUscita);
                                        Page.RegisterStartupScript("", "<script>window.close();</script>");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (avanzaCor)
                            {
                                //ricavo la system_id del corrispondente selezionato, contenuta nella prima colonna del datagrid
                                string key = dg_lista_corr.Items[itemIndex].Cells[0].Text;

                                //prendo la hashTable che contiene i corrisp dalla sesisone
                                ht_destinatariTO_CC = DocumentManager.getHash(this);

                                if (ht_destinatariTO_CC != null)
                                {
                                    if (ht_destinatariTO_CC.ContainsKey(key))
                                    {
                                        //prendo il corrispondente dalla hashTable secondo quanto è stato richiesto dall'utente
                                        destSelected = (DocsPAWA.DocsPaWR.Corrispondente)ht_destinatariTO_CC[key];

                                        #region CHIARIMENTO
                                        /*	CASI POSSIBILI PER RISPONDERE A UN PROTOCOLLO	
							*			----------------------------------------------------------------------------------
							*			caso 1 - il documento in ingresso (il protocollo di risposta) non è protocollato
							*						
							*					caso 1.1 - il campo mittente non è valorizzato, quindi posso
							*							   popolarlo con quello selezionato dall'utente
							*					
							*					caso 1.2 - il campo mittente è popolato, se sono diversi avviso l'utente
							*			
							*			Analogo il discorso per il campo oggetto, se sono diversi avviso l'utente e gli
							*			daremo la possibilità di scegliere se proseguire o meno con l'operazione
							*			di collegamento	
							* 		   
							*			-----------------------------------------------------------------------------------
							*			caso 2 - il documento in ingresso è già protocollato, ha quindi un mittente
							*					
							*					caso 2.1 - mittente scelto è diverso da quello corrente, si avvisa l'utente
							*		     				   e si da la possibilità si proseguire o meno con il collegamento
							* 
							*			Analogo il discorso per il campo oggetto
							*			-----------------------------------------------------------------------------------
							* */
                                        #endregion

                                        //prendo la scheda documento corrente in sessione
                                        schedaDocIngresso = DocumentManager.getDocumentoInLavorazione(this);

                                        //Gestione corrispondenti nel caso di Corrispondenti extra AOO
                                        if (UserManager.isFiltroAooEnabled(this))
                                        {

                                            if (schedaDocIngresso != null)
                                            {
                                                DocsPAWA.DocsPaWR.Registro tempRegUser = UserManager.getRegistroSelezionato(this);
                                                infoDocSel = DocumentManager.getInfoDocumento(this);
                                                if (tempRegUser.systemId != infoDocSel.idRegistro && !string.IsNullOrEmpty(destSelected.idRegistro))
                                                {
                                                    DocsPAWA.DocsPaWR.Corrispondente tempDest = destSelected;
                                                    tempDest.codiceRubrica = null;
                                                    tempDest.idOld = "0";
                                                    tempDest.systemId = null;
                                                    tempDest.tipoCorrispondente = "O";
                                                    tempDest.tipoIE = null;
                                                    tempDest.idRegistro = tempRegUser.systemId;
                                                    tempDest.idAmministrazione = UserManager.getUtente(this).idAmministrazione;
                                                    diventaOccasionale = false;
                                                    destSelected = tempDest;
                                                }
                                            }
                                        }

                                        if (schedaDocIngresso != null)
                                        {
                                            #region GESTIONE RISPOSTA AL PROTOCOLLO NON ANCORA PROTOCOLLATA
                                            //se non è protocollato
                                            if (schedaDocIngresso.protocollo != null && (schedaDocIngresso.protocollo.numero == null || schedaDocIngresso.protocollo.numero.Equals("")))
                                            {
                                                if (!this.schedaDocIngresso.tipoProto.Equals("P") && !this.schedaDocIngresso.tipoProto.Equals("I"))
                                                {
                                                    // il campo mittente della scheda documento relativa al protocollo di risposta non è popolato
                                                    if (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente == null)
                                                    {
                                                        mittenteOK = true;
                                                        //popolo il campo mittente con il destinatario selezionato dal protocollo a cui ri risponde 
                                                        //((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = destSelected;
                                                        RicercaDocumentiSessionMng.setCorrispondenteRispostaUSCITA(this, destSelected);
                                                    }
                                                    else
                                                    {
                                                        //il campo mittente è stato popolato prima di selezionere il protocollo a cui rispondere,
                                                        //quindi si verifica se i corrispondenti coincidono. In caso affermativo si procede con il collegamento,
                                                        //in caso contrario avviso l'utente, dando la possibilità di scegliere se proseguire con i nuovi dati o meno.

                                                        //per i mittenti occasionali come si fa ?
                                                        bool verificaUguaglianzaCorr = verificaUguaglianzacorrispondenti(destSelected, ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente);
                                                        if (verificaUguaglianzaCorr)
                                                        {
                                                            mittenteOK = true;
                                                        }
                                                        else
                                                        {
                                                            //setto il destinatario selezionato in sessione
                                                            RicercaDocumentiSessionMng.setCorrispondenteRispostaUSCITA(this, destSelected);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    mittenteOK = true;
                                                }

                                                if (this.DataGrid1.SelectedIndex >= 0)
                                                {
                                                    oggettoProtoSel = ((Label)DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[2].Controls[1]).Text;
                                                }
                                                //inizio verifica congruenza campo oggetto
                                                if (schedaDocIngresso.oggetto != null && schedaDocIngresso.oggetto.descrizione != null && schedaDocIngresso.oggetto.descrizione != String.Empty)
                                                {
                                                    if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                                    {
                                                        if (schedaDocIngresso.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                                        {
                                                            oggettoOK = true;
                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                    oggettoOK = true;
                                                    if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                                    {
                                                        DocsPaWR.Oggetto ogg = new DocsPAWA.DocsPaWR.Oggetto();
                                                        ogg.descrizione = oggettoProtoSel.ToString();
                                                        schedaDocIngresso.oggetto = ogg;
                                                    }
                                                }

                                                infoDocSel = DocumentManager.getInfoDocumento(this);
                                                if (infoDocSel != null)
                                                {
                                                    schedaDocIngresso.rispostaDocumento = infoDocSel;
                                                    schedaDocIngresso.modificaRispostaDocumento = true;
                                                }

                                                if (rbl_TipoDoc.Items.FindByValue("Partenza").Selected && schedaDocIngresso.tipoProto.Equals("A"))
                                                {
                                                    if (RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this) != null)
                                                    {
                                                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = destSelected;
                                                    }
                                                }

                                                if (rbl_TipoDoc.Items.FindByValue("Partenza").Selected && schedaDocIngresso.tipoProto.Equals("I"))
                                                {
                                                    ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocIngresso.protocollo).mittente = destSelected;
                                                }

                                                DocumentManager.setDocumentoSelezionato(this, schedaDocIngresso);

                                                RicercaDocumentiSessionMng.SetDialogReturnValue(this, true);

                                                if (!diventaOccasionale)
                                                {
                                                    if (!this.Page.IsStartupScriptRegistered("avvisoModale"))
                                                    {
                                                        mittenteOK = true;
                                                        oggettoOK = true;
                                                        string scriptString = "<SCRIPT>OpenAvvisoModale('" + mittenteOK + "' , '" + oggettoOK + "' , 'False' , '" + diventaOccasionale + "');</SCRIPT>";
                                                        this.Page.RegisterStartupScript("avvisoModale", scriptString);
                                                    }
                                                }
                                                else
                                                {

                                                    Page.RegisterStartupScript("", "<script>window.close();</script>");
                                                }
                                            }
                                        }


                                            #endregion

                                        #region GESTIONE RISPOSTA AL PROTOCOLLO GIA' PRECEDENTEMENTE PROTOCOLLATA

                                        if (schedaDocIngresso.protocollo != null && schedaDocIngresso.protocollo.numero != null && schedaDocIngresso.protocollo.numero != String.Empty)
                                        {
                                            bool verificaUguaglianzaCorr;
                                            //INSERITO PER BUG SE DA UN PROTOCOLLO INTERNO CERCO UNO IN PARTENZA
                                            if (schedaDocIngresso.tipoProto.Equals("I"))
                                            {
                                                verificaUguaglianzaCorr = false;
                                            }
                                            else
                                            {
                                                verificaUguaglianzaCorr = verificaUguaglianzacorrispondenti(destSelected, ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente);
                                            }
                                            if (verificaUguaglianzaCorr)
                                            {
                                                mittenteOK = true;
                                            }
                                            if (this.DataGrid1.SelectedIndex >= 0)
                                            {
                                                oggettoProtoSel = ((Label)DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[2].Controls[1]).Text;
                                            }
                                            //inizio verifica congruenza campo oggetto
                                            if (schedaDocIngresso.oggetto != null && schedaDocIngresso.oggetto.descrizione != null && schedaDocIngresso.oggetto.descrizione != String.Empty)
                                            {
                                                if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                                {
                                                    if (schedaDocIngresso.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                                    {
                                                        oggettoOK = true;
                                                    }
                                                }
                                            }
                                            if (!mittenteOK || !oggettoOK || !diventaOccasionale)
                                            {
                                                //se i corrisp non coincidono si lancia un avviso	all'utente 
                                                if (!this.Page.IsStartupScriptRegistered("avvisoModale"))
                                                {
                                                    string scriptString = "<SCRIPT>OpenAvvisoModale('" + mittenteOK + "' , '" + oggettoOK + "' , 'True' , '" + diventaOccasionale + "');</SCRIPT>";
                                                    this.Page.RegisterStartupScript("avvisoModale", scriptString);
                                                }
                                            }
                                            else
                                            {
                                                infoDocSel = DocumentManager.getInfoDocumento(this);
                                                if (infoDocSel != null)
                                                {
                                                    schedaDocIngresso.rispostaDocumento = infoDocSel;
                                                    schedaDocIngresso.modificaRispostaDocumento = true;
                                                }


                                                DocumentManager.setDocumentoSelezionato(this, schedaDocIngresso);
                                                Page.RegisterStartupScript("", "<script>window.close();</script>");
                                            }
                                        #endregion
                                        }
                                    }
                                    else
                                    {
                                        //se entro qui è perchè si è verificato errore
                                        throw new Exception("Errore nella gestione dei corrispondenti nella risposta al protocollo");
                                    }
                                }
                            }
                            else
                            {
                                //avviso l'utente che non ha selezionato nessun corrispondente
                                Page.RegisterStartupScript("alert", "<SCRIPT>alert('ATTENZIONE: selezionare un corrispondente dalla lista');</SCRIPT>");
                            }
                        }
                    }
                    else
                    {
                        //avviso l'utente che non ha selezionato nessun corrispondente
                        Page.RegisterStartupScript("alert", "<SCRIPT>alert('ATTENZIONE: selezionare un corrispondente dalla lista');</SCRIPT>");
                    }
                }
                else
                {
                    //avviso l'utente che non ha selezionato nulla
                    Page.RegisterStartupScript("alert", "<SCRIPT>alert('ATTENZIONE: selezionare un documento');</SCRIPT>");
                }
            }
            #endregion

            #region PROTOCOLLO ARRIVO
            if (rbl_TipoDoc.Items.FindByValue("Arrivo").Selected)
            {
                avanzaDoc = verificaSelezioneDocumento(out itemIndex);
                if (avanzaDoc)
                {
                    bool mittenteOK = false;
                    bool oggettoOK = false;
                    string oggettoProtoSel = "";

                    ArrayList listDest = new ArrayList();
                    //prendo il mittente del documento selezionato
                    DocsPaWR.Corrispondente CorrMitt = null;

                    this.infoDoc = RicercaDocumentiSessionMng.GetListaInfoDocumenti(this);

                    if (itemIndex > -1)
                    {
                        infoDocSel = (DocsPAWA.DocsPaWR.InfoDocumento)this.infoDoc[itemIndex];
                        DocumentManager.setInfoDocumento(this, infoDocSel);
                    }

                    if (infoDocSel != null)
                    {
                        //prendo il dettaglio del documento e estraggo il mittente del protocollo
                        DocsPaWR.SchedaDocumento schedaDocIn = new DocsPAWA.DocsPaWR.SchedaDocumento();
                        schedaDocIn = DocumentManager.getDettaglioDocumento(this, infoDocSel.idProfile, infoDocSel.docNumber);
                        //prendo il mittente
                        CorrMitt = ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIn.protocollo).mittente;
                        listDest.Add(CorrMitt);

                    }
                    #region CHIARIMENTO
                    /*	CASI POSSIBILI PER RISPONDERE A UN PROTOCOLLO	
							*			----------------------------------------------------------------------------------
							*			caso 1 - il documento in uscita (il protocollo di risposta) non è protocollato
							*						
							*					caso 1.1 - il campo destinatario viene popolato con il mittente 
							*		                    del documento selezionato
							*					
							*					
							*			
							*			Analogo il discorso per il campo oggetto, se sono diversi avviso l'utente e gli
							*			daremo la possibilità di scegliere se proseguire o meno con l'operazione
							*			di collegamento	
							* 		   
							*			-----------------------------------------------------------------------------------
							*			caso 2 - il documento in uscita è già protocollato
							*					
							*					caso 2.1 - destinatario scelto è diverso da quello corrente, si avvisa l'utente
							*		     				   e si da la possibilità si proseguire o meno con il collegamento
							* 
							*			Analogo il discorso per il campo oggetto
							*			-----------------------------------------------------------------------------------
							* */
                    #endregion

                    //prendo la scheda documento corrente in sessione
                    schedaDocUscita = DocumentManager.getDocumentoInLavorazione(this);

                    //Gestione corrispondenti nel caso di Corrispondenti extra AOO
                    if (UserManager.isFiltroAooEnabled(this))
                    {

                        if (schedaDocUscita != null)
                        {
                            DocsPAWA.DocsPaWR.Registro tempRegUser = UserManager.getRegistroSelezionato(this);
                            infoDocSel = DocumentManager.getInfoDocumento(this);
                            if (tempRegUser.systemId != infoDocSel.idRegistro && !string.IsNullOrEmpty(CorrMitt.idRegistro))
                            {
                                DocsPAWA.DocsPaWR.Corrispondente tempDest = CorrMitt;
                                tempDest.codiceRubrica = null;
                                tempDest.idOld = "0";
                                tempDest.systemId = null;
                                tempDest.tipoCorrispondente = "O";
                                tempDest.tipoIE = null;
                                tempDest.idRegistro = tempRegUser.systemId;
                                tempDest.idAmministrazione = UserManager.getUtente(this).idAmministrazione;
                                diventaOccasionale = false;
                                CorrMitt = tempDest;
                            }
                        }
                    }

                    if (schedaDocUscita != null)
                    {
                        #region GESTIONE RISPOSTA AL PROTOCOLLO NON ANCORA PROTOCOLLATA
                        //se non è protocollato
                        if (schedaDocUscita.protocollo != null && (schedaDocUscita.protocollo.numero == null || schedaDocUscita.protocollo.numero.Equals("")))
                        {
                            if (!schedaDocUscita.tipoProto.Equals("A"))
                            {
                                if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari == null)
                                {
                                    mittenteOK = true;

                                }
                                else
                                {
                                    //aggiungo il mittente ai destinatari già presenti
                                    DocsPaWR.Corrispondente[] corr = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari;
                                    foreach (DocsPaWR.Corrispondente c in corr)
                                    {
                                        listDest.Add(c);
                                    }

                                    mittenteOK = true;
                                }
                            }
                            else
                            {
                                mittenteOK = true;
                            }

                            DocsPaWR.Corrispondente[] corrispondenti = new DocsPAWA.DocsPaWR.Corrispondente[listDest.Count];
                            listDest.CopyTo(corrispondenti);
                            RicercaDocumentiSessionMng.setCorrispondenteRispostaINGRESSO(this, corrispondenti);

                            if (itemIndex >= 0)
                            {
                                oggettoProtoSel = ((Label)DataGrid1.Items[itemIndex].Cells[2].Controls[1]).Text;
                            }
                            //inizio verifica congruenza campo oggetto
                            if (schedaDocUscita.oggetto != null && schedaDocUscita.oggetto.descrizione != null && schedaDocUscita.oggetto.descrizione != String.Empty)
                            {
                                if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                {
                                    if (schedaDocUscita.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                    {
                                        oggettoOK = true;
                                    }
                                }
                            }
                            else
                            {

                                oggettoOK = true;
                                if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                {
                                    DocsPaWR.Oggetto ogg = new DocsPAWA.DocsPaWR.Oggetto();
                                    ogg.descrizione = oggettoProtoSel.ToString();
                                    schedaDocUscita.oggetto = ogg;
                                }
                            }

                            if (!mittenteOK || !oggettoOK || !diventaOccasionale)
                            {
                                //se i corrisp non coincidono si lancia un avviso	all'utente 
                                if (!this.Page.IsStartupScriptRegistered("avvisoModale"))
                                {
                                    string scriptString = "<SCRIPT>OpenAvvisoModale('" + mittenteOK + "' , '" + oggettoOK + "' , 'False' , '" + diventaOccasionale + "');</SCRIPT>";
                                    this.Page.RegisterStartupScript("avvisoModale", scriptString);
                                }
                            }
                            else
                            {

                                schedaDocUscita.rispostaDocumento = infoDocSel;
                                schedaDocUscita.modificaRispostaDocumento = true;

                                if (!schedaDocUscita.tipoProto.Equals("A"))
                                {
                                    if (RicercaDocumentiSessionMng.getCorrispondenteRispostaINGRESSO(this) != null)
                                    {
                                        schedaDocUscita = CopiaCorrispondenti(schedaDocUscita);
                                        //((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari = RicercaProtocolliIngressoSessionMng.getCorrispondenteRisposta(this);
                                    }
                                }

                                if (schedaDocUscita.tipoProto.Equals("P"))
                                {
                                    DocsPaWR.Corrispondente[] corrispondentiTemp = new DocsPAWA.DocsPaWR.Corrispondente[1];
                                    corrispondentiTemp[0] = CorrMitt;
                                    ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari = corrispondentiTemp;
                                }

                                if (schedaDocUscita.tipoProto.Equals("I"))
                                {
                                    DocsPaWR.Corrispondente[] corrispondentiTemp = new DocsPAWA.DocsPaWR.Corrispondente[1];
                                    corrispondentiTemp[0] = CorrMitt;
                                    ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDocUscita.protocollo).destinatari = corrispondentiTemp;
                                }

                                DocumentManager.setDocumentoSelezionato(this, schedaDocUscita);

                                RicercaDocumentiSessionMng.SetDialogReturnValue(this, true);

                                Page.RegisterStartupScript("", "<script>window.close();</script>");
                            }
                        }
                    }


                        #endregion

                    #region GESTIONE RISPOSTA AL PROTOCOLLO GIA' PRECEDENTEMENTE PROTOCOLLATA

                    if (schedaDocUscita.protocollo != null && schedaDocUscita.protocollo.numero != null && schedaDocUscita.protocollo.numero != String.Empty)
                    {
                        DocsPaWR.Corrispondente[] corrispondenti = new DocsPAWA.DocsPaWR.Corrispondente[listDest.Count];
                        listDest.CopyTo(corrispondenti);


                        bool verificaUguaglianzaCorr = true;

                        if (schedaDocUscita.tipoProto.Equals("P"))
                            verificaUguaglianzaCorr = verificaUguaglianzacorrispondenti(corrispondenti, ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari);
                        if (schedaDocUscita.tipoProto.Equals("A"))
                            verificaUguaglianzaCorr = verificaUguaglianzacorrispondenti(CorrMitt, ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocUscita.protocollo).mittente);

                        if (verificaUguaglianzaCorr)
                        {
                            mittenteOK = true;
                        }
                        if (this.DataGrid1.SelectedIndex >= 0)
                        {
                            oggettoProtoSel = ((Label)DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[2].Controls[1]).Text;
                        }
                        //inizio verifica congruenza campo oggetto
                        if (schedaDocUscita.oggetto != null && schedaDocUscita.oggetto.descrizione != null && schedaDocUscita.oggetto.descrizione != String.Empty)
                        {
                            if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                            {
                                if (schedaDocUscita.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                {
                                    oggettoOK = true;
                                }
                            }
                        }
                        if (!mittenteOK || !oggettoOK || !diventaOccasionale)
                        {
                            //se i corrisp non coincidono si lancia un avviso	all'utente 
                            if (!this.Page.IsStartupScriptRegistered("avvisoModale"))
                            {
                                string scriptString = "<SCRIPT>OpenAvvisoModale('" + mittenteOK + "' , '" + oggettoOK + "' , 'True' , '" + diventaOccasionale + "');</SCRIPT>";
                                this.Page.RegisterStartupScript("avvisoModale", scriptString);
                            }
                        }
                        else
                        {
                            infoDocSel = DocumentManager.getInfoDocumento(this);
                            if (infoDocSel != null)
                            {
                                schedaDocUscita.rispostaDocumento = infoDocSel;
                                schedaDocUscita.modificaRispostaDocumento = true;
                            }


                            DocumentManager.setDocumentoSelezionato(this, schedaDocUscita);
                            Page.RegisterStartupScript("", "<script>window.close();</script>");
                        }
                    }
                    #endregion

                    #region GESTIONE RISPOSTA AD UN DOCUMENTO NON PROTOCOLLATO

                    if (this.callPage.Equals("profilo"))
                    {
                        if (this.DataGrid1.SelectedIndex >= 0)
                        {
                            oggettoProtoSel = ((Label)DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[2].Controls[1]).Text;
                        }
                        //inizio verifica congruenza campo oggetto
                        if (schedaDocUscita.docNumber != null)
                        {
                            if (schedaDocUscita.oggetto != null && schedaDocUscita.oggetto.descrizione != null && schedaDocUscita.oggetto.descrizione != String.Empty)
                            {
                                if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                {
                                    if (schedaDocUscita.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                    {
                                        oggettoOK = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            oggettoOK = true;
                            schedaDocUscita.oggetto.descrizione = oggettoProtoSel;
                        }
                        if (!oggettoOK)
                        {
                            //se i corrisp non coincidono si lancia un avviso	all'utente 
                            if (!this.Page.IsStartupScriptRegistered("avvisoModale"))
                            {
                                string scriptString = "";
                                if (!infoDocSel.segnatura.Equals(""))
                                {
                                    scriptString = "<SCRIPT>OpenAvvisoModale('" + null + "' , '" + oggettoOK + "' , 'False' , '" + diventaOccasionale + "');</SCRIPT>";
                                }
                                else
                                {
                                    scriptString = "<SCRIPT>OpenAvvisoModale('" + null + "' , '" + oggettoOK + "' , 'True' , '" + diventaOccasionale + "');</SCRIPT>";
                                }
                                this.Page.RegisterStartupScript("avvisoModale", scriptString);
                            }
                        }
                        else
                        {
                            infoDocSel = DocumentManager.getInfoDocumento(this);
                            if (infoDocSel != null)
                            {
                                schedaDocUscita.rispostaDocumento = infoDocSel;
                                schedaDocUscita.modificaRispostaDocumento = true;
                            }


                            DocumentManager.setDocumentoSelezionato(this, schedaDocUscita);
                            Page.RegisterStartupScript("", "<script>window.close();</script>");
                        }
                    }

                    #endregion
                }
                else
                {
                    //avviso l'utente che non ha selezionato nulla
                    Page.RegisterStartupScript("alert", "<SCRIPT>alert('ATTENZIONE: selezionare un documento');</SCRIPT>");
                }
            }
            #endregion

            #region DOCUMENTI NON PROTOCOLLATI
            if (rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected || rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
            {
                if (verificaSelezioneDocumento(out itemIndex))
                {
                    //prendo la scheda documento corrente in sessione
                    schedaDoc = DocumentManager.getDocumentoInLavorazione(this);
                    if (schedaDoc != null)
                    {
                        this.infoDoc = RicercaDocumentiSessionMng.GetListaInfoDocumenti(this);
                        if (itemIndex > -1)
                        {
                            infoDocSel = (DocsPAWA.DocsPaWR.InfoDocumento)this.infoDoc[itemIndex];
                            DocumentManager.setInfoDocumento(this, infoDocSel);
                        }
                        infoDocSel = DocumentManager.getInfoDocumento(this);
                        // infoDocSel.oggetto = schedaDoc.oggetto.descrizione;
                        infoDocSel.isCatenaTrasversale = "1";
                        if (infoDocSel != null)
                        {
                            schedaDoc.rispostaDocumento = infoDocSel;
                            schedaDoc.modificaRispostaDocumento = true;
                            if (infoDocSel.oggetto != null && schedaDoc.oggetto.descrizione == null)
                            {
                                schedaDoc.oggetto.descrizione = infoDocSel.oggetto;
                            }
                        }
                        DocumentManager.setDocumentoSelezionato(this, schedaDoc);
                        RicercaDocumentiSessionMng.SetDialogReturnValue(this, true);
                        Page.RegisterStartupScript("", "<script>window.close();</script>");
                    }

                }
                else
                {
                    //avviso l'utente che non ha selezionato nulla
                    Page.RegisterStartupScript("alert", "<SCRIPT>alert('ATTENZIONE: selezionare un documento');</SCRIPT>");
                }
            }
            #endregion

            #region PROTOCOLLO INTERNO

            if (rbl_TipoDoc.Items.FindByValue("Interno") != null)
            {
                if (rbl_TipoDoc.Items.FindByValue("Interno").Selected)
                {
                     DocsPaWR.InfoDocumento infoDocumentoLav = DocumentManager.getInfoDocumento(DocumentManager.getDocumentoSelezionato(this));

                     if (this.callPage.Equals("profilo") || infoDocumentoLav.tipoProto.Equals("I") || infoDocumentoLav.tipoProto.Equals("R") || infoDocumentoLav.tipoProto.Equals("C") || ((infoDocumentoLav.tipoProto.Equals("P") || infoDocumentoLav.tipoProto.Equals("A")) && infoDocumentoLav.docNumber != null))
                    {
                        if (verificaSelezioneDocumento(out itemIndex))
                        {
                            //prendo la scheda documento corrente in sessione
                            schedaDoc = DocumentManager.getDocumentoInLavorazione(this);
                            if (schedaDoc != null)
                            {
                                this.infoDoc = RicercaDocumentiSessionMng.GetListaInfoDocumenti(this);
                                if (itemIndex > -1)
                                {
                                    infoDocSel = (DocsPAWA.DocsPaWR.InfoDocumento)this.infoDoc[itemIndex];
                                    DocumentManager.setInfoDocumento(this, infoDocSel);
                                }
                                infoDocSel = DocumentManager.getInfoDocumento(this);
                                // infoDocSel.oggetto = schedaDoc.oggetto.descrizione;
                                infoDocSel.isCatenaTrasversale = "1";
                                if (infoDocSel != null)
                                {
                                    if (infoDocumentoLav.tipoProto.Equals("I"))
                                    {
                                        if (schedaDoc.protocollo != null && !string.IsNullOrEmpty(schedaDoc.protocollo.segnatura))
                                        {
                                            //CASO IN CUI IL PROTOCOLLO DI PARTENZA E' PROTOCOLLATO (POSSIBILE QUALCOSA IN SEGUITO VERRA' AGGIUNTO
                                        }
                                        else
                                        {
                                            DocsPAWA.DocsPaWR.SchedaDocumento intDoc = DocumentManager.getDettaglioDocumento(this, infoDocSel.idProfile, infoDocSel.docNumber);

                                            //PRENDO IL MITTENTE DEL PROTOCOLLO INTERNO DI PARTENZA
                                            DocsPAWA.DocsPaWR.Corrispondente corrispondenteMitt = ((DocsPAWA.DocsPaWR.ProtocolloInterno)intDoc.protocollo).mittente;

                                            DocsPAWA.DocsPaWR.Corrispondente corrispondenteMittAttuale = null;

                                            //PRENDO IL MITTENTE DEL PROTOCOLLO INTERNO DI NUOVA CREAZIONE
                                            if(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).mittente!=null)
                                            {
                                                corrispondenteMittAttuale = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).mittente;
                                            }

                                            //UNISCO I DESTINATARI DEL PROTOCOLLO DI PARTENZA CON QUELLO CONCATENATO SOLO SE NON PRESENTI
                                            if (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari != null)
                                            {
                                                foreach (DocsPAWA.DocsPaWR.Corrispondente corr in ((DocsPAWA.DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatari)
                                                {
                                                    if (!UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari, corr))
                                                    {
                                                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari = UserManager.addCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari, corr);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari = ((DocsPAWA.DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatari;
                                            }

                                            //AGGIUNGO IL MITTENTE AI DESTINATARI SE NON PRESENTE
                                            if (!UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatari, corrispondenteMitt))
                                            {
                                                ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari = UserManager.addCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari, corrispondenteMitt);
                                            }

                                            int cancellaDest = -1;

                                            if (corrispondenteMittAttuale != null)
                                            {

                                                for (int i = 0; i < (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari).Length; i++)
                                                {
                                                    DocsPAWA.DocsPaWR.Corrispondente temp = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari[i];
                                                    if (temp.systemId.Equals(corrispondenteMittAttuale.systemId))
                                                    {
                                                        cancellaDest = i;
                                                        break;
                                                    }
                                                }

                                                if (cancellaDest != -1)
                                                {
                                                    //SE IL MITTENTE è presente nei destinatari lo elimino
                                                    ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari = UserManager.removeCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari, cancellaDest);
                                                }
                                            }

                                            if (((DocsPAWA.DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatariConoscenza != null && ((DocsPAWA.DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatariConoscenza.Length > 0)
                                            {
                                                // ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = ((DocsPAWA.DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatariConoscenza;

                                                foreach (DocsPAWA.DocsPaWR.Corrispondente corr in ((DocsPAWA.DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatariConoscenza)
                                                {
                                                    if (!UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari, corr) && !UserManager.esisteCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza, corr))
                                                    {
                                                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = UserManager.addCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza, corr);
                                                    }
                                                }

                                                cancellaDest = -1;

                                                for (int i = 0; i < (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza).Length; i++)
                                                {
                                                    DocsPAWA.DocsPaWR.Corrispondente temp = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza[i];
                                                    if (temp.systemId.Equals(corrispondenteMitt.systemId))
                                                    {
                                                        cancellaDest = i;
                                                        break;
                                                    }
                                                }

                                                if (cancellaDest != -1)
                                                {
                                                    //SE IL MITTENTE ATTUALE è presente nei destinatari lo elimino
                                                    ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = UserManager.removeCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza, cancellaDest);
                                                }

                                                if (corrispondenteMittAttuale != null)
                                                {
                                                    cancellaDest = -1;

                                                    for (int i = 0; i < (((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza).Length; i++)
                                                    {
                                                        DocsPAWA.DocsPaWR.Corrispondente temp = ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza[i];
                                                        if (temp.systemId.Equals(corrispondenteMittAttuale.systemId))
                                                        {
                                                            cancellaDest = i;
                                                            break;
                                                        }
                                                    }

                                                    if (cancellaDest != -1)
                                                    {
                                                        //SE IL MITTENTE è presente nei destinatari lo elimino
                                                        ((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = UserManager.removeCorrispondente(((DocsPAWA.DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza, cancellaDest);
                                                    }
                                                }

                                            }

                                        }

                                        schedaDoc.rispostaDocumento = infoDocSel;
                                        schedaDoc.modificaRispostaDocumento = true;
                                        if (infoDocSel.oggetto != null && schedaDoc.oggetto.descrizione == null)
                                        {
                                            schedaDoc.oggetto.descrizione = infoDocSel.oggetto;
                                        }
                                    }
                                }
                                DocumentManager.setDocumentoSelezionato(this, schedaDoc);
                                RicercaDocumentiSessionMng.SetDialogReturnValue(this, true);
                                Page.RegisterStartupScript("", "<script>window.close();</script>");
                            }

                        }
                        else
                        {
                            //avviso l'utente che non ha selezionato nulla
                            Page.RegisterStartupScript("alert", "<SCRIPT>alert('ATTENZIONE: selezionare un documento');</SCRIPT>");
                        }
                    }
                    //FINE PARTE DA DOCUMENTO GRIGIO
                    else
                    {
                        //RICHIAMATO DA PROTOCOLLO
                        avanzaDoc = verificaSelezioneDocumento();
                        if (avanzaDoc)
                        {
                            bool avanzaCor = verificaSelezione(out itemIndex);
                            if (avanzaCor)
                            {
                                bool mittenteOK = false;
                                bool oggettoOK = false;
                                string oggettoProtoSel = "";
                                //ricavo la system_id del corrispondente selezionato, contenuta nella prima colonna del datagrid
                                string key = dg_lista_corr.Items[itemIndex].Cells[0].Text;

                                //prendo la hashTable che contiene i corrisp dalla sesisone
                                ht_destinatariTO_CC = DocumentManager.getHash(this);

                                if (ht_destinatariTO_CC != null)
                                {
                                    if (ht_destinatariTO_CC.ContainsKey(key))
                                    {
                                        //prendo il corrispondente dalla hashTable secondo quanto è stato richiesto dall'utente
                                        destSelected = (DocsPAWA.DocsPaWR.Corrispondente)ht_destinatariTO_CC[key];
                                        //prendo la scheda documento corrente in sessione
                                        schedaDocIngresso = DocumentManager.getDocumentoInLavorazione(this);

                                        //Gestione corrispondenti nel caso di Corrispondenti extra AOO
                                        if (UserManager.isFiltroAooEnabled(this))
                                        {

                                            if (schedaDocIngresso != null)
                                            {
                                                DocsPAWA.DocsPaWR.Registro tempRegUser = UserManager.getRegistroSelezionato(this);
                                                infoDocSel = DocumentManager.getInfoDocumento(this);
                                                if (tempRegUser.systemId != infoDocSel.idRegistro && !string.IsNullOrEmpty(destSelected.idRegistro))
                                                {
                                                    DocsPAWA.DocsPaWR.Corrispondente tempDest = destSelected;
                                                    tempDest.codiceRubrica = null;
                                                    tempDest.idOld = "0";
                                                    tempDest.systemId = null;
                                                    tempDest.tipoCorrispondente = "O";
                                                    tempDest.tipoIE = null;
                                                    tempDest.idRegistro = tempRegUser.systemId;
                                                    tempDest.idAmministrazione = UserManager.getUtente(this).idAmministrazione;
                                                    diventaOccasionale = false;
                                                    destSelected = tempDest;
                                                }
                                            }
                                        }

                                        if (schedaDocIngresso != null)
                                        {
                                            #region GESTIONE RISPOSTA AL PROTOCOLLO NON ANCORA PROTOCOLLATA
                                            //se non è protocollato
                                            if (schedaDocIngresso.protocollo != null && (schedaDocIngresso.protocollo.numero == null || schedaDocIngresso.protocollo.numero.Equals("")))
                                            {
                                                if (!this.schedaDocIngresso.tipoProto.Equals("P") && !this.schedaDocIngresso.tipoProto.Equals("I"))
                                                {
                                                    // il campo mittente della scheda documento relativa al protocollo di risposta non è popolato
                                                    if (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente == null)
                                                    {
                                                        mittenteOK = true;
                                                        //popolo il campo mittente con il destinatario selezionato dal protocollo a cui ri risponde 
                                                        //((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = destSelected;
                                                        RicercaDocumentiSessionMng.setCorrispondenteRispostaUSCITA(this, destSelected);
                                                    }
                                                    else
                                                    {
                                                        //il campo mittente è stato popolato prima di selezionere il protocollo a cui rispondere,
                                                        //quindi si verifica se i corrispondenti coincidono. In caso affermativo si procede con il collegamento,
                                                        //in caso contrario avviso l'utente, dando la possibilità di scegliere se proseguire con i nuovi dati o meno.

                                                        //per i mittenti occasionali come si fa ?
                                                        bool verificaUguaglianzaCorr = verificaUguaglianzacorrispondenti(destSelected, ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente);
                                                        if (verificaUguaglianzaCorr)
                                                        {
                                                            mittenteOK = true;
                                                        }
                                                        else
                                                        {
                                                            //setto il destinatario selezionato in sessione
                                                            RicercaDocumentiSessionMng.setCorrispondenteRispostaUSCITA(this, destSelected);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    mittenteOK = true;
                                                }

                                                if (this.DataGrid1.SelectedIndex >= 0)
                                                {
                                                    oggettoProtoSel = ((Label)DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[2].Controls[1]).Text;
                                                }
                                                //inizio verifica congruenza campo oggetto
                                                if (schedaDocIngresso.oggetto != null && schedaDocIngresso.oggetto.descrizione != null && schedaDocIngresso.oggetto.descrizione != String.Empty)
                                                {
                                                    if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                                    {
                                                        if (schedaDocIngresso.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                                        {
                                                            oggettoOK = true;
                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                    oggettoOK = true;
                                                    if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                                    {
                                                        DocsPaWR.Oggetto ogg = new DocsPAWA.DocsPaWR.Oggetto();
                                                        ogg.descrizione = oggettoProtoSel.ToString();
                                                        schedaDocIngresso.oggetto = ogg;
                                                    }
                                                }

                                                infoDocSel = DocumentManager.getInfoDocumento(this);
                                                if (infoDocSel != null)
                                                {
                                                    schedaDocIngresso.rispostaDocumento = infoDocSel;
                                                    schedaDocIngresso.modificaRispostaDocumento = true;
                                                }

                                                if (schedaDocIngresso.tipoProto.Equals("A"))
                                                {
                                                    if (RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this) != null)
                                                    {
                                                        ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = destSelected;
                                                    }
                                                }

                                                if (schedaDocIngresso.tipoProto.Equals("P"))
                                                {
                                                   // if (RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this) != null)
                                                   // {
                                                        ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocIngresso.protocollo).mittente = destSelected;
                                                    //}
                                                }

                                                DocumentManager.setDocumentoSelezionato(this, schedaDocIngresso);

                                                RicercaDocumentiSessionMng.SetDialogReturnValue(this, true);

                                                Page.RegisterStartupScript("", "<script>window.close();</script>");
                                            }
                                        }


                                            #endregion

                                        #region GESTIONE RISPOSTA AL PROTOCOLLO GIA' PRECEDENTEMENTE PROTOCOLLATA

                                        if (schedaDocIngresso.protocollo != null && schedaDocIngresso.protocollo.numero != null && schedaDocIngresso.protocollo.numero != String.Empty)
                                        {
                                            bool verificaUguaglianzaCorr;
                                            //INSERITO PER BUG SE DA UN PROTOCOLLO INTERNO CERCO UNO IN PARTENZA
                                            if (schedaDocIngresso.tipoProto.Equals("I"))
                                            {
                                                verificaUguaglianzaCorr = false;
                                            }
                                            else
                                            {
                                                verificaUguaglianzaCorr = verificaUguaglianzacorrispondenti(destSelected, ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente);
                                            }
                                            if (verificaUguaglianzaCorr)
                                            {
                                                mittenteOK = true;
                                            }
                                            if (this.DataGrid1.SelectedIndex >= 0)
                                            {
                                                oggettoProtoSel = ((Label)DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[2].Controls[1]).Text;
                                            }
                                            //inizio verifica congruenza campo oggetto
                                            if (schedaDocIngresso.oggetto != null && schedaDocIngresso.oggetto.descrizione != null && schedaDocIngresso.oggetto.descrizione != String.Empty)
                                            {
                                                if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                                {
                                                    if (schedaDocIngresso.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                                    {
                                                        oggettoOK = true;
                                                    }
                                                }
                                            }
                                            if (!mittenteOK || !oggettoOK || !diventaOccasionale)
                                            {
                                                //se i corrisp non coincidono si lancia un avviso	all'utente 
                                                if (!this.Page.IsStartupScriptRegistered("avvisoModale"))
                                                {
                                                    string scriptString = "<SCRIPT>OpenAvvisoModale('" + mittenteOK + "' , '" + oggettoOK + "' , 'True' , '" + diventaOccasionale + "');</SCRIPT>";
                                                    this.Page.RegisterStartupScript("avvisoModale", scriptString);
                                                }
                                            }
                                            else
                                            {
                                                infoDocSel = DocumentManager.getInfoDocumento(this);
                                                if (infoDocSel != null)
                                                {
                                                    schedaDocIngresso.rispostaDocumento = infoDocSel;
                                                    schedaDocIngresso.modificaRispostaDocumento = true;
                                                }


                                                DocumentManager.setDocumentoSelezionato(this, schedaDocIngresso);
                                                Page.RegisterStartupScript("", "<script>window.close();</script>");
                                            }
                                        #endregion
                                        }
                                    }
                                    else
                                    {
                                        //se entro qui è perchè si è verificato errore
                                        throw new Exception("Errore nella gestione dei corrispondenti nella risposta al protocollo");
                                    }
                                }

                            }
                            else
                            {
                                //avviso l'utente che non ha selezionato nessun corrispondente
                                Page.RegisterStartupScript("alert", "<SCRIPT>alert('ATTENZIONE: selezionare un corrispondente dalla lista');</SCRIPT>");
                            }
                        }
                        else
                        {
                            //avviso l'utente che non ha selezionato nulla
                            Page.RegisterStartupScript("alert", "<SCRIPT>alert('ATTENZIONE: selezionare un documento');</SCRIPT>");
                        }
                    }
                }
            }
            #endregion
        }

        private string GestioneAvvisoModale(string valore)
        {
            string retValue = string.Empty;
            try
            {
                //prendo il protocollo in ingresso in sessione
                schedaDocIngresso = DocumentManager.getDocumentoInLavorazione(this);
                //prendo il protocollo in uscita selezionato dal datagrid
                infoDocSel = DocumentManager.getInfoDocumento(this);
                retValue = valore;
                switch (valore)
                {
                    case "Y": //Gestione pulsante SI, CONTINUA

                        /* Alla pressione del pulsante CONTINUA l'utente vuole proseguire il 
                         * collegamento nonostante oggetto e corrispondente dei due protocolli siano diversi */

                        if (schedaDocIngresso != null && schedaDocIngresso.protocollo != null)
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocIngresso.rispostaDocumento = infoDocSel;
                                schedaDocIngresso.modificaRispostaDocumento = true;
                            }

                            this.hd_returnValueModal.Value = "";

                            //QUI CRASHA PERCHé MITTENTE è null
                         //   if (((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente == null)
                         //   {
                                if (RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this) != null)
                                {
                                    ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this);
                          //      }
                            }
                            DocumentManager.setDocumentoSelezionato(this, schedaDocIngresso);
                            //metto in sessione la scheda documento con le informazioni del protocollo a cui risponde

                            Page.RegisterStartupScript("", "<script>window.close();</script>");
                        }

                        //Per la concatenazione con i grigi

                        if (schedaDocIngresso != null && (schedaDocIngresso.tipoProto.Equals("G") || schedaDocIngresso.tipoProto.Equals("NP")))
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocIngresso.rispostaDocumento = infoDocSel;
                                schedaDocIngresso.modificaRispostaDocumento = true;
                            }

                            this.hd_returnValueModal.Value = "";

                           
                            DocumentManager.setDocumentoSelezionato(this, schedaDocIngresso);
                            //metto in sessione la scheda documento con le informazioni del protocollo a cui risponde

                            Page.RegisterStartupScript("", "<script>window.close();</script>");
                        }

                        //FINE CONCATENAZIONE CON I GRIGI

                        break;

                    case "N":

                        /* Alla pressione del pulsante NO, RESETTA l'utente vuole proseguire il collegamento 
                         * con i dati che ha digitato sulla pagina di protocollo */

                        //Per la concatenazione con i grigi

                        if(schedaDocIngresso != null && (schedaDocIngresso.tipoProto.Equals("G") || schedaDocIngresso.tipoProto.Equals("NP")))
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocIngresso.rispostaDocumento = infoDocSel;
                                schedaDocIngresso.modificaRispostaDocumento = true;
                            }

                            if (schedaDocIngresso.oggetto != null)
                            {
                                schedaDocIngresso.oggetto.descrizione = infoDocSel.oggetto.ToString();
                            }
                            else
                            {
                                DocsPaWR.Oggetto ogg = new DocsPAWA.DocsPaWR.Oggetto();
                                ogg.descrizione = infoDocSel.oggetto.ToString();
                                schedaDocIngresso.oggetto = ogg;
                            }

                            //metto in sessione la scheda documento con le informazioni del protocollo a cui risponde
                            DocumentManager.setDocumentoSelezionato(this, schedaDocIngresso);
                            Page.RegisterStartupScript("", "<script>window.close();</script>");

                            this.hd_returnValueModal.Value = "";
                        }

                        //FINE GESTIONE DOCUMENTI CON RISPOSTA

                        if (schedaDocIngresso != null && schedaDocIngresso.protocollo != null)
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocIngresso.rispostaDocumento = infoDocSel;
                                schedaDocIngresso.modificaRispostaDocumento = true;
                            }

                            if (schedaDocIngresso.oggetto != null)
                            {
                                schedaDocIngresso.oggetto.descrizione = infoDocSel.oggetto.ToString();
                            }
                            else
                            {
                                DocsPaWR.Oggetto ogg = new DocsPAWA.DocsPaWR.Oggetto();
                                ogg.descrizione = infoDocSel.oggetto.ToString();
                                schedaDocIngresso.oggetto = ogg;
                            }

                            if (RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this) != null)
                            {
                                ((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this);
                            }

                            //	popolo il campo mittente con il destinatario selezionato dal protocollo a cui ri risponde 
                            //((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = destSelected;

                            //metto in sessione la scheda documento con le informazioni del protocollo a cui risponde
                            DocumentManager.setDocumentoSelezionato(this, schedaDocIngresso);
                            Page.RegisterStartupScript("", "<script>window.close();</script>");

                            this.hd_returnValueModal.Value = "";
                        }

                        break;

                    case "S":
                        //non posso modificare il mittente o oggetto, quindi il pulsante continua
                        //si limiterà a popolare il campo risposta al protocollo con l'infoDoc corrente
                        if (schedaDocIngresso != null && schedaDocIngresso.protocollo != null)
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocIngresso.rispostaDocumento = infoDocSel;
                                schedaDocIngresso.modificaRispostaDocumento = true;
                            }
                        }
                        DocumentManager.setDocumentoSelezionato(this, schedaDocIngresso);
                        Page.RegisterStartupScript("", "<script>window.close();</script>");

                        break;
                }
            }
            catch
            {

            }

            return retValue;
        }

        private DocsPAWA.DocsPaWR.SchedaDocumento CopiaCorrispondenti(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocUscita)
        {
            //OCCHIO: DocsPAWA.DocsPaWR.Corrispondente[] listaCorrIng = RicercaDocumentiSessionMng.getCorrispondenteRisposta(this);
            DocsPAWA.DocsPaWR.Corrispondente[] listaCorrIng = null;
            if (RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this) != null)
            {
                listaCorrIng[0] = RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this);
                ArrayList listDest = new ArrayList();
                if (((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari != null)
                {
                    foreach (DocsPAWA.DocsPaWR.Corrispondente corrDocIng in listaCorrIng)
                    {
                        bool trovato = false;
                        foreach (DocsPAWA.DocsPaWR.Corrispondente corrDocUscita in ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari)
                        {
                            if (corrDocIng.systemId == corrDocUscita.systemId)
                                trovato = true;
                        }
                        if (!trovato)
                            listDest.Add(corrDocIng);
                    }
                    listDest.CopyTo(((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari);
                }
            }
            return schedaDocUscita;
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
        private bool verificaSelezioneDocumento()
        {
            bool verificaSelezione = false;

            if (DataGrid1.SelectedIndex >= 0)
                verificaSelezione = true;
            return verificaSelezione;
        }

        //VERIFICA SE è STATO SELEZIONATO ALMENO UNA OPZIONE (CORRISPONDENTE) NEL PANNELLO pnl_corr
        private bool verificaSelezioneDocumento(out int itemIndex)
        {
            bool verificaSelezione = false;
            itemIndex = -1;
            foreach (DataGridItem dgItem in this.DataGrid1.Items)
            {
                RadioButton optCorr = dgItem.Cells[4].FindControl("optCorr") as RadioButton;
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

        private bool verificaUguaglianzacorrispondenti(DocsPAWA.DocsPaWR.Corrispondente[] destSelected, DocsPAWA.DocsPaWR.Corrispondente[] destCorrenti)
        {
            bool verificaUguaglianza = false;

            foreach (DocsPAWA.DocsPaWR.Corrispondente dc in destCorrenti)
            {
                foreach (DocsPAWA.DocsPaWR.Corrispondente ds in destSelected)
                {
                    if (ds.systemId == dc.systemId)
                    {
                        verificaUguaglianza = true;
                    }
                }
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
                //e pulisce i campi
                clearSearchDoc("Protocollo");
                clearSearchDoc("Documento");
                this.ddl_numProto.Enabled = false;
                this.ddl_dtaProto.Enabled = false;
                this.GetCalendarControl("txtInitDtaProto").txt_Data.Enabled = false;
                this.GetCalendarControl("txtInitDtaProto").btn_Cal.Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").btn_Cal.Visible = false;
                this.txtInitNumProto.Enabled = false;
                this.txt_annoProto.Enabled = false;
                this.txtInitNumProto.Enabled = false;

                this.ddl_numDoc.Enabled = false;
                this.ddl_dtaDoc.Enabled = false;

                this.GetCalendarControl("txtInitDtaDoc").txt_Data.Enabled = false;
                this.GetCalendarControl("txtInitDtaDoc").btn_Cal.Visible = false;
                this.GetCalendarControl("txtEndDataDoc").Visible = false;
                this.GetCalendarControl("txtEndDataDoc").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataDoc").btn_Cal.Visible = false;
                this.txtInitDoc.Enabled = false;

                //GESTIONE CATENE EXTRA AOO
                if (UserManager.isFiltroAooEnabled(this))
                {
                    ddl_reg.Enabled = false;
                    if (ddl_reg.Items.Count > 0)
                    {
                        ddl_reg.SelectedIndex = 0;
                    }
                }
            }
            else
            {
                //se il checkBox non è spuntato

                //riabilito i filtri di ricerca
                this.ddl_numProto.Enabled = true;
                this.ddl_dtaProto.Enabled = true;
                this.txt_annoProto.Enabled = true;
                this.GetCalendarControl("txtInitDtaProto").txt_Data.Enabled = true;
                this.GetCalendarControl("txtInitDtaProto").btn_Cal.Visible = true;
                this.txtInitNumProto.Enabled = true;

                this.ddl_numDoc.Enabled = true;
                this.ddl_dtaDoc.Enabled = true;

                this.GetCalendarControl("txtInitDtaDoc").txt_Data.Enabled = true;
                this.GetCalendarControl("txtInitDtaDoc").btn_Cal.Visible = true;
                this.txtInitDoc.Enabled = true;

                //GESTIONE CATENE EXTRA AOO
                if (UserManager.isFiltroAooEnabled(this))
                {
                    ddl_reg.Enabled = true;
                }

                string s = "<SCRIPT language='javascript'>try{document.getElementById('" + txtInitNumProto.ID + "').focus();} catch(e){}</SCRIPT>";
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

        private void btn_find_Click(object sender, System.EventArgs e)
        {
            try
            {
                //impostazioni iniziali
                this.pnl_corr.Visible = false;
                this.DataGrid1.Visible = false;
                this.lbl_countRecord.Visible = false;
                //pulisce selezioni precedenti
                resetOption();
                RicercaDocumentiSessionMng.ClearSessionData(this);
                //fine impostazioni

                //INIZIO VALIDAZIONE DATI INPUT ALLA RICERCA	
			    
                //VALIDAZIONE PER DOCUMENTI NON PROTOCOLLATI E PREDISPOSTI
                if (rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected || rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
                {
                    if (txtInitDoc.Text != "")
                    {
                        if (IsValidNumber(txtInitDoc) == false)
                        {
                            Response.Write("<script>alert('Il numero di documento deve essere numerico!');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txtInitDoc.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return;
                        }
                    }
                    if (txtEndNumDoc.Text != "")
                    {
                        if (IsValidNumber(txtEndNumDoc) == false)
                        {
                            Response.Write("<script>alert('Il numero di documento deve essere numerico!');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txtEndNumDoc.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return;
                        }
                    }

                    if (this.ddl_dtaDoc.SelectedIndex == 1)
                    {
                        if (Utils.isDate(this.GetCalendarControl("txtInitDtaDoc").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txtEndDataDoc").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txtInitDtaDoc").txt_Data.Text, this.GetCalendarControl("txtEndDataDoc").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Verificare intervallo date !');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaDoc").txt_Data.ID + "').focus();</SCRIPT>";
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                            // Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return;
                        }
                    }

                    if (txtInitDoc.Text != "")
                    {
                        if (IsValidNumber(txtInitDoc) == false)
                        {
                            Response.Write("<script>alert('Il numero di documento deve essere numerico!');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txtInitDoc.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return;
                        }
                    }
                    if (txtEndNumDoc.Text != "")
                    {
                        if (IsValidNumber(txtEndNumDoc) == false)
                        {
                            Response.Write("<script>alert('Il numero di documento deve essere numerico!');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txtEndNumDoc.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return;
                        }
                    }

                    if (this.ddl_dtaDoc.SelectedIndex == 1)
                    {
                        if (Utils.isDate(this.GetCalendarControl("txtInitDtaDoc").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txtEndDataDoc").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txtInitDtaDoc").txt_Data.Text, this.GetCalendarControl("txtEndDataDoc").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Verificare intervallo date !');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaDoc").txt_Data.ID + "').focus();</SCRIPT>";
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                            // Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return;
                        }
                    }
                }
                //VALIDAZIONE PER PROTOCOLLI
                else
                {
                    if (txtInitNumProto.Text != "")
                    {
                        if (IsValidNumber(txtInitNumProto) == false)
                        {
                            Response.Write("<script>alert('Il numero di protocollo deve essere numerico!');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txtInitNumProto.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return;
                        }
                    }
                    if (txtEndNumProto.Text != "")
                    {
                        if (IsValidNumber(txtEndNumProto) == false)
                        {
                            Response.Write("<script>alert('Il numero di protocollo deve essere numerico!');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txtEndNumProto.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return;
                        }
                    }
                    //controllo validità anno inserito
                    if (txt_annoProto.Text != "")
                    {
                        if (IsValidYear(txt_annoProto.Text) == false)
                        {
                            Response.Write("<script>alert('Formato anno non corretto !');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_annoProto.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return;
                        }
                    }

                    if (this.ddl_dtaProto.SelectedIndex == 1)
                    {
                        if (Utils.isDate(this.GetCalendarControl("txtInitDtaProto").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txtInitDtaProto").txt_Data.Text, this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Verificare intervallo date !');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDtaProto").txt_Data.ID + "').focus();</SCRIPT>";
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "focus", s);
                            // Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return;
                        }
                    }

                    if (txtInitDoc.Text != "")
                    {
                        if (IsValidNumber(txtInitNumProto) == false)
                        {
                            Response.Write("<script>alert('Il numero di documento deve essere numerico!');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txtInitDoc.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return;
                        }
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


        #region classe per la gestione della sessione
        /// <summary>
        /// Classe per la gestione dei dati in sessione relativamente
        /// alla dialog "RicercaProtocolliUscita"
        /// </summary>
        public sealed class RicercaDocumentiSessionMng
        {
            private RicercaDocumentiSessionMng()
            {
            }

            /// <summary>
            /// Gestione rimozione dati in sessione
            /// </summary>
            /// <param name="page"></param>
            public static void ClearSessionData(Page page)
            {
                DocumentManager.removeFiltroRicDoc(page);
                DocumentManager.removeDataGridProtocolliIngresso(page);
                DocumentManager.removeDataGridProtocolliUscita(page);

                DocumentManager.removeInfoDocumento(page);
                DocumentManager.removeHash(page);

                RemoveListaInfoDocumenti(page);
                page.Session.Remove("RicercaDocumentiSessionMng.dialogReturnValue");
                removeCorrispondenteRispostaINGRESSO(page);
                removeCorrispondenteRispostaUSCITA(page);

            }

            public static void SetListaInfoDocumenti(Page page, DocsPaWR.InfoDocumento[] listaDocumenti)
            {
                page.Session["RicercaProtocolliUscita.ListaInfoDoc"] = listaDocumenti;
            }

            public static DocsPAWA.DocsPaWR.InfoDocumento[] GetListaInfoDocumenti(Page page)
            {
                return page.Session["RicercaProtocolliUscita.ListaInfoDoc"] as DocsPAWA.DocsPaWR.InfoDocumento[];
            }

            public static void RemoveListaInfoDocumenti(Page page)
            {
                page.Session.Remove("RicercaProtocolliUscita.ListaInfoDoc");
            }

            /// <summary>
            /// Impostazione flag booleano, se true, la dialog è stata caricata almeno una volta
            /// </summary>
            /// <param name="page"></param>
            public static void SetAsLoaded(Page page)
            {
                page.Session["RicercaDocumentiSessionMng.isLoaded"] = true;
            }

            /// <summary>
            /// Impostazione flag relativo al caricamento della dialog
            /// </summary>
            /// <param name="page"></param>
            public static void SetAsNotLoaded(Page page)
            {
                page.Session.Remove("RicercaDocumentiSessionMng.isLoaded");
            }

            /// <summary>
            /// Verifica se la dialog è stata caricata almeno una volta
            /// </summary>
            /// <param name="page"></param>
            /// <returns></returns>
            public static bool IsLoaded(Page page)
            {
                return (page.Session["RicercaDocumentiSessionMng.isLoaded"] != null);
            }

            /// <summary>
            /// Impostazione valore di ritorno
            /// </summary>
            /// <param name="page"></param>
            /// <param name="dialogReturnValue"></param>
            public static void SetDialogReturnValue(Page page, bool dialogReturnValue)
            {
                page.Session["RicercaDocumentiSessionMng.dialogReturnValue"] = dialogReturnValue;
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
                    retValue = Convert.ToBoolean(page.Session["RicercaDocumentiSessionMng.dialogReturnValue"]);

                page.Session.Remove("RicercaDocumentiSessionMng.isLoaded");

                return retValue;
            }

            /// <summary>
            /// Metodo per il settaggio in sessione del corrispondente selezionato per il protocollo di risposta
            /// </summary>
            /// <param name="page"></param>
            /// <param name="corrispondente"></param>
            public static void setCorrispondenteRispostaUSCITA(Page page, DocsPAWA.DocsPaWR.Corrispondente corrispondente)
            {
                page.Session["RicercaProtocolliUscita.corrispondenteRisposta"] = corrispondente;
            }

            public static DocsPAWA.DocsPaWR.Corrispondente getCorrispondenteRispostaUSCITA(Page page)
            {
                return (DocsPAWA.DocsPaWR.Corrispondente)page.Session["RicercaProtocolliUscita.corrispondenteRisposta"];
            }

            public static void removeCorrispondenteRispostaUSCITA(Page page)
            {
                page.Session.Remove("RicercaProtocolliUscita.corrispondenteRisposta");
            }


            public static void setCorrispondenteRispostaINGRESSO(Page page, DocsPAWA.DocsPaWR.Corrispondente[] corrispondente)
            {
                page.Session["RicercaProtocolliIngresso.corrispondenteRisposta"] = corrispondente;
            }

            public static DocsPAWA.DocsPaWR.Corrispondente[] getCorrispondenteRispostaINGRESSO(Page page)
            {
                return (DocsPAWA.DocsPaWR.Corrispondente[])page.Session["RicercaProtocolliIngresso.corrispondenteRisposta"];
            }

            public static void removeCorrispondenteRispostaINGRESSO(Page page)
            {
                page.Session.Remove("RicercaProtocolliIngresso.corrispondenteRisposta");
            }
        #endregion
        }



        #endregion

        protected void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            DocsPaWR.Corrispondente[] listaCorrTo = null;
            DocsPaWR.Corrispondente[] listaCorrCC = null;

            if (this.DataGrid1.SelectedIndex >= 0)
            {
                str_indexSel = ((Label)this.DataGrid1.Items[this.DataGrid1.SelectedIndex].Cells[7].Controls[1]).Text;
                int indexSel = Int32.Parse(str_indexSel);
                //this.infoDoc = (DocsPAWA.DocsPaWR.InfoDocumento[]) Session["RicercaProtocolliUscita.ListaInfoDoc"];
                this.infoDoc = RicercaDocumentiSessionMng.GetListaInfoDocumenti(this);

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

        protected void rbl_TipoDoc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            lbl_countRecord.Visible = false;
            DataGrid1.Visible = false;
            pnl_corr.Visible = false;
            string searchType = null;
            if (this.rbl_TipoDoc.Items.FindByValue("Arrivo").Selected || this.rbl_TipoDoc.Items.FindByValue("Partenza").Selected)
            {
                pnl_proto.Visible = true;
                pnl_doc.Visible = false;
                searchType = "Protocollo";
                clearSearchDoc(searchType);
            }
            if(this.rbl_TipoDoc.Items.FindByValue("Interno")!=null)
            {
                if (this.rbl_TipoDoc.Items.FindByValue("Interno").Selected)
                {
                    pnl_proto.Visible = true;
                    pnl_doc.Visible = false;
                    searchType = "Protocollo";
                    clearSearchDoc(searchType);
                }
            }
            if (this.rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected || this.rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
            {
                pnl_proto.Visible = false;
                pnl_doc.Visible = true;
                searchType = "Documento";
                clearSearchDoc(searchType);
            }
        }

        private void clearSearchDoc(string searchType)
        {
            if (searchType.Equals("Protocollo"))
            {
                txtInitDoc.Text = "";
                txtEndNumDoc.Text = "";
                txtInitDtaDoc.Text = "";
                txtEndDataDoc.Text = "";
                txtEndNumDoc.Visible = false;
                lblEndNumDoc.Visible = false;
                lblInitNumDoc.Visible = false;
                lblEndDataDoc.Visible = false;
                lblInitDtaDoc.Visible = false;
                ddl_numDoc.SelectedIndex = 0;

                ddl_dtaDoc.SelectedIndex = 0;
                txtEndDataDoc.Text = "";
                txtInitDtaDoc.Text = "";
                txtEndDataDoc.Visible = false;
                this.GetCalendarControl("txtEndDataDoc").Visible = false;
                this.GetCalendarControl("txtEndDataDoc").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataDoc").btn_Cal.Visible = false;
            }
            if (searchType.Equals("Documento"))
            {
                txtInitNumProto.Text = "";
                txtEndNumProto.Text = "";
                txt_annoProto.Text = "";
                txtInitDtaProto.Text = "";
                txtEndDataProtocollo.Text = "";
                ddl_numProto.SelectedIndex = 0;
                ddl_dtaProto.SelectedIndex = 0;
                lblEndNumProto.Visible = false;
                txtEndNumProto.Visible = false;
                lblInitNumProto.Visible = false;
                lblInitDtaProto.Visible = false;
                lblEndDataProtocollo.Visible = false;
                txtEndDataProtocollo.Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").txt_Data.Visible = false;
                this.GetCalendarControl("txtEndDataProtocollo").btn_Cal.Visible = false;
            }
            
        }

        private void getLettereProtocolli()
        {

            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
            string idAmm = cr.idAmministrazione;
            DocsPaWR.InfoUtente infoUt = new DocsPAWA.DocsPaWR.InfoUtente();
            infoUt = UserManager.getInfoUtente(this);
            this.etichette = wws.getEtichetteDocumenti(infoUt, idAmm);
            this.opArr.Text = etichette[0].Etichetta; //Valore A
            this.opPart.Text = etichette[1].Etichetta; //Valore P
            this.opInt.Text = etichette[2].Etichetta;//Valore I
           // this.opGrigio.Text = etichette[3].Etichetta;//Valore G
           // this.opAll.Text = etichette[4].Etichetta;
        }

        public void CaricaComboRegistri(DropDownList ddl)
        {
            userRegistri = UserManager.getListaRegistri(this);
            string stato;
            string inCondition = "IN ( ";
            string inConditionSimple = "";
            int elemento = 0;
            if (userRegistri.Length > 1)
            {
                for (int i = 0; i < userRegistri.Length; i++)
                {
                    stato = UserManager.getStatoRegistro(userRegistri[i]);
                    {
                        DocsPAWA.DocsPaWR.Registro registro = UserManager.getRegistroBySistemId(this.Page, userRegistri[i].systemId);
                        if (!registro.Sospeso)
                        {
                            ddl.Items.Add(userRegistri[i].codRegistro);
                            ddl.Items[elemento].Value = userRegistri[i].systemId;
                            elemento++;
                        }
                    }
                    inCondition = inCondition + userRegistri[i].systemId;
                    inConditionSimple = inConditionSimple + userRegistri[i].systemId;
                    if (i < userRegistri.Length - 1)
                    {
                        inCondition = inCondition + " , ";
                        inConditionSimple = inConditionSimple + " , ";
                    }
                }
                inCondition = inCondition + " )";
            }
            else
            {
                //è presente un solo registro quindi non faccio visualizzare la combo
                pnl_catene_extra_aoo.Visible = false;
            }
        }

        private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //mette in sessione il registro selezionato
            /*  if (ddl_registri.SelectedIndex != -1)
              {
                  if (userRegistri == null)
                      userRegistri = UserManager.getListaRegistri(this);
                  setStatoReg(UserManager.getRegistroBySistemId(this, ddl_registri.SelectedValue));
                  setRegistro(UserManager.getRegistroBySistemId(this, ddl_registri.SelectedValue));

                  this.Session["RegistroSelezionato"] = ddl_registri.SelectedValue.Trim();
              }*/
        }

    }



}
