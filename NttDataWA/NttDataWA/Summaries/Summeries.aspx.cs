using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System.Collections;
using NttDataWA.Utils;
using System.Globalization;
using NttDataWA.UIManager.SummariesManager;
using NttDatalLibrary;
using NttDataWA.UIManager.SummeriesManager;

namespace NttDataWA.Summaries
{
    public partial class Summeries : System.Web.UI.Page
    {
        protected ArrayList filtri = new ArrayList();
        NttDataWA.DocsPaWR.Utente _utente = new NttDataWA.DocsPaWR.Utente();
        #region Fields

        public static string componentType = Constans.TYPE_SMARTCLIENT;

        #endregion

        private void InitApplet()
        {

            switch (componentType)
            {
                case Constans.TYPE_APPLET:
                    this.plcApplet.Visible = true;
                    break;
                case Constans.TYPE_SOCKET:
                    break;
                case Constans.TYPE_ACTIVEX:
                case Constans.TYPE_SMARTCLIENT:

                    Control ShellWrapper = Page.LoadControl("../ActivexWrappers/ShellWrapper.ascx");
                    this.plcActiveX.Controls.Add(ShellWrapper);

                    Control AdoStreamWrapper = Page.LoadControl("../ActivexWrappers/AdoStreamWrapper.ascx");
                    this.plcActiveX.Controls.Add(AdoStreamWrapper);

                    Control FsoWrapper = Page.LoadControl("../ActivexWrappers/FsoWrapper.ascx");
                    this.plcActiveX.Controls.Add(FsoWrapper);

                    this.plcActiveX.Visible = true;
                    break;
                default:
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            componentType = UserManager.getComponentType(Request.UserAgent);
            this.InitApplet();

            if (!this.IsPostBack)
            {


                #region Caricamenti ereditati dalla vecchia versione
                FileManager.RemoveFileReport(this);
                setRadioButtonList();
                SetDInitialDateTime();

                _utente = UserManager.GetUserInSession();
                if (_utente != null)
                {
                    this.DO_SetControlFromDocsPA(_utente);

                    //NUOVI REPORT FASCICOLI
                    //if (DocsPAWA.UserManager.ruoloIsAutorized(this, "EXP_FASC_COUNT"))//OLD
                    if (UserManager.IsAuthorizedFunctions("EXP_FASC_COUNT"))
                    {
                        caricaComboRf();
                        caricaComboTitolari();
                    }
                }
                else
                {
                    //Caricamento delle amministrazionei
                    setDdlAmministrazioni();
                }

                //Caricamento stampa protocollo arma
                setStampaProtocolloArma();

                //titolario per report annuale fascicolo per voce di titolario
                caricaComboTitolariReportTitolario();

                ////Laura Mev Obiettivi
                //CaricaComboTipoComunicazione();

                #endregion

                this.InitializePage();
                //this.InitializeLanguage();
                this.SummeriesZoom.Enabled = false;

                //this.PopulateDDLRegistry(this.Role);
                //CaricaTipologia(this.DocumentDdlTypeDocument);
                //this.LoadGridVisibility(null, 0);
            }
            else
            {
                //this.ReadRetValueFromPopup();
            }

            //this.RefreshScript();
            this.RefreshScripts();
        }

        private void SetDInitialDateTime()
        {

            this.cld_chiusura_al.Visible = false;
            this.lbl_al_chiusura.Visible = false;
            this.lbl_dal_chiusura.Visible = false;
            this.cld_chiusura_dal.Enabled = true;
            this.cld_creazione_al.Visible = false;
            this.lbl_al_apertura.Visible = false;
            this.lbl_dal_apertura.Visible = false;
            this.cld_creazione_dal.Enabled = true;
        }

        private void RefreshScripts()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resizeIframe", "resizeIframe();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }
        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }
        //protected void DataChangedHandler(object sender, System.EventArgs e)
        //{
        //    this.IsModified = true;
        //}
        private bool IsModified
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsModified"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["IsModified"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["IsModified"] = value;
            }
        }

        private void setStampaProtocolloArma()
        {
            //if (!string.IsNullOrEmpty(wws.isEnableProtocolloTitolario()))//OLD

            //if (!string.IsNullOrEmpty(ProspettiRiepilogativi.Frontend.Controller.isEnableProtocolloTitolario()))
            //{
            NttDataWA.UIManager.SummeriesManager.Controller.DO_GetTitolario(ref this.ddl_et_titolario, ddl_amm.SelectedValue);
            if (UserManager.GetUserInSession() == null)
                NttDataWA.UIManager.SummeriesManager.Controller.DO_GetRegistriCCByAmministrazione(ddl_regCC, ddl_amm.SelectedValue);
            else
                caricaDdlRegCC(ddl_regCC);
            //}
        }
        private void caricaDdlRegCC(DropDownList ddlRegCC)
        {
            ddlRegCC.Items.Clear();
            //ddlRegCC.Items.Add(new ListItem("Tutti", ""));
            for (int i = 0; i < _utente.ruoli.Length; i++)
            {
                NttDataWA.DocsPaWR.Ruolo ruolo = (NttDataWA.DocsPaWR.Ruolo)_utente.ruoli[i];
                for (int j = 0; j < ruolo.registri.Length; j++)
                {
                    NttDataWA.DocsPaWR.Registro reg = (NttDataWA.DocsPaWR.Registro)ruolo.registri[j];
                    if (!DO_VerifyList(ddlRegCC, reg.systemId))
                    {
                        ddlRegCC.Items.Add(new ListItem(reg.descrizione, reg.systemId));
                    }
                    //ddl_registro.SelectedIndex = 1;
                }
            }
        }




        protected void rb_scelta_sott_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string _value = rb_scelta_sott.SelectedValue;
            if (_value.Equals("UO"))
            {
                this.pnl_scelta_uo.Visible = true;
                this.pnl_scelta_rf.Visible = false;
                this.ImgProprietarioAddressBook.Visible = true;
            }
            else
            {
                this.pnl_scelta_uo.Visible = false;
                this.pnl_scelta_rf.Visible = true;
                this.ImgProprietarioAddressBook.Visible = false;
            }
        }

        private void caricaComboRf()
        {
            this.ddl_rf.Items.Clear();

            NttDataWA.DocsPaWR.OrgRegistro[] listaTotale = null;
            //voglio la lista dei soli RF, quindi al webMethod passero come chaRF il valore 1 (solo RF)

            // NttDataWA.DocsPaWR.Corrispondente cr = (NttDataWA.DocsPaWR.Corrispondente)this.Session["userData"];//OLD ADP
            DocsPaWR.Corrispondente cr = UserManager.getCorrispondenteByIdPeople(this, UserManager.GetUserInSession().idPeople, DocsPaWR.AddressbookTipoUtente.INTERNO);//DA VERIFICARE !


            NttDataWA.DocsPaWR.PR_Amministrazione amm = NttDataWA.UIManager.SummeriesManager.Controller.Do_GetAmmByIdAmm(Convert.ToInt32(cr.idAmministrazione));

            listaTotale = UIManager.AdministrationManager.AmmGetRegistri(amm.Codice, "1");   //OLD wws.AmmGetRegistri(amm.Codice, "1");

            if (listaTotale != null && listaTotale.Length > 0)
            {
                int y = 0;
                for (int i = 0; i < listaTotale.Length; i++)
                {
                    string testo = listaTotale[i].Codice;
                    this.ddl_rf.Items.Add(testo);
                    this.ddl_rf.Items[y].Value = listaTotale[i].IDRegistro;
                    y++;
                }

                NttDataWA.DocsPaWR.Registro rf_reg = UIManager.RegistryManager.getRegistroBySistemId(ddl_rf.SelectedItem.Value); //OLD wws.GetRegistroBySistemId(ddl_rf.SelectedItem.Value);
            }

            NttDataWA.DocsPaWR.Registro[] registriRf = null;
            //NttDataWA.DocsPaWR.Ruolo ruoloUtente = UIManager.RoleManager.GetRoleInSession();//OLD   DocsPAWA.UserManager.getRuolo();
            //registriRf = UIManager.RegistryManager.getListaRegistriWithRF(ruoloUtente.systemId, "1"); //OLD DocsPAWA.UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "1", "");
            registriRf = RegistryManager.GetRFListInSession();
            if (registriRf != null && registriRf.Length > 0)
            {
                this.ddl_rf.SelectedValue = registriRf[0].systemId;
            }
        }

        private void caricaComboTitolari()
        {
            ddl_titolari.Items.Clear();
            //ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(DocsPAWA.UserManager.getUtente(this).idAmministrazione));// OLD
            ArrayList listaTitolari = new ArrayList(ClassificationSchemeManager.getTitolariUtilizzabili());
            //Esistono dei titolari chiusi
            if (listaTitolari.Count > 1)
            {
                //Creo le voci della ddl dei titolari
                string valueTutti = string.Empty;
                foreach (NttDataWA.DocsPaWR.OrgTitolario titolario in listaTitolari)
                {
                    ListItem it = null;
                    switch (titolario.Stato)
                    {
                        case NttDataWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                        case NttDataWA.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
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
                    if (valueTutti.IndexOf(',') == -1)
                        valueTutti = valueTutti + "," + valueTutti;

                    ListItem it = new ListItem("Tutti i titolari", valueTutti);
                    ddl_titolari.Items.Insert(0, it);
                }

            }

            //Non esistono titolario chiusi
            if (listaTitolari.Count == 1)
            {
                NttDataWA.DocsPaWR.OrgTitolario titolario = (NttDataWA.DocsPaWR.OrgTitolario)listaTitolari[0];
                if (titolario.Stato != NttDataWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    ddl_titolari.Items.Add(it);
                }
                ddl_titolari.Enabled = false;
            }
        }

        private void DO_SetFormControl()
        {
            ArrayList filtri = new ArrayList();
            try
            {
                ddl_prospettiRiepilogativi.SelectedIndex = 0;
                //elisa 09-01-2006
                //if(ddl_amm.Items.Count == 2)
                if (ddl_amm.Items.Count >= 2)
                {
                    ddl_amm.SelectedIndex = 1;
                    string amm = ddl_amm.SelectedValue;

                    UIManager.SummeriesManager.Parametro _amm = new UIManager.SummeriesManager.Parametro("amm", ddl_amm.SelectedItem.Text, ddl_amm.SelectedItem.Value);
                    filtri.Add(_amm);

                    NttDataWA.UIManager.SummeriesManager.Controller.DO_GetRegistriByAmministrazione(ddl_registro, amm);
                    //utility.SetFocus(this, ddl_registro);
                }
                //if(ddl_registro.Items.Count == 2)
                if (ddl_registro.Items.Count >= 2)
                {
                    ddl_registro.SelectedIndex = 1;
                    int idReg = Convert.ToInt32(ddl_registro.SelectedValue);

                    UIManager.SummeriesManager.Parametro reg = new UIManager.SummeriesManager.Parametro("reg", ddl_registro.SelectedItem.Text, ddl_registro.SelectedItem.Value);
                    filtri.Add(reg);

                    ddl_anno.Items.Clear(); //svuoto la ddl_anno per evitare ripetizioni
                    NttDataWA.UIManager.SummeriesManager.Controller.DO_GetAnniProfilazione(ddl_anno, idReg);
                    // verifico presenza Sedi, se presenti abilito la combobox e aggiungo
                    //il filtro di ricerca.
                    ddl_sede.Items.Clear(); // svuoto la ddl per evitare ripetizioni
                    bool popolata = false;
                    NttDataWA.UIManager.SummeriesManager.Controller.DO_GetSedi(Convert.ToInt32(NttDataWA.UIManager.SummeriesManager.Controller.DO_GetIdAmmByCodAmm(ddl_amm.SelectedValue)), ddl_sede, out popolata);
                    if (popolata)
                    {
                        // Gabriele Melini 08-05-2014
                        // se è selezionato "Tutte le sedi", il valore del parametro deve essere una stringa vuota
                        UIManager.SummeriesManager.Parametro _sede = null;
                        if(ddl_sede.SelectedIndex == 0)
                            _sede = new UIManager.SummeriesManager.Parametro("sede", ddl_sede.SelectedItem.Text, string.Empty);
                        else
                            _sede = new UIManager.SummeriesManager.Parametro("sede", ddl_sede.SelectedItem.Text, ddl_sede.SelectedItem.Value);
                        filtri.Add(_sede);

                        utility.SetFocus(this, ddl_sede);
                    }
                    else
                    {
                        utility.SetFocus(this, ddl_anno);
                    }
                }
                //if(ddl_anno.Items.Count == 2)
                if (ddl_anno.Items.Count >= 2)
                {
                    ddl_anno.SelectedIndex = 1;

                    UIManager.SummeriesManager.Parametro _anno = new UIManager.SummeriesManager.Parametro("anno", ddl_anno.SelectedItem.Text, ddl_anno.SelectedItem.Value);
                    filtri.Add(_anno);

                    //utility.SetFocus(this, btnStampa);//OLD ADP
                }
                Session.Add("filtri", filtri);
                Session.Add("SetFormControl", true);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        private void DO_SetControlFromDocsPA(NttDataWA.DocsPaWR.Utente _utente)
        {
            try
            {
                //Selezioniamo il primo report
                ddl_prospettiRiepilogativi.SelectedIndex = 0;

                NttDataWA.DocsPaWR.PR_Amministrazione amm = NttDataWA.UIManager.SummeriesManager.Controller.Do_GetAmmByIdAmm(Convert.ToInt32(_utente.idAmministrazione));
                ddl_amm.Items.Clear();
                ddl_amm.Items.Add("");
                ddl_amm.Items.Add(new ListItem(amm.Descrizione, amm.Codice));

                ddl_amm.SelectedIndex = 1;
                ddl_amm.Enabled = false;

                UIManager.SummeriesManager.Parametro _amm = new UIManager.SummeriesManager.Parametro("amm", ddl_amm.SelectedItem.Text, ddl_amm.SelectedItem.Value);
                filtri.Add(_amm);

                /*
                * Dobbiamo usare il registro dell'utente 
                */
                ddl_registro.Items.Clear();
                ddl_registro.Items.Add("");
                for (int i = 0; i < _utente.ruoli.Length; i++)
                {
                    NttDataWA.DocsPaWR.Ruolo ruolo = (NttDataWA.DocsPaWR.Ruolo)_utente.ruoli[i];
                    for (int j = 0; j < ruolo.registri.Length; j++)
                    {
                        NttDataWA.DocsPaWR.Registro reg = (NttDataWA.DocsPaWR.Registro)ruolo.registri[j];
                        if (!DO_VerifyList(ddl_registro, reg.systemId))
                        {
                            ddl_registro.Items.Add(new ListItem(reg.descrizione, reg.systemId));
                        }
                        //ddl_registro.SelectedIndex = 1;
                    }
                }
                //elisa 04/01/2006
                //if(ddl_registro.Items.Count == 2)
                if (ddl_registro.Items.Count >= 2)
                {
                    ddl_registro.SelectedIndex = 1;
                    int idReg = Convert.ToInt32(ddl_registro.SelectedValue);

                    UIManager.SummeriesManager.Parametro reg = new UIManager.SummeriesManager.Parametro("reg", ddl_registro.SelectedItem.Text, ddl_registro.SelectedItem.Value);
                    filtri.Add(reg);

                    ddl_anno.Items.Clear(); //svuoto la ddl_anno per evitare ripetizioni
                    NttDataWA.UIManager.SummeriesManager.Controller.DO_GetAnniProfilazione(ddl_anno, idReg);
                    // verifico presenza Sedi, se presenti abilito la combobox e aggiungo
                    //il filtro di ricerca.
                    bool popolata = false;
                    ddl_sede.Items.Clear(); //svuoto la ddl_sede per evitare ripetizioni
                    NttDataWA.UIManager.SummeriesManager.Controller.DO_GetSedi(Convert.ToInt32(NttDataWA.UIManager.SummeriesManager.Controller.DO_GetIdAmmByCodAmm(ddl_amm.SelectedValue)), ddl_sede, out popolata);
                    if (popolata)
                    {
                        //						if(ddl_sede.SelectedIndex != 0)
                        //						{
                        // Gabriele Melini 08-05-2014
                        // se è selezionato "Tutte le sedi", il valore del parametro deve essere una stringa vuota
                        UIManager.SummeriesManager.Parametro _sede = null;
                        if(ddl_sede.SelectedIndex == 0)
                            _sede = new UIManager.SummeriesManager.Parametro("sede", ddl_sede.SelectedItem.Text, string.Empty);
                        else
                            _sede = new UIManager.SummeriesManager.Parametro("sede", ddl_sede.SelectedItem.Text, ddl_sede.SelectedItem.Value);

                        filtri.Add(_sede);
                        //						}
                    }
                    else
                    {
                        utility.SetFocus(this, ddl_anno);
                    }
                }
                if (ddl_anno.Items.Count > 1)
                {
                    ddl_anno.SelectedIndex = ddl_anno.Items.Count - 1;

                    UIManager.SummeriesManager.Parametro _anno = new UIManager.SummeriesManager.Parametro("anno", ddl_anno.SelectedItem.Text, ddl_anno.SelectedItem.Value);
                    filtri.Add(_anno);

                    //utility.SetFocus(this, btnStampa);//ADP old
                }

                Controller.DO_GETMesi(ddl_Mese);
                Session.Add("filtri", filtri);
                Session.Add("SetFormControl", true);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        private void setDdlAmministrazioni()
        {
            ddl_amm.Items.Add("");
            try
            {
                Controller.DO_GetAmministrazioni(ddl_amm);
                utility.SetFocus(this, ddl_amm);
                DO_SetFormControl();
            }

            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            if (Session["filtri"] != null)
            {
                if (Session["SetFormControl"] == null)
                {
                    Session.Remove("filtri");
                }
            }
        }

        private void setRadioButtonList()
        {
            ArrayList reports = NttDataWA.UIManager.SummeriesManager.Controller.DO_ReadXML();

            ddl_prospettiRiepilogativi.Items.Clear();
            foreach (NttDataWA.DocsPaWR.Report r in reports)
            {
                if (r.Valore == "stampaProtArma" || r.Valore == "stampaDettaglioPratica" || r.Valore == "stampaGiornaleRiscontri" || r.Valore == "documentiSpediti")
                {
                    //if (!string.IsNullOrEmpty(wws.isEnableProtocolloTitolario()))//OLD

                    //if (!string.IsNullOrEmpty(ProspettiRiepilogativi.Frontend.Controller.isEnableProtocolloTitolario()))
                    //    ddl_prospettiRiepilogativi.Items.Add(new ListItem(r.Descrizione, r.Valore));
                }
                else
                {
                    if ((r.Valore == "reportNumFasc" || r.Valore == "reportNumDocInFasc"))
                    {
                        //if (DocsPAWA.UserManager.ruoloIsAutorized(this, "EXP_FASC_COUNT"))//OLD
                        if (UserManager.IsAuthorizedFunctions("EXP_FASC_COUNT"))
                        {
                            ddl_prospettiRiepilogativi.Items.Add(new ListItem(r.Descrizione, r.Valore));
                        }
                    }
                    else
                    {
                        //Laura 19 Dicembre
                        if (r.Valore == "reportDocInMisOb")
                        {
                            //UserManager.IsAuthorizedFunctions(
                            //if (DocsPAWA.UserManager.ruoloIsAutorized(this, "EXP_DOC_MIS_OB"))//OLD
                            if (UserManager.IsAuthorizedFunctions("EXP_DOC_MIS_OB"))
                            {
                                ddl_prospettiRiepilogativi.Items.Add(new ListItem(r.Descrizione, r.Valore));
                            }
                        }
                        else
                        {
                            ddl_prospettiRiepilogativi.Items.Add(new ListItem(r.Descrizione, r.Valore));
                        }
                    }
                }
            }

            //Controllo dei report per CDC
            setRadioButtonListCDC();


            panel_sede.Visible = true;
        }

        private void setRadioButtonListCDC()
        {

            //Ruolo ruolo = DocsPAWA.UserManager.getRuolo(this);//OLD
            NttDataWA.DocsPaWR.Ruolo ruolo = NttDataWA.UIManager.RoleManager.GetRoleInSession();
            if (ruolo != null && ruolo.funzioni != null)
            {
                NttDataWA.DocsPaWR.Funzione REPORT_CDC_GLOBALI = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_GLOBALI")).FirstOrDefault();
                NttDataWA.DocsPaWR.Funzione REPORT_CDC_PER_UFFICIO = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_PER_UFFICIO")).FirstOrDefault();
                NttDataWA.DocsPaWR.Funzione REPORT_CDC_DECRETI = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_DECRETI")).FirstOrDefault();

                //Funzioni per CDC SRC
                NttDataWA.DocsPaWR.Funzione REPORT_CDC_GLOBALI_SRC = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_GLOBALI_SRC")).FirstOrDefault();
                NttDataWA.DocsPaWR.Funzione REPORT_CDC_PER_UFFICIO_SRC = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_PER_UFFICIO_SRC")).FirstOrDefault();
                NttDataWA.DocsPaWR.Funzione REPORT_CDC_DECRETI_SRC = ruolo.funzioni.Where(funzione => funzione.codice.ToUpper().Equals("REPORT_CDC_DECRETI_SRC")).FirstOrDefault();


                if (REPORT_CDC_GLOBALI != null || REPORT_CDC_PER_UFFICIO != null)
                {
                    ddl_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. Centrali di Controllo (Controllo Preventivo)", "CDC_reportControlloPreventivo"));
                    // Questi report non sono più necessari
                    //rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. Centrali di Controllo (Pensioni Civili)", "CDC_reportPensioniCivili"));
                    //rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. Centrali di Controllo (Pensioni Militari)", "CDC_reportPensioniMilitari"));
                }

                if (REPORT_CDC_DECRETI != null)
                {
                    // Visualizzazione report "Decreti in esame" e "Decreti pervenuti in intervallo temporale"
                    ddl_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. di Controllo (Scadenzario - Decreti in esame)", "CDC_reportDecretiInEsame"));
                    ddl_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. di Controllo (Decreti pervenuti in intervallo temporale)", "CDC_reportDecretiPervenutiInIntTemporale"));

                    ddl_prospettiRiepilogativi.Items.Add(new ListItem("Elenchi decreti registrati Uffici Sez. Centrale", "CDC_reportElencoDecretiSCCLA"));
                    ddl_prospettiRiepilogativi.Items.Add(new ListItem("Elenchi decreti restituiti Uffici Sez. Centrale", "CDC_reportElencoDecretiRestituitiSCCLA"));
                    ddl_prospettiRiepilogativi.Items.Add(new ListItem("Elenchi decreti restituiti con rilievo Uffici Sez. Centrale ", "CDC_reportElencoDecretiRestituitiConRilievoSCCLA"));
                }

                if (REPORT_CDC_GLOBALI_SRC != null || REPORT_CDC_PER_UFFICIO_SRC != null)
                {
                    ddl_prospettiRiepilogativi.Items.Add(new ListItem("Report Sez. Regionali di Controllo (Controllo Preventivo)", "CDC_reportControlloPreventivoSRC"));
                    // Questi report non sono più necessari
                    //rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. Centrali di Controllo (Pensioni Civili)", "CDC_reportPensioniCivili"));
                    //rb_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. Centrali di Controllo (Pensioni Militari)", "CDC_reportPensioniMilitari"));
                }

                if (REPORT_CDC_DECRETI_SRC != null)
                {
                    // Visualizzazione report "Decreti in esame" e "Decreti pervenuti in intervallo temporale"
                    ddl_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. di Controllo Reg. (Scadenzario - Decreti in esame)", "CDC_reportDecretiInEsameSRC"));
                    ddl_prospettiRiepilogativi.Items.Add(new ListItem("Report Uff. di Controllo Reg. (Decreti pervenuti in intervallo temporale)", "CDC_reportDecretiPervenutiInIntTemporaleSRC"));

                    ddl_prospettiRiepilogativi.Items.Add(new ListItem("Elenchi decreti registrati Sez. Regionale", "CDC_reportElencoDecretiSRC"));
                    ddl_prospettiRiepilogativi.Items.Add(new ListItem("Elenchi decreti restituiti Sez. Regionale", "CDC_reportElencoDecretiRestituitiSRC"));
                    ddl_prospettiRiepilogativi.Items.Add(new ListItem("Elenchi decreti restituiti con rilievo Sez. Regionale", "CDC_reportElencoDecretiRestituitiConRilievoSRC"));
                }
            }
        }



        private void InitializePage()
        {
            this.InitializeLanguage();
            this.Registry = RoleManager.GetRoleInSession().registri[0];
            UIManager.RegistryManager.SetRegistryInSession(RoleManager.GetRoleInSession().registri[0]);
            this.SetAjaxAddressBook();

            //this.LoadKeys();             
            //this.PopulateDdlSearchDocument();
            //this.LinkSearchVisibility.Attributes["onclick"] = "disallowOp('IdMasterBody')";
        }
        private Registro Registry
        {
            get
            {
                DocsPaWR.Registro result = null;
                if (HttpContext.Current.Session["registry"] != null)
                {
                    result = HttpContext.Current.Session["registry"] as DocsPaWR.Registro;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["registry"] = value;
            }
        }

        private Ruolo Role
        {
            get
            {
                return UIManager.RoleManager.GetRoleInSession();
            }
            set
            {
                HttpContext.Current.Session["role"] = value;
            }
        }

        protected void SetAjaxAddressBook()
        {
            string dataUser = this.Role.systemId;
            if (this.Registry == null)
            {
                this.Registry = RegistryManager.GetRegistryInSession();
            }
            dataUser = dataUser + "-" + this.Registry.systemId;

            this.CallType = RubricaCallType.CALLTYPE_OWNER_AUTHOR;

            //this.RapidCreatoreTransm.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + callType;
            this.RapidProprietario.ContextKey = dataUser + "-" + UIManager.UserManager.GetUserInSession().idAmministrazione + "-" + this.CallType;


        }

        protected void TxtCode_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                CustomTextArea caller = sender as CustomTextArea;
                string codeAddressBook = caller.Text;

                if (!string.IsNullOrEmpty(codeAddressBook))
                {
                    this.SearchCorrespondent(codeAddressBook, caller.ID);
                }
                else
                {
                    switch (caller.ID)
                    {
                        //case "txt1_corr_sott":
                        //    this.txt1_corr_sott.Text = string.Empty;
                        //    this.txt2_corr_sott.Text = string.Empty;
                        //    this. .Value = string.Empty;
                        //    this.UpPnlDestRole.Update();
                        //    break;
                        case "txt_codCorr":
                            this.txt_codCorr.Text = string.Empty;
                            this.txt_descrCorr.Text = string.Empty;
                            this.idProprietario.Value = string.Empty;
                            this.UpPnlSummeriesUO.Update();
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchCorrespondent(string addressCode, string idControl)
        {
            DocsPaWR.Corrispondente corr = UIManager.AddressBookManager.getCorrispondenteRubrica(addressCode, this.CallType);
            if (corr == null)
            {
                switch (idControl)
                {
                    //case "txt1_corr_sott":
                    //    this.txt1_corr_sott.Text = string.Empty;
                    //    this.txt2_corr_sott.Text = string.Empty;
                    //    this.idCreatore.Value = string.Empty;
                    //    this.UpPnlDestRole.Update();
                    //    break;
                    case "txt_codCorr":
                        this.txt_codCorr.Text = string.Empty;
                        this.txt_descrCorr.Text = string.Empty;
                        this.idProprietario.Value = string.Empty;
                        this.UpPnlSummeriesUO.Update();
                        break;

                }

                string msg = "ErrorTransmissionCorrespondentNotFound";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
            }
            else
            {
                switch (idControl)
                {
                    //case "txt1_corr_sott":
                    //    this.txt1_corr_sott.Text = corr.codiceRubrica;
                    //    this.txt2_corr_sott.Text = corr.descrizione;
                    //    this.idCreatore.Value = corr.systemId;
                    //    this.UpContainer.Update();
                    //    this.UpPnlDestRole.Update();
                    //    break;
                    case "txt_codCorr":
                        this.txt_codCorr.Text = corr.codiceRubrica;
                        this.txt_descrCorr.Text = corr.descrizione;
                        this.idProprietario.Value = corr.systemId;
                        //this.rbl_tipo_corr.SelectedIndex = -1;
                        //this.rbl_tipo_corr.Items.FindByValue(corr.tipoCorrispondente).Selected = true;
                        this.UpPnlSummeriesUO.Update();
                        break;

                }
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            //this.PDFViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
            this.DocumentViewerSummeries.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);


            this.LitSummeries.Text = Utils.Languages.GetLabelFromCode("SummeriesTitle", language);

            ///////////////////////////////////////////////////////////////////////////////////////
            //Pulsanti
            this.SummeriesPrint.Text = Utils.Languages.GetLabelFromCode("RegisterRepertoriesBtnPrint", language); //Stampa
            this.SummeriesZoom.Text = Utils.Languages.GetLabelFromCode("DocumentZoomFile", language);//Zoom


            //labels
            this.SummeriestLblAmm.Text = Utils.Languages.GetLabelFromCode("SummeriestLblAmm", language);
            this.SummeriestLblReg.Text = Utils.Languages.GetLabelFromCode("SummeriestLblReg", language);
            this.SummeriestLblTit.Text = Utils.Languages.GetLabelFromCode("SummeriestLblTit", language);
            this.SummeriestLblSede.Text = Utils.Languages.GetLabelFromCode("SummeriestLblSede", language);
            this.SummeriestLblAnno.Text = Utils.Languages.GetLabelFromCode("SummeriestLblAnno", language);
            this.SummeriestLblMese.Text = Utils.Languages.GetLabelFromCode("SummeriestLblMese", language);
            this.SummeriestLblModa.Text = Utils.Languages.GetLabelFromCode("SummeriestLblModa", language);
            this.SummeriestLblReg2.Text = Utils.Languages.GetLabelFromCode("SummeriestLblReg2", language);
            this.SummeriestLblTit2.Text = Utils.Languages.GetLabelFromCode("SummeriestLblTit2", language);
            this.SummeriestLblPra.Text = Utils.Languages.GetLabelFromCode("SummeriestLblPra", language);
            this.SummeriestLblSpe.Text = Utils.Languages.GetLabelFromCode("SummeriestLblSpe", language);
            this.SummeriestLblProto.Text = Utils.Languages.GetLabelFromCode("SummeriestLblProto", language);
            this.SummeriestLblProprie.Text = Utils.Languages.GetLabelFromCode("SummeriestLblProprie", language);
            this.SummeriestLblUO.Text = Utils.Languages.GetLabelFromCode("SummeriestLblUO", language);
            this.SummeriestLblDcrea.Text = Utils.Languages.GetLabelFromCode("SummeriestLblDcrea", language);
            this.ImgProprietarioAddressBook.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            this.ImgProprietarioAddressBook.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgCustomCorrespondentAddressBookCustom", language);
            //            =Amministrazione *
            //=Registro *
            //=Titolario *
            //=Sede
            //=Anno *
            //=Mese
            //=Modalità
            //=Registro *
            //=Titolario *
            //=Pratica *
            //=Spedizione *
            //=Conferma di protocollazione *
            //=Proprietario *
            //=UO *
            //=Data creazione
        }

        //ADP 
        //Gestione campi relativi alla data di creazione Report Conteggio fascicoli procidimentali
        protected void ddl_tipo_data_creazione_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.cld_creazione_al.Text = "";
            this.cld_creazione_dal.Text = "";

            if (this.ddl_tipo_data_creazione.SelectedIndex == 0)
            {
                this.cld_creazione_al.Visible = false;
                this.lbl_al_apertura.Visible = false;
                this.lbl_dal_apertura.Visible = false;
                this.cld_creazione_dal.Enabled = true;
            }

            if (this.ddl_tipo_data_creazione.SelectedIndex == 1)
            {
                this.cld_creazione_al.Visible = true;
                this.lbl_al_apertura.Visible = true;
                this.lbl_dal_apertura.Visible = true;
                this.cld_creazione_dal.Enabled = true;
                this.cld_creazione_al.Enabled = true;
            }

            if (this.ddl_tipo_data_creazione.SelectedIndex == 2)
            {
                this.cld_creazione_al.Visible = false;
                this.lbl_al_apertura.Visible = false;
                this.lbl_dal_apertura.Visible = false;
                this.cld_creazione_dal.Text = NttDataWA.UIManager.DocumentManager.toDay();
                this.cld_creazione_dal.Enabled = false;
            }

            if (this.ddl_tipo_data_creazione.SelectedIndex == 3)
            {
                this.cld_creazione_al.Visible = true;
                this.lbl_al_apertura.Visible = true;
                this.lbl_dal_apertura.Visible = true;
                this.cld_creazione_dal.Text = NttDataWA.UIManager.DocumentManager.getFirstDayOfWeek();
                this.cld_creazione_al.Text = NttDataWA.UIManager.DocumentManager.getLastDayOfWeek();
                this.cld_creazione_dal.Enabled = false;
                this.cld_creazione_al.Enabled = false;
            }

            if (this.ddl_tipo_data_creazione.SelectedIndex == 4)
            {
                this.cld_creazione_al.Visible = true;
                this.lbl_al_apertura.Visible = true;
                this.lbl_dal_apertura.Visible = true;
                this.cld_creazione_dal.Text = NttDataWA.UIManager.DocumentManager.getFirstDayOfMonth();
                this.cld_creazione_al.Text = NttDataWA.UIManager.DocumentManager.getLastDayOfMonth();
                this.cld_creazione_dal.Enabled = false;
                this.cld_creazione_al.Enabled = false;
            }
        }

        //ADP 
        //Gestione campi relativi alla data di creazione Report Conteggio fascicoli procidimentali
        protected void ddl_tipo_data_chiusura_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.cld_chiusura_al.Text = "";
            this.cld_chiusura_dal.Text = "";

            if (this.ddl_tipo_data_chiusura.SelectedIndex == 0)
            {
                this.cld_chiusura_al.Visible = false;
                this.lbl_al_chiusura.Visible = false;
                this.lbl_dal_chiusura.Visible = false;
                this.cld_chiusura_dal.Enabled = true;
            }

            if (this.ddl_tipo_data_chiusura.SelectedIndex == 1)
            {
                this.cld_chiusura_al.Visible = true;
                this.lbl_al_chiusura.Visible = true;
                this.lbl_dal_chiusura.Visible = true;
                this.cld_chiusura_dal.Enabled = true;
                this.cld_chiusura_al.Enabled = true;
            }

            if (this.ddl_tipo_data_chiusura.SelectedIndex == 2)
            {
                this.cld_chiusura_al.Visible = false;
                this.lbl_al_chiusura.Visible = false;
                this.lbl_dal_chiusura.Visible = false;
                this.cld_chiusura_dal.Text = NttDataWA.UIManager.DocumentManager.toDay();
                this.cld_chiusura_dal.Enabled = false;
            }

            if (this.ddl_tipo_data_chiusura.SelectedIndex == 3)
            {
                this.cld_chiusura_al.Visible = true;
                this.lbl_al_chiusura.Visible = true;
                this.lbl_dal_chiusura.Visible = true;
                this.cld_chiusura_dal.Text = NttDataWA.UIManager.DocumentManager.getFirstDayOfWeek();
                this.cld_chiusura_al.Text = NttDataWA.UIManager.DocumentManager.getLastDayOfWeek();
                this.cld_chiusura_dal.Enabled = false;
                this.cld_chiusura_al.Enabled = false;
            }

            if (this.ddl_tipo_data_chiusura.SelectedIndex == 4)
            {
                this.cld_chiusura_al.Visible = true;
                this.lbl_al_chiusura.Visible = true;
                this.lbl_dal_chiusura.Visible = true;
                this.cld_chiusura_dal.Text = NttDataWA.UIManager.DocumentManager.getFirstDayOfMonth();
                this.cld_chiusura_al.Text = NttDataWA.UIManager.DocumentManager.getLastDayOfMonth();
                this.cld_chiusura_dal.Enabled = false;
                this.cld_chiusura_al.Enabled = false;
                //GetCalendarControl
            }
        }

        protected void ddl_prospettiRiepilogativi_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            #region SWITCH commentato
            //if (!string.IsNullOrEmpty(this.ddl_prospettiRiepilogativi.SelectedValue))
            //{
            //    switch (this.ddl_prospettiRiepilogativi.SelectedValue)
            //    {
            //            // PlaceHolder as Panel
            //        case "0": //Protocollati
            //            this.PhlSearchIDDocument.Visible = false;
            //            PhlSearchProto.Visible = true;
            //            this.phlSearchTypologyDoc.Visible = false;
            //            break;
            //        case "1": //Non Protocollati 
            //            this.PhlSearchIDDocument.Visible = true;
            //            PhlSearchProto.Visible = false;
            //            this.phlSearchTypologyDoc.Visible = false;
            //            break;
            //        case "2": // Ricerca per tipologia 
            //            this.PhlSearchIDDocument.Visible = false;
            //            PhlSearchProto.Visible = false;
            //            this.phlSearchTypologyDoc.Visible = true;
            //            break;
            //        case  "reportAnnualeDoc"  :
            //              break;
            //        case"reportDocClassificati":
            //              break;
            //        case"reportAnnualeDocTrasmAmm":
            //              break;
            //        case"reportAnnualeDocTrasmAOO":
            //              break;
            //        case"reportAnnualeFasc":
            //              break;
            //        case"reportAnnualeFascXTit":
            //              break;
            //        case"tempiMediLavFasc":
            //              break;
            //        case"ReportDocXSede":
            //              break;
            //        case"ReportDocXUo":
            //              break;
            //        case"reportNumFasc":
            //              break;
            //        case"reportNumDocInFasc":
            //              break;

            //        ////al momento non sono attivi
            //        //case"stampaProtArma":
            //        //      break;
            //        //case"stampaDettaglioPratica":
            //        //      break;
            //        //case"stampaGiornaleRiscontri":
            //        //      break;
            //        //case"documentiSpediti":
            //        //      break;

            //    }
            //}

            #endregion

            #region OLD
            try
            {

                // utility.do_openinRightFrame(this, "whitepage.htm");//OLD!! ADP

                utility.RemoveFileReport(this);
                string _value = ddl_prospettiRiepilogativi.SelectedValue;

                //ADP 8 maggio 2013
                this.pnl_sottoposto.Visible = false;


                if ((_value != "reportDocInMisOb"))
                {
                    ////ADP 8 maggio 2013
                    pnl_MisObiettivi.Visible = false;

                    caricaComboRegistri(false);

                }
                else
                {
                    ////ADP 8 maggio 2013
                    pnl_MisObiettivi.Visible = true;

                    caricaComboRegistri(true);
                    pnl_anno.Visible = false;

                    pnl_mese.Visible = false;

                    this.dtaCollocation_TxtFrom.Text = "01/01/" + System.DateTime.Today.Year.ToString();//OLD
                    this.dtaCollocation_TxtTo.Text = System.DateTime.Today.ToString().Substring(0, 10);

                    this.SummeriesZoom.Enabled = false;
                }


                //if ((_value != "reportAnnualeDoc") && (_value != "reportDocClassificati") && (_value != "tempiMediLavFasc"))
                if ((_value != "reportAnnualeDoc") && (_value != "reportDocClassificati"))
                {
                    panel_sede.Visible = false;
                }
                else
                {
                    panel_sede.Visible = true;
                }
                if ((_value == "reportAnnualeFascXTit") || (_value == "reportDocClassificati") || (_value == "tempiMediLavFasc"))
                {
                    pnl_modalita.Visible = true;
                }
                else
                {
                    pnl_modalita.Visible = false;
                }

                if ((_value == "reportAnnualeDoc") || (_value == "reportAnnualeFasc"))
                {
                    pnl_mese.Visible = true;
                }
                else
                {
                    pnl_mese.Visible = false;
                }
                if (_value == "reportContatoriDocumento" || _value == "reportContatoriFascicolo")
                {
                    panel_reg.Visible = false;
                }
                else
                {
                    panel_reg.Visible = true;
                }
                if (_value == "stampaProtArma" || _value == "stampaDettaglioPratica" || _value == "stampaGiornaleRiscontri" || _value == "documentiSpediti" || _value == "reportNumFasc" || _value == "reportNumDocInFasc")
                {
                    pnl_protArma.Visible = true;
                    pnl_regCC.Visible = true;
                    pnl_anno.Visible = false;
                    pnl_amm.Visible = false;
                    this.panel_reg.Visible = false;
                    this.pnl_DettPratica.Visible = false;
                    this.pnl_DocSpediti.Visible = false;
                    if (_value == "stampaDettaglioPratica")
                        this.pnl_DettPratica.Visible = true;
                    if (_value == "stampaGiornaleRiscontri")
                        pnl_protArma.Visible = false;
                    if (_value == "documentiSpediti")
                    {
                        pnl_protArma.Visible = false;
                        this.pnl_DocSpediti.Visible = true;
                    }
                    if (_value == "reportNumFasc" || _value == "reportNumDocInFasc")
                    {
                        pnl_protArma.Visible = false;
                        pnl_regCC.Visible = false;
                        pnl_anno.Visible = false;
                        pnl_amm.Visible = false;
                        this.panel_reg.Visible = false;
                        this.pnl_DettPratica.Visible = false;
                        this.pnl_DocSpediti.Visible = false;

                        //8 maggio 2013
                        this.pnl_sottoposto.Visible = true;
                        this.pnl_scelta_uo.Visible = true;
                        this.pnl_scelta_rf.Visible = false;
                        this.rb_scelta_sott.SelectedValue = "UO";
                        // this.btn_img_sott_rubr.Visible = true;//ADP 8 maggio 2013

                        //Inserisci uo 
                        //DocsPAWA.DocsPaWR.InfoUtente infoUtente = DocsPAWA.UserManager.getInfoUtente(this);//OLD
                        DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                        //Ruolo ruoloTipologia = DocsPAWA.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo, this);//OLD
                        NttDataWA.DocsPaWR.Ruolo ruoloTipologia = NttDataWA.UIManager.RoleManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                        if (ruoloTipologia != null && ruoloTipologia.uo != null)
                        {
                            ////8 maggio 2013
                            //this.txt2_corr_sott.Text = ruoloTipologia.uo.descrizione;
                            //this.txt1_corr_sott.Text = ruoloTipologia.uo.codice;
                        }
                        this.SummeriesZoom.Enabled = false;
                    }
                }
                else
                {
                    this.pnl_regCC.Visible = false;
                    this.panel_reg.Visible = true;
                    this.pnl_amm.Visible = true;


                    ////Laura Mis Obiettivi
                    if (_value != "reportDocInMisOb")
                    {
                        this.pnl_anno.Visible = true;
                    }

                    this.pnl_protArma.Visible = false;
                    this.pnl_DettPratica.Visible = false;
                    this.pnl_DocSpediti.Visible = false;
                }

                if (_value == "reportAnnualeFascXTit" || _value == "reportDocClassificati" || _value == "reportAnnualeDoc" || _value == "tempiMediLavFasc")
                {
                    panel_tit_ann_fasc.Visible = true;
                }
                else
                {
                    panel_tit_ann_fasc.Visible = false;
                }

                //Controllo dei report per CDC
                setPanelReportCDC();

                caricaComboTitolariReportTitolario();

                this.SummeriesZoom.Enabled = false;
                UpPnlDocumentData.Visible = false;
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                //DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
            }
            #endregion

        }

        private void caricaComboTitolariReportTitolario()
        {
            ddl_titolari_report_annuale.Items.Clear();
            //ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(DocsPAWA.UserManager.getUtente(this).idAmministrazione));
            ArrayList listaTitolari = new ArrayList(ClassificationSchemeManager.getTitolariUtilizzabili());
            //Esistono dei titolari chiusi
            if (listaTitolari.Count > 1)
            {
                //Creo le voci della ddl dei titolari
                string valueTutti = string.Empty;
                foreach (NttDataWA.DocsPaWR.OrgTitolario titolario in listaTitolari)
                {
                    ListItem it = null;
                    switch (titolario.Stato)
                    {
                        case NttDataWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari_report_annuale.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                        case NttDataWA.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari_report_annuale.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                    }
                }

                //Imposto la voce tutti i titolari             
                string _value = ddl_prospettiRiepilogativi.SelectedValue;

                if ((_value != "reportAnnualeFascXTit"))
                {
                    valueTutti = valueTutti.Substring(0, valueTutti.Length - 1);
                    if (valueTutti != string.Empty)
                    {
                        if (valueTutti.IndexOf(',') == -1)
                            valueTutti = valueTutti + "," + valueTutti;

                        ListItem it = new ListItem("Tutti i titolari", "0");
                        ddl_titolari_report_annuale.Items.Insert(0, it);
                    }
                }
            }

            //Non esistono titolario chiusi
            if (listaTitolari.Count == 1)
            {
                NttDataWA.DocsPaWR.OrgTitolario titolario = (NttDataWA.DocsPaWR.OrgTitolario)listaTitolari[0];
                if (titolario.Stato != NttDataWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    ddl_titolari_report_annuale.Items.Add(it);
                }
                ddl_titolari_report_annuale.Enabled = false;
            }
        }

        private void setPanelReportCDC()
        {
            //pnl_FiltriCDC.Visible = false;
            //((TextBox)FindControl("txt_Elenco")).Text = "";

            //switch (ddl_prospettiRiepilogativi.SelectedValue)
            //{
            //    case "CDC_reportControlloPreventivo":
            //    case "CDC_reportControlloPreventivoSRC":
            //    case "CDC_reportPensioniCivili":
            //    case "CDC_reportPensioniMilitari":
            //    case "CDC_reportDecretiPervenutiInIntTemporale":
            //        pnl_amm.Visible = false;
            //        panel_reg.Visible = false;
            //        panel_sede.Visible = false;
            //        pnl_anno.Visible = false;
            //        pnl_mese.Visible = false;
            //        pnl_modalita.Visible = false;
            //        pnl_regCC.Visible = false;
            //        pnl_protArma.Visible = false;
            //        pnl_DettPratica.Visible = false;
            //        pnl_DocSpediti.Visible = false;
            //        this.pnlContrData.Visible = true;
            //        ((Panel)FindControl("pnl_FiltriCDCElencoDecreti")).Visible = false;
            //        pnl_FiltriCDC.Visible = true;
            //        showCorr();
            //        resetElementsCDC();
            //        setVisibilityElementsCDC();
            //        setDdlReportCDC();
            //        break;

            //    case "CDC_reportDecretiPervenutiInIntTemporaleSRC":
            //        pnl_amm.Visible = false;
            //        panel_reg.Visible = false;
            //        panel_sede.Visible = false;
            //        pnl_anno.Visible = false;
            //        pnl_mese.Visible = false;
            //        pnl_modalita.Visible = false;
            //        pnl_regCC.Visible = false;
            //        pnl_protArma.Visible = false;
            //        pnl_DettPratica.Visible = false;
            //        pnl_DocSpediti.Visible = false;
            //        this.pnlContrData.Visible = true;

            //        pnl_FiltriCDC.Visible = true;
            //        ((Panel)FindControl("pnl_FiltriCDCElencoDecreti")).Visible = false;
            //        showCorr();
            //        resetElementsCDC();
            //        setVisibilityElementsCDC();
            //        setDdlReportCDC();
            //        break;

            //    case "CDC_reportDecretiInEsame":
            //        pnl_amm.Visible = false;
            //        panel_reg.Visible = false;
            //        panel_sede.Visible = false;
            //        pnl_anno.Visible = false;
            //        pnl_mese.Visible = false;
            //        pnl_modalita.Visible = false;
            //        pnl_regCC.Visible = false;
            //        pnl_protArma.Visible = false;
            //        pnl_DettPratica.Visible = false;
            //        pnl_DocSpediti.Visible = false;
            //        pnlContrData.Visible = false;
            //        pnl_FiltriCDC.Visible = true;
            //        ((Panel)FindControl("pnl_FiltriCDCElencoDecreti")).Visible = false;
            //        showCorr();
            //        resetElementsCDC();
            //        setVisibilityElementsCDC();
            //        setDdlReportCDC();

            //        break;

            //    case "CDC_reportDecretiInEsameSRC":
            //        pnl_amm.Visible = false;
            //        panel_reg.Visible = false;
            //        panel_sede.Visible = false;
            //        pnl_anno.Visible = false;
            //        pnl_mese.Visible = false;
            //        pnl_modalita.Visible = false;
            //        pnl_regCC.Visible = false;
            //        pnl_protArma.Visible = false;
            //        pnl_DettPratica.Visible = false;
            //        pnl_DocSpediti.Visible = false;
            //        pnlContrData.Visible = false;
            //        pnl_FiltriCDC.Visible = true;
            //        showCorr();
            //        ((Panel)FindControl("pnl_FiltriCDCElencoDecreti")).Visible = false;

            //        resetElementsCDC();
            //        setVisibilityElementsCDC();
            //        setDdlReportCDC();

            //        break;

            //    case "CDC_reportElencoDecretiSRC":
            //    case "CDC_reportElencoDecretiRestituitiSRC":
            //    case "CDC_reportElencoDecretiRestituitiConRilievoSRC":
            //    case "CDC_reportElencoDecretiSCCLA":
            //    case "CDC_reportElencoDecretiRestituitiSCCLA":
            //    case "CDC_reportElencoDecretiRestituitiConRilievoSCCLA":
            //        pnl_amm.Visible = false;
            //        panel_reg.Visible = false;
            //        panel_sede.Visible = false;
            //        pnl_anno.Visible = false;
            //        pnl_mese.Visible = false;
            //        pnl_modalita.Visible = false;
            //        pnl_regCC.Visible = false;
            //        pnl_protArma.Visible = false;
            //        pnl_DettPratica.Visible = false;
            //        pnl_DocSpediti.Visible = false;

            //        pnl_FiltriCDC.Visible = false;
            //        this.pnlContrData.Visible = false;
            //        ((Panel)FindControl("pnl_FiltriCDCElencoDecreti")).Visible = true;
            //        hideCorr();
            //        resetElementsCDC();
            //        setVisibilityElementsCDC();
            //        setDdlReportCDC();

            //        break;
            //}
        }


        private bool DO_VerifyList(DropDownList list, string system_id)
        {
            bool result = false;
            for (int i = 0; i < list.Items.Count; i++)
            {
                if (system_id == list.Items[i].Value)
                {
                    result = true;
                    return result;
                }
            }
            return result;
        }

        private void caricaComboRegistri(bool all)
        {

            ddl_registro.Items.Clear();
            ddl_registro.Items.Add("");

            if (all)
            {
                string amm = ddl_amm.SelectedValue;
                //Laura 20 Dicembre
                Controller.DO_GetRegistriByAmministrazione(ddl_registro, amm, true);
                //ddl_registro.SelectedValue = UserManager.getRuolo(this).registri[0].systemId;
                ddl_registro.SelectedIndex = 1;
                //UserManager.setRegistroSelezionato(this, wws.GetRegistroBySistemId(ddl_registro.SelectedValue));//OLD
                UserManager.setRegistroSelezionato(this, UIManager.RegistryManager.getRegistroBySistemId(ddl_registro.SelectedValue));//OLD
                UIManager.SummeriesManager.Parametro reg = new UIManager.SummeriesManager.Parametro("reg", ddl_registro.SelectedItem.Text, ddl_registro.SelectedItem.Value);
                if (Session["filtri"] != null)
                {
                    filtri = (ArrayList)Session["filtri"];
                    utility.DO_UpdateParameters(filtri, reg);
                    Session["filtri"] = filtri;
                }
                ddl_registro.Items[0].Text = "Tutte le sedi";
            }
            else
            {

                _utente = UserManager.GetUserInSession();

                for (int i = 0; i < _utente.ruoli.Length; i++)
                {
                    NttDataWA.DocsPaWR.Ruolo ruolo = (NttDataWA.DocsPaWR.Ruolo)_utente.ruoli[i];
                    for (int j = 0; j < ruolo.registri.Length; j++)
                    {
                        NttDataWA.DocsPaWR.Registro reg = (NttDataWA.DocsPaWR.Registro)ruolo.registri[j];
                        if (!DO_VerifyList(ddl_registro, reg.systemId))
                        {
                            ddl_registro.Items.Add(new ListItem(reg.descrizione, reg.systemId));
                        }
                    }
                }

            }
            if (ddl_registro.Items.Count >= 2)
            {
                if (!all)
                    ddl_registro.SelectedIndex = 1;
                int idReg = Convert.ToInt32(ddl_registro.SelectedValue);

                UIManager.SummeriesManager.Parametro reg = new UIManager.SummeriesManager.Parametro("reg", ddl_registro.SelectedItem.Text, ddl_registro.SelectedItem.Value);
                filtri.Add(reg);

                ddl_anno.Items.Clear(); //svuoto la ddl_anno per evitare ripetizioni
                NttDataWA.UIManager.SummeriesManager.Controller.DO_GetAnniProfilazione(ddl_anno, idReg);
                // verifico presenza Sedi, se presenti abilito la combobox e aggiungo
                //il filtro di ricerca.
                bool popolata = false;
                ddl_sede.Items.Clear(); //svuoto la ddl_sede per evitare ripetizioni
                NttDataWA.UIManager.SummeriesManager.Controller.DO_GetSedi(Convert.ToInt32(NttDataWA.UIManager.SummeriesManager.Controller.DO_GetIdAmmByCodAmm(ddl_amm.SelectedValue)), ddl_sede, out popolata);
                if (popolata)
                {
                    // Gabriele Melini 08-05-2014
                    // se è selezionato "Tutte le sedi", il valore del parametro deve essere una stringa vuota
                    UIManager.SummeriesManager.Parametro _sede = null;
                    if(ddl_sede.SelectedIndex == 0)
                        _sede = new UIManager.SummeriesManager.Parametro("sede", ddl_sede.SelectedItem.Text, string.Empty);
                    else
                        _sede = new UIManager.SummeriesManager.Parametro("sede", ddl_sede.SelectedItem.Text, ddl_sede.SelectedItem.Value);

                    filtri.Add(_sede);
                }
                else
                {
                    utility.SetFocus(this, ddl_anno);
                }
            }
        }



        protected void DocumentDdlTypeDocument_OnSelectedIndexChanged(object sender, System.EventArgs e)
        {

        }

        protected string GetTitle()
        {
            return this.VisibilityHistory.Title;
        }




        protected void SummeriesImgSenderAddressBook_Click(object sender, EventArgs e)
        {
            try
            {
                this.CallType = RubricaCallType.CALLTYPE_OWNER_AUTHOR;
                HttpContext.Current.Session["AddressBook.from"] = "F_X_X_S";
                HttpContext.Current.Session["AddressBook.EnableOnly"] = "U"; //Unità organizzativa
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AddressBook", "ajaxModalPopupAddressBook();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
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


        //Controllo campi selezionati
        private bool controllaCampiObbligatoriExportFasc()
        {
            bool result = true;

            //ADP 8 maggio 2013
            if (rb_scelta_sott.SelectedValue.Equals("UO"))
            {
                //if ((string.IsNullOrEmpty(txt1_corr_sott.Text)) || (string.IsNullOrEmpty(txt2_corr_sott.Text)))
                if ((string.IsNullOrEmpty(this.txt_codCorr.Text)) || (string.IsNullOrEmpty(this.txt_descrCorr.Text)))
                {
                    //OLD alert
                    //utility.do_alert(this, "Attenzione! Inserire una UO");
                    //utility.do_openinRightFrame(this, "whitepage.htm");
                    string msgDesc = "AlertSummeriesUO";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);


                    result = false;
                }
            }
            //ADP 8 maggio 2013
            if (rb_scelta_sott.SelectedValue.Equals("RF") && result)
            {
                if ((string.IsNullOrEmpty(ddl_rf.SelectedValue)))
                {
                    //OLD alert
                    //utility.do_alert(this, "Attenzione! Inserire un RF");
                    //utility.do_openinRightFrame(this, "whitepage.htm");
                    string msgDesc = "AlertSummeriesRF";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);


                    result = false;
                }
            }
            //ADP 8 maggio 2013
            if (string.IsNullOrEmpty(ddl_titolari.SelectedValue) && result)
            {
                //OLD alert
                //utility.do_alert(this, "Attenzione! Scegliere un titolario");
                //utility.do_openinRightFrame(this, "whitepage.htm");
                string msgDesc = "AlertSummeriesT";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);

                result = false;
            }
            //ADP 8 maggio 2013
            if (!string.IsNullOrEmpty(this.cld_creazione_dal.Text) && result)
            {
                if (!utils.isDate(this.cld_creazione_dal.Text))
                {
                    //OLD alert
                    //utility.do_alert(this, "Attenzione! Formato data creazione errato");
                    //utility.do_openinRightFrame(this, "whitepage.htm");
                    string msgDesc = "AlertSummeriesDataCrea";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);


                    result = false;
                }
            }
            //ADP 8 maggio 2013
            if (!string.IsNullOrEmpty(this.cld_chiusura_dal.Text) && result)
            {
                if (!utils.isDate(this.cld_chiusura_dal.Text))
                {
                    //OLD alert
                    //utility.do_alert(this, "Attenzione! Formato data chiusura errato");
                    //utility.do_openinRightFrame(this, "whitepage.htm");
                    string msgDesc = "AlertSummeriesDataChiu";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);

                    result = false;
                }
            }


            //ADP 8 maggio 2013
            if (!string.IsNullOrEmpty(this.cld_creazione_al.Text) && result)
            {
                if (!utils.isDate(this.cld_creazione_al.Text))
                {
                    //OLD alert
                    //utility.do_alert(this, "Attenzione! Formato data creazione errato");
                    //utility.do_openinRightFrame(this, "whitepage.htm");
                    string msgDesc = "AlertSummeriesDataCrea";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);


                    result = false;
                }
            }

            //ADP 8 maggio 2013
            if (!string.IsNullOrEmpty(this.cld_chiusura_al.Text) && result)
            {
                if (!utils.isDate(this.cld_chiusura_al.Text))
                {
                    //OLD alert
                    //utility.do_alert(this, "Attenzione! Formato data chiusura errato");
                    //utility.do_openinRightFrame(this, "whitepage.htm");
                    string msgDesc = "AlertSummeriesDataChiu";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);

                    result = false;
                }
            }
            //ADP 8 maggio 2013
            if (result && !string.IsNullOrEmpty(cld_chiusura_dal.Text) && !string.IsNullOrEmpty(cld_chiusura_al.Text) && utils.verificaIntervalloDate(this.cld_chiusura_dal.Text, this.cld_chiusura_al.Text))
            {
                //OLD alert
                //utility.do_alert(this, "Attenzione! Intervallo data chiusura errato");
                //utility.do_openinRightFrame(this, "whitepage.htm");
                string msgDesc = "AlertSummeriesDataChiuIN";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);


                result = false;
            }
            //ADP 8 maggio 2013
            if (result && !string.IsNullOrEmpty(cld_creazione_dal.Text) && !string.IsNullOrEmpty(cld_creazione_al.Text) && NttDataWA.Utils.utils.verificaIntervalloDate(this.cld_creazione_dal.Text, this.cld_creazione_al.Text))
            {
                //OLD alert
                //utility.do_alert(this, "Attenzione! Intervallo data creazione errato");
                //utility.do_openinRightFrame(this, "whitepage.htm");
                string msgDesc = "AlertSummeriesDataCreaIN";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);


                result = false;
            }


            return result;
        }

        private NttDataWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (NttDataWA.UserControls.Calendar)this.FindControl(controlId);
        }

        //STAMPA
        protected void SummeriesPrint_Click(object sender, EventArgs e)
        {
            ////SCRIPT di alert
            //string msgDesc = "AlertSummeriesUO";
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);


            #region OLD GFD
            try
            {
                //usato con id univoco per le tabelle dei prospetti
                // DocsPAWA.DocsPaWR.Utente ut = (DocsPAWA.DocsPaWR.Utente)Session["userData"];//OLD
                DocsPaWR.Utente ut = UserManager.GetUserInSession();
                int idPeople = Convert.ToInt32(ut.idPeople);

                // timestamp per i prospetti
                string timeStamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").Replace(".", ":");

                // DocsPAWA.DocsPaWR.InfoUtente infoUtente = DocsPAWA.UserManager.getInfoUtente(this);//OLD
                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

                // DocsPAWA.DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];//OLD
                DocsPaWR.Corrispondente cr = UserManager.getCorrispondenteByIdPeople(this, ut.idPeople, DocsPaWR.AddressbookTipoUtente.INTERNO);// DA VERIFICARE

                infoUtente.idAmministrazione = cr.idAmministrazione;

                DocsPaWR.FileDocumento fileRep = new DocsPaWR.FileDocumento();

                string searchType = "";


                //EXPORT FASCICOLI
                //if (DocsPAWA.UserManager.ruoloIsAutorized(this, "EXP_FASC_COUNT") && (rb_prospettiRiepilogativi.SelectedValue.Equals("reportNumFasc") || rb_prospettiRiepilogativi.SelectedValue.Equals("reportNumDocInFasc")))//OLD
                if (UserManager.IsAuthorizedFunctions("EXP_FASC_COUNT") && (ddl_prospettiRiepilogativi.SelectedValue.Equals("reportNumFasc") || ddl_prospettiRiepilogativi.SelectedValue.Equals("reportNumDocInFasc")))
                {

                    if (controllaCampiObbligatoriExportFasc())
                    {
                        //Session.Remove("filtri");
                        ArrayList filtriTemp = new ArrayList();
                        filtriTemp = (ArrayList)Session["filtri"];

                        if (ddl_prospettiRiepilogativi.SelectedValue.Equals("reportNumFasc"))
                        {

                            UIManager.SummeriesManager.Parametro _tipo_ricerca = new UIManager.SummeriesManager.Parametro("tipo_ricerca", "tipo_ricerca", "reportNumFasc");
                            utility.DO_UpdateParameters(filtri, _tipo_ricerca);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            searchType = "reportNumFasc";
                        }
                        else
                        {
                            UIManager.SummeriesManager.Parametro _tipo_ricerca = new UIManager.SummeriesManager.Parametro("tipo_ricerca", "tipo_ricerca", "reportNumDocInFasc");
                            utility.DO_UpdateParameters(filtri, _tipo_ricerca);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            searchType = "reportNumDocInFasc";
                        }

                        if (this.rb_scelta_sott.SelectedValue.Equals("UO"))
                        {
                            //DocsPAWA.DocsPaWR.Corrispondente corrUo = DocsPAWA.UserManager.getCorrispondenteByCodRubrica(this, this.txt1_corr_sott.Text);//OLD
                            NttDataWA.DocsPaWR.Corrispondente corrUo = NttDataWA.UIManager.AddressBookManager.getCorrispondenteByCodRubrica(this.txt_codCorr.Text, false);//txt1_corr_sott

                            UIManager.SummeriesManager.Parametro _uo = new UIManager.SummeriesManager.Parametro("tipo_scelta", "uo", corrUo.systemId);
                            UIManager.SummeriesManager.Parametro _sottoposti = new UIManager.SummeriesManager.Parametro("sottoposti", "sottoposti", (this.chk_sottoposti.Checked).ToString());
                            UIManager.SummeriesManager.Parametro _nome_scelta = new UIManager.SummeriesManager.Parametro("nome_scelta", "nome_scelta", (this.txt_descrCorr.Text));//txt2_corr_sott
                            utility.DO_UpdateParameters(filtri, _uo);
                            //  Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            utility.DO_UpdateParameters(filtri, _sottoposti);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            utility.DO_UpdateParameters(filtri, _nome_scelta);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];

                        }
                        else
                        {
                            UIManager.SummeriesManager.Parametro _rf = new UIManager.SummeriesManager.Parametro("tipo_scelta", "rf", this.ddl_rf.SelectedValue);
                            UIManager.SummeriesManager.Parametro _nome_scelta = new UIManager.SummeriesManager.Parametro("nome_scelta", "nome_scelta", (this.ddl_rf.SelectedItem.Text));
                            utility.DO_UpdateParameters(filtri, _rf);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            utility.DO_UpdateParameters(filtri, _nome_scelta);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];

                        }

                        //Comuni per tutti e due
                        UIManager.SummeriesManager.Parametro _titolario = new UIManager.SummeriesManager.Parametro("titolario_fasc", "titolario_fasc", this.ddl_titolari.SelectedValue);
                        utility.DO_UpdateParameters(filtri, _titolario);
                        Session["filtri"] = filtri;
                        filtri = (ArrayList)Session["filtri"];

                        //DATA CREAZIONE
                        if (ddl_tipo_data_creazione.SelectedValue.Equals("0") || ddl_tipo_data_creazione.SelectedValue.Equals("2"))
                        {
                            if (ddl_tipo_data_creazione.SelectedValue.Equals("2"))
                            {
                                UIManager.SummeriesManager.Parametro _data_creazioneDal = new UIManager.SummeriesManager.Parametro("dataC", this.cld_creazione_dal.Text, "APERTURA_TODAY");
                                utility.DO_UpdateParameters(filtri, _data_creazioneDal);
                            }

                            if (ddl_tipo_data_creazione.SelectedValue.Equals("4"))
                            {
                                UIManager.SummeriesManager.Parametro _data_creazioneDal = new UIManager.SummeriesManager.Parametro("dataC", this.cld_creazione_dal.Text, "APERTURA_MC");
                                utility.DO_UpdateParameters(filtri, _data_creazioneDal);
                            }

                            if (ddl_tipo_data_creazione.SelectedValue.Equals("3"))
                            {
                                UIManager.SummeriesManager.Parametro _data_creazioneDal = new UIManager.SummeriesManager.Parametro("dataC", this.cld_creazione_dal.Text, "APERTURA_SC");
                                utility.DO_UpdateParameters(filtri, _data_creazioneDal);
                            }

                            if (!ddl_tipo_data_creazione.SelectedValue.Equals("2") && !ddl_tipo_data_creazione.SelectedValue.Equals("3") && !ddl_tipo_data_creazione.SelectedValue.Equals("4"))
                            {
                                UIManager.SummeriesManager.Parametro _data_creazioneDal = new UIManager.SummeriesManager.Parametro("dataC", "dataC", this.cld_creazione_dal.Text);
                                utility.DO_UpdateParameters(filtri, _data_creazioneDal);
                            }


                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                        }
                        if (ddl_tipo_data_creazione.SelectedValue.Equals("1") || ddl_tipo_data_creazione.SelectedValue.Equals("3") || ddl_tipo_data_creazione.SelectedValue.Equals("4"))
                        {
                            UIManager.SummeriesManager.Parametro _data_creazioneDal = new UIManager.SummeriesManager.Parametro("dataCdal", "dataCdal", this.cld_creazione_dal.Text);
                            UIManager.SummeriesManager.Parametro _data_creazioneAl = new UIManager.SummeriesManager.Parametro("dataCal", "dataCal", this.cld_creazione_al.Text);
                            utility.DO_UpdateParameters(filtri, _data_creazioneDal);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            utility.DO_UpdateParameters(filtri, _data_creazioneAl);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                        }

                        //DATA CHIUSURA

                        if (ddl_tipo_data_chiusura.SelectedValue.Equals("0") || ddl_tipo_data_chiusura.SelectedValue.Equals("2"))
                        {
                            UIManager.SummeriesManager.Parametro _data_chiusuraDal = new UIManager.SummeriesManager.Parametro("dataCh", "dataCh", this.cld_chiusura_dal.Text);
                            utility.DO_UpdateParameters(filtri, _data_chiusuraDal);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                        }
                        if (ddl_tipo_data_chiusura.SelectedValue.Equals("1") || ddl_tipo_data_chiusura.SelectedValue.Equals("3") || ddl_tipo_data_chiusura.SelectedValue.Equals("4"))
                        {
                            UIManager.SummeriesManager.Parametro _data_chiusuraDal = new UIManager.SummeriesManager.Parametro("dataChdal", "dataChdal", this.cld_chiusura_dal.Text);
                            UIManager.SummeriesManager.Parametro _data_chiusuraAl = new UIManager.SummeriesManager.Parametro("dataChal", "dataChal", this.cld_chiusura_al.Text);
                            utility.DO_UpdateParameters(filtri, _data_chiusuraDal);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                            utility.DO_UpdateParameters(filtri, _data_chiusuraAl);
                            Session["filtri"] = filtri;
                            filtri = (ArrayList)Session["filtri"];
                        }


                        fileRep = Controller.DO_StampaExcel(filtri, timeStamp, infoUtente, searchType);

                        string tipologia = "XLS";

                        if (fileRep != null)
                        {
                            exportDatiSessionManager sessioneManager = new exportDatiSessionManager();
                            sessioneManager.SetSessionExportFile(fileRep);

                            if (componentType == Constans.TYPE_APPLET)
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "openFile", "OpenFileApplet('" + tipologia + "');", true);
                            else
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "openFile", "OpenFileActiveX('" + tipologia + "');", true);
                        }
                        else
                        {
                            //utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                            //utility.do_openinRightFrame(this, "whitepage.htm");
                            string msgDesc = "AlertSummeriesNoRepo";//Non ci sono dati per il Rapporto selezionato
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                            this.SummeriesZoom.Enabled = false;
                        }
                        Session["filtri"] = filtriTemp;
                    }
                }
                //FINE EXPORT FASCIOLI
                else
                {
                    //ClientScript.RegisterStartupScript(this.GetType(), "wait", "top.principale.frames[1].location='waitingpage.htm';", true);
                    //verifica campi obbligatori (amministrazione,registro,anno)
                    if ((ddl_anno.SelectedValue.Equals("") || ddl_registro.SelectedValue.Equals("") || ddl_amm.SelectedValue.Equals("")) && (this.pnl_DettPratica.Visible == false && this.pnl_protArma.Visible == false && this.pnl_regCC.Visible == false))
                    {
                        //utility.do_alert(this, "Attenzione inserire i campi obbligatori");
                        //utility.do_openinRightFrame(this, "whitepage.htm");

                        string msgDesc = "AlertSummeriesCampiOBB";//Attenzione inserire i campi obbligatori
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                    else
                    {
                        if (this.pnl_DettPratica.Visible == true && string.IsNullOrEmpty(this.txt_NumPratica.Text))
                        {
                            //utility.do_alert(this, "Attenzione inserire i campi obbligatori");
                            //utility.do_openinRightFrame(this, "whitepage.htm");
                            string msgDesc = "AlertSummeriesCampiOBB";//Attenzione inserire i campi obbligatori
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                        else
                        {
                            if (Session["filtri"] != null)
                            {
                                filtri = (ArrayList)Session["filtri"];
                                UIManager.SummeriesManager.Parametro _anno = new UIManager.SummeriesManager.Parametro("anno", "anno", ddl_anno.SelectedValue.ToString());
                                //UIManager.SummeriesManager.Parametro _mod = new UIManager.SummeriesManager.Parametro("mod", "mod", "Compatta");
                                UIManager.SummeriesManager.Parametro _mod = new UIManager.SummeriesManager.Parametro("mod", "mod", rb_modalita.SelectedValue.ToString());
                                //utility.DO_UpdateParameters(filtri, _anno, false);
                                //utility.DO_UpdateParameters(filtri, _mod, false);
                                utility.DO_UpdateParameters(filtri, _anno);
                                utility.DO_UpdateParameters(filtri, _mod);
                                // SEDI
                                if (panel_sede.Visible == true)
                                {
                                    UIManager.SummeriesManager.Parametro _sede = null;
                                    if (ddl_sede.SelectedIndex == 0)
                                        _sede = new UIManager.SummeriesManager.Parametro("sede", ddl_sede.SelectedItem.Text, string.Empty);
                                    else
                                        _sede = new UIManager.SummeriesManager.Parametro("sede", ddl_sede.SelectedItem.Text, ddl_sede.SelectedItem.Value);
                                    utility.DO_UpdateParameters(filtri, _sede);
                                }
                                if (pnl_mese.Visible == true)
                                {
                                    UIManager.SummeriesManager.Parametro _mese = new UIManager.SummeriesManager.Parametro();
                                    _mese.Nome = "mese";
                                    _mese.Valore = ddl_Mese.SelectedValue;
                                    utility.DO_UpdateParameters(filtri, _mese);
                                }
                                if (this.pnl_protArma.Visible == true)
                                {
                                    UIManager.SummeriesManager.Parametro _titolario = new UIManager.SummeriesManager.Parametro("titolario", ddl_et_titolario.SelectedItem.Text, ddl_et_titolario.SelectedValue);
                                    utility.DO_UpdateParameters(filtri, _titolario);
                                }
                                if (pnl_DettPratica.Visible == true)
                                {
                                    string idTitolario = ddl_et_titolario.SelectedValue;
                                    string numProtPtratica = this.txt_NumPratica.Text;
                                    string idReg = this.ddl_registro.SelectedValue;

                                    //DocsPAWA.DocsPaWR.Registro reg = wws.GetRegistroBySistemId(idReg);//OLD
                                    NttDataWA.DocsPaWR.Registro reg = UIManager.RegistryManager.getRegistroBySistemId(idReg);

                                    //ArrayList nodoTitolario = new ArrayList(wws.getNodiFromProtoTit(reg, ((DocsPAWA.DocsPaWR.Utente)Session["userData"]).idAmministrazione, numProtPtratica, idTitolario));//OLD

                                    //ADP 7 maggio 2013

                                    DocsPaWR.Utente utenza = UserManager.GetUserInSession();

                                    ArrayList nodoTitolario = new ArrayList(ClassificationSchemeManager.getNodiFromProtoTitolario(reg, utenza.idAmministrazione, numProtPtratica, idTitolario));

                                    UIManager.SummeriesManager.Parametro _pratica = new UIManager.SummeriesManager.Parametro();//("pratica", numProtPtratica);
                                    _pratica.Nome = "pratica";
                                    _pratica.Valore = numProtPtratica;
                                    UIManager.SummeriesManager.Parametro _classifica = new UIManager.SummeriesManager.Parametro();//("classifica", ((DocsPAWA.DocsPaWR.OrgNodoTitolario)nodoTitolario[0]).Codice);
                                    _classifica.Nome = "classifica";
                                    if (nodoTitolario.Count > 0)
                                    {
                                        _classifica.Valore = ((NttDataWA.DocsPaWR.OrgNodoTitolario)nodoTitolario[0]).Codice;
                                    }
                                    else
                                    {
                                        _classifica.Valore = string.Empty;
                                    }
                                    utility.DO_UpdateParameters(filtri, _pratica);
                                    Session["filtri"] = filtri;
                                    filtri = (ArrayList)Session["filtri"];
                                    utility.DO_UpdateParameters(filtri, _classifica);
                                    Session["filtri"] = filtri;
                                }
                                if (pnl_regCC.Visible == true)
                                {
                                    filtri = (ArrayList)Session["filtri"];
                                    UIManager.SummeriesManager.Parametro _registroCC = new UIManager.SummeriesManager.Parametro();
                                    _registroCC.Nome = "registroCC";
                                    _registroCC.Valore = ddl_regCC.SelectedValue;
                                    _registroCC.Descrizione = ddl_regCC.SelectedItem.Text;
                                    utility.DO_UpdateParameters(filtri, _registroCC);
                                    // DocsPAWA.DocsPaWR.Registro reg = wws.GetRegistroBySistemId(ddl_regCC.SelectedValue);//OLD
                                    NttDataWA.DocsPaWR.Registro reg = UIManager.RegistryManager.getRegistroBySistemId(ddl_regCC.SelectedValue);

                                    UIManager.SummeriesManager.Parametro _regCodice = new UIManager.SummeriesManager.Parametro();
                                    _regCodice.Nome = "codReg";
                                    _regCodice.Descrizione = reg.codRegistro;
                                    utility.DO_UpdateParameters(filtri, _regCodice);
                                }
                                if (this.pnl_DocSpediti.Visible == true)
                                {
                                    this.setParamReportDocSpediti(ut);
                                }
                                if (panel_tit_ann_fasc.Visible == true)
                                {
                                    UIManager.SummeriesManager.Parametro _titolario = new UIManager.SummeriesManager.Parametro("titolario", ddl_titolari_report_annuale.Text, ddl_titolari_report_annuale.SelectedValue);
                                    utility.DO_UpdateParameters(filtri, _titolario);
                                }
                                if (ddl_prospettiRiepilogativi.SelectedValue.Equals("reportAnnualeFasc"))
                                {
                                    // Gabriele Melini 08-05-2014
                                    // il report deve selezionare i fascicoli sul TITOLARIO ATTIVO e non su TUTTI I TITOLARI!!
                                    //UIManager.SummeriesManager.Parametro _titolario = new UIManager.SummeriesManager.Parametro("titolario", "Tutti i titolari", "0");
                                    UIManager.SummeriesManager.Parametro _titolario = new UIManager.SummeriesManager.Parametro("titolario", "Titolario attivo", UIManager.ClassificationSchemeManager.getTitolarioAttivo(Controller.DO_GetIdAmmByCodAmm(this.ddl_amm.SelectedValue).ToString()).ID);
                                    utility.DO_UpdateParameters(filtri, _titolario);
                                }

                                string templateFilePath = "";

                                //elisa 02/02/2006
                                //se l'amministrazione non ha i protocolli interni,
                                //l'aaplicazione prende i template xml senza la colonna dei protocolli interni.

                                // DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();//OLD                          
                                //bool protoInterno = ws.IsInternalProtocolEnabled(cr.idAmministrazione);//OLD

                                bool protoInterno = !string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString())) && Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString()).Equals("1");


                                if (protoInterno)
                                {//ADP  sostituito Frontend con Summeries
                                    templateFilePath = Server.MapPath("../Summaries/TemplateXML/");
                                }
                                else
                                {
                                    templateFilePath = Server.MapPath("../Summaries/TemplateXML_NO_Interni/");
                                }

                                switch (ddl_prospettiRiepilogativi.SelectedValue)
                                {
                                    #region reportAnnualeDoc
                                    case "reportAnnualeDoc":
                                        try
                                        {

                                            if (ddl_Mese.SelectedValue.ToString() != "")
                                            {
                                                templateFilePath += "R_mensileByReg.xml";
                                            }
                                            else
                                            {
                                                templateFilePath += "R_annualeByReg.xml";
                                            }

                                            fileRep = NttDataWA.UIManager.SummeriesManager.Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Annuale_By_Registro, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                            // DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//viene comparato ad un alert??
                                        }

                                        if (fileRep != null)
                                        {
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            //this.SummeriesZoom.Visible = true;
                                            //  utility.setSelectedFileReport(this, fileRep, "");
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                            //this.UpPnlDocumentData.Update();

                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoRepo";//Non ci sono dati per il Rapporto selezionato
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);

                                        }
                                        break;
                                    #endregion

                                    #region reportDocClassificati
                                    case "reportDocClassificati":
                                        try
                                        {
                                            if (rb_modalita.SelectedValue == "Compatta")
                                            {
                                                templateFilePath += "R_DocClassCompact.xml";
                                                fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Documenti_Classificati, filtri, idPeople, timeStamp);
                                            }
                                            else
                                            {
                                                templateFilePath += "R_DocClass.xml";
                                                fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Documenti_Classificati, filtri, idPeople, timeStamp);
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            // DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoRepo";//Non ci sono dati per il Rapporto selezionato
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        }
                                        break;
                                    #endregion

                                    #region reportAnnualeDocTrasmAmm
                                    case "reportAnnualeDocTrasmAmm":
                                        //Documenti trasmessi ad altre amministrazioni
                                        try
                                        {
                                            templateFilePath += "R_DocTrasmToAOO.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Documenti_Trasmessi_Altre_AOO, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            //DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_openinRightFrame(this,"whitepage.htm");
                                            //utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoRepo";//Non ci sono dati per il Rapporto selezionato
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        }
                                        break;
                                    #endregion

                                    #region reportAnnualeFasc
                                    case "reportAnnualeFasc":
                                        try
                                        {
                                            templateFilePath += "R_annualeByFasc.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Annuale_By_Fascicolo, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            // DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoRepo";//Non ci sono dati per il Rapporto selezionato
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        }
                                        break;
                                    #endregion

                                    #region reportAnnualeFascXTit
                                    case "reportAnnualeFascXTit":
                                        try
                                        {
                                            if (rb_modalita.SelectedValue == "Compatta")
                                            {
                                                templateFilePath += "R_FascPerVTCompact.xml";
                                                fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Fascicoli_Per_VT, filtri, idPeople, timeStamp);
                                            }
                                            else
                                            {
                                                templateFilePath += "R_FascPerVT.xml";
                                                fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.Fascicoli_Per_VT, filtri, idPeople, timeStamp);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            //DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoRepo";//Non ci sono dati per il Rapporto selezionato
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        }
                                        break;
                                    #endregion

                                    #region tempiMediLavFasc
                                    case "tempiMediLavFasc":
                                        try
                                        {
                                            if (rb_modalita.SelectedValue != "Compatta")
                                            {
                                                templateFilePath += "R_TempiMediLavFasc.xml";
                                                fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.TempiMediLavFascicoli, filtri, idPeople, timeStamp);
                                            }
                                            else
                                            {
                                                templateFilePath += "R_TempiMediLavFascCompact.xml";
                                                fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.TempiMediLavFascicoli, filtri, idPeople, timeStamp);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            //DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoRepo";//Non ci sono dati per il Rapporto selezionato
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        }
                                        break;
                                    #endregion

                                    #region ReportDocXSede
                                    case "ReportDocXSede":
                                        try
                                        {
                                            templateFilePath += "R_DocXSede.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportDocXSede, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            //DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoRepo";//Non ci sono dati per il Rapporto selezionato
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        }
                                        break;
                                    #endregion

                                    #region ReportDocXUo
                                    case "ReportDocXUo":
                                        try
                                        {
                                            templateFilePath += "R_DocXUo.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportDocXUo, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            //DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }

                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per il Rapporto selezionato");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoRepo";//Non ci sono dati per il Rapporto selezionato
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);

                                        }
                                        break;

                                    #endregion

                                    #region ReportContatoriDoc
                                    case "reportContatoriDocumento":
                                        try
                                        {
                                            templateFilePath += "R_ContatoriDoc.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportContatoriDocumento, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            //DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }
                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per il Rapporto contatori documento");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoData1";//Non ci sono dati per il Rapporto contatori documento
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        }
                                        break;
                                    #endregion

                                    #region ReportContatoriFasc
                                    case "reportContatoriFascicolo":
                                        try
                                        {
                                            templateFilePath += "R_ContatoriFasc.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportContatoriFascicolo, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            // DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }
                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per il Rapporto contatori fascicolo");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoData2";//Non ci sono dati per il Rapporto contatori fascicolo
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        }
                                        break;
                                    #endregion

                                    #region ReportStampaProtArma
                                    case "stampaProtArma":
                                        try
                                        {
                                            templateFilePath += "R_ProtocolloArma.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportProtocolloArma, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            //DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }
                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per la Stampa Protocollo Arma");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoData3";//Non ci sono dati per la Stampa Protocollo Arma
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        }
                                        break;

                                    #endregion

                                    #region ReportDettaglioPratica
                                    case "stampaDettaglioPratica":
                                        try
                                        {
                                            templateFilePath += "R_DettaglioPratica.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportDettaglioPratica, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            // DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }
                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per la Stampa Dettaglio Protocollo");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoData4";//Non ci sono dati per la Stampa Dettaglio Protocollo
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        }
                                        break;
                                    #endregion

                                    #region ReportStampaGiornaleRiscontri
                                    case "stampaGiornaleRiscontri":
                                        try
                                        {
                                            templateFilePath += "R_GiornaleRiscontri.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportGiornaleRiscontri, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            //DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }
                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per la Stampa del Giornale Riscontri");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoData5";//Non ci sono dati per la Stampa del Giornale Riscontri
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        }
                                        break;
                                    #endregion

                                    #region ReportDocumentiSpediti
                                    case "documentiSpediti":
                                        try
                                        {
                                            templateFilePath += "R_DocSpediti.xml";
                                            fileRep = Controller.DO_StampaReport(templateFilePath, ReportDisponibili.ReportDocSpeditiInterop, filtri, idPeople, timeStamp);
                                        }
                                        catch (Exception ex)
                                        {
                                            // DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                                            UIManager.AdministrationManager.DiagnosticError(ex);
                                        }
                                        if (fileRep != null)
                                        {
                                            //utility.setSelectedFileReport(this, fileRep, "");
                                            Session["FileManager.selectedReport"] = fileRep;
                                            this.SummeriesZoom.Enabled = true;
                                            this.frame.Attributes["src"] = "PDFViewer.aspx";
                                            this.UpPnlContentDxSx.Update();
                                            this.UpPnlDocumentData.Visible = true;
                                        }
                                        else
                                        {
                                            //utility.do_alert(this, "Non ci sono dati per la Stampa dei documenti spediti per interoperabilità");
                                            //utility.do_openinRightFrame(this, "whitepage.htm");
                                            string msgDesc = "AlertSummeriesNoData6";//Non ci sono dati per la Stampa dei documenti spediti per interoperabilità
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msgDesc.Replace("'", @"\'") + "', 'warning', '');}", true);
                                        }
                                        break;
                                    #endregion
                                    
                                    //Laura 19 Dicembre
                                    #region reportAnnualeDocObiettivo
                                    case "reportDocInMisOb":
                                        ArrayList filtriTemp = new ArrayList();
                                        filtriTemp = (ArrayList)Session["filtri"];

                                        if (controllaCampiExportOb())
                                        {
                                            if (txtCodiceObiettivo.Text != "" && txtDescrizioneObiettivo.Text != "" && !String.IsNullOrEmpty(hd_idOb.Value.ToString()))
                                            {
                                                UIManager.SummeriesManager.Parametro _id_Obiettivo = new UIManager.SummeriesManager.Parametro("idOb", "idOb", this.hd_idOb.Value.ToString());
                                                utility.DO_UpdateParameters(filtri, _id_Obiettivo);
                                            }
                                            else
                                            {
                                                UIManager.SummeriesManager.Parametro _id_Obiettivo = new UIManager.SummeriesManager.Parametro("idOb", "idOb", "0");
                                                utility.DO_UpdateParameters(filtri, _id_Obiettivo);
                                            }

                                            //Laura 7 febbraio 2013
                                            //ProspettiRiepilogativi.Frontend.Parametro _id_Cdc = new ProspettiRiepilogativi.Frontend.Parametro("idCdc", "idCdc", idUO.Text.ToString());
                                            if (txtCDC.Text != "" && txtdescrCDC.Text != "" && !String.IsNullOrEmpty(hd_IdCorrCDC.Value.ToString()))
                                            {
                                                UIManager.SummeriesManager.Parametro _id_Cdc = new UIManager.SummeriesManager.Parametro("idCdc", "idCdc", hd_IdCorrCDC.Value.ToString());
                                                utility.DO_UpdateParameters(filtri, _id_Cdc);
                                            }
                                            else
                                            {
                                                UIManager.SummeriesManager.Parametro _id_Cdc = new UIManager.SummeriesManager.Parametro("idCdc", "idCdc", "0");
                                                utility.DO_UpdateParameters(filtri, _id_Cdc);
                                            }


                                            //Laura 21 gennaio  eliminazione fase
                                            //if (chkConclusiva.Checked && !chkIntermedia.Checked)
                                            //{
                                            //    ProspettiRiepilogativi.Frontend.Parametro _fase = new ProspettiRiepilogativi.Frontend.Parametro("fase", "fase", "C");
                                            //    utility.DO_UpdateParameters(filtri, _fase);
                                            //}
                                            //else if (!chkConclusiva.Checked && chkIntermedia.Checked)
                                            //{
                                            //    ProspettiRiepilogativi.Frontend.Parametro _fase = new ProspettiRiepilogativi.Frontend.Parametro("fase", "fase", "I");
                                            //    utility.DO_UpdateParameters(filtri, _fase);
                                            //}
                                            //else {
                                            //    ProspettiRiepilogativi.Frontend.Parametro _fase = new ProspettiRiepilogativi.Frontend.Parametro("fase", "fase", "");
                                            //    utility.DO_UpdateParameters(filtri, _fase);
                                            //}

                                            if (ddl_comunicazione.SelectedIndex != 0)
                                            {
                                                UIManager.SummeriesManager.Parametro _id_Com = new UIManager.SummeriesManager.Parametro("idCom", "idCom", ddl_comunicazione.SelectedValue);
                                                utility.DO_UpdateParameters(filtri, _id_Com);
                                            }
                                            else
                                            {
                                                UIManager.SummeriesManager.Parametro _id_Com = new UIManager.SummeriesManager.Parametro("idCom", "idCom", "0");
                                                utility.DO_UpdateParameters(filtri, _id_Com);
                                            }


                                            if (!String.IsNullOrEmpty(hd_systemIdMit.Value))
                                            {
                                                UIManager.SummeriesManager.Parametro _idMitt = new UIManager.SummeriesManager.Parametro("idMitt", "idMitt", hd_systemIdMit.Value.ToString());
                                                utility.DO_UpdateParameters(filtri, _idMitt);
                                                UIManager.SummeriesManager.Parametro _descMitt = new UIManager.SummeriesManager.Parametro("descMitt", "descMitt", "");
                                                utility.DO_UpdateParameters(filtri, _descMitt);
                                            }
                                            else
                                            {
                                                UIManager.SummeriesManager.Parametro _idMitt = new UIManager.SummeriesManager.Parametro("idMitt", "idMitt", "");
                                                utility.DO_UpdateParameters(filtri, _idMitt);
                                                UIManager.SummeriesManager.Parametro _descMitt = new UIManager.SummeriesManager.Parametro("descMitt", "descMitt", txt_DescMit.Text.ToString().ToUpper());
                                                utility.DO_UpdateParameters(filtri, _descMitt);
                                            }

                                            if (!String.IsNullOrEmpty(hd_systemIdDest.Value))
                                            {
                                                UIManager.SummeriesManager.Parametro _idDest = new UIManager.SummeriesManager.Parametro("idDest", "idDest", hd_systemIdDest.Value.ToString());
                                                utility.DO_UpdateParameters(filtri, _idDest);
                                                UIManager.SummeriesManager.Parametro _descDest = new UIManager.SummeriesManager.Parametro("descDest", "descDest", "");
                                                utility.DO_UpdateParameters(filtri, _descDest);
                                            }
                                            else
                                            {
                                                UIManager.SummeriesManager.Parametro _idDest = new UIManager.SummeriesManager.Parametro("idDest", "idDest", "");
                                                utility.DO_UpdateParameters(filtri, _idDest);
                                                UIManager.SummeriesManager.Parametro _descDest = new UIManager.SummeriesManager.Parametro("descDest", "descDest", txt_DescDest.Text.ToString().ToUpper());
                                                utility.DO_UpdateParameters(filtri, _descDest);
                                            }



                                            UIManager.SummeriesManager.Parametro _dataProtDa = new UIManager.SummeriesManager.Parametro("dataProtDa", "dataProtDa", this.dtaCollocation_TxtFrom.Text);
                                            utility.DO_UpdateParameters(filtri, _dataProtDa);

                                            UIManager.SummeriesManager.Parametro _dataProtA = new UIManager.SummeriesManager.Parametro("dataProtA", "dataProtA", this.dtaCollocation_TxtTo.Text);
                                            utility.DO_UpdateParameters(filtri, _dataProtA);


                                            bool count = false;
                                            int tot = 0;
                                            string valoreChiaveDB = Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, "MAX_ROW_SEARCHABLE");
                                            if (valoreChiaveDB.Length == 0)
                                                valoreChiaveDB = Utils.InitConfigurationKeys.GetValue("0", "MAX_ROW_SEARCHABLE");

                                            fileRep = Controller.DO_StampaExcelObiettivi(filtri, timeStamp, infoUtente, searchType, out count, out tot, Convert.ToInt32(valoreChiaveDB));

                                            if (fileRep != null)
                                            {

                                                Session["Estrazione"] = fileRep;//??

                                                //this.iframeVisUnificata.Attributes["src"] = "ReportExternalFile.aspx";

                                                //utility.do_openinRightFrame(this, "whitepage.htm");
                                                this.frame.Attributes["src"] = "../Summaries/PDFViewer.aspx";
                                                this.UpPnlContentDxSx.Update();


                                                this.SummeriesZoom.Enabled = false;

                                            }
                                            else
                                            {
                                                if (count && tot > Convert.ToInt32(valoreChiaveDB))
                                                {
                                                    string msg = "Con i criteri di ricerca inseriti il sistema ha restituito " + tot.ToString() + " elementi ma il numero massimo ammesso è pari a " + valoreChiaveDB + ". E’ necessario affinare la ricerca per procedere.";
                                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorCustom, 'warning', '', '" + utils.FormatJs(msg) + "');", true);
                                                }
                                                else
                                                {
                                                    string msg = "Non ci sono dati per il Rapporto selezionato";
                                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorCustom, 'warning', '', '" + utils.FormatJs(msg) + "');", true);
                                                }

                                            }
                                            Session["filtri"] = filtriTemp;
                                        }
                                        break;
                                    #endregion
                                }
                                //Laura 19 Dicembre
                                if (ddl_prospettiRiepilogativi.SelectedValue != "reportDocInMisOb")
                                {
                                    SummeriesZoom.Visible = true;
                                    this.SummeriesZoom.Enabled = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);//OLD
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            ////Stampa report CDC
            //stampaReportCDC();
            #endregion
        }


        private void setParamReportDocSpediti(NttDataWA.DocsPaWR.Utente ut)
        {
            filtri = (ArrayList)Session["filtri"];
            UIManager.SummeriesManager.Parametro _confermaProt = new UIManager.SummeriesManager.Parametro();
            _confermaProt.Nome = "confermaProt";
            _confermaProt.Valore = rb_confermaSpedizione.SelectedValue;
            utility.DO_UpdateParameters(filtri, _confermaProt);
            UIManager.SummeriesManager.Parametro _dataSpedDa = new UIManager.SummeriesManager.Parametro();
            _dataSpedDa.Nome = "dataSpedDa";
            _dataSpedDa.Valore = this.txt_dataSpedDa.Text;
            utility.DO_UpdateParameters(filtri, _dataSpedDa);
            UIManager.SummeriesManager.Parametro _dataSpedA = new UIManager.SummeriesManager.Parametro();
            _dataSpedA.Nome = "dataSpedA";
            if (this.txt_dataSpedA.Text != string.Empty)
            {
                _dataSpedA.Valore = this.txt_dataSpedA.Text;
            }
            else
            {
                System.DateTime now = System.DateTime.Now;
                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
                _dataSpedA.Valore = now.ToString("dd/MM/yyyy", ci);
                this.txt_dataSpedA.Text = _dataSpedA.Valore;
            }
            utility.DO_UpdateParameters(filtri, _dataSpedA);
            UIManager.SummeriesManager.Parametro _confermeAttese = new UIManager.SummeriesManager.Parametro();
            UIManager.SummeriesManager.Parametro _totDocSpediti = new UIManager.SummeriesManager.Parametro();
            UIManager.SummeriesManager.Parametro _confermeMancanti = new UIManager.SummeriesManager.Parametro();

            switch (_confermaProt.Valore)
            {
                case "1":
                    string confermeAttese = Controller.DO_GetNumDocSpediti(_dataSpedDa.Valore, _dataSpedA.Valore, ddl_regCC.SelectedValue, "1", Convert.ToInt32(ut.idAmministrazione));
                    //ProspettiRiepilogativi.Frontend.Parametro _confermeAttese = new ProspettiRiepilogativi.Frontend.Parametro();                    
                    _confermeAttese.Nome = "confermeAttese";
                    _confermeAttese.Valore = confermeAttese;
                    utility.DO_UpdateParameters(filtri, _confermeAttese);
                    // ProspettiRiepilogativi.Frontend.Parametro _totDocSpediti = new ProspettiRiepilogativi.Frontend.Parametro();
                    _totDocSpediti.Nome = "totDocSpediti";
                    _totDocSpediti.Valore = confermeAttese;
                    utility.DO_UpdateParameters(filtri, _totDocSpediti);
                    //ProspettiRiepilogativi.Frontend.Parametro _confermeMancanti = new ProspettiRiepilogativi.Frontend.Parametro();
                    _confermeMancanti.Nome = "confermeMancanti";
                    _confermeMancanti.Valore = "0";
                    utility.DO_UpdateParameters(filtri, _confermeMancanti);
                    break;
                case "0":
                    string confermaMancanti = Controller.DO_GetNumDocSpediti(_dataSpedDa.Valore, _dataSpedA.Valore, ddl_regCC.SelectedValue, "0", Convert.ToInt32(ut.idAmministrazione));
                    //ProspettiRiepilogativi.Frontend.Parametro _confermeMancanti = new ProspettiRiepilogativi.Frontend.Parametro();
                    _confermeMancanti.Nome = "confermeMancanti";
                    _confermeMancanti.Valore = confermaMancanti;
                    utility.DO_UpdateParameters(filtri, _confermeMancanti);
                    // ProspettiRiepilogativi.Frontend.Parametro _confermeAttese = new ProspettiRiepilogativi.Frontend.Parametro();
                    _confermeAttese.Nome = "confermeAttese";
                    _confermeAttese.Valore = "0";
                    utility.DO_UpdateParameters(filtri, _confermeAttese);
                    // ProspettiRiepilogativi.Frontend.Parametro _totDocSpediti = new ProspettiRiepilogativi.Frontend.Parametro();
                    _totDocSpediti.Nome = "totDocSpediti";
                    _totDocSpediti.Valore = confermaMancanti;
                    utility.DO_UpdateParameters(filtri, _totDocSpediti);
                    break;
                case "T":
                    string totDocumenti = Controller.DO_GetNumDocSpediti(_dataSpedDa.Valore, _dataSpedA.Valore, ddl_regCC.SelectedValue, "T", Convert.ToInt32(ut.idAmministrazione));
                    // ProspettiRiepilogativi.Frontend.Parametro _totDocSpediti = new ProspettiRiepilogativi.Frontend.Parametro();
                    _totDocSpediti.Nome = "totDocSpediti";
                    _totDocSpediti.Valore = totDocumenti;
                    utility.DO_UpdateParameters(filtri, _totDocSpediti);
                    string spedizioni = Controller.DO_GetNumDocSpediti(_dataSpedDa.Valore, _dataSpedA.Valore, ddl_regCC.SelectedValue, "1", Convert.ToInt32(ut.idAmministrazione));
                    // ProspettiRiepilogativi.Frontend.Parametro _confermeAttese = new ProspettiRiepilogativi.Frontend.Parametro();
                    _confermeAttese.Nome = "confermeAttese";
                    _confermeAttese.Valore = spedizioni;
                    utility.DO_UpdateParameters(filtri, _confermeAttese);
                    // ProspettiRiepilogativi.Frontend.Parametro _confermeMancanti = new ProspettiRiepilogativi.Frontend.Parametro();
                    _confermeMancanti.Nome = "confermeMancanti";
                    if (totDocumenti == "")
                        totDocumenti = "0";
                    if (spedizioni == "")
                        spedizioni = "0";
                    _confermeMancanti.Valore = Convert.ToString(Convert.ToInt32(totDocumenti) - Convert.ToInt32(spedizioni));
                    utility.DO_UpdateParameters(filtri, _confermeMancanti);
                    break;
            }

        }

        //ZOOM

        //  //ADP 13 maggio 2013
        //private bool IsZoom
        //{
        //    set
        //    {
        //        HttpContext.Current.Session["isZoom"] = value;
        //    }
        //}

        protected void SummeriesZoom_Click(object sender, EventArgs e)
        {
            try
            {
                //string url = "PDFViewer.aspx";
                //utility.do_windowOpen(this, url);

                //FileManager.setSelectedFile((DocsPaWR.FileRequest)Session["Estrazione"]);
                //FileManager.setSelectedFile((DocsPaWR.FileRequest)Session["FileManager.selectedReport"]);

                //FileManager.setSelectedReport((NttDataWA.DocsPaWR.FileDocumento)Session["FileManager.selectedReport"]);

                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "PDFViewer", "ajaxModalPopupPDFViewer();", true);

                //IsZoom = true;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupDocumentViewerSummeries", "ajaxModalPopupDocumentViewerSummeries();", true);

            }
            catch (Exception ex)
            {
                // DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

        }

        protected void btn_obiettivo_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void btn_CDC_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void txt_cod_obiettivo_TextChanged(object sender, EventArgs e)
        {

        }

        protected void txt_cod_costo_TextChanged(object sender, EventArgs e)
        {

        }


        private bool controllaCampiExportOb()
        {
            if (this.dtaCollocation_TxtFrom.Text != "" && !utils.isDate(this.dtaCollocation_TxtFrom.Text))
            {
                string msg = "Attenzione! Formato data protocollazione errato";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorCustom, 'warning', '', '" + utils.FormatJs(msg) + "');", true);
                return false;
            }

            if (this.dtaCollocation_TxtTo.Text != "" && !utils.isDate(this.dtaCollocation_TxtTo.Text))
            {
                string msg = "Attenzione! Formato data protocollazione errato";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorCustom, 'warning', '', '" + utils.FormatJs(msg) + "');", true);
                return false;
            }

            if (verificaIntervalloDate(this.dtaCollocation_TxtFrom.Text, this.dtaCollocation_TxtTo.Text))
            {
                string msg = "L'intervallo temporale selezionato per la data di protocollazione non è coerente";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "dialog", "ajaxDialogModal('ErrorCustom, 'warning', '', '" + utils.FormatJs(msg) + "');", true);
                return false;
            }

            return true;
        }

        protected void txt_CodMit_TextChanged(object sender, EventArgs e)
        {

        }

        public static bool verificaIntervalloDate(string dataUno, string dataDue)
        {
            if (dataUno != "" && dataDue != "")
            {
                try
                {
                    dataUno = dataUno.Trim();
                    dataDue = dataDue.Trim();
                    CultureInfo ci = new CultureInfo("it-IT");
                    string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };
                    DateTime d_Uno = DateTime.ParseExact(dataUno, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                    DateTime d_Due = DateTime.ParseExact(dataDue, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

                    if (d_Uno > d_Due)
                        return true;
                    else
                        return false;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            else
            {
                return false;
            }
        }

        protected void ddl_registro_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddl_registro.SelectedIndex != 0)
                {
                    //Laura 19 Dicembre
                    if (ddl_prospettiRiepilogativi.SelectedValue == "reportDocInMisOb")
                    {
                        UserManager.setRegistroSelezionato(this, RegistryManager.getRegistroBySistemId(ddl_registro.SelectedItem.Value));
                    }
                    UIManager.SummeriesManager.Parametro reg = new UIManager.SummeriesManager.Parametro("reg", ddl_registro.SelectedItem.Text, ddl_registro.SelectedItem.Value);

                    if (Session["filtri"] != null)
                    {
                        filtri = (ArrayList)Session["filtri"];
                        utility.DO_UpdateParameters(filtri, reg);
                        Session["filtri"] = filtri;
                    }
                    int idReg = Convert.ToInt32(ddl_registro.SelectedValue);
                    ddl_anno.Items.Clear();
                    Controller.DO_GetAnniProfilazione(ddl_anno, idReg);
                    // verifico presenza Sedi, se presenti abilito la combobox e aggiungo
                    //il filtro di ricerca.
                    bool popolata = false;
                    if (panel_sede.Visible == true)
                    {
                        ddl_sede.Items.Clear();
                        Controller.DO_GetSedi(Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(ddl_amm.SelectedValue)), ddl_sede, out popolata);
                        utility.SetFocus(this, ddl_sede);
                    }
                    else
                    {
                        utility.SetFocus(this, ddl_anno);
                    }
                }
                else
                {
                    //Laura 19 Dicembre
                    if (ddl_prospettiRiepilogativi.SelectedValue == "reportDocInMisOb")
                    {
                        UIManager.SummeriesManager.Parametro reg = new UIManager.SummeriesManager.Parametro("reg", "ALL", "ALL");
                        if (Session["filtri"] != null)
                        {
                            filtri = (ArrayList)Session["filtri"];
                            utility.DO_UpdateParameters(filtri, reg);
                            Session["filtri"] = filtri;
                        }
                    }
                    else
                    {
                        ddl_anno.Items.Clear();
                    }

                    ddl_sede.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                //DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        public static string httpFullPath
        {
            get
            {
                return utils.getHttpFullPath();
            }
        }

    }
}