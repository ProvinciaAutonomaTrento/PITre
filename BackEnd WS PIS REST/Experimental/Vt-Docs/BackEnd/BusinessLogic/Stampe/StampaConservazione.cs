using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;
using DocsPaVO.filtri;
using DocsPaVO.utente;
using DocsPaVO.utente.RegistroConservazione;
using DocsPaDB.Query_DocsPAWS;
using BusinessLogic.Documenti;
using log4net;


namespace BusinessLogic.Stampe
{

    /// <summary>
    /// Classe per la gestione del processo di stampa automatica del registro di conservazione.
    /// </summary>
    public class StampaConservazione
    {

        private static ILog logger = LogManager.GetLogger(typeof(StampaConservazione));

        /// <summary>
        /// Metodo da schedulare che verifica se ci sono amministrazioni con stampa abilitata
        /// e, se è stata raggiunta la data di prossima stampa, la esegue.
        /// </summary>
        public void GeneratePrintRegCons()
        {

            logger.Debug("stampa registro di conservazione - START");
            //check per vedere se esistono amministrazioni con registro di conservazione abilitato
            if (this.CheckRegAbilitato())
            {
                
                RegistroConservazionePrintManager manager = new RegistroConservazionePrintManager();
                List<RegistroConservazionePrint> printRanges = manager.GetRegConsPrintRange();

                //loop in cui recupero info e le inserisco in un oggetto
                foreach (var range in printRanges)
                {
                    logger.Debug(string.Format("ID AMM={0} - check", range.idAmministrazione));
                    //check se data prossima stampa è >= data attuale (e se esistono record da stampare)
                    if (manager.VerifyNextPrint(range.nextPrintDate, range.idAmministrazione, range.idLastPrinted, range.printHour))
                    {

                        try
                        {
                            //metodo che esegue la stampa
                            PrintRegistro(range);

                            //aggiorna la data di prossima stampa automatica e l'ultimo documento/istanza stampati
                            if (!manager.UpdateNextPrintDate(range))
                                throw new Exception("Errore durante l'aggiornamento delle impostazioni di stampa.");

                            logger.Debug("termine stampa ID AMM=" + range.idAmministrazione);
                        }
                        catch (Exception exc)
                        {

                            logger.Debug("Errore nella stampa del registro per amm ID=" + range.idAmministrazione + ": ", exc);
                        }
                    }
                    /*
                    else
                    {
                        throw new Exception(string.Format("Prossima data di stampa: {0}", range.nextPrintDate.ToString("dd/MM/yyyy")));
                        
                    }
                    */
                }
            }
        }

        /// <summary>
        /// Metodo che esegue la stampa del registro relativo ad una amministrazione
        /// </summary>
        public void PrintRegistro(RegistroConservazionePrint infoPrint)
        {

            logger.Debug("Avvio Stampa - DATI:");
            logger.Debug(string.Format("freq={0}, idrange={1}-{2},  daterange={3}-{4}, user={5}, role={6}",
                infoPrint.printFreq, infoPrint.idLastPrinted, infoPrint.idLastToPrint,
                infoPrint.lastPrintDate.ToString("dd/MM/yyyy"), infoPrint.nextPrintDate.ToString("dd/MM/yyyy"),
                infoPrint.print_userId, infoPrint.print_role));

            RegistroConservazionePrintManager manager = new RegistroConservazionePrintManager();

            //creo un oggetto InfoUtente
            InfoUtente userInfo = this.GetInfoUtenteStampa(infoPrint);

            //creo un ruolo dall'id_gruppo
            //GM 23-7-2013: se non ho un ruolo definito nella tabella di configurazione
            //uso un valore fittizio per evitare errori nell'assegnazione delle visibilità
            Ruolo role = new Ruolo();
            role.idGruppo = "0";
            if(!string.IsNullOrEmpty(infoPrint.print_role))            
                role = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoPrint.print_role);            
            

            //definisco filtri per la ricerca come nella stampa dei repertori
            List<FiltroRicerca> filters = new List<FiltroRicerca>()
            {
                new FiltroRicerca() {argomento = "id_amm", valore = infoPrint.idAmministrazione},       //id amministrazione
                new FiltroRicerca() {argomento = "next_system_id", valore = infoPrint.idLastPrinted},   //ultimo id stampato
                new FiltroRicerca() {argomento = "last_system_id", valore = infoPrint.idLastToPrint}    //ultimo id da stampare
            };

            //ricavo la frequenza di stampa da stampare nel report
            string printFreq = manager.GetPrintFreq(infoPrint.printFreq);

            //creo la stampa utilizzando i metodi esistenti
            //va definita la classe con il report in businesslogic.reporting (StampeRegistroConservazioneReportGeneratorCommand)
            //e le modalità di estrazione dei dati in docspadb.query_docspaws.reporting.
            FileDocumento fileDocument = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(
                new DocsPaVO.Report.PrintReportRequest()
                {
                    UserInfo = userInfo,
                    SearchFilters = filters,
                    ReportType = DocsPaVO.Report.ReportTypeEnum.PDF,
                    ReportKey = "StampaRegistroConservazione",
                    ContextName = "StampaRegistroConservazione",
                    AdditionalInformation = String.Format("Stampa del {0} (frequenza di stampa {1})\n\n",DateTime.Now.ToString("dd/MM/yyyy"),printFreq)
                }).Document;

