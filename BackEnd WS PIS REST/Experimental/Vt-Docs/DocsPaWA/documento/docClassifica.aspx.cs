using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using log4net;
using DocsPAWA.DocsPaWR;
namespace DocsPAWA.documento
{
    public partial class docClassifica : DocsPAWA.CssPage
    {
        private ILog logger = LogManager.GetLogger(typeof(docClassifica));
        #region sezione variabili globali

        private enum tipoVisualizzazioneFascicoli
        {
            tvfUndefined, tvfDaAreaDiLavoro, tvfProcedimentali, tvfAll
        }
        private const string QUERYSTRING_tipoVisualizzazione = "tipoVisualizzazione";
        private const string QUERYSTRING_codClassifica = "codClassifica";
        private const string QUERYSTRING_idTitolario = "idTitolario";
        private const int CONST_NUMBER_OF_DDL_CONTROLS = 6;
        private const string CONST_BASE_NAME_CONTROL_OF_LIST = "ddl_livello";
        private const int CONST_BASE_INDEX_CONTROL_OF_LIST = 1;
        private const string CONST_navigatePageSearch = "listaFascicoli.aspx";
        private const string SESSION_HashTableListControls = "HashTableListControls";
        private const string VIEWSTATE_TipoVisFascicoli = "TipoVisFascicoli";
        private Hashtable m_hashListaNameCombo;
        private DocsPAWA.DocsPaWR.FascicolazioneClassificazione m_classificazioneSelezionata;
        private DocsPAWA.DocsPaWR.SchedaDocumento m_documentoSelezionato;
        private DocsPAWA.DocsPaWR.Fascicolo m_fascicoloSelezionato;
        protected DocsPAWA.dataSet.DataSetLFascContDoc dataSetLFascContDoc1 = new DocsPAWA.dataSet.DataSetLFascContDoc();
        private DataTable m_dataTableFascicoli;
        private DocsPAWA.DocsPaWR.Registro[] userRegistri;
        private DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
        protected bool viewCodiceInCombo;
        protected Utilities.MessageBox msg_inserisciDoc;
        private string active = string.Empty;
        protected System.Web.UI.WebControls.LinkButton LinkButton1;
        protected System.Web.UI.WebControls.Label lbl_codice;
        protected DocsPAWA.DocsPaWR.InfoUtente infoUtente;
        protected List<string> listaFascNonVisibili = new List<string>();
        protected int numFasc;
        DocsPaWR.Fascicolo[] listaFasc;

        #endregion

