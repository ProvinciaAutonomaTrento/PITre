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
using DocsPAWA.utils;
using DocsPAWA;
using System.Collections.Generic;
using DocsPAWA.DocsPaWR;
using System.Linq;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.exportDati
{
    public class exportDatiSelection : DocsPAWA.CssPage
    {
        #region WebControls e variabili
        protected System.Web.UI.WebControls.RadioButton rd_formatoPDF;
        protected System.Web.UI.WebControls.Button btn_export;
        protected System.Web.UI.WebControls.Button btn_annulla;
        protected System.Web.UI.WebControls.TextBox txt_titolo;
        protected System.Web.UI.WebControls.RadioButtonList rbl_XlsOrPdf;
        protected System.Web.UI.WebControls.GridView gv_listaCampi;
        protected System.Web.UI.WebControls.CheckBox cb_selezionaTutti;
        protected System.Web.UI.WebControls.Panel panel_listaCampi;
        protected System.Web.UI.WebControls.Label lbl_selezionaCampo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_export;
        protected bool docInFasc;

        protected string appTitle;

        #endregion

        #region Inizializzazione Pagina e variabili
        private void Page_Load(object sender, System.EventArgs e)
        {
            this.Response.Expires = -1;

            if (!IsPostBack)
            {
                this.btn_annulla.Attributes.Add("onclick", "window.close();");
                this.hd_export.Value = Request.QueryString["export"];
                ricercaDoc.SchedaRicerca schedaricerca = (ricercaDoc.SchedaRicerca)Session["SchedaRicerca"];

                if (hd_export.Value.Equals("rubrica"))
                {

                    //nel caso dell'export della rubrica non si selezionano i campi e 
                    //l'export è solo in excel.
                    panel_listaCampi.Visible = false;
                    lbl_selezionaCampo.Visible = false;
                    cb_selezionaTutti.Visible = false;
                    rbl_XlsOrPdf.SelectedIndex = 1;
                    rbl_XlsOrPdf.Enabled = false;
                }
                else
                {
                    lbl_selezionaCampo.Visible = true;
                    cb_selezionaTutti.Visible = true;
                    rbl_XlsOrPdf.SelectedIndex = 0;
                    rbl_XlsOrPdf.Enabled = true;
                    caricaGridViewAndControls();
                }

                string valoreChiave;
                valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_EXPORT_DA_MODELLO");
                if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                {
                    //rbl_XlsOrPdf.Items[2].Enabled = false;
                    DocsPAWA.DocsPaWR.FiltroRicerca[][] filtri = null;
                    if (DocumentManager.getFiltroRicDoc(null) != null)
                        filtri = DocumentManager.getFiltroRicDoc(null);
                    else
                        filtri = DocumentManager.getMemoriaFiltriRicDoc(null);
                    //if (!schedaricerca.getSearchKey().Equals("RicercaDocCompleta")){
                    //    rbl_XlsOrPdf.Items[2].Enabled = false;
                    //}
                    if (filtri != null)
                    {
                        foreach (DocsPaWR.FiltroRicerca[] filtroUno in filtri)
                        {
                            foreach (DocsPaWR.FiltroRicerca filtroDue in filtroUno)
                            {
                                switch (filtroDue.argomento)
                                {
                                    case "TIPO_ATTO":
                                        ListItem modello = new ListItem();
                                        modello.Value = "Model";
                                        modello.Text = "Modello";
                                        if (rbl_XlsOrPdf.Items.Count == 3)
                                            rbl_XlsOrPdf.Items.Add(modello);
                                        //rbl_XlsOrPdf.Items[2].Enabled = true;
                                        break;
                                }
                            }
                        }
                    }
                }


            }


        }
        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        private void btn_export_Click(object sender, System.EventArgs e)
        {
            this.Export();
        }

        private void Export()
        {
            string oggetto = this.hd_export.Value;
            string tipologia = string.Empty;
            string titolo = this.txt_titolo.Text;
            DocsPaWR.InfoUtente userInfo = UserManager.getInfoUtente(this);
            DocsPaWR.Ruolo userRuolo = UserManager.getRuolo(this);
            DocsPaWR.FileDocumento file = new DocsPAWA.DocsPaWR.FileDocumento();
            string docOrFasc = Request.QueryString["docOrFasc"];

            // Lista dei system id degli elementi selezionati
            String[] objSystemId = null;

            // Se nella request c'è il valore fromMassiveOperation...
            if (Request["fromMassiveOperation"] != null)
            {
                List<MassiveOperationTarget> temp = MassiveOperationUtils.GetSelectedItems();
                objSystemId = new String[temp.Count];
                for (int i = 0; i < temp.Count; i++) objSystemId[i] = temp[i].Id;
            }

            // Reperimento dalla sessione del contesto di ricerca fulltext
            DocsPAWA.DocsPaWR.FullTextSearchContext context = Session["FULL_TEXT_CONTEXT"] as DocsPAWA.DocsPaWR.FullTextSearchContext;

            if (rbl_XlsOrPdf.SelectedValue == "PDF")
                tipologia = "PDF";
            else if (rbl_XlsOrPdf.SelectedValue == "XLS")
                tipologia = "XLS";
            else if (rbl_XlsOrPdf.SelectedValue == "Model")
                tipologia = "Model";
            else if (rbl_XlsOrPdf.SelectedValue == "ODS")
                tipologia = "ODS";
            try
            {

                //Se la tipologia scelta è "XLS" recupero i campi scelti dall'utente
                if (tipologia == "XLS" || tipologia == "ODS")
                {
                    ArrayList campiSelezionati = getCampiSelezionati();

                    if ((campiSelezionati.Count != 0) || (hd_export.Value.Equals("rubrica")))
                    {
                        exportDatiManager manager = new exportDatiManager(oggetto, tipologia, titolo, userInfo, userRuolo, context, docOrFasc, campiSelezionati, objSystemId);
                        file = manager.Export();
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "noCampi", "alert('Selezionare almeno un campo per da esportare.');", true);
                    }
                }
                else
                {
                    if (tipologia == "Model")
                    {
                        ArrayList campiSelezionati = getCampiSelezionati();

                        if ((campiSelezionati.Count != 0))
                        {
                            exportDatiManager manager = new exportDatiManager(oggetto, tipologia, titolo, userInfo, userRuolo, context, docOrFasc, campiSelezionati, objSystemId);
                            file = manager.Export();
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "noCampi", "alert('Selezionare almeno un campo per da esportare.');", true);
                        }
                    }
                    else
                    {
                        if (tipologia == "PDF")
                        {
                            exportDatiManager manager = new exportDatiManager(oggetto, tipologia, titolo, userInfo, userRuolo, context, docOrFasc, objSystemId);
                            file = manager.Export();
                        }
                    }
                }


                if (file == null || file.content == null || file.content.Length == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "noRisultatiRicerca", "alert('Nessun documento trovato.');", true);
                    ClientScript.RegisterStartupScript(this.GetType(), "openFile", "OpenFile('" + tipologia + "');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "openFile", "OpenFile('" + tipologia + "');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "errore", "alert('Errore di sistema: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        protected void rbl_XlsOrPdf_SelectedIndexChanged(object sender, EventArgs e)
        {
            caricaGridViewAndControls();
        }

        private void caricaGridViewAndControls()
        {
            //Documenti
            if (rbl_XlsOrPdf.SelectedValue == "PDF" && this.hd_export.Value == "doc")
            { 
                caricaListaCampiDocumento(false, false);
                disabilitaCheckBoxGridView();
                cb_selezionaTutti.Visible = false;
            }

            if ((rbl_XlsOrPdf.SelectedValue == "XLS" || rbl_XlsOrPdf.SelectedValue == "ODS") && this.hd_export.Value == "doc")
            {
                caricaListaCampiDocumento(true, true);
                cb_selezionaTutti.Visible = true;
            }

            if (rbl_XlsOrPdf.SelectedValue == "Model" && this.hd_export.Value == "doc")
            {
                caricaListaCampiDocumento(true, true);
                cb_selezionaTutti.Visible = true;
            }


            //Doc in Cestino
            if (rbl_XlsOrPdf.SelectedValue == "PDF" && this.hd_export.Value == "docInCest")
            {
                rbl_XlsOrPdf.Items[2].Enabled = false;
                caricaListaCampiDocInCestino(false);
                disabilitaCheckBoxGridView();
                cb_selezionaTutti.Visible = false;
            }

            if (rbl_XlsOrPdf.SelectedValue == "XLS" && this.hd_export.Value == "docInCest")
            {
                rbl_XlsOrPdf.Items[2].Enabled = false;
                caricaListaCampiDocInCestino(false);
                cb_selezionaTutti.Visible = true;
            }

            //Doc in Fasc
            if (rbl_XlsOrPdf.SelectedValue == "PDF" && this.hd_export.Value == "docInfasc")
            {
                caricaListaCampiDocumento(false, false);
                disabilitaCheckBoxGridView();
                cb_selezionaTutti.Visible = false;
            }

            if ((rbl_XlsOrPdf.SelectedValue == "XLS"  || rbl_XlsOrPdf.SelectedValue == "ODS") && this.hd_export.Value == "docInfasc")
            {
                this.docInFasc = true;
                caricaListaCampiDocumento(true, true);
                cb_selezionaTutti.Visible = true;
            }

            //Fascicoli
            if (rbl_XlsOrPdf.SelectedValue == "PDF" && this.hd_export.Value == "fasc")
            {
                caricaListaCampiFascicolo(false, false);
                disabilitaCheckBoxGridView();
                cb_selezionaTutti.Visible = false;
            }

            if ((rbl_XlsOrPdf.SelectedValue == "XLS" || rbl_XlsOrPdf.SelectedValue == "ODS") && this.hd_export.Value == "fasc")
            {
                caricaListaCampiFascicolo(true, true);
                cb_selezionaTutti.Visible = true;
            }

            //Trasmissioni
            if (rbl_XlsOrPdf.SelectedValue == "PDF" && (this.hd_export.Value == "trasm" || this.hd_export.Value == "toDoList"))
            {
                rbl_XlsOrPdf.Items[2].Enabled = false;
                caricaListaCampiTrasmissioni();
                disabilitaCheckBoxGridView();
                cb_selezionaTutti.Visible = false;
            }

            if (rbl_XlsOrPdf.SelectedValue == "XLS" && (this.hd_export.Value == "trasm" || this.hd_export.Value == "toDoList"))
            {
                rbl_XlsOrPdf.Items[2].Enabled = false;
                caricaListaCampiTrasmissioni();
                cb_selezionaTutti.Visible = true;
            }

            //Abilitazione dell'esportazione personalizzata
            if (System.Configuration.ConfigurationManager.AppSettings["GV_EXPORT_ENABLE"] != null && System.Configuration.ConfigurationManager.AppSettings["GV_EXPORT_ENABLE"] == "1")
            {
                panel_listaCampi.Visible = true;
                lbl_selezionaCampo.Visible = true;
                ClientScript.RegisterStartupScript(this.GetType(), "resizeWindow", "window.dialogWidth = '450px'; window.dialogHeight = '420px';", true);
            }
            else
            {
                panel_listaCampi.Visible = false;
                cb_selezionaTutti.Visible = false;
                lbl_selezionaCampo.Visible = false;
                ClientScript.RegisterStartupScript(this.GetType(), "resizeWindow", "window.dialogWidth = '450px'; window.dialogHeight = '250px';", true);
            }

            //Documenti
            if (rbl_XlsOrPdf.SelectedValue == "PDF" && this.hd_export.Value == "scarto")
            {
                rbl_XlsOrPdf.Items[2].Enabled = false;
                caricaListaCampiScarto();
                disabilitaCheckBoxGridView();
                cb_selezionaTutti.Visible = false;
            }

            if (rbl_XlsOrPdf.SelectedValue == "XLS" && this.hd_export.Value == "scarto")
            {
                rbl_XlsOrPdf.Items[2].Enabled = false;
                caricaListaCampiScarto();
                cb_selezionaTutti.Visible = true;
            }
        }

        /// <summary>
        /// Carica i campi da stampare
        /// </summary>
        /// <param name="profilazione"></param>
        /// <param name="esportazioneExcel">True se è richiesta una esportazione excel</param>
        private void caricaListaCampiDocumento(bool profilazione, bool esportazioneExcel)
        {
            gv_listaCampi.Columns[0].Visible = true;
            gv_listaCampi.Columns[1].Visible = true;
            gv_listaCampi.Columns[4].Visible = true; 
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");
            // Aggiunta colonna per il system id del campo
            dt.Columns.Add("ID");
            dt.Columns.Add("VISIBILE");

            
            // Se è una esportazione Excel, vengono caricati i campi della griglia
            if (esportazioneExcel && GridManager.IsRoleEnabledToUseGrids())
                this.ShowGridFiels(dt);
            else
                this.CaricaListaCampiDocumentoPreesistente(dt, profilazione, esportazioneExcel);
            
            
            dt.AcceptChanges();
            gv_listaCampi.DataSource = dt;
            gv_listaCampi.DataBind();
            gv_listaCampi.Visible = true;
            gv_listaCampi.Columns[0].Visible = false;
            gv_listaCampi.Columns[1].Visible = false;

            //Elimino la selezione di default di tutti i campi che non sono quelli standar
            if(!esportazioneExcel && !GridManager.IsRoleEnabledToUseGrids())
                for (int i = 9; i < gv_listaCampi.Rows.Count; i++)
                    ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = false;
            
            // Se è stata richiesta l'esportazione in formato excel e non sono attive le griglie...
            if (esportazioneExcel && !GridManager.IsRoleEnabledToUseGrids())
                for (int i = 0; i < gv_listaCampi.Rows.Count; i++)
                ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = true;

            //Griglie custom
            if (esportazioneExcel && GridManager.IsRoleEnabledToUseGrids())
            {
                for (int i = 0; i < gv_listaCampi.Rows.Count; i++)
                {
                    string visibile = gv_listaCampi.Rows[i].Cells[4].Text;
                    if (!string.IsNullOrEmpty(visibile) && visibile.ToUpper().Equals("TRUE"))
                    {
                        ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = true;
                    }
                    else
                    {
                        ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = false;
                    }
                    
                }
            }

            gv_listaCampi.Columns[4].Visible = false;
        }

        /// <summary>
        /// Funzione utilizzata per mostrare i campi della griglia
        /// </summary>
        /// <param name="dataTable">Data table in cui inserire i nomi dei campi</param>
        private void ShowGridFiels(DataTable dataTable)
        {
            bool allChecked = true;
            // Caricamento della griglia
            if (GridManager.SelectedGrid == null)
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);

            foreach (Field field in GridManager.SelectedGrid.Fields.Where(e => !e.Locked).OrderBy(e => e.Position).ToList())
            {
                DataRow row = dataTable.NewRow();
                row["CAMPO_STANDARD"] = field.CustomObjectId == 0 ? "0" : field.CustomObjectId.ToString();
                row["CAMPO_COMUNE"] = "0";
                row["CAMPI"] = field.Label;
                row["ID"] = field.FieldId;
                row["VISIBILE"] = (field.Visible).ToString();
                dataTable.Rows.Add(row);
                if (!field.Visible)
                {
                    allChecked = false;
                }
            }

            if (allChecked)
            {
                this.cb_selezionaTutti.Checked = true;
            }
            else
            {
                this.cb_selezionaTutti.Checked = false;
            }

        }

        private void CaricaListaCampiDocumentoPreesistente(DataTable dt, bool profilazione, bool esportazioneExcel)
        {
            //Aggiungo i campi standar del documento
            DataRow rw1 = dt.NewRow();
            rw1[0] = "1";
            rw1[1] = "0";
            rw1[2] = "Registro";
            rw1[3] = "D2";
            dt.Rows.Add(rw1);
            DataRow rw2 = dt.NewRow();
            rw2[0] = "1";
            rw2[1] = "0";
            rw2[2] = "Prot. / Id. Doc.";
            rw2[3] = "D1";
            dt.Rows.Add(rw2);
            DataRow rw3 = dt.NewRow();
            rw3[0] = "1";
            rw3[1] = "0";
            rw3[2] = "Data";
            rw3[3] = "D9";
            dt.Rows.Add(rw3);
            DataRow rw4 = dt.NewRow();
            rw4[0] = "1";
            rw4[1] = "0";
            rw4[2] = "Oggetto";
            rw4[3] = "D4";
            dt.Rows.Add(rw4);
            DataRow rw5 = dt.NewRow();
            rw5[0] = "1";
            rw5[1] = "0";
            rw5[2] = "Tipo";
            rw5[3] = "D3";
            dt.Rows.Add(rw5);
            DataRow rw6 = dt.NewRow();
            rw6[0] = "1";
            rw6[1] = "0";
            rw6[2] = "Mitt. / Dest.";
            rw6[3] = "D5";
            dt.Rows.Add(rw6);
            DataRow rw7 = dt.NewRow();
            rw7[0] = "1";
            rw7[1] = "0";
            rw7[2] = "Cod. Fascicoli";
            rw7[3] = "D18";
            dt.Rows.Add(rw7);
            DataRow rw8 = dt.NewRow();
            rw8[0] = "1";
            rw8[1] = "0";
            rw8[2] = "Annullato";
            rw8[3] = "D11";
            dt.Rows.Add(rw8);
            DataRow rw9 = dt.NewRow();
            rw9[0] = "1";
            rw9[1] = "0";
            rw9[2] = "File";
            rw9[3] = "D23";
            dt.Rows.Add(rw9);

            // Se è richiesta una esportazione excel...
            if (esportazioneExcel)
            {
                DataRow rw10 = dt.NewRow();
                rw10[0] = "1";
                rw10[1] = "0";
                rw10[2] = "Note";
                rw10[3] = "D17";
                dt.Rows.Add(rw10);
            }

            //Verifico che si vogliono o meno visualizzare anche i campi del profilo di documento
            if (profilazione)
            {
                //Verifico che è stata selezionata una tipologia ed in caso affermativo
                //rendo possibile la selezione anche dei campi profilati dinamicamente
                DocsPAWA.DocsPaWR.FiltroRicerca[][] filtri = null;
                if (DocumentManager.getFiltroRicDoc(null) != null)
                    filtri = DocumentManager.getFiltroRicDoc(null);
                else
                    filtri = DocumentManager.getMemoriaFiltriRicDoc(null);

                if (filtri == null)
                    filtri = (DocsPAWA.DocsPaWR.FiltroRicerca[][])HttpContext.Current.Session["ricFasc.listaFiltri"];

                if (this.docInFasc)
                {
                    if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["SearchFilters"] != null)
                    {
                        filtri = CallContextStack.CurrentContext.ContextState["SearchFilters"] as FiltroRicerca[][];
                    }
                }

                if (filtri != null)
                {
                    foreach (DocsPaWR.FiltroRicerca[] filtroUno in filtri)
                    {
                        foreach (DocsPaWR.FiltroRicerca filtroDue in filtroUno)
                        {
                            switch (filtroDue.argomento)
                            {
                                case "TIPO_ATTO":
                                    DocsPaWR.Templates template = ProfilazioneDocManager.getTemplateById(filtroDue.valore, this);
                                    foreach (DocsPaWR.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                                    {
                                        DataRow rw = dt.NewRow();
                                        rw[0] = oggettoCustom.SYSTEM_ID;
                                        rw[1] = oggettoCustom.CAMPO_COMUNE;
                                        rw[2] = oggettoCustom.DESCRIZIONE;
                                        rw[3] = "A"+oggettoCustom.SYSTEM_ID;
                                        dt.Rows.Add(rw);
                                    }
                                    break;
                                    
                                case "CONTATORE_GRIGLIE_NO_CUSTOM":
                                        DataRow rf = dt.NewRow();
                                        rf[0] = "1";
                                        rf[1] = "0";
                                        rf[2] = "Contatore";
                                        rf[3] = "CONTATORE";
                                        dt.Rows.Add(rf);
                                        break;
                            }
                        }
                    }
                }
            }
        }

        private void caricaListaCampiDocInCestino(bool profilazione)
        {
            gv_listaCampi.Columns[0].Visible = true;
            gv_listaCampi.Columns[1].Visible = true;
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");
            // Aggiunta colonna per il system id del campo
            dt.Columns.Add("ID");

            //Aggiungo i campi per i documenti in cestino
            DataRow rw1 = dt.NewRow();
            rw1[0] = "1";
            rw1[1] = "0";
            rw1[2] = "Registro";
            dt.Rows.Add(rw1);
            DataRow rw2 = dt.NewRow();
            rw2[0] = "1";
            rw2[1] = "0";
            rw2[2] = "Prot. / Id. Doc.";
            dt.Rows.Add(rw2);
            DataRow rw3 = dt.NewRow();
            rw3[0] = "1";
            rw3[1] = "0";
            rw3[2] = "Data";
            dt.Rows.Add(rw3);
            DataRow rw4 = dt.NewRow();
            rw4[0] = "1";
            rw4[1] = "0";
            rw4[2] = "Oggetto";
            dt.Rows.Add(rw4);
            DataRow rw5 = dt.NewRow();
            rw5[0] = "1";
            rw5[1] = "0";
            rw5[2] = "Tipo";
            dt.Rows.Add(rw5);
            DataRow rw6 = dt.NewRow();
            rw6[0] = "1";
            rw6[1] = "0";
            rw6[2] = "Mitt. / Dest.";
            dt.Rows.Add(rw6);
            DataRow rw7 = dt.NewRow();
            rw7[0] = "1";
            rw7[1] = "0";
            rw7[2] = "Motivo Rimozione";
            dt.Rows.Add(rw7);

            dt.AcceptChanges();
            gv_listaCampi.DataSource = dt;
            gv_listaCampi.DataBind();
            gv_listaCampi.Visible = true;
            gv_listaCampi.Columns[0].Visible = false;
            gv_listaCampi.Columns[1].Visible = false;
        }

        private void caricaListaCampiFascicolo(bool profilazione, bool esportazioneExcel)
        {
            gv_listaCampi.Columns[0].Visible = true;
            gv_listaCampi.Columns[1].Visible = true;
            gv_listaCampi.Columns[4].Visible = true; 
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");
            // Aggiunta colonna per il system id del campo
            dt.Columns.Add("ID");
            dt.Columns.Add("VISIBILE");

            // Se è una esportazione Excel, vengono visualizzati i campi della griglia
            if (esportazioneExcel && GridManager.IsRoleEnabledToUseGrids())
                this.ShowGridFiels(dt);
            else
                this.caricaListaCampiFascicoloPreesistente(dt, profilazione, esportazioneExcel);

            dt.AcceptChanges();
            gv_listaCampi.DataSource = dt;
            gv_listaCampi.DataBind();
            gv_listaCampi.Visible = true;
            gv_listaCampi.Columns[0].Visible = false;
            gv_listaCampi.Columns[1].Visible = false;

            //Elimino la selezione di default di tutti i campi che non sono quelli standar
            if (!esportazioneExcel && !GridManager.IsRoleEnabledToUseGrids())
                for (int i = 8; i < gv_listaCampi.Rows.Count; i++)
                    ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = false;

            // Se è stata richiesta l'esportazione in formato excel e non sono attive le griglie...
            if (esportazioneExcel && !GridManager.IsRoleEnabledToUseGrids())
                for (int i = 0; i < gv_listaCampi.Rows.Count; i++)
                    ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = true;

            //Griglie custom
            if (esportazioneExcel && GridManager.IsRoleEnabledToUseGrids())
            {
                for (int i = 0; i < gv_listaCampi.Rows.Count; i++)
                {
                    string visibile = gv_listaCampi.Rows[i].Cells[4].Text;
                    if (!string.IsNullOrEmpty(visibile) && visibile.ToUpper().Equals("TRUE"))
                    {
                        ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = true;
                    }
                    else
                    {
                        ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = false;
                    }

                }
            }

            gv_listaCampi.Columns[4].Visible = false;
        }

        private void caricaListaCampiFascicoloPreesistente(DataTable dt, bool profilazione, bool esportazioneExcel)
        {
            //Aggiungo i campi standar del fascicolo
            DataRow rw1 = dt.NewRow();
            rw1[0] = "1";
            rw1[1] = "0";
            rw1[2] = "Registro";
            rw1[3] = "P7";
            dt.Rows.Add(rw1);

            DataRow rw2 = dt.NewRow();
            rw2[0] = "1";
            rw2[1] = "0";
            rw2[2] = "Tipo";
            rw2[3] = "P1";
            dt.Rows.Add(rw2);

            DataRow rw3 = dt.NewRow();
            rw3[0] = "1";
            rw3[1] = "0";
            rw3[2] = "Codice";
            rw3[3] = "P3";
            dt.Rows.Add(rw3);

            DataRow rw4 = dt.NewRow();
            rw4[0] = "1";
            rw4[1] = "0";
            rw4[2] = "Descrizione";
            rw4[3] = "P4";
            dt.Rows.Add(rw4);

            DataRow rw5 = dt.NewRow();
            rw5[0] = "1";
            rw5[1] = "0";
            rw5[2] = "Data Apertura";
            rw5[3] = "P5";
            dt.Rows.Add(rw5);

            DataRow rw6 = dt.NewRow();
            rw6[0] = "1";
            rw6[1] = "0";
            rw6[2] = "Data Chiusura";
            rw6[3] = "P6";
            dt.Rows.Add(rw6);

            DataRow rw7 = dt.NewRow();
            rw7[0] = "1";
            rw7[1] = "0";
            rw7[2] = "Collocazione Fisica";
            rw7[3] = "P22";
            dt.Rows.Add(rw7);

            DataRow rw8 = dt.NewRow();
            rw8[0] = "1";
            rw8[1] = "0";
            rw8[2] = "Tipologia";
            rw8[3] = "U1";
            dt.Rows.Add(rw8);

            // Se è richiesta l'esportazione in excel...
            if (esportazioneExcel)
            {
                // ...aggiungo il campo note
                DataRow rw9 = dt.NewRow();
                rw9[0] = "1";
                rw9[1] = "0";
                rw9[2] = "Note";
                rw9[3] = "P8";
                dt.Rows.Add(rw9);
            }

            //Verifico che si vogliono o meno visualizzare anche i campi del profilo di un fascicolo
            if (profilazione)
            {
                //Verifico che è stata selezionata una tipologia ed in caso affermativo
                //rendo possibile la selezione anche dei campi profilati dinamicamente
                DocsPAWA.DocsPaWR.FiltroRicerca[][] filtri = null;
                if (FascicoliManager.getFiltroRicFasc(null) != null)
                    filtri = FascicoliManager.getFiltroRicFasc(null);
                else
                    filtri = FascicoliManager.getMemoriaFiltriRicFasc(null);

                if (filtri != null)
                {
                    foreach (DocsPaWR.FiltroRicerca[] filtroUno in filtri)
                    {
                        foreach (DocsPaWR.FiltroRicerca filtroDue in filtroUno)
                        {
                            switch (filtroDue.argomento)
                            {
                                case "TIPOLOGIA_FASCICOLO":
                                    DocsPaWR.Templates template = ProfilazioneDocManager.getTemplateById(filtroDue.valore, this);
                                    foreach (DocsPaWR.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                                    {
                                        DataRow rw = dt.NewRow();
                                        rw[0] = oggettoCustom.SYSTEM_ID;
                                        rw[1] = oggettoCustom.CAMPO_COMUNE;
                                        rw[2] = oggettoCustom.DESCRIZIONE;
                                        rw[3] = "A" + oggettoCustom.SYSTEM_ID;
                                        dt.Rows.Add(rw);
                                    }
                                    break;

                                case "CONTATORE_GRIGLIE_NO_CUSTOM":
                                    DataRow rf = dt.NewRow();
                                    rf[0] = "1";
                                    rf[1] = "0";
                                    rf[2] = "Contatore";
                                    rf[3] = "CONTATORE";
                                    dt.Rows.Add(rf);
                                    break;
                            }
                        }
                    }
                }
            }

        }

        private void caricaListaCampiTrasmissioni()
        {
            gv_listaCampi.Columns[0].Visible = true;
            gv_listaCampi.Columns[1].Visible = true;
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");
            // Aggiunta colonna per il system id del campo
            dt.Columns.Add("ID");

            //Aggiungo i campi standar del fascicolo
            DataRow rw1 = dt.NewRow();
            rw1[0] = "1";
            rw1[1] = "0";
            rw1[2] = "Data Trasm.";
            dt.Rows.Add(rw1);

            DataRow rw2 = dt.NewRow();
            rw2[0] = "1";
            rw2[1] = "0";
            rw2[2] = "Documento Trasmesso";
            dt.Rows.Add(rw2);

            DataRow rw3 = dt.NewRow();
            rw3[0] = "1";
            rw3[1] = "0";
            rw3[2] = "Mittenti";
            dt.Rows.Add(rw3);

            DataRow rw4 = dt.NewRow();
            rw4[0] = "1";
            rw4[1] = "0";
            rw4[2] = "Destinatari";
            dt.Rows.Add(rw4);

            dt.AcceptChanges();
            gv_listaCampi.DataSource = dt;
            gv_listaCampi.DataBind();
            gv_listaCampi.Visible = true;
            gv_listaCampi.Columns[0].Visible = false;
            gv_listaCampi.Columns[1].Visible = false;
        }

        private void caricaListaCampiScarto()
        {
            gv_listaCampi.Columns[0].Visible = true;
            gv_listaCampi.Columns[1].Visible = true;
            DataTable dt = new DataTable();
            dt.Columns.Add("CAMPO_STANDARD");
            dt.Columns.Add("CAMPO_COMUNE");
            dt.Columns.Add("CAMPI");
            // Aggiunta colonna per il system id del campo
            dt.Columns.Add("ID");

            //Aggiungo i campi standar del documento
            DataRow rw1 = dt.NewRow();
            rw1[0] = "1";
            rw1[1] = "0";
            rw1[2] = "Tipo";
            dt.Rows.Add(rw1);
            DataRow rw2 = dt.NewRow();
            rw2[0] = "1";
            rw2[1] = "0";
            rw2[2] = "Codice Classificazione";
            dt.Rows.Add(rw2);
            DataRow rw3 = dt.NewRow();
            rw3[0] = "1";
            rw3[1] = "0";
            rw3[2] = "Codice";
            dt.Rows.Add(rw3);
            DataRow rw4 = dt.NewRow();
            rw4[0] = "1";
            rw4[1] = "0";
            rw4[2] = "Descrizione";
            dt.Rows.Add(rw4);
            DataRow rw5 = dt.NewRow();
            rw5[0] = "1";
            rw5[1] = "0";
            rw5[2] = "Data Chiusura";
            dt.Rows.Add(rw5);
            DataRow rw6 = dt.NewRow();
            rw6[0] = "1";
            rw6[1] = "0";
            rw6[2] = "Mesi conservazione";
            dt.Rows.Add(rw6);
            DataRow rw7 = dt.NewRow();
            rw7[0] = "1";
            rw7[1] = "0";
            rw7[2] = "Mesi di chiusura";
            dt.Rows.Add(rw7);
            dt.AcceptChanges();
            gv_listaCampi.DataSource = dt;
            gv_listaCampi.DataBind();
            gv_listaCampi.Visible = true;
            gv_listaCampi.Columns[0].Visible = false;
            gv_listaCampi.Columns[1].Visible = false;

            DocsPAWA.DocsPaWR.InfoScarto infoScarto = new DocsPAWA.DocsPaWR.InfoScarto();
            infoScarto = FascicoliManager.getIstanzaScarto(this);
            if (infoScarto != null)
            {
                txt_titolo.Text = infoScarto.descrizione + "---" + infoScarto.note;
                txt_titolo.Enabled = false;
            }
        }

        private void disabilitaCheckBoxGridView()
        {
            for (int i = 0; i < gv_listaCampi.Rows.Count; i++)
            {
                ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = true;
                ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Enabled = false;
            }
        }

        private ArrayList getCampiSelezionati()
        {
            ArrayList campiSelezionati = new ArrayList();
            for (int i = 0; i < gv_listaCampi.Rows.Count; i++)
            {
                if (((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked)
                {
                    DocsPAWA.DocsPaWR.CampoSelezionato campoSelezionato = new DocsPAWA.DocsPaWR.CampoSelezionato();
                    campoSelezionato.campoStandard = gv_listaCampi.Rows[i].Cells[0].Text;
                    campoSelezionato.campoComune = gv_listaCampi.Rows[i].Cells[1].Text;
                    campoSelezionato.nomeCampo = HttpUtility.HtmlDecode(gv_listaCampi.Rows[i].Cells[2].Text);
                    campoSelezionato.fieldID = (((HiddenField)gv_listaCampi.Rows[i].Cells[3].FindControl("hfLongDescriuption")).Value).ToString();

                    int id = 0;
                    Int32.TryParse(((HiddenField)gv_listaCampi.Rows[i].Cells[3].FindControl("hfLongDescriuption")).Value, out id);
                    campoSelezionato.SystemId = id;

                    campiSelezionati.Add(campoSelezionato);
                }
            }
            return campiSelezionati;
        }

        protected void cb_selezionaTutti_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_selezionaTutti.Checked)
            {
                for (int i = 0; i < gv_listaCampi.Rows.Count; i++)
                    ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = true;
            }
            else
            {
                for (int i = 0; i < gv_listaCampi.Rows.Count; i++)
                    ((System.Web.UI.WebControls.CheckBox)gv_listaCampi.Rows[i].Cells[3].Controls[1]).Checked = false;
            }
        }

        protected String GetLongDescription(DataRowView row)
        {
            String retVal = String.Empty;

            try
            {
                retVal = row["ID"].ToString();
            }
            catch
            {
                // Non è necessarion fare nulla
            }

            return retVal;

        }
    }
}
