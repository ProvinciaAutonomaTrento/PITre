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
using System.Linq;
namespace Amministrazione.Gestione_Funzioni
{
	/// <summary>
	/// Summary description for TipiFunzione.
	/// </summary>
	public class TipiFunzione : System.Web.UI.Page
	{
		#region WebControls e variabili
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.WebControls.Label lbl_position;		
		protected System.Web.UI.WebControls.DataGrid dg_macroFunzioni;
		protected System.Web.UI.WebControls.Button btn_nuova;
		protected System.Web.UI.WebControls.Panel pnl_info;
		protected System.Web.UI.WebControls.TextBox txt_codice;
		protected System.Web.UI.WebControls.Label lbl_cod;
		protected System.Web.UI.WebControls.TextBox txt_descrizione;
		protected System.Web.UI.WebControls.DataGrid dg_funzioni;
		protected System.Web.UI.WebControls.Button btn_aggiungi;		
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.HtmlControls.HtmlTableCell funzInUO;
		protected System.Web.UI.WebControls.Label lbl_tit;
        protected System.Web.UI.WebControls.Button btn_find;
        protected System.Web.UI.WebControls.TextBox txt_ricerca;
        protected System.Web.UI.WebControls.DropDownList ddl_ricTipo;
		// ---------------------------------------------------------------------------------------
		protected System.Web.UI.WebControls.ImageButton btn_chiudiPnlInfo;
		protected System.Web.UI.WebControls.Panel pnlGrid;
		protected System.Web.UI.WebControls.Button btn_aggiungiTutti; 
		// ---------------------------------------------------------------------------------------
		#endregion

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            if (ddl_ricTipo.SelectedIndex == 0)
            {
                txt_ricerca.Enabled = false;
            }
        
        }
		#region Page_Load

