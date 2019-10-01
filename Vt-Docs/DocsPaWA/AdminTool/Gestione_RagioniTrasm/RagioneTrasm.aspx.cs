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
using System.Xml;
using System.IO;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using DocsPaWA.UserControls;

namespace Amministrazione.Gestione_RagioniTrasm
{	
	public class RagioneTrasm : System.Web.UI.Page
	{
		#region WebControls e variabili

		protected System.Web.UI.WebControls.Label lbl_position;
		protected System.Web.UI.WebControls.DataGrid dg_Ragioni;
		protected System.Web.UI.WebControls.Button btn_nuova;
		
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		
		protected System.Web.UI.WebControls.Label lbl_tit;
		protected System.Web.UI.WebControls.TextBox txt_codice;
		protected System.Web.UI.WebControls.Label lbl_cod;
		protected System.Web.UI.WebControls.DropDownList ddl_visibilita;
		protected System.Web.UI.WebControls.DropDownList ddl_tipo;
		protected System.Web.UI.WebControls.DropDownList ddl_diritti;
		protected System.Web.UI.WebControls.DropDownList ddl_destinatario;
		protected System.Web.UI.WebControls.DropDownList ddl_eredita;
		protected System.Web.UI.WebControls.TextBox txt_note;
		protected System.Web.UI.WebControls.Button btn_aggiungi;
		protected System.Web.UI.WebControls.Panel pnl_info;
		protected System.Web.UI.WebControls.ImageButton btn_chiudiPnlInfo;
		protected System.Web.UI.WebControls.DropDownList ddl_notifica;
        protected System.Web.UI.WebControls.ImageButton imgModMsgNotifica;
        protected System.Web.UI.WebControls.DropDownList ddl_cedeDiritti;
        protected System.Web.UI.WebControls.DropDownList ddl_mantieniLettura;
        protected System.Web.UI.WebControls.Panel pnl_cessione;
        protected System.Web.UI.WebControls.Panel pnlPrevedeRisposta;
        protected System.Web.UI.WebControls.Panel pnlTipoAttivita;
        protected System.Web.UI.WebControls.Panel pnlEUnaRisposta;
        protected System.Web.UI.WebControls.Panel pnlTipologia;
        protected System.Web.UI.WebControls.DropDownList DdlTipoAttivita;
        protected System.Web.UI.WebControls.DropDownList ddl_contributo_obbligatorio;
        protected System.Web.UI.WebControls.DropDownList ddlTipologia;
        protected System.Web.UI.WebControls.Panel pnlColonnaVuota;
        protected System.Web.UI.WebControls.Panel pnlClassificazioneObbligatoria;
        protected System.Web.UI.WebControls.DropDownList ddl_classificazioneObbligatoria;

