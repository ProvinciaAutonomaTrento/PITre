using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;
using DocsPaVO.filtri;
using DocsPaVO.utente;
using DocsPaVO.utente.Repertori;
using BusinessLogic.Documenti;
using DocsPaDB.Query_DocsPAWS;
using log4net;

namespace BusinessLogic.Stampe
{
    /// <summary>
    /// Classe per la gestione del processo di generazione di una stampa di repertorio
    /// </summary>
    public class StampaRepertorio
    {
        private ILog logger = LogManager.GetLogger(typeof(StampaRepertorio));
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <param name="userInfo"></param>
        /// <param name="role"></param>
        public void GeneratePrintRepertorio(String counterId, String registryId, String rfId, InfoUtente userInfo, Ruolo role)
        {
            // Risultato dell'operazione
            //SchedaDocumento document = null;

            // Controllo dello stato del repertorio per la particolare istanza
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();

            String message;
            // Si può stampare solo se l'istanza è chiusa
            if (this.CheckPrintCondition(counterId, registryId, rfId/*, out lastPrintedNumber, out lastNumberToPrint*/, out message))
            {
                // Recupero delle informazioni sui repertori da stampare
                List<RepertorioPrintRange> ranges = manager.GetRepertoriPrintRanges(counterId, registryId, rfId,false);

                foreach (var range in ranges)
                {
                    GeneratePrint(counterId, registryId, rfId, userInfo, role, range.Year.ToString(), range.FirstNumber.ToString(), range.LastNumber.ToString());

                    if (!manager.UpdateLastPrintendNumber(counterId, range.LastNumber.ToString(), rfId, registryId))
                        throw new Exception("Errore durante l'aggiornamento dell'anagrafica dei repertori");
                }
            }
            else
                throw new Exception(message);

            // Restituzione documento creato
            //return document;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <param name="userInfo"></param>
        /// <param name="role"></param>
        /// <param name="year"></param>
        /// <param name="lastPrintedNumber"></param>
        /// <param name="lastNumberToPrint"></param>
        public void GeneratePrint(String counterId, String registryId, String rfId, InfoUtente userInfo, Ruolo role, String year, String lastPrintedNumber, String lastNumberToPrint)
        {
            RegistriRepertorioPrintManager manager = new RegistriRepertorioPrintManager();

            // Generazione filtri per la stampa
            List<FiltroRicerca> filters = new List<FiltroRicerca>()
                {
                    new FiltroRicerca() { argomento = "idCounter", valore = counterId },
                    new FiltroRicerca() { argomento ="idRegistry", valore = registryId },
                    new FiltroRicerca() { argomento = "idRf", valore = rfId },
                    new FiltroRicerca() { argomento = "year", valore = year},
                    new FiltroRicerca() { argomento = "lastPrintedNumber", valore = lastPrintedNumber },
                    new FiltroRicerca() { argomento = "lastNumberToPrint", valore = lastNumberToPrint }
                };

            // Creazione della stampa
            FileDocumento fileDocument = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(
                new DocsPaVO.Report.PrintReportRequest()
                {
                    UserInfo = userInfo,
                    SearchFilters = filters,
                    ReportType = DocsPaVO.Report.ReportTypeEnum.PDF,
                    ReportKey = "StampaRegistriRepertori",
                    ContextName = "StampaRegistriRepertori",
                    AdditionalInformation = String.Format("Stampa del repertorio per l'anno {0} dal n. {1} al n. {2}", year, Convert.ToInt32(lastPrintedNumber), lastNumberToPrint)
                }).Document;

            // Apertura di un contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {

                if (userInfo != null && string.IsNullOrEmpty(userInfo.dst))                
                    userInfo.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

                // Reperimento del nome della tipologia
                string tipologia = manager.GetNomeTipologia(counterId);

                // Creazione della scheda documento
                SchedaDocumento document = InitializeDocument(userInfo, registryId, rfId, year, lastPrintedNumber, lastNumberToPrint, tipologia);

                Ruolo[] roles = new Ruolo[] { };
                // Salvataggio del documento
                String docNumber = this.SaveDocument(userInfo, role, document, fileDocument, out roles);

                if (String.IsNullOrEmpty(docNumber))
                    throw new Exception("Errore durante la creazione della stampa");

                // Aggiornamento del registro delle stampe e del prossimo numero da stampare
                if (!manager.UpdatePrinterManager(counterId, lastPrintedNumber, lastNumberToPrint, docNumber, registryId, rfId, year))
                    throw new Exception("Errore durante l'aggiornamento dell'anagrafica delle stampe effettuate");
                int rights = 0;
                // Assegnazione della visibilità della stampa al responsabile di repertorio (se impostato)
                if (!BusinessLogic.utenti.RegistriRepertorioPrintManager.AssignDocumentVisibilityToResponsable(userInfo, docNumber, counterId, rfId, registryId, out rights))
                    throw new Exception("Errore assegnazione visiblità a ruolo responsabile");

                // Se ci sono ruoli superiori, viene assegnata la visibilità
                foreach (Ruolo r in roles)
                    if (!BusinessLogic.utenti.RegistriRepertorioPrintManager.AssignDocumentVisibilityToRole(userInfo, docNumber, counterId, rfId, registryId, r.idGruppo,rights))
                        throw new Exception(String.Format("Errore assegnazione visiblità al ruolo {0}", r.codice));

                //assegnazione visibilità nel caso di repertori di tipologia/RF
                if ((string.IsNullOrEmpty(rfId) && string.IsNullOrEmpty(registryId)) ||
                    (!string.IsNullOrEmpty(rfId)))
                {
                    DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                    object[] supRolePrinter = gerarchia.getGerarchiaSup(role, ((DocsPaVO.utente.Registro)role.registri[0]).systemId, null, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO).ToArray();
                    foreach (Ruolo r in supRolePrinter)
                        if (!BusinessLogic.utenti.RegistriRepertorioPrintManager.AssignDocumentVisibilityToRole(userInfo, docNumber, counterId, rfId, registryId, r.idGruppo,rights))
                            throw new Exception(String.Format("Errore assegnazione visiblità al ruolo {0}", r.codice));
                }

                // Invio in conservazione
                Conservazione.ConservazioneManager cons = new Conservazione.ConservazioneManager();
                if (cons.GetStatoAttivazione(userInfo.idAmministrazione) && this.IsStampaRepDaConservare(counterId))
                {
                    this.InviaInConservazione(docNumber, userInfo.idAmministrazione);
                }

                // Completamento transazione
                transactionContext.Complete();

            }
        }

