using System;
using System.Data;
using System.Collections;
using log4net;

namespace BusinessLogic.Fascicoli 
{
	/// <summary>
	/// </summary>
	public class TitolarioManager 
	{
        private static ILog logger = LogManager.GetLogger(typeof(TitolarioManager));
		/// <summary>
		/// </summary>
		/// <param name="nodoTitolario"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static DocsPaVO.fascicolazione.Classificazione updateTitolario(DocsPaVO.fascicolazione.Classificazione nodoTitolario) 
		{
			#region Codice Commentato
			/*logger.Debug("updateTitolario");
			if(nodoTitolario != null && nodoTitolario.systemID != null) {
				if (!(nodoTitolario.descrizione != null && !nodoTitolario.descrizione.Equals("")))
					throw new Exception("Verificare il campo descrizione");
				
				string updateStr =
					"UPDATE PROJECT SET DESCRIPTION = '" + nodoTitolario.descrizione.Replace("'", "''") + "' WHERE SYSTEM_ID=" + nodoTitolario.systemID;
				logger.Debug(updateStr);
				DocsPa_V15_Utils.Database db = DocsPa_V15_Utils.dbControl.getDatabase();
				db.openConnection();
				db.executeNonQuery(updateStr);
				db.closeConnection();
			}
			return nodoTitolario;*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			
			DocsPaVO.fascicolazione.Classificazione result = fascicoli.UpdateTitolario(nodoTitolario);

			if(result == null)
			{
				logger.Debug("Errore nella gestione del titolario. (updateTitolario)");
				throw new Exception("Verificare il campo descrizione");				
			}

			return result;
		}


        public static bool fascicolazionCanAddFasc(string idNodo)
        {
            bool rtn = false;
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			string sql="SELECT CHA_RW FROM PROJECT WHERE SYSTEM_ID="+idNodo;
            using (IDataReader reader = fascicoli.ExecuteReader(sql))
            {
                if (reader.Read())
                {
                    if(!reader.IsDBNull(0))
                      rtn = reader.GetString(0).Equals("W"); //"R" vuol dire READ only , quindi non si creano fasc/sfasc
                }
            }
            return rtn;
        
        }

        ///// <summary>
        ///// </summary>
        ///// <param name="nodoTitolario"></param>
        ///// <param name="infoUtente"></param>
        //public static void deleteTitolario(DocsPaVO.fascicolazione.Classificazione nodoTitolario, DocsPaVO.utente.InfoUtente infoUtente) 
        //{
        //    ArrayList listaID = new ArrayList();
        //    listaID.Add(nodoTitolario.systemID);
        //    /*DocsPa_V15_Utils.Database db = DocsPa_V15_Utils.dbControl.getDatabase();*/
        //    int numFigli = 1;
        //    try {
        //        //db.openConnection();
        //        listaID = ProjectsManager.getChildren(/*db,*/ "T", listaID);
        //        // posso cancellare il folder solo se non ha figli
        //        string idProject = (string)listaID[0];
        //        for (int i=1; i < listaID.Count; i++)
        //            idProject += "," + (string)listaID[i];

        //        #region Codice Commentato
        //        /*string queryString =
        //            "SELECT COUNT(*) FROM PROJECT_COMPONENTS WHERE PROJECT_ID IN (" + idProject + ")";
        //        logger.Debug(queryString);
        //        numFigli = Int32.Parse(db.executeScalar(queryString).ToString());*/
        //        #endregion

        //        DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

        //        numFigli = fascicoli.DeleteTitolario(idProject);
        //        fascicoli.Dispose();
				

        //        //db.closeConnection();
        //    } catch (Exception e) {
        //        logger.Debug(e.Message);				
        //        //db.closeConnection();				

        //        logger.Debug("Errore nella gestione dei fascicoli. (deleteTitolario)",e);
        //        throw e;				
        //    }
        //    if (numFigli == 0) 
        //    {				
        //        for (int i=0; i < listaID.Count; i++)
        //        {
        //            //ProjectsManager.deletePCDProject((string)listaID[i], infoUtente);

        //            string library = DocsPaDB.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary();
        //            DocsPaDocumentale.Documentale.ProjectManager documentManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente, library);
					
        //            if(!documentManager.DeleteProject((string)listaID[i]))
        //            {
        //                logger.Debug("Errore nella gestione dei fascicoli. (deleteTitolario)");
        //                throw new Exception("Errore della cancellazione di un progetto");						
        //            }
        //        }
        //    } 
        //    else
        //    {
        //        logger.Debug("Errore nella gestione dei fascicoli. Il titolario contiene dei documenti. (deleteTitolario)");
        //        throw new Exception("Il titolario contiene dei documenti");				
        //    }
        //}

		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="nodoTitolario"></param>
		/// <returns></returns>
		public static string getCodiceFiglioTitolario(string idAmministrazione, DocsPaVO.fascicolazione.Classificazione nodoTitolario) 
		{
			int nextVal = 1;	
			string codiceParent = null;	
			
			if(nodoTitolario != null && nodoTitolario.systemID != null) 
			{
				string separator = DocsPaDB.Utils.Personalization.getInstance(idAmministrazione).getSeparator();
				codiceParent = nodoTitolario.codice + separator;				
			
				if(nodoTitolario.childs != null && nodoTitolario.childs.Count > 0) 
				{
					for (int i=0; i < nodoTitolario.childs.Count; i++) 
					{
						string tmp = codiceParent + nextVal.ToString();
					
						if(((DocsPaVO.fascicolazione.Classificazione)nodoTitolario.childs[i]).codice.Equals(tmp)) 
						{
							nextVal++;
							i = 0;
						}
					}					
				}
			} 
			else 
			{
				#region Codice Commentato
				/*string queryStr =
					"SELECT COUNT(*) FROM PROJECT WHERE CHA_TIPO_PROJ = 'T' " +
					"AND (NUM_LIVELLO = 0 OR NUM_LIVELLO IS NULL) AND ID_AMM = " + infoUtente.idAmministrazione;
				logger.Debug(queryStr);
				nextVal = Int32.Parse(db.executeScalar(queryStr).ToString()) + 1;*/
				#endregion

				DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
				nextVal = Int32.Parse(fascicoli.GetCodiceFiglioTitolario(idAmministrazione)) + 1;
				fascicoli.Dispose();
			}

			string ret = checkCodice(idAmministrazione, codiceParent, nextVal);

			while(ret == null)
			{
				ret = checkCodice(idAmministrazione, codiceParent, nextVal++);
			}

			return ret;
		}

		/// <summary>
		/// </summary>
		/// <param name="codiceParent"></param>
		/// <param name="nextVal"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		private static string checkCodice(string idAmministrazione, string codiceParent, int nextVal) 
		{
			#region Codice Commentato
			/*string ret = "";
			if(codiceParent != null)
				ret = codiceParent;
			ret += nextVal.ToString();
			string queryStr =
				"SELECT COUNT(*) FROM PROJECT WHERE CHA_TIPO_PROJ = 'T' " +
				"AND VAR_CODICE = '" + ret + 
				"' AND ID_AMM = " + infoUtente.idAmministrazione;
			logger.Debug(queryStr);
			if(!db.executeScalar(queryStr).ToString().Equals("0"))		
				ret = null;
			return ret;*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			string ret = fascicoli.CheckCodice(idAmministrazione,codiceParent,nextVal.ToString());
			fascicoli.Dispose();
			return ret ;
		}

		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="nodoTitolario"></param>
		/// <returns></returns>
		public static ArrayList getAutorizzazioniNodoTitolario(DocsPaVO.fascicolazione.Classificazione nodoTitolario) 
		{
			ArrayList listaDiritti = null;
			
			try 
			{
				listaDiritti = ProjectsManager.getVisibilita(nodoTitolario.systemID, false,"0");
			} 
			catch(Exception e) 
			{			
				logger.Debug("Errore nella gestione dei fascicoli. (getAutorizzazioniNodoTitolario)", e);
				throw new Exception(e.Message);				
			}

			return listaDiritti;
		}

		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="nodoTitolario"></param>
		/// <param name="corrAdd"></param>
		/// <param name="corrRemove"></param>
		/// <param name="ereditaDiritti"></param>
		public static void setAutorizzazioniNodoTitolario(DocsPaVO.fascicolazione.Classificazione nodoTitolario, DocsPaVO.utente.Corrispondente[] corrAdd, DocsPaVO.utente.Corrispondente[] corrRemove, bool ereditaDiritti) 
		{
			logger.Debug("setAutorizzazioniNodoTitolario");
			ArrayList listaID = new ArrayList();

			#region Codice Commentato
//			DocsPa_V15_Utils.database.SqlServerAgent db = new DocsPa_V15_Utils.database.SqlServerAgent();
//			//db.openConnection();
//			if(!ereditaDiritti)
//				checkChildsPermission(/*db,*/ nodoTitolario, corrRemove);
//			try {
//				db.beginTransaction();
//				ArrayList listaID = new ArrayList();
//				listaID.Add(nodoTitolario.systemID);
//				if(nodoTitolario.childs != null) {
//					for(int i=0; i < nodoTitolario.childs.Count; i++) 
//						listaID.Add(((DocsPaVO.fascicolazione.Classificazione)nodoTitolario.childs[i]).systemID);
//				}
//				ProjectsManager.setVisibilita(/*db,*/ "T", listaID, corrAdd, corrRemove);
//				db.commitTransaction();
//				//db.closeConnection();	
//			} catch (Exception e) {
//				//db.closeConnection();				
//				throw new Exception(e.Message);
//			}
			#endregion

			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			fascicoli.SetAutorizzazioniNodoTitolario(nodoTitolario,corrAdd,corrRemove,ereditaDiritti);
			fascicoli.Dispose();
		}

