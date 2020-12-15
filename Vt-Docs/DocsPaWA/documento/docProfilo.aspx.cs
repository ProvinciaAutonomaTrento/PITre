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
using System.IO;
using System;
using DocsPAWA.DocsPaWR;
using System.Configuration;
using DocsPAWA.utils;
using log4net;

namespace DocsPAWA.documento
{
    /// <summary>
    /// Summary description for docProfilo.
    /// </summary>
    public class docProfilo : DocsPAWA.CssPage
    {

        /*
         * Andrea
         */
        private ArrayList listaExceptionTrasmissioni = new ArrayList();
        private string messError = "";
        /*
         * End Andrea
         */

        private ILog logger = LogManager.GetLogger(typeof(docProfilo));
        protected ScriptManager ScriptManager;
        protected System.Web.UI.WebControls.TextBox lbl_dataCreazione;
        protected DocsPaWebCtrlLibrary.ImageButton btn_oggettario;
        protected DocsPaWebCtrlLibrary.ImageButton btn_selezionaParoleChiave;
        protected DocsPaWebCtrlLibrary.ImageButton btn_log;
        protected System.Web.UI.WebControls.ListBox lbx_paroleChiave;
        //protected System.Web.UI.WebControls.TextBox txt_oggetto;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoAtto;
        protected DocsPaWebCtrlLibrary.ImageButton btn_rimuovi;
        protected DocsPaWebCtrlLibrary.ImageButton btn_riproponi;
        protected DocsPaWebCtrlLibrary.ImageButton btn_stampa;
        protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungi;
        protected DocsPaWebCtrlLibrary.ImageButton btn_prodisponiProtocollo;
        protected DocsPaWebCtrlLibrary.ImageButton btn_salva;
        protected DocsPaWebCtrlLibrary.ImageButton btn_visibilita;
        protected System.Web.UI.WebControls.TextBox txt_docNumber;
        protected System.Web.UI.WebControls.Label ldl_docNumber;
        protected System.Web.UI.WebControls.CheckBox chkPrivato;
        protected System.Web.UI.WebControls.CheckBox chkUtente;
        //protected System.Web.UI.WebControls.Label lbl_ADL;
        protected System.Web.UI.WebControls.Panel panel_numOgg_commRef;
        protected System.Web.UI.WebControls.TextBox txt_numOggetto;
        protected DocsPaWebCtrlLibrary.ImageButton btn_eliminaParoleChiave;
        protected System.Web.UI.WebControls.TextBox txt_NomeFirma;
        protected System.Web.UI.WebControls.ListBox lbx_firmatari;
        protected System.Web.UI.HtmlControls.HtmlTable Table2;
        protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungiFirmatario;
        protected DocsPaWebCtrlLibrary.ImageButton btn_cancFirmatario;
        protected System.Web.UI.WebControls.Panel panel_Firm;
        protected System.Web.UI.WebControls.Label lbl_Nome_F;
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.TextBox txt_CognomeFirma;
        protected System.Web.UI.WebControls.RadioButtonList rbl_sesso;
        protected System.Web.UI.WebControls.DropDownList ddl_eta;
        protected System.Web.UI.WebControls.Panel pnl_sessoEta;
        protected System.Web.UI.WebControls.DropDownList ddl_commRef;
        protected DocsPaWebCtrlLibrary.ImageButton btn_modificaOgget;
        protected DocsPaWebCtrlLibrary.ImageButton btn_storiaOgg;
        protected System.Web.UI.WebControls.Button btnNote;
        protected Note.DettaglioNota dettaglioNota;
        protected DocsPaWebCtrlLibrary.ImageButton btn_storiaCons;
        protected System.Web.UI.WebControls.Image imgTipoAlleg;
        //my var
        private DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;
        protected DocsPaWebCtrlLibrary.ImageButton btn_addTipoAtto;
        protected System.Web.UI.HtmlControls.HtmlInputHidden h_tipoAtto;
        protected System.Web.UI.WebControls.ImageButton btn_CampiPersonalizzati;
        protected DocsPaWebCtrlLibrary.ImageButton btn_salva_disabled;
        protected System.Web.UI.WebControls.Label lbl_statoAttuale;
        protected System.Web.UI.WebControls.DropDownList ddl_statiSuccessivi;
        protected System.Web.UI.WebControls.Panel Panel_DiagrammiStato;
        protected System.Web.UI.WebControls.Panel Panel_DataScadenza;
        protected DocsPaWebCtrlLibrary.DateMask txt_dataScadenza;
        protected DocsPaWebCtrlLibrary.ImageButton img_btnStoriaDiagrammi;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txt_confirmStatoFinale;
        protected System.Web.UI.HtmlControls.HtmlInputHidden confirmStatoFinale;
        protected Utilities.MessageBox MessageBox;
        protected Utilities.MessageBox MessageBox1;
        protected Utilities.MessageBox msg_TrasmettiDoc;
        protected Utilities.MessageBox msg_PersonaleDoc;
        protected System.Web.UI.WebControls.Panel pnl_trasm_rapida;
        protected System.Web.UI.WebControls.DropDownList ddl_tmpl;
        protected DocsPaWebService wws = new DocsPaWebService();
        protected System.Web.UI.WebControls.TextBox txt_CodFascicolo;
        protected System.Web.UI.WebControls.TextBox txt_DescFascicolo;
        protected System.Web.UI.WebControls.Panel pnl_fasc_rapida;
        protected System.Web.UI.HtmlControls.HtmlTableRow tr_tipologia;
        protected DocsPaWebCtrlLibrary.ImageButton imgFasc;
        protected static string separatore = "----------------";
        protected string codClassifica = "";
        protected DocsPaWebCtrlLibrary.ImageButton imgDescOgg;
        protected DocsPaWebCtrlLibrary.ImageButton btn_Correttore;
        protected DocsPAWA.documento.Oggetto ctrl_oggetto;
        private bool isFascRapidaRequired = false;
        protected System.Web.UI.WebControls.Label labelFascRapid;
        protected System.Web.UI.HtmlControls.HtmlInputHidden isFascRequired;
        protected DocsPaWebCtrlLibrary.ImageButton btn_inoltra;
        protected System.Web.UI.WebControls.ImageButton btn_stampaSegn;
        protected System.Web.UI.WebControls.Panel pnl_star;
        //		/// <summary>
        //		/// Se true abilita l'editing dell'oggetto del documento 
        //		/// </summary>
        //		bool enableSubjectEditing = false;

        protected DocsPaWebCtrlLibrary.ImageButton btnGoToDocumentoPrincipale;
        protected TextBox txtDocumentoPrincipale;
        protected System.Web.UI.HtmlControls.HtmlTableRow trDocumentoPrincipale;
        protected bool isRiproposto;
        protected System.Web.UI.WebControls.ImageButton Help;
        protected System.Web.UI.WebControls.TextBox txt_codModello;
        //CATENE
        protected System.Web.UI.WebControls.Panel rispProtoPanelGrigio;
        protected DocsPaWebCtrlLibrary.ImageButton btn_in_risposta_a;
        protected DocsPaWebCtrlLibrary.ImageButton btn_Risp;
        protected DocsPaWebCtrlLibrary.ImageButton btn_risp_dx;
        protected DocsPaWebCtrlLibrary.ImageButton btn_risp_sx;
        protected System.Web.UI.WebControls.Panel pnl_text_risposta;
        protected System.Web.UI.WebControls.TextBox txt_RispostaDocGrigio;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] ListaFiltri;

        protected System.Web.UI.WebControls.HiddenField abilitaModaleVis;
        protected System.Web.UI.WebControls.HiddenField estendiVisibilita;

        protected System.Web.UI.WebControls.Panel pnl_protocolloTitolario;
        protected System.Web.UI.WebControls.Label lbl_etProtTitolario;
        protected System.Web.UI.WebControls.Label lbl_txtProtTitolario;
        protected DocsPaWebCtrlLibrary.ImageButton btn_titolario;

        protected int numeroRuoliDestInTrasmissione = 0;
        protected int numeroUtentiConNotifica = 0;
        protected string idPeopleNewOwner = string.Empty;
        protected Utilities.MessageBox msg_copiaDoc;

        protected DocsPaWebCtrlLibrary.ImageButton btn_risp_grigio;
        protected DocsPaWebCtrlLibrary.ImageButton imgNewFasc;

        protected System.Web.UI.WebControls.Panel pnl_fasc_Primaria;
        protected System.Web.UI.WebControls.Label lbl_fasc_Primaria;

        protected System.Web.UI.WebControls.Label lblStatoConsolidamento;
        protected DocsPAWA.UserControls.DocumentConsolidation documentConsolidationCtrl;
        protected DocsPaWebCtrlLibrary.ImageButton btn_delTipologyDoc;
        protected Utilities.MessageBox msg_rimuoviTipologia;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            logger.Info("BEGIN");
            //Inizializzazione user control Oggetto
            ctrl_oggetto = this.GetControlOggetto();
            //Attivo la ricerca sul codice oggetto
            ctrl_oggetto.cod_oggetto_postback = true;
            //Imposto l'aspetto del controllo oggetto
            ctrl_oggetto.DimensioneOggetto("default", "profilo");

            this.InitializeControlConsolidation();
            this.MaintainScrollPositionOnPostBack = true;
            if (Session["docRiproposto"] != null && Session["docRiproposto"].ToString().ToLower().Equals("true"))
                isRiproposto = true;

