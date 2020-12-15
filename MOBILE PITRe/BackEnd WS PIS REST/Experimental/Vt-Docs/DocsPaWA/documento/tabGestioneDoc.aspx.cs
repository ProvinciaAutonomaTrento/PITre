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
using System.Configuration;
using System.Globalization;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for tab_gestioneDoc.
	/// </summary>
	public class tabGestioneDoc : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.DropDownList ddl_registri;
		protected System.Web.UI.WebControls.Label lbl_registri;
		protected System.Web.UI.WebControls.Image img_statoReg;
		protected DocsPaWebCtrlLibrary.ImageButton btn_profilo;
		protected DocsPaWebCtrlLibrary.ImageButton btn_protocollo;
		protected DocsPaWebCtrlLibrary.ImageButton btn_classifica;
		protected DocsPaWebCtrlLibrary.ImageButton btn_allegati;
		protected DocsPaWebCtrlLibrary.ImageButton btn_versioni;
		protected DocsPaWebCtrlLibrary.ImageButton btn_trasmissioni;
		protected System.Web.UI.HtmlControls.HtmlTableCell td_tab1;
		protected System.Web.UI.HtmlControls.HtmlTableCell td_tab2;
		protected System.Web.UI.HtmlControls.HtmlTableCell td_tab3;
		protected System.Web.UI.HtmlControls.HtmlTableCell td_tab4;
		protected System.Web.UI.HtmlControls.HtmlTableCell td_tab5;
		protected DocsPaWebCtrlLibrary.IFrameWebControl IframeTabs;
		protected System.Web.UI.HtmlControls.HtmlTableCell td_tab6;


		private DocsPAWA.DocsPaWR.Registro[] userRegistri;
		private DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;
		protected System.Web.UI.WebControls.Image icoReg;
		protected System.Web.UI.WebControls.Panel pnl_regStato;

		private string nomeTab;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_currentTabName;
		protected System.Web.UI.WebControls.Label lbl_ruolo;
		protected System.Web.UI.WebControls.Panel pnl_ruolo;
		protected System.Web.UI.WebControls.Panel pnl_random;
		protected System.Web.UI.WebControls.Panel pnl_cont;
		string isNew="";
        string daCestino;

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

		private void Page_Load(object sender, System.EventArgs e)
		{
            if (CallContextStack.CurrentContext.ContextState["isCheckin"] == null)
                CallContextStack.CurrentContext.ContextState["isCheckin"] = string.Empty;
			try 
			{ 
                string color = string.Empty;
                string Tema = GetCssAmministrazione();
                if (Tema != null && !Tema.Equals(""))
                {
                    string[] realTema = Tema.Split('^');
                    color = "#" + realTema[2];
                }
                else
                    color = "#810d06";


                daCestino = Request.QueryString["daCestino"];
                if (daCestino == null) daCestino = "0";
				Utils.startUp(this);
				isNew = Request.QueryString["isNew"];

				if (isNew==null) isNew="0";

				if (isNew=="1" && !IsPostBack)
				{
					DocumentManager.removeDocumentoSelezionato(this);
					DocumentManager.removeDocumentoInLavorazione(this);
					CleanSessionMemoria();
				}

				if((!Request.QueryString["tab"].Equals("")) && (!Request.QueryString["tab"].Equals(null)))
				{
                    if (!string.IsNullOrEmpty(CallContextStack.CurrentContext.ContextState["isCheckin"].ToString())
                       && CallContextStack.CurrentContext.ContextState["isCheckin"].Equals("isCheckin"))
                    {
                        nomeTab = "profilo";
                        CallContextStack.CurrentContext.ContextState["isCheckin"] = String.Empty;
                    }
                    else
                        nomeTab = Request.QueryString["tab"].ToString();

					//disabilita funzioni di stampa segnatura
					if (Session["allegato"]!=null)
					{
						Session.Remove("allegato");
					}

					CaricaTab(nomeTab);	
				}
				else
					nomeTab = null;			

				loadSchedaDocumento();

				string rigaDescrizione="<tr><td align=\"center\" height=\"15\" class=\"titolo_bianco\" bgcolor=\"" + color + "\">Registro</td></tr>";
				
                if(!IsPostBack) 
				{
					CaricaComboRegistri(ddl_registri);
                    
                    if (schedaDocumento != null && schedaDocumento.registro != null && schedaDocumento.registro.systemId != null)
					{
						Page.RegisterClientScriptBlock("CallDescReg","<!--Desc Reg inzio--><DIV onmouseover=\"closeIt()\"><LAYER onmouseover=\"closeIt()\"></LAYER></DIV><DIV id=\"descreg\" style=\"visibility:hidden;LEFT: 200px; POSITION: absolute; TOP: 24px\"><div align=\"left\"><table cellSpacing=\"0\" border='1' bordercolordark='#ffffff' cellPadding=\"0\" bgColor=\"#d9d9d9\"  width='210px' height='60px'>"+rigaDescrizione+"<tr><td  bgColor=\"#d9d9d9\" class=\"testo_grigio_sp\">"+schedaDocumento.registro.descrizione+"</td></tr></table></div></DIV><!--Fine desc reg-->");	

						this.Session["RegistroSelezionato"] =schedaDocumento.registro.systemId.Trim();
					}
					else
					{
                        Page.RegisterClientScriptBlock("CallDescReg", "<!--Desc Reg inzio--><DIV onmouseover=\"closeIt()\"><LAYER onmouseover=\"closeIt()\"></LAYER></DIV><DIV id=\"descreg\" style=\"visibility:hidden;LEFT: 200px; POSITION: absolute; TOP: 24px\"><div align=\"left\"><table cellSpacing=\"0\" border='1' bordercolordark='#ffffff' cellPadding=\"0\" bgColor=\"#d9d9d9\"  width='210px' height='60px'>" + rigaDescrizione + "<tr><td  bgColor=\"#d9d9d9\" class=\"testo_grigio_sp\">" + userRegistri[this.ddl_registri.SelectedIndex].descrizione + "</td></tr></table></div></DIV><!--Fine desc reg-->");
					
					    if (ddl_registri.SelectedValue != null)
						    this.Session["RegistroSelezionato"] =ddl_registri.SelectedValue.Trim();
					}
				}
				else
					Page.RegisterClientScriptBlock("CallDescReg","<!--Desc Reg inzio--><DIV onmouseover=\"closeIt()\"><LAYER onmouseover=\"closeIt()\"></LAYER></DIV><DIV id=\"descreg\" style=\"visibility:hidden;LEFT: 200px; POSITION: absolute; TOP: 24px\"><div align=\"left\"><table cellSpacing=\"0\" border='1' bordercolordark='#ffffff' cellPadding=\"0\" bgColor=\"#d9d9d9\"  width='210px' height='60px'>"+rigaDescrizione+"<tr><td  bgColor=\"#d9d9d9\" class=\"testo_grigio_sp\">"+UserManager.getRuolo(this).registri[ddl_registri.SelectedIndex].descrizione+"</td></tr></table></div></DIV><!--Fine desc reg-->");	

                if (!this.IsPostBack)
                {
                    this.CurrentTabName = nomeTab;
                    Session.Remove("refreshDxPageVisualizzatore");
                }

                //gestisce refresh documento visualizzato
                
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void CleanSessionMemoria()
		{
			DocumentManager.removeMemoriaFiltriRicDoc(this);
			DocumentManager.removeMemoriaNumPag(this);
			DocumentManager.removeMemoriaTab(this);
			DocumentManager.RemoveMemoriaVisualizzaBack(this);

			FascicoliManager.removeMemoriaRicFasc(this);
			FascicoliManager.RemoveMemoriaVisualizzaBack(this);
			FascicoliManager.SetFolderViewTracing(this, false);
		}

		private void nuovaSchedaDocumento() 
		{
            schedaDocumento = DocumentManager.getDocumentoSelezionato(this);

			if (schedaDocumento == null) 
			{
                //crea nuovo documento
                DocsPaWR.Utente utente = UserManager.getUtente(this);
                DocsPaWR.Ruolo ruolo = UserManager.getRuolo(this);
                schedaDocumento = new DocsPAWA.DocsPaWR.SchedaDocumento();
                schedaDocumento.systemId = null;
                schedaDocumento.oggetto = new DocsPAWA.DocsPaWR.Oggetto();

                // campi obbligatori per DocsFusion
                schedaDocumento.idPeople = utente.idPeople;
                schedaDocumento.userId = utente.userId;
                //schedaDocumento.typeId = "LETTERA";
                schedaDocumento.typeId = DocumentManager.getTypeId();
                schedaDocumento.appId = "ACROBAT";
                schedaDocumento.privato = "0";  //doc non privato
			}
//			else
//			{
				// campi obbligatori per DocsFusion perchè se:
				// si fa new proto poi si va su profilo poi si torna proto e 
				// si fa riproponidati e si proto da errore perchè questi dati 
				//obbligatori per docfusion non ci sono più sono a null 21/02/2003
//				if(schedaDocumento.appId==null)
//					schedaDocumento.appId="ACROBAT";
//				if(schedaDocumento.idPeople==null)
//					schedaDocumento.idPeople=UserManager.getUtente(this).idPeople;
//				if(schedaDocumento.typeId==null)
//					schedaDocumento.typeId = "LETTERA";
//				if(schedaDocumento.userId==null)
//					schedaDocumento.userId = UserManager.getUtente(this).userId;
//				if(schedaDocumento.modOggetto=="0")
//					schedaDocumento.modOggetto=null;
//				if(schedaDocumento.assegnato=="0")
//					schedaDocumento.assegnato=null;
//				if(schedaDocumento.fascicolato=="0")
//					schedaDocumento.fascicolato=null;
//			}
		}


		
		private void loadSchedaDocumento()
		{	
			try 
			{
				Response.Expires = 0;
				DocumentManager.removeDatagridDocumento(this);  //per la ricerca
				DocumentManager.setBlockQuickProt(this,false);
				nuovaSchedaDocumento();
				if (schedaDocumento != null) 
				{
					bool dbPredisponiProtocollazione = false;
					if(DocumentManager.getDocumentoSelezionato(this) != null)
						dbPredisponiProtocollazione = DocumentManager.getDocumentoSelezionato(this).predisponiProtocollazione;
					bool predisponiProtocollazione = false;
					if((DocumentManager.getDocumentoInLavorazione(this)) != null)
						predisponiProtocollazione = (DocumentManager.getDocumentoInLavorazione(this)).predisponiProtocollazione;
					if (!dbPredisponiProtocollazione.Equals(predisponiProtocollazione) && predisponiProtocollazione)			
					{
						schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
					}
				}
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		private string CurrentTabName
		{
			get
			{
				string retValue=string.Empty;

				if (this.ViewState["CurrentTabName"]!=null)
					retValue=this.ViewState["CurrentTabName"].ToString();

				return retValue;
			}
			set
			{
				this.ViewState["CurrentTabName"]=value;
			}
		}

		private void btn_Click(ImageButton btn)
		{
            schedaDocumento = DocumentManager.getDocumentoSelezionato();
            bool inRepositoryContext = (schedaDocumento != null && schedaDocumento.repositoryContext != null);

            if (!inRepositoryContext)
            {
                DocumentManager.removeDocumentoInLavorazione(this);

                if (nomeTab.Equals("protocollo") && schedaDocumento != null)
                {
                    schedaDocumento.protocollo = null;
                    if (daCestino != "1") //browse dettaglio documento venendo dal cestino.
                        schedaDocumento = DocumentManager.getDettaglioDocumento(this, schedaDocumento.systemId, schedaDocumento.docNumber);
                    else
                        schedaDocumento = DocumentManager.getDettaglioDocumentoDaCestino(this, schedaDocumento.systemId, schedaDocumento.docNumber);
                    if (schedaDocumento != null)
                        DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                }
            }

			loadSchedaDocumento();
			updateTab(btn);
		}

		/// <summary>
		/// Aggiornamento del contesto del documento relativamente
		/// al tab correntemente selezionato
		/// </summary>
		private void RefreshContext()
		{
			SiteNavigation.CallContext currentContext=SiteNavigation.CallContextStack.CurrentContext;
			
			if (currentContext!=null && currentContext.ContextName=="Documento")
			{
				currentContext.QueryStringParameters["isNew"]="0";
				currentContext.QueryStringParameters["tab"]=this.GetCurrentTabName();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabName"></param>
		/// <returns></returns>
		private string GetTabDescription(string tabName)
		{
			string description=string.Empty;

			switch (tabName.ToUpper())
			{
				case "PROFILO":
					description="Profilo doc.";
					break;

				case "PROTOCOLLO":
					description="Protocollo doc.";
					break;

				case "CLASSIFICA":
					description="Classifica doc.";
					break;

				case "ALLEGATI":
					description="Allegati doc.";
					break;

				case "VERSIONI":
					description="Versioni doc.";
					break;

				case "TRASMISSIONI":
					description="Trasmissioni doc.";
					break;
			}

			return description;
		}

		private void updateTab(ImageButton btn) {
			
			string strNumTab=btn.ID.Substring(btn.ID.IndexOf("_")+1);
			nomeTab = strNumTab;

			//pulire la session se fatta ricerca trasmissione e reload del iframe_dx.
			CleanSessionTabTrasm(strNumTab);//

			//ripulisco la sessione per gli altri tab e ricarico il destro se necessario
			string strNumTabPrec = "";
			if (ViewState["ID_Butt_precedente"]!=null)
			{
				strNumTabPrec = ((string) ViewState["ID_Butt_precedente"]);
				strNumTabPrec = strNumTabPrec.Substring(strNumTabPrec.IndexOf("_")+1);
                if (strNumTabPrec.Equals("versioni") || strNumTabPrec.Equals("allegati") || strNumTabPrec.Equals("trasmissioni"))
                {
                    Session.Remove("refreshDxPageVisualizzatore");
                }
			}

			CleanSession(strNumTab, strNumTabPrec);
			
//			//solo se già un butt è stato cliccato
//			if(ViewState["ID_Butt_precedente"]!=null)
//			{
//				//img del butt precedentemente cliccato torna a nonattivo.	
//				
//				ImageButton btnPrec=((ImageButton)Page.FindControl((string)ViewState["ID_Butt_precedente"]));
//				btnPrec.ImageUrl=this.GetTabButtonImageUrl(btnPrec,false);
//			}

			//serve per segnare la prima volta che si click un bottone.
//			if(Page.IsPostBack)
//				ViewState.Add("ID_Butt_precedente",btn.ID);
				
			//cambio img del butt cliccato
			//btn.ImageUrl="../images/"+strNumTab+"_attivo.gif";		

//			btn.ImageUrl=this.GetTabButtonImageUrl(btn,true);;

			if (nomeTab.Equals("protocollo"))
			{
				pnl_regStato.Visible = true;
				this.pnl_random.Visible=true;
			}
			else
			{
				pnl_regStato.Visible = false;
				this.pnl_random.Visible=false;
			}
			
			this.ViewState["ID_Butt_precedente"]=btn.ID;
		}

		private void btn_protocollo_Click(object sender,System.Web.UI.ImageClickEventArgs e)
		{
			btn_Click((ImageButton)sender);
		}

		private void CleanSession(string tab, string old_tab)
		{
			DocumentManager.removeDataGridAllegati(this);
			DocumentManager.removeDataGridVersioni(this);

			Session.Remove("VisSegn");

			if(tab.Equals("allegati") || tab.Equals("versioni"))
			{
				FileManager.removeSelectedFile(this);
		//		System.Diagnostics.Trace.WriteLine(this.GetType().ToString()+"alert('tabdoc1');top.principale.iFrame_dx.document.location='tabDoc.aspx';");
				Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='tabDoc.aspx'; </script>");//='tabDoc.aspx';
			}
//			if (!( (tab.Equals("profili") && old_tab.Equals("protocollo")) || (old_tab.Equals("profili") && tab.Equals("protocollo")) ))
//				Response.Write("<script>parent.iFrame_dx.document.location='tabDoc.aspx';</script>");

		}
		private void CleanSessionTabTrasm(string tab)
		{
			TrasmManager.removeDocTrasmQueryEff(this);
			TrasmManager.removeDataTableRic(this);
			TrasmManager.removeDocTrasmSel(this);
			if(!tab.Equals("allegati") || !tab.Equals("versioni"))
			{
				if(tab.Equals("trasmissioni")  )
				{
				//	System.Diagnostics.Trace.WriteLine(this.GetType().ToString()+"top.principale.iFrame_dx.document.location='tabTrasmissioniEff.aspx';");
					Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location='tabTrasmissioniEff.aspx';</script>");
				}
				else
				{
                    // Se è attivo l'autopreview viene effettuato direttamente il refresh della pagina di destra in modo da aggiornare
                    // i dati sullo stato di checkout del documento (MAC INPS 71)
                    if(ConfigSettings.getKey(ConfigSettings.KeysENUM.DOCUMENT_AUTOPREVIEW).ToLower() == "true")
                        Response.Write("<script language='javascript'>try {top.principale.iFrame_dx.document.location='tabDoc.aspx'} catch(e) { try {top.principale.iFrame_dx.document.location='tabDoc.aspx'} catch(e) {} }</script>");
                    else
				        // PAT: Non vogliono che la pagina di destra si refreshi quando cliccano sul tab profilo
                        if (Session["refreshDxPageVisualizzatore"] == null && Convert.ToBoolean(Session["refreshDxPageVisualizzatore"]) != true)
                        {
                            Response.Write("<script language='javascript'>try {top.principale.iFrame_dx.document.location='tabDoc.aspx'} catch(e) { try {top.principale.iFrame_dx.document.location='tabDoc.aspx'} catch(e) {} }</script>");
                        }
                }
			}
		}

        private void btn_trasmissioni_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
			btn_Click((ImageButton)sender);
		}

		private void btn_classifica_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
			btn_Click((ImageButton)sender);
		}

		private void btn_versioni_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
			btn_Click((ImageButton)sender);
		}

		private void btn_profilo_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
			btn_Click((ImageButton)sender);
		}

		private void btn_allegati_Click(object sender, System.Web.UI.ImageClickEventArgs e) {
			btn_Click((ImageButton)sender);
		}

		private void CaricaTab(string nomeTab)
		{
			string nomeButtTab="btn_"+nomeTab;

			ImageButton ButtImg = (ImageButton) Page.FindControl(nomeButtTab);
			updateTab(ButtImg);			
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
            //this.RefreshTabButtonsImages();
            base.OnInit(e);
		}
//		private void InitializeComponent()
//		{    
//			this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
//			this.btn_profilo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_profilo_Click);
//			this.btn_protocollo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_protocollo_Click);
//			this.btn_classifica.Click += new System.Web.UI.ImageClickEventHandler(this.btn_classifica_Click);
//			this.btn_allegati.Click += new System.Web.UI.ImageClickEventHandler(this.btn_allegati_Click);
//			this.btn_versioni.Click += new System.Web.UI.ImageClickEventHandler(this.btn_versioni_Click);
//			this.btn_trasmissioni.Click += new System.Web.UI.ImageClickEventHandler(this.btn_trasmissioni_Click);
//			this.Load += new System.EventHandler(this.Page_Load);
//			this.PreRender += new System.EventHandler(this.tabGestioneDoc_PreRender);
//
//		}
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btn_profilo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_profilo_Click);
			this.btn_protocollo.Click += new System.Web.UI.ImageClickEventHandler(this.btn_protocollo_Click);
			this.btn_classifica.Click += new System.Web.UI.ImageClickEventHandler(this.btn_classifica_Click);
			this.btn_allegati.Click += new System.Web.UI.ImageClickEventHandler(this.btn_allegati_Click);
			this.btn_versioni.Click += new System.Web.UI.ImageClickEventHandler(this.btn_versioni_Click);
			this.btn_trasmissioni.Click += new System.Web.UI.ImageClickEventHandler(this.btn_trasmissioni_Click);
			this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.tabGestioneDoc_PreRender);

		}
		#endregion


		private void CaricaComboRegistri(DropDownList ddl)
		{
			userRegistri = UserManager.getListaRegistri(this);
			string stato;
			string inCondition = "IN ( ";
			string inConditionSimple="";
            int elemento = 0;
			for(int i=0;i<userRegistri.Length;i++)
			{
				stato = UserManager.getStatoRegistro(userRegistri[i]);
				{
                    DocsPAWA.DocsPaWR.Registro registro = UserManager.getRegistroBySistemId(this.Page, userRegistri[i].systemId);
                    if (!registro.Sospeso)
                    {
                        //Andrea De Marco - Visibilità registri in creazione documento - i registri di pregresso non devono essere visibili
                        if (!registro.flag_pregresso)
                        {
                            ddl.Items.Add(userRegistri[i].codRegistro);
                            ddl.Items[elemento].Value = userRegistri[i].systemId;
                            elemento++;
                        }
                        //End De Marco - per ripristino, commentare De Marco e decommentare codice sottostante
                        //ddl.Items.Add(userRegistri[i].codRegistro);
                        //ddl.Items[elemento].Value = userRegistri[i].systemId;
                        //elemento++;
                    }
				}
				inCondition = inCondition + userRegistri[i].systemId ;
				inConditionSimple = inConditionSimple + userRegistri[i].systemId; 
				if(i < userRegistri.Length - 1)
				{
					inCondition = inCondition +  " , ";
					inConditionSimple = inConditionSimple +  " , ";
				}
			}
			inCondition = inCondition + " )";

			Session["inRegCondition"] = inCondition;
			Session["inRegConditionSimple"] = inConditionSimple;
			//setto lo stato del registro
			if (userRegistri.Length > 0)
				setStatoReg(userRegistri[0]);
		}

		private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//mette in sessione il registro selezionato
			if (ddl_registri.SelectedIndex!=-1)
			{
				if (userRegistri == null)
					userRegistri = UserManager.getListaRegistri(this);
				setStatoReg(UserManager.getRegistroBySistemId(this, ddl_registri.SelectedValue));
				setRegistro(UserManager.getRegistroBySistemId(this, ddl_registri.SelectedValue));

				this.Session["RegistroSelezionato"] = ddl_registri.SelectedValue.Trim();
			}
		}

	#region GestioneRegistro

		private void setStatoReg(DocsPAWA.DocsPaWR.Registro reg)
		{
			// inserisco il registro selezionato in sessione			
			UserManager.setRegistroSelezionato(this, reg);
			string nomeImg;

			if(UserManager.getStatoRegistro(reg).Equals("G"))
				nomeImg = "stato_giallo2.gif";
			else if (UserManager.getStatoRegistro(reg).Equals("V"))
				nomeImg = "stato_verde2.gif";
			else
				nomeImg = "stato_rosso2.gif";

			this.img_statoReg.ImageUrl = "../images/" + nomeImg;
			
		}


		private void setRegistro(DocsPAWA.DocsPaWR.Registro reg)
		{
			schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
			schedaDocumento.registro = reg;
			//aggiunto per annullare la risposta al protocollo se si cambia registro
			//commentato il 20/07/2005 per modifica richiesta per ANAS
			//schedaDocumento.protocollo.rispostaProtocollo = null;

			//aggiunto per risettare la data
			if (schedaDocumento.protocollo != null)
			{
				schedaDocumento.protocollo.dataProtocollazione = null;
				if(schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloEntrata)) 
				{
					if (! corrInRegistro(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente, reg))
						((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittente = null;
					if(! corrInRegistro(((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio, reg))
						((DocsPAWA.DocsPaWR.ProtocolloEntrata)schedaDocumento.protocollo).mittenteIntermedio = null;

				}
				else
					if(schedaDocumento.protocollo.GetType() == typeof(DocsPAWA.DocsPaWR.ProtocolloUscita)) 
				{
					// destinatari
					DocsPaWR.Corrispondente[] listaCorr = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari;
					if (listaCorr != null) 
					{
						for (int i=listaCorr.Length-1; i > -1; i--) 
						{
							if(!corrInRegistro(listaCorr[i], reg)) 
								listaCorr = UserManager.removeCorrispondente(listaCorr,i);
						}
						((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatari = listaCorr;
					}
					// destinatari per conoscenza
					listaCorr = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza;
					if (listaCorr != null) 
					{
						for (int i=listaCorr.Length-1; i > -1; i--) 
						{
							if(!corrInRegistro(listaCorr[i], reg)) 
								listaCorr = UserManager.removeCorrispondente(listaCorr,i);
						}
						((DocsPAWA.DocsPaWR.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza = listaCorr;
					}
				}
			}
			
		}

        private void setRegistroNoControlloCorrInReg(DocsPAWA.DocsPaWR.Registro reg)
        {
            schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
            schedaDocumento.registro = reg;
            //aggiunto per annullare la risposta al protocollo se si cambia registro
            //commentato il 20/07/2005 per modifica richiesta per ANAS
            //schedaDocumento.protocollo.rispostaProtocollo = null;

            //aggiunto per risettare la data
            if (schedaDocumento.protocollo != null)
            {
                schedaDocumento.protocollo.dataProtocollazione = null;
            }

        }

		private bool corrInRegistro(DocsPAWA.DocsPaWR.Corrispondente corr, DocsPAWA.DocsPaWR.Registro reg)
		{
			if (corr == null)
				return true;
			
			if (corr.idRegistro == null || corr.idRegistro == reg.systemId ||  corr.idRegistro == "")
				return true;
			else 
				return false;
		}

		#endregion

		private void tabGestioneDoc_PreRender(object sender, System.EventArgs e)
		{
            btn_allegati.Attributes.Add("onclick","DocsPa_FuncJS_WaitWindows()");
//			btn_classifica.Attributes.Add("onclick","DocsPa_FuncJS_WaitWindows()");
//			//btn_profilo.Attributes.Add("onclick","DocsPa_FuncJS_WaitWindows()");
//			btn_protocollo.Attributes.Add("onclick","DocsPa_FuncJS_WaitWindows()");
//			btn_trasmissioni.Attributes.Add("onclick","DocsPa_FuncJS_WaitWindows()");
//			btn_versioni.Attributes.Add("onclick","DocsPa_FuncJS_WaitWindows()");


            if (schedaDocumento.registro != null && schedaDocumento.registro.systemId!=null && schedaDocumento.tipoProto != null && !schedaDocumento.tipoProto.Equals("G"))
            {
				setStatoReg(schedaDocumento.registro);
				//lbl_registri.Text = schedaDocumento.registro.descrizione;
				lbl_registri.Text = schedaDocumento.registro.codRegistro;
				//setto anche il valore della combobox nel caso di documenti provenienti da "Riproponi dati"
				bool trovato = false;
				for (int i=0; i<this.ddl_registri.Items.Count && !trovato; i++)
				{
					if (this.ddl_registri.Items[i].Value == schedaDocumento.registro.systemId)
					{
						this.ddl_registri.SelectedIndex = i;
						trovato = true;
					}
				}

                if (Session["docRiproposto"] != null && (bool)Session["docRiproposto"])
                {
                    if (userRegistri == null)
                    {
                        userRegistri = UserManager.getListaRegistri(this);
                    }
				    setStatoReg(UserManager.getRegistroBySistemId(this, ddl_registri.SelectedValue));
                    setRegistroNoControlloCorrInReg(UserManager.getRegistroBySistemId(this, ddl_registri.SelectedValue));

				    this.Session["RegistroSelezionato"] = ddl_registri.SelectedValue.Trim();
                }
			}

			if(schedaDocumento.protocollo == null ||  schedaDocumento.protocollo.segnatura == null || schedaDocumento.protocollo.segnatura.Equals("")) {
				if(nomeTab.Equals("protocollo")) 
				{
					
					//this.ddl_registri.Visible = true;
					//this.img_statoReg.Visible = true;
					this.lbl_registri.Visible = false;
					//this.icoReg.Visible = true;
				}
				else {
					/*this.ddl_registri.Visible = false;
					this.lbl_registri.Visible = false;
					this.img_statoReg.Visible = false;
					this.icoReg.Visible = false;*/
				}
			} else {
				this.ddl_registri.Visible = false;
				this.lbl_registri.Visible = true;
				this.img_statoReg.Visible = true;
				this.icoReg.Visible = true;
			}
            		
            if (string.IsNullOrEmpty(schedaDocumento.systemId) )
			{
                if (schedaDocumento.repositoryContext == null)
                {
                    // Gestione repository context disabilitata
                    
                    if (nomeTab != "allegati")
                    {
                        if (!nomeTab.Equals("profilo"))
                            // evita il problema del cambio tab ptofilo -proto perdo i dati.
                            this.btn_profilo.Enabled = false;

                        else if (!nomeTab.Equals("protocollo"))
                            // evita il problema del cambio tab ptofilo -proto perdo i dati.
                            this.btn_protocollo.Enabled = false;
                    }
                }
                else
                {
                    this.btn_profilo.Enabled = schedaDocumento.repositoryContext.IsDocumentoGrigio;
                    this.btn_protocollo.Enabled = !this.btn_profilo.Enabled;
                }

                // Gestione allegati prima di salvare il documento:
                // Il tab "Allegati" è abilitato solo se:
                // - è abilitata la gestione dei repository context
                //  oppure
                // - la scheda documento corrente è gestita nell'ambito dei repository context
                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                if (!this.IsDisabledSessionRepositoryContext(infoUtente))
                    this.btn_allegati.Enabled = true;
                else
                    // NB: Nonostante non sia abilitata la gestione dei repository context,
                    // in alcuni contesti il tab allegati può essere abiltato prima ancora di salvare
                    // es. inoltra con allegati, riproponi con copia
                    this.btn_allegati.Enabled = (schedaDocumento.repositoryContext != null);

                this.btn_classifica.Enabled = false;
				this.btn_trasmissioni.Enabled = false;
                this.btn_versioni.Enabled = false;
			}
            else
			{
                bool isAllegato = this.IsDocumentoAllegato();

                this.btn_protocollo.Enabled = (schedaDocumento.protocollo != null && !isAllegato);
                this.btn_classifica.Enabled = !isAllegato;
                this.btn_allegati.Enabled = !isAllegato;
                this.btn_trasmissioni.Enabled = !isAllegato;
            }
            // modifica del 15/05/2009
            if (schedaDocumento.tipoProto != null)
            {
                if (schedaDocumento.tipoProto.Equals("R") || schedaDocumento.tipoProto.Equals("C"))
                {
                    this.btn_allegati.Enabled = false;
                    this.btn_classifica.Enabled = false;
                    this.btn_trasmissioni.Enabled = false;
                    this.btn_versioni.Enabled = true;
                }
            }
            //fine modifica del 15/05/2009
			
            #region	Gestione tasto back
			string mytab  = "";
			
			if(DocumentManager.getMemoriaVisualizzaBack(this) == null)
			{
				this.pnl_cont.Visible=false;
				this.pnl_random.Visible=false;
				if(nomeTab.Equals("protocollo")) 
				{
					this.pnl_cont.Visible=true;
					this.pnl_random.Visible=true;
				}
//				this.btn_BackToQuery.Visible = false;
//				this.pnl_riga.Visible=false;

				if (TrasmManager.getMemoriaVisualizzaBack(this) !=null)
				{
					this.pnl_cont.Visible=true;
//					this.pnl_riga.Visible=true;
//					this.btn_BackToQuery.Visible = true;
					this.pnl_random.Visible=true;
					mytab=TrasmManager.getMemoriaTab(this);
//					this.btn_BackToQuery.Attributes.Add("onclick","javascript:top.principale.document.location='../RicercaTrasm/gestioneRicTrasm.aspx?verso=" + mytab + "&back=true'"); 
				}
			}
			else
			{
				this.pnl_cont.Visible=true;
//				this.pnl_riga.Visible=true;
//				this.btn_BackToQuery.Visible = true;
				this.pnl_random.Visible=true;
				mytab=DocumentManager.getMemoriaTab(this);
//				this.btn_BackToQuery.Attributes.Add("onclick","javascript:top.principale.document.location='../RicercaDoc/gestioneRicDoc.aspx?tab="+mytab+"&back=SI'"); 
			}

			if(FascicoliManager.GetFolderViewTracing(this))
			{
				this.pnl_cont.Visible=true;
//				this.btn_BackToFolder.Visible=true;
//				this.pnl_riga.Visible=true;
				this.pnl_random.Visible=true;
				//this.pnl_random.Visible=true;
//				this.btn_BackToFolder.Attributes.Add("onclick","javascript:top.principale.document.location='../fascicolo/gestioneFasc.aspx?tab=documenti&back=Y'");
			}
			
			#endregion

			
			DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);
			IframeTabs.NavigateTo="doc"+nomeTab+".aspx?isNew="+isNew;

			// Impostazione nome del tab correntemente visualizzato
			this.SetCurrentTabName();

			// Aggiornamento url dell'immagine dei pulsanti del tab 
			this.RefreshTabButtonsImages();
			
			// Aggiornamento numero allegati e versioni nel tooltip dei
			// relativi pulsanti del tab
			this.RefreshTabTooltipObjectCount();

			//abilitazione delle funzioni in base al ruolo
			//qui c'è un problema se l'utente non può fare new protocollo
			//ma fa una ricerca vedendo il dettaglio di un protocollo, su ha un tab protocollo non 
			//attivo, ma il dati sono visibili.
			//UserManager.disabilitaFunzNonAutorizzate(this);

			this.RefreshContext();
		}

		/// <summary>
		/// Verifica se il documento è stato fascicolato
		/// </summary>
		/// <returns></returns>
		private bool IsDocumentoFascicolato()
		{
			bool isFascicolato=false;

			try
			{
				isFascicolato=(schedaDocumento!=null && 
							   Convert.ToInt32(schedaDocumento.fascicolato) > 0);
			}
			catch
			{
			}
			
			return isFascicolato;
		}

		/// <summary>
		/// Verifica se il documento contiene allegati
		/// </summary>
		/// <returns></returns>
		private bool ContainsAllegati()
		{
			return (schedaDocumento!=null && 
					schedaDocumento.allegati!=null && 
					schedaDocumento.allegati.Length > 0);
		}

		/// <summary>
		/// Verifica se il documento contiene più di una versione
		/// </summary>
		/// <returns></returns>
		private bool ContainsVersioni()
		{
			return (schedaDocumento!=null && 
					schedaDocumento.documenti!=null && 
					schedaDocumento.documenti.Length > 1);
		}

		/// <summary>
		/// Verifica se il documento contiene trasmissioni
		/// </summary>
		/// <returns></returns>
		private bool ContainsTrasmissioni()
		{
			bool retValue=false;

			if (schedaDocumento!=null)
			{
				try
				{
					int idProfile=Convert.ToInt32(schedaDocumento.systemId);

					if (idProfile!=0)
					{
						DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();
						retValue=(ws.DocumentoGetCountTrasmissioniDocumento(idProfile)>0);
					}
				}
				catch
				{
				}
			}
			
			return retValue;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool IsDocumentoAllegato()
        {
            return (this.schedaDocumento != null && this.schedaDocumento.documentoPrincipale != null);
        }

		/// <summary>
		/// Aggiornamento url dell'immagine dei pulsanti del tab
		/// </summary>
		private void RefreshTabButtonsImages()
		{ 
			// Impostazione immagine non selezionata per tutti i pulsanti
			this.btn_profilo.ImageUrl=this.GetTabButtonImageUrl(this.btn_profilo,false);
			this.btn_protocollo.ImageUrl=this.GetTabButtonImageUrl(this.btn_protocollo,false);
			this.btn_classifica.ImageUrl=this.GetTabButtonImageUrl(this.btn_classifica,false);
			this.btn_allegati.ImageUrl=this.GetTabButtonImageUrl(this.btn_allegati,false);
			this.btn_versioni.ImageUrl=this.GetTabButtonImageUrl(this.btn_versioni,false);
			this.btn_trasmissioni.ImageUrl=this.GetTabButtonImageUrl(this.btn_trasmissioni,false);

			string selectedButtonID=this.ViewState["ID_Butt_precedente"] as string;
			if (selectedButtonID!=null)
			{
				// Impostazione immagine selezionata per il pulsante selezionato
				ImageButton selectedButton=((ImageButton) Page.FindControl(selectedButtonID));
				selectedButton.ImageUrl=this.GetTabButtonImageUrl(selectedButton,true);
			}
		}

		/// <summary>
		/// Reperimento dell'url dell'immagine da associare al pulsante del tab,
		/// a seconda dello stato del documento corrente
		/// </summary>
		/// <param name="imageButton"></param>
		/// <param name="selectedImage">indica se reperire l'immagine per lo stato selezionato o deselezionato</param>
		/// <returns></returns>
		private string GetTabButtonImageUrl(ImageButton imageButton,bool selectedImage)
		{
			string retValue=string.Empty;

			string buttonID=imageButton.ID;
			buttonID=buttonID.Substring(buttonID.IndexOf("_") + 1);

			bool contain=false;

			switch (buttonID)
			{		
				case "classifica":
					contain=this.IsDocumentoFascicolato();
					break;
				case "allegati":
					contain=this.ContainsAllegati();
					break;
				case "versioni":
					contain=this.ContainsVersioni();
					break;
				case "trasmissioni":
					contain=this.ContainsTrasmissioni();
					break;
			}

			string enabledImageUrl=string.Empty;

			if (selectedImage)
				retValue="_attivo";
			else
				retValue="_nonattivo";

			if (contain)
				retValue="_presente" + retValue;

            return "~/App_Themes/" + this.Page.Theme + "/" + buttonID + retValue + ".gif";
            //return "~/App_Themes/" + this.Page.Theme + "/" + (DocsPaWebCtrlLibrary.ImageButton) imageButton.Thema + this.SkinID + retValue + ".gif";
		}

		private void SetTabButtonsImage(DocsPaWebCtrlLibrary.ImageButton button,
										string enabledImageUrl,
										string disabledImageUrl)
		{
			button.ImageUrl=enabledImageUrl;
			button.DisabledUrl=disabledImageUrl;
		}


//		private void SetImagesTabButtonClassifica(bool isFascicolato)
//		{
//			string enabledImageName="classifica_attivo";
//			string disabledImageName="classifica_nonattivo";
//
//			if (isFascicolato)
//			{
//				enabledImageName="classifica_presente_attivo";
//				disabledImageName="classifica_presente_nonattivo";
//			}					   
//
//			this.SetImagesTabButton(this.btn_classifica,"../images/" + enabledImageName + ".gif",
//									"images/" + disabledImageName + ".gif");
//		}
//
//		private void SetImagesTabButtonAllegati(bool containsAllegati)
//		{
//			string enabledImageName="allegati_attivo";
//			string disabledImageName="allegati_nonattivo";
//
//			if (containsAllegati)
//			{
//				enabledImageName="allegati_presenti_attivo";
//				disabledImageName="allegati_presenti_nonattivo";
//			}					   
//
//			this.SetImagesTabButton(this.btn_allegati,"../images/" + enabledImageName + ".gif",
//				"images/" + disabledImageName + ".gif");
//		}


		/// <summary>
		/// Aggiornamento numero allegati e versioni nel tooltip
		/// </summary>
		private void RefreshTabTooltipObjectCount()
		{
			if (schedaDocumento!=null)
			{
				string text=string.Empty;
				int count=0;
				
				if (schedaDocumento.allegati!=null)
					count=schedaDocumento.allegati.Length;

				if (count==1)
					text="allegato";
				else
					text="allegati";

				this.btn_allegati.ToolTip=count.ToString() + " " + text;

				count=0;

				if (schedaDocumento.documenti!=null)
					count=schedaDocumento.documenti.Length;

				if (count==1)
					text="versione";
				else
					text="versioni";

				this.btn_versioni.ToolTip=count.ToString() + " " + text;
			}			
		}

		/// <summary>
		/// Impostazione, in un campo nascosto, del nome del tab correntemente visualizzato
		/// </summary>
		private void SetCurrentTabName()
		{
			this.hd_currentTabName.Value=nomeTab;
		}

		/// <summary>
		/// Reperimento, dal campo nascosto, del nome del tab correntemente visualizzato
		/// </summary>
		/// <returns></returns>
		private string GetCurrentTabName()
		{
			return this.hd_currentTabName.Value;
		}

        /// <summary>
        /// 
        /// </summary>
        protected bool IsDisabledSessionRepositoryContext(DocsPaWR.InfoUtente infoUtente)
        {
            bool retValue = false;

            if (this.ViewState["IsDisabledSessionRepositoryContext"] == null)
            {
                retValue = DocumentManager.IsDisabledSessionRepositoryContext(infoUtente);
                this.ViewState["IsDisabledSessionRepositoryContext"] = retValue;
            }
            else
                retValue = Convert.ToBoolean(this.ViewState["IsDisabledSessionRepositoryContext"]);

            return retValue;
        }
    }
}
