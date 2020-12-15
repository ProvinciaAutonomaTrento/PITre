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
using DocsPaWR = DocsPAWA.DocsPaWR;

namespace Amministrazione.Gestione_MezzoSpedizione
{
	/// <summary>
	/// Summary description for Ruoli.
	/// </summary>
    public class MezzoSpedizione : System.Web.UI.Page
	{		
		#region WebControls e variabili

		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.WebControls.Label lbl_position;
        protected System.Web.UI.WebControls.DataGrid dg_mezzi;
        protected System.Web.UI.WebControls.Button btn_nuovoMezzo;
		//protected System.Web.UI.WebControls.TextBox txt_chatipocanale;
		protected System.Web.UI.WebControls.TextBox txt_descrizione;
		protected System.Web.UI.WebControls.Panel pnl_info;
		protected System.Web.UI.WebControls.Button btn_salva;
        protected DocsPAWA.DocsPaWR.InfoUtenteAmministratore datiAmministratore;

		//------------------------------------------------------------------------------------------------------
		protected System.Web.UI.WebControls.Label lbl_tit;
		protected System.Web.UI.WebControls.ImageButton btn_chiudiPnlInfo;
		

		/// <summary>
		/// Costanti che identificano i nomi delle colonne del dataset 
		/// utilizzato per caricare i dati nel datagrid
		/// </summary>
        private const string TABLE_COL_ID = "IDSYSTEM_MS";
        private const string TABLE_COL_DESCRIZIONE = "DESCRIPTION";
       // private const string TABLE_COL_CHATIPOCANALE = "CHA_TIPO_CANALE";
        private const string TABLE_COL_DETTAGLIO = "DETTAGLIO";
        private const string TABLE_COL_DISABLED = "CANCELLATO";

		/// <summary>
		/// Costanti che identificano i nomi delle colonne del datagrid
		/// </summary>
		private const int GRID_COL_ID=0;
		private const int GRID_COL_DESCRIZIONE=1;
       // private const int GRID_COL_CHATIPOCANALE = 2;
        private const int GRID_COL_DISABLED = 2;
        private const int GRID_COL_DETTAGLIO = 3;
        private const int GRID_COL_DELETE = 4;
        
		#endregion

		#region Form_Load
		private void Page_Load(object sender, System.EventArgs e)
		{
            Session["AdminBookmark"] = "MezzoSpedizione";

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
				lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"1");										

