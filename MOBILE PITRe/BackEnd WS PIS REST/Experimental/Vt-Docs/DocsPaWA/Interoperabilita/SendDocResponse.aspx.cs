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
using DocsPaVO.utente;
using DocsPaVO.Interoperabilita;

namespace DocsPAWA.Interoperabilita
{
	/// <summary>
	/// Summary description for SendDocumentResponse.
	/// </summary>
	public class SendDocResponse : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Button btnClose;
		protected System.Web.UI.HtmlControls.HtmlTable tblContainer;
		protected System.Web.UI.HtmlControls.HtmlTable tblSendDetails;
		protected System.Web.UI.WebControls.Label lblSendDateTime;
		protected System.Web.UI.WebControls.Label txtSendDateTime;
		protected System.Web.UI.WebControls.Label lblRegistro;
		protected System.Web.UI.WebControls.Label txtRegistro;
		protected System.Web.UI.WebControls.DataGrid grdSendMailResponse;
		protected System.Web.UI.WebControls.Label lblSegnatura;
		protected System.Web.UI.WebControls.Label lblSendResponse;
		protected System.Web.UI.WebControls.Label txtSendResponse;
		protected System.Web.UI.WebControls.Label txtSegnatura;
		protected System.Web.UI.WebControls.Button btnSendMail;
		protected System.Web.UI.HtmlControls.HtmlGenericControl panelGridCheckResponse;
		protected System.Web.UI.HtmlControls.HtmlGenericControl panelWaitSendMail;
		protected System.Web.UI.WebControls.CheckBox chkSelectUnselectAll;

		/// <summary>
		/// Costanti che identificano le colonne del datagrid
		/// </summary>
		private const int GRID_COL_MAIL=0;
		private const int GRID_COL_ADRESSEE=1;
		private const int GRID_COL_RESPONSE=2;
		private const int GRID_COL_RESPONSE_STRING=3;
		private const int GRID_COL_SEND=4;
		private const int GRID_COL_NO_INTEROP=5;

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires=-1;
		
