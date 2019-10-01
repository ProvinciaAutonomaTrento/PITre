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
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;
using System.Linq;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for dettagliFirmaDetail.
	/// </summary>
    public class dettagliFirmaDetail : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Table tblSignedDocument;
	
		private const string ITEM_NOT_DEFINED="";

		private string _requestType=string.Empty;
		private int _requestDocumentIndex=0;
		private int _requestSignerIndex=0;

		private FileDocumento _fileDocument=null;
		private VerifySignatureResult _signatureResult=null;

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires=-1;

			this._requestType=(string) Request.QueryString["type"];
			this._requestDocumentIndex=Convert.ToInt32(Request.QueryString["documentIndex"]);
			//this._requestSignerIndex=Convert.ToInt32(Request.QueryString["index"]);

			this._fileDocument=DocumentManager.GetSignedDocument();
			this._signatureResult=this._fileDocument.signatureResult;
			
			// Put user code to initialize the page here
			this.FillTable();
		}

        SignerInfo getSignerInfo(int documentIndex, string sIndex)
        {
            string[] singerIndexes = sIndex.Split(':');
            List<Int32> intLst = new List<int>();

            foreach (string sIdx  in singerIndexes)
            {
                int idx;
                Int32.TryParse(sIdx, out idx);
                intLst.Add (idx);
            }
            

            SignerInfo[] rootSI = _signatureResult.PKCS7Documents[documentIndex].SignersInfo;
            
            if (intLst.Count ==1)
                return rootSI[intLst.FirstOrDefault()];

            //voglio i controfirmatari
            int count = 1;
            foreach (int sIdx in intLst)
            {
                if (count == intLst.Count)
                    break;

                rootSI = rootSI[sIdx].counterSignatures;
                count++;
                
            }

            int singerIndex = intLst.LastOrDefault();
            return rootSI[singerIndex];
        }

		private void FillTable()
		{
			switch (this._requestType)
			{
//				case "signatureResult":
//					this.FillTableSignatureResult();
//					break;

				case "originalDocument":
					this.FillTableOriginalDocument();
					break;

				case "pk7mDocument":
					this.FillTableP7MDocument();
					break;

				case "sign":
					this.FillTableSign();
					break;

                case "certificate":
                    this.FillTableCertificate();
                    break;

                case "timestamp":
                    this.FillTableTimeStamp();
					break;

				case "invalid":
					this.FillTableInvalid();
					break;
			}
		}


		private void FillTableOriginalDocument()
		{
			bool isSignedDocument=(this._signatureResult!=null);

			TableRow row=this.CreateHeaderTableRow();
			row.Cells[0].Text="Dati generali";
			row=null;

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Stato del documento:";

			if (isSignedDocument)			
				row.Cells[2].Text="Firmato digitalmente";
			else
				row.Cells[2].Text="Non firmato digitalmente";

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Tipo documento:";
			row.Cells[2].Text=this._fileDocument.contentType;
			row=null;
			
			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Nome file originale:";
			if (isSignedDocument)
				row.Cells[2].Text=this._signatureResult.FinalDocumentName;
			else
				row.Cells[2].Text=this._fileDocument.name;

			row=null;

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Dimensioni:";
			row.Cells[2].Text=this._fileDocument.length + " KB";
			row=null;

			if (isSignedDocument)
			{
                bool isPades = this._signatureResult.PKCS7Documents[this._requestDocumentIndex].SignAlgorithm.Contains("PADES");
				row=this.CreateStandardTableRow();
                row.Cells[0].Text = "Nome file P7M:";
                row.Cells[2].Text = this._signatureResult.FinalDocumentName;

                if (isPades)
                    row.Cells[0].Text = "Nome file:";
                else
                    row.Cells[2].Text += ".P7M";

				row=null;
			}
		}

		private void FillTableP7MDocument()
		{	
			PKCS7Document document=this._signatureResult.PKCS7Documents[this._requestDocumentIndex];
			
			TableRow row=this.CreateStandardTableRow();
			row.Cells[0].Text="Nome file :";		
			row.Cells[2].Text=document.DocumentFileName;
            if (!document.SignAlgorithm.Contains("PADES"))
            {
                row.Cells[0].Text = "Nome file P7M:";		
                row.Cells[2].Text += ".P7M";
            }
			row=null;

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Livello:";
			row.Cells[2].Text=document.Level.ToString();
			row=null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = "Algoritmo di Firma documento:";
            row.Cells[2].Text = document.SignAlgorithm;
            row = null;
            /*
            if (this._signatureResult.tsInfo != null)
                this.AppendRowsTimeStamp(this._signatureResult.tsInfo);
            */
			document=null;
		}

		private void FillTableSign()
		{
			// Dati controllo
			this.AppendRowsStatus();

			// Dati certificato
            this.AppendSeparatorRow();
			this.AppendRowsCertificate();

			// Dati soggetto
            this.AppendSeparatorRow();
			this.AppendRowsSoggetto();

			// Dati relativi all'algoritmo e firma digitale
            this.AppendSeparatorRow();
			this.AppendRowsDocumentSign();

            
		}

        private void FillTableTimeStamp()
        {
            TSInfo[] tsinfo = null;
            if ((Request.QueryString["documentIndex"] == null) && (Request.QueryString["index"]==null))
            {
                tsinfo = this._signatureResult.DocumentTimeStampInfo;
            }
            else
            {
                //SignerInfo signerInfo = this._signatureResult.PKCS7Documents[this._requestDocumentIndex].SignersInfo[this._requestSignerIndex];
                SignerInfo signerInfo = getSignerInfo(this._requestDocumentIndex, Request.QueryString["index"]);
                tsinfo = signerInfo.SignatureTimeStampInfo;
                
            }
            this.AppendRowsTimeStamp(tsinfo);
        }

		private void FillTableCertificate()
		{
            this.AppendRowsCertificate();
		}

		private void FillTableInvalid()
		{
			TableRow row=this.CreateStandardTableRow();
			row.Cells[0].Text="Status documento:";
			row.Cells[2].Text="Firma non valida";
			row=null;

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Codice errore:";
			row.Cells[2].Text=this._signatureResult.StatusCode.ToString();
			row=null;

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Descrizione errore:";
			row.Cells[2].Text=this._signatureResult.StatusDescription;
			row=null;			
		}

		private void AppendRowsStatus()
		{
			//SignerInfo signerInfo=this._signatureResult.PKCS7Documents[this._requestDocumentIndex].SignersInfo[this._requestSignerIndex];
            SignerInfo signerInfo = getSignerInfo(this._requestDocumentIndex, Request.QueryString["index"]);
			TableRow row=this.CreateHeaderTableRow();
			row.Cells[0].Text="Risultato verifica";

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Stato della firma";

			row.Cells[1].Controls.Add(this.GetStatusFirmaImage(signerInfo!=null));

			// Per verificare la validità della firma,
			// verifica la presenza di almeno un firmatario
            if (signerInfo.CertificateInfo.ThumbPrint != null && signerInfo.CertificateInfo.ThumbPrint != "")
            {
                row.Cells[2].Text = "Valido";
                //controlliamo l'aa
                if (this._signatureResult.StatusCode == -5)
                    row.Cells[2].Text = "Non Conforme CADES (idaasigningcertificateV2 non trovato nella firma)";
            }
            else
                row.Cells[2].Text = "Non valido";

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Stato del certificato";			
			row.Cells[1].Controls.Add(this.GetStatusImage(signerInfo.CertificateInfo.RevocationStatus==0));
			row.Cells[2].Text=signerInfo.CertificateInfo.RevocationStatusDescription;
			row=null;

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Verifica CRL:";
			if (this._signatureResult.CRLOnlineCheck)
				row.Cells[2].Text="In linea";
			else
				row.Cells[2].Text="Locale";
			row=null;
		}

		private void AppendRowsSoggetto()
		{
			//SignerInfo signerInfo=this._signatureResult.PKCS7Documents[this._requestDocumentIndex].SignersInfo[this._requestSignerIndex];
            SignerInfo signerInfo = getSignerInfo(this._requestDocumentIndex, Request.QueryString["index"]);
	
			TableRow row=this.CreateHeaderTableRow();
			row.Cells[0].Text="Soggetto";
			row=null;

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Nome:";
			row.Cells[2].Text=this.GetItemValue(signerInfo.SubjectInfo.Nome);
			row=null;

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Cognome:";
			row.Cells[2].Text=this.GetItemValue(signerInfo.SubjectInfo.Cognome);
			row=null;

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Codice fiscale:";
			row.Cells[2].Text=this.GetItemValue(signerInfo.SubjectInfo.CodiceFiscale);
			row=null;

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Data di nascita:";
			row.Cells[2].Text=this.GetDateValue(signerInfo.SubjectInfo.DataDiNascita);
			row=null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = "Organizzazione:";
            row.Cells[2].Text = this.GetItemValue(signerInfo.SubjectInfo.Organizzazione);
            row = null;
            
			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Ruolo:";
			row.Cells[2].Text=this.GetItemValue(signerInfo.SubjectInfo.Ruolo);
			row=null;

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Paese:";
			row.Cells[2].Text=this.GetItemValue(signerInfo.SubjectInfo.Country);
			row=null;

			row=this.CreateStandardTableRow();
			row.Cells[0].Text="ID titolare:";
			row.Cells[2].Text=this.GetItemValue(signerInfo.SubjectInfo.CertId);
			row=null;

		}

		private void AppendRowsDocumentSign()
		{
            PKCS7Document document = this._signatureResult.PKCS7Documents[this._requestDocumentIndex];
            SignerInfo signerInfo = getSignerInfo(this._requestDocumentIndex, Request.QueryString["index"]);
			CertificateInfo certInfo=signerInfo.CertificateInfo;

            TableRow row = this.CreateHeaderTableRow();
			row.Cells[0].Text="Firma documento";
			row=null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = "Algoritmo di Firma documento:";
            row.Cells[2].Text = signerInfo.SignatureAlgorithm;  //document.SignAlgorithm;
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = "Impronta documento:";
            row.Cells[2].Text = document.SignHash;
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = "Controfirmatario:";
            row.Cells[2].Text = signerInfo.isCountersigner?"Vero":"Falso";
            row = null;

            if (signerInfo.SigningTime != DateTime.MinValue)
            {
                row = this.CreateStandardTableRow();
                row.Cells[0].Text = "Data di firma:";
                row.Cells[2].Text = signerInfo.SigningTime.ToLocalTime().ToString () ;
                row = null;
            }

        
        }

        private void AppendRowsTimeStamp(TSInfo[] tsInfo)
        {
            foreach (TSInfo tsi in tsInfo)
            {
                this.AppendSeparatorRow();
                TableRow row = this.CreateHeaderTableRow();

                row.Cells[0].Text = String.Format("Marca Temporale"/*, tsi.TSType.ToString()*/);
                row = this.CreateStandardTableRow();
                row.Cells[0].Text = "Emessa da:";
                row.Cells[2].Text = tsi.TSANameIssuer;
                row = null;

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = "Autorità emittente:";
                row.Cells[2].Text = tsi.TSANameSubject;
                row = null;

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = "Data Marcatura documento:";
                row.Cells[2].Text = tsi.TSdateTime.ToLocalTime().ToString();
                row = null;

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = "Seriale Marcatura:";
                row.Cells[2].Text = tsi.TSserialNumber;
                row = null;

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = "Impronta Marcatura:";
                row.Cells[2].Text = tsi.TSimprint;
                row = null;

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = "Data Inizio validità Marca:";
                row.Cells[2].Text = tsi.dataInizioValiditaCert.ToLocalTime().ToString();
                row = null;

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = "Data Fine validità Marca:";
                row.Cells[2].Text = tsi.dataFineValiditaCert.ToLocalTime().ToString();
                row = null;

            }

        }
		private void AppendRowsCertificate()
		{
			//SignerInfo signerInfo=this._signatureResult.PKCS7Documents[this._requestDocumentIndex].SignersInfo[this._requestSignerIndex];
            SignerInfo signerInfo = getSignerInfo(this._requestDocumentIndex, Request.QueryString["index"]);
			CertificateInfo certInfo=signerInfo.CertificateInfo;

			TableRow row=this.CreateHeaderTableRow();
			row.Cells[0].Text="Certificato";
			row=null;

            //Mev Firma1 < aggiunta info ente certificatore
            row = this.CreateStandardTableRow();
            row.Cells[0].Text = "Ente Certificatore:";
            row.Cells[2].Text = Utils.getEnteCertificatore(certInfo.IssuerName);
            row = null;
            //>
			row=this.CreateStandardTableRow();
			row.Cells[0].Text="S.N. certificato:";
			row.Cells[2].Text=certInfo.SerialNumber;
			row=null;

            //Mev Firma1 < posizionameto orizzontale delle info date dal - al
			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Valido dal:";
            row.Cells[2].Text = string.Format("{0}&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Valido sino al: {1}", certInfo.ValidFromDate.ToLongDateString(), certInfo.ValidToDate.ToLongDateString()); ;
			row=null;
            //>

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = "Algoritmo Firma Certificato:";
            row.Cells[2].Text = certInfo.SignatureAlgorithm;
            row = null;
            
			row=this.CreateStandardTableRow();
			row.Cells[0].Text="Firmatario:";
			row.Cells[2].Text=this.GetItemValue(signerInfo.SubjectInfo.Cognome + " " + signerInfo.SubjectInfo.Nome);
			row=null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = "Thumbprint Certificato:";
            row.Cells[2].Text = certInfo.ThumbPrint;
            row = null;

			certInfo=null;
		}

        private void AppendSeparatorRow()
        {
            TableRow row = this.CreateStandardTableRow();
            row.Cells[0].Text = "";
            row.Cells[0].BackColor = Color.Gray;
            row.Cells[1].Text = "";
            row.Cells[1].BackColor = Color.Gray;
            row.Cells[2].Text = "";
            row.Cells[2].BackColor = Color.Gray;
            row = null;
        }

		private TableRow CreateStandardTableRow()
		{
			TableRow row=new TableRow();
			this.AppendStandardTableCell(row);
			this.AppendStandardTableCell(row);
			this.AppendStandardTableCell(row);
			this.tblSignedDocument.Rows.Add(row);
			return row;
		}

		private void AppendStandardTableCell(TableRow row)
		{
			TableCell cell=new TableCell();
			cell.Font.Name="verdana";
			cell.Font.Size=FontUnit.Smaller;
			row.Cells.Add(cell);
			cell=null;
		}

		private TableRow CreateHeaderTableRow()
		{
			TableRow row=new TableRow();
			this.AppendHeaderTableCell(row);
			this.tblSignedDocument.Rows.Add(row);
			return row;
		}

		private void AppendHeaderTableCell(TableRow row)
		{
			TableCell cell=new TableCell();
			cell.Font.Name="verdana";
			cell.Font.Bold=true;
			cell.Font.Size=FontUnit.Smaller;
			row.Cells.Add(cell);
			cell=null;
		}

		
		private HtmlImage CreateImage()
		{
			HtmlImage img=new HtmlImage();
			img.ID="ImageStatus";
			img.Width=32;
			img.Height=32;
			img.Align="center";
			return img;
		}

		private HtmlImage GetStatusImage(bool statusValid)
		{
			string imageUrl=@"..\images\tabDocImages\";

			HtmlImage img=this.CreateImage();
			if (statusValid)
				imageUrl += "certificato_valido.gif";
			else
				imageUrl += "certificato_non_valido.gif";
			img.Src=imageUrl;

			return img;
		}

		private HtmlImage GetStatusFirmaImage(bool statusValid)
		{
			string imageUrl=@"..\images\tabDocImages\";

			HtmlImage img=this.CreateImage();
			if (statusValid)
				imageUrl += "firma_valida.gif";
			else
				imageUrl += "firma_non_valida.gif";
			img.Src=imageUrl;

			return img;
		}

		private string GetItemValue(string item)
		{
			if (item==null || item.Trim()==string.Empty)
				return ITEM_NOT_DEFINED;
			else
				return item;
		}

		private string GetDateValue(string item)
		{
			if (item==null || item.Trim()==string.Empty)
			{
				return ITEM_NOT_DEFINED;
			}
			else
			{
				if (DocsPAWA.Utils.isDate(item))
					return DocsPAWA.Utils.formatStringToDate(item).ToLongDateString();
				else
					return ITEM_NOT_DEFINED;					
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
	}
}
