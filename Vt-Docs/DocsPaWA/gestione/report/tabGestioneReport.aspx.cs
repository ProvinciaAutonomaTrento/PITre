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
using log4net;

namespace DocsPAWA.gestione.report
{
    /// <summary>
    /// Summary description for tabGestioneReport.
    /// </summary>
    public class tabGestioneReport : DocsPAWA.CssPage
    {
        private ILog logger = LogManager.GetLogger(typeof(tabGestioneReport));
        private DocsPAWA.DocsPaWR.Registro[] userRegistri;
        protected DocsPAWA.DocsPaWR.RagioneTrasmissione[] listaRagioni;
        protected Hashtable m_hashTableRagioneTrasmissione;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        //protected DocsPaWebCtrlLibrary.DateMask txt_initDataTrasm;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineDataTrasm;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineDataProt_E;
        //protected DocsPaWebCtrlLibrary.DateMask txt_initDataProt_E;
        //protected DocsPaWebCtrlLibrary.DateMask txt_initDataProt_B;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineDataProt_B;
        protected DocsPAWA.gestione.report.CalendarReport txt_initDataTrasm;
        protected DocsPAWA.gestione.report.CalendarReport txt_fineDataTrasm;
        protected DocsPAWA.gestione.report.CalendarReport txt_fineDataProt_E;
        protected DocsPAWA.gestione.report.CalendarReport txt_initDataProt_E;
        protected DocsPAWA.gestione.report.CalendarReport txt_initDataProt_B;
        protected DocsPAWA.gestione.report.CalendarReport txt_fineDataProt_B;
        protected DocsPaWebCtrlLibrary.ImageButton btn_stampaRegistro;
        protected DocsPaWebCtrlLibrary.ImageButton btn_stampaRegistroDisabled;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_rep_av_ruolo;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_rep_av_rag_trasm;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_rep_av_data;
        protected System.Web.UI.WebControls.DropDownList ddl_rep_av_data;
        //GIORDANO IACOZZILLI 14/03/2013
        //VARIABILI PER NUOVO FILTRO REPORT
        protected System.Web.UI.HtmlControls.HtmlTableCell tdtxt_giorni_trascorsi;
        protected System.Web.UI.WebControls.TextBox txt_giorni_trascorsi;
        protected System.Web.UI.WebControls.RegularExpressionValidator Regtxt_giorni_trascorsi;
        protected System.Web.UI.WebControls.RangeValidator VRVtxt_giorni_trascorsi;
        //FINE GIORDANO IACOZZILLI 14/03/2013
        protected System.Web.UI.WebControls.Label lbl_rep_av_da;
        protected System.Web.UI.WebControls.Label lbl_rep_av_a;
        protected System.Web.UI.WebControls.DropDownList ddl_rep_av_ruolo;
        protected System.Web.UI.WebControls.DropDownList ddl_rep_av_rag_trasm;
        //protected System.Web.UI.WebControls.TextBox txt_rep_av_initData;
        //protected System.Web.UI.WebControls.TextBox txt_rep_av_fineData;
        protected System.Web.UI.WebControls.Label lbl_registri;
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.Label lbl_ET_stato;
        protected System.Web.UI.WebControls.Image icoReg;
        protected System.Web.UI.WebControls.Image img_statoReg;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        protected System.Web.UI.WebControls.DropDownList ddl_report;
        protected System.Web.UI.WebControls.Label lblInput;
        protected System.Web.UI.WebControls.TextBox txtInput;
        protected System.Web.UI.WebControls.Panel pnlInput;
        protected System.Web.UI.WebControls.DropDownList DDLOggettoTab1;
        protected System.Web.UI.WebControls.DropDownList ddl_dataTrasm;
        protected System.Web.UI.WebControls.Label lbl_initdataTrasm;
        protected System.Web.UI.WebControls.Label lbl_finedataTrasm;
        protected System.Web.UI.WebControls.DropDownList ddl_ragioni;
        protected System.Web.UI.WebControls.Panel pnl_trasmUO;
        protected System.Web.UI.WebControls.DropDownList ddl_numProt_E;
        protected System.Web.UI.WebControls.Label lblDAnumprot_E;
        protected System.Web.UI.WebControls.TextBox txt_initNumProt_E;
        protected System.Web.UI.WebControls.Label lblAnumprot_E;
        protected System.Web.UI.WebControls.TextBox txt_fineNumProt_E;
        protected System.Web.UI.WebControls.DropDownList ddl_dataProt_E;
        protected System.Web.UI.WebControls.Label lbl_initdataProt_E;
        protected System.Web.UI.WebControls.Label lbl_finedataProt_E;
        protected System.Web.UI.WebControls.TextBox txt_codUO;
        protected System.Web.UI.WebControls.TextBox txt_descUO;
        protected System.Web.UI.WebControls.Panel pnl_DocumentiRegistro;
        protected System.Web.UI.WebControls.TextBox txt_Anno_B;
        protected System.Web.UI.WebControls.DropDownList ddl_numProt_B;
        protected System.Web.UI.WebControls.Label lblDAnumprot_B;
        protected System.Web.UI.WebControls.TextBox txt_initNumProt_B;
        protected System.Web.UI.WebControls.Label lblAnumprot_B;
        protected System.Web.UI.WebControls.TextBox txt_fineNumProt_B;
        protected System.Web.UI.WebControls.DropDownList ddl_dataProt_B;
        protected System.Web.UI.WebControls.Label lbl_initdataProt_B;
        protected System.Web.UI.WebControls.Label lbl_finedataProt_B;
        protected System.Web.UI.WebControls.Panel pnl_StampaBuste;
        protected System.Web.UI.HtmlControls.HtmlImage btn_Rubrica_E;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_systemIdUo;
        protected System.Web.UI.WebControls.CheckBox chk_uo;
        protected System.Web.UI.WebControls.Panel pnl_reportAvanzati;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoAttoDR;
        protected System.Web.UI.WebControls.ImageButton btn_CampiPersonalizzatiDR;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoAttoDG;
        protected System.Web.UI.WebControls.ImageButton btn_CampiPersonalizzatiDG;
        protected System.Web.UI.WebControls.Panel pnl_DocumentiGrigi;
        protected System.Web.UI.WebControls.DropDownList ddl_idDoc_E;
        protected System.Web.UI.WebControls.Label lbl_initIdDoc_E;
        protected System.Web.UI.WebControls.TextBox txt_initIdDoc_E;
        protected System.Web.UI.WebControls.Label lbl_fineIdDoc_E;
        protected System.Web.UI.WebControls.TextBox txt_fineIdDoc_E;
        protected System.Web.UI.WebControls.DropDownList ddl_dataCreazioneG_E;
        protected System.Web.UI.WebControls.Label lbl_initDataCreazioneG_E;
        //protected System.Web.UI.WebControls.TextBox txt_initDataCreazioneG_E;
        protected System.Web.UI.WebControls.Label lbl_fineDataCreazioneG_E;
        //protected System.Web.UI.WebControls.TextBox txt_fineDataCreazioneG_E;
        protected DocsPAWA.gestione.report.CalendarReport txt_initDataCreazioneG_E;
        protected DocsPAWA.gestione.report.CalendarReport txt_fineDataCreazioneG_E;
        protected DocsPAWA.gestione.report.CalendarReport txt_rep_av_initData;
        protected DocsPAWA.gestione.report.CalendarReport txt_rep_av_fineData;

        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                this.RegisterClientScript("nascondi", "nascondi();");

                userRegistri = UserManager.getListaRegistri(this);

                if (ConfigSettings.getKey(ConfigSettings.KeysENUM.STAMPA_BUSTE) == null || ConfigSettings.getKey(ConfigSettings.KeysENUM.STAMPA_BUSTE).Equals("0"))
                {
                    for (int i = 0; i < this.ddl_report.Items.Count; i++)
                        if (this.ddl_report.Items[i].Value.Equals("B"))
                            this.ddl_report.Items.Remove(this.ddl_report.Items[i]);
                }

