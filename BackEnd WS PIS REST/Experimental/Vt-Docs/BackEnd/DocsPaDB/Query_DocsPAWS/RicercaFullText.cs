using System;
using System.Collections;
using System.Data;
using DocsPaVO.Logger;
using DocsPaVO.utente;
using DocsPaUtils.Functions;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
	/// <summary>
	/// Classe per la gestione dell'accesso ai dati relativamente
	/// alla ricerca full text su hummingbird
	/// </summary>
	public class RicercaFullText : DBProvider
	{
        private ILog logger = LogManager.GetLogger(typeof(RicercaFullText));
		#region Costruttori

		public RicercaFullText()
		{
		}

		#endregion

		public string GetLibreria(string idAmministrazione)
		{
			string cmd="SELECT VAR_LIBRERIA FROM DPA_AMMINISTRA WHERE SYSTEM_ID=" + idAmministrazione;
			string outParam;
			ExecuteScalar(out outParam,cmd);
			return outParam;
		}

		public ArrayList GetDocumenti(string[] resultKeys,InfoUtente infoUtente)
		{
			logger.Debug("GetDocumenti");
			
			string resultQuery=this.GetSearchResultFilterCriteria(resultKeys);

			ArrayList result = new ArrayList();

			string querySql="SELECT DISTINCT " +
									"P.SYSTEM_ID, " +
									"P.DOCNUMBER, " +
									"P.NUM_PROTO, " +
									"P.VAR_SEGNATURA, " +
									DocsPaDbManagement.Functions.Functions.ToChar("P.CREATION_DATE",false) + " AS DATA, " +
									"P.CHA_TIPO_PROTO As A_P, " +
									"P.VAR_PROF_OGGETTO AS OGGETTO, " + 
									"P.ID_REGISTRO AS ID_REGISTRO, " +
                                    "P.CHA_IMG AS CHA_IMG " +
							"FROM	PROFILE P, SECURITY S " + //, DPA_EL_REGISTRI R " +
							"WHERE	P.SYSTEM_ID IN (" + resultQuery + ")  " +
									//"AND (S.ACCESSRIGHTS > 0   " +
									//"AND (S.PERSONORGROUP = " + infoUtente.idPeople + " OR S.PERSONORGROUP=" + infoUtente.idGruppo + "))  " +
									"AND S.THING = P.SYSTEM_ID " + 
									//"AND (R.SYSTEM_ID=P.ID_REGISTRO OR P.ID_REGISTRO IS NULL) " +
									//"AND P.CHA_TIPO_PROTO!='R' " +
							"ORDER BY P.DOCNUMBER DESC";

			logger.Debug(querySql);

			DataSet ds = new DataSet();

			this.ExecuteQuery(out ds,"PROFILE",querySql);

			// recupero informazioni documenti
			foreach (DataRow dr in ds.Tables[0].Rows)
			{
				DocsPaVO.documento.InfoDocumento tmp = this.GetInfoDocumento(dr);
				result.Add(tmp);
				tmp=null;
			}
			
			ds = null;

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="resultKeys"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public string[] GetIDDocumentiETDOCS(string[] resultKeys,InfoUtente infoUtente)
		{
			ArrayList retValue=new ArrayList();

			logger.Debug("GetCountDocumentiETDOCS");

			string resultQuery=this.GetSearchResultFilterCriteriaETDOCS(resultKeys);

			ArrayList result = new ArrayList();

			string commandText="SELECT DISTINCT " +
									"P.SYSTEM_ID, " +				
									"P.DOCNUMBER " +
								"FROM	PROFILE P,COMPONENTS C,SECURITY S " + 
								"WHERE	C.VERSION_ID IN (" + resultQuery + ")  " +
										"AND S.THING = P.SYSTEM_ID AND C.DOCNUMBER=P.DOCNUMBER " + 
								"ORDER BY P.DOCNUMBER DESC";
			
			logger.Debug(commandText);

			DataSet dsp = new DataSet();
			
			using (IDataReader reader=this.ExecuteReader(commandText))
			{
				while (reader.Read())
				{
					retValue.Add(reader.GetValue(reader.GetOrdinal("SYSTEM_ID")).ToString());
				}
			}
				
			return (string[]) retValue.ToArray(typeof(string));
		}

		public ArrayList GetDocumentiETDOCS_OLD(string[] resultKeys,InfoUtente infoUtente)
		{
			logger.Debug("GetDocumentiETDOCS");
			
			string resultQuery=this.GetSearchResultFilterCriteriaETDOCS(resultKeys);

			ArrayList result = new ArrayList();

			string querySql="SELECT DISTINCT " +
						"P.SYSTEM_ID, " +
						"P.DOCNUMBER, " +
						"P.NUM_PROTO, " +
						"P.VAR_SEGNATURA, " +
						DocsPaDbManagement.Functions.Functions.ToChar("P.CREATION_DATE",false) + " AS DATA, " +
						"P.CHA_TIPO_PROTO As A_P, " +
						"P.VAR_PROF_OGGETTO AS OGGETTO, " + 
						"P.ID_REGISTRO AS ID_REGISTRO, " +
                        "P.CHA_IMG AS CHA_IMG, " +
                        "P.ID_DOCUMENTO_PRINCIPALE " +
				"FROM	PROFILE P,SECURITY S " +
                "WHERE	P.SYSTEM_ID IN (" + resultQuery + ")  " +
					"AND S.THING = P.SYSTEM_ID " + 
				"ORDER BY P.DOCNUMBER DESC";

//			//query su DOCSPA
//			string querySql="SELECT DISTINCT " +
//				"P.SYSTEM_ID, " +
//				"P.DOCNUMBER, " +
//				"P.NUM_PROTO, " +
//				"P.VAR_SEGNATURA, " +
//				DocsPaDbManagement.Functions.Functions.ToChar("P.CREATION_DATE",false) + " AS DATA, " +
//				"P.CHA_TIPO_PROTO As A_P, " +
//				"P.VAR_PROF_OGGETTO AS OGGETTO, " + 
//				"P.ID_REGISTRO AS ID_REGISTRO " +
//				"FROM PROFILE P,COMPONENTS C, SECURITY S " + //, DPA_EL_REGISTRI R " +
//				"WHERE C.VERSION_ID IN (" + resultQuery + ")  " +
//					//"AND (S.ACCESSRIGHTS > 0   " +
//					//"AND (S.PERSONORGROUP = " + infoUtente.idPeople + " OR S.PERSONORGROUP=" + infoUtente.idGruppo + "))  " +
//					"AND S.THING = P.SYSTEM_ID AND C.DOCNUMBER=P.DOCNUMBER " + 
//					//"AND (R.SYSTEM_ID=P.ID_REGISTRO OR P.ID_REGISTRO IS NULL) " +
//					//"AND P.CHA_TIPO_PROTO!='R' " +
//				"ORDER BY P.DOCNUMBER DESC";

			logger.Debug(querySql);

			DataSet dsp = new DataSet();

			this.ExecuteQuery(out dsp,"PROFILE",querySql);

			// recupero informazioni documenti
			foreach (DataRow dr in dsp.Tables[0].Rows)
			{
				DocsPaVO.documento.InfoDocumento tmp = this.GetInfoDocumento(dr);
				result.Add(tmp);
				tmp=null;
			}

			return result;
			
			//return new ArrayList();
		}

        public ArrayList GetDocumentiETDOCS(string[] resultKeys, InfoUtente infoUtente)
        {
            logger.Debug("GetDocumentiETDOCS_NO_SECURITY");

            string resultQuery = this.GetSearchResultFilterCriteriaETDOCS(resultKeys);

            ArrayList result = new ArrayList();

            string queryString = "";
            DocsPaUtils.Query q;
            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FULLTEXT_GET_DOCUMENTI");
            q.setParam("param", resultQuery);
            q.setParam("creationDate", DocsPaDbManagement.Functions.Functions.ToChar("P.CREATION_DATE", false));
            q.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
            queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet dsp = new DataSet();

            this.ExecuteQuery(out dsp, "PROFILE", queryString);

            // recupero informazioni documenti
            foreach (DataRow dr in dsp.Tables[0].Rows)
            {
                DocsPaVO.documento.InfoDocumento tmp = this.GetInfoDocumento(dr);
                result.Add(tmp);
                tmp = null;
            }

            return result;
        }

		private ArrayList GetElencoCorrispondenti(string idDocumento)
		{
			string sqlQuery=
						"SELECT DPA_DOC_ARRIVO_PAR.SYSTEM_ID, " +
								"DPA_CORR_GLOBALI.VAR_DESC_CORR " +
						"FROM   DPA_CORR_GLOBALI, DPA_DOC_ARRIVO_PAR " +
						"WHERE  DPA_DOC_ARRIVO_PAR.ID_PROFILE = '" + idDocumento + "' " +
								"AND DPA_DOC_ARRIVO_PAR.ID_MITT_DEST=DPA_CORR_GLOBALI.SYSTEM_ID";

			DataSet ds=new DataSet();
			this.ExecuteQuery(out ds,sqlQuery);

			ArrayList result = new ArrayList();

			foreach (DataRow row in ds.Tables[0].Rows)
				result.Add(row["VAR_DESC_CORR"].ToString());

			ds.Dispose();
			ds=null;

			return result;
		}

		/// <summary>
		/// GetInfoDocumento: metodo che popola il VO per il trasferimento dei dati al frontend
		/// </summary>
		/// <param name="dr">DataRow contenente i dati</param>
		/// <param name="co">CommonObject</param>
		/// <returns>Un VO popolato</returns>
		private DocsPaVO.documento.InfoDocumento GetInfoDocumento(DataRow row)
		{
			//Creiamo l'oggetto che restituiremo
			DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
			
			//Popoliamo l'oggetto
			infoDoc.idProfile = row["SYSTEM_ID"].ToString();
			infoDoc.docNumber=row["DOCNUMBER"].ToString();
			infoDoc.numProt = row["NUM_PROTO"].ToString();
			infoDoc.dataApertura = row["DATA"].ToString();
			infoDoc.tipoProto = row["A_P"].ToString();
            //aggiunto per gestire gli allegati
            if (row["ID_DOCUMENTO_PRINCIPALE"] != DBNull.Value)
                infoDoc.allegato = (Convert.ToInt32(row["ID_DOCUMENTO_PRINCIPALE"]) > 0);

			ArrayList mitt = this.GetElencoCorrispondenti(infoDoc.idProfile);
			infoDoc.mittDest = mitt;
			
			infoDoc.idRegistro = row["ID_REGISTRO"].ToString();
			infoDoc.codRegistro = string.Empty;

//			if (infoDoc.idRegistro.Length>0)
//				infoDoc.codRegistro = row["codRegistro"].ToString();

			infoDoc.oggetto = row["OGGETTO"].ToString();
			infoDoc.segnatura = row["VAR_SEGNATURA"].ToString();

            //nella ricerca FULL_TEXT, il file c'è sempre.
            infoDoc.acquisitaImmagine = row["CHA_IMG"].ToString();
            
			return infoDoc;
		}

		/// <summary>
		/// Creazione della stringa di filtro relativamente
		/// a tutti i systemid estratti dalla ricerca fulltext
		/// </summary>
		/// <param name="resultKeys"></param>
		/// <returns></returns>
		private string GetSearchResultFilterCriteria(string[] resultKeys)
		{
			string retValue="";

			if (resultKeys.Length>0)
			{
				foreach (string resultKey in resultKeys)
				{
					if (retValue!="")
						retValue += ",";
				
					retValue += resultKey;
				}
			}
			else
			{
				retValue="0000";
			}

			return retValue;
		}
		#region QueryETDOCS
/// <summary>
/// 
/// </summary>
/// <param name="resultKeys"></param>
/// <returns></returns>
		private string GetSearchResultFilterCriteriaETDOCS(string[] resultKeys)
		{
			string retValue="";

			if (resultKeys.Length>0)
			{
				foreach (string resultKey in resultKeys)
				{
					if (retValue!="")
						retValue += ",";

                    //retValue += "'" + resultKey + "'";
                    //PERCHè apici SE è sysTEM_ID ?
                    retValue +=  resultKey;
				}
			}
			else
			{
				retValue="0000";
			}

			return retValue;
		}
		#endregion

		#region Query per Filenet

		public string GetSystemIDFromDocnumber(string docnumber)
		{
			string systemid="";

			logger.Debug("GetSystemIDFromDocnumber");
		
			try
			{
				DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("FILENET_S_GET_PROFILE_SYSTEM_ID");

				q.setParam("param1", docnumber);
				string queryString = q.getSQL();
				this.ExecuteScalar(out systemid, queryString);
				return systemid;
			}
			catch(Exception ex)
			{
				logger.Debug (ex.Message);				
				logger.Debug("Errore metodo GetSystemIDFromdocnumber (Query - FILENET_S_GET_PROFILE_SYSTEM_ID)",ex);
				throw new Exception(ex.Message);
			}
		
		}

		public ArrayList GetDocumentiFilenet(string[] resultKeys,InfoUtente infoUtente)
		{
			logger.Debug("GetDocumenti");
			
			string resultQuery=this.GetSearchResultFilterCriteria(resultKeys);

			ArrayList result = new ArrayList();

			string querySql="SELECT DISTINCT " +
				"P.SYSTEM_ID,P.DOCNUMBER, " +
				"P.NUM_PROTO, " +
				"P.VAR_SEGNATURA, " +
				DocsPaDbManagement.Functions.Functions.ToChar("P.CREATION_DATE",false) + " AS DATA, " +
				"P.ID_REGISTRO AS ID_REGISTRO, " +
				// "R.VAR_CODICE As codRegistro, " +
				"P.CHA_TIPO_PROTO As A_P, " +
				"P.VAR_PROF_OGGETTO AS OGGETTO, " +
				"P.SYSTEM_ID, " +
                "P.CHA_IMG AS CHA_IMG, " +  //aggiunto
                "P.ID_DOCUMENTO_PRINCIPALE " +  //aggiunto
				"FROM	PROFILE P,SECURITY S,DPA_EL_REGISTRI R " +
				"WHERE	P.DOCNUMBER IN (" + resultQuery + ")  " +
				"AND (S.ACCESSRIGHTS >= 0   " +
				"AND (S.PERSONORGROUP = " + infoUtente.idPeople + " OR S.PERSONORGROUP=" + infoUtente.idGruppo + "))  " +
				"AND S.THING = P.SYSTEM_ID " + 
				"AND (R.SYSTEM_ID=P.ID_REGISTRO OR P.ID_REGISTRO IS NULL)";
									
			logger.Debug(querySql);

			DataSet ds = new DataSet();

			this.ExecuteQuery(out ds,"PROFILE",querySql);

			// recupero informazioni
			foreach (DataRow dr in ds.Tables["PROFILE"].Rows)
			{
				DocsPaVO.documento.InfoDocumento tmp = this.GetInfoDocumento(dr);
				result.Add(tmp);
				tmp=null;
			}
			
			ds = null;

			return result;
		}

		#endregion

	}
}
