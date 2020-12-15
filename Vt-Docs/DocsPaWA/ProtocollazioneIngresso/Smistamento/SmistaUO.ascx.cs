namespace ProtocollazioneIngresso.Smistamento
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using ProtocollazioneIngresso.Log;

	/// <summary>
	///		Summary description for SmistaUO.
	/// </summary>
	public class SmistaUO : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid grdUOApp;

		// Costanti indicanti gli indici delle colonne della griglia
		private const int COL_ID=0;
		private const int COL_TYPE=1;
		private const int COL_DESCRIPTION=2;
		private const int COL_COMP=3;
		private const int COL_CC=4;
		private const int COL_RESET=5;
		private const int COL_HAS_RUOLI_RIF=6;
		private const int COL_LIST_RUOLI_RIF=7;
        private string tipoProto;

       
        

		private void Page_Load(object sender, System.EventArgs e)
		{
            
            if (!this.IsPostBack)
			{
				this.RegisterClientEvents();
                //Session["tipoProto"] = tipoProto;
			}
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			this.RefreshGridItems();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.grdUOApp.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.OnDataGridItemCreated);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		#region Public members

		/// <summary>
		/// Aggiornamento delle selezioni delle UO in base all'array fornito in ingresso
		/// </summary>
		/// <param name="listUO"></param>
		public void RefreshSelections(DocsPAWA.DocsPaWR.UOSmistamento[] listUO)
		{
			if (this.grdUOApp.Items.Count>0 && listUO!=null && listUO.Length>0)
			{
				Hashtable tableListUO=new Hashtable();
				foreach (DocsPAWA.DocsPaWR.UOSmistamento uo in listUO)
					if (!tableListUO.ContainsKey(uo.ID))
						tableListUO.Add(uo.ID,uo);				
		

				foreach (DataGridItem item in this.grdUOApp.Items)
				{
                    DocsPAWA.DocsPaWR.UOSmistamento uo = tableListUO[item.Cells[COL_ID].Text] as DocsPAWA.DocsPaWR.UOSmistamento;

					RadioButton radioButton=radioButton=(RadioButton) item.Cells[COL_RESET].FindControl("optNull");

					if (uo!=null)
					{
						if (uo.FlagCompetenza)
							radioButton=(RadioButton) item.Cells[COL_COMP].FindControl("optComp");
						else if (uo.FlagConoscenza)
							radioButton=(RadioButton) item.Cells[COL_CC].FindControl("optCC");

						uo=null;
					}

					radioButton.Checked=true;
				}

				tableListUO.Clear();
				tableListUO=null;
			}
		}

		/// <summary>
		/// Caricamento delle UO cui smistare il documento (dipendenti da registro)
		/// </summary>
		/// <param name="idRegistro"></param>
		public void LoadData(string idRegistro)
		{
            DocsPAWA.DocsPaWR.UOSmistamento[] listUO = GetListUOSmistamento(idRegistro, this.CreateMittenteSmistamento());

			DataSet ds=this.CreateGridDataSet();

			foreach (DocsPAWA.DocsPaWR.UOSmistamento uo in listUO)
			{	
				string listRuoliRif;
				bool hasRuoliRiferimento=this.HasRuoliRiferimento(uo,out listRuoliRif);
				
				this.AppendDataRow(	ds.Tables["GRID_TABLE"],
									"U",
									uo.ID,
									uo.Descrizione,
									hasRuoliRiferimento,
									listRuoliRif);
			}
			
			this.grdUOApp.DataSource=ds;
			this.grdUOApp.DataBind();

			this.SetSessionListUO(listUO);
		}

		/// <summary>
		/// Smistamento del documento alle UO selezionate
		/// </summary>
		/// <returns></returns>
		public DocsPAWA.DocsPaWR.EsitoSmistamentoDocumento[] SmistaDocumento()
		{
			return this.SmistaDocumento(this.GetSelectedUO());
		}

		/// <summary>
		/// Smistamento del documento alle UO selezionate
		/// </summary>
		/// <param name="selectedUO"></param>
		/// <returns></returns>
		public DocsPAWA.DocsPaWR.EsitoSmistamentoDocumento[] SmistaDocumento(DocsPAWA.DocsPaWR.UOSmistamento[] uoDestinatarie)
		{
            ProtocollazioneIngresso.Login.LoginMng loginMng = new ProtocollazioneIngresso.Login.LoginMng(this.Page);
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = loginMng.GetInfoUtente();
			loginMng=null;

			Protocollo.ProtocolloMng protocolloMng=new Protocollo.ProtocolloMng(this.Page);
			string idDocumento=protocolloMng.GetDocumentoCorrente().systemId;
			protocolloMng=null;

            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

			// Reperimento dati del documento da smistare
            DocsPAWA.DocsPaWR.DocumentoSmistamento documentoSmistamento =
				ws.GetDocumentoSmistamento(idDocumento,infoUtente,false);

			 DocsPAWA.DocsPaWR.EsitoSmistamentoDocumento[] retValue= ws.SmistaDocumentoNonTrasmesso(
										this.CreateMittenteSmistamento(),
										infoUtente,
										documentoSmistamento,
										uoDestinatarie,
                                        DocsPAWA.Utils.getHttpFullPath());	

			try
			{
				foreach (DocsPAWA.DocsPaWR.EsitoSmistamentoDocumento item in retValue)
				{
					ProtocollazioneIngressoLog.WriteLogEntry(string.Format("Smistamento (Esito: {0} - DenominazioneDestinatario: {1}",
						item.DescrizioneEsitoSmistamento,
						item.DenominazioneDestinatario));
				}
			}
			catch 
			{

			}

			return retValue;
		}

		/// <summary>
		/// Rimozione delle selezioni dei radio buttons competenza e conoscenza
		/// </summary>
		public void ClearSelections()
		{
			foreach (DataGridItem item in this.grdUOApp.Items)
			{
				RadioButton radioButton=item.Cells[COL_RESET].FindControl("optNull") as RadioButton;

				if (radioButton!=null)
					radioButton.Checked=true;					
			}
		}

		/// <summary>
		/// Gestione abilitazione / disabilitazione controlli UI
		/// </summary>
		/// <param name="isEnabled"></param>
		public void EnableControls(bool isEnabled)
		{
			this.grdUOApp.Enabled=isEnabled;

			foreach (System.Web.UI.Control ctl in this.grdUOApp.Controls)
				this.EnableButtonClearOptions(isEnabled,ctl);
		}

		/// <summary>
		/// Verifica se almeno una UO è stata selezionata dalla griglia (per comp o per cc)
		/// </summary>
		/// <returns></returns>
		public bool AlmostOneUOSelected()
		{
			return (this.GetSelectedUO().Length>0);
		}

		/// <summary>
		/// Reperimento di tutte le UO selezionate (per comp o cc) nella griglia
		/// </summary>
		/// <returns></returns>
		public DocsPAWA.DocsPaWR.UOSmistamento[] GetSelectedUO()
		{
			ArrayList selectedUO=new ArrayList();
            DocsPAWA.DocsPaWR.UOSmistamento uo = null;

			string idUO=string.Empty;
			string type=string.Empty;
			bool flagComp=false;
			bool flagCC=false;
	
			foreach (DataGridItem item in this.grdUOApp.Items)
			{
				this.LoadItemValues(item,out idUO,out type,out flagComp,out flagCC);
				
				if (flagComp || flagCC)
				{
					uo=this.GetUOFromSession(idUO);

					uo.FlagCompetenza=flagComp;
					uo.FlagConoscenza=flagCC;

					// Impostazione dei FlagCompetenza e FlagConoscenza
					// per i soli ruoli di riferimento della UO
					foreach (DocsPAWA.DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
					{
						if (ruolo.RuoloRiferimento)
						{
							ruolo.FlagCompetenza=flagComp;
							ruolo.FlagConoscenza=flagCC;

							foreach (DocsPAWA.DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
							{
								utente.FlagCompetenza=flagComp;
								utente.FlagConoscenza=flagCC;
							}
						}
					}

					selectedUO.Add(uo);
				}
			}

            DocsPAWA.DocsPaWR.UOSmistamento[] retValue = new DocsPAWA.DocsPaWR.UOSmistamento[selectedUO.Count];
			selectedUO.CopyTo(retValue);
			return retValue;
		}

		/// <summary>
		/// Impostazione del focus sul primo radiobutton abilitato disponibile in griglia
		/// </summary>
		public void SetFocusNullRadio()
		{
			foreach (DataGridItem gridItem in this.grdUOApp.Items)
			{
				RadioButton radioButton=gridItem.Cells[COL_COMP].FindControl("optNull") as RadioButton;

				if (radioButton!=null && radioButton.Enabled)
				{
					this.RegisterClientScript("SetFocus","SetControlFocus('" + radioButton.ClientID + "');");
					break;
				}
			}
		}

		#endregion

		#region Private members
		
		/// <summary>
		/// Gestione abilitazione / disabilitazione pulsante cancellazione radiobuttons
		/// </summary>
		/// <param name="isEnabled"></param>
		/// <param name="parentCtl"></param>
		private void EnableButtonClearOptions(bool isEnabled,System.Web.UI.Control parentCtl)
		{
			foreach (System.Web.UI.Control ctl in parentCtl.Controls)
			{
				if (ctl.ID!=null && ctl.ID.Equals("btnClearOptions"))
				{
					DocsPaWebCtrlLibrary.ImageButton imgButton=
						(DocsPaWebCtrlLibrary.ImageButton) ctl;
					imgButton.Enabled=isEnabled;
					break;
				}

				this.EnableButtonClearOptions(isEnabled,ctl);
			}
		}

		private void LoadItemValues(DataGridItem grdItem,
									out string id,
									out string type,
									out bool flagComp,
									out bool flagCC)
		{
			id=grdItem.Cells[COL_ID].Text;
			type=grdItem.Cells[COL_TYPE].Text;
			flagComp=false;
			flagCC=false;			

			RadioButton optComp=grdItem.Cells[COL_COMP].FindControl("optComp") as RadioButton;
			RadioButton optCC=grdItem.Cells[COL_CC].FindControl("optCC") as RadioButton;
						
			if (optComp!=null && optCC!=null && optComp.Visible && optCC.Visible)
			{
				flagComp=optComp.Checked;
				flagCC=optCC.Checked;			
			}
		}

		/// <summary>
		/// Reperimento, in base all'idUO,
		/// di un oggetto "UOSmistamento" dalla sessione
		/// </summary>
		/// <param name="idUO"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.UOSmistamento GetUOFromSession(string idUO)
		{
            DocsPAWA.DocsPaWR.UOSmistamento retValue = null;

			// Reperimento array oggetti "UOSmistamento" dalla sessione
            DocsPAWA.DocsPaWR.UOSmistamento[] listUO = GetSessionListUO();

			foreach (DocsPAWA.DocsPaWR.UOSmistamento uo in listUO)
			{
				if (uo.ID.Equals(idUO))
				{
					retValue=uo;
					break;
				}
			}

			return retValue;
		}

		private void SetSessionListUO(DocsPAWA.DocsPaWR.UOSmistamento[] listUO)
		{
			Session["SmistaUO.ListUO"]=listUO;
		}

		private DocsPAWA.DocsPaWR.UOSmistamento[] GetSessionListUO()
		{
			return Session["SmistaUO.ListUO"] as DocsPAWA.DocsPaWR.UOSmistamento[];
		}

		/// <summary>
		/// Reperimento lista UO cui è possibile smistare il documento
		/// </summary>
		/// <param name="idRegistro"></param>
		/// <param name="mittente"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.UOSmistamento[] GetListUOSmistamento(string idRegistro,DocsPAWA.DocsPaWR.MittenteSmistamento mittente)
		{
            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
			return ws.GetUOSmistamento(idRegistro,mittente);
		}

		/// <summary>
		/// Creazione oggetto MittenteSmistamento
		/// </summary>
		private DocsPAWA.DocsPaWR.MittenteSmistamento CreateMittenteSmistamento()
		{
            ProtocollazioneIngresso.Login.LoginMng loginMng = new ProtocollazioneIngresso.Login.LoginMng(this.Page);
            DocsPAWA.DocsPaWR.InfoUtente infoUtente = loginMng.GetInfoUtente();
            DocsPAWA.DocsPaWR.Utente utente = loginMng.GetUtente();
            DocsPAWA.DocsPaWR.Ruolo ruolo = loginMng.GetRuolo();
			loginMng=null;

            DocsPAWA.DocsPaWR.MittenteSmistamento retValue = new DocsPAWA.DocsPaWR.MittenteSmistamento();
			retValue.IDPeople=infoUtente.idPeople;
			retValue.IDAmministrazione=infoUtente.idAmministrazione;
			
			string[] registriApp=new string[ruolo.registri.Length];
			for (int i=0;i<ruolo.registri.Length;i++)
			{
				registriApp[i]=ruolo.registri[i].systemId;
			}
			retValue.RegistriAppartenenza=registriApp;
			registriApp=null;

			retValue.EMail=utente.email;
			retValue.IDCorrGlobaleRuolo=ruolo.systemId;
			retValue.IDGroup=ruolo.idGruppo;
			retValue.LivelloRuolo=ruolo.uo.livello;

			return retValue;
		}

		private void RegisterClientEvents()
		{
			
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

		/// <summary>
		/// Verifica se l'UO ha almeno un ruolo di riferimento
		/// </summary>
		/// <param name="uo"></param>
		/// <param name="listRuoliRiferimento">retvalue, stringa contenente i ruoli di riferimento</param>
		/// <returns></returns>
		private bool HasRuoliRiferimento(DocsPAWA.DocsPaWR.UOSmistamento uo,out string listRuoliRiferimento)
		{
			bool retValue=false;
			listRuoliRiferimento=string.Empty;

			foreach (DocsPAWA.DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
			{
				if (ruolo.RuoloRiferimento)
				{
					if (listRuoliRiferimento!=string.Empty)
						listRuoliRiferimento += "\n";

					listRuoliRiferimento += " - " + ruolo.Codice + " (" + ruolo.Descrizione + ")";

					retValue=true;
				}
			}

			if (listRuoliRiferimento!=string.Empty)
				listRuoliRiferimento="Ruoli di riferimento:\n\n" + listRuoliRiferimento;

			return retValue;
		}

		#region Gestione griglia UO

		private DataSet CreateGridDataSet()
		{
			DataSet retValue=new DataSet();

			DataTable dt=new DataTable("GRID_TABLE");
			
			dt.Columns.Add("ID",typeof(string));
			dt.Columns.Add("TYPE",typeof(string));
			dt.Columns.Add("DESCRIPTION",typeof(string));
			dt.Columns.Add("HAS_RUOLI_RIF",typeof(bool));
			dt.Columns.Add("LIST_RUOLI_RIF",typeof(string));

			retValue.Tables.Add(dt);

			return retValue;
		}

		private void AppendDataRow(DataTable dt,
									string type,
									string id,
									string descrizione,
									bool hasRuoliRiferimento,
									string listRuoliRif)
		{
			DataRow row=dt.NewRow();
			row["ID"]=id;
			row["TYPE"]=type;
			row["DESCRIPTION"]=this.GetImage(type,hasRuoliRiferimento) + " " + descrizione;
			row["HAS_RUOLI_RIF"]=hasRuoliRiferimento;
			row["LIST_RUOLI_RIF"]=listRuoliRif;
			dt.Rows.Add(row);
			row=null;
		}

		private string GetImage(string rowType,bool hasRuoliRiferimento)
		{
			string retValue=string.Empty;
			string spaceIndent=string.Empty;

			switch (rowType)
			{
				case "U":
					if (hasRuoliRiferimento)
						retValue="uo_ruoli";
					else
						retValue="uo";

					break;

				case "R":
					retValue="ruolo_noexp";
					spaceIndent="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
					break;

				case "P":
					retValue="utente_noexp";
					spaceIndent="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
					break;
			}

			retValue=spaceIndent + "<img src='Smistamento/Images/" + retValue + ".gif' border='0'>";
			
			return retValue;
		}
		
		/// <summary>
		/// Gestione aggiornamento controlli per ciascun elemento della griglia
		/// relativamente alla presenza di almeno un ruolo di riferimento nella UO
		/// </summary>
		private void RefreshGridItems()
		{
			foreach (DataGridItem item in this.grdUOApp.Items)
			{
				RadioButton radioButtonComp=(RadioButton) item.Cells[COL_COMP].FindControl("optComp");
				RadioButton radioButtonCC=(RadioButton) item.Cells[COL_CC].FindControl("optCC");
				RadioButton radioButtonNull=(RadioButton) item.Cells[COL_RESET].FindControl("optNull");

				// Ruolo di riferimento
				bool hasRuoliRiferimento=false;
				try
				{
					hasRuoliRiferimento=Convert.ToBoolean(item.Cells[COL_HAS_RUOLI_RIF].Text);
				}
				catch {}

				if (radioButtonComp!=null)
					radioButtonComp.Enabled=hasRuoliRiferimento;
				if (radioButtonCC!=null)
					radioButtonCC.Enabled=hasRuoliRiferimento;
				if (radioButtonNull!=null)
					radioButtonNull.Enabled=hasRuoliRiferimento;

				string toolTip=string.Empty;

				if (!hasRuoliRiferimento)
					toolTip="Nessun ruolo di riferimento impostato, impossibile smistare il documento a questa UO";
				else
					toolTip=item.Cells[COL_LIST_RUOLI_RIF].Text;

				item.Cells[COL_DESCRIPTION].ToolTip=toolTip;
			}
		}

        private void OnDataGridItemCreated(object sender, DataGridItemEventArgs e)
        {
            ImageButton button = (ImageButton)e.Item.Cells[COL_RESET].FindControl("btnClearOptions");

            tipoProto = Session["tipoProto"].ToString();

            if (button != null)
                button.Attributes.Add("onClick", "ResetRadioButtons('SELEZIONE');");

            RadioButton radioButtonComp = (RadioButton)e.Item.Cells[COL_COMP].FindControl("optComp");
            RadioButton radioButtonCC = (RadioButton)e.Item.Cells[COL_CC].FindControl("optCC");
            RadioButton radioButtonNull = (RadioButton)e.Item.Cells[COL_RESET].FindControl("optNull");


            if (radioButtonComp != null)
            {
                if (tipoProto.Equals("A"))
                {
                    radioButtonComp.Attributes.Add("onClick", "RefreshButtonProtocollaEnabled();");
                }
                else
                {
                    radioButtonComp.Attributes.Add("onClick", "RefreshButtonProtocollaUscitaEnabled();");
                }
            }
            if (radioButtonCC != null)
            {
                if (tipoProto.Equals("A"))
                {
                    radioButtonCC.Attributes.Add("onClick", "RefreshButtonProtocollaEnabled();");
                }
                else
                {
                    radioButtonCC.Attributes.Add("onClick", "RefreshButtonProtocollaUscitaEnabled();");
                }
                if (radioButtonNull != null)
                {
                    if (tipoProto.Equals("A"))
                    {
                        radioButtonNull.Attributes.Add("onClick", "RefreshButtonProtocollaEnabled();");
                    }
                    else
                    {
                        radioButtonNull.Attributes.Add("onClick", "RefreshButtonProtocollaUscitaEnabled();");
                    }
                }
            }


        }

		/// <summary>
		/// Gestione impostazione pulsante di protocollazione come pulsante di default.
		/// E' necessario impostare un handler di evento javascript per ogni controllo 
		/// editabile sul quale ci si può posizionare.
		/// </summary>
		public void SetDefaultButtonJSHandler(ImageButton btn)
		{
			foreach (DataGridItem itm in this.grdUOApp.Items)
			{
				RadioButton radioButton=(RadioButton) itm.Cells[COL_COMP].FindControl("optComp");
				if (radioButton!=null)
					DocsPAWA.Utils.DefaultButton(this.Page,ref radioButton,ref btn);

				radioButton=(RadioButton) itm.Cells[COL_CC].FindControl("optCC");
				if (radioButton!=null)
					DocsPAWA.Utils.DefaultButton(this.Page,ref radioButton,ref btn);

				radioButton=(RadioButton) itm.Cells[COL_RESET].FindControl("optNull");
				if (radioButton!=null)
					DocsPAWA.Utils.DefaultButton(this.Page,ref radioButton,ref btn);
			}
		}

		#endregion

     
		#endregion

	}
}
