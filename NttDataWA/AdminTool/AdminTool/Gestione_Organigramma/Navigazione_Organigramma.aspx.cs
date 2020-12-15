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

namespace Amministrazione.Gestione_Organigramma
{
	/// <summary>
	/// Permette la visualizzazione e la navigazione dell'Organigramma rispetto un'Amministrazione data.
	///
	///
	///	########################################################################################
	///	PARAMETRI IN QUERYSTRING PASSATI ALLA PAGINA WEB:
	///		
	///		"navigazione" [possibili valori: 1,2,3]		... vedi più in basso VISUALIZZAZIONE DATI
	///		"selezione" [possibili valori: 1,2,3]		... vedi più in basso SELEZIONE DATI
	///		"subselezione" [possibili valori: 1,2,3]	... vedi più in basso SELEZIONE DATI
	///		"readonly" [possibili valori: 0,1]			... vedi più in basso MODALITA' READONLY
	///		"idAmm" [indica la system_id della tabella DPA_AMMINISTRA]	
	///		
	///		(tutti i parametri vengono memorizzati in campi di tipo "HIDDEN")
	///	########################################################################################
	///	
	///	
	///	########################################################################################
	///	PARAMETRI DI RITORNO DI QUESTA PAGINA WEB:
	///		
	/// Se sono previste selezioni da effettuare (vedi più in basso SELEZIONE DATI) ed il parametro "readonly"
	/// è impostato a "0", questa pagina web modale ha un valore di ritorno di questo tipo:		
	/// 
	///		retValue = codice | descrizione | idCorrGlobale | idGroup o idPeople
	///	
	///		
	///	Esempi:
	///	
	///	Selezione di una UO:
	///	
	///			retValue = "ET|Etnoteam Spa|1123|<NULL> o <BLANK>"
	///		
	///	Selezione di un Ruolo:
	///	
	///			retValue = "PRGDOCSPA|Programmatore DocsPA|1789|1791
	///			
	///	Selezione di un Utente:
	///	
	///			retValue = "GADAMO|Adamo Gian Luca|1969|1971"
	///		
	///	########################################################################################
	///		
	/// 
	/// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	/// VISUALIZZAZIONE DATI:
	/// 
	/// I dati possono essere visualizzati tramite 3 tipologie:
	///		- navigazione 1 = visualizza solo le UO
	///		- navigazione 2 = visualizza le UO e i ruoli
	///		- navigazione 3 = visualizza le UO, i ruoli e gli utenti
	///	La navigazione viene specificata tramite il parametro di QueryString "navigazione".
	///	++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	///	
	///	
	/// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	/// SELEZIONE DATI:
	/// 
	/// I dati visualizzati potranno essere selezionati (scelti) rispetto al parametro 
	/// di querystring "selezione":
	///		- selezione 1 = si potranno selezionare solo le UO
	///		- selezione 2 = si potranno selezionare le UO o i ruoli
	///		- selezione 3 = si potranno selezionare sia le UO, sia i ruoli, sia gli utenti
	///	NOTA: Se la "selezione" non viene inizializzata, vale il parametro "subselezione".
	///	
	///	Tuttavia, nonostante il parametro "selezione", si potrà specificare un altro parametro, denominato
	///	"subselezione", per poter selezionare solo le UO o solo i ruoli o soli gli utenti:
	///		- subselezione 1 = si potranno selezionare solo le UO
	///		- subselezione 2 = si potranno selezionare solo i ruoli
	///		- subselezione 3 = si potranno selezionare solo gli utenti
	///	NOTA: Se la "subselezione" non viene inizializzata, vale il parametro "selezione".
	///	++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	///	
	///
	/// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	/// MODALITA' READONLY:
	/// 
	/// Se viene impostata la navigazione in modalità READONLY (valore "1"), si potranno solo espandere 
	/// i nodi dell'organigramma senza poterli selezionare: in questa modalità la pagina web non avrà 
	/// parametro di ritorno.
	/// Impostando il valore uguale a "0" si avrà un paramtro di ritorno come descritto sopra.
	///	++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	///	
	///	
	///	
	///	°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°
	///	Suggerimenti:
	///	
	///	Tip 1: Inutile dire che un corretto uso dei parametri comporta che "navigazione" sia strettamente
	///	legato ai parametri "selezione" o "subselezione"! Assurdo infatti impostare un parametro di 
	///	visualizzazione uguale a 2 (visualizza UO e Ruoli) se poi la sub-selezione è solo su utenti!
	///	
	///	Tip 2: Se i parametri "selezione" e "subselezione" non vengono inizializzati, la pagina web sarà 
	///	utile per la sola visualizzazione e navigazione dell'organigramma.
	///	
	///	Tip 3: Se la "selezione" non viene inizializzata ma la "subselezione" sì, allora vale la "subselezione".
	///	Idem il contrario.
	///	
	/// Tip 4: Se, per esempio, la "selezione" è uguale a 3 ma la "subselezione" è uguale a 2, la condizione
	/// ricade sulla "subselezione" e potranno quindi essere selezionati solo i ruoli.
	/// 
	/// Tip 5: Se, per esempio, la "subselezione" è uguale a 3 ma la "selezione" è uguale a 2, la condizione
	/// ricade sulla "subselezione" e potranno quindi essere selezionati solo gli utenti.
	///	
	///	°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°°
	///	
	/// </summary>
	public class Navigazione_Organigramma : System.Web.UI.Page
	{
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
			this.treeViewUO.Expand += new Microsoft.Web.UI.WebControls.ClickEventHandler(this.treeViewUO_Expand);
			this.treeViewUO.SelectedIndexChange += new Microsoft.Web.UI.WebControls.SelectEventHandler(this.treeViewUO_SelectedIndexChange);
			this.treeViewUO.Collapse += new Microsoft.Web.UI.WebControls.ClickEventHandler(this.treeViewUO_Collapse);
			this.ddl_ricTipo.SelectedIndexChanged += new System.EventHandler(this.ddl_ricTipo_SelectedIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region class myTreeNode
		/// <summary>
		/// Summary description for myTreeNode
		/// </summary>
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

			// IDCorrGlobale
			public string getIDCorrGlobale() 
			{ 				
				return ViewState["IDCorrGlobale"].ToString();
			}
			public void setIDCorrGlobale(string id) 
			{ 
				ViewState["IDCorrGlobale"] = id;
			}

			// Codice
			public string getCodice() 
			{ 				
				return ViewState["Codice"].ToString();
			}
			public void setCodice(string id) 
			{ 
				ViewState["Codice"] = id;
			}

			// CodiceRubrica
			public string getCodiceRubrica() 
			{ 				
				return ViewState["CodiceRubrica"].ToString();
			}
			public void setCodiceRubrica(string id) 
			{ 
				ViewState["CodiceRubrica"] = id;
			}

			// Descrizione
			public string getDescrizione() 
			{ 				
				return ViewState["Descrizione"].ToString();
			}
			public void setDescrizione(string id) 
			{ 
				ViewState["Descrizione"] = id;
			}		
	
			// Livello
			public string getLivello() 
			{ 				
				return ViewState["Livello"].ToString();
			}
			public void setLivello(string id) 
			{ 
				ViewState["Livello"] = id;
			}	

			// Amministrazione
			public string getIDAmministrazione() 
			{ 				
				return ViewState["IDAmministrazione"].ToString();
			}
			public void setIDAmministrazione(string id) 
			{ 
				ViewState["IDAmministrazione"] = id;
			}				

			// Tipo ruolo
			public string getIDTipoRuolo() 
			{ 				
				return ViewState["IDTipoRuolo"].ToString();
			}
			public void setIDTipoRuolo(string id) 
			{ 
				ViewState["IDTipoRuolo"] = id;
			}	

			// ID Group
			public string getIDGruppo() 
			{ 				
				return ViewState["IDGruppo"].ToString();
			}
			public void setIDGruppo(string id) 
			{ 
				ViewState["IDGruppo"] = id;
			}				

			// ID People
			public string getIDPeople() 
			{ 				
				return ViewState["IDPeople"].ToString();
			}
			public void setIDPeople(string id) 
			{ 
				ViewState["IDPeople"] = id;
			}	
		}