        #region sezione template pagina
        private void getParametri()
        {
            try
            {
                //recupero sempre i parametri dalla queryString
                if (!IsPostBack)
                {
                    //primo caricamento pagina
                    getParametriDatiPaginePrecedenti();
                    buildParametriPagina();
                    setParametriPaginaInSession();
                }
                else
                {
                    //post back:
                    //recupero eventuali dati già creati per la 
                    //pagina e memorizzati nel view state e in session
                    //getParametriPaginaViewState();
                    getParametriPaginaInSession();


                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void getParametriPaginaInSession()
        {
            //recupero i parametri che devono essere
            //disponibili alla pagina tra le varie 
            //sequenze di postback
            try
            {
                m_hashListaNameCombo = (Hashtable)Session[SESSION_HashTableListControls];
                m_classificazioneSelezionata = DocumentManager.getClassificazioneSelezionata(this);
                m_documentoSelezionato = DocumentManager.getDocumentoSelezionato(this);
                m_dataTableFascicoli = DocumentManager.getDataGridFascicoliContenitori(this);
                this.listaFascNonVisibili = DocumentManager.getDataFascicoliNonVisibili(this);
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
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

        private void deleteDocument(int key)
        {

            DataTable dataTableFascicoli = DocumentManager.getDataGridFascicoliContenitori(this);
            this.listaFascNonVisibili = DocumentManager.getDataFascicoliNonVisibili(this);
            DocsPaWR.SchedaDocumento documentoSelezionato = DocumentManager.getDocumentoSelezionato(this);
            string codFasc = dataTableFascicoli.Rows[key][0].ToString();
            string sysReg = dataTableFascicoli.Rows[key][7].ToString();
            string idTitolario = dataTableFascicoli.Rows[key][9].ToString();
            string systemId = dataTableFascicoli.Rows[key][10].ToString();
            DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(documentoSelezionato);

            //In questa operazione adesso è necessario tener conto dei titolari multipli
            //DocsPaWR.Fascicolo fascicoloSelezionato = FascicoliManager.getFascicoloInClassifica(this, codFasc, sysReg);
            DocsPaWR.Fascicolo fascicoloSelezionato = FascicoliManager.getFascicoloInClassifica(this, codFasc, sysReg, idTitolario, systemId);
            if (fascicoloSelezionato != null)
            {
                DocsPaWR.Folder folder = FascicoliManager.getFolder(this, fascicoloSelezionato);
                string msg = string.Empty;

                string valoreChiaveFasc = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
                if (string.IsNullOrEmpty(valoreChiaveFasc))
                    valoreChiaveFasc = "false";

                DocsPAWA.DocsPaWR.ValidationResultInfo result = FascicoliManager.deleteDocFromProject(this, folder, infoDoc.idProfile, valoreChiaveFasc, fascicoloSelezionato, out msg);
                if (result != null && result.BrokenRules.Length > 0)
                {
                    DocsPAWA.DocsPaWR.BrokenRule br = (DocsPAWA.DocsPaWR.BrokenRule)result.BrokenRules[0];
                    ClientScript.RegisterStartupScript(this.GetType(), "sottonumeroSuccessivo", "alert('" + br.Description + "');", true);
                    return;
                }

                if (msg == string.Empty)
                {
                    //si ricarica il datagrid senza il dato eliminato
                    creazioneDataTableFascicoli();
                    if (m_dataTableFascicoli.DefaultView.Count != 0)
                    {
                        if (this.Datagrid2.Items.Count == 1 && Datagrid2.CurrentPageIndex > 0)
                        {
                            Datagrid2.CurrentPageIndex = Datagrid2.CurrentPageIndex - 1;
                        }
                        this.bindDataGrid();
                    }
                    else
                    {
                        Datagrid2.Visible = false;
                    }

                    this.Datagrid2.SelectedIndex = -1;
                    FascicoliManager.removeFascicoloSelezionato(this);
                }
                else
                {
                    //Response.Write("<script>alert(\"" + msg + "\")</script>");
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert(\"" + msg + "\");", true);
                }
            }
            // FascicoliManager.removeFascicoloSelezionato(this);
        }

        private DocsPAWA.DocsPaWR.Fascicolo getFascicolo(string codiceFascicolo)
        {
            DocsPaWR.Fascicolo fascicoloSelezionato = null;
            if (codiceFascicolo != "")
            {
                //fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice(this, codiceFascicolo);
                //fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice2(this, codiceFascicolo, ddl_titolari.SelectedValue);
                fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice2(this, codiceFascicolo, getIdTitolario(null));
            }
            return fascicoloSelezionato;
        }

        /// <summary>
        /// Metodo per il recupero del fascicolo da codice
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo[] getFascicoli(DocsPAWA.DocsPaWR.Registro registro, string codClassificazione)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            listaFasc = FascicoliManager.getListaFascicoliDaCodice(this, codClassificazione, registro, "R");
            if (listaFasc != null)
                return listaFasc;
            else
                return null;
        }


        private DocsPAWA.DocsPaWR.Fascicolo getFascicoloInClassifica(string codiceFascicolo)
        {
            DocsPaWR.Fascicolo fascicoloSelezionato = null;
            if (codiceFascicolo != "")
            {
                //fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice(this, codiceFascicolo);
                //fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice2(this, codiceFascicolo, ddl_titolari.SelectedValue);
                fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice2(this, codiceFascicolo, getIdTitolario(null));
            }
            return fascicoloSelezionato;
        }

        private void getParametriDatiPaginePrecedenti()
        {
            //recupero parametri dalla sessione impostati
            //da altre pagine
            try
            {
                m_documentoSelezionato = DocumentManager.getDocumentoSelezionato(this);

            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void caricamentoControlliPagina()
        {
            //eseguo l'inizializzazione dei controlli
            //sulla pagina
            try
            {
                loadListaCombo();
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void gestioneParametri()
        {
            //gestisce i parametri per il rendering della 
            //pagina.
            try
            {
                if (!IsPostBack)
                {
                    //se sono nel primo caricamento della pagina,
                    //eseguo il bind della griglia con la sorgente dati creata
                    bindDataGrid();
                }
                else
                {

                    if (m_classificazioneSelezionata != null && Session["Titolario"] != null && Session["Titolario"].ToString() == "Y")
                    {
                        this.txt_codClass.Text = m_classificazioneSelezionata.codice;
                        cercaClassificazioneDaCodice();
                        m_classificazioneSelezionata = null;
                        DocumentManager.setClassificazioneSelezionata(this, m_classificazioneSelezionata);
                        Session.Remove("Titolario");
                    }
                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void buildParametriPagina()
        {
            //creo i dati che dovrà gestire la pagina
            try
            {
                //eseguo l'inizializzazione della ddl dei titolari
                caricaComboTitolari();
                if (ddl_titolari.Items.Count != 0)
                {
                    //eseguo l'inizializzazione dei controlli
                    //sulla pagina
                    caricamentoControlliPagina();
                    caricamentoClassificazioniPadri();
                    creazioneDataTableFascicoli();

                    btn_fascProcedim.Enabled = true;
                    imgFasc.Enabled = true;
                    btn_insInFascicolo.Enabled = true;
                }
                else
                {
                    btn_fascProcedim.Enabled = false;
                    imgFasc.Enabled = false;
                    btn_insInFascicolo.Enabled = false;
                }

            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void setParametriPaginaInSession()
        {
            //memorizzo i parametri che dovranno essere
            //disponibili alla pagina tra le varie 
            //sequenze di postback
            try
            {
                DocumentManager.setDataGridFascicoliContenitori(this, m_dataTableFascicoli);
                DocumentManager.setDataFascicoliNonVisibili(this, this.listaFascNonVisibili);
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void scaricaVariabiliSessione()
        {
            //scarica Variabili Sessione
            try
            {

            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void scaricaPagina()
        {
            //esegue la pulizia della session
            //e scarica la pagina
            try
            {
                scaricaVariabiliSessione();

                string funct = " window.close(); ";
                // Response.Write("<script> " + funct + "</script>");
                ClientScript.RegisterStartupScript(this.GetType(), "close", funct, true);
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void addChiamateJScriptAiComandi()
        {
            //per i controlli HTML lato client, aggiunge le chiamate 
            //ai gestori di evento javascript
            #region btn_titolario
            //String pageName=Request.Url.Segments[Request.Url.Segments.Length-1].ToString();

            string queryString = "codClass='+document.docClassifica.txt_codClass.value+'";

            //this.btn_titolario.Attributes.Add("onclick","ApriTitolario('"+pageName+"','"+queryString+"')");			
            //this.btn_titolario.Attributes.Add("onclick","return btn_titolario_onClick('"+pageName+"','"+queryString+"');");			
            this.btn_titolario.Attributes.Add("onclick", "return btn_titolario_onClick('" + queryString + "');");
            //this.proponiClassifica.Attributes.Add("onclick","return proponiclassifica_onClick();");
            #endregion

            #region txt_codClass
            this.txt_codClass.Attributes.Add("onblur", "__doPostBack('txt_codClass','')");
            #endregion
        }

        #endregion

        #region sezione routine specifiche pagina

        private void DisabilitaTutto()
        {
            this.btn_aggAreaDiLavoro.Enabled = false;
            this.btn_insInFascicolo.Enabled = false;
            this.btn_new.Enabled = false;
            this.btn_new_tit.Enabled = false;
            this.txt_codClass.ReadOnly = true;
            this.txt_codFasc.ReadOnly = true;
            this.btn_fascInAreaDiLav.Enabled = false;
            this.btn_fascProcedim.Enabled = false;
            this.btn_titolario.Enabled = false;
            this.imgFasc.Enabled = false;
            this.OptLst.Enabled = false;
            this.ddl_livello1.Enabled = false;
            this.ddl_livello2.Enabled = false;
            this.ddl_livello3.Enabled = false;
            this.ddl_livello4.Enabled = false;
            this.ddl_livello5.Enabled = false;
            this.ddl_livello6.Enabled = false;
            //Non è possibile modificare la fascicolazione primaria
            string valoreChiave;
            valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_FASC_PRIMARIA");

            if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
            {
                for (int i = 0; i < this.Datagrid2.Items.Count; i++)
                {
                    DataGridItem dgItem = Datagrid2.Items[i];
                    RadioButton rbSelection = dgItem.Cells[5].FindControl("rbSel") as RadioButton;
                    rbSelection.Enabled = false;
                }
            }
        }

        public void viewInfoFascicolo(string codice, string idRegistro, string idTitolario, string systemId)
        {

            //OLD: poichè NELLA 3.1.6 è stato introdotto il concetto di nodi con lo stesso codice
            //sotto registri diversi
            //DocsPaWR.Fascicolo fascicoloSelezionato=FascicoliManager.getFascicoloDaCodice(this,codice);

            DocsPaWR.Fascicolo fascicoloSelezionato = FascicoliManager.getFascicoloInClassifica(this, codice, idRegistro, idTitolario, systemId);

            if (fascicoloSelezionato != null)
            {
                FascicoliManager.setFascicoloSelezionato(this, fascicoloSelezionato);
                loadNewFrameset();
            }

            Session["template"] = null;
        }

        private void loadNewFrameset()
        {
            string newUrl = "../fascicolo/gestioneFasc.aspx?tab=documenti";

            //Response.Write("<script language='javascript'>top.principale.document.location='" + newUrl + "';</script>");
            ClientScript.RegisterStartupScript(this.GetType(), "refresh", "top.principale.document.location='" + newUrl + "';", true);
        }

        private void caricamentoClassificazioniPadri()
        {
            //DocsPaWR.FascicolazioneClassifica[] classifiche = FascicoliManager.GetFigliClassifica(this, null, UserManager.getUtente(this).idAmministrazione);
            //DocsPaWR.FascicolazioneClassifica[] classifiche = FascicoliManager.GetFigliClassifica2(this, null, UserManager.getUtente(this).idAmministrazione, ddl_titolari.SelectedValue);
            DocsPaWR.FascicolazioneClassifica[] classifiche = FascicoliManager.GetFigliClassifica2(this, null, UserManager.getUtente(this).idAmministrazione, getIdTitolario(null));
            caricaCombo(0, classifiche);

        }

        /// <summary>
        /// Creazione di una stringa contenente tutte le descrizioni 
        /// dei sottofascicoli contenenti il documento
        /// </summary>
        /// <param name="folders"></param>
        /// <param name="systemIdFascicolo"></param>
        /// <returns></returns>
        private string GetDescriptionFoldersDocument(DocsPAWA.DocsPaWR.Folder[] folders, string systemIdFascicolo)
        {
            string retValue = string.Empty;

            foreach (DocsPAWA.DocsPaWR.Folder folder in folders)
            {
                if (folder.idFascicolo.Equals(systemIdFascicolo))
                {
                    if (!retValue.Equals(string.Empty))
                        retValue += "\n";

                    retValue += " - " + folder.descrizione;
                }
            }

            if (!retValue.Equals(string.Empty))
                retValue = "Sottofascicoli contenenti il documento:\n\n" + retValue;

            return retValue;
        }

        private void creazioneDataTableFascicoli()
        {
            try
            {
                //costruzione m_dataTableFascicoli
                string codiceRegistro = "";
                string idRegistro = "";

                if (m_documentoSelezionato != null)
                {
                    //RECUPERARE VALORE PER infoDocumento
                    DocsPaWR.InfoDocumento infoDocumento = DocumentManager.getInfoDocumento(m_documentoSelezionato);
                    //Session["
                    DocsPaWR.Fascicolo[] listaFascicoli;

                    string valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_TAB_TRASM_ALL");
                    if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                    {
                        //Nuovo metodo per prendere la lista dei fascicoli senza security
                        listaFascicoli = DocumentManager.GetFascicoliDaDocNoSecurity(this, infoDocumento.idProfile);
                    }
                    else
                    {
                        listaFascicoli = DocumentManager.GetFascicoliDaDoc(this, infoDocumento.idProfile);
                    }

                    // Reperimento sottocartelle contenenti il doc corrente
                    DocsPaWR.Folder[] folders = FascicoliManager.GetFoldersDocument(this, infoDocumento.idProfile);

                    m_dataTableFascicoli = new DataTable();

                    if (listaFascicoli != null)
                    {


                        if (listaFascicoli.Length > 0)
                        {

                            this.txt_numero_fascicoli.Value = listaFascicoli.Length.ToString();
                            //if (listaFascicoli.Length < 2)
                            //{

                            //    //controllo se la fascicolazione è obbligatoria
                            //    if (ConfigSettings.getKey(ConfigSettings.KeysENUM.FASC_RAPIDA_REQUIRED).ToUpper().Equals("TRUE"))
                            //    {
                            //        active = "attivo";
                            //        txt_confirmDel.Value = active;
                            //    }
                            //}
                            //else
                            //{
                            txt_confirmDel.Value = "";
                            //}

                            for (int i = 0; i < listaFascicoli.Length; i++)
                            {
                                DocsPaWR.Fascicolo fasc = listaFascicoli[i];
                                DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fasc.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;

                                idRegistro = fasc.idRegistroNodoTit; // systemId del registro
                                codiceRegistro = fasc.codiceRegistroNodoTit; //codice del Registro
                                if (idRegistro != null && idRegistro == String.Empty)//se il fascicolo è associato a un TITOLARIO con idREGISTRO = NULL
                                    codiceRegistro = "<B>TUTTI</B>";

                                string descrizioneFasc = null;

                                valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_TAB_TRASM_ALL");
                                if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                                {
                                    if (fasc.sicurezzaUtente.Equals("0"))
                                    {
                                        descrizioneFasc = "Descrizione non visualizzabile";
                                        this.listaFascNonVisibili.Add(fasc.codice);
                                    }
                                    else
                                    {
                                        descrizioneFasc = fasc.descrizione;
                                    }
                                }
                                else
                                {
                                    descrizioneFasc = fasc.descrizione;
                                }
                                this.dataSetLFascContDoc1.element1.Addelement1RowInClassifica(fasc.codice, descrizioneFasc, codiceGerarchia, i, this.GetDescriptionFoldersDocument(folders, fasc.systemID), fasc.stato, codiceRegistro, idRegistro, fasc.accessRights, fasc.idTitolario, fasc.systemID, fasc.isFascPrimaria);
                            }
                            m_dataTableFascicoli = this.dataSetLFascContDoc1.Tables[0];
                            DocumentManager.setDataGridFascicoliContenitori(this, m_dataTableFascicoli);
                            DocumentManager.setDataFascicoliNonVisibili(this, listaFascNonVisibili);
                        }
                    }
                }
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        private void bindDataGrid()
        {
            if (m_dataTableFascicoli != null)
            {
                if (m_dataTableFascicoli.DefaultView.Count != 0)
                {
                    this.Datagrid2.DataSource = m_dataTableFascicoli.DefaultView;
                    this.Datagrid2.DataBind();

                    //Usato per eliminare il link dai fascicoli non visibili
                    if (UserManager.ruoloIsAutorized(this, "DO_DEL_DOC_FASC"))
                    {
                        for (int i = 0; i < this.Datagrid2.Items.Count; i++)
                        {
                            DataGridItem dgItem = Datagrid2.Items[i];
                            dgItem.FindControl("img_cancella").Visible = true;
                            LinkButton controlloFascLink = Datagrid2.Items[i].Cells[0].FindControl("LinkButton1") as LinkButton;
                            string controlloFascCod = controlloFascLink.Text;
                            if (this.listaFascNonVisibili.Contains(controlloFascCod))
                            {
                                dgItem.FindControl("LinkButton1").Visible = false;
                                dgItem.FindControl("lbl_codice").Visible = true;
                                dgItem.FindControl("img_cancella").Visible = false;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < this.Datagrid2.Items.Count; i++)
                        {
                            DataGridItem dgItem = Datagrid2.Items[i];
                            LinkButton controlloFascLink = Datagrid2.Items[i].Cells[0].FindControl("LinkButton1") as LinkButton;
                            string controlloFascCod = controlloFascLink.Text;
                            if (this.listaFascNonVisibili.Contains(controlloFascCod))
                            {
                                dgItem.FindControl("LinkButton1").Visible = false;
                                dgItem.FindControl("lbl_codice").Visible = true;
                            }
                        }
                    }


                    string valoreChiave;
                    valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_FASC_PRIMARIA");

                    if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                    {
                        Datagrid2.Columns[5].Visible = true;
                        for (int i = 0; i < this.Datagrid2.Items.Count; i++)
                        {
                            DataGridItem dgItem = Datagrid2.Items[i];
                            RadioButton rbSelection = dgItem.Cells[5].FindControl("rbSel") as RadioButton;
                            string fascPrimaria = ((Label)dgItem.Cells[12].Controls[1]).Text.ToString();
                            if (((Label)dgItem.Cells[1].Controls[1]).Text.ToString().Equals("Descrizione non visualizzabile"))
                                rbSelection.Enabled = false;
                            if (fascPrimaria == "1")
                                rbSelection.Checked = true;
                            else
                                rbSelection.Checked = false;
                        }
                    }
                    else
                        Datagrid2.Columns[5].Visible = false;
                }
            }
        }

        private void caricaComboClassifica(DropDownList ddlDaCaricare, DocsPaWR.FascicolazioneClassifica[] classifiche)
        {
            logger.Info("BEGIN");
            if (ddlDaCaricare != null && classifiche != null)
            {
                string aaaa = ddlDaCaricare.ID;
                ddlDaCaricare.Items.Add("");
                ListItem newItem;
                for (int i = 0; i < classifiche.Length; i++)
                {
                    DocsPaWR.FascicolazioneClassifica classifica = classifiche[i];
                    if (viewCodiceInCombo)//se la chiave VIEW_CODICE_IN_COMBO_CLASSIFICA, sul web.config del WA , è settata a 1 
                    {
                        newItem = new ListItem(classifica.codice + " - " + classifica.descrizione, classifica.codice);
                    }
                    else
                    {
                        newItem = new ListItem(classifica.descrizione, classifica.codice);
                    }

                    ddlDaCaricare.Items.Add(newItem);

                }
            }
            logger.Info("END");
        }

        private void initListaCombo(int indexToStart)
        {
            if (indexToStart >= 0 && indexToStart < CONST_NUMBER_OF_DDL_CONTROLS)
            {
                for (int i = indexToStart; i < CONST_NUMBER_OF_DDL_CONTROLS; i++)
                {
                    DropDownList ddlControl = (DropDownList)this.FindControl((string)m_hashListaNameCombo[i]);
                    ddlControl.Items.Clear();
                }
            }

        }

        private void loadListaCombo()
        {
            m_hashListaNameCombo = new Hashtable(CONST_NUMBER_OF_DDL_CONTROLS);
            for (int i = 0; i < CONST_NUMBER_OF_DDL_CONTROLS; i++)
            {
                string nameOfControl = CONST_BASE_NAME_CONTROL_OF_LIST + (i + CONST_BASE_INDEX_CONTROL_OF_LIST).ToString();
                int keyOfControl = i;
                m_hashListaNameCombo.Add(keyOfControl, nameOfControl);
            }
            Session[SESSION_HashTableListControls] = m_hashListaNameCombo;
        }

        private int indexByElement(DropDownList ddlControl)
        {
            int retValue = -1;

            for (int i = 0; i < m_hashListaNameCombo.Count; i++)
            {
                string ddlItemXName = (string)m_hashListaNameCombo[i];
                if (ddlControl.ID.ToString() == ddlItemXName.ToString())
                {
                    retValue = i;
                    break;
                }
            }
            return retValue;
        }

        private DropDownList elementByIndex(int indexOfElement)
        {
            DropDownList retValue = null;
            if (m_hashListaNameCombo != null && indexOfElement >= 0 && indexOfElement <= m_hashListaNameCombo.Count - 1)
            {
                retValue = (DropDownList)this.FindControl(m_hashListaNameCombo[indexOfElement].ToString());
            }
            return retValue;
        }

        private bool cercaClassificazioneDaCodice()
        {
            logger.Info("BEGIN");
            string codClassificazione = this.txt_codClass.Text.ToString();
            Session["res"] = null;
            bool res = false;

            this.numFasc = 0;
            if (!string.IsNullOrEmpty(codClassificazione))
            {
                this.listaFasc = getFascicoli(UserManager.getRegistroSelezionato(this), codClassificazione);
                if (listaFasc != null && listaFasc.Length > 0)
                {
                    this.numFasc = listaFasc.Length;
                }
            }

            //if (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && listaFasc.Length > 1)
            if (!string.IsNullOrEmpty(codClassificazione))
                if (checkRicercaFasc(codClassificazione) == "NO_RICERCA")
                {
                    //Page.RegisterStartupScript("cod_class", "<script>alert('Codice presente su più titolari. Selezionare un titolario.');</script>");
                    ClientScript.RegisterStartupScript(this.GetType(), "apriSceltaTitolario", "ApriSceltaTitolario('" + codClassificazione + "');", true);
                    //txt_codClass.Text = "";
                    txt_codFasc.Text = "";
                    ClearDDL(1, 6);
                    string funct_dx = "top.principale.iFrame_dx.document.location='tabDoc.aspx';";
                    //Response.Write("<script language='javascript'> " + funct_dx + "</script>");
                    ClientScript.RegisterStartupScript(this.GetType(), "ref", funct_dx, true);
                    //return false;
                    return true;
                }

            //if (ddl_titolari.SelectedItem.Text != "Tutti i titolari")
            if (!string.IsNullOrEmpty(codClassificazione))
                if (checkRicercaFasc(codClassificazione) == "SI_RICERCA")
                {

                    //DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchiaDaCodice(this, codClassificazione, UserManager.getUtente(this).idAmministrazione);
                    //DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchiaDaCodice2(this, codClassificazione, UserManager.getUtente(this).idAmministrazione, ddl_titolari.SelectedValue);
                    DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchiaDaCodice2(this, codClassificazione, UserManager.getUtente(this).idAmministrazione, getIdTitolario(codClassificazione));

                    //esegue routine di svuotamento di tutte le combobox
                    initListaCombo(0);
                    if (gerClassifica != null)
                    {
                        //Recupero l'idTitolario di appartenenza presente solo sull'ultimo nodo figlio
                        string idTitolatio = gerClassifica[gerClassifica.Length - 1].idTitolario;
                        if (idTitolatio != null && idTitolatio != "")
                            ddl_titolari.SelectedValue = idTitolatio;
                        //res= true;
                        DocsPaWR.FascicolazioneClassifica prevClassifica = null;
                        int lastIndex = 0;
                        for (int i = 0; i < gerClassifica.Length; i++)
                        {
                            DocsPaWR.FascicolazioneClassifica classifica = (DocsPAWA.DocsPaWR.FascicolazioneClassifica)gerClassifica[i];
                            DocsPaWR.FascicolazioneClassifica classPadre = prevClassifica;
                            prevClassifica = classifica;
                            //Elisa 11/08/2005 gestione nodo titolario ReadOnly
                            //Session.Add("cha_ReadOnly", classifica.cha_ReadOnly);
                            Session.Add("classificaSelezionata", classifica);
                            //
                            //recupera tutte le classificazioni figlie di primo
                            //livello della classifica selezionata
                            //DocsPaWR.FascicolazioneClassifica[] classifiche = FascicoliManager.GetFigliClassifica(this, classPadre, UserManager.getUtente(this).idAmministrazione);
                            DocsPaWR.FascicolazioneClassifica[] classifiche = FascicoliManager.GetFigliClassifica2(this, classPadre, UserManager.getUtente(this).idAmministrazione, getIdTitolario(codClassificazione));

                            //modifica per far si che se il nodo del titolario richiesto è presente
                            //nell'amministrazione di interesse ma non è visibili all'utente loggato
                            //a DocsPa venga comunque fornito l'alert a video.
                            for (int k = 0; k < classifiche.Length; k++)
                            {
                                if (codClassificazione.ToUpper() == classifiche[k].codice.ToUpper())
                                {
                                    res = true;
                                    k = classifiche.Length;
                                }
                            }
                            //fine mod
                            caricaCombo(i, classifiche);
                            selectValueInCombo(i, classifica.codice);
                            lastIndex = i;
                        }
                        //mod 07/07/2005
                        if (res == false)
                        {
                            return res;
                        }
                        //fine modifica
                        gestioneSelezioneElemento(elementByIndex(lastIndex));


                    }
                    else
                    {
                        caricamentoClassificazioniPadri();
                    }
                }

            logger.Info("END");
            return res;
        }

        private void selectValueInCombo(int indexCombo, string valore)
        {
            DropDownList ddlCombo = elementByIndex(indexCombo);
            if (ddlCombo != null)
            {
                ListItem itemToSelect = ddlCombo.Items.FindByValue(valore);
                if (itemToSelect != null)
                {
                    itemToSelect.Selected = true;
                }
            }
        }

        private void caricaCombo(int indexCombo, DocsPaWR.FascicolazioneClassifica[] classifiche)
        {
            DropDownList ddlCombo = elementByIndex(indexCombo);
            if (ddlCombo != null)
            {
                caricaComboClassifica(ddlCombo, classifiche);
            }
        }

        private void CaricaComboRegistri(DropDownList ddl, DocsPAWA.DocsPaWR.Registro[] userReg)
        {
            if (userReg != null)
            {
                for (int i = 0; i < userReg.Length; i++)
                {
                    ddl.Items.Add(userReg[i].codRegistro);
                    ddl.Items[i].Value = userReg[i].systemId;
                }
            }
        }

        private void setCodiceClassificaInTextBox(int indexSelectedCombo)
        {
            DropDownList ddlControl = elementByIndex(indexSelectedCombo);
            string codClassifica = "";

            if (ddlControl != null)
            {
                if (ddlControl.SelectedIndex >= 0)
                    codClassifica = ddlControl.SelectedItem.Value.ToString();
                if (codClassifica == "")
                {
                    if (indexSelectedCombo > 0)
                    {
                        DropDownList ddlPrevControl = elementByIndex(indexSelectedCombo - 1);
                        if (ddlPrevControl.SelectedIndex >= 0)
                            codClassifica = ddlPrevControl.SelectedItem.Value.ToString();
                    }
                    else
                    {
                        codClassifica = "";
                    }
                }

                this.txt_codClass.Text = codClassifica;
                this.txt_codFasc.Text = codClassifica;
                this.h_codFasc.Value = codClassifica.ToString();
            }
        }


        private void visualizzaFascicoli(tipoVisualizzazioneFascicoli tipoVis)
        {
            logger.Info("BEGIN");
            string tipoVisualizzazione = tipoVis.ToString();
            ViewState[VIEWSTATE_TipoVisFascicoli] = tipoVisualizzazione;

            string newUrl = null;
            if (tipoVis == tipoVisualizzazioneFascicoli.tvfDaAreaDiLavoro)
            {
                //visualizzazione fascicoli da AREA DI LAVORO

                newUrl = CONST_navigatePageSearch + "?"
                    + QUERYSTRING_tipoVisualizzazione + "=" + tipoVisualizzazione
                    + "&" + QUERYSTRING_idTitolario + "=" + this.ddl_titolari.SelectedValue + "&pagina=ricfascADL";
            }
            //else if (tipoVis==tipoVisualizzazioneFascicoli.tvfProcedimentali)
            //ora la lentina deve ricercare anche il fascicolo generale
            else if (tipoVis == tipoVisualizzazioneFascicoli.tvfAll)
            {
                //visualizzazione fascicoli PROCEDIMENTALI
                string codClassifica = this.txt_codClass.Text.ToString();
                newUrl = CONST_navigatePageSearch + "?"
                    + QUERYSTRING_tipoVisualizzazione + "=" + tipoVisualizzazione
                    + "&" + QUERYSTRING_codClassifica + "=" + this.Server.UrlEncode(codClassifica)
                    + "&" + QUERYSTRING_idTitolario + "=" + this.ddl_titolari.SelectedValue;
                //newUrl = this.Server.HtmlEncode(newUrl);

            }

            //Response.Write("<script>parent.parent.iFrame_dx.document.location='"+newUrl+"';</script>");	
            //Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='" + newUrl + "';</script>");
            ClientScript.RegisterStartupScript(this.GetType(), "ref2", "top.principale.iFrame_dx.document.location='" + newUrl + "';", true);
            logger.Info("END");
        }

        private void aggiungiAdAreaDiLavoro()
        {
            if (m_documentoSelezionato != null)
            {
                DocumentManager.addAreaLavoro(this, m_documentoSelezionato);
                string msg = "Documento aggiunto all'area di lavoro";
                // Response.Write("<script>alert(\"" + msg + "\");</script>");
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert(\"" + msg + "\");", true);
            }
        }

        private DocsPAWA.DocsPaWR.Fascicolo getFascicoloSelezionato()
        {
            DocsPaWR.Fascicolo fascicoloSelezionato = null;
            DocsPaWR.Fascicolo fascicoloSelezionatoInGriglia = null;

            string codFascicolo = this.h_codFasc.Value.ToString();
            this.txt_codFasc.Text = codFascicolo.ToString();

            if (this.txt_codFasc.Text != "")
            {
                fascicoloSelezionatoInGriglia = FascicoliManager.getFascicoloSelezionato(this);

                string codiceFascicolo = txt_codFasc.Text;

                if (fascicoloSelezionatoInGriglia != null)
                {
                    if (fascicoloSelezionatoInGriglia.codice.ToLower() != codiceFascicolo.ToLower())
                    {
                        //il codice presente nella casella di testo non corrisponde al
                        //codice del fascicolo seelzionato:
                        //significa che ho digitato manualmente un codice, o che 
                        //ricaricando la classifica è stato impostato il codice del
                        //fascicolo generale della classifica

                        //fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice(this, codiceFascicolo);
                        //fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice2(this, codiceFascicolo,ddl_titolari.SelectedValue);
                        fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice2(this, codiceFascicolo, getIdTitolario(null));

                        //ricalcolo la root folder poichè se entro qui significa che l'utente
                        //ha digitato un codice nuovo nel campo codice Fascicolo
                        if (fascicoloSelezionato != null)
                        {
                            //ABBATANGELI GINALUIGI - Correzione bug INC000000076800
                            DocsPaWR.Folder selectedFolder = FascicoliManager.getFolderSelezionato(this);
                            if (selectedFolder == null)
                                selectedFolder = FascicoliManager.getFolder(this, fascicoloSelezionato);

                            FascicoliManager.setFolderSelezionato(this, selectedFolder);
                        }
                    }
                    else
                    {
                        fascicoloSelezionato = fascicoloSelezionatoInGriglia;
                    }
                }
                else
                {
                    //fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice(this, codiceFascicolo);
                    //fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice2(this, codiceFascicolo,ddl_titolari.SelectedValue);
                    fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice2(this, codiceFascicolo, getIdTitolario(null));
                }
            }

            return fascicoloSelezionato;
        }

        private void inserisciDocumentoInFascicolo()
        {
            logger.Info("BEGIN");
            DocsPaWR.Folder selectedFolder;
            bool outValue = false;

            if (m_documentoSelezionato != null)
            {
                m_fascicoloSelezionato = getFascicoloSelezionato();

                if (m_fascicoloSelezionato != null)
                {
                    //se lo stato del fascicolo è chiuso non si deve inserire il documento
                    if (m_fascicoloSelezionato.stato == "C")
                    {
                        // Page.RegisterStartupScript("", "<script>alert('Attenzione il fascicolo è chiuso!')</script>");
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Attenzione il fascicolo è chiuso!');", true);
                        return;
                    }
                    if (m_fascicoloSelezionato.accessRights != null && Convert.ToInt32(m_fascicoloSelezionato.accessRights) <= 45)
                    {
                        // Page.RegisterStartupScript("", "<script>alert('Attenzione il fascicolo è in lettura!')</script>");
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Attenzione il fascicolo è in lettura!');", true);
                        return;
                    }

                    string valoreChiaveConsentiClass = string.Empty;
                    valoreChiaveConsentiClass = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
                    if (m_fascicoloSelezionato.isFascConsentita != null && m_fascicoloSelezionato.isFascConsentita == "0" && !string.IsNullOrEmpty(valoreChiaveConsentiClass) && valoreChiaveConsentiClass.Equals("1"))
                    {
                        // Page.RegisterStartupScript("", "<script>alert('Non è possibile inserire documenti nel fascicolo. Selezionare un nodo foglia')</script>");
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Non è possibile inserire documenti nel fascicolo. Selezionare un nodo foglia');", true);
                        return;
                    }

                    /* Vedo se in sessione cè una folder. Questo è il caso in cui si classifica quando:
                    1) l'utente ha selezionato un fascicolo dalla lista a seguito di una ricerca
                    2) l'utente ha digitato un codice fascicolo a mano dopo una ricerca di fascicoli
                    */

                    selectedFolder = FascicoliManager.getFolderSelezionato(this);

                    if (selectedFolder == null)
                    {
                        //Ricavo la folder dal fascicolo selezionato					
                        selectedFolder = FascicoliManager.getFolder(this, m_fascicoloSelezionato);
                    }

                    if (selectedFolder != null)
                    {
                        //Inserisco/Classifico il documento nel folder/sottoFascicolo selezionato
                        String message = String.Empty;
                        DocumentManager.addDocumentoInFolder(this, m_documentoSelezionato.systemId, selectedFolder.systemID, false, out outValue, out message);

                    }
                    //dopo la classifica rimuovo la Folder Selezionata
                    FascicoliManager.removeFolderSelezionato(this);
                    //Faillace 10/03/2015 Questo impedisce l'autoclassificazione in fase di protocollazione
                    FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
                    if (outValue) // SE il doc è già nella folder indicata
                    {
                        //  Page.RegisterStartupScript("advise", "<script>alert('ATTENZIONE: il documento è già classificato nel fascicolo indicato.')</script>");
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('ATTENZIONE: il documento è già classificato nel fascicolo indicato.')", true);
                        return;
                    }
                    string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1].ToString();
                    ClientScript.RegisterStartupScript(this.GetType(), "ref_dx", "top.principale.iFrame_dx.document.location='tabDoc.aspx';window.document.location='" + pageName + "';", true);
                    // Response.Write("<script>top.principale.iFrame_dx.document.location='tabDoc.aspx';window.document.location='" + pageName + "';</script>");


                }
                else
                {
                    this.txt_codFasc.Text = this.txt_codClass.Text;
                    this.h_codFasc.Value = this.txt_codClass.Text;

                    // Page.RegisterStartupScript("alert", "<script>alert('Attenzione! codice fascicolo non presente!')</script>");
                    //string s = "<SCRIPT language='javascript'>try{document.getElementById('" + txt_codFasc.ID + "').focus();} catch(e){} </SCRIPT>";
                    string s = "try{document.getElementById('" + txt_codFasc.ID + "').focus();} catch(e){}";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Attenzione! codice fascicolo non presente!');", true);
                    //      ClientScript.RegisterStartupScript(this.GetType(), "refresh", s , true);
                    // RegisterStartupScript("focus", s);
                    return;
                }
            }
            logger.Info("END");
        }

        //		private void inserisciDocumentoInFascicolo_old()
        //		{
        //			if (m_documentoSelezionato!=null)
        //			{
        //				m_fascicoloSelezionato=getFascicoloSelezionato();
        //				if (m_fascicoloSelezionato!=null)
        //				{
        //					//se lo stato del fascicolo è chiuso non si deve inserire il documento
        //					if(m_fascicoloSelezionato.stato=="C")
        //					{
        //						Page.RegisterStartupScript("","<script>alert('Attenzione il fascicolo è chiuso!')</script>"); 
        //						return;	
        //					}
        //					DocumentManager.addDocumentoInFascicolo(this,m_documentoSelezionato.systemId,m_fascicoloSelezionato.systemID);
        //					
        //					string pageName=Request.Url.Segments[Request.Url.Segments.Length-1].ToString();
        //					Response.Write("<script>window.document.location='"+pageName+"';</script>");
        //					
        //				}
        //				else
        //				{
        //					Page.RegisterStartupScript("alert","<script>alert('Attenzione! codice fascicolo non presente!')</script>"); 
        //					string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_codFasc.ID + "').focus();</SCRIPT>";
        //					RegisterStartupScript("focus", s);
        //					return;
        //				}
        //			}
        //			
        //		}

        private void gestioneSelezioneElemento(DropDownList ddlControl)
        {
            int indexCombo = indexByElement(ddlControl);
            if (indexCombo >= 0)
            {
                string codClassifica = "";
                if (ddlControl.SelectedIndex >= 0)
                    codClassifica = ddlControl.Items[ddlControl.SelectedIndex].Value.ToString();

                setCodiceClassificaInTextBox(indexCombo);

                DocsPaWR.FascicolazioneClassifica[] classificheFiglie;
                //11/08/2005 elisa
                if (this.txt_codClass.Text != "")
                    codClassifica = this.txt_codClass.Text;
                //
                if (codClassifica != "")
                {
                    //carico la gerarchia per la classifica selezionata
                    //DocsPaWR.FascicolazioneClassifica[] gerClassifiche = FascicoliManager.getGerarchiaDaCodice(this, codClassifica, UserManager.getUtente(this).idAmministrazione);
                    //DocsPaWR.FascicolazioneClassifica[] gerClassifiche = FascicoliManager.getGerarchiaDaCodice2(this, codClassifica, UserManager.getUtente(this).idAmministrazione,ddl_titolari.SelectedValue);
                    DocsPaWR.FascicolazioneClassifica[] gerClassifiche = FascicoliManager.getGerarchiaDaCodice2(this, codClassifica, UserManager.getUtente(this).idAmministrazione, getIdTitolario(codClassifica));

                    if (gerClassifiche != null)
                    {
                        //classifica selezionata
                        DocsPaWR.FascicolazioneClassifica classifica = gerClassifiche[gerClassifiche.Length - 1];
                        //Elisa 11/08/2005 gestione nodo titolario ReadOnly
                        //Session.Add("cha_ReadOnly", classifica.cha_ReadOnly);
                        Session.Add("classificaSelezionata", classifica);
                        //
                        //classificheFiglie = FascicoliManager.GetFigliClassifica(this, classifica, UserManager.getUtente(this).idAmministrazione);
                        //classificheFiglie = FascicoliManager.GetFigliClassifica2(this, classifica, UserManager.getUtente(this).idAmministrazione,ddl_titolari.SelectedValue);
                        classificheFiglie = FascicoliManager.GetFigliClassifica2(this, classifica, UserManager.getUtente(this).idAmministrazione, getIdTitolario(codClassifica));

                        txt_protoPratica.Text = classifica.numProtoTit;
                    }
                    else
                    {
                        classificheFiglie = null;
                    }
                }
                else
                {
                    classificheFiglie = null;
                }

                DropDownList ddlNext = elementByIndex(indexCombo + 1);
                if (ddlNext != null)
                {
                    initListaCombo(indexCombo + 1);
                    caricaComboClassifica(ddlNext, classificheFiglie);
                }

            }
        }


        #endregion

        #region sezione eventi webcontrols

        protected override void OnInit(EventArgs e)
        {
            this.Initialize();
            base.OnInit(e);
        }

        /// <summary>
        /// Inizializzazione dati
        /// </summary>
        private void Initialize()
        {
            this.dataSetLFascContDoc1 = new DocsPAWA.dataSet.DataSetLFascContDoc();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetLFascContDoc1)).BeginInit();
            this.dataSetLFascContDoc1.DataSetName = "DataSetLFascContDoc";
            this.dataSetLFascContDoc1.Locale = new System.Globalization.CultureInfo("en-US");
            ((System.ComponentModel.ISupportInitialize)(this.dataSetLFascContDoc1)).EndInit();
            this.msg_inserisciDoc.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_inserisciDoc_GetMessageBoxResponse);
            this.msgEliminaUltimoDoc.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msgEliminaUltimoDoc_GetMessageBoxResponse);
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            logger.Info("BEGIN");
            try
            {


                //if (ConfigSettings.getKey(ConfigSettings.KeysENUM.FASC_RAPIDA_REQUIRED).ToUpper().Equals("TRUE"))
                //{

                //    if (txt_numero_fascicoli.Value == "1")
                //        this.txt_confirmDel.Value = "attivo";
                //}

                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
                if (schedaDocumento.documenti != null && schedaDocumento.documenti.Length > 0)
                {
                    FileManager.setSelectedFile(this, schedaDocumento.documenti[0]);
                }
                // controllo se il documento è annullato - in tal caso disabilito tutto
                if (schedaDocumento.protocollo != null)
                {
                    DocsPaWR.ProtocolloAnnullato protAnnull;
                    protAnnull = schedaDocumento.protocollo.protocolloAnnullato;
                    if (protAnnull != null && protAnnull.dataAnnullamento != null && !protAnnull.Equals(""))
                    {
                        DisabilitaTutto();
                    }
                }

                //abilitazione delle funzioni in base al ruolo
                UserManager.disabilitaFunzNonAutorizzate(this);

                //this.btn_new.Attributes.Add("onclick", "top.principale.iFrame_dx.document.location='tabDoc.aspx'; ApriFinestraNewFascNewTit('" + this.txt_codClass.Text + "','docClassifica');");
                //this.btn_new_tit.Attributes.Add("onclick", "top.principale.iFrame_dx.document.location='tabDoc.aspx'; ApriFinestraNewFascNewTit('" + this.txt_codClass.Text + "','docClassifica','insertNewNodoTitolario.aspx','','"+ddl_titolari.SelectedValue+"');");
                this.btn_new_tit.Attributes.Add("onclick", "top.principale.iFrame_dx.document.location='tabDoc.aspx'; ApriFinestraNewFascNewTit('" + this.txt_codClass.Text + "','docClassifica','insertNewNodoTitolario.aspx','','" + getIdTitolario(txt_codClass.Text) + "');");

                // gestione nodo titolario ReadOnly
                //if (Session["cha_ReadOnly"] != null)
                if (Session["classificaSelezionata"] != null)
                {
                    DocsPaWR.FascicolazioneClassifica classifica = (DocsPaWR.FascicolazioneClassifica)Session["classificaSelezionata"];

                    //if (((bool)Session["cha_ReadOnly"] == true) || (this.txt_codClass.Text == null || this.txt_codClass.Text.Equals("")))
                    if (classifica.cha_ReadOnly || (this.txt_codClass.Text == null || this.txt_codClass.Text.Equals("")))
                    {
                        impostaAbilitazioneNuovoFascNuovoTit();
                        //this.btn_new.Enabled = false;
                        //this.btn_new_tit.Enabled = false;
                    }
                    else
                    {
                        if (ClientScript.IsStartupScriptRegistered("alert"))
                        {
                            this.btn_new.Enabled = false;
                            this.btn_new_tit.Enabled = false;
                        }
                        else
                        {
                            if (this.txt_codClass.Text != "" && checkRicercaFasc(this.txt_codClass.Text) == "SI_RICERCA")
                            {
                                impostaAbilitazioneNuovoFascNuovoTit();
                            }
                        }
                    }
                }
                else
                {
                    if (this.txt_codClass.Text != null && !this.txt_codClass.Text.Equals("") && this.ddl_titolari.SelectedItem.Text.ToUpper() != "TUTTI I TITOLARI")
                    {
                        this.btn_new.Enabled = true;
                        this.btn_new_tit.Enabled = true;
                    }
                    else
                    {
                        this.btn_new.Enabled = false;
                        this.btn_new_tit.Enabled = false;
                    }
                }

                this.verificaHMdiritti(schedaDocumento);
                //il pulsante classifica deve essere attivo nel caso di trasmissione cross AOO in inps
                //questo blocco disabilitava il pulsante in inps nel caso di trasm. cross AOO
                /*if (UserManager.isFiltroAooEnabled(this))
                {
                    if (schedaDocumento != null && schedaDocumento.protocollo != null)
                    {
                        if (btn_insInFascicolo.Enabled)
                        {
                            DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);
                            btn_insInFascicolo.Enabled = UserManager.verifyRegNoAOO(schedaDocumento, userRegistri);
                        }
                    }
                }*/

                //Giornale ricontri
                cercaRiscontroMittente(schedaDocumento);
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
            logger.Info("END");
        }

        private bool vis_ddl_registri(string tipoProto)
        {
            bool visibile = false;


            if (tipoProto != null && tipoProto != String.Empty)
            {
                switch (tipoProto)
                {
                    case "G":
                        visibile = true;
                        break;
                }
            }

            return visibile;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            logger.Info("BEGIN");

            Session.Remove("refreshDxPageVisualizzatore");

            //Se la chiave sul web.config del Wa è settata a 1 allora le comboBox in DocClassifica
            //riporteranno oltra alla descrizione anche il codice del nodo di titolario
            viewCodiceInCombo = (System.Configuration.ConfigurationManager.AppSettings["VIEW_CODICE_IN_COMBO_CLASSIFICA"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["VIEW_CODICE_IN_COMBO_CLASSIFICA"].Equals("1"));

            // Se il documento è bloccato, viene disabilitato il pulsante di classifica
            this.btn_insInFascicolo.Enabled = !this.IsSelectedDocumentCheckedOut();

            //if (ConfigSettings.getKey(ConfigSettings.KeysENUM.FASC_RAPIDA_REQUIRED).ToUpper().Equals("TRUE"))
            //{

            //    if(txt_numero_fascicoli.Value == "1" )
            //        this.txt_confirmDel.Value = "attivo";
            //}




            //Abilitazione indice sistematico
            if (wws.isEnableIndiceSistematico())
                pnl_indiceSis.Visible = true;

            DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(this);

            //this.linkCss.Href = DocsPAWA.utils

            // Inizializzazione controllo verifica acl
            if ((schedaDocumento != null) && (schedaDocumento.inCestino != "1") && (schedaDocumento.systemId != null))
            {
                this.InitializeControlAclDocumento();
            }
            userRegistri = UserManager.getListaRegistri(this);

            this.RegisterClientScript("nascondi", "nascondi();");

            impostaVisibiltaBtnFascTit();

            infoUtente = UserManager.getInfoUtente(this);

            //inizio gestione registri su doc Classifica
            if (!IsPostBack)
            {
               
                ddl_titolari.SelectedIndex = 1;

                bool isVisible;
                if (schedaDocumento != null)
                {
                    if (schedaDocumento.tipoProto != null && schedaDocumento.tipoProto != String.Empty)
                    {

                        isVisible = vis_ddl_registri(schedaDocumento.tipoProto);

                        /* se isVisible = true (il documento è grigio)
                            quindi rendo visibile il pannello dei registri */
                        this.pnl_registri.Visible = isVisible;

                        if (isVisible)
                        {
                            CaricaComboRegistri(ddl_registri, userRegistri);//I REGISTRI li carico solo la prima volta
                            if (userRegistri != null && userRegistri.Length == 1)
                            {
                                this.ddl_registri.Enabled = false;
                            }
                            settaRegistroSelezionato();
                        }
                    }
                }
            }


            if (this.Session["urlRicercaFascicoli"] != null)
            {
                string urlRicFasc = this.Session["urlRicercaFascicoli"].ToString();
                this.Session.Remove("urlRicercaFascicoli");
                // Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='" + urlRicFasc + "';</script>");
                ClientScript.RegisterStartupScript(this.GetType(), "refresh_dx", "top.principale.iFrame_dx.document.location='" + urlRicFasc + "';", true);
                return;
            }

            try
            {
                if (OptLst.SelectedItem.Value.Equals("Cod"))
                {
                    //Disabilitare le liste
                    EnabledisabledDDL(1, 6, false);
                    txt_codFasc.Enabled = true;
                    txt_codFasc.ReadOnly = false;
                    txt_codClass.Enabled = true;
                    txt_codClass.ReadOnly = false;
                }
                else
                {
                    //Abilitare le liste
                    EnabledisabledDDL(1, 6, true);
                    txt_codClass.Enabled = false;
                    txt_codClass.ReadOnly = true;
                    txt_codFasc.Enabled = false;
                    txt_codFasc.ReadOnly = true;
                }


                Utils.startUp(this);
                //recupera i parametri dalla session o vengono
                //costruiti appositamente in base ai parametri
                //ricevuti.
                getParametri();

                string IdClass = (string)Session["classificazione"];

                //gestisce i parametri per il rendering della 
                //pagina.
                gestioneParametri();

                if ((IdClass != "") && (IdClass != null))
                {
                    txt_codClass.Text = IdClass;

                    Session["classificazione"] = null;
                    CaricaFascicoli();
                }

                //Verifico se provengo da una selezione a causa di :
                //1. risoluzione di un fascicolo su più di un titolario 
                //2. risoluzione di più nodi di titolario a partire da un indice sistematico
                if (Session["idTitolarioSelezionato"] != null)
                {
                    if (Session["codiceNodoSelezionato"] != null)
                    {
                        txt_codClass.Text = Session["codiceNodoSelezionato"].ToString();
                        Session.Remove("codiceNodoSelezionato");
                    }

                    ddl_titolari.SelectedValue = Session["idTitolarioSelezionato"].ToString();
                    if (cercaClassificazioneDaCodice())
                    {
                        Session["newClass"] = "S";
                        FascicoliManager.removeClassificazioneSelezionata(this);
                    }
                    impostaAbilitazioneNuovoFascNuovoTit();
                    Session.Remove("idTitolarioSelezionato");
                }
                
                //FINE VERIFICA SELEZIONE

                if ((schedaDocumento.inCestino != null && schedaDocumento.inCestino == "1") || (schedaDocumento.inArchivio != null && schedaDocumento.inArchivio == "1"))
                {
                    DisabilitaTutto();
                }

                //Protocollo Titolario
                string contatoreTit = wws.isEnableContatoreTitolario();
                if (!string.IsNullOrEmpty(contatoreTit))
                {
                    lbl_protoTitolario.Text = contatoreTit;
                    pnl_PrototolloTitolario.Visible = true;
                    btn_new_tit.Text = contatoreTit;
                    btn_new_tit.ToolTip = "";
                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
            logger.Info("END");
        }

        private void CaricaFascicoli()
        {
            logger.Info("BEGIN");
            try
            {
                bool res = false;
                res = cercaClassificazioneDaCodice();
                if (!res)
                {
                    //Response.Write("<script>alert(\" Attenzione! codice classifica non presente! \");</script>");
                    string alert = "<script>alert(\" Attenzione! codice di classifica non presente! \");</script>";
                    RegisterStartupScript("cod_class", alert);
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_codClass.ID + "').focus(); </SCRIPT>";
                    RegisterStartupScript("focus", s);
                    ddl_titolari.SelectedIndex = 0;
                    return;
                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
            logger.Info("END");
        }

        protected void ddl_livelloControl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                //Celeste
                Session["res"] = null;
                DropDownList ddlControl = (DropDownList)sender;
                gestioneSelezioneElemento(ddlControl);

                //this.h_codFasc.Value = this.txt_codClass.Text;
                //this.txt_codFasc.Text = this.txt_codClass.Text;
                impostaAbilitazioneNuovoFascNuovoTit();
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        protected void txt_codFasc_TextChanged(object sender, System.EventArgs e)
        {
            this.h_codFasc.Value = this.txt_codFasc.Text;
        }

        protected void txt_codClass_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                this.ddl_livello1.Enabled = false;
                this.ddl_livello2.Enabled = false;
                this.ddl_livello3.Enabled = false;
                this.ddl_livello4.Enabled = false;
                this.ddl_livello5.Enabled = false;
                this.ddl_livello6.Enabled = false;

                bool res = false;
                string funct_dx = "";

                res = cercaClassificazioneDaCodice();

                Session["res"] = res.ToString();

                /* Session["res"] viene messo a false in due casi
                 * CASO 1 - codice classifica è inesistente: in tal caso lo lascio a false ;
                 * CASO 2 - codice classifica non è stato specificato: in tal caso metto a NULL la variabile*/

                //CASO 1
                if (!res && txt_codClass.Text != "")
                {
                    //Response.Write("<script>alert(\" Attenzione! codice classifica non presente! \");</script>");
                    //string alert = "<script>alert(\" Attenzione! codice di classifica non presente! \");</script>";
                    // RegisterStartupScript("cod_class", alert);
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert(\" Attenzione! codice di classifica non presente! \");", true);
                    // string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_codClass.ID + "').focus(); </SCRIPT>";
                    //RegisterStartupScript("focus", s);
                    ClientScript.RegisterStartupScript(this.GetType(), "no_class", "document.getElementById('" + txt_codClass.ID + "').focus();", true);
                    //ricarica il frame destro
                    funct_dx = "top.principale.iFrame_dx.document.location='tabDoc.aspx';";
                    // Response.Write("<script language='javascript'> " + funct_dx + "</script>");
                    ClientScript.RegisterStartupScript(this.GetType(), "ref_dx", funct_dx, true);
                    this.txt_codFasc.Text = "";
                    this.h_codFasc.Value = "";
                    ddl_titolari.SelectedIndex = 0;
                    return;
                }

                //CASO 2
                if (!res && txt_codClass.Text == "")
                {
                    //RESET DELLA VARIABILE POICHE' IL CODICE CLASSIFICA NON è SBAGLIATO
                    //MA NON è STATO SPECIFICATO
                    Session["res"] = null;
                    this.txt_codFasc.Text = "";
                    this.h_codFasc.Value = "";
                }

                /* A questo punto Session["res"] può assumere tre valori:
                * null: se il codice di classifica non è stato specificato;
                * "True": se il codice di classifica specificato è valido;
                * "False": se il codice di classifica specificato non è valido */

                //ricarica il frame destro dopo la ricerca di un nuovo cod classifica
                funct_dx = "top.principale.iFrame_dx.document.location='tabDoc.aspx';";
                //Response.Write("<script language='javascript'> " + funct_dx + "</script>");
                ClientScript.RegisterStartupScript(this.GetType(), "refdx", funct_dx, true);

            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        protected void btn_aggAreaDiLavoro_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                try
                {
                    aggiungiAdAreaDiLavoro();
                }
                catch (System.Exception es)
                {
                    ErrorManager.redirect(this, es);
                }
            }
        }

        protected void btn_insInFascicolo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            logger.Info("BEGIN");
            // Se il documento risulta bloccato non può essere classificato
            if (this.IsSelectedDocumentCheckedOut())
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "NonClassificabile",
                    "alert('Non è possibile effettuare la classificazione in quanto il documento principale oppure almeno un suo allegato risulta bloccato.');",
                    true);
            else
                this.ClassificaDocumento();
            logger.Info("END");
        }

        /// <summary>
        /// Funzione per la classificazione del documento
        /// </summary>
        private void ClassificaDocumento()
        {
            logger.Info("BEGIN");
            try
            {
                if (!this.GetControlAclDocumento().AclRevocata)
                {
                    if ((txt_codClass.Text != "" || txt_codFasc.Text != "" || h_codFasc.Value != ""))
                    {

                        if (Session["res"] == null || Session["res"].Equals("True"))
                        {
                            Fascicolo fasc = getFascicoloSelezionato();
                            if ((m_documentoSelezionato.privato == "1" || m_documentoSelezionato.personale == "1") && fasc != null && fasc.tipo != "G")
                            {
                                string messaggio;
                                if (m_documentoSelezionato.privato == "1")
                                    messaggio = InitMessageXml.getInstance().getMessage("insDocPrivato");
                                else
                                    messaggio = InitMessageXml.getInstance().getMessage("insDocPersonale");
                                msg_inserisciDoc.Confirm(messaggio);
                            }
                            else
                            {

                                inserisciDocumentoInFascicolo();
                            }

                        }
                        else
                        {

                            //Response.Write("<script>alert(\" Attenzione! codice classifica non presente! \");</script>");
                            // string alert1 = "<script>alert(\" Attenzione! codice classifica non presente! \");</script>";
                            string alert1 = "alert(\" Attenzione! codice classifica non presente! \");";
                            if (!ClientScript.IsStartupScriptRegistered("cod_class"))
                            {
                                //RegisterStartupScript("cod_class", alert1);
                                ClientScript.RegisterStartupScript(this.GetType(), "cod_class", alert1, true);
                            }

                            // string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_codClass.ID + "').focus(); </SCRIPT>";
                            // RegisterStartupScript("focus", s);
                            string s = "document.getElementById('" + txt_codClass.ID + "').focus();";
                            ClientScript.RegisterStartupScript(this.GetType(), "focus", s, true);
                            this.txt_codFasc.Text = "";
                            this.h_codFasc.Value = "";
                            ddl_titolari.SelectedIndex = 0;
                        }

                    }
                    else
                    {
                        if (!ClientScript.IsStartupScriptRegistered("cod_class"))
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Attenzione! inserire il codice fascicolo o il codice classifica!');", true);
                            string s = "try{document.getElementById('" + txt_codFasc.ID + "').focus();} catch(e){}";
                            ClientScript.RegisterStartupScript(this.GetType(), "focus", s, true);
                        }

                    }
                }

            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
            logger.Info("END");
        }

        /// <summary>
        /// Funzione per verificare se il documento è in stato di blocco
        /// </summary>
        /// <returns>True se il documento è bloccato</returns>
        private bool IsSelectedDocumentCheckedOut()
        {
            // La scheda documento
            DocsPAWA.DocsPaWR.SchedaDocumento doc;

            // Il valore da restituire
            bool toReturn;

            // Prelevamento della scheda documento
            doc = DocumentManager.getDocumentoSelezionato();

            // Verifica dello stato di blocco del documento
            toReturn = CheckInOut.CheckInOutServices.IsCheckedOutDocument(
                doc.systemId,
                doc.docNumber,
                UserManager.getInfoUtente(this),
                true);

            // Restituzione del risultato
            return toReturn;

        }

        private void msg_inserisciDoc_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                inserisciDocumentoInFascicolo();
            }
        }

        protected void Datagrid2_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            this.Datagrid2.CurrentPageIndex = e.NewPageIndex;
            bindDataGrid();
        }

        protected void btn_fascProcedim_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                bool res = false;
                if (this.txt_codClass.Text.Equals(""))
                {
                    if (!ClientScript.IsStartupScriptRegistered("cod_class"))
                        ClientScript.RegisterStartupScript(this.GetType(), "cod_class", "alert('Inserire un codice di classifica valido');", true);

                    DocsPAWA.Utils.SetFocus(txt_codClass.ClientID, this);

                    //ricarica il frame destro
                    string funct_dx = "top.principale.iFrame_dx.document.location='tabDoc.aspx';";
                    //Response.Write("<script language='javascript'> " + funct_dx + "</script>");
                    ClientScript.RegisterStartupScript(this.GetType(), "pag_class", "" + funct_dx + "", true);
                    return;
                }
                res = cercaClassificazioneDaCodice();
                if (res)
                {
                    FascicoliManager.removeFascicoloSelezionato(this);
                    FascicoliManager.removeFolderSelezionato(this);
                    //23/03/2006
                    //Ora la lentina deve ricercare anche il fascicolo generale
                    //visualizzaFascicoli(tipoVisualizzazioneFascicoli.tvfProcedimentali);

                    visualizzaFascicoli(tipoVisualizzazioneFascicoli.tvfAll);
                    //
                }
                Session["res"] = res.ToString();
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        protected void btn_fascInAreaDiLav_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                this.h_codFasc.Value = this.txt_codClass.Text;
                this.txt_codFasc.Text = this.txt_codClass.Text;

                FascicoliManager.removeFascicoloSelezionato(this);
                FascicoliManager.removeFolderSelezionato(this);
                visualizzaFascicoli(tipoVisualizzazioneFascicoli.tvfDaAreaDiLavoro);
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        protected void img_indiceSis_Click(object sender, ImageClickEventArgs e)
        {
            if (txt_indiceSis.Text != null && txt_indiceSis.Text != "")
            {
                //Recupero la lista dei nodi associati all'indice selezionato
                ArrayList listaNodi = new ArrayList(wws.getCodNodiByIndice(txt_indiceSis.Text, ddl_titolari.SelectedValue));

                //Nessun nodo trovato
                if (listaNodi.Count == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "noNodi", "alert('Nessun nodo associato alla voce di indice.');", true);
                    return;
                }

                //E' stato trovato un solo nodo
                if (listaNodi.Count == 1)
                {
                    txt_codClass.Text = ((DocsPAWA.DocsPaWR.VoceIndiceSistematico)listaNodi[0]).codiceNodo;
                    txt_codClass_TextChanged(txt_codClass, new EventArgs());
                    return;
                }
                //Sono stato trovati due o più nodi
                else
                {
                    ClearDDL(1, 6);
                    txt_codClass.Text = "";
                    txt_codFasc.Text = "";
                    string queryString = this.Server.UrlEncode("indice=" + txt_indiceSis.Text + "&idTitolario=" + ddl_titolari.SelectedValue + "&TipoChiamata=IndiceSistematico");
                    ClientScript.RegisterStartupScript(this.GetType(), "apriSceltaNodo", "ApriSceltaNodo('" + queryString + "');", true);
                    return;
                }
            }
        }

        #endregion

        #region Eventi Vari
        private void SetDefaultButton()
        {
            DocsPAWA.Utils.DefaultButton(this, ref this.txt_codFasc, ref this.btn_insInFascicolo);
        }

        protected void Datagrid2_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName == "viewInfoFascicolo")
            {
                string codice = e.CommandArgument.ToString();
                string idRegistro = ((Label)this.Datagrid2.Items[e.Item.ItemIndex].Cells[8].Controls[1]).Text.ToString();
                string idTitolario = ((Label)this.Datagrid2.Items[e.Item.ItemIndex].Cells[10].Controls[1]).Text.ToString();
                string systemId = ((Label)this.Datagrid2.Items[e.Item.ItemIndex].Cells[11].Controls[1]).Text.ToString();
                //nuovo metto in sessione il registro del fascicolo selezionato
                //se idRegistro è null lascio quello della combo (ma va bene ???)
                if (idRegistro != null && idRegistro != string.Empty)
                {
                    DocsPaWR.Registro registroFascicolo = UserManager.getRegistroBySistemId(this, idRegistro);
                    if (registroFascicolo != null)
                    {
                        //setto il registro del fasciolo altrimenti in
                        //../fascicolo/gestioneFasc.aspx?tab=documenti si vede un registro diverso da quello del fasciolo
                        UserManager.setRegistroSelezionato(this, registroFascicolo);
                    }
                }
                else
                {
                    UserManager.removeRegistroSelezionato(this);
                }

                viewInfoFascicolo(codice, idRegistro, idTitolario, systemId);
            }
            if (e.CommandName == "update")
            {
                int a = this.Datagrid2.Items[e.Item.ItemIndex].Controls.Count;
                string stato = ((Label)this.Datagrid2.Items[e.Item.ItemIndex].Cells[7].Controls[1]).Text.ToString();
                int accessRights = Convert.ToInt32(((Label)this.Datagrid2.Items[e.Item.ItemIndex].Cells[9].Controls[1]).Text);
                if (stato == "C")
                {
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "no_elimina", "alert('Non è possibile eliminare un documento da un fascicolo chiuso.');", true);
                    //Response.Write("<Script language='javascript'>alert('Non è possibile eliminare un documento da un fascicolo chiuso.');</Script>");
                }
                else
                {
                    if (DocumentManager.getDocumentoSelezionato() != null
                        && (DocumentManager.getDocumentoSelezionato().inCestino != null && DocumentManager.getDocumentoSelezionato().inCestino == "1"))
                    {
                        Response.Write("<Script language='javascript'>alert('Impossibile eliminare un documento rimosso da un fascicolo');</Script>");

                    }
                    else
                        if (accessRights <= 45)
                        {
                            Response.Write("<Script language='javascript'>alert('Impossibile eliminare un documento da un fascicolo in sola lettura.');</Script>");
                        }
                        else
                        {
                            int key = e.Item.DataSetIndex;
                            eliminaDocDaFasciolo(key);
                        }
                }
            }
        }

        private void EnabledisabledDDL(int indexMin, int indexMax, bool enabled)
        {
            DropDownList ddl;
            DropDownList ddlprev;
            string DDLName = "ddl_livello";

            for (int index = indexMin; index <= indexMax; index++)
            {
                ddl = (DropDownList)this.FindControl(DDLName + index.ToString());

                if (enabled)
                {
                    if (ddl.ID.Substring(DDLName.Length).ToString().Equals(indexMin.ToString()))
                        ddl.Enabled = true;
                    else
                    {
                        ddlprev = (DropDownList)this.FindControl(DDLName + Convert.ToString(index - 1));
                        if (ddlprev.SelectedValue.Length > 0)
                            ddl.Enabled = true;
                        else
                            ddl.Enabled = false;
                    }

                }
                else
                    ddl.Enabled = false;
            }

            return;
        }

        private void ClearDDL(int indexMin, int indexMax)
        {
            DropDownList ddl;
            for (int index = indexMin; index <= indexMax; index++)
            {
                ddl = (DropDownList)this.FindControl("ddl_livello" + index.ToString());
                ddl.SelectedIndex = -1;
            }
            return;
        }

        protected void OptLst_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (IsStartupScriptRegistered("alert"))
            {
                RegisterStartupScript("alert", null);
            }
            //this.h_codFasc.Value =  this.txt_codFasc.Text ;
            //se cambio da codice a livello resetto il campo codice fascicolo
            //si presuppone che l'utente voglia fare una nuova operazione
            //this.txt_codFasc.Text = this.txt_codClass.Text;
            //this.h_codFasc.Value = this.txt_codClass.Text;
        }

        protected void btn_titolario_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //E' necessario che sia selezionato un titolario e non la voce "tutti i titolari"
            if (ddl_titolari.Enabled && ddl_titolari.SelectedIndex == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "selezionareUnTitolario", "alert('Selezionare un titolario.');", true);
                return;
            }

            Session["Titolario"] = "Y";
            this.h_codFasc.Value = this.txt_codClass.Text;
            this.txt_codFasc.Text = this.txt_codClass.Text;

            FascicoliManager.removeFolderSelezionato(this);
            FascicoliManager.removeFascicoloSelezionato(this);

            if (!this.IsStartupScriptRegistered("apriModalDialog"))
            {
                //string scriptString = "<SCRIPT>ApriTitolario('codClass=" + txt_codClass.Text + "&idTit=" + ddl_titolari.SelectedValue + "','gestClass')</SCRIPT>";
                string scriptString = "<SCRIPT>ApriTitolario('codClass=" + txt_codClass.Text + "&idTit=" + getIdTitolario(txt_codClass.Text) + "','gestClass')</SCRIPT>";
                this.RegisterStartupScript("apriModalDialog", scriptString);
            }
        }

        protected void btnFiltriRicFasc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                if (this.txt_codClass.Text.Equals(""))
                {
                    if (!ClientScript.IsStartupScriptRegistered("cod_class"))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "cod_class", "alert('Inserire un codice di classifica valido');", true);
                        string s = "document.getElementById('" + txt_codClass.ID + "').focus();";
                        ClientScript.RegisterStartupScript(this.GetType(), "focus", s, true);
                        return;
                    }
                }

                string scriptString = "<SCRIPT>ApriRicercaFascicoli('" + "CodClass=" + this.txt_codClass.Text + "','" + ddl_titolari.SelectedIndex + "')</SCRIPT>";
                this.RegisterStartupScript("ApriRicercaFascicoli", scriptString);

            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        protected void imgFasc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            FascicoliManager.removeFascicoloSelezionato(this);
            this.h_codFasc.Value = this.txt_codClass.Text;
            this.txt_codFasc.Text = this.txt_codClass.Text;
            FascicoliManager.removeFolderSelezionato();

            /* Session["res"] può assumere tre valori:
             * null: se il codice di classifica non è stato specificato;
             * "True": se il codice di classifica specificato è valido;
             * "False": se il codice di classifica specificato non è valido */

            if (Session["res"] == null || (string)Session["res"] == "True")
            {
                string cod_class = Server.UrlEncode(txt_codClass.Text);
                //   string scriptString = "<SCRIPT>ApriRicercaFascicoli('" + cod_class + "','" + ddl_titolari.SelectedIndex + "');</SCRIPT>";
                //string scriptString = "<SCRIPT>ApriRicercaFascicoli();</SCRIPT>";
                //  this.RegisterStartupScript("ApriRicercaFascicoli", scriptString);
                string scriptString = "ApriRicercaFascicoli('" + cod_class + "','" + ddl_titolari.SelectedIndex + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "ricerca_fascicoli", scriptString, true);
            }
            else
            {
                if (!ClientScript.IsStartupScriptRegistered("cod_class"))
                    ClientScript.RegisterStartupScript(this.GetType(), "cod_class", "alert('Attenzione: codice di classifica non presente');", true);
                //RegisterStartupScript("cod_class", "<script>alert('Attenzione: codice di classifica non presente'); </script>");
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

        protected void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // Quando seleziono un nuovo registro devo:
            // 1) svuotare le combo;
            // 2) pulire il campi di testo relativo al codice, al fascicolo selezionato in precedenza e all'indice sistematico
            // 3) rimuovere la classificazione e l'eventuale folder settata in precedenza.
            // 4) mettere in sessione il registro selezionato
            // 5) caricare le combobox con il titolario associato al nuovo registro selezionato

            // 1) 
            try
            {
                initListaCombo(0);

                // 2)
                this.txt_codClass.Text = String.Empty;
                this.txt_codFasc.Text = String.Empty;
                this.h_codFasc.Value = "";
                this.txt_indiceSis.Text = string.Empty;
                //

                // 3)
                FascicoliManager.removeClassificazioneSelezionata(this);
                FascicoliManager.removeFolderSelezionato(this);

                // 4)
                settaRegistroSelezionato();

                // 5)
                caricamentoClassificazioniPadri();
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        protected void Datagrid2_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!ClientScript.IsStartupScriptRegistered(scriptKey))
            {
                // string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                string scriptString = scriptValue;
                // this.Page.RegisterStartupScript(scriptKey, scriptString);
                ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString, true);
            }
        }

        private void impostaVisibiltaBtnFascTit()
        {
            if (UserManager.ruoloIsAutorized(this, "FASC_NUOVO"))
            {
                btn_new.Visible = true;
            }
            else
            {
                btn_new.Visible = false;
            }

            if (UserManager.ruoloIsAutorized(this, "DO_TITOLARIO"))
            {
                btn_new_tit.Visible = true;
            }
            else
            {
                btn_new_tit.Visible = false;
            }
        }

        protected void btn_new_Click(object sender, ImageClickEventArgs e)
        {
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                //   RegisterStartupScript("createNewFasc", "<script>OnClickNewFascicolo('" + this.txt_codClass.Text + "');</script>");
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                {
                    //ClientScript.RegisterStartupScript(this.GetType(), "createNewFasc", "<script>OnClickNewFascicolo('" + this.txt_codClass.Text + "','1','" + ddl_titolari.SelectedValue + "');</script>");
                    ClientScript.RegisterStartupScript(this.GetType(), "createNewFasc", "<script>OnClickNewFascicolo('" + this.txt_codClass.Text + "','1','" + getIdTitolario(this.txt_codClass.Text) + "');</script>");
                }
                else
                {
                    //ClientScript.RegisterStartupScript(this.GetType(), "createNewFasc", "<script>OnClickNewFascicolo('" + this.txt_codClass.Text + "','0', '"+ddl_titolari.SelectedValue+"');</script>");
                    ClientScript.RegisterStartupScript(this.GetType(), "createNewFasc", "<script>OnClickNewFascicolo('" + this.txt_codClass.Text + "','0', '" + getIdTitolario(this.txt_codClass.Text) + "');</script>");
                }
            }
        }

        protected void h_codFasc_ServerChange(object sender, EventArgs e)
        {
            this.txt_codFasc.Text = this.h_codFasc.Value;
        }

        //Questo metodo serve a verificare la ricerca tramite codice classifica i casi sono i seguenti :
        //1. Selezione "Tutti i titolari" il codice restituisce un solo fascicolo OK si effettua la ricerca
        //2. Selezione <<uno specifico titolario>> OK si effettua la ricerca
        //3. Selezione "Tutti i titolari" il codice restituisce piu' di un fascicolo NO la ricerca viene bloccata e si chiede la selezione di un titolario
        private string checkRicercaFasc(string codClassificazione)
        {
            // DocsPaWR.Fascicolo[] listaFasc = getFascicoli(UserManager.getRegistroSelezionato(this), codClassificazione);
            string result = "SI_RICERCA";

            if (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && this.numFasc > 1)
                result = "NO_RICERCA";

            if (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && this.numFasc == 1)
                result = "SI_RICERCA";

            return result;
        }

        private string getIdTitolario(string codClassificazione)
        {
            if (codClassificazione != null && codClassificazione != "")
            {
                //  DocsPaWR.Fascicolo[] listaFasc = getFascicoli(UserManager.getRegistroSelezionato(this), codClassificazione);

                //In questo caso il metodo "GetFigliClassifica2" funzionerebbe male
                //per questo viene restituti l'idTitolario dell'unico fascicolo risolto
                if (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && this.listaFasc != null && this.listaFasc.Length == 1)
                {
                    DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)listaFasc[0];
                    return fasc.idTitolario;
                }
            }

            //In tutti gli altri casi è sufficiente restituire il value degli item della
            //ddl_Titolario in quanto formati secondo le specifiche di uno o piu' titolari
            return ddl_titolari.SelectedValue;
        }
        #endregion

        #region Titolari
        private void caricaComboTitolari()
        {
            logger.Info("BEGIN");
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
                    if (valueTutti.IndexOf(',') == -1)
                        valueTutti = valueTutti + "," + valueTutti;

                    ListItem it = new ListItem("Tutti i titolari", valueTutti);
                    ddl_titolari.Items.Insert(0, it);
                }
                OptLst.SelectedIndex = 0;
                OptLst.Enabled = false;
                txt_codClass.Enabled = true;

                //se la chiave di db è a 1, seleziono di default il titolario attivo
                if (utils.InitConfigurationKeys.GetValue("0", "FE_CHECK_TITOLARIO_ATTIVO").Equals("1") || utils.InitConfigurationKeys.GetValue("0", "FE_CHECK_TITOLARIO_ATTIVO").Equals("2"))
                {
                    int indexTitAtt=0;
                    foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                    {
                        if (titolario.Stato == DocsPaWR.OrgStatiTitolarioEnum.Attivo)
                        {
                            ddl_titolari.SelectedIndex = ++indexTitAtt;
                            OptLst.Enabled = true;
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
                if (titolario.Stato != DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    ddl_titolari.Items.Add(it);
                }
                ddl_titolari.Enabled = false;
            }

            //Imposto le etichette per il titolario e i suoi livelli
            if (listaTitolari != null && listaTitolari.Count > 0)
            {
                DocsPaWR.OrgTitolario titolario = (DocsPaWR.OrgTitolario)listaTitolari[0];
                if (!string.IsNullOrEmpty(titolario.EtichettaTit))
                    lbl_Titolari.Text = titolario.EtichettaTit;
                if (!string.IsNullOrEmpty(titolario.EtichettaLiv1))
                    lbl_livello1.Text = titolario.EtichettaLiv1;
                if (!string.IsNullOrEmpty(titolario.EtichettaLiv2))
                    lbl_livello2.Text = titolario.EtichettaLiv2;
                if (!string.IsNullOrEmpty(titolario.EtichettaLiv3))
                    lbl_livello3.Text = titolario.EtichettaLiv3;
                if (!string.IsNullOrEmpty(titolario.EtichettaLiv4))
                    lbl_livello4.Text = titolario.EtichettaLiv4;
                if (!string.IsNullOrEmpty(titolario.EtichettaLiv5))
                    lbl_livello5.Text = titolario.EtichettaLiv5;
                if (!string.IsNullOrEmpty(titolario.EtichettaLiv6))
                    lbl_livello6.Text = titolario.EtichettaLiv6;
                //Abilito le ddl in funzione del livello di profondità del titolario
                try
                {
                    int maxLivTitolario = Convert.ToInt16(titolario.MaxLivTitolario);
                    for (int i = 6; i > maxLivTitolario; i--)
                    {
                        DropDownList ddlControl = (DropDownList)this.FindControl("ddl_livello" + i.ToString());
                        Label labelControl = (Label)this.FindControl("lbl_livello" + i.ToString());
                        labelControl.Visible = false;
                        ddlControl.Visible = false;
                    }
                }
                catch (Exception e) { }
            }
            logger.Info("END");
        }

        protected void ddl_titolari_SelectedIndexChanged(object sender, EventArgs e)
        {
            initListaCombo(0);
            txt_codClass.Text = string.Empty;
            txt_protoPratica.Text = string.Empty;
            txt_codClass.Enabled = true;
            txt_codClass.ReadOnly = false;

            txt_codFasc.Text = string.Empty;
            txt_codFasc.Enabled = true;
            txt_codFasc.ReadOnly = false;

            impostaAbilitazioneNuovoFascNuovoTit();
            caricamentoClassificazioniPadri();
        }

        private void impostaAbilitazioneNuovoFascNuovoTit()
        {
            //E' selezionata la voce tutti i titolari
            if (ddl_titolari.Enabled && ddl_titolari.SelectedIndex == 0)
            {
                OptLst.SelectedIndex = 0;
                OptLst.Enabled = false;
                EnabledisabledDDL(1, 6, false);
                btn_new.Enabled = false;
                btn_new_tit.Enabled = false;
            }
            else
            {
                //Verifico se il titolario selezionato è attivo o meno
                //DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(ddl_titolari.SelectedValue);
                bool chiave = true;
                //NUOVA FUNZIONE POSSO CREARE NUOVI FASCICOLI IN TUTTI I TITOLARI ANCHE QUELLI CHIUSI
                DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(getIdTitolario(null));
                if (utils.InitConfigurationKeys.GetValue("0", "BE_FASC_TUTTI_TIT").Equals("1"))
                {
                    OptLst.Enabled = true;
                    if (txt_codClass.Text != null && txt_codClass.Text != "")
                    {
                        if (Session["classificaSelezionata"] != null)
                        {
                            DocsPaWR.FascicolazioneClassifica classifica = (DocsPaWR.FascicolazioneClassifica)Session["classificaSelezionata"];
                            btn_new.Enabled = true;
                            btn_new_tit.Enabled = (classifica.bloccaNodiFigli == "SI" ? false : true);
                        }
                    }
                    else
                    {
                        btn_new.Enabled = false;
                        btn_new_tit.Enabled = false;
                    }
                }
                else
                {
                    switch (titolario.Stato)
                    {
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            OptLst.Enabled = true;
                            if (txt_codClass.Text != null && txt_codClass.Text != "")
                            {
                                //btn_new.Enabled = true;
                                //btn_new_tit.Enabled = true;

                                if (Session["classificaSelezionata"] != null)
                                {
                                    DocsPaWR.FascicolazioneClassifica classifica = (DocsPaWR.FascicolazioneClassifica)Session["classificaSelezionata"];
                                    btn_new.Enabled = !classifica.cha_ReadOnly;
                                    btn_new_tit.Enabled = (classifica.bloccaNodiFigli == "SI" ? false : true);
                                }
                            }
                            else
                            {
                                btn_new.Enabled = false;
                                btn_new_tit.Enabled = false;

                            }
                            break;
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            OptLst.Enabled = true;
                            btn_new.Enabled = false;
                            btn_new_tit.Enabled = false;
                            break;
                    }
                }
            }
        }


        #endregion

        #region Gestione controllo acl documento

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

        protected void Datagrid2_DataBound(object sender, DataGridItemEventArgs e)
        {
            /*   if (e.Item.ItemType == ListItemType.Pager)
               {
                   if (UserManager.ruoloIsAutorized(this, "DO_DEL_DOC_FASC"))
                   {
                       if (e.Item.Cells.Count > 0)
                       {
                           e.Item.Cells[5].Visible = true;
                       }
                   }
               }*/
        }

        private void verificaHMdiritti(DocsPaWR.SchedaDocumento schedaDocumento)
        {
            //disabilitazione dei bottoni in base all'autorizzazione di HM 
            //sul documento
            if (schedaDocumento != null && schedaDocumento.accessRights != null && schedaDocumento.accessRights != "")
            {
                if (UserManager.disabilitaButtHMDiritti(schedaDocumento.accessRights))
                {
                    //bottoni che devono essere disabilitati in caso
                    //di diritti di sola lettura


                    //bottoni che devono essere disabilitati in caso
                    //di documento trasmesso con "Worflow" e ancora da accettare
                    if (UserManager.disabilitaButtHMDirittiTrasmInAccettazione(schedaDocumento.accessRights))
                    {
                        this.btn_aggAreaDiLavoro.Enabled = false;
                        //this.btn_insInFascicolo.Enabled = false;
                        //this.btn_new.Enabled = false;
                        this.btn_new_tit.Enabled = false;
                        this.btn_titolario.Enabled = false;
                    }

                    //bottoni che devono essere disabilitati in casi di
                    //documento in stato finale
                    if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
                    {
                        if (schedaDocumento.tipologiaAtto != null)
                        {
                            DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoDoc(schedaDocumento.tipologiaAtto.systemId, UserManager.getUtente(this).idAmministrazione, this);
                            if (dg != null)
                            {
                                if (verificaStatoFinale(schedaDocumento, dg))
                                {
                                    this.btn_aggAreaDiLavoro.Enabled = false;
                                    this.btn_insInFascicolo.Enabled = false;
                                    this.btn_new.Enabled = false;
                                    this.btn_new_tit.Enabled = false;
                                    this.btn_titolario.Enabled = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void txt_protoPratica_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_protoPratica.Text))
            {
                ArrayList listaNodiTitolario = new ArrayList(wws.getNodiFromProtoTit(UserManager.getRegistroSelezionato(this), UserManager.getUtente(this).idAmministrazione, txt_protoPratica.Text, ddl_titolari.SelectedValue));

                //Nessun nodo trovato
                if (listaNodiTitolario.Count == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "nessunNodoTrovato", "alert('Nessun nodo di titolario corrispondente al numero inserito.');", true);
                }

                //Trovato un solo nodo si procede alla valorizzazione delle combo
                if (listaNodiTitolario.Count == 1)
                {
                    this.txt_codClass.Text = ((DocsPAWA.DocsPaWR.OrgNodoTitolario)listaNodiTitolario[0]).Codice;
                    this.ddl_titolari.SelectedValue = ((DocsPAWA.DocsPaWR.OrgNodoTitolario)listaNodiTitolario[0]).ID_Titolario;
                    if (cercaClassificazioneDaCodice())
                    {
                        Session["newClass"] = "S";
                        FascicoliManager.removeClassificazioneSelezionata(this);
                    }
                }

                //Trovato più di un nodo si procede alla richiesta di scelta
                if (listaNodiTitolario.Count > 1)
                {
                    string queryString = this.Server.UrlEncode("indice=" + txt_protoPratica.Text + "&idTitolario=" + ddl_titolari.SelectedValue + "&TipoChiamata=ProtocolloTitolario");
                    ClientScript.RegisterStartupScript(this.GetType(), "apriSceltaNodo", "ApriSceltaNodo('" + queryString + "');", true);
                }
            }
        }

        protected void cercaRiscontroMittente(DocsPaWR.SchedaDocumento schedaDocumento)
        {
            if (wws.isEnableRiferimentiMittente() &&
                    schedaDocumento.tipoProto == "A" &&
                    !string.IsNullOrEmpty(((DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente.systemId) &&
                    !string.IsNullOrEmpty(schedaDocumento.riferimentoMittente)
                    )
            {
                //Verifico l'abilitazione del pulsante
                buttonGiornaleRiscontri.Visible = true;
                buttonGiornaleRiscontri.ImageUrl = "../images/rubrica/l_exp_o_grigia.gif";
                buttonGiornaleRiscontri.ToolTip = "Crea Riscontro";
                buttonGiornaleRiscontri.CommandName = "creaRiscontro";
                if (m_dataTableFascicoli.DefaultView.Count > 0)
                {
                    buttonGiornaleRiscontri.Enabled = true;
                    buttonGiornaleRiscontri.CommandName = "creaRiscontro";
                }
                else
                {
                    buttonGiornaleRiscontri.Enabled = false;
                }

                //Verifico se esiste un riscontro
                DocsPaWR.RiscontroMittente riscontro = new DocsPAWA.DocsPaWR.RiscontroMittente();
                riscontro.riferimentoMittente = schedaDocumento.riferimentoMittente.Split('$')[0].ToString();
                riscontro.idCorrGlobaliMittente = ((DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente.systemId;
                DocsPaWR.RiscontroMittente riscontroResult = FascicoliManager.cercaRiscontroMittente(riscontro);
                if (riscontroResult != null)
                {
                    buttonGiornaleRiscontri.ImageUrl = "../images/cancella.gif";
                    buttonGiornaleRiscontri.ToolTip = "Elimina Riscontro";
                    buttonGiornaleRiscontri.CommandName = "eliminaRiscontro";
                }

                if (!IsPostBack)
                {
                    ////Verifico se esiste un riscontro
                    //DocsPaWR.RiscontroMittente riscontro = new DocsPAWA.DocsPaWR.RiscontroMittente();
                    //riscontro.riferimentoMittente = schedaDocumento.riferimentoMittente;
                    //riscontro.idCorrGlobaliMittente = ((DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente.systemId;
                    //DocsPaWR.RiscontroMittente riscontroResult = FascicoliManager.cercaRiscontroMittente(riscontro);
                    if (riscontroResult != null)
                    {
                        //buttonGiornaleRiscontri.ImageUrl = "../images/cancella.gif";
                        //buttonGiornaleRiscontri.ToolTip = "Elimina Riscontro";
                        //buttonGiornaleRiscontri.CommandName = "eliminaRiscontro";

                        //Imposto il codice di classifica se presente
                        if (!string.IsNullOrEmpty(riscontroResult.codClassificaDestinatario))
                            txt_codClass.Text = riscontroResult.codClassificaDestinatario;

                        //Imposto il codice di fascicolo se presente
                        if (!string.IsNullOrEmpty(riscontroResult.codFascicoloDestinatario))
                            txt_codFasc.Text = riscontroResult.codFascicoloDestinatario;

                        //Imposto il titolario se presente
                        if (!string.IsNullOrEmpty(riscontroResult.idTitolarioDestinatario))
                            ddl_titolari.SelectedValue = riscontroResult.idTitolarioDestinatario;

                        //Per quanto riguarda il protocollo titolario non ho bisogno di effettuare nessun settaggio
                        //perchè se la classifica selezionata ha un protocollo titolario quest'ultimo viene riportato automaticamente
                        //Richiamo il metodo seguente perchè con il codice classifica popolato verranno riempite
                        //correttamente le combo di classificazione
                        if (!string.IsNullOrEmpty(txt_codClass.Text))
                            cercaClassificazioneDaCodice();

                        apriRicercaFasc();
                    }
                }
            }
        }

        protected void buttonGiornaleRiscontri_Click(object sender, EventArgs e)
        {
            DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
            DocsPaWR.RiscontroMittente riscontroMittente = new DocsPAWA.DocsPaWR.RiscontroMittente();
            if (wws.isEnableRiferimentiMittente() &&
                    schedaDocumento.tipoProto == "A" &&
                    !string.IsNullOrEmpty(((DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente.systemId) &&
                    !string.IsNullOrEmpty(schedaDocumento.riferimentoMittente)
                    )
            {
                switch (((ImageButton)sender).CommandName)
                {
                    case "creaRiscontro":
                        riscontroMittente.riferimentoMittente = schedaDocumento.riferimentoMittente;
                        riscontroMittente.idCorrGlobaliMittente = ((DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente.systemId;

                        //Ultima classifica
                        string codiceClassifica = DocumentManager.GetClassificaDoc(this, schedaDocumento.systemId);
                        riscontroMittente.codClassificaDestinatario = codiceClassifica;

                        //Prendendo il fascicoli[0] prendo l'ultimo fascicolo di classifica            
                        DocsPaWR.Fascicolo[] fascicoli = DocumentManager.GetFascicoliDaDoc(this, schedaDocumento.systemId);
                        if (fascicoli.Length > 0)
                        {
                            riscontroMittente.codFascicoloDestinatario = fascicoli[0].codice;
                            riscontroMittente.idRegistroDestinatario = fascicoli[0].idRegistroNodoTit;
                        }

                        //Recupero la gerachia dell'ultima classifica. Prendendo il massimo di quest'array recupero l'ultimo nodo di classifica
                        DocsPaWR.FascicolazioneClassifica[] gerarchia = FascicoliManager.getGerarchiaDaCodice2(this, codiceClassifica, UserManager.getInfoUtente().idAmministrazione, fascicoli[0].idTitolario);
                        if (gerarchia.Length > 0)
                        {
                            DocsPaWR.FascicolazioneClassifica classifica = gerarchia[gerarchia.Length - 1];
                            riscontroMittente.protocolloTitolarioDestinatario = classifica.numProtoTit;
                            riscontroMittente.idTitolarioDestinatario = classifica.idTitolario;
                        }

                        //Creo il riscontro 
                        FascicoliManager.creaRiscontroMittente(riscontroMittente);
                        ClientScript.RegisterStartupScript(this.GetType(), "riscontroMittente", "alert('Riscontro creato : " + riscontroMittente.riferimentoMittente.Replace("\\", "\\\\").Split('$')[0].ToString() + " --> " + codiceClassifica.Replace("\\", "\\\\") + "');", true);

                        //Modifica Riscontro
                        //Imposto il codice di classifica se presente
                        if (!string.IsNullOrEmpty(riscontroMittente.codClassificaDestinatario))
                            txt_codClass.Text = riscontroMittente.codClassificaDestinatario;

                        //Imposto il codice di fascicolo se presente
                        if (!string.IsNullOrEmpty(riscontroMittente.codFascicoloDestinatario))
                            txt_codFasc.Text = riscontroMittente.codFascicoloDestinatario;

                        //Imposto il titolario se presente
                        if (!string.IsNullOrEmpty(riscontroMittente.idTitolarioDestinatario))
                            ddl_titolari.SelectedValue = riscontroMittente.idTitolarioDestinatario;

                        //Per quanto riguarda il protocollo titolario non ho bisogno di effettuare nessun settaggio
                        //perchè se la classifica selezionata ha un protocollo titolario quest'ultimo viene riportato automaticamente
                        //Richiamo il metodo seguente perchè con il codice classifica popolato verranno riempite
                        //correttamente le combo di classificazione
                        if (!string.IsNullOrEmpty(txt_codClass.Text))
                            cercaClassificazioneDaCodice();

                        apriRicercaFasc();

                        //fine modifica riscontro

                        break;

                    case "eliminaRiscontro":
                        riscontroMittente.riferimentoMittente = schedaDocumento.riferimentoMittente;
                        riscontroMittente.idCorrGlobaliMittente = ((DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente.systemId;

                        //Elimino il riscontro mittente
                        FascicoliManager.eliminaRiscontroMittente(riscontroMittente);

                        //Ripulisco i campi
                        txt_codClass.Text = string.Empty;
                        txt_codFasc.Text = string.Empty;
                        txt_protoPratica.Text = string.Empty;
                        txt_indiceSis.Text = string.Empty;
                        ddl_titolari.SelectedIndex = 0;
                        ClearDDL(1, 6);

                        string funct_dx = "top.principale.iFrame_dx.document.location='tabDoc.aspx';";
                             //Response.Write("<script language='javascript'> " + funct_dx + "</script>");
                             ClientScript.RegisterStartupScript(this.GetType(), "pag_class", "" + funct_dx + "", true);
                        break;
                }
            }
        }

        private void msgEliminaUltimoDoc_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                int key = (int)Session["key"];
                eliminaDocDaFasciolo(key);
            }
        }

        protected void rbSel_CheckedChanged(object sender, EventArgs e)
        {
            DataGridItem item = null;
            for (int i = 0; i < Datagrid2.Items.Count; i++)
            {
                RadioButton rbSelection = Datagrid2.Items[i].Cells[5].FindControl("rbSel") as RadioButton;
                if (rbSelection.Checked)
                    item = this.Datagrid2.Items[i];
            }
            if (item != null)
            {
                string idProject = ((Label)item.Cells[11].Controls[1]).Text.ToString();
                DocumentManager.cambiaFascPrimaria(this, idProject, DocumentManager.getDocumentoSelezionato().systemId);
                creazioneDataTableFascicoli();
                if (m_dataTableFascicoli != null && m_dataTableFascicoli.DefaultView.Count != 0)
                {
                    if (this.Datagrid2.Items.Count == 1 && Datagrid2.CurrentPageIndex > 0)
                    {
                        Datagrid2.CurrentPageIndex = Datagrid2.CurrentPageIndex - 1;
                    }
                    this.bindDataGrid();
                }
            }
        }

        protected bool verificaStatoFinale(DocsPaWR.SchedaDocumento schedaDocumento, DocsPaWR.DiagrammaStato dg)
        {
            bool isFinalState = false;
            isFinalState = wws.isDocumentiInStatoFinale((dg.SYSTEM_ID).ToString(), schedaDocumento.tipologiaAtto.systemId);
            return isFinalState;
        }

        protected void apriRicercaFasc()
        {
            try
            {
                bool res = false;
                if (this.txt_codClass.Text.Equals(""))
                {
                    if (!ClientScript.IsStartupScriptRegistered("cod_class"))
                        ClientScript.RegisterStartupScript(this.GetType(), "cod_class", "alert('Inserire un codice di classifica valido');", true);

                    DocsPAWA.Utils.SetFocus(txt_codClass.ClientID, this);

                    //ricarica il frame destro
                    string funct_dx = "top.principale.iFrame_dx.document.location='tabDoc.aspx';";
                    //Response.Write("<script language='javascript'> " + funct_dx + "</script>");
                    ClientScript.RegisterStartupScript(this.GetType(), "pag_class", "" + funct_dx + "", true);
                    return;
                }
                res = cercaClassificazioneDaCodice();
                if (res)
                {
                    FascicoliManager.removeFascicoloSelezionato(this);
                    FascicoliManager.removeFolderSelezionato(this);
                    //23/03/2006
                    //Ora la lentina deve ricercare anche il fascicolo generale
                    //visualizzaFascicoli(tipoVisualizzazioneFascicoli.tvfProcedimentali);

                    visualizzaFascicoli(tipoVisualizzazioneFascicoli.tvfAll);
                    //
                }
                Session["res"] = res.ToString();
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

    }
}
