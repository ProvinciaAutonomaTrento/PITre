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
using log4net;

namespace DocsPAWA
{
	public class testata2 : System.Web.UI.Page
	{
        private ILog logger = LogManager.GetLogger(typeof(testata2));
		protected DocsPaWebCtrlLibrary.ImageButton btn_doc;
		protected DocsPaWebCtrlLibrary.ImageButton btn_fasc;
		protected DocsPaWebCtrlLibrary.ImageButton btn_trasm;
		protected DocsPaWebCtrlLibrary.ImageButton btn_gest;
		protected DocsPaWebCtrlLibrary.ImageButton btn_search;
		protected DocsPaWebCtrlLibrary.ImageButton btn_config;
		protected DocsPaWebCtrlLibrary.ImageButton btn_logout;
		
		protected System.Web.UI.WebControls.ImageButton img_logo;
		
		protected docsPaMenu.DocsPaMenuWC menuDoc;
		protected docsPaMenu.DocsPaMenuWC menuTrasm;
		protected docsPaMenu.DocsPaMenuWC menuFasc;
		protected docsPaMenu.DocsPaMenuWC menuRic;
		protected docsPaMenu.DocsPaMenuWC menuGest;
		protected docsPaMenu.DocsPaMenuWC menuConf;
		
		protected System.Web.UI.HtmlControls.HtmlTableCell mnubar0;
		protected System.Web.UI.HtmlControls.HtmlTableCell mnubar1;
		protected System.Web.UI.HtmlControls.HtmlTableCell mnubar2;
		protected System.Web.UI.HtmlControls.HtmlTableCell mnubar3;
		protected System.Web.UI.HtmlControls.HtmlTableCell mnubar4;
		protected System.Web.UI.HtmlControls.HtmlTableCell mnubar5;
		protected System.Web.UI.HtmlControls.HtmlTableCell mnubar6;
		
		//my var
		protected	DocsPaWR.Ruolo userRuolo;
		protected   DocsPAWA.DocsPaWR.InfoUtente infoUt;
		protected	DocsPaWR.Registro userReg;

