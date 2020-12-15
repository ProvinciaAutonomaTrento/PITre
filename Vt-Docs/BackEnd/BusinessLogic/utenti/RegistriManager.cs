using System;
using System.Data;
using System.Collections;
using System.Globalization;
using log4net;
using DocsPaVO.utente;

namespace BusinessLogic.Utenti
{
	/// <summary>
	/// Summary description for RegistriManager.
	/// </summary>
	public class RegistriManager 
	{
        private static ILog logger = LogManager.GetLogger(typeof(RegistriManager));
		/// <summary>
		/// </summary>
		/// <param name="idRegistro"></param>
		/// <returns></returns>
		public static DocsPaVO.utente.Registro getRegistro(string idRegistro) 
		{
			DocsPaVO.utente.Registro reg = null;

			if(!(idRegistro != null && !idRegistro.Equals("")))
			{
				return reg;
			}

			DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
			utenti.GetRegistro(idRegistro, ref reg);

			#region Codice Commentato
			/*string queryString = 
				"SELECT A.SYSTEM_ID, A.VAR_CODICE, A.NUM_RIF, A.VAR_DESC_REGISTRO, " +
				"A.VAR_EMAIL_REGISTRO, A.CHA_STATO, A.ID_AMM, " +
				DocsPaWS.Utils.dbControl.toChar("A.DTA_OPEN",false) + " AS DTA_OPEN, " +
				DocsPaWS.Utils.dbControl.toChar("A.DTA_CLOSE",false) + " AS DTA_CLOSE, " +
				DocsPaWS.Utils.dbControl.toChar("A.DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO " +
				"FROM DPA_EL_REGISTRI A WHERE A.SYSTEM_ID=" + idRegistro;
			logger.Debug(queryString);
			
			IDataReader dr = db.executeReader(queryString);
															   
			if(dr.Read()){
				reg = new DocsPaVO.utente.Registro();

				reg.systemId = dr.GetValue(0).ToString();
				reg.codRegistro = dr.GetValue(1).ToString();
				reg.codice = dr.GetValue(2).ToString();
				reg.descrizione = dr.GetValue(3).ToString();
				reg.email = dr.GetValue(4).ToString();
				reg.stato = dr.GetValue(5).ToString();				
				reg.idAmministrazione = dr.GetValue(6).ToString();
				reg.codAmministrazione = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
				reg.dataApertura = dr.GetValue(7).ToString();
				reg.dataChiusura = dr.GetValue(8).ToString();
				reg.dataUltimoProtocollo = dr.GetValue(9).ToString();
			}
			dr.Close();*/
			#endregion

			return reg;
		}
        public static DocsPaVO.utente.Registro getRegistroByCodAOO(string codAOO, string idAmministrazione) 
		{
			DocsPaVO.utente.Registro reg = null;
			if(!(codAOO != null && !codAOO.Equals("")))
			{
				return reg;
			}
			DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            utenti.GetRegistroByCodAOO(codAOO, idAmministrazione, ref reg);
					

			return reg;
		}

