// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ProfilazioneDinamicaLite;
using Pi3.Core.Extensions;
using Serilog;
using System.Collections;

namespace BusinessLogic.ProfilazioneDinamica;

public class ProfilazioneDocumenti
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(ProfilazioneDocumenti));
    public static DocsPaVO.ProfilazioneDinamica.Templates getTemplate(string docNumber)
    {

        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplate(docNumber);
                transactionContext.Complete();
                return template;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTemplate", e);
                return null;
            }
        }
    }

    public static DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] getListTemplatesLite(string idAmministrazione)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                TemplateLite[] result = null;
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                result = modelDB.getListTemplatesLite(idAmministrazione);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getListTemplatesLite", e);
                return null;
            }
        }
    }

    public static string getIdAmmByCod(string codiceAmministrazione)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                string idAmm = modelDB.getIdAmmByCod(codiceAmministrazione);
                transactionContext.Complete();
                return idAmm;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getIdAmmByCod", e);
                return null;
            }
        }
    }

    public static string getIdTemplate(string docNumber)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                string idTemplate = modelDB.getIdTemplate(docNumber);
                transactionContext.Complete();
                return idTemplate;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getIdTemplate", e);
                return null;
            }
        }
    }

    public static bool isDocRepertoriato(string docNumber, string idTipoAtto)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                bool result = modelDB.isDocRepertoriato(docNumber, idTipoAtto);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: isDocRepertoriato", e);
                return false;
            }
        }
    }

    public static DocsPaVO.ProfilazioneDinamica.Templates getTemplateById(string idTemplate)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplateById(idTemplate);
                transactionContext.Complete();
                return template;

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTemplateById", e);
                return null;
            }
        }
    }

    public static ArrayList getDirittiCampiTipologiaDoc(string idRuolo, string idTemplate)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                ArrayList dirittiCampi = modelDB.getDirittiCampiTipologiaDoc(idRuolo, idTemplate);
                transactionContext.Complete();
                return dirittiCampi;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getDirittiCampiTipologiaDoc", e);
                return null;
            }
        }
    }

    public static DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli getDirittiCampoTipologiaDoc(string idRuolo, string idTemplate, string idOggettoCustom)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoliResult = modelDB.getDirittiCampoTipologiaDoc(idRuolo, idTemplate, idOggettoCustom);
                transactionContext.Complete();
                return assDocFascRuoliResult;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getDirittiCampoTipologiaDoc", e);
                return null;
            }
        }
    }

    public static void salvaAssociazioneModelli(string idTipoDoc, string idDiagramma, ArrayList modelliSelezionati, string idStato)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.salvaAssociazioneModelli(idTipoDoc, idDiagramma, modelliSelezionati, idStato);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: salvaAssociazioneModelli", e);
            }
        }
    }

    public static ArrayList getIdModelliTrasmAssociati(string idTipoDoc, string idDiagramma, string idStato)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                ArrayList idModelliAssociati = modelDB.getIdModelliTrasmAssociati(idTipoDoc, idDiagramma, idStato);
                transactionContext.Complete();
                return idModelliAssociati;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getIdModelliTrasmAssociati", e);
                return null;
            }
        }
    }

    public static ArrayList getRuoliByAmm(string idAmm, string codiceRicerca, string tipoRicerca)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                ArrayList ruoli = modelDB.getRuoliByAmm(idAmm, codiceRicerca, tipoRicerca);
                transactionContext.Complete();
                return ruoli;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getRuoliByAmm", e);
                return null;
            }
        }
    }

    public static ArrayList getTemplatesArchivioDeposito(string idAmm, bool seRepertorio)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                ArrayList tamplateArchivio = modelDB.getTemplatesArchivioDeposito(idAmm, seRepertorio);
                transactionContext.Complete();
                return tamplateArchivio;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTemplatesArchivioDeposito", e);
                return null;
            }
        }
    }

    public static ArrayList getRuoliFromOggettoCustomDoc(string idTemplate, string idOggettoCustom)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                ArrayList ruoliFromOggettoCustom = modelDB.getRuoliFromOggettoCustomDoc(idTemplate, idOggettoCustom);
                transactionContext.Complete();
                return ruoliFromOggettoCustom;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getRuoliFromOggettoCustomDoc", e);
                return null;
            }
        }
    }

    public static DocsPaVO.ProfilazioneDinamica.OggettoCustom getOggettoById(string idOggetto)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = modelDB.getOggettoById(idOggetto);
                transactionContext.Complete();
                return oggettoCustom;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getOggettoById", e);
                return null;
            }
        }
    }

    public static DocsPaVO.ProfilazioneDinamica.Contatore[] GetValuesContatoriDoc(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                DocsPaVO.ProfilazioneDinamica.Contatore[] listaContatori = modelDB.GetValuesContatoriDoc(oggettoCustom);
                transactionContext.Complete();
                return listaContatori;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: GetValuesContatoriDoc", e);
                return null;
            }
        }
    }

    public static void DeleteValueContatoreDoc(DocsPaVO.ProfilazioneDinamica.Contatore contatore)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.DeleteValueContatoreDoc(contatore);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: DeleteValueContatoreDoc", e);
            }
        }
    }

    /// <summary>
    /// Metodo per il recupero della lista con le informazioni relative allo
    /// stato di abilitazione dello storico per i campi di una determinata tipologia
    /// </summary>
    /// <param name="templateId">Id della tipologia di cui caricare le informazioni</param>
    /// <returns>Lista di oggetti con le informazioni sullo stato di abilitazione dello storico per i campi che compongono la tipologia</returns>
    public static SelectiveHistoryResponse GetCustomHistoryList(SelectiveHistoryRequest request)
    {
        Model profDb = new Model();
        SelectiveHistoryResponse response = new SelectiveHistoryResponse();
        response.Fields = profDb.GetCustomHistoryList(request.TemplateId);

        return response;
    }

    /// <summary>
    /// Metodo per l'attivazione dello storico su determinati campi di una tipolgia
    /// </summary>
    /// <param name="request">Informazioni su tipologia e campi della tipologia per cui attivare lo storico</param>
    /// <returns>Esito della richiesta</returns>
    public static DocsPaVO.ProfilazioneDinamica.SelectiveHistoryResponse ActiveSelectiveHistory(DocsPaVO.ProfilazioneDinamica.SelectiveHistoryRequest request)
    {
        Model profDb = new Model();
        bool result = false;

        // Se bisogna abilitare lo storico per tutti i campi della tipologia, viene
        // richiamata la funzione che abilita tutti gli storici per la tipolgia
        if (request.ActiveAllFields)
            result = profDb.ActiveSelectiveHistory(request.TemplateId);
        else
            result = profDb.ActiveSelectiveHistory(request.TemplateId, request.CustomObjects);

        return new SelectiveHistoryResponse() { Result = result };
    }

    public static bool isInUseCampoComuneDoc(string idTemplate, string idCampoComune)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                bool result = modelDB.isInUseCampoComuneDoc(idTemplate, idCampoComune);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: isInUseCampoComuneDoc", e);
                return false;
            }
        }
    }

    public static bool eliminaOggettoCustomDaDB(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DocsPaVO.ProfilazioneDinamica.Templates template)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                bool result = modelDB.eliminaOggettoCustomDaDB(oggettoCustom, template);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: eliminaOggettoCustomDaDB", e);
                return false;
            }
        }
    }

    public static void aggiornaPosizione(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DocsPaVO.ProfilazioneDinamica.Templates template)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.aggiornaPosizione(oggettoCustom, template);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: aggiornaPosizione", e);
            }
        }
    }

    public static int countDocTipoDoc(string tipo_atto, string codiceAmm)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                int numberDocTipoDoc = modelDB.countDocTipoDoc(tipo_atto, codiceAmm);
                transactionContext.Complete();
                return numberDocTipoDoc;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: countDocTipoDoc", e);
                return 0;
            }
        }
    }

    public static void UpdatePrivatoTipoDoc(int systemId_template, string privato)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.UpdatePrivatoTipoDoc(systemId_template, privato);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: UpdatePrivatoTipoDoc", e);
            }
        }
    }

    public static void UpdateMesiConsTipoDoc(int systemId_template, string mesiCons)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.UpdateMesiConsTipoDoc(systemId_template, mesiCons);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: UpdatePrivatoTipoDoc", e);
            }
        }
    }

    public static void UpdateInvioConsTipoDoc(int systemId_template, string invioCons)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.UpdateInvioConsTipoDoc(systemId_template, invioCons);
                transactionContext.Complete();
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in ProfilazioneDocumenti - metodo: UpdateInvioConsTipoDoc", ex);
            }
        }
    }

    public static void UpdateConsolidaCampo(int systemId, string consolida, string systemId_template)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.UpdateConsolidaCampo(systemId, consolida, systemId_template);
                transactionContext.Complete();
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in ProfilazioneDocumenti - metodo: UpdateConsolidaCampo", ex);
            }
        }
    }

    public static void UpdateConservaCampo(int systemId, string conserva, string systemId_template)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.UpdateConservaCampo(systemId, conserva, systemId_template);
                transactionContext.Complete();
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in ProfilazioneDocumenti - metodo: UpdateConservaCampo", ex);
            }
        }
    }

    public static bool isValueInUse(string idOggetto, string idTemplate, string valoreOggettoDB)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                bool result = modelDB.isValueInUse(idOggetto, idTemplate, valoreOggettoDB);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: isValueInUse", e);
                return false;
            }
        }
    }

    public static ArrayList getRuoliTipoDoc(string idTipoDoc)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                ArrayList ruoli = modelDB.getRuoliTipoDoc(idTipoDoc);
                transactionContext.Complete();
                return ruoli;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getRuoliTipoDoc", e);
                return null;
            }
        }
    }

    public static bool salvaTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                bool result = modelDB.salvaTemplate(template, idAmministrazione);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: salvaTemplate", e);
                return false;
            }
        }
    }

    public static bool aggiornaTemplate(DocsPaVO.ProfilazioneDinamica.Templates template)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                bool result = modelDB.aggiornaTemplate(template);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: aggiornaTemplate", e);
                return false;
            }
        }
    }

    public static void messaInEsercizioTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.messaInEsercizioTemplate(template, idAmministrazione);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: messaInEsercizioTemplate", e);
            }
        }
    }

    public static bool UpdateIsTypeInstance(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
    {
        bool result = true;
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                result = modelDB.UpdateIsTypeInstance(template, idAmministrazione);
                if (result)
                    transactionContext.Complete();

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: UpdateIsTypeInstance", e);
                return false; ;
            }
            return result;
        }
    }

    public static bool disabilitaTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione, string serverPath, string codiceAmministrazione)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                bool result = modelDB.disabilitaTemplate(template, idAmministrazione, serverPath, codiceAmministrazione);
                transactionContext.Complete();
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: disabilitaTemplate", e);
                return false;
            }
        }
    }

    public static void aggiornaPosizioni(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_1, DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_2, DocsPaVO.ProfilazioneDinamica.Templates template)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.aggiornaPosizioni(oggettoCustom_1, oggettoCustom_2, template);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: aggiornaPosizioni", e);
            }
        }
    }

    public static void updateScadenzeTipoDoc(int systemId_template, string scadenza, string preScadenza)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.updateScadenzeTipoDoc(systemId_template, scadenza, preScadenza);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: updateScadenzeTipoDoc", e);
            }
        }
    }

    public static void salvaDirittiCampiTipologiaDoc(ArrayList listaDirittiCampiSelezionati)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.salvaDirittiCampiTipologiaDoc(listaDirittiCampiSelezionati);
                transactionContext.Complete();

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: salvaDirittiCampiTipologiaDoc", e);
            }
        }
    }

    public static void estendiDirittiCampiARuoliDoc(ArrayList listaDirittiCampiSelezionati, ArrayList listaRuoli)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.estendiDirittiCampiARuoliDoc(listaDirittiCampiSelezionati, listaRuoli);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: estendiDirittiCampiARuoli", e);
            }
        }
    }

    public static DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(
                    DocsPaVO.utente.InfoUtente infoUtente,
                    string idDocumento, string anno, string idRegistro, bool sbloccati, string IdTipologia, bool Protocollati, string IdAmministrazione)
    {
        using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            return amm.GetDocumentiStatoFinale(infoUtente, idDocumento, anno, idRegistro, sbloccati, IdTipologia, Protocollati, IdAmministrazione);
    }

    public static DocsPaVO.ProfilazioneDinamica.Templates getTemplateCampiComuniById(DocsPaVO.utente.InfoUtente infoUtente, string idTemplate)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplateCampiComuniById(infoUtente, idTemplate);
                transactionContext.Complete();
                return template;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTemplateCampiComuniById", e);
                return null;
            }
        }
    }

    public static DocsPaVO.ProfilazioneDinamica.Templates getTemplateByDescrizione(string descrizioneTemplate, string idAmm)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplateByDescrizione(descrizioneTemplate, idAmm);
                transactionContext.Complete();
                return template;

            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTemplateByDescrizione", e);
                return null;
            }
        }
    }

    public static ArrayList getTemplates(string idAmministrazione)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                ArrayList templates = modelDB.getTemplates(idAmministrazione);
                transactionContext.Complete();
                return templates;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTemplates", e);
                return null;
            }
        }
    }

    public static void salvaAssociazioneDocRuoli(ArrayList assDocRuoli)
    {
        //using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        //{
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.salvaAssociazioneDocRuoli(assDocRuoli);
                //transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: salvaAssociazioneDocRuoli", e);
            }
        //}
    }

    public static void salvaModelli(byte[] dati, string nomeProfilo, string codiceAmministrazione, string nomeFile, string estensione, string serverPath, DocsPaVO.ProfilazioneDinamica.Templates template)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.salvaModelli(dati, nomeProfilo, codiceAmministrazione, nomeFile, estensione, serverPath.PathAsUnixPath(), template);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: salvaModelli", e);
            }
        }
    }

    public static void estendiDirittiRuoloACampiDoc(ArrayList listaDirittiRuoli, ArrayList listaCampi)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.estendiDirittiRuoloACampiDoc(listaDirittiRuoli, listaCampi);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: estendiDirittiRuoloACampiDoc", e);
            }
        }
    }


    public static void eliminaModelli(string nomeProfilo, string codiceAmministrazione, string nomeFile, string estensione, string serverPath, DocsPaVO.ProfilazioneDinamica.Templates template)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                modelDB.eliminaModelli(nomeProfilo, codiceAmministrazione, nomeFile, estensione, serverPath.PathAsUnixPath(), template);
                transactionContext.Complete();
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: eliminaModelli", e);
            }
        }
    }

    public static DocsPaVO.ProfilazioneDinamica.Templates impostaCampiComuniDoc(DocsPaVO.ProfilazioneDinamica.Templates modello, ArrayList campiComuni)
    {
        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.impostaCampiComuniDoc(modello, campiComuni);
                transactionContext.Complete();
                return template;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in ProfilazioneDocumenti  - metodo: impostaCampiComuniDoc", e);
                return null;
            }
        }
    }

}
