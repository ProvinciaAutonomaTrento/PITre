using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using DocsPaVO.ricerche;
using log4net;
using DocsPaVO.documento;

namespace BusinessLogic.Documenti
{
    public class areaConservazioneManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(areaConservazioneManager));

        /// <summary>
        /// Creazione istanza di conservazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="descrizione"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public static string CreateIstanzaConservazione(DocsPaVO.utente.InfoUtente infoUtente, string descrizione, string note, string idPolicy, string stato, bool consolidamento,string tipoIstanza)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione conservazioneDb = new DocsPaDB.Query_DocsPAWS.Conservazione();

            return conservazioneDb.CreateAreaConservazione(infoUtente, descrizione, note, idPolicy, stato, consolidamento,tipoIstanza);
        }

        /// <summary>
        /// Rimozione di un'istanza di conservazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static void DeleteIstanzaConservazione(DocsPaVO.utente.InfoUtente infoUtente, string id)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione conservazioneDb = new DocsPaDB.Query_DocsPAWS.Conservazione();

            conservazioneDb.DeleteIstanzaConservazione(infoUtente, id);
        }

        /// <summary>
        /// Creazione di una nuova istanza di conservazione e inserimento di un documento nell'istanza stessa
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="idPolicy"></param>
        /// <returns></returns>
        public static string CreateAndAddDocInAreaConservazione(
                        DocsPaVO.utente.InfoUtente infoUtente,
                        string idProfile,
                        string idProject,
                        string docNumber,
                        string tipoOggetto,
                        string idPolicy)
        {
            string result = String.Empty;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
                logger.DebugFormat("ADDAREACONSERVAZIONE START");
                result = doc.addAreaConservazione(string.Empty, idProfile, idProject, docNumber, infoUtente, tipoOggetto, idPolicy);
                logger.DebugFormat("ADDAREACONSERVAZIONE END -> RESULT {0}", result);
                if (result == "-1")
                {
                    logger.Debug("Errore nella generazione dell'area conservazione (CreateAndAddDocInAreaConservazione)");
                    throw new Exception();
                }

                transactionContext.Complete();

                return result;
            }
        }

        /// <summary>
        /// Inserimento di un documento in un'istanza di conservazione già esistente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="tipoOggetto"></param>
        /// <returns></returns>
        public static string AddDocInAreaConservazione(
                                    DocsPaVO.utente.InfoUtente infoUtente,
                                    string idConservazione,
                                    string idProfile,
                                    string idProject,
                                    string docNumber,
                                    string tipoOggetto)
        {
            string result = String.Empty;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
                result = doc.addAreaConservazione(idConservazione, idProfile, idProject, docNumber, infoUtente, tipoOggetto, null);
                if (result == "-1")
                {
                    logger.Debug("Errore nella generazione dell'area conservazione (CreateAndAddDocInAreaConservazione)");
                    throw new Exception();
                }

                transactionContext.Complete();
                return result;
            }
        }

        public static void updateAreaConservazione(string sysId, string tipo_cons, string note, string descr, string idTipoSupp, bool consolida)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
            bool result = true;
            result = doc.UpdateStatoAreaCons(sysId, tipo_cons, note, descr, idTipoSupp, consolida);
            if (!result)
                throw new Exception();
        }

        public static void updateSizeItemCons(string sysId, int size)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
            bool result = true;
            result = doc.insertSizeInItemsCons(size, sysId);
            if (!result)
                throw new Exception();
        }

        public static void cancellaAreaConservazione(string idProfile, DocsPaVO.fascicolazione.Fascicolo fasc, string idIstanza, bool deleteIstanza, string systemId)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
            bool result = true;
            if (fasc != null)
            {
                if (idProfile == string.Empty)
                {
                    result = doc.DeleteAreaConservazione(fasc.systemID, null, idIstanza, deleteIstanza, systemId);
                }
                else
                {
                    result = doc.DeleteAreaConservazione(fasc.systemID, idProfile, idIstanza, deleteIstanza, systemId);
                }
            }
            else
            {
                result = doc.DeleteAreaConservazione(null, idProfile, idIstanza, deleteIstanza, systemId);
            }
            if (!result)
                throw new Exception();
        }
        public static int canDeleteItemConservazione(string idProfile, string idPeople, string idGruppo)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
            int result = 0;
            result = doc.CanDeleteFromConservazione(idProfile, idPeople, idGruppo);
            if (result == -1)
                throw new Exception();
            return result;
        }
        public static int isPrimaIstanzaCons(string idPeople, string idGruppo)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
            int result = 0;
            result = doc.isPrimaIstanzaConservazione(idPeople, idGruppo);
            if (result == -1)
                throw new Exception();
            return result;
        }

        public static string getPrimaIstanzaCons(string idPeople, string idGruppo)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
            string result = string.Empty;
            result = doc.getPrimaIstanzaConservazione(idPeople, idGruppo);
            if (result == "Errore")
                throw new Exception();
            return result;
        }

        public static List<SearchResultInfo> getListaDocumentiByIdProject(string idProject)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            List<SearchResultInfo> tempList = fascicoli.getIdDocFasc(idProject);
            return tempList;
        }
        public static void updateItemsCons(string tipoFile, string numAllegati, string systemId)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
            bool result = true;
            result = doc.updateItemsCons(tipoFile, numAllegati, systemId);
            if (!result)
                throw new Exception();
        }

        public static void DeleteDocumentoFromItemCons(string idIstanza, string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();

            doc.DeleteDocumentoFromItemCons(idIstanza, idProfile);
        }

        public static bool DeleteFromDpaItemsCons(string idIstanza, string systemId, string idProject)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
            bool result = false;
            result = doc.DocumentoDeleteFromItemsCons(idIstanza, systemId, idProject);
            if (!result)
                throw new Exception();
            return result;
        }
        public static bool UpdateEsitoDpaItemsCons(string systemId, string esito)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
            bool result = false;
            result = doc.updateEsitoItemCons(systemId, esito);
            if (!result)
                throw new Exception();
            return result;
        }

        /// <summary>
        /// Inserimento di un documento in un'istanza di conservazione già esistente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="tipoOggetto"></param>
        /// <returns></returns>
        public static bool AddDocInAreaConservazioneWithPolicy(
                                    DocsPaVO.utente.InfoUtente infoUtente,
                                    string idConservazione,
                                    string idProfile,
                                    string idProject,
                                    string docNumber,
                                    string tipoOggetto,
                                    string stato,
                                    DocsPaVO.areaConservazione.ItemPolicyValidator itemPolicyValidator )
        {
            bool result = false;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();

                //result = doc.addAreaConservazione(idConservazione, idProfile, idProject, docNumber, infoUtente, tipoOggetto, null);

                result = doc.AddDocInAreaConservazioneWithPolicy(idConservazione, idProfile, idProject, docNumber, tipoOggetto, infoUtente, stato, itemPolicyValidator);

                transactionContext.Complete();

                return result;
            }
        }

        //
        // Mev Cs 1.4 - Esibizione
        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="idPolicy"></param>
        /// <returns></returns>
        public static string CreateAndAddDocInAreaEsibizione(
                        DocsPaVO.utente.InfoUtente infoUtente,
                        string idProfile,
                        string idProject,
                        string docNumber,
                        string tipoOggetto,
                        string idConservazione,
                        out DocsPaVO.documento.SchedaDocumento sd
                        )
        {
            string result = String.Empty;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
                logger.DebugFormat("ADDAREAESIBIZIONE START");
                
                result = doc.addAreaEsibizione(string.Empty, idProfile, idProject, docNumber, infoUtente, tipoOggetto, idConservazione, out sd);
                
                logger.DebugFormat("ADDAREAESIBIZIONE END -> RESULT {0}", result);
                if (result == "-1")
                {
                    logger.Debug("Errore nella generazione dell'area conservazione (CreateAndAddDocInAreaEsibizione)");
                    throw new Exception();
                }

                transactionContext.Complete();

                return result;
            }
        }

        /// <summary>
        /// Update dimensione item esibizione
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="size"></param>
        public static void updateSizeItemEsib(string sysId, int size)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
            bool result = true;
            result = doc.insertSizeInItemsEsib(size, sysId);
            if (!result)
                throw new Exception();
        }

        /// <summary>
        /// Update informazione items esibizione
        /// </summary>
        /// <param name="tipoFile"></param>
        /// <param name="numAllegati"></param>
        /// <param name="systemId"></param>
        public static void updateItemsEsib(string tipoFile, string numAllegati, string systemId)
        {
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
            bool result = true;
            result = doc.updateItemsEsib(tipoFile, numAllegati, systemId);
            if (!result)
                throw new Exception();
        }

        /// <summary>
        /// Metodo per verificare la presenza di un item esibizione con idProfile in un'istanza di esibizione per lo stesso idPeople e idGruppo proveniente dalla stessa istanza di conservazione
        /// </summary>
        /// <param name="id_profile"></param>
        /// <param name="id_project"></param>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanzaConservazione"></param>
        /// <returns></returns>
        public static bool checkItemEsibizionePresenteInIstanzaEsibizione(string id_profile, string id_project, string type, DocsPaVO.utente.InfoUtente infoUtente, string idIstanzaConservazione)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
            result = doc.checkItemEsibizionePresenteInIstanzaEsibizione(id_profile, id_project, type, infoUtente, idIstanzaConservazione);
            return result;
        }
        
        // End Mev Cs 1.4 - Esibizione
        // 

        #region CS 1.5 - Requisito F03_01
        // Metodi ausiliari

        /// <summary>
        /// Metodo che recupera la dimensione massima delle istanze per l'amministrazione in termini di MB Ammessi
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static int getDimensioneMassimaIstanze_Byte(string idAmm) 
        {
            logger.Debug("Debug - BusinessLogic.Documenti.areaConservazioneManager.cs");
            logger.Debug("INIZIO - getDimensioneMassimaIstanze_MB");
            // Valore di ritorno
            int dimMax_MB = 0;

            // Valore della chiave di configrazione BE_CONSERVAZIONE_MAX_DIM_ISTANZA per amministrazione
            string configString = string.Empty;
            try
            {
                logger.DebugFormat("Get chiave di configurazione: BE_CONSERVAZIONE_MAX_DIM_ISTANZA per l'amministrazione {0}", idAmm );
                configString = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_CONSERVAZIONE_MAX_DIM_ISTANZA");
                logger.DebugFormat("Valore BE_CONSERVAZIONE_MAX_DIM_ISTANZA: {1} per l'amministrazione {0}", idAmm, configString);
            }
            catch (Exception ex) 
            {
                // Non è stato possibile recuperare il valore della chiave di configurazione
                logger.DebugFormat("Exception: {0}", ex.Message);
            }

            if (!string.IsNullOrEmpty(configString))
            {
                string dimMaxIst = configString.Split('§')[1];
                Int32.TryParse(dimMaxIst, out dimMax_MB);
            }
            else
            {   
                // Default Value: 650MB & 250 #Doc
                dimMax_MB = 650;
            }

            double dimMax_Byte = dimMax_MB * 1024 * 1024;
            int dimMaxByte = Convert.ToInt32(dimMax_Byte);

            //return dimMax_MB;
            return dimMaxByte;
        }

        /// <summary>
        /// Metodo che recupera la dimensione massima delle istanze per l'amministrazione in termini di Numero di Documenti Ammessi
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static int getDimensioneMassimaIstanze_NumDoc(string idAmm) 
        {
            // Valore di ritorno
            int dimMax_numDoc = 0;

            logger.Debug("Debug - BusinessLogic.Documenti.areaConservazioneManager.cs");
            logger.Debug("INIZIO - getDimensioneMassimaIstanze_NumDoc");
            
            // Valore della chiave di configrazione BE_CONSERVAZIONE_MAX_DIM_ISTANZA per amministrazione
            string configString = string.Empty;
            try
            {
                logger.DebugFormat("Get chiave di configurazione: BE_CONSERVAZIONE_MAX_DIM_ISTANZA per l'amministrazione {0}", idAmm);
                configString = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_CONSERVAZIONE_MAX_DIM_ISTANZA");
                logger.DebugFormat("Valore BE_CONSERVAZIONE_MAX_DIM_ISTANZA: {1} per l'amministrazione {0}", idAmm, configString);
            }
            catch (Exception ex)
            {
                // Non è stato possibile recuperare il valore della chiave di configurazione
                logger.DebugFormat("Exception: {0}", ex.Message);
            }

            if (!string.IsNullOrEmpty(configString))
            {
                string dimMaxIst = configString.Split('§')[0];
                Int32.TryParse(dimMaxIst, out dimMax_numDoc);
            }
            else
            {
                // Default Value: 650MB & 250 #Doc
                dimMax_numDoc = 250;
            }

            return dimMax_numDoc;
        }

        /// <summary>
        /// Metodo per reperire la percentuale di tolleranza sulla dimensione delle istanze da inviare in conservazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static int getPercentualeTolleranzaDinesioneIstanze(string idAmm)
        {
            // Valore di ritorno
            int percToll = 0;

            logger.Debug("Debug - BusinessLogic.Documenti.areaConservazioneManager.cs");
            logger.Debug("INIZIO - getPercentualeTolleranzaDinesioneIstanze");

            // Valore della chiave di configrazione BE_CONSERVAZIONE_MAX_DIM_ISTANZA per amministrazione
            string configString = string.Empty;
            try
            {
                logger.DebugFormat("Get chiave di configurazione: BE_CONS_PERC_TOLL_MAX_DIM_IST per l'amministrazione {0}", idAmm);
                configString = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_CONS_PERC_TOLL_MAX_DIM_IST");
                logger.DebugFormat("Valore BE_CONS_PERC_TOLL_MAX_DIM_IST: {1} per l'amministrazione {0}", idAmm, configString);
            }
            catch (Exception ex)
            {
                // Non è stato possibile recuperare il valore della chiave di configurazione
                logger.DebugFormat("Exception: {0}", ex.Message);
            }

            if (!string.IsNullOrEmpty(configString))
            {
                Int32.TryParse(configString, out percToll);
            }
            else
            {
                // Default Value: 10 %
                percToll = 10;
            }

            return percToll;
        }

        /// <summary>
        /// Ritorna la somma della dimensione documenti nell'istanza
        /// </summary>
        /// <param name="idIstanzaConserv"></param>
        /// <returns></returns>
        public static int getDimensioneIstanza_Byte(string idIstanzaConserv)
        {
            // Valore di ritorno
            int retVal = 0;

            logger.Debug("Debug - BusinessLogic.Documenti.areaConservazioneManager.cs");
            logger.Debug("INIZIO - getDimensioneIstanza_Byte");

            // get size_items in dpa_items_conservazione
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();

            string sumSize = string.Empty; 
            sumSize = doc.getSumSizeItem_byIdConservazione(idIstanzaConserv);

            logger.DebugFormat("getDimensioneIstanza_Byte - sumSize value: {0}", sumSize.ToString());
            if (!string.IsNullOrEmpty(sumSize)) 
            {
                Int32.TryParse(sumSize, out retVal);
            }

            return retVal;
        }

        /// <summary>
        /// Ritorna il numero di documenti nell'istanza
        /// </summary>
        /// <param name="idIstanzaConserv"></param>
        /// <returns></returns>
        public static int getNumeroDocIstanza(string idIstanzaConserv)
        {
            // Valore di ritorno
            int retVal = 0;

            logger.Debug("Debug - BusinessLogic.Documenti.areaConservazioneManager.cs");
            logger.Debug("INIZIO - getNumeroDocIstanza");

            // get count in dpa_items_conservazione
            DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();

            string count = string.Empty;
            count = doc.getCountNumDocInIstanza_byIdConservazione(idIstanzaConserv);

            logger.DebugFormat("getNumeroDocIstanza - count: {0}", count.ToString());
            if (!string.IsNullOrEmpty(count))
            {
                Int32.TryParse(count, out retVal);
            }

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numDocInIstanza">Numero di documenti raggiunti per il documento i-esimo</param>
        /// <param name="vincoloNumDocInStanza">Vincolo numero di documenti ammessi per l'amministrazione</param>
        /// <returns></returns>
        public static bool isVincoloNumeroDocumentiIstanzaViolato(int numDocInIstanza, int vincoloNumDocInStanza) 
        {
            logger.Debug("Debug - BusinessLogic.Documenti.areaConservazioneManager.cs");
            logger.Debug("INIZIO - isVincoloNumeroDocumentiIstanzaViolato");
            logger.DebugFormat("Numero di documenti che si vogliono inserire all'interno dell'istanza: {0}", numDocInIstanza);
            logger.DebugFormat("Numero di documenti massimi per l'istanza: {0}", vincoloNumDocInStanza);
            
            bool NumDocIstViolato = false;

            if (numDocInIstanza > vincoloNumDocInStanza)
                NumDocIstViolato = true;

            logger.DebugFormat("Vincolo Numero Documenti Violato: {0}", NumDocIstViolato);

            return NumDocIstViolato;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DimensioneInIstanza">Dimensione raggiunta per il documento i-esimo</param>
        /// <param name="vincoloDimensioneInStanza">Vincolo di dimensione massima ammessa per l'amministrazione</param>
        /// <param name="percentualeTolleranza">percentuale di tolleranza ammessa per l'amministrazione</param>
        /// <returns></returns>
        public static bool isVincoloDimensioneIstanzaViolato(int DimensioneInIstanza, int vincoloDimensioneInIStanza, int percentualeTolleranza)
        {
            logger.Debug("Debug - BusinessLogic.Documenti.areaConservazioneManager.cs");
            logger.Debug("INIZIO - isVincoloDimensioneIstanzaViolato");
            logger.DebugFormat("Dimensione documenti che si vogliono inserire all'interno dell'istanza: {0}", DimensioneInIstanza);
            logger.DebugFormat("Dimensione massima per l'istanza: {0}", vincoloDimensioneInIStanza);
            logger.DebugFormat("Percentuale di Tolleranza: {0}", percentualeTolleranza);
            
            bool DimIstViolato = false;

            double DimensioneMassimaConsentitaPerIstanza = 0;
            DimensioneMassimaConsentitaPerIstanza = vincoloDimensioneInIStanza - ((vincoloDimensioneInIStanza * percentualeTolleranza) / 100);

            int DimMaxConsentita = 0;
            DimMaxConsentita = Convert.ToInt32(DimensioneMassimaConsentitaPerIstanza);

            logger.DebugFormat("Dimensione Massima Complessiva Consentita: {0}", DimMaxConsentita);

            if (DimensioneInIstanza > DimMaxConsentita)
                DimIstViolato = true;

            logger.DebugFormat("Vincolo Dimensione Istanza Violato: {0}", DimIstViolato);

            return DimIstViolato;
        }

        /// <summary>
        /// Creazione di una nuova istanza di conservazione e inserimento di un documento nell'istanza stessa con il rispetto dei vincoli 
        /// sul numero di documenti e dimensione massima consentita.
        /// Il metodo è lo stesso sia per l'operazione puntuale che per quella Massiva.
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idProfile"></param>
        /// <param name="idProject"></param>
        /// <param name="docNumber"></param>
        /// <param name="tipoOggetto"></param>
        /// <param name="idPolicy"></param>
        /// <returns></returns>
        public static string CreateAndAddDocInAreaConservazione_WithConstraint(
                        DocsPaVO.utente.InfoUtente infoUtente,
                        string idProfile,
                        string idProject,
                        string docNumber,
                        string tipoOggetto,
                        string idPolicy,
                        bool numDocIstanzaViolato,
                        bool dimIstanzaViolato,
                        int vincoloDimIstanza, 
                        int vincoloNumDocIstanza,
                        int sizeItem
            )
        {
            string result = String.Empty;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Conservazione doc = new DocsPaDB.Query_DocsPAWS.Conservazione();
                logger.DebugFormat("ADDAREACONSERVAZIONE_WithConstraint START");
                // Da Cambiare
                result = doc.addAreaConservazione_WithConstraint(string.Empty, idProfile, idProject, docNumber, infoUtente, tipoOggetto, idPolicy, numDocIstanzaViolato, dimIstanzaViolato, vincoloDimIstanza, vincoloNumDocIstanza, sizeItem);
                logger.DebugFormat("ADDAREACONSERVAZIONE_WithConstraint END -> RESULT {0}", result);
                if (result == "-1")
                {
                    logger.Debug("Errore nella generazione dell'area conservazione (CreateAndAddDocInAreaConservazione_WithConstraint)");
                    throw new Exception();
                }

                transactionContext.Complete();

                return result;
            }
        }
        #endregion
    }
}
