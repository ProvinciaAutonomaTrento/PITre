using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Collections;



namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Db layer for the management of the users interested in following documents and folders
    /// </summary>
    public class FollowDomainObjectDB : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(FollowDomainObjectDB));

        #region CONSTANT

        const string ADD_DOC_FOLDER = "addDocFolder";
        const string REMOVE_DOC_FOLDER = "removeDocFolder";
        const string ADD_FOLDER = "addFolder";
        const string REMOVE_FOLDER = "removeFolder";
        const string ADD_DOC = "addDoc";
        const string REMOVE_DOC = "removeDoc";

        #endregion

        public bool FollowDomainObjectManager(string operation, string idDoc, string idFasc)
        {
            bool res = false;
            try
            {
                this.BeginTransaction();

                logger.Debug("INIZIO SPFollowDomainObject");

                // Creazione parametri per la Store Procedure
                ArrayList parameters = new ArrayList();
                DocsPaUtils.Data.ParameterSP outParam;
                outParam = new DocsPaUtils.Data.ParameterSP("resultValue", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                parameters.Add(new DocsPaUtils.Data.ParameterSP("idFasc", idFasc));
                parameters.Add(new DocsPaUtils.Data.ParameterSP("idDoc", idDoc));
                parameters.Add(new DocsPaUtils.Data.ParameterSP("operation", operation));
                parameters.Add(new DocsPaUtils.Data.ParameterSP("idPeople", "0"));
                parameters.Add(new DocsPaUtils.Data.ParameterSP("idGroup", "0"));
                parameters.Add(new DocsPaUtils.Data.ParameterSP("idAmm", "0"));
                parameters.Add(new DocsPaUtils.Data.ParameterSP("application", ""));
                this.ExecuteStoredProcedure("SPFollowDomainObject", parameters, null);
                if (outParam.Valore != null && (int)outParam.Valore == 0)
                {
                    res = true;
                    logger.Debug("STORE PROCEDURE SPFollowDomainObject: esito positivo");
                }
                else
                {
                    res = false;
                    logger.Debug("STORE PROCEDURE SPFollowDomainObject: esito negativo");
                }
            }
            catch (Exception exc)
            { }
            return res;
        }

        public bool FollowDomainObjectManager(string operation, string idObject, string idPeople, string idGroup, string idAmm, string application)
        {
            bool res = false;
            try
            {
                this.BeginTransaction();

                logger.Debug("INIZIO SPFollowDomainObject");

                // Creazione parametri per la Store Procedure
                ArrayList parameters = new ArrayList();
                DocsPaUtils.Data.ParameterSP outParam;
                outParam = new DocsPaUtils.Data.ParameterSP("resultValue", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                if (operation.Equals(ADD_FOLDER) || operation.Equals(REMOVE_FOLDER))
                {
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("idFasc", idObject));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("idDoc", "0"));
                }
                else
                {
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("idDoc", idObject));
                    parameters.Add(new DocsPaUtils.Data.ParameterSP("idFasc", "0"));
                }
                parameters.Add(new DocsPaUtils.Data.ParameterSP("operation", operation));
                parameters.Add(new DocsPaUtils.Data.ParameterSP("idPeople", idPeople));
                parameters.Add(new DocsPaUtils.Data.ParameterSP("idGroup", idGroup));
                parameters.Add(new DocsPaUtils.Data.ParameterSP("idAmm", idAmm));
                parameters.Add(new DocsPaUtils.Data.ParameterSP("application", application));
                this.ExecuteStoredProcedure("SPFollowDomainObject", parameters, null);
                if (outParam.Valore != null && (int)outParam.Valore == 0)
                {
                    res = true;
                    logger.Debug("STORE PROCEDURE SPFollowDomainObject: esito positivo");
                }
                else
                {
                    res = false;
                    logger.Debug("STORE PROCEDURE SPFollowDomainObject: esito negativo");
                }
            }
            catch (Exception exc)
            { }
            return res;
        }
    }

}