        /// <summary>
        /// Metodo per la verifica delle condizioni per l'avvio di una stampa. Attualmente le condizioni sono che
        /// il registro sia chiuso e che ci siano numeri da stampare.
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <param name="checkError"></param>
        /// <returns>False se almeno una delle condizioni fallisce</returns>
        public bool CheckPrintCondition(String counterId, String registryId, String rfId/*, out String lastPrintedNumber, out String lastNumberToPrint*/, out String checkError)
        {
            // Il repertorio deve essere chiuso
            RegistriRepertorioPrintManager manager = new RegistriRepertorioPrintManager();
            RegistroRepertorioSingleSettings.RepertorioState state = manager.GetState(counterId, registryId, rfId);//, out lastPrintedNumber, out lastNumberToPrint);

            // Ci potrebbero essere dei documenti repertoriati modificati dopo l'ultima stampa
            //bool modified = manager.ExistsModifiedDocsAfterLastPrint(counterId, registryId, rfId);

            bool retVal = true;
            checkError = String.Empty;
            if (state == RegistroRepertorioSingleSettings.RepertorioState.O)
            {
                retVal = false;
                checkError = "Il registro di repertorio deve essere chiuso";
            }
            else
                // Se l'ultimo numero stampato è uguale all'ultimo numero staccato e se non
                // esistono documenti modificati dopo la data dell'ultima stampa, significa che non
                // ci sono documenti da stampare
                //if ((lastPrintedNumber == lastNumberToPrint)) /*&& !modified)*/
                if (!manager.ExistsRepertoriToPrint(counterId, registryId, rfId))
                {
                    retVal = false;
                    checkError = "Non ci sono repertori da stampare";
                }

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <param name="year"></param>
        /// <param name="startNum"></param>
        /// <param name="endNum"></param>
        /// <returns></returns>
        private SchedaDocumento InitializeDocument(InfoUtente userInfo, string registryId, string rfId, String year, String startNum, String endNum, string repName)
        {
            // Inizializzazione scheda documento vuota
            SchedaDocumento retVal = Documenti.DocManager.NewSchedaDocumento(userInfo);

            // Inizializzazione del registro 
            DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();

            if (String.IsNullOrEmpty(registryId) && String.IsNullOrEmpty(rfId))
                reg.systemId = "0";
            else
                reg.systemId = String.IsNullOrEmpty(registryId) ? rfId : registryId;

            retVal.registro = reg;

            // Impostazione proprietà varie della scheda documento
            retVal.appId = ((DocsPaVO.documento.Applicazione)BusinessLogic.Documenti.FileManager.getApplicazioni("PDF")[0]).application;
            retVal.idPeople = userInfo.idPeople;
            retVal.userId = userInfo.userId;
            retVal.typeId = "LETTERA";

            // Impostazione oggetto del documento
            if (!string.IsNullOrEmpty(repName))
                retVal.oggetto = new Oggetto() { descrizione = String.Format("Giornale di Repertorio {3} - Anno {0} - dal rep. n. {1} al rep. n. {2}", year, Convert.ToInt32(startNum), endNum, repName) };
            else
                retVal.oggetto = new Oggetto() { descrizione = String.Format("Giornale di Repertorio - Anno {0} - dal rep. n. {1} al rep. n. {2}", year, Convert.ToInt32(startNum), endNum) };

            // Tipologia documento: Stampa repertorio
            retVal.tipoProto = "C";

            // Restituzione della scheda inizializzata
            return retVal;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="role"></param>
        /// <param name="document"></param>
        /// <param name="fileDocument"></param>
        /// <param name="superiori"></param>
        /// <returns></returns>
        private string SaveDocument(InfoUtente userInfo, Ruolo role, SchedaDocumento document, FileDocumento fileDocument, out Ruolo[] superiori)
        {
            // Salvataggio del documento
            DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(userInfo);

            // Salvataggio dell'oggetto
            document = ProtoManager.addOggettoLocked(userInfo.idAmministrazione, document);

            Ruolo[] ruoliSuperiori;
            if (docManager.CreateDocumentoStampaRegistro(document, role, out ruoliSuperiori))
            {
                // Notifica evento documento creato
                DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(userInfo);
                eventsNotification.DocumentoCreatoEventHandler(document, role, ruoliSuperiori);

                // Salvataggio del file associato al documento
                DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)document.documenti[0];

                fileRequest = BusinessLogic.Documenti.FileManager.putFile(fileRequest, fileDocument, userInfo);

                if (fileRequest == null)
                {
                    string outmessage = "";
                    BusinessLogic.Documenti.DocManager.CestinaDocumento(userInfo, document, "", "Errore nella creazione della stampa", out outmessage);
                    throw new ApplicationException("Si è verificato un errore nell'upload del documento per la stampa del registro di repertorio");
                }
            }

            // Restituzione del numero di documento
            superiori = ruoliSuperiori;
            if (superiori == null)
                superiori = new Ruolo[] { };
            return document.docNumber;
        }

        public void GeneratePrintByYear(string counterId, string year)
        {
            logger.Info("START");
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();

            List<RegistroRepertorioPrint> reg1 = manager.GetRegistersToPrintByCounter(counterId);

            foreach (RegistroRepertorioPrint print in reg1)
            {
                logger.DebugFormat("Stampa del repertorio {0} della tipologia {5}, per l'anno {1}, su registro {2} o RF {3}, responsabile {4}", 
                    print.CounterDescription, year, print.RegistryId,print.RFId, print.PrinterUser.userId, print.TipologyDescription);
                InfoUtente userInfo = new InfoUtente()
                {
                    idAmministrazione = print.PrinterUser.idAmministrazione,
                    idCorrGlobali = print.PrinterUser.systemId,
                    idGruppo = print.PrinterRole.idGruppo,
                    idPeople = print.PrinterUser.idPeople,
                    userId = print.PrinterUser.userId
                };

                RepertorioPrintRange rpr = manager.GetRepertoriPrintRangesByYear(counterId, print.RegistryId, print.RFId, year);
                // Generazione filtri per la stampa
                if (rpr != null && rpr.LastNumber != null && rpr.LastNumber != 0 && (rpr.LastNumber - rpr.FirstNumber) > 0)
                {
                    string minX = rpr.FirstNumber.ToString();
                    string maxX = rpr.LastNumber.ToString();
                    logger.DebugFormat("Limite inferiore {0}, superiore {1}", minX, maxX);

                    List<FiltroRicerca> filters = new List<FiltroRicerca>()
                    {
                    new FiltroRicerca() { argomento = "idCounter", valore = counterId },
                    new FiltroRicerca() { argomento ="idRegistry", valore = print.RegistryId },
                    new FiltroRicerca() { argomento = "idRf", valore = print.RFId },
                    new FiltroRicerca() { argomento = "year", valore = year},
                    new FiltroRicerca() { argomento = "lastPrintedNumber", valore = minX },
                    new FiltroRicerca() { argomento = "lastNumberToPrint", valore = maxX }
                    };

                    // Creazione della stampa
                    FileDocumento fileDocument = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(
                        new DocsPaVO.Report.PrintReportRequest()
                        {
                            UserInfo = userInfo,
                            SearchFilters = filters,
                            ReportType = DocsPaVO.Report.ReportTypeEnum.PDF,
                            ReportKey = "StampaRegistriRepertori",
                            ContextName = "StampaRegistriRepertori",
                            AdditionalInformation = String.Format(" Stampa del repertorio per l'anno {0} dal n. {1} al n. {2}. La presente stampa riporta tutte le registrazioni giornaliere di repertorio nell'intervallo indicato. Si è resa necessaria in quanto problematiche tecniche del sistema avevano inizialmente impedito la regolare stampa.", year, minX, maxX)
                        }).Document;

                    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                    {

                        if (userInfo != null && string.IsNullOrEmpty(userInfo.dst))
                            userInfo.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

                        // Reperimento del nome della tipologia
                        string tipologia = manager.GetNomeTipologia(counterId);

                        // Creazione della scheda documento
                        SchedaDocumento document = InitializeDocument(userInfo, print.RegistryId, print.RFId, year, minX, maxX, tipologia);

                        document.oggetto.descrizione += " - RECUPERO";
                        Ruolo[] roles = new Ruolo[] { };
                        // Salvataggio del documento
                        String docNumber = this.SaveDocument(userInfo, print.PrinterRole, document, fileDocument, out roles);

                        if (String.IsNullOrEmpty(docNumber))
                            throw new Exception("Errore durante la creazione della stampa");
                        // Aggiornamento del registro delle stampe e del prossimo numero da stampare
                        if (!manager.UpdatePrinterManager(counterId, minX, maxX, docNumber, print.RegistryId, print.RFId, year))
                            throw new Exception("Errore durante l'aggiornamento dell'anagrafica delle stampe effettuate");
                
                        int rights = 0;
                        // Assegnazione della visibilità della stampa al responsabile di repertorio (se impostato)
                        if (!BusinessLogic.utenti.RegistriRepertorioPrintManager.AssignDocumentVisibilityToResponsable(userInfo, docNumber, counterId, print.RFId, print.RegistryId, out rights))
                            throw new Exception("Errore assegnazione visiblità a ruolo responsabile");

                        // Se ci sono ruoli superiori, viene assegnata la visibilità
                        foreach (Ruolo r in roles)
                            if (!BusinessLogic.utenti.RegistriRepertorioPrintManager.AssignDocumentVisibilityToRole(userInfo, docNumber, counterId, print.RFId, print.RegistryId, r.idGruppo, rights))
                                throw new Exception(String.Format("Errore assegnazione visiblità al ruolo {0}", r.codice));

                        //assegnazione visibilità nel caso di repertori di tipologia/RF
                        if ((string.IsNullOrEmpty(print.RFId) && string.IsNullOrEmpty(print.RegistryId)) ||
                            (!string.IsNullOrEmpty(print.RFId)))
                        {
                            DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                            object[] supRolePrinter = gerarchia.getGerarchiaSup(print.PrinterRole, ((DocsPaVO.utente.Registro)print.PrinterRole.registri[0]).systemId, null, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO).ToArray();
                            foreach (Ruolo r in supRolePrinter)
                                if (!BusinessLogic.utenti.RegistriRepertorioPrintManager.AssignDocumentVisibilityToRole(userInfo, docNumber, counterId, print.RFId, print.RegistryId, r.idGruppo, rights))
                                    throw new Exception(String.Format("Errore assegnazione visiblità al ruolo {0}", r.codice));
                        }

                        // Completamento transazione
                        transactionContext.Complete();

                    }
                }
                
            }
        }

        public string GeneratePrintByDocumentId(string idDoc)
        {
            string retVal = "";
            logger.Info("START");
            DocsPaDB.Query_DocsPAWS.Conservazione dbCons = new DocsPaDB.Query_DocsPAWS.Conservazione();

            DocsPaVO.areaConservazione.StampaRegistro stReg = dbCons.getInfoStampaReperiorio(idDoc);
            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();

            List<RegistroRepertorioPrint> reg1 = manager.GetRegistersToPrintByCounter(stReg.idRepertorio);

            foreach (RegistroRepertorioPrint print in reg1)
            {
                if (print.RegistryId == stReg.idRegistro || print.RFId == stReg.idRegistro)
                {
                    logger.DebugFormat("Stampa del repertorio {0} della tipologia {5}, per l'anno {1}, su registro {2} o RF {3}, responsabile {4}",
                        print.CounterDescription, stReg.anno, print.RegistryId, print.RFId, print.PrinterUser.userId, print.TipologyDescription);

                    InfoUtente userInfo = new InfoUtente()
                    {
                        idAmministrazione = print.PrinterUser.idAmministrazione,
                        idCorrGlobali = print.PrinterUser.systemId,
                        idGruppo = print.PrinterRole.idGruppo,
                        idPeople = print.PrinterUser.idPeople,
                        userId = print.PrinterUser.userId
                    };

                    //RepertorioPrintRange rpr = manager.GetRepertoriPrintRangesByYear(counterId, print.RegistryId, print.RFId, year);
                    // Generazione filtri per la stampa

                    string minX = stReg.numProtoStart;
                    string maxX = stReg.numProtoEnd;
                        logger.DebugFormat("Limite inferiore {0}, superiore {1}", minX, maxX);

                        List<FiltroRicerca> filters = new List<FiltroRicerca>()
                    {
                    new FiltroRicerca() { argomento = "idCounter", valore = stReg.idRepertorio },
                    new FiltroRicerca() { argomento ="idRegistry", valore = print.RegistryId },
                    new FiltroRicerca() { argomento = "idRf", valore = print.RFId },
                    new FiltroRicerca() { argomento = "year", valore = stReg.anno},
                    new FiltroRicerca() { argomento = "lastPrintedNumber", valore = minX },
                    new FiltroRicerca() { argomento = "lastNumberToPrint", valore = maxX }
                    };

                        // Creazione della stampa
                        FileDocumento fileDocument = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(
                            new DocsPaVO.Report.PrintReportRequest()
                            {
                                UserInfo = userInfo,
                                SearchFilters = filters,
                                ReportType = DocsPaVO.Report.ReportTypeEnum.PDF,
                                ReportKey = "StampaRegistriRepertori",
                                ContextName = "StampaRegistriRepertori",
                                AdditionalInformation = String.Format(" Stampa del repertorio per l'anno {0} dal n. {1} al n. {2}. La presente stampa riporta tutte le registrazioni giornaliere di repertorio nell'intervallo indicato. Si è resa necessaria in quanto problematiche tecniche del sistema avevano inizialmente impedito la regolare stampa.", stReg.anno, minX, maxX)
                            }).Document;

                        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                        {

                            if (userInfo != null && string.IsNullOrEmpty(userInfo.dst))
                                userInfo.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

                            // Reperimento del nome della tipologia
                            string tipologia = manager.GetNomeTipologia(stReg.idRepertorio);

                            // Creazione della scheda documento
                            SchedaDocumento document = InitializeDocument(userInfo, print.RegistryId, print.RFId, stReg.anno, minX, maxX, tipologia);
                            document.oggetto.descrizione += " - RECUPERO";
                       
                            Ruolo[] roles = new Ruolo[] { };
                            // Salvataggio del documento
                            String docNumber = this.SaveDocument(userInfo, print.PrinterRole, document, fileDocument, out roles);

                            if (String.IsNullOrEmpty(docNumber))
                                throw new Exception("Errore durante la creazione della stampa");

                            retVal = docNumber;

                            // Aggiornamento del registro delle stampe e del prossimo numero da stampare
                            if (!manager.UpdatePrinterManager(stReg.idRepertorio, minX, maxX, docNumber, print.RegistryId, print.RFId, stReg.anno))
                                throw new Exception("Errore durante l'aggiornamento dell'anagrafica delle stampe effettuate");

                            int rights = 0;
                            // Assegnazione della visibilità della stampa al responsabile di repertorio (se impostato)
                            if (!BusinessLogic.utenti.RegistriRepertorioPrintManager.AssignDocumentVisibilityToResponsable(userInfo, docNumber, stReg.idRepertorio, print.RFId, print.RegistryId, out rights))
                                throw new Exception("Errore assegnazione visiblità a ruolo responsabile");

                            // Se ci sono ruoli superiori, viene assegnata la visibilità
                            foreach (Ruolo r in roles)
                                if (!BusinessLogic.utenti.RegistriRepertorioPrintManager.AssignDocumentVisibilityToRole(userInfo, docNumber, stReg.idRepertorio, print.RFId, print.RegistryId, r.idGruppo, rights))
                                    throw new Exception(String.Format("Errore assegnazione visiblità al ruolo {0}", r.codice));

                            //assegnazione visibilità nel caso di repertori di tipologia/RF
                            if ((string.IsNullOrEmpty(print.RFId) && string.IsNullOrEmpty(print.RegistryId)) ||
                                (!string.IsNullOrEmpty(print.RFId)))
                            {
                                DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                                object[] supRolePrinter = gerarchia.getGerarchiaSup(print.PrinterRole, ((DocsPaVO.utente.Registro)print.PrinterRole.registri[0]).systemId, null, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO).ToArray();
                                foreach (Ruolo r in supRolePrinter)
                                    if (!BusinessLogic.utenti.RegistriRepertorioPrintManager.AssignDocumentVisibilityToRole(userInfo, docNumber, stReg.idRepertorio, print.RFId, print.RegistryId, r.idGruppo, rights))
                                        throw new Exception(String.Format("Errore assegnazione visiblità al ruolo {0}", r.codice));
                            }

                            // Completamento transazione
                            transactionContext.Complete();

                        }

                        if (!string.IsNullOrEmpty(retVal))
                        {
                            DocsPaDB.Query_DocsPAWS.Report dbreport = new DocsPaDB.Query_DocsPAWS.Report();
                            dbreport.UpdStampaExp(retVal, stReg.dtaStampaTruncString);
                        }

                }
            }
            return retVal;
        }

        public string GeneratePrintByRanges(string anno, string counterId, string numRepStart, string numRepEnd,string idRegistro, string dataStampa, bool ultimastampa )
        {
            string retVal = "";
            logger.Info("START");
            DocsPaDB.Query_DocsPAWS.Conservazione dbCons = new DocsPaDB.Query_DocsPAWS.Conservazione();

            // uguale al metodo precedente tranne che per questo pezzo.
            //DocsPaVO.areaConservazione.StampaRegistro stReg = dbCons.getInfoStampaReperiorio(idDoc);

            DocsPaVO.areaConservazione.StampaRegistro stReg = new DocsPaVO.areaConservazione.StampaRegistro();

            stReg.anno = anno;
            stReg.idRepertorio= counterId;
            stReg.numProtoStart= numRepStart;
            if (!string.IsNullOrEmpty(numRepEnd))
                stReg.numProtoEnd= numRepEnd;
            stReg.idRegistro = idRegistro;
            stReg.dtaStampaTruncString = dataStampa;

            DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager manager = new DocsPaDB.Query_DocsPAWS.RegistriRepertorioPrintManager();

            List<RegistroRepertorioPrint> reg1 = manager.GetRegistersToPrintByCounter(stReg.idRepertorio);

            foreach (RegistroRepertorioPrint print in reg1)
            {
                if (print.RegistryId == stReg.idRegistro || print.RFId == stReg.idRegistro)
                {
                    logger.DebugFormat("Stampa del repertorio {0} della tipologia {5}, per l'anno {1}, su registro {2} o RF {3}, responsabile {4}",
                        print.CounterDescription, stReg.anno, print.RegistryId, print.RFId, print.PrinterUser.userId, print.TipologyDescription);

                    InfoUtente userInfo = new InfoUtente()
                    {
                        idAmministrazione = print.PrinterUser.idAmministrazione,
                        idCorrGlobali = print.PrinterUser.systemId,
                        idGruppo = print.PrinterRole.idGruppo,
                        idPeople = print.PrinterUser.idPeople,
                        userId = print.PrinterUser.userId
                    };

                    //RepertorioPrintRange rpr = manager.GetRepertoriPrintRangesByYear(counterId, print.RegistryId, print.RFId, year);
                    // Generazione filtri per la stampa
                    List<FiltroRicerca> filters = null;
                    string minX = stReg.numProtoStart;
                    if (!ultimastampa)
                    {
                        string maxX = stReg.numProtoEnd;
                        logger.DebugFormat("Limite inferiore {0}, superiore {1}", minX, maxX);

                        filters = new List<FiltroRicerca>()
                    {
                    new FiltroRicerca() { argomento = "idCounter", valore = stReg.idRepertorio },
                    new FiltroRicerca() { argomento ="idRegistry", valore = print.RegistryId },
                    new FiltroRicerca() { argomento = "idRf", valore = print.RFId },
                    new FiltroRicerca() { argomento = "year", valore = stReg.anno},
                    new FiltroRicerca() { argomento = "lastPrintedNumber", valore = minX },
                    new FiltroRicerca() { argomento = "lastNumberToPrint", valore = maxX },
                    new FiltroRicerca() { argomento = "RECUPERO_evitaMod", valore = "true" }
                    };
                    }
                    else
                    {
                        logger.DebugFormat("Limite inferiore {0}", minX);

                        filters = new List<FiltroRicerca>()
                    {
                    new FiltroRicerca() { argomento = "idCounter", valore = stReg.idRepertorio },
                    new FiltroRicerca() { argomento ="idRegistry", valore = print.RegistryId },
                    new FiltroRicerca() { argomento = "idRf", valore = print.RFId },
                    new FiltroRicerca() { argomento = "year", valore = stReg.anno},
                    new FiltroRicerca() { argomento = "lastPrintedNumber", valore = minX }
                    };
                    }

                    // Creazione della stampa
                    FileDocumento fileDocument = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(
                        new DocsPaVO.Report.PrintReportRequest()
                        {
                            UserInfo = userInfo,
                            SearchFilters = filters,
                            ReportType = DocsPaVO.Report.ReportTypeEnum.PDF,
                            ReportKey = "StampaRegistriRepertori",
                            ContextName = "StampaRegistriRepertori",
                            AdditionalInformation = String.Format(" Stampa del repertorio per l'anno {0} dal n. {1} al n. {2} - stampa di recupero del giorno {3}. La presente stampa riporta tutte le registrazioni giornaliere di repertorio nell'intervallo indicato. Si è resa necessaria in quanto problematiche tecniche del sistema avevano inizialmente impedito la corretta stampa.", stReg.anno, minX, numRepEnd,dataStampa)
                        }).Document;

                    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                    {

                        if (userInfo != null && string.IsNullOrEmpty(userInfo.dst))
                            userInfo.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

                        // Reperimento del nome della tipologia
                        string tipologia = manager.GetNomeTipologia(stReg.idRepertorio);

                        // Creazione della scheda documento
                        SchedaDocumento document = InitializeDocument(userInfo, print.RegistryId, print.RFId, stReg.anno, minX, numRepEnd, tipologia);
                        document.oggetto.descrizione += " - RECUPERO";
                       
                        Ruolo[] roles = new Ruolo[] { };
                        // Salvataggio del documento
                        String docNumber = this.SaveDocument(userInfo, print.PrinterRole, document, fileDocument, out roles);

                        if (String.IsNullOrEmpty(docNumber))
                            throw new Exception("Errore durante la creazione della stampa");

                        retVal = docNumber;

                        // Aggiornamento del registro delle stampe e del prossimo numero da stampare
                        if (!manager.UpdatePrinterManager(stReg.idRepertorio, minX, numRepEnd, docNumber, print.RegistryId, print.RFId, stReg.anno))
                            throw new Exception("Errore durante l'aggiornamento dell'anagrafica delle stampe effettuate");

                        int rights = 0;
                        // Assegnazione della visibilità della stampa al responsabile di repertorio (se impostato)
                        if (!BusinessLogic.utenti.RegistriRepertorioPrintManager.AssignDocumentVisibilityToResponsable(userInfo, docNumber, stReg.idRepertorio, print.RFId, print.RegistryId, out rights))
                            throw new Exception("Errore assegnazione visiblità a ruolo responsabile");

                        // Se ci sono ruoli superiori, viene assegnata la visibilità
                        foreach (Ruolo r in roles)
                            if (!BusinessLogic.utenti.RegistriRepertorioPrintManager.AssignDocumentVisibilityToRole(userInfo, docNumber, stReg.idRepertorio, print.RFId, print.RegistryId, r.idGruppo, rights))
                                throw new Exception(String.Format("Errore assegnazione visiblità al ruolo {0}", r.codice));

                        //assegnazione visibilità nel caso di repertori di tipologia/RF
                        if ((string.IsNullOrEmpty(print.RFId) && string.IsNullOrEmpty(print.RegistryId)) ||
                            (!string.IsNullOrEmpty(print.RFId)))
                        {
                            DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                            object[] supRolePrinter = gerarchia.getGerarchiaSup(print.PrinterRole, ((DocsPaVO.utente.Registro)print.PrinterRole.registri[0]).systemId, null, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO).ToArray();
                            foreach (Ruolo r in supRolePrinter)
                                if (!BusinessLogic.utenti.RegistriRepertorioPrintManager.AssignDocumentVisibilityToRole(userInfo, docNumber, stReg.idRepertorio, print.RFId, print.RegistryId, r.idGruppo, rights))
                                    throw new Exception(String.Format("Errore assegnazione visiblità al ruolo {0}", r.codice));
                        }

                        // Completamento transazione
                        transactionContext.Complete();

                    }

                    if (!string.IsNullOrEmpty(retVal))
                    {
                        DocsPaDB.Query_DocsPAWS.Report dbreport = new DocsPaDB.Query_DocsPAWS.Report();
                        dbreport.UpdStampaExp(retVal, stReg.dtaStampaTruncString);
                    }

                }
            }
            return retVal;
        }

        private void InviaInConservazione(string idProfile, string idAmm)
        {
            try
            {
                Conservazione.ConservazioneManager cons = new Conservazione.ConservazioneManager();
                cons.ExecuteVersamentoSingolo(idProfile, idAmm, true);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore non gestito nel processo di invio in conservazione della stampa: {0}", ex);
            }
        }

        private bool IsStampaRepDaConservare(string idContatore)
        {
            bool result = true;

            try
            {
                DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getOggettoById(idContatore);
                if(ogg != null)
                {
                    if (!string.IsNullOrEmpty(ogg.CONS_REPERTORIO) && ogg.CONS_REPERTORIO == "1")
                        result = true;
                    else
                        result = false;
                }
            }
            catch(Exception ex)
            {
                logger.Debug(ex.Message);
                result = true;
            }

            return result;
        }
    }

}
