using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.IO;

namespace DocsPaWS
{
	/// <summary>
	/// Web Services per protocollo ASL (Avelco Italia)
	/// </summary>
	[WebService(Namespace="http://www.etnoteam.it/docspawsprotoasl/")]
	public class DocsPaWSProtoASL : System.Web.Services.WebService
	{

		public DocsPaWSProtoASL()
		{
			InitializeComponent();
		}

		#region Component Designer generated code
		
		private IContainer components = null;
				
		private void InitializeComponent()
		{
		}

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
		/// Metodo per la produzione di file XML per ASL
		/// </summary>
		/// <returns>
		/// Parametri di return:
		/// Il metodo torna un tipo String che può assumere i seguenti valori
		/// (0) - Non ci sono dati da produrre
		/// (1) - E' stato prodotto un nuovo file XML
		/// (-1)- E' stato generato un errore nella procedura
		/// </returns>
		[WebMethod]
		public  virtual string buildXmlData()
		{
			BusinessLogic.report.ProtoASL.ReportXML asl = new BusinessLogic.report.ProtoASL.ReportXML(); 

			return asl.BuildXMLReport(this.Server.MapPath("XML/protocolloASL.xsd"));	
		}


	}
}
