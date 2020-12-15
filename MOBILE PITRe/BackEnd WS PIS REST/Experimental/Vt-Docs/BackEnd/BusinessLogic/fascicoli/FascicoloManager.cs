using System;
using System.Data;
using System.Collections;
using System.Threading;
using DocsPaVO.ricerche;
using log4net;
using DocsPaVO.fascicolazione;
using System.Linq;
using System.Collections.Generic;
using DocsPaVO.Grid;
using DocsPaVO.Grids;
using DocsPaVO.utente;

namespace BusinessLogic.Fascicoli
{
	/// <summary>
	/// </summary>
	public class FascicoloManager 
	{
        private static ILog logger = LogManager.GetLogger(typeof(FascicoloManager));
		private static Mutex semFasc = new Mutex();

		/// <summary>
		/// Eccezione che indica che il codice fascicolo è già presente
		/// </summary>
		public class FascicoloPresenteException : Exception
		{
			public override string Message
			{
				get	{ return "Codice fascicolo già presente"; }
			}
		}
        public class FormatoFascicolaturaException : Exception
        {
            public override string Message
            {
                get { return "Formato fascicolatura non presente. Contattare l'Amministratore"; }
            }
        }

		/// <summary>
		/// Reperimento di un fascicolo dalla systemID
		/// </summary>
		/// <param name="id"></param>
		/// <param name="infoUtente"></param>
		/// <param name="registro"></param>
		/// <param name="enableUffRef"></param>
		/// <returns></returns>
		public static DocsPaVO.fascicolazione.Fascicolo GetFascicolo(string id,
																		DocsPaVO.utente.InfoUtente infoUtente,
																		DocsPaVO.utente.Registro registro,
																		bool enableUffRef,
                                                                        bool enableProfilazione) 
		{
			DocsPaVO.fascicolazione.Fascicolo retValue=null;

			using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli=new DocsPaDB.Query_DocsPAWS.Fascicoli())
                retValue = fascicoli.GetFascicolo(id, infoUtente, registro, enableUffRef, enableProfilazione);

			return retValue;
		}

		/// <summary>
		/// </summary>
		/// <param name="codiceFascicolo"></param>
		/// <param name="registro"></param>
		/// <param name="infoUtente"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static DocsPaVO.fascicolazione.Fascicolo getFascicoloById(string idFascicolo, DocsPaVO.utente.InfoUtente infoUtente) 
		{
			logger.Debug("getFascicoloById. idFascicolo: "+idFascicolo);

			DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            fascicolo = fascicoli.GetFascicoloById(idFascicolo, infoUtente);

            if (fascicolo == null)
                logger.Debug("getFascicoloById = NULL: fascicolo non trovato!!!");
            else
            {
                string key_beprojectstructure = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");
                if (!string.IsNullOrEmpty(key_beprojectstructure) && key_beprojectstructure.Equals("1"))
                {
                    Folder[] folders = fascicoli.GetFascicoloTemplate(fascicolo.systemID, fascicolo.idClassificazione, infoUtente.idAmministrazione);
                    fascicolo.HasStrutturaTemplate = !(folders == null);
                }
            }
			fascicoli.Dispose();
			return fascicolo;
		}

      /// <summary>
		/// </summary>
		/// <param name="codiceFascicolo"></param>
		/// <param name="registro"></param>
		/// <param name="infoUtente"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
      public static ArrayList getListaFolderDaCodiceFascicolo(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo, string descrFolder, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione) 
		{

			ArrayList listaFolder = null;
			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

         listaFolder = fascicoli.GetFolderByCodFasc(infoUtente, codiceFascicolo, descrFolder, registro, enableUffRef, enableProfilazione);

         if (listaFolder == null)
			{
            logger.Debug("getListaFolderDaCodice = NULL: folder non trovato!!!");
								
			}

			fascicoli.Dispose();


         return listaFolder;
		}

      /// <summary>
		/// </summary>
		/// <param name="codiceFascicolo"></param>
		/// <param name="registro"></param>
		/// <param name="infoUtente"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
      public static ArrayList getListaFolderDaIdFascicolo(DocsPaVO.utente.InfoUtente infoUtente, string idFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione) 
		{

			ArrayList listaFolder = null;
			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

         listaFolder = fascicoli.GetFolderByIdFasc(infoUtente, idFascicolo, registro, enableUffRef, enableProfilazione);

         if (listaFolder == null)
			{
            logger.Debug("getListaFolderDaIdFascicolo = NULL: folder non trovato!!!");
								
			}

			fascicoli.Dispose();


         return listaFolder;
		}

		/// <summary>
		/// </summary>
		/// <param name="codiceFascicolo"></param>
		/// <param name="registro"></param>
		/// <param name="infoUtente"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static ArrayList getListaFascicoliDaCodice(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione, string insRic) 
		{

			ArrayList listaFascicoli = null;
			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            listaFascicoli = fascicoli.GetListaFascicoliDaCodice(infoUtente, codiceFascicolo, registro, enableUffRef, enableProfilazione, insRic);
			
			if(listaFascicoli == null)
			{
				logger.Debug("getListaFascicoliDaCodice = NULL: fascicolo non trovato!!!");
								
			}

			fascicoli.Dispose();
	
			
			return listaFascicoli;
		}


        public static DocsPaVO.fascicolazione.Fascicolo getFascicoloDaCodice(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione) 
		{

			DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            fascicolo = fascicoli.GetFascicoloDaCodice(infoUtente, codiceFascicolo, registro, enableUffRef, enableProfilazione);
			
			if(fascicolo == null)
			{
				logger.Debug("GetFascicoloDaCodice = NULL: fascicolo non trovato!!!");
							
			}

			fascicoli.Dispose();
			
			return fascicolo;
		}

