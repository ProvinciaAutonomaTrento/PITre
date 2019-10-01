using System;
using System.Collections;
using System.Data;
using DocsPaDB.Query_DocsPAWS;
using log4net;
using System.Collections.Generic;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ProfilazioneDinamicaLite;
using System.Linq;

namespace BusinessLogic.ProfilazioneDinamica
{
    public class ProfilazioneDocumenti
    {
        private static ILog logger = LogManager.GetLogger(typeof(ProfilazioneDocumenti));

        public ProfilazioneDocumenti(){}

        public static ArrayList getIdModelliTrasmAssociati(string idTipoDoc, string idDiagramma, string idStato)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    ArrayList idModelliAssociati = modelDB.getIdModelliTrasmAssociati(idTipoDoc,idDiagramma,idStato);
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

        public static ArrayList getTipiOggetto()
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    ArrayList tipiOggetto = modelDB.getTipiOggetto();
                    transactionContext.Complete();
                    return tipiOggetto;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTipiOggetto", e);
                    return null;
                }
            }
        }

        public static ArrayList getTipiDocumento(string idAmministrazione)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    ArrayList tipiDocumento = modelDB.getTipiDocumento(idAmministrazione);
                    transactionContext.Complete();
                    return tipiDocumento;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTipiDocumento", e);
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

        public static DocsPaVO.ProfilazioneDinamica.Templates getTemplateDettagli(string docNumber)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplateDettagli(docNumber);
                    transactionContext.Complete();
                    return template;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTemplateDettagli", e);
                    return null;
                }
            }
        }

        public static void salvaInserimentoUtenteProfDim(DocsPaVO.utente.InfoUtente info, DocsPaVO.ProfilazioneDinamica.Templates template, string docNumber)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    //Verifico se il documento è in libro firma e se è prevista la repertoriazione del documento
                    if (LibroFirma.LibroFirmaManager.IsDocInLibroFirma(docNumber))
                    {
                        bool daRepertoriare = false;
                        if (template != null && !string.IsNullOrEmpty(template.ID_TIPO_ATTO) && template.ELENCO_OGGETTI != null && template.ELENCO_OGGETTI.Count > 0)
                        {
                            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = (from o in template.ELENCO_OGGETTI.Cast<DocsPaVO.ProfilazioneDinamica.OggettoCustom>()
                                                                               where o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") && o.REPERTORIO.Equals("1")
                                                                               && o.CONTATORE_DA_FAR_SCATTARE && string.IsNullOrEmpty(o.VALORE_DATABASE)
                                                                               select o).FirstOrDefault();
                            if (ogg != null && !LibroFirma.LibroFirmaManager.IsTitolarePassoInAttesa(docNumber, info, DocsPaVO.LibroFirma.Azione.DOCUMENTO_REPERTORIATO))
                                throw new Exception("Non è possibile procedere con la repertoriazione poichè il documento è in Libro Firma");
                        }
                    }
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    modelDB.salvaInserimentoUtenteProfDim(template, docNumber);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: salvaInserimentoUtenteProfDim", e);
                }
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

        public static DocsPaVO.ProfilazioneDinamica.Templates getTemplatePerRicerca(string idAmministrazione, string tipoAtto)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplatePerRicerca(idAmministrazione, tipoAtto);
                    transactionContext.Complete();
                    return template;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTemplatePerRicerca", e);
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

        public static void salvaModelli(byte[] dati, string nomeProfilo, string codiceAmministrazione, string nomeFile, string estensione, string serverPath, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    modelDB.salvaModelli(dati, nomeProfilo, codiceAmministrazione, nomeFile, estensione, serverPath, template);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: salvaModelli", e);
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
                    modelDB.eliminaModelli(nomeProfilo, codiceAmministrazione, nomeFile, estensione, serverPath, template);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: eliminaModelli", e);
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

        public static void salvaAssociazioneDocRuoli(ArrayList assDocRuoli)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    modelDB.salvaAssociazioneDocRuoli(assDocRuoli);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: salvaAssociazioneDocRuoli", e);
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

        public static ArrayList getRuoliTipoAtto(string idTipoDoc, string testo, string tipoRicerca)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    ArrayList ruoli = modelDB.getRuoliTipoAtto(idTipoDoc, testo, tipoRicerca);
                    transactionContext.Complete();
                    return ruoli;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getRuoliTipoAtto", e);
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

        public static DocsPaVO.ProfilazioneDinamica.OggettoCustom[] GetValuesDropDownList(string oggettoCustomId, string tipo)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom[] listaOgg = modelDB.GetValuesDropDownList(oggettoCustomId, tipo);
                    transactionContext.Complete();
                    return listaOgg;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: GetValuesDropDownList", e);
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

        public static void SetValuesContatoreDoc(DocsPaVO.ProfilazioneDinamica.Contatore contatore)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    modelDB.SetValuesContatoreDoc(contatore);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: SetValuesContatoreDoc", e);                    
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

        public static void InsertValuesContatoreDoc(DocsPaVO.ProfilazioneDinamica.Contatore contatore)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    modelDB.InsertValuesContatoreDoc(contatore);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: InsertValuesContatoreDoc", e);
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
            Model model = new Model();

            try
            {
                toReturn = model.GetModelNameById(modelId);

            }
            catch (Exception e)
            {
                logger.Debug("Errore durante il reperimento del nome del modello documento", e);
            }

            // Restituzione nome del template
            return toReturn;
        }

        public static ArrayList getListaStoricoAtto(string id_tipo_atto,string doc_number, string idGroup)
        {
            ArrayList storico = null;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    storico = modelDB.getListaStoricoAtto(id_tipo_atto,doc_number,idGroup);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getListaStoricoAtto", e);
                }
            }
            return storico;
        }

        public static DocsPaVO.ProfilazioneDinamica.Templates getTemplateDettagliFilterObjects(string docNumber, string idTipoAtto, string[] visibleFields)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplateDettagliFilterObjects(docNumber, idTipoAtto, visibleFields);
                    transactionContext.Complete();
                    return template;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTemplateDettagliFilterObjects", e);
                    return null;
                }
            }
        }

        public static DocsPaVO.ProfilazioneDinamica.Templates getTemplatePerRicercaById(string idAtto)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    DocsPaVO.ProfilazioneDinamica.Templates template = modelDB.getTemplatePerRicercaById(idAtto);
                    transactionContext.Complete();
                    return template;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getTemplatePerRicerca", e);
                    return null;
                }
            }
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

        /// <summary>
        /// Metodo per la disattivazione dello storico relativo ai campi di una specifica tipologia
        /// </summary>
        /// <param name="request">Informazioni sulla tipologia di cui disattivare la storicizzaizone dei campi</param>
        /// <returns>Esito della richiesta</returns>
        public static DocsPaVO.ProfilazioneDinamica.SelectiveHistoryResponse DeactivateAllHistory(DocsPaVO.ProfilazioneDinamica.SelectiveHistoryRequest request)
        {
            Model profDb = new Model();
            bool result = profDb.DeactivateAllHistory(request.TemplateId);

            return new SelectiveHistoryResponse() { Result = result };

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

        public static DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] getListTemplatesLiteByRole(string idAmministrazione,string idRuolo)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    TemplateLite[] result = null;
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    result = modelDB.getListTemplatesLiteByRole(idAmministrazione,idRuolo);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: getListTemplatesLiteByRole", e);
                    return null;
                }
            }
        }

        public static void AnnullaContatoreDiRepertorio(string idOggetto, string docNumber)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    modelDB.AnnullaContatoreDiRepertorio(idOggetto, docNumber);

                    //Aggiorno Last_edit_date della PROFILE
                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                    documenti.UpdateLastEditDateProfile(docNumber);

                    transactionContext.Complete();                    
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: AnnullaContatoreDiRepertorio", e);                    
                }
            }
        }

        public static void Storicizza(DocsPaVO.ProfilazioneDinamica.Storicizzazione storico)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    modelDB.Storicizza(storico);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: Storicizza", e);
                }
            }
        }

        public static String GetTipologyDescriptionByDocNumber(String docNumber)
        {
            Model model = new Model();
            return model.GetTipologyDescriptionByDocNumber(docNumber);
        }

        public static String GetTipologyDescriptionByIdProfile(String idProfile)
        {
            Model model = new Model();
            return model.GetTipologyDescriptionByIdProfile(idProfile);
        }

        public static string RemoveTipologyDoc(DocsPaVO.utente.InfoUtente info, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            string msg = string.Empty;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    //La rimozione della tipologia è possibile solo se non sono presenti campi contatore oppure questi non sono valorizzati
                    if (schedaDocumento != null && schedaDocumento.template != null && schedaDocumento.template.ELENCO_OGGETTI != null)
                    {
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in schedaDocumento.template.ELENCO_OGGETTI)
                        {
                            if (ogg.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("CONTATORE") || ogg.TIPO.DESCRIZIONE_TIPO.ToUpper().Equals("CONTATORESOTTOCONTATORE"))
                            {
                                if ((!String.IsNullOrEmpty(ogg.VALORE_DATABASE) && !ogg.VALORE_DATABASE.Equals("0")) ||
                                    (!String.IsNullOrEmpty(ogg.VALORE_SOTTOCONTATORE) && !ogg.VALORE_SOTTOCONTATORE.Equals("0"))
                                    )
                                    msg = "Non è possibile eliminare la tipologia perchè contiene dei contatori valorizzati.";
                            }
                        }
                    }

                    if (String.IsNullOrEmpty(msg))
                    {
                        modelDB.RemoveTipologyDoc(info, schedaDocumento);
                        transactionContext.Complete();
                    }
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in ProfilazioneDocumenti  - metodo: RemoveTipologyDoc", e);
                }
            }
            return msg;
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
        public static bool UpdateIsTypeInstance(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione)
        {
            bool result = true;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
                    result = modelDB.UpdateIsTypeInstance(template, idAmministrazione);
                    if(result)
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

        public static ArrayList getIdDocByAssTemplates(string idOggetto, string valoreDB, string ordine, string idTemplate)
        {
            DocsPaDB.Query_DocsPAWS.Model modelDB = new DocsPaDB.Query_DocsPAWS.Model();
            if (string.IsNullOrEmpty(idTemplate))
                return modelDB.getIdDocByAssTemplates(idOggetto, valoreDB, ordine);
            else
                return modelDB.getIdDocByAssTemplates(idOggetto, valoreDB, ordine, idTemplate);
        }

    }
}