				// Caricamento lista mezzi spedizione
				this.FillListMezziSpedizione();
			}
        }
		#endregion		

        #region dg_mezzi

        private DataView OrdinaGrid(DataView dv, string sortColumn)
		{
			string sortMode = " ASC";
			dv.Sort = sortColumn + sortMode;
			return dv;
		}

		private void dg_mezzi_ItemCommand(object source, DataGridCommandEventArgs e)
		{			
			pnl_info.Visible=false;
			
			ViewState["riga"] = e.Item.DataSetIndex;

			switch(e.CommandName) 
			{
				case "Select":
					pnl_info.Visible=true;

					btn_salva.Text="Modifica";

					SetFocus(txt_descrizione);
		
					DocsPaWR.MezzoSpedizione m_sped = this.GetMezzoSpedizione(e.Item.Cells[GRID_COL_ID].Text);
					this.BindUI(m_sped);

					break;

				case "Delete":
                    this.dg_mezzi.SelectedIndex = e.Item.ItemIndex;
                    string idTipoRuolo = this.dg_mezzi.SelectedItem.Cells[GRID_COL_ID].Text;

					this.Delete();
					
					break;
			}
		}

        private void dg_mezzi_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{	
		    if(!e.Item.Cells[GRID_COL_DELETE].Text.Equals(""))
			    e.Item.Cells[GRID_COL_DELETE].Attributes.Add ("onclick","if (!window.confirm('Sei sicuro di voler eliminare questo mezzo di spedizione?')) {return false};");
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
        private void btn_nuovoMezzo_Click(object sender, System.EventArgs e)
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
			this.dg_mezzi.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dg_mezzi_ItemCreated);
			this.dg_mezzi.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_mezzi_ItemCommand);
			this.btn_chiudiPnlInfo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiPnlInfo_Click);
            this.btn_nuovoMezzo.Click += new EventHandler(this.btn_nuovoMezzo_Click);
			this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
			this.Load += new System.EventHandler(this.Page_Load);
            this.dg_mezzi.PreRender += new System.EventHandler(dg_mezzi_PreRender);
           // this.DivDGList.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DivDGList_ItemDataBound);
            //this.DataGrid1.PreRender += new System.EventHandler(this.Datagrid1_PreRender);
        }
		#endregion

		private void btn_chiudiPnlInfo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			txt_descrizione.Text="";
			//txt_chatipocanale.Text="";
			pnl_info.Visible=false;

            dg_mezzi.SelectedIndex = -1;
		}


        private void dg_mezzi_PreRender(object sender, System.EventArgs e)
        {
            for (int i = 0; i < this.dg_mezzi.Items.Count; i++)
			{
                if (this.dg_mezzi.Items[i].ItemIndex >= 0)
				{
                    if (!this.dg_mezzi.Items[i].Cells[2].Text.Equals("") && !this.dg_mezzi.Items[i].Cells[2].Text.Equals("&nbsp;"))
                    {
                        this.dg_mezzi.Items[i].Cells[1].ForeColor = Color.Red;
                        this.dg_mezzi.Items[i].Cells[2].ForeColor = Color.Red;
                        this.dg_mezzi.Items[i].Cells[2].HorizontalAlign = HorizontalAlign.Center;
                        this.dg_mezzi.Items[i].Cells[3].Text = "";
                        this.dg_mezzi.Items[i].Cells[4].Text = "";
                    }

                    if (this.dg_mezzi.Items[i].Cells[1].Text.ToUpper().Equals("LETTERA") || this.dg_mezzi.Items[i].Cells[1].Text.ToUpper().Equals("MAIL") || this.dg_mezzi.Items[i].Cells[1].Text.ToUpper().Equals("INTEROPERABILITA"))
                        this.dg_mezzi.Items[i].Cells[4].Text = "";
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
		/// Reperimento Lista mezzi di spedizione
		/// </summary>
		/// <returns></returns>
        private DocsPAWA.DocsPaWR.MezzoSpedizione[] GetListaMezzoSpedizione()
		{
			DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();
            string idAmministrazione = this.GetIdAmministrazione();
            return ws.AmmListaMezzoSpedizione(idAmministrazione, true);
		}

        /// <summary>
        /// Reperimento del mezzo di spedizione
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.MezzoSpedizione GetMezzoSpedizione(string idMezzoSpedizione)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            return ws.AmmGetMezzoSpedizione(idMezzoSpedizione);
        }
        
		/// <summary>
		/// Inserimento di un nuovo mezzo di spedizione
		/// </summary>
		/// <param name="mezzo spedizione"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.ValidationResultInfo InsertMezzoSpedizione(ref DocsPAWA.DocsPaWR.MezzoSpedizione m_sped)
		{
            DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            datiAmministratore = session.getUserAmmSession();
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
			return ws.AmmInsertMezzoSpedizione(datiAmministratore, ref m_sped, idAmm);

            this.FillListMezziSpedizione();
            this.txt_descrizione.Text = "";
            // this.txt_chatipocanale.Text = "";
		}

        /// <summary>
        /// Gestione caricamento lista mezzi spedizione
        /// </summary>
        private void FillListMezziSpedizione()
        {
            DocsPAWA.DocsPaWR.MezzoSpedizione[] m_sped = this.GetListaMezzoSpedizione();
            if (m_sped != null)
            {
                DataSet ds = this.ConvertToDataSet(m_sped);

                DataView dv = ds.Tables["MezzoSpedizione"].DefaultView;

                this.dg_mezzi.DataSource = dv;
                this.dg_mezzi.DataBind();

                ds.Dispose();
                ds = null;

                m_sped = null;
            }
        }

		/// <summary>
		/// Aggiornamento di un tipo ruolo
		/// </summary>
		/// <param name="tipoRuolo"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.ValidationResultInfo UpdateMezzoSpedizione(ref DocsPAWA.DocsPaWR.MezzoSpedizione m_sped)
		{
			DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();
			return ws.AmmUpdateMezzoSpedizione(ref m_sped);
		}

	        /// <summary>
        /// Cancellazione di un mezzo di spedizione
        /// </summary>
        /// <param name="tipoRuolo"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.ValidationResultInfo DeleteMezzoSpedizione(ref DocsPAWA.DocsPaWR.MezzoSpedizione m_sped)
        {
            DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
            datiAmministratore = session.getUserAmmSession();
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            return ws.AmmDeleteMezzoSpedizione(datiAmministratore, ref m_sped, idAmm);
        }

		/// <summary>
		/// Associazione dati del tipo ruolo ai campi della UI
		/// </summary>
		/// <param name="tipoRuolo"></param>
		private void BindUI(DocsPAWA.DocsPaWR.MezzoSpedizione m_sped)
		{
			this.ClearData();

			//this.txt_chatipocanale.Text=m_sped.chaTipoCanale;
			this.txt_descrizione.Text=m_sped.Descrizione;
		}

		/// <summary>
		/// Aggiornamento del tipo ruolo dai dati dei campi della UI
		/// </summary>
		private void RefreshTipoRuoloFromUI(DocsPAWA.DocsPaWR.OrgTipoRuolo tipoRuolo)
		{
			tipoRuolo.IDTipoRuolo=this.CurrentIDTipoRuolo.ToString();
			tipoRuolo.Descrizione=this.txt_descrizione.Text.Trim();
		}

		/// <summary>
		/// Rimozione dati UI
		/// </summary>
		private void ClearData()
		{
			this.CurrentIDTipoRuolo=0;
			this.txt_descrizione.Text=string.Empty;
			//this.txt_chatipocanale.Text=string.Empty;
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
        /// Reperimento id amministrazione corrente
        /// </summary>
        /// <returns></returns>
        private string GetIdAmministrazione()
        {
            return AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
        }

		/// <summary>
		/// Impostazione dell'id dell'amministrazione nel tipo ruolo
		/// </summary>
		/// <param name="tipoRuolo"></param>
		private void SetIdAmministrazione(DocsPAWA.DocsPaWR.OrgTipoRuolo tipoRuolo)
		{
			Manager.OrganigrammaManager orgManager=new Manager.OrganigrammaManager();
			orgManager.CurrentIDAmm(this.GetCodiceAmministrazione());

			tipoRuolo.IDAmministrazione=orgManager.getIDAmministrazione();
		}

		/// <summary>
		/// Salvataggio dati del mezzo spedizione corrente
		/// </summary>
		private void Save()
		{
			DocsPaWR.MezzoSpedizione m_sped=new DocsPAWA.DocsPaWR.MezzoSpedizione();

			DocsPaWR.ValidationResultInfo result=null;

			bool insertMode=this.OnInsertMode();
            if (!this.txt_descrizione.Text.Equals(""))
            {
                m_sped.Descrizione = this.txt_descrizione.Text;
                // m_sped.chaTipoCanale = this.txt_chatipocanale.Text;
                m_sped.chaTipoCanale = this.txt_descrizione.Text.Substring(0, 1).ToUpper();
            }
            else
            {
                this.RegisterClientScript("AlertDescrizione", "alert('Inserire una descrizione per il mezzo di spedizione');");
                return;
            }

            if (insertMode)
            {
                result = this.InsertMezzoSpedizione(ref m_sped);
                if (!result.Value)
                    this.RegisterClientScript("alert", "alert(\"Descrizione già presente\");");
                else
                {
                    this.FillListMezziSpedizione();
                    this.lbl_tit.Visible = true;
                    // Predisposizione per un nuovo inserimento
                    this.SetInsertMode();
                }
            }
            else
            {
                string idMezzo = this.dg_mezzi.SelectedItem.Cells[0].Text.ToString();
                m_sped = this.GetMezzoSpedizione(idMezzo);
                m_sped.Descrizione = this.txt_descrizione.Text;
                // m_sped.chaTipoCanale = this.txt_chatipocanale.Text;
                m_sped.chaTipoCanale = this.txt_descrizione.Text.Substring(0, 1).ToUpper();
                result = this.UpdateMezzoSpedizione(ref m_sped);
                if (!result.Value)
                    this.RegisterClientScript("alert", "alert(\"Modifica non effettuata\");");
                else
                {
                    // Aggiornamento
                    this.RefreshGridItem(m_sped);
                    this.ClearData();
                    this.pnl_info.Visible = false;
                    this.lbl_tit.Visible = true;
                    this.dg_mezzi.SelectedIndex = -1;
                }
            }
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

			//this._businessRuleControls.Add("CODICE_TIPO_RUOLO",this.txt_codice);
			this._businessRuleControls.Add("DESCRIZIONE_TIPO_RUOLO",this.txt_descrizione);
			//this._businessRuleControls.Add("LIVELLO_TIPO_RUOLO",this.txt_livello);
		}

		private Control GetBusinessRuleControl(string idBusinessRule)
		{
			return this._businessRuleControls[idBusinessRule] as Control;
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
		/// Aggiornamento elemento griglia corrente
		/// </summary>
		/// <param name="registro"></param>
		private void RefreshGridItem(DocsPAWA.DocsPaWR.MezzoSpedizione m_sped)
		{
            DataGridItem item = this.dg_mezzi.SelectedItem;
			
			if (item!=null)
			{
				item.Cells[GRID_COL_DESCRIZIONE].Text=m_sped.Descrizione;
			}
		}

		/// <summary>
		/// Conversione array
		/// </summary>
		/// <param name="registri"></param>
		/// <returns></returns>
        private DataSet ConvertToDataSet(DocsPAWA.DocsPaWR.MezzoSpedizione[] m_sped)
		{
			DataSet ds=this.CreateGridDataSet();
            DataTable dt = ds.Tables["MezzoSpedizione"];

            foreach (DocsPAWA.DocsPaWR.MezzoSpedizione m_spediz in m_sped)
			{
				DataRow row=dt.NewRow();
				
				row[TABLE_COL_ID]=m_spediz.IDSystem;
				row[TABLE_COL_DESCRIZIONE]= m_spediz.Descrizione;
                //row[TABLE_COL_CHATIPOCANALE] = m_spediz.chaTipoCanale;
                row[TABLE_COL_DISABLED] = m_spediz.Disabled;

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
            DataTable dt = new DataTable("MezzoSpedizione");

			dt.Columns.Add(new DataColumn(TABLE_COL_ID));
			dt.Columns.Add(new DataColumn(TABLE_COL_DESCRIZIONE));
            //dt.Columns.Add(new DataColumn(TABLE_COL_CHATIPOCANALE));
            dt.Columns.Add(new DataColumn(TABLE_COL_DISABLED));

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
			btn_salva.Text			= "Aggiungi";

            this.dg_mezzi.SelectedIndex = -1;

            SetFocus(txt_descrizione);
		}

		/// <summary>
		/// Cancellazione mezzo di spedizione
		/// </summary>
		private void Delete()
		{
            if (!this.dg_mezzi.SelectedItem.Cells[GRID_COL_ID].Text.Equals(""))
            {
                DocsPaWR.MezzoSpedizione m_sped = new DocsPAWA.DocsPaWR.MezzoSpedizione();
                m_sped = this.GetMezzoSpedizione(this.dg_mezzi.SelectedItem.Cells[GRID_COL_ID].Text.ToString());
                DocsPaWR.ValidationResultInfo result = this.DeleteMezzoSpedizione(ref m_sped);

                if (result.Value)
                {
                    this.FillListMezziSpedizione();
                    pnl_info.Visible = false;
                    this.ClearData();
                    dg_mezzi.SelectedIndex = -1;
                }
                else
                {
                    this.RegisterClientScript("alert2", "alert('La cancellazione mezzo di spedizione non è avvenuta correttamente');");
                }
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
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}

		#endregion
	}
}