		#region Metodo Commentato
//		/// <summary>
//		/// con la diversa strutturra dell'applicazione
//		/// questo metodo non viene più chiamato
//		/// </summary>
//		private static void checkChildsPermission(/*DocsPa_V15_Utils.Database db,*/ DocsPaVO.fascicolazione.Classificazione nodoTitolario, DocsPaVO.utente.Corrispondente[] corrRemove) {
//			if(corrRemove == null || nodoTitolario.childs == null)
//				return;
//			if(corrRemove.Length > 0 && nodoTitolario.childs.Count > 0) {
//				string personOrGroup = "(" + ProjectsManager.getIdUtenteRuolo(corrRemove[0]);
//				for (int i=1; i < corrRemove.Length; i++) 
//					ProjectsManager.getIdUtenteRuolo(corrRemove[i]);
//				personOrGroup += ")";
//				string thing = "(" + ((DocsPaVO.fascicolazione.Classificazione)nodoTitolario.childs[0]).systemID;
//
//				for(int i=1; i < nodoTitolario.childs.Count; i++) 
//					thing += "," + ((DocsPaVO.fascicolazione.Classificazione)nodoTitolario.childs[i]).systemID;
//				thing += ")";
//
//				/*string queryStr =
//					"SELECT COUNT(*) FROM SECURITY WHERE PERSONORGROUP IN " + personOrGroup +
//					" AND THING IN " + thing;*/
//				//logger.Debug(queryStr);
//				DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
//				
//				
//				
//				//if(!db.executeScalar(queryStr).ToString().Equals("0"))
//				int securityCount = fascicoli.GetSecurityCount(personOrGroup,thing) ;
//				fascicoli.Dispose();
//				
//				if (securityCount.ToString().Equals("0"))
//					throw new Exception("Impossibile rimuovere i ruoli selezionati");				
//			}
//		}
		#endregion
	
