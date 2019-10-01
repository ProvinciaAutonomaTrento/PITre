using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Drawing;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class CheckMailboxReport : System.Web.UI.Page
    {
        #region Property

        /// <summary>
        /// Id corrispondente all'oggetto InfoCheckMailbox 
        /// </summary>
        private string IdCheckMailbox
        {
            get
            {
                if (HttpContext.Current.Session["idCheckMailbox"] != null)
                    return (String)HttpContext.Current.Session["idCheckMailbox"];
                else
                    return null;
            }
        }

        private MailAccountCheckResponse MailCheckResponse
        {
            get
            {
                if (HttpContext.Current.Session["mailCheckResponse"] != null)
                    return (MailAccountCheckResponse)HttpContext.Current.Session["mailCheckResponse"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["mailCheckResponse"] = value;
            }
        }

        private DataSet DataSet
        {
            get
            {
                if (HttpContext.Current.Session["dataSet"] != null)
                    return (DataSet)HttpContext.Current.Session["dataSet"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["dataSet"] = value;
            }
        }

        private PrintReportRequestDataset RequestPrintReport
        {
            get
            {
                if (HttpContext.Current.Session["requestPrintReport"] != null)
                    return (PrintReportRequestDataset)HttpContext.Current.Session["requestPrintReport"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["requestPrintReport"] = value;
            }
        }

        private bool ReadOnlySubtitle
        {
            set
            {
                HttpContext.Current.Session["readOnlySubtitle"] = value;
            }
        }

        private Dictionary<string, int> TipoMailRicevutaPec
        {
            get
            {
                if (HttpContext.Current.Session["tipoMailRicevutaPec"] != null)
                    return (Dictionary<string, int>)HttpContext.Current.Session["tipoMailRicevutaPec"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["tipoMailRicevutaPec"] = value;
            }
        }

        private bool ReportCreated
        {
            get
            {
                if (HttpContext.Current.Session["ReportCreated"] != null)
                    return (bool)HttpContext.Current.Session["ReportCreated"];
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["ReportCreated"] = value;
            }
        }
        #endregion

        #region Global variable
            static Dictionary<String, Int32> TipiMailRicevutaPEC = null;
            static Dictionary<String, Int32> ProcessedTypeDict = null;
            static DataSet reportDataSet = null;
            static DataSet reportDataSetDocGrigio = null;
            private ReportMetadata[] ReportRegistry;
            private static string number;
            private static string typeMail;
            private static string list;
            private static string detailsMail;
            
        #endregion

        #region Const
        private const string ND = "N.D.";
        #endregion

        #region Standard Method
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    InitializeLanguage();
                    //if (MailCheckResponse == null)
                    MailCheckResponse = CheckMailboxManager.InfoReportMailbox(IdCheckMailbox);
                    MailCheckResponse.MailProcessedList = (from resp in MailCheckResponse.MailProcessedList orderby resp.Date descending select resp).ToArray();
                    FetchMailAccountCheckInfo();
                    GrdMails_Bind();
                }
                else
                {
                    Refresh();
                    ReadRetValueFromPopup();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializePage()
        {
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.ReportGenerator.Title = Utils.Languages.GetLabelFromCode("CheckMailboxReportGenerator", language);
            this.CheckMailboxReportExport.Text = Utils.Languages.GetLabelFromCode("CheckMailboxReportExport", language);
            this.CheckMailboxReportCreateDoc.Text = Utils.Languages.GetLabelFromCode("CheckMailboxReportCreateDoc", language);
            this.CheckMailboxReportClose.Text = Utils.Languages.GetLabelFromCode("CheckMailboxReportClose", language);
            this.lblMailUserId.Text = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblMailUserId", language);
            this.lblMailServer.Text = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblMailServer", language);
            this.lblIndirizzoEmail.Text = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblIndirizzoEmail", language);
            this.lblRegistro.Text = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblRegistro", language);
            this.lblEsitoCasella.Text = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblEsitoCasella", language);
            this.lblMessages.Text = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblMessages", language);
            number = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblNumber", language);
            typeMail = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblTypeMail", language);
            list = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblList", language);
            detailsMail = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblDetailsMail", language);
        }

        /// <summary>
        /// Refresh della pagina dopo aver applicato i filtri sul tipo di notifica da visualizzare nella griglia di riepilogo
        /// </summary>
        private void Refresh()
        {
            MailAccountCheckResponse checkResponse = MailCheckResponse;
            if (checkResponse != null)
            {
                this.FetchMailAccountCheckInfo();
                TipiMailRicevutaPEC = TipoMailRicevutaPec;
                DataSet ds = this.UpdateToDataSet(checkResponse);
                this.GrdMails.DataSource = ds;
                this.GrdMails.DataBind();
            }
        }

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.ReportGenerator.ReturnValue))
            {
                HttpContext.Current.Session.Remove("ReportRegistry");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ReportGenerator','')", true);
                return;
            }
        }

        #endregion

        #region Event Handler

        protected void CheckMailboxReportExport_Click(object sender, EventArgs e)
        {
            try {
                string language = UIManager.UserManager.GetUserLanguage();
                InfoUtente userInfo = UserManager.GetInfoUser();

                string userRole = "";
                //CH: Modifica per avere evidenza sul report della Connessione al server di posta fallita
                if (this.txtEsitoCasella.InnerText != "OK")
                    userRole = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblUser", language) + " " + UserManager.GetUserInSession().descrizione + ", " + Utils.Languages.GetLabelFromCode("CheckMailboxReportLblRole", language) + " " + RoleManager.getRuoloById(userInfo.idCorrGlobali).descrizione + ". \r\n " + Utils.Languages.GetLabelFromCode("CheckMailboxReportLblResultCheckMailbox", language);
                else
                    //Estraggo le informazioni da visualizzare su utente, ruolo
                    userRole = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblUser", language) + " " + UserManager.GetUserInSession().descrizione + ", " + Utils.Languages.GetLabelFromCode("CheckMailboxReportLblRole", language) + " " + RoleManager.getRuoloById(userInfo.idCorrGlobali).descrizione;

                // Salvataggio della request nel call context 
                PrintReportRequestDataset request =
                    new PrintReportRequestDataset()
                    {
                        ContextName = "CasellaIstituzionale",
                        SearchFilters = null,
                        UserInfo = userInfo,
                        InputDataset = reportDataSet,
                        AdditionalInformation = userRole,
                        SubTitle = String.Format(Utils.Languages.GetLabelFromCode("ReportGeneratorTbwSubtitle", language), MailCheckResponse.DtaConcluded.ToShortDateString(), MailCheckResponse.DtaConcluded.ToShortTimeString())
                    };

                //ReportingUtils.PrintRequest = request;
                ReadOnlySubtitle = true;
                RequestPrintReport = request;
                Session["visibleGrdFields"] = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ReportGenerator", "ajaxModalPopupReportGenerator();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CheckMailboxReportCreateDoc_Click(object sender, EventArgs e)
        {
            try {
                creaGrigioDaReport(false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CheckMailboxReportClose_Click(object sender, EventArgs e)
        {
            try {
                    CheckMailboxManager.RemoveReportMailbox(IdCheckMailbox);
                    HttpContext.Current.Session.Remove("mailCheckResponse");
                    ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('CheckMailboxReport','close');", true);
                }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Grid manager

        private void GrdMails_Bind()
        {
            this.GrdMails.DataSource = ToDataSet(MailCheckResponse);
            this.GrdMails.DataBind();
        }

        /// <summary>
        /// Conversione in dataset dell'oggetto MailCheckResponse
        /// </summary>
        /// <param name="checkResponse"></param>
        /// <returns></returns>
        private DataSet ToDataSet(MailAccountCheckResponse checkResponse)
        {
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
                if ((!string.IsNullOrEmpty(mailProcessed.ErrorMessage)) && (!mailProcessed.ErrorMessage.Contains("INTEROPERABILITA")))
                    errorMessage = mailProcessed.ErrorMessage;
                newRow["CheckResult"] = errorMessage;

                mailProcessedTable.Rows.Add(newRow);

            }
            String listaTipi = detailsMail + "\r\n";


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
            if (!IsPostBack)
                DataSet = dataSet;

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
                        this.CheckMailboxReportCreateDoc.Visible = true;

                        //INC000001079959 - Report casella istituzionale con messaggio di errore non notificato
                        //Non interrompo perchè se sono presenti mail non elaborate non genera il report
                        //break;
                    }
                    else
                    {
                        //creo il grigio automaticamente
                        if (!ReportCreated)
                        {
                            creaGrigioDaReport(true);
                            ReportCreated = true;
                            break;
                        }
                        else
                        {
                            break;
                        }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkResponse"></param>
        /// <returns></returns>
        private DataSet UpdateToDataSet(MailAccountCheckResponse checkResponse)
        {
            string tipoNotifica = string.Empty;
            if (!string.IsNullOrEmpty(tipo.Value))
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
            String listaTipi = detailsMail + "\r\n";
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
            reportDataSetDocGrigio = DataSet.Copy();
            return dataSet;
        }

        #endregion

        #region Auxiliary methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkResponse"></param>
        private string SetMailMessageCount()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            string message = string.Empty;

            if (MailCheckResponse.MailProcessedList.Length == 0)
            {
                message = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblNoMessage", language);
            }
            else
            {
                
                message = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblTotal", language) + " " + MailCheckResponse.MailProcessedList.Length.ToString();

                int countProcessed = MailCheckResponse.MailProcessedList.Length;

                foreach (MailProcessed mail in MailCheckResponse.MailProcessedList)
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
                message += " - " + Utils.Languages.GetLabelFromCode("CheckMailboxReportLblValid", language) + " " + countProcessed.ToString();
            }

            return message;
        }

        private int creaGrigioDaReport(bool trasmDoc)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            InfoUtente userInfo = UserManager.GetInfoUser();
            Ruolo ruolo = RoleManager.GetRoleInSession();
            
            PrintReportRequestDataset request =
               new PrintReportRequestDataset()
               {
                   ContextName = "CasellaIstituzionale",
                   ReportType = ReportTypeEnum.PDF,
                   ReportKey = "RicercaCasellaIstituzionale",
                   Title = Utils.Languages.GetLabelFromCode("CheckMailboxReportLblIstitutionalBox", language),
                   SubTitle = String.Format(Utils.Languages.GetLabelFromCode("ReportGeneratorTbwSubtitle", language), MailCheckResponse.DtaConcluded.ToShortDateString(), MailCheckResponse.DtaConcluded.ToShortTimeString()),
                   SearchFilters = null,
                   UserInfo = userInfo,
                   InputDataset = reportDataSetDocGrigio
               };
            RequestPrintReport = request;
            this.ReportRegistry = CheckMailboxManager.GetReportRegistry(RequestPrintReport.ContextName);
            RequestPrintReport.ColumnsToExport = this.ReportRegistry.Where(r => r.ReportKey == request.ReportKey).First().ExportableFields;

            FileDocumento fd = CheckMailboxManager.GenerateReport(request);

            int retValue = 0;
            try
            {

                DocsPaWR.SchedaDocumento scheda = new SchedaDocumento();
                DocsPaWR.Oggetto ogg = new Oggetto();
                DocsPaWR.FileRequest fr = null;
                //Report Casella Istituzional del RF: codice/descrizione

                ogg.descrizione = String.Format("Report Casella Istituzionale {0} del {1} alle {2}", this.txtRegistro.Text, MailCheckResponse.DtaConcluded.ToShortDateString(), MailCheckResponse.DtaConcluded.ToShortTimeString());
                scheda.oggetto = ogg;
                scheda.personale = "0";
                scheda.privato = "0";
                scheda.userId = UserManager.GetInfoUser().userId;
                scheda.typeId = "LETTERA";
                scheda.tipoProto = "G";
                scheda.appId = "ACROBAT";
                scheda.idPeople = UserManager.GetInfoUser().idPeople;
                scheda =  CheckMailboxManager.DocumentoAddDocGrigia(scheda, UserManager.GetInfoUser(), RoleManager.GetRoleInSession());
                fr = (FileRequest)scheda.documenti[0];
                fr.docNumber = scheda.docNumber;
                fr = CheckMailboxManager.DocumentoPutFile(fr, fd, UserManager.GetInfoUser());
                if (fr != null) retValue = 1;
                //disabilito il pulsante di creazione doc grigio
                this.CheckMailboxReportCreateDoc.Enabled = false;
                if (trasmDoc)
                    CreateAndTrasmDoc(scheda, userInfo, ruolo);
            }
            catch (Exception ex) { retValue = 0; }
            return retValue;
         }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkResponse"></param>
        private void FetchMailAccountCheckInfo()
        {
            this.txtMailUserId.Text = MailCheckResponse.MailUserID;
            this.txtMailServer.Text = MailCheckResponse.MailServer;
            this.txtIndirizzoEmail.Text = MailCheckResponse.MailAddress;
            this.txtRegistro.Text = MailCheckResponse.Registro;
            string errorMessage = "OK";
            if (MailCheckResponse.ErrorMessage != string.Empty)
                errorMessage = MailCheckResponse.ErrorMessage;
            this.txtEsitoCasella.InnerText = errorMessage;
        }

        private void popolaDatiTblMailList()
        {
            this.tbl_MailTypelist.CssClass = "testo_grigio";
            this.tbl_MailTypelist.CellPadding = 1;
            this.tbl_MailTypelist.CellSpacing = 1;
            this.tbl_MailTypelist.BorderWidth = 1;
            this.tbl_MailTypelist.Attributes.Add("style", "'BORDER-RIGHT: 1px solid; BORDER-TOP: 1px solid; BORDER-LEFT: 1px solid; WIDTH: 60%; BORDER-BOTTOM: 1px solid'");
            this.tbl_MailTypelist.BackColor = Color.FromArgb(255, 255, 255);
            this.tbl_MailTypelist.ID = "table_Todolist";
            TableRow triga;
            TableCell tcell;

            triga = new TableRow();
            triga.Height = 15;
            triga.BackColor = Color.FromArgb(149, 149, 149);

            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(50);
            tcell.Text = typeMail;
            tcell.HorizontalAlign = HorizontalAlign.Right;
            tcell.ColumnSpan = 1;
            triga.Cells.Add(tcell);

            tcell = new TableCell();
            tcell.CssClass = "titolo_bianco";
            tcell.Width = Unit.Percentage(50);
            tcell.Text = number;
            tcell.HorizontalAlign = HorizontalAlign.Right;
            triga.Cells.Add(tcell);

            this.tbl_MailTypelist.Rows.Add(triga);
           
            foreach (string s in TipiMailRicevutaPEC.Keys)
            {
                triga = new TableRow();
                triga.Height = 15;
                triga.BackColor = Color.FromArgb(242, 242, 242);


                tcell = new TableCell();
                tcell.CssClass = "bg_grigioNP";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(50);
                tcell.Text = s;
                tcell.HorizontalAlign = HorizontalAlign.Right;
                triga.Cells.Add(tcell);

                tcell = new TableCell();
                tcell.CssClass = "bg_grigioNP";
                tcell.Font.Bold = true;
                tcell.Width = Unit.Percentage(50);
                //creo il link button
                LinkButton link = new LinkButton();
                link.ID = "link_" + s;
                link.ForeColor = Color.Black;
                link.Font.Underline = true;
                link.Font.Bold = true;
                link.Text = TipiMailRicevutaPEC[s].ToString();
                link.OnClientClick = "document.getElementById('" + tipo.ClientID + "').value = '" + s + "'; __doPostBack('panelGrdMails');return false;";
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
            tcell.Width = Unit.Percentage(50);
            tcell.Text = list;
            tcell.HorizontalAlign = HorizontalAlign.Right;
            triga.Cells.Add(tcell);
            tcell = new TableCell();
            tcell.CssClass = "bg_grigioNP";
            tcell.Font.Bold = true;
            tcell.Width = Unit.Percentage(50);
            //creo il link button
            LinkButton linkAll = new LinkButton();
            linkAll.ID = "link_all";
            linkAll.ForeColor = Color.Black;
            linkAll.Font.Underline = true;
            linkAll.Font.Bold = true;
            linkAll.Text = SetMailMessageCount();
            linkAll.OnClientClick = "document.getElementById('" + tipo.ClientID + "').value = 'all'; __doPostBack('panelGrdMails');return false;";
            tcell.HorizontalAlign = HorizontalAlign.Right;
            tcell.Controls.Add(linkAll);
            triga.Cells.Add(tcell);
            this.tbl_MailTypelist.Rows.Add(triga);

            //salvo in sessione le informazioni sulla griglia tbl_MailTypelist
            if (!IsPostBack)
            {
                TipoMailRicevutaPec = TipiMailRicevutaPEC;
            }
        }


        private string pecMessageResolver(MailProcessed mr)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            if (mr.ProcessedType == MailProcessedType.Signature)
                return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblPecWithSignature", language);
            switch (mr.PecXRicevuta)
            {


                case MailPecXRicevuta.PEC_Delivered_Notify:
                case MailPecXRicevuta.PEC_Delivered_Notify_Short:
                case MailPecXRicevuta.PEC_Delivered:
                    return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblDelivery", language);

                case MailPecXRicevuta.PEC_Accept_Notify:
                    return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblAcceptance", language);

                case MailPecXRicevuta.PEC_Alert_Virus:
                case MailPecXRicevuta.PEC_Contain_Virus:
                case MailPecXRicevuta.PEC_Error_Delivered_Notify_By_Virus:
                    return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblDetectionVirus", language);

                case MailPecXRicevuta.From_Non_PEC:
                case MailPecXRicevuta.PEC_Error:
                case MailPecXRicevuta.PEC_Error_Preavviso_Delivered_Notify:
                case MailPecXRicevuta.Delivery_Status_Notification:
                    return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblWithError", language);

                case MailPecXRicevuta.PEC_Non_Accept_Notify:
                    return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblFailureAcceptance", language);

                case MailPecXRicevuta.PEC_Presa_In_Carico:
                    return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblTakeCharge", language);

                case MailPecXRicevuta.PEC_Mancata_Consegna:
                    return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblFailureToDeliver", language);

                case MailPecXRicevuta.PEC_NO_XRicevuta:
                    return "Pec";

                case MailPecXRicevuta.unknown:
                default:
                    {
                        if (mr.ProcessedType == MailProcessedType.ConfirmReception)
                            return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblConfirmationReceipt", language);
                        if (mr.ProcessedType == MailProcessedType.Eccezione)
                            return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblException", language);
                        if (mr.ProcessedType == MailProcessedType.NotifyCancellation)
                            return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblCancelProtocol", language);
                        if (mr.ProcessedType == MailProcessedType.Signature)
                            return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblPecWithSignature", language);
                        if (mr.ProcessedType == MailProcessedType.NonPEC)
                            return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblNotPec", language);
                        if (mr.ProcessedType == MailProcessedType.Pec)
                            return "Pec";
                        return Utils.Languages.GetLabelFromCode("CheckMailboxReportLblOther", language);
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
            trasm.infoDocumento = DocumentManager.getInfoDocumento(scheda);
            trasm.ruolo = ruolo;
            trasm.tipoOggetto = TrasmissioneTipoOggetto.DOCUMENTO;
            trasm.utente = UserManager.GetUserInSession();

            trasmS.ragione = TrasmManager.GetReasonNotify(userInfo.idAmministrazione);
            trasmS.tipoDest = TrasmissioneTipoDestinatario.RUOLO;
            trasmS.tipoTrasm = "S";
            trasmS.corrispondenteInterno = UserManager.getCorrispondentBySystemID(ruolo.systemId);
            trasmU.daNotificare = true;
            trasmU.utente = UserManager.GetUserInSession();

            trasmS.trasmissioneUtente = new TrasmissioneUtente[1];
            trasmS.trasmissioneUtente[0] = trasmU;
            trasm.trasmissioniSingole = new TrasmissioneSingola[1];
            trasm.trasmissioniSingole[0] = trasmS;
            Trasmissione trasmRes = TrasmManager.saveExecuteTrasm(this.Page, trasm, userInfo);
        }

        #endregion
   
    }
}