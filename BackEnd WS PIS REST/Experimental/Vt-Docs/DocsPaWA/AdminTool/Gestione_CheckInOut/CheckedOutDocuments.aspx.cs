using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.CheckInOut;

namespace DocsPAWA.AdminTool.Gestione_CheckInOut
{
	/// <summary>
	/// Gestione amministrazione funzionalità di checkin-checkout
	/// </summary>
	public class CheckedOutDocuments : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_position;
		protected System.Web.UI.WebControls.DataGrid grdCheckOutDocuments;
		protected HtmlForm frmCheckedOutDocuments;
		protected System.Web.UI.WebControls.Button btnRefresh;
		protected System.Web.UI.WebControls.Label lblMessage;
		protected System.Web.UI.HtmlControls.HtmlInputHidden undoCheckOutRequested;


		/// <summary>
		/// Costanti che identificano le colonne visualizzate nella griglia
		/// </summary>
		private const string COL_ID="ID";
		private const string COL_DOCUMENT="Document";
        private const string COL_SEGNATURE = "Segnature";
		private const string COL_CHECK_OUT_USER="CheckOutUser";
		private const string COL_CHECK_OUT_DATE="CheckOutDate";
		private const string COL_DOCUMENT_LOCATION="DocumentLocation";

        private bool _onFetchError = false;

