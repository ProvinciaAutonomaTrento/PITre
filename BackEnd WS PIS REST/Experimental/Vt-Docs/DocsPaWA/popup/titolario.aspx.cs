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
using System.Xml;
using System.Web.Services.Description;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for titolario.
	/// </summary>
    public class frm_titolario : DocsPAWA.CssPage
	{
		#region class myTreeNode
		/// <summary>
		/// Summary description for myTreeNode
		/// </summary>
		public class myTreeNode : Microsoft.Web.UI.WebControls.TreeNode
		{
			public string getIDRECORD() 
			{ 				
				return ViewState["idrecord"].ToString();
			}
			public void setIDRECORD(string id) 
			{ 
				ViewState["idrecord"] = id;
			}

			public string getCODICE() 
			{ 
				return ViewState["codice"].ToString();
			}
			public void setCODICE(string cod) 
			{ 
				ViewState["codice"] = cod;
			}

			public string getDESCRIZIONE() 
			{ 
				return ViewState["descrizione"].ToString();
			}
			public void setDESCRIZIONE(string desc) 
			{ 
				ViewState["descrizione"] = desc;
			}

            public string getNUMMESICONSERVAZIONE() 
			{ 
				return ViewState["nummesiconservazione"].ToString();
			}
			public void setNUMMESICONSERVAZIONE(string desc) 
			{
                ViewState["nummesiconservazione"] = desc;
			}
           

			public string getREGISTRO() 
			{ 
				if(ViewState["registro"] != null)
				{
					return ViewState["registro"].ToString();
				}
				else
				{
					return null;
				}
			}
			public void setREGISTRO(string reg) 
			{ 
				ViewState["registro"] = reg;
			}

			public string getLIVELLO() 
			{ 
				return ViewState["livello"].ToString();
			}
			public void setLIVELLO(string liv) 
			{ 
				ViewState["livello"] = liv;
			}
			public string getPARENT() 
			{ 
				return ViewState["idparent"].ToString();
			}
			public void setPARENT(string idparent) 
			{ 
				ViewState["idparent"] = idparent;
			}
			public string getCODLIV() 
			{ 
				return ViewState["codliv"].ToString();
			}
			public void setCODLIV(string codliv) 
			{ 
				ViewState["codliv"] = codliv;
			}
		}
		#endregion
	
		#region variabili globali
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.Label lbl_msg;
		protected System.Web.UI.WebControls.TextBox txt_find_cod;
		protected System.Web.UI.WebControls.TextBox txt_find_desc;
		protected System.Web.UI.WebControls.ImageButton btn_find;
		protected System.Web.UI.WebControls.Label lbl_td;
		protected System.Web.UI.WebControls.Panel pnl_ric;
		protected System.Web.UI.WebControls.ImageButton btn_chiudi_risultato;
		protected Microsoft.Web.UI.WebControls.TreeView TreeView1;
        protected string idTitolario = string.Empty;
        protected System.Web.UI.WebControls.TextBox txt_indice;
        protected System.Web.UI.WebControls.TextBox txt_note;
        protected System.Web.UI.WebControls.Label lbl_indice;
        protected System.Web.UI.WebControls.ImageButton img_exportIndice;
        private DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        private DocsPAWA.DocsPaWR.Registro[] userRegistri;
        protected System.Web.UI.WebControls.Label lbl_registri;
        
        #endregion

		#region Page Load
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
            //modifica per la funzione di riclassificazione in inps
            if (wws.isFiltroAooEnabled())
            {
                DocsPaWR.Registro[] reg = UserManager.getListaRegistri(this.Page);
                DocsPaWR.Registro regCorrente = new DocsPaWR.Registro();
                if (UserManager.getRegistroSelezionato(this.Page) != null)
                {
                    string IdRegistroSelezionato = UserManager.getRegistroSelezionato(this.Page).systemId;
                    for (int i = 0; i < reg.Length; i++)
                    {
                        if (reg[i].systemId.Equals(IdRegistroSelezionato))
                            regCorrente = reg[i];

                    }
                    //if (reg[0].systemId != UserManager.getRegistroSelezionato(this.Page).systemId)
                    if (reg[0].systemId != IdRegistroSelezionato)
                        UserManager.setRegistroSelezionato(this.Page, regCorrente);
                    //UserManager.setRegistroSelezionato(this.Page, reg[0]);
                }
            }

            //Abilitazione indice sistematico
            if (wws.isEnableIndiceSistematico())
            {
                lbl_indice.Visible = true;
                txt_indice.Visible = true;
                img_exportIndice.Visible = true;
            }

            //Se non è selezionato nessun titolario
            idTitolario = Request.QueryString["idTit"];
            if (idTitolario != null && idTitolario != string.Empty)
            {
                try
                {
                    if (!IsPostBack)
                    {
                        DocsPaWR.Registro rg = UserManager.getRegistroSelezionato(this.Page);
                        if (rg == null)
                        {
                            this.CaricaComboRegistri(ddl_registri);
                        }
                        else
                        {
                            this.ddl_registri.Visible = false;
                            this.lbl_registri.Visible = false;
                        }

                        this.caricaTreeView();
                    }
                }
                catch
                {
                    lbl_msg.Text = "Attenzione! Si è verificato un errore di caricamento della pagina.";
                }
            }
            else
            {
                lbl_msg.Text = "Attenzione!Titolario non selezionato correttamente";
            }
            string abilita = utils.InitConfigurationKeys.GetValue("0","FE_DBLCLICK_INVIO_TITOLARIO");
            if (abilita.Equals("1"))
            {
                this.TreeView1.Attributes.Add("onkeyup", "invioEvent(window.event.keyCode);");
                this.TreeView1.Attributes.Add("ondblclick", "TreeViewDoubleClick();");   
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
			this.btn_find.Click += new System.Web.UI.ImageClickEventHandler(this.btn_find_Click);
			this.TreeView1.Expand += new Microsoft.Web.UI.WebControls.ClickEventHandler(this.TreeView1_Expand_1);
			this.TreeView1.SelectedIndexChange += new Microsoft.Web.UI.WebControls.SelectEventHandler(this.TreeView1_SelectedIndexChange);
			this.TreeView1.Collapse += new Microsoft.Web.UI.WebControls.ClickEventHandler(this.TreeView1_Collapse_1);
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.btn_chiudi_risultato.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudi_risultato_Click);
			this.ID = "frm_titolario";
			this.Load += new System.EventHandler(this.Page_Load);
            this.ddl_registri.SelectedIndexChanged += new EventHandler(ddl_registri_SelectedIndexChanged);
		}
		#endregion		

		#region Treeview

		/// <summary>
		/// espande nodo
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TreeView1_Expand_1(object sender, Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e)
		{
			try
			{
				if(e.Node != "0")
				{
					LoadTreeViewChild(e.Node);
				}
			}
			catch
			{				
				lbl_msg.Text = "Attenzione! si è verificato un errore di caricamento della pagina.";
			}
		}

		/// <summary>
		/// chiude nodo
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TreeView1_Collapse_1(object sender, Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e)
		{
			try
			{
				Microsoft.Web.UI.WebControls.TreeNode nodoFiglio;
				Microsoft.Web.UI.WebControls.TreeNode TreeNodo;
				TreeNodo = TreeView1.GetNodeFromIndex(e.Node);
			
				if(e.Node!="0")
				{
					if(TreeNodo.Nodes.Count > 0)
						TreeNodo.Nodes.Clear();	
					nodoFiglio = new Microsoft.Web.UI.WebControls.TreeNode();
					nodoFiglio.Text="<font color='#ff0000'>&nbsp;loading...</font>";
					TreeNodo.Nodes.Add(nodoFiglio);
				}
			}
			catch
			{				
				lbl_msg.Text = "Attenzione! si è verificato un errore di caricamento della pagina.";
			}
		}

		/// <summary>
		/// seleziona nodo
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TreeView1_SelectedIndexChange(object sender, Microsoft.Web.UI.WebControls.TreeViewSelectEventArgs e)
		{
            Microsoft.Web.UI.WebControls.TreeNode selectedNode = TreeView1.GetNodeFromIndex(e.NewNode);
            selectedNode.NavigateUrl = "javascript:void(0);";
		}

		/// <summary>
		/// carica treeview
		/// </summary>
		private void LoadTreeView()
		{				
			try
			{
                string codAmm;
				TreeView1.Nodes.Clear();

                Microsoft.Web.UI.WebControls.TreeNode treenode = new Microsoft.Web.UI.WebControls.TreeNode();
                //if (Request.QueryString["isFasc"] == "gestArchivio")
                //{
                //    codAmm = Request.QueryString["codAmm"];
                //    treenode.Text = codAmm;
                //}
                //else
                //{
                treenode.Text = UserManager.getRegistroSelezionato(this.Page).codAmministrazione;                
                    
                //} 
                                    
                TreeView1.Nodes.Add(treenode);
                treenode.Expanded = true;
                Microsoft.Web.UI.WebControls.TreeNode tNode = new Microsoft.Web.UI.WebControls.TreeNode();
                tNode = TreeView1.Nodes[0];

                myTreeNode nodoT;
                myTreeNode nodoFiglio;
			
                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
				string xmlStream;
                DocsPaWR.Registro rg	= UserManager.getRegistroSelezionato(this.Page);
				DocsPaWR.Ruolo ru		= UserManager.getRuolo(this.Page);
                string idAmm			= ru.idAmministrazione;
				string idGruppo			= ru.idGruppo;
                string idRegistro       = rg.systemId;
               

                xmlStream = ws.NodoTitolarioSecurity(idAmm, "0", idGruppo, idRegistro, idTitolario);
                
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xmlStream);
			
				XmlNode lista = doc.SelectSingleNode("NewDataSet");
				if(lista.ChildNodes.Count > 0)
				{					
					foreach (XmlNode nodo in lista.ChildNodes)
					{
                        nodoT = new myTreeNode();

                        nodoT.ID = nodo.SelectSingleNode("CODICE").InnerText;

                        switch (nodo.SelectSingleNode("STATO").InnerText)
                        {
                            case "A":
                                nodoT.Text = nodo.SelectSingleNode("DESCRIZIONE").InnerText + " attivo";
                                break;

                            case "C":
                                DateTime dataAttivazione = Convert.ToDateTime(nodo.SelectSingleNode("DATA_ATTIVAZIONE").InnerText);
                                DateTime dataCessazione = Convert.ToDateTime(nodo.SelectSingleNode("DATA_CESSAZIONE").InnerText);
                                nodoT.Text = nodo.SelectSingleNode("DESCRIZIONE").InnerText + " in vigore dal " + dataAttivazione.ToString("dd/MM/yyyy") + " al " + dataCessazione.ToString("dd/MM/yyyy");
                                break;
                        }
                        
                        tNode.Nodes.Add(nodoT);

                        nodoT.setIDRECORD(nodo.SelectSingleNode("IDRECORD").InnerText);
                        nodoT.setPARENT(nodo.SelectSingleNode("IDPARENT").InnerText);
                        nodoT.setCODLIV(nodo.SelectSingleNode("CODLIV").InnerText);
                        nodoT.setCODICE(nodo.SelectSingleNode("CODICE").InnerText);
                        nodoT.setDESCRIZIONE(nodo.SelectSingleNode("DESCRIZIONE").InnerText);
                        nodoT.setLIVELLO(nodo.SelectSingleNode("LIVELLO").InnerText);
                        if (nodo.SelectSingleNode("NUMMESICONSERVAZIONE") != null)
                            nodoT.setNUMMESICONSERVAZIONE(nodo.SelectSingleNode("NUMMESICONSERVAZIONE").InnerText);
                        else
                            nodoT.setNUMMESICONSERVAZIONE("0");

                        XmlNode nodoReg = nodo.SelectSingleNode("REGISTRO");
                        if (nodoReg != null)
                        {
                            nodoT.setREGISTRO(nodo.SelectSingleNode("REGISTRO").InnerText);
                        }

                        if (Convert.ToInt32(nodo.SelectSingleNode("FIGLIO").InnerText) > 0)
                        {
                            nodoFiglio = new myTreeNode();
                            nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                            nodoT.Nodes.Add(nodoFiglio);
                        }
					}		
				}	
				else
				{
					lbl_msg.Text = "Nessun nodo di titolario presente!";
				}				
			}
			catch
			{				
				lbl_msg.Text = "Attenzione! si è verificato un errore di caricamento della pagina.";
			}
		}

		/// <summary>
		/// carica nodi figli della treeview
		/// </summary>
		/// <param name="indice"></param>
		private void LoadTreeViewChild(string indice)
		{	
			try
			{
				TreeView1.SelectedNodeIndex = indice;
			
				myTreeNode TreeNodo;
				TreeNodo = (myTreeNode)TreeView1.GetNodeFromIndex(indice);
				TreeNodo.Expanded = true;			
			
				if(TreeNodo.Nodes.Count > 0)
					TreeNodo.Nodes.RemoveAt(0);

				myTreeNode nodoT;
				myTreeNode nodoFiglio;

				string idParent = TreeNodo.getIDRECORD();
	
				DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
				string xmlStream;

				DocsPaWR.Registro rg	= UserManager.getRegistroSelezionato(this.Page);
				DocsPaWR.Ruolo ru		= UserManager.getRuolo(this.Page);

				string idAmm		= ru.idAmministrazione;
				string idGruppo		= ru.idGruppo;
				string idRegistro	= rg.systemId;
				
				xmlStream = ws.NodoTitolarioSecurity(idAmm,idParent,idGruppo,idRegistro,idTitolario);
				
					
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xmlStream);
			
				XmlNode lista = doc.SelectSingleNode("NewDataSet");
				if(lista.ChildNodes.Count > 0)
				{					
					foreach (XmlNode nodo in lista.ChildNodes)
					{
						nodoT = new myTreeNode();
						nodoT.ID = nodo.SelectSingleNode("CODICE").InnerText; 
						nodoT.Text = nodo.SelectSingleNode("CODICE").InnerText + " - " + nodo.SelectSingleNode("DESCRIZIONE").InnerText;										
					
						TreeNodo.Nodes.Add(nodoT);					
					
						nodoT.setIDRECORD(nodo.SelectSingleNode("IDRECORD").InnerText);
						nodoT.setPARENT(nodo.SelectSingleNode("IDPARENT").InnerText);
						nodoT.setCODLIV(nodo.SelectSingleNode("CODLIV").InnerText);
						nodoT.setCODICE(nodo.SelectSingleNode("CODICE").InnerText);
						nodoT.setDESCRIZIONE(nodo.SelectSingleNode("DESCRIZIONE").InnerText);
						nodoT.setLIVELLO(nodo.SelectSingleNode("LIVELLO").InnerText);
                        nodoT.setNUMMESICONSERVAZIONE(nodo.SelectSingleNode("NUMMESICONSERVAZIONE").InnerText);
						XmlNode nodoReg = nodo.SelectSingleNode("REGISTRO");
						if(nodoReg != null)
						{
							nodoT.setREGISTRO(nodo.SelectSingleNode("REGISTRO").InnerText);
						}

						if(Convert.ToInt32(nodo.SelectSingleNode("FIGLIO").InnerText) > 0)
						{
							nodoFiglio = new myTreeNode();						
							nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
							nodoT.Nodes.Add(nodoFiglio);
						}
					}	
				}
			}
			catch
			{				
				lbl_msg.Text = "Attenzione! si è verificato un errore di caricamento della pagina.";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idrecord"></param>
		/// <param name="idparent"></param>
		/// <param name="livello"></param>
		public void LoadTreeViewRicerca(string idrecord, string idparent, int livello)
		{
			try
			{
				LoadTreeView();

				DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
				string xmlStream;

                //Poichè è stato inserito un nodo in piu' che è quello che rappresenta l'amministrazione
                //aumento il livello di una unità
                ++livello;
				xmlStream = ws.findNodoRoot(idrecord, idparent, livello);

				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xmlStream);
			
				XmlNode lista = doc.SelectSingleNode("NewDataSet");
				if(lista.ChildNodes.Count > 0)
				{			
					string indiceCiclo = "0";
					for(int n=1; n<=livello; n++)
					{
						XmlNode liv = doc.SelectSingleNode(".//livello[text()='"+n.ToString()+"']");
						XmlNode root = liv.ParentNode;
						string id = root.ChildNodes.Item(0).InnerText;

						indiceCiclo = EspandiNodoTreeview(id, indiceCiclo);
					}
				}
			}
			catch
			{				
				lbl_msg.Text = "Attenzione! si è verificato un errore di caricamento della pagina.";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idrecord"></param>
		/// <param name="indiceCiclo"></param>
		/// <returns></returns>
		private string EspandiNodoTreeview(string idrecord, string indiceCiclo)
		{	
			try
			{
				Microsoft.Web.UI.WebControls.TreeNode nodes = TreeView1.GetNodeFromIndex(indiceCiclo);

				foreach (myTreeNode myNodo in nodes.Nodes)
				{							
					if(myNodo.getIDRECORD() == idrecord)
					{				
						indiceCiclo = myNodo.GetNodeIndex();
						LoadTreeViewChild(indiceCiclo);					
						return indiceCiclo;
					}
				}	
			}
			catch
			{
				lbl_msg.Text = "Attenzione! si è verificato un errore di caricamento della pagina.";
			}

			return indiceCiclo;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="codClass"></param>
		private void LoadTreeViewConRicerca(string codClass)
		{
			string idrecord=null; 
			string idparent=null; 
			int livello = 0;

			try
			{				
				DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
				string xmlStream;
				
				DocsPaWR.Registro rg = UserManager.getRegistroSelezionato(this.Page);
				string idAmm = rg.idAmministrazione;			

				xmlStream = ws.findNodoByCod(codClass,idAmm, rg.systemId, idTitolario);

				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xmlStream);
			
				XmlNode lista = doc.SelectSingleNode("NewDataSet");
				if(lista.ChildNodes.Count == 1)
				{			
					foreach (XmlNode nodo in lista.ChildNodes)
					{
						idrecord	= nodo.SelectSingleNode("SYSTEM_ID").InnerText;
						idparent	= nodo.SelectSingleNode("ID_PARENT").InnerText;
						livello		= Int32.Parse(nodo.SelectSingleNode("NUM_LIVELLO").InnerText);
					}

					LoadTreeViewRicerca(idrecord, idparent, livello);
				}
				else
				{
					lbl_msg.Text = "Codice non trovato!";
				}
			}
			catch
			{
				lbl_msg.Text = "Attenzione! si è verificato un errore di caricamento della pagina.";
			}
		}

		#endregion	

		#region Tasto OK
		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			if(TreeView1.SelectedNodeIndex != "0")
			{
				Page.Session.Remove("risultatoRicerca");

				myTreeNode nodoSel = (myTreeNode)this.TreeView1.GetNodeFromIndex(TreeView1.SelectedNodeIndex);
                if (nodoSel != null && nodoSel.getCODICE() != "T")
				{				
					DocsPaWR.FascicolazioneClassificazione classificazione = new DocsPAWA.DocsPaWR.FascicolazioneClassificazione();
					classificazione.codice = nodoSel.getCODICE();
                    classificazione.descrizione = nodoSel.getDESCRIZIONE();
                    classificazione.systemID = nodoSel.getIDRECORD();
                    //classificazione.numMesiConservazione = nodoSel.getNUMMESICONSERVAZIONE();

                    //
                    // Mev Ospedale Maggiore Policlinico
                    if (Request.QueryString["isFasc"] == "gestRiclassFasc")
                    {
                        FascicoliManager.setClassificazioneSelezionata(this, classificazione);
                    }
                    // End Mev
                    //

                    if (Request.QueryString["isFasc"] == "gestFasc")
					{
						FascicoliManager.setClassificazioneSelezionata(this,classificazione);						
					}
                    if (Request.QueryString["isFasc"] == "gestArchivio" || Request.QueryString["isFasc"] == "gestScarto")
                    {
                        Session.Add("DaTit", "T");
                        DocsPaWR.Fascicolo fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice(this, classificazione.codice);
                        if (fascicoloSelezionato != null)
                        {
                            FascicoliManager.setFascicoloSelezionato(this, fascicoloSelezionato);
                        }

                    }
                    //if (Request.QueryString["isFasc"] == "gestScarto")
                    //{
                    //    FascicoliManager.setClassificazioneSelezionata(this, classificazione);
                    //}
                    if (Request.QueryString["isFasc"] == "gestClass" || Request.QueryString["isFasc"] == "gestDoc" || Request.QueryString["isFasc"] == "gestProt" || Request.QueryString["isFasc"] == "gestTodolist" || Request.QueryString["isFasc"] == "gestProtInSempl")
                    {
                        DocumentManager.setClassificazioneSelezionata(this, classificazione);
                    }
			
					if(!this.IsStartupScriptRegistered("chiudiModalDialog2"))
					{
						string scriptString = "<SCRIPT>window.close()</SCRIPT>";
						this.RegisterStartupScript("chiudiModalDialog2", scriptString);
					}
				}
			}
		}
		#endregion

		#region ricerca tramite codice e descrizione
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_find_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			try
			{						
				ViewState["risultatoRicerca"] = null;
				GestionePannello("con_ricerca");
				lbl_msg.Text = "";
				lbl_td.Text	 = "";

				string myHtml = "";
				string codice		= txt_find_cod.Text.Trim();
				string descrizione	= txt_find_desc.Text.Trim();
                string note         = txt_note.Text;
                string indice       = txt_indice.Text;

				if(codice != "" || descrizione != "" || note != "" || indice != "")
				{
					DocsPaWR.Ruolo ru = UserManager.getRuolo(this.Page);
					string idAmm = ru.idAmministrazione;
					
					DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    string xmlStream = ws.filtroRicercaTitDocspa(codice, descrizione, note, indice, idAmm, ru.idGruppo, (UserManager.getRegistroSelezionato(this)).systemId, idTitolario);
					if(xmlStream == "<NewDataSet />")
					{
						lbl_msg.Text = "Nessun risultato trovato!";
					}
					else
					{
						XmlDocument doc = new XmlDocument();
						doc.LoadXml(xmlStream);
			
						XmlNode lista = doc.SelectSingleNode("NewDataSet");
						if(lista.ChildNodes.Count > 0)
						{															
							foreach (XmlNode nodo in lista.ChildNodes)
							{
								myHtml +=	"<TR bgColor=#FAFAFA>";
								myHtml +=	"<TD class=testo_grigio_scuro><a href='titolario.aspx?azione=ricerca&idrecord=" + nodo.SelectSingleNode("ID").InnerText + "&idparent=" + nodo.SelectSingleNode("IDPARENT").InnerText + "&livello=" + nodo.SelectSingleNode("LIVELLO").InnerText + "&isFasc=" + Request.QueryString["isFasc"] + "&idTit=" + idTitolario + "' class='testo_grigio_scuro'>" + nodo.SelectSingleNode("CODICE").InnerText + "</a></TD>";
								myHtml +=	"<TD class=testo_grigio>" + nodo.SelectSingleNode("DESCRIZIONE").InnerText.Replace("'","&#39;").Replace("\"","&quot;") + "</TD>";
								myHtml +=	"<TD align=center class=testo_grigio>" + nodo.SelectSingleNode("LIVELLO").InnerText + "</TD>";
								myHtml +=	"</TR>";					
							}

							lbl_td.Text = myHtml;
							Page.Session["risultatoRicerca"] = myHtml;
						}
					}
				}
			}
			catch
			{				
				lbl_msg.Text = "Attenzione! si è verificato un errore di caricamento della pagina.";
			}
		}
		#endregion

		#region gestione pannelli
		private void GestionePannello(string mode)
		{
			switch(mode)
			{
				case "no_ricerca":					
					pnl_ric.Visible		= false;
					TreeView1.Height	= new Unit("550");					
					break;
				case "con_ricerca":
					pnl_ric.Visible		= true;
					TreeView1.Height	= new Unit("320");
					break;
				case "ricerca":					
					pnl_ric.Visible		= true;
					lbl_td.Text			= Page.Session["risultatoRicerca"].ToString();
					TreeView1.Height	= new Unit("320");
					break;
			}
		}
		#endregion

		#region tasto CHIUDI
		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{			
			Session.Remove("risultatoRicerca");
			Session.Remove("Titolario");

			if(!this.IsStartupScriptRegistered("chiudiModalDialog"))
			{
				string scriptString = "<SCRIPT>window.returnValue = 'N'; window.close()</SCRIPT>";
				this.RegisterStartupScript("chiudiModalDialog", scriptString);
			}			
		}		

		private void btn_chiudi_risultato_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{			
			lbl_td.Text = "";
			GestionePannello("no_ricerca");
		}
		#endregion

        protected void img_exportIndice_Click(object sender, ImageClickEventArgs e)
        {
            DocsPAWA.DocsPaWR.OrgTitolario titolario = new DocsPAWA.DocsPaWR.OrgTitolario();
            titolario.ID = idTitolario;
            
            /* Andrea De Marco - Integra
             * modificato il timeout ad infinito perchè il servizio 
             * impiegava più di tre minuti per l'ExportIndiceSistematico
             * 
             * Dopo il rilascio è opportuno utilizzare soluzioni più performanti
             * che traducano il dataset in xml.
            */
            wws.Timeout = System.Threading.Timeout.Infinite;
            //End - Andrea De Marco - Integra

            DocsPAWA.DocsPaWR.FileDocumento fileDoc = wws.ExportIndiceSistematico(titolario);

            if (fileDoc != null)
            {
                DocsPAWA.exportDati.exportDatiSessionManager session = new DocsPAWA.exportDati.exportDatiSessionManager();
                session.SetSessionExportFile(fileDoc);
                ClientScript.RegisterStartupScript(this.GetType(), "openFile", "OpenFile();", true);
            }
        }
        private void ddl_registri_SelectedIndexChanged(object sender, EventArgs e)
        {
            //mette in sessione il registro selezionato
            if (ddl_registri.SelectedIndex != -1)
            {
                DocsPAWA.DocsPaWR.Registro reg = UserManager.getRegistroBySistemId(this.Page, ddl_registri.SelectedValue.Trim());
                UserManager.setRegistroSelezionato(this, reg);
                this.caricaTreeView();
            }                            
        }
        private void CaricaComboRegistri(DropDownList ddl)
        {
            userRegistri = UserManager.getListaRegistri(this);
            string stato;           
            int elemento = 0;
            for (int i = 0; i < userRegistri.Length; i++)
            {
                stato = UserManager.getStatoRegistro(userRegistri[i]);
                {
                    DocsPAWA.DocsPaWR.Registro registro = UserManager.getRegistroBySistemId(this.Page, userRegistri[i].systemId);
                    if (!registro.Sospeso)
                    {
                        ddl.Items.Add(userRegistri[i].codRegistro);
                        ddl.Items[elemento].Value = userRegistri[i].systemId;
                        elemento++;
                    }
                }               
            }
            DocsPAWA.DocsPaWR.Registro reg = UserManager.getRegistroBySistemId(this, this.ddl_registri.Items[0].Value);
            UserManager.setRegistroSelezionato(this, reg);
        }

        private void caricaTreeView()
        {
            switch (Request.QueryString["azione"])
            {
                case null:
                    GestionePannello("no_ricerca");
                    string codClass = Request.QueryString["codClass"];
                    if (codClass == null || codClass == "")
                    {
                        LoadTreeView();
                    }
                    else
                    {
                        LoadTreeViewConRicerca(codClass);
                    }
                    break;
                case "ricerca":
                    GestionePannello("ricerca");
                    string idRecord = Request.QueryString["idrecord"];
                    string idparent = Request.QueryString["idparent"];
                    int livello = Convert.ToInt32(Request.QueryString["livello"]);

                    LoadTreeViewRicerca(idRecord, idparent, livello);
                    break;
            }           
        }
	}
}