                if (!IsPostBack)
                {
                    this.CaricaComboRegistri(ddl_registri);

                    //carica il ruolo scelto
                    DocsPaWR.Ruolo ruolo = UserManager.getRuolo(this);
                    this.getAutorizzazioniReport(ruolo);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        #region Report Avanzati
        /// <summary>
        /// GADAMO - 22/05/2008
        /// Nuova implementazione per la reportistica su richiesta da Ministero della Salute
        /// estendibile a chiunque tramite le microfunzioni:
        ///     1 - DO_TRASM_RUOLO_STAMPA
        ///     2 - DO_TRASM_PEND_STAMPA
        ///     3 - DO_PROT_REG_STAMPA
        ///     4 - DO_PROT_REG_RUOLO_STAMPA
        ///     AGGIUNTI DA IACOZZILLI GIORDANO PER PAOLO CUFFARI 11/03/2013
        ///     5 - DO_PR_FA_CL_NNCL_REG_STAMPA
        ///     6 - DO_INTEROP_REG_STAMPA
        ///     7 - DO_PEC_REG_STAMPA
        ///     
        /// </summary>
        private void getAutorizzazioniReport(DocsPaWR.Ruolo ruolo)
        {
            ListItem newItem;
            string[] arrFunzione = new string[3];
            //***************************************************************************************
            // Mev: Giordano Iacozzilli
            // Aggiunta altri 4 Report per il ministero della salute:
            // 18/02/2013.
            //Old code:
            //for (int i = 1; i < 5; i++)
            //{
            //    arrFunzione = this.datiFunzioneAvanzata(i);

            //    if (UserManager.ruoloIsAutorized(this, arrFunzione[0]))
            //    {
            //        newItem = new ListItem();
            //        newItem.Text = arrFunzione[1];
            //        newItem.Value = arrFunzione[2];
            //        this.ddl_report.Items.Add(newItem);
            //    }
            //}
            //
            //New CODE:
            for (int i = 1; i < 10; i++)
            {
                arrFunzione = this.datiFunzioneAvanzata(i);

                if (UserManager.ruoloIsAutorized(this, arrFunzione[0]))
                {
                    newItem = new ListItem();
                    newItem.Text = arrFunzione[1];
                    newItem.Value = arrFunzione[2];
                    this.ddl_report.Items.Add(newItem);
                }
            }
            //***************************************************************************************
        }

        /// <summary>
        /// GADAMO - 22/05/2008
        /// Fornisce i dati dei nuovi quattro tipi di report avanzati
        /// </summary>
        /// <param name="index">indice ricorsivo</param>
        /// <returns></returns>
        private string[] datiFunzioneAvanzata(int index)
        {
            string[] retValue = new string[3];

            switch (index)
            {
                case 1:
                    retValue[0] = "DO_TRASM_RUOLO_STAMPA";
                    retValue[1] = "Numero di trasmissioni effettuate da ruolo con dettaglio utente";
                    retValue[2] = "TX_R";
                    break;
                case 2:
                    retValue[0] = "DO_TRASM_PEND_STAMPA";
                    retValue[1] = "Numero di trasmissioni pendenti con dettaglio del ruolo";
                    retValue[2] = "TX_P";
                    break;
                case 3:
                    retValue[0] = "DO_PROT_REG_STAMPA";
                    retValue[1] = "Numero di documenti protocollati per registro";
                    retValue[2] = "PR_REG";
                    break;
                case 4:
                    retValue[0] = "DO_PROT_REG_RUOLO_STAMPA";
                    retValue[1] = "Numero di documenti protocollati per registro con dettaglio ruolo";
                    retValue[2] = "PR_REG_R";
                    break;
                //***************************************************************************************
                // Mev: Giordano Iacozzilli
                // Aggiunta altri 5 Report per il ministero della salute:
                // 18/02/2013.
                // aggiungo che questa gestione delle stampe è lontanissima dall'essere parametrica.
                case 5:
                    retValue[0] = "DO_PR_FA_CL_NNCL_REG_STAMPA";
                    retValue[1] = "Numero di documenti fascicolati per registro";
                    retValue[2] = "PFCNC_REG_R";
                    break;
                case 6:
                    retValue[0] = "DO_INTEROP_REG_STAMPA";
                    retValue[1] = "Numero di documenti scambiati tramite interoperabilita";
                    retValue[2] = "INTEROP_REG";
                    break;
                case 7:
                    retValue[0] = "DO_PEC_REG_STAMPA";
                    retValue[1] = "Numero di documenti arrivati via mail e protocollati";
                    retValue[2] = "PEC_REG";
                    break;
                case 8:
                    retValue[0] = "DO_TRASM_RIF_REG_STAMPA";
                    retValue[1] = "Numero di trasmissioni effettuate da ruolo con rifiuti";
                    retValue[2] = "TRASM_EVI_RIF_REG";
                    break;
                case 9:
                    retValue[0] = "DO_TRASM_RIC_RUOLO_STAMPA";
                    retValue[1] = "Numero di trasmissioni ricevute da ruolo con accettazioni";
                    retValue[2] = "TX_R_RIC";
                    break;
                //***************************************************************************************
            }

            return retValue;
        }

        /// <summary>
        /// GADAMO - 22/05/2008
        /// Gestione della modifica della voce della combo-box conn la lista dei report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddl_report_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //rimuove da sessione eventuale selezione uo per stampa registri uo.
                System.Web.HttpContext.Current.Session.Remove("corrStampaUo");
                //GIORDANO IACOZZILLI 14/03/2013
                //Resetto la mia textbox per l'aggiunta di giorni X alla data di arrivo pec.
                txt_giorni_trascorsi.Text = "1";
                //FINE
                string tipoReport = this.ddl_report.SelectedValue;
                switch (tipoReport)
                {
                    case "F": // Fascette Fascicolo
                        this.pnlInput.Visible = true;
                        this.pnl_DocumentiRegistro.Visible = false;
                        this.pnl_StampaBuste.Visible = false;
                        this.pnl_trasmUO.Visible = false;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = false;
                        this.GetCalendarControl("txt_fineDataTrasm").Visible = false;
                        this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = false;
                        this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = false;
                        this.lbl_finedataTrasm.Visible = false;
                        this.lbl_initdataTrasm.Visible = false;
                        this.toglieVociTuttiRegistri();
                        break;
                    case "TR": // Trasmissioni UO
                        this.pnlInput.Visible = false;
                        this.pnl_DocumentiRegistro.Visible = false;
                        this.pnl_StampaBuste.Visible = false;
                        this.pnl_trasmUO.Visible = true;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = false;
                        this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text = DateTime.Now.ToShortDateString().ToString();
                        this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text = "";
                        this.ddl_dataTrasm.SelectedIndex = 0;
                        this.GetCalendarControl("txt_fineDataTrasm").Visible = false;
                        this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = false;
                        this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = false;
                        this.lbl_finedataTrasm.Visible = false;
                        this.lbl_initdataTrasm.Visible = false;
                        this.toglieVociTuttiRegistri();
                        this.CaricaRagioni();
                        break;
                    case "DR": // Documenti Registro
                        this.pnlInput.Visible = false;
                        this.pnl_trasmUO.Visible = false;
                        this.pnl_StampaBuste.Visible = false;
                        this.pnl_DocumentiRegistro.Visible = true;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = false;
                        this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = DateTime.Now.ToShortDateString().ToString();
                        CaricaComboTipologiaAtto(ddl_tipoAttoDR);
                        // Aggiornamento visibilità campi filtro
                        this.RefreshFiltersCtlsNumProtocollo();
                        this.RefreshFiltersCtlsDataProtocollo();
                        this.toglieVociTuttiRegistri();
                        break;
                    case "B": // Stampa Buste
                        this.pnlInput.Visible = false;
                        this.pnl_trasmUO.Visible = false;
                        this.pnl_DocumentiRegistro.Visible = false;
                        this.pnl_StampaBuste.Visible = true;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = false;
                        this.GetCalendarControl("txt_initDataProt_B").txt_Data.Text = DateTime.Now.ToShortDateString().ToString();
                        // Aggiornamento visibilità campi filtro
                        this.RefreshFiltersCtlsNumProtocollo_B();
                        this.RefreshFiltersCtlsDataProtocollo_B();
                        this.toglieVociTuttiRegistri();
                        break;
                    //***************************************************************************************
                    // Mev: Giordano Iacozzilli
                    // Aggiunta altri 5 Report per il ministero della salute:
                    // 15/03/2013.
                    case "TRASM_EVI_RIF_REG":
                    case "TX_R_RIC":
                    //***************************************************************************************
                    case "TX_R":
                    case "TX_P":
                    case "PR_REG":
                    case "PR_REG_R":
                        this.inserisceVoceTuttiRegistri();
                        this.impostaHtmlVisibile(tipoReport);
                        this.CaricaRagioniPerRicerca();
                        this.CaricaListaRuoliRepAv();
                        break;
                    //***************************************************************************************
                    // Mev: Giordano Iacozzilli
                    // Aggiunta altri 4 Report per il ministero della salute:
                    // 18/02/2013.
                    case "PFCNC_REG_R":
                    case "PEC_REG":
                    case "INTEROP_REG":
                        this.inserisceVoceTuttiRegistri();
                        this.impostaHtmlVisibile(tipoReport);
                        break;
                    //***************************************************************************************
                    case "DG": // Documenti Registro
                        this.pnlInput.Visible = false;
                        this.pnl_trasmUO.Visible = false;
                        this.pnl_DocumentiRegistro.Visible = false;
                        this.pnl_StampaBuste.Visible = false;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = true;
                        this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text = DateTime.Now.ToShortDateString().ToString();
                        CaricaComboTipologiaAtto(ddl_tipoAttoDG);
                        // Aggiornamento visibilità campi filtro
                        this.RefreshFiltersCtlsIdDoc();
                        this.RefreshFiltersCtlsDataCreazioneG();
                        this.toglieVociTuttiRegistri();
                        break;
                    case "E": //Corrispondenti esterni
                        this.inserisceVoceTuttiRegistri();
                        this.pnlInput.Visible = false;
                        this.pnl_trasmUO.Visible = false;
                        this.pnl_DocumentiRegistro.Visible = false;
                        this.pnl_StampaBuste.Visible = false;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = false;
                        break;
                    default:
                        this.pnlInput.Visible = false;
                        this.pnl_trasmUO.Visible = false;
                        this.pnl_DocumentiRegistro.Visible = false;
                        this.pnl_StampaBuste.Visible = false;
                        this.pnl_reportAvanzati.Visible = false;
                        this.pnl_DocumentiGrigi.Visible = false;
                        this.toglieVociTuttiRegistri();
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        /// <summary>
        /// GADAMO - 22/05/2008
        /// Imposta gli oggetti HTML visibili all'utente dopo la selezione del tipo report
        /// </summary>
        /// <param name="tipo">tipo funzione (tipo report)</param>
        private void impostaHtmlVisibile(string tipo)
        {
            //GIORDANO IACOZZILLI 14/03/2013
            //aggiunta una TD per un filtro.
            this.tdtxt_giorni_trascorsi.Visible = false;
            this.pnlInput.Visible = false;
            this.pnl_trasmUO.Visible = false;
            this.pnl_DocumentiRegistro.Visible = false;
            this.pnl_StampaBuste.Visible = false;
            this.pnl_DocumentiGrigi.Visible = false;
            this.pnl_reportAvanzati.Visible = true;

            this.td_rep_av_ruolo.Visible = true;
            this.td_rep_av_rag_trasm.Visible = true;
            this.td_rep_av_data.Visible = true;

            this.lbl_rep_av_da.Visible = true;
            this.GetCalendarControl("txt_rep_av_initData").Visible = true;
            this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text = "01/01/" + DateTime.Now.Year.ToString();

            this.lbl_rep_av_a.Visible = true;
            this.GetCalendarControl("txt_rep_av_fineData").Visible = true;
            this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text = DateTime.Now.ToShortDateString().ToString(); ;

            this.ddl_rep_av_ruolo.Visible = true;
            this.ddl_rep_av_rag_trasm.Visible = true;

            switch (tipo)
            {
                case "PR_REG":
                    this.td_rep_av_rag_trasm.Visible = false;
                    this.td_rep_av_ruolo.Visible = false;
                    break;
                case "PR_REG_R":
                    this.td_rep_av_rag_trasm.Visible = false;
                    break;
                //***************************************************************************************
                // Mev: Giordano Iacozzilli
                // Aggiunta altri 4 Report per il ministero della salute:
                // 18/02/2013.
                case "PFCNC_REG_R":
                    this.td_rep_av_rag_trasm.Visible = false;
                    this.td_rep_av_ruolo.Visible = false;
                    break;
                case "INTEROP_REG":
                    this.td_rep_av_rag_trasm.Visible = false;
                    this.td_rep_av_ruolo.Visible = false;
                    break;
                case "PEC_REG":
                    this.tdtxt_giorni_trascorsi.Visible = true;
                    this.td_rep_av_rag_trasm.Visible = false;
                    this.td_rep_av_ruolo.Visible = false;
                    break;
                //***************************************************************************************
            }
        }

        /// <summary>
        /// GADAMO - 22/05/2008
        /// Permette l'inserimento della voce "Tutti" nella combo-box dei registri
        /// quando si è selezionato uno dei nuovi quattro tipi di report avanzati
        /// </summary>
        private void inserisceVoceTuttiRegistri()
        {
            string valoreTutti = string.Empty;
            ListItem itemTutti;

            if (this.ddl_registri.Items.FindByText("Tutti") == null)
            {
                if (this.ddl_registri.Items.Count > 1)
                {
                    for (int i = 0; i < this.ddl_registri.Items.Count; i++)
                        valoreTutti += this.ddl_registri.Items[i].Value + "_";

                    valoreTutti = valoreTutti.Substring(0, valoreTutti.Length - 1);

                    itemTutti = new ListItem();
                    itemTutti.Text = "Tutti";
                    itemTutti.Value = valoreTutti;
                    this.ddl_registri.Items.Add(itemTutti);
                }
            }

        }

        /// <summary>
        /// GADAMO - 22/05/2008
        /// Elimina la voce "Tutti" nella combo-box dei registri quando viene
        /// selezionato un tipo di report di default (no avanzato)
        /// </summary>
        private void toglieVociTuttiRegistri()
        {
            if (this.ddl_registri.Items.FindByText("Tutti") != null)
                this.ddl_registri.Items.RemoveAt(this.ddl_registri.Items.Count - 1);
        }

        /// <summary>
        /// GADAMO - 22/05/2008
        /// Imposta la sessione con tutti gli ID dei registri disponibili nella
        /// combo-box dei registri
        /// </summary>
        private void setTuttiRegistri()
        {
            string[] listaIDRegistri;
            string listaIDRegCombo = string.Empty;

            try
            {
                listaIDRegCombo = this.ddl_registri.SelectedValue;

                listaIDRegistri = listaIDRegCombo.Split('_');

                UserManager.setListaIdRegistri(this, listaIDRegistri);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        /// <summary>
        /// GADAMO - 22/05/2008
        /// Imposta l'HTML visibile all'utente dopo la scelta del periodo di riferimento    
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddl_rep_av_data_SelectedIndexChanged(object sender, EventArgs e)
        {
            // valore singolo
            if (this.ddl_rep_av_data.SelectedIndex == 0)
            {
                this.lbl_rep_av_da.Visible = true;
                this.lbl_rep_av_a.Visible = false;
                this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text = System.DateTime.Now.ToShortDateString();
                //Giordano Iacozzilli 20/02/2013:
                //Bug: passando da valore singolo ad intervallo non visualizzava più la fineData.
                //Old code:
                // this.GetCalendarControl("txt_rep_av_fineData").Visible = false;
                //New Code:
                this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Visible = false;
                this.GetCalendarControl("txt_rep_av_fineData").btn_Cal.Visible = false;
            }
            else // intervallo di tempo
            {
                this.lbl_rep_av_da.Visible = true;
                this.lbl_rep_av_a.Visible = true;
                this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text = "01/01/" + System.DateTime.Now.Year.ToString();
                //Giordano Iacozzilli 20/02/2013:
                //Bug: passando da valore singolo ad intervallo non visualizzava più la fineData.
                //Old code:
                // this.GetCalendarControl("txt_rep_av_fineData").Visible = true;
                //New code:
                this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Visible = true;
                this.GetCalendarControl("txt_rep_av_fineData").btn_Cal.Visible = true;
                this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text = System.DateTime.Now.ToShortDateString().ToString();
            }
        }

        /// <summary>
        /// GADAMO - 22/05/2008
        /// Reperimento dei tipi ruolo in amministrazione corrente
        /// </summary>
        /// <returns>Array oggetti Tipo Ruoli</returns>
        private DocsPAWA.DocsPaWR.OrgTipoRuolo[] getTipiRuolo()
        {
            userRegistri = UserManager.getListaRegistri(this);

            string codiceAmministrazione = userRegistri[0].codAmministrazione;

            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            return ws.AmmGetTipiRuolo(codiceAmministrazione);
        }

        /// <summary>
        /// GADAMO - 22/05/2008
        /// Popolamento della combo-box dei ruoli
        /// </summary>
        private void CaricaListaRuoliRepAv()
        {
            ListItem newItem;

            switch (this.ddl_report.SelectedValue)
            {
                //GIORDANO IACOZZILLI 15/03/2013
                //Nuovi report per Cuffari (report 4)
                //devo caricare i tipi ruolo.
                case "TRASM_EVI_RIF_REG":
                case "TX_R_RIC":
                //FINE GIORDANO IACOZZILLI 15/03/2013
                case "TX_R":
                case "TX_P":
                case "PR_REG_R":
                    this.ddl_rep_av_ruolo.Items.Clear();

                    newItem = new ListItem("", "0");
                    this.ddl_rep_av_ruolo.Items.Add(newItem);

                    DocsPAWA.DocsPaWR.OrgTipoRuolo[] ruoli = this.getTipiRuolo();
                    foreach (DocsPAWA.DocsPaWR.OrgTipoRuolo ruolo in ruoli)
                    {
                        newItem = new ListItem(ruolo.Descrizione.ToUpper(), ruolo.IDTipoRuolo);
                        this.ddl_rep_av_ruolo.Items.Add(newItem);
                    }

                    this.ddl_rep_av_ruolo.SelectedIndex = 0;
                    break;
            }
        }

        /// <summary>
        /// GADAMO - 22/05/2008
        /// verifica i filtri impostati;
        /// reperisce i dati del report;
        /// imposta l'oggetto stampaReportXLS
        /// chiama la pagina che visualizza il report 
        /// </summary>
        private void impostaDatiRepAv()
        {
            try
            {
                // verifica le date inserite
                if (
                        (this.ddl_rep_av_data.SelectedValue.Equals("0") &&  // è stato selezionato 'valore singolo'
                         this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text.Trim() != "" &&      // ed il campo data non è vuoto
                         Utils.isDate(this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text))       // ed è un formato data...
                         ||                                                 //                              ... oppure
                        (this.ddl_rep_av_data.SelectedValue.Equals("1") &&                                                  // è stato selezionato 'intervallo'
                         ((this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text.Trim() != "" && Utils.isDate(this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text)) &&    // ed il campo data 'Da' non è vuoto ed è un formato data
                          (this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text.Trim() != "" && Utils.isDate(this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text))       // ed il campo data 'A' non è vuoto ed è un formato data
                         )
                        )
                    )
                {
                    string tipoReport = this.ddl_report.SelectedValue;

                    Ruolo ruolo = UserManager.getRuolo(this);
                    ExportExcelClass objReport = new ExportExcelClass();
                    ExportDataFilterExcel objFilter = new ExportDataFilterExcel();

                    objFilter.tipologiaReport = tipoReport;
                    objFilter.idRegistro = this.ddl_registri.SelectedValue;
                    objFilter.idAmministrazione = ruolo.idAmministrazione;

                    // memorizza i filtri
                    switch (tipoReport)
                    {
                        //*****************************************************************************
                        //Giordano Iacozzilli
                        //20/02/2013
                        //MEV: Aggiunti altri 5 report per il ministero della salute.
                        case "TRASM_EVI_RIF_REG":
                        case "TX_R_RIC":
                        //*****************************************************************************
                        case "TX_R":
                        case "TX_P":
                            objFilter.idRuolo = this.ddl_rep_av_ruolo.SelectedValue;
                            objFilter.idRagTrasm = this.ddl_rep_av_rag_trasm.SelectedValue;
                            if (this.GetCalendarControl("txt_rep_av_initData").Visible)
                                objFilter.dataDa = this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text;
                            if (this.GetCalendarControl("txt_rep_av_fineData").Visible)
                                objFilter.dataA = this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text;
                            break;
                        //*****************************************************************************
                        //Giordano Iacozzilli
                        //20/02/2013
                        //MEV: Aggiunti altri 5 report per il ministero della salute.
                        case "PFCNC_REG_R":
                            if (this.GetCalendarControl("txt_rep_av_initData").Visible)
                                objFilter.dataDa = this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text;
                            if (this.GetCalendarControl("txt_rep_av_fineData").Visible)
                                objFilter.dataA = this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text;
                            break;
                        case "INTEROP_REG":
                            if (this.GetCalendarControl("txt_rep_av_initData").Visible)
                                objFilter.dataDa = this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text;
                            if (this.GetCalendarControl("txt_rep_av_fineData").Visible)
                                objFilter.dataA = this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text;
                            break;
                        case "PEC_REG":
                            if (this.GetCalendarControl("txt_rep_av_initData").Visible)
                                objFilter.dataDa = this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text;
                            if (this.GetCalendarControl("txt_rep_av_fineData").Visible)
                                objFilter.dataA = this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text;

                            //Controllo il range anche se ho un validator, non voglio tcare troppo codice di qesta pagina.
                            if (!string.IsNullOrEmpty(txt_giorni_trascorsi.Text.Trim()))
                            {
                                if (Convert.ToInt16(txt_giorni_trascorsi.Text.Trim()) > 7 || Convert.ToInt16(txt_giorni_trascorsi.Text.Trim()) < 0)
                                {
                                    RegisterClientScript("RepAlert", "alert('Attenzione! si è verificato un errore di sistema durante la generazione del report');");
                                    logger.Error("Generazione del file per la stampa report in ERRORE: Range della textbox giorni trascorsi fuori range (0-7)");
                                    return;
                                }
                                objFilter.giorniTrascorsiXProtocollazioneField = Convert.ToInt16(txt_giorni_trascorsi.Text.Trim());
                            }
                            else
                                objFilter.giorniTrascorsiXProtocollazioneField = 0;
                            break;

                        //*****************************************************************************
                        case "PR_REG":
                            if (this.GetCalendarControl("txt_rep_av_initData").Visible)
                                objFilter.dataDa = this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text;
                            if (this.GetCalendarControl("txt_rep_av_fineData").Visible)
                                objFilter.dataA = this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text;
                            break;
                        case "PR_REG_R":
                            objFilter.idRuolo = this.ddl_rep_av_ruolo.SelectedValue;
                            if (this.GetCalendarControl("txt_rep_av_initData").Visible)
                                objFilter.dataDa = this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text;
                            if (this.GetCalendarControl("txt_rep_av_fineData").Visible)
                                objFilter.dataA = this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text;
                            break;
                    }

                    objReport.filtro = objFilter;

                    DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    ws.Timeout = System.Threading.Timeout.Infinite;
                    objReport.file = ws.stampaReportAvanzatiXLS(objReport);

                    if (objReport.file != null && objReport.file.length > 0)
                    {
                        // imposta la sessione
                        DocsPAWA.gestione.report.stampaReportXLS.stampaReportXLS_Session sessioneStampaReportAv = new stampaReportXLS.stampaReportXLS_Session();
                        sessioneStampaReportAv.SetSessionReportXLS(objReport);

                        // chiama la pagina che visualizza il report in Excel                    
                        //this.RegisterClientScript("Rep", "openReportXls()");
                        this.RegisterClientScript("Rep", "OpenFileXLS();");
                    }
                    else
                        RegisterClientScript("RepNOFile", "alert('Nessun dato trovato!\\n\\nImpostare un diverso criterio di ricerca.');");
                }
                else
                    RegisterClientScript("RepAlert", "alert('Attenzione! verificare la correttezza delle date inserite.');");
            }
            catch (System.Exception ex)
            {
                RegisterClientScript("RepAlert", "alert('Attenzione! si è verificato un errore di sistema durante la generazione del report');");
                logger.Error("Generazione del file per la stampa report in ERRORE: " + ex.ToString());
            }
        }

        #endregion

        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
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
            this.ddl_dataTrasm.SelectedIndexChanged += new System.EventHandler(this.ddl_dataTrasm_SelectedIndexChanged);
            this.ddl_numProt_E.SelectedIndexChanged += new System.EventHandler(this.ddl_numProt_E_SelectedIndexChanged);
            this.ddl_dataProt_E.SelectedIndexChanged += new System.EventHandler(this.ddl_dataProt_E_SelectedIndexChanged);
            this.ddl_idDoc_E.SelectedIndexChanged += new System.EventHandler(this.ddl_idDoc_E_SelectedIndexChanged);
            this.ddl_dataCreazioneG_E.SelectedIndexChanged += new System.EventHandler(this.ddl_dataCreazioneG_E_SelectedIndexChanged);
            this.txt_codUO.TextChanged += new System.EventHandler(this.txt_codUO_TextChanged);
            this.txt_Anno_B.TextChanged += new System.EventHandler(this.txt_Anno_B_TextChanged);
            this.ddl_numProt_B.SelectedIndexChanged += new System.EventHandler(this.ddl_numProt_B_SelectedIndexChanged);
            this.ddl_dataProt_B.SelectedIndexChanged += new System.EventHandler(this.ddl_dataProt_B_SelectedIndexChanged);
            this.ddl_tipoAttoDR.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoAttoDR_SelectedIndexChanged);
            this.ddl_tipoAttoDG.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoAttoDG_SelectedIndexChanged);
            this.btn_stampaRegistro.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampaRegistro_Click);
            this.btn_CampiPersonalizzatiDR.Click += new System.Web.UI.ImageClickEventHandler(this.btn_CampiPersonalizzatiDR_Click);
            this.btn_CampiPersonalizzatiDG.Click += new System.Web.UI.ImageClickEventHandler(this.btn_CampiPersonalizzatiDG_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.tabGestioneReport_PreRender);
            this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
        }
        #endregion

