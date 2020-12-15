using System;
using System.Collections;
using System.Data;
using System.Configuration;
using log4net;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BusinessLogic.Trasmissioni
{
	/// <summary>
	/// </summary>
	public class TrasmManager 
	{
        private static ILog logger = LogManager.GetLogger(typeof(TrasmManager));
		/// <summary>
		/// </summary>
		/// <param name="objTrasm"></param>
		/// <returns></returns>
		public static DocsPaVO.trasmissione.Trasmissione saveTrasmMethod(DocsPaVO.trasmissione.Trasmissione objTrasm)
		{
            logger.Info("BEGIN");
			logger.Debug("saveTrasmMethod");

			checkInputData(objTrasm);

			try 
			{
				objTrasm = saveTrasmissione(objTrasm);

				// aggiorno il campo assegnazione
				setAssegnazione(objTrasm);

				string basePathFiles=ConfigurationManager.AppSettings["LOG_PATH"];
				basePathFiles = basePathFiles.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
				basePathFiles = basePathFiles+"\\Interoperabilita";				
			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);				
				logger.Debug("Errore nella gestione delle trasmissioni (saveTrasmMethod)",e);
				throw e ;
			}
            logger.Info("END");
			return objTrasm;
		}
        /// <summary>
        /// return dta_vista 
        /// </summary>
        /// <param name="idTrasmUt"></param>
        /// <returns></returns>
        public static string getDataVistaTrasmUt(string idTrasmUt)
        {
            string dtaVista=string.Empty;
            using (DocsPaDB.DBProvider db = new DocsPaDB.DBProvider())
            {
                try
                {
                    string sql="SELECT "+DocsPaDbManagement.Functions.Functions.ToChar("DTA_VISTA",false)+" FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID="+idTrasmUt;
                    DocsPaUtils.Query q=new DocsPaUtils.Query(sql);
                   if(!db.ExecuteScalar(out dtaVista,sql)) 
                        throw new Exception("Errore durante il reperimento della dta_vista della trasm utente con id "+idTrasmUt);
                    else
                    return dtaVista;
                    
                }
                catch (Exception e)
                {
                    logger.Debug(e);
                    return dtaVista;
                }
            }
        
        
        
        }
		/// <summary>
		/// </summary>
		/// <param name="objTrasm"></param>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.Trasmissione saveTrasmissione(DocsPaVO.trasmissione.Trasmissione objTrasm) 
		{
            logger.Info("BEGIN");
            bool inModifica = false;
            string id = string.Empty;
            DocsPaDB.Query_DocsPAWS.Trasmissione objQuery = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            DocsPaVO.trasmissione.Trasmissione trasmInUscita = new DocsPaVO.trasmissione.Trasmissione();
            DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola;
            
			logger.Debug("saveTrasmissione");

			// se la trasmissione è stata inviata non devo fare nulla
			if (objTrasm.dataInvio != null && !objTrasm.dataInvio.Equals(""))
				throw new Exception("Trasmissione già inviata");

            // verifica se il documento non è in cestino
            if (objTrasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                string incestino = BusinessLogic.Documenti.DocManager.checkdocInCestino(objTrasm.infoDocumento.docNumber);
                if (incestino != "" && incestino == "1")
                    throw new Exception("Il documento è stato rimosso, non è più possibile trasmetterlo");
            }

            if (objTrasm.systemId != null && !objTrasm.systemId.Equals(""))
            {
                inModifica = true;
                trasmInUscita = updateTrasmissione(objTrasm);
            }
            else           
            {
                trasmInUscita = insertTrasmissione(objTrasm);
                objTrasm.systemId = trasmInUscita.systemId;
            }

            if (inModifica)
            {
                // elimina tutte trasm.ni utente e trasm.ni singole
                objQuery.DeleteTrasmSingola(objTrasm);
                // e deve inserire tutte le trasm.ni singole come se fossero nuove
                for (int i = 0; i < objTrasm.trasmissioniSingole.Count; i++)
                {
                    objTrasmSingola = (DocsPaVO.trasmissione.TrasmissioneSingola)objTrasm.trasmissioniSingole[i];
                    objTrasmSingola.systemId = "";
                    objTrasm.trasmissioniSingole[i] = objTrasmSingola; 
                }
            }

            //for (int i = 0; i < objTrasm.trasmissioniSingole.Count; i++)
            ////foreach (DocsPaVO.trasmissione.TrasmissioneSingola trs in objTrasm.trasmissioniSingole)
            //{               
            //    id = objQuery.insertTrasmSingola(trs, objTrasm.systemId);
            //    trs.systemId = id;
            //    logger.Debug("idTrasmSingola = " + id);

            //    foreach (DocsPaVO.trasmissione.TrasmissioneUtente tru in trs.trasmissioneUtente)
            //    {
            //        trasmUtenteInUscita = new DocsPaVO.trasmissione.TrasmissioneUtente();
            //        trasmUtenteInUscita = objQuery.insertTrasmUtente(tru, id);
            //        tru.systemId = trasmUtenteInUscita.systemId;
            //    }
            //}

           
            for (int i = 0; i < objTrasm.trasmissioniSingole.Count; i++)
            {
                objTrasmSingola = (DocsPaVO.trasmissione.TrasmissioneSingola)objTrasm.trasmissioniSingole[i];
                if (objTrasmSingola!=null)
                objTrasmSingola = saveTrasmSingola(objTrasmSingola, objTrasm.systemId);
                if (objTrasmSingola != null)
                    objTrasm.trasmissioniSingole[i] = objTrasmSingola;                
            }			
            //// elimino le trasmissioni singole che devono essere cancellate
            //for (int i=idDaEliminare.Count; i > 0 ; i--)
            //    objTrasm.trasmissioniSingole.RemoveAt((int)idDaEliminare[i-1]);

			// se non esistono trasmissioni singole associate alla trasmissione
			// viene eliminata anche la trasmissione stessa
			if (objTrasm.trasmissioniSingole.Count == 0)
				DeleteTrasmissione(objTrasm);
            logger.Info("END");
			return objTrasm;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTrasmSingola"></param>
		/// <param name="idTrasmissione"></param>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.TrasmissioneSingola saveTrasmSingola(DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola, string idTrasmissione) 
		{
            logger.Info("BEGIN");
			logger.Debug("saveTrasmSingola");
			if (objTrasmSingola.daEliminare) 
				return deleteTrasmSingola(/*db,*/ objTrasmSingola);

			if (objTrasmSingola.systemId != null && !objTrasmSingola.systemId.Equals(""))
				objTrasmSingola = updateTrasmSingola(/*db,*/ objTrasmSingola);
			else
				objTrasmSingola = insertTrasmSingola(/*db,*/ objTrasmSingola, idTrasmissione);

			// aggiorno la DPA_TRASM_UTENTE
			for (int j = 0; j < objTrasmSingola.trasmissioneUtente.Count; j++) {
				DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente = (DocsPaVO.trasmissione.TrasmissioneUtente) objTrasmSingola.trasmissioneUtente[j];
                bool isUserDisabled = string.IsNullOrEmpty(objTrasmUtente.utente.disabilitato) ? BusinessLogic.Utenti.UserManager.isUserDisabled(objTrasmUtente.utente.codiceRubrica, objTrasmUtente.utente.idAmministrazione)
                    : objTrasmUtente.utente.disabilitato.Equals("Y");
                if (!isUserDisabled)
                {
                    objTrasmUtente = saveTrasmUtente(/*db,*/ objTrasmUtente, objTrasmSingola.systemId);
                    objTrasmSingola.trasmissioneUtente[j] = objTrasmUtente;
                }
			}
			// aggiorno la DPA_TRASM_UTENTE relativa alla risposta
			if (objTrasmSingola.idTrasmUtente != null && !objTrasmSingola.idTrasmUtente.Equals(""))
				updateTrasmRispSing(/*db,*/ objTrasmSingola.idTrasmUtente, "'" + objTrasmSingola.systemId + "'");
            logger.Info("END");
			return objTrasmSingola;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTrasmUtente"></param>
		/// <param name="idTrasmissioneSingola"></param>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.TrasmissioneUtente saveTrasmUtente(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente, string idTrasmissioneSingola) 
		{
            logger.Info("BEGIN");
			logger.Debug("saveTrasmUtente");
			if (objTrasmUtente.systemId != null && !objTrasmUtente.systemId.Equals(""))
				objTrasmUtente = updateTrasmUtente(/*db,*/ objTrasmUtente);
			else
				objTrasmUtente = insertTrasmUtente(/*db,*/ objTrasmUtente, idTrasmissioneSingola);
            logger.Info("END");
			return objTrasmUtente;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <param name="str"></param>
		/// <returns></returns>
		private static string notNull(string val, bool str) 
		{
			if (val != null) {
				if (str || !val.Equals(""))
					return val.Replace("'","''");
				else
					return "null";
			}
			else if(str)
				return "";
			else 
				return "null";
		}
			
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTrasm"></param>
		/// <returns></returns>
		private static string getIdProfile(DocsPaVO.trasmissione.Trasmissione objTrasm) 
		{
			if (objTrasm.infoDocumento != null)
				return objTrasm.infoDocumento.idProfile;
			else return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTrasm"></param>
		/// <returns></returns>
		private static string getIdProject(DocsPaVO.trasmissione.Trasmissione objTrasm) 
		{
			if (objTrasm.infoFascicolo != null)
				return objTrasm.infoFascicolo.idFascicolo;
			else return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTrasm"></param>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.Trasmissione insertTrasmissione(DocsPaVO.trasmissione.Trasmissione objTrasm) 
		{
			logger.Debug("insertTrasmissione");

			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			String idTrasmissione = trasmissione.insertTrasmissione(objTrasm);
			trasmissione.Dispose();

			logger.Debug("idTrasmissione = " + idTrasmissione);
			objTrasm.systemId = idTrasmissione;
			return objTrasm;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTrasmSingola"></param>
		/// <param name="idTrasmissione"></param>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.TrasmissioneSingola insertTrasmSingola(DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola, string idTrasmissione) 
		{
			#region Codice Commentato
			/*logger.Debug("insertTrasmSingola");
			string sqlString = 
				"INSERT INTO DPA_TRASM_SINGOLA (" + DocsPaWS.Utils.dbControl.getSystemIdColName() + 
				"ID_RAGIONE,ID_TRASMISSIONE,CHA_TIPO_DEST,ID_CORR_GLOBALE,VAR_NOTE_SING,CHA_TIPO_TRASM,DTA_SCADENZA,ID_TRASM_UTENTE) " +
				"VALUES ("+DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_TRASM_SINGOLA") + notNull(objTrasmSingola.ragione.systemId,false) + "," + 
				notNull(idTrasmissione,false) + ",'" + notNull((string)DocsPaVO.trasmissione.TrasmissioneSingola.tipoDestStringa[objTrasmSingola.tipoDest],true) + "'," + 
				notNull(objTrasmSingola.corrispondenteInterno.systemId,false) + ",'" +
				notNull(objTrasmSingola.noteSingole,true) + "','" + objTrasmSingola.tipoTrasm + "'," +
				DocsPaWS.Utils.dbControl.toDate(objTrasmSingola.dataScadenza,false) + "," + 
				notNull(objTrasmSingola.idTrasmUtente,false) + ")";
			logger.Debug(sqlString);						
			String idTrasmSingola = db.insertLocked(sqlString, "DPA_TRASM_SINGOLA");*/
			#endregion 
            ArrayList idDaEliminare = new ArrayList();
			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			String idTrasmSingola = trasmissione.insertTrasmSingola(/*db,*/objTrasmSingola,idTrasmissione);
			trasmissione.Dispose();
			logger.Debug("idTrasmSingola = " + idTrasmSingola);
			objTrasmSingola.systemId = idTrasmSingola;
            for (int i = 0; i < objTrasmSingola.trasmissioneUtente.Count; i++)
            {
                if (((DocsPaVO.trasmissione.TrasmissioneUtente)objTrasmSingola.trasmissioneUtente[i]).daNotificare)
                {
                    DocsPaVO.utente.Utente user = ((DocsPaVO.trasmissione.TrasmissioneUtente)objTrasmSingola.trasmissioneUtente[i]).utente;
                    bool isUserDisabled = string.IsNullOrEmpty(user.disabilitato) ? BusinessLogic.Utenti.UserManager.isUserDisabled(user.codiceRubrica, user.idAmministrazione)
                        : user.disabilitato.Equals("Y");
                    if (!isUserDisabled)
                    {
                        objTrasmSingola.trasmissioneUtente[i] = insertTrasmUtente(/*db,*/ (DocsPaVO.trasmissione.TrasmissioneUtente)objTrasmSingola.trasmissioneUtente[i], objTrasmSingola.systemId);
                    }
                }
                else
                {
                    idDaEliminare.Add(i);
                }
            }

            // elimino le trasmissioni utente che devono essere cancellate
            for (int i = idDaEliminare.Count; i > 0; i--)
                objTrasmSingola.trasmissioneUtente.RemoveAt((int)idDaEliminare[i - 1]);

            return objTrasmSingola;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTrasmUtente"></param>
		/// <param name="idTrasmSingola"></param>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.TrasmissioneUtente insertTrasmUtente(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente, string idTrasmSingola) 
		{
			#region Codice Commentato
			/*logger.Debug("insertTrasmUtente");
			string sqlString = 
				"INSERT INTO DPA_TRASM_UTENTE (" + DocsPaWS.Utils.dbControl.getSystemIdColName() + 
				"ID_PEOPLE,ID_TRASM_RISP_SING,ID_TRASM_SINGOLA,CHA_VISTA,CHA_ACCETTATA,CHA_RIFIUTATA,CHA_VALIDA) " +
				"VALUES ("+DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_TRASM_UTENTE") + 
				notNull(objTrasmUtente.utente.idPeople,false) + "," + notNull(objTrasmUtente.idTrasmRispSing,false)+ "," +
				idTrasmSingola + ",'0','0','0','1')";
			logger.Debug(sqlString);						
			String idTrasmUtente = db.insertLocked(sqlString, "DPA_TRASM_UTENTE");
			logger.Debug("idTrasmUtente = " + idTrasmUtente);
			objTrasmUtente.systemId = idTrasmUtente;
			return objTrasmUtente;*/
			#endregion 

			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			return trasmissione.insertTrasmUtente(/*db,*/objTrasmUtente,idTrasmSingola);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTrasm"></param>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.Trasmissione updateTrasmissione(DocsPaVO.trasmissione.Trasmissione objTrasm) 
		{
			#region Codice Commentato
			/*logger.Debug("updateTrasmissione");
			if (!objTrasm.daAggiornare) 
				return	objTrasm;
			string sqlString =
				"UPDATE DPA_TRASMISSIONE SET CHA_TIPO_OGGETTO = '" + notNull((string)DocsPaVO.trasmissione.Trasmissione.oggettoStringa[objTrasm.tipoOggetto],true) + 
				"', VAR_NOTE_GENERALI = '" + notNull(objTrasm.noteGenerali,true) + "' WHERE SYSTEM_ID=" + objTrasm.systemId;
			logger.Debug(sqlString);						
			db.executeNonQuery(sqlString);
			return objTrasm;*/
			#endregion 

			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			return trasmissione.updateTrasmissione(objTrasm);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTrasmSingola"></param>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.TrasmissioneSingola updateTrasmSingola(DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola) 
		{
			#region Codice Commentato
			/*logger.Debug("updateTrasmSingola");
			if (!objTrasmSingola.daAggiornare) 
				return objTrasmSingola;
			string sqlString = 
				"UPDATE DPA_TRASM_SINGOLA SET ID_RAGIONE = " + notNull(objTrasmSingola.ragione.systemId,false) + 
				", CHA_TIPO_DEST = '" + notNull((string)DocsPaVO.trasmissione.TrasmissioneSingola.tipoDestStringa[objTrasmSingola.tipoDest],true) + "'" +
				", ID_CORR_GLOBALE = " + notNull(objTrasmSingola.corrispondenteInterno.systemId,false) +
				", VAR_NOTE_SING = '" + notNull(objTrasmSingola.noteSingole,true) + "'" +
				", CHA_TIPO_TRASM = '" + notNull(objTrasmSingola.tipoTrasm,true) + "'" +
				", DTA_SCADENZA = " + DocsPaWS.Utils.dbControl.toDate(objTrasmSingola.dataScadenza,false) +
				" WHERE SYSTEM_ID=" + objTrasmSingola.systemId;
			logger.Debug(sqlString);						
			db.executeNonQuery(sqlString);
			objTrasmSingola.daAggiornare = false;
			return objTrasmSingola;*/
			#endregion 

			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			return trasmissione.updateTrasmSingola(objTrasmSingola);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTrasmUtente"></param>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.TrasmissioneUtente updateTrasmUtente(DocsPaVO.trasmissione.TrasmissioneUtente objTrasmUtente) 
		{
			#region Codice Commentato
			/*logger.Debug("updateTrasmUtente");
			if (!objTrasmUtente.daAggiornare) 
				return objTrasmUtente;
			string sqlString = 
				"UPDATE DPA_TRASM_UTENTE SET ID_TRASM_SINGOLA = " + notNull(objTrasmUtente.idTrasmRispSing,false) + 				
				", DTA_VISTA = " + DocsPaWS.Utils.dbControl.toDate(objTrasmUtente.dataVista,false) +
				", DTA_ACCETTATA = " + DocsPaWS.Utils.dbControl.toDate(objTrasmUtente.dataAccettata,false) +
				", DTA_RIFIUTATA = " + DocsPaWS.Utils.dbControl.toDate(objTrasmUtente.dataRifiutata,false) +
				", DTA_RISPOSTA = " + DocsPaWS.Utils.dbControl.toDate(objTrasmUtente.dataRisposta,false) +
				", VAR_NOTE_ACC = '" + notNull(objTrasmUtente.noteAccettazione,true) + "'" +
				", VAR_NOTE_RIF = '" + notNull(objTrasmUtente.noteRifiuto,true) + "'";
			if (!(objTrasmUtente.dataAccettata != null && !objTrasmUtente.dataAccettata.Equals("")))
				sqlString += ", CHA_ACCETTATA='1'";
			if (!(objTrasmUtente.dataRifiutata != null && !objTrasmUtente.dataRifiutata.Equals("")))
				sqlString += ", CHA_RIFIUTATA='1'";
			if (!(objTrasmUtente.dataVista != null && !objTrasmUtente.dataVista.Equals("")))
				sqlString += ", CHA_VISTA='1'";

			sqlString += " WHERE SYSTEM_ID=" + objTrasmUtente.systemId;
			
			logger.Debug(sqlString);						
			db.executeNonQuery(sqlString);
			objTrasmUtente.daAggiornare = false;
			return objTrasmUtente;*/
			#endregion 

			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			return trasmissione.UpdateTrasmUtente(objTrasmUtente);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="systemId"></param>
		/// <param name="idTrasmRispSingola"></param>
		private static void updateTrasmRispSing(string systemId, string idTrasmRispSingola) 
		{
			#region Codice Commentato
			/*logger.Debug("updateTrasmRispSing");
			string sqlString = 
				"UPDATE DPA_TRASM_UTENTE SET ID_TRASM_RISP_SING = " + idTrasmRispSingola + 
				"WHERE SYSTEM_ID = " + systemId;
			logger.Debug(sqlString);						
			db.executeNonQuery(sqlString);*/
			#endregion 

			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			trasmissione.UpdateTrasmRispSing(systemId,idTrasmRispSingola);
			trasmissione.Dispose();
		}
		
		/// <summary>
        /// Elimina la trasmissione in toto (trasm.ni singole e trasm.ni utente)
		/// </summary>
		/// <param name="objTrasmissione">oggetto trasmissione</param>
		/// <returns>true o false</returns>
		public static bool DeleteTrasmissione(DocsPaVO.trasmissione.Trasmissione objTrasmissione) 
		{
            logger.Debug("deleteTrasmissione");

            bool retValue = true;

            try
            {
                if (objTrasmissione.systemId == null)
                    return false;

                DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                // elimina prima le trasmissioni singole
                foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in objTrasmissione.trasmissioniSingole)
                    trasmissione.DeleteTrasmSingola(ts);

                // quindi elimina la trasmissione stessa
                trasmissione.DeleteTrasmissione(objTrasmissione);
            }
            catch
            {
                retValue = false;
            }

            return retValue; 
		 }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTrasmSingola"></param>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.TrasmissioneSingola deleteTrasmSingola(DocsPaVO.trasmissione.TrasmissioneSingola objTrasmSingola) 
		{
			logger.Debug("deleteTrasmSingola");
			
			if (objTrasmSingola.systemId == null)
			{
				return null;
			}
			
			#region Codice Commentato
			/*string sqlString = "";
			sqlString =
				"DELETE DPA_TRASM_UTENTE WHERE ID_TRASM_SINGOLA=" + objTrasmSingola.systemId;
			logger.Debug(sqlString);						
			db.executeNonQuery(sqlString);
			sqlString =
				"DELETE DPA_TRASM_SINGOLA WHERE SYSTEM_ID=" + objTrasmSingola.systemId;
			logger.Debug(sqlString);						
			db.executeNonQuery(sqlString);*/
			#endregion 

			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione =	new DocsPaDB.Query_DocsPAWS.Trasmissione();

			// se ho cancellato risposte singole devo aggiorare le relative ID_TRASM_RISP_SING
			if (objTrasmSingola.idTrasmUtente != null && ! objTrasmSingola.Equals("")) 
			{
				updateTrasmRispSing(/*db,*/ objTrasmSingola.idTrasmUtente,"null");
			}

			return null;
		}

		#region Metodo Commentato
//		private static Exception throwException (DocsPa_V15_Utils.DBAgent db, string msg) 
//		{
//			logger.Debug(msg);
//			db.rollbackTransaction();
//			db.closeConnection();
//			return new Exception(msg);
//		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="db"></param>
		/// <param name="objTrasm"></param>
		/// <returns></returns>
		private static DocsPaVO.trasmissione.Trasmissione checkInputData(DocsPaVO.trasmissione.Trasmissione objTrasm) 
		{
			logger.Debug("checkInputData");
			int i;
			// verifico i dati per le trasmissioni singole
			ArrayList tsList = objTrasm.trasmissioniSingole;
			for (i=0; i < tsList.Count; i++) {
				DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola) tsList[i];
				// verifico il campo CHA_TIPO_TRASM
                if (ts!=null && ts.tipoTrasm != null)
                {
					if (!ts.tipoTrasm.Equals("S") && !ts.tipoTrasm.Equals("T")) 
					{
						#region Codice Commentato
						//throw throwException(db, "Tipo Trasmissione non consentito - Valore attuale: " + ts.tipoTrasm);
						//db.rollbackTransaction();
						//db.closeConnection();
						#endregion 

						logger.Debug("Errore nella gestione delle trasmissioni (checkInputData). Tipo Trasmissione non consentito - Valore attuale: " + ts.tipoTrasm);
						throw new Exception("Tipo Trasmissione non consentito - Valore attuale: " + ts.tipoTrasm);
					}
				}
				#region Codice Commentato
				// verifico il campo CHA_TIPO_DEST
                //   non dovrebbe essere necessario perchè tipoDest è un enum
					/*if (!ts.tipoDest.Equals("U") && !ts.tipoDest.Equals("R") && !ts.tipoDest.Equals("G")) 
						throw throwException("Tipo Destinatario non consentito - Valore attuale: " + ts.tipoDest);*/
				#endregion 
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objTrasm"></param>
		private static void setAssegnazione(DocsPaVO.trasmissione.Trasmissione objTrasm) 
		{
			#region Codice Commentato
			/*if(objTrasm == null || objTrasm.infoDocumento == null)
				return;
			string idProfile = objTrasm.infoDocumento.idProfile;
			if (!(idProfile != null && !idProfile.Equals("")))
				return;
			for (int i=0; i< objTrasm.trasmissioniSingole.Count; i++) {
				DocsPaVO.trasmissione.RagioneTrasmissione ragione = ((DocsPaVO.trasmissione.TrasmissioneSingola)objTrasm.trasmissioniSingole[i]).ragione;
				if (ragione.tipo.Equals("W") && ragione.tipoDestinatario == DocsPaVO.trasmissione.TipoGerarchia.INFERIORE) {
					string updateString =
						"UPDATE PROFILE SET CHA_ASSEGNATO='1' WHERE NOT CHA_ASSEGNATO='1' AND SYSTEM_ID=" + idProfile;
					logger.Debug(updateString);
					db.executeNonQuery(updateString);
					return;
				}
			}*/
			#endregion 
			
			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione =	new DocsPaDB.Query_DocsPAWS.Trasmissione();
			trasmissione.SetAssegnazione(objTrasm);
        }

        #region Scadenze Trasmissione
        public void checkScadenze(string path)
        {
            try
            {
                string preScadenza = string.Empty;
                string scadenza = string .Empty;

                if (System.Configuration.ConfigurationManager.AppSettings["dbType"].ToUpper() == "ORACLE")
                {
                    preScadenza = " TO_DATE(TO_CHAR(SYSDATE+NUMTODSINTERVAL(dpa_tipo_atto.gg_pre_scadenza,'DAY'),'dd/mm/yyyy'),'dd/mm/yyyy') ";
                    scadenza = " TO_DATE(TO_CHAR(SYSDATE,'dd/mm/yyyy'),'dd/mm/yyyy') ";
                }
                else
                {
                    preScadenza = " DATEADD(DAY, +dpa_tipo_atto.gg_pre_scadenza, GETDATE()) ";
                    scadenza = " GETDATE() ";                
                }
                
                //DOCUMENTI IN SCADENZA
                DocsPaDB.Query_DocsPAWS.Trasmissione tr_1 = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                System.Data.DataSet dsScadenzaDoc_1 = tr_1.checkScadenzeDoc(preScadenza);

                if (dsScadenzaDoc_1.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < dsScadenzaDoc_1.Tables[0].Rows.Count; i++)
                    {
                        System.Diagnostics.Debug.WriteLine("CheckScadenze - Inizio processo Documento DocNumber : " + dsScadenzaDoc_1.Tables[0].Rows[i]["DOCNUMBER"].ToString());
                        logger.Debug("CheckScadenze - Inizio processo Documento DocNumber : " + dsScadenzaDoc_1.Tables[0].Rows[i]["DOCNUMBER"].ToString());
					
                        DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();
                        trasmissione.noteGenerali = "Il documento è in scadenza.";
                        trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                                                

                        System.Data.DataSet dsScadenzaMitt = tr_1.checkScadenzeMitt(dsScadenzaDoc_1.Tables[0].Rows[i]["DOCNUMBER"].ToString(), preScadenza);
                        string query_1 = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = " + dsScadenzaMitt.Tables[0].Rows[0]["PERSONORGROUP"].ToString() + ")";
                        //trasmissione.ruolo = Utenti.UserManager.getRuolo(dsScadenzaMitt.Tables[0].Rows[0]["PERSONORGROUP"].ToString());
                        trasmissione.ruolo = Utenti.UserManager.getRuolo(query_1);
                        System.Diagnostics.Debug.WriteLine("CheckScadenze - Ruolo Mittente della Trasmissione : " + trasmissione.ruolo.descrizione);
                        logger.Debug("CheckScadenze - Ruolo Mittente della Trasmissione : " + trasmissione.ruolo.descrizione);

                        DocsPaVO.utente.Utente utente = Utenti.UserManager.getUtente(dsScadenzaDoc_1.Tables[0].Rows[i]["PERSONORGROUP"].ToString());
                        DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, trasmissione.ruolo);

                        DocsPaVO.documento.SchedaDocumento sd = Documenti.DocManager.getDettaglioNoSecurity(infoUtente, dsScadenzaDoc_1.Tables[0].Rows[i]["DOCNUMBER"].ToString());
                        trasmissione.infoDocumento = Documenti.DocManager.getInfoDocumento(sd);
                        trasmissione.utente = utente;
					
                        //Ricordarsi : La ragione adesso non è un tipo di ragione
                        //Viene prelevata con la query "S_DPARagNotifica" dove è cablato il nome della ragione "NOTIFICA"
                        //DocsPaVO.trasmissione.RagioneTrasmissione ragione = RagioniManager.getRagione("36846");
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = RagioniManager.getRagioneNotifica(trasmissione.ruolo.idAmministrazione);

                        System.Data.DataSet dsScadenzaDest = tr_1.checkScadenzeDest(dsScadenzaDoc_1.Tables[0].Rows[i]["DOCNUMBER"].ToString(), preScadenza);
                        for (int j = 0; j < dsScadenzaDest.Tables[0].Rows.Count; j++)
                        {
                            string query_2 = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = " + dsScadenzaDest.Tables[0].Rows[j]["PERSONORGROUP"].ToString() + ")";
                            string query_3 = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = " + dsScadenzaDest.Tables[0].Rows[j]["PERSONORGROUP"].ToString() + ")";
                            //DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getRuolo(dsScadenzaDest.Tables[0].Rows[j]["PERSONORGROUP"].ToString());
                            
                            DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                            if(dsScadenzaDest.Tables[0].Rows[j]["TIPO"].ToString() == "R")
                                corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getRuolo(query_2);
                            if (dsScadenzaDest.Tables[0].Rows[j]["TIPO"].ToString() == "P")
                                corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getCorrispondenteBySystemID(query_3);

                            System.Diagnostics.Debug.WriteLine("CheckScadenze - Destinatario "+(j+1)+" della trasmissione: " + corr.descrizione);
                            logger.Debug("CheckScadenze - Destinatario "+(j + 1) + " della trasmissione: " + corr.descrizione);

                            trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, "Il documento è in scadenza.", "T");	
                        }
                        if (infoUtente.delegato != null)
                            trasmissione.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;
				
                        //trasmissione = saveTrasmMethod(trasmissione);
                        //ExecTrasmManager.executeTrasmMethod(path, trasmissione);
                        ////ExecTrasmManager.executeTrasmMethod(path, trasmissione);

                        trasmissione = ExecTrasmManager.saveExecuteTrasmMethod(path, trasmissione);

                        System.Diagnostics.Debug.WriteLine("CheckScadenze - Fine processo Documento DocNumber : " + dsScadenzaDoc_1.Tables[0].Rows[i]["DOCNUMBER"].ToString());
                        logger.Debug("CheckScadenze - Fine processo Documento DocNumber : " + dsScadenzaDoc_1.Tables[0].Rows[i]["DOCNUMBER"].ToString());
					}
                }

                //DOCUMENTI SCADUTI
                DocsPaDB.Query_DocsPAWS.Trasmissione tr_2 = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                System.Data.DataSet dsScadenzaDoc_2 = tr_2.checkScadenzeDoc(scadenza);

                if (dsScadenzaDoc_2.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < dsScadenzaDoc_2.Tables[0].Rows.Count; i++)
                    {
                        System.Diagnostics.Debug.WriteLine("CheckScadenze - Inizio processo Documento DocNumber : " + dsScadenzaDoc_2.Tables[0].Rows[i]["DOCNUMBER"].ToString());
                        logger.Debug("CheckScadenze - Inizio processo Documento DocNumber : " + dsScadenzaDoc_2.Tables[0].Rows[i]["DOCNUMBER"].ToString());

                        DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();
                        trasmissione.noteGenerali = "Il documento è scaduto.";
                        trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                        System.Data.DataSet dsScadenzaMitt = tr_2.checkScadenzeMitt(dsScadenzaDoc_2.Tables[0].Rows[i]["DOCNUMBER"].ToString(), scadenza);
                        string query_1 = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = " + dsScadenzaMitt.Tables[0].Rows[0]["PERSONORGROUP"].ToString() + ")";
                        //trasmissione.ruolo = Utenti.UserManager.getRuolo(dsScadenzaMitt.Tables[0].Rows[0]["PERSONORGROUP"].ToString());
                        trasmissione.ruolo = Utenti.UserManager.getRuolo(query_1);
                        System.Diagnostics.Debug.WriteLine("CheckScadenze - Ruolo Mittente della Trasmissione : " + trasmissione.ruolo.descrizione);
                        logger.Debug("CheckScadenze - Ruolo Mittente della Trasmissione : " + trasmissione.ruolo.descrizione);

                        DocsPaVO.utente.Utente utente = Utenti.UserManager.getUtente(dsScadenzaDoc_2.Tables[0].Rows[i]["PERSONORGROUP"].ToString());
                        DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, trasmissione.ruolo);

                        DocsPaVO.documento.SchedaDocumento sd = new DocsPaVO.documento.SchedaDocumento();
                        sd = Documenti.DocManager.getDettaglioNoSecurity(infoUtente, dsScadenzaDoc_2.Tables[0].Rows[i]["DOCNUMBER"].ToString());
                        trasmissione.infoDocumento = Documenti.DocManager.getInfoDocumento(sd);
                        trasmissione.utente = Utenti.UserManager.getUtente(dsScadenzaDoc_2.Tables[0].Rows[i]["PERSONORGROUP"].ToString());


                        //Ricordarsi : La ragione adesso non è un tipo di ragione
                        //Viene prelevata con la query "S_DPARagNotifica" dove è cablato il nome della ragione "NOTIFICA"
                        //DocsPaVO.trasmissione.RagioneTrasmissione ragione = RagioniManager.getRagione("36846");
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = RagioniManager.getRagioneNotifica(trasmissione.ruolo.idAmministrazione);

                        System.Data.DataSet dsScadenzaDest = tr_2.checkScadenzeDest(dsScadenzaDoc_2.Tables[0].Rows[i]["DOCNUMBER"].ToString(), scadenza);
                        for (int j = 0; j < dsScadenzaDest.Tables[0].Rows.Count; j++)
                        {
                            string query_2 = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = " + dsScadenzaDest.Tables[0].Rows[j]["PERSONORGROUP"].ToString() + ")";
                            string query_3 = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = " + dsScadenzaDest.Tables[0].Rows[j]["PERSONORGROUP"].ToString() + ")";
                            //DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getRuolo(dsScadenzaDest.Tables[0].Rows[j]["PERSONORGROUP"].ToString());

                            DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                            if (dsScadenzaDest.Tables[0].Rows[j]["TIPO"].ToString() == "R")
                                corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getRuolo(query_2);
                            if (dsScadenzaDest.Tables[0].Rows[j]["TIPO"].ToString() == "P")
                                corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getCorrispondenteBySystemID(query_3);
                            
                            System.Diagnostics.Debug.WriteLine("CheckScadenze - Destinatario " + (j + 1) + " della trasmissione: " + corr.descrizione);
                            logger.Debug("CheckScadenze - Destinatario " + (j + 1) + " della trasmissione: " + corr.descrizione);

                            trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, "Il documento è scaduto.", "T");
                        }
                        if (infoUtente.delegato != null)
                            trasmissione.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;
				
                        //trasmissione = saveTrasmMethod(trasmissione);
                        //ExecTrasmManager.executeTrasmMethod(path, trasmissione);
                        ////ExecTrasmManager.executeTrasmMethod(path, trasmissione);

                        trasmissione = ExecTrasmManager.saveExecuteTrasmMethod(path, trasmissione);

                        System.Diagnostics.Debug.WriteLine("CheckScadenze - Fine processo Documento DocNumber : " + dsScadenzaDoc_2.Tables[0].Rows[i]["DOCNUMBER"].ToString());
                        logger.Debug("CheckScadenze - Fine processo Documento DocNumber : " + dsScadenzaDoc_2.Tables[0].Rows[i]["DOCNUMBER"].ToString());
                    }
                }

                if (System.Configuration.ConfigurationManager.AppSettings["dbType"].ToUpper() == "ORACLE")
                {
                    preScadenza = " TO_DATE(TO_CHAR(SYSDATE+NUMTODSINTERVAL(dpa_tipo_fasc.gg_pre_scadenza,'DAY'),'dd/mm/yyyy'),'dd/mm/yyyy') ";
                    scadenza = " TO_DATE(TO_CHAR(SYSDATE,'dd/mm/yyyy'),'dd/mm/yyyy') ";
                }
                else
                {
                    preScadenza = " DATEADD(DAY, +dpa_tipo_fasc.gg_pre_scadenza, GETDATE()) ";
                    scadenza = " GETDATE() ";
                }

                //FASCICOLI IN SCADENZA
                DocsPaDB.Query_DocsPAWS.Trasmissione tr_3 = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                System.Data.DataSet dsScadenzaFasc_3 = tr_3.checkScadenzeFasc(preScadenza);

                if (dsScadenzaFasc_3.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < dsScadenzaFasc_3.Tables[0].Rows.Count; i++)
                    {
                        System.Diagnostics.Debug.WriteLine("CheckScadenze - Inizio processo Fascicolo idProject : " + dsScadenzaFasc_3.Tables[0].Rows[i]["IDPROJECT"].ToString());
                        logger.Debug("CheckScadenze - Inizio processo Fascicolo idProject : " + dsScadenzaFasc_3.Tables[0].Rows[i]["IDPROJECT"].ToString());

                        DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();
                        trasmissione.noteGenerali = "Il fascicolo è in scadenza.";
                        trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;

                        System.Data.DataSet dsScadenzaMitt = tr_3.checkScadenzeMittFasc(dsScadenzaFasc_3.Tables[0].Rows[i]["IDPROJECT"].ToString(), preScadenza);
                        string query_1 = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = " + dsScadenzaMitt.Tables[0].Rows[0]["PERSONORGROUP"].ToString() + ")";
                        //trasmissione.ruolo = Utenti.UserManager.getRuolo(dsScadenzaMitt.Tables[0].Rows[0]["PERSONORGROUP"].ToString());
                        trasmissione.ruolo = Utenti.UserManager.getRuolo(query_1);
                        System.Diagnostics.Debug.WriteLine("CheckScadenze - Ruolo Mittente della Trasmissione : " + trasmissione.ruolo.descrizione);
                        logger.Debug("CheckScadenze - Ruolo Mittente della Trasmissione : " + trasmissione.ruolo.descrizione);

                        DocsPaVO.utente.Utente utente = Utenti.UserManager.getUtente(dsScadenzaFasc_3.Tables[0].Rows[i]["PERSONORGROUP"].ToString());
                        DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, trasmissione.ruolo);

                        DocsPaVO.fascicolazione.Fascicolo fasc = Fascicoli.FascicoloManager.getFascicoloById(dsScadenzaFasc_3.Tables[0].Rows[i]["IDPROJECT"].ToString(), infoUtente);
                        DocsPaVO.fascicolazione.InfoFascicolo infoFasc = new DocsPaVO.fascicolazione.InfoFascicolo(fasc);
                        trasmissione.infoFascicolo = infoFasc;
                        trasmissione.utente = utente;

                        //Ricordarsi : La ragione adesso non è un tipo di ragione
                        //Viene prelevata con la query "S_DPARagNotifica" dove è cablato il nome della ragione "NOTIFICA"
                        //DocsPaVO.trasmissione.RagioneTrasmissione ragione = RagioniManager.getRagione("36846");
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = RagioniManager.getRagioneNotifica(trasmissione.ruolo.idAmministrazione);

                        System.Data.DataSet dsScadenzaDest = tr_3.checkScadenzeDestFasc(dsScadenzaFasc_3.Tables[0].Rows[i]["IDPROJECT"].ToString(), preScadenza);
                        for (int j = 0; j < dsScadenzaDest.Tables[0].Rows.Count; j++)
                        {
                            string query_2 = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = " + dsScadenzaDest.Tables[0].Rows[j]["PERSONORGROUP"].ToString() + ")";
                            string query_3 = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = " + dsScadenzaDest.Tables[0].Rows[j]["PERSONORGROUP"].ToString() + ")";
                            //DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getRuolo(dsScadenzaDest.Tables[0].Rows[j]["PERSONORGROUP"].ToString());

                            DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                            if (dsScadenzaDest.Tables[0].Rows[j]["TIPO"].ToString() == "R")
                                corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getRuolo(query_2);
                            if (dsScadenzaDest.Tables[0].Rows[j]["TIPO"].ToString() == "P")
                                corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getCorrispondenteBySystemID(query_3);

                            System.Diagnostics.Debug.WriteLine("CheckScadenze - Destinatario " + (j + 1) + " della trasmissione: " + corr.descrizione);
                            logger.Debug("CheckScadenze - Destinatario " + (j + 1) + " della trasmissione: " + corr.descrizione);

                            trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, "Il fascicolo è in scadenza.", "T");
                        }
                        if (infoUtente.delegato != null)
                            trasmissione.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;
				
                        //trasmissione = saveTrasmMethod(trasmissione);
                        //ExecTrasmManager.executeTrasmMethod(path, trasmissione);
                        //ExecTrasmManager.executeTrasmMethod(path, trasmissione);

                        trasmissione = ExecTrasmManager.saveExecuteTrasmMethod(path, trasmissione);

                        System.Diagnostics.Debug.WriteLine("CheckScadenze - Fine processo Fascicolo idProject : " + dsScadenzaFasc_3.Tables[0].Rows[i]["IDPROJECT"].ToString());
                        logger.Debug("CheckScadenze - Fine processo Fascicolo idProject : " + dsScadenzaFasc_3.Tables[0].Rows[i]["IDPROJECT"].ToString());
                    }
                }

                //FASCICOLI SCADUTI
                DocsPaDB.Query_DocsPAWS.Trasmissione tr_4 = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                System.Data.DataSet dsScadenzaFasc_4 = tr_4.checkScadenzeFasc(scadenza);

                if (dsScadenzaFasc_4.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < dsScadenzaFasc_4.Tables[0].Rows.Count; i++)
                    {
                        System.Diagnostics.Debug.WriteLine("CheckScadenze - Inizio processo Fascicolo idProject : " + dsScadenzaFasc_4.Tables[0].Rows[i]["IDPROJECT"].ToString());
                        logger.Debug("CheckScadenze - Inizio processo Fascicolo idProject : " + dsScadenzaFasc_4.Tables[0].Rows[i]["IDPROJECT"].ToString());

                        DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();
                        trasmissione.noteGenerali = "Il fascicolo è scaduto.";
                        trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;

                        System.Data.DataSet dsScadenzaMitt = tr_4.checkScadenzeMittFasc(dsScadenzaFasc_4.Tables[0].Rows[i]["IDPROJECT"].ToString(), scadenza);
                        string query_1 = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = " + dsScadenzaMitt.Tables[0].Rows[0]["PERSONORGROUP"].ToString() + ")";
                        //trasmissione.ruolo = Utenti.UserManager.getRuolo(dsScadenzaMitt.Tables[0].Rows[0]["PERSONORGROUP"].ToString());
                        trasmissione.ruolo = Utenti.UserManager.getRuolo(query_1);
                        System.Diagnostics.Debug.WriteLine("CheckScadenze - Ruolo Mittente della Trasmissione : " + trasmissione.ruolo.descrizione);
                        logger.Debug("CheckScadenze - Ruolo Mittente della Trasmissione : " + trasmissione.ruolo.descrizione);

                        DocsPaVO.utente.Utente utente = Utenti.UserManager.getUtente(dsScadenzaFasc_4.Tables[0].Rows[i]["PERSONORGROUP"].ToString());
                        DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, trasmissione.ruolo);

                        DocsPaVO.fascicolazione.Fascicolo fasc = Fascicoli.FascicoloManager.getFascicoloById(dsScadenzaFasc_4.Tables[0].Rows[i]["IDPROJECT"].ToString(), infoUtente);
                        DocsPaVO.fascicolazione.InfoFascicolo infoFasc = new DocsPaVO.fascicolazione.InfoFascicolo(fasc);
                        trasmissione.infoFascicolo = infoFasc;
                        trasmissione.utente = Utenti.UserManager.getUtente(dsScadenzaFasc_4.Tables[0].Rows[i]["PERSONORGROUP"].ToString());

                        //Ricordarsi : La ragione adesso non è un tipo di ragione
                        //Viene prelevata con la query "S_DPARagNotifica" dove è cablato il nome della ragione "NOTIFICA"
                        //DocsPaVO.trasmissione.RagioneTrasmissione ragione = RagioniManager.getRagione("36846");
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = RagioniManager.getRagioneNotifica(trasmissione.ruolo.idAmministrazione);

                        System.Data.DataSet dsScadenzaDest = tr_4.checkScadenzeDestFasc(dsScadenzaFasc_4.Tables[0].Rows[i]["IDPROJECT"].ToString(), scadenza);
                        for (int j = 0; j < dsScadenzaDest.Tables[0].Rows.Count; j++)
                        {
                            string query_2 = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = " + dsScadenzaDest.Tables[0].Rows[j]["PERSONORGROUP"].ToString() + ")";
                            string query_3 = "(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE = " + dsScadenzaDest.Tables[0].Rows[j]["PERSONORGROUP"].ToString() + ")";
                            //DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getRuolo(dsScadenzaDest.Tables[0].Rows[j]["PERSONORGROUP"].ToString());

                            DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                            if (dsScadenzaDest.Tables[0].Rows[j]["TIPO"].ToString() == "R")
                                corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getRuolo(query_2);
                            if (dsScadenzaDest.Tables[0].Rows[j]["TIPO"].ToString() == "P")
                                corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getCorrispondenteBySystemID(query_3);

                            System.Diagnostics.Debug.WriteLine("CheckScadenze - Destinatario " + (j + 1) + " della trasmissione: " + corr.descrizione);
                            logger.Debug("CheckScadenze - Destinatario " + (j + 1) + " della trasmissione: " + corr.descrizione);

                            trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, "Il fascicolo è scaduto.", "T");
                        }
                        if (infoUtente.delegato != null)
                            trasmissione.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;
				
                        //trasmissione = saveTrasmMethod(trasmissione);
                        //ExecTrasmManager.executeTrasmMethod(path, trasmissione);
                        //ExecTrasmManager.executeTrasmMethod(path, trasmissione);

                        trasmissione = ExecTrasmManager.saveExecuteTrasmMethod(path, trasmissione);

                        System.Diagnostics.Debug.WriteLine("CheckScadenze - Fine processo Fascicolo idProject : " + dsScadenzaFasc_4.Tables[0].Rows[i]["IDPROJECT"].ToString());
                        logger.Debug("CheckScadenze - Fine processo Fascicolo idProject : " + dsScadenzaFasc_4.Tables[0].Rows[i]["IDPROJECT"].ToString());
                    }
                }

                //TRASMISSIONI Documenti SCADUTE
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("SCADENZE_TRASMISSIONI_DOC");
                queryMng.setParam("param1", "");
                string commandText = queryMng.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - CheckScadenze - Trasmissioni Scadute - QUERY : " + commandText);
                logger.Debug("SQL - CheckScadenze - Trasmissioni Scadute - QUERY : " + commandText);

                System.Data.DataSet dsTrasmScadute = new System.Data.DataSet();
                dbProvider.ExecuteQuery(dsTrasmScadute, commandText);

                if (dsTrasmScadute.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < dsTrasmScadute.Tables[0].Rows.Count; i++)
                    {
                        
                        //Mittente e ragione della trasmissione
                        string dataInvio = dsTrasmScadute.Tables[0].Rows[i]["DTA_INVIO"].ToString();
                        string tipoOggetto = dsTrasmScadute.Tables[0].Rows[i]["CHA_TIPO_OGGETTO"].ToString();
                        string idProfile = dsTrasmScadute.Tables[0].Rows[i]["ID_PROFILE"].ToString();
                        string idProject = dsTrasmScadute.Tables[0].Rows[i]["ID_PROJECT"].ToString();
                        string docNumber = dsTrasmScadute.Tables[0].Rows[i]["DOCNUMBER"].ToString();
                        string idMittente = dsTrasmScadute.Tables[0].Rows[i]["ID_MITTENTE"].ToString();
                        string tipoTrasm = dsTrasmScadute.Tables[0].Rows[i]["CHA_TIPO_TRASM"].ToString();
                        string idRuoloInUo = dsTrasmScadute.Tables[0].Rows[i]["ID_RUOLO_IN_UO"].ToString();

                        DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();
                        trasmissione.noteGenerali = "La Trasmissione del : " + ((string[])dataInvio.Split(' '))[0] + " è scaduta.";
                        trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                        trasmissione.utente = Utenti.UserManager.getUtente(idMittente);
                        trasmissione.ruolo = Utenti.UserManager.getRuolo(idRuoloInUo);

                        DocsPaVO.documento.SchedaDocumento sd = new DocsPaVO.documento.SchedaDocumento();
                        sd = Documenti.DocManager.getDettaglioNoSecurity(new DocsPaVO.utente.InfoUtente(trasmissione.utente, trasmissione.ruolo), docNumber);
                        trasmissione.infoDocumento = Documenti.DocManager.getInfoDocumento(sd);

                        //Ricordarsi : La ragione adesso non è un tipo di ragione
                        //Viene prelevata con la query "S_DPARagNotifica" dove è cablato il nome della ragione "NOTIFICA"
                        //DocsPaVO.trasmissione.RagioneTrasmissione ragione = RagioniManager.getRagione("36846");
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = RagioniManager.getRagioneNotifica(trasmissione.ruolo.idAmministrazione);

                        //Seleziono i destinatari e li aggiungo alla trasmissione
                        DocsPaDB.DBProvider dbProvider_1 = new DocsPaDB.DBProvider();
                        DocsPaUtils.Query queryMng_1 = DocsPaUtils.InitQuery.getInstance().getQuery("SCADENZE_TRASMISSIONI_DOC");
                        queryMng_1.setParam("param1", ", dpa_trasm_utente.ID_PEOPLE as ID_DESTINATARIO, dpa_trasm_singola.CHA_TIPO_DEST, dpa_trasm_singola.ID_CORR_GLOBALE ");
                        string commandText_1 = queryMng_1.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - CheckScadenze - Trasmissioni Scadute - QUERY : " + commandText_1);
                        logger.Debug("SQL - CheckScadenze - Trasmissioni Scadute - QUERY : " + commandText_1);

                        System.Data.DataSet dsTrasmScadute_Dest = new System.Data.DataSet();
                        dbProvider.ExecuteQuery(dsTrasmScadute_Dest, commandText_1);

                        if (dsTrasmScadute_Dest.Tables[0].Rows.Count != 0)
                        {
                            for (int j = 0; j < dsTrasmScadute_Dest.Tables[0].Rows.Count; j++)
                            {
                                if (idProfile == dsTrasmScadute_Dest.Tables[0].Rows[j]["ID_PROFILE"].ToString())
                                {
                                    //Aggiungo il destinatario alla trasmissione
                                    string chaTipoDest = dsTrasmScadute_Dest.Tables[0].Rows[j]["CHA_TIPO_DEST"].ToString();
                                    DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                                    if (chaTipoDest == "R")
                                        corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getRuolo(dsTrasmScadute_Dest.Tables[0].Rows[j]["ID_CORR_GLOBALE"].ToString());
                                    if (chaTipoDest == "U")
                                    {
                                        corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getUtente(dsTrasmScadute_Dest.Tables[0].Rows[j]["ID_DESTINATARIO"].ToString());
                                        corr.systemId = dsTrasmScadute_Dest.Tables[0].Rows[j]["ID_CORR_GLOBALE"].ToString();
                                    }

                                    trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, "La Trasmissione del : " + ((string[])dataInvio.Split(' '))[0] + " è scaduta.", tipoTrasm);
                                }
                            }
                        }

                        //Eseguo la trasmissione
                        
                        //trasmissione = saveTrasmMethod(trasmissione);
                        //ExecTrasmManager.executeTrasmMethod(path, trasmissione);

                        trasmissione = ExecTrasmManager.saveExecuteTrasmMethod(path, trasmissione);

                    }
                }

                //TRASMISSIONI Fascicoli SCADUTE
                DocsPaDB.DBProvider dbProvider_2 = new DocsPaDB.DBProvider();
                DocsPaUtils.Query queryMng_2 = DocsPaUtils.InitQuery.getInstance().getQuery("SCADENZE_TRASMISSIONI_FASC");
                queryMng_2.setParam("param1", "");
                string commandText_2 = queryMng_2.getSQL();
                System.Diagnostics.Debug.WriteLine("SQL - CheckScadenze - Trasmissioni Scadute - QUERY : " + commandText_2);
                logger.Debug("SQL - CheckScadenze - Trasmissioni Scadute - QUERY : " + commandText_2);

                System.Data.DataSet dsTrasmScadute_2 = new System.Data.DataSet();
                dbProvider_2.ExecuteQuery(dsTrasmScadute_2, commandText_2);

                if (dsTrasmScadute_2.Tables[0].Rows.Count != 0)
                {
                    for (int i = 0; i < dsTrasmScadute_2.Tables[0].Rows.Count; i++)
                    {

                        //Mittente e ragione della trasmissione
                        string dataInvio = dsTrasmScadute_2.Tables[0].Rows[i]["DTA_INVIO"].ToString();
                        string tipoOggetto = dsTrasmScadute_2.Tables[0].Rows[i]["CHA_TIPO_OGGETTO"].ToString();
                        //string idProfile = dsTrasmScadute_2.Tables[0].Rows[i]["ID_PROFILE"].ToString();
                        string idProject = dsTrasmScadute_2.Tables[0].Rows[i]["ID_PROJECT"].ToString();
                        //string docNumber = dsTrasmScadute_2.Tables[0].Rows[i]["DOCNUMBER"].ToString();
                        string idMittente = dsTrasmScadute_2.Tables[0].Rows[i]["ID_MITTENTE"].ToString();
                        //string tipoTrasm = dsTrasmScadute.Tables[0].Rows[i]["CHA_TIPO_TRASM"].ToString();
                        string idRuoloInUo = dsTrasmScadute_2.Tables[0].Rows[i]["ID_RUOLO_IN_UO"].ToString();

                        DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();
                        trasmissione.noteGenerali = "La Trasmissione del : " + ((string[])dataInvio.Split(' '))[0] + " è scaduta.";
                        trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
                        DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                        
                        DocsPaVO.utente.InfoUtente infoUtenteTrasm = new DocsPaVO.utente.InfoUtente(Utenti.UserManager.getUtente(idMittente), Utenti.UserManager.getRuolo(idRuoloInUo));
                        fascicolo = Fascicoli.FascicoloManager.getFascicoloById(idProject, infoUtenteTrasm);

                        DocsPaVO.fascicolazione.InfoFascicolo infoFasc = new DocsPaVO.fascicolazione.InfoFascicolo();
                        infoFasc.idFascicolo = fascicolo.systemID;
                        infoFasc.descrizione = fascicolo.descrizione;
                        infoFasc.idClassificazione = fascicolo.idClassificazione;
                        infoFasc.codice = fascicolo.codice;
                        if (fascicolo.stato != null && fascicolo.stato.Equals("A"))
                            infoFasc.apertura = fascicolo.apertura;
                        trasmissione.infoFascicolo = infoFasc;
                        trasmissione.utente = Utenti.UserManager.getUtente(idMittente);
                        trasmissione.ruolo = Utenti.UserManager.getRuolo(idRuoloInUo);

                        //Ricordarsi : La ragione adesso non è un tipo di ragione
                        //Viene prelevata con la query "S_DPARagNotifica" dove è cablato il nome della ragione "NOTIFICA"
                        //DocsPaVO.trasmissione.RagioneTrasmissione ragione = RagioniManager.getRagione("36846");
                        DocsPaVO.trasmissione.RagioneTrasmissione ragione = RagioniManager.getRagioneNotifica(trasmissione.ruolo.idAmministrazione);

                        //Seleziono i destinatari e li aggiungo alla trasmissione
                        DocsPaDB.DBProvider dbProvider_3 = new DocsPaDB.DBProvider();
                        DocsPaUtils.Query queryMng_3 = DocsPaUtils.InitQuery.getInstance().getQuery("SCADENZE_TRASMISSIONI_FASC");
                        queryMng_3.setParam("param1", ", dpa_trasm_singola.CHA_TIPO_TRASM, dpa_trasm_utente.ID_PEOPLE as ID_DESTINATARIO, dpa_trasm_singola.CHA_TIPO_DEST, dpa_trasm_singola.ID_CORR_GLOBALE ");
                        string commandText_3 = queryMng_3.getSQL();
                        System.Diagnostics.Debug.WriteLine("SQL - CheckScadenze - Trasmissioni Scadute - QUERY : " + commandText_3);
                        logger.Debug("SQL - CheckScadenze - Trasmissioni Scadute - QUERY : " + commandText_3);

                        System.Data.DataSet dsTrasmScadute_Dest_3 = new System.Data.DataSet();
                        dbProvider_3.ExecuteQuery(dsTrasmScadute_Dest_3, commandText_3);

                        if (dsTrasmScadute_Dest_3.Tables[0].Rows.Count != 0)
                        {
                            for (int j = 0; j < dsTrasmScadute_Dest_3.Tables[0].Rows.Count; j++)
                            {
                                if (idProject == dsTrasmScadute_Dest_3.Tables[0].Rows[j]["ID_PROJECT"].ToString())
                                {
                                    //Aggiungo il destinatario alla trasmissione
                                    string chaTipoDest = dsTrasmScadute_Dest_3.Tables[0].Rows[j]["CHA_TIPO_DEST"].ToString();
                                    DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                                    if (chaTipoDest == "R")
                                        corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getRuolo(dsTrasmScadute_Dest_3.Tables[0].Rows[j]["ID_CORR_GLOBALE"].ToString());
                                    if (chaTipoDest == "U")
                                    {
                                        corr = (DocsPaVO.utente.Corrispondente)Utenti.UserManager.getUtente(dsTrasmScadute_Dest_3.Tables[0].Rows[j]["ID_DESTINATARIO"].ToString());
                                        corr.systemId = dsTrasmScadute_Dest_3.Tables[0].Rows[j]["ID_CORR_GLOBALE"].ToString();
                                    }

                                    string tipoTrasm = dsTrasmScadute_Dest_3.Tables[0].Rows[i]["CHA_TIPO_TRASM"].ToString();
                                    trasmissione = addTrasmissioneSingola(trasmissione, corr, ragione, "La Trasmissione del : " + ((string[])dataInvio.Split(' '))[0] + " è scaduta.", tipoTrasm);
                                }
                            }
                        }

                        //Eseguo la trasmissione
                        
                        //trasmissione = saveTrasmMethod(trasmissione);
                        //ExecTrasmManager.executeTrasmMethod(path, trasmissione);

                        trasmissione = ExecTrasmManager.saveExecuteTrasmMethod(path, trasmissione);

                    }
                }
                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragione, string note, string tipoTrasm)
        {
            //Controllo se il ruolo è disabilitato alla ricezione delle trasmissioni
            if(corr != null && corr.tipoCorrispondente == "R")
            {
                DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti();
                DocsPaVO.utente.Corrispondente corrispondente = utentiDb.getRuoloById(corr.systemId);
                if (corrispondente != null && corrispondente.disabledTrasm)
                    return trasmissione;
            }

            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Count; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts.corrispondenteInterno.systemId != null && ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;
            
            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPaVO.utente.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                //DocsPaVO.utente.Corrispondente[] listaUtenti = queryUtenti(corr);
                ArrayList listaUtenti = queryUtenti(corr);
                //if (listaUtenti.Length == 0)
                if (listaUtenti.Count == 0)
                {
                    //Prova Andrea
                    trasmissione.listaDestinatariNonRaggiungibili.Add("Nessun utente per il ruolo " + corr.codiceCorrispondente + " (" + corr.descrizione + ").");
                    //End Andrea
                    trasmissioneSingola = null;
                }
                //ciclo per utenti se dest è gruppo o ruolo
                //for (int i = 0; i < listaUtenti.Length; i++)
                for (int i = 0; i < listaUtenti.Count; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaVO.utente.Utente)listaUtenti[i];
                    //trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
                }
            }

            if (corr is DocsPaVO.utente.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;
                //trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
            }

            if (corr is DocsPaVO.utente.UnitaOrganizzativa)
            {
                DocsPaVO.utente.UnitaOrganizzativa theUo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                //qca.ruolo = UserManager.getRuolo();
                qca.ruolo = trasmissione.ruolo;
                qca.queryCorrispondente = new DocsPaVO.addressbook.QueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                //DocsPaVO.utente.Ruolo[] ruoli = Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo);
                ArrayList ruoli = Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo);
                //Andrea
                if (ruoli == null || ruoli.Count == 0) 
                { 
                    //Popola una lista con tutti i destinatari non raggiungibili
                    trasmissione.listaDestinatariNonRaggiungibili.Add("Manca un ruolo di riferimento per la UO: " + corr.codiceCorrispondente + " (" + corr.descrizione + ") " + ".");
                }
                //End Andrea
                foreach (DocsPaVO.utente.Ruolo r in ruoli)
                    trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm);

                return trasmissione;
            }

            if (trasmissioneSingola != null)
                //trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
                trasmissione.trasmissioniSingole.Add(trasmissioneSingola);
            return trasmissione;
        }

        public static ArrayList queryUtenti(DocsPaVO.utente.Corrispondente corr)
        {

            //costruzione oggetto queryCorrispondente
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;

            //qco.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
            qco.idAmministrazione = corr.idAmministrazione;

            //corrispondenti interni
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
            qco.fineValidita = true;

            //return Utenti.addressBookManager.getListaCorrispondenti(qco);
            return Utenti.addressBookManager.getListaCorrispondenti(qco);
        }

        #endregion Scadenze Trasmissione

        #region Nuova Gestione TODOLIST


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <param name="registri"></param>
        /// <param name="type"></param>
        /// <param name="filter"></param>
        /// <param name="requestedPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecordCount"></param>
        /// <returns></returns>
        public ArrayList getMyTodoList(DocsPaVO.utente.InfoUtente infoUtente,
                    string docOrFasc, string registri,
                    DocsPaVO.filtri.FiltroRicerca[] filter,
                    int requestedPage, int pageSize, out int totalRecordCount)
        {
            totalRecordCount = 0;

            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();

            return trasm.getMyTodoList(docOrFasc, infoUtente.idPeople, infoUtente.idGruppo, registri, filter, requestedPage, pageSize, out totalRecordCount);
        }

        //Gestione unificata todolist: ricerca sia i documenti che i fascicoli
        public ArrayList getMyNewTodoList(DocsPaVO.utente.InfoUtente infoUtente,
                   string registri,
                   DocsPaVO.filtri.FiltroRicerca[] filter,
                   int requestedPage, int pageSize, out int totalRecordCount, out int totalTrasmNonViste)
        {
            totalRecordCount = 0;
            totalTrasmNonViste = 0;

            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();

            return trasm.getMyNewTodoList(infoUtente.idPeople, infoUtente.idGruppo, registri, filter, requestedPage, pageSize, out totalRecordCount, out totalTrasmNonViste);
        }

        public bool RimuoviToDoListACL(string idTrasmUtente, string idTrasmSingola, string idPeople)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();

            return trasm.RimuoviToDoListACL(idTrasmUtente, idTrasmSingola, idPeople);
            
        }

        public bool getAllTodoList(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruoloCorr)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasm.getAllTodoList(utente, ruoloCorr);
        }

        public bool getPredInTodoList(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruoloCorr)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasm.getPredInTodoList(utente, ruoloCorr);
        }

        public ArrayList getDettagliAllTodoList(DocsPaVO.utente.Utente utente)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasm.getDettagliAllTodolist(utente);
        }

        #endregion

        #region Gestione CESSIONE DIRITTI

        private static bool verificaCessione(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            return (objTrasm.cessione!=null && objTrasm.cessione.docCeduto);
        }

        private static void gestioneCessioneDiritti(DocsPaVO.trasmissione.Trasmissione objTrasm)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            trasmissione.execSenderRigths(objTrasm);
        }

        #endregion

        /// <summary>
        /// Crea un oggetto trasmissione tramite la system_id di una trasmissione
        /// </summary>
        /// <param name="idTrasmissione">system_id della trasmissione</param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.Trasmissione CreateObjTrasmissioneByID(string idTrasmissione)
        {
            logger.Debug("INIZIO : crea oggetto TRASMISSIONE");

            DocsPaDB.Query_DocsPAWS.Utenti queryUt = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaDB.Query_DocsPAWS.Documenti queryDoc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaDB.Query_DocsPAWS.Fascicoli queryFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            DocsPaDB.Query_DocsPAWS.Trasmissione queryTrasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();            

            DocsPaVO.trasmissione.Trasmissione newTrasmissione = new DocsPaVO.trasmissione.Trasmissione(); ;
            DocsPaVO.trasmissione.TrasmissioneSingola newTrasSingola;
            DocsPaVO.trasmissione.RagioneTrasmissione newRagioneTrasm;
            DocsPaVO.trasmissione.TrasmissioneUtente newTrasmUtente;
            DocsPaVO.utente.Utente newUtente;
            DocsPaVO.utente.Ruolo newRuolo;
            DocsPaVO.utente.UnitaOrganizzativa newUO;

            DataSet dsDati;
            DataSet dsDatiUtente;

            dsDati = queryTrasm.GetDatiTrasmissioneByID(idTrasmissione);

            if (dsDati.Tables[0].Rows.Count > 0)
            {
                foreach(DataRow rowTX in dsDati.Tables[0].Rows)
                {
                    // DATI TRASMISSIONE
                    
                    newTrasmissione.systemId = idTrasmissione;
                    newTrasmissione.dataInvio = rowTX["dta_invio"].ToString();
                    newTrasmissione.noteGenerali = rowTX["var_note_generali"].ToString();
                    if (rowTX["cha_tipo_oggetto"].ToString().Equals("D"))
                    {
                        newTrasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;                        
                        DocsPaVO.documento.InfoDocumento infoDoc = queryDoc.GetInfoDocumento(null, null, rowTX["id_profile"].ToString(), false);
                        newTrasmissione.infoDocumento = infoDoc;
                    }
                    else
                    {
                        newTrasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
                        DocsPaVO.fascicolazione.InfoFascicolo infoFasc = new DocsPaVO.fascicolazione.InfoFascicolo();
                            infoFasc.idFascicolo = rowTX["id_project"].ToString();
                            infoFasc.idClassificazione = queryFasc.GetProjectData("ID_PARENT", "WHERE SYSTEM_ID = " + rowTX["id_project"].ToString());
                            infoFasc.idRegistro = queryFasc.GetProjectData("ID_REGISTRO", "WHERE SYSTEM_ID = " + rowTX["id_project"].ToString());
                        newTrasmissione.infoFascicolo = infoFasc;
                    }
                    newTrasmissione.utente = BusinessLogic.Utenti.UserManager.getUtenteNoFiltroDisabled(rowTX["id_people"].ToString());
                    newTrasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuoloEnabledAndDisabled(rowTX["id_ruolo_in_uo"].ToString());
                    newTrasmissione.delegato = rowTX["id_people_delegato"].ToString();
                    // DATI TRASMISSIONI SINGOLE
                    dsDati = null;
                    dsDati = queryTrasm.GetTrasmissioniSingoleByIDTrasmissione(idTrasmissione);
                    if (dsDati.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rowTS in dsDati.Tables[0].Rows)
                        {
                            newTrasSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();

                            newTrasSingola.systemId = rowTS["system_id"].ToString();
                            newTrasSingola.noteSingole = rowTS["var_note_sing"].ToString();
                            newTrasSingola.tipoTrasm = rowTS["cha_tipo_trasm"].ToString();
                            newTrasSingola.dataScadenza = rowTS["dta_scadenza"].ToString();

                            if (rowTS["cha_tipo_dest"].ToString().Equals("R"))
                            {
                                newTrasSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                                newRuolo = new DocsPaVO.utente.Ruolo();
                                newRuolo = BusinessLogic.Utenti.UserManager.getRuoloEnabledAndDisabled(rowTS["id_corr_globale"].ToString());
                                newTrasSingola.corrispondenteInterno = newRuolo;                               
                            }
                            if (rowTS["cha_tipo_dest"].ToString().Equals("U"))
                            {
                                newTrasSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                                newUtente = new DocsPaVO.utente.Utente();
                                newUtente = BusinessLogic.Utenti.UserManager.getUtenteNoFiltroDisabled(queryUt.GetFromCorrGlobGeneric("ID_PEOPLE", "SYSTEM_ID = " + rowTS["id_corr_globale"].ToString()));                                
                                newTrasSingola.corrispondenteInterno = newUtente;
                            }

                            // verifica il settaggio del campo eredita in base alla scelta utente
                            string setEredita = rowTS["cha_set_eredita"].ToString();
                            if (string.IsNullOrEmpty(setEredita))
                            {
                                setEredita = "1";
                            }

                            // ragione di trasmissione
                            newRagioneTrasm = new DocsPaVO.trasmissione.RagioneTrasmissione();
                            newRagioneTrasm.systemId = rowTS["id_ragione"].ToString();
                            newRagioneTrasm.descrizione = rowTS["var_desc_ragione"].ToString();
                            newRagioneTrasm.tipo = rowTS["cha_tipo_ragione"].ToString();
                            newRagioneTrasm.risposta = rowTS["cha_risposta"].ToString();
                            newRagioneTrasm.note = rowTS["var_note"].ToString();
                            if (setEredita == "1")
                            {
                                newRagioneTrasm.eredita = rowTS["cha_eredita"].ToString();
                            }
                            else
                            {
                                newRagioneTrasm.eredita = "0";
                            }
                            newRagioneTrasm.tipoRisposta = rowTS["cha_tipo_risposta"].ToString();
                            newRagioneTrasm.notifica = rowTS["var_notifica_trasm"].ToString();
                            newRagioneTrasm.testoMsgNotificaDoc = rowTS["var_testo_msg_notifica_doc"].ToString();
                            newRagioneTrasm.testoMsgNotificaFasc = rowTS["var_testo_msg_notifica_fasc"].ToString();
                            if (rowTS["cha_tipo_diritti"].ToString().Equals("W"))
                                newRagioneTrasm.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.WRITE;
                            if (rowTS["cha_tipo_diritti"].ToString().Equals("N"))
                                newRagioneTrasm.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.NONE;
                            if (rowTS["cha_tipo_diritti"].ToString().Equals("R"))
                                newRagioneTrasm.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.READ;
                            if (rowTS["cha_tipo_dest"].ToString().Equals("I"))
                                newRagioneTrasm.tipoDestinatario = DocsPaVO.trasmissione.TipoGerarchia.INFERIORE;
                            if (rowTS["cha_tipo_dest"].ToString().Equals("P"))
                                newRagioneTrasm.tipoDestinatario = DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO;
                            if (rowTS["cha_tipo_dest"].ToString().Equals("S"))
                                newRagioneTrasm.tipoDestinatario = DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE;
                            if (rowTS["cha_tipo_dest"].ToString().Equals("T"))
                                newRagioneTrasm.tipoDestinatario = DocsPaVO.trasmissione.TipoGerarchia.TUTTI;                            
                            newTrasSingola.ragione = newRagioneTrasm;

                            // trasmissioni utente
                            dsDatiUtente = queryTrasm.GetTrasmissioniUtenteByIDTrasmSingola(rowTS["system_id"].ToString());
                            if (dsDatiUtente.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow rowUt in dsDatiUtente.Tables[0].Rows)
                                {
                                    newTrasmUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();

                                    newTrasmUtente.systemId = rowUt["system_id"].ToString();
                                    newTrasmUtente.dataAccettata = rowUt["dta_accettata"].ToString();
                                    newTrasmUtente.dataRifiutata = rowUt["dta_rifiutata"].ToString();
                                    newTrasmUtente.dataVista = rowUt["dta_vista"].ToString();
                                    newTrasmUtente.dataRisposta = rowUt["dta_risposta"].ToString();
                                    newTrasmUtente.noteAccettazione = rowUt["var_note_acc"].ToString();
                                    newTrasmUtente.noteRifiuto = rowUt["var_note_rif"].ToString();
                                    newTrasmUtente.valida = rowUt["cha_valida"].ToString();
                                    newTrasmUtente.idTrasmRispSing = rowUt["id_trasm_risp_sing"].ToString();
                                    newTrasmUtente.dataRimossaTDL = rowUt["dta_rimozione_todolist"].ToString();
                                    newTrasmUtente.utente = queryUt.GetUtente(rowUt["id_people"].ToString());

                                    newTrasSingola.trasmissioneUtente.Add(newTrasmUtente);
                                }
                            }

                            newTrasmissione.trasmissioniSingole.Add(newTrasSingola);
                        }
                    }
                }
            }

            logger.Debug("FINE : crea oggetto TRASMISSIONE");
            return newTrasmissione;
        }

        public static bool checkTrasm_UNO_TUTTI_AccettataRifiutata(DocsPaVO.trasmissione.TrasmissioneSingola trasmSingola)
        {
            bool retValue = true;

            if (trasmSingola.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.RUOLO)
            {
                DocsPaDB.Query_DocsPAWS.Trasmissione queryObj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                switch (trasmSingola.tipoTrasm)
                {
                    case "S":
                        retValue = queryObj.checkTrasm_UNO_AccettataRifiutata(trasmSingola.systemId);
                        break;

                    case "T":
                        retValue = queryObj.checkTrasm_TUTTI_AccettataRifiutata(trasmSingola.systemId);
                        break;
                }
            }

            return retValue;
        }

        public static int ContaTrasm_Da_ACC_RIF_Per_OggettoRuoloUtente(string tipoOggetto, string IDOggetto, string IDGroup, string IDPeople)
        {
            int retValue = 0;

            try
            {                
                DocsPaDB.Query_DocsPAWS.Trasmissione queryObj = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                retValue = queryObj.ContaTrasm_Da_ACC_Per_OggettoRuoloUtente(tipoOggetto, IDOggetto, IDGroup, IDPeople);
            }
            catch
            {
                retValue = -1;
            }

            return retValue;
        }

        public static bool TrasmissioneExecuteTrasmFascDaModello(string serverPath, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

            //Parametri della trasmissione
            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

            trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
            //trasmissione.infoDocumento = DocumentManager.getInfoDocumento(scheda);
            DocsPaVO.fascicolazione.Fascicolo fasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicolo.systemID, infoUtente);
            trasmissione.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(fasc);

            trasmissione.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);
            trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);

            trasmissione.NO_NOTIFY = modello.NO_NOTIFY;

            //Parametri delle trasmissioni singole
            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
            {
                DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];

                    //DocsPaVO.utente.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this,mittDest.VAR_COD_RUBRICA,DocsPaWR.AddressbookTipoUtente.INTERNO);
                    DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, infoUtente);

                    //DocsPaVO.trasmissione.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());?
                    DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());
                    trasmissione = TrasmManager.addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM);
                }
            }
            if (infoUtente.delegato != null)
                trasmissione.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;

            //trasmissione = BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasmissione);
            //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(
            trasmissione = ExecTrasmManager.saveExecuteTrasmMethod(serverPath, trasmissione);
            return true;

        }

        /// <summary>
        /// Funzione per l'invio di una trasmissione da modello tramite specifica del codice del modello
        /// </summary>
        /// <param name="userInfo">Le informazioni sull'utente</param>
        /// <param name="serverPath">Il path dell'applicazione WA</param>
        /// <param name="scheda">La scheda del documento da trasmettere</param>
        /// <param name="modelCode">Il codice del modello</param>
        /// <param name="role">Il ruolo dell'utente</param>
        /// <param name="trasmModel">Variabile di out, conterrà il modello recuperato</param>
        /// <returns>True se la trasmissione avviene con successo, false altrimenti. 
        ///          Se il risultato è false provare a controllare il valore della variabile di out.
        ///          Se è null, significa che non è stato possibile recuperare il modello, altrimenti se
        ///          è valorizzato controllare il tipo di oggetto da trasmettere cui si applica il modello
        ///          ("D" = Documento, "F" = Fascicolo)
        /// </returns>
        public static bool TransmissionExecuteDocTransmFromModelCode(DocsPaVO.utente.InfoUtente userInfo, string serverPath, DocsPaVO.documento.SchedaDocumento scheda, string modelCode, DocsPaVO.utente.Ruolo role, out DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione trasmModel)
        {
            #region

            // L'indice dell'ultimo _
            int lastUnderscore = -1;

            // L'id del modello da recuperare
            string modelId = String.Empty;

            // Il modello recuperato
            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model = null;

            // L'oggetto trasmissione
            DocsPaVO.trasmissione.Trasmissione transmission;

            // Un oggetto ragione destinatario utilizzato durante la creazione delle
            // trasmissioni singole
            DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest;

            // L'array list dei destinatari
            ArrayList destinatari;

            // Le informazioni sul corrispondente cui inviare la trasmissione
            DocsPaVO.utente.Corrispondente corr;

            // La ragione di trasmissione
            DocsPaVO.trasmissione.RagioneTrasmissione ragione;

            // Il valore da restituire
            bool toReturn = false;

            #endregion

            try
            {
                // Individuazione dell'id del modello a partire dal codice
                lastUnderscore = modelCode.LastIndexOf('_');

                // Se è stato trovato un '_' si preleva l'id altrimenti l'id è pari al codice
                if (lastUnderscore != -1)
                    modelId = modelCode.Substring(lastUnderscore + 1);
                else
                    modelId = modelCode;

                // Richiesta del modello di trasmissione
                model = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByID(role.idAmministrazione, modelId);

                // Se è stato trovato un modello è tale modello si applica ai tocumenti, 
                // si procede con la trasmissione
                if (model != null && model.CHA_TIPO_OGGETTO == "D")
                {
                    // Creazione dell'oggetto trasmissione
                    transmission = new DocsPaVO.trasmissione.Trasmissione();

                    // Impostazione dei parametri della trasmissione
                    // Note generali
                    transmission.noteGenerali = model.VAR_NOTE_GENERALI;

                    // Tipo oggetto
                    transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                    // Informazioni sul documento
                    transmission.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(scheda);

                    // Utente mittente della trasmissione
                    transmission.utente = BusinessLogic.Utenti.UserManager.getUtente(userInfo.idPeople);

                    // Ruolo mittente
                    transmission.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(userInfo.idCorrGlobali);

                    transmission.NO_NOTIFY = model.NO_NOTIFY;

                    // Impostazione dei parametri per le trasmissioni singole
                    for (int i = 0; i < model.RAGIONI_DESTINATARI.Count; i++)
                    {
                        // Recupero delli ragioni di trasmissione
                        ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)model.RAGIONI_DESTINATARI[i];

                        // Creazione della lista dei destinatari
                        destinatari = new ArrayList(ragDest.DESTINATARI);

                        // Per ogni destinatario...
                        foreach (DocsPaVO.Modelli_Trasmissioni.MittDest mittDest in destinatari)
                        {
                            // ...prelevamento dei dati sul corrispondente
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(
                                mittDest.VAR_COD_RUBRICA, userInfo);

                            // ...prelevamento della ragione di trasmissione
                            ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(
                                mittDest.ID_RAGIONE.ToString());

                            // ...aggiunta della trasmissione singola
                            transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                                transmission,
                                corr,
                                ragione,
                                mittDest.VAR_NOTE_SING,
                                mittDest.CHA_TIPO_TRASM);
                        }
                    }

                    // Se l'utente è un delegato...
                    if (userInfo.delegato != null)
                        // ...impostazione dell'idPeople del delegato
                        transmission.delegato = ((DocsPaVO.utente.InfoUtente)(userInfo.delegato)).idPeople;

                    // Trasmissione
                    transmission = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(
                        serverPath, transmission);

                    // Il risultato da restituire è successo
                    toReturn = true;
                }
            }
            catch (Exception e)
            {
                // Il risulto è insuccesso
                toReturn = false;

            }

            // Impostazione del modello di trasmissione
            trasmModel = model;

            // Restituzione dell'esito
            return toReturn;

        }


        /// <summary>
        /// Funzione che data una trasmissione singola trova la trasmissione utente
        /// </summary>
        /// </returns>
        public static DocsPaVO.trasmissione.TrasmissioneUtente[] getTrasmissioneUtenteInRuolo(DocsPaVO.utente.InfoUtente infoUtente, string trasmissione, DocsPaVO.utente.Utente utente)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            DocsPaVO.trasmissione.TrasmissioneUtente[] trasmissioniUtenteInRuolo = null;
            trasmissioniUtenteInRuolo = trasm.getTrasmissioneUtenteInRuolo(infoUtente, trasmissione, utente);
            return trasmissioniUtenteInRuolo;
        }

        /// <summary>
        /// Funzione che dato l'id della trasmissione singola, restituisce l'id della trasmissione
        /// </summary>
        /// <param name="idTrasmSingola"></param>
        /// <returns></returns>
        public static string GetIdTrasmissioneByIdTrasmSingola(string idTrasmSingola)
        { 
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasm.GetIdTrasmissioneByIdTrasmSingola(idTrasmSingola);
        }

        /// <summary>
        /// Funzione che data una trasmissione singola ritorna tutte le trasmissioni utente
        /// </summary>
        /// </returns>
        public static DocsPaVO.trasmissione.TrasmissioneUtente[] getTrasmissioniUtenteDaSingola(string idTrasmSing)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            DocsPaVO.trasmissione.TrasmissioneUtente[] trasmissioniUtente = null;
            trasmissioniUtente = trasm.getTrasmissioniUtenteDaSingola(idTrasmSing);
            return trasmissioniUtente;
        }

        public ArrayList getMyNewTodoListMigrazione(DocsPaVO.utente.InfoUtente infoUtente)
        {
            ArrayList list = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                list = trasm.getMyNewTodoListMigrazione(infoUtente.idPeople, infoUtente.idGruppo);
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore in getMyNewDocTodoListMigrazione:\n{0}", ex.Message));
            }
            return list;
        }

        /// <summary>
        /// Crea un oggetto trasmissione tramite la system_id di una trasmissione e system_id trasmissione utente
        /// </summary>
        /// <param name="idTrasmissione">system_id della trasmissione e system_id trasmissione utente</param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.Trasmissione CreateObjTrasmissioneByIDLite(string idTrasmissione, string idTrasmissioneUtente)
        {
            logger.Debug("INIZIO : crea oggetto TRASMISSIONE_LITE");
            DocsPaDB.Query_DocsPAWS.Trasmissione queryTrasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            DocsPaVO.trasmissione.Trasmissione newTrasmissione = new DocsPaVO.trasmissione.Trasmissione(); ;
            DocsPaVO.trasmissione.TrasmissioneSingola newTrasSingola;
            DocsPaVO.trasmissione.RagioneTrasmissione newRagioneTrasm;
            DocsPaVO.utente.Utente newUser = new DocsPaVO.utente.Utente();
            DocsPaVO.utente.Ruolo newRole = new DocsPaVO.utente.Ruolo();
            DocsPaVO.utente.Ruolo newRuolo = new DocsPaVO.utente.Ruolo();
            DocsPaVO.utente.Utente newUtente = new DocsPaVO.utente.Utente();
            DocsPaVO.trasmissione.TrasmissioneUtente newTrasmUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
            DocsPaDB.Query_DocsPAWS.Utenti queryUt = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaDB.Query_DocsPAWS.Documenti queryDoc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaDB.Query_DocsPAWS.Fascicoli queryFasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            DataSet dsDati;
            DataSet dsDatiUtente;

            dsDati = queryTrasm.GetDatiTrasmissioneByIDLite(idTrasmissione, idTrasmissioneUtente);

            if (dsDati.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow rowTX in dsDati.Tables[0].Rows)
                {
                    newTrasmissione.systemId = idTrasmissione;
                    if (rowTX["cha_tipo_oggetto"].ToString().Equals("D"))
                    {
                        newTrasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                        DocsPaVO.documento.InfoDocumento infoDoc = queryDoc.GetInfoDocumento(null, null, rowTX["id_profile"].ToString(), false);
                        newTrasmissione.infoDocumento = infoDoc;
                    }
                    else
                    {
                        newTrasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
                        DocsPaVO.fascicolazione.InfoFascicolo infoFasc = new DocsPaVO.fascicolazione.InfoFascicolo();
                        infoFasc.idFascicolo = rowTX["id_project"].ToString();
                        infoFasc.idClassificazione = queryFasc.GetProjectData("ID_PARENT", "WHERE SYSTEM_ID = " + rowTX["id_project"].ToString());
                        infoFasc.idRegistro = queryFasc.GetProjectData("ID_REGISTRO", "WHERE SYSTEM_ID = " + rowTX["id_project"].ToString());
                        newTrasmissione.infoFascicolo = infoFasc;
                    }
                    newUser.idPeople = rowTX["id_people"].ToString();
                    if(rowTX["id_amm_people"] != DBNull.Value)
                        newUser.idAmministrazione = rowTX["id_amm_people"].ToString();
                    newTrasmissione.utente = newUser;
                    newRole.systemId = rowTX["id_ruolo_in_uo"].ToString();
                    newRole.idGruppo = rowTX["id_gruppo"].ToString();
                    newTrasmissione.ruolo = newRole;

                    newTrasSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();

                    newTrasSingola.systemId = rowTX["system_id"].ToString();
                    newTrasSingola.tipoTrasm = rowTX["cha_tipo_trasm"].ToString();

                    if (rowTX["cha_tipo_dest"].ToString().Equals("R"))
                    {
                        newTrasSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                        newRuolo = new DocsPaVO.utente.Ruolo();
                        newRuolo = BusinessLogic.Utenti.UserManager.getRuoloEnabledAndDisabled(rowTX["id_corr_globale"].ToString());
                        newTrasSingola.corrispondenteInterno = newRuolo;
                    }
                    if (rowTX["cha_tipo_dest"].ToString().Equals("U"))
                    {
                        newTrasSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                        newUtente = new DocsPaVO.utente.Utente();
                        newUtente = BusinessLogic.Utenti.UserManager.getUtenteNoFiltroDisabled(queryUt.GetFromCorrGlobGeneric("ID_PEOPLE", "SYSTEM_ID = " + rowTX["id_corr_globale"].ToString()));
                        newTrasSingola.corrispondenteInterno = newUtente;
                    }

                    // verifica il settaggio del campo eredita in base alla scelta utente
                    string setEredita = rowTX["cha_set_eredita"].ToString();
                    if (string.IsNullOrEmpty(setEredita))
                    {
                        setEredita = "1";
                    }

                    // ragione di trasmissione
                    newRagioneTrasm = new DocsPaVO.trasmissione.RagioneTrasmissione();
                    newRagioneTrasm.tipo = rowTX["cha_tipo_ragione"].ToString();
                    newRagioneTrasm.systemId = rowTX["id_ragione"].ToString();

                    if (setEredita == "1")
                    {
                        newRagioneTrasm.eredita = rowTX["cha_eredita"].ToString();
                    }
                    else
                    {
                        newRagioneTrasm.eredita = "0";
                    }

                    if (rowTX["cha_tipo_diritti"].ToString().Equals("W"))
                        newRagioneTrasm.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.WRITE;
                    if (rowTX["cha_tipo_diritti"].ToString().Equals("N"))
                        newRagioneTrasm.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.NONE;
                    if (rowTX["cha_tipo_diritti"].ToString().Equals("R"))
                        newRagioneTrasm.tipoDiritti = DocsPaVO.trasmissione.TipoDiritto.READ;
                    if (rowTX["cha_tipo_dest_rag"].ToString().Equals("I"))
                        newRagioneTrasm.tipoDestinatario = DocsPaVO.trasmissione.TipoGerarchia.INFERIORE;
                    if (rowTX["cha_tipo_dest_rag"].ToString().Equals("P"))
                        newRagioneTrasm.tipoDestinatario = DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO;
                    if (rowTX["cha_tipo_dest_rag"].ToString().Equals("S"))
                        newRagioneTrasm.tipoDestinatario = DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE;
                    if (rowTX["cha_tipo_dest_rag"].ToString().Equals("T"))
                        newRagioneTrasm.tipoDestinatario = DocsPaVO.trasmissione.TipoGerarchia.TUTTI;
                    newTrasSingola.ragione = newRagioneTrasm;

                    // trasmissioni utente
                    dsDatiUtente = queryTrasm.GetTrasmissioniUtenteByIDTrasmSingola(newTrasSingola.systemId);
                    if (dsDatiUtente.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rowUt in dsDatiUtente.Tables[0].Rows)
                        {
                            newTrasmUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();

                            newTrasmUtente.systemId = rowUt["system_id"].ToString();
                            newTrasmUtente.dataAccettata = rowUt["dta_accettata"].ToString();
                            newTrasmUtente.dataRifiutata = rowUt["dta_rifiutata"].ToString();
                            newTrasmUtente.dataVista = rowUt["dta_vista"].ToString();
                            newTrasmUtente.dataRisposta = rowUt["dta_risposta"].ToString();
                            newTrasmUtente.noteAccettazione = rowUt["var_note_acc"].ToString();
                            newTrasmUtente.noteRifiuto = rowUt["var_note_rif"].ToString();
                            newTrasmUtente.valida = rowUt["cha_valida"].ToString();
                            newTrasmUtente.idTrasmRispSing = rowUt["id_trasm_risp_sing"].ToString();
                            newTrasmUtente.dataRimossaTDL = rowUt["dta_rimozione_todolist"].ToString();
                            newTrasmUtente.utente = queryUt.GetUtente(rowUt["id_people"].ToString());

                            newTrasSingola.trasmissioneUtente.Add(newTrasmUtente);
                        }
                    }

                    newTrasmissione.trasmissioniSingole.Add(newTrasSingola);
                }
                logger.Debug("FINE : crea oggetto TRASMISSIONE");

            }
            return newTrasmissione;
        }

        /// <summary>
        /// Funzione che data una trasmissione singola ritorna tutte le trasmissioni utente
        /// </summary>
        /// </returns>
        public static string getDestinatariTrasmissioniUtenteDaSingola(string idTrasmSing)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            string destinatari = string.Empty;
            destinatari = trasm.getDestinatariTrasmissioniUtenteDaSingola(idTrasmSing);
            return destinatari;
        }

        /// <summary>
        /// Metodo utilizzato per verificare se la to do list di un ruolo contiene trasmissioni rivolte a ruolo
        /// pendenti
        /// </summary>
        /// <param name="roleIdCorrGlob">Id corr globali del ruolo da verificare</param>
        /// <returns>True se ci sono trasmissioni a ruolo pendenti</returns>
        public static bool CheckIfUserHaveRoleTransmission(String roleIdCorrGlob)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasm.CheckIfUserHaveRoleTransmission(roleIdCorrGlob);

        }

        #region Metodi per la gestione dell'accettazione massiva di documenti e fascicoli in To do list

        /// <summary>
        /// Metodo per l'accettazione massiva di trasmissioni presenti nella to do list
        /// di uno specifico utente
        /// </summary>
        /// <param name="user">Utente di cui analizzare la to do list</param>
        /// <param name="role">Ruolo</param>
        /// <param name="notes">Note generali da impostare</param>
        /// <returns>Esito dell'operazione</returns>
        public static DocsPaVO.amministrazione.EsitoOperazione AccettazioneMassivaInTdl(DocsPaVO.utente.Utente user, DocsPaVO.utente.Ruolo role, String notes)
        {
            // Recupero della to do list dell'utente
            DocsPaVO.trasmissione.infoToDoList[] userToDoList = (DocsPaVO.trasmissione.infoToDoList[])(new DocsPaDB.Query_DocsPAWS.Trasmissione().getMyNewTodoListMigrazione(user.systemId, role.idGruppo).ToArray(typeof(DocsPaVO.trasmissione.infoToDoList)));

            if (userToDoList == null)
                userToDoList = new DocsPaVO.trasmissione.infoToDoList[0];

            // Recupero dei dettagli delle trasmissioni in to do list
            List<DocsPaVO.trasmissione.Trasmissione> trasmsDetails = GetTrasmDetails(userToDoList, role, user);

            // Oggetto da restituire all'utente
            DocsPaVO.amministrazione.EsitoOperazione retVal = new DocsPaVO.amministrazione.EsitoOperazione();

            // Messaggio da restituire all'utente
            StringBuilder message = new StringBuilder();
            String msg;

            // Risultato dell'elaborazione
            bool result = true;

            // Analisi delle trasmissioni
            foreach (var trasm in trasmsDetails)
            {
                // Prelevamento di tutte le trasmissioni singole dirette a RUOLO
                IEnumerable<DocsPaVO.trasmissione.TrasmissioneSingola> roleTransmissions = ((DocsPaVO.trasmissione.TrasmissioneSingola [])trasm.trasmissioniSingole.ToArray(typeof(DocsPaVO.trasmissione.TrasmissioneSingola))).Where(e => e.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.RUOLO);

                // Analisi delle trasmissioni singole che compongono la trasmissione
                foreach (DocsPaVO.trasmissione.TrasmissioneSingola singleTrasm in roleTransmissions)
	            {
                    DocsPaDB.Query_DocsPAWS.Trasmissione tManager = new DocsPaDB.Query_DocsPAWS.Trasmissione();

                    // Se la ragione di trasmissione prevede accettazione (Workflow), viene verificato di che tipo di trasmissione
                    // si tratta (Singola o Tutti) e si procede con le accettazioni
                    if (singleTrasm.ragione.tipo.ToUpper().Equals("W"))
                    {
                        switch(singleTrasm.tipoTrasm.ToUpper())
                        {
                            case "S":   // Singola
                                // Se la trasmissione singola non è già stata accettata, si procede con l'accettazione
                                if (tManager.checkTrasm_UNO_AccettataRifiutata(singleTrasm.systemId))
                                {
                                    DocsPaVO.trasmissione.TrasmissioneUtente userTrasm = singleTrasm.trasmissioneUtente[0] as DocsPaVO.trasmissione.TrasmissioneUtente;
                                    userTrasm.noteAccettazione = notes;
                                    trasm.utente.idAmministrazione = user.idAmministrazione;
                                    result &= AcceptTransmission(userTrasm, role, user, trasm, out msg);
                                    if (!String.IsNullOrEmpty(msg))
                                        message.AppendLine(msg);
                                }
                                break;
                            case "T":   // Tutti
                                // Se la trasmissione non è già stata accettata, si procede con l'accettazione
                                if (tManager.checkTrasm_TUTTI_AccettataRifiutata(singleTrasm.systemId))
                                    foreach (DocsPaVO.trasmissione.TrasmissioneUtente tu in singleTrasm.trasmissioneUtente)
                                    {
                                        tu.noteAccettazione = notes;
                                        result &= AcceptTransmission(tu, role, user, trasm, out msg);
                                        if (!String.IsNullOrEmpty(msg))
                                            message.AppendLine(msg);
                                    
                                    }
                                break;
                        }

                    }
                    
                    // Impostazione della data vista
                    if (trasm.tipoOggetto.ToString().Equals("DOCUMENTO"))
                        //new DocsPaDB.Query_DocsPAWS.Documenti().SetDataVistaSP_TASTOVISTO(user.systemId, trasm.infoDocumento.idProfile);
                        new DocsPaDB.Query_DocsPAWS.Documenti().SetDataVistaSP_TASTOVISTO(BusinessLogic.Utenti.UserManager.GetInfoUtente(user, role), trasm.infoDocumento.idProfile, "D");
                    else
                        //new DocsPaDB.Query_DocsPAWS.Fascicoli().SetDataVista(user.systemId, trasm.infoFascicolo.idFascicolo);
                        new DocsPaDB.Query_DocsPAWS.Documenti().SetDataVistaSP_TASTOVISTO(BusinessLogic.Utenti.UserManager.GetInfoUtente(user, role), trasm.infoFascicolo.idFascicolo, "F");
                }

            }

            // Costruzione e restituzione dell'esito
            retVal.Codice = result ? 0 : -1;
            retVal.Descrizione = message.ToString();

            return retVal;
              
        }

        /// <summary>
        /// Metodo per il reperimento dei dettagli delle trasmissioni relative ad un utente
        /// </summary>
        /// <param name="userTodoList">To do list dell'utente</param>
        /// <param name="role">Ruolo per cui ricercare le trasmissioni</param>
        /// <param name="user">Informazioni sull'utente di cui ricercare le trasmissioni da accettare</param>
        /// <returns>Lista con i dettagli delle trasmissioni pendenti</returns>
        private static List<DocsPaVO.trasmissione.Trasmissione> GetTrasmDetails(DocsPaVO.trasmissione.infoToDoList[] userTodoList, DocsPaVO.utente.Ruolo role, DocsPaVO.utente.Utente user)
        {
            // Lista delle trasmissioni
            List<DocsPaVO.trasmissione.Trasmissione> trasms = new List<DocsPaVO.trasmissione.Trasmissione>();

            // Oggetto della trasmissione
            DocsPaVO.trasmissione.OggettoTrasm trasmObj = new DocsPaVO.trasmissione.OggettoTrasm();

            // Per ogni oggetto nella to do list
            foreach (var toDo in userTodoList)
            {
                // Se la trasmissione si riferisce ad un documento, viene recuperata l'info documento altrimenti l'info fascicolo
                if (!String.IsNullOrEmpty(toDo.sysIdDoc))
                    using (DocsPaDB.Query_DocsPAWS.Documenti dbDoc = new DocsPaDB.Query_DocsPAWS.Documenti())
                        trasmObj.infoDocumento = dbDoc.GetInfoDocumento(role.idGruppo, user.idPeople, toDo.sysIdDoc, false);
                else
                    trasmObj.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo { idFascicolo = toDo.sysIdFasc };

                // Aggiunta del dettaglio della trasmissione e alla lista delle trasmissioni
                trasms.AddRange((DocsPaVO.trasmissione.Trasmissione[])QueryTrasmManager.getQueryDettaglioTrasmMethod(trasmObj, user, role, null, toDo.sysIdTrasm).ToArray(typeof(DocsPaVO.trasmissione.Trasmissione)));
            }

            // Restituzione della lista delle trasmissioni
            return trasms;
        }

        /// <summary>
        /// Metodo per l'accettazione di una trasmissione e per l'impostazione della data vista
        /// </summary>
        /// <param name="userTrasm">Trasmissone utente da accettare</param>
        /// <param name="role">Ruolo con cui accettare</param>
        /// <param name="userInfo">Informazioni sull'utente che deve compiere l'accettazione</param>
        /// <param name="trasm">Tramsissione da accettare</param>
        /// <param name="errorMessage">Eventuale messaggio di errore</param>
        /// <returns>True se non ci sono stati errori bloccanti nell'effettuazione dell'accettazione</returns>
        private static bool AcceptTransmission(DocsPaVO.trasmissione.TrasmissioneUtente userTrasm, DocsPaVO.utente.Ruolo role, DocsPaVO.utente.Utente user, DocsPaVO.trasmissione.Trasmissione trasm, out String errorMessage)
        {
            string mode;
            string idObj;
            string msg;
            const string ACCETTATADOC = "ACCETTATA_DOCUMENTO";
            const string ACCETTATAFASC = "ACCETTATA_FASCICOLO";
            const string RIFIUTATADOC = "RIFIUTATA_DOCUMENTO";
            const string RIFIUTATAFASC = "RIFIUTATA_FASCICOLO";

            // Costruzione dell'info utente
            DocsPaVO.utente.InfoUtente userInfo = BusinessLogic.Utenti.UserManager.GetInfoUtente(user, role);

            // Accettazione, se non è già stata accettata
            bool retVal = true;
            retVal = ExecTrasmManager.executeAccRifMethod(userTrasm, trasm.systemId, role, userInfo, out errorMessage, out mode, out idObj);
            if (retVal && !string.IsNullOrEmpty(mode))
            {
                DocsPaVO.trasmissione.TrasmissioneSingola sing = BusinessLogic.Trasmissioni.ExecTrasmManager.getTrasmSingola(userTrasm);
                switch (mode)
                {
                    case ACCETTATADOC:
                        msg = "Accettazione della trasmissione. Id documento: " + idObj;
                        BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, role.idGruppo, user.idAmministrazione, "ACCEPTTRASMDOCUMENT", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", sing.systemId);
                        break;
                    case ACCETTATAFASC:
                        msg = "Accettazione della trasmissione. Id fascicolo: " + idObj;
                        BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, role.idGruppo, user.idAmministrazione, "ACCEPTTRASMFOLDER", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", sing.systemId);
                        break;
                    case RIFIUTATAFASC:
                        msg = "Rifiuto della trasmissione. Id fascicolo: " + idObj;
                        BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, role.idGruppo, user.idAmministrazione, "REJECTTRASMFOLDER", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", sing.systemId);
                        break;
                    case RIFIUTATADOC:
                        msg = "Rifiuto della trasmissione. Id documento: " + idObj;
                        BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, role.idGruppo, user.idAmministrazione, "REJECTTRASMDOCUMENT", idObj, msg, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1", sing.systemId);
                        break;
                }
            }

            return retVal;

        }

        #endregion




        /// <summary>
        /// Funzione che data una lista di trasmissioni restituisce la lista dei destinatari delle trasmissioni utente
        /// </summary>
        /// </returns>
        public static Dictionary<string, string> getDestinatariTrasmByListaTrasm(ArrayList trasmissioni)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasm = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            Dictionary<string, string> destTrasm = new Dictionary<string, string>();
            destTrasm = trasm.getDestinatariTrasmByListaTrasm(trasmissioni);
            return destTrasm;
        }

        /// <summary>
        /// Funzione per l'invio di una trasmissione da modello tramite specifica del codice del modello
        /// </summary>
        /// <param name="userInfo">Le informazioni sull'utente</param>
        /// <param name="serverPath">Il path dell'applicazione WA</param>
        /// <param name="scheda">La scheda del documento da trasmettere</param>
        /// <param name="modelCode">Il codice del modello</param>
        /// <param name="role">Il ruolo dell'utente</param>
        /// <param name="trasmModel">Variabile di out, conterrà il modello recuperato</param>
        /// <returns>True se la trasmissione avviene con successo, false altrimenti. 
        ///          Se il risultato è false provare a controllare il valore della variabile di out.
        ///          Se è null, significa che non è stato possibile recuperare il modello, altrimenti se
        ///          è valorizzato controllare il tipo di oggetto da trasmettere cui si applica il modello
        ///          ("D" = Documento, "F" = Fascicolo)
        /// </returns>
        public static bool TransmissionExecuteDocTransmFromModelCodeSoloConNotifica(DocsPaVO.utente.InfoUtente userInfo, string serverPath, DocsPaVO.documento.SchedaDocumento scheda, string modelCode, DocsPaVO.utente.Ruolo role, out DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione trasmModel)
        {
            #region

            // L'indice dell'ultimo _
            int lastUnderscore = -1;

            // L'id del modello da recuperare
            string modelId = String.Empty;

            // Il modello recuperato
            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model = null;

            // L'oggetto trasmissione
            DocsPaVO.trasmissione.Trasmissione transmission;

            // Un oggetto ragione destinatario utilizzato durante la creazione delle
            // trasmissioni singole
            DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest;

            // L'array list dei destinatari
            ArrayList destinatari;

            // Le informazioni sul corrispondente cui inviare la trasmissione
            DocsPaVO.utente.Corrispondente corr;

            // La ragione di trasmissione
            DocsPaVO.trasmissione.RagioneTrasmissione ragione;

            // Il valore da restituire
            bool toReturn = false;

            #endregion

            try
            {
                // Individuazione dell'id del modello a partire dal codice
                lastUnderscore = modelCode.LastIndexOf('_');

                // Se è stato trovato un '_' si preleva l'id altrimenti l'id è pari al codice
                if (lastUnderscore != -1)
                    modelId = modelCode.Substring(lastUnderscore + 1);
                else
                    modelId = modelCode;

                // Richiesta del modello di trasmissione
                model = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByIDSoloConNotifica(role.idAmministrazione, modelId);

                // Se è stato trovato un modello è tale modello si applica ai tocumenti, 
                // si procede con la trasmissione
                if (model != null && model.CHA_TIPO_OGGETTO == "D")
                {
                    // Creazione dell'oggetto trasmissione
                    transmission = new DocsPaVO.trasmissione.Trasmissione();

                    // Impostazione dei parametri della trasmissione
                    // Note generali
                    transmission.noteGenerali = model.VAR_NOTE_GENERALI;

                    // Tipo oggetto
                    transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;

                    // Informazioni sul documento
                    transmission.infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(scheda);

                    // Utente mittente della trasmissione
                    transmission.utente = BusinessLogic.Utenti.UserManager.getUtente(userInfo.idPeople);

                    // Ruolo mittente
                    transmission.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(userInfo.idCorrGlobali);

                    transmission.NO_NOTIFY = model.NO_NOTIFY;

                    string generaNotifica = "1";

                    if (!string.IsNullOrEmpty(model.NO_NOTIFY) && model.NO_NOTIFY.Equals("1"))
                    {
                        generaNotifica = "0";
                    }

                    // Impostazione dei parametri per le trasmissioni singole
                    for (int i = 0; i < model.RAGIONI_DESTINATARI.Count; i++)
                    {
                        // Recupero delli ragioni di trasmissione
                        ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)model.RAGIONI_DESTINATARI[i];

                        // Creazione della lista dei destinatari
                        destinatari = new ArrayList(ragDest.DESTINATARI);

                        // Per ogni destinatario...
                        foreach (DocsPaVO.Modelli_Trasmissioni.MittDest mittDest in destinatari)
                        {
                            // ...prelevamento dei dati sul corrispondente
                            corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(
                                mittDest.VAR_COD_RUBRICA, userInfo);

                            // ...prelevamento della ragione di trasmissione
                            ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(
                                mittDest.ID_RAGIONE.ToString());

                            // ...aggiunta della trasmissione singola
                            transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingolaSoloConNotifica(
                                transmission,
                                corr,
                                ragione,
                                mittDest.VAR_NOTE_SING,
                                mittDest.CHA_TIPO_TRASM,
                                mittDest);
                        }
                    }

                    // Se l'utente è un delegato...
                    if (userInfo.delegato != null)
                        // ...impostazione dell'idPeople del delegato
                        transmission.delegato = ((DocsPaVO.utente.InfoUtente)(userInfo.delegato)).idPeople;

                    // Trasmissione
                    transmission = BusinessLogic.Trasmissioni.ExecTrasmManager.saveExecuteTrasmMethod(
                        serverPath, transmission);
                    string desc = string.Empty;
                    if (transmission != null)
                    {
                        // LOG per documento
                        if (transmission.infoDocumento != null && !string.IsNullOrEmpty(transmission.infoDocumento.idProfile))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in transmission.trasmissioniSingole)
                            {
                                string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                if (transmission.infoDocumento.segnatura == null)
                                    desc = "Trasmesso Documento : " + transmission.infoDocumento.docNumber.ToString();
                                else
                                    desc = "Trasmesso Documento : " + transmission.infoDocumento.segnatura.ToString();

                                BusinessLogic.UserLog.UserLog.WriteLog(transmission.utente.userId, transmission.utente.idPeople, transmission.ruolo.idGruppo, transmission.utente.idAmministrazione, method, transmission.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, userInfo.delegato, generaNotifica, single.systemId);
                            }
                        }
                        // LOG per fascicolo
                        if (transmission.infoFascicolo != null && !string.IsNullOrEmpty(transmission.infoFascicolo.idFascicolo))
                        {
                            foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in transmission.trasmissioniSingole)
                            {
                                string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                                desc = "Trasmesso Fascicolo ID: " + transmission.infoFascicolo.idFascicolo.ToString();
                                BusinessLogic.UserLog.UserLog.WriteLog(transmission.utente.userId, transmission.utente.idPeople, transmission.ruolo.idGruppo, transmission.utente.idAmministrazione, method, transmission.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, userInfo.delegato, generaNotifica, single.systemId);
                            }
                        }

                    }
                    // Il risultato da restituire è successo
                    toReturn = true;
                }
            }
            catch (Exception e)
            {
                // Il risulto è insuccesso
                toReturn = false;

            }

            // Impostazione del modello di trasmissione
            trasmModel = model;

            // Restituzione dell'esito
            return toReturn;

        }

        public static DocsPaVO.trasmissione.Trasmissione addTrasmissioneSingolaSoloConNotifica(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragione, string note, string tipoTrasm, DocsPaVO.Modelli_Trasmissioni.MittDest destinatari)
        {
            //Controllo se il ruolo è disabilitato alla ricezione delle trasmissioni
            if (corr != null && corr.tipoCorrispondente == "R")
            {
                DocsPaDB.Query_DocsPAWS.Utenti utentiDb = new DocsPaDB.Query_DocsPAWS.Utenti();
                DocsPaVO.utente.Corrispondente corrispondente = utentiDb.getRuoloById(corr.systemId);
                if (corrispondente != null && corrispondente.disabledTrasm)
                    return trasmissione;
            }

            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Count; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts.corrispondenteInterno.systemId != null && ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPaVO.utente.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                //DocsPaVO.utente.Corrispondente[] listaUtenti = queryUtenti(corr);
               // ArrayList listaUtenti = queryUtenti(corr);
                //if (listaUtenti.Length == 0)
                if (destinatari.UTENTI_NOTIFICA.Count == 0)
                    trasmissioneSingola = null;

                //ciclo per utenti se dest è gruppo o ruolo
                //for (int i = 0; i < listaUtenti.Length; i++)
               // DocsPaVO.Modelli_Trasmissioni.MittDest mittDest in ragioneDest.DESTINATARI
                for (int i = 0; i < destinatari.UTENTI_NOTIFICA.Count; i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm utTemp = new DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm();
                    utTemp = (DocsPaVO.Modelli_Trasmissioni.UtentiConNotificaTrasm)destinatari.UTENTI_NOTIFICA[i];
                    trasmissioneUtente.utente = Utenti.UserManager.getUtenteById(utTemp.ID_PEOPLE);
                    //trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                    trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
                }
            }

            if (corr is DocsPaVO.utente.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;
                //trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                trasmissioneSingola.trasmissioneUtente.Add(trasmissioneUtente);
            }

            if (corr is DocsPaVO.utente.UnitaOrganizzativa)
            {
                DocsPaVO.utente.UnitaOrganizzativa theUo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                //qca.ruolo = UserManager.getRuolo();
                qca.ruolo = trasmissione.ruolo;

                //DocsPaVO.utente.Ruolo[] ruoli = Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo);
                ArrayList ruoli = Utenti.addressBookManager.getRuoliRiferimentoAutorizzati(qca, theUo);
                foreach (DocsPaVO.utente.Ruolo r in ruoli)
                    trasmissione = addTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm);

                return trasmissione;
            }

            if (trasmissioneSingola != null)
                //trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
                trasmissione.trasmissioniSingole.Add(trasmissioneSingola);
            return trasmissione;
        }

        public static bool TrasmissioneExecuteTrasmFascDaModelloSoloConNotifica(string serverPath, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modello, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = new DocsPaVO.trasmissione.Trasmissione();

            //Parametri della trasmissione
            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;

            trasmissione.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
            //trasmissione.infoDocumento = DocumentManager.getInfoDocumento(scheda);
            DocsPaVO.fascicolazione.Fascicolo fasc = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(fascicolo.systemID, infoUtente);
            trasmissione.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(fasc);

            trasmissione.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUtente.idPeople);
            trasmissione.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUtente.idCorrGlobali);

            trasmissione.NO_NOTIFY = modello.NO_NOTIFY;

            string generaNotifica = "1";

            if (!string.IsNullOrEmpty(modello.NO_NOTIFY) && modello.NO_NOTIFY.Equals("1"))
            {
                generaNotifica = "0";
            }

            //Parametri delle trasmissioni singole
            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Count; i++)
            {
                DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                for (int j = 0; j < destinatari.Count; j++)
                {
                    DocsPaVO.Modelli_Trasmissioni.MittDest mittDest = (DocsPaVO.Modelli_Trasmissioni.MittDest)destinatari[j];

                    //DocsPaVO.utente.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this,mittDest.VAR_COD_RUBRICA,DocsPaWR.AddressbookTipoUtente.INTERNO);
                    DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, infoUtente);

                    //DocsPaVO.trasmissione.RagioneTrasmissione ragione = wws.getRagioneById(mittDest.ID_RAGIONE.ToString());?
                    DocsPaVO.trasmissione.RagioneTrasmissione ragione = BusinessLogic.Trasmissioni.QueryTrasmManager.getRagioneById(mittDest.ID_RAGIONE.ToString());
                    trasmissione = TrasmManager.addTrasmissioneSingolaSoloConNotifica(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest);
                }
            }
            if (infoUtente.delegato != null)
                trasmissione.delegato = ((DocsPaVO.utente.InfoUtente)(infoUtente.delegato)).idPeople;

            //trasmissione = BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasmissione);
            //BusinessLogic.Trasmissioni.ExecTrasmManager.executeTrasmMethod(
            trasmissione = ExecTrasmManager.saveExecuteTrasmMethod(serverPath, trasmissione);
            string desc = string.Empty;
            if (trasmissione != null)
            {
                // LOG per documento
                if (trasmissione.infoDocumento != null && !string.IsNullOrEmpty(trasmissione.infoDocumento.idProfile))
                {
                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasmissione.trasmissioniSingole)
                    {
                        string method = "TRASM_DOC_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                        if (trasmissione.infoDocumento.segnatura == null)
                            desc = "Trasmesso Documento : " + trasmissione.infoDocumento.docNumber.ToString();
                        else
                            desc = "Trasmesso Documento : " + trasmissione.infoDocumento.segnatura.ToString();

                        BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, method, trasmissione.infoDocumento.docNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                    }
                }
                // LOG per fascicolo
                if (trasmissione.infoFascicolo != null && !string.IsNullOrEmpty(trasmissione.infoFascicolo.idFascicolo))
                {
                    foreach (DocsPaVO.trasmissione.TrasmissioneSingola single in trasmissione.trasmissioniSingole)
                    {
                        string method = "TRASM_FOLDER_" + single.ragione.descrizione.ToUpper().Replace(" ", "_");
                        desc = "Trasmesso Fascicolo ID: " + trasmissione.infoFascicolo.idFascicolo.ToString();
                        BusinessLogic.UserLog.UserLog.WriteLog(trasmissione.utente.userId, trasmissione.utente.idPeople, trasmissione.ruolo.idGruppo, trasmissione.utente.idAmministrazione, method, trasmissione.infoFascicolo.idFascicolo, desc, DocsPaVO.Logger.CodAzione.Esito.OK, infoUtente.delegato, generaNotifica, single.systemId);
                    }
                }

            }
            return true;

        }

        public static bool DocumentAlreadyTransmitted_Opt(string idDocument)
        {
            bool retval = false;
            DocsPaDB.Query_DocsPAWS.Trasmissione dbInt = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            retval = dbInt.DocumentAlreadyTransmitted_Opt(idDocument);
            return retval;
        }

        public static string getTipoRagioneDaTramSingola(string idTrasmSingola)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione dbInt = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return dbInt.GetTipoRagioneByIdTrasmSingola(idTrasmSingola);
        }

        /// <summary>
        /// Metodo per il recupero del tipo associato all'ultima trasmissione effettuata per un dato documento
        /// </summary>
        /// <param name="documentId">Id del documento</param>
        /// <returns>S se la trasmissione è di tipo Singola; T se è di tipo Tutti</returns>
        public static char GetLastTrasmType(String documentId)
        {
            using (DocsPaDB.Query_DocsPAWS.Trasmissione trasmDb = new DocsPaDB.Query_DocsPAWS.Trasmissione())
            {
                return trasmDb.GetLastTrasmTypeForDocument(documentId);
            }
        }

    }
}
