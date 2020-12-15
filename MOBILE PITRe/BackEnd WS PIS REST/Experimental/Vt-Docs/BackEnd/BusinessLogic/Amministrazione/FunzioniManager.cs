using System;
using System.Collections;
using System.Data;
using DocsPaDB;
using DocsPaVO.amministrazione;
using DocsPaVO.Validations;
using log4net;

namespace BusinessLogic.Amministrazione
{
	/// <summary>
	/// Classe per la gestione delle funzioni elementari
	/// </summary>
	public class FunzioniManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(FunzioniManager));
		private FunzioniManager()
		{
		}

		#region Public methods

		/// <summary>
		/// Reperimento di tutte le funzioni elementari presenti in anagrafica
		/// </summary>
		/// <returns></returns>
		public static OrgFunzioneAnagrafica[] GetFunzioniAnagrafica()
		{
			ArrayList retValue=new ArrayList();

			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_ANAGRAFICA_FUNZIONI_ELEMENTARI");

			string commandText=queryDef.getSQL();
			logger.Debug(commandText);

			using (DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider())
			{
				using (System.Data.IDataReader reader=dbProvider.ExecuteReader(commandText))
				{
					while (reader.Read())
						retValue.Add(CreateFunzioneAnagrafica(reader));
				}	
			}

			return (OrgFunzioneAnagrafica[]) retValue.ToArray(typeof(OrgFunzioneAnagrafica));
		}

		/// <summary>
		/// Reperimento delle funzioni elementari presenti in un tipo funzione
		/// </summary>
		/// <param name="idTipoFunzione"></param>
		/// <returns></returns>
		public static OrgFunzione[] GetFunzioni(string idTipoFunzione)
		{
			ArrayList retValue=new ArrayList();

			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_FUNZIONI_ELEMENTARI_TIPO_FUNZIONE");
			queryDef.setParam("idTipoFunzione",idTipoFunzione);

			string commandText=queryDef.getSQL();
			logger.Debug(commandText);
	
			using (DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider())
			{
				using (System.Data.IDataReader reader=dbProvider.ExecuteReader(commandText))
				{
					while (reader.Read())
						retValue.Add(CreateFunzione(reader));
				}	
			}

			return (OrgFunzione[]) retValue.ToArray(typeof(OrgFunzione));
		}

		/// <summary>
		/// Aggiornamento del legame tra funzione e tipo funzione
		/// </summary>
		/// <param name="funzione"></param>
		/// <returns></returns>
		public static ValidationResultInfo Update(OrgFunzione funzione)
		{
			ValidationResultInfo retValue=new ValidationResultInfo();

			if (funzione.StatoFunzione!=OrgFunzione.StatoOrgFunzioneEnum.Unchanged)
			{
				DBProvider dbProvider=new DBProvider();

				try
				{
					dbProvider.BeginTransaction();
				
					retValue.Value=Update(dbProvider,funzione);

					if (retValue.Value)
					{
						dbProvider.CommitTransaction();
					}
					else
					{
						dbProvider.RollbackTransaction();

						retValue.BrokenRules.Add(new DocsPaVO.Validations.BrokenRule("DB_ERROR","Errore in aggiornamento della funzione"));
					}
				}
				catch
				{
					dbProvider.RollbackTransaction();
					retValue.Value=false;
				}
				finally
				{
					dbProvider.Dispose();
					dbProvider=null;
				}
			}

			return retValue;
		}
		
		/// <summary>
		/// Reperimento funzione elementare da ID
		/// </summary>
		/// <param name="idFunzione"></param>
		/// <returns></returns>
		public static OrgFunzione GetFunzione(int idFunzione)
		{
			OrgFunzione retValue=null;

			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_FUNZIONE_ELEMENTARE");
			queryDef.setParam("idFunzione",idFunzione.ToString());

			string commandText=queryDef.getSQL();
			logger.Debug(commandText);

			using (DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider())
			{
				using (System.Data.IDataReader reader=dbProvider.ExecuteReader(commandText))
				{
					if (reader.Read())
						retValue=CreateFunzione(reader);
				}
			}

			return retValue;
		}

		/// <summary>
		/// Reperimento funzione elementare da codice e id tipo funzione
		/// </summary>
		/// <param name="codiceFunzione"></param>
		/// <param name="idTipoFunzione"></param>
		/// <returns></returns>
		public static OrgFunzione GetFunzione(string codiceFunzione,int idTipoFunzione)
		{
			OrgFunzione retValue=null;

			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_FUNZIONE_ELEMENTARE_DA_CODICE");
			queryDef.setParam("codiceFunzione",codiceFunzione);
			queryDef.setParam("idTipoFunzione",idTipoFunzione.ToString());

			string commandText=queryDef.getSQL();
			logger.Debug(commandText);

			using (DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider())
			{
				using (System.Data.IDataReader reader=dbProvider.ExecuteReader(commandText))
				{
					if (reader.Read())
						retValue=CreateFunzione(reader);
				}
			}

			return retValue;
		}

        // MEV esportazione dettagli funzione

        public static OrgFunzioneAnagrafica GetAnagraficaPerReport(string codiceFunzione)
        {
            OrgFunzioneAnagrafica retVal = null;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_ANAGRAFICA_FUNZIONE_REPORT");
            queryDef.setParam("codice", codiceFunzione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                        retVal = CreateFunzioneAnagrafica(reader);
                }
            }

            return retVal;
        }

        //Verifica se una data microfunzione è presente nella tabella dpa_anagrafica_funzioni
        public static bool FunzioneEsistente(string codiceFunzione)
        {
            bool retValue = false;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("GET_FUNZIONE");
            queryDef.setParam("codFunzione", codiceFunzione);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string outParam;
                if (dbProvider.ExecuteScalar(out outParam, commandText))
                    retValue = (Convert.ToInt32(outParam) > 0);
            }
            return retValue;
        }
		#endregion

		#region Private methods

		/// <summary>
		/// Creazione oggetto "OrgFunzioneAnagrafica" da datareader
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		private static OrgFunzioneAnagrafica CreateFunzioneAnagrafica(System.Data.IDataReader reader)
		{
			OrgFunzioneAnagrafica retValue=new OrgFunzioneAnagrafica();

			retValue.Codice=reader.GetValue(reader.GetOrdinal("CODICE")).ToString();
			retValue.Descrizione=reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
			retValue.TipoFunzione=reader.GetValue(reader.GetOrdinal("TIPO_FUNZIONE")).ToString();

			return retValue;
		}

		/// <summary>
		/// Creazione oggetto "OrgFunzione" da datareader
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		private static OrgFunzione CreateFunzione(System.Data.IDataReader reader)
		{
			OrgFunzione retValue=new OrgFunzione();

			if (!reader.IsDBNull(reader.GetOrdinal("ID_ASSOCIAZIONE")))
				retValue.ID=reader.GetValue(reader.GetOrdinal("ID_ASSOCIAZIONE")).ToString();

			if (!reader.IsDBNull(reader.GetOrdinal("ID_TIPO_FUNZIONE")))
				retValue.IDTipoFunzione=reader.GetValue(reader.GetOrdinal("ID_TIPO_FUNZIONE")).ToString();

			string associato=reader.GetString(reader.GetOrdinal("ASSOCIATO"));
			
			retValue.Associato=Convert.ToBoolean(associato);

			retValue.FunzioneAnagrafica=CreateFunzioneAnagrafica(reader);
			
			return retValue;
		}

		/// <summary>
		/// Aggiornamento legame funzione - tipo funzione
		/// </summary>
		/// <param name="dbProvider"></param>
		/// <param name="funzione"></param>
		/// <returns></returns>
		internal static bool Update(DBProvider dbProvider,OrgFunzione funzione)
		{
			bool retValue=false;

			bool insertMode=(funzione.StatoFunzione==DocsPaVO.amministrazione.OrgFunzione.StatoOrgFunzioneEnum.Inserted);
			bool deleteMode=(funzione.StatoFunzione==DocsPaVO.amministrazione.OrgFunzione.StatoOrgFunzioneEnum.Deleted);
			
			DocsPaUtils.Query queryDef=null;
			string commandText=string.Empty;

			if (insertMode)
			{
				// Inserimento del legame tra tipo funzione e anagrafica funzione
				queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_INSERT_FUNZIONE_ELEMENTARE");
				queryDef.setParam("colSystemID",DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
				queryDef.setParam("systemID",DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
				queryDef.setParam("codiceFunzione",GetStringParameterValue(funzione.FunzioneAnagrafica.Codice));
				queryDef.setParam("idTipoFunzione",funzione.IDTipoFunzione);
				queryDef.setParam("chaTipoFunz",GetStringParameterValue(funzione.FunzioneAnagrafica.TipoFunzione));
				queryDef.setParam("descrizioneFunzione",GetStringParameterValue(funzione.FunzioneAnagrafica.Codice));
                queryDef.setParam("idAmm", funzione.IDAmministrazione);
			}
			else if (deleteMode)
			{
				// Cancellazione del legame tra tipo funzione e anagrafica funzione
				queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_FUNZIONE_ELEMENTARE");
				queryDef.setParam("systemID",funzione.ID);				
			}

			commandText=queryDef.getSQL();
			logger.Debug(commandText);
			
			int rowsAffected=0;

			try
			{
				retValue=dbProvider.ExecuteNonQuery(commandText,out rowsAffected);
				retValue=(retValue && rowsAffected==1);

				if (retValue)
				{
					if (insertMode)
					{
						// Reperimento e impostazione systemID appena inserita
						commandText=DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
						logger.Debug(commandText);

						string outParam;
						retValue=dbProvider.ExecuteScalar(out outParam,commandText);

						if (retValue)
							funzione.ID=outParam;
					}
					else if (deleteMode)
					{
						// Funzione non più presente su database,
						// rimozione degli id
						funzione.ID=string.Empty;
						funzione.IDTipoFunzione=string.Empty;
					}
				
					// Impostazione dello stato della funzione
					funzione.StatoFunzione=DocsPaVO.amministrazione.OrgFunzione.StatoOrgFunzioneEnum.Unchanged;
				}
			}
			catch
			{
				retValue=false;
			}
			finally
			{
			}

			return retValue;
		}

		private static string GetStringParameterValue(string paramValue)
		{
			if (paramValue==string.Empty)
				return "Null";
			else
				return "'" + DocsPaUtils.Functions.Functions.ReplaceApexes(paramValue) + "'";
		}

		#endregion
	}
}