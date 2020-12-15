using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using ConservazioneWA.Utils;
using ConservazioneWA.DocsPaWR;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ConservazioneWA
{
    public partial class RicercaFascicoli : System.Web.UI.Page
    {
        protected WSConservazioneLocale.InfoUtente infoUtente;
        protected DocsPaWR.InfoUtente infoUser;
        protected WSConservazioneLocale.InfoAmministrazione amm;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);

            if (!IsPostBack)
            {
                GestioneGrafica();
                GetTypeDocument();
                CaricaComboTitolari();
                this.RecordCount = 0;
                this.SelectedPage = 1;
                this.SearchFilters = null;
                this.Result = null;
                this.TemplateProf = null;
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session["fascicoli"] != null && !string.IsNullOrEmpty(is_fasc.Value))
            {
                WSConservazioneLocale.Fascicolo[] projectList = HttpContext.Current.Session["fascicoli"] as WSConservazioneLocale.Fascicolo[];
                txtCodFascicolo.Text = projectList[0].codice;
                txtDescFascicolo.Text = projectList[0].descrizione;
                this.id_Fasc.Value = this.is_fasc.Value;
                this.is_fasc.Value = string.Empty;
                projectList = null;
            }
        }

        protected void CaricaComboTitolari()
        {
            ddl_titolari.Items.Clear();

            ArrayList listaTitolari = new ArrayList(ConservazioneManager.getTitolariUtilizzabili(infoUtente.idAmministrazione));

            //Esistono dei titolari chiusi
            if (listaTitolari.Count > 1)
            {
                //Creo le voci della ddl dei titolari
                string valueTutti = string.Empty;
                foreach (WSConservazioneLocale.OrgTitolario titolario in listaTitolari)
                {
                    ListItem it = null;
                    switch (titolario.Stato)
                    {
                        case WSConservazioneLocale.OrgStatiTitolarioEnum.Attivo:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                        case WSConservazioneLocale.OrgStatiTitolarioEnum.Chiuso:
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
                WSConservazioneLocale.OrgTitolario titolario = (WSConservazioneLocale.OrgTitolario)listaTitolari[0];
                if (titolario.Stato != WSConservazioneLocale.OrgStatiTitolarioEnum.InDefinizione)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    ddl_titolari.Items.Add(it);
                }
                ddl_titolari.Enabled = false;
            }
        }

        /// <summary>
        /// Evento generato al cambio del testo nella casella del codice rubrica
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCodRuolo_TextChanged(object sender, EventArgs e)
        {
            if (this.infoUtente != null)
            {
                this.infoUser = new InfoUtente();
                infoUser.idCorrGlobali = this.infoUtente.idCorrGlobali;
                infoUser.idPeople = this.infoUtente.idPeople;
                infoUser.idGruppo = this.infoUtente.idGruppo;
                infoUser.dst = this.infoUtente.dst;
                infoUser.idAmministrazione = this.infoUtente.idAmministrazione;
                infoUser.userId = this.infoUtente.userId;
                infoUser.sede = this.infoUtente.sede;
            }

            if (!string.IsNullOrEmpty(txtCodRuolo.Text))
            {
                setDescCorr(txtCodRuolo.Text);
            }
            else
            {
                txtCodRuolo.Text = string.Empty;
                txtDescRuolo.Text = string.Empty;
                id_corr.Value = string.Empty;
                tipo_corr.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Inserire un codice da cercare in rubrica');", true);
            }
            this.upFiltriRicerca.Update();
        }

        /// <summary>
        /// Evento generato al cambio del testo nella casella del codice fascicolo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCodFascicolo_TextChanged(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(this.txtCodFascicolo.Text))
            {
                this.SearchCodProject();
            }
            else
            {
                txtCodFascicolo.Text = string.Empty;
                txtDescFascicolo.Text = string.Empty;
                this.id_Fasc.Value = string.Empty;
                this.is_fasc.Value = string.Empty;
            }
            this.upFiltriRicerca.Update();
        }

        protected void SearchCodProject()
        {
            Session.Remove("fascicoli");
            WSConservazioneLocale.Fascicolo[] projectList = ConservazioneManager.GetFascicoloDaCodiceNoSecurity(this.txtCodFascicolo.Text, this.infoUtente.idAmministrazione, true);

            if (projectList == null || projectList.Length == 0)
            {
                txtCodFascicolo.Text = string.Empty;
                txtDescFascicolo.Text = string.Empty;
                this.id_Fasc.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "alert('Nessun fascicolo trovato con questo codice');", true);
            }
            else
            {
                if (projectList.Length == 1)
                {
                    txtCodFascicolo.Text = projectList[0].codice;
                    txtDescFascicolo.Text = projectList[0].descrizione;
                    this.id_Fasc.Value = projectList[0].systemID;
                }
                else
                {
                    HttpContext.Current.Session["fascicoli"] = projectList as WSConservazioneLocale.Fascicolo[];

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "OpenSceltaFascicoli();", true);
                    txtCodFascicolo.Text = string.Empty;
                    txtDescFascicolo.Text = string.Empty;
                    this.id_Fasc.Value = string.Empty;
                }
            }


        }

        protected void GestioneGrafica()
        {
            this.lblA.Visible = false;
            this.lbl_dataCreazioneA.Visible = false;
            this.lbl_finedataC.Visible = false;
            this.txt_fineDataC.Visible = false;
            this.lbl_finedataA.Visible = false;
            this.txt_fineDataA.Visible = false;
            this.btnFind.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            this.btnFind.Attributes.Add("onmouseout", "this.className='cbtn';");
            this.tbAnnoProtocollo.Text = DateTime.Now.Year.ToString();
            amm = Utils.ConservazioneManager.GetInfoAmmCorrente(infoUtente.idAmministrazione);
            this.lbl_amm.Text = this.amm.Codice + " - " + this.amm.Descrizione;
        }

        protected void GetTypeDocument()
        {

            WSConservazioneLocale.TemplateLite[] templateList;
            templateList = ConservazioneManager.GetTypeDocumentsWithDiagramByIdAmm(Convert.ToInt32(infoUtente.idAmministrazione), "F");
            if (templateList != null && templateList.Length > 0)
            {
                ddl_type_documents.Items.Clear();
                ddl_type_documents.Items.Add("");
                for (int i = 0; i < templateList.Length; i++)
                {
                    ddl_type_documents.Items.Add(templateList[i].name);
                    ddl_type_documents.Items[i + 1].Value = templateList[i].system_id;
                }
            }

        }

        protected void setDescCorr(string codRubrica)
        {

            Corrispondente corr = Utils.UserManager.GetCorrispondenteByCodRubricaIE(codRubrica, AddressbookTipoUtente.INTERNO, this.infoUser);
            if (corr == null)
            {
                txtCodRuolo.Text = string.Empty;
                txtDescRuolo.Text = string.Empty;
                id_corr.Value = string.Empty;
                tipo_corr.Value = string.Empty;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Corrispondente non trovato');", true);
            }
            else
            {
                if (corr != null && corr.tipoCorrispondente.Equals("U"))
                {
                    txtCodRuolo.Text = string.Empty;
                    txtDescRuolo.Text = string.Empty;
                    id_corr.Value = string.Empty;
                    tipo_corr.Value = string.Empty;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Inserire soltanto Ruoli o Persone');", true);
                }
                else
                {
                    txtCodRuolo.Text = corr.codiceRubrica;
                    txtDescRuolo.Text = corr.descrizione;
                    id_corr.Value = corr.systemId;
                    tipo_corr.Value = corr.tipoCorrispondente;
                }
            }
        }

        protected void ddl_dataCreazione_E_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataCreazione_E.SelectedIndex)
            {
                case 0:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;
                    this.lblDa.Text = "Il";

                    this.lblDa.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;

                case 1:
                    this.lblA.Visible = true;
                    this.lbl_dataCreazioneA.Visible = true;
                    this.lblDa.Text = "Da";

                    this.lblDa.Visible = true;
                    this.lbl_dataCreazioneDa.Visible = true;
                    break;

                case 2:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;

                case 3:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;

                case 4:
                    this.lblA.Visible = false;
                    this.lbl_dataCreazioneA.Visible = false;
                    this.lbl_dataCreazioneA.Text = string.Empty;

                    this.lblDa.Visible = false;
                    this.lbl_dataCreazioneDa.Visible = false;
                    this.lbl_dataCreazioneDa.Text = string.Empty;
                    break;

            }

            this.upFiltriRicerca.Update();
        }

        protected void ddl_dataC_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataC.SelectedIndex)
            {
                case 0:
                    this.lbl_finedataC.Visible = false;
                    this.txt_fineDataC.Visible = false;
                    this.txt_fineDataC.Text = string.Empty;
                    this.lbl_initdataC.Text = "Il";

                    this.lbl_initdataC.Visible = true;
                    this.txt_initDataC.Visible = true;
                    break;

                case 1:
                    this.lbl_finedataC.Visible = true;
                    this.txt_fineDataC.Visible = true;
                    this.lbl_initdataC.Text = "Da";

                    this.lbl_initdataC.Visible = true;
                    this.txt_initDataC.Visible = true;
                    break;

                case 2:
                    this.lbl_finedataC.Visible = false;
                    this.txt_fineDataC.Visible = false;
                    this.txt_fineDataC.Text = string.Empty;

                    this.lbl_initdataC.Visible = false;
                    this.txt_initDataC.Visible = false;
                    this.txt_initDataC.Text = string.Empty;
                    break;

                case 3:
                    this.lbl_finedataC.Visible = false;
                    this.txt_fineDataC.Visible = false;
                    this.txt_fineDataC.Text = string.Empty;

                    this.lbl_initdataC.Visible = false;
                    this.txt_initDataC.Visible = false;
                    this.txt_initDataC.Text = string.Empty;
                    break;

                case 4:
                    this.lbl_finedataC.Visible = false;
                    this.txt_fineDataC.Visible = false;
                    this.txt_fineDataC.Text = string.Empty;

                    this.lbl_initdataC.Visible = false;
                    this.txt_initDataC.Visible = false;
                    this.txt_initDataC.Text = string.Empty;
                    break;

            }

            this.upFiltriRicerca.Update();
        }

        protected void ddl_dataA_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataA.SelectedIndex)
            {
                case 0:
                    this.lbl_finedataA.Visible = false;
                    this.txt_fineDataA.Visible = false;
                    this.lbl_finedataA.Text = string.Empty;
                    this.lbl_initdataA.Text = "Il";

                    this.lbl_initdataA.Visible = true;
                    this.txt_initDataA.Visible = true;
                    break;

                case 1:
                    this.lbl_finedataA.Visible = true;
                    this.txt_fineDataA.Visible = true;
                    this.lbl_initdataA.Text = "Da";

                    this.lbl_initdataA.Visible = true;
                    this.txt_initDataA.Visible = true;
                    break;

                case 2:
                    this.lbl_finedataA.Visible = false;
                    this.txt_fineDataA.Visible = false;
                    this.txt_fineDataA.Text = string.Empty;

                    this.lbl_initdataA.Visible = false;
                    this.txt_initDataA.Visible = false;
                    this.txt_initDataA.Text = string.Empty;
                    break;

                case 3:
                    this.lbl_finedataA.Visible = false;
                    this.txt_fineDataA.Visible = false;
                    this.txt_fineDataA.Text = string.Empty;

                    this.lbl_initdataA.Visible = false;
                    this.lbl_initdataA.Visible = false;
                    this.lbl_initdataA.Text = string.Empty;
                    break;

                case 4:
                    this.lbl_finedataA.Visible = false;
                    this.txt_fineDataA.Visible = false;
                    this.txt_fineDataA.Text = string.Empty;

                    this.lbl_finedataA.Visible = false;
                    this.txt_initDataA.Visible = false;
                    this.txt_initDataA.Text = string.Empty;
                    break;

            }

            this.upFiltriRicerca.Update();
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            int number = 0;
            if (GetFiltriRicerca())
            {
                if (this.SearchFilters != null)
                {
                    this.Result = SearchProject(this.SearchFilters, this.SelectedPage, out number);
                    this.dgResult.VirtualItemCount = number;
                    this.dgResult.CurrentPageIndex = 0;
                    this.dgResult.DataSource = this.Result;
                    this.dgResult.DataBind();


                    this.pnl_result.Visible = true;
                    this.upRisultati.Update();
                }
            }
        }

        /// <summary>
        /// Al cambio di pagina, vengono caricati i documenti per la pagina selezionata
        /// e vengono visualizzati
        /// </summary>
        protected void dgResult_SelectedPageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            // Aggiornamento del numero di pagina memorizzato nel call context
            this.dgResult.CurrentPageIndex = e.NewPageIndex;
            this.SelectedPage = e.NewPageIndex + 1;

            // Ricerca dei documenti e visualizzazione dei risultati
            int number = 0;
            this.Result = SearchProject(this.SearchFilters, this.SelectedPage, out number);

            this.dgResult.VirtualItemCount = number;
            this.dgResult.DataSource = this.Result;
            this.dgResult.DataBind();
            this.upRisultati.Update();
        }

        /// <summary>
        /// Funzione per la ricerca dei documenti
        /// </summary>
        /// <param name="recordNumber">Numero di record restituiti dalla ricerca</param>
        protected WSConservazioneLocale.SearchObject[] SearchProject(WSConservazioneLocale.FiltroRicerca[][] searchFilters, int selectedPage, out int recordNumber)
        {
            // Documenti individuati dalla ricerca
            WSConservazioneLocale.SearchObject[] projects;

            // Numero totale di pagine
            int pageNumbers;

            // Lista dei system id dei documenti restituiti dalla ricerca
            WSConservazioneLocale.SearchResultInfo[] idProjects = null;

            projects = ConservazioneManager.GetListaFascicoliPagingCustom(null, null, this.SearchFilters[0], false, this.SelectedPage, out pageNumbers, out recordNumber, 10, false, out idProjects, null, false, false, null, null, infoUtente, false);

            // Memorizzazione del numero di risultati restituiti dalla ricerca, del numero di pagine e dei risultati
            this.RecordCount = recordNumber;
            this.PageCount = pageNumbers;
            this.Result = projects;

            return projects;
        }

        protected void dgResult_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        protected String GetSystemID(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectID;
        }

        protected String GetTipo(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P1")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetCodice(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P3")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetCodClass(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P2")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetDescrizione(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P4")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetDataApertura(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P5")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetDataChiusura(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P6")).FirstOrDefault().SearchObjectFieldValue;
        }
       
        protected String GetTipologia(WSConservazioneLocale.SearchObject temp)
        {
            return temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("U1")).FirstOrDefault().SearchObjectFieldValue;
        }

        protected String GetIstanze(WSConservazioneLocale.SearchObject temp)
        {
            string istanze = string.Empty;
            string totaleIstanze = temp.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ISTANZECONSERVAZIONE")).FirstOrDefault().SearchObjectFieldValue;
            //Gabriele Melini 27-09-2013
            //MEV CONS 1.4
            //Accedendo al dettaglio di un'istanza
            //Devo avere evidenza dei documenti appartenenti al fascicolo di partenza
            string idFascicolo = temp.SearchObjectID;

            if (!string.IsNullOrEmpty(totaleIstanze))
            {
                string[] singoleIstanze = totaleIstanze.Split('-');
                if (singoleIstanze != null)
                {
                    istanze = "<ul class=\"link_istanze\">";
                    for (int i = 0; i < singoleIstanze.Length; i++)
                    {
                        if (i != singoleIstanze.Length - 1)
                        {
                            //istanze += "<li class=\"spazioLi\"><a href=\"RicercaIstanze.aspx?id=" + singoleIstanze[i] + "\" title=\"Istanza numero " + singoleIstanze[i] + "\">" + singoleIstanze[i] + "</a></li>";
                            istanze += "<li class=\"spazioLi\"><a href=\"RicercaIstanze.aspx?id=" + singoleIstanze[i] + "&project=" + idFascicolo + "\" title=\"Istanza numero " + singoleIstanze[i] + "\">" + singoleIstanze[i] + "</a></li>";
                        }
                        else
                        {
                            //istanze += "<li><a href=\"RicercaIstanze.aspx?id=" + singoleIstanze[i] + "\" title=\"Istanza numero " + singoleIstanze[i] + "\">" + singoleIstanze[i] + "</a></li>";
                            istanze += "<li><a href=\"RicercaIstanze.aspx?id=" + singoleIstanze[i] + "&project=" + idFascicolo + "\" title=\"Istanza numero " + singoleIstanze[i] + "\">" + singoleIstanze[i] + "</a></li>";
                        }
                    }
                    istanze += "</ul>";
                }
            }
            return istanze;
        }

        protected bool GetFiltriRicerca()
        {
            //array contenitore degli array filtro di ricerca
            WSConservazioneLocale.FiltroRicerca[][] qV = new WSConservazioneLocale.FiltroRicerca[1][];
            qV[0] = new WSConservazioneLocale.FiltroRicerca[1];
            WSConservazioneLocale.FiltroRicerca[] fVList = new WSConservazioneLocale.FiltroRicerca[0];
            WSConservazioneLocale.FiltroRicerca fV1 = null;

            #region filtro numero fascicolo
            if (!string.IsNullOrEmpty(txtNumFasc.Text))
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.NUMERO_FASCICOLO.ToString();
                fV1.valore = this.txtNumFasc.Text.ToString();
                if (isNumeric(fV1.valore))
                {
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Il numero del fascicolo non è numerico!');", true);
                    return false;
                }
            }
            #endregion

            #region  filtro numero protocollo
            if (!string.IsNullOrEmpty(this.tbAnnoProtocollo.Text))
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.ANNO_FASCICOLO.ToString();
                fV1.valore = this.tbAnnoProtocollo.Text.ToString();
                if (isNumeric(fV1.valore))
                {
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('L\\'anno digitato non è numerico!');", true);
                    return false;
                }
            }
            #endregion

            #region  filtro sulla data di apertura fascicolo
            if (this.ddl_dataA.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.APERTURA_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataA.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.APERTURA_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataA.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.APERTURA_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataA.SelectedIndex == 0)
            {//valore singolo carico DATA_APERTURA
                if (this.GetCalendarControl("txt_initDataA").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataA").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("txt_initDataA").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }

                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.APERTURA_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataA").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            if (this.ddl_dataA.SelectedIndex == 1)
            {//valore singolo carico DATA_APERTURA_DAL - DATA_APERTURA_AL
                if (!this.GetCalendarControl("txt_initDataA").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("txt_initDataA").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }

                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.APERTURA_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataA").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.GetCalendarControl("txt_fineDataA").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("txt_fineDataA").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.APERTURA_PRECEDENTE_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_fineDataA").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region  filtro sulla data chiusura di un fascicolo
            if (this.ddl_dataC.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CHIUSURA_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataC.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CHIUSURA_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataC.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CHIUSURA_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataC.SelectedIndex == 0)
            {//valore singolo carico DATA_CHIUSURA
                if (this.GetCalendarControl("txt_initDataC").txt_Data.Text != null && !this.GetCalendarControl("txt_initDataC").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("txt_initDataC").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }

                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CHIUSURA_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataC").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            if (this.ddl_dataC.SelectedIndex == 1)
            {//valore singolo carico DATA_CHIUSURA_DAL - DATA_CHIUSURA_AL
                if (!this.GetCalendarControl("txt_initDataC").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("txt_initDataC").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }

                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CHIUSURA_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_initDataC").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.GetCalendarControl("txt_fineDataC").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("txt_fineDataC").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CHIUSURA_PRECEDENTE_IL.ToString();
                    fV1.valore = this.GetCalendarControl("txt_fineDataC").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region  filtro sulla data creazione di un fascicolo
            if (this.ddl_dataCreazione_E.SelectedIndex == 2)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CREAZIONE_TODAY.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 3)
            {
                // siamo nel caso di Settimana corrente
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CREAZIONE_SC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 4)
            {
                // siamo nel caso di Mese corrente
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CREAZIONE_MC.ToString();
                fV1.valore = "1";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 0)
            {//valore singolo carico DATA_CREAZIONE
                if (this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text != null && !this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }

                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CREAZIONE_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            if (this.ddl_dataCreazione_E.SelectedIndex == 1)
            {//valore singolo carico DATA_CREAZIONE_DAL - DATA_CREAZIONE_AL
                if (!this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }

                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CREAZIONE_SUCCESSIVA_AL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dataCreazioneDa").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
                if (!this.GetCalendarControl("lbl_dataCreazioneA").txt_Data.Text.Equals(""))
                {
                    if (!isDate(this.GetCalendarControl("lbl_dataCreazioneA").txt_Data.Text))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');", true);
                        return false;
                    }
                    fV1 = new WSConservazioneLocale.FiltroRicerca();
                    fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CREAZIONE_PRECEDENTE_IL.ToString();
                    fV1.valore = this.GetCalendarControl("lbl_dataCreazioneA").txt_Data.Text;
                    fVList = addToArrayFiltroRicerca(fVList, fV1);
                }
            }
            #endregion

            #region descrizione
            if (!this.txtDescr.Text.Equals(""))
            {

                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.TITOLO.ToString();
                fV1.valore = this.txtDescr.Text.ToString();
                fVList = addToArrayFiltroRicerca(fVList, fV1);

            }
            #endregion

            #region filtro tipologia fascicolo e profilazione dinamica
            if (this.ddl_type_documents.SelectedIndex > 0)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.TIPOLOGIA_FASCICOLO.ToString();
                fV1.valore = this.ddl_type_documents.SelectedItem.Value;
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion

            #region FILTRO SUI SOTTOFASCICOLI

            fV1 = new WSConservazioneLocale.FiltroRicerca();
            fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.SOTTOFASCICOLO.ToString();
            fV1.valore = this.txt_sottofascicolo.Text;
            fVList = addToArrayFiltroRicerca(fVList, fV1);

            #endregion

            #region proprietario
            if (!string.IsNullOrEmpty(this.id_corr.Value))
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = "ID_OWNER";
                fV1.valore = this.id_corr.Value;
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            if (!string.IsNullOrEmpty(this.tipo_corr.Value))
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = "CORR_TYPE_OWNER";
                fV1.valore = this.tipo_corr.Value;
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }
            #endregion

            fV1 = new WSConservazioneLocale.FiltroRicerca();
            fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.IN_CONSERVAZIONE.ToString();
            fV1.valore = "F";
            fVList = addToArrayFiltroRicerca(fVList, fV1);

            fV1 = new WSConservazioneLocale.FiltroRicerca();
            fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.ID_TITOLARIO.ToString();
            fV1.valore = this.ddl_titolari.SelectedValue;
            fVList = addToArrayFiltroRicerca(fVList, fV1);

            //Gabriele Melini 5-11-2013
            //aggiunto filtro su codice fascicolo
            if (!string.IsNullOrEmpty(this.id_Fasc.Value))
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                //fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CODICE_FASCICOLO.ToString();
                fV1.argomento = WSConservazioneLocale.FiltriFascicolazione.CONSERVAZIONE_CODFASC.ToString();
                fV1.valore = this.id_Fasc.Value;
                //fV1.valore = " IN (" + this.id_Fasc.Value + ") ";
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            if (this.TemplateProf != null)
            {
                fV1 = new WSConservazioneLocale.FiltroRicerca();
                fV1.argomento = WSConservazioneLocale.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString();
                fV1.valore = "Profilazione Dinamica";
                fV1.template = this.TemplateProf;
                fVList = addToArrayFiltroRicerca(fVList, fV1);
            }

            qV[0] = fVList;

            this.SearchFilters = qV;

            return true;
        }

        public static bool isDate(string data)
        {
            try
            {
                data = data.Trim();
                CultureInfo ci = new CultureInfo("it-IT");
                string[] formati = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
                DateTime d_ap = DateTime.ParseExact(data, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        protected ConservazioneWA.UserControl.Calendar GetCalendarControl(string controlId)
        {
            return (ConservazioneWA.UserControl.Calendar)this.FindControl(controlId);
        }

        public static WSConservazioneLocale.FiltroRicerca[] addToArrayFiltroRicerca(WSConservazioneLocale.FiltroRicerca[] array, WSConservazioneLocale.FiltroRicerca nuovoElemento)
        {
            WSConservazioneLocale.FiltroRicerca[] nuovaLista;
            if (array != null)
            {
                int len = array.Length;
                nuovaLista = new WSConservazioneLocale.FiltroRicerca[len + 1];
                array.CopyTo(nuovaLista, 0);
                nuovaLista[len] = nuovoElemento;
                return nuovaLista;
            }
            else
            {
                nuovaLista = new WSConservazioneLocale.FiltroRicerca[1];
                nuovaLista[0] = nuovoElemento;
                return nuovaLista;
            }
        }

        protected void ViewCampiProlilati(object sender, EventArgs e)
        {

        }

        protected void ChangeTypeDocument(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddl_type_documents.SelectedValue))
            {
                this.btnCampiProfilati.OnClientClick = String.Format("OpenCampiProfilati('" + ddl_type_documents.SelectedValue + "');");
                this.btnCampiProfilati.Enabled = true;
                this.TemplateProf = null;
            }
            else
            {
                this.btnCampiProfilati.Enabled = false;
            }
            this.upFiltriRicerca.Update();
        }


        public static bool isNumeric(string val)
        {
            string appo = val;
            Regex regExp = new Regex("\\D");

            return !regExp.IsMatch(appo);
        }

        protected int SelectedPage
        {
            get
            {
                int toReturn = 20;
                if (HttpContext.Current.Session["selectedPage"] != null)
                    toReturn = Convert.ToInt32(HttpContext.Current.Session["selectedPage"].ToString());

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["selectedPage"] = value;
            }
        }

        protected int RecordCount
        {
            get
            {
                int toReturn = 20;
                if (HttpContext.Current.Session["recordCount"] != null)
                    toReturn = Convert.ToInt32(HttpContext.Current.Session["recordCount"].ToString());

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["recordCount"] = value;
            }
        }

        protected string UrlCampiProfilati
        {
            get
            {
                return "PopUp/CampiProfilati.aspx?type=F";
            }

        }

        protected string UrlChooseProject
        {
            get
            {
                return "ChooseProject.aspx";
            }

        }

        protected WSConservazioneLocale.FiltroRicerca[][] SearchFilters
        {
            get
            {
                return HttpContext.Current.Session["searchFilters"] as WSConservazioneLocale.FiltroRicerca[][];

            }

            set
            {
                HttpContext.Current.Session["searchFilters"] = value;
            }
        }

        /// <summary>
        /// Numero di pagine restituiti dalla ricerca
        /// </summary>
        protected int PageCount
        {
            get
            {
                int toReturn = 1;

                if (HttpContext.Current.Session["PageCount"] != null &&
                    Int32.TryParse(
                        HttpContext.Current.Session["PageCount"].ToString(),
                        out toReturn)) ;

                return toReturn;
            }

            set
            {
                HttpContext.Current.Session["PageCount"] = value;
            }
        }

        /// <summary>
        /// Risultati restituiti dalla ricerca.
        /// </summary>
        public WSConservazioneLocale.SearchObject[] Result
        {
            get
            {
                return HttpContext.Current.Session["Result"] as WSConservazioneLocale.SearchObject[];
            }
            set
            {
                HttpContext.Current.Session["Result"] = value;
            }
        }

        /// <summary>
        /// Template selezionato
        /// </summary>
        protected WSConservazioneLocale.Templates TemplateProf
        {
            get
            {
                return HttpContext.Current.Session["TemplateProf"] as WSConservazioneLocale.Templates;
            }
            set
            {
                HttpContext.Current.Session["TemplateProf"] = value;
            }
        }
    }
}