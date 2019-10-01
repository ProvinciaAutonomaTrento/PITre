using System;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using Microsoft.Web.UI.WebControls;
using System.IO;
using ImageButton = DocsPaWebCtrlLibrary.ImageButton;

namespace DocsPAWA.fascicoli
{
    public class FiltriRicFasc : DocsPAWA.CssPage
    {
        #region variabili codice
        private bool allClass;
        private FiltroRicerca[][] qV;
        private FiltroRicerca fV1;
        private FiltroRicerca[] fVList;
        private Ruolo userRuolo;
        private Registro userReg;
        private const string QUERYSTRING_tipoVisualizzazione = "tipoVisualizzazione";
        private const string QUERYSTRING_codClassifica = "codClassifica";
        private string classSelez = "";
        protected Button Button1;
        protected TextBox TextBox1;
        protected Label lbl_mostraTuttiFascicoli;
        protected RadioButtonList rbl_MostraTutti;
        protected HtmlInputHidden hd_systemIdUffRef;
        protected System.Web.UI.WebControls.Panel pnl_profilazione;
        protected System.Web.UI.WebControls.Panel pnl_mostraSottonodi;
        protected System.Web.UI.WebControls.Panel pnl_chiuso;
        protected System.Web.UI.WebControls.Panel pnl_spazioFinale;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoFasc;
        protected System.Web.UI.WebControls.ImageButton img_dettagliProfilazione;
        protected DocsPaWR.DocsPaWebService wws = new DocsPaWR.DocsPaWebService();
        private bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
            && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));

        protected System.Web.UI.WebControls.Panel pnl_sottofascicolo;
        protected System.Web.UI.WebControls.TextBox txt_sottofascicolo;
        protected System.Web.UI.HtmlControls.HtmlLink idLinkCss;

        private enum tipoVisualizzazioneFascicoli
        {
            tvfUndefined, tvfGenerali, tvfProcedimentali, tvfAll
        }

        protected System.Web.UI.WebControls.DropDownList ddl_titolari;
        protected System.Web.UI.WebControls.Label lbl_Titolari;

        //Variabili per ricerca filtrata con elenco valori su file excel
        protected System.Web.UI.WebControls.Panel pnl_filtroExcel;
        protected System.Web.UI.WebControls.DropDownList ddl_attributo;
        protected System.Web.UI.WebControls.Panel pnl_tipoFascExcel;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoFascExcel;
        protected System.Web.UI.WebControls.Panel pnl_attrTipoFascExcel;
        protected System.Web.UI.WebControls.DropDownList ddl_attrTipoFascExcel;
        protected System.Web.UI.WebControls.FileUpload uploadedFile;
        protected System.Web.UI.WebControls.Label lbl_fileExcel;
        protected System.Web.UI.WebControls.ImageButton btn_elimina_excel;
        protected System.Web.UI.WebControls.Button UploadBtn;
        #endregion

        #region Sezione gestione script JavaScript lato client
        //è necessario che alla fine delal sezione HTML della pagina
        //sia presente il seguente codice:
        //
        //				<script language="javascript">
        //					esecuzioneScriptUtente();
        //				</script>
        //
        //nella page_Load:		
        //				if (!IsPostBack)
        //				{
        //					generaFunctionChiamataScript();
        //				}
        //chiamare addScript() per aggiungere gli script 
        //da eseguire alla fine della routine che ne richiede
        //l'esecuzione. Infine, eseguire generaFunctionChiamataScript()

        private ArrayList storedScriptCall = new ArrayList();
        //private string nameScript="script";
        protected System.Web.UI.WebControls.Button btn_ricFascicoli;
        //protected System.Web.UI.WebControls.Label lbl_result;
        protected Boolean l_createFascicolo;
        protected Label lbl_esito;
        protected DropDownList ddl_ragioni;
        protected RadioButtonList rbl_esito;
        protected Label lbl_initdataA;
        protected Label lbl_finedataA;
        protected Label lbl_initdataC;
        protected Label lbl_finedataC;
        //protected DocsPaWebCtrlLibrary.DateMask txt_initDataA;
        protected DocsPAWA.UserControls.Calendar txt_initDataA;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineDataA;
        protected DocsPAWA.UserControls.Calendar txt_fineDataA;
        //protected DocsPaWebCtrlLibrary.DateMask txt_initDataC;
        protected DocsPAWA.UserControls.Calendar txt_initDataC;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineDataC;
        protected DocsPAWA.UserControls.Calendar txt_fineDataC;
        protected DropDownList ddl_dataA;
        protected DropDownList ddl_dataC;
        protected Label lblAnnoFasc;
        protected Label lbl_NumFasc;
        protected TextBox txtAnnoFasc;
        protected Label lbl_Stato;
        protected Label lbl_tipo;
        protected DropDownList ddlStato;
        protected DropDownList ddlTipo;
        protected Label lbl_descr;
        protected TextBox txtDescr;
        protected Label ldl_apertA;
        protected Label lbl_dtaC;
        protected Label lbl_dtaCreaz;
        protected DropDownList ddl_creaz;
        protected Label lbl_initCreaz;
        protected Label lbl_finCreaz;
        //protected DocsPaWebCtrlLibrary.DateMask txt_initDataCrea;
        protected DocsPAWA.UserControls.Calendar txt_initDataCrea;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineDataCrea;
        protected DocsPAWA.UserControls.Calendar txt_fineDataCrea;
        protected ImageButton btn_ricTit;
        protected Microsoft.Web.UI.WebControls.TreeView Titolario;
        protected RadioButtonList rblst11;
        protected RadioButtonList rbLst1;
        protected Label Label2;
        protected DropDownList ddl_data_LF;
        protected Label lbl_dta_LF_DA;
        //protected DocsPaWebCtrlLibrary.DateMask txt_dta_LF_DA;
        protected DocsPAWA.UserControls.Calendar txt_dta_LF_DA;
        protected Label lbl_dta_LF_A;
        //protected DocsPaWebCtrlLibrary.DateMask txt_dta_LF_A;
        protected DocsPAWA.UserControls.Calendar txt_dta_LF_A;
        protected Label Label3;
        protected TextBox txt_descr_LF;
        protected TextBox txt_varCodRubrica_LF;
        protected Label lbl_uffRef;
        protected TextBox txt_cod_UffRef;
        protected TextBox txt_desc_uffRef;
        protected System.Web.UI.WebControls.Image btn_rubricaRef;
        protected Panel pnl_uffRef;
        protected HtmlInputHidden hd_systemIdLF;

        //private int indexScript=0;
        private FascicolazioneClassificazione m_classificazioneSelezionata;
        protected System.Web.UI.WebControls.Button Button2;
        protected System.Web.UI.WebControls.TextBox txtNumFasc;
        protected System.Web.UI.WebControls.Button btn_chiudi;
        protected System.Web.UI.WebControls.Image btn_Rubrica;
        protected System.Web.UI.WebControls.Label lblNote;
        protected System.Web.UI.WebControls.TextBox txtNote;

        protected DocsPAWA.DocsPaWR.Corrispondente[] listaCorr;

        //		private void addScript(string scriptBody)
        //		{
        //			indexScript++;
        //			string newScriptName=nameScript+indexScript.ToString();
        //			creaScript(newScriptName,scriptBody);
        //		}

        private void creaScript(string nameScript, string scriptBody)
        {
            try
            {
                //crea funxione script
                string script = "<script language=\"javascript\">" +
                    "function " + nameScript + "(){" + scriptBody + "}</script>";
                Response.Write(script);

                //crea chiamata alla funzione
                storedScriptCall.Add(nameScript);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
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

        private void generaFunctionChiamataScript()
        {
            try
            {
                string script = "<script language=\"javascript\">" +
                    "function esecuzioneScriptUtente()" +
                    "{";

                for (int i = 0; i < storedScriptCall.Count; i++)
                    script += ((string)storedScriptCall[i] + "();");

                script += "}</script>";

                RegisterClientScriptBlock("script_esecuzioneScriptUtente", script);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }
        #endregion

        #region Impostazioni iniziali / Eventi pagina

        private void getParameterUser()
        {
            userRuolo = UserManager.getRuolo(this);
            userReg = UserManager.getRegistroSelezionato(this);
        }


        private bool IsAbilitataRicercaSottoFascicoli()
        {
            if (ConfigSettings.getKey(ConfigSettings.KeysENUM.CERCA_SOTTOFASCICOLI) == null || ConfigSettings.getKey(ConfigSettings.KeysENUM.CERCA_SOTTOFASCICOLI).Equals("0"))
                return false;
            else
                return true;
        }

        private void Page_PreRender(object sender, EventArgs e)
        {
            string Tema = GetCssAmministrazione();
            if (Tema != null && !Tema.Equals(""))
            {
                string[] realTema = Tema.Split('^');
                this.idLinkCss.Href = "../App_Themes/" + realTema[0] + "/" + realTema[0] + ".css";
            }
            else
                this.idLinkCss.Href = "../App_Themes/TemaRosso/TemaRosso.css";

            if (IsAbilitataRicercaSottoFascicoli())
                this.pnl_sottofascicolo.Visible = true;
            else
                this.pnl_sottofascicolo.Visible = false;

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

                this.pnl_uffRef.Visible = true;

                if (use_new_rubrica == "1")
                    this.btn_rubricaRef.Attributes.Add("onClick", "_ApriRubrica('filtri_uffref');");
                else
                    this.btn_rubricaRef.Attributes.Add("onclick", "ApriRubricaUfficioRef('fascUffRef','U');");

                // carico l'ufficio referente selezionato, se esiste
                Corrispondente cor = FascicoliManager.getUoReferenteSelezionato(this);
                if (cor != null)
                {
                    this.txt_cod_UffRef.Text = cor.codiceRubrica;
                    this.txt_desc_uffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(cor);
                    this.hd_systemIdUffRef.Value = cor.systemId;
                }

                FascicoliManager.removeUoReferenteSelezionato(this);

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
                this.Label2.Text = "Data collocaz.";

        }

        private string GetCssAmministrazione()
        {
            string Tema = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
            {
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                DocsPAWA.UserManager userM = new UserManager();
                Tema = utils.InitImagePath.getInstance(idAmm).getPath("CSS");

            }
            else
            {
                if (UserManager.getInfoUtente() != null && !string.IsNullOrEmpty(UserManager.getInfoUtente().idAmministrazione))
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    DocsPAWA.UserManager userM = new UserManager();
                    Tema = utils.InitImagePath.getInstance(idAmm).getPath("CSS");
                }
            }
            return Tema;
        }

        private void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            getParameterUser();
            classSelez = "";

            if (this.Request.QueryString["CodClass"] != null)
            {
                if (!this.Request.QueryString["CodClass"].Equals("undefined"))
                    classSelez = this.Request.QueryString["CodClass"].ToString();
                if (this.Request.QueryString["CodClass"] != "")
                {
                    this.pnl_mostraSottonodi.Visible = true;
                    this.lbl_mostraTuttiFascicoli.Text = "Estendi ricerca a tutti i sottonodi di " + this.Request.QueryString["CodClass"];
                }
                else
                {
                    this.pnl_mostraSottonodi.Visible = false;
                    this.pnl_spazioFinale.Visible = true;
                    this.rbl_MostraTutti.SelectedValue = "N";
                }
            }
            else
            {
                classSelez = "1";
            }

            try
            {
                if (!Page.IsPostBack)
                {
                    l_createFascicolo = false;
                    impostazioniIniziali();
                    caricaComboTitolari();
                    if (UserManager.FunzioneEsistente(this, "FILTRO_FASC_EXCEL"))
                        pnl_filtroExcel.Visible = true;
                    else
                        pnl_filtroExcel.Visible = false;
                }

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
                    Session.Remove("rubrica.listaCorr");
                    FascicoliManager.removeUoReferenteSelezionato(this);
                    FascicoliManager.DO_RemoveIdUO_LF();
                    FascicoliManager.DO_RemoveLocazioneFisica();
                }

                getParametri();
                generaFunctionChiamataScript();

                //Profilazione dinamica fascicoli
                if (!IsPostBack)
                {
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

        private void impostazioniIniziali()
        {
            FascicoliManager.removeAllClassValue(this);
            FascicoliManager.removeClassificazioneSelezionata(this);
        }

        public void SetDefaultButton(Page page, TextBox textControl, Button defaultButton)
        {
            string theScript = @"

				<SCRIPT language=""javascript"">

				<!--
				function fnTrapKD(btn, event){
					if (document.all){
						if (event.keyCode == 9){
							event.returnValue=false;
							event.cancel = true;
							btn.click();
						}
					}
					else if (document.getElementById){
						if (event.which ==9){
							event.returnValue=false;
							event.cancel = true;
							btn.click();
						}
					}
					else if(document.layers){
						if(event.which == 9){
							event.returnValue=false;
							event.cancel = true;
							btn.click();
					}
				}
		}
		// -->
		</SCRIPT>";
            Page.RegisterStartupScript("ForceDefaultToScript", theScript);
            textControl.Attributes.Add("onkeydown", "fnTrapKD(" + defaultButton.ClientID + ",event)");
        }

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
            }

            //Non esistono titolario chiusi
            if (listaTitolari.Count == 1)
            {
                DocsPaWR.OrgTitolario titolario = (DocsPaWR.OrgTitolario)listaTitolari[0];
                ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                ddl_titolari.Items.Add(it);
                ddl_titolari.Enabled = false;
            }

            //Controllo se nel queryLIst è presente un indice per preselezionare il titolario
            if (this.Request.QueryString["IndexTitolario"] != null)
                ddl_titolari.SelectedIndex = Convert.ToInt32(this.Request.QueryString["IndexTitolario"].ToString());
        }
        #endregion

        #region PARAMETRI
        private void getParametri()
        {
            try
            {
                if (!IsPostBack)
                {
                    /* 05/12/2005: commentato poichè non serve caricare la classificazione dei padri in questa pagina
                    buildParametriPagina();*/
                }
                else
                {
                    getParametriPaginaInSession();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void getParametriPaginaInSession()
        {
            try
            {
                m_classificazioneSelezionata = FascicoliManager.getClassificazioneSelezionata(this);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void caricamentoClassificazioniPadri()
        {
            //Vengono estratti i nodi di livello 1 (nodi padri) queryList: S_J_PROJECT__SECURITY
            FascicolazioneClassifica[] classifiche = FascicoliManager.GetFigliClassifica(this, null, UserManager.getUtente(this).idAmministrazione);
            //FascicolazioneClassifica[] classifiche = FascicoliManager.GetFigliClassifica2(this, null, UserManager.getUtente(this).idAmministrazione,ddl_titolari.SelectedValue);
            //Carica la dropDownList di livello 1
            caricaCombo(0, classifiche);
        }
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
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            this.ddl_dataA.SelectedIndexChanged += new System.EventHandler(this.ddl_data_SelectedIndexChanged);
            this.ddl_dataC.SelectedIndexChanged += new System.EventHandler(this.ddl_dataC_SelectedIndexChanged);
            this.ddl_creaz.SelectedIndexChanged += new System.EventHandler(this.ddl_creaz_SelectedIndexChanged);
            this.ddlTipo.SelectedIndexChanged += new System.EventHandler(this.ddlTipo_SelectedIndexChanged);
            this.ddl_data_LF.SelectedIndexChanged += new System.EventHandler(this.ddl_data_LF_SelectedIndexChanged);
            this.txt_varCodRubrica_LF.TextChanged += new System.EventHandler(this.txt_varCodRubrica_LF_TextChanged);
            this.txt_cod_UffRef.TextChanged += new System.EventHandler(this.txt_cod_UffRef_TextChanged);
            this.rbl_MostraTutti.SelectedIndexChanged += new System.EventHandler(this.rbl_MostraTutti_SelectedIndexChanged);
            this.btn_ricFascicoli.Click += new System.EventHandler(this.btn_ricFascicoli_Click);
            this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
            this.ddl_tipoFasc.SelectedIndexChanged += new EventHandler(ddl_tipoFasc_SelectedIndexChanged);
            this.img_dettagliProfilazione.Click += new ImageClickEventHandler(img_dettagliProfilazione_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);
            this.UploadBtn.Click += new EventHandler(UploadBtn_Click);
            this.btn_elimina_excel.Click += new System.Web.UI.ImageClickEventHandler(this.btn_elimina_excel_Click);
            this.ddl_attributo.SelectedIndexChanged += new EventHandler(ddl_attributo_SelectedIndexChanged);
            this.ddl_tipoFascExcel.SelectedIndexChanged += new EventHandler(ddl_tipoFascExcel_SelectedIndexChanged);

        }
        #endregion

        #region Eventi vari

        private bool ricercaFascicoli()
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
                if (ddlStato.SelectedIndex > 0)
                {
                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.STATO.ToString();
                    fV1.valore = ddlStato.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
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
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataA").txt_Data.ID + "').focus() </SCRIPT>";
                        //RegisterStartupScript("focus", s);
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

                #region Note
                if (!this.txtNote.Text.Equals(""))
                {
                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.VAR_NOTE.ToString();
                    fV1.valore = this.txtNote.Text.ToString() + "@-@Q";
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
                    //if (this.txt_desc_uffRef.Text!=null && !this.txt_desc_uffRef.Text.Equals(""))
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

                        //						else//old perchè uff ref è solo interno
                        //						{
                        //							fV1.argomento=DocsPaWR.FiltriFascicolazione.DESC_UO_REF.ToString();
                        //							fV1.valore = DocsPaUtils.Functions.Functions.ReplaceApexes(this.txt_desc_uffRef.Text);
                        //						}
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

                #region sottofascicolo
                if (!this.txt_sottofascicolo.Text.Equals(""))
                {

                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriFascicolazione.SOTTOFASCICOLO.ToString();
                    fV1.valore = this.txt_sottofascicolo.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion

                #region  filtro su elenco valori file excel
                if (Session["fileExcel"] != null && Session["datiExcel"] != null)
                {
                    if (ddl_attributo.SelectedIndex == 0)
                    {
                        Page.RegisterStartupScript("", "<script>alert('Non è stato selezionato nessun attributo');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txtNumFasc.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return false;

                    }
                    else
                    {
                        fV1 = new FiltroRicerca();
                        fV1.argomento = FiltriFascicolazione.FILE_EXCEL.ToString();
                        string[] nomeFile = Session["fileExcel"].ToString().Split('\\');
                        fV1.valore = nomeFile[nomeFile.Length - 1]; ;

                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        if (ddl_attributo.SelectedIndex != 5)
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = FiltriFascicolazione.ATTRIBUTO_EXCEL.ToString();
                            fV1.valore = ddl_attributo.SelectedValue;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        else
                        {
                            fV1 = new FiltroRicerca();
                            fV1.argomento = FiltriFascicolazione.ATTRIBUTO_EXCEL.ToString();
                            fV1.valore = "TIPOLOGIA&" + ddl_tipoFascExcel.SelectedItem.Text + "&" + ddl_tipoFascExcel.SelectedItem.Value + "&" + ddl_attrTipoFascExcel.SelectedItem.Text + "&" + ddl_attrTipoFascExcel.SelectedItem.Value;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }

                        Session.Remove("fileExcel");
                    }
                }
                else
                {
                    if (ddl_attributo.SelectedIndex > 0)
                    {
                        Page.RegisterStartupScript("", "<script>alert('Non è stato selezionato nessun file excel!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txtNumFasc.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return false;

                    }
                }
                #endregion

                qV[0] = fVList;

                FascicoliManager.setFiltroRicFasc(this, qV);
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
                argomento.Equals("CREAZIONE_PRECEDENTE_IL"))
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
            tipoVisualizzazioneFascicoli tipoVis = tipoVisualizzazioneFascicoli.tvfUndefined;

            if (!ricercaFascicoli())
            {
                return;
            }

            switch (this.ddlTipo.SelectedIndex.ToString())
            {
                case "0":
                    tipoVis = tipoVisualizzazioneFascicoli.tvfAll;
                    break;

                case "1":
                    tipoVis = tipoVisualizzazioneFascicoli.tvfGenerali;
                    break;

                case "2":
                    tipoVis = tipoVisualizzazioneFascicoli.tvfProcedimentali;
                    break;

                default:
                    break;
            }

            string showChilds = this.rbl_MostraTutti.SelectedValue;
            //se non viene selezionato nessun nodo di titolario
            //il campo showChilds deve avere valore ='S'
            //per uniformarsi alla ricerca fatta da gestione fascicoli.
            if (classSelez.Equals(""))
                showChilds = "S";

            string newUrl, queryString;

            queryString = QUERYSTRING_tipoVisualizzazione + "=" + tipoVis
                + "&" + QUERYSTRING_codClassifica + "=" + classSelez
                + "&enableFiltriFasc=1&showChilds=" + showChilds
                + "&idTitolario=" + ddl_titolari.SelectedValue;

            newUrl = "../documento/listaFascicoli.aspx?FROM=FiltriRicFasc&" + queryString;

            this.Session.Add("urlRicercaFascicoli", newUrl);
            Response.Write("<SCRIPT>window.returnValue=true;</SCRIPT>");
            Response.Write("<SCRIPT>window.close()</SCRIPT>");
        }

        private bool getMostraTuttiFascicoliValue()
        {
            bool retFunction;

            if (this.rbl_MostraTutti.SelectedItem.Value.Equals("S"))
            {
                retFunction = true;
            }
            else
            {
                retFunction = false;
            }

            return retFunction;
        }

        private void rbl_MostraTutti_SelectedIndexChanged(object sender, EventArgs e)
        {
            allClass = getMostraTuttiFascicoliValue();
            FascicoliManager.setAllClassValue(this, allClass);
            //eseguiRicerca();
        }

        private void enableFascFields()
        {
            //abilitazione campi per la ricerca dei fascicoli
            this.lbl_mostraTuttiFascicoli.Enabled = true;
            this.rbl_MostraTutti.Enabled = true;
            this.btn_ricFascicoli.Enabled = true;
        }

        private void ddl_data_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.GetCalendarControl("txt_fineDataA").txt_Data.Text = "";
            if (this.ddl_dataA.SelectedIndex == 0)
            {
                this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = false;
                this.lbl_finedataA.Visible = false;
                this.lbl_initdataA.Visible = false;
            }
            else
            {
                this.GetCalendarControl("txt_fineDataA").txt_Data.Visible = true;
                this.GetCalendarControl("txt_fineDataA").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataA").Visible = true;
                this.lbl_finedataA.Visible = true;
                this.lbl_initdataA.Visible = true;
            }
        }

        private void ddl_dataC_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.GetCalendarControl("txt_fineDataC").txt_Data.Text = "";
            if (this.ddl_dataC.SelectedIndex == 0)
            {
                this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = false;
                this.lbl_finedataC.Visible = false;
                this.lbl_initdataC.Visible = false;
            }
            else
            {
                this.GetCalendarControl("txt_fineDataC").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataC").txt_Data.Visible = true;
                this.GetCalendarControl("txt_fineDataC").Visible = true;
                this.lbl_finedataC.Visible = true;
                this.lbl_initdataC.Visible = true;
            }
        }

        private void ddl_creaz_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text = "";
            if (this.ddl_creaz.SelectedIndex == 0)
            {
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = false;
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = false;
                this.lbl_finCreaz.Visible = false;
                this.lbl_initCreaz.Visible = false;
            }
            else
            {
                this.GetCalendarControl("txt_fineDataCrea").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Visible = true;
                this.GetCalendarControl("txt_fineDataCrea").Visible = true;
                this.lbl_finCreaz.Visible = true;
                this.lbl_initCreaz.Visible = true;
            }
        }

        private void ddlTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddlTipo.SelectedIndex == 1)
            {
                this.ddlStato.SelectedIndex = 0;
                this.ddlStato.Enabled = false;
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
                //
                this.lbl_finCreaz.Visible = false;
                this.GetCalendarControl("txt_fineDataCrea").txt_Data.Text = "";
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

                this.ddlStato.Enabled = true;
                this.txtNumFasc.ReadOnly = false;
                this.txtNumFasc.BackColor = Color.White;
                this.txtAnnoFasc.ReadOnly = false;
                this.txtAnnoFasc.BackColor = Color.White;
                //data creazione abilitata
                this.ddl_creaz.Enabled = true;
                this.lbl_initCreaz.Visible = false;
                this.GetCalendarControl("txt_initDataCrea").txt_Data.BackColor = Color.White;
                if (enableUfficioRef)//PER I FASCICOLI procedimentali LA RICERCA è ABILITATA
                {
                    //					this.txt_desc_uffRef.Text="";
                    //					this.txt_cod_UffRef.Text="";
                    this.txt_cod_UffRef.ReadOnly = false;
                    this.txt_cod_UffRef.BackColor = Color.White;
                    this.txt_desc_uffRef.BackColor = Color.White;
                    this.btn_rubricaRef.Enabled = true;
                }

            }

        }

        private DropDownList elementByIndex(int indexOfElement)
        {
            DropDownList retValue = null;

            {
                retValue = (DropDownList)this.FindControl("ddl_livello" + (indexOfElement + 1).ToString());
            }
            return retValue;
        }

        private void caricaCombo(int indexCombo, FascicolazioneClassifica[] classifiche)
        {
            DropDownList ddlCombo = elementByIndex(indexCombo);
            if (ddlCombo != null)
            {
                caricaComboClassifica(ddlCombo, classifiche);
            }
        }

        private void caricaComboClassifica(DropDownList ddlDaCaricare, FascicolazioneClassifica[] classifiche)
        {
            if (ddlDaCaricare != null && classifiche != null)
            {
                ddlDaCaricare.Items.Add("");
                for (int i = 0; i < classifiche.Length; i++)
                {
                    FascicolazioneClassifica classifica = classifiche[i];
                    ListItem newItem = new ListItem(classifica.descrizione, classifica.codice);
                    ddlDaCaricare.Items.Add(newItem);
                }
            }
        }

        private void ddl_data_LF_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddl_data_LF.SelectedIndex == 0)
            {
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = false;
                this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = false;
                this.GetCalendarControl("txt_dta_LF_A").Visible = false;
                this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
                this.lbl_dta_LF_DA.Visible = false;
                this.lbl_dta_LF_A.Visible = false;
            }
            else
            {
                this.GetCalendarControl("txt_dta_LF_A").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_dta_LF_A").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dta_LF_A").Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").txt_Data.Visible = true;
                this.GetCalendarControl("txt_dta_LF_DA").Visible = true;
                this.lbl_dta_LF_DA.Visible = true;
                this.lbl_dta_LF_A.Visible = true;
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
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

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
                //this.txt_cod_UffRef.Text = "";
                this.txt_desc_uffRef.Text = "";
                this.hd_systemIdUffRef.Value = "";
            }

        }

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

        private void Button2_Click(object sender, System.EventArgs e)
        {
            try
            {
                enableFascFields();
                eseguiRicerca();
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }

        }

        private void btn_chiudi_Click(object sender, System.EventArgs e)
        {
            Response.Write("<script>window.close();</script>");
        }

        private void btn_ricFascicoli_Click(object sender, System.EventArgs e)
        {
            try
            {
                enableFascFields();
                eseguiRicerca();
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        #endregion

        #region Profilazione dinamica
        private void CaricaComboTipologiaFasc()
        {
            ArrayList listaTipiFasc = new ArrayList(ProfilazioneFascManager.getTipoFascFromRuolo(UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "1", this));

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
        }

        private void verificaCampiPersonalizzati()
        {
            DocsPaWR.Templates template = new DocsPAWA.DocsPaWR.Templates();
            if (!ddl_tipoFasc.SelectedValue.Equals(""))
            {
                template = (DocsPAWA.DocsPaWR.Templates)Session["templateRicerca"];
                if (template == null)
                {
                    template = ProfilazioneFascManager.getTemplateFascById(ddl_tipoFasc.SelectedValue, this);
                    Session.Add("templateRicerca", template);
                }
                if (template != null && !(ddl_tipoFasc.SelectedItem.Text.ToUpper()).Equals(template.DESCRIZIONE.ToUpper()))
                {
                    template = ProfilazioneFascManager.getTemplateFascById(ddl_tipoFasc.SelectedValue, this);
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

        #region Filtro foglio Excel
        private void ddl_tipoFascExcel_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.pnl_attrTipoFascExcel.Visible = false;
            Templates template = ProfilazioneFascManager.getAttributiTipoFasc(UserManager.getInfoUtente(this), ddl_tipoFascExcel.SelectedValue, this);
            if (template != null)
            {
                ddl_attrTipoFascExcel.Items.Clear();
                this.pnl_attrTipoFascExcel.Visible = true;

                for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                {
                    ListItem item_1 = new ListItem();
                    item_1.Value = template.ELENCO_OGGETTI[i].SYSTEM_ID.ToString();
                    item_1.Text = template.ELENCO_OGGETTI[i].DESCRIZIONE;
                    ddl_attrTipoFascExcel.Items.Add(item_1);
                }
            }
        }

        private void ddl_attributo_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            pnl_tipoFascExcel.Visible = false;
            pnl_attrTipoFascExcel.Visible = false;
            if (ddl_attributo.SelectedIndex == 5 && ddl_tipoFasc.Items.Count > 1)
            {
                pnl_tipoFascExcel.Visible = true;
                ddl_tipoFascExcel.Items.Clear();
                ArrayList listaTipiFasc = new ArrayList(ProfilazioneFascManager.getTipoFascFromRuolo(UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "1", this));
                ListItem item = new ListItem();
                item.Value = "";
                item.Text = "";
                ddl_tipoFascExcel.Items.Add(item);
                for (int i = 0; i < listaTipiFasc.Count; i++)
                {
                    DocsPaWR.Templates templates = (DocsPaWR.Templates)listaTipiFasc[i];
                    ListItem item_1 = new ListItem();
                    item_1.Value = templates.SYSTEM_ID.ToString();
                    item_1.Text = templates.DESCRIZIONE;
                    if (templates.IPER_FASC_DOC == "1")
                        ddl_tipoFascExcel.Items.Insert(1, item_1);
                    else
                        ddl_tipoFascExcel.Items.Add(item_1);
                }
            }
        }

        private void UploadBtn_Click(object sender, EventArgs e)
        {
            //Verfica che il controllo uploadedFile contenga un file 
            if (this.uploadedFile.HasFile)
            {
                string fileName = Server.HtmlEncode(uploadedFile.FileName);
                string extension = System.IO.Path.GetExtension(fileName);

                if (extension == ".xls")
                {
                    Session.Add("fileExcel", uploadedFile.PostedFile.FileName);
                    HttpPostedFile p = uploadedFile.PostedFile;
                    Stream fs = p.InputStream;
                    byte[] dati = new byte[fs.Length];
                    fs.Read(dati, 0, (int)fs.Length);
                    fs.Close();
                    Session.Add("DatiExcel", dati);
                    this.lbl_fileExcel.Text = "File correttamente caricato.            Elimina ";
                    this.btn_elimina_excel.Visible = true;
                    this.btn_elimina_excel.Attributes.Add("onclick", "if (!window.confirm('Sei sicuro di voler eliminare il file excel selezionato?')) {return false};");
                }
                else
                {
                    Page.RegisterStartupScript("", "<script>alert('Selezionare solo file .xls!');</script>");
                }
            }
        }

        protected void btn_elimina_excel_Click(object sender, System.EventArgs e)
        {
            this.lbl_fileExcel.Text = "Nessun file excel caricato.";
            this.btn_elimina_excel.Visible = false;
            Session.Remove("fileExcel");
            Session.Remove("DatiExcel");
        }

        #endregion
    }
}
