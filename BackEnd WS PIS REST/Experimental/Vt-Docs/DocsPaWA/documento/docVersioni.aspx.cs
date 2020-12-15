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

namespace DocsPAWA.documento
{
	/// <summary>
	/// Summary description for docVersioni.
	/// </summary>
	public class docVersioni : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
	
		//my var
		protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;
		protected ArrayList Dt_elem;
		protected DocsPaWebCtrlLibrary.ImageButton btn_rimuoviVersione;
		protected DocsPaWebCtrlLibrary.ImageButton btn_modiVersione;
		protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungiAreaLav;
		protected DocsPaWebCtrlLibrary.ImageButton btn_nuovaVersione;
		protected System.Web.UI.WebControls.Label lbl_message;
		//protected System.Web.UI.WebControls.Label lbl_ADL;
		protected DocsPAWA.DocsPaWR.Documento[] ListaDocVersioni;
        protected System.Web.UI.HtmlControls.HtmlLink idLinkCss;


		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			Utils.startUp(this);
			schedaDocumento = (DocsPAWA.DocsPaWR.SchedaDocumento) DocumentManager.getDocumentoSelezionato(this);

            Session.Remove("refreshDxPageVisualizzatore");

            // Inizializzazione controllo verifica acl
            if ((schedaDocumento != null) && (schedaDocumento.inCestino != "1") && (schedaDocumento.systemId != null))
            {
                this.InitializeControlAclDocumento();
            }

			if (!this.Page.IsPostBack)
			{
				this.DisableAllButtons();

//				this.btn_aggiungiAreaLav.Enabled = true;
//				this.btn_modiVersione.Enabled = false;
//				this.btn_rimuoviVersione.Enabled = false;
				//DocumentManager.setLabelADL(this.lbl_ADL);
				BindGrid();

				this.PerformActionSelectionFirstVersion();
			}

			setFormProperties();
            	
