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
using Microsoft.Web.UI.WebControls;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;


namespace DocsPAWA.organigramma
{
	/// <summary>
	/// Classe per la funzionalità di visualizzazione e stampa organigramma.
	/// Nuova funzionalità prevista per la versione 3.2.0
	/// gadamo : Settembre-Ottobre 2006
	/// </summary>
	public class Stampa_Organigramma : DocsPAWA.CssPage
	{
		#region WebControls e variabili
		protected System.Web.UI.WebControls.TextBox txt_ricCod;
		protected System.Web.UI.WebControls.TextBox txt_ricDesc;
		protected System.Web.UI.WebControls.ImageButton btn_find;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;
		protected Microsoft.Web.UI.WebControls.TreeView treeViewUO;
		protected System.Web.UI.HtmlControls.HtmlTableCell td_descRicerca;        
		protected System.Web.UI.WebControls.DropDownList ddl_visualizza;
		protected System.Web.UI.WebControls.DropDownList ddl_ricTipo;
		protected System.Web.UI.WebControls.ImageButton btn_clear;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_lastReturnValueModal;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idAmm;
		protected System.Web.UI.WebControls.ImageButton btn_stampa;
		protected System.Web.UI.WebControls.ImageButton btn_impostaRoot;
		protected System.Web.UI.WebControls.ImageButton btn_espandeOrg;
		protected System.Web.UI.WebControls.ImageButton btn_root;
		protected System.Web.UI.WebControls.ImageButton btn_back;	
		private string HttpFullPath;
        protected string appTitle;

        private DocsPAWA.DocsPaWR.OrgRegistro[] _RFList;
        private DocsPAWA.DocsPaWR.OrgRuolo[] _ruoliConRFselezionato;
        protected System.Web.UI.HtmlControls.HtmlTableCell td_codRicerca;
        protected System.Web.UI.WebControls.DropDownList ddl_RF;
        protected System.Web.UI.WebControls.ImageButton btn_findRF;
        private bool _ricercaRFimpostata = false;

		#endregion

		#region classe myTreeNode
		public class myTreeNode : Microsoft.Web.UI.WebControls.TreeNode
		{
			// Tipo Nodo [Possibili Valori: U=(Unità organizz.), R=(Ruolo), U=(Utente) ]
			public string getTipoNodo() 
			{ 				
				return ViewState["TipoNodo"].ToString();
			}
			public void setTipoNodo(string id) 
			{ 
				ViewState["TipoNodo"] = id;
			}

			public string getRuoliUO() 
			{ 				
				return ViewState["RuoliUO"].ToString();
			}
			public void setRuoliUO(string id) 
			{ 
				ViewState["RuoliUO"] = id;
			}

			public string getSottoUO() 
			{ 				
				return ViewState["SottoUO"].ToString();
			}
			public void setSottoUO(string id) 
			{ 
				ViewState["SottoUO"] = id;
			}

			public string getLivello() 
			{ 				
				return ViewState["Livello"].ToString();
			}
			public void setLivello(string id) 
			{ 
				ViewState["Livello"] = id;
			}	
		}
		#endregion

		#region classe UOSort
		public class UOSort:IComparer
		{
			public UOSort() : base() {}

			int IComparer.Compare(object x,object y)
			{
				DocsPaWR.OrgUO UoX = (DocsPAWA.DocsPaWR.OrgUO) x;
				DocsPaWR.OrgUO UoY = (DocsPAWA.DocsPaWR.OrgUO) y;
				return UoX.Descrizione.CompareTo(UoY.Descrizione);							
			}
		}
		#endregion

        private int convertToInt(string value)
        {
            int result = 0;
            bool ok = false;
            if (value.ToUpper().Equals("A"))
            {
                result = 10;
                ok = true;
            }
            if (value.ToUpper().Equals("B"))
            {
                result = 11;
                ok = true;
            }
            if (value.ToUpper().Equals("C"))
            {
                result = 12;
                ok = true;
            }
            if (value.ToUpper().Equals("D"))
            {
                result = 13;
                ok = true;
            }
            if (value.ToUpper().Equals("E"))
            {
                result = 14;
                ok = true;
            }
            if (value.ToUpper().Equals("F"))
            {
                result = 15;
                ok = true;
            }

            if (!ok)
                result = Convert.ToInt16(value);

            return result;
        }

		#region Page Load
		/// <summary>
		/// Page Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetCacheability (HttpCacheability.NoCache);
			Response.Expires = -1;
            Session["Bookmark"] = "GestioneOrganigramma";
            string titolo = System.Configuration.ConfigurationManager.AppSettings["TITLE"];
            if (titolo != null)
                this.appTitle = titolo;
            else
                this.appTitle = "DOCSPA";

            string color = string.Empty;
            string Tema = GetCssAmministrazione();
            if (Tema != null && !Tema.Equals(""))
            {
                string[] realTema = Tema.Split('^');
                color = realTema[2];
            }
            else
                color = "810d06";

