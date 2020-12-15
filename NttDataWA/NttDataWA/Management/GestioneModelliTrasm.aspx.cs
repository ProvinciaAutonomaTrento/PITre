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
using NttDataWA.DocsPaWR;
using System.Xml.Linq;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using System.Collections.Generic;

namespace NttDataWA.Management
{
    /// <summary>
    /// Summary description for GestioneModelliTrasm.
    /// </summary>
    public partial class GestioneModelliTrasm : System.Web.UI.Page
    {

        protected string idAmministrazione;

        protected NttDataWA.DocsPaWR.ModelloTrasmissione ModelloTrasmissione = new NttDataWA.DocsPaWR.ModelloTrasmissione();
        protected NttDataWA.DocsPaWR.RagioneTrasmissione[] listaRagioni;
        private NttDataWA.DocsPaWR.Registro[] userRegistri;
        protected string gerarchia_trasm;
        protected string cha_tipo_ragione;
        private Utente utente;

        private Ruolo userRuolo;

        #region Property

        private PrintReportRequestDataset RequestPrintReport
        {
            get
            {
                if (HttpContext.Current.Session["requestPrintReport"] != null)
                    return (PrintReportRequestDataset)HttpContext.Current.Session["requestPrintReport"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["requestPrintReport"] = value;
            }
        }


        #endregion

        private void Page_Load(object sender, System.EventArgs e)
        {
            idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
            userRuolo = UIManager.RoleManager.GetRoleInSession();
            utente = UIManager.UserManager.GetUserInSession();

            if (!IsPostBack)
            {
                this.ModelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.GetInfoUser(), this.prmtRicerca.CreateSearchFilters());
                caricaDataGridModelli();

                ModelloTrasmissione = new ModelloTrasmissione();
                ModelloTrasmissione.ID_AMM = idAmministrazione;
                ModelloTrasmissione.SINGLE = "0";
                ModelloTrasmissione.ID_PEOPLE = utente.idPeople;
                Session.Add("Modello", ModelloTrasmissione);

                if (ddl_registri.Items.Count == 0)
                    CaricaComboRegistri(ddl_registri);
            }
            else
            {
                ReadRetValueFromPopup();
                //caricaDataGridModelli();
            }
            if (((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]) != null && ((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MITTENTE == null)
            {
                DocsPaWR.MittDest mittente = new NttDataWA.DocsPaWR.MittDest();
                ((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MITTENTE = new MittDest[1];
                mittente.CHA_TIPO_MITT_DEST = "M";
                mittente.VAR_COD_RUBRICA = userRuolo.codiceRubrica;
                mittente.DESCRIZIONE = userRuolo.descrizione;
                mittente.ID_CORR_GLOBALI = Convert.ToInt32(userRuolo.systemId);
                mittente.CHA_TIPO_URP = "R";

                ((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MITTENTE[0] = mittente;
            }


            this.CaricaRagioni(idAmministrazione, false);
            if (Session["selDestDaRubrica"] != null)
            {
                addDestSelDaRubrica((ElementoRubrica[])Session["selDestDaRubrica"]);
            }

            tastoRicerca();

            //if (Session["retValueNotifiche"] != null)
            //{
            //    this.hd_returnValueModal.Value = Session["retValueNotifiche"].ToString();
            //    // valore di ritorno della modale delle notifiche (possibili valori: "I", "U" o "")
            //    if (this.hd_returnValueModal.Value != null && this.hd_returnValueModal.Value != string.Empty && this.hd_returnValueModal.Value != "undefined")
            //    {
            //        if (this.hd_returnValueModal.Value.Equals("I"))
            //            this.PerformSaveModel();

            //        Session["retValueNotifiche"] = null;
            //        this.hd_returnValueModal.Value = string.Empty;
            //    }
            //}

            if (Session["ClickFindAR"] != null)
            {
                Session.Remove("ClickFindAR");
                btn_lista_modelli_Click(null, null);
            }

            //if (Session["CompileDest"] != null)
            //    this.CompileCodeAndDescription();

            this.RefreshScript();
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);

            string maxLength = Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_TRASM.ToString());
            maxLength = string.IsNullOrEmpty(maxLength) || Int64.Parse(maxLength) > 2000 ? "2000" : maxLength;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshtxt_noteGenerali", "charsLeft('txt_noteGenerali', '" + maxLength + "', '" + this.Ltrtxt_noteGenerali.Text.Replace("'", "\'") + "');", true);
            this.txt_noteGenerali_chars.Attributes["rel"] = "txt_noteGenerali_'" + maxLength + "'_" + this.Ltrtxt_noteGenerali.Text;
        }
        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            this.InitializeLanguage();
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_lista_modelli.Click += new System.EventHandler(this.btn_lista_modelli_Click);
            this.btn_salvaModello.Click += new System.EventHandler(this.btn_salvaModello_Click);

            this.dt_listaModelli.SelectedIndexChanged += new System.EventHandler(this.dt_listaModelli_SelectedIndexChanged);
            this.txt_nomeModello.TextChanged += new System.EventHandler(this.txt_nomeModello_TextChanged);
            this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
            this.btn_Rubrica_dest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Rubrica_dest_Click);
            this.dt_dest.RowCreated += new GridViewRowEventHandler(this.dt_dest_ItemCreated);
            this.Load += new System.EventHandler(this.Page_Load);
            this.dt_listaModelli.PreRender += new EventHandler(dt_listaModelli_PreRender);
            this.PreRender += new EventHandler(GestioneModelliTrasm_PreRender);
            this.ddl_ragioni.SelectedIndexChanged += new EventHandler(ddl_ragioni_SelectedIndexChanged);
            this.btn_pp_notifica.Click += new System.EventHandler(this.btn_pp_notifica_Click);
            // Associazione evento di ricerca al pannello dei filtri
            this.prmtRicerca.Search += new EventHandler(this.btn_ricerca_Click);
        }

        public void RefreshPanel()
        {
            this.UpPnlListaModelli.Update();
            this.UpPnlNuovoModello.Update();
            this.UpPnlButtons.Update();
        }

        void ddl_ragioni_SelectedIndexChanged(object sender, EventArgs e)
        {
            DocsPaWR.RagioneTrasmissione ragioneSel = new NttDataWA.DocsPaWR.RagioneTrasmissione();
            ragioneSel = listaRagioni[this.ddl_ragioni.SelectedIndex];
            if (!string.IsNullOrEmpty(ragioneSel.mantieniLettura) && (ragioneSel.mantieniLettura == "1"))
            {
                if (((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]) != null)
                    ((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MANTIENI_LETTURA = "1";
            }
            else
                if (((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]) != null)
                    ((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MANTIENI_LETTURA = "0";

            Session["noRagioneTrasmNullaFE"] = true;
        }

        void GestioneModelliTrasm_PreRender(object sender, EventArgs e)
        {
            if (Session["noRagioneTrasmNullaFE"] == null)
            {
                if (dt_dest.Rows.Count != 0)
                {
                    this.lbl_ragione.Visible = false;
                    ListItem item = new ListItem("", "0");
                    if (!ddl_ragioni.Items.Contains(item))
                        ddl_ragioni.Items.Add(item);
                    int index = ddl_ragioni.Items.IndexOf(ddl_ragioni.Items.FindByValue("0"));

                    this.ddl_ragioni.SelectedIndex = index;
                }
                else
                    this.lbl_ragione.Visible = true;
            }
            else
                Session.Remove("noRagioneTrasmNullaFE");
        }

        void dt_listaModelli_PreRender(object sender, EventArgs e)
        {
            if (UserManager.IsAuthorizedFunctions("DO_DEL_MOD_TRASM"))
                this.dt_listaModelli.Columns[7].Visible = true;
            else
                this.dt_listaModelli.Columns[7].Visible = false;
        }

        protected void dt_dest_ItemCreated(object sender, GridViewRowEventArgs e)
        {
            try
            {

            }
            catch
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Selezione del destinatario


        ///// <summary>
        ///// Funzione per la compilazione delle caselle di codice e descrizione 
        ///// </summary>
        //private void CompileCodeAndDescription()
        //{
        //    try
        //    {

        //        List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
        //        ArrayList destinatari = new ArrayList();

        //        if (HttpContext.Current.Session["AddressBook.from"] != null)
        //        {
        //            string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

        //            switch (addressBookCallFrom)
        //            {
        //                case "M_D_T_M":
        //                    if (atList != null && atList.Count > 0)
        //                    {
        //                        string ragione = this.ddl_ragioni.SelectedItem.Text;
        //                        foreach (NttDataWA.Popup.AddressBook.CorrespondentDetail corr in atList)
        //                        {
        //                            Corrispondente tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(corr.SystemID);

        //                            DocsPaWR.MittDest destinatario = new NttDataWA.DocsPaWR.MittDest();
        //                            destinatario.CHA_TIPO_MITT_DEST = "D";
        //                            destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
        //                            destinatario.VAR_COD_RUBRICA = tempCorrSingle.codiceRubrica;
        //                            destinatario.DESCRIZIONE = tempCorrSingle.descrizione;
        //                            destinatario.CHA_TIPO_URP = tempCorrSingle.tipoCorrispondente;

        //                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(tempCorrSingle.systemId);

        //                            destinatari.Add(destinatario);
        //                        }

        //                        AggiornaRagioneDest();
        //                        AddMittDest(ragione, destinatari, cha_tipo_ragione);
        //                        caricaDataGridDest();
        //                        this.UpPnlNuovoModello.Update();
        //                        HttpContext.Current.Session["AddressBook.At"] = null;
        //                        HttpContext.Current.Session["AddressBook.Cc"] = null;
        //                        // Pulizia sessione
        //                        Session.Remove("CompileDest");

        //                    }
        //                    else
        //                        Session.Remove("CompileDest");
        //                    break;
        //            }

        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        UIManager.AdministrationManager.DiagnosticError(ex);
        //        return;
        //    }
        //}


        /// <summary>
        /// Seleziona destinatari per codice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ibtnMoveToA_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                Session["noRagioneTrasmNullaFE"] = true;
                if (this.ddl_ragioni.SelectedItem.Text.Equals(""))
                {
                    string msg = "WarningNoRagioneTrasmissione";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                }
                else
                {
                    if (this.txt_codDest.Text.Trim() != "" || this.txt_codDest.Text.Trim() != string.Empty)
                    {
                        DocsPaWR.ParametriRicercaRubrica qco = new NttDataWA.DocsPaWR.ParametriRicercaRubrica();
                        UserManager.setQueryRubricaCaller(ref qco);
                        qco.codice = this.txt_codDest.Text.Trim();
                        qco.tipoIE = NttDataWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
                        //cerco su tutti i tipi utente:
                        if (System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
                            qco.doListe = true;

                        if (AdministrationManager.IsEnableRF(UserManager.GetInfoUser().idAmministrazione))
                            qco.doRF = true;

                        qco.doRuoli = true;
                        qco.doUtenti = true;
                        qco.doUo = true;
                        //query per codice  esatta, no like.
                        qco.queryCodiceEsatta = true;

                        DocsPaWR.RagioneTrasmissione rt = TrasmManager.getRagioneSel(this);
                        this.gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0, 1);

                        switch (this.gerarchia_trasm)
                        {
                            case "T":
                                qco.calltype = NttDataWA.DocsPaWR.RubricaCallType.CALLTYPE_MODELLI_TRASM_ALL;
                                break;
                            case "I":
                                qco.calltype = NttDataWA.DocsPaWR.RubricaCallType.CALLTYPE_MODELLI_TRASM_INF;
                                break;
                            case "S":
                                qco.calltype = NttDataWA.DocsPaWR.RubricaCallType.CALLTYPE_MODELLI_TRASM_SUP;
                                break;
                            case "P":
                                qco.calltype = NttDataWA.DocsPaWR.RubricaCallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO;
                                break;
                        }

                        string objtype = string.Empty;

                        DocsPaWR.ElementoRubrica[] corrSearch = UserManager.getElementiRubrica(this.Page, qco);
                        if (corrSearch != null && corrSearch.Length > 0)
                        {
                            //Verifica della disabilitazione alla ricezione delle trasmissioni
                            if (corrSearch[0].tipo == "R" && corrSearch[0].disabledTrasm)
                            {
                                string msg = "WarningRuoloDisabilitatoTrasmissione";

                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                            else
                            {
                                this.ImpostaDestinatario(corrSearch, qco);
                                this.txt_codDest.Text = "";
                            }
                        }
                        else
                        {
                            string msg = "WarningNoDestinatarioCodice";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                        }

                        corrSearch = null;


                        qco = null;
                        rt = null;

                        this.dt_dest.Visible = true;
                        this.UpPnlDest.Update();
                    }
                }
            }
            catch
            {
                string msg = "WarningErrorRicercaDestinatario";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

            }
            this.RefreshPanel();
            this.Page_Load(null, null);

        }

        /// <summary>
        /// Imposta Destinatario
        /// </summary>
        /// <param name="corrSearch"></param>
        /// <param name="qco"></param>
        private void ImpostaDestinatario(NttDataWA.DocsPaWR.ElementoRubrica[] corrSearch, NttDataWA.DocsPaWR.ParametriRicercaRubrica prr)
        {
            string t_avviso = string.Empty;
            DocsPaWR.Corrispondente corr;

            DocsPaWR.Trasmissione trasmissione = TrasmManager.getGestioneTrasmissione(this.Page);

            // verifica liste di distribuzione
            if (corrSearch[0].tipo.Equals("L"))
            {
                if (System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
                {
                    string idAmm = UserManager.GetInfoUser().idAmministrazione;
                    ArrayList listaCorr = UserManager.getCorrispondentiByCodLista(this.Page, prr.codice, idAmm);
                    if (listaCorr != null && listaCorr.Count > 0)
                    {
                        DocsPaWR.Trasmissione trasmissioni = TrasmManager.getGestioneTrasmissione(this.Page);
                        if (trasmissioni == null)
                            trasmissioni = new Trasmissione();

                        ElementoRubrica[] ers = new ElementoRubrica[listaCorr.Count];
                        ArrayList el = null;

                        if (Session["selDestDaRubrica"] != null)
                            el = (ArrayList)Session["selDestDaRubrica"];

                        ArrayList nuovaListaCorr = el;

                        for (int i = 0; i < listaCorr.Count; i++)
                        {
                            DocsPaWR.ElementoRubrica er_1 = new NttDataWA.DocsPaWR.ElementoRubrica();
                            DocsPaWR.Corrispondente c = (NttDataWA.DocsPaWR.Corrispondente)listaCorr[i];
                            er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                            ers[i] = er_1;

                            bool daInserire = true;
                            if (el != null)
                            {
                                foreach (Corrispondente corrisp in el)
                                {
                                    if (corrisp.systemId == c.systemId)
                                        daInserire = false;
                                }
                            }
                            else
                                if (nuovaListaCorr == null)
                                    nuovaListaCorr = new ArrayList();

                            if (daInserire)
                                nuovaListaCorr.Add(c);
                        }

                        NttDataWA.DocsPaWR.ElementoRubrica[] elRub = new ElementoRubrica[nuovaListaCorr.Count];
                        for (int a = 0; a < nuovaListaCorr.Count; a++)
                        {
                            DocsPaWR.ElementoRubrica er_1 = new NttDataWA.DocsPaWR.ElementoRubrica();
                            DocsPaWR.Corrispondente c = (NttDataWA.DocsPaWR.Corrispondente)nuovaListaCorr[a];
                            er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                            elRub[a] = er_1;
                        }
                        Session.Add("selDestDaRubrica", elRub);

                    }
                }
                else
                {
                    string msg = "WarningListeAttivate";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
            }
            else
                if (corrSearch[0].tipo.Equals("F"))
                {

                    if (AdministrationManager.IsEnableRF(UserManager.GetInfoUser().idAmministrazione))
                    {
                        string idAmm = UserManager.GetInfoUser().idAmministrazione;
                        ArrayList listaCorr = UserManager.getCorrispondentiByCodRF(this.Page, prr.codice);
                        if (listaCorr != null && listaCorr.Count > 0)
                        {
                            DocsPaWR.Trasmissione trasmissioni = TrasmManager.getGestioneTrasmissione(this.Page);
                            if (trasmissioni == null)
                                trasmissioni = new Trasmissione();

                            ElementoRubrica[] ers = new ElementoRubrica[listaCorr.Count];
                            ArrayList el = null;

                            if (Session["selDestDaRubrica"] != null)
                                el = (ArrayList)Session["selDestDaRubrica"];

                            ArrayList nuovaListaCorr = el;

                            for (int i = 0; i < listaCorr.Count; i++)
                            {
                                DocsPaWR.ElementoRubrica er_1 = new NttDataWA.DocsPaWR.ElementoRubrica();
                                DocsPaWR.Corrispondente c = (NttDataWA.DocsPaWR.Corrispondente)listaCorr[i];
                                er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                                ers[i] = er_1;

                                bool daInserire = true;
                                if (el != null)
                                {
                                    foreach (Corrispondente corrisp in el)
                                    {
                                        if (corrisp.systemId == c.systemId)
                                            daInserire = false;
                                    }
                                }
                                else
                                    if (nuovaListaCorr == null)
                                        nuovaListaCorr = new ArrayList();

                                if (daInserire)
                                    nuovaListaCorr.Add(c);
                            }

                            NttDataWA.DocsPaWR.ElementoRubrica[] elRub = new ElementoRubrica[nuovaListaCorr.Count];
                            for (int a = 0; a < nuovaListaCorr.Count; a++)
                            {
                                DocsPaWR.ElementoRubrica er_1 = new NttDataWA.DocsPaWR.ElementoRubrica();
                                DocsPaWR.Corrispondente c = (NttDataWA.DocsPaWR.Corrispondente)nuovaListaCorr[a];
                                er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                                elRub[a] = er_1;
                            }
                            Session.Add("selDestDaRubrica", elRub);
                        }
                    }
                    else
                    {
                        string msg = "WarningRfNonPrevisti";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        return;
                    }
                }
                else
                {
                    DocsPaWR.AddressbookQueryCorrispondente qco = new NttDataWA.DocsPaWR.AddressbookQueryCorrispondente();
                    qco.codiceRubrica = prr.codice;
                    qco.getChildren = false;
                    qco.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                    qco.tipoUtente = NttDataWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.fineValidita = true;

                    corr = UserManager.getListaCorrispondenti(this.Page, qco)[0];
                    ArrayList el = null;

                    if (Session["selDestDaRubrica"] != null)
                        el = (ArrayList)Session["selDestDaRubrica"];

                    ArrayList nuovaListaCorr = el;
                    bool daInserire = true;
                    if (el != null)
                    {
                        foreach (Corrispondente c in el)
                        {
                            if (corr.systemId == c.systemId)
                                daInserire = false;
                        }
                    }
                    else
                        if (nuovaListaCorr == null)
                            nuovaListaCorr = new ArrayList();

                    if (daInserire)
                        nuovaListaCorr.Add(corr);

                    NttDataWA.DocsPaWR.ElementoRubrica[] elRub = new ElementoRubrica[nuovaListaCorr.Count];
                    for (int a = 0; a < nuovaListaCorr.Count; a++)
                    {
                        DocsPaWR.ElementoRubrica er_1 = new NttDataWA.DocsPaWR.ElementoRubrica();
                        DocsPaWR.Corrispondente c = (NttDataWA.DocsPaWR.Corrispondente)nuovaListaCorr[a];
                        er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                        elRub[a] = er_1;
                    }
                    Session.Add("selDestDaRubrica", elRub);


                }



        }


        /// <summary>
        /// Aggiunge Trasmissione Singola
        /// </summary>
        /// <param name="trasmissione"></param>
        /// <param name="corr"></param>
        /// <returns></returns>
        private NttDataWA.DocsPaWR.Trasmissione addTrasmissioneSingola(NttDataWA.DocsPaWR.Trasmissione trasmissione, NttDataWA.DocsPaWR.Corrispondente corr)
        {
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                {
                    DocsPaWR.TrasmissioneSingola ts = (NttDataWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((NttDataWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaWR.TrasmissioneSingola trasmissioneSingola = new NttDataWA.DocsPaWR.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = "S";
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = TrasmManager.getRagioneSel(this.Page);

            // Aggiungo la lista di trasmissioniUtente
            if (corr is NttDataWA.DocsPaWR.Ruolo)
            {
                trasmissioneSingola.tipoDest = NttDataWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);
                if (listaUtenti.Length == 0)
                    trasmissioneSingola = null;

                //ciclo per utenti se dest è gruppo o ruolo
                for (int i = 0; i < listaUtenti.Length; i++)
                {
                    DocsPaWR.TrasmissioneUtente trasmissioneUtente = new NttDataWA.DocsPaWR.TrasmissioneUtente();
                    trasmissioneUtente.utente = (NttDataWA.DocsPaWR.Utente)listaUtenti[i];

                    trasmissioneUtente.daNotificare = TrasmManager.getTxRuoloUtentiChecked();
                    trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                }
            }

            if (corr is NttDataWA.DocsPaWR.Utente)
            {
                trasmissioneSingola.tipoDest = NttDataWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                DocsPaWR.TrasmissioneUtente trasmissioneUtente = new NttDataWA.DocsPaWR.TrasmissioneUtente();
                trasmissioneUtente.utente = (NttDataWA.DocsPaWR.Utente)corr;

                trasmissioneUtente.daNotificare = TrasmManager.getTxRuoloUtentiChecked();
                trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
            }

            if (corr is NttDataWA.DocsPaWR.UnitaOrganizzativa)
            {
                DocsPaWR.UnitaOrganizzativa theUo = (NttDataWA.DocsPaWR.UnitaOrganizzativa)corr;
                DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new NttDataWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = RoleManager.GetRoleInSession();

                DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(this.Page, qca, theUo);
                foreach (NttDataWA.DocsPaWR.Ruolo r in ruoli)
                    trasmissione = addTrasmissioneSingola(trasmissione, r);

                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);

            return trasmissione;
        }

        /// <summary>
        /// query Utenti
        /// </summary>
        /// <param name="corr"></param>
        /// <returns></returns>
        private NttDataWA.DocsPaWR.Corrispondente[] queryUtenti(NttDataWA.DocsPaWR.Corrispondente corr)
        {

            //costruzione oggetto queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new NttDataWA.DocsPaWR.AddressbookQueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;

            qco.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
            qco.fineValidita = true;

            //corrispondenti interni
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;

            return UserManager.getListaCorrispondenti(this.Page, qco);
        }

        #endregion

        #region DATAGRID

        public void caricaDataGridModelli()
        {
            this.EnableEsportAndFindButtons = true;
            if (this.ModelliTrasmissione != null && this.ModelliTrasmissione.Count != 0)
            {

                DataTable dt = new DataTable();
                dt.Columns.Add("SYSTEM_ID");
                dt.Columns.Add("CODICE");
                dt.Columns.Add("MODELLO");
                dt.Columns.Add("REGISTRO");
                dt.Columns.Add("TIPO DI TRASM.");
                dt.Columns.Add("VISIBILITA'");

                foreach (ModelloTrasmissione model in this.ModelliTrasmissione)
                {
                    if (model.NumMittenti <= 1) //visualizzo solo i modelli con un mittente
                    {
                        DocsPaWR.Registro reg = null;
                        if (model.ID_REGISTRO != "0")
                        {
                            reg = UserManager.getRegistroBySistemId(this, model.ID_REGISTRO);
                        }
                        DataRow row = dt.NewRow();

                        row[0] = model.SYSTEM_ID;
                        //row[1] = model.CODICE;
                        row[1] = this.GetSpanElement((model).Valid, model.CODICE);
                        //row[2] = model.NOME;
                        row[2] = this.GetSpanElement((model).Valid, model.NOME);
                        if (reg != null)
                        {
                            row[3] = reg.descrizione;
                        }
                        else
                        {
                            row[3] = "";
                        }
                        if (model.CHA_TIPO_OGGETTO == "D")
                            row[4] = "Documento";
                        if (model.CHA_TIPO_OGGETTO == "F")
                            row[4] = "Fascicolo";
                        if (!string.IsNullOrEmpty(model.ID_PEOPLE))
                            row[5] = "Solo a me stesso";
                        else
                            row[5] = "A tutto il ruolo";

                        dt.Rows.Add(row);
                    }

                }

                this.prmtRicerca.SearchResult = String.Empty;
                dt_listaModelli.DataSource = dt;



                dt_listaModelli.DataBind();
                dt_listaModelli.Visible = true;
            }
            else
            {
                this.EnableEsportAndFindButtons = false;
                dt_listaModelli.Visible = false;
                //messaggio nessun modello risponde alla ricerca effettuata                
                this.prmtRicerca.SearchResult = "Nessun modello per questa ricerca!";
            }
        }



        public bool EnableEsportAndFindButtons
        {
            set
            {
                this.btnExport.Enabled = value;
                this.btnFindAndReplace.Enabled = value;
            }
        }


        /// <summary>
        /// Funzione per la creazione di uno span element per codice e descrizione di un modello
        /// </summary>
        /// <param name="valid">True se il modello è valido</param>
        /// <param name="text">Testo da decorare</param>
        /// <returns>Elemento span contenete il testo text</returns>
        private String GetSpanElement(bool valid, String text)
        {
            // Span da utilizzare per la decorazione di codice e descrizione del destinatario
            String retVal = "<span style=\"color: {0}; {1}\">{2}</span>";

            // Se il modello non è valido, viene colorato in rosso altrimenti in nero
            if (valid)
                retVal = String.Format(retVal, "Black", String.Empty, text);
            else
                retVal = String.Format(retVal, "Red", String.Empty, text);

            return retVal;
        }

        /// <summary>
        /// Funzione per la costruzione di un elemento span in cui inserire codice e descrizione del corrispondente
        /// </summary>
        /// <param name="dest">Destinatario da verificare</param>
        /// <param name="text">Testo da inserire nell'elemento span</param>
        /// <returns>Elemento span con stile e testo impostati</returns>
        private String GetSpanElement(MittDest dest, String text)
        {
            // Valore da restituire
            String retVal = String.Empty;

            // Span da utilizzare per la decorazione di codice e descrizione del destinatario
            String formatString = "<span style=\"color: {0}; {1}\">{2}</span>";

            // Se il ruolo è inibito, viene colorato di rosso
            if (dest.Inhibited)
                retVal = String.Format("<span style=\"color:Red;\">{0}</span>", text);

            // Se il ruolo è disabilitato, viene visualizzato nero barrato
            if (dest.Disabled)
                retVal = String.Format("<span style=\"text-decoration: line-through;\">{0}</span>", String.IsNullOrEmpty(retVal) ? text : retVal);

            if (!dest.Disabled && !dest.Inhibited)
                retVal = String.Format("<span style=\"color:Black;\">{0}</span>", text);


            return retVal;
        }

        public void caricaDataGridDest()
        {
            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            DataTable dt = new DataTable();
            dt.Columns.Add("SYSTEM_ID");
            dt.Columns.Add("RAGIONE");
            dt.Columns.Add("VAR_COD_RUBRICA");
            dt.Columns.Add("VAR_DESC_CORR");
            dt.Columns.Add("ID_RAGIONE");
            dt.Columns.Add("NASCONDI_VERSIONI_PRECEDENTI", typeof(Boolean));

            if (modello != null && modello.RAGIONI_DESTINATARI != null)
            {
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {
                    for (int j = 0; j < modello.RAGIONI_DESTINATARI[i].DESTINATARI.Length; j++)
                    {
                        DataRow row = dt.NewRow();
                        DocsPaWR.RagioneDest rd = modello.RAGIONI_DESTINATARI[i];
                        if (j == 0)
                            row[1] = rd.RAGIONE;
                        DocsPaWR.MittDest mittDest = modello.RAGIONI_DESTINATARI[i].DESTINATARI[j];
                        row[0] = mittDest.SYSTEM_ID;
                        //row[2] = mittDest.VAR_COD_RUBRICA;
                        row[2] = this.GetSpanElement(mittDest, mittDest.VAR_COD_RUBRICA);
                        if (!string.IsNullOrEmpty(mittDest.DESCRIZIONE))
                        {
                            //row[3] = mittDest.DESCRIZIONE;
                            row[3] = this.GetSpanElement(mittDest, mittDest.DESCRIZIONE);
                        }
                        else
                        {
                            if (mittDest.CHA_TIPO_MITT_DEST == "UT_P")
                                row[3] = "Utente proprietario";
                            if (mittDest.CHA_TIPO_MITT_DEST == "R_P")
                                row[3] = "Ruolo proprietario";
                            if (mittDest.CHA_TIPO_MITT_DEST == "UO_P")
                                row[3] = "UO proprietaria";
                            if (mittDest.CHA_TIPO_MITT_DEST == "RSP_P")
                                row[3] = "Resp. UO proprietaria";
                        }
                        row[4] = mittDest.ID_RAGIONE;
                        row["NASCONDI_VERSIONI_PRECEDENTI"] = mittDest.NASCONDI_VERSIONI_PRECEDENTI;

                        dt.Rows.Add(row);
                    }
                }
            }
            dt_dest.DataSource = dt;
            dt_dest.DataBind();

            this.UpPnlDest.Update();

            this.btn_pp_notifica.Visible = false;

            if (dt_dest.Rows.Count != 0)
            {
                int k = 0;
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {
                    for (int j = 0; j < modello.RAGIONI_DESTINATARI[i].DESTINATARI.Length; j++)
                    {

                        DocsPaWR.RagioneDest rd = modello.RAGIONI_DESTINATARI[i];
                        DocsPaWR.MittDest mittDest = modello.RAGIONI_DESTINATARI[i].DESTINATARI[j];

                        string imgurl = "../Images/Icons/";
                        switch (mittDest.CHA_TIPO_URP)
                        {
                            case "U":
                                ((ImageButton)dt_dest.Rows[k].Cells[2].Controls[1]).ImageUrl = imgurl + "uo_icon.png";
                                ((ImageButton)dt_dest.Rows[k].Cells[2].Controls[1]).ToolTip = "Ufficio";

                                break;

                            case "R":
                                ((ImageButton)dt_dest.Rows[k].Cells[2].Controls[1]).ImageUrl = imgurl + "role2_icon.png";
                                ((ImageButton)dt_dest.Rows[k].Cells[2].Controls[1]).ToolTip = "Ruolo";
                                break;

                            case "P":
                                ((ImageButton)dt_dest.Rows[k].Cells[2].Controls[1]).ImageUrl = imgurl + "user_icon.png";
                                ((ImageButton)dt_dest.Rows[k].Cells[2].Controls[1]).ToolTip = "Utente";
                                break;
                        }

                        ((TextBox)dt_dest.Rows[k].Cells[6].Controls[1]).Text = mittDest.VAR_NOTE_SING;
                        if (mittDest.CHA_TIPO_TRASM == "S")
                            ((DropDownList)dt_dest.Rows[k].Cells[5].Controls[1]).SelectedIndex = 0;
                        if (mittDest.CHA_TIPO_TRASM == "T")
                            ((DropDownList)dt_dest.Rows[k].Cells[5].Controls[1]).SelectedIndex = 1;
                        if (!modello.RAGIONI_DESTINATARI[i].CHA_TIPO_RAGIONE.Equals("W"))
                        {

                            ((TextBox)dt_dest.Rows[k].Cells[7].Controls[3]).ReadOnly = true;
                            ((TextBox)dt_dest.Rows[k].Cells[7].Controls[3]).BackColor = Color.Gray;
                        }
                        else
                        {
                            if (mittDest.SCADENZA != 0)
                                ((TextBox)dt_dest.Rows[k].Cells[7].Controls[3]).Text = mittDest.SCADENZA.ToString();
                            else
                                ((TextBox)dt_dest.Rows[k].Cells[7].Controls[3]).Text = "";
                        }
                        k++;
                    }
                }
                dt_dest.Visible = true;

                //this.gestioneTastoNotifiche((modello.SYSTEM_ID > 0));
            }
        }

        private void addDestSelDaRubrica(ElementoRubrica[] selDestDaRubrica)
        {
            if (selDestDaRubrica != null)
            {
                string ragione = this.ddl_ragioni.SelectedItem.Text;
                ArrayList destinatari = new ArrayList();
                for (int i = 0; i < selDestDaRubrica.Length; i++)
                {
                    DocsPaWR.ElementoRubrica el = (NttDataWA.DocsPaWR.ElementoRubrica)selDestDaRubrica[i];
                    if (el.tipo.Equals("L"))
                    {
                        string idAmm = UserManager.GetInfoUser().idAmministrazione;
                        ArrayList lista = UserManager.getCorrispondentiByCodLista(this.Page, el.codice, idAmm);
                        foreach (Corrispondente corr in lista)
                        {
                            DocsPaWR.MittDest destinatario = new NttDataWA.DocsPaWR.MittDest();
                            destinatario.CHA_TIPO_MITT_DEST = "D";
                            destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                            destinatario.VAR_COD_RUBRICA = corr.codiceRubrica;
                            destinatario.DESCRIZIONE = corr.descrizione;
                            destinatario.CHA_TIPO_URP = corr.tipoCorrispondente;

                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(corr.systemId);

                            destinatari.Add(destinatario);
                        }
                    }
                    else if (el.tipo.Equals("F"))
                    {
                        ArrayList lista = UserManager.getCorrispondentiByCodRF(this.Page, el.codice);
                        foreach (Corrispondente corr in lista)
                        {
                            DocsPaWR.MittDest destinatario = new NttDataWA.DocsPaWR.MittDest();
                            destinatario.CHA_TIPO_MITT_DEST = "D";
                            destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                            destinatario.VAR_COD_RUBRICA = corr.codiceRubrica;
                            destinatario.DESCRIZIONE = corr.descrizione;
                            destinatario.CHA_TIPO_URP = corr.tipoCorrispondente;

                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(corr.systemId);

                            destinatari.Add(destinatario);
                        }
                    }
                    else
                    {
                        DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this, el.codice, el.interno ? NttDataWA.DocsPaWR.AddressbookTipoUtente.INTERNO : NttDataWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);

                        DocsPaWR.MittDest destinatario = new NttDataWA.DocsPaWR.MittDest();
                        destinatario.CHA_TIPO_MITT_DEST = "D";
                        destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                        destinatario.VAR_COD_RUBRICA = el.codice;
                        destinatario.DESCRIZIONE = el.descrizione;
                        destinatario.CHA_TIPO_URP = el.tipo;
                        destinatario.ID_CORR_GLOBALI = Convert.ToInt32(corr.systemId);

                        destinatari.Add(destinatario);
                    }
                }
                Session.Remove("selDestDaRubrica");
                AggiornaRagioneDest();
                AddMittDest(ragione, destinatari, cha_tipo_ragione);
                caricaDataGridDest();
            }
        }

        private void AggiornaRagioneDest()
        {
            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            if (dt_dest != null && dt_dest.Rows != null && dt_dest.Rows.Count > 0)
            {

                int n = 0;
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {
                    for (int j = 0; j < modello.RAGIONI_DESTINATARI[i].DESTINATARI.Length; j++)
                    {
                        ((NttDataWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]).CHA_TIPO_TRASM = ((DropDownList)dt_dest.Rows[n].Cells[5].Controls[1]).SelectedValue;
                        ((NttDataWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]).VAR_NOTE_SING = ((TextBox)dt_dest.Rows[n].Cells[6].Controls[1]).Text;
                        if (((TextBox)dt_dest.Rows[n].Cells[7].Controls[3]).Text != "")
                            ((NttDataWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]).SCADENZA = Convert.ToInt32(((TextBox)dt_dest.Rows[n].Cells[7].Controls[3]).Text);
                        else
                            ((NttDataWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]).SCADENZA = 0;

                        CheckBox chkNascondiVersioniPrecedentiDocumento = dt_dest.Rows[n].FindControl("chkNascondiVersioniPrecedentiDocumento") as CheckBox;
                        if (chkNascondiVersioniPrecedentiDocumento != null)
                            ((NttDataWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]).NASCONDI_VERSIONI_PRECEDENTI = chkNascondiVersioniPrecedentiDocumento.Checked;
                        else
                            ((NttDataWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]).NASCONDI_VERSIONI_PRECEDENTI = false;

                        n++;
                    }
                }
                Session.Add("Modello", modello);
            }

        }

        private void AddMittDest(string ragione, ArrayList destinatari, string cha_tipo_ragione)
        {
            ArrayList array_1;
            DocsPaWR.ModelloTrasmissione modello = ((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]);

            //controllo se esiste già una ragioneDest
            if (((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI != null)
            {
                //controllo se esiste per la ragione
                DocsPaWR.RagioneDest rd = null;
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {
                    if (((NttDataWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i]).RAGIONE.Equals(ragione))
                    {
                        rd = (NttDataWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                        break;
                    }
                }

                //se esiste già una ragione aggiungo i destinatari alla stessa

                if (rd != null)
                {
                    array_1 = new ArrayList(rd.DESTINATARI);
                    verificaEsistenzaDest(array_1, ref destinatari);
                    array_1.AddRange(destinatari);
                    rd.DESTINATARI = new NttDataWA.DocsPaWR.MittDest[array_1.Count];
                    array_1.CopyTo(rd.DESTINATARI);
                }
                else
                {
                    DocsPaWR.RagioneDest rd1 = new NttDataWA.DocsPaWR.RagioneDest();
                    rd1.RAGIONE = ragione;
                    rd1.CHA_TIPO_RAGIONE = cha_tipo_ragione;
                    array_1 = new ArrayList();
                    array_1.AddRange(destinatari);
                    rd1.DESTINATARI = new NttDataWA.DocsPaWR.MittDest[array_1.Count];
                    array_1.CopyTo(rd1.DESTINATARI);

                    ArrayList array_2;
                    if (((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI != null)
                        array_2 = new ArrayList(((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI);
                    else
                        array_2 = new ArrayList();

                    array_2.Add(rd1);
                    ((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI = new NttDataWA.DocsPaWR.RagioneDest[array_2.Count];
                    array_2.CopyTo(((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI);
                }

            }
            else
            {
                DocsPaWR.RagioneDest rd1 = new NttDataWA.DocsPaWR.RagioneDest();
                rd1.RAGIONE = ragione;
                rd1.CHA_TIPO_RAGIONE = cha_tipo_ragione;
                array_1 = new ArrayList();
                array_1.AddRange(destinatari);
                rd1.DESTINATARI = new NttDataWA.DocsPaWR.MittDest[array_1.Count];
                array_1.CopyTo(rd1.DESTINATARI);

                ArrayList array_2 = new ArrayList();
                array_2.Add(rd1);
                ((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI = new NttDataWA.DocsPaWR.RagioneDest[array_2.Count];
                array_2.CopyTo(((NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI);
            }

            Session.Add("Modello", modello);
        }


        private void verificaEsistenzaDest(ArrayList destVecchi, ref ArrayList destNuovi)
        {
            ArrayList elNuovi_1 = new ArrayList();
            bool result = true;
            for (int i = 0; i < destNuovi.Count; i++)
            {
                for (int j = 0; j < destVecchi.Count; j++)
                {
                    if (((NttDataWA.DocsPaWR.MittDest)destNuovi[i]).VAR_COD_RUBRICA == ((NttDataWA.DocsPaWR.MittDest)destVecchi[j]).VAR_COD_RUBRICA)
                        result = false;
                }
                if (result)
                {
                    elNuovi_1.Add(destNuovi[i]);
                }
                result = true;
            }
            destNuovi.Clear();
            destNuovi.AddRange(elNuovi_1);
        }

        #endregion

        #region SetFocus
        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
            RegisterStartupScript("focus", s);
        }
        #endregion SetFocus

        #region Utils

        private void pulisciCampi()
        {
            this.txt_nomeModello.Text = "";
            this.txt_noteGenerali.Text = "";
            this.ddl_ragioni.SelectedIndex = 0;
            this.ddl_tipoTrasmissione.SelectedIndex = 0;
        }

        private string CheckFields()
        {
            string msg = string.Empty;

            if (this.txt_nomeModello.Text.Trim() == "")
            {
                msg = "Inserire il nome del modello.";
                SetFocus(this.txt_nomeModello);
                return msg;
            }

            if (this.dt_dest != null && this.dt_dest.Rows.Count == 0)
            {
                msg = "Inserire almeno un destinatario del modello.";
                return msg;
            }

            for (int i = 0; i < dt_dest.Rows.Count; i++)
            {
                if (!((TextBox)dt_dest.Rows[i].Cells[7].Controls[3]).ReadOnly && ((TextBox)dt_dest.Rows[i].Cells[7].Controls[3]).Text != "")
                {
                    try
                    {
                        int giorniScadenza = Convert.ToInt32(((TextBox)dt_dest.Rows[i].Cells[7].Controls[3]).Text);
                    }
                    catch (Exception e)
                    {
                        msg = "I giorni di scadenza devono essere un numero intero.";
                        return msg;
                    }
                }
            }

            msg = this.checkRuoliUtentiDuplicati();
            if (msg != string.Empty)
            {
                msg = "Non è possibile inserire gli stessi destinatari con ragioni di trasmissione diverse: <br/>" + msg;
                return msg;
            }

            msg = this.checkRagTrasmConCessioneDuplicate();
            if (msg != string.Empty)
            {
                msg = "Non è possibile inserire più ragioni di trasmissione che prevedono cessione: <br/>" + msg;
                return msg;
            }

            msg = this.checkModUOConCessione();
            if (msg != string.Empty)
            {
                msg = "Non è possibile inserire ragioni che prevedono cessione se i destinatari sono UO: <br/>" + msg;
                return msg;
            }

            string valoreChiave = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, "FE_MAX_LENGTH_DESC_TRASM");
            if (this.txt_noteGenerali.Text.Length > Convert.ToInt32(valoreChiave))
                msg = "La lunghezza massima prevista per le note è di " + valoreChiave + " caratteri!";

            if (this.ddl_registri.Visible)
            {
                if (string.IsNullOrEmpty(this.ddl_registri.SelectedValue))
                {
                    msg = "Selezionare almeno un registro";
                }
            }

            return msg;
        }


        #endregion utils

        private void CaricaComboRegistri(DropDownList ddl)
        {
            //userRegistri = UserManager.getListaRegistri(this);
            userRegistri = RoleManager.GetRoleInSession().registri;
            if (userRegistri.Length > 1)
            {
                ListItem it = new ListItem("", "");
                this.ddl_registri.Items.Add(it);

                foreach (NttDataWA.DocsPaWR.Registro registro in userRegistri)
                {
                    ListItem item = new ListItem(registro.codRegistro, registro.systemId);
                    this.ddl_registri.Items.Add(item);
                }
            }
            else
            {
                this.ddl_registri.Visible = false;

                this.lbl_registri.Text = userRegistri[0].descrizione;
                this.lbl_registri.Visible = true;
            }
        }

        public void CaricaRagioni(string idAmm, bool all)
        {
            try
            {
                listaRagioni = UIManager.ModelliTrasmManager.getlistRagioniTrasm(idAmm, all, this.ddl_tipoTrasmissione.Text);
                if (listaRagioni != null && listaRagioni.Length > 0)
                {
                    int selezione = ddl_ragioni.SelectedIndex;
                    ddl_ragioni.Items.Clear();
                    for (int i = 0; i < listaRagioni.Length; i++)
                    {
                        ListItem newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                        ddl_ragioni.Items.Add(newItem);
                    }

                    DocsPaWR.RagioneTrasmissione ragioneSel = new NttDataWA.DocsPaWR.RagioneTrasmissione();
                    if (selezione >= listaRagioni.Length)
                    {
                        ListItem item = new ListItem("", "0");
                        if (!ddl_ragioni.Items.Contains(item))
                            ddl_ragioni.Items.Add(item);
                        selezione = ddl_ragioni.Items.IndexOf(ddl_ragioni.Items.FindByValue("0"));
                        ragioneSel = listaRagioni[0];
                    }
                    else
                    {
                        ragioneSel = listaRagioni[this.ddl_ragioni.SelectedIndex];
                    }

                    ddl_ragioni.SelectedIndex = selezione;

                    TrasmManager.setRagioneSel(this, ragioneSel);
                    DocsPaWR.RagioneTrasmissione rt = TrasmManager.getRagioneSel(this);
                    this.gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0, 1);
                    this.cha_tipo_ragione = rt.tipo;
                }

            }
            catch
            {
                string msg = "WarningErroreCaricamentoRagioni";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);


            }
        }

        private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.ddl_registri.SelectedIndex != 0)
            {
                DocsPaWR.Registro reg = RegistryManager.getRegistroBySistemId(this.ddl_registri.SelectedValue);
                if (reg.Sospeso)
                {
                    string msg = "WarningRegistroSospeso";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                    this.ddl_registri.SelectedIndex = 0;
                    return;
                }
            }

            if (dt_dest.Rows.Count != 0)
            {
                //msg_ConfirmDel.Confirm("Attenzione. La seguente modifica comporta la perdita dei dati finora inseriti.");
                DocsPaWR.Registro reg = new Registro();
                if (Session["userRegistro"] != null)
                    reg = (NttDataWA.DocsPaWR.Registro)Session["userRegistro"];

                reg.systemId = this.ddl_registri.SelectedValue.ToString();
                reg.codRegistro = this.ddl_registri.SelectedItem.Text.ToString();
                Session.Add("userRegistro", reg);
                return;
            }

            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            modello.ID_AMM = idAmministrazione;
            modello.NOME = this.txt_nomeModello.Text;
            modello.VAR_NOTE_GENERALI = this.txt_noteGenerali.Text;
            modello.CHA_TIPO_OGGETTO = this.ddl_tipoTrasmissione.SelectedValue;
            modello.ID_REGISTRO = this.ddl_registri.SelectedValue;
            modello.CODICE = this.txt_codModello.Text;
            Session.Add("Modello", modello);
            AggiornaRagioneDest();

            DocsPaWR.Registro registro = new Registro();
            if (Session["userRegistro"] != null)
                registro = (NttDataWA.DocsPaWR.Registro)Session["userRegistro"];

            registro.systemId = this.ddl_registri.SelectedValue.ToString();
            registro.codRegistro = this.ddl_registri.SelectedItem.Text.ToString();
            Session.Add("userRegistro", registro);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddl_tipoTrasmissione_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.EnableColumnNascondiVersioni(this.ddl_tipoTrasmissione.SelectedValue == "D");

            this.caricaDataGridDest();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        protected void EnableColumnNascondiVersioni(bool enabled)
        {
            int index = -1;
            int i = 0;

            foreach (DataControlField col in this.dt_dest.Columns)
            {
                if (col.HeaderText == "Nasc. vers.")
                {
                    index = i;
                    break;
                }

                i++;
            }

            if (index > -1)
            {
                //Commeno la seguente riga:rimuovendo la colonna, schianta nelle operazioni successive
                //this.dt_dest.Columns.RemoveAt(index);
                if(!enabled)
                    this.dt_dest.Columns[index].Visible = false;
                else
                    this.dt_dest.Columns[index].Visible = true;
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



        protected void btn_nuovoModello_Click(object sender, System.EventArgs e)
        {
            pulisciCampi();
            this.Panel_ListaModelli.Visible = false;
            this.Panel_NuovoModello.Visible = true;
            this.lbl_registro_obb.Visible = true;
            this.lbl_registri.Visible = true;
            this.Panel_dest.Visible = true;
            this.btn_salvaModello.Visible = true;
            this.btn_lista_modelli.Visible = true;
            ModelloTrasmissione = new NttDataWA.DocsPaWR.ModelloTrasmissione();
            ModelloTrasmissione.ID_AMM = idAmministrazione;

            this.txt_codModello.Visible = false;
            this.lbl_codice.Visible = false;

            SetFocus(this.txt_nomeModello);
            Session.Add("Modello", ModelloTrasmissione);

            this.EnableColumnNascondiVersioni(this.ddl_tipoTrasmissione.SelectedValue == "D");

            caricaDataGridDest();

            // imposta una sessione per mantenere lo stato di "bisogna SALVARE"
            bool modelloToSave = true;
            Session.Add("modelloToSave", modelloToSave);

            // imposta una sessione per mantenere lo stato di "bisogna IMPOSTARE le notifiche"
            bool impostaNotifiche = true;
            Session.Add("impostaNotifiche", impostaNotifiche);
            this.lbl_stato.Text = "Nuovo";

            //this.gestioneTastoNotifiche(false);

            this.RefreshPanel();

            this.Panel_ListaModelli.Visible = false;
            this.UpPnlListaModelli.Update();


        }



        private void btn_lista_modelli_Click(object sender, System.EventArgs e)
        {
            dt_listaModelli.PageIndex = 0;
            this.ModelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.GetInfoUser(), this.prmtRicerca.CreateSearchFilters());
            caricaDataGridModelli();
            pulisciCampi();
            this.btn_salvaModello.Visible = false;
            this.Panel_NuovoModello.Visible = false;
            this.btn_lista_modelli.Visible = false;
            this.Panel_dest.Visible = false;
            this.Panel_ListaModelli.Visible = true;
            this.btn_pp_notifica.Visible = false;
            this.dt_dest.Visible = false;
            this.UpPnlDest.Update();
            this.UpPnlNuovoModello.Update();

            Session.Remove("modelloToSave");

            Session.Remove("impostaNotifiche");

            this.RefreshPanel();

        }

        protected void btn_ricerca_Click(object sender, System.EventArgs e)
        {
            dt_listaModelli.PageIndex = 0;

            this.ModelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.GetInfoUser(), this.prmtRicerca.CreateSearchFilters());
            caricaDataGridModelli();
            this.UpPnlListaModelli.Update();
            if (Panel_ListaModelli.Visible == false)
                btn_lista_modelli_Click(null, null);
            btn_lista_modelli.Visible = true;

            this.dt_dest.Visible = false;
            this.UpPnlDest.Update();
            this.Panel_NuovoModello.Visible = false;
            this.UpPnlNuovoModello.Update();

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            // Recupero dell informazioni sull'utente / amministratore loggato
            InfoUtente userInfo = null;

            userInfo = UserManager.GetInfoUser();


            // Salvataggio della request nel call context
            PrintReportRequestDataset request =
                new PrintReportRequestDataset()
                {
                    ContextName = this.prmtRicerca.SearchContext,
                    SearchFilters = this.prmtRicerca.CreateSearchFilters(),
                    UserInfo = userInfo
                };

            //ReportingUtils.PrintRequest = request;
            RequestPrintReport = request;
            Session["visibleGrdFields"] = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ReportGenerator", "ajaxModalPopupReportGenerator();", true);



        }

        public void tastoRicerca()
        {
            //Utils.DefaultButton(this, ref txt_ricerca, ref btn_ricerca);
        }

        private void txt_nomeModello_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void btn_salvaModello_Click(object sender, System.EventArgs e)
        {
            string msgErr = CheckFields();
            string codModello = string.Empty;
            if (msgErr != null && !msgErr.Equals(String.Empty))
            {
                string msgDesc = "WarningModelliCustom";
                string errFormt = Server.UrlEncode(msgErr);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + utils.FormatJs(errFormt) + "');}; ", true);



            }
            else
            {
                AggiornaModello();


                AggiornaRagioneDest();
                DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

                if (this.NotificheUtDaImpostate() && !this.unicaUOinModello())
                {
                    if (rbl_share.Items[0].Selected)
                    {
                        modello.MITTENTE[0].ID_CORR_GLOBALI = 0;
                        modello.ID_PEOPLE = UserManager.GetInfoUser().idPeople;
                    }
                    else
                    {
                        modello.ID_PEOPLE = "";
                        modello.MITTENTE[0].ID_CORR_GLOBALI = Convert.ToInt32(RoleManager.GetRoleInSession().systemId);// Convert.ToInt32(UserManager.getRuolo() UserManager.getInfoUtente().idCorrGlobali);
                    }
                    Session["Modello"] = modello;

                    Session.Add("mode", "INSERT");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "GestioneNotifiche", "ajaxModalPopupGestioneNotifiche();", true);

                }
                else
                {
                    this.PerformSaveModel();


                }


            }
        }

        private void PerformSaveModel()
        {

            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            if (rbl_share.Items[0].Selected)
            {
                modello.MITTENTE[0].ID_CORR_GLOBALI = 0;
                modello.ID_PEOPLE = UserManager.GetInfoUser().idPeople;
            }
            else
            {
                modello.ID_PEOPLE = "";
                modello.MITTENTE[0].ID_CORR_GLOBALI = Convert.ToInt32(RoleManager.GetRoleInSession().systemId);// Convert.ToInt32(UserManager.getRuolo() UserManager.getInfoUtente().idCorrGlobali);
            }

            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

            TransmissionModelsManager.SaveTemplate(modello, infoUtente);
            Session.Remove("Modello");

            dt_listaModelli.PageIndex = 0;
            this.ModelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.GetInfoUser(), this.prmtRicerca.CreateSearchFilters());
            caricaDataGridModelli();
            pulisciCampi();
            this.btn_salvaModello.Visible = false;
            this.btn_lista_modelli.Visible = false;
            this.Panel_NuovoModello.Visible = false;
            this.Panel_dest.Visible = false;
            this.Panel_ListaModelli.Visible = true;
            this.btn_pp_notifica.Visible = false;
            this.dt_dest.Visible = false;
            dt_listaModelli.SelectedIndex = -1;
            this.UpPnlDest.Update();
            this.RefreshPanel();
            Session.Remove("modelloToSave");            
        }

        private void AggiornaModello()
        {
            //aggiorno i dati generali
            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            modello.ID_AMM = idAmministrazione;
            //userRegistri = UserManager.getListaRegistri(this);
            userRegistri = RoleManager.GetRoleInSession().registri;

            //if (modello.ID_REGISTRO != "0")
            //{
            if (userRegistri.Length > 1)
                modello.ID_REGISTRO = this.ddl_registri.SelectedValue;
            else
                modello.ID_REGISTRO = userRegistri[0].systemId;
            //}

            modello.NOME = this.txt_nomeModello.Text;
            modello.VAR_NOTE_GENERALI = this.txt_noteGenerali.Text;
            modello.CHA_TIPO_OGGETTO = this.ddl_tipoTrasmissione.SelectedValue;
            modello.SINGLE = "0";
            modello.ID_PEOPLE = utente.idPeople;
            modello.CODICE = this.txt_codModello.Text;

            Session.Add("Modello", modello);
        }

        private void dt_listaModelli_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            //int indx = dt_listaModelli.SelectedIndex;

            //string idModello = dt_listaModelli.DataKeys[indx].Values["SYSTEM_ID"].ToString();



            this.RefreshPanel();


        }

        protected void dt_dest_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Select"))
            {
                string indx = e.CommandArgument.ToString();
                string idModello = dt_listaModelli.DataKeys[Int32.Parse(indx)].Values["SYSTEM_ID"].ToString();

                DocsPaWR.ModelloTrasmissione modelloTrasmSel = UserManager.getModelloByID(this, idAmministrazione, idModello);
                Session.Add("Modello", modelloTrasmSel);
                caricaValoriGen();

                //userRegistri = UserManager.getListaRegistri(this);
                userRegistri = RoleManager.GetRoleInSession().registri;
                if (userRegistri.Length > 1 && ddl_registri.SelectedValue == "")
                {
                    if (modelloTrasmSel.ID_REGISTRO != "0")
                    {
                        string msg = "WarningRuoloNoAbilitato";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);

                        return;
                    }
                }
                caricaDataGridDest();
                this.Panel_ListaModelli.Visible = false;
                this.btn_salvaModello.Visible = true;
                this.btn_lista_modelli.Visible = true;
                this.btn_pp_notifica.Visible = true;
                this.Panel_NuovoModello.Visible = true;
                this.Panel_dest.Visible = true;
                this.lbl_codice.Visible = true;
                this.txt_codModello.Visible = true;

                this.UpPnlDest.Update();

                // imposta una sessione per mantenere lo stato di "bisogna IMPOSTARE le notifiche"
                bool impostaNotifiche = true;
                Session.Add("impostaNotifiche", impostaNotifiche);
                this.lbl_stato.Text = "Modifica";
                //this.gestioneTastoNotifiche(true);
            }
        }

        public void caricaValoriGen()
        {
            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            this.txt_nomeModello.Text = modello.NOME;
            this.txt_noteGenerali.Text = modello.VAR_NOTE_GENERALI;
            if (modello.CHA_TIPO_OGGETTO != null && modello.CHA_TIPO_OGGETTO.Equals("D"))
                ddl_tipoTrasmissione.SelectedIndex = 0;
            else
                ddl_tipoTrasmissione.SelectedIndex = 1;

            if (modello.CHA_TIPO_OGGETTO != null && modello.CHA_TIPO_OGGETTO.Equals("D"))
            {
                ddl_tipoTrasmissione.SelectedIndex = 0;
                this.EnableColumnNascondiVersioni(true);
            }
            else
            {
                ddl_tipoTrasmissione.SelectedIndex = 1;
                this.EnableColumnNascondiVersioni(false);
            }


            if (!string.IsNullOrEmpty(modello.ID_PEOPLE))
                rbl_share.SelectedIndex = 0;
            else
                rbl_share.SelectedIndex = 1;

            if (modello.CODICE != null)
            {
                this.txt_codModello.Text = modello.CODICE;
                this.txt_codModello.Enabled = false;
            }

            CaricaRagioni(idAmministrazione, false);

            //userRegistri = UserManager.getListaRegistri(this);
            userRegistri = RoleManager.GetRoleInSession().registri;
            if (userRegistri.Length > 1)
            {
                for (int i = 0; i < ddl_registri.Items.Count; i++)
                {
                    if (ddl_registri.Items[i].Value == modello.ID_REGISTRO)
                    {
                        ddl_registri.SelectedIndex = i;
                        DocsPaWR.Registro reg = new NttDataWA.DocsPaWR.Registro();
                        if (Session["userRegistro"] != null)
                            reg = (NttDataWA.DocsPaWR.Registro)Session["userRegistro"];
                        reg.systemId = this.ddl_registri.SelectedValue.ToString();
                        reg.codRegistro = this.ddl_registri.SelectedItem.Text.ToString();
                        Session.Add("userRegistro", reg);
                    }
                    if (modello.ID_REGISTRO == "0")
                    {
                        ddl_registri.SelectedIndex = 0;
                    }
                }
            }
            else
            {
                if (modello.ID_REGISTRO == "0")
                {

                    this.lbl_registri.Visible = false;
                    this.lbl_registro_obb.Visible = false;
                }
                else
                {
                    this.ddl_registri.Visible = false;
                    Registro reg = RegistryManager.getRegistroBySistemId(modello.ID_REGISTRO);
                    this.lbl_registri.Text = reg.descrizione;
                    this.lbl_registri.Visible = true;
                }
            }
        }



        private void CancellaDestinatario(string idragione, string var_cod_rubrica)
        {
            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPaWR.RagioneDest rd = modello.RAGIONI_DESTINATARI[i];
                for (int j = 0; j < modello.RAGIONI_DESTINATARI[i].DESTINATARI.Length; j++)
                {
                    DocsPaWR.MittDest dest = ((NttDataWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]);

                    if (dest.ID_RAGIONE.ToString().Equals(idragione) && dest.VAR_COD_RUBRICA.Equals(var_cod_rubrica))
                    {

                        if (rd.DESTINATARI.Length > 1)
                        {
                            ArrayList appoggio = new ArrayList(rd.DESTINATARI);
                            appoggio.RemoveAt(j);
                            rd.DESTINATARI = new NttDataWA.DocsPaWR.MittDest[appoggio.Count];
                            appoggio.CopyTo(rd.DESTINATARI);
                            Session.Add("Modello", modello);
                            return;
                        }
                        else
                        {
                            ArrayList appoggio = new ArrayList(modello.RAGIONI_DESTINATARI);
                            appoggio.RemoveAt(i);
                            modello.RAGIONI_DESTINATARI = new NttDataWA.DocsPaWR.RagioneDest[appoggio.Count];
                            appoggio.CopyTo(modello.RAGIONI_DESTINATARI);
                            Session.Add("Modello", modello);
                            return;
                        }
                    }
                }
            }

        }



        private void pulisciModello()
        {
            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            modello.RAGIONI_DESTINATARI = null;
            //userRegistri = UserManager.getListaRegistri(this);
            userRegistri = RoleManager.GetRoleInSession().registri;
            if (userRegistri.Length > 1)
                modello.ID_REGISTRO = this.ddl_registri.SelectedValue;
            else
            {

                Registro reg = RegistryManager.getRegistroBySistemId(modello.ID_REGISTRO);
                this.lbl_registri.Text = reg.descrizione;
                this.lbl_registri.Visible = true;
                this.ddl_registri.Visible = false;
            }
            Session.Add("Modello", modello);
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.btn_lista_modelli.Text = Utils.Languages.GetLabelFromCode("ModelliBtnList", language);
            this.btn_nuovoModello.Text = Utils.Languages.GetLabelFromCode("ModelliBtnNew", language);
            this.btn_salvaModello.Text = Utils.Languages.GetLabelFromCode("ModelliBtnSave", language);
            this.GestioneModelliTrasmissione.Text = Utils.Languages.GetLabelFromCode("BaseMasterModelliTrasm", language);
            this.lbl_titolo.Text = Utils.Languages.GetLabelFromCode("ModelsTitolo", language);
            this.GestioneModelliTrasmissione.Text = Utils.Languages.GetLabelFromCode("GestioneModelliTrasmissione", language);
            this.ibtnMoveToA.ToolTip = Utils.Languages.GetLabelFromCode("imgSrcDestinatari", language);
            this.ibtnMoveToA.AlternateText = Utils.Languages.GetLabelFromCode("imgSrcDestinatari", language);
            this.btn_Rubrica_dest.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.btn_Rubrica_dest.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.FindAndReplace.Title = Utils.Languages.GetLabelFromCode("FindAndReplace", language);
            this.AddressBook.Title = Utils.Languages.GetLabelFromCode("AddressBookTitle", language);
            this.btnFindAndReplace.Text = Utils.Languages.GetLabelFromCode("ModelliBtnFindAndReplace", language);
            this.btnFind.Text = Utils.Languages.GetLabelFromCode("ModelliBtnFind", language);
            this.btnExport.Text = Utils.Languages.GetLabelFromCode("ModelliBtnExport", language);
            this.ReportGenerator.Title = Utils.Languages.GetLabelFromCode("ReportGeneratorLblReportGenerator", language);
            this.GestioneNotifiche.Title = Utils.Languages.GetLabelFromCode("GestioneModelliTrasm_Notifiche", language);
            this.ddl_ragioni.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));
            this.btn_pp_notifica.Text = Utils.Languages.GetLabelFromCode("btn_pp_notifica", language);
            this.Ltrtxt_noteGenerali.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language) + " ";
        }


        protected void btnAddressBookPostback_Click(object sender, EventArgs e)
        {
            try
            {
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> atList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.At"];
                List<NttDataWA.Popup.AddressBook.CorrespondentDetail> ccList = (List<NttDataWA.Popup.AddressBook.CorrespondentDetail>)HttpContext.Current.Session["AddressBook.Cc"];
                string addressBookCallFrom = HttpContext.Current.Session["AddressBook.from"].ToString();

                switch (addressBookCallFrom)
                {
                    case "F_X_X_S":
                        if (atList != null && atList.Count > 0)
                        {
                            NttDataWA.Popup.AddressBook.CorrespondentDetail corrInSess = atList[0];
                            Corrispondente tempCorrSingle;
                            if (!corrInSess.isRubricaComune)
                                tempCorrSingle = UIManager.AddressBookManager.GetCorrespondentBySystemId(atList[0].SystemID);
                            else
                                tempCorrSingle = UIManager.AddressBookManager.getCorrispondenteByCodRubricaRubricaComune(corrInSess.CodiceRubrica);

                            this.prmtRicerca.destCodice = tempCorrSingle.codiceRubrica;
                            this.prmtRicerca.destDescrizione = tempCorrSingle.descrizione;
                            this.prmtRicerca.idDestinatario = tempCorrSingle.systemId;
                            this.prmtRicerca.RefreshCorrespondent();
                            this.UpPnlSearchModel.Update();
                        }
                        break;
                    case "G_M_T_I":

                        if (atList != null && atList.Count > 0)
                        {
                            foreach (NttDataWA.Popup.AddressBook.CorrespondentDetail addressBookCorrespondent in atList)
                            {

                                this.txt_codDest.Text = addressBookCorrespondent.CodiceRubrica;
                                this.ibtnMoveToA_Click(null, null);
                            }
                            this.dt_dest.Visible = true;
                            this.UpPnlDest.Update();

                        }
                        break;

                }
                HttpContext.Current.Session["AddressBook.At"] = null;
                HttpContext.Current.Session["AddressBook.Cc"] = null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        #region Notifiche trasmissione e cessione diritti

        /// <summary>
        /// Tasto Notifiche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_pp_notifica_Click(object sender, EventArgs e)
        {
            string jscript = string.Empty;
            bool modelloToSave = false;

            if (Session["modelloToSave"] != null)
                modelloToSave = (bool)Session["modelloToSave"];

            if (modelloToSave)
            {
                string msgDesc = "AlertModelsNotify";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);


            }
            else
            {

                Session.Add("mode", "UPDATE");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ControlloGestioneNotifiche", "ajaxModalPopupGestioneNotifiche();", true);
            }
        }

        ///// <summary>
        ///// Imposta la visibilità e testo del tasto notifiche
        ///// </summary>
        ///// <param name="stato"></param>
        //private void gestioneTastoNotifiche(bool stato)
        //{
        //    string testo = "Gestione notifiche";
        //    this.btn_pp_notifica.Visible = stato;

        //    if (stato)
        //        if (this.checkUserAutorizedEditingACL())
        //            this.btn_pp_notifica.Text = testo + " e Cessione diritti";
        //        else
        //            this.btn_pp_notifica.Text = testo;
        //}

        /// <summary>
        /// GESTIONE CESSIONE DIRITTI:
        /// verifica se l'utente è abilitato alla funzione ABILITA_CEDI_DIRITTI_DOC / ABILITA_CEDI_DIRITTI_FASC
        /// </summary>
        private bool checkUserAutorizedEditingACL()
        {
            string funzione = string.Empty;
            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            if (modello.CHA_TIPO_OGGETTO.Equals("D"))
                funzione = "ABILITA_CEDI_DIRITTI_DOC";
            if (modello.CHA_TIPO_OGGETTO.Equals("F"))
                funzione = "ABILITA_CEDI_DIRITTI_FASC";

            return UserManager.IsAuthorizedFunctions(funzione);
        }

        private bool NotificheUtDaImpostate()
        {
            bool retValue = false;

            if (Session["impostaNotifiche"] != null)
                retValue = (bool)Session["impostaNotifiche"];

            return retValue;
        }
        #endregion

        private string checkRuoliUtentiDuplicati()
        {
            string msg = string.Empty;
            int quanti;

            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {
                foreach (DocsPaWR.MittDest mittDest in ragDest.DESTINATARI)
                {
                    quanti = this.contaDest_perRicercaDuplicati(modello, mittDest.ID_CORR_GLOBALI);
                    if (mittDest.ID_CORR_GLOBALI != 0)
                    {
                        if (quanti > 1)
                            msg += "<br/>- " + mittDest.DESCRIZIONE + " (" + ragDest.RAGIONE + ")";
                    }
                }
            }

            return msg;
        }

        private int contaDest_perRicercaDuplicati(DocsPaWR.ModelloTrasmissione modello, int idCorrGlob)
        {
            int quanti = 0;

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
                foreach (DocsPaWR.MittDest mittDest in ragDest.DESTINATARI)
                    if (mittDest.ID_CORR_GLOBALI.Equals(idCorrGlob))
                        quanti++;

            return quanti;
        }

        private string checkRagTrasmConCessioneDuplicate()
        {
            string msg = string.Empty;
            int contaRagConCessione = 0;

            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            DocsPaWR.OrgRagioneTrasmissione ragione = null;
            DocsPaWR.MittDest mittDest = null;

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {
                mittDest = ragDest.DESTINATARI[0];

                ragione = TrasmManager.GetRagioneTrasmissione(Convert.ToString(mittDest.ID_RAGIONE));
                if (ragione != null)
                {
                    if (!ragione.PrevedeCessione.Equals(NttDataWA.DocsPaWR.CedeDiritiEnum.No))
                    {
                        if (contaRagConCessione > 0)
                        {
                            msg += "<br/>- " + ragDest.RAGIONE;
                        }
                        else
                        {
                            contaRagConCessione++;
                            msg += "<br/>- " + ragDest.RAGIONE;
                        }
                    }
                }
            }

            if (contaRagConCessione <= 1)
                msg = string.Empty;

            return msg;
        }

        private string checkModUOConCessione()
        {
            string msg = string.Empty;

            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            DocsPaWR.OrgRagioneTrasmissione ragione = null;
            DocsPaWR.MittDest mittDest = null;

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {
                mittDest = ragDest.DESTINATARI[0];
                if (mittDest.CHA_TIPO_URP.Equals("U")) // se è una UO allora verifica se la ragione è CESSIONE
                {
                    ragione = TrasmManager.GetRagioneTrasmissione(Convert.ToString(mittDest.ID_RAGIONE));
                    if (ragione != null)
                    {
                        if (!ragione.PrevedeCessione.Equals(NttDataWA.DocsPaWR.CedeDiritiEnum.No))
                        {
                            msg += "<br/>- " + mittDest.DESCRIZIONE;
                        }
                    }
                }
            }

            return msg;
        }



        private bool unicaUOinModello()
        {
            bool unicaUO = true;
            bool retValue = false;
            int contaUO = 0;
            int contaAltro = 0;

            DocsPaWR.MittDest mittDest = null;
            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            //DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {

                if (unicaUO)
                {
                    for (int i = 0; i < ragDest.DESTINATARI.Length; i++)
                    {
                        mittDest = ragDest.DESTINATARI[i];
                        if (mittDest.CHA_TIPO_MITT_DEST == "D")
                        {
                            if (mittDest.CHA_TIPO_URP.Equals("U"))
                            { // se è una UO allora verifica se la ragione è CESSIONE

                                contaUO++;
                            }
                            else
                            {// Aggiunto perchè se c'è solo un utente per quel ruolo, viene settata automaticamente la notifica
                                if (mittDest.CHA_TIPO_URP == "R")
                                {
                                    if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length == 1)
                                    {
                                        mittDest.UTENTI_NOTIFICA[0].FLAG_NOTIFICA = "1";
                                        contaUO++;
                                    }
                                    else
                                    {
                                        contaAltro++;
                                        unicaUO = false;
                                        break;
                                    }
                                }

                                // Aggiunto perchè se c'è solo un utente per quel ruolo, viene settata automaticamente la notifica
                                if (mittDest.CHA_TIPO_URP == "P")
                                {
                                    if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length == 1)
                                    {
                                        mittDest.UTENTI_NOTIFICA[0].FLAG_NOTIFICA = "1";
                                        contaUO++;
                                    }
                                    else
                                    {
                                        contaAltro++;
                                        unicaUO = false;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            contaUO++;
                        }
                    }
                }


            }



            if (contaUO > 0 && contaAltro.Equals(0))
                retValue = true;
            return retValue;
        }


        public bool destinatariDaRubrica()
        {
            bool result = false;
            DocsPaWR.ModelloTrasmissione modello = (NttDataWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            DocsPaWR.MittDest mittDest = null;
            bool destinatarioDaRubrica = false;

            if (!destinatarioDaRubrica)
            {
                foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
                {
                    for (int i = 0; i < ragDest.DESTINATARI.Length; i++)
                    {
                        mittDest = ragDest.DESTINATARI[i];
                        if (mittDest.CHA_TIPO_MITT_DEST == "D")
                        {
                            result = true;
                            destinatarioDaRubrica = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        protected void btn_Rubrica_dest_Click(object sender, ImageClickEventArgs e)
        {
            //userRegistri = UserManager.getListaRegistri(this);
            userRegistri = RoleManager.GetRoleInSession().registri;
            Session["noRagioneTrasmNullaFE"] = true;
            if (string.IsNullOrEmpty(ddl_ragioni.SelectedItem.Text))
            {
                string msg = "WarningNoRagioneTrasmissione";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            }
            else
            {
                Session["CompileDest"] = true;
                //this.CallType = NttDataWA.DocsPaWR.RubricaCallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI;
                DocsPaWR.RagioneTrasmissione rt = TrasmManager.getRagioneSel(this);
                this.gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0, 1);
                switch (gerarchia_trasm)
                {
                    case "T":
                        this.CallType = NttDataWA.DocsPaWR.RubricaCallType.CALLTYPE_MODELLI_TRASM_ALL;
                        break;

                    case "I":
                        this.CallType = NttDataWA.DocsPaWR.RubricaCallType.CALLTYPE_MODELLI_TRASM_INF;
                        break;

                    case "S":
                        this.CallType = NttDataWA.DocsPaWR.RubricaCallType.CALLTYPE_MODELLI_TRASM_SUP;
                        break;

                    case "P":
                        this.CallType = NttDataWA.DocsPaWR.RubricaCallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO;
                        break;
                }
                HttpContext.Current.Session["AddressBook.from"] = "G_M_T_I";

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);


            }

        }

        public RubricaCallType CallType
        {
            get
            {
                if (HttpContext.Current.Session["callType"] != null)
                    return (RubricaCallType)HttpContext.Current.Session["callType"];
                else return RubricaCallType.CALLTYPE_PROTO_IN;
            }
            set
            {
                HttpContext.Current.Session["callType"] = value;
            }

        }

        public ArrayList ModelliTrasmissione
        {
            get
            {
                if (HttpContext.Current.Session["modelliTrasmissione"] != null)
                    return (ArrayList)HttpContext.Current.Session["modelliTrasmissione"];
                else return new ArrayList();
            }
            set
            {
                HttpContext.Current.Session["modelliTrasmissione"] = value;
            }

        }




        protected void dt_listaModelli_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int indx = e.RowIndex;
                string idModello = dt_listaModelli.DataKeys[indx].Values["SYSTEM_ID"].ToString();
                Session["DeleteModel"] = idModello;

                string msgConfirm = "ConfirmDeleteModels";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'deleteMod');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'deleteMod');}", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }




        }

        /// <summary>
        /// Metodo per la gestione del ret value da popup
        /// </summary>
        protected void ReadRetValueFromPopup()
        {
            if (this.deleteMod.Value == "true")
            {
                string idModello = Convert.ToString(Session["DeleteModel"]);
                UserManager.cancellaModello(this, idAmministrazione, idModello);
                Session["DeleteModel"] = null;
                this.deleteMod.Value = string.Empty;

                this.ModelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.GetInfoUser(), this.prmtRicerca.CreateSearchFilters());
                caricaDataGridModelli();

                this.RefreshPanel();
            }

            if (this.deleteDest.Value == "true")
            {
                string idragione = Convert.ToString(Session["DeleteidRagione"]);
                string var_cod_rubrica = Convert.ToString(Session["DeleteVarCorRubrica"]);
                Session["DeleteidRagione"] = null;
                Session["DeleteVarCorRubrica"] = null;
                this.deleteDest.Value = string.Empty;


                if (var_cod_rubrica.Equals("&nbsp;"))
                    var_cod_rubrica = var_cod_rubrica.Replace("&nbsp;", "");
                CancellaDestinatario(idragione, var_cod_rubrica);
                caricaDataGridDest();

                this.RefreshPanel();
            }

            if (!string.IsNullOrEmpty(this.GestioneNotifiche.ReturnValue))
            {
                btn_lista_modelli_Click(null, null);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('GestioneNotifiche','');", true);
            }
        }

        protected void dt_dest_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int indx = e.RowIndex;

            string idragione = dt_dest.DataKeys[indx].Values["ID_RAGIONE"].ToString();
            string var_cod_rubrica = dt_dest.DataKeys[indx].Values["VAR_COD_RUBRICA"].ToString();

            // Se cerchi di cancellare un destinatario cliccando sul bidoncino non succede nulla perché il codice
            // che arriva contiene anche i tag html span di formattazione.
            XElement elem = XElement.Parse(var_cod_rubrica);
            var_cod_rubrica = elem.Value;

            Session["DeleteidRagione"] = idragione;
            Session["DeleteVarCorRubrica"] = var_cod_rubrica;

            string msgConfirm = "ConfirmDeleteDestinatario";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal", "if (parent.fra_main) {parent.fra_main.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'deleteDest');} else {parent.ajaxConfirmModal('" + msgConfirm.Replace("'", @"\'") + "', 'deleteDest');}", true);
        }

        protected void dt_listaModelli_PageIndexChanged(object sender, EventArgs e)
        {

            this.ModelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.GetInfoUser(), this.prmtRicerca.CreateSearchFilters());
            caricaDataGridModelli();
        }

        protected void dt_listaModelli_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            dt_listaModelli.PageIndex = e.NewPageIndex;
            //modelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.GetInfoUser(), this.prmtRicerca.CreateSearchFilters());
            //caricaDataGridModelli();
        }

        protected void btnFindAndReplace_Click(object sender, EventArgs e)
        {
            Session.Add("ClickFindAR", "click");

            // Salvataggio filtri di ricerca nel context
            ModelliTrasmManager.SearchFilters = this.prmtRicerca.CreateSearchFilters();


            ScriptManager.RegisterStartupScript(this, this.GetType(), "FindAndReplace", "ajaxModalPopupFindAndReplace();", true);


        }
    }
}
