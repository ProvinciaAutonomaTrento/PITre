using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using DocsPaVO.Procedimento;
using log4net;

namespace BusinessLogic.Procedimenti
{
    public class ProcedimentiManager
    {

        private static ILog logger = LogManager.GetLogger(typeof(ProcedimentiManager));

        public static bool InsertDoc(string idProject, string idProfile, string idCorrGlobali, bool isDocPrincipale, string idProcedimento, bool visualizzato)
        {
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            return proc.InsertDoc(idProject, idProfile, idCorrGlobali, isDocPrincipale, idProcedimento, visualizzato);
        }

        public static bool InsertFaseProcedimento(string idProject, string idStato)
        {
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            return proc.InsertFaseProcedimento(idProject, idStato);
        }

        public static bool UpdateStato(string idProfile, string idProject)
        {
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            return proc.UpdateStato(idProfile, idProject);
        }

        public static bool IsDocVisualizzato(string idProject, string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            return proc.IsDocVisualizzato(idProject, idProfile);
        }

        public static bool IsProcedimento(string idFascicolo)
        {
            bool result = false;

            Procedimento proc = GetProcedimentoByIdFascicolo(idFascicolo);
            if (proc != null && !string.IsNullOrEmpty(proc.Id) && proc.Id.Equals(idFascicolo))
                result = true;

            return result;
        }

        public static bool IsFolderInProceeding(string idFolder)
        {
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            return proc.IsFolderInProceeding(idFolder);
        }

        public static List<Procedimento> GetProcedimentiNonVisualizzati(string idCorrGlobali)
        {
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            return proc.GetProcedimentiNonVisualizzati(idCorrGlobali);
        }

        public static Procedimento GetProcedimentoByIdFascicolo(string idFascicolo)
        {
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            return proc.GetProcedimentoByIdFascicolo(idFascicolo);
        }

        public static Procedimento GetProcedimentoByIdFolder(string idFolder)
        {
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            return proc.GetProcedimentoByIdFolder(idFolder);
        }

        public static Procedimento GetProcedimentoByIdEsterno(string idProcedimento)
        {
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            return proc.GetProcedimentoByIdEsterno(idProcedimento);
        }

        public static Procedimento GetProcedimentoByIdDoc(string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            return proc.GetProcedimentoByIdDoc(idProfile);
        }

        public static EsitoProcedimento GetEsitoProcedimento(string idFascicolo)
        {
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            return proc.GetEsitoProcedimento(idFascicolo);
        }

        public static string[] GetTipiProcedimentoAmministrazione(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            return proc.GetTipiProcedimentoAmministrazione(idAmm);
        }

        public static void CambioStatoProcedimento(string idFascicolo, string tipoEvento, string idOggetto, DocsPaVO.utente.InfoUtente utente)
        {
            logger.Debug("BEGIN");
            DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
            string idStato = proc.GetIdPerCambioStato(tipoEvento, idOggetto);

            if (!string.IsNullOrEmpty(idStato))
            {
                logger.Debug("Stato: " + idStato);
                DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascDettagli(idFascicolo);
                if (template != null)
                {
                    int idDiagram = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociatoFasc(template.ID_TIPO_FASC);
                    if (idDiagram != 0)
                    {
                        DocsPaVO.DiagrammaStato.DiagrammaStato stateDiagram = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagram.ToString());
                        if (stateDiagram != null)
                        {
                            logger.DebugFormat("Trovato evento per cambio stato - stato={0} fascicolo={1} tipoevento={2}", idStato,idFascicolo,tipoEvento);
                            BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStatoFasc(idFascicolo, idStato, stateDiagram, utente.idPeople, utente, string.Empty);

                            // CABLATURA PER DEMO 21/11
                            if (tipoEvento.ToUpper() == "ACCETTAZIONE")
                            {
                                DocsPaVO.trasmissione.RagioneTrasmissione ragTrasm = BusinessLogic.Trasmissioni.RagioniManager.getRagione(idOggetto);
                                if (ragTrasm != null)
                                {
                                    if (template.ELENCO_OGGETTI != null && template.ELENCO_OGGETTI.Count > 0)
                                    {
                                        bool toUpdate = false;
                                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI)
                                        {
                                            if (ogg.DESCRIZIONE.ToUpper() == "RUOLO ASSEGNATARIO")
                                            {
                                                logger.Debug("Ruolo assegnatario - ID=" + utente.idCorrGlobali);
                                                ogg.VALORE_DATABASE = utente.idCorrGlobali;
                                                toUpdate = true;
                                            }
                                            if (ogg.DESCRIZIONE.ToUpper() == "UTENTE ASSEGNATARIO")
                                            {
                                                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByIdPeople(utente.idPeople, DocsPaVO.addressbook.TipoUtente.INTERNO, utente);
                                                if (corr != null)
                                                {
                                                    logger.Debug("Utente assegnatario - idPeople=" + utente.idPeople + " - idCorrGlobali=" + corr.systemId);
                                                    ogg.VALORE_DATABASE = corr.systemId;
                                                    toUpdate = true;
                                                }
                                            }
                                        }
                                        if (toUpdate)
                                        {
                                            BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.salvaInserimentoUtenteProfDimFasc(template, idFascicolo);
                                        }
                                    }
                                }
                            }
                        }                    
                    }
                }
            }