        private void CaricaComboRegistri(DropDownList ddl)
        {
            string stato = string.Empty;
            for (int i = 0; i < userRegistri.Length; i++)
            {
                stato = UserManager.getStatoRegistro(userRegistri[i]);

                ddl.Items.Add(userRegistri[i].codRegistro);
                ddl.Items[i].Value = userRegistri[i].systemId;
            }

            //setto lo stato del registro
            if (userRegistri.Length > 0)
            {
                setStatoReg(userRegistri[0]);
            }
        }

        private void CaricaRagioni()
        {
            this.ddl_ragioni.Items.Clear();

            listaRagioni = TrasmManager.getListaRagioni(this, null, false);

            //if (!Page.IsPostBack)
            //{
            //m_hashTableRagioneTrasmissione = new Hashtable();
            if (listaRagioni != null && listaRagioni.Length > 0)
            {
                ddl_ragioni.Items.Add("");
                for (int i = 0; i < listaRagioni.Length; i++)
                {
                    //m_hashTableRagioneTrasmissione.Add(i, listaRagioni[i]);

                    ListItem newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                    ddl_ragioni.Items.Add(newItem);
                }
                //TrasmManager.setHashRagioneTrasmissione(this, m_hashTableRagioneTrasmissione);

                this.ddl_ragioni.SelectedIndex = 0;
            }
            //}
            //else
            //{
            //    m_hashTableRagioneTrasmissione = TrasmManager.getHashRagioneTrasmissione(this);
            //}
        }

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.gestione.report.CalendarReport GetCalendarControl(string controlId)
        {
            Session["punti"] = "OK";
            return (DocsPAWA.gestione.report.CalendarReport)this.FindControl(controlId);
        }