            string red = color.Substring(0, 2);
            string green = color.Substring(2, 2);
            string blu = color.Substring(4);
            int iRed = (convertToInt(red.Substring(0, 1)) * 16) + (convertToInt(red.Substring(1)));
            int iGreen = (convertToInt(green.Substring(0, 1)) * 16) + (convertToInt(green.Substring(1)));
            int iBlu = (convertToInt(blu.Substring(0, 1)) * 16) + (convertToInt(blu.Substring(1)));

            this.treeViewUO.BorderColor = System.Drawing.Color.FromArgb(iRed, iGreen, iBlu);

			try
			{
				HttpFullPath = DocsPAWA.Utils.getHttpFullPath(this);
				this.treeViewUO.SystemImagesPath = HttpFullPath + "/AdminTool/Images/treeimages/";

				if(!IsPostBack)
				{
					DocsPaWR.Ruolo ruolo = UserManager.getRuolo(this);
					this.hd_idAmm.Value = ruolo.idAmministrazione;
					
					this.btn_find.Attributes.Add("onClick","VisualizzaAttendi(); ApriRisultRic('"+this.hd_idAmm.Value+"');");
					this.btn_clear.Attributes.Add("onClick","VisualizzaAttendi();");
					this.btn_espandeOrg.Attributes.Add("onClick","if(!window.confirm('Questa operazione potrebbe richiedere un tempo di attesa piuttosto lungo.\\n\\nProcedere comunque?')) {return false}; VisualizzaAttendi();");
					this.btn_impostaRoot.Attributes.Add("onClick","VisualizzaAttendi();");
					this.btn_root.Attributes.Add("onClick","VisualizzaAttendi();");
					this.ddl_visualizza.Attributes.Add("onChange","VisualizzaAttendi();");
					this.btn_stampa.Attributes.Add("onClick","VisualizzaAttendi();");

					this.PermetteRefreshPagina(true);

					this.VisualizzaNodoRoot();

                    // gestione degli RF - gadamo agosto/2008
                    this.verificaAbilitazioneRF();
				}
				else
				{
					// gestione del valore di ritorno della modal Dialog (ricerca)
					if(this.hd_returnValueModal.Value != null && this.hd_returnValueModal.Value != string.Empty && this.hd_returnValueModal.Value != "undefined")
					{
						this.PermetteRefreshPagina(true);
						this.VisualizzaNodoRicercato(this.hd_returnValueModal.Value);
					}					
				}
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}			
		}
		#endregion		

        private string GetCssAmministrazione()
        {
            string Tema = string.Empty;
            if (UserManager.getInfoUtente() != null)
            {
                string idAmm = UserManager.getInfoUtente().idAmministrazione;
                DocsPAWA.UserManager userM = new UserManager();
                Tema = userM.getCssAmministrazione(idAmm);
            }
            return Tema;
        }

		#region Visualizzazione dei dati nella Treeview

		/// <summary>
		/// Visualizza come nodo Root la UO di livello 0
		/// </summary>
		private void VisualizzaNodoRoot()
		{
			string idCorrGlob = string.Empty;
			Amministrazione.Manager.OrganigrammaManager manager = new Amministrazione.Manager.OrganigrammaManager();
			manager.ListaUOLivelloZero(this.hd_idAmm.Value);
			ArrayList lista = manager.getListaUO();

			if(lista!=null && lista.Count>0)
			{
				foreach(DocsPAWA.DocsPaWR.OrgUO uo in lista)
					idCorrGlob = uo.IDCorrGlobale;

				this.VisualizzaNodoRicercato(idCorrGlob+"_0_U");
			}
		}

