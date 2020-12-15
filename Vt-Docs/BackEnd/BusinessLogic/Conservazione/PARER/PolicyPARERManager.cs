using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using DocsPaVO.Conservazione;
using DocsPaVO.documento;
using DocsPaVO.utente;
using log4net;

namespace BusinessLogic.Conservazione.PARER
{
    public class PolicyPARERManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(PolicyPARERManager));

        public static ArrayList getListaPolicy(string idAmm, string tipo)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetListaPolicyPARER(idAmm, tipo);
        }

        public static DocsPaVO.Conservazione.PARER.PolicyPARER GetPolicyById(string idPolicy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetPolicyPARERById(idPolicy);
        }

        public static DocsPaVO.Conservazione.PARER.EsecuzionePolicy GetInfoEsecuzionePolicy(string idPolicy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetInfoEsecuzionePolicy(idPolicy);
        }

        public static bool UpdateStatoPolicy(ArrayList lista, InfoUtente utente)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.UpdateStatoPolicy(lista, utente);
        }

        public static bool UpdateStatoSingolaPolicy(string idPolicy, string attiva, string notifica)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.UpdateStatoSingolaPolicy(idPolicy, attiva, notifica);
        }

        public static bool InsertNewPolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.InsertNewPolicyPARER(policy);
        }

        public static bool UpdatePolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.UpdatePolicyPARER(policy);
        }

        public static bool DeletePolicy(string idPolicy)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.DeletePolicyPARER(idPolicy);
        }

        public static string GetCountDocumentiFromPolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetCountDocumentiFromPolicy(policy, idAmm);
        }

        static object _PolicyLockObject = new object();
        public static void ExecutePolicy()
        {

            logger.Debug("BEGIN");

            try
            {
                lock (_PolicyLockObject)
                {

                    // lista delle amministrazioni dell'istanza
                    ArrayList listaAmm = BusinessLogic.Amministrazione.AmministraManager.GetAmministrazioni();
                    DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();

                    foreach (DocsPaVO.utente.Amministrazione amm in listaAmm)
                    {
                        // Estraggo le policy da eseguire nella data corrente
                        ArrayList listaPolicy = c.GetListaPolicyDaEseguire(amm.systemId);

                        if (listaPolicy != null && listaPolicy.Count > 0)
                        {
                            logger.Debug(string.Format("{0} policy attive nell'amministrazione {1}", listaPolicy.Count, amm.codice));

                            // Stato attivazione reportistica per l'amministrazione
                            bool disableReport = (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(amm.systemId, "BE_ENABLE_REPORT_POLICY_PARER"))
                                                    && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(amm.systemId, "BE_ENABLE_REPORT_POLICY_PARER").Equals("0"));

                            int limiteDoc;

                            foreach (DocsPaVO.Conservazione.PARER.EsecuzionePolicy info in listaPolicy)
                            {
                                // Estraggo il dettaglio della policy
                                DocsPaVO.Conservazione.PARER.PolicyPARER policy = GetPolicyById(info.idPolicy);

                                try
                                {

                                    // limite massimo documenti versabili per l'amministrazione
                                    if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(policy.idAmm, "FE_MAX_DOC_VERSAMENTO")))
                                        limiteDoc = Convert.ToInt32(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(policy.idAmm, "FE_MAX_DOC_VERSAMENTO"));
                                    else
                                        limiteDoc = -1;

                                    // creazione infoutente responsabile conservazione
                                    string idRoleRespCons = c.GetIdRoleResponsabileConservazione(policy.idAmm);
                                    string idUserRespCons = c.GetIdUtenteResponsabileConservazione(policy.idAmm);
                                    if (string.IsNullOrEmpty(idRoleRespCons) || string.IsNullOrEmpty(idUserRespCons))
                                    {
                                        continue;
                                    }

                                    InfoUtente infoUtResp = new InfoUtente();
                                    infoUtResp.idGruppo = idRoleRespCons;
                                    infoUtResp.idPeople = idUserRespCons;
                                    infoUtResp.idAmministrazione = policy.idAmm;
                                    infoUtResp.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

                                    // lista documenti da versare
                                    InfoDocumento[] listaDoc = null;
                                    listaDoc = c.GetListaDocFromPolicy(policy);

                                    if (listaDoc != null)
                                    {
                                        if (listaDoc.Length > 0)
                                        {
                                            // controllo superamento limite
                                            if (limiteDoc > 0 && listaDoc.Length > limiteDoc)
                                            {
                                                DocsPaVO.Conservazione.PARER.EsecuzionePolicy infoAgg = new DocsPaVO.Conservazione.PARER.EsecuzionePolicy();
                                                infoAgg.dataUltimaEsecuzione = DateTime.Today.ToString("dd/MM/yyyy");


                                                // Limite superato: la policy deve essere spenta
                                                UpdateStatoSingolaPolicy(policy.id, "0", policy.notificaMail);

                                                // Produzione report di esecuzione

                                                DocsPaVO.documento.FileDocumento report = CreateReportFailure(policy, infoUtResp, "SUP");
                                                if (report != null)
                                                {
                                                    // Inserisco il report nel sistema
                                                    DocsPaVO.documento.SchedaDocumento docReport = SaveReportDocument(policy, infoAgg, infoUtResp, report);

                                                    // Trasmissione al ruolo responsabile della conservazione
                                                    string note = string.Format("Report di mancata esecuzione della policy {0} (\"{1}\") del {2}", policy.codice, policy.descrizione, infoAgg.dataUltimaEsecuzione);

                                                    //1
                                                    if (!disableReport)
                                                    {

                                                        DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                                                        if (!string.IsNullOrEmpty(policy.idGruppoRuoloResp))
                                                        {
                                                            InfoUtente respPolicy = new InfoUtente();
                                                            respPolicy.idGruppo = policy.idGruppoRuoloResp;
                                                            trasm = ExecuteTransmission(infoUtResp, respPolicy, docReport, note);
                                                        }
                                                        else
                                                        {
                                                            trasm = ExecuteTransmission(infoUtResp, null, docReport, note);
                                                        }
                                                        // Inserimento log
                                                        if (trasm != null)
                                                        {
                                                            logger.Debug("INSERIMENTO LOG");
                                                            if (trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile))
                                                            {
                                                                foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasm.trasmissioniSingole)
                                                                {
                                                                    string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                                                    string desc = "Trasmesso Documento : " + trasm.infoDocumento.docNumber.ToString();
                                                                    BusinessLogic.UserLog.UserLog.WriteLog(trasm.utente.userId, trasm.utente.idPeople, trasm.ruolo.idGruppo, infoUtResp.idAmministrazione,
                                                                        method, trasm.infoDocumento.idProfile, desc, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", single.systemId);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //throw new Exception("Errore nella produzione del report di esecuzione della policy " + policy.codice);
                                                    logger.Debug("Errore nella produzione del report di esecuzione della policy " + policy.codice);
                                                }

                                                if (policy.notificaMail == "1")
                                                {
                                                    logger.Debug("Invio mail di notifica mancata esecuzione");
                                                    SendMailNotification(policy.idAmm, policy);
                                                }

                                            }
                                            else
                                            {
                                                // Apertura contesto transazionale per inserimento in coda
                                                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                                                {

                                                    // numero esecuzioni policy
                                                    int numEsecuzioniPolicy = Convert.ToInt32(info.numeroEsecuzioni) + 1;

                                                    BusinessLogic.Conservazione.ConservazioneManager consManager = new ConservazioneManager();

                                                    foreach (InfoDocumento doc in listaDoc)
                                                    {
                                                        // Inserimento doc in coda di versamento
                                                        string esito = consManager.insertDocInCons(doc.idProfile, infoUtResp, policy.ente, policy.struttura);
                                                        //logger.Debug(string.Format("EXECUTEPOLICY - {0}: {1}", doc.idProfile, esito));

                                                        if (esito.Equals("OK"))
                                                        {
                                                            // Inserimento doc in tabella associativa policy-documenti
                                                            DocsPaVO.Conservazione.PARER.EsecuzionePolicy infoEsecuzione = new DocsPaVO.Conservazione.PARER.EsecuzionePolicy();
                                                            infoEsecuzione.idPolicy = policy.id;
                                                            infoEsecuzione.numeroEsecuzioni = numEsecuzioniPolicy.ToString();
                                                            if (!c.SetInfoVersamentoAutomatico(doc.idProfile, infoEsecuzione))
                                                                throw new Exception(string.Format("Errore inserimento dell'associazione documento-policy per ID doc={0}, policy={1}", doc.idProfile, policy.id));

                                                            // Estensione visibilità al responsabile della policy
                                                            if (!string.IsNullOrEmpty(policy.idGruppoRuoloResp))
                                                            {
                                                                consManager.SetVisibilitaRuoloResp(doc.idProfile, policy.idGruppoRuoloResp, string.Empty);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("Errore inserimento in coda documento ID={0} - Codice errore {1}", doc.idProfile, esito));
                                                        }
                                                    }

                                                    // Aggiornamento informazioni esecuzione policy
                                                    DocsPaVO.Conservazione.PARER.EsecuzionePolicy infoAgg = new DocsPaVO.Conservazione.PARER.EsecuzionePolicy();
                                                    infoAgg.idPolicy = policy.id;
                                                    infoAgg.numeroEsecuzioni = numEsecuzioniPolicy.ToString();
                                                    infoAgg.dataUltimaEsecuzione = DateTime.Today.ToString("dd/MM/yyyy");
                                                    infoAgg.dataProssimaEsecuzione = c.GetDataProssimaEsecuzione(policy);

                                                    // se la policy è configurata per l'esecuzione "Una Tantum", deve essere disattivata al termine dell'operazione
                                                    if (policy.periodicita.Equals("O"))
                                                    {
                                                        infoAgg.dataProssimaEsecuzione = string.Empty;
                                                        UpdateStatoSingolaPolicy(policy.id, "0", policy.notificaMail);
                                                    }

                                                    if (!c.SetInfoEsecuzionePolicy(infoAgg))
                                                        throw new Exception("Errore nell'aggiornamento del dettaglio di esecuzione della policy");


                                                    // Produzione report di esecuzione

                                                    DocsPaVO.documento.FileDocumento report = CreateReportSuccess(listaDoc, policy, infoAgg, infoUtResp);

                                                    if (report != null)
                                                    {
                                                        // Inserisco il report nel sistema
                                                        DocsPaVO.documento.SchedaDocumento docReport = SaveReportDocument(policy, infoAgg, infoUtResp, report);
                                                        if (string.IsNullOrEmpty(docReport.docNumber))
                                                            throw new Exception("Errore nella creazione del report di esecuzione della policy " + policy.codice);

                                                        //2
                                                        if (!disableReport)
                                                        {
                                                            // Trasmissione al ruolo responsabile della conservazione
                                                            string note = string.Format("Report di esecuzione della policy {0} (\"{1}\") del {2}", policy.codice, policy.descrizione, infoAgg.dataUltimaEsecuzione);
                                                            DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                                                            if (!string.IsNullOrEmpty(policy.idGruppoRuoloResp))
                                                            {
                                                                InfoUtente respPolicy = new InfoUtente();
                                                                respPolicy.idGruppo = policy.idGruppoRuoloResp;
                                                                trasm = ExecuteTransmission(infoUtResp, respPolicy, docReport, note);
                                                            }
                                                            else
                                                            {
                                                                trasm = ExecuteTransmission(infoUtResp, null, docReport, note);
                                                            }

                                                            // Inserimento log
                                                            if (trasm != null)
                                                            {
                                                                logger.Debug("INSERIMENTO LOG");
                                                                if (trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile))
                                                                {
                                                                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasm.trasmissioniSingole)
                                                                    {
                                                                        string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                                                        string desc = "Trasmesso Documento : " + trasm.infoDocumento.docNumber.ToString();
                                                                        BusinessLogic.UserLog.UserLog.WriteLog(trasm.utente.userId, trasm.utente.idPeople, trasm.ruolo.idGruppo, infoUtResp.idAmministrazione,
                                                                            method, trasm.infoDocumento.idProfile, desc, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", single.systemId);
                                                                    }
                                                                }
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        //throw new Exception("Errore nella produzione del report di esecuzione della policy " + policy.codice);
                                                        logger.Debug("Errore nella produzione del report di esecuzione della policy " + policy.codice);
                                                    }


                                                    // Completamento transazione
                                                    transactionContext.Complete();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // Nessun documento estratto

                                            DocsPaVO.Conservazione.PARER.EsecuzionePolicy infoAgg = new DocsPaVO.Conservazione.PARER.EsecuzionePolicy();
                                            infoAgg.idPolicy = policy.id;
                                            infoAgg.dataUltimaEsecuzione = DateTime.Today.ToString("dd/MM/yyyy");
                                            infoAgg.dataProssimaEsecuzione = c.GetDataProssimaEsecuzione(policy);

                                            // se la policy è configurata per l'esecuzione "Una Tantum", deve essere disattivata al termine dell'operazione
                                            if (policy.periodicita.Equals("O"))
                                            {
                                                infoAgg.dataProssimaEsecuzione = string.Empty;
                                                UpdateStatoSingolaPolicy(policy.id, "0", policy.notificaMail);
                                            }

                                            if (!c.SetInfoEsecuzionePolicy(infoAgg))
                                                throw new Exception("Errore nell'aggiornamento del dettaglio di esecuzione della policy");

                                            // Produzione report di esecuzione

                                            if (!(policy.tipo == "S" && policy.statoVersamento != "N"))
                                            {
                                                DocsPaVO.documento.FileDocumento report = CreateReportFailure(policy, infoUtResp, "NO_DATA");
                                                if (report != null)
                                                {
                                                    // Inserisco il report nel sistema
                                                    DocsPaVO.documento.SchedaDocumento docReport = SaveReportDocument(policy, infoAgg, infoUtResp, report);

                                                    // Trasmissione al ruolo responsabile della conservazione
                                                    //3
                                                    if (!disableReport)
                                                    {
                                                        string note = string.Format("Report di mancata esecuzione della policy {0} (\"{1}\") del {2}", policy.codice, policy.descrizione, infoAgg.dataUltimaEsecuzione);
                                                        DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                                                        if (!string.IsNullOrEmpty(policy.idGruppoRuoloResp))
                                                        {
                                                            InfoUtente respPolicy = new InfoUtente();
                                                            respPolicy.idGruppo = policy.idGruppoRuoloResp;
                                                            trasm = ExecuteTransmission(infoUtResp, respPolicy, docReport, note);
                                                        }
                                                        else
                                                        {
                                                            trasm = ExecuteTransmission(infoUtResp, null, docReport, note);
                                                        }
                                                        // Inserimento log
                                                        if (trasm != null)
                                                        {
                                                            logger.Debug("INSERIMENTO LOG");
                                                            if (trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile))
                                                            {
                                                                foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasm.trasmissioniSingole)
                                                                {
                                                                    string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                                                    string desc = "Trasmesso Documento : " + trasm.infoDocumento.docNumber.ToString();
                                                                    BusinessLogic.UserLog.UserLog.WriteLog(trasm.utente.userId, trasm.utente.idPeople, trasm.ruolo.idGruppo, infoUtResp.idAmministrazione,
                                                                        method, trasm.infoDocumento.idProfile, desc, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", single.systemId);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //throw new Exception("Errore nella produzione del report di esecuzione della policy " + policy.codice);
                                                    logger.Debug("Errore nella produzione del report di esecuzione della policy " + policy.codice);
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        // listaDoc == null
                                        // Non ho risultati dalla ricerca
                                        DocsPaVO.Conservazione.PARER.EsecuzionePolicy infoAgg = new DocsPaVO.Conservazione.PARER.EsecuzionePolicy();
                                        infoAgg.idPolicy = policy.id;
                                        infoAgg.dataUltimaEsecuzione = DateTime.Today.ToString("dd/MM/yyyy");
                                        infoAgg.dataProssimaEsecuzione = c.GetDataProssimaEsecuzione(policy);

                                        // se la policy è configurata per l'esecuzione "Una Tantum", deve essere disattivata al termine dell'operazione
                                        if (policy.periodicita.Equals("O"))
                                        {
                                            infoAgg.dataProssimaEsecuzione = string.Empty;
                                            UpdateStatoSingolaPolicy(policy.id, "0", policy.notificaMail);
                                        }

                                        if (!c.SetInfoEsecuzionePolicy(infoAgg))
                                            throw new Exception("Errore nell'aggiornamento del dettaglio di esecuzione della policy");

                                        // Produzione report di esecuzione

                                        if (!(policy.tipo == "S" && policy.statoVersamento != "N"))
                                        {
                                            DocsPaVO.documento.FileDocumento report = CreateReportFailure(policy, infoUtResp, "NO_DATA");
                                            if (report != null)
                                            {
                                                // Inserisco il report nel sistema
                                                DocsPaVO.documento.SchedaDocumento docReport = SaveReportDocument(policy, infoAgg, infoUtResp, report);

                                                //4
                                                if (!disableReport)
                                                {
                                                    // Trasmissione al ruolo responsabile della conservazione
                                                    string note = string.Format("Report di mancata esecuzione della policy {0} (\"{1}\") del {2}", policy.codice, policy.descrizione, infoAgg.dataUltimaEsecuzione);
                                                    DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                                                    if (!string.IsNullOrEmpty(policy.idGruppoRuoloResp))
                                                    {
                                                        InfoUtente respPolicy = new InfoUtente();
                                                        respPolicy.idGruppo = policy.idGruppoRuoloResp;
                                                        trasm = ExecuteTransmission(infoUtResp, respPolicy, docReport, note);
                                                    }
                                                    else
                                                    {
                                                        trasm = ExecuteTransmission(infoUtResp, null, docReport, note);
                                                    }
                                                    // Inserimento log
                                                    if (trasm != null)
                                                    {
                                                        logger.Debug("INSERIMENTO LOG");
                                                        if (trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile))
                                                        {
                                                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasm.trasmissioniSingole)
                                                            {
                                                                string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                                                string desc = "Trasmesso Documento : " + trasm.infoDocumento.docNumber.ToString();
                                                                BusinessLogic.UserLog.UserLog.WriteLog(trasm.utente.userId, trasm.utente.idPeople, trasm.ruolo.idGruppo, infoUtResp.idAmministrazione,
                                                                    method, trasm.infoDocumento.idProfile, desc, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", single.systemId);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //throw new Exception("Errore nella produzione del report di esecuzione della policy " + policy.codice);
                                                logger.Debug("Errore nella produzione del report di esecuzione della policy " + policy.codice);
                                            }
                                        }
                                    }
                                }
                                catch(Exception ex)
                                {
                                    logger.Debug("Errore nella lavorazione della policy " + info.idPolicy);
                                    if(policy != null && policy.notificaMail == "1")
                                    {
                                        SendMailNotification(policy.idAmm, policy);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("ExecutePolicy - ", ex);
            }

            logger.Debug("END");
        }

        public static DocsPaVO.documento.FileDocumento ExportPolicy(string[] id, string formato, string tipo)
        {
            DocsPaVO.documento.FileDocumento doc = new FileDocumento();

            // costruisco la lista di ID da passare al generatore
            string listaID = string.Empty;
            for (int i = 0; i < id.Length; i++)
            {
                listaID = listaID + id[i];
                if (i < id.Length - 1)
                    listaID = listaID + ",";
            }

            // preparazione filtri request
            List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idPolicy", valore = listaID });
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "type", valore = tipo });

            DocsPaVO.Report.PrintReportRequest request = new DocsPaVO.Report.PrintReportRequest();
            request.ContextName = "AmmExportPolicyPARER";
            request.ReportKey = "AmmExportPolicyPARER";
            request.Title = "Dettaglio Policy";
            request.SubTitle = string.Empty;
            request.AdditionalInformation = string.Empty;
            request.SearchFilters = filters;

            switch (formato)
            {
                case "XLS":
                    request.ReportType = DocsPaVO.Report.ReportTypeEnum.Excel;
                    break;

                case "ODS":
                    request.ReportType = DocsPaVO.Report.ReportTypeEnum.ODS;
                    break;
            }

            doc = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request).Document;

            if (doc != null)
            {
                if (tipo.Equals("DOC"))
                    doc.name = string.Format("Report_Policy_Documenti_{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
                else
                    doc.name = string.Format("Report_Policy_Stampe_{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
            }

            return doc;
        }

        private static DocsPaVO.documento.FileDocumento CreateReportSuccess(InfoDocumento[] listaDoc, DocsPaVO.Conservazione.PARER.PolicyPARER policy, DocsPaVO.Conservazione.PARER.EsecuzionePolicy info, InfoUtente infoUtResp)
        {
            logger.Debug("BEGIN");
            ConservazioneManager consMan = new ConservazioneManager();
            // costruzione dataset
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ds.Tables.Add(dt);
            DataColumn col1 = new DataColumn("ID DOC. / NUM. PROTOCOLLO");
            DataColumn col2 = new DataColumn("TIPO");
            DataColumn col3 = new DataColumn("DATA CREAZIONE / PROTOCOLLAZIONE");
            DataColumn col4 = new DataColumn("OGGETTO");
            DataColumn col5 = new DataColumn("TIPO REGISTRO (RF)");
            dt.Columns.Add(col1);
            dt.Columns.Add(col2);
            dt.Columns.Add(col3);
            dt.Columns.Add(col4);
            dt.Columns.Add(col5);

            DocsPaVO.amministrazione.InfoAmministrazione amm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(policy.idAmm);

            for (int i = 0; i < listaDoc.Length; i++)
            {
                DataRow row = dt.NewRow();

                if (!string.IsNullOrEmpty(listaDoc[i].numProt))
                    row["ID DOC. / NUM. PROTOCOLLO"] = listaDoc[i].numProt;
                else
                    row["ID DOC. / NUM. PROTOCOLLO"] = listaDoc[i].idProfile;

                switch (listaDoc[i].tipoProto)
                {
                    case "A":
                        row["TIPO"] = "Arrivo";
                        break;
                    case "P":
                        row["TIPO"] = "Partenza";
                        break;
                    case "I":
                        row["TIPO"] = "Interno";
                        break;
                    case "G":
                        row["TIPO"] = "Non protocollato";
                        break;
                    case "R":
                        row["TIPO"] = "Stampa reg. protocollo";
                        break;
                    case "C":
                        row["TIPO"] = "Stampa reg. repertorio";
                        break;
                }

                row["DATA CREAZIONE / PROTOCOLLAZIONE"] = listaDoc[i].dataApertura;
                row["OGGETTO"] = listaDoc[i].oggetto;

                try
                {

                    SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtResp, listaDoc[i].idProfile);

                    string codRF = string.Empty;
                    if (listaDoc[i].tipoProto.Equals("C"))
                        codRF = GetCodRF(listaDoc[i].idProfile);

                    if (string.IsNullOrEmpty(codRF))
                        row["TIPO REGISTRO (RF)"] = consMan.getChiaveVersamento(sd, consMan.getTipoDocumento(sd, amm.Codice)).tipoRegistro;
                    else
                        row["TIPO REGISTRO (RF)"] = consMan.getChiaveVersamento(sd, consMan.getTipoDocumento(sd, amm.Codice)).tipoRegistro + " " + codRF;
                }
                catch(Exception ex)
                {
                    row["TIPO REGISTRO (RF)"] = string.Empty;
                }

                dt.Rows.Add(row);
            }

            string infoPolicy = string.Format("Data di esecuzione: {0}\r\nNumero esecuzioni: {1}", info.dataUltimaEsecuzione, info.numeroEsecuzioni);
            if (!policy.periodicita.Equals("O"))
            {
                infoPolicy = infoPolicy + "\r\n";
                string periodicita = string.Empty;
                if (policy.periodicita.Equals("D"))
                    periodicita = "giornaliera";
                if (policy.periodicita.Equals("W"))
                    periodicita = "settimanale";
                if (policy.periodicita.Equals("M"))
                    periodicita = "mensile";
                if (policy.periodicita.Equals("Y"))
                    periodicita = "annuale";
                infoPolicy = infoPolicy + string.Format("Data di prossima esecuzione: {0} (periodicità {1}).", info.dataProssimaEsecuzione, periodicita);
            }
            else
            {
                infoPolicy = infoPolicy + "\r\n";
                infoPolicy = infoPolicy + "Policy configurata per l'esecuzione una tantum.";
            }

            infoPolicy = infoPolicy + "\r\n";
            if (policy.statoVersamento.Equals("R"))
            {
                infoPolicy = infoPolicy + "Stato conservazione: documenti rifiutati";
            }
            else if (policy.statoVersamento.Equals("F"))
            {
                infoPolicy = infoPolicy + "Stato conservazione: versamento fallito";
            }
            else
            {
                infoPolicy = infoPolicy + "Stato conservazione: documenti non conservati";
            }

            // Creazione request
            DocsPaVO.Report.PrintReportRequestDataset request = new DocsPaVO.Report.PrintReportRequestDataset();
            request.UserInfo = infoUtResp;
            request.ReportType = DocsPaVO.Report.ReportTypeEnum.PDF;
            request.ReportKey = "EsecuzionePolicyPARER";
            request.ContextName = "EsecuzionePolicyPARER";
            request.InputDataset = ds;
            request.Title = "Report di esecuzione della Policy";
            request.SubTitle = string.Format("AMMINISTRAZIONE - Codice: {0}, Descrizione: {1}\r\nPOLICY - Codice: {2}, Descrizione: {3} \r\n \r\n", amm.Codice, amm.Descrizione, policy.codice, policy.descrizione);
            request.AdditionalInformation = infoPolicy;

            List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "type", valore = "SUCCESS" });
            request.SearchFilters = filters;

            // creazione documento
            DocsPaVO.documento.FileDocumento fd = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request).Document;

            return fd;

        }

        private static DocsPaVO.documento.FileDocumento CreateReportFailure(DocsPaVO.Conservazione.PARER.PolicyPARER policy, InfoUtente infoUtResp, string type)
        {
            logger.Debug("BEGIN");

            DocsPaVO.amministrazione.InfoAmministrazione amm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(policy.idAmm);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ds.Tables.Add(dt);

            string infoPolicy = " \r\n";

            // Notifica di mancata esecuzione
            if (type.Equals("SUP"))
            {
                if (policy.statoVersamento.Equals("R"))
                {
                    infoPolicy = infoPolicy + "Stato conservazione: documenti rifiutati";
                }
                else if (policy.statoVersamento.Equals("F"))
                {
                    infoPolicy = infoPolicy + "Stato conservazione: versamento fallito";
                }
                else
                {
                    infoPolicy = infoPolicy + "Stato conservazione: documenti non conservati";
                }
                infoPolicy = infoPolicy + "\r\n\r\n";

                infoPolicy = infoPolicy + "La policy non è stata eseguita a causa del superamento dei limiti impostati in termini di numero di documenti versabili tramite una singola operazione.\r\n";
                infoPolicy = infoPolicy + "A seguito della mancata esecuzione la policy è stata disattivata.\r\n";
            }
            if (type.Equals("NO_DATA"))
            {
                infoPolicy = "\r\n";
                infoPolicy = infoPolicy + "Nessun documento soddisfa i criteri di selezione configurati nella policy.\r\n";
            }


            // Creazione request
            DocsPaVO.Report.PrintReportRequestDataset request = new DocsPaVO.Report.PrintReportRequestDataset();
            request.UserInfo = infoUtResp;
            request.ReportType = DocsPaVO.Report.ReportTypeEnum.PDF;
            request.ReportKey = "EsecuzionePolicyPARER";
            request.ContextName = "EsecuzionePolicyPARER";
            request.InputDataset = ds;
            request.Title = "Report di esecuzione della Policy";
            request.SubTitle = string.Format("AMMINISTRAZIONE - Codice: {0}, Descrizione: {1}\r\nPOLICY - Codice: {2}, Descrizione: {3} \r\n \r\n", amm.Codice, amm.Descrizione, policy.codice, policy.descrizione);
            request.AdditionalInformation = infoPolicy;

            List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "type", valore = "FAILURE" });
            request.SearchFilters = filters;

            // Creazione documento
            DocsPaVO.documento.FileDocumento fd = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request).Document;

            // CODICE TEMPORANEO PER SALVATAGGIO IN LOCALE
            /*
            System.IO.FileStream fs = System.IO.File.Create("C:\\temp\\reportKO.pdf", fd.content.Length);
            fs.Write(fd.content, 0, fd.content.Length);
            fs.Close();
            */
            logger.Debug("END");
            return fd;
        }

        public static string CreateReportDocsRejected()
        {
            string result = string.Empty;
            DocsPaVO.amministrazione.InfoAmministrazione[] listaAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetListAmministrazioni();
            if (listaAmm != null && listaAmm.Length > 0)
            {
                foreach (DocsPaVO.amministrazione.InfoAmministrazione amm in listaAmm)
                {
                    // Verifico che siano presenti policy attive
                    string val = CountPolicyAttiveAmm(amm.IDAmm);
                    if (val.Equals("0"))
                    {
                        result = result + "Nessuna policy attiva per l'amministrazione " + amm.Descrizione + "\r\n";
                    }
                    else
                    {
                        // Verifico l'esistenza di documenti in stato "Rifiutato"
                        string n = CountDocVersati("R", amm.IDAmm);
                        if (!string.IsNullOrEmpty(n))
                        {
                            if (!n.Equals("0"))
                            {
                                string s = CreateReport(amm.IDAmm, "R");
                                if (!string.IsNullOrEmpty(s))
                                {
                                    result = result + "Errore nella creazione del report R per l'amministrazione " + amm.Descrizione + ": " + s + " \r\n";
                                }
                            }
                            else
                            {
                                //string s = CreateEmptyReport(amm.IDAmm, "R");
                                //if (!string.IsNullOrEmpty(s))
                                //    result = result + "Errore nella creazione del report RE per l'amministrazione " + amm.Descrizione + ": " + s + "\r\n";
                                //else
                                //    result = result + "Nessun documento in stato Rifiutato per l'amministrazione " + amm.Descrizione + " \r\n";
                            }
                        }
                        else
                        {
                            result = result + "Errore nel reperimento dei documenti in stato Rifiutato per l'amministrazione " + amm.Descrizione + " \r\n";
                        }
                    }
                }
            }

            return result;
        }

        public static string CreateReportDocsFailed()
        {
            string result = string.Empty;
            DocsPaVO.amministrazione.InfoAmministrazione[] listaAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetListAmministrazioni();
            if (listaAmm != null && listaAmm.Length > 0)
            {
                foreach (DocsPaVO.amministrazione.InfoAmministrazione amm in listaAmm)
                {
                    // Verifico che siano presenti policy attive
                    string val = CountPolicyAttiveAmm(amm.IDAmm);
                    if (val.Equals("0"))
                    {
                        result = result + "Nessuna policy attiva per l'amministrazione " + amm.Descrizione + "\r\n";
                    }
                    else
                    {
                        // Verifico l'esistenza di documenti in stato "Versamento fallito"
                        string n = CountDocVersati("F", amm.IDAmm);
                        if (!string.IsNullOrEmpty(n))
                        {
                            if (!n.Equals("0"))
                            {
                                string s = CreateReport(amm.IDAmm, "F");
                                if (!string.IsNullOrEmpty(s))
                                {
                                    result = result + "Errore nella creazione del report F per l'amministrazione " + amm.Descrizione + ": " + s + " \r\n";
                                }
                            }
                            else
                            {
                                //string s = CreateEmptyReport(amm.IDAmm, "F");
                                //if (!string.IsNullOrEmpty(s))
                                //    result = result + "Errore nella creazione del report FE per l'amministrazione" + amm.Descrizione + ": " + s + " \r\n";
                                //else
                                //    result = result + "Nessun documento in stato Versamento fallito per l'amministrazione " + amm.Descrizione + " \r\n";
                            }
                        }
                        else
                        {
                            result = result + "Errore nel reperimento dei documenti in stato Versamento fallito per l'amministrazione " + amm.Descrizione + " \r\n";
                        }
                    }
                }
            }

            return result;
        }

        private static string CreateReport(string idAmm, string tipo)
        {
            logger.Debug("BEGIN");
            string retVal = string.Empty;

            // creazione della request
            DocsPaVO.Report.PrintReportRequest request = new DocsPaVO.Report.PrintReportRequest();
            request.ReportType = DocsPaVO.Report.ReportTypeEnum.PDF;
            request.ReportKey = "ReportVersamentiPARER";
            request.ContextName = "ReportVersamentiPARER";

            List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "stato", valore = tipo });
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idAmm", valore = idAmm });
            request.SearchFilters = filters;

            // creazione del documento
            DocsPaVO.documento.FileDocumento fd = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request).Document;

            using (DocsPaDB.TransactionContext transaction = new DocsPaDB.TransactionContext())
            {
                try
                {
                    #region Salvataggio nel documentale

                    DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();

                    // Creazione infoutente responsabile delle conservazione
                    InfoUtente respCons = new InfoUtente();
                    respCons.idPeople = cons.GetIdUtenteResponsabileConservazione(idAmm);
                    respCons.idGruppo = cons.GetIdRoleResponsabileConservazione(idAmm);
                    respCons.idAmministrazione = idAmm;
                    respCons.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

                    // Creazione schedadocumento
                    DocsPaVO.documento.SchedaDocumento sd = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(respCons);
                    sd.appId = "ACROBAT";
                    sd.tipoProto = "G";
                    sd.typeId = "LETTERA";
                    sd.privato = "1";
                    Oggetto oggetto = new Oggetto();
                    if (tipo.Equals("R"))
                        oggetto.descrizione = string.Format("Report versamenti rifiutati del {0}", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
                    else
                        oggetto.descrizione = string.Format("Report versamenti falliti del {0}", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
                    sd.oggetto = oggetto;

                    DocsPaVO.utente.Ruolo r = new Ruolo();
                    r = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(respCons.idGruppo);

                    // Salvataggio del documento
                    DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(respCons);
                    sd = BusinessLogic.Documenti.ProtoManager.addOggettoLocked(idAmm, sd);

                    if (docManager.CreateDocumentoGrigio(sd, r))
                    {
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                        fr = BusinessLogic.Documenti.FileManager.putFile(fr, fd, respCons);
                        if (fr == null)
                            throw new Exception("Errore nell'upload del report nel sistema");
                    }
                    else
                    {
                        throw new Exception("Errore nella creazione del documento");
                    }

                    #endregion

                    #region Trasmissione - disabilitato
                    // MEV Reportistica 2017
                    // I report non sono più notificati

                    /*
                    // Trasmissione
                    DocsPaVO.documento.InfoDocumento infoDoc = new InfoDocumento();
                    infoDoc.idProfile = sd.systemId;
                    infoDoc.docNumber = sd.docNumber;
                    infoDoc.oggetto = sd.oggetto.descrizione;
                    infoDoc.tipoProto = "G";

                    DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                    DocsPaVO.trasmissione.RagioneTrasmissione ragione = new DocsPaVO.trasmissione.RagioneTrasmissione();
                    ArrayList trasmSingole = new ArrayList();
                    ArrayList trasmUtente = new ArrayList();

                    ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneNotifica(respCons.idAmministrazione);
                    ragione.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.WRITE;
                    trasm.ruolo = r;
                    trasm.utente = BusinessLogic.Utenti.UserManager.getUtenteNoFiltroDisabled(respCons.idPeople);
                    trasm.utente.dst = respCons.dst;
                    trasm.infoDocumento = infoDoc;

                    string note = string.Empty;
                    if (tipo.Equals("R"))
                        note = string.Format("Report versamenti rifiutati del {0}", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
                    else
                        note = string.Format("Report versamenti falliti del {0}", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
                    trasm.noteGenerali = note;

                    // Trasmissione al ruolo responsabile della conservazione
                    DocsPaVO.trasmissione.TrasmissioneSingola tsRespCons = new DocsPaVO.trasmissione.TrasmissioneSingola();
                    tsRespCons.corrispondenteInterno = r;
                    tsRespCons.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                    tsRespCons.tipoTrasm = "S";
                    tsRespCons.ragione = ragione;

                    // Costruisco le trasmissioni singole
                    ArrayList listaUtC = new ArrayList();
                    listaUtC = BusinessLogic.Utenti.UserManager.getListaUtentiByIdRuolo(r.systemId);
                    foreach (Utente u in listaUtC)
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente trasmUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                        trasmUt.utente = u;
                        tsRespCons.trasmissioneUtente.Add(trasmUt);
                    }
                    trasm.trasmissioniSingole.Add(tsRespCons);

                    DocsPaVO.trasmissione.Trasmissione result = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(string.Empty, trasm);

                    // Inserimento log
                    if (trasm != null)
                    {
                        logger.Debug("INSERIMENTO LOG");
                        if (trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasm.trasmissioniSingole)
                            {
                                string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                string desc = "Trasmesso Documento : " + trasm.infoDocumento.docNumber.ToString();
                                BusinessLogic.UserLog.UserLog.WriteLog(trasm.utente.userId, trasm.utente.idPeople, trasm.ruolo.idGruppo, idAmm,
                                    method, trasm.infoDocumento.idProfile, desc, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", single.systemId);
                            }
                        }
                    }
                    */
                    #endregion

                    retVal = string.Empty;
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    retVal = ex.ToString();
                    logger.Debug(ex.Message);
                }
            }
            logger.Debug("END");

            return retVal;
        }

        private static string CreateEmptyReport(string idAmm, string tipo)
        {
            logger.Debug("BEGIN");
            string retVal = string.Empty;

            DocsPaVO.amministrazione.InfoAmministrazione amm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(idAmm);
            string ammString = string.Format("AMMINISTRAZIONE - Codice: {0}, Descrizione: {1}\r\n", amm.Codice, amm.Descrizione);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ds.Tables.Add(dt);

            // creazione della request
            DocsPaVO.Report.PrintReportRequestDataset request = new DocsPaVO.Report.PrintReportRequestDataset();
            request.ReportType = DocsPaVO.Report.ReportTypeEnum.PDF;
            request.ReportKey = "ReportVersamentiPAREREmpty";
            request.ContextName = "ReportVersamentiPAREREmpty";
            request.InputDataset = ds;

            if (tipo.Equals("R"))
            {
                request.Title = "Report sui documenti in stato \"Rifiutato\" ";
                request.SubTitle = ammString + string.Format("\r\nNessun documento versato in conservazione il giorno {0} è stato rifiutato dal sistema di conservazione.", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
            }
            else
            {
                request.Title = "Report sui documenti in stato \"Versamento fallito\" ";
                request.SubTitle = ammString + string.Format("\r\nNessun documento versato in conservazione il giorno {0} risulta essere in stato \"Versamento fallito\".", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
            }

            // creazione del documento
            DocsPaVO.documento.FileDocumento fd = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request).Document;

            using (DocsPaDB.TransactionContext transaction = new DocsPaDB.TransactionContext())
            {
                try
                {
                    #region Salvataggio nel documentale

                    DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();

                    // Creazione infoutente responsabile delle conservazione
                    InfoUtente respCons = new InfoUtente();
                    respCons.idPeople = cons.GetIdUtenteResponsabileConservazione(idAmm);
                    respCons.idGruppo = cons.GetIdRoleResponsabileConservazione(idAmm);
                    respCons.idAmministrazione = idAmm;
                    respCons.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

                    // Creazione schedadocumento
                    DocsPaVO.documento.SchedaDocumento sd = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(respCons);
                    sd.appId = "ACROBAT";
                    sd.tipoProto = "G";
                    sd.typeId = "LETTERA";
                    sd.privato = "1";
                    Oggetto oggetto = new Oggetto();
                    if (tipo.Equals("R"))
                        oggetto.descrizione = string.Format("Report versamenti rifiutati del {0}", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
                    else
                        oggetto.descrizione = string.Format("Report versamenti falliti del {0}", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
                    sd.oggetto = oggetto;

                    DocsPaVO.utente.Ruolo r = new Ruolo();
                    r = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(respCons.idGruppo);

                    // Salvataggio del documento
                    DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(respCons);
                    sd = BusinessLogic.Documenti.ProtoManager.addOggettoLocked(idAmm, sd);

                    if (docManager.CreateDocumentoGrigio(sd, r))
                    {
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                        fr = BusinessLogic.Documenti.FileManager.putFile(fr, fd, respCons);
                        if (fr == null)
                            throw new Exception("Errore nell'upload del report nel sistema");
                    }
                    else
                    {
                        throw new Exception("Errore nella creazione del documento");
                    }

                    #endregion

                    #region Trasmissione - disabilitato
                    // MEV Reportistica 2017
                    // I report non sono più notificati

                    /*
                    // Trasmissione
                    DocsPaVO.documento.InfoDocumento infoDoc = new InfoDocumento();
                    infoDoc.idProfile = sd.systemId;
                    infoDoc.docNumber = sd.docNumber;
                    infoDoc.oggetto = sd.oggetto.descrizione;
                    infoDoc.tipoProto = "G";

                    DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                    DocsPaVO.trasmissione.RagioneTrasmissione ragione = new DocsPaVO.trasmissione.RagioneTrasmissione();
                    ArrayList trasmSingole = new ArrayList();
                    ArrayList trasmUtente = new ArrayList();

                    ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneNotifica(respCons.idAmministrazione);
                    ragione.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.WRITE;
                    trasm.ruolo = r;
                    trasm.utente = BusinessLogic.Utenti.UserManager.getUtenteNoFiltroDisabled(respCons.idPeople);
                    trasm.utente.dst = respCons.dst;
                    trasm.infoDocumento = infoDoc;

                    string note = string.Empty;
                    if (tipo.Equals("R"))
                        note = string.Format("Report versamenti rifiutati del {0}", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
                    else
                        note = string.Format("Report versamenti falliti del {0}", DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));
                    trasm.noteGenerali = note;

                    // Trasmissione al ruolo responsabile della conservazione
                    DocsPaVO.trasmissione.TrasmissioneSingola tsRespCons = new DocsPaVO.trasmissione.TrasmissioneSingola();
                    tsRespCons.corrispondenteInterno = r;
                    tsRespCons.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                    tsRespCons.tipoTrasm = "S";
                    tsRespCons.ragione = ragione;

                    // Costruisco le trasmissioni singole
                    ArrayList listaUtC = new ArrayList();
                    listaUtC = BusinessLogic.Utenti.UserManager.getListaUtentiByIdRuolo(r.systemId);
                    foreach (Utente u in listaUtC)
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente trasmUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                        trasmUt.utente = u;
                        tsRespCons.trasmissioneUtente.Add(trasmUt);
                    }
                    trasm.trasmissioniSingole.Add(tsRespCons);

                    DocsPaVO.trasmissione.Trasmissione result = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(string.Empty, trasm);

                    // Inserimento log
                    if (trasm != null)
                    {
                        logger.Debug("INSERIMENTO LOG");
                        if (trasm.infoDocumento != null && !string.IsNullOrEmpty(trasm.infoDocumento.idProfile))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasm.trasmissioniSingole)
                            {
                                string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                string desc = "Trasmesso Documento : " + trasm.infoDocumento.docNumber.ToString();
                                BusinessLogic.UserLog.UserLog.WriteLog(trasm.utente.userId, trasm.utente.idPeople, trasm.ruolo.idGruppo, idAmm,
                                    method, trasm.infoDocumento.idProfile, desc, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", single.systemId);
                            }
                        }
                    }
                    */
                    #endregion

                    retVal = string.Empty;
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    retVal = ex.ToString();
                    logger.Debug(ex.Message);
                }
            }

            logger.Debug("END");
            return retVal;

        }

        private static string CountDocVersati(string stato, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetCountVersamentiGiornoPrecedente(stato, idAmm);
        }

        private static string CountPolicyAttiveAmm(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione c = new DocsPaDB.Query_DocsPAWS.Conservazione();
            return c.GetCountPolicyAttiveAmm(idAmm);
        }

        private static string GetCodRF(string idDoc)
        {
            string result = string.Empty;
            ConservazioneManager man = new ConservazioneManager();
            try
            {
                DocsPaVO.areaConservazione.StampaRegistro stampa = man.getInfoStampaRepertorio(idDoc);
                DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getOggettoById(stampa.idRepertorio);

                if (ogg != null && ogg.TIPO_CONTATORE.Equals("R"))
                {
                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(stampa.idRegistro);
                    result = string.Format("({0})", reg.codRegistro);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            return result;
        }

        private static SchedaDocumento SaveReportDocument(DocsPaVO.Conservazione.PARER.PolicyPARER policy, DocsPaVO.Conservazione.PARER.EsecuzionePolicy info, InfoUtente utente, DocsPaVO.documento.FileDocumento fd)
        {
            // Creazione schedadocumento
            DocsPaVO.documento.SchedaDocumento doc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(utente);

            doc.appId = "ACROBAT";
            doc.tipoProto = "G";
            doc.typeId = "LETTERA";
            doc.oggetto = new Oggetto() { descrizione = string.Format("Report esecuzione policy {0} del {1}", policy.codice, info.dataUltimaEsecuzione) };
            doc.privato = "1";

            // Creazione oggetto ruolo responsabile
            DocsPaVO.utente.Ruolo r = new Ruolo();
            r = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(utente.idGruppo);

            // Salvataggio del documento
            DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(utente);

            // Salvataggio dell'oggetto
            doc = BusinessLogic.Documenti.ProtoManager.addOggettoLocked(utente.idAmministrazione, doc);

            if (docManager.CreateDocumentoGrigio(doc, r))
            {
                DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)doc.documenti[0];
                fr = BusinessLogic.Documenti.FileManager.putFile(fr, fd, utente);
                if (fr == null)
                    throw new Exception("Errore nell'upload del report della policy nel sistema.");
            }

            return doc;
        }

        private static DocsPaVO.trasmissione.Trasmissione ExecuteTransmission(InfoUtente respCons, InfoUtente respPolicy, SchedaDocumento doc, string note)
        {

            logger.Debug("BEGIN");

            try
            {
                // Ruolo responsabile della conservazione
                Ruolo ruoloRespCons = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(respCons.idGruppo);

                // Utente responsabile della conservazione
                //Utente utRespCons = BusinessLogic.Utenti.UserManager.getUtenteNoFiltroDisabled(respCons.idPeople);

                DocsPaVO.documento.InfoDocumento infoDoc = new InfoDocumento();
                infoDoc.idProfile = doc.systemId;
                infoDoc.docNumber = doc.docNumber;
                infoDoc.oggetto = doc.oggetto.descrizione;
                infoDoc.tipoProto = "G";

                DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                DocsPaVO.trasmissione.RagioneTrasmissione ragione = new DocsPaVO.trasmissione.RagioneTrasmissione();
                ArrayList trasmSingole = new ArrayList();
                ArrayList trasmUtente = new ArrayList();

                ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneNotifica(respCons.idAmministrazione);
                ragione.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.WRITE;
                trasm.ruolo = ruoloRespCons;
                trasm.utente = BusinessLogic.Utenti.UserManager.getUtenteNoFiltroDisabled(respCons.idPeople);
                trasm.utente.dst = respCons.dst;
                trasm.infoDocumento = infoDoc;
                trasm.noteGenerali = note;

                // Trasmissione al ruolo responsabile della conservazione
                DocsPaVO.trasmissione.TrasmissioneSingola tsRespCons = new DocsPaVO.trasmissione.TrasmissioneSingola();
                tsRespCons.corrispondenteInterno = ruoloRespCons;
                tsRespCons.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                tsRespCons.tipoTrasm = "S";
                tsRespCons.ragione = ragione;

                // Costruisco le trasmissioni singole
                ArrayList listaUtC = new ArrayList();
                listaUtC = BusinessLogic.Utenti.UserManager.getListaUtentiByIdRuolo(ruoloRespCons.systemId);
                foreach (Utente u in listaUtC)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trasmUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trasmUt.utente = u;
                    tsRespCons.trasmissioneUtente.Add(trasmUt);
                }
                trasm.trasmissioniSingole.Add(tsRespCons);

                // Trasmissione al ruolo responsabile della policy (se configurato)
                if (respPolicy != null && !string.IsNullOrEmpty(respPolicy.idGruppo))
                {
                    Ruolo ruoloRespPolicy = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(respPolicy.idGruppo);
                    DocsPaVO.trasmissione.TrasmissioneSingola tsRespPolicy = new DocsPaVO.trasmissione.TrasmissioneSingola();
                    tsRespPolicy.corrispondenteInterno = ruoloRespPolicy;
                    tsRespPolicy.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                    tsRespPolicy.tipoTrasm = "S";
                    tsRespPolicy.ragione = ragione;

                    ArrayList listaUtenti = new ArrayList();
                    listaUtenti = BusinessLogic.Utenti.UserManager.getListaUtentiByIdRuolo(ruoloRespPolicy.systemId);
                    foreach (Utente u in listaUtenti)
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente trasmUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                        trasmUt.utente = u;
                        tsRespPolicy.trasmissioneUtente.Add(trasmUt);
                    }
                    if (listaUtenti != null)
                        trasm.trasmissioniSingole.Add(tsRespPolicy);
                }

                DocsPaVO.trasmissione.Trasmissione result = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(string.Empty, trasm);
                logger.Debug("END");

                return result;

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return null;
            }
        }

        private static void SendMailNotification(string idAmm, DocsPaVO.Conservazione.PARER.PolicyPARER policy)
        {
            logger.Debug("BEGIN");

            DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();

            try
            {
                DocsPaVO.Conservazione.PARER.ReportConfiguration config = cons.GetReportConfiguration(idAmm);
                if (config == null)
                {
                    throw new Exception("Configurazione report conservazione assente o errata");
                }
                if (config.MailBoxConfiguration == null)
                {
                    throw new Exception("Configurazione casella mail conservazione assente o errata");
                }

                string body = config.PolicyBody;
                body = body.Replace("#CODICE_POLICY#", policy.codice);
                body = body.Replace("#DESCRIZIONE_POLICY#", policy.descrizione);
                body = body.Replace("#DATA_ORA_POLICY#", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                string msgTo = string.Empty;
                if(config.PolicyRecipients != null && config.PolicyRecipients.Count() > 0)
                {
                    for(int i=0; i<config.PolicyRecipients.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(msgTo))
                            msgTo = msgTo + ",";

                        msgTo = msgTo + config.PolicyRecipients[i];
                    }
                }

                BusinessLogic.Interoperabilità.SvrPosta svr = new Interoperabilità.SvrPosta(config.MailBoxConfiguration.Server, config.MailBoxConfiguration.Username, config.MailBoxConfiguration.Password, config.MailBoxConfiguration.Port, System.IO.Path.GetTempPath(), BusinessLogic.Interoperabilità.CMClientType.SMTP, (config.MailBoxConfiguration.UseSSL ? "1" : "0"), string.Empty, "0");
                svr.connect();
                string error = string.Empty;
                svr.sendMail(config.MailBoxConfiguration.From, msgTo, "", string.Empty, config.PolicySubject, body, BusinessLogic.Interoperabilità.CMMailFormat.HTML, null, out error);
                if(!string.IsNullOrEmpty(error))
                {
                    logger.Debug("Mail non inviata: " + error);
                }

            }
            catch(Exception ex)
            {
                logger.Debug("Errore nell'invio della mail - ", ex);
            }

            logger.Debug("END");
        }
    }
}
