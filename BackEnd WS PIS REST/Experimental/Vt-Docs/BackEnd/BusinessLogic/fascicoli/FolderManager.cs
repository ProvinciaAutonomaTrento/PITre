using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using DocsPaVO.ricerche;
using log4net;
using DocsPaVO.Grid;
using DocsPaVO.fascicolazione;

namespace BusinessLogic.Fascicoli
{
    /// <summary>
    /// </summary>
    public class FolderManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(FolderManager));
        #region Metodo Commentato
        //		/// <summary>
        //		/// </summary>
        //		/// <param name="db"></param>
        //		/// <param name="folder"></param>
        //		/// <param name="infoUtente"></param>
        //		/// <param name="ereditaDiritti"></param>
        //		/// <param name="debug"></param>
        //		/// <returns></returns>
        //		public static DocsPaDocManagement.Documentale.Documento newFolder(/*DocsPaWS.Utils.Database db,*/ ref DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.InfoUtente infoUtente, bool ereditaDiritti) 
        //		{
        //			/*
        //			PCDCLIENTLib.PCDDocObject docObj = new PCDCLIENTLib.PCDDocObject();
        //			*/
        //			DocsPaDocManagement.Documentale.Documento documento;
        //			
        //			string library = DocsPaDB.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary();
        //
        //			//folder.systemID = ProjectsManager.createPCDProject(out documento, folder.descrizione , infoUtente);
        //			DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente.userId, infoUtente.dst, library);
        //			documentManager.CreatePCDProject(out documento, folder.descrizione);			
        //			
        //			try 
        //			{
        //				/*string updateString = 
        //					"UPDATE PROJECT SET CHA_TIPO_PROJ = 'C'" +
        //					", ID_PARENT = " + folder.idParent +
        //					", ID_AMM = " + infoUtente.idAmministrazione +
        //					", ID_REGISTRO = null " +
        //					", ID_FASCICOLO = " + folder.idFascicolo +
        //					", DTA_APERTURA = " + DocsPaWS.Utils.dbControl.getDate() +
        //					", DTA_CHIUSURA = null " +
        //					" WHERE SYSTEM_ID = " + folder.systemID;
        //				logger.Debug(updateString);
        //				db.executeNonQuery(updateString);*/
        //				DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
        //				fascicoli.NewFolder(folder,infoUtente);
        //				fascicoli.Dispose();
        //
        //				if(ereditaDiritti)
        //				{
        //					ProjectsManager.setVisibilita(/*db,*/ folder.systemID, folder.idFascicolo);
        //				}
        //			} 
        //			catch (Exception e) 
        //			{
        //				logger.Debug(e.Message);
        //				documento.Delete();
        //				logger.Debug("Risultato della delete su Fusion: " + documento.GetErrorCode());
        //				//db.closeConnection();				
        //
        //				throw new Exception("F_System");
        //			}
        //
        //			return documento;
        //		}
        #endregion

        /// <summary>
        /// I campi minimi che devono essere settati per l'oggetto Folder sono:
        /// descrizione
        /// idFascicolo
        /// idParent
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Folder newFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.fascicolazione.ResultCreazioneFolder result)
        {
            bool created = false;

            DocsPaDocumentale.Documentale.ProjectManager projectManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);

            // Inizializzazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                // Ruoli superiori cui viene trasmessa la visibilità
                DocsPaVO.utente.Ruolo[] ruoliSuperiori;

                created = projectManager.CreateFolder(folder, ruolo, out result, out ruoliSuperiori);

                if (created)
                {
                    try
                    {
                        // Notifica evento folder creato
                        DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(infoUtente);
                        eventsNotification.SottofascicoloCreatoEventHandler(folder, ruolo, ruoliSuperiori);

                        // Completamento transazione
                        transactionContext.Complete();
                    }
                    catch (Exception ex)
                    {
                        folder = null;

                        created = false;
                        result = DocsPaVO.fascicolazione.ResultCreazioneFolder.DM_ERROR;

                        logger.Debug(string.Format("Errore nella creazione del folder: {0}", ex.Message));
                    }
                }
                else
                    folder = null;
            }

            return folder;
        }

        /// <summary>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static bool NewFolderXml(ref DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.InfoUtente infoUtente, out DocsPaVO.fascicolazione.ResultCreazioneFolder result)
        {
            bool retValue = false;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                DocsPaVO.utente.Ruolo ruolo = null;
                if (infoUtente.idGruppo != null)
                    ruolo = amministrazioneXml.GetRuolo(infoUtente.idGruppo);

                DocsPaDocumentale.Documentale.ProjectManager documentManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);

                retValue = documentManager.CreateFolder(folder, ruolo, out result);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nella creazione del folder.", exception);

                retValue = false;
                result = DocsPaVO.fascicolazione.ResultCreazioneFolder.GENERIC_ERROR;
            }

            return retValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="objFolderOld"></param>
        /// <param name="objFolderNew"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Folder moveFolder(DocsPaVO.fascicolazione.Folder objFolderOld, DocsPaVO.fascicolazione.Folder objFolderNew)
        {
            //TODO: moveFolder
            return new DocsPaVO.fascicolazione.Folder();
        }

        /// <summary>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        public static void delFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.InfoUtente infoUtente)
        {
            // verifico se è un root folder
            #region Codice Commentato
            //			if(folder.idParent.Equals(folder.idFascicolo))
            //				throw new Exception("Non si può cancellare un root folder");
            //
            //			DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            //			int numFigli = 0;
            //			try {
            //				db.openConnection();
            //				// posso cancellare il folder solo se non ha figli
            //				/*string queryString =
            //					"SELECT COUNT(*) FROM PROJECT WHERE ID_PARENT=" + folder.systemID;
            //				numFigli = Int32.Parse(db.executeScalar(queryString).ToString());*/
            //				DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new
            //					DocsPaDB.Query_DocsPAWS.Fascicoli();
            //				numFigli = fascicoli.GetFolderCount(folder.systemID);
            //
            //				db.closeConnection();
            //			} catch (Exception e) {
            //				logger.Debug(e.Message);				
            //				db.closeConnection();				
            //				throw new Exception("F_System");
            //			}
            //			if (numFigli == 0)
            //				ProjectsManager.deletePCDProject(folder.systemID, infoUtente);
            //			else
            //				throw new Exception("Il folder non è vuoto");
            #endregion

            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            int numFigli = fascicoli.DelFolder(folder, infoUtente);

            if (numFigli == 0)
            {
                //ProjectsManager.deletePCDProject(folder.systemID, infoUtente);
                DocsPaDocumentale.Documentale.ProjectManager projectManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);

                if (!projectManager.DeleteFolder(folder))
                {
                    logger.Debug("Errore nella gestione dei fascicoli. Errore nella cancellazione del Folder. (delFolder)");
                    throw new Exception("Errore nella cancellazione del Folder!");
                }
            }
            else
            {
                logger.Debug("Errore nella gestione dei fascicoli. Errore nella cancellazione del Folder. (delFolder)");
                throw new Exception("Errore nella cancellazione del Folder!");
            }

            fascicoli.Dispose();
        }

        /// <summary>
        /// </summary>
        /// <param name="objFascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Folder getPrimoChildFolder(string idPeople, string idGruppo, string idFasc, string idFolder)
        {

            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            folderObject = fascicoli.GetFolderPrimoChild(idPeople, idGruppo, idFasc, idFolder);

            if (folderObject == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getPrimoChildFolder)");
                throw new Exception("F_System");
            }

            fascicoli.Dispose();

            return folderObject;
        }

        /// <summary>
        /// </summary>
        /// <param name="objFascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Folder getFolder(string idPeople, string idGruppo, DocsPaVO.fascicolazione.Fascicolo objFascicolo)
        {
            //return getFolder(objFascicolo.systemID, infoUtente);
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            folderObject = fascicoli.GetFolder(idPeople, idGruppo, objFascicolo.systemID);

            if (folderObject == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getFolder)");
                throw new Exception("F_System");
            }

            fascicoli.Dispose();

            return folderObject;
        }

        //ABBATANGELI GIANLUIGI
        /// <summary>
        /// </summary>
        /// <param name="objFascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Folder getFolderAndChild(string idPeople, string idGruppo, DocsPaVO.fascicolazione.Folder objFolder)
        {
            //return getFolder(objFascicolo.systemID, infoUtente);
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            folderObject = fascicoli.GetFolderAndChildByIdFascicoloIdFolder(idPeople, idGruppo, objFolder.idFascicolo, objFolder.systemID);

            if (folderObject == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getFolder)");
                throw new Exception("F_System");
            }

            fascicoli.Dispose();

            return folderObject;
        }

        public static DocsPaVO.fascicolazione.Folder getFolderByIdFascicolo(string idPeople, string idGruppo, DocsPaVO.fascicolazione.Fascicolo objFascicolo)
        {
            //return getFolder(objFascicolo.systemID, infoUtente);
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            folderObject = fascicoli.GetFolderByIdFascicolo(idPeople, idGruppo, objFascicolo.systemID);

            if (folderObject == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getFolder)");
                throw new Exception("F_System");
            }

            fascicoli.Dispose();

            return folderObject;
        }


        /// <summary>
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Folder getFolder(string idPeople, string idGruppo, string idFascicolo)
        {
            #region Codice Commentato
            //			logger.Debug("getFolder");
            //			DocsPaWS.Utils.Database database = DocsPaWS.Utils.dbControl.getDatabase();			
            //			DocsPaVO.fascicolazione.Folder folderObject= new DocsPaVO.fascicolazione.Folder();
            //			try {
            //				database.openConnection();
            //				DataSet dataSet= new DataSet();
            //				
            //				string commandString1=
            //					" SELECT DISTINCT A.* FROM PROJECT A, SECURITY B " +
            //					" WHERE A.SYSTEM_ID=B.THING AND A.ID_FASCICOLO=" + idFascicolo +
            //					" AND (B.PERSONORGROUP=" + infoUtente.idPeople + " OR B.PERSONORGROUP=" + infoUtente.idGruppo + ") AND B.ACCESSRIGHTS > 0";
            //					
            //				logger.Debug(commandString1);
            //				
            //				database.fillTable(commandString1,dataSet,"FOLDER");
            //
            //				DataRow[] folderRootRows = dataSet.Tables["FOLDER"].Select("ID_PARENT=" + idFascicolo);
            //				if (folderRootRows.Length > 0) 
            //					folderObject = getFolderData(folderRootRows[0],dataSet.Tables["FOLDER"]);
            //				
            //			} catch (Exception e) {
            //				logger.Debug (e.Message);				
            //				
            //				database.closeConnection();
            //				throw new Exception("F_System");
            //			}
            //			return folderObject;
            #endregion

            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();
            folderObject = fascicoli.GetFolder(idPeople, idGruppo, idFascicolo);
            fascicoli.Dispose();

            return folderObject;
        }

        /// <summary>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Folder getFolderById(string idPeople, string idGruppo, string idFolder)
        {
            logger.Debug("getFolderById. idFolder: " + idFolder);

            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();
            folderObject = fascicoli.GetFolderById(idPeople, idGruppo, idFolder);
            fascicoli.Dispose();
            if (folderObject == null)
            {
                logger.Debug("getFolderById = NULL: folder non trovato!!!");
            }

            return folderObject;
        }

        public static ArrayList getFolderByDescr(string idPeople, string idGruppo, string idFasc, string descrFolder)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            ArrayList result = fascicoli.GetFolderByDescr(idPeople, idGruppo, idFasc, descrFolder);

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Folder modifyFolder(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Folder folder)
        {
            DocsPaDocumentale.Documentale.ProjectManager projectManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);

            if (!projectManager.ModifyFolder(folder))
            {
                string errorMessage = "Errore nella modifica dei dati del fascicolo";
                logger.Debug(errorMessage);
                throw new Exception(errorMessage);
            }

            return folder;
        }

        /// <summary>
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="table"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        internal static DocsPaVO.fascicolazione.Folder getFolderData(DataRow dataRow, DataTable table)
        {
            DocsPaVO.fascicolazione.Folder folder = new DocsPaVO.fascicolazione.Folder();
            folder.descrizione = dataRow["DESCRIPTION"].ToString();
            folder.systemID = dataRow["SYSTEM_ID"].ToString();
            folder.idFascicolo = dataRow["ID_FASCICOLO"].ToString();
            folder.idParent = dataRow["ID_PARENT"].ToString();
            folder.childs = getFolderChildren(folder.systemID, table);

            return folder;
        }

        /// <summary>
        /// </summary>
        /// <param name="parent_id"></param>
        /// <param name="table"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private static ArrayList getFolderChildren(string parent_id, DataTable table)
        {
            logger.Debug("parent_id: " + parent_id);
            DataRow[] folderChildrenRows = table.Select("ID_PARENT=" + parent_id);
            logger.Debug("num figli: " + folderChildrenRows.Length.ToString());
            ArrayList folderChildren = new ArrayList();

            for (int i = 0; i < folderChildrenRows.Length; i++)
            {
                logger.Debug("System id: " + folderChildrenRows[i]["SYSTEM_ID"].ToString());
                folderChildren.Add(getFolderData(folderChildrenRows[i], table));
            }

            return folderChildren;
        }

        /// <summary>
        /// Creazione di un array di oggetti "Folder" che
        /// contengono il documento fornito.
        /// </summary>
        public static ArrayList GetFoldersDocument(string systemIdDocument)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli queryFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return queryFasc.GetFoldersDocument(systemIdDocument);
        }

        /// <summary>
        /// Creazione di un array di oggetti "Folder" che
        /// contengono il documento fornito.
        /// </summary>
        public static ArrayList GetFoldersDocument(string systemIdDocument, string systemIdFascicolo)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli queryFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return queryFasc.GetFoldersDocument(systemIdDocument, systemIdFascicolo);
        }


        /// <summary>
        /// </summary>
        /// <param name="objFolder"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getDocumenti(string idGruppo, string idPeople, DocsPaVO.fascicolazione.Folder objFolder)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            ArrayList result = fascicoli.GetDocumenti(idGruppo, idPeople, objFolder);

            if (result == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getDocumenti)");
                throw new Exception("F_System");
            }

            return result;

            #region Codice Commentato
            /*System.Collections.ArrayList listaDocumenti = new System.Collections.ArrayList();
			DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			try 
			{
				db.openConnection();
				string queryString = "";
					"SELECT DISTINCT A.SYSTEM_ID,A.DOCNUMBER, A.DOCNAME, A.STATUS, A.DOCSERVER_LOC, A.PATH, " + 
					DocsPaWS.Utils.dbControl.toChar("A.CREATION_DATE",false) + " AS CREATION_DATE, " +
					"A.ID_REGISTRO, A.CHA_TIPO_PROTO, A.ID_OGGETTO, A.NUM_PROTO, " +
					"A.NUM_ANNO_PROTO, A.VAR_PROTO_EME, A.DTA_PROTO_EME, " +
					"A.VAR_COGNOME_EME, A.VAR_NOME_EME, A.ID_PARENT, A.DTA_PROTO, " +
					"A.CHA_MOD_OGGETTO, A.CHA_MOD_MITT_DEST, A.CHA_MOD_MITT_INT, " +
					"A.CHA_MOD_DEST_OCC, A.DTA_PROTO_IN, A.VAR_PROTO_IN, " +
					"A.ID_ANNULLATORE, A.DTA_ANNULLA, A.VAR_AUT_ANNULLA, " +
					"A.VAR_SEGNATURA, A.CHA_DA_PROTO, A.VAR_NOTE, A.CHA_ASSEGNATO, A.CHA_IMG, A.CHA_FASCICOLATO, A.CHA_INVIO_CONFERMA, " +
					"A.ID_TIPO_ATTO, A.CHA_PRIVATO, A.CHA_EVIDENZA, C.VAR_DESC_OGGETTO, B.PROJECT_ID, B.LINK, B.TYPE, " +
					"D.TYPE_ID, D.DESCRIPTION, E.ACCESSRIGHTS " +
					"FROM PROFILE A, SECURITY E, PROJECT_COMPONENTS B, DPA_OGGETTARIO C,  DOCUMENTTYPES D " +
					"WHERE B.TYPE = 'D' AND C.SYSTEM_ID=A.ID_OGGETTO " +
					"AND A.DOCUMENTTYPE=D.SYSTEM_ID " +
					"AND A.SYSTEM_ID=E.THING AND A.SYSTEM_ID=B.LINK AND B.PROJECT_ID=" + objFolder.systemID + " AND " +
					"(E.PERSONORGROUP=" + infoUtente.idGruppo + " OR E.PERSONORGROUP=" + infoUtente.idPeople + ") AND E.ACCESSRIGHTS > 0" +
					" ORDER BY A.SYSTEM_ID DESC";
				
				logger.Debug (queryString);
				DataSet dataSet = new DataSet();
				db.fillTable(queryString, dataSet, "PROFILE");	

				//creazione della lista oggetti
				foreach(DataRow dataRow in dataSet.Tables["PROFILE"].Rows) 					
				{
					listaDocumenti.Add(DocManager.getSchedaDocumento(db, dataRow));
				}

				dataSet.Dispose();
				db.closeConnection();
			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);				
				db.closeConnection();
				
				throw new Exception("F_System");
			}

			return listaDocumenti;*/
            #endregion
        }

        #region paginazione

        public static System.Collections.ArrayList getDocumentiPaging(
                        string idGruppo,
                        string idPeople,
                        DocsPaVO.fascicolazione.Folder objFolder,
                        DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca,
                        int numPage,
                        out int numTotPage,
                        out int nRec,
                        bool compileIdProfileList,
                        out List<SearchResultInfo> idProfiles)
        {
            numTotPage = 0;
            nRec = 0;
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            ArrayList result = fascicoli.GetDocumentiPaging(idGruppo, idPeople, objFolder, filtriRicerca, numPage, out numTotPage, out nRec, compileIdProfileList, out idProfiles);

            if (result == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getDocumentiPaging)");
                throw new Exception("F_System");
            }

            return result;
        }

        public static System.Collections.ArrayList getDocumentiPaging(string idGruppo, string idPeople, DocsPaVO.fascicolazione.Folder objFolder, int numPage, out int numTotPage, out int nRec, bool compileIdProfileList, out List<SearchResultInfo> idProfiles)
        {
            numTotPage = 0;
            nRec = 0;
            return getDocumentiPaging(idGruppo, idPeople, objFolder, null, numPage, out numTotPage, out nRec, compileIdProfileList, out idProfiles);
        }



        #endregion

        /// <summary>
        /// Verifica se un documento può essere inserito in un folder (record di tipo "C")
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <param name="errorMssage">
        /// Errore di validazione riscontrato
        /// </param>
        /// <returns></returns>
        public static bool canAddDocFolder(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string idFolder, out string errorMssage)
        {
            bool retValue = false;
            errorMssage = string.Empty;

            using (DocsPaDB.Query_DocsPAWS.Fascicoli dbFascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
            {
                if (!dbFascicoli.IsDocumentoClassificatoInFolder(idProfile, idFolder))
                {
                    if (dbFascicoli.isFascicoloGenerale(infoUtente, idFolder))
                    {
                        // Classificazione su fascicolo generale
                        if (dbFascicoli.isTitolarioChiuso(infoUtente, idFolder))
                        {
                            // Il titolario è chiuso, impossibile classificare
                            retValue = false;
                            errorMssage = "Impossibile classificare il documento su un titolo non aperto";
                        }
                        else
                            retValue = true;
                    }
                    else
                    {
                        // Fascicolazione documento su procedimentale

                        // Verifica che lo stato del fascicolo non sia chiuso
                        if (!dbFascicoli.isFascicoloProcedimentaleAperto(infoUtente, idFolder))
                        {
                            retValue = false;
                            errorMssage = "Impossibile fascicolare il documento su un procedimentale chiuso";
                        }
                        else
                            retValue = true;
                    }
                }
                else
                {
                    retValue = false;
                    errorMssage = "Il documento risulta già classificato nel fascicolo richiesto";
                }
            }

            return retValue;
        }

        /// <summary>
        /// add doc to fascicolo
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        public static bool addDocFolder(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string idFolder, bool fascRapida, out string msg)
        {
            logger.Info("BEGIN");
            bool result = true;
            msg = string.Empty;

            // Creazione contesto transazionale
            //QUI
            //DocsPaVO.Validations.ValidationResultInfo result = null;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                if (BusinessLogic.CheckInOut.CheckInOutServices.IsCheckedOut(idProfile, idProfile, infoUtente))
                {
                    msg = string.Format("Il documento con ID {0} risulta bloccato e non può essere inserito nel folder con ID {1}", idProfile, idFolder);
                    result = false;
                }
                else
                {
                    // Verifica se il titolario in cui si cerca di inserire il documento è chiuso o meno
                    if (canAddDocFolder(infoUtente, idProfile, idFolder, out msg))
                    {
                        DocsPaDocumentale.Documentale.ProjectManager projectManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);
                        result = projectManager.AddDocumentInFolder(idProfile, idFolder);

                        if (!result)
                            throw new ApplicationException(string.Format("Errore nell'inserimento del documento con id {0} nel folder con id {1}", idProfile, idFolder));

                        //AS400:
                        AS400.AS400.setAs400InFolder(idProfile, infoUtente, idFolder, DocsPaAS400.Constants.CREATE_MODIFY_OPERATION);

                        //Richiamo il metodo per il calcolo della atipicità del documento
                        DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                        documentale.CalcolaAtipicita(infoUtente, idProfile, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.DOCUMENTO);

                        transactionContext.Complete();
                    }
                    else
                    {
                        logger.Debug(string.Format("Errore nell'inserimento del documento con ID '{0}' nel folder con ID '{1}'", idProfile, idFolder));
                        throw new ApplicationException(msg);
                    }
                }
                transactionContext.Complete();
                logger.Info("END");
                return result;
            }

        }

        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        private static void updateDocTrustees(string idGruppo, string idProfile, string idFolder)
        {
            #region Codice Commentato
            // l'operazione va fatta solo se si tratta di un fascicolo procedimentale
            /*string queryString = "SELECT CHA_TIPO_FASCICOLO FROM PROJECT WHERE SYSTEM_ID = (SELECT B.ID_FASCICOLO FROM PROJECT B WHERE B.SYSTEM_ID=" + idFolder + ")";
            logger.Debug(queryString);
			
            if(!db.executeScalar(queryString).ToString().ToUpper().Equals("P"))
            {
                return;
            }

            // leggo il registro cui è associato il documento
            queryString = "SELECT ID_REGISTRO FROM PROFILE WHERE SYSTEM_ID=" + idProfile;
            object idReg = db.executeScalar(queryString);
            string tabRegistro = "";
            string condRegistro = "";
			
            if (idReg != null && idReg.ToString() != null && !idReg.ToString().Equals("")) 
            {
                tabRegistro = ", DPA_L_RUOLO_REG B, DPA_CORR_GLOBALI C";
                condRegistro = "PERSONORGROUP=C.ID_GRUPPO AND C.SYSTEM_ID=B.ID_RUOLO_IN_UO AND B.ID_REGISTRO=" + idReg.ToString() + " AND ";
            }

            // aggiungo al documento tutti i diritti dati al fascicolo
            logger.Debug("updateDocTrustees");
            string updateString =
                "INSERT INTO SECURITY (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO) " +
                "(SELECT DISTINCT '" + idProfile + "', PERSONORGROUP, ACCESSRIGHTS, '" + infoUtente.idGruppo + "', 'F' " +
                "FROM SECURITY" + tabRegistro + " WHERE " + condRegistro + "THING=" + idFolder + " AND NOT PERSONORGROUP IN " +
                "(SELECT PERSONORGROUP FROM SECURITY WHERE THING='" + idProfile +"'))";
            logger.Debug(updateString);
            db.executeNonQuery(updateString);*/
            #endregion

            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            fascicoli.updateDocTrustees(idGruppo, idProfile, idFolder);
            fascicoli.Dispose();
        }

        /// <summary>
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static ArrayList getIdFolderDoc(string idFascicolo)
        {
            #region Codice Commentato
            /*DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
		    System.Data.DataSet ds=new System.Data.DataSet();
			ArrayList lista=new ArrayList();
			bool dbOpen=false;
			try
			{
			   db.openConnection();

			   //trova tutte le folder appartenenti al fascicolo
			   string queryFolderString="SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ='C' AND ID_FASCICOLO="+idFascicolo;
			   db.fillTable(queryFolderString,ds,"FOLDER");
			   for(int i=0;i<ds.Tables["FOLDER"].Rows.Count;i++){
			      lista.Add(ds.Tables["FOLDER"].Rows[i]["SYSTEM_ID"].ToString());
			   }
			   
			   //trova tutti i documenti appartenenti alle folders
			   string queryDocString="SELECT LINK FROM PROJECT_COMPONENTS WHERE PROJECT_ID IN (";
				   for(int j=0;j<ds.Tables["FOLDER"].Rows.Count;j++){
				     queryDocString=queryDocString+ds.Tables["FOLDER"].Rows[j]["SYSTEM_ID"].ToString();
					   if(j<ds.Tables["FOLDER"].Rows.Count-1){
					     queryDocString=queryDocString+",";
					   }
				   }
			   queryDocString=queryDocString+")";
			   db.fillTable(queryDocString,ds,"DOC");
			   for(int k=0;k<ds.Tables["DOC"].Rows.Count;k++)
			   {
					lista.Add(ds.Tables["DOC"].Rows[k]["LINK"].ToString());
			   }
			   return lista;
			}
			catch(Exception e){
			   logger.Debug(e.Message);
			   if(dbOpen){
			      db.closeConnection();
			   }
			   throw e;
			}*/
            #endregion

            //GetIdFolderDoc
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            ArrayList lista = new ArrayList();

            lista = fascicoli.GetIdFolderDoc(idFascicolo);

            if (lista == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getIdFolder)");
                throw new Exception();
            }

            fascicoli.Dispose();

            return lista;
        }
        /// <summary>
        /// get docs in folder, filtrati per registri 
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="regs"></param>
        /// <returns></returns>
        public static ArrayList getIdFolderDoc(string idFascicolo, ArrayList regs)
        {

            //GetIdFolderDoc
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            ArrayList lista = new ArrayList();

            lista = fascicoli.GetIdFolderDoc(idFascicolo, regs);

            if (lista == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getIdFolder)");
                throw new Exception();
            }

            fascicoli.Dispose();

            return lista;
        }

        public static DocsPaVO.Validations.ValidationResultInfo RemoveDocumentFromFolder(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, DocsPaVO.fascicolazione.Folder folder, string fascRapida, out string msg)
        {
            logger.Debug("deleteDoc");
            DocsPaVO.Validations.ValidationResultInfo result = null;
            msg = string.Empty;
            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                if (BusinessLogic.CheckInOut.CheckInOutServices.IsCheckedOut(idProfile, idProfile, infoUtente))
                {
                    result = new DocsPaVO.Validations.ValidationResultInfo();
                    string ID = "DOCUMENTO_IN_CHECKOUT";
                    result.BrokenRules.Add(new DocsPaVO.Validations.BrokenRule(ID, "Il documento non può essere rimosso dal fascicolo perchè in checkout"));

                }
                else
                {
                    DocsPaDocumentale.Documentale.ProjectManager projectManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);

                    if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableProtocolloTitolario()))
                    {
                        DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                        result = fasc.canRemoveClassificazione(infoUtente, idProfile, folder);
                        if (result != null && result.BrokenRules.Count > 0)
                            return result;
                    }

                    if (fascRapida != null && fascRapida.ToUpper().Equals("TRUE"))
                    {
                        DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                        string count = fascicoli.getCountFascicoliDocumento(idProfile);
                        if (count.Equals("1"))
                        {
                            msg = "Impossibile eliminare il documento perché la fascicolazione è obbligatoria.";
                            return result;
                        }
                    }

                    if (!projectManager.RemoveDocumentFromFolder(idProfile, folder))
                        throw new ApplicationException(string.Format("Errore nella rimozione del documento con id {0} dal folder con id {1}", idProfile, folder.systemID));
                    else
                        transactionContext.Complete();

                }
            }

            return result;
        }

        public static DocsPaVO.Validations.ValidationResultInfo RemoveDocumentFromProject(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, DocsPaVO.fascicolazione.Folder folder, string fascRapida, out string msg)
        {
            logger.Debug("deleteDoc");
            DocsPaVO.Validations.ValidationResultInfo result = null;
            msg = string.Empty;
            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                if (BusinessLogic.CheckInOut.CheckInOutServices.IsCheckedOut(idProfile, idProfile, infoUtente))
                {
                    result = new DocsPaVO.Validations.ValidationResultInfo();
                    string ID = "DOCUMENTO_IN_CHECKOUT";
                    result.BrokenRules.Add(new DocsPaVO.Validations.BrokenRule(ID, "Il documento non può essere rimosso dal fascicolo perchè in checkout"));

                }
                else
                {
                    DocsPaDocumentale.Documentale.ProjectManager projectManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);

                    if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableProtocolloTitolario()))
                    {
                        DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                        result = fasc.canRemoveClassificazione(infoUtente, idProfile, folder);
                        if (result != null && result.BrokenRules.Count > 0)
                            return result;
                    }

                    if (fascRapida != null && fascRapida.ToUpper().Equals("TRUE"))
                    {
                        DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                        string count = fascicoli.getCountFascicoliDocumento(idProfile);
                        if (count.Equals("1"))
                        {
                            msg = "Impossibile eliminare il documento dall\'ultimo fascicolo che lo contiene perché la fascicolazione è obbligatoria.";
                            return result;
                        }
                    }

                    if (!projectManager.RemoveDocumentFromProject(idProfile, folder))
                        throw new ApplicationException(string.Format("Errore nella rimozione del documento con id {0} dal folder con id {1}", idProfile, folder.systemID));
                    else
                        transactionContext.Complete();
                }
            }

            return result;
        }

        public static bool getCountSottofascDoc(string idProject, out string nFasc)
        {
            bool result = false;
            nFasc = "";
            try
            {
                logger.Debug("getCountSottofascDoc");
                DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                result = fascicoli.GetCountSottofascDoc(idProject, out nFasc);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nel'eliminazione del fascicolo. (getCountSottofascDoc)", e);
                result = false;
            }
            return result;
        }

        public static System.Collections.ArrayList getDocumentiPagingCustom(
                        DocsPaVO.utente.InfoUtente infoUtente,
                        DocsPaVO.fascicolazione.Folder objFolder,
                        DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca,
                        int numPage,
                        out int numTotPage,
                        out int nRec,
                        bool compileIdProfileList,
                        out List<SearchResultInfo> idProfiles,
                        bool showGridPersonalization,
                        bool export,
                        Field[] visibleFieldsTemplate,
                        String[] documentsSystemId, int pageSize, DocsPaVO.filtri.FiltroRicerca[][] orderRicerca)
        {
            numTotPage = 0;
            nRec = 0;
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            ArrayList result = fascicoli.GetDocumentiPagingCustom(infoUtente, objFolder, filtriRicerca, numPage, out numTotPage, out nRec, compileIdProfileList, out idProfiles, showGridPersonalization, export, visibleFieldsTemplate, documentsSystemId, pageSize, orderRicerca);

            if (result == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getDocumentiPaging)");
                throw new Exception("F_System");
            }

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Folder getFolderByIdNoSecurity(string idPeople, string idGruppo, string idFolder)
        {
            logger.Debug("getFolderById. idFolder: " + idFolder);

            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();
            folderObject = fascicoli.GetFolderByIdNoSecurity(idPeople, idGruppo, idFolder);
            fascicoli.Dispose();
            if (folderObject == null)
            {
                logger.Debug("getFolderById = NULL: folder non trovato!!!");
            }

            return folderObject;
        }


        #region TRASFERIMENTO DEPOSITO
        /// <summary>
        /// Restituisce la lista dei documenti classificati in un dato fascicolo generale
        /// per poterli archiviare
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="fascicolo"></param>
        /// <param name="numPage"></param>
        /// <param name="numTotPage"></param>
        /// <param name="nRec"></param>
        /// <param name="anno"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getDocumentiDaArchiviare(
                        string idGruppo, string idPeople,
                        DocsPaVO.fascicolazione.Fascicolo fascicolo,
                        int numPage, out int numTotPage,
                        out int nRec,
                        string anno)
        //public static System.Collections.ArrayList getDocumentiDaArchiviare(string idGruppo, string idPeople, DocsPaVO.fascicolazione.Fascicolo fascicolo, out int nRec, string anno)
        {
            numTotPage = 0;
            nRec = 0;
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            ArrayList result = fascicoli.GetDocumentiDaArchiviarePaging(idGruppo, idPeople, fascicolo, numPage, out numTotPage, out nRec, anno);
            //ArrayList result = fascicoli.GetDocumentiDaArchiviare(idGruppo, idPeople, fascicolo, out nRec, anno);

            if (result == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getDocumentiDaArchiviare)");
                throw new Exception("F_System");
            }

            return result;
        }


        #endregion


        public static bool getIfDocumentiCountVisibleIsEgualNotVisible(
                        DocsPaVO.utente.InfoUtente infoUtente,
                        DocsPaVO.fascicolazione.Folder objFolder)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            bool result = fascicoli.GetIfDocumentiCountVisibleIsEgualNotVisible(infoUtente, objFolder);

            return result;
        }

        public static bool MoveFolder(string folderId, string parentId)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            bool result = fascicoli.MoveFolder(folderId, parentId);
            return result;
        }

        //Laura 25 Marzo
        public static int getCountDocumentiInFolderCustom(DocsPaVO.utente.InfoUtente infoUtente,
                        DocsPaVO.fascicolazione.Folder objFolder,
                        DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca,
                        out List<SearchResultInfo> idProfiles
                        )
        {

            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();


            int count = fascicoli.GetCountDocumentiCustom(infoUtente, objFolder, filtriRicerca, out idProfiles);

            return count;
        }


        public static bool RenameFolder(string systemid, string newname)
        {
            return new DocsPaDB.Query_DocsPAWS.Fascicoli().RenameFolder(systemid, newname);
        }

        /// <summary>
        /// Imposta la descrizione del folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="idpeople"></param>
        /// <param name="idgruppo"></param>
        public static void setFolderDescription(ref Folder folder, string idpeople, string idgruppo)
        {
            logger.DebugFormat("setFolderDescription for : {0}", folder.descrizione);
            string newcodlevel = string.Empty;

            // Nomenclatura completa a partire dal primo figlio del fascicolo
            for (int i = 1; i < folder.codicelivello.Length / 4; i++)
            {
                string templevel = folder.codicelivello.Substring(i * 4, 4);
                newcodlevel += string.Format("{0}.", Convert.ToInt32(templevel));
            }

            // Se non è il fascicolo imposto la descrizione
            if (folder.idParent != folder.idFascicolo)
            {
                if (newcodlevel.Length > 0)
                {
                    folder.descrizione = string.Format("{0} - {1}",
                        newcodlevel.Substring(0, newcodlevel.Length - 1),
                        folder.descrizione);
                }
            }

            if (folder.childs == null)
                return;

            // Ricorsione
            for (int i = 0; i < folder.childs.Count; i++)
            {
                Folder item = folder.childs[i] as Folder;
                setFolderDescription(ref item, idpeople, idgruppo);
            }
        }

        public static bool FolderHasDocuments(Folder item)
        {
            return new DocsPaDB.Query_DocsPAWS.Fascicoli().FolderHasDocument(item);
        }
    }
}
