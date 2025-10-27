// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using BusinessLogic.RubricaComune;
using Serilog;
using System.Collections;

namespace BusinessLogic.DiagrammiStato
{
    public class DiagrammiStato
    {
        private static ILogger logger = Log.ForContext(typeof(DiagrammiStato));

        public static DocsPaVO.DiagrammaStato.DiagrammaStato getDiagrammaById(string idDiagramma)
        {

            try
            {
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diagrammaStatoDB.getDiagrammaById(idDiagramma);
                return diagramma;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: getDiagrammaById", e);
                return null;
            }
        }

        public static int getDiagrammaAssociatoFasc(string idTipoFasc)
        {

            try
            {
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                int diagrammaAssociato = diagrammaStatoDB.getDiagrammaAssociatoFasc(idTipoFasc);
                return diagrammaAssociato;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: getDiagrammaAssociatoFasc", e);
                return 0;
            }

        }

        public static void salvaModificaStatoFasc(string idProject, string idStato, DocsPaVO.DiagrammaStato.DiagrammaStato diagramma, string idUtente, DocsPaVO.utente.InfoUtente user, string dataScadenza)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.salvaModificaStatoFasc(idProject, idStato, diagramma, idUtente, user, dataScadenza);
                    // Se si tratta di un procedimento è necessario tracciare la data di cambio stato
                    if (DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_ENABLE_PORTALE_PROCEDIMENTI").Equals("1") && Procedimenti.ProcedimentiManager.IsProcedimento(idProject))
                    {
                        if (!Procedimenti.ProcedimentiManager.InsertFaseProcedimento(idProject, idStato))
                        {
                            throw new Exception(" Errore nell'associazione fase-procedimento ");
                        }
                    }

                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: salvaModificaStatoFasc", e);
                }
            }
        }

        public static void salvaModificaStato(string docNumber, string idStato, DocsPaVO.DiagrammaStato.DiagrammaStato diagramma, string idUtente, DocsPaVO.utente.InfoUtente user, string dataScadenza, IBaseRubricaComuneService rubricaComuneService)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.salvaModificaStato(docNumber, idStato, diagramma, idUtente, user, dataScadenza);

                    DocsPaVO.DiagrammaStato.Stato[] stati = (DocsPaVO.DiagrammaStato.Stato[])diagramma.STATI.ToArray(typeof(DocsPaVO.DiagrammaStato.Stato));

                    DocsPaVO.DiagrammaStato.Stato stato = stati.Where(e => e.SYSTEM_ID.ToString() == idStato).First();

                    DocsPaDB.Query_DocsPAWS.LibroFirma librofirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();
                    //Avvio il processo Automatico di firma
                    if (!string.IsNullOrEmpty(stato.ID_PROCESSO_FIRMA) && !librofirma.IsModelloDiFirma(stato.ID_PROCESSO_FIRMA))
                    {
                        DocsPaVO.LibroFirma.ProcessoFirma processo = librofirma.GetProcessoDiFirmaById(stato.ID_PROCESSO_FIRMA, user);
                        string modalita = "A";
                        string note = string.Empty;
                        DocsPaVO.LibroFirma.OpzioniNotifica opzioniNotifiche = new DocsPaVO.LibroFirma.OpzioniNotifica();
                        opzioniNotifiche.Notifica_interrotto = true;
                        opzioniNotifiche.Notifica_concluso = false;
                        DocsPaVO.LibroFirma.ResultProcessoFirma resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.OK;
                        if (!AvviaProcessoFirma(processo, docNumber, user, modalita, note, opzioniNotifiche, rubricaComuneService, out resultAvvioProcesso))
                        {
                            logger.Debug(string.Format("Errore nel passaggio allo stato {0} per il documento con id {1}: non è stato possibile avviare il processo di firma.", stato.DESCRIZIONE, docNumber));
                            throw new Exception(string.Format("Errore nel passaggio allo stato {0} per il documento con id {1}: non è stato possibile avviare il processo di firma.", stato.DESCRIZIONE, docNumber));
                        }

                    }
                    if (BusinessLogic.Documenti.DocumentConsolidation.IsConfigEnabled())
                    {
                        if (stato.STATO_CONSOLIDAMENTO > DocsPaVO.documento.DocumentConsolidationStateEnum.None)
                        {
                            DocsPaVO.documento.DocumentConsolidationStateInfo actualState = BusinessLogic.Documenti.DocumentConsolidation.GetState(user, docNumber);

                            if (actualState.State >= stato.STATO_CONSOLIDAMENTO)
                            {
                                logger.Debug(string.Format("Il documento con id {0} non può essere consolidato allo stato '{1}' in quanto risulta già consolidato in stato '{2}': documento consolidato allo stato {2}",
                                        stato.DESCRIZIONE,
                                        docNumber,
                                        DocsPaVO.documento.DocumentConsolidationStateDescriptionAttribute.GetDescription(stato.STATO_CONSOLIDAMENTO),
                                        DocsPaVO.documento.DocumentConsolidationStateDescriptionAttribute.GetDescription(actualState.State)));
                            }
                            else
                            {
                                DocsPaVO.documento.DocumentConsolidationStateInfo info = BusinessLogic.Documenti.DocumentConsolidation.Consolidate(user, docNumber, stato.STATO_CONSOLIDAMENTO, true);

                                logger.Debug(string.Format("Passaggio allo stato {0} per il documento con id {1}: documento consolidato allo stato {2}", stato.DESCRIZIONE, docNumber, DocsPaVO.documento.DocumentConsolidationStateDescriptionAttribute.GetDescription(stato.STATO_CONSOLIDAMENTO)));
                            }
                        }
                    }
                    else
                    {
                        logger.Debug(string.Format("Errore nel passaggio allo stato {0} per il documento con id {1}: l'utente non dispone dei diritti sufficenti per consolidare il documento", stato.DESCRIZIONE, docNumber));
                    }

                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: salvaModificaStato", e);
                }
            }
        }

        public static bool AvviaProcessoFirma(DocsPaVO.LibroFirma.ProcessoFirma processo, string docNumber, DocsPaVO.utente.InfoUtente user,
            string modalita, string note, DocsPaVO.LibroFirma.OpzioniNotifica opzioniNotifiche, IBaseRubricaComuneService rubricaComuneService, out DocsPaVO.LibroFirma.ResultProcessoFirma resultAvvioProcesso)
        {
            bool result = true;
            resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.OK;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma librofirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();

                DocsPaVO.documento.Documento[] docs = BusinessLogic.Documenti.DocManager.GetVersionsMainDocument(user, docNumber);
                result = BusinessLogic.LibroFirma.LibroFirmaManager.StartProcessoDiFirma(processo, docs[0], user, modalita, note, opzioniNotifiche, rubricaComuneService, out resultAvvioProcesso, true);
                if (result)
                {
                    string method = "AVVIATO_PROCESSO_DI_FIRMA_DOCUMENTO";
                    string description = "Avviato processo di firma per la versione " + docs[0].version;
                    BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, user.idGruppo, user.idAmministrazione, method, docs[0].docNumber,
                        description, DocsPaVO.Logger.CodAzione.Esito.OK, (user != null && user.delegato != null ? user.delegato : null), "1");
                }
                else
                {
                    logger.Debug(string.Format("Errore nel passaggio di stato. Impossibile avviare il processo di firma per il documento con id {0}: {1} ", docNumber, resultAvvioProcesso.ToString()));
                    throw new Exception((string.Format("Errore nel passaggio di stato. Impossibile avviare il processo di firma per il documento con id {0}: {1} ", docNumber, resultAvvioProcesso.ToString())));
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nell'avvio del processo di firma " + ex.Message);
                result = false;
            }

            return result;
        }

        public static DocsPaVO.DiagrammaStato.Stato getStatoDoc(string docNumber)
        {

            try
            {
                DocsPaVO.DiagrammaStato.Stato stato = null;
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                if (!string.IsNullOrEmpty(docNumber))
                {
                    stato = diagrammaStatoDB.getStatoDoc(docNumber);
                }
                return stato;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: getStatoDoc", e);
                return null;
            }

        }

        public static ArrayList isStatoTrasmAuto(string idAmm, string idStato, string idTemplate)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                ArrayList modelliTrasm = diagrammaStatoDB.isStatoTrasmAuto(idAmm, idStato, idTemplate);
                return modelliTrasm;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: isStatoTrasmAuto", e);
                return null;
            }

        }

        public static void salvaStoricoTrasmDiagrammiFasc(string idTrasm, string idProject, string idStato)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.salvaStoricoTrasmDiagrammiFasc(idTrasm, idProject, idStato);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: salvaStoricoTrasmDiagrammiFasc", e);
                }
            }
        }

        public static bool associaTipoDocDiagramma(string idTipoDoc, string idDiagramma)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    bool result = diagrammaStatoDB.associaTipoDocDiagramma(idTipoDoc, idDiagramma);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: associaTipoDocDiagramma", e);
                    return false;
                }
            }
        }

        public static int getDiagrammaAssociato(string idTipoDoc)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                int diagrammaAssociato = diagrammaStatoDB.getDiagrammaAssociato(idTipoDoc);
                return diagrammaAssociato;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: getDiagrammaAssociato", e);
                return 0;
            }
        }

        public static bool isDocumentiInStatoFinale(string idDiagramma, string idTemplate)
        {

            try
            {
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                bool result = diagrammaStatoDB.isDocumentiInStatoFinale(idDiagramma, idTemplate);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: isDocumentiInStatoFinale", e);
                return false;
            }

        }

        public static void disassociaTipoDocDiagramma(string idTipoDoc)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.disassociaTipoDocDiagramma(idTipoDoc);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: disassociaTipoDocDiagramma", e);
                }
            }
        }

        public static ArrayList getDiagrammi(string idAmm)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                ArrayList diagrammi = diagrammaStatoDB.getDiagrammi(idAmm);
                return diagrammi;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: getDiagramma", e);
                return null;
            }
        }

        public static bool isUniqueNameDiagramma(string nomeDiagramma)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                bool result = diagrammaStatoDB.isUniqueNameDiagramma(nomeDiagramma);
                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: isUniqueNameDiagramma", e);
                return false;
            }
        }

        public static void salvaDiagramma(DocsPaVO.DiagrammaStato.DiagrammaStato dg, string idAmm)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.salvaDiagramma(dg, idAmm);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: salvaDiagramma", e);
                }
            }
        }

        public static List<DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma> getRuoliStatiDiagramma(int idDiagramma)
        {
            List<DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma> assRuoliStatiDiagramma = new List<DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma>();
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrStato = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    assRuoliStatiDiagramma = diagrStato.GetRuoliStatiDiagramma(idDiagramma);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in BusinessLogic.DiagrammiStato.DiagrammiStato - Method: getRuoliStatiDiagramma", e);
                }
            }
            return assRuoliStatiDiagramma;
        }

        public static bool ModifyRuoloStatiDiagramma(List<DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma> listAssRoleStatiDia)
        {
            bool res = false;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrStato = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    res = diagrStato.ModifyAssRuoloStatiDiagramma(listAssRoleStatiDia);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in BusinessLogic.DiagrammiStato.DiagrammiStato - Method: ModifyRuoloStatiDiagramma", e);
                    return false;
                }
            }
            return true;
        }

        public static void updateDiagramma(DocsPaVO.DiagrammaStato.DiagrammaStato dg)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.updateDiagramma(dg);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: updateDiagramma", e);
                }
            }
        }
    }
}