		// Solo i menu sono cliccabili, i bottoni no, quindi devono essere sempre enabled=false 
		private void Page_Load(object sender, System.EventArgs e)
		{
			Utils.startUp(this);
			CleanSessionMemoria();
			//rimozione per il back al dettaglio del fascicolo selezionato
//			FascicoliManager.removeFascicoloSelezionato(this);
//			FascicoliManager.removeFolderSelezionato(this);
			FascicoliManager.removeMemoriaFascicoloSelezionato(this);
			FascicoliManager.removeMemoriaFolderSelezionata(this);
			//
			this.menuDoc.MyTableStyle.BackColor=Color.FromName("#959595");
			this.menuDoc.MyCellStyle.BackColor=Color.FromName("#810d06");
			this.menuDoc.MyTableStyle.BorderColor=Color.White;
			this.menuDoc.MyTableStyle.BorderWidth=1;
			this.menuDoc.MenuPosLeft=143;
			this.menuFasc.MyTableStyle.BackColor=Color.FromName("#959595");
			this.menuFasc.MyCellStyle.BackColor=Color.FromName("#810d06");
			this.menuFasc.MyTableStyle.BorderColor=Color.White;
			this.menuFasc.MenuPosLeft=238;
			this.menuFasc.MyTableStyle.BorderWidth=1;
			this.menuTrasm.MyTableStyle.BackColor=Color.FromName("#959595");
			this.menuTrasm.MyCellStyle.BackColor=Color.FromName("#810d06");
			this.menuTrasm.MyTableStyle.BorderColor=Color.White;
			this.menuTrasm.MyTableStyle.BorderWidth=1;
			this.menuTrasm.MenuPosLeft=331;
			this.menuGest.MyTableStyle.BackColor=Color.FromName("#959595");
			this.menuGest.MyCellStyle.BackColor=Color.FromName("#810d06");
			this.menuGest.MyTableStyle.BorderColor=Color.White;
			// se menuTrasm è  visibile
			//this.menuGest.MenuPosLeft=443;
			// se menuTrasm è non visibile
			this.menuGest.MenuPosLeft=331;
			this.menuGest.MyTableStyle.BorderWidth=1;
			this.menuRic.MyTableStyle.BackColor=Color.FromName("#959595");
			this.menuRic.MyCellStyle.BackColor=Color.FromName("#810d06");
			this.menuRic.MyTableStyle.BorderColor=Color.White;
			this.menuRic.MyTableStyle.BorderWidth=1;
			// SE ANCHE IL MENU FASC E MENU TRASM SONO VISIBILI
			//this.menuRic.MenuPosLeft=535;
			// se menuTrasm è non visibile
			//SE ANCHE IL MENU FASC O MENU TRASM E' VISIBILE
			//this.menuRic.MenuPosLeft=425;
			//SE MENU FASC E MNAU TRASM SONO INVISIBILE 
			this.menuRic.MenuPosLeft=238;
			this.menuConf.MyTableStyle.BackColor=Color.FromName("#959595");
			this.menuConf.MyCellStyle.BackColor=Color.FromName("#810d06");
			this.menuConf.MyTableStyle.BorderColor=Color.White;
			this.menuConf.MyTableStyle.BorderWidth=1;
			// se menuTrasm è  visibile
			//this.menuConf.MenuPosLeft=620;
			// se menuTrasm è non visibile
			this.menuConf.MenuPosLeft=420;
			
			//Disabilito tutti i bottoni poi faccio il controllo
			this.btn_doc.Enabled = false;
			this.btn_fasc.Enabled = false;
			this.btn_trasm.Enabled = false;
			this.btn_gest.Enabled = false;
			this.btn_config.Enabled = false;
				
			//Abilito i bottoni in base al ruolo ricoperto dall'utente
			userRuolo=(DocsPAWA.DocsPaWR.Ruolo) Session["userRuolo"];
			
			if(userRuolo!=null)
			{
				enableFunction();
			}
			//per il problema Regione marche menu gestione:
			if(IsPostBack)
			{
				UserManager.disabilitaVociMenuNonAutorizzate(this);
				RidimensionaMenu();

			}
			Page.DataBind();
			//			if(this.Request.Params.Get("__eventTarget")!=null  && !this.Request.Params.Get("__eventTarget").StartsWith("menuGest"))
			//			{
			//				DocumentManager.removeDocumentoSelezionato(this);
			//				DocumentManager.removeDocumentoInLavorazione(this);
			//			}

			

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
			this.btn_logout.Click += new System.Web.UI.ImageClickEventHandler(this.btn_logout_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.On_PreRender);

		}
		#endregion


		public void enableFunction()
		{
			this.img_logo.Attributes.Add("onclick", "ApriFrame('GestioneRuolo.aspx','principale');");
			DocsPaWR.Funzione userFunz;
			for(int j=0;j<userRuolo.funzioni.Length;++j)
			{
				userFunz=((DocsPAWA.DocsPaWR.Funzione)(userRuolo.funzioni[j]));

				switch(userFunz.codice.Trim())
				{
					case "MENU_DOCUMENTI":
						this.btn_doc.Enabled = false;
						this.btn_doc.DisabledUrl= "images/testata/btn_documenti.gif";
						this.mnubar1.Attributes.Add("onClick", "openIt(0);");
						break;	
					case "MENU_FASCICOLI":
						this.btn_fasc.Enabled =false;
						this.btn_fasc.DisabledUrl = "images/testata/btn_fascicoli.gif";
						this.mnubar3.Attributes.Add("onClick", "openIt(1);");
						break;	
					case "MENU_GESTIONE":
						this.btn_gest.Enabled =false;
						this.btn_gest.DisabledUrl = "images/testata/btn_gestione.gif";
						this.mnubar5.Attributes.Add("onClick", "openIt(3);");
						break;	
					case "MENU_TRASMISSIONI":
						this.btn_trasm.Enabled = false;
						this.btn_trasm.DisabledUrl = "images/testata/btn_trasmissioni.gif";
						this.mnubar4.Attributes.Add("onClick", "openIt(2);");
						break;
					case "MENU_OPZIONI":
						this.mnubar6.Attributes.Add("onClick", "openIt(5);");
						this.btn_config.Enabled=false;
						this.btn_config.DisabledUrl = "images/testata/btn_opzioni.gif";
						break;
				}
			}
			//la ricerca è sempre abilitata
			btn_search.Enabled=false;
			btn_search.DisabledUrl="images/testata/btn_ricerca.gif";
			this.mnubar2.Attributes.Add("onClick", "openIt(4);");
		}