			if (!this.IsPostBack)
			{
				this.RegisterClientEvents();

				this.Fetch();
			}
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
		DocsPaWR.SendDocumentResponse sendDocumentResponse=this.GetSendDocumentResponse();
			if(sendDocumentResponse!=null)
			{
				this.SetVisibilityCheckGridColumn();

				this.RefreshControlsSend();
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
			this.btnSendMail.Click += new System.EventHandler(this.btnSendMail_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		private void SetVisibilityCheckGridColumn()
		{
			bool hasMailNotSend=this.HasMailNotSend(this.GetSendDocumentResponse());

			this.grdSendMailResponse.Columns[GRID_COL_SEND].Visible=hasMailNotSend;
		}

		/// <summary>
		/// Reperimento dalla sessione dell'oggetto contenente i dettagli 
		/// della spedizione del documento
		/// </summary>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.SendDocumentResponse GetSendDocumentResponse()
		{
			return SendDocumentSessionManager.CurrentSendDocumentResponse;
		}

		/// <summary>
		/// Verifica se almeno un invio mail non è andato a buon fine
		/// </summary>
		/// <param name="sendResponse"></param>
		/// <returns></returns>
		private bool HasMailNotSend(DocsPAWA.DocsPaWR.SendDocumentResponse sendResponse)
		{
			bool retValue=false;

			foreach (DocsPAWA.DocsPaWR.SendDocumentMailResponse sendMailResponse in sendResponse.SendDocumentMailResponseList)
			{
				retValue=!sendMailResponse.SendSucceded;
				if (retValue)
					break;
			}

			return retValue;
		}

		/// <summary>
		/// Caricamento dati
		/// </summary>
		private void Fetch()
		{
			DocsPaWR.SendDocumentResponse sendDocumentResponse=this.GetSendDocumentResponse();

			if (sendDocumentResponse!=null)
			{
				this.FetchGeneralData(sendDocumentResponse);

				this.FetchGridSendDocumentMail(sendDocumentResponse);
			}
		}

		/// <summary>
		/// Caricamento dati generali
		/// </summary>
		/// <param name="sendResponse"></param>
		private void FetchGeneralData(DocsPAWA.DocsPaWR.SendDocumentResponse sendResponse)
		{	
			this.txtSendDateTime.Text=sendResponse.SendDateTime.ToString();
			this.txtSegnatura.Text=sendResponse.SchedaDocumento.protocollo.segnatura;
			this.txtRegistro.Text=sendResponse.SchedaDocumento.registro.codRegistro + " - " + sendResponse.SchedaDocumento.registro.descrizione;
			if (sendResponse.SendSucceded)
				this.txtSendResponse.Text="Documento spedito correttamente a tutti i destinatari";
			else
				this.txtSendResponse.Text="Errore nella spedizione del documento ad almeno un destinatario";
		}

		/// <summary>
		/// Caricamento griglia documenti inviati
		/// </summary>
		/// <param name="checkResponse"></param>
		private void FetchGridSendDocumentMail(DocsPAWA.DocsPaWR.SendDocumentResponse sendResponse)
		{
			this.grdSendMailResponse.DataSource=this.ToDataSet(sendResponse);
			this.grdSendMailResponse.DataBind();
		}

		/// <summary>
		/// Conversione in dataset dell'oggetto MailCheckResponse
		/// </summary>
		/// <param name="checkResponse"></param>
		/// <returns></returns>
		private DataSet ToDataSet(DocsPAWA.DocsPaWR.SendDocumentResponse sendResponse)
		{
			DataSet dataSet=new DataSet("sendResponseDataSet");
 
			DataTable mailSendTable=new DataTable("MailSendList");

			mailSendTable.Columns.Add("MailAddress",typeof(string));
			mailSendTable.Columns.Add("Adressee",typeof(string));
			mailSendTable.Columns.Add("SendResultBoolean",typeof(bool));
			mailSendTable.Columns.Add("SendResult",typeof(string));
			mailSendTable.Columns.Add("NoInterop",typeof(string));

			dataSet.Tables.Add(mailSendTable);

            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            bool interopInterna = ws.IsEnabledInteropInterna();

			foreach (DocsPAWA.DocsPaWR.SendDocumentMailResponse sendMailResponse in sendResponse.SendDocumentMailResponseList)
			{
				DataRow newRow=mailSendTable.NewRow();

                if (interopInterna)
                {
                    DocsPAWA.DocsPaWR.Corrispondente corr = (DocsPAWA.DocsPaWR.Corrispondente)sendMailResponse.Destinatari[0];
                    if ((corr.tipoIE == null && corr.tipoCorrispondente != null && corr.tipoCorrispondente == "O")
                        || (corr.tipoIE != null && corr.tipoIE == "E"))
                    {

                        newRow["MailAddress"] = sendMailResponse.MailAddress;
                    }
                    else
                    {

                        newRow["MailAddress"] = ((DocsPAWA.DocsPaWR.Corrispondente)sendMailResponse.Destinatari[0]).codiceAOO;
                    }
                }
                else
                {
                    newRow["MailAddress"] = sendMailResponse.MailAddress;
                }
				newRow["Adressee"]=this.GetAdressee(sendMailResponse.Destinatari);
				newRow["SendResultBoolean"]=sendMailResponse.SendSucceded;

				string errorMessage="Inviato";
				if (!sendMailResponse.SendSucceded)
					errorMessage=sendMailResponse.SendErrorMessage;
				newRow["SendResult"]=errorMessage;
				newRow["NoInterop"]=sendMailResponse.MailNonInteroperante.ToString();

				mailSendTable.Rows.Add(newRow);
			}

			return dataSet;
		}

		/// <summary>
		/// Reperimento di tutti i destinatari legati all'indirizzo mail
		/// </summary>
		/// <param name="adressee"></param>
		/// <returns></returns>
		private string GetAdressee(DocsPAWA.DocsPaWR.Corrispondente[] adressee)
		{
			string retValue=string.Empty;

			foreach (DocsPAWA.DocsPaWR.Corrispondente adresse in adressee)
			{
				if (retValue!=string.Empty)
					retValue+=Environment.NewLine;

				retValue+=adresse.codiceCorrispondente + " - " + adresse.descrizione;
			}

			return retValue;
		}

		/// <summary>
		/// Invio del documento ad un singolo indirizzo mail
		/// </summary>
		/// <param name="mailAddress"></param>
		private bool SendDocumentMail(string mailAddress)
		{
			DocsPaWR.SendDocumentResponse currentResponse=this.GetSendDocumentResponse();

			DocsPaWR.SendDocumentResponse newResponse=DocumentManager.spedisciDocMail(this,currentResponse.SchedaDocumento,mailAddress);

			currentResponse.SchedaDocumento=newResponse.SchedaDocumento;
			currentResponse.SendDateTime=newResponse.SendDateTime;
			currentResponse.SendSucceded=newResponse.SendSucceded;

			if (newResponse!=null && newResponse.SendDocumentMailResponseList.Length==1)
			{
				for (int i=0;i<currentResponse.SendDocumentMailResponseList.Length;i++)
				{
					DocsPaWR.SendDocumentMailResponse mailResponse=currentResponse.SendDocumentMailResponseList[i];

					if (mailResponse.MailAddress.Equals(mailAddress))
						mailResponse=newResponse.SendDocumentMailResponseList[0];
					
				}
			}

			return newResponse.SendSucceded;
		}

		/// <summary>
		/// Aggiornamento visualizzazione controlli relativamente
		/// all'esito della spedizione della mail.
		/// Per ogni mail inviata, viene visualizzato o meno
		/// un check che permette di selezionare l'elemento e rispedire la mail.
		/// </summary>
		private void RefreshControlsSend()
		{
			bool almostOneChecked=false;

			bool columnVisible=this.grdSendMailResponse.Columns[GRID_COL_SEND].Visible;

			if (columnVisible)
			{
				bool selectAll=this.chkSelectUnselectAll.Checked;

				foreach (DataGridItem item in this.grdSendMailResponse.Items)
				{	
					if (item.Cells[GRID_COL_SEND].Controls.Count>0)
					{
						CheckBox checkBox=item.Cells[GRID_COL_SEND].FindControl("chkSendMail") as CheckBox;

						if (checkBox!=null)
						{
							checkBox.Visible=(item.Cells[GRID_COL_RESPONSE].Text!=string.Empty && 
											  item.Cells[GRID_COL_RESPONSE].Text.ToLower().Equals("false")
												&& item.Cells[GRID_COL_NO_INTEROP].Text.ToLower().Equals("false"));

							checkBox.Checked=selectAll;

							if (!almostOneChecked && checkBox.Checked)
								almostOneChecked=true;
						}
					}
				}
			}
			
			// visualizzazione del check selezione / deselezione tutti
			this.chkSelectUnselectAll.Visible=columnVisible;

			// visualizzazione del pulsante di selezione
			this.btnSendMail.Visible=columnVisible;
		}

		/// <summary>
		/// Verifica se almeno un elemento è selezionato
		/// </summary>
		/// <returns></returns>
		private bool AlmostOneItemChecked()
		{
			bool retValue=false;

			foreach (DataGridItem item in this.grdSendMailResponse.Items)
			{
				CheckBox chkSendMail=item.FindControl("chkSendMail") as CheckBox;
				retValue=(chkSendMail!=null && chkSendMail.Checked);
				if (retValue)
					break;
			}

			return retValue;
		}

		/// <summary>
		/// Azione di invio mail a tutti i destinatari selezionati
		/// </summary>
		private void PerformActionSendMail()
		{
			foreach (DataGridItem item in this.grdSendMailResponse.Items)
			{
				CheckBox checkBox=item.Cells[GRID_COL_SEND].FindControl("chkSendMail") as CheckBox;

				if (checkBox!=null && checkBox.Visible && checkBox.Checked)
				{
					string mailAddress=item.Cells[GRID_COL_MAIL].Text;

					this.SendDocumentMail(mailAddress);
				}
			}
		
			this.Fetch();

			this.panelWaitSendMail.Style["VISIBILITY"]="hidden";
			this.panelGridCheckResponse.Style["VISIBILITY"]="visible";
		}

		/// <summary>
		/// Handler evento pulsante invio mail
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSendMail_Click(object sender, System.EventArgs e)
		{
			if (this.AlmostOneItemChecked())
			{
				this.PerformActionSendMail();
			}
			else
			{
				this.RegisterClientScript("AlmostOneItemChecked","alert('Nessun elemento selezionato');");
			}
		}

		#region Gestione javascript

		private void RegisterClientEvents()
		{
			this.btnClose.Attributes.Add("onClick","CloseWindow();");
			this.btnSendMail.Attributes.Add("onClick","WaitSendMail();");
		}

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}

		#endregion

        protected void grdSendMailResponse_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                bool interopInterna = ws.IsEnabledInteropInterna();
                if (interopInterna)
                {
                    string headerName = "Codice AOO / Indirizzo email";
                    ((TableCell)e.Item.Cells[0]).Text = headerName;
                }
                else
                {
                    string headerName = "Indirizzo email";
                    ((TableCell)e.Item.Cells[0]).Text = headerName;
                }
                
            }
        }
	}
}