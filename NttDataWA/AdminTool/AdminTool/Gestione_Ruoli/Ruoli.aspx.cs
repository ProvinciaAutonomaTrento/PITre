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
using System.IO;
using System.Xml;
using DocsPaWR = SAAdminTool.DocsPaWR;

namespace Amministrazione.Gestione_Ruoli
{
	/// <summary>
	/// Summary description for Ruoli.
	/// </summary>
	public class Ruoli : System.Web.UI.Page
	{		
		#region WebControls e variabili

		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.WebControls.Label lbl_position;
		protected System.Web.UI.WebControls.DataGrid dg_ruoli;
		protected System.Web.UI.WebControls.Button btn_nuovoRuolo;
		protected System.Web.UI.WebControls.TextBox txt_codice;
		protected System.Web.UI.WebControls.TextBox txt_descrizione;
		protected System.Web.UI.WebControls.Panel pnl_info;
		protected System.Web.UI.WebControls.Button btn_salva;
		protected System.Web.UI.WebControls.TextBox txt_livello;
        protected SAAdminTool.DocsPaWR.InfoUtenteAmministratore datiAmministratore;

		//------------------------------------------------------------------------------------------------------
		protected System.Web.UI.WebControls.Label lbl_cod;
		protected System.Web.UI.WebControls.Label lbl_tit;
		protected System.Web.UI.WebControls.ImageButton btn_chiudiPnlInfo;
		protected System.Web.UI.WebControls.Button btn_utenti;
		

		/// <summary>
		/// Costanti che identificano i nomi delle colonne del dataset 
		/// utilizzato per caricare i dati nel datagrid
		/// </summary>
		private const string TABLE_COL_ID="ID";
		private const string TABLE_COL_CODICE="Codice";
		private const string TABLE_COL_DESCRIZIONE="Descrizione";
		private const string TABLE_COL_LIVELLO="Livello";

		/// <summary>
		/// Costanti che identificano i nomi delle colonne del datagrid
		/// </summary>
		private const int GRID_COL_ID=0;
		private const int GRID_COL_CODICE=1;
		private const int GRID_COL_DESCRIZIONE=2;
		private const int GRID_COL_LIVELLO=3;
		private const int GRID_COL_DETAIL=4;
		private const int GRID_COL_DELETE=5;
        
		#endregion

		#region Form_Load
		private void Page_Load(object sender, System.EventArgs e)
		{
            Session["AdminBookmark"] = "TipiRuolo";

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

			// Inizializzazione hashtable businessrules
			this.InitializeBusinessRuleControls();

			if (!IsPostBack)
			{				
				this.AddControlsClientAttribute();

				lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"1");										

				// Caricamento lista tipi ruolo
				this.FillListTipiRuolo();
			}
        }
		#endregion		

		#region dg_ruoli

		private DataView OrdinaGrid(DataView dv, string sortColumn)
		{
			string sortMode = " ASC";
			dv.Sort = sortColumn + sortMode;
			return dv;
		}

		private void dg_ruoli_ItemCommand(object source, DataGridCommandEventArgs e)
		{			
			pnl_info.Visible=false;
			
			ViewState["riga"] = e.Item.DataSetIndex;

			switch(e.CommandName) 
			{
				case "Select":
					pnl_info.Visible=true;

					lbl_cod.Visible=true;
					txt_codice.Visible=false;
                    if (SAAdminTool.Utils.GetAbilitazioneAtipicita())
                        txt_livello.Enabled = false;
                    else
                        txt_livello.Enabled = true;
                    
					btn_salva.Text="Modifica";

					SetFocus(txt_descrizione);

					//VerificaUtenti(e.Item.Cells[0].Text);
					this.btn_utenti.Visible=true;		

                    // Se è attiva l'atipicità, non si può modificare il livello del ruolo
                    this.txt_livello.Enabled = !SAAdminTool.Utils.GetAbilitazioneAtipicita();
		
					DocsPaWR.OrgTipoRuolo ruolo=this.GetTipoRuolo(e.Item.Cells[GRID_COL_ID].Text);
					this.BindUI(ruolo);

					break;

				case "Delete":
					this.dg_ruoli.SelectedIndex=e.Item.ItemIndex;
					string idTipoRuolo=this.dg_ruoli.SelectedItem.Cells[GRID_COL_ID].Text;
					this.CurrentIDTipoRuolo=Convert.ToInt32(idTipoRuolo);

					this.Delete();
					
					break;
			}
		}

