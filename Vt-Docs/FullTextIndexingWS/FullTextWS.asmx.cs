using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;

namespace DocsPaDocumentale.FullTextSearch
{
	/// <summary>
	/// WebService che espone i servizi per la ricerca FullText
	/// mediante Microsoft Indexing Services
	/// </summary>
	public class FullTextWS : System.Web.Services.WebService
	{
		public FullTextWS()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion


		/// <summary>
		/// Ricerca FullText
		/// </summary>
		/// <param name="textQuery"></param>
		/// <param name="indexCatalogName"></param>
		/// <param name="maxRowCount"></param>
		/// <returns></returns>
		[WebMethod()]
		public FullTextResultInfo[] Search(string textToSearch,string indexCatalogName,int maxRowCount)
		{
			FullTextIndexingServices services=new FullTextIndexingServices();
			
			return services.Search(textToSearch,indexCatalogName,maxRowCount);
		}
	}
}