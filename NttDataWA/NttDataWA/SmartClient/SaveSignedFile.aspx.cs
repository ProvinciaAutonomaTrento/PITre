using System;
using System.Linq;
using System.Collections.Generic;
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
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using Newtonsoft.Json;

namespace NttDataWA.SmartClient
{
	/// <summary>
	/// Summary description for SaveSignedFile.
	/// </summary>
	public class SaveSignedFile : Page
	{
		protected System.Web.UI.WebControls.Button Button1;
		protected string tipofirma;
		protected bool firmabool;
        
		private void Page_Load(object sender, System.EventArgs e)
		{
            bool retValue = false;
            bool isContent = false;
            bool issocket = false;
            string idDocument = string.Empty;
            string base64content = string.Empty;
			// Put user code to initialize the page here
			try
			{
				this.startUp();
				this.ErrorPage="";
				if(Request.QueryString["tipofirma"]!=null)
					if(!Request.QueryString["tipofirma"].Equals(""))
						tipofirma = Request.QueryString["tipofirma"];
				if(tipofirma.Equals("cosign"))
					firmabool = true;
				else 
					firmabool = false;


                if (Request.QueryString["iscontent"] != null)
                     bool.TryParse(Request.QueryString["iscontent"],out isContent);

                if (Request.QueryString["issocket"] != null)
                    bool.TryParse(Request.QueryString["issocket"], out issocket);

                if (Request.QueryString["idDocumento"] != null)
                    idDocument = Request.QueryString["idDocumento"].ToString();

                byte[] ba = null;

                if (!issocket)
                    ba = Request.BinaryRead(Request.ContentLength);
                else
                {
                    string contentFile = Request["contentFile"];
                    //Stream stream=Request.InputStream;
                    if (!String.IsNullOrEmpty(contentFile))
                    {
                        contentFile = contentFile.Replace(' ', '+');
                        contentFile = contentFile.Trim();
                        NttDataWA.Utils.FileJSON file = JsonConvert.DeserializeObject<NttDataWA.Utils.FileJSON>(contentFile);
                        ba = Convert.FromBase64String(file.content);
                    }
                }
				DocsPaWR.DocsPaWebService DocsPaWS = ProxyManager.GetWS();
				DocsPaWR.FileDocumento fd=new NttDataWA.DocsPaWR.FileDocumento();
				//fd.content=ba;
				
				ASCIIEncoding ae = new ASCIIEncoding();
                if (!isContent)
                    base64content = ae.GetString(ba);

                if (UIManager.UserManager.getBoolDocSalva(this) == null)
				{
					if(!IsPostBack )
					{
					//	string prova=base64content.Replace(base64content.Substring(4,2),"Ds");
                        FileRequest fr = null;
                        DocsPaWR.SchedaDocumento schedaDocumento = null;

                        if (string.IsNullOrEmpty(idDocument))
                            fr = UIManager.FileManager.getSelectedFile();
                        else
                        {
                            fr = UIManager.FileManager.getSelectedMassSignature(idDocument).fileRequest;
                            FirmaDigitaleMng mmg = new FirmaDigitaleMng();
                            schedaDocumento = mmg.GetSchedaDocumento(idDocument);
                        }

                        if (!string.IsNullOrEmpty(Request.QueryString["signedAsPdf"]))
                        {
                            // Se il file è in formato pdf, viene modificato il nome del file
                            bool signedAsPdf;
                            bool.TryParse(Request.QueryString["signedAsPdf"], out signedAsPdf);
                            if (signedAsPdf)
                                fr.fileName += ".PdF_convertito"; //SERVE PER IL BACKEND NON MODIFICARLO!!!  
                        }

						fr.dataInserimento=DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

						if (fr.GetType().Equals(typeof(DocsPaWR.Allegato)))
						{
							// Creazione di un nuovo allegato per il file firmato
							DocsPaWR.Allegato currentAllegato=(DocsPaWR.Allegato) fr;

							DocsPaWR.Allegato newAllegato=new DocsPaWR.Allegato();
							newAllegato.descrizione=currentAllegato.descrizione;
							newAllegato.numeroPagine=currentAllegato.numeroPagine;
                            newAllegato.fileName = fr.fileName;
							newAllegato.firmatari=currentAllegato.firmatari;
							newAllegato.docNumber=currentAllegato.docNumber;
							newAllegato.version="0";
                            newAllegato.cartaceo = false;
                            newAllegato.repositoryContext = UIManager.DocumentManager.GetSelectedAttachment().repositoryContext;

							fr=newAllegato;
						}

                        if (isContent)
                            retValue = DocsPaWS.AppendContentFirmato(ba, firmabool, ref fr, UIManager.UserManager.GetInfoUser());
                        else
                            retValue = DocsPaWS.AppendDocumentoFirmato(base64content, firmabool, ref fr, UIManager.UserManager.GetInfoUser());
						
						if(!retValue)
						{				
							throw new Exception();
						}

                        UIManager.FileManager.setSelectedFile(fr);
                        if (schedaDocumento==null)
                            schedaDocumento = UIManager.DocumentManager.getSelectedRecord();

                        List<DocsPaWR.Allegato> attachments = new List<DocsPaWR.Allegato>(schedaDocumento.allegati);

                        if (UIManager.DocumentManager.getSelectedAttachId() != null)
                        {
                            //attachments.Add((Allegato)fr);
                            //schedaDocumento.allegati = attachments.ToArray();
                            int index = schedaDocumento.allegati.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.versionId.Equals(UIManager.DocumentManager.GetSelectedAttachment().versionId)).index;
                            Allegato a = new Allegato();
                            a.applicazione = fr.applicazione;
                            a.daAggiornareFirmatari = fr.daAggiornareFirmatari;
                            a.dataInserimento = fr.dataInserimento;
                            a.descrizione = fr.descrizione;
                            a.docNumber = fr.docNumber;
                            a.docServerLoc = fr.docServerLoc;
                            a.fileName = fr.fileName;
                            a.fileSize = fr.fileSize;
                            a.firmatari = fr.firmatari;
                            a.firmato = fr.firmato;
                            a.idPeople = fr.idPeople;
                            a.path = fr.path;
                            a.subVersion = fr.version;
                            a.version = fr.version;
                            a.versionId = fr.versionId;
                            a.versionLabel = schedaDocumento.allegati[index].versionLabel;
                            a.cartaceo = fr.cartaceo;
                            a.repositoryContext = fr.repositoryContext;
                            a.TypeAttachment = 1;
                            //a.numeroPagine = (fr as Allegato).numeroPagine;
                            // modifica necessaria per FILENET (A.B.)
                            if ((fr.fNversionId != null) && (fr.fNversionId != ""))
                                a.fNversionId = fr.fNversionId;
                            schedaDocumento.allegati[index] = a;

                            UIManager.DocumentManager.setSelectedAttachId(fr.versionId);
                            UIManager.DocumentManager.setSelectedNumberVersion(a.version);
                            //schedaDocumento.allegati = UIManager.DocumentManager.AddAttachment((Allegato)fr);
                        }
                        else
                        {
                            //fr = UIManager.DocumentManager.AddVersion(fr, false);
                            schedaDocumento.documenti = UIManager.DocumentManager.addVersion(schedaDocumento.documenti, (Documento)fr);
                            UIManager.DocumentManager.setSelectedNumberVersion(fr.version);
                        }
                        
                        UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                        UIManager.UserManager.setBoolDocSalva(this, "salvato");
					}  
				}
			}     
			catch(Exception es) 
			{	
				ErrorManager.setError(this, es);
			}
		}

        private void startUp()
        {
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1;
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
