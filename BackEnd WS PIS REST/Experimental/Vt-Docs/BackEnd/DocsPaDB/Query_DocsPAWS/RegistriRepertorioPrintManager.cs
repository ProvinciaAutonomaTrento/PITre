using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DocsPaUtils;
using DocsPaUtils.Data;
using DocsPaVO.utente.Repertori;
using DocsPaVO.utente;
using DocsPaVO.documento;
using System.Configuration;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Questa classe contiene tutti i metodi per la gestione delle stampe dei registri di repertorio.
    /// </summary>
    public class RegistriRepertorioPrintManager
    {
        private ILog logger = LogManager.GetLogger(typeof(RegistriRepertorioPrintManager));
        

        /// <summary>
        /// Metodo per il recupero delle informazioni sui registri di repertorio registrati 
        /// per una data amministrazione.
        /// </summary>
        /// <param name="administrationId">Id dell'amministrazione di cui recuperare i registri di repertorio</param>
        /// <returns>Lista dei registri di repertorio definiti per l'amminiustrazione specificata</returns>
        public List<RegistroRepertorio> GetRegisteredRegisters(String administrationId)
        {
            // Lista dei registri da restituire
            List<RegistroRepertorio> returnValue = new List<RegistroRepertorio>();

            // Recupero della lista dei registri di repertorio definiti per i documenti
            returnValue.AddRange(this.GetRegisteredRegistries(administrationId, RegistroRepertorio.TipologyKind.D));

            // Recupero della lista dei registri di repertorio definiti per i fascicoli
            //returnValue.AddRange(this.GetRegisteredRegistries(administrationId, RegistroRepertorio.TipologyKind.F));

            return returnValue;
        }

        /// <summary>
        /// Metodo per il recupero dei dettagli minimali relativi alle impostazioni 
        /// </summary>
        /// <param name="counterId">Id del contatore di cui recuperare le impostazioni minimali</param>
        /// <param name="tipologyKind">Tipologia documentale cui si riferisce il contatore di cui recuperare i dettagli minimali</param>
        /// <returns>Lista delle impostazioni minimali</returns>
        public List<RegistroRepertorioSettingsMinimalInfo> GetSettingsMinimalInfo(String counterId, RegistroRepertorio.TipologyKind tipologyKind, string idAmm)
        {
            List<RegistroRepertorioSettingsMinimalInfo> returnValue = new List<RegistroRepertorioSettingsMinimalInfo>();

            Query query = InitQuery.getInstance().getQuery("S_REGISTER_SETTINGS_MINIMAL_INFO");
            query.setParam("counterId", counterId);
            query.setParam("tipologyKind", tipologyKind.ToString());
            query.setParam("idAmm", idAmm);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {

                    while (dataReader.Read())
                        returnValue.Add(new RegistroRepertorioSettingsMinimalInfo()
                            {
                                RegistryId = dataReader["RegistryId"].ToString(),
                                RegistryOrRfDescription = dataReader["RegistryOrRfDescription"].ToString(),
                                RfId = dataReader["RfId"].ToString(),
                                RoleAndUserDescription = dataReader["RoleAndUserDescription"].ToString(),
                                Settings = (RegistroRepertorio.SettingsType)Enum.Parse(typeof(RegistroRepertorio.SettingsType), dataReader["Settings"].ToString()),
                                CounterState = (RegistroRepertorioSingleSettings.RepertorioState)Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataReader["CounterState"].ToString())
                            });
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Metodo per il recupero del dettaglio delle impostazioni di un registro di repertorio
        /// </summary>
        /// <param name="counterId">Id del registro</param>
        /// <param name="registryId">Id del registro</param>
        /// <param name="rfId">Id dell'RF</param>
        /// <param name="tipologyKind">Tipologia documentale cui appartiene il contatore</param>
        /// <param name="settingsType">Tipo di impostazione scelto.</param>
        /// <returns>Registro di repertorio con le varie impostazioni</returns>
        public RegistroRepertorioSingleSettings GetRegisterSettings(String counterId, String registryId, String rfId, RegistroRepertorio.TipologyKind tipologyKind, RegistroRepertorio.SettingsType settingsType)
        {
            RegistroRepertorioSingleSettings returnValue = null;

            Query query = InitQuery.getInstance().getQuery("S_REGISTRY_SETTINGS");

            // Impostazione dell'id del contatore
            query.setParam("counterId", counterId);

            // Se sono state scelte impostazione globali, non vengono aggiunti i filtri su RF e AOO
            if (settingsType == RegistroRepertorio.SettingsType.S)
            {
                String filters = String.Format(" And RegistryId {0} And RfId {1} And TipologyKind = '{2}'",
                    String.IsNullOrEmpty(registryId) ? " is null" : String.Format(" = {0}", registryId),
                    String.IsNullOrEmpty(rfId) ? " is null" : String.Format(" = {0}", rfId),
                    tipologyKind.ToString());
                query.setParam("filter", filters);
            }
            else
                query.setParam("filter", String.Empty);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {

                    if (dataReader != null)
                        while (dataReader.Read())
                        {

                            returnValue = new RegistroRepertorioSingleSettings()
                                {
                                    RegistryId = dataReader["RegistryId"].ToString(),
                                    RFId = dataReader["RFId"].ToString(),
                                    RegistryOrRfDescription = dataReader["RegistryOrRfDescription"].ToString(),
                                    RoleRespId = dataReader["RoleRespId"].ToString(),
                                    PrinterRoleRespId = dataReader["PrinterRoleRespId"].ToString(),
                                    PrinterUserRespId = dataReader["PrinterUserRespId"].ToString(),
                                    RoleAndUserDescription = dataReader["RoleAndUserDescription"].ToString(),
                                    CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataReader["CounterState"].ToString())),
                                    PrintFrequency = (RegistroRepertorioSingleSettings.Frequency)Enum.Parse(typeof(RegistroRepertorioSingleSettings.Frequency), dataReader["PrintFrequency"].ToString()),
                                    LastPrintedNumber = Int32.Parse(dataReader["LastPrintedNumber"].ToString()),
                                    RoleRespRight = (RegistroRepertorioSingleSettings.ResponsableRight)Enum.Parse(typeof(RegistroRepertorioSingleSettings.ResponsableRight), dataReader["RespRights"].ToString())
                                };

                            DateTime dateTime = DateTime.Now;

                            if (DateTime.TryParse(dataReader["DateAutomaticPrintFinish"].ToString(), out dateTime))
                                returnValue.DateAutomaticPrintFinish = dateTime;
                            else
                                returnValue.DateAutomaticPrintFinish = DateTime.Now;

                            if (DateTime.TryParse(dataReader["DateAutomaticPrintStart"].ToString(), out dateTime))
                                returnValue.DateAutomaticPrintStart = dateTime;
                            else
                                returnValue.DateAutomaticPrintStart = DateTime.Now;

                            if (DateTime.TryParse(dataReader["DateLastPrint"].ToString(), out dateTime))
                                returnValue.DateLastPrint = dateTime;
                            else
                                returnValue.DateLastPrint = DateTime.MinValue;

                            if (DateTime.TryParse(dataReader["DateNextAutomaticPrint"].ToString(), out dateTime))
                                returnValue.DateNextAutomaticPrint = dateTime;
                            else
                                returnValue.DateNextAutomaticPrint = DateTime.MinValue;

                            break;
                        }
                }

            }

            return returnValue;
        }

        /// <summary>
        /// Metodo per l'aggiornamento dei dati di dettaglio di un registro di repertorio
        /// </summary>
        /// <param name="counterId">Id del contatore cui appartengono le impostazioni da salvare</param>
        /// <param name="settingsType">Tipo di impostazioni scelte per il contatore (Globali o Singole)</param>
        /// <param name="tipologyKind">Tipologia cui appartiene il contatore</param>
        /// <param name="settings">Impostazioni da salvare</param>
        /// <returns>Esito dell'operazione di salvataggio</returns>
        public bool SaveRegisterSettings(String counterId, RegistroRepertorio.SettingsType settingsType, RegistroRepertorio.TipologyKind tipologyKind, RegistroRepertorioSingleSettings settings)
        {
            bool result = false;

            string dbType = ConfigurationManager.AppSettings["dbType"].ToUpper();

            // Preparazione parametri
            ArrayList parameters = new ArrayList();
            parameters.Add(new ParameterSP("countId", counterId, DirectionParameter.ParamInput));
            parameters.Add(new ParameterSP("settingsType", settingsType.ToString(), DirectionParameter.ParamInput));
            parameters.Add(new ParameterSP("roleIdGroup", String.IsNullOrEmpty(settings.PrinterRoleRespId) ? String.Empty : settings.PrinterRoleRespId, DirectionParameter.ParamInput));
            parameters.Add(new ParameterSP("userIdPeople", String.IsNullOrEmpty(settings.PrinterUserRespId) ? String.Empty : settings.PrinterUserRespId, DirectionParameter.ParamInput));
            parameters.Add(new ParameterSP("roleRespIdGroup", String.IsNullOrEmpty(settings.RoleRespId) ? String.Empty : settings.RoleRespId, DirectionParameter.ParamInput));
            parameters.Add(new ParameterSP("printFrequency", settings.PrintFrequency.ToString(), DirectionParameter.ParamInput));
            //parameters.Add(new ParameterSP("dateAutomaticPrintStart", settings.DateAutomaticPrintStart, DirectionParameter.ParamInput));
            //parameters.Add(new ParameterSP("dateAutomaticPrintFinish", settings.DateAutomaticPrintFinish, DirectionParameter.ParamInput));
            //parameters.Add(new ParameterSP("dateNextAutomaticPrint", settings.DateNextAutomaticPrint, DirectionParameter.ParamInput));
            parameters.Add(new ParameterSP("reg", settings.RegistryId, DirectionParameter.ParamInput));
            parameters.Add(new ParameterSP("rf", settings.RFId, DirectionParameter.ParamInput));
            parameters.Add(new ParameterSP("tipology", tipologyKind.ToString(), DirectionParameter.ParamInput));
            parameters.Add(new ParameterSP("state", settings.CounterState.ToString(), DirectionParameter.ParamInput));
            parameters.Add(new ParameterSP("rights", settings.RoleRespRight.ToString(), DirectionParameter.ParamInput) { Size = 2 });
            if (dbType.ToUpper().Equals("SQL"))
            {
                parameters.Add(new ParameterSP("returnValue", 0, DirectionParameter.ParamOutput));

                // DateAutomaticPrintStart
                if (settings.DateAutomaticPrintStart.ToString().StartsWith("01/01/0001"))
                    parameters.Add(new ParameterSP("dateAutomaticPrintStart", DBNull.Value, DirectionParameter.ParamInput));
                else
                    parameters.Add(new ParameterSP("dateAutomaticPrintStart", settings.DateAutomaticPrintStart, DirectionParameter.ParamInput));

                // DateAutomaticPrintFinish
                if (settings.DateAutomaticPrintFinish.ToString().StartsWith("01/01/0001"))
                    parameters.Add(new ParameterSP("dateAutomaticPrintFinish", DBNull.Value, DirectionParameter.ParamInput));
                else
                    parameters.Add(new ParameterSP("dateAutomaticPrintFinish", settings.DateAutomaticPrintFinish, DirectionParameter.ParamInput));

                // DateNextAutomaticPrint
                if (settings.DateNextAutomaticPrint.ToString().StartsWith("01/01/0001"))
                    parameters.Add(new ParameterSP("dateNextAutomaticPrint", DBNull.Value, DirectionParameter.ParamInput));
                else
                    parameters.Add(new ParameterSP("dateNextAutomaticPrint", settings.DateNextAutomaticPrint, DirectionParameter.ParamInput));
            }
            else
            {
                parameters.Add(new ParameterSP("dateAutomaticPrintStart", settings.DateAutomaticPrintStart, DirectionParameter.ParamInput));
                parameters.Add(new ParameterSP("dateAutomaticPrintFinish", settings.DateAutomaticPrintFinish, DirectionParameter.ParamInput));
                parameters.Add(new ParameterSP("dateNextAutomaticPrint", settings.DateNextAutomaticPrint, DirectionParameter.ParamInput));
            }

            // Esecuzione store
            using (DBProvider dbProvider = new DBProvider())
            {
                result = dbProvider.ExecuteStoreProcedure("SaveCounterSettings", parameters) == 1;
            }

            // restituzione esito dell'operazione
            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <returns></returns>
        public RegistroRepertorioSingleSettings.RepertorioState GetState(String counterId, String registryId, String rfId)
        {
            // Stato della particolare istanza di repertorio
            RegistroRepertorioSingleSettings.RepertorioState retVal = RegistroRepertorioSingleSettings.RepertorioState.O;

            // Costruzione della condizione per la selezione dell'ultimo numero di repertorio da stampare
            //String regList, lastPrinted = "", lastToPrint = "";

            //if (String.IsNullOrEmpty(registryId) && String.IsNullOrEmpty(rfId))
            //    regList = " = 0";
            //else
            //    regList = String.Format(" in ({0}) ", String.IsNullOrEmpty(registryId) ? rfId : registryId);

            Query query = InitQuery.getInstance().getQuery("S_GET_COUNTER_PRINT_INFO");
            query.setParam("counterId", counterId);
            query.setParam("registryId", String.IsNullOrEmpty(registryId) ? " is null" : String.Format("= {0}", registryId));
            query.setParam("rfId", String.IsNullOrEmpty(rfId) ? " is null" : String.Format("= {0}", rfId));
            //query.setParam("regList", regList);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                    {
                        retVal = (RegistroRepertorioSingleSettings.RepertorioState)Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataReader[0].ToString());
                        //lastPrinted = dataReader["LastPrintedNumber"].ToString();
                        //lastToPrint = dataReader["LastNumberToPrint"].ToString();
                    }
                }

            }

            //lastPrintedNumber = lastPrinted;
            //lastNumberToPrint = lastToPrint;
            return retVal;

        }

        public bool UpdatePrinterManager(String counterId, String numStart, String numEnd, String docNumber, String registryId, String rfId, string year)
        {
            bool retVal = false;
            // Recupero query per inserimento dati nel gestore stampe
            Query query = InitQuery.getInstance().getQuery("I_SAVE_PRINT_REPERTORIO");
            query.setParam("counterId", counterId);
            query.setParam("numStart", numStart);
            query.setParam("numEnd", numEnd);
            query.setParam("year", year);
            query.setParam("docNumber", docNumber);

            String regId = "null";
            if (!String.IsNullOrEmpty(registryId))
                regId = registryId;
            else
                if (!String.IsNullOrEmpty(rfId))
                    regId = rfId;

            query.setParam("registryId", regId);

            using (DBProvider dbProvider = new DBProvider())
            {
                retVal = dbProvider.ExecuteNonQuery(query.getSQL());

            }

            return retVal;

        }

        /// <summary>
        /// Metodo per l'aggiornamento dell'ultimo numero stampato
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="numEnd"></param>
        /// <param name="rfId"></param>
        /// <param name="registryId"></param>
        /// <returns></returns>
        public bool UpdateLastPrintendNumber(String counterId, String numEnd, String rfId, String registryId)
        {           
            Query query = InitQuery.getInstance().getQuery("U_LAST_PRINTED_NUMBER_REPERTORI");
            query.setParam("counterId", counterId);
            query.setParam("rfId", String.IsNullOrEmpty(rfId) ? " is null" : String.Format(" = {0} ", rfId));
            query.setParam("registryId", String.IsNullOrEmpty(registryId) ? " is null" : String.Format(" = {0} ", registryId));
            query.setParam("lastPrintedNumber", numEnd);

            // Esecuzione della query
            bool retVal = false;
            using (DBProvider dbProvider = new DBProvider())
            {
                retVal = dbProvider.ExecuteNonQuery(query.getSQL());
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per il caricamento delle informazioni sui registri di repertorio definiti per
        /// una data categoria documentale
        /// </summary>
        /// <param name="administrationId">Id dell'amministrazione di cui recuperare i registri di repertorio</param>
        /// <param name="type">Categoria della tipologia documentale di cui recuperare i registri di repertori</param>
        /// <returns>Lista dei registri di repertorio definiti per le categorie documentali di una data amministrazione</returns>
        private IEnumerable<RegistroRepertorio> GetRegisteredRegistries(string administrationId, RegistroRepertorio.TipologyKind type)
        {
            List<RegistroRepertorio> returnValue = new List<RegistroRepertorio>();

            Query query = InitQuery.getInstance().getQuery(String.Format("S_REGISTERED_REGISTRIES_MIN_INFO_{0}",
                type == RegistroRepertorio.TipologyKind.D ? "DOC" : "PRJ"));
            query.setParam("administrationId", administrationId);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {

                    while (dataReader.Read())
                    {
                        RegistroRepertorio reg = this.ExtractData(dataReader);
                        returnValue.Add(reg);
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Metodo per l'estrazione dei dati base relativi ad un repertorio da un data reader
        /// </summary>
        /// <param name="currentRow">Riga da cui estrarre i dati</param>
        /// <returns>Registro di repertorio con i dati estratti dal data reader</returns>
        private RegistroRepertorio ExtractData(IDataReader currentRow)
        {
            RegistroRepertorio reg = new RegistroRepertorio()
                {
                    CounterId = currentRow["CounterId"].ToString(),
                    CounterDescription = currentRow["CounterDescription"].ToString(),
                    TipologyDescription = currentRow["TipologyDescription"].ToString(),
                    Enabled = Convert.ToBoolean(currentRow["TipologyEnabled"].ToString()),
                    Settings = (RegistroRepertorio.SettingsType)Enum.Parse(typeof(RegistroRepertorio.SettingsType), currentRow["Settings"].ToString())
                };

            reg.Tipology = (RegistroRepertorio.TipologyKind)Enum.Parse(typeof(RegistroRepertorio.TipologyKind), currentRow["Typology"].ToString());

            return reg;
        }

        /// <summary>
        /// Metodo che restituisce i registri di repertorio di cui si è Repsonsabile e Stampatore o solo Responsabile o Stampatore
        /// Ogni oggetto registro di repertorio conterrà al suo interno eventualmente la lista dei registri o rf a cui afferisce
        /// </summary>
        public ArrayList GetRegistriesWithAooOrRf(string idRoleResp, string idRolePrinter)
        {
            ArrayList repertori = new ArrayList();
            string  dbType = ConfigurationManager.AppSettings["dbType"].ToUpper();

            using (DBProvider dbProvider = new DBProvider())
            {
                string command = String.Empty;
                DataSet dataSet;

                //Recupero dei registri di repertorio
                
                command =   "select distinct(counterid), oc.descrizione as o_desc, ta.var_desc_atto as t_desc " +
                            "from dpa_registri_repertorio rp, dpa_oggetti_custom oc , dpa_tipo_atto ta " +
                            "where ";
                            if (!String.IsNullOrEmpty(idRoleResp) && !String.IsNullOrEmpty(idRolePrinter))
                                command += "(rp.rolerespid = " + idRoleResp + " or rp.printerrolerespid = " + idRolePrinter + ") ";
                            else if (!String.IsNullOrEmpty(idRoleResp))
                                command += "rp.rolerespid = " + idRoleResp + " ";
                            else if (!String.IsNullOrEmpty(idRolePrinter))
                                command += "rp.printerrolerespid = " + idRolePrinter + " ";
                            command += "and " +
                                        "rp.counterid = oc.system_id " +
                                        "and " +
                                        "rp.tipologyid = ta.system_id " +
                                        "order by t_desc asc";

                dataSet = new DataSet();
                dbProvider.ExecuteQuery(dataSet, command);

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    RegistroRepertorio rep = new RegistroRepertorio();
                    rep.CounterId = dataSet.Tables[0].Rows[i]["COUNTERID"].ToString();
                    rep.CounterDescription = dataSet.Tables[0].Rows[i]["O_DESC"].ToString();
                    rep.TipologyDescription = dataSet.Tables[0].Rows[i]["T_DESC"].ToString();
                    rep.SingleSettings = new List<RegistroRepertorioSingleSettings>();
                    repertori.Add(rep);
                }

                //Recupero eventuali dettagli del repertorio di contatore
                if (repertori.Count != 0)
                {
                    foreach (RegistroRepertorio rep in repertori)
                    {
                        command = "select rp.counterstate, ";
                        if (dbType.ToUpper() == "SQL")
                            command += "convert(nvarchar (10), rp.DTALASTPRINT, 103) AS data_last_stampa, ";
                        else
                            command += "TO_CHAR(rp.DTALASTPRINT, 'DD/MM/YYYY HH24:MM') || '' as data_last_stampa, ";
                        
                        command += "(select var_desc_corr from dpa_corr_globali where id_gruppo=rp.rolerespid) as ruolo_responsabile " +
                                    "from dpa_registri_repertorio rp " +
                                    "where ";
                        if (!String.IsNullOrEmpty(idRoleResp) && !String.IsNullOrEmpty(idRolePrinter))
                            command += "(rp.rolerespid = " + idRoleResp + " or rp.printerrolerespid = " + idRolePrinter + ") ";
                        else if (!String.IsNullOrEmpty(idRoleResp))
                            command += "rp.rolerespid = " + idRoleResp + " ";
                        else if (!String.IsNullOrEmpty(idRolePrinter))
                            command += "rp.printerrolerespid = " + idRolePrinter + " ";
                        command += "and " +
                                    "rp.counterid = " + rep.CounterId + " " +
                                    "and " +
                                    "rp.registryid is null " +
                                    "and " +
                                    "rp.rfid is null ";

                        dataSet = new DataSet();
                        dbProvider.ExecuteQuery(dataSet, command);

                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            RegistroRepertorioSingleSettings repSingleSetting = new RegistroRepertorioSingleSettings();
                            repSingleSetting.CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataSet.Tables[0].Rows[i]["counterstate"].ToString()));
                            DateTime dtalastprint;
                            DateTime.TryParse(dataSet.Tables[0].Rows[i]["DATA_LAST_STAMPA"].ToString(),out dtalastprint);
                            repSingleSetting.DateLastPrint = dtalastprint;
                            repSingleSetting.RoleAndUserDescription = dataSet.Tables[0].Rows[i]["RUOLO_RESPONSABILE"].ToString();
                            rep.SingleSettings.Add(repSingleSetting);
                        }
                    }
                }

                //Recupero eventuali dettagli dei registri(AOO) associati al registro di repertorio 
                if (repertori.Count != 0)
                {
                    foreach (RegistroRepertorio rep in repertori)
                    {
                        if (dbType.ToUpper().Equals("SQL"))
                            command = "select rp.registryid, '[' + dr.var_codice + ']' + dr.var_desc_registro as description , rp.counterstate, " + 
                                    "convert(nvarchar (10), rp.DTALASTPRINT, 103) AS data_last_stampa, ";
                        else
                            command = "select rp.registryid, '[' || dr.var_codice || ']' || dr.var_desc_registro as description , rp.counterstate, " +
                                    "TO_CHAR(rp.DTALASTPRINT, 'DD/MM/YYYY HH24:MM') || '' as data_last_stampa, ";

                        command += "(select var_desc_corr from dpa_corr_globali where id_gruppo=rp.rolerespid) as ruolo_responsabile " +
                                    "from dpa_registri_repertorio rp, dpa_el_registri dr " +
                                    "where ";
                        if (!String.IsNullOrEmpty(idRoleResp) && !String.IsNullOrEmpty(idRolePrinter))
                            command += "(rp.rolerespid = " + idRoleResp + " or rp.printerrolerespid = " + idRolePrinter + ") ";
                        else if (!String.IsNullOrEmpty(idRoleResp))
                            command += "rp.rolerespid = " + idRoleResp + " ";
                        else if (!String.IsNullOrEmpty(idRolePrinter))
                            command += "rp.printerrolerespid = " + idRolePrinter + " and dr.id_amm in (select id_amm from dpa_corr_globali where id_gruppo=rp.printerrolerespid) ";
                        command += "and " +
                                    "rp.counterid = " + rep.CounterId + " " +
                                    "and " +
                                    "rp.registryid = dr.system_id " +
                                    "order by dr.var_desc_registro asc";

                        dataSet = new DataSet();
                        dbProvider.ExecuteQuery(dataSet, command);

                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            RegistroRepertorioSingleSettings repSingleSetting = new RegistroRepertorioSingleSettings();
                            repSingleSetting.RegistryId = dataSet.Tables[0].Rows[i]["REGISTRYID"].ToString();
                            repSingleSetting.RegistryOrRfDescription = dataSet.Tables[0].Rows[i]["DESCRIPTION"].ToString();
                            repSingleSetting.CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataSet.Tables[0].Rows[i]["counterstate"].ToString()));
                            DateTime dtalastprint;
                            DateTime.TryParse(dataSet.Tables[0].Rows[i]["DATA_LAST_STAMPA"].ToString(), out dtalastprint);
                            repSingleSetting.DateLastPrint = dtalastprint;
                            repSingleSetting.RoleAndUserDescription = dataSet.Tables[0].Rows[i]["RUOLO_RESPONSABILE"].ToString();
                            rep.SingleSettings.Add(repSingleSetting);
                        }
                    }
                }


                //Recupero eventuali dettagli degli RF associati al registro di repertorio 
                if (repertori.Count != 0)
                {
                    foreach (RegistroRepertorio rep in repertori)
                    {
                        
                        if (dbType.ToUpper().Equals("SQL"))
                            command = "select rp.rfid, '[' + dr.var_codice + ']' + dr.var_desc_registro as description , rp.counterstate, " + 
                                    "convert(nvarchar (10), rp.DTALASTPRINT, 103) AS data_last_stampa, ";
                        else
                            command = "select rp.rfid, '[' || dr.var_codice || ']' || dr.var_desc_registro as description , rp.counterstate, " + 
                                    "TO_CHAR(rp.DTALASTPRINT, 'DD/MM/YYYY HH24:MM') || '' as data_last_stampa, ";

                        command += "(select var_desc_corr from dpa_corr_globali where id_gruppo=rp.rolerespid) as ruolo_responsabile " +
                                    "from dpa_registri_repertorio rp, dpa_el_registri dr " +
                                    "where ";
                        if (!String.IsNullOrEmpty(idRoleResp) && !String.IsNullOrEmpty(idRolePrinter))
                            command += "(rp.rolerespid = " + idRoleResp + " or rp.printerrolerespid = " + idRolePrinter + ") ";
                        else if (!String.IsNullOrEmpty(idRoleResp))
                            command += "rp.rolerespid = " + idRoleResp + " ";
                        else if (!String.IsNullOrEmpty(idRolePrinter))
                            command += "rp.printerrolerespid = " + idRolePrinter + " and dr.id_amm in (select id_amm from dpa_corr_globali where id_gruppo=rp.printerrolerespid) ";
                        command += "and " +
                                    "rp.counterid = " + rep.CounterId + " " +
                                    "and " +
                                    "rp.rfid = dr.system_id " +
                                    "order by dr.var_desc_registro asc";

                        dataSet = new DataSet();
                        dbProvider.ExecuteQuery(dataSet, command);

                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            RegistroRepertorioSingleSettings repSingleSetting = new RegistroRepertorioSingleSettings();
                            repSingleSetting.RFId = dataSet.Tables[0].Rows[i]["RFID"].ToString();
                            repSingleSetting.RegistryOrRfDescription = dataSet.Tables[0].Rows[i]["DESCRIPTION"].ToString();
                            repSingleSetting.CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataSet.Tables[0].Rows[i]["counterstate"].ToString()));
                            DateTime dtalastprint;
                            DateTime.TryParse(dataSet.Tables[0].Rows[i]["DATA_LAST_STAMPA"].ToString(), out dtalastprint);
                            repSingleSetting.DateLastPrint = dtalastprint;
                            repSingleSetting.RoleAndUserDescription = dataSet.Tables[0].Rows[i]["RUOLO_RESPONSABILE"].ToString();
                            rep.SingleSettings.Add(repSingleSetting);
                        }
                    }
                }

            }
            return repertori;
        }

        public string ElencoRuoliSuperiori(string idRolePrinter)
        {
            string result = idRolePrinter + ",";
            using (DBProvider dbProvider = new DBProvider())
            {
                DataSet dataSet;
                string command = "SELECT d.id_gruppo " +
                    " FROM dpa_corr_globali d, dpa_tipo_ruolo c " +
                    "WHERE d.cha_tipo_urp = 'R' " +
                    "AND d.id_uo IN (SELECT     system_id " +
                    "FROM dpa_corr_globali " +
                    "CONNECT BY PRIOR id_parent = system_id " +
                    "START WITH system_id = (SELECT id_uo " +
                    "FROM dpa_corr_globali " +
                    "WHERE id_gruppo = " + idRolePrinter + ")) " +
                    "AND c.system_id = d.id_tipo_ruolo " +
                    "AND c.num_livello >= (SELECT c.num_livello " +
                    "FROM dpa_corr_globali d, dpa_tipo_ruolo c " +
                    "WHERE c.system_id = d.id_tipo_ruolo AND d.id_gruppo = " + idRolePrinter + ")";

                dataSet = new DataSet();
                dbProvider.ExecuteQuery(dataSet, command);
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        if(!String.IsNullOrEmpty(dataSet.Tables[0].Rows[i]["id_gruppo"].ToString()))
                            result += dataSet.Tables[0].Rows[i]["id_gruppo"].ToString() + ",";
                        else
                            result += "0" + ",";
                    }
                }
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }

        // Ritorna l'elenco dei ruoli sottoposti rispetto a quello passato
        // filtra per i soli ruoli possibili tra i stampatori dei repetori (gli altri non mi interessano)
        // connect by prior invertito rispetto al superiori
        public string ElencoRuoliSottoposti(string idRole)
        {
            string result = idRole + ",";
            string  dbType = ConfigurationManager.AppSettings["dbType"].ToUpper();
            using (DBProvider dbProvider = new DBProvider())
            {
                DataSet dataSet;
                string command = string.Empty; 

                if(dbType.ToUpper().Equals("SQL"))
                    command += "WITH Chain(system_id, id_parent, id_uo) AS (" +
                        "SELECT system_id, id_parent, id_uo " +
                        "FROM dpa_corr_globali " +
                        "WHERE id_gruppo = " + idRole + " " +
                        "UNION ALL " +
                        "SELECT cg.system_id, cg.id_parent, cg.id_uo " +
                        "FROM dpa_corr_globali as cg, Chain " +
                        "where Chain.system_id = cg.id_parent " +
                        ") SELECT d.id_gruppo " +
                     " FROM dpa_corr_globali d, dpa_tipo_ruolo c " +
                     "WHERE d.cha_tipo_urp = 'R' and dta_fine is null " +
                     "AND id_gruppo in (select printerrolerespid from dpa_registri_repertorio) " +
                     "AND d.id_uo IN ( SELECT id_uo FROM Chain ) ";
                else
                    command += "SELECT d.id_gruppo " +
                     " FROM dpa_corr_globali d, dpa_tipo_ruolo c " +
                     "WHERE d.cha_tipo_urp = 'R' and dta_fine is null " +
                     "AND id_gruppo in (select printerrolerespid from dpa_registri_repertorio) " +
                     "AND d.id_uo IN ( SELECT system_id " +
                        "FROM dpa_corr_globali " +
                        "CONNECT BY PRIOR system_id = id_parent " +
                        "START WITH system_id = (SELECT id_uo " +
                        "FROM dpa_corr_globali " +
                        "WHERE id_gruppo = " + idRole + ")) ";

                command += "AND c.system_id = d.id_tipo_ruolo " +
                    "AND c.num_livello >= (SELECT c.num_livello " +
                    "FROM dpa_corr_globali d, dpa_tipo_ruolo c " +
                    "WHERE c.system_id = d.id_tipo_ruolo AND d.id_gruppo = " + idRole + ")";

                dataSet = new DataSet();        
                dbProvider.ExecuteQuery(dataSet, command);
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++) 
                    {
                        if (!String.IsNullOrEmpty(dataSet.Tables[0].Rows[i]["id_gruppo"].ToString()))
                            result += dataSet.Tables[0].Rows[i]["id_gruppo"].ToString() + ",";
                        else
                            result += "0" + ",";
                    }
                }
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }
        
        /// <summary>
        /// Metodo che restituisce i registri di repertorio di cui si è Repsonsabile e Stampatore o solo Responsabile o Stampatore
        /// o un superiore di del stampatore.
        /// Ogni oggetto registro di repertorio conterrà al suo interno eventualmente la lista dei registri o rf a cui afferisce
        /// </summary>
        public ArrayList GetRegistriesWithAooOrRfSup(string idRoleResp, string idRolePrinter)
        {
            ArrayList repertori = new ArrayList();
           
            //string ruoliSottoposti = ElencoRuoliSottoposti(idRolePrinter);
            string ruoliSottoposti = string.Empty;
            if (!string.IsNullOrEmpty(idRolePrinter))
                ruoliSottoposti = ElencoRuoliSottoposti(idRolePrinter);
            else
                ruoliSottoposti = "''";
            
            using (DBProvider dbProvider = new DBProvider())
            {
                string command = String.Empty;
                DataSet dataSet;
                
                //Recupero dei registri di repertorio

                command = "select distinct(counterid), oc.descrizione as o_desc, ta.var_desc_atto as t_desc " +
                            "from dpa_registri_repertorio rp, dpa_oggetti_custom oc , dpa_tipo_atto ta " +
                            "where ";
                if (!String.IsNullOrEmpty(idRoleResp) && !String.IsNullOrEmpty(idRolePrinter))
                    command += "(rp.rolerespid = " + idRoleResp + " or rp.printerrolerespid in (" + ruoliSottoposti + ")) and ";
                else if (!String.IsNullOrEmpty(idRoleResp))
                    command += "rp.rolerespid = " + idRoleResp + " and ";
                else if (!String.IsNullOrEmpty(idRolePrinter))
                    command += "rp.printerrolerespid = " + idRolePrinter + " and ";
                command += //"and " +
                            "rp.counterid = oc.system_id " +
                            "and " +
                            "rp.tipologyid = ta.system_id " +
                            "order by t_desc asc";

                dataSet = new DataSet();
                dbProvider.ExecuteQuery(dataSet, command);

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    RegistroRepertorio rep = new RegistroRepertorio();
                    rep.CounterId = dataSet.Tables[0].Rows[i]["COUNTERID"].ToString();
                    rep.CounterDescription = dataSet.Tables[0].Rows[i]["O_DESC"].ToString();
                    rep.TipologyDescription = dataSet.Tables[0].Rows[i]["T_DESC"].ToString();
                    rep.SingleSettings = new List<RegistroRepertorioSingleSettings>();
                    repertori.Add(rep);
                }

                //Recupero eventuali dettagli del repertorio di contatore
                if (repertori.Count != 0)
                {
                    foreach (RegistroRepertorio rep in repertori)
                    {
                        command = "select rp.counterstate " +
                                    "from dpa_registri_repertorio rp " +
                                    "where ";
                        if (!String.IsNullOrEmpty(idRoleResp) && !String.IsNullOrEmpty(idRolePrinter))
                            command += "(rp.rolerespid = " + idRoleResp + " or rp.printerrolerespid in (" + ruoliSottoposti + ")) and ";
                        else if (!String.IsNullOrEmpty(idRoleResp))
                            command += "rp.rolerespid = " + idRoleResp + " and ";
                        else if (!String.IsNullOrEmpty(idRolePrinter))
                            command += "rp.printerrolerespid = " + idRolePrinter + " and ";
                        command += //"and " +
                                    "rp.counterid = " + rep.CounterId + " " +
                                    "and " +
                                    "rp.registryid is null " +
                                    "and " +
                                    "rp.rfid is null ";

                        dataSet = new DataSet();
                        dbProvider.ExecuteQuery(dataSet, command);

                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            RegistroRepertorioSingleSettings repSingleSetting = new RegistroRepertorioSingleSettings();
                            repSingleSetting.CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataSet.Tables[0].Rows[i]["counterstate"].ToString()));
                            rep.SingleSettings.Add(repSingleSetting);
                        }
                    }
                }

                //Recupero eventuali dettagli dei registri(AOO) associati al registro di repertorio 
                if (repertori.Count != 0)
                {
                    foreach (RegistroRepertorio rep in repertori)
                    {
                        command = "select rp.registryid, dr.var_desc_registro , rp.counterstate " +
                                    "from dpa_registri_repertorio rp, dpa_el_registri dr " +
                                    "where ";
                        if (!String.IsNullOrEmpty(idRoleResp) && !String.IsNullOrEmpty(idRolePrinter))
                            command += "(rp.rolerespid = " + idRoleResp + " or rp.printerrolerespid in (" + ruoliSottoposti + ")) and dr.id_amm in (select id_amm from dpa_corr_globali where id_gruppo=" + idRoleResp + ") and ";
                        else if (!String.IsNullOrEmpty(idRoleResp))
                            command += "rp.rolerespid = " + idRoleResp + " and ";
                        else if (!String.IsNullOrEmpty(idRolePrinter))
                            command += "rp.printerrolerespid = " + idRolePrinter + " and ";
                        command += //"and " +
                                    "rp.counterid = " + rep.CounterId + " " +
                                    "and " +
                                    "rp.registryid = dr.system_id " +
                                    "order by dr.var_desc_registro asc";

                        dataSet = new DataSet();
                        dbProvider.ExecuteQuery(dataSet, command);

                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            RegistroRepertorioSingleSettings repSingleSetting = new RegistroRepertorioSingleSettings();
                            repSingleSetting.RegistryId = dataSet.Tables[0].Rows[i]["REGISTRYID"].ToString();
                            repSingleSetting.RegistryOrRfDescription = dataSet.Tables[0].Rows[i]["VAR_DESC_REGISTRO"].ToString();
                            repSingleSetting.CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataSet.Tables[0].Rows[i]["counterstate"].ToString()));
                            rep.SingleSettings.Add(repSingleSetting);
                        }
                    }
                }


                //Recupero eventuali dettagli degli RF associati al registro di repertorio 
                if (repertori.Count != 0)
                {
                    foreach (RegistroRepertorio rep in repertori)
                    {
                        command = "select rp.rfid, dr.var_desc_registro , rp.counterstate " +
                                    "from dpa_registri_repertorio rp, dpa_el_registri dr " +
                                    "where ";
                        if (!String.IsNullOrEmpty(idRoleResp) && !String.IsNullOrEmpty(idRolePrinter))
                            command += "(rp.rolerespid = " + idRoleResp + " or rp.printerrolerespid in (" + ruoliSottoposti + ")) and dr.id_amm in (select id_amm from dpa_corr_globali where id_gruppo=" + idRoleResp + ") and ";
                        else if (!String.IsNullOrEmpty(idRoleResp))
                            command += "rp.rolerespid = " + idRoleResp + " and ";
                        else if (!String.IsNullOrEmpty(idRolePrinter))
                            command += "rp.printerrolerespid = " + idRolePrinter + " and ";
                        command += //"and " +
                                    "rp.counterid = " + rep.CounterId + " " +
                                    "and " +
                                    "rp.rfid = dr.system_id " +
                                    "order by dr.var_desc_registro asc";

                        dataSet = new DataSet();
                        dbProvider.ExecuteQuery(dataSet, command);

                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            RegistroRepertorioSingleSettings repSingleSetting = new RegistroRepertorioSingleSettings();
                            repSingleSetting.RFId = dataSet.Tables[0].Rows[i]["RFID"].ToString();
                            repSingleSetting.RegistryOrRfDescription = dataSet.Tables[0].Rows[i]["VAR_DESC_REGISTRO"].ToString();
                            repSingleSetting.CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataSet.Tables[0].Rows[i]["counterstate"].ToString()));
                            rep.SingleSettings.Add(repSingleSetting);
                        }
                    }
                }

            }
            return repertori;
        }


        /// <summary>
        /// Metodo che restituisce tutti i registri di repertorio dell'amministrazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public ArrayList GetRegistriesRespCons(string idAmm)
        {
            ArrayList repertori = new ArrayList();

            //string ruoliSottoposti = ElencoRuoliSottoposti(idRolePrinter);
            string ruoliSottoposti = string.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                string command = String.Empty;
                DataSet dataSet;

                //Recupero dei registri di repertorio

                command = "select distinct(counterid), oc.descrizione as o_desc, ta.var_desc_atto as t_desc " +
                            "from dpa_registri_repertorio rp, dpa_oggetti_custom oc , dpa_tipo_atto ta " +
                            "where ";
               
                command += 
                            "rp.counterid = oc.system_id " +
                            "and " +
                            "rp.tipologyid = ta.system_id " +
                            "and " +
                            "ta.id_amm=" + idAmm +
                            " order by t_desc asc";

                dataSet = new DataSet();
                dbProvider.ExecuteQuery(dataSet, command);

                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    RegistroRepertorio rep = new RegistroRepertorio();
                    rep.CounterId = dataSet.Tables[0].Rows[i]["COUNTERID"].ToString();
                    rep.CounterDescription = dataSet.Tables[0].Rows[i]["O_DESC"].ToString();
                    rep.TipologyDescription = dataSet.Tables[0].Rows[i]["T_DESC"].ToString();
                    rep.SingleSettings = new List<RegistroRepertorioSingleSettings>();
                    repertori.Add(rep);
                }

                //Recupero eventuali dettagli del repertorio di contatore
                if (repertori.Count != 0)
                {
                    foreach (RegistroRepertorio rep in repertori)
                    {
                        command = "select rp.counterstate " +
                                    "from dpa_registri_repertorio rp " +
                                    "where ";
                        command += //"and " +
                                    "rp.counterid = " + rep.CounterId + " " +
                                    "and " +
                                    "rp.registryid is null " +
                                    "and " +
                                    "rp.rfid is null ";

                        dataSet = new DataSet();
                        dbProvider.ExecuteQuery(dataSet, command);

                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            RegistroRepertorioSingleSettings repSingleSetting = new RegistroRepertorioSingleSettings();
                            repSingleSetting.CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataSet.Tables[0].Rows[i]["counterstate"].ToString()));
                            rep.SingleSettings.Add(repSingleSetting);
                        }
                    }
                }

                //Recupero eventuali dettagli dei registri(AOO) associati al registro di repertorio 
                if (repertori.Count != 0)
                {
                    foreach (RegistroRepertorio rep in repertori)
                    {
                        command = "select rp.registryid, dr.var_desc_registro , rp.counterstate " +
                                    "from dpa_registri_repertorio rp, dpa_el_registri dr " +
                                    "where ";
                        command += //"and " +
                                    "rp.counterid = " + rep.CounterId + " " +
                                    "and " +
                                    "rp.registryid = dr.system_id " +
                                    "and " +
                                    "dr.id_amm=" + idAmm + " " +
                                    "order by dr.var_desc_registro asc";

                        dataSet = new DataSet();
                        dbProvider.ExecuteQuery(dataSet, command);

                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            RegistroRepertorioSingleSettings repSingleSetting = new RegistroRepertorioSingleSettings();
                            repSingleSetting.RegistryId = dataSet.Tables[0].Rows[i]["REGISTRYID"].ToString();
                            repSingleSetting.RegistryOrRfDescription = dataSet.Tables[0].Rows[i]["VAR_DESC_REGISTRO"].ToString();
                            repSingleSetting.CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataSet.Tables[0].Rows[i]["counterstate"].ToString()));
                            rep.SingleSettings.Add(repSingleSetting);
                        }
                    }
                }


                //Recupero eventuali dettagli degli RF associati al registro di repertorio 
                if (repertori.Count != 0)
                {
                    foreach (RegistroRepertorio rep in repertori)
                    {
                        command = "select rp.rfid, dr.var_desc_registro , rp.counterstate " +
                                    "from dpa_registri_repertorio rp, dpa_el_registri dr " +
                                    "where ";
                        command += //"and " +
                                    "rp.counterid = " + rep.CounterId + " " +
                                    "and " +
                                    "rp.rfid = dr.system_id " +
                                    "and " +
                                    "dr.id_amm=" + idAmm + " " +
                                    "order by dr.var_desc_registro asc";

                        dataSet = new DataSet();
                        dbProvider.ExecuteQuery(dataSet, command);

                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            RegistroRepertorioSingleSettings repSingleSetting = new RegistroRepertorioSingleSettings();
                            repSingleSetting.RFId = dataSet.Tables[0].Rows[i]["RFID"].ToString();
                            repSingleSetting.RegistryOrRfDescription = dataSet.Tables[0].Rows[i]["VAR_DESC_REGISTRO"].ToString();
                            repSingleSetting.CounterState = (RegistroRepertorioSingleSettings.RepertorioState)(Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataSet.Tables[0].Rows[i]["counterstate"].ToString()));
                            rep.SingleSettings.Add(repSingleSetting);
                        }
                    }
                }

            }
            return repertori;
        }

        /// <summary>
        /// Metodo per il recupero dell'id della tipologia associata al contatore
        /// </summary>
        /// <param name="counterId">Id del contatore</param>
        /// <returns>Id della tipologia</returns>
        public String GetCounterTypology(String counterId)
        {
            Query query = InitQuery.getInstance().getQuery("S_COUNTER_TYPOLOGY");
            query.setParam("counterId", counterId);

            String retVal = String.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteScalar(out retVal, query.getSQL());
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per il salvataggio dello stato di un registro di repertorio
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool SaveCounterState(String counterId, String rfId, String registryId, RegistroRepertorioSingleSettings.RepertorioState state)
        {
            bool retVal = true;
            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("U_SAVE_REPERTORIO_STATE");
                query.setParam("counterId", counterId);
                query.setParam("counterState", state.ToString());
                query.setParam("rfId", String.IsNullOrEmpty(rfId) ? " is null " : String.Format(" = {0} ", rfId));
                query.setParam("registryId", String.IsNullOrEmpty(registryId) ? " is null " : String.Format(" = {0} ", registryId));

                retVal = dbProvider.ExecuteNonQuery(query.getSQL());

            }

            return retVal;

        }

        /// <summary>
        /// Metodo per il recupero dell'id dell'eventuale responsabile di un contatore di repertorio in una dele sue istanze
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="rfId"></param>
        /// <param name="registryId"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        public String GetResponsableRoleId(String counterId, String rfId, String registryId, out RegistroRepertorioSingleSettings.ResponsableRight rights)
        {
            Query query = InitQuery.getInstance().getQuery("S_REPERTORIO_RESP_ID");
            query.setParam("counterId", counterId);
            query.setParam("rfId", String.IsNullOrEmpty(rfId) ? " is null " : String.Format(" = {0} ", rfId));
            query.setParam("registryId", String.IsNullOrEmpty(registryId) ? " is null " : String.Format(" = {0} ", registryId));

            rights = RegistroRepertorioSingleSettings.ResponsableRight.R;

            String retVal = String.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                    {
                        retVal = dataReader["RespId"].ToString();
                        rights = (RegistroRepertorioSingleSettings.ResponsableRight)Enum.Parse(typeof(RegistroRepertorioSingleSettings.ResponsableRight), dataReader["Rights"].ToString());
                    }
                }
            }

            return retVal;
        }

        public String GetResponsableRoleIdFromIdTemplate(String idTemplate, String rfId, String registryId, out RegistroRepertorioSingleSettings.ResponsableRight rights, out string idRespCorr)
        {
            Query query = InitQuery.getInstance().getQuery("S_REPERTORIO_RESP_ID_FROM_IDTEMPLATE");
            query.setParam("idTemplate", idTemplate);
            query.setParam("rfId", String.IsNullOrEmpty(rfId) ? " is null " : String.Format(" = {0} ", rfId));
            query.setParam("registryId", String.IsNullOrEmpty(registryId) ? " is null " : String.Format(" = {0} ", registryId));
            idRespCorr = "";
            rights = RegistroRepertorioSingleSettings.ResponsableRight.R;

            String retVal = String.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                    {
                        retVal = dataReader["RespId"].ToString();
                        idRespCorr = dataReader["RespIdCorr"].ToString();
                        rights = (RegistroRepertorioSingleSettings.ResponsableRight)Enum.Parse(typeof(RegistroRepertorioSingleSettings.ResponsableRight), dataReader["Rights"].ToString());
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <returns></returns>
        public bool ExistsModifiedDocsAfterLastPrint(String counterId, String registryId, String rfId)
        {

            Query query = InitQuery.getInstance().getQuery("S_NUM_REPERTORI_MODIFICATI");
            query.setParam("counterId", counterId);

            String regId = String.Empty;

            if (String.IsNullOrEmpty(registryId))
                regId = rfId;
            else
                regId = registryId;

            query.setParam("regId", String.IsNullOrEmpty(regId) ? " is null " : String.Format(" = {0} ", regId));

            String retVal = String.Empty;
            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteScalar(out retVal, query.getSQL());

            }

            return !String.IsNullOrEmpty(retVal) && retVal != "0";

        }

        /// <summary>
        /// Metodo per il recupero delle informazioni sui repertori da stampare.
        /// </summary>
        /// <returns>Lista con le informazioni necessarie per invocare la stampa dei repertori</returns>
        public List<RegistroRepertorioPrint> GetRegistersToPrint(bool repairBrokenPrint)
        {
            Query query = null;

            
            if (repairBrokenPrint)
                query = InitQuery.getInstance().getQuery("S_REGISTERS_REPAIR_PRINT");// Recupero delle informazioni sulle stampe che devono essere riaparate
            else
                query = InitQuery.getInstance().getQuery("S_REGISTERS_MINIMAL_SETTINGS");// Recupero delle informazioni sulle stampe che devono partire oggi

            List<RegistroRepertorioPrint> retVal = new List<RegistroRepertorioPrint>();
            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    while (dataReader.Read())
                    {
                        RegistroRepertorioPrint reg = new RegistroRepertorioPrint()
                            {
                                CounterId = dataReader["CounterId"].ToString(),
                                CounterDescription = dataReader["CounterDescription"].ToString(),
                                TipologyDescription = dataReader["TipologyDescription"].ToString(),
                                RegistryId = dataReader["RegistryId"].ToString(),
                                RFId = dataReader["RFId"].ToString(),
                                CounterState = (RegistroRepertorioSingleSettings.RepertorioState)Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataReader["CounterState"].ToString()),
                                PrintFrequency = (RegistroRepertorioSingleSettings.Frequency)Enum.Parse(typeof(RegistroRepertorioSingleSettings.Frequency), dataReader["PrintFrequency"].ToString())
                            };

                        // Lettura della data di prossima stampa
                        reg.NextAutomaticPrint = (repairBrokenPrint?DateTime.Now:DateTime.Parse(dataReader["DtaNextAutomaticPrint"].ToString()));

                        // Recupero delle informazioni sul ruolo stampatore
                        Utenti users = new Utenti();
                        reg.PrinterRole = users.getRuoloById(dataReader["PrinterRoleCorrGlob"].ToString());

                        // Caricamento dei registri associati al ruolo
                        reg.PrinterRole.registri = users.GetRegistriRuolo(reg.PrinterRole.systemId);

                        // Recupero delle informazioni sull'utente responsabile
                        reg.PrinterUser = users.GetUtente(dataReader["PrinterUserRespId"].ToString());

                        retVal.Add(reg);

                    }
                }

            }

            return retVal;

        }

        /// <summary>
        /// Metodo per il salvataggio della prossima data di stampa automatica
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <param name="date"></param>
        public void SaveNextAutomaticPrintDate(String counterId, String registryId, String rfId, DateTime date)
        {
            Query query = InitQuery.getInstance().getQuery("U_NEXT_AUTOMATIC_PRINT_REPERTORI");
            query.setParam("counterId", counterId);
            string dbType = string.Empty;
            dbType = ConfigurationManager.AppSettings["DBType"];
            if (!string.IsNullOrEmpty(dbType) && dbType.ToLower().Equals("sql"))
                query.setParam("date", String.Format("convert(date, '{0}', 111)", date.ToString("yyyy/MM/dd")));
            else
                query.setParam("date", String.Format("to_date('{0}', 'yyyy/mm/dd')", date.ToString("yyyy/MM/dd")));

            query.setParam("registryId", String.IsNullOrEmpty(registryId) ? " is null " : String.Format(" = {0} ", registryId));
            query.setParam("rfId", String.IsNullOrEmpty(rfId) ? " is null " : String.Format(" = {0} ", rfId));

            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteNonQuery(query.getSQL());
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="docNumber"></param>
        /// <param name="counterId"></param>
        /// <param name="rfId"></param>
        /// <param name="registryId"></param>
        /// <returns></returns>
        public bool AssignDocumentVisibilityToResponsable(InfoUtente userInfo, String docNumber, String counterId, String rfId, String registryId)
        {
            // Recupero dell'id del ruolo responsabile e dei diritti da assegnargli
            RegistroRepertorioSingleSettings.ResponsableRight rights = RegistroRepertorioSingleSettings.ResponsableRight.R;
            String responsableIdGroup = this.GetResponsableRoleId(counterId, rfId, registryId, out rights);


            Documenti docs = new Documenti();
            bool retVal = true;
            if (!String.IsNullOrEmpty(responsableIdGroup))
                retVal = docs.AddPermissionToRole(new DirittoOggetto()
                {
                    accessRights = rights == RegistroRepertorioSingleSettings.ResponsableRight.R ? 45 : 63,
                    personorgroup = responsableIdGroup,
                    tipoDiritto = TipoDiritto.TIPO_ACQUISITO,
                    idObj = docNumber,
                    soggetto = new Ruolo() { tipoCorrispondente = "R", idGruppo = responsableIdGroup }
                });

            return retVal;

        }

        /// <summary>
        /// Metodo per l'assegnazione della visibilità di un documento repertoriato al responsabile del repertorio
        /// </summary>
        /// <param name="counterType">Tipo di contatore</param>
        /// <param name="userInfo">Informazioni sull'utente richiedente</param>
        /// <param name="docNumber">Identificativo del documento la cui visibilità va assegnata al responsabile di repertorio</param>
        /// <param name="counterId">Id del contatore</param>
        /// <param name="registryRfId">Id del registro o RF per cui è definito il contatore</param>
        /// <returns>Esito dell'operazione</returns>
        public bool AssignRepertorioVisibilityToResponsableRole(String counterType, InfoUtente userInfo, String docNumber, String counterId, String registryRfId)
        {
            bool retVal = false;

            switch (counterType)
            {
                case "T":
                    retVal = this.AssignDocumentVisibilityToResponsable(userInfo, docNumber, counterId, null, null);
                    break;

                case "A":
                    retVal = this.AssignDocumentVisibilityToResponsable(userInfo, docNumber, counterId, null, registryRfId);
                    break;

                case "R":
                    retVal = this.AssignDocumentVisibilityToResponsable(userInfo, docNumber, counterId, registryRfId, null);
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per il salvataggio dello stato di un repertorio in tutte le sue istanze
        /// </summary>
        /// <param name="counterId">Id del contatore</param>
        /// <param name="state">Stato</param>
        /// <returns>Esito dell'operazione</returns>
        public bool SaveCounterStateInAllInstances(String counterId, RegistroRepertorioSingleSettings.RepertorioState state)
        {
            bool retVal = false;
            Query query = InitQuery.getInstance().getQuery("U_ALL_REPERTORIO_INSTANCES_STATE");
            query.setParam("counterId", counterId);
            query.setParam("counterState", state.ToString());
            using (DBProvider dbProvider = new DBProvider())
            {
                retVal = dbProvider.ExecuteNonQuery(query.getSQL());
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per la costruzione della lista dei range di repertori da stampare. Ogni oggetto della lista
        /// farà riferimento ad un particolare anno
        /// </summary>
        /// <param name="counterId">Id del contatore</param>
        /// <param name="registryId">Id del registro</param>
        /// <param name="rfId">Id dell'RF</param>
        /// <returns>Lista con le informazioni sui range numerici dei repertori da stampare</returns>
        public List<RepertorioPrintRange> GetRepertoriPrintRanges(String counterId, String registryId, String rfId, bool repairBrokenPrint)
        {
            List<RepertorioPrintRange> retVal = new List<RepertorioPrintRange>();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Recupero della query da eseguire
                Query query = null;
                
                if (repairBrokenPrint)
                    query = InitQuery.getInstance().getQuery("S_REP_TO_REPAIR_PRINT");
                else
                    query = InitQuery.getInstance().getQuery("S_REP_TO_PRINT");

                // Impostazione dei parametri di filtro
                query.setParam("counterId", counterId);

                String registry = String.IsNullOrEmpty(registryId) ? " is null " : String.Format(" = {0}", registryId);
                String rf = String.IsNullOrEmpty(rfId) ? " is null " : String.Format(" = {0}", rfId);

                String regRfList = String.Empty;
                if (String.IsNullOrEmpty(registryId) && String.IsNullOrEmpty(rfId))
                    regRfList = "0";
                else
                    regRfList = String.IsNullOrEmpty(registryId) ? rfId : registryId;

                query.setParam("regRfList", regRfList);
                query.setParam("registry", registry);
                query.setParam("rf", rf);

                // Esecuzione della query
                using (IDataReader dataReader = dbProvider.ExecuteReader(query.getSQL()))
                {
                    if (dataReader != null)
                    {
                        // Analisi risultati
                        while (dataReader.Read())
                            retVal.Add(new RepertorioPrintRange()
                                {
                                    FirstNumber = Convert.ToInt32(dataReader["FirstNumber"].ToString()),
                                    LastNumber = Convert.ToInt32(dataReader["LastNumber"].ToString()),
                                    Year = Convert.ToInt32(dataReader["RepYear"].ToString())

                                });
                    }
                }
                
            }

            return retVal;

        }

        /// <summary>
        /// Metodo per la verifica dell'esistenza di repertori da stampare
        /// </summary>
        /// <param name="counterId"></param>
        /// <param name="registryId"></param>
        /// <param name="rfId"></param>
        /// <returns></returns>
        public bool ExistsRepertoriToPrint(String counterId, String registryId, String rfId)
        {
            bool retVal = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                // Recupero della query da eseguire
                Query query = InitQuery.getInstance().getQuery("S_EXISTS_REP_TO_PRINT");

                // Impostazione dei parametri di filtro
                query.setParam("counterId", counterId);

                String registry = String.IsNullOrEmpty(registryId) ? " is null " : String.Format(" = {0}", registryId);
                String rf = String.IsNullOrEmpty(rfId) ? " is null " : String.Format(" = {0}", rfId);

                String regRfList = String.Empty;
                if (String.IsNullOrEmpty(registryId) && String.IsNullOrEmpty(rfId))
                    regRfList = "0";
                else
                    regRfList = String.IsNullOrEmpty(registryId) ? rfId : registryId;

                query.setParam("regRfList", regRfList);
                query.setParam("registry", registry);
                query.setParam("rf", rf);

                String val = String.Empty;
                dbProvider.ExecuteScalar(out val, query.getSQL());

                retVal = !String.IsNullOrEmpty(val) && Convert.ToInt16(val) > 0;

            }

            return retVal;

        }

        public List<RegistroRepertorioPrint> GetRegistersToPrintByCounter(string counterId)
        {
            List<RegistroRepertorioPrint> retVal = new List<RegistroRepertorioPrint>();
            Query query = InitQuery.getInstance().getQuery("S_REP_TO_PRINT_BY_COUNTER");
            query.setParam("counterid", counterId);
            string querystring = query.getSQL();
            logger.Debug(querystring);
                
            using (DBProvider dbProvider = new DBProvider())
            {

                using (IDataReader dataReader = dbProvider.ExecuteReader(querystring))
                {
                    while (dataReader.Read())
                    {
                        RegistroRepertorioPrint reg = new RegistroRepertorioPrint()
                        {
                            CounterId = dataReader["CounterId"].ToString(),
                            CounterDescription = dataReader["CounterDescription"].ToString(),
                            TipologyDescription = dataReader["TipologyDescription"].ToString(),
                            RegistryId = dataReader["RegistryId"].ToString(),
                            RFId = dataReader["RFId"].ToString(),
                            CounterState = (RegistroRepertorioSingleSettings.RepertorioState)Enum.Parse(typeof(RegistroRepertorioSingleSettings.RepertorioState), dataReader["CounterState"].ToString()),
                            PrintFrequency = (RegistroRepertorioSingleSettings.Frequency)Enum.Parse(typeof(RegistroRepertorioSingleSettings.Frequency), dataReader["PrintFrequency"].ToString())
                        };

                        
                        // Recupero delle informazioni sul ruolo stampatore
                        Utenti users = new Utenti();
                        reg.PrinterRole = users.getRuoloById(dataReader["PrinterRoleCorrGlob"].ToString());

                        // Caricamento dei registri associati al ruolo
                        reg.PrinterRole.registri = users.GetRegistriRuolo(reg.PrinterRole.systemId);

                        // Recupero delle informazioni sull'utente responsabile
                        reg.PrinterUser = users.GetUtente(dataReader["PrinterUserRespId"].ToString());

                        retVal.Add(reg);

                    }
                }
            }

            return retVal;
        }

        public RepertorioPrintRange GetRepertoriPrintRangesByYear(string counterId, string registryId, string rfId, string year )
        {
            RepertorioPrintRange retval = null;

            using (DBProvider dbProvider = new DBProvider())
            {
                // Recupero della query da eseguire
                Query query =  InitQuery.getInstance().getQuery("S_REP_TO_PRINT_BY_YEAR_SPECIFIC");

                // Impostazione dei parametri di filtro
                query.setParam("counterid", counterId);
                query.setParam("year", year);
                if(!string.IsNullOrEmpty(rfId))
                    query.setParam("idaoorf", rfId);
                else if (!string.IsNullOrEmpty(registryId))
                    query.setParam("idaoorf", registryId);
                else query.setParam("idaoorf", "0");
                string querystring = query.getSQL();
                logger.Debug(querystring);
                // Esecuzione della query
                using (IDataReader dataReader = dbProvider.ExecuteReader(querystring))
                {
                    if (dataReader != null)
                    {
                        // Analisi risultati
                        while (dataReader.Read())
                            retval= new RepertorioPrintRange(){
                                FirstNumber = Convert.ToInt32(dataReader["minimo"].ToString()),
                                LastNumber = Convert.ToInt32(dataReader["massimo"].ToString()),
                                Year = Convert.ToInt32(year)

                            };
                    }
                }

            }
            return retval;
        }

        /// <summary>
        /// INTEGRAZIONE PITRE-PARER
        /// MEV Policy e responsabile conservazione
        /// Aggiorna il ruolo responsabile della conservazione per l'amministrazione selezionata
        /// </summary>
        /// <param name="idGroup"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public bool SaveRuoloRespConservazione(string idGroup, string idUtente, string idAmm)
        {
            bool result = false;

            try
            {

                Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_SET_ROLE_RESP_CONS");
                q.setParam("idGr", idGroup);
                q.setParam("idUt", idUtente);
                q.setParam("idAmm", idAmm);

                string command = q.getSQL();
                logger.Debug(command);

                using (DBProvider dbProvider = new DBProvider())
                {
                    if (!dbProvider.ExecuteNonQuery(command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                logger.Debug(ex.Message);
            }

            return result;
        }

        public string GetNomeTipologia(string idContatore)
        {
            string result = string.Empty;

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_NOME_TIPOLOGIA");
                    query.setParam("idCounter", idContatore);

                    string commandText = query.getSQL();
                    logger.Debug(commandText);

                    if (!dbProvider.ExecuteScalar(out result, commandText))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    //logger.Debug("Tipologia: " + result);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                result = string.Empty;
            }


            return result;
        }
    }
}
