using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.DocsPaWR;
using SAAdminTool.SiteNavigation;
using SAAdminTool.UserControls;

namespace SAAdminTool.AdminTool.Gestione_Conservazione
{
    public partial class CampiProfilati : System.Web.UI.Page
    {
        protected Table table;
        private DocsPaWR.DocsPaWebService _wsInstance = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            if (Request.QueryString["id"] != null && !string.IsNullOrEmpty(Request.QueryString["id"].ToString()))
            {
                if (this.Template == null)
                {
                    if (Request.QueryString["type"] != null && (Request.QueryString["type"].ToString()).Equals("F"))
                    {
                        this.Template = ProfilazioneFascManager.getTemplateFascById(Request.QueryString["id"].ToString(),this.Page);
                    }
                    else
                    {
                        this.Template = ProfilazioneDocManager.getTemplateById(Request.QueryString["id"].ToString(), this.Page);
                    }
                }
               lbl_NomeModello.Text = this.Template.DESCRIZIONE;

               CaricaRegistri();
               inserisciComponenti();
            }
        }

        protected void CaricaRegistri()
        {
            DocsPaWR.Registro[] registri = WsInstance.GetRfByIdAmm(IdAmministrazione, "0");
            DocsPaWR.Registro[] rf = WsInstance.GetRfByIdAmm(IdAmministrazione, "1");


            int totaleRegistri = 0;
            if (registri != null && registri.Length > 0)
            {
                totaleRegistri = registri.Length;
            }
            if (rf != null && rf.Length > 0)
            {
                totaleRegistri = totaleRegistri + rf.Length;
            }

            totaleRegistri = totaleRegistri - 1;
            this.RegistriRfVisibili = new Registro[totaleRegistri];
            int temp = 0;
            if (registri != null && registri.Length > 0)
            {
                for (int i = 0; i < registri.Length; i++)
                {
                    this.RegistriRfVisibili[i] = registri[i];
                    this.RegistriRfVisibili[i].chaRF = "0";
                    temp = i;
                }
            }

            if (rf != null && rf.Length > 0)
            {
                for (int y = 0; y < rf.Length; y++)
                {
                    this.RegistriRfVisibili[temp] = rf[y];
                    this.RegistriRfVisibili[temp].chaRF = "1";
                    temp++;
                }
            }
        }

