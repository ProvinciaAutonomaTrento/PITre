using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using Amministrazione.Manager;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SiteNavigation;
using DocsPAWA.utils;
using System.Linq;

namespace DocsPAWA.AdminTool.Gestione_Organigramma
{
    public partial class SaveChangesToRole : System.Web.UI.Page, System.Web.UI.ICallbackEventHandler
    {
        /// <summary>
        /// Dettaglio del ruolo da spostare
        /// </summary>
        public static SaveChangesToRoleRequest SaveChangesRequest
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["SaveChangesRequest"] as SaveChangesToRoleRequest;
            }

            set
            {
                if (CallContextStack.CurrentContext == null)
                    CallContextStack.CurrentContext = new CallContext("SaveChangesRequest");
                CallContextStack.CurrentContext.ContextState["SaveChangesRequest"] = value;
            }

        }

        /// <summary>
        /// Report dell'elaborazione
        /// </summary>
        public static List<SaveChangesToRoleReport> Report
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["Report"] as List<SaveChangesToRoleReport>;
            }

            set
            {
                if (CallContextStack.CurrentContext == null)
                    CallContextStack.CurrentContext = new CallContext("SaveChangesRequest");
                CallContextStack.CurrentContext.ContextState["Report"] = value;
            }

        }

        /// <summary>
        /// Report dell'elaborazione
        /// </summary>
        public static OrganigrammaManager OrgManager
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["OrgManager"] as OrganigrammaManager;
            }

            set
            {
                if (CallContextStack.CurrentContext == null)
                    CallContextStack.CurrentContext = new CallContext("SaveChangesRequest");
                CallContextStack.CurrentContext.ContextState["OrgManager"] = value;
            }

        }

        /// <summary>
        /// Messaggio da visualizzare all'utente
        /// </summary>
        public static String MessageToShow
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["MessageToShow"] as String;
            }

            set
            {
                if (CallContextStack.CurrentContext == null)
                    CallContextStack.CurrentContext = new CallContext("SaveChangesRequest");
                CallContextStack.CurrentContext.ContextState["MessageToShow"] = value;
            }

        }

        /// <summary>
        /// Script per l'apertura della pagina di modifica a ruolo
        /// </summary>
        public static String ScriptToOpen
        {
            get
            {
                return String.Format(
                    "var retVal = window.showModalDialog('{0}/AdminTool/Gestione_Organigramma/SaveChangesToRole.aspx', '', 'dialogHeight: 270px; dialogWidth:750px; resizable: no;status:no;scroll:yes;help:no;close:no;center:yes;');Form1.hfRetValModSposta.value = retVal;document.forms[0].submit();",
                    Utils.getHttpFullPath());
            }
        }

        /// <summary>
        /// Script per l'apertura della pagina di export report
        /// </summary>
        public static String ReportScript
        {
            get
            {
                return ReportingUtils.GetOpenReportPageScript(false);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                this.RegisterCallBack(String.Empty);
                OrgManager = new OrganigrammaManager();
                Report = new List<SaveChangesToRoleReport>();
                MessageToShow = String.Empty;

            }

        }

        /// <summary>
        /// Metodo per la registrazione delle collback
        /// </summary>
        /// <param name="argument">Argomento da passare alla funzione</param>
        private void RegisterCallBack(String argument)
        {
            // Registrazione della funzione client che verrà richiamata dal server
            String callbackRef = Page.ClientScript.GetCallbackEventReference(this, String.Empty, "analyzeResult", String.Empty);

            // Registrazione della funzione client che verrà utilizzata dal client per richiamare il server
            String callbackScript = "function callServer() {" + callbackRef + ";}";
            ClientScript.RegisterClientScriptBlock(this.GetType(), "serverCalling", callbackScript, true);

        }

        /// <summary>
        /// Metodo chiamato dal client
        /// </summary>
        /// <param name="eventArgument">Parametri inviati dal client</param>
        public void RaiseCallbackEvent(string eventArgument) { }

        /// <summary>
        /// Analisi del risultato e invio della risposta al client sotto forma di stringa
        /// </summary>
        /// <returns>Risultato che il client analizzerà e su cui baserà le sue azioni</returns>
        public string GetCallbackResult()
        {
            // Esito dell'azione eseguita
            SaveChangesToRoleReportResult result = SaveChangesToRoleReportResult.OK;

            try
            {
                // Se questa è la prima volta che viene chiamata la callback (il report
                // non conterrà righe), viene eseguita l'inizializzazione del report
                // altrimenti viene eseguita l'azione, viene analizzato il risultato e,
                // se è il caso, si procede con la predisposizione del sistema ad eseguire
                // la prossima fase
                if (Report.Count == 0)
                    this.InitializeReport();
                else
                    result = this.ExecuteCurrentPhase();

                // Se la fase è finish viene chiusa l'ultima riga
                if (SaveChangesRequest.Phase == SaveChangesToRolePhase.Finish)
                    this.CloseReportVoice(SaveChangesToRoleReportResult.OK, String.Empty);


            }
            catch (Exception exception)
            {
                this.CloseReportForException();
            }

            return this.RenderReport(result);
        }

        /// <summary>
        /// Metodo per l'inizializzazione del report
        /// </summary>
        private void InitializeReport()
        {
            // Aggiunta della voce di inizializzazione
            this.AddNewReportRow();

            // Passaggio allo stato successivo
            SaveChangesToRoleResponse response = OrgManager.SaveChangesToRole(SaveChangesRequest);
            SaveChangesRequest.Phase = response.NextPhase;

        }

        /// <summary>
        /// Metodo per l'esecuzione della fase corrente, per l'analisi del
        /// risultato ottenuto, per l'aggiornamento del report e per l'eventuale
        /// aggiunta di una nuova riga di wait relativa alla prossima fase con conseguente
        /// aggiornamento della request
        /// </summary>
        /// <returns>Esito dell'operazione</returns>
        private SaveChangesToRoleReportResult ExecuteCurrentPhase()
        {
            // Esecuzione azione
            SaveChangesToRoleResponse response = OrgManager.SaveChangesToRole(SaveChangesRequest);

            // Analisi risultato e aggiornamento del report
            this.CloseReportVoice(this.AnalyzeResult(response.Result.Codice), response.Result.Descrizione);

            // Aggiornamento request
            SaveChangesRequest.IdOldRole = response.IdOldRole;
            SaveChangesRequest.IdCorrGlobOldRole = response.IdCorrGlobOldRole;
            SaveChangesRequest.Users = response.Users;
            SaveChangesRequest.ModifiedRole = response.ModifiedRole;
            SaveChangesRequest.ComputeAtipicita = response.ComputeAtipicita;
            MessageToShow = response.MessageToShowToAdministrator;
            SaveChangesRequest.IdOldRoleType = response.IdOldRoleType;
            SaveChangesRequest.IdOldUO = response.IdOldUO;

            // Aggiunta di una nuova voce al report solo se il risultato dell'azione corrente non è KO
            if (this.AnalyzeResult(response.Result.Codice) != SaveChangesToRoleReportResult.KO)
                this.AddNewReportRow();

            SaveChangesRequest.Phase = response.NextPhase;

            return this.AnalyzeResult(response.Result.Codice);

        }

        /// <summary>
        /// Metodo per l'aggiunta di una nuova voce al report (voce relativa alla prossima azione da compiere)
        /// </summary>
        private void AddNewReportRow()
        {

            // Analisi della fase da compiere ed esecuzione dell'azione corrispondente
            switch (SaveChangesRequest.Phase)
            {
                case SaveChangesToRolePhase.Start:
                    // Inizializzazione del report
                    Report = new List<SaveChangesToRoleReport>();

                    // Aggiunta della voce di inizializzazione
                    this.AddWorkInProgressItemToReport("Inizializzazione operazione di modifica ruolo");

                    break;
                case SaveChangesToRolePhase.Initialize:
                    if (SaveChangesRequest.Historicize)
                        this.AddWorkInProgressItemToReport("Pulizia della lista delle cose da fare relativa agli utenti del ruolo");
                    else
                        this.AddWorkInProgressItemToReport("Salvataggio modifiche al ruolo");
                    break;
                case SaveChangesToRolePhase.CleanToDoList:
                    this.AddWorkInProgressItemToReport("Salvataggio modifiche al ruolo");
                    break;
                case SaveChangesToRolePhase.SaveChanges:

                    if (SaveChangesRequest.ExtendVisibility == ExtendVisibilityOption.N)
                        if (Utils.GetAbilitazioneAtipicita() && (SaveChangesRequest.IdOldRoleType != SaveChangesRequest.ModifiedRole.IDTipoRuolo || ((SaveChangesRequest.IdOldUO != SaveChangesRequest.ModifiedRole.IDUo) && SaveChangesRequest.ComputeAtipicita)))
                            this.AddWorkInProgressItemToReport("Calcolo dell'atipicità su documenti e fascicoli");
                        else
                            this.AddWorkInProgressItemToReport("Aggiornamento ruolo proprietario delle liste di distribuzione");
                    else
                        this.AddWorkInProgressItemToReport("Estensione della visibilità ai ruoli superiori");
                    break;
                case SaveChangesToRolePhase.ExtendVisibility:

                    if (Utils.GetAbilitazioneAtipicita() && (SaveChangesRequest.IdOldRoleType != SaveChangesRequest.ModifiedRole.IDTipoRuolo || SaveChangesRequest.IdOldUO != SaveChangesRequest.ModifiedRole.IDUo))
                        this.AddWorkInProgressItemToReport("Calcolo dell'atipicità su documenti e fascicoli");
                    else
                        this.AddWorkInProgressItemToReport("Aggiornamento ruolo proprietario delle liste di distribuzione");

                    break;
                case SaveChangesToRolePhase.CalculateAtipicita:
                    this.AddWorkInProgressItemToReport("Aggiornamento ruolo proprietario delle liste di distribuzione");

                    break;
                case SaveChangesToRolePhase.UpdateLists:
                    if (!SaveChangesRequest.UpdateTransModelsAssociation)
                        this.AddWorkInProgressItemToReport("Aggiornamento modelli di trasmissione");
                    else
                        this.AddWorkInProgressItemToReport("Conclusione operazione di modifica");
                    break;
                case SaveChangesToRolePhase.UpdateTransmissionModelsAssociation:
                    this.AddWorkInProgressItemToReport("Conclusione operazione di modifica");
                    break;

            }

        }

        /// <summary>
        /// Metodo per l'aggiunta di una voce di "lavori in corso" al report
        /// </summary>
        /// <param name="description">Descrizione associata alla riga del report</param>
        private void AddWorkInProgressItemToReport(String description)
        {
            Report.Add(new SaveChangesToRoleReport()
                {
                    Description = description,
                    Result = SaveChangesToRoleReportResult.Waiting,
                    ImageUrl = "Images/wait.gif"
                });
        }

        /// <summary>
        /// Metodo per la renderizzazione del report. Questa funzione aggiunge anche un eventuale messaggio di richiesta
        /// da mostrare all'amministratore
        /// </summary>
        /// <param name="result">Risultato dell'ultima azione eseguita. Viene utilizzato per decidere se chiudere o meno il report</param>
        /// <returns>Renderizzazione del datagrid cui è stato bindato il report</returns>
        private string RenderReport(SaveChangesToRoleReportResult result)
        {
            // Associazione della sorgente dati al datagrid
            this.dgReport.DataSource = Report;
            this.dgReport.DataBind();

            // Response da restituire al client
            StringBuilder response;
            // Il primo carattere della response sarà un numero che assumerà il valore 0 se la fase corrente è Finish o
            // se result è KO altrimenti assumerà il valore 1 se l'operazione ancora deve continuare o 2 se bisogna
            // mostrare una richiesta all'amministratore. Questo valore sarà utilizzato dal client per decidere
            // se deve richiamare di nuovo il server o se l'operazione si è conclusa.
            if (result != SaveChangesToRoleReportResult.KO && !String.IsNullOrEmpty(MessageToShow))
            {
                response = new StringBuilder("2");
                response.AppendFormat("{0}||", MessageToShow);
            }
            else
                response = new StringBuilder(result != SaveChangesToRoleReportResult.KO && SaveChangesRequest.Phase != SaveChangesToRolePhase.Finish ? "1" : "0");

            // Inizializzazione degli oggetti necessari alla creazione della stringa HTML da inviare al client
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(stringWriter);

            // Renderizzazione del datagrid del report
            this.dgReport.RenderControl(writer);

            // Flushing del writer e finalizzazione del prodotto di renderizzazione
            stringWriter.Flush();
            response.Append(stringWriter.GetStringBuilder());
            stringWriter.Close();

            // Restituzione del report renderizzato
            return response.ToString();
        }

        /// <summary>
        /// Funzione per l'analisi dell'ultimo risultato
        /// </summary>
        /// <param name="code">Codice dell'esito da analizzare</param>
        /// <returns>Esito dell'operazione</returns>
        private SaveChangesToRoleReportResult AnalyzeResult(int code)
        {
            return code == 0 ? SaveChangesToRoleReportResult.OK : SaveChangesToRoleReportResult.KO;
        }

        /// <summary>
        /// Metodo per la chiusura di un fase di lavoro
        /// </summary>
        /// <param name="result">Risultato dell'azione precedente</param>
        /// <param name="details">Dettagli sul risultato precedente</param>
        private void CloseReportVoice(SaveChangesToRoleReportResult result, String details)
        {
            if (Report[Report.Count - 1].Result == SaveChangesToRoleReportResult.Waiting || Report[Report.Count - 1].Result != SaveChangesToRoleReportResult.KO)
                Report[Report.Count - 1].Result = result;
            Report[Report.Count - 1].Description += "<br />" + details;
            Report[Report.Count - 1].ImageUrl = Report[Report.Count - 1].Result == SaveChangesToRoleReportResult.OK ? "Images/completed.jpg" : "Images/failed.jpg";

            // Se la fase è Finish o se il risultato dell'operazione è un errore, viene inizializzato il report
            if (SaveChangesRequest.Phase == SaveChangesToRolePhase.Finish || result == SaveChangesToRoleReportResult.KO)
                ReportingUtils.PrintRequest = new PrintReportObjectTransformationRequest()
                    {
                        DataObject = Report.ToArray(),
                        ContextName = "ModificaRuolo",
                        ReportKey = "ModificaRuolo"
                    };

            //ABBATANGELI-PANICI LIBRO FIRMA
            if (result == SaveChangesToRoleReportResult.OK)
            {
                InvalidaPassiCorrelati();
            }
            //FINE
        }

        private void InvalidaPassiCorrelati()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            wws.Timeout = System.Threading.Timeout.Infinite;
            List<ProcessoFirma> processiCoinvolti_R = (Session["processiCoinvolti_R"] != null && ((int)Session["processiCoinvolti_R"]) > 0? wws.GetProcessiDiFirmaByRuoloTitolare(SaveChangesRequest.ModifiedRole.IDGruppo).ToList() : new List<ProcessoFirma>());
            List<IstanzaProcessoDiFirma> istazaProcessiCoinvolti_R = (Session["istazaProcessiCoinvolti_R"] != null && ((int)Session["istazaProcessiCoinvolti_R"]) > 0? wws.GetIstanzeProcessiDiFirmaByRuoloCoinvolto(SaveChangesRequest.ModifiedRole.IDGruppo).ToList() : new List<IstanzaProcessoDiFirma>());

            if (processiCoinvolti_R.Count > 0)
            {
                List<string> idPassi = new List<string>();
                foreach (ProcessoFirma processo in processiCoinvolti_R)
                {
                    foreach(PassoFirma passo in processo.passi) 
                    {
                        if (!idPassi.Contains(passo.idPasso))
                        {
                            idPassi.Add(passo.idPasso);
                        }
                    }
                }

                wws.TickPasso(idPassi.ToArray(), "R");
                Session["processiCoinvolti_R"] = null;
            }

            if (istazaProcessiCoinvolti_R.Count > 0)
            {
                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                wws.TickIstanze(istazaProcessiCoinvolti_R.ToArray(), "R", sessionManager.getUserAmmSession());
                Session["istazaProcessiCoinvolti_R"] = null;
            }
        }

        /// <summary>
        /// Metodo per la chiusura di un report dovuta ad eccezioni
        /// </summary>
        private void CloseReportForException()
        {
            // Se non ci sono voci di report ne viene aggiunta una
            if (Report.Count == 0)
                Report.Add(new SaveChangesToRoleReport());

            // Chiusura dell'attuale voce di report con un KO
            this.CloseReportVoice(SaveChangesToRoleReportResult.KO, "Errore non gestito");

            // La fase da eseguire è finish
            SaveChangesRequest.Phase = SaveChangesToRolePhase.Finish;

        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            ReportingUtils.PrintRequest = null;

            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "ClosePage",
                String.Format("window.returnValue='{0}_{1}';window.close();", SaveChangesRequest.ModifiedRole.IDCorrGlobale, SaveChangesRequest.ModifiedRole.IDUo),
                true);
        }

    }

}