	/// <summary>
	/// Questo metodo è stato aggiunto nella versione 3.1.6
	/// </summary>
	/// <param name="idAmministrazione"></param>
	/// <param name="idGruppo"></param>
	/// <param name="idPeople"></param>
	/// <param name="codiceFascicolo"></param>
	/// <param name="idRegistro"></param>
	/// <param name="enableUffRef"></param>
	/// <returns></returns>
        public static DocsPaVO.fascicolazione.Fascicolo getFascicoloInClassifica(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo, string idRegistro, bool enableUffRef, string idTitolario, bool enableProfilazione,string systemId) 
		{
			DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
			try
			{
	
				
				DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                fascicolo = fascicoli.GetFascicoloInClassifica(infoUtente, codiceFascicolo, idRegistro, enableUffRef, idTitolario, enableProfilazione, systemId);

				fascicoli.Dispose();
			}
			catch(Exception e) 
			{
				logger.Debug(e.Message);
				fascicolo = null;
			
			}
			return fascicolo;
		}

		/// <summary>
		/// </summary>
		/// <param name="objClassificazione"></param>
		/// <param name="infoUtente"></param>
		/// <param name="objListaFiltri"></param>
		/// <param name="childs"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static System.Collections.ArrayList getListaFascicoli(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Classificazione objClassificazione, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, bool enableUfficioRef, bool enableProfilazione, bool childs, byte[] datiExcel, string serverPath) 
		{
			#region Codice Commentato
			/*logger.Debug("getListaFascicoli");
			System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

			DocsPa_V15_Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			try 
			{
				db.openConnection();
				string queryString = getQueryFascicoli(infoUtente,objClassificazione.registro);
				if (objClassificazione != null)
				{
					if (childs)
						queryString += " AND A.VAR_CODICE LIKE '" + objClassificazione.codice +"%'";
					else
						queryString += " AND A.ID_PARENT = " + objClassificazione.systemID;
				}
					queryString += getSqlQuery(objListaFiltri);
				queryString += " ORDER BY A.SYSTEM_ID";
				logger.Debug(queryString);
				
				DataSet dataSet = new DataSet();
				db.fillTable(queryString, dataSet, "PROJECT");	

				//creazione della lista oggetti
				foreach(DataRow dataRow in dataSet.Tables["PROJECT"].Rows) 
				{
					listaFascicoli.Add(getFascicolo(dataSet, dataRow));
				}  
				dataSet.Dispose();
				db.closeConnection();
				
			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}		*/
			#endregion

			System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();
			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            listaFascicoli = fascicoli.GetListaFascicoli(infoUtente, objClassificazione, objListaFiltri, enableUfficioRef, enableProfilazione, childs, datiExcel, serverPath);
			
			if(listaFascicoli == null)
			{
				logger.Debug("Errore nella gestione dei fascicoli (getListaFascicoli)");
				throw new Exception("F_System");
				
			}

			fascicoli.Dispose();

			return listaFascicoli;
		}