		protected void btn_doc_Click(object sender, EventArgs e)
		{
			//rimuovo gli oggetti che sono in sessione relativi al documento
			//			DocumentManager.removeDocumentoSelezionato(this);
			//			DocumentManager.removeDocumentoInLavorazione(this);
			DocumentManager.removeRisultatoRicerca(this);
			//modifica per problemi su tasto_back 23/05/05
			DocumentManager.removeFiltroRicDoc(this);
			DocumentManager.removeDatagridDocumento(this);

			//annullamento variabili di sessione impostate 
			//dalla gestione ricerca fascicoli
			FascicoliManager.SetFolderViewTracing(this,false);
			CleanSessionMemoria();
			this.btn_doc.DisabledUrl= "images/testata/btn_documenti_on.gif";
			
			//se faccio un nuovo doc rimuovo il fascicolo e la folder in sessione
			FascicoliManager.removeFascicoloSelezionato(this);
			FascicoliManager.removeFolderSelezionato(this);
		}

		protected void btn_fasc_Click(object sender, EventArgs e)
		{
			this.btn_fasc.DisabledUrl = "images/testata/btn_fascicoli_on.gif";
			CleanSessionMemoria();
		}

		protected void btn_trasm_Click(object sender,EventArgs e)
		{
			//refresh session elements for trasmric.
			CleanSessionRisultatiRicerca();
			TrasmManager.removeDataTableEff(this);
			TrasmManager.removeDataTableRic(this);
			CleanSessionMemoria();
			
			this.btn_trasm.DisabledUrl = "images/testata/btn_trasmissioni_on.gif";
		}

		protected void btn_gest_Click(object sender, System.EventArgs e)
		{
			int msgRtn;
			System.Web.UI.WebControls.CommandEventArgs ev=(System.Web.UI.WebControls.CommandEventArgs)e;
			try
			{
				if(ev.CommandArgument.Equals("GEST_FAX"))
				{
					DocsPaWR.DocsPaWebService WS = ProxyManager.getWS();
					infoUt = UserManager.getInfoUtente(this);
					userRuolo = UserManager.getRuolo(this);
					userReg	= userRuolo.registri[0];
					msgRtn=WS.FaxProcessaCasella(Utils.getHttpFullPath(this),infoUt,userRuolo,userReg);
					if(msgRtn < 0)
					{
						logger.Error("Errore nella testata (GEST_FAX)");
						throw new Exception();
					}
					switch(msgRtn)
					{
						case 0:
							Response.Write("<script>alert('Nelle caselle Fax controllate,\\nnon risultano nuovi Fax da Processare ')</script>");
							break;
						case 1:
							Response.Write("<script>alert('Trovato "+msgRtn.ToString()+" Fax,\\nconsultare la lista COSE DA FARE per vedere la trasmissione ad esso relativa.')</script>");
							break;
						default:
							Response.Write("<script >alert('Trovati "+msgRtn.ToString()+" Fax,\\nconsultare la lista COSE DA FARE per vedere le trasmissioni ad essi relativa.')</script>");
							break;
					}
				}
				GestManager.removeRegistroSel(this);
				CleanSessionMemoria();
			}
			catch(Exception ex)
			{	
				string f = ex.Message.ToString();
				ErrorManager.redirectToErrorPage(this,ex);
			}

			this.btn_gest.DisabledUrl = "images/testata/btn_gestione_on.gif";
		}

		protected void btn_search_Click(object sender, EventArgs e)
		{
			DocumentManager.removeDocumentoInLavorazione(this);
			TrasmManager.removeDataTableEff(this);
			TrasmManager.removeDataTableRic(this);
			DocumentManager.removeFiltroRicDoc(this);
			DocumentManager.removeDatagridDocumento(this);
			TrasmManager.removeDocTrasmQueryEff(this);
			TrasmManager.removeDocTrasmQueryRic(this);
			#region
			CleanSessionMemoria();
//			DocumentManager.removeMemoriaFiltriRicDoc(this);
//			DocumentManager.removeMemoriaNumPag(this);
//			DocumentManager.removeMemoriaTab(this);
//			DocumentManager.RemoveMemoriaVisualizzaBack(this);
			#endregion

			btn_search.DisabledUrl="images/testata/btn_ricerca_on.gif";
		}
		
