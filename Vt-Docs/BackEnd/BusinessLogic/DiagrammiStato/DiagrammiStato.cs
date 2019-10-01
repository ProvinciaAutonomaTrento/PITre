using System;
using System.Collections;
using System.Data;
using System.Linq;
using log4net;
using System.Collections.Generic;

namespace BusinessLogic.DiagrammiStato
{
	public class DiagrammiStato
	{
        private static ILog logger = LogManager.GetLogger(typeof(DiagrammiStato));
		public DiagrammiStato(){}

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

		public static void delDiagramma(DocsPaVO.DiagrammaStato.DiagrammaStato dg)
		{
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.delDiagramma(dg);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: delDiagramma", e);
                }
            }
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

        public static bool getDocOrFascInStato(string idStato)
		{
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    bool result = diagrammaStatoDB.getDocOrFascInStato(idStato);
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: getDocOrFascInStato", e);
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

		public static bool isModificabile(int systemIdDiagramma)
		{
            
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    bool result = diagrammaStatoDB.isModificabile(systemIdDiagramma);
                   return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: isModificabile", e);
                    return false;
                }
            
		}

		public static DocsPaVO.DiagrammaStato.DiagrammaStato getDgByIdTipoDoc(string systemIdTipoDoc, string idAmm)
		{
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diagrammaStatoDB.getDgByIdTipoDoc(systemIdTipoDoc, idAmm);
                    return diagramma;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: getDgByIdTipoDoc", e);
                    return null;
                }
		}

		public static void salvaModificaStato(string docNumber, string idStato, DocsPaVO.DiagrammaStato.DiagrammaStato diagramma, string idUtente, DocsPaVO.utente.InfoUtente user,string dataScadenza)
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
                        if (!AvviaProcessoFirma(processo, docNumber, user, modalita, note, opzioniNotifiche, out resultAvvioProcesso))
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

        public static bool SalvaModificaStatoAvviaProcesso(string docNumber, string idStato, DocsPaVO.DiagrammaStato.DiagrammaStato diagramma, string idUtente, 
            DocsPaVO.utente.InfoUtente user, string dataScadenza, DocsPaVO.LibroFirma.ProcessoFirma processo, string modalita, 
            string note, DocsPaVO.LibroFirma.OpzioniNotifica opzioniNotifiche, out DocsPaVO.LibroFirma.ResultProcessoFirma resultAvvioProcesso)
        {
            bool result = true;
            resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.OK;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.salvaModificaStato(docNumber, idStato, diagramma, idUtente, user, dataScadenza);

                    DocsPaVO.DiagrammaStato.Stato[] stati = (DocsPaVO.DiagrammaStato.Stato[])diagramma.STATI.ToArray(typeof(DocsPaVO.DiagrammaStato.Stato));

                    DocsPaVO.DiagrammaStato.Stato stato = stati.Where(e => e.SYSTEM_ID.ToString() == idStato).First();
                    if (!AvviaProcessoFirma(processo, docNumber, user, modalita, note, opzioniNotifiche, out resultAvvioProcesso))
                    {
                        result = false;
                        logger.Debug(string.Format("Errore nel passaggio allo stato {0} per il documento con id {1}: non è stato possibile avviare il processo di firma.", stato.DESCRIZIONE, docNumber));
                        throw new Exception(string.Format("Errore nel passaggio allo stato {0} per il documento con id {1}: non è stato possibile avviare il processo di firma.", stato.DESCRIZIONE, docNumber));
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
                    result = false;
                }
            }
            return result;
        }

        public static bool AvviaProcessoFirma(DocsPaVO.LibroFirma.ProcessoFirma processo, string docNumber, DocsPaVO.utente.InfoUtente user,
            string modalita, string note, DocsPaVO.LibroFirma.OpzioniNotifica opzioniNotifiche, out DocsPaVO.LibroFirma.ResultProcessoFirma resultAvvioProcesso)
        {
            bool result = true;
            resultAvvioProcesso = DocsPaVO.LibroFirma.ResultProcessoFirma.OK;
            try
            {
                DocsPaDB.Query_DocsPAWS.LibroFirma librofirma = new DocsPaDB.Query_DocsPAWS.LibroFirma();

                DocsPaVO.documento.Documento[] docs = BusinessLogic.Documenti.DocManager.GetVersionsMainDocument(user, docNumber);
                result = BusinessLogic.LibroFirma.LibroFirmaManager.StartProcessoDiFirma(processo, docs[0], user, modalita, note, opzioniNotifiche, out resultAvvioProcesso, true);
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

		public static bool isStatoAuto(string idStato, string idDiagramma)
		{
            
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    bool result = diagrammaStatoDB.isStatoAuto(idStato, idDiagramma);
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: isStatoAuto", e);
                    return false;
                }
          
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

		public static string getStatoDocStorico(string docNumber)
		{
            
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    string stato = diagrammaStatoDB.getStatoDocStorico(docNumber);
                    return stato;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: getStatoDocStorico", e);
                    return null;
                }
            
		}

		public static DataSet getDiagrammaStoricoDoc(string docNumber)
		{
            
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    DataSet dataSet = diagrammaStatoDB.getDiagrammaStoricoDoc(docNumber);
                   return dataSet;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: getDiagrammaStoricoDoc", e);
                    return null;
                }
            		}

		public static void salvaStoricoTrasmDiagrammi(string idTrasm, string docNumber, string idStato)
		{
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.salvaStoricoTrasmDiagrammi(idTrasm, docNumber, idStato);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: salvaStoricoTrasmDiagrammi", e);
                }
            }
		}

		public static void deleteStoricoTrasmDiagrammi(string docNumber, string idStato)
		{
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.deleteStoricoTrasmDiagrammi(docNumber, idStato);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: deleteStoricoTrasmDiagrammi", e);
                }
            }
		}

		public static DocsPaVO.DiagrammaStato.Stato getStatoSuccessivoAutomatico(string docNumber)
		{
           
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    DocsPaVO.DiagrammaStato.Stato stato = diagrammaStatoDB.getStatoSuccessivoAutomatico(docNumber);
                   return stato;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: getStatoSuccessivoAutomatico", e);
                    return null;
                }
         
		}

		public static bool isUltimaDaAccettare(string idTrasmissione)
		{
           
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    bool result = diagrammaStatoDB.isUltimaDaAccettare(idTrasmissione);
                   return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: isUltimaDaAccettare", e);
                    return false;
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

        public static bool associaTipoFascDiagramma(string idTipoFasc, string idDiagramma)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    bool result = diagrammaStatoDB.associaTipoFascDiagramma(idTipoFasc, idDiagramma);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: associaTipoFascDiagramma", e);
                    return false;
                }
            }
        }

        public static bool isFascicoliInStatoFinale(string idDiagramma, string idTemplate)
        {
           
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    bool result = diagrammaStatoDB.isFascicoliInStatoFinale(idDiagramma, idTemplate);
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: isFascicoliInStatoFinale", e);
                    return false;
                }
            
        }

        public static void disassociaTipoFascDiagramma(string idTipoFasc)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.disassociaTipoFascDiagramma(idTipoFasc);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: disassociaTipoFascDiagramma", e);
                }
            }
        }

        public static DocsPaVO.DiagrammaStato.DiagrammaStato getDgByIdTipoFasc(string systemIdTipoFasc, string idAmm)
        {
            try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = diagrammaStatoDB.getDgByIdTipoFasc(systemIdTipoFasc, idAmm);
                    return diagramma;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: getDgByIdTipoFasc", e);
                    return null;
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

        public static ArrayList isStatoTrasmAutoFasc(string idAmm, string idStato, string idTipoFasc)
        {
            
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    ArrayList modelliTrasm = diagrammaStatoDB.isStatoTrasmAutoFasc(idAmm, idStato, idTipoFasc);
                    return modelliTrasm;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: isStatoTrasmAutoFasc", e);
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

        public static DocsPaVO.DiagrammaStato.Stato getStatoFasc(string idProject)
        {
          
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    DocsPaVO.DiagrammaStato.Stato stato = diagrammaStatoDB.getStatoFasc(idProject);
                 
                   return stato;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: getStatoFasc", e);
                    return null;
                }
         
        }

        public static void salvaDataScadenzaFasc(string idProject, string dataScadenza, string idTipoFasc)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.salvaDataScadenzaFasc(idProject, dataScadenza, idTipoFasc);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: salvaDataScadenzaFasc", e);
                }
            }
        }

        public static void salvaDataScadenzaDoc(string docNumber, string dataScadenza, string idTipoAtto)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.salvaDataScadenzaDoc(docNumber, dataScadenza, idTipoAtto);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: salvaDataScadenzaDoc", e);
                }
            }
        }

        public static DataSet getDiagrammaStoricoFasc(string idProject)
        {
            
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    DataSet dataSet = diagrammaStatoDB.getDiagrammaStoricoFasc(idProject);
                    return dataSet;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: getDiagrammaStoricoFasc", e);
                    return null;
                }
           
        }

        public static void deleteStoricoTrasmDiagrammiFasc(string idProject, string idStato)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    diagrammaStatoDB.deleteStoricoTrasmDiagrammiFasc(idProject, idStato);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: deleteStoricoTrasmDiagrammiFasc", e);
                }
            }
        }

        public static DocsPaVO.DiagrammaStato.Stato getStatoSuccessivoAutomaticoFasc(string idProject)
        {
           
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    DocsPaVO.DiagrammaStato.Stato stato = diagrammaStatoDB.getStatoSuccessivoAutomaticoFasc(idProject);
                   return stato;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in DiagrammiStato  - metodo: getStatoSuccessivoAutomaticoFasc", e);
                    return null;
                }
          
        }

        public static ArrayList getStatiPerRicerca(string idDiagramma, string docOrFasc)
        {

            try
            {
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                return diagrammaStatoDB.getStatiPerRicerca(idDiagramma, docOrFasc);                
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: getStatiPerRicerca", e);
                return null;
            }

        }

        /// <summary>
        /// Restituisce la lista delle trasmissioni salvate da inviare in automatico al cambio di stato
        /// </summary>
        /// <param name="idProject"></param>
        /// <param name="idStato"></param>
        /// <param name="user"></param>
        /// <param name="infoFascicolo"></param>
        /// <returns></returns>
        public static List<DocsPaVO.trasmissione.Trasmissione> GetTrasmCambioStato(string idProject, string idStato, DocsPaVO.utente.InfoUtente user, DocsPaVO.fascicolazione.InfoFascicolo infoFascicolo)
        {
            logger.Debug("BEGIN");
            List<DocsPaVO.trasmissione.Trasmissione> listaTrasm = new List<DocsPaVO.trasmissione.Trasmissione>();

            try
            {
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                ArrayList listaID = diagrammaStatoDB.GetTrasmissioniStatoFasc(idProject, idStato);

                if (listaID.Count > 0)
                {
                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtenteById(user.idPeople);
                    DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloById(user.idCorrGlobali);

                    DocsPaVO.trasmissione.OggettoTrasm objTrasm = new DocsPaVO.trasmissione.OggettoTrasm();
                    objTrasm.infoFascicolo = infoFascicolo;

                    foreach (string idTrasm in listaID)
                    {
                        listaTrasm.Add(BusinessLogic.Trasmissioni.QueryTrasmManager.GetTrasmissioneById(objTrasm, utente, ruolo, idTrasm));
                    }
                }

            }
            catch (Exception ex)
            {
                logger.DebugFormat("{0}\r\n{1}", ex.Message, ex.StackTrace);
                return null;
            }

            logger.Debug("END");
            return listaTrasm;
        }

        public static ArrayList ChangeStateGetMissingRoles(string idProject, string idStato, DocsPaVO.utente.InfoUtente user, DocsPaVO.fascicolazione.InfoFascicolo infoFascicolo)
        {
            logger.Debug("BEGIN");
            ArrayList missingRoles = new ArrayList();
            string missingRolesField = string.Empty;
            bool updateTemplate = false;
            try
            {
                DocsPaVO.utente.Corrispondente tempCorrObj;

                // Lista di ragioni associate al cambio stato
                List<string> listaRagioni = GetRagioniCambioStato(idStato);

                if (listaRagioni != null && listaRagioni.Count > 0)
                {
                    bool isReview = listaRagioni.Where(r => r.ToUpper() != "AUTHORIZER" && r.ToUpper() != "PROPOSER").Count() > 0;

                    // 1 - Estraggo i ruoli mancanti dal campo profilato del fascicolo
                    logger.Debug("Analisi ruoli mancanti da campo profilato");
                    DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascDettagli(idProject);
                    if (template != null)
                    {
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI)
                        {
                            if (ogg.DESCRIZIONE.ToUpper() == DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(user.idAmministrazione, "BE_CAP_MISSING_ROLES_FIELD"))
                            {
                                if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                                {
                                    missingRolesField = ogg.VALORE_DATABASE;
                                    string[] roleReason = ogg.VALORE_DATABASE.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                                    // Seleziono le ragioni da inviare per questo cambio stato
                                    for (int i = 0; i < roleReason.Length; i++)
                                    {
                                        if (!isReview)
                                        {
                                            foreach (string r in listaRagioni)
                                            {
                                                if (roleReason[i].ToUpper().Contains(r.ToUpper()))
                                                {
                                                    missingRoles.Add(roleReason[i]);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!(roleReason[i].ToUpper().Contains("AUTHORIZER") || roleReason[i].ToUpper().Contains("PROPOSER")))
                                            {
                                                missingRoles.Add(roleReason[i]);
                                            }
                                        }
                                    }
                                    continue;
                                }
                            }
                        }
                    }

                    // 2 - Verifico se qualcuno di questi ruoli è stato inserito
                    if (missingRoles.Count > 0)
                    {
                        ArrayList updatedRoles = new ArrayList();

                        //foreach (string r in missingRoles)
                        for (int i = missingRoles.Count - 1; i >= 0; i-- )
                        {
                            string r = missingRoles[i].ToString();
                            string corr = r.Split('(')[0].Trim();
                            tempCorrObj = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(corr, user);
                            if (tempCorrObj != null)
                            {
                                // Il ruolo è stato aggiunto: lo rimuovo dalla lista dei ruoli mancanti...
                                missingRoles.Remove(r);

                                //...e lo aggiungo a quella dei ruoli inseriti
                                updatedRoles.Add(r);

                                // Aggiorno il campo del template
                                missingRolesField = missingRolesField.Replace(r, string.Empty);
                                updateTemplate = true;
                            }
                        }

                        if (updatedRoles.Count > 0)
                        {
                            // Se alcuni ruoli sono stati inseriti devo creare le trasmissioni
                            CreateTrasmission(updatedRoles, infoFascicolo, user);
                        }
                    }

                    // 3 - Analizzo le trasmissioni salvate e non inviate e verifico i ruoli
                    List<DocsPaVO.trasmissione.Trasmissione> listaTrasm = DiagrammiStato.GetTrasmCambioStato(idProject, idStato, user, infoFascicolo);
                    if (listaTrasm.Count > 0)
                    {
                        foreach (DocsPaVO.trasmissione.Trasmissione trasm in listaTrasm)
                        {
                            bool toUpdate = false;

                            //foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in trasm.trasmissioniSingole)
                            for(int i = trasm.trasmissioniSingole.Count - 1; i >=0; i--)
                            {
                                DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)trasm.trasmissioniSingole[i];

                                // Controllo se il ruolo esiste
                                tempCorrObj = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(ts.corrispondenteInterno.codiceRubrica, user);
                                if (tempCorrObj != null)
                                {
                                    // serve fare qualcosa?
                                }
                                else
                                {
                                    // Ruolo non trovato - aggiungo alla lista dei missing roles
                                    string corrString = string.Empty;

                                    switch (ts.ragione.descrizione.ToUpper())
                                    {
                                        case "AUTHORIZER":
                                            corrString = string.Format("{0} (Authorizer)", ts.corrispondenteInterno.descrizione);
                                            break;
                                        case "PROPOSER":
                                            corrString = string.Format("{0} (Proposer)", ts.corrispondenteInterno.descrizione);
                                            break;
                                        case "REVIEW":
                                            corrString = string.Format("{0} ({0})", ts.corrispondenteInterno.descrizione);
                                            break;
                                    }

                                    missingRoles.Add(ts.corrispondenteInterno.descrizione);
                                    missingRolesField = missingRolesField + Environment.NewLine + corrString;
                                    updateTemplate = true;

                                    // devo cancellare la trasmissione singola
                                    trasm.trasmissioniSingole.RemoveAt(i);
                                    toUpdate = true;
                                }
                            }

                            if (toUpdate)
                            {
                                trasm.daAggiornare = true;
                                DocsPaVO.trasmissione.Trasmissione resultTrasm = BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasm);
                            }
                        }
                    }

                    // Se sono stati rimossi/aggiunti ruoli mancanti aggiorno il campo profilato
                    if (updateTemplate)
                    {
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI)
                        {
                            if (ogg.DESCRIZIONE.ToUpper() == DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(user.idAmministrazione, "BE_CAP_MISSING_ROLES_FIELD"))
                            {
                                ogg.VALORE_DATABASE = missingRolesField;
                            }
                        }

                        BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.salvaInserimentoUtenteProfDimFasc(template, idProject);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.DebugFormat("{0}\r\n{1}");
                return null;
            }

            logger.Debug("END");
            return missingRoles;
        }

        public static ArrayList CreateTrasmission(ArrayList corrList, DocsPaVO.fascicolazione.InfoFascicolo infoFascicolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            ArrayList result = new ArrayList();

            DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
            DocsPaVO.trasmissione.Trasmissione resultTrasm = null;

            DocsPaVO.trasmissione.RagioneTrasmissione ragioneAuth = null;
            DocsPaVO.trasmissione.RagioneTrasmissione ragioneProp = null;
            DocsPaVO.trasmissione.RagioneTrasmissione ragioneRev = null;

            // Costruzione oggetto trasmissione
            transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
            transmission.infoFascicolo = infoFascicolo;
            transmission.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);
            transmission.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);
            transmission.NO_NOTIFY = "0";

            DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascDettagli(infoFascicolo.idFascicolo);

            foreach (string roleReason in corrList)
            {
                try
                {
                    string codRole = roleReason.Split('(')[0].Trim();
                    string ragTrasm = roleReason.Split('(')[1].Trim();
                    if (ragTrasm.EndsWith(")"))
                        ragTrasm = ragTrasm.Substring(0, ragTrasm.Length - 1).Trim();

                    // Estrazione corrispondente
                    DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(codRole, infoUtente);

                    // Selezione ragione e creazione trasmissione singola
                    if (ragTrasm.ToUpper() == "AUTHORIZER")
                    {
                        if (ragioneAuth == null)
                        {
                            ragioneAuth = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, "AUTHORIZER");
                        }
                        transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(transmission, corr, ragioneAuth, string.Empty, "S");
                        result.Add(roleReason);
                    }
                    else if (ragTrasm.ToUpper() == "PROPOSER")
                    {
                        if (ragioneProp == null)
                        {
                            ragioneProp = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, "PROPOSER");
                        }
                        transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(transmission, corr, ragioneProp, string.Empty, "S");
                        result.Add(roleReason);
                    }
                    else
                    {
                        // Review
                        string nuovaRagione = ragTrasm.Split('§')[0];
                        ragioneRev = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUtente.idAmministrazione, nuovaRagione);
                        transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(transmission, corr, ragioneRev, string.Empty, "S");
                        result.Add(roleReason);
                    }
                    // Inserisco il ruolo nel campo corrispondente
                    CAPServicesInsertRoleInProfField(codRole, ragTrasm, template, infoUtente);
                }
                catch (Exception ex)
                {
                    logger.DebugFormat("Errore nella creazione della trasmissione per {0}.\r\n{1}\r\n{2}\r\n", roleReason, ex.Message, ex.StackTrace);
                }
            }

            BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.salvaInserimentoUtenteProfDimFasc(template, infoFascicolo.idFascicolo);

            if (transmission.trasmissioniSingole != null && transmission.trasmissioniSingole.Count > 0)
            {
                resultTrasm = BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(transmission);
            }

            if (resultTrasm != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static void CreateTransmissionsForMissingRoles(string[] missingRoles, DocsPaVO.utente.InfoUtente infoUt, DocsPaVO.fascicolazione.InfoFascicolo infoFascicolo, string idStato)
        {
            logger.Debug("BEGIN");
            try
            {
                ArrayList authorizers = new ArrayList();
                ArrayList proposers = new ArrayList();
                ArrayList reviewers = new ArrayList();

                // Utente mittente della trasmissione: è il creatore del fascicolo
                DocsPaVO.fascicolazione.Fascicolo fasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(infoFascicolo.idFascicolo, infoUt);

                DocsPaVO.utente.Utente u = Utenti.UserManager.getUtenteById(fasc.creatoreFascicolo.idPeople);
                DocsPaVO.utente.Ruolo r = Utenti.UserManager.getRuoloById(fasc.creatoreFascicolo.idCorrGlob_Ruolo);

                DocsPaVO.utente.InfoUtente infoUtente = Utenti.UserManager.GetInfoUtente(u, r);
                ArrayList list = new ArrayList(missingRoles);

                // Creazione delle trasmissioni
                ArrayList roles = CreateTrasmission(list, infoFascicolo, infoUtente);

                if (roles != null && roles.Count > 0)
                {
                    // Invio delle trasmissioni salvate
                    SendTransmissions(idStato, infoFascicolo, infoUtente);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            logger.Debug("END");
        }

        public static void SendTransmissions(string idStato, DocsPaVO.fascicolazione.InfoFascicolo infoFascicolo, DocsPaVO.utente.InfoUtente user)
        {
            // Estrazione delle trasmissioni associate al cambio stato
            List<DocsPaVO.trasmissione.Trasmissione> listaTrasmissioni = GetTrasmCambioStato(infoFascicolo.idFascicolo, idStato, user, infoFascicolo);

            if (listaTrasmissioni != null && listaTrasmissioni.Count > 0)
            {
                DocsPaVO.trasmissione.Trasmissione resultTrasm = null;

                foreach (DocsPaVO.trasmissione.Trasmissione trasm in listaTrasmissioni)
                {
                    resultTrasm = BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(string.Empty, trasm);
                    if (resultTrasm != null && resultTrasm.infoFascicolo != null && !string.IsNullOrEmpty(resultTrasm.infoFascicolo.idFascicolo))
                    {
                        foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in resultTrasm.trasmissioniSingole)
                        {
                            string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                            string desc = "Trasmesso Fascicolo: " + resultTrasm.infoFascicolo.codice;
                            BusinessLogic.UserLog.UserLog.WriteLog(resultTrasm.utente.userId, resultTrasm.utente.idPeople, resultTrasm.ruolo.idGruppo, user.idAmministrazione, method, resultTrasm.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                                (user != null && user.delegato != null ? user.delegato : null), "1", single.systemId);
                        }
                    }
                }
            }         
        }

        public static List<string> GetRagioniCambioStato(string idStato)
        {
            DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrammaStatoDB = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
            return diagrammaStatoDB.GetRagioniCambioStato(idStato);
        }

        private static void CAPServicesInsertRoleInProfField(string role, string reason, DocsPaVO.ProfilazioneDinamica.Templates template, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.ProfilazioneDinamica.OggettoCustom field = null;
            
            if (reason.ToUpper() == "AUTHORIZER")
            {
                field = (from DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI where ogg.DESCRIZIONE.ToUpper() == DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_CAP_AUTHORIZER_FIELD") select ogg).FirstOrDefault();
                if (!field.VALORE_DATABASE.EndsWith(Environment.NewLine))
                {
                    field.VALORE_DATABASE = field.VALORE_DATABASE + Environment.NewLine;
                }
                field.VALORE_DATABASE = field.VALORE_DATABASE + role;
            }
            else if (reason.ToUpper() == "PROPOSER")
            {
                field = (from DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI where ogg.DESCRIZIONE.ToUpper() == DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_CAP_PROPOSERS_FIELD") select ogg).FirstOrDefault();
                if (!field.VALORE_DATABASE.EndsWith(Environment.NewLine))
                {
                    field.VALORE_DATABASE = field.VALORE_DATABASE + Environment.NewLine;
                }
                field.VALORE_DATABASE = field.VALORE_DATABASE + role;
            }
            else
            {
                // REVIEWS - Ho bisogno del codice della vecchia ragione
                string newReason = string.Empty;
                string oldReason = string.Empty;
                if (reason.IndexOf('§') > 0)
                {
                    newReason = reason.Split('§')[0];
                    oldReason = reason.Split('§')[1];
                }
                else
                {
                    newReason = reason;
                    oldReason = reason;
                }

                field = (from DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI where ogg.DESCRIZIONE.ToUpper() == DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_CAP_REVIEWS_FIELD") select ogg).FirstOrDefault();
                if (!field.VALORE_DATABASE.EndsWith(Environment.NewLine))
                {
                    field.VALORE_DATABASE = field.VALORE_DATABASE + Environment.NewLine;
                }
                field.VALORE_DATABASE = field.VALORE_DATABASE + string.Format("{0} ({1})", role, oldReason);
            }
        }

        #region Visibilità ruolo /stati diagramma
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

        public static bool IsAssociatoRuoloDiagramma(string idDiagramma, string idRuolo)
        {

            bool res = false;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrStato = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    res = diagrStato.IsAssociatoRuoloDiagramma(idDiagramma, idRuolo);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in BusinessLogic.DiagrammiStato.DiagrammiStato - Method: IsAssociatoRuoloDiagramma", e);
                    return false;
                }
                return res;
            }
        }

        public static bool IsAssociatoRuoloStatoDia(string idDiagramma, string idRuolo, string idStato)
        {
            bool res = false;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrStato = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                    res = diagrStato.IsAssociatoRuoloStatoDia(idDiagramma, idRuolo, idStato);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in BusinessLogic.DiagrammiStato.DiagrammiStato - Method: IsAssociatoRuoloStatoDia", e);
                    return false;
                }
                return res;
            }
        }

        public static List<DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma> GetFasiStatiDiagramma(string idDiagramma, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrStato = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                return diagrStato.GetFasiStatiDiagramma(idDiagramma, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetFasiStatiDiagramma " + e.Message);
                return null;
            }
        }

        public static List<DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma> GetFaseDiagrammaByIdFase(string idDiagramma, string idFase, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrStato = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                return diagrStato.GetFaseDiagrammaByIdFase(idDiagramma, idFase, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetFaseDiagrammaByIdFase " + e.Message);
                return null;
            }
        }

        public static DocsPaVO.DiagrammaStato.Stato GetStatoById(string idStato, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.DiagrammiStato diagrStato = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
                return diagrStato.GetStatoById(idStato, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetStatoById " + e.Message);
                return null;
            }
        }
        #endregion

        public static ArrayList GetIdDocsByDiagramStatus(string descStato, string descDiagramma, string codAmm)
        {
            DocsPaDB.Query_DocsPAWS.DiagrammiStato dBds = new DocsPaDB.Query_DocsPAWS.DiagrammiStato();
            return dBds.GetIdDocsByDiagramStatus(descStato, descDiagramma, codAmm);
        }

    }
}
