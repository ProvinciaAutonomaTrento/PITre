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
using Microsoft.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for dettagliFirmaTree.
	/// </summary>
    public class dettagliFirmaTree : DocsPAWA.CssPage
	{
		protected Microsoft.Web.UI.WebControls.TreeView trvDettagliFirma;
	
		private const string BLANK_PAGE="../blank_Page.htm";
		private const string DETAIL_PAGE="dettagliFirmaDetail.aspx";
		private const string TARGET="right";
		private const string ITEM_NOT_DEFINED="NON PRESENTE";

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				this.BuildTreeView();

				string initTargetPage=string.Empty;

				if (this.trvDettagliFirma.Nodes.Count>0)
					initTargetPage=this.trvDettagliFirma.Nodes[0].NavigateUrl;
				else
					initTargetPage=BLANK_PAGE;

				Response.Write("<script>parent.right.location='" + initTargetPage + "';</script>");
			}
		}

		private FileDocumento GetSignedDocumentFromSession() 
		{
			bool loadSignedDocFromSession=this.LoadSignedDocumentFromSession();
			DocsPaWR.FileDocumento signedDocument=null;

			if (!loadSignedDocFromSession)
			{
				// Reperimento documento firmato da backend
				DocumentManager.RemoveSignedDocument();

				DocsPaWR.FileRequest fileRequest=FileManager.getSelectedFile(this);
	
				if (fileRequest!=null && fileRequest.fileName!=null && fileRequest.fileName!="") 
				{
					DocsPaWR.InfoUtente infoUtente=UserManager.getInfoUtente(this);

					DocsPaWR.DocsPaWebService docsPaWS=new DocsPAWA.DocsPaWR.DocsPaWebService();
					signedDocument=docsPaWS.DocumentoGetFile(fileRequest,infoUtente);
					docsPaWS=null;
				
					DocumentManager.SetSignedDocument(signedDocument);
				}
			}
			else
			{
				// Reperimento documento firmato da session
				signedDocument=DocumentManager.GetSignedDocument();
				Session["docToSign"]=signedDocument;
			}

			return signedDocument;
		}

		/// <summary>
		/// Reperimento parametro in query string per verificare la modalità
		/// con la quale è stato richiesto il servizio di visualizzazione 
		/// dettagli file firmato:
		/// - se "SIGNED_DOCUMENT_ON_SESSION" è presente ed è a "true",
		///   il file firmato (oggetto "DocsPaWR.FileDocumento")
		///   è reperito direttamente dalla session mediante il metodo 
		///   "DocumentManager.GetSignedDocument()"
		/// - se non presente o è impostato a "false", il file firmato
		///   viene reperito direttamente da backend (webmethod "docsPaWS.DocumentoGetFile")
		///   mediante un oggetto di tipo "DocsPaWR.FileRequest"
		/// </summary>
		/// <returns></returns>
		private bool LoadSignedDocumentFromSession()
		{
			bool retValue=false;
			string reqArgs=Request.QueryString["SIGNED_DOCUMENT_ON_SESSION"];
			
			if (reqArgs!=null && !reqArgs.Equals(string.Empty))
                retValue=Convert.ToBoolean(reqArgs);

			return retValue;
		}

		private void BuildTreeView()
		{	
			FileDocumento signedDocument=this.GetSignedDocumentFromSession();
			
			VerifySignatureResult signatureResult=signedDocument.signatureResult;

			if (signatureResult!=null)
			{
				//this.trvDettagliFirma.Nodes.Add(this.GetNodeCheckDocumento(signatureResult));

				int documentIndex=0;

				Microsoft.Web.UI.WebControls.TreeNode documentNode=null;
                Microsoft.Web.UI.WebControls.TreeNode parentNode = null;
                Microsoft.Web.UI.WebControls.TreeNode originalDocument = null;

				foreach (PKCS7Document document in signatureResult.PKCS7Documents)
				{
					// Aggiunta del nodo solo se il documento è p7m
					documentNode=this.GetNodePKCS7Document(document,documentIndex);
					
					if (documentIndex==0)
						this.trvDettagliFirma.Nodes.Add(documentNode);							
					else
						parentNode.Nodes.Add(documentNode);

					parentNode=documentNode;

					documentIndex++;				
				}

				// Aggiunta del nodo relativo al file originale
				originalDocument=this.GetNodeDocumentoOriginale(signedDocument);

				if (parentNode!=null)
					parentNode.Nodes.Add(originalDocument);
				else
					this.trvDettagliFirma.Nodes.Add(originalDocument);


                //Questo Codice commentato visualizza la marca temporale sulla ROOT del certificato
                //Per visualizzarla è necessario decommentarla
                /*
                if (signatureResult.tsInfo != null)
                {
                    int idx = 0;
                    foreach (TSInfo ts in signatureResult.tsInfo)
                    {
                        Microsoft.Web.UI.WebControls.TreeNode tsNode = new Microsoft.Web.UI.WebControls.TreeNode();
                        tsNode.NavigateUrl = DETAIL_PAGE + "?type=timestamp";
                        tsNode.Target = TARGET;
                        string tsText = "Marca Temporale";
                        tsNode.Text = tsText;
                        this.trvDettagliFirma.Nodes.Add(tsNode);
                    }
                }
                */

			}
			else
			{
				// Il documento non è firmato digitalmente
				this.trvDettagliFirma.Nodes.Add(this.GetNodeDocumentoOriginale(signedDocument));

			}
		}

		private Microsoft.Web.UI.WebControls.TreeNode GetNodeCheckDocumento(VerifySignatureResult signatureResult)
		{
			Microsoft.Web.UI.WebControls.TreeNode node=new Microsoft.Web.UI.WebControls.TreeNode();
			node.NavigateUrl=DETAIL_PAGE + "?type=signatureResult";
			node.Target="right";
			node.Text="Risultato verifica";	
			return node;
		}

		private Microsoft.Web.UI.WebControls.TreeNode GetNodeDocumentoOriginale(FileDocumento originalDocument)
		{			 
			bool isSignedDocument=(originalDocument.signatureResult!=null);

			Microsoft.Web.UI.WebControls.TreeNode node=new Microsoft.Web.UI.WebControls.TreeNode();
			node.NavigateUrl=DETAIL_PAGE + "?type=originalDocument";
			node.Target="right";
			node.Text="Documento originale";

			string documentFileName=string.Empty;
			if (isSignedDocument)
				documentFileName=originalDocument.signatureResult.FinalDocumentName;
			else
				documentFileName=originalDocument.name;

			System.IO.FileInfo info=new System.IO.FileInfo(documentFileName);
			string extFileOrignale=info.Extension;
			if (extFileOrignale!="")
				extFileOrignale=extFileOrignale.Replace(".","").ToLower();
			info=null;

			string imageUrl=FileManager.getFileIcon(this,extFileOrignale);
			node.ImageUrl=imageUrl;
			node.SelectedImageUrl=imageUrl;

			return node;
		}

		private Microsoft.Web.UI.WebControls.TreeNode GetNodePKCS7Document(PKCS7Document document,int documentIndex)
		{
			Microsoft.Web.UI.WebControls.TreeNode node=new Microsoft.Web.UI.WebControls.TreeNode();
			node.NavigateUrl=DETAIL_PAGE + "?type=pk7mDocument&documentIndex=" + documentIndex.ToString();
			node.Target="right";
            node.Text = String.Format("Documento firmato"/*, document.SignatureType.ToString()*/); ;

			string imageUrl=FileManager.getFileIcon(this,"p7m");
            
            if (document.SignatureType ==  SignType.PADES )
                imageUrl = FileManager.getFileIcon(this, "pdf");

			node.ImageUrl=imageUrl;
			node.SelectedImageUrl=imageUrl;
			
//			TreeNode folderNode=new TreeNode();
//			folderNode.Text="Altre informazioni";

			// Aggiunta nodi relativi alle firme digitali
			node.Nodes.Add(this.GetNodesFirmeDigitali(document.SignersInfo,documentIndex,null));

			// Aggiunta nodi relativi ai certificati
//			folderNode.Nodes.Add(this.GetNodesCertificati(document.SignersInfo,documentIndex));

			//node.Nodes.Add(node);

			return node;
		}

		private Microsoft.Web.UI.WebControls.TreeNode GetNodesFirmeDigitali(SignerInfo[] signersInfo, int documentIndex, string signerLevel)
		{
			Microsoft.Web.UI.WebControls.TreeNode rootNode=new Microsoft.Web.UI.WebControls.TreeNode();
			rootNode.Text="Firme (" + signersInfo.Length + ")";
			int index=0;

			foreach (SignerInfo info in signersInfo)
			{
				Microsoft.Web.UI.WebControls.TreeNode signNode=new Microsoft.Web.UI.WebControls.TreeNode();
				signNode.NavigateUrl=DETAIL_PAGE + "?type=sign&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString()+ signerLevel;
				signNode.Target=TARGET;
				
				string nodeText=info.SubjectInfo.Cognome + " " + info.SubjectInfo.Nome;
				if (nodeText.Trim()==string.Empty)
					nodeText=ITEM_NOT_DEFINED;

				signNode.Text=nodeText;

                if (info.SignatureTimeStampInfo != null)
                {
                    foreach (TSInfo ts in info.SignatureTimeStampInfo)
                    {
                        Microsoft.Web.UI.WebControls.TreeNode tsNode = new Microsoft.Web.UI.WebControls.TreeNode();
                        tsNode.NavigateUrl = DETAIL_PAGE + "?type=timestamp&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString()+ signerLevel;
                        tsNode.Target = TARGET;
                        string tsText = "Marca Temporale";
                        tsNode.Text = tsText;
                        signNode.Nodes.Add(tsNode);
                    }
                }

                if (info.counterSignatures != null)
                {
                    int signLevels = 0;
                    foreach (SignerInfo csigner in info.counterSignatures)
                    {
                        List<SignerInfo> tmpLst = new List<SignerInfo>();
                        tmpLst.Add(csigner);
                        signerLevel = ":" + signLevels.ToString();
                        signNode.Nodes.Add(GetNodesFirmeDigitali(tmpLst.ToArray(), documentIndex, signerLevel));
                        signLevels++;
                    }
                }

                rootNode.Nodes.Add(signNode);
                signNode=null;

				index++;
			}

			return rootNode;
		}
		
		private Microsoft.Web.UI.WebControls.TreeNode GetNodesCertificati(SignerInfo[] signersInfo, int documentIndex)
		{
			Microsoft.Web.UI.WebControls.TreeNode rootNode=new Microsoft.Web.UI.WebControls.TreeNode();
			rootNode.Text="Certificati (" + signersInfo.Length + ")";

			int index=0;

			foreach (SignerInfo info in signersInfo)
			{
				Microsoft.Web.UI.WebControls.TreeNode signNode=new Microsoft.Web.UI.WebControls.TreeNode();
				signNode.NavigateUrl=DETAIL_PAGE + "?type=certificate&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString();
				signNode.Target=TARGET;
				signNode.Text=info.SubjectInfo.Cognome + " " + info.SubjectInfo.Nome;
				rootNode.Nodes.Add(signNode);
				signNode=null;

				index++;
			}

			return rootNode;
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
