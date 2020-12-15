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
using DocsPaWA.UserControls;
using System.Collections.Generic;

namespace DocsPAWA.documento
{
	public class AnteprimaProfDinamicaFasc : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_NomeModello;
		protected System.Web.UI.WebControls.Panel panel_Contenuto;
		protected DocsPAWA.DocsPaWR.Templates template;
		protected System.Web.UI.WebControls.Button btn_Chiudi;
		protected Table table;
        protected ArrayList dirittiCampiRuolo;
        protected Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente> dic_Corr;
        
		private void Page_Load(object sender, System.EventArgs e)
		{

                Fascicolo fasc = FascicoliManager.getFascicoloSelezionato();
                if (fasc.template != null)
                {
                    template = fasc.template;
                    lbl_NomeModello.Text = template.DESCRIZIONE;
                    inserisciComponenti("SI");
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
			this.btn_Chiudi.Click += new System.EventHandler(this.btn_Chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion		

		public void inserisciComponenti(string readOnly)
		{
            table = new Table();
            table.Width = Unit.Percentage(100);
            dirittiCampiRuolo = ProfilazioneFascManager.getDirittiCampiTipologiaFasc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), this);
			for(int i=0; i<template.ELENCO_OGGETTI.Length; i++)
			{
				DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom) template.ELENCO_OGGETTI[i];
                ProfilazioneFascManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

				switch(oggettoCustom.TIPO.DESCRIZIONE_TIPO)
				{
					case "CampoDiTesto":
						inserisciCampoDiTesto(oggettoCustom,readOnly);
						break;
					case "CasellaDiSelezione":
						inserisciCasellaDiSelezione(oggettoCustom,readOnly);
						break;
					case "MenuATendina":
						inserisciMenuATendina(oggettoCustom,readOnly);
						break;
					case "SelezioneEsclusiva":
						inserisciSelezioneEsclusiva(oggettoCustom,readOnly);
						break;
					case "Contatore":
						inserisciContatore(oggettoCustom);
						break;
					case "Data" :
						inserisciData(oggettoCustom,readOnly);
						break;
                    case "Corrispondente" :
                        inserisciCorrispondente(oggettoCustom,readOnly);
                        break;
                    case "Link":
                        inserisciLink(oggettoCustom, readOnly);
                        break;
                    case "OggettoEsterno":
                        inserisciOggettoEsterno(oggettoCustom, readOnly);
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
		public void inserisciCampoDiTesto(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
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

            if(readOnly == "SI")
				txt_CampoDiTesto.ReadOnly = true;
		}
		#endregion inserisciCampoDiTesto

		#region inserisciCasellaDiSelezione
		public void inserisciCasellaDiSelezione(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
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
			casellaSelezione.ID = oggettoCustom.SYSTEM_ID.ToString();
			int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreElenco = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                if (i < oggettoCustom.VALORI_SELEZIONATI.Length)
                {
                    string valoreSelezionato = (string)(oggettoCustom.VALORI_SELEZIONATI[i]);
                    if (valoreElenco.ABILITATO == 1 || (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato)))
                    {
                        //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                        if (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato))
                            valoreElenco.ABILITATO = 1;

                        casellaSelezione.Items.Add(new ListItem(valoreElenco.VALORE, valoreElenco.VALORE));
                        //Valore di default
                        if (valoreElenco.VALORE_DI_DEFAULT.Equals("SI"))
                        {
                            valoreDiDefault = i;
                        }
                    }
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

			if(oggettoCustom.VALORI_SELEZIONATI != null)
			{
				impostaSelezioneCaselleDiSelezione(oggettoCustom,casellaSelezione);
			}

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaCasellaSelezione, casellaSelezione, oggettoCustom, template);

            if(readOnly == "SI")
				casellaSelezione.Enabled = false;
            
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
		public void inserisciMenuATendina(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
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
			menuATendina.ID = oggettoCustom.SYSTEM_ID.ToString();
			int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    menuATendina.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    {
                        valoreDiDefault = i;
                    }
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
			if(!oggettoCustom.VALORE_DATABASE.Equals(""))
			{
				menuATendina.SelectedIndex = impostaSelezioneMenuATendina(oggettoCustom.VALORE_DATABASE,menuATendina);
			}

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaMenuATendina, menuATendina, oggettoCustom, template);

            if(readOnly == "SI")
				menuATendina.Enabled = false;

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
		public void inserisciSelezioneEsclusiva(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
		{
            TableRow row_1 = new TableRow();
            TableCell cell_1 = new TableCell();
            
            if(oggettoCustom.DESCRIZIONE.Equals(""))
			{
				return;
			}
			Label etichettaSelezioneEsclusiva = new Label();
			
			if(oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
			{
				etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE + " *";
			}
			else
			{
				etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;
			}

			etichettaSelezioneEsclusiva.Font.Size = FontUnit.Point(8);
			etichettaSelezioneEsclusiva.Font.Bold = true;
			etichettaSelezioneEsclusiva.Font.Name = "Verdana";
			//etichettaSelezioneEsclusiva.Width = 400;
            etichettaSelezioneEsclusiva.Width = Unit.Percentage(90);
				
			RadioButtonList selezioneEsclusiva = new RadioButtonList();
			selezioneEsclusiva.ID = oggettoCustom.SYSTEM_ID.ToString();
			int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    selezioneEsclusiva.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    {
                        valoreDiDefault = i;
                    }
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
			if(!oggettoCustom.VALORE_DATABASE.Equals(""))
			{
				selezioneEsclusiva.SelectedIndex = impostaSelezioneEsclusiva(oggettoCustom.VALORE_DATABASE,selezioneEsclusiva);
			}

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSelezioneEsclusiva(etichettaSelezioneEsclusiva, selezioneEsclusiva, oggettoCustom, template);

            if(readOnly == "SI")
				selezioneEsclusiva.Enabled = false;

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
		public void inserisciContatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
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

            //Imposto il contatore in funzione del formato
            TextBox contatore = new TextBox();
            contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
            if (oggettoCustom.FORMATO_CONTATORE != "")
            {
                contatore.Text = oggettoCustom.FORMATO_CONTATORE;
                if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                {
                    //controllo se il contatore è custom in tal caso visualizzo anno accademico e non il semplice anno solare
                    if (!string.IsNullOrEmpty(oggettoCustom.ANNO_ACC))
                    {
                        string IntervalloDate = oggettoCustom.DATA_INIZIO.Substring(6, 4) + oggettoCustom.DATA_FINE.Substring(5, 5);
                        contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO_ACC);
                    }
                    else
                    { contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO); }
                    contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);
                    string codiceAmministrazione = UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice;
                    contatore.Text = contatore.Text.Replace("COD_AMM", codiceAmministrazione);
                    contatore.Text = contatore.Text.Replace("COD_UO", oggettoCustom.CODICE_DB);
                    contatore.Text = contatore.Text.Replace("gg/mm/aaaa", oggettoCustom.DATA_INSERIMENTO);
                    if ( !string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
                    {
                        Registro reg = UserManager.getRegistroBySistemId(this,oggettoCustom.ID_AOO_RF);
                        if (reg != null)
                        {
                            contatore.Text = contatore.Text.Replace("RF", reg.codRegistro);
                            contatore.Text = contatore.Text.Replace("AOO", reg.codRegistro);
                        }
                    }
                }
                else
                {
                    contatore.Text = "";
                    //contatore.Text = contatore.Text.Replace("ANNO", "");
                    //contatore.Text = contatore.Text.Replace("CONTATORE", "");
                    //contatore.Text = contatore.Text.Replace("RF", "");
                    //contatore.Text = contatore.Text.Replace("AOO", "");
                    //contatore.Text = contatore.Text.Replace("COD_AMM", "");
                    //contatore.Text = contatore.Text.Replace("COD_UO", "");
                    //contatore.Text = contatore.Text.Replace("gg/mm/aaaa", "");
                }
            }
            else
            {
                contatore.Text = oggettoCustom.VALORE_DATABASE;
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloContatore(etichettaContatore, contatore, oggettoCustom, template);

            contatore.Width = Unit.Percentage(60);
			contatore.Enabled = false;
			contatore.BackColor = System.Drawing.Color.WhiteSmoke;
			contatore.CssClass = "comp_profilazione_anteprima";
			contatore.Style.Add("TEXT-ALIGN","right");

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaContatore);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(contatore);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);   			            
		}
		#endregion inserisciContatore

		#region inserisciData
		public void inserisciData(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
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

            DocsPAWA.UserControls.Calendar data = (DocsPAWA.UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data.fromUrl = "../UserControls/DialogCalendar.aspx";
            data.CSS = "testo_grigio";
            data.ID = oggettoCustom.SYSTEM_ID.ToString();
            data.VisibleTimeMode = ProfilazioneFascManager.getVisibleTimeMode(oggettoCustom);

            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                //data.txt_Data.Text = oggettoCustom.VALORE_DATABASE;
                data.Text = oggettoCustom.VALORE_DATABASE;

            if (readOnly == "SI")
            {
                TableRow row = new TableRow();
                TableCell cell_1 = new TableCell();
                cell_1.Controls.Add(etichettaData);
                row.Cells.Add(cell_1);
                TableCell cell_2 = new TableCell();
                cell_2.Controls.Add(data);
                row.Cells.Add(cell_2);
                table.Rows.Add(row);
                //data.txt_Data.ReadOnly = true;
                data.ReadOnly = true;
                data.EnableBtnCal = false;
            }
            else
            {
                TableRow row = new TableRow();
                TableCell cell_1 = new TableCell();
                cell_1.Controls.Add(etichettaData);
                row.Cells.Add(cell_1);
                TableCell cell_2 = new TableCell();
                cell_2.Controls.Add(data);
                row.Cells.Add(cell_2);
                table.Rows.Add(row);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaData, data, oggettoCustom, template);
		}
		#endregion inserisciData	

        #region inserisciCorrispondente
        public void inserisciCorrispondente(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            Label etichetta = new Label();
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            etichetta.Font.Size = FontUnit.Point(8);
            etichetta.Font.Bold = true;
            etichetta.Font.Name = "Verdana";

            DocsPAWA.UserControls.Corrispondente corrispondente = (DocsPAWA.UserControls.Corrispondente)this.LoadControl("../UserControls/Corrispondente.ascx");
            corrispondente.CSS_CODICE = "comp_profilazione_anteprima";
            corrispondente.CSS_DESCRIZIONE = "comp_profilazione_anteprima";
            corrispondente.DESCRIZIONE_READ_ONLY = true;
            corrispondente.TIPO_CORRISPONDENTE = oggettoCustom.TIPO_RICERCA_CORR;
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();
            corrispondente.RICERCA_AJAX = false;

            if (Session["dictionaryCorrispondente"] != null)
                dic_Corr = (Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>)Session["dictionaryCorrispondente"];

            if (dic_Corr != null && dic_Corr.ContainsKey(corrispondente.ID) && dic_Corr[corrispondente.ID] != null)
            {
                corrispondente.SYSTEM_ID_CORR = dic_Corr[corrispondente.ID].systemId;
                corrispondente.CODICE_TEXT = dic_Corr[corrispondente.ID].codiceRubrica;
                corrispondente.DESCRIZIONE_TEXT = dic_Corr[corrispondente.ID].descrizione;
                oggettoCustom.VALORE_DATABASE = dic_Corr[corrispondente.ID].systemId;
            }
            else
            {
                //Da amministrazione è stato impostato un ruolo di default per questo campo.
                if (oggettoCustom.ID_RUOLO_DEFAULT != null && oggettoCustom.ID_RUOLO_DEFAULT != "" && oggettoCustom.ID_RUOLO_DEFAULT != "0")
                {
                    DocsPaWR.Ruolo ruolo = (DocsPaWR.Ruolo)UserManager.getRuoloById(oggettoCustom.ID_RUOLO_DEFAULT, this);
                    if (ruolo != null)
                    {
                        corrispondente.SYSTEM_ID_CORR = ruolo.systemId;
                        corrispondente.CODICE_TEXT = ruolo.codiceRubrica;
                        corrispondente.DESCRIZIONE_TEXT = ruolo.descrizione;
                    }
                    oggettoCustom.ID_RUOLO_DEFAULT = "0";
                }

                //DocsPAWA.DocsPaWR.Corrispondente corr_1 = (DocsPAWA.DocsPaWR.Corrispondente)UserManager.getCorrispondenteBySystemID(this,oggettoCustom.VALORE_DATABASE);

                //Nel caso in cui si debbano vedere dei corrispondenti che risultano ora essere disabilitati, il metodo getCorrispondenteBySystemId
                // ritornerebbe un valore null: per questo, inserisco uun nuovo metodo che riporti anche i corrispondenti disabilitati

                if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                {
                    DocsPAWA.DocsPaWR.Corrispondente corr_1 = (DocsPAWA.DocsPaWR.Corrispondente)UserManager.getCorrispondenteBySystemIDDisabled(this, oggettoCustom.VALORE_DATABASE);
                    if (corr_1 != null)
                    {
                        corrispondente.SYSTEM_ID_CORR = corr_1.systemId;
                        corrispondente.CODICE_TEXT = corr_1.codiceRubrica;
                        corrispondente.DESCRIZIONE_TEXT = corr_1.descrizione;
                        oggettoCustom.VALORE_DATABASE = corr_1.systemId;
                        if (dic_Corr == null)
                            dic_Corr = new Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>();
                        dic_Corr[corrispondente.ID] = corr_1;
                        Session["dictionaryCorrispondente"] = dic_Corr;
                    }
                    //oggettoCustom.VALORE_DATABASE = "";
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
                                dic_Corr = new Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>();
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

                //if (Session["CorrSelezionatoDaMulti"] != null)
                //{
                //    DocsPAWA.DocsPaWR.Corrispondente corr_3 = (DocsPaWR.Corrispondente)Session["CorrSelezionatoDaMulti"];
                //    int idCorrMulti = 0;
                //    if (Session["idCorrMulti"] != null)
                //        idCorrMulti = (int)Session["idCorrMulti"];

                //    if (corr_3 != null && idCorrMulti.ToString().Equals(corrispondente.ID))
                //    {
                //        oggettoCustom.VALORE_DATABASE = corr_3.systemId;
                //        corrispondente.CODICE_TEXT = corr_3.codiceRubrica;
                //        corrispondente.DESCRIZIONE_TEXT = corr_3.descrizione;
                //        if (dic_Corr == null)
                //            dic_Corr = new Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>();
                //        dic_Corr[corrispondente.ID] = corr_3;
                //        oggettoCustom.VALORE_DATABASE = corr_3.systemId;
                //        Session.Remove("CorrSelezionatoDaMulti");
                //        Session.Remove("noDoppiaRicerca");
                //        Session["dictionaryCorrispondente"] = dic_Corr;
                //        Session.Remove("idCorrMulti");
                //    }
                //}
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, corrispondente, oggettoCustom, template);

            if (readOnly == "SI")
                corrispondente.CODICE_READ_ONLY = true;

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

        #region inserisciLink
        public void inserisciLink(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichetta = new Label();
            if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            etichetta.CssClass = "titolo_scheda";
            DocsPAWA.UserControls.LinkDocFasc link = (DocsPAWA.UserControls.LinkDocFasc)this.LoadControl("../UserControls/LinkDocFasc.ascx");
            link.ID = oggettoCustom.SYSTEM_ID.ToString();
            link.TextCssClass = "testo_grigio";
            link.IsEsterno = (oggettoCustom.TIPO_LINK.Equals("ESTERNO"));
            link.IsFascicolo = ("FASCICOLO".Equals(oggettoCustom.TIPO_OBJ_LINK));
            link.HandleInternalDoc = HandleInternalDoc;
            link.HandleInternalFasc = HandleInternalFasc;
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, link, oggettoCustom, template);
            link.Value = oggettoCustom.VALORE_DATABASE;
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

        #region inserisciOggettoEsterno
        public void inserisciOggettoEsterno(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichetta = new Label();
            if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            etichetta.CssClass = "titolo_scheda";
            DocsPaWA.UserControls.IntegrationAdapter intAd = (DocsPaWA.UserControls.IntegrationAdapter)this.LoadControl("../UserControls/IntegrationAdapter.ascx");
            intAd.ID = oggettoCustom.SYSTEM_ID.ToString();
            intAd.View = DocsPaWA.UserControls.IntegrationAdapterView.INSERT_MODIFY;
            intAd.CssClass = "testo_grigio";
            intAd.ManualInsertCssClass = "testo_red";
            intAd.IsFasc = true;
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, intAd, oggettoCustom, template);
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
        #endregion inserisciOggettoEsterno

        #region impostaValori
        private int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
		{
			for(int i=0; i<	ddl.Items.Count; i++)
			{
				if(ddl.Items[i].Text == valore)
					return i;
			}
			return 0;
		}

		private int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
		{
			for(int i=0; i<	rbl.Items.Count; i++)
			{
				if(rbl.Items[i].Text == valore)
					return i;
			}
			return 0;
		}

		private void impostaSelezioneCaselleDiSelezione(DocsPAWA.DocsPaWR.OggettoCustom objCustom, CheckBoxList cbl)
		{
			for(int i=0; i<objCustom.VALORI_SELEZIONATI.Length; i++)
			{
				for(int j=0; j<cbl.Items.Count; j++)
				{
					if( (string) objCustom.VALORI_SELEZIONATI[i] == cbl.Items[j].Text)
					{
						cbl.Items[j].Selected = true;
					}
				}
			}
		}

		private void impostaSelezioneCaselleDiSelezione(string valore, CheckBoxList cbl)
		{
			string [] caselleSelezionate = valore.Split('-');
			for(int i=0; i<caselleSelezionate.Length; i++)
			{
				if(!caselleSelezionate[i].Equals(""))
				{
					for(int j=0; j<cbl.Items.Count; j++)
					{
						if(caselleSelezionate[i] == cbl.Items[j].Text)
						{
							cbl.Items[j].Selected = true;
						}
					}
				}
			}
		}
		#endregion

        #region Button Chiudi
        private void btn_Chiudi_Click(object sender, System.EventArgs e)
		{
			Session.Remove("focus");
			//Session.Remove("modificaProfilazione");
			RegisterStartupScript("chiudi finestra","<script language=\"javascript\">chiudiFinestra();</script>");
     	}
		#endregion Button Chiudi		

        #region imposta diritti ruolo sul campo
        public void impostaDirittiRuoloSulCampo(Object etichetta, Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneFascManager.getDirittiCampoTipologiaFasc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

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
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPAWA.UserControls.Calendar)campo).Visible = false;
                                ((DocsPAWA.UserControls.Calendar)campo).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                                ((DocsPAWA.UserControls.Calendar)campo).btn_Cal.Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Corrispondente":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPAWA.UserControls.Corrispondente)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Link":
                            ((DocsPAWA.UserControls.LinkDocFasc)campo).IsInsertModify = false; 
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPAWA.UserControls.LinkDocFasc)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "OggettoEsterno":
                            ((DocsPaWA.UserControls.IntegrationAdapter)campo).View = DocsPaWA.UserControls.IntegrationAdapterView.READ_ONLY;
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
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

        public void impostaDirittiRuoloSelezioneEsclusiva(Object etichetta, Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneFascManager.getDirittiCampoTipologiaFasc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Visible = false;
                        //((System.Web.UI.HtmlControls.HtmlAnchor)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                }
            }
        }

        public void impostaDirittiRuoloContatore(Object etichettaContatore, Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneFascManager.getDirittiCampoTipologiaFasc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatore).Visible = false;
                        //((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((System.Web.UI.WebControls.TextBox)campo).Visible = false;
                        //((System.Web.UI.WebControls.CheckBox)checkBox).Visible = false;
                        //((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;                
                    }
                }
            }
        }
        #endregion 

        #region delegates per LinkDocFasc
        private void HandleInternalDoc(string idDoc)
        {
                Response.Write("<script language='javascript'>window.returnValue = 'Link_D_"+idDoc+"'; window.close();</script>");
        }

        private void HandleInternalFasc(string idFasc)
        {
            Response.Write("<script language='javascript'>window.returnValue = 'Link_F_" + idFasc + "'; window.close();</script>");
        }
        #endregion delegates per LinkDocFasc
	}
}