        /// <summary>
        /// GADAMO - 22/05/2008
        /// Carica la combobox delle ragioni di trasmissioni con tutte le ragioni utili
        /// </summary>
        private void CaricaRagioniPerRicerca()
        {
            ListItem newItem;
            switch (this.ddl_report.SelectedValue)
            {
                //GIORDANO IACOZZILLI 19/03/2013
                //Per i nuovi report per Cuffari
                case "TX_R_RIC":
                //FINE
                case "TX_R":
                case "TX_P":
                    this.ddl_rep_av_rag_trasm.Items.Clear();
                    newItem = new ListItem("", "0");
                    this.ddl_rep_av_rag_trasm.Items.Add(newItem);

                    listaRagioni = TrasmManager.getListaRagioni(this, null, true);
                    if (listaRagioni != null && listaRagioni.Length > 0)
                    {
                        for (int i = 0; i < listaRagioni.Length; i++)
                        {
                            newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                            this.ddl_rep_av_rag_trasm.Items.Add(newItem);
                        }
                        this.ddl_rep_av_rag_trasm.SelectedIndex = 0;
                    }
                    break;
                //GIORDANO IACOZZILLI 15/03/2013
                //Per i nuovi report per Cuffari (report 4) mi servono le trasmissioni.
                //Solo quelle WORKFLOW
                case "TRASM_EVI_RIF_REG":
                    this.ddl_rep_av_rag_trasm.Items.Clear();
                    newItem = new ListItem("", "0");
                    this.ddl_rep_av_rag_trasm.Items.Add(newItem);
                    listaRagioni = TrasmManager.getListaRagioni(this, null, true);
                    if (listaRagioni != null && listaRagioni.Length > 0)
                    {
                        for (int i = 0; i < listaRagioni.Length; i++)
                        {
                            //FILTRO SOLO LE RAGIONI CON CHA_TIPO_RAGIONE = W
                            //per i report mi servono solo  quelle di tipo workflow.
                            if (listaRagioni[i].tipo == "W")
                            {
                                newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                                this.ddl_rep_av_rag_trasm.Items.Add(newItem);
                            }
                        }
                        this.ddl_rep_av_rag_trasm.SelectedIndex = 0;
                    }
                    break;
                //FINE GIORDANO IACOZZILLI 15/03/2013
            }
        }

        private void setStatoReg(DocsPAWA.DocsPaWR.Registro reg)
        {
            // inserisco il registro selezionato in sessione			
            UserManager.setRegistroSelezionato(this, reg);
            string nomeImg;

            if (UserManager.getStatoRegistro(reg).Equals("G"))
                nomeImg = "stato_giallo2.gif";
            else if (UserManager.getStatoRegistro(reg).Equals("V"))
                nomeImg = "stato_verde2.gif";
            else
                nomeImg = "stato_rosso2.gif";

            this.img_statoReg.ImageUrl = "../../images/" + nomeImg;
        }

        private bool ricercaTrasmissioni()
        {
            bool res = true;
            try
            {
                //array contenitore degli array filtro di ricerca
                qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];

                fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];
                #region  filtro UO
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.ID_UO.ToString();
                fV1.valore = UserManager.getRuolo(this).uo.systemId;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                res = checkData(fV1.argomento, fV1.valore);
                #endregion