            //inserimento stampa nel gestore documentale

            //apertura contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                if (userInfo != null && string.IsNullOrEmpty(userInfo.dst))
                    userInfo.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

                //creazione scheda documento
                SchedaDocumento document = this.InitializeDocument(userInfo, infoPrint.lastPrintDate, DateTime.Now);

                Ruolo[] roles = new Ruolo[] { };
                //salvataggio documento
                
                string docNumber = this.SaveDocument(userInfo, role, document, fileDocument, out roles);

                if (String.IsNullOrEmpty(docNumber))
                    throw new Exception("Errore durante la creazione della stampa");

                //modifico il tipo_proto del documento inserito in "M"
                if (!manager.UpdateTipoProto(docNumber))
                    throw new Exception("Errore nell'aggiornamento della tipologia protocollo"); 

                //inserisco la stampa nel registro delle stampe
                if (!manager.UpdateRegStampeCons(infoPrint, docNumber))
                    throw new Exception("Errore durante l'aggiornamento dell'anagrafica delle stampe effettuate");

                //aggiorno il campo corrispondente del registro di conservazione
                if (!manager.UpdatePrintedRecords(infoPrint))
                    throw new Exception("Errore nell'aggiornamento del registro di conservazione");
                
                //chiusura transazione
                transactionContext.Complete();

            }

        }

        /// <summary>
        /// Verifica se ci sono amministrazioni con registro di conservazione enabled.
        /// </summary>
        /// <returns></returns>
        private bool CheckRegAbilitato()
        {

            RegistroConservazionePrintManager manager = new RegistroConservazionePrintManager();
            bool retVal = false;

            if (Convert.ToInt32(manager.GetEnabledRegCons()) > 0)
            {
                retVal = true;
            }

            return retVal;

        }

        /// <summary>
        /// Costruisce un oggetto InfoUtente a partire dagli id recuperati
        /// dalla tabella di configurazione stampa
        /// </summary>
        /// <param name="reg">Oggetto RegistroConservazionePrint contentente user_id e id_gruppo</param>
        /// <returns></returns>
        private InfoUtente GetInfoUtenteStampa(RegistroConservazionePrint reg)
        {

            DocsPaVO.utente.Utente u = Utenti.UserManager.getUtenteById(reg.print_userId);
            DocsPaVO.utente.Ruolo r = Utenti.UserManager.getRuoloByIdGruppo(reg.print_role);

            InfoUtente retVal = Utenti.UserManager.GetInfoUtente(u, r);

            return retVal;

        }

        private SchedaDocumento InitializeDocument(InfoUtente userInfo, DateTime startDate, DateTime endDate)
        {
            //inizializzazione scheda documento
            SchedaDocumento retVal = Documenti.DocManager.NewSchedaDocumento(userInfo);

            //se non ho ruoli imposto il documento come personale
            if (string.IsNullOrEmpty(userInfo.idGruppo))
                retVal.personale = "1";


            //impostazione delle proprietà della scheda
            retVal.appId = ((DocsPaVO.documento.Applicazione)BusinessLogic.Documenti.FileManager.getApplicazioni("PDF")[0]).application;
            retVal.idPeople = userInfo.idPeople;
            retVal.userId = userInfo.userId;
            retVal.typeId = "LETTERA";

            //oggetto documento
            retVal.oggetto = new Oggetto() { descrizione = String.Format("Stampa registro conservazione - da {0} a {1}",startDate.ToString("dd/MM/yyyy"),endDate.ToString("dd/MM/yyyy")) };

            //tipologia documento
            retVal.tipoProto = "M";

            return retVal;
        }

        private string SaveDocument(InfoUtente userInfo, Ruolo role, SchedaDocumento document, FileDocumento fileDoc, out Ruolo[] superiori)
        {

            //Salvataggio del documento
            DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(userInfo);

            //Salvataggio dell'oggetto
            document = ProtoManager.addOggettoLocked(userInfo.idAmministrazione, document);

            Ruolo[] ruoliSuperiori;
            //if (docManager.CreateDocumentoStampaRegistro(document, role, out ruoliSuperiori))
            if (docManager.CreateDocumentoGrigio(document, role, out ruoliSuperiori))
            //if(docManager.CreateDocumentoGrigio(document, role))
            {
                //Notifica evento documento creato
                //DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(userInfo);
                //eventsNotification.DocumentoCreatoEventHandler(document, role, ruoliSuperiori);
                
                //Salvataggio del file associato al documento
                DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)document.documenti[0];

                fileRequest = BusinessLogic.Documenti.FileManager.putFile(fileRequest, fileDoc, userInfo);
                if(fileRequest == null)
                    throw new ApplicationException("Si è verificato un errore nell'upload del documento per la stampa del registro di conservazione");

            }

            superiori = ruoliSuperiori;
            if (superiori == null)
                superiori = new Ruolo[] { };

            //Restituzione del numero documento            
            return document.docNumber;
        }


    }
}
