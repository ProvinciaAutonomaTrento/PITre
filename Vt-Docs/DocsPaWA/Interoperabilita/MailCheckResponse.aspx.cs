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
using DocsPAWA.utils;
using DocsPAWA.SiteNavigation;
using System.Linq;

namespace DocsPAWA.Interoperabilita
{
	/// <summary>
	/// Summary description for InteropMailCheckResponse.
	/// </summary>
	public class MailCheckResponse : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Button btnClose;
        protected System.Web.UI.WebControls.Button btnEsporta;
        protected System.Web.UI.WebControls.Button btnDocGrigio;
		protected System.Web.UI.WebControls.DataGrid grdCheckResponse;
		protected System.Web.UI.HtmlControls.HtmlGenericControl panelGridCheckResponse;
		protected System.Web.UI.WebControls.Label lblMailAddress;
		protected System.Web.UI.WebControls.Label txtMailAddress;
		protected System.Web.UI.HtmlControls.HtmlTable tblCheckDetails;
		protected System.Web.UI.WebControls.Label lblCodiceRegistro;
		protected System.Web.UI.WebControls.Label txtCodiceRegistro;
		protected System.Web.UI.HtmlControls.HtmlTable Table1;
		protected System.Web.UI.WebControls.Label lblCheckResponse;
		protected System.Web.UI.HtmlControls.HtmlGenericControl panelCheckResponse;
		protected System.Web.UI.WebControls.Label lblMailMessageCount;
		//protected System.Web.UI.WebControls.Label txtMailMessageCount;
        protected string txtMailMessageCount = string.Empty;
		protected System.Web.UI.WebControls.Label lblMailUserID;
		protected System.Web.UI.WebControls.Label txtMailUserID;
		protected System.Web.UI.WebControls.Label lblMailServer;
		protected System.Web.UI.WebControls.Label txtMailServer;
		protected System.Web.UI.HtmlControls.HtmlTable tblContainer;
        protected System.Web.UI.WebControls.Table tbl_MailTypelist;
        protected System.Web.UI.WebControls.HiddenField tipo;
        protected string appTitle;

        private static DocsPaWebService docsPaWS = ProxyManager.getWS();
        static DataSet reportDataSet = null;
        static DataSet reportDataSetDocGrigio = null;
        static Dictionary<String, Int32> TipiMailRicevutaPEC = null;
        static Dictionary<String, Int32> ProcessedTypeDict = null;

        private ReportMetadata[] ReportRegistry;

		private void Page_Load(object sender, System.EventArgs e)
		{
            this.RegisterClientEvents();
            if (this.Page.IsPostBack)
            {
                Refresh();
            }
            else if (!this.IsPostBack)
            {
                this.Fetch();
            }
            string titolo = System.Configuration.ConfigurationManager.AppSettings["TITLE"];
            if (titolo != null)
                this.appTitle = titolo;
            else
                this.appTitle = "DOCSPA";
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
            this.btnDocGrigio.Click += new EventHandler(btnDocGrigio_Click);
		}
		#endregion
        
