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


namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Summary description for EsitoRicercaDocumenti.
	/// </summary>
	public class EsitoRicercaDocumenti : SessionWebPage
	{
		protected Button btnBack;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
			{	
				SearchProperties searchProperties=RicercaDocumentiHandler.CurrentFilter;

                //// Caricamento documenti grigi
                //ListaDocumenti lista=GetControlListaDocumentiGrigi();
                //lista.Visible=(searchProperties.DocumentiGrigi);
                //if (lista.Visible)
                //    lista.Fetch();

                //// Caricamento documenti protocollati
                //lista = GetControlListaDocumentiProtocollati();
                //lista.Visible = (searchProperties.ProtocolliArrivo || searchProperties.ProtocolliInterni || searchProperties.ProtocolliPartenza);
                //if (lista.Visible)
                //    lista.Fetch();

                // Caricamento documenti unificati
                ListaDocumenti lista = GetControlListaDocumentiUnificati();
                lista.Visible = (searchProperties.ProtocolliArrivo || searchProperties.ProtocolliInterni || searchProperties.ProtocolliPartenza || searchProperties.DocumentiGrigi);
                if (lista.Visible)
                    lista.Fetch();
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
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Reperimento controllo lista documenti grigi
		/// </summary>
		/// <returns></returns>
        //private ListaDocumenti GetControlListaDocumentiGrigi()
        //{
        //    return this.FindControl("listaDocumentiGrigi") as ListaDocumenti;
        //}

		/// <summary>
		/// Reperimento controllo lista documenti protocollati
		/// </summary>
		/// <returns></returns>
        //private ListaDocumenti GetControlListaDocumentiProtocollati()
        //{
        //    return this.FindControl("listaDocumentiProtocollati") as ListaDocumenti;
        //}

        /// <summary>
        /// Reperimento controllo lista documenti unificati
        /// </summary>
        /// <returns></returns>
        private ListaDocumenti GetControlListaDocumentiUnificati()
        {
            return this.FindControl("listaDocumentiUnificati") as ListaDocumenti;
        }
        
        private void btnBack_Click(object sender, System.EventArgs e)
		{
			Response.Redirect(EnvironmentContext.RootPath + "Ricerca/Documenti.aspx");
		}
	}
}
