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
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{
    public partial class VisibilitaFrame : DocsPAWA.CssPage
    {
        //protected System.Web.UI.WebControls.DropDownList ddl_tipologiaDoc;
        //protected System.Web.UI.WebControls.Panel panel_Contenuto;
        protected System.Web.UI.WebControls.DropDownList ddl_Contatori;
        protected Table table;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
        protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        //protected System.Web.UI.WebControls.Panel pnlAnno;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //GIORDANO IACOZZILLI 16/07/2013
                //Reset lbl
                lblDocInDeposito.Visible = false;
                //FINE

                this.IF_VisDoc.NavigateTo = "../blank_page.htm";
                ddl_registri = GetRegistriByRuolo(ddl_registri, this);
                tbx_anno.Text = System.DateTime.Now.Year.ToString();

                CaricaTipologia(this.ddl_tipologiaDoc);
                ddl_tipologiaDoc.SelectedIndex = 0;

                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1")
                {
                    ListItem tipDoc = new ListItem();
                    tipDoc.Value = "Tipologia";
                    tipDoc.Text = "Cerca per Tipologia";
                    this.rblTipo.Items.Add(tipDoc);
                }
            }

            if (Session["template"] != null)
                inizializzaPanelContenuto();



        }

        protected void btn_cerca_Click(object sender, EventArgs e)
        {
            //cerco idProfile partendo dai dati inseriti
            string idDocProt = string.Empty;
            int idProfile = 0;
            bool numeroRisultati = true;
            DocsPaWR.InfoDocumento[] ListaDoc = null;
            string inArchivio = "-1";

            switch (rblTipo.SelectedValue.ToString())
            {
                case "P":
                    idDocProt = tbx_numProto.Text;
                    idProfile = UserManager.getIdProfileByData(UserManager.getInfoUtente(this), idDocProt, tbx_anno.Text, ddl_registri.SelectedValue, out inArchivio);
                    break;

                case "NP":
                    idDocProt = tbxDoc.Text;
                    idProfile = UserManager.getIdProfileByData(UserManager.getInfoUtente(this), idDocProt, null, null, out inArchivio);
                    break;

                case "Tipologia":
                    // parametri in input: 
                    //1) tipologia documento 2)tipo contatore 3)AOO o RF 4) Numero contatore
                    if (ddl_tipologiaDoc.SelectedIndex == 0)
                    {
                        Response.Write("<script>alert('Attenzione selezionare una tipologia documento.')</script>");
                        this.panel_Contenuto.Visible = false;
                        this.pnl_RFAOO.Visible = false;
                        this.pnlAnno.Visible = false;
                        this.pnlNumero.Visible = false;
                        return;
                    }
                    DropDownList ddl = (DropDownList)panel_Contenuto.FindControl("ddl_Contatori");
                    if (ddl != null && ddl.SelectedValue == "")
                    {
                        Response.Write("<script>alert('Attenzione selezionare un contatore.')</script>");
                        this.panel_Contenuto.Visible = true;
                        this.pnl_RFAOO.Visible = false;
                        return;
                    }
                    if (string.IsNullOrEmpty(this.TxtAnno.Text))
                    {
                        Response.Write("<script>alert('Attenzione selezionare un anno.')</script>");
                        this.pnl_RFAOO.Visible = true;
                        this.lblAooRF.Visible = true;
                        this.ddlAooRF.Visible = true;
                        return;
                    }
                    if (string.IsNullOrEmpty(this.TxtNumero.Text))
                    {
                        Response.Write("<script>alert('Attenzione selezionare un numero contatore.')</script>");
                        this.pnl_RFAOO.Visible = true;
                        this.lblAooRF.Visible = true;
                        this.ddlAooRF.Visible = true;
                        return;
                    }

                    DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                    //DocsPAWA.DocsPaWR.Templates template = new DocsPAWA.DocsPaWR.Templates();
                    //DocsPAWA.DocsPaWR.OggettoCustom ogg = new DocsPAWA.DocsPaWR.OggettoCustom();
                    //ogg.SYSTEM_ID = Convert.ToInt32(ddl.SelectedValue);
                    //ogg.VALORE_DATABASE = this.TxtNumero.Text + "@" + this.TxtNumero.Text;
                    //ogg.ID_AOO_RF = this.ddl_registri.SelectedValue;
                    //template.ELENCO_OGGETTI[0] = ogg;

                    for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                    {
                        DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                        if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("Contatore"))
                        {
                            if (ddl != null && ddl.SelectedIndex != -1)
                            {
                                if (oggettoCustom.SYSTEM_ID == Convert.ToInt32(ddl.SelectedValue))
                                {
                                    //oggettoCustom.TIPO_CONTATORE = ddl.SelectedValue;
                                    oggettoCustom.VALORE_DATABASE = this.TxtNumero.Text + "@" + this.TxtNumero.Text;
                                    oggettoCustom.ID_AOO_RF = this.ddlAooRF.SelectedValue;
                                }
                            }
                            else
                            {
                                oggettoCustom.VALORE_DATABASE = this.TxtNumero.Text + "@" + this.TxtNumero.Text;
                                oggettoCustom.ID_AOO_RF = this.ddlAooRF.SelectedValue;
                            }
                        }
                        else
                        {
                            // poichè la ricerca deve essere fatta per un solo contatore, metto a
                            // stringa vuota il valore di tutti gli altri oggetti del template
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                            oggettoCustom.ID_AOO_RF = string.Empty;
                        }
                        //}
                    }

                    qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
                    qV[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
                    fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[0];

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                    fV1.valore = this.TxtAnno.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
                    fV1.valore = this.ddl_tipologiaDoc.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.FROM_RICERCA_VIS.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
                    fV1.template = template;
                    fV1.valore = "Profilazione Dinamica";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    qV[0] = fVList;

                    int numTotPage = 0;
                    int nRec = 0;
                    SearchResultInfo[] idProfileList;
                    ListaDoc = DocumentManager.getQueryInfoDocumentoPaging(UserManager.getInfoUtente(this).idGruppo, UserManager.getInfoUtente(this).idPeople, this, qV, 1, out numTotPage, out nRec, false, false, false, false, out idProfileList);

                    if (ListaDoc.Length > 1)
                    {
                        // non dovrebbe succedere ma per errori di inserimento nel DB, potrebbe 
                        // accadere che questa query restituisca più di un risultato (se il numero
                        // del contatore è valorizzato)--> in questo caso si restituisce
                        // solo il primo documento trovato.

                        if (!string.IsNullOrEmpty(this.TxtNumero.Text))
                        {
                            idProfile = Convert.ToInt32(ListaDoc[0].idProfile);
                            inArchivio = ListaDoc[0].inArchivio;
                        }
                        else
                            numeroRisultati = false;
                    }
                    else
                    {
                        if (ListaDoc.Length != 0)
                        {
                            idProfile = Convert.ToInt32(ListaDoc[0].idProfile);
                            inArchivio = ListaDoc[0].inArchivio;
                        }
                        else
                            idProfile = 0;
                    }
                    //this.ddlAooRF.Visible = true;
                    break;
            }

            if (numeroRisultati)
            {
                if (idProfile > 0 || inArchivio == "1")
                    IF_VisDoc.NavigateTo = "visibilitaDocumento.aspx?From=ricerca&VisFrame=" + idProfile + "&inArchivio=" + inArchivio;
                //else
                    //**********************************************************************************************
                    // GIORDANO IACOZZILLI: 16/07/2013
                    // Se nel corrente cè ed è abilitata la chiave BE_HAS_ARCHIVE, verifico se l'id cercato 
                    // è presente anche nel deposito.
                    //**********************************************************************************************
                    //if (!string.IsNullOrEmpty(utils.InitConfigurationKeys.GetValue("0", "BE_HAS_ARCHIVE"))
                    //    && utils.InitConfigurationKeys.GetValue("0", "BE_HAS_ARCHIVE").Equals("1"))
                    //{
                    //    DocsPaWR.DocsPaWebService docsPaWS = new DocsPaWebService();
                    //    Int32 DocInDep = docsPaWS.IsIDdocInArchive(Convert.ToInt32(idDocProt));
                    //    if (DocInDep > 0)
                    //    {
                    //        lblDocInDeposito.Visible = true;
                    //        lblDocInDeposito.InnerText = "Il documento ricercato è stato versato in Deposito";
                    //    }
                    //}
                    //else
                    //    ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert('Documento non trovato. \\n Verificare i dati inseriti.')</script>");
                    //**********************************************************************************************
                    // GIORDANO IACOZZILLI: 16/07/2013
                    //**********************************************************************************************
            }
            else
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert('I parametri di ricerca inseriti hanno trovato più di un risultato. \\n Verificare i dati inseriti.')</script>");


        }

        #region Metodi di supporto
        public static DropDownList GetRegistriByRuolo(DropDownList list, Page page)
        {
            try
            {
                ArrayList registri = null;
                bool filtroAoo = false;
                DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistriNoFiltroAOO(page, out filtroAoo);

                if (userRegistri != null && filtroAoo)
                {
                    registri = new ArrayList(userRegistri);
                }
                else
                {
                    DocsPAWA.DocsPaWR.Ruolo ruolo = UserManager.getRuolo(page);
                    registri = new ArrayList(UserManager.GetRegistriByRuolo(page, ruolo.systemId));
                }
                list.Items.Clear();
                foreach (DocsPAWA.DocsPaWR.Registro reg in registri)
                {
                    list.Items.Add(new ListItem(reg.codRegistro, reg.systemId));
                }
                return list;
            }

            catch (Exception ex)
            {
                //errore nel recupero dei dati
                throw ex;
            }
        }
        #endregion

        protected void IF_VisDoc_Navigate(object sender, EventArgs e)
        {

        }

        protected void ddl_registri_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void rblTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (rblTipo.SelectedValue.ToString())
            {
                case "NP":
                    pnlDOC.Visible = true;
                    pnlProt.Visible = false;
                    pnlTipologia.Visible = false;
                    pnl_RFAOO.Visible = false;
                    panel_Contenuto.Visible = false;
                    pnlAnno.Visible = false;
                    pnlNumero.Visible = false;
                    break;

                case "P":
                    pnlDOC.Visible = false;
                    pnlProt.Visible = true;
                    pnlTipologia.Visible = false;
                    pnl_RFAOO.Visible = false;
                    panel_Contenuto.Visible = false;
                    pnlAnno.Visible = false;
                    pnlNumero.Visible = false;
                    break;

                case "Tipologia":
                    pnlDOC.Visible = false;
                    pnlProt.Visible = false;
                    pnlTipologia.Visible = true;
                    this.ddl_tipologiaDoc.SelectedIndex = 0;
                    pnl_RFAOO.Visible = false;
                    panel_Contenuto.Visible = false;
                    pnlAnno.Visible = false;
                    pnlNumero.Visible = false;
                    break;
            }
        }

        #region metodi per la ricerca per contatori

        //Recupera tutte le tipologie di documento che hanno almeno un contatore che dipende dall'anno
        // viene utilizzato il metodo che restituisce i contatori di repertorio (vedi Archivio/OpzioniArchivio)
        // a cui viene passato il booleano false per non prendere i contatori di repertorio 
        private void CaricaTipologia(DropDownList ddl)
        {
            DocsPaWR.Templates[] listaTemplates;
            listaTemplates = DocumentManager.getTipoAttoTrasfDeposito(this, UserManager.getInfoUtente(this).idAmministrazione, false);
            ddl.Items.Clear();
            ddl.Items.Add("");
            int cont = 0;
            if (listaTemplates != null)
            {
                for (int i = 0; i < listaTemplates.Length; i++)
                {
                    DocsPaWR.Templates templ = listaTemplates[i];
                    if (templ.ABILITATO_SI_NO.Equals("1") && templ.IN_ESERCIZIO.ToUpper().Equals("SI"))
                    {
                        ddl.Items.Add(templ.DESCRIZIONE);
                        ddl.Items[cont + 1].Value = templ.SYSTEM_ID.ToString();
                        cont++;
                    }
                }
            }
        }

        protected void ddl_tipologiaDoc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(ddl_tipologiaDoc.SelectedValue))
            {
                Session.Remove("template");
                panel_Contenuto.Controls.Clear();
                this.panel_Contenuto.Visible = false;
                this.pnl_RFAOO.Visible = false;
                this.pnlAnno.Visible = false;
                this.pnlNumero.Visible = false;
            }
            else
            {
                //this.TxtAnno.Visible = true;
                //this.lblAnno.Visible = true;
                this.pnlAnno.Visible = true;
                this.panel_Contenuto.Visible = true;
                this.pnl_RFAOO.Visible = true;
                this.ddlAooRF.Visible = false;
                this.lblAooRF.Visible = false;
                this.pnlNumero.Visible = true;
                this.TxtAnno.Text = "";
                string idTemplate = ddl_tipologiaDoc.SelectedValue;
                DocsPaWR.Templates templateInSessione = (DocsPaWR.Templates)Session["template"];
                if (!string.IsNullOrEmpty(idTemplate) && templateInSessione != null && !string.IsNullOrEmpty(templateInSessione.SYSTEM_ID.ToString()))
                {
                    if (ddl_tipologiaDoc.SelectedValue != templateInSessione.SYSTEM_ID.ToString())
                    {
                        Session.Remove("template");
                        panel_Contenuto.Controls.Clear();
                    }
                    panel_Contenuto.Controls.Clear();
                }
                if (idTemplate != "")
                {
                    DocsPaWR.Templates template = ProfilazioneDocManager.getTemplateById(idTemplate, this);
                    if (template != null)
                    {
                        Session.Add("template", template);
                        //pnl_RFAOO.Visible = false;
                        ddlAooRF.Items.Clear();
                        inizializzaPanelContenuto();
                        this.TxtAnno.Visible = true;
                        this.TxtAnno.Text = "";
                        this.lblAnno.Visible = true;

                    }
                    else
                    {
                        pnl_RFAOO.Visible = false;
                    }
                }
            }
        }

        //Recupera i contatori per una scelta tipologia di documento e li inserisce nella 
        //dropdownlist ddl_Contatori
        private void inizializzaPanelContenuto()
        {
            //pnl_RFAOO.Visible = false;
            if (Session["template"] != null)
            {
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                table = new Table();
                table.ID = "table_Contatori";
                TableCell cell_2 = new TableCell();
                int numContatori = 0;
                string testoUnicoContatore = "";
                string idUnicoContatore = "";
                DocsPAWA.DocsPaWR.OggettoCustom oggettoUnico = null;
                ddl_Contatori = new DropDownList();
                ddl_Contatori.ID = "ddl_Contatori";
                //ddl_Contatori.Font.Size = FontUnit.Point(7);
                ddl_Contatori.CssClass = "titolo_scheda";
                ddl_Contatori.Font.Name = "Verdana";
                foreach (DocsPAWA.DocsPaWR.OggettoCustom oggetto in template.ELENCO_OGGETTI)
                {
                    //if (oggetto.REPERTORIO == "1")
                    //{
                    //rendo visibili i pannelli
                    if (oggetto.TIPO.DESCRIZIONE_TIPO == "Contatore")
                    {
                        if (oggetto.DESCRIZIONE.Equals(""))
                        {
                            return;
                        }
                        //testoUnicoContatore e idUnicoContatore servono nel caso in cui sia presente un solo
                        //contatore, in questo caso non visualizzo la dropdownlist ma una semplice label
                        testoUnicoContatore = oggetto.DESCRIZIONE.ToString();
                        idUnicoContatore = oggetto.SYSTEM_ID.ToString();
                        oggettoUnico = oggetto;
                        ddl_Contatori.Items.Add(new ListItem(oggetto.DESCRIZIONE.ToString(), oggetto.SYSTEM_ID.ToString()));
                        numContatori++;
                    }
                    //}
                }
                if (oggettoUnico != null)
                {
                    TableRow row = new TableRow();
                    row.ID = "row_Contatori";
                    TableCell cell_1 = new TableCell();
                    TableCell cell_3 = new TableCell();
                    if (numContatori > 1)
                    {
                        ListItem emptyCont = new ListItem();
                        emptyCont.Value = "";
                        emptyCont.Text = "";
                        ddl_Contatori.Items.Add(emptyCont);
                        ddl_Contatori.SelectedValue = "";

                        this.ddlAooRF.Visible = false;

                        cell_1.Controls.Add(ddl_Contatori);
                        ddl_Contatori.AutoPostBack = true;
                        this.ddl_Contatori.SelectedIndexChanged += new System.EventHandler(this.ddl_Contatori_SelectedIndexChanged);
                    }
                    else
                    {
                        Label lblContatore = new Label();
                        lblContatore.ID = "lblContatore";
                        //lblContatore.Font.Size = FontUnit.Point(7);
                        lblContatore.CssClass = "titolo_scheda";
                        lblContatore.Font.Name = "Verdana";
                        lblContatore.Text = testoUnicoContatore;
                        cell_1.Controls.Add(lblContatore);
                        Label lblContatoreID = new Label();
                        lblContatoreID.ID = "lblContID";
                        lblContatoreID.Text = idUnicoContatore;
                        lblContatoreID.Visible = false;
                        cell_3.Controls.Add(lblContatoreID);
                        ddl_Contatori.Visible = false;
                        if (ddlAooRF.SelectedIndex == -1)
                        {
                            DocsPaWR.Ruolo ruoloUtente = UserManager.getRuolo(this);
                            DocsPaWR.Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "", "");

                            switch (oggettoUnico.TIPO_CONTATORE)
                            {
                                case "T":
                                    break;
                                case "A":
                                    lblAooRF.Text = "&nbsp;AOO";
                                    ////Aggiungo un elemento vuoto
                                    ListItem it = new ListItem();
                                    it.Value = "";
                                    it.Text = "";
                                    ddlAooRF.Items.Add(it);
                                    //Distinguo se è un registro o un rf
                                    for (int i = 0; i < registriRfVisibili.Length; i++)
                                    {
                                        ListItem item = new ListItem();
                                        if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "0")
                                        {
                                            item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                            item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                            ddlAooRF.Items.Add(item);
                                        }
                                    }
                                    ddlAooRF.Width = 100;
                                    this.pnl_RFAOO.Visible = true;
                                    break;
                                case "R":
                                    lblAooRF.Text = "&nbsp;RF";
                                    ////Aggiungo un elemento vuoto
                                    ListItem it_1 = new ListItem();
                                    it_1.Value = "";
                                    it_1.Text = "";
                                    ddlAooRF.Items.Add(it_1);
                                    //Distinguo se è un registro o un rf
                                    for (int i = 0; i < registriRfVisibili.Length; i++)
                                    {
                                        ListItem item = new ListItem();
                                        if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "1" && ((DocsPaWR.Registro)registriRfVisibili[i]).rfDisabled == "0")
                                        {
                                            item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                            item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                            ddlAooRF.Items.Add(item);
                                        }
                                    }
                                    ddlAooRF.Width = 100;
                                    this.pnl_RFAOO.Visible = true;
                                    break;
                            }
                        }
                    }
                    row.Cells.Add(cell_1);
                    if (cell_3 != null)
                        row.Cells.Add(cell_3);
                    row.Cells.Add(cell_2);
                    table.Rows.Add(row);

                    panel_Contenuto.Controls.Add(table);
                    this.panel_Contenuto.Visible = true;
                }
                //this.btn_ricerca.Visible = true;
            }
        }

        //Se il contatore è di tipo AOO o rf recupera la lista di AOO o la lista di rf 
        //e li inserisci nella dropdownlist ddlAooRF
        private void ddl_Contatori_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ddl_Contatori.SelectedValue == "")
                this.lblAooRF.Visible = false;
            ddlAooRF.Items.Clear();
            //this.pnl_RFAOO.Visible = false;
            this.panel_Contenuto.Visible = true;
            this.pnl_RFAOO.Visible = true;
            this.pnlAnno.Visible = true;
            this.pnlNumero.Visible = true;
            this.TxtAnno.Text = "";
            Session["aoo_rf"] = "";

            Session.Remove("template");
            string idTemplate = ddl_tipologiaDoc.SelectedValue;
            DocsPaWR.Templates template = ProfilazioneDocManager.getTemplateById(idTemplate, this);
            Session.Add("template", template);

            // DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            foreach (DocsPAWA.DocsPaWR.OggettoCustom oggetto in template.ELENCO_OGGETTI)
            {
                if (oggetto.TIPO.DESCRIZIONE_TIPO == "Contatore")
                {
                    if (oggetto.DESCRIZIONE.Equals(""))
                    {
                        return;
                    }

                    if (oggetto.SYSTEM_ID.ToString().Equals(ddl_Contatori.SelectedItem.Value))
                    {
                        DocsPaWR.Ruolo ruoloUtente = UserManager.getRuolo(this);
                        DocsPaWR.Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "", "");
                        this.pnl_RFAOO.Visible = false;
                        this.lblAooRF.Visible = true;
                        switch (oggetto.TIPO_CONTATORE)
                        {
                            case "T":
                                this.pnl_RFAOO.Visible = false;
                                break;
                            case "A":
                                lblAooRF.Text = "&nbsp;AOO";
                                ////Aggiungo un elemento vuoto
                                ListItem it = new ListItem();
                                it.Value = "";
                                it.Text = "";
                                ddlAooRF.Items.Add(it);
                                //Distinguo se è un registro o un rf
                                for (int i = 0; i < registriRfVisibili.Length; i++)
                                {
                                    ListItem item = new ListItem();
                                    if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "0")
                                    {
                                        item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                        item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                        ddlAooRF.Items.Add(item);
                                    }
                                }
                                ddlAooRF.Width = 100;
                                this.pnl_RFAOO.Visible = true;
                                this.ddlAooRF.Visible = true;
                                break;
                            case "R":
                                lblAooRF.Text = "&nbsp;RF";
                                ////Aggiungo un elemento vuoto
                                ListItem it_1 = new ListItem();
                                it_1.Value = "";
                                it_1.Text = "";
                                ddlAooRF.Items.Add(it_1);
                                //Distinguo se è un registro o un rf
                                for (int i = 0; i < registriRfVisibili.Length; i++)
                                {
                                    ListItem item = new ListItem();
                                    if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "1" && ((DocsPaWR.Registro)registriRfVisibili[i]).rfDisabled == "0")
                                    {
                                        item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                        item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                        ddlAooRF.Items.Add(item);
                                    }
                                }
                                ddlAooRF.Width = 100;
                                this.pnl_RFAOO.Visible = true;
                                this.ddlAooRF.Visible = true;
                                break;
                        }

                    }
                    else
                    {
                        // poichè la ricerca deve essere fatta per un solo contatore, metto a
                        // stringa vuota il valore di tutti gli altri oggetti del template
                        oggetto.VALORE_DATABASE = string.Empty;
                    }
                }
            }
        }

        #endregion
    }
}
