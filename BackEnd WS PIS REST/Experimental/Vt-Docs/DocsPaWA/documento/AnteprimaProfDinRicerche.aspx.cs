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
using DocsPaWA.UserControls;
using System.Collections.Generic;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.documento
{
    public class AnteprimaProfDinRicerche : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Label lbl_NomeModello;
        protected System.Web.UI.WebControls.Panel panel_Contenuto;
        protected System.Web.UI.WebControls.Label Label_Avviso;
        protected System.Web.UI.WebControls.Button btn_ConfermaProfilazione;
        protected DocsPAWA.DocsPaWR.Templates template;
        protected System.Web.UI.WebControls.Button btn_resettaPopUp;
        protected System.Web.UI.WebControls.Button btn_Chiudi;
        protected bool focus;
        protected Table table;
        DocsPAWA.ricercaDoc.SchedaRicerca sRic = null;
        protected ArrayList dirittiCampiRuolo;
        protected string reset = string.Empty;
        protected Dictionary<string, Corrispondente> dic_Corr;

        private void Page_Load(object sender, System.EventArgs e)
        {
            template = (DocsPAWA.DocsPaWR.Templates)Session["templateRicerca"];
            reset = Request.QueryString["reset"].ToString();
            if (template == null)
                return;

            if (!IsPostBack)
            {
                UserManager.setCorrispondenteSelezionato(this, null);
            }
            sRic = (DocsPAWA.ricercaDoc.SchedaRicerca)Session[DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY];

            lbl_NomeModello.Text = template.DESCRIZIONE;

            if (Session["focus"] == null)
                Session.Add("focus", true);

            inserisciComponenti();
        }

        public void ricercaFiltro()
        {
            if (sRic != null && sRic.FiltriRicerca != null)
            {
                bool found = false;
                DocsPaWR.FiltroRicerca aux = null;
                for (int i = 0; !found && i < sRic.FiltriRicerca[0].Length; i++)
                {
                    aux = sRic.FiltriRicerca[0][i];
                    if (aux != null && aux.argomento == DocsPAWA.DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString())
                        this.template = aux.template;
                }
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.btn_resettaPopUp.Click += new System.EventHandler(this.btn_resettaPopUp_Click);
            this.btn_ConfermaProfilazione.Click += new System.EventHandler(this.btn_ConfermaProfilazione_Click);
            this.btn_Chiudi.Click += new System.EventHandler(this.btn_Chiudi_Click);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        #region inserisciComponenti
        public void inserisciComponenti()
        {
            ricercaFiltro();
            table = new Table();
            table.Width = Unit.Percentage(100);
            dirittiCampiRuolo = ProfilazioneDocManager.getDirittiCampiTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), this);
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                ProfilazioneDocManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                {
                    case "CampoDiTesto":
                        inserisciCampoDiTesto(oggettoCustom);
                        break;
                    case "CasellaDiSelezione":
                        inserisciCasellaDiSelezione(oggettoCustom);
                        break;
                    case "MenuATendina":
                        inserisciMenuATendina(oggettoCustom);
                        break;
                    case "SelezioneEsclusiva":
                        inserisciSelezioneEsclusiva(oggettoCustom);
                        break;
                    case "Contatore":
                        inserisciContatore(oggettoCustom);
                        break;
                    case "Data":
                        inserisciData(oggettoCustom);
                        break;
                    case "Corrispondente":
                        inserisciCorrispondente(oggettoCustom);
                        break;
                    case "ContatoreSottocontatore":
                        inserisciContatoreSottocontatore(oggettoCustom);
                        break;
                    case "OggettoEsterno":
                        inserisciOggettoEsterno(oggettoCustom);
                        break;
                }
            }
            //table.GridLines = GridLines.Both;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.Rows[i].Cells.Count == 2)
                {
                    table.Rows[i].Cells[0].Width = Unit.Percentage(30);
                    table.Rows[i].Cells[1].Width = Unit.Percentage(70);
                }
                if (table.Rows[i].Cells.Count == 1)
                    table.Rows[i].Cells[0].Width = Unit.Percentage(100);
            }
            panel_Contenuto.Controls.Add(table);
        }
        #endregion inserisciComponenti

        #region inserisciCampoDiTesto
        public void inserisciCampoDiTesto(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaCampoDiTesto = new Label();
            TextBox txt_CampoDiTesto = new TextBox();
            if (oggettoCustom.MULTILINEA.Equals("SI"))
            {
                etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;
                etichettaCampoDiTesto.Font.Size = FontUnit.Point(8);
                etichettaCampoDiTesto.Font.Bold = true;
                etichettaCampoDiTesto.Font.Name = "Verdana";

                //txt_CampoDiTesto.Width = 430;
                txt_CampoDiTesto.Width = Unit.Percentage(100);
                txt_CampoDiTesto.TextMode = TextBoxMode.MultiLine;

                if (oggettoCustom.NUMERO_DI_LINEE.Equals(""))
                {
                    txt_CampoDiTesto.Height = 55;
                }
                else
                {
                    txt_CampoDiTesto.Rows = Convert.ToInt32(oggettoCustom.NUMERO_DI_LINEE);
                }

                if (oggettoCustom.NUMERO_DI_CARATTERI.Equals(""))
                {
                    txt_CampoDiTesto.MaxLength = 150;
                }
                else
                {
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }

                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;

                TableRow row_1 = new TableRow();
                TableCell cell_1 = new TableCell();
                cell_1.Controls.Add(etichettaCampoDiTesto);
                cell_1.ColumnSpan = 2;
                row_1.Cells.Add(cell_1);
                table.Rows.Add(row_1);

                TableRow row_2 = new TableRow();
                TableCell cell_2 = new TableCell();
                cell_2.Controls.Add(txt_CampoDiTesto);
                cell_2.ColumnSpan = 2;
                row_2.Cells.Add(cell_2);
                table.Rows.Add(row_2);
            }
            else
            {
                etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;
                etichettaCampoDiTesto.Font.Size = FontUnit.Point(8);
                etichettaCampoDiTesto.Font.Bold = true;
                etichettaCampoDiTesto.Font.Name = "Verdana";

                if (!oggettoCustom.NUMERO_DI_CARATTERI.Equals(""))
                {
                    //ATTENZIONE : La lunghezza della textBox non è speculare al numero massimo di
                    //caratteri che l'utente inserisce.
                    txt_CampoDiTesto.Width = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6;
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }
                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.CssClass = "comp_profilazione_anteprima";

                TableRow row = new TableRow();
                TableCell cell_1 = new TableCell();
                cell_1.Controls.Add(etichettaCampoDiTesto);
                row.Cells.Add(cell_1);
                TableCell cell_2 = new TableCell();
                cell_2.Controls.Add(txt_CampoDiTesto);
                row.Cells.Add(cell_2);
                table.Rows.Add(row);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaCampoDiTesto, txt_CampoDiTesto, oggettoCustom, template);

            if ((bool)Session["focus"])
            {
                //SetFocus(txt_CampoDiTesto);
                Session.Add("focus", false);
            }
        }
        #endregion inserisciCampoDiTesto

        #region inserisciCasellaDiSelezione
        public void inserisciCasellaDiSelezione(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaCasellaSelezione = new Label();
            etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE;
            etichettaCasellaSelezione.Font.Size = FontUnit.Point(8);
            etichettaCasellaSelezione.Font.Bold = true;
            etichettaCasellaSelezione.Font.Name = "Verdana";
            //etichettaCasellaSelezione.Width = 430;
            etichettaCasellaSelezione.Width = Unit.Percentage(100);

            CheckBoxList casellaSelezione = new CheckBoxList();
            casellaSelezione.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                casellaSelezione.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                {
                    valoreDiDefault = i;
                }
            }
            casellaSelezione.CssClass = "comp_profilazione_anteprima";
            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Vertical;
            }
            if (valoreDiDefault != -1)
            {
                casellaSelezione.SelectedIndex = valoreDiDefault;
            }

            if (oggettoCustom.VALORI_SELEZIONATI != null)
            {
                impostaSelezioneCaselleDiSelezione(oggettoCustom, casellaSelezione);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaCasellaSelezione, casellaSelezione, oggettoCustom, template);

            TableRow row_1 = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaCasellaSelezione);
            cell_1.ColumnSpan = 2;
            row_1.Cells.Add(cell_1);
            table.Rows.Add(row_1);

            TableRow row_2 = new TableRow();
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(casellaSelezione);
            cell_2.ColumnSpan = 2;
            row_2.Cells.Add(cell_2);
            table.Rows.Add(row_2);

            if ((bool)Session["focus"])
            {
                //SetFocus(casellaSelezione);
                Session.Add("focus", false);
            }
        }
        #endregion inserisciCasellaDiSelezione

        #region inserisciMenuATendina
        public void inserisciMenuATendina(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaMenuATendina = new Label();
            etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE;
            etichettaMenuATendina.Font.Size = FontUnit.Point(8);
            etichettaMenuATendina.Font.Bold = true;
            etichettaMenuATendina.Font.Name = "Verdana";

            DropDownList menuATendina = new DropDownList();
            menuATendina.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;

            // quando salviamo la ricerca documenti, viene salvato anche l'elenco dei valori possibili nel menu a tendina 
            // e questo elenco viene mostrato all'utente: ma in questo modo non vengono mostrati gli eventuali altri valori inseriti 
            // da amministrazione in un momento successivo al salvataggio della ricerca.
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            DocsPAWA.DocsPaWR.OggettoCustom[] oggCus = ws.GetValuesDropDownList(oggettoCustom.SYSTEM_ID.ToString(), "D");
            for (int i = 0; i < oggCus.Length; i++)
            {
                menuATendina.Items.Add(new ListItem(oggCus[i].VALORE_DATABASE, oggCus[i].VALORE_DATABASE));
            }

            //for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            //{
            //    DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
            //    menuATendina.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
            //}

            menuATendina.CssClass = "comp_profilazione_anteprima";
            if (valoreDiDefault != -1)
            {
                menuATendina.SelectedIndex = valoreDiDefault;
            }
            if (!(valoreDiDefault != -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
            {
                menuATendina.Items.Insert(0, "");
            }
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                menuATendina.SelectedIndex = impostaSelezioneMenuATendina(oggettoCustom.VALORE_DATABASE, menuATendina);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaMenuATendina, menuATendina, oggettoCustom, template);

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaMenuATendina);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(menuATendina);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);

            if ((bool)Session["focus"])
            {
                Session.Add("focus", false);
            }
        }
        #endregion inserisciMenuATendina

        #region inserisciSelezioneEsclusiva
        public void inserisciSelezioneEsclusiva(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            TableRow row_1 = new TableRow();
            TableCell cell_1 = new TableCell();

            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaSelezioneEsclusiva = new Label();
            ImageButton cancella_selezioneEsclusiva = new ImageButton();

            etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;
            etichettaSelezioneEsclusiva.Font.Size = FontUnit.Point(8);
            etichettaSelezioneEsclusiva.Font.Bold = true;
            etichettaSelezioneEsclusiva.Font.Name = "Verdana";
            etichettaSelezioneEsclusiva.Width = Unit.Percentage(90);

            if (!oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                cancella_selezioneEsclusiva.ID = "_" + oggettoCustom.SYSTEM_ID.ToString();
                cancella_selezioneEsclusiva.ImageUrl = "../images/cancella.gif";
                cancella_selezioneEsclusiva.CssClass = "resettaSelezioneEsclusiva";
                cancella_selezioneEsclusiva.Width = 10;
                cancella_selezioneEsclusiva.Height = 10;
                cancella_selezioneEsclusiva.Click += new System.Web.UI.ImageClickEventHandler(cancella_selezioneEsclusiva_Click);
                cell_1.Controls.Add(cancella_selezioneEsclusiva);
            }

            RadioButtonList selezioneEsclusiva = new RadioButtonList();
            selezioneEsclusiva.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                selezioneEsclusiva.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
            }
            selezioneEsclusiva.CssClass = "comp_profilazione_anteprima";
            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Vertical;
            }
            if (valoreDiDefault != -1)
            {
                selezioneEsclusiva.SelectedIndex = valoreDiDefault;
            }
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                selezioneEsclusiva.SelectedIndex = impostaSelezioneEsclusiva(oggettoCustom.VALORE_DATABASE, selezioneEsclusiva);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSelezioneEsclusiva(etichettaSelezioneEsclusiva, selezioneEsclusiva, cancella_selezioneEsclusiva, oggettoCustom, template);

            cell_1.Controls.Add(etichettaSelezioneEsclusiva);
            cell_1.ColumnSpan = 2;
            row_1.Cells.Add(cell_1);
            table.Rows.Add(row_1);

            TableRow row_2 = new TableRow();
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(selezioneEsclusiva);
            cell_2.ColumnSpan = 2;
            row_2.Cells.Add(cell_2);
            table.Rows.Add(row_2);

            if ((bool)Session["focus"])
            {
                Session.Add("focus", false);
            }
        }
        #endregion inserisciSelezioneEsclusiva

        #region inserisciContatore
        public void inserisciContatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaContatore = new Label();
            etichettaContatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaContatore.Font.Size = FontUnit.Point(8);
            etichettaContatore.Font.Bold = true;
            etichettaContatore.Font.Name = "Verdana";

            TextBox contatoreDa = new TextBox();
            contatoreDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreDa.Text = oggettoCustom.VALORE_DATABASE;
            contatoreDa.Width = 40;
            contatoreDa.CssClass = "comp_profilazione_anteprima";

            TextBox contatoreA = new TextBox();
            contatoreA.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreA.Text = oggettoCustom.VALORE_DATABASE;
            contatoreA.Width = 40;
            contatoreA.CssClass = "comp_profilazione_anteprima";

            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            Label etichettaContatoreDa = new Label();
            etichettaContatoreDa.Text = "&nbsp;&nbsp;da&nbsp;";
            etichettaContatoreDa.Font.Size = FontUnit.Point(8);
            etichettaContatoreDa.Font.Bold = true;
            etichettaContatoreDa.Font.Name = "Verdana";
            Label etichettaContatoreA = new Label();
            etichettaContatoreA.Text = "&nbsp;a&nbsp;";
            etichettaContatoreA.Font.Size = FontUnit.Point(8);
            etichettaContatoreA.Font.Bold = true;
            etichettaContatoreA.Font.Name = "Verdana";

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaContatore);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(etichettaContatoreDa);
            cell_2.Controls.Add(contatoreDa);
            cell_2.Controls.Add(etichettaContatoreA);
            cell_2.Controls.Add(contatoreA);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);

            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            DocsPaWR.Ruolo ruoloUtente = UserManager.getRuolo(this);
            DocsPaWR.Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "", "");
            Label etichettaDDL = new Label();
            DropDownList ddl = new DropDownList();

            switch (oggettoCustom.TIPO_CONTATORE)
            {
                case "T":
                    break;
                case "A":
                    etichettaDDL.Text = "&nbsp;AOO";
                    etichettaDDL.Font.Size = FontUnit.Point(8);
                    etichettaDDL.Font.Bold = true;
                    etichettaDDL.Font.Name = "Verdana";
                    etichettaDDL.Width = 30;
                    cell_2.Controls.Add(etichettaDDL);
                    ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                    ddl.CssClass = "comp_profilazione_anteprima";
                    //Aggiungo un elemento vuoto
                    ListItem it = new ListItem();
                    it.Value = "";
                    it.Text = "";
                    ddl.Items.Add(it);
                    //Distinguo se è un registro o un rf
                    for (int i = 0; i < registriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "0")
                        {
                            item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                            item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                            ddl.Items.Add(item);
                        }
                    }
                    ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                    ddl.Width = 100;
                    cell_2.Controls.Add(ddl);
                    break;
                case "R":
                    etichettaDDL.Text = "&nbsp;RF";
                    etichettaDDL.Font.Size = FontUnit.Point(8);
                    etichettaDDL.Font.Bold = true;
                    etichettaDDL.Font.Name = "Verdana";
                    etichettaDDL.Width = 30;
                    cell_2.Controls.Add(etichettaDDL);
                    ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                    ddl.CssClass = "comp_profilazione_anteprima";
                    //Aggiungo un elemento vuoto
                    ListItem it_1 = new ListItem();
                    it_1.Value = "";
                    it_1.Text = "";
                    ddl.Items.Add(it_1);
                    //Distinguo se è un registro o un rf
                    for (int i = 0; i < registriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "1" && ((DocsPaWR.Registro)registriRfVisibili[i]).rfDisabled == "0")
                        {
                            item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                            item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                            ddl.Items.Add(item);
                        }
                    }
                    ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                    ddl.Width = 100;
                    cell_2.Controls.Add(ddl);
                    break;
            }

            row.Cells.Add(cell_2);
            table.Rows.Add(row);

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloContatore(etichettaContatore, contatoreDa, contatoreA, etichettaContatoreDa, etichettaContatoreA, etichettaDDL, ddl, oggettoCustom, template);

            if ((bool)Session["focus"])
            {
                Session.Add("focus", false);
            }
        }
        #endregion inserisciContatore

        #region inserisciData
        public void inserisciData(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            //Per il momento questo tipo di campo è stato implementato con tre semplici textBox
            //Sarebbe opportuno creare un oggetto personalizzato, che espone le stesse funzionalità
            //della textBox, ma che mi permette di gestire la data con i tre campi separati.
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaData = new Label();
            etichettaData.Text = oggettoCustom.DESCRIZIONE;
            etichettaData.Font.Size = FontUnit.Point(8);
            etichettaData.Font.Bold = true;
            etichettaData.Font.Name = "Verdana";

            DocsPAWA.UserControls.Calendar dataDa = (DocsPAWA.UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            dataDa.fromUrl = "../UserControls/DialogCalendar.aspx";
            dataDa.CSS = "testo_grigio";
            dataDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            dataDa.VisibleTimeMode = ProfilazioneDocManager.getVisibleTimeMode(oggettoCustom);

            DocsPAWA.UserControls.Calendar dataA = (DocsPAWA.UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            dataA.fromUrl = "../UserControls/DialogCalendar.aspx";
            dataA.CSS = "testo_grigio";
            dataA.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            dataA.VisibleTimeMode = ProfilazioneDocManager.getVisibleTimeMode(oggettoCustom);

            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] date = oggettoCustom.VALORE_DATABASE.Split('@');
                    //dataDa.txt_Data.Text = date[0].ToString();
                    //dataA.txt_Data.Text = date[1].ToString();
                    dataDa.Text = date[0].ToString();
                    dataA.Text = date[1].ToString();
                }
                else
                {
                    //dataDa.txt_Data.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    //dataA.txt_Data.Text = "";
                    dataDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    dataA.Text = "";
                }
            }

            Label etichettaDataDa = new Label();
            etichettaDataDa.Text = "&nbsp;&nbsp;da&nbsp;";
            etichettaDataDa.Font.Size = FontUnit.Point(8);
            etichettaDataDa.Font.Bold = true;
            etichettaDataDa.Font.Name = "Verdana";
            Label etichettaDataA = new Label();
            etichettaDataA.Text = "&nbsp;a&nbsp;";
            etichettaDataA.Font.Size = FontUnit.Point(8);
            etichettaDataA.Font.Bold = true;
            etichettaDataA.Font.Name = "Verdana";

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaData);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(etichettaDataDa);
            cell_2.Controls.Add(dataDa);
            cell_2.Controls.Add(etichettaDataA);
            cell_2.Controls.Add(dataA);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloData(etichettaData, etichettaDataDa, etichettaDataA, dataDa, dataA, oggettoCustom, template);

        }
        #endregion inserisciData

        #region inserisciCorrispondente
        public void inserisciCorrispondente(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }

            Label etichetta = new Label();
            etichetta.Font.Size = FontUnit.Point(8);
            etichetta.Font.Bold = true;
            etichetta.Font.Name = "Verdana";
            etichetta.Text = oggettoCustom.DESCRIZIONE;

            DocsPAWA.UserControls.Corrispondente corrispondente = (DocsPAWA.UserControls.Corrispondente)this.LoadControl("../UserControls/Corrispondente.ascx");
            Session.Add("ricercaCorrispondenteStoricizzato", true);
            corrispondente.CSS_CODICE = "comp_profilazione_anteprima";
            corrispondente.CSS_DESCRIZIONE = "comp_profilazione_anteprima";
            corrispondente.DESCRIZIONE_READ_ONLY = false;
            //corrispondente.TIPO_CORRISPONDENTE = "0";
            corrispondente.TIPO_CORRISPONDENTE = oggettoCustom.TIPO_RICERCA_CORR;
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();
            corrispondente.RICERCA_AJAX = false;

            if (Session["dictionaryCorrispondente"] != null)
                dic_Corr = (Dictionary<string, Corrispondente>)Session["dictionaryCorrispondente"];

            if (dic_Corr != null && dic_Corr.ContainsKey(corrispondente.ID) && dic_Corr[corrispondente.ID] != null)
            {
                corrispondente.SYSTEM_ID_CORR = dic_Corr[corrispondente.ID].systemId;
                corrispondente.CODICE_TEXT = dic_Corr[corrispondente.ID].codiceRubrica;
                corrispondente.DESCRIZIONE_TEXT = dic_Corr[corrispondente.ID].descrizione;
                oggettoCustom.VALORE_DATABASE = dic_Corr[corrispondente.ID].systemId;
            }
            else
            {
                // caso in cui si cerca di cancellare il dato inserito
                if (dic_Corr != null && dic_Corr.ContainsKey(corrispondente.ID) && dic_Corr[corrispondente.ID] == null && Session["rubrica.campoCorrispondente"] == null)
                {
                    corrispondente.SYSTEM_ID_CORR = string.Empty;
                    corrispondente.CODICE_TEXT = string.Empty;
                    corrispondente.DESCRIZIONE_TEXT = string.Empty;
                    oggettoCustom.VALORE_DATABASE = string.Empty;
                }
                else
                {
                    //E' stato valorizzato il campo.
                    if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                    {
                		if (Char.IsNumber(oggettoCustom.VALORE_DATABASE, 0))
                		{
                        	DocsPAWA.DocsPaWR.Corrispondente corr_1 = (DocsPAWA.DocsPaWR.Corrispondente)UserManager.getCorrispondenteBySystemIDDisabled(this, oggettoCustom.VALORE_DATABASE);
                        	if (corr_1 != null)
                        	{
                            	corrispondente.SYSTEM_ID_CORR = corr_1.systemId;
                            	corrispondente.CODICE_TEXT = corr_1.codiceRubrica.ToString();
                            	corrispondente.DESCRIZIONE_TEXT = corr_1.descrizione.ToString();
                            	oggettoCustom.VALORE_DATABASE = corr_1.systemId;
                            	if (dic_Corr == null)
                            	    dic_Corr = new Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>();
                            	dic_Corr[corrispondente.ID] = corr_1;
                            	Session["dictionaryCorrispondente"] = dic_Corr;
                        	}
						}
                    }
                    
                    //E' stato selezionato un corrispondente da rubrica.
                    if (Session["rubrica.campoCorrispondente"] != null)
                    {
                        DocsPAWA.DocsPaWR.Corrispondente corr_3 = (DocsPAWA.DocsPaWR.Corrispondente)Session["rubrica.campoCorrispondente"];
                        if (corr_3 != null)
                        {
                            //Verifico che l'id del campo sia quello che mi interessa.
                            //Questo id viene messo in sessione dallo UserControl e serve a 
                            //distinguere i diversi campi corrispondete che una popup di profilazione puo' contenere
                            if (Session["rubrica.idCampoCorrispondente"] != null && Session["rubrica.idCampoCorrispondente"].ToString() == corrispondente.ID)
                            {
                                corrispondente.SYSTEM_ID_CORR = corr_3.systemId;
                                corrispondente.CODICE_TEXT = corr_3.codiceRubrica;
                                corrispondente.DESCRIZIONE_TEXT = corr_3.descrizione;
                                if (dic_Corr == null)
                                    dic_Corr = new Dictionary<string, Corrispondente>();
                                dic_Corr[corrispondente.ID] = corr_3;
                                oggettoCustom.VALORE_DATABASE = corr_3.systemId;
                                Session.Remove("rubrica.campoCorrispondente");
                                Session.Remove("rubrica.idCampoCorrispondente");
                                Session["noRicercaCodice"] = true;
                                Session["noRicercaDesc"] = true;
                                Session["dictionaryCorrispondente"] = dic_Corr;
                            }
                        }
                    }

                    //E' stato selezionato un corrispondente dalla popup dei corrispondenti multipli.
                    //if (Session["CorrSelezionatoDaMulti"] != null)
                    //{
                    //    DocsPAWA.DocsPaWR.Corrispondente corr_4 = (DocsPAWA.DocsPaWR.Corrispondente)Session["CorrSelezionatoDaMulti"];
                    //    int idCorrMulti = 0;
                    //    if (Session["idCorrMulti"] != null)
                    //        idCorrMulti = (int)Session["idCorrMulti"];

                    //    if (corr_4 != null && idCorrMulti.ToString().Equals(corrispondente.ID))
                    //    {
                    //        corrispondente.CODICE_TEXT = corr_4.codiceRubrica;
                    //        corrispondente.DESCRIZIONE_TEXT = corr_4.descrizione;
                    //        corrispondente.SYSTEM_ID_CORR = corr_4.systemId;
                    //        if (dic_Corr == null)
                    //            dic_Corr = new Dictionary<string, Corrispondente>();
                    //        dic_Corr[corrispondente.ID] = corr_4;
                    //        oggettoCustom.VALORE_DATABASE = corr_4.systemId;
                    //        Session.Remove("CorrSelezionatoDaMulti");
                    //        Session["dictionaryCorrispondente"] = dic_Corr;
                    //        Session.Remove("idCorrMulti");
                    //    }
                    //}
                }
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, corrispondente, oggettoCustom, template);

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichetta);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(corrispondente);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
        }
        #endregion inserisciCorrispondente

        #region inserisciContatoreSottocontatore
        public void inserisciContatoreSottocontatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaContatoreSottocontatore = new Label();
            etichettaContatoreSottocontatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaContatoreSottocontatore.Font.Size = FontUnit.Point(8);
            etichettaContatoreSottocontatore.Font.Bold = true;
            etichettaContatoreSottocontatore.Font.Name = "Verdana";
            etichettaContatoreSottocontatore.Height = Unit.Percentage(100);
            etichettaContatoreSottocontatore.Style.Add("vertical-align", "top");

            TextBox contatoreDa = new TextBox();
            contatoreDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreDa.Text = oggettoCustom.VALORE_DATABASE;
            contatoreDa.Width = 40;
            contatoreDa.CssClass = "comp_profilazione_anteprima";

            TextBox contatoreA = new TextBox();
            contatoreA.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreA.Text = oggettoCustom.VALORE_DATABASE;
            contatoreA.Width = 40;
            contatoreA.CssClass = "comp_profilazione_anteprima";

            TextBox sottocontatoreDa = new TextBox();
            sottocontatoreDa.ID = "da_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            sottocontatoreDa.Width = 40;
            sottocontatoreDa.CssClass = "comp_profilazione_anteprima";

            TextBox sottocontatoreA = new TextBox();
            sottocontatoreA.ID = "a_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            sottocontatoreA.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            sottocontatoreA.Width = 40;
            sottocontatoreA.CssClass = "comp_profilazione_anteprima";

            DocsPAWA.UserControls.Calendar dataSottocontatoreDa = (DocsPAWA.UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            dataSottocontatoreDa.fromUrl = "../UserControls/DialogCalendar.aspx";
            dataSottocontatoreDa.CSS = "testo_grigio";
            dataSottocontatoreDa.ID = "da_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            dataSottocontatoreDa.VisibleTimeMode = ProfilazioneDocManager.getVisibleTimeMode(oggettoCustom);

            DocsPAWA.UserControls.Calendar dataSottocontatoreA = (DocsPAWA.UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            dataSottocontatoreA.fromUrl = "../UserControls/DialogCalendar.aspx";
            dataSottocontatoreA.CSS = "testo_grigio";
            dataSottocontatoreA.ID = "a_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            dataSottocontatoreA.VisibleTimeMode = ProfilazioneDocManager.getVisibleTimeMode(oggettoCustom);

            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            if (!oggettoCustom.VALORE_SOTTOCONTATORE.Equals(""))
            {
                if (oggettoCustom.VALORE_SOTTOCONTATORE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_SOTTOCONTATORE.Split('@');
                    sottocontatoreDa.Text = contatori[0].ToString();
                    sottocontatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE.ToString();
                    sottocontatoreA.Text = "";
                }
            }

            if (!oggettoCustom.DATA_INSERIMENTO.Equals(""))
            {
                if (oggettoCustom.DATA_INSERIMENTO.IndexOf("@") != -1)
                {
                    string[] date = oggettoCustom.DATA_INSERIMENTO.Split('@');
                    dataSottocontatoreDa.Text = date[0].ToString();
                    dataSottocontatoreA.Text = date[1].ToString();
                }
                else
                {
                    dataSottocontatoreDa.Text = oggettoCustom.DATA_INSERIMENTO.ToString();
                    dataSottocontatoreA.Text = "";
                }
            }

            Label etichettaContatoreDa = new Label();
            etichettaContatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaContatoreDa.Font.Size = FontUnit.Point(8);
            etichettaContatoreDa.Font.Bold = true;
            etichettaContatoreDa.Font.Name = "Verdana";
            Label etichettaContatoreA = new Label();
            etichettaContatoreA.Text = "&nbsp;a&nbsp;";
            etichettaContatoreA.Font.Size = FontUnit.Point(8);
            etichettaContatoreA.Font.Bold = true;
            etichettaContatoreA.Font.Name = "Verdana";

            Label etichettaSottocontatoreDa = new Label();
            etichettaSottocontatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaSottocontatoreDa.Font.Size = FontUnit.Point(8);
            etichettaSottocontatoreDa.Font.Bold = true;
            etichettaSottocontatoreDa.Font.Name = "Verdana";
            Label etichettaSottocontatoreA = new Label();
            etichettaSottocontatoreA.Text = "&nbsp;a&nbsp;";
            etichettaSottocontatoreA.Font.Size = FontUnit.Point(8);
            etichettaSottocontatoreA.Font.Bold = true;
            etichettaSottocontatoreA.Font.Name = "Verdana";

            Label etichettaDataSottocontatoreDa = new Label();
            etichettaDataSottocontatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaDataSottocontatoreDa.Font.Size = FontUnit.Point(8);
            etichettaDataSottocontatoreDa.Font.Bold = true;
            etichettaDataSottocontatoreDa.Font.Name = "Verdana";
            Label etichettaDataSottocontatoreA = new Label();
            etichettaDataSottocontatoreA.Text = "&nbsp;a&nbsp;";
            etichettaDataSottocontatoreA.Font.Size = FontUnit.Point(8);
            etichettaDataSottocontatoreA.Font.Bold = true;
            etichettaDataSottocontatoreA.Font.Name = "Verdana";

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaContatoreSottocontatore);
            row.Cells.Add(cell_1);

            TableCell cell_2 = new TableCell();

            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            DocsPaWR.Ruolo ruoloUtente = UserManager.getRuolo(this);
            DocsPaWR.Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "", "");
            Label etichettaDDL = new Label();
            DropDownList ddl = new DropDownList();

            switch (oggettoCustom.TIPO_CONTATORE)
            {
                case "T":
                    break;
                case "A":
                    etichettaDDL.Text = "&nbsp;AOO";
                    etichettaDDL.Font.Size = FontUnit.Point(8);
                    etichettaDDL.Font.Bold = true;
                    etichettaDDL.Font.Name = "Verdana";
                    etichettaDDL.Width = 30;
                    cell_2.Controls.Add(etichettaDDL);
                    ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                    ddl.CssClass = "comp_profilazione_anteprima";
                    //Aggiungo un elemento vuoto
                    ListItem it = new ListItem();
                    it.Value = "";
                    it.Text = "";
                    ddl.Items.Add(it);
                    //Distinguo se è un registro o un rf
                    for (int i = 0; i < registriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "0")
                        {
                            item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                            item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                            ddl.Items.Add(item);
                        }
                    }
                    ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                    ddl.Width = 100;
                    cell_2.Controls.Add(ddl);
                    break;
                case "R":
                    etichettaDDL.Text = "&nbsp;RF";
                    etichettaDDL.Font.Size = FontUnit.Point(8);
                    etichettaDDL.Font.Bold = true;
                    etichettaDDL.Font.Name = "Verdana";
                    etichettaDDL.Width = 30;
                    cell_2.Controls.Add(etichettaDDL);
                    ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                    ddl.CssClass = "comp_profilazione_anteprima";
                    //Aggiungo un elemento vuoto
                    ListItem it_1 = new ListItem();
                    it_1.Value = "";
                    it_1.Text = "";
                    ddl.Items.Add(it_1);
                    //Distinguo se è un registro o un rf
                    for (int i = 0; i < registriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "1" && ((DocsPaWR.Registro)registriRfVisibili[i]).rfDisabled == "0")
                        {
                            item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                            item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                            ddl.Items.Add(item);
                        }
                    }
                    ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                    ddl.Width = 100;
                    cell_2.Controls.Add(ddl);
                    break;
            }

            cell_2.Controls.Add(etichettaContatoreDa);
            cell_2.Controls.Add(contatoreDa);
            cell_2.Controls.Add(etichettaContatoreA);
            cell_2.Controls.Add(contatoreA);

            cell_2.Controls.Add(etichettaSottocontatoreDa);
            cell_2.Controls.Add(sottocontatoreDa);
            cell_2.Controls.Add(etichettaSottocontatoreA);
            cell_2.Controls.Add(sottocontatoreA);

            etichettaDataSottocontatoreDa.Visible = false;
            cell_2.Controls.Add(etichettaDataSottocontatoreDa);
            dataSottocontatoreDa.Visible = false;
            cell_2.Controls.Add(dataSottocontatoreDa);
            etichettaDataSottocontatoreA.Visible = false;
            cell_2.Controls.Add(etichettaDataSottocontatoreA);
            dataSottocontatoreA.Visible = false;
            cell_2.Controls.Add(dataSottocontatoreA);

            row.Cells.Add(cell_2);
            table.Rows.Add(row);

            //Verifico i diritti del ruolo sul campo
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        etichettaContatoreSottocontatore.Visible = false;
                        etichettaContatoreDa.Visible = false;
                        contatoreDa.Visible = false;
                        etichettaContatoreA.Visible = false;
                        contatoreA.Visible = false;
                        etichettaSottocontatoreDa.Visible = false;
                        sottocontatoreDa.Visible = false;
                        etichettaSottocontatoreA.Visible = false;
                        sottocontatoreA.Visible = false;
                        etichettaDataSottocontatoreDa.Visible = false;
                        dataSottocontatoreDa.Visible = false;
                        etichettaDataSottocontatoreA.Visible = false;
                        dataSottocontatoreA.Visible = false;
                        etichettaDDL.Visible = false;
                        ddl.Visible = false;
                    }
                }
            }

            if ((bool)Session["focus"])
            {
                Session.Add("focus", false);
            }
        }
        #endregion

        #region inserisciOggettoEsterno
        public void inserisciOggettoEsterno(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichetta = new Label();
            etichetta.Font.Size = FontUnit.Point(8);
            etichetta.Font.Bold = true;
            etichetta.Font.Name = "Verdana";
            etichetta.Text = oggettoCustom.DESCRIZIONE;
            DocsPaWA.UserControls.IntegrationAdapter intAd = (DocsPaWA.UserControls.IntegrationAdapter)this.LoadControl("../UserControls/IntegrationAdapter.ascx");
            intAd.ID = oggettoCustom.SYSTEM_ID.ToString();
            intAd.View = DocsPaWA.UserControls.IntegrationAdapterView.RICERCA;
            intAd.CssClass = "testo_grigio";
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, intAd, oggettoCustom, template);
            intAd.ConfigurationValue = oggettoCustom.CONFIG_OBJ_EST;
            IntegrationAdapterValue value = new IntegrationAdapterValue(oggettoCustom.CODICE_DB, oggettoCustom.VALORE_DATABASE, oggettoCustom.MANUAL_INSERT);
            intAd.Value = value;
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, intAd, oggettoCustom, template);

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichetta);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(intAd);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
        }
        #endregion inserisciOggettoEsterno

        #region Button_ConfermaProfilazione_Click - controllaCampiObbligatori
        private void btn_ConfermaProfilazione_Click(object sender, System.EventArgs e)
        {
            int result = 0;
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                if (controllaCampi(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString()))
                {
                    result++;
                }
                if (oggettoCustom.TIPO.DESCRIZIONE_TIPO.Equals("Data"))
                {
                    try
                    {
                        UserControls.Calendar dataDa = (UserControls.Calendar)panel_Contenuto.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                        //if (dataDa.txt_Data.Text != null && dataDa.txt_Data.Text != "")
                        if (dataDa.Text != null && dataDa.Text != "")
                        {
                            //DateTime dataAppoggio = Convert.ToDateTime(dataDa.txt_Data.Text);
                            DateTime dataAppoggio = Convert.ToDateTime(dataDa.Text);
                        }
                        UserControls.Calendar dataA = (UserControls.Calendar)panel_Contenuto.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());
                        //if (dataA.txt_Data.Text != null && dataA.txt_Data.Text != "")
                        if (dataA.Text != null && dataA.Text != "")
                        {
                            //DateTime dataAppoggio = Convert.ToDateTime(dataA.txt_Data.Text);
                            DateTime dataAppoggio = Convert.ToDateTime(dataA.Text);
                        }
                    }
                    catch (Exception)
                    {
                        Label_Avviso.Text = "Inserire valori validi per il campo data !";
                        Label_Avviso.Visible = true;
                        return;
                    }
                }
            }
            if (result == template.ELENCO_OGGETTI.Length)
            {
                Label_Avviso.Text = "Inserire almeno un criterio di ricerca !";
                Label_Avviso.Visible = true;
                return;
            }

            Session.Add("templateRicerca", template);
            Session.Remove("ricercaCorrispondenteStoricizzato");
            //creaFiltroRicerca();

            //Imposto il template come filtro di ricerca
            //DocsPaWR.FiltroRicercaProfilazione filtro = new DocsPAWA.DocsPaWR.FiltroRicercaProfilazione();
            DocsPaWR.FiltroRicerca filtro = new DocsPAWA.DocsPaWR.FiltroRicerca();
            filtro.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
            filtro.template = template;
            filtro.valore = "Profilazione Dinamica";
            if (sRic != null)
                sRic.SetFiltro(DocsPAWA.DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString(), filtro);
            Session.Add(DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY, sRic);

            RegisterStartupScript("chiudi finestra", "<script language=\"javascript\">chiudiFinestra()</script>");
        }

        private bool controllaCampi(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
            //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"

            Label_Avviso.Visible = false;
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto":
                    TextBox textBox = (TextBox)panel_Contenuto.FindControl(idOggetto);
                    if (textBox.Text.Equals(""))
                    {
                        //SetFocus(textBox);
                        oggettoCustom.VALORE_DATABASE = textBox.Text;
                        return true;
                    }
                    oggettoCustom.VALORE_DATABASE = textBox.Text;
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList checkBox = (CheckBoxList)panel_Contenuto.FindControl(idOggetto);
                    if (checkBox.SelectedIndex == -1)
                    {
                        //SetFocus(checkBox);
                        for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                            oggettoCustom.VALORI_SELEZIONATI[i] = null;

                        return true;
                    }

                    oggettoCustom.VALORI_SELEZIONATI = new string[checkBox.Items.Count];
                    oggettoCustom.VALORE_DATABASE = "";
                    for (int i = 0; i < checkBox.Items.Count; i++)
                    {
                        if (checkBox.Items[i].Selected)
                        {
                            oggettoCustom.VALORI_SELEZIONATI[i] = checkBox.Items[i].Text;
                        }
                    }
                    break;
                case "MenuATendina":
                    DropDownList dropDwonList = (DropDownList)panel_Contenuto.FindControl(idOggetto);
                    if (dropDwonList.SelectedItem.Text.Equals(""))
                    {
                        //SetFocus(dropDwonList);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }
                    oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)panel_Contenuto.FindControl(idOggetto);
                    if (oggettoCustom.VALORE_DATABASE == "-1" || radioButtonList.SelectedIndex == -1 || radioButtonList.SelectedValue == "-1")
                    {
                        //SetFocus(radioButtonList);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }
                    oggettoCustom.VALORE_DATABASE = radioButtonList.SelectedItem.Text;
                    break;
                case "Data":
                    UserControls.Calendar dataDa = (UserControls.Calendar)panel_Contenuto.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                    UserControls.Calendar dataA = (UserControls.Calendar)panel_Contenuto.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());

                    if (dataDa.Text.Equals("") && dataA.Text.Equals(""))
                    {
                        //SetFocus(dataDa.txt_Data);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }

                    if (dataDa.Text.Equals("") && dataA.Text != "")
                    {
                        //SetFocus(dataDa.txt_Data);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }

                    if (dataDa.Text != "" && dataA.Text != "")
                        //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text + "@" + dataA.txt_Data.Text;
                        oggettoCustom.VALORE_DATABASE = dataDa.Text + "@" + dataA.Text;

                    if (dataDa.Text != "" && dataA.Text == "")
                        //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text;
                        oggettoCustom.VALORE_DATABASE = dataDa.Text;
                    break;
                case "Contatore":
                    TextBox contatoreDa = (TextBox)panel_Contenuto.FindControl("da_" + idOggetto);
                    TextBox contatoreA = (TextBox)panel_Contenuto.FindControl("a_" + idOggetto);
                    //Controllo la valorizzazione di campi ed eventualmente notifico gli errori
                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "T":
                            if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }
                            break;
                        case "A":
                            DropDownList ddlAoo = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }

                            if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa.Text.Equals("") && ddlRf.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }

                            if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                    }

                    if (contatoreDa.Text.Equals("") && contatoreA.Text != "")
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }

                    try
                    {
                        if (contatoreDa.Text != null && contatoreDa.Text != "")
                            Convert.ToInt32(contatoreDa.Text);
                        if (contatoreA.Text != null && contatoreA.Text != "")
                            Convert.ToInt32(contatoreA.Text);
                    }
                    catch (Exception ex)
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }


                    //I campi sono valorizzati correttamente procedo
                    if (contatoreDa.Text != "" && contatoreA.Text != "")
                        oggettoCustom.VALORE_DATABASE = contatoreDa.Text + "@" + contatoreA.Text;

                    if (contatoreDa.Text != "" && contatoreA.Text == "")
                        oggettoCustom.VALORE_DATABASE = contatoreDa.Text;

                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "A":
                            DropDownList ddlAoo = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                            break;
                    }
                    break;
                case "Corrispondente":
                    UserControls.Corrispondente corr = (UserControls.Corrispondente)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    DocsPaWR.Corrispondente corrispondente = new DocsPaWR.Corrispondente();
                    if (Session["dictionaryCorrispondente"] != null)
                    {
                        dic_Corr = (Dictionary<string, Corrispondente>)Session["dictionaryCorrispondente"];
                        if (dic_Corr != null && dic_Corr.ContainsKey(oggettoCustom.SYSTEM_ID.ToString()) && dic_Corr[oggettoCustom.SYSTEM_ID.ToString()] != null)
                            oggettoCustom.VALORE_DATABASE = dic_Corr[oggettoCustom.SYSTEM_ID.ToString()].systemId;
                    }
                    else
                    {
                        //if (Session["CorrSelezionatoDaMulti"] != null)
                        //{
                        //    DocsPaWR.Corrispondente corr1 = (DocsPaWR.Corrispondente)Session["CorrSelezionatoDaMulti"];
                        //    oggettoCustom.VALORE_DATABASE = corr1.systemId;
                        //}
                        //else
                        //{
                            if (Session["rubrica.campoCorrispondente"] != null)
                            {
                                DocsPAWA.DocsPaWR.Corrispondente corr_3 = (DocsPAWA.DocsPaWR.Corrispondente)Session["rubrica.campoCorrispondente"];
                                oggettoCustom.VALORE_DATABASE = corr_3.systemId;
                            }
                            else
                            {
                                // 1 - Ambedue i campi del corrispondente non sono valorizzati
                                if (corr.CODICE_TEXT == "" && corr.DESCRIZIONE_TEXT == "")
                                {
                                    oggettoCustom.VALORE_DATABASE = "";
                                    return true;
                                }
                                // 2 - E' stato valorizzato solo il campo descrizione del corrispondente
                                if (corr.CODICE_TEXT == "" && corr.DESCRIZIONE_TEXT != "")
                                {
                                    oggettoCustom.VALORE_DATABASE = corr.DESCRIZIONE_TEXT;
                                }
                                // 3 - E' valorizzato il campo codice del corrispondente
                                if (corr.CODICE_TEXT != "")
                                {
                                    //Cerco il corrispondente
                                    if (!string.IsNullOrEmpty(corr.SYSTEM_ID_CORR))
                                        corrispondente = UserManager.getCorrispondenteBySystemIDDisabled(this, corr.SYSTEM_ID_CORR);
                                    else
                                        corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);

                                    //corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);
                                    // 3.1 - Corrispondente trovato per codice
                                    if (corrispondente != null)
                                    {
                                        oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                                    }
                                    // 3.2 - Corrispondente non trovato per codice
                                    else
                                    {
                                        // 3.2.1 - Campo descrizione non valorizzato
                                        if (corr.DESCRIZIONE_TEXT == "")
                                        {
                                            oggettoCustom.VALORE_DATABASE = "";
                                            return true;
                                        }
                                        // 3.2.2 - Campo descrizione valorizzato
                                        else
                                            oggettoCustom.VALORE_DATABASE = corr.DESCRIZIONE_TEXT;
                                    }
                                }
                            }
                        //}
                    }
                break;
                case "ContatoreSottocontatore":
                    TextBox contatoreSDa = (TextBox)panel_Contenuto.FindControl("da_" + idOggetto);
                    TextBox contatoreSA = (TextBox)panel_Contenuto.FindControl("a_" + idOggetto);
                    TextBox sottocontatoreDa = (TextBox)panel_Contenuto.FindControl("da_sottocontatore_" + idOggetto);
                    TextBox sottocontatoreA = (TextBox)panel_Contenuto.FindControl("a_sottocontatore_" + idOggetto);
                    UserControls.Calendar dataSottocontatoreDa = (UserControls.Calendar)panel_Contenuto.FindControl("da_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());
                    UserControls.Calendar dataSottocontatoreA = (UserControls.Calendar)panel_Contenuto.FindControl("a_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());

                    //Controllo la valorizzazione di campi ed eventualmente notifico gli errori
                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "T":
                            if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals("") &&
                                sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals("") &&
                                dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals("")
                                )
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                oggettoCustom.VALORE_SOTTOCONTATORE = "";
                                oggettoCustom.DATA_INSERIMENTO = "";
                                return true;
                            }
                            break;
                        case "A":
                            DropDownList ddlAoo = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreSDa.Text.Equals("") && sottocontatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                oggettoCustom.VALORE_SOTTOCONTATORE = "";
                                return true;
                            }

                            if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";

                            if (sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_SOTTOCONTATORE = "";

                            if (dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals(""))
                                oggettoCustom.DATA_INSERIMENTO = "";
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreSDa.Text.Equals("") && sottocontatoreDa.Text.Equals("") && ddlRf.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                oggettoCustom.VALORE_SOTTOCONTATORE = "";
                                return true;
                            }

                            if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";

                            if (sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_SOTTOCONTATORE = "";

                            if (dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals(""))
                                oggettoCustom.DATA_INSERIMENTO = "";
                            break;
                    }

                    if (contatoreSDa.Text.Equals("") && contatoreSA.Text.Equals("") &&
                        sottocontatoreDa.Text.Equals("") && sottocontatoreA.Text.Equals("") &&
                        dataSottocontatoreDa.Text.Equals("") && dataSottocontatoreA.Text.Equals("")
                        )
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        oggettoCustom.VALORE_SOTTOCONTATORE = "";
                        oggettoCustom.DATA_INSERIMENTO = "";
                        return true;
                    }

                    try
                    {
                        if (contatoreSDa.Text != null && contatoreSDa.Text != "")
                            Convert.ToInt32(contatoreSDa.Text);
                        if (contatoreSA.Text != null && contatoreSA.Text != "")
                            Convert.ToInt32(contatoreSA.Text);
                        if (sottocontatoreDa.Text != null && sottocontatoreDa.Text != "")
                            Convert.ToInt32(sottocontatoreDa.Text);
                        if (sottocontatoreA.Text != null && sottocontatoreA.Text != "")
                            Convert.ToInt32(sottocontatoreA.Text);
                    }
                    catch (Exception ex)
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        oggettoCustom.VALORE_SOTTOCONTATORE = "";
                        return true;
                    }

                    //I campi sono valorizzati correttamente procedo
                    if (contatoreSDa.Text != "" && contatoreSA.Text != "")
                        oggettoCustom.VALORE_DATABASE = contatoreSDa.Text + "@" + contatoreSA.Text;

                    if (contatoreSDa.Text != "" && contatoreSA.Text == "")
                        oggettoCustom.VALORE_DATABASE = contatoreSDa.Text;

                    if (sottocontatoreDa.Text != "" && sottocontatoreA.Text != "")
                        oggettoCustom.VALORE_SOTTOCONTATORE = sottocontatoreDa.Text + "@" + sottocontatoreA.Text;

                    if (sottocontatoreDa.Text != "" && sottocontatoreA.Text == "")
                        oggettoCustom.VALORE_SOTTOCONTATORE = sottocontatoreDa.Text;

                    if (dataSottocontatoreDa.Text != "" && dataSottocontatoreA.Text != "")
                        oggettoCustom.DATA_INSERIMENTO = dataSottocontatoreDa.Text + "@" + dataSottocontatoreA.Text;

                    if (dataSottocontatoreDa.Text != "" && dataSottocontatoreA.Text == "")
                        oggettoCustom.DATA_INSERIMENTO = dataSottocontatoreDa.Text;

                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "A":
                            DropDownList ddlAoo = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                            break;
                    }
                    break;

                case "OggettoEsterno":
                    IntegrationAdapter intAd = (IntegrationAdapter)panel_Contenuto.FindControl(idOggetto);
                    IntegrationAdapterValue value = intAd.Value;
                    if (value == null)
                    {
                        //SetFocus(radioButtonList);
                        oggettoCustom.VALORE_DATABASE = "";
                        oggettoCustom.CODICE_DB = "";
                        return true;
                    }
                    oggettoCustom.VALORE_DATABASE = value.Descrizione;
                    oggettoCustom.CODICE_DB = value.Codice;
                    break;
            }
            return false;
        }
        #endregion Button_ConfermaProfilazione_Click - controllaCampiObbligatori

        #region SetFocus
        private void SetFocus(System.Web.UI.Control ctrl)
        {
            //string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
            //RegisterStartupScript("focus", s);
            ClientScript.RegisterStartupScript(this.GetType(), "scriptFocus", "try{document.getElementById('" + ctrl.ID + "').focus();}catch(e){}", true);
        }
        #endregion SetFocus

        #region btn_resettaPopUp - resettaCampo - chiudi
        private void btn_resettaPopUp_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                resettaCampo(oggettoCustom);
            }
            Session.Remove("templateRicerca");
            Session.Remove("rubrica.campoCorrispondente");
            Session.Remove("CorrSelezionatoDaMulti");
            Session.Remove("dictionaryCorrispondente");

            if (sRic != null)
                sRic.RimuoviFiltro(DocsPAWA.DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString());

            RegisterStartupScript("chiudi finestra", "<script language=\"javascript\">chiudiFinestra()</script>");
        }

        private void resettaCampo(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "Corrispondente":
                    oggettoCustom.VALORE_DATABASE = string.Empty;
                    break;
                case "CampoDiTesto":
                    TextBox textBox = (TextBox)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    oggettoCustom.VALORE_DATABASE = string.Empty;
                    textBox.Text = "";
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList checkBox = (CheckBoxList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    oggettoCustom.VALORE_DATABASE = string.Empty;
                    oggettoCustom.VALORI_SELEZIONATI = null;
                    checkBox.SelectedIndex = -1;
                    break;
                case "MenuATendina":
                    DropDownList dropDwonList = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    oggettoCustom.VALORE_DATABASE = string.Empty;
                    dropDwonList.SelectedIndex = -1;
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    oggettoCustom.VALORE_DATABASE = string.Empty;
                    radioButtonList.SelectedIndex = -1;
                    break;
                case "Data":
                    UserControls.Calendar dataDa = (UserControls.Calendar)panel_Contenuto.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                    UserControls.Calendar dataA = (UserControls.Calendar)panel_Contenuto.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());
                    //dataDa.txt_Data.Text = "";
                    //dataA.txt_Data.Text = "";
                    dataDa.Text = "";
                    dataA.Text = "";
                    oggettoCustom.VALORE_DATABASE = string.Empty;

                    //TextBox giorno	= (TextBox) panel_Contenuto.FindControl("giorno"+oggettoCustom.SYSTEM_ID.ToString());
                    //TextBox mese	= (TextBox) panel_Contenuto.FindControl("mese"+oggettoCustom.SYSTEM_ID.ToString());
                    //TextBox anno	= (TextBox) panel_Contenuto.FindControl("anno"+oggettoCustom.SYSTEM_ID.ToString());
                    //giorno.Text = "";
                    //mese.Text	= "";
                    //anno.Text	= "";
                    break;
                case "Contatore":
                    TextBox contatoreDa = (TextBox)panel_Contenuto.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                    TextBox contatoreA = (TextBox)panel_Contenuto.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());
                    contatoreDa.Text = "";
                    contatoreA.Text = "";
                    oggettoCustom.VALORE_DATABASE = string.Empty;
                    //TextBox contatore = (TextBox) panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    //contatore.Text = "";					
                    break;
                case "ContatoreSottocontatore":
                    TextBox contatoreSDa = (TextBox)panel_Contenuto.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                    TextBox contatoreSA = (TextBox)panel_Contenuto.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());
                    TextBox sottocontatoreDa = (TextBox)panel_Contenuto.FindControl("da_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());
                    TextBox sottocontatoreA = (TextBox)panel_Contenuto.FindControl("a_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());
                    UserControls.Calendar dataSottocontatoreDa = (UserControls.Calendar)panel_Contenuto.FindControl("da_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());
                    UserControls.Calendar dataSottocontatoreA = (UserControls.Calendar)panel_Contenuto.FindControl("a_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString());
                    contatoreSDa.Text = "";
                    contatoreSA.Text = "";
                    sottocontatoreDa.Text = "";
                    sottocontatoreA.Text = "";
                    dataSottocontatoreDa.Text = "";
                    dataSottocontatoreA.Text = "";
                    oggettoCustom.VALORE_DATABASE = string.Empty;
                    //TextBox contatore = (TextBox) panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    //contatore.Text = "";					
                    break;
                case "OggettoEsterno":
                    IntegrationAdapterValue nullValue = new IntegrationAdapterValue("", "", false);
                    ((IntegrationAdapter)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString())).Value = nullValue;
                    break;
                
                default:
                    oggettoCustom.VALORE_DATABASE = string.Empty;
                    break;
            }
        }

        private void btn_Chiudi_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                resettaCampo(oggettoCustom);
            }
            Session.Remove("templateRicerca");
            Session.Remove("focus");
            Session.Remove("ricercaCorrispondenteStoricizzato");
            Session.Remove("rubrica.corrispondenteSelezionato");
            Session.Remove("rubrica.campoCorrispondente");
            Session.Remove("dictionaryCorrispondente");
            Session.Remove("rubrica.idCampoCorrispondente");
            RegisterStartupScript("chiudi finestra", "<script language=\"javascript\">chiudiFinestra()</script>");
        }
        #endregion btn_resettaPopUp - resettaCampo - chiudi

        #region cancella_selezioneEsclusiva_Click
        private void cancella_selezioneEsclusiva_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string idOggetto = (((ImageButton)sender).ID).Substring(1);
            ((RadioButtonList)panel_Contenuto.FindControl(idOggetto)).SelectedIndex = -1;
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                if (((DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i]).SYSTEM_ID.ToString() == idOggetto)
                {
                    ((DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i]).VALORE_DATABASE = "";
                    Session.Add("templateRicerca", template);
                }
            }
        }
        #endregion cancella_selezioneEsclusiva_Click

        #region impostaValori
        private int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        private int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
        {
            for (int i = 0; i < rbl.Items.Count; i++)
            {
                if (rbl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        private void impostaSelezioneCaselleDiSelezione(DocsPAWA.DocsPaWR.OggettoCustom objCustom, CheckBoxList cbl)
        {
            for (int i = 0; i < objCustom.VALORI_SELEZIONATI.Length; i++)
            {
                for (int j = 0; j < cbl.Items.Count; j++)
                {
                    if ((string)objCustom.VALORI_SELEZIONATI[i] == cbl.Items[j].Text)
                    {
                        cbl.Items[j].Selected = true;
                    }
                }
            }
        }

        private void impostaSelezioneCaselleDiSelezione(string valore, CheckBoxList cbl)
        {
            string[] caselleSelezionate = valore.Split('-');
            for (int i = 0; i < caselleSelezionate.Length; i++)
            {
                if (!caselleSelezionate[i].Equals(""))
                {
                    for (int j = 0; j < cbl.Items.Count; j++)
                    {
                        if (caselleSelezionate[i] == cbl.Items[j].Text)
                        {
                            cbl.Items[j].Selected = true;
                        }
                    }
                }
            }
        }
        #endregion

        #region creaFiltroRicerca()
        public void creaFiltroRicerca()
        {
            //QUERY DI RICERCA IN "AND"
            DocsPaWR.FiltroRicerca filtro = new DocsPAWA.DocsPaWR.FiltroRicerca();
            filtro.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString();
            string querySelect = string.Empty;
            string queryCorpoCentrale = string.Empty;
            string querySerieAnd = string.Empty;
            ArrayList indiciDpa = new ArrayList();

            querySelect = creaCorpoSelect(template, ref indiciDpa);

            //Carattere speciale che serve per il salvataggio delle ricerche
            querySelect += " # ";

            queryCorpoCentrale = creaCorpoCentrale(indiciDpa);
            querySerieAnd = creaSerieAnd(indiciDpa, template);

            //Carattere speciale che serve per il salvataggio delle ricerche
            queryCorpoCentrale += " # ";

            filtro.valore = querySelect + " " + queryCorpoCentrale + " " + querySerieAnd;

            if (sRic != null)
                sRic.SetFiltro(DocsPAWA.DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString(), filtro);
            Session.Add(DocsPAWA.ricercaDoc.SchedaRicerca.SESSION_KEY, sRic);
        }

        public string creaCorpoSelect(DocsPAWA.DocsPaWR.Templates template, ref ArrayList indiciDpa)
        {
            string select = string.Empty;
            int numOggetti = 0;
            int indiceDpa = 0;
            bool flag = true;

            while (flag)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[numOggetti];
                oggettoCustom.VALORE_DATABASE = oggettoCustom.VALORE_DATABASE.Replace("'", "''");
                string tipoOggetto = oggettoCustom.TIPO.DESCRIZIONE_TIPO;

                switch (tipoOggetto)
                {
                    case "CasellaDiSelezione":
                        for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                        {
                            if (oggettoCustom.VALORI_SELEZIONATI[i] != null && (string)oggettoCustom.VALORI_SELEZIONATI[i] != "")
                            {
                                select += " (SELECT Doc_Number, Valore_Oggetto_Db,ID_OGGETTO,ID_AOO_RF FROM DPA_ASSOCIAZIONE_TEMPLATES) DPA" + indiceDpa + ",";
                                indiciDpa.Add(indiceDpa);
                                indiceDpa++;
                            }
                        }
                        numOggetti++;
                        break;

                    case "Contatore":
                        switch (oggettoCustom.TIPO_CONTATORE)
                        {
                            case "T":
                                if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                                {
                                    select += " (SELECT Doc_Number, Valore_Oggetto_Db,ID_OGGETTO,ID_AOO_RF FROM DPA_ASSOCIAZIONE_TEMPLATES) DPA" + indiceDpa + ",";
                                    indiciDpa.Add(indiceDpa);
                                    indiceDpa++;
                                }
                                numOggetti++;
                                break;
                            //O è di tipo "A" o di tipo "R"
                            default:
                                if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                {
                                    if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                                    {
                                        //Nr.Contatore SI - Aoo/Rf SI
                                        select += " (SELECT Doc_Number, Valore_Oggetto_Db,ID_OGGETTO,ID_AOO_RF FROM DPA_ASSOCIAZIONE_TEMPLATES) DPA" + indiceDpa + ",";
                                        indiciDpa.Add(indiceDpa);
                                        indiceDpa++;
                                    }
                                    else
                                    {
                                        //Nr.Contatore NO - Aoo/Rf SI
                                        select += " (SELECT Doc_Number, Valore_Oggetto_Db,ID_OGGETTO,ID_AOO_RF FROM DPA_ASSOCIAZIONE_TEMPLATES) DPA" + indiceDpa + ",";
                                        indiciDpa.Add(indiceDpa);
                                        indiceDpa++;
                                    }
                                }
                                else
                                {
                                    if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                                    {
                                        //Nr.Contatore SI - Aoo/Rf NO
                                        select += " (SELECT Doc_Number, Valore_Oggetto_Db,ID_OGGETTO,ID_AOO_RF FROM DPA_ASSOCIAZIONE_TEMPLATES) DPA" + indiceDpa + ",";
                                        indiciDpa.Add(indiceDpa);
                                        indiceDpa++;
                                    }
                                }
                                numOggetti++;
                                break;
                        }
                        break;

                    default:
                        if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                        {
                            select += " (SELECT Doc_Number, Valore_Oggetto_Db,ID_OGGETTO,ID_AOO_RF FROM DPA_ASSOCIAZIONE_TEMPLATES) DPA" + indiceDpa + ",";
                            indiciDpa.Add(indiceDpa);
                            indiceDpa++;
                        }
                        numOggetti++;
                        break;
                }

                //Verifico condizione di uscita
                if (numOggetti == template.ELENCO_OGGETTI.Length)
                    flag = false;
            }
            if (select != "")
                select = select.Substring(0, select.Length - 1);
            return select;
        }

        public string creaCorpoCentrale(ArrayList indiciDpa)
        {
            string corpoCentrale = string.Empty;
            corpoCentrale += " AND DPA_OGGETTI_CUSTOM.Campo_Di_Ricerca = 'SI'";

            if (indiciDpa.Count - 1 == 0)
            {
                corpoCentrale += " AND DPA0.ID_OGGETTO = DPA_OGGETTI_CUSTOM.SYSTEM_ID ";
                corpoCentrale += " AND A.DOCNUMBER = DPA0.Doc_Number ";
                return corpoCentrale;
            }

            for (int i = 0; i < indiciDpa.Count - 1; i++)
            {
                if (i == 0)
                {
                    corpoCentrale += " AND DPA" + indiciDpa[i] + ".ID_OGGETTO = DPA_OGGETTI_CUSTOM.SYSTEM_ID ";
                    corpoCentrale += " AND A.DOCNUMBER = DPA" + indiciDpa[i] + ".Doc_Number ";
                    corpoCentrale += " AND DPA" + indiciDpa[i] + ".Doc_Number=DPA" + indiciDpa[i + 1] + ".Doc_Number";
                }
                else
                {
                    corpoCentrale += " AND DPA" + indiciDpa[i] + ".Doc_Number=DPA" + indiciDpa[i + 1] + ".Doc_Number";
                }
            }
            return corpoCentrale;
        }

        public string creaSerieAnd(ArrayList indiciDpa, DocsPAWA.DocsPaWR.Templates template)
        {
            string serieAnd = string.Empty;
            int numOggetti = 0;
            int indiceDpa = 0;
            bool flag = true;

            while (flag)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[numOggetti];
                oggettoCustom.VALORE_DATABASE = oggettoCustom.VALORE_DATABASE.Replace("'", "''");
                string tipoOggetto = oggettoCustom.TIPO.DESCRIZIONE_TIPO;

                switch (tipoOggetto)
                {
                    case "Corrispondente":
                        if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                        {
                            int codiceCorrispondente;
                            if (Int32.TryParse(oggettoCustom.VALORE_DATABASE, out codiceCorrispondente))
                            {
                                serieAnd += " AND DPA" + indiciDpa[indiceDpa] + ".Valore_Oggetto_Db in ( (select SYSTEM_ID from DPA_CORR_GLOBALI where SYSTEM_ID = " + codiceCorrispondente + ") ) AND DPA" + indiciDpa[indiceDpa] + ".ID_OGGETTO = " + oggettoCustom.SYSTEM_ID;
                                indiceDpa++;
                            }
                            else
                            {
                                serieAnd += " AND DPA" + indiciDpa[indiceDpa] + ".Valore_Oggetto_Db in ( (select SYSTEM_ID from DPA_CORR_GLOBALI where UPPER(VAR_DESC_CORR) like UPPER('%" + oggettoCustom.VALORE_DATABASE + "%')) ) AND DPA" + indiciDpa[indiceDpa] + ".ID_OGGETTO = " + oggettoCustom.SYSTEM_ID;
                                indiceDpa++;
                            }
                        }
                        numOggetti++;
                        break;
                    case "CasellaDiSelezione":
                        for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                        {
                            if (oggettoCustom.VALORI_SELEZIONATI[i] != null && (string)oggettoCustom.VALORI_SELEZIONATI[i] != "")
                            {
                                serieAnd += " AND DPA" + indiciDpa[indiceDpa] + ".Valore_Oggetto_Db = '" + oggettoCustom.VALORI_SELEZIONATI[i] + "' AND DPA" + indiciDpa[indiceDpa] + ".ID_OGGETTO = " + oggettoCustom.SYSTEM_ID;
                                indiceDpa++;
                            }
                            else
                            {
                                serieAnd += " AND (DPA" + indiciDpa[indiceDpa] + ".Valore_Oggetto_Db is null OR DPA" + indiciDpa[indiceDpa] + ".Valore_Oggetto_Db = '') " +
                                " AND DPA" + indiciDpa[indiceDpa] + ".ID_OGGETTO = " + oggettoCustom.SYSTEM_ID;
                                indiceDpa++;
                            }
                        }
                        numOggetti++;
                        break;
                    case "CampoDiTesto":
                        if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                        {
                            serieAnd += " AND DPA" + indiciDpa[indiceDpa] + ".Valore_Oggetto_Db like '%" + oggettoCustom.VALORE_DATABASE + "%' AND DPA" + indiciDpa[indiceDpa] + ".ID_OGGETTO = " + oggettoCustom.SYSTEM_ID;
                            indiceDpa++;
                        }
                        numOggetti++;
                        break;

                    case "Contatore":
                        switch (oggettoCustom.TIPO_CONTATORE)
                        {
                            case "T":
                                if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                                {
                                    serieAnd += " AND DPA" + indiciDpa[indiceDpa] + ".Valore_Oggetto_Db = '" + oggettoCustom.VALORE_DATABASE + "' AND DPA" + indiciDpa[indiceDpa] + ".ID_OGGETTO = " + oggettoCustom.SYSTEM_ID;
                                    indiceDpa++;
                                }
                                numOggetti++;
                                break;
                            //O è di tipo "A" o di tipo "R"
                            default:
                                if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "")
                                {
                                    if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                                    {
                                        //Nr.Contatore SI - Aoo/Rf SI
                                        serieAnd += " AND DPA" + indiciDpa[indiceDpa] + ".Valore_Oggetto_Db = '" + oggettoCustom.VALORE_DATABASE + "' AND DPA" + indiciDpa[indiceDpa] + ".ID_OGGETTO = " + oggettoCustom.SYSTEM_ID + "AND DPA" + indiciDpa[indiceDpa] + ".ID_AOO_RF = " + oggettoCustom.ID_AOO_RF;
                                        indiceDpa++;
                                    }
                                    else
                                    {
                                        //Nr.Contatore NO - Aoo/Rf SI
                                        serieAnd += " AND DPA" + indiciDpa[indiceDpa] + ".ID_OGGETTO = " + oggettoCustom.SYSTEM_ID + "AND DPA" + indiciDpa[indiceDpa] + ".ID_AOO_RF = " + oggettoCustom.ID_AOO_RF;
                                        indiceDpa++;
                                    }
                                }
                                else
                                {
                                    if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                                    {
                                        //Nr.Contatore SI - Aoo/Rf NO
                                        serieAnd += " AND DPA" + indiciDpa[indiceDpa] + ".Valore_Oggetto_Db = '" + oggettoCustom.VALORE_DATABASE + "' AND DPA" + indiciDpa[indiceDpa] + ".ID_OGGETTO = " + oggettoCustom.SYSTEM_ID;
                                        indiceDpa++;
                                    }
                                }
                                numOggetti++;
                                break;
                        }
                        break;

                    default:
                        if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                        {
                            serieAnd += " AND DPA" + indiciDpa[indiceDpa] + ".Valore_Oggetto_Db = '" + oggettoCustom.VALORE_DATABASE + "' AND DPA" + indiciDpa[indiceDpa] + ".ID_OGGETTO = " + oggettoCustom.SYSTEM_ID;
                            indiceDpa++;
                        }
                        numOggetti++;
                        break;
                }

                //Verifico condizione di uscita
                if (numOggetti == template.ELENCO_OGGETTI.Length)
                    flag = false;
            }
            return serieAnd;
        }
        #endregion creaFiltroRicerca()

        #region imposta diritti ruolo sul campo
        public void impostaDirittiRuoloSulCampo(Object etichetta, Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.TextBox)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "CasellaDiSelezione":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "MenuATendina":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.DropDownList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "SelezioneEsclusiva":
                            //Per la selezione esclusiva è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Contatore":
                            //Per il contatore è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Data":
                            //Per la data è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Corrispondente":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPAWA.UserControls.Corrispondente)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "OggettoEsterno":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPaWA.UserControls.IntegrationAdapter)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                    }
                }
            }
        }

        public void impostaDirittiRuoloSelezioneEsclusiva(Object etichetta, Object campo, Object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Visible = false;
                        ((System.Web.UI.WebControls.ImageButton)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                }
            }
        }

        public void impostaDirittiRuoloContatore(Object etichettaContatore, Object contatoreDa, Object contatoreA, Object etichettaContatoreDa, Object etichettaContatoreA, Object etichettaDDL, Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatore).Visible = false;

                        ((System.Web.UI.WebControls.Label)etichettaContatoreDa).Visible = false;
                        ((System.Web.UI.WebControls.TextBox)contatoreDa).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaContatoreA).Visible = false;
                        ((System.Web.UI.WebControls.TextBox)contatoreA).Visible = false;

                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;
                    }
                }
            }
        }

        public void impostaDirittiRuoloData(Object etichettaData, Object etichettaDataDa, Object etichettaDataA, Object dataDa, Object dataA, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneDocManager.getDirittiCampoTipologiaDoc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaData).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaDataDa).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaDataA).Visible = false;


                        ((DocsPAWA.UserControls.Calendar)dataDa).Visible = false;
                        ((DocsPAWA.UserControls.Calendar)dataDa).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                        ((DocsPAWA.UserControls.Calendar)dataDa).btn_Cal.Visible = false;
                        ((DocsPAWA.UserControls.Calendar)dataA).Visible = false;
                        ((DocsPAWA.UserControls.Calendar)dataA).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                        ((DocsPAWA.UserControls.Calendar)dataA).btn_Cal.Visible = false;
                    }
                }
            }
        }
        #endregion
    }
}
