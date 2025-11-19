// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using BusinessLogic.Interoperabilita;
using BusinessLogic.RubricaComune;
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Serilog;
using System.Collections;

namespace BusinessLogic.LibroFirma;

public class LibroFirmaManager
{
    private static ILogger logger = Log.ForContext(typeof(LibroFirmaManager));

    public delegate void EseguiPassoAutomaticoDelegate(IstanzaPassoDiFirma passo, IstanzaProcessoDiFirma istanzaProcesso, IBaseRubricaComuneService rubricaComuneService);

    private static void CallBack(IAsyncResult result)
    {

        var del = result.AsyncState as EseguiPassoAutomaticoDelegate;

        if (del != null)
            del.EndInvoke(result);
    }


    public static List<ProcessoFirma> GetProcessiDiFirmaByIdAmm(string idAmministrazione)
    {
        List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            listProcessiDiFirma = libroFirma.GetProcessiFirmaByIdAmm(idAmministrazione);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessiDiFirmaByIdAmm ", e);
        }
        return listProcessiDiFirma;
    }

    public static IstanzaProcessoDiFirma GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(string idIstanzaProcesso, DocsPaVO.utente.InfoUtente infoUtente)
    {
        IstanzaProcessoDiFirma istanzaProcessoDiFirma = null;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            istanzaProcessoDiFirma = libroFirma.GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(idIstanzaProcesso, infoUtente);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetIstanzaProcessoDiFirmaByIdIstanzaProcesso ", e);
        }
        return istanzaProcessoDiFirma;
    }

    public static bool IsDocInLibroFirma(string docNumber)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            return libroFirma.IsDocInLibroFirma(docNumber);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsDocInLibroFirma ", e);
            return false;
        }
    }

    public static bool IsTitolarePassoInAttesa(string docnumber, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.LibroFirma.Azione azione)
    {
        bool result = true;
        DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
        IstanzaPassoDiFirma istanza = libroFirma.GetIstanzaPassoFirmaInAttesaByDocnumber(docnumber);
        if (istanza != null)
        {
            if (!infoUtente.idGruppo.Equals(istanza.RuoloCoinvolto.idGruppo))
                result = false;
            if (!string.IsNullOrEmpty(istanza.UtenteCoinvolto.idPeople) && !infoUtente.idPeople.Equals(istanza.UtenteCoinvolto.idPeople))
                result = false;
            if (!string.IsNullOrEmpty(istanza.UtenteLocker) && !infoUtente.idPeople.Equals(istanza.UtenteCoinvolto.idPeople))
                result = false;
            if (!istanza.TipoFirma.Equals(azione.ToString()))
                result = false;
        }
        return result;
    }

    public static bool CanExecuteAction(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente)
    {
        bool result = true;
        try
        {
            //Se il documento è in libro firma, posso eseguire l'azione solo se sono il titolare del passo e l'azione corrisponde al passo
            if (IsDocInLibroFirma(fileRequest.docNumber))
            {
                //Caso allegato o doc principale: se sono il titolare del passo posso eseguire l'azione.
                if (IsTitolare(fileRequest.docNumber, infoUtente))
                    return true;
                else
                    return false;
            }

            if (IsModificaBloccataPerDocumentoPrincipaleInLF(fileRequest.docNumber, infoUtente.idAmministrazione))
                return false;

        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: CanExecuteAction ", e);
        }
        return result;
    }

    public static bool IsTitolare(string docNumber, DocsPaVO.utente.InfoUtente infoUtente)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            return libroFirma.IsTitolare(docNumber, infoUtente.idGruppo, infoUtente.idPeople);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsTitolare ", e);
            return false;
        }
    }

    public static bool IsModificaBloccataPerDocumentoPrincipaleInLF(string docnumber, string idAmm)
    {
        bool result = false;
        try
        {
            if (IsAttivoBloccoModificheDocumentoInLibroFirma(idAmm) && IsDocumentoPrincipaleInLF(docnumber))
                return true;
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsModificaBloccataPerDocumentoPrincipaleInLF ", e);
        }
        return result;
    }

    public static bool IsAttivoBloccoModificheDocumentoInLibroFirma(string idAmm)
    {
        bool result = false;
        try
        {
            string attivo = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BLOCCO_MODIFICHE_DOC_IN_LF");
            if (!string.IsNullOrEmpty(attivo) && !attivo.Equals("0"))
            {
                result = true;
            }
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsAttivoBloccoDocumentiInLibroFirma ", e);
            return false;
        }
        return result;
    }

    public static bool IsDocumentoPrincipaleInLF(string docNumber)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            return libroFirma.IsDocumentoPrincipaleInLF(docNumber);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsDocumentoPrincipaleInLF ", e);
            return false;
        }
    }

    public static int GetCountProcessiDiFirmaByUtenteTitolare(string idUtenteTitolare, string idRuoloCoinvolto)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            return libroFirma.GetCountProcessiDiFirmaByUtenteTitolare(idUtenteTitolare, idRuoloCoinvolto);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessiDiFirmaByUtenteTitolare ", e);
            return 0;
        }
    }

    public static int GetCountIstanzaProcessiDiFirmaByUtenteTitolare(string idUtenteCoinvolto, string idRuoloCoinvolto)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            return libroFirma.GetCountIstanzaProcessiDiFirmaByUtenteTitolare(idUtenteCoinvolto, idRuoloCoinvolto);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetCountIstanzaProcessiDiFirmaByUtenteTitolare ", e);
            return 0;
        }
    }

    /// <summary>
    /// Invalida i processi di firma ed interrompe le istanze del ruolo/utente specificato
    /// </summary>
    /// <param name="idRuolo"></param>
    /// <param name="idPeople"></param>
    /// <returns></returns>
    public static bool InvalidaPassiCorrelatiTitolare(string idRuolo, string idPeople, string tipoTick, DocsPaVO.utente.InfoUtente infoUtente, ISpreadsheetService spreadsheetService, IFileConverterFactory fileConverterPDF, IReportGeneratorService reportGeneratorService, IBaseRubricaComuneService rubricaComuneService)
    {
        bool result = true;
        try
        {
            //Inserisco i processi da invalidare in una tabella
            DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            string idReport = libro.InsertReportProcessiTick(idPeople, idRuolo);

            if (InvalidaProcessiTitolare(idRuolo, idPeople, tipoTick))
                InviaReportCreatoriProcessi(idReport, infoUtente, spreadsheetService, fileConverterPDF, reportGeneratorService);
            InterrompiIstanzeTitolare(idRuolo, idPeople, infoUtente, rubricaComuneService);

            //libro.DeleteReportProcessiTick(idReport);
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InvalidaPassiCorrelatiTitolare ", ex);
        }
        return result;
    }

    private static bool InvalidaProcessiTitolare(string idRuolo, string idPeople, string tipoTick)
    {
        bool result = true;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();

            //Se il processo è associato ad un diagramma degli stati lo rimuovo
            libroFirma.DisassociaProcessoDaDiagrammaStato(idRuolo, idPeople);

            result = libroFirma.InvalidaProcessiFirmaTitolare(idRuolo, idPeople, tipoTick);
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InvalidaProcessiTitolare ", ex);
        }
        return result;
    }

    /// <summary>
    /// Invio un Report con i processi invalidati rispettivamente ad ogni ruolo creatore
    /// </summary>
    /// <param name="idReport"></param>
    /// <param name="infoUtente"></param>
    private static void InviaReportCreatoriProcessi(string idReport, DocsPaVO.utente.InfoUtente infoUtente, ISpreadsheetService spreadsheetService, IFileConverterFactory fileConverterPDF, IReportGeneratorService reportGeneratorService)
    {
        DocsPaVO.documento.FileDocumento report = new DocsPaVO.documento.FileDocumento();
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            List<string> listaIdRuoliCreatori = libro.GetListaIdCreatoriProcessiInvalidati(idReport);
            if (listaIdRuoliCreatori != null && listaIdRuoliCreatori.Count > 0)
            {
                string basePathFiles = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                string oggetto = "Report processi invalidati";
                string body = "A seguito delle modifiche dei ruoli coinvolti nei processi/modelli di firma da Lei creati, viene inoltrato il report dei processi invalidati.";
                //Ricerca se esiste l'email from notifica dell'amministrazione
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                string idAmministrazionePerMail = string.Empty;
                string fromEmailAmministra = amm.GetEmailAddress(infoUtente.idAmministrazione);
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                foreach (string idRuoloCreatore in listaIdRuoliCreatori)
                {
                    report = GeneraReportCreatoreProcessi(idReport, idRuoloCreatore, infoUtente, spreadsheetService, fileConverterPDF, reportGeneratorService);

                    //Estraggo gli utenti a cui inviare il report
                    DocsPaVO.utente.Ruolo ruolo = utenti.GetRuoloByIdGruppo(idRuoloCreatore);
                    System.Collections.ArrayList listaUtenti = new System.Collections.ArrayList();
                    DocsPaVO.addressbook.QueryCorrispondente qc = new DocsPaVO.addressbook.QueryCorrispondente();
                    qc.codiceRubrica = ruolo.codiceRubrica;
                    System.Collections.ArrayList registri = ruolo.registri;
                    qc.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                    //qc.idRegistri = registri;
                    qc.idAmministrazione = ruolo.idAmministrazione;
                    qc.getChildren = true;
                    qc.fineValidita = true;
                    listaUtenti = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qc);
                    string emailsDest = string.Empty;
                    string emailUser = string.Empty;
                    for (int k = 0; k < listaUtenti.Count; k++)
                    {
                        emailUser = ((DocsPaVO.utente.Utente)listaUtenti[k]).email;
                        if (!string.IsNullOrEmpty(emailUser))
                            emailsDest += string.IsNullOrEmpty(emailsDest) ? emailUser : "," + emailUser;
                    }
                    if (!string.IsNullOrEmpty(emailsDest))
                    {
                        CMAttachment[] allegato = new CMAttachment[1];
                        allegato[0] = CreaAllegatoMail(report);
                        if (allegato[0] != null)
                        {
                            bool res = false;
                            res = BusinessLogic.Interoperabilita.Notifica.notificaByMail(emailsDest, fromEmailAmministra, oggetto, body, string.Empty, ruolo.idAmministrazione, allegato);
                            if (!res)
                            {
                                logger.Error("Errore durante l'invio della mail contentente il report");
                            }
                        }

                    }
                }
            }

        }
        catch (Exception ex)
        {
            logger.Debug(ex.Message);
        }
    }

    private static bool InterrompiIstanzeTitolare(string idRuolo, string idPeople, DocsPaVO.utente.InfoUtente infoUtente, IBaseRubricaComuneService rubricaComuneService)
    {
        bool result = true;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            List<IstanzaProcessoDiFirma> istanzaProcessiCoinvolti = libroFirma.GetIstanzaProcessiDiFirmaByTitolare(idRuolo, idPeople);

            if (istanzaProcessiCoinvolti.Count() > 0)
            {
                string noteInterruzione = "Interrotto processo per modifica da amministrazione di un ruolo coinvolto.";
                result = BusinessLogic.LibroFirma.LibroFirmaManager.InterruptionSignatureProcessByAdministrator(istanzaProcessiCoinvolti.ToArray(), noteInterruzione, infoUtente, rubricaComuneService);
            }
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InterrompiIstanzeRuoloCoinvolto ", ex);
        }
        return result;
    }

    private static DocsPaVO.documento.FileDocumento GeneraReportCreatoreProcessi(string idReport, string idRuoloCreatore, DocsPaVO.utente.InfoUtente infoUtente, ISpreadsheetService spreadsheetService, IFileConverterFactory fileConverterPDF, IReportGeneratorService reportGeneratorService)
    {
        DocsPaVO.documento.FileDocumento report = new DocsPaVO.documento.FileDocumento();
        try
        {
            List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idReport", valore = idReport });
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idRuoloCreatore", valore = idRuoloCreatore });

            // request generazione report
            DocsPaVO.Report.PrintReportRequest request = new DocsPaVO.Report.PrintReportRequest();
            request.SearchFilters = filters;
            request.ReportType = DocsPaVO.Report.ReportTypeEnum.PDF;
            request.Title = "Processi Invalidati";

            request.ContextName = "ExportProcessiDiFirmaInvalidati";
            request.ReportKey = "ExportProcessiDiFirmaInvalidati";
            request.Title = string.Empty;
            request.AdditionalInformation = string.Empty;

            report = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request, spreadsheetService, fileConverterPDF, reportGeneratorService).Document;

        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
            report = null;
        }
        return report;
    }

    public static bool InterruptionSignatureProcessByAdministrator(DocsPaVO.LibroFirma.IstanzaProcessoDiFirma[] istanzeProcessi, string noteInterruzione, DocsPaVO.utente.InfoUtente infoUtente, IBaseRubricaComuneService rubricaComuneService)
    {
        bool result = false;
        string msg = string.Empty;
        string varMetodo = string.Empty;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            foreach (DocsPaVO.LibroFirma.IstanzaProcessoDiFirma istanza in istanzeProcessi)
            {
                // Contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    string idPassoInAttesa = (from i in istanza.istanzePassoDiFirma
                                              where i.statoPasso.Equals(TipoStatoPasso.LOOK)
                                              select i.idIstanzaPasso).FirstOrDefault();
                    string dateInterruption = DateTime.Now.ToString();
                    libroFirma.AggiornaDataEsecuzioneElemento(istanza.docNumber, DocsPaVO.LibroFirma.TipoStatoElemento.INTERROTTO.ToString());
                    ElementoInLibroFirma elemento = libroFirma.GetElementiInLibroFirmaByIdIstanzaPasso(idPassoInAttesa);
                    if (libroFirma.EliminaElementoInLibroFirma(idPassoInAttesa))
                    {
                        string interrottoDa = "A"; //Amministratore
                        result = libroFirma.InterruptionSignatureProcess(istanza.idIstanzaProcesso, TipoStatoProcesso.STOPPED, istanza.docNumber, noteInterruzione, dateInterruption, interrottoDa, infoUtente);
                        if (result)
                        {

                            SalvaStoricoIstanzaProcessoFirma(istanza.idIstanzaProcesso, istanza.docNumber, noteInterruzione, infoUtente);
                            ModificaStatoDocumentoPerInterruzione(istanza, infoUtente, rubricaComuneService);
                            msg = "Interruzione del processo di firma per il file " + istanza.docNumber;
                            varMetodo = istanza.docAll.Equals("D") ? "INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN" : "INTERROTTO_PROCESSO_ALLEGATO_DA_ADMIN";
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, null, infoUtente.idAmministrazione, varMetodo, istanza.docNumber, msg, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", null, dateInterruption);
                        }
                    }
                    if (result)
                    {
                        if (elemento != null && !string.IsNullOrEmpty(elemento.InfoDocumento.IdDocumentoPrincipale))
                            BusinessLogic.LibroFirma.LibroFirmaManager.StopPassoWait(elemento.InfoDocumento.IdDocumentoPrincipale, infoUtente);

                        transactionContext.Complete();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InterruptionSignatureProcessByProponent ", ex);
            return false;
        }

        return result;
    }

    public static void SalvaStoricoIstanzaProcessoFirma(string idIstanza, string docnumber, string azione, DocsPaVO.utente.InfoUtente infoUtente)
    {
        DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
        libroFirma.SalvaStoricoIstanzaProcessoFirma(idIstanza, docnumber, azione, infoUtente);
    }

    public static void ModificaStatoDocumentoPerInterruzione(IstanzaProcessoDiFirma istanza, DocsPaVO.utente.InfoUtente infoUtente, IBaseRubricaComuneService rubricaComuneService)
    {
        //Se il documento è legato ad un diagramma di stato in caso di interruzione e se specifitao nel processo riporto il documento
        //nello stato specificato dal processo.
        //Se invece nel processo non è specificato l'idStatoInterruzione ma il processo è stato avviato automaticamente per cambio stato
        //riporto il documento nello stato precedente
        try
        {
            if (!string.IsNullOrEmpty(istanza.IdStatoInterruzione))
            {
                //Se nell'istanza è presente uno stato di ripristino dle documento in caso di interruzione, porto il documento in quello stato
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diag = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                DocsPaVO.DiagrammaStato.Stato statoDocumento = diag.getStatoDoc(istanza.docNumber);
                if (!istanza.IdStatoInterruzione.Equals(statoDocumento.SYSTEM_ID.ToString()))
                {
                    DocsPaVO.DiagrammaStato.Stato statoInterruzione = diag.GetStatoById(istanza.IdStatoInterruzione, infoUtente);
                    DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diag.getDiagrammaById(Convert.ToString(statoInterruzione.ID_DIAGRAMMA));
                    SalvaStatoDiagrammaDoc(statoInterruzione, diagramma, istanza.docNumber, infoUtente, rubricaComuneService);
                }
            }
            else if (istanza.AttivatoPerPassaggioStato)
            {
                //Verifico se è stato avviato dal passaggio di stato, in caso torno allo stato precedente
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diag = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                DocsPaVO.DiagrammaStato.Stato statoDiagrammaPrecedente = diag.GetStatoDocPrecedente(istanza.docNumber);
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diag.getDiagrammaById(Convert.ToString(statoDiagrammaPrecedente.ID_DIAGRAMMA));
                SalvaStatoDiagrammaDoc(statoDiagrammaPrecedente, diagramma, istanza.docNumber, infoUtente, rubricaComuneService);
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in ModificaStatoDocumentoPerInterruzione " + e.Message);
        }
    }

    public static bool StopPassoWait(string docNumber, DocsPaVO.utente.InfoUtente infoUtente)
    {
        bool retVal = false;

        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            List<IstanzaProcessoDiFirma> listIstanzaProcessoDiFirma = libroFirma.GetIstanzaProcessoDiFirmaByDocnumber(docNumber, infoUtente);

            if (listIstanzaProcessoDiFirma != null && listIstanzaProcessoDiFirma.Count() > 0)
            {
                foreach (IstanzaProcessoDiFirma istanzaProcessoDiFirma in listIstanzaProcessoDiFirma)
                {
                    if (string.IsNullOrEmpty(istanzaProcessoDiFirma.dataChiusura))
                    {
                        List<IstanzaPassoDiFirma> listIstanzaPassi = libroFirma.GetIstanzePassoDiFirma(istanzaProcessoDiFirma.idIstanzaProcesso);

                        foreach (IstanzaPassoDiFirma istanzaPasso in listIstanzaPassi)
                        {
                            if (istanzaPasso.Evento.TipoEvento.Equals("W"))
                            {
                                DocsPaDB.Query_Utils.Utils date = new DocsPaDB.Query_Utils.Utils();
                                string dataEvento = date.GetDBDate(true);

                                if (istanzaPasso.statoPasso.Equals(TipoStatoPasso.LOOK))
                                {
                                    libroFirma.StopProcessSteps(istanzaProcessoDiFirma.idIstanzaProcesso, istanzaPasso.numeroSequenza);

                                    //string dataEvento = DocsPaDbManagement.Functions.Functions.ToDate(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt",  new System.Globalization.CultureInfo("it-IT")));

                                    if (libroFirma.SetProcesComplete(istanzaProcessoDiFirma.idIstanzaProcesso, DocsPaVO.LibroFirma.TipoStatoProcesso.CLOSED, istanzaProcessoDiFirma.docNumber, dataEvento))
                                    {

                                        string method_TRONCAMENTO = "TRONCAMENTO_PROCESSO";
                                        string description_TRONCAMENTO = "Anomalia nel processo per interruzione su allegato.";

                                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method_TRONCAMENTO, istanzaProcessoDiFirma.docNumber,
                                        description_TRONCAMENTO, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", "", dataEvento);

                                        SalvaStoricoIstanzaProcessoFirma(istanzaProcessoDiFirma.idIstanzaProcesso, istanzaProcessoDiFirma.docNumber, description_TRONCAMENTO, infoUtente);

                                        string method_CONCLUSIONE = "CONCLUSIONE_PROCESSO_LF_DOCUMENTO";
                                        string description_CONCLUSIONE = "Conclusione del processo di firma per documento.";

                                        SalvaStoricoIstanzaProcessoFirma(istanzaProcessoDiFirma.idIstanzaProcesso, istanzaProcessoDiFirma.docNumber, description_CONCLUSIONE, infoUtente);

                                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method_CONCLUSIONE, istanzaProcessoDiFirma.docNumber,
                                        description_CONCLUSIONE, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", "", dataEvento);

                                        retVal = true;
                                    }

                                    break;
                                }
                                else if (istanzaPasso.statoPasso.Equals(TipoStatoPasso.NEW))
                                {
                                    libroFirma.StopProcessSteps(istanzaProcessoDiFirma.idIstanzaProcesso, istanzaPasso.numeroSequenza);

                                    string method_TRONCAMENTO = "TRONCAMENTO_PROCESSO";
                                    string description_TRONCAMENTO = "Anomalia nel processo per interruzione su allegato.";

                                    BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method_TRONCAMENTO, istanzaProcessoDiFirma.docNumber,
                                    description_TRONCAMENTO, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1", "", dataEvento);

                                    SalvaStoricoIstanzaProcessoFirma(istanzaProcessoDiFirma.idIstanzaProcesso, istanzaProcessoDiFirma.docNumber, description_TRONCAMENTO, infoUtente);
                                }
                            }
                        }

                        break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetIstanzaProcessoDiFirmaByDocnumber ", e);
        }
        return retVal;
    }

    public static void SalvaStatoDiagrammaDoc(DocsPaVO.DiagrammaStato.Stato stato, DocsPaVO.DiagrammaStato.DiagrammaStato diagramma, string docnumber, DocsPaVO.utente.InfoUtente infoUtente, IBaseRubricaComuneService rubricaComuneService)
    {
        try
        {
            BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(docnumber, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.userId, infoUtente, string.Empty, rubricaComuneService);
            string method = "DOC_CAMBIO_STATO";

            //Se non ho il ruolo vuol dire che stò effettuando l'operazione d'amministrazione
            if (string.IsNullOrEmpty(infoUtente.idGruppo))
                method = "DOC_CAMBIO_STATO_ADMIN";

            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method, docnumber, String.Format("Stato passato a  {0}", stato.DESCRIZIONE.ToUpper()), DocsPaVO.Logger.CodAzione.Esito.OK,
                infoUtente.delegato, "1");
        }
        catch (Exception e)
        {
            logger.Error("Errore durante il cambio di stato " + e);
        }
    }

    public static bool IsDocOrAllInLibroFirma(string docNumber)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            return libroFirma.IsDocOrAllInLibroFirma(docNumber);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: IsDocOrAllInLibroFirma ", e);
            return false;
        }
    }

    /// <summary>
    /// Avvia un processo di firma
    /// </summary>
    /// <param name="processo">Processo originale</param>
    /// <param name="docNumber">Documento/allegato scelto</param>
    /// /// <param name="versionId">Versione del file</param>
    /// <param name="infoUtente">Utente operatore</param>
    /// <returns>true/false</returns>
    public static bool StartProcessoDiFirma(ProcessoFirma processoDiFirma, DocsPaVO.documento.FileRequest file, DocsPaVO.utente.InfoUtente infoUtente, string modalita, string note, DocsPaVO.LibroFirma.OpzioniNotifica opzioniNotifiche, IBaseRubricaComuneService rubricaComuneService, out DocsPaVO.LibroFirma.ResultProcessoFirma resultAvvioProcesso, bool daCambioStato = false)
    {
        bool retVal = false;
        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.OK;
        IstanzaProcessoDiFirma istanzaProcesso = null;
        DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        IstanzaPassoDiFirma istanzaPassoCorrente = new IstanzaPassoDiFirma();

        //Verifico se è possibile avviare il processo di firma
        if (!CanAvvioProcessoFirma(processoDiFirma, file, infoUtente, out resultAvvioProcesso))
        {
            return false;
        }

        bool canExecuteTransmission = false;
        string newIdElemento = string.Empty;
        try
        {
            // Contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                if (!libroFirma.IsDocInLibroFirma(file.docNumber) && libroFirma.UpdateLockDocument(file.docNumber, "1"))
                {
                    //Se la chiave è attiva la notifica si presenza di destintari interoperanti è obbligatoria
                    string attiva = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "NOTIFICA_DEST_NO_INTEROP_OBB");
                    if (!string.IsNullOrEmpty(attiva) && attiva.Equals("1"))
                    {
                        opzioniNotifiche.NotificaPresenzaDestNonInterop = true;
                    }
                    istanzaProcesso = libroFirma.CreateIstanzaFromProcesso(processoDiFirma, file, infoUtente, note, opzioniNotifiche, daCambioStato);
                    DocsPaVO.documento.InfoDocumento infoDoc = doc.GetInfoDocumentoLite(istanzaProcesso.docNumber);
                    istanzaPassoCorrente = libroFirma.GetIstanzaPassoDiFirmaInAttesa(istanzaProcesso.idIstanzaProcesso);

                    //Se il tipo di passo è di ATTESA("WAIT") non devo trasmettere
                    if (!istanzaPassoCorrente.Evento.TipoEvento.Equals("W"))
                    {

                        if (!istanzaPassoCorrente.Evento.TipoEvento.Equals("E"))
                        {
                            newIdElemento = libroFirma.InsertElementoInLibroFirma(istanzaProcesso, infoUtente, modalita);
                            if (!string.IsNullOrEmpty(newIdElemento))
                            {
                                retVal = true;
                                //PER EVITARE DI TRASMETTERE PIù VOLTE LO STESSO DOCUMENTO, CONTROLLO CHE NON ESISTE NEL LIBRO FIRMA DELL'UTENTE UN ALLEGATO DI UN DOCUMENTO GIà TRASMESSO
                                canExecuteTransmission = libroFirma.CanExecuteTransmission(infoDoc.docNumber, istanzaPassoCorrente.RuoloCoinvolto.idGruppo, istanzaPassoCorrente.UtenteCoinvolto.idPeople, newIdElemento);
                            }
                            else
                            {
                                libroFirma.RollbackStartProcessoDiFirma(istanzaProcesso, file);
                                return false;
                            }
                        }
                        else
                        {
                            canExecuteTransmission = true;
                            retVal = true;
                        }
                    }
                    else
                    {
                        //Se è passo di wait verifico se ci sono allegati in lf, se non ci sono vado al passo successivo
                        List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> listAllInProcess = libroFirma.GetInfoProcessesStartedForDocument(istanzaProcesso.docNumber);
                        listAllInProcess = (from l in listAllInProcess where l.docAll.Equals("A") select l).ToList();
                        if (listAllInProcess == null || listAllInProcess.Count == 0)
                        {
                            libroFirma.UpdateStatoIstanzaPasso(istanzaPassoCorrente.idIstanzaPasso, string.Empty, DocsPaVO.LibroFirma.TipoStatoPasso.CLOSE.ToString(), infoUtente);
                            istanzaPassoCorrente = libroFirma.GetNextIstanzaPasso(istanzaPassoCorrente.idIstanzaProcesso, istanzaPassoCorrente.numeroSequenza, istanzaProcesso.versionId);
                            if (istanzaPassoCorrente != null && (!string.IsNullOrEmpty(istanzaPassoCorrente.idIstanzaPasso)))
                            {
                                libroFirma.UpdateStatoIstanzaPasso(istanzaPassoCorrente.idIstanzaPasso, istanzaProcesso.versionId, DocsPaVO.LibroFirma.TipoStatoPasso.LOOK.ToString(), infoUtente);
                            }

                            //Devo inserire in libro firma
                            if (istanzaPassoCorrente != null && !istanzaPassoCorrente.Evento.TipoEvento.Equals("E"))
                            {
                                newIdElemento = libroFirma.InsertElementoInLibroFirma(istanzaProcesso, infoUtente, modalita);
                                if (!string.IsNullOrEmpty(newIdElemento))
                                {
                                    retVal = true;
                                    //PER EVITARE DI TRASMETTERE PIù VOLTE LO STESSO DOCUMENTO, CONTROLLO CHE NON ESISTE NEL LIBRO FIRMA DELL'UTENTE UN ALLEGATO DI UN DOCUMENTO GIà TRASMESSO
                                    canExecuteTransmission = libroFirma.CanExecuteTransmission(infoDoc.docNumber, istanzaPassoCorrente.RuoloCoinvolto.idGruppo, istanzaPassoCorrente.UtenteCoinvolto.idPeople, newIdElemento);
                                }
                                else
                                {
                                    libroFirma.RollbackStartProcessoDiFirma(istanzaProcesso, file);
                                    return false;
                                }
                            }
                            else
                            {
                                canExecuteTransmission = true;
                                retVal = true;
                            }
                        }
                        retVal = true;
                    }

                    #region TRASMISSIONE DEL DOCUMENTO
                    //INVIO TRAMISSIONE

                    if (canExecuteTransmission)
                    {
                        DocsPaVO.trasmissione.Trasmissione result = ExecuteTransmission(istanzaProcesso, istanzaPassoCorrente, infoDoc, infoUtente);
                        string desc = string.Empty;
                        string method = "TRASM_DOC_" + (result.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).ragione.descrizione.ToUpper().Replace(" ", "_");
                        if (result.infoDocumento.segnatura == null)
                            desc = "Trasmesso Documento : " + result.infoDocumento.docNumber.ToString();
                        else
                            desc = "Trasmesso Documento : " + result.infoDocumento.segnatura.ToString();
                        if (result != null)
                        {
                            if (!string.IsNullOrEmpty(newIdElemento))
                                libroFirma.UpdateIdTrasmInElementoLF(newIdElemento, (result.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId, string.Empty);
                            string checkNotify = libroFirma.EventToBeNotified(istanzaPassoCorrente, "INSERIMENTO_DOCUMENTO_LF") ? "1" : "0";
                            if (istanzaPassoCorrente.Evento.TipoEvento.Equals("E"))
                                checkNotify = "1";
                            BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                                (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), checkNotify, (result.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId);

                        }
                        else
                        {
                            retVal = false;
                            BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.KO,
                                    (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "0", (result.trasmissioniSingole[0] as DocsPaVO.trasmissione.TrasmissioneSingola).systemId);
                        }

                    }
                    #endregion
                }
                else
                {
                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_GIA_IN_LIBRO_FIRMA;
                }
                if (retVal && istanzaPassoCorrente.IsAutomatico)
                {
                    //Se il passo è un passo proseguo con la sua esecuzione
                    EseguiPassoAutomaticoAsync(istanzaPassoCorrente, istanzaProcesso, rubricaComuneService);
                }
                if (retVal)
                {
                    // Integrazione con portale - cambio di stato del fascicolo
                    if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ENABLE_PORTALE_PROCEDIMENTI")) && !DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ENABLE_PORTALE_PROCEDIMENTI").Equals("0"))
                    {
                        DocsPaVO.Procedimento.Procedimento proc = null;
                        string idProcedimento = string.Empty;
                        System.Collections.ArrayList listaFasc = Fascicoli.FascicoloManager.getFascicoliDaDocNoSecurity(infoUtente, file.docNumber);
                        if (listaFasc != null && listaFasc.Count > 0)
                        {
                            foreach (DocsPaVO.fascicolazione.Fascicolo fasc in listaFasc)
                            {
                                proc = new DocsPaVO.Procedimento.Procedimento();
                                proc = Procedimenti.ProcedimentiManager.GetProcedimentoByIdFascicolo(fasc.systemID);
                                if (proc != null && proc.Id == fasc.systemID)
                                {
                                    idProcedimento = fasc.systemID;
                                    break;
                                }
                            }
                        }

                        if (proc != null)
                        {
                            DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, file.docNumber);
                            if (schedaDoc != null && schedaDoc.template != null)
                            {
                                BusinessLogic.Procedimenti.ProcedimentiManager.CambioStatoProcedimento(proc.Id, "FIRMA", schedaDoc.template.ID_TIPO_ATTO, infoUtente);
                            }
                        }
                    }
                    SalvaStoricoIstanzaProcessoFirma(istanzaProcesso.idIstanzaProcesso, istanzaProcesso.docNumber, "Avviato processo di firma", infoUtente);

                    transactionContext.Complete();
                }
            }
        }
        catch (Exception e)
        {
            logger.Debug("Errore in StartProcessoDiFirma  - ", e);
            libroFirma.RollbackStartProcessoDiFirma(istanzaProcesso, file);
        }

        return retVal;
    }

    public static void EseguiPassoAutomaticoAsync(IstanzaPassoDiFirma passo, IstanzaProcessoDiFirma istanzaProcesso, IBaseRubricaComuneService rubricaComuneService)
    {
        AsyncCallback callback = new AsyncCallback(CallBack);
        EseguiPassoAutomaticoDelegate esecuzionePassoAutomatico = new EseguiPassoAutomaticoDelegate(EseguiPassoAutomatico);
        esecuzionePassoAutomatico.BeginInvoke(passo, istanzaProcesso, rubricaComuneService, callback, esecuzionePassoAutomatico);
    }

    private static bool CanAvvioProcessoFirma(ProcessoFirma processoDiFirma, DocsPaVO.documento.FileRequest file, DocsPaVO.utente.InfoUtente infoUtente, out DocsPaVO.LibroFirma.ResultProcessoFirma resultAvvioProcesso)
    {
        bool retVal = true;
        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.OK;
        try
        {
            bool isAllegato = false;
            string idDocumentoPrincipale = string.Empty;
            if (file.GetType().Equals(typeof(DocsPaVO.documento.Allegato)))
            {
                DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato() { docNumber = file.docNumber };
                idDocumentoPrincipale = BusinessLogic.Documenti.AllegatiManager.getIdDocumentoPrincipale(all);
                isAllegato = true;
            }
            else
            {
                idDocumentoPrincipale = file.docNumber;
            }

            #region CONTROLLI SUL MODELLO

            if (processoDiFirma.IsProcessModel)
            {
                if (processoDiFirma.passi != null && processoDiFirma.passi.Count() == 1 && processoDiFirma.passi[0].IsFacoltativo && processoDiFirma.passi[0].IncludiInIstanzaPasso == IncludiInIstanzaPasso.NO)
                {
                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_FACOLTATIVO_NON_INCLUSO_UNICO_PASSO_PROCESSO;
                    return false;
                }

                foreach (PassoFirma passo in processoDiFirma.passi)
                {
                    //Per gli allegato il passo Cambio stato non viene considerato
                    if (!(isAllegato && passo.Evento.CodiceAzione.Equals(Azione.DOC_CAMBIO_STATO.ToString())))
                    {
                        if (passo.IsFacoltativo && passo.IncludiInIstanzaPasso == IncludiInIstanzaPasso.NON_SPECIFICATO)
                        {
                            resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_FACOLTATIVO_INCLUDI_NON_SPECIFICATO;
                            return false;
                        }
                    }
                }
            }

            #endregion

            #region CONTROLLI SUL DOCUMENTO
            if (!(!string.IsNullOrEmpty(file.fileSize) && Convert.ToUInt32(file.fileSize) > 0))
            {
                resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.FILE_NON_ACQUISITO;
                return false;
            }

            //Verifico se il documento è consolidato
            DocsPaVO.documento.DocumentConsolidationStateInfo consolidationState = BusinessLogic.Documenti.DocumentConsolidation.GetState(infoUtente, idDocumentoPrincipale);
            if (consolidationState.State != DocsPaVO.documento.DocumentConsolidationStateEnum.None)
            {
                resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_CONSOLIDATO;
                return false;
            }

            //Verifico se il doucmento è bloccato
            if (BusinessLogic.CheckInOut.CheckInOutServices.IsCheckedOut(file.docNumber, file.docNumber, infoUtente))
            {
                resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_BLOCCATO;
                return false;
            }
            //Verifico se il file è ammesso alla firma
            if (BusinessLogic.FormatiDocumento.Configurations.SupportedFileTypesEnabled)
            {
                DocsPaVO.FormatiDocumento.SupportedFileType[] fileTypes = BusinessLogic.FormatiDocumento.SupportedFormatsManager.GetFileTypes(Convert.ToInt32(infoUtente.idAmministrazione));
                string extensionFile = BusinessLogic.Documenti.FileManager.getEstensioneIntoSignedFile(file.fileName);
                int count = fileTypes.Count(e => e.FileExtension.ToLowerInvariant() == extensionFile.ToLowerInvariant() &&
                                                        e.FileTypeUsed && e.FileTypeSignature);
                bool retValue = count > 0;
                if (!retValue)
                {
                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.FILE_NON_AMMESSO_ALLA_FIRMA;
                    return false;
                }
            }
            #endregion

            #region CONTROLLI SUI PASSI DI FIRMA

            DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, file.docNumber);
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            foreach (PassoFirma passo in processoDiFirma.passi)
            {
                if (!passo.IsFacoltativo || passo.IncludiInIstanzaPasso == IncludiInIstanzaPasso.SI)
                {
                    Azione azioneEvento = (Azione)Enum.Parse(typeof(Azione), passo.Evento.CodiceAzione, true);
                    switch (azioneEvento)
                    {
                        case Azione.DOC_SIGNATURE_P:
                            //Non è possibile applicare firma PADES su CADES
                            if (file.firmato.Equals("1") && (file.tipoFirma.Equals(DocsPaVO.documento.TipoFirma.CADES) || file.tipoFirma.Equals(DocsPaVO.documento.TipoFirma.CADES_ELETTORNICA)))
                            {
                                resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_PADES_SU_FILE_CADES;
                                return false;
                            }
                            break;
                            //Non è possibile applicare firma PADES su file non PDF
                            bool isPdf = BusinessLogic.Documenti.FileManager.getEstensioneIntoSignedFile(file.fileName).ToUpper() == "PDF";
                            if (!isPdf)
                            {
                                resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_PADES_SU_FILE_NON_PDF;
                                return false;
                            }
                        case Azione.RECORD_PREDISPOSED:
                            if (!isAllegato)
                            {
                                //Verifico se il documento risulta gia protocollato
                                if (!doc.CheckProto(schedaDocumento))
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_PROTO_DOC_GIA_PROTOCOLLATO;
                                    return false;
                                }
                                if (passo.IsAutomatico)
                                {
                                    //PER IL PASSO AUTOMATICO VERIFICO CHE IL DOCUMENTO è PREDISPOSTO
                                    if (!isPredisposto(schedaDocumento))
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_PROTO_DOC_NON_PREDISPOSTO;
                                        return false;
                                    }
                                    //Il registro del predisposto deve essere lo stesso registro con cui andrò a protocollare
                                    if (!passo.IdAOO.Equals(schedaDocumento.registro.systemId))
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_AUTOMATICO_REGISTRO_ERRATO;
                                        return false;
                                    }
                                    //Verifico che il registro selezionato non sia chiuso
                                    if (schedaDocumento.registro.stato.Equals("C"))
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_PROTO_REG_CHIUSO;
                                        return false;
                                    }
                                }
                            }
                            break;
                        case Azione.DOCUMENTO_REPERTORIATO:
                            //Verifico se il documento è repertoriato
                            if (!isAllegato && passo.IsAutomatico)
                            {
                                //DOCUMENTO TIPIZZATO
                                string idTemplate = ProfilazioneDinamica.ProfilazioneDocumenti.getIdTemplate(idDocumentoPrincipale);
                                if (passo.IsAutomatico && string.IsNullOrEmpty(idTemplate))
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_DOC_NON_TIPIZZATO;
                                    return false;
                                }

                                //DOCUMENTO REPERTORIATO
                                if (ProfilazioneDinamica.ProfilazioneDocumenti.isDocRepertoriato(idDocumentoPrincipale, idTemplate))
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_DOC_GIA_REPERTORIATO;
                                    return false;
                                }

                                //NESSUN CONTATORE PER LA TIPOLOGIA
                                DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(idTemplate);
                                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto = (from ogg in template.ELENCO_OGGETTI.Cast<DocsPaVO.ProfilazioneDinamica.OggettoCustom>()
                                                                                       where ogg.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") || ogg.TIPO.DESCRIZIONE_TIPO.Equals("ContatoreSottocontatore")
                                                                                       select ogg).FirstOrDefault();
                                if (oggetto == null)
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_NESSUN_CONTATORE_TIPO_DOC;
                                    return false;
                                }

                                if (oggetto.TIPO_CONTATORE.Equals("R") && string.IsNullOrEmpty(passo.IdRF))
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_RF_MANCANTE;
                                    return false;
                                }

                                //DIRITTI IN SCRITTURA DEL RUOLO COINVOLTO SUL CAMPO CONTATORE
                                List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> dirittiCampiRuolo = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getDirittiCampiTipologiaDoc(passo.ruoloCoinvolto.idGruppo, idTemplate).Cast<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>().ToList();
                                bool ruoloInsRepertorio = false;
                                foreach (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
                                {
                                    if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggetto.SYSTEM_ID.ToString())
                                    {
                                        if (assDocFascRuoli.INS_MOD_OGG_CUSTOM == "1")
                                            ruoloInsRepertorio = true;
                                        break;
                                    }
                                }
                                if (!ruoloInsRepertorio)
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_REP_NO_DIRITTI_SCRITTURA_CONTATORE;
                                    return false;
                                }
                            }
                            break;
                        case Azione.DOCUMENTOSPEDISCI:
                            if (!isAllegato)
                            {
                                //Se il passo è automatico, verifico che il documento è protocollato; nel caso in cui non lo sia verifico se la spedizione
                                //è preceduta da un passo di protocollazione altrimenti blocco l'avvio del processo
                                if (passo.IsAutomatico && (schedaDocumento.protocollo == null || string.IsNullOrEmpty(schedaDocumento.protocollo.segnatura)))
                                {
                                    if ((from p1 in processoDiFirma.passi
                                         where p1.numeroSequenza < passo.numeroSequenza && p1.Evento.CodiceAzione.Equals(Azione.RECORD_PREDISPOSED.ToString())
                                         select p1).FirstOrDefault() == null)
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_SPEDIZIONE_DOC_NON_PROTOCOLLATO;
                                        return false;
                                    }
                                    // Il registro del predisposto deve essere lo stesso registro con cui andrò a spedire
                                    if (!passo.IdAOO.Equals(schedaDocumento.registro.systemId))
                                    {
                                        resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_AUTOMATICO_REGISTRO_ERRATO;
                                        return false;
                                    }
                                }
                                string tipoProto = doc.getTipoProto(schedaDocumento.docNumber);
                                if (!string.IsNullOrEmpty(tipoProto) && tipoProto.Equals("A"))
                                {
                                    resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_SPEDIZIONE_PROTO_ARRIVO;
                                    return false;
                                }
                            }
                            break;
                        case Azione.DOC_CAMBIO_STATO:
                            if (!isAllegato)
                            {

                            }

                            break;
                    }
                }
            }

            #endregion
        }
        catch (Exception e)
        {
            retVal = false;
            resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.KO;

        }
        return retVal;
    }

    /// <summary>
    /// Esegue la trasmissione a segueto dell'inserimento il libro firma. Il mittente della trasmissione è
    /// 1 - per istanza di passo n = 1 il proponente(utente che ha avviato il processo di firma)
    /// 2 - per istanza di passo n > 1  è il titolare del passo n -1.
    /// Il destinatario della trasmissione è il titolare:
    /// 1 - se non è specificato l'i utente titolare, il file è inserito nel libro firma di tutti gli utenti del ruolo titolare e quindi la trasmissione è al ruolo.
    /// 2 - se è specificato l'id utente titolare, il file è inserito nel libro firma del solo utente titolare e quindi sarà una trasmissione utente
    /// </summary>
    private static DocsPaVO.trasmissione.Trasmissione ExecuteTransmission(IstanzaProcessoDiFirma istanzaProcesso, IstanzaPassoDiFirma istanzaPasso, DocsPaVO.documento.InfoDocumento infoDoc, DocsPaVO.utente.InfoUtente infoUtenteMit)
    {
        DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();

        DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

        trasm.ruolo = u.GetRuoloByIdGruppo(infoUtenteMit.idGruppo);//istanzaProcesso.RuoloProponente;
        trasm.utente = u.getUtenteById(infoUtenteMit.idPeople);//istanzaProcesso.UtenteProponente;
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

        trasm.infoDocumento = infoDoc;

        string notePasso = string.IsNullOrEmpty(istanzaProcesso.NoteDiAvvio) ? istanzaPasso.Note : (string.IsNullOrEmpty(istanzaPasso.Note)) ? istanzaProcesso.NoteDiAvvio : (istanzaProcesso.NoteDiAvvio + " - " + istanzaPasso.Note);

        string tipoPasso = string.Empty;
        if (istanzaPasso.Evento.TipoEvento.Equals("E"))
        {
            trasm.noteGenerali = "Azione richiesta " + istanzaPasso.Evento.Descrizione + ". " + notePasso;
            tipoPasso = istanzaPasso.Evento.Gruppo;
        }
        else
        {
            trasm.noteGenerali = notePasso;
            tipoPasso = istanzaPasso.Evento.CodiceAzione;
        }
        if (istanzaPasso.IsAutomatico)
            tipoPasso += "_AUTOMATICO";

        //INSERISCO LA RAGIONE DI TRASMISSIONE DI SISTEMA PER LIBRO FIRMA
        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        DocsPaVO.utente.Ruolo ruolo = utenti.GetRuoloByIdGruppo(istanzaPasso.RuoloCoinvolto.idGruppo);
        DocsPaVO.trasmissione.RagioneTrasmissione ragione = Trasmissioni.RagioniManager.GetRagioneByTipoOperazione(tipoPasso, ruolo.idAmministrazione);

        //CREO LA TRASMISSIONE SINGOLA
        DocsPaVO.trasmissione.TrasmissioneSingola trasmSing = new DocsPaVO.trasmissione.TrasmissioneSingola();
        trasmSing.ragione = ragione;
        trasmSing.tipoTrasm = "S";

        trasmSing.corrispondenteInterno = ruolo;
        trasmSing.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;

        System.Collections.ArrayList listaUtenti = new System.Collections.ArrayList();
        DocsPaVO.addressbook.QueryCorrispondente qc = new DocsPaVO.addressbook.QueryCorrispondente();
        qc.codiceRubrica = ruolo.codiceRubrica;
        System.Collections.ArrayList registri = ruolo.registri;
        qc.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
        //qc.idRegistri = registri;
        qc.idAmministrazione = ruolo.idAmministrazione;
        qc.getChildren = true;
        qc.fineValidita = true;
        listaUtenti = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qc);
        System.Collections.ArrayList trasmissioniUt = new System.Collections.ArrayList();

        //Se l'id utente titolare non è specificato, il documento è stato inserito  nel libro firma di tutti gli utenti del ruolo,
        //e quindi la trasmissione andrà notificata a tutti gli utenti, altrimenti al solo utente titolare
        if (string.IsNullOrEmpty(istanzaPasso.UtenteCoinvolto.idPeople))
        {
            for (int k = 0; k < listaUtenti.Count; k++)
            {
                DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trUt.utente = (DocsPaVO.utente.Utente)listaUtenti[k];
                trasmissioniUt.Add(trUt);
            }
        }
        else
        {
            for (int k = 0; k < listaUtenti.Count; k++)
            {
                DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trUt.utente = (DocsPaVO.utente.Utente)listaUtenti[k];
                trUt.daNotificare = (listaUtenti[k] as DocsPaVO.utente.Utente).idPeople.Equals(istanzaPasso.UtenteCoinvolto.idPeople);
                trasmissioniUt.Add(trUt);
            }
        }

        // Modifica per invio in mail dell'url del frontend
        string urlfrontend = "";
        if (DocsPaVO.Settings.AppSettings.Instance.URL_PATH_IS != null)
            urlfrontend = DocsPaVO.Settings.AppSettings.Instance.URL_PATH_IS.ToString();

        trasmSing.trasmissioneUtente = trasmissioniUt;
        trasm.trasmissioniSingole = new System.Collections.ArrayList() { trasmSing };
        return BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, trasm);
    }

    public static void EseguiPassoAutomatico(IstanzaPassoDiFirma passo, IstanzaProcessoDiFirma istanzaProcesso, IBaseRubricaComuneService rubricaComuneService)
    {
        DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
        try
        {
            Azione codiceEvento = (Azione)Enum.Parse(typeof(Azione), passo.CodiceTipoEvento, true);
            DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(passo.RuoloCoinvolto.idGruppo);
            DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.GetUtenteAutomatico(ruolo.idAmministrazione);
            DocsPaVO.utente.InfoUtente infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(utente, ruolo);
            string docnumber = istanzaProcesso.docNumber;
            string error = string.Empty;
            bool result = false;
            switch (codiceEvento)
            {
                case Azione.RECORD_PREDISPOSED:
                    string segnatura = string.Empty;
                    result = Protocolla(docnumber, passo, infoUtente, ruolo, out segnatura, out error);
                    if (result)
                    {
                        //Se il processo era in errore e si sta procedendo con un ritentativo di esecuzione, aggiorno lo stato
                        if (istanzaProcesso.statoProcesso == TipoStatoProcesso.IN_ERROR)
                        {
                            libro.SetErroreIstanzaPassoFirma(string.Empty, passo.idIstanzaPasso, passo.idIstanzaProcesso, TipoStatoProcesso.IN_EXEC);
                        }
                        string varDescOggetto = string.Format("{0}{1} / {2}{3}", "N.ro Doc.: ", docnumber, "Segnatura: ", segnatura);
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "RECORDPREDISPOSED", docnumber,
                                varDescOggetto, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
                    }
                    break;
                case Azione.DOCUMENTO_REPERTORIATO:
                    string numRep = string.Empty;
                    result = Repertoria(docnumber, passo, infoUtente, ruolo, out numRep, out error);
                    if (result)
                    {
                        //Se il processo era in errore e si sta procedendo con un ritentativo di esecuzione, aggiorno lo stato
                        if (istanzaProcesso.statoProcesso == TipoStatoProcesso.IN_ERROR)
                        {
                            libro.SetErroreIstanzaPassoFirma(string.Empty, passo.idIstanzaPasso, passo.idIstanzaProcesso, TipoStatoProcesso.IN_EXEC);
                        }
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTO_REPERTORIATO", docnumber, "Repertoriato documento: " + numRep, DocsPaVO.Logger.CodAzione.Esito.OK);
                    }
                    break;
                case Azione.DOCUMENTOSPEDISCI:
                    result = Spedisci(docnumber, passo, infoUtente, ruolo, out error);
                    if (result)
                    {
                        //Se il processo era in errore e si sta procedendo con un ritentativo di esecuzione, aggiorno lo stato
                        if (istanzaProcesso.statoProcesso == TipoStatoProcesso.IN_ERROR)
                            libro.SetErroreIstanzaPassoFirma(string.Empty, passo.idIstanzaPasso, passo.idIstanzaProcesso, TipoStatoProcesso.IN_EXEC);

                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOSPEDISCI", docnumber, string.Format("{0}{1}", "Spedizione del doc: ", docnumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                    }
                    break;
                case Azione.DOC_CAMBIO_STATO:
                    result = CambioStato(docnumber, passo, infoUtente, ruolo, rubricaComuneService, out error );
                    if (result)
                    {
                        //Se il processo era in errore e si sta procedendo con un ritentativo di esecuzione, aggiorno lo stato
                        if (istanzaProcesso.statoProcesso == TipoStatoProcesso.IN_ERROR)
                            libro.SetErroreIstanzaPassoFirma(string.Empty, passo.idIstanzaPasso, passo.idIstanzaProcesso, TipoStatoProcesso.IN_EXEC);
                    }
                    break;
            }
            if (!result)
            {
                if (string.IsNullOrEmpty(error))
                    error = "Errore durante l'esecuzione del passo automatico";
                libro.SetErroreIstanzaPassoFirma(error, passo.idIstanzaPasso, passo.idIstanzaProcesso, TipoStatoProcesso.IN_ERROR);
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: EseguiPassoAutomatico ", e);
            libro.SetErroreIstanzaPassoFirma(e.Message, passo.idIstanzaPasso, passo.idIstanzaProcesso, TipoStatoProcesso.IN_ERROR);
        }
    }

    private static bool isPredisposto(DocsPaVO.documento.SchedaDocumento doc)
    {
        bool result = false;

        if (doc.tipoProto.Equals("A") || doc.tipoProto.Equals("P") || doc.tipoProto.Equals("I"))
        {
            if (!(doc.protocollo != null && !(string.IsNullOrEmpty(doc.protocollo.segnatura))))
                result = true;
        }

        return result;
    }

    private static bool Protocolla(string docnumber, IstanzaPassoDiFirma passo, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, out string segnatura, out string error)
    {
        bool result = false;
        segnatura = string.Empty;
        error = string.Empty;
        DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
        if (schedaDoc.protocollo != null && !string.IsNullOrEmpty(schedaDoc.protocollo.segnatura))
        {
            error = "Errore protocollazione: il documento è già protocollato.";
            return false;
        }
        if (!isPredisposto(schedaDoc))
        {
            error = "Errore protocollazione: il documento non è predisposto per la protocollazione.";
            return false;
        }
        if (schedaDoc.registro.stato.Equals("C"))
        {
            error = "Errore protocollazione: il registro è chiuso";
            return false;
        }
        //DocsPaVO.utente.Registro registro = BusinessLogic.Utenti.RegistriManager.getRegistro(schedaDoc.re)
        string idRegistro = passo.IdRF;
        if (string.IsNullOrEmpty(idRegistro))
            idRegistro = passo.IdAOO;
        schedaDoc.id_rf_prot = idRegistro;
        schedaDoc.id_rf_invio_ricevuta = idRegistro;
        schedaDoc.cod_rf_prot = BusinessLogic.Utenti.RegistriManager.getRegistro(idRegistro).codRegistro;
        DocsPaVO.documento.ResultProtocollazione resultProtocollazione = DocsPaVO.documento.ResultProtocollazione.OK;
        try
        {
            BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out resultProtocollazione, true);
        }
        catch (Exception e)
        {
            error = "Errore protocollazione.";
            return false;
        }
        if (!resultProtocollazione.Equals(DocsPaVO.documento.ResultProtocollazione.OK))
        {
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", schedaDoc.docNumber, "Errore nell'esecuzione del passo automatico di protocollazione", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
            error = "Errore protocollazione: " + resultProtocollazione.ToString();
            return false;
        }
        else
        {
            segnatura = schedaDoc.protocollo.segnatura;
            result = true;

            //Se previsto applico il sigillo
            if (schedaDoc.tipoProto.Equals("P") && passo.ApplicaSegnaturaPermanente.Equals("1"))
            {
                if (!(schedaDoc.documenti[0] as FileRequest).conSegnaturaPermanente)
                {
                    ResultSigilloElettronico sigilloResult = ResultSigilloElettronico.OK;
                    DocsPaVO.documento.labelPdf labelPdf = new DocsPaVO.documento.labelPdf();
                    labelPdf.position = passo.PosizioneSegnaturaPermanente;
                    labelPdf.default_position = passo.PosizioneSegnaturaPermanente;
                    DocsPaVO.documento.FileDocumento fileDocumento = BusinessLogic.Documenti.FileManager.getVoidFileConSegnatura(schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest, schedaDoc, infoUtente, "", labelPdf);
                    DocsPaVO.documento.SchedaDocumento schedaResult = BusinessLogic.Documenti.FileManager.Stamp(fileDocumento.LabelPdf, infoUtente, schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest, schedaDoc, out sigilloResult);
                }
            }
        }

        return result;
    }

    private static bool Repertoria(string docnumber, IstanzaPassoDiFirma passo, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, out string numRep, out string error)
    {
        bool result = false;
        numRep = string.Empty;
        error = string.Empty;

        DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
        DocsPaVO.ProfilazioneDinamica.Templates template = model.getTemplateDettagli(docnumber);
        DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
        if (template == null || template.SYSTEM_ID == 0)
        {
            error = "Errore repertoriazione: il documento non risulta tipizzato.";
            return false;
        }
        DocsPaVO.ProfilazioneDinamica.OggettoCustom oggetto = (from ogg in template.ELENCO_OGGETTI.Cast<DocsPaVO.ProfilazioneDinamica.OggettoCustom>()
                                                               where ogg.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") || ogg.TIPO.DESCRIZIONE_TIPO.Equals("ContatoreSottocontatore")
                                                               select ogg).FirstOrDefault();
        if (oggetto == null)
        {
            error = "Errore repertoriazione: alla tipologia non risulta associato un contatore di repertorio.";
            return false;
        }
        /*List<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> dirittiCampiRuolo = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getDirittiCampiTipologiaDoc(ruolo.idGruppo, idTemplate).Cast<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli>().ToList();
        bool ruoloInsRepertorio = false;
        foreach(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
        {
            if(assDocFascRuoli.ID_OGGETTO_CUSTOM == oggetto.SYSTEM_ID.ToString())
            {
                if (assDocFascRuoli.INS_MOD_OGG_CUSTOM == "2")
                    ruoloInsRepertorio = true;
                break;
            }
        }
        if (!ruoloInsRepertorio)
        {
            throw new Exception("Errore repertoriazione: il ruolo non è abilitato a far scattare il contatore di repertorio.");
        }
        */

        string codiceAOO_RF = string.Empty;
        if (oggetto.TIPO_CONTATORE.Equals("A"))
        {
            codiceAOO_RF = passo.IdAOO;
        }
        if (oggetto.TIPO_CONTATORE.Equals("R"))
        {
            codiceAOO_RF = passo.IdRF;
        }
        if (oggetto.TIPO_CONTATORE.Equals("T"))
        {
            if (schedaDoc.registro != null && !string.IsNullOrEmpty(schedaDoc.registro.systemId))
            {
                codiceAOO_RF = schedaDoc.registro.systemId;
            }
            else
            {
                //se il grigio non ha estraggo il registro del ruolo creatore...non dovrebbe mai succedere perchè i grigi da giugno 2023 devono avere i registri, tutti
                //anche il pregresso, solo i conservati non hanno registro
                ArrayList registri = BusinessLogic.Utenti.RegistriManager.getRegistriRuolo(schedaDoc.creatoreDocumento.idCorrGlob_Ruolo);
                if (registri != null && registri.Count > 0)
                    codiceAOO_RF = (registri[0] as Registro).systemId;
            }
        }
        oggetto.ID_AOO_RF = codiceAOO_RF;
        oggetto.CONTATORE_DA_FAR_SCATTARE = true;
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            model.salvaInserimentoUtenteProfDim(template, docnumber);
            transactionContext.Complete();
        }
        string codiceAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione).Codice;
        string dataAnnullamento = String.Empty;
        numRep = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(docnumber, codiceAmm, false, out dataAnnullamento);
        if (string.IsNullOrEmpty(numRep))
        {
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", docnumber, "Errore nell'esecuzione del passo automatico di repertoriazione", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
            error = "Errore repertoriazione.";
            return false;
        }
        else
        {
            result = true;

            //Se previsto, applico il sigillo
            if (passo.ApplicaSegnaturaPermanente.Equals("1"))
            {
                ResultSigilloElettronico sigilloResult = ResultSigilloElettronico.OK;
                schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
                if (!(schedaDoc.documenti[0] as FileRequest).conSegnaturaPermanente)
                {
                    DocsPaVO.documento.labelPdf labelPdf = new DocsPaVO.documento.labelPdf();
                    labelPdf.position = passo.PosizioneSegnaturaPermanente;
                    labelPdf.default_position = passo.PosizioneSegnaturaPermanente;
                    DocsPaVO.documento.FileDocumento fileDocumento = BusinessLogic.Documenti.FileManager.getVoidFileConSegnatura(schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest, schedaDoc, infoUtente, "", labelPdf);
                    DocsPaVO.documento.SchedaDocumento schedaResult = BusinessLogic.Documenti.FileManager.Stamp(fileDocumento.LabelPdf, infoUtente, schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest, schedaDoc, out sigilloResult);
                }
            }
        }
        return result;
    }

    private static bool Spedisci(string docnumber, IstanzaPassoDiFirma passo, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, out string error)
    {
        bool result = false;
        error = string.Empty;
        DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
        if (schedaDoc.protocollo == null || string.IsNullOrEmpty(schedaDoc.protocollo.segnatura))
        {
            error = "Errore spedizione: il documento non è protocollato.";
            return false;
        }
        if (schedaDoc.protocollo != null && schedaDoc.tipoProto.Equals("A"))
        {
            error = "Errore spedizione: non è possibile spedire un protocollo in arrivo.";
            return false;
        }
        //Se uno degli allegati del documento è in Libro Firma blocco la spedizione
        if (CheckAllegatiInLibroFirma(docnumber))
        {
            error = "Errore spedizione: non è possibile spedire il protocollo poichè è attivo un processo di firma per i suoi allegati.";
            return false;
        }

        if ((schedaDoc.protocollo as DocsPaVO.documento.ProtocolloUscita).destinatariConoscenza == null)
            (schedaDoc.protocollo as DocsPaVO.documento.ProtocolloUscita).destinatariConoscenza = new ArrayList();
        DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizione = BusinessLogic.Spedizione.SpedizioneManager.GetSpedizioneDocumento(infoUtente, schedaDoc);

        //Estraggo la casella da cui effettuare la spedizione
        DocsPaDB.Query_DocsPAWS.LibroFirma libro = new DocsPaDB.Query_DocsPAWS.LibroFirma();
        DocsPaVO.amministrazione.CasellaRegistro casella = libro.GetCasellaRegistroByIdMail(passo.IdMailRegistro);
        infoSpedizione.mailAddress = casella.EmailRegistro;
        infoSpedizione.IdRegistroRfMittente = casella.IdRegistro;
        try
        {
            BusinessLogic.Spedizione.SpedizioneManager.SpedisciDocumento(infoUtente, schedaDoc, infoSpedizione);
        }
        catch (Exception e)
        {
            error = "Errore spedizione: " + e.Message;
            return false;
        }

        string destinatariNonRaggiunti = string.Empty;
        DocsPaVO.utente.Corrispondente corr;
        if (infoSpedizione.DestinatariEsterni != null && infoSpedizione.DestinatariEsterni.Count > 0)
        {
            foreach (DocsPaVO.Spedizione.DestinatarioEsterno dest in infoSpedizione.DestinatariEsterni)
            {
                if (dest.IncludiInSpedizione && !dest.StatoSpedizione.Equals(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito))
                {
                    corr = dest.DatiDestinatari[0] as DocsPaVO.utente.Corrispondente;
                    destinatariNonRaggiunti += "#DESTINATARIO#" + corr.codiceRubrica + " (" + corr.descrizione + ")#DESCRIZIONE#: " + dest.StatoSpedizione.Descrizione + "";
                }
            }
        }
        if (infoSpedizione.DestinatariInterni != null && infoSpedizione.DestinatariInterni.Count > 0)
        {
            foreach (DocsPaVO.Spedizione.DestinatarioInterno dest in infoSpedizione.DestinatariInterni)
            {
                if (dest.IncludiInSpedizione && !dest.StatoSpedizione.Equals(DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito))
                {
                    destinatariNonRaggiunti += "#DESTINATARIO#" + dest.DatiDestinatario.codiceRubrica + " (" + dest.DatiDestinatario.descrizione + ")#DESCRIZIONE#: " + dest.StatoSpedizione.Descrizione + "";
                }
            }
        }
        if (!string.IsNullOrEmpty(destinatariNonRaggiunti))
        {
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", schedaDoc.docNumber, "Errore nell'esecuzione del passo automatico di spedizione", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
            error = "Errore di spedizione per i seguenti destinatari: " + destinatariNonRaggiunti;
            return false;
        }
        //SE E' PRESENTE UN DESTINATARIO NON INTEROPERANTE ED è STATA RICHIESTA LA NOTITICA, NOTIFICO
        if (libro.NotificaPresenzaDestinatariInterop(passo.idIstanzaProcesso))
        {
            bool presentiDestNonInteroperanti = (from c in infoSpedizione.DestinatariEsterni
                                                 where !c.Interoperante
                                                 select c).FirstOrDefault() != null;
            if (presentiDestNonInteroperanti)
            {
                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_DESTINATARI_NON_INTEROP", schedaDoc.docNumber, "Presenza di destinatati non interoperanti nella spedizione del documento", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
            }
        }

        if (infoSpedizione.Spedito)
            result = true;
        else
        {
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, "PROCESSO_FIRMA_ERRORE_PASSO_AUTOMATICO", schedaDoc.docNumber, "Errore nell'esecuzione del passo automatico di spedizione", DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, "1");
            error = "Errore spedizione.";
            return false;
        }
        return result;
    }

    private static bool CambioStato(string docnumber, IstanzaPassoDiFirma passo, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo,IBaseRubricaComuneService rubricaComuneService, out string error)
    {
        bool result = false;
        error = string.Empty;
        DocsPaDB.Query_DocsPAWS.DiagrammiStato diag = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();

        if (string.IsNullOrEmpty(passo.IdStatoDiagramma))
        {
            error = "Stato non specificato nel passo";
            return false;
        }

        DocsPaVO.DiagrammaStato.Stato stato = diag.GetStatoById(passo.IdStatoDiagramma, infoUtente);
        if (stato == null || string.IsNullOrEmpty(stato.SYSTEM_ID.ToString()))
        {
            error = "Stato specificato nel passo non trovato";
            return false;
        }

        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diag.getDiagrammaById(stato.ID_DIAGRAMMA.ToString());
        if (diagramma == null || string.IsNullOrEmpty(diagramma.SYSTEM_ID.ToString()))
        {
            error = "Diagramma di stato non trovato";
            return false;
        }

        if (!SalvaStatoAutomaticoLF(stato, diagramma, docnumber, infoUtente, rubricaComuneService))
        {
            error = "Errore durante il salvataggio del stato";
            return false;
        }
        else
        {
            result = true;
        }

        return result;
    }

    public static List<FirmaElettronica> GetFirmaElettronicaDaFileRequest(DocsPaVO.documento.FileRequest fileRq)
    {
        DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
        List<FirmaElettronica> firmaFound = libroFirma.GetFirmaElettronicaDaFileRequest(fileRq);

        return firmaFound;
    }

    public static FirmaElettronica InserisciFirmaElettronica(FirmaElettronica firma)
    {
        DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
        return libroFirma.InsertElectronicSignFromDigitalSign(firma);
    }

    public static bool CheckAllegatiInLibroFirma(string idDocumentoPrincipale)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            return libroFirma.CheckAllegatiInLibroFirma(idDocumentoPrincipale);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: CheckAllegatiInLibroFirma ", e);
            return false;
        }
    }

    public static bool SalvaStatoAutomaticoLF(DocsPaVO.DiagrammaStato.Stato statoDoc, DocsPaVO.DiagrammaStato.DiagrammaStato diagramma, string docnumber, DocsPaVO.utente.InfoUtente infoUtente, IBaseRubricaComuneService rubricaComuneService)
    {
        bool retVal = true;
        try
        {
            if (statoDoc != null)
            {
                SalvaStatoDiagrammaDoc(statoDoc, diagramma, docnumber, infoUtente, rubricaComuneService);
                //Controllo che lo stato sia uno stato di conversione pdf lato server
                //In caso affermativo faccio partire la conversione
                DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docnumber);
                if (BusinessLogic.Documenti.DocManager.isEnabledConversionePdfServer())
                {
                    if (statoDoc.CONVERSIONE_PDF)
                    {
                        ConvertiInPdf(schedaDocumento, infoUtente);
                    }
                }
                string idTemplate = schedaDocumento.template.SYSTEM_ID.ToString();

                DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
                ArrayList modelli = new ArrayList(BusinessLogic.DiagrammiStato.DiagrammiStato.isStatoTrasmAuto(infoUtente.idAmministrazione, statoDoc.SYSTEM_ID.ToString(), idTemplate));
                for (int i = 0; i < modelli.Count; i++)
                {
                    DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod = (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione)modelli[i];
                    if (mod.SINGLE == "1")
                    {
                        infoDoc = getInfoDocumento(schedaDocumento);
                        if (infoDoc != null)
                            effettuaTrasmissioneDocDaModello(mod, statoDoc.SYSTEM_ID.ToString(), infoDoc, infoUtente, schedaDocumento, rubricaComuneService);
                    }
                    else
                    {
                        for (int k = 0; k < mod.MITTENTE.Count; k++)
                        {
                            if ((mod.MITTENTE[k] as DocsPaVO.Modelli_Trasmissioni.MittDest).ID_CORR_GLOBALI.ToString() == infoUtente.idCorrGlobali)
                            {
                                infoDoc = getInfoDocumento(schedaDocumento);
                                effettuaTrasmissioneDocDaModello(mod, statoDoc.SYSTEM_ID.ToString(), infoDoc, infoUtente, schedaDocumento, rubricaComuneService);
                                break;
                            }
                        }
                    }
                }
                //SE è STATO FINALE METTO IL DOCUMENTO IN SOLA LETTURA
                if (statoDoc.STATO_FINALE)
                {
                    BusinessLogic.Documenti.DocManager.cambiaDirittiDocumenti((Convert.ToInt32(DocsPaVO.Security.SecurityItemInfo.SecurityAccessRightsEnum.ACCESS_RIGHT_45)), schedaDocumento.docNumber);
                }
            }

        }
        catch (Exception ex)
        {
            logger.Error("Errore in SalvaStatoAutomaticoLF " + ex.Message);
            retVal = false;
        }
        return retVal;
    }

    /// <summary>
    /// Converte in PDF il file
    /// </summary>
    /// <param name="schedaDocumento"></param>
    /// <param name="infoUtente"></param>
    private static void ConvertiInPdf(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente)
    {
#if false   // usa aspose
        DocsPaVO.documento.FileDocumento fileDocumento = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0], infoUtente);
        if (fileDocumento != null && fileDocumento.content != null && fileDocumento.name != null && fileDocumento.name != "")
        {
            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ASPOSE_PDF_CONVERSION")) &&
               DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ASPOSE_PDF_CONVERSION").ToString().Equals("1"))
            {
                BusinessLogic.Modelli.AsposeModelProcessor.DocModelProcessor processor = new BusinessLogic.Modelli.AsposeModelProcessor.DocModelProcessor();
                processor.ConvertToPdfAsync(fileDocumento.content, (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0], infoUtente);
            }
            else
            {
                DocsPaVO.documento.ObjServerPdfConversion objServerPdfConversion = new DocsPaVO.documento.ObjServerPdfConversion();
                objServerPdfConversion.content = fileDocumento.content;
                objServerPdfConversion.fileName = (schedaDocumento.documenti[0] as DocsPaVO.documento.FileRequest).fileName;
                objServerPdfConversion.idProfile = schedaDocumento.systemId;
                objServerPdfConversion.docNumber = schedaDocumento.docNumber;

                if (!string.IsNullOrEmpty(objServerPdfConversion.idProfile) && !string.IsNullOrEmpty(objServerPdfConversion.docNumber))
                    BusinessLogic.LiveCycle.LiveCyclePdfConverter.EnqueueServerPdfConversion(infoUtente, objServerPdfConversion, null);
            }
        }