		#endregion

		#region WebControls e variabili
		protected System.Web.UI.WebControls.DropDownList ddl_ricTipo;
		protected System.Web.UI.WebControls.TextBox txt_ricCod;
		protected System.Web.UI.WebControls.TextBox txt_ricDesc;
		protected System.Web.UI.WebControls.Button btn_find;
		protected Microsoft.Web.UI.WebControls.TreeView treeViewUO;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idAmm;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_tipologia;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_selezione;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_subselezione;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_readonly;
		protected System.Web.UI.HtmlControls.HtmlTableCell td_descRicerca;		
		#endregion
	
		#region Inizializzazione dati
		/// <summary>
		/// Page Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = -1;
			Response.Buffer = false;

			try
			{
				if(!IsPostBack)
				{
					string myPath = SAAdminTool.Utils.getHttpFullPath(this);					
					this.treeViewUO.SystemImagesPath = myPath + "/AdminTool/Images/treeimages/";

					//reperisce i dati dal chiamante
					this.GetDataQueryString();

					this.Inizialize();		
					
					this.SetDDLFiltriRicerca();
				}
				else
				{				
					// gestione del valore di ritorno della modal Dialog (ricerca)
					if(this.hd_returnValueModal.Value != null && this.hd_returnValueModal.Value != string.Empty && this.hd_returnValueModal.Value != "undefined")
						this.RicercaNodo(this.hd_returnValueModal.Value);					

//					if(Request.QueryString["indice"] != null)
//						this.treeViewUO.SelectedNodeIndex = Request.QueryString["indice"].ToString();							
				}
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}