        public void inserisciComponenti()
		{
            table = new Table();
            table.Width = Unit.Percentage(100);
            for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
			{
                DocsPaWR.OggettoCustom oggettoCustom = (SAAdminTool.DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];

				switch(oggettoCustom.TIPO.DESCRIZIONE_TIPO)
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

        public void inserisciCampoDiTesto(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
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

        public void inserisciCasellaDiSelezione(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
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
                DocsPaWR.ValoreOggetto valoreOggetto = ((SAAdminTool.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
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

        private void impostaSelezioneCaselleDiSelezione(SAAdminTool.DocsPaWR.OggettoCustom objCustom, CheckBoxList cbl)
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

        public void inserisciMenuATendina(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
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
                DocsPaWR.ValoreOggetto valoreOggetto = ((SAAdminTool.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
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

        private int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        public void inserisciSelezioneEsclusiva(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
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
                cancella_selezioneEsclusiva.ImageUrl = "../../images/cancella.gif";
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
                DocsPaWR.ValoreOggetto valoreOggetto = ((SAAdminTool.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
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

        private int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
        {
            for (int i = 0; i < rbl.Items.Count; i++)
            {
                if (rbl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        public void inserisciContatore(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
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
                    for (int i = 0; i < this.RegistriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((DocsPaWR.Registro)this.RegistriRfVisibili[i]).chaRF == "0")
                        {
                            item.Value = ((DocsPaWR.Registro)this.RegistriRfVisibili[i]).systemId;
                            item.Text = ((DocsPaWR.Registro)this.RegistriRfVisibili[i]).codice;
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
                    for (int i = 0; i < this.RegistriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((DocsPaWR.Registro)this.RegistriRfVisibili[i]).chaRF == "1")
                        {
                            item.Value = ((DocsPaWR.Registro)this.RegistriRfVisibili[i]).systemId;
                            item.Text = ((DocsPaWR.Registro)this.RegistriRfVisibili[i]).codice;
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

        public void inserisciData(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
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

            SAAdminTool.UserControls.Calendar dataDa = (SAAdminTool.UserControls.Calendar)this.LoadControl("../../UserControls/Calendar.ascx");
            dataDa.fromUrl = "../../UserControls/DialogCalendar.aspx";
            dataDa.CSS = "testo_grigio";
            dataDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            dataDa.VisibleTimeMode = ProfilazioneDocManager.getVisibleTimeMode(oggettoCustom);

            SAAdminTool.UserControls.Calendar dataA = (SAAdminTool.UserControls.Calendar)this.LoadControl("../../UserControls/Calendar.ascx");
            dataA.fromUrl = "../../UserControls/DialogCalendar.aspx";
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

        public void inserisciCorrispondente(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
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

            SAAdminTool.UserControls.Corrispondente corrispondente = (SAAdminTool.UserControls.Corrispondente)this.LoadControl("../../UserControls/Corrispondente.ascx");
            corrispondente.CSS_CODICE = "comp_profilazione_anteprima";
            corrispondente.CSS_DESCRIZIONE = "comp_profilazione_anteprima";
            corrispondente.DESCRIZIONE_READ_ONLY = false;
            corrispondente.TIPO_CORRISPONDENTE = "0";
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();

            //E' stato valorizzato il campo.
            if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
            {
                if (Char.IsNumber(oggettoCustom.VALORE_DATABASE, 0))
                {
                    SAAdminTool.DocsPaWR.Corrispondente corr_1 = (SAAdminTool.DocsPaWR.Corrispondente)UserManager.getCorrispondenteBySystemID(this, oggettoCustom.VALORE_DATABASE);
                    if (corr_1 != null)
                    {
                        corrispondente.CODICE_TEXT = corr_1.codiceRubrica;
                        corrispondente.DESCRIZIONE_TEXT = corr_1.descrizione;
                    }
                }
                else
                {
                    corrispondente.CODICE_TEXT = "";
                    corrispondente.DESCRIZIONE_TEXT = oggettoCustom.VALORE_DATABASE;
                }
                oggettoCustom.VALORE_DATABASE = "";
            }
            //E' stato selezionato un corrispondente da rubrica.
            if (Session["rubrica.campoCorrispondente"] != null)
            {
                SAAdminTool.DocsPaWR.Corrispondente corr_3 = (SAAdminTool.DocsPaWR.Corrispondente)Session["rubrica.campoCorrispondente"];
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

        public void inserisciContatoreSottocontatore(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
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

            SAAdminTool.UserControls.Calendar dataSottocontatoreDa = (SAAdminTool.UserControls.Calendar)this.LoadControl("../../UserControls/Calendar.ascx");
            dataSottocontatoreDa.fromUrl = "../../UserControls/DialogCalendar.aspx";
            dataSottocontatoreDa.CSS = "testo_grigio";
            dataSottocontatoreDa.ID = "da_dataSottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            dataSottocontatoreDa.VisibleTimeMode = ProfilazioneDocManager.getVisibleTimeMode(oggettoCustom);

            SAAdminTool.UserControls.Calendar dataSottocontatoreA = (SAAdminTool.UserControls.Calendar)this.LoadControl("../../UserControls/Calendar.ascx");
            dataSottocontatoreA.fromUrl = "../../UserControls/DialogCalendar.aspx";
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
                    for (int i = 0; i < this.RegistriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((DocsPaWR.Registro)this.RegistriRfVisibili[i]).chaRF == "0")
                        {
                            item.Value = ((DocsPaWR.Registro)this.RegistriRfVisibili[i]).systemId;
                            item.Text = ((DocsPaWR.Registro)this.RegistriRfVisibili[i]).codice;
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
                    for (int i = 0; i < this.RegistriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((DocsPaWR.Registro)this.RegistriRfVisibili[i]).chaRF == "1")
                        {
                            item.Value = ((DocsPaWR.Registro)this.RegistriRfVisibili[i]).systemId;
                            item.Text = ((DocsPaWR.Registro)this.RegistriRfVisibili[i]).codice;
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
        }

        public void inserisciOggettoEsterno(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
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
            SAAdminTool.UserControls.IntegrationAdapter intAd = (SAAdminTool.UserControls.IntegrationAdapter)this.LoadControl("../../UserControls/IntegrationAdapter.ascx");
            intAd.ID = oggettoCustom.SYSTEM_ID.ToString();
            intAd.View = SAAdminTool.UserControls.IntegrationAdapterView.RICERCA;
            intAd.CssClass = "testo_grigio";

            intAd.ConfigurationValue = oggettoCustom.CONFIG_OBJ_EST;
            IntegrationAdapterValue value = new IntegrationAdapterValue(oggettoCustom.CODICE_DB, oggettoCustom.VALORE_DATABASE, oggettoCustom.MANUAL_INSERT);
            intAd.Value = value;

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichetta);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(intAd);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
        }

        private void cancella_selezioneEsclusiva_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string idOggetto = (((ImageButton)sender).ID).Substring(1);
            ((RadioButtonList)panel_Contenuto.FindControl(idOggetto)).SelectedIndex = -1;
            for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
            {
                if (((SAAdminTool.DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).SYSTEM_ID.ToString() == idOggetto)
                {
                    ((SAAdminTool.DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).VALORE_DATABASE = "";
                }
            }
        }

        protected void btn_resettaPopUp_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (SAAdminTool.DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];
                resettaCampo(oggettoCustom);
            }
        }

        private void resettaCampo(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
        {
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto":
                    TextBox textBox = (TextBox)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    textBox.Text = "";
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList checkBox = (CheckBoxList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    checkBox.SelectedIndex = -1;
                    break;
                case "MenuATendina":
                    DropDownList dropDwonList = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    dropDwonList.SelectedIndex = -1;
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    radioButtonList.SelectedIndex = -1;
                    break;
                case "Data":
                    UserControls.Calendar dataDa = (UserControls.Calendar)panel_Contenuto.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                    UserControls.Calendar dataA = (UserControls.Calendar)panel_Contenuto.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());
                    //dataDa.txt_Data.Text = "";
                    //dataA.txt_Data.Text = "";
                    dataDa.Text = "";
                    dataA.Text = "";

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
                    //TextBox contatore = (TextBox) panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    //contatore.Text = "";					
                    break;
                case "OggettoEsterno":
                    IntegrationAdapterValue nullValue = new IntegrationAdapterValue("", "", false);
                    ((IntegrationAdapter)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString())).Value = nullValue;
                    break;
            }
        }

        /// Al clic viene salvata la lista dei campi profilati
        /// </summary>
        protected void BtnSaveDocumentFormat_Click(object sender, EventArgs e)
        {
            int result = 0;
            for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (SAAdminTool.DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];
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
            if (result == this.Template.ELENCO_OGGETTI.Length)
            {
                Label_Avviso.Text = "Inserire almeno un criterio di ricerca !";
                Label_Avviso.Visible = true;
                return;
            }
            ClientScript.RegisterStartupScript(this.GetType(), "chiudi", "close_and_save();", true);
        }

        /// Al clic viene salvata la lista dei campi profilati
        /// </summary>
        protected void BtnCloseNotSave_Click(object sender, EventArgs e)
        {
            string idPolicy = string.Empty;
            if (Request.QueryString["idPolicy"] != null && !string.IsNullOrEmpty(Request.QueryString["idPolicy"].ToString()))
            {
                idPolicy = Request.QueryString["idPolicy"].ToString();
                Policy pol = _wsInstance.GetPolicyById(idPolicy);
                this.Template = pol.template;
            }
            else
            {
                this.Template = null;
            }
            ClientScript.RegisterStartupScript(this.GetType(), "chiudi", "window.close();", true);
        }

        private bool controllaCampi(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
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


        /// <summary>
        /// Template selezionato
        /// </summary>
        protected Templates Template
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["TemplateProf"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["TemplateProf"] as Templates;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["TemplateProf"] = value;
            }
        }

        protected DocsPaWR.DocsPaWebService WsInstance
        {
            get
            {
                if (this._wsInstance == null)
                    this._wsInstance = new SAAdminTool.DocsPaWR.DocsPaWebService();
                return this._wsInstance;
            }
        }

         /// <summary>
        /// Reperimento idamministrazione corrente
        /// </summary>
        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }


        /// <summary>
        /// Template selezionato
        /// </summary>
        protected Registro[] RegistriRfVisibili
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["registri"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["registri"] as Registro[];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["registri"] = value;
            }
        }
	
    }
}