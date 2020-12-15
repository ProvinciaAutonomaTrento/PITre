using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Deposito;
using DocsPaUtils;
using log4net;
using System.Data;
using System.Data.Common;
using System.Collections;

namespace DocsPaDB.Query_DocsPAWS
{
    public class Deposito : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(Deposito));

        #region ARCHIVE_Disposal_TMP_Update

        /// <summary>
        /// Update tabelle temporanee scarto
        /// </summary>
        /// <param name="Disposal_ID"></param>
        /// <param name="listSystemID"></param>
        /// <returns></returns>
        public Boolean Update_ARCHIVE_TempProjectDisposal(Int32 Disposal_ID, string listProjectsID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Update_TempProjectDisposal]");
                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Disposal_ID", Disposal_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ProjectsList", listProjectsID));
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoreProcedure("sp_ARCHIVE_Update_TempProjectDisposal", sp_params);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Update tabelle temporanee scarto
        /// </summary>
        /// <param name="Disposal_ID"></param>
        /// <param name="listSystemID"></param>
        /// <returns></returns>
        public Boolean Update_ARCHIVE_TempProfileDisposal(Int32 Disposal_ID, string listProfilesID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Update_TempProfileDisposal]");
                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Disposal_ID", Disposal_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ProfilesList", listProfilesID));
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoreProcedure("sp_ARCHIVE_Update_TempProfileDisposal", sp_params);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region ARCHIVE_JOB_Disposal

        /// <summary>
        /// Persists a new instance of ARCHIVE_JOB_Disposal to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public void InsertARCHIVE_JOB_Disposal(Int32 Disposal_ID, Int32 jobType_ID, ref Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_JOB_Insert_Disposal]");
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Disposal_ID", Disposal_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@JobType_ID", jobType_ID));

            DocsPaUtils.Data.ParameterSP System_ID = new DocsPaUtils.Data.ParameterSP("@System_ID", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
            sp_params.Add(System_ID);

            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_JOB_Insert_Disposal", sp_params, ds);
            }
            system_ID = (Int32)System_ID.Valore;
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_Disposal>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_Transfer. </results>
        public List<ARCHIVE_JOB_Disposal> GetARCHIVE_JOB_DisposalByDisposal_ID(Int32 disposal_ID)
        {
            List<ARCHIVE_JOB_Disposal> dataList;
            logger.Debug("[sp_ARCHIVE_JOB_Select_Disposal_By_ARCHIVE_Disposal_ID]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Disposal_ID", disposal_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_JOB_Select_Disposal_By_ARCHIVE_Disposal_ID", sp_params, ds);
                dataList = new List<ARCHIVE_JOB_Disposal>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();
                int disposal_IDOrdinal = dr.GetOrdinal("System_ID");
                int transfer_IDOrdinal = dr.GetOrdinal("Disposal_ID");
                int jobType_IDOrdinal = dr.GetOrdinal("JobType_ID");
                int inserJobTimestampOrdinal = dr.GetOrdinal("InsertJobTimestamp");
                int startJobTimestampOrdinal = dr.GetOrdinal("StartJobTimestamp");
                int endJobTimestampOrdinal = dr.GetOrdinal("EndJobTimestamp");
                int executedOrdinal = dr.GetOrdinal("Executed");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_JOB_Disposal(dr.GetInt32(disposal_IDOrdinal),
                        dr.GetInt32(transfer_IDOrdinal),
                        dr.GetInt32(jobType_IDOrdinal),
                        dr.GetDateTime(inserJobTimestampOrdinal),
                        dr.IsDBNull(startJobTimestampOrdinal) ? (DateTime?)null : dr.GetDateTime(startJobTimestampOrdinal),
                        dr.IsDBNull(endJobTimestampOrdinal) ? (DateTime?)null : dr.GetDateTime(endJobTimestampOrdinal),
                        dr.GetInt32(executedOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }


        #endregion

        #region ARCHIVE_View_Documents_Disposal
        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_Documents_Policy>.
        /// Per questioni di tempo devo usare questo oggetto anche per gli scarti.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Documents_Policy. </results>
        public List<ARCHIVE_View_Documents_Policy> GetAllARCHIVE_View_Documents_Disposal(Int32 Disposal_ID)
        {
            List<ARCHIVE_View_Documents_Policy> dataList;
            logger.Debug("[sp_ARCHIVE_BE_GetDocumentsByDisposalID]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@disposalID", Disposal_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_GetDocumentsByDisposalID", sp_params, ds);
                dataList = new List<ARCHIVE_View_Documents_Policy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int registro_IDOrdinal = dr.GetOrdinal("Registro");
                int titolarioOrdinal = dr.GetOrdinal("Titolario");
                int classetitolarioOrdinal = dr.GetOrdinal("Classetitolario");
                int tipologiaOrdinal = dr.GetOrdinal("Tipologia");
                int annoCreazioneOrdinal = dr.GetOrdinal("Anno_Creazione");
                int totale = dr.GetOrdinal("Totale");
                int countDistinct = dr.GetOrdinal("CountDistinct");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_View_Documents_Policy(
                         dr.IsDBNull(registro_IDOrdinal) ? null : dr.GetString(registro_IDOrdinal),
                         dr.IsDBNull(titolarioOrdinal) ? null : dr.GetString(titolarioOrdinal),
                         dr.IsDBNull(classetitolarioOrdinal) ? null : dr.GetString(classetitolarioOrdinal),
                         dr.IsDBNull(tipologiaOrdinal) ? null : dr.GetString(tipologiaOrdinal),
                         dr.IsDBNull(annoCreazioneOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoCreazioneOrdinal)),
                         dr.IsDBNull(totale) ? null : new Nullable<Int32>(dr.GetInt32(totale)),
                         dr.GetInt32(countDistinct)));

                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }

        #endregion

        #region ARCHIVE_View_Projects_Disposal

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_Projects_Policy>.
        /// Per questioni di tempo devo usare questo oggetto anche per gli scarti.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Projects_Policy. </results>
        public List<ARCHIVE_View_Projects_Policy> GetAllARCHIVE_View_Projects_Disposal(Int32 Disposal_ID)
        {
            List<ARCHIVE_View_Projects_Policy> dataList;
            logger.Debug("[sp_ARCHIVE_BE_GetProjectsByDisposalID]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@disposalID", Disposal_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_GetProjectsByDisposalID", sp_params, ds);
                dataList = new List<ARCHIVE_View_Projects_Policy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int registro_IDOrdinal = dr.GetOrdinal("Registro");
                int titolarioOrdinal = dr.GetOrdinal("Titolario");
                int classetitolarioOrdinal = dr.GetOrdinal("Classetitolario");
                int tipologiaOrdinal = dr.GetOrdinal("Tipologia");
                int annoChiusuraOrdinal = dr.GetOrdinal("Anno_Chiusura");
                int totale = dr.GetOrdinal("Totale");
                int countDistinct = dr.GetOrdinal("CountDistinct");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_View_Projects_Policy(
                        dr.IsDBNull(registro_IDOrdinal) ? null : dr.GetString(registro_IDOrdinal),
                         dr.IsDBNull(titolarioOrdinal) ? null : dr.GetString(titolarioOrdinal),
                         dr.IsDBNull(classetitolarioOrdinal) ? null : dr.GetString(classetitolarioOrdinal),
                         dr.IsDBNull(tipologiaOrdinal) ? null : dr.GetString(tipologiaOrdinal),
                         dr.IsDBNull(annoChiusuraOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoChiusuraOrdinal)),
                          dr.IsDBNull(totale) ? null : new Nullable<Int32>(dr.GetInt32(totale)),
                          dr.GetInt32(countDistinct)));
                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }

        #endregion

        #region ARCHIVE_Disposal
        /// <summary>
        /// Returns an instance of List<DisposalFilterForSearch>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of DisposalFilterForSearch. </results>
        public List<ARCHIVE_DisposalForSearch> GetAllARCHIVE_DisposalForSearch(String st_indefinizione, String st_analisicompletata, String st_proposto,
                                                                               String st_approvato, String st_inesecuzione, String st_effettuato, String st_inerrore, Int32 finger)
        {
            List<ARCHIVE_DisposalForSearch> dataList;
            logger.Debug("[sp_ARCHIVE_BE_SearchDisposal]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_InDefinizione", st_indefinizione));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_RicercaCompletata", st_analisicompletata));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_Proposto", st_proposto));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_Approvato", st_approvato));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_InEsecuzione", st_inesecuzione));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_Effettuato", st_effettuato));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_InErrore", st_inerrore));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@tipoStatoEsecuzione", finger));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_SearchDisposal", sp_params, ds);
                dataList = new List<ARCHIVE_DisposalForSearch>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_idOrdinal = dr.GetOrdinal("ID_SCARTO");
                int id_ammOrdinal = dr.GetOrdinal("ID_AMMINISTRAZIONE");
                int descriptionOrdinal = dr.GetOrdinal("DESCRIZIONE");
                int stato_IDOrdinal = dr.GetOrdinal("STATO");
                int dateTimeOrdinal = dr.GetOrdinal("DATA");
                int numero_doc_scartatiOrdinal = dr.GetOrdinal("NUM_DOCUMENTI_SCARTATI");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_DisposalForSearch(
                        dr.GetInt32(system_idOrdinal),
                        dr.GetInt32(id_ammOrdinal),
                        dr.GetString(descriptionOrdinal),
                        dr.GetString(stato_IDOrdinal),
                        dr.GetDateTime(dateTimeOrdinal),
                        dr.GetInt32(numero_doc_scartatiOrdinal)));
                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }


        public bool StartAnalysisForDisposal(Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_BE_StartAnalysisForDisposal]");
                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferID", system_ID));
                DataSet _ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_StartAnalysisForDisposal", sp_params, _ds);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_Disposal>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Disposal. </results>
        public List<ARCHIVE_Disposal> GetARCHIVE_DisposalBySystem_ID(Int32 system_ID)
        {
            List<ARCHIVE_Disposal> dataList;
            logger.Debug("[sp_ARCHIVE_Select_Disposal_PK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("system_ID", system_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_Disposal_PK", sp_params, ds);
                dataList = new List<ARCHIVE_Disposal>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();
                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int iD_AmministrazioneOrdinal = dr.GetOrdinal("ID_Amministrazione");
                int descriptionOrdinal = dr.GetOrdinal("Description");
                int noteOrdinal = dr.GetOrdinal("Note");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_Disposal(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(iD_AmministrazioneOrdinal),
                        dr.GetString(descriptionOrdinal),
                        dr.IsDBNull(noteOrdinal) ? null : dr.GetString(noteOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns all instances of ARCHIVE_Disposal.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_Disposal. </results>
        public List<ARCHIVE_Disposal> GetAllARCHIVE_Disposal()
        {
            List<ARCHIVE_Disposal> dataList;
            logger.Debug("[sp_ARCHIVE_Select_Disposal_All]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_Disposal_All", sp_params, ds);
                dataList = new List<ARCHIVE_Disposal>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();
                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int iD_AmministrazioneOrdinal = dr.GetOrdinal("ID_Amministrazione");
                int descriptionOrdinal = dr.GetOrdinal("Description");
                int noteOrdinal = dr.GetOrdinal("Note");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_Disposal(dr.GetInt32(system_IDOrdinal),
                       dr.GetInt32(iD_AmministrazioneOrdinal),
                       dr.GetString(descriptionOrdinal),
                       dr.IsDBNull(noteOrdinal) ? null : dr.GetString(noteOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_Disposal based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_Disposal(Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Delete_Disposal_PK]");
                ArrayList sp_params = new ArrayList();

                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@system_ID", system_ID));
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(rowsAffected);
                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Delete_Disposal_PK", sp_params, ds);
                    return (Int32)rowsAffected.Valore > 0;

                }
            }
            catch (Exception e)
            {
                return false;
            }

        }
        /// <summary>
        /// Updates an instance of ARCHIVE_Disposal.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_Disposal(String description, String note, Int32 system_ID, Int32 iD_Amministrazione)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Update_Disposal_PK]");
                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description", description));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Note", note));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ID_Amministrazione", iD_Amministrazione));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoreProcedure("sp_ARCHIVE_Update_Disposal_PK", sp_params);
                }
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_Disposal", e);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_Disposal if the original data has not changed.
        /// </summary>
        /// <param name="description_Original">This field is used for optimistic concurrency management. It should contain the original value of 'description_Original'. </param>
        /// <param name="note">This is not a required field. </param>
        /// <param name="note_Original">This field is used for optimistic concurrency management. It should contain the original value of 'note_Original'. This is not a required field. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_Disposal(String description, String description_Original, String note, String note_Original,
                                                Int32 id_Amministrazione, Int32 id_Amministrazione_Original, Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_Update_Disposal]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description", description));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description_Original", description_Original));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Note", note));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Note_Original", note));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ID_Amministrazione", id_Amministrazione));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ID_Amministrazione_Original", id_Amministrazione_Original));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_Disposal", sp_params, ds);
            }

            success = (ds.Tables[0].Rows.Count > 0);
            return success;
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_Disposal to the database.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public void InsertARCHIVE_Disposal(String description, String note, ref Int32 system_ID, Int32 id_Amministrazione, Int32 DisposalStateType_ID)
        {

            logger.Debug("[sp_ARCHIVE_Insert_Disposal]");
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description", description));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Note", note));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ID_Amministrazione", id_Amministrazione));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@DisposalStateType_ID", DisposalStateType_ID));
            DocsPaUtils.Data.ParameterSP System_ID = new DocsPaUtils.Data.ParameterSP("@System_ID", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
            sp_params.Add(System_ID);

            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Insert_Disposal", sp_params, ds);
            }
            system_ID = (Int32)System_ID.Valore;
        }


        #endregion

        #region ARCHIVE_DisposalState

        /// <summary>
        /// Returns all instances of ARCHIVE_DisposalState.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalState. </results>
        public List<ARCHIVE_DisposalState> GetAllARCHIVE_DisposalState()
        {
            List<ARCHIVE_DisposalState> dataList;
            logger.Debug("[sp_ARCHIVE_Select_DisposalState_All]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_DisposalState_All", sp_params, ds);
                dataList = new List<ARCHIVE_DisposalState>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int disposal_IDOrdinal = dr.GetOrdinal("Disposal_ID");
                int disposalStateType_IDOrdinal = dr.GetOrdinal("DisposalStateType_ID");
                int dateTimeOrdinal = dr.GetOrdinal("DateTime");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_DisposalState(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(disposal_IDOrdinal),
                        dr.GetInt32(disposalStateType_IDOrdinal),
                        dr.GetDateTime(dateTimeOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_DisposalState>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalState. </results>
        public List<ARCHIVE_DisposalState> GetARCHIVE_DisposalStateBySystem_ID(Int32 system_ID)
        {

            List<ARCHIVE_DisposalState> dataList;
            logger.Debug("[sp_ARCHIVE_Select_DisposalState_PK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("system_ID", system_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_DisposalState_PK", sp_params, ds);
                dataList = new List<ARCHIVE_DisposalState>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int disposal_IDOrdinal = dr.GetOrdinal("Disposal_ID");
                int disposalStateType_IDOrdinal = dr.GetOrdinal("DisposalStateType_ID");
                int dateTimeOrdinal = dr.GetOrdinal("DateTime");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_DisposalState(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(disposal_IDOrdinal),
                        dr.GetInt32(disposalStateType_IDOrdinal),
                        dr.GetDateTime(dateTimeOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_DisposalState>.
        /// </summary>
        /// <param name="disposal_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalState. </results>
        public List<ARCHIVE_DisposalState> GetARCHIVE_DisposalStateByDisposal_ID(Int32 disposal_ID)
        {

            List<ARCHIVE_DisposalState> dataList;
            logger.Debug("[sp_ARCHIVE_Select_DisposalState_By_ARCHIVE_Disposal_Disposal_ID_FK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("disposal_ID", disposal_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_DisposalState_By_ARCHIVE_Disposal_Disposal_ID_FK", sp_params, ds);
                dataList = new List<ARCHIVE_DisposalState>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int disposal_IDOrdinal = dr.GetOrdinal("Disposal_ID");
                int disposalStateType_IDOrdinal = dr.GetOrdinal("DisposalStateType_ID");
                int dateTimeOrdinal = dr.GetOrdinal("DateTime");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_DisposalState(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(disposal_IDOrdinal),
                        dr.GetInt32(disposalStateType_IDOrdinal),
                        dr.GetDateTime(dateTimeOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns a collection of ARCHIVE_DisposalState based on the following criteria: DisposalStateType_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalState. </results>
        public List<ARCHIVE_DisposalState> GetARCHIVE_DisposalStateByDisposalStateType_ID(Int32 disposalStateType_ID)
        {
            List<ARCHIVE_DisposalState> dataList;
            logger.Debug("[sp_ARCHIVE_Select_DisposalState_By_ARCHIVE_DisposalStateType_DisposalStateType_ID_FK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@DisposalStateType_ID", disposalStateType_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_DisposalState_By_ARCHIVE_DisposalStateType_DisposalStateType_ID_FK", sp_params, ds);
                dataList = new List<ARCHIVE_DisposalState>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int disposal_IDOrdinal = dr.GetOrdinal("Disposal_ID");
                int disposalStateType_IDOrdinal = dr.GetOrdinal("DisposalStateType_ID");
                int dateTimeOrdinal = dr.GetOrdinal("DateTime");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_DisposalState(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(disposal_IDOrdinal),
                        dr.GetInt32(disposalStateType_IDOrdinal),
                        dr.GetDateTime(dateTimeOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_DisposalState based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_DisposalState(Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_Delete_DisposalState_PK]");
            ArrayList sp_params = new ArrayList();

            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@system_ID", system_ID));

            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Delete_DisposalState_PK", sp_params, ds);
            }
            return true;
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_DisposalState based on Disposal_ID.
        /// </summary>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_DisposalStateByDisposal_ID(Int32 disposal_ID)
        {


            logger.Debug("[sp_ARCHIVE_Delete_DisposalState_By_ARCHIVE_Disposal_Disposal_ID_FK]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("disposal_ID", disposal_ID));
            int rowsAffected = 0;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {

                dbProvider.ExecuteNonQuery("sp_ARCHIVE_Delete_DisposalState_By_ARCHIVE_Disposal_Disposal_ID_FK", out rowsAffected);
                success = (rowsAffected > 0);
                return success;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_DisposalState.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_DisposalState(Int32 disposal_ID, Int32 disposalStateType_ID, Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_Update_DisposalState_PK]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@disposal_ID", disposal_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@disposalStateType_ID", disposalStateType_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_DisposalState_PK", sp_params, ds);
            }

            success = (ds.Tables[0].Rows.Count > 0);
            return success;
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_DisposalState if the original data has not changed.
        /// </summary>
        /// <param name="disposal_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'disposal_ID_Original'. </param>
        /// <param name="disposalStateType_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'disposalStateType_ID_Original'. </param>
        /// <param name="dateTime_Original">This field is used for optimistic concurrency management. It should contain the original value of 'dateTime_Original'. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_DisposalState(Int32 disposal_ID, Int32 disposal_ID_Original, Int32 disposalStateType_ID, Int32 disposalStateType_ID_Original, Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_Update_DisposalState]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@disposal_ID", disposal_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@disposal_ID_Original", disposal_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@DisposalStateType_ID", disposalStateType_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@DisposalStateType_ID_Original", disposalStateType_ID_Original));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_DisposalState", sp_params, ds);
            }

            success = (ds.Tables[0].Rows.Count > 0);
            return success;
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_DisposalState to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public void InsertARCHIVE_DisposalState(Int32 disposal_ID, Int32 disposalStateType_ID, ref Int32 system_ID)
        {
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Disposal_ID", disposal_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@DisposalStateType_ID", disposalStateType_ID));
            DocsPaUtils.Data.ParameterSP System_ID = new DocsPaUtils.Data.ParameterSP("@System_ID", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
            sp_params.Add(System_ID);
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Insert_DisposalState", sp_params, ds);
            }
            system_ID = (Int32)System_ID.Valore;
        }

        #endregion ARCHIVE_DisposalState

        #region ARCHIVE_DisposalStateType
        /// <summary>
        /// Returns all instances of ARCHIVE_DisposalStateType.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalStateType. </results>
        public List<ARCHIVE_DisposalStateType> GetAllARCHIVE_DisposalStateType()
        {
            List<ARCHIVE_DisposalStateType> dataList;
            logger.Debug("[sp_ARCHIVE_Select_DisposalStateType_All]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_DisposalStateType_All", sp_params, ds);
                dataList = new List<ARCHIVE_DisposalStateType>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int nameOrdinal = dr.GetOrdinal("Name");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_DisposalStateType(dr.GetInt32(system_IDOrdinal),
                        dr.GetString(nameOrdinal)));
                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_DisposalStateType>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_DisposalStateType. </results>
        public List<ARCHIVE_DisposalStateType> GetARCHIVE_DisposalStateTypeBySystem_ID(Int32 system_ID)
        {

            List<ARCHIVE_DisposalStateType> dataList;
            logger.Debug("[sp_ARCHIVE_Select_DisposalStateType_PK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("system_ID", system_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_DisposalStateType_PK", sp_params, ds);
                dataList = new List<ARCHIVE_DisposalStateType>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int nameOrdinal = dr.GetOrdinal("Name");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_DisposalStateType(dr.GetInt32(system_IDOrdinal),
                        dr.GetString(nameOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_DisposalStateType based on System_ID.
        /// </summary>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_DisposalStateType(Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_Delete_DisposalStateType_PK]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("system_ID", system_ID));
            int rowsAffected = 0;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {

                dbProvider.ExecuteNonQuery("sp_ARCHIVE_Delete_DisposalStateType_PK", out rowsAffected);
                success = (rowsAffected > 0);
                return success;
            }
        }
        /// <summary>
        /// Deletes all instances of ARCHIVE_DisposalStateType.
        /// </summary>
        /// <results>The number of items deleted. </results>
        public Int32 DeleteAllARCHIVE_DisposalStateType()
        {
            logger.Debug("[sp_ARCHIVE_Delete_DisposalStateType_All]");
            int rowsAffected = 0;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteNonQuery("sp_ARCHIVE_Delete_DisposalStateType_All", out rowsAffected);
                return rowsAffected;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_DisposalStateType.
        /// </summary>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_DisposalStateType(String name, Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_Update_DisposalStateType_PK]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Name", name));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_DisposalStateType_PK", sp_params, ds);
            }

            success = (ds.Tables[0].Rows.Count > 0);
            return success;
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_DisposalStateType if the original data has not changed.
        /// </summary>
        /// <param name="name_Original">This field is used for optimistic concurrency management. It should contain the original value of 'name_Original'. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_DisposalStateType(String name, String name_Original, Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_Update_DisposalStateType]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Name", name));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Name_Original", name_Original));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_DisposalStateType", sp_params, ds);
            }

            success = (ds.Tables[0].Rows.Count > 0);
            return success;
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_DisposalStateType to the database.
        /// </summary>
        public Boolean InsertARCHIVE_DisposalStateType(Int32 system_ID, String name)
        {
            logger.Debug("[sp_ARCHIVE_Insert_DisposalStateType]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Name", name));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Insert_DisposalStateType", sp_params, ds);
            }

            success = (ds.Tables[0].Rows.Count > 0);
            return success;
        }

        #endregion

        #region ARCHIVE_AUTH

        /// <summary>
        /// Returns an instance of List<ARCHIVE_AUTH_Authorization>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_AUTH_Authorization. </results>
        public List<ARCHIVE_AUTH_Authorization> GetARCHIVE_AutorizationBySystem_ID(Int32 system_ID)
        {
            List<ARCHIVE_AUTH_Authorization> dataList;
            logger.Debug("[sp_ARCHIVE_AUTH_Select_Autorization_PK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@system_ID", system_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_AUTH_Select_Autorization_PK", sp_params, ds);
                dataList = new List<ARCHIVE_AUTH_Authorization>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();
                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int People_IDOrdinal = dr.GetOrdinal("People_ID");
                int StartDateOrdinal = dr.GetOrdinal("StartDate");
                int EndDateDateOrdinal = dr.GetOrdinal("EndDate");
                int noteOrdinal = dr.GetOrdinal("Note");


                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_AUTH_Authorization(
                    dr.GetInt32(system_IDOrdinal),
                    dr.GetInt32(People_IDOrdinal),
                    dr.IsDBNull(StartDateOrdinal) ? (DateTime?)null : dr.GetDateTime(StartDateOrdinal),
                    dr.IsDBNull(EndDateDateOrdinal) ? (DateTime?)null : dr.GetDateTime(EndDateDateOrdinal),
                    dr.GetString(noteOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_AUTH_Authorization>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_AUTH_Authorization. </results>
        public List<ARCHIVE_AUTH_Authorization> GetALLARCHIVE_Autorizations()
        {
            List<ARCHIVE_AUTH_Authorization> dataList;
            logger.Debug("[sp_ARCHIVE_AUTH_Select_Autorization_All]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_AUTH_Select_Autorization_All", sp_params, ds);
                dataList = new List<ARCHIVE_AUTH_Authorization>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();
                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int People_IDOrdinal = dr.GetOrdinal("People_ID");
                int StartDateOrdinal = dr.GetOrdinal("StartDate");
                int EndDateDateOrdinal = dr.GetOrdinal("EndDate");
                int noteOrdinal = dr.GetOrdinal("Note");


                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_AUTH_Authorization(
                    dr.GetInt32(system_IDOrdinal),
                    dr.GetInt32(People_IDOrdinal),
                    dr.IsDBNull(StartDateOrdinal) ? (DateTime?)null : dr.GetDateTime(StartDateOrdinal),
                    dr.IsDBNull(EndDateDateOrdinal) ? (DateTime?)null : dr.GetDateTime(EndDateDateOrdinal),
                    dr.GetString(noteOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }

        /// <summary>
        /// Deletes an instance of ARCHIVE_Transfer based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_Autorizations_BySystem_ID(Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_AUTH_Delete_Autorization_PK]");
                ArrayList sp_params = new ArrayList();

                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@system_ID", system_ID));

                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_AUTH_Delete_Autorization_PK", sp_params, ds);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_AUTH_Authorization to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public void InsertARCHIVE_Authorization(Int32 People_ID, String StartDate, String EndDate, String note, String profileList,
                                                        String projectList, ref Int32 systemID)
        {
            logger.Debug("[sp_ARCHIVE_BE_MergeAuthorization]");
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@peopleID", People_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@startDate", Convert.ToDateTime(StartDate)));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@endDate", Convert.ToDateTime(EndDate)));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@note", note));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@profileList", profileList));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@projectList", projectList));
            // sp_params.Add(new DocsPaUtils.Data.ParameterSP("@systemID", systemID));
            DocsPaUtils.Data.ParameterSP System_ID = new DocsPaUtils.Data.ParameterSP("@systemID", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
            sp_params.Add(System_ID);
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_MergeAuthorization", sp_params, ds);
            }
            systemID = (Int32)System_ID.Valore;
        }

        /// <summary>
        /// Updates an instance of ARCHIVE_AUTH_Authorization if the original data has not changed.
        /// </summary>
        public Boolean UpdateARCHIVE_Authorization(Int32 People_ID, String StartDate, String EndDate, String note, String profileList,
                                                        String projectList, ref Int32 systemID)
        {
            logger.Debug("[sp_ARCHIVE_BE_MergeAuthorization]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@peopleID", People_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@startDate", Convert.ToDateTime(StartDate)));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@endDate", Convert.ToDateTime(EndDate)));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Note", note));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@SystemID", systemID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@profileList", profileList));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@projectList", projectList));
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_MergeAuthorization", sp_params, ds);
            }

            success = (ds.Tables[0].Rows.Count > 0);
            return success;
        }
        #endregion

        #region ARCHIVE_AUTH_OBJECT

        /// <summary>
        /// Returns an instance of List<ARCHIVE_AUTH_AuthorizedObject>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_AUTH_AuthorizedObject. </results>
        public List<ARCHIVE_AUTH_AuthorizedObject> GetARCHIVE_AutorizedObjectBySystem_ID(Int32 system_ID)
        {
            List<ARCHIVE_AUTH_AuthorizedObject> dataList;
            logger.Debug("[sp_ARCHIVE_AUTH_Select_AutorizedObject_PK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_AUTH_Select_AutorizedObject_PK", sp_params, ds);
                dataList = new List<ARCHIVE_AUTH_AuthorizedObject>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();
                int Authorization_IDOrdinal = dr.GetOrdinal("Authorization_ID");
                int Profile_IDOrdinal = dr.GetOrdinal("Profile_ID");
                int Project_IDOrdinal = dr.GetOrdinal("Project_ID");
                int System_IDOrdinal = dr.GetOrdinal("System_ID");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_AUTH_AuthorizedObject(
                    dr.GetInt32(System_IDOrdinal),
                    dr.GetInt32(Authorization_IDOrdinal),
                    dr.IsDBNull(Project_IDOrdinal) ? 0 : new Nullable<Int32>(dr.GetInt32(Project_IDOrdinal)),
                    dr.IsDBNull(Profile_IDOrdinal) ? 0 : new Nullable<Int32>(dr.GetInt32(Profile_IDOrdinal))));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_AUTH_AuthorizedObject>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_AUTH_AuthorizedObject. </results>
        public List<ARCHIVE_AUTH_AuthorizedObject> GetALLARCHIVE_AutorizedObject()
        {
            List<ARCHIVE_AUTH_AuthorizedObject> dataList;
            logger.Debug("[sp_ARCHIVE_AUTH_Select_AutorizedObject_All]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_AUTH_Select_AutorizedObject_All", sp_params, ds);
                dataList = new List<ARCHIVE_AUTH_AuthorizedObject>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();
                int Authorization_IDOrdinal = dr.GetOrdinal("Authorization_ID");
                int Profile_IDOrdinal = dr.GetOrdinal("Profile_ID");
                int Project_IDOrdinal = dr.GetOrdinal("Project_ID");
                int System_IDOrdinal = dr.GetOrdinal("System_ID");


                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_AUTH_AuthorizedObject(
                    dr.GetInt32(System_IDOrdinal),
                    dr.GetInt32(Authorization_IDOrdinal),
                    dr.IsDBNull(Project_IDOrdinal) ? 0 : new Nullable<Int32>(dr.GetInt32(Project_IDOrdinal)),
                    dr.IsDBNull(Profile_IDOrdinal) ? 0 : new Nullable<Int32>(dr.GetInt32(Profile_IDOrdinal))));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }

        /// <summary>
        /// Deletes an instance of ARCHIVE_AUTH_AuthorizedObject based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_AutorizedObject_BySystem_ID(Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_AUTH_Delete_AutorizedObject_PK]");
                ArrayList sp_params = new ArrayList();

                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@system_ID", system_ID));

                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_AUTH_Delete_AutorizedObject_PK", sp_params, ds);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_AUTH_AuthorizedObject to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public void InsertARCHIVE_AutorizedObject(Int32 Authorization_ID, Int32 Project_ID, Int32 profile_ID, ref Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_AUTH_Insert_AuthorizedObject]");
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Authorization_ID", Authorization_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Project_ID", Project_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Profile_ID", profile_ID));

            DocsPaUtils.Data.ParameterSP System_ID = new DocsPaUtils.Data.ParameterSP("@System_ID", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
            sp_params.Add(System_ID);

            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_AUTH_Insert_AuthorizedObject", sp_params, ds);
            }
            system_ID = (Int32)System_ID.Valore;
        }

        /// <summary>
        /// Updates an instance of ARCHIVE_AUTH_Authorization if the original data has not changed.
        /// </summary>
        public Boolean UpdateARCHIVE_AutorizedObject(Int32 Authorization_ID, Int32 Project_ID, Int32 profile_ID, ref Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_AUTH_Update_AutorizedObject_PK]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Authorization_ID", Authorization_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Project_ID", Project_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@profile_ID", profile_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_AUTH_Update_AutorizedObject_PK", sp_params, ds);
            }

            success = (ds.Tables[0].Rows.Count > 0);
            return success;
        }
        #endregion

        #region ARCHIVE_TRANSFER_LOG

        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_Transfer>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_Transfer. </results>
        public List<ARCHIVE_LOG_TransferAndPolicy> GetARCHIVE_LOG_TransferAndPolicy(String ListaVersamentoIDANDPolicyID)
        {
            List<ARCHIVE_LOG_TransferAndPolicy> dataList;
            logger.Debug("[sp_ARCHIVE_LOG_Select_TransferANDPolicy]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@transferPolicyList", ListaVersamentoIDANDPolicyID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_LOG_Select_TransferANDPolicy", sp_params, ds);
                dataList = new List<ARCHIVE_LOG_TransferAndPolicy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_id_Ordinal = dr.GetOrdinal("System_ID");
                int TimestampOrdinal = dr.GetOrdinal("Timestamp");
                int ActionOrdinal = dr.GetOrdinal("Action");
                int ActionTypeOrdinal = dr.GetOrdinal("ActionType");
                int ObjectTypeOrdinal = dr.GetOrdinal("ObjectType");
                int ObjectIDOrdinal = dr.GetOrdinal("ObjectID");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_LOG_TransferAndPolicy(
                       dr.GetInt32(system_id_Ordinal),
                       dr.GetDateTime(TimestampOrdinal),
                       dr.GetString(ActionOrdinal),
                       dr.GetString(ActionTypeOrdinal),
                       dr.GetInt32(ObjectTypeOrdinal),
                       dr.GetInt32(ObjectIDOrdinal)
                       ));
                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }
        #endregion

        #region ARCHIVE_JOB_Transfer

        /// <summary>
        /// Persists a new instance of ARCHIVE_JOB_Transfer to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public void InsertARCHIVE_JOB_Transfer(Int32 transfer_ID, Int32 jobType_ID, ref Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_JOB_Insert_Transfer]");
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", transfer_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@JobType_ID", jobType_ID));

            DocsPaUtils.Data.ParameterSP System_ID = new DocsPaUtils.Data.ParameterSP("@System_ID", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
            sp_params.Add(System_ID);

            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_JOB_Insert_Transfer", sp_params, ds);
            }
            system_ID = (Int32)System_ID.Valore;
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_Transfer>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_Transfer. </results>
        public List<ARCHIVE_JOB_Transfer> GetARCHIVE_JOB_TransferByTransfer_ID(Int32 transfer_ID)
        {
            List<ARCHIVE_JOB_Transfer> dataList;
            logger.Debug("[sp_ARCHIVE_JOB_Select_Transfer_By_ARCHIVE_Transfer_ID_FK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", transfer_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_JOB_Select_Transfer_By_ARCHIVE_Transfer_ID_FK", sp_params, ds);
                dataList = new List<ARCHIVE_JOB_Transfer>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();
                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int transfer_IDOrdinal = dr.GetOrdinal("Transfer_ID");
                int jobType_IDOrdinal = dr.GetOrdinal("JobType_ID");
                int inserJobTimestampOrdinal = dr.GetOrdinal("InsertJobTimestamp");
                int startJobTimestampOrdinal = dr.GetOrdinal("StartJobTimestamp");
                int endJobTimestampOrdinal = dr.GetOrdinal("EndJobTimestamp");
                int executedOrdinal = dr.GetOrdinal("Executed");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_JOB_Transfer(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(transfer_IDOrdinal),
                        dr.GetInt32(jobType_IDOrdinal),
                        dr.GetDateTime(inserJobTimestampOrdinal),
                        dr.IsDBNull(startJobTimestampOrdinal) ? (DateTime?)null : dr.GetDateTime(startJobTimestampOrdinal),
                        dr.IsDBNull(endJobTimestampOrdinal) ? (DateTime?)null : dr.GetDateTime(endJobTimestampOrdinal),
                        dr.GetInt32(executedOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }

        #endregion

        #region ARCHIVE_JOB_TransferPolicy
        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_TransferPolicy>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_TransferPolicy, related to a transfer </results>
        public List<ARCHIVE_JOB_TransferPolicy> GetARCHIVE_JOB_TransferPolicyByTransfer_ID(Int32 transfer_ID)
        {
            List<ARCHIVE_JOB_TransferPolicy> dataList;
            logger.Debug("[sp_ARCHIVE_JOB_Select_TransferPolicyByTransferID]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", transfer_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_JOB_Select_TransferPolicyByTransferID", sp_params, ds);
                dataList = new List<ARCHIVE_JOB_TransferPolicy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();
                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int transferPolicy_IDOrdinal = dr.GetOrdinal("TransferPolicy_ID");
                int jobType_IDOrdinal = dr.GetOrdinal("JobType_ID");
                int inserJobTimestampOrdinal = dr.GetOrdinal("InsertJobTimestamp");
                int startJobTimestampOrdinal = dr.GetOrdinal("StartJobTimestamp");
                int endJobTimestampOrdinal = dr.GetOrdinal("EndJobTimestamp");
                int executedOrdinal = dr.GetOrdinal("Executed");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_JOB_TransferPolicy(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(transferPolicy_IDOrdinal),
                        dr.GetInt32(jobType_IDOrdinal),
                        dr.GetDateTime(inserJobTimestampOrdinal),
                        dr.IsDBNull(startJobTimestampOrdinal) ? (DateTime?)null : dr.GetDateTime(startJobTimestampOrdinal),
                        dr.IsDBNull(endJobTimestampOrdinal) ? (DateTime?)null : dr.GetDateTime(endJobTimestampOrdinal),
                        dr.GetInt32(executedOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_JOB_TransferPolicy to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public void InsertARCHIVE_JOB_TransferPolicy(Int32 transferPolicy_ID, Int32 jobType_ID, ref Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_JOB_Insert_TransferPolicy]");
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicy_ID", transferPolicy_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@JobType_ID", jobType_ID));

            DocsPaUtils.Data.ParameterSP System_ID = new DocsPaUtils.Data.ParameterSP("@System_ID", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
            sp_params.Add(System_ID);

            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_JOB_Insert_TransferPolicy", sp_params, ds);
            }
            system_ID = (Int32)System_ID.Valore;
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_JOB_TransferPolicy>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_JOB_TransferPolicy. </results>
        public List<ARCHIVE_JOB_TransferPolicy> GetARCHIVE_JOB_TransferPolicyByTransferPolicy_ID(Int32 transferPolicy_ID)
        {
            List<ARCHIVE_JOB_TransferPolicy> dataList;
            logger.Debug("[sp_ARCHIVE_JOB_Select_Transfer_By_ARCHIVE_TransferPolicy_ID_FK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", transferPolicy_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_JOB_Select_Transfer_By_ARCHIVE_TransferPolicy_ID_FK", sp_params, ds);
                dataList = new List<ARCHIVE_JOB_TransferPolicy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();
                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int transferPolicy_IDOrdinal = dr.GetOrdinal("TransferPolicy_ID");
                int jobType_IDOrdinal = dr.GetOrdinal("JobType_ID");
                int inserJobTimestampOrdinal = dr.GetOrdinal("InsertJobTimestamp");
                int startJobTimestampOrdinal = dr.GetOrdinal("StartJobTimestamp");
                int endJobTimestampOrdinal = dr.GetOrdinal("EndJobTimestamp");
                int executedOrdinal = dr.GetOrdinal("Executed");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_JOB_TransferPolicy(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(transferPolicy_IDOrdinal),
                        dr.GetInt32(jobType_IDOrdinal),
                        dr.GetDateTime(inserJobTimestampOrdinal),
                        dr.IsDBNull(startJobTimestampOrdinal) ? (DateTime?)null : dr.GetDateTime(startJobTimestampOrdinal),
                        dr.IsDBNull(endJobTimestampOrdinal) ? (DateTime?)null : dr.GetDateTime(endJobTimestampOrdinal),
                        dr.GetInt32(executedOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }


        #endregion

        #region Utility di Search & porting Transfer Policy


        /// <summary>
        /// Chiamata per lo start della ricerca dei documenti in base alla policy che abbiamo inserito.
        /// </summary>
        /// <param name="system_ID"></param>
        /// <returns></returns>
        public bool StartSearchForTransferPolicy(Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_BE_StartSearchForPolicy]");
                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@PolicyID", system_ID));
                DataSet _ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_StartSearchForPolicy", sp_params, _ds);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// avvia la ricerca della lista di policy passate per parametro
        /// </summary>
        /// <param name="listSystemID"></param>
        /// <returns></returns>
        public bool StartSearchForTransferPolicyList(string listSystemID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_BE_StartSearchForPolicyList]");
                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@policyList", listSystemID));
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoreProcedure("sp_ARCHIVE_BE_StartSearchForPolicyList", sp_params);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// avvia l'analisi della lista di policy passate per parametro
        /// </summary>
        /// <param name="listSystemID"></param>
        /// <returns></returns>
        public bool StartAnalysisForTransferPolicyList(string listSystemID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_BE_StartAnalysisForPolicyList]");
                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@policyList", listSystemID));
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoreProcedure("sp_ARCHIVE_BE_StartAnalysisForPolicyList", sp_params);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool StartAnalysisForTransfer(Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_BE_StartAnalysisForTransfer]");
                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferID", system_ID));
                DataSet _ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_StartAnalysisForTransfer", sp_params, _ds);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns an instance of List<TransferFilterForSearch>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of TransferFilterForSearch. </results>
        public List<ARCHIVE_TransferForSearch> GetAllARCHIVE_TransferFilterForSearch(String st_indefinizione, String st_analisicompletata, String st_proposto,
                                                                               String st_approvato, String st_inesecuzione, String st_effettuato, String st_inerrore, Int32 finger)
        {
            List<ARCHIVE_TransferForSearch> dataList;
            logger.Debug("[sp_ARCHIVE_BE_SearchTransfer]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_InDefinizione", st_indefinizione));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_AnalisiCompletata", st_analisicompletata));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_Proposto", st_proposto));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_Approvato", st_approvato));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_InEsecuzione", st_inesecuzione));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_Effettuato", st_effettuato));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@filtro_InErrore", st_inerrore));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@tipoStatoEsecuzione", finger));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_SearchTransfer", sp_params, ds);
                dataList = new List<ARCHIVE_TransferForSearch>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_idOrdinal = dr.GetOrdinal("ID_VERSAMENTO");

                int id_ammOrdinal = dr.GetOrdinal("ID_AMMINISTRAZIONE");
                int descriptionOrdinal = dr.GetOrdinal("DESCRIZIONE");
                int stato_IDOrdinal = dr.GetOrdinal("STATO");
                int dateTimeOrdinal = dr.GetOrdinal("DATA");
                int numero_doc_effettiviOrdinal = dr.GetOrdinal("NUMERO_DOC_EFFETTIVI");
                int numero_dc_copieOrdinal = dr.GetOrdinal("NUMERO_DOC_COPIE");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_TransferForSearch(
                        dr.GetInt32(system_idOrdinal),
                        dr.GetInt32(id_ammOrdinal),
                        dr.GetString(descriptionOrdinal),
                        dr.GetString(stato_IDOrdinal),
                        dr.GetDateTime(dateTimeOrdinal),
                        dr.GetInt32(numero_doc_effettiviOrdinal),
                        dr.GetInt32(numero_dc_copieOrdinal)));
                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }

        #endregion

        #region ARCHIVE_TransferPolicy

        /// <summary>
        /// Returns an instance of List<ARCHIVE_TransferPolicy>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public List<ARCHIVE_TransferPolicy> GetARCHIVE_TransferPolicyBySystem_ID(Int32 system_ID)
        {
            List<ARCHIVE_TransferPolicy> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TransferPolicy_PK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("system_ID", system_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TransferPolicy_PK", sp_params, ds);
                dataList = new List<ARCHIVE_TransferPolicy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int descriptionOrdinal = dr.GetOrdinal("Description");
                int enabledOrdinal = dr.GetOrdinal("Enabled");
                int transfer_IDOrdinal = dr.GetOrdinal("Transfer_ID");
                int transferPolicyType_IDOrdinal = dr.GetOrdinal("TransferPolicyType_ID");
                int transferPolicyState_IDOrdinal = dr.GetOrdinal("TransferPolicyState_ID");
                int registro_IDOrdinal = dr.GetOrdinal("Registro_ID");
                int uO_IDOrdinal = dr.GetOrdinal("UO_ID");
                int includiSottoalberoUOOrdinal = dr.GetOrdinal("IncludiSottoalberoUO");
                int tipologia_IDOrdinal = dr.GetOrdinal("Tipologia_ID");
                int titolario_IDOrdinal = dr.GetOrdinal("Titolario_ID");
                int classeTitolarioOrdinal = dr.GetOrdinal("ClasseTitolario");
                int includiSottoalberoClasseTitOrdinal = dr.GetOrdinal("IncludiSottoalberoClasseTit");
                int annoCreazioneDaOrdinal = dr.GetOrdinal("AnnoCreazioneDa");
                int annoCreazioneAOrdinal = dr.GetOrdinal("AnnoCreazioneA");
                int annoProtocollazioneDaOrdinal = dr.GetOrdinal("AnnoProtocollazioneDa");
                int annoProtocollazioneAOrdinal = dr.GetOrdinal("AnnoProtocollazioneA");
                int annoChiusuraDaOrdinal = dr.GetOrdinal("AnnoChiusuraDa");
                int annoChiusuraAOrdinal = dr.GetOrdinal("AnnoChiusuraA");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_TransferPolicy(dr.GetInt32(system_IDOrdinal),
                        dr.GetString(descriptionOrdinal),
                        dr.GetInt32(enabledOrdinal),
                        dr.IsDBNull(transfer_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(transfer_IDOrdinal)),
                        dr.GetInt32(transferPolicyType_IDOrdinal),
                        dr.GetInt32(transferPolicyState_IDOrdinal),
                        dr.IsDBNull(registro_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(registro_IDOrdinal)),
                        dr.IsDBNull(uO_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(uO_IDOrdinal)),
                        dr.IsDBNull(includiSottoalberoUOOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(includiSottoalberoUOOrdinal)),
                        dr.IsDBNull(tipologia_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(tipologia_IDOrdinal)),
                        dr.IsDBNull(titolario_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(titolario_IDOrdinal)),
                        dr.IsDBNull(classeTitolarioOrdinal) ? null : dr.GetString(classeTitolarioOrdinal),
                        dr.IsDBNull(includiSottoalberoClasseTitOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(includiSottoalberoClasseTitOrdinal)),
                        dr.IsDBNull(annoCreazioneDaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoCreazioneDaOrdinal)),
                        dr.IsDBNull(annoCreazioneAOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoCreazioneAOrdinal)),
                        dr.IsDBNull(annoProtocollazioneDaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoProtocollazioneDaOrdinal)),
                        dr.IsDBNull(annoProtocollazioneAOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoProtocollazioneAOrdinal)),
                        dr.IsDBNull(annoChiusuraDaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoChiusuraDaOrdinal)),
                        dr.IsDBNull(annoChiusuraAOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoChiusuraAOrdinal))));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }

        /// <summary>
        /// Returns all instances of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public List<ARCHIVE_TransferPolicy> GetAllARCHIVE_TransferPolicy()
        {
            List<ARCHIVE_TransferPolicy> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TransferPolicy_All]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TransferPolicy_All", sp_params, ds);
                dataList = new List<ARCHIVE_TransferPolicy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int descriptionOrdinal = dr.GetOrdinal("Description");
                int enabledOrdinal = dr.GetOrdinal("Enabled");
                int transfer_IDOrdinal = dr.GetOrdinal("Transfer_ID");
                int transferPolicyType_IDOrdinal = dr.GetOrdinal("TransferPolicyType_ID");
                int transferPolicyState_IDOrdinal = dr.GetOrdinal("TransferPolicyState_ID");
                int registro_IDOrdinal = dr.GetOrdinal("Registro_ID");
                int uO_IDOrdinal = dr.GetOrdinal("UO_ID");
                int includiSottoalberoUOOrdinal = dr.GetOrdinal("IncludiSottoalberoUO");
                int tipologia_IDOrdinal = dr.GetOrdinal("Tipologia_ID");
                int titolario_IDOrdinal = dr.GetOrdinal("Titolario_ID");
                int classeTitolarioOrdinal = dr.GetOrdinal("ClasseTitolario");
                int includiSottoalberoClasseTitOrdinal = dr.GetOrdinal("IncludiSottoalberoClasseTit");
                int annoCreazioneDaOrdinal = dr.GetOrdinal("AnnoCreazioneDa");
                int annoCreazioneAOrdinal = dr.GetOrdinal("AnnoCreazioneA");
                int annoProtocollazioneDaOrdinal = dr.GetOrdinal("AnnoProtocollazioneDa");
                int annoProtocollazioneAOrdinal = dr.GetOrdinal("AnnoProtocollazioneA");
                int annoChiusuraDaOrdinal = dr.GetOrdinal("AnnoChiusuraDa");
                int annoChiusuraAOrdinal = dr.GetOrdinal("AnnoChiusuraA");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_TransferPolicy(dr.GetInt32(system_IDOrdinal),
                        dr.GetString(descriptionOrdinal),
                        dr.GetInt32(enabledOrdinal),
                        dr.IsDBNull(transfer_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(transfer_IDOrdinal)),
                        dr.GetInt32(transferPolicyType_IDOrdinal),
                         dr.GetInt32(transferPolicyState_IDOrdinal),
                        dr.IsDBNull(registro_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(registro_IDOrdinal)),
                        dr.IsDBNull(uO_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(uO_IDOrdinal)),
                        dr.IsDBNull(includiSottoalberoUOOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(includiSottoalberoUOOrdinal)),
                        dr.IsDBNull(tipologia_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(tipologia_IDOrdinal)),
                        dr.IsDBNull(titolario_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(titolario_IDOrdinal)),
                        dr.IsDBNull(classeTitolarioOrdinal) ? null : dr.GetString(classeTitolarioOrdinal),
                        dr.IsDBNull(includiSottoalberoClasseTitOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(includiSottoalberoClasseTitOrdinal)),
                        dr.IsDBNull(annoCreazioneDaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoCreazioneDaOrdinal)),
                        dr.IsDBNull(annoCreazioneAOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoCreazioneAOrdinal)),
                        dr.IsDBNull(annoProtocollazioneDaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoProtocollazioneDaOrdinal)),
                        dr.IsDBNull(annoProtocollazioneAOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoProtocollazioneAOrdinal)),
                        dr.IsDBNull(annoChiusuraDaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoChiusuraDaOrdinal)),
                        dr.IsDBNull(annoChiusuraAOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoChiusuraAOrdinal))));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns a collection of ARCHIVE_TransferPolicy based on the following criteria: TransferPolicyType_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public List<ARCHIVE_TransferPolicy> GetARCHIVE_TransferPolicyByTransferPolicyType_ID(Int32 transferPolicyType_ID)
        {
            List<ARCHIVE_TransferPolicy> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TransferPolicy_By_ARCHIVE_TransferPolicyType_TransferPolicyType_ID_FK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicyType_ID", transferPolicyType_ID));
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TransferPolicy_By_ARCHIVE_TransferPolicyType_TransferPolicyType_ID_FK", sp_params, ds);
                dataList = new List<ARCHIVE_TransferPolicy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int descriptionOrdinal = dr.GetOrdinal("Description");
                int enabledOrdinal = dr.GetOrdinal("Enabled");
                int transfer_IDOrdinal = dr.GetOrdinal("Transfer_ID");
                int transferPolicyType_IDOrdinal = dr.GetOrdinal("TransferPolicyType_ID");
                int transferPolicyState_IDOrdinal = dr.GetOrdinal("TransferPolicyState_ID");
                int registro_IDOrdinal = dr.GetOrdinal("Registro_ID");
                int uO_IDOrdinal = dr.GetOrdinal("UO_ID");
                int includiSottoalberoUOOrdinal = dr.GetOrdinal("IncludiSottoalberoUO");
                int tipologia_IDOrdinal = dr.GetOrdinal("Tipologia_ID");
                int titolario_IDOrdinal = dr.GetOrdinal("Titolario_ID");
                int classeTitolarioOrdinal = dr.GetOrdinal("ClasseTitolario");
                int includiSottoalberoClasseTitOrdinal = dr.GetOrdinal("IncludiSottoalberoClasseTit");
                int annoCreazioneDaOrdinal = dr.GetOrdinal("AnnoCreazioneDa");
                int annoCreazioneAOrdinal = dr.GetOrdinal("AnnoCreazioneA");
                int annoProtocollazioneDaOrdinal = dr.GetOrdinal("AnnoProtocollazioneDa");
                int annoProtocollazioneAOrdinal = dr.GetOrdinal("AnnoProtocollazioneA");
                int annoChiusuraDaOrdinal = dr.GetOrdinal("AnnoChiusuraDa");
                int annoChiusuraAOrdinal = dr.GetOrdinal("AnnoChiusuraA");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_TransferPolicy(dr.GetInt32(system_IDOrdinal),
                        dr.GetString(descriptionOrdinal),
                        dr.GetInt32(enabledOrdinal),
                        dr.IsDBNull(transfer_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(transfer_IDOrdinal)),
                        dr.GetInt32(transferPolicyType_IDOrdinal),
                        dr.GetInt32(transferPolicyState_IDOrdinal),
                        dr.IsDBNull(registro_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(registro_IDOrdinal)),
                        dr.IsDBNull(uO_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(uO_IDOrdinal)),
                        dr.IsDBNull(includiSottoalberoUOOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(includiSottoalberoUOOrdinal)),
                        dr.IsDBNull(tipologia_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(tipologia_IDOrdinal)),
                        dr.IsDBNull(titolario_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(titolario_IDOrdinal)),
                        dr.IsDBNull(classeTitolarioOrdinal) ? null : dr.GetString(classeTitolarioOrdinal),
                        dr.IsDBNull(includiSottoalberoClasseTitOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(includiSottoalberoClasseTitOrdinal)),
                        dr.IsDBNull(annoCreazioneDaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoCreazioneDaOrdinal)),
                        dr.IsDBNull(annoCreazioneAOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoCreazioneAOrdinal)),
                        dr.IsDBNull(annoProtocollazioneDaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoProtocollazioneDaOrdinal)),
                        dr.IsDBNull(annoProtocollazioneAOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoProtocollazioneAOrdinal)),
                        dr.IsDBNull(annoChiusuraDaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoChiusuraDaOrdinal)),
                        dr.IsDBNull(annoChiusuraAOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoChiusuraAOrdinal))));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns a collection of ARCHIVE_TransferPolicy based on the following criteria: Transfer_ID.
        /// </summary>
        /// <param name="transfer_ID">This is not a required field. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferPolicy. </results>
        public List<ARCHIVE_TransferPolicy> GetARCHIVE_TransferPolicyByTransfer_ID(Int32? Transfer_ID)
        {
            List<ARCHIVE_TransferPolicy> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TransferPolicy_By_ARCHIVE_Transfer_Transfer_ID_FK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", Transfer_ID));
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TransferPolicy_By_ARCHIVE_Transfer_Transfer_ID_FK", sp_params, ds);
                dataList = new List<ARCHIVE_TransferPolicy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int descriptionOrdinal = dr.GetOrdinal("Description");
                int enabledOrdinal = dr.GetOrdinal("Enabled");
                int transfer_IDOrdinal = dr.GetOrdinal("Transfer_ID");
                int transferPolicyType_IDOrdinal = dr.GetOrdinal("TransferPolicyType_ID");
                int transferPolicyState_IDOrdinal = dr.GetOrdinal("TransferPolicyState_ID");
                int registro_IDOrdinal = dr.GetOrdinal("Registro_ID");
                int uO_IDOrdinal = dr.GetOrdinal("UO_ID");
                int includiSottoalberoUOOrdinal = dr.GetOrdinal("IncludiSottoalberoUO");
                int tipologia_IDOrdinal = dr.GetOrdinal("Tipologia_ID");
                int titolario_IDOrdinal = dr.GetOrdinal("Titolario_ID");
                int classeTitolarioOrdinal = dr.GetOrdinal("ClasseTitolario");
                int includiSottoalberoClasseTitOrdinal = dr.GetOrdinal("IncludiSottoalberoClasseTit");
                int annoCreazioneDaOrdinal = dr.GetOrdinal("AnnoCreazioneDa");
                int annoCreazioneAOrdinal = dr.GetOrdinal("AnnoCreazioneA");
                int annoProtocollazioneDaOrdinal = dr.GetOrdinal("AnnoProtocollazioneDa");
                int annoProtocollazioneAOrdinal = dr.GetOrdinal("AnnoProtocollazioneA");
                int annoChiusuraDaOrdinal = dr.GetOrdinal("AnnoChiusuraDa");
                int annoChiusuraAOrdinal = dr.GetOrdinal("AnnoChiusuraA");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_TransferPolicy(dr.GetInt32(system_IDOrdinal),
                        dr.GetString(descriptionOrdinal),
                        dr.GetInt32(enabledOrdinal),
                        dr.IsDBNull(transfer_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(transfer_IDOrdinal)),
                        dr.GetInt32(transferPolicyType_IDOrdinal),
                        dr.GetInt32(transferPolicyState_IDOrdinal),
                        dr.IsDBNull(registro_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(registro_IDOrdinal)),
                        dr.IsDBNull(uO_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(uO_IDOrdinal)),
                        dr.IsDBNull(includiSottoalberoUOOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(includiSottoalberoUOOrdinal)),
                        dr.IsDBNull(tipologia_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(tipologia_IDOrdinal)),
                        dr.IsDBNull(titolario_IDOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(titolario_IDOrdinal)),
                        dr.IsDBNull(classeTitolarioOrdinal) ? null : dr.GetString(classeTitolarioOrdinal),
                        dr.IsDBNull(includiSottoalberoClasseTitOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(includiSottoalberoClasseTitOrdinal)),
                        dr.IsDBNull(annoCreazioneDaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoCreazioneDaOrdinal)),
                        dr.IsDBNull(annoCreazioneAOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoCreazioneAOrdinal)),
                        dr.IsDBNull(annoProtocollazioneDaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoProtocollazioneDaOrdinal)),
                        dr.IsDBNull(annoProtocollazioneAOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoProtocollazioneAOrdinal)),
                        dr.IsDBNull(annoChiusuraDaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoChiusuraDaOrdinal)),
                        dr.IsDBNull(annoChiusuraAOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoChiusuraAOrdinal))));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferPolicy based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_TransferPolicy(Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Delete_TransferPolicy_PK]");
                //Boolean success;
                //righe modificate nel db
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                ArrayList sp_params = new ArrayList();
                sp_params.Add(rowsAffected);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));


                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    // dbProvider.ExecuteNonQuery("sp_ARCHIVE_Delete_TransferPolicy_PK", out rowsAffected);
                    dbProvider.ExecuteStoreProcedure("sp_ARCHIVE_Delete_TransferPolicy_PK", sp_params);

                    return (Int32)rowsAffected.Valore > 0;

                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_Transfer", e);
                return false;
            }

        }

        /// <summary>
        /// Deletes all instances of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <results>The number of items deleted. </results>
        public Int32 DeleteAllARCHIVE_TransferPolicy()
        {
            logger.Debug("[sp_ARCHIVE_Delete_TransferPolicy_All]");
            ArrayList sp_params = new ArrayList();
            int rowsAffected = 0;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {

                dbProvider.ExecuteNonQuery("sp_ARCHIVE_Delete_TransferPolicy_All", out rowsAffected);
                return rowsAffected;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferPolicy based on TransferPolicyType_ID.
        /// </summary>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_TransferPolicyByTransferPolicyType_ID(Int32 transferPolicyType_ID)
        {
            logger.Debug("[sp_ARCHIVE_Delete_TransferPolicy_By_ARCHIVE_TransferPolicyType_TransferPolicyType_ID_FK]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicyType_ID", transferPolicyType_ID));
            int rowsAffected = 0;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {

                dbProvider.ExecuteNonQuery("sp_ARCHIVE_Delete_TransferPolicy_By_ARCHIVE_TransferPolicyType_TransferPolicyType_ID_FK", out rowsAffected);
                success = (rowsAffected > 0);
                return success;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_TransferPolicy.
        /// </summary>
        /// <param name="transfer_ID">This is not a required field. </param>
        /// <param name="registro_ID">This is not a required field. </param>
        /// <param name="uO_ID">This is not a required field. </param>
        /// <param name="includiSottoalberoUO">This is not a required field. </param>
        /// <param name="tipologia_ID">This is not a required field. </param>
        /// <param name="titolario_ID">This is not a required field. </param>
        /// <param name="classeTitolario">This is not a required field. </param>
        /// <param name="includiSottoalberoClasseTit">This is not a required field. </param>
        /// <param name="annoCreazioneDa">This is not a required field. </param>
        /// <param name="annoCreazioneA">This is not a required field. </param>
        /// <param name="annoProtocollazioneDa">This is not a required field. </param>
        /// <param name="annoProtocollazioneA">This is not a required field. </param>
        /// <param name="annoChiusuraDa">This is not a required field. </param>
        /// <param name="annoChiusuraA">This is not a required field. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_TransferPolicy(String description, Int32 enabled, Int32? transfer_ID, Int32 transferPolicyType_ID, Int32 transferPolicyState_ID, Int32? registro_ID,
            Int32? uO_ID, Int32? includiSottoalberoUO, Int32? tipologia_ID, Int32? titolario_ID, String classeTitolario,
            Int32? includiSottoalberoClasseTit, Int32? annoCreazioneDa, Int32? annoCreazioneA, Int32? annoProtocollazioneDa,
            Int32? annoProtocollazioneA, Int32? annoChiusuraDa, Int32? annoChiusuraA, Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Update_TransferPolicy_PK]");

                //int rowsAffected = 0;
                ArrayList sp_params = new ArrayList();
                //righe modificate nel db
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(rowsAffected);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description", description));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Enabled", enabled));
                if (transfer_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", transfer_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", DBNull.Value));
                }
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicyType_ID", transferPolicyType_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicyState_ID", transferPolicyState_ID));

                if (registro_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Registro_ID", registro_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Registro_ID", DBNull.Value));
                }

                if (includiSottoalberoUO.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO", includiSottoalberoUO));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO", DBNull.Value));
                }

                if (uO_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@UO_ID", uO_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@UO_ID", DBNull.Value));
                }


                if (tipologia_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Tipologia_ID", tipologia_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Tipologia_ID", DBNull.Value));
                }

                if (titolario_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Titolario_ID", titolario_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Titolario_ID", DBNull.Value));
                }

                if (!string.IsNullOrEmpty(classeTitolario))
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ClasseTitolario", classeTitolario));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ClasseTitolario", DBNull.Value));
                }

                if (includiSottoalberoClasseTit.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoClasseTit", includiSottoalberoClasseTit));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoClasseTit", DBNull.Value));
                }

                if (annoCreazioneDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneDa", annoCreazioneDa));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneDa", DBNull.Value));
                }

                if (annoCreazioneA.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneA", annoCreazioneA));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneA", DBNull.Value));
                }

                if (annoProtocollazioneDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneDa", annoProtocollazioneDa));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneDa", DBNull.Value));
                }

                if (annoProtocollazioneDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneA", annoProtocollazioneA));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneA", DBNull.Value));
                }

                if (annoChiusuraDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraDa", annoChiusuraDa));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraDa", DBNull.Value));
                }

                if (annoChiusuraA.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraA", annoChiusuraA));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraA", DBNull.Value));
                }

                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));

                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_TransferPolicy_PK", sp_params, ds);
                }
                int result = (Int32)rowsAffected.Valore;
                return result > 0;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_Transfer", e);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_TransferPolicy if the original data has changed.
        /// </summary>
        /// <param name="description_Original">This field is used for optimistic concurrency management. It should contain the original value of 'description_Original'. </param>
        /// <param name="enabled_Original">This field is used for optimistic concurrency management. It should contain the original value of 'enabled_Original'. </param>
        /// <param name="transfer_ID">This is not a required field. </param>
        /// <param name="transfer_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'transfer_ID_Original'. This is not a required field. </param>
        /// <param name="transferPolicyType_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'transferPolicyType_ID_Original'. </param>
        /// <param name="registro_ID">This is not a required field. </param>
        /// <param name="registro_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'registro_ID_Original'. This is not a required field. </param>
        /// <param name="uO_ID">This is not a required field. </param>
        /// <param name="uO_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'uO_ID_Original'. This is not a required field. </param>
        /// <param name="includiSottoalberoUO">This is not a required field. </param>
        /// <param name="includiSottoalberoUO_Original">This field is used for optimistic concurrency management. It should contain the original value of 'includiSottoalberoUO_Original'. This is not a required field. </param>
        /// <param name="tipologia_ID">This is not a required field. </param>
        /// <param name="tipologia_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'tipologia_ID_Original'. This is not a required field. </param>
        /// <param name="titolario_ID">This is not a required field. </param>
        /// <param name="titolario_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'titolario_ID_Original'. This is not a required field. </param>
        /// <param name="classeTitolario">This is not a required field. </param>
        /// <param name="classeTitolario_Original">This field is used for optimistic concurrency management. It should contain the original value of 'classeTitolario_Original'. This is not a required field. </param>
        /// <param name="includiSottoalberoClasseTit">This is not a required field. </param>
        /// <param name="includiSottoalberoClasseTit_Original">This field is used for optimistic concurrency management. It should contain the original value of 'includiSottoalberoClasseTit_Original'. This is not a required field. </param>
        /// <param name="annoCreazioneDa">This is not a required field. </param>
        /// <param name="annoCreazioneDa_Original">This field is used for optimistic concurrency management. It should contain the original value of 'annoCreazioneDa_Original'. This is not a required field. </param>
        /// <param name="annoCreazioneA">This is not a required field. </param>
        /// <param name="annoCreazioneA_Original">This field is used for optimistic concurrency management. It should contain the original value of 'annoCreazioneA_Original'. This is not a required field. </param>
        /// <param name="annoProtocollazioneDa">This is not a required field. </param>
        /// <param name="annoProtocollazioneDa_Original">This field is used for optimistic concurrency management. It should contain the original value of 'annoProtocollazioneDa_Original'. This is not a required field. </param>
        /// <param name="annoProtocollazioneA">This is not a required field. </param>
        /// <param name="annoProtocollazioneA_Original">This field is used for optimistic concurrency management. It should contain the original value of 'annoProtocollazioneA_Original'. This is not a required field. </param>
        /// <param name="annoChiusuraDa">This is not a required field. </param>
        /// <param name="annoChiusuraDa_Original">This field is used for optimistic concurrency management. It should contain the original value of 'annoChiusuraDa_Original'. This is not a required field. </param>
        /// <param name="annoChiusuraA">This is not a required field. </param>
        /// <param name="annoChiusuraA_Original">This field is used for optimistic concurrency management. It should contain the original value of 'annoChiusuraA_Original'. This is not a required field. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_TransferPolicy(String description, String description_Original, Int32 enabled, Int32 enabled_Original, Int32? transfer_ID,
            Int32? transfer_ID_Original, Int32 transferPolicyType_ID, Int32 transferPolicyType_ID_Original, Int32 transferPolicyState_ID, Int32 transferPolicyState_ID_Original, Int32? registro_ID, Int32? registro_ID_Original,
            Int32? uO_ID, Int32? uO_ID_Original, Int32? includiSottoalberoUO, Int32? includiSottoalberoUO_Original, Int32? tipologia_ID,
            Int32? tipologia_ID_Original, Int32? titolario_ID, Int32? titolario_ID_Original, String classeTitolario, String classeTitolario_Original,
            Int32? includiSottoalberoClasseTit, Int32? includiSottoalberoClasseTit_Original, Int32? annoCreazioneDa, Int32? annoCreazioneDa_Original,
            Int32? annoCreazioneA, Int32? annoCreazioneA_Original, Int32? annoProtocollazioneDa, Int32? annoProtocollazioneDa_Original,
            Int32? annoProtocollazioneA, Int32? annoProtocollazioneA_Original, Int32? annoChiusuraDa, Int32? annoChiusuraDa_Original,
            Int32? annoChiusuraA, Int32? annoChiusuraA_Original, Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Update_TransferPolicy]");
                Boolean success;

                ArrayList sp_params = new ArrayList();
                //righe modificate nel db
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(rowsAffected);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description", description));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description_Original", description_Original));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Enabled", enabled));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Enabled_Original", enabled_Original));

                if (transfer_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", transfer_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", DBNull.Value));
                }

                if (transfer_ID_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID_Original", transfer_ID_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID_Original", DBNull.Value));
                }

                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicyType_ID", transferPolicyType_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicyType_ID_Original", transferPolicyType_ID_Original));

                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicyState_ID", transferPolicyState_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicyState_ID_ID_Original", transferPolicyState_ID_Original));


                if (registro_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Registro_ID", registro_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Registro_ID", DBNull.Value));
                }

                if (registro_ID_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Registro_ID_Original", registro_ID_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Registro_ID_Original", DBNull.Value));
                }

                if (includiSottoalberoUO.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO", includiSottoalberoUO));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO", DBNull.Value));
                }

                if (includiSottoalberoUO_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO_Original", includiSottoalberoUO_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO_Original", DBNull.Value));
                }

                if (uO_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@UO_ID", uO_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@UO_ID", DBNull.Value));
                }

                if (uO_ID_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@UO_ID_Original", uO_ID_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@UO_ID_Original", DBNull.Value));
                }


                if (includiSottoalberoUO.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO", includiSottoalberoUO));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO", DBNull.Value));
                }

                if (includiSottoalberoUO_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO_Original", includiSottoalberoUO_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO_Original", DBNull.Value));
                }


                if (tipologia_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Tipologia_ID", tipologia_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Tipologia_ID", DBNull.Value));
                }

                if (tipologia_ID_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Tipologia_ID_Original", tipologia_ID_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Tipologia_ID_Original", DBNull.Value));
                }

                if (titolario_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Titolario_ID", titolario_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Titolario_ID", DBNull.Value));
                }

                if (titolario_ID_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Titolario_ID_Original", titolario_ID_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Titolario_ID_Original", DBNull.Value));
                }


                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ClasseTitolario", classeTitolario));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ClasseTitolario_Original", classeTitolario_Original));

                if (includiSottoalberoClasseTit.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoClasseTit", includiSottoalberoClasseTit));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoClasseTit", DBNull.Value));
                }

                if (includiSottoalberoClasseTit_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoClasseTit_Original", includiSottoalberoClasseTit_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoClasseTit_Original", DBNull.Value));
                }


                if (annoCreazioneDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneDa", annoCreazioneDa));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneDa", DBNull.Value));
                }

                if (annoCreazioneDa_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneDa_Original", annoCreazioneDa_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneDa_Original", DBNull.Value));
                }

                if (annoCreazioneA.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneA", annoCreazioneA));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneA", DBNull.Value));
                }


                if (annoCreazioneA_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneA_Original", annoCreazioneA_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneA_Original", DBNull.Value));
                }



                if (annoProtocollazioneDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneDa", annoProtocollazioneDa));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneDa", DBNull.Value));
                }

                if (annoProtocollazioneDa_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneDa_Original", annoProtocollazioneDa_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneDa_Original", DBNull.Value));
                }



                if (annoProtocollazioneDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneA", annoProtocollazioneA));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneA", DBNull.Value));
                }

                if (annoProtocollazioneDa_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneA_Original", annoProtocollazioneA_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneA_Original", DBNull.Value));
                }



                if (annoChiusuraDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraDa", annoChiusuraDa));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraDa", DBNull.Value));
                }

                if (annoChiusuraDa_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraDa_Original", annoChiusuraDa_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraDa_Original", DBNull.Value));
                }


                if (annoChiusuraA.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraA", annoChiusuraA));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraA", DBNull.Value));
                }

                if (annoChiusuraA_Original.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraA_Original", annoChiusuraA_Original));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraA_Original", DBNull.Value));
                }

                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));

                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_TransferPolicy", sp_params, ds);
                }


                success = (Int32)rowsAffected.Valore > 0;
                return success;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_TransferPolicy", ex);
                return false;
            }
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_TransferPolicy to the database.
        /// </summary>
        /// <param name="transfer_ID">This is not a required field. </param>
        /// <param name="registro_ID">This is not a required field. </param>
        /// <param name="UO_ID">This is not a required field. </param>
        /// <param name="includiSottoalberoUO">This is not a required field. </param>
        /// <param name="tipologia_ID">This is not a required field. </param>
        /// <param name="titolario_ID">This is not a required field. </param>
        /// <param name="classeTitolario">This is not a required field. </param>
        /// <param name="includiSottoalberoClasseTit">This is not a required field. </param>
        /// <param name="annoCreazioneDa">This is not a required field. </param>
        /// <param name="annoCreazioneA">This is not a required field. </param>
        /// <param name="annoProtocollazioneDa">This is not a required field. </param>
        /// <param name="annoProtocollazioneA">This is not a required field. </param>
        /// <param name="annoChiusuraDa">This is not a required field. </param>
        /// <param name="annoChiusuraA">This is not a required field. </param>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public void InsertARCHIVE_TransferPolicy(String description, Int32 enabled, Int32? transfer_ID, Int32 transferPolicyType_ID, Int32 transferPolicyState_ID, Int32? registro_ID,
            Int32? UO_ID, Int32? includiSottoalberoUO, Int32? tipologia_ID, Int32? titolario_ID, String classeTitolario,
            Int32? includiSottoalberoClasseTit, Int32? annoCreazioneDa, Int32? annoCreazioneA, Int32? annoProtocollazioneDa,
            Int32? annoProtocollazioneA, Int32? annoChiusuraDa, Int32? annoChiusuraA, ref Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Insert_TransferPolicy]");
                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description", description));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Enabled", enabled));

                DocsPaUtils.Data.ParameterSP System_ID = new DocsPaUtils.Data.ParameterSP("@System_ID", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(System_ID);

                if (transfer_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", transfer_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", DBNull.Value));
                }
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicyType_ID", transferPolicyType_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicyState_ID", transferPolicyState_ID));

                if (registro_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Registro_ID", registro_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Registro_ID", DBNull.Value));
                }

                if (includiSottoalberoUO.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO", includiSottoalberoUO));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO", DBNull.Value));
                }

                if (UO_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@UO_ID", UO_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@UO_ID", DBNull.Value));
                }


                if (tipologia_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Tipologia_ID", tipologia_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Tipologia_ID", DBNull.Value));
                }

                if (titolario_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Titolario_ID", titolario_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Titolario_ID", DBNull.Value));
                }

                if (!string.IsNullOrEmpty(classeTitolario))
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ClasseTitolario", classeTitolario));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ClasseTitolario", DBNull.Value));
                }

                if (includiSottoalberoClasseTit.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoClasseTit", includiSottoalberoClasseTit));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoClasseTit", DBNull.Value));
                }

                if (annoCreazioneDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneDa", annoCreazioneDa));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneDa", DBNull.Value));
                }

                if (annoCreazioneA.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneA", annoCreazioneA));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneA", DBNull.Value));
                }

                if (annoProtocollazioneDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneDa", annoProtocollazioneDa));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneDa", DBNull.Value));
                }

                if (annoProtocollazioneDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneA", annoProtocollazioneA));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneA", DBNull.Value));
                }

                if (annoChiusuraDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraDa", annoChiusuraDa));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraDa", DBNull.Value));
                }

                if (annoChiusuraA.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraA", annoChiusuraA));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraA", DBNull.Value));
                }


                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Insert_TransferPolicy", sp_params, ds);
                }
                system_ID = (Int32)System_ID.Valore;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_TransferPolicy", ex);
                system_ID = 0;
            }
        }

        /// <summary>
        /// Persists a new instance, or update an existing istance of ARCHIVE_TransferPolicy to the database.
        /// </summary>
        /// <param name="transfer_ID">This is not a required field. </param>
        /// <param name="registro_ID">This is not a required field. </param>
        /// <param name="UO_ID">This is not a required field. </param>
        /// <param name="includiSottoalberoUO">This is not a required field. </param>
        /// <param name="tipologia_ID">This is not a required field. </param>
        /// <param name="titolario_ID">This is not a required field. </param>
        /// <param name="classeTitolario">This is not a required field. </param>
        /// <param name="includiSottoalberoClasseTit">This is not a required field. </param>
        /// <param name="annoCreazioneDa">This is not a required field. </param>
        /// <param name="annoCreazioneA">This is not a required field. </param>
        /// <param name="annoProtocollazioneDa">This is not a required field. </param>
        /// <param name="annoProtocollazioneA">This is not a required field. </param>
        /// <param name="annoChiusuraDa">This is not a required field. </param>
        /// <param name="annoChiusuraA">This is not a required field. </param>
        /// <param name="isA">true if type of policy is in arrivo</param>
        /// <param name="isP">true if type of policy is in partenza</param>
        /// <param name="isI">true if type of policy is interno</param>
        /// <param name="isNonProt">true if type of policy is non protcollato</param>
        /// <param name="isStRegProt">true if type of policy is stampa registro protocollo</param>
        /// <param name="isStRep">true if type of policy is stampa repertorio</param> 
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public void Insert_UpdateARCHIVE_TransferPolicy(String description, Int32 enabled, Int32? transfer_ID, Int32 transferPolicyType_ID, Int32 transferPolicyState_ID, Int32? registro_ID,
            Int32? UO_ID, Int32? includiSottoalberoUO, Int32? tipologia_ID, Int32? titolario_ID, String classeTitolario,
            Int32? includiSottoalberoClasseTit, Int32? annoCreazioneDa, Int32? annoCreazioneA, Int32? annoProtocollazioneDa,
            Int32? annoProtocollazioneA, Int32? annoChiusuraDa, Int32? annoChiusuraA, bool isA, bool isP,
            bool isI, bool isNonProt, bool isStRegProt, bool isStRep, ref Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Insert_Update_TransferPolicy]");
                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description", description));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Enabled", enabled));
                DocsPaUtils.Data.ParameterSP System_ID;
                if (system_ID > 0)
                    System_ID = new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID);
                else
                    System_ID = new DocsPaUtils.Data.ParameterSP("@System_ID", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);

                sp_params.Add(System_ID);

                if (transfer_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", transfer_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", DBNull.Value));
                }
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicyType_ID", transferPolicyType_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicyState_ID", transferPolicyState_ID));

                if (registro_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Registro_ID", registro_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Registro_ID", DBNull.Value));
                }

                if (includiSottoalberoUO.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO", includiSottoalberoUO));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoUO", DBNull.Value));
                }

                if (UO_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@UO_ID", UO_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@UO_ID", DBNull.Value));
                }


                if (tipologia_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Tipologia_ID", tipologia_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Tipologia_ID", DBNull.Value));
                }

                if (titolario_ID.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Titolario_ID", titolario_ID));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Titolario_ID", DBNull.Value));
                }

                if (!string.IsNullOrEmpty(classeTitolario))
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ClasseTitolario", classeTitolario));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ClasseTitolario", DBNull.Value));
                }

                if (includiSottoalberoClasseTit.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoClasseTit", includiSottoalberoClasseTit));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IncludiSottoalberoClasseTit", DBNull.Value));
                }

                if (annoCreazioneDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneDa", annoCreazioneDa));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneDa", DBNull.Value));
                }

                if (annoCreazioneA.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneA", annoCreazioneA));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoCreazioneA", DBNull.Value));
                }

                if (annoProtocollazioneDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneDa", annoProtocollazioneDa));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneDa", DBNull.Value));
                }

                if (annoProtocollazioneDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneA", annoProtocollazioneA));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoProtocollazioneA", DBNull.Value));
                }

                if (annoChiusuraDa.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraDa", annoChiusuraDa));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraDa", DBNull.Value));
                }

                if (annoChiusuraA.HasValue)
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraA", annoChiusuraA));
                }
                else
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("@AnnoChiusuraA", DBNull.Value));
                }

                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IsA", isA ? 1 : 0));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IsP", isP ? 1 : 0));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IsI", isI ? 1 : 0));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IsNonProt", isNonProt ? 1 : 0));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IsStRegProt", isStRegProt ? 1 : 0));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@IsStRep", isStRep ? 1 : 0));

                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Insert_Update_TransferPolicy", sp_params, ds);
                }
                system_ID = (Int32)System_ID.Valore;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: Insert_UpdateARCHIVE_TransferPolicy", ex);
                system_ID = 0;
            }
        }

        #endregion

        #region ARCHIVE_Profile_Transfer AND TransferPolicy

        /// <summary>
        /// Returns an instance of List<ARCHIVE_Profile_TransferPolicy>.
        /// </summary>
        /// <param name="TransferPolicy_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Profile_TransferPolicy. </results>
        public List<ARCHIVE_Profile_TransferPolicy> GetARCHIVE_Profile_TransferPolicyByTransferPolicy_ID(Int32 TransferPolicy_ID)
        {
            List<ARCHIVE_Profile_TransferPolicy> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TempProfile_TransferPolicy_ID]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("transferPolicy_ID", TransferPolicy_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TempProfile_TransferPolicy_ID", sp_params, ds);
                dataList = new List<ARCHIVE_Profile_TransferPolicy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int transferPolicy_IDOrdinal = dr.GetOrdinal("TransferPolicy_ID");
                int profile_IDOrdinal = dr.GetOrdinal("Profile_ID");
                int dataUltimoAccessoOrdinal = dr.GetOrdinal("DataUltimoAccesso");
                int numeroUtentiAccedutiUltimoAnnoOrdinal = dr.GetOrdinal("NumeroUtentiAccedutiUltimoAnno");
                int numeroAccessiUltimoAnnoOrdinal = dr.GetOrdinal("NumeroAccessiUltimoAnno");
                int tipoTrasferimento_PolicyOrdinal = dr.GetOrdinal("TipoTrasferimento_Policy");
                int tipoTrasferimento_VersamentoOrdinal = dr.GetOrdinal("TipoTrasferimento_Versamento");
                int copiaPerCatenaDoc_PolicyOrdinal = dr.GetOrdinal("CopiaPerCatenaDoc_Policy");
                int copiaPerConservazione_PolicyOrdinal = dr.GetOrdinal("CopiaPerConservazione_Policy");
                int copiaPerFascicolo_PolicyOrdinal = dr.GetOrdinal("CopiaPerFascicolo_Policy");
                int copiaPerCatenaDoc_VersamentoOrdinal = dr.GetOrdinal("CopiaPerCatenaDoc_Versamento");
                int copiaPerConservazione_VersamentoOrdinal = dr.GetOrdinal("CopiaPerConservazione_Versamento");
                int copiaPerFascicolo_VersamentoOrdinal = dr.GetOrdinal("CopiaPerFascicolo_Versamento");
                int oggettoDocumentoOrdinal = dr.GetOrdinal("OggettoDocumento");
                int tipoDocumentoOrdinal = dr.GetOrdinal("TipoDocumento");
                int registroOrdinal = dr.GetOrdinal("Registro");
                int uOOrdinal = dr.GetOrdinal("UO");
                int tipologiaOrdinal = dr.GetOrdinal("Tipologia");
                int dataCreazioneOrdinal = dr.GetOrdinal("DataCreazione");
                int mantieniCopiaOrdinal = dr.GetOrdinal("MantieniCopia");

                while (dr.Read())
                {

                    dataList.Add(new ARCHIVE_Profile_TransferPolicy(
                        dr.GetInt32(transferPolicy_IDOrdinal),
                        dr.GetInt32(profile_IDOrdinal),
                        dr.IsDBNull(dataUltimoAccessoOrdinal) ? null : new Nullable<DateTime>(dr.GetDateTime(dataUltimoAccessoOrdinal)),
                        dr.IsDBNull(numeroUtentiAccedutiUltimoAnnoOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(numeroUtentiAccedutiUltimoAnnoOrdinal)),
                        dr.IsDBNull(numeroAccessiUltimoAnnoOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(numeroAccessiUltimoAnnoOrdinal)),
                        dr.IsDBNull(tipoTrasferimento_PolicyOrdinal) ? String.Empty : dr.GetString(tipoTrasferimento_PolicyOrdinal),
                        dr.IsDBNull(tipoTrasferimento_VersamentoOrdinal) ? String.Empty : dr.GetString(tipoTrasferimento_VersamentoOrdinal),
                        dr.IsDBNull(copiaPerCatenaDoc_PolicyOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(copiaPerCatenaDoc_PolicyOrdinal)),
                        dr.IsDBNull(copiaPerConservazione_PolicyOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(copiaPerConservazione_PolicyOrdinal)),
                        dr.IsDBNull(copiaPerFascicolo_PolicyOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(copiaPerFascicolo_PolicyOrdinal)),
                        dr.IsDBNull(copiaPerCatenaDoc_VersamentoOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(copiaPerCatenaDoc_VersamentoOrdinal)),
                        dr.IsDBNull(copiaPerConservazione_VersamentoOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(copiaPerConservazione_VersamentoOrdinal)),
                        dr.IsDBNull(copiaPerFascicolo_VersamentoOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(copiaPerFascicolo_VersamentoOrdinal)),
                        dr.IsDBNull(oggettoDocumentoOrdinal) ? String.Empty : dr.GetString(oggettoDocumentoOrdinal),
                        dr.IsDBNull(tipoDocumentoOrdinal) ? String.Empty : dr.GetString(tipoDocumentoOrdinal),
                        dr.IsDBNull(registroOrdinal) ? String.Empty : dr.GetString(registroOrdinal),
                        dr.IsDBNull(uOOrdinal) ? String.Empty : dr.GetString(uOOrdinal),
                        dr.IsDBNull(tipologiaOrdinal) ? String.Empty : dr.GetString(tipologiaOrdinal),
                        dr.IsDBNull(dataCreazioneOrdinal) ? null : new Nullable<DateTime>(dr.GetDateTime(dataCreazioneOrdinal)),
                        dr.IsDBNull(mantieniCopiaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(mantieniCopiaOrdinal))));

                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }

        /// <summary>
        /// Returns an instance of List<ARCHIVE_Profile_TransferPolicy>.
        /// </summary>
        /// <param name="TransferPolicyList">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Profile_TransferPolicy. </results>
        public List<ARCHIVE_Profile_TransferPolicy> GetARCHIVE_Profile_TransferPolicyByTransferPolicyList(string TransferPolicyList)
        {
            List<ARCHIVE_Profile_TransferPolicy> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TempProfile_TransferPolicyList]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("transferPolicyList", TransferPolicyList));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TempProfile_TransferPolicyList", sp_params, ds);
                dataList = new List<ARCHIVE_Profile_TransferPolicy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int transferPolicy_IDOrdinal = dr.GetOrdinal("TransferPolicy_ID");
                int profile_IDOrdinal = dr.GetOrdinal("Profile_ID");
                int dataUltimoAccessoOrdinal = dr.GetOrdinal("DataUltimoAccesso");
                int numeroUtentiAccedutiUltimoAnnoOrdinal = dr.GetOrdinal("NumeroUtentiAccedutiUltimoAnno");
                int numeroAccessiUltimoAnnoOrdinal = dr.GetOrdinal("NumeroAccessiUltimoAnno");
                int tipoTrasferimento_PolicyOrdinal = dr.GetOrdinal("TipoTrasferimento_Policy");
                int tipoTrasferimento_VersamentoOrdinal = dr.GetOrdinal("TipoTrasferimento_Versamento");
                int copiaPerCatenaDoc_PolicyOrdinal = dr.GetOrdinal("CopiaPerCatenaDoc_Policy");
                int copiaPerConservazione_PolicyOrdinal = dr.GetOrdinal("CopiaPerConservazione_Policy");
                int copiaPerFascicolo_PolicyOrdinal = dr.GetOrdinal("CopiaPerFascicolo_Policy");
                int copiaPerCatenaDoc_VersamentoOrdinal = dr.GetOrdinal("CopiaPerCatenaDoc_Versamento");
                int copiaPerConservazione_VersamentoOrdinal = dr.GetOrdinal("CopiaPerConservazione_Versamento");
                int copiaPerFascicolo_VersamentoOrdinal = dr.GetOrdinal("CopiaPerFascicolo_Versamento");
                int oggettoDocumentoOrdinal = dr.GetOrdinal("OggettoDocumento");
                int tipoDocumentoOrdinal = dr.GetOrdinal("TipoDocumento");
                int registroOrdinal = dr.GetOrdinal("Registro");
                int uOOrdinal = dr.GetOrdinal("UO");
                int tipologiaOrdinal = dr.GetOrdinal("Tipologia");
                int dataCreazioneOrdinal = dr.GetOrdinal("DataCreazione");
                int mantieniCopiaOrdinal = dr.GetOrdinal("MantieniCopia");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_Profile_TransferPolicy(
                        dr.GetInt32(transferPolicy_IDOrdinal),
                        dr.GetInt32(profile_IDOrdinal),
                        dr.IsDBNull(dataUltimoAccessoOrdinal) ? null : new Nullable<DateTime>(dr.GetDateTime(dataUltimoAccessoOrdinal)),
                        dr.IsDBNull(numeroUtentiAccedutiUltimoAnnoOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(numeroUtentiAccedutiUltimoAnnoOrdinal)),
                        dr.IsDBNull(numeroAccessiUltimoAnnoOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(numeroAccessiUltimoAnnoOrdinal)),
                        dr.IsDBNull(tipoTrasferimento_PolicyOrdinal) ? String.Empty : dr.GetString(tipoTrasferimento_PolicyOrdinal),
                        dr.IsDBNull(tipoTrasferimento_VersamentoOrdinal) ? String.Empty : dr.GetString(tipoTrasferimento_VersamentoOrdinal),
                        dr.IsDBNull(copiaPerCatenaDoc_PolicyOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(copiaPerCatenaDoc_PolicyOrdinal)),
                        dr.IsDBNull(copiaPerConservazione_PolicyOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(copiaPerConservazione_PolicyOrdinal)),
                        dr.IsDBNull(copiaPerFascicolo_PolicyOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(copiaPerFascicolo_PolicyOrdinal)),
                        dr.IsDBNull(copiaPerCatenaDoc_VersamentoOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(copiaPerCatenaDoc_VersamentoOrdinal)),
                        dr.IsDBNull(copiaPerConservazione_VersamentoOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(copiaPerConservazione_VersamentoOrdinal)),
                        dr.IsDBNull(copiaPerFascicolo_VersamentoOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(copiaPerFascicolo_VersamentoOrdinal)),
                        dr.IsDBNull(oggettoDocumentoOrdinal) ? String.Empty : dr.GetString(oggettoDocumentoOrdinal),
                        dr.IsDBNull(tipoDocumentoOrdinal) ? String.Empty : dr.GetString(tipoDocumentoOrdinal),
                        dr.IsDBNull(registroOrdinal) ? String.Empty : dr.GetString(registroOrdinal),
                        dr.IsDBNull(uOOrdinal) ? String.Empty : dr.GetString(uOOrdinal),
                        dr.IsDBNull(tipologiaOrdinal) ? String.Empty : dr.GetString(tipologiaOrdinal),
                        dr.IsDBNull(dataCreazioneOrdinal) ? null : new Nullable<DateTime>(dr.GetDateTime(dataCreazioneOrdinal)),
                        dr.IsDBNull(mantieniCopiaOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(mantieniCopiaOrdinal))));

                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }


        /// <summary>
        /// Update temp profile by profileList and transferID
        /// </summary>
        public Boolean UpdateARCHIVE_Profile_TransferPolicyByProfileList(string ProfileList, Int32 TransferID)
        {
            try
            {
                List<ARCHIVE_Profile_TransferPolicy> dataList;
                logger.Debug("[sp_ARCHIVE_Update_TempProfile_MantieniCopia]");
                DataSet ds = new DataSet();

                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("ProfileListID", ProfileList));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("TransferID", TransferID));

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_TempProfile_MantieniCopia", sp_params, ds);
                    dataList = new List<ARCHIVE_Profile_TransferPolicy>();


                    ds.Dispose();
                }
                return true;
            }
            catch
            {
                return false;

            }
        }
        #endregion

        #region ARCHIVE_TransferPolicy_ProfileTipe

        /// <summary>
        /// Deletes an instance of ARCHIVE_Transferpolicy_ProfileType based on TransferPolicy_ID and ProfileType_ID.
        /// </summary>
        /// <param name="TransferPolicy_ID">The ID of  </param>
        /// <param name="ProfileType_ID">  </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_Transferpolicy_ProfileType(Int32 TransferPolicy_ID, Int32 ProfileType_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Delete_Transferpolicy_ProfileType_PK]");

                ArrayList sp_params = new ArrayList();
                //righe modificate nel db
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(rowsAffected);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicy_ID", TransferPolicy_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ProfileType_ID", ProfileType_ID));
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoreProcedure("sp_ARCHIVE_Delete_Transferpolicy_ProfileType_PK", sp_params);


                    return (Int32)rowsAffected.Valore > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_Transferpolicy_ProfileType", ex);
                return false;
            }
        }

        /// <summary>
        /// Persists a new instance of ARCHIVE_TransferPolicy_ProfileType to the database.
        /// </summary>
        /// <results>Returns true if it's stored, otherwise false.</results>
        public Boolean InsertARCHIVE_TransferPolicy_ProfileType(Int32 TransferPolicy_ID, Int32 ProfileType)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Insert_TransferPolicy_ProfileType]");
                ArrayList sp_params = new ArrayList();
                //righe modificate nel db
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(rowsAffected);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicy_ID", TransferPolicy_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ProfileType", ProfileType));

                //DocsPaUtils.Data.ParameterSP System_ID = new DocsPaUtils.Data.ParameterSP("@System_ID", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                //sp_params.Add(System_ID);
                //DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoreProcedure("sp_ARCHIVE_Insert_TransferPolicy_ProfileType", sp_params);
                }
                return (Int32)rowsAffected.Valore > 0;
                //system_ID = (Int32)System_ID.Valore;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: InsertARCHIVE_TransferPolicy_ProfileType", ex);
                return false;
            }

        }

        public List<ARCHIVE_TransferPolicy_ProfileType> GetARCHIVE_TransferPolicy_ProfileTypeByTransferPolicyID(int TransferPolicy_ID)
        {
            List<ARCHIVE_TransferPolicy_ProfileType> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TransferPolicy_ProfileType_By_TransferPolicy_ID]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferPolicy_ID", TransferPolicy_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TransferPolicy_ProfileType_By_TransferPolicy_ID", sp_params, ds);
                dataList = new List<ARCHIVE_TransferPolicy_ProfileType>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("TransferPolicy_ID");
                int descriptionOrdinal = dr.GetOrdinal("ProfileType_ID");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_TransferPolicy_ProfileType(dr.GetInt32(system_IDOrdinal), dr.GetInt32(descriptionOrdinal)));

                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }

        }

        #endregion

        #region ARCHIVE_Transfer

        /// <summary>
        /// Returns an instance of List<ARCHIVE_Transfer>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Transfer. </results>
        public List<ARCHIVE_Transfer> GetARCHIVE_TransferBySystem_ID(Int32 system_ID)
        {
            List<ARCHIVE_Transfer> dataList;
            logger.Debug("[sp_ARCHIVE_Select_Transfer_PK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("system_ID", system_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_Transfer_PK", sp_params, ds);
                dataList = new List<ARCHIVE_Transfer>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();
                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int iD_AmministrazioneOrdinal = dr.GetOrdinal("ID_Amministrazione");
                int descriptionOrdinal = dr.GetOrdinal("Description");
                int noteOrdinal = dr.GetOrdinal("Note");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_Transfer(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(iD_AmministrazioneOrdinal),
                        dr.GetString(descriptionOrdinal),
                        dr.IsDBNull(noteOrdinal) ? null : dr.GetString(noteOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns all instances of ARCHIVE_Transfer.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_Transfer. </results>
        public List<ARCHIVE_Transfer> GetAllARCHIVE_Transfer()
        {
            List<ARCHIVE_Transfer> dataList;
            logger.Debug("[sp_ARCHIVE_Select_Transfer_All]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_Transfer_All", sp_params, ds);
                dataList = new List<ARCHIVE_Transfer>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();
                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int iD_AmministrazioneOrdinal = dr.GetOrdinal("ID_Amministrazione");
                int descriptionOrdinal = dr.GetOrdinal("Description");
                int noteOrdinal = dr.GetOrdinal("Note");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_Transfer(dr.GetInt32(system_IDOrdinal),
                       dr.GetInt32(iD_AmministrazioneOrdinal),
                       dr.GetString(descriptionOrdinal),
                       dr.IsDBNull(noteOrdinal) ? null : dr.GetString(noteOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_Transfer based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_Transfer(Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Delete_Transfer_PK]");
                ArrayList sp_params = new ArrayList();
                //righe modificate nel db
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(rowsAffected);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@system_ID", system_ID));

                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Delete_Transfer_PK", sp_params, ds);
                }
                return (Int32)rowsAffected.Valore > 0;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_Transfer", ex);
                return false;
            }

        }
        /// <summary>
        /// Updates an instance of ARCHIVE_Transfer.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_Transfer(String description, String note, Int32 system_ID, Int32 iD_Amministrazione)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Update_Transfer_PK]");

                ArrayList sp_params = new ArrayList();
                //righe modificate nel db
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(rowsAffected);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description", description));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Note", note));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ID_Amministrazione", iD_Amministrazione));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoreProcedure("sp_ARCHIVE_Update_Transfer_PK", sp_params);
                }
                return (Int32)rowsAffected.Valore > 0;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_Transfer", e);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_Transfer if the original data has not changed.
        /// </summary>
        /// <param name="description_Original">This field is used for optimistic concurrency management. It should contain the original value of 'description_Original'. </param>
        /// <param name="note">This is not a required field. </param>
        /// <param name="note_Original">This field is used for optimistic concurrency management. It should contain the original value of 'note_Original'. This is not a required field. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_Transfer(String description, String description_Original, String note, String note_Original,
                                                Int32 id_Amministrazione, Int32 id_Amministrazione_Original, Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Update_Transfer]");
                Boolean success;

                ArrayList sp_params = new ArrayList();
                //righe modificate nel db
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(rowsAffected);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description", description));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description_Original", description_Original));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Note", note));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Note_Original", note));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ID_Amministrazione", id_Amministrazione));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ID_Amministrazione_Original", id_Amministrazione_Original));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_Transfer", sp_params, ds);
                }

                success = (Int32)rowsAffected.Valore > 0;
                return success;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_Transfer", e);
                return false;
            }
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_Transfer to the database.
        /// </summary>
        /// <param name="note">This is not a required field. </param>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public void InsertARCHIVE_Transfer(String description, String note, ref Int32 system_ID, Int32 id_Amministrazione, Int32 transferStateType_ID)
        {

            logger.Debug("[sp_ARCHIVE_Insert_Transfer]");
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Description", description));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Note", note));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@ID_Amministrazione", id_Amministrazione));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferStateType_ID", transferStateType_ID));
            DocsPaUtils.Data.ParameterSP System_ID = new DocsPaUtils.Data.ParameterSP("@System_ID", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
            sp_params.Add(System_ID);

            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Insert_Transfer", sp_params, ds);
            }
            system_ID = (Int32)System_ID.Valore;
        }


        #endregion

        #region ARCHIVE_TransferState

        /// <summary>
        /// Returns all instances of ARCHIVE_TransferState.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferState. </results>
        public List<ARCHIVE_TransferState> GetAllARCHIVE_TransferState()
        {
            List<ARCHIVE_TransferState> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TransferState_All]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TransferState_All", sp_params, ds);
                dataList = new List<ARCHIVE_TransferState>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int transfer_IDOrdinal = dr.GetOrdinal("Transfer_ID");
                int transferStateType_IDOrdinal = dr.GetOrdinal("TransferStateType_ID");
                int dateTimeOrdinal = dr.GetOrdinal("DateTime");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_TransferState(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(transfer_IDOrdinal),
                        dr.GetInt32(transferStateType_IDOrdinal),
                        dr.GetDateTime(dateTimeOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_TransferState>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferState. </results>
        public List<ARCHIVE_TransferState> GetARCHIVE_TransferStateBySystem_ID(Int32 system_ID)
        {

            List<ARCHIVE_TransferState> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TransferState_PK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("system_ID", system_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TransferState_PK", sp_params, ds);
                dataList = new List<ARCHIVE_TransferState>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int transfer_IDOrdinal = dr.GetOrdinal("Transfer_ID");
                int transferStateType_IDOrdinal = dr.GetOrdinal("TransferStateType_ID");
                int dateTimeOrdinal = dr.GetOrdinal("DateTime");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_TransferState(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(transfer_IDOrdinal),
                        dr.GetInt32(transferStateType_IDOrdinal),
                        dr.GetDateTime(dateTimeOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_TransferState>.
        /// </summary>
        /// <param name="transfer_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferState. </results>
        public List<ARCHIVE_TransferState> GetARCHIVE_TransferStateByTransfer_ID(Int32 transfer_ID)
        {

            List<ARCHIVE_TransferState> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TransferState_By_ARCHIVE_Transfer_Transfer_ID_FK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("transfer_ID", transfer_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TransferState_By_ARCHIVE_Transfer_Transfer_ID_FK", sp_params, ds);
                dataList = new List<ARCHIVE_TransferState>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int transfer_IDOrdinal = dr.GetOrdinal("Transfer_ID");
                int transferStateType_IDOrdinal = dr.GetOrdinal("TransferStateType_ID");
                int dateTimeOrdinal = dr.GetOrdinal("DateTime");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_TransferState(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(transfer_IDOrdinal),
                        dr.GetInt32(transferStateType_IDOrdinal),
                        dr.GetDateTime(dateTimeOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns a collection of ARCHIVE_TransferState based on the following criteria: TransferStateType_ID.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferState. </results>
        public List<ARCHIVE_TransferState> GetARCHIVE_TransferStateByTransferStateType_ID(Int32 transferStateType_ID)
        {
            List<ARCHIVE_TransferState> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TransferState_By_ARCHIVE_TransferStateType_TransferStateType_ID_FK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferStateType_ID", transferStateType_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TransferState_By_ARCHIVE_TransferStateType_TransferStateType_ID_FK", sp_params, ds);
                dataList = new List<ARCHIVE_TransferState>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int transfer_IDOrdinal = dr.GetOrdinal("Transfer_ID");
                int transferStateType_IDOrdinal = dr.GetOrdinal("TransferStateType_ID");
                int dateTimeOrdinal = dr.GetOrdinal("DateTime");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_TransferState(dr.GetInt32(system_IDOrdinal),
                        dr.GetInt32(transfer_IDOrdinal),
                        dr.GetInt32(transferStateType_IDOrdinal),
                        dr.GetDateTime(dateTimeOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferState based on System_ID.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_TransferState(Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Delete_TransferState_PK]");
                ArrayList sp_params = new ArrayList();
                //righe modificate nel db
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(rowsAffected);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@system_ID", system_ID));

                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Delete_TransferState_PK", sp_params, ds);
                }
                return (Int32)rowsAffected.Valore > 0;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_TransferState", ex);
                return false;
            }


        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferState based on Transfer_ID.
        /// </summary>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_TransferStateByTransfer_ID(Int32 transfer_ID)
        {
            try
            {

                logger.Debug("[sp_ARCHIVE_Delete_TransferState_By_ARCHIVE_Transfer_Transfer_ID_FK]");
                Boolean success;
                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("transfer_ID", transfer_ID));
                //righe modificate nel db
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(rowsAffected);
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoreProcedure("sp_ARCHIVE_Delete_TransferState_By_ARCHIVE_Transfer_Transfer_ID_FK", sp_params);
                    success = ((Int32)rowsAffected.Valore > 0);
                    return success;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: DeleteARCHIVE_TransferStateByTransfer_ID", ex);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_TransferState.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_TransferState(Int32 transfer_ID, Int32 transferStateType_ID, Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Update_TransferState_PK]");
                Boolean success;

                ArrayList sp_params = new ArrayList();
                //righe modificate nel db
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(rowsAffected);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@transfer_ID", transfer_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@transferStateType_ID", transferStateType_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_TransferState_PK", sp_params, ds);
                }

                success = (Int32)rowsAffected.Valore > 0;
                return success;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_TransferState", e);
                return false;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_TransferState if the original data has not changed.
        /// </summary>
        /// <param name="transfer_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'transfer_ID_Original'. </param>
        /// <param name="transferStateType_ID_Original">This field is used for optimistic concurrency management. It should contain the original value of 'transferStateType_ID_Original'. </param>
        /// <param name="dateTime_Original">This field is used for optimistic concurrency management. It should contain the original value of 'dateTime_Original'. </param>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_TransferState(Int32 transfer_ID, Int32 transfer_ID_Original, Int32 transferStateType_ID, Int32 transferStateType_ID_Original, Int32 system_ID)
        {
            try
            {
                logger.Debug("[sp_ARCHIVE_Update_TransferState]");
                Boolean success;

                ArrayList sp_params = new ArrayList();
                //righe modificate nel db
                DocsPaUtils.Data.ParameterSP rowsAffected = new DocsPaUtils.Data.ParameterSP("@RowsAffected", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                sp_params.Add(rowsAffected);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@transfer_ID", transfer_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@transfer_ID_Original", transfer_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferStateType_ID", transferStateType_ID));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferStateType_ID_Original", transferStateType_ID_Original));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
                DataSet ds = new DataSet();
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_TransferState", sp_params, ds);
                }

                success = (Int32)rowsAffected.Valore > 0;
                return success;
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DiagrammiStato  - metodo: UpdateARCHIVE_Transfer", e);
                return false;
            }
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_TransferState to the database.
        /// </summary>
        /// <param name="system_ID">Returns the value of system_ID. The database automatically generates this value. </param>
        public void InsertARCHIVE_TransferState(Int32 transfer_ID, Int32 transferStateType_ID, ref Int32 system_ID)
        {
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", transfer_ID));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@TransferStateType_ID", transferStateType_ID));
            DocsPaUtils.Data.ParameterSP System_ID = new DocsPaUtils.Data.ParameterSP("@System_ID", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
            sp_params.Add(System_ID);
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Insert_TransferState", sp_params, ds);
            }
            system_ID = (Int32)System_ID.Valore;
        }

        #endregion ARCHIVE_TransferState

        #region ARCHIVE_TransferStateType
        /// <summary>
        /// Returns all instances of ARCHIVE_TransferStateType.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferStateType. </results>
        public List<ARCHIVE_TransferStateType> GetAllARCHIVE_TransferStateType()
        {
            List<ARCHIVE_TransferStateType> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TransferStateType_All]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TransferStateType_All", sp_params, ds);
                dataList = new List<ARCHIVE_TransferStateType>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int nameOrdinal = dr.GetOrdinal("Name");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_TransferStateType(dr.GetInt32(system_IDOrdinal),
                        dr.GetString(nameOrdinal)));
                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Returns an instance of List<ARCHIVE_TransferStateType>.
        /// </summary>
        /// <results>Returns a strongly typed list of ARCHIVE_TransferStateType. </results>
        public List<ARCHIVE_TransferStateType> GetARCHIVE_TransferStateTypeBySystem_ID(Int32 system_ID)
        {

            List<ARCHIVE_TransferStateType> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TransferStateType_PK]");
            DataSet ds = new DataSet();

            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("system_ID", system_ID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Select_TransferStateType_PK", sp_params, ds);
                dataList = new List<ARCHIVE_TransferStateType>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_IDOrdinal = dr.GetOrdinal("System_ID");
                int nameOrdinal = dr.GetOrdinal("Name");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_TransferStateType(dr.GetInt32(system_IDOrdinal),
                        dr.GetString(nameOrdinal)));
                }

                dr.Close();
                ds.Dispose();
                return dataList;
            }
        }
        /// <summary>
        /// Deletes an instance of ARCHIVE_TransferStateType based on System_ID.
        /// </summary>
        /// <results>'true' if the instance was deleted, otherwise, 'false'. </results>
        public Boolean DeleteARCHIVE_TransferStateType(Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_Delete_TransferStateType_PK]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("system_ID", system_ID));
            int rowsAffected = 0;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {

                dbProvider.ExecuteNonQuery("sp_ARCHIVE_Delete_TransferStateType_PK", out rowsAffected);
                success = (rowsAffected > 0);
                return success;
            }
        }
        /// <summary>
        /// Deletes all instances of ARCHIVE_TransferStateType.
        /// </summary>
        /// <results>The number of items deleted. </results>
        public Int32 DeleteAllARCHIVE_TransferStateType()
        {
            logger.Debug("[sp_ARCHIVE_Delete_TransferStateType_All]");
            int rowsAffected = 0;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteNonQuery("sp_ARCHIVE_Delete_TransferStateType_All", out rowsAffected);
                return rowsAffected;
            }
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_TransferStateType.
        /// </summary>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_TransferStateType(String name, Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_Update_TransferStateType_PK]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Name", name));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_TransferStateType_PK", sp_params, ds);
            }

            success = (ds.Tables[0].Rows.Count > 0);
            return success;
        }
        /// <summary>
        /// Updates an instance of ARCHIVE_TransferStateType if the original data has not changed.
        /// </summary>
        /// <param name="name_Original">This field is used for optimistic concurrency management. It should contain the original value of 'name_Original'. </param>
        /// <results>'true' if the instance was updated, otherwise, 'false'. </results>
        public Boolean UpdateARCHIVE_TransferStateType(String name, String name_Original, Int32 system_ID)
        {
            logger.Debug("[sp_ARCHIVE_Update_TransferStateType]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Name", name));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Name_Original", name_Original));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Update_TransferStateType", sp_params, ds);
            }

            success = (ds.Tables[0].Rows.Count > 0);
            return success;
        }
        /// <summary>
        /// Persists a new instance of ARCHIVE_TransferStateType to the database.
        /// </summary>
        public Boolean InsertARCHIVE_TransferStateType(Int32 system_ID, String name)
        {
            logger.Debug("[sp_ARCHIVE_Insert_TransferStateType]");
            Boolean success;
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Name", name));
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@System_ID", system_ID));
            DataSet ds = new DataSet();
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_Insert_TransferStateType", sp_params, ds);
            }

            success = (ds.Tables[0].Rows.Count > 0);
            return success;
        }

        #endregion

        #region ARCHIVE_View_Policy
        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_Documents_Policy>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Documents_Policy. </results>
        public List<ARCHIVE_View_Policy> GetAllARCHIVE_View_Policy(Int32 transferID)
        {
            List<ARCHIVE_View_Policy> dataList;
            logger.Debug("[sp_ARCHIVE_GetTransferPolicyStateByTransferID]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@transferID", transferID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_GetTransferPolicyStateByTransferID", sp_params, ds);
                dataList = new List<ARCHIVE_View_Policy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int id_policyOrdinal = dr.GetOrdinal("ID_POLICY");
                int descrizioneOrdinal = dr.GetOrdinal("DESCRIZIONE");
                int enabledOrdinal = dr.GetOrdinal("ENABLED");
                int statoOrdinal = dr.GetOrdinal("STATO");
                int id_statoOrdinal = dr.GetOrdinal("ID_STATO");
                int totale_fascicoliOrdinal = dr.GetOrdinal("TOTALE_FASCICOLI");
                int totale_documentiOrdinal = dr.GetOrdinal("TOTALE_DOCUMENTI");
                int num_documenti_trasferitiOrdinal = dr.GetOrdinal("NUM_DOCUMENTI_TRASFERITI");
                int num_documenti_copiatiOrdinal = dr.GetOrdinal("NUM_DOCUMENTI_COPIATI");


                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_View_Policy(
                        dr.GetInt32(id_policyOrdinal),
                        dr.GetString(descrizioneOrdinal),
                        dr.GetInt32(enabledOrdinal),
                        dr.GetString(statoOrdinal),
                        dr.GetInt32(id_statoOrdinal),
                        dr.GetInt32(totale_fascicoliOrdinal),
                        dr.GetInt32(totale_documentiOrdinal),
                        dr.GetInt32(num_documenti_trasferitiOrdinal),
                        dr.GetInt32(num_documenti_copiatiOrdinal)));
                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }

        #endregion

        #region ARCHIVE_View_Documents_Policy
        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_Documents_Policy>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Documents_Policy. </results>
        public List<ARCHIVE_View_Documents_Policy> GetAllARCHIVE_View_Documents_Policy(String transferPolicyList)
        {
            List<ARCHIVE_View_Documents_Policy> dataList;
            logger.Debug("[sp_ARCHIVE_BE_GetDocumentsByTransferPolicyList]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@transferPolicyList", transferPolicyList));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_GetDocumentsByTransferPolicyList", sp_params, ds);
                dataList = new List<ARCHIVE_View_Documents_Policy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int registro_IDOrdinal = dr.GetOrdinal("Registro");
                int titolarioOrdinal = dr.GetOrdinal("Titolario");
                int classetitolarioOrdinal = dr.GetOrdinal("Classetitolario");
                int tipologiaOrdinal = dr.GetOrdinal("Tipologia");
                int annoCreazioneOrdinal = dr.GetOrdinal("Anno_Creazione");
                int totale = dr.GetOrdinal("Totale");
                int countDistinct = dr.GetOrdinal("CountDistinct");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_View_Documents_Policy(
                         dr.IsDBNull(registro_IDOrdinal) ? null : dr.GetString(registro_IDOrdinal),
                         dr.IsDBNull(titolarioOrdinal) ? null : dr.GetString(titolarioOrdinal),
                         dr.IsDBNull(classetitolarioOrdinal) ? null : dr.GetString(classetitolarioOrdinal),
                         dr.IsDBNull(tipologiaOrdinal) ? null : dr.GetString(tipologiaOrdinal),
                         dr.IsDBNull(annoCreazioneOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoCreazioneOrdinal)),
                         dr.IsDBNull(totale) ? null : new Nullable<Int32>(dr.GetInt32(totale)),
                         dr.GetInt32(countDistinct)));

                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }

        #endregion

        #region ARCHIVE_View_Projects_Policy

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_Projects_Policy>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_View_Projects_Policy. </results>
        public List<ARCHIVE_View_Projects_Policy> GetAllARCHIVE_View_Projects_Policy(String transferPolicyList)
        {
            List<ARCHIVE_View_Projects_Policy> dataList;
            logger.Debug("[sp_ARCHIVE_BE_GetProjectsByTransferPolicyList]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@transferPolicyList", transferPolicyList));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_GetProjectsByTransferPolicyList", sp_params, ds);
                dataList = new List<ARCHIVE_View_Projects_Policy>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int registro_IDOrdinal = dr.GetOrdinal("Registro");
                int titolarioOrdinal = dr.GetOrdinal("Titolario");
                int classetitolarioOrdinal = dr.GetOrdinal("Classetitolario");
                int tipologiaOrdinal = dr.GetOrdinal("Tipologia");
                int annoChiusuraOrdinal = dr.GetOrdinal("Anno_Chiusura");
                int totale = dr.GetOrdinal("Totale");
                int countDistinct = dr.GetOrdinal("CountDistinct");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_View_Projects_Policy(
                        dr.IsDBNull(registro_IDOrdinal) ? null : dr.GetString(registro_IDOrdinal),
                         dr.IsDBNull(titolarioOrdinal) ? null : dr.GetString(titolarioOrdinal),
                         dr.IsDBNull(classetitolarioOrdinal) ? null : dr.GetString(classetitolarioOrdinal),
                         dr.IsDBNull(tipologiaOrdinal) ? null : dr.GetString(tipologiaOrdinal),
                         dr.IsDBNull(annoChiusuraOrdinal) ? null : new Nullable<Int32>(dr.GetInt32(annoChiusuraOrdinal)),
                          dr.IsDBNull(totale) ? null : new Nullable<Int32>(dr.GetInt32(totale)),
                          dr.GetInt32(countDistinct)));
                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }

        #endregion

        #region ARCHIVE_Result_Trandfer_Policy

        /// <summary>
        /// Returns an instance of List<ARCHIVE_Result_Transfer>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_Result_Transfer. </results>
        public List<ARCHIVE_Result_Transfer> GetAllARCHIVE_Result_Transfer(String transferResultList)
        {
            List<ARCHIVE_Result_Transfer> dataList;
            logger.Debug("[sp_ARCHIVE_BE_GetTransferStateByTransferList]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@transferList", transferResultList));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("sp_ARCHIVE_BE_GetTransferStateByTransferList", sp_params, ds);
                dataList = new List<ARCHIVE_Result_Transfer>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int system_id_VersamentoOrdinal = dr.GetOrdinal("System_id_Versamento");
                int descrizioneOrdinal = dr.GetOrdinal("Descrizione");
                int statoOrdinal = dr.GetOrdinal("Stato");
                int dataEsecuzioneOrdinal = dr.GetOrdinal("DataEsecuzione");
                int totale_documentiOrdinal = dr.GetOrdinal("Totale_documenti");
                int num_documenti_trasferitiOrdinal = dr.GetOrdinal("Num_documenti_trasferiti");
                int num_documenti_copiatiOrdinal = dr.GetOrdinal("Num_documenti_copiati");



                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_Result_Transfer(
                       dr.GetInt32(system_id_VersamentoOrdinal),
                       dr.GetString(descrizioneOrdinal),
                       dr.GetString(statoOrdinal),
                       dr.GetDateTime(dataEsecuzioneOrdinal),
                       dr.GetInt32(totale_documentiOrdinal),
                       dr.GetInt32(num_documenti_trasferitiOrdinal),
                       dr.GetInt32(num_documenti_copiatiOrdinal)
                       ));
                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }

        #endregion

        #region ARCHIVE_View_FascReport

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_FascReport>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_View_FascReport. </results>
        public List<ARCHIVE_View_FascReport> GetARCHIVE_View_ARCHIVE_FascReportByTransferID(Int32 transferID)
        {
            List<ARCHIVE_View_FascReport> dataList;
            logger.Debug("[[sp_ARCHIVE_Select_TempProjectViewByTransferID]]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", transferID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("[sp_ARCHIVE_Select_TempProjectViewByTransferID]", sp_params, ds);
                dataList = new List<ARCHIVE_View_FascReport>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int RegistroOrdinal = dr.GetOrdinal("Registro");
                int UOOrdinal = dr.GetOrdinal("UO");
                int Project_IDOrdinal = dr.GetOrdinal("Project_ID");
                int DESCRIPTIONOrdinal = dr.GetOrdinal("DESCRIPTION");
                int DTA_CreazioneOrdinal = dr.GetOrdinal("DTA_Creazione");
                int DTA_CHIUSURAOrdinal = dr.GetOrdinal("DTA_CHIUSURA");
                int TipologiaOrdinal = dr.GetOrdinal("Tipologia");
                int TipoTrasferimento_VersamentoOrdinal = dr.GetOrdinal("TipoTrasferimento_Versamento");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_View_FascReport(
                        dr.IsDBNull(RegistroOrdinal) ? String.Empty : dr.GetString(RegistroOrdinal),
                        dr.IsDBNull(UOOrdinal) ? String.Empty : dr.GetString(UOOrdinal),
                        dr.IsDBNull(DESCRIPTIONOrdinal) ? String.Empty : dr.GetString(DESCRIPTIONOrdinal),
                        dr.GetInt32(Project_IDOrdinal),
                        dr.IsDBNull(DTA_CreazioneOrdinal) ? (DateTime?)null : dr.GetDateTime(DTA_CreazioneOrdinal),
                        dr.IsDBNull(DTA_CHIUSURAOrdinal) ? (DateTime?)null : dr.GetDateTime(DTA_CHIUSURAOrdinal),
                        dr.IsDBNull(TipologiaOrdinal) ? String.Empty : dr.GetString(TipologiaOrdinal),
                        dr.IsDBNull(TipoTrasferimento_VersamentoOrdinal) ? String.Empty : dr.GetString(TipoTrasferimento_VersamentoOrdinal)));

                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }

        #endregion

        #region ARCHIVE_View_DocReport

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_FascReport>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_View_FascReport. </results>
        public List<ARCHIVE_View_DocReport> GetARCHIVE_View_ARCHIVE_DocReportByTransferID(Int32 transferID)
        {
            List<ARCHIVE_View_DocReport> dataList;
            logger.Debug("[[sp_ARCHIVE_Select_TempProfileViewByTransferID]]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Transfer_ID", transferID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("[sp_ARCHIVE_Select_TempProfileViewByTransferID]", sp_params, ds);
                dataList = new List<ARCHIVE_View_DocReport>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int DocIDOrdinal = dr.GetOrdinal("DocID");
                int UOOrdinal = dr.GetOrdinal("UO");
                int NUM_PROTOOrdinal = dr.GetOrdinal("NUM_PROTO");
                int NUM_ANNO_PROTOOrdinal = dr.GetOrdinal("NUM_ANNO_PROTO");
                int RegistroOrdinal = dr.GetOrdinal("Registro");
                int OggettoDocumentoOrdinal = dr.GetOrdinal("OggettoDocumento");
                int TipoDocumentoOrdina = dr.GetOrdinal("TipoDocumento");
                int DataCreazioneOrdinal = dr.GetOrdinal("DataCreazione");
                int TipologiaOrdinal = dr.GetOrdinal("Tipologia");
                int TipoTrasferimento_VersamentoOrdinal = dr.GetOrdinal("TipoTrasferimento_Versamento");
                int CodeOrdinal = dr.GetOrdinal("Code");
                int CorrOrdinal = dr.GetOrdinal("Corr");


                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_View_DocReport(
                        dr.IsDBNull(RegistroOrdinal) ? String.Empty : dr.GetString(RegistroOrdinal),
                        dr.IsDBNull(UOOrdinal) ? String.Empty : dr.GetString(UOOrdinal),
                        dr.GetInt32(DocIDOrdinal),
                        dr.IsDBNull(CodeOrdinal) ? String.Empty : dr.GetString(CodeOrdinal),
                        dr.IsDBNull(NUM_PROTOOrdinal) ? (Int32?)null : dr.GetInt32(NUM_PROTOOrdinal),
                        dr.IsDBNull(OggettoDocumentoOrdinal) ? String.Empty : dr.GetString(OggettoDocumentoOrdinal),
                        dr.IsDBNull(DataCreazioneOrdinal) ? (DateTime?)null : dr.GetDateTime(DataCreazioneOrdinal),
                        dr.IsDBNull(NUM_ANNO_PROTOOrdinal) ? (Int32?)null : dr.GetInt32(NUM_ANNO_PROTOOrdinal),
                        dr.IsDBNull(TipoDocumentoOrdina) ? String.Empty : dr.GetString(TipoDocumentoOrdina),
                        dr.IsDBNull(TipologiaOrdinal) ? String.Empty : dr.GetString(TipologiaOrdinal),
                        dr.IsDBNull(TipoTrasferimento_VersamentoOrdinal) ? String.Empty : dr.GetString(TipoTrasferimento_VersamentoOrdinal),
                        dr.IsDBNull(CorrOrdinal) ? String.Empty : dr.GetString(CorrOrdinal)));

                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }

        #endregion

        #region ARCHIVE_View_FascReportDisposal

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_FascReportDisposal>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_View_FascReportDisposal. </results>
        public List<ARCHIVE_View_FascReportDisposal> GetARCHIVE_View_ARCHIVE_FascReportByDisposalID(Int32 DisposalID)
        {
            List<ARCHIVE_View_FascReportDisposal> dataList;
            logger.Debug("[sp_ARCHIVE_Select_TempProjectViewByDisposalID]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Disposal_ID", DisposalID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("[sp_ARCHIVE_Select_TempProjectViewByDisposalID]", sp_params, ds);
                dataList = new List<ARCHIVE_View_FascReportDisposal>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int Project_IDOrdinal = dr.GetOrdinal("PROJECT_ID");
                int DTA_CreazioneOrdinal = dr.GetOrdinal("DTA_CREAZIONE");
                int DTA_CHIUSURAOrdinal = dr.GetOrdinal("DTA_CHIUSURA");
                int RegistroOrdinal = dr.GetOrdinal("REGISTRO");
                int DESCRIPTIONOrdinal = dr.GetOrdinal("DESCRIPTION");
                int UOOrdinal = dr.GetOrdinal("UO");
                int TipologiaOrdinal = dr.GetOrdinal("TIPOLOGIA");
                int ScartareOrdinal = dr.GetOrdinal("DASCARTARE");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_View_FascReportDisposal(
                    dr.IsDBNull(RegistroOrdinal) ? String.Empty : dr.GetString(RegistroOrdinal),
                    dr.IsDBNull(UOOrdinal) ? String.Empty : dr.GetString(UOOrdinal),
                    dr.IsDBNull(DESCRIPTIONOrdinal) ? String.Empty : dr.GetString(DESCRIPTIONOrdinal),
                    dr.GetInt32(Project_IDOrdinal),
                    dr.IsDBNull(DTA_CreazioneOrdinal) ? (DateTime?)null : dr.GetDateTime(DTA_CreazioneOrdinal),
                    dr.IsDBNull(DTA_CHIUSURAOrdinal) ? (DateTime?)null : dr.GetDateTime(DTA_CHIUSURAOrdinal),
                    dr.IsDBNull(TipologiaOrdinal) ? String.Empty : dr.GetString(TipologiaOrdinal),
                    String.Empty,
                    dr.GetInt32(ScartareOrdinal)));
                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }

        #endregion

        #region ARCHIVE_View_DocReportDisposal

        /// <summary>
        /// Returns an instance of List<ARCHIVE_View_FascReportDisposal>.
        /// </summary>
        /// <param name="system_ID">The database automatically generates this value. </param>
        /// <results>Returns a strongly typed list of ARCHIVE_View_FascReportDisposal. </results>
        public List<ARCHIVE_View_DocReportDisposal> GetARCHIVE_View_ARCHIVE_DocReportByDisposalID(Int32 DisposalID)
        {
            List<ARCHIVE_View_DocReportDisposal> dataList;
            logger.Debug("[[sp_ARCHIVE_Select_TempProfileViewByDisposalID]]");
            DataSet ds = new DataSet();
            ArrayList sp_params = new ArrayList();
            sp_params.Add(new DocsPaUtils.Data.ParameterSP("@Disposal_ID", DisposalID));

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteStoredProcedure("[sp_ARCHIVE_Select_TempProfileViewByDisposalID]", sp_params, ds);
                dataList = new List<ARCHIVE_View_DocReportDisposal>();

                DataTableReader dr = ds.Tables[0].CreateDataReader();

                int DocIDOrdinal = dr.GetOrdinal("DOCID");
                int NUM_PROTOOrdinal = dr.GetOrdinal("NUM_PROTO");
                int NUM_ANNO_PROTOOrdinal = dr.GetOrdinal("NUM_ANNO_PROTO");
                int RegistroOrdinal = dr.GetOrdinal("REGISTRO");
                int UOOrdinal = dr.GetOrdinal("UO");
                int OggettoDocumentoOrdinal = dr.GetOrdinal("OggettoDocumento");
                int TipoDocumentoOrdina = dr.GetOrdinal("TipoDocumento");
                int DataCreazioneOrdinal = dr.GetOrdinal("DataCreazione");
                int TipologiaOrdinal = dr.GetOrdinal("Tipologia");
                int CodeOrdinal = dr.GetOrdinal("CODE");
                int CorrOrdinal = dr.GetOrdinal("CORR");
                int ScartareOrdinal = dr.GetOrdinal("DASCARTARE");

                while (dr.Read())
                {
                    dataList.Add(new ARCHIVE_View_DocReportDisposal(
                        dr.IsDBNull(RegistroOrdinal) ? String.Empty : dr.GetString(RegistroOrdinal),
                        dr.IsDBNull(UOOrdinal) ? String.Empty : dr.GetString(UOOrdinal),
                        dr.GetInt32(DocIDOrdinal),
                        dr.IsDBNull(CodeOrdinal) ? String.Empty : dr.GetString(CodeOrdinal),
                        dr.IsDBNull(NUM_PROTOOrdinal) ? (Int32?)null : dr.GetInt32(NUM_PROTOOrdinal),
                        dr.IsDBNull(OggettoDocumentoOrdinal) ? String.Empty : dr.GetString(OggettoDocumentoOrdinal),
                        dr.IsDBNull(DataCreazioneOrdinal) ? (DateTime?)null : dr.GetDateTime(DataCreazioneOrdinal),
                        dr.IsDBNull(NUM_ANNO_PROTOOrdinal) ? (Int32?)null : dr.GetInt32(NUM_ANNO_PROTOOrdinal),
                        dr.IsDBNull(TipoDocumentoOrdina) ? String.Empty : dr.GetString(TipoDocumentoOrdina),
                        dr.IsDBNull(TipologiaOrdinal) ? String.Empty : dr.GetString(TipologiaOrdinal),
                        String.Empty,
                        dr.IsDBNull(CorrOrdinal) ? String.Empty : dr.GetString(CorrOrdinal),
                        dr.GetInt32(ScartareOrdinal)));
                }

                dr.Close();

                ds.Dispose();
                return dataList;
            }
        }

        #endregion
    }
}
