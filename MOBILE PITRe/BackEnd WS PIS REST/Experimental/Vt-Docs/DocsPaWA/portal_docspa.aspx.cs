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

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for portal_login.
	/// </summary>
	public class portal_docspa : System.Web.UI.Page
	{
		private string dst;
		private string show;
		protected System.Web.UI.WebControls.Label lblError;
		private string userName;
		private string idDoc;
		protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
		protected DocsPAWA.DocsPaWR.Utente userHome;
		protected DocsPAWA.DocsPaWR.InfoUtente Safe;
		protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
		protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
		protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
		protected System.Web.UI.WebControls.Label lblMessage;
		protected DocsPAWA.DocsPaWR.InfoDocumento[] ListaDoc;
		protected DocsPAWA.DocsPaWR.Utente utente;
		protected string inString;
		protected DocsPAWA.DocsPaWR.LoginResult oldLoginResult;
		protected string tipoProto;
 
		private void Page_Load(object sender, System.EventArgs e)
		{
			lblError.Text="";
			lblError.Visible = false;
			
			dst = this.Request.QueryString["dst"];
			userName = this.Request.QueryString["userName"];
			show = this.Request.QueryString["show"];
			
			string message;
			DocsPaWR.LoginResult loginResult;
			
			utente = ExecuteLogin(out message, out loginResult);
			UserManager.setUtente(this,utente);
			//si effettua la ricerca del doc ricercando il ruolo dell'utente che ha la 
			//visibilità su di esso
			if (utente != null)
			{		
				switch (show)
				{
					case "login" :
						// redirect alla pagina di scelta ruolo
						string script="<script>";
						script += "var popup = window.open('index.aspx?FromPortal=true','Index',";
                        // script += "'fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes, scrollbars=auto');";
                        //ie7
                        script += "'location=0,resizable=yes');";
                        script += "popup.moveTo(0,0); popup.resizeTo(screen.availWidth,screen.availHeight);popup.focus();";
						script += " if(popup!=self) {window.opener=null;self.close();}";
						script += "</script>";
						Page.RegisterStartupScript("Index", script);	
						break;
						//redirect alla visualizzazione del documento selezionato
					case "view" :
						/* Viene creata la condizione IN per ricercare i ruoli che hanno visibilità 
						sul documento tra tutti i ruoli a cui l'utente è associato*/
						CreaInString();
						setVisibilitaRuolo();
						tipoProto = DocumentManager.getQueryTipoProto(this,idDoc);
						if (!tipoProto.Equals("R"))
						{
							ricercaOggetto();
						}
						else 
						{
							//per la stampa del registro non è gestito l'acesso dal portale
							string msg = "Per la visualizzazione della stampa di un registro\\n \\n utilizzare l\\'applicativo di protocollo";
							Response.Write("<script>alert('" + msg + "');window.parent.close();</script>");
						}
						break;
				}
				
			}
			else
			{
				lblError.Text = message;
				lblError.Visible=true;
			}

		}

		private DocsPAWA.DocsPaWR.Utente ExecuteLogin(out string message, out DocsPAWA.DocsPaWR.LoginResult loginResult)
		{
			message = "";
			DocsPaWR.UserLogin userLogin;

			DocsPaWR.Utente utente = UserManager.LoginBatch(this.Session.SessionID, userName, dst, 
				out loginResult, out userLogin);
			oldLoginResult = loginResult;

			switch(loginResult)
			{
					// L'utente si è connesso regolarmente
				case DocsPAWA.DocsPaWR.LoginResult.OK:					
					Session["userData"] = utente;
					break;

				case DocsPAWA.DocsPaWR.LoginResult.UNKNOWN_USER:
					message = "Nome o password errati";
					break;

				case DocsPAWA.DocsPaWR.LoginResult.USER_ALREADY_LOGGED_IN:
					string loginMode = null;
					try 
					{
						loginMode = ConfigSettings.getKey(ConfigSettings.KeysENUM.ADMINISTERED_LOGIN_MODE);
					} 
					catch(Exception) {}

					if(loginMode == null || loginMode.ToUpper() == Boolean.TrueString.ToUpper())
					{
						// Gestione tramite tool di amministrazione
						Session["AdminMode"]=true;
						message = "L'utente ha gia' una connessione aperta. Contattare l'amministrazione.";
					}
					else
					{
						Session.Add("loginData", userLogin);
						utente = UserManager.ForcedLogin(this, userLogin ,out loginResult);
					}
					break;
				default: // Application Error
					message = "Errore nella procedura di Login. Contattare l'amministrazione.";
					break;
			}
			
			return utente;
		}


		private void ricercaOggetto()
		{
			string msg;
			
			if(idDoc != null) 
			{
				//Ricerca del documento per DOCNUMBER
				//array contenitore degli array filtro di ricerca
				qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
				qV[0]=new DocsPAWA.DocsPaWR.FiltroRicerca[1];
				
				fVList=new DocsPAWA.DocsPaWR.FiltroRicerca[0];			
					
				// filtro DOCNUMBER
				fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
				fV1.argomento=DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
				fV1.valore = idDoc;
				fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);

				// filtro TIPODOC
				fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
				fV1.argomento=DocsPaWR.FiltriDocumento.TIPO.ToString();
				fV1.valore = tipoProto;
				fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);

				qV[0]=fVList;

				Safe= new DocsPAWA.DocsPaWR.InfoUtente();
				Safe=UserManager.getInfoUtente(this);
				ListaDoc=DocumentManager.getQueryInfoDocumento(Safe.idGruppo, Safe.idPeople,this,qV);
				if (ListaDoc != null && ListaDoc.Length >0)
				{
					DocumentManager.setRisultatoRicerca(this,(DocsPAWA.DocsPaWR.InfoDocumento)ListaDoc[0]);
					Session["tabRicDoc.InfoDoc"]=(DocsPAWA.DocsPaWR.InfoDocumento)ListaDoc[0];
					DocsPaWR.InfoDocumento inf=(DocsPAWA.DocsPaWR.InfoDocumento)ListaDoc[0];
					#region commento
					//					switch(inf.tipoProto.ToUpper())
					//					{
					//						case "A":
					//							tab="protocollo";
					//							break;
					//						case "P":
					//							tab="protocollo";
					//							break;
					//						case "G":
					//							tab="profilo";
					//							break;
					//					}
					//					//si passa alla pagina gestioneDoc.aspx
					//					string newUrl=Utils.getHttpFullPath(this)+"/"+"documento/gestioneDoc.aspx?tab="+tab;
					//					string	scriptString="<script language='javascript'>var wnd=window.open('"+newUrl+"','principale','fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes, scrollbars=auto');if(wnd!=self) {window.opener=null;window.close();}</script>";
					//					if(!this.IsClientScriptBlockRegistered("clientScript"))
					//						this.RegisterClientScriptBlock("clientScript", scriptString);	
					#endregion
					string newUrl=Utils.getHttpFullPath(this)+"/"+"index.aspx"+"?tipoOggetto=" + tipoProto + "&idObj=" + idDoc + "&from=portal" ;
					string scriptString="<script language='javascript'> var wnd=window.open('"+newUrl+"','Index','fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes, scrollbars=auto');wnd.moveTo(0,0);";
					scriptString += " wnd.resizeTo(screen.availWidth,screen.availHeight);wnd.focus();if(wnd!=self) {window.opener = null; window.close();}</script>";	
					Response.Write(scriptString);
				}
				else 
				{
					//il doc non è visibile nè ai ruoli dell'utente nè all'utente stesso
					msg = "Questo utente non ha visibilità sul documento richiesto";
					Response.Write("<script>alert('" + msg + "');window.parent.close();</script>");
				}
			}
			else
			{
				msg = "Dati insufficienti per effettuare la ricerca";
				Response.Write("<script>alert('" + msg + "');window.parent.close();</script>");
			}
		}

		public void CreaInString()
		{
			if (utente.ruoli.Length > 0)
			{
				// calcolo la stringa per la condizione IN
				inString="(";
				for(int l=0;l<utente.ruoli.Length;l++)
				{
					inString=inString + utente.ruoli[l].idGruppo;
					if(l<utente.ruoli.Length-1)
					{
						inString=inString+",";
					}
				}
				inString=inString+")";
			}
		}
		
		public void setVisibilitaRuolo()
		{
			string[] ruoliAut = null; 
			//si ricava il docNumber ricevuto in queryString
			idDoc = this.Request.QueryString["idDoc"];
			//se l'utente ha più di un ruolo estraggo i ruoli che vedono il doc
			if(utente.ruoli.Length > 1)
			{
				ruoliAut = UserManager.getListaVisibilitaRuolo(this,idDoc,inString);
			}
			//se almeno uno dei ruoli dell'utente ha la visibilità sul documento
			if (ruoliAut != null && ruoliAut.Length > 0)
			{
				//se un solo ruolo dell'utente ha visibilità sul documento
				if (ruoliAut.Length == 1)
				{
					//ricerco il ruolo utente con l'idGruppo che ha la visibilità
					//e lo metto in sessione
					userRuolo = findRuolo(utente.ruoli, ruoliAut[0]);
				}
				//se più ruoli dell'utente hanno la visibilità sul documento
				//metto in sessione quello con diritti maggiori
				if (ruoliAut.Length > 1)
				{
					char[] dot={','};
					string[] splitArray = inString.Split(dot);
					userRuolo = findRuolo(utente.ruoli,splitArray[0].Substring(1));
				}

				UserManager.setRuolo(this, userRuolo);
			} 
			else
			{
				//caso i cui: 1) la visibilità del documento è relativa all'utente e non al ruolo
				//2)l'utente ha un solo ruolo
				//In sessione settiamo il ruolo con ACCESSRIGHTS maggiore
				UserManager.setRuolo(this, utente.ruoli[0]);
			}
		}

		public DocsPAWA.DocsPaWR.Ruolo findRuolo(DocsPAWA.DocsPaWR.Ruolo[] ruoliUtente , string idGruppo)
		{
			DocsPaWR.Ruolo result = new DocsPAWA.DocsPaWR.Ruolo();
			foreach (DocsPAWA.DocsPaWR.Ruolo r in ruoliUtente)
			{
				if(r.idGruppo.Equals(idGruppo))
				{
					result = r;
					break;
				}
			}
			return result;
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