            if ( (schedaDocumento.inCestino!=null && schedaDocumento.inCestino == "1")
                || (schedaDocumento.inArchivio != null && schedaDocumento.inArchivio == "1"))
                this.DisableAllButtons();
		}

        private string GetCssAmministrazione()
        {
            string Tema = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
            {
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                DocsPAWA.UserManager userM = new UserManager();
                Tema = userM.getCssAmministrazione(idAmm);
            }
            else
            {
                if (UserManager.getInfoUtente() != null)
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    DocsPAWA.UserManager userM = new UserManager();
                    Tema = userM.getCssAmministrazione(idAmm);
                }
            }
            return Tema;
        }

        private void Page_PreRender(object sender, System.EventArgs e)
		{
            string Tema = GetCssAmministrazione();

            if (Tema != null && !Tema.Equals(""))
            {
                string[] realTema = Tema.Split('^');
                this.idLinkCss.Href = "../App_Themes/" + realTema[0] + "/" + realTema[0] + ".css";
            }
            else
                this.idLinkCss.Href = "../App_Themes/TemaRosso/TemaRosso.css";
            
            //abilitazione delle funzioni in base al ruolo
			UserManager.disabilitaFunzNonAutorizzate(this);

			verificaHMdiritti();

			// Ulteriori controlli di disabilitazione

            // controllo se il documento è annullato - in tal caso disabilito tutto
			if (schedaDocumento.protocollo !=null)
			{   
				DocsPaWR.ProtocolloAnnullato protAnnull = schedaDocumento.protocollo.protocolloAnnullato;
				
                if (protAnnull != null && protAnnull.dataAnnullamento != null && !protAnnull.Equals(""))
                    this.DisableAllButtons();
			}

            if (UserManager.isFiltroAooEnabled(this))
            {
                if (this.schedaDocumento != null && this.schedaDocumento.protocollo != null)
                {
                    DocsPaWR.Registro[] userRegistri = DocsPAWA.UserManager.getListaRegistri(this.Page);

                    if (btn_nuovaVersione.Enabled)
                    {
                        btn_nuovaVersione.Enabled = UserManager.verifyRegNoAOO(schedaDocumento, userRegistri);
                    }

                    if (btn_modiVersione.Enabled)
                    {
                        btn_modiVersione.Enabled = UserManager.verifyRegNoAOO(schedaDocumento, userRegistri);
                    }

                    if (btn_rimuoviVersione.Enabled)
                    {
                        btn_rimuoviVersione.Enabled = UserManager.verifyRegNoAOO(schedaDocumento, userRegistri);
                    }

                }
            }

            if (this.schedaDocumento != null && this.schedaDocumento.tipoProto.Equals("C"))
                this.btn_modiVersione.Enabled = false;

            // Controllo su stato documento consolidato
            if (schedaDocumento.ConsolidationState != null &&
                schedaDocumento.ConsolidationState.State >= DocsPaWR.DocumentConsolidationStateEnum.Step1)
            {
                this.DisableButtonsPerConsolidation();
            }
		}

        /// <summary>
        /// Disabilitazione pulsanti per consolidamento
        /// </summary>
        private void DisableButtonsPerConsolidation()
        {
            this.btn_modiVersione.Enabled = false;
            this.btn_rimuoviVersione.Enabled = false;
            this.btn_nuovaVersione.Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisableAllButtons()
        {
            this.btn_aggiungiAreaLav.Enabled = false;
            this.btn_modiVersione.Enabled = false;
            this.btn_rimuoviVersione.Enabled = false;
            this.btn_nuovaVersione.Enabled = false;
        }

		private void verificaHMdiritti()
		{
			//disabilitazione dei bottoni in base all'autorizzazione di HM 
			//sul documento
			if(schedaDocumento!=null && schedaDocumento.accessRights!=null && schedaDocumento.accessRights!="")
			{
				if(UserManager.disabilitaButtHMDiritti(schedaDocumento.accessRights))
				{
					//bottoni che devono essere disabilitati in caso
					//di diritti di sola lettura
					this.btn_nuovaVersione.Enabled = false;
					this.btn_modiVersione.Enabled = false;
					this.btn_rimuoviVersione.Enabled = false;

                    //bottoni che devono essere disabilitati in caso
                    //di documento trasmesso con "Worflow" e ancora da accettare
                    if (UserManager.disabilitaButtHMDirittiTrasmInAccettazione(schedaDocumento.accessRights))
                    {
                        this.btn_aggiungiAreaLav.Enabled = false;
                    }
				}
			}
		}

		private void setFormProperties()
		{
//			this.btn_aggiungiAreaLav.Enabled = false;
//			this.btn_modiVersione.Enabled = false;
//			this.btn_rimuoviVersione.Enabled = false;
//			this.btn_nuovaVersione.Enabled = false;
			

			if (schedaDocumento != null && schedaDocumento.systemId != null)
			{
                bool isStampaRegistro = (this.schedaDocumento.tipoProto == "R" ||  this.schedaDocumento.tipoProto == "C");

                // Abilitazione dei pulsanti di creazione versione 
                // e di inserimento in area di lavoro solamente se il documento
                // non è di tipo stampa registro
                this.btn_aggiungiAreaLav.Enabled = !isStampaRegistro;
                this.btn_nuovaVersione.Enabled = !isStampaRegistro;

				/*  -- elimino il controllo 141202  --
				if (schedaDocumento.protocollo == null || schedaDocumento.protocollo.segnatura == null || schedaDocumento.protocollo.segnatura.Equals(""))
				{		
					this.btn_nuovaVersione.Enabled = true;
				}
				*/
			}
			if(this.DataGrid1.SelectedIndex >=0)
			{
				this.btn_modiVersione.Enabled = true;
				//this.btn_rimuoviVersione.Enabled = true;
			}
			//controllo per la non cancellazione delle versioni
			//per un documento protocollato
			if(schedaDocumento.protocollo != null)
			{
				this.btn_modiVersione.Enabled = false;
				this.btn_rimuoviVersione.Enabled = false;
			
			}

			//if(schedaDocumento.protocollo

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
			this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
			this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_pager);
			this.DataGrid1.PreRender += new System.EventHandler(this.DataGrid1_PreRender);
			this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
			this.btn_nuovaVersione.Click += new System.Web.UI.ImageClickEventHandler(this.btn_nuovaVersione_Click);
			this.btn_aggiungiAreaLav.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungiAreaLav_Click);
			this.btn_modiVersione.Click += new System.Web.UI.ImageClickEventHandler(this.btn_modiVersione_Click);
			this.btn_rimuoviVersione.Click += new System.Web.UI.ImageClickEventHandler(this.btn_rimuoviVersione_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		#region datagrid

		public void BindGrid()
		{
			if (schedaDocumento == null)
				schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
			if (schedaDocumento != null)
			{
				ListaDocVersioni = schedaDocumento.documenti;
				if (ListaDocVersioni != null)
				{
					//Costruisco il datagrid
					Dt_elem = new ArrayList();
					string data_ins;
					for(int i=0;i<ListaDocVersioni.Length;i++)
					{
						//formatto la data nel formato dd/mm/aaaa
						data_ins = ListaDocVersioni[i].dataInserimento;
						
						if (data_ins == null || data_ins == "")
						{
							
							data_ins =Utils.dateLength(schedaDocumento.dataCreazione);
						}
						else
						{					
							data_ins = data_ins.Substring(0,10);
						}
	
						Dt_elem.Add(new Cols(ListaDocVersioni[i].version, ListaDocVersioni[i].descrizione, data_ins, i));
					}
					if (ListaDocVersioni.Length > 0)
					{	
						DocumentManager.setDataGridVersioni(this,Dt_elem );
						this.DataGrid1.DataSource=Dt_elem;
						this.DataGrid1.DataBind();
						int i = 0;
						foreach(DataGridItem dgi in DataGrid1.Items)
						{
                            string autore;
                            if (!string.IsNullOrEmpty(ListaDocVersioni[i].idPeopleDelegato) && ListaDocVersioni[i].idPeopleDelegato != "0")
                                autore = ListaDocVersioni[i].idPeopleDelegato + "\nDelegato da " + ListaDocVersioni[i].autore;
                            else
                                autore = ListaDocVersioni[i].autore;
                            dgi.ToolTip = autore;
							i++;
						}
					}
					else
						this.lbl_message.Visible = true;
				}
				else
					this.lbl_message.Visible = true;
				
			}				
		}


		public class Cols 
		{		
			private string codice;
			private string note;
			private string data;
			private int chiave;

			public Cols(string codice, string note, string data, int chiave)
			{
				this.codice = codice;
				this.note = note;
				this.data = data;
				this.chiave = chiave;
			}
					
			public string Codice{get{return codice;}}
			public string Note{get{return note;}}
			public string Data{get{return data;}}
			public int Chiave{get{return chiave;}}
					
					
		}


		private void DataGrid1_pager(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.DataGrid1.CurrentPageIndex=e.NewPageIndex;
			ArrayList ColNew;
			ColNew = DocumentManager.getDataGridVersioni(this);
			
			DataGrid1.DataSource=ColNew; 
						
			DataGrid1.DataBind();

			this.PerformActionSelectionFirstVersion();
		}

		private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//setto il file da visualizzare
			string str_indexSel = ((Label) this.DataGrid1.SelectedItem.Cells[4].Controls[1]).Text;
			int indexSel = Int32.Parse(str_indexSel);

			this.PerformVersionSelection(indexSel);			
		}

		#endregion datagrid

		private void btn_aggiungiAreaLav_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                try
                {
                    DocumentManager.addAreaLavoro(this, schedaDocumento);
                    string msg = "Documento aggiunto all'area di lavoro";
                    Response.Write("<script>alert(\"" + msg + "\");</script>");
                }
                catch (Exception ex)
                {
                    ErrorManager.redirect(this, ex);
                }
            }
		}

		private void btn_modiVersione_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                try
                {
                    string str_indexSel = ((Label)this.DataGrid1.SelectedItem.Cells[4].Controls[1]).Text;
                    int indexSel = Int32.Parse(str_indexSel);
                    if (indexSel >= 0)
                    {
                        Response.Write("<script language='javascript' src='../LIBRERIE/DocsPA_Func.js'></script>");
                        Response.Write("<script>ApriFinestraGestVersioni('" + str_indexSel + "'); </script>");
                    }
                }
                catch (Exception ex)
                {
                    ErrorManager.redirect(this, ex);
                }
            }
		}

		/// <summary>
		/// Gestione abilitazione / disabiltazione pulsanti
		/// rispetto alla versione correntemente selezionata
		/// </summary>
		private void EnableButtonsVersioneSeleziontata()
		{
			// se documento principale, non può essere rimosso
			this.btn_rimuoviVersione.Enabled=this.IsDocumentoVersionePrincipale();
		}

		/// <summary>
		/// Verifica se la versione selezionata è la prima immessa
		/// </summary>
		/// <param name="selectedIndex"></param>
		/// <returns></returns>
		private bool IsDocumentoVersionePrincipale(int selectedIndex)
		{
			bool retValue=false;
            if (selectedIndex == 0)
            {
                retValue = true;
            }
            else
            {
                retValue=false;
            }
			return retValue;
		}

		/// <summary>
		/// Verifica se la versione selezionata è la prima immessa
		/// </summary>
		/// <returns></returns>
		private bool IsDocumentoVersionePrincipale()
		{
			int selectedIndex = Convert.ToInt32( ((Label) this.DataGrid1.SelectedItem.Cells[4].Controls[1]).Text);
			
			return this.IsDocumentoVersionePrincipale(selectedIndex);
		}
		
		/// <summary>
		/// Azione di inserimento di una nuova versione.
		/// Precondizioni:
		///		- Il documento non deve essere bloccato
		/// </summary>
		private void AddVersion()
		{
			string checkOutMessage;

			if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(out checkOutMessage))
			{
				this.RegisterClientScript("checkOutMessage","alert('" + checkOutMessage + "');");
			}
			else
			{
				Response.Write("<script language='javascript' src='../LIBRERIE/DocsPA_Func.js'></script>");
				this.RegisterClientScript("ApriFinestraGestVersioni","ApriFinestraGestVersioni('');");
			}
		}

		/// <summary>
		/// Azione di selezione prima versione disponibile
		/// </summary>
		private void PerformActionSelectionFirstVersion()
		{
			if (this.DataGrid1.Items.Count>0)
			{
				string str_indexSel = ((Label) this.DataGrid1.Items[0].Cells[4].Controls[1]).Text;
				int selectedIndex=Int32.Parse(str_indexSel);

				PerformVersionSelection(selectedIndex);

				this.DataGrid1.SelectedIndex=0;
			}
		}

		/// <summary>
		/// Azione di selezione di un documento nella griglia
		/// </summary>
		/// <param name="selectedIndex"></param>
		private void PerformVersionSelection(int selectedIndex)
		{			
			this.btn_rimuoviVersione.Attributes["onClick"]="ApriFinestraRimuoviVersione(" + selectedIndex + ");";

			if (selectedIndex >= 0)
			{	
				FileManager.setSelectedFile(this, schedaDocumento.documenti[selectedIndex]);
			}

			if (schedaDocumento.protocollo != null && schedaDocumento.protocollo.segnatura != null && schedaDocumento.protocollo.segnatura.Length > 0)  
			{
				this.btn_rimuoviVersione.Enabled = false;
				this.btn_modiVersione.Enabled = false;
			}
			else
			{
                if (this.DataGrid1.Items.Count == 1)
                    //this.btn_rimuoviVersione.Enabled = (!this.IsDocumentoVersionePrincipale(selectedIndex));
                    this.btn_rimuoviVersione.Enabled = false;
                else
                    this.btn_rimuoviVersione.Enabled = true;

                if (schedaDocumento.inCestino != null && schedaDocumento.inCestino == "1")
                   this.btn_modiVersione.Enabled = false;
                else
                   this.btn_modiVersione.Enabled = true;
			}
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

		private void btn_nuovaVersione_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                this.AddVersion();
            }
		}

		private void DataGrid1_PreRender(object sender, System.EventArgs e)
		{
			try
			{
			
				
				for(int i=0;i<this.DataGrid1.Items.Count;i++)
				{
					if(this.DataGrid1.Items[i].ItemIndex>=0)
					{	
//						switch(this.DataGrid1.Items[i].ItemType.ToString().Trim())
//						{
//							case "Item":
//							{
//								this.DataGrid1.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
//								this.DataGrid1.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioN'");
//								break;
//							}
//							case "AlternatingItem":
//					
//							{
//								this.DataGrid1.Items[i].Attributes.Add("onmouseover","this.className='bg_grigioS'");
//								this.DataGrid1.Items[i].Attributes.Add("onmouseout","this.className='bg_grigioA'");
//								break;
//							}
//				
//						}
					
						//abilito - disabilito il bottone con i dati per i firmatari
						if (schedaDocumento.documenti!=null && schedaDocumento.documenti.Length > 0)
						{
							if (schedaDocumento.documenti[i].firmatari != null && schedaDocumento.documenti[i].firmatari.Length > 0)
								((ImageButton) this.DataGrid1.Items[i].Cells[3].Controls[1]).Visible = true;
						}
					}
				}
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}
		}

		private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if(e.CommandName.Equals("Firma"))
			{
				string index;
				index = ((Label) this.DataGrid1.Items[e.Item.ItemIndex].Cells[4].Controls[1]).Text;
				int indexSel = Int32.Parse(index);
				if (indexSel >= 0)
				{
					Page.Response.Write("<script language='javascript' src='../LIBRERIE/DocsPA_Func.js'></script>");
					Page.Response.Write("<script>ApriFinestraFirmatari('../popup/dettagliFirma.aspx'," + index + ");</script>");
				}
			}
		}
       
		private void btn_rimuoviVersione_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            if (!this.GetControlAclDocumento().AclRevocata)
            {
                BindGrid();

                this.PerformActionSelectionFirstVersion();
            }
		}

        protected void DataGrid1_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        #region Gestione controllo acl documento

        /// <summary>
        /// Inizializzazione controllo verifica acl
        /// </summary>
        protected virtual void InitializeControlAclDocumento()
        {
            AclDocumento ctl = this.GetControlAclDocumento();
            ctl.IdDocumento = DocumentManager.getDocumentoSelezionato().systemId;
            ctl.OnAclRevocata += new EventHandler(this.OnAclDocumentoRevocata);
        }

        /// <summary>
        /// Reperimento controllo acldocumento
        /// </summary>
        /// <returns></returns>
        protected AclDocumento GetControlAclDocumento()
        {
            return (AclDocumento)this.FindControl("aclDocumento");
        }

        /// <summary>
        /// Listener evento OnAclDocumentoRevocata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAclDocumentoRevocata(object sender, EventArgs e)
        {
            // Redirect alla homepage di docspa
            SiteNavigation.CallContextStack.Clear();
            SiteNavigation.NavigationContext.RefreshNavigation();
            string script = "<script>window.open('../GestioneRuolo.aspx','principale','width=410,height=300,scrollbars=NO');</script>";
            Response.Write(script);
        }

        #endregion

	}
}
