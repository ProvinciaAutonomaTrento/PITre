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
using System.Text;
using System.IO;
using log4net;

namespace DocsPAWA.documento
{
	/// <summary>
	/// Summary description for docSalva.
	/// </summary>
	public class docSalva : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button Button1;
		protected string tipofirma;
		protected bool firmabool;
        private ILog logger = LogManager.GetLogger(typeof(docSalva));
	
		private void Page_Load(object sender, System.EventArgs e)
		{
		
			// Put user code to initialize the page here
			try
			{
				Utils.startUp(this);
				this.ErrorPage="";
				this.Button1.Attributes.Add("onclick","window.close();");

                bool useHash = false;

                string signType = this.Request.QueryString["signType"];
                if (!String.IsNullOrEmpty(signType))
                {
                    if (signType == "P" || signType == "C")
                        useHash = true;
                }

                bool coSign = false;
                string cosignStr = this.Request.QueryString["tipoFirma"];
                if (!String.IsNullOrEmpty(cosignStr))
                {
                    if (cosignStr.ToLower().Equals("cosign"))
                        coSign = true;
                }

				if(Request.QueryString["tipofirma"]!=null)
					if(!Request.QueryString["tipofirma"].Equals(""))
						tipofirma = Request.QueryString["tipofirma"];
				if(tipofirma.Equals("cosign"))
					firmabool = true;
				else 
					firmabool = false;
				byte[] ba = Request.BinaryRead(Request.ContentLength);
				DocsPaWR.DocsPaWebService DocsPaWS = ProxyManager.getWS();
				DocsPaWR.FileDocumento fd=new DocsPAWA.DocsPaWR.FileDocumento();
				//fd.content=ba;
				
				ASCIIEncoding ae = new ASCIIEncoding();
                string base64content = ae.GetString(ba);

				if(UserManager.getBoolDocSalva(this)==null)
				{
					if(!IsPostBack )
					{
					//	string prova=base64content.Replace(base64content.Substring(4,2),"Ds");
						DocsPaWR.FileRequest fr=FileManager.getSelectedFile(this);

                        if (!string.IsNullOrEmpty(Request.QueryString["signedAsPdf"]))
                        {
                            // Se il file è in formato pdf, viene modificato il nome del file
                            bool signedAsPdf;
                            bool.TryParse(Request.QueryString["signedAsPdf"], out signedAsPdf);
                            if (signedAsPdf)
                                fr.fileName += ".pdf";
                        }

						fr.dataInserimento=DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

						if (fr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Allegato)))
						{
							// Creazione di un nuovo allegato per il file firmato
							DocsPaWR.Allegato currentAllegato=(DocsPAWA.DocsPaWR.Allegato) fr;

							DocsPaWR.Allegato newAllegato=new DocsPAWA.DocsPaWR.Allegato();
							newAllegato.descrizione=currentAllegato.descrizione;
							newAllegato.numeroPagine=currentAllegato.numeroPagine;
                            newAllegato.fileName = fr.fileName;
							newAllegato.firmatari=currentAllegato.firmatari;
							newAllegato.docNumber=currentAllegato.docNumber;
							newAllegato.version="0";
                            newAllegato.repositoryContext = DocumentManager.getDocumentoInLavorazione().repositoryContext;

							fr=newAllegato;
						}

                        bool retValue=false;
                        if (!useHash)
						    retValue= DocsPaWS.AppendDocumentoFirmato(base64content,firmabool,ref fr,UserManager.getInfoUtente(this));
                        else
                        {
                            bool pades = (signType == "P");
                            DocsPaWR.MassSignature msReq = new DocsPaWR.MassSignature { fileRequest = fr, base64Signature = base64content, signPades = pades , cosign = coSign };
                            DocsPaWR.MassSignature msRet = DocsPaWS.signDocument (msReq,UserManager.getInfoUtente(this));
                            retValue = msRet.result;
                        }
						
						if(!retValue)
						{
							logger.Error("Errore nel Page_Load (docs = NULL)");							
							throw new Exception();
						}
						
						FileManager.setSelectedFile(this,fr);
						DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
						
						if (fr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Allegato)))
							schedaDocumento.allegati=DocumentManager.addAllegato(schedaDocumento.allegati,(DocsPAWA.DocsPaWR.Allegato) fr);
						else
							schedaDocumento.documenti = DocumentManager.addVersione(schedaDocumento.documenti,(DocsPAWA.DocsPaWR.Documento) fr);

						DocumentManager.setDocumentoSelezionato(this,schedaDocumento);				
						UserManager.setBoolDocSalva(this,"salvato");
					}  
				}
			}     
			catch(Exception es) 
			{	
				ErrorManager.setError(this, es);
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
			this.Button1.Click += new System.EventHandler(this.Button1_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Button1_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}
