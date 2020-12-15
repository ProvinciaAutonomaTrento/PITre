using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace DocsPAWA.RicercaCampiComuni
{
    public partial class RicercaCampiComuni : DocsPAWA.CssPage
    {
        protected DocsPAWA.DocsPaWR.Templates template_CC_Doc;
        protected DocsPAWA.DocsPaWR.Templates template_CC_Fasc;

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);
            if (!IsPostBack && !SiteNavigation.CallContextStack.CurrentContext.IsBack)
            {
                SiteNavigation.CallContextStack.CurrentContext.ContextState.Remove("template_CC_Doc");
                SiteNavigation.CallContextStack.CurrentContext.ContextState.Remove("template_CC_Fasc");

                //Recupero il template Campi Comuni Documenti
                ArrayList templatesDocumenti = ProfilazioneDocManager.getTemplates(UserManager.getInfoUtente(this).idAmministrazione, this);
                foreach (DocsPAWA.DocsPaWR.Templates templateDoc in templatesDocumenti)
                {
                    if (templateDoc.IPER_FASC_DOC == "1")
                    {
                        template_CC_Doc = ProfilazioneDocManager.getTemplateById(templateDoc.SYSTEM_ID.ToString(), this);
                        SiteNavigation.CallContextStack.CurrentContext.ContextState.Add("template_CC_Doc", template_CC_Doc);
                        inserisciComponenti(template_CC_Doc, "PNL_DOC");
                    }
                }

                //Recupero il template Campi Comuni Fascicoli
                ArrayList templatesFascicoli = ProfilazioneFascManager.getTemplatesFasc(UserManager.getInfoUtente(this).idAmministrazione, this);
                foreach (DocsPAWA.DocsPaWR.Templates templateFasc in templatesFascicoli)
                {
                    if (templateFasc.IPER_FASC_DOC == "1")
                    {
                        template_CC_Fasc = ProfilazioneFascManager.getTemplateFascById(templateFasc.SYSTEM_ID.ToString(), this);
                        SiteNavigation.CallContextStack.CurrentContext.ContextState.Add("template_CC_Fasc", template_CC_Fasc);
                        inserisciComponenti(template_CC_Fasc, "PNL_FASC");
                    }
                }
            }
            else
            {
                if (SiteNavigation.CallContextStack.CurrentContext.ContextState["template_CC_Doc"] != null && SiteNavigation.CallContextStack.CurrentContext.ContextState["template_CC_Fasc"] != null)
                {
                    template_CC_Doc = ((DocsPAWA.DocsPaWR.Templates)SiteNavigation.CallContextStack.CurrentContext.ContextState["template_CC_Doc"]);
                    inserisciComponenti(((DocsPAWA.DocsPaWR.Templates)SiteNavigation.CallContextStack.CurrentContext.ContextState["template_CC_Doc"]), "PNL_DOC");
                    template_CC_Fasc = ((DocsPAWA.DocsPaWR.Templates)SiteNavigation.CallContextStack.CurrentContext.ContextState["template_CC_Fasc"]);
                    inserisciComponenti(((DocsPAWA.DocsPaWR.Templates)SiteNavigation.CallContextStack.CurrentContext.ContextState["template_CC_Fasc"]), "PNL_FASC");
                }
            }
        }

        public void inserisciComponenti(DocsPAWA.DocsPaWR.Templates template, string panel)
        {
            Table tableDoc = new Table();
            tableDoc.Width = Unit.Percentage(100);
            tableDoc.ID = "tableDoc";
            Table tableFasc = new Table();
            tableFasc.Width = Unit.Percentage(100);
            tableFasc.ID = "tableFasc";
            switch(panel)
            {
                case "PNL_DOC":
                    for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                    {
                        DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                        switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "CampoDiTesto":
                                inserisciCampoDiTesto(oggettoCustom, tableDoc);
                                break;
                            case "CasellaDiSelezione":
                                inserisciCasellaDiSelezione(oggettoCustom, tableDoc);
                                break;
                            case "MenuATendina":
                                inserisciMenuATendina(oggettoCustom, tableDoc);
                                break;
                            case "SelezioneEsclusiva":
                                inserisciSelezioneEsclusiva(oggettoCustom, tableDoc);
                                break;
                            case "Contatore":
                                inserisciContatore(oggettoCustom, tableDoc);
                                break;
                            case "Data":
                                inserisciData(oggettoCustom, tableDoc);
                                break;
                            case "Corrispondente":
                                inserisciCorrispondente(oggettoCustom, tableDoc);
                                break;
                        }
                    }

                    for (int i = 0; i < tableDoc.Rows.Count; i++)
                    {
                        if (tableDoc.Rows[i].Cells.Count == 2)
                        {
                            tableDoc.Rows[i].Cells[0].Width = Unit.Percentage(35);
                            tableDoc.Rows[i].Cells[1].Width = Unit.Percentage(65);
                        }
                        if (tableDoc.Rows[i].Cells.Count == 1)
                            tableDoc.Rows[i].Cells[0].Width = Unit.Percentage(100);
                    }
                    panel_ContenutoCampiDoc.Controls.Add(tableDoc);
                    break;

                case "PNL_FASC":
                    for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                    {
                        DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                        switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "CampoDiTesto":
                                inserisciCampoDiTesto(oggettoCustom, tableFasc);
                                break;
                            case "CasellaDiSelezione":
                                inserisciCasellaDiSelezione(oggettoCustom, tableFasc);
                                break;
                            case "MenuATendina":
                                inserisciMenuATendina(oggettoCustom, tableFasc);
                                break;
                            case "SelezioneEsclusiva":
                                inserisciSelezioneEsclusiva(oggettoCustom, tableFasc);
                                break;
                            case "Contatore":
                                inserisciContatore(oggettoCustom, tableFasc);
                                break;
                            case "Data":
                                inserisciData(oggettoCustom, tableFasc);
                                break;
                            case "Corrispondente":
                                inserisciCorrispondente(oggettoCustom, tableFasc);
                                break;
                        }
                    }

                    for (int i = 0; i < tableFasc.Rows.Count; i++)
                    {
                        if (tableFasc.Rows[i].Cells.Count == 2)
                        {
                            tableFasc.Rows[i].Cells[0].Width = Unit.Percentage(35);
                            tableFasc.Rows[i].Cells[1].Width = Unit.Percentage(65);
                        }
                        if (tableFasc.Rows[i].Cells.Count == 1)
                            tableFasc.Rows[i].Cells[0].Width = Unit.Percentage(100);
                    }
                    panel_ContenutoCampiFasc.Controls.Add(tableFasc);
                    break;
            }
            
        }

        #region inserisciCampoDiTesto
        public void inserisciCampoDiTesto(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, Table table)
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
        }
        #endregion inserisciCampoDiTesto

        #region inserisciCasellaDiSelezione
        public void inserisciCasellaDiSelezione(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, Table table)
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
        }
        #endregion inserisciCasellaDiSelezione

        #region inserisciMenuATendina
        public void inserisciMenuATendina(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, Table table)
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
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                menuATendina.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
            }
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

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaMenuATendina);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(menuATendina);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
        }
        #endregion inserisciMenuATendina

        #region inserisciSelezioneEsclusiva
        public void inserisciSelezioneEsclusiva(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, Table table)
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
                cancella_selezioneEsclusiva.Attributes.Add("tableParent", table.ID);
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
        }
        #endregion inserisciSelezioneEsclusiva

        #region inserisciContatore
        public void inserisciContatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, Table table)
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
        }
        #endregion inserisciContatore

        #region inserisciData
        public void inserisciData(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, Table table)
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
        }
        #endregion inserisciData

        #region inserisciCorrispondente
        public void inserisciCorrispondente(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, Table table)
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
            corrispondente.CSS_CODICE = "comp_profilazione_anteprima";
            corrispondente.CSS_DESCRIZIONE = "comp_profilazione_anteprima";
            corrispondente.DESCRIZIONE_READ_ONLY = false;
            corrispondente.TIPO_CORRISPONDENTE = "0";
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();

            if (IsPostBack && SiteNavigation.CallContextStack.CurrentContext.IsBack)
            {
                if (!string.IsNullOrEmpty(corrispondente.CODICE_TEXT))
                {
                    DocsPAWA.DocsPaWR.Corrispondente corr_1 = UserManager.getCorrispondenteByCodRubrica(this, corrispondente.CODICE_TEXT);
                    if (corr_1 != null)
                    {
                        corrispondente.CODICE_TEXT = corr_1.codiceRubrica;
                        corrispondente.DESCRIZIONE_TEXT = corr_1.descrizione;
                        oggettoCustom.VALORE_DATABASE = corr_1.systemId;
                    }
                    else
                    {
                        corrispondente.CODICE_TEXT = "";
                        corrispondente.DESCRIZIONE_TEXT = "";
                        oggettoCustom.VALORE_DATABASE = "";
                    }
                }
                else
                {
                    corrispondente.CODICE_TEXT = "";
                    corrispondente.DESCRIZIONE_TEXT = "";
                    oggettoCustom.VALORE_DATABASE = "";
                }
            }
            else
            {
                //E' stato valorizzato il campo.
                if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                {
                    if (Char.IsNumber(oggettoCustom.VALORE_DATABASE, 0))
                    {
                        DocsPAWA.DocsPaWR.Corrispondente corr_1 = (DocsPAWA.DocsPaWR.Corrispondente)UserManager.getCorrispondenteBySystemID(this, oggettoCustom.VALORE_DATABASE);
                        if (corr_1 != null)
                        {
                            corrispondente.CODICE_TEXT = corr_1.codiceRubrica;
                            corrispondente.DESCRIZIONE_TEXT = corr_1.descrizione;
                        }
                    }
                    else
                    {
                        corrispondente.CODICE_TEXT = "";
                        corrispondente.DESCRIZIONE_TEXT = "";
                    }
                    // oggettoCustom.VALORE_DATABASE = "";
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
                        corrispondente.CODICE_TEXT = corr_3.codiceRubrica;
                        corrispondente.DESCRIZIONE_TEXT = corr_3.descrizione;
                        Session.Remove("rubrica.campoCorrispondente");
                        Session.Remove("rubrica.idCampoCorrispondente");
                    }
                }
            }

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

        #region cancella_selezioneEsclusiva_Click
        private void cancella_selezioneEsclusiva_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string idOggetto = (((ImageButton)sender).ID).Substring(1);
            string tableParent = ((ImageButton)sender).Attributes["tableParent"];
            switch (tableParent)
            {
                case "tableDoc":
                    ((RadioButtonList)panel_ContenutoCampiDoc.FindControl(idOggetto)).SelectedIndex = -1;

                    for (int i = 0; i < template_CC_Doc.ELENCO_OGGETTI.Length; i++)
                    {
                        if (((DocsPAWA.DocsPaWR.OggettoCustom)template_CC_Doc.ELENCO_OGGETTI[i]).SYSTEM_ID.ToString() == idOggetto)
                        {
                            ((DocsPAWA.DocsPaWR.OggettoCustom)template_CC_Doc.ELENCO_OGGETTI[i]).VALORE_DATABASE = "";
                            SiteNavigation.CallContextStack.CurrentContext.ContextState.Add("template_CC_Doc", template_CC_Doc);
                        }
                    }
                    break;
                case "tableFasc":
                    ((RadioButtonList)panel_ContenutoCampiFasc.FindControl(idOggetto)).SelectedIndex = -1;
                    for (int i = 0; i < template_CC_Fasc.ELENCO_OGGETTI.Length; i++)
                    {
                        if (((DocsPAWA.DocsPaWR.OggettoCustom)template_CC_Fasc.ELENCO_OGGETTI[i]).SYSTEM_ID.ToString() == idOggetto)
                        {
                            ((DocsPAWA.DocsPaWR.OggettoCustom)template_CC_Fasc.ELENCO_OGGETTI[i]).VALORE_DATABASE = "";
                            SiteNavigation.CallContextStack.CurrentContext.ContextState.Add("template_CC_Fasc", template_CC_Fasc);
                        }
                    }
                    break;
            }           
        }
        #endregion cancella_selezioneEsclusiva_Click

        #region Ricerca
        protected void btn_ricerca_Click(object sender, EventArgs e)
        {
            //Valorizzo i campi comuni dei documenti
            foreach (DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom in template_CC_Doc.ELENCO_OGGETTI)
            {
                controllaCampi(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString(), panel_ContenutoCampiDoc);
            }

            //Valorizzo i campi comuni dei fascicoli
            foreach (DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom in template_CC_Fasc.ELENCO_OGGETTI)
            {
                controllaCampi(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString(), panel_ContenutoCampiFasc);
            }

            //Creo il filtro di ricerca, lo salvo in sessione e chiamo la pagina "RisultatoRicercaCampiComuni.aspx"
            DocsPAWA.DocsPaWR.FiltroRicerca[][] listaFiltri = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
            listaFiltri[0] = new DocsPAWA.DocsPaWR.FiltroRicerca[1];
            DocsPAWA.DocsPaWR.FiltroRicerca[] fVList = new DocsPAWA.DocsPaWR.FiltroRicerca[2];

            //CAMPI COMUNI FASCICOLO
            DocsPAWA.DocsPaWR.FiltroRicerca fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            fV1.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.TEMPLATE_CAMPI_COMUNI_FASC.ToString();
            fV1.template = template_CC_Fasc;
            fV1.valore = "Profilazione Dinamica";

            //CAMPI COMUNI DOCUMENTO
            DocsPAWA.DocsPaWR.FiltroRicerca fV2 = new DocsPAWA.DocsPaWR.FiltroRicerca();
            fV2.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.TEMPLATE_CAMPI_COMUNI_DOC.ToString();
            fV2.template = template_CC_Doc;
            fV2.valore = "Profilazione Dinamica";

            fVList[0] = fV1;
            fVList[1] = fV2;
            listaFiltri[0] = fVList;

            SiteNavigation.CallContextStack.CurrentContext.IsBack = false;
            SiteNavigation.CallContextStack.CurrentContext.PageNumber = 1;
            SiteNavigation.CallContextStack.CurrentContext.QueryStringParameters["docFascIndex"] = "-1";
            SiteNavigation.CallContextStack.CurrentContext.ContextState["filtriRicercaCampiComuni"] = listaFiltri;

            Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'RisultatoRicercaCampiComuni.aspx';</script>");
        }

        private bool controllaCampi(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string idOggetto, Panel panel_Contenuto)
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
            //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"
			
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto":
                    TextBox textBox = (TextBox)panel_Contenuto.FindControl(idOggetto);
                    if (textBox.Text.Equals(""))
                    {
                        oggettoCustom.VALORE_DATABASE = textBox.Text;
                        return true;
                    }
                    oggettoCustom.VALORE_DATABASE = textBox.Text;
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList checkBox = (CheckBoxList)panel_Contenuto.FindControl(idOggetto);
                    if (checkBox.SelectedIndex == -1)
                    {
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
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }
                    oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)panel_Contenuto.FindControl(idOggetto);
                    if (oggettoCustom.VALORE_DATABASE == "-1" || radioButtonList.SelectedIndex == -1 || radioButtonList.SelectedValue == "-1")
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }
                    oggettoCustom.VALORE_DATABASE = radioButtonList.SelectedItem.Text;
                    break;
                case "Data":
                    UserControls.Calendar dataDa = (UserControls.Calendar)panel_Contenuto.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                    UserControls.Calendar dataA = (UserControls.Calendar)panel_Contenuto.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());

                    //if (dataDa.txt_Data.Text.Equals("") && dataA.txt_Data.Text.Equals(""))
                    if (dataDa.Text.Equals("") && dataA.Text.Equals(""))
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }

                    //if (dataDa.txt_Data.Text.Equals("") && dataA.txt_Data.Text != "")
                    if (dataDa.Text.Equals("") && dataA.Text != "")
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }

                    //if (dataDa.txt_Data.Text != "" && dataA.txt_Data.Text != "")
                    if (dataDa.Text != "" && dataA.Text != "")
                        //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text + "@" + dataA.txt_Data.Text;
                        oggettoCustom.VALORE_DATABASE = dataDa.Text + "@" + dataA.Text;

                    //if (dataDa.txt_Data.Text != "" && dataA.txt_Data.Text == "")
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
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }
                            break;
                        case "A":
                            DropDownList ddlAoo = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
                            {
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
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }

                            if (contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                    }

                    if (contatoreDa.Text.Equals("") && contatoreA.Text != "")
                    {
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
                        DocsPaWR.Corrispondente corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);
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
                    break;
            }
            return false;
        }
        #endregion Ricerca
    }
}