		/// <summary>
		/// Constanti che identificano gli indici delle colonne del datagrid
		/// delle ragioni trasmissoine
		/// </summary>
		private const int GRID_COL_ID=0;
		private const int GRID_COL_CODICE=1;
		private const int GRID_COL_VISIBILE=2;
		private const int GRID_COL_DIRITTO=3;
		private const int GRID_COL_DESTINATARIO=4;
		private const int GRID_COL_DESCRIZIONE=5;
		private const int GRID_COL_SELECT=6;
		protected System.Web.UI.WebControls.DropDownList ddl_tipoRisposta;
		protected System.Web.UI.WebControls.DropDownList ddl_risposta;
		private const int GRID_COL_DELETE=7;
        private const int GRID_COL_SISTEMA = 8;

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
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btn_nuova.Click += new System.EventHandler(this.btn_nuova_Click);
			this.dg_Ragioni.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dg_Ragioni_ItemCreated);
			this.dg_Ragioni.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_Ragioni_ItemCommand);
			this.btn_chiudiPnlInfo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiPnlInfo_Click);
			this.btn_aggiungi.Click += new System.EventHandler(this.btn_aggiungi_Click);
			this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);

            // Se è attiva l'interoperabilità semplificata, viene aggiunta una voce che consente di
            // filtrare per ricevute Interoperabilità semplificata
            if (InteroperabilitaSemplificataManager.IsEnabledSimpInterop)
                this.ddl_tipo.Items.Add(
                    new ListItem(
                        InteroperabilitaSemplificataManager.ChannelDescription,
                        TipiTrasmissioneEnum.ConInteroperabilitaSemplificata.ToString()));

		}

		#endregion

		#region Page Load e caricamento dati

		/// <summary>
		/// Page Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
            Session["AdminBookmark"] = "RagioniTrasmissione";
            
            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
			if(Session.IsNewSession)
			{
				Response.Redirect("../Exit.aspx?FROM=EXPIRED");
			}
			
			AmmUtils.WebServiceLink  ws=new AmmUtils.WebServiceLink();
			if(!ws.CheckSession(Session.SessionID))
			{
				Response.Redirect("../Exit.aspx?FROM=ABORT");
			}
			// ---------------------------------------------------------------

			this.InitializeBusinessRuleControls();

			this.RegisterScrollKeeper("DivDGList");

			if (!IsPostBack)
			{
				this.FillGridRagioniTrasmissione();

				lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"1");
			}
        }

		/// <summary>
		/// SetFocus agli oggetti
		/// </summary>
		/// <param name="ctrl"></param>
		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
		}

		#endregion

		#region Pannello e tasti
		/// <summary>
		/// Tasto di abilitazione all'inserimento di una nuova ragione trasm.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_nuova_Click(object sender, System.EventArgs e)
		{
			this.SetInsertMode();
		}

		/// <summary>
		/// Predisposizione per un nuovo inserimento
		/// </summary>
		private void SetInsertMode()
		{
			this.ClearData();

			pnl_info.Visible = true;
            this.impostaBlocchiRispettoCessione(false);
			btn_aggiungi.Text = "Aggiungi";
			txt_codice.Visible = true;
			lbl_cod.Visible = false;
			txt_codice.Enabled = true;

            // Se è attiva la cessione diritti, vengono visualizzati i pannelli relativi
            if (this.isCessioneAbilitata())
                this.pnl_cessione.Visible = true;

            if (InitConfigurationKeys.GetValue("0", "ENABLE_TASK") != null && InitConfigurationKeys.GetValue("0", "ENABLE_TASK").Equals("1"))
            {
                this.pnlPrevedeRisposta.Visible = false;
                this.pnlEUnaRisposta.Visible = false;
                this.pnlTipoAttivita.Visible = true;
                this.pnlColonnaVuota.Visible = true;
            }
            else
            {
                this.pnlPrevedeRisposta.Visible = true;
                this.pnlEUnaRisposta.Visible = true;
                this.pnlTipoAttivita.Visible = false;
                this.pnlColonnaVuota.Visible = false;
            }

			this.SetFocus(this.txt_codice);
		}

        protected void DdlTipo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ddl_tipo.SelectedValue.Equals(TipiTrasmissioneEnum.ConWorkflow.ToString()))
            {
                this.pnlClassificazioneObbligatoria.Enabled = true;
            }
            else
            {
                this.pnlClassificazioneObbligatoria.Enabled = false;
                this.ddl_classificazioneObbligatoria.SelectedValue = "No";
            }
        }

        protected void DdlTipoAttivita_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (DdlTipoAttivita.SelectedValue.Equals("Si"))
            {
                this.pnlTipologia.Visible = true;
                this.pnlColonnaVuota.Visible = false;
                //Carico le tipologie in Esercizio
                ArrayList listaTemplates = new ArrayList(DocsPAWA.ProfilazioneDocManager.getTemplates(GetIDAmministrazione(), this));
                if (listaTemplates != null && listaTemplates.Count > 0)
                {
                    this.ddlTipologia.Items.Clear();
                    ListItem item = new ListItem();
                    item.Text = string.Empty;
                    item.Value = string.Empty;
                    this.ddlTipologia.Items.Add(item);
                    DocsPAWA.DocsPaWR.Templates template = null;
                    for (int i = 0; i < listaTemplates.Count; i++)
                    {
                        template = ((DocsPAWA.DocsPaWR.Templates)listaTemplates[i]);
                        if (template.IN_ESERCIZIO.Equals("SI") && template.IPER_FASC_DOC.Equals("0"))
                        {
                            item = new ListItem();
                            item.Text = template.DESCRIZIONE;
                            item.Value = template.SYSTEM_ID.ToString();
                            this.ddlTipologia.Items.Add(item);
                        }
                    }
                }
            }
            else
            {
                this.pnlTipologia.Visible = false;
                this.pnlColonnaVuota.Visible = true;
                this.ddlTipologia.SelectedIndex = 0;
                this.ddl_contributo_obbligatorio.SelectedValue = "No";
            }
        }
		/// <summary>
		/// Tasto di inserimento o modifica dati
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void 
            btn_aggiungi_Click(object sender, System.EventArgs e)
		{
			this.Save();
		}

		/// <summary>
		/// Tasto di chiusura pannello
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_chiudiPnlInfo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			//visibilità informazioni
			this.pnl_info.Visible = false;
			this.ClearData();
            this.impostaBlocchiRispettoCessione(false);
		}

		#endregion

		#region DATAGRID

		/// <summary>
		/// Tasti del Datagrid
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void dg_Ragioni_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			switch (e.CommandName)
			{	
				case "Select":
					this.pnl_info.Visible=true;
					//this.SetFocus(this.ddl_notifica);
					this.btn_aggiungi.Text="Modifica";
					this.txt_codice.Visible=false;
					this.lbl_cod.Visible=!this.txt_codice.Visible;

                    DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione = this.GetRagioneTrasmissione(e.Item.Cells[GRID_COL_ID].Text);
					this.BindUI(ragione);

					this.EnableFieldDestinatario(ragione);

					break;
				
				case "Delete":
					this.dg_Ragioni.SelectedIndex=e.Item.ItemIndex;
					string idRagione=this.dg_Ragioni.SelectedItem.Cells[GRID_COL_ID].Text;
					this.CurrentIDRagioneTrasmissione=idRagione;

					this.Delete();
					
					break;
			}
		}

		/// <summary>
		/// Gestione abilitazione / disabilitazione campo destinatario
		/// in base al fatto che la ragione di trasmissione sia selezionata
		/// in amministrazione come ragione predefinita per i destinatari
		/// </summary>
		/// <param name="ragione"></param>
		private void EnableFieldDestinatario(DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione)
		{
			string alertMessage="ATTENZIONE!\\nsi sta modificando una ragione di trasmissione scelta nella sezione HOME per le trasmissioni in automatico.\\n\\nIl campo DESTINATARI non potrà essere modificato.";

			//this.ddl_destinatario.Enabled=true;
			if (ragione.RagionePredefinitaDestinatari || ragione.RagionePredefinitaDestinatariCC)
			{
				this.ddl_destinatario.Enabled=false;

				this.RegisterClientScript("AlertMessage","alert('" + alertMessage + "')");
			}
		}

		/// <summary>
		/// dg_Ragioni_ItemCreated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dg_Ragioni_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
            //e.Item.Cells[GRID_COL_DELETE].Attributes.Add ("onclick","if (!window.confirm('Sei sicuro di voler eliminare questa ragione?')) {return false};");
		}

		/// <summary>
		/// Impostazione scrollkeeper per div
		/// </summary>
		/// <param name="divID"></param>
		private void RegisterScrollKeeper(string divID)
		{
            DocsPAWA.AdminTool.UserControl.ScrollKeeper scrollKeeper = new DocsPAWA.AdminTool.UserControl.ScrollKeeper();
			scrollKeeper.WebControl=divID;
			this.Form1.Controls.Add(scrollKeeper);
		}

		#endregion

		#region Accesso ai dati

		/// <summary>
		/// Caricamento datagrid ragioni trasmissione
		/// </summary>
		private void FillGridRagioniTrasmissione()
		{
            DocsPAWA.DocsPaWR.OrgRagioneTrasmissione[] ragioniTrasmissione = this.GetRagioniTrasmissione();

			DataSet ds=this.CreateDatasetGrid(ragioniTrasmissione);
			
			this.dg_Ragioni.DataSource=ds;
			this.dg_Ragioni.DataBind();
		}

		/// <summary>
		/// Aggiornamento elemento correntemente selezionato del datagrid
		/// </summary>
		/// <param name="ragione"></param>
		private void RefreshGridItem(DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione)
		{
			DataGridItem item=this.dg_Ragioni.SelectedItem;
			
			if (item!=null)
			{
				item.Cells[GRID_COL_ID].Text=ragione.ID;
				item.Cells[GRID_COL_CODICE].Text=ragione.Codice;
				item.Cells[GRID_COL_DESCRIZIONE].Text=ragione.Descrizione;
				item.Cells[GRID_COL_VISIBILE].Text=this.GetDescriptionVisibilita(ragione.Visibilita);
				item.Cells[GRID_COL_DIRITTO].Text=this.GetDescriptionTipoDiritto(ragione.TipoDiritto);
				item.Cells[GRID_COL_DESTINATARIO].Text=this.GetDescriptionTipoDestinatario(ragione.TipoDestinatario);
			}
		}

		/// <summary>
		/// Creazione dataset necessario per effettuare il bind
		/// dei dati delle funzioni nel datagrid 
		/// </summary>
		/// <returns></returns>
		private DataSet CreateDatasetGrid(DocsPAWA.DocsPaWR.OrgRagioneTrasmissione[] ragioni)
		{
			DataSet ds=new DataSet();
			DataTable dt=new DataTable("Ragioni");

			dt.Columns.Add(new DataColumn("ID"));
			dt.Columns.Add(new DataColumn("Codice"));
			dt.Columns.Add(new DataColumn("Descrizione"));
			dt.Columns.Add(new DataColumn("Visibile"));
			dt.Columns.Add(new DataColumn("Diritto"));
			dt.Columns.Add(new DataColumn("Destinatario"));
            dt.Columns.Add(new DataColumn("Sistema"));			

			ds.Tables.Add(dt);

			foreach (DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione in ragioni)
			{
				DataRow row=dt.NewRow();
				
				row["ID"]=ragione.ID;
				row["Codice"]=ragione.Codice;
				row["Descrizione"]=ragione.Descrizione;
				row["Visibile"]=this.GetDescriptionVisibilita(ragione.Visibilita);
				row["Diritto"]=this.GetDescriptionTipoDiritto(ragione.TipoDiritto);
				row["Destinatario"]=this.GetDescriptionTipoDestinatario(ragione.TipoDestinatario);
                row["Sistema"] = this.GetDescriptionDiSistema(ragione.DiSistema);

				dt.Rows.Add(row);
			}

			return ds;
		}

		/// <summary>
		/// Reperimento descrizione visibilità
		/// </summary>
		/// <param name="visibile"></param>
		/// <returns></returns>
		private string GetDescriptionVisibilita(bool visibile)
		{
			return (visibile?"Sì":"No");
		}

		/// <summary>
		/// Reperimento descrizione tipo diritto
		/// </summary>
		/// <param name="tipoDiritto"></param>
		/// <returns></returns>
		private string GetDescriptionTipoDiritto(DocsPAWA.DocsPaWR.TipiDirittiTrasmissioneEnum tipoDiritto)
		{
            //return (tipoDiritto == DocsPAWA.DocsPaWR.TipiDirittiTrasmissioneEnum.Lettura ? "Solo Lettura" : "Lettura e scrittura");
            if (tipoDiritto == DocsPAWA.DocsPaWR.TipiDirittiTrasmissioneEnum.Lettura)
                return "Solo Lettura";
            if (tipoDiritto == DocsPAWA.DocsPaWR.TipiDirittiTrasmissioneEnum.LetturaScrittura)
                return "Lettura e scrittura";

            return "Nessuno";            
        }

		/// <summary>
		/// Reperimento descrizione tipo destinatario
		/// </summary>
		/// <param name="tipoDestinatario"></param>
		/// <returns></returns>
		private string GetDescriptionTipoDestinatario(DocsPAWA.DocsPaWR.TipiDestinatariTrasmissioneEnum tipoDestinatario)
		{
			string retValue="Tutti";

            if (tipoDestinatario == DocsPAWA.DocsPaWR.TipiDestinatariTrasmissioneEnum.SoloSuperiori)
				retValue="Solo superiori";
            else if (tipoDestinatario == DocsPAWA.DocsPaWR.TipiDestinatariTrasmissioneEnum.SoloSottposti)
				retValue="Solo sottoposti";
            else if (tipoDestinatario == DocsPAWA.DocsPaWR.TipiDestinatariTrasmissioneEnum.Parilivello)
				retValue="Pari livello";
			
			return retValue;
		}

        private string GetDescriptionDiSistema(DocsPAWA.DocsPaWR.RagioneDiSistemaEnum ragioneDiSistema)
        {
            string retValue = "0";

            if (ragioneDiSistema == DocsPAWA.DocsPaWR.RagioneDiSistemaEnum.Si)
                retValue = "1";

            return retValue;
        }

		/// <summary>
		/// Reperimento codice amministrazione corrente
		/// </summary>
		/// <returns></returns>
		private string GetCodiceAmministrazione()
		{
			return AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0");
		}

		/// <summary>
		/// Reperimento ID amministrazione corrente
		/// </summary>
		/// <returns></returns>
		private string GetIDAmministrazione()
		{
			Manager.OrganigrammaManager orgManager=new Manager.OrganigrammaManager();
			orgManager.CurrentIDAmm(this.GetCodiceAmministrazione());

			return orgManager.getIDAmministrazione();
		}

		/// <summary>
		/// Reperimento ragioni trasmissione per l'amministrazione corrente
		/// </summary>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.OrgRagioneTrasmissione[] GetRagioniTrasmissione()
		{
			string idAmministrazione=this.GetIDAmministrazione();

			AmmUtils.WebServiceLink ws=new AmmUtils.WebServiceLink();
			return ws.GetRagioniTrasmissione(idAmministrazione);
		}

		/// <summary>
		/// Reperimento ragione trasmissione
		/// </summary>
		/// <param name="idRagione"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.OrgRagioneTrasmissione GetRagioneTrasmissione(string idRagione)
		{
			AmmUtils.WebServiceLink ws=new AmmUtils.WebServiceLink();
			return ws.GetRagioneTrasmissione(idRagione);
		}

		/// <summary>
		/// Inserimento nuova ragione trasmissione
		/// </summary>
		/// <param name="ragione"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.ValidationResultInfo InsertRagioneTrasmissione(ref DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione)
		{
            string idAmm = this.GetIDAmministrazione();
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			return ws.InsertRagioneTrasmissione(ref ragione, idAmm);
		}

		/// <summary>
		/// Aggiornamento nuova ragione trasmissione
		/// </summary>
		/// <param name="ragione"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.ValidationResultInfo UpdateRagioneTrasmissione(ref DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione)
		{
            string idAmm = this.GetIDAmministrazione();
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
			return ws.UpdateRagioneTrasmissione(ref ragione, idAmm);
		}

		/// <summary>
		/// Aggiornamento nuova ragione trasmissione
		/// </summary>
		/// <param name="ragione"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.ValidationResultInfo DeleteRagioneTrasmissione(ref DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione)
		{
			AmmUtils.WebServiceLink ws=new AmmUtils.WebServiceLink();
			return ws.DeleteRagioneTrasmissione(ref ragione);
		}

		/// <summary>
		/// Rimozione valori campi UI
		/// </summary>
		private void ClearData()
		{
            this.AbilitaOpzioni();

			this.dg_Ragioni.SelectedIndex=-1;

			this.CurrentIDRagioneTrasmissione=string.Empty;
			this.txt_codice.Text=string.Empty;
			this.lbl_cod.Text=string.Empty;
			this.txt_note.Text=string.Empty;
            this.SelectComboItem(this.ddl_tipo, DocsPAWA.DocsPaWR.TipiTrasmissioneEnum.SenzaWorkflow.ToString());
            if (DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_FASC_REQUIRED_TIPI_DOC") != null && DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_FASC_REQUIRED_TIPI_DOC").ToString().Equals("1"))
                this.pnlClassificazioneObbligatoria.Visible = true;
            this.pnlClassificazioneObbligatoria.Enabled = false;
            this.ddl_classificazioneObbligatoria.SelectedValue = "No";
            //this.SelectComboItem(this.ddl_diritti, DocsPAWA.DocsPaWR.TipiDirittiTrasmissioneEnum.LetturaScrittura.ToString());
            this.SelectComboItem(this.ddl_diritti, DocsPAWA.DocsPaWR.TipiDirittiTrasmissioneEnum.Nessuno.ToString());
            this.SelectComboItem(this.ddl_notifica, DocsPAWA.DocsPaWR.TipiNotificaTrasmissioneEnum.Nessuna.ToString());
            this.SelectComboItem(this.ddl_destinatario, DocsPAWA.DocsPaWR.TipiDestinatariTrasmissioneEnum.Tutti.ToString());
			this.SelectComboItem(this.ddl_tipoRisposta,"False");
			this.SelectComboItem(this.ddl_visibilita,"False");
			this.SelectComboItem(this.ddl_risposta,"False");
			this.SelectComboItem(this.ddl_eredita,"False");
            this.DdlTipoAttivita.SelectedValue = "No";
            this.DdlTipoAttivita_SelectedIndexChanged(null, null);
            if (this.isCessioneAbilitata())
            {
                this.SelectComboItem(this.ddl_cedeDiritti, DocsPAWA.DocsPaWR.CedeDiritiEnum.No.ToString());
                //
                // Nuova Gestione Diritti - un clear mantiene come default il diritto Nessuno.
                this.SelectComboItemMantieniDiritti(this.ddl_mantieniLettura, false, false);
                // End
                //
                //this.SelectComboItem(this.ddl_mantieniLettura, "False");
            }
		}

		/// <summary>
		/// Associazione dati ragione trasmissione alla UI
		/// </summary>
		private void BindUI(DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione)
		{
			this.ClearData();

			this.CurrentIDRagioneTrasmissione=ragione.ID;
		
			this.txt_codice.Text=ragione.Codice;
			this.lbl_cod.Text=this.txt_codice.Text;
			this.txt_note.Text=ragione.Descrizione;

			this.SelectComboItem(this.ddl_tipo,ragione.Tipo.ToString());
            this.DdlTipo_SelectedIndexChanged(null, null);
            this.ddl_classificazioneObbligatoria.SelectedValue = ragione.ClassificazioneObbligatoria ? "Si" : "No";

			this.SelectComboItem(this.ddl_diritti,ragione.TipoDiritto.ToString());
			this.SelectComboItem(this.ddl_notifica,ragione.TipoNotifica.ToString());
			this.SelectComboItem(this.ddl_destinatario,ragione.TipoDestinatario.ToString());
			this.SelectComboItem(this.ddl_tipoRisposta,ragione.PrevedeRisposta.ToString());
			this.SelectComboItem(this.ddl_visibilita,ragione.Visibilita.ToString());
			this.SelectComboItem(this.ddl_risposta,ragione.Risposta.ToString());
			this.SelectComboItem(this.ddl_eredita,ragione.Eredita.ToString());

            if (this.isCessioneAbilitata())
            {
                this.pnl_cessione.Visible = true;
                this.SelectComboItem(this.ddl_cedeDiritti, ragione.PrevedeCessione.ToString());
                
                //
                // Nuova Logica: ******************************************************************************************* 
                // Se ragione.MantieniLettura == true && ragione.MantieniScrittura == true allora mantiene Scrittura
                // Se ragione.MantieniLettura == true && ragione.MantieniScrittura == false allora mantiene Lettura
                // Se ragione.MantieniLettura == false && ragione.MantieniScrittura == false allora nessun diritto mantenuto
                // *********************************************************************************************************
                this.SelectComboItemMantieniDiritti(this.ddl_mantieniLettura, ragione.MantieniLettura, ragione.MantieniScrittura);
                // End
                //
                
                //this.SelectComboItem(this.ddl_mantieniLettura, ragione.MantieniLettura.ToString());
                this.ddl_cedeDiritti_SelectedIndexChanged(null, null);
            }

            //Gestione Task
            if (InitConfigurationKeys.GetValue("0", "ENABLE_TASK") != null && InitConfigurationKeys.GetValue("0", "ENABLE_TASK").Equals("1"))
            {
                this.pnlPrevedeRisposta.Visible = false;
                this.pnlEUnaRisposta.Visible = false;
                this.pnlTipoAttivita.Visible = true;
                if (ragione.TipoTask)
                {
                    this.DdlTipoAttivita.SelectedValue = "Si";
                    this.DdlTipoAttivita_SelectedIndexChanged(null, null);
                    this.ddlTipologia.SelectedValue = ragione.IdTipoAtto;
                    this.ddl_contributo_obbligatorio.SelectedValue = ragione.ContributoTaskObbligatorio ? "Si" : "No";
                }
                else
                {
                    this.DdlTipoAttivita.SelectedValue = "No";
                    this.DdlTipoAttivita_SelectedIndexChanged(null, null);
                }
            }
            else
            {
                this.pnlPrevedeRisposta.Visible = true;
                this.pnlEUnaRisposta.Visible = true;
                this.pnlTipoAttivita.Visible = false;
                this.pnlColonnaVuota.Visible = false;
            }
            if (CurrentIDRagioneTrasmissione != null && CurrentIDRagioneTrasmissione != String.Empty)
            {
                this.imgModMsgNotifica.Attributes.Add("onclick", "apriGestioneMsgNotifica(" + this.CurrentIDRagioneTrasmissione + "," + this.GetIDAmministrazione() + ");");
            }

            this.impostaBlocchiRispettoRagioniDiSistema(ragione);
		}

		/// <summary>
		/// Aggiornamento dati ragione trasmissione dalla UI
		/// </summary>
		/// <param name="ragione"></param>
		private void RefreshRagioneTrasmissioneFromUI(DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione)
		{
			ragione.ID=this.CurrentIDRagioneTrasmissione;
			ragione.IDAmministrazione=this.GetIDAmministrazione();
			ragione.Codice=this.txt_codice.Text.Trim();
			ragione.Descrizione=this.txt_note.Text.Trim();
			ragione.Tipo=(DocsPAWA.DocsPaWR.TipiTrasmissioneEnum) this.GetEnumValue(this.ddl_tipo,(typeof(DocsPAWA.DocsPaWR.TipiTrasmissioneEnum)));
            ragione.ClassificazioneObbligatoria = this.ddl_classificazioneObbligatoria.SelectedValue.Equals("Si");
			ragione.TipoNotifica=(DocsPAWA.DocsPaWR.TipiNotificaTrasmissioneEnum) this.GetEnumValue(this.ddl_notifica,(typeof(DocsPAWA.DocsPaWR.TipiNotificaTrasmissioneEnum)));
			ragione.TipoDiritto=(DocsPAWA.DocsPaWR.TipiDirittiTrasmissioneEnum) this.GetEnumValue(this.ddl_diritti,(typeof(DocsPAWA.DocsPaWR.TipiDirittiTrasmissioneEnum)));
			ragione.TipoDestinatario=(DocsPAWA.DocsPaWR.TipiDestinatariTrasmissioneEnum) this.GetEnumValue(this.ddl_destinatario,(typeof(DocsPAWA.DocsPaWR.TipiDestinatariTrasmissioneEnum)));
			ragione.Risposta=this.GetBooleanValue(this.ddl_risposta);
			ragione.PrevedeRisposta=this.GetBooleanValue(this.ddl_tipoRisposta);
			ragione.Eredita=this.GetBooleanValue(this.ddl_eredita);
			ragione.Visibilita=this.GetBooleanValue(this.ddl_visibilita);
            ragione.TipoTask = this.DdlTipoAttivita.SelectedValue.Equals("Si");
            ragione.ContributoTaskObbligatorio = this.ddl_contributo_obbligatorio.SelectedValue.Equals("Si");
            ragione.IdTipoAtto = this.ddlTipologia.SelectedValue;
            if (this.isCessioneAbilitata())
            {
                ragione.PrevedeCessione = (DocsPAWA.DocsPaWR.CedeDiritiEnum)this.GetEnumValue(this.ddl_cedeDiritti, (typeof(DocsPAWA.DocsPaWR.CedeDiritiEnum)));
                //
                // Calcolo valori Mantieni Lettura e Mantieni Scrittura da UI
                this.calcolaMantieniLetturaScrittura(this.ddl_mantieniLettura, ref ragione);
                // End
                //
                //ragione.MantieniLettura = this.GetBooleanValue(this.ddl_mantieniLettura);
            }
            else
            {
                ragione.PrevedeCessione = DocsPAWA.DocsPaWR.CedeDiritiEnum.No;
                ragione.MantieniLettura = false;
                //
                // Agginto per Mev Cessione Diritti con Mantieni Diritti: Nessuno/Lettura/Scrittura
                ragione.MantieniScrittura = false;
                // End
                //
            }
		}

        //
        // Calcolo valori Mantieni Lettura e Mantieni Scrittura da UI
        private void calcolaMantieniLetturaScrittura(DropDownList combo, ref DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione) 
        {
            switch (combo.SelectedValue) 
            { 
                case "Nessuno":
                    ragione.MantieniLettura = false;
                    ragione.MantieniScrittura = false;
                    break;

                case "Lettura":
                    ragione.MantieniLettura = true;
                    ragione.MantieniScrittura = false;
                    break;

                case "Scrittura":
                    ragione.MantieniLettura = true;
                    ragione.MantieniScrittura = true;
                    break;
            }
        }
        // End
        //

		private bool GetBooleanValue(DropDownList combo)
		{
			bool retValue=false;

			try
			{
				retValue=Convert.ToBoolean(combo.SelectedValue);
			}
			catch
			{
			}

			return retValue;
		}

		private Enum GetEnumValue(DropDownList combo,Type enumType)
		{
			return (Enum) Enum.Parse(enumType,combo.SelectedValue,true);
		}

		private void SelectComboItem(DropDownList combo,string value)
		{
			combo.SelectedIndex=combo.Items.IndexOf(combo.Items.FindByValue(value));
		}

        //
        // Gestione valore Mantieni Diritti
        // Metodo ausiliario per la selezione del diritto: Nessuno, Lettura, Scrittura
        private void SelectComboItemMantieniDiritti(DropDownList combo, bool isLettura, bool isScrittura)
        {
            if (isLettura && isScrittura) 
                combo.SelectedIndex = combo.Items.IndexOf(combo.Items.FindByValue("Scrittura"));
                
            if(isLettura && !isScrittura)
                combo.SelectedIndex = combo.Items.IndexOf(combo.Items.FindByValue("Lettura"));

            if (!isLettura && !isScrittura)
                combo.SelectedIndex = combo.Items.IndexOf(combo.Items.FindByValue("Nessuno"));
        }
        // End
        //

		/// <summary>
		/// Reperimento id ragione trasmissione corrente
		/// </summary>
		private string CurrentIDRagioneTrasmissione
		{
			get
			{
				if (this.ViewState["CurrentIDRagioneTrasmissione"]!=null)
					return this.ViewState["CurrentIDRagioneTrasmissione"].ToString();
				else
					return string.Empty;
			}
			set
			{
				this.ViewState["CurrentIDRagioneTrasmissione"]=value;
			}
		}

		private bool OnInsertMode()
		{	
			return this.btn_aggiungi.Text.Equals("Aggiungi");
		}

		private void Save()
		{
			bool insertMode=this.OnInsertMode();

            DocsPAWA.DocsPaWR.ValidationResultInfo result = null;
            DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione = new DocsPAWA.DocsPaWR.OrgRagioneTrasmissione();

			this.RefreshRagioneTrasmissioneFromUI(ragione);

			if (insertMode)
				result=this.InsertRagioneTrasmissione(ref ragione);
			else
				result=this.UpdateRagioneTrasmissione(ref ragione);

			if (!result.Value)
			{
				this.ShowValidationMessage(result);
			}
			else if (!insertMode)
			{
				// Aggiornamento
				this.RefreshGridItem(ragione);

				this.ClearData();

				this.pnl_info.Visible=false;
				this.lbl_cod.Visible=true;
                this.impostaBlocchiRispettoCessione(false);
			}
			else
			{
				// Refresh lista ragioni trasmissione
				this.FillGridRagioniTrasmissione();

				// Predisposizione per un nuovo inserimento
				this.SetInsertMode();
			}
		}

		/// <summary>
		/// Cancellazione ragione trasmissione
		/// </summary>
		private void Delete()
		{
            DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione = new DocsPAWA.DocsPaWR.OrgRagioneTrasmissione();
			this.RefreshRagioneTrasmissioneFromUI(ragione);

            DocsPAWA.DocsPaWR.ValidationResultInfo result = this.DeleteRagioneTrasmissione(ref ragione);

			if (result.Value)
			{
				this.FillGridRagioniTrasmissione();

				pnl_info.Visible=false;
				this.ClearData();
                this.impostaBlocchiRispettoCessione(false);
			}
			else
			{
				this.ShowValidationMessage(result);
			}
		}

		private string GetValidationMessage(DocsPAWA.DocsPaWR.ValidationResultInfo validationResult,
			out Control firstInvalidControl,
			out bool warningMessage)
		{
			string retValue=string.Empty;
			bool errorMessage=false;
			firstInvalidControl=null;

			foreach (DocsPAWA.DocsPaWR.BrokenRule rule in validationResult.BrokenRules)
			{
                if (!errorMessage && rule.Level == DocsPAWA.DocsPaWR.BrokenRuleLevelEnum.Error)
					errorMessage=true;

				if (retValue!=string.Empty)
					retValue+="\\n";

				retValue += " - " + rule.Description;

				if (firstInvalidControl==null)
					firstInvalidControl=this.GetBusinessRuleControl(rule.ID);
			}

			if (errorMessage)
				retValue="Sono state riscontrate le seguenti anomalie:\\n\\n" + retValue;
			else
				retValue="Attenzione:\\n\\n" + retValue;

			warningMessage=!errorMessage;
			
			return retValue.Replace("'","\\'");
		}

		/// <summary>
		/// Hashtable businessrules
		/// </summary>
		private Hashtable _businessRuleControls=null;

		/// <summary>
		/// Inizializzazione hashtable per le businessrule:
		/// - Key:		ID della regola di business
		/// - Value:	Controllo della UI contenente il 
		///				dato in conflitto con la regola di business
		/// </summary>
		private void InitializeBusinessRuleControls()
		{
			this._businessRuleControls=new Hashtable();

			this._businessRuleControls.Add("CODICE_RAGIONE",this.txt_codice);
			this._businessRuleControls.Add("DESCRIZIONE_RAGIONE",this.txt_note);
		}

		private Control GetBusinessRuleControl(string idBusinessRule)
		{
			return this._businessRuleControls[idBusinessRule] as Control;
		}

		/// <summary>
		/// Visualizzazione messaggi di validazione
		/// </summary>
		/// <param name="validationResult"></param>
		private void ShowValidationMessage(DocsPAWA.DocsPaWR.ValidationResultInfo validationResult)
		{
			// Visualizzazione delle regole di business non valide
			bool warningMessage;
			Control firstInvalidControl;

			string validationMessage=this.GetValidationMessage(validationResult,out firstInvalidControl,out warningMessage);

			this.RegisterClientScript("ShowValidationMessage","ShowValidationMessage('" + validationMessage + "'," + warningMessage.ToString().ToLower() + ");");

			if (firstInvalidControl!=null)
				this.SetFocus(firstInvalidControl);
		}

		#endregion

		#region Gestione javascript

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}

		#endregion


        private void Page_PreRender(object sender, System.EventArgs e)
        {
            //if (CurrentIDRagioneTrasmissione != null && CurrentIDRagioneTrasmissione != String.Empty)
            //{
            //    this.imgModMsgNotifica.Attributes.Add("onclick", "apriGestioneMsgNotifica(" + this.CurrentIDRagioneTrasmissione + "," + this.GetIDAmministrazione() + ");");
            //}

            //for (int n = 0; n < this.dg_Ragioni.Items.Count; n++)
            //{
            //    if (
            //        this.dg_Ragioni.Items[n].Cells[GRID_COL_CODICE].Text.ToUpper().Equals("NOTIFICA") ||
            //        this.dg_Ragioni.Items[n].Cells[GRID_COL_CODICE].Text.ToUpper().Equals("RIFIUTO") ||
            //        this.dg_Ragioni.Items[n].Cells[GRID_COL_CODICE].Text.ToUpper().StartsWith("INTEROPERABILIT")
            //       )
            //    {
            //        this.dg_Ragioni.Items[n].Cells[GRID_COL_DELETE].Text = "";
            //    }
            //    else
            //    {
            //        this.dg_Ragioni.Items[n].Cells[GRID_COL_DELETE].Attributes.Add("onclick", "if (!window.confirm('Sei sicuro di voler eliminare questa ragione?')) {return false};");
            //    }
            //}

            for (int n = 0; n < this.dg_Ragioni.Items.Count; n++)
            {
                if (this.dg_Ragioni.Items[n].Cells[GRID_COL_SISTEMA].Text.ToUpper().Equals("1"))
                {
                    this.dg_Ragioni.Items[n].Cells[GRID_COL_CODICE].Text = "<b>" + this.dg_Ragioni.Items[n].Cells[GRID_COL_CODICE].Text + "</b>";
                    this.dg_Ragioni.Items[n].Cells[GRID_COL_DELETE].Text = "";
                }
                else
                {
                    this.dg_Ragioni.Items[n].Cells[GRID_COL_DELETE].Attributes.Add("onclick", "if (!window.confirm('Sei sicuro di voler eliminare la ragione " + this.dg_Ragioni.Items[n].Cells[GRID_COL_CODICE].Text + "?')) {return false};");
                }
            }
        }

        private bool isCessioneAbilitata()
        {
            return (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["GEST_RAG_TRASM_CESSIONE"]) && System.Configuration.ConfigurationManager.AppSettings["GEST_RAG_TRASM_CESSIONE"].Equals("1"));
        }

        protected void ddl_cedeDiritti_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            bool isCessione = (!this.ddl_cedeDiritti.SelectedValue.Equals("No"));
            this.impostaBlocchiRispettoCessione(isCessione);
        }

        private void impostaBlocchiRispettoCessione(bool isCessione)
        {
            if (isCessione)
            {
                // tipo = senza workflow
                this.ddl_tipo.SelectedIndex = 0;
                this.ddl_tipo.Enabled = false;

                // è risposta = no
                this.ddl_risposta.SelectedIndex = 1;
                this.ddl_risposta.Enabled = false;

                // prevede risposta = no
                this.ddl_tipoRisposta.SelectedIndex = 0;
                this.ddl_tipoRisposta.Enabled = false;

                // eredita = no
                this.ddl_eredita.SelectedIndex = 0;
                this.ddl_eredita.Enabled = false;

                // diritti = lettura e scrittura
                this.ddl_diritti.SelectedIndex = 1;
                this.ddl_diritti.Enabled = false;
                
                //Mantieni lettura
                this.ddl_mantieniLettura.Enabled = true;
            }
            else
            {
                // tipo
                this.ddl_tipo.Enabled = true;

                // è risposta
                this.ddl_risposta.Enabled = true;

                // prevede risposta
                this.ddl_tipoRisposta.Enabled = true;

                // eredita
                this.ddl_eredita.Enabled = true; ;

                // diritti = lettura e scrittura
                this.ddl_diritti.Enabled = true;

                //Mantieni lettura
                this.ddl_mantieniLettura.SelectedIndex = 0;
                this.ddl_mantieniLettura.Enabled = false;
            }
        }

        private void impostaBlocchiRispettoRagioniDiSistema(DocsPAWA.DocsPaWR.OrgRagioneTrasmissione ragione)
        {
            //if (
            //    ragione.Codice.ToUpper().Equals("NOTIFICA") ||
            //    ragione.Codice.ToUpper().Equals("RIFIUTO")                
            //    )
            //{
            //    // tipo = senza workflow
            //    this.ddl_tipo.SelectedIndex = 0;
            //    this.ddl_tipo.Enabled = false;

            //    if (isCessioneAbilitata())
            //        this.pnl_cessione.Visible = false; 
            //}

            //if (ragione.Codice.ToUpper().StartsWith("INTEROPERABILIT"))
            //{
            //    // tipo = interoperabilita
            //    this.ddl_tipo.SelectedIndex = 2;
            //    this.ddl_tipo.Enabled = false;

            //    if (isCessioneAbilitata())
            //        this.pnl_cessione.Visible = false;               
            //}

            if (ragione.DiSistema.Equals(DocsPAWA.DocsPaWR.RagioneDiSistemaEnum.Si))
                this.DisabilitaOpzioni(); 
        }

        private void DisabilitaOpzioni()
        {
            this.txt_note.Enabled = false;
            this.ddl_tipo.Enabled = false;
            this.ddl_diritti.Enabled = false;
            this.ddl_notifica.Enabled = false;
            this.ddl_destinatario.Enabled = false;
            this.ddl_tipoRisposta.Enabled = false;
            this.ddl_visibilita.Enabled = false;
            this.ddl_risposta.Enabled = false;
            this.ddl_eredita.Enabled = false;
            this.DdlTipoAttivita.Enabled = false;
            this.ddlTipologia.Enabled = false;
            if (this.isCessioneAbilitata())
            {
                this.ddl_cedeDiritti.Enabled = false;
                this.ddl_mantieniLettura.Enabled = false;
            }
            this.imgModMsgNotifica.Enabled = false;

            this.btn_aggiungi.Enabled = false;
        }


        private void AbilitaOpzioni()
        {
            this.txt_note.Enabled = true;
            this.ddl_tipo.Enabled = true;
            this.ddl_diritti.Enabled = true;
            this.ddl_notifica.Enabled = true;
            this.ddl_destinatario.Enabled = true;
            this.ddl_tipoRisposta.Enabled = true;
            this.ddl_visibilita.Enabled = true;
            this.ddl_risposta.Enabled = true;
            this.ddl_eredita.Enabled = true;
            if (this.isCessioneAbilitata())
            {
                this.ddl_cedeDiritti.Enabled = true;
                this.ddl_mantieniLettura.Enabled = false;
            }
            this.imgModMsgNotifica.Enabled = true;



            this.btn_aggiungi.Enabled = true;
        }
	}
}
