namespace DocsPAWA.SitoAccessibile.Documenti
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using DocsPAWA.DocsPaWR;

	/// <summary>
	///	UserControl che gestisce la visualizzazione delle 
	///	possibili parole chiavi da assoficare ad un documento
	/// </summary>
	public class ListaParoleChiavi : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.CheckBoxList listParoleChiavi;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
				this.Fetch();
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

//		/// <summary>
//		/// Abilitazione / disabilitazione modalità inserimento parola chiave
//		/// </summary>
//		public bool AllowInsertMode
//		{
//			get
//			{
//				return false;
//			}
//			set
//			{
//
//			}
//		}

		/// <summary>
		/// Reperimento delle parole chiavi selezionate
		/// </summary>
		/// <returns></returns>
		public DocumentoParolaChiave[] GetSelectedParoleChiavi()
		{
			ArrayList retValue=new ArrayList();
			
			foreach (ListItem item in this.listParoleChiavi.Items)
			{
				if (item.Selected)
				{
					ParolaChiave parolaChiave=this.GetParolaChiaveViewState(item.Value);

					DocumentoParolaChiave docParolaChiave=new DocumentoParolaChiave();
					docParolaChiave.idAmministrazione=parolaChiave.IDAmministrazione;
					docParolaChiave.systemId=parolaChiave.ID;
					docParolaChiave.descrizione=parolaChiave.Descrizione;
					retValue.Add(docParolaChiave);
				}
			}

			return (DocumentoParolaChiave[]) retValue.ToArray(typeof(DocumentoParolaChiave));
		}

		/// <summary>
		/// Caricamento dati parole chiavi
		/// </summary>
		private void Fetch()
		{
			DocumentoHandler handler=new DocumentoHandler();
			DocumentoParolaChiave[] items=handler.GetParoleChiavi();

			if (items.Length==0)
			{
				this.listParoleChiavi.Items.Add(this.GetListItemParolaChiave(null));
			}
			else
			{
				foreach (DocumentoParolaChiave item in items)
				{
					this.listParoleChiavi.Items.Add(this.GetListItemParolaChiave(item));
					
					this.AddParolaChiaveViewState(new ParolaChiave(item.idAmministrazione,item.systemId,item.descrizione));
				}
			}
		}

		/// <summary>
		/// Inserimento oggetto parola chiave in viewstate
		/// </summary>
		/// <param name="parolaChiave"></param>
		private void AddParolaChiaveViewState(ParolaChiave parolaChiave)
		{
			this.ViewState[parolaChiave.ID]=parolaChiave;
		}

		/// <summary>
		/// Reperimento oggetto parola chiave da viewstate
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private ParolaChiave GetParolaChiaveViewState(string id)
		{
			if (this.ViewState[id]!=null)
				return (ParolaChiave) this.ViewState[id];
			else
				return null;
		}


		/// <summary>
		/// Creazione elemento parola chiave
		/// </summary>
		/// <param name="parolaChiave"></param>
		/// <returns></returns>
		private ListItem GetListItemParolaChiave(DocumentoParolaChiave parolaChiave)
		{
			ListItem retValue=null;

			if (parolaChiave==null)
				retValue=new ListItem(string.Empty,string.Empty);
			else
				retValue=new ListItem(parolaChiave.descrizione,parolaChiave.systemId);

			return retValue;
		}

		/// <summary>
		/// 
		/// </summary>
		[Serializable()]
		private class ParolaChiave
		{
			public ParolaChiave(string idAmministrazione,string id,string descrizione)
			{
				this.IDAmministrazione=IDAmministrazione;
				this.ID=id;
				this.Descrizione=descrizione;
			}

			public string IDAmministrazione=string.Empty;
			public string ID=string.Empty;
			public string Descrizione=string.Empty;
		}
	}
}