        /// <summary>
        /// Refresh della pagina dopo aver applicato i filtri sul tipo di notifica da visualizzare nella griglia di riepilogo
        /// </summary>
        private void Refresh()
        {
            MailAccountCheckResponse checkResponse = this.GetCheckResponse();
            if (checkResponse != null)
            {
                this.FetchMailAccountCheckInfo(checkResponse);
                this.SetMailMessageCount(checkResponse);
                TipiMailRicevutaPEC = HttpContext.Current.Session["TipiMailRicevutaPEC"] as Dictionary<string, int>;
                DataSet ds = this.UpdateToDataSet(checkResponse);
                this.grdCheckResponse.DataSource = ds;
                this.grdCheckResponse.DataBind();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkResponse"></param>
        /// <returns></returns>
        private DataSet UpdateToDataSet(MailAccountCheckResponse checkResponse)
        {
            string tipoNotifica = string.Empty;
            if(!string.IsNullOrEmpty(tipo.Value))
                tipoNotifica = tipo.Value;
            DataSet dataSet = new DataSet("checkResponseDataSet");
            DataTable mailProcessedTable = new DataTable("MailProcessedList");

            mailProcessedTable.Columns.Add("MailID", typeof(string));
            mailProcessedTable.Columns.Add("Type", typeof(string));
            mailProcessedTable.Columns.Add("From", typeof(string));
            mailProcessedTable.Columns.Add("Subject", typeof(string));
            mailProcessedTable.Columns.Add("Date", typeof(string));
            mailProcessedTable.Columns.Add("CountAttatchments", typeof(string));
            mailProcessedTable.Columns.Add("CheckResult", typeof(string));
            mailProcessedTable.Columns.Add("system_id", typeof(int));
            mailProcessedTable.Columns.Add("PecXRicevuta", typeof(string));

            dataSet.Tables.Add(mailProcessedTable);
            int sysId = 0;
            string nd = "N.D.";

            foreach (MailProcessed mailProcessed in checkResponse.MailProcessedList)
            {
                if (pecMessageResolver(mailProcessed).Equals(tipoNotifica) || string.IsNullOrEmpty(tipoNotifica) || tipoNotifica.Equals("all"))
                {
                    string PecXRicevuta = pecMessageResolver(mailProcessed);//mailProcessed.PecXRicevuta.ToString().Replace ("_"," ");

                    DataRow newRow = mailProcessedTable.NewRow();
                    newRow["system_id"] = sysId++;
                    if ((!string.IsNullOrEmpty(mailProcessed.ErrorMessage)) && (!mailProcessed.ErrorMessage.Contains("INTEROPERABILITA")))
                    {
                        //newRow["Type"] = nd;
                        //Andrea De Marco - Mev Gestione Eccezioni - decommentare sopra per ripristino
                        //if (!mailProcessed.ErrorMessage.Contains("OK. Eccezione non bloccante."))
                        if (!mailProcessed.ErrorMessage.Contains("OK. Eccezione non bloccante nella segnatura informatica"))
                            newRow["Type"] = nd;
                        else
                            newRow["Type"] = pecMessageResolver(mailProcessed);
                        //End De Marco
                    }
                    else
                        newRow["Type"] = pecMessageResolver(mailProcessed);
                    newRow["MailID"] = mailProcessed.MailID;
                    newRow["From"] = mailProcessed.From;
                    newRow["Subject"] = mailProcessed.Subject;
                    newRow["Date"] = mailProcessed.Date.ToString();
                    if ((!string.IsNullOrEmpty(mailProcessed.ErrorMessage)) && (!mailProcessed.ErrorMessage.Contains("INTEROPERABILITA")))
                    {
                        //newRow["CountAttatchments"] = mailProcessed.CountAttatchments;
                        //Andrea De Marco - Mev Gestione Eccezioni - decommentare sopra per ripristino
                        //if (!mailProcessed.ErrorMessage.Contains("OK. Eccezione non bloccante."))
                        if (!mailProcessed.ErrorMessage.Contains("OK. Eccezione non bloccante nella segnatura informatica"))
                            newRow["CountAttatchments"] = nd;
                        else
                            newRow["CountAttatchments"] = mailProcessed.CountAttatchments;
                        //End De Marco
                    }
                    else
                        newRow["CountAttatchments"] = mailProcessed.CountAttatchments;
                    if ((!string.IsNullOrEmpty(mailProcessed.ErrorMessage)) && (!mailProcessed.ErrorMessage.Contains("INTEROPERABILITA")))
                        newRow["PecXRicevuta"] = nd;
                    else
                        newRow["PecXRicevuta"] = PecXRicevuta;

                    string errorMessage = "OK";
                    if (mailProcessed.ErrorMessage != string.Empty)
                        errorMessage = mailProcessed.ErrorMessage;
                    newRow["CheckResult"] = errorMessage;

                    mailProcessedTable.Rows.Add(newRow);
                }
            }
            String listaTipi = "Dettaglio Email\r\n";
            foreach (string s in TipiMailRicevutaPEC.Keys)
                listaTipi += String.Format("{0} ({1}) \r\n", s, TipiMailRicevutaPEC[s].ToString());
            listaTipi += "\r\n";
            foreach (string s in TipiMailRicevutaPEC.Keys)
            {
                //if (s != "Altre")
                listaTipi += String.Format("{0} ({1}) \r\n", s, TipiMailRicevutaPEC[s].ToString());
            }
            //txtMailMessageCount.ToolTip = listaTipi;
            popolaDatiTblMailList();
            reportDataSet = dataSet.Copy();
            reportDataSetDocGrigio = (System.Web.HttpContext.Current.Session["grdCheckResponse.save"] as DataSet).Copy();
            return dataSet;
        }

		/// <summary>
		/// Reperimento dalla sessione dell'oggetto contenente i dettagli del controllo 
		/// della casella di posta elettronica certificata
		/// </summary>
		/// <returns></returns>
		private MailAccountCheckResponse GetCheckResponse()
		{
			return MailCheckResponseSessionManager.CurrentMailCheckResponse;
		}

		/// <summary>
		/// 
		/// </summary>
		private void Fetch()
		{
			MailAccountCheckResponse checkResponse=this.GetCheckResponse();

			if (checkResponse!=null)
			{
				this.FetchMailAccountCheckInfo(checkResponse);

				this.SetMailMessageCount(checkResponse);

				this.FetchGridMailMessages(checkResponse);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="checkResponse"></param>
		private void FetchMailAccountCheckInfo(MailAccountCheckResponse checkResponse)
		{
			this.txtMailUserID.Text=checkResponse.MailUserID;
			this.txtMailServer.Text=checkResponse.MailServer;
			this.txtMailAddress.Text=checkResponse.MailAddress;
			this.txtCodiceRegistro.Text=checkResponse.Registro;
			
			string errorMessage="OK";
			if (checkResponse.ErrorMessage!=string.Empty)
				errorMessage=checkResponse.ErrorMessage;
			this.panelCheckResponse.InnerText=errorMessage;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="checkResponse"></param>
		private void SetMailMessageCount(MailAccountCheckResponse checkResponse)
		{
			string message=string.Empty;

			if (checkResponse.MailProcessedList.Length==0)
			{
				message="Nessun messaggio trovato";
			}
			else
			{
				message="Totale: " + checkResponse.MailProcessedList.Length.ToString();

				int countProcessed=checkResponse.MailProcessedList.Length;

                foreach (MailProcessed mail in checkResponse.MailProcessedList)
                {
                    //if (mail.ErrorMessage!=string.Empty)
                    if ((!string.IsNullOrEmpty(mail.ErrorMessage)) && (!mail.ErrorMessage.Contains("INTEROPERABILITA")))
                    {
                        //Andrea De Marco - Mev Gestione Eccezioni
                        //if (!mail.ErrorMessage.Contains("OK. Eccezione non bloccante."))
                        if (!mail.ErrorMessage.Contains("OK. Eccezione non bloccante nella segnatura informatica"))
                            countProcessed--;
                    }
                }
				message += " - Validi: " + countProcessed.ToString(); 
			}

			this.txtMailMessageCount = message;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="checkResponse"></param>
		private void FetchGridMailMessages(MailAccountCheckResponse checkResponse)
		{
			this.grdCheckResponse.DataSource=this.ToDataSet(checkResponse);
			this.grdCheckResponse.DataBind();
		}

		/// <summary>
		/// Conversione in dataset dell'oggetto MailCheckResponse
		/// </summary>
		/// <param name="checkResponse"></param>
		/// <returns></returns>
		private DataSet ToDataSet(MailAccountCheckResponse checkResponse)
		{
			DataSet dataSet=new DataSet("checkResponseDataSet");
 
			DataTable mailProcessedTable=new DataTable("MailProcessedList");

			mailProcessedTable.Columns.Add("MailID",typeof(string));
            mailProcessedTable.Columns.Add("Type", typeof(string));
			mailProcessedTable.Columns.Add("From",typeof(string));
			mailProcessedTable.Columns.Add("Subject",typeof(string));
			mailProcessedTable.Columns.Add("Date",typeof(string));
			mailProcessedTable.Columns.Add("CountAttatchments",typeof(string));
			mailProcessedTable.Columns.Add("CheckResult",typeof(string));
            mailProcessedTable.Columns.Add("system_id", typeof(int));
            mailProcessedTable.Columns.Add("PecXRicevuta", typeof(string));

			dataSet.Tables.Add(mailProcessedTable);
            int sysId = 0;
            string nd = "N.D.";

            TipiMailRicevutaPEC = new Dictionary<string, int>();
            ProcessedTypeDict = new Dictionary<string, int>();

			foreach (MailProcessed mailProcessed in checkResponse.MailProcessedList)
			{
                string PecXRicevuta = pecMessageResolver(mailProcessed);//mailProcessed.PecXRicevuta.ToString().Replace ("_"," ");

                if (TipiMailRicevutaPEC.ContainsKey(PecXRicevuta))
                    TipiMailRicevutaPEC[PecXRicevuta]++;
                else
                    TipiMailRicevutaPEC.Add(PecXRicevuta, 1);

                
                if (ProcessedTypeDict.ContainsKey(mailProcessed.ProcessedType.ToString()))
                    ProcessedTypeDict[mailProcessed.ProcessedType.ToString()]++;
                else
                    ProcessedTypeDict.Add(mailProcessed.ProcessedType.ToString(), 1);

                

				DataRow newRow=mailProcessedTable.NewRow();
                newRow["system_id"] = sysId++;
                if ((!string.IsNullOrEmpty(mailProcessed.ErrorMessage)) && (!mailProcessed.ErrorMessage.Contains("INTEROPERABILITA")))
                {
                    //newRow["Type"] = nd;
                    //Andrea De Marco - Mev Gestione Eccezioni - decommentare sopra per ripristino
                    //if (!mailProcessed.ErrorMessage.Contains("OK. Eccezione non bloccante."))
                    if (!mailProcessed.ErrorMessage.Contains("OK. Eccezione non bloccante nella segnatura informatica"))
                        newRow["Type"] = nd;
                    else
                        newRow["Type"] = pecMessageResolver(mailProcessed);
                    //End De Marco
                }
                else
                    newRow["Type"] = pecMessageResolver(mailProcessed);
				newRow["MailID"]=mailProcessed.MailID;
				newRow["From"]=mailProcessed.From;
				newRow["Subject"]=mailProcessed.Subject;
				newRow["Date"]=mailProcessed.Date.ToString();
                if ((!string.IsNullOrEmpty(mailProcessed.ErrorMessage)) && (!mailProcessed.ErrorMessage.Contains("INTEROPERABILITA")))
                {
                    //newRow["CountAttatchments"] = mailProcessed.CountAttatchments;
                    //Andrea De Marco - Mev Gestione Eccezioni - decommentare sopra per ripristino
                    //if (!mailProcessed.ErrorMessage.Contains("OK. Eccezione non bloccante."))
                    if (!mailProcessed.ErrorMessage.Contains("OK. Eccezione non bloccante nella segnatura informatica"))
                        newRow["CountAttatchments"] = nd;
                    else
                        newRow["CountAttatchments"] = mailProcessed.CountAttatchments;
                    //End De Marco
                }
                else
                    newRow["CountAttatchments"] = mailProcessed.CountAttatchments;
                if ((!string.IsNullOrEmpty(mailProcessed.ErrorMessage)) && (!mailProcessed.ErrorMessage.Contains("INTEROPERABILITA")))
                    newRow["PecXRicevuta"] = nd;
                else
                    newRow["PecXRicevuta"] = PecXRicevuta;

				string errorMessage="OK";
                if ((!string.IsNullOrEmpty(mailProcessed.ErrorMessage)) && (!mailProcessed.ErrorMessage.Contains("INTEROPERABILITA")))
					errorMessage=mailProcessed.ErrorMessage;
				newRow["CheckResult"]=errorMessage;

				mailProcessedTable.Rows.Add(newRow);


               
			}
            String listaTipi = "Dettaglio Email\r\n";

            
            foreach (string s in ProcessedTypeDict.Keys)
                listaTipi += String.Format("{0} ({1}) \r\n", s, ProcessedTypeDict[s].ToString());
            
            listaTipi += "\r\n";
            

            foreach (string s in TipiMailRicevutaPEC.Keys)
            {
                //if (s != "Altre")
                    listaTipi += String.Format("{0} ({1}) \r\n", s, TipiMailRicevutaPEC[s].ToString());

            }
 
            //txtMailMessageCount.ToolTip = listaTipi;
            popolaDatiTblMailList();
            reportDataSet = dataSet.Copy();
            reportDataSetDocGrigio = dataSet.Copy();
            //salvo in sessione il dataset per grdCheckResponse
            if(!IsPostBack)
                System.Web.HttpContext.Current.Session["grdCheckResponse.save"] = dataSet;

            //verifico se abbiamo ricevuto una mail contenente  un errore
            foreach (MailProcessed mailProcessed in checkResponse.MailProcessedList)
            {
                if ((!string.IsNullOrEmpty(mailProcessed.ErrorMessage)) && (!mailProcessed.ErrorMessage.Contains("INTEROPERABILITA")) ||
                    (pecMessageResolver(mailProcessed) == "Con Errori" || pecMessageResolver(mailProcessed) == "Eccezione")
                    )
                //if (!string.IsNullOrEmpty(mailProcessed.ErrorMessage))
                {
                    //Andrea De Marco - Gestione Eccezioni PEC
                    //if (mailProcessed.ErrorMessage.Equals("OK. Eccezione non bloccante."))
                    if (mailProcessed.ErrorMessage.Equals("OK. Eccezione non bloccante nella segnatura informatica"))
                    {
                        this.btnDocGrigio.Visible = true;
                        break;
                    }
                    else 
                    {
                        //creo il grigio automaticamente
                        creaGrigioDaReport(true);
                        break;
                    }
                    //End Andrea De Marco
                    //Decommentare per ripristinare modifica De Marco
                    ////creo il grigio automaticamente
                    //creaGrigioDaReport(true);
                    //break;
                }
            }

			return dataSet;
		}

        private int creaGrigioDaReport(bool trasmDoc)
        {

            InfoUtente userInfo = UserManager.getInfoUtente();
            Ruolo ruolo = UserManager.getRuolo();
            if (CallContextStack.CurrentContext == null)
                CallContextStack.CurrentContext = new CallContext("ReportingContext");

            PrintReportRequestDataset request =
               new PrintReportRequestDataset()
               {
                   ContextName = "CasellaIstituzionale",
                   ReportType = ReportTypeEnum.PDF,
                   ReportKey = "RicercaCasellaIstituzionale",
                   Title = "Casella Istituzionale",
                   SubTitle = String.Format("Report generato {0} alle {1}", DateTime.Now.ToString("dddd, dd MMMM yyyy"), (DateTime.Now).ToString("HH:mm:ss")),
                   SearchFilters = null,
                   UserInfo = userInfo,
                   InputDataset = reportDataSetDocGrigio
               };
            ReportingUtils.PrintRequest = request;
            this.ReportRegistry = ReportingUtils.GetReportRegistry(ReportingUtils.PrintRequest.ContextName);
            ReportingUtils.PrintRequest.ColumnsToExport = this.ReportRegistry.Where(r => r.ReportKey == request.ReportKey).First().ExportableFields;

            FileDocumento fd = ReportingUtils.GenerateReport(request);

            int retValue = 0;
            try
            {

                DocsPaWR.SchedaDocumento scheda = new SchedaDocumento();
                DocsPaWR.Oggetto ogg = new Oggetto();
                DocsPaWR.FileRequest fr = null;
                //Report Casella Istituzional del RF: codice/descrizione

                ogg.descrizione = String.Format("Report Casella Istituzionale {0} del {1} alle {2}", txtCodiceRegistro.Text, DateTime.Now.ToString("dddd, dd MMMM yyyy"), (DateTime.Now).ToString("HH:mm:ss"));
                scheda.oggetto = ogg;
                scheda.personale = "0";
                scheda.privato = "0";
                scheda.userId = UserManager.getInfoUtente().userId;
                scheda.typeId = "LETTERA";
                scheda.tipoProto = "G";
                scheda.appId = "ACROBAT";
                scheda.idPeople = UserManager.getInfoUtente().idPeople;
                scheda = docsPaWS.DocumentoAddDocGrigia(scheda, UserManager.getInfoUtente(), UserManager.getRuolo());
                fr = (DocsPAWA.DocsPaWR.FileRequest)scheda.documenti[0];
                fr.docNumber = scheda.docNumber;
                fr = docsPaWS.DocumentoPutFile(fr, fd, UserManager.getInfoUtente());
                if (fr != null) retValue = 1;
                //disabilito il pulsante di creazione doc grigio
                btnDocGrigio.Enabled = false;
                if (trasmDoc)
                    CreateAndTrasmDoc(scheda, userInfo, ruolo);
            }
            catch (Exception ex) { retValue = 0; }
            return retValue;
        }
      
        protected void btn_esporta_Click(object sender, EventArgs e)
        {

            InfoUtente userInfo = UserManager.getInfoUtente();
            // Inizializzazione del call context
            if (CallContextStack.CurrentContext == null)
                CallContextStack.CurrentContext = new CallContext("ReportingContext");

            string userRole = "";
            //CH: Modifica per avere evidenza sul report dei problemi durante lo scarico della posta
            if (this.panelCheckResponse.InnerText != "OK")
                userRole = "Utente: " + UserManager.getUtente().descrizione + ", Ruolo: " + UserManager.getRuoloById(userInfo.idCorrGlobali, this.Page).descrizione + ". \r\n ESITO CONTROLLO CASELLA: " + this.panelCheckResponse.InnerText;
            else
                //Estraggo le informazioni da visualizzare su utente, ruolo
                userRole = "Utente: " + UserManager.getUtente().descrizione + ", Ruolo: " + UserManager.getRuoloById(userInfo.idCorrGlobali, this.Page).descrizione;
            
            // Salvataggio della request nel call context 
            PrintReportRequestDataset request =
                new PrintReportRequestDataset()
                {
                    ContextName = "CasellaIstituzionale",
                    SearchFilters = null,
                    UserInfo = userInfo,
                    InputDataset = reportDataSet,
                    AdditionalInformation = userRole
                };

            ReportingUtils.PrintRequest = request;

            Page.ClientScript.RegisterStartupScript(this.GetType(), "openReport", ReportingUtils.GetOpenReportPageScript(true), true);
        }

        private void popolaDatiTblMailList()
        {
            this.tbl_MailTypelist.CssClass = "testo_grigio";
            this.tbl_MailTypelist.CellPadding = 1;
            this.tbl_MailTypelist.CellSpacing = 1;
            this.tbl_MailTypelist.BorderWidth = 1;
            this.tbl_MailTypelist.Attributes.Add("style", "'BORDER-RIGHT: 1px solid; BORDER-TOP: 1px solid; BORDER-LEFT: 1px solid; WIDTH: 100%; BORDER-BOTTOM: 1px solid'");
            this.tbl_MailTypelist.BackColor = Color.FromArgb(255, 255, 255);
            this.tbl_MailTypelist.ID = "table_Todolist";
            TableRow triga;
            TableCell tcell;

            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(149, 149, 149);

            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(80);
            tcell.Text = "Tipo MAIL";
            tcell.HorizontalAlign = HorizontalAlign.Right;
            tcell.ColumnSpan = 1;
            triga.Cells.Add(tcell);

            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(20);
            tcell.Text = "Numero";
            tcell.HorizontalAlign = HorizontalAlign.Right;
            triga.Cells.Add(tcell);

            this.tbl_MailTypelist.Rows.Add(triga);
            /*
            foreach (string s in ProcessedTypeDict.Keys)
            {
                triga = new TableRow();
                triga.Height = 15;
                triga.BackColor = Color.FromArgb(242, 242, 242);


                tcell = new TableCell();
                tcell.CssClass = "bg_grigioNP";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(80);
                tcell.Text = s;
                tcell.HorizontalAlign = HorizontalAlign.Right;
                triga.Cells.Add(tcell);

                tcell = new TableCell();
                tcell.CssClass = "bg_grigioNP";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(20);
                tcell.Text = ProcessedTypeDict[s].ToString();
                tcell.HorizontalAlign = HorizontalAlign.Right;
                triga.Cells.Add(tcell);
                this.tbl_MailTypelist.Rows.Add(triga);

            }
            */

            foreach (string s in TipiMailRicevutaPEC.Keys)
            {
                triga = new TableRow();
                triga.Height = 15;
                triga.BackColor = Color.FromArgb(242, 242, 242);


                tcell = new TableCell();
                tcell.CssClass = "bg_grigioNP";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(80);
                tcell.Text = s;
                tcell.HorizontalAlign = HorizontalAlign.Right;
                triga.Cells.Add(tcell);

                tcell = new TableCell();
                tcell.CssClass = "bg_grigioNP";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(20);
                //creo il link button
                LinkButton link = new LinkButton();
                link.ID = "link_" + s;
                link.ForeColor = Color.Black;
                link.Font.Underline = true;
                link.Font.Bold = true;
                link.Text = TipiMailRicevutaPEC[s].ToString();
                link.OnClientClick = "document.getElementById('" + tipo.ClientID + "').value = '" + s + "';";
                tcell.HorizontalAlign = HorizontalAlign.Right;
                tcell.Controls.Add(link);
                triga.Cells.Add(tcell);
                this.tbl_MailTypelist.Rows.Add(triga);

            }

            //aggiungo una riga per l'elenco completo delle notifiche
            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(242, 242, 242);
            tcell = new TableCell();
            tcell.CssClass = "bg_grigioNP";
            tcell.Font.Bold = true;
            tcell.Width = Unit.Percentage(80);
            tcell.Text = "Elenco";
            tcell.HorizontalAlign = HorizontalAlign.Right;
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "bg_grigioNP";
            tcell.Font.Bold = true;
            tcell.Width = Unit.Percentage(20);
            //creo il link button
            LinkButton linkAll = new LinkButton();
            linkAll.ID = "link_all";
            linkAll.ForeColor = Color.Black;
            linkAll.Font.Underline = true;
            linkAll.Font.Bold = true;
            linkAll.Text = this.txtMailMessageCount;
            linkAll.OnClientClick = "document.getElementById('" + tipo.ClientID + "').value = 'all';";
            tcell.HorizontalAlign = HorizontalAlign.Right;
            tcell.Controls.Add(linkAll);
            triga.Cells.Add(tcell);
            this.tbl_MailTypelist.Rows.Add(triga);

            //salvo in sessione le informazioni sulla griglia tbl_MailTypelist
            if (!IsPostBack)
            {
                System.Web.HttpContext.Current.Session["TipiMailRicevutaPEC"] = TipiMailRicevutaPEC;
            }
        }

        private string pecMessageResolver(MailProcessed mr)
        {
            if (mr.ProcessedType == MailProcessedType.Signature)
                return "Pec con segnatura";
            switch (mr.PecXRicevuta)
            {


                case MailPecXRicevuta.PEC_Delivered_Notify:
                case MailPecXRicevuta.PEC_Delivered_Notify_Short:
                case MailPecXRicevuta.PEC_Delivered:
                    return "Consegna";
                    
                case MailPecXRicevuta.PEC_Accept_Notify:
                    return "Accettazione";

                case MailPecXRicevuta.PEC_Alert_Virus:
                case MailPecXRicevuta.PEC_Contain_Virus:
                case MailPecXRicevuta.PEC_Error_Delivered_Notify_By_Virus:
                    return "Rilevazione Virus";

                case MailPecXRicevuta.From_Non_PEC:
                case MailPecXRicevuta.PEC_Error:
                case MailPecXRicevuta.PEC_Error_Preavviso_Delivered_Notify:
                case MailPecXRicevuta.Delivery_Status_Notification:
                    return "Con Errori";

                case MailPecXRicevuta.PEC_Non_Accept_Notify:
                    return "Mancata Accettazione";

                case MailPecXRicevuta.PEC_Presa_In_Carico:
                    return "Presa in Carico";

                case MailPecXRicevuta.PEC_Mancata_Consegna:
                    return "Mancata Consegna";

                case MailPecXRicevuta.PEC_NO_XRicevuta:
                    return "Pec";

                case MailPecXRicevuta.unknown:
                default:
                    {
                        if (mr.ProcessedType == MailProcessedType.ConfirmReception)
                            return "Conferma Ricezione";
                        if (mr.ProcessedType == MailProcessedType.Eccezione)
                            return "Eccezione";
                        if (mr.ProcessedType == MailProcessedType.NotifyCancellation)
                            return "Annullamento protocollazione";
                        if (mr.ProcessedType == MailProcessedType.Signature)
                            //return "Firmata";
                            return "Pec con segnatura";
                        if (mr.ProcessedType == MailProcessedType.NonPEC)
                            return "Non Pec";
                        if (mr.ProcessedType == MailProcessedType.Pec)
                            return "Pec";
                        return "Altre";
                    }

            }
        }

        private void CreateAndTrasmDoc(SchedaDocumento scheda, InfoUtente userInfo, Ruolo ruolo)
        {
            //creo la trasmissione
            Trasmissione trasm = new Trasmissione();
            TrasmissioneSingola trasmS = new TrasmissioneSingola();
            TrasmissioneUtente trasmU = new TrasmissioneUtente();

            //trasm.dataInvio = DateTime.Now.ToShortDateString();
            trasm.DataDocFasc = scheda.dataCreazione;
            trasm.infoDocumento = DocsPAWA.DocumentManager.getInfoDocumento(scheda);
            trasm.ruolo = ruolo;
            trasm.tipoOggetto = TrasmissioneTipoOggetto.DOCUMENTO;
            trasm.utente = DocsPAWA.UserManager.getUtente();

            trasmS.ragione = TrasmManager.GetRagioneNotifica(userInfo.idAmministrazione);
            trasmS.tipoDest = TrasmissioneTipoDestinatario.RUOLO;
            trasmS.tipoTrasm = "S";
            trasmS.corrispondenteInterno = UserManager.getCorrispondenteBySystemID(this.Page, ruolo.systemId);
            trasmU.daNotificare = true;
            trasmU.utente = UserManager.getUtente();

            trasmS.trasmissioneUtente = new TrasmissioneUtente[1];
            trasmS.trasmissioneUtente[0] = trasmU;
            trasm.trasmissioniSingole = new TrasmissioneSingola[1];
            trasm.trasmissioniSingole[0] = trasmS;
            Trasmissione trasmRes = TrasmManager.saveExecuteTrasm(this.Page, trasm, userInfo);

        }

		#region Gestione javascript

		private void RegisterClientEvents()
		{
			this.btnClose.Attributes.Add("onClick","CloseWindow();");
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

        protected void btnDocGrigio_Click(object sender, EventArgs e)
        {
            creaGrigioDaReport(false);
        }
		#endregion

     
	}
}