                #region filtro "oggetto trasmesso"
                if (this.DDLOggettoTab1.SelectedIndex >= 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TIPO_OGGETTO.ToString();
                    fV1.valore = this.DDLOggettoTab1.SelectedItem.Value.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region  filtro sulla ragione
                if (ddl_ragioni.SelectedIndex > 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.RAGIONE.ToString();
                    fV1.valore = ddl_ragioni.SelectedValue.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region  filtro sulla data invio INIZIO
                if (!this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    if (this.ddl_dataTrasm.SelectedIndex.Equals(0))
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_IL.ToString();
                    else
                        fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    res = checkData(fV1.argomento, fV1.valore);
                }
                #endregion

                #region  filtro sulla data invio FINE
                if (!this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriTrasmissioneNascosti.TRASMISSIONE_PRECEDENTE_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    res = checkData(fV1.argomento, fV1.valore);
                }
                #endregion

                qV[0] = fVList;
                return res;
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
                res = false;
                return res;
            }
        }

        private bool checkData(string argomento, string valore)
        {
            if (argomento.Equals("TRASMISSIONE_IL") || argomento.Equals("TRASMISSIONE_SUCCESSIVA_AL") || argomento.Equals("TRASMISSIONE_PRECEDENTE_IL"))
            {
                if (!Utils.isDate(valore))
                    return false;
            }
            return true;
        }

        private void btn_stampaRegistro_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DocsPaWR.Registro registro = null;
            if (this.pnl_trasmUO.Visible)
            {
                //Controllo intervallo date trasmissione
                if (this.ddl_dataTrasm.SelectedIndex != 0)
                {
                    if (Utils.isDate(this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text, this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date Data Trasmissione!');</script>");
                        return;
                    }
                }
            }

            if (this.pnl_DocumentiRegistro.Visible)
            {
                //Controllo intervallo date protocollo
                if (this.ddl_dataProt_E.SelectedIndex != 0)
                {
                    if (Utils.isDate(this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text, this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date Data Protocollo!');</script>");
                        return;
                    }
                }
            }

            if (this.pnl_DocumentiGrigi.Visible)
            {
                //Controllo intervallo date creazione
                if (this.ddl_dataCreazioneG_E.SelectedIndex != 0)
                {
                    if (Utils.isDate(this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text, this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date Data Creazione!');</script>");
                        return;
                    }
                }
            }

            if (this.pnl_StampaBuste.Visible)
            {
                //Controllo intervallo date protocollo
                if (this.ddl_dataProt_B.SelectedIndex != 0)
                {
                    if (Utils.isDate(this.GetCalendarControl("txt_initDataProt_B").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_fineDataProt_B").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataProt_B").txt_Data.Text, this.GetCalendarControl("txt_fineDataProt_B").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date Data Protocollo!');</script>");
                        return;
                    }
                }
            }

            if (this.pnl_reportAvanzati.Visible)
            {
                //Controllo intervallo date di riferimento
                if (this.ddl_rep_av_data.SelectedIndex != 0)
                {
                    if (Utils.isDate(this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_rep_av_initData").txt_Data.Text, this.GetCalendarControl("txt_rep_av_fineData").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date Data di Riferimento!');</script>");
                        return;
                    }
                }
            }

            DocsPaWR.FileDocumento fileRep = null;
            string registri = "";

            // GADAMO - 22/05/2008
            string tipoReport = this.ddl_report.SelectedValue;
            switch (tipoReport)
            {
                //**********************************************************
                //Giordano Iacozzilli
                //20/02/2013
                //MEV:Aggiunta altri 5 report per il ministero della salute.
                case "PFCNC_REG_R":
                case "INTEROP_REG":
                case "PEC_REG":
                case "TRASM_EVI_RIF_REG":
                case "TX_R_RIC":
                //**********************************************************
                case "TX_R":
                case "TX_P":
                case "PR_REG":
                case "PR_REG_R":
                    this.impostaDatiRepAv();
                    this.ShowBlankPage();
                    return;
                    break;
            }

            if (ddl_registri.SelectedItem.Text == "Tutti" && tipoReport == "E")
            {
                foreach (DocsPAWA.DocsPaWR.Registro reg in userRegistri)
                {
                    registri += reg.systemId + ",";
                }
                registri = registri.Substring(0, registri.Length - 1);
            }
            else
            {
                registro = userRegistri[ddl_registri.SelectedIndex];
            }
            InfoUtente infoUtente = UserManager.getInfoUtente(this);
            Ruolo ruolo = UserManager.getRuolo(this);

            switch (tipoReport)
            {
                case "T": fileRep = FascicoliManager.reportTitolario(this, registro); break;
                case "F": fileRep = FascicoliManager.reportFascette(this, this.txtInput.Text, registro); break;
                case "C": fileRep = UserManager.reportCorrispondenti(this, tipoReport, registro); break;
                case "I": fileRep = UserManager.reportCorrispondenti(this, tipoReport, registro); break;
                case "E":
                    //Vecchio report per corrispondenti
                    //fileRep = UserManager.reportCorrispondenti(this, tipoReport, registro); break;

                    //nuovo report per corrispondenti
                    //if (registri == "")
                    registri = "";
                    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                    bool store = false;

                    fileRep = ws.ExportRubrica(UserManager.getInfoUtente(this), store, registri);
                    break;
                case "TR":
                    if (!ricercaTrasmissioni())
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "erroreCriteriRicerca", "alert('Si è verificato un errore nella scelta dei criteri di ricerca!');", true);
                        this.ShowBlankPage();
                        return;
                    }
                    fileRep = TrasmManager.getReportTrasmUO(this, fVList, UserManager.getRuolo(this).uo.descrizione);
                    break;
                case "DR":
                    // Stampa documenti registro
                    if (!CheckRequiredFieldsStampaRegistro())
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "noCriteriRicerca", "alert('Inserire almento un criterio di ricerca');", true);
                        this.ShowBlankPage();
                        return;
                    }

