// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaDB;
using DocsPaVO.amministrazione;
using DocsPaVO.Validations;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Serilog;
using System.Collections;

namespace BusinessLogic.Amministrazione
{
    public class TipiFunzioneManager
    {
        private static ILogger logger = Serilog.Log.ForContext(typeof(TipiFunzioneManager));

        public static OrgTipoFunzione GetTipoFunzioneByCod(string codFunzione, bool fillFunzioniElementari)
        {
            OrgTipoFunzione retValue = null;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_TIPO_FUNZIONE_BY_COD");
            queryDef.setParam("codFunzione", codFunzione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        retValue = CreateTipoFunzione(reader);

                        if (fillFunzioniElementari)
                            retValue.Funzioni = FunzioniManager.GetFunzioni(retValue.IDTipoFunzione);
                    }
                }
            }

            return retValue;
        }

        private static OrgTipoFunzione CreateTipoFunzione(System.Data.IDataReader reader)
        {
            OrgTipoFunzione retValue = new OrgTipoFunzione();

            retValue.IDTipoFunzione = reader.GetValue(reader.GetOrdinal("ID")).ToString();
            retValue.Codice = reader.GetValue(reader.GetOrdinal("CODICE")).ToString();
            retValue.Descrizione = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
            retValue.IDAmministrazione = string.Empty;

            return retValue;
        }

        public static DocsPaVO.documento.FileDocumento ReportFunzioni(string tipoReport, string formato, string idFunzione, string idAmm, ISpreadsheetService spreadsheetService, IFileConverterFactory fileConverterPDF, IReportGeneratorService reportGeneratorService)
        {
            DocsPaVO.documento.FileDocumento report = new DocsPaVO.documento.FileDocumento();
            try
            {

                // filtri report
                List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "id_amm", valore = idAmm });
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "id_funzione", valore = idFunzione });

                // request generazione report
                DocsPaVO.Report.PrintReportRequest request = new DocsPaVO.Report.PrintReportRequest();
                request.SearchFilters = filters;

                // selezione formato report
                switch (formato)
                {
                    case "XLS":
                        request.ReportType = DocsPaVO.Report.ReportTypeEnum.Excel;
                        break;

                    case "ODS":
                        request.ReportType = DocsPaVO.Report.ReportTypeEnum.ODS;
                        break;
                }

                string descrizione = string.Empty;
                // selezione tipo report
                switch (tipoReport)
                {
                    case "MACRO_FUNZ":
                        descrizione = GetTipoFunzioneByCod(idFunzione, false).Descrizione;
                        request.ContextName = "AmmExportMacroFunzioni";
                        request.ReportKey = "AmmExportMacroFunzioni";
                        request.Title = string.Empty;
                        request.SubTitle = string.Format("Codice: {0} - Descrizione: {1}", idFunzione, descrizione);
                        request.AdditionalInformation = string.Empty;
                        break;

                    case "MICRO_FUNZ":
                        descrizione = FunzioniManager.GetAnagraficaPerReport(idFunzione).Descrizione;
                        request.ContextName = "AmmExportMicroFunzioni";
                        request.ReportKey = "AmmExportMicroFunzioni";
                        request.Title = "Dettaglio ruoli e tipi funzione associati alla funzione.";
                        request.SubTitle = string.Format("Codice : {0} - Descrizione: {1}", idFunzione, descrizione);
                        request.AdditionalInformation = string.Empty;
                        break;
                }

                report = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request, spreadsheetService, fileConverterPDF, reportGeneratorService).Document;

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return report;
        }

        public static OrgTipoFunzione[] GetTipiFunzione(bool fillFunzioniElementari, string idAmm)
        {
            ArrayList retValue = new ArrayList();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_TIPI_FUNZIONE");
            queryDef.setParam("idAmm", idAmm);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        DocsPaVO.amministrazione.OrgTipoFunzione tipoFunzione = CreateTipoFunzione(reader);

                        if (fillFunzioniElementari)
                            tipoFunzione.Funzioni = FunzioniManager.GetFunzioni(tipoFunzione.IDTipoFunzione);

                        retValue.Add(tipoFunzione);
                    }
                }
            }

            return (OrgTipoFunzione[])retValue.ToArray(typeof(OrgTipoFunzione));
        }

        public static OrgTipoFunzione GetTipoFunzione(string idTipoFunzione, bool fillFunzioniElementari)
        {
            OrgTipoFunzione retValue = null;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_TIPO_FUNZIONE");
            queryDef.setParam("idTipoFunzione", idTipoFunzione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        retValue = CreateTipoFunzione(reader);

                        if (fillFunzioniElementari)
                            retValue.Funzioni = FunzioniManager.GetFunzioni(retValue.IDTipoFunzione);
                    }
                }
            }

            return retValue;
        }

        public static ValidationResultInfo InsertTipoFunzione(OrgTipoFunzione tipoFunzione)
        {
            ValidationResultInfo retValue = CanInsertTipoFunzione(tipoFunzione);

            if (retValue.Value)
            {
                DBProvider dbProvider = new DBProvider();

                try
                {
                    dbProvider.BeginTransaction();

                    // Inserimento tipo funzione
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_I_INSERT_TIPO_FUNZIONE");
                    queryDef.setParam("colSystemID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryDef.setParam("systemID", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                    queryDef.setParam("codice", GetStringParameterValue(tipoFunzione.Codice));
                    queryDef.setParam("descrizione", GetStringParameterValue(tipoFunzione.Descrizione));
                    queryDef.setParam("idAmm", tipoFunzione.IDAmministrazione);
                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    int rowsAffected = 0;

                    retValue.Value = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue.Value = (retValue.Value && rowsAffected == 1);

                    if (retValue.Value && tipoFunzione.Funzioni != null)
                    {
                        // Reperimento SystemID tipo funzione
                        commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                        logger.Debug(commandText);
                        string systemID;
                        retValue.Value = dbProvider.ExecuteScalar(out systemID, commandText);

                        if (retValue.Value)
                        {
                            tipoFunzione.IDTipoFunzione = systemID;

                            // Inserimento funzioni elementari
                            if (tipoFunzione.Funzioni != null && tipoFunzione.Funzioni.Length > 0)
                                retValue.Value = UpdateFunzioniElementari(dbProvider, tipoFunzione);
                        }
                        else
                        {
                            retValue.BrokenRules.Add(new BrokenRule("DB_ERROR", "Errore in inserimento del tipo funzione"));
                        }
                    }
                    else
                    {
                        retValue.BrokenRules.Add(new BrokenRule("DB_ERROR", "Errore in inserimento del tipo funzione"));
                    }

                    if (retValue.Value)
                        dbProvider.CommitTransaction();
                    else
                        dbProvider.RollbackTransaction();
                }
                catch
                {
                    dbProvider.RollbackTransaction();
                    retValue.Value = false;
                }
                finally
                {
                    dbProvider.Dispose();
                    dbProvider = null;
                }
            }

            return retValue;
        }

        public static ValidationResultInfo CanInsertTipoFunzione(OrgTipoFunzione tipoFunzione)
        {
            ValidationResultInfo retValue = IsValidRequiredFieldsTipoFunzione(DBActionTypeTipoFunzioneEnum.InsertMode, tipoFunzione);

            if (retValue.Value)
            {
                // Verifica univocità codice tipo funzione
                if (ContainsCodiceTipoFunzione(tipoFunzione))
                {
                    retValue.Value = false;
                    retValue.BrokenRules.Add(new BrokenRule("CODICE_TIPO_FUNZIONE", "Codice tipo funzione già presente"));
                }
            }

            return retValue;
        }

        private static string GetStringParameterValue(string paramValue)
        {
            return DocsPaUtils.Functions.Functions.ReplaceApexes(paramValue);
        }

        private static bool UpdateFunzioniElementari(DBProvider dbProvider, DocsPaVO.amministrazione.OrgTipoFunzione tipoFunzione)
        {
            bool retValue = true;

            foreach (OrgFunzione funzione in tipoFunzione.Funzioni)
            {
                if (funzione.StatoFunzione != OrgFunzione.StatoOrgFunzioneEnum.Unchanged)
                {
                    funzione.IDTipoFunzione = tipoFunzione.IDTipoFunzione;

                    retValue = FunzioniManager.Update(dbProvider, funzione);

                    if (!retValue)
                        break;
                }
            }

            return retValue;
        }

        private enum DBActionTypeTipoFunzioneEnum
        {
            InsertMode,
            UpdateMode,
            DeleteMode
        }

        private static ValidationResultInfo IsValidRequiredFieldsTipoFunzione(
                            DBActionTypeTipoFunzioneEnum actionType,
                            DocsPaVO.amministrazione.OrgTipoFunzione tipoFunzione)
        {
            ValidationResultInfo retValue = new ValidationResultInfo();

            if (actionType != DBActionTypeTipoFunzioneEnum.InsertMode &&
                (tipoFunzione.IDTipoFunzione == null ||
                 tipoFunzione.IDTipoFunzione == string.Empty ||
                 tipoFunzione.IDTipoFunzione == "0"))
            {
                retValue.Value = false;
                retValue.BrokenRules.Add(new BrokenRule("ID_TIPO_FUNZIONE", "ID tipo funzione mancante"));
            }

            if (actionType == DBActionTypeTipoFunzioneEnum.InsertMode ||
                actionType == DBActionTypeTipoFunzioneEnum.UpdateMode)
            {
                if (tipoFunzione.Codice == null || tipoFunzione.Codice == string.Empty)
                {
                    retValue.Value = false;
                    retValue.BrokenRules.Add(new BrokenRule("CODICE_TIPO_FUNZIONE", "Codice tipo funzione mancante"));
                }

                if (tipoFunzione.Descrizione == null || tipoFunzione.Descrizione == string.Empty)
                {
                    retValue.Value = false;
                    retValue.BrokenRules.Add(new BrokenRule("DESCRIZIONE_TIPO_FUNZIONE", "Descrizione tipo funzione mancante"));
                }

                DocsPaVO.Validations.BrokenRule ruleFunzioniMancanti = new BrokenRule("FUNZIONI_MANCANTI", "Nessuna funzione elementare associata al tipo funzione");

                if (tipoFunzione.Funzioni == null || tipoFunzione.Funzioni.Length == 0)
                {
                    retValue.Value = false;
                    retValue.BrokenRules.Add(ruleFunzioniMancanti);
                }
            }

            if (actionType == DBActionTypeTipoFunzioneEnum.UpdateMode)
            {
                DocsPaVO.Validations.BrokenRule ruleFunzioniMancanti = new BrokenRule("FUNZIONI_MANCANTI", "Nessuna funzione elementare associata al tipo funzione");

                DBProvider dbProvider = new DBProvider();

                try
                {
                    dbProvider.BeginTransaction();
                    int functionsAssociate = 0;
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_COUNT_FUNZ_TIPO_FUNZIONE");
                    queryDef.setParam("nomeTipo", tipoFunzione.Codice.ToUpper());
                    queryDef.setParam("idAmm", tipoFunzione.IDAmministrazione);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    string parm = string.Empty;
                    if (dbProvider.ExecuteScalar(out parm, commandText))
                    //retValue.Value = dbProvider.ExecuteNonQuery(commandText, out functionsAssociate);
                    {
                        functionsAssociate = Convert.ToInt32(parm);
                        foreach (DocsPaVO.amministrazione.OrgFunzione funzione in tipoFunzione.Funzioni)
                        {
                            if (funzione.StatoFunzione != OrgFunzione.StatoOrgFunzioneEnum.Unchanged && !funzione.Associato)
                                functionsAssociate--;
                        }

                        if (functionsAssociate == 0)
                        {
                            retValue.Value = false;
                            retValue.BrokenRules.Add(ruleFunzioniMancanti);
                        }
                    }
                }
                catch
                {
                    dbProvider.RollbackTransaction();
                    retValue.Value = false;
                }
                finally
                {
                    dbProvider.Dispose();
                    dbProvider = null;
                }
            }

            return retValue;
        }

        private static bool ContainsCodiceTipoFunzione(DocsPaVO.amministrazione.OrgTipoFunzione tipoFunzione)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_CONTAINS_CODICE_TIPO_FUNZIONE");
            queryDef.setParam("codiceTipoFunzione", tipoFunzione.Codice);
            queryDef.setParam("idAmm", tipoFunzione.IDAmministrazione);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DBProvider dbProvider = new DBProvider())
            {
                string outParam;
                if (dbProvider.ExecuteScalar(out outParam, commandText))
                    retValue = (Convert.ToInt32(outParam) > 0);
            }

            return retValue;
        }

        public static ValidationResultInfo CanUpdateTipoFunzione(OrgTipoFunzione tipoFunzione)
        {
            ValidationResultInfo retValue = IsValidRequiredFieldsTipoFunzione(DBActionTypeTipoFunzioneEnum.UpdateMode, tipoFunzione);

            return retValue;
        }

        public static ValidationResultInfo UpdateTipoFunzione(OrgTipoFunzione tipoFunzione)
        {
            ValidationResultInfo retValue = CanUpdateTipoFunzione(tipoFunzione);

            if (retValue.Value)
            {
                DBProvider dbProvider = new DBProvider();

                try
                {
                    dbProvider.BeginTransaction();

                    // Aggiornamento tipo funzione
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_U_UPDATE_TIPO_FUNZIONE");
                    queryDef.setParam("descrizione", GetStringParameterValue(tipoFunzione.Descrizione));
                    queryDef.setParam("systemID", tipoFunzione.IDTipoFunzione);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    int rowsAffected = 0;

                    retValue.Value = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue.Value = (retValue.Value && rowsAffected == 1);

                    if (retValue.Value && tipoFunzione.Funzioni != null && tipoFunzione.Funzioni.Length > 0)
                        // Aggiornamento funzioni elementari
                        retValue.Value = UpdateFunzioniElementari(dbProvider, tipoFunzione);

                    if (retValue.Value)
                        dbProvider.CommitTransaction();
                    else
                        dbProvider.RollbackTransaction();
                }
                catch
                {
                    dbProvider.RollbackTransaction();
                    retValue.Value = false;
                }
                finally
                {
                    dbProvider.Dispose();
                    dbProvider = null;
                }
            }

            return retValue;
        }

        public static ValidationResultInfo CanDeleteTipoFunzione(OrgTipoFunzione tipoFunzione)
        {
            ValidationResultInfo retValue = IsValidRequiredFieldsTipoFunzione(DBActionTypeTipoFunzioneEnum.DeleteMode, tipoFunzione);

            if (retValue.Value)
            {
                // Verifica presenza del tipo funzione in almeno un ruolo
                if (ContainsRuoli(tipoFunzione))
                {
                    retValue.Value = false;
                    retValue.BrokenRules.Add(new BrokenRule("CONTAINS_ROLE", "Al tipo funzione risulta associato almeno un ruolo in organigramma"));
                }
            }
            return retValue;
        }

        public static ValidationResultInfo DeleteTipoFunzione(OrgTipoFunzione tipoFunzione)
        {
            ValidationResultInfo retValue = CanDeleteTipoFunzione(tipoFunzione);

            if (retValue.Value)
            {
                DBProvider dbProvider = new DBProvider();

                try
                {
                    // Cancellazione tipo funzione
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_TIPO_FUNZIONE");
                    queryDef.setParam("systemID", tipoFunzione.IDTipoFunzione);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    int rowsAffected = 0;

                    retValue.Value = dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                    retValue.Value = (retValue.Value && rowsAffected == 1);

                    if (retValue.Value)
                    {
                        // Cancellazione funzioni elementari
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_D_DELETE_FUNZIONI_TIPO_FUNZIONE");
                        queryDef.setParam("idTipoFunzione", tipoFunzione.IDTipoFunzione);

                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);

                        retValue.Value = dbProvider.ExecuteNonQuery(commandText);
                    }

                    if (retValue.Value)
                        dbProvider.CommitTransaction();
                    else
                        dbProvider.RollbackTransaction();
                }
                catch
                {
                    dbProvider.RollbackTransaction();
                    retValue.Value = false;
                }
                finally
                {
                    dbProvider.Dispose();
                    dbProvider = null;
                }
            }

            return retValue;
        }

        private static bool ContainsRuoli(DocsPaVO.amministrazione.OrgTipoFunzione tipoFunzione)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("AMM_S_GET_COUNT_RUOLI_TIPO_FUNZIONE");
            queryDef.setParam("idTipoFunzione", tipoFunzione.IDTipoFunzione);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string countRuoli;

            using (DBProvider dbProvider = new DBProvider())
            {
                if (dbProvider.ExecuteScalar(out countRuoli, commandText))
                    retValue = (Convert.ToInt32(countRuoli) > 0);
            }

            return retValue;
        }

    }
}