		private void btn_logout_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{	
			try
			{
				// gestito da Global.asax.cs > Session_End
				this.Session.Abandon();
			}
			catch(Exception ex) 
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}

			//this.Response.Redirect(System.Configuration.ConfigurationManager.AppSettings.Get("exitPage").ToString());
			if(!this.Page.IsStartupScriptRegistered("alertJS"))
			{					
				string scriptString = "<SCRIPT>window.parent.close();</SCRIPT>";
                this.ClientScript.RegisterStartupScript(this.GetType(), "alertJS", scriptString);
                //this.Page.RegisterStartupScript("alertJS", scriptString);
			}	
		}
	
		private void On_PreRender(object sender,System.EventArgs e)
		{
			//Controllo per disabilitare le funzionalità in base al ruolo scelto
			UserManager.disabilitaVociMenuNonAutorizzate(this);


			RidimensionaMenu();
			//gestione tasto back
			//CleanSessionMemoria();
		}
		
		protected void RidimensionaMenu()
		{
			//Menu Doc
			int[] widthCellDoc = {115,115,90} ;
			int[] widthCellFasc = {110} ;
			int[] widthCellTrasm = {40} ;
			//old:	int[] widthCellGest = {45,115,45,90,100,100,120,45} ;
			//12/09/2005: int[] widthCellGest = {45,115,100,100,100,120,45} ;
			//Modifica Prospetti Riepilogativi
			int[] widthCellGest = {45,120,140,40,40,110,110,60,60,30,125,110};
			//int[] widthCellGest = {45,115,100,100,100,120,45} ;
//			this.menuDoc.WidthTable="0";
//			this.menuFasc.WidthTable="0";
//			this.menuGest.WidthTable="0";
//			this.menuTrasm.WidthTable="0";
			for(int i=0;i<this.menuDoc.Links.Count;i++)
			{
				this.menuDoc.Links[i].WndOpenProprities=" ";
				this.menuDoc.Links[i].WidthCell=widthCellDoc[i];
				if(this.menuDoc.Links[i].Visible)
					this.menuDoc.WidthTable+=widthCellDoc[i];
			}
			//menu Fasc
			for(int i=0;i<this.menuFasc.Links.Count;i++)
			{	
				this.menuFasc.Links[i].WndOpenProprities=" ";
				this.menuFasc.Links[i].WidthCell=widthCellFasc[i];
				if(this.menuFasc.Links[i].Visible)
					this.menuFasc.WidthTable+=widthCellFasc[i];
			}
			//menu trasm
			for(int i=0;i<this.menuTrasm.Links.Count;i++)
			{
				this.menuTrasm.Links[i].WndOpenProprities=" ";
				this.menuTrasm.Links[i].WidthCell=widthCellTrasm[i];
				if(this.menuTrasm.Links[i].Visible)
					this.menuTrasm.WidthTable+=widthCellTrasm[i];
			}
			//menu gest
			for(int i=0;i<this.menuGest.Links.Count;i++)
			{
				this.menuGest.Links[i].WidthCell=widthCellGest[i];
				if(this.menuGest.Links[i].Visible)
				{
					if(this.menuGest.Links[i].Text == "Liste")
					{
						if(System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
						{
							this.menuGest.Links[i].Visible = true;
							this.menuGest.WidthTable+=widthCellGest[i];
						}
						else
						{
							this.menuGest.Links[i].Visible = false;
						}
					}                    
				}
				
			}
		}

		private void CleanSessionRisultatiRicerca()
		{
			TrasmManager.removeDocTrasmSel(this);
			TrasmManager.removeDocTrasmQueryEff(this);
			TrasmManager.removeDataTableRic(this);
			TrasmManager.removeDataTableEff(this);
			TrasmManager.removeDataTableRic(this);
		}

		private void CleanSessionMemoria()
		{
			DocumentManager.removeMemoriaFiltriRicDoc(this);
			DocumentManager.removeMemoriaNumPag(this);
			TrasmManager.removeMemoriaNumPag(this);
			DocumentManager.removeMemoriaTab(this);
			DocumentManager.RemoveMemoriaVisualizzaBack(this);

			FascicoliManager.removeMemoriaRicFasc(this);
			FascicoliManager.RemoveMemoriaVisualizzaBack(this);
			FascicoliManager.SetFolderViewTracing(this, false);
		}		
	}
}