		/// <summary>
		/// </summary>
		/// <param name="objClassificazione"></param>
		/// <param name="infoUtente"></param>
		/// <param name="objListaFiltri"></param>
		/// <param name="childs"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static System.Collections.ArrayList getListaFascicoli(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Classificazione objClassificazione, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, bool enableUfficioRef, bool enableProfilazione, bool childs, DocsPaVO.utente.Registro registro, byte[] datiExcel, string serverPath) 
		{
	
			System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();
			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            listaFascicoli = fascicoli.GetListaFascicoli(infoUtente, objClassificazione, objListaFiltri, enableUfficioRef, enableProfilazione, childs, registro, datiExcel, serverPath);
			
			if(listaFascicoli == null)
			{
				logger.Debug("Errore nella gestione dei fascicoli (getListaFascicoli)");
				throw new Exception("F_System");
				
			}

			fascicoli.Dispose();

			return listaFascicoli;
		}
		#region paging

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="objClassificazione"></param>
        /// <param name="registro"></param>
        /// <param name="filtriFascicoli"></param>
        /// <param name="filtriDocumentiInFascicoli"></param>
        /// <param name="enableUfficioRef"></param>
        /// <param name="enableProfilazione"></param>
        /// <param name="childs"></param>
        /// <param name="numTotPage"></param>
        /// <param name="nRec"></param>
        /// <param name="numPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="getSystemIdList">True se si desidera ricevere anche la lista dei system id dei fascicoli restituiti dalla ricerca</param>
        /// <param name="idProjectList">Lista dei system id dei fascicoli restituiti dalla ricerca. Verrà valorizzata solo se getSystemIdList è true</param>
        /// <returns></returns>
        public static ArrayList getListaFascicoliPaging(DocsPaVO.utente.InfoUtente infoUtente, 
                DocsPaVO.fascicolazione.Classificazione objClassificazione, 
                DocsPaVO.utente.Registro registro, 
                DocsPaVO.filtri.FiltroRicerca[] filtriFascicoli,
                DocsPaVO.filtri.FiltroRicerca[] filtriDocumentiInFascicoli, 
                bool enableUfficioRef, 
                bool enableProfilazione, 
                bool childs, 
                out int numTotPage, out int nRec, int numPage, int pageSize,
                bool getSystemIdList, out System.Collections.Generic.List<SearchResultInfo> idProjectList, byte[] datiExcel, string serverPath)
        {
            ArrayList listaFascicoli = null;

            using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
                listaFascicoli = fascicoli.GetListaFascicoliPaging(infoUtente, objClassificazione, registro, filtriFascicoli, filtriDocumentiInFascicoli, enableUfficioRef, enableProfilazione, childs, out numTotPage, out nRec, numPage, pageSize, getSystemIdList, out idProjectList, datiExcel, serverPath);
            
            if (listaFascicoli == null)
                throw new ApplicationException("Errore nella ricerca dei fascicoli");
            else
                return listaFascicoli;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objClassificazione"></param>
		/// <param name="infoUtente"></param>
		/// <param name="objListaFiltri"></param>
		/// <param name="childs"></param>
        /// <param name="getSystemIdList">True se si desidera ricevere anche la lista dei system id dei fascicoli restituiti dalla ricerca</param>
        /// <param name="idProjectList">Lista dei system id dei fascicoli restituiti dalla ricerca. Verrà valorizzata solo se getSystemIdList è true</param>
		/// <returns></returns>
        public static ArrayList getListaFascicoliPaging(DocsPaVO.utente.InfoUtente infoUtente, 
                DocsPaVO.fascicolazione.Classificazione objClassificazione, 
                DocsPaVO.utente.Registro registro, 
                DocsPaVO.filtri.FiltroRicerca[] filtriFascicoli,
                bool enableUfficioRef, 
                bool enableProfilazione, 
                bool childs,
                 out int numTotPage, out int nRec, int numPage, int pageSize,
                bool getSystemIdList, out System.Collections.Generic.List<SearchResultInfo> idProjectList, byte[] datiExcel, string serverPath)
        {
            return getListaFascicoliPaging(infoUtente, objClassificazione, registro, filtriFascicoli, null, enableUfficioRef, enableProfilazione, childs, out numTotPage, out nRec, numPage, pageSize, getSystemIdList, out idProjectList, datiExcel, serverPath);
        }

		#endregion

        public static DocsPaVO.fascicolazione.Fascicolo newFascicolo(DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, bool enableUffRef, out DocsPaVO.fascicolazione.ResultCreazioneFascicolo resultCreazione) 
		{
            logger.Info("BEGIN");
            resultCreazione = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.GENERIC_ERROR;

            try
            {
                if (semFasc.WaitOne())
                {
                    // Inizializzazione contesto transazionale
                    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                    {
                        DocsPaDocumentale.Documentale.ProjectManager projectManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);

                        // Ruoli superiori cui viene trasmessa la visibilità
                        DocsPaVO.utente.Ruolo[] ruoliSuperiori;
                        fascicolo = getDatiCreatore(fascicolo, ruolo, infoUtente);

                        bool created = projectManager.CreateProject(classificazione, fascicolo, ruolo, enableUffRef, out resultCreazione, out ruoliSuperiori);

                        if (created)
                        {
                            try
                            {
                                // Notifica evento fascicolo creato
                                DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(infoUtente);
                                eventsNotification.FascicoloCreatoEventHandler(classificazione, fascicolo, ruolo, ruoliSuperiori);

                                //Richiamo il metodo per il calcolo della atipicità del fascicolo
                                DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                                fascicolo.InfoAtipicita = documentale.CalcolaAtipicita(infoUtente, fascicolo.systemID, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO);
                                
                                // Completamento transazione
                                transactionContext.Complete();
                            }
                            catch (Exception ex)
                            {
                                fascicolo = null;
                                created = false;
                                resultCreazione = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.GENERIC_ERROR;

                                logger.Debug(string.Format("Errore nella creazione del fascicolo: {0}", ex.Message));
                            }
                        }
                        else
                            fascicolo = null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore nella creazione del fascicolo:\n{0}", ex.Message));

                fascicolo = null;
                resultCreazione = DocsPaVO.fascicolazione.ResultCreazioneFascicolo.GENERIC_ERROR;
            }
            finally
            {
                semFasc.ReleaseMutex();
            }
            logger.Info("END");
			return fascicolo;
		} 

        public static string getFascNumRif(string idTitolario, string idRegistro)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            string numFascicolo = "";
            if (!fascicoli.getFascNumRif(idTitolario, idRegistro, out numFascicolo))     
            {
                throw new Exception("Errore nel metodo: getFascNumRif");
            }
            return numFascicolo;
        }

		public static DocsPaVO.fascicolazione.Fascicolo updateFascicolo( DocsPaVO.fascicolazione.Fascicolo fascicolo) 
		{
			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			
			DocsPaVO.fascicolazione.Fascicolo result = fascicoli.UpdateFascicolo(fascicolo);

			if(result == null)
			{
				logger.Debug("Errore nella gestione del titolario. (updateFascicolo)");
				throw new Exception("Verificare il campo descrizione");				
			}

			return result;
		}

		
		/// <summary>
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="codTitolo"></param>
		/// <param name="data"></param>
		/// <param name="codFascicolo"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static string getCodiceFascicolo(string idAmm, string codTitolo, string data, string codFascicolo)
		{
			logger.Debug("getCodiceFascicolo");
			logger.Debug("codTitolo " + codTitolo);
			logger.Debug("data " + data);
			logger.Debug("codFascicolo " + codFascicolo);
			//string sep = DocsPaDB.Utils.Personalization.getInstance(idAmm).getSepFascicolo();
			//logger.Debug("sep = "+sep);
			
			string format = DocsPaDB.Utils.Personalization.getInstance(idAmm).FormatoFascicolatura;
			logger.Debug("format = " + format);
			
			format = format.Replace("COD_TITOLO", codTitolo);
			format = format.Replace("DATA_COMP", data.Substring(0,10));
			format = format.Replace("DATA_ANNO", data.Substring(6,4));
			format = format.Replace("NUM_PROG", codFascicolo);
	
			return format;
		}

		/// <summary>
		/// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fascicolo"></param>		
		public static void setFascicolo(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Fascicolo fascicolo) 
		{
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Interfaces.IProjectManager projectManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);
                
                //Verifica se si sta modificando il campo controllato per la tracciatura del log
                DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                string controllato = fasc.isControllatoModified(fascicolo.systemID);
                
                if (!projectManager.ModifyProject(fascicolo))
                {
                    string message = string.Format("Errore nell'aggiornamento del fascicolo con id {0}", fascicolo.systemID);
                    logger.Debug(message);
                    throw new Exception(message);
                }
                else
                {
                    // Controllo se il fascicolo ha una struttura template associata
                    string key_beprojectstructure = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");
                    if (!string.IsNullOrEmpty(key_beprojectstructure) && key_beprojectstructure.Equals("1"))
                    {
                        Folder[] folders = new DocsPaDB.Query_DocsPAWS.Fascicoli().GetFascicoloTemplate(fascicolo.systemID, fascicolo.idClassificazione, infoUtente.idAmministrazione);
                        fascicolo.HasStrutturaTemplate = !(folders == null);
                    }

                    //Richiamo il metodo per il calcolo della atipicità del fascicolo
                    DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                    fascicolo.InfoAtipicita = documentale.CalcolaAtipicita(infoUtente, fascicolo.systemID, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO);

                    transactionContext.Complete();
                    if (!string.IsNullOrEmpty(controllato) && (controllato != fascicolo.controllato))
                    {
                        //è stata fatta modifica sul campo controllato => tracciatura log
                        UserLog.UserLog.WriteLog(infoUtente, "FASCCONTROLLATO", fascicolo.codice, "Modificata proprietà controllato del fascicolo " + fascicolo.systemID, DocsPaVO.Logger.CodAzione.Esito.OK);
                    }
                }
            }
		}
	
		/// <summary>
		/// </summary>
		/// <param name="objInfoFascicolo"></param>
		/// <param name="objSicurezza"></param>
		/// <returns></returns>
		public static DocsPaVO.fascicolazione.Fascicolo getDettaglio(DocsPaVO.utente.InfoUtente infoUtente,DocsPaVO.fascicolazione.InfoFascicolo objInfoFascicolo, bool enableUffRef) 
		{
			#region Codice Commentato
			/*DocsPaVO.fascicolazione.Fascicolo objFascicolo = null;
			
			DocsPa_V15_Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			
			try 
			{
				db.openConnection();
				string queryString =
					"SELECT A.SYSTEM_ID, A.DESCRIPTION, A.CHA_TIPO_PROJ, A.VAR_CODICE, A.ID_AMM, " + 
					"A.NUM_LIVELLO, A.CHA_TIPO_FASCICOLO, A.ID_FASCICOLO, A.ID_PARENT, A.VAR_COD_ULTIMO, " +
					"A.VAR_NOTE, " + DocsPaWS.Utils.dbControl.toChar("A.DTA_APERTURA",false) +" AS DTA_APERTURA, " + DocsPaWS.Utils.dbControl.toChar("A.DTA_CHIUSURA",false) +" AS DTA_CHIUSURA, A.CHA_STATO, ID_TIPO_PROC " +
					"FROM PROJECT A, SECURITY B  WHERE A.SYSTEM_ID=B.THING " +
					"AND A.SYSTEM_ID = " + objInfoFascicolo.idFascicolo +
					" AND B.PERSONORGROUP=" + objSicurezza.idGruppo + "  AND B.ACCESSRIGHTS > 0";
				logger.Debug(queryString);
				
				DataSet dataSet = new DataSet();
				db.fillTable(queryString, dataSet, "PROJECT");	

				// TODO: calcolare l'intero oggetto classificazione
				DocsPaVO.fascicolazione.Classificazione objClassificazione = new  DocsPaVO.fascicolazione.Classificazione();
				objClassificazione.systemID = objInfoFascicolo.idClassificazione;
				
				DataRow dataRow = dataSet.Tables["PROJECT"].Rows[0];
				objFascicolo = getFascicolo(dataSet, dataRow);
				dataSet.Dispose();
				db.closeConnection();
			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}		
			
			return objFascicolo;*/
			//GetDettaglio(DocsPaVO.fascicolazione.InfoFascicolo objInfoFascicolo, DocsPaVO.utente.InfoUtente objSicurezza
			#endregion

			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            DocsPaVO.fascicolazione.Fascicolo objFascicolo = fascicoli.GetDettaglio(infoUtente, objInfoFascicolo, enableUffRef);
			//DocsPaDB.Query_DocsPAWS.Fascicoli.GetDettaglio( objInfoFascicolo, objSicurezza);
			
			if(objFascicolo == null)
			{
				logger.Debug("Errore nella gestione dei fascicoli (getDettaglio)");
				throw new Exception("F_System");				
			}

			return objFascicolo;
		}

		/// <summary>
		/// Reperimento numero classificazioni di un documento
		/// </summary>
		/// <param name="idProfile"></param>
		/// <param name="idGruppo"></param>
		/// <param name="idPeople"></param>
		/// <returns></returns>
		public static int GetCountClassificazioniDocumento(string idProfile,string idGruppo,string idPeople)
		{
			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();	

			return fascicoli.GetCountClassificazioniDocumento(idProfile,idGruppo,idPeople);
		}

		/// <summary>
		/// </summary>
		/// <param name="infoDocumento"></param>
		/// <param name="sicurezza"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static System.Collections.ArrayList getFascicoliDaDoc(DocsPaVO.utente.InfoUtente infoUtente, string idProfile) 
		{
			#region Codice Commentato
			/*			logger.Debug("getFascicoliDaDoc");
						System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();
			
						DocsPa_V15_Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
						try 
						{
							db.openConnection();
							string queryString =
								"SELECT DISTINCT A.SYSTEM_ID, A.DESCRIPTION, A.CHA_TIPO_PROJ, A.VAR_CODICE, A.ID_AMM, " + 
								"A.NUM_LIVELLO, A.CHA_TIPO_FASCICOLO, A.ID_FASCICOLO, A.ID_PARENT, A.VAR_COD_ULTIMO, " +
								DocsPaWS.Utils.dbControl.toChar("A.DTA_APERTURA",false) +" AS DTA_APERTURA, " + 
								DocsPaWS.Utils.dbControl.toChar("A.DTA_CHIUSURA",false) + " AS DTA_CHIUSURA, " +
								"A.CHA_STATO, A.ID_TIPO_PROC, A.VAR_NOTE, C.CHA_TIPO_DIRITTO " +
								"FROM PROJECT A, SECURITY C WHERE A.CHA_TIPO_PROJ = 'F' AND A.SYSTEM_ID IN " +
								"(SELECT A.ID_FASCICOLO FROM PROJECT A, PROJECT_COMPONENTS B WHERE A.SYSTEM_ID=B.PROJECT_ID AND B.LINK=" + infoDocumento.idProfile + ") " +
								" AND A.SYSTEM_ID=C.THING " +
								"AND (C.PERSONORGROUP="+sicurezza.idPeople+" OR C.PERSONORGROUP ="+sicurezza.idGruppo+") ORDER BY A.SYSTEM_ID";
							logger.Debug(queryString);
				
							DataSet dataSet = new DataSet();
							db.fillTable(queryString, dataSet, "PROJECT");	

							//creazione della lista oggetti
							foreach(DataRow dataRow in dataSet.Tables["PROJECT"].Rows) 
							{
								listaFascicoli.Add(getFascicolo(dataSet, dataRow));
							}  
							dataSet.Dispose();
							db.closeConnection();
						} 
						catch (Exception e) 
						{
							logger.Debug (e.Message);				
							db.closeConnection();
							throw new Exception("F_System");
						}		*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();	
			System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

            listaFascicoli = fascicoli.GetFascicoliDaDoc(infoUtente, idProfile);

			if(listaFascicoli == null)
			{
				logger.Debug("Errore nella gestione dei fascicoli (getFascicoliDaDoc)");
				throw new Exception("F_System");				
			}
			
			return listaFascicoli;
		}

        /// <summary>
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <param name="sicurezza"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getFascicoliDaDocNoSecurity(DocsPaVO.utente.InfoUtente infoUtente, string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

            listaFascicoli = fascicoli.GetFascicoliDaDocNoSecurity(infoUtente, idProfile);

            if (listaFascicoli == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli (getFascicoliDaDoc)");
                throw new Exception("F_System");
            }

            return listaFascicoli;
        }

		/// <summary>
		/// </summary>
		/// <param name="idProfile"></param>
		/// <param name="idFascicolo"></param>
		/// <param name="infoUtente"></param>
		/// <param name="debug"></param>
		//public static void addDocFascicolo(string idPeople, string idGruppo, string idProfile, string idFascicolo,bool fascRapida, out bool outValue) 
        public static bool addDocFascicolo(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string idFascicolo, bool fascRapida, out string msg)
		{
            DocsPaVO.fascicolazione.Folder folder = FolderManager.getFolder(infoUtente.idPeople, infoUtente.idGruppo, idFascicolo);

            bool outValue = false;
            msg = string.Empty;

			if(folder != null)
			{
                outValue = FolderManager.addDocFolder(infoUtente, idProfile, folder.systemID, fascRapida, out msg);
			}
			else 
			{
				logger.Debug("Errore nella gestione dei fascicoli. Root folder non trovato. (addDocFascicolo)");
				throw new Exception("Root folder non trovato");				
			}

            return outValue;
		}
	
		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="corr"></param>
		/// <param name="fasc"></param>
		/// <param name="debug"></param>
		public static void sospendiRiattivaUtente(string idPeople,DocsPaVO.utente.Corrispondente corr, DocsPaVO.fascicolazione.Fascicolo fasc)
		{
			#region Codice Commentato
			/*logger.Debug("sospendiRiattivaUtente");
			DocsPa_V15_Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
			bool dbOpen=false;	
			bool transOpen=false;
			try
			{
				db.openConnection();
				dbOpen=true;
				//si controlla se l'utente ha diritti di proprietario sul fascicolo
				string utenteString="SELECT CHA_TIPO_DIRITTO FROM SECURITY WHERE THING="+fasc.systemID+" AND PERSONORGROUP="+infoUtente.idPeople;
				logger.Debug(utenteString);
				string dirittoUtente=null;
				if(db.executeScalar(utenteString)!=null)
				{
					dirittoUtente=db.executeScalar(utenteString).ToString();
				}
				if(dirittoUtente!=null && dirittoUtente.Equals("P"))
				{
					//si controlla quali sono i diritti del corrispondente
					string idCorr="";
					if(corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
					{
						idCorr=((DocsPaVO.utente.Utente)corr).idPeople;
					}
					else
					{
						idCorr=((DocsPaVO.utente.Ruolo)corr).idGruppo;
					}
					logger.Debug("idCorr="+idCorr);
					
					string corrString="SELECT CHA_TIPO_DIRITTO FROM SECURITY WHERE THING="+fasc.systemID+" AND PERSONORGROUP="+idCorr;
					string dirittoCorr=db.executeScalar(corrString).ToString();
					logger.Debug("dirittoCorr="+dirittoCorr);
					if(dirittoCorr.Equals("S") || dirittoCorr.Equals("T"))
					{
						//bisogna eseguire degli update
						System.Collections.ArrayList updateQueries=new System.Collections.ArrayList();
						string updateString="";
						string tipoDiritto="";
						string tipoDirittoDoc="";
						string oldTipoDirittoDoc="";
						string accessRight="";
						if(dirittoCorr.Equals("S"))
						{
							tipoDiritto="T";
							tipoDirittoDoc="F";
							oldTipoDirittoDoc="S";
							accessRight="45";
						}
						else
						{
							tipoDiritto="S";
							tipoDirittoDoc="S";
							oldTipoDirittoDoc="F";
							accessRight="0";
						}
						updateString="UPDATE SECURITY SET CHA_TIPO_DIRITTO='"+tipoDiritto+"',ACCESSRIGHTS="+accessRight+" WHERE THING="+fasc.systemID+" AND PERSONORGROUP="+idCorr;
						logger.Debug(updateString);
						updateQueries.Add(updateString);
						
						//update dei documenti del fascicolo che verificano certe condizioni
						ArrayList docsUpdateList=getDocFolderFasc(fasc.systemID,oldTipoDirittoDoc);
						for(int i=0;i<docsUpdateList.Count;i++){
						   string updateDocString="UPDATE SECURITY SET CHA_TIPO_DIRITTO='"+tipoDirittoDoc+"',ACCESSRIGHTS="+accessRight+" WHERE THING="+docsUpdateList[i].ToString()+" AND PERSONORGROUP="+idCorr;
						   updateQueries.Add(updateDocString);
						}

						//si eseguono gli update
						db.beginTransaction();
						transOpen=true;
						for(int m=0;m<updateQueries.Count;m++){
						  db.executeNonQuery(updateQueries[m].ToString());
						  logger.Debug("Eseguita: "+updateQueries[m]);
						}
						db.commitTransaction();
						transOpen=false;
						db.closeConnection();
						dbOpen=false;
															
					}
				}
				else
				{
					db.closeConnection();
					//l'utente non è proprietario del fascicolo: eccezione
					throw new Exception("L'utente non ha i diritti necessari");				
				}
			}
			catch(Exception e)
			{
				if(transOpen)
				{
					db.rollbackTransaction();
				}
				if(dbOpen)
				{
					db.closeConnection();
				}
				logger.Debug(e.Message);
				throw new Exception("F_System");		
			}*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			
			if(! fascicoli.SospendiRiattivaUtente(idPeople,corr,fasc))
			{
				logger.Debug("Errore nella gestione dei fascicoli. Root folder non trovato. (sospendiRiattivaUtente)");
				throw new Exception("F_System");				
			}

			fascicoli.Dispose();
		}

		/// <summary>
		/// </summary>
		/// <param name="idFascicolo"></param>
		/// <param name="oldDiritto"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static ArrayList getDocFolderFasc(string idFascicolo,string oldDiritto)
		{
			#region Codice Commentato
			/*logger.Debug("getDocFolderFasc");
			DocsPa_V15_Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
			bool dbOpen=false;
			System.Data.DataSet ds=new System.Data.DataSet();
			System.Collections.ArrayList result=new System.Collections.ArrayList();
			try
			{
				db.openConnection();
				dbOpen=true;
				string queryFolderString="SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ='C' AND ID_FASCICOLO="+idFascicolo;
				db.fillTable(queryFolderString,ds,"FOLDER");
				System.Collections.ArrayList folderFascList=new System.Collections.ArrayList();
				
				//riempie l'array list delle folder del fascicolo
				for(int j=0;j<ds.Tables["FOLDER"].Rows.Count;j++)
				{
					folderFascList.Add(ds.Tables["FOLDER"].Rows[j]["SYSTEM_ID"].ToString());
				}

				//trova tutti i documenti appartenenti alle folders
				string queryDocString="SELECT DISTINCT A.LINK FROM PROJECT_COMPONENTS A, SECURITY B WHERE A.PROJECT_ID IN (";
				for(int j=0;j<folderFascList.Count;j++)
				{
					queryDocString=queryDocString+folderFascList[j].ToString();
					if(j<folderFascList.Count-1)
					{
						queryDocString=queryDocString+",";
					}
				}
				queryDocString=queryDocString+") AND B.CHA_TIPO_DIRITTO='"+oldDiritto+"' AND B.THING=A.LINK"; 
				logger.Debug(queryDocString);
				db.fillTable(queryDocString,ds,"DOCS");

				//si verifica se il documento appartiene ad altre folder diverse
				for(int k=0;k<ds.Tables["DOCS"].Rows.Count;k++)
				{
					System.Collections.ArrayList folderDocList=new System.Collections.ArrayList();
					string queryFolderDocString="SELECT PROJECT_ID FROM PROJECT_COMPONENTS WHERE LINK="+ds.Tables["DOCS"].Rows[k]["LINK"].ToString();
					logger.Debug(queryFolderDocString);
					db.fillTable(queryFolderDocString,ds,"FOLDER_DOC");
					//riempie l'array list delle folder
					for(int m=0;m<ds.Tables["FOLDER_DOC"].Rows.Count;m++)
					{
						folderDocList.Add(ds.Tables["FOLDER_DOC"].Rows[m]["PROJECT_ID"].ToString());
					}
					//si verifica se le folder del documento sono contenute nell'insieme delle folder del fascicolo
					if(isContained(folderDocList,folderFascList))
					{
						logger.Debug("Aggiunto doc "+ds.Tables["DOCS"].Rows[k]["LINK"].ToString());
						result.Add(ds.Tables["DOCS"].Rows[k]["LINK"].ToString());
					}		
					ds.Tables["FOLDER_DOC"].Reset();
				}
				return result;	
			}
			catch(Exception e)
			{
				if(dbOpen){
				  db.closeConnection();
				}
				logger.Debug(e.Message);
				throw new Exception("F_System");	
			}*/
			#endregion

			System.Collections.ArrayList result=new System.Collections.ArrayList();
			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			
			result = fascicoli.GetDocFolderFasc(idFascicolo,oldDiritto);

			if(result == null)
			{
				logger.Debug("Errore nella gestione dei fascicoli. (getDocFolderFascicolo)");
				throw new Exception("F_System");				
			}
			
			fascicoli.Dispose();

			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="list"></param>
		/// <param name="container"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static bool isContained(System.Collections.ArrayList list,System.Collections.ArrayList container)
		{
			logger.Debug("isContained");

			for(int i=0;i<list.Count;i++)
			{
				bool contained=false;
			
				for(int j=0;j<container.Count;j++)
				{
					if(list[i].ToString().Equals(container[j].ToString()))
					{
						contained=true;
					}
				}
				if(contained==false) return false;
			}

			return true;
		}

        public static ArrayList getFascicoloDaCodice3(string idAmministrazione, string idGruppo, string idPeople, string codiceFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione)
        {
            ArrayList listaFascicoli = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            listaFascicoli = fascicoli.GetFascicoloDaCodice3(idAmministrazione, idGruppo, idPeople, codiceFascicolo, registro, enableUffRef, enableProfilazione);

            if (listaFascicoli.Count == 0)
            {
                logger.Debug("getFascicoloDaCodice3 = NULL: fascicolo non trovato!!!");

            }

            fascicoli.Dispose();

            return listaFascicoli;
        }

        public static DocsPaVO.fascicolazione.Fascicolo getFascicoloDaCodice2(string idAmministrazione, string idGruppo, string idPeople, string codiceFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione, string idTitolario)
        {
		
            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            fascicolo = fascicoli.GetFascicoloDaCodice2(idAmministrazione, idGruppo, idPeople, codiceFascicolo, registro, enableUffRef, enableProfilazione, idTitolario);

            if (fascicolo == null)
            {
                logger.Debug("getFascicoloDaCodice2 = NULL: fascicolo non trovato!!!");

            }

            fascicoli.Dispose();

            return fascicolo;
        }

        public static bool IsDocumentoInFolder(string idProfile, string idFolder)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            return fascicoli.IsDocumentoClassificatoInFolder(idProfile, idFolder);
        }

        #region editing ACL
        public static bool EditingFascACL(out string descrizione, DocsPaVO.fascicolazione.DirittoOggetto fascDiritto, string personOrGroup, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                result = fasc.EditingFascACL(out descrizione, fascDiritto, personOrGroup, infoUtente);

                if (result)
                {
                    DocsPaDocumentale.Interfaces.IProjectManager projectManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);
                    result = projectManager.RemovePermission(fascDiritto);

                    if (result)
                        transactionContext.Complete();
                }
            }

            return result;
        }

        public static bool RipristinaFascACL(out string descrizione, DocsPaVO.fascicolazione.DirittoOggetto fascDiritto, string personOrGroup, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                result = fasc.RipristinaACL(out descrizione, fascDiritto, personOrGroup, infoUtente);

                if (result)
                {
                    DocsPaDocumentale.Interfaces.IProjectManager projectManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);
                    result = projectManager.AddPermission(fascDiritto);
                    
                    if (result)
                        transactionContext.Complete();
                }
            }

            return result;
        }

        
        #endregion

        #region trasferimento in deposito
        public static DocsPaVO.fascicolazione.Fascicolo getFascicoloDaArchiviare(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo)
        {
            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
            DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            fascicolo = fasc.GetFascicoloDaArchiviare(infoUtente, codiceFascicolo);

            if (fascicolo == null)
            {
                logger.Debug("getListaFascicoliDaCodice = NULL: fascicolo non trovato!!!");

            }
            fasc.Dispose();
            return fascicolo;
        }

        public static System.Collections.ArrayList getListaFascicoliDaArchiviare(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Classificazione objClassificazione, DocsPaVO.utente.Registro registro, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, bool enableUfficioRef, bool enableProfilazione, bool childs, out int numTotPage, out int nRec, int numPage, int pageSize)
        {

            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            listaFascicoli = fascicoli.GetListaFascicoliDaArchiviare(infoUtente, objClassificazione, registro, objListaFiltri, enableUfficioRef, enableProfilazione, childs, out numTotPage, out nRec, numPage, pageSize);

            if (listaFascicoli == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli (getListaFascicoliPaging)");
                throw new Exception("F_System");
            }
            fascicoli.Dispose();
            return listaFascicoli;
        }

        #endregion

        internal static DocsPaVO.fascicolazione.Fascicolo getDatiCreatore(DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.utente.InfoUtente objUtente)
        {
            if (fascicolo.creatoreFascicolo == null || fascicolo.creatoreFascicolo.idPeople.Equals(string.Empty))
            {
                fascicolo.creatoreFascicolo = new DocsPaVO.fascicolazione.CreatoreFascicolo(objUtente, objRuolo);
            }
            return fascicolo;
        }

        public static void creaRiscontroMittente(DocsPaVO.fascicolazione.RiscontroMittente riscontroMittente)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                    fascicoli.creaRiscontroMittente(riscontroMittente);
                    transactionContext.Complete();
                }
                catch(Exception ex)
                {
                    logger.Debug(string.Format("Errore nella creazione del riscontro : ", ex.Message));
                }
            }
        }

        public static void eliminaRiscontroMittente(DocsPaVO.fascicolazione.RiscontroMittente riscontroMittente)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                    fascicoli.eliminaRiscontroMittente(riscontroMittente);
                    transactionContext.Complete();
                }
                catch (Exception ex)
                {
                    logger.Debug(string.Format("Errore nella eliminazione del riscontro : ", ex.Message));
                }
            }
        }

        public static DocsPaVO.fascicolazione.RiscontroMittente cercaRiscontroMittente(DocsPaVO.fascicolazione.RiscontroMittente riscontroMittente)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                return fascicoli.cercaRiscontroMittente(riscontroMittente);                
            }
        }

        /// <summary>
        /// Funzione per il reperimento del numero di trasmissioni che coinvolgono un determinato fascicolo
        /// </summary>
        /// <param name="idProject">L'id del fascicolo di cui contarne le trasmissioni</param>
        /// <returns>Il numero di trasmissioni che coinvolgono il fascicolo</returns>
        public static int GetCountTrasmissioniFascicolo(int idProject)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return fasc.GetCountTrasmissioniFascicolo(idProject);
        }

        /// <summary>
        /// Ritorna true se l'utente ha visibilità sul fascicolo
        /// </summary>
    /*    public static bool fascIsVisibleByUser(string systemId, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return fasc.fascIsVisibleByUser(systemId, infoUtente);
        }
     * */

        /// <summary>
        /// Funzione per il reperimento dell'accessright di un fascicolo
        /// </summary>
        public static string getAccessRightFascBySystemID(string fascSystemId, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return fasc.getAccessRightFascBySystemID(fascSystemId, infoUtente);
        }
 
        public static string getRootFolderFasc(string idFasc)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return fasc.getRootFolderFasc(idFasc);
        }

        public static string GetFascicolazionePrimaria(DocsPaVO.utente.InfoUtente infoUtente, string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return fascicoli.GetFascicolazionePrimaria(infoUtente, idProfile);
        }

        public static bool CambiaFascicolazionePrimaria(DocsPaVO.utente.InfoUtente infoUtente , string idProject, string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return fascicoli.CambiaFascicolazionePrimaria(infoUtente, idProject, idProfile);
        }

        /// <summary>
        /// Funzione per verificare se un documento è fascicolato in un fascicolo / sottofascicolo
        /// </summary>
        /// <param name="userInfo">Informazioni sull'utente richiedente</param>
        /// <param name="idProfile">Id del documento da verificare</param>
        /// <param name="project">Id del fascicolo da verificare. Valorizzare folderSelezionato per effettuare la verifica di fascicolazione in un sottofascicolo</param>
        /// <returns>True se il documento è già fascicolato nel fascicolo / sottofascicolo</returns>
        public static bool IsDocumentInFolderOrSubFolder(DocsPaVO.utente.InfoUtente userInfo, String idProfile, DocsPaVO.fascicolazione.Fascicolo project)
        {
            bool retVal = false;

            DocsPaDB.Query_DocsPAWS.Fascicoli f = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            List<FascOrFolderBaseInfo> baseInfo =  f.GetBaseInfoFascOrFolderByIdDoc(idProfile);

            if (project.folderSelezionato != null)
                retVal = baseInfo.Where(e => e.Id == project.folderSelezionato.systemID).Count() > 0;
            else
                retVal = baseInfo.Where(e => e.Id == project.systemID).Count() > 0;

            return retVal;

        }

        public static ArrayList getListaFascicoliPagingCustom(DocsPaVO.utente.InfoUtente infoUtente,
        DocsPaVO.fascicolazione.Classificazione objClassificazione,
        DocsPaVO.utente.Registro registro,
        DocsPaVO.filtri.FiltroRicerca[] filtriFascicoli,
        bool enableUfficioRef,
        bool enableProfilazione,
        bool childs,
         out int numTotPage, out int nRec, int numPage, int pageSize,
        bool getSystemIdList, out System.Collections.Generic.List<SearchResultInfo> idProjectList, byte[] datiExcel, string serverPath, bool showGridPersonalization, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, bool security)
        {
            return getListaFascicoliPagingCustom(infoUtente, objClassificazione, registro, filtriFascicoli, null, enableUfficioRef, enableProfilazione, childs, out numTotPage, out nRec, numPage, pageSize, getSystemIdList, out idProjectList, datiExcel, serverPath, showGridPersonalization, export, visibleFieldsTemplate, documentsSystemId, security);
        }

        public static ArrayList getListaFascicoliPagingCustom(DocsPaVO.utente.InfoUtente infoUtente,
               DocsPaVO.fascicolazione.Classificazione objClassificazione,
               DocsPaVO.utente.Registro registro,
               DocsPaVO.filtri.FiltroRicerca[] filtriFascicoli,
               DocsPaVO.filtri.FiltroRicerca[] filtriDocumentiInFascicoli,
               bool enableUfficioRef,
               bool enableProfilazione,
               bool childs,
               out int numTotPage, out int nRec, int numPage, int pageSize,
               bool getSystemIdList, out System.Collections.Generic.List<SearchResultInfo> idProjectList, byte[] datiExcel, string serverPath, bool showGridPersonalization, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, bool security)
        {
            ArrayList listaFascicoli = null;

            using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
                listaFascicoli = fascicoli.GetListaFascicoliPagingCustom(infoUtente, objClassificazione, registro, filtriFascicoli, filtriDocumentiInFascicoli, enableUfficioRef, enableProfilazione, childs, out numTotPage, out nRec, numPage, pageSize, getSystemIdList, out idProjectList, datiExcel, serverPath, showGridPersonalization, export, visibleFieldsTemplate, documentsSystemId, security);

            if (listaFascicoli == null)
                throw new ApplicationException("Errore nella ricerca dei fascicoli");
            else
                return listaFascicoli;
        }

        public static SearchObject GetObjectFascicoloBySystemId(string systemId, InfoUtente infoUtente)
        {
            SearchObject result = null;
            using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
                result = fascicoli.GetObjectFascicoloBySystemId(systemId, infoUtente);

            return result;
        }


        /// <summary>
        /// Metodo per verificare se un fascicolo è generale
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="idFascicolo">Id del fascicolo da verificare</param>
        /// <returns>True se è generale</returns>
        public static bool IsFascicoloGenerale(DocsPaVO.utente.InfoUtente userInfo, String idFascicolo)
        {
            bool retVal = false;

            using (DocsPaDB.Query_DocsPAWS.Fascicoli prjManager = new DocsPaDB.Query_DocsPAWS.Fascicoli())
            {
                retVal = prjManager.IsFascicoloGeneraleFromIdFascicolo(idFascicolo);
            }

            return retVal;
        }

        public static DocsPaVO.fascicolazione.Fascicolo[] GetFascicoloDaCodiceNoSecurity(string codiceFasc, string idAmm, string titolario, bool soloGenerali)
        {
            DocsPaVO.fascicolazione.Fascicolo[] retValue = null;

            using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
                retValue = fascicoli.GetFascicoloDaCodiceNoSecurity(codiceFasc, idAmm, titolario, soloGenerali);

            return retValue;
        }

        /// <summary>
        /// MEV CS 1.4
        /// Gabriele Melini 07/11/2013
        /// Metodo duplicato per le ricerche per codice fascicolo in Conservazione/Esibizione
        /// aggiunta nella where la condizione CHA_TIPO_PROJ='F'
        /// per evitare la duplicazione dei risultati
        /// </summary>
        /// <param name="codiceFasc"></param>
        /// <param name="idAmm"></param>
        /// <param name="titolario"></param>
        /// <param name="soloGenerali"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Fascicolo[] GetFascicoloDaCodiceNoSecurityConservazione(string codiceFasc, string idAmm, string titolario, bool soloGenerali, bool isRicFasc, string idRegistro)
        {
            DocsPaVO.fascicolazione.Fascicolo[] retValue = null;

            using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
                retValue = fascicoli.GetFascicoloDaCodiceNoSecurityConservazione(codiceFasc, idAmm, titolario, soloGenerali, isRicFasc, idRegistro);

            return retValue;
        }

        public static DocsPaVO.fascicolazione.Fascicolo[] GetFascicoloDaCodiceNoSecurityConservazione(string codiceFasc, string idAmm, string titolario, bool soloGenerali, bool isRicFasc)
        {
            return GetFascicoloDaCodiceNoSecurityConservazione(codiceFasc, idAmm, titolario, soloGenerali, isRicFasc, string.Empty);
        }

        /// <summary>
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <param name="registro"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Fascicolo getFascicoloByIdNoSecurity(string idFascicolo)
        {
            logger.Debug("getFascicoloById. idFascicolo: " + idFascicolo);

            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            fascicolo = fascicoli.getFascicoloByIdNoSecurity(idFascicolo);

            if (fascicolo == null)
            {
                logger.Debug("getFascicoloById = NULL: fascicolo non trovato!!!");
            }
            fascicoli.Dispose();
            return fascicolo;
        }


        public static DocsPaVO.fascicolazione.Fascicolo[] GetFascicoloDaCodiceConSecurity(string codiceFasc, string idAmm, string titolario, bool soloGenerali, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.fascicolazione.Fascicolo[] retValue = null;

            using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
                retValue = fascicoli.GetFascicoloDaCodiceConSecurity(codiceFasc, idAmm, titolario, soloGenerali, infoUtente);

            return retValue;
        }

        public static DocsPaVO.documento.Tab GetProjectTab(string projectId, InfoUtente infoUser)
        {
            using (DocsPaDB.Query_DocsPAWS.Fascicoli Fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
            {
                return Fascicoli.GetProjectTab(projectId, infoUser);
            }
        }

        public static string GetChaStateProject(string idProject)
        {
            logger.Debug("GetChaStateProject");
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            return fascicoli.GetChaStateProject(idProject);
        }

        public static CreatoreFascicolo GetCreatoreFascicolo(string systemID)
        {
            logger.Debug("getCreatoreFascicolo");
            return new DocsPaDB.Query_DocsPAWS.Fascicoli().GetCreatoreFascicolo(systemID);
        }
    }
}
