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

namespace DocsPAWA.fascicoli
{
	/// <summary>
	/// Summary description for gestFasc.
	/// </summary>
	public class gestFasc : System.Web.UI.Page
	{
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_sx;
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_dx;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			Utils.startUp(this);

            Session["Bookmark"] = "RicercaFasc";

			// Impostazione contesto corrente
			this.SetContext();

			string url="ricFasc.aspx";
            string ricADL = Request.QueryString["ricADL"];

            string urlDx = "NewTabSearchResult.aspx";

			string back=this.Request.QueryString["back"];
            

			if (back!=null && back.ToLower()=="true" &&
                FascicoliManager.getFiltroRicFasc(this) != null)
			{
                // Gestione del tasto "back", viene effettuata
                // nuovamente la ricerca e viene visualizzata
                // la pagina di attesa
                this.iFrame_dx.NavigateTo = "../waitingpage.htm";

                url += "?back=true";

                // Tipologia di ricerca classificazione (codice o livello)
                if (this.Request.QueryString["tipoClass"] != null)
                    url += "&tipoClass=" + this.Request.QueryString["tipoClass"];

                // Indice del fascicolo predecentemente selezionato in lista
				if (this.Request.QueryString["fascIndex"]!=null)
					url+="&fascIndex=" + this.Request.QueryString["fascIndex"];
			}
            //else
            //{
            //    if (ricADL != null && ricADL == "1")
            //    {
            //        this.iFrame_dx.NavigateTo = "NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli";
            //        urlDx = "NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli";
            //        ClientScript.RegisterStartupScript(this.GetType(), "lanciaPagDx", "top.principale.iFrame_dx.document.location='" + urlDx + "';", true);
            //    }
            //    else
            //    {
            //        this.iFrame_dx.NavigateTo = "NewTabSearchResult.aspx?tabRes=fascicoli";
            //        urlDx = "NewTabSearchResult.aspx?tabRes=fascicoli";
            //        ClientScript.RegisterStartupScript(this.GetType(), "lanciaPagDx", "top.principale.iFrame_dx.document.location='" + urlDx + "';", true);
            //    }
            //}

            if (ricADL != null && ricADL == "1")
            {
                if (back != null && back.ToLower() == "true" && FascicoliManager.getFiltroRicFasc(this) != null)
                {
                    url += "&ricADL=1";
                }
                else
                {
                    url += "?ricADL=1";
                }
                if (Request.QueryString["gridper"] != string.Empty && Request.QueryString["gridper"] != null)
                {
                    url += "&gridper=" + Request.QueryString["gridper"].ToString();
                }
                if (Request.QueryString["numRes"] != string.Empty && Request.QueryString["numRes"] != null)
                {
                    url += "&numRes=" + Request.QueryString["numRes"].ToString();
                }
                ClientScript.RegisterStartupScript(this.GetType(), "lanciaPag", "top.principale.iFrame_sx.document.location='" + url + "';", true);
                //this.iFrame_sx.NavigateTo = url;
            }
            else
            {
                if (Request.QueryString["gridper"] != string.Empty && Request.QueryString["gridper"] != null)
                {
                    url += "?gridper=" + Request.QueryString["gridper"].ToString();
                    url += "&numRes=" + Request.QueryString["numRes"].ToString();
                }
                ClientScript.RegisterStartupScript(this.GetType(), "lanciaPag", "top.principale.iFrame_sx.document.location='" + url + "';", true);
            }

            if (back != null && back.ToLower() == "true" &&
                FascicoliManager.getFiltroRicFasc(this) != null)
            {
             
            }
            else
            {
                if (Request.QueryString["gridper"] != string.Empty && Request.QueryString["gridper"] != null)
                {

                }
                else
                {
                    if (ricADL != null && ricADL == "1")
                    {
                        this.iFrame_dx.NavigateTo = "NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli";
                        urlDx = "NewTabSearchResult.aspx?ricADL=1&tabRes=fascicoli";
                        ClientScript.RegisterStartupScript(this.GetType(), "lanciaPagDx", "top.principale.iFrame_dx.document.location='" + urlDx + "';", true);
                    }
                    else
                    {
                        this.iFrame_dx.NavigateTo = "NewTabSearchResult.aspx?tabRes=fascicoli";
                        urlDx = "NewTabSearchResult.aspx?tabRes=fascicoli";
                        ClientScript.RegisterStartupScript(this.GetType(), "lanciaPagDx", "top.principale.iFrame_dx.document.location='" + urlDx + "';", true);
                    }
                }
            }

           

            if (!IsPostBack && (string.IsNullOrEmpty(back) || !(back.ToLower() == "true")))
            {
                FascicoliManager.removeFiltroRicFasc(this);
                FascicoliManager.removeMemoriaFiltriRicFasc(this);

                if(String.IsNullOrEmpty(Request["gridPer"]))
                    Session.Remove("itemUsedSearch");
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Impostazione contesto corrente
		/// </summary>
		private void SetContext()
		{   
            string url=DocsPAWA.Utils.getHttpFullPath() + "/ricercaFascicoli/gestioneRicFasc.aspx";

            string contextName = string.Empty;

            string ricAdl = this.Request.QueryString["ricADL"];
            if (!string.IsNullOrEmpty(ricAdl))
            {
                contextName = SiteNavigation.NavigationKeys.RICERCA_FASCICOLI_ADL;
                url += "?ricADL=" + ricAdl;
            }
            else
            {
                contextName = SiteNavigation.NavigationKeys.RICERCA_FASCICOLI;
            }

            SiteNavigation.CallContext newContext = new SiteNavigation.CallContext(contextName, url);
			newContext.ContextFrameName="top.principale";
			
			if (SiteNavigation.CallContextStack.SetCurrentContext(newContext))
				SiteNavigation.NavigationContext.RefreshNavigation();
		}
	}
}