		/// <summary>
		/// Reperimento dati passati in querystring
		/// </summary>
		private void GetDataQueryString()
		{
			this.hd_idAmm.Value = Request.QueryString["idAmm"].ToString();
			this.hd_tipologia.Value = Request.QueryString["navigazione"].ToString();
			this.hd_selezione.Value = Request.QueryString["selezione"].ToString();
			this.hd_subselezione.Value = Request.QueryString["subselezione"].ToString();
			this.hd_readonly.Value = Request.QueryString["readonly"].ToString();			
		}

		/// <summary>
		/// Inizializzazione
		/// </summary>
		private void Inizialize()
		{
			try
			{			
				Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
				theManager.ListaUOLivelloZero(this.hd_idAmm.Value);

				if(theManager.getListaUO() != null && theManager.getListaUO().Count>0)
				{
					this.btn_find.Attributes.Add("onclick","ApriRisultRic('"+this.hd_idAmm.Value+"');");
					this.LoadTreeviewLivelloZero(theManager.getListaUO());	
					this.SetFocus(this.txt_ricDesc);
				}
				else
				{
					this.executeJS("<SCRIPT>alert('Attenzione, nessun organigramma presente'); window.close();</SCRIPT>");
				}							
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}

		/// <summary>
		/// Imposta la DDL dei filtri di ricerca
		/// </summary>
		private void SetDDLFiltriRicerca()
		{
			/*
			<asp:ListItem Value="U" Selected="True">Unità Organizz.</asp:ListItem>
			<asp:ListItem Value="R">Ruolo</asp:ListItem>
			<asp:ListItem Value="PN">Nome</asp:ListItem>
			<asp:ListItem Value="PC">Cognome</asp:ListItem> 
			*/

			ListItem itemU = new ListItem();
			itemU.Value = "U";
			itemU.Text = "Unità Organizz.";
			itemU.Selected = true;

			ListItem itemR = new ListItem();
			itemR.Value = "R";
			itemR.Text = "Ruolo";			

			ListItem itemPN = new ListItem();
			itemPN.Value = "PN";
			itemPN.Text = "Nome";

			ListItem itemPC = new ListItem();
			itemPC.Value = "PC";
			itemPC.Text = "Cognome";

			this.ddl_ricTipo.Items.Add(itemU); //sempre almeno le UO!!!

			switch(this.GetTipoNavigazione())
			{				
				case 2:
					this.ddl_ricTipo.Items.Add(itemR);
					break;
				case 3:
					this.ddl_ricTipo.Items.Add(itemR);
					this.ddl_ricTipo.Items.Add(itemPN);
					this.ddl_ricTipo.Items.Add(itemPC);
					break;
			}
		}

		/// <summary>
		/// Imposta il Focus()
		/// </summary>
		/// <param name="ctrl"></param>
		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
		}
		#endregion