		/// <summary>
		/// Visualizza nella Treeview il dato ricercato
		/// </summary>
		/// <param name="returnValue"></param>
		private void VisualizzaNodoRicercato(string returnValue)
		{
			try
			{			
				myTreeNode treenode;

				this.hd_returnValueModal.Value = "";
				this.hd_lastReturnValueModal.Value = returnValue;

				/*
					possibili valori di ritorno:
					
						idCorrGlobUO_idParentUO_U					=	ricerca traUO
						idCorrGlobRuolo_idCorrGlobUO_R				=	ricerca tra ruoli
						idCorrGlobPersona_idCorrGlobRuolo_<PN>/<PC>	=	ricerca tra utenti
				*/

				string[] appo = returnValue.Split('_');
				string idCorrGlobale = appo[0];
				string idParent = appo[1];	
				string tipo = appo[2];

				switch(tipo)
				{
					case "R":
						idCorrGlobale = idParent;
						break;
					
					case "PN":
					case "PC":
						idCorrGlobale = this.GetUOPadre(idParent,tipo);
						break;					
				}
				
				DocsPaWR.OrgUO currentUO = this.GetDatiUOCorrente(idCorrGlobale);
				if(currentUO!=null)
				{					
					// diventa la ROOT della treeview
					treenode = new myTreeNode();
					treenode = this.SetRootTreeview(currentUO);

					this.SetRootTreeviewExpanded();	
				}
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}				
		
		/// <summary>
		/// Reperimento dati della UO passata come parametro
		/// </summary>
		/// <param name="idCorrGlobale"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.OrgUO GetDatiUOCorrente(string idCorrGlobale)
		{
			DocsPaWR.OrgUO currentUO;

			Amministrazione.Manager.OrganigrammaManager manager = new Amministrazione.Manager.OrganigrammaManager();
			manager.DatiUOCorrente(idCorrGlobale);

			currentUO = manager.getDatiUO();	

			return currentUO;
		}

		private void TornaAllaUOPadre()
		{			
			if(this.treeViewUO.Nodes.Count>0)
			{
				myTreeNode nodoRoot = (myTreeNode)this.treeViewUO.GetNodeFromIndex("0");
				DocsPaWR.OrgUO currentUO = this.GetDatiUOCorrente(nodoRoot.ID);
				if(currentUO!=null && !currentUO.IDParent.Equals("0"))
				{
					DocsPaWR.OrgUO UOpadre = this.GetDatiUOCorrente(currentUO.IDParent);
					if(UOpadre!=null)
					{
						// diventa la ROOT della treeview
						myTreeNode treenode = new myTreeNode();
						treenode = this.SetRootTreeview(UOpadre);

						this.SetRootTreeviewExpanded();	

						this.ImpostaValoreDiRicercaNascosto(UOpadre.IDCorrGlobale + "_" + UOpadre.IDParent + "_U");							
					}
				}
			}
		}

		/// <summary>
		/// Visualizza partendo dalla UO selezionata in Treeview
		/// </summary>
		private void VisualizzaUOSelezionata()
		{
			try
			{
				if(this.treeViewUO.SelectedNodeIndex!=string.Empty || this.treeViewUO.SelectedNodeIndex!="")
				{
					myTreeNode TreeNodo;
					TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
					
					if(TreeNodo.getTipoNodo().Equals("U")) // solo se è una UO
					{
						DocsPaWR.OrgUO currentUO = this.GetDatiUOCorrente(TreeNodo.ID);
						if(currentUO!=null)
						{
							// diventa la ROOT della treeview
							myTreeNode treenode = new myTreeNode();
							treenode = this.SetRootTreeview(currentUO);

							this.SetRootTreeviewExpanded();	

							this.ImpostaValoreDiRicercaNascosto(currentUO.IDCorrGlobale + "_" + currentUO.IDParent + "_U");							
						}
					}
				}
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}

		/// <summary>
		/// Espande tutto l'organigramma dalla Root
		/// </summary>
		private void EspandeOrgDallaRoot()
		{
            if(this.treeViewUO.Nodes.Count>0)
			{
			    myTreeNode TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex("0");			    			
			    this.ricorsioneCercaFigli(TreeNodo);
            }
		}

		/// <summary>
		/// Ricorsione sui nodi figli della treeview
		/// </summary>
		/// <param name="nodo"></param>
		private void ricorsioneCercaFigli(myTreeNode nodo)
		{		
			if(nodo.getTipoNodo().Equals("U"))
			{
				this.executeExpand(nodo.GetNodeIndex());

				foreach(myTreeNode currentNode in nodo.Nodes)
					this.ricorsioneCercaFigli(currentNode);		
			}
		}

		/// <summary>
		/// Visualizza le sotto UO
		/// </summary>
		/// <param name="currentUO"></param>
		/// <param name="indice"></param>
		/// <returns></returns>
		private bool SottoUO(DocsPAWA.DocsPaWR.OrgUO currentUO, string indice)
		{			
			bool retValue = true;			

			myTreeNode TreeNodo;
			myTreeNode TreeNodoFiglio;

			if(indice!=null)
				TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(indice);
			else
				TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

			myTreeNode nodoUO;

			ArrayList lista = new ArrayList();

			int livello = Convert.ToInt32(currentUO.Livello) + 1;

			Amministrazione.Manager.OrganigrammaManager manager = new Amministrazione.Manager.OrganigrammaManager();
			manager.ListaUO(currentUO.IDCorrGlobale,Convert.ToString(livello),currentUO.IDAmministrazione);
			lista = manager.getListaUO();			
			
			if(lista != null && lista.Count > 0)
			{
                if (!this.isOrdinamentoAbilitato())
                    lista.Sort(new UOSort());

				foreach(DocsPAWA.DocsPaWR.OrgUO sottoUO in lista)
				{
					nodoUO = new myTreeNode();
	
					nodoUO.ID = sottoUO.IDCorrGlobale;
					nodoUO.Text = sottoUO.CodiceRubrica + " - " + sottoUO.Descrizione;
					nodoUO.ImageUrl = HttpFullPath + "/AdminTool/Images/uo.gif";					
	
					TreeNodo.Nodes.Add(nodoUO);

					nodoUO.setTipoNodo("U");
					nodoUO.setRuoliUO(sottoUO.Ruoli);
					nodoUO.setSottoUO(sottoUO.SottoUo);		
					nodoUO.setLivello(sottoUO.Livello);

					if((this.GetTipoNavigazione()>1 && Convert.ToInt32(sottoUO.Ruoli)>0) || Convert.ToInt32(sottoUO.SottoUo)>0)
					{
						TreeNodoFiglio = new myTreeNode();				
						TreeNodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
						nodoUO.Nodes.Add(TreeNodoFiglio);
					}
				}
			}
			else
				retValue = false;

			return retValue;
		}

		/// <summary>
		/// Visualizza i ruoli di una UO
		/// </summary>
		/// <param name="currentUO"></param>
		/// <param name="indice"></param>
		private void RuoliUO(DocsPAWA.DocsPaWR.OrgUO currentUO,string indice)
		{			
			myTreeNode nodoRuoli;			
			myTreeNode TreeNodo;

			TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(indice);

			Amministrazione.Manager.OrganigrammaManager manager = new Amministrazione.Manager.OrganigrammaManager();
			manager.ListaRuoliUO(currentUO.IDCorrGlobale);
			
			if(manager.getListaRuoliUO()!=null && manager.getListaRuoliUO().Count>0)
			{
				TreeNodo.Expanded = true;	

				foreach(DocsPAWA.DocsPaWR.OrgRuolo ruolo in manager.getListaRuoliUO())
				{
                    // filtro per nuova gestione RF
                    if (this.visualizzaRuolo_filtroRF(ruolo.IDCorrGlobale))
                    {
					    nodoRuoli = new myTreeNode();
    	
					    nodoRuoli.ID = ruolo.IDCorrGlobale;
					    nodoRuoli.Text = ruolo.CodiceRubrica + " - " + ruolo.Descrizione;
					    nodoRuoli.ImageUrl = HttpFullPath + "/AdminTool/Images/ruolo.gif";
                    
                        TreeNodo.Nodes.Add(nodoRuoli);

                        nodoRuoli.setTipoNodo("R");

                        if (this.GetTipoNavigazione().Equals(3) && ruolo.Utenti.Length > 0)
                            this.UtentiRuolo(nodoRuoli, ruolo);
                    }
				}				
			}
		}
		
		/// <summary>
		/// Visualizza gli utenti di un ruolo
		/// </summary>
		/// <param name="nodoRuoli"></param>
		/// <param name="ruolo"></param>
		private void UtentiRuolo(myTreeNode nodoRuoli, DocsPAWA.DocsPaWR.OrgRuolo ruolo)
		{
			myTreeNode nodoUtenti;

			nodoRuoli.Expanded = true;

			foreach(DocsPAWA.DocsPaWR.OrgUtente utente in ruolo.Utenti)
			{
				nodoUtenti = new myTreeNode();

				nodoUtenti.ID = utente.IDCorrGlobale;
				nodoUtenti.Text = utente.CodiceRubrica + " - " + utente.Cognome + " " + utente.Nome;
				nodoUtenti.ImageUrl = HttpFullPath + "/AdminTool/Images/utente.gif";

				nodoRuoli.Nodes.Add(nodoUtenti);	
				
				nodoUtenti.setTipoNodo("P");
			}		
		}		
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
			this.ddl_visualizza.SelectedIndexChanged += new System.EventHandler(this.ddl_visualizza_SelectedIndexChanged);
			this.ddl_ricTipo.SelectedIndexChanged += new System.EventHandler(this.ddl_ricTipo_SelectedIndexChanged);
			this.btn_clear.Click += new System.Web.UI.ImageClickEventHandler(this.btn_clear_Click);
			this.btn_root.Click += new System.Web.UI.ImageClickEventHandler(this.btn_root_Click);
			this.btn_back.Click += new System.Web.UI.ImageClickEventHandler(this.btn_back_Click);
			this.btn_impostaRoot.Click += new System.Web.UI.ImageClickEventHandler(this.btn_impostaRoot_Click);
			this.btn_espandeOrg.Click += new System.Web.UI.ImageClickEventHandler(this.btn_espandeOrg_Click);
			this.btn_stampa.Click += new System.Web.UI.ImageClickEventHandler(this.btn_stampa_Click);
			this.treeViewUO.Expand += new Microsoft.Web.UI.WebControls.ClickEventHandler(this.treeViewUO_Expand);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region Modalità di visualizzazione dei dati in Treeview
		/// <summary>
		/// Selezione modaltà di visualizzazione
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ddl_visualizza_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            // gestione RF
            if (this.verificaStatoRicercaRF())
            {
                switch (ddl_visualizza.SelectedIndex)
                {
                    case 0:
                        this.executeJS("<SCRIPT>alert('Stato ricerca RF: impossibile impostare la visualizzazione per UO');</SCRIPT>");
                        ddl_visualizza.SelectedIndex = 1;
                        return;
                        break;

                    case 1:
                        this.performFindRF(false);
                        return;
                        break;

                    case 2:
                        this.performFindRF(true);
                        return;
                        break;
                }               
            }

			if(this.treeViewUO.Nodes.Count>0)
				if(this.hd_lastReturnValueModal.Value != null && this.hd_lastReturnValueModal.Value != string.Empty && this.hd_lastReturnValueModal.Value != "undefined")
					this.VisualizzaNodoRicercato(this.hd_lastReturnValueModal.Value);
		}

		/// <summary>
		/// Imposta la modalità di visualizzazione rispetto ai filtri di ricerca
		/// </summary>
		private void ImpostaModoVisualizzazione()
		{
			switch (this.ddl_ricTipo.SelectedValue)
			{
				case "U":					
					this.ddl_visualizza.SelectedIndex = 0;
					break;
				case "R":
                case "RF":
					this.ddl_visualizza.SelectedIndex = 1;
					break;
				case "PN":					
				case "PC":                
					this.ddl_visualizza.SelectedIndex = 2;                
					break;
			}
		}

		#endregion

		#region Tipologia di ricerca
		/// <summary>
		/// DDL per impostare la tipologia della ricerca in organigramma
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ddl_ricTipo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch (this.ddl_ricTipo.SelectedValue)
			{
				case "U":
                    this.restoreSearchDefault();
					this.td_descRicerca.InnerHtml = "Nome UO:";
                    this.SetFocus(this.txt_ricDesc);
					break;
				case "R":
                    this.restoreSearchDefault();
					this.td_descRicerca.InnerHtml = "Nome ruolo:";
                    this.SetFocus(this.txt_ricDesc);
					break;
				case "PN":
                    this.restoreSearchDefault();
					this.td_descRicerca.InnerHtml = "Nome utente:";
                    this.SetFocus(this.txt_ricDesc);
					break;
				case "PC":
                    this.restoreSearchDefault();
					this.td_descRicerca.InnerHtml = "Cognome utente:";
                    this.SetFocus(this.txt_ricDesc);
					break;
                case "RF":
                    this.setComboRF();
                    break;
			}			
		}
				