#endif
    }

    private static DocsPaVO.documento.InfoDocumento getInfoDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
    {
        try
        {
            DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();

            infoDoc.idProfile = schedaDocumento.systemId;
            infoDoc.oggetto = schedaDocumento.oggetto.descrizione;
            infoDoc.docNumber = schedaDocumento.docNumber;
            infoDoc.tipoProto = schedaDocumento.tipoProto;
            infoDoc.evidenza = schedaDocumento.evidenza;

            if (schedaDocumento.registro != null)
            {
                infoDoc.codRegistro = schedaDocumento.registro.codRegistro;
                infoDoc.idRegistro = schedaDocumento.registro.systemId;
            }

            if (schedaDocumento.protocollo != null)
            {
                infoDoc.numProt = schedaDocumento.protocollo.numero;
                infoDoc.daProtocollare = schedaDocumento.protocollo.daProtocollare;
                infoDoc.dataApertura = schedaDocumento.protocollo.dataProtocollazione;
                infoDoc.segnatura = schedaDocumento.protocollo.segnatura;

                if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
                {
                    string[] mittDest = new string[1];
                    DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;

                    if (pe != null && pe.mittente != null && infoDoc.mittDest != null && infoDoc.mittDest.Count > 0)
                    {
                        mittDest[0] = pe.mittente.descrizione;
                    }
                    infoDoc.mittDest.AddRange(mittDest);
                }
                else if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloUscita)))
                {
                    DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;
                    if (pu.destinatari != null)
                    {
                        string[] mittDest = new string[pu.destinatari.Count];
                        for (int i = 0; i < pu.destinatari.Count; i++)
                            mittDest[i] = ((DocsPaVO.utente.Corrispondente)pu.destinatari[i]).descrizione;
                        infoDoc.mittDest.AddRange(mittDest);
                    }
                }
            }
            else
            {
                infoDoc.dataApertura = schedaDocumento.dataCreazione;
            }

            infoDoc.privato = schedaDocumento.privato;
            infoDoc.personale = schedaDocumento.personale;

            return infoDoc;
        }
        catch (System.Exception ex)
        {
            return null;
        }
    }

    public static void effettuaTrasmissioneDocDaModello(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello, string idStato,
        DocsPaVO.documento.InfoDocumento infoDocumento, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc, IBaseRubricaComuneService rubricaComuneService)
    {

        try
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

            //Parametri della trasmissione
            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
            trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
            trasmissione.infoDocumento = infoDocumento;

            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

            trasmissione.ruolo = u.GetRuoloByIdGruppo(infoUtente.idGruppo);//istanzaProcesso.RuoloProponente;
            trasmissione.utente = u.getUtenteById(infoUtente.idPeople);//istanzaProcesso.UtenteProponente;
            if (modello != null)
                trasmissione.NO_NOTIFY = modello.NO_NOTIFY;

            //Parametri delle trasmissioni singole
            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
            {

                DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];
                    DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                    if (mittDest.CHA_TIPO_MITT_DEST == "D")
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente, rubricaComuneService);
                    }
                    else
                    {
                        corr = getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, schedaDoc, infoUtente, trasmissione.ruolo);
                    }
                    if (corr != null)
                    {
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());
                        trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, infoUtente, trasmissione.ruolo);
                    }
                }
            }
            trasmissione = impostaNotificheUtentiDaModello(trasmissione, modello);

            //
            // Aggiunto codice mancante per segnalazione Zanotti
            if (trasmissione != null && modello.CEDE_DIRITTI.Equals("1"))
            {

                if (trasmissione.cessione == null)
                {
                    DocsPaVO.documento.CessioneDocumento cessione = new DocsPaVO.documento.CessioneDocumento();
                    cessione.docCeduto = true;
                    cessione.idPeople = infoUtente.idPeople;
                    cessione.idRuolo = infoUtente.idGruppo;
                    cessione.userId = infoUtente.userId;
                    cessione.idPeopleNewPropr = modello.ID_PEOPLE_NEW_OWNER;
                    cessione.idRuoloNewPropr = modello.ID_GROUP_NEW_OWNER;
                    trasmissione.cessione = cessione;
                }
            }
            //
            // End Aggiunta codice per segnalazione Zanotti


            trasmissione = saveExecuteTrasm(trasmissione, infoUtente);
            if (idStato != null && idStato != "")
                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaStoricoTrasmDiagrammiFasc(trasmissione.systemId, infoDocumento.docNumber, idStato);

        }
        catch (System.Exception ex)
        {

        }
    }

    public static DocsPaVO.utente.Corrispondente getCorrispondenti(string tipo_destinatario, DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
    {
        try
        {
            DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
            //se la il modello di trasmissione ha come destinatario l'utente proprietario del documento
            if (schedaDocumento != null)
            {
                if (tipo_destinatario == "UT_P")
                {
                    string utenteProprietario = string.Empty;
                    if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                    {
                        //caso predispsosto con ruolo creatore diverso da protocollatore:
                        if (schedaDocumento.creatoreDocumento != null)
                        {
                            utenteProprietario = schedaDocumento.creatoreDocumento.idPeople;
                        }
                        else utenteProprietario = schedaDocumento.protocollatore.utente_idPeople;

                    }
                    else
                    {
                        utenteProprietario = schedaDocumento.creatoreDocumento.idPeople;
                    }
                    corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(utenteProprietario, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                }
                //ruolo proprietario del documento
                if (tipo_destinatario == "R_P")
                {
                    string idCorrGlobaliRuolo = string.Empty;
                    if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                    {
                        //caso predispsosto con ruolo creatore diverso da protocollatore:
                        if (schedaDocumento.creatoreDocumento != null)
                        {
                            idCorrGlobaliRuolo = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                        }
                        else
                            idCorrGlobaliRuolo = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                    }
                    else
                    {
                        idCorrGlobaliRuolo = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                    }
                    // corr = UserManager.getCorrispondenteBySystemID(page, idCorrGlobaliRuolo);
                    corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuolo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                }
                //trasmissione a UO del proprietario
                if (tipo_destinatario == "UO_P")
                {
                    string idCorrGlobaliUo = string.Empty;
                    if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                    {
                        //caso predispsosto con ruolo creatore diverso da protocollatore:
                        if (schedaDocumento.creatoreDocumento != null)
                        {
                            idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                        }
                        else
                            idCorrGlobaliUo = schedaDocumento.protocollatore.uo_idCorrGlobali;
                    }
                    else
                    {
                        idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                    }
                    corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);//.getCorrispondenteByIdPeople(idPeople, tipoIE, u);

                }//RUOLO Responsabile UO proprietario
                if (tipo_destinatario == "RSP_P")
                {
                    string idCorrGlobaliUo = string.Empty;
                    string idCorr = string.Empty;
                    if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                    {
                        //caso predispsosto con ruolo creatore diverso da protocollatore:
                        if (schedaDocumento.creatoreDocumento != null)
                        {
                            idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                            //idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                        }
                        else
                        {
                            idCorrGlobaliUo = schedaDocumento.protocollatore.uo_idCorrGlobali;
                            //idCorr = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                        }
                    }
                    else
                    {
                        idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                        //idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                    }
                    idCorr = ruolo.systemId;
                    string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    }
                    else
                    {
                        corr = null;
                    }
                }
                //Ruolo segretario UO PROPRIETARIO
                if (tipo_destinatario == "R_S")
                {
                    string idCorrGlobaliUo = string.Empty;
                    string idCorr = String.Empty;
                    if (schedaDocumento.protocollatore != null && schedaDocumento.protocollo != null && !string.IsNullOrEmpty(schedaDocumento.protocollo.numero))
                    {
                        //caso predispsosto con ruolo creatore diverso da protocollatore:
                        if (schedaDocumento.creatoreDocumento != null)
                        {
                            idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                            // idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                        }
                        else
                        {
                            idCorrGlobaliUo = schedaDocumento.protocollatore.uo_idCorrGlobali;
                            // idCorr = schedaDocumento.protocollatore.ruolo_idCorrGlobali;
                        }
                    }
                    else
                    {
                        idCorrGlobaliUo = schedaDocumento.creatoreDocumento.idCorrGlob_UO;
                        //idCorr = schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo;
                    }
                    idCorr = ruolo.systemId;
                    string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);

                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    }
                    else
                    {
                        corr = null;
                    }
                }
                //ruolo responsabile uo mittente
                if (tipo_destinatario == "RSP_M")
                {
                    string idCorrGlobaliUo = ruolo.uo.systemId;
                    string idCorr = ruolo.systemId;

                    string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "R", idCorr);

                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    }
                    else
                    {
                        corr = null;
                    }
                }

                //ruolo segretario uo mittente
                if (tipo_destinatario == "S_M")
                {
                    string idCorrGlobaliUo = ruolo.uo.systemId;
                    string idCorr = ruolo.systemId;

                    string idCorrGlobaliRuoloRespUo = BusinessLogic.Utenti.UserManager.getRuoloRespUofromUo(idCorrGlobaliUo, "S", idCorr);

                    if (idCorrGlobaliRuoloRespUo != "0" && idCorrGlobaliRuoloRespUo != "-1")
                    {
                        corr = BusinessLogic.Utenti.UserManager.getCorrispondenteCompletoBySystemId(idCorrGlobaliRuoloRespUo, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtente);
                    }
                    else
                    {
                        corr = null;
                    }
                }
            }
            return corr;
        }
        catch (System.Exception ex)
        {
            return null;
        }
    }

    public static DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
    {
        try
        {
            return addTrasmissioneSingola(trasmissione, corr, ragione, note, tipoTrasm, scadenza, false, infoUtente, ruolo);
        }
        catch (System.Exception ex)
        {
            return null;
        }
    }

    public static DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragione, string note, string tipoTrasm, int scadenza, bool nascondiVersioniPrecedenti, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
    {
        logger.Debug("INIZIO addTrasmissioneSingola");
        if (trasmissione.trasmissioniSingole != null)
        {
            // controllo se esiste la trasmissione singola associata a corrispondente selezionato
            for (int i = 0; i < trasmissione.trasmissioniSingole.Count; i++)
            {
                DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                {
                    if (ts.daEliminare)
                    {
                        ((DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                        return trasmissione;
                    }
                    else
                        return trasmissione;
                }
            }
        }

        //Quando la ragione di trasmissione ha mantieni lettura, anche la trasmissione deve mantenere la lettura
        if (ragione != null && !string.IsNullOrEmpty(ragione.mantieniLettura) && ragione.mantieniLettura.Equals("1"))
        {
            if (trasmissione != null)
            {
                trasmissione.mantieniLettura = true;
            }
        }

        // Mev Cessione Diritti - Mantieni Scrittura
        if (ragione != null && !string.IsNullOrEmpty(ragione.mantieniScrittura) && ragione.mantieniScrittura.Equals("1"))
        {
            if (trasmissione != null)
            {
                trasmissione.mantieniScrittura = true;
            }
        }
        // End Mev


        // Aggiungo la trasmissione singola
        DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
        trasmissioneSingola.tipoTrasm = tipoTrasm;
        trasmissioneSingola.corrispondenteInterno = corr;
        trasmissioneSingola.ragione = ragione;
        trasmissioneSingola.noteSingole = note;
        trasmissioneSingola.hideDocumentPreviousVersions = nascondiVersioniPrecedenti;

        //Imposto la data di scadenza
        if (scadenza > 0)
        {
            string dataScadenza = "";
            System.DateTime data = System.DateTime.Now.AddDays(scadenza);
            dataScadenza = data.Day + "/" + data.Month + "/" + data.Year;
            trasmissioneSingola.dataScadenza = dataScadenza;
        }

        // Aggiungo la lista di trasmissioniUtente
        if (corr is DocsPaVO.utente.Ruolo)
        {
            trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
            DocsPaVO.utente.Corrispondente[] listaUtenti = queryUtenti(corr, infoUtente);
            if (listaUtenti.Length == 0)
            {
                trasmissioneSingola = null;

                //Andrea
                //throw new ExceptionTrasmissioni("Non è presente alcun utente per la Trasmissione al ruolo: "
                //                                + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                //                                + ".");
                //End Andrea
            }
            else
            {
                //ciclo per utenti se dest è gruppo o ruolo
                for (int i = 0; i < listaUtenti.Length; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaVO.utente.Utente)listaUtenti[i];
                    trasmissioneSingola.trasmissioneUtente = addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                }
            }
        }

        if (corr is DocsPaVO.utente.Utente)
        {
            trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
            DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
            trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;

            //Andrea
            if (trasmissioneUtente.utente == null)
            {
                //throw new ExceptionTrasmissioni("L utente: " + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                //                                + " è inesistente.");

            }
            //End Andrea
            else
                trasmissioneSingola.trasmissioneUtente = addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
        }

        if (corr is DocsPaVO.utente.UnitaOrganizzativa)
        {
            DocsPaVO.utente.UnitaOrganizzativa theUo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
            DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
            qca.ragione = trasmissioneSingola.ragione;
            qca.ruolo = ruolo;
            qca.queryCorrispondente = new DocsPaVO.addressbook.QueryCorrispondente();
            qca.queryCorrispondente.fineValidita = true;

            DocsPaVO.utente.Ruolo[] ruoli = BusinessLogic.Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo).Cast<DocsPaVO.utente.Ruolo>().ToArray();

            //Andrea
            if (ruoli == null || ruoli.Length == 0)
            {
                //throw new ExceptionTrasmissioni("Manca un ruolo di riferimento per la UO: "
                //                                + corr.codiceCorrispondente + " (" + corr.descrizione + ")"
                //                                + ".");
            }
            //End Andrea
            else
            {
                foreach (DocsPaVO.utente.Ruolo r in ruoli)
                    trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm, scadenza, nascondiVersioniPrecedenti, infoUtente, ruolo);
            }
            return trasmissione;
        }

        if (trasmissioneSingola != null)
            trasmissione.trasmissioniSingole = addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
        logger.Debug("FINE addTrasmissioneSingola");
        return trasmissione;
    }

    private static DocsPaVO.trasmissione.Trasmissione impostaNotificheUtentiDaModello(DocsPaVO.trasmissione.Trasmissione objTrasm, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
    {
        try
        {
            if (objTrasm.trasmissioniSingole != null && objTrasm.trasmissioniSingole.Count > 0)
            {
                DocsPaVO.trasmissione.TrasmissioneSingola trasmSingola;
                for (int cts = 0; cts < objTrasm.trasmissioniSingole.Count; cts++)
                {
                    trasmSingola = objTrasm.trasmissioniSingole[cts] as DocsPaVO.trasmissione.TrasmissioneSingola;
                    if ((objTrasm.trasmissioniSingole[cts] as DocsPaVO.trasmissione.TrasmissioneSingola).trasmissioneUtente.Count > 0)
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente trasmUtente;
                        for (int ctu = 0; ctu < (objTrasm.trasmissioniSingole[cts] as DocsPaVO.trasmissione.TrasmissioneSingola).trasmissioneUtente.Count; ctu++)
                        {
                            trasmUtente = trasmSingola.trasmissioneUtente[ctu] as DocsPaVO.trasmissione.TrasmissioneUtente;
                            trasmUtente.daNotificare = daNotificareSuModello(trasmUtente.utente.idPeople, trasmSingola.corrispondenteInterno.systemId, modello);
                        }
                    }
                }
            }
            return objTrasm;
        }
        catch (System.Exception ex)
        {
            return null;
        }
    }

    public static DocsPaVO.trasmissione.Trasmissione saveExecuteTrasm(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.InfoUtente infoUtente)
    {
        DocsPaVO.trasmissione.Trasmissione result = null;
        string desc = string.Empty;
        try
        {
            string urlfrontend = "";
            if (DocsPaVO.Settings.AppSettings.Instance.URL_PATH_IS != null)
                urlfrontend = DocsPaVO.Settings.AppSettings.Instance.URL_PATH_IS.ToString();

            if (infoUtente.delegato != null)
                trasmissione.delegato = infoUtente.delegato.idPeople;
            result = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(urlfrontend, trasmissione);
            string notify = "1";
            if (trasmissione.NO_NOTIFY != null && trasmissione.NO_NOTIFY.Equals("1"))
            {
                notify = "0";
            }
            else
            {
                notify = "1";
            }
            if (result != null)
            {
                // LOG per documento
                if (result.infoDocumento != null && !string.IsNullOrEmpty(result.infoDocumento.idProfile))
                {
                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in result.trasmissioniSingole)
                    {
                        string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                        if (result.infoDocumento.segnatura == null)
                            desc = "Trasmesso Documento : " + result.infoDocumento.docNumber.ToString();
                        else
                            desc = "Trasmesso Documento : " + result.infoDocumento.segnatura.ToString();

                        BusinessLogic.UserLog.UserLog.WriteLog(result.utente.userId, result.utente.idPeople, result.ruolo.idGruppo, result.utente.idAmministrazione, method, result.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, notify, single.systemId);
                    }
                }
            }
            if (result == null)
            {
                throw new Exception();
            }
        }
        catch (System.Exception ex)
        {
            // LOG per documento
            if (trasmissione.infoDocumento != null && !string.IsNullOrEmpty(result.infoDocumento.idProfile))
            {
                if (trasmissione.infoDocumento.segnatura == null)
                    desc = "Trasmesso Documento : " + trasmissione.infoDocumento.docNumber.ToString();
                else
                    desc = "Trasmesso Documento : " + trasmissione.infoDocumento.segnatura.ToString();
                BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, "DOCUMENTOTRASMESSO", trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.KO, null);
            }
            logger.Debug("Errore in DocsPaWS.asmx  - metodo: TrasmissioneSaveExecuteTrasm - ", ex);
            result = null;
        }
        return result;
    }

    private static DocsPaVO.utente.Corrispondente[] queryUtenti(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.InfoUtente infoUtente)
    {
        try
        {
            //costruzione oggetto queryCorrispondente
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;
            qco.idAmministrazione = infoUtente.idAmministrazione;
            qco.fineValidita = true;

            //corrispondenti interni
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
            return BusinessLogic.Utenti.addressBookManager.getListaCorrispondenti(qco).Cast<DocsPaVO.utente.Corrispondente>().ToArray();
        }
        catch (System.Exception ex)
        {
            return null;
        }
    }

    public static ArrayList addTrasmissioneUtente(ArrayList array, DocsPaVO.trasmissione.TrasmissioneUtente nuovoElemento)
    {
        try
        {
            ArrayList nuovaLista = new ArrayList();
            if (array != null)
            {
                nuovaLista.AddRange(array);
                nuovaLista.Add(nuovoElemento);
                return nuovaLista;
            }
            else
            {
                nuovaLista.Add(nuovoElemento);
                return nuovaLista;
            }
        }
        catch (System.Exception ex)
        {
            return null;
        }
    }

    public static ArrayList addTrasmissioneSingola(ArrayList array, DocsPaVO.trasmissione.TrasmissioneSingola nuovoElemento)
    {
        try
        {
            ArrayList nuovaLista = new ArrayList();
            if (array != null)
            {
                nuovaLista.AddRange(array);
                nuovaLista.Add(nuovoElemento);
                return nuovaLista;
            }
            else
            {
                nuovaLista.Add(nuovoElemento);
                return nuovaLista;
            }
        }
        catch (System.Exception ex)
        {
            return null;
        }
    }

    private static bool daNotificareSuModello(string currentIDPeople, string currentIDCorrGlobRuolo, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello)
    {
        bool retValue = true;
        try
        {
            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
            {
                DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];
                    if (mittDest.ID_CORR_GLOBALI.Equals(Convert.ToInt32(currentIDCorrGlobRuolo)))
                    {
                        if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Count > 0)
                        {
                            for (int cut = 0; cut < mittDest.UTENTI_NOTIFICA.Count; cut++)
                            {
                                if ((mittDest.UTENTI_NOTIFICA[cut] as DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm).ID_PEOPLE.Equals(currentIDPeople))
                                {
                                    if ((mittDest.UTENTI_NOTIFICA[cut] as DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm).FLAG_NOTIFICA.Equals("1"))
                                        retValue = true;
                                    else
                                        retValue = false;

                                    return retValue;
                                }
                            }
                        }
                    }
                }
            }
            return retValue;
        }
        catch (System.Exception ex)
        {
            return false;
        }
    }

    public static List<ProcessoFirma> GetProcessiDiFirmaByTitolarePaging(string idRuoloTitolare, string idUtenteTitolare, int numPage, int pageSize, out int numTotPage, out int nRec)
    {
        List<ProcessoFirma> listProcessiDiFirma = new List<ProcessoFirma>();
        numTotPage = 0;
        nRec = 0;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            listProcessiDiFirma = libroFirma.GetProcessiDiFirmaByTitolarePaging(idRuoloTitolare, idUtenteTitolare, numPage, pageSize, out numTotPage, out nRec);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetProcessiDiFirmaByTitolarePaging ", e);
        }
        return listProcessiDiFirma;
    }

    public static string GetDescDiagrammiByIdProcesso(string idProcesso)
    {
        string descDiagramma = string.Empty;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            descDiagramma = libroFirma.GetDescDiagrammiByIdProcesso(idProcesso);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetDescDiagrammiByIdProcesso ", e);
        }
        return descDiagramma;
    }

    public static bool AggiornaDataEsecuzioneElemento(string docnumber, string stato)
    {
        bool retValue = true;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            retValue = libroFirma.AggiornaDataEsecuzioneElemento(docnumber, stato);
        }
        catch (Exception e)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AggiornaDataEsecuzioneElemento ", e);
            return false;
        }
        return retValue;
    }


    public static bool AmmStoricizzaRuoloPassiCorrelati(string idRuoloOld, string idRuoloNew)
    {
        bool result = true;
        try
        {
            if (!string.IsNullOrEmpty(idRuoloOld) && !string.IsNullOrEmpty(idRuoloNew))
            {
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    if (!StoricizzaRuoloPassoProcesso(idRuoloOld, idRuoloNew))
                    {
                        result = false;
                        throw new Exception("Errore durante la storicizzazione del ruolo nei passi del processo");
                    }
                    if (!StoricizzaRuoloPassoIstanza(idRuoloOld, idRuoloNew))
                    {
                        result = false;
                        throw new Exception("Errore durante la storicizzazione del ruolo passi dell'istanza");
                    }
                    if (!StoricizzaRuoloElementiInLibroFirma(idRuoloOld, idRuoloNew))
                    {
                        result = false;
                        throw new Exception("Errore durante la storicizzazione del ruolo negli elementi in libro firma");
                    }
                    if (result)
                        transactionContext.Complete();
                }
            }
            else
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AmmStoricizzaRuoloPassiCorrelati: non sono presenti i campi necessari");
                result = false;
            }
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AmmStoricizzaRuoloPassiCorrelati ", ex);
        }
        return result;
    }

    private static bool StoricizzaRuoloPassoProcesso(string idRuoloOld, string idRuoloNew)
    {
        bool result = true;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            result = libroFirma.StoricizzaRuoloPassoProcesso(idRuoloOld, idRuoloNew);
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: StoricizzaRuoloPassoProcesso ", ex);
        }
        return result;
    }

    private static bool StoricizzaRuoloPassoIstanza(string idRuoloOld, string idRuoloNew)
    {
        bool result = true;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            result = libroFirma.StoricizzaRuoloPassoIstanza(idRuoloOld, idRuoloNew);
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: StoricizzaRuoloPassoIstanza ", ex);
        }
        return result;
    }

    private static bool StoricizzaRuoloElementiInLibroFirma(string idRuoloOld, string idRuoloNew)
    {
        bool result = true;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            result = libroFirma.StoricizzaRuoloElementiInLibroFirma(idRuoloOld, idRuoloNew);
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: StoricizzaRuoloElementiInLibroFirma ", ex);
        }
        return result;
    }

    public static int GetCountProcessiDiFirmaByTitolare(string idRuoloTitolare, string idUtenteTitolare)
    {
        int result = 0;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            result = libroFirma.GetCountProcessiDiFirmaByTitolare(idRuoloTitolare, idUtenteTitolare);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetCountProcessiDiFirmaByTitolare ", e);
        }
        return result;
    }

    public static int GetCountIstanzaProcessiDiFirmaByTitolare(string idRuoloTitolare, string idUtenteTitolare)
    {
        int result = 0;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            result = libroFirma.GetCountIstanzaProcessiDiFirmaByTitolare(idRuoloTitolare, idUtenteTitolare);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetCountIstanzaProcessiDiFirmaByTitolare ", e);
        }
        return result;
    }

    public static bool InvalidaProcessiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro, DocsPaVO.utente.InfoUtente infoUtente, ISpreadsheetService spreadsheetService, IFileConverterFactory fileConverterPDF, IReportGeneratorService reportGeneratorService, IBaseRubricaComuneService rubricaComuneService)
    {
        bool result = true;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            string idReport = libroFirma.InsertReportProcessiTickByRegistroAndEmailRegistro(idRegistro, emailRegistro);
            result = libroFirma.InvalidaProcessiFirmaByIdRegistroAndEmailRegistro(idRegistro, emailRegistro);
            if (result)
                InviaReportCreatoriProcessi(idReport, infoUtente,  spreadsheetService, fileConverterPDF, reportGeneratorService);
            InterrompiIstanzeByIdRegistroAndEmailRegistro(idRegistro, emailRegistro, infoUtente, rubricaComuneService);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InvalidaProcessiFirmaByIdRegistroAndEmailRegistro ", e);
            return false;
        }
        return result;
    }

    private static bool InterrompiIstanzeByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro, DocsPaVO.utente.InfoUtente infoUtente, IBaseRubricaComuneService rubricaComuneService)
    {
        bool result = true;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            List<IstanzaProcessoDiFirma> istanzaProcessiCoinvolti = libroFirma.GetIstanzaProcessiDiFirmaByIdRegistroAndEmailRegistro(idRegistro, emailRegistro, infoUtente);

            if (istanzaProcessiCoinvolti.Count() > 0)
            {
                string noteInterruzione = "Interrotto processo per modifica da amministrazione ad un Registro/RF coinvolto.";
                result = BusinessLogic.LibroFirma.LibroFirmaManager.InterruptionSignatureProcessByAdministrator(istanzaProcessiCoinvolti.ToArray(), noteInterruzione, infoUtente, rubricaComuneService);
            }
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: InterrompiIstanzeRegistriCoinvolti ", ex);
        }
        return result;
    }

    public static bool ExistsPassiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            return libroFirma.ExistsPassiFirmaByIdRegistroAndEmailRegistro(idRegistro, emailRegistro);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: ExistsPassiFirmaByIdRegistroAndEmailRegistro ", e);
            return false;
        }
    }

    private static CMAttachment CreaAllegatoMail(DocsPaVO.documento.FileDocumento report)
    {
        CMAttachment allegato;
        System.IO.FileStream fs = null;
        try
        {
            string basePathFiles = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");

            string pathFiles = System.IO.Path.Combine(basePathFiles, @"ReportProcessiInvalidtati\" + Guid.NewGuid().ToString());
            DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(pathFiles);
            string fileAttPath = pathFiles + "\\" + report.fullName;
            fs = new System.IO.FileStream(fileAttPath, System.IO.FileMode.Create);
            fs.Write(report.content, 0, report.content.Length);
            fs.Close();
            fs = null;

            allegato = new CMAttachment(System.IO.Path.GetFileName(fileAttPath), Interoperabilita.MimeMapper.GetMimeType(System.IO.Path.GetExtension(fileAttPath)), fileAttPath);

        }
        catch (Exception ex)
        {
            logger.Error("Errore nella creazione dell'allegato alla mail" + ex);
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
            allegato = null;
        }
        return allegato;
    }

    public static List<string> GetIdRuoliProcessiUltimoUtente(string idPeople)
    {
        List<string> listIdRuoli = new List<string>();
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            listIdRuoli = libroFirma.GetIdRuoliProcessiUltimoUtente(idPeople);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetIdRuoliProcessiUltimoUtente ", e);
        }
        return listIdRuoli;
    }

    public static bool ExistsPassiFirmaByRuoloTitolareAndRegistro(DocsPaVO.amministrazione.RightRuoloMailRegistro[] rightRuoloMailReg, string idRuoloInUO, string idGruppo)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            return libroFirma.ExistsPassiFirmaByRuoloTitolareAndRegistro(rightRuoloMailReg, idRuoloInUO, idGruppo);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: ExistsPassiFirmaByRuoloTitolareAndRegistro ", e);
            return false;
        }
    }

    public static List<IstanzaProcessoDiFirma> GetIstanzaProcessiDiFirmaByTitolarePaging(string idRuoloTitolare, string idUtenteTitolare, int numPage, int pageSize, out int numTotPage, out int nRec)
    {
        List<IstanzaProcessoDiFirma> listProcessiDiFirma = new List<IstanzaProcessoDiFirma>();
        numTotPage = 0;
        nRec = 0;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            listProcessiDiFirma = libroFirma.GetIstanzaProcessiDiFirmaByTitolarePaging(idRuoloTitolare, idUtenteTitolare, numPage, pageSize, out numTotPage, out nRec);
        }
        catch (Exception e)
        {
            logger.Debug("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: GetIstanzaProcessiDiFirmaByTitolarePaging ", e);
        }
        return listProcessiDiFirma;
    }
    /// <summary>
    /// Esportazione processi di firma
    /// </summary>
    /// <param name="tipoReport"></param>
    /// <param name="idFunzione"></param>
    /// <param name="idAmm"></param>
    /// <returns></returns>
    public static DocsPaVO.documento.FileDocumento GetReportProcessiFirma(string idRuolo, string idUtente, string formato, ISpreadsheetService spreadsheetService, IFileConverterFactory fileConverterPDF, IReportGeneratorService reportGeneratorService)
    {
        DocsPaVO.documento.FileDocumento report = new DocsPaVO.documento.FileDocumento();
        try
        {

            // filtri report
            List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idRuolo", valore = idRuolo });
            filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idUtente", valore = idUtente });

            // request generazione report
            DocsPaVO.Report.PrintReportRequest request = new DocsPaVO.Report.PrintReportRequest();
            request.SearchFilters = filters;

            // selezione formato report
            switch (formato)
            {
                case "XLS":
                    request.ReportType = DocsPaVO.Report.ReportTypeEnum.Excel;
                    break;

                case "ODS":
                    request.ReportType = DocsPaVO.Report.ReportTypeEnum.ODS;
                    break;
            }

            string descrizione = string.Empty;

            string descrizioneRuolo = string.Empty;
            string descrizioneUtente = string.Empty;

            if (!string.IsNullOrEmpty(idRuolo))
                descrizioneRuolo = BusinessLogic.Utenti.UserManager.GetRoleDescriptionByIdGroup(idRuolo);

            if (!string.IsNullOrEmpty(idUtente))
                descrizioneUtente = BusinessLogic.Utenti.UserManager.getUtente(idUtente).descrizione;

            request.ContextName = "ExportProcessiDiFirma";
            request.ReportKey = "ExportProcessiDiFirma";
            request.Title = string.Empty;

            if (!string.IsNullOrEmpty(descrizioneRuolo) && !string.IsNullOrEmpty(descrizioneUtente))
                request.SubTitle = string.Format("Utente: {0} - Ruolo: {1}", descrizioneUtente, descrizioneRuolo);
            else if (!string.IsNullOrEmpty(descrizioneRuolo))
                request.SubTitle = string.Format("Ruolo: {0}", descrizioneRuolo);
            else if (!string.IsNullOrEmpty(descrizioneUtente))
                request.SubTitle = string.Format("Utente: {0}", descrizioneUtente);

            request.AdditionalInformation = string.Empty;

            report = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request, spreadsheetService, fileConverterPDF, reportGeneratorService).Document;

        }
        catch (Exception ex)
        {
            logger.Debug(ex.Message);
        }

        return report;
    }

    public static bool AmmSostituisciUtentePassiCorrelati(string idRuolo, string idOldPeople, string idNewPeople)
    {
        bool result = true;
        try
        {
            if (!string.IsNullOrEmpty(idRuolo) && !string.IsNullOrEmpty(idOldPeople) && !string.IsNullOrEmpty(idNewPeople))
            {
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    if (!SostituisciUtentePassoProcesso(idRuolo, idOldPeople, idNewPeople))
                    {
                        result = false;
                        throw new Exception("Errore durante la sostituzione dell'utente nei passi del processo");
                    }
                    if (!SostituisciUtentePassoIstanza(idRuolo, idOldPeople, idNewPeople))
                    {
                        result = false;
                        throw new Exception("Errore durante la sostituzione dell'utente nei passi dell'istanza");
                    }
                    if (!SostituisciUtenteElementiInLibroFirma(idRuolo, idOldPeople, idNewPeople))
                    {
                        result = false;
                        throw new Exception("Errore durante la sostituzione dell'utente negli elementi in libro firma");
                    }
                    if (result)
                        transactionContext.Complete();
                }
            }
            else
            {
                logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AmmSostituisciUtentePassiCorrelati: non sono presenti i campi necessari");
                result = false;
            }
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: AmmSostituisciUtentePassiCorrelati ", ex);
        }
        return result;
    }

    private static bool SostituisciUtentePassoProcesso(string idRuolo, string idOldPeople, string idNewPeople)
    {
        bool result = true;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            result = libroFirma.SostituisciUtentePassoProcesso(idRuolo, idOldPeople, idNewPeople);
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: SostituisciUtentePassoProcesso ", ex);
        }
        return result;
    }

    private static bool SostituisciUtentePassoIstanza(string idRuolo, string idOldPeople, string idNewPeople)
    {
        bool result = true;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            result = libroFirma.SostituisciUtentePassoIstanza(idRuolo, idOldPeople, idNewPeople);
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: SostituisciUtentePassoIstanza ", ex);
        }
        return result;
    }

    private static bool SostituisciUtenteElementiInLibroFirma(string idRuolo, string idOldPeople, string idNewPeople)
    {
        bool result = true;
        try
        {
            DocsPaDB.Query_DocsPAWS.LibroFirma libroFirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
            result = libroFirma.SostituisciUtenteElementiInLibroFirma(idRuolo, idOldPeople, idNewPeople);
        }
        catch (Exception ex)
        {
            logger.Error("Errore in BusinessLogic.LibroFirma.LibroFirmaManager  - metodo: SostituisciUtenteElementiInLibroFirma ", ex);
        }
        return result;
    }


}