		private void dg_ruoli_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{			
			e.Item.Cells[GRID_COL_DELETE].Attributes.Add ("onclick","if (!window.confirm('Sei sicuro di voler eliminare questo ruolo?')) {return false};");
		}

		#endregion

		#region pannello info
		
		/// <summary>
		/// Bottone Salva del pannello info
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_salva_Click(object sender, System.EventArgs e)
		{
			this.Save();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_nuovoRuolo_Click(object sender, System.EventArgs e)
		{			
			this.SetInsertMode();
		}
		
		#endregion
		
		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
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
			this.btn_nuovoRuolo.Click += new System.EventHandler(this.btn_nuovoRuolo_Click);
			this.dg_ruoli.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dg_ruoli_ItemCreated);
			this.dg_ruoli.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_ruoli_ItemCommand);
			this.btn_chiudiPnlInfo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiPnlInfo_Click);
			this.btn_utenti.Click += new System.EventHandler(this.btn_utenti_Click);
			this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btn_chiudiPnlInfo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			txt_codice.Text="";
			txt_descrizione.Text="";
			txt_livello.Text="";
			pnl_info.Visible=false;
			
			dg_ruoli.SelectedIndex=-1;
		}

		private void btn_utenti_Click(object sender, System.EventArgs e)
		{
			if(this.lbl_cod.Text!=null && this.lbl_cod.Text!=string.Empty)
			{
				string codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"0");
				string codRuolo = this.lbl_cod.Text;
				string descRuolo = this.txt_descrizione.Text;

				if(!this.Page.IsStartupScriptRegistered("openModal"))
				{					
					string scriptString = "<SCRIPT>ApriGestioneUtenti('" + codAmm + "','" + codRuolo.Replace("'","\\'") + "','" + descRuolo.Replace("'","\\'")+ "');</SCRIPT>";				
					this.Page.RegisterStartupScript("openModal", scriptString);
				}
			}
		}

		#region Gestione accesso ai dati

		/// <summary>
		/// Reperimento id tipo ruolo corrente
		/// </summary>
		private int CurrentIDTipoRuolo
		{
			get
			{
				if (this.ViewState["CurrentIDTipoRuolo"]!=null)
					return Convert.ToInt32(this.ViewState["CurrentIDTipoRuolo"]);
				else
					return 0;
			}
			set
			{
				this.ViewState["CurrentIDTipoRuolo"]=value;
			}
		}

		/// <summary>
		/// Reperimento dei tipi ruolo in amministrazione corrente
		/// </summary>
		/// <returns></returns>
		private SAAdminTool.DocsPaWR.OrgTipoRuolo[] GetTipiRuolo()
		{
			string codiceAmministrazione=this.GetCodiceAmministrazione();
			return this.GetTipiRuolo(codiceAmministrazione);
		}

		/// <summary>
		/// Reperimento dei tipi ruolo
		/// </summary>
		/// <param name="codiceAmministrazione"></param>
		/// <returns></returns>
		private SAAdminTool.DocsPaWR.OrgTipoRuolo[] GetTipiRuolo(string codiceAmministrazione)
		{
			DocsPaWR.DocsPaWebService ws=new SAAdminTool.DocsPaWR.DocsPaWebService();
			return ws.AmmGetTipiRuolo(codiceAmministrazione);
		}

		/// <summary>
		/// Reperimento di un tipo ruolo
		/// </summary>
		/// <param name="idTipoRuolo"></param>
		/// <returns></returns>
		private SAAdminTool.DocsPaWR.OrgTipoRuolo GetTipoRuolo(string idTipoRuolo)
		{
			DocsPaWR.DocsPaWebService ws=new SAAdminTool.DocsPaWR.DocsPaWebService();
			return ws.AmmGetTipoRuolo(idTipoRuolo);
		}

		/// <summary>
		/// Inserimento di un nuovo tipo ruolo
		/// </summary>
		/// <param name="tipoRuolo"></param>
		/// <returns></returns>
		private SAAdminTool.DocsPaWR.ValidationResultInfo InsertTipoRuolo(ref SAAdminTool.DocsPaWR.OrgTipoRuolo tipoRuolo)
		{
            SAAdminTool.AdminTool.Manager.SessionManager session = new SAAdminTool.AdminTool.Manager.SessionManager();
            datiAmministratore = session.getUserAmmSession();
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

            DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
			return ws.AmmInsertTipoRuolo(datiAmministratore, ref tipoRuolo, idAmm);
		}

		/// <summary>
		/// Aggiornamento di un tipo ruolo
		/// </summary>
		/// <param name="tipoRuolo"></param>
		/// <returns></returns>
		private SAAdminTool.DocsPaWR.ValidationResultInfo UpdateTipoRuolo(ref SAAdminTool.DocsPaWR.OrgTipoRuolo tipoRuolo)
		{
			DocsPaWR.DocsPaWebService ws=new SAAdminTool.DocsPaWR.DocsPaWebService();
			return ws.AmmUpdateTipoRuolo(ref tipoRuolo);
		}

		/// <summary>
		/// Cancellazione di un tipo ruolo
		/// </summary>
		/// <param name="tipoRuolo"></param>
		/// <returns></returns>
		private SAAdminTool.DocsPaWR.ValidationResultInfo DeleteTipoRuolo(ref SAAdminTool.DocsPaWR.OrgTipoRuolo tipoRuolo)
		{
            SAAdminTool.AdminTool.Manager.SessionManager session = new SAAdminTool.AdminTool.Manager.SessionManager();
            datiAmministratore = session.getUserAmmSession();
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
			DocsPaWR.DocsPaWebService ws=new SAAdminTool.DocsPaWR.DocsPaWebService();
			return ws.AmmDeleteTipoRuolo(datiAmministratore, ref tipoRuolo, idAmm);
		}

		/// <summary>
		/// Associazione dati del tipo ruolo ai campi della UI
		/// </summary>
		/// <param name="tipoRuolo"></param>
		private void BindUI(SAAdminTool.DocsPaWR.OrgTipoRuolo tipoRuolo)
		{
			this.ClearData();

			this.CurrentIDTipoRuolo=Convert.ToInt32(tipoRuolo.IDTipoRuolo);
			this.txt_codice.Text=tipoRuolo.Codice;
			this.lbl_cod.Text=this.txt_codice.Text;
			this.txt_descrizione.Text=tipoRuolo.Descrizione;
			this.txt_livello.Text=tipoRuolo.Livello;
		}

		/// <summary>
		/// Aggiornamento del tipo ruolo dai dati dei campi della UI
		/// </summary>
		private void RefreshTipoRuoloFromUI(SAAdminTool.DocsPaWR.OrgTipoRuolo tipoRuolo)
		{
			tipoRuolo.IDTipoRuolo=this.CurrentIDTipoRuolo.ToString();
			tipoRuolo.Codice=this.txt_codice.Text.Trim();
			tipoRuolo.Descrizione=this.txt_descrizione.Text.Trim();
			tipoRuolo.Livello=this.txt_livello.Text.Trim();
		}

		/// <summary>
		/// Rimozione dati UI
		/// </summary>
		private void ClearData()
		{
			this.CurrentIDTipoRuolo=0;
			this.txt_codice.Text=string.Empty;
			this.lbl_cod.Text=string.Empty;
			this.txt_descrizione.Text=string.Empty;
			this.txt_livello.Text=string.Empty;
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
		/// Impostazione dell'id dell'amministrazione nel tipo ruolo
		/// </summary>
		/// <param name="tipoRuolo"></param>
		private void SetIdAmministrazione(SAAdminTool.DocsPaWR.OrgTipoRuolo tipoRuolo)
		{
			Manager.OrganigrammaManager orgManager=new Manager.OrganigrammaManager();
			orgManager.CurrentIDAmm(this.GetCodiceAmministrazione());

			tipoRuolo.IDAmministrazione=orgManager.getIDAmministrazione();
		}

		/// <summary>
		/// Salvataggio dati del tipo ruolo corrente
		/// </summary>
		private void Save()
		{
			DocsPaWR.OrgTipoRuolo tipoRuolo=new SAAdminTool.DocsPaWR.OrgTipoRuolo();
			this.RefreshTipoRuoloFromUI(tipoRuolo);

			DocsPaWR.ValidationResultInfo result=null;

			this.SetIdAmministrazione(tipoRuolo);

			bool insertMode=this.OnInsertMode();

			if (insertMode)
				result=this.InsertTipoRuolo(ref tipoRuolo);
			else
				result=this.UpdateTipoRuolo(ref tipoRuolo);

			if (!result.Value)
			{
				this.ShowValidationMessage(result);
			}
			else if (!insertMode)
			{
				// Aggiornamento
				this.RefreshGridItem(tipoRuolo);

				this.ClearData();

				this.pnl_info.Visible=false;
				this.lbl_tit.Visible=true;

				this.dg_ruoli.SelectedIndex=-1;	
			}
			else
			{
				// Inserimento
				this.lbl_tit.Visible=true;

				// Refresh lista registri
				this.FillListTipiRuolo();

				// Predisposizione per un nuovo inserimento
				this.SetInsertMode();
			}
		}

		private string GetValidationMessage(SAAdminTool.DocsPaWR.ValidationResultInfo validationResult,
											out Control firstInvalidControl,
											out bool warningMessage)
		{
			string retValue=string.Empty;
			bool errorMessage=false;
			firstInvalidControl=null;

			foreach (SAAdminTool.DocsPaWR.BrokenRule rule in validationResult.BrokenRules)
			{
				if (!errorMessage && rule.Level==DocsPaWR.BrokenRuleLevelEnum.Error)
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

			this._businessRuleControls.Add("CODICE_TIPO_RUOLO",this.txt_codice);
			this._businessRuleControls.Add("DESCRIZIONE_TIPO_RUOLO",this.txt_descrizione);
			this._businessRuleControls.Add("LIVELLO_TIPO_RUOLO",this.txt_livello);
		}

		private Control GetBusinessRuleControl(string idBusinessRule)
		{
			return this._businessRuleControls[idBusinessRule] as Control;
		}

		/// <summary>
		/// Visualizzazione messaggi di validazione
		/// </summary>
		/// <param name="validationResult"></param>
		private void ShowValidationMessage(SAAdminTool.DocsPaWR.ValidationResultInfo validationResult)
		{
			// Visualizzazione delle regole di business non valide
			bool warningMessage;
			Control firstInvalidControl;

			string validationMessage=this.GetValidationMessage(validationResult,out firstInvalidControl,out warningMessage);

			this.RegisterClientScript("ShowValidationMessage","ShowValidationMessage('" + validationMessage + "'," + warningMessage.ToString().ToLower() + ");");

			if (firstInvalidControl!=null)
				this.SetFocus(firstInvalidControl);
		}

		/// <summary>
		/// Verifica se si è in fase di inserimento
		/// </summary>
		/// <returns></returns>
		private bool OnInsertMode()
		{
			return (btn_salva.Text.Equals("Aggiungi"));
		}

		/// <summary>
		/// Gestione caricamento lista tipi ruolo
		/// </summary>
		private void FillListTipiRuolo()
		{
			DocsPaWR.OrgTipoRuolo[] tipiRuolo=this.GetTipiRuolo();
			DataSet ds=this.ConvertToDataSet(tipiRuolo);

			DataView dv=ds.Tables["TipiRuolo"].DefaultView;
			dv.Sort=TABLE_COL_LIVELLO + " ASC";
			
			this.dg_ruoli.DataSource=dv;
			this.dg_ruoli.DataBind();

			ds.Dispose();
			ds=null;

			tipiRuolo=null;			
		}

		/// <summary>
		/// Aggiornamento elemento griglia corrente
		/// </summary>
		/// <param name="registro"></param>
		private void RefreshGridItem(SAAdminTool.DocsPaWR.OrgTipoRuolo tipoRuolo)
		{
			DataGridItem item=this.dg_ruoli.SelectedItem;
			
			if (item!=null)
			{
				item.Cells[GRID_COL_DESCRIZIONE].Text=tipoRuolo.Descrizione;
				item.Cells[GRID_COL_LIVELLO].Text=tipoRuolo.Livello;
			}
		}

		/// <summary>
		/// Conversione array
		/// </summary>
		/// <param name="registri"></param>
		/// <returns></returns>
		private DataSet ConvertToDataSet(SAAdminTool.DocsPaWR.OrgTipoRuolo[] tipiRuolo)
		{
			DataSet ds=this.CreateGridDataSet();
			DataTable dt=ds.Tables["TipiRuolo"];

			foreach (SAAdminTool.DocsPaWR.OrgTipoRuolo tipoRuolo in tipiRuolo)
			{
				DataRow row=dt.NewRow();
				
				row[TABLE_COL_ID]=tipoRuolo.IDTipoRuolo;
				row[TABLE_COL_CODICE]=tipoRuolo.Codice;
				row[TABLE_COL_DESCRIZIONE]=tipoRuolo.Descrizione;
				row[TABLE_COL_LIVELLO]=tipoRuolo.Livello;

				dt.Rows.Add(row);
			}

			return ds;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private DataSet CreateGridDataSet()
		{
			DataSet ds=new DataSet();
			DataTable dt=new DataTable("TipiRuolo");

			dt.Columns.Add(new DataColumn(TABLE_COL_ID));
			dt.Columns.Add(new DataColumn(TABLE_COL_CODICE));
			dt.Columns.Add(new DataColumn(TABLE_COL_DESCRIZIONE));
			dt.Columns.Add(new DataColumn(TABLE_COL_LIVELLO,typeof(int)));

			ds.Tables.Add(dt);
			return ds;
		}

		/// <summary>
		/// Predisposizione per l'inserimento di un nuovo registro
		/// </summary>
		private void SetInsertMode()
		{
			// Rimozione dati controlli UI
			this.ClearData();

			//visibilità informazioni
			pnl_info.Visible		= true;
			txt_codice.Visible		= true;
			lbl_cod.Visible			= false;
			btn_salva.Text			= "Aggiungi";
            txt_livello.Enabled     = true;

            this.dg_ruoli.SelectedIndex=-1;

			SetFocus(txt_codice);

			this.btn_utenti.Visible=false;
		}

		/// <summary>
		/// Cancellazione registro
		/// </summary>
		private void Delete()
		{
			DocsPaWR.OrgTipoRuolo tipoRuolo=new SAAdminTool.DocsPaWR.OrgTipoRuolo();
			this.RefreshTipoRuoloFromUI(tipoRuolo);

			DocsPaWR.ValidationResultInfo result=this.DeleteTipoRuolo(ref tipoRuolo);

			if (result.Value)
			{
				this.FillListTipiRuolo();

				pnl_info.Visible=false;

				this.ClearData();

				dg_ruoli.SelectedIndex=-1;
			}
			else
			{
				this.ShowValidationMessage(result);
			}
		}

		#endregion

		#region Gestione javascript

		private void AddControlsClientAttribute()
		{
			this.txt_livello.Attributes.Add("onkeypress","ValidateNumericKey();");
		}

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
	}
}