		#endregion
		
        #region Pulsanti eventi
		/// <summary>
		/// Pulsante Ripulisce tutto
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_clear_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            this.restoreSearchDefault();
			this.PulisceTuttaTreeView();
			this.ddl_visualizza.SelectedIndex=0;
			this.ddl_ricTipo.SelectedIndex=0;
			this.txt_ricCod.Text="";
			this.txt_ricDesc.Text="";
			this.hd_returnValueModal.Value="";
			this.hd_lastReturnValueModal.Value="";
			this.PermetteRefreshPagina(true);
		}

		/// <summary>
		/// Visualizza l'organigramma partendo dalla UO principale (livello 0)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_root_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            this.restoreSearchDefault();
			this.ddl_ricTipo.SelectedIndex=0;
			this.txt_ricCod.Text="";
			this.txt_ricDesc.Text="";
			this.PermetteRefreshPagina(true);
			this.VisualizzaNodoRoot();
		}		

		/// <summary>
		/// Imposta come Root la UO selezionata
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_impostaRoot_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			this.ddl_ricTipo.SelectedIndex=0;
			this.txt_ricCod.Text="";
			this.txt_ricDesc.Text="";
			this.PermetteRefreshPagina(true);
			this.VisualizzaUOSelezionata();
		}

		/// <summary>
		/// Espande tutti i nodi della UO root
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_espandeOrg_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            this.restoreSearchDefault();
			this.ddl_ricTipo.SelectedIndex=0;
			this.txt_ricCod.Text="";
			this.txt_ricDesc.Text="";
			this.PermetteRefreshPagina(false);
			this.EspandeOrgDallaRoot();
		}

		/// <summary>
		/// Torna alla UO padre
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_back_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			this.ddl_ricTipo.SelectedIndex=0;
			this.txt_ricCod.Text="";
			this.txt_ricDesc.Text="";
			this.PermetteRefreshPagina(true);
			this.TornaAllaUOPadre();
		}

		#endregion

		#region Treeview
		/// <summary>
		/// Imposta la Root nella Treeview
		/// </summary>
		/// <param name="currentUO"></param>
		/// <returns></returns>
		private myTreeNode SetRootTreeview(DocsPAWA.DocsPaWR.OrgUO currentUO)
		{			
			this.PulisceTuttaTreeView();						

			myTreeNode treenode = new myTreeNode();				

			treenode.ID = currentUO.IDCorrGlobale;
			treenode.Text = currentUO.Codice + " - " + currentUO.Descrizione;

			treeViewUO.Nodes.Add(treenode);	

			treenode.setTipoNodo("U");
			treenode.setRuoliUO(currentUO.Ruoli);
			treenode.setSottoUO(currentUO.SottoUo);		
			treenode.setLivello(currentUO.Livello);

			return treenode;
		}

		/// <summary>
		/// Espande i figli del nodo Root
		/// </summary>
		private void SetRootTreeviewExpanded()
		{
			TreeViewClickEventArgs e = new TreeViewClickEventArgs("0");
			this.treeViewUO_Expand(null,e);
		}

		/// <summary>
		/// Espande un nodo della Treeview
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeViewUO_Expand(object sender, Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e)
		{
			this.executeExpand(e.Node);				
		}

		/// <summary>
		/// Esegue l'espansione del nodo
		/// </summary>
		/// <param name="indice"></param>
		private void executeExpand(string indice)
		{
			DocsPaWR.OrgUO currentUO;	
						
			myTreeNode TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(indice);		
			TreeNodo.Expanded = true;
//			this.treeViewUO.SelectedNodeIndex = indice;
	
			if(TreeNodo.Nodes.Count > 0)
				TreeNodo.Nodes.Clear();

			switch(TreeNodo.getTipoNodo())
			{
				case "U":					

					currentUO = new DocsPAWA.DocsPaWR.OrgUO();
					currentUO.IDCorrGlobale = TreeNodo.ID;
					currentUO.IDAmministrazione = this.hd_idAmm.Value;
					currentUO.Ruoli = TreeNodo.getRuoliUO();
					currentUO.SottoUo = TreeNodo.getSottoUO();
					currentUO.Livello = TreeNodo.getLivello();

					if(this.GetTipoNavigazione()>1 && Convert.ToInt32(currentUO.Ruoli)>0) 
						this.RuoliUO(currentUO,indice);

					if(Convert.ToInt32(currentUO.SottoUo)>0)
						this.SottoUO(currentUO,indice);

					break;
			}										
		}

		#endregion
		
		#region Funzionalità di stampa PDF
		/// <summary>
		/// Converte i dati visualizzati nella Treeview in un file XML
		/// </summary>
		/// <returns></returns>
		private XmlDocument exportToXML()
		{		
			XmlDocument xmldoc = new XmlDocument();	
				
			if(this.treeViewUO.Nodes.Count>0)
			{
				myTreeNode nodo = (myTreeNode)treeViewUO.GetNodeFromIndex("0");

                //gestione RF
                string titoloPDF = string.Empty;
                if (this.verificaStatoRicercaRF())
                    titoloPDF = "Composizione RF: " + this.ddl_RF.Items[this.ddl_RF.SelectedIndex].Text;
                else
                    titoloPDF = "Organigramma";

				XmlNode organigramma = xmldoc.AppendChild(xmldoc.CreateElement("ORGANIGRAMMA"));

                XmlAttribute attrRoot = xmldoc.CreateAttribute("title");
                attrRoot.InnerText = titoloPDF;
                organigramma.Attributes.Append(attrRoot);

				XmlElement record = xmldoc.CreateElement("RECORD");
				record.SetAttribute("tipo",nodo.getTipoNodo());
				record.SetAttribute("desc",nodo.Text);					
				xmldoc.DocumentElement.AppendChild(record);

				this.addElement(nodo,xmldoc,1);																			
			}					

			return xmldoc;
		}	
				
		/// <summary>
		/// Ricorsione
		/// </summary>
		/// <param name="nodo"></param>
		/// <param name="xmldoc"></param>
		/// <param name="indentazionePadre"></param>
		private void addElement(myTreeNode nodo, XmlDocument xmldoc, int indentazionePadre)
		{		
			string prefisso = string.Empty;			
			
			if(nodo.Expanded && nodo.Nodes.Count>0)
			{				
				indentazionePadre += 1;

				Microsoft.Web.UI.WebControls.TreeNodeCollection nodi = nodo.Nodes;
				foreach (myTreeNode n in nodi)
				{
					XmlElement record = xmldoc.CreateElement("RECORD");
					record.SetAttribute("tipo",n.getTipoNodo());
					switch(n.getTipoNodo())
					{
						case "U":
							prefisso = "[UO] ";
							break;
						case "R":
							prefisso = "[R] ";
							break;
						case "P":
							prefisso = "[U] ";
							break;
					}
					record.SetAttribute("desc",this.addIndentation(indentazionePadre) + prefisso + n.Text);					
					xmldoc.DocumentElement.AppendChild(record);

					this.addElement(n,xmldoc,indentazionePadre);
				}
			}
		}

		/// <summary>
		/// Imposta sull'XML l'indentazione dei dati
		/// </summary>
		/// <param name="indentazionePadre"></param>
		/// <returns></returns>
		private string addIndentation(int indentazionePadre)
		{
			string indent = string.Empty;

			string fixPlus = "+ ";

			if(indentazionePadre.Equals(2))
				indent = fixPlus;
			else if(indentazionePadre.Equals(3))
				indent = "--" + fixPlus;
			else if(indentazionePadre>3)
			{
				for(int n=1; n<=(indentazionePadre); n++)
				{
					indent += "-";					
				}
				indent += fixPlus;
			}

			return indent;
		}

		/// <summary>
		/// Pulsante di Stampa
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_stampa_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			try
			{
				XmlDocument xmlDoc = this.exportToXML();

				if(xmlDoc!=null && (xmlDoc.InnerXml!=string.Empty || xmlDoc.InnerXml!=""))
				{
					DocsPaWR.FileDocumento filePdf = new DocsPAWA.DocsPaWR.FileDocumento();
					Amministrazione.Manager.OrganigrammaManager manager = new Amministrazione.Manager.OrganigrammaManager();
				
					manager.StampaOrganigramma(xmlDoc);

					filePdf = manager.getFilePDF();
				
					if(filePdf!=null && filePdf.content.Length>0)
					{
						manager.setSessionFilePDF(filePdf);
						this.executeJS("<SCRIPT>stampa();</SCRIPT>");
					}
				}
			}
			catch(Exception ex)
			{
				this.executeJS("<SCRIPT>alert('Errore di sistema: " + ex.Message.Replace("'","\\'") + "');</SCRIPT>");
			}
		}
		#endregion

		#region Utility

		/// <summary>
		/// Imposta il valore del campo nascosto come se provenisse da una ricerca
		/// </summary>
		/// <param name="valore"></param>
		private void ImpostaValoreDiRicercaNascosto(string valore)
		{
			this.hd_lastReturnValueModal.Value = valore;
		}

		/// <summary>
		/// Gestione del refresh della pagina sugli eventi treeview
		/// </summary>
		/// <param name="boolValue"></param>
		private void PermetteRefreshPagina(bool boolValue)
		{
			this.treeViewUO.AutoPostBack = boolValue;
		}

		/// <summary>
		/// Restituisce la selezione sulla modalità di visualizzazione
		/// dei dati nella Treeview
		/// </summary>
		/// <returns></returns>
		private int GetTipoNavigazione()
		{
			return Convert.ToInt16(this.ddl_visualizza.SelectedValue);						
		}

		/// <summary>
		/// Ripulisce la Treeview
		/// </summary>
		private void PulisceTuttaTreeView()
		{
			treeViewUO.Nodes.Clear();
		}

		/// <summary>
		/// Ritorna la UO padre
		/// </summary>
		/// <param name="idCorrGlobRuolo"></param>
		/// <param name="tipo"></param>
		/// <returns></returns>
		private string GetUOPadre(string idCorrGlobRuolo, string tipo)
		{
			string idCorrGlob = string.Empty;

			Amministrazione.Manager.OrganigrammaManager manager = new Amministrazione.Manager.OrganigrammaManager();
			manager.ListaIDParentRicerca(idCorrGlobRuolo, tipo);
			if(manager.getListaIDParentRicerca()!=null && manager.getListaIDParentRicerca().Count>0)	
			{
				ArrayList lista = manager.getListaIDParentRicerca();
				idCorrGlob = lista[1].ToString();				
			}
			return idCorrGlob;
		}
		/// <summary>
		/// Imposta il Focus ad un controllo
		/// </summary>
		/// <param name="ctrl"></param>
		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
		}

		/// <summary>
		/// Esegue JS
		/// </summary>
		/// <param name="key"></param>
		private void executeJS(string key)
		{
			if(!this.Page.IsStartupScriptRegistered("theJS"))			
				this.Page.RegisterStartupScript("theJS", key);
		}

		#endregion		
		
        #region Gestione RF

        /// <summary>
        /// verifica l'abilitazione agli RF e imposta la voce RF nella combo di tipo ricerca
        /// </summary>   
        private void verificaAbilitazioneRF()
        {
            try
            {
                if (this.isEnabledRF())
                {
                    this.getRF();

                    if (this._RFList.Length > 0)
                        this.addRFRicercaTipo();
                }
            }
            catch
            {
                this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema (gestione RF)');</SCRIPT>");
            }
        }

        /// <summary>
        /// imposta la voce RF sulla combobox di tipo ricerca
        /// </summary>
        private void addRFRicercaTipo()
        {
            ListItem item = new ListItem();
            item.Text = "RF";
            item.Value = "RF";
            this.ddl_ricTipo.Items.Add(item);
        }

        /// <summary>
        /// Verifica se RF è abilitato su web.config del WS
        /// </summary>
        /// <returns></returns>
        private bool isEnabledRF()
        {
            AdminTool.Manager.AmministrazioneManager objAmm = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
            return objAmm.IsEnabledRF(this.hd_idAmm.Value);
        }

        /// <summary>
        /// Reperisce la lista di tutti gli RF disponibile e abilitati
        /// </summary>
        private void getRF()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPAWA.DocsPaWR.InfoAmministrazione infoAmm = new DocsPAWA.DocsPaWR.InfoAmministrazione();
            
            infoAmm = ws.AmmGetInfoAmmCorrente(this.hd_idAmm.Value);

            this._RFList = ws.AmmGetRegistri(infoAmm.Codice, "1");
        }

        /// <summary>
        /// Reperisce la lista dei ruoli con l'RF selezionato
        /// </summary>
        private void getRuoliRFSelezionato()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            this._ruoliConRFselezionato = ws.AmmGetListaRuoliAOO(this.ddl_RF.SelectedValue);
        }

        /// <summary>
        /// Imposta la GUI dopo la scelta su RF
        /// </summary>
        private void setComboRF()
        {
            this.td_codRicerca.InnerHtml = "Lista RF:";
                              
            this.txt_ricCod.Visible = false;
            this.ddl_RF.Visible = true;  
                           
            this.td_descRicerca.Visible = false;
            this.txt_ricDesc.Visible = false;

            this.btn_find.Visible = false;
            this.btn_findRF.Visible = true;
            this.btn_findRF.Attributes.Add("onClick", "if(!window.confirm('Questa operazione potrebbe richiedere un tempo di attesa piuttosto lungo.')) {return false}; VisualizzaAttendi();");

            this.btn_back.Enabled = false;
            this.btn_back.AlternateText = "TASTO DISABILITATO IN RICERCA RF";

            this.fillComboBoxRF();

            this.PulisceTuttaTreeView();
        }
        
        /// <summary>
        /// imposta la combobox con gli RF disponibili
        /// </summary>
        private void fillComboBoxRF()
        {
            this.getRF();

            if (this._RFList.Length > 0)
            {
                ListItem item;
                this.ddl_RF.Items.Clear();

                foreach(DocsPAWA.DocsPaWR.OrgRegistro currentRF in this._RFList)
                {
                    item = new ListItem();
                    item.Text = currentRF.Descrizione;
                    item.Value = currentRF.IDRegistro;

                    this.ddl_RF.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// re-imposta la GUI di default per la tipologia di ricerca
        /// </summary>
        private void restoreSearchDefault()
        {
            this.td_codRicerca.InnerHtml = "Codice:";
            this.txt_ricCod.Visible = true;
            this.ddl_RF.Visible = false;

            this.td_descRicerca.Visible = true;
            this.txt_ricDesc.Visible = true;

            this.btn_find.Visible = true;
            this.btn_findRF.Visible = false;

            this._ricercaRFimpostata = false;

            this.btn_back.Enabled = true;
            this.btn_back.AlternateText = "UO padre";
        }

        /// <summary>
        /// tasto cerca ruoli in organigramma con RF selezionato
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_findRF_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                this.performFindRF(false);
            }
            catch(Exception ex)
            {
                this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema (ricerca RF)');</SCRIPT>");
            }
        }

        /// <summary>
        /// Ricerca per RF selezionato
        /// </summary>
        /// <param name="ricercaAncheUtenti">True o False: indica se la ricerca deve visualizzare anche gli utenti dei ruoli</param> 
        private void performFindRF(bool ricercaAncheUtenti)
        {
            this._ricercaRFimpostata = true;

            this.getRuoliRFSelezionato();

            if (this._ruoliConRFselezionato.Length > 0)
            {
                this.PulisceTuttaTreeView();
                this.VisualizzaNodoRoot();

                if (ricercaAncheUtenti)
                {
                    this.ddl_visualizza.SelectedIndex = 2;
                }
                else
                {
                    if (this.ddl_visualizza.SelectedIndex == 0)
                        this.ddl_visualizza.SelectedIndex = 1;
                }
                
                this.PermetteRefreshPagina(false);
                this.EspandeOrgDallaRoot();
                this.pulisceNodiOrfaniRF();
            }
            else
                this.executeJS("<SCRIPT>alert('Nessun ruolo associato a RF: " + this.ddl_RF.Items[this.ddl_RF.SelectedIndex].Text.ToUpper() + "');</SCRIPT>");
        }

        /// <summary>
        /// Imposta l'aggiunta del ruolo nella UO. Regole: 1) sempre, quando non è ricerca per RF; 2) quando si ricerca per un RF ed il ruolo appartiene allo stesso RF 
        /// </summary>
        /// <param name="idRuolo">ID della tabella DPA_CORR_GLOBALI</param>
        /// <returns>True o False</returns>
        private bool visualizzaRuolo_filtroRF(string idRuolo)
        {
            bool retValue = true;
            if (this._ricercaRFimpostata)
                retValue = this.esitoRicercaRuoloConRF(idRuolo);

            return retValue;
        }

        /// <summary>
        /// Filtro su lista dei ruoli con un RF dato
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        private bool esitoRicercaRuoloConRF(string idRuolo)
        {
            bool retValue = false;

            foreach (DocsPAWA.DocsPaWR.OrgRuolo currentRuolo in this._ruoliConRFselezionato)
            {
                if (currentRuolo.IDCorrGlobale.Equals(idRuolo))
                {
                    retValue = true;
                    break;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Elimina i nodi di tipo UO sulla TreeView che non presentano ruoli associati all'RF selezionato
        /// </summary>
        private void pulisceNodiOrfaniRF()
        {
            if (this.treeViewUO.Nodes.Count > 0)
            {
                myTreeNode TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex("0");

                this.ricorsioneCercaFigliOrfaniRF(TreeNodo);
            }
        }

        /// <summary>
        /// Ricorsione del metodo "pulisceNodiOrfaniRF"
        /// </summary>
        /// <param name="nodo"></param>
        private void ricorsioneCercaFigliOrfaniRF(myTreeNode nodo)
        {           
            if (nodo.getTipoNodo().Equals("U"))
            {
                if (nodo.Nodes.Count > 0)
                {
                    try
                    {
                        foreach (myTreeNode currentNode in nodo.Nodes)
                        {                            
                            if (currentNode.Nodes.Count == 0 && currentNode.getTipoNodo().Equals("U"))
                                currentNode.Remove();                                
                            else
                                this.ricorsioneCercaFigliOrfaniRF(currentNode);                            
                        }
                    }
                    catch
                    {
                        this.pulisceNodiOrfaniRF();
                        return;
                    }
                }                                   
            }
        }

        /// <summary>
        /// Verifica se l'utente ha selezionato il filtro di ricerca per RF
        /// </summary>
        /// <returns></returns>
        private bool verificaStatoRicercaRF()
        {
            bool retValue = false;
            if (this.ddl_ricTipo.Items[this.ddl_ricTipo.SelectedIndex].Value.Equals("RF"))
                retValue = true;
            return retValue;
        }
        #endregion

        private bool isOrdinamentoAbilitato()
        {
            return (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ORDINAMENTO_ORGANIGRAMMA"]) && System.Configuration.ConfigurationManager.AppSettings["ORDINAMENTO_ORGANIGRAMMA"].Equals("1"));
        }
    }
}
