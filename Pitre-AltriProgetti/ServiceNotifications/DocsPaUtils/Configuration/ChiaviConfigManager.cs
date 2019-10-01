using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.amministrazione;
using DocsPaVO.Validations;
using DocsPaUtils.Interfaces.DbManagement;
using log4net;

namespace DocsPaUtils.Configuration
{
    public class ChiaviConfigManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ChiaviConfigManager));

        public ChiaviConfigManager()
        { }

        #region public Methods   

        /// <summary>
		/// Reperimento di tutte le chiavi
		/// </summary>
		/// <returns></returns>
		public static ArrayList GetChiaviConfig(string idAmm)
		{
			ArrayList retValue=new ArrayList();

			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_CHIAVI_CONFIGURAZIONE");
            queryDef.setParam("idAmm", idAmm);
			string commandText=queryDef.getSQL();
			//ATTENZIONE 
            logger.Debug(commandText);

            IDatabase database = DatabaseFactory.CreateDatabase();
            try
            {
                using (System.Data.IDataReader reader = database.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        DocsPaVO.amministrazione.ChiaveConfigurazione chiaveConfig = CreateChiaveConfigurazione(reader);
                        retValue.Add(chiaveConfig);
                   }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel caricamento dell'hashtable. " + e.Message.ToString());
            }

            return retValue;
		}

		/// <summary>
		/// Reperimento chiave NB: al momento non è utilizzata
		/// </summary>
		/// <param name="idChiave"></param>
		/// <returns></returns>
		public static ChiaveConfigurazione GetChiave(string idChiave)
		{
			ChiaveConfigurazione retValue=null;

			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_CHIAVE_CONFIGURAZIONE");
			queryDef.setParam("idChiave",idChiave);

			string commandText=queryDef.getSQL();
            logger.Debug(commandText);

            IDatabase database = DatabaseFactory.CreateDatabase();

            using (System.Data.IDataReader reader = database.ExecuteReader(commandText))
			{
				if (reader.Read())
				{
					retValue=CreateChiaveConfigurazione(reader);
				}
			}	

			return retValue;
		}

	
		private static string GetStringParameterValue(string paramValue)
		{
			return DocsPaUtils.Functions.Functions.ReplaceApexes(paramValue);
		}

		/// <summary>
		/// Verifica aggiornamento chiaveConfig
		/// </summary>
		/// <param name="tipoFunzione"></param>
		/// <returns></returns>
		public static ValidationResultInfo CanUpdateChiaveConfig(ChiaveConfigurazione chiaveConfig)
		{
			ValidationResultInfo retValue=IsValidRequiredFieldsChiaveConfigurazione(DBActionTypeChiaveConfigurazioneEnum.UpdateMode,chiaveConfig);
			return retValue;
		}

		/// <summary>
		/// Aggiornamento di un tipo chiave
		/// </summary>
		/// <param name="tipoFunzione"></param>
		/// <returns></returns>
		public static ValidationResultInfo UpdateChiaveConfigurazione(ChiaveConfigurazione chiaveConfig)
		{
			ValidationResultInfo retValue=CanUpdateChiaveConfig(chiaveConfig);
			
			if (retValue.Value)
			{
                IDatabase database = DatabaseFactory.CreateDatabase();
                
				try
				{
                    //database.BeginTransaction();  //non c'è bisogno della transazione perchè viene aggiornata una sola tabella

					// Aggiornamento tipo chiave
					DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_UPDATE_CHIAVE_CONFIGURAZIONE");
					queryDef.setParam("descrizione",GetStringParameterValue(chiaveConfig.Descrizione));
                    queryDef.setParam("valore",GetStringParameterValue(chiaveConfig.Valore));
					queryDef.setParam("systemID",chiaveConfig.IDChiave);

					string commandText=queryDef.getSQL();
                    logger.Debug(commandText);

					int rowsAffected=0;

                    retValue.Value = database.ExecuteNonQuery(commandText, out rowsAffected);
                    if (retValue.Value)
                        retValue.Value=(retValue.Value && rowsAffected==1);
                    if (!retValue.Value)
                        retValue.BrokenRules.Add(new BrokenRule("AGGIORNAMENTO_CHIAVE_CONFIGURAZIONE", "Non è stata aggiornata alcuna chiave"));
                   
                   

                    //if (retValue.Value)
                    //    database.CommitTransaction();
                    //else
                    //    database.RollbackTransaction();
				}
				catch
				{
                    //database.RollbackTransaction();
					retValue.Value=false;
				}
				finally
				{
                    database.Dispose();
                    database = null;
				}
			}

			return retValue;
		}

        private static ChiaveConfigurazione CreateChiaveConfigurazione(System.Data.IDataReader reader)
        {
            ChiaveConfigurazione retValue = new ChiaveConfigurazione();

            retValue.IDChiave = reader.GetValue(reader.GetOrdinal("ID")).ToString();
            retValue.IDAmministrazione = reader.GetValue(reader.GetOrdinal("ID_AMMINISTRAZIONE")).ToString();
            retValue.Codice = reader.GetValue(reader.GetOrdinal("CODICE")).ToString();
            retValue.Descrizione = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
            retValue.Valore = reader.GetValue(reader.GetOrdinal("VALORE")).ToString();
            retValue.Visibile = reader.GetValue(reader.GetOrdinal("CHA_VIS")).ToString();
            retValue.Modificabile = reader.GetValue(reader.GetOrdinal("CHA_MOD")).ToString();
            retValue.TipoChiave = reader.GetValue(reader.GetOrdinal("TIPO")).ToString();
            retValue.IsGlobale = reader.GetValue(reader.GetOrdinal("CHA_GLOBALE")).ToString();
            return retValue;
        }

		#endregion

		#region Private methods

		private enum DBActionTypeChiaveConfigurazioneEnum
		{
			InsertMode,
			UpdateMode,
			DeleteMode
		}

		private static ValidationResultInfo IsValidRequiredFieldsChiaveConfigurazione(
							DBActionTypeChiaveConfigurazioneEnum actionType,
							DocsPaVO.amministrazione.ChiaveConfigurazione chiaveConfig)
		{
			ValidationResultInfo retValue=new ValidationResultInfo();

			if (actionType!=DBActionTypeChiaveConfigurazioneEnum.InsertMode &&
				(chiaveConfig.IDChiave==null || 
				 chiaveConfig.IDChiave==string.Empty || 
				 chiaveConfig.IDChiave=="0"))
			{
				retValue.Value=false;
				retValue.BrokenRules.Add(new BrokenRule("ID_CHIAVE","ID chiave mancante"));
			}

            if (actionType == DBActionTypeChiaveConfigurazioneEnum.InsertMode ||
                actionType == DBActionTypeChiaveConfigurazioneEnum.UpdateMode)
			{
				if (chiaveConfig.Codice==null || chiaveConfig.Codice==string.Empty)
				{
					retValue.Value=false;
					retValue.BrokenRules.Add(new BrokenRule("CODICE_CHIAVE_CONFIGURAZIONE","Codice chiave configurazione mancante"));
				}

				if (chiaveConfig.Descrizione==null || chiaveConfig.Descrizione==string.Empty)
				{
					retValue.Value=false;
					retValue.BrokenRules.Add(new BrokenRule("DESCRIZIONE_CHIAVE_CONFIGURAZIONE","Descrizione chiave configurazione mancante"));
				}

                if (chiaveConfig.Valore == null || chiaveConfig.Valore == string.Empty)
                {
                    retValue.Value = false;
                    retValue.BrokenRules.Add(new BrokenRule("VALORE_CHIAVE_CONFIGURAZIONE", "Valore chiave configurazione mancante"));
                }
				
			}

			return retValue;
		}

	
		#endregion
    }
}