		public string filtroRicTitDocspa(string codice, string descrizione, string note, string indice, string idAmm, string idGruppo, string idRegistro, string idTitolario)
		{
			System.Data.DataSet ds;

			string result;

			try
			{
				DocsPaDB.Query_DocsPAWS.Fascicoli obj = new DocsPaDB.Query_DocsPAWS.Fascicoli();
				ds = obj.filtroRicTitolarioDocspa(codice, descrizione, note, indice, idAmm, idGruppo, idRegistro, idTitolario);
				if(ds != null)
				{
					result = ds.GetXml();
				}
				else
				{
					result = null;
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="registro"></param>
		/// <param name="codiceClassifica"></param>
		/// <returns></returns>
		public static System.Collections.ArrayList getTitolario(string idAmministrazione, string idGruppo, string idPeople, DocsPaVO.utente.Registro registro, string codiceClassifica,bool getFigli) 
		{
			System.Collections.ArrayList listaObject= new System.Collections.ArrayList();

			#region Codice Commentato
			/*logger.Debug("getTitolario");
			DocsPa_V15_Utils.Database database=DocsPa_V15_Utils.dbControl.getDatabase();
			
			database.openConnection();
			DataSet dataSet= new DataSet();
			ArrayList listaObject= new ArrayList();
            
			try {
				string condRegistro = "";
				if (registro != null)
					condRegistro = " AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO='" + registro.systemId + "')";

				//estrae le classificazioni e i fascicoli
				string commandString1 =
					"SELECT DISTINCT A.SYSTEM_ID, A.DESCRIPTION, A.ID_PARENT, A.VAR_CODICE, " +
					"A.NUM_LIVELLO, A.VAR_COD_ULTIMO, A.ID_REGISTRO, " +
					"VAR_COD_LIV1, VAR_COD_LIV2, VAR_COD_LIV3, VAR_COD_LIV4, VAR_COD_LIV5, VAR_COD_LIV6, VAR_COD_LIV7, VAR_COD_LIV8 " +
					"FROM PROJECT A";
				//se sono un amministratore non devo fare la join con la security
				if(infoUtente.idGruppo != null && !infoUtente.idGruppo.Equals(""))
					commandString1 += 
						", SECURITY B WHERE A.SYSTEM_ID=B.THING AND B.ACCESSRIGHTS > 0 AND (B.PERSONORGROUP=" + infoUtente.idPeople + " OR B.PERSONORGROUP=" + infoUtente.idGruppo + ") AND ";
				else
					commandString1 += " WHERE ";
				commandString1 += 
					"ID_AMM='"+infoUtente.idAmministrazione+"' AND CHA_TIPO_PROJ='T'" + condRegistro;
				//istanzia la class di personalizzazione 
				string separator=DocsPaWS.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getSeparator();
               
				if (codiceClassifica != null && !codiceClassifica.Equals("")) {
					commandString1 += " AND (VAR_CODICE = '" + codiceClassifica + "' OR VAR_CODICE LIKE '" + codiceClassifica + separator + "%')";
					//					string[] codici = codiceClassifica.Split(separator.ToCharArray());
					//					for (int i=0; i < codici.Length; i++) {
					//						int numLivello = i+1;
					//						commandString1 += " AND VAR_COD_LIV" + numLivello.ToString() + "= '" + codici[i] + "'";
					//					}
				}	
				commandString1 += " ORDER BY NUM_LIVELLO, VAR_COD_LIV1, VAR_COD_LIV2, VAR_COD_LIV3, VAR_COD_LIV4, VAR_COD_LIV5, VAR_COD_LIV6, VAR_COD_LIV7, VAR_COD_LIV8";
				logger.Debug(commandString1);

				database.fillTable(commandString1,dataSet,"CLASSIFICAZIONI");
				//modifica sabrina --- ??? inserito controllo per verificare se ci sono risultati
				if (dataSet.Tables["CLASSIFICAZIONI"].Rows.Count > 0) {
					string numLivello = dataSet.Tables["CLASSIFICAZIONI"].Rows[0]["NUM_LIVELLO"].ToString();
				
					//si estraggono le classificazioni root
					DataRow[] rootClassRows=dataSet.Tables["CLASSIFICAZIONI"].Select("NUM_LIVELLO=" + numLivello);
					for(int i=0;i< rootClassRows.Length;i++) {
						DocsPaVO.fascicolazione.Classificazione rootClass=new DocsPaVO.fascicolazione.Classificazione();
						rootClass.systemID = rootClassRows[i]["SYSTEM_ID"].ToString();
						rootClass.descrizione= rootClassRows[i]["DESCRIPTION"].ToString();
						rootClass.codice = rootClassRows[i]["VAR_CODICE"].ToString();
						rootClass.codUltimo = getCodUltimo(rootClassRows[i]["VAR_COD_ULTIMO"].ToString());
						if(registro != null)
							rootClass.registro = registro;
						else if(rootClassRows[i]["ID_REGISTRO"] != null)
							rootClass.registro = RegistriManager.getRegistro(database, rootClassRows[i]["ID_REGISTRO"].ToString());

					
						//ricerca delle classificazioni figlie
						ArrayList classificazioni=getClassificazioni(database, rootClass.codice,dataSet.Tables["CLASSIFICAZIONI"],rootClass.systemID, registro, separator);
						for(int k=0;k<classificazioni.Count;k++) {
							rootClass.childs.Add(classificazioni[k]);
						}
						listaObject.Add(rootClass);

					}
				}
				dataSet.Dispose();
				
				database.closeConnection();
			} catch(Exception e){
				logger.Debug (e.Message);				
				
				database.closeConnection();
				throw new Exception("F_System");
			}*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			
			listaObject = fascicoli.GetTitolario(idAmministrazione, idGruppo, idPeople, registro,codiceClassifica,getFigli);
			
			if(listaObject == null)
			{
				logger.Debug("Errore nella gestione dei fascicoli. (getTitolario)");
				throw new Exception("F_System");				
			}

			return listaObject;
		}

        /// <summary>
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="registro"></param>
        /// <param name="codiceClassifica"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getTitolario2(string idAmministrazione, string idGruppo, string idPeople, DocsPaVO.utente.Registro registro, string codiceClassifica, bool getFigli, string idTitolario)
        {
            System.Collections.ArrayList listaObject = new System.Collections.ArrayList();

            
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            listaObject = fascicoli.GetTitolario2(idAmministrazione, idGruppo, idPeople, registro, codiceClassifica, getFigli, idTitolario);

            if (listaObject == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getTitolario)");
                throw new Exception("F_System");
            }

            return listaObject;
        }

//		/// <summary>
//		/// </summary>
//		/// <param name="codice_parent"></param>
//		/// <param name="table"></param>
//		/// <param name="parent_id"></param>
//		/// <param name="registro"></param>
//		/// <param name="separator"></param>
//		/// <returns></returns>
//		private static ArrayList getClassificazioni(string codice_parent, DataTable table, string parent_id, DocsPaVO.utente.Registro registro, string separator)
//		{
//			logger.Debug("getClassificazioni");
//			DataRow[] classificazioniRows=table.Select("ID_PARENT="+parent_id);
//			ArrayList classificazioni=new ArrayList();
//			for(int i=0;i<classificazioniRows.Length;i++) {
//				DocsPaVO.fascicolazione.Classificazione classificazione=new DocsPaVO.fascicolazione.Classificazione();
//				classificazione.codice=classificazioniRows[i]["VAR_CODICE"].ToString();
//				classificazione.descrizione=classificazioniRows[i]["DESCRIPTION"].ToString();
//				classificazione.systemID=classificazioniRows[i]["SYSTEM_ID"].ToString();
//				//classificazione.accessRights = classificazioniRows[i]["ACCESSRIGHTS"].ToString();
//				classificazione.codUltimo = getCodUltimo(classificazioniRows[i]["VAR_COD_ULTIMO"].ToString());
//				if(registro != null)
//					classificazione.registro = registro;
//				else if(classificazioniRows[i]["ID_REGISTRO"] != null)
//					classificazione.registro = BusinessLogic.Utenti.RegistriManager.getRegistro(classificazioniRows[i]["ID_REGISTRO"].ToString());
//
//
//				ArrayList classChildren = getClassificazioni(classificazione.codice,table,classificazioniRows[i]["SYSTEM_ID"].ToString(), registro, separator);
//				for(int j=0;j<classChildren.Count;j++){
//					classificazione.childs.Add(classChildren[j]);
//				}
//				classificazioni.Add(classificazione);
//			}
//			return classificazioni;
//		}

		/// <summary>
		/// </summary>
		/// <param name="idClassificazione"></param>
		/// <param name="codiceClassificazione"></param>
		/// <returns></returns>
		public static DocsPaVO.fascicolazione.Classifica[] getGerarchia(string idClassificazione, string codiceClassificazione,string idAmm) 
		{
			return getGerarchia(idClassificazione, codiceClassificazione, null,idAmm);
		}

		//aggiunta per gestione filtro registro
		/// <summary>
		/// </summary>
		/// <param name="idClassificazione"></param>
		/// <param name="codiceClassificazione"></param>
		/// <param name="registro"></param>
		/// <returns></returns>
		private static DocsPaVO.fascicolazione.Classifica[] getGerarchia(string idClassificazione, string codiceClassificazione, DocsPaVO.utente.Registro registro,string idAmm) 
		{
			DocsPaVO.fascicolazione.Classifica[] lista = null;

			#region Codice Commentato
			/*DocsPa_V15_Utils.Database db = DocsPa_V15_Utils.dbControl.getDatabase();
			try {
				db.openConnection();
				int numLivello = 0;
				string idParent = "0";
				string queryString =
					"SELECT A.VAR_COD_LIV1, A.VAR_COD_LIV2, A.VAR_COD_LIV3, A.VAR_COD_LIV4, " +
					"A.VAR_COD_LIV5, A.VAR_COD_LIV6, A.VAR_COD_LIV7, A.VAR_COD_LIV8, " +
					"A.DESCRIPTION, A.ID_PARENT, A.NUM_LIVELLO, A.VAR_CODICE, A.SYSTEM_ID "+
					"FROM PROJECT A WHERE A.CHA_TIPO_PROJ='T' AND ";

				//add per filtro su registro
				if (registro != null)
				{
					string condRegistro = "";
					condRegistro = " (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO='" + registro.systemId + "') ";
					queryString += condRegistro; 
					queryString += " AND ";
				}
				//end add 

				if(idClassificazione != null)
					queryString += "A.SYSTEM_ID=" + idClassificazione;
				else
					queryString += "A.VAR_CODICE='" + codiceClassificazione + "'";

				logger.Debug(queryString);
				IDataReader dr = db.executeReader(queryString);
				if (dr.Read()) {
					numLivello = Int32.Parse(dr.GetValue(10).ToString());
					lista = new DocsPaVO.fascicolazione.Classifica[numLivello];
					for (int i=0; i<numLivello; i++) {
						lista[i] = new DocsPaVO.fascicolazione.Classifica();
						//lista[i].codice = dr.GetValue(i).ToString();
					}
					numLivello -= 1;
					lista[numLivello].systemId = dr.GetValue(12).ToString();;
					lista[numLivello].descrizione = dr.GetValue(8).ToString();					
					lista[numLivello].codice = dr.GetValue(11).ToString();
					idParent = dr.GetValue(9).ToString();
				} 
				dr.Close();

				while (!idParent.Equals("0") && numLivello > 0) {
					numLivello -= 1;
					lista[numLivello].systemId = idParent;
					queryString = 
						"SELECT DESCRIPTION, ID_PARENT, NUM_LIVELLO, VAR_CODICE " + 
						"FROM PROJECT WHERE SYSTEM_ID=" + idParent;
					logger.Debug(queryString);
					dr = db.executeReader(queryString);
					if (dr.Read()) {
						lista[numLivello].descrizione = dr.GetValue(0).ToString();					
						lista[numLivello].codice = dr.GetValue(3).ToString();
					}
					idParent = dr.GetValue(1).ToString();
					dr.Close();
				}

				db.closeConnection();
			} catch (Exception e) {
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			
			lista = fascicoli.GetGerarchia(idClassificazione,codiceClassificazione,registro,idAmm);

            /*
			if(lista == null)
			{
				logger.Debug("Errore nella gestione dei fascicoli. (newGerarchia)");
				throw new Exception("F_System");								
			}
            */

			return lista;
		}

        //aggiunta per gestione filtro registro
        /// <summary>
        /// </summary>
        /// <param name="idClassificazione"></param>
        /// <param name="codiceClassificazione"></param>
        /// <param name="registro"></param>
        /// <returns></returns>
        private static DocsPaVO.fascicolazione.Classifica[] getGerarchia2(string idClassificazione, string codiceClassificazione, DocsPaVO.utente.Registro registro, string idAmm, string idTitolario)
        {
            DocsPaVO.fascicolazione.Classifica[] lista = null;

            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            lista = fascicoli.GetGerarchia2(idClassificazione, codiceClassificazione, registro, idAmm, idTitolario);

            /*
            if (lista == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (newGerarchia)");
                throw new Exception("F_System");
            }
            */

            return lista;
        }


		#region Codice Commentato
//old:		
//		internal static DocsPaVO.fascicolazione.Classifica[] getGerarchiaDaCodice(string codiceClassificazione) {
//			logger.Debug("getGerarchiaDaCodice");
//			if (codiceClassificazione != null && !codiceClassificazione.Trim().Equals(""))
//				return  getGerarchia(null,codiceClassificazione);
//			else
//				return null;
//		}
//new1:
		#endregion

		/// <summary>
		/// </summary>
		/// <param name="codiceClassificazione"></param>
		/// <returns></returns>
		public static DocsPaVO.fascicolazione.Classifica[] getGerarchiaDaCodice(string codiceClassificazione,string idAmm) 
		{
			logger.Debug("getGerarchiaDaCodice");
			return  getGerarchiaDaCodice(codiceClassificazione, null,idAmm);
		}

		//new2: aggiunta per gestione filtro registro
		/// <summary>
		/// </summary>
		/// <param name="codiceClassificazione"></param>
		/// <param name="registro"></param>
		/// <returns></returns>
		public static DocsPaVO.fascicolazione.Classifica[] getGerarchiaDaCodice(string codiceClassificazione, DocsPaVO.utente.Registro registro,string idAmm) 
		{
			logger.Debug("getGerarchiaDaCodice");

			if(codiceClassificazione != null && !codiceClassificazione.Trim().Equals(""))
			{
				return getGerarchia(null, codiceClassificazione, registro,idAmm);
			}
			else
			{
				return null;
			}
		}

        public static DocsPaVO.fascicolazione.Classifica[] getGerarchiaDaCodice2(string codiceClassificazione, DocsPaVO.utente.Registro registro, string idAmm, string idTitolario)
        {
            logger.Debug("getGerarchiaDaCodice");

            if (codiceClassificazione != null && !codiceClassificazione.Trim().Equals(""))
            {
                return getGerarchia2(null, codiceClassificazione, registro, idAmm, idTitolario);
            }
            else
            {
                return null;
            }
        }

		/// <summary>
		/// </summary>
		/// <param name="idClassificazione"></param>
		/// <returns></returns>
		public static DocsPaVO.fascicolazione.Classifica[] getGerarchia(string idClassificazione,string idAmm) 
		{
			logger.Debug("getGerarchia");
			if (idClassificazione != null && !idClassificazione.Trim().Equals(""))
				return getGerarchia(idClassificazione, null,idAmm);
			else 
				return null;
		}

//		/// <summary>
//		/// </summary>
//		/// <param name="codice"></param>
//		/// <returns></returns>
//		private static string getCodUltimo (string codice) 
//		{
//			if (!(codice != null && !codice.Equals("")))
//				return "1";
//			try {
//				int numCodice = Int32.Parse(codice) + 1;
//				return numCodice.ToString();
//			}  catch (Exception) {}
//			return "";
//		}

		/// <summary>
		/// </summary>
		/// <param name="classifica"></param>
		/// <param name="idRegistro"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static ArrayList getFigliClassifica2(string idGruppo, string idPeople, DocsPaVO.fascicolazione.Classifica classifica, string idRegistro,string idAmm, string idTitolario)
		{
			ArrayList lista = new ArrayList();

			#region Codice Commentato
			/*DocsPa_V15_Utils.Database db = DocsPa_V15_Utils.dbControl.getDatabase();
			try {
				db.openConnection();
				string queryString = 
					"SELECT A.SYSTEM_ID, A.VAR_COD_LIV1, A.VAR_COD_LIV2, A.VAR_COD_LIV3, " +
					"A.VAR_COD_LIV4, A.VAR_COD_LIV5, A.VAR_COD_LIV6, A.VAR_COD_LIV7, " +
					"A.VAR_COD_LIV8, A.DESCRIPTION, A.NUM_LIVELLO, A.VAR_CODICE " + 
					"FROM PROJECT A, SECURITY B  WHERE A.SYSTEM_ID=B.THING AND A.CHA_TIPO_PROJ='T' AND " + 
					"(B.PERSONORGROUP=" + infoUtente.idGruppo + " OR B.PERSONORGROUP=" + infoUtente.idPeople + ")  AND B.ACCESSRIGHTS > 0 " +
					" AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO='" + idRegistro + "')";
				if (classifica != null && classifica.systemId != null)
					queryString += " AND A.ID_PARENT=" + classifica.systemId;
				else
					queryString += " AND A.NUM_LIVELLO=1"; 

				logger.Debug(queryString);
				IDataReader dr = db.executeReader(queryString);
				while (dr.Read()) {	
					DocsPaVO.fascicolazione.Classifica c = new DocsPaVO.fascicolazione.Classifica();
					
					c.systemId = dr.GetValue(0).ToString();	
					//int numLivello = Int32.Parse(dr.GetValue(10).ToString());
					//c.codice = dr.GetValue(numLivello).ToString();
					c.codice = dr.GetValue(11).ToString();
					c.descrizione = dr.GetValue(9).ToString();

					lista.Add(c);
				}
				dr.Close();

				db.closeConnection();
			} catch (Exception e) {
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			
			lista = fascicoli.GetFigliClassifica2(idGruppo, idPeople, classifica,idRegistro,idAmm, idTitolario);

			if(lista == null)
			{
				logger.Debug("Errore nella gestione dei fascicoli. (getFigliClassifica)");
				throw new Exception("F_System");				
			}

			fascicoli.Dispose();
			return lista;
		}

        /// <summary>
        /// </summary>
        /// <param name="classifica"></param>
        /// <param name="idRegistro"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static ArrayList getFigliClassifica(string idGruppo, string idPeople, DocsPaVO.fascicolazione.Classifica classifica, string idRegistro, string idAmm)
        {
            ArrayList lista = new ArrayList();

            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            lista = fascicoli.GetFigliClassifica(idGruppo, idPeople, classifica, idRegistro, idAmm);

            if (lista == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli. (getFigliClassifica)");
                throw new Exception("F_System");
            }

            fascicoli.Dispose();
            return lista;
        }

		public static string getDataFromProject(string campo, string condizione)
		{
			string retValue = string.Empty;

			DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
			retValue = fascicoli.GetProjectData(campo, condizione);

			return retValue;
		}
	}
}