        /// <summary>
        /// Ritorna uno dei tre possibili stati del registro 
        /// (V - aperto, R - chiuso, G - giallo)
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        public static string getStatoRegistro(DocsPaVO.utente.Registro registro)
        {
            // R = Rosso -  CHIUSO
            // V = Verde -  APERTO
            // G = Giallo - APERTO IN GIALLO

            string dataApertura = registro.dataApertura;

            if (!dataApertura.Equals(""))
            {

                DateTime dt_cor = DateTime.Now;

                CultureInfo ci = new CultureInfo("it-IT");

                string[] formati ={ "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };

                DateTime d_ap = DateTime.ParseExact(dataApertura, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                //aggiungo un giorno per fare il confronto con now (che comprende anche minuti e secondi)
                d_ap = d_ap.AddDays(1);

                string mydate = dt_cor.ToString(ci);

                //DateTime dt = DateTime.ParseExact(mydate,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);

                
                if (registro.stato.Equals("A"))
                {
                    if (dt_cor.CompareTo(d_ap) > 0)
                    {
                        //data odierna maggiore della data di apertura del registro
                        return "G";
                    }
                    else
                        return "V";
                }
            }
            return "R";

        }
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="idRuolo"></param>
		/// <returns></returns>
		public static ArrayList getRegistri(string idRuolo)
		{
			ArrayList registri = null;

			try 
			{
				registri = getRegistriRuolo(idRuolo);				
			} 
			catch (Exception e) 
			{	
				logger.Debug("Errore nella gestione degli utenti (getRegistri)",e);
				throw new Exception("F_System");
			}

			return registri;
		}

        public static ArrayList getRegistriNoFiltroAOO(string idAmm)
        {
            //TODO: DOMANI
            ArrayList registri = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                registri = utenti.getRegistriNoFiltroAOO(idAmm);	
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione degli utenti (getRegistriNoFiltroAOO)", e);
                throw new Exception("F_System");
            }

            return registri;
        }

		public static Hashtable GetRegistriByRuolo (string id_amm)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
			return amm.GetRegistriByRuolo (id_amm);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idRuolo"></param>
		/// <returns></returns>
		public static ArrayList getRegistriRuolo (string idRuolo) 
		{
			DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

			return utenti.GetRegistriRuolo(idRuolo);		

			#region Codice Commentato
			/*logger.Debug("getRegistri");
			ArrayList registri = new ArrayList();
			//ricerca dei registri associati al ruolo
			string queryString = 
				"SELECT A.SYSTEM_ID, A.VAR_CODICE, A.NUM_RIF, A.VAR_DESC_REGISTRO, " +
				"A.VAR_EMAIL_REGISTRO, A.CHA_STATO, A.ID_AMM, " +
				DocsPaWS.Utils.dbControl.toChar("A.DTA_OPEN",false) + " AS DTA_OPEN, " +
				DocsPaWS.Utils.dbControl.toChar("A.DTA_CLOSE",false) + " AS DTA_CLOSE, " +
				DocsPaWS.Utils.dbControl.toChar("A.DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO " +
				"FROM DPA_L_RUOLO_REG B, DPA_EL_REGISTRI A " +
				"WHERE A.SYSTEM_ID=B.ID_REGISTRO AND ID_RUOLO_IN_UO=" + idRuolo +
				" ORDER BY B.CHA_PREFERITO DESC, A.VAR_DESC_REGISTRO";
			logger.Debug(queryString);
			IDataReader dr = db.executeReader(queryString);
			while(dr.Read()){
				DocsPaVO.utente.Registro reg=new DocsPaVO.utente.Registro();

				reg.systemId = dr.GetValue(0).ToString();
				reg.codRegistro = dr.GetValue(1).ToString();
				reg.codice = dr.GetValue(2).ToString();
				reg.descrizione = dr.GetValue(3).ToString();
				reg.email = dr.GetValue(4).ToString();
				reg.stato = dr.GetValue(5).ToString();				
				reg.idAmministrazione = dr.GetValue(6).ToString();
				reg.codAmministrazione = DocsPaWS.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
				reg.dataApertura = dr.GetValue(7).ToString();
				reg.dataChiusura = dr.GetValue(8).ToString();
				reg.dataUltimoProtocollo = dr.GetValue(9).ToString();
				registri.Add(reg);
			}
			dr.Close();

			// leggo il numero di protocollo solo se lo stato del registro è chiuso
			for (int i=0; i < registri.Count; i++) {
				DocsPaVO.utente.Registro reg = (DocsPaVO.utente.Registro)registri[i];
				if(reg.stato.Equals("C")) {
					if(reg.dataUltimoProtocollo.Substring(6,4).Equals(DocsPaWS.Utils.DateControl.getDate(false).Substring(6,4))) {
						queryString = 
							"SELECT NUM_RIF FROM DPA_REG_PROTO WHERE ID_REGISTRO = " + reg.systemId;
						logger.Debug(queryString);
						reg.ultimoNumeroProtocollo = db.executeScalar(queryString).ToString();
						reg.ultimoNumeroProtocollo=obj.getUltimoNumProto(reg.systemId);
					} else
						reg.ultimoNumeroProtocollo = "1";
					registri[i] = reg;
				}
			}
			return registri;*/
			#endregion
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registro"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static DocsPaVO.utente.Registro cambiaStato(string idPeople, string idCorrGlobali, DocsPaVO.utente.Registro registro) 
		{
			#region Codice Commentato
			/*logger.Debug("cambiaStato");
			string updateString = 
				"UPDATE DPA_EL_REGISTRI SET ";

			if(registro.stato.Equals("A")) {
				registro.stato = "C";
				registro.dataChiusura = DocsPaWS.Utils.DateControl.getDate(false);	
				updateString += " DTA_CLOSE = " + DocsPaWS.Utils.dbControl.getDate();
			}
			else {
				registro.stato = "A";
				registro.dataApertura = DocsPaWS.Utils.DateControl.getDate(false);
				registro.dataChiusura = ""; 
				updateString += " DTA_CLOSE = null, DTA_OPEN = " + DocsPaWS.Utils.dbControl.getDate();
			}
			updateString += 
				", CHA_STATO = '" + registro.stato + "'" +
				" WHERE SYSTEM_ID = " + registro.systemId;
			DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			try {
				db.openConnection();	
				if (!registro.stato.Equals("C")) {					
					string sqlString = 
						"UPDATE DPA_REG_PROTO SET NUM_RIF=1 " +
						"WHERE ID_REGISTRO = (SELECT SYSTEM_ID FROM DPA_EL_REGISTRI " +
						"WHERE SYSTEM_ID= " + registro.systemId + " AND " + DocsPaWS.Utils.dbControl.getYear(DocsPaWS.Utils.dbControl.getDate()) + "!=" + DocsPaWS.Utils.dbControl.getYear("DTA_OPEN") + ")";									
					logger.Debug(sqlString);
					db.executeNonQuery(sqlString);
				}									
				logger.Debug(updateString);
				db.executeNonQuery(updateString);	
				if (registro.stato.Equals("C")) {
					registro.ultimoNumeroProtocollo = db.executeScalar("SELECT NUM_RIF FROM DPA_REG_PROTO WHERE ID_REGISTRO = " + registro.systemId).ToString();
					aggiornaStorico(db, registro, infoUtente);
				} 
				db.closeConnection();
			} catch (Exception e) {
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}*/
			//DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			#endregion

			DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
			logger.Debug("CALL : RegistroCambiaStato");
			utenti.RegistroCambiaStato(idPeople, idCorrGlobali, ref registro);

			logger.Debug("END : DocsPAWS > Utenti > RegistriManager > cambiaStato");
			return registro;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registro"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static DocsPaVO.utente.Registro modificaRegistro(DocsPaVO.utente.Registro registro, DocsPaVO.utente.InfoUtente infoUtente) 
		{
			#region Codice Commentato
			/*logger.Debug("modificaRegistro");
			if(registro.stato.Equals("A"))
				throw new Exception("Il registro non è chiuso");
			DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			try {
				db.openConnection();
				string updateString = 
					"UPDATE DPA_EL_REGISTRI SET " +
					"VAR_EMAIL_REGISTRO = '" + registro.email + "'" +
					", VAR_DESC_REGISTRO = '" + registro.descrizione + "'" +
					" WHERE SYSTEM_ID = " + registro.systemId;
				logger.Debug(updateString);
				db.executeNonQuery(updateString);
				updateString = 
					"UPDATE DPA_REG_PROTO SET " +
					"NUM_RIF = " + registro.ultimoNumeroProtocollo + 
					" WHERE ID_REGISTRO = " + registro.systemId;
				logger.Debug(updateString);
				db.executeNonQuery(updateString);			
				db.closeConnection();

			} catch (Exception e) {
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}*/
			//DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			#endregion

			DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
			utenti.RegistroModifica(ref registro,infoUtente);
			return registro;
		}

		#region Metodo Commentato
		/*private static void aggiornaStorico(DocsPaWS.Utils.Database db, DocsPaVO.utente.Registro registro, DocsPaVO.utente.InfoUtente infoUtente) {
			string insertString =
				"INSERT INTO DPA_REGISTRO_STO " +
				"(" + DocsPaWS.Utils.dbControl.getSystemIdColName() + " ID_REGISTRO, DTA_OPEN, DTA_CLOSE, NUM_RIF, ID_PEOPLE, ID_RUOLO_IN_UO) " +
				"SELECT " + DocsPaWS.Utils.dbControl.getSystemIdNextVal("DPA_REGISTRO_STO") + 
				"SYSTEM_ID, DTA_OPEN, DTA_CLOSE, " + registro.ultimoNumeroProtocollo + ", " + infoUtente.idPeople + "," + infoUtente.idCorrGlobali + 
				" FROM DPA_EL_REGISTRI WHERE SYSTEM_ID = " + registro.systemId;
			logger.Debug(insertString);
			db.executeNonQuery(insertString);			
		}*/
		#endregion

        public static string getIdRegistro(string codiceAmm, string codiceReg)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                string idAmm = model.getIdAmmByCod(codiceAmm);
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAElRegistri");
                q.setParam("param1", " SYSTEM_ID ");
                q.setParam("param2", " ID_AMM = " + idAmm + " AND upper(VAR_CODICE) = '" + codiceReg.ToUpper() + "'");
                string sql = q.getSQL();
                logger.Debug(sql);
                DataSet ds = new DataSet();
                dbProvider.ExecuteQuery(ds, sql);
                if (ds.Tables[0].Rows.Count != 0)
                    return ds.Tables[0].Rows[0]["SYSTEM_ID"].ToString();
                else
                    return string.Empty;

            }
            catch (Exception ex)
            {
                logger.Debug("Errore durante getIdRegistro.", ex);
                return string.Empty;
            }
        }

        public static string getIdRegistroIS(string codiceAmm, string codiceReg)
        {
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAElRegistri_IS");
                    q.setParam("param1", " R.SYSTEM_ID ");
                    q.setParam("codiceAmministrazione", codiceAmm);
                    q.setParam("codiceRegistro", codiceReg);
                    string sql = q.getSQL();
                    logger.Debug(sql);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, "REGISTRO", sql);
                    if (ds.Tables["REGISTRO"].Rows.Count != 0)
                        return ds.Tables["REGISTRO"].Rows[0]["SYSTEM_ID"].ToString();
                    else
                        return string.Empty;

                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore durante getIdRegistroIS.", ex);
                return string.Empty;
            }
        }

