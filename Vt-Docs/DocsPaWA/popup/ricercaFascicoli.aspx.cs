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
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{
    /// <summary>
    /// Summary description for ricercaFascicoli.
    /// </summary>
    public class ricercaFascicoli : DocsPAWA.CssPage
    {
        #region Variabili

        protected System.Web.UI.WebControls.Label ldl_apertA;
        protected System.Web.UI.WebControls.DropDownList ddl_dataA;
        protected System.Web.UI.WebControls.Label lbl_initdataA;
        protected System.Web.UI.WebControls.Label lbl_finedataA;
        protected System.Web.UI.WebControls.Label lbl_dtaC;
        protected System.Web.UI.WebControls.DropDownList ddl_dataC;
        protected System.Web.UI.WebControls.Label lbl_initdataC;
        protected System.Web.UI.WebControls.Label lbl_finedataC;
        protected System.Web.UI.WebControls.Label lbl_dtaCreaz;
        protected System.Web.UI.WebControls.DropDownList ddl_creaz;
        protected System.Web.UI.WebControls.Label lbl_initCreaz;
        protected System.Web.UI.WebControls.Label lbl_finCreaz;
        protected System.Web.UI.WebControls.Label lbl_NumFasc;
        protected System.Web.UI.WebControls.TextBox txtNumFasc;
        protected System.Web.UI.WebControls.Label lblAnnoFasc;
        protected System.Web.UI.WebControls.TextBox txtAnnoFasc;
        protected System.Web.UI.WebControls.Label lbl_tipo;
        protected System.Web.UI.WebControls.DropDownList ddlTipo;
        protected System.Web.UI.WebControls.Label lbl_descr;
        protected System.Web.UI.WebControls.TextBox txtDescr;
        protected System.Web.UI.WebControls.Label Label2;
        protected System.Web.UI.WebControls.DropDownList ddl_data_LF;
        protected System.Web.UI.WebControls.Label lbl_dta_LF_DA;
        protected System.Web.UI.WebControls.Label lbl_dta_LF_A;
        protected System.Web.UI.WebControls.Label Label3;
        protected System.Web.UI.WebControls.TextBox txt_varCodRubrica_LF;
        protected System.Web.UI.WebControls.TextBox txt_descr_LF;
        protected System.Web.UI.WebControls.Image btn_Rubrica;
        protected System.Web.UI.WebControls.Label lbl_uffRef;
        protected System.Web.UI.WebControls.TextBox txt_cod_UffRef;
        protected System.Web.UI.WebControls.TextBox txt_desc_uffRef;
        protected System.Web.UI.WebControls.Image btn_rubricaRef;
        protected System.Web.UI.WebControls.Panel pnl_uffRef;
        //protected DocsPaWebCtrlLibrary.DateMask txt_initDataA;
        protected DocsPAWA.UserControls.Calendar txt_initDataA;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineDataA;
        protected DocsPAWA.UserControls.Calendar txt_fineDataA;
        //protected DocsPaWebCtrlLibrary.DateMask txt_initDataC;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineDataC;
        //protected DocsPaWebCtrlLibrary.DateMask txt_initDataCrea;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineDataCrea;
        //protected DocsPaWebCtrlLibrary.DateMask txt_dta_LF_DA;
        //protected DocsPaWebCtrlLibrary.DateMask txt_dta_LF_A;
        protected DocsPAWA.UserControls.Calendar txt_initDataC;
        protected DocsPAWA.UserControls.Calendar txt_fineDataC;
        protected DocsPAWA.UserControls.Calendar txt_initDataCrea;
        protected DocsPAWA.UserControls.Calendar txt_fineDataCrea;
        protected DocsPAWA.UserControls.Calendar txt_dta_LF_DA;
        protected DocsPAWA.UserControls.Calendar txt_dta_LF_A;
        protected Ruolo userRuolo;
        protected HtmlInputHidden hd_systemIdLF;
        protected Registro userReg;
        protected DocsPAWA.DocsPaWR.Corrispondente[] listaCorr;
        protected HtmlInputHidden hd_systemIdUffRef;
        protected System.Web.UI.WebControls.Label lbl_mostraTuttiFascicoli;
        protected System.Web.UI.WebControls.RadioButtonList rbl_MostraTutti;
        protected System.Web.UI.WebControls.ImageButton btn_chiudi_risultato;
        protected System.Web.UI.WebControls.Button btn_ok;
        protected System.Web.UI.WebControls.Button Button1;
        protected System.Web.UI.WebControls.Button btn_ricFascicoli;
        protected System.Web.UI.WebControls.DataGrid DgListaFasc;
        protected System.Web.UI.WebControls.Button btn_find;
        protected System.Web.UI.WebControls.Label lbl_countRecord;
        protected System.Web.UI.WebControls.Button btn_chiudi;
        protected System.Web.UI.WebControls.Panel pnl_ric;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        protected System.Web.UI.WebControls.Label lbl_registro;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        protected System.Web.UI.WebControls.Panel pnl_registri;
        protected System.Web.UI.WebControls.Panel pnl_mostraSottonodi;
        private DocsPAWA.DocsPaWR.Registro[] userRegistri;
        protected static int currentPage;
        protected System.Web.UI.WebControls.Button ricFascicoli;
        protected System.Web.UI.WebControls.Panel pnlButtonOk;
        protected DocsPAWA.DocsPaWR.Fascicolo[] listaFascicoli;
        protected DocsPAWA.DocsPaWR.Fascicolo fascSel;
        private bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
            && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
        private Hashtable m_hashTableFascicoli;
        protected string codClassificaQS;
        protected string nodoMultiRegQS;
        protected System.Web.UI.WebControls.Label lbl_codClass;
        protected System.Web.UI.WebControls.Label lbl_note;
        protected System.Web.UI.WebControls.TextBox txt_note;
        protected string callerQS;
        protected System.Web.UI.WebControls.DropDownList ddl_titolari;
        protected DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPaWebService();
        protected int _grdPageSize;
        protected int nRec = 0;
        protected int numTotPage = 0;
        protected System.Web.UI.WebControls.TextBox txt_sottofascicolo;
        protected System.Web.UI.WebControls.Panel pnl_sottofacicoli;
        protected System.Web.UI.WebControls.CheckBox checkADL;
        protected System.Web.UI.WebControls.Panel pnl_profilazione;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoFasc;
        protected System.Web.UI.WebControls.ImageButton img_dettagliProfilazione;
        
        #endregion

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
            this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
            this.ddl_dataA.SelectedIndexChanged += new System.EventHandler(this.ddl_data_SelectedIndexChanged);
            this.ddl_dataC.SelectedIndexChanged += new System.EventHandler(this.ddl_dataC_SelectedIndexChanged);
            this.ddl_creaz.SelectedIndexChanged += new System.EventHandler(this.ddl_creaz_SelectedIndexChanged);
            this.ddlTipo.SelectedIndexChanged += new System.EventHandler(this.ddlTipo_SelectedIndexChanged);
            this.txt_varCodRubrica_LF.TextChanged += new System.EventHandler(this.txt_varCodRubrica_LF_TextChanged);
            this.ddl_data_LF.SelectedIndexChanged += new System.EventHandler(this.ddl_data_LF_SelectedIndexChanged);
            this.txt_cod_UffRef.TextChanged += new System.EventHandler(this.txt_cod_UffRef_TextChanged);
            this.ricFascicoli.Click += new System.EventHandler(this.ricFascicoli_Click);
            this.DgListaFasc.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DgListaFasc_PageIndexChange);
            this.DgListaFasc.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DgListaFasc_ItemDataBound);
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
            this.img_dettagliProfilazione.Click += new ImageClickEventHandler(img_dettagliProfilazione_Click);
            this.ddl_tipoFasc.SelectedIndexChanged += new EventHandler(ddl_tipoFasc_SelectedIndexChanged);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.ricercaFascicoli_PreRender);

        }

        #endregion

        #region Impostazioni iniziali / Eventi di pagina
        private void Page_Load(object sender, System.EventArgs e)
        {
            Response.Expires = -1;

            setDefaultButton();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            getParameterUser();

            bool isVisibleCombo = true;

           // _grdPageSize = FascicoliManager.getGrdFascicoliPageSize(this);
            _grdPageSize = 10;

            try
            {
                if (IsPostBack)
                {
                    if (this.Session["rubrica.listaCorr"] != null)
                    {
                        listaCorr = (DocsPAWA.DocsPaWR.Corrispondente[])Session["rubrica.listaCorr"];
                        if (listaCorr.Length > 0)
                        {
                            if (this.Session["typeRequest"] != null)
                            {
                                string typeRequest = this.Session["typeRequest"] as string;
                                switch (typeRequest.ToLower())
                                {
                                    case "filtrifasclf":
                                        this.txt_varCodRubrica_LF.Text = listaCorr[0].codiceRubrica;
                                        this.txt_descr_LF.Text = listaCorr[0].descrizione;
                                        setDescCorrispondente(this.txt_varCodRubrica_LF.Text, true);
                                        break;
                                    case "fascuffref":
                                        this.txt_cod_UffRef.Text = listaCorr[0].codiceRubrica;
                                        this.txt_desc_uffRef.Text = listaCorr[0].descrizione;
                                        setDescUffRef(this.txt_cod_UffRef.Text);
                                        break;
                                }
                            }
                        }
                        Session.Remove("rubrica.listaCorr");
                    }
                }
                else
                {
                    //SetControlFocus("txtNumFasc");
                    Session.Remove("rubrica.listaCorr");
                    FascicoliManager.removeUoReferenteSelezionato(this);
                    FascicoliManager.DO_RemoveIdUO_LF();
                    FascicoliManager.DO_RemoveLocazioneFisica();
                }

                //prendo i registri visibili all'utente.
                userRegistri = UserManager.getListaRegistri(this);

                pnl_registri.Visible = isVisibleCombo;

                if (!IsPostBack)
                {
                    //prendo i parametri dal queryString
                    getParametriQueryString();

                    RicercaFascicoliSessionMng.SetAsLoaded(this);	//la pagina è stata caricata

                    // Si pulisce la sessione da classificazioni precedenti
                    impostazioniIniziali();
                    caricaComboTitolari();

                    Session.Remove("rubrica.listaCorr");
                    FascicoliManager.removeUoReferenteSelezionato(this);
                    FascicoliManager.DO_RemoveIdUO_LF();
                    FascicoliManager.DO_RemoveLocazioneFisica();

                    // caso 1: nessun codice specificato
                    if (codClassificaQS != null && codClassificaQS == String.Empty)
                    {
                        this.pnl_mostraSottonodi.Visible = false;
                        this.rbl_MostraTutti.SelectedValue = "N";
                        this.rbl_MostraTutti.Visible = false;

                        this.lbl_codClass.Visible = false;
                        CaricaComboRegistri(ddl_registri, userRegistri);

                        switch (callerQS)
                        {
                            case ("profilo"):
                                if (userRegistri != null && userRegistri.Length == 1)
                                {
                                    this.ddl_registri.Enabled = false;
                                }
                                break;
                            case ("protocollo"):
                                this.ddl_registri.Enabled = false;
                                //di default si da selezionato quello selezionato su DocProtocollo
                                this.ddl_registri.SelectedIndex =
                                    ddl_registri.Items.IndexOf(ddl_registri.Items.FindByValue(UserManager.getRegistroSelezionato(this).systemId));
                                break;
                            case ("opzioniArchivio"):
                                this.ddl_registri.Enabled = true;
                                //this.ddl_registri.SelectedIndex = -1;
                                this.ddlTipo.Visible = false;
                                lbl_tipo.Visible = false;
                                this.lbl_NumFasc.Visible = false;
                                this.txtNumFasc.Visible = false;
                                this.pnl_sottofacicoli.Visible = false;
                                break;
                            default:
                                break;

                        }
                    }
                    else
                    {
                        // caso 2: codice specificato
                        this.lbl_mostraTuttiFascicoli.Text = "Estendi ricerca a tutti i sottonodi di " + codClassificaQS;
                        this.pnl_mostraSottonodi.Visible = true;

                        this.lbl_codClass.Visible = true;
                        this.lbl_codClass.Text = "sul codice di classifica <B>" + codClassificaQS + "</B>";
                        switch (callerQS)
                        {
                            case ("profilo"):
                                {

                                    switch (nodoMultiRegQS)
                                    {
                                        case "Y":
                                            {
                                                //codice digitato presente su più registri
                                                this.ddl_registri.Enabled = true;
                                                Hashtable hashRegistriNodi = new Hashtable();
                                                DocsPaWR.Fascicolo[] listaFascInSession = (DocsPAWA.DocsPaWR.Fascicolo[])Session["listaFascFascRapida"];
                                                hashRegistriNodi = getRegistriNodi(listaFascInSession);
                                                CaricaComboRegistriByHash(ddl_registri, hashRegistriNodi);
                                                Session.Remove("listaFascFascRapida");
                                            }
                                            break;

                                        //Il codice digitato è presente su solo registro
                                        //In tal caso si ricerca sul registro associato al nodo selezionato
                                        //o su tutti se il registro del nodo è null
                                        case "N":
                                            {
                                                DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)Session["FascSelezFascRap"];
                                                string idRegistroNodoTit = fasc.idRegistroNodoTit;
                                                //string idRegistroNodoTit = FascicoliManager.getFascicoloSelezionatoFascRapida(this).idRegistroNodoTit;
                                                if (idRegistroNodoTit != null && idRegistroNodoTit != String.Empty)
                                                {
                                                    //I REGISTRI li carico solo la prima volta
                                                    CaricaComboRegistri(ddl_registri, userRegistri);
                                                    //chiamante è docProtocollo
                                                    this.ddl_registri.Enabled = false;
                                                    this.ddl_registri.SelectedIndex =
                                                        ddl_registri.Items.IndexOf(ddl_registri.Items.FindByValue(idRegistroNodoTit));
                                                }
                                                else
                                                {
                                                    ddl_registri.Items.Add("Tutti");
                                                    this.ddl_registri.Enabled = false;
                                                }
                                                Session.Remove("FascSelezFascRap");
                                            }
                                            break;

                                        default:
                                            break;

                                    }
                                }
                                break;

                            case ("protocollo"):
                                {
                                    //I REGISTRI li carico solo la prima volta
                                    CaricaComboRegistri(ddl_registri, userRegistri);
                                    //chiamante è docProtocollo
                                    this.ddl_registri.Enabled = false;
                                    this.ddl_registri.SelectedIndex =
                                        ddl_registri.Items.IndexOf(ddl_registri.Items.FindByValue(UserManager.getRegistroSelezionato(this).systemId));
                                }
                                break;
                            case ("opzioniArchivio"):
                                CaricaComboRegistri(ddl_registri, userRegistri);
                                this.ddl_registri.Enabled = true;
                                //this.ddl_registri.SelectedIndex = -1;
                                this.ddlTipo.Visible = false;
                                lbl_tipo.Visible = false;
                                this.lbl_NumFasc.Visible = false;
                                this.txtNumFasc.Visible = false;
                                this.pnl_sottofacicoli.Visible = false;
                                break;
                            default:
                                break;

                        }
                    }

                }
                #region commento
                //					if(codClassificaQS!=null && codClassificaQS == String.Empty)
                //					{
                //						
                //						CaricaComboRegistri(ddl_registri, userRegistri);
                //						
                //						if(Request.QueryString["caller"]!=null && Request.QueryString["caller"]!="protocollo")
                //						{				
                //							if(userRegistri!= null && userRegistri.Length==1)
                //							{
                //								this.ddl_registri.Enabled=false;
                //							}
                //							settaRegistroSelezionato();
                //						}
                //						else
                //						{
                //							//chiamante è docProtocollo
                //							this.ddl_registri.Enabled=false;
                //							this.ddl_registri.SelectedIndex = 
                //								ddl_registri.Items.IndexOf(ddl_registri.Items.FindByValue(UserManager.getRegistroSelezionato(this).systemId));
                //						}
                //					}
                //					else
                //					{
                //						//se ho digitato qualcosa come codice:
                //						if(Request.QueryString["caller"]!=null && Request.QueryString["caller"]=="profilo")
                //						{
                //							if(Request.QueryString["NodoMultiReg"]!=null && Request.QueryString["NodoMultiReg"]=="Y")
                //							{
                //								this.ddl_registri.Enabled=true;
                //							}
                //							else
                //							{
                //								// nel caso di Profilo ricerco sul registro associato al nodo selezionato
                //								string idRegistroNodoTit = (FascicoliManager.getFascicoloSelezionatoFascRapida(this).idRegistroNodoTit);
                //								if(idRegistroNodoTit!= null && idRegistroNodoTit!=String.Empty)
                //								{
                //									//I REGISTRI li carico solo la prima volta
                //									CaricaComboRegistri(ddl_registri, userRegistri);
                //									//chiamante è docProtocollo
                //									this.ddl_registri.Enabled=false;
                //									this.ddl_registri.SelectedIndex = 
                //										ddl_registri.Items.IndexOf(ddl_registri.Items.FindByValue(idRegistroNodoTit));
                //								}
                //								else
                //								{
                //									ddl_registri.Items.Add("Tutti");
                //									this.ddl_registri.Enabled=false;
                //								}
                //							}
                //						}
                //						else
                //						{
                //							//I REGISTRI li carico solo la prima volta
                //							CaricaComboRegistri(ddl_registri, userRegistri);
                //							//chiamante è docProtocollo
                //							this.ddl_registri.Enabled=false;
                //							this.ddl_registri.SelectedIndex = 
                //								ddl_registri.Items.IndexOf(ddl_registri.Items.FindByValue(UserManager.getRegistroSelezionato(this).systemId));
                //						
                //						}
                //					}
                //				}
                #endregion
                else // se è PostBack
                {
                    if (this.Session["rubrica.listaCorr"] != null)
                    {
                        listaCorr = (DocsPAWA.DocsPaWR.Corrispondente[])Session["rubrica.listaCorr"];
                        if (listaCorr.Length > 0)
                        {
                            if (this.Session["typeRequest"] != null)
                            {
                                string typeRequest = this.Session["typeRequest"] as string;
                                switch (typeRequest.ToLower())
                                {
                                    case "filtrifasclf":
                                        this.txt_varCodRubrica_LF.Text = listaCorr[0].codiceRubrica;
                                        this.txt_descr_LF.Text = listaCorr[0].descrizione;
                                        setDescCorrispondente(this.txt_varCodRubrica_LF.Text, true);
                                        break;
                                    case "fascuffref":
                                        this.txt_cod_UffRef.Text = listaCorr[0].codiceRubrica;
                                        this.txt_desc_uffRef.Text = listaCorr[0].descrizione;
                                        setDescUffRef(this.txt_cod_UffRef.Text);
                                        break;
                                }
                            }
                        }
                        Session.Remove("rubrica.listaCorr");
                    }
                }
                //Profilazione dinamica fascicoli
                if (!IsPostBack)
                {
                    //this.AddControlsClientAttribute();
                    //this.txtTipoCorrispondente.Value = this.optListTipiCreatore.SelectedItem.Value;

                    if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    {
                        pnl_profilazione.Visible = true;
                        CaricaComboTipologiaFasc();
                    }
                }

                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                {
                    verificaCampiPersonalizzati();
                }
                else
                {
                    img_dettagliProfilazione.Visible = false;
                }
                //Fine profilazione dinamica fascicoli
            }

            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }

        public Hashtable getRegistriNodi(DocsPAWA.DocsPaWR.Fascicolo[] listaFasc)
        {
            Hashtable hasRegistriNodi = new Hashtable();

            foreach (DocsPAWA.DocsPaWR.Fascicolo fasc in listaFasc)
            {
                if (fasc.idRegistroNodoTit != null && fasc.idRegistroNodoTit != "")
                {
                    if (!hasRegistriNodi.ContainsKey(fasc.idRegistroNodoTit))
                    {
                        DocsPaWR.Registro reg = new Registro();
                        reg.codRegistro = fasc.codiceRegistroNodoTit;
                        reg.systemId = fasc.idRegistroNodoTit;
                        hasRegistriNodi.Add(fasc.idRegistroNodoTit, reg);
                    }
                }
            }
            return hasRegistriNodi;
        }

        public void getParametriQueryString()
        {
            //'codClassificaQS' indica il codice del nodo selezionato dall'utente
            codClassificaQS = Request.QueryString["codClassifica"];

            //'caller': indica la pagina chiamante
            //possibili valori: profilo/protocollo
            callerQS = Request.QueryString["caller"];

            //'nodoMultiRegQS' indica se il nodo è presente su più registri
            //possibili valori:'Y' in caso affermativo, 'N' altrimenti
            nodoMultiRegQS = Request.QueryString["NodoMultiReg"];
        }

        private void setDefaultButton()
        {
            //data apertura
            DocsPAWA.Utils.DefaultButton(this, ref this.GetCalendarControl("txt_initDataA").txt_Data, ref ricFascicoli);
            DocsPAWA.Utils.DefaultButton(this, ref this.GetCalendarControl("txt_fineDataA").txt_Data, ref ricFascicoli);

            //data chiusura
            DocsPAWA.Utils.DefaultButton(this, ref this.GetCalendarControl("txt_initDataC").txt_Data, ref ricFascicoli);
            DocsPAWA.Utils.DefaultButton(this, ref this.GetCalendarControl("txt_fineDataC").txt_Data, ref ricFascicoli);

            //data creazione
            DocsPAWA.Utils.DefaultButton(this, ref this.GetCalendarControl("txt_initDataCrea").txt_Data, ref ricFascicoli);
            DocsPAWA.Utils.DefaultButton(this, ref this.GetCalendarControl("txt_fineDataCrea").txt_Data, ref ricFascicoli);

            //combo registro
            DocsPAWA.Utils.DefaultButton(this, ref ddl_registri, ref ricFascicoli);

            //combo tipo
            DocsPAWA.Utils.DefaultButton(this, ref ddlTipo, ref ricFascicoli);

            //collocazione fisica
            DocsPAWA.Utils.DefaultButton(this, ref txt_varCodRubrica_LF, ref ricFascicoli);

            //ufficio referente
            DocsPAWA.Utils.DefaultButton(this, ref txt_cod_UffRef, ref ricFascicoli);
        }

        private void getParameterUser()
        {
            userRuolo = UserManager.getRuolo(this);
            userReg = UserManager.getRegistroSelezionato(this);
        }

        private void impostazioniIniziali()
        {
            FascicoliManager.removeAllClassValue(this);
            FascicoliManager.removeClassificazioneSelezionata(this);
        }

        #endregion

        #region per la gestione del ruolo multi-registro

        private void CaricaComboRegistriByHash(DropDownList ddl, Hashtable listaReg)
        {
            if (listaReg != null && listaReg.Count != 0)
            {
                foreach (DictionaryEntry entry in listaReg)
                {
                    DocsPaWR.Registro registro = entry.Value as DocsPAWA.DocsPaWR.Registro;

                    if (registro != null)
                    {
                        ddl.Items.Add(new ListItem(registro.codRegistro, registro.systemId));
                    }
                }
            }
            else
            {
                CaricaComboRegistri(ddl_registri, UserManager.getListaRegistri(this));
            }
        }

        private void CaricaComboRegistri(DropDownList ddl, DocsPAWA.DocsPaWR.Registro[] userRegistri)
        {
            if (userRegistri != null)
            {
                for (int i = 0; i < userRegistri.Length; i++)
                {
                    ddl.Items.Add(userRegistri[i].codRegistro);
                    ddl.Items[i].Value = userRegistri[i].systemId;
                }
            }

        }

        private void settaRegistroSelezionato()
        {
            if (ddl_registri.SelectedIndex != -1)//Di default la combo riporta il primo registro ritornato dalla query
            {
                if (userRegistri == null)
                    userRegistri = UserManager.getListaRegistri(this);
                if (userRegistri != null)
                {
                    //metto in sessione il registro selezionato
                    UserManager.setRegistroSelezionato(this, userRegistri[this.ddl_registri.SelectedIndex]);
                }
            }
        }

        private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            settaRegistroSelezionato();
        }

        #endregion

        #region classe per la creazione del datagrid

        public class RicercaFascicoliPerFascicolazioneRapida
        {

            private string tipo;
            private string codice;
            private string desc;
            private string apertura;
            private string chiusura;
            private string chiave;
            private string stato;
            private string registro;



            public RicercaFascicoliPerFascicolazioneRapida(string tipo, string codice, string desc, string apertura, string chiusura, string chiave, string stato, string registro)
            {

                this.tipo = tipo;
                this.codice = codice;
                this.desc = desc;
                this.apertura = apertura;
                this.chiusura = chiusura;
                this.chiave = chiave;
                this.stato = stato;
                this.registro = registro;
            }

            public string Tipo { get { return tipo; } }
            public string Codice { get { return codice; } }
            public string Descrizione { get { return desc; } }
            public string Apertura { get { return apertura; } }
            public string Chiusura { get { return chiusura; } }
            public string Chiave { get { return chiave; } }
            public string Stato { get { return stato; } }
            public string Registro { get { return registro; } }
        }

        #endregion

        #region gestione eventi della pagina

        private void ddl_data_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.GetCalendarControl("txt_fineDataA").txt_Data.Text = "";
            if (this.ddl_dataA.SelectedIndex == 0)
            {
                this.GetCalendarControl("txt_fineDataA").Visible = false;
                this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = false;
                this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = false;
                this.lbl_finedataA.Visible = false;
                this.lbl_initdataA.Visible = false;

            }
            else
            {
                this.GetCalendarControl("txt_fineDataA").Visible = true;
                this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = true;
                this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = true;
                this.lbl_finedataA.Visible = true;
                this.lbl_initdataA.Visible = true;
            }
        }

        private void ddl_dataC_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.GetCalendarControl("txt_fineDataC").txt_Data.Text = "";
            if (this.ddl_dataC.SelectedIndex == 0)
            {
                this.GetCalendarControl("txt_fineDataC").Visible = false;
                this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = false;
                this.lbl_finedataC.Visible = false;
                this.lbl_initdataC.Visible = false;

            }
            else
            {
                this.GetCalendarControl("txt_fineDataC").Visible = true;
                this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = true;
                this.lbl_finedataC.Visible = true;
                this.lbl_initdataC.Visible = true;
            }
        }

        private void ddl_creaz_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text = "";
            if (this.ddl_creaz.SelectedIndex == 0)
            {
                this.GetCalendarControl("txt_fineDataCrea").Visible = false;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = false;
                this.lbl_finCreaz.Visible = false;
                this.lbl_initCreaz.Visible = false;

            }
            else
            {
                this.GetCalendarControl("txt_fineDataCrea").Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = true;
                this.lbl_finCreaz.Visible = true;
                this.lbl_initCreaz.Visible = true;
            }
        }

        private void ddl_data_LF_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddl_data_LF.SelectedIndex == 0)
            {
                this.GetCalendarControl("txt_dta_LF_A").Visible = false;
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = false;
                this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = false;
                this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
                this.lbl_dta_LF_DA.Visible = false;
                this.lbl_dta_LF_A.Visible = false;
            }
            else
            {
                this.GetCalendarControl("txt_dta_LF_A").Visible = true;
                this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
                this.lbl_dta_LF_DA.Visible = true;
                this.lbl_dta_LF_A.Visible = true;
            }
        }

        private void ddlTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddlTipo.SelectedIndex == 1)
            {
                //				this.ddlStato.SelectedIndex = 0;
                //				this.ddlStato.Enabled=false;
                this.txtNumFasc.Text = "";
                this.txtNumFasc.ReadOnly = true;
                this.txtNumFasc.BackColor = Color.WhiteSmoke;
                this.txtAnnoFasc.Text = "";
                this.txtAnnoFasc.ReadOnly = true;
                this.txtAnnoFasc.BackColor = Color.WhiteSmoke;
                //data creazione non abilitata
                this.ddl_creaz.Enabled = false;
                this.lbl_initCreaz.Visible = false;
                this.GetCalendarControl("txt_initDataCrea").txt_Data.BackColor = Color.WhiteSmoke;
                this.GetCalendarControl("txt_initDataCrea").txt_Data.Text = "";
                this.GetCalendarControl("txt_initDataCrea").txt_Data.Enabled = false;
                //
                this.lbl_finCreaz.Visible = false;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text = "";
                this.GetCalendarControl("txt_fineDataCrea").Visible = false;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = false;
                this.ddl_creaz.SelectedIndex = 0;
                if (enableUfficioRef)//PER I FASCICOLI GENERALI LA RICERCA NN è ABILITATA
                {
                    this.txt_desc_uffRef.Text = "";
                    this.txt_cod_UffRef.Text = "";
                    this.txt_cod_UffRef.ReadOnly = true;
                    this.txt_cod_UffRef.BackColor = Color.WhiteSmoke;
                    this.txt_desc_uffRef.BackColor = Color.WhiteSmoke;
                    this.btn_rubricaRef.Enabled = false;
                }
            }
            else
            {

                //				this.ddlStato.Enabled=true;
                this.txtNumFasc.ReadOnly = false;
                this.txtNumFasc.BackColor = Color.White;
                this.txtAnnoFasc.ReadOnly = false;
                this.txtAnnoFasc.BackColor = Color.White;
                //data creazione abilitata
                this.ddl_creaz.Enabled = true;
                this.lbl_initCreaz.Visible = false;
                this.GetCalendarControl("txt_initDataCrea").txt_Data.BackColor = Color.White;
                this.GetCalendarControl("txt_initDataCrea").txt_Data.Enabled = true;
                if (enableUfficioRef)//PER I FASCICOLI procedimentali LA RICERCA è ABILITATA
                {
                    this.txt_cod_UffRef.ReadOnly = false;
                    this.txt_cod_UffRef.BackColor = Color.White;
                    this.txt_desc_uffRef.BackColor = Color.White;
                    this.btn_rubricaRef.Enabled = true;
                }

            }

        }


        #endregion

        #region popolamento locazione fisica

        private void setDescCorrispondente(string codiceRubrica, bool fineValidita)
        {
            string msg = "Codice rubrica non esistente";
            Corrispondente corr = null;
            try
            {
                corr = UserManager.GetCorrispondenteInterno(this, codiceRubrica, fineValidita);

                if ((corr != null && corr.descrizione != "") && corr.GetType().Equals(typeof(UnitaOrganizzativa)))
                {
                    txt_descr_LF.Text = corr.descrizione;
                    this.hd_systemIdLF.Value = corr.systemId;
                    FascicoliManager.DO_SetIdUO_LF(corr.systemId);
                    DocsPaVO.LocazioneFisica.LocazioneFisica LF =
                        new DocsPaVO.LocazioneFisica.LocazioneFisica(corr.systemId, corr.descrizione,
                        this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text, codiceRubrica);
                    FascicoliManager.DO_SetLocazioneFisica(LF);
                }
                else
                {
                    RegisterStartupScript("alert", "<script language='javascript'>alert('" + msg + "');</script>");
                }

            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void txt_varCodRubrica_LF_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txt_descr_LF.Text = "";
                if (txt_varCodRubrica_LF.Text != "")
                {
                    setDescCorrispondente(this.txt_varCodRubrica_LF.Text, true);
                }
                else
                {
                    FascicoliManager.DO_RemoveLocazioneFisica();
                    FascicoliManager.DO_RemoveFlagLF();
                    hd_systemIdLF.Value = "";
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        #endregion

        #region popolamento ufficio referente

        private void setDescUffRef(string codiceRubrica)
        {
            Corrispondente corr = null;
            string msg = "Codice rubrica non valido per l\\'Ufficio referente!";
            if (!codiceRubrica.Equals(""))
                corr = UserManager.getCorrispondenteReferente(this, codiceRubrica, false);
            if (corr != null && (corr.GetType().Equals(typeof(UnitaOrganizzativa))))
            {
                this.txt_desc_uffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
                this.hd_systemIdUffRef.Value = corr.systemId;
                FascicoliManager.setUoReferenteSelezionato(this, corr);
            }
            else
            {
                if (!codiceRubrica.Equals(""))
                {
                    RegisterStartupScript("alert", "<script language='javascript'>alert('" + msg + "');</script>");
                }

                this.txt_desc_uffRef.Text = "";
                this.hd_systemIdUffRef.Value = "";
            }

        }

        private void txt_cod_UffRef_TextChanged(object sender, EventArgs e)
        {
            try
            {
                setDescUffRef(this.txt_cod_UffRef.Text);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        #endregion

        #region RICERCA FASCICOLI


        private bool ricercaFasc()
        {
            bool result = true;
            try
            {
                //array contenitore degli array filtro di ricerca
                qV = new FiltroRicerca[1][];
                qV[0] = new FiltroRicerca[1];
                fVList = new FiltroRicerca[0];

                #region  filtro sulla tipologia del fascicolo
                if (ddlTipo.SelectedIndex > 0)
                {
                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.TIPO_FASCICOLO.ToString();
                    fV1.valore = ddlTipo.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region  filtro sullo stato del fascicolo

                //SI CERCANO SOLO FASCICOLI APERTI, IN QUELLI CHIUSI NON SI PUO FASCICOLARE
                fV1 = new FiltroRicerca();
                fV1.argomento = FiltriFascicolazione.STATO.ToString();
                fV1.valore = "A";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                #endregion

                #region numero fascicolo
                if (!this.txtNumFasc.Text.Equals(""))
                {
                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.NUMERO_FASCICOLO.ToString();
                    fV1.valore = this.txtNumFasc.Text.ToString();
                    if (Utils.isNumeric(fV1.valore))
                    {
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        Page.RegisterStartupScript("", "<script>alert('Il numero del fascicolo non è numerico!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txtNumFasc.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return false;
                    }
                }
                #endregion

                #region anno fascicolo
                if (!this.txtAnnoFasc.Text.Equals(""))
                {
                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.ANNO_FASCICOLO.ToString();
                    fV1.valore = this.txtAnnoFasc.Text.ToString();
                    if (Utils.isNumeric(fV1.valore))
                    {
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        Page.RegisterStartupScript("", "<script>alert('L\\'anno digitato non è numerico!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txtAnnoFasc.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return false;
                    }
                }

                #endregion

                #region  filtro sulla data di apertura fascicolo
                if (!this.GetCalendarControl("txt_initDataA").txt_Data.Text.Equals(""))
                {
                    fV1 = new FiltroRicerca();
                    if (this.ddl_dataA.SelectedIndex.Equals(0))
                        fV1.argomento = FiltriFascicolazione.APERTURA_IL.ToString();
                    else
                        fV1.argomento = FiltriFascicolazione.APERTURA_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataA").txt_Data.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    result = checkData(fV1.argomento, fV1.valore);
                    if (!result)
                    {
                        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataA").txt_Data.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return false;
                    }
                }
                if (!this.GetCalendarControl("txt_fineDataA").txt_Data.Text.Equals(""))
                {
                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.APERTURA_PRECEDENTE_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_fineDataA").txt_Data.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    result = checkData(fV1.argomento, fV1.valore);
                    if (!result)
                    {
                        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        return false;
                    }
                }
                #endregion

                #region  filtro sulla data chiusura di un fascicolo
                if (!this.GetCalendarControl("txt_initDataC").txt_Data.Text.Equals(""))
                {
                    fV1 = new FiltroRicerca();
                    if (this.ddl_dataC.SelectedIndex.Equals(0))
                        fV1.argomento = FiltriFascicolazione.CHIUSURA_IL.ToString();
                    else
                        fV1.argomento = FiltriFascicolazione.CHIUSURA_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataC").txt_Data.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    result = checkData(fV1.argomento, fV1.valore);
                    if (!result)
                    {
                        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        return false;
                    }
                }

                if (!this.GetCalendarControl("txt_fineDataC").txt_Data.Text.Equals(""))
                {
                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.CHIUSURA_PRECEDENTE_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_fineDataC").txt_Data.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    result = checkData(fV1.argomento, fV1.valore);
                    if (!result)
                    {
                        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        return false;
                    }
                }
                #endregion

                #region  filtro sulla data creazione di un fascicolo
                if (!this.GetCalendarControl("txt_initDataCrea").txt_Data.Text.Equals(""))
                {
                    fV1 = new FiltroRicerca();
                    if (this.ddl_creaz.SelectedIndex.Equals(0))
                        fV1.argomento = FiltriFascicolazione.CREAZIONE_IL.ToString();
                    else
                        fV1.argomento = FiltriFascicolazione.CREAZIONE_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataCrea").txt_Data.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    result = checkData(fV1.argomento, fV1.valore);
                    if (!result)
                    {
                        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        return false;
                    }
                }

                if (!this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text.Equals(""))
                {
                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.CREAZIONE_PRECEDENTE_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    result = checkData(fV1.argomento, fV1.valore);
                    if (!result)
                    {
                        Page.RegisterStartupScript("", "<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        return false;
                    }
                }
                #endregion

                #region descrizione
                if (!this.txtDescr.Text.Equals(""))
                {

                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.TITOLO.ToString();
                    fV1.valore = this.txtDescr.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion

                #region  note

                if (!this.txt_note.Text.Equals(""))
                {
                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.VAR_NOTE.ToString();
                    fV1.valore = this.txt_note.Text.ToString() + "@-@Q";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region data Locazione Fisica

                if (ddl_data_LF.SelectedIndex == 0)
                {
                    //si è scelto un valore singolo per le date
                    if (this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text != "")
                    {
                        fV1 = new FiltroRicerca();
                        fV1.argomento = FiltriFascicolazione.DATA_LF_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        result = checkData(fV1.argomento, fV1.valore);
                        if (!result)
                        {
                            Page.RegisterStartupScript("", "<script>alert('Il formato della data di locazione fisica non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            return false;
                        }
                    }
                }
                else
                {
                    //Si è scelto un intervallo di date
                    if (this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text != "")
                    {
                        fV1 = new FiltroRicerca();
                        fV1.argomento = FiltriFascicolazione.DATA_LF_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        result = checkData(fV1.argomento, fV1.valore);
                        if (!result)
                        {
                            Page.RegisterStartupScript("", "<script>alert('Il formato della data di locazione fisica non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            return false;
                        }
                    }
                    if (this.GetCalendarControl("txt_dta_LF_A").txt_Data.Text != "")
                    {
                        fV1 = new FiltroRicerca();
                        fV1.argomento = FiltriFascicolazione.DATA_LF_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        result = checkData(fV1.argomento, fV1.valore);
                        if (!result)
                        {
                            Page.RegisterStartupScript("", "<script>alert('Il formato della data di locazione fisica non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            return false;
                        }
                    }

                }
                #endregion

                #region descrizione Locazione Fisica
                if (this.hd_systemIdLF != null && this.hd_systemIdLF.Value != "")
                {
                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.ID_UO_LF.ToString();
                    string IdUoLF = this.hd_systemIdLF.Value; ;
                    fV1.valore = IdUoLF;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro ufficio referente fascicolo
                if (enableUfficioRef)
                {
                    if (this.txt_cod_UffRef.Text != null && !this.txt_cod_UffRef.Text.Equals(""))
                    {
                        if (this.hd_systemIdUffRef != null && !this.hd_systemIdUffRef.Value.Equals(""))
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = FiltriFascicolazione.ID_UO_REF.ToString();
                            fV1.valore = this.hd_systemIdUffRef.Value;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        else
                        {
                            if (!IsStartupScriptRegistered("alert"))
                            {
                                Page.RegisterStartupScript("", "<script>alert('Codice rubrica non valido per l\\'Ufficio referente!');</script>");
                            }

                            return false;
                        }
                    }

                }
                #endregion

                #region filtro tipologia fascicolo e profilazione dinamica

                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                {
                    if (Session["filtroProfDinamica"] != null)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1 = (DocsPaWR.FiltroRicerca)Session["filtroProfDinamica"];
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (this.ddl_tipoFasc.SelectedIndex > 0)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriFascicolazione.TIPOLOGIA_FASCICOLO.ToString();
                        fV1.valore = this.ddl_tipoFasc.SelectedItem.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                #endregion

                #region Filtro diTitolario

                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString();
                fV1.valore = this.ddl_titolari.SelectedValue;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                #endregion

                #region  sottofascicolo

                if (!this.txt_sottofascicolo.Text.Equals(""))
                {
                   fV1 = new FiltroRicerca();
                   fV1.argomento = FiltriFascicolazione.SOTTOFASCICOLO.ToString();
                   fV1.valore = this.txt_sottofascicolo.Text.ToString();
                   fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro per ADL
                if (this.checkADL.Checked)
                {
                    fV1 = new FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.DOC_IN_FASC_ADL.ToString();
                    fV1.valore = UserManager.getInfoUtente(this).idPeople.ToString() + "@" + UserManager.getRuolo(this).systemId.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion
                qV[0] = fVList;

                FascicoliManager.setFiltroRicFascNew(this, qV);
                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
                result = false;
                return result;
            }
        }

        private bool checkData(string argomento, string valore)
        {
            if (argomento.Equals("APERTURA_IL") ||
                argomento.Equals("APERTURA_SUCCESSIVA_AL") ||
                argomento.Equals("APERTURA_PRECEDENTE_IL") ||
                argomento.Equals("CHIUSURA_IL") ||
                argomento.Equals("CHIUSURA_SUCCESSIVA_AL") ||
                argomento.Equals("CHIUSURA_PRECEDENTE_IL") ||
                argomento.Equals("CREAZIONE_IL") ||
                argomento.Equals("CREAZIONE_SUCCESSIVA_AL") ||
                argomento.Equals("CREAZIONE_PRECEDENTE_IL") ||
                argomento.Equals("DATA_LF_IL") ||
                argomento.Equals("DATA_LF_SUCCESSIVA_AL") ||
                argomento.Equals("DATA_LF_PRECEDENTE_IL"))
            {
                if (!Utils.isDate(valore))
                {
                    return false;
                }
            }
            return true;
        }

        private void eseguiRicerca()
        {
            currentPage = 1;

            if (ricercaFasc())
            {
                LoadData(true,currentPage, nRec, numTotPage);
            }
        }
        #endregion

        #region caricamento del Datagrid

        private void LoadData(bool updateGrid, int numPage,int nRec, int numTotPage)
        {

            //ricavo i parametri dal QueryString
            string codClass = Request.QueryString["codClassifica"];
            DocsPaWR.FascicolazioneClassificazione classificazione = null;
            DocsPaWR.FiltroRicerca[][] filtroRicerca = null;
           
            DocsPaWR.Registro regSel = null;



            bool showChilds = false;
            if (codClass != null)
            {
                if (codClass != "")
                {

                    string mostraTutti = this.rbl_MostraTutti.SelectedValue.ToString();
                    switch (mostraTutti)
                    {
                        case "S":
                            {
                                showChilds = true;
                                break;
                            }
                        case "N":
                            {
                                showChilds = false;
                                break;
                            }
                    }

                    // vuol dire che l'utente su docProfilo ha specificato un fascicolo

                    // - se è GENERALE, il codClass è il codice del nodo di titolario a cui è associato
                    // - se è PROCEDIMENTALE il codClass coincide con il codice del nodo di titolario
                    // sotto il quale il fascicolo è stato creato
                    if (Request.QueryString["caller"] != null && Request.QueryString["caller"] == "profilo")
                    {
                        // nel caso di Profilo ricerco sul registro associato al nodo selezionato
                        if (!(Request.QueryString["nodoMultiReg"] != null && Request.QueryString["nodoMultiReg"].Equals("Y")))
                        {

                            string idRegistroNodoTit = string.Empty;
                           Fascicolo fasc= (FascicoliManager.getFascicoloSelezionatoFascRapida(this));
                           if (fasc != null)
                               idRegistroNodoTit = fasc.idRegistroNodoTit;
                            if (idRegistroNodoTit != null && idRegistroNodoTit != String.Empty)
                            {
                                regSel = UserManager.getRegistroBySistemId(this, idRegistroNodoTit);
                            }
                            else
                            {
                                regSel = null;
                            }
                        }
                        else
                        {

                            string sysRegSel = ddl_registri.SelectedItem.Value;
                            regSel = UserManager.getRegistroBySistemId(this, sysRegSel);
                        }
                    }
                    else
                    {
                        regSel = UserManager.getRegistroSelezionato(this);
                    }
                    //DocsPaWR.FascicolazioneClassificazione[] titolario = FascicoliManager.fascicolazioneGetTitolario(this,codClass,regSel,false);
                    DocsPaWR.FascicolazioneClassificazione[] titolario = FascicoliManager.fascicolazioneGetTitolario2(this, codClass, regSel, false, ddl_titolari.SelectedValue);
                    if (titolario == null || titolario.Length < 1)
                    {
                        //return;
                        //Response.Write("<script>window.alert('Attenzione, si è verificato un errore\\ndurante l'operazione di ricerca'); window.close();</script>");
                    }
                    if (titolario.Length > 0
                        && titolario[0] != null)
                        classificazione = titolario[0];
                  

                    FascicoliManager.setClassificazioneSelezionata(this, classificazione);
                    filtroRicerca = FascicoliManager.getFiltroRicFascNew(this);

                    // Lista dei system id dei fascicoli. Non utilizzato
                    SearchResultInfo[] idProjects = null;

                    if (Request.QueryString["caller"] != null && Request.QueryString["caller"] == "profilo")
                    {
                        listaFascicoli = FascicoliManager.getListaFascicoliPaging(this, classificazione, regSel, filtroRicerca[0], showChilds, numPage, out numTotPage, out nRec, _grdPageSize, false, out idProjects, null);
                        //listaFascicoli = FascicoliManager.getListaFascicoli(this, classificazione, filtroRicerca[0], showChilds, regSel);
   
       				}
                    if (Request.QueryString["caller"] != null && Request.QueryString["caller"] == "protocollo")
                    {
                        listaFascicoli = FascicoliManager.getListaFascicoliPaging(this, classificazione, null, filtroRicerca[0], showChilds, numPage, out numTotPage, out nRec, _grdPageSize, false, out idProjects, null);
                        //listaFascicoli = FascicoliManager.getListaFascicoli(this, classificazione, filtroRicerca[0], showChilds);

                    }
                    if (Request.QueryString["caller"] != null && Request.QueryString["caller"] == "opzioniArchivio")
                    {
                        listaFascicoli = FascicoliManager.getListaFascicoliDaArchiviare(this, classificazione, regSel, filtroRicerca[0], showChilds, numPage, out numTotPage, out nRec, _grdPageSize);
                    }
                    if (Request.QueryString["caller"] != null && Request.QueryString["caller"] == "smistamento")
                    {
                        listaFascicoli = FascicoliManager.getListaFascicoliPaging(this, classificazione, regSel, filtroRicerca[0], showChilds, numPage, out numTotPage, out nRec, _grdPageSize, false, out idProjects, null);
                        //listaFascicoli = FascicoliManager.getListaFascicoli(this, classificazione, filtroRicerca[0], showChilds, regSel);
                    }
               
                }
                else
                {
                    // CASO IN CUI l'utente non ha digitato alcun codice fascicolo nella pagina chiamante,
                    // quindi la ricerca dei fascicoli verrà effettuata su tutto il titolario
                    //Si stanno cercando i fascicoli nell'intero titolario, allora
                    //prendo,				
                    //nel protocollo: il registro su cui si sta protocollando
                    //nel profilo: il regitro selezionato dall'utente nella combo
                    string sysRegSel = ddl_registri.SelectedItem.Value;
                    regSel = UserManager.getRegistroBySistemId(this, sysRegSel);
                    //regSel = UserManager.getRegistroSelezionato(this);
                    //				
                    classificazione = null;
                    //si cerca in tutto il titolario
                    showChilds = true;
                    FascicoliManager.setClassificazioneSelezionata(this, classificazione);
                    filtroRicerca = FascicoliManager.getFiltroRicFascNew(this);

                    if (Request.QueryString["caller"] != null && Request.QueryString["caller"] == "opzioniArchivio")
                    {
                        listaFascicoli = FascicoliManager.getListaFascicoliDaArchiviare(this, classificazione, regSel, filtroRicerca[0], showChilds, numPage, out numTotPage, out nRec, _grdPageSize);
                    }
                    else
                    {
                        SearchResultInfo[] idProjects = null;
                        listaFascicoli = FascicoliManager.getListaFascicoliPaging(this, classificazione, regSel, filtroRicerca[0], showChilds, numPage, out numTotPage, out nRec, _grdPageSize, false, out idProjects, null);
                        //listaFascicoli = FascicoliManager.getListaFascicoli(this, classificazione, filtroRicerca[0], showChilds, regSel);
                    }
                }

            }

            if (listaFascicoli != null && listaFascicoli.Length > 0)
            {

                this.DgListaFasc.CurrentPageIndex = currentPage - 1;
                this.DgListaFasc.VirtualItemCount = nRec;
                this.DgListaFasc.DataSource = listaFascicoli;
                this.BindGrid(listaFascicoli);
                this.lbl_countRecord.Text = "Fascicoli totali:  " + nRec;


                this.DgListaFasc.Visible = true;
                this.pnlButtonOk.Visible = true;


                //this.lbl_countRecord.Text = "Fascicoli totali:  " + m_hashTableFascicoli.Count;
                //this.DgListaFasc.VirtualItemCount = m_hashTableFascicoli.Count;
            }
            else
            {
                this.lbl_countRecord.Text = "Fascicoli totali: 0";
            }
            this.lbl_countRecord.Visible = true;


            //appoggio il risultato in sessione



            //setto la HASHTABLE in sessione e la lista fascicoli
            FascicoliManager.setHashFascicoli(this, m_hashTableFascicoli);
            RicercaFascicoliSessionMng.SetListaFascicoli(this, listaFascicoli);
        }

        private void DgListaFasc_PageIndexChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {

            DgListaFasc.CurrentPageIndex = e.NewPageIndex;
            currentPage = e.NewPageIndex + 1;

            DgListaFasc.SelectedIndex = -1;
            DgListaFasc.VirtualItemCount = nRec;
 
            // Cricamento del DataGrid
            this.LoadData(true, currentPage, nRec, numTotPage);
        }

        public void BindGrid(DocsPAWA.DocsPaWR.Fascicolo[] listaFasc)
        {
            DocsPaWR.Fascicolo currentFasc;
            m_hashTableFascicoli = new Hashtable(listaFasc.Length);
            if (listaFasc != null && listaFasc.Length > 0)
            {
                //Costruisco il datagrid
                ArrayList Dg_elem = new ArrayList();

                for (int i = 0; i < listaFasc.Length; i++)
                {
                    //la chiave della HashTable è la systemId del fascicolo
                    if (!m_hashTableFascicoli.ContainsKey(listaFasc[i].systemID))
                    {
                        m_hashTableFascicoli.Add(listaFasc[i].systemID, listaFasc[i]);

                        currentFasc = ((DocsPAWA.DocsPaWR.Fascicolo)listaFasc[i]);

                        string dtaApertura = "";
                        string dtaChiusura = "";

                        if (currentFasc.apertura != null && currentFasc.apertura.Length > 0)
                            dtaApertura = currentFasc.apertura.Substring(0, 10);

                        if (currentFasc.chiusura != null && currentFasc.chiusura.Length > 0)
                            dtaChiusura = currentFasc.chiusura.Substring(0, 10);

                        //dati registro associato al nodo di titolario
                        string idRegistro = currentFasc.idRegistroNodoTit; // systemId del registro
                        string codiceRegistro = currentFasc.codiceRegistroNodoTit; //codice del Registro
                        //
                        if (idRegistro != null && idRegistro == String.Empty)//se il fascicolo è associato a un TITOLARIO con idREGISTRO = NULL
                            codiceRegistro = "TUTTI";


                        string tipoFasc = currentFasc.tipo;
                        string descFasc = currentFasc.descrizione;
                        string stato = currentFasc.stato;
                        string chiave = currentFasc.systemID;
                        string codFasc = currentFasc.codice;

                        Dg_elem.Add(new RicercaFascicoliPerFascicolazioneRapida(tipoFasc, codFasc, descFasc, dtaApertura, dtaChiusura, chiave, stato, codiceRegistro));
                    }

                }

                this.DgListaFasc.SelectedIndex = -1;
                this.DgListaFasc.DataSource = Dg_elem;
                this.DgListaFasc.DataBind();
            }
        }

        private bool IsAbilitataRicercaSottoFascicoli()
        {
           if (ConfigSettings.getKey(ConfigSettings.KeysENUM.CERCA_SOTTOFASCICOLI) == null || ConfigSettings.getKey(ConfigSettings.KeysENUM.CERCA_SOTTOFASCICOLI).Equals("0"))
              return false;
           else
              return true;
        }

        private void DgListaFasc_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
           if (e.Item.ItemType == ListItemType.Header)
           {
              if (IsAbilitataRicercaSottoFascicoli())
                 DgListaFasc.Columns[1].Visible = true;
              else
                 DgListaFasc.Columns[1].Visible = false;
           }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RadioButton rb = e.Item.Cells[0].Controls[0].FindControl("OptFasc") as RadioButton;

                //il record relativo ai fascicoli chiusi li sottolineo
                //non si puo fascicolare in fascicoli chiusi
                string dtaChiusuraFasc = ((Label)e.Item.Cells[8].Controls[1]).Text;

                if (dtaChiusuraFasc != null)
                {
                    if (dtaChiusuraFasc == String.Empty)//se il fascicolo è aperto
                    {
                        rb.Enabled = true;
                    }
                    else //se il fascicolo è chiuso
                    {
                        rb.Enabled = false;
                        e.Item.ToolTip = "Il fascicolo è chiuso";
                    }

                    //					try
                    //					{
                    //						DateTime dt = Convert.ToDateTime(dtaChiusuraFasc);
                    //						e.Item.ForeColor = Color.Gray;
                    //						e.Item.Font.Bold=true;
                    //						e.Item.Font.Strikeout=true;	
                    //					}
                    //					catch {}		
                }
            }
        }

        private void btn_chiudi_Click(object sender, System.EventArgs e)
        {
            //if (!this.IsStartupScriptRegistered("chiudiModalDialog"))
            //{
            //    string scriptString = "<SCRIPT>window.returnValue = 'N'; window.close()</SCRIPT>";
            //    this.RegisterStartupScript("chiudiModalDialog", scriptString);
            //}
            ClientScript.RegisterStartupScript(this.GetType(), "chiudi", "window.close();", true);
        }

        #region focus
        /// <summary>
        /// Impostazione del focus su un controllo
        /// </summary>
        /// <param name="controlID"></param>
        private void SetControlFocus(string controlID)
        {
            this.RegisterClientScript("SetFocus", "SetControlFocus('" + controlID + "');");
        }
        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        #endregion

        private void ricFascicoli_Click(object sender, System.EventArgs e)
        {
            //impostazioni iniziali

            this.DgListaFasc.Visible = false;
            this.lbl_countRecord.Visible = false;
            resetOption();
            //fine impostazioni iniziali

            //pulisco i risultati di ricerca precedenti
            //ad eccezione del registro, settato nel page load della pagina
            RicercaFascicoliSessionMng.ClearDatiRicerca(this);

            eseguiRicerca();
        }

        private void resetOption()
        {
            foreach (DataGridItem dgItem in this.DgListaFasc.Items)
            {
                RadioButton optFasc = dgItem.Cells[0].FindControl("optFasc") as RadioButton;
                optFasc.Checked = false;
            }
        }

        private void btn_ok_Click(object sender, System.EventArgs e)
        {
            //key: chiave dell'item, ovvero dell'elemento selezionato
            string key;
           

            bool avanza = verificaSelezione(out key);

            if (avanza)
            {
                m_hashTableFascicoli = FascicoliManager.getHashFascicoli(this);
                if (m_hashTableFascicoli != null)
                {
                    fascSel = (DocsPAWA.DocsPaWR.Fascicolo)m_hashTableFascicoli[key];
                    if (!string.IsNullOrEmpty(Request.QueryString["ricerca"]))
                    {
                        FascicoliManager.setFascicoloSelezionatoRicerca(this, fascSel);
                    }
                    else
                    {
                        FascicoliManager.setFascicoloSelezionatoFascRapida(this, fascSel);
                    }
                    Response.Write("<script>window.returnValue = 'Y'; window.close();</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Attenzione: selezionare un fascicolo');</script>");
            }

        }

        /// <summary>
        /// Verifica se è stata selezionata almeno una opzione, ovvero un fascicoli
        /// nel datagrid
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        private bool verificaSelezione(out string key)
        {
            bool verificaSelezione = false;
            key = "";
            foreach (DataGridItem dgItem in this.DgListaFasc.Items)
            {
                RadioButton optFasc = dgItem.Cells[3].FindControl("optFasc") as RadioButton;
                if ((optFasc != null) && optFasc.Checked == true)
                {
                    key = ((Label)dgItem.Cells[9].Controls[1]).Text;
                    verificaSelezione = true;
                    break;
                }
            }
            return verificaSelezione;
        }

        private void ricercaFascicoli_PreRender(object sender, System.EventArgs e)
        {
            string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
            if (use_new_rubrica == "1")
                this.btn_Rubrica.Attributes.Add("onClick", "_ApriRubrica('filtri_locfis');");
            else
                this.btn_Rubrica.Attributes.Add("onclick", "ApriRubricaUfficioRef('FiltriFascLF','U');");

            // **************************************************************************

            btn_Rubrica.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + btn_Rubrica.ClientID + "');";
            btn_Rubrica.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + btn_Rubrica.ClientID + "');";

            if (enableUfficioRef)
            {
                btn_rubricaRef.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + btn_rubricaRef.ClientID + "');";
                btn_rubricaRef.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + btn_rubricaRef.ClientID + "');";
            }

            DocsPaVO.LocazioneFisica.LocazioneFisica LF = FascicoliManager.DO_GetLocazioneFisica();

            if (LF != null)
            {
                this.txt_varCodRubrica_LF.Text = LF.CodiceRubrica;
                this.txt_descr_LF.Text = LF.Descrizione;
                this.hd_systemIdLF.Value = LF.UO_ID;
                FascicoliManager.DO_RemoveIdUO_LF();
                FascicoliManager.DO_RemoveLocazioneFisica();
            }


            if (!this.Page.IsClientScriptBlockRegistered("imposta_cursore"))
            {
                this.Page.RegisterClientScriptBlock("imposta_cursore",
                    "<script language=\"javascript\">\n" +
                    "function ImpostaCursore (t, ctl)\n{\n" +
                    "document.getElementById(ctl).style.cursor = (t == 0) ? 'default' : 'hand';\n" +
                    "}\n</script>\n");
            }
            // personalizzazzione label data collocazione fisica da web.config.
            if (Utils.label_data_Loc_fisica.Trim() != "")
                this.Label2.Text = Utils.label_data_Loc_fisica;
            else
                this.Label2.Text = "Data Collocaz.";
        }

        #region classe per la gestione della sessione
        /// <summary>
        /// Classe per la gestione dei dati in sessione relativamente
        /// alla dialog "RicercaFascicoli.aspx"
        /// </summary>
        public sealed class RicercaFascicoliSessionMng
        {
            private RicercaFascicoliSessionMng()
            {
            }
            /// <summary>
            /// Gestione rimozione dati in sessione
            /// </summary>
            /// <param name="page"></param>
            public static void ClearSessionData(Page page)
            {

                FascicoliManager.removeFiltroRicFascNew(page);
                //UserManager.removeRegistroSelezionato(page);
                //FascicoliManager.removeFascicoloSelezionatoFascRapida(page);
                FascicoliManager.DO_RemoveFlagLF();
                FascicoliManager.DO_RemoveLocazioneFisica();
                RemoveListaFascicoli(page);
                FascicoliManager.removeHashFascicoli(page);
            }

            public static void ClearDatiRicerca(Page page)
            {

                FascicoliManager.removeFiltroRicFascNew(page);
                //FascicoliManager.removeFascicoloSelezionatoFascRapida(page);
                RemoveListaFascicoli(page);
                FascicoliManager.removeHashFascicoli(page);
            }


            public static void SetListaFascicoli(Page page, DocsPaWR.Fascicolo[] listaFascicoli)
            {
                page.Session["RicercaFascicoli.ListaFasc"] = listaFascicoli;
            }

            public static DocsPAWA.DocsPaWR.Fascicolo[] GetListaFascicoli(Page page)
            {
                return page.Session["RicercaFascicoli.ListaFasc"] as DocsPAWA.DocsPaWR.Fascicolo[];
            }

            public static void RemoveListaFascicoli(Page page)
            {
                page.Session.Remove("RicercaFascicoli.ListaFasc");
            }

            /// <summary>
            /// Impostazione flag booleano, se true, la dialog è stata caricata almeno una volta
            /// </summary>
            /// <param name="page"></param>
            public static void SetAsLoaded(Page page)
            {
                page.Session["RicercaFascicoli.isLoaded"] = true;
            }

            /// <summary>
            /// Impostazione flag relativo al caricamento della dialog
            /// </summary>
            /// <param name="page"></param>
            public static void SetAsNotLoaded(Page page)
            {
                page.Session.Remove("RicercaFascicoli.isLoaded");
            }

            /// <summary>
            /// Verifica se la dialog è stata caricata almeno una volta
            /// </summary>
            /// <param name="page"></param>
            /// <returns></returns>
            public static bool IsLoaded(Page page)
            {
                return (page.Session["RicercaFascicoli.isLoaded"] != null);
            }
        #endregion

        }
        #endregion

        #region Titolari
        private void caricaComboTitolari()
        {
            ddl_titolari.Items.Clear();
            //ArrayList listaTitolari = new ArrayList(wws.getTitolari(UserManager.getUtente(this).idAmministrazione));
            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(UserManager.getUtente(this).idAmministrazione));

            //Esistono dei titolari chiusi
            if (listaTitolari.Count > 1)
            {
                //Creo le voci della ddl dei titolari
                string valueTutti = string.Empty;
                foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                {
                    ListItem it = null;
                    switch (titolario.Stato)
                    {
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                    }
                }
                //Imposto la voce tutti i titolari
                valueTutti = valueTutti.Substring(0, valueTutti.Length - 1);
                if (valueTutti != string.Empty)
                {
                    ListItem it = new ListItem("Tutti i titolari", valueTutti);
                    ddl_titolari.Items.Insert(0, it);
                }

                //se la chiave di db è a 1, seleziono di default il titolario attivo
                if (utils.InitConfigurationKeys.GetValue("0", "FE_CHECK_TITOLARIO_ATTIVO").Equals("1"))
                {
                    int indexTitAtt = 0;
                    foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                    {
                        if (titolario.Stato == DocsPaWR.OrgStatiTitolarioEnum.Attivo)
                        {
                            ddl_titolari.SelectedIndex = ++indexTitAtt;
                            break;
                        }
                        indexTitAtt++;
                    }
                }
            }

            //Non esistono titolario chiusi
            if (listaTitolari.Count == 1)
            {
                DocsPaWR.OrgTitolario titolario = (DocsPaWR.OrgTitolario)listaTitolari[0];
                ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                ddl_titolari.Items.Add(it);
                ddl_titolari.Enabled = false;
            }
        }
        #endregion

        #region Profilazione dinamica
        private void CaricaComboTipologiaFasc()
        {
            ArrayList listaTipiFasc = new ArrayList(ProfilazioneFascManager.getTipoFascFromRuolo(UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "1",this));
            ListItem item = new ListItem();
            item.Value = "";
            item.Text = "";
            ddl_tipoFasc.Items.Add(item);
            for (int i = 0; i < listaTipiFasc.Count; i++)
            {
                DocsPaWR.Templates templates = (DocsPaWR.Templates)listaTipiFasc[i];
                ListItem item_1 = new ListItem();
                item_1.Value = templates.SYSTEM_ID.ToString();
                item_1.Text = templates.DESCRIZIONE;
                if (templates.IPER_FASC_DOC == "1")
                    ddl_tipoFasc.Items.Insert(1, item_1);
                else
                    ddl_tipoFasc.Items.Add(item_1);
            }
        }

        private void img_dettagliProfilazione_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "profilazioneDinamica", "apriPopupAnteprima();", true);
        }

        private void ddl_tipoFasc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Session.Remove("templateRicerca");
            Session.Remove("filtroProfDinamica");
            verificaCampiPersonalizzati();
            //CaricaComboStatiFasc();
        }

        private void verificaCampiPersonalizzati()
        {
            DocsPaWR.Templates template = new DocsPAWA.DocsPaWR.Templates();
            if (!ddl_tipoFasc.SelectedValue.Equals(""))
            {
                template = (DocsPAWA.DocsPaWR.Templates)Session["templateRicerca"];
                if (template == null)
                {
                    template = ProfilazioneFascManager.getTemplateFascById(ddl_tipoFasc.SelectedValue,this);
                    Session.Add("templateRicerca", template);
                }
                if (template != null && !(ddl_tipoFasc.SelectedItem.Text.ToUpper()).Equals(template.DESCRIZIONE.ToUpper()))
                {
                    template = ProfilazioneFascManager.getTemplateFascById(ddl_tipoFasc.SelectedValue,this);
                    Session.Add("templateRicerca", template);
                }
            }
            if (template != null && template.SYSTEM_ID == 0)
            {
                img_dettagliProfilazione.Visible = false;
            }
            else
            {
                if (template != null && template.ELENCO_OGGETTI.Length != 0)
                {
                    img_dettagliProfilazione.Visible = true;
                }
                else
                {
                    img_dettagliProfilazione.Visible = false;
                }
            }
        }

        #endregion


        protected void DgListaFasc_ItemCommand(object source, DataGridCommandEventArgs e)
        {              
              if (e.CommandName.Equals("Sottofascicoli"))
              {
                 string idFascicolo = ((Label)e.Item.Cells[9].Controls[1]).Text;
                 string registro = ((Label)e.Item.Cells[3].Controls[1]).Text;
                 string codFascicolo = ((Label)e.Item.Cells[2].Controls[1]).Text;
                 RegisterStartupScript("openModale", "<script>ApriRicercaSottoFascicoli('" + idFascicolo + "','" + txt_sottofascicolo.Text + "')</script>");
              }           
        }

       
       
    }
}