                    string rtn = IsValidControlsProtocollo();
                    if (rtn != "")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "invalidControlsProtocollo", "alert('" + rtn + "');", true);
                        this.ShowBlankPage();
                        return;
                    }

                    // Costruzione oggetto filtro
                    this.BuildFiltroDocumentiRegistro();
                    //fileRep = GestManager.GetReportRegistroWithFilters(this, infoUtente, ruolo, registro, qV);
                    DocumentManager.setFiltroRicDoc(this, qV);

                    ClientScript.RegisterStartupScript(this.GetType(), "stampaRisultati_DG", "StampaRisultatoRicerca();", true);
                    infoUtente = null;
                    ruolo = null;

                    break;

                case "DG":
                    // Stampa documenti grigi
                    if (!CheckRequiredFieldsDocumentiGrigi())
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "noCriteriRicerca", "alert('Inserire almento un criterio di ricerca');", true);
                        this.ShowBlankPage();
                        return;
                    }

                    string result = IsValidControlsDocumentiGrigi();
                    if (result != "")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "invalidControlsDocumentiGrigi", "alert('" + result + "');", true);
                        this.ShowBlankPage();
                        return;
                    }

                    // Costruzione oggetto filtro
                    this.BuildFiltroDocumentiGrigi();
                    DocumentManager.setFiltroRicDoc(this, qV);

                    ClientScript.RegisterStartupScript(this.GetType(), "stampaRisultati_DG", "StampaRisultatoRicerca();", true);
                    infoUtente = null;
                    ruolo = null;

                    break;

                case "B":
                    // Stampa Buste
                    if (!CheckRequiredFieldsStampaBuste())
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "noCriteriRicerca", "alert('Inserire almento un criterio di ricerca');", true);
                        this.ShowBlankPage();
                        return;
                    }

                    if (!IsValidControlsStampaBuste())
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "invalidControlsStampaBuste", "alert('Si è verificato un errore nella scelta dei criteri di ricerca!');", true);
                        this.ShowBlankPage();
                        return;
                    }

                    // Costruzione oggetto filtro
                    this.BuildFiltroDocumentiStampaBuste();
                    fileRep = GestManager.GetReportBusteWithFilters(this, infoUtente, ruolo, registro, qV);

                    infoUtente = null;
                    ruolo = null;

                    break;
            }

            if (fileRep != null)
            {
                this.Session["FileManager.selectedReport"] = fileRep;
                if (tipoReport.Equals("F"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "stampaFascette", "StampaFascette();", true);

                }
                else
                {
                    string sval = @"../../popup/ModalVisualReport.aspx?id=" + this.Session.SessionID;
                    ClientScript.RegisterStartupScript(this.GetType(), "ApriModale", "OpenMyDialog('" + sval + "');", true);
                }
            }
            else
            {
                switch (tipoReport)
                {
                    //In questi due casi non fa nulla perchè è la popup di selezione campi che fa la ricerca
                    //ed eventualmente comunica che non sono stati trovati documenti
                    case "DG":
                    case "DR":
                        break;

                    default:
                        ClientScript.RegisterStartupScript(this.GetType(), "noRisultati", "alert('I dati immessi non hanno prodotto alcun report.');", true);
                        break;
                }
            }
            this.ShowBlankPage();
        }

        /// <summary>
        /// Visualizzazione pagina vuota nell'area di stampa
        /// </summary>
        private void ShowBlankPage()
        {
            Response.Write("<SCRIPT>top.principale.iFrame_dettagli.document.location='../../blank_page.htm';</SCRIPT>");
        }

        protected void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //mette in sessione il registro selezionato
            if (ddl_registri.SelectedIndex != -1)
            {
                if (userRegistri == null)
                    userRegistri = UserManager.getListaRegistri(this);

                if (this.ddl_registri.Items[this.ddl_registri.SelectedIndex].Text.Equals("Tutti"))
                {
                    this.setTuttiRegistri();
                    this.icoReg.Visible = false;
                    this.lbl_ET_stato.Visible = false;
                    this.img_statoReg.Visible = false;
                }
                else
                {
                    this.setStatoReg(userRegistri[ddl_registri.SelectedIndex]);
                    this.icoReg.Visible = true;
                    this.lbl_ET_stato.Visible = true;
                    this.img_statoReg.Visible = true;
                }
            }
        }

        private void ddl_dataTrasm_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text = "";
            //if (this.ddl_dataTrasm.SelectedIndex == 0)
            //{
            //    this.GetCalendarControl("txt_fineDataTrasm").Visible = false;
            //    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = false;
            //    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = false;
            //    this.lbl_finedataTrasm.Visible = false;
            //    this.lbl_initdataTrasm.Visible = false;
            //}
            //else
            //{
            //    this.GetCalendarControl("txt_fineDataTrasm").Visible = true;
            //    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = true;
            //    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = true;
            //    this.lbl_finedataTrasm.Visible = true;
            //    this.lbl_initdataTrasm.Visible = true;
            //}
            switch (this.ddl_dataTrasm.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataTrasm").Visible = false;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = false;
                    this.lbl_finedataTrasm.Visible = false;
                    this.lbl_initdataTrasm.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Enabled = true;
                    this.lbl_finedataTrasm.Visible = true;
                    this.lbl_initdataTrasm.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataTrasm").Visible = false;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = false;
                    this.lbl_finedataTrasm.Visible = false;
                    this.lbl_initdataTrasm.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Enabled = false;
                    this.lbl_finedataTrasm.Visible = true;
                    this.lbl_initdataTrasm.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_initDataTrasm").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataTrasm").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataTrasm").Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataTrasm").txt_Data.Enabled = false;
                    this.lbl_finedataTrasm.Visible = true;
                    this.lbl_initdataTrasm.Visible = true;
                    break;
            }
        }

        private void ddl_numProt_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.RefreshFiltersCtlsNumProtocollo();
        }

        private void ddl_dataProt_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.RefreshFiltersCtlsDataProtocollo();
        }

        private void ddl_idDoc_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.RefreshFiltersCtlsIdDoc();
        }

        private void ddl_dataCreazioneG_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.RefreshFiltersCtlsDataCreazioneG();
        }

        private void RefreshFiltersCtlsIdDoc()
        {
            // Aggiornamento visibilità controlli filtro per id documento
            bool rangeFilter = (this.ddl_idDoc_E.SelectedIndex != 0);

            this.lbl_initIdDoc_E.Visible = rangeFilter;
            this.lbl_fineIdDoc_E.Visible = rangeFilter;
            this.txt_fineIdDoc_E.Visible = rangeFilter;

            this.txt_fineIdDoc_E.Text = "";
        }

        private void RefreshFiltersCtlsDataCreazioneG()
        {
            // Aggiornamento visibilità controlli filtro per data creazione grigio
            //if (this.ddl_dataCreazioneG_E.SelectedIndex == 0)
            //{
            //    this.lbl_initDataCreazioneG_E.Visible = true;
            //    this.lbl_fineDataCreazioneG_E.Visible = false;
            //    this.GetCalendarControl("txt_initDataCreazioneG_E").Visible = true;
            //    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Visible = true;
            //    this.GetCalendarControl("txt_initDataCreazioneG_E").btn_Cal.Visible = true;
            //    this.GetCalendarControl("txt_fineDataCreazioneG_E").Visible = false;
            //    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Visible = false;
            //    this.GetCalendarControl("txt_fineDataCreazioneG_E").btn_Cal.Visible = false;

            //    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text = System.DateTime.Now.ToShortDateString();
            //    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Text = "";
            //}
            //else
            //{
            //    this.lbl_initDataCreazioneG_E.Visible = true;
            //    this.lbl_fineDataCreazioneG_E.Visible = true;
            //    this.GetCalendarControl("txt_initDataCreazioneG_E").Visible = true;
            //    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Visible = true;
            //    this.GetCalendarControl("txt_initDataCreazioneG_E").btn_Cal.Visible = true;
            //    this.GetCalendarControl("txt_fineDataCreazioneG_E").Visible = true;
            //    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Visible = true;
            //    this.GetCalendarControl("txt_fineDataCreazioneG_E").btn_Cal.Visible = true;

            //    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text = "";
            //    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Text = System.DateTime.Now.ToShortDateString();
            //}
            switch (this.ddl_dataCreazioneG_E.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataCreazioneG_E").Visible = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").Visible = false;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Visible = false;
                    this.lbl_fineDataCreazioneG_E.Visible = false;
                    this.lbl_initDataCreazioneG_E.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataCreazioneG_E").Visible = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").Visible = true;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Enabled = true;
                    this.lbl_fineDataCreazioneG_E.Visible = true;
                    this.lbl_initDataCreazioneG_E.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataCreazioneG_E").Visible = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").Visible = false;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Visible = false;
                    this.lbl_fineDataCreazioneG_E.Visible = false;
                    this.lbl_initDataCreazioneG_E.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataCreazioneG_E").Visible = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").Visible = true;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Enabled = false;
                    this.lbl_fineDataCreazioneG_E.Visible = true;
                    this.lbl_initDataCreazioneG_E.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataCreazioneG_E").Visible = true;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").Visible = true;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Enabled = false;
                    this.lbl_fineDataCreazioneG_E.Visible = true;
                    this.lbl_initDataCreazioneG_E.Visible = true;
                    break;
            }

        }

        private void RefreshFiltersCtlsNumProtocollo()
        {
            // Aggiornamento visibilità controlli filtro per numero protocollo
            bool rangeFilter = (this.ddl_numProt_E.SelectedIndex != 0);

            this.lblDAnumprot_E.Visible = rangeFilter;
            this.lblAnumprot_E.Visible = rangeFilter;
            this.txt_fineNumProt_E.Visible = rangeFilter;

            this.txt_fineNumProt_E.Text = "";
        }

        private void RefreshFiltersCtlsDataProtocollo()
        {
            // Aggiornamento visibilità controlli filtro per data protocollo
            //bool rangeFilter = (this.ddl_dataProt_E.SelectedIndex != 0);

            //this.lbl_initdataProt_E.Visible = rangeFilter;
            //this.lbl_finedataProt_E.Visible = rangeFilter;
            //this.GetCalendarControl("txt_fineDataProt_E").Visible = rangeFilter;
            //this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = rangeFilter;
            //this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = rangeFilter;
            //if (rangeFilter)
            //{
            //    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = "";
            //    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text = System.DateTime.Now.ToShortDateString();
            //}
            switch (this.ddl_dataProt_E.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProt_E").Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = false;
                    this.lbl_finedataProt_E.Visible = false;
                    this.lbl_initdataProt_E.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Enabled = true;
                    this.lbl_finedataProt_E.Visible = true;
                    this.lbl_initdataProt_E.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_E").Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = false;
                    this.lbl_finedataProt_E.Visible = false;
                    this.lbl_initdataProt_E.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Enabled = false;
                    this.lbl_finedataProt_E.Visible = true;
                    this.lbl_initdataProt_E.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_initDataProt_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_E").Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Enabled = false;
                    this.lbl_finedataProt_E.Visible = true;
                    this.lbl_initdataProt_E.Visible = true;
                    break;
            }
        }

        private void RefreshFiltersCtlsNumProtocollo_B()
        {
            // Aggiornamento visibilità controlli filtro per numero protocollo
            bool rangeFilter = (this.ddl_numProt_B.SelectedIndex != 0);

            this.lblDAnumprot_B.Visible = rangeFilter;
            this.lblAnumprot_B.Visible = rangeFilter;
            this.txt_fineNumProt_B.Visible = rangeFilter;

            this.txt_fineNumProt_B.Text = "";
        }

        private void RefreshFiltersCtlsDataProtocollo_B()
        {
            // Aggiornamento visibilità controlli filtro per data protocollo
            bool rangeFilter = (this.ddl_dataProt_B.SelectedIndex != 0);

            this.lbl_initdataProt_B.Visible = rangeFilter;
            this.lbl_finedataProt_B.Visible = rangeFilter;
            this.GetCalendarControl("txt_fineDataProt_B").Visible = rangeFilter;
            this.GetCalendarControl("txt_fineDataProt_B").txt_Data.Visible = rangeFilter;
            this.GetCalendarControl("txt_fineDataProt_B").btn_Cal.Visible = rangeFilter;
            if (rangeFilter)
            {
                this.GetCalendarControl("txt_initDataProt_B").txt_Data.Text = "";
                this.GetCalendarControl("txt_fineDataProt_B").txt_Data.Text = System.DateTime.Now.ToShortDateString();
            }
        }

        /// <summary>
        /// Verifica della presenza di almeno un dato
        /// immesso nei campi di filtro della stampa 
        /// dei registri
        /// </summary>
        /// <returns></returns>
        private bool CheckRequiredFieldsStampaRegistro()
        {
            return (this.txt_initNumProt_E.Text.Length > 0 ||
                    this.txt_fineNumProt_E.Text.Length > 0 ||
                    this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text.Length > 0 ||
                    this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text.Length > 0 ||
                    this.ddl_tipoAttoDR.SelectedItem.Text.Length > 0);
        }

        /// <summary>
        /// Verifica della presenza di almeno un dato
        /// immesso nei campi di filtro dei
        /// documenti grigi
        /// </summary>
        /// <returns></returns>
        private bool CheckRequiredFieldsDocumentiGrigi()
        {
            return (this.txt_initIdDoc_E.Text.Length > 0 ||
                    this.txt_fineIdDoc_E.Text.Length > 0 ||
                    this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text.Length > 0 ||
                    this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Text.Length > 0 ||
                    this.ddl_tipoAttoDG.SelectedItem.Text.Length > 0);
        }

        /// <summary>
        /// Verifica della presenza di almeno un dato
        /// immesso nei campi di filtro della stampa 
        /// buste destinatari
        /// </summary>
        /// <returns></returns>
        private bool CheckRequiredFieldsStampaBuste()
        {
            return (this.txt_initNumProt_B.Text.Length > 0 ||
                this.txt_fineNumProt_B.Text.Length > 0 ||
                this.GetCalendarControl("txt_initDataProt_B").txt_Data.Text.Length > 0 ||
                this.GetCalendarControl("txt_fineDataProt_B").txt_Data.Text.Length > 0 ||
                this.txt_Anno_B.Text.Length > 0);

        }

        /// <summary>
        /// Validazione dati immessi nei controlli di filtro per data e numero documento
        /// </summary>
        /// <returns></returns>
        private string IsValidControlsDocumentiGrigi()
        {
            string retValue = string.Empty;
            try
            {
                int intValue;

                // Validazione id documento
                if (this.txt_initIdDoc_E.Text != "")
                    intValue = Convert.ToInt32(this.txt_initIdDoc_E.Text);

                if (ddl_idDoc_E.SelectedIndex != 0 && this.txt_fineIdDoc_E.Text != "")
                    intValue = Convert.ToInt32(this.txt_fineIdDoc_E.Text);

                // Validazione data creazione
                if (this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text != "")
                    if (!Utils.isDate(this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text))
                    {
                        retValue = "Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa";
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);

                    }

                if (ddl_dataCreazioneG_E.SelectedIndex != 0 && this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Text != "")
                    if (!Utils.isDate(this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Text))
                    {
                        retValue = "Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa";
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);

                    }
            }
            catch (Exception)
            {
                retValue = "Si è verificato un errore nella scelta dei criteri di ricerca!";
            }

            return retValue;
        }

        /// <summary>
        /// Validazione dati immessi nei controlli di filtro per data e numero protocollo
        /// </summary>
        /// <returns></returns>
        private string IsValidControlsProtocollo()
        {
            string retValue = string.Empty;

            // Validazione anno protocollazione
            try
            {
                int intValue;


                if (this.txt_initNumProt_E.Text != "")
                    intValue = Convert.ToInt32(this.txt_initNumProt_E.Text);

                if (ddl_numProt_E.SelectedIndex != 0 && this.txt_fineNumProt_E.Text != "")
                    intValue = Convert.ToInt32(this.txt_fineNumProt_E.Text);

                if (this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text != "")
                    if (!Utils.isDate(this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text))
                    {
                        retValue = "Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa";
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt_E").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);

                    }

                if (ddl_dataProt_E.SelectedIndex != 0 && this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text != "")
                    if (!Utils.isDate(this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text))
                    {
                        retValue = "Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa";
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataProt_E").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);

                    }

                //				if (tbAnnoProtocollo.Text!="")
                //					intValue=Convert.ToInt32(this.tbAnnoProtocollo.Text);
            }
            catch (Exception)
            {
                retValue = "Si è verificato un errore nella scelta dei criteri di ricerca!";
            }

            return retValue;
        }

        /// <summary>
        /// Validazione dati immessi nei controlli di filtro per data, numero protocollo e anno
        /// </summary>
        /// <returns></returns>
        private bool IsValidControlsStampaBuste()
        {
            bool retValue = true;

            try
            {
                int intValue;
                DateTime dateValue;

                if (this.txt_initNumProt_B.Text != "")
                    intValue = Convert.ToInt32(this.txt_initNumProt_B.Text);

                if (ddl_numProt_B.SelectedIndex != 0 && this.txt_fineNumProt_B.Text != "")
                    intValue = Convert.ToInt32(this.txt_fineNumProt_B.Text);

                if (this.GetCalendarControl("txt_initDataProt_B").txt_Data.Text != "")
                    dateValue = Convert.ToDateTime(this.GetCalendarControl("txt_initDataProt_B").txt_Data.Text);

                if (ddl_dataProt_B.SelectedIndex != 0 && this.GetCalendarControl("txt_fineDataProt_B").txt_Data.Text != "")
                    dateValue = Convert.ToDateTime(this.GetCalendarControl("txt_fineDataProt_B").txt_Data.Text);

                if (txt_Anno_B.Text != "")
                    intValue = Convert.ToInt32(this.txt_Anno_B.Text);
            }
            catch (Exception)
            {
                retValue = false;
            }

            return retValue;
        }

        private void BuildFiltroDocumentiRegistro()
        {
            //array contenitore degli array filtro di ricerca
            qV = new FiltroRicerca[1][];
            fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];
            FiltroRicerca filterItem = null;

            #region Composizione filtri per numero protocollo
            if (this.ddl_numProt_E.SelectedIndex == 0)
            {//valore singolo carico NUM_PROTOCOLLO
                filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                filterItem.valore = this.txt_initNumProt_E.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);

            }
            else
            {
                //valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                if (!this.txt_initNumProt_E.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                    filterItem.valore = this.txt_initNumProt_E.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
                }
                if (!this.txt_fineNumProt_E.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                    filterItem.valore = this.txt_fineNumProt_E.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);

                }
            }
            #endregion

            #region Composizione filtri per data protocollo

            //valore singolo  DATA_PROTOCOLLO
            //if (this.ddl_dataProt_E.SelectedIndex == 0)
            //{
            //    if (this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text.Equals(""))
            //    {
            //        filterItem = new FiltroRicerca();
            //        filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
            //        filterItem.valore = this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text;
            //        fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            //    }
            //}
            //else // intervallo DATA PROTOCOLLO
            //{
            //    if (this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text.Equals(""))
            //    {
            //        filterItem = new FiltroRicerca();
            //        filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
            //        filterItem.valore = this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text;
            //        fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            //    }
            //    if (!this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text.Equals(""))
            //    {
            //        filterItem = new FiltroRicerca();
            //        filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
            //        filterItem.valore = this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text;
            //        fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            //    }
            //}
            if (this.ddl_dataProt_E.SelectedIndex == 2)
            {
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString();
                fV1.valore = "1";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString();
                fV1.valore = "1";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString();
                fV1.valore = "1";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataProt_E.SelectedIndex == 0)
            {//valore singolo carico DATA_PROTOCOLLO
                if (!this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt_E").txt_Data.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        //return false;
                    }
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            if (this.ddl_dataProt_E.SelectedIndex == 1)
            {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                if (!this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt_E").txt_Data.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        //return false;
                    }
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataProt_E").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text.Equals(""))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataProt_E").txt_Data.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");

                        //return false;
                    }
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_fineDataProt_E").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }

            #endregion

            #region Composizione filtri per Unità Organizzativa
            if (!chk_uo.Checked) //ricerca protocolli effettuati solo dalla uo selzionata
            {
                if (!this.txt_descUO.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();

                    if (this.hd_systemIdUo != null && !this.hd_systemIdUo.Value.Equals(""))
                    {
                        fV1.argomento = DocsPaWR.FiltriDocumento.ID_UO_PROT.ToString();
                        fV1.valore = this.hd_systemIdUo.Value;
                    }


                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            else //ricerca protocolli effettuati dalla uo selzionata e dalle sue figlie
            {
                if (!this.txt_descUO.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();

                    if (this.hd_systemIdUo != null && !this.hd_systemIdUo.Value.Equals(""))
                    {
                        fV1.argomento = DocsPaWR.FiltriDocumento.ID_UO_PROT_GERARCHIA.ToString();
                        fV1.valore = this.hd_systemIdUo.Value;
                    }


                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {

                }
            }

            #endregion

            #region Filtro Tipologia Documento
            if (this.ddl_tipoAttoDR.SelectedIndex > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
                filterItem.valore = this.ddl_tipoAttoDR.SelectedItem.Value;
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            }
            #endregion

            #region PROTOCOLLO_ARRIVO
            filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
            filterItem.valore = "true";
            fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion

            #region PROTOCOLLO_PARTENZA
            filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
            filterItem.valore = "true";
            fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion

            #region PROTOCOLLO_INTERNO
            filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
            filterItem.valore = "true";
            fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion

            #region Filtro PROFILAZIONE_DINAMICA
            if (Session["templateRicerca"] != null)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
                filterItem.valore = "ProfilazioneDinamica";
                filterItem.template = (DocsPaWR.Templates)Session["templateRicerca"];
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            }
            #endregion filtro PROFILAZIONE_DINAMICA

            #region Filtro ORDER
            filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriStampaRegistro.ORDER_FILTER.ToString();
            filterItem.valore = String.Empty;
            fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion filtro ORDER

            #region Filtro No Secutiry
            if (this.ddl_tipoAttoDR.SelectedIndex > 0 && DocsPAWA.UserManager.ruoloIsAutorized(this, "STAMPA_REG_NO_SEC"))
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriFascicolazione.REG_NO_SECURITY.ToString();
                filterItem.valore = "true";
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            }
            #endregion

            //#region Filtro REGISTRO
            //filterItem = new FiltroRicerca();
            //filterItem.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
            //filterItem.valore = this.ddl_registri.SelectedItem.Value;
            //fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            //#endregion

            qV[0] = fVList;
        }

        private void BuildFiltroDocumentiGrigi()
        {
            //array contenitore degli array filtro di ricerca
            qV = new FiltroRicerca[1][];
            fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];
            FiltroRicerca filterItem = null;

            #region Documento Grigio
            //Imposto sempre il TIPO in questo caso "G"
            filterItem = new FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
            filterItem.valore = "G";
            fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion

            #region Id Documento Grigio
            if (this.ddl_idDoc_E.SelectedIndex == 0)
            {
                //valore singolo carico DOCNUMBER
                filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                filterItem.valore = this.txt_initIdDoc_E.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);

            }
            else
            {
                //valore singolo carico DOCNUMBER_DAL - DOCNUMBER_AL
                if (!this.txt_initIdDoc_E.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                    filterItem.valore = this.txt_initIdDoc_E.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
                }
                if (!this.txt_fineIdDoc_E.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                    filterItem.valore = this.txt_fineIdDoc_E.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);

                }
            }
            #endregion

            #region Data Creazione Grigio
            if (this.ddl_dataCreazioneG_E.SelectedIndex == 0)
            {
                //valore singolo  DATA_CREAZIONE_IL
                if (this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                    filterItem.valore = this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
                }
            }
            else
            {
                // intervallo DATA_CREAZIONE_SUCCESSIVA_AL - DATA_CREAZIONE_PRECEDENTE_IL
                if (this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                    filterItem.valore = this.GetCalendarControl("txt_initDataCreazioneG_E").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
                }
                if (!this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Text.Equals(""))
                {
                    filterItem = new FiltroRicerca();
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                    filterItem.valore = this.GetCalendarControl("txt_fineDataCreazioneG_E").txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
                }
            }
            #endregion

            #region Filtro Tipologia Documento
            if (this.ddl_tipoAttoDG.SelectedIndex > 0)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
                filterItem.valore = this.ddl_tipoAttoDG.SelectedItem.Value;
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            }
            #endregion

            #region GRIGI
            filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
            filterItem.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
            filterItem.valore = "true";
            fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            #endregion

            #region Filtro PROFILAZIONE_DINAMICA
            if (Session["templateRicerca"] != null)
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
                filterItem.valore = "ProfilazioneDinamica";
                filterItem.template = (DocsPaWR.Templates)Session["templateRicerca"];
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            }
            #endregion filtro PROFILAZIONE_DINAMICA

            #region Filtro No Security
            if (this.ddl_tipoAttoDG.SelectedIndex > 0 && DocsPAWA.UserManager.ruoloIsAutorized(this, "STAMPA_REG_NO_SEC"))
            {
                filterItem = new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriFascicolazione.REG_NO_SECURITY.ToString();
                filterItem.valore = "true";
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            }
            #endregion

            //#region Filtro REGISTRO
            //filterItem = new FiltroRicerca();
            //filterItem.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
            //filterItem.valore = this.ddl_registri.SelectedItem.Value;
            //fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
            //#endregion

            qV[0] = fVList;
        }

        private void BuildFiltroDocumentiStampaBuste()
        {
            //array contenitore degli array filtro di ricerca
            qV = new FiltroRicerca[1][];
            fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];
            FiltroRicerca filterItem = null;

            #region Composizione filtri per numero protocollo

            if (!this.txt_initNumProt_B.Text.Equals(""))
            {
                filterItem = new FiltroRicerca();

                if (this.ddl_numProt_B.SelectedValue == "1")
                    filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();

                filterItem.valore = this.txt_initNumProt_B.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
                filterItem = null;
            }

            if (!this.txt_fineNumProt_B.Text.Equals(""))
            {
                filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                filterItem.valore = this.txt_fineNumProt_B.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
                filterItem = null;
            }

            #endregion

            #region Composizione filtri per data protocollo

            if (!this.GetCalendarControl("txt_initDataProt_B").txt_Data.Text.Equals(""))
            {
                filterItem = new FiltroRicerca();

                if (this.ddl_dataProt_B.SelectedValue == "1")
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                else
                    filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();

                filterItem.valore = this.GetCalendarControl("txt_initDataProt_B").txt_Data.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
                filterItem = null;
            }

            if (!this.GetCalendarControl("txt_fineDataProt_B").txt_Data.Text.Equals(""))
            {
                filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                filterItem.valore = this.GetCalendarControl("txt_fineDataProt_B").txt_Data.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
                filterItem = null;
            }

            if (!this.txt_Anno_B.Text.Equals(""))
            {
                filterItem = new FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                filterItem.valore = this.txt_Anno_B.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, filterItem);
                filterItem = null;
            }

            #endregion


            qV[0] = fVList;
        }

        private void ddl_numProt_B_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.RefreshFiltersCtlsNumProtocollo_B();
        }

        private void ddl_dataProt_B_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.RefreshFiltersCtlsDataProtocollo_B();
        }

        private void txt_Anno_B_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void txt_codUO_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                setDescUO(this.txt_codUO.Text, "UO");
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void setDescUO(string codiceRubrica, string tipoMit)
        {
            DocsPaWR.Corrispondente corr = null;

            if (!codiceRubrica.Equals(""))
                corr = UserManager.getCorrispondente(this, codiceRubrica, false);

            if (tipoMit == "UO")
            {
                if (corr != null)
                {
                    this.txt_descUO.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
                    this.hd_systemIdUo.Value = corr.systemId;
                }

                else
                {
                    this.txt_codUO.Text = "";
                    this.txt_descUO.Text = "";
                    this.hd_systemIdUo.Value = "";
                }
            }
        }

        private void tabGestioneReport_PreRender(object sender, System.EventArgs e)
        {
            //HtmlImage btn_rubrica_E = (HtmlImage) FindControl ("btn_rubrica_E");
            string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);


            //popolamento e controllo automatico del filtro uo.

            if (pnl_DocumentiRegistro.Visible)
            {
                this.txt_descUO.ReadOnly = true;

                if (!UserManager.ruoloIsAutorized(this, "STAMPA_REG_UO"))
                {
                    this.txt_codUO.Text = UserManager.getRuolo(this).uo.codiceRubrica;
                    this.txt_descUO.Text = UserManager.getRuolo(this).uo.descrizione;
                    this.hd_systemIdUo.Value = UserManager.getRuolo(this).uo.systemId;

                    this.txt_codUO.ReadOnly = true;
                    btn_Rubrica_E.Visible = false;
                }
                else //attivo ricerca da rubrica 
                {
                    if (btn_Rubrica_E != null) //può essere invisibile.
                    {
                        if (use_new_rubrica != "1")
                        {
                            //btn_Rubrica_E.Attributes["onClick"] = "ApriRubrica('ric_E','');setHiddenField();";
                            btn_Rubrica_E.Attributes["onClick"] = "ApriRubrica('ric_E','');";
                        }
                        else
                        {
                            //btn_Rubrica_E.Attributes["onClick"] = "_ApriRubrica('ric_estesa');setHiddenField();";
                            btn_Rubrica_E.Attributes["onClick"] = "_ApriRubrica('ruo');";
                        }
                    }
                }

                if (Session["corrStampaUo"] != null)
                {
                    DocsPaWR.Corrispondente corr = (DocsPAWA.DocsPaWR.Corrispondente)Session["corrStampaUo"];
                    this.txt_codUO.Text = corr.codiceCorrispondente;
                    this.txt_descUO.Text = corr.descrizione;
                    hd_systemIdUo.Value = corr.systemId;
                    Session.Remove("corrStampaUo");
                }
            }
            else
            {
                this.txt_codUO.Text = "";
                this.txt_descUO.Text = "";
                this.hd_systemIdUo.Value = "";
            }

        }

        private void btn_CampiPersonalizzatiDR_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            verificaCampiPersonalizzati(ddl_tipoAttoDR);
            RegisterStartupScript("Apri", "<script>apriPopupAnteprima();</script>");
        }

        private void btn_CampiPersonalizzatiDG_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            verificaCampiPersonalizzati(ddl_tipoAttoDG);
            RegisterStartupScript("Apri", "<script>apriPopupAnteprima();</script>");
        }

        private void ddl_tipoAttoDR_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            verificaCampiPersonalizzati(ddl_tipoAttoDR);
            Session.Remove("templateRicerca");
        }

        private void ddl_tipoAttoDG_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            verificaCampiPersonalizzati(ddl_tipoAttoDG);
            Session.Remove("templateRicerca");
        }

        private void CaricaComboTipologiaAtto(DropDownList ddl)
        {
            DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null)
            {
                //listaTipologiaAtto=DocumentManager.getListaTipologiaAtto(this,UserManager.getInfoUtente(this).idAmministrazione);
                listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "1");
            }
            else
            {
                listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);
            }

            ddl.Items.Clear();
            //aggiunge una riga vuota alla combo
            ddl.Items.Add("");
            if (listaTipologiaAtto != null)
            {
                for (int i = 0; i < listaTipologiaAtto.Length; i++)
                {
                    ddl.Items.Add(listaTipologiaAtto[i].descrizione);
                    ddl.Items[i + 1].Value = listaTipologiaAtto[i].systemId;
                }
            }

            btn_CampiPersonalizzatiDR.Visible = false;
            btn_CampiPersonalizzatiDG.Visible = false;
        }

        private void verificaCampiPersonalizzati(DropDownList ddl)
        {
            DocsPaWR.Templates template = new DocsPAWA.DocsPaWR.Templates();
            if (!ddl.SelectedValue.Equals(""))
            {
                template = (DocsPAWA.DocsPaWR.Templates)Session["templateRicerca"];
                if (Session["templateRicerca"] == null)
                {
                    template = ProfilazioneDocManager.getTemplatePerRicerca((UserManager.getInfoUtente(this)).idAmministrazione, ddl.SelectedItem.Text, this);
                    Session.Add("templateRicerca", template);
                }
                if (template != null && !(ddl.SelectedItem.Text.ToUpper()).Equals(template.DESCRIZIONE.ToUpper()))
                {
                    template = ProfilazioneDocManager.getTemplatePerRicerca((UserManager.getInfoUtente(this)).idAmministrazione, ddl.SelectedItem.Text, this);
                    Session.Add("templateRicerca", template);
                }
            }
            if (template != null && template.SYSTEM_ID == 0)
            {
                btn_CampiPersonalizzatiDR.Visible = false;
                btn_CampiPersonalizzatiDG.Visible = false;
            }
            else
            {
                if (template != null && template.ELENCO_OGGETTI.Length != 0)
                {
                    btn_CampiPersonalizzatiDR.Visible = true;
                    btn_CampiPersonalizzatiDG.Visible = true;
                }
                else
                {
                    btn_CampiPersonalizzatiDR.Visible = false;
                    btn_CampiPersonalizzatiDG.Visible = false;
                }
            }
        }
    }
}