        public static ArrayList getListaRegistriRfRuolo(string idRuolo, string all, string idAooColl)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utenti.GetListaRegistriRfRuolo(idRuolo, all, idAooColl);

        }

        public static DocsPaVO.utente.Registro GetRegistroDaPec(string idProfile)
        {
            DocsPaVO.utente.Registro reg = null;

            if (!string.IsNullOrEmpty(idProfile))
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                utenti.GetRegistroDaPec(idProfile, ref reg);
            }

            return reg;
        }

        public static bool enabledRF(string idAmministazione)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["ENABLE_RF"] != null &&
             System.Configuration.ConfigurationManager.AppSettings["ENABLE_RF"] != "0")
                return true;
            else return false;

        }

        public static bool existRf(string idAmministazione)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    bool result = amm.existRf(idAmministazione);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in RegistriManager  - metodo: existRf", e);
                    return false;
                }
            }
        }

        #region Salvataggio e Reperimento dettagli RF (per l'invio a Rubrica Comune)

        /// <summary>
        /// Metodo per il salvataggio del dettaglio di un Raggruppamento Funzionale nella rubrica
        /// locale. 
        /// </summary>
        /// <param name="rf">Dettagli da salvare</param>
        /// <param name="idRf">Id dell'RF</param>
        public static bool SaveRaggruppamentoFunzionaleRC(RaggruppamentoFunzionale rf, String idRf)
        {
            bool retVal = false;

            using (DocsPaDB.Query_DocsPAWS.RF rfDb = new DocsPaDB.Query_DocsPAWS.RF())
            {
                retVal = rfDb.SaveRaggruppamentoFunzionaleCorrGlobali(rf, idRf);
            }


            return retVal;
        }

        /// <summary>
        /// Metodo per il reperimento dei dettaglio di un Raggruppamento Funzionale. 
        /// </summary>
        /// <param name="idRf">Id dell'RF da caricare</param>
        /// <returns>Dettaglio del raggruppamento funzionale</returns>
        public static RaggruppamentoFunzionale GetRaggruppamentoFunzionaleRC(String idRf)
        {
            RaggruppamentoFunzionale rf = new RaggruppamentoFunzionale();

            using (DocsPaDB.Query_DocsPAWS.RF rfDb = new DocsPaDB.Query_DocsPAWS.RF())
            {
                rf = rfDb.GetRaggruppamentoFunzionaleRC(idRf);
            }

            return rf;
 
        }

        #endregion

        public static String GetSystemIdRFDaDPA_EL_REGISTRI(string codiceRF)
        {
            using (DocsPaDB.Query_DocsPAWS.RF rfDb = new DocsPaDB.Query_DocsPAWS.RF())
            {
                return rfDb.getSystemIdRFDaDPA_EL_REGISTRI(codiceRF);

            }
        }

        public static DocsPaVO.utente.Registro getRegistroByCode(string codiceRegistro)
        {
            DocsPaVO.utente.Registro reg = null;

            if (!string.IsNullOrEmpty(codiceRegistro))
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                utenti.GetRegistroByCodice(codiceRegistro, ref reg);
            }

            return reg;
        }
    }
}