		#region Gestione della Treeview
		/// <summary>
		/// Caricamento dati UO livello ZERO
		/// </summary>
		/// <param name="listaUO"></param>
		private void LoadTreeviewLivelloZero(ArrayList listaUO)
		{
			try
			{								
				string myPath = SAAdminTool.Utils.getHttpFullPath(this);

				this.PulisceTuttaTreeView();

                Microsoft.Web.UI.WebControls.TreeNode treenode = new Microsoft.Web.UI.WebControls.TreeNode();				

				treenode.Text = "Organigramma";

				treeViewUO.Nodes.Add(treenode);

                Microsoft.Web.UI.WebControls.TreeNode tNode = new Microsoft.Web.UI.WebControls.TreeNode();
				tNode = treeViewUO.Nodes[0];

				myTreeNode nodoT;
				myTreeNode nodoFiglio;	

				foreach(SAAdminTool.DocsPaWR.OrgUO uo in listaUO)
				{
					nodoT = new myTreeNode();
			
					nodoT.ID = uo.IDCorrGlobale;
					nodoT.Text = uo.CodiceRubrica + " - " + uo.Descrizione;
					nodoT.ImageUrl = myPath + "/AdminTool/Images/uo.gif";
			
					tNode.Nodes.Add(nodoT);				

					nodoT.setTipoNodo("U");
					nodoT.setIDCorrGlobale(uo.IDCorrGlobale);
					nodoT.setCodice(uo.Codice);
					nodoT.setCodiceRubrica(uo.CodiceRubrica);
					nodoT.setDescrizione(uo.Descrizione);
					nodoT.setLivello(uo.Livello);
					nodoT.setIDAmministrazione (uo.IDAmministrazione);

					if((!uo.Ruoli.Equals("0")) || (!uo.SottoUo.Equals("0")))
					{
						nodoFiglio = new myTreeNode();						
						nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
						nodoT.Nodes.Add(nodoFiglio);
					}
					else
					{
						nodoT.Text = uo.CodiceRubrica + " - " + uo.Descrizione;
					}
				}

				tNode.Expanded = true;	
				//this.SelezionaPrimo();
				this.LoadTreeViewLivelloFigli("0.0","U");
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}

		/// <summary>
		/// Caricamento dati UO livelli figli
		/// </summary>
		/// <param name="indice"></param>
		/// <param name="tipoNodo"></param>
		private void LoadTreeViewLivelloFigli(string indice, string tipoNodo)
		{	
			try
			{
				string myPath = SAAdminTool.Utils.getHttpFullPath(this);		

				if(this.GetReadOnly())
					treeViewUO.SelectedNodeIndex = indice;

				ArrayList lista = new ArrayList();
			
				myTreeNode TreeNodo;
				TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(indice);
				TreeNodo.Expanded = true;						
			
				if(TreeNodo.Nodes.Count > 0)
					TreeNodo.Nodes.RemoveAt(0);

				myTreeNode nodoRuoli;
				myTreeNode nodoUtenti;
				myTreeNode nodoUO;
				myTreeNode nodoFiglio;

				Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
				
				theManager.ListaRuoliUO(TreeNodo.getIDCorrGlobale());

				if(this.GetTipoNavigazione()>1)
				{					
					lista = theManager.getListaRuoliUO();

					// ... ruoli
					if(lista != null && lista.Count > 0)
					{
						foreach(SAAdminTool.DocsPaWR.OrgRuolo ruolo in lista)
						{
							nodoRuoli = new myTreeNode();
	
							nodoRuoli.ID = ruolo.IDCorrGlobale;
							nodoRuoli.Text = ruolo.CodiceRubrica + " - " + ruolo.Descrizione;
							nodoRuoli.ImageUrl = myPath + "/AdminTool/Images/ruolo.gif";
	
							TreeNodo.Nodes.Add(nodoRuoli);								

							nodoRuoli.setTipoNodo("R");
							nodoRuoli.setIDCorrGlobale(ruolo.IDCorrGlobale);
							nodoRuoli.setIDTipoRuolo(ruolo.IDTipoRuolo);
							nodoRuoli.setIDGruppo(ruolo.IDGruppo);
							nodoRuoli.setCodice(ruolo.Codice);
							nodoRuoli.setCodiceRubrica(ruolo.CodiceRubrica);
							nodoRuoli.setDescrizione(ruolo.Descrizione);
							nodoRuoli.setIDAmministrazione(ruolo.IDAmministrazione);
						
							// ... utenti
							if(ruolo.Utenti.Length > 0)
							{
								if(this.GetTipoNavigazione()>2)
								{
									foreach(SAAdminTool.DocsPaWR.OrgUtente utente in ruolo.Utenti)
									{
										nodoUtenti = new myTreeNode();

										nodoUtenti.ID = utente.IDCorrGlobale;
										nodoUtenti.Text = utente.CodiceRubrica + " - " + utente.Cognome + " " + utente.Nome;
										nodoUtenti.ImageUrl = myPath + "/AdminTool/Images/utente.gif";

										nodoRuoli.Nodes.Add(nodoUtenti);

										nodoUtenti.setTipoNodo("P");
										nodoUtenti.setIDCorrGlobale(utente.IDCorrGlobale);
										nodoUtenti.setIDPeople(utente.IDPeople);
										nodoUtenti.setCodice(utente.Codice);
										nodoUtenti.setCodiceRubrica(utente.CodiceRubrica);
										nodoUtenti.setIDAmministrazione(utente.IDAmministrazione);							
									}
								}
							} // fine inserimento utenti	
							else
							{
								nodoRuoli.Text = ruolo.CodiceRubrica + " - " + ruolo.Descrizione;
							}
						} // fine inserimento ruoli 						
					}
				}


				// ... uo sottostanti				
				int livello = Convert.ToInt32(TreeNodo.getLivello()) + 1;
				
				theManager.ListaUO(TreeNodo.getIDCorrGlobale(),livello.ToString(),TreeNodo.getIDAmministrazione());
				lista = theManager.getListaUO();

				if(lista != null && lista.Count > 0)
				{
					foreach(SAAdminTool.DocsPaWR.OrgUO sub_uo in lista)
					{
						nodoUO = new myTreeNode();
	
						nodoUO.ID = sub_uo.IDCorrGlobale;
						nodoUO.Text = sub_uo.CodiceRubrica + " - " + sub_uo.Descrizione;
						nodoUO.ImageUrl = myPath + "/AdminTool/Images/uo.gif";
	
						TreeNodo.Nodes.Add(nodoUO);

						nodoUO.setTipoNodo("U");
						nodoUO.setIDCorrGlobale(sub_uo.IDCorrGlobale);
						nodoUO.setCodice(sub_uo.Codice);
						nodoUO.setCodiceRubrica(sub_uo.CodiceRubrica);
						nodoUO.setDescrizione(sub_uo.Descrizione);
						nodoUO.setLivello(sub_uo.Livello);
						nodoUO.setIDAmministrazione(sub_uo.IDAmministrazione);						

						if((!sub_uo.Ruoli.Equals("0")) || (!sub_uo.SottoUo.Equals("0")))
						{
							nodoFiglio = new myTreeNode();						
							nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
							nodoUO.Nodes.Add(nodoFiglio);
						}
						else
						{
							nodoUO.Text = sub_uo.CodiceRubrica + " - " + sub_uo.Descrizione;
						}
					} // fine inserimento uo sottostanti
				}	
			}			
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}			
		}		

