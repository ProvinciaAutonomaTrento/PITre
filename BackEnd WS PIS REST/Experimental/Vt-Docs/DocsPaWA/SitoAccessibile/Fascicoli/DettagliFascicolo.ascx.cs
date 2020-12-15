namespace DocsPAWA.SitoAccessibile.Fascicoli
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DocsPAWA.DocsPaWR;
	using SitoAccessibile.WebControls;
	using DocsPAWA.SitoAccessibile.Trasmissioni;
	using SitoAccessibile.Documenti.Trasmissioni;
	using SitoAccessibile.Paging;

	/// <summary>
	///	Dettaglio di un fascicolo
	/// </summary>
	public class DettagliFascicolo : System.Web.UI.UserControl
	{
		private string _idFascicolo=string.Empty;
		private string _idRegistro=string.Empty;
		private bool _readOnlyMode=false;

		protected System.Web.UI.WebControls.TextBox txtClassifica;
		protected System.Web.UI.WebControls.TextBox txtCodice;
		protected System.Web.UI.WebControls.TextBox txtTipo;
		protected System.Web.UI.WebControls.TextBox txtStato;
		protected System.Web.UI.WebControls.TextBox txtDescrizione;
		protected System.Web.UI.WebControls.TextBox txtNote;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblClassifica;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblCodice;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblTipo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblStato;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDescrizione;
		protected System.Web.UI.WebControls.RadioButtonList rblSearchType;
		protected System.Web.UI.WebControls.Button btnSearch;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblNote;
		

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Associazione attributo for per i campi label richiesti
			this.BindLabelsToFields();
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
			{
				// Disabilitazione campi in modalità readonly
				this.DisableReadOnlyFields();
			}
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
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new EventHandler(this.Page_PreRender);

		}
		#endregion

		/// <summary>
		/// Caricamento dati fascicolo
		/// </summary>
		/// <param name="idFascicolo"></param>
		/// <param name="idRegistro"></param>
		/// <param name="readOnlyMode"></param>
		public void Fetch(string idFascicolo,string idRegistro,bool readOnlyMode)
		{
			this._idFascicolo=idFascicolo;
			this._idRegistro=idRegistro;
			this._readOnlyMode=readOnlyMode;

			if (this._idFascicolo==string.Empty)
				throw new ArgumentException("Parametro 'idFascicolo' mancante","idFascicolo");

			DocsPaWR.Fascicolo fascicolo=this.GetFascicolo();

			if (fascicolo==null)
				throw new ApplicationException("Impossibile reperire il fascicolo, si potrebbero non avere i diritti sufficienti per la visualizzazione");

            this.txtClassifica.Text = this.GetCodiceTitolario(fascicolo);
			this.txtCodice.Text=fascicolo.codice;
			this.txtTipo.Text=TipiFascicolo.GetDescrizione(fascicolo.tipo);
			this.txtStato.Text=StatiFascicolo.GetDescrizione(fascicolo.stato);
			this.txtDescrizione.Text=fascicolo.descrizione;

            Note.INoteManager noteManager = new Note.FascicoloNoteManager(fascicolo);
            this.txtNote.Text = noteManager.GetUltimaNotaAsString();
            
			this.FetchTreeView(fascicolo);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        private string GetCodiceTitolario(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
        {
            FascicoloHandler handler = new FascicoloHandler();
            return handler.GetCodiceTitolario(fascicolo);
        }

		/// <summary>
		/// Verifica se il dettaglio del fascicolo è in modalità readonly
		/// </summary>
		/// <returns></returns>
		public bool IsReadOnlyMode()
		{
			return this._readOnlyMode;
		}

		/// <summary>
		/// Disabilitazione campi readonly
		/// </summary>
		private void DisableReadOnlyFields()
		{
			bool readOnlyMode=this.IsReadOnlyMode();
			
			this.txtClassifica.ReadOnly=readOnlyMode;
			this.txtCodice.ReadOnly=readOnlyMode;
			this.txtTipo.ReadOnly=readOnlyMode;
			this.txtStato.ReadOnly=readOnlyMode;
			this.txtDescrizione.ReadOnly=readOnlyMode;
			this.txtNote.ReadOnly=readOnlyMode;
		}

		/// <summary>
		/// Reperimento del fascicolo da visualizzare
		/// </summary>
		/// <param name="idFascicolo"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.Fascicolo GetFascicolo()
		{
			FascicoloHandler fascicoloHandler=new FascicoloHandler();
			return fascicoloHandler.GetFascicolo(this._idFascicolo,this._idRegistro);
		}

		/// <summary>
		/// Reperimento folder del fascicolo corrente
		/// </summary>
		/// <param name="fascicolo"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.Folder GetFolder(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
		{
			FascicoloHandler fascicoloHandler=new FascicoloHandler();
			return fascicoloHandler.GetFolder(fascicolo);
		}

		/// <summary>
		/// Associazione attributo for per i campi label richiesti
		/// </summary>
		private void BindLabelsToFields()
		{
			this.BindLabelToField(this.lblClassifica,this.txtClassifica);
			this.BindLabelToField(this.lblCodice,this.txtCodice);
			this.BindLabelToField(this.lblTipo,this.txtTipo);
			this.BindLabelToField(this.lblStato,this.txtStato);
			this.BindLabelToField(this.lblDescrizione,this.txtDescrizione);
			this.BindLabelToField(this.lblNote,this.txtNote);
		}

		/// <summary>
		/// Associazione attributo "for"
		/// </summary>
		/// <param name="labelControl"></param>
		/// <param name="controlToBind"></param>
		private void BindLabelToField(HtmlGenericControl labelControl,System.Web.UI.Control controlToBind)
		{
			labelControl.Attributes.Add("for",controlToBind.ClientID);
		}

		/// <summary>
		/// Caricamento albero subfolders del fascicolo
		/// </summary>
		private void FetchTreeView(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
		{
			this.GetControlTreeViewFolders().Nodes.Clear();

			DocsPaWR.Folder folder=this.GetFolder(fascicolo);

			this.FillTreeView(null,folder,fascicolo.codice);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="folder"></param>
		private void FillTreeView(AccessibleTreeView.AccessibleTreeNode parentNode,DocsPaWR.Folder folder,string codiceFascicoloRoot)
		{
			AccessibleTreeView.AccessibleTreeNode treeNode=this.CreateTreeNode(folder,codiceFascicoloRoot);
			
			if (parentNode!=null)
				parentNode.ChildNodes.Add(treeNode);
			else
				this.GetControlTreeViewFolders().Nodes.Add(treeNode);
			
			foreach (DocsPAWA.DocsPaWR.Folder childFolder in folder.childs)
				this.FillTreeView(treeNode,childFolder,codiceFascicoloRoot);
		}

		/// <summary>
		/// Creazione nuovo nodo
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="fascicolo"></param>
		/// <returns></returns>
		private AccessibleTreeView.AccessibleTreeNode CreateTreeNode(DocsPAWA.DocsPaWR.Folder folder,string codiceFascicoloRoot)
		{
			string nodeText=string.Empty;
			if (folder.descrizione.IndexOf("Root")==-1)
				nodeText=folder.descrizione;
			else
				nodeText=codiceFascicoloRoot;

			AccessibleTreeView.AccessibleTreeNode retvalue=
				new AccessibleTreeView.AccessibleTreeNode("id_" + folder.systemID,
					nodeText,
					EnvironmentContext.RootPath + "Fascicoli/DettagliDocumentiFascicolo.aspx?idFascicolo=" + this._idFascicolo + "&idFolder=" + folder.systemID + "&idRegistro=" + this._idRegistro);

			retvalue.Expand();

			return retvalue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private AccessibleTreeView GetControlTreeViewFolders()
		{
			return this.FindControl("trvFolders") as AccessibleTreeView;
		}

		/// <summary>
		/// Caricamento trasmissioni del fascicolo
		/// </summary>
		private void SearchTrasmissioni()
		{
			this.HideControlsTrasmissioni();

			TrasmissioniFascicolo trasmissioniFascicolo=(TrasmissioniFascicolo) this.GetControlTrasmissioni(this.GetSelectedSearchType());
			trasmissioniFascicolo.Visible=true;
			trasmissioniFascicolo.Fetch(this._idFascicolo,this._idRegistro);
		}

		/// <summary>
		/// Ricerca trasmissioni fascicolo
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			this.SearchTrasmissioni();
		}

		/// <summary>
		/// Reperimento tipologia ricerca trasmissioni
		/// </summary>
		/// <returns></returns>
		private SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum GetSelectedSearchType()
		{
			SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum retValue=TipiTrasmissioniEnum.Effettuate;

			try
			{
				retValue=(SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum) Enum.Parse(typeof(SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum),this.rblSearchType.SelectedItem.Value,true);
			}
			catch
			{
			}

			return retValue;
		}

		private void HideControlsTrasmissioni()
		{
			this.GetControlTrasmissioni(TipiTrasmissioniEnum.Effettuate).Visible=false;
			this.GetControlTrasmissioni(TipiTrasmissioniEnum.Ricevute).Visible=false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private Control GetControlTrasmissioni(TipiTrasmissioniEnum tipoTrasmissione)
		{
			string controlID=string.Empty;

			if (tipoTrasmissione.Equals(TipiTrasmissioniEnum.Effettuate))
				controlID="trasmissioniEffettuate";
			else
				controlID="trasmissioniRicevute";

			return this.FindControl(controlID);
		}
	}
}