		private void Page_Load(object sender, System.EventArgs e)
		{
            Session["AdminBookmark"] = "TipiFunzione";
            
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

			this.RegisterScrollKeeper("DivDGList");
			
			this.InitializeBusinessRuleControls();
           
			if (!IsPostBack)

			{
                DocsPAWA.Utils.DefaultButton(this, ref this.txt_ricerca, ref this.btn_find);
                ViewState["dsMicroFunzioni"] = null;
                ViewState["tipoFunzione"] = null;
                ViewState["selectedFuncionForExport"] = null;
                ddl_ricTipo.SelectedIndex = 0;
             
				this.FillListTipiFunzione();

                ddl_ricTipo.Attributes.Add("onChange", "checkRicerca();");
                btn_find.Attributes.Add("onclick", "checkValTxtRicerca();");
				lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"1");
			}
            if (ddl_ricTipo.SelectedIndex > -1)
            {
                txt_ricerca.Enabled = !(ddl_ricTipo.SelectedValue == "T" || ddl_ricTipo.SelectedValue == "SEL");
               
                if (!txt_ricerca.Enabled)
                {
                    txt_ricerca.BackColor = Color.Gainsboro;
                }
                else
                {
                    txt_ricerca.BackColor = Color.White;
                }
            }
          


        }

		/// <summary>
		/// 
		/// </summary>
		private void RegisterScrollKeeper(string divID)
		{
            DocsPAWA.AdminTool.UserControl.ScrollKeeper scrollKeeper = new DocsPAWA.AdminTool.UserControl.ScrollKeeper();
			scrollKeeper.WebControl=divID;
			this.Form1.Controls.Add(scrollKeeper);
		}

		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
            if (!this.DesignMode)
            {
                if (Context != null && Context.Session != null && Session != null)
                {
                    InitializeComponent();
                    base.OnInit(e);
                }
            }
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btn_nuova.Click += new System.EventHandler(this.btn_nuova_Click);
			this.dg_macroFunzioni.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dg_macroFunzioni_ItemCreated);
			this.dg_macroFunzioni.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_macroFunzioni_ItemCommand);

            this.dg_funzioni.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_funzioni_ItemCommand);
           
			this.btn_chiudiPnlInfo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiPnlInfo_Click);
			this.btn_aggiungiTutti.Click += new System.EventHandler(this.btn_aggiungiTutti_Click);
			this.btn_aggiungi.Click += new System.EventHandler(this.btn_aggiungi_Click);
            //this.btn_find.Click += new System.EventHandler(this.btn_find_Click);
            //this.ddl_ricTipo.SelectedIndexChanged += new System.EventHandler(this.ddl_ricTipo_SelectedIndexChanged);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);
            

		}
		#endregion

		#region dg_macroFunzioni
        
		private void dg_macroFunzioni_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{			
			e.Item.Cells[GRID_TIPI_FUNZIONE_COL_DELETE].Attributes.Add ("onclick","if (!window.confirm('Sei sicuro di voler eliminare questa funzione?')) {return false};");
		}

        private void resetFiltriricerca()
        {
            ViewState["dsMicroFunzioni"] = null;
            
            ddl_ricTipo.SelectedIndex = 0;
            txt_ricerca.Text = "";
        }
		private void dg_macroFunzioni_ItemCommand(object source, DataGridCommandEventArgs e)
		{
            resetFiltriricerca();
            
			if (e.CommandName.Equals("Select"))
			{	
				this.dg_funzioni.Visible=true;
				this.RegisterScrollKeeper("divGridFunzioni");
                
                DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione = this.GetTipoFunzione(e.Item.Cells[GRID_TIPI_FUNZIONE_COL_ID].Text);
                //ViewState["tipoFunzione"] = tipoFunzione;
                ViewState.Add("tipoFunzione",tipoFunzione);
                this.BindUI(tipoFunzione);
			}
			else if (e.CommandName.Equals("Delete"))
			{
				this.dg_macroFunzioni.SelectedIndex=e.Item.ItemIndex;
				this.CurrentIDTipoFunzione=this.dg_macroFunzioni.SelectedItem.Cells[GRID_TIPI_FUNZIONE_COL_ID].Text;
				
				this.Delete();
			}
		}

		#endregion

		#region pannello Info

       


		

		private void btn_nuova_Click(object sender, System.EventArgs e)
		{
			this.SetInsertMode();
		}

		#endregion

		private void btn_chiudiPnlInfo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			//visibilità informazioni
			pnl_info.Visible = false;
			dg_macroFunzioni.SelectedIndex = -1;
		}

		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
		}

		private void btn_aggiungiTutti_Click(object sender, System.EventArgs e)
		{
			this.SelectAllFunctions(true);
		}

		/// <summary>
		/// Selezione / deselezione checkbox selezione funzioni elementari
		/// </summary>
		/// <param name="checkValue"></param>
		private void SelectAllFunctions(bool checkValue)
		{
			foreach (DataGridItem item in this.dg_funzioni.Items)
			{		
				CheckBox checkBox=this.GetCheckBoxAssociazioneFunzione(item);

				if (checkBox!=null)
					checkBox.Checked=checkValue;
			}
		}

		#region Gestione accesso ai dati

		/// <summary>
		/// Reperimento id tipo funzione corrente
		/// </summary>
		private string CurrentIDTipoFunzione
		{
			get
			{
				if (this.ViewState["CurrentIDTipoFunzione"]!=null)
					return this.ViewState["CurrentIDTipoFunzione"].ToString();
				else
					return string.Empty;
			}
			set
			{
				this.ViewState["CurrentIDTipoFunzione"]=value;
			}
		}


		/// <summary>
		/// Reperimento dei tipi funzione
		/// </summary>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.OrgTipoFunzione[] GetTipiFunzione(string idAmm)
		{
			AmmUtils.WebServiceLink ws=new AmmUtils.WebServiceLink();
			return ws.GetTipiFunzione(false, idAmm);
		}

		/// <summary>
		/// Reperimento di un tipo funzione
		/// </summary>
		/// <param name="idTipoFunzione"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.OrgTipoFunzione GetTipoFunzione(string idTipoFunzione)
		{
			AmmUtils.WebServiceLink ws=new AmmUtils.WebServiceLink();
			return ws.GetTipoFunzione(idTipoFunzione,true);
		}

		private DocsPAWA.DocsPaWR.OrgFunzione[] GetFunzioni(string idTipoFunzione)
		{
			AmmUtils.WebServiceLink ws=new AmmUtils.WebServiceLink();
			return ws.GetFunzioni(idTipoFunzione);
		}

		/// <summary>
		/// Inserimento di un nuovo tipo funzione
		/// </summary>
		/// <param name="tipoRuolo"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.ValidationResultInfo InsertTipoFunzione(ref DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione)
		{
            DocsPAWA.DocsPaWR.ValidationResultInfo canInsert = this.CanSaveFunction(ref tipoFunzione);

			AmmUtils.WebServiceLink ws=new AmmUtils.WebServiceLink();

            if (canInsert.Value)
                return ws.InsertTipoFunzione(ref tipoFunzione);
            else
                return canInsert;
		}

		/// <summary>
		/// Aggiornamento di un tipo funzione
		/// </summary>
		/// <param name="tipoRuolo"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.ValidationResultInfo UpdateTipoFunzione(ref DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione)
		{
            DocsPAWA.DocsPaWR.ValidationResultInfo canUpdate = this.CanSaveFunction(ref tipoFunzione);

			AmmUtils.WebServiceLink ws=new AmmUtils.WebServiceLink();

            if (canUpdate.Value)
                return ws.UpdateTipoFunzione(ref tipoFunzione);
            else
                return canUpdate;
		}

        /// <summary>
        /// Metodo utilizzato per verificare se è possibile creare o apportare modifiche ad una macrofunzione. 
        /// Una macrofunzione è modificabile solo se c'è almeno una microfunzione associata
        /// </summary>
        /// <param name="tipoFunzione"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.ValidationResultInfo CanSaveFunction(ref DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione)
        {
            DocsPAWA.DocsPaWR.ValidationResultInfo retVal = new DocsPAWA.DocsPaWR.ValidationResultInfo();
            retVal.Value = true;
            
            if(this.ddl_ricTipo.SelectedIndex < 2)
                retVal.Value = tipoFunzione.Funzioni.Count(f => f.Associato == true) > 0;
            if (!retVal.Value)
                retVal.BrokenRules = new DocsPAWA.DocsPaWR.BrokenRule[] { new DocsPAWA.DocsPaWR.BrokenRule() { Description = "Associare almeno una funzione elementare al tipo funzione", Level = DocsPAWA.DocsPaWR.BrokenRuleLevelEnum.Error, ID = "FUNZIONI_MANCANTI" } };

            return retVal;
        }

		/// <summary>
		/// Cancellazione di un tipo funzione
		/// </summary>
		/// <param name="tipoRuolo"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.ValidationResultInfo DeleteTipoFunzione(ref DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione)
		{
			AmmUtils.WebServiceLink ws=new AmmUtils.WebServiceLink();
			return ws.DeleteTipoFunzione(tipoFunzione);
		}

		/// <summary>
		/// Associazione dati del tipo funzione ai campi della UI
		/// </summary>
		/// <param name="tipoRuolo"></param>
		private void BindUI(DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione)
		{
			this.ClearData();
			
			this.CurrentIDTipoFunzione=tipoFunzione.IDTipoFunzione;

			this.pnl_info.Visible= true;
			this.btn_aggiungi.Text = "Modifica";
			this.txt_codice.Text = tipoFunzione.Codice;
			this.txt_codice.Visible = false;
			this.lbl_cod.Visible = true;
			this.lbl_cod.Text = this.txt_codice.Text;
			this.txt_descrizione.Text = tipoFunzione.Descrizione;
            this.SetFocus(this.btn_find);
			// Caricamento lista microfunzioni associate al tipo funzione
			this.FillListFunzioni(tipoFunzione.Funzioni);
           // this.SetFocus(this.btn_aggiungi);
		}

		/// <summary>
		/// Aggiornamento del tipo funzione dai dati dei campi della UI
		/// </summary>
		private void RefreshTipoFunzioneFromUI(DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione)
		{
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
			tipoFunzione.IDTipoFunzione=this.CurrentIDTipoFunzione;
            tipoFunzione.IDAmministrazione = idAmm;//string.Empty;
			tipoFunzione.Codice=this.txt_codice.Text.Trim();
			tipoFunzione.Descrizione=this.txt_descrizione.Text.Trim();
            
            this.RefreshFunzioniFromUI(tipoFunzione);
		}

		/// <summary>
		/// Aggiornamento di tutte le singole funzioni dai dati dei campi della UI
		/// </summary>
		/// <param name="tipoFunzione"></param>
		private void RefreshFunzioniFromUI(DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione)
		{
			ArrayList funzioni=new ArrayList();
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

			foreach (DataGridItem item in this.dg_funzioni.Items)
			{
				// Reperimento funzione elementare
                DocsPAWA.DocsPaWR.OrgFunzione funzione = this.CreateFunzioneFromGridItem(tipoFunzione.IDTipoFunzione, item);
                funzione.IDAmministrazione = idAmm;
				// Aggiornamento solamente se lo stato è diverso da non modificato
				//if (funzione.StatoFunzione!=DocsPaWR.StatoOrgFunzioneEnum.Unchanged)
					funzioni.Add(funzione);
			}

			tipoFunzione.Funzioni=(DocsPAWA.DocsPaWR.OrgFunzione[]) funzioni.ToArray(typeof(DocsPAWA.DocsPaWR.OrgFunzione));
		}

		private DocsPAWA.DocsPaWR.OrgFunzione CreateFunzioneFromGridItem(string idTipoFunzione,DataGridItem item)
		{
            DocsPAWA.DocsPaWR.OrgFunzione funzione = new DocsPAWA.DocsPaWR.OrgFunzione();
			funzione.ID=item.Cells[GRID_FUNZIONE_COL_ID].Text.Replace("&nbsp;",string.Empty).Trim();
			funzione.IDTipoFunzione=idTipoFunzione;

			// Reperimento valore check associazione
			CheckBox chkSelection=this.GetCheckBoxAssociazioneFunzione(item);
			if (chkSelection!=null)
				funzione.Associato=chkSelection.Checked;

			// Impostazione dello stato della funzione (se inserito, cancellato o invariato)
			bool originalValue=Convert.ToBoolean(item.Cells[GRID_FUNZIONE_COL_ASSOCIATO].Text.Replace("&nbsp;",string.Empty).Trim());
            DocsPAWA.DocsPaWR.StatoOrgFunzioneEnum statoFunzione = DocsPAWA.DocsPaWR.StatoOrgFunzioneEnum.Unchanged;
			if (originalValue && !chkSelection.Checked)
                statoFunzione = DocsPAWA.DocsPaWR.StatoOrgFunzioneEnum.Deleted;
			else if (!originalValue && chkSelection.Checked)
                statoFunzione = DocsPAWA.DocsPaWR.StatoOrgFunzioneEnum.Inserted;
			funzione.StatoFunzione=statoFunzione;
			
			funzione.FunzioneAnagrafica=new DocsPAWA.DocsPaWR.OrgFunzioneAnagrafica();
			funzione.FunzioneAnagrafica.Codice=item.Cells[GRID_FUNZIONE_COL_CODICE].Text.Replace("&nbsp;",string.Empty).Trim();
			funzione.FunzioneAnagrafica.Descrizione=item.Cells[GRID_FUNZIONE_COL_DESCRIZIONE].Text.Replace("&nbsp",string.Empty).Trim();
			funzione.FunzioneAnagrafica.TipoFunzione=item.Cells[GRID_FUNZIONE_COL_TIPO_FUNZIONE].Text.Replace("&nbsp;",string.Empty).Trim();

			return funzione;
		}

		/// <summary>
		/// Rimozione dati UI
		/// </summary>
		private void ClearData()
		{
			this.dg_macroFunzioni.SelectedIndex=-1;	

			this.CurrentIDTipoFunzione=string.Empty;
			this.txt_codice.Text=string.Empty;
			this.lbl_cod.Text=string.Empty;
			this.txt_descrizione.Text=string.Empty;

			this.dg_funzioni.DataSource=null;
		}

		/// <summary>
		/// Salvataggio dati del tipo ruolo corrente
		/// </summary>
		private void Save()
		{
            DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione = new DocsPAWA.DocsPaWR.OrgTipoFunzione();
			this.RefreshTipoFunzioneFromUI(tipoFunzione);

            DocsPAWA.DocsPaWR.ValidationResultInfo result = null;

			bool insertMode=this.OnInsertMode();

			if (insertMode)
				result=this.InsertTipoFunzione(ref tipoFunzione);
			else
				result=this.UpdateTipoFunzione(ref tipoFunzione);

			if (!result.Value)
			{
				this.ShowValidationMessage(result);
			}
			else if (!insertMode)
			{
				// Aggiornamento
				this.RefreshTipoFunzioneGridItem(tipoFunzione);

				this.ClearData();

				this.pnl_info.Visible=false;
				this.lbl_cod.Visible=true;

				this.dg_macroFunzioni.SelectedIndex=-1;	
			}
			else
			{
				// Inserimento
				this.lbl_cod.Visible=true;

				// Refresh lista tipi funzione
				this.FillListTipiFunzione();

				// Predisposizione per un nuovo inserimento
				this.SetInsertMode();
			}
		}

		/// <summary>
		/// Caricamento lista delle funzioni per il tipo funzioni correntemente selezionato
		/// </summary>
		/// <param name="tipoFunzione"></param>
		private void FillListFunzioni(DocsPAWA.DocsPaWR.OrgFunzione[] funzioni)
		{

            DataSet ds;
            DataView view;

            if (ViewState["dsMicroFunzioni"] == null)
            {
                ds=this.ConvertToDataSetFunzioni(funzioni);

                ViewState["dsMicroFunzioni"] = ds;
            }
            else
            {
                ds = (DataSet)ViewState["dsMicroFunzioni"];
            }
            view = ds.Tables[0].DefaultView;
            DataTable dt = ds.Tables[0];
            
            if (ddl_ricTipo.SelectedValue != "T" && ddl_ricTipo.SelectedValue != "SEL")
            {
                string testoRic = txt_ricerca.Text.Trim().Replace("%", "");

                string filtro = ddl_ricTipo.SelectedValue + " like '%" + testoRic.Replace("'", "''") + "%'";
                view.RowFilter = filtro;

               

              
               
                //EnumerableRowCollection<DataRow> query = from data in dt.AsEnumerable()
                //                                         where data.Field<string>(ddl_ricTipo.SelectedValue).Contains(testoRic)
                //                                         select data;

            
            }

            this.dg_funzioni.DataSource = view;
			this.dg_funzioni.DataBind();

			this.FillCheckFunzioniAssociate();
            /* ANOMALIA INPS - Errore nella creazione della funzione
             * Modifica: Commentata linea di modifica del testo pulsante aggiungi
             * Se clicco su cerca per trovare una microfunzione, viene richiamato 
             * il metodo FillListFunzioni(). Modificando il testo del pulsante aggiungi,
             * quando l'utente clicca sul pulsante viene lanciata una update, che fallisce
             * in quanto la funzione non è stata ancora creata
             */
             //this.btn_aggiungi.Text = "Modifica";
             DocsPAWA.Utils.DefaultButton(this, ref txt_ricerca, ref btn_find);
            
            this.btn_find.Focus();
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

			this._businessRuleControls.Add("CODICE_TIPO_FUNZIONE",this.txt_codice);
			this._businessRuleControls.Add("DESCRIZIONE_TIPO_FUNZIONE",this.txt_descrizione);
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

		/// <summary>
		/// Verifica se si è in fase di inserimento
		/// </summary>
		/// <returns></returns>
		private bool OnInsertMode()
		{
			return (btn_aggiungi.Text.Equals("Aggiungi"));
		}

		#region Gestione datagrid TipiFunzione

		/// <summary>
		/// Constanti che identificano i nomi delle colonne del dataset
		/// utilizzato per caricare il datagrid dei tipi funzione
		/// </summary>
		private const string TABLE_COL_ID="ID";
		private const string TABLE_COL_CODICE="Codice";
		private const string TABLE_COL_DESCRIZIONE="Descrizione";

		/// <summary>
		/// Costanti che identificano i nomi delle colonne del datagrid dei tipi funzione
		/// </summary>
		private const int GRID_TIPI_FUNZIONE_COL_ID=0;
		private const int GRID_TIPI_FUNZIONE_COL_CODICE=1;
		private const int GRID_TIPI_FUNZIONE_COL_DESCRIZIONE=2;
		private const int GRID_TIPI_FUNZIONE_COL_SELECT=3;
		private const int GRID_TIPI_FUNZIONE_COL_DELETE=4;
		
		private const string TABLE_FUNZIONI_COL_ID="ID";
		private const string TABLE_FUNZIONI_COL_CODICE="Codice";
		private const string TABLE_FUNZIONI_COL_DESCRIZIONE="Descrizione";
		private const string TABLE_FUNZIONI_COL_TIPO_FUNZIONE="TipoFunzione";
		private const string TABLE_FUNZIONI_COL_ASSOCIATO="Associato";

		private const int GRID_FUNZIONE_COL_SELECT=0;
		private const int GRID_FUNZIONE_COL_DESCRIZIONE=1;
		private const int GRID_FUNZIONE_COL_CODICE=2;
		private const int GRID_FUNZIONE_COL_TIPO_FUNZIONE=3;
		private const int GRID_FUNZIONE_COL_ID=4;
		private const int GRID_FUNZIONE_COL_ASSOCIATO=5;
		
		/// <summary>
		/// Gestione caricamento lista tipi funzione
		/// </summary>
		private void FillListTipiFunzione()
		{
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            DocsPAWA.DocsPaWR.OrgTipoFunzione[] tipiFunzione = this.GetTipiFunzione(idAmm);

			this.dg_macroFunzioni.DataSource=this.ConvertToDataSetTipiFunzione(tipiFunzione);
			this.dg_macroFunzioni.DataBind();
		}

		/// <summary>
		/// Aggiornamento elemento datagrid dei tipi funzione corrente
		/// </summary>
		/// <param name="registro"></param>
		private void RefreshTipoFunzioneGridItem(DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione)
		{
			DataGridItem item=this.dg_macroFunzioni.SelectedItem;
			
			if (item!=null)
			{
				item.Cells[GRID_TIPI_FUNZIONE_COL_DESCRIZIONE].Text=tipoFunzione.Descrizione;
				item.Cells[GRID_TIPI_FUNZIONE_COL_CODICE].Text=tipoFunzione.Codice;
				item.Cells[GRID_TIPI_FUNZIONE_COL_ID].Text=tipoFunzione.IDTipoFunzione;
			}
		}

		/// <summary>
		/// Conversione array TipiFunzione in dataset
		/// </summary>
		/// <param name="registri"></param>
		/// <returns></returns>
		private DataSet ConvertToDataSetTipiFunzione(DocsPAWA.DocsPaWR.OrgTipoFunzione[] tipiFunzione)
		{
			DataSet ds=this.CreateGridDataSetTipiFunzione();
			DataTable dt=ds.Tables["TipiFunzione"];

			foreach (DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione in tipiFunzione)
			{
				DataRow row=dt.NewRow();
				
				row[TABLE_COL_ID]=tipoFunzione.IDTipoFunzione;
				row[TABLE_COL_CODICE]=tipoFunzione.Codice;
				row[TABLE_COL_DESCRIZIONE]=tipoFunzione.Descrizione;

				dt.Rows.Add(row);
			}

			return ds;
		}

		/// <summary>
		/// Creazione dataset necessario per effettuare il bind
		/// dei dati dei tipi funzioni nel datagrid
		/// </summary>
		/// <returns></returns>
		private DataSet CreateGridDataSetTipiFunzione()
		{
			DataSet ds=new DataSet();
			DataTable dt=new DataTable("TipiFunzione");

			dt.Columns.Add(new DataColumn(TABLE_COL_ID));
			dt.Columns.Add(new DataColumn(TABLE_COL_CODICE));
			dt.Columns.Add(new DataColumn(TABLE_COL_DESCRIZIONE));

			ds.Tables.Add(dt);
			return ds;
		}

		private DataSet ConvertToDataSetFunzioni(DocsPAWA.DocsPaWR.OrgFunzione[] funzioni)
		{
			DataSet ds=this.CreateGridDataSetFunzioni();
			DataTable dt=ds.Tables["Funzioni"];

			foreach (DocsPAWA.DocsPaWR.OrgFunzione funzione in funzioni)
			{
				DataRow row=dt.NewRow();
				
				row[TABLE_FUNZIONI_COL_ID]=funzione.ID;
				row[TABLE_FUNZIONI_COL_CODICE]=funzione.FunzioneAnagrafica.Codice;
				row[TABLE_FUNZIONI_COL_DESCRIZIONE]=funzione.FunzioneAnagrafica.Descrizione;
				row[TABLE_FUNZIONI_COL_TIPO_FUNZIONE]=funzione.FunzioneAnagrafica.TipoFunzione;
				row[TABLE_FUNZIONI_COL_ASSOCIATO]=funzione.Associato;

				dt.Rows.Add(row);
			}

			return ds;
		}

		/// <summary>
		/// Creazione dataset necessario per effettuare il bind
		/// dei dati delle funzioni nel datagrid 
		/// </summary>
		/// <returns></returns>
		private DataSet CreateGridDataSetFunzioni()
		{
			DataSet ds=new DataSet();
			DataTable dt=new DataTable("Funzioni");

			dt.Columns.Add(new DataColumn(TABLE_FUNZIONI_COL_ID));
			dt.Columns.Add(new DataColumn(TABLE_FUNZIONI_COL_CODICE));
			dt.Columns.Add(new DataColumn(TABLE_FUNZIONI_COL_DESCRIZIONE));
			dt.Columns.Add(new DataColumn(TABLE_FUNZIONI_COL_TIPO_FUNZIONE));
			dt.Columns.Add(new DataColumn(TABLE_FUNZIONI_COL_ASSOCIATO,typeof(bool)));

			ds.Tables.Add(dt);
			return ds;
		}

		/// <summary>
		/// Aggiornamento del valore del check della griglia delle funzioni associate
		/// </summary>
		private void FillCheckFunzioniAssociate()
		{
			foreach (DataGridItem item in this.dg_funzioni.Items)
			{
				CheckBox chkSelection=this.GetCheckBoxAssociazioneFunzione(item);
				
				if (chkSelection!=null)
					chkSelection.Checked=Convert.ToBoolean(item.Cells[GRID_FUNZIONE_COL_ASSOCIATO].Text);
			}
		}

		private CheckBox GetCheckBoxAssociazioneFunzione(DataGridItem item)
		{
			return item.Cells[GRID_FUNZIONE_COL_SELECT].FindControl("cbSel") as CheckBox;
		}

		private void dg_funzioni_PreRender(object sender, System.EventArgs e)
		{
			//this.FillCheckFunzioniAssociate();
		}

		#endregion

		/// <summary>
		/// Predisposizione per l'inserimento di un nuovo tipo funzione
		/// </summary>
		private void SetInsertMode()
		{	// Rimozione dati controlli UI
			this.ClearData();
			
			this.SetFocus(txt_codice);
			
			this.pnl_info.Visible = true;
			
			this.txt_codice.Visible = true;

			this.dg_funzioni.Visible=true;
			this.RegisterScrollKeeper("divGridFunzioni");

			// Caricamento lista funzioni
            this.resetFiltriricerca();
			this.FillListFunzioni(this.GetFunzioni("Null"));
            this.btn_aggiungi.Text = "Aggiungi";
		}

		/// <summary>
		/// Cancellazione tipo funzione
		/// </summary>
		private void Delete()
		{
            DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione = new DocsPAWA.DocsPaWR.OrgTipoFunzione();
			this.RefreshTipoFunzioneFromUI(tipoFunzione);

            DocsPAWA.DocsPaWR.ValidationResultInfo result = this.DeleteTipoFunzione(ref tipoFunzione);

			if (result.Value)
			{
				this.FillListTipiFunzione();

				this.pnl_info.Visible=false;

				this.ClearData();

				this.dg_macroFunzioni.SelectedIndex=-1;
			}
			else
			{
				this.ShowValidationMessage(result);
			}
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
				//this.Page.RegisterStartupScript(
                this.ClientScript.RegisterStartupScript(this.GetType(),scriptKey, scriptString);
			}
		}

		#endregion

        protected void btn_find_Click(object sender, EventArgs e)
        {
            if (ddl_ricTipo.SelectedValue != "SEL")
            {
                DocsPAWA.DocsPaWR.OrgTipoFunzione tipoFunzione = tipoFunzione = new DocsPAWA.DocsPaWR.OrgTipoFunzione();


                switch (ddl_ricTipo.SelectedIndex)
                {
                    case 2:
                        if (!string.IsNullOrEmpty(txt_ricerca.Text))
                        {
                            tipoFunzione.Codice = txt_ricerca.Text;
                        }
                        break;
                    case 3:
                        if (!string.IsNullOrEmpty(txt_ricerca.Text))
                        {
                            tipoFunzione.Descrizione = txt_ricerca.Text;
                        }
                        break;
                }

              
                    FillListFunzioni(tipoFunzione.Funzioni);

            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "selezionaRic", "javascript:alert('Selezionare un tipo di Ricerca');", true);
            }
          
            
        }

        protected void btn_aggiungi_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        // MEV ESTRAZIONE REPORT

        //protected void btn_export_Click(object sender, CommandEventArgs e)
        //{

        //    string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
        //    string reportType = e.CommandName;
        //    string idFunzione = this.txt_codice.Text.Trim();
        //    string formato = "XLS";

        //    DocsPAWA.DocsPaWR.FileDocumento fileDoc = new DocsPAWA.DocsPaWR.FileDocumento();

        //    // da spostare in metodo privato?
        //    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
        //    fileDoc = ws.GetReportFunzioni(reportType, formato, idFunzione, idAmm);

        //    if (fileDoc != null)
        //    {
        //        Response.ContentType = "application/vnd.ms-excel";
        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + fileDoc.name);
        //        Response.BinaryWrite(fileDoc.content);
        //        Response.Flush();
        //        Response.End();
        //    }
 
        //}

        protected void btn_export(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "MACRO_FUNZ":
                    string idFunzione = this.txt_codice.Text.Trim();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenExportMacro", "OpenExportMacro('" + idFunzione + "');", true);
                    break;

                case "MICRO_FUNZ":
                    if (!string.IsNullOrEmpty(this.selectedFunctionForExport))
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenExportMicro", "OpenExportMicro('" + this.selectedFunctionForExport + "');", true);
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noFuncSelected", "alert('Nessuna funzione selezionata.');", true);
                    break;
            }
            
            
        }

        //protected void btn_export_micro_Click(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(this.selectedFunctionForExport))
        //    {
                
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenExportMicro", "OpenExportMicro('" + this.selectedFunctionForExport + "');", true); 
        //    }
        //    else
        //    {
        //        // nessuna funzione selezionata
        //    }

        //}

        private void dg_funzioni_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            if (e.CommandName.Equals("SELECT"))
            {
                this.selectedFunctionForExport = e.Item.Cells[GRID_FUNZIONE_COL_CODICE].Text.Trim();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setScroll", "setGridScroll();", true); 
            }
        }

        protected string UrlExport
        {
            get
            {
                return "ExportDettagli.aspx";
            }

        }

        protected string selectedFunctionForExport
        {
            get
            {
                if (this.ViewState["selectedFunctionForExport"] != null)
                    return ViewState["selectedFunctionForExport"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["selectedFunctionForExport"] = value;
            }

        }

       
	}

}
