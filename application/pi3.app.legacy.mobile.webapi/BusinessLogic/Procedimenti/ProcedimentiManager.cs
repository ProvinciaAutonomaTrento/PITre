// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Procedimento;
using Serilog;
using System.Collections;

namespace BusinessLogic.Procedimenti;

public class ProcedimentiManager
{
    private static ILogger logger = Log.ForContext(typeof(ProcedimentiManager));

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
                        logger.Debug("Trovato evento per cambio stato - stato={0} fascicolo={1} tipoevento={2}", idStato, idFascicolo, tipoEvento);
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

    public static bool IsProcedimento(string idFascicolo)
    {
        bool result = false;

        Procedimento proc = GetProcedimentoByIdFascicolo(idFascicolo);
        if (proc != null && !string.IsNullOrEmpty(proc.Id) && proc.Id.Equals(idFascicolo))
            result = true;

        return result;
    }

    public static bool InsertFaseProcedimento(string idProject, string idStato)
    {
        DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
        return proc.InsertFaseProcedimento(idProject, idStato);
    }

    public static Procedimento GetProcedimentoByIdFascicolo(string idFascicolo)
    {
        DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
        return proc.GetProcedimentoByIdFascicolo(idFascicolo);
    }

    public static bool InsertDoc(string idProject, string idProfile, string idCorrGlobali, bool isDocPrincipale, string idProcedimento, bool visualizzato)
    {
        DocsPaDB.Query_DocsPAWS.Procedimenti proc = new DocsPaDB.Query_DocsPAWS.Procedimenti();
        return proc.InsertDoc(idProject, idProfile, idCorrGlobali, isDocPrincipale, idProcedimento, visualizzato);
    }
}
