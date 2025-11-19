// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaDB;
using DocsPaVO.amministrazione;
using Serilog;
using System.Collections;

namespace BusinessLogic.Amministrazione
{
    public class FunzioniManager
    {
        private static ILogger logger = Log.ForContext(typeof(FunzioniManager));

        public static OrgFunzione[] GetFunzioni(string idTipoFunzione)
        {
            ArrayList retValue = new ArrayList();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_FUNZIONI_ELEMENTARI_TIPO_FUNZIONE");
            queryDef.setParam("idTipoFunzione", idTipoFunzione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        retValue.Add(CreateFunzione(reader));
                }
            }

            return (OrgFunzione[])retValue.ToArray(typeof(OrgFunzione));
        }

        private static OrgFunzione CreateFunzione(System.Data.IDataReader reader)
        {
            OrgFunzione retValue = new OrgFunzione();

            if (!reader.IsDBNull(reader.GetOrdinal("ID_ASSOCIAZIONE")))
                retValue.ID = reader.GetValue(reader.GetOrdinal("ID_ASSOCIAZIONE")).ToString();

            if (!reader.IsDBNull(reader.GetOrdinal("ID_TIPO_FUNZIONE")))
                retValue.IDTipoFunzione = reader.GetValue(reader.GetOrdinal("ID_TIPO_FUNZIONE")).ToString();

            string associato = reader.GetString(reader.GetOrdinal("ASSOCIATO"));

            retValue.Associato = Convert.ToBoolean(associato);

            retValue.FunzioneAnagrafica = CreateFunzioneAnagrafica(reader);

            return retValue;
        }

        private static OrgFunzioneAnagrafica CreateFunzioneAnagrafica(System.Data.IDataReader reader)
        {
            OrgFunzioneAnagrafica retValue = new OrgFunzioneAnagrafica();

            retValue.Codice = reader.GetValue(reader.GetOrdinal("CODICE")).ToString();
            retValue.Descrizione = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
            retValue.TipoFunzione = reader.GetValue(reader.GetOrdinal("TIPO_FUNZIONE")).ToString();

            return retValue;
        }

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

        public static OrgFunzioneAnagrafica[] GetFunzioniAnagrafica()
        {
            ArrayList retValue = new ArrayList();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_ANAGRAFICA_FUNZIONI_ELEMENTARI");

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        retValue.Add(CreateFunzioneAnagrafica(reader));
                }
            }

            return (OrgFunzioneAnagrafica[])retValue.ToArray(typeof(OrgFunzioneAnagrafica));
        }

        internal static bool Update(DBProvider dbProvider, OrgFunzione funzione)
        {
            bool retValue = false;

            bool insertMode = (funzione.StatoFunzione == DocsPaVO.amministrazione.OrgFunzione.StatoOrgFunzioneEnum.Inserted);
            bool deleteMode = (funzione.StatoFunzione == DocsPaVO.amministrazione.OrgFunzione.StatoOrgFunzioneEnum.Deleted);

            DocsPaUtils.Query queryDef = null;
            string commandText = string.Empty;

            if (insertMode || deleteMode)
            {
                if (insertMode)
                {
                    // Inserimento del legame tra tipo funzione e anagrafica funzione
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_INSERT_FUNZIONE_ELEMENTARE");
                    queryDef.setParam("colSystemID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryDef.setParam("systemID", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                    queryDef.setParam("codiceFunzione", GetStringParameterValue(funzione.FunzioneAnagrafica.Codice));
                    queryDef.setParam("idTipoFunzione", funzione.IDTipoFunzione);
                    queryDef.setParam("chaTipoFunz", GetStringParameterValue(funzione.FunzioneAnagrafica.TipoFunzione));
                    queryDef.setParam("descrizioneFunzione", GetStringParameterValue(funzione.FunzioneAnagrafica.Codice));
                    queryDef.setParam("idAmm", funzione.IDAmministrazione);
                }
                else
                {
                    // Cancellazione del legame tra tipo funzione e anagrafica funzione
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_FUNZIONE_ELEMENTARE");
                    queryDef.setParam("systemID", funzione.ID);
                }

                commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected = 0;

                try
                {
                    retValue = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue = (retValue && rowsAffected == 1);

                    if (retValue)
                    {
                        if (insertMode)
                        {
                            // Reperimento e impostazione systemID appena inserita
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                            logger.Debug(commandText);

                            string outParam;
                            retValue = dbProvider.ExecuteScalar(out outParam, commandText);

                            if (retValue)
                                funzione.ID = outParam;
                        }
                        else if (deleteMode)
                        {
                            // Funzione non più presente su database,
                            // rimozione degli id
                            funzione.ID = string.Empty;
                            funzione.IDTipoFunzione = string.Empty;
                        }

                        // Impostazione dello stato della funzione
                        funzione.StatoFunzione = DocsPaVO.amministrazione.OrgFunzione.StatoOrgFunzioneEnum.Unchanged;
                    }
                }
                catch
                {
                    retValue = false;
                }
                finally
                {
                }
            }

            return retValue;
        }

        private static string GetStringParameterValue(string paramValue)
        {
            if (paramValue == string.Empty)
                return "Null";
            else
                return "'" + DocsPaUtils.Functions.Functions.ReplaceApexes(paramValue) + "'";
        }

    }
}
