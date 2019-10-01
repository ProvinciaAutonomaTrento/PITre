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
using SAAdminTool;
using SAAdminTool.DocsPaWR;
using SAAdminTool.UserControls;

namespace SAAdminTool.AdminTool.Gestione_ProfDinamica
{
	public class AnteprimaProfDinamica : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_NomeModello;
		protected System.Web.UI.WebControls.Panel panel_Contenuto;
        protected Table table;
		protected SAAdminTool.DocsPaWR.Templates template;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected ArrayList menuDaInserire = new ArrayList();
		protected bool focus = true;
						
		private void Page_Load(object sender, System.EventArgs e)
		{
			template = (SAAdminTool.DocsPaWR.Templates) Session["template"];
			lbl_NomeModello.Text = template.DESCRIZIONE;

			inserisciComponenti();			
		}
        
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		public void inserisciComponenti()
		{
            table = new Table();
            table.Width = Unit.Percentage(100);
			for(int i=0; i<template.ELENCO_OGGETTI.Length; i++)
			{
				SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom = (SAAdminTool.DocsPaWR.OggettoCustom) template.ELENCO_OGGETTI[i];
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
					case "Data" :
						inserisciData(oggettoCustom);
						break;
                    case "Corrispondente":
                        inserisciCorrispondente(oggettoCustom);
                        break;
                    case "Link":
                        inserisciLink(oggettoCustom);
                        break;
                    case "ContatoreSottocontatore":
                        inserisciSottocontatore(oggettoCustom);
                        break;
                    case "Separatore":
                        inserisciCampoSeparatore(oggettoCustom);
                        break;
                    case "OggettoEsterno":
                        inserisciOggettoEsterno(oggettoCustom,i);
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

		#region inserisciCampoDiTesto
		public void inserisciCampoDiTesto(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
		{
			if(oggettoCustom.DESCRIZIONE.Equals(""))
			{
				return;
			}
			Label etichettaCampoDiTesto = new Label();
			TextBox txt_CampoDiTesto = new TextBox();				
			if(oggettoCustom.MULTILINEA.Equals("SI"))
			{
				if(oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
				{
					etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE + " *";
				}
				else
				{
					etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;				
				}
				etichettaCampoDiTesto.Font.Size = FontUnit.Point(8);
				etichettaCampoDiTesto.Font.Bold = true;
				etichettaCampoDiTesto.Font.Name = "Verdana";
				
				//txt_CampoDiTesto.Width = 430;
                txt_CampoDiTesto.Width = Unit.Percentage(100);
				txt_CampoDiTesto.TextMode = TextBoxMode.MultiLine;
				
				if(oggettoCustom.NUMERO_DI_LINEE.Equals(""))
				{
					txt_CampoDiTesto.Height = 55;
				}
				else
				{
					txt_CampoDiTesto.Rows = Convert.ToInt32(oggettoCustom.NUMERO_DI_LINEE);
				}

				if(oggettoCustom.NUMERO_DI_CARATTERI.Equals(""))
				{
					txt_CampoDiTesto.MaxLength = 150;
				}
				else
				{
					txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
				}

				txt_CampoDiTesto.ID = oggettoCustom.POSIZIONE;

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
				if(oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
				{
					etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE + " *";
				}
				else
				{
					etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;				
				}
				etichettaCampoDiTesto.Font.Size = FontUnit.Point(8);
				etichettaCampoDiTesto.Font.Bold = true;
				etichettaCampoDiTesto.Font.Name = "Verdana";
				
				if(!oggettoCustom.NUMERO_DI_CARATTERI.Equals(""))
				{
					//ATTENZIONE : La lunghezza della textBox non è speculare al numero massimo di
					//caratteri che l'utente inserisce.
					txt_CampoDiTesto.Width = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6;
					txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
				}
				txt_CampoDiTesto.ID = oggettoCustom.POSIZIONE;
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
            
			if(focus)
			{
				SetFocus(txt_CampoDiTesto);
				focus = false;	
			}
		}
		#endregion inserisciCampoDiTesto

		#region inserisciCasellaDiSelezione
		public void inserisciCasellaDiSelezione(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
		{
			if(oggettoCustom.DESCRIZIONE.Equals(""))
			{
				return;
			}
			Label etichettaCasellaSelezione = new Label();
			if(oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
			{
				etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE + " *";
			}
			else
			{
				etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE;			
			}
			etichettaCasellaSelezione.Font.Size = FontUnit.Point(8);
			etichettaCasellaSelezione.Font.Bold = true;
			etichettaCasellaSelezione.Font.Name = "Verdana";
			//etichettaCasellaSelezione.Width = 430;
            etichettaCasellaSelezione.Width = Unit.Percentage(100);
				
			CheckBoxList casellaSelezione = new CheckBoxList();
			casellaSelezione.ID = oggettoCustom.POSIZIONE;
			int valoreDiDefault = -1;
			for(int i=0; i<oggettoCustom.ELENCO_VALORI.Length; i++)
			{
				SAAdminTool.DocsPaWR.ValoreOggetto valoreOggetto = ((SAAdminTool.DocsPaWR.ValoreOggetto) (oggettoCustom.ELENCO_VALORI[i]));
				casellaSelezione.Items.Add(new ListItem(valoreOggetto.VALORE,valoreOggetto.VALORE));
				if(valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
				{
					valoreDiDefault = i;
				}
			}
			casellaSelezione.CssClass = "comp_profilazione_anteprima";
			if(oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
			{
				casellaSelezione.RepeatDirection = RepeatDirection.Horizontal;
			}
			else
			{
				casellaSelezione.RepeatDirection = RepeatDirection.Vertical;
			}
			if(valoreDiDefault != -1)
			{
				casellaSelezione.SelectedIndex = valoreDiDefault;
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

			if(focus)
			{
				SetFocus(casellaSelezione);
				focus = false;	
			}
		}
		#endregion inserisciCasellaDiSelezione

		#region inserisciMenuATendina
		public void inserisciMenuATendina(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
		{
			if(oggettoCustom.DESCRIZIONE.Equals(""))
			{
				return;
			}
			Label etichettaMenuATendina = new Label();
			if(oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
			{
				etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE + " *";
			}
			else
			{
				etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE;			
			}
			etichettaMenuATendina.Font.Size = FontUnit.Point(8);
			etichettaMenuATendina.Font.Bold = true;
			etichettaMenuATendina.Font.Name = "Verdana";

			DropDownList menuATendina = new DropDownList();
			menuATendina.ID = oggettoCustom.POSIZIONE;
			int valoreDiDefault = -1;
			for(int i=0; i<oggettoCustom.ELENCO_VALORI.Length; i++)
			{
				SAAdminTool.DocsPaWR.ValoreOggetto valoreOggetto = ((SAAdminTool.DocsPaWR.ValoreOggetto) (oggettoCustom.ELENCO_VALORI[i]));
				menuATendina.Items.Add(new ListItem(valoreOggetto.VALORE,valoreOggetto.VALORE));
				if(valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
				{
					valoreDiDefault = i;
				}
			}
			menuATendina.CssClass = "comp_profilazione_anteprima";
			if(valoreDiDefault != -1)
			{
				menuATendina.SelectedIndex = valoreDiDefault;
			}
			if( !(valoreDiDefault != -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")) )
			{
				menuATendina.Items.Insert(0,"");			
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
		public void inserisciSelezioneEsclusiva(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
		{
			if(oggettoCustom.DESCRIZIONE.Equals(""))
			{
				return;
			}
			Label etichettaSelezioneEsclusiva = new Label();

			HtmlAnchor cancella_selezioneEsclusiva = new HtmlAnchor();
			if(oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
			{
				etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE + " *";
			}
			else
			{
				etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;
			
				cancella_selezioneEsclusiva.HRef = "javascript:clearSelezioneEsclusiva("+oggettoCustom.POSIZIONE+","+oggettoCustom.ELENCO_VALORI.Length+");";
				//cancella_selezioneEsclusiva.HRef = "javascript:clearSelezioneEsclusiva_"+oggettoCustom.POSIZIONE+"();";
				cancella_selezioneEsclusiva.InnerHtml = "<img src=\"../Images/cancella.gif\" width=\"10\" height=\"10\" border=\"0\" alt=\"Resetta selezione\" class=\"resettaSelezioneEsclusiva\">";						
			}

			etichettaSelezioneEsclusiva.Font.Size = FontUnit.Point(8);
			etichettaSelezioneEsclusiva.Font.Bold = true;
			etichettaSelezioneEsclusiva.Font.Name = "Verdana";
			//etichettaSelezioneEsclusiva.Width = 400;
            etichettaSelezioneEsclusiva.Width = Unit.Percentage(90);
				
			RadioButtonList selezioneEsclusiva = new RadioButtonList();
			selezioneEsclusiva.ID = oggettoCustom.POSIZIONE;
			int valoreDiDefault = -1;
			for(int i=0; i<oggettoCustom.ELENCO_VALORI.Length; i++)
			{
				SAAdminTool.DocsPaWR.ValoreOggetto valoreOggetto = ((SAAdminTool.DocsPaWR.ValoreOggetto) (oggettoCustom.ELENCO_VALORI[i]));
				selezioneEsclusiva.Items.Add(new ListItem(valoreOggetto.VALORE,valoreOggetto.VALORE));
				if(valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
				{
					valoreDiDefault = i;
				}
			}
			selezioneEsclusiva.CssClass = "comp_profilazione_anteprima";
			if(oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
			{
				selezioneEsclusiva.RepeatDirection = RepeatDirection.Horizontal;
			}
			else
			{
				selezioneEsclusiva.RepeatDirection = RepeatDirection.Vertical;
			}
			if(valoreDiDefault != -1)
			{
				selezioneEsclusiva.SelectedIndex = valoreDiDefault;
			}

            TableRow row_1 = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(cancella_selezioneEsclusiva);
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
			
			if(focus)
			{
				SetFocus(selezioneEsclusiva);
				focus = false;	
			}
		}
		#endregion inserisciSelezioneEsclusiva

		#region inserisciContatore
		public void inserisciContatore(OggettoCustom oggettoCustom)
		{
			if(oggettoCustom.DESCRIZIONE.Equals(""))
			{
				return;
			}
			Label etichettaContatore = new Label();	
			etichettaContatore.Text = oggettoCustom.DESCRIZIONE;
			etichettaContatore.Font.Size = FontUnit.Point(8);
			etichettaContatore.Font.Bold = true;
			etichettaContatore.Font.Name = "Verdana";

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaContatore);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
                                    
            if (oggettoCustom.CONTA_DOPO == "1")
            {
                //Label etichettaContaDopo = new Label();
                //etichettaContaDopo.Text = "&nbsp;Calcola";
                //etichettaContaDopo.Font.Size = FontUnit.Point(8);
                //etichettaContaDopo.Font.Bold = true;
                //etichettaContaDopo.Font.Name = "Verdana";
                //cell_2.Controls.Add(etichettaContaDopo);
                CheckBox cbContaDopo = new CheckBox();
                cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";
                cbContaDopo.CssClass = "comp_profilazione_anteprima";
                cell_2.Controls.Add(cbContaDopo);
            }

			TextBox contatore = new TextBox();
			contatore.ID = oggettoCustom.POSIZIONE;
            contatore.Width = Unit.Percentage(100);
			contatore.Enabled = false;
			contatore.BackColor = System.Drawing.Color.WhiteSmoke;
			contatore.CssClass = "comp_profilazione_anteprima";
            cell_2.Controls.Add(contatore);

            switch (oggettoCustom.TIPO_CONTATORE)
            {
                case "T":
                    break;
                case "A":
                    Label etichettaAoo = new Label();
                    etichettaAoo.Text = "&nbsp;AOO";
                    etichettaAoo.Font.Size = FontUnit.Point(8);
                    etichettaAoo.Font.Bold = true;
                    etichettaAoo.Font.Name = "Verdana";
                    etichettaAoo.Width = 30;
                    cell_2.Controls.Add(etichettaAoo);
                    DropDownList ddlAoo = new DropDownList();
                    ddlAoo.CssClass = "comp_profilazione_anteprima";
                    ddlAoo.Items.Add("AOO");
                    ddlAoo.Width = 100;
                    cell_2.Controls.Add(ddlAoo);
                    break;
                case "R":
                    Label etichettaRf = new Label();
                    etichettaRf.Text = "&nbsp;RF";
                    etichettaRf.Font.Size = FontUnit.Point(8);
                    etichettaRf.Font.Bold = true;
                    etichettaRf.Font.Name = "Verdana";
                    etichettaRf.Width = 30;
                    cell_2.Controls.Add(etichettaRf);
                    DropDownList ddlRf = new DropDownList();
                    ddlRf.CssClass = "comp_profilazione_anteprima";
                    ddlRf.Items.Add("RF");
                    ddlRf.Width = 100;
                    cell_2.Controls.Add(ddlRf);
                    break;
            }            

            row.Cells.Add(cell_2);
            table.Rows.Add(row);   			
		}
		#endregion inserisciContatore

		#region inserisciData
		public void inserisciData(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
		{
			//Per il momento questo tipo di campo è stato implementato con tre semplici textBox
			//Sarebbe opportuno creare un oggetto personalizzato, che espone le stesse funzionalità
			//della textBox, ma che mi permette di gestire la data con i tre campi separati.
			if(oggettoCustom.DESCRIZIONE.Equals(""))
			{
				return;
			}
			Label etichettaData = new Label();
			if(oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
			{
				etichettaData.Text = oggettoCustom.DESCRIZIONE + " *";
			}
			else
			{
				etichettaData.Text = oggettoCustom.DESCRIZIONE;			
			}
			etichettaData.Font.Size = FontUnit.Point(8);
			etichettaData.Font.Bold = true;
			etichettaData.Font.Name = "Verdana";

            SAAdminTool.UserControls.Calendar data = (SAAdminTool.UserControls.Calendar)this.LoadControl("../../UserControls/Calendar.ascx");            
            data.fromUrl = "../../UserControls/DialogCalendar.aspx";
            data.CSS = "comp_profilazione_anteprima";
            data.VisibleTimeMode = ProfilazioneDocManager.getVisibleTimeMode(oggettoCustom);
            
            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaData);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(data);
            row.Cells.Add(cell_2);
            table.Rows.Add(row); 
		}
		#endregion inserisciData

        #region inserisciCorrispondente
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

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }

            TextBox txt_codice = new TextBox();
            txt_codice.Width = Unit.Percentage(20);
            txt_codice.ID = "txt_codice" + oggettoCustom.POSIZIONE;
            txt_codice.CssClass = "comp_profilazione_anteprima";
            TextBox txt_descrizione = new TextBox();
            txt_descrizione.Width = Unit.Percentage(55);
            txt_descrizione.ID = "txt_descrizione" + oggettoCustom.POSIZIONE;
            txt_descrizione.CssClass = "comp_profilazione_anteprima";
            ImageButton btn_rubrica = new ImageButton();
            btn_rubrica.ImageUrl = "~/images/proto/rubrica.gif";
            btn_rubrica.ID = "btn_rubrica " + oggettoCustom.POSIZIONE;

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichetta);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(txt_codice);
            cell_2.Controls.Add(txt_descrizione);
            cell_2.Controls.Add(btn_rubrica);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);

            /*
            SAAdminTool.UserControls.Corrispondente corrispondente = (SAAdminTool.UserControls.Corrispondente)this.LoadControl("../../UserControls/Corrispondente.ascx");
            corrispondente.CSS_CODICE = "comp_profilazione_anteprima";
            corrispondente.CSS_DESCRIZIONE = "comp_profilazione_anteprima";
            corrispondente.DESCRIZIONE_READ_ONLY = true;
            corrispondente.TIPO_CORRISPONDENTE = oggettoCustom.TIPO_RICERCA_CORR;
            
            if (oggettoCustom.ID_RUOLO_DEFAULT != null && oggettoCustom.ID_RUOLO_DEFAULT != "" && oggettoCustom.ID_RUOLO_DEFAULT != "0")
            {
                corrispondente.CODICE_READ_ONLY = true;
                corrispondente.DESCRIZIONE_TEXT = UserManager.getRuoloById(oggettoCustom.ID_RUOLO_DEFAULT,this).descrizione;
            }
            else
            {
                if (Session["rubrica.campoCorrispondente"] != null)
                {
                    SAAdminTool.DocsPaWR.Corrispondente corr = (SAAdminTool.DocsPaWR.Corrispondente)Session["rubrica.campoCorrispondente"];
                    if (corr != null)
                    {
                        corrispondente.DESCRIZIONE_TEXT = corr.descrizione;
                        corrispondente.CODICE_TEXT = corr.codiceRubrica;
                        Session.Remove("rubrica.campoCorrispondente");
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
            */
        }
        #endregion inserisciCorrispondente

        #region inserisciLink
        public void inserisciLink(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }

            Label etichetta = new Label();
            etichetta.Font.Size = FontUnit.Point(8);
            etichetta.Font.Bold = true;
            etichetta.Font.Name = "Verdana";

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            LinkDocFasc link = (LinkDocFasc)this.LoadControl("../../UserControls/LinkDocFasc.ascx");
            link.IsInsertModify = true;
            link.IsAnteprima = true;
            link.IsEsterno = ("ESTERNO".Equals(oggettoCustom.TIPO_LINK));
            link.IsFascicolo = ("FASCICOLO".Equals(oggettoCustom.TIPO_OBJ_LINK));
            link.TextCssClass = "comp_profilazione_anteprima";
            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichetta);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(link);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
        }
        #endregion inserisciLink

        #region inserisciSottocontatore
        public void inserisciSottocontatore(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaSottocontatore = new Label();
            etichettaSottocontatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaSottocontatore.Font.Size = FontUnit.Point(8);
            etichettaSottocontatore.Font.Bold = true;
            etichettaSottocontatore.Font.Name = "Verdana";

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaSottocontatore);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();

            if (oggettoCustom.CONTA_DOPO == "1")
            {
                //Label etichettaContaDopo = new Label();
                //etichettaContaDopo.Text = "&nbsp;Calcola";
                //etichettaContaDopo.Font.Size = FontUnit.Point(8);
                //etichettaContaDopo.Font.Bold = true;
                //etichettaContaDopo.Font.Name = "Verdana";
                //cell_2.Controls.Add(etichettaContaDopo);
                CheckBox cbContaDopoSottocontatore = new CheckBox();
                cbContaDopoSottocontatore.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";
                cbContaDopoSottocontatore.CssClass = "comp_profilazione_anteprima";
                cell_2.Controls.Add(cbContaDopoSottocontatore);
            }

            TextBox contatoreSottocontatore = new TextBox();
            contatoreSottocontatore.ID = oggettoCustom.POSIZIONE;
            contatoreSottocontatore.Width = Unit.Percentage(10);
            contatoreSottocontatore.Enabled = false;
            contatoreSottocontatore.BackColor = System.Drawing.Color.WhiteSmoke;
            contatoreSottocontatore.CssClass = "comp_profilazione_anteprima";
            cell_2.Controls.Add(contatoreSottocontatore);

            TextBox sottocontatore = new TextBox();
            sottocontatore.ID = oggettoCustom.POSIZIONE;
            sottocontatore.Width = Unit.Percentage(10);
            sottocontatore.Enabled = false;
            sottocontatore.BackColor = System.Drawing.Color.WhiteSmoke;
            sottocontatore.CssClass = "comp_profilazione_anteprima";
            cell_2.Controls.Add(sottocontatore);

            TextBox dataSottocontatore = new TextBox();
            dataSottocontatore.ID = oggettoCustom.POSIZIONE;
            dataSottocontatore.Width = Unit.Percentage(20);
            dataSottocontatore.Enabled = false;
            dataSottocontatore.BackColor = System.Drawing.Color.WhiteSmoke;
            dataSottocontatore.CssClass = "comp_profilazione_anteprima";
            cell_2.Controls.Add(dataSottocontatore);

            switch (oggettoCustom.TIPO_CONTATORE)
            {
                case "T":
                    break;
                case "A":
                    Label etichettaAooSottocontaore = new Label();
                    etichettaAooSottocontaore.Text = "&nbsp;AOO";
                    etichettaAooSottocontaore.Font.Size = FontUnit.Point(8);
                    etichettaAooSottocontaore.Font.Bold = true;
                    etichettaAooSottocontaore.Font.Name = "Verdana";
                    etichettaAooSottocontaore.Width = 30;
                    cell_2.Controls.Add(etichettaAooSottocontaore);
                    DropDownList ddlAooSottocontatore = new DropDownList();
                    ddlAooSottocontatore.CssClass = "comp_profilazione_anteprima";
                    ddlAooSottocontatore.Items.Add("AOO");
                    ddlAooSottocontatore.Width = 100;
                    cell_2.Controls.Add(ddlAooSottocontatore);
                    break;
                case "R":
                    Label etichettaRfSottocontatore = new Label();
                    etichettaRfSottocontatore.Text = "&nbsp;RF";
                    etichettaRfSottocontatore.Font.Size = FontUnit.Point(8);
                    etichettaRfSottocontatore.Font.Bold = true;
                    etichettaRfSottocontatore.Font.Name = "Verdana";
                    etichettaRfSottocontatore.Width = 30;
                    cell_2.Controls.Add(etichettaRfSottocontatore);
                    DropDownList ddlRfSottocontatore = new DropDownList();
                    ddlRfSottocontatore.CssClass = "comp_profilazione_anteprima";
                    ddlRfSottocontatore.Items.Add("RF");
                    ddlRfSottocontatore.Width = 100;
                    cell_2.Controls.Add(ddlRfSottocontatore);
                    break;
            }

            row.Cells.Add(cell_2);
            table.Rows.Add(row);   		
        }
        #endregion inserisciSottocontatore

        #region inserisciSeparatore
        public void inserisciCampoSeparatore(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }

            Label etichettaCampoSeparatore = new Label();
            etichettaCampoSeparatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaCampoSeparatore.Font.Size = FontUnit.Point(8);
            etichettaCampoSeparatore.Font.Bold = true;
            etichettaCampoSeparatore.Font.Name = "Verdana";

            System.Web.UI.HtmlControls.HtmlGenericControl rigaSeparatore = new HtmlGenericControl("hr");
            rigaSeparatore.Style.Value = "width=100%;";

            TableRow row_1 = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaCampoSeparatore);
            cell_1.Controls.Add(rigaSeparatore);
            cell_1.ColumnSpan = 2;
            row_1.Cells.Add(cell_1);
            table.Rows.Add(row_1);

        }
        #endregion inserisciSeparatore

        #region inserisciOggettoEsterno
        public void inserisciOggettoEsterno(SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom,int position)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichetta = new Label();
            etichetta.Font.Size = FontUnit.Point(8);
            etichetta.Font.Bold = true;
            etichetta.Font.Name = "Verdana";

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            IntegrationAdapter intAd = (IntegrationAdapter)this.LoadControl("../../UserControls/IntegrationAdapter.ascx");
            intAd.View = IntegrationAdapterView.ANTEPRIMA;
            intAd.ConfigurationValue = oggettoCustom.CONFIG_OBJ_EST;
            intAd.CssClass = "comp_profilazione_anteprima";
            intAd.Position = "" + position;
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

        #region SetFocus
        private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>try{document.getElementById('" + ctrl.ID + "').focus();}catch(e){}</SCRIPT>";
			RegisterStartupScript("focus", s);
		}
		#endregion SetFocus		
	}
}