		/// <summary>
		/// treeViewUO_Collapse
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeViewUO_Collapse(object sender, Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e)
		{
			try
			{									
				if(e.Node!="0")
				{
					myTreeNode TreeNodo;
					TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(e.Node);

                    Microsoft.Web.UI.WebControls.TreeNode nodoFiglio;

					if(TreeNodo.getTipoNodo().Equals("U"))
					{
						if(TreeNodo.Nodes.Count > 0)
							TreeNodo.Nodes.Clear();

                        nodoFiglio = new Microsoft.Web.UI.WebControls.TreeNode();
						nodoFiglio.Text="<font color='#ff0000'>&nbsp;loading...</font>";
						TreeNodo.Nodes.Add(nodoFiglio);
					}			
					
					if(this.GetReadOnly())
						treeViewUO.SelectedNodeIndex = e.Node;
				}							
			}
			catch
			{				
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}

		/// <summary>
		/// treeViewUO_Expand
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeViewUO_Expand(object sender, Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e)
		{
			try
			{								
				if(e.Node != "0")
				{				
					myTreeNode TreeNodo;
					TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(e.Node);					
	
					if(TreeNodo.getTipoNodo().Equals("U"))
					{						
						if(TreeNodo.Nodes.Count > 0)
							TreeNodo.Nodes.Clear();

						this.LoadTreeViewLivelloFigli(e.Node, TreeNodo.getTipoNodo());					
					}								
					
					if(this.GetReadOnly())
						treeViewUO.SelectedNodeIndex = e.Node;
				}
				else
				{					
					this.Inizialize();					
				}				
			}
			catch
			{				
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}

		/// <summary>
		/// treeViewUO_SelectedIndexChange
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeViewUO_SelectedIndexChange(object sender, Microsoft.Web.UI.WebControls.TreeViewSelectEventArgs e)
		{
			try
			{
				if(!e.NewNode.Equals("0"))
				{					
					myTreeNode TreeNodo;
					TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(e.NewNode);
					
					this.TornaAllaWndChiamante(TreeNodo);						
				}	
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}

		/// <summary>
		/// Pulisce TreeView
		/// </summary>
		private void PulisceTuttaTreeView()
		{
			treeViewUO.Nodes.Clear();
		}

		/// <summary>
		/// Seleziona il primo nodo
		/// </summary>
		private void SelezionaPrimo()
		{
			treeViewUO.SelectedNodeIndex = "0.0";							
		}
		#endregion		

		#region Ricerca e ritorno al chiamante
		/// <summary>
		/// Combo-box di ricerca
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ddl_ricTipo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch (this.ddl_ricTipo.SelectedValue)
			{
				case "U":
					this.td_descRicerca.InnerHtml = "Nome UO:";
					break;
				case "R":
					this.td_descRicerca.InnerHtml = "Nome ruolo:";
					break;
				case "PN":
					this.td_descRicerca.InnerHtml = "Nome utente:";
					break;
				case "PC":
					this.td_descRicerca.InnerHtml = "Cognome utente:";
					break;
			}

			this.SetFocus(this.txt_ricDesc);
		}

		/// <summary>
		/// Salto alla pagina web chiamante dopo aver selezionato una ricerca
		/// </summary>
		/// <param name="TreeNodo"></param>
		private void TornaAllaWndChiamante(myTreeNode TreeNodo)
		{
			bool gotoParent = false;
			string idGeneric = string.Empty; // potrebbe essere idGroup (x i ruoli) o idPeople (x gli utenti) o uguale a idCorrGlob (x le UO)

			try
			{
				switch(TreeNodo.getTipoNodo())
				{
					case "U":
						if(this.SelezionePossibile("U"))
						{
							gotoParent = true;
							idGeneric = TreeNodo.getIDCorrGlobale();
						}
						break;

					case "R":
						if(this.SelezionePossibile("R"))
						{
							gotoParent = true;
							idGeneric = TreeNodo.getIDGruppo();
						}
						break;

					case "P":
						if(this.SelezionePossibile("P"))
						{
							gotoParent = true;
							idGeneric = TreeNodo.getIDPeople();
						}
						break;
				}

				if(gotoParent)
				{						
					string codice = TreeNodo.getCodice();
					string descrizione = TreeNodo.getDescrizione();					
					string idCorrGlob = TreeNodo.getIDCorrGlobale();					

					string retValue = codice+"|"+descrizione+"|"+idCorrGlob+"|"+idGeneric;
					this.executeJS("<SCRIPT>window.returnValue='" + retValue + "'; window.close()</SCRIPT>");				
				}
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}

		/// <summary>
		/// Verifica se è possibile selezionare il nodo
		/// </summary>
		/// <returns></returns>
		private bool SelezionePossibile(string tipoOggetto)
		{
			bool seleziona = false;
			
			switch(tipoOggetto)
			{
				case "U":
					if(this.GetSelezione()>0 || this.GetSubSelezione().Equals(1))
						seleziona = true;
					break;

				case "R":
					if(this.GetSelezione()>1 || this.GetSubSelezione().Equals(2))
						seleziona = true;
					break;

				case "P":
					if(this.GetSelezione()>2 || this.GetSubSelezione().Equals(3))
						seleziona = true;
					break;
			}			

			return seleziona;
		}

		#endregion

		#region Imposta Treeview dopo la ricerca
		/// <summary>
		/// Ricerca il nodo in Treeview
		/// </summary>
		/// <param name="returnValue"></param>
		private void RicercaNodo(string returnValue)
		{		
			bool finded = false;
			myTreeNode TreeNodoFinded = new myTreeNode();

			try
			{
				
				this.hd_returnValueModal.Value="";

				string[] appo = returnValue.Split('_');

				string idCorrGlobale = appo[0];
				string idParent = appo[1];
			
				string tipo = this.ddl_ricTipo.SelectedValue;
				
				Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
				theManager.ListaUOLivelloZero(this.hd_idAmm.Value);				

				this.LoadTreeviewLivelloZero(theManager.getListaUO());	

				this.SelezionaPrimo();

				theManager.ListaIDParentRicerca(idParent,tipo);

				if(theManager.getListaIDParentRicerca() != null && theManager.getListaIDParentRicerca().Count > 0)
				{
					ArrayList lista = new ArrayList();
					theManager.getListaIDParentRicerca().Reverse();
					lista = theManager.getListaIDParentRicerca();

					lista.Add(Convert.ToInt32(idCorrGlobale));				
				
					for(int n=1; n<=lista.Count-1; n++)
					{
						myTreeNode TreeNodo;
						TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
			
						foreach(myTreeNode nodo in TreeNodo.Nodes)
						{
							if(nodo.ID.Equals(lista[n].ToString()) && nodo.ID != idCorrGlobale)
							{
								if(nodo.getTipoNodo().Equals("U"))
								{
									this.LoadTreeViewLivelloFigli(nodo.GetNodeIndex(), nodo.getTipoNodo());																			
								}
								else
								{
									nodo.Expanded = true;
								}
								treeViewUO.SelectedNodeIndex = nodo.GetNodeIndex();								
								break;
							}
							if(nodo.ID.Equals(lista[n].ToString()) && nodo.ID.Equals(idCorrGlobale))
							{								
								treeViewUO.SelectedNodeIndex = nodo.GetNodeIndex();	
								finded = true;
								TreeNodoFinded = nodo;
								break;
							}
						}
					}
					
					if(finded && !this.GetReadOnly())
						this.TornaAllaWndChiamante(TreeNodoFinded);
				}
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
			}
		}
		#endregion

		#region Utility
		/// <summary>
		/// Generic JS
		/// </summary>
		/// <param name="key"></param>
		private void executeJS(string key)
		{
			if(!this.Page.IsStartupScriptRegistered("theJS"))			
				this.Page.RegisterStartupScript("theJS", key);
		}

		/// <summary>
		/// Reperimento tipologia di navigazione
		/// </summary>
		/// <returns></returns>
		private int GetTipoNavigazione()
		{
			int retValue = 1; // almeno la navigazione delle UO!

			if(this.hd_tipologia.Value!="")
				retValue = Convert.ToInt16(this.hd_tipologia.Value);
			
			return retValue;
		}

		/// <summary>
		/// Reperimento selezione
		/// </summary>
		/// <returns></returns>
		private int GetSelezione()
		{
			int retValue = 0;

			if(this.hd_selezione.Value!="")
				retValue = Convert.ToInt16(this.hd_selezione.Value);

			return retValue;
		}

		/// <summary>
		/// Reperimento sub-selezione
		/// </summary>
		/// <returns></returns>
		private int GetSubSelezione()
		{			
			int retValue = 0;

			if(this.hd_subselezione.Value!="")
				retValue = Convert.ToInt16(this.hd_subselezione.Value);

			return retValue;
		}

		/// <summary>
		/// Reperimento readonly
		/// </summary>
		/// <returns></returns>
		private bool GetReadOnly()
		{
			bool retValue = false;

			if(this.hd_readonly.Value!="")
			{
				switch(this.hd_readonly.Value)
				{
					case "0":
						retValue = false;
						break;
					case "1":
						retValue = true;
						break;
				}				
			}
			return retValue;
		}
		#endregion
	}
}
