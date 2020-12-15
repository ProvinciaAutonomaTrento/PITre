using System;
using System.Collections;
using log4net;

namespace BusinessLogic.Documenti
{
	/// <summary>
	/// </summary>
	public class Oggettario
	{
        private static ILog logger = LogManager.GetLogger(typeof(Oggettario));
		/// <summary>
		/// </summary>
		/// <param name="oggetto"></param>
		/// <param name="registro"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.Oggetto inserisciOggetto(string idAmministrazione, DocsPaVO.documento.Oggetto oggetto, DocsPaVO.utente.Registro registro,ref string errMsg) 
		{
			logger.Debug("START : DocsPAWS > documenti > Oggettario.cs > inserisciOggetto");
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			
			logger.Debug("CALL : InsertOggettario");
			oggetto = doc.InsertOggettario(idAmministrazione, oggetto, registro,ref errMsg);

			#region Codice Commentato
			/*DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			
			db.openConnection();
			string id_registro;
			string id_amministrazione;
			if (registro != null)
			{
				id_registro = registro.systemId;
			}
			else
			{
				id_registro = "null";
			}
			id_amministrazione = infoUtente.idAmministrazione;

			string numOggetti= checkOggetto(oggetto, registro,id_amministrazione,db);
			if (!numOggetti.Equals("0"))
            {
				throw new Exception("Oggetto già presente");
			}
			
			try 
			{
				string insertString = 
					"INSERT INTO DPA_OGGETTARIO " +
					"(" + DocsPaWS.Utils.dbControl.getSystemIdColName() + " ID_REGISTRO, ID_AMM, VAR_DESC_OGGETTO, CHA_OCCASIONALE) " +
					" VALUES (" + DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_OGGETTARIO") +
					id_registro + ", " + id_amministrazione + ", '" + oggetto.descrizione.Replace("'", "''") + "', '0')";
				
				logger.Debug(insertString);
				oggetto.systemId = db.insertLocked(insertString, "DPA_OGGETTARIO");
							
				db.closeConnection();

			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}*/
			#endregion

			logger.Debug("END : DocsPAWS > documenti > Oggettario.cs > inserisciOggetto");
			return oggetto;			
		}


        /// <summary>
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="oggetto"></param>
        /// <returns></returns>
        public static bool modificaOggetto(string idAmministrazione, DocsPaVO.documento.Oggetto oggetto)
        {
            bool result;
            logger.Debug("START : DocsPAWS > documenti > Oggettario.cs > modificaOggetto");
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            logger.Debug("CALL : UpdateOggettario");
            result = doc.UpdateOggettario(idAmministrazione, oggetto);

            logger.Debug("END : DocsPAWS > documenti > Oggettario.cs > modificaOggetto");
            return result;
        }

		#region Metodo Commentato
		/*protected static string checkOggetto(DocsPaVO.documento.Oggetto oggetto, DocsPaVO.utente.Registro registro, string id_amministrazione, DocsPaWS.Utils.Database db) 
		{
			//si verifica se l'oggetto è già presente
			string id_registro;
			
			if (registro != null) 
			{
				id_registro = registro.systemId;
			}
			else
			{
				id_registro = null;
			}

			string selectString =
				"SELECT COUNT(*) FROM DPA_OGGETTARIO WHERE upper(VAR_DESC_OGGETTO)='"+ oggetto.descrizione.ToUpper() +"'";
			if (id_registro != null)
			{
				selectString += " AND (ID_REGISTRO =" + id_registro + " OR ID_REGISTRO IS NULL) ";
			}
			if (id_amministrazione != null && !id_amministrazione.Equals(""))
			{
				selectString += " AND ID_AMM =" + id_amministrazione;
			}
			selectString += " AND CHA_OCCASIONALE='0'";
			logger.Debug(selectString);
			string numPar=db.executeScalar(selectString).ToString();
			return numPar;
			string numPar;
			DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();
			obj.getNumOggetti(out numPar, oggetto.descrizione, id_registro, id_amministrazione);
			return numPar;
		}*/
		#endregion

		/// <summary>
		/// </summary>
		/// <param name="idProfile"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static ArrayList getListaStoriciOggetto(string idProfile) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			return doc.GetListaStoriciOggetto(idProfile);
				
			#region Codice Commentato
			/*logger.Debug("getStoricoOggetto");
			ArrayList lista = new ArrayList();
			string queryString =
				"SELECT A.SYSTEM_ID, A.ID_PEOPLE, A.ID_RUOLO_IN_UO, " +
				DocsPaWS.Utils.dbControl.toChar("A.DTA_MODIFICA",false) + " AS DTA_MODIFICA, " +
				"B.VAR_DESC_OGGETTO, B.CHA_OCCASIONALE " +
				"FROM DPA_OGGETTI_STO A, DPA_OGGETTARIO B " +
				"WHERE A.ID_OGGETTO=B.SYSTEM_ID AND A.ID_PROFILE=" + idProfile;
			
			DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			try 
			{
				db.openConnection();					
				DataSet dataSet;
				DocsPaWS.Utils.Logger.log("Query storico", logLevelTime);
				//db.fillTable(queryString, dataSet, "STORICO");	
				
				DocsPaWS.Utils.Logger.log("Dopo query storico", logLevelTime);	
				foreach (DataRow dataRow in dataSet.Tables["STORICO"].Rows)
					lista.Add(getStoricoOggetto (db, dataRow));
				dataSet.Dispose();	
				db.closeConnection();
			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}
			return lista;*/
			#endregion
		}

        public static ArrayList getListaLog(string idOggetto, string varOggetto)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.getListaLog(idOggetto, varOggetto);
        }

        public static ArrayList getListaLog(string idOggetto, string idFolder, string varOggetto, DocsPaVO.filtri.FilterVisibility[] filter)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.getListaLogFilter(idOggetto, idFolder, varOggetto,filter);
        }
		#region Metodo Commentato
		/*private static DocsPaVO.documento.StoricoOggetto getStoricoOggetto(DocsPaWS.Utils.Database db, DataRow dataRow) 
		{
			DocsPaVO.documento.StoricoOggetto storico = new DocsPaVO.documento.StoricoOggetto();
			storico.systemId = dataRow["SYSTEM_ID"].ToString();
			storico.dataModifica = dataRow["DTA_MODIFICA"].ToString();
			if (dataRow["CHA_OCCASIONALE"] != null)
			{
				storico.occasionale = dataRow["CHA_OCCASIONALE"].ToString();
			}
			storico.descrizione = dataRow["VAR_DESC_OGGETTO"].ToString();
			storico.utente = UserManager.getUtente(db, dataRow["ID_PEOPLE"].ToString());
			storico.ruolo = UserManager.getRuolo(db, dataRow["ID_RUOLO_IN_UO"].ToString());

			return storico;
		}*/
		#endregion	
	}
}
