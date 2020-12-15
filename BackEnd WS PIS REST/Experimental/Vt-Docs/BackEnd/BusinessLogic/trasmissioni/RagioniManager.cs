using System;
using System.Data;
using log4net;


namespace BusinessLogic.Trasmissioni
{
	/// <summary>
	/// </summary>
	public class RagioniManager	
	{
        private static ILog logger = LogManager.GetLogger(typeof(RagioniManager));
		/// <summary>
		/// Prende la ragione di trasmissione per il protocollo interno
		/// tramite la system_id dell'amministrazione ed il tipo destinatario
		/// </summary>
		/// <param name="tipoDest">tipo del destinatario (TO o CC)</param>
		/// <param name="idAmm">system_id dell'amministrazione</param>
		/// <returns>Object Ragione</returns>
		public static DocsPaVO.trasmissione.RagioneTrasmissione GetRagione(string tipoDest, string idAmm) 
		{
			logger.Debug("GetRagione");
			DocsPaVO.trasmissione.RagioneTrasmissione objRagione = new DocsPaVO.trasmissione.RagioneTrasmissione();
			string queryString = getQueryRagione(tipoDest,idAmm);
			logger.Debug(queryString);

			DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			strDB.GetRag(queryString, ref objRagione);

			return objRagione;
		}

        public static DocsPaVO.trasmissione.RagioneTrasmissione getRagioneNotifica(string idAmm)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
                DocsPaVO.trasmissione.RagioneTrasmissione objRagione = new DocsPaVO.trasmissione.RagioneTrasmissione();

                strDB.getRagioneNotifica(idAmm, ref objRagione);
                return objRagione;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


		/// <summary>
		/// Prende la ragione di trasmissione tramite la system_id
		/// </summary>
		/// <param name="idRagione">system_id della ragione</param>
		/// <returns></returns>
		public static DocsPaVO.trasmissione.RagioneTrasmissione getRagione(string idRagione) 
		{
			logger.Debug("getRagione");
			DocsPaVO.trasmissione.RagioneTrasmissione objRagione = new DocsPaVO.trasmissione.RagioneTrasmissione();
			string queryString = getQueryRagione(idRagione);
			logger.Debug(queryString);

			DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			strDB.GetRag(queryString, ref objRagione);

			return objRagione;
		}


        public static DocsPaVO.trasmissione.RagioneTrasmissione getRagioneByCodice(string idAmm, string codice)
        {
            logger.Debug("getRagione");
            DocsPaVO.trasmissione.RagioneTrasmissione objRagione = new DocsPaVO.trasmissione.RagioneTrasmissione();
            string queryString = getQueryRagioneByCodice(idAmm,codice);
            logger.Debug(queryString);

            DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            strDB.GetRag(queryString, ref objRagione);

            return objRagione;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objDiritti"></param>
		/// <returns></returns>
		public static System.Collections.ArrayList getListaRagioniATutti(DocsPaVO.trasmissione.Diritti objDiritti) 
		{

			DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			return strDB.GetListRagATutti(objDiritti);

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objDiritti"></param>
		/// <returns></returns>
		public static System.Collections.ArrayList getListaRagioni(DocsPaVO.trasmissione.Diritti objDiritti,bool flgDaRicercaTrasm, bool sysExt = false) 
		{
			#region Codice Commentato
			/*
			System.Collections.ArrayList objListaRagioni = new System.Collections.ArrayList();
			DocsPa_V15_Utils.database.SqlServerAgent db = new DocsPa_V15_Utils.database.SqlServerAgent();
			db.openConnection();

			try 
			{ 
				string queryString = getQueryRagione(null);
				queryString += " AND (ID_AMM IS NULL ";
				if(objDiritti != null)
					queryString += " OR ID_AMM = " + objDiritti.idAmministrazione;
				queryString += ")";
				
				try 
				{
					if (objDiritti != null && objDiritti.accessRights != null && objDiritti.accessRights.Equals("45"))
					{
						queryString += " AND CHA_TIPO_DIRITTI IN ('R', 'C') ";
					}
				} 
				catch (Exception) {}

				logger.Debug(queryString);
				IDataReader dr = db.executeReader(queryString); 
				while (dr.Read()) 
				{					
					objListaRagioni.Add(getDatiRagione(dr));
				}
				dr.Close();
			} catch (Exception e) 
			{
				logger.Debug (e.Message);
				db.closeConnection();
				throw new Exception(e.Message);
			}
			// chiudo le connessioni
			db.closeConnection();
			return objListaRagioni;
			*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Trasmissione strDB = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			return strDB.GetListRag(objDiritti,flgDaRicercaTrasm,sysExt);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idRagione"></param>
		/// <returns></returns>
		private static string getQueryRagione(string idRagione) 
		{
			#region Codice Commentato
			/*string queryString =
				"SELECT SYSTEM_ID, VAR_DESC_RAGIONE, CHA_TIPO_RAGIONE, " +
				"CHA_TIPO_DIRITTI, CHA_RISPOSTA, CHA_TIPO_DEST, VAR_NOTE, CHA_EREDITA, CHA_TIPO_RISPOSTA FROM DPA_RAGIONE_TRASM WHERE ";
			if (idRagione != null)
				queryString += " SYSTEM_ID=" + idRagione;
			else
				queryString += " DPA_RAGIONE_TRASM.CHA_VIS = '1'";
			return queryString;*/
			#endregion 

			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			return trasmissione.GetQueryRagione(idRagione);

		}

        private static string getQueryRagioneByCodice(string idAmm, string codice)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasmissione.GetQueryRagioneByCodice(idAmm, codice);

        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="descRagione"></param>
		/// <param name="idAmm"></param>
		/// <returns></returns>
		private static string getQueryRagione(string tipoDest, string idAmm) 
		{
			DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			return trasmissione.GetQueryRagione(tipoDest,idAmm);
		}

        /// <summary>
        /// Estrae la ragione di trasmissione in base al tipo di operazione
        /// </summary>
        /// <param name="tipoOperazione"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static DocsPaVO.trasmissione.RagioneTrasmissione GetRagioneByTipoOperazione(string tipoOperazione, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Trasmissione trasmissione = new DocsPaDB.Query_DocsPAWS.Trasmissione();
            return trasmissione.GetRagioneByTipoOperazione(tipoOperazione, idAmm);
        }
		
		#region Metodo Commentato
		/*
		private static DocsPaVO.trasmissione.RagioneTrasmissione getDatiRagione(IDataReader dr) {
			DocsPaVO.trasmissione.RagioneTrasmissione objRagione = new DocsPaVO.trasmissione.RagioneTrasmissione();
					
			objRagione.systemId = dr.GetValue(0).ToString();
			objRagione.descrizione = dr.GetValue(1).ToString();
			objRagione.tipo = dr.GetValue(2).ToString();
			objRagione.tipoDiritti = (DocsPaVO.trasmissione.TipoDiritto) DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoDirittoStringa,dr.GetValue(3).ToString());
			objRagione.risposta = dr.GetValue(4).ToString();
			objRagione.tipoDestinatario = (DocsPaVO.trasmissione.TipoGerarchia) DocsPaDB.Utils.HashTableManager.GetKeyFromValue(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa, dr.GetValue(5).ToString());
			objRagione.note = dr.GetValue(6).ToString();
			objRagione.eredita = dr.GetValue(7).ToString();
			if(dr.GetValue(8) != null)
				objRagione.tipoRisposta = dr.GetValue(8).ToString();

			return objRagione;
		}
		*/
		#endregion
	}
}
