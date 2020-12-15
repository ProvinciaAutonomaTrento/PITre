using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace DocsPAWA.Note
{
    public partial class ElencoNote : DocsPAWA.CssPage
    {
        protected static DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();
        protected static int numNote;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lbl_messaggio.Text = "Elenco note - Trovati 0 elementi";

                txt_desc.Text = "";
                CaricaRfVisibili();
                visib_pulsanti(true);
                SetFocus(btn_filtro);
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
            this.btn_filtro.Click += new EventHandler(btn_filtro_Click);
            this.btn_nuova.Click += new EventHandler(btn_nuova_Click);
            this.btn_rimuovi.Click += new EventHandler(btn_rimuovi_Click);
            this.btn_chiudi.Click += new EventHandler(btn_chiudi_Click);
            this.btn_modifica.Click += new EventHandler(btn_modifica_Click);
            this.btn_chiudiPnl.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiPnl_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.btn_elimina_excel.Click += new System.Web.UI.ImageClickEventHandler(this.btn_elimina_excel_Click);
            this.UploadBtn.Click += new EventHandler(UploadBtn_Click);
        }
        #endregion

        #region pulsanti/operazioni
        private void btn_filtro_Click(object sender, EventArgs e)
        {
            DocsPAWA.DocsPaWR.InfoUtente infoUt = UserManager.getInfoUtente(this);
            if (ddlFiltroRf.Enabled)
                this.ListaNote = docsPaWS.GetListaNote(infoUt, ddlFiltroRf.SelectedItem.Value, txt_desc.Text, out numNote);
            BindGridAndSelect();
            visib_pulsanti(true);

        }

        private void btn_nuova_Click(object sender, EventArgs e)
        {
            pnl_risultatiExcel.Visible = false;

            //Caso in cui è stata creata una nuova nota e si deve salvare
            if (pnl_nuovaNota.Visible)
            {
                btn_nuova.Text = "Nuova";
                btn_nuova.ToolTip = "Crea nuova nota";
                if (chk_Excel.Checked)
                {
                    //Inserimento da foglio excel
                    InsNotaInElencoDaExcel();
                }
                else
                {
                    //Inserimento da txtbox
                    InsNotaInElenco();
                }
            }
            else
            {
                //Caso in cui si deve creare una nuova nota
                btn_nuova.Text = "Inserisci";
                btn_nuova.ToolTip = "Inserisci nuova nota";
                visib_pulsanti(false);
                btn_nuova.Enabled = true;
                CaricaComboRegistri();
                ddl_regRF.SelectedIndex = 0;
                ddl_regRF.Enabled = true;
                txt_descNota.Enabled = true;
                txt_descNota.Text = string.Empty;

                //Verifica che l'utente sia abilitato a importare la note da un dato file excel
                if (UserManager.ruoloIsAutorized(this, "IMPORT_ELENCO_NOTE"))
                {
                    pnl_excel.Visible = true;
                    uploadedFile.Enabled = false;
                    UploadBtn.Enabled = false;
                    chk_Excel.Checked = false;
                    lbl_fileExcel.Text = " Nessun file excel caricato.";
                    btn_elimina_excel.Visible = false;
                }
                else
                    pnl_excel.Visible = false;
            }
        }

        private void btn_chiudi_Click(object sender, EventArgs e)
        {
            RegisterStartupScript("ChiudiNota", "<script>window.close()</script>");
        }

        private void btn_modifica_Click(object sender, EventArgs e)
        {
            pnl_excel.Visible = false;
            pnl_risultatiExcel.Visible = false;
            //Caso in cui è stata modificata una nota e si deve salvare
            if (pnl_nuovaNota.Visible)
                ModNotaInElenco();
            else
            {
                //Caso in cui si sta per modificare una nota
                CaricaComboRegistri();
                this.SetFocus(this.btn_modifica);
                //recupero le informazioni sulla nota che si vuole modificare
                int posizione = verificaSelezioneNota();
                if (posizione >= 0)
                {
                    //imposto interfaccia grafica
                    visib_pulsanti(false);
                    btn_modifica.Enabled = true;
                    lbl_operazione.Text = "Modifica nota";
                    this.pnl_nuovaNota.Visible = true;
                    DocsPAWA.DocsPaWR.NotaElenco nota = (DocsPAWA.DocsPaWR.NotaElenco)this.ListaNote[posizione];
                    this.txt_descNota.Text = nota.descNota;
                    int i = 0;
                    foreach (ListItem item in ddl_regRF.Items)
                    {
                        if (item.Value == nota.idRegRf)
                            break;
                        i++;
                    }
                    //imposto la dropdownlist sul valore del rf della nota 
                    if (i < ddl_regRF.Items.Count)
                        ddl_regRF.SelectedIndex = i;
                }
                else
                {
                    Response.Write("<script>alert('Nessuna nota selezionata!');</script>");
                    return;
                }
            }
        }

        private void btn_rimuovi_Click(object sender, EventArgs e)
        {
            ArrayList lista = new ArrayList();
            //Recupero la lista delle note da rimuovere
            for (int i = 0; i < dgNote.Items.Count; i++)
            {
                CheckBox chkSelection = dgNote.Items[i].Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                {
                    DocsPAWA.DocsPaWR.NotaElenco nota = (DocsPAWA.DocsPaWR.NotaElenco)this.ListaNote[i];
                    lista.Add(nota);
                }
            }

            if (lista.Count > 0)
            {
                DocsPaWR.InfoUtente infoUt = UserManager.getInfoUtente(this);
                if (docsPaWS.DeleteNoteInElenco(infoUt, (DocsPAWA.DocsPaWR.NotaElenco[])lista.ToArray(typeof(DocsPAWA.DocsPaWR.NotaElenco))))
                {
                    this.ListaNote = docsPaWS.GetListaNote(infoUt, ddlFiltroRf.SelectedItem.Value, txt_desc.Text, out numNote);
                    BindGridAndSelect();
                    visib_pulsanti(true);

                }
                else
                {
                    Response.Write("<script>alert('Attenzione, impossibile rimuovere la nota. Riprovare più tardi.');</script>");
                    return;
                }
            }
            else
            {
                Response.Write("<script>alert('Nessuna nota selezionata!');</script>");
                return;
            }
        }

        private void InsNotaInElenco()
        {
            if (ddl_regRF.SelectedIndex == 0)
            {
                Response.Write("<script>alert('Selezionare un rf');</script>");
                return;
            }
            else if (string.IsNullOrEmpty(txt_descNota.Text))
            {
                Response.Write("<script>alert('Selezionare una nota');</script>");
                return;
            }
            else
            {
                DocsPAWA.DocsPaWR.NotaElenco nota = new DocsPAWA.DocsPaWR.NotaElenco();
                nota.codRegRf = ddl_regRF.SelectedItem.Text.Substring(0, ddl_regRF.SelectedItem.Text.IndexOf("-") - 1);
                nota.idRegRf = ddl_regRF.SelectedItem.Value;
                nota.descNota = txt_descNota.Text;
                DocsPaWR.InfoUtente infoUt = UserManager.getInfoUtente(this);
                string message = "";
                if (docsPaWS.InsertNotaInElenco(infoUt, nota, out message))
                {
                    if (!string.IsNullOrEmpty(message))
                        Response.Write("<script>alert('" + message + "');</script>");
                    pnl_nuovaNota.Visible = false;
                    this.ListaNote = docsPaWS.GetListaNote(infoUt, ddlFiltroRf.SelectedItem.Value, txt_desc.Text, out numNote);
                    BindGridAndSelect();
                    visib_pulsanti(true);
                }
                else
                {
                    if (!string.IsNullOrEmpty(message))
                        Response.Write("<script>alert('" + message + "');</script>");
                    else
                        Response.Write("<script>alert('Attenzione, impossibile inserire la nuova nota. Riprovare più tardi.');</script>");
                    btn_nuova.Text = "Inserisci";
                    btn_nuova.ToolTip = "Inserisci nuova nota";
                    return;
                }
            }
        }

        private void InsNotaInElencoDaExcel()
        {
            byte[] dati = null;
            if (ViewState["DatiExcel"] != null)
            {
                dati = (byte[])ViewState["DatiExcel"];
                ViewState.Remove("DatiExcel");
            }
            if (dati == null)
            {
                Response.Write("<script>alert('Selezionare un file excel');</script>");
                return;
            }
            else
            {
                DocsPaWR.InfoUtente infoUt = UserManager.getInfoUtente(this);
                string message = "";
                DocsPaWR.ImportResult[] report = null;
                try
                {
                    report = (docsPaWS.InsertNotaInElencoDaExcel(infoUt, dati, ViewState["fileExcel"].ToString()));
                    ViewState.Remove("DatiExcel");
                    btn_nuova.Enabled = false;
                    btn_nuova.Text = "Inserisci";
                    btn_nuova.ToolTip = "Inserisci nuova nota";
                }
                catch (Exception ex)
                {
                    // Creazione di un array du result con un solo elemento
                    // che conterrà il dettaglio dell'eccezione
                    report = new DocsPAWA.DocsPaWR.ImportResult[1];
                    report[0] = new DocsPAWA.DocsPaWR.ImportResult()
                    {
                        Outcome = DocsPAWA.DocsPaWR.OutcomeEnumeration.KO,
                        Message = ex.Message,
                    };
                }

                // Associazione dell'array dei risultati alla griglia
                this.grdRisExcel.DataSource = report;

                // Binding della sorgente dati
                this.grdRisExcel.DataBind();
                this.pnl_risultatiExcel.Visible = true;
            }
        }

        /// <summary>
        /// Funzione per la restituzione dell'esito dell'operazione
        /// </summary>
        /// <param name="result">L'oggetto result associato alla riga corrente</param>
        /// <returns>L'esito</returns>
        protected string GetResult(DocsPAWA.DocsPaWR.ImportResult result)
        {
            string toReturn;

            // A seconda dell'esito bisogna visualizzarlo in rosso, in verde o in giallo
            switch (result.Outcome)
            {
                case DocsPAWA.DocsPaWR.OutcomeEnumeration.KO:
                    toReturn = String.Format("<span style=\"color:Red;\">{0}</span>",
                        result.Outcome);
                    break;

                case DocsPAWA.DocsPaWR.OutcomeEnumeration.OK:
                    toReturn = String.Format("<span style=\"color:Green;\">{0}</span>",
                        result.Outcome);
                    break;

                case DocsPAWA.DocsPaWR.OutcomeEnumeration.Warnings:
                    toReturn = String.Format("<span style=\"color:Yellow;\">{0}</span>",
                        result.Outcome);
                    break;

                default:
                    toReturn = String.Format("<span style=\"color:Green;\">{0}</span>",
                        result.Outcome);
                    break;
            }

            // Restituzione del testo
            return toReturn;
        }

        /// Funzione per la restituzione dei dettagli sull'esito
        /// </summary>
        /// <param name="result">L'oggetto result associato alla riga corrente</param>
        /// <returns>Gli eventuali dettagli sull'esito</returns>
        protected string GetDetails(DocsPAWA.DocsPaWR.ImportResult result)
        {
            string toReturn;

            System.Text.StringBuilder message = new System.Text.StringBuilder();

            // Se ci sono dettagli da mostrare
            if (result.OtherInformation != null)
            {
                // ...aggiunta del tag di inizio numerazione
                message.AppendLine("<ul>");

                // ...per ogni dettaglio...
                foreach (string str in result.OtherInformation)
                    // ...aggiunta dell'item 
                    message.AppendFormat("<li>{0}</li>",
                        str);

                // ...aggiunta del tag di chiusura della lista
                message.AppendLine("</ul>");

            }

            // Restituzione dei dettagli
            toReturn = message.ToString();

            // Restituzione del testo
            return toReturn;

        }

        private void ModNotaInElenco()
        {
            int posizione = verificaSelezioneNota();
            if (posizione >= 0 && ddl_regRF.SelectedIndex > 0)
            {
                DocsPAWA.DocsPaWR.NotaElenco nota = (DocsPAWA.DocsPaWR.NotaElenco)this.ListaNote[posizione];

                nota.codRegRf = ddl_regRF.SelectedItem.Text.Substring(0, ddl_regRF.SelectedItem.Text.IndexOf("-") - 1);
                nota.idRegRf = ddl_regRF.SelectedItem.Value;
                nota.descNota = txt_descNota.Text;
                DocsPaWR.InfoUtente infoUt = UserManager.getInfoUtente(this);
                string message = "";
               
                if (docsPaWS.ModNotaInElenco(infoUt, nota, out message))
                {
                    if (!string.IsNullOrEmpty(message))
                        Response.Write("<script>alert('" + message + "');</script>");
                    pnl_nuovaNota.Visible = false;
                    this.ListaNote = docsPaWS.GetListaNote(infoUt, ddlFiltroRf.SelectedItem.Value, txt_desc.Text, out numNote);
                    BindGridAndSelect();
                    visib_pulsanti(true);
                }
                else
                {
                    if (!string.IsNullOrEmpty(message))
                        Response.Write("<script>alert('" + message + "');</script>");
                    else
                        Response.Write("<script>alert('Attenzione, impossibile modificare la nota. Riprovare più tardi.');</script>");
                    return;
                }
            }
            else
            {
                Response.Write("<script>alert('Attenzione, selezionare un rf.');</script>");
                return;
            }
        }

        private void btn_chiudiPnl_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            SetFocus(btn_nuova);
            disabilitaCheckDataGrid();
            btn_nuova.Text = "Nuova";
            btn_nuova.ToolTip = "Crea nuova nota";
            if (this.pnl_risultatiExcel.Visible)
            {
                pnl_risultatiExcel.Visible = false;
                DocsPAWA.DocsPaWR.InfoUtente infoUt = UserManager.getInfoUtente(this);
                this.ListaNote = docsPaWS.GetListaNote(infoUt, ddlFiltroRf.SelectedItem.Value, txt_desc.Text, out numNote);
                BindGridAndSelect();
            }
            visib_pulsanti(true);
            pnl_nuovaNota.Visible = false;
        }

        #endregion

        private void visib_pulsanti(bool vis)
        {

            if (vis)
            {
                pnl_nuovaNota.Visible = false;
                btn_nuova.Enabled = true;
                if (this.ListaNote.Length == 0)
                {
                    btn_modifica.Enabled = false;
                    btn_rimuovi.Enabled = false;
                    lbl_messaggio.Text = "Elenco note - Trovati 0 elementi";

                }
                if (this.ListaNote.Length == 1)
                {
                    btn_modifica.Enabled = true;
                    btn_rimuovi.Enabled = true;
                    CheckBox chkSelection = dgNote.Items[0].Cells[0].FindControl("cbSel") as CheckBox;
                    chkSelection.Checked = vis;
                }
                else
                {
                    btn_modifica.Enabled = false;
                    btn_rimuovi.Enabled = false;
                }
            }
            else
            {
                pnl_nuovaNota.Visible = true;
                btn_modifica.Enabled = false;
                btn_rimuovi.Enabled = false;
                btn_nuova.Enabled = false;
            }
            if (!ddlFiltroRf.Enabled)
                btn_nuova.Enabled = false;
        }

        private void CaricaComboRegistri()
        {
            int truncate = 45;
            this.ddl_regRF.Items.Clear();
            DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(Page, "1", "");
            if (listaRF != null && listaRF.Length > 0)
            {
                ListItem lit = new ListItem();
                lit.Value = "";
                lit.Text = " ";
                this.ddl_regRF.Items.Add(lit);
                foreach (DocsPaWR.Registro regis in listaRF)
                {
                    ListItem li = new ListItem();
                    li.Value = regis.systemId;
                    if (regis.descrizione.Length > truncate)
                        li.Text = regis.codRegistro + " - " + regis.descrizione.Substring(0, truncate) + "...";
                    else
                        li.Text = regis.codRegistro + " - " + regis.descrizione;
                    this.ddl_regRF.Items.Add(li);
                }
            }
        }

        private void CaricaRfVisibili()
        {
            int truncate = 60;
            this.ddlFiltroRf.Items.Clear();
            DocsPaWR.Registro[] listaRF = UserManager.getListaRegistriWithRF(Page, "1", "");
            if (listaRF != null && listaRF.Length > 0)
            {
                if (listaRF.Length == 1)
                {
                    ListItem li = new ListItem();
                    li.Value = listaRF[0].systemId;
                    if (listaRF[0].descrizione.Length > truncate)
                        li.Text = listaRF[0].codRegistro + " - " + listaRF[0].descrizione.Substring(0, truncate) + "...";
                    else
                        li.Text = listaRF[0].codRegistro + " - " + listaRF[0].descrizione;
                    this.ddlFiltroRf.Items.Add(li);
                }
                else
                {
                    ListItem lit = new ListItem();
                    lit.Value = "";
                    lit.Text = "Seleziona rf";
                    this.ddlFiltroRf.Items.Add(lit);
                    ListItem lit2 = new ListItem();
                    lit2.Value = "T";
                    lit2.Text = "Tutti";
                    this.ddlFiltroRf.Items.Add(lit2);
                    foreach (DocsPaWR.Registro regis in listaRF)
                    {
                        ListItem li = new ListItem();
                        li.Value = regis.systemId;
                        if (regis.descrizione.Length > truncate)
                            li.Text = regis.codRegistro + " - " + regis.descrizione.Substring(0, truncate) + "...";
                        else
                            li.Text = regis.codRegistro + " - " + regis.descrizione;
                        this.ddlFiltroRf.Items.Add(li);
                    }
                }
            }
            else
            {
                ddlFiltroRf.Enabled = false;
            }
        }

        #region GESTIONE DATAGRID
        protected DocsPaWR.NotaElenco[] ListaNote
        {
            get
            {
                if (this.ViewState["ListaNote"] != null)
                    return (DocsPaWR.NotaElenco[])this.ViewState["ListaNote"];
                else
                    return new DocsPaWR.NotaElenco[0];
            }
            set
            {
                this.ViewState["ListaNote"] = value;
            }
        }

        protected void BindGridAndSelect()
        {
            DataGridItem item;
            if (this.ListaNote != null)
            {
                DocsPaWR.NotaElenco[] note = this.ListaNote;
                this.dgNote.DataSource = note;
                this.dgNote.DataBind();
                if (ListaNote.Length > 0)
                {
                    this.dgNote.Visible = true;
                    lbl_messaggio.Text = "Elenco note - Trovati " + numNote.ToString() + " elementi";
                }
                else
                {
                    this.dgNote.Visible = false;
                    visib_pulsanti(true);
                }
            }
            else
            {
                this.dgNote.Visible = false;
                visib_pulsanti(true);
            }
            btn_nuova.Enabled = true;
        }

        private void disabilitaCheckDataGrid()
        {
            foreach (DataGridItem item in this.dgNote.Items)
            {
                CheckBox chkSelection = item.Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                    chkSelection.Checked = false;
            }
        }

        protected void cbSel_CheckedChanged(object sender, EventArgs e)
        {

            pnl_nuovaNota.Visible = false;
            btn_nuova.Enabled = true;
            btn_modifica.Enabled = false;
            btn_rimuovi.Enabled = true;
            int elemSelezionati = 0;
            for (int i = 0; i < dgNote.Items.Count; i++)
            {
                DataGridItem item = dgNote.Items[i];
                CheckBox chkSelection = item.Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                    elemSelezionati++;
            }
            if (elemSelezionati == 0)
                btn_rimuovi.Enabled = false;
            if (elemSelezionati == 1)
                btn_modifica.Enabled = true;
        }

        //Controllo se sono state selezionate alcune note dall'elenco
        //Nel caso in cui sia stata selezionata solo una nota memorizzo la sua posizione
        //nell'elenco
        private int verificaSelezioneNota()
        {
            int posizione = -1;
            int numSel = 0;
            for (int i = 0; i < dgNote.Items.Count; i++)
            {
                DataGridItem item = dgNote.Items[i];
                CheckBox chkSelection = item.Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                {
                    posizione = i;
                    numSel++;
                }
            }
            if (numSel != 1)
                posizione = -1;
            //else
            //    ViewState.Add("posizione", posizione);
            return posizione;
        }
        #endregion

        #region Excel
        private void UploadBtn_Click(object sender, EventArgs e)
        {
            if (pnl_risultatiExcel.Visible)
            {
                pnl_risultatiExcel.Visible = false;
                btn_nuova.Text = "Inserisci";
                btn_nuova.ToolTip = "Inserisci nuova nota";
                btn_nuova.Enabled = true;
            }

            //Verfica che il controllo uploadedFile contenga un file 
            if (this.uploadedFile.HasFile)
            {
                string fileName = Server.HtmlEncode(uploadedFile.FileName);
                string extension = System.IO.Path.GetExtension(fileName);

                if (extension == ".xls")
                {
                    ViewState.Add("fileExcel", uploadedFile.PostedFile.FileName);
                    HttpPostedFile p = uploadedFile.PostedFile;
                    System.IO.Stream fs = p.InputStream;
                    byte[] dati = new byte[fs.Length];
                    fs.Read(dati, 0, (int)fs.Length);
                    fs.Close();
                    ViewState.Add("DatiExcel", dati);
                    this.lbl_fileExcel.Text = "File correttamente caricato.            Elimina ";
                    this.btn_elimina_excel.Visible = true;
                    this.btn_elimina_excel.Attributes.Add("onclick", "if (!window.confirm('Sei sicuro di voler eliminare il file excel selezionato?')) {return false};");
                    SetFocus(btn_nuova);
                }
                else
                {
                    Page.RegisterStartupScript("", "<script>alert('Selezionare solo file .xls!');</script>");
                }
            }
        }

        protected void btn_elimina_excel_Click(object sender, System.EventArgs e)
        {
            if (pnl_risultatiExcel.Visible)
            {
                pnl_risultatiExcel.Visible = false;
                btn_nuova.Text = "Inserisci";
                btn_nuova.ToolTip = "Inserisci nuova nota";
                btn_nuova.Enabled = true;
            }

            this.lbl_fileExcel.Text = "Nessun file excel caricato.";
            this.btn_elimina_excel.Visible = false;
            ViewState.Remove("fileExcel");
            ViewState.Remove("DatiExcel");
        }

        protected void chk_Excel_Changed(object sender, EventArgs e)
        {
            if (pnl_risultatiExcel.Visible)
            {
                pnl_risultatiExcel.Visible = false;
                btn_nuova.Text = "Inserisci";
                btn_nuova.ToolTip = "Inserisci nuova nota";
                btn_nuova.Enabled = true;
            }

            if (chk_Excel.Checked)
            {
                SetFocus(UploadBtn);
                this.ddl_regRF.Enabled = false;
                this.txt_descNota.Enabled = false;
                this.uploadedFile.Enabled = true;
                this.UploadBtn.Enabled = true;
            }
            else
            {
                this.ddl_regRF.Enabled = true;
                this.txt_descNota.Enabled = true;
                this.uploadedFile.Enabled = false;
                this.UploadBtn.Enabled = false;
            }
        }
        #endregion

    }

}