            logger.Debug("END");
        }

        public static void PopolaCampiProcedimento(ref DocsPaVO.ProfilazioneDinamica.Templates template, DocsPaVO.utente.InfoUtente infoUtente)
        {
            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI)
            {
                switch (ogg.DESCRIZIONE.ToUpper())
                {
                    case "TERMINI":
                        ogg.VALORE_DATABASE = "120"; // CABLATO PER ORA
                        break;

                    case "RESPONSABILE PROCEDIMENTO":
                        DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica("renzo.piano", infoUtente); // CABLATO PER ORA
                        if (corr != null)
                        {
                            ogg.VALORE_DATABASE = corr.systemId;
                        }
                        break;
                }
            }
        }

        public static DocsPaVO.Procedimento.Report.ReportProcedimentoResponse GetProcedimentiReport(DocsPaVO.Procedimento.Report.ReportProcedimentoRequest request)
        {
            DocsPaVO.Procedimento.Report.ReportProcedimentoResponse response = new DocsPaVO.Procedimento.Report.ReportProcedimentoResponse();

            try
            {
                DocsPaVO.documento.FileDocumento report = new DocsPaVO.documento.FileDocumento();

                DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();

                List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();

                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "ID_TIPO_FASC", valore = request.IdProcedimento });
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "ANNO", valore = request.Anno });

                List<DettaglioProcedimento> items = proc.GetProcedimentiReport(filters);

                if (items.Count > 0)
                {
                    BusinessLogic.Modelli.AsposeModelProcessor.PdfModelProcessor processor = new Modelli.AsposeModelProcessor.PdfModelProcessor();

                    report = processor.CreaReportProcedimentoSingolo(request, items);

                    response.Doc = report;
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                logger.Error("Errore in GetProcedimentiReport: ", ex);
                response.Doc = null;
                response.Success = false;
            }

            return response;
        }

        public static DocsPaVO.documento.DocumentConsolidationStateInfo ConsolidateNoSecurity(DocsPaVO.utente.InfoUtente userInfo, string idDocument, DocsPaVO.documento.DocumentConsolidationStateEnum toState, bool bypassFinalStateCheck)
        {
            return BusinessLogic.Documenti.DocumentConsolidation.ConsolidateNoSecurity(userInfo, idDocument, toState, bypassFinalStateCheck);
        }

        public static void CheckEventiFirma(string idProfile, string idPeople, string idGruppo, string idAmm)
        {
            logger.Debug("BEGIN");
            try
            {
                DocsPaVO.ProfilazioneDinamica.Templates template = ProfilazioneDinamica.ProfilazioneDocumenti.getTemplate(idProfile);

                if (template != null)
                {
                    DocsPaVO.utente.InfoUtente infoUt = new DocsPaVO.utente.InfoUtente();
                    DocsPaVO.utente.Utente u = Utenti.UserManager.getUtenteById(idPeople);
                    DocsPaVO.utente.Ruolo r = Utenti.UserManager.getRuoloByIdGruppo(idGruppo);
                    infoUt = Utenti.UserManager.GetInfoUtente(u, r);

                    ArrayList listaFascicoli = BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDocNoSecurity(infoUt, idProfile);
                    logger.Debug("Analisi fascicoli");
                    foreach (DocsPaVO.fascicolazione.Fascicolo f in listaFascicoli)
                    {
                        DocsPaVO.ProfilazioneDinamica.Templates templateFasc = ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFasc(f.systemID);
                        if (templateFasc != null && templateFasc.SYSTEM_ID != 0)
                        {
                            logger.Debug("Fascicolo ID=" + f.systemID + " tipizzato - " + templateFasc.DESCRIZIONE);
                            CambioStatoProcedimento(f.systemID, "FIRMA", template.SYSTEM_ID.ToString(), infoUt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in CheckEventiFirma", ex);
            }

            logger.Debug("END");
        }
    }
}