		private void Page_Load(object sender, System.EventArgs e)
		{
            Session["AdminBookmark"] = "SbloccaDocumenti";
            
            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
			if(Session.IsNewSession)
			{
				Response.Redirect("../Exit.aspx?FROM=EXPIRED");
			}
			
			AmmUtils.WebServiceLink  ws=new AmmUtils.WebServiceLink();
			if(!ws.CheckSession(Session.SessionID))
			{
				Response.Redirect("../Exit.aspx?FROM=ABORT");
			}
			// ---------------------------------------------------------------

			
			this.RegisterScrollKeeper("divGrdCheckOutDocuments");			

			if (!IsPostBack)
			{
                this.btnRefresh.Attributes.Add("onClick", "ShowWaitCursor()");

                try
                {
                    this.Fetch();
                }
                catch (Exception ex)
                {
                    this.SetMessage(ex.Message);
                }

				this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"1");
			}
        }

		private void Page_PreRender(object sender, System.EventArgs e)
		{
            this.SetRecordCount();

            this.SetGridVisibility();

            if (!this._onFetchError)
            {
                this.grdCheckOutDocuments.Columns[5].Visible = this.CanForceUndoCheckOut;

                if (this.grdCheckOutDocuments.Columns[5].Visible)
                {
                    foreach (DataGridItem item in this.grdCheckOutDocuments.Items)
                    {
                        DocsPaWebCtrlLibrary.ImageButton button = item.Cells[5].FindControl("btnUndoCheckOut") as DocsPaWebCtrlLibrary.ImageButton;

                        if (button != null)
                            button.Attributes.Add("onClick", "return ConfirmUndoCheckOut();");
                    }
                }
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
			this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
			this.grdCheckOutDocuments.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdCheckOutDocuments_ItemCommand);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		/// <summary>
		/// Verifica se l'utente amministratore
		/// può annullare il blocco sul documento
		/// </summary>
		private bool CanForceUndoCheckOut
		{
			get
			{
				return CheckInOutAdminServices.CanForceUndoCheckOutAdminDocument(null);
			}
		}

		/// <summary>
		/// Caricamento dati
		/// </summary>
		private void Fetch()
        {
			CheckOutStatus[] documents=this.GetCheckedOutDocuments();

            if (documents != null)
            {
                this.grdCheckOutDocuments.DataSource = this.ToDataset(documents);
                this.grdCheckOutDocuments.DataBind();

                this.CheckOutStatusLocalList = new List<CheckOutStatus>(documents);
            }
            else
            {
                this._onFetchError = true;

                throw new ApplicationException("Si è verificato un errore nel reperimento dei dati sui documenti bloccati");
            }
		}

        /// <summary>
        /// 
        /// </summary>
        private void FetchLocal()
        {
            if (this.CheckOutStatusLocalList != null)
            {
                this.grdCheckOutDocuments.DataSource = this.ToDataset(this.CheckOutStatusLocalList.ToArray());
                this.grdCheckOutDocuments.DataBind();
            }
            else
            {
                this._onFetchError = true;

                throw new ApplicationException("Si è verificato un errore nel reperimento dei dati sui documenti bloccati");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idStatus"></param>
        /// <returns></returns>
        private CheckOutStatus GetCheckOutStatus(string idStatus)
        {
            CheckOutStatus status = null;

            // Reperimento status documento
            foreach (CheckOutStatus item in this.CheckOutStatusLocalList)
            {
                if (item.ID.Equals(idStatus))
                {
                    status = item;
                    break;
                }
            }

            return status;
        }

        /// <summary>
        /// 
        /// </summary>
        private List<CheckOutStatus> CheckOutStatusLocalList
        {
            get
            {
                if (this.ViewState["CheckOutStatusLocalList"] != null)
                    return (List<CheckOutStatus>)this.ViewState["CheckOutStatusLocalList"];
                else
                    return null;
            }
            set
            {
                if (this.ViewState["CheckOutStatusLocalList"] == null)
                    this.ViewState.Add("CheckOutStatusLocalList", value);
                else
                    this.ViewState["CheckOutStatusLocalList"] = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="checkOutDocuments"></param>
		/// <returns></returns>
		private DataSet ToDataset(CheckOutStatus[] checkOutDocuments)
		{
			DataSet ds=new DataSet("DatasetTrasmissioniEffettuate");
			DataTable dt=new DataTable("TableTrasmissioniEffettuate");
            
			dt.Columns.Add(COL_ID,typeof(string));
			dt.Columns.Add(COL_DOCUMENT,typeof(string));
			dt.Columns.Add(COL_CHECK_OUT_USER,typeof(string));
			dt.Columns.Add(COL_CHECK_OUT_DATE,typeof(string));
			dt.Columns.Add(COL_DOCUMENT_LOCATION,typeof(string));

			foreach (CheckOutStatus item in checkOutDocuments)
			{
				DataRow row=dt.NewRow();

				row[COL_ID]=item.ID;

                string document = item.DocumentNumber + "<br /> - <br />";

                if (item.IsAllegato)
                    document += "Allegato";
                else if (!string.IsNullOrEmpty(item.Segnature))
                    document += item.Segnature;
                else
                    document += "Non protocollato";

                row[COL_DOCUMENT] = document;

				string user=item.UserName;
				if (item.RoleName!=null && item.RoleName!=string.Empty)
					user+="<br />(" + item.RoleName + ")";

				row[COL_CHECK_OUT_USER]=user;
				row[COL_CHECK_OUT_DATE]=item.CheckOutDate.ToString();

				string documentLocation=item.DocumentLocation;
				if (item.MachineName!=null && item.MachineName!=string.Empty)
					documentLocation+=" su " + item.MachineName;
				row[COL_DOCUMENT_LOCATION]=documentLocation;

				dt.Rows.Add(row);
			}

			ds.Tables.Add(dt);

			return ds;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private CheckOutStatus[] GetCheckedOutDocuments()
		{
            // Reperimento idamministrazione correntemente selezionata
            string idAdministration = AmmUtils.UtilsXml.GetAmmDataSession((string) Session["AMMDATASET"], "3");

            DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
            InfoUtente infoUtente = sessionManager.getUserAmmSession();

            return CheckInOutAdminServices.GetCheckOutAdminDocuments(infoUtente, idAdministration);
		}

		/// <summary>
		/// 
		/// </summary>
		private void RegisterScrollKeeper(string divID)
		{
            AdminTool.UserControl.ScrollKeeper scrollKeeper = new AdminTool.UserControl.ScrollKeeper();
			scrollKeeper.WebControl=divID;
			this.frmCheckedOutDocuments.Controls.Add(scrollKeeper);
		}

		/// <summary>
		/// Impostazione numero documenti bloccati
		/// </summary>
		private void SetRecordCount()
		{
            if (!this._onFetchError)
            {
                int checkOutDocuments = this.grdCheckOutDocuments.Items.Count;

                string message = string.Empty;

                if (checkOutDocuments == 1)
                    message = "Trovato 1 documento bloccato";
                else if (checkOutDocuments == 0)
                    message = "Nessun documento bloccato";
                else
                    message = "Trovati " + checkOutDocuments.ToString() + " documenti bloccati";

                this.SetMessage(message);
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void SetMessage(string message)
        {
            this.lblMessage.Text = message;
        }

		/// <summary>
		/// Impostazione visibilità griglia
		/// </summary>
		private void SetGridVisibility()
		{
			this.grdCheckOutDocuments.Visible=(!this._onFetchError && this.grdCheckOutDocuments.Items.Count>0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idStatus"></param>
		private bool UndoCheckOut(string idStatus)
		{
			bool retValue=false;

			// Reperimento status documento
            CheckOutStatus status = this.GetCheckOutStatus(idStatus);

            DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
            InfoUtente adminUser = sessionManager.getUserAmmSession();

			ValidationResultInfo resultInfo=CheckInOutAdminServices.ForceUndoCheckOutAdminDocument(adminUser,status);

			if (resultInfo.Value)
			{
                this.CheckOutStatusLocalList.Remove(status);

                this.FetchLocal();

				retValue=true;
			}
			else
			{
				string errorMessage=string.Empty;

				foreach (BrokenRule brokenRule in resultInfo.BrokenRules)
				{
					if (errorMessage!=string.Empty)
						errorMessage+=Environment.NewLine;

					errorMessage+=brokenRule.Description;
				}

				errorMessage=errorMessage.Replace("'","\\'");

				// Visualizzazione messaggio di errore
				this.RegisterClientScript("UndoCheckOutError","alert('" + errorMessage + "');");
			}

			return retValue;
		}

		private void btnRefresh_Click(object sender, System.EventArgs e)
		{
            try
            {
                this.Fetch();
            }
            catch (Exception ex)
            {
                this.SetMessage(ex.Message);
            }
		}

		/// <summary>
		/// 
		/// </summary>
		private bool UndoCheckOutRequested
		{
			get
			{
				bool retValue=false;
				
				try
				{
					retValue=Convert.ToBoolean(this.undoCheckOutRequested.Value);
				}
				catch
				{
				}

				return retValue;
			}
			set
			{
				this.undoCheckOutRequested.Value=value.ToString().ToLower();
			}
		}

		private void grdCheckOutDocuments_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName=="UNDO_CHECK_OUT" && this.UndoCheckOutRequested)
			{	
				if (this.UndoCheckOut(e.Item.Cells[0].Text))
					this.UndoCheckOutRequested=false;
			}
		}


		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.ClientScript.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				//this.Page.RegisterStartupScript(scriptKey, scriptString);
                this.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
			}
		}
	}
}