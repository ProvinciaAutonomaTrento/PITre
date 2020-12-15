using System;
using System.Collections;
using System.Data;
using DocsPaDB.Query_DocsPAWS;
using log4net;
using DocsPaVO.ProfilazioneDinamicaLite;

namespace BusinessLogic.ProfilazioneDinamica
{
    public class ProfilazioneFascicoli
    {
        private static ILog logger = LogManager.GetLogger(typeof(ProfilazioneFascicoli));

        public ProfilazioneFascicoli() { }

        public static ArrayList getTemplatesFasc(string idAmministrazione)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    ArrayList templatesFasc = modelFascDB.getTemplatesFasc(idAmministrazione);
                    transactionContext.Complete();
                    return templatesFasc;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getTemplatesFasc", e);
                    return null;
                }
            }
        }

        public static bool eliminaOggettoCustomDaDBFasc(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    bool result = modelFascDB.eliminaOggettoCustomDaDBFasc(oggettoCustom, template);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: eliminaOggettoCustomDaDBFasc", e);
                    return false;
                }
            }
        }

        public static void aggiornaPosizioneFasc(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    modelFascDB.aggiornaPosizioneFasc(oggettoCustom, template);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: aggiornaPosizioneFasc", e);
                }
            }
        }

        public static bool salvaTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    bool result = modelFascDB.salvaTemplateFasc(template, idAmministrazione);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: salvaTemplateFasc", e);
                    return false;
                }
            }
        }

        public static bool aggiornaTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    bool result = modelFascDB.aggiornaTemplateFasc(template);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: aggiornaTemplateFasc", e);
                    return false;
                }
            }
        }

        public static bool isValueInUseFasc(string idOggetto, string idTemplate, string valoreOggettoDB)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    bool result = modelFascDB.isValueInUseFasc(idOggetto, idTemplate, valoreOggettoDB);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: isValueInUseFasc", e);
                    return false;
                }
            }
        }

        public static bool disabilitaTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione, string serverPath, string codiceAmministrazione)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    bool result = modelFascDB.disabilitaTemplateFasc(template, idAmministrazione, serverPath, codiceAmministrazione);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: disabilitaTemplateFasc", e);
                    return false;
                }
            }
        }

        public static DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascById(string idTemplate)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelFascDB.getTemplateFascById(idTemplate);
                    transactionContext.Complete();
                    return template;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getTemplateFascById", e);
                    return null;
                }
            }
        }

        public static DocsPaVO.ProfilazioneDinamica.Templates getTemplatePerRicerca(string idAmministrazione, string tipoFasc)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplatePerRicerca(idAmministrazione, tipoFasc);
                    transactionContext.Complete();
                    return template;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getTemplatePerRicerca", e);
                    return null;
                }
            }
        }

        public static void aggiornaPosizioniFasc(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_1, DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom_2, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    modelFascDB.aggiornaPosizioniFasc(oggettoCustom_1, oggettoCustom_2, template);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: aggiornaPosizioniFasc", e);
                }
            }
        }

        public static void messaInEsercizioTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    modelFascDB.messaInEsercizioTemplateFasc(template, idAmministrazione);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: messaInEsercizioTemplateFasc", e);
                }
            }
        }

        public static ArrayList getTipoFasc(string idAmministrazione)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    ArrayList tipoFasc = modelFascDB.getTipoFasc(idAmministrazione);
                    transactionContext.Complete();
                    return tipoFasc;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getTipoFasc", e);
                    return null;
                }
            }
        }

        public static ArrayList getTipoFascFromRuolo(string idAmministrazione, string idRuolo, string diritti)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    ArrayList tipoFascFromRuolo = modelFascDB.getTipoFascFromRuolo(idAmministrazione, idRuolo, diritti);
                    transactionContext.Complete();
                    return tipoFascFromRuolo;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getTipoFascFromRuolo", e);
                    return null;
                }
            }
        }

        public static void salvaInserimentoUtenteProfDimFasc(DocsPaVO.ProfilazioneDinamica.Templates template, string idProject)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    modelFascDB.salvaInserimentoUtenteProfDimFasc(template, idProject);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: salvaInserimentoUtenteProfDimFasc", e);
                }
            }
        }

        public static DocsPaVO.ProfilazioneDinamica.OggettoCustom getOggettoById(string idOggetto)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = modelFascDB.getOggettoById(idOggetto);
                    transactionContext.Complete();
                    return oggettoCustom;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getOggettoById", e);
                    return null;
                }
            }
        }

        public static DocsPaVO.ProfilazioneDinamica.Templates getTemplateFasc(string idProject)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelFascDB.getTemplateFasc(idProject);
                    transactionContext.Complete();
                    return template;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getTemplateFasc", e);
                    return null;
                }
            }
        }

        public static DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascDettagli(string idProject)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelFascDB.getTemplateFascDettagli(idProject);
                    transactionContext.Complete();
                    return template;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getTemplateFascDettagli", e);
                    return null;
                }
            }
        }

        public static ArrayList getRuoliTipoFasc(string idTipoFasc)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    ArrayList ruoliTipoFasc = modelFascDB.getRuoliTipoFasc(idTipoFasc);
                    transactionContext.Complete();
                    return ruoliTipoFasc;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getRuoliTipoFasc", e);
                    return null;
                }
            }
        }

        public static void salvaAssociazioneFascRuoli(ArrayList assFascRuoli)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    modelFascDB.salvaAssociazioneFascRuoli(assFascRuoli);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: salvaAssociazioneFascRuoli", e);
                }
            }
        }

        public static bool existTemplatesFasc()
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    bool result = modelFascDB.existTemplatesFasc();
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: existTemplatesFasc", e);
                    return false;
                }
            }
        }

        public static DocsPaVO.ProfilazioneDinamica.Templates impostaCampiComuniFasc(DocsPaVO.ProfilazioneDinamica.Templates modello, ArrayList campiComuni)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelFascDB.impostaCampiComuniFasc(modello, campiComuni);
                    transactionContext.Complete();
                    return template;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: impostaCampiComuniFasc", e);
                    return null;
                }
            }
        }

        public static bool isInUseCampoComuneFasc(string idTemplate, string idCampoComune)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    bool result = modelFascDB.isInUseCampoComuneFasc(idTemplate, idCampoComune);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: isInUseCampoComuneFasc", e);
                    return false;
                }
            }
        }

        public static int countFascTipoFasc(string tipo_fasc, string codiceAmm)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    int numberFascTipoFasc = modelFascDB.countFascTipoFasc(tipo_fasc, codiceAmm);
                    transactionContext.Complete();
                    return numberFascTipoFasc;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: countFascTipoFasc", e);
                    return 0;
                }
            }
        }

        public static void UpdatePrivatoTipoFasc(int systemId_template, string privato)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    modelFascDB.UpdatePrivatoTipoFasc(systemId_template, privato);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: UpdatePrivatoTipoFasc", e);
                }
            }
        }

        public static void UpdateMesiConsTipoFasc(int systemId_template, string mesiCons)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    modelFascDB.UpdateMesiConsTipoFasc(systemId_template, mesiCons);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: UpdatePrivatoTipoFasc", e);
                }
            }
        }

        public static ArrayList getIdModelliTrasmAssociatiFasc(string idTipoFasc, string idDiagramma, string idStato)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    ArrayList idModelliAssociati = modelFascDB.getIdModelliTrasmAssociatiFasc(idTipoFasc, idDiagramma, idStato);
                    transactionContext.Complete();
                    return idModelliAssociati;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getIdModelliTrasmAssociatiFasc", e);
                    return null;
                }
            }
        }

        public static void salvaAssociazioneModelliFasc(string idTipoFasc, string idDiagramma, ArrayList modelliSelezionati, string idStato)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    modelFascDB.salvaAssociazioneModelliFasc(idTipoFasc, idDiagramma, modelliSelezionati, idStato);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: salvaAssociazioneModelliFasc", e);
                }
            }
        }

        public static void updateScadenzeTipoFasc(int systemId_template, string scadenza, string preScadenza)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    modelFascDB.updateScadenzeTipoFasc(systemId_template, scadenza, preScadenza);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: updateScadenzeTipoFasc", e);
                }
            }
        }

        public static string getIdTemplateFasc(string idProject)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    string idTemplate = modelFascDB.getIdTemplateFasc(idProject);
                    transactionContext.Complete();
                    return idTemplate;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getIdTemplateFasc", e);
                    return null;
                }
            }
        }

        public static DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascCampiComuniById(DocsPaVO.utente.InfoUtente infoUtente, string idTemplate)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelFascDB.getTemplateFascCampiComuniById(infoUtente, idTemplate);
                    transactionContext.Complete();
                    return template;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getTemplateFascCampiComuniById", e);
                    return null;
                }
            }
        }

        public static ArrayList getDirittiCampiTipologiaFasc(string idRuolo, string idTemplate)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    ArrayList dirittiCampi = modelDB.getDirittiCampiTipologiaFasc(idRuolo, idTemplate);
                    transactionContext.Complete();
                    return dirittiCampi;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getDirittiCampiTipologiaFasc", e);
                    return null;
                }
            }
        }

        public static void salvaDirittiCampiTipologiaFasc(ArrayList listaDirittiCampiSelezionati)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    modelDB.salvaDirittiCampiTipologiaFasc(listaDirittiCampiSelezionati);
                    transactionContext.Complete();

                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: salvaDirittiCampiTipologiaFasc", e);
                }
            }
        }

        public static void estendiDirittiCampiARuoliFasc(ArrayList listaDirittiCampiSelezionati, ArrayList listaRuoli)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    modelDB.estendiDirittiCampiARuoliFasc(listaDirittiCampiSelezionati, listaRuoli);
                    transactionContext.Complete();

                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: estendiDirittiCampiARuoliFasc", e);
                }
            }
        }

        public static DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli getDirittiCampoTipologiaFasc(string idRuolo, string idTemplate, string idOggettoCustom)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli assDocFascRuoliResult = modelDB.getDirittiCampoTipologiaFasc(idRuolo, idTemplate, idOggettoCustom);
                    transactionContext.Complete();
                    return assDocFascRuoliResult;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getDirittiCampoTipologiaFasc", e);
                    return null;
                }
            }
        }

        public static void estendiDirittiRuoloACampiFasc(ArrayList listaDirittiRuoli, ArrayList listaCampi)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    modelDB.estendiDirittiRuoloACampiFasc(listaDirittiRuoli, listaCampi);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: estendiDirittiRuoloACampiFasc", e);
                }
            }
        }

        public static ArrayList getRuoliFromOggettoCustomFasc(string idTemplate, string idOggettoCustom)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    ArrayList ruoliFromOggettoCustom = modelDB.getRuoliFromOggettoCustomFasc(idTemplate, idOggettoCustom);
                    transactionContext.Complete();
                    return ruoliFromOggettoCustom;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getRuoliFromOggettoCustomFasc", e);
                    return null;
                }
            }
        }

        //Data il system_id di una tipologia fascicolo restituisce la lista dei suoi attributi
        public static DocsPaVO.ProfilazioneDinamica.Templates getAttributiTipoFasc(DocsPaVO.utente.InfoUtente infoUtente, string idTipoFasc)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelFascDB.getTemplateFascCampiComuniById(infoUtente, idTipoFasc);
                    transactionContext.Complete();
                    return template;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getAttributiTipoFasc", e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Funzione per il reperimento del nome del modello a partire da suo id
        /// </summary>
        /// <param name="modelId">Id del modello</param>
        /// <returns>Nome del modello</returns>
        public static String GetModelNameById(String modelId)
        {
            // Valore da restituire
            String toReturn = String.Empty;

            // Reperimento del nome del modello
            ModelFasc model = new ModelFasc();

            try
            {
                toReturn = model.GetModelNameById(modelId);

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il reperimento del nome del modello fascicolo", e);
            }

            // Restituzione nome del template
            return toReturn;
        }

        public static ArrayList getListaStoricoFascicolo(string id_tipo_fasc, string idProject)
        {
            ArrayList storico = null;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    storico = modelDB.getListaStoricoFascicolo(id_tipo_fasc, idProject);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getListaStoricoFasc", e);
                }
            }
            return storico;
        }

        public static DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] getListTemplatesLite(string idAmministrazione)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    TemplateLite[] result = null;
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    result = modelDB.getListTemplatesLiteFasc(idAmministrazione);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getListTemplatesLite", e);
                    return null;
                }
            }
        }

        public static DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] getListTemplatesLiteByRole(string idAmministrazione,string idRuolo)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    TemplateLite[] result = null;
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    result = modelDB.getListTemplatesLiteFascByRole(idAmministrazione,idRuolo);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getListTemplatesLite", e);
                    return null;
                }
            }
        }

        public static DocsPaVO.ProfilazioneDinamica.Templates getTemplateByDescrizione(string descrizione)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelFascDB.getTemplateFascByDescrizione(descrizione);
                    transactionContext.Complete();
                    return template;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneFascicoli  - metodo: getTemplateByDescrizione", e);
                    return null;
                }
            }
        }

        public static ArrayList getIdPrjByAssTemplates(string idOggFasc, string valoreDB, string ordine)
        {
            DocsPaDB.Query_DocsPAWS.ModelFasc modelFascDB = new DocsPaDB.Query_DocsPAWS.ModelFasc();
            return modelFascDB.getIdPrjByAssTemplates(idOggFasc, valoreDB, ordine);
        }
    }
}