            if (!IsPostBack)
            {
                Session.Remove("refreshDxPageProf");
            }
            Response.Expires = 0;
            Utils.startUp(this);
            Session.Remove("validCodeFasc");
            this.RegisterClientScript("nascondi", "nascondi();");
            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'Y');");

            string isNew = Request.QueryString["isNew"];
            if (isNew == "1")
            {
                this.btn_log.Visible = false;

            }

            if (isNew == "1" && !IsPostBack)
            {
                schedaDocumento = DocumentManager.getDocumentoInLavorazione();
                bool onRepositoryContext = (schedaDocumento != null && schedaDocumento.repositoryContext != null);

                if (Session["docRiproposto"] == null && !onRepositoryContext)
                {
                    DocumentManager.removeDocumentoSelezionato(this);
                    DocumentManager.removeDocumentoInLavorazione(this);
                    FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                    Session.Remove("Modello");
                }
            }

            string valoreChiaveFasc = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");

            if (valoreChiaveFasc != null && valoreChiaveFasc.ToUpper().Equals("TRUE"))
                this.isFascRapidaRequired = true;
            else
                this.isFascRapidaRequired = false;

            if (isFascRapidaRequired)
            {
                this.isFascRequired.Value = "true";
                this.labelFascRapid.Text = "Fascicolazione Rapida *";
            }
            else
            {
                this.isFascRequired.Value = "false";
                this.labelFascRapid.Text = "Fascicolazione Rapida";
            }

            if (!this.Page.IsPostBack)
            {
                this.ctrl_oggetto.oggetto_SetControlFocus();
                loadFormControls();
            }




            try
            {
                schedaDocumento = DocumentManager.getDocumentoInLavorazione();

                if (schedaDocumento != null && schedaDocumento.protocollo != null)
                {
                    this.btn_riproponi.Visible = false;
                    this.btn_prodisponiProtocollo.Visible = false;
                }

                if (schedaDocumento != null && schedaDocumento.protocollo != null && schedaDocumento.protocollo.daProtocollare.Equals("1"))
                    this.btn_riproponi.Visible = false;

                this.btn_inoltra.Visible = false;
                if (System.Configuration.ConfigurationManager.AppSettings["INOLTRA_DOC"] != null && System.Configuration.ConfigurationManager.AppSettings["INOLTRA_DOC"] == "1")
                {
                    if (schedaDocumento != null && schedaDocumento.tipoProto == "G")
                        this.btn_inoltra.Visible = true;
                }

                //abilito il pulsante di creazione diretta dei fascicoli procedimentali
                if (utils.InitConfigurationKeys.GetValue("0", "FE_NUOVO_FASC_DIRECT") != null && utils.InitConfigurationKeys.GetValue("0", "FE_NUOVO_FASC_DIRECT").Equals("1"))
                {
                    this.imgNewFasc.Visible = true;
                }

                // Inizializzazione controllo verifica acl
                if ((schedaDocumento != null) && (schedaDocumento.inCestino != "1") && (schedaDocumento.systemId != null))
                {
                    this.InitializeControlAclDocumento();
                }
                if (schedaDocumento == null)
                {
                    nuovaSchedaDocumento();
                }
                //int lungh = schedaDocumento.paroleChiave.Length;

                if (this.Page.IsPostBack && (!this.h_tipoAtto.Value.Equals("") && !this.h_tipoAtto.Value.Equals("N")))
                {
                    CaricaComboTipologiaAtto(this.ddl_tipoAtto);
                    this.h_tipoAtto.Value = "N";
                }

                if (!IsPostBack)
                {
                    caricaModelliTrasm();
                    caricaComboTemplateProfilo();

                    //Abilito il pulsante di protocollo in risposta grigio
                    if (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
                        && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1")
                    {
                        this.btn_risp_grigio.Visible = true;
                    }

                    //Emanuela: gestione per l'apertura di documenti di tipo fatturazione.
                    if (schedaDocumento != null && schedaDocumento.autore != null)
                    {
                        DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                        DocsPAWA.DocsPaWR.Utente user = ws.getUtenteById(schedaDocumento.autore);
                        if (user.userId.ToUpper().Contains("TIBCO"))
                        {
                            btn_prodisponiProtocollo.Enabled = false;
                            string accessRights = "";
                            string idGruppoTrasm = "";
                            string tipoDiritto = "";
                            ws.SelectSecurity(schedaDocumento.systemId, schedaDocumento.autore, "", out accessRights, out idGruppoTrasm, out tipoDiritto);
                            if (!string.IsNullOrEmpty(accessRights))
                                ((ImageButton)FindControl("btn_salva")).AlternateText = "Salva e acquisisci diritti";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }


            if (schedaDocumento != null && schedaDocumento.systemId != null && !schedaDocumento.systemId.Equals(""))
            {
                this.ctrl_oggetto.oggetto_isReadOnly = false;
                this.btn_oggettario.Enabled = true;
                // se è protocollato bisogna disabiltare la mod da profilo
                if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.segnatura != null)
                {
                    this.ctrl_oggetto.oggetto_isReadOnly = true;
                    this.btn_oggettario.Enabled = false;
                }
            }

            //PROFILAZIONE DINAMICA
            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                //Disabilito il pulsante che permette da questa pagina l'inserimento di una nuova tipologia di atto
                btn_addTipoAtto.Visible = false;

                if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.segnatura != null && !schedaDocumento.protocollo.segnatura.Equals(""))
                    ddl_tipoAtto.Enabled = false;

                if (!IsPostBack)
                    if (Session["templateRiproposto"] == null)
                        Session.Remove("template");
            }
            else
            {
                btn_CampiPersonalizzati.Visible = false;
            }
            //FINE PROFILAZIONE DINAMICA
            // Inizializzazione dati relativi al documento principale
            this.InitializeDataDocumentoPrincipale();

            //Nodo titolario scelto
            DocsPAWA.DocsPaWR.FascicolazioneClassificazione classificazione = DocumentManager.getClassificazioneSelezionata(this);
            if (classificazione != null)
            {
                txt_CodFascicolo.Text = classificazione.codice;
                this.txt_CodFascicolo_TextChanged(sender, e);
                /*
                this.txt_CodFascicolo.Text = classificazione.codice;
                //this.txt_DescFascicolo.Text = classificazione.descrizione;
                Fascicolo Fasc = getFascicolo(classificazione.codice);
                FascicoliManager.setFascicoloSelezionatoFascRapida(Fasc);
                DocumentManager.setClassificazioneSelezionata(this, null);
                */
            }

            tastoInvio();

            //Gestione bottone freccia destra in risposta
            if (System.Configuration.ConfigurationManager.AppSettings["NON_VISUALIZZA_FRECCIA_RISPOSTA"] != null && System.Configuration.ConfigurationManager.AppSettings["NON_VISUALIZZA_FRECCIA_RISPOSTA"] == "1")
            {
                this.btn_risp_dx.Visible = false;
            }

            //Prova Andrea Messaggio errore per Trasmissioni
            if (Session["MessError"] != null)
            {
                messError = Session["MessError"].ToString();
                //Response.Write("<script language=\"javascript\">alert('Trasmissioni avvenute con successo per tutti i destinatari ad eccezione delle seguenti: \\n" + messError + "\\n');</script>");
                Response.Write("<script language=\"javascript\">alert('Trasmissioni con esito negativo: \\n" + messError + "\\n');</script>");
                Session.Remove("MessError");
            }
            //End Andrea
            
            logger.Info("END");
        }
        //    //Protocollo Titolario
        //    if (!IsPostBack)
        //    {
        //        string protocolloTitolario = DocumentManager.GetProtocolloTitolario(schedaDocumento);
        //        if (!string.IsNullOrEmpty(protocolloTitolario))
        //        {
        //            pnl_protocolloTitolario.Visible = true;
        //            lbl_etProtTitolario.Text = wws.isEnableProtocolloTitolario();
        //            lbl_txtProtTitolario.Text = protocolloTitolario;
        //        }
        //    }

        //    // Inizializzazione dati relativi al documento principale
        //    this.InitializeDataDocumentoPrincipale();

        //    tastoInvio();
        //}

        #region GESTIONE TEMPLATE TRASMISSIONE E MODELLI TRASMISSIONE

        private void caricaComboTemplateProfilo()
        {

            Session.Remove("doc_profilo.tmplTrasm");
            DocsPaWR.TemplateTrasmissione[] listaTmp;

            listaTmp = TrasmManager.getListaTemplate(this, UserManager.getUtente(this), UserManager.getRuolo(this), "D");

            if (listaTmp != null && listaTmp.Length > 0)
            {
                if (ddl_tmpl.Items.Count == 0)
                    ddl_tmpl.Items.Add(" "); // valore vuoto;

                Session["doc_profilo.tmplTrasm"] = listaTmp;

                for (int i = 0; i < listaTmp.Length; i++)
                {
                    System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                    li.Text = listaTmp[i].descrizione;
                    li.Value = listaTmp[i].systemId;
                    ddl_tmpl.Items.Add(li);
                }
            }
        }

        private void caricaModelliTrasm()
        {
            logger.Info("BEGIN");
            string idAmm = UserManager.getRegistroSelezionato(this).idAmministrazione;
            string idRegistro = UserManager.getRegistroSelezionato(this).systemId;
            string idPeople = UserManager.getInfoUtente(this).idPeople;
            string idCorrGlobali = UserManager.getInfoUtente(this).idCorrGlobali;
            string idRuoloUtente = UserManager.getInfoUtente(this).idGruppo;
            string idTipoDoc = "";
            string idDiagramma = "";
            string idStato = "";

            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                if (schedaDocumento.tipologiaAtto != null)
                {
                    //DocsPaWR.Templates template = (Templates)Session["template"];
                    //if(template == null)
                    //    template = wws.getTemplate(idAmm, schedaDocumento.tipologiaAtto.descrizione, schedaDocumento.docNumber);

                    //if (template != null)
                    //{
                    //    idTipoDoc = template.SYSTEM_ID.ToString();

                    if (schedaDocumento.template != null && schedaDocumento.template.SYSTEM_ID.ToString() != "" && !isRiproposto)
                    {
                        idTipoDoc = schedaDocumento.template.SYSTEM_ID.ToString();

                        if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                        {
                            DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoDoc(schedaDocumento.tipologiaAtto.systemId, idAmm, this);
                            if (dg != null)
                            {
                                idDiagramma = dg.SYSTEM_ID.ToString();
                                DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
                                //controllo che il ruolo corrente abbia visibilità dello stato
                                if (stato != null && 
                                    DiagrammiManager.IsRuoloAssociatoStatoDia(idDiagramma, UserManager.getRuolo().idGruppo, stato.SYSTEM_ID.ToString()))
                                    idStato = stato.SYSTEM_ID.ToString();
                            }
                        }
                    }
                }
            }

            bool allReg = true;
            if (this.schedaDocumento != null && this.schedaDocumento.tipoProto != "G" && this.schedaDocumento.protocollo != null)
                allReg = false;

            //ArrayList idModelli = new ArrayList(ModelliTrasmManager.getModelliPerTrasm(idAmm, ((Ruolo)UserManager.getRuolo(this)).registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, "D", this, this.schedaDocumento.systemId, idRuoloUtente, allReg, schedaDocumento.accessRights));
            ArrayList idModelli = new ArrayList(ModelliTrasmManager.getModelliPerTrasmLite(idAmm, ((Ruolo)UserManager.getRuolo(this)).registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, "D", this, this.schedaDocumento.systemId, idRuoloUtente, allReg, schedaDocumento.accessRights));
            
            if (ddl_tmpl.Items.Count == 0)
            {
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                li.Text = " ";
                ddl_tmpl.Items.Add(li);
            }

            for (int i = 0; i < idModelli.Count; i++)
            {
                DocsPaWR.ModelloTrasmissione mod = (DocsPAWA.DocsPaWR.ModelloTrasmissione)idModelli[i];
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                li.Value = mod.SYSTEM_ID.ToString();
                li.Text = mod.NOME;
                if (System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CODICE_MODELLI_TRASM"] != null && System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CODICE_MODELLI_TRASM"] == "1")
                    li.Text += " (" + mod.CODICE + ")";
                ddl_tmpl.Items.Add(li);
            }

            if (idModelli.Count > 0)
            {
                ddl_tmpl.Items.Add(separatore);
            }

            if (isRiproposto)
                if (Session["Modello"] != null)
                {
                    DocsPaWR.ModelloTrasmissione modello = (DocsPaWR.ModelloTrasmissione)Session["Modello"];
                    this.ddl_tmpl.SelectedValue = modello.SYSTEM_ID.ToString();
                    this.txt_codModello.Text = modello.CODICE;
                }
            logger.Info("END");
        }

        #region OLD
        //		private DocsPAWA.DocsPaWR.TemplateTrasmissione getTemplateTrasmissioneProfilo()
        //		{
        //			DocsPaWR.TemplateTrasmissione[] listaTmp;
        //			DocsPaWR.TemplateTrasmissione template = null;
        //			
        //			if ((ddl_tmpl.SelectedIndex > 0) && !(ddl_tmpl.SelectedValue.Equals("----------------")))
        //			{
        //				listaTmp=(DocsPAWA.DocsPaWR.TemplateTrasmissione[]) (Session["doc_profilo.tmplTrasm"]);
        //				
        //				if (listaTmp != null && listaTmp.Length>0)
        //					template = (DocsPAWA.DocsPaWR.TemplateTrasmissione)listaTmp[ddl_tmpl.SelectedIndex-1];
        //			
        //			}
        //			
        //			return template;
        //		}
        #endregion

        #endregion

        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        private void CaricaComboTipologiaAtto(DropDownList ddl)
        {
            logger.Info("BEGIN");
            SchedaDocumento sch = DocumentManager.getDocumentoSelezionato();

            if (sch != null
                && sch.documentoPrincipale != null)
            {
                tr_tipologia.Visible = false;

            }
            else
            {
                tr_tipologia.Visible = true;
                DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
                    listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "2");
                else
                    listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);

                ddl.Items.Clear();
                ddl.Items.Add("");
                if (listaTipologiaAtto != null)
                {
                    for (int i = 0; i < listaTipologiaAtto.Length; i++)
                    {
                        ddl.Items.Add(listaTipologiaAtto[i].descrizione);
                        ddl.Items[i + 1].Value = listaTipologiaAtto[i].systemId;
                    }
                }
            }
            logger.Info("END");
        }

        private void loadFormControls()
        {
            CaricaComboTipologiaAtto(this.ddl_tipoAtto);
            if (!UserManager.getStringaConfigurazione(this.Page, true).Equals(""))
            {
                this.pnl_sessoEta.Visible = true;
            }
            else
            {
                this.pnl_sessoEta.Visible = false;
            }

            if (!UserManager.getStringaConfigurazione(this.Page, true).Equals(""))
                this.panel_Firm.Visible = true;
            else
                this.panel_Firm.Visible = false;
        }

        protected void setFormProperties()
        {
            //verifico l'abilitazione a creare nuovi fascicoli
            if (UserManager.ruoloIsAutorized(this, "FASC_NUOVO"))
            {
                this.imgNewFasc.Enabled = true;
            }
            else
            {
                this.imgNewFasc.Enabled = false;
            }
            //this.btn_selezionaParoleChiave.Attributes.Add("onclick","ApriFinestraParoleChiave('docProf');return false;");
            this.btn_oggettario.Attributes.Add("onclick", "ApriOggettario('doc_Prof');return false;");
            this.btn_visibilita.Attributes.Add("onclick", "ApriFinestraVisibilita();return false;");
            this.btn_storiaOgg.Attributes.Add("onclick", "ApriFinestraStoriaMod('oggetto');return false;");

            bool isAllegato = (schedaDocumento.documentoPrincipale != null);
            if (isAllegato)
            {
                // Se allegato, impostazione della microfunzione per la rimozione dell'allegato
                this.btn_rimuovi.Tipologia = "DO_ALL_RIMUOVI";
                this.btn_rimuovi.Attributes.Add("onclick", "return ApriFinestraRimuoviAllegato();");
            }
            else
            {
                this.btn_rimuovi.Tipologia = "DO_PRO_RIMUOVI";
                this.btn_rimuovi.Attributes.Add("onclick", "return ApriFinestraRimuoviProfilo('docProfilo');");
            }


            //abilito o disabilito i bottoni
            if (isAllegato && schedaDocumento.inCestino == "1")
            {
                string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
                this.Response.Write(script);
            }


            if ((schedaDocumento.systemId == null || schedaDocumento.inCestino == "1")
                  || (schedaDocumento.inArchivio != null && schedaDocumento.inArchivio == "1")
                || (isAllegato && schedaDocumento.inCestino == "1"))
            {
                this.btn_aggiungi.Enabled = false;
                this.btn_rimuovi.Enabled = false;
                this.btn_riproponi.Enabled = false;
                this.btn_prodisponiProtocollo.Enabled = false;
                this.btn_visibilita.Enabled = false;
                this.btn_stampa.Enabled = false;
                this.btn_modificaOgget.Enabled = false;
                this.btn_storiaOgg.Enabled = false;
            }
            else
            {
                this.btn_aggiungi.Enabled = true;
                this.btn_visibilita.Enabled = !isAllegato;

                if (schedaDocumento.docNumber != null)
                {
                    this.chkPrivato.Enabled = false;
                    this.chkUtente.Enabled = false;
                    if (schedaDocumento.documentoPrincipale == null) //per allegati no RIproponi!!
                        this.btn_riproponi.Enabled = true;
                    else
                        this.btn_riproponi.Enabled = false;
                }
                else
                    this.btn_riproponi.Enabled = false;

                this.btn_modificaOgget.Enabled = true;
                this.btn_storiaOgg.Enabled = true;

                this.btn_salva.Enabled = (schedaDocumento.tipoProto != "R" && schedaDocumento.tipoProto != "C");
                this.btn_prodisponiProtocollo.Enabled = (schedaDocumento.protocollo == null && !isAllegato && schedaDocumento.tipoProto != "R" && schedaDocumento.tipoProto != "C");

                if (schedaDocumento != null && schedaDocumento.autore != null && this.btn_prodisponiProtocollo.Enabled)
                {
                    DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                    DocsPAWA.DocsPaWR.Utente user = ws.getUtenteById(schedaDocumento.autore);
                    if (user.userId.ToUpper().Contains("TIBCO"))
                    {
                        btn_prodisponiProtocollo.Enabled = false;
                    }
                }

                if (schedaDocumento.protocollo != null)
                {
                    this.btn_prodisponiProtocollo.Enabled = !isAllegato;

                    if ((schedaDocumento.protocollo.numero != null && !schedaDocumento.protocollo.numero.Equals("")))
                    {
                        this.btn_rimuovi.Enabled = false;
                    }

                    // controllo se il documento è annullato - in tal caso disabilito tutto
                    //annullamento
                    if (schedaDocumento.protocollo != null)
                    {
                        DocsPaWR.ProtocolloAnnullato protAnnull;
                        protAnnull = schedaDocumento.protocollo.protocolloAnnullato;
                        if (protAnnull != null && protAnnull.dataAnnullamento != null && !protAnnull.Equals(""))
                        {
                            this.btn_aggiungi.Enabled = false;
                            this.btn_rimuovi.Enabled = false;
                            this.btn_modificaOgget.Enabled = false;
                            this.btn_salva.Enabled = false;
                            this.btn_stampa.Enabled = false;

                            this.btn_selezionaParoleChiave.Enabled = false;
                            this.btn_oggettario.Enabled = false;
                            this.btn_modificaOgget.Enabled = false;
                            this.btn_aggiungi.Enabled = false;
                            this.btn_eliminaParoleChiave.Enabled = false;
                            this.btn_addTipoAtto.Enabled = false;

                            this.ddl_tipoAtto.Enabled = false;
                        }
                    }
                }

                //se non si tratta di un nuovo documento
                if (schedaDocumento != null && schedaDocumento.documenti != null && (!string.IsNullOrEmpty(schedaDocumento.documenti[0].versionId)))
                {
                    //visualizzo l'icona associata all'allegato (allegato pec/all utente)
                    if (isAllegato)
                    {
                        string isAllPec = DocumentManager.AllegatoIsPEC(schedaDocumento.documenti[0].versionId);
                        if (isAllPec.Equals("1"))
                        { 
                            imgTipoAlleg.Visible = true;
                            imgTipoAlleg.ImageUrl = "~/images/allegato_pec.jpg";
                            imgTipoAlleg.ToolTip = "documento spedito tramite la posta elettronica certificata";
                        }
                        else
                        {
                            if (DocumentManager.AllegatoIsIS(schedaDocumento.documenti[0].versionId).Equals("1"))
                            {
                                imgTipoAlleg.Visible = true;
                                imgTipoAlleg.ImageUrl = "~/images/ico_allegato_PI3.jpg";
                                imgTipoAlleg.ToolTip = "documento spedito tramite interoperabilità PITRE";
                            }
                            else
                            {
                                if (DocumentManager.AllegatoIsEsterno(schedaDocumento.documenti[0].versionId).Equals("1"))
                                {
                                    imgTipoAlleg.Visible = true;
                                    imgTipoAlleg.ImageUrl = "~/images/allegato_esterno.jpg";
                                    imgTipoAlleg.ToolTip = "documento inserito da sistemi esterni";
                                }
                                else
                                {
                                    imgTipoAlleg.Visible = true;
                                    imgTipoAlleg.ImageUrl = "~/images/ico_allegato_disabled.gif";
                                    imgTipoAlleg.ToolTip = "documento inserito dall'utente";
                                }
                            }
                        }
                    }
                }
            }


            #region rispostaDocGrigio

            if (schedaDocumento != null && schedaDocumento.tipoProto != null)
            {
                DocsPAWA.DocsPaWR.InfoDocumento infoDocRisposta = GetDocInRisposta(schedaDocumento.systemId);
                if (infoDocRisposta != null)
                {

                    this.btn_risp_dx.AlternateText = "visualizza documento in risposta al " + this.schedaDocumento.docNumber;
                    //DocumentManager.setRisultatoRicerca(this, infoDocRisposta);
                    //this.btn_risp_dx.Attributes.Add("onclick", "VaiRispostaDocGrigio (); return false");
                }
                else
                {
                    /* ABBATANGELI GIANLUIGI
                    * Verifico il caso in cui ci siano più di un 
                    * documento (protocollo,doc grigio o predisposto) in risposta al protocollo */
                    if (GetCountDocInRisposta(schedaDocumento.systemId) > 1)
                    {
                        this.btn_risp_dx.AlternateText = "visualizza elenco documenti in risposta al " + this.schedaDocumento.docNumber;
                        this.btn_risp_dx.Attributes.Add("onclick", "ShowDialogRispostaDocGrigio ('" + this.schedaDocumento.systemId + "','" + this.schedaDocumento.tipoProto + "'); return false");

                        this.btn_risp_dx.Enabled = true;
                        this.btn_risp_dx.Visible = true;
                        this.btn_visibilita.Enabled = true;
                    }
                    else 
                    {
                        this.btn_risp_dx.Enabled = true;
                        this.btn_risp_dx.Visible = true;
                        this.btn_risp_dx.Attributes.Add("onclick", "alert ('Non ci sono documento in risposta.');");
                    }
                }

                if (schedaDocumento.rispostaDocumento != null)
                {
                    this.btn_risp_sx.AlternateText = "vai al documento " + this.schedaDocumento.rispostaDocumento.docNumber;
                }
                else
                {
                    this.btn_risp_sx.Enabled = true;
                    //this.btn_risp_sx.Visible = true;
                    this.btn_risp_sx.Attributes.Add("onclick", "alert ('Non ci sono documento in risposta.');");
                }
            }

            #endregion



            //abilito e disabilito i bottoni per i firmatari
            if (schedaDocumento.documenti != null && schedaDocumento.documenti.Length > 0 && schedaDocumento.documenti[0].fileSize != null && !schedaDocumento.documenti[0].fileSize.Equals("0"))
            {
                this.txt_NomeFirma.Enabled = false;
                this.txt_CognomeFirma.Enabled = false;
                this.btn_aggiungiFirmatario.Enabled = false;
                this.btn_cancFirmatario.Enabled = false;
            }
            else
            {
                this.txt_NomeFirma.Enabled = true;
                this.txt_CognomeFirma.Enabled = true;
                this.btn_aggiungiFirmatario.Enabled = true;
                this.btn_cancFirmatario.Enabled = true;
            }
        }

        public DocsPAWA.DocsPaWR.InfoDocumento GetDocInRisposta(string sys)
        {
            DocsPAWA.DocsPaWR.InfoDocumento infoDocRisp = null;
            if (GetFiltroDocGrigi(sys))
            {
                int numTotPage;
                int nRec;
                DocumentManager.setFiltroRicDoc(this, qV);
                DocsPaWR.InfoUtente infoUt = new DocsPAWA.DocsPaWR.InfoUtente();
                infoUt = UserManager.getInfoUtente(this);
                ListaFiltri = DocumentManager.getFiltroRicDoc(this);
                SearchResultInfo[] idProfileList;
                DocsPAWA.DocsPaWR.InfoDocumento[] infoDoc = DocumentManager.getQueryInfoDocumentoPaging(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, 1, out numTotPage, out nRec, true, true, true, false, out idProfileList);
                if (infoDoc != null && infoDoc.Length == 1)
                {
                    infoDocRisp = infoDoc[0];
                }
            }
            return infoDocRisp;
        }

        public bool GetFiltroDocGrigi(string docSys)
        {
            try
            {
                if (string.IsNullOrEmpty(docSys))
                    return false;

                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];


                //non metto filtri sul tipo doc in quanto un doc grigio può essere collegato a :
                // - docGrigio
                // - docPredisposto
                //fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                //fV1.valore = "G";


                //fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);


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


                qV[0] = fVList;
                return true;

            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
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
        /* eventuali sbind...
         * 	this.btn_BackToFolder.Click += new System.Web.UI.ImageClickEventHandler(this.btn_BackToFolder_Click);
            this.btn_visibilita.Click += new System.Web.UI.ImageClickEventHandler(this.btn_visibilita_Click);
            this.lbl_dataCreazione.TextChanged += new System.EventHandler(this.lbl_dataCreazione_TextChanged);
            this.chkPrivato.CheckedChanged += new System.EventHandler(this.chkPrivato_CheckedChanged);
            this.rbl_sesso.SelectedIndexChanged += new System.EventHandler(this.rbl_sesso_SelectedIndexChanged);
            this.ddl_eta.SelectedIndexChanged += new System.EventHandler(this.ddl_eta_SelectedIndexChanged);
            this.btn_modificaOgget.Click += new System.Web.UI.ImageClickEventHandler(this.btn_modificaOgget_Click);
            this.txt_oggetto.TextChanged += new System.EventHandler(this.txt_oggetto_TextChanged);
            this.btn_selezionaParoleChiave.Click += new System.Web.UI.ImageClickEventHandler(this.btn_selezionaParoleChiave_Click);
            this.btn_eliminaParoleChiave.Click += new System.Web.UI.ImageClickEventHandler(this.btn_eliminaParoleChiave_Click);
            this.txt_note.TextChanged += new System.EventHandler(this.txt_note_TextChanged);
            this.txt_numOggetto.TextChanged += new System.EventHandler(this.txt_numOggetto_TextChanged);
            this.ddl_commRef.SelectedIndexChanged += new System.EventHandler(this.ddl_commRef_SelectedIndexChanged);
            this.btn_addTipoAtto.Click += new System.Web.UI.ImageClickEventHandler(this.btn_addTipoAtto_Click);
            this.ddl_tipoAtto.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoAtto_SelectedIndexChanged);
            this.btn_aggiungiFirmatario.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungiFirmatario_Click);
            this.btn_cancFirmatario.Click += new System.Web.UI.ImageClickEventHandler(this.btn_cancFirmatario_Click);
            this.btn_salva.Click += new System.Web.UI.ImageClickEventHandler(this.btn_salva_Click);
            this.btn_prodisponiProtocollo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_prodisponiProtocollo_Click);
            this.btn_aggiungi.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungi_Click);
            this.btn_stampa.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampa_Click);
            this.btn_rimuovi.Click += new System.Web.UI.ImageClickEventHandler(this.btn_rimuovi_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.docProfilo_PreRender);
         * */
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //this.btn_visibilita.Click += new System.Web.UI.ImageClickEventHandler(this.btn_visibilita_Click);
            this.chkPrivato.CheckedChanged += new System.EventHandler(this.chkPrivato_CheckedChanged);
            this.chkUtente.CheckedChanged += new System.EventHandler(this.chkUtente_CheckedChanged);
            this.rbl_sesso.SelectedIndexChanged += new System.EventHandler(this.rbl_sesso_SelectedIndexChanged);
            this.ddl_eta.SelectedIndexChanged += new System.EventHandler(this.ddl_eta_SelectedIndexChanged);
            this.btn_selezionaParoleChiave.Click += new System.Web.UI.ImageClickEventHandler(this.btn_selezionaParoleChiave_Click);
            this.btn_eliminaParoleChiave.Click += new System.Web.UI.ImageClickEventHandler(this.btn_eliminaParoleChiave_Click);
            this.ctrl_oggetto.OggettoChangedEvent += new Oggetto.OggettoChangedDelegate(this.ctrl_oggetto_OggChanged);
            this.txt_numOggetto.TextChanged += new System.EventHandler(this.txt_numOggetto_TextChanged);
            this.ddl_commRef.SelectedIndexChanged += new System.EventHandler(this.ddl_commRef_SelectedIndexChanged);
            this.ddl_tipoAtto.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoAtto_SelectedIndexChanged);
            this.btn_CampiPersonalizzati.Click += new System.Web.UI.ImageClickEventHandler(this.btn_CampiPersonalizzati_Click);
            this.img_btnStoriaDiagrammi.Click += new System.Web.UI.ImageClickEventHandler(this.img_btnStoriaDiagrammi_Click);
            this.txt_CodFascicolo.TextChanged += new System.EventHandler(this.txt_CodFascicolo_TextChanged);
            this.ddl_tmpl.SelectedIndexChanged += new System.EventHandler(this.ddl_tmpl_SelectedIndexChanged);
            this.MessageBox.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.MessageBox_GetMessageBoxResponse);
            this.MessageBox1.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.MessageBox1_GetMessageBoxResponse);
            this.msg_TrasmettiDoc.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_TrasmettiDoc_GetMessageBoxResponse);
            this.msg_PersonaleDoc.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_PersonaleDoc_GetMessageBoxResponse);
            this.btn_salva.Click += new System.Web.UI.ImageClickEventHandler(this.btn_salva_Click);
            this.btn_prodisponiProtocollo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_prodisponiProtocollo_Click);
            this.btn_aggiungi.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungi_Click);
            this.btn_stampa.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampa_Click);
            this.btn_inoltra.Click += new System.Web.UI.ImageClickEventHandler(this.btn_inoltra_Click);
            this.btn_addTipoAtto.Click += new System.Web.UI.ImageClickEventHandler(this.btn_addTipoAtto_Click);
            this.imgFasc.Click += new System.Web.UI.ImageClickEventHandler(this.imgFasc_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.docProfilo_PreRender);
            this.btn_log.Click += new System.Web.UI.ImageClickEventHandler(this.btn_log_Click);
            this.btn_riproponi.Click += new System.Web.UI.ImageClickEventHandler(this.btn_riproponi_Click);
            this.txt_codModello.TextChanged += new EventHandler(txt_codModello_TextChanged);
            this.btn_storiaCons.Click += new System.Web.UI.ImageClickEventHandler(this.btn_storiaCons_Click);
            this.msg_copiaDoc.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_copiaDoc_GetMessageBoxResponse);
            this.imgNewFasc.Click += new System.Web.UI.ImageClickEventHandler(this.imgNewFasc_Click);
            this.msg_rimuoviTipologia.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_rimuoviTipologia_GetMessageBoxResponse);
            this.btn_delTipologyDoc.Click += new System.Web.UI.ImageClickEventHandler(this.btn_delTipologyDoc_Click);
        }
        #endregion

        private void msg_copiaDoc_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                RiproponiDocGrigio(false, true);
            }
            else if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Cancel)
            {
                RiproponiDocGrigio(false, false);
            }
        }

        void txt_codModello_TextChanged(object sender, EventArgs e)
        {
            if (!this.txt_codModello.Text.Equals(""))
            {
                if (this.txt_codModello.Text.Length > 3 && this.txt_codModello.Text.Substring(0, 3).ToUpper().Equals("MT_"))
                {
                    // visto che il codice del modello trasmissione viene creato automaticamente con "MT_" + system_id
                    // utilizzo il già esistente metodo getModelloById che richiede il solo system_id restituendo il
                    // modello ricercato. Il system_id me lo ricavo dalla textbox txt_codModello escludendo le prime 3
                    // lettere "MT_".
                    string idModello = this.txt_codModello.Text.Substring(3);
                    DocsPaWR.ModelloTrasmissione modello = ModelliTrasmManager.getModelloByID(UserManager.getRegistroSelezionato(this).idAmministrazione, idModello, this);
                    if (modello != null)
                    {
                        this.ddl_tmpl.SelectedValue = modello.SYSTEM_ID.ToString();
                        this.txt_codModello.Text = modello.CODICE;
                    }
                    else
                    {
                        this.ddl_tmpl.SelectedIndex = 0;
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_modello", "alert('Non esiste un modello trasmissione con questo codice');", true);
                        //Response.Write("<script>alert('Non esiste un modello trasmissione con questo codice');</script>");
                    }
                }
                else
                {
                    this.ddl_tmpl.SelectedIndex = 0;
                    // Response.Write("alert('Non esiste un modello trasmissione con questo codice');</script>");
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_modello_cdice", "alert('Non esiste un modello trasmissione con questo codice');", true);
                }
            }
            else
                this.ddl_tmpl.SelectedIndex = 0;
        }

        private string CheckFields(SchedaDocumento schedaDocumento)
        {
            string msg = "";
            //controllo sull'inserimento dell'oggetto
            if (ctrl_oggetto.oggetto_text.Trim() == "" || ctrl_oggetto.oggetto_text == null)
            {
                msg = "Inserire un valore per il campo oggetto.";
                this.ctrl_oggetto.oggetto_SetControlFocus();
                return msg;
            }

            //controllo sulla lunghezza dell'oggetto (max 2000 car.)
            int maxLength = 2000;
            Int32.TryParse(utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_MAX_LENGTH_OGGETTO"), out maxLength);
            if (this.ctrl_oggetto.oggetto_text.Length > maxLength)
            {
                msg = String.Format("La lunghezza massima del campo oggetto non deve superare i {0} caratteri.", maxLength);
                this.ctrl_oggetto.oggetto_SetControlFocus();
                return msg;
            }

            //controllo sulla fascicolazione obbligatoria
            if (this.isFascRapidaRequired)
            {
                if (schedaDocumento.systemId == null)
                {
                    if (this.txt_CodFascicolo.Text.Equals("") || this.txt_DescFascicolo.Text.Equals(""))
                    {
                        msg = "La fascicolazione rapida è obbligatoria";
                        return msg;
                    }
                }
                else
                {
                    // controllo su obbligatorietà della fascicolazione e chiamata al web service
                    // che ci da informazione se il documento è già stato o meno fascicolato
                    if (schedaDocumento.documentoPrincipale == null)
                        if (!DocumentManager.getSeDocFascicolato(this, schedaDocumento) && string.IsNullOrEmpty(this.txt_CodFascicolo.Text) && string.IsNullOrEmpty(this.txt_DescFascicolo.Text))
                        {
                            msg = "La fascicolazione rapida è obbligatoria";
                            return msg;
                        }
                }
            }
            // fine controllo fascicolazione obbligatoria

            //controllo campi obbligatori profilazione dinamica
            //PROFILAZIONE DINAMINCA
            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                //Salvataggio dei campi della profilazione dinamica
                if (Session["template"] != null)
                {
                    if (ProfilazioneDocManager.verificaCampiObbligatori((DocsPAWA.DocsPaWR.Templates)Session["template"]))
                    {
                        msg = "Ci sono dei campi obbligatori relativi al tipo di documento selezionato !";
                        return msg;
                    }

                    string messag = ProfilazioneDocManager.verificaOkContatore((DocsPAWA.DocsPaWR.Templates)Session["template"]);
                    if (messag != string.Empty)
                        return messag;
                }
                if (Panel_DataScadenza.Visible && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                {
                    try
                    {
                        DateTime dataInserita = Convert.ToDateTime(txt_dataScadenza.Text);
                        DateTime dataOdierna = System.DateTime.Now;
                        if ((dataInserita < dataOdierna) && txt_dataScadenza.Enabled)
                        {
                            msg = "La data di scadenza deve essere successiva a quella odierna.";
                            return msg;
                        }
                        schedaDocumento.dataScadenza = Utils.formatDataDocsPa(dataInserita);
                    }
                    catch (Exception ex)
                    {
                        msg = "Inserire una data valida";
                        return msg;
                    }
                }
            }
            //FINE PROFILAZIONE DINAMINCA	

            //tipo atto se obbligatorio
            string idAmm = UserManager.getInfoUtente().idAmministrazione;
            if (DocumentManager.GetTipoDocObbl(idAmm).Equals("1") && this.ddl_tipoAtto.SelectedIndex == 0 && schedaDocumento.documentoPrincipale == null)
                //if (System.Configuration.ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"] != null
                //    && System.Configuration.ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"] == "1"
                //    && this.ddl_tipoAtto.SelectedIndex == 0
                //    && schedaDocumento.documentoPrincipale == null) //no per gli allegati.
                msg = "Inserire un valore per il campo Tipologia documento.";
            return msg;
        }

        private void btn_riproponi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            logger.Info("BEGIN");
            if ((this.schedaDocumento.documenti != null && (!string.IsNullOrEmpty(this.schedaDocumento.documenti[0].fileName)))
               || (this.schedaDocumento.allegati != null && schedaDocumento.allegati.Length > 0))
            {
                if (System.Configuration.ConfigurationManager.AppSettings["RIPROPONI_AVANZATO_ENABLED"] != null && System.Configuration.ConfigurationManager.AppSettings["RIPROPONI_AVANZATO_ENABLED"].ToUpper() == "TRUE")
                {
                    string messaggio = InitMessageXml.getInstance().getMessage("docRiproponiCopiaDocumenti");
                    msg_copiaDoc.Confirm(messaggio);
                }
                else RiproponiDocGrigio(false, false);
            }
            else
            {
                RiproponiDocGrigio(false, true);
            }
            logger.Info("END");
        }

        private void RiproponiDocGrigio(bool daRisposta, bool conCopia)
        {
            logger.Info("BEGIN");
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                DocumentManager.removeMemoriaFiltriRicDoc(this);
                DocumentManager.RemoveMemoriaVisualizzaBack(this);


                TrasmManager.RemoveMemoriaVisualizzaBack(this);
                if (this.ddl_tmpl.SelectedIndex == 0 && Session["Modello"] != null)
                    Session.Remove("Modello");
                Session.Add("docRiproposto", true);
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                Session.Add("templateRiproposto", template);
                if (this.ddl_tipoAtto != null && this.ddl_tipoAtto.SelectedItem != null)
                {
                    Session.Add("tipoAttoRipropostoTesto", this.ddl_tipoAtto.SelectedItem.Text);
                    Session.Add("tipoAttoRipropostoId", this.ddl_tipoAtto.SelectedItem.Value);
                }

                // Se è attiva la funzionalità di riproponi con conoscenza, deve essere riproposta anche la fascicolazione
                if (this.IsEnabledRiproponiConConoscenza)
                {
                    InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
                    Fascicolo[] listaFascicoli = DocumentManager.GetFascicoliDaDoc(this, infoDocumento.idProfile);
                    if (listaFascicoli != null && listaFascicoli.Length > 0)
                        FascicoliManager.SetDoFascRapida(this, listaFascicoli[listaFascicoli.Length - 1]);
                } 
                
                bool eUffRef = false;

                if (conCopia)
                {
                    schedaDocumento = DocumentManager.riproponiConCopiaDoc(this, schedaDocumento, eUffRef);

                    FileManager.removeSelectedFile(this);
                    DocumentManager.removeDocumentoInLavorazione();
                    DocumentManager.setDocumentoInLavorazione(schedaDocumento);
                    DocumentManager.setDocumentoSelezionato(schedaDocumento);

                    if (schedaDocumento.repositoryContext != null)
                        schedaDocumento.repositoryContext.IsDocumentoGrigio = true;

                    //Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';</script>");
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Riproponi", "top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';", true);
                }
                else
                {
                    schedaDocumento = DocumentManager.riproponiDatiGrigio(this, schedaDocumento, eUffRef, daRisposta);

                    FileManager.removeSelectedFile(this);
                    DocumentManager.removeDocumentoInLavorazione();
                    DocumentManager.setDocumentoInLavorazione(schedaDocumento);
                    DocumentManager.setDocumentoSelezionato(schedaDocumento);

                    if (schedaDocumento.repositoryContext != null)
                        schedaDocumento.repositoryContext.IsDocumentoGrigio = true;

                    Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';</script>");
                }
            }
            logger.Info("END");
        }

        private void setParoleChiavi(DocsPAWA.DocsPaWR.DocumentoParolaChiave[] listaParoleChiavi)
        {
            this.lbx_paroleChiave.Attributes.Clear();
            this.lbx_paroleChiave.Items.Clear();
            if (listaParoleChiavi != null)
            {
                if (listaParoleChiavi.Length > 0)
                {
                    for (int i = 0; i < listaParoleChiavi.Length; i++)
                    {
                        this.lbx_paroleChiave.Items.Add(listaParoleChiavi[i].descrizione);
                        this.lbx_paroleChiave.Items[i].Value = listaParoleChiavi[i].systemId;
                    }
                }
            }
        }

        private void setTipoAtto(string tipoAtto)
        {
            if (tipoAtto != null)
            {
                for (int i = 0; i < this.ddl_tipoAtto.Items.Count; i++)
                {
                    if (this.ddl_tipoAtto.Items[i].Value == tipoAtto)
                    {
                        ddl_tipoAtto.SelectedItem.Selected = false;
                        this.ddl_tipoAtto.Items[i].Selected = true;
                    }
                }
            }
        }

        private void setListBoxFirmatari()
        {
            DocsPaWR.Firmatario firmatario;
            if (schedaDocumento.documenti == null || schedaDocumento.documenti.Length < 1)
                return;
            if (schedaDocumento.documenti[0].firmatari != null)
            {
                for (int i = 0; i < schedaDocumento.documenti[0].firmatari.Length; i++)
                {
                    firmatario = schedaDocumento.documenti[0].firmatari[i];
                    this.lbx_firmatari.Items.Add(firmatario.nome + " - " + firmatario.cognome);
                    this.lbx_firmatari.Items[i].Value = firmatario.systemId;
                }
            }
        }

        private void setCommRef(string commRef)
        {
            if (commRef != null)
            {
                for (int i = 0; i < this.ddl_commRef.Items.Count; i++)
                {
                    if (this.ddl_commRef.Items[i].Value == commRef)
                    {
                        this.ddl_commRef.Items[i].Selected = true;
                        return;
                    }
                }
            }
        }

        private void docProfilo_PreRender(object sender, System.EventArgs e)
        {
            logger.Info("BEGIN");
            try
            {
                //se il documento esiste, riempie i campi con i valori
                if (schedaDocumento != null)
                {
                    //oggetto1
                    if (schedaDocumento.oggetto != null && schedaDocumento.oggetto.descrizione != null && schedaDocumento.oggetto.descrizione != "")
                    {
                        this.ctrl_oggetto.oggetto_text = schedaDocumento.oggetto.descrizione;
                        //solo se il campo codice oggetto è attivo!!!
                        if (schedaDocumento.oggetto.codOggetto != null)
                        {
                            this.ctrl_oggetto.cod_oggetto_text = schedaDocumento.oggetto.codOggetto;
                        }
                    }
                    //se il documento è protocollato l'oggetto
                    if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.segnatura != null && !schedaDocumento.protocollo.segnatura.Equals(""))
                    {
                        //se protocollo l'oggetto si modifica solo dal tab protocollo, non da profilo.
                        this.ctrl_oggetto.oggetto_isReadOnly = true;
                    }
                    //file
                    if (schedaDocumento.documenti != null && schedaDocumento.documenti.Length > 0)
                        FileManager.setSelectedFile(this, schedaDocumento.documenti[0]);

                    if (schedaDocumento != null && schedaDocumento.dataCreazione != null && schedaDocumento.dataCreazione != "")
                    {
                        if (schedaDocumento.dataCreazione.ToString().Length > 10)
                            this.lbl_dataCreazione.Text = Utils.dateTimeLength(schedaDocumento.dataCreazione);
                        else
                            this.lbl_dataCreazione.Text = schedaDocumento.dataCreazione;
                    }

                    //doc Number
                    this.txt_docNumber.Text = schedaDocumento.docNumber;
                    this.ldl_docNumber.Text = schedaDocumento.docNumber;

                    // ripropone la fascicolatura rapida
                    //Folder[] folder = schedaDocumento.folder;
                    //if (folder != null)
                    //{
                    //    foreach (Folder fol in folder)
                    //    {
                    //        this.txt_CodFascicolo.Text = fol.idFascicolo;
                    //        this.txt_DescFascicolo.Text = fol.descrizione;
                    //    }
                    //}

                    //tipo Atto
                    if (schedaDocumento.tipologiaAtto != null)
                        setTipoAtto(schedaDocumento.tipologiaAtto.systemId);

                    //parole chiave
                    if (schedaDocumento.paroleChiave != null /*&& schedaDocumento.paroleChiave.Length > 0*/)
                        setParoleChiavi(schedaDocumento.paroleChiave);
                    else
                    {
                        // ricarica delle parole chiave nel caso di riproponi doc protocollato
                        DocsPaWR.DocumentoParolaChiave[] listaParole = (DocsPaWR.DocumentoParolaChiave[])Session["ricDoc.listaParoleChiave"];
                        if (listaParole != null)
                        {
                            this.setParoleChiavi(listaParole);
                            schedaDocumento.paroleChiave = addParoleChiaveToDoc(schedaDocumento, listaParole);
                            schedaDocumento.daAggiornareParoleChiave = true;
                            DocumentManager.setDocumentoInLavorazione(schedaDocumento);
                            Session.Remove("ricDoc.listaParoleChiave");
                        }
                    }

                    //numero oggetto e commissione referente
                    this.txt_numOggetto.Text = schedaDocumento.numOggetto;
                    if (schedaDocumento.commissioneRef != null)
                        setCommRef(schedaDocumento.commissioneRef);

                    #region privato e personale
                    //Controllo se il ruolo utente è autorizzato a creare documenti personali
                    if (!UserManager.ruoloIsAutorized(this, "DO_CREA_PERSONALE") || UserManager.getInfoUtente().delegato != null)
                        this.chkUtente.Visible = false; //se non sono autorizzato non lo vedo.

                    //Controllo se il ruolo utente è autorizzato a creare documenti privati
                    if (!UserManager.ruoloIsAutorized(this, "DO_CREA_PRIVATO") || UserManager.getInfoUtente().delegato != null)
                        this.chkPrivato.Visible = false;


                    //privato
                    if (schedaDocumento.privato != null && schedaDocumento.privato.Equals("1"))
                    {
                        this.chkPrivato.Checked = true;
                        //in questo caso in cui privato=1
                        //forzo la visibilità anche se 
                        //il ruolo non è autorizzato, 
                        // altrimenti non è visibile 
                        //l'informazione che il documento è privato
                        this.chkPrivato.Visible = true;
                    }
                    else
                        this.chkPrivato.Checked = false;

                    //visualizzo label personale o utente
                    if (System.Configuration.ConfigurationManager.AppSettings["Label_Personale"] != null)
                    {
                        chkUtente.Text = System.Configuration.ConfigurationManager.AppSettings["Label_Personale"].ToString();
                    }
                    else
                        chkUtente.Text = "Utente";
                    if (schedaDocumento.personale != null && schedaDocumento.personale.Equals("1"))
                        this.chkUtente.Checked = true;
                    else
                        this.chkUtente.Checked = false;



                    #endregion

                    //firmatari
                    this.lbx_firmatari.Items.Clear();
                    setListBoxFirmatari();

                    if (UserManager.ruoloIsAutorized(this, "FASC_INS_DOC"))
                    {
                        setFascicolazioneRapida();
                        this.ClearResourcesRicercaFascicoliFascRapida();
                    }

                    //se presente e abilitata chiave "FE_FASC_PRIMARIA" in dpa_chiavi_configurazione e 
                    //se il documento è fascicolato allora si rende visibile la label che indica 
                    //la descrizione del fascicolo principale
                    string valoreChiave;
                    valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_FASC_PRIMARIA");

                    // Se il documento è riproposto ed è fascicolato viene impostato a 0 il flag fascicolato in modo
                    // che non venga più visualizzato il messaggio "Descrizione non visualizzabile"
                    if (Session["docRiproposto"] != null && schedaDocumento != null && schedaDocumento.fascicolato == "1")
                        schedaDocumento.fascicolato = "0";

                    if (!string.IsNullOrEmpty(schedaDocumento.fascicolato) && schedaDocumento.fascicolato == "1" && valoreChiave.Equals("1"))
                    {
                        string descFascPrimaria = DocumentManager.GetFascicolazionePrimaria(this, schedaDocumento.docNumber);
                        if (!string.IsNullOrEmpty(descFascPrimaria))
                        {
                            lbl_fasc_Primaria.Text = "Fasc. Princ. " + descFascPrimaria;
                            pnl_fasc_Primaria.Visible = true;
                        }
                    }
                    else
                        pnl_fasc_Primaria.Visible = false;

                    if (System.Configuration.ConfigurationManager.AppSettings["NON_VISUALIZZA_FRECCIA_RISPOSTA"] != null
                    && System.Configuration.ConfigurationManager.AppSettings["NON_VISUALIZZA_FRECCIA_RISPOSTA"] == "1")
                    {
                        if (schedaDocumento.systemId == null || GetCountDocInRisposta(schedaDocumento.systemId) == 0)
                        {
                            this.btn_risp_dx.Visible = false;
                        }
                        else
                        {
                            this.btn_risp_dx.Visible = true;
                        }
                    }
                }

                //rende visibili/invisibili i vari pannellistef
                enableProfiloFields();

                //gestione visibilità pulsanti catene
                gestionePulsantiCatene();

                // Imposta la pagina
                setFormProperties();

                //settaggio tooltip oggetto
                if (this.ctrl_oggetto.oggetto_text != "")
                    this.ctrl_oggetto.OggToolTip = ctrl_oggetto.oggetto_text;

                //copio il documento in sessione
                DocumentManager.setDocumentoInLavorazione(schedaDocumento);

				string codiceApplicazione = string.Empty;

                if (ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"] != null)
                {
                    codiceApplicazione = ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"].ToString();
                }
                //Per anomalia documento personale
                if (schedaDocumento.personale == "1" && string.IsNullOrEmpty(codiceApplicazione))
                    schedaDocumento.accessRights = "255";

                verificaHMdiritti();

                //abilitazione delle funzioni in base al ruolo
                UserManager.disabilitaFunzNonAutorizzate(this);

                //per ora la matitina di modifica non serve, ma potrebbe tornare utile per sviluppi futuri
                //quindi la rendo solo invisibile:
                this.btn_modificaOgget.Visible = false;
                this.imgDescOgg.Enabled = true;

                //Attivazione del controllo ortografico di default è disabilitato
                bool useSpell = false;
                if (ConfigurationManager.AppSettings["USE_SPELL_ACTIVEX"] != null)
                {
                    bool.TryParse(ConfigurationManager.AppSettings["USE_SPELL_ACTIVEX"], out useSpell);
                }
                if (useSpell)
                {
                    this.btn_Correttore.Enabled = true;
                    this.btn_Correttore.Visible = true;
                }
                //*************************************

                if (schedaDocumento.docNumber == null ||
                    schedaDocumento.docNumber.Equals("0"))
                    this.btn_stampa.Enabled = false;

                if (!this.IsPostBack)
                    this.SetDocumentOnContext();

                //tipo atto se obligatorio
                string idAmm = UserManager.getInfoUtente().idAmministrazione;
                if (DocumentManager.GetTipoDocObbl(idAmm).Equals("1"))
                    //if (System.Configuration.ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"] != null
                    //    && System.Configuration.ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"] == "1")
                    this.pnl_star.Visible = true;
                else
                    this.pnl_star.Visible = false;

                //luluciani if (UserManager.ruoloIsAutorized(this, "ACL_RIMUOVI"))
                //{
                //utente è autorizzato a rimuovere le acl per il documento 
                //verifica che ci siano ACL rimosse 
                if (schedaDocumento != null && schedaDocumento.systemId != null)
                {
                    int result = DocumentManager.verificaDeletedACL(this.schedaDocumento.systemId, UserManager.getInfoUtente(this));
                    if (result == 1)
                    {
                        btn_visibilita.BorderWidth = 1;
                        btn_visibilita.BorderColor = Color.Red;
                        //btn_visibilita.ImageUrl = "../images/proto/ico_visibilita_rimosso.gif";
                    }
                    else
                        btn_visibilita.BorderWidth = 0;
                }
                // }

                if (schedaDocumento != null && schedaDocumento.inCestino == "1")
                    this.ddl_tipoAtto.Enabled = false;

                if (!this.IsPostBack)
                {
                    // Caricamento dati dettaglio nota
                    //this.dettaglioNota.AttatchPulsanteConferma(this.btn_salva.ClientID);
                    this.dettaglioNota.Fetch();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }



            //Richiamo il metodo verificaCampiPersonalizzati per abilitare o meno il pannello corrispondente
            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                //verificaCampiPersonalizzati();
                ProfilazioneDocManager.verificaCampiPersonalizzati(this, schedaDocumento);
                //DIAGRAMMI DI STATO
                if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                {
                    //Controllo se lo stato è uno stato automatico
                    if (Session["statoAutomatico"] != null)
                    {
                        if ((string)Session["statoAutomatico"] == "SI")
                            MessageBox1.Confirm("Lo stato selezionato è uno stato automatico.\\nConfermi il salvataggio ?");
                    }

                    //Controllo se lo stato è uno stato finale
                    if (Session["docSolaLettura"] != null)
                    {
                        if ((string)Session["docSolaLettura"] == "SI")
                            MessageBox.Confirm("Si sta portando il documento in uno stato finale.\\nIl documento diventerà di sola lettura.\\nConfermi il salvataggio ?");
                    }

                    //Verifico se esiste un diagramma di stato associato al tipo di documento
                    DocsPaWR.DiagrammaStato dg = null;
                    if (schedaDocumento != null && schedaDocumento.tipologiaAtto != null)
                        dg = DocsPAWA.DiagrammiManager.getDgByIdTipoDoc(schedaDocumento.tipologiaAtto.systemId, (UserManager.getInfoUtente(this)).idAmministrazione, this);
                    else
                        dg = DocsPAWA.DiagrammiManager.getDgByIdTipoDoc(ddl_tipoAtto.SelectedValue, (UserManager.getInfoUtente(this)).idAmministrazione, this);
                    Session.Add("DiagrammaSelezionato", dg);
                    if (ddl_tipoAtto.SelectedValue != "" && dg != null)
                    {
                        Panel_DiagrammiStato.Visible = true;
                        //Controllo se il documento è nuovo o meno
                        if (schedaDocumento != null && schedaDocumento.docNumber != null && schedaDocumento.docNumber != "")
                        {
                            DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
                            //Controllo che il doc non è gia' in un determintato stato del diagramma suddetto "DPA_DIAGRAMMI_DOC"
                            if (stato != null)
                            {
                                //se il ruolo corrente ha visibilità sullo stato corrente allora carico gli stati successivi
                                if (DiagrammiManager.IsRuoloAssociatoStatoDia(dg.SYSTEM_ID.ToString(), UserManager.getRuolo().idGruppo, stato.SYSTEM_ID.ToString()))
                                {
                                    lbl_statoAttuale.Text = stato.DESCRIZIONE;
                                    lbl_statoAttuale.Visible = true;
                                    if (stato.STATO_FINALE)
                                    {
                                        ddl_statiSuccessivi.Enabled = false;
                                        if (schedaDocumento.accessRights.Equals("45"))
                                            this.btn_salva.Enabled = false;
                                    }
                                    else
                                    {
                                        popolaComboBoxStatiSuccessivi(stato, dg);
                                    }
                                }
                            }
                            //Controllo che il doc non è in uno stato di un diagramma non piu' attivo "DPA_DIAGRAMMI_STO" (in questo caso sicuro è uno stato finale)
                            else
                            {
                                string st = DocsPAWA.DiagrammiManager.getStatoDocStorico(schedaDocumento.docNumber, this);
                                lbl_statoAttuale.Text = st;
                                lbl_statoAttuale.Visible = true;
                                //ddl_statiSuccessivi.Enabled = false;
                                popolaComboBoxStatiSuccessivi(null, dg);
                            }

                            //Imposto la data di scadenza per un doc non nuovo
                            if (schedaDocumento.dataScadenza != null && schedaDocumento.dataScadenza != "" && schedaDocumento.dataScadenza != "0" && schedaDocumento.dataScadenza.IndexOf("1900") == -1)
                            {
                                Panel_DataScadenza.Visible = true;
                                txt_dataScadenza.Text = schedaDocumento.dataScadenza;
                                txt_dataScadenza.Enabled = false;
                            }
                        }
                        else
                        {
                            //Documento nuovo popolo la combo box degli stati con gli stati iniziali del diagramma corrispondente alla tipologia di documento
                            //Al seguente metodo se come parametro gli viene passato "null", popola la combo box con gli stati iniziali, altrimenti se gli viene
                            //passato uno stato, popola la combo box con gli stati successivi possibili
                            popolaComboBoxStatiSuccessivi(null, dg);

                            //Imposto la data di scadenza per un doc nuovo
                            if (Session["template"] != null)
                            {
                                DocsPaWR.Templates template = (DocsPaWR.Templates)Session["template"];
                                if (template.SCADENZA != null && template.SCADENZA != "" && template.SCADENZA != "0")
                                {
                                    try
                                    {
                                        DateTime dataOdierna = System.DateTime.Now;
                                        int scadenza = Convert.ToInt32(template.SCADENZA);
                                        DateTime dataCalcolata = dataOdierna.AddDays(scadenza);
                                        Panel_DataScadenza.Visible = true;
                                        if (schedaDocumento.dataScadenza == null || schedaDocumento.dataScadenza == "")
                                        {
                                            txt_dataScadenza.Text = Utils.formatDataDocsPa(dataCalcolata);
                                            schedaDocumento.dataScadenza = Utils.formatDataDocsPa(dataCalcolata);
                                        }
                                    }
                                    catch (Exception ex) { }
                                }
                                else
                                {
                                    txt_dataScadenza.Text = "";
                                    Panel_DataScadenza.Visible = false;
                                    schedaDocumento.dataScadenza = "";
                                }
                            }
                        }

                        //Quando un documento è in sola lettura, non deve essere possibile cambiare lo stato
                        if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45")
                            ddl_statiSuccessivi.Enabled = false;

                        //Quando un documento è di una tipologia non in esercizio, non deve essere possibile cambiare lo stato
                        if ((DocsPaWR.Templates)Session["template"] != null && ((DocsPaWR.Templates)Session["template"]).IN_ESERCIZIO.ToUpper().Equals("NO"))
                        {
                            ddl_statiSuccessivi.SelectedIndex = 0;
                            ddl_statiSuccessivi.Enabled = false;
                        }
                    }
                    else
                    {
                        Panel_DiagrammiStato.Visible = false;
                        txt_dataScadenza.Text = "";
                        //Controllo che non ci sia solo la data scadenza
                        if (Session["template"] != null)
                        {
                            if (schedaDocumento != null && schedaDocumento.docNumber != null)
                            {
                                //Documento non nuovo
                                if (schedaDocumento.dataScadenza != null && schedaDocumento.dataScadenza != "")
                                {
                                    txt_dataScadenza.Text = schedaDocumento.dataScadenza;
                                    txt_dataScadenza.Enabled = false;
                                    Panel_DataScadenza.Visible = true;
                                }
                            }
                            else
                            {
                                //Documento Nuovo
                                DocsPaWR.Templates template = (DocsPaWR.Templates)Session["template"];
                                if (template.SCADENZA != null && template.SCADENZA != "" && template.SCADENZA != "0")
                                {
                                    try
                                    {
                                        DateTime dataOdierna = System.DateTime.Now;
                                        int scadenza = Convert.ToInt32(template.SCADENZA);
                                        DateTime dataCalcolata = dataOdierna.AddDays(scadenza);
                                        Panel_DataScadenza.Visible = true;
                                        if (schedaDocumento.dataScadenza == null || schedaDocumento.dataScadenza == "")
                                        {
                                            txt_dataScadenza.Text = Utils.formatDataDocsPa(dataCalcolata);
                                            schedaDocumento.dataScadenza = Utils.formatDataDocsPa(dataCalcolata);
                                        }
                                        else
                                        {
                                            txt_dataScadenza.Text = schedaDocumento.dataScadenza;
                                        }
                                    }
                                    catch (Exception ex) { }
                                }
                                else
                                {
                                    txt_dataScadenza.Text = "";
                                    Panel_DataScadenza.Visible = false;
                                }
                            }
                        }
                    }
                    
                    //Quando un documento è in sola lettura, non deve essere possibile cambiare lo stato
                    if (((SchedaDocumento)DocumentManager.getDocumentoInLavorazione(this)).accessRights == "45")
                        ddl_statiSuccessivi.Enabled = false;

                    //Quando un documento è di una tipologia non in esercizio, non deve essere possibile cambiare lo stato
                    if ((DocsPaWR.Templates)Session["template"] != null && ((DocsPaWR.Templates)Session["template"]).IN_ESERCIZIO.ToUpper().Equals("NO") && ddl_statiSuccessivi.Items.Count > 0)
                    {
                        ddl_statiSuccessivi.SelectedIndex = 0;
                        ddl_statiSuccessivi.Enabled = false;                        
                    }
                }
                //FINE DIAGRAMMI DI STATO	
            }

            // Visibilità pulsante stampa, solamente se è un documento grigio (non documento allegato)
            if (this.btn_stampaSegn.Enabled)
                this.btn_stampaSegn.Enabled = (this.schedaDocumento != null &&
                        this.schedaDocumento.protocollo == null && this.schedaDocumento.documentoPrincipale == null);

            if (isRiproposto != null && isRiproposto)
            {
                ddl_tipoAtto.Enabled = true;
                Session.Remove("docRiproposto");
            }

            //string conservazione = ConfigurationManager.AppSettings["CONSERVAZIONE"];
            //if (!string.IsNullOrEmpty(conservazione) && conservazione == "1")
            //{
            //    this.btn_storiaCons.Visible = true;
            //}
            if (UserManager.ruoloIsAutorized(this, "DO_CONS"))
            {
                this.btn_storiaCons.Visible = true;
            }

            if (Session["oggetto_popup"] != null)
            {
                this.ctrl_oggetto.oggetto_text = Session["oggetto_popup"].ToString();
                this.schedaDocumento.oggetto.descrizione = this.ctrl_oggetto.oggetto_text;
                this.schedaDocumento.oggetto.daAggiornare = true;
                if (this.ctrl_oggetto.cod_oggetto_isVisible)
                {
                    this.ctrl_oggetto.cod_oggetto_text = string.Empty;
                    this.schedaDocumento.oggetto.codOggetto = string.Empty;
                }
                Session.Remove("oggetto_popup");
            }

            // Il codice che segue è commentato anche nella pagina del protocollo, penso che sia stato
            // commentato per risolvere il problema che si verifica in questa pagina ovvero consente di
            // selezionare una nuova tipologia da associare al protocollo ma quando si clicca su salva,
            // viene associata la nuova tipologia con tutti i campi vuoti
            //if (tr_tipologia.Visible)
            //{
            //    if(schedaDocumento!= null && schedaDocumento.protocollo !=null)
            //    {
            //        if (UserManager.isFiltroAooEnabled(this))
            //        {
            //            DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);
            //            this.ddl_tipoAtto.Enabled = UserManager.verifyRegNoAOO(schedaDocumento, userRegistri);
            //            this.btn_salva.Enabled = UserManager.verifyRegNoAOO(schedaDocumento, userRegistri);
            //        }
            //    }
            //}

            if (this.schedaDocumento != null)
            {
                int isInAdl = DocumentManager.isDocInADL(this.schedaDocumento.systemId, this);
                if (isInAdl == 1)
                {
                    this.btn_aggiungi.Enabled = true;
                    this.btn_aggiungi.ToolTip = "Rimuovi documento da area di lavoro";
                    this.btn_aggiungi.ImageUrl = "../images/proto/canc_area.gif";
                }
                else
                {
                    this.btn_aggiungi.Enabled = true;
                    this.btn_aggiungi.ToolTip = "Inserisci documento in area di lavoro";
                    this.btn_aggiungi.ImageUrl = "../images/proto/ins_area.gif";
                }
            }

            if (this.schedaDocumento != null && this.schedaDocumento.tipoProto != null && this.schedaDocumento.tipoProto.Equals("C"))
            {
                this.btn_riproponi.Enabled = false;
                this.btn_log.Visible = false;
            }

            #region DISABILITAZIONE CONTROLLI SUL CONSOLIDAMENTO (ATTENZIONE: DEVE RIMANERE SEMPRE L'ULTIMA OPERAZIONE FATTA NEL PRERENDER)

            // Abilitazione / Disabilitazione controlli in caso di documento consolidato
            if (this.schedaDocumento != null)
            {
                if (this.schedaDocumento.ConsolidationState != null &&
                    this.schedaDocumento.ConsolidationState.State > DocumentConsolidationStateEnum.None)
                {
                    // Diabilitazione controlli su documento consolidato
                    this.btn_prodisponiProtocollo.Enabled = false;
                    this.btn_rimuovi.Enabled = false;

                    string message = string.Empty;

                    if (this.schedaDocumento.ConsolidationState.State == DocumentConsolidationStateEnum.Step1)
                    {
                        message = "CONSOLIDATO CONTENUTO";
                    }
                    else if (this.schedaDocumento.ConsolidationState.State == DocumentConsolidationStateEnum.Step2)
                    {
                        message = "CONSOLIDATO CONTENUTO E METADATI";

                        System.Drawing.Color colorDisabled = System.Drawing.Color.FromArgb(225, 225, 225);

                        // Diabilitazione controlli su documento consolidato nei metadati
                        this.ctrl_oggetto.oggetto_isReadOnly = true;
                        this.ctrl_oggetto.BackColor = colorDisabled;
                    }

                    this.lblStatoConsolidamento.Visible = (!string.IsNullOrEmpty(message));
                    this.lblStatoConsolidamento.Text = message;
                }

                if (!string.IsNullOrEmpty(schedaDocumento.accessRights) && schedaDocumento.accessRights.Equals("45"))
                    this.ddl_tipoAtto.Enabled = false;
            }

            #endregion

            //Verifica atipicita documento
            Utils.verificaAtipicita(schedaDocumento, DocsPaWR.TipoOggettoAtipico.DOCUMENTO, ref btn_visibilita);

            // Verifica del vero creatore del documento
            Utils.CheckCreatorRole(schedaDocumento, ref this.btn_log);
			string codiceApplicazioneTemp = string.Empty;
            if (ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"] != null)
            {
                codiceApplicazioneTemp = ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"].ToString();
            }
            if (!string.IsNullOrEmpty(codiceApplicazioneTemp) && schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.accessRights) && schedaDocumento.accessRights.Equals("45") && !schedaDocumento.codiceApplicazione.Equals(codiceApplicazioneTemp))
            {
                //bottoni che devono essere disabilitati in caso
                //di diritti di sola lettura
                this.btn_addTipoAtto.Enabled = false;
                this.btn_aggiungiFirmatario.Enabled = false;
                this.btn_aggiungiFirmatario.Enabled = false;
                this.btn_salva.Enabled = false;
                this.btn_rimuovi.Enabled = false;
                this.btn_eliminaParoleChiave.Enabled = false;
                this.btn_prodisponiProtocollo.Enabled = false;
                this.btn_modificaOgget.Enabled = false;
                this.btn_cancFirmatario.Enabled = false;
                this.btn_selezionaParoleChiave.Enabled = false;
                this.btn_oggettario.Enabled = false;
                this.ctrl_oggetto.oggetto_isReadOnly = true;
                this.chkPrivato.Enabled = false;
                this.chkUtente.Enabled = false;
            }
			
            logger.Info("END");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_selezionaParoleChiave_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                // SetOggetto
                string javascript = null;
                //TODO switch per abilitare la nuova gestione parole chiave
                string keyAdvanced = string.Empty;
                keyAdvanced = DocsPAWA.utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione,
                                                                      "FE_PAROLE_CHIAVI_AVANZATE");
                if(keyAdvanced.Equals("1"))
                    javascript = "win=window.open('../popup/newParoleChiave.aspx?wnd=docProf','ParoleChiave','width=520,height=356,top=114,left=429,toolbar=no,directories=no,menubar=no,resizable=no,scrollbars=no');";
                else
                {
                    javascript = "win=window.open('../popup/paroleChiave.aspx?wnd=docProf','ParoleChiave','width=480,height=380,top=114,left=429,toolbar=no,directories=no,menubar=no,resizable=yes,scrollbars=no');";
                }
                    
                
                javascript += "win.focus();";
                //Response.Write("<script>" + javascript + "</script>");
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_modello", "" + javascript + "", true);


            }
        }

        private DocsPAWA.DocsPaWR.DocumentoParolaChiave[] addParoleChiaveToDoc(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento, DocsPAWA.DocsPaWR.DocumentoParolaChiave[] listaDocParoleChiave)
        {
            DocsPaWR.DocumentoParolaChiave[] listaPC;
            listaPC = schedaDocumento.paroleChiave;
            if (listaDocParoleChiave != null)
            {
                for (int i = 0; i < listaDocParoleChiave.Length; i++)
                {
                    if (!listaContains(schedaDocumento.paroleChiave, listaDocParoleChiave[i]))
                        listaPC = Utils.addToArrayParoleChiave(listaPC, listaDocParoleChiave[i]);
                }
            }
            return listaPC;

        }

        private bool listaContains(DocsPAWA.DocsPaWR.DocumentoParolaChiave[] lista, DocsPAWA.DocsPaWR.DocumentoParolaChiave el)
        {
            bool trovato = false;
            if (lista != null)
            {
                for (int i = 0; i < lista.Length; i++)
                {
                    if (lista[i].systemId.Equals(el.systemId))
                    {
                        trovato = true;
                        break;
                    }
                }
            }
            return trovato;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetOggetto()
        {
            try
            {
                DocsPaWR.Oggetto ProtoObj = new DocsPAWA.DocsPaWR.Oggetto();
                ProtoObj.descrizione = this.ctrl_oggetto.oggetto_text;
                schedaDocumento.oggetto = ProtoObj;
                if (schedaDocumento.systemId != null)
                    schedaDocumento.oggetto.daAggiornare = true;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        //private void txt_note_TextChanged(object sender, System.EventArgs e)
        //{
        //    try
        //    {
        //        schedaDocumento.note = this.txt_note.Text;
        //        schedaDocumento.daAggiornareNote = true;
        //        this.btn_salva.Enabled = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorManager.redirect(this, ex);
        //    }
        //}



        private void ddl_tipoAtto_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Session["rubrica.campoCorrispondente"] != null)
                Session.Remove("rubrica.campoCorrispondente");
            if (Session["CorrSelezionatoDaMulti"] != null)
                Session.Remove("CorrSelezionatoDaMulti");
            if (Session["dictionaryCorrispondente"] != null)
                Session.Remove("dictionaryCorrispondente");

            DocsPaWR.TipologiaAtto tipologiaAtto = new DocsPAWA.DocsPaWR.TipologiaAtto();
            tipologiaAtto.systemId = this.ddl_tipoAtto.SelectedItem.Value;
            tipologiaAtto.descrizione = this.ddl_tipoAtto.SelectedItem.Text;

            //Spostato nella pagina anteprimaProfDinamica
            //DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
            //string count = ws.CountTipoFromTipologia("CORRISPONDENTE", tipologiaAtto.descrizione);

            //Session.Add("CountCorr", count);

            schedaDocumento.tipologiaAtto = tipologiaAtto;
            schedaDocumento.daAggiornareTipoAtto = true;
            this.btn_salva.Enabled = true;
            txt_dataScadenza.Text = "";
            schedaDocumento.dataScadenza = "";

            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                //DocsPaWR.Templates template = wws.getTemplate((UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoAtto.SelectedItem.Text, schedaDocumento.docNumber);
                //if (template != null)
                //{

                string idTemplate = ddl_tipoAtto.SelectedValue;
                if (idTemplate != "")
                {
                    DocsPaWR.Templates template = ProfilazioneDocManager.getTemplateById(idTemplate, this);
                    if (template != null)
                    {
                        Session.Remove("template");
                        schedaDocumento.template = template;

                        Session.Add("refreshDxPageProf", true);
                        //ClientScript.RegisterStartupScript(this.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);

                        //Se la tipologia documento è privato allora il documento deve essere privato
                        //if (template.PRIVATO != null && template.PRIVATO == "1")
                        if (template.PRIVATO != null && template.PRIVATO == "1")
                        {
                            chkPrivato.Checked = true;
                            //chkPrivato.Enabled = false;
                            schedaDocumento.privato = "1";
                        }
                        //else
                        //    {
                        //        chkPrivato.Checked = false;
                        //        //chkPrivato.Enabled = true;
                        //        schedaDocumento.privato = "0";
                        //    }
                    }
                    else
                    {
                        //ClientScript.RegisterStartupScript(this.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=0';", true);
                        chkPrivato.Checked = false;
                        //chkPrivato.Enabled = true;
                    }
                }
            }

            if (ddl_tipoAtto.SelectedIndex == 0)
            {
                Session.Remove("template");
                Panel_DiagrammiStato.Visible = false;
                //ClientScript.RegisterStartupScript(this.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=0';", true);
            }
        }

        protected void btn_rimuovi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string alertMessage = "Il documento è stato rimosso.\\nNon è più possibile visualizzarlo.";
            // this.Response.Write("<script>alert('" + alertMessage + "');</script>");
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_modello", "alert('" + alertMessage + "');", true);

            // Redirect alla homepage di docspa
            SiteNavigation.CallContextStack.Clear();
            SiteNavigation.NavigationContext.RefreshNavigation();

            string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
            this.Response.Write(script);
        }

        private void btn_log_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string scriptString = "<SCRIPT>ApriFinestraLog('D');</SCRIPT>";
            this.Page.RegisterStartupScript("apriModalDialogLog", scriptString);
        }

        private void btn_stampa_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                try
                {
                    //controllo solo protocollati si stampa altri no.
                    InfoUtente infoUtente = UserManager.getInfoUtente(this);
                    DocsPaWR.FileDocumento fileRep = DocumentManager.getSchedaDocReport(this, schedaDocumento, infoUtente);
                    FileManager.setSelectedFileReport(this, fileRep, "../popup");
                }
                catch (Exception ex)
                {
                    ErrorManager.redirectToErrorPage(this, ex);
                }
            }
        }

        private void btn_inoltra_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                schedaDocumento = DocumentManager.InoltraDocumento(this, schedaDocumento, false);
                FileManager.removeSelectedFile(this);
                schedaDocumento.predisponiProtocollazione = true;
                DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                Response.Write("<script language='javascript'>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=protocollo';</script>");

            }
        }

        /// <summary>
        /// Aggiornamento in batch delle sole note
        /// </summary>
        protected virtual void UpdateNote()
        {
            AssociazioneNota oggettoAssociato = new AssociazioneNota();
            oggettoAssociato.TipoOggetto = OggettiAssociazioniNotaEnum.Documento;
            oggettoAssociato.Id = this.schedaDocumento.systemId;

            // Inserimento della nota creata
            this.dettaglioNota.Save();

            // Aggiornamento delle note sul backend
            this.schedaDocumento.noteDocumento = Note.NoteManager.Update(oggettoAssociato, this.schedaDocumento.noteDocumento);

            // Disabilitazione del dettaglio nota e del pulsante salva
            this.dettaglioNota.Enabled = false;
            this.btn_salva.Enabled = false;
        }

        private void btn_salva_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            logger.Info("BEGIN");
            if (Session["templateRiproposto"] != null)
                Session.Remove("templateRiproposto");
            if (Session["rubrica.campoCorrispondente"] != null)
                Session.Remove("rubrica.campoCorrispondente");

            if (!this.GetControlAclDocumento().AclRevocata)
            {
                if (this.dettaglioNota.ReadOnly && this.dettaglioNota.IsDirty)
                {
                    this.UpdateNote();
                }
                // Se il documento è in checkout non può in alcun modo essere salvato
                else if (this.schedaDocumento.checkOutStatus == null)
                {
                    string msg = CheckFields(this.schedaDocumento);

                    msg += ProfilazioneDocManager.VerifyAndSetTipoDoc(UserManager.getInfoUtente(this), ref schedaDocumento, this);

                    if (msg.Equals(""))
                    {
                        bool isPrivPers = false;
                        string messaggio = "";
                        //Aggiunto controllo se il documento è privato e si è scelto di fare 
                        //una trasmissione rapida oppure se il documento è privato e si è scelto di 
                        //trasformarlo in pubblico (quest'ultimo cosa al momento non si fa +)
                        //if ( ((schedaDocumento.privato == "1") && (ddl_tmpl.SelectedIndex != 0)) || (schedaDocumento.daAggiornarePrivato == true) )
                        if (schedaDocumento.privato == "1")
                        {
                            if ((ddl_tmpl.SelectedIndex != 0) && (txt_CodFascicolo.Text != ""))
                            {
                                messaggio = InitMessageXml.getInstance().getMessage("fasc_trasmDocPrivato");
                            }
                            //else if (ddl_tmpl.SelectedIndex != 0)
                            //    messaggio = InitMessageXml.getInstance().getMessage("trasmDocPrivato");
                            else if (!String.IsNullOrEmpty(txt_CodFascicolo.Text) && FascicoliManager.getFascicoloSelezionatoFascRapida(this) != null && FascicoliManager.getFascicoloSelezionatoFascRapida(this).tipo != "G")
                                messaggio = InitMessageXml.getInstance().getMessage("insDocPrivato");
                            if (messaggio != "")
                            {
                                isPrivPers = true;
                                msg_TrasmettiDoc.Confirm(messaggio);
                            }
                        }

                        if (schedaDocumento.personale == "1")
                        {
                            if ((ddl_tmpl.SelectedIndex != 0) && (txt_CodFascicolo.Text != ""))
                            {
                                messaggio = InitMessageXml.getInstance().getMessage("fasc_trasmDocPersonale");
                            }
                            else
                                if (ddl_tmpl.SelectedIndex != 0)
                                    messaggio = InitMessageXml.getInstance().getMessage("trasmDocPersonale");
                                else if (txt_CodFascicolo.Text != "" && FascicoliManager.getFascicoloSelezionatoFascRapida(this) != null && FascicoliManager.getFascicoloSelezionatoFascRapida(this).tipo != "G")
                                    messaggio = InitMessageXml.getInstance().getMessage("insDocPersonale");
                            if (messaggio != "")
                            {
                                isPrivPers = true;
                                msg_TrasmettiDoc.Confirm(messaggio);
                            }
                        }

                        Ruolo ruoloUtente = UserManager.getRuolo();
                        DocsPaWR.Registro[] registriRf = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "1", "");
                        bool okRF = true;

                        // verifico se è stata selezionata una nota di RF e se si sia selezionato un RF corretto nel caso di utenti con 2 RF almeno
                        if (registriRf != null && registriRf.Length > 1 && (dettaglioNota.TipoVisibilita == TipiVisibilitaNotaEnum.RF))
                        {
                            try
                            {
                                this.dettaglioNota.Save();
                            }
                            catch (Exception exc)
                            {
                                okRF = false;
                                RegisterStartupScript("alertRFNote", "<script>alert('" + exc.Message + "');</script>");
                            }
                        }
                        
                        if (okRF && !isPrivPers)
                        {
                            salva();

                            //Emanuela :se il salvataggio del documento è andato a buon fine e se il documento è di tipo fatturazione
                            //elettronica il primo utente che effettua il salva ne acquisisce i diritti
                            if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.systemId))
                            {
                                if (((ImageButton)FindControl("btn_salva")).AlternateText.Equals("Salva e acquisisci diritti"))
                                {
                                    //acquisisce i diritti
                                    DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                                    bool result = ws.AcquisisciDirittiDocumento(schedaDocumento, UserManager.getInfoUtente(this));
                                    if(!result)
                                        Response.Write("<script>alert('I diritti per il documento di tipo fatturazione elettronica non sono stati acquisiti');</script>");
                                }
                            }
                        }
                    }
                    else
                    {
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Messaggio", "alert('" + msg + "');", true);
                        //Response.Write("<script>alert('" + msg + "');</script>");
                    }
                }
                else
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_bloccato", "<script>alert('Impossibile salvare i dati del documento in quanto risulta bloccato');", true);
                    // Response.Write("<script>alert('Impossibile salvare i dati del documento in quanto risulta bloccato');</script>");
                }
            }
            
            logger.Info("END");
        }

        private void btn_prodisponiProtocollo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            logger.Info("BEGIN");
            if (Session["templateRiproposto"] != null)
                Session.Remove("templateRiproposto");

            if (!this.GetControlAclDocumento().AclRevocata)
            {
                // Se il documento è in checkout non può in alcun modo essere salvato
                if (this.schedaDocumento.checkOutStatus == null)
                {
                    string messaggio = "";
                    try
                    {
                        if (schedaDocumento.modificaRispostaDocumento == true)
                        {
                            string message = "Risposta al documento non salvata";
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Message", "alert('" + message + "');", true);
                            // Response.Write("<script>alert('" + message + "');</script>");
                            return;

                        }
                        if (this.CheckProtoEnabled())
                        {
                            //Verifica se il documento da predisporre è personale, in tal caso si notifica 
                            //(e si chiede conferma) all'utente che il documento verrà trasformato 
                            //da personale a privato.
                            if (schedaDocumento.personale == "1")
                            {
                                //Solo il proprietario di un documento personale può portarlo nello stato privato.
                                //Verifica dunque che l'utente che vuole predisporre e il proprietario del 
                                //documento siano la stessa persona
                                if (UserManager.getInfoUtente(this).idPeople.Equals(schedaDocumento.creatoreDocumento.idPeople))
                                {
                                    //Controllo se il ruolo utente è autorizzato a creare documenti privati
                                    if (UserManager.ruoloIsAutorized(this, "DO_CREA_PRIVATO"))
                                    {
                                        messaggio = InitMessageXml.getInstance().getMessage("docPredisponiPersonale");
                                        msg_PersonaleDoc.Confirm(messaggio);
                                    }
                                    else
                                    {
                                        messaggio = InitMessageXml.getInstance().getMessage("docPredisponiPersonale-NO");
                                        //Response.Write("<script>alert(\"" + messaggio + "\");</script>");
                                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Prediuno", "alert('" + messaggio + "');", true);
                                        //Response.Write("<script>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';</script>");
                                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "predidue", "top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';", true);
                                    }
                                }
                                else
                                {
                                    messaggio = "Utente non abilitato alla protocollazione";
                                    // Response.Write("<script>alert('" + messaggio + "');</script>");
                                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Predidue", "alert('" + messaggio + "');", true);
                                    // Response.Write("<script>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';</script>");
                                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Preditre", "top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';", true);
                                }
                            }
                            else
                            {
                                FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                                FascicoliManager.removeCodiceFascRapida(this);
                                FascicoliManager.removeDescrizioneFascRapida(this);
                                predisponi();
                            }
                        }
                        else
                        {
                            string message = "Utente non abilitato alla protocollazione";
                            //Response.Write("<script>alert('" + message + "');</script>");
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_abiuno", "alert('" + message + "');", true);
                            //Response.Write("<script>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';</script>");
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_abidue", "top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.redirect(this, ex);
                    }
                }
                else
                {
                    // Response.Write("<script>alert('Impossibile salvare i dati del documento in quanto risulta bloccato');</script>");
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_salvare", "alert('Impossibile salvare i dati del documento in quanto risulta bloccato');", true);
                }
            }
            logger.Info("END");
        }

        private void predisponi()
        {
            logger.Info("BEGIN");
            DocsPaWR.SchedaDocumento mySchedaDoc = DocumentManager.getDocumentoSelezionato(this);
            mySchedaDoc.predisponiProtocollazione = true;
            
            if (this.IsRoleOutwardEnabled())
            {
                mySchedaDoc.tipoProto = "P";
                mySchedaDoc.protocollo = new DocsPAWA.DocsPaWR.ProtocolloUscita();
            }
            else if (this.IsRoleInwardEnabled())
            {
                mySchedaDoc.tipoProto = "A";
                mySchedaDoc.protocollo = new DocsPAWA.DocsPaWR.ProtocolloEntrata();
            }
            else if (this.IsRoleInternalEnabled())
            {
                mySchedaDoc.tipoProto = "I";
                mySchedaDoc.protocollo = new DocsPAWA.DocsPaWR.ProtocolloInterno();
            }
            
            //Se un documento predisposto ha una tipologia sospesa, al predisposto non viene impostata nessuna tipologia di documento
            if (mySchedaDoc != null && mySchedaDoc.template != null && mySchedaDoc.template.IN_ESERCIZIO.ToUpper().Equals("NO"))
            {
                mySchedaDoc.template = null;
                mySchedaDoc.tipologiaAtto = null;
            }
            
            btn_salva.Enabled = true;
            Session["saveButtonEnabled"] = true;
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Tab_gestione", "top.principale.iFrame_sx.document.location= 'tabGestioneDoc.aspx?tab=protocollo';", true);
            logger.Info("END");
        }

        private void btn_aggiungi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (this.schedaDocumento.tipoProto != "C" && this.schedaDocumento.tipoProto != "R")
            {
                if (!this.GetControlAclDocumento().AclRevocata)
                {
                    try
                    {
                        int isInAdl = DocumentManager.isDocInADL(this.schedaDocumento.systemId, this);
                        string msg = string.Empty;
                        if (isInAdl == 1)
                        {
                            DocumentManager.eliminaDaAreaLavoro(this.Page, this.schedaDocumento.systemId, null);
                            msg = "Documento rimosso dall'Area di Lavoro";
                        }
                        else
                        {
                            DocumentManager.addAreaLavoro(this, schedaDocumento);
                            msg = "Documento aggiunto all'area di lavoro";
                        }
                        //Response.Write("<script>alert(\"" + msg + "\");</script>");
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_aggiunto", "alert(\"" + msg + "\");", true);
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.redirect(this, ex);
                    }
                }
            }
            else
                ClientScript.RegisterStartupScript(this.GetType(), "alertStampe", "alert('Non è possibile inserire una stampa in Area Di Lavoro');", true);
        }

        private void chkPrivato_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.chkPrivato.Checked)
            {
                schedaDocumento.privato = "1";
                schedaDocumento.personale = "0";
                chkUtente.Enabled = false;
            }
            else
            {
                chkUtente.Enabled = true; ;
                schedaDocumento.privato = "0";
            }
        }

        private void chkUtente_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.chkUtente.Checked)
            {
                chkPrivato.Enabled = false;
                schedaDocumento.personale = "1";
                schedaDocumento.privato = "0";
            }
            else
            {
                chkPrivato.Enabled = true;
                schedaDocumento.personale = "0";
            }
        }

        private void txt_numOggetto_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                schedaDocumento.numOggetto = this.txt_numOggetto.Text;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void btn_eliminaParoleChiave_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                if (this.lbx_paroleChiave.SelectedIndex >= 0)
                {
                    DocsPaWR.DocumentoParolaChiave[] listaParole;
                    listaParole = schedaDocumento.paroleChiave;
                    schedaDocumento.paroleChiave = DocumentManager.removeParoleChiave(listaParole, this.lbx_paroleChiave.SelectedIndex);
                    schedaDocumento.daAggiornareParoleChiave = true;
                    this.lbx_paroleChiave.Items.RemoveAt(this.lbx_paroleChiave.SelectedIndex);
                    this.btn_salva.Enabled = true;
                }
            }
        }

        private void btn_aggiungiFirmatario_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                if (this.txt_CognomeFirma.Text.Trim().Equals(""))
                {
                    //Response.Write("<script>alert('" + "Inserire il COGNOME del firmatario" + "');</script>");
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_dati", "alert('" + "Inserire il COGNOME del firmatario" + "');", true);
                    return;
                }
                DocsPaWR.Firmatario firmatario = new DocsPAWA.DocsPaWR.Firmatario();
                firmatario.nome = this.txt_NomeFirma.Text;
                firmatario.cognome = this.txt_CognomeFirma.Text;
                DocsPaWR.Firmatario[] listaFirmatari = null;
                if (schedaDocumento.documenti == null || schedaDocumento.documenti.Length < 1)
                {
                    schedaDocumento.documenti = new DocsPAWA.DocsPaWR.Documento[1];
                    schedaDocumento.documenti[0] = new DocsPAWA.DocsPaWR.Documento();
                }
                else
                {
                    if (schedaDocumento.documenti.Length > 0)
                        if (schedaDocumento.documenti[0].firmatari != null)
                            listaFirmatari = schedaDocumento.documenti[0].firmatari;
                }
                if (!UserManager.esisteFirmatario(listaFirmatari, firmatario))
                {
                    schedaDocumento.documenti[0].firmatari = UserManager.addFirmatario(listaFirmatari, firmatario);
                    schedaDocumento.documenti[0].daAggiornareFirmatari = true;
                }
                this.txt_NomeFirma.Text = "";
                this.txt_CognomeFirma.Text = "";
            }
        }

        private void btn_cancFirmatario_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                if (this.lbx_firmatari.SelectedIndex >= 0)
                    removeFirmatari(this.lbx_firmatari.SelectedIndex);
            }
        }

        private void removeFirmatari(int index)
        {
            DocsPaWR.Firmatario[] listaFirmatari;
            listaFirmatari = schedaDocumento.documenti[0].firmatari;
            schedaDocumento.documenti[0].firmatari = UserManager.removeFirmatari(listaFirmatari, index);
            schedaDocumento.documenti[0].daAggiornareFirmatari = true;
        }

        private void rbl_sesso_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.rbl_sesso.SelectedIndex != -1)
            {
                this.schedaDocumento.tipoSesso = this.rbl_sesso.SelectedIndex.ToString();
            }
            else
            {
                this.schedaDocumento.tipoSesso = "0";
            }
        }

        private void ddl_eta_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.ddl_eta.SelectedIndex != -1)
            {
                this.schedaDocumento.idFasciaEta = this.ddl_eta.SelectedIndex;
            }
            else
            {
                this.schedaDocumento.idFasciaEta = 0;
            }
        }

        private void ddl_commRef_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (this.ddl_commRef.SelectedIndex > 0)
                    schedaDocumento.commissioneRef = this.ddl_commRef.SelectedItem.Value;
                else
                    schedaDocumento.commissioneRef = null;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void ctrl_oggetto_OggChanged(object sender, Oggetto.OggTextEvent e)
        {
            try
            {
                DocsPaWR.Oggetto ProtoObj = new DocsPAWA.DocsPaWR.Oggetto();
                ProtoObj.descrizione = e.Testo;
                schedaDocumento.oggetto = ProtoObj;
                if (schedaDocumento.systemId != null)
                    schedaDocumento.oggetto.daAggiornare = true;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void btn_modificaOgget_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                this.ctrl_oggetto.oggetto_isReadOnly = false;
                this.btn_oggettario.Enabled = true;
                //			enableSubjectEditing = true;
                this.btn_salva.Enabled = true;
            }
        }

        protected void btn_addTipoAtto_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                if (!Page.IsStartupScriptRegistered("ApriTipologiaAtto"))
                {
                    Page.RegisterStartupScript("ApriTipologiaAtto", "<script>ApriFinestraTipologiaAtto();</script>");
                }
            }
        }

        /// <summary>
        /// Torna alla visualizzazione documenti del folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_BackToFolder_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                Response.Write(FascicoliManager.FolderViewReloadScript());
            }
        }

        private bool CheckProtoEnabled()
        {
            bool Result = false;
            if (this.IsRoleInwardEnabled())
                return true;
            if (this.IsRoleOutwardEnabled())
                return true;
            if (this.IsRoleInternalEnabled())
                return true;
            return Result;
        }

        private bool IsRoleInwardEnabled()
        {
            bool result = false;
            if (UserManager.getRuolo(this) != null)
            {
                DocsPaWR.Ruolo ruo = UserManager.getRuolo(this);
                for (int i = 0; i < ruo.funzioni.Length; i++)
                {
                    if (ruo.funzioni[i].descrizione == "PROTO_IN")
                    {
                        result = true;
                        break;
                    }
                }
                if (!result)
                    result = UserManager.ruoloIsAutorized(this, "DO_PRO_PREDISPONI") &&
                       UserManager.ruoloIsAutorized(this, "DO_PRO_SALVA");

            }
            return result;
        }

        private bool IsRoleOutwardEnabled()
        {
            bool result = false;
            if (UserManager.getRuolo(this) != null)
            {
                DocsPaWR.Ruolo ruo = UserManager.getRuolo(this);
                for (int i = 0; i < ruo.funzioni.Length; i++)
                {
                    if (ruo.funzioni[i].descrizione == "PROTO_OUT")
                    {
                        result = true;
                        break;
                    }
                }
                if (!result)
                    result = UserManager.ruoloIsAutorized(this, "DO_PRO_PREDISPONI") &&
                       UserManager.ruoloIsAutorized(this, "DO_PRO_SALVA");

            }
            return result;
        }

        private bool IsRoleInternalEnabled()
        {
            bool result = false;
            if (UserManager.getRuolo(this) != null)
            {
                DocsPaWR.Ruolo ruo = UserManager.getRuolo(this);
                for (int i = 0; i < ruo.funzioni.Length; i++)
                {
                    if (ruo.funzioni[i].descrizione == "PROTO_OWN")
                    {
                        result = true;
                        break;
                    }
                }
                if (!result)
                    result = UserManager.ruoloIsAutorized(this, "DO_PRO_PREDISPONI") &&
                       UserManager.ruoloIsAutorized(this, "DO_PRO_SALVA");

            }
            return result;
        }

        private void nuovaSchedaDocumento()
        {
            schedaDocumento = DocumentManager.getDocumentoSelezionato(this);

            if (schedaDocumento == null)
            {
                // Creazione di una nuova scheda documento
                schedaDocumento = DocumentManager.NewSchedaDocumento(this.Page);

                if (schedaDocumento.repositoryContext != null)
                    schedaDocumento.repositoryContext.IsDocumentoGrigio = true;

                //// Impostazione della nuova scheda documento e del primo documento in sessione
                //FileManager.setSelectedFile(this, schedaDocumento.documenti[0]);
                //DocumentManager.setDocumentoInLavorazione(schedaDocumento);
                //DocumentManager.setDocumentoSelezionato(schedaDocumento);
            }
        }

        private void verificaHMdiritti()
        {
            //disabilitazione dei bottoni in base all'autorizzazione di HM 
            //sul documento
            if (schedaDocumento != null && schedaDocumento.accessRights != null && schedaDocumento.accessRights != "")
            {
                bool inCestino = (schedaDocumento.inCestino != null && schedaDocumento.inCestino == "1");
                bool inArchivio = (schedaDocumento.inArchivio != null && schedaDocumento.inArchivio == "1");

                if (UserManager.disabilitaButtHMDiritti(schedaDocumento.accessRights) ||
                                inCestino || inArchivio)
                {
                    //bottoni che devono essere disabilitati in caso
                    //di diritti di sola lettura
                    this.btn_addTipoAtto.Enabled = false;
                    this.btn_aggiungiFirmatario.Enabled = false;
                    this.btn_aggiungiFirmatario.Enabled = false;

                    if (this.dettaglioNota.ReadOnly)
                    {
                        this.dettaglioNota.Enabled = (!inCestino && !inArchivio);

                        this.btn_salva.Enabled = this.dettaglioNota.Enabled;
                    }
                    else
                        this.btn_salva.Enabled = false;

                    this.btn_rimuovi.Enabled = false;
                    this.btn_eliminaParoleChiave.Enabled = false;
                    this.btn_prodisponiProtocollo.Enabled = false;
                    this.btn_modificaOgget.Enabled = false;
                    this.btn_cancFirmatario.Enabled = false;
                    this.btn_selezionaParoleChiave.Enabled = false;
                    this.btn_oggettario.Enabled = false;
                    this.ctrl_oggetto.oggetto_isReadOnly = true;

                    this.chkPrivato.Enabled = false;
                    this.chkUtente.Enabled = false;

                    //bottoni che devono essere disabilitati in caso
                    //di documento trasmesso con "Worflow" e ancora da accettare
                    if (UserManager.disabilitaButtHMDirittiTrasmInAccettazione(schedaDocumento.accessRights))
                    {
                        this.btn_stampaSegn.Enabled = false;
                        this.btn_aggiungi.Enabled = false;
                        this.ddl_tipoAtto.Enabled = false;
                    }

                    this.ddl_tipoAtto.Enabled = false;
                    this.btn_riproponi.Enabled = this.IsEnabledRiproponiConConoscenza;
                    // PALUMBO: abilitata freccia destra per catene in caso di doc in stato finale
                    //this.btn_risp_dx.Enabled = false;
                    this.btn_risp_dx.Enabled = true;

                    this.btn_in_risposta_a.Enabled = false;
                    this.btn_inoltra.Visible = false;
                    this.btn_Risp.Enabled = false;
                    this.btn_risp_grigio.Enabled = false;
                }
            }
        }

        public void tastoInvio()
        {
            ctrl_oggetto.DefButton_Ogg(ref btn_salva);
            //Utils.DefaultButton(this, ref txt_note, ref btn_salva);
        }

        #region Diagrammi di Stato
        private bool controllaStatoFinale()
        {
            DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"];
            for (int i = 0; i < dg.STATI.Length; i++)
            {
                DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)dg.STATI[i];
                if (st.SYSTEM_ID.ToString() == ddl_statiSuccessivi.SelectedValue && st.STATO_FINALE)
                    return true;
            }
            return false;
        }

        private void popolaComboBoxStatiSuccessivi(DocsPAWA.DocsPaWR.Stato stato, DocsPAWA.DocsPaWR.DiagrammaStato diagramma)
        {
            int selectedIndex = this.ddl_statiSuccessivi.SelectedIndex;
            //Inizializzazione
            ddl_statiSuccessivi.Items.Clear();
            ListItem itemEmpty = new ListItem();
            ddl_statiSuccessivi.Items.Add(itemEmpty);

            //Popola la combo con gli stati iniziali del diagramma
            if (stato == null)
            {
                lbl_statoAttuale.Text = "";
                for (int i = 0; i < diagramma.STATI.Length; i++)
                {
                    DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)diagramma.STATI[i];
                    if (st.STATO_INIZIALE && DiagrammiManager.IsRuoloAssociatoStatoDia(diagramma.SYSTEM_ID.ToString(), UserManager.getRuolo().idGruppo, st.SYSTEM_ID.ToString()))
                    {
                        ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID)); 
                        ddl_statiSuccessivi.Items.Add(item);
                        if (st.STATO_SISTEMA) 
                            item.Enabled = false;
                    }
                }
                if (ddl_statiSuccessivi.Items.Count == 2)
                    selectedIndex = 1;
            }
            //Popola la combo con i possibili stati, successivi a quello passato
            else
            {
                for (int i = 0; i < diagramma.PASSI.Length; i++)
                {
                    DocsPaWR.Passo step = (DocsPAWA.DocsPaWR.Passo)diagramma.PASSI[i];
                    if (step.STATO_PADRE.SYSTEM_ID == stato.SYSTEM_ID)
                    {
                        for (int j = 0; j < step.SUCCESSIVI.Length; j++)
                        {
                            DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)step.SUCCESSIVI[j];
                            if (DiagrammiManager.IsRuoloAssociatoStatoDia(diagramma.SYSTEM_ID.ToString(), UserManager.getRuolo().idGruppo, st.SYSTEM_ID.ToString()))
                            {
                                ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                                if (st.STATO_SISTEMA)
                                    item.Enabled = false;
                                ddl_statiSuccessivi.Items.Add(item);
                            }
                        }
                    }
                }
            }

            if (selectedIndex < this.ddl_statiSuccessivi.Items.Count)
                this.ddl_statiSuccessivi.SelectedIndex = selectedIndex;
        }
        #endregion

        private void btn_salva_disabled_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
        }


        protected void btn_CampiPersonalizzati_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
            }
        }

        private void img_btnStoriaDiagrammi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                if (DocumentManager.getDocumentoSelezionato(this) != null && DocumentManager.getDocumentoSelezionato(this).docNumber != "")
                {
                    DataSet storico = DocsPAWA.DiagrammiManager.getDiagrammaStoricoDoc(DocumentManager.getDocumentoSelezionato(this).docNumber, this);
                    if (storico == null || storico.Tables[0].Rows.Count == 0)
                    {
                        RegisterStartupScript("chiudiFinestra", "<script>alert('Non esiste uno storico degli stati per il documento corrente !');</script>");
                    }
                    else
                    {
                        RegisterStartupScript("apriStoricoStati", "<script>apriStoricoStati();</script>");
                    }
                }
            }
        }

        #region MessageBox
        private void MessageBox_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            string idTemplate = ProfilazioneDocManager.getIdTemplate(schedaDocumento.docNumber, this);
            DocsPaWR.ModelloTrasmissione modelloTrasm = null;

            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                if (Session["docSolaLettura"] != null && (string)Session["docSolaLettura"] == "SI")
                {
                    DocsPAWA.DiagrammiManager.salvaModificaStato(schedaDocumento.docNumber, (string)Session["statoFinale"], (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"], UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), txt_dataScadenza.Text, this);

                    // Aggiornamento stato di consolidamento del documento successivamente al passaggio ad uno stato successivo
                    this.schedaDocumento.ConsolidationState = this.wws.GetDocumentConsolidationState(UserManager.getInfoUtente(), this.schedaDocumento.systemId);

                    if (idTemplate != "")
                    {
                        ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAuto(UserManager.getInfoUtente(this).idAmministrazione, (string)Session["statoFinale"], idTemplate, this));
                        for (int i = 0; i < modelli.Count; i++)
                        {
                            DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                            if (mod.SINGLE == "1")
                            {
                                TrasmManager.effettuaTrasmissioneDocDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, schedaDocumento, this);
                            }
                            else
                            {
                                for (int k = 0; k < mod.MITTENTE.Length; k++)
                                {
                                    if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                    {
                                        TrasmManager.effettuaTrasmissioneDocDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, schedaDocumento, this);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    DocsPaVO.HMDiritti.HMdiritti diritti = new DocsPaVO.HMDiritti.HMdiritti();
                    wws.cambiaDirittiDocumenti(diritti.HMdiritti_Read, schedaDocumento.systemId);
                    schedaDocumento.accessRights = Convert.ToString(diritti.HMdiritti_Read);
                }
            }
            ClientScript.RegisterStartupScript(this.GetType(), "apriPopUp", "top.principale.iFrame_dx.document.location='tabDoc.aspx?profilazione=1';", true);
            Session.Add("docSolaLettura", "NO");
            //Session.Add("statoAutomatico","NO");
        }

        private void MessageBox1_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            string idTemplate = ProfilazioneDocManager.getIdTemplate(schedaDocumento.docNumber, this);
            DocsPaWR.ModelloTrasmissione modelloTrasm = null;

            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                if (Session["statoAutomatico"] != null && (string)Session["statoAutomatico"] == "SI")
                {
                    DocsPAWA.DiagrammiManager.salvaModificaStato(schedaDocumento.docNumber, (string)Session["statoSelezionato"], (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"], UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), txt_dataScadenza.Text, this);

                    // Aggiornamento stato di consolidamento del documento successivamente al passaggio ad uno stato successivo
                    this.schedaDocumento.ConsolidationState = this.wws.GetDocumentConsolidationState(UserManager.getInfoUtente(), this.schedaDocumento.systemId);

                    if (idTemplate != "")
                    {
                        ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAuto(UserManager.getInfoUtente(this).idAmministrazione, (string)Session["statoSelezionato"], idTemplate, this));
                        for (int i = 0; i < modelli.Count; i++)
                        {
                            DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                            if (mod.SINGLE == "1")
                            {
                                TrasmManager.effettuaTrasmissioneDocDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, schedaDocumento, this);
                            }
                            else
                            {
                                for (int k = 0; k < mod.MITTENTE.Length; k++)
                                {
                                    if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                    {
                                        TrasmManager.effettuaTrasmissioneDocDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, schedaDocumento, this);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ClientScript.RegisterStartupScript(this.GetType(), "ricaricaSinistra", "top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';", true);
            Session.Add("statoAutomatico", "NO");
        }

        private void msg_TrasmettiDoc_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                salva();
                
                //Emanuela :se il salvataggio del documento è andato a buon fine e se il documento è di tipo fatturazione
                //elettronica il primo utente che effettua il salva ne acquisisce i diritti
                if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.systemId))
                {
                    if (((ImageButton)FindControl("btn_salva")).AlternateText.Equals("Salva e acquisisci diritti"))
                    {
                        //acquisisce i diritti
                        DocsPaWR.DocsPaWebService ws = new DocsPaWebService();
                        bool result = ws.AcquisisciDirittiDocumento(schedaDocumento, UserManager.getInfoUtente(this));
                        if (!result)
                            Response.Write("<script>alert('I diritti per il documento di tipo fatturazione elettronica non sono stati acquisiti');</script>");
                    }
                }
            }
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Cancel)
            {
                this.chkPrivato.Enabled = true;
                this.chkUtente.Enabled = true;
            }

        }

        private void msg_PersonaleDoc_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                //se dco personale e ho ocnfermato al messsaggio, allora lo trasformo in privato
                if (this.schedaDocumento.personale != null && schedaDocumento.personale == "1")
                {
                    this.schedaDocumento.personale = "0";
                    this.schedaDocumento.privato = "1";

                }
                predisponi();
            }
        }
        #endregion

        private void salva()
        {
            logger.Info("BEGIN");
            try
            {
                bool daAggiornareUffRef;
                //DocsPaWR.TemplateTrasmissione template;
                bool nuovoDoc = true;
                string segnatura = "";

                /* ABBATANGELI GIANLUIGI - TEST
                 * Commentato il codice sottostante che scrive inutilmente dati in sessione 
                 * 
                //PROFILAZIONE DINAMICA
                string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
                if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
                {
                    //Salvataggio dei campi della profilazione dinamica
                    if (Session["template"] != null)
                    {
                        if (!ProfilazioneDocManager.verificaCampiObbligatori((DocsPAWA.DocsPaWR.Templates)Session["template"]))
                        {
                            if (ProfilazioneDocManager.verificaOkContatore((DocsPAWA.DocsPaWR.Templates)Session["template"]) == string.Empty)
                            {
                                //wws.salvaInserimentoUtenteProfDim(UserManager.getInfoUtente(this), (DocsPaWR.Templates)Session["template"], schedaDocumento.docNumber, "");
                                schedaDocumento.template = (DocsPaWR.Templates)Session["template"];
                                //Session["template"] = wws.getTemplate((UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoAtto.SelectedItem.Text, schedaDocumento.docNumber);
                                Session["template"] = ProfilazioneDocManager.getTemplateDettagli(schedaDocumento.docNumber, this);
                            }
                        }
                    }
                }
                //FINE PROFILAZIONE DINAMICA

                * */
                // Save dati nota
                this.dettaglioNota.Save();
                DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);

                //caso modifica
                if (schedaDocumento.systemId != null && !schedaDocumento.systemId.Equals(""))
                {
                    //modifica del 15/05/2009
                    String temp = schedaDocumento.tipoProto;
                    //fine nodifica del 15/05/2009
                    schedaDocumento = DocumentManager.salva(this, schedaDocumento, false, out daAggiornareUffRef);
                    //modifica del 18/05/2009
                    schedaDocumento.tipoProto = temp;
                    //fine modifica del 15/05/2009
                    if (schedaDocumento.tipologiaAtto != null)
                        schedaDocumento.daAggiornareTipoAtto = false;
                    nuovoDoc = false;
                }
                else //caso creazione documento grigio
                {
                    SetOggetto();
                    string valoreChiaveConsentiClass = string.Empty;
                    valoreChiaveConsentiClass = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
                    if (fasc != null && fasc.systemID != null && fasc.isFascConsentita != null && fasc.isFascConsentita == "0" && !string.IsNullOrEmpty(valoreChiaveConsentiClass) && valoreChiaveConsentiClass.Equals("1"))
                    {
                        nuovoDoc = false;
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_fasc", "alert('Non è possibile inserire documenti nel fascicolo selezionato. Selezionare un nodo foglia.');", true);
                    }
                    else
                    {
                        schedaDocumento = DocumentManager.creaDocumentoGrigio(this, schedaDocumento);
                        if (schedaDocumento.tipologiaAtto != null)
                            schedaDocumento.daAggiornareTipoAtto = false;
                        Session.Add("isDocCreato", true);
                    }
                }
                if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.systemId))
                {
                    // se la creazione del doc è andata a buon fine ..
                    //eseguo la fascicolazione RAPIDA
                    if (fasc != null && fasc.systemID != null)
                    {
                        if (schedaDocumento.protocollo != null)
                            segnatura = schedaDocumento.protocollo.segnatura;

                        string msg = string.Empty;
                        if (!DocumentManager.fascicolaRapida(this, schedaDocumento.systemId, schedaDocumento.docNumber, segnatura, fasc, out msg))
                        {
                            if (string.IsNullOrEmpty(msg))
                                msg = "Il documento non è stato fascicolato";
                            Response.Write("<script>alert('" + msg + "');</script>");
                        }
                        else
                        {
                            schedaDocumento.fascicolato = "1";
                        }
                    }
                }

                if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.systemId))
                {
                    //metodo per la trasmissione rapida (templ & Modelli)
                    execTrasmRapida();
                }
                DocumentManager.setDocumentoSelezionato(this, schedaDocumento);

                if (nuovoDoc)
                {
                    //ricarica il frame destro
                    string funct_dx = "top.principale.iFrame_dx.document.location='tabDoc.aspx'; ";
                    //Response.Write("<script> " + funct_dx + "</script>");
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_funct", "" + funct_dx + "", true);

                    //riportato dalla 2.5 by massimo digregorio 
                    //alert per confermare l'avvenuta operazione
                    //se questo parametro nel web.config della wa non è presente o è a "0" allora niente alert!
                    string result = "Operazione avvenuta con successo.\\n\\n";
                    string alert_conferma_creazione = ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_ALERT_PROFILO_CREATO);
                    if ((alert_conferma_creazione != null) && (alert_conferma_creazione.Equals("1")))
                    {
                        result = result + " num. di documento: [ " + schedaDocumento.docNumber + " ]";
                        // Response.Write("<script>alert('" + result + "');</script>");
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Ok", "alert('" + result + "');", true);
                    }
                }

                //Response.Write("<script>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';</script>");				

                //PROFILAZIONE DINAMINCA
                string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
                if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
                {
                    ////Salvataggio dei campi della profilazione dinamica
                    //if (Session["template"] != null)
                    //{
                    //    if (!verificaCampiObbligatori((DocsPAWA.DocsPaWR.Templates)Session["template"]))
                    //    {
                    //        //wws.salvaInserimentoUtenteProfDim(UserManager.getInfoUtente(this), (DocsPaWR.Templates)Session["template"], schedaDocumento.docNumber, "");
                    //        schedaDocumento.template = (DocsPaWR.Templates)Session["template"];
                    //        Session["template"] = wws.getTemplate((UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoAtto.SelectedItem.Text, schedaDocumento.docNumber);                            
                    //    }
                    //}

                    //DIAGRAMMI DI STATO
                    if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                    {
                        if (this.ddl_statiSuccessivi.SelectedItem != null)
                        {
                            if (Session["DiagrammaSelezionato"] != null && ddl_statiSuccessivi.SelectedItem.Value != "")
                            {
                                bool statoFinale = false;
                                bool statoAutomatico = false;
                                Session.Add("statoSelezionato", ddl_statiSuccessivi.SelectedItem.Value);

                                //Controllo se lo stato selezionato è finale
                                if (controllaStatoFinale())
                                {
                                    if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(schedaDocumento.systemId, schedaDocumento.docNumber, UserManager.getInfoUtente(this), true))
                                    {
                                        RegisterStartupScript("docInCheckOut", "<script>alert('Attenzione non è possibile passare in uno stato finale. Documento o allegati bloccati !');</script>");
                                        return;
                                    }
                                    Session.Add("statoFinale", ddl_statiSuccessivi.SelectedValue);
                                    Session.Add("docSolaLettura", "SI");
                                    statoFinale = true;
                                }
                                //Controllo se lo stato selezionato è uno stato automatico
                                if (DocsPAWA.DiagrammiManager.isStatoAuto(ddl_statiSuccessivi.SelectedItem.Value, Convert.ToString(((DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"]).SYSTEM_ID), this))
                                {
                                    Session.Add("statoAutomatico", "SI");
                                    statoAutomatico = true;
                                }

                                if (statoAutomatico || statoFinale)
                                {
                                    statoAutomatico = false;
                                    statoFinale = false;
                                }
                                else
                                {
                                    DocsPAWA.DiagrammiManager.salvaModificaStato(schedaDocumento.docNumber, ddl_statiSuccessivi.SelectedItem.Value, (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"], UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), txt_dataScadenza.Text, this);

                                    // Aggiornamento stato di consolidamento del documento successivamente al passaggio ad uno stato successivo
                                    this.schedaDocumento.ConsolidationState = this.wws.GetDocumentConsolidationState(UserManager.getInfoUtente(), this.schedaDocumento.systemId);

                                    string idTemplate = ProfilazioneDocManager.getIdTemplate(schedaDocumento.docNumber, this);

                                    if (idTemplate != "")
                                    {
                                        ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAuto(UserManager.getInfoUtente(this).idAmministrazione, ddl_statiSuccessivi.SelectedItem.Value, idTemplate, this));
                                        for (int i = 0; i < modelli.Count; i++)
                                        {
                                            DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                            if (mod.SINGLE == "1")
                                            {
                                                TrasmManager.effettuaTrasmissioneDocDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, schedaDocumento, this);
                                            }
                                            else
                                            {
                                                for (int k = 0; k < mod.MITTENTE.Length; k++)
                                                {
                                                    if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                                    {
                                                        TrasmManager.effettuaTrasmissioneDocDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, schedaDocumento, this);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //else
                        //{
                        //    if (Panel_DataScadenza.Visible && schedaDocumento.dataScadenza != null && schedaDocumento.dataScadenza != "")
                        //        DocsPAWA.DiagrammiManager.salvaDataScadenzaDoc(schedaDocumento.docNumber, schedaDocumento.dataScadenza,this);
                        //}

                    }
                    //FINE DIAGRAMMI DI STATO 
                    if (Panel_DataScadenza.Visible &&
                         schedaDocumento.dataScadenza != null &&
                         schedaDocumento.dataScadenza != "" &&
                         schedaDocumento.tipologiaAtto != null &&
                         schedaDocumento.tipologiaAtto.systemId != null &&
                         schedaDocumento.tipologiaAtto.systemId != "")
                    {

                        DocsPAWA.DiagrammiManager.salvaDataScadenzaDoc(schedaDocumento.docNumber, schedaDocumento.dataScadenza, schedaDocumento.tipologiaAtto.systemId, this);
                    }
                }

                //Estensione delle funzionalità di fascicolazione e trasmissione rapida
                FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                FascicoliManager.removeCodiceFascRapida(this);
                FascicoliManager.removeDescrizioneFascRapida(this);
                //txt_DescFascicolo.Text = "";
                //txt_CodFascicolo.Text = "";

                Session.Remove("RFNote");

                Fascicolo[] listaFascicoli = DocumentManager.GetFascicoliDaDoc(this, schedaDocumento.systemId);
                if (listaFascicoli != null && listaFascicoli.Length > 0)
                    FascicoliManager.SetDoFascRapida(this, listaFascicoli[listaFascicoli.Length - 1]);

                //FINE PROFILAZIONE DINAMINCA
                //Response.Write("<script>top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';</script>");
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Profilazione", "top.principale.iFrame_sx.document.location = 'tabGestioneDoc.aspx?tab=profilo';", true);

            }
            catch (Exception ex)
            {
                ErrorManager.OpenErrorPage(this, ex, "creazione documento");
                //	ErrorManager.redirect(this, ex);
            }
            finally
            {
                // FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                //Session.Remove("validCodeFasc");
            }
            logger.Info("END");
        }

        #region CATENE
        private void gestionePulsantiCatene()
        {
            if (schedaDocumento.documentoPrincipale != null && !string.IsNullOrEmpty(schedaDocumento.documentoPrincipale.docNumber))
            {
                rispProtoPanelGrigio.Visible = false;
            }
            else
            {

                if (schedaDocumento != null && schedaDocumento.docNumber != null && schedaDocumento.docNumber != string.Empty) //doc creato
                {
                    //modifica del 15/05/2009
                    if (schedaDocumento.tipoProto.Equals("R") || schedaDocumento.tipoProto.Equals("C"))
                    {

                        this.btn_addTipoAtto.Enabled = false;
                        this.ddl_tipoAtto.Enabled = false;
                        this.btn_CampiPersonalizzati.Enabled = false;
                        this.txtDocumentoPrincipale.Enabled = false;
                        this.btnGoToDocumentoPrincipale.Enabled = false;
                        this.btn_in_risposta_a.Enabled = false;
                        this.btn_risp_dx.Enabled = false;
                        this.btn_risp_sx.Enabled = false;
                        this.btn_Risp.Enabled = false;
                        this.btn_risp_grigio.Enabled = false;
                        this.pnl_trasm_rapida.Visible = false;
                        this.pnl_fasc_rapida.Visible = false;
                        this.btn_prodisponiProtocollo.Enabled = false;
                        this.btn_stampa.Enabled = false;
                        this.btn_rimuovi.Enabled = false;
                        this.ctrl_oggetto.oggetto_isReadOnly = true;
                        this.ddl_tipoAtto.Enabled = false;
                        dettaglioNota.Enabled = false;
                        btn_eliminaParoleChiave.Enabled = false;
                        btn_selezionaParoleChiave.Enabled = false;
                        btn_oggettario.Enabled = false;
                        btn_aggiungi.Enabled = false;
                        this.tr_tipologia.Disabled = true;
                    }
                    else
                    {
                        //this.ddl_tipoAtto.Enabled = true;
                        dettaglioNota.Enabled = true;
                        btn_eliminaParoleChiave.Enabled = true;
                        btn_selezionaParoleChiave.Enabled = true;
                        btn_oggettario.Enabled = true;
                        btn_aggiungi.Enabled = true;
                        this.tr_tipologia.Disabled = false;
                        //gia esiteva
                        this.btn_Risp.Enabled = true;
                        this.btn_risp_grigio.Enabled = true;
                        //this.btn_risp_dx.Visible = true;
                        this.btn_risp_dx.Enabled = true;
                        this.btn_risp_sx.Visible = false;
                        //this.ctrl_oggetto.oggetto_isReadOnly = false;
                        //fien gia esisteva  
                    }
                    //fine modifica     

                    //presente una risposta 
                    if (schedaDocumento.rispostaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.rispostaDocumento.docNumber))
                    {
                        if (schedaDocumento != null && schedaDocumento.rispostaDocumento.segnatura != null && schedaDocumento.rispostaDocumento.segnatura != string.Empty)
                        {
                            //sto visualizzando il profilo di un doc protocollato
                            // pnl_text_risposta.Visible = false;
                            //txt_RispostaDocGrigio.Text = "";
                            this.btn_risp_sx.Visible = true;
                            pnl_text_risposta.Visible = true;
                            txt_RispostaDocGrigio.Text = schedaDocumento.rispostaDocumento.segnatura;
                        }
                        else
                        {
                            //sto visualizzando la risposta di un documento grigio
                            this.btn_risp_sx.Visible = true;
                            pnl_text_risposta.Visible = true;
                            txt_RispostaDocGrigio.Text = schedaDocumento.rispostaDocumento.docNumber;
                        }
                        btn_in_risposta_a.Enabled = false;

                    }
                    else
                    {
                        txt_RispostaDocGrigio.Text = "";
                    }


                }
                else //Nuovo
                {
                    this.btn_risp_sx.Visible = false;

                    this.btn_in_risposta_a.Enabled = true;

                    this.btn_Risp.Visible = true;
                    this.btn_Risp.Enabled = false;
                    this.btn_risp_grigio.Enabled = false;

                    //this.btn_risp_dx.Visible = true;
                    this.btn_risp_dx.Enabled = false;

                    if (schedaDocumento.rispostaDocumento != null && schedaDocumento.rispostaDocumento.docNumber != null)
                    {
                        if (schedaDocumento.rispostaDocumento.segnatura != null && schedaDocumento.rispostaDocumento.segnatura != string.Empty)
                        {
                            //sto visualizzando il profilo di un doc protocollato
                            //rispProtoPanelGrigio.Visible = true;
                            pnl_text_risposta.Visible = true;
                            txt_RispostaDocGrigio.Text = schedaDocumento.rispostaDocumento.segnatura;
                        }
                        else
                        {
                            //sto visualizzando la risposta di un documento grigio

                            pnl_text_risposta.Visible = true;
                            txt_RispostaDocGrigio.Text = schedaDocumento.rispostaDocumento.docNumber;
                        }
                    }

                }
                //if ((schedaDocumento.protocollo != null && schedaDocumento.protocollo.daProtocollare != null &&
                //    schedaDocumento.protocollo.daProtocollare.Equals("1"))
                //    || (schedaDocumento.predisponiProtocollazione == true && schedaDocumento.systemId != null))
                if ((schedaDocumento.protocollo == null) || (schedaDocumento.protocollo.daProtocollare != null &&
                    schedaDocumento.protocollo.daProtocollare.Equals("1"))
                    || (schedaDocumento.predisponiProtocollazione == true && schedaDocumento.systemId != null))
                {
                    rispProtoPanelGrigio.Visible = true;
                }
                else
                    rispProtoPanelGrigio.Visible = false;
            }
        }


        #region eventi

        protected void btn_in_risposta_a_Click(object sender, ImageClickEventArgs e)
        {
            string scriptString = "";
            if (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
                   && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1")
            {
                scriptString = "<SCRIPT>ApriFinestraDocumenti();</SCRIPT>";
                this.RegisterStartupScript("apriModalDialogTuttiProtocolli", scriptString);
            }
            scriptString = "<SCRIPT>ApriFinestraDocumentiNonProtocollati();</SCRIPT>";
            this.RegisterStartupScript("apriModalDialogProtocolli", scriptString);
        }

        protected void btn_Risp_Click(object sender, ImageClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.schedaDocumento.docNumber) || (System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] != null
              && System.Configuration.ConfigurationManager.AppSettings["CATENE_DOCUMENTALI_TRASVERSALI"] == "1"))
            {
                DocumentManager.removeMemoriaFiltriRicDoc(this);
                DocumentManager.RemoveMemoriaVisualizzaBack(this);

                SchedaDocumento schedaNewDoc = DocumentManager.NewSchedaDocumento(this);
                if (schedaDocumento.oggetto != null)
                {
                    schedaNewDoc.oggetto = schedaDocumento.oggetto;
                }
                schedaNewDoc.idPeople = schedaDocumento.idPeople;
                schedaNewDoc.userId = schedaDocumento.userId;
                schedaNewDoc.typeId = schedaDocumento.typeId;
                schedaNewDoc.appId = schedaDocumento.appId;
                schedaNewDoc.privato = "0";

                FileManager.removeSelectedFile(this);

                schedaNewDoc.registro = null;
                schedaNewDoc.tipoProto = "G";
                schedaNewDoc.protocollo = null;


                schedaNewDoc.predisponiProtocollazione = false;
                //Viene popolato l'oggetto risposta al protocollo:
                DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaDocumento);
                schedaNewDoc.rispostaDocumento = infoDoc;

                // Impostazione del flag IsDocumentoGrigio nel repository context della nuova scheda documento
                // Questo controllo fa in modo che se il documento è grigio, venga visualizzato il tab profilo attivo
                if(schedaNewDoc.repositoryContext != null)
                    schedaNewDoc.repositoryContext.IsDocumentoGrigio = schedaDocumento.protocollo == null;

                DocumentManager.setDocumentoSelezionato(this, schedaNewDoc);
                DocumentManager.setDocumentoInLavorazione(this, schedaNewDoc);

                ClientScript.RegisterStartupScript(this.GetType(), "Gestione_risposta", "top.principale.document.location = '../documento/gestioneDoc.aspx?tab=profilo&IsNew=0';", true);
            }
        }

        protected void btn_risp_sx_Click(object sender, ImageClickEventArgs e)
        {
            //Riporta al dettaglio del documento grigio in risposta
            try
            {
                if (this.txt_RispostaDocGrigio != null && this.txt_RispostaDocGrigio.Text != "")
                {
                    string docNum = (DocumentManager.getDocumentoInLavorazione(this)).rispostaDocumento.docNumber;
                    string tipoProto = (DocumentManager.getDocumentoInLavorazione(this)).rispostaDocumento.tipoProto.ToString();

                    DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                    DocsPaWR.InfoDocumento infoDocumento = DocumentManager.GetCatenaDocumentoMittente(this, infoUtente.idGruppo, infoUtente.idPeople, docNum, tipoProto);

                    if (infoDocumento == null)
                    {
                        //Response.Write("<script>window.alert('Non si posseggono i diritti necessari alla visualizzazione del documento richiesto.')</script>");
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_dir", "window.alert('Non si posseggono i diritti necessari alla visualizzazione del documento richiesto.')", true);
                    }
                    else
                    {

                        DocumentManager.setRisultatoRicerca(this, infoDocumento);
                        //Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=profilo';</script>");
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Profilazione_agg", "top.principale.document.location = '../documento/gestionedoc.aspx?tab=profilo';", true);
                    }
                }
                else 
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_dir", "window.alert('Non ci sono documenti in risposta.')", true);
                }
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }



        #endregion


        #endregion

        #region PANNELLO FASCICOLAZIONE RAPIDA

        /// <summary>
        /// Abilita i vari pannelli a seconda delle funzioni elementari in possesso
        /// del ruolo corrente
        /// </summary>
        private void enableProfiloFields()
        {
            //Commentato per nuovo sviluppo estensione delle funzionalità di fascicolazione e trasmissione rapida
            //if ((schedaDocumento.systemId == null || schedaDocumento.systemId.Equals("")))
            //{
            if (UserManager.ruoloIsAutorized(this, "FASC_INS_DOC"))
            {

                if (this.schedaDocumento.documentoPrincipale == null)
                    this.pnl_fasc_rapida.Visible = true;
                else
                    this.pnl_fasc_rapida.Visible = false;
            }
            else
            {
                this.pnl_fasc_rapida.Visible = false;
            }

            if (UserManager.ruoloIsAutorized(this, "DO_TRA_NUOVA"))
            {
                if (schedaDocumento.documentoPrincipale == null)
                    this.pnl_trasm_rapida.Visible = true;
                else
                    this.pnl_trasm_rapida.Visible = false;
            }
            else
            {
                this.pnl_trasm_rapida.Visible = false;
            }
            //Se si possiedono solo i diritti di lettura non si 
            //può modificare la tipologia
            if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.accessRights) && schedaDocumento.accessRights.Equals("45"))
            {
                this.ddl_tipoAtto.Enabled = false;
            }
            //}
            //else
            //{
            //    this.pnl_fasc_rapida.Visible = false;
            //    this.pnl_trasm_rapida.Visible = false;
            //}
        }
        #endregion

        #region Trasmissione rapida

        private void ddl_tmpl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //MODELLI TRASMISSIONE NUOVI
            DocsPaWR.ModelloTrasmissione modello = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
            //int indexSel = ddl_tmpl.Items.IndexOf(ddl_tmpl.Items.FindByValue(separatore));
            int indexSel = ddl_tmpl.SelectedIndex;

            //se l'item selezionato è il separatore oppure è un template vecchio..esco dal metodo
            if (ddl_tmpl.SelectedItem.Text == separatore || indexSel <= 0)
            {
                Session.Remove("Modello");
                return;
            }
            modello = ModelliTrasmManager.getModelloByID(UserManager.getRegistroSelezionato(this).idAmministrazione, ddl_tmpl.SelectedValue, this);
            //Attenzione dava errore: this.txt_codModello.Text = modello.CODICE;
            if (modello != null && modello.SYSTEM_ID != 0)
            {
                this.txt_codModello.Text = modello.CODICE;
                Session.Add("Modello", modello);

                if (this.wws.ereditaVisibilita("null", modello.SYSTEM_ID.ToString()))
                {
                    this.abilitaModaleVis.Value = "true";
                }
                else
                {
                    this.abilitaModaleVis.Value = "false";
                }

            }
            else
                Session.Remove("Modello");
            //FINE MODELLI TRASMISSIONE		
        }

        //MODELLI TRASMISSIONE NUOVI
        private void execTrasmRapida()
        {
            logger.Info("BEGIN");
            int indexSel = ddl_tmpl.Items.IndexOf(ddl_tmpl.Items.FindByValue(separatore));

            if (this.ddl_tmpl.SelectedIndex > 0 && ddl_tmpl.SelectedIndex < indexSel)
            {
                if (Session["Modello"] != null)
                {
                    DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
                    Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();

                    //Parametri della trasmissione
                    trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
                    trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
                    trasmissione.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
                    trasmissione.utente = UserManager.getUtente(this);
                    trasmissione.ruolo = UserManager.getRuolo(this);

                    if (modello != null)
                        trasmissione.NO_NOTIFY = modello.NO_NOTIFY;
                    //Parametri delle trasmissioni singole
                    for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                    {
                        DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                        ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                        for (int j = 0; j < destinatari.Count; j++)
                        {
                            DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                            DocsPaWR.Corrispondente corr = new Corrispondente();
                            //old: ritoranva anche i corr diasbilitati DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                            if (mittDest.CHA_TIPO_MITT_DEST == "D")
                            {
                                corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                            }
                            else
                            {
                                corr = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, schedaDocumento, null, this);
                            }
                            if (corr != null)
                            {   //il corr è null se non esiste o se è stato disabiltato.    
                                DocsPaWR.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());
                                
                                //Andrea - try - catch
                                try
                                {
                                    trasmissione = TrasmManager.addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, mittDest.NASCONDI_VERSIONI_PRECEDENTI, this);
                                }
                                catch (ExceptionTrasmissioni e)
                                {
                                    //Aggiungo l'errore alla lista
                                    listaExceptionTrasmissioni.Add(e.Messaggio);

                                    //foreach (string s in listaExceptionTrasmissioni)
                                    //{
                                    //    //messError = messError + s + "\r\n";
                                    //    messError = messError + s + "|";
                                    //}

                                    //if (messError != "")
                                    //{
                                    //    Session.Add("MessError", messError);

                                    //    //Response.Write("<script language='javascript'>window.alert(" + messError + ");</script>");
                                    //}
                                }
                                //End Andrea
                            }
                        }
                    }
                    //Andrea
                    foreach (string s in listaExceptionTrasmissioni)
                    {
                        //messError = messError + s + "\r\n";
                        messError = messError + s + "\\n";
                    }

                    if (messError != "")
                    {
                        Session.Add("MessError", messError);
                    }
                    //End Andrea

                    DocsPaWR.Trasmissione t_rs = null;
                    if (trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Length > 0)
                    {
                        trasmissione = this.impostaNotificheUtentiDaModello(trasmissione);

                        if (estendiVisibilita.Value == "false")
                        {
                            TrasmissioneSingola[] appoTrasmSingole = new TrasmissioneSingola[trasmissione.trasmissioniSingole.Length];
                            for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                            {
                                TrasmissioneSingola trasmSing = new TrasmissioneSingola();
                                trasmSing = trasmissione.trasmissioniSingole[i];
                                trasmSing.ragione.eredita = "0";
                                appoTrasmSingole[i] = trasmSing;
                            }
                            trasmissione.trasmissioniSingole = appoTrasmSingole;
                        }

                        DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                        if (infoUtente.delegato != null)
                            trasmissione.delegato = ((DocsPAWA.DocsPaWR.InfoUtente)(infoUtente.delegato)).idPeople;

                        //Nuovo metodo saveExecuteTrasm

                        //Modifica per cessione diritti

                        if (this.cessioneDirittiAbilitato(trasmissione, modello))
                        {
                            bool aperturaPopUpSceltaNuovoProprietario = false;

                            // verifica se esistono ruoli tra i destinatari
                            this.verificaRuoliDestInTrasmissione(trasmissione);

                            switch (this.numeroRuoliDestInTrasmissione)
                            {
                                case 0:
                                    // non ci sono ruoli tra i destinatari! avvisa...
                                    this.inviaMsgNoRuoli();
                                    return;
                                    break;

                                case 1:
                                    // ce n'è 1, verifica se un solo utente del ruolo ha la notifica...
                                    this.utentiConNotifica(trasmissione);
                                    if (this.numeroUtentiConNotifica > 1)
                                        aperturaPopUpSceltaNuovoProprietario = true;
                                    else
                                    {
                                        // 1 solo utente con notifica, il sistema ha già memorizzato il suo id_people...
                                        trasmissione.cessione.idPeopleNewPropr = this.idPeopleNewOwner;
                                        trasmissione.cessione.idRuoloNewPropr = ((DocsPAWA.DocsPaWR.Ruolo)trasmissione.trasmissioniSingole[0].corrispondenteInterno).idGruppo;

                                        modello.CEDE_DIRITTI = "1";
                                        modello.ID_PEOPLE_NEW_OWNER = trasmissione.cessione.idPeopleNewPropr;
                                        modello.ID_GROUP_NEW_OWNER = trasmissione.cessione.idRuoloNewPropr;
                                    }
                                    break;

                                default:
                                    // ce ne sono + di 1, quindi ne fa scegliere uno...                                    
                                    aperturaPopUpSceltaNuovoProprietario = true;
                                    break;
                            }
                        }

                        if (this.verificaNotificheUtentiDaModello(trasmissione))
                            t_rs = TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
                        else
                            Response.Write("<script>window.alert('Trasmissione non effettuata in quanto il modello di trasmissione utilizzato prevede dei ruoli per i quali non sono presenti utenti con notifica. \\n Ricontrollare il modello.');</script>");
                        //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
                        //t_rs = TrasmManager.executeTrasm(this, trasmissione);
                    }
                    if (t_rs != null && t_rs.ErrorSendingEmails)
                        //Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_inoltro", "window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');", true);

                    //Salvataggio del system_id della trasm per il cambio di stato automatico
                    if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                    {
                        DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
                        bool trasmWF = false;
                        if (trasmissione != null && trasmissione.trasmissioniSingole != null)
                            for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                            {
                                DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                                if (trasmSing.ragione.tipo == "W")
                                    trasmWF = true;
                            }
                        if (stato != null && 
                            DiagrammiManager.IsRuoloAssociatoStatoDia(stato.ID_DIAGRAMMA.ToString(), UserManager.getRuolo().idGruppo, stato.SYSTEM_ID.ToString()) &&
                            trasmWF)
                            DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammi(trasmissione.systemId, schedaDocumento.docNumber, Convert.ToString(stato.SYSTEM_ID), this);
                    }
                    return;
                }
            }
            //FINE MODELLI TRASMISSIONE NUOVI
            DocsPaWR.Trasmissione trasmEff = null;

            try
            {
                if (this.ddl_tmpl.SelectedIndex > 0 && ddl_tmpl.SelectedIndex > indexSel)
                {
                    trasmEff = creaTrasmissione();

                    if (trasmEff == null)
                        //Response.Write("<script language='javascript'>alert('Si è verificato un errore nella creazione della trasmissione da template');</script>");
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Err_template", "alert('Si è verificato un errore nella creazione della trasmissione da template');", true);
                    else
                    {

                        if (estendiVisibilita.Value == "false")
                        {
                            TrasmissioneSingola[] appoTrasmSingole = new TrasmissioneSingola[trasmEff.trasmissioniSingole.Length];
                            for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
                            {
                                TrasmissioneSingola trasmSing = new TrasmissioneSingola();
                                trasmSing = trasmEff.trasmissioniSingole[i];
                                trasmSing.ragione.eredita = "0";
                                appoTrasmSingole[i] = trasmSing;
                            }
                            trasmEff.trasmissioniSingole = appoTrasmSingole;
                        }

                        DocsPaWR.Trasmissione t_rs = TrasmManager.executeTrasm(this, trasmEff);

                        if (t_rs != null && t_rs.ErrorSendingEmails)
                            //Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");
                            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Err_mail", "window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');", true);

                        //resetto il template della trasmissione
                        this.ddl_tmpl.SelectedIndex = 0;

                        //Salvataggio del system_id della trasm per il cambio di stato automatico
                        if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                        {
                            DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
                            bool trasmWF = false;
                            for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
                            {
                                DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmEff.trasmissioniSingole[i];
                                if (trasmSing.ragione.tipo == "W")
                                    trasmWF = true;
                            }
                            if (stato != null && 
                                DiagrammiManager.IsRuoloAssociatoStatoDia(stato.ID_DIAGRAMMA.ToString(), UserManager.getRuolo().idGruppo, stato.SYSTEM_ID.ToString()) &&
                                trasmWF)
                                DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammi(trasmEff.systemId, schedaDocumento.docNumber, Convert.ToString(stato.SYSTEM_ID), this);
                        }
                    }
                }
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(this, es);
            }
            logger.Info("END");
        }

        private bool verificaNotificheUtentiDaModello(DocsPaWR.Trasmissione objTrasm)
        {
            if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Length > 0)
            {
                for (int cts = 0; cts < objTrasm.trasmissioniSingole.Length; cts++)
                {
                    if (objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length > 0)
                    {
                        bool boolUtente = false;
                        for (int ctu = 0; ctu < objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length; ctu++)
                        {
                            if (this.daNotificareSuModello(objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].utente.idPeople, objTrasm.trasmissioniSingole[cts].corrispondenteInterno.systemId))
                            {
                                boolUtente = true;
                            }
                        }
                        if (!boolUtente)
                            return false;
                    }
                }
            }

            return true;
        }

        private DocsPAWA.DocsPaWR.Trasmissione creaTrasmissione()
        {
            logger.Info("BEGIN");
            //crea trasmissione da template
            DocsPaWR.TemplateTrasmissione[] listaTmp;
            DocsPaWR.Trasmissione trasmissione = null;
            try
            {
                DocsPaWR.TemplateTrasmissione template = null;
                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
                if (schedaDocumento == null)
                    schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
                DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaDocumento);
                listaTmp = (DocsPAWA.DocsPaWR.TemplateTrasmissione[])(Session["doc_profilo.tmplTrasm"]);

                //Adesso con i modelli di trasmissione nuovi il calcolo del template vecchio selezionato
                //é necessario farlo cosi'. In ogni caso funziona sia se ci sono sia se non ci sono modelli nuovi.
                int numberOldTemplate = ddl_tmpl.Items.Count - listaTmp.Length;
                if (listaTmp != null && listaTmp.Length > 0)
                    template = (DocsPAWA.DocsPaWR.TemplateTrasmissione)listaTmp[ddl_tmpl.SelectedIndex - numberOldTemplate];

                if (template != null)
                    trasmissione = TrasmManager.addTrasmDaTemplate(this, infoDoc, template, infoUtente);
            }
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
            logger.Info("END");
            return trasmissione;
        }

        #endregion

        #region fascicolazione rapida

        public void setFascicolazioneRapida()
        {
            DocsPaWR.Fascicolo fascRap = new Fascicolo();
            if (isRiproposto)
            {
                if (FascicoliManager.GetDoFascRapida(this) != null)
                {
                    fascRap = (DocsPaWR.Fascicolo)FascicoliManager.GetDoFascRapida(this);
                    FascicoliManager.setFascicoloSelezionatoFascRapida(fascRap);
                    FascicoliManager.setCodiceFascRapida(Page, fascRap.codice);
                    FascicoliManager.setDescrizioneFascRapida(Page, fascRap.descrizione);
                }
            }
            else
                fascRap = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
            string codiceFascRapida = FascicoliManager.getCodiceFascRapida(this);
            string descrFascRapida = FascicoliManager.getDescrizioneFascRapida(this);

            if (fascRap != null)
            {
                if (fascRap.folderSelezionato != null && codiceFascRapida != string.Empty && descrFascRapida != string.Empty)
                {
                    //this.txt_CodFascicolo.Text = fascRap.codice + "//" + fascRap.folderSelezionato.descrizione;
                    //this.txt_DescFascicolo.Text = fascRap.folderSelezionato.descrizione;
                    this.txt_CodFascicolo.Text = codiceFascRapida;
                    this.txt_DescFascicolo.Text = descrFascRapida;
                }
                else
                {
                    this.txt_CodFascicolo.Text = fascRap.codice;
                    this.txt_DescFascicolo.Text = fascRap.descrizione;
                }
            }
            else
            {

                //this.txt_CodFascicolo.Text = "";
                this.txt_DescFascicolo.Text = "";
            }

            //setto la tooltip del fascicolo
            this.txt_DescFascicolo.ToolTip = txt_DescFascicolo.Text;
        }

        /// <summary>
        /// Metodo per inserire la descrizione del fascicolo nel campo descrizione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_CodFascicolo_TextChanged(object sender, System.EventArgs e)
        {
            Session["validCodeFasc"] = "true";
            //inizialmente svuoto il campo e pulisco la sessione
            FascicoliManager.removeFascicoloSelezionatoFascRapida(this);

            // MAC 3891
            /* Annulla la precedente classificazione. 
             * In docProfilo.Page_Load, se esiste una classificazione, viene forzato l'evento txt_CodFascicolo_TextChanged
             * che va a copiare in sessione la classificazione esistente. Ciò sovrascrive la nuova classificazione.
             */
            DocumentManager.removeClassificazioneSelezionata(this);

            this.txt_DescFascicolo.Text = "";
            //
            if (this.txt_CodFascicolo.Text.Equals(""))
            {
                txt_DescFascicolo.Text = "";
                return;
            }
            //su DocProfilo devo cercare senza condizione sul registro.
            //Basta che il fascicolo sia visibile al ruolo loggato


            if (this.txt_CodFascicolo.Text.IndexOf("//") > -1)
            {

                string codice = string.Empty;
                string descrizione = string.Empty;

                DocsPaWR.Fascicolo SottoFascicolo = getFolder(null, ref codice, ref descrizione);
                if (SottoFascicolo != null)
                {
                    if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                    {
                        txt_DescFascicolo.Text = descrizione;
                        txt_CodFascicolo.Text = codice;
                        FascicoliManager.setCodiceFascRapida(this, codice);
                        FascicoliManager.setDescrizioneFascRapida(this, descrizione);
                        FascicoliManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);

                    }
                    else
                    {
                        //caso 0: al codice digitato non corrisponde alcun fascicolo

                        Session["validCodeFasc"] = "false";
                        Page.RegisterStartupScript("", "<script>alert('Attenzione, sottofascicolo non presente')</script>");
                        this.txt_DescFascicolo.Text = "";
                        this.txt_CodFascicolo.Text = "";

                    }
                }
                else
                {
                    Session["validCodeFasc"] = "false";
                    Page.RegisterStartupScript("", "<script>alert('Attenzione, sottofascicolo non presente')</script>");
                    this.txt_DescFascicolo.Text = "";
                    this.txt_CodFascicolo.Text = "";
                }
            }
            else
            {
                DocsPAWA.DocsPaWR.Registro reg = null;

                //Se il ruolo ha un solo registro non serve aprire la popup (Modifica 28/07/2010) bug inps 3471
                DocsPAWA.DocsPaWR.Registro[] register = UserManager.GetRegistriByRuolo(this, (UserManager.getRuolo(this)).systemId);
                if (register != null)
                {
                    if (register.Length > 0)
                    {
                        reg = register[0];
                    }
                }


                DocsPaWR.Fascicolo[] listaFasc = getFascicolo(reg);

                if (listaFasc != null)
                {
                    if (listaFasc.Length > 0)
                    {
                        //caso 1: al codice digitato corrisponde un solo fascicolo
                        if (listaFasc.Length == 1)
                        {
                            txt_DescFascicolo.Text = listaFasc[0].descrizione;
                            //metto il fascicolo in sessione
                            //FascicoliManager.setFascicoloSelezionato(this,fasc);
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                codClassifica = listaFasc[0].codice;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            FascicoliManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                            //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'N');");
                        }
                        else
                        {
                            //Hashtable hashRegistriNodi = getRegistriNodi(listaFasc);
                            //caso 2: al codice digitato corrispondono piu fascicoli
                            Session.Add("listaFascFascRapida", listaFasc);
                            codClassifica = this.txt_CodFascicolo.Text;
                            if (listaFasc[0].tipo.Equals("G"))
                            {
                                codClassifica = codClassifica;
                            }
                            else
                            {
                                //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                codClassifica = codiceGerarchia;
                            }
                            //Page.RegisterStartupScript("openListaFasc","<SCRIPT>ApriSceltaFascicolo();</SCRIPT>");
                            //Session.Add("hasRegistriNodi",hasRegistriNodi);

                            //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                            //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'Y');");
                            RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli2('" + codClassifica + "', 'Y')</script>");

                            return;
                        }
                    }
                    else
                    {
                        //caso 0: al codice digitato non corrisponde alcun fascicolo
                        if (listaFasc.Length == 0)
                        {
                            //Provo il caso in cui il fascicolo è chiuso
                            Fascicolo chiusoFasc = FascicoliManager.getFascicoloDaCodice(this.Page, this.txt_CodFascicolo.Text);
                            if (chiusoFasc != null && !string.IsNullOrEmpty(chiusoFasc.stato) && chiusoFasc.stato.Equals("C"))
                            {
                                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "fasc_chiuso", "alert('Attenzione, il fascicolo scelto è chiuso. Pertanto il documento non può essere inserito nel fascicolo selezionato');", true);
                            }
                            else
                            {
                                Page.RegisterStartupScript("", "<script>alert('Attenzione, codice fascicolo non presente')</script>");
                            }
                            Session["validCodeFasc"] = "false";
                           // Page.RegisterStartupScript("", "<script>alert('Attenzione, codice fascicolo non presente')</script>");
                            this.txt_DescFascicolo.Text = "";
                            this.txt_CodFascicolo.Text = "";
                        }
                        //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                        //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', '');");
                    }
                }
            }
        }



        /// <summary>
        /// Metodo per il recupero del fascicolo da codice
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo[] getFascicolo(DocsPAWA.DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                string codiceFascicolo = txt_CodFascicolo.Text;
                listaFasc = FascicoliManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Metodo per il recupero del sottofascicolo da codice fascicolo e descrizione sottofascicolo
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        /// 
        private DocsPAWA.DocsPaWR.Fascicolo getFolder(DocsPAWA.DocsPaWR.Registro registro, ref string codice, ref string descrizione)
        {
            DocsPaWR.Folder[] listaFolder = null;
            DocsPaWR.Fascicolo fasc = null;
            string separatore = "//";
            int posSep = this.txt_CodFascicolo.Text.IndexOf("//");
            if (this.txt_CodFascicolo.Text != string.Empty && posSep > -1)
            {

                string codiceFascicolo = txt_CodFascicolo.Text.Substring(0, posSep);
                string descrFolder = txt_CodFascicolo.Text.Substring(posSep + separatore.Length);

                listaFolder = FascicoliManager.getListaFolderDaCodiceFascicolo(this, codiceFascicolo, descrFolder, registro);
                if (listaFolder != null && listaFolder.Length > 0)
                {

                    //calcolo fascicolazionerapida
                    fasc = FascicoliManager.getFascicoloById(this, listaFolder[0].idFascicolo);

                    if (fasc != null)
                    {
                        //folder selezionato è l'ultimo
                        fasc.folderSelezionato = listaFolder[listaFolder.Length - 1];
                    }
                    codice = fasc.codice + separatore;
                    descrizione = fasc.descrizione + separatore;
                    for (int i = 0; i < listaFolder.Length; i++)
                    {
                        codice += listaFolder[i].descrizione + "/";
                        descrizione += listaFolder[i].descrizione + "/";
                    }
                    codice = codice.Substring(0, codice.Length - 1);
                    descrizione = descrizione.Substring(0, descrizione.Length - 1);

                }

            }
            if (fasc != null)
            {

                return fasc;

            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Metodo per il recupero del sottofascicolo da codice fascicolo e descrizione sottofascicolo
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo getFascicolo()
        {
            DocsPaWR.Fascicolo fascicoloSelezionato = null;
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                string codiceFascicolo = txt_CodFascicolo.Text;
                fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice(this, codiceFascicolo);
            }
            if (fascicoloSelezionato != null)
            {
                return fascicoloSelezionato;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Metodo per il recupero del fascicolo da codice
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo getFascicolo(string codice)
        {
            DocsPaWR.Fascicolo fascicoloSelezionato = null;
            if (!codice.Equals(""))
            {
                //string codiceFascicolo = txt_CodFascicolo.Text;
                fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice(this, codice);


                //OLD:   Fascicolo Fasc = FascicoliManager.getFascicoloDaCodice(this,);

                Fascicolo[] FascS = FascicoliManager.getListaFascicoliDaCodice(this, codice, null, "I");



                if (FascS != null && FascS.Length > 0 && FascS[0] != null)
                {
                    fascicoloSelezionato = (Fascicolo)FascS[0];

                }
            }

            if (fascicoloSelezionato != null)
                return fascicoloSelezionato;
            else
                return null;
        }

        #endregion

        /// <summary>
        /// Gestione deallocazione risorse utilizzata dalla dialog ricerca fascicoli
        /// </summary>
        private void ClearResourcesRicercaFascicoliFascRapida()
        {
            //Rimuove le variabili usate per la gestione della pagina ricercaFascicoli.aspx
            if (DocsPAWA.popup.ricercaFascicoli.RicercaFascicoliSessionMng.IsLoaded(this))
            {
                DocsPAWA.popup.ricercaFascicoli.RicercaFascicoliSessionMng.SetAsNotLoaded(this);
                DocsPAWA.popup.ricercaFascicoli.RicercaFascicoliSessionMng.ClearSessionData(this);
            }
            //Rimuove le variabili usate per la gestione della pagina scegliFascicoloFascRapida.aspx
            if (DocsPAWA.popup.scegliFascicoloFascRapida.ListaFascicoliSessionMng.IsLoaded(this))
            {
                DocsPAWA.popup.scegliFascicoloFascRapida.ListaFascicoliSessionMng.SetAsNotLoaded(this);
                DocsPAWA.popup.scegliFascicoloFascRapida.ListaFascicoliSessionMng.ClearSessionData(this);
            }
        }

        private void imgFasc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
            if (fasc != null)
            {
                if (this.txt_CodFascicolo.Text != "" && this.txt_DescFascicolo.Text != "")
                {
                    if (fasc.tipo.Equals("G"))
                    {
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + fasc.codice + "')</script>");
                    }
                    else
                    {
                        ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fasc.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "')</script>");
                    }
                }
            }
            else
            //{
            //    if (this.txt_CodFascicolo.Text != "")
            //    {
            //        DocsPaWR.Fascicolo[] listaFasc = getFascicolo(null);

            //        if (listaFasc != null)
            //        {
            //            Session.Add("listaFascFascRapida", listaFasc);
            //            switch (listaFasc.Length)//il codice corrisponde a un solo fascicolo
            //            {
            //                case 0:
            //                    {
            //                        RegisterStartupScript("AlertNoFasc", "<script>alert('Attenzione, codice fascicolo non presente');</script>");
            //                        this.txt_DescFascicolo.Text = "";
            //                        this.txt_CodFascicolo.Text = "";
            //                    }
            //                    break;
            //                case 1:
            //                    {
            //                        if (listaFasc[0].tipo.Equals("G"))
            //                        {
            //                            RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + listaFasc[0].codice + "', 'N')</script>");
            //                        }
            //                        else
            //                        {
            //                            ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
            //                            DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
            //                            string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
            //                            RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "', 'N')</script>");
            //                        }
            //                    }
            //                    break;
            //                default:
            //                    {
            //                        if (listaFasc[0].tipo.Equals("G"))
            //                        {
            //                            RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + listaFasc[0].codice + "', 'Y')</script>");
            //                        }
            //                        else
            //                        {
            //                            ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
            //                            DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
            //                            string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
            //                            RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "', 'Y')</script>");
            //                        }
            //                    }
            //                    break;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (!(Session["validCodeFasc"] != null && Session["validCodeFasc"].ToString() == "false"))
            //            RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + txt_CodFascicolo.Text + "', 'N')</script>");
            //    }
            //    //RegisterStartupScript("openModale","<script>ApriRicercaFascicoli('"+txt_CodFascicolo.Text+"', 'N')</script>");
            //}
            {
                if (this.txt_CodFascicolo.Text != "")
                {
                    DocsPAWA.DocsPaWR.Registro reg = null;
                    DocsPaWR.Fascicolo[] listaFasc = getFascicolo(reg);

                    if (listaFasc != null)
                    {
                        Session.Add("listaFascFascRapida", listaFasc);
                        switch (listaFasc.Length)//il codice corrisponde a un solo fascicolo
                        {
                            case 0:
                                {
                                    RegisterStartupScript("AlertNoFasc", "<script>alert('Attenzione, codice fascicolo non presente');</script>");
                                    this.txt_DescFascicolo.Text = "";
                                    this.txt_CodFascicolo.Text = "";
                                }
                                break;
                            case 1:
                                {
                                    if (listaFasc[0].tipo.Equals("G"))
                                    {
                                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + listaFasc[0].codice + "', 'N')</script>");
                                    }
                                    else
                                    {
                                        ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "', 'N')</script>");
                                    }
                                }
                                break;
                            default:
                                {
                                    if (listaFasc[0].tipo.Equals("G"))
                                    {
                                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + listaFasc[0].codice + "', 'Y')</script>");
                                    }
                                    else
                                    {
                                        ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "', 'Y')</script>");
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    if (!(Session["validCodeFasc"] != null && Session["validCodeFasc"].ToString() == "false"))
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + txt_CodFascicolo.Text + "', 'N')</script>");
                }
            }
        }

        //controllo per la creazione di un nuovo fascicolo procedimentale e la sua successiva selezione 
        //auomatica all'interno della form di fascicolazione rapida
        private void imgNewFasc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

            //schedaDocumento = DocumentManager.getDocumentoInLavorazione();
            //Session["grigio"] = schedaDocumento;
            DocsPaWR.Registro reg = UserManager.getRegistroSelezionato(this);
            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(UserManager.getUtente(this).idAmministrazione));
            if (reg != null && reg.Sospeso)
            {
                RegisterClientScriptBlock("alertRegistroSospeso", "<SCRIPT language='javascript'>alert('Il registro selezionato è sospeso!');</SCRIPT>");
                return;
            }

            DocsPAWA.DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
            if (fasc != null && fasc.isFascConsentita != null && fasc.isFascConsentita == "0")
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_fasc", "alert('Non è possibile creare un fascicolo nel fascicolo selezionato.');", true);
                return;
            }
            string profilazione = System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"];
            if (this.txt_CodFascicolo.Text.Trim().Equals(""))
            {
                RegisterClientScriptBlock("alertCodiceInesistente", "<SCRIPT language='javascript'>alert('Inserire un nodo di titolario!');</SCRIPT>");
            }


            if (!this.txt_CodFascicolo.Text.Trim().Equals(""))
            {

                //ClientScript.RegisterStartupScript(this.GetType(), "apreGestNew", "ApriFinestraNewFascNewTit('" + Server.UrlEncode(this.txt_codClass.Text) + "','ricercaFascicoli','fascNewFascicolo.aspx','" + profilazione + "','"+ddl_titolari.SelectedValue+"');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "apreGestNew", "ApriFinestraNewFascNewTit('" + Server.UrlEncode(this.txt_CodFascicolo.Text) + "','docProfilo','fascNewFascicolo.aspx','" + profilazione + "','" + getIdTitolario(this.txt_CodFascicolo.Text, listaTitolari) + "');", true);


            }
        }

        protected void imgDescOgg_Click(object sender, ImageClickEventArgs e)
        {
            //Session.Add("SessionDescCampo", this.ctrl_oggetto.oggetto_text);
           // RegisterStartupScript("openDescOggetto", "<SCRIPT>ApriDescrizioneCampo('O');</SCRIPT>");
            Session.Add("SessionDescCampo", this.ctrl_oggetto.oggetto_text);
            bool valore_o = false;
            if (btn_salva.Enabled)
            {
                valore_o = true;
            }
            Session.Add("Abilitato_modifica", valore_o);
            this.ClientScript.RegisterStartupScript(this.GetType(), "openDescOggetto", "ApriDescrizioneCampo('O');", true);
            this.ClientScript.RegisterStartupScript(this.GetType(), "apri_nota", "top.principale.iFrame_sx.document.location='tabGestioneDoc.aspx?tab=profilo';", true);
        }

        #region Gestione CallContext

        /// <summary>
        /// Impostazione dei dati identificativi del documento corrente nel contesto corrente.
        /// In base agli identificativi immessi, il documento verrà
        /// ripristinato in una fase di back.
        /// </summary>
        private void SetDocumentOnContext()
        {
            // Se il documento è presente su database
            if (!string.IsNullOrEmpty(this.schedaDocumento.systemId))
            {
                SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
                if (currentContext.ContextName == SiteNavigation.NavigationKeys.DOCUMENTO ||
                    currentContext.ContextName == SiteNavigation.NavigationKeys.ALLEGATO)
                {
                    currentContext.ContextState["idProfile"] = this.schedaDocumento.systemId;
                    currentContext.ContextState["docNumber"] = this.schedaDocumento.docNumber;
                }
            }
        }

        #endregion

        #region Gestione stampa etichetta

        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_classifica;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_fascicolo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_amministrazioneEtichetta;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_descrizioneAmministrazione;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_num_doc;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_dataCreazione;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_codiceUoCreatore;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_numero_allegati;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_UrlIniFileDispositivo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_dispositivo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_modello_dispositivo;
        private string _urlStampaEtichettaPage = "../blank_page.htm";

        /// <summary>
        /// Stampa etichetta documento grigio
        /// </summary>
        protected void PrintSignature()
        {
            try
            {
                // caricamento campi etichetta
                this.FillCampiEtichetta();
                this._urlStampaEtichettaPage = "stampaSegnaturaGrigio.htm";
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string UrlStampaEtichettaPage
        {
            get
            {
                return this._urlStampaEtichettaPage;
            }
        }

        /// <summary>
        /// Handler evento stampa etichetta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_stampaSegn_Click(object sender, ImageClickEventArgs e)
        {
            this.PrintSignature();
        }


        protected void Help_Click(object sender, ImageClickEventArgs e)
        {
            string script = "<script>window.open('../Help/manualepitre_tmphhp/manualepitre.chm','help','width=410,height=300,scrollbars=NO');</script>";
            Response.Write(script);
        }

        /// <summary>
        /// Caricamento dei dati utilizzabili per la stampa dell'etichetta
        /// in un'insieme di campi testo nascosti
        /// </summary>
        /// <param name="schedaDocumento"></param>
        private void FillCampiEtichetta()
        {
            #region parametro Dispositivo Di Stampa
            //versione precedente
            //if (ConfigSettings.getKey(ConfigSettings.KeysENUM.DISPOSITIVO_STAMPA) != null)
            //    this.hd_dispositivo.Value = ConfigSettings.getKey(ConfigSettings.KeysENUM.DISPOSITIVO_STAMPA);
            //else
            //    this.hd_dispositivo.Value = "Penna";
            //this.hd_modello_dispositivo.Value = ConfigSettings.getKey(ConfigSettings.KeysENUM.MODELLO_DISPOSITIVO_STAMPA);
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            var dispositivoStampaUtente = ws.AmmGetDispositivoStampaUtente(UserManager.getInfoUtente().idPeople);
            if (dispositivoStampaUtente != null)
            {
                this.hd_dispositivo.Value = "Etichette";
                this.hd_modello_dispositivo.Value = dispositivoStampaUtente.ToString();
            }
            else
                this.hd_dispositivo.Value = "Penna";

            // 1. Chiamata al servizio web (UserManager.getInfoUtente();)
            //DocsPaWR.DispositivoStampaEtichetta dispositivoCorrente = null;
            //dispositivoCorrente.Code 
            this.hd_modello_dispositivo.Value = ConfigSettings.getKey(ConfigSettings.KeysENUM.MODELLO_DISPOSITIVO_STAMPA);
            #endregion parametro Dispositivo Di Stampa

            #region parametro Descrizione Amministrazione

            string descAmm = GetDescrizioneAmministrazione(UserManager.getInfoUtente().idAmministrazione);

            #endregion parametro Descrizione Amministrazione

            #region parametro Classifica Primaria

            string classificaPrimaria = String.Empty;

            string classificazioneInEtichetta = System.Configuration.ConfigurationManager.AppSettings["StampaClassificazioneInEtichetta"];
            if (classificazioneInEtichetta != null)
            {
                switch (classificazioneInEtichetta)
                {
                    case "1": // stampa il codice classifica In Etichetta
                        classificaPrimaria = GetClassificaPrimaria();
                        break;
                    default:
                        //massimo digregorio, non necessario se l'assegnazione avviene in dichiarazione. old: classificaPrimaria = String.Empty;
                        break;
                }
            }

            this.hd_classifica.Value = classificaPrimaria;

            #endregion parametro Classifica Primaria

            #region parametro Fascicolo primario

            string fascicoloInEtichetta = System.Configuration.ConfigurationManager.AppSettings["StampaFascicoloInEtichetta"];
            if (fascicoloInEtichetta != null)
            {
                switch (fascicoloInEtichetta)
                {
                    case "1": // stampa il codice fascicolo In Etichetta
                        this.hd_fascicolo.Value = this.GetCodiceFascicolo();
                        break;
                    default:
                        this.hd_fascicolo.Value = String.Empty;
                        break;
                }
            }

            #endregion parametro Fascicolo primario

            #region patch per cuneo

            string descAmministrInEtichetta = System.Configuration.ConfigurationManager.AppSettings["StampaDescrizioneAmministrazioneInEtichetta"];
            if (descAmministrInEtichetta != null)
            {
                switch (descAmministrInEtichetta)
                {
                    case "1": // Stampa Descrizione Amministrazione In Etichetta
                        this.hd_amministrazioneEtichetta.Value = descAmm;
                        break;
                    default:
                        this.hd_amministrazioneEtichetta.Value = string.Empty;
                        break;
                }
            }

            //aggiuto tag Hidden "hd_desAmministrazione" per ActiveX di stampa
            /* se parametro esiste ed a 0, a hd_desAmministrazione viene assegnata la classifica
                     * se parametro non esiste o esiste <> 0, a hd_desAmministrazione viene assegnata la descrizione dell'amministrazione
                     */
            bool BarCodeConAmministrazione = true;
            DocsPAWA.DocsPaWR.Configurazione visualizzaClassificaSopraBarCode = UserManager.getParametroConfigurazione(this.Page);

            if (visualizzaClassificaSopraBarCode != null)
            {
                if (visualizzaClassificaSopraBarCode.valore.Equals("0")) BarCodeConAmministrazione = false;
            }

            if (BarCodeConAmministrazione)
            {
                this.hd_descrizioneAmministrazione.Value = descAmm;
            }
            else
            {
                this.hd_descrizioneAmministrazione.Value = classificaPrimaria;
            }

            #endregion patch per cuneo

            #region parametro URL File di configurazione Dispositivo di Stampa

            this.hd_UrlIniFileDispositivo.Value = ConfigSettings.getKey(ConfigSettings.KeysENUM.URL_INIFILE_DISPOSITIVO_STAMPA);

            #endregion parametro URL File di configurazione Dispositivo di Stampa

            #region parametri scheda Documento

            this.hd_num_doc.Value = this.schedaDocumento.docNumber;
            this.hd_numero_allegati.Value = this.schedaDocumento.allegati.Length.ToString();

            DateTime dataCreazione;
            if (DateTime.TryParse(this.schedaDocumento.dataCreazione, out dataCreazione))
                this.hd_dataCreazione.Value = string.Format(@"{0:dd\/MM\/yyyy}", dataCreazione);

            this.hd_codiceUoCreatore.Value = schedaDocumento.creatoreDocumento.uo_codiceCorrGlobali;

            #endregion
        }

        /// <summary>
        /// Reperimento del codice fascicolo a cui il documento è associato
        /// </summary>
        /// <returns>codice Fascicolo</returns>
        private string GetCodiceFascicolo()
        {
            return DocumentManager.getFascicoloDoc(this, DocsPAWA.DocumentManager.getInfoDocumento(this.schedaDocumento));
        }

        /// <summary>
        /// Reperimento del codice classifica a cui la scheda documento è associata.
        /// </summary>
        /// <returns>codice classifica</returns>
        private string GetClassificaPrimaria()
        {
            return DocumentManager.GetClassificaDoc(this, this.schedaDocumento.systemId);
        }

        /// <summary>
        /// Reperimento della descrizione dell' Amministrazione attuale.
        /// </summary>
        /// <param name="IdAmministrazione"></param>
        /// <returns>restituisce la descrizione dell'amministrazione passata con il parametro di input IdAmministrazione</returns>
        private string GetDescrizioneAmministrazione(string IdAmministrazione)
        {
            string descAmm = string.Empty;
            string returnMsg = string.Empty;
            DocsPAWA.DocsPaWR.Amministrazione[] amministrazioni = UserManager.getListaAmministrazioni(this, out returnMsg);

            if (amministrazioni.Length == 1)
            {
                descAmm = amministrazioni[0].descrizione;
            }
            else
            {
                bool found = false;
                int i = 0;

                while ((!found) && (i < amministrazioni.Length))
                {
                    if (amministrazioni[i].systemId == IdAmministrazione)
                    {
                        found = true;
                        descAmm = amministrazioni[i].descrizione;
                    }
                    i++;
                }
            }
            return descAmm;
        }

        #endregion

        #region Gestione controllo acl documento

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNote_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Inizializzazione controllo verifica acl
        /// </summary>
        protected virtual void InitializeControlAclDocumento()
        {
            AclDocumento ctl = this.GetControlAclDocumento();
            ctl.IdDocumento = DocumentManager.getDocumentoSelezionato().systemId;
            ctl.OnAclRevocata += new EventHandler(this.OnAclDocumentoRevocata);
        }

        /// <summary>
        /// Reperimento controllo acldocumento
        /// </summary>
        /// <returns></returns>
        protected AclDocumento GetControlAclDocumento()
        {
            return (AclDocumento)this.FindControl("aclDocumento");
        }

        /// <summary>
        /// Listener evento OnAclDocumentoRevocata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAclDocumentoRevocata(object sender, EventArgs e)
        {
            // Redirect alla homepage di docspa
            SiteNavigation.CallContextStack.Clear();
            SiteNavigation.NavigationContext.RefreshNavigation();
            string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
            Response.Write(script);
        }

        #endregion

        protected void btn_Correttore_Click(object sender, ImageClickEventArgs e)
        {
            this.ctrl_oggetto.SpellCheck();
        }

        //Ricerca dello user control Oggetto lato server
        protected Oggetto GetControlOggetto()
        {
            return (Oggetto)this.FindControl("ctrl_oggetto");
        }

        #region Gestione navigazione verso documento principale

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGoToDocumentoPrincipale_Click(object sender, ImageClickEventArgs e)
        {
            this.GoToDocumentoPrincipale();
        }

        /// <summary>
        /// Caricamento dati relativi al documento principale
        /// </summary>
        private void InitializeDataDocumentoPrincipale()
        {
            // Gestione documento principale
            this.trDocumentoPrincipale.Visible = (schedaDocumento.documentoPrincipale != null);

            if (this.trDocumentoPrincipale.Visible)
            {
                if (schedaDocumento.documentoPrincipale.tipoProto == "G")
                    this.txtDocumentoPrincipale.Text = string.Format("Documento num. {0}", schedaDocumento.documentoPrincipale.docNumber);
                else
                {
                    if (!string.IsNullOrEmpty(schedaDocumento.documentoPrincipale.numProt))
                        this.txtDocumentoPrincipale.Text = schedaDocumento.documentoPrincipale.segnatura;
                    else
                        this.txtDocumentoPrincipale.Text = string.Format("Documento predisposto num. {0}", schedaDocumento.documentoPrincipale.docNumber);
                    this.btn_rimuovi.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Navigazione verso il documento principale
        /// </summary>
        private void GoToDocumentoPrincipale()
        {
            if (this.schedaDocumento.documentoPrincipale != null)
            {
                DocsPaWR.SchedaDocumento schedaDocumentoPrincipale = null;

                if (this.schedaDocumento.documentoPrincipale.inCestino == "1")
                    schedaDocumentoPrincipale = DocumentManager.getDettaglioDocumento(this, this.schedaDocumento.documentoPrincipale.idProfile, this.schedaDocumento.documentoPrincipale.docNumber);
                else
                    schedaDocumentoPrincipale = DocumentManager.getDettaglioDocumentoDaCestino(this, this.schedaDocumento.documentoPrincipale.idProfile, this.schedaDocumento.documentoPrincipale.docNumber);

                if (schedaDocumentoPrincipale != null)
                {
                    DocumentManager.setDocumentoSelezionato(this, schedaDocumentoPrincipale);
                    DocumentManager.setDocumentoInLavorazione(schedaDocumentoPrincipale);

                    InfoDocumento infoDocPrincipale = DocumentManager.getInfoDocumento(schedaDocumentoPrincipale);

                    DocumentManager.setRisultatoRicerca(this, infoDocPrincipale);
                    // Navigazione verso il documento principale dell'allegato,
                    // indicando di forzare la crezione di un nuovo contesto
                    if (schedaDocumento.documentoPrincipale.tipoProto == "G")
                    {
                        //   this.Response.Write(string.Format("<script>window.open('gestioneDoc.aspx?tab=profilo&forceNewContext=true&daCestino={0}', 'principale');</script>", this.schedaDocumento.documentoPrincipale.inCestino));
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Err_risp", "top.principale.document.location = '../documento/gestionedoc.aspx?tab=profilo';", true);
                    }

                    else
                    { //sia protocollato che predisposto deve andare su tab protocollo
                        //   this.Response.Write(string.Format("<script>window.open('gestioneDoc.aspx?tab=protocollo&forceNewContext=true&daCestino={0}', 'principale');</script>", this.schedaDocumento.documentoPrincipale.inCestino));
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Err_risp", "top.principale.document.location = '../documento/gestionedoc.aspx?tab=protocollo';", true);


                    }

                }
            }
        }

        #endregion

        #region Gestione notifica utenti

        private DocsPaWR.Trasmissione impostaNotificheUtentiDaModello(DocsPaWR.Trasmissione objTrasm)
        {
            if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Length > 0)
            {
                for (int cts = 0; cts < objTrasm.trasmissioniSingole.Length; cts++)
                {
                    if (objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length > 0)
                    {
                        for (int ctu = 0; ctu < objTrasm.trasmissioniSingole[cts].trasmissioneUtente.Length; ctu++)
                        {
                            objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].daNotificare = this.daNotificareSuModello(objTrasm.trasmissioniSingole[cts].trasmissioneUtente[ctu].utente.idPeople, objTrasm.trasmissioniSingole[cts].corrispondenteInterno.systemId);
                        }
                    }
                }
            }

            return objTrasm;
        }

        private bool daNotificareSuModello(string currentIDPeople, string currentIDCorrGlobRuolo)
        {
            bool retValue = true;

            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                    if (mittDest.ID_CORR_GLOBALI.Equals(Convert.ToInt32(currentIDCorrGlobRuolo)))
                    {
                        if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length > 0)
                        {
                            for (int cut = 0; cut < mittDest.UTENTI_NOTIFICA.Length; cut++)
                            {
                                if (mittDest.UTENTI_NOTIFICA[cut].ID_PEOPLE.Equals(currentIDPeople))
                                {
                                    if (mittDest.UTENTI_NOTIFICA[cut].FLAG_NOTIFICA.Equals("1"))
                                        retValue = true;
                                    else
                                        retValue = false;

                                    return retValue;
                                }
                            }
                        }
                    }
                }
            }
            return retValue;
        }

        #endregion

        protected void btn_risp_dx_Click(object sender, ImageClickEventArgs e)
        {
            DocsPAWA.DocsPaWR.InfoDocumento infoDocRisposta = GetDocInRisposta(schedaDocumento.systemId);
            if (infoDocRisposta != null)
            {
                //this.btn_risp_dx.AlternateText = "visualizza documento in risposta al " + this.schedaDocumento.docNumber;
                DocumentManager.setRisultatoRicerca(this, infoDocRisposta);
                //this.btn_risp_dx.Attributes.Add("onclick", "VaiRispostaDocGrigio (); return false");
                //Response.Write("<script language='javascript'> top.principale.document.location = '../documento/gestionedoc.aspx?tab=profilo';</script>");
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Err_risp", "top.principale.document.location = '../documento/gestionedoc.aspx?tab=profilo';", true);
            }
        }


        protected void btn_titolario_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ////E' necessario che sia selezionato un titolario e non la voce "tutti i titolari"
            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(UserManager.getUtente(this).idAmministrazione));
            string idTitolario = this.getIdTitolario("", listaTitolari);
            FascicoliManager.removeFascicoloSelezionatoFascRapida();
            if (!string.IsNullOrEmpty(idTitolario))
            {
                if (!this.IsStartupScriptRegistered("apriModalDialog"))
                {
                    //string scriptString = "<SCRIPT>ApriTitolario('codClass=" + txt_codClass.Text + "&idTit=" + ddl_titolari.SelectedValue + "','gestClass')</SCRIPT>";
                    string scriptString = "<SCRIPT>ApriTitolario('codClass=" + string.Empty + "&idTit=" + idTitolario + "','gestDoc')</SCRIPT>";
                    this.RegisterStartupScript("apriModalDialog", scriptString);
                }
            }
            else
            {
                string script = "<SCRIPT>alert('Non è presente nessun titolario attivo');</SCRIPT>";
                this.RegisterStartupScript("alert", script);
            }
        }

        private string getIdTitolario(string codClassificazione, ArrayList titolari)
        {
            string result = string.Empty;
            foreach (DocsPaWR.OrgTitolario titolario in titolari)
            {
                if (titolario.Stato == DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo)
                {
                    result = titolario.ID;
                }
            }
            return result;
        }

        #region Area Conservazione

        private void btn_storiaCons_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string idProfile = schedaDocumento.systemId;
            RegisterStartupScript("openStoriaConsDoc", "<SCRIPT>ApriStoriaConservazione('" + idProfile + "');</SCRIPT>");
        }

        #endregion

        /// <summary>
        /// Verifica se è una trasmissione con cessione dei diritti sull'oggetto.
        /// Ritorna True se:
        /// 
        ///     1 - la trasmissione è generata da un modello di trasmissione con cessione diritti (anche se l'utente non è abilitato all'editing ACL)
        ///     2 - la trasmissione prevede già cessione dei diritti poichè l'utente (abilitato all'editing ACL) ha selezionato la checkbox "Cedi diritti"
        ///     
        /// </summary>
        /// <returns>True, False</returns>
        private bool cessioneDirittiAbilitato(DocsPAWA.DocsPaWR.Trasmissione trasm, DocsPaWR.ModelloTrasmissione modello)
        {
            bool retValue = false;
            if (modello != null)
                trasm.NO_NOTIFY = modello.NO_NOTIFY;
            if (modello != null && modello.CEDE_DIRITTI.Equals("1"))
            {

                if (trasm.cessione == null)
                {
                    DocsPaWR.CessioneDocumento cessione = new DocsPAWA.DocsPaWR.CessioneDocumento();
                    cessione.docCeduto = true;
                    //*******************************************************************************************
                    // MODIFICA IACOZZILLI GIORDANO 18/07/2012
                    // Modifica inerente la cessione dei diritti di un doc da parte di un utente non proprietario ma 
                    // nel ruolo del proprietario, in quel caso non posso valorizzare l'IDPEOPLE  con il corrente perchè
                    // il proprietario può essere un altro utente del mio ruolo, quindi andrei a generare un errore nella security,
                    // devo quindi controllare che nell'idpeople venga inserito l'id corretto del proprietario.
                    string valoreChiaveCediDiritti = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_CEDI_DIRITTI_IN_RUOLO");
                    if (!string.IsNullOrEmpty(valoreChiaveCediDiritti) && valoreChiaveCediDiritti.Equals("1"))
                    {
                        //Devo istanziare una classe utente.
                        string idProprietario = string.Empty;
                        if (!string.IsNullOrEmpty(schedaDocumento.systemId) && !string.IsNullOrEmpty(schedaDocumento.docNumber))
                            idProprietario = GetAnagUtenteProprietario(schedaDocumento.docNumber);
                        else
                            idProprietario = UserManager.getInfoUtente(this).idPeople;
                        //idProprietario = GetAnagUtenteProprietario();
                        Utente _utproprietario = UserManager.GetUtenteByIdPeople(idProprietario);

                        cessione.idPeople = idProprietario;
                        cessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;
                        cessione.userId = _utproprietario.cognome + " " + _utproprietario.nome;


                        if (cessione.idPeople == null || cessione.idPeople == "")
                            cessione.idPeople = idProprietario;

                        if (cessione.idRuolo == null || cessione.idRuolo == "")
                            cessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;

                        if (cessione.userId == null || cessione.userId == "")
                            cessione.userId = _utproprietario.cognome + " " + _utproprietario.nome;
                    }
                    else
                    {
                        //OLD CODE:
                        cessione.idPeople = UserManager.getInfoUtente(this).idPeople;
                        cessione.idRuolo = UserManager.getInfoUtente(this).idGruppo;
                        cessione.userId = UserManager.getInfoUtente(this).userId;
                    }
                    //*******************************************************************************************
                    // FINE MODIFICA
                    //********************************************************************************************
                    cessione.idPeopleNewPropr = modello.ID_PEOPLE_NEW_OWNER;
                    cessione.idRuoloNewPropr = modello.ID_GROUP_NEW_OWNER;
                    trasm.cessione = cessione;
                }
            }
            else
                retValue = (trasm.cessione != null && trasm.cessione.docCeduto);

            return retValue;
        }


        /// <summary>
        /// Iacozzilli: Faccio la Get dell'idProprietario del doc!
        /// </summary>
        /// <returns></returns>
        private string GetAnagUtenteProprietario(string docnumber)
        {
            DocumentoDiritto[] listaVisibilita = null;
            //DocsPaWR.SchedaDocumento sd = DocumentManager.getDocumentoSelezionato(this);
            string idProprietario = string.Empty;
            //listaVisibilita = DocumentManager.getListaVisibilitaSemplificata(this, sd.docNumber, true);
            listaVisibilita = DocumentManager.getListaVisibilitaSemplificata(this, docnumber, true);
            if (listaVisibilita != null && listaVisibilita.Length > 0)
            {
                for (int i = 0; i < listaVisibilita.Length; i++)
                {
                    if (listaVisibilita[i].accessRights == 0)
                    {
                        return idProprietario = listaVisibilita[i].personorgroup;
                    }
                }
            }

            return "";
        }



        /// <summary>
        /// Verifica se esistono RUOLI tra i destinatari della trasmissione 
        /// ed imposta quanti sono
        /// </summary>
        private void verificaRuoliDestInTrasmissione(DocsPAWA.DocsPaWR.Trasmissione trasm)
        {
            foreach (DocsPAWA.DocsPaWR.TrasmissioneSingola trasmy in trasm.trasmissioniSingole)
            {
                if (trasmy.tipoDest.ToString().ToUpper().Equals("RUOLO"))
                    this.numeroRuoliDestInTrasmissione++;
            }
        }

        /// <summary>
        /// Invia un messaggio a video che avvisa l'utente che tra i destinatari della trasmissioni
        /// non ci sono ruoli
        /// </summary>
        private void inviaMsgNoRuoli()
        {
            utils.CessioneDiritti obj = new DocsPAWA.utils.CessioneDiritti();
            if (!ClientScript.IsStartupScriptRegistered("OpenMsg"))
                ClientScript.RegisterStartupScript(this.GetType(), "OpenMsg", obj.InviaMsgNoRuoli());
        }

        /// <summary>
        /// Verifica se ci sono utenti con notifica, quanti sono e, nel caso di 1, ne memorizza l'ID
        /// </summary>
        private void utentiConNotifica(DocsPAWA.DocsPaWR.Trasmissione trasm)
        {
            foreach (DocsPAWA.DocsPaWR.TrasmissioneSingola trasmy in trasm.trasmissioniSingole)
                foreach (DocsPAWA.DocsPaWR.TrasmissioneUtente trasmUt in trasmy.trasmissioneUtente)
                    if (trasmUt.daNotificare)
                    {
                        this.numeroUtentiConNotifica++;
                        this.idPeopleNewOwner = trasmUt.utente.idPeople; // memorizza l'id people... da utilizzare nel caso di un solo utente con notifica                        
                    }
        }

        //[System.Web.Services.WebMethod]
        //[System.Web.Script.Services.ScriptMethod]
        //public static string[] GetListNote(string prefixText, int count)
        //{
        //    string[] toReturn;

        //    toReturn = new string[] { prefixText, count.ToString(), "Ciao" };

        //    return toReturn;

        //}

        protected void btn_risp_grigio_Click(object sender, ImageClickEventArgs e)
        {
            DocumentManager.removeMemoriaFiltriRicDoc(this);
            DocumentManager.RemoveMemoriaVisualizzaBack(this);

            SchedaDocumento schedaNewDoc = this.wws.NewSchedaDocumento(UserManager.getInfoUtente());
            if (schedaDocumento.oggetto != null)
            {
                schedaNewDoc.oggetto = schedaDocumento.oggetto;
            }
            schedaNewDoc.idPeople = schedaDocumento.idPeople;
            schedaNewDoc.userId = schedaDocumento.userId;
            schedaNewDoc.typeId = schedaDocumento.typeId;
            schedaNewDoc.appId = schedaDocumento.appId;
            schedaNewDoc.privato = "0";

            FileManager.removeSelectedFile(this);

            // schedaNewDoc.registro = null;
            schedaNewDoc.tipoProto = "A";
            // schedaNewDoc.protocollo = null;

            schedaNewDoc.predisponiProtocollazione = true;
            //Viene popolato l'oggetto risposta al protocollo:
            DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(schedaDocumento);
            schedaNewDoc.rispostaDocumento = infoDoc;

            DocumentManager.setDocumentoSelezionato(this, schedaNewDoc);
            DocumentManager.setDocumentoInLavorazione(this, schedaNewDoc);
            ClientScript.RegisterStartupScript(this.GetType(), "Gestione_risposta_grigio", "top.principale.document.location = '../documento/gestionedoc.aspx?tab=protocollo';", true);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void InitializeControlConsolidation()
        {
            this.documentConsolidationCtrl.Consolidated += new DocumentConsolidatedDelegate(documentConsolidationCtrl_Consolidated);
        }

        /// <summary>
        /// Listener evento documento consolidato
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void documentConsolidationCtrl_Consolidated(object sender, DocumentConsolidatedEventArgs e)
        {
            this.ddl_tmpl.Items.Clear();
            this.caricaModelliTrasm();
        }
        public int GetCountDocInRisposta(string sys)
        {
            int numDocInRisposta = 0;
            DocsPAWA.DocsPaWR.InfoDocumento infoDocRisp = null;
            //ABBATANGELI GIANLUIGI
            if (!string.IsNullOrEmpty(sys))
            {
                if (!schedaDocumento.tipoProto.Equals("C") && GetFiltroDocInRisposta(schedaDocumento))
                {
                    int numTotPage;
                    int nRec;
                    DocumentManager.setFiltroRicDoc(this, qV);
                    DocsPaWR.InfoUtente infoUt = new DocsPAWA.DocsPaWR.InfoUtente();
                    infoUt = UserManager.getInfoUtente(this);
                    ListaFiltri = DocumentManager.getFiltroRicDoc(this);
                    DocsPAWA.DocsPaWR.InfoDocumento[] infoDoc;
                    if (UserManager.isFiltroAooEnabled(this))
                        numDocInRisposta = DocumentManager.getNumDocInRisposta(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, false);
                    else
                        numDocInRisposta = DocumentManager.getNumDocInRisposta(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, true);
                }
            }
            return numDocInRisposta;
        }

        public bool GetFiltroDocInRisposta(DocsPAWA.DocsPaWR.SchedaDocumento sd) //string docSys, string idRegistro, string tipoProto
        {
            try
            {

                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                if (!UserManager.isFiltroAooEnabled(this))
                {
                    if (sd.tipoProto.Equals("A") || (sd.protocollo != null && !string.IsNullOrEmpty(sd.protocollo.daProtocollare) && sd.protocollo.daProtocollare.Equals("1") || sd.predisponiProtocollazione == true))
                    {
                        //Filtro per REGISTRI VISIBILI ALL'UTENTE
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRI_UTENTE_IN_CONDITION_CON_NULL.ToString();
                        fV1.valore = (String)Session["inRegCondition"];
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        if (sd.tipoProto.Equals("G"))
                        {
                            //Filtro per REGISTRI VISIBILI ALL'UTENTE
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRI_UTENTE_IN_CONDITION_CON_NULL.ToString();
                            fV1.valore = (String)Session["inRegCondition"];
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        else
                        {

                            //Filtro per REGISTRO DEL DOCUMENTO PROTOCOLLATO
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                            fV1.valore = sd.registro.systemId;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                }

                //Filtro per ID_PARENT
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ID_PARENT.ToString();
                fV1.valore = sd.systemId;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                qV[0] = fVList;
                return true;

            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
                return false;
            }
        }

        public bool IsEnabledRiproponiConConoscenza
        {
            get
            {
                String riproponiConConoscenza = InitConfigurationKeys.GetValue("0", "FE_RIPROPONI_CON_CONOSCENZA");
                return riproponiConConoscenza == "1";
            }
        }

        private void msg_rimuoviTipologia_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                string msg = ProfilazioneDocManager.RemoveTipologyDoc(UserManager.getInfoUtente(), schedaDocumento, this);
                if (!String.IsNullOrEmpty(msg))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "removeTipology", "alert('" + msg + "');", true);
                }
                else
                {
                    schedaDocumento.template = null;
                    schedaDocumento.tipologiaAtto = null;
                    DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                    DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
                    ddl_tipoAtto.SelectedIndex = 0;
                }
            }
        }

        private void btn_delTipologyDoc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            msg_rimuoviTipologia.Confirm("Si conferma al rimozione della tipologia per questo documento ?");
        }

    }
}
