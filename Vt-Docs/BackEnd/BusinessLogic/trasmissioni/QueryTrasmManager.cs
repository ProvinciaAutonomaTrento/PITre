using System;
using System.Collections;
using System.Data;
using System.Configuration;
using log4net;
using System.Linq;


namespace BusinessLogic.Trasmissioni
{
    /// <summary>
    /// </summary>
    public class QueryTrasmManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(QueryTrasmManager));
        private static Hashtable utenti = new Hashtable();
        private static Hashtable ruoli = new Hashtable();
        private static Hashtable corrispondenti = new Hashtable();
        private static Hashtable ragioni = new Hashtable();

        #region Personali

        //		/// <summary>
        //		/// 
        //		/// </summary>
        //		/// <param name="objOggettoTrasmesso"></param>
        //		/// <param name="objUtente"></param>
        //		/// <param name="objRuolo"></param>
        //		/// <param name="objListaFiltri"></param>
        //		/// <returns></returns>
        //		private static System.Collections.ArrayList getQueryEffettuateMethod_part1(DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri) 
        //		{
        //			
        //			logger.Debug("getQueryEffettuateMethod_part1");
        //			System.Collections.ArrayList lista = new System.Collections.ArrayList();
        //			// apro la connessione			
        //			//DocsPa_V15_Utils.database.SqlServerAgent db = new DocsPa_V15_Utils.database.SqlServerAgent();
        //			//db.openConnection();
        //
        //			try 
        //			{
        //				#region Codice Commentato	
        //				//string queryString = "";
        //				/*
        //				// leggo il valore del corrispondente associato all'utente
        //				queryString = 
        //					"SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = " + objUtente.idPeople;
        //				logger.Debug(queryString);				
        //				string idCorrispondente = db.executeScalar(queryString).ToString();
        //				*/
        //				#endregion 
        //
        //				DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
        //				string idCorrispondente = strDB.getQueryEffMet1(objUtente.idPeople);				
        //				
        //				DataSet dataSet;
        //
        //				bool repeatQuery = false;
        //
        //				//strDB.getQueryTrasmEff(out dataSet,objListaFiltri,objOggettoTrasmesso, objRuolo,objUtente,idCorrispondente,ref repeatQuery);
        //				strDB.GetQueryTrasmEffettuate(out dataSet,objListaFiltri,objOggettoTrasmesso, objRuolo,objUtente,idCorrispondente,ref repeatQuery);
        //				#region Codice Commentato
        //				/*
        //				DocsPa_V15_Utils.Query query = getQueryTrasmissioni(objOggettoTrasmesso);
        //				bool repeatQuery = getCondFiltri(ref query, objListaFiltri);
        //				bool inf = cercaInf(objListaFiltri);
        //				
        //				query.Where += " AND ((";
        //
        //				//ricevute
        //				query.Where +=
        //					" C.CHA_VALIDA='1' AND A.DTA_INVIO IS NOT NULL) AND " +
        //					" (A.SYSTEM_ID IN (SELECT B.ID_TRASMISSIONE FROM DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C WHERE B.SYSTEM_ID=C.ID_TRASM_SINGOLA AND (B.ID_CORR_GLOBALE = " + objRuolo.systemId + " AND C.ID_PEOPLE=" + objUtente.idPeople + ") OR B.ID_CORR_GLOBALE=" + idCorrispondente;
        //				if (inf && !ruoliInf.Equals("()"))//2004-02-13 mdigregorio:modificato per errore su tipoRuolo di ultimo liv 
        //					query.Where += " OR B.ID_CORR_GLOBALE IN " + ruoliInf;				
        //				query.Where += "))) OR (";
        //				
        //				//effettuate
        //				getCondVisibilitaEffettuate(ref query, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);
        //				
        //				query.Where += ")";
        //				
        //				queryString = query.getQuery();
        //				logger.Debug(queryString);				
        //				
        //				db.fillTable(queryString, dataSet, "TRASMISSIONI");	
        //                */
        //				#endregion
        //						
        //				lista = getListaTrasmissioni (dataSet, repeatQuery, "E", objOggettoTrasmesso, objUtente, objRuolo);
        //				dataSet.Dispose();
        //
        //			} 
        //			catch (Exception e) {
        //				logger.Debug (e.Message);
        //				//db.closeConnection();
        //				logger.Debug("Errore nella gestione delle trasmissioni (getQueryEffettuateMethod)",e);
        //				throw new Exception(e.Message);
        //			}
        //
        //			#region Codice Commentato
        //			// chiudo le connessioni
        //			//db.closeConnection();
        //	
        //			//logger.Debug(listaIdTrasm.Count.ToString());
        //			//throw new Exception("Stop");
        //			#endregion 
        //
        //			return lista;
        //		}
        #endregion Personali

        #region Ricevute
        /// <summary>
        /// </summary>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getQueryRicevuteMethod(DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            logger.Debug("getQueryRicevuteMethod");

            System.Collections.ArrayList lista = new System.Collections.ArrayList();
            // apro la connessione			
            //DocsPa_V15_Utils.database.SqlServerAgent db = new DocsPa_V15_Utils.database.SqlServerAgent();
            //db.openConnection();
            try
            {
                DataSet dataSet;
                bool repeatQuery = false;
                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                strDB.getQueryRicMet(ref repeatQuery, out dataSet, objUtente.idPeople, objRuolo, objListaFiltri, objOggettoTrasmesso);

                #region codice originale
                /*DocsPa_V15_Utils.Query query = getQueryTrasmissioni(objOggettoTrasmesso);
				string queryString = "";
				// leggo il valore del corrispondente associato all'utente
				queryString = 
					"SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = " + objUtente.idPeople;
				logger.Debug(queryString);
				string idCorrispondente = db.executeScalar(queryString).ToString();
								
				query.Where +=
					" AND C.CHA_VALIDA='1' AND A.DTA_INVIO IS NOT NULL " +
					"AND ((B.ID_CORR_GLOBALE = " + objRuolo.systemId + " AND C.ID_PEOPLE=" + objUtente.idPeople + ") OR B.ID_CORR_GLOBALE=" + idCorrispondente + ")";
				
				bool repeatQuery = getCondFiltri(ref query, objListaFiltri);
								
				queryString = query.getQuery();
				logger.Debug(queryString);				
				DataSet dataSet = new DataSet();
				
				db.fillTable(queryString, dataSet, "TRASMISSIONI");	
				*/
                # endregion
                //bool inf = cercaInf(objListaFiltri);
                bool inf = false;
                lista = getListaTrasmissioni(dataSet, repeatQuery, "R", inf, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);
                dataSet.Dispose();

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                logger.Debug("Errore nella gestione delle trasmissioni (getQueryRicevuteMethod)", e);
                throw new Exception(e.Message);
            }
            // chiudo le connessioni
            //db.closeConnection();
            return lista;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getQueryRicevuteMethodPaging(
                                DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                DocsPaVO.utente.Utente objUtente,
                                DocsPaVO.utente.Ruolo objRuolo,
                                DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                int pageNumber,
                                out int totalPageNumber,
                                out int recordCount)
        {

            totalPageNumber = 0;
            recordCount = 0;

            logger.Debug("getQueryRicevuteMethod");

            System.Collections.ArrayList lista = new System.Collections.ArrayList();

            try
            {
                DataSet dataSet;
                bool repeatQuery = false;
                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                strDB.getQueryTrasmRicPaging(ref repeatQuery, out dataSet, objUtente, objRuolo, objListaFiltri, objOggettoTrasmesso, pageNumber, out totalPageNumber, out recordCount);

                bool inf = false;
                lista = getListaTrasmissioni(dataSet, repeatQuery, "R", inf, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione delle trasmissioni (getQueryRicevuteMethod)", e);
                throw new Exception(e.Message);
            }

            return lista;
        }



        public static System.Collections.ArrayList getQueryDettaglioTrasmMethod(
               DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente,
               DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, string systemIDTrasm)
        {

            logger.Debug("getQueryDettaglioTrasmMethod");
            System.Collections.ArrayList lista = new System.Collections.ArrayList();

            try
            {
                DataSet dataSet;
                bool repeatQuery = false;
                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                // prima query ritorna un bool !?!?
                strDB.getDettaglioTrasmissione(ref repeatQuery, out dataSet, objUtente.idPeople, objRuolo, objListaFiltri, objOggettoTrasmesso, systemIDTrasm);

                bool inf = false;
                // seconda query 
                lista = getListaTrasmissioni(dataSet, repeatQuery, "R", inf, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione delle trasmissioni (getQueryDettaglioTrasmMethod)", e);
                throw new Exception(e.Message);
            }

            return lista;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getQueryRicevuteMethodPagingLite(
                                DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                DocsPaVO.utente.Utente objUtente,
                                DocsPaVO.utente.Ruolo objRuolo,
                                DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                int pageNumber,
                                bool excel,
                                int pageSize,
                                out int totalPageNumber,
                                out int recordCount)
        {

            totalPageNumber = 0;
            recordCount = 0;

            logger.Debug("getQueryRicevuteMethodPagingLite");

            System.Collections.ArrayList lista = new System.Collections.ArrayList();

            try
            {
                DataSet dataSet;
                bool repeatQuery = false;
                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                strDB.getQueryTrasmRicPagingLite(ref repeatQuery, out dataSet, objUtente, objRuolo, objListaFiltri, objOggettoTrasmesso, pageNumber, excel,pageSize, out totalPageNumber, out recordCount);

                bool inf = false;
                lista = getListaTrasmissioniLite(dataSet, repeatQuery, "R", inf, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri, excel);
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione delle trasmissioni (getQueryRicevuteMethodPagingLite)", e);
                throw new Exception(e.Message);
            }

            return lista;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getQueryRicevuteMethodPagingLiteWithoutTrasmUtente(
                                DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                DocsPaVO.utente.Utente objUtente,
                                DocsPaVO.utente.Ruolo objRuolo,
                                DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                int pageNumber,
                                bool excel,
                                int pageSize,
                                out int totalPageNumber,
                                out int recordCount)
        {

            totalPageNumber = 0;
            recordCount = 0;

            logger.Debug("getQueryRicevuteMethodPagingLite");

            System.Collections.ArrayList lista = new System.Collections.ArrayList();

            try
            {
                DataSet dataSet;
                bool repeatQuery = false;
                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                strDB.getQueryTrasmRicPagingLiteWithoutTrasmUtente(ref repeatQuery, out dataSet, objUtente, objRuolo, objListaFiltri, objOggettoTrasmesso, pageNumber, excel, pageSize, out totalPageNumber, out recordCount);

                bool inf = false;
                lista = getListaTrasmissioniLiteWithoutTrasmUtente(dataSet, repeatQuery, "R", inf, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri, excel);
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione delle trasmissioni (getQueryRicevuteMethodPagingLite)", e);
                throw new Exception(e.Message);
            }

            return lista;
        }

        #endregion Ricevute

        #region Effettuate

        #region Metodo Commentato
        //		/// <summary>
        //		/// 
        //		/// </summary>
        //		/// <param name="objOggettoTrasmesso"></param>
        //		/// <param name="objUtente"></param>
        //		/// <param name="objRuolo"></param>
        //		/// <param name="objListaFiltri"></param>
        //		/// <returns></returns>
        //		public static System.Collections.ArrayList getQueryEffettuateMethod(DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri) 
        //		{
        //			string l_typeQuerySetting = null;
        //			
        //			try {
        //				l_typeQuerySetting = ConfigurationManager.AppSettings["TRASMISSIONI_QUERY_PARTICOLARE"].ToString();
        //			} catch (Exception) {}
        //
        //			if ((l_typeQuerySetting==null)||(l_typeQuerySetting=="0".ToString())) {
        //				return getQueryEffettuateMethod_def(objOggettoTrasmesso,objUtente,objRuolo,objListaFiltri);
        //			}
        //			else if(l_typeQuerySetting=="1".ToString()) {
        //				return getQueryEffettuateMethod_part1(objOggettoTrasmesso,objUtente,objRuolo,objListaFiltri);
        //			}
        //			else {
        //				return null;
        //			}			
        //		}
        #endregion

        /// <summary>
        /// </summary>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getQueryEffettuateMethod_def(DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            logger.Debug("getQueryEffettuateMethod_def");
            System.Collections.ArrayList lista = new System.Collections.ArrayList();
            // apro la connessione			
            //DocsPa_V15_Utils.database.SqlServerAgent db = new DocsPa_V15_Utils.database.SqlServerAgent();
            //db.openConnection();

            try
            {
                DataSet dataSet;
                bool repeatQuery = false;

                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                strDB.getQueryEffMetdef(ref repeatQuery, out dataSet, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);

                //int totalPageNumber,recordCount;
                //strDB.getQueryTrasmEffPaging(ref repeatQuery, out dataSet, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri, 1,out totalPageNumber,out recordCount);

                #region Codice Commentato
                /*
				DocsPa_V15_Utils.Query query = getQueryTrasmissioni(objOggettoTrasmesso);
				
				query.Where += " AND ";
				getCondVisibilitaEffettuate(ref query, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);
				bool repeatQuery = getCondFiltri(ref query, objListaFiltri);

				string queryString = query.getQuery();
				logger.Debug(queryString);
	
				DataSet dataSet = new DataSet();
				
				db.fillTable(queryString, dataSet, "TRASMISSIONI");	
				*/
                #endregion

                lista = getListaTrasmissioni(dataSet, repeatQuery, "E", false, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);
                dataSet.Dispose();

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                logger.Debug("Errore nella gestione delle trasmissioni (getQueryEffettuateMethod_def)", e);
                throw new Exception(e.Message);
            }

            #region Codice Commentato
            // chiudo le connessioni
            //db.closeConnection();

            //logger.Debug(listaIdTrasm.Count.ToString());
            //throw new Exception("Stop");
            #endregion

            return lista;
        }

        /// <summary>
        ///	Reperimento di tutte le trasmissioni effettuate (e paginate)
        ///	relativamente ad un singolo documento, indipendentemente
        ///	dal ruolo e utente correntemente connesso
        /// </summary>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getQueryEffettuateDocMethodPaging(
                                    DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                    DocsPaVO.utente.Utente objUtente,
                                    DocsPaVO.utente.Ruolo objRuolo,
                                    DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                    int pageNumber,
                                    out int totalPageNumber,
                                    out int recordCount)
        {
            return getQueryEffettuateMethodPagingInternal(objOggettoTrasmesso,
                                                            objUtente,
                                                            objRuolo,
                                                            objListaFiltri,
                                                            false,
                                                            pageNumber,
                                                            out totalPageNumber,
                                                            out recordCount);
        }

        /// <summary>
        ///	Reperimento di tutte le trasmissioni effettuate (e paginate)
        /// </summary>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getQueryEffettuateMethodPaging(
                                                    DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                                    DocsPaVO.utente.Utente objUtente,
                                                    DocsPaVO.utente.Ruolo objRuolo,
                                                    DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                                    int pageNumber,
                                                    out int totalPageNumber,
                                                    out int recordCount)
        {
            return getQueryEffettuateMethodPagingInternal(objOggettoTrasmesso,
                                                            objUtente,
                                                            objRuolo,
                                                            objListaFiltri,
                                                            true,
                                                            pageNumber,
                                                            out totalPageNumber,
                                                            out recordCount);
        }

        private static System.Collections.ArrayList getQueryEffettuateMethodPagingInternal(
                                    DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                    DocsPaVO.utente.Utente objUtente,
                                    DocsPaVO.utente.Ruolo objRuolo,
                                    DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                    bool ricercaCondizioniVisibilitaUtente,
                                    int pageNumber,
                                    out int totalPageNumber,
                                    out int recordCount)
        {
            logger.Debug("getQueryEffettuateMethodPaging");
            System.Collections.ArrayList lista = new System.Collections.ArrayList();

            try
            {
                bool repeatQuery = false;

                DataSet dataSet = null;

                // Esecuzione query paginata
                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                if (ricercaCondizioniVisibilitaUtente)
                {
                    //ABBATANGELI GIANLUIGI - Aggiunto parametro idAmministrazione per il caricamento del limite massimo di righe accettate come risultato di ricerca
                    strDB.getQueryTrasmEffPaging(ref repeatQuery, out dataSet, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri, pageNumber, out totalPageNumber, out recordCount, objUtente.idAmministrazione);
                }
                else
                {
                    //ABBATANGELI GIANLUIGI - Aggiunto parametro idAmministrazione per il caricamento del limite massimo di righe accettate come risultato di ricerca
                    strDB.getQueryTrasmEffPaging(ref repeatQuery, out dataSet, objOggettoTrasmesso, objListaFiltri, pageNumber, objUtente.idAmministrazione, out totalPageNumber, out recordCount);
                }

                lista = getListaTrasmissioni(dataSet, repeatQuery, "E", false, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);

                dataSet.Dispose();
                dataSet = null;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                logger.Debug("Errore nella gestione delle trasmissioni (getQueryEffettuateMethodPaging)", e);
                throw new Exception(e.Message);
            }

            return lista;
        }

        #region Metodi Commentati
        /*
		private static string getListaRuoliInf (DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Ruolo objRuolo, bool cercaInf) {
			logger.Debug("getListaRuoliInf");
			string ret = "(";
			int i;
			logger.Debug("cercaInf = " + cercaInf);
			
			if(cercaInf) {
				ArrayList listaRuoliInf ;
			
				DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();

				if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoDocumento != null) 
					listaRuoliInf = gerarchia.getGerarchiaInf(objRuolo,objOggettoTrasmesso.infoDocumento.idRegistro,null,DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO);
				else if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoFascicolo != null) 
					listaRuoliInf = gerarchia.getGerarchiaInf(objRuolo,objOggettoTrasmesso.infoFascicolo.idRegistro,objOggettoTrasmesso.infoFascicolo.idClassificazione,DocsPaVO.trasmissione.TipoOggetto.FASCICOLO);
				else 
					listaRuoliInf = gerarchia.getGerarchiaInf(objRuolo,null,null,DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO);	
				
				if (listaRuoliInf == null)
				{
				  throw new Exception("Errore in : getListaRuoliInf");
				}
				if(listaRuoliInf.Count > 0)
					ret += ((DocsPaVO.utente.Ruolo)listaRuoliInf[0]).systemId;
				for (i = 1; i < listaRuoliInf.Count; i++)
					ret += ", " + ((DocsPaVO.utente.Ruolo)listaRuoliInf[i]).systemId;
				
			}
			ret += ")";
			return ret;
		}
		*/

        /*
        private static void getCondVisibilitaEffettuate(ref DocsPa_V15_Utils.Query query, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri) {
            bool inf = cercaInf(objListaFiltri);
            query.Where +=
                " ((A.ID_RUOLO_IN_UO = " + objRuolo.systemId + 
                " AND A.ID_PEOPLE=" + objUtente.idPeople + ")";
//2004-02-13 mdigregorio:modificato per errore su tipoRuolo di ultimo liv 
            string ruoliInf = getListaRuoliInf(objOggettoTrasmesso, objRuolo, inf);
            if(inf && !(ruoliInf.Equals("()"))) 
                query.Where += "OR A.ID_RUOLO_IN_UO IN " + ruoliInf;
//end modifica
            query.Where += ")";
        }
        */
        #endregion

        #region Trasmissioni Non Concluse

        /*
		private static DocsPa_V15_Utils.Query getQueryTrasmissioniRichiedentiRisposta(DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso) {
			DocsPa_V15_Utils.Query query = getQueryTrasmissioni(objOggettoTrasmesso);
			query.From += ", DPA_RAGIONE_TRASM D ";
			query.Where += " AND B.ID_RAGIONE=D.SYSTEM_ID AND D.CHA_RISPOSTA='1' ";
			return query;
		}
		*/

        private static string getNomeColonnaOggetto(DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            for (int i = 0; i < objListaFiltri.Length; i++)
            {
                if (objListaFiltri[i].argomento.Equals("TIPO_OGGETTO"))
                {
                    if (objListaFiltri[i].valore.Equals("F"))
                        return "A.ID_PROJECT";
                    else
                        return "A.ID_PROFILE";
                }
            }
            return "A.ID_PROFILE";
        }

        /*
        private static string getQueryOggettiCompletati(DocsPaVO.filtri.FiltroRicerca[] objListaFiltri) {
            string col = getNomeColonnaOggetto(objListaFiltri);
            string queryStr = 
                "SELECT " + col + " FROM DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_RAGIONE_TRASM D " +
                "WHERE A.SYSTEM_ID=B.ID_TRASMISSIONE AND B.ID_RAGIONE=D.SYSTEM_ID AND D.CHA_TIPO_RISPOSTA='C'";
            return queryStr;
        }
        */

        #endregion Trasmissioni Non Concluse

        #endregion Effettuate

        #region Trasmissioni

        #region Metodi Commentati
        /*
		internal static DocsPa_V15_Utils.Query getQueryTrasmissioni(DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso) {
			DocsPa_V15_Utils.Query query = new DocsPa_V15_Utils.Query();
			query.Columns = "SELECT ";
			// trasmissioni
			query.Columns +=
				"A.ID_RUOLO_IN_UO, A.ID_PEOPLE, A.CHA_TIPO_OGGETTO, " +
				"A.ID_PROFILE, A.ID_PROJECT, " + 
				DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO",false) + " AS DTA_INVIO, " +
				"A.VAR_NOTE_GENERALI, ";
			// trasmissioni singole
			query.Columns +=
				"B.ID_RAGIONE, B.ID_TRASMISSIONE, B.ID_TRASM_UTENTE, B.CHA_TIPO_DEST, " +
				"B.ID_CORR_GLOBALE, B.VAR_NOTE_SING, B.CHA_TIPO_TRASM, " +
				DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA",false) + " AS DTA_SCADENZA, "; 
			// trasmissioni utente
			query.Columns +=
				"C.SYSTEM_ID AS ID_TRASMISSIONE_UTENTE, C.ID_PEOPLE AS ID_DESTINATARIO, " + 
				DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_VISTA",false) + " AS DTA_VISTA,CHA_VISTA, " + 
				DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_ACCETTATA",false) + " AS DTA_ACCETTATA,CHA_ACCETTATA, " + 
				DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RIFIUTATA",false) + " AS DTA_RIFIUTATA,CHA_RIFIUTATA, "+
				"C.VAR_NOTE_RIF, C.VAR_NOTE_ACC, C.ID_TRASM_SINGOLA, C.CHA_VALIDA";
	
			query.From = "FROM DPA_TRASMISSIONE A, DPA_TRASM_SINGOLA B, DPA_TRASM_UTENTE C";

			query.Where =
				"WHERE A.SYSTEM_ID=B.ID_TRASMISSIONE AND B.SYSTEM_ID=C.ID_TRASM_SINGOLA";
 
			query.OrderBy = 
				"ORDER BY A.DTA_INVIO DESC, B.ID_TRASMISSIONE DESC, C.SYSTEM_ID";
			
			getCondOggettoTrasmesso(ref query, objOggettoTrasmesso);

			return query;
		}
		
		private static void getCondOggettoTrasmesso(ref DocsPa_V15_Utils.Query query, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso) {
			if (objOggettoTrasmesso == null)
				return;
			string whereStr = null;
			bool doc = false;

			// condizione sui documenti
			if (objOggettoTrasmesso.infoDocumento != null) {
				whereStr = "A.ID_PROFILE=" + objOggettoTrasmesso.infoDocumento.idProfile;
				doc = true;
			}

			//condizione sui fascicoli
			if (objOggettoTrasmesso.infoFascicolo != null) {
				if (doc) whereStr += " OR ";
				whereStr += "A.ID_PROJECT=" + objOggettoTrasmesso.infoFascicolo.idFascicolo;
			}

			if(whereStr != null) {
				query.Where += " AND ";
				if (doc) query.Where += "(";
				query.Where += whereStr;
				if (doc) query.Where += ") ";
			}
		}
		*/

        /*  -- ATTENZIONE! sembra che questo metodo non venga più usato!!!  ---
		 
        private static DataSet getDocumentiTrasmissioni(DocsPa_V15_Utils.database.SqlServerAgent db, DataSet dataSet, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo) 
        {
            logger.Debug("getDocumentiTrasmissioni");
            ArrayList idProfileList = new ArrayList();
			
            string queryWhere = "";
            string queryFrom = "";
            string queryColumns = "";

            //DocsPa_V15_Utils.Query query = DocsPaWS.documenti.InfoDocManager.getQueryDocumento(new DocsPaVO.utente.InfoUtente(utente, ruolo),ref queryWhere, ref queryFrom, ref queryColumns);
            DocsPaWS.documenti.InfoDocManager.getQueryDocumento(new DocsPaVO.utente.InfoUtente(utente, ruolo),ref queryWhere, ref queryFrom, ref queryColumns);

            string idTrasmissione = "";			
            foreach (DataRow trasmissioneRow in dataSet.Tables["TRASMISSIONI"].Rows) {
                if(!idTrasmissione.Equals(trasmissioneRow["ID_TRASMISSIONE"].ToString())) 
                    idProfileList.Add(trasmissioneRow["ID_PROFILE"].ToString());
            }

            if(idProfileList.Count > 0) {
				
                string inStr = "(" + (string)idProfileList[0];
                for (int i = 1; i < idProfileList.Count; i++)
                    inStr += ", " + (string)idProfileList[i];
                inStr += ")";
                query.Where += " AND A.SYSTEM_ID IN " + inStr;
                string queryString = query.getQuery();				
                logger.Debug(queryString);
                db.fillTable(queryString, dataSet, "DOCUMENTI");
				
            }			

            DocsPa_V15_Utils.Logger.log("Fine getDocumentiTrasmissioni", logLevelTime);	

            return dataSet;
        }
        */
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="objOggettoTrasmesso"></param>
        private static void repeatQueryTrasmissioni(ref DataSet dataSet, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            logger.Debug("repeatQueryTrasmissioni");
            ArrayList idTrasmissioneList = new ArrayList();

            foreach (DataRow dataRow in dataSet.Tables["TRASMISSIONI"].Rows)
            {
                if (!idTrasmissioneList.Contains(dataRow["ID_TRASMISSIONE"].ToString()))
                    idTrasmissioneList.Add(dataRow["ID_TRASMISSIONE"].ToString());
            }
            if (idTrasmissioneList.Count > 0)
            {
                #region Codice Commentato
                /*
				string inStr = "(" + (string)idTrasmissioneList[0];
				for (int i = 1; i < idTrasmissioneList.Count; i++)
					inStr += ", " + (string)idTrasmissioneList[i];
				inStr += ")";
				dataSet.Tables.Remove("TRASMISSIONI");
				DocsPa_V15_Utils.Query query = getQueryTrasmissioni(objOggettoTrasmesso);
				query.Where += " AND B.ID_TRASMISSIONE IN " + inStr;
				string queryString = query.getQuery();
				logger.Debug(queryString);
				db.fillTable(queryString, dataSet, "TRASMISSIONI");
				*/
                #endregion

                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                strDB.repeatQueryTrasm(ref dataSet, objOggettoTrasmesso, idTrasmissioneList, objListaFiltri);

            }
            //			DocsPa_V15_Utils.Logger.log("Fine repeatQueryTrasmissioni", logLevelTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="repeatQuery"></param>
        /// <param name="tipoQuery"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <returns></returns>
        private static System.Collections.ArrayList getListaTrasmissioni(DataSet dataSet, bool repeatQuery, string tipoQuery, bool inf, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            //DocsPa_V15_Utils.database.SqlServerAgent db = new DocsPa_V15_Utils.database.SqlServerAgent();
            logger.Debug("getListaTrasmissioni");

            if (!utenti.ContainsKey(objUtente.idPeople))
            {
                utenti.Add(objUtente.idPeople, objUtente);
            }
            if (!ruoli.ContainsKey(objRuolo.systemId))
            {
                ruoli.Add(objRuolo.systemId, objRuolo);
            }

            #region Codice Commentato
            //dataSet = getDocumentiTrasmissioni(db, dataSet, objUtente, objRuolo);

            // leggo il valore del corrispondente associato all'utente
            /*
            string sqlString = 
                "SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = " + objUtente.idPeople;
            logger.Debug(sqlString);
            objUtente.systemId = db.executeScalar(sqlString).ToString();
            */
            #endregion

            DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            objUtente.systemId = strDB.getQueryEffMet1(objUtente.idPeople);

            logger.Debug("repeatQuery = " + repeatQuery);
            if (repeatQuery) repeatQueryTrasmissioni(ref dataSet, objOggettoTrasmesso, objListaFiltri);

            Hashtable htTrasmissioni = new Hashtable();
            ArrayList result = new ArrayList();

            int numRighe = Int32.Parse(ConfigurationManager.AppSettings["numeroMaxRisultatiQuery"]);
            for (int i = 0; i < dataSet.Tables["TRASMISSIONI"].Rows.Count && i < numRighe; i++)
            {
                DataRow trasmissioneRow = dataSet.Tables["TRASMISSIONI"].Rows[i];
                string idTrasmissione = trasmissioneRow["ID_TRASMISSIONE"].ToString();

                if (!htTrasmissioni.ContainsKey(idTrasmissione))
                {
                    DocsPaVO.trasmissione.Trasmissione trasm = getTrasmissione(dataSet, trasmissioneRow, tipoQuery, inf, objOggettoTrasmesso, objUtente, objRuolo);
                    if (trasm != null)
                    {
                        try
                        {
                            htTrasmissioni.Add(idTrasmissione, null);
                        }
                        catch { };

                        result.Add(trasm);
                    }
                }
                else
                {
                    numRighe++;
                }
                if (htTrasmissioni.Count > Int32.Parse(ConfigurationManager.AppSettings["numeroMaxRisultatiQuery"]))
                    break;
            }
            utenti.Clear();
            ruoli.Clear();
            corrispondenti.Clear();
            ragioni.Clear();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.Trasmissione GetTrasmissioneByIdTrasmSing(DocsPaVO.trasmissione.OggettoTrasm oggettoTrasmesso, string idTrasmSing, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
        {
            logger.Debug("START GetTrasmissioneByIdTrasmSing");
            System.Collections.ArrayList lista = new System.Collections.ArrayList();
            DataSet dataSet = null;
            try
            {


                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                strDB.getQueryTrasmissioneByidTrasmSing(out dataSet, oggettoTrasmesso, idTrasmSing);
                if (dataSet != null && dataSet.Tables["TRASMISSIONI"] != null && dataSet.Tables["TRASMISSIONI"].Rows.Count > 0)
                {
                    DataRow trasmissioneRow = dataSet.Tables["TRASMISSIONI"].Rows[0];
                    DocsPaVO.trasmissione.Trasmissione trasm = getTrasmissione(dataSet, trasmissioneRow, "E", true, oggettoTrasmesso, utente, ruolo);
                    lista.Add(trasm);
                    dataSet.Dispose();
                    dataSet = null;
                }
                if (lista.Count > 0)
                    return (DocsPaVO.trasmissione.Trasmissione)lista[0];
                else
                    return null;
              }
              catch (Exception e)
              {
                logger.Debug(e.Message);
                logger.Debug("ERRORE GetTrasmissioneByIdTrasmSing", e);
                return null;
              }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.Trasmissione GetTrasmissioneById(DocsPaVO.trasmissione.OggettoTrasm oggettoTrasmesso, DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo, string systemId)
        {
            logger.Debug("START GetTrasmissioneById");
            System.Collections.ArrayList lista = new System.Collections.ArrayList();

            try
            {
                DataSet dataSet = null;

                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                strDB.getQueryTrasmissioneById(out dataSet, oggettoTrasmesso, systemId);

                {
                    if (!utenti.ContainsKey(utente.idPeople))
                    {
                        utenti.Add(utente.idPeople, utente);
                    }
                    if (!ruoli.ContainsKey(ruolo.systemId))
                    {
                        ruoli.Add(ruolo.systemId, ruolo);
                    }

                    utente.systemId = strDB.getQueryEffMet1(utente.idPeople);

                    Hashtable htTrasmissioni = new Hashtable();
                    for (int i = 0; i < dataSet.Tables["TRASMISSIONI"].Rows.Count; i++)
                    {
                        DataRow trasmissioneRow = dataSet.Tables["TRASMISSIONI"].Rows[i];
                        string idTrasmissione = trasmissioneRow["ID_TRASMISSIONE"].ToString();

                        if (!htTrasmissioni.ContainsKey(idTrasmissione))
                        {
                            DocsPaVO.trasmissione.Trasmissione trasm = getTrasmissione(dataSet, trasmissioneRow, "E", true, oggettoTrasmesso, utente, ruolo);
                            if (trasm != null)
                            {
                                try
                                {
                                    htTrasmissioni.Add(idTrasmissione, null);
                                }
                                catch { };

                                lista.Add(trasm);
                            }
                        }
                    }
                    utenti.Clear();
                    ruoli.Clear();
                    corrispondenti.Clear();
                    ragioni.Clear();
                }

                dataSet.Dispose();
                dataSet = null;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("ERRORE GetTrasmissioneById", e);
                throw new Exception(e.Message);
            }

            if (lista.Count > 0)
                return (DocsPaVO.trasmissione.Trasmissione)lista[0];
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="repeatQuery"></param>
        /// <param name="tipoQuery"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <returns></returns>
        private static System.Collections.ArrayList getListaTrasmissioniLite(DataSet dataSet, bool repeatQuery, string tipoQuery, bool inf, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, bool excel)
        {

            logger.Debug("getListaTrasmissioni");

            if (!utenti.ContainsKey(objUtente.idPeople))
            {
                utenti.Add(objUtente.idPeople, objUtente);
            }
            if (!ruoli.ContainsKey(objRuolo.systemId))
            {
                ruoli.Add(objRuolo.systemId, objRuolo);
            }

            DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            objUtente.systemId = strDB.getQueryEffMet1(objUtente.idPeople);

            //logger.Debug("repeatQuery = " + repeatQuery);
            //if (repeatQuery) repeatQueryTrasmissioni(ref dataSet, objOggettoTrasmesso, objListaFiltri);

            Hashtable htTrasmissioni = new Hashtable();
            ArrayList result = new ArrayList();

            int numRighe = Int32.Parse(ConfigurationManager.AppSettings["numeroMaxRisultatiQuery"]);
            for (int i = 0; i < dataSet.Tables["TRASMISSIONI"].Rows.Count && i < numRighe; i++)
            {
                DataRow trasmissioneRow = dataSet.Tables["TRASMISSIONI"].Rows[i];
                string idTrasmissione = trasmissioneRow["ID_TRASMISSIONE"].ToString();

                if (!htTrasmissioni.ContainsKey(idTrasmissione))
                {
                    try
                    {
                        DocsPaVO.trasmissione.Trasmissione trasm = getTrasmissioneLite(dataSet, trasmissioneRow, tipoQuery, inf, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);

                        if (trasm != null)
                        {
                            //try
                            //{
                            htTrasmissioni.Add(idTrasmissione, null);
                            //}
                            //catch { };

                            result.Add(trasm);
                        }


                        else
                        {
                            numRighe++;
                        }
                    }
                    catch (Exception e) { };
                    if (htTrasmissioni.Count > Int32.Parse(ConfigurationManager.AppSettings["numeroMaxRisultatiQuery"]))
                        break;

                }

            }
            utenti.Clear();
            ruoli.Clear();
            corrispondenti.Clear();
            ragioni.Clear();

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="repeatQuery"></param>
        /// <param name="tipoQuery"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <returns></returns>
        private static System.Collections.ArrayList getListaTrasmissioniLiteWithoutTrasmUtente(DataSet dataSet, bool repeatQuery, string tipoQuery, bool inf, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, bool excel)
        {

            logger.Debug("getListaTrasmissioni");

            if (!utenti.ContainsKey(objUtente.idPeople))
            {
                utenti.Add(objUtente.idPeople, objUtente);
            }
            if (!ruoli.ContainsKey(objRuolo.systemId))
            {
                ruoli.Add(objRuolo.systemId, objRuolo);
            }

            DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            objUtente.systemId = strDB.getQueryEffMet1(objUtente.idPeople);

            //logger.Debug("repeatQuery = " + repeatQuery);
            //if (repeatQuery) repeatQueryTrasmissioni(ref dataSet, objOggettoTrasmesso, objListaFiltri);

            Hashtable htTrasmissioni = new Hashtable();
            ArrayList result = new ArrayList();

            int numRighe = Int32.Parse(ConfigurationManager.AppSettings["numeroMaxRisultatiQuery"]);
            for (int i = 0; i < dataSet.Tables["TRASMISSIONI"].Rows.Count && i < numRighe; i++)
            {
                DataRow trasmissioneRow = dataSet.Tables["TRASMISSIONI"].Rows[i];
                string idTrasmissione = trasmissioneRow["ID_TRASMISSIONE"].ToString();

                if (!htTrasmissioni.ContainsKey(idTrasmissione))
                {
                    try
                    {
                        DocsPaVO.trasmissione.Trasmissione trasm = getTrasmissioneLiteWithoutTrasmUtente(dataSet, trasmissioneRow, tipoQuery, inf, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);

                        if (trasm != null)
                        {
                            //try
                            //{
                            htTrasmissioni.Add(idTrasmissione, null);
                            //}
                            //catch { };

                            result.Add(trasm);
                        }


                        else
                        {
                            numRighe++;
                        }
                    }
                    catch (Exception e) { };
                    if (htTrasmissioni.Count > Int32.Parse(ConfigurationManager.AppSettings["numeroMaxRisultatiQuery"]))
                        break;

                }

            }
            utenti.Clear();
            ruoli.Clear();
            corrispondenti.Clear();
            ragioni.Clear();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objListaFiltri"></param>
        /// <returns></returns>
        private static bool cercaInf(DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            if (objListaFiltri != null)
            {
                for (int i = 0; i < objListaFiltri.Length; i++)
                {
                    if (objListaFiltri[i].argomento.Equals("NO_CERCA_INFERIORI"))
                        return false;
                }
            }
            return true;
        }

        #region Metodo Commentato
        /*
		private static bool getCondFiltri(ref DocsPa_V15_Utils.Query query, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri) {
			// ret è false se i filtri coninvolgono le tabelle B e C e quindi la query va ripetuta
			bool ret = false;
			if(objListaFiltri == null) 
				return ret;
				
			DocsPaVO.filtri.FiltroRicerca f;
			for (int i = 0; i < objListaFiltri.Length; i++) {
				f = objListaFiltri[i];
				if (f.valore != null && !f.valore.Equals("")) {
					switch(f.argomento) {
						case "TRASMISSIONE_IL":
							query.Where += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_INVIO",false) + "= '" + f.valore + "'";				
							break;
						case "TRASMISSIONE_SUCCESSIVA_AL":
							query.Where += " AND A.DTA_INVIO>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
							break;
						case "TRASMISSIONE_PRECEDENTE_IL":
							query.Where += " AND A.DTA_INVIO<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
							break;
						case "ACCETTATA_RIFIUTATA_IL":
							ret = true;
							query.Where += " AND (" + DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_ACCETTATA",false) + "= '" + f.valore  + "' OR " + DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RIFIUTATA",false) + "= '" + f.valore + "')";				
							break;
						case "ACCETTATA_RIFIUTATA_SUCCESSIVA_AL":
							ret = true;
							query.Where += " AND (C.DTA_ACCETTATA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false) + " OR C.DTA_RIFIUTATA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false) + ")";				
							break;
						case "ACCETTATA_RIFIUTATA_PRECEDENTE_IL":
							ret = true;
							query.Where += " AND (C.DTA_ACCETTATA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false) + " OR C.DTA_RIFIUTATA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false) + ")";				
							break;
						case "SCADENZA_IL":
							ret = true;
							query.Where += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("B.DTA_SCADENZA",false) + "= '" + f.valore + "'";				
							break;
						case "SCADENZA_SUCCESSIVA_AL":
							ret = true;
							query.Where += " AND B.DTA_SCADENZA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
							break;
						case "SCADENZA_PRECEDENTE_IL":
							ret = true;
							query.Where += " AND DTA_SCADENZA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
							break;
						case "RISPOSTA_IL":
							ret = true;
							query.Where += " AND " + DocsPaDbManagement.Functions.Functions.ToChar("C.DTA_RISPOSTA",false) + "= '" + f.valore + "'";				
							break;
						case "RISPOSTA_SUCCESSIVA_AL":
							ret = true;
							query.Where += " AND C.DTA_RISPOSTA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
							break;
						case "RISPOSTA_PRECEDENTE_IL":
							query.Where += " AND C.DTA_RISPOSTA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore,false);				
							break;						
						case "NOTE_GENERALI":
							query.Where += " AND UPPER(A.VAR_NOTE_GENERALI) LIKE '%" + f.valore.ToUpper() + "%'";				
							break;					
						case "NOTE_INDIVIDUALI":
							ret = true;
							query.Where += " AND UPPER(B.VAR_NOTE_SING) LIKE '%" + f.valore.ToUpper() + "%'";				
							break;						
						case "RAGIONE":
							ret = true;
							query.addJoin("DPA_RAGIONE_TRASM D","B.ID_RAGIONE=D.SYSTEM_ID");
							query.Where += " AND UPPER(D.VAR_DESC_RAGIONE) LIKE '%" + f.valore.ToUpper() + "%'";				
							break;					
						case "STATO (A-C)":
							ret = true;
							if (f.valore.Equals("A"))
								query.Where += " AND C.CHA_ACCETTATA ='1'";
							else
								query.Where += " AND C.CHA_RIFIUTATA ='1'";
							break;					
						case "TIPO_OGGETTO":
							query.Where += " AND A.CHA_TIPO_OGGETTO = '" + f.valore + "'";
							break;	
						case "DESTINATARIO_UTENTE":
							query.Where += " AND B.CHA_TIPO_DEST='U' AND B.ID_CORR_GLOBALE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND VAR_DESC_CORR LIKE '%"+f.valore+"%')";
							break;
						case "DESTINATARIO_RUOLO":
							query.Where += " AND B.CHA_TIPO_DEST='R' AND B.ID_CORR_GLOBALE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='R' AND CHA_TIPO_IE='I' AND VAR_DESC_CORR LIKE '%"+f.valore+"%')";
							break;
						case "ID_TRASMISSIONE":
							query.Where += " AND A.SYSTEM_ID = " +f.valore;
							break;


					}
				}
					
				// filtri indipendenti dal valore
				switch(f.argomento) {
					case "ASSEGNAZIONI_PENDENTI":
						ret = true;
						query.addJoin("DPA_RAGIONE_TRASM D","B.ID_RAGIONE=D.SYSTEM_ID");
						query.Where += " AND D.CHA_TIPO_RAGIONE='W' AND C.CHA_ACCETTATA='1' AND C.ID_TRASM_RISP_SING IS NULL";
						break;		
					case "TODO_LIST":
						ret = true;
						query.addJoin("DPA_RAGIONE_TRASM D","B.ID_RAGIONE=D.SYSTEM_ID");
						query.Where += 
							" AND ((D.CHA_TIPO_RAGIONE='W' AND C.CHA_ACCETTATA='0' AND C.CHA_RIFIUTATA='0' AND C.CHA_VALIDA='1')" + 
							"OR (D.CHA_TIPO_RAGIONE='N' AND C.CHA_VISTA='0'))";
						break;
					case "IN_RISPOSTA":
						ret = true;
						query.addJoin("DPA_RAGIONE_TRASM D","B.ID_RAGIONE=D.SYSTEM_ID");
						query.Where += " AND D.CHA_RISPOSTA='1' AND C.CHA_ACCETTATA='1' AND C.ID_TRASM_RISP_SING IS NULL";
						break;	
					case "ATTIVITA_NON_CONCLUSE":
						ret = true;
						query.addJoin("DPA_RAGIONE_TRASM D","B.ID_RAGIONE=D.SYSTEM_ID");
						query.Where += " AND D.CHA_RISPOSTA='1'";	
						//condizione sui documenti e non sulla trasmissione
						query.Where += 
							" AND " + getNomeColonnaOggetto(objListaFiltri) + 
							" NOT IN (" + getQueryOggettiCompletati(objListaFiltri) + ")";
						break;
				}
			}
			return ret;
		}
		*/
        #endregion

        #endregion Trasmissioni

        #region Trasmissione
        /// <summary>
        /// </summary>
        /// <param name="idTrasmissione"></param>
        /// <param name="tipoQuery"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <returns></returns>
        internal static DocsPaVO.trasmissione.Trasmissione getTrasmissione(string idTrasmissione, string tipoQuery, bool inf, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo)
        {

            logger.Debug("getTrasmissione n. " + idTrasmissione);
            DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();

            // leggo il valore del corrispondente associato all'utente
            if ((objUtente.systemId == null || objUtente.systemId.Equals("")))
            {
                #region Codice Commentato
                /*
				string sqlString = 
					"SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = " + objUtente.idPeople;
				logger.Debug(sqlString);
				objUtente.systemId = db.executeScalar(sqlString).ToString();
				*/
                #endregion

                objUtente.systemId = strDB.getQueryEffMet1(objUtente.idPeople);
            }

            DocsPaVO.trasmissione.Trasmissione trasmissione = null;

            DataSet dataSet;

            #region Codice Commentato
            /*
			DocsPa_V15_Utils.Query query = getQueryTrasmissioni(objOggettoTrasmesso);
			
			query.Where += " AND A.SYSTEM_ID=" + idTrasmissione;
								
			string queryString = query.getQuery();
			logger.Debug(queryString);				
			
			
			db.fillTable(queryString, dataSet, "TRASMISSIONI");	
			*/
            #endregion

            strDB.getTrasm(out dataSet, idTrasmissione, objOggettoTrasmesso);

            //dataSet = getDocumentiTrasmissioni(db, dataSet, objUtente, objRuolo);

            if (dataSet.Tables["TRASMISSIONI"].Rows.Count > 0)
            {
                DataRow trasmissioneRow = dataSet.Tables["TRASMISSIONI"].Rows[0];
                trasmissione = getTrasmissione(dataSet, trasmissioneRow, tipoQuery, inf, objOggettoTrasmesso, objUtente, objRuolo);
            }
            dataSet.Dispose();

            //			DocsPa_V15_Utils.Logger.log("Fine getTrasmissione " + idTrasmissione, logLevelTime);	
            return trasmissione;

        }

        // tipoQuery: E=effettuata, R=ricevuta
        /// <summary>
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dataRow"></param>
        /// <param name="tipoQuery"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <returns></returns>
        private static DocsPaVO.trasmissione.Trasmissione getTrasmissione(DataSet dataSet, DataRow dataRow, string tipoQuery, bool inf, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo)
        {
            logger.Debug("getTrasmissione");

            DocsPaVO.trasmissione.Trasmissione objTrasm = new DocsPaVO.trasmissione.Trasmissione();

            objTrasm.systemId = dataRow["ID_TRASMISSIONE"].ToString();
            DocsPaVO.utente.InfoUtente objSicurezza = null;
            if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoDocumento != null)
            {
                objTrasm.infoDocumento = objOggettoTrasmesso.infoDocumento;
            }
            else
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                if (objUtente != null && objRuolo != null)
                {
                    objSicurezza = new DocsPaVO.utente.InfoUtente(objUtente, objRuolo);

                    //Tolta la sicurezza per permettere di vedre trasmissioni sottoposti
                    objTrasm.infoDocumento = doc.GetInfoDocumento(objSicurezza.idGruppo, objSicurezza.idPeople, dataRow["ID_PROFILE"].ToString(), true);
                    //objTrasm.infoDocumento = doc.GetInfoDocumento(null, null, dataRow["ID_PROFILE"].ToString(), false);
                }
                else
                {
                    objTrasm.infoDocumento = doc.GetInfoDocumento(null, null, dataRow["ID_PROFILE"].ToString(), false);
                }
            }
            if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoFascicolo != null)
                objTrasm.infoFascicolo = objOggettoTrasmesso.infoFascicolo;
            else
            {
                if (objSicurezza == null && objUtente != null && objRuolo != null)
                    objSicurezza = new DocsPaVO.utente.InfoUtente(objUtente, objRuolo);
                // objTrasm.infoFascicolo = BusinessLogic.Fascicoli.FascicoloManager.getInfoFascicolo(dataRow["ID_PROJECT"].ToString(),objSicurezza);
                //objTrasm.infoFascicolo = null;

                //25102004 aggiunta creazione oggetto infofascicolo 
                //per risolvere anomalia(137) sul link fascicolo
                //per il futuro verificare se l'oggetto necessita di ulteriori informazioni
                objTrasm.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo();
                objTrasm.infoFascicolo.idFascicolo = dataRow["ID_PROJECT"].ToString();

                //29102004
                //aggiunto Veronica, in risposta al commento sopra... è arrivato il momento in cui servono
                //ulteriori informazioni. Per poter visualizzare il codice e la descrizione del fascicolo 
                //nel datagrid della ricerca delle trasmissioni vado a leggere la tabella project (anomalia 641)
                if (objTrasm.infoFascicolo.idFascicolo != "")
                {
                    DataSet dataSetFasc = new DataSet();
                    DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                    strDB.GetInfoFasc(out dataSetFasc, Convert.ToInt32(objTrasm.infoFascicolo.idFascicolo));
                    if (dataSetFasc.Tables["FASCICOLI"].Rows.Count > 0)
                    {
                        DataRow fascicoloRow = dataSetFasc.Tables["FASCICOLI"].Rows[0];
                        objTrasm.infoFascicolo.codice = fascicoloRow["VAR_CODICE"].ToString();
                        objTrasm.infoFascicolo.descrizione = fascicoloRow["DESCRIPTION"].ToString();
                        objTrasm.infoFascicolo.apertura = fascicoloRow["DTA_APERTURA"].ToString();
                        if (fascicoloRow["ID_REGISTRO"] != DBNull.Value)
                            objTrasm.infoFascicolo.idRegistro = fascicoloRow["ID_REGISTRO"].ToString();
                    }
                    dataSetFasc.Dispose();
                }
                //fine aggiunta 29102004
                //fine aggiunta 25102004
            }
            if (objTrasm.infoDocumento == null && objTrasm.infoFascicolo == null)
                return null;
            if (objUtente.idPeople.Equals(dataRow["ID_PEOPLE"].ToString()))
                objTrasm.utente = objUtente;
            {
                DocsPaVO.utente.Utente userNew = new DocsPaVO.utente.Utente();
                userNew.idPeople = dataRow["ID_PEOPLE"].ToString();
                userNew.descrizione = dataRow["FULL_NAME"].ToString();
                userNew.userId = dataRow["USER_ID"].ToString();
                objTrasm.utente = userNew;
                // objTrasm.utente = getUtente(dataRow["ID_PEOPLE"].ToString());
            }

            if (objRuolo.systemId.Equals(dataRow["ID_RUOLO_IN_UO"].ToString()))
                objTrasm.ruolo = objRuolo;
            else
            {
                //objTrasm.ruolo = getRuoloEnabledAndDisabled(dataRow["ID_RUOLO_IN_UO"].ToString());
                DocsPaVO.utente.Ruolo ruoloNew = new DocsPaVO.utente.Ruolo();
                ruoloNew.systemId = dataRow["ID_RUOLO_IN_UO"].ToString();
                ruoloNew.descrizione = dataRow["desc_ruolo_mitt"].ToString();
                ruoloNew.codice = dataRow["cod_ruolo_mitt"].ToString();

                //Emanuela 15-06-2015: aggiunto idGruppo
                if (dataRow.Table.Columns.Contains("id_gruppo") && !string.IsNullOrEmpty(dataRow["id_gruppo"].ToString()))
                {
                    ruoloNew.idGruppo = dataRow["id_gruppo"].ToString();
                }
                objTrasm.ruolo = ruoloNew;
                //objTrasm.ruolo = getRuolo(dataRow["ID_RUOLO_IN_UO"].ToString())
            }

            if (dataRow["DTA_INVIO_F"] != null)
                objTrasm.dataInvio = dataRow["DTA_INVIO_F"].ToString().Trim();
            objTrasm.daAggiornare = false;
            objTrasm.tipoOggetto = (DocsPaVO.trasmissione.TipoOggetto)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.Trasmissione.oggettoStringa, dataRow["CHA_TIPO_OGGETTO"].ToString().Trim());

            objTrasm.noteGenerali = dataRow["VAR_NOTE_GENERALI"].ToString();

            objTrasm.delegato = "";
            if (dataRow["ID_PEOPLE_DELEGATO"] != null)
                if (!string.IsNullOrEmpty(dataRow["ID_PEOPLE_DELEGATO"].ToString()))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
                    DocsPaVO.utente.Utente delegato =  utente.GetUtenteNoFiltroDisabled(dataRow["ID_PEOPLE_DELEGATO"].ToString());
                    //Uso il metodo GetUtenteNoFiltroDisabled e non GetUtente perchè nel caso in cui il delegato è disabilitato non ritorna nulla
                    //info.UtenteDelegato = utente.GetUtente(dataRow["ID_PEOPLE_DELEGATO"].ToString()).descrizione;

                    objTrasm.delegato = (delegato != null) ? delegato.descrizione : string.Empty;
                }

            // trasmissione salvata con cessione dei diritti
            if (dataRow["CHA_SALVATA_CON_CESSIONE"].ToString() != null && dataRow["CHA_SALVATA_CON_CESSIONE"].ToString().Equals("1"))
                objTrasm.salvataConCessione = true;

            objTrasm = getTrasmSingole(dataSet, objTrasm, tipoQuery, inf, objUtente, objRuolo);

            return objTrasm;
        }


       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="objTrasm"></param>
        /// <param name="tipoQuery"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <returns></returns>
        private static DocsPaVO.trasmissione.Trasmissione getTrasmSingole(DataSet dataSet, DocsPaVO.trasmissione.Trasmissione objTrasm, string tipoQuery, bool inf, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo)
        {
            logger.Debug("getTrasmSingole");
            string queryString = "";
            queryString = "ID_TRASMISSIONE=" + objTrasm.systemId;

            //EM tolto perchè non permetteva di trovare da ricerca trasm ricevute le trasmissioni singole.
            // leggo solo le righe riferite all'utente, ho settato objUtente.systemId in precedenza, se necessario
            //if (tipoQuery.Equals("R") && !inf) 
            //{	
            //   queryString += " AND (ID_CORR_GLOBALE = " + objRuolo.systemId + " OR ID_CORR_GLOBALE=" + objUtente.systemId + ")";
            //}		

            logger.Debug(queryString);

            DataView dv = dataSet.Tables["TRASMISSIONI"].DefaultView;
            dv.RowFilter = queryString;
            dv.Sort = "CHA_TIPO_DEST ASC";

            string id_TrasmSingola = string.Empty;
            Hashtable trasmsingole = new Hashtable();

            foreach (DataRowView rowView in dv)
            {
                DataRow dataRow = rowView.Row;

                DocsPaVO.trasmissione.RagioneTrasmissione newRag = new DocsPaVO.trasmissione.RagioneTrasmissione();
                if (!trasmsingole.ContainsKey(Int32.Parse(dataRow["ID_TRASM_SINGOLA"].ToString().Trim())))
                {
                    trasmsingole.Add(Int32.Parse(dataRow["ID_TRASM_SINGOLA"].ToString().Trim()), dataRow["ID_TRASM_SINGOLA"].ToString());
                    id_TrasmSingola = dataRow["ID_TRASM_SINGOLA"].ToString();
                    DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
                    objTrasmSingola.systemId = id_TrasmSingola;
                    newRag.systemId = dataRow["ID_RAGIONE"].ToString();
                    newRag.descrizione = dataRow["VAR_DESC_RAGIONE"].ToString();
                    newRag.tipo = dataRow["cha_tipo_ragione"].ToString();
                    newRag.tipoDiritti = (DocsPaVO.trasmissione.TipoDiritto)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoDirittoStringa, dataRow["cha_tipo_diritti"].ToString());
                    newRag.risposta = dataRow["cha_risposta"].ToString();
                    newRag.tipoDestinatario = (DocsPaVO.trasmissione.TipoGerarchia)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa, dataRow["tipo_dest_rag"].ToString());
                    newRag.note = dataRow["var_note"].ToString();
                    newRag.eredita = dataRow["cha_eredita"].ToString();
                    if (dataRow["cha_tipo_risposta"] != null)
                    {
                        newRag.tipoRisposta = dataRow["cha_tipo_risposta"].ToString();
                    }
                    newRag.notifica = dataRow["var_notifica_trasm"].ToString();

                    if (dataRow["var_testo_msg_notifica_doc"] != null)
                        newRag.testoMsgNotificaDoc = dataRow["var_testo_msg_notifica_doc"].ToString();

                    if (dataRow["var_testo_msg_notifica_fasc"] != null)
                        newRag.testoMsgNotificaFasc = dataRow["var_testo_msg_notifica_fasc"].ToString();

                    if (dataRow["cha_cede_diritti"] != null)
                        newRag.prevedeCessione = dataRow["cha_cede_diritti"].ToString();
                    else
                        newRag.prevedeCessione = "N";
                    if (dataRow["cha_mantieni_lett"] != null)
                        newRag.mantieniLettura = dataRow["cha_mantieni_lett"].ToString();
                    else
                        newRag.mantieniLettura = "false";

                    //MEV LIBRO FIRMA 04-06-2015
                    if (dataRow.Table.Columns.Contains("CHA_PROC_RES") && !string.IsNullOrEmpty(dataRow["CHA_PROC_RES"].ToString()))
                    {
                        newRag.azioneRichiesta = dataRow["CHA_PROC_RES"].ToString();
                    }
                    //MEV FASCICOLAZIONE OBBLIGATORIA
                    if (dataRow.Table.Columns.Contains("CHA_FASC_OBBLIGATORIA") && !string.IsNullOrEmpty(dataRow["CHA_FASC_OBBLIGATORIA"].ToString()))
                    {
                        newRag.fascicolazioneObbligatoria = dataRow["CHA_FASC_OBBLIGATORIA"].ToString().Equals("1");
                    }

                    //VEDO SE è UNA RAGIONE DI TIPO TASK
                    if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ENABLE_TASK")) &&
                            DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ENABLE_TASK").ToString().Equals("1"))
                    {
                        DocsPaDB.Query_DocsPAWS.Task t = new DocsPaDB.Query_DocsPAWS.Task();
                        newRag.isTipoTask = t.IsRagioneDiTipoTask(newRag.systemId);
                    }
                    objTrasmSingola.ragione = newRag;
                    //  objTrasmSingola.ragione = getRagione(dataRow["ID_RAGIONE"].ToString());


                    objTrasmSingola.tipoDest = (DocsPaVO.trasmissione.TipoDestinatario)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.TrasmissioneSingola.tipoDestStringa, dataRow["CHA_TIPO_DEST"].ToString());
                    objTrasmSingola.corrispondenteInterno = getCorrispondente(dataRow["ID_CORR_GLOBALE"].ToString(), true);
                    //Emanuela 17/03/2014: aggiungo un controllo sul corrispondente per evitare l'errore in caso di ruolo storicizzato
                    if (objTrasmSingola.corrispondenteInterno != null)
                    {
                        objTrasmSingola.noteSingole = dataRow["VAR_NOTE_SING"].ToString();
                        objTrasmSingola.tipoTrasm = dataRow["CHA_TIPO_TRASM"].ToString();
                        objTrasmSingola.idTrasmUtente = dataRow["ID_TRASM_UTENTE"].ToString();
                        objTrasmSingola.dataScadenza = dataRow["DTA_SCADENZA"].ToString().Trim();

                        if (dataRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                            objTrasmSingola.hideDocumentPreviousVersions = (dataRow["HIDE_DOC_VERSIONS"].ToString() == "1" ? true : false);
                        else
                            objTrasmSingola.hideDocumentPreviousVersions = false;

                        objTrasmSingola = getTrasmUtente(dataSet, objTrasmSingola, tipoQuery, inf, objUtente, objRuolo);
                        objTrasm.trasmissioniSingole.Add(objTrasmSingola);
                    }
                }
            }

            // DataRow[] trasmissioniSingoleRows = dataSet.Tables["TRASMISSIONI"].Select(queryString);

            //string id_TrasmSingola = "";
            //Hashtable trasmsingole=new Hashtable();

            //foreach(DataRow dataRow in trasmissioniSingoleRows) 
            //{
            //    if (!trasmsingole.ContainsKey(Int32.Parse(dataRow["ID_TRASM_SINGOLA"].ToString().Trim()))) 
            //    {
            //        trasmsingole.Add(Int32.Parse(dataRow["ID_TRASM_SINGOLA"].ToString().Trim()),dataRow["ID_TRASM_SINGOLA"].ToString());
            //        id_TrasmSingola = dataRow["ID_TRASM_SINGOLA"].ToString();
            //        DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            //        objTrasmSingola.systemId = id_TrasmSingola;				
            //        objTrasmSingola.ragione = getRagione(dataRow["ID_RAGIONE"].ToString());		
            //        objTrasmSingola.tipoDest = (DocsPaVO.trasmissione.TipoDestinatario) DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.TrasmissioneSingola.tipoDestStringa,dataRow["CHA_TIPO_DEST"].ToString());			
            //        objTrasmSingola.corrispondenteInterno = getCorrispondente(dataRow["ID_CORR_GLOBALE"].ToString(), true);
            //        objTrasmSingola.noteSingole = dataRow["VAR_NOTE_SING"].ToString();			
            //        objTrasmSingola.tipoTrasm = dataRow["CHA_TIPO_TRASM"].ToString();	
            //        objTrasmSingola.idTrasmUtente = dataRow["ID_TRASM_UTENTE"].ToString();
            //        objTrasmSingola.dataScadenza = dataRow["DTA_SCADENZA"].ToString().Trim();
            //        objTrasmSingola = getTrasmUtente(dataSet, objTrasmSingola, tipoQuery, inf, objUtente, objRuolo);
            //        objTrasm.trasmissioniSingole.Add(objTrasmSingola);
            //    }
            //}

            //			DocsPa_V15_Utils.Logger.log("Fine getTrasmSingole", logLevelTime);	

            return objTrasm;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="objTrasm"></param>
        /// <param name="tipoQuery"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <returns></returns>
        private static DocsPaVO.trasmissione.Trasmissione getTrasmSingoleWithoutTrasmUtente(DataSet dataSet, DocsPaVO.trasmissione.Trasmissione objTrasm, string tipoQuery, bool inf, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo)
        {
            logger.Debug("getTrasmSingole");
            string queryString = "";
            queryString = "ID_TRASMISSIONE=" + objTrasm.systemId;

            //EM tolto perchè non permetteva di trovare da ricerca trasm ricevute le trasmissioni singole.
            // leggo solo le righe riferite all'utente, ho settato objUtente.systemId in precedenza, se necessario
            //if (tipoQuery.Equals("R") && !inf) 
            //{	
            //   queryString += " AND (ID_CORR_GLOBALE = " + objRuolo.systemId + " OR ID_CORR_GLOBALE=" + objUtente.systemId + ")";
            //}		

            logger.Debug(queryString);

            DataView dv = dataSet.Tables["TRASMISSIONI"].DefaultView;
            dv.RowFilter = queryString;
            dv.Sort = "CHA_TIPO_DEST ASC";

            string id_TrasmSingola = string.Empty;
            Hashtable trasmsingole = new Hashtable();

            foreach (DataRowView rowView in dv)
            {
                DataRow dataRow = rowView.Row;

                DocsPaVO.trasmissione.RagioneTrasmissione newRag = new DocsPaVO.trasmissione.RagioneTrasmissione();
                if (!trasmsingole.ContainsKey(Int32.Parse(dataRow["ID_TRASM_SINGOLA"].ToString().Trim())))
                {
                    trasmsingole.Add(Int32.Parse(dataRow["ID_TRASM_SINGOLA"].ToString().Trim()), dataRow["ID_TRASM_SINGOLA"].ToString());
                    id_TrasmSingola = dataRow["ID_TRASM_SINGOLA"].ToString();
                    DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
                    objTrasmSingola.systemId = id_TrasmSingola;
                    newRag.systemId = dataRow["ID_RAGIONE"].ToString();
                    newRag.descrizione = dataRow["VAR_DESC_RAGIONE"].ToString();
                    newRag.tipo = dataRow["cha_tipo_ragione"].ToString();

                    objTrasmSingola.ragione = newRag;
                    //  objTrasmSingola.ragione = getRagione(dataRow["ID_RAGIONE"].ToString());


                    objTrasmSingola.tipoDest = (DocsPaVO.trasmissione.TipoDestinatario)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.TrasmissioneSingola.tipoDestStringa, dataRow["CHA_TIPO_DEST"].ToString());
                    objTrasmSingola.corrispondenteInterno = getCorrispondente(dataRow["ID_CORR_GLOBALE"].ToString(), true);
                    //Emanuela 17/03/2014: aggiungo un controllo sul corrispondente per evitare l'errore in caso di ruolo storicizzato
                    if (objTrasmSingola.corrispondenteInterno != null)
                    {
                        objTrasmSingola.dataScadenza = dataRow["DTA_SCADENZA"].ToString().Trim();

                        objTrasm.trasmissioniSingole.Add(objTrasmSingola);
                    }
                }
            }

            return objTrasm;
        }


        /// <summary></summary>
        /// <param name="dataSet"></param>
        /// <param name="objTrasmSingola"></param>
        /// <param name="tipoQuery"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <returns></returns>
        private static DocsPaVO.trasmissione.TrasmissioneSingola getTrasmUtente(DataSet dataSet, DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola, string tipoQuery, bool inf, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo)
        {
            IFormatProvider format = new System.Globalization.CultureInfo("it-IT", true);
            DateTime xdatePost;

            logger.Debug("getTrasmUtente");

            string queryString = "ID_TRASM_SINGOLA = " + objTrasmSingola.systemId;

            // legge solo le righe riferite all'utente
            if (tipoQuery.Equals("R") && !inf)
                queryString += " AND ID_DESTINATARIO=" + objUtente.idPeople;
            logger.Debug(queryString);

            DataRow[] trasmissioniUtenteRows = dataSet.Tables["TRASMISSIONI"].Select(queryString);

            foreach (DataRow dataRow in trasmissioniUtenteRows)
            {
                DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                objTrasmUtente.systemId = dataRow["ID_TRASMISSIONE_UTENTE"].ToString();
                if (objUtente.idPeople.Equals(dataRow["ID_DESTINATARIO"].ToString()))
                {
                    logger.Debug("senza getUtente");
                    objTrasmUtente.utente = objUtente;
                }
                else
                {
                    logger.Debug("getUtente");
                    objTrasmUtente.utente = getUtente(dataRow["ID_DESTINATARIO"].ToString());
                }
                objTrasmUtente.dataVista = dataRow["DTA_VISTA"].ToString().Trim();
                objTrasmUtente.dataAccettata = dataRow["DTA_ACCETTATA"].ToString().Trim();
                objTrasmUtente.dataRifiutata = dataRow["DTA_RIFIUTATA"].ToString().Trim();
                objTrasmUtente.noteRifiuto = dataRow["VAR_NOTE_RIF"].ToString();
                objTrasmUtente.noteAccettazione = dataRow["VAR_NOTE_ACC"].ToString();
                objTrasmUtente.valida = dataRow["CHA_VALIDA"].ToString();
                if (dataRow["DELEGATO_UTENTE"] != null)
                {
                    if (!string.IsNullOrEmpty(dataRow["DELEGATO_UTENTE"].ToString()) && !dataRow["DELEGATO_UTENTE"].ToString().Equals("0"))
                    {
                        DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
                        //objTrasmUtente.idPeopleDelegato = utente.GetUtente(dataRow["DELEGATO_UTENTE"].ToString()).descrizione;

                        objTrasmUtente.idPeopleDelegato = utente.GetUtenteNoFiltroDisabled(dataRow["DELEGATO_UTENTE"].ToString()).descrizione;
                    }
                }

                if (dataRow["ACCETTATA_DELEGATO"] != null)
                {
                    if (!string.IsNullOrEmpty(dataRow["ACCETTATA_DELEGATO"].ToString()))
                    {
                        objTrasmUtente.cha_accettata_delegato = dataRow["ACCETTATA_DELEGATO"].ToString();
                    }
                }

                if (dataRow["VISTA_DELEGATO"] != null)
                {
                    if (!string.IsNullOrEmpty(dataRow["VISTA_DELEGATO"].ToString()))
                    {
                        objTrasmUtente.cha_vista_delegato = dataRow["VISTA_DELEGATO"].ToString();
                    }
                }

                if (dataRow["RIFIUTATA_DELEGATO"] != null)
                {
                    if (!string.IsNullOrEmpty(dataRow["RIFIUTATA_DELEGATO"].ToString()))
                    {
                        objTrasmUtente.cha_rifiutata_delegato = dataRow["RIFIUTATA_DELEGATO"].ToString();
                    }
                }

                if (dataRow.Table.Columns.Contains("RIMOSSA_DELEGATO") && dataRow["RIMOSSA_DELEGATO"] != null)
                {
                    if (!string.IsNullOrEmpty(dataRow["RIMOSSA_DELEGATO"].ToString()))
                    {
                        objTrasmUtente.cha_rimossa_delegato = dataRow["RIMOSSA_DELEGATO"].ToString();
                    }
                }

                // nuovo campo rimozione dalla todolist
                if (!dataRow["DTA_RIMOSSA_TDL"].ToString().Equals(string.Empty))
                {
                    xdatePost = Convert.ToDateTime(dataRow["DTA_RIMOSSA_TDL"].ToString(), format);
                    objTrasmUtente.dataRimossaTDL = xdatePost.ToShortDateString();
                }
                objTrasmSingola.trasmissioneUtente.Add(objTrasmUtente);
                logger.Debug("Aggiunta tr.ut.");
            }

            //			DocsPa_V15_Utils.Logger.log("Fine getTrasmUtente", logLevelTime);

            return objTrasmSingola;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.Utente getUtente(string idPeople)
        {
            logger.Debug("getUtente");
            DocsPaVO.utente.Utente utente = null;
            if (utenti != null && utenti.Count > 0 && utenti.ContainsKey(idPeople))
                utente = (DocsPaVO.utente.Utente)utenti[idPeople];
            else
            {
                utente = BusinessLogic.Utenti.UserManager.getUtenteNoFiltroDisabled(idPeople);
                if (utente.idPeople != null)
                {
                    try
                    {
                        if (!utenti.ContainsKey(idPeople))
                        {
                            utenti.Add(utente.idPeople, utente);
                        }
                    }
                    catch { };

                }
            }
            return utente;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.Ruolo getRuolo(string idRuolo)
        {
            DocsPaVO.utente.Ruolo ruolo = null;
            if (ruoli.ContainsKey(idRuolo))
                ruolo = (DocsPaVO.utente.Ruolo)ruoli[idRuolo];
            else
            {
                ruolo = BusinessLogic.Utenti.UserManager.getRuolo(idRuolo);
                ruoli.Add(ruolo.systemId, ruolo);
            }
            return ruolo;
        }


        private static DocsPaVO.utente.Ruolo getRuoloEnabledAndDisabled(string idRuolo)
        {
            DocsPaVO.utente.Ruolo ruolo = null;
            if (ruoli.ContainsKey(idRuolo))
                ruolo = (DocsPaVO.utente.Ruolo)ruoli[idRuolo];
            else
            {
                ruolo = BusinessLogic.Utenti.UserManager.getRuoloEnabledAndDisabled(idRuolo);
                ruoli.Add(ruolo.systemId, ruolo);
            }
            return ruolo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idCorrispondente"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.Corrispondente getCorrispondente(string idCorrispondente, bool isEnable)
        {
            DocsPaVO.utente.Corrispondente corr = null;
            if (corrispondenti.ContainsKey(idCorrispondente))
                corr = (DocsPaVO.utente.Corrispondente)corrispondenti[idCorrispondente];
            else
            {
                corr = BusinessLogic.Utenti.UserManager.getCorrispondente(idCorrispondente, isEnable);
                if(corr != null)
                    corrispondenti.Add(corr.systemId, corr);
            }
            return corr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRagione"></param>
        /// <returns></returns>
        private static DocsPaVO.trasmissione.RagioneTrasmissione getRagione(string idRagione)
        {
            DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;
            if (ragioni.ContainsKey(idRagione))
                ragione = (DocsPaVO.trasmissione.RagioneTrasmissione)ragioni[idRagione];
            else
            {
                ragione = RagioniManager.getRagione(idRagione);
                if (ragione != null)
                    ragioni.Add(ragione.systemId, ragione);
                else
                    logger.Debug("ATTENZIONE! ragione di trasmissione inesistente. ID: " + idRagione);
            }
            return ragione;
        }

        public static DocsPaVO.trasmissione.RagioneTrasmissione getRagioneById(string idRagione)
        {
            DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;
            if (ragioni.ContainsKey(idRagione))
                ragione = (DocsPaVO.trasmissione.RagioneTrasmissione)ragioni[idRagione];
            else
            {
                ragione = RagioniManager.getRagione(idRagione);
                if (ragione != null)
                    ragioni.Add(ragione.systemId, ragione);
                else
                    logger.Debug("ATTENZIONE! ragione di trasmissione inesistente. ID: " + idRagione);
            }
            return ragione;
        }

        // tipoQuery: E=effettuata, R=ricevuta
        /// <summary>
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dataRow"></param>
        /// <param name="tipoQuery"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <returns></returns>
        private static DocsPaVO.trasmissione.Trasmissione getTrasmissioneLite(DataSet dataSet, DataRow dataRow, string tipoQuery, bool inf, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            logger.Debug("getTrasmissione");

            //è un documento
            DocsPaVO.filtri.FiltroRicerca documentOrFolder = objListaFiltri.Where(e => e.argomento == "TIPO_OGGETTO").FirstOrDefault();

            DocsPaVO.trasmissione.Trasmissione objTrasm = new DocsPaVO.trasmissione.Trasmissione();

            DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
            DocsPaVO.fascicolazione.InfoFascicolo infoFasc = new DocsPaVO.fascicolazione.InfoFascicolo();

            objTrasm.systemId = dataRow["ID_TRASMISSIONE"].ToString();
            DocsPaVO.utente.InfoUtente objSicurezza = null;
            if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoDocumento != null)
            {
                objTrasm.infoDocumento = objOggettoTrasmesso.infoDocumento;
            }
            else
            {
                objSicurezza = new DocsPaVO.utente.InfoUtente(objUtente, objRuolo);
                if (documentOrFolder!=null && documentOrFolder.valore == "D")
                {
                    objTrasm.infoDocumento = infoDoc;
                    objTrasm.infoDocumento.idProfile = dataRow["ID_PROFILE"].ToString();
                    objTrasm.infoDocumento.numProt = dataRow["NUM_PROTO"].ToString();
                    objTrasm.infoDocumento.dataApertura = dataRow["DATA_PROTO"].ToString();
                    objTrasm.infoDocumento.oggetto = dataRow["OGGETTO_PROTO"].ToString();
                    objTrasm.infoDocumento.docNumber = dataRow["DOCNUMBER"].ToString();
                    objTrasm.infoDocumento.segnatura = dataRow["VAR_SEGNATURA"].ToString();
                    objTrasm.infoDocumento.idRegistro = dataRow["ID_REGISTRO"].ToString();
                    objTrasm.infoDocumento.tipoProto = dataRow["cha_tipo_proto"].ToString();
                    System.Collections.Generic.List<string> dest = new System.Collections.Generic.List<string>();
                    dest.Add(dataRow["DESTINATARI_PROTO"].ToString());
                    objTrasm.infoDocumento.Destinatari = dest;
                    objTrasm.infoDocumento.mittDoc = dataRow["MITTENTI_PROTO"].ToString();
                }
            }
            if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoFascicolo != null)
            {
                objTrasm.infoFascicolo = objOggettoTrasmesso.infoFascicolo;
            }
            else
            {
                if (objSicurezza == null && objUtente != null && objRuolo != null)
                    objSicurezza = new DocsPaVO.utente.InfoUtente(objUtente, objRuolo);
                // objTrasm.infoFascicolo = BusinessLogic.Fascicoli.FascicoloManager.getInfoFascicolo(dataRow["ID_PROJECT"].ToString(),objSicurezza);
                //objTrasm.infoFascicolo = null;

                //25102004 aggiunta creazione oggetto infofascicolo 
                //per risolvere anomalia(137) sul link fascicolo
                //per il futuro verificare se l'oggetto necessita di ulteriori informazioni
                objTrasm.infoFascicolo = infoFasc;
                objTrasm.infoFascicolo.idFascicolo = dataRow["ID_PROJECT"].ToString();

                //29102004
                //aggiunto Veronica, in risposta al commento sopra... è arrivato il momento in cui servono
                //ulteriori informazioni. Per poter visualizzare il codice e la descrizione del fascicolo 
                //nel datagrid della ricerca delle trasmissioni vado a leggere la tabella project (anomalia 641)
                if (objTrasm.infoFascicolo.idFascicolo != "")
                {
                    objTrasm.infoFascicolo.codice = dataRow["VAR_CODICE"].ToString();
                    objTrasm.infoFascicolo.descrizione = dataRow["DESCRIPTION"].ToString();
                    objTrasm.infoFascicolo.apertura = dataRow["DTA_APERTURA"].ToString();
                    objTrasm.infoFascicolo.idRegistro = dataRow["ID_REGISTRO"].ToString();

                }
                //fine aggiunta 29102004
                //fine aggiunta 25102004
            }
            if (objTrasm.infoDocumento == null && objTrasm.infoFascicolo == null)
                return null;
            if (objUtente.idPeople.Equals(dataRow["ID_PEOPLE"].ToString()))
                objTrasm.utente = objUtente;
            {
                DocsPaVO.utente.Utente userNew = new DocsPaVO.utente.Utente();
                userNew.idPeople = dataRow["ID_PEOPLE"].ToString();
                userNew.descrizione = dataRow["FULL_NAME"].ToString();
                userNew.userId = dataRow["USER_ID"].ToString();
                objTrasm.utente = userNew;
                // objTrasm.utente = getUtente(dataRow["ID_PEOPLE"].ToString());
            }

            if (objRuolo.systemId.Equals(dataRow["ID_RUOLO_IN_UO"].ToString()))
                objTrasm.ruolo = objRuolo;
            else
            {
                //objTrasm.ruolo = getRuoloEnabledAndDisabled(dataRow["ID_RUOLO_IN_UO"].ToString());
                DocsPaVO.utente.Ruolo ruoloNew = new DocsPaVO.utente.Ruolo();
                ruoloNew.systemId = dataRow["ID_RUOLO_IN_UO"].ToString();
                ruoloNew.descrizione = dataRow["desc_ruolo_mitt"].ToString();
                ruoloNew.codice = dataRow["cod_ruolo_mitt"].ToString();
                objTrasm.ruolo = ruoloNew;
                //objTrasm.ruolo = getRuolo(dataRow["ID_RUOLO_IN_UO"].ToString())
            }

            if (dataRow["DTA_INVIO_F"] != null)
                objTrasm.dataInvio = dataRow["DTA_INVIO_F"].ToString().Trim();
            objTrasm.daAggiornare = false;
            objTrasm.tipoOggetto = (DocsPaVO.trasmissione.TipoOggetto)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.Trasmissione.oggettoStringa, dataRow["CHA_TIPO_OGGETTO"].ToString().Trim());

            objTrasm.noteGenerali = dataRow["VAR_NOTE_GENERALI"].ToString();

            objTrasm.delegato = "";
            if (dataRow["ID_PEOPLE_DELEGATO"] != null)
                if (!string.IsNullOrEmpty(dataRow["ID_PEOPLE_DELEGATO"].ToString()))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
                    objTrasm.delegato = utente.GetUtente(dataRow["ID_PEOPLE_DELEGATO"].ToString()).descrizione;
                }

            // trasmissione salvata con cessione dei diritti
            if (dataRow["CHA_SALVATA_CON_CESSIONE"].ToString() != null && dataRow["CHA_SALVATA_CON_CESSIONE"].ToString().Equals("1"))
                objTrasm.salvataConCessione = true;

            objTrasm = getTrasmSingole(dataSet, objTrasm, tipoQuery, inf, objUtente, objRuolo);

            return objTrasm;
        }


        // tipoQuery: E=effettuata, R=ricevuta
        /// <summary>
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dataRow"></param>
        /// <param name="tipoQuery"></param>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <returns></returns>
        private static DocsPaVO.trasmissione.Trasmissione getTrasmissioneLiteWithoutTrasmUtente(DataSet dataSet, DataRow dataRow, string tipoQuery, bool inf, DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Utente objUtente, DocsPaVO.utente.Ruolo objRuolo, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            logger.Debug("getTrasmissione");

            //è un documento
            DocsPaVO.filtri.FiltroRicerca documentOrFolder = objListaFiltri.Where(e => e.argomento == "TIPO_OGGETTO").FirstOrDefault();

            DocsPaVO.trasmissione.Trasmissione objTrasm = new DocsPaVO.trasmissione.Trasmissione();

            DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
            DocsPaVO.fascicolazione.InfoFascicolo infoFasc = new DocsPaVO.fascicolazione.InfoFascicolo();

            objTrasm.systemId = dataRow["ID_TRASMISSIONE"].ToString();
            DocsPaVO.utente.InfoUtente objSicurezza = null;
            if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoDocumento != null)
            {
                objTrasm.infoDocumento = objOggettoTrasmesso.infoDocumento;
            }
            else
            {
                objSicurezza = new DocsPaVO.utente.InfoUtente(objUtente, objRuolo);
                if (documentOrFolder != null && documentOrFolder.valore == "D")
                {
                    objTrasm.infoDocumento = infoDoc;
                    objTrasm.infoDocumento.idProfile = dataRow["ID_PROFILE"].ToString();
                    objTrasm.infoDocumento.numProt = dataRow["NUM_PROTO"].ToString();
                    objTrasm.infoDocumento.dataApertura = dataRow["DATA_PROTO"].ToString();
                    objTrasm.infoDocumento.oggetto = dataRow["OGGETTO_PROTO"].ToString();
                    objTrasm.infoDocumento.docNumber = dataRow["DOCNUMBER"].ToString();
                    objTrasm.infoDocumento.segnatura = dataRow["VAR_SEGNATURA"].ToString();
                    objTrasm.infoDocumento.idRegistro = dataRow["ID_REGISTRO"].ToString();
                    objTrasm.infoDocumento.tipoProto = dataRow["cha_tipo_proto"].ToString();
                    objTrasm.infoDocumento.contatore = !string.IsNullOrEmpty(dataRow["COUNTER_REPERTORY"].ToString()) ? dataRow["COUNTER_REPERTORY"].ToString().Substring(0, dataRow["COUNTER_REPERTORY"].ToString().Length - 2) : string.Empty;
                    System.Collections.Generic.List<string> dest = new System.Collections.Generic.List<string>();
                    dest.Add(dataRow["DESTINATARI_PROTO"].ToString());
                    objTrasm.infoDocumento.Destinatari = dest;
                    objTrasm.infoDocumento.mittDoc = dataRow["MITTENTI_PROTO"].ToString();
                }
            }
            if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoFascicolo != null)
            {
                objTrasm.infoFascicolo = objOggettoTrasmesso.infoFascicolo;
            }
            else
            {
                if (objSicurezza == null && objUtente != null && objRuolo != null)
                    objSicurezza = new DocsPaVO.utente.InfoUtente(objUtente, objRuolo);
                // objTrasm.infoFascicolo = BusinessLogic.Fascicoli.FascicoloManager.getInfoFascicolo(dataRow["ID_PROJECT"].ToString(),objSicurezza);
                //objTrasm.infoFascicolo = null;

                //25102004 aggiunta creazione oggetto infofascicolo 
                //per risolvere anomalia(137) sul link fascicolo
                //per il futuro verificare se l'oggetto necessita di ulteriori informazioni
                objTrasm.infoFascicolo = infoFasc;
                objTrasm.infoFascicolo.idFascicolo = dataRow["ID_PROJECT"].ToString();

                //29102004
                //aggiunto Veronica, in risposta al commento sopra... è arrivato il momento in cui servono
                //ulteriori informazioni. Per poter visualizzare il codice e la descrizione del fascicolo 
                //nel datagrid della ricerca delle trasmissioni vado a leggere la tabella project (anomalia 641)
                if (objTrasm.infoFascicolo.idFascicolo != "")
                {
                    objTrasm.infoFascicolo.codice = dataRow["VAR_CODICE"].ToString();
                    objTrasm.infoFascicolo.descrizione = dataRow["DESCRIPTION"].ToString();
                    objTrasm.infoFascicolo.apertura = dataRow["DTA_APERTURA"].ToString();
                    objTrasm.infoFascicolo.idRegistro = dataRow["ID_REGISTRO"].ToString();

                }
                //fine aggiunta 29102004
                //fine aggiunta 25102004
            }
            if (objTrasm.infoDocumento == null && objTrasm.infoFascicolo == null)
                return null;
            if (objUtente.idPeople.Equals(dataRow["ID_PEOPLE"].ToString()))
                objTrasm.utente = objUtente;
            {
                DocsPaVO.utente.Utente userNew = new DocsPaVO.utente.Utente();
                userNew.idPeople = dataRow["ID_PEOPLE"].ToString();
                userNew.descrizione = dataRow["FULL_NAME"].ToString();
                userNew.userId = dataRow["USER_ID"].ToString();
                objTrasm.utente = userNew;
                // objTrasm.utente = getUtente(dataRow["ID_PEOPLE"].ToString());
            }

            if (objRuolo.systemId.Equals(dataRow["ID_RUOLO_IN_UO"].ToString()))
                objTrasm.ruolo = objRuolo;
            else
            {
                //objTrasm.ruolo = getRuoloEnabledAndDisabled(dataRow["ID_RUOLO_IN_UO"].ToString());
                DocsPaVO.utente.Ruolo ruoloNew = new DocsPaVO.utente.Ruolo();
                ruoloNew.systemId = dataRow["ID_RUOLO_IN_UO"].ToString();
                ruoloNew.descrizione = dataRow["desc_ruolo_mitt"].ToString();
                ruoloNew.codice = dataRow["cod_ruolo_mitt"].ToString();
                objTrasm.ruolo = ruoloNew;
                //objTrasm.ruolo = getRuolo(dataRow["ID_RUOLO_IN_UO"].ToString())
            }

            if (dataRow["DTA_INVIO_F"] != null)
                objTrasm.dataInvio = dataRow["DTA_INVIO_F"].ToString().Trim();
            objTrasm.daAggiornare = false;
            objTrasm.tipoOggetto = (DocsPaVO.trasmissione.TipoOggetto)DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.Trasmissione.oggettoStringa, dataRow["CHA_TIPO_OGGETTO"].ToString().Trim());
            objTrasm.delegato = "";
            if (dataRow["ID_PEOPLE_DELEGATO"] != null)
                if (!string.IsNullOrEmpty(dataRow["ID_PEOPLE_DELEGATO"].ToString()))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
                    objTrasm.delegato = utente.GetUtente(dataRow["ID_PEOPLE_DELEGATO"].ToString()).descrizione;
                }

            objTrasm = getTrasmSingoleWithoutTrasmUtente(dataSet, objTrasm, tipoQuery, inf, objUtente, objRuolo);

            return objTrasm;
        }

        

        /// <summary>
        ///	Reperimento di tutte le trasmissioni effettuate (e paginate)
        /// </summary>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getQueryEffettuateMethodPagingLite(
                                                    DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                                    DocsPaVO.utente.Utente objUtente,
                                                    DocsPaVO.utente.Ruolo objRuolo,
                                                    DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                                    int pageNumber,
            bool excel,
            int pageSize,
                                                    out int totalPageNumber,
                                                    out int recordCount)
        {
            return getQueryEffettuateMethodPagingInternalLite(objOggettoTrasmesso,
                                                            objUtente,
                                                            objRuolo,
                                                            objListaFiltri,
                                                            true,
                                                            pageNumber,
                                                            excel,
                                                            pageSize,
                                                            out totalPageNumber,
                                                            out recordCount);
        }

        /// <summary>
        ///	Reperimento di tutte le trasmissioni effettuate (e paginate)
        /// </summary>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objUtente"></param>
        /// <param name="objRuolo"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalPageNumber"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getQueryEffettuateMethodPagingLiteWithoutTrasmUtente(
                                                    DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                                    DocsPaVO.utente.Utente objUtente,
                                                    DocsPaVO.utente.Ruolo objRuolo,
                                                    DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                                    int pageNumber,
            bool excel,
            int pageSize,
                                                    out int totalPageNumber,
                                                    out int recordCount)
        {
            return getQueryEffettuateMethodPagingInternalLiteWithoutTrasmUtente(objOggettoTrasmesso,
                                                            objUtente,
                                                            objRuolo,
                                                            objListaFiltri,
                                                            true,
                                                            pageNumber,
                                                            excel,
                                                            pageSize,
                                                            out totalPageNumber,
                                                            out recordCount);
        }


        private static System.Collections.ArrayList getQueryEffettuateMethodPagingInternalLite(
                                    DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                                    DocsPaVO.utente.Utente objUtente,
                                    DocsPaVO.utente.Ruolo objRuolo,
                                    DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                    bool ricercaCondizioniVisibilitaUtente,
                                    int pageNumber,
                                    bool excel,
                                    int pageSize,
                                    out int totalPageNumber,
                                    out int recordCount)
        {
            logger.Debug("getQueryEffettuateMethodPagingLite");
            System.Collections.ArrayList lista = new System.Collections.ArrayList();

            try
            {
                bool repeatQuery = false;

                DataSet dataSet = null;

                // Esecuzione query paginata
                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                strDB.getQueryTrasmEffPagingLite(ref repeatQuery, out dataSet, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri, pageNumber,excel,pageSize, out totalPageNumber, out recordCount);

                repeatQuery = false;

                lista = getListaTrasmissioniLite(dataSet, repeatQuery, "E", false, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri,excel);

                dataSet.Dispose();
                dataSet = null;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                logger.Debug("Errore nella gestione delle trasmissioni (getQueryEffettuateMethodPagingLite)", e);
                throw new Exception(e.Message);
            }

            return lista;
        }


        private static System.Collections.ArrayList getQueryEffettuateMethodPagingInternalLiteWithoutTrasmUtente(
                            DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso,
                            DocsPaVO.utente.Utente objUtente,
                            DocsPaVO.utente.Ruolo objRuolo,
                            DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                            bool ricercaCondizioniVisibilitaUtente,
                            int pageNumber,
                            bool excel,
                            int pageSize,
                            out int totalPageNumber,
                            out int recordCount)
        {
            logger.Debug("getQueryEffettuateMethodPagingLite");
            System.Collections.ArrayList lista = new System.Collections.ArrayList();

            try
            {
                bool repeatQuery = false;

                DataSet dataSet = null;

                // Esecuzione query paginata
                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                strDB.getQueryTrasmEffPagingLiteWithoutTrasmUtente(ref repeatQuery, out dataSet, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri, pageNumber, excel, pageSize, out totalPageNumber, out recordCount);

                repeatQuery = false;

                lista = getListaTrasmissioniLiteWithoutTrasmUtente(dataSet, repeatQuery, "E", false, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri, excel);

                dataSet.Dispose();
                dataSet = null;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                logger.Debug("Errore nella gestione delle trasmissioni (getQueryEffettuateMethodPagingLite)", e);
                throw new Exception(e.Message);
            }

            return lista;
        }



        
        #endregion Trasmissione

        #region todolist

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="docOrFasc"></param>
        /// <param name="registri"></param>
        /// <param name="filter"></param>
        /// <param name="objectId">Lista degli elementi selezionati della todolist</param>
        /// <returns></returns>
        public static ArrayList getToDoList(DocsPaVO.utente.InfoUtente infoUtente,
                     string docOrFasc, string registri,
                     DocsPaVO.filtri.FiltroRicerca[] filter,
                     String[] objectId)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();

            return trasm.getTodoList(docOrFasc, infoUtente.idPeople, infoUtente.idGruppo, registri, filter, objectId);
        }


        #endregion

        #region Query per Report
        public static void getTrasmissioniDocFasc(out DataTable dt, DocsPaVO.trasmissione.OggettoTrasm obj, string tipoDest)
        {
            DataSet dataSet;
            string oggettoTrasm = "getTrasmissioniDocFasc relative a";
            if (obj.infoDocumento != null)
                oggettoTrasm += "DOCUMENTO: " + obj.infoDocumento.idProfile;
            else
                oggettoTrasm += "FASCICOLO: " + obj.infoFascicolo.idFascicolo;
            logger.Debug(oggettoTrasm);
            DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            strDB.getTrasmissioniDocFasc(out dataSet, obj, tipoDest);
            if (dataSet != null)
                dt = dataSet.Tables["REP_TRASMISSIONI"];
            else
                dt = null;
        }

        public static void getTrasmissioniUO(out DataTable dt, DocsPaVO.filtri.FiltroRicerca[] filtriTrasm, string tipoTrasm)
        {
            DataSet dataSet;
            DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            strDB.getTrasmissioniUO(out dataSet, filtriTrasm, tipoTrasm);
            if (dataSet != null)
                dt = dataSet.Tables["REP_TRASMISSIONI"];
            else
                dt = null;
        }
        #endregion

        #region InfoTrasmissioni

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocOrFasc"></param>
        /// <param name="docOrFas"></param>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.InfoTrasmissione[] GetInfoTrasmissioniEffettuate(
                DocsPaVO.utente.InfoUtente infoUtente,
                string idDocOrFasc, string docOrFas,
                DocsPaVO.ricerche.SearchPagingContext pagingContext)
        {
            using (DocsPaDB.Query_DocsPAWS.Trasmissione dbTrasm = new DocsPaDB.Query_DocsPAWS.Trasmissione())
                return dbTrasm.GetInfoTrasmissioniEffettuate(infoUtente, idDocOrFasc, docOrFas, pagingContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idFascicolo"></param>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.InfoTrasmissione[] GetInfoTrasmissioniRicevute(
                    DocsPaVO.utente.InfoUtente infoUtente,
                    string idDocOrFasc, string docOrFas,
                    DocsPaVO.ricerche.SearchPagingContext pagingContext)
        {
            using (DocsPaDB.Query_DocsPAWS.Trasmissione dbTrasm = new DocsPaDB.Query_DocsPAWS.Trasmissione())
                return dbTrasm.GetInfoTrasmissioniRicevute(infoUtente, idDocOrFasc, docOrFas, pagingContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocOrFasc"></param>
        /// <param name="docOrFas"></param>
        /// <param name="pagingContext"></param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.InfoTrasmissione[] GetInfoTrasmissioniFiltered(
                DocsPaVO.utente.InfoUtente infoUtente
                , string idDocOrFasc, string docOrFasc
                , DocsPaVO.filtri.FiltroRicerca[] objListaFiltri
                , ref DocsPaVO.ricerche.SearchPagingContext pagingContext)
        {
            DocsPaVO.filtri.FiltroRicerca f;
            for (int i = 0; i < objListaFiltri.Length; i++)
            {
                f = objListaFiltri[i];
                if (f.valore != null && !f.valore.Equals(""))
                {
                    switch (f.argomento)
                    {
                        case "MITTENTE_UTENTE":
                            f.valore = BusinessLogic.Utenti.UserManager.getUtente(f.valore.Split('|')[0], f.valore.Split('|')[1]).idPeople;
                            break;
                        case "DESTINATARIO_UTENTE":
                            f.valore = BusinessLogic.Utenti.UserManager.getUtente(f.valore.Split('|')[0], f.valore.Split('|')[1]).idPeople;
                            break;
                    }
                }
            }

            using (DocsPaDB.Query_DocsPAWS.Trasmissione dbTrasm = new DocsPaDB.Query_DocsPAWS.Trasmissione())
                return dbTrasm.GetInfoTrasmissioniFiltered(infoUtente, idDocOrFasc, docOrFasc, objListaFiltri, ref pagingContext);
        }

        /// <summary>
        /// Reperimento delle informazioni di stato relative ad una trasmissione utente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idTrasmissioneUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.StatoTrasmissioneUtente getStatoTrasmissioneUtente(DocsPaVO.utente.InfoUtente infoUtente, string idTrasmissioneUtente)
        {
            using (DocsPaDB.Query_DocsPAWS.Trasmissione dbTrasm = new DocsPaDB.Query_DocsPAWS.Trasmissione())
                return dbTrasm.getStatoTrasmissioneUtente(infoUtente, idTrasmissioneUtente);
        }

        #endregion
    }